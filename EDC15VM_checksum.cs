//---------------------------------------------------------------------------
//
// Bosch VAG TDI v4.1 - 2002 Checksum Algorithm Version 1.0
// Bosch VAG TDI v4.1 Checksum Algorithm Version 1.1
//
// Copyright 2007-2012 MTX Electronics
// Web: www.mtx-electronics.com
// Email: info@mtx-electronics.com
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    public class EDC15VM_checksum
    {

        public void DumpChecksumLocations(string info, byte[] allBytes)
        {
            //0x7BFFA & 0x7BFFB
            byte b1 = allBytes[0x7BFFA];
            byte b2 = allBytes[0x7BFFB];
            int checksumAddress = 0;
            bool found = true;
            while (found)
            {
                found = false;
                checksumAddress = Tools.Instance.findSequence(allBytes, checksumAddress, new byte[2] { b1, b2 }, new byte[2] { 1, 1 });
                if (checksumAddress > 0)
                {
                    if ((checksumAddress & 0x000F) == 0x000A)
                    {
                        int realAddress = checksumAddress + 2;
                        Console.WriteLine(info + " detected checksumAddress: " + realAddress.ToString("X8"));
                    }
                    checksumAddress += 2;
                    found = true;
                }
            }
        }

        private UInt16 chk_match = 0;

        public UInt16 ChecksumsMatch
        {
            get { return chk_match; }
            set { chk_match = value; }
        }


        private UInt16 chk_found = 0;

        public UInt16 ChecksumsFound
        {
            get { return chk_found; }
            set { chk_found = value; }
        }
        private UInt16 chk_fixed = 0;

        public UInt16 ChecksumsIncorrect
        {
            get { return chk_fixed; }
            set { chk_fixed = value; }
        }

        // there are three more checksums, not sure what they are though:
        //checksum ID: 99C9
        //0x88000
        //0x90000
        //0xFEC00

        private void DumpChecksum(string remark, byte[] allBytes, bool debugInfo)
        {
            if(debugInfo) Console.WriteLine(remark);//+ " " + allBytes[0x5BFFA].ToString("X2") + " " + allBytes[0x5BFFB].ToString("X2") + " " + allBytes[0x5BFFC].ToString("X2") + " " + allBytes[0x5BFFD].ToString("X2") + " " + allBytes[0x5BFFE].ToString("X2") + " " + allBytes[0x5BFFF].ToString("X2"));
        }
        public ChecksumResult tdi41_checksum_search(byte[] file_buffer, UInt32 file_size, bool debug)
        {
            if (file_buffer.Length == 0x40000) return tdi41_checksum_search_256kb(file_buffer, file_size, debug);
            else if (file_buffer.Length == 0x80000) return tdi41_checksum_search_512kb(file_buffer, file_size, debug);
            else return tdi41_checksum_search_1024kb(file_buffer, file_size, debug);
            
        }
        //256kB file EDC15V
        //0x000B80
        //0x008000
        //0x014000
        //0x038B80
        //0x040000
        private ChecksumResult tdi41_checksum_search_256kb(byte[] file_buffer, UInt32 file_size, bool debug)
        {
            bool first_pass = true;
            UInt32 chk_oldvalue, chk_value, chk_start_addr, chk_end_addr;
            UInt32[] chk_array = new UInt32[5] { 0x000B80, 0x008000, 0x014000, 0x038B80, 0x040000 };
            UInt16 seed_a = 0, seed_b = 0;

            chk_found = 0;
            chk_fixed = 0;
            chk_match = 0;

            for (; chk_found < chk_array.Length - 1; chk_found++)
            {
                chk_start_addr = chk_array[chk_found];
                chk_end_addr = chk_array[chk_found + 1];
                //DumpChecksum("chks " + chk_start_addr.ToString("X8") + "-" + chk_end_addr.ToString("X8"), file_buffer, debug);
                if (!first_pass)
                {
                    seed_a |= 0x8631;
                    seed_b |= 0xEFCD;
                }

                //if (CheckEmpty(file_buffer, chk_start_addr, chk_end_addr)) continue;

                chk_oldvalue = ((UInt32)file_buffer[chk_end_addr - 1] << 24)
                             + ((UInt32)file_buffer[chk_end_addr - 2] << 16)
                             + ((UInt32)file_buffer[chk_end_addr - 3] << 8)
                             + (UInt32)file_buffer[chk_end_addr - 4];

                chk_value = tdi41_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr - 4, seed_a, seed_b);

                if (chk_oldvalue != chk_value && chk_oldvalue != 0xC3C3C3C3)
                {
                    file_buffer[chk_end_addr - 4] = Convert.ToByte(chk_value & 0x000000ff);
                    file_buffer[chk_end_addr - 3] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                    file_buffer[chk_end_addr - 2] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                    file_buffer[chk_end_addr - 1] = Convert.ToByte((chk_value >> 24) & 0x000000ff);
                    Console.WriteLine("Checksum at address " + chk_end_addr.ToString("X8") + " failed");
                    chk_fixed++;
                }
                else if (chk_oldvalue == chk_value) chk_match++;

                first_pass = false;
            }
            Console.WriteLine("edc15v41_chkfixed: " + chk_fixed.ToString() + " / " + chk_found.ToString());
            Console.WriteLine("edc15v41_chkmatch: " + chk_match.ToString() + " / " + chk_found.ToString());
            if (chk_fixed == 0) return ChecksumResult.ChecksumOK;
            else if (chk_match > 3) return ChecksumResult.ChecksumFail;
            else if (chk_fixed >= 6) return ChecksumResult.ChecksumTypeError;
            return ChecksumResult.ChecksumFail;
        }

        private ChecksumResult tdi41_checksum_search_1024kb(byte[] file_buffer, UInt32 file_size, bool debug)
        {
            bool first_pass = false;
            UInt32 chk_oldvalue, chk_value, chk_start_addr, chk_end_addr;
            UInt32[] chk_array = new UInt32[34] { 
                0x4000, 
                0x4B80, // << IGNORE
                0x10000,
                0x14000,
                0x14B80,
                0x20000,
                0x24000,
                0x24B80,
                0x30000,
                0x34000,//10
                0x34B80,
                0x40000,
                0x44000,
                0x44B80,
                0x50000,
                0x54000,
                0x54B80,
                0x60000,
                0x64000,
                0x64B80,//20
                0x70000,
                0x74000,
                0x74B80,
                0x94000, //<< IGNORE
                0xCC000,
                0xD0000,
                0xD0B80,
                0xDC000,
                0xE0000,
                0xE0B80,
                0xEC000,
                0xF0000,
                0xF0B80,
                0xFC000
                };
            UInt16 seed_a = 0, seed_b = 0;

            chk_found = 0;
            chk_fixed = 0;
            chk_match = 0;

            for (; chk_found < chk_array.Length - 1; chk_found++)
            {
                chk_start_addr = chk_array[chk_found];
                chk_end_addr = chk_array[chk_found + 1];
                //DumpChecksum("chks " + chk_start_addr.ToString("X8") + "-" + chk_end_addr.ToString("X8"), file_buffer, debug);
                if (!first_pass)
                {
                    seed_a |= 0x8631;
                    seed_b |= 0xEFCD;
                }

                //if (CheckEmpty(file_buffer, chk_start_addr, chk_end_addr)) continue;

                chk_oldvalue = ((UInt32)file_buffer[chk_end_addr - 1] << 24)
                             + ((UInt32)file_buffer[chk_end_addr - 2] << 16)
                             + ((UInt32)file_buffer[chk_end_addr - 3] << 8)
                             + (UInt32)file_buffer[chk_end_addr - 4];

                chk_value = tdi41_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr - 4, seed_a, seed_b);

                if (chk_oldvalue != chk_value && chk_oldvalue != 0xC3C3C3C3 && chk_end_addr != 0x94000)
                {
                    file_buffer[chk_end_addr - 4] = Convert.ToByte(chk_value & 0x000000ff);
                    file_buffer[chk_end_addr - 3] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                    file_buffer[chk_end_addr - 2] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                    file_buffer[chk_end_addr - 1] = Convert.ToByte((chk_value >> 24) & 0x000000ff);
                    Console.WriteLine("Checksum at address " + chk_end_addr.ToString("X8") + " failed");
                    chk_fixed++;
                }
                else if (chk_oldvalue == chk_value) chk_match++;

                first_pass = false;
            }

            Console.WriteLine("edc15v41_1024kB_chkfixed: " + chk_fixed.ToString() + " / " + chk_found.ToString());
            Console.WriteLine("edc15v41_1024kB_chkmatch: " + chk_match.ToString() + " / " + chk_found.ToString());
            if (chk_fixed == 0) return ChecksumResult.ChecksumOK;
            else if (chk_match > 3) return ChecksumResult.ChecksumFail;
            else if (chk_fixed >= 6) return ChecksumResult.ChecksumTypeError;
            return ChecksumResult.ChecksumFail;
        }

        private ChecksumResult tdi41_checksum_search_512kb(byte[] file_buffer, UInt32 file_size, bool debug)
        {
            bool first_pass = true;
            UInt32 chk_oldvalue, chk_value, chk_start_addr, chk_end_addr;
            UInt32[] chk_array = new UInt32[12] { 0x10000, 0x14000, 0x4C000, 0x50000, 0x50B80, 0x5C000, 0x60000, 0x60B80, 0x6C000, 0x70000, 0x70B80, 0x7C000 };
            UInt16 seed_a = 0, seed_b = 0;

            chk_found = 0;
            chk_fixed = 0;
            chk_match = 0;

            for (; chk_found < chk_array.Length - 1; chk_found++)
            {
                chk_start_addr = chk_array[chk_found];
                chk_end_addr = chk_array[chk_found + 1];
                //DumpChecksum("chks " + chk_start_addr.ToString("X8") + "-" + chk_end_addr.ToString("X8"), file_buffer, debug);
                if (!first_pass)
                {
                    seed_a |= 0x8631;
                    seed_b |= 0xEFCD;
                }

                //if (CheckEmpty(file_buffer, chk_start_addr, chk_end_addr)) continue;

                chk_oldvalue = ((UInt32)file_buffer[chk_end_addr - 1] << 24)
                             + ((UInt32)file_buffer[chk_end_addr - 2] << 16)
                             + ((UInt32)file_buffer[chk_end_addr - 3] << 8)
                             + (UInt32)file_buffer[chk_end_addr - 4];

                chk_value = tdi41_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr - 4, seed_a, seed_b);

                if (chk_oldvalue != chk_value && chk_oldvalue != 0xC3C3C3C3)
                {
                    file_buffer[chk_end_addr - 4] = Convert.ToByte(chk_value & 0x000000ff);
                    file_buffer[chk_end_addr - 3] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                    file_buffer[chk_end_addr - 2] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                    file_buffer[chk_end_addr - 1] = Convert.ToByte((chk_value >> 24) & 0x000000ff);
                    Console.WriteLine("Checksum at address " + chk_end_addr.ToString("X8") + " failed");
                    chk_fixed++;
                }
                else if (chk_oldvalue == chk_value) chk_match++;
                first_pass = false;
            }

            Console.WriteLine("edc15v41_512kB_chkfixed: " + chk_fixed.ToString() + " / " + chk_found.ToString());
            Console.WriteLine("edc15v41_512kB_chkmatch: " + chk_match.ToString() + " / " + chk_found.ToString());
            if (chk_fixed == 0) return ChecksumResult.ChecksumOK;
            else if (chk_match > 3) return ChecksumResult.ChecksumFail;
            else if (chk_fixed >= 6) return ChecksumResult.ChecksumTypeError;
            return ChecksumResult.ChecksumFail;
        }

        public ChecksumResult tdi41v2_checksum_search(byte[] file_buffer, UInt32 file_size, bool debug)
        {
            bool first_pass = true;
            UInt32 chk_oldvalue, chk_value, chk_start_addr, chk_end_addr;
            //UInt32[] chk_array = new UInt32[8] { 0x10000, 0x14000, 0x4C000, 0x50000, 0x50B80, 0x58000, 0x58B80, 0x5C000, 0x60000, 0x64000, 0x68000, 0x70000, 0x70B80, 0x7C000 };
            UInt32[] chk_array = new UInt32[8] { 0x10000, 0x14000, 0x58000, 0x58B80, 0x64000, 0x70000, 0x70B80, 0x7C000 };
            UInt16 seed_a = 0, seed_b = 0;

            chk_found = 0;
            chk_fixed = 0;
            chk_match = 0;

            for (; chk_found < chk_array.Length - 1; chk_found++)
            {
                chk_start_addr = chk_array[chk_found];
                chk_end_addr = chk_array[chk_found + 1];
                // if array is empty, don't check
                //DumpChecksum("chks " + chk_start_addr.ToString("X8") + "-" + chk_end_addr.ToString("X8"), file_buffer, debug);

                if (!first_pass)
                {
                    seed_a |= 0x8631;
                    seed_b |= 0xEFCD;
                }


                if (CheckEmpty(file_buffer, chk_start_addr, chk_end_addr)) continue;

                chk_oldvalue = ((UInt32)file_buffer[chk_end_addr - 1] << 24)
                             + ((UInt32)file_buffer[chk_end_addr - 2] << 16)
                             + ((UInt32)file_buffer[chk_end_addr - 3] << 8)
                             + (UInt32)file_buffer[chk_end_addr - 4];

                chk_value = tdi41_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr - 4, seed_a, seed_b);

                if (chk_oldvalue != chk_value && chk_oldvalue != 0xC3C3C3C3)
                {
                    file_buffer[chk_end_addr - 4] = Convert.ToByte(chk_value & 0x000000ff);
                    file_buffer[chk_end_addr - 3] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                    file_buffer[chk_end_addr - 2] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                    file_buffer[chk_end_addr - 1] = Convert.ToByte((chk_value >> 24) & 0x000000ff);
                    Console.WriteLine("Checksum at address " + chk_end_addr.ToString("X8") + " failed");
                    chk_fixed++;
                }
                else if (chk_oldvalue == chk_value) chk_match++;
                Console.WriteLine("Checking " + chk_start_addr.ToString("X8") + " - " + chk_end_addr.ToString("X8") + " file " + chk_oldvalue.ToString("X8") + " calc " + chk_value.ToString("X8"));
                first_pass = false;
            }

            Console.WriteLine("edc15v41_512kB_chkfixed: " + chk_fixed.ToString() + " / " + chk_found.ToString());
            Console.WriteLine("edc15v41_512kB_chkmatch: " + chk_match.ToString() + " / " + chk_found.ToString());
            if (chk_fixed == 0) return ChecksumResult.ChecksumOK;
            else if (chk_match > 3) return ChecksumResult.ChecksumFail;
            else if (chk_fixed >= 4) return ChecksumResult.ChecksumTypeError;
            return ChecksumResult.ChecksumFail;
        }

        private bool CheckEmpty(byte[] file_buffer, uint chk_start_addr, uint chk_end_addr)
        {
            for (uint i = chk_start_addr; i < chk_end_addr - 4; i++)
            {
                if (file_buffer[i] != 0xC3) return false;
            }
            return true;
        }


        UInt32 tdi41_checksum_calculate(byte[] file_buffer, UInt32 chk_start_addr, UInt32 chk_end_addr, UInt16 seed_a, UInt16 seed_b)
        {
            UInt16 var_1;
            byte var_2;

            do
            {
                var_2 = 0;
                seed_a ^= Convert.ToUInt16((((UInt16)file_buffer[chk_start_addr + 1] << 8) + (UInt16)file_buffer[chk_start_addr]));
                chk_start_addr += 2;

                if ((seed_b & 0xF) > 0)
                {
                    var_1 = Convert.ToUInt16(seed_a >> (16 - (seed_b & 0xF)));
                    seed_a <<= (seed_b & 0xF);
                    seed_a |= var_1;

                    var_2 = Convert.ToByte(seed_a & 1);
                }

                seed_b -= Convert.ToUInt16((((UInt16)file_buffer[chk_start_addr + 1] << 8) + (UInt16)file_buffer[chk_start_addr]));
                seed_b -= var_2;
                chk_start_addr += 2;
                seed_b ^= seed_a;

                if (chk_start_addr == chk_end_addr)
                    break;

                seed_a -= Convert.ToUInt16((((UInt16)file_buffer[chk_start_addr + 1] << 8) + (UInt16)file_buffer[chk_start_addr]));
                chk_start_addr += 2;
                seed_a += 0xDAAC;
                seed_b ^= Convert.ToUInt16((((UInt16)file_buffer[chk_start_addr + 1] << 8) + (UInt16)file_buffer[chk_start_addr]));
                chk_start_addr += 2;

                if ((seed_a & 0xF) > 0)
                {
                    var_1 = Convert.ToUInt16((seed_b << (16 - (seed_a & 0xF))) & 0xffff);
                    seed_b >>= (seed_a & 0xF);
                    seed_b |= var_1;
                }
            }
            while (chk_start_addr != chk_end_addr);

            seed_a -= 0x8631;
            seed_a += 0xDAAC;
            seed_b ^= 0xDF9B;

            return (((UInt32)seed_b << 16) + seed_a);
        }

        public ChecksumResult tdi41_2002_checksum_search(byte[] file_buffer, UInt32 file_size, bool debug)
        {
            UInt32 seed_1, seed_2;
            UInt16 seed_1_msb, seed_1_lsb, seed_2_lsb, seed_2_msb;

            UInt32 chk_oldvalue, chk_value, chk_start_addr, chk_end_addr, chk_store_addr;

            chk_found = 2;
            chk_fixed = 0;
            chk_match = 0;

            // Find seed 1
            seed_1 = tdi41_2002_checksum_calculate(file_buffer, 0x14000, 0x4bffe, 0x8631, 0xefcd, 0, 0, true);
            seed_1_msb = (UInt16)(seed_1 >> 16);
            seed_1_lsb = (UInt16)seed_1;

            // Find seed 2
            seed_2 = tdi41_2002_checksum_calculate(file_buffer, 0, 0x7ffe, 0, 0, 0, 0, true);
            seed_2_msb = (UInt16)(seed_2 >> 16);
            seed_2_lsb = (UInt16)seed_2;

            // checksum 1
            chk_oldvalue = ((UInt32)file_buffer[0xffff] << 24)
                         + ((UInt32)file_buffer[0xfffe] << 16)
                         + ((UInt32)file_buffer[0xfffd] << 8)
                         + (UInt32)file_buffer[0xfffc];

            chk_value = tdi41_2002_checksum_calculate(file_buffer, 0x8000, 0xfffb, seed_2_lsb, seed_2_msb, 0x4531, 0x3550, false);

            if (chk_oldvalue != chk_value)
            {
                file_buffer[0xfffc] = Convert.ToByte(chk_value & 0x000000ff);
                file_buffer[0xfffd] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                file_buffer[0xfffe] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                file_buffer[0xffff] = Convert.ToByte((chk_value >> 24) & 0x000000ff);

                chk_fixed++;
            }
            else chk_match++;

            // Checksum 2
            chk_oldvalue = ((UInt32)file_buffer[0x13fff] << 24)
                         + ((UInt32)file_buffer[0x13ffe] << 16)
                         + ((UInt32)file_buffer[0x13ffd] << 8)
                         + (UInt32)file_buffer[0x13ffc];

            chk_value = tdi41_2002_checksum_calculate(file_buffer, 0x10000, 0x13ffb, 0, 0, 0x8631, 0xefcd, false);

            if (chk_oldvalue != chk_value)
            {
                file_buffer[0x13ffc] = Convert.ToByte(chk_value & 0x000000ff);
                file_buffer[0x13ffd] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                file_buffer[0x13ffe] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                file_buffer[0x13fff] = Convert.ToByte((chk_value >> 24) & 0x000000ff);

                chk_fixed++;
            }
            else chk_match++;

            // Checksum blocks loop
            chk_store_addr = 0x4fffb;
            do
            {
                if ((file_buffer[chk_store_addr + 13] == 0x56) &&
                    (file_buffer[chk_store_addr + 14] == 0x34) &&
                    (file_buffer[chk_store_addr + 15] == 0x2e) &&
                    (file_buffer[chk_store_addr + 16] == 0x31))
                {
                    // Checksum
                    chk_start_addr = chk_store_addr - 0x3ffb;
                    chk_end_addr = chk_store_addr;

                    chk_oldvalue = ((UInt32)file_buffer[chk_store_addr + 4] << 24)
                                 + ((UInt32)file_buffer[chk_store_addr + 3] << 16)
                                 + ((UInt32)file_buffer[chk_store_addr + 2] << 8)
                                 + (UInt32)file_buffer[chk_store_addr + 1];

                    chk_value = tdi41_2002_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr, seed_1_lsb, seed_1_msb, seed_1_lsb, seed_1_msb, false);

                    if (chk_oldvalue != chk_value)
                    {
                        file_buffer[chk_store_addr + 1] = Convert.ToByte(chk_value & 0x000000ff);
                        file_buffer[chk_store_addr + 2] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                        file_buffer[chk_store_addr + 3] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                        file_buffer[chk_store_addr + 4] = Convert.ToByte((chk_value >> 24) & 0x000000ff);

                        chk_fixed++;
                    }
                    else chk_match++;

                    Console.WriteLine("2002 start " + chk_start_addr.ToString("X8") + " - " + chk_end_addr.ToString("X8") + " store " + chk_store_addr.ToString("X8") + " file " + chk_oldvalue.ToString("X8") + " calc " + chk_value.ToString("X8"));

                    // Checksum
                    chk_start_addr = chk_store_addr + 5;
                    chk_end_addr = chk_store_addr + 0xb80;
                   

                    chk_oldvalue = ((UInt32)file_buffer[chk_store_addr + 2948] << 24)
                                 + ((UInt32)file_buffer[chk_store_addr + 2947] << 16)
                                 + ((UInt32)file_buffer[chk_store_addr + 2946] << 8)
                                 + (UInt32)file_buffer[chk_store_addr + 2945];

                    chk_value = tdi41_2002_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr, seed_1_lsb, seed_1_msb, seed_1_lsb, seed_1_msb, false);

                    if (chk_oldvalue != chk_value)
                    {
                        file_buffer[chk_store_addr + 2945] = Convert.ToByte(chk_value & 0x000000ff);
                        file_buffer[chk_store_addr + 2946] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                        file_buffer[chk_store_addr + 2947] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                        file_buffer[chk_store_addr + 2948] = Convert.ToByte((chk_value >> 24) & 0x000000ff);

                        chk_fixed++;
                    }
                    else chk_match++;

                    Console.WriteLine("2002 start " + chk_start_addr.ToString("X8") + " - " + chk_end_addr.ToString("X8") + " store " + chk_store_addr.ToString("X8") + " file " + chk_oldvalue.ToString("X8") + " calc " + chk_value.ToString("X8"));

                    // Checksum
                    chk_start_addr = chk_store_addr + 0xb85;
                    chk_end_addr = chk_store_addr + 0xc000;

                    chk_oldvalue = ((UInt32)file_buffer[chk_store_addr + 49156] << 24)
                                 + ((UInt32)file_buffer[chk_store_addr + 49155] << 16)
                                 + ((UInt32)file_buffer[chk_store_addr + 49154] << 8)
                                 + (UInt32)file_buffer[chk_store_addr + 49153];

                    chk_value = tdi41_2002_checksum_calculate(file_buffer, chk_start_addr, chk_end_addr, seed_1_lsb, seed_1_msb, seed_1_lsb, seed_1_msb, false);

                    if (chk_oldvalue != chk_value)
                    {
                        file_buffer[chk_store_addr + 49153] = Convert.ToByte(chk_value & 0x000000ff);
                        file_buffer[chk_store_addr + 49154] = Convert.ToByte((chk_value >> 8) & 0x000000ff);
                        file_buffer[chk_store_addr + 49155] = Convert.ToByte((chk_value >> 16) & 0x000000ff);
                        file_buffer[chk_store_addr + 49156] = Convert.ToByte((chk_value >> 24) & 0x000000ff);

                        chk_fixed++;
                    }
                    else chk_match++;

                    Console.WriteLine("2002 start " + chk_start_addr.ToString("X8") + " - " + chk_end_addr.ToString("X8") + " store " + chk_store_addr.ToString("X8") + " file " + chk_oldvalue.ToString("X8") + " calc " + chk_value.ToString("X8"));


                    chk_found += 3;
                }

                chk_store_addr += 0x10000;
            } while (chk_store_addr + 5 < file_size);


            Console.WriteLine("edc15v41_2002_chkfixed: " + chk_fixed.ToString() + " / " + chk_found.ToString());
            Console.WriteLine("edc15v41_2002_chkfixed: " + chk_match.ToString() + " / " + chk_found.ToString());

            if (chk_fixed == 0) return ChecksumResult.ChecksumOK;
            else if (chk_match > 3) return ChecksumResult.ChecksumFail;
            else if (chk_fixed >= chk_found - 1) return ChecksumResult.ChecksumTypeError;
            return ChecksumResult.ChecksumFail;
        }



        UInt32 tdi41_2002_checksum_calculate(byte[] file_buffer, UInt32 chk_start_addr, UInt32 chk_end_addr, UInt16 seed_a, UInt16 seed_b, UInt16 seed_c, UInt16 seed_d, bool first_pass)
        {
            UInt32 count = chk_start_addr / 2;
            UInt32 end_count = chk_end_addr / 2;
            UInt32 buffer_addr = chk_start_addr;
            UInt32 checksum, var_6, var_7 = 0;

            UInt16 var_1 = 0, var_2 = 0, var_3, var_4, var_5;

            if (count != end_count)
            {
                var_1 = seed_a;
                var_2 = seed_b;

                if (chk_start_addr == 0x8000)
                {
                    var_1 = Convert.ToUInt16(var_1 ^ 0xD565);
                    var_2 += 0x308a;
                }

                do
                {
                    var_1 ^= Convert.ToUInt16(((UInt16)file_buffer[buffer_addr + 1] << 8) + (UInt16)file_buffer[buffer_addr]);
                    var_3 = Convert.ToUInt16(var_2 & 0xf);
                    ++count;
                    buffer_addr += 2;
                    var_4 = 0;

                    if ((var_2 & 0xf) > 0)
                    {
                        do
                        {
                            var_4 = (UInt16)(var_1 >> 15);
                            var_1 = (UInt16)(((var_1 * 2) + var_4));

                            --var_3;
                        } while (var_3 > 0);
                    }

                    var_2 -= (UInt16)((var_4 + ((UInt16)file_buffer[buffer_addr + 1] << 8) + (UInt16)file_buffer[buffer_addr]));
                    var_2 = (UInt16)(var_1 ^ var_2);

                    buffer_addr += 2;
                    ++count;

                    if (count > end_count)
                        break;

                    var_5 = (UInt16)(((UInt16)file_buffer[buffer_addr + 1] << 8) + (UInt16)file_buffer[buffer_addr]);
                    buffer_addr += 4;
                    var_1 += (UInt16)((0xffff - var_5 + 0xdaad));
                    var_6 = (UInt32)((UInt16)file_buffer[buffer_addr - 1] << 8);
                    var_2 ^= (UInt16)((UInt16)var_6 + (UInt16)file_buffer[buffer_addr - 2]);
                    var_4 = (UInt16)(var_1 & 0xf);
                    count += 2;

                    if ((var_1 & 0xf) > 0)
                    {
                        do
                        {
                            var_6 = (var_6 | 0xffff) & var_2;
                            var_6 <<= 15;
                            var_2 = (UInt16)(((var_2 >> 1) + var_6));

                            --var_4;
                        } while (var_4 > 0);
                    }
                } while (count <= end_count);
            }

            if (chk_start_addr == 0)
            {
                var_1 -= 0x79cf;
                var_2 -= 0x1033;
            }

            if (!first_pass)
            {
                var_5 = seed_d;
                var_1 -= seed_c;
                var_6 = (UInt16)((seed_c | 0xffff) & 0xdaad);
                var_1 += (UInt16)(var_6 - 1);
                var_7 = var_7 & 0xffff;

                for (count = (UInt32)(seed_c & 0xf); count > 0; var_5 = (UInt16)((((UInt32)var_5 >> 15) + var_7)))
                {
                    --count;
                    var_7 = (var_7 | 0xffff) & var_5;
                    var_7 *= 2;
                }

                checksum = (UInt32)(((UInt32)var_1 + (((UInt32)var_5 ^ (UInt32)var_2) << 16)));
            }
            else
            {
                checksum = (UInt32)(((UInt32)var_1 + ((UInt32)var_2 << 16)));
            }

            return (checksum);
        }
    }
}
