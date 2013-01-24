using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    public class EDC15CFileParser : IEDCFileParser
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
            /*string retval = string.Empty;
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
            return retval;*/
        }

        private string DetermineNumberByFlashBank(long address, List<CodeBlock> currBlocks)
        {
            foreach (CodeBlock cb in currBlocks)
            {
                if (cb.StartAddress <= address && cb.EndAddress >= address)
                {
                   /* if (cb.CodeID == 1) return "codeblock 1";// - MAN";
                    if (cb.CodeID == 2) return "codeblock 2";// - AUT (hydr)";
                    if (cb.CodeID == 3) return "codeblock 3";// - AUT (elek)";
                    return cb.CodeID.ToString();*/
                    if (cb.BlockGearboxType == GearboxType.Automatic)
                    {
                        return "codeblock " + cb.CodeID.ToString() + " AUT";
                    }
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
            string additionalInfo = ExtractInfo(allBytes);
            string partnumber = ExtractPartnumber(allBytes);
            partNumberConverter pnc = new partNumberConverter();
            ECUInfo info = pnc.ConvertPartnumber(boschnumber, allBytes.Length);
            // check if V6 2.5 TDI/R4 1.9 TDI/R3 1.4 TDi
            int nrCylinders = pnc.GetNumberOfCylinders(info.EngineType, additionalInfo);
            // we can detect maps depending on ECUType/EngineType/nrCylinders/Filesize etc
            

            VerifyCodeBlocks(allBytes, newSymbols, newCodeBlocks);

            for (int t = 0; t < allBytes.Length - 1; t += 2)
            {
                int len2skip = 0;
                if (CheckMap(t, allBytes, newSymbols, newCodeBlocks, out len2skip))
                {
                    if (len2skip > 2) len2skip -= 2; // make sure we don't miss maps
                    if ((len2skip % 2) > 0) len2skip -= 1;
                    if (len2skip < 0) len2skip = 0;
                   // t += len2skip;
                    // Console.WriteLine("map detected: " + t.ToString("X8"));
                }
            }

            newSymbols.SortColumn = "Flash_start_address";
            newSymbols.SortingOrder = GenericComparer.SortOrder.Ascending;
            newSymbols.Sort();
            NameKnownMaps(allBytes, newSymbols, newCodeBlocks);
            BuildAxisIDList(newSymbols, newAxisHelpers);
            MatchAxis(newSymbols, newAxisHelpers);

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
        public override void FindSVBL(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            bool found = true;
            int offset = 0;
            while (found)
            {
                int SVBLAddress = Tools.Instance.findSequence(allBytes, offset, new byte[10] { 0xDF, 0x7A, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDF, 0x7A }, new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                if (SVBLAddress > 0)
                {
                    //Console.WriteLine("Alternative SVBL " + SVBLAddress.ToString("X8"));
                    SymbolHelper shsvbl = new SymbolHelper();
                    shsvbl.Category = "Detected maps";
                    shsvbl.Subcategory = "Limiters";
                    shsvbl.Flash_start_address = SVBLAddress - 2;

                    // if value = 0xC3 0x00 -> two more back
                    int[] testValue = Tools.Instance.readdatafromfileasint(filename, (int)shsvbl.Flash_start_address, 1, EDCFileType.EDC15C);
                    if (testValue[0] == 0xC300) shsvbl.Flash_start_address -= 2;

                    shsvbl.Varname = "SVBL Boost limiter [" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks) + "]";
                    shsvbl.Length = 2;
                    shsvbl.CodeBlock = DetermineCodeBlockByByAddress(shsvbl.Flash_start_address, newCodeBlocks);
                    newSymbols.Add(shsvbl);
                    offset = SVBLAddress + 1;
                }
                else found = false;
            }
        }

        public override void NameKnownMaps(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            SymbolAxesTranslator st = new SymbolAxesTranslator();

            foreach (SymbolHelper sh in newSymbols)
            {
                //sh.X_axis_descr = st.TranslateAxisID(sh.X_axis_ID);
                //sh.Y_axis_descr = st.TranslateAxisID(sh.Y_axis_ID);
                if (sh.Length == 544)
                {
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 17)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            sh.Varname = "N146 Pump voltage map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            //sh.Correction = 0.001221001; // ?1.221001
                            //Todo: N146 correction factor EDC15C
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Pump voltage (V)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                }
                else if (sh.Length == 468)
                {
                    if (sh.X_axis_length == 13 && sh.Y_axis_length == 18)
                    {
                        if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID / 256 == 0xC0)
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
                    }
                }
                else if (sh.Length == 416)
                {
                    if (sh.X_axis_length == 0x10 && sh.Y_axis_length == 0x0d)
                    {
                        if (sh.X_axis_ID / 256 == 0xF9 && sh.Y_axis_ID / 256 == 0xDB)
                        {
                            // this is IQ by MAF limiter!
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "Smoke limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Airflow mg/stroke";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.1; 
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";

                        }
                        else if (sh.X_axis_ID / 256 == 0xF9 && sh.Y_axis_ID / 256 == 0xDA)
                        {
                            // this is IQ by MAF limiter!
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "Smoke limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Airflow mg/stroke";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.1; 
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";

                        }
                        else if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID / 256 == 0xDA)
                        {
                            // this is IQ by MAF limiter!
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "Smoke limiter II [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Airflow mg/stroke";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.1; 
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";

                        }
                        else if (sh.X_axis_ID / 256 == 0xDD && sh.Y_axis_ID / 256 == 0xDA)
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
                        else if (sh.X_axis_ID == 0xE08A && sh.Y_axis_ID == 0xDDD8)
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
                        else    if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID / 256 == 0xEA)
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
                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDE)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Start of injection (N108 SOI)", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Start of injection (N108 SOI) " + injDurCount.ToString() + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");

                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.X_axis_correction = 0.01; // TODODONE : Check for x or y
                            sh.XaxisUnits = "mg/st";
                            sh.X_axis_descr = "IQ (mg/stroke)";


                            sh.Correction = 0.023437;
                            //sh.Offset = 78;
                            sh.Z_axis_descr = "Start position (degrees BTDC)";
                        }
                    }
                }
                if (sh.Length == 384)
                {
                    
                    if (sh.X_axis_ID / 256 == 0xDD && sh.Y_axis_ID / 256 == 0xC0)
                    {
                        // inv driver wish
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
                    }
                    if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC1)
                    {
                        // inv driver wish
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
                    } 
                    if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDC)
                    {
                        // smoke limiter
                        if (sh.X_axis_length == 16 && sh.Y_axis_length == 12)
                        {
                            int smCount = GetMapNameCountForCodeBlock("Smoke limiter", sh.CodeBlock, newSymbols, false);
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            sh.Varname = "Smoke limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Airflow mg/stroke";
                            sh.Correction = 0.01;
                            sh.X_axis_correction = 0.1; 
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }

                }
                if (sh.Length == 320) // 16*10
                {
                    if (sh.X_axis_ID / 256 == 0xDD && sh.Y_axis_ID / 256 == 0xC0)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";
                        int tbCount = GetMapNameCountForCodeBlock("Boost target map", sh.CodeBlock, newSymbols, false);
                        sh.Varname = "Boost target map (" + tbCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        //Console.WriteLine("***** " + sh.Varname + " " + sh.Flash_start_address.ToString("X8"));
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Boost target (mbar)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID == 0xC0BA)
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
                    else if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID == 0xC036)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";
                        int tbCount = GetMapNameCountForCodeBlock("Boost target map", sh.CodeBlock, newSymbols, false);
                        sh.Varname = "Boost target map (" + tbCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Boost target (mbar)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC3)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";
                        int tbCount = GetMapNameCountForCodeBlock("Boost target map", sh.CodeBlock, newSymbols, false);
                        sh.Varname = "Boost target map (" + tbCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                        sh.X_axis_correction = 0.01;
                        sh.X_axis_descr = "IQ (mg/stroke)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Z_axis_descr = "Boost target (mbar)";
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mg/st";
                    }
                    
                }
                if (sh.Length == 312)
                {
                    if (sh.X_axis_length == 13 && sh.Y_axis_length == 12)
                    {
                        if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID / 256 == 0xD7)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            int injDurCount = GetMapNameCountForCodeBlock("Start of injection (N108 SOI)", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Start of injection (N108 SOI) " + injDurCount.ToString() + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.Correction = 0.023437;
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.X_axis_correction = 0.01; // TODODONE : Check for x or y
                            sh.XaxisUnits = "mg/st";
                            sh.X_axis_descr = "IQ (mg/stroke)";

                            sh.Z_axis_descr = "Start position (degrees BTDC)";
                        }
                    }
                }
                else if (sh.Length == 256) // 8*16
                {
                    /*if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC1)
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

                    }*/
                    if (sh.X_axis_ID / 256 == 0xDD && sh.Y_axis_ID / 256 == 0xC0)
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
                    if (sh.X_axis_ID / 256 == 0xC0 && sh.Y_axis_ID / 256 == 0xDD)
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
                    else if (sh.X_axis_ID / 256 == 0xC0 && sh.Y_axis_ID / 256 == 0xDC)
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
                    else if (sh.X_axis_ID  == 0xC2BE && sh.Y_axis_ID  == 0xE08A)
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
                }
                else if (sh.Length == 192) // 8*12
                {
                    if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC1)
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
                else if (sh.Length == 182) // 7*13
                {
                    if (sh.X_axis_ID / 256 == 0xDC && sh.Y_axis_ID / 256 == 0xC0)
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
                else if (sh.Length == 150)
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
                else if (sh.Length == 114)
                {
                    if (sh.X_axis_length == 3 && sh.Y_axis_length == 19)
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
            }

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
                int codeBlockAddress = Convert.ToInt32(allBytes[offset + 0x01002]) + Convert.ToInt32(allBytes[offset + 0x01003]) * 256 + offset;
                if (endOfTable == offset + 0xC3C3) return 0;
                codeBlockID = Convert.ToInt32(allBytes[codeBlockAddress]) + Convert.ToInt32(allBytes[codeBlockAddress + 1]) * 256;

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
            }
            newSymbols.Add(newSymbol);
            newSymbol.CodeBlock = DetermineCodeBlockByByAddress(newSymbol.Flash_start_address, newCodeBlocks);
            return true;
        }

        private bool isValidLength(int length, int id)
        {
            int idstrip = id / 256;
            if (idstrip == 0xEB /*|| idstrip == 0xDE*/)
            {
                if (length > 0 && length <= 32) return true;
            }
            else
            {
                if (length > 0 && length < 32) return true;
            }
            return false;
        }

        private bool isAxisID(int id)
        {
            int idstrip = id / 256;
            if (idstrip == 0xDB) return true;
            if (idstrip == 0xC0 || idstrip == 0xC1 || idstrip == 0xC2 || idstrip == 0xC3 || idstrip == 0xC4 || idstrip == 0xC5) return true;
            if (idstrip == 0xE0 || idstrip == 0xE4 || idstrip == 0xE5 || idstrip == 0xE9 || idstrip == 0xEA || idstrip == 0xEB || idstrip == 0xEC) return true;
            if (idstrip == 0xDA || idstrip == 0xDC || idstrip == 0xDD || idstrip == 0xDE) return true;
            if (idstrip == 0xF9 || idstrip == 0xFE) return true;
            if (idstrip == 0xD7 || idstrip == 0xE6) return true;
            if (idstrip == 0xD5) return true;
            if (idstrip == 0xD9 || idstrip == 0xE8) return true;
            if (idstrip == 0xD0) return true; //<20120822>
            if (idstrip == 0xCD || idstrip == 0xCB || idstrip == 0xCC || idstrip == 0xCA || idstrip == 0xC7) return true;
            if (idstrip == 0xC8 || idstrip == 0xC9 || idstrip == 0xC6 || idstrip == 0xCE || idstrip == 0xCF) return true;
            return false;
        }

        // we need to check AHEAD for selector maps
        // if these are present we may be facing a complex map structure
        // which we need to handle in a special way (selectors always have data like 00 01 00 02 00 03 00 04 etc)
        private bool CheckMap(int t, byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks, out int len2Skip)
        {
            len2Skip = 0;
            bool retval = false;
            bool mapFound = false;
            bool _dontGenMaps = false;
            List<MapSelector> mapSelectors = new List<MapSelector>();
            if (t < allBytes.Length - 0x100)
            {

                if (CheckAxisCount(t, allBytes, out mapSelectors) > 3)
                {
                    // check for selectors as well, and count them in the process
                    Console.WriteLine("Offset " + t.ToString("X8") + " has more than 3 consecutive axis");
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
                                    if (!_dontGenMaps)
                                    {
                                        // this has something to do with repeating several times with the same axis set

                                        Console.WriteLine("Added " + len2skip.ToString() + " because of z axis " + newSymbol.Flash_start_address.ToString("X8"));
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
                                        Console.WriteLine("Map start address = " + newSymbol.Flash_start_address.ToString("X8"));
                                        long lastFlashAddress = newSymbol.Flash_start_address;
                                        foreach (MapSelector ms in mapSelectors)
                                        {

                                            if (ms.NumRepeats > 0)
                                            {
                                                // check the memory size between the start of the map and the 
                                                // start of the map selector
                                                long memsize = ms.StartAddress - lastFlashAddress;
                                                memsize /= 2; // in words

                                                int mapsize = Convert.ToInt32(memsize) / ms.NumRepeats;
                                                //Console.WriteLine("selector: " + ms.StartAddress.ToString("X8") + " " + ms.MapLength.ToString() + " " + ms.NumRepeats.ToString());
                                                //Console.WriteLine("memsize = " + memsize.ToString() + " mapsize " + mapsize.ToString());
                                                //Console.WriteLine("starting at address: " + lastFlashAddress.ToString("X8"));
                                                if ((xaxislen * yaxislen) == mapsize)
                                                {
                                                    // first axis set
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
                                                        retval = AddToSymbolCollection(newSymbols, shGen2, newCodeBlocks);
                                                        if (retval)
                                                        {
                                                            mapFound = true;
                                                            len2Skip += (xaxislen * 2) + (yaxislen * 2) + shGen2.Length;
                                                            t += (xaxislen * 2) + (yaxislen * 2) + shGen2.Length;
                                                        }
                                                        lastFlashAddress = address + mapsize * 2;
                                                        // Console.WriteLine("Set last address to " + lastFlashAddress.ToString("X8"));
                                                    }
                                                    lastFlashAddress += ms.NumRepeats * 4 + 4;
                                                }
                                                else if ((zaxislen * maxislen) == mapsize)
                                                {
                                                    // second axis set
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
                                                        retval = AddToSymbolCollection(newSymbols, shGen2, newCodeBlocks);
                                                        if (retval)
                                                        {
                                                            mapFound = true;
                                                            len2Skip += (xaxislen * 2) + (yaxislen * 2) + shGen2.Length;
                                                            t += (xaxislen * 2) + (yaxislen * 2) + shGen2.Length;
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
                            retval = AddToSymbolCollection(newSymbols, newSymbol, newCodeBlocks);
                            if (retval)
                            {
                                mapFound = true;
                                len2Skip += (xaxislen * 2) + (yaxislen * 2) + newSymbol.Length;
                                t += (xaxislen * 2) + (yaxislen * 2) + newSymbol.Length;
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
                                len2Skip += (xaxislen * 2);
                                t += (xaxislen * 2);
                            }
                            // 2d map
                        }
                    }

                }
            }
            return mapFound;
        }

        private int CheckAxisCount(int offset, byte[] allBytes, out List<MapSelector> mapSelectors)
        {
            int axisCount = 0;
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

                                if (allBytes[t + 4 + (axislen * 2) + i] != 0)
                                {
                                    selectorValid = false;
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
                                mapSelectors.Add(newSel);

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
                Tools.Instance.m_codeBlock6ID = CheckCodeBlock(0x60000, allBytes, newSymbols, newCodeBlocks); //automatic specific
                Tools.Instance.m_codeBlock7ID = CheckCodeBlock(0x70000, allBytes, newSymbols, newCodeBlocks); //quattro specific
            }
        }

    }
}
