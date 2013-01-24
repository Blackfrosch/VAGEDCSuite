using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{

    public enum CarMakes : int
    {
        Audi,
        BMW,
        Volkswagen,
        Unknown
    }

    public sealed class Tools
    {
        private static volatile Tools instance;
        private static object syncRoot = new Object();

        public EDCFileType m_currentFileType = EDCFileType.EDC15P; // default

        public TransactionLog m_ProjectTransactionLog;
        public string m_CurrentWorkingProject = string.Empty;
        public ProjectLog m_ProjectLog = new ProjectLog();

        public string m_currentfile = string.Empty;
        public int m_currentfilelength = 0x80000;
        public int m_codeBlock5ID = 0;
        public int m_codeBlock6ID = 0;
        public int m_codeBlock7ID = 0;
        public CarMakes m_carMake = CarMakes.Unknown; // used for EDC17
        public List<CodeBlock> codeBlockList = new List<CodeBlock>();
        public SymbolCollection m_symbols = new SymbolCollection();
        public List<AxisHelper> AxisList = new List<AxisHelper>();
        public int TorqueToPowerkW(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            // convert to kW in stead of horsepower
            power *= 0.73549875;
            return Convert.ToInt32(power);
        }

        public string ExtractBoschPartnumber(byte[] allBytes)
        {
            string retval = string.Empty;
            try
            {
                int partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[5] { 0x45, 0x44, 0x43, 0x20, 0x20 }, new byte[5] { 1, 1, 1, 1, 1 });
                if (partnumberAddress > 0)
                {
                    // for EDC
                    retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress + 23, 10).Trim();
                    if (StripNonAscii(retval).Length < 10)
                    {
                        // try again, read from "EDC" id - 0x100 to EDC id + 100 and find 10 digit sequence
                        retval = FindDigits(allBytes, partnumberAddress - 0x100, partnumberAddress + 0x100, 10);
                    }
                    if (retval == string.Empty)
                    {
                        // try EDC16, other partnumber struct
                        retval = FindAscii(allBytes, partnumberAddress - 0x100, partnumberAddress + 0x100, 11);
                        if (retval.StartsWith("ECM"))
                        {
                            retval = FindAscii(allBytes, partnumberAddress - 0x100, partnumberAddress + 0x100, 19);
                        }
                    }
                }
                else
                {
                    partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[4] { 0x30, 0x32, 0x38, 0x31}, new byte[4] { 1, 1, 1, 1 });
                    if (partnumberAddress > 0)
                    {
                        retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress, 10).Trim();
                    }
                }
                if (retval == string.Empty)
                {
                    partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[4] { 0x30, 0x32, 0x38, 0x31 }, new byte[4] { 1, 1, 1, 1 });
                    if (partnumberAddress > 0)
                    {
                        retval = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress, 10).Trim();
                    }
                }
                if (retval == string.Empty)
                {
                    // check 512 kB EDC17 file, audi Q7 3.0TDI
                    partnumberAddress = Tools.Instance.findSequence(allBytes, 0, new byte[7] { 0x45, 0x44, 0x43, 0x31, 0x37, 0x20, 0x20 }, new byte[7] { 1, 1, 1, 1, 1, 1, 1 });
                    if (partnumberAddress > 0)
                    {
                        retval = "EDC17" + System.Text.ASCIIEncoding.ASCII.GetString(allBytes, partnumberAddress - 68, 10).Trim();
                    }
                    if (retval == string.Empty)
                    {
                        //EDC17_CPx4 = Audi
                        //EDC17_CPx2 = BMW?
                        partnumberAddress = Tools.Instance.findSequence(allBytes, 0, Encoding.ASCII.GetBytes("EDC17_"), new byte[6] { 1, 1, 1, 1, 1, 1 });
                        if (partnumberAddress > 0)
                        {
                            retval = "EDC17";
                        }
                    }
                   

                }
				if(retval == string.Empty) {

					// 1. General lookup for EDC17-string in file
					int pos = Tools.Instance.findSequence(allBytes, 0, Encoding.ASCII.GetBytes("ME(D)/EDC17"), new byte[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
					if(pos > 0) {
						Console.WriteLine("Found ME(D)/EDC17 in firmware!");
						retval = "EDC17";
					}
					// 2. BMW has 0281xxxxxx (boschnumber) written at FD00 and FE33
					// 3. BMW has 10373xxxxx (softwarenumber)written at 0x4001A, use this as a backup if boschnumber can't be found

					StringBuilder sb1 = new StringBuilder();
					StringBuilder sb2 = new StringBuilder();
					bool identified = true;
					// Check1
					for(int offset = 0xFD00;offset < 0xFD00 + 10;offset++) {
						byte b = allBytes[offset];
						b -= 0x30;
						if(b < 0 || b > 9)
							identified = false;
						sb1.Append(b.ToString());
					}
					for(int offset = 0xFE33;offset < 0xFE33 + 10;offset++) {
						byte b = allBytes[offset];
						b -= 0x30;
						if(b < 0 || b > 9)
							identified = false;
						sb2.Append(b.ToString());
					}
					//Console.WriteLine("sb1=" + sb1.ToString());
					//Console.WriteLine("sb2=" + sb2.ToString());
					if(identified && sb1.ToString() == sb2.ToString()) {
						partnumberAddress = 0xFD00;
						retval = sb1.ToString();
					}

					StringBuilder sb3 = new StringBuilder();
					identified = true;
					// Check2
					for(int offset = 0x4001A;offset < 0x4001A + 10;offset++) {
						byte b = allBytes[offset];
						b -= 0x30;
						if(b < 0 || b > 9)
							identified = false;
						sb3.Append(b.ToString());
					}
					//Console.WriteLine("sb3=" + sb3.ToString());
					if(identified) {
						partnumberAddress = 0x4001A;
						retval = sb3.ToString();
					}
				}
                if (retval.StartsWith("EDC17"))
                {
                    if (Tools.Instance.findSequence(allBytes, 0, Encoding.ASCII.GetBytes("EDC17_CPx4"), new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }) > 0)
                    {
                        Tools.Instance.m_carMake = CarMakes.Audi;
                    }
                    else if (Tools.Instance.findSequence(allBytes, 0, Encoding.ASCII.GetBytes("EDC17_CPx2"), new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }) > 0)
                    {
                        Tools.Instance.m_carMake = CarMakes.BMW;
                    }
                    else if (Tools.Instance.findSequence(allBytes, 0, Encoding.ASCII.GetBytes("EDC17_C06"), new byte[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 }) > 0)
                    {
                        Tools.Instance.m_carMake = CarMakes.BMW;
                    }
                }
			}
            catch (Exception)
            {
            }
            retval = StripNonAscii(retval);
            return retval;
        }


        private string FindAscii(byte[] allBytes, int start, int end, int length)
        {
            for (int i = start; i < end; i++)
            {
                string testStr = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, i, length);
                testStr = StripNonAsciiCapital(testStr);
                if (testStr.Length == length) return testStr;
            }
            return "";
        }

        private string FindDigits(byte[] allBytes, int start, int end, int length)
        {
            for (int i = start; i < end; i++)
            {
                string testStr = System.Text.ASCIIEncoding.ASCII.GetString(allBytes, i, length);
                testStr = StripNonDigit(testStr);
                if (testStr.Length == length) return testStr;
            }
            return "";
        }

        private string StripNonAsciiCapital(string input)
        {
            string retval = string.Empty;
            foreach (char c in input)
            {
                if (c >= 0x30 && c <= 0x39) retval += c;
                if (c >= 0x41 && c <= 0x5A) retval += c;
            }
            return retval;
        }

        private string StripNonDigit(string input)
        {
            string retval = string.Empty;
            foreach (char c in input)
            {
                if (c >= 0x30 && c <= 0x39) retval += c;
            }
            return retval;
        }

        public string StripNonAscii(string input)
        {
            string retval = string.Empty;
            foreach (char c in input)
            {
                if (c >= 0x30 && c <= 0x39) retval += c;
                else if (c >= 0x41 && c <= 0x5A) retval += c;
                else if (c >= 0x61 && c <= 0x7A) retval += c;
            }
            return retval;
        }

        public EDCFileType DetermineFileType(string fileName, bool isPrimaryFile)
        {

            byte[] allBytes = File.ReadAllBytes(fileName);
            string boschnumber = ExtractBoschPartnumber(allBytes);
            //Console.WriteLine("Bosch number: " + boschnumber);
            partNumberConverter pnc = new partNumberConverter();
            ECUInfo info = pnc.ConvertPartnumber(boschnumber, allBytes.Length);
            if (info.EcuType.Contains("EDC15P-6"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC15P6;
                return EDCFileType.EDC15P6;
            }
            else if (info.EcuType.Contains("EDC15P"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC15P;
                return EDCFileType.EDC15P;
            }
            else if (info.EcuType.Contains("EDC15M")) 
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC15M;
                return EDCFileType.EDC15M;
            }
            else if (info.EcuType.Contains("MSA15") || info.EcuType.Contains("EDC15V-5"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.MSA15;
                return EDCFileType.MSA15;
            }
            else if (info.EcuType.Contains("MSA12"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.MSA12;
                return EDCFileType.MSA12;
            }
            else if (info.EcuType.Contains("MSA11"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.MSA11;
                return EDCFileType.MSA11;
            }
            else if (info.EcuType.Contains("MSA6"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.MSA6;
                return EDCFileType.MSA6;
            }
            else if (info.EcuType.Contains("EDC15V"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC15V;
                return EDCFileType.EDC15V;
            }
            if (info.EcuType.Contains("EDC15C"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC15C;
                return EDCFileType.EDC15C;
            }
            else if (info.EcuType.Contains("EDC16"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC16;
                return EDCFileType.EDC16;
            }
            else if (info.EcuType.Contains("EDC17"))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC17;
                return EDCFileType.EDC17;
            }
            
            else if (IsEDC16Partnumber(boschnumber))
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC16;
                return EDCFileType.EDC16;
            }
            else if (boschnumber != string.Empty)
            {
                if (allBytes.Length == 1024 * 1024 * 2)
                {
                    if (isPrimaryFile) m_currentFileType = EDCFileType.EDC17;
                    return EDCFileType.EDC17;
                }
                else if (boschnumber.StartsWith("EDC17"))
                {
                    if (isPrimaryFile) m_currentFileType = EDCFileType.EDC17;
                    return EDCFileType.EDC17;
                }
                else
                {
                    if (isPrimaryFile) m_currentFileType = EDCFileType.EDC15V;
                    return EDCFileType.EDC15V;
                }
            }
            else
            {
                if (isPrimaryFile) m_currentFileType = EDCFileType.EDC16;
                return EDCFileType.EDC16; // default to EDC16???
            }
        }

        public bool IsEDC16Partnumber(string partnumber)
        {
            //03G906021AB 
            if(partnumber.Length != 11) return false;
            if(!isDigit(partnumber[0])) return false;
            if(!isDigit(partnumber[1])) return false;
            if(!isLetter(partnumber[2])) return false;
            if(!isDigit(partnumber[3])) return false;
            if(!isDigit(partnumber[4])) return false;
            if(!isDigit(partnumber[5])) return false;
            if(!isDigit(partnumber[6])) return false;
            if(!isDigit(partnumber[7])) return false;
            if(!isDigit(partnumber[8])) return false;
            if(!isLetter(partnumber[9])) return false;
            if(!isLetter(partnumber[10])) return false;
            return true;
            
        }

        private bool isLetter(char c)
        {
            if (c >= 0x41 && c <= 0x5A) return true;
            return false;
        }

        private bool isDigit(char c)
        {
            if (c >= 0x30 && c <= 0x39) return true;
            return false;
        }

        public IEDCFileParser GetParserForFile(string filename, bool isPrimaryFile)
        {
            IEDCFileParser parser = null;
            EDCFileType fileType = DetermineFileType(filename, isPrimaryFile);
            switch (fileType)
            {
                case EDCFileType.EDC15P:
                    parser = new EDC15PFileParser();
                    break;
                case EDCFileType.EDC15P6:
                    parser = new EDC15P6FileParser();
                    break;
                case EDCFileType.EDC15V:
                    parser = new EDC15VFileParser();
                    break;
                case EDCFileType.EDC15C:
                    parser = new EDC15CFileParser();
                    break;
                case EDCFileType.EDC15M:
                    parser = new EDC15MFileParser();
                    break;
                case EDCFileType.EDC16:
                    parser = new EDC16FileParser();
                    break;
                case EDCFileType.EDC17:
                    parser = new EDC17FileParser();
                    break;
                case EDCFileType.MSA15: //?
                case EDCFileType.MSA12:
                case EDCFileType.MSA11:
                    parser = new MSA15FileParser();
                    break;
                case EDCFileType.MSA6:
                    parser = new MSA6FileParser();
                    break;

            }
            return parser;
        }


        public int PowerToTorque(int power, int rpm)
        {
            double torque = (power * 7121) / rpm;
            return Convert.ToInt32(torque);
        }

        public int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            return Convert.ToInt32(power);
        }

        public double GetCorrectionFactorForRpm(int rpm, int numberCylinders)
        {
            double correction = 1;
            if (numberCylinders == 6)
            {
                if (rpm >= 4000) correction = 0.80;
                else if (rpm >= 3500) correction = 0.90;
                else if (rpm >= 3250) correction = 0.90;
                else if (rpm >= 3000) correction = 0.93;
                else if (rpm >= 2500) correction = 0.90;
                else if (rpm >= 2250) correction = 0.90;
                else if (rpm >= 1700) correction = 0.90;
                else correction = 0.9;
            }
            else
            {
                if (rpm >= 4000) correction = 0.75;
                else if (rpm >= 3500) correction = 0.83;
                else if (rpm >= 3250) correction = 0.89;
                else if (rpm >= 3000) correction = 0.96;
                else if (rpm >= 2500) correction = 0.98;
                else if (rpm >= 2250) correction = 0.99;
                else correction = 1.00;
            }
            return correction;

        }

        public int IQToTorque(int IQ, int rpm, int numberCylinders)
        {
            double tq = Convert.ToDouble(IQ) * 6;

            // correct for number of cylinders
            tq *= numberCylinders;
            tq /= 4;

            double correction = GetCorrectionFactorForRpm(rpm, numberCylinders);
            tq *= correction;
            return Convert.ToInt32(tq);
        }

        public int TorqueToIQ(int torque, int rpm, int numberCylinders)
        {
            double iq = Convert.ToDouble(torque) / 6;

            // correct for number of cylinders
            iq *= 4;
            iq /= numberCylinders;
            

            double correction = GetCorrectionFactorForRpm(rpm, numberCylinders);
            iq /= correction;
            return Convert.ToInt32(iq);
        }

        public Int64 GetSymbolAddressLike(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname.StartsWith(symbolname) || sh.Userdescription.StartsWith(symbolname))
                {
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }

        public static Tools Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Tools();
                        }
                    }
                }

                return instance;
            }
        }

        public string GetWorkingDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VAGEDCSuite");
        }

        public string GetSymbolNameByAddress(Int32 address)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Flash_start_address == address) return sh.Varname;
            }
            return address.ToString();
        }

        public string GetSymbolNameByAddressInRange(Int32 address, Int32 addressTo)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Flash_start_address <= address && (sh.Flash_start_address + sh.Length) >= address) return sh.Varname;
                if (sh.Flash_start_address <= addressTo && (sh.Flash_start_address + sh.Length) >= addressTo) return sh.Varname;
                if (sh.Flash_start_address >= address && (sh.Flash_start_address + sh.Length) <= addressTo) return sh.Varname;
            }
            return string.Empty;
        }

        public void savedatatobinary(int address, int length, byte[] data, string filename, bool DoTransActionEntry, string note, EDCFileType type)
        {
            // depends on filetype (EDC16 is not reversed)
            if (type != EDCFileType.EDC16)
            {
                data = reverseEndian(data);
            }
            if (address > 0 && address < m_currentfilelength)
            {
                try
                {
                    byte[] beforedata = readdatafromfile(filename, address, length, type);

                    FileStream fsi1 = File.OpenWrite(filename);
                    BinaryWriter bw1 = new BinaryWriter(fsi1);
                    fsi1.Position = address;
                    for (int i = 0; i < length; i++)
                    {
                        bw1.Write((byte)data.GetValue(i));
                    }
                    fsi1.Flush();
                    bw1.Close();
                    fsi1.Close();
                    fsi1.Dispose();

                    if (m_ProjectTransactionLog != null && DoTransActionEntry)
                    {
                        // depends on filetype (EDC16 is not reversed)
                        if (type != EDCFileType.EDC16)
                        {
                            data = reverseEndian(data);
                        }
                        TransactionEntry tentry = new TransactionEntry(DateTime.Now, address, length, beforedata, data, 0, 0, note);
                        m_ProjectTransactionLog.AddToTransactionLog(tentry);
                        if (m_CurrentWorkingProject != string.Empty)
                        {
                            m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionExecuted, GetSymbolNameByAddress(address) + " " + note);
                        }
                    }

                }
                catch (Exception E)
                {
                    frmInfoBox info = new frmInfoBox("Failed to write to binary. Is it read-only? Details: " + E.Message);
                }
            }
        }

        public int findSequence(byte[] fileData, int offset, byte[] sequence, byte[] mask)
        {

            byte data;
            int i, max;
            i = 0;
            max = 0;
            int position = offset;
            while (position < fileData.Length)
            {
                data = (byte)fileData[position++];
                if (data == sequence[i] || mask[i] == 0)
                {
                    i++;


                }
                else
                {
                    if (i > max) max = i;
                    position -= i;
                    i = 0;
                }
                if (i == sequence.Length) break;
            }
            if (i == sequence.Length)
            {
                return ((int)position - sequence.Length);
            }
            else
            {
                return -1;
            }
        }



        public ChecksumResultDetails UpdateChecksum(string filename, bool verifyOnly)
        {
            byte[] allBytes;
            ChecksumResult res = new ChecksumResult(); ;
            ChecksumResultDetails result = new ChecksumResultDetails();
            
            EDCFileType fileType = DetermineFileType(filename, false);
            switch (fileType)
            {
                case EDCFileType.EDC15P:
                case EDCFileType.EDC15P6:
                    allBytes = File.ReadAllBytes(filename);
                    res = CalculateEDC15PChecksum(filename, allBytes, verifyOnly, out result);
                    break;
                case EDCFileType.EDC15V:
                    // EDC15VM+ is similar to EDC15P
                    allBytes = File.ReadAllBytes(filename);
                    res = CalculateEDC15VMChecksum(filename, allBytes, verifyOnly, out result);
                    break;
                case EDCFileType.EDC15C:
                    //TODO: Implement EDC15C checksum routine here
                    break;
                case EDCFileType.EDC15M:
                    //TODO: Implement EDC15M checksum routine here
                    break;
                case EDCFileType.EDC16:
                    //TODO: Implement EDC16x checksum routine here
                    break;
                case EDCFileType.EDC17:
                    //TODO: Implement EDC17x checksum routine here
                    break;
                case EDCFileType.MSA15:
                case EDCFileType.MSA12:
                case EDCFileType.MSA11:
                    //TODO: Implement MSA15 checksum routine here

                    // this should be Bosch TDI V3.1 (Version 2.04)
                    
                   /* result.TypeResult = ChecksumType.VAG_EDC15P_V41;
                    allBytes = File.ReadAllBytes(filename);
                    //allBytes = reverseEndian(allBytes);
                    MSA15_checksum msa15chks = new MSA15_checksum();

                    res = msa15chks.tdi41_checksum_search(allBytes, (uint)allBytes.Length, false);
                    result.NumberChecksumsTotal = msa15chks.ChecksumsFound;
                    result.NumberChecksumsFail = msa15chks.ChecksumsIncorrect;
                    result.NumberChecksumsOk = msa15chks.ChecksumsFound - msa15chks.ChecksumsIncorrect;
                    if (res == ChecksumResult.ChecksumOK)
                    {
                        //Console.WriteLine("Checksum matched");
                        result.CalculationOk = true;
                    }
                    else if (res == ChecksumResult.ChecksumFail)
                    {
                        Console.WriteLine("UpdateChecksum: Checksum failed " + filename);
                        if (!verifyOnly)
                        {
                            File.WriteAllBytes(filename, allBytes);
                            result.CalculationOk = true;
                            Console.WriteLine("UpdateChecksum: Checksum fixed");
                        }

                    }*/
                    break;
                case EDCFileType.MSA6:
                    //TODO: Implement MSA6 checksum routine here
                    break;
            }
            if (result.CalculationOk) result.CalculationResult = ChecksumResult.ChecksumOK;
            return result;
        }

        private ChecksumResult CalculateEDC15PChecksum(string filename, byte[] allBytes, bool verifyOnly, out ChecksumResultDetails result)
        {
            ChecksumResult res = new ChecksumResult();
            // checksum for EDC15P is implemented
            result = new ChecksumResultDetails();

            result.CalculationResult = ChecksumResult.ChecksumFail; // default = failed
            result.TypeResult = ChecksumType.VAG_EDC15P_V41;

           
            if (allBytes.Length != 0x80000) return res;

            if (allBytes[0x50008] == 'V' && allBytes[0x50009] == '4' && allBytes[0x5000A] == '.' && allBytes[0x5000B] == '1')
            {
                // checksum V4.1 rev.1 
                result.TypeResult = ChecksumType.VAG_EDC15P_V41;
            }
            else if (allBytes[0x58008] == 'V' && allBytes[0x58009] == '4' && allBytes[0x5800A] == '.' && allBytes[0x5800B] == '1')
            {
                // checksum V4.1 rev.2 
                result.TypeResult = ChecksumType.VAG_EDC15P_V41V2;
            }

            //allBytes = reverseEndian(allBytes);
            EDC15P_checksum chks = new EDC15P_checksum();
            if (result.TypeResult == ChecksumType.VAG_EDC15P_V41)
            {
                res = chks.tdi41_checksum_search(allBytes, (uint)allBytes.Length, false);
            }
            else
            {
                res = chks.tdi41v2_checksum_search(allBytes, (uint)allBytes.Length, false);
            }
            result.NumberChecksumsTotal = chks.ChecksumsFound;
            result.NumberChecksumsFail = chks.ChecksumsIncorrect;
            result.NumberChecksumsOk = chks.ChecksumsFound - chks.ChecksumsIncorrect;
            if (res == ChecksumResult.ChecksumOK)
            {
                Console.WriteLine("Checksum V4.1 matched");
                result.CalculationOk = true;
            }
            else if (res == ChecksumResult.ChecksumFail)
            {
                Console.WriteLine("UpdateChecksum: Checksum failed " + filename);
                if (!verifyOnly)
                {
                    File.WriteAllBytes(filename, allBytes);
                    result.CalculationOk = true;
                    Console.WriteLine("UpdateChecksum: Checksum fixed");
                }

            }
            else if (res == ChecksumResult.ChecksumTypeError)
            {
                result.TypeResult = ChecksumType.VAG_EDC15P_V41_2002;
                EDC15P_checksum chks2002 = new EDC15P_checksum();
                allBytes = File.ReadAllBytes(filename);

                //chks2002.DumpChecksumLocations("V41 2002", allBytes); // for debug info only

                ChecksumResult res2002 = chks2002.tdi41_2002_checksum_search(allBytes, (uint)allBytes.Length, false);
                result.NumberChecksumsTotal = chks2002.ChecksumsFound;
                result.NumberChecksumsFail = chks2002.ChecksumsIncorrect;
                result.NumberChecksumsOk = chks2002.ChecksumsFound - chks2002.ChecksumsIncorrect;

                if (res2002 == ChecksumResult.ChecksumOK)
                {
                    Console.WriteLine("Checksum 2002 matched " + filename);
                    result.CalculationOk = true;
                }
                else if (res2002 == ChecksumResult.ChecksumFail)
                {
                    Console.WriteLine("UpdateChecksum: Checksum 2002 failed " + filename);
                    if (!verifyOnly)
                    {
                        File.WriteAllBytes(filename, allBytes);
                        result.CalculationOk = true;
                        Console.WriteLine("UpdateChecksum: Checksum fixed");
                    }
                }
                else if (res2002 == ChecksumResult.ChecksumTypeError)
                {
                    // unknown checksum type
                    result.CalculationOk = false;
                    result.CalculationResult = ChecksumResult.ChecksumTypeError;
                    result.TypeResult = ChecksumType.Unknown;
                }
            }
            return res;
        }

        private ChecksumResult CalculateEDC15VMChecksum(string filename, byte[] allBytes, bool verifyOnly, out ChecksumResultDetails result)
        {
            ChecksumResult res = new ChecksumResult();
            result = new ChecksumResultDetails();
            result.CalculationResult = ChecksumResult.ChecksumFail; // default = failed
            result.TypeResult = ChecksumType.VAG_EDC15VM_V41;
            if (/*allBytes.Length != 0x40000 && */allBytes.Length != 0x80000 && allBytes.Length != 0x100000) return res;
            if (allBytes.Length >= 0x80000)
            {
                if (allBytes[0x50008] == 'V' && allBytes[0x50009] == '4' && allBytes[0x5000A] == '.' && allBytes[0x5000B] == '1')
                {
                    // checksum V4.1 rev.1 
                    result.TypeResult = ChecksumType.VAG_EDC15VM_V41;
                }
                else if (allBytes[0x58008] == 'V' && allBytes[0x58009] == '4' && allBytes[0x5800A] == '.' && allBytes[0x5800B] == '1')
                {
                    // checksum V4.1 rev.2 
                    result.TypeResult = ChecksumType.VAG_EDC15VM_V41V2;
                }
            }
            //allBytes = reverseEndian(allBytes);
            EDC15VM_checksum chks = new EDC15VM_checksum();
            if (result.TypeResult == ChecksumType.VAG_EDC15VM_V41)
            {
                res = chks.tdi41_checksum_search(allBytes, (uint)allBytes.Length, false);
            }
            else
            {
                res = chks.tdi41v2_checksum_search(allBytes, (uint)allBytes.Length, false);
            }
            result.NumberChecksumsTotal = chks.ChecksumsFound;
            result.NumberChecksumsFail = chks.ChecksumsIncorrect;
            result.NumberChecksumsOk = chks.ChecksumsFound - chks.ChecksumsIncorrect;
            if (res == ChecksumResult.ChecksumOK)
            {
                Console.WriteLine("Checksum V4.1 matched");
                result.CalculationOk = true;
            }
            else if (res == ChecksumResult.ChecksumFail)
            {
                Console.WriteLine("UpdateChecksum: Checksum failed " + filename);
                if (!verifyOnly)
                {
                    File.WriteAllBytes(filename, allBytes);
                    result.CalculationOk = true;
                    Console.WriteLine("UpdateChecksum: Checksum fixed");
                }

            }
            else if (res == ChecksumResult.ChecksumTypeError)
            {
                result.TypeResult = ChecksumType.VAG_EDC15VM_V41_2002;
                EDC15VM_checksum chks2002 = new EDC15VM_checksum();
                allBytes = File.ReadAllBytes(filename);
                ChecksumResult res2002 = chks2002.tdi41_2002_checksum_search(allBytes, (uint)allBytes.Length, false);
                result.NumberChecksumsTotal = chks2002.ChecksumsFound;
                result.NumberChecksumsFail = chks2002.ChecksumsIncorrect;
                result.NumberChecksumsOk = chks2002.ChecksumsFound - chks2002.ChecksumsIncorrect;

                if (res2002 == ChecksumResult.ChecksumOK)
                {
                    Console.WriteLine("Checksum 2002 matched " + filename);
                    result.CalculationOk = true;
                }
                else if (res2002 == ChecksumResult.ChecksumFail)
                {
                    Console.WriteLine("UpdateChecksum: Checksum 2002 failed " + filename);
                    if (!verifyOnly)
                    {
                        File.WriteAllBytes(filename, allBytes);
                        result.CalculationOk = true;
                        Console.WriteLine("UpdateChecksum: Checksum fixed");
                    }
                }
                else if (res2002 == ChecksumResult.ChecksumTypeError)
                {
                    // unknown checksum type
                    result.CalculationOk = false;
                    result.CalculationResult = ChecksumResult.ChecksumTypeError;
                    result.TypeResult = ChecksumType.Unknown;
                }
            }
            return res;
        }


        public void savedatatobinary(int address, int length, byte[] data, string filename, bool DoTransActionEntry, EDCFileType type)
        {
            // depends on filetype (EDC16 is not reversed)
            if (type != EDCFileType.EDC16)
            {
                data = reverseEndian(data);
            }
            if (address > 0 && address < Tools.Instance.m_currentfilelength)
            {
                try
                {
                    byte[] beforedata = readdatafromfile(filename, address, length, type);
                    FileStream fsi1 = File.OpenWrite(filename);
                    BinaryWriter bw1 = new BinaryWriter(fsi1);
                    fsi1.Position = address;
                    for (int i = 0; i < length; i++)
                    {
                        bw1.Write((byte)data.GetValue(i));
                    }
                    fsi1.Flush();
                    bw1.Close();
                    fsi1.Close();
                    fsi1.Dispose();
                }
                catch (Exception E)
                {
                   // MessageBox.Show("Failed to write to binary. Is it read-only? Details: " + E.Message);
                }
            }
        }

        public int[] readdatafromfileasint(string filename, int address, int length, EDCFileType type)
        {
            int[] retval = new int[length];
            FileStream fsi1 = File.OpenRead(filename);
            while (address > fsi1.Length) address -= (int)fsi1.Length;
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = address;
            string temp = string.Empty;

            for (int i = 0; i < length; i++)
            {
                if (type != EDCFileType.EDC16)
                {
                    int iVal = Convert.ToInt32(br1.ReadByte());
                    iVal += Convert.ToInt32(br1.ReadByte()) * 256;
                    retval.SetValue(iVal, i);
                }
                else
                {
                    int iVal = Convert.ToInt32(br1.ReadByte()) * 256;
                    iVal += Convert.ToInt32(br1.ReadByte());
                    retval.SetValue(iVal, i);
                }
            }
            // little endian, reverse bytes
            //retval = reverseEndian(retval);
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();
            return retval;
        }

        public byte[] reverseEndian(byte[] retval)
        {
            byte[] ret = new byte[retval.Length];

            try
            {
                if (retval.Length > 0 && retval.Length % 2 == 0)
                {
                    for (int i = 0; i < retval.Length; i += 2)
                    {
                        byte b1 = retval[i];
                        byte b2 = retval[i + 1];
                        ret[i] = b2;
                        ret[i + 1] = b1;
                    }
                }
            }
            catch (Exception E)
            {

            }
            return ret;
        }

        public int[] reverseEndian(int[] retval)
        {
            int[] ret = new int[retval.Length];

            try
            {
                if (retval.Length > 0 && retval.Length % 2 == 0)
                {
                    for (int i = 0; i < retval.Length; i += 2)
                    {
                        int b1 = retval[i];
                        int b2 = retval[i + 1];
                        ret[i] = b2;
                        ret[i + 1] = b1;
                    }
                }
            }
            catch (Exception E)
            {

            }
            return ret;
        }

        public byte[] readdatafromfile(string filename, int address, int length, EDCFileType type)
        {
            byte[] retval = new byte[length];
            try
            {
                FileStream fsi1 = File.OpenRead(filename);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryReader br1 = new BinaryReader(fsi1);
                fsi1.Position = address;
                string temp = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    retval.SetValue(br1.ReadByte(), i);
                }
                // depends on filetype (EDC16 is not reversed)
                if (type != EDCFileType.EDC16)
                {
                    retval = reverseEndian(retval);
                }
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }
    }
}
