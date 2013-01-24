using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    public class EDC16FileParser : IEDCFileParser
    {
        public override SymbolCollection parseFile(string filename, out List<CodeBlock> newCodeBlocks, out List<AxisHelper> newAxisHelpers)
        {
            newCodeBlocks = new List<CodeBlock>();
            SymbolCollection newSymbols = new SymbolCollection();
            newAxisHelpers = new List<AxisHelper>();
            // Bosch EDC16 style mapdetection LL LL AXIS AXIS MAPDATA
            byte[] allBytes = File.ReadAllBytes(filename);

            for (int i = 0; i < allBytes.Length - 32; i+=2)
            {
                int len2Skip = CheckMap(i, allBytes, newSymbols, newCodeBlocks);
                if ((len2Skip % 2) > 0)
                {
                    if (len2Skip > 2) len2Skip--;
                    else len2Skip++;
                }
                i += len2Skip;
            }
            newSymbols.SortColumn = "Flash_start_address";
            newSymbols.SortingOrder = GenericComparer.SortOrder.Ascending;
            newSymbols.Sort();
            NameKnownMaps(allBytes, newSymbols, newCodeBlocks);
            FindSVBL(allBytes, filename, newSymbols, newCodeBlocks);
            SymbolTranslator strans = new SymbolTranslator();
            foreach (SymbolHelper sh in newSymbols)
            {
                sh.Description = strans.TranslateSymbolToHelpText(sh.Varname);
            }
            return newSymbols;       
        }

        private int CheckMap(int t, byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            int retval = 0;
            // read LL LL 
            int len1 = Convert.ToInt32(allBytes[t]) * 256 + Convert.ToInt32(allBytes[t + 1]);
            int len2 = Convert.ToInt32(allBytes[t + 2]) * 256 + Convert.ToInt32(allBytes[t + 3]);
            if (len1 < 32 && len2 < 32 && len1 > 0 && len2 > 0)
            {
                SymbolHelper sh = new SymbolHelper();
                sh.X_axis_address = t + 4;
                sh.X_axis_length = len1;
                sh.Y_axis_address = sh.X_axis_address + sh.X_axis_length * 2;
                sh.Y_axis_length = len2;
                sh.Flash_start_address = sh.Y_axis_address + sh.Y_axis_length * 2;
                sh.Length = sh.X_axis_length * sh.Y_axis_length * 2;
                if (sh.X_axis_length > 1 && sh.Y_axis_length > 1)
                {
                    sh.Varname = "3D " + sh.Flash_start_address.ToString("X8");
                }
                else
                {
                    sh.Varname = "2D " + sh.Flash_start_address.ToString("X8");
                }
                
                AddToSymbolCollection(newSymbols, sh, newCodeBlocks);
                retval = (len1 + len2) * 2 + sh.Length ;

            }
            return retval;
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

        public override void FindSVBL(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            
        }
        private string DetermineNumberByFlashBank(long address, List<CodeBlock> currBlocks)
        {
            foreach (CodeBlock cb in currBlocks)
            {
                if (cb.StartAddress <= address && cb.EndAddress >= address)
                {
                    if (cb.CodeID == 1) return "codeblock 1";// - MAN";
                    if (cb.CodeID == 2) return "codeblock 2";// - AUT (hydr)";
                    if (cb.CodeID == 3) return "codeblock 3";// - AUT (elek)";
                    return cb.CodeID.ToString();
                }
            }
            long bankNumber = address / 0x10000;
            return "flashbank " + bankNumber.ToString();
        }

        

        public override void NameKnownMaps(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            SymbolAxesTranslator st = new SymbolAxesTranslator();

            foreach (SymbolHelper sh in newSymbols)
            {
               // sh.X_axis_descr = st.TranslateAxisID(sh.X_axis_ID);
               // sh.Y_axis_descr = st.TranslateAxisID(sh.Y_axis_ID);
                if (sh.X_axis_length == 4 && sh.Y_axis_length == 20)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Limiters";
                    //int lmCount = GetMapNameCountForCodeBlock("Torque limiter", sh.CodeBlock, newSymbols, false);
                    sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Z_axis_descr = "Maximum IQ (mg)";
                    //sh.Y_axis_descr = "Atm. pressure (mbar)";
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.Correction = 0.01;
                    sh.YaxisUnits = "rpm";
                }

                if (sh.X_axis_length == 4 && sh.Y_axis_length == 21)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Limiters";
                    //int lmCount = GetMapNameCountForCodeBlock("Torque limiter", sh.CodeBlock, newSymbols, false);
                    sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Z_axis_descr = "Maximum IQ (mg)";
                    //sh.Y_axis_descr = "Atm. pressure (mbar)";
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.Correction = 0.01;
                    sh.YaxisUnits = "rpm";
                }
                else if (sh.X_axis_length == 3 && sh.Y_axis_length == 21)
                {
                    if (!CollectionContainsMapInSize(newSymbols, 21, 4))
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        //int lmCount = GetMapNameCountForCodeBlock("Torque limiter", sh.CodeBlock, newSymbols, false);
                        sh.Varname = "Torque limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        //sh.Y_axis_descr = "Atm. pressure (mbar)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.Correction = 0.01;
                        sh.YaxisUnits = "rpm";
                    }
                }
                else if (sh.X_axis_length == 16 && sh.Y_axis_length == 8)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Misc";
                    int dwCount = GetMapNameCountForCodeBlock("Driver wish ", sh.CodeBlock, newSymbols, false);
                            
                    sh.Varname = "Driver wish (" + dwCount.ToString() + ") [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Correction = 0.01;
                    sh.X_axis_correction = 0.01;
                    sh.X_axis_descr = "Throttle  position";
                    sh.Z_axis_descr = "Requested IQ (mg)";
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.YaxisUnits = "rpm";
                    sh.XaxisUnits = "TPS %";
                }
                else if (sh.X_axis_length == 15 && sh.Y_axis_length == 16)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Misc";
                    sh.Varname = "IQ to Torque conversion [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                    sh.Correction = 0.01;
                    sh.X_axis_correction = 0.1;
                    sh.X_axis_descr = "Torque";
                    sh.Z_axis_descr = "IQ (mg)";
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.YaxisUnits = "rpm";
                    sh.XaxisUnits = "Nm";
                }
                else if (sh.X_axis_length == 11 && sh.Y_axis_length == 10)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Limiters";
                    sh.Varname = "Boost limit map [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                    //   sh.Correction = 0.01;
                    //sh.X_axis_correction = 0.01;
                    sh.X_axis_descr = "Atmospheric pressure (mbar)";
                    sh.Z_axis_descr = "Maximum boost pressure (mbar)";
                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.YaxisUnits = "rpm";
                    sh.XaxisUnits = "mbar";
                }
                else if (sh.X_axis_length == 19 && sh.Y_axis_length == 15)
                {
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Fuel";
                    int injDurCount = GetMapNameCountForCodeBlock("Injector duration", sh.CodeBlock, newSymbols, false);
                    sh.Varname = "Injector duration " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                    sh.Y_axis_correction = 0.01; // TODO: Check for x or y
                    sh.Correction = 0.023437;
                    sh.X_axis_descr = "Engine speed (rpm)";
                    //sh.Y_axis_descr = "Airflow mg/stroke";
                    sh.Y_axis_descr = "Requested Quantity mg/stroke";

                    sh.Z_axis_descr = "Duration (crankshaft degrees)";
                    sh.XaxisUnits = "rpm";
                    sh.YaxisUnits = "mg/st";
                }
                else if (sh.X_axis_length == 16 && sh.Y_axis_length == 12)
                {
                    //int xmax = GetMaxAxisValue(allBytes, sh, MapViewerEx.AxisIdent.X_Axis);
                    //int ymax = GetMaxAxisValue(allBytes, sh, MapViewerEx.AxisIdent.Y_Axis);
                    //Console.WriteLine(xmax.ToString() + " " + ymax.ToString() + " " + sh.Flash_start_address.ToString("X8"));
                    if (GetMaxAxisValue(allBytes, sh, MapViewerEx.AxisIdent.Y_Axis) < 4000)
                    {
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Limiters";
                        sh.Varname = "Smoke limiter [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";
                        sh.Z_axis_descr = "Maximum IQ (mg)";
                        sh.Y_axis_descr = "Engine speed (rpm)";
                        sh.X_axis_descr = "Manifold pressure (mbar)";
                        sh.Correction = 0.01;
                        //sh.X_axis_correction = 0.1; // TODO: Check for x or y
                        sh.YaxisUnits = "rpm";
                        sh.XaxisUnits = "mbar";
                    }
                }
                else if (sh.X_axis_length == 16 && sh.Y_axis_length == 14)
                {
                    // SOI
                    sh.Category = "Detected maps";
                    sh.Subcategory = "Fuel";
                    int injDurCount = GetMapNameCountForCodeBlock("Start of injection (SOI)", sh.CodeBlock, newSymbols, false);

                    //Console.WriteLine("Temperature switch for SOI " + injDurCount.ToString() + " " + tempRange.ToString());
                    //sh.Varname = "Start of injection (SOI) " + injDurCount.ToString("D2") + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");
                    sh.Varname = "Start of injection (SOI) " + injDurCount.ToString() + " [" + DetermineNumberByFlashBank(sh.Flash_start_address, newCodeBlocks) + "]";// " + sh.Flash_start_address.ToString("X8") + " " + sh.X_axis_ID.ToString("X4") + " " + sh.Y_axis_ID.ToString("X4");

                    sh.Correction = 0.023437;
                    //sh.Offset = 78;

                    sh.Y_axis_descr = "Engine speed (rpm)";
                    sh.YaxisUnits = "rpm";
                    sh.X_axis_correction = 0.01; // TODODONE : Check for x or y
                    sh.XaxisUnits = "mg/st";

                    sh.X_axis_descr = "IQ (mg/stroke)";
                    sh.Z_axis_descr = "Start position (degrees BTDC)";
                }
            }
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
                    int val = Convert.ToInt32(allBytes[offset+1]) + Convert.ToInt32(allBytes[offset]) * 256;
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
                    int val = Convert.ToInt32(allBytes[offset+1]) + Convert.ToInt32(allBytes[offset]) * 256;
                    if (val > retval) retval = val;
                    offset += 2;
                }
            }
            return retval;
        }
        private bool CollectionContainsMapInSize(SymbolCollection newSymbols, int ysize, int xsize)
        {
            foreach (SymbolHelper sh in newSymbols)
            {
                if (sh.Y_axis_length == ysize && sh.X_axis_length == xsize) return true;
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

    }
}
