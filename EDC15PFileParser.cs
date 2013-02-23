// Todo: find cowFUN_AGR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    public class EDC15PFileParser : IEDCFileParser
    {

        public override string ExtractInfo(byte[] allBytes)
        {
            // assume info will be @ 0x53452 12 bytes
            string retval = string.Empty;
            try
            {
                int partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[5] { 0x45, 0x44, 0x43, 0x20, 0x20 }, new byte[5] { 1, 1, 1, 1, 1 });
                if (partnumberAddress > 0)
                {
                    retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress - 8, 12).Trim();
                }
            }
            catch (Exception)
            {
            }
            return retval;
        }

        public override string ExtractPartnumber(byte[] allBytes)
        {
            // assume info will be @ 0x53446 12 bytes
            string retval = string.Empty;
            try
            {
                int partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[5] { 0x45, 0x44, 0x43, 0x20, 0x20 }, new byte[5] { 1, 1, 1, 1, 1 });
                if (partnumberAddress > 0)
                {
                    retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress - 20, 12).Trim();
                }
            }
            catch (Exception)
            {
            }
            return retval;
        }

        public override string ExtractSoftwareNumber(byte[] allBytes)
        {
            string retval = string.Empty;
            try
            {
                int partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[5] { 0x45, 0x44, 0x43, 0x20, 0x20 }, new byte[5] { 1, 1, 1, 1, 1 });
                if (partnumberAddress > 0)
                {
                    retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress + 5, 8).Trim();
                    retval = retval.Replace(" ", "");
                }
            }
            catch (Exception)
            {
            }
            return retval;
        }

        public override string ExtractBoschPartnumber(byte[] allBytes)
        {
            return Tools.Instance.ExtractBoschPartnumber(allBytes);
            string retval = string.Empty;
            try
            {
                int partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[5] { 0x45, 0x44, 0x43, 0x20, 0x20 }, new byte[5] { 1, 1, 1, 1, 1 });
                if (partnumberAddress > 0)
                {
                    retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress + 23, 10).Trim();
                }
            }
            catch (Exception)
            {
            }
            return retval;
        }

        private string DetermineNumberByFlashBank(long address, List<CodeBlock> currBlocks)
        {
            foreach (CodeBlock cb in currBlocks)
            {
                if (cb.StartAddress <= address && cb.EndAddress >= address)
                {
                  //  if (cb.CodeID == 1) return "codeblock 1";// - MAN";
                  //  if (cb.CodeID == 2) return "codeblock 2";// - AUT (hydr)";
                  //  if (cb.CodeID == 3) return "codeblock 3";// - AUT (elek)";
                  //  return cb.CodeID.ToString();
                    if (cb.BlockGearboxType == GearboxType.Automatic)
                    {
                        return "codeblock " + cb.CodeID.ToString() + ", automatic";
                    }
                    else if (cb.CodeID == 2) return "codeblock " + cb.CodeID.ToString() + ", manual";
                    else if (cb.CodeID == 3) return "codeblock " + cb.CodeID.ToString() + ", 4x4";
                    return "codeblock " + cb.CodeID.ToString();
                }
            }
            long bankNumber = address / 0x10000;
            return "flashbank " + bankNumber.ToString();
        }

        private int DetermineCodeBlockByByAddress(long address, List<CodeBlock> currBlocks)
        {
            foreach (CodeBlock cb in currBlocks)
            {
                if (cb.StartAddress <= address && cb.EndAddress >= address)
                {
                    return cb.CodeID;
                }
            }
            return 0;
        }


        public override SymbolCollection parseFile(string filename, out List<CodeBlock> newCodeBlocks, out List<AxisHelper> newAxisHelpers)
        {
            newCodeBlocks = new List<CodeBlock>();
            SymbolCollection newSymbols = new SymbolCollection();
            newAxisHelpers = new List<AxisHelper>();
            byte[] allBytes = File.ReadAllBytes(filename);
            string boschnumber = ExtractBoschPartnumber(allBytes);
            string softwareNumber = ExtractSoftwareNumber(allBytes);
            partNumberConverter pnc = new partNumberConverter();

            VerifyCodeBlocks(allBytes, newSymbols, newCodeBlocks);

            for (int t = 0; t < allBytes.Length - 1; t += 2)
            {
                int len2skip = 0;
                //if (t == 0x4dc26) Console.WriteLine("ho");
                if (CheckMap(t, allBytes, newSymbols, newCodeBlocks, out len2skip))
                {
                    int from = t;
                    if (len2skip > 2) len2skip -= 2; // make sure we don't miss maps
                    if ((len2skip % 2) > 0) len2skip -= 1;
                    if (len2skip < 0) len2skip = 0;
                    t += len2skip;
                    /*                    if (from > 0x4dc00 && from < 0x4dd00)
                                        {
                                           // Console.WriteLine("map detected: " + from.ToString("X8") + " - " + t.ToString("X8") + " len: " + len2skip.ToString("X8"));
                                        }*/
                }
            }

            newSymbols.SortColumn = "Flash_start_address";
            newSymbols.SortingOrder = GenericComparer.SortOrder.Ascending;
            newSymbols.Sort();
            NameKnownMaps(allBytes, newSymbols, newCodeBlocks);

            BuildAxisIDList(newSymbols, newAxisHelpers);
            MatchAxis(newSymbols, newAxisHelpers);

            RemoveNonSymbols(newSymbols, newCodeBlocks);
            FindSVBL(allBytes, filename, newSymbols, newCodeBlocks);
            SymbolTranslator strans = new SymbolTranslator();
            foreach (SymbolHelper sh in newSymbols)
            {
                sh.Description = strans.TranslateSymbolToHelpText(sh.Varname);
            }
            // check for must have maps... if there are maps missing, report it
            return newSymbols;
        }

        private void MatchAxis(SymbolCollection newSymbols, List<AxisHelper> newAxisHelpers)
        {
            foreach (SymbolHelper sh in newSymbols)
            {
                if (!sh.YaxisAssigned)
                {
                    foreach (AxisHelper ah in newAxisHelpers)
                    {
                        if (sh.X_axis_ID == ah.AxisID)
                        {
                            sh.Y_axis_descr = ah.Description;
                            sh.YaxisUnits = ah.Units;
                            sh.Y_axis_offset = ah.Offset;
                            sh.Y_axis_correction = ah.Correction;
                            break;
                        }
                    }
                }
                if (!sh.XaxisAssigned)
                {
                    foreach (AxisHelper ah in newAxisHelpers)
                    {
                        if (sh.Y_axis_ID == ah.AxisID)
                        {
                            sh.X_axis_descr = ah.Description;
                            sh.XaxisUnits = ah.Units;
                            sh.X_axis_offset = ah.Offset;
                            sh.X_axis_correction = ah.Correction;
                            break;
                        }
                    }
                }

            }
        }

        private void BuildAxisIDList(SymbolCollection newSymbols, List<AxisHelper> newAxisHelpers)
        {
            foreach (SymbolHelper sh in newSymbols)
            {
                if (!sh.Varname.StartsWith("2D") && !sh.Varname.StartsWith("3D"))
                {
                    AddToAxisCollection(newAxisHelpers, sh.Y_axis_ID, sh.X_axis_descr, sh.XaxisUnits, sh.X_axis_correction, sh.X_axis_offset);
                    AddToAxisCollection(newAxisHelpers, sh.X_axis_ID, sh.Y_axis_descr, sh.YaxisUnits, sh.Y_axis_correction, sh.Y_axis_offset);
                }
            }
        }

        private void AddToAxisCollection(List<AxisHelper> newAxisHelpers, int ID, string descr, string units, double correction, double offset)
        {
            if (ID == 0) return;
            foreach (AxisHelper ah in newAxisHelpers)
            {
                if (ah.AxisID == ID) return;
            }
            AxisHelper ahnew = new AxisHelper();
            ahnew.AxisID = ID;
            ahnew.Description = descr;
            ahnew.Units = units;
            ahnew.Correction = correction;
            ahnew.Offset = offset;
            newAxisHelpers.Add(ahnew);
        }

        private void RemoveNonSymbols(SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            if (newCodeBlocks.Count > 0)
            {
                foreach (SymbolHelper sh in newSymbols)
                {
                    if (sh.CodeBlock == 0 && (sh.Varname.StartsWith("2D") || sh.Varname.StartsWith("3D")))
                    {
                        sh.Subcategory = "Zero codeblock stuff";

                    }
                }
            }
        }

        public override void FindSVBL(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            if (!FindSVBLSequenceOne(allBytes, filename, newSymbols, newCodeBlocks))
            {
                FindSVBLSequenceTwo(allBytes, filename, newSymbols, newCodeBlocks);
            }
        }

        private bool FindSVBLSequenceTwo(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            bool found = true;
            bool SVBLFound = false;
            int offset = 0;
            while (found)
            {

                int SVBLAddress = Tools.Instance.findSequence(allBytes, offset, new byte[10] { 0xDF, 0x7A, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDF, 0x7A }, new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                if (SVBLAddress > 0)
                {
                    SVBLFound = true;
                    SymbolHelper shsvbl = new SymbolHelper();
                    shsvbl.Category = "Detected maps";
                    shsvbl.Subcategory = "Limiters";
                    shsvbl.Flash_start_address = SVBLAddress - 2;
                    //shsvbl.Flash_start_address = SVBLAddress + 16;

                    // if value = 0xC3 0x00 -> two more back
                    int[] testValue = Tools.Instance.readdatafromfileasint(filename, (int)shsvbl.Flash_start_address, 1, EDCFileType.EDC15P);
                    if (testValue[0] == 0xC300) shsvbl.Flash_start_address -= 2;

                    shsvbl.Varname = "SVBL Boost limiter [" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks) + "]";
                    shsvbl.Length = 2;
                    shsvbl.CodeBlock = DetermineCodeBlockByByAddress(shsvbl.Flash_start_address, newCodeBlocks);
                    newSymbols.Add(shsvbl);

                    int MAPMAFSwitch = Tools.Instance.findSequence(allBytes, SVBLAddress - 0x100, new byte[8] { 0x41, 0x02, 0xFF, 0xFF, 0x00, 0x01, 0x01, 0x00 }, new byte[8] { 1, 1, 0, 0, 1, 1, 1, 1 });
                    if (MAPMAFSwitch > 0)
                    {
                        MAPMAFSwitch += 2;
                        SymbolHelper mapmafsh = new SymbolHelper();
                        //mapmafsh.BitMask = 0x0101;
                        mapmafsh.Category = "Detected maps";
                        mapmafsh.Subcategory = "Switches";
                        mapmafsh.Flash_start_address = MAPMAFSwitch;
                        mapmafsh.Varname = "MAP/MAF switch (0 = MAF, 257/0x101 = MAP)" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks);
                        mapmafsh.Length = 2;
                        mapmafsh.CodeBlock = DetermineCodeBlockByByAddress(mapmafsh.Flash_start_address, newCodeBlocks);
                        newSymbols.Add(mapmafsh);
                       //Console.WriteLine("Found MAP MAF switch @ " + MAPMAFSwitch.ToString("X8"));
                    }


                    offset = SVBLAddress + 1;
                }
                else found = false;
            }
            return SVBLFound;
        }

        private bool FindSVBLSequenceOne(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            bool found = true;
            bool SVBLFound = false;
            int offset = 0;
            while (found)
            {
                //int SVBLAddress = Tools.Instance.findSequence(allBytes, offset, new byte[10] { 0xDF, 0x7A, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDF, 0x7A }, new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                int SVBLAddress = Tools.Instance.findSequence(allBytes, offset, new byte[16] { 0xD2, 0x00, 0xFC, 0x03, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0xFF, 0xFF, 0xFF, 0xC3, 0x00, 0x00 }, new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 });

                if (SVBLAddress > 0)
                {
                    //Console.WriteLine("Alternative SVBL " + SVBLAddress.ToString("X8"));
                    SVBLFound = true;
                    SymbolHelper shsvbl = new SymbolHelper();
                    shsvbl.Category = "Detected maps";
                    shsvbl.Subcategory = "Limiters";
                    //shsvbl.Flash_start_address = SVBLAddress - 2;
                    shsvbl.Flash_start_address = SVBLAddress + 16;

                    // if value = 0xC3 0x00 -> two more back
                    int[] testValue = Tools.Instance.readdatafromfileasint(filename, (int)shsvbl.Flash_start_address, 1, EDCFileType.EDC15P);
                    if (testValue[0] == 0xC300) shsvbl.Flash_start_address -= 2;

                    shsvbl.Varname = "SVBL Boost limiter [" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks) + "]";
                    shsvbl.Length = 2;
                    shsvbl.CodeBlock = DetermineCodeBlockByByAddress(shsvbl.Flash_start_address, newCodeBlocks);
                    newSymbols.Add(shsvbl);

                    int MAPMAFSwitch = Tools.Instance.findSequence(allBytes, SVBLAddress - 0x100, new byte[8] { 0x41, 0x02, 0xFF, 0xFF, 0x00, 0x01, 0x01, 0x00 }, new byte[8] { 1, 1, 0, 0, 1, 1, 1, 1 });
                    if (MAPMAFSwitch > 0)
                    {
                        MAPMAFSwitch += 2;
                        SymbolHelper mapmafsh = new SymbolHelper();
                        //mapmafsh.BitMask = 0x0101;
                        mapmafsh.Category = "Detected maps";
                        mapmafsh.Subcategory = "Switches";
                        mapmafsh.Flash_start_address = MAPMAFSwitch;
                        mapmafsh.Varname = "MAP/MAF switch (0 = MAF, 257/0x101 = MAP)" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks);
                        mapmafsh.Length = 2;
                        mapmafsh.CodeBlock = DetermineCodeBlockByByAddress(mapmafsh.Flash_start_address, newCodeBlocks);
                        newSymbols.Add(mapmafsh);
                        //Console.WriteLine("Found MAP MAF switch @ " + MAPMAFSwitch.ToString("X8"));
                    }


                    offset = SVBLAddress + 1;
                }

                else found = false;
            }
            return SVBLFound;
        }

        public override void NameKnownMaps(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            SymbolAxesTranslator st = new SymbolAxesTranslator();
            
            foreach (SymbolHelper sh in newSymbols)
            {
                //sh.X_axis_descr = st.TranslateAxisID(sh.X_axis_ID);
                //sh.Y_axis_descr = st.TranslateAxisID(sh.Y_axis_ID);
                if (sh.Length == 700) // 25*14
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Misc";
                    sh.Varname = "Launch control map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Y_axis_correction = 0.156250;
                    //sh.Y_axis_correction = 0.000039;
                    sh.Correction = 0.01;
                    sh.X_axis_descr = "Engine speed (rpm)";
                    //sh.Y_axis_descr = "Ratio vehicle/engine speed";
                    sh.Y_axis_descr = "Approx. vehicle speed (km/h)";
                    //sh.Z_axis_descr = "Output percentage";
                    sh.Z_axis_descr = "IQ limit";
                    sh.YaxisUnits = "km/h";
                    sh.XaxisUnits = "rpm";
                }

                if (sh.Length == 570)
                {
                    if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {

                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";
                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC4 && sh.Y_axis_ID / 256 == 0xEA)
                    {

                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC4 && sh.Y_axis_ID / 256 == 0xEC)
                    {

                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }
                }
                else if (sh.Length == 480)
                {
                    if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {

                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;
                        //IAT, ECT or Fuel temp?

                        double tempRange = GetTemperatureDurRange(injDurCount - 1);
                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";
                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC4 && sh.Y_axis_ID / 256 == 0xEA)
                    {

                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;
                        //IAT, ECT or Fuel temp?

                        double tempRange = GetTemperatureDurRange(injDurCount - 1);
                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Y_axis_correction = 0.01;
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";
                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }

                }
                else if (sh.Length == 448)
                {
                    if (sh.MapSelector.NumRepeats == 10)
                    {
                        // SOI maps detected
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Start of injection (SOI)", sh.CodeBlock, newSymbols, false);
                        
                        //based on coolant temperature
                        double tempRange = GetTemperatureSOIRange(sh.MapSelector, injDurCount - 1);
                        sh.Varname = "Start of injection (SOI) " + tempRange.ToString() + " °C [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");

                        sh.Correction = -0.023437;
                        sh.Offset = 78;

                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.X_axis_correction = 0.01; // TODODONE : Check for x or y
                        sh.XaxisUnits = "mg/st";

                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Z_axis_descr = "Start position (degrees BTDC)";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";

                    }
                }
                else if (sh.Length == 416)
                {
                    string strAddrTest = sh.Flash_start_address.ToString("X8");
                    if (sh.X_axis_ID / 256 == 0xF9 && sh.Y_axis_ID / 256 == 0xDA)
                    {
                        // this is IQ by MAF limiter!
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        int smokeCount = GetMapNameCountForCodeBlock("Smoke limiter", sh.CodeBlock, newSymbols, false);
                        //sh.Varname = "Smoke limiter " + smokeCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Varname = "Smoke limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        if (sh.MapSelector != null)
                        {
                            if (sh.MapSelector.MapIndexes != null)
                            {
                                if (sh.MapSelector.MapIndexes.Length > 1)
                                {
                                    if (!MapSelectorIndexEmpty(sh))
                                    {
                                        double tempRange = GetTemperatureSOIRange(sh.MapSelector, smokeCount - 1);
                                        sh.Varname = "Smoke limiter " + tempRange.ToString() + " °C [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                                    }
                                }
                            }
                        }
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.X_axis_descr = "Airflow mg/stroke";
                        sh.Correction = 0.01;
                        sh.X_axis_correction = 0.1; 
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";

                    }
                    else if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xDA)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        // if x axis = upto 3000 -> MAP limit, not MAF limit
                        if (GetMaxAxisValue(allBytes, sh, MapViewerEx.AxisIdent.Y_Axis) < 4000)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "IQ by MAP limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") +" " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.Correction = 0.01;
                            sh.X_axis_descr = "Boost pressure";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mbar";

                        }
                        else
                        {
                            int iqMAFLimCount = GetMapNameCountForCodeBlock("IQ by MAF limiter", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "IQ by MAF limiter " + iqMAFLimCount.ToString() + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            //sh.Varname = "IQ by MAF limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.1; 
                            sh.XaxisUnits = "mg/st";
                            sh.X_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                        }

                    }
                    else if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xEA)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";
                        sh.Varname = "N75 duty cycle [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Duty cycle %";
                        sh.Correction = -0.01;
                        sh.Offset = 100;
                        //sh.Correction = 0.01;
                        sh.X_axis_correction = 0.01; 
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    /*else if (strAddrTest.EndsWith("116"))
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                        sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Correction = 0.1;
                        sh.X_axis_correction = 0.01;
                        sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }*/
                    else if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID == 0xE9D4)
                    {
                        // x axis should start with 0!
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";
                        sh.Varname = "N75 duty cycle [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Duty cycle %";
                        sh.Correction = -0.01;
                        sh.Offset = 100;
                        //sh.Correction = 0.01;
                        sh.X_axis_correction = 0.01; 
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    else if ((sh.X_axis_ID / 256 == 0xEC) && (sh.Y_axis_ID / 256 == 0xC0 || sh.Y_axis_ID / 256 == 0xE9))
                    {
                        // x axis should start with 0!
                        if (allBytes[sh.Y_axis_address] == 0 && allBytes[sh.Y_axis_address + 1] == 0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                    else if (sh.X_axis_ID / 256 == 0xEA && sh.Y_axis_ID / 256 == 0xE9)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Start of injection (SOI)", sh.CodeBlock, newSymbols, false);
                        //IAT, ECT or Fuel temp?
                        double tempRange = GetTemperatureSOIRange(sh.MapSelector, injDurCount - 1);
                        sh.Varname = "Start of injection (SOI) " + tempRange.ToString() + " °C [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Correction = -0.023437;
                        sh.Offset = 78;

                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.X_axis_correction = 0.01; // TODODONE : Check for x or y
                        sh.XaxisUnits = "mg/st";

                        sh.Z_axis_descr = "Start position (degrees BTDC)";
                    }
                    else if (sh.X_axis_ID / 256 == 0xEA && sh.Y_axis_ID / 256 == 0xE8)
                    {
                        // EGR or N75
                        if (allBytes[sh.X_axis_address] == 0 && allBytes[sh.X_axis_address + 1] == 0)
                        {
                            // EGR
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";

                        }
                        else
                        {
                            //N75
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Turbo";
                            sh.Varname = "N75 duty cycle [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Duty cycle %";
                            sh.Correction = -0.01;
                            sh.Offset = 100;
                            //sh.Correction = 0.01;
                            sh.X_axis_correction = 0.01; 
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";

                        }
                    }
                   /* else if ((sh.X_axis_ID / 256 == 0xEA) && (sh.Y_axis_ID / 256 == 0xE8))
                    {
                        // x axis should start with 0!
                        if (allBytes[sh.Y_axis_address] == 0 && allBytes[sh.Y_axis_address + 1] == 0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }*/
                }
                else if (sh.Length == 390)
                {
                    // 15x12 = inj dur limiter on R3 files
                    if (sh.X_axis_length == 13 && sh.Y_axis_length == 15)
                    {
                           /* sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "Injection duration limiter B [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.Correction = 0.023438;
                            sh.Y_axis_correction = 0.01;
                            sh.Y_axis_descr = "IQ (mg/stroke)";
                            sh.Z_axis_descr = "Max. degrees";
                            sh.X_axis_descr = "Engine speed (rpm)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";*/
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            //if (injDurCount < 1) injDurCount = 1;

                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";                      
                    }

                    else if ((sh.X_axis_ID / 256 == 0xEC) && (sh.Y_axis_ID / 256 == 0xC0))
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                        sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Correction = 0.1;
                        sh.X_axis_correction = 0.01;
                        sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }

                }
                else if (sh.Length == 384)
                {
                    if (sh.X_axis_length == 12 && sh.Y_axis_length == 16)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        sh.Varname = "Inverse driver wish [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Correction = 0.01;
                        sh.X_axis_correction = 0.01;
                        sh.Z_axis_descr = "Throttle  position";
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        //sh.Z_axis_descr = "Requested IQ (mg)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_length == 16 && sh.Y_axis_length == 12)
                    {
                        if ((sh.X_axis_ID / 256 == 0xEA) && (sh.Y_axis_ID / 256 == 0xDA))
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            int smokeCount = GetMapNameCountForCodeBlock("Smoke limiter", sh.CodeBlock, newSymbols, false);
                            //sh.Varname = "Smoke limiter " + smokeCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Varname = "Smoke limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            if (sh.MapSelector != null)
                            {
                                if (sh.MapSelector.MapIndexes != null)
                                {
                                    if (sh.MapSelector.MapIndexes.Length > 1)
                                    {
                                        if (!MapSelectorIndexEmpty(sh))
                                        {
                                            double tempRange = GetTemperatureSOIRange(sh.MapSelector, smokeCount - 1);
                                            sh.Varname = "Smoke limiter " + tempRange.ToString() + " °C [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                                        }
                                    }
                                }
                            }
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Airflow mg/stroke";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.1; 
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                        else if ((sh.X_axis_ID / 256 == 0xEC) && (sh.Y_axis_ID / 256 == 0xC0))
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                }
                else if (sh.Length == 360)
                {
                    // 15x12 = inj dur limiter on R3 files
                    if (sh.X_axis_length == 12 && sh.Y_axis_length == 15)
                    {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            //if (injDurCount < 1) injDurCount = 1;

                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";                   
                    }
                    
                }
                else if (sh.Length == 352)
                {
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 11)
                    {
                        if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            int egrCount = GetMapNameCountForCodeBlock("EGR", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "EGR " + egrCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Mass Air Flow (mg/stroke)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                        else if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xEA)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Turbo";
                            sh.Varname = "N75 duty cycle [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Duty cycle %";
                            sh.Correction = -0.01;
                            sh.Offset = 100;
                            sh.X_axis_correction = 0.01; 
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                }
                else if (sh.Length == 320)
                {
                    sh.Category = "Probable maps";
                    sh.Subcategory = "Turbo";
                    sh.Varname = "Boost map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "] " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                    if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0)
                    {
                        sh.Category = "Detected maps";
                        sh.Varname = "Boost target map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Boost target (mbar)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xEA && sh.Y_axis_ID / 256 == 0xC0)
                    {
                        sh.Category = "Detected maps";
                        sh.Varname = "Boost target map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Boost target (mbar)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xDA)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "IQ by MAP limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") +" " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.Correction = 0.01;
                        sh.X_axis_descr = "Boost pressure";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mbar";
                    }

                }
                else if (sh.Length == 308)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Limiters";
                    //sh.Varname = "Boost limiter (temperature) [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Varname = "SOI limiter (temperature) [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Correction = -0.023437;
                    sh.Offset = 78;
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.X_axis_descr = "Temperature"; //IAT, ECT or Fuel temp?
                    sh.X_axis_correction = 0.1;
                    sh.X_axis_offset = -273.1;
                    sh.Z_axis_descr = "SOI limit (degrees)";
                    sh.YaxisUnits = "rpm";
                    sh.XaxisUnits = "°C";
                }
                else if (sh.Length == 286)
                {
                    if (sh.X_axis_length == 0x0d && sh.Y_axis_length == 0x0b)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        sh.Varname = "Driver wish [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Correction = 0.01;
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "Throttle  position";
                        sh.Z_axis_descr = "Requested IQ (mg)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "TPS %";
                    }
                }
                else if (sh.Length == 280) // boost target can be 10x14 as well in Seat maps
                {
                    if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";
                        sh.Varname = "Boost target map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "] " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Boost target (mbar)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                }
                else if (sh.Length == 260) // EXPERIMENTAL
                {
                    if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";

                    }
                }
                else if (sh.Length == 256)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Misc";
                    sh.Varname = "Driver wish [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Correction = 0.01;
                    sh.X_axis_correction = 0.01;
                    sh.X_axis_descr = "Throttle  position";
                    sh.Z_axis_descr = "Requested IQ (mg)";
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.YaxisUnits = "rpm";
                    sh.XaxisUnits = "TPS %";

                }
                else if (sh.Length == 240)
                {
                    if (sh.X_axis_length == 12 && sh.Y_axis_length == 10)
                    {
                        if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            sh.Varname = "Driver wish [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.01;
                            sh.X_axis_descr = "Throttle  position";
                            sh.Z_axis_descr = "Requested IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "TPS %";
                        }
                    }

                }
                else if (sh.Length == 220) // EXPERIMENTAL
                {
                    if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";

                    }
                }
                else if (sh.Length == 216)
                {
                    if (sh.X_axis_length == 12 && sh.Y_axis_length == 9)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        sh.Varname = "Driver wish [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Correction = 0.01;
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "Throttle  position";
                        sh.Z_axis_descr = "Requested IQ (mg)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "TPS %";
                    }

                }
                else if (sh.Length == 200)
                {
                    if (sh.X_axis_ID / 256 == 0xC0 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Boost limit map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        //   sh.Correction = 0.01;
                        //sh.X_axis_correction = 0.01;
                        sh.Y_axis_descr = "Atmospheric pressure (mbar)";
                        sh.Z_axis_descr = "Maximum boost pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC0 && sh.Y_axis_ID / 256 == 0xEA)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Boost limit map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        //   sh.Correction = 0.01;
                        //sh.X_axis_correction = 0.01;
                        sh.Y_axis_descr = "Atmospheric pressure (mbar)";
                        sh.Z_axis_descr = "Maximum boost pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        //if (!MapContainsNegativeValues(allBytes, sh))
                        if (GetMaxAxisValue(allBytes, sh, MapViewerEx.AxisIdent.X_Axis) > 3500) // was 5000
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel"; // was Limiters
                            // was limiter
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;

                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.Correction = 0.023438;
                            sh.Y_axis_correction = 0.01;
                            sh.Y_axis_descr = "IQ (mg/stroke)";
                            //sh.Z_axis_descr = "Max. degrees";
                            sh.Z_axis_descr = "Duration (crankshaft degrees)";

                            sh.X_axis_descr = "Engine speed (rpm)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";

                        }
                        else
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            //if (injDurCount < 1) injDurCount = 1;
                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";

                        }
                    }
                    else if (sh.X_axis_ID / 256 == 0xC4 && sh.Y_axis_ID / 256 == 0xEA)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;
                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xC4 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;
                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";
                    }
                }
                else if (sh.Length == 198) // EXPERIMENTAL
                {
                    if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Fuel";
                        int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                        injDurCount--;
                        //if (injDurCount < 1) injDurCount = 1;

                        sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Y_axis_correction = 0.01; 
                        sh.Correction = 0.023437;
                        sh.X_axis_descr = "Engine speed (rpm)";
                        //sh.Y_axis_descr = "Airflow mg/stroke";
                        sh.Y_axis_descr = "Requested Quantity mg/stroke";

                        sh.Z_axis_descr = "Duration (crankshaft degrees)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mg/st";

                    }
                }
                else if (sh.Length == 192)
                {
                    if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        sh.Varname = "Driver wish [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Correction = 0.01;
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "Throttle  position";
                        sh.Z_axis_descr = "Requested IQ (mg)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "TPS %";

                    }
                }
                else if (sh.Length == 180)
                {
                    if (sh.X_axis_length == 9 && sh.Y_axis_length == 10)
                    {
                        if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC1)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int sIQCount = GetMapNameCountForCodeBlock("Start IQ ", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Start IQ (" + sIQCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.01;
                            sh.X_axis_descr = "CT (celcius)";
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Z_axis_descr = "Requested IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "degC";
                        }
                        else if (sh.X_axis_ID / 256 == 0xC0 && sh.Y_axis_ID / 256 == 0xEC)
                        {
                            // atm boost limit R3 file versions
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "Boost limit map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            //   sh.Correction = 0.01;
                            //sh.X_axis_correction = 0.01;
                            sh.Y_axis_descr = "Atmospheric pressure (mbar)";
                            sh.Z_axis_descr = "Maximum boost pressure (mbar)";
                            sh.X_axis_descr = "Engine speed (rpm)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mbar";
                        }
                        else if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            //if (injDurCount < 1) injDurCount = 1;
                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";
                        }
                        else if (sh.X_axis_ID / 256 == 0xC4 && sh.Y_axis_ID / 256 == 0xEA)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            //if (injDurCount < 1) injDurCount = 1;
                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";
                        }
                    }
                    else if (sh.X_axis_length == 10 && sh.Y_axis_length == 9)
                    {
                        if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            //if (injDurCount < 1) injDurCount = 1;
                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";
                        }
                    }
                }
                else if (sh.Length == 162)
                {
                    if (sh.X_axis_length == 9 && sh.Y_axis_length == 9)
                    {
                        if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC1)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int sIQCount = GetMapNameCountForCodeBlock("Start IQ ", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Start IQ (" + sIQCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.01;
                            sh.X_axis_descr = "CT (celcius)";
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Z_axis_descr = "Requested IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "degC";
                        }
                    }
                }
                else if (sh.Length == 160)
                {
                    if (sh.X_axis_length == 8 && sh.Y_axis_length == 10)
                    {
                        if (sh.X_axis_ID / 256 == 0xC5 && sh.Y_axis_ID / 256 == 0xEC)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                            injDurCount--;
                            sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_correction = 0.01; 
                            sh.Correction = 0.023437;
                            sh.X_axis_descr = "Engine speed (rpm)";
                            //sh.Y_axis_descr = "Airflow mg/stroke";
                            sh.Y_axis_descr = "Requested Quantity mg/stroke";

                            sh.Z_axis_descr = "Duration (crankshaft degrees)";
                            sh.XaxisUnits = "rpm";
                            sh.YaxisUnits = "mg/st";
                        }
                    }
                }
                else if (sh.Length == 144)
                {
                    if (sh.X_axis_length == 9 && sh.Y_axis_length == 8)
                    {
                        if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            sh.Varname = "Fuel volume correction map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Userdescription = "zmwMKOR_KF";
                            sh.Z_axis_descr = "IQ correction per 100K";
                            sh.Correction = 0.002441;
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_correction = 0.01;
                            sh.X_axis_descr = "IQ (mg/stroke)";
                        }
                    }
                    if (sh.X_axis_length == 8 && sh.Y_axis_length == 9)
                    {
                        if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC1)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int sIQCount = GetMapNameCountForCodeBlock("Start IQ ", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Start IQ (" + sIQCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.X_axis_descr = "CT (celcius)";
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Z_axis_descr = "Requested IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Correction = 0.01;
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "degC";
                        }
                    }
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 24)
                    {
                        // Tq Lim
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Correction = 0.01;
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";
                    }
                }

                else if (sh.Length == 128)
                {
                    if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC1)
                    {
                        // check for valid axis data on temp data
                        if (IsValidTemperatureAxis(allBytes, sh, MapViewerEx.AxisIdent.Y_Axis))
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            int maflimTempCount = GetMapNameCountForCodeBlock("MAF correction by temperature", sh.CodeBlock, newSymbols, false);
                            maflimTempCount--;
                            sh.Varname = "MAF correction by temperature " + maflimTempCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Limit";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Intake air temperature"; //IAT, ECT or Fuel temp?
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Correction = 0.01;
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "°C";
                        }

                    }
                    else if (sh.X_axis_ID / 256 == 0xEA && sh.Y_axis_ID / 256 == 0xC1)
                    {
                        // check for valid axis data on temp data
                        if (IsValidTemperatureAxis(allBytes, sh, MapViewerEx.AxisIdent.Y_Axis))
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            int maflimTempCount = GetMapNameCountForCodeBlock("MAF correction by temperature", sh.CodeBlock, newSymbols, false);
                            maflimTempCount--;
                            sh.Varname = "MAF correction by temperature " + maflimTempCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Limit";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Intake air temperature"; //IAT, ECT or Fuel temp?
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Correction = 0.01;
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "°C";
                        }

                    }
                    else if (sh.X_axis_ID / 256 == 0xEC && sh.Y_axis_ID / 256 == 0xC0) // EXPERIMENTAL
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Expected fuel temperature [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Userdescription = "zmwMKBT_KF";
                        sh.Correction = 0.1;
                        sh.Offset = -273.1;
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.YaxisUnits = "rpm";
                        sh.X_axis_correction = 0.01;
                        sh.XaxisUnits = "mg/st";
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Z_axis_descr = "Fuel temperature °C";
                        

                    }

                }
                else if (sh.Length == 150)  // 3L (1.2 TDi, three cylinder VW Lupo) has this
                {
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 25)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Correction = 0.01;
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";

                    }
                }
                else if (sh.Length == 138)  // R3 (1.4 TDi, three cylinder) has this
                {
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 23)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Correction = 0.01;
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";

                    }
                }
                else if (sh.Length == 132)  // R3 (1.4 TDi, three cylinder) has this
                {
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 22)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Correction = 0.01;
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";

                    }
                }
                else if (sh.Length == 126)
                {
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 21)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";
                        sh.Correction = 0.01;
                    }
                }
                else if (sh.Length == 120)
                {
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 20)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.X_axis_descr = "Engine speed (rpm)";
                        sh.Correction = 0.01;
                        sh.XaxisUnits = "rpm";
                        sh.YaxisUnits = "mbar";

                    }
                }
                else if (sh.Length == 64)
                {
                    if (sh.X_axis_length == 32 && sh.Y_axis_length == 1)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Misc";
                        sh.Varname = "MAF linearization [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    }
                }

                else if (sh.Length == 60)
                {
                    if (sh.Y_axis_length == 6 && sh.X_axis_length == 5)
                    {
                        if (sh.Y_axis_ID == 0xC1A2)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            sh.Varname = "EGR temperature map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.X_axis_descr = "Temperature"; //IAT, ECT or Fuel temp?
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Z_axis_descr = "Mass airflow correction";
                        }
                    }
                }
                else if (sh.Length >= 18 && sh.Length <= 70)
                {
                    if (sh.X_axis_ID / 16 == 0xC1A && sh.Y_axis_ID / 16 == 0xEC3)
                    {
                        
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            //Temp after intercooler
                            sh.Y_axis_descr = "Temperature";
                            sh.X_axis_descr = "Engine speed (rpm)"; //IAT, ECT or Fuel temp?
                            sh.Y_axis_correction = 0.1;
                            sh.Y_axis_offset = -273.1;
                            sh.Z_axis_descr = "%";
                            sh.Correction = 0.01;
                            sh.Varname = "IQ by air intake temp[" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        }
                    }
                }
                
                else if (sh.Length == 20)
                {
                    if (sh.Y_axis_length == 5 && sh.X_axis_length == 2)
                    {
                        //if (sh.Y_axis_ID == 0xC1A2)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            //sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Y_axis_descr = "Air pressure";
                            sh.X_axis_descr = "Temperature"; //IAT, ECT or Fuel temp?
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Z_axis_descr = "Time (sec)";
                            sh.Correction = 0.01;
                            sh.Varname = "Pre-glow map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        }
                    }
                }

                else if (sh.Length == 12)
                {
                    if (sh.X_axis_length == 6 && sh.Y_axis_length == 1)
                    {
                        if ((sh.X_axis_ID & 0xFFF0) == 0xECB0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            sh.Varname = "Selector for injector duration [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            // soi values as axis!!
                            sh.Y_axis_correction = -0.023437;
                            sh.Y_axis_offset = 78;
                            sh.Correction = 0.00390625;
                            sh.Z_axis_descr = "Map index";

                            sh.YaxisUnits = "SOI";
                        }
                    }
                }
                else if (sh.Length == 8)
                {
                    /*if (sh.X_axis_ID / 256 == 0xC1) // idle RPM
                    {
                        if (IsValidTemperatureAxis(allBytes, sh, MapViewerEx.AxisIdent.X_Axis))
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            int lmCount = GetMapNameCountForCodeBlock("Idle RPM", sh.CodeBlock, newSymbols, false);

                            sh.Varname = "Idle RPM (" + lmCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Y_axis_descr = "Coolant temperature";
                            sh.Y_axis_correction = 0.1;
                            sh.Y_axis_offset = -273.1;
                            sh.Z_axis_descr = "Target engine speed";
                            sh.YaxisUnits = "°C";
                        }

                    }*/
                }
                else if (sh.Length == 4)
                {
                    if (sh.X_axis_length == 2 && sh.Y_axis_length == 1)
                    {
                        if (sh.X_axis_ID == 0xEBA2 || sh.X_axis_ID == 0xEBA4 || sh.X_axis_ID == 0xE9BC)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            sh.Varname = "MAP linearization [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        }
                        else if (sh.X_axis_ID / 256 == 0xC1) // idle RPM
                        {
                            if (IsValidTemperatureAxis(allBytes, sh, MapViewerEx.AxisIdent.X_Axis))
                            {
                                sh.Category = "Detected maps";
                                sh.Subcategory = "Misc";
                                int lmCount = GetMapNameCountForCodeBlock("Idle RPM", sh.CodeBlock, newSymbols, false);

                                sh.Varname = "Idle RPM (" + lmCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                                sh.Y_axis_descr = "Coolant temperature";
                                sh.Y_axis_correction = 0.1;
                                sh.Y_axis_offset = -273.1;
                                sh.Z_axis_descr = "Target engine speed";
                                sh.YaxisUnits = "°C";
                            }

                        }
                    }
                }
                if (sh.X_axis_ID == 0xDA6C && sh.Y_axis_ID == 0xDA6A)
                {
                    sh.Category = "Detected maps";
                    sh.X_axis_correction = 0.1;
                    sh.X_axis_offset = -273.1;
                    sh.XaxisUnits = "°C";
                    sh.Subcategory = "Limiters";
                    sh.Varname = "Boost correction by temperature [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                    sh.X_axis_descr = "IAT (celcius)";
                    sh.Y_axis_descr = "Requested boost";
                    sh.Z_axis_descr = "Boost limit (mbar)";
                    sh.YaxisUnits = "mbar";
                }
            }



        }

        private bool MapSelectorIndexEmpty(SymbolHelper sh)
        {
            bool retval = true;
            if (sh.MapSelector != null)
            {
                foreach (int iTest in sh.MapSelector.MapIndexes)
                {
                    if (iTest != 0) retval = false;
                }
            }
            return retval;
        }

        private int GetMaxAxisValue(byte[] allBytes, SymbolHelper sh, MapViewerEx.AxisIdent axisIdent)
        {
            int retval = 0;
            if (axisIdent == MapViewerEx.AxisIdent.X_Axis)
            {
                //read x axis values
                int offset = sh.X_axis_address;
                for (int i = 0; i < sh.X_axis_length; i++)
                {
                    int val = Convert.ToInt32(allBytes[offset]) + Convert.ToInt32(allBytes[offset + 1]) * 256;
                    if (val > retval) retval = val;
                    offset += 2;
                }
            }
            else if (axisIdent == MapViewerEx.AxisIdent.Y_Axis)
            {
                //read x axis values
                int offset = sh.Y_axis_address;
                for (int i = 0; i < sh.Y_axis_length; i++)
                {
                    int val = Convert.ToInt32(allBytes[offset]) + Convert.ToInt32(allBytes[offset + 1]) * 256;
                    if (val > retval) retval = val;
                    offset += 2;
                }
            }
            return retval;
        }

        private bool IsValidTemperatureAxis(byte[] allBytes, SymbolHelper sh, MapViewerEx.AxisIdent axisIdent)
        {
            bool retval = true;
            if (axisIdent == MapViewerEx.AxisIdent.X_Axis)
            {
                //read x axis values
                int offset = sh.X_axis_address;
                for (int i = 0; i < sh.X_axis_length; i++)
                {
                    int val = Convert.ToInt32(allBytes[offset]) + Convert.ToInt32(allBytes[offset + 1]) * 256;
                    double tempVal = (Convert.ToDouble(val) * 0.1) - 273.1;
                    if (tempVal < -80 || tempVal > 200) retval = false;
                    offset += 2;
                }
            }
            else if (axisIdent == MapViewerEx.AxisIdent.Y_Axis)
            {
                //read x axis values
                int offset = sh.Y_axis_address;
                for (int i = 0; i < sh.Y_axis_length; i++)
                {
                    int val = Convert.ToInt32(allBytes[offset]) + Convert.ToInt32(allBytes[offset + 1]) * 256;
                    double tempVal = (Convert.ToDouble(val) * 0.1) - 273.1;
                    if (tempVal < -80 || tempVal > 200) retval = false;
                    offset += 2;
                }
            }
            return retval;
        }

        private double GetTemperatureDurRange(int index)
        {
            double retval = 0;
            // 
            return retval;
        }

        //SOI is selected on coolant temperature!
        private double GetTemperatureSOIRange(MapSelector sh, int index)
        {
            double retval = index;
            if (sh.MapData != null)
            {
                if (sh.MapData.Length > index)
                {
                    retval = (Convert.ToDouble(sh.MapData.GetValue(index)) * 0.1) - 273.1;
                }

            }
            return Math.Round(retval, 0);
        }

        private bool MapContainsNegativeValues(byte[] allBytes, SymbolHelper sh)
        {
            for (int i = 0; i < sh.Length; i += 2)
            {
                int currval = Convert.ToInt32(allBytes[sh.Flash_start_address + i + 1]) * 256 + Convert.ToInt32(allBytes[sh.Flash_start_address + i]);
                if (currval > 0xF000) return true;
            }
            return false;

        }

        private int GetMapNameCountForCodeBlock(string varName, int codeBlock, SymbolCollection newSymbols, bool debug)
        {
            int count = 0;
            if (debug) Console.WriteLine("Check " + varName + " " + codeBlock);

            foreach (SymbolHelper sh in newSymbols)
            {
                if (debug)
                {
                    if (!sh.Varname.StartsWith("2D") && !sh.Varname.StartsWith("3D"))
                    {
                        Console.WriteLine(sh.Varname + " " + sh.CodeBlock);
                    }
                }
                if (sh.Varname.StartsWith(varName) && sh.CodeBlock == codeBlock)
                {

                    if (debug) Console.WriteLine("Found " + sh.Varname + " " + sh.CodeBlock);

                    count++;
                }
            }
            count++;
            return count;
        }

        private int CheckCodeBlock(int offset, byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            int codeBlockID = 0;
            try
            {
                int endOfTable = Convert.ToInt32(allBytes[offset + 0x01000]) + Convert.ToInt32(allBytes[offset + 0x01001]) * 256 + offset;
                //sth wrong here with File 019AQ (ARL)
                int codeBlockAddress = Convert.ToInt32(allBytes[offset + 0x01002]) + Convert.ToInt32(allBytes[offset + 0x01003]) * 256 + offset;
                if (endOfTable == offset + 0xC3C3) return 0;               
                codeBlockID = Convert.ToInt32(allBytes[codeBlockAddress]) + Convert.ToInt32(allBytes[codeBlockAddress + 1]) * 256;
                //Why do we need line obove?
                //codeBlockID = Convert.ToInt32(allBytes[codeBlockAddress]);

                foreach (CodeBlock cb in newCodeBlocks)
                {
                    if (cb.StartAddress <= codeBlockAddress && cb.EndAddress >= codeBlockAddress)
                    {
                        cb.CodeID = codeBlockID;
                        cb.AddressID = codeBlockAddress;
                    }
                }
            }
            catch (Exception)
            {
            }
            return codeBlockID;
        }
        private bool AddToSymbolCollection(SymbolCollection newSymbols, SymbolHelper newSymbol, List<CodeBlock> newCodeBlocks)
        {
            if (newSymbol.Length >= 800) return false;
            foreach (SymbolHelper sh in newSymbols)
            {
                if (sh.Flash_start_address == newSymbol.Flash_start_address)
                {
                    //   Console.WriteLine("Already in collection: " + sh.Flash_start_address.ToString("X8"));
                    return false;
                }
                // not allowed to overlap
                /*else if (newSymbol.Flash_start_address > sh.Flash_start_address && newSymbol.Flash_start_address < (sh.Flash_start_address + sh.Length))
                {
                    Console.WriteLine("Overlapping map: " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_length.ToString() + " x " + sh.Y_axis_length.ToString());
                    Console.WriteLine("Overlapping new: " + newSymbol.Flash_start_address.ToString("X8") + " " + newSymbol.X_axis_length.ToString() + " x " + newSymbol.Y_axis_length.ToString());
                    return false;
                }*/
            }
            newSymbols.Add(newSymbol);
            newSymbol.CodeBlock = DetermineCodeBlockByByAddress(newSymbol.Flash_start_address, newCodeBlocks);
            return true;
        }

        private bool isValidLength(int length, int id)
        {
            int idstrip = id / 256;
            if ((idstrip & 0xF0) == 0xE0) 
            //if (idstrip == 0xEB /*|| idstrip == 0xDE*/)
            {
                if (length > 0 && length <= 32) return true;
            }
            else
            {
                if (length > 0 && length < 32) return true;
            }
            //if (length <= 64) Console.WriteLine("seen id " + id.ToString("X4") + " with len " + length.ToString());

            return false;
        }

        private bool isAxisID(int id)
        {
            int idstrip = id / 256;
            if (idstrip == 0xDB) return true;
            if (idstrip == 0xC0 || idstrip == 0xC1 || idstrip == 0xC2 || idstrip == 0xC4 || idstrip == 0xC5) return true;
            if (idstrip == 0xE0 || idstrip == 0xE4 || idstrip == 0xE5 || idstrip == 0xE9 || idstrip == 0xEA || idstrip == 0xEB || idstrip == 0xEC) return true;
            if (idstrip == 0xDA /*|| idstrip == 0xDC */|| idstrip == 0xDD || idstrip == 0xDE) return true;
            if (idstrip == 0xF9 || idstrip == 0xFE) return true;
            if (idstrip == 0xE8) return true;
            //if (idstrip == 0xD7 || idstrip == 0xE6) return true;
           // if (idstrip == 0xD5) return true;
            return false;
        }

        // we need to check AHEAD for selector maps
        // if these are present we may be facing a complex map structure
        // which we need to handle in a special way (selectors always have data like 00 01 00 02 00 03 00 04 etc)
        private bool CheckMap(int t, byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks, out int len2Skip)
        {
            bool mapFound = false;
            bool retval = false;
            bool _dontGenMaps = false;
            len2Skip = 0;
            List<MapSelector> mapSelectors = new List<MapSelector>();
            if (t < allBytes.Length - 0x100)
            {
                /*if (t > 0x58000 && t < 0x60000)
                {
                    Console.WriteLine("Checkmap: " + t.ToString("X8"));
                }*/
                if (CheckAxisCount(t, allBytes, out mapSelectors) > 3)
                {
                    // check for selectors as well, and count them in the process
                    //Console.WriteLine("Offset " + t.ToString("X8") + " has more than 3 consecutive axis");
                    /*foreach (MapSelector ms in mapSelectors)
                    {
                        Console.WriteLine("selector: " + ms.StartAddress.ToString("X8") + " " + ms.MapLength.ToString() + " " + ms.NumRepeats.ToString());
                    }*/
                    _dontGenMaps = true;

                }

                int xaxisid = (Convert.ToInt32(allBytes[t + 1]) * 256) + Convert.ToInt32(allBytes[t]);

                if (isAxisID(xaxisid))
                {
                    int xaxislen = (Convert.ToInt32(allBytes[t + 3]) * 256) + Convert.ToInt32(allBytes[t + 2]);
                    // Console.WriteLine("Valid XID: " + xaxisid.ToString("X4") + " @" + t.ToString("X8") + " len: " + xaxislen.ToString("X2"));
                    if (isValidLength(xaxislen, xaxisid))
                    {
                        //Console.WriteLine("Valid XID: " + xaxisid.ToString("X4") + " @" + t.ToString("X8") + " len: " + xaxislen.ToString("X2"));
                        // misschien is er nog een as
                        int yaxisid = (Convert.ToInt32(allBytes[t + 5 + (xaxislen * 2)]) * 256) + Convert.ToInt32(allBytes[t + 4 + (xaxislen * 2)]);
                        int yaxislen = (Convert.ToInt32(allBytes[t + 7 + (xaxislen * 2)]) * 256) + Convert.ToInt32(allBytes[t + 6 + (xaxislen * 2)]);
                        if (isAxisID(yaxisid) && isValidLength(yaxislen, yaxisid))
                        {
                            // 3d map

                            int zaxisid = (Convert.ToInt32(allBytes[t + 9 + (xaxislen * 2) + (yaxislen * 2)]) * 256) + Convert.ToInt32(allBytes[t + 8 + (xaxislen * 2) + (yaxislen * 2)]);
                            //Console.WriteLine("Valid YID: " + yaxisid.ToString("X4") + " @" + t.ToString("X8") + " len: " + yaxislen.ToString("X2"));


                            //Console.WriteLine(t.ToString("X8") + " XID: " + xaxisid.ToString("X4") + " XLEN: " + xaxislen.ToString("X2") + " YID: " + yaxisid.ToString("X4") + " YLEN: " + yaxislen.ToString("X2"));
                            SymbolHelper newSymbol = new SymbolHelper();
                            newSymbol.X_axis_length = xaxislen;
                            newSymbol.Y_axis_length = yaxislen;
                            newSymbol.X_axis_ID = xaxisid;
                            newSymbol.Y_axis_ID = yaxisid;
                            newSymbol.X_axis_address = t + 4;
                            newSymbol.Y_axis_address = t + 8 + (xaxislen * 2);

                            newSymbol.Length = xaxislen * yaxislen * 2;
                            newSymbol.Flash_start_address = t + 8 + (xaxislen * 2) + (yaxislen * 2);
                            if (isAxisID(zaxisid))
                            {
                                int zaxislen = (Convert.ToInt32(allBytes[t + 11 + (xaxislen * 2) + (yaxislen * 2)]) * 256) + Convert.ToInt32(allBytes[t + 10 + (xaxislen * 2) + (yaxislen * 2)]);

                                int zaxisaddress = t + 12 + (xaxislen * 2) + (yaxislen * 2);

                                if (isValidLength(zaxislen, zaxisid))
                                {
                                    //   newSymbol.Flash_start_address += 0x10; // dan altijd 16 erbij
                                    int len2skip = (4 + zaxislen * 2);
                                    if (len2skip < 16) len2skip = 16; // at least 16 bytes
                                    newSymbol.Flash_start_address += len2skip;

                                    len2Skip += (xaxislen * 2) + (yaxislen * 2) + zaxislen * 2;

                                    if (!_dontGenMaps)
                                    {
                                        // this has something to do with repeating several times with the same axis set

                                        Console.WriteLine("Added " + len2skip.ToString() + " because of z axis " + newSymbol.Flash_start_address.ToString("X8"));


                                        // maybe there are multiple maps between the end of the map and the start of the next axis
                                        int nextMapAddress = findNextMap(allBytes, (int)(newSymbol.Flash_start_address + newSymbol.Length), newSymbol.Length * 10);
                                        if (nextMapAddress > 0)
                                        {
                                            // is it divisable by the maplength

                                            if (((nextMapAddress - newSymbol.Flash_start_address) % newSymbol.Length) == 0)
                                            {

                                                int numberOfrepeats = (int)(nextMapAddress - newSymbol.Flash_start_address) / newSymbol.Length;
                                                numberOfrepeats = zaxislen;
                                                if (numberOfrepeats > 1)
                                                {
                                                    MapSelector ms = new MapSelector();
                                                    ms.NumRepeats = numberOfrepeats;
                                                    ms.MapLength = newSymbol.Length;
                                                    ms.StartAddress = zaxisaddress;
                                                    ms.XAxisAddress = newSymbol.X_axis_address;
                                                    ms.YAxisAddress = newSymbol.Y_axis_address;
                                                    ms.XAxisLen = newSymbol.X_axis_length;
                                                    ms.YAxisLen = newSymbol.Y_axis_length;
                                                    ms.MapData = new int[zaxislen];
                                                    int boffset = 0;
                                                    for (int ia = 0; ia < zaxislen; ia++)
                                                    {
                                                        int axisValue = Convert.ToInt32(allBytes[zaxisaddress + boffset]) + Convert.ToInt32(allBytes[zaxisaddress + boffset + 1]) * 256;
                                                        ms.MapData.SetValue(axisValue, ia);
                                                        boffset += 2;
                                                    }

                                                    ms.MapIndexes = new int[zaxislen];
                                                    for (int ia = 0; ia < zaxislen; ia++)
                                                    {
                                                        int axisValue = Convert.ToInt32(allBytes[zaxisaddress + boffset]) + Convert.ToInt32(allBytes[zaxisaddress + boffset + 1]) * 256;
                                                        ms.MapIndexes.SetValue(axisValue, ia);
                                                        boffset += 2;
                                                    }

                                                    // numberOfrepeats--;
                                                    //int idx = 0;

                                                    for (int maprepeat = 0; maprepeat < numberOfrepeats; maprepeat++)
                                                    {
                                                        // idx ++;
                                                        SymbolHelper newGenSym = new SymbolHelper();
                                                        newGenSym.X_axis_length = newSymbol.X_axis_length;
                                                        newGenSym.Y_axis_length = newSymbol.Y_axis_length;
                                                        newGenSym.X_axis_ID = newSymbol.X_axis_ID;
                                                        newGenSym.Y_axis_ID = newSymbol.Y_axis_ID;
                                                        newGenSym.X_axis_address = newSymbol.X_axis_address;
                                                        newGenSym.Y_axis_address = newSymbol.Y_axis_address;
                                                        newGenSym.Flash_start_address = newSymbol.Flash_start_address + maprepeat * newSymbol.Length;
                                                        newGenSym.Length = newSymbol.Length;
                                                        newGenSym.Varname = "3D GEN " + newGenSym.Flash_start_address.ToString("X8") + " " + xaxisid.ToString("X4") + " " + yaxisid.ToString("X4");
                                                        newGenSym.MapSelector = ms;
                                                        // attach a mapselector to these maps
                                                        // only add it if the map is not empty
                                                        // otherwise we will cause confusion among users
                                                        if (maprepeat > 0)
                                                        {
                                                            try
                                                            {
                                                                if (ms.MapIndexes[maprepeat] > 0)
                                                                {
                                                                    retval = AddToSymbolCollection(newSymbols, newGenSym, newCodeBlocks);
                                                                    if (retval)
                                                                    {
                                                                        mapFound = true;
                                                                        //GUIDO len2Skip += newGenSym.Length;
                                                                        //t += (xaxislen * 2) + (yaxislen * 2) + newGenSym.Length;
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {
                                                            }
                                                        }
                                                        else
                                                        {
                                                            retval = AddToSymbolCollection(newSymbols, newGenSym, newCodeBlocks);
                                                            if (retval)
                                                            {
                                                                mapFound = true;
                                                                //GUIDO len2Skip += (xaxislen * 2) + (yaxislen * 2) + newGenSym.Length;
                                                                //t += (xaxislen * 2) + (yaxislen * 2) + newGenSym.Length;
                                                            }
                                                        }
                                                    }
                                                }
                                                //Console.WriteLine("Indeed!");
                                                // the first one will be added anyway.. add the second to the last

                                            }

                                        }
                                    }
                                    else
                                    {

                                        int maxisid = (Convert.ToInt32(allBytes[t + 13 + (xaxislen * 2) + (yaxislen * 2) + (zaxislen * 2)]) * 256) + Convert.ToInt32(allBytes[t + 12 + (xaxislen * 2) + (yaxislen * 2) + zaxislen * 2]);
                                        int maxislen = (Convert.ToInt32(allBytes[t + 15 + (xaxislen * 2) + (yaxislen * 2) + (zaxislen * 2)]) * 256) + Convert.ToInt32(allBytes[t + 14 + (xaxislen * 2) + (yaxislen * 2) + zaxislen * 2]);
                                        //maxislen *= 2;

                                        int maxisaddress = t + 16 + (xaxislen * 2) + (yaxislen * 2);

                                        if (isAxisID(maxisid))
                                        {
                                            newSymbol.Flash_start_address += (maxislen * 2) + 4;
                                        }
                                        // special situation, handle selectors
                                        //Console.WriteLine("Map start address = " + newSymbol.Flash_start_address.ToString("X8"));
                                        long lastFlashAddress = newSymbol.Flash_start_address;
                                        foreach (MapSelector ms in mapSelectors)
                                        {

                                            // check the memory size between the start of the map and the 
                                            // start of the map selector
                                            long memsize = ms.StartAddress - lastFlashAddress;
                                            memsize /= 2; // in words
                                            if (ms.NumRepeats > 0)
                                            {
                                                int mapsize = Convert.ToInt32(memsize) / ms.NumRepeats;
                                                if ((xaxislen * yaxislen) == mapsize)
                                                {
                                                    //Console.WriteLine("selector: " + ms.StartAddress.ToString("X8") + " " + ms.MapLength.ToString() + " " + ms.NumRepeats.ToString());
                                                    //Console.WriteLine("memsize = " + memsize.ToString() + " mapsize " + mapsize.ToString());
                                                    //Console.WriteLine("starting at address: " + lastFlashAddress.ToString("X8"));
                                                    // first axis set
                                                    //len2Skip += (xaxislen * 2) + (yaxislen * 2);
                                                    for (int i = 0; i < ms.NumRepeats; i++)
                                                    {
                                                        SymbolHelper shGen2 = new SymbolHelper();
                                                        shGen2.MapSelector = ms;
                                                        shGen2.X_axis_length = newSymbol.X_axis_length;
                                                        shGen2.Y_axis_length = newSymbol.Y_axis_length;
                                                        shGen2.X_axis_ID = newSymbol.X_axis_ID;
                                                        shGen2.Y_axis_ID = newSymbol.Y_axis_ID;
                                                        shGen2.X_axis_address = newSymbol.X_axis_address;
                                                        shGen2.Y_axis_address = newSymbol.Y_axis_address;
                                                        shGen2.Length = mapsize * 2;
                                                        //shGen2.Category = "Generated";
                                                        long address = lastFlashAddress;
                                                        shGen2.Flash_start_address = address;
                                                        //shGen2.Correction = 0.023437; // TEST
                                                        //shGen2.Varname = "Generated* " + shGen2.Flash_start_address.ToString("X8") + " " + ms.StartAddress.ToString("X8") + " " + ms.NumRepeats.ToString() + " " + i.ToString();
                                                        shGen2.Varname = "3D " + shGen2.Flash_start_address.ToString("X8") + " " + shGen2.X_axis_ID.ToString("X4") + " " + shGen2.Y_axis_ID.ToString("X4");
                                                        //if (i < ms.NumRepeats - 1)
                                                        {
                                                            retval = AddToSymbolCollection(newSymbols, shGen2, newCodeBlocks);
                                                            if (retval)
                                                            {
                                                                mapFound = true;
                                                                //GUIDO len2Skip += shGen2.Length;
                                                                //t += (xaxislen * 2) + (yaxislen * 2) + shGen2.Length;
                                                            }
                                                        }
                                                        lastFlashAddress = address + mapsize * 2;
                                                        // Console.WriteLine("Set last address to " + lastFlashAddress.ToString("X8"));
                                                    }
                                                    lastFlashAddress += ms.NumRepeats * 4 + 4;
                                                }
                                                else if ((zaxislen * maxislen) == mapsize)
                                                {
                                                    // second axis set
                                                   // len2Skip += (xaxislen * 2) + (yaxislen * 2);
                                                    for (int i = 0; i < ms.NumRepeats; i++)
                                                    {
                                                        SymbolHelper shGen2 = new SymbolHelper();
                                                        shGen2.MapSelector = ms;
                                                        shGen2.X_axis_length = maxislen;
                                                        shGen2.Y_axis_length = zaxislen;
                                                        shGen2.X_axis_ID = maxisid;
                                                        shGen2.Y_axis_ID = zaxisid;
                                                        shGen2.X_axis_address = maxisaddress;
                                                        shGen2.Y_axis_address = zaxisaddress;
                                                        shGen2.Length = mapsize * 2;
                                                        //shGen2.Category = "Generated";
                                                        long address = lastFlashAddress;
                                                        shGen2.Flash_start_address = address;
                                                        //shGen2.Varname = "Generated** " + shGen2.Flash_start_address.ToString("X8");
                                                        shGen2.Varname = "3D " + shGen2.Flash_start_address.ToString("X8") + " " + shGen2.X_axis_ID.ToString("X4") + " " + shGen2.Y_axis_ID.ToString("X4");

                                                        //if (i < ms.NumRepeats - 1)
                                                        {
                                                            retval = AddToSymbolCollection(newSymbols, shGen2, newCodeBlocks);
                                                            if (retval)
                                                            {
                                                                mapFound = true;
                                                                //GUIDO len2Skip += shGen2.Length;
                                                                //t += (xaxislen * 2) + (yaxislen * 2) + shGen2.Length;
                                                            }
                                                        }
                                                        lastFlashAddress = address + mapsize * 2;
                                                        //Console.WriteLine("Set last address 2 to " + lastFlashAddress.ToString("X8"));
                                                    }
                                                    lastFlashAddress += ms.NumRepeats * 4 + 4;
                                                }
                                            }
                                            //if(ms.NumRepeats

                                        }
                                    }
                                }
                            }

                            newSymbol.Varname = "3D " + newSymbol.Flash_start_address.ToString("X8") + " " + xaxisid.ToString("X4") + " " + yaxisid.ToString("X4");
                            //Console.WriteLine(newSymbol.Varname + " " + newSymbol.Length.ToString() + " " + newSymbol.X_axis_length.ToString() + "x" + newSymbol.Y_axis_length.ToString());
                            retval = AddToSymbolCollection(newSymbols, newSymbol, newCodeBlocks);
                            if (retval)
                            {
                                mapFound = true;
                                //GUIDO len2Skip += (xaxislen * 2) + (yaxislen * 2) + newSymbol.Length;
                                //t += (xaxislen * 2) + (yaxislen * 2) + newSymbol.Length;
                            }

                        }
                        else
                        {
                            if (yaxisid > 0xC000 && yaxisid < 0xF000 && yaxislen <= 32) Console.WriteLine("Unknown map id: " + yaxisid.ToString("X4") + " len " + yaxislen.ToString("X4") + " at address " + t.ToString("X8"));
                            SymbolHelper newSymbol = new SymbolHelper();
                            newSymbol.X_axis_length = xaxislen;
                            newSymbol.X_axis_ID = xaxisid;
                            newSymbol.X_axis_address = t + 4;
                            newSymbol.Length = xaxislen * 2;
                            newSymbol.Flash_start_address = t + 4 + (xaxislen * 2);
                            newSymbol.Varname = "2D " + newSymbol.Flash_start_address.ToString("X8") + " " + xaxisid.ToString("X4");
                            //newSymbols.Add(newSymbol);
                            newSymbol.CodeBlock = DetermineCodeBlockByByAddress(newSymbol.Flash_start_address, newCodeBlocks);
                            retval = AddToSymbolCollection(newSymbols, newSymbol, newCodeBlocks);
                            if (retval)
                            {
                                mapFound = true;
                                //GUIDO len2Skip += (xaxislen * 2);
                                //t += (xaxislen * 2);
                            }
                            // 2d map
                        }
                    }

                }
            }
            return mapFound;
        }

        private bool MapIsEmpty(byte[] allBytes, SymbolHelper sh)
        {
            for (int i = 0; i < sh.Length; i += 2)
            {
                int currval = Convert.ToInt32(allBytes[sh.Flash_start_address + i + 1]) * 256 + Convert.ToInt32(allBytes[sh.Flash_start_address + i]);
                if (currval != 0) return false;
            }
            return true;
        }

        private int findNextMap(byte[] allBytes, int index, int maxBytesToSearch)
        {
            int retval = 0;
            for (int i = index; i < index + maxBytesToSearch; i += 2)
            {
                int xaxisid = (Convert.ToInt32(allBytes[i + 1]) * 256) + Convert.ToInt32(allBytes[i]);
                if (isAxisID(xaxisid))
                {
                    int xaxislen = (Convert.ToInt32(allBytes[i + 3]) * 256) + Convert.ToInt32(allBytes[i + 2]);
                    if (isValidLength(xaxislen, xaxisid))
                    {
                        return i;
                    }
                }
            }
            return retval;
        }

        private int CheckAxisCount(int offset, byte[] allBytes, out List<MapSelector> mapSelectors)
        {
            int axisCount = 0;
            /*if (offset == 0x58504)
            {
                Console.WriteLine("58504");
            }*/
            mapSelectors = new List<MapSelector>();
            bool axisFound = true;
            int t = offset;
            while (axisFound)
            {
                axisFound = false;
                int axisid = (Convert.ToInt32(allBytes[t + 1]) * 256) + Convert.ToInt32(allBytes[t]);
                if (isAxisID(axisid))
                {
                    int axislen = (Convert.ToInt32(allBytes[t + 3]) * 256) + Convert.ToInt32(allBytes[t + 2]);
                    if (axislen > 0 && axislen < 32)
                    {
                        axisCount++;
                        axisFound = true;
                        t += 4 + (axislen * 2);
                    }

                }
            }
            // search from offset 't' for selectors
            // maximum searchrange = 0x1000
            int BytesToSearch = 5120 + 16;
            if (axisCount > 3)
            {
                while (BytesToSearch > 0)
                {
                    int axisid = (Convert.ToInt32(allBytes[t + 1]) * 256) + Convert.ToInt32(allBytes[t]);
                    if (isAxisID(axisid))
                    {
                        //Console.WriteLine("Checking address: " + t.ToString("X8"));
                        int axislen = (Convert.ToInt32(allBytes[t + 3]) * 256) + Convert.ToInt32(allBytes[t + 2]);
                        if (axislen <= 10) // more is not valid for selectors
                        {
                            // read & verify data (00 00 00 01 00 02 00 03 etc)
                            bool selectorValid = true;
                            int num = 0;
                            uint prevSelector = 0;
                            for (int i = 0; i < (axislen * 2); i += 2)
                            {
                                uint selValue = Convert.ToUInt32(allBytes[t + 4 + (axislen * 2) + i]) + Convert.ToUInt32(allBytes[t + 4 + (axislen * 2) + 1 + i]);
                                //Console.WriteLine("Selval: " + selValue.ToString() + " num: " + num.ToString());
                                /*if (axislen < 3)
                                {
                                    selectorValid = false;
                                    break;
                                }*/
                                if (allBytes[t + 4 + (axislen * 2) + i] != 0)
                                {
                                    if(allBytes[t + 4 + (axislen * 2) + i] != 0x40) selectorValid = false;
                                    break;
                                }
                                if (allBytes[t + 4 + (axislen * 2) + 1 + i] > 9)
                                {
                                    selectorValid = false;
                                    break;
                                }
                                if (prevSelector > selValue)
                                {
                                    selectorValid = false;
                                    break;
                                }
                                prevSelector = selValue;
                                /*if (num != selValue)
                                {
                                    // not a valid selector
                                    selectorValid = false;
                                    break;
                                }*/
                                num++;
                            }
                            if (selectorValid)
                            {
                                // create a new selector
                                //Console.WriteLine("Selector valid " + t.ToString("X8"));
                                MapSelector newSel = new MapSelector();
                                newSel.NumRepeats = axislen;
                                newSel.StartAddress = t;

                                // read the data into the mapselector
                                newSel.MapData = new int[axislen];
                                int boffset = 0;
                                for (int ia = 0; ia < axislen; ia++)
                                {
                                    int axisValue = Convert.ToInt32(allBytes[newSel.StartAddress + 4 + boffset]) + Convert.ToInt32(allBytes[newSel.StartAddress + 4 + boffset + 1]) * 256;
                                    newSel.MapData.SetValue(axisValue, ia);
                                    boffset += 2;
                                }
                                mapSelectors.Add(newSel);
                                if (mapSelectors.Count > 5) break;

                                BytesToSearch = 5120 + 16;
                            }
                        }
                    }
                    t += 2;
                    BytesToSearch -= 2;
                }
            }
            return axisCount;
        }

        private void VerifyCodeBlocks(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            //000001=automatic,000002=manual,000003=4wd ????
            Tools.Instance.m_codeBlock5ID = 0;
            Tools.Instance.m_codeBlock6ID = 0;
            Tools.Instance.m_codeBlock7ID = 0;
            bool found = true;
            int offset = 0;
            int defaultCodeBlockLength = 0x10000;
            int currentCodeBlockLength = 0;
            int prevCodeBlockStart = 0;
            while (found)
            {
                int CodeBlockAddress = Tools.Instance.findSequence(allBytes, offset, new byte[11] { 0xC1, 0x02, 0x00, 0x68, 0x00, 0x25, 0x03, 0x00, 0x00, 0x10, 0x27 }, new byte[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                if (CodeBlockAddress > 0)
                {
                    CodeBlock newcodeblock = new CodeBlock();
                    newcodeblock.StartAddress = CodeBlockAddress - 1;
                    if (prevCodeBlockStart == 0) prevCodeBlockStart = newcodeblock.StartAddress;
                    else if (currentCodeBlockLength == 0)
                    {
                        currentCodeBlockLength = newcodeblock.StartAddress - prevCodeBlockStart;
                        if (currentCodeBlockLength > 0x10000) currentCodeBlockLength = 0x10000;
                    }
                    // find the next occurence of the checksum
                    newCodeBlocks.Add(newcodeblock);
                    offset = CodeBlockAddress + 1;
                }
                else found = false;
            }
            foreach (CodeBlock cb in newCodeBlocks)
            {
                if (currentCodeBlockLength != 0) cb.EndAddress = cb.StartAddress + currentCodeBlockLength - 1;
                else cb.EndAddress = cb.StartAddress + defaultCodeBlockLength - 1;
            }
            foreach (CodeBlock cb in newCodeBlocks)
            {
                int autoSequenceIndex = Tools.Instance.findSequence(allBytes, cb.StartAddress, new byte[7] { 0x45, 0x44, 0x43, 0x20, 0x20, 0x41, 0x47 }, new byte[7] { 1, 1, 1, 1, 1, 1, 1 });
                int manualSequenceIndex = Tools.Instance.findSequence(allBytes, cb.StartAddress, new byte[7] { 0x45, 0x44, 0x43, 0x20, 0x20, 0x53, 0x47 }, new byte[7] { 1, 1, 1, 1, 1, 1, 1 });
                if (autoSequenceIndex < cb.EndAddress && autoSequenceIndex >= cb.StartAddress) cb.BlockGearboxType = GearboxType.Automatic;
                if (manualSequenceIndex < cb.EndAddress && manualSequenceIndex >= cb.StartAddress) cb.BlockGearboxType = GearboxType.Manual;
            }
            if (Tools.Instance.m_currentfilelength >= 0x80000)
            {
                Tools.Instance.m_codeBlock5ID = CheckCodeBlock(0x50000, allBytes, newSymbols, newCodeBlocks); //manual specific
                //File ARL 019AQ -> CodeBlock ID=5882 appered?
                Tools.Instance.m_codeBlock6ID = CheckCodeBlock(0x60000, allBytes, newSymbols, newCodeBlocks); //automatic specific
                Tools.Instance.m_codeBlock7ID = CheckCodeBlock(0x70000, allBytes, newSymbols, newCodeBlocks); //quattro specific
            }
        }
    }
}
