using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    public class MSA6FileParser : IEDCFileParser
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
                    /*if (cb.CodeID == 1) return "codeblock 1";// - MAN";
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
            partNumberConverter pnc = new partNumberConverter();

            VerifyCodeBlocks(allBytes, newSymbols, newCodeBlocks);

            for (int t = 0; t < allBytes.Length - 1; t++)
            {
                CheckMap(t, allBytes, newSymbols, newCodeBlocks);
            }

            newSymbols.SortColumn = "Flash_start_address";
            newSymbols.SortingOrder = GenericComparer.SortOrder.Ascending;
            newSymbols.Sort();
            NameKnownMaps(allBytes, newSymbols, newCodeBlocks);
            FindSVBL(allBytes, filename, newSymbols, newCodeBlocks);
            FindMAPMAFSwitch(allBytes, filename, newSymbols, newCodeBlocks);

            SymbolTranslator strans = new SymbolTranslator();
            foreach (SymbolHelper sh in newSymbols)
            {
                sh.Description = strans.TranslateSymbolToHelpText(sh.Varname);
            }
            // check for must have maps... if there are maps missing, report it
            return newSymbols;
        }

        private void FindMAPMAFSwitch(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            foreach (CodeBlock cb in newCodeBlocks)
            {
                if (cb.CodeID > 0)
                {
                    Console.WriteLine("codeblock " + cb.CodeID.ToString() + " address " + cb.StartAddress.ToString("X8") + " adrid " + cb.AddressID.ToString("X8"));
                    int MAPMAFSwitch = cb.AddressID - 8;
                    // read data
                    if ((allBytes[MAPMAFSwitch] == 0x01 && allBytes[MAPMAFSwitch + 1] == 0x01) || (allBytes[MAPMAFSwitch ] == 0x00 && allBytes[MAPMAFSwitch + 1] == 0x00))
                    {
                        // verify validity
                        SymbolHelper mapmafsh = new SymbolHelper();
                        //mapmafsh.BitMask = 0x0101;
                        mapmafsh.Category = "Detected maps";
                        mapmafsh.Subcategory = "Switches";
                        mapmafsh.Flash_start_address = MAPMAFSwitch;
                        mapmafsh.Varname = "MAP/MAF switch (0 = MAF, 257/0x0101 = MAP)" + DetermineNumberByFlashBank(MAPMAFSwitch, newCodeBlocks);
                        mapmafsh.Length = 2;
                        mapmafsh.CodeBlock = DetermineCodeBlockByByAddress(mapmafsh.Flash_start_address, newCodeBlocks);
                        newSymbols.Add(mapmafsh);
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
            int offset = 0;
            bool svblfound = false;
            while (found)
            {
                int SVBLAddress = Tools.Instance.findSequence(allBytes, offset, new byte[10] { 0xDF, 0x7A, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDF, 0x7A }, new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                if (SVBLAddress > 0)
                {
                    //Console.WriteLine("Alternative SVBL " + SVBLAddress.ToString("X8"));
                    svblfound = true;
                    SymbolHelper shsvbl = new SymbolHelper();
                    shsvbl.Category = "Detected maps";
                    shsvbl.Subcategory = "Limiters";
                    shsvbl.Flash_start_address = SVBLAddress - 2;

                    // if value = 0xC3 0x00 -> two more back
                    int[] testValue = Tools.Instance.readdatafromfileasint(filename, (int)shsvbl.Flash_start_address, 1, EDCFileType.MSA6);
                    if (testValue[0] == 0xC300) shsvbl.Flash_start_address -= 2;

                    shsvbl.Varname = "SVBL Boost limiter [" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks) + "]";
                    shsvbl.Length = 2;
                    shsvbl.CodeBlock = DetermineCodeBlockByByAddress(shsvbl.Flash_start_address, newCodeBlocks);
                    newSymbols.Add(shsvbl);
                    offset = SVBLAddress + 1;
                }
                else found = false;
            }
            return svblfound;
        }

        private bool FindSVBLSequenceOne(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            bool found = true;
            int offset = 0;
            bool svblfound = false;
            while (found)
            {
                int SVBLAddress = Tools.Instance.findSequence(allBytes, offset, new byte[10] { 0x2B, 0x7D, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x7D }, new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                if (SVBLAddress > 0)
                {
                    //Console.WriteLine("Alternative SVBL " + SVBLAddress.ToString("X8"));
                    svblfound = true;
                    SymbolHelper shsvbl = new SymbolHelper();
                    shsvbl.Category = "Detected maps";
                    shsvbl.Subcategory = "Limiters";
                    shsvbl.Flash_start_address = SVBLAddress - 2;

                    // if value = 0xC3 0x00 -> two more back
                    int[] testValue = Tools.Instance.readdatafromfileasint(filename, (int)shsvbl.Flash_start_address, 1, EDCFileType.MSA6);
                    if (testValue[0] == 0xC300) shsvbl.Flash_start_address -= 2;

                    shsvbl.Varname = "SVBL Boost limiter [" + DetermineNumberByFlashBank(shsvbl.Flash_start_address, newCodeBlocks) + "]";
                    shsvbl.Length = 2;
                    shsvbl.CodeBlock = DetermineCodeBlockByByAddress(shsvbl.Flash_start_address, newCodeBlocks);
                    newSymbols.Add(shsvbl);
                    offset = SVBLAddress + 1;
                }
                else found = false;
            }
            return svblfound;
        }

        public override void NameKnownMaps(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            //N146 Pump Voltage map:
            SymbolAxesTranslator st = new SymbolAxesTranslator();

            foreach (SymbolHelper sh in newSymbols)
            {
              //  sh.X_axis_descr = st.TranslateAxisID(sh.X_axis_ID);
              //  sh.Y_axis_descr = st.TranslateAxisID(sh.Y_axis_ID);

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
                            //Todo: N146 correction factor for MSA6
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Pump voltage (V)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                }
                if (sh.Length == 448)
                {
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 14)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            sh.Varname = "N146 Pump voltage map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            //sh.Correction = 0.001221001; // ?1.221001
                            //Todo: N146 correction factor for MSA6
                            sh.X_axis_correction = 0.01;
                            sh.Z_axis_descr = "Pump voltage (V)";
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                }
                //12x13 E0 DD = EGR
                if (sh.Length == 416)
                {
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 13)
                    {
                        if (sh.X_axis_ID == 0xE08A && sh.Y_axis_ID  == 0xDDD8)
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
                        else if (sh.X_axis_ID == 0xE08A && sh.Y_axis_ID == 0xDD30)
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
                        else if (sh.X_axis_ID == 0xE08A && sh.Y_axis_ID == 0xDEDC)
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
                        }
                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDE)
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
                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDD)
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
                            sh.Z_axis_descr = "Start position (degrees BTDC)";
                        }
                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDC)
                        {
                            // IQ by MAP
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
                }
                else if (sh.Length == 384)
                {
                    if (sh.X_axis_length == 12 && sh.Y_axis_length == 16)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
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
                    else if (sh.X_axis_length == 16 && sh.Y_axis_length == 12)
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
                else if (sh.Length == 320)
                {
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 10)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDC)
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
                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC3)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Turbo";
                            int boostTargetCount = GetMapNameCountForCodeBlock("Boost target map", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Boost target map (" + boostTargetCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.X_axis_correction = 0.01;
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Z_axis_descr = "Boost target (mbar)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Turbo";
                            int boostTargetCount = GetMapNameCountForCodeBlock("Boost target map", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Boost target map (" + boostTargetCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.X_axis_correction = 0.01;
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Z_axis_descr = "Boost target (mbar)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }

                        else if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xDD)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            sh.Varname = "IQ to airmass [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Required airmass";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01; 
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                        else if (sh.X_axis_ID == 0xE08A && sh.Y_axis_ID == 0xDEDC)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Fuel";
                            sh.Varname = "IQ to airmass [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Required airmass";
                            sh.Correction = 0.1;
                            sh.X_axis_correction = 0.01; 
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                        else if (sh.X_axis_ID == 0xE08A && sh.Y_axis_ID == 0xC2D0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Turbo";
                            int boostTargetCount = GetMapNameCountForCodeBlock("Boost target map", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Boost target map (" + boostTargetCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                            sh.X_axis_correction = 0.01;
                            sh.X_axis_descr = "IQ (mg/stroke)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Z_axis_descr = "Boost target (mbar)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "mg/st";
                        }
                    }
                }
                else if (sh.Length == 256)
                {
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 8)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
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
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC0)
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
                else if (sh.Length == 200)
                {
                    if (sh.X_axis_ID / 256 == 0xC2 && sh.Y_axis_ID / 256 == 0xE0)
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
                else if (sh.Length == 192)
                {
                    if (sh.X_axis_length == 12 && sh.Y_axis_length == 8)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
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
                }
                else if (sh.Length == 182) // 7*13
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
                    if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC2)
                    {
                        if (IsValidTemperatureAxis(allBytes, sh, MapViewerEx.AxisIdent.Y_Axis))
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            //sh.Varname = "Boost limiter (temperature) [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Varname = "SOI limiter (temperature) [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Correction = 0.023437;
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.X_axis_descr = "Temperature"; //IAT, ECT or Fuel temp?
                            sh.X_axis_correction = 0.1;
                            sh.X_axis_offset = -273.1;
                            sh.Z_axis_descr = "SOI limit (degrees)";
                            sh.YaxisUnits = "rpm";
                            sh.XaxisUnits = "°C";
                        }
                    }
                    
                }
                else if (sh.Length == 144)
                {
                    if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID / 256 == 0xC0)
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
                else if (sh.Length == 128)
                {
                    if (sh.X_axis_ID / 256 == 0xE0 && sh.Y_axis_ID == 0xC002)
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
                else if (sh.Length == 38)
                {
                    if (sh.X_axis_length == 19 && sh.Y_axis_length == 1)
                    {
                        if (sh.X_axis_ID / 256 == 0xE0)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Limiters";
                            int lmCount = GetMapNameCountForCodeBlock("Torque limiter", sh.CodeBlock, newSymbols, false);
                            sh.Varname = "Torque limiter (" + lmCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                            sh.Z_axis_descr = "Maximum IQ (mg)";
                            //sh.Y_axis_descr = "Atm. pressure (mbar)";
                            sh.Y_axis_descr = "Engine speed (rpm)";
                            sh.Correction = 0.01;
                            sh.YaxisUnits = "rpm";
                        }
                    }
                }
                else if (sh.Length == 4)
                {
                    if (sh.X_axis_length == 2 && sh.Y_axis_length == 1)
                    {
                        //if (sh.X_axis_ID >= 0xEBA0 && sh.X_axis_ID <= 0xEBAF)
                        if (sh.X_axis_ID == 0xDF4A)
                        {
                            sh.Category = "Detected maps";
                            sh.Subcategory = "Misc";
                            sh.Varname = "MAP linearization [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        }
                    }
                    if (sh.X_axis_length == 2 && sh.Y_axis_length == 1)
                    {
                        if (sh.X_axis_ID == 0xC002) // idle RPM
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
                int endOfTable = Convert.ToInt32(allBytes[offset ]) + Convert.ToInt32(allBytes[offset + 1]) * 256 + offset;
                int codeBlockAddress = Convert.ToInt32(allBytes[offset + 2]) + Convert.ToInt32(allBytes[offset + 3]) * 256 + offset;
                if (endOfTable == offset + 0xC3C3) codeBlockAddress = 0x1530 + offset;
                codeBlockAddress -= 0xc00;
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
            if (idstrip == 0xEB || idstrip == 0xDF)
            {
                if (length > 0 && length <= 32) return true;
            }
            else
            {
                if (length > 0 && length < 32) return true;
            }
//            if (length == 32 || length == 48 || length == 64) Console.WriteLine("seen id " + id.ToString("X4") + " with len " + length.ToString());
            return false;
        }

        private bool isAxisID(int id)
        {
            int idstrip = id / 256;
            if (idstrip == 0xDB) return true;
            if (idstrip == 0xC0 || idstrip == 0xC1 || idstrip == 0xC2 || idstrip == 0xC4 || idstrip == 0xC5) return true;
            if (idstrip == 0xE0 || idstrip == 0xE4 || idstrip == 0xE5 || idstrip == 0xE9 || idstrip == 0xEA || idstrip == 0xEB || idstrip == 0xEC) return true;
            if (idstrip == 0xDA || idstrip == 0xDC || idstrip == 0xDD || idstrip == 0xDE || idstrip == 0xDF) return true;
            if (idstrip == 0xF9 || idstrip == 0xFE || idstrip == 0xFC) return true;
            if (idstrip == 0xD7 || idstrip == 0xE6) return true;
            if (idstrip == 0xD5) return true;
            if (idstrip == 0xC3 || idstrip == 0xE7) return true;
            return false;
        }

        // we need to check AHEAD for selector maps
        // if these are present we may be facing a complex map structure
        // which we need to handle in a special way (selectors always have data like 00 01 00 02 00 03 00 04 etc)
        private bool CheckMap(int t, byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            bool retval = false;
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
                                        /*if (zaxislen <= 10)
                                        {
                                            for (int genMap = 0; genMap < zaxislen; genMap++)
                                            {
                                                SymbolHelper shGen = new SymbolHelper();
                                                shGen.X_axis_length = newSymbol.X_axis_length;
                                                shGen.Y_axis_length = newSymbol.Y_axis_length;
                                                shGen.X_axis_ID = newSymbol.X_axis_ID;
                                                shGen.Y_axis_ID = newSymbol.Y_axis_ID;
                                                shGen.X_axis_address = newSymbol.X_axis_address;
                                                shGen.Y_axis_address = newSymbol.Y_axis_address;
                                                shGen.Length = newSymbol.Length;
                                                shGen.Category = "Generated";
                                                long address = newSymbol.Flash_start_address + newSymbol.Length * (genMap + 1);
                                                shGen.Flash_start_address = address;
                                                shGen.Varname = "Generated " + shGen.Flash_start_address.ToString("X8");
                                                AddToSymbolCollection(newSymbols, shGen, newCodeBlocks);
                                            }
                                        }*/
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

                                            // check the memory size between the start of the map and the 
                                            // start of the map selector
                                            long memsize = ms.StartAddress - lastFlashAddress;
                                            memsize /= 2; // in words
                                            if (ms.NumRepeats > 0)
                                            {
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
                                                        //if (i < ms.NumRepeats - 1)
                                                        {
                                                            AddToSymbolCollection(newSymbols, shGen2, newCodeBlocks);
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

                                                        //if (i < ms.NumRepeats - 1)
                                                        {
                                                            AddToSymbolCollection(newSymbols, shGen2, newCodeBlocks);
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

                            t += (xaxislen * 2) + (yaxislen * 2) + newSymbol.Length;

                        }
                        else
                        {
                            /*bool skipped = false;
                            if (xaxislen == 0x19)
                            {
                                //Console.WriteLine("Address " + t.ToString("X8"));
                                if (allBytes[t - 31] == 0xC2 && allBytes[t - 30] == 0x02 && allBytes[t - 29] == 0x00 && allBytes[t - 23] == 0xEC && allBytes[t - 22] == 0x02 && allBytes[t - 21] == 0x00)
                                {
                                    // probably launch control map
                                    SymbolHelper newSymbol = new SymbolHelper();
                                    newSymbol.X_axis_length = 14;
                                    newSymbol.Y_axis_length = 25;
                                    newSymbol.X_axis_address = t - 28;
                                    newSymbol.X_axis_ID = 0xFFFF;
                                    newSymbol.Y_axis_ID = xaxisid;
                                    //newSymbol.X_axis_address = t - 28;// t + 4;
                                    newSymbol.Y_axis_address = t + 4;// t + 8 + (xaxislen * 2);
                                    newSymbol.Length = newSymbol.X_axis_length * newSymbol.Y_axis_length * 2;
                                    newSymbol.Flash_start_address = t + 4 + (newSymbol.Y_axis_length * 2);
                                    newSymbol.Varname = "3D " + newSymbol.Flash_start_address.ToString("X8") + " " + newSymbol.X_axis_ID.ToString("X4") + " " + newSymbol.Y_axis_ID.ToString("X4");
                                    retval = AddToSymbolCollection(newSymbols, newSymbol, newCodeBlocks);

                                    t += (newSymbol.X_axis_length * 2) + (newSymbol.Y_axis_length * 2) + newSymbol.Length;
                                    skipped = true;
                                }
                            }
                            if (!skipped)*/
                            {
                                //if (yaxisid != 0) <GS-20120820> switched off
                                {
                                    if (yaxisid > 0x9000 && yaxisid < 0xF000 && yaxislen <= 32) Console.WriteLine("Unknown map id: " + yaxisid.ToString("X4") + " len " + yaxislen.ToString("X4") + " at address " + t.ToString("X8"));
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
                                    t += (xaxislen * 2);
                                    // Console.WriteLine("t set to " + t.ToString("X8"));
                                }
                            }
                            // 2d map
                        }

                        // Console.WriteLine("t set to " + t.ToString("X8"));
                    }

                }
            }
            return retval;
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
                int CodeBlockAddress = Tools.Instance.findSequence(allBytes, offset, new byte[16] { 0x14, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                if (CodeBlockAddress > 0)
                {
                    CodeBlock newcodeblock = new CodeBlock();
                    newcodeblock.StartAddress = CodeBlockAddress +128;
                    if (prevCodeBlockStart == 0) prevCodeBlockStart = newcodeblock.StartAddress;
                    else if (currentCodeBlockLength == 0)
                    {
                        currentCodeBlockLength = newcodeblock.StartAddress - prevCodeBlockStart;
                        if (currentCodeBlockLength > 0x10000) currentCodeBlockLength = 0x10000;
                    }
                    // find the next occurence of the checksum
                    newCodeBlocks.Add(newcodeblock);
                    offset = CodeBlockAddress + 128;
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
                CheckCodeBlock(cb.StartAddress, allBytes, newSymbols, newCodeBlocks);
            }
        }

    }
}
