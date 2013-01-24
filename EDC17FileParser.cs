using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    public class EDC17FileParser : IEDCFileParser
    {
        public override SymbolCollection parseFile(string filename, out List<CodeBlock> newCodeBlocks, out List<AxisHelper> newAxisHelpers)
        {
            newCodeBlocks = new List<CodeBlock>();
            SymbolCollection newSymbols = new SymbolCollection();
            newAxisHelpers = new List<AxisHelper>();
            // Bosch EDC17 style mapdetection LL LL AXIS AXIS MAPDATA
            byte[] allBytes = File.ReadAllBytes(filename);
			// EDC17 = ReverseEndian
			allBytes = Tools.Instance.reverseEndian(allBytes);
            for (int i = 0; i < allBytes.Length - 32; i += 2)
            {
                int len2Skip = CheckMap(i, allBytes, newSymbols, newCodeBlocks);
                if ((len2Skip % 2) > 0)
                {
                    if (len2Skip > 2) len2Skip--;
                    else len2Skip++;
                }
				//i += len2Skip;
				//i += len2Skip - 2;
				// For now we don't skip since we can have false alarm and thereby jumping over the start of the "real" map.
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


		// EDC17 Little<->Bigendian compared to EDC16
        private int CheckMap(int t, byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
            int retval = 0;
            // read LL LL 
            int len1 = Convert.ToInt32(allBytes[t]) * 256 + Convert.ToInt32(allBytes[t+1]);
            int len2 = Convert.ToInt32(allBytes[t+2]) * 256 + Convert.ToInt32(allBytes[t + 3]);

			if(len1 == 1 && len2 == 1)
				return retval;
			// BMW uses up to 40(0x28) for one axis.
            if ((len1 <= 64 && len2 <= 64) && (len1 > 0 && len2 >= 0)) {
				//Console.WriteLine("---New map-- t=" + t.ToString("X"));
				bool ok = true;
				int startX = t + 2; // For 0D-arrays
				if(len2 > 0)
					startX += 2; // For 2D and 3D-arrays
				for(int dX = startX;dX <= startX + len1 * 2 - 4;dX += 2) {
					int b1 = Convert.ToInt32(allBytes[dX]) * 256 + Convert.ToInt32(allBytes[dX+1]);
					int b2 = Convert.ToInt32(allBytes[dX+2]) * 256 + Convert.ToInt32(allBytes[dX + 3]);
					if(b1 >= b2) {
						ok = false;
						break;
					}
				}
				//Console.WriteLine("-----");
				if(len2 > 0) {
					for(int dY = startX + len1 * 2;dY <= startX + len1 * 2 + len2 * 2 - 4;dY += 2) {
						int b1 = Convert.ToInt32(allBytes[dY]) * 256 + Convert.ToInt32(allBytes[dY + 1]);
						int b2 = Convert.ToInt32(allBytes[dY + 2]) * 256 + Convert.ToInt32(allBytes[dY + 3]);
						if(b1 >= b2) {
							ok = false;
							break;
						}
					}
				}

				if(ok) {
					//Console.WriteLine("--------------");
					//for(int i=t; i<t+len1*len2*2;i++)
					//	Console.Write(allBytes[t].ToString("X2"));
					//Console.WriteLine("----end----");

					SymbolHelper sh = new SymbolHelper();
					sh.X_axis_length = len1;
					if(len2 == 0) {
						sh.X_axis_address = t + 2;
						sh.Y_axis_address = t + 2;
						sh.Y_axis_length = 1;
						sh.Flash_start_address = t + 2 + len1*2;
						sh.Varname = "1D " + sh.Flash_start_address.ToString("X8");
						sh.Is1D = true;
					} else {
						sh.X_axis_address = t + 4;
						sh.Y_axis_length = len2;
						sh.Y_axis_address = sh.X_axis_address + sh.X_axis_length * 2;
						sh.Flash_start_address = sh.Y_axis_address + sh.Y_axis_length * 2;
						if(sh.X_axis_length > 1 && sh.Y_axis_length > 1) {
							sh.Varname = "3D " + sh.Flash_start_address.ToString("X8");
							sh.Is3D = true;
						} else {
							sh.Varname = "2D " + sh.Flash_start_address.ToString("X8");
							sh.Is2D = true;
						}
					}
					sh.Length = sh.X_axis_length * sh.Y_axis_length * 2;
					//sh.Varname = sh.Flash_start_address.ToString("X8");
					int length = (len1 + len2) * 2 + sh.Length + 4;
					sh.Currentdata = new byte[length];
					Array.Copy(allBytes, t, sh.Currentdata, 0, length);

					AddToSymbolCollection(newSymbols, sh, newCodeBlocks);
					retval = (len1 + len2) * 2 + sh.Length;
				}
            }
            return retval;
        }

        private bool AddToSymbolCollection(SymbolCollection newSymbols, SymbolHelper newSymbol, List<CodeBlock> newCodeBlocks)
        {
            //if (newSymbol.Length >= 800) return false;
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

        public override void NameKnownMaps(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks)
        {
			SymbolAxesTranslator st = new SymbolAxesTranslator();
			foreach(SymbolHelper sh in newSymbols) {
                if (Tools.Instance.m_carMake == CarMakes.BMW)
                {
                    //Console.WriteLine("Flash startadress: " + sh.Flash_start_address);		
                    // Look in X-axis for RPM
                    int maxX = 0;
                    int minX = 0;
                    int maxY = 0;
                    int minY = 0;
                    if (sh.Is1D)
                    {
                        maxX = (sh.Currentdata[2 + sh.X_axis_length * 2 - 2]) * 256 +
                               (sh.Currentdata[2 + sh.X_axis_length * 2 - 1]);
                        minX = maxX;/*		(sh.Currentdata[2]) * 256 +	(sh.Currentdata[3]); */
                    }
                    else
                    {
                        maxX = (sh.Currentdata[4 + sh.X_axis_length * 2 - 2]) * 256 +
                                (sh.Currentdata[4 + sh.X_axis_length * 2 - 1]);
                        minX = (sh.Currentdata[4]) * 256 + (sh.Currentdata[5]);
                    }

                    //Console.WriteLine("" + sh.Flash_start_address + sh.Y_axis_offset);
                    // Check max value in Y for 9200 -> RPM
                    if (sh.Is1D)
                    {
                        maxY = (sh.Currentdata[2 + sh.X_axis_length * 2 + sh.Y_axis_length * 2 - 2]) * 256 +
                               (sh.Currentdata[2 + sh.X_axis_length * 2 + sh.Y_axis_length * 2 - 1]);
                        minY = (sh.Currentdata[2 + sh.X_axis_length * 2]) * 256 +
                               (sh.Currentdata[2 + sh.X_axis_length * 2 + 1]);
                    }
                    else
                    {
                        maxY = (sh.Currentdata[4 + sh.X_axis_length * 2 + sh.Y_axis_length * 2 - 2]) * 256 +
                               (sh.Currentdata[4 + sh.X_axis_length * 2 + sh.Y_axis_length * 2 - 1]);
                        minY = (sh.Currentdata[4 + sh.X_axis_length * 2]) * 256 +
                               (sh.Currentdata[4 + sh.X_axis_length * 2 + 1]);
                    }
                    //Console.WriteLine("MinX=" + minX + ", maxX=" + maxX);
                    //Console.WriteLine("MinY=" + minY + ", maxY=" + maxY);
                    /*
                    if((maxX == 10000 && minX == 1400) || // 750->5000rpm
                       (maxX == 10000 && minX == 1000) || // 500->5000rpm
                       (maxX == 9200)) {				  // ->4600rpm
                        //sh.Category = "Partly detected maps";
                        //sh.Subcategory = "RPM";
                        sh.Y_axis_correction = 0.5;
                        sh.YaxisUnits = "rpm";
                        sh.Y_axis_descr = "rpm";
                    }
                    */
                    // Temp-related X-axis
                    if ((minX >= 1731 && maxX <= 3931) &&
                        ((minX - 31) % 50 == 0 ||
                          (maxX - 31) % 50 == 0
                        ))
                    {
                        sh.X_axis_correction = 0.1;
                        sh.X_axis_offset = -273.1;
                        //sh.Correction = 0.1;
                        sh.Category = "Partly detected maps - temp";
                        sh.X_axis_descr = "Temperature";
                        sh.XaxisUnits = "C";
                    }
                    if ((sh.X_axis_length == 16 || sh.X_axis_length == 17) && sh.Y_axis_length == 40)
                    {
                        //Injection time
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Injection timing";
                        sh.Varname = "Injection time [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.X_axis_correction = 0.01;
                        sh.XaxisUnits = "mm3/inj";
                        sh.X_axis_descr = "mm3/inj";
                        sh.Z_axis_descr = "time";
                        sh.Correction = 0.4;
                        sh.Y_axis_correction = 0.1;
                        sh.YaxisUnits = "bar";
                        sh.Y_axis_descr = "bar";
                    }
                    if (sh.X_axis_length == 16 && sh.Y_axis_length == 16 && (maxY == 7000 || maxY == 7500) && minY == 0)
                    {
                        // Todo! Check steps in X and/or Y to further isolate maps!
                        // Injection timing
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Injection timing";
                        sh.Varname = "Injection timing map [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.X_axis_correction = 0.01;
                        sh.XaxisUnits = "mm3/cyc";
                        sh.X_axis_descr = "mm3/cyc";
                        sh.Z_axis_descr = "Injection timing";
                        sh.Correction = 0.021969;
                    }
                    if ((sh.X_axis_length == 14 && sh.Y_axis_length == 8))
                    { //|| // 14x8
                        //(sh.X_axis_length == 8 && sh.Y_axis_length == 1 ) ) { // 8x1(0) torquelimiter
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Torque";
                        sh.Varname = "Pedal request map [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.X_axis_correction = 0.01220703125;
                        sh.X_axis_descr = "Percent";
                        sh.Y_axis_correction = 0.5;
                        sh.Y_axis_descr = "RPM";
                        sh.Z_axis_descr = "Torque (Nm)";
                        sh.XaxisUnits = "%";
                        sh.Correction = 0.1;
                    }
                    if (sh.X_axis_length == 8 && sh.Y_axis_length == 10 &&
                        (maxY % 2 == 1 || minY % 2 == 1))
                    { // 8x10 and odd data
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Torque";
                        sh.Varname = "Torque limit given temp [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.Y_axis_correction = 0.5;
                        sh.Y_axis_descr = "RPM";
                        sh.X_axis_correction = 0.1;
                        sh.X_axis_offset = -273.1;
                        sh.Correction = 0.1;
                        sh.X_axis_descr = "Temperature";
                        sh.Z_axis_descr = "Torque (Nm)";
                        sh.XaxisUnits = "C";
                    }
                    if (sh.X_axis_length == 21 && sh.Y_axis_length == 3)
                    { // 21x3, only one exist in BMW-files
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Torque";
                        sh.Varname = "Torque limit for speed [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.Y_axis_correction = 0.5;
                        sh.Y_axis_descr = "RPM";
                        sh.X_axis_correction = 1;
                        sh.Correction = 0.1;
                        sh.X_axis_descr = "#";
                        sh.Z_axis_descr = "Torque (Nm)";
                        sh.XaxisUnits = "#";
                    }
                    /* // These addresses are only valid for one specific boschnumber.... Needs to be more general
                    if(sh.Flash_start_address == 0x75200 ||
                        sh.Flash_start_address == 0x75444 ||
                        sh.Flash_start_address == 0x75688) {
                        sh.Varname = "Fuel quantity injected [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Injected Fuel";
                        sh.Y_axis_correction = 0.5;
                        sh.YaxisUnits = "RPM";
                        sh.Y_axis_descr = "rpm";
                        sh.X_axis_correction = 0.1;
                        sh.XaxisUnits = "Nm";
                        sh.X_axis_descr = "Nm";
                        sh.Z_axis_descr = "Fuel injected quantity f(RPM,Trq)";
                        sh.Correction = 0.01;
                    }

                    if(sh.Flash_start_address == 0x52036) {
                        sh.Varname = "Gas quantity recirculation [" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Air control";

                        sh.Y_axis_correction = 0.5;
                        sh.YaxisUnits = "RPM";
                        sh.Y_axis_descr = "rpm";

                        sh.X_axis_correction = 0.01;
                        sh.XaxisUnits = "mm3/hub";
                        sh.X_axis_descr = "mm3";
                        sh.Z_axis_descr = "Gas quantity";

                        sh.Correction = 0.1;
                    }
                    if(sh.Flash_start_address == 0x6EFE4) {
                        sh.Varname = "Limiter of turbo f(APS)[" + sh.Flash_start_address.ToString("X8") + "]";
                        sh.Category = "Detected maps";
                        sh.Subcategory = "Turbo";

                        sh.Y_axis_correction = 0.5;
                        sh.YaxisUnits = "RPM";
                        sh.Y_axis_descr = "rpm";

                        sh.X_axis_correction = 1;
                        sh.XaxisUnits = "hPa";
                        sh.X_axis_descr = "hPa";
                        sh.Z_axis_descr = "Pressure";

                        sh.Correction = 1;
                    }
                     */
                }
			}
		}		
    }
}
