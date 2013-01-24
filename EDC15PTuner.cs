using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    class EDC15PTuner
    {
        public bool TuneEDC15PFile(string filename, SymbolCollection symbols, int peakTorque, int peakHP, bool autoUpdateChecksum)
        {
            bool retval = true;
            retval = TuneDriverWish(filename, symbols, peakTorque, peakHP, autoUpdateChecksum);
            if (retval) retval = TuneTorqueLimiter(filename, symbols, peakTorque, peakHP, autoUpdateChecksum);
            if (retval) retval = TuneSmokeLimiter(filename, symbols, peakTorque, peakHP, autoUpdateChecksum);
            if (retval) retval = DisableEGRMap(filename, symbols, autoUpdateChecksum);
            
            //Increase duration limiter?

            //Increase turbo boost request
            //Adjust N75 map?
            //Increase boost limiter(s) (map + SVBL)
            
            return false;
        }

        private bool DisableEGRMap(string filename, SymbolCollection symbols, bool autoUpdateChecksum)
        {
            bool retval = true;
            int egrAddress = (int)Tools.Instance.GetSymbolAddressLike(Tools.Instance.m_symbols, "EGR");
            if (egrAddress > 0)
            {
                foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                {
                    if (sh.Flash_start_address == egrAddress)
                    {
                        byte[] egrdata = Tools.Instance.readdatafromfile(filename, egrAddress, sh.Length, EDCFileType.EDC15P);
                        int newValue = 8500;
                        for (int rows = 0; rows < sh.X_axis_length; rows++)
                        {
                            for (int cols = 0; cols < sh.Y_axis_length; cols++)
                            {
                                byte b1 = (byte)((newValue & 0x00FF00) / 256);
                                byte b2 = (byte)(newValue & 0x0000FF);

                                egrdata[rows * sh.Y_axis_length * 2 + cols * 2] = b1;
                                egrdata[rows * sh.Y_axis_length * 2 + (cols * 2) + 1] = b2;
                            }
                        }
                        SaveAndSyncData(egrdata.Length, (int)sh.Flash_start_address, egrdata, filename, true, "Disabled EGR map", autoUpdateChecksum);
                    }
                }
            }
            else retval = false;
            return retval;
        }

        private bool TuneSmokeLimiter(string filename, SymbolCollection symbols, int peakTorque, int peakHP, bool autoUpdateChecksum)
        {
            bool retval = true;
            int peakIQ = peakTorque/6;
            //Smokelimit is when AFR = 1:17, but should decrease to 1:25 on low airmass?
            // full load would allow 1:15.5 or 1:16
            // 

            // x = rpm
            int slAddress = (int)Tools.Instance.GetSymbolAddressLike(Tools.Instance.m_symbols, "Smoke limiter");

            if (slAddress > 0)
            {
                foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                {
                    if (sh.Flash_start_address == slAddress)
                    {
                        byte[] sldata = Tools.Instance.readdatafromfile(filename, slAddress, sh.Length, EDCFileType.EDC15P);
                        //Console.WriteLine(sh.Varname + " " + sh.Y_axis_length.ToString() + " " + sh.X_axis_length.ToString());

                        int[] rpms = Tools.Instance.readdatafromfileasint(filename, sh.X_axis_address, sh.X_axis_length, EDCFileType.EDC15P);
                        int maxSupportedAirflow = peakIQ * 17;

                        // range from 300 mg - maxSupportedAirflow
                        int airFlowStep = (maxSupportedAirflow - 300) / sh.Y_axis_length;
                        int[] airFlowAxis = new int[sh.Y_axis_length];
                        for (int i = 0; i < airFlowAxis.Length; i++)
                        {
                            airFlowAxis[i] = (300 + (i * airFlowStep)) * 10;
                        }
                        // save axis
                        byte[] barr = new byte[airFlowAxis.Length * 2];
                        int bCount = 0;
                        for (int i = 0; i < airFlowAxis.Length; i++)
                        {
                            int iVal = (int)airFlowAxis.GetValue(i);
                            byte b1 = (byte)((iVal & 0x00FF00) / 256);
                            byte b2 = (byte)(iVal & 0x0000FF);
                            barr[bCount++] = b1;
                            barr[bCount++] = b2;
                        }
                        Tools.Instance.savedatatobinary(sh.Y_axis_address, barr.Length, barr, filename, true, "Tuned smoke limiter axis", EDCFileType.EDC15P);
                        // check for other symbols with the same length and the same END address
                        foreach (SymbolHelper shaxis in Tools.Instance.m_symbols)
                        {
                            if (shaxis.X_axis_address != sh.X_axis_address)
                            {
                                if ((shaxis.X_axis_address & 0x0FFFF) == (sh.X_axis_address & 0x0FFFF))
                                {
                                    if (shaxis.X_axis_length * 2 == barr.Length)
                                    {
                                        Tools.Instance.savedatatobinary(sh.X_axis_address, barr.Length, barr, filename, true, "Tuned smoke limiter axis", EDCFileType.EDC15P);
                                    }
                                }
                            }
                            else if (shaxis.Y_axis_address != sh.Y_axis_address)
                            {
                                if ((shaxis.Y_axis_address & 0x0FFFF) == (sh.Y_axis_address & 0x0FFFF))
                                {
                                    if (shaxis.Y_axis_length * 2 == barr.Length)
                                    {
                                        Tools.Instance.savedatatobinary(shaxis.Y_axis_address, barr.Length, barr, filename, true, "Tuned smoke limiter axis", EDCFileType.EDC15P);
                                    }
                                }
                            }
                        }
                        
                        if(autoUpdateChecksum) Tools.Instance.UpdateChecksum(filename, false);
                        // end of save axis


                        int[] airmasses = Tools.Instance.readdatafromfileasint(filename, sh.Y_axis_address, sh.Y_axis_length, EDCFileType.EDC15P);

                        for (int rows = 0; rows < sh.X_axis_length; rows++)
                        {
                            for (int cols = 0; cols < sh.Y_axis_length; cols++)
                            {
                                // whats the rpm?

                                int rpm = rpms[rows];
                                // what is the airmass
                                int airmass = airmasses[cols];

                                // we should simply limit to 1:17 for all rpm ranges
                                int newIQ = airmass / 170;

                                int targetTorque = peakTorque;
                                int targetPower = Tools.Instance.TorqueToPower(targetTorque, rpm);
                                if (targetPower > peakHP)
                                {
                                    targetTorque = Tools.Instance.PowerToTorque(peakHP, rpm);
                                    targetPower = Tools.Instance.TorqueToPower(targetTorque, rpm);
                                }
                                int targetIQ = Tools.Instance.TorqueToIQ(targetTorque, rpm, 4);

                                if (newIQ > targetIQ) newIQ = targetIQ;

                                //Console.WriteLine("Calculate for rpm: " + rpm.ToString() + " targetT: " + targetTorque.ToString() + " targetP: " + targetPower.ToString() + " targetIQ: " + targetIQ.ToString());

                                newIQ *= 100;
                                byte b1 = (byte)((newIQ & 0x00FF00) / 256);
                                byte b2 = (byte)(newIQ & 0x0000FF);

                                sldata[rows * sh.Y_axis_length * 2 + cols * 2] = b1;
                                sldata[rows * sh.Y_axis_length * 2 + (cols * 2) + 1] = b2;
                            }
                        }
                        SaveAndSyncData(sldata.Length, (int)sh.Flash_start_address, sldata, filename, true, "Tuned smoke limiter", autoUpdateChecksum);

                    }
                }

            }
            else retval = false;
            return retval;
        }

        private bool TuneTorqueLimiter(string filename, SymbolCollection symbols, int peakTorque, int peakHP, bool autoUpdateChecksum)
        {
            bool retval = true;
            //Increase driverwish to peakIQ if it is not already (only the rightmost 2 columns)
            int tlAddress = (int)Tools.Instance.GetSymbolAddressLike(Tools.Instance.m_symbols, "Torque limiter");

            if (tlAddress > 0)
            {
                foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                {
                    if (sh.Flash_start_address == tlAddress)
                    {
                        byte[] tldata = Tools.Instance.readdatafromfile(filename, tlAddress, sh.Length, EDCFileType.EDC15P);
                       // Console.WriteLine(sh.Varname + " " + sh.Y_axis_length.ToString() + " " + sh.X_axis_length.ToString());
                        int[] rpms = Tools.Instance.readdatafromfileasint(filename, sh.Y_axis_address, sh.Y_axis_length, EDCFileType.EDC15P);
                        for (int rows = 0; rows < sh.X_axis_length; rows++)
                        {
                            for (int cols = 0; cols < sh.Y_axis_length; cols++)
                            {
                                // whats the rpm?
                                int rpm = rpms[cols];
                                int targetTorque = peakTorque;
                                int targetPower = Tools.Instance.TorqueToPower(targetTorque, rpm);
                                if (targetPower > peakHP) targetTorque = Tools.Instance.PowerToTorque(peakHP, rpm);
                                int targetIQ = Tools.Instance.TorqueToIQ(targetTorque, rpm, 4);
                                int maxIQ = targetIQ;
                                if (rpm < 550) maxIQ = 0;
                                if (rpm > 5000) maxIQ = 0;
                                if (cols == 0) maxIQ = 0;
                                if (cols == sh.Y_axis_length - 1) maxIQ = 0;
                                if (rpm >= 550 && rpm < 1000) maxIQ = 30;
                                if (rpm >= 1000 && rpm < 1250) maxIQ = 45;
                                if (rpm >= 1250 && rpm < 1500) maxIQ = 55;
                                if (rpm >= 1500 && rpm < 1750) maxIQ = 60;
                                if (rpm >= 1750 && rpm < 1900) maxIQ = 65;
                                if (targetIQ > maxIQ) targetIQ = maxIQ;

                                //Console.WriteLine("Calculate for rpm: " + rpm.ToString() + " targetT: " + targetTorque.ToString() + " targetP: " + targetPower.ToString() + " targetIQ: " + targetIQ.ToString());
                                targetIQ *= 100;
                                byte b1 = (byte)((targetIQ & 0x00FF00) / 256);
                                byte b2 = (byte)(targetIQ & 0x0000FF);

                                tldata[rows * sh.Y_axis_length * 2 + cols * 2] = b1;
                                tldata[rows * sh.Y_axis_length * 2 + (cols * 2) + 1] = b2;
                            }
                        }
                        SaveAndSyncData(tldata.Length, (int)sh.Flash_start_address, tldata, filename, true, "Tuned torque limiter", autoUpdateChecksum);

                    }
                }

            }
            else retval = false;
            return retval;
        }

        private bool TuneDriverWish(string filename, SymbolCollection symbols, int peakTorque, int peakHP, bool autoUpdateChecksum)
        {
            bool retval = true;
            //Increase driverwish to peakIQ if it is not already (only the rightmost 2 columns)
            int dwAddress = (int)Tools.Instance.GetSymbolAddressLike(Tools.Instance.m_symbols, "Driver wish");

            if (dwAddress > 0)
            {
                foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                {
                    if (sh.Flash_start_address == dwAddress)
                    {
                        byte[] dwdata = Tools.Instance.readdatafromfile(filename, dwAddress, sh.Length, EDCFileType.EDC15P);
                        //Console.WriteLine(sh.Varname + " " + sh.Y_axis_length.ToString() + " " + sh.X_axis_length.ToString());
                        int[] rpms = Tools.Instance.readdatafromfileasint(filename, sh.X_axis_address, sh.X_axis_length, EDCFileType.EDC15P);
                        for (int rows = 0; rows < sh.X_axis_length; rows++)
                        {
                            // whats the rpm?
                            int rpm = rpms[rows];
                            int targetTorque = peakTorque;
                            int targetPower = Tools.Instance.TorqueToPower(targetTorque, rpm);
                            if (targetPower > peakHP) targetTorque = Tools.Instance.PowerToTorque(peakHP, rpm);
                            int targetIQ = Tools.Instance.TorqueToIQ(targetTorque, rpm, 4);
                            // Console.WriteLine("Calculate for rpm: " + rpm.ToString() + " targetT: " + targetTorque.ToString() + " targetP: " + targetPower.ToString() + " targetIQ: " + targetIQ.ToString());
                            targetIQ *= 100;
                            byte b1 = (byte)((targetIQ & 0x00FF00) / 256);
                            byte b2 = (byte)(targetIQ & 0x0000FF);
                            dwdata[rows * sh.Y_axis_length * 2 + (sh.Y_axis_length * 2) - 2] = b1;
                            dwdata[rows * sh.Y_axis_length * 2 + (sh.Y_axis_length * 2) - 1] = b2;
                            dwdata[rows * sh.Y_axis_length * 2 + (sh.Y_axis_length * 2) - 4] = b1;
                            dwdata[rows * sh.Y_axis_length * 2 + (sh.Y_axis_length * 2) - 3] = b2;
                        }
                        SaveAndSyncData(dwdata.Length, (int)sh.Flash_start_address, dwdata, filename, true, "Tuned driver wish", autoUpdateChecksum);

                    }
                }

            }
            else retval = false;
            return retval;
        }

        private void SaveAndSyncData(int length, int address, byte[] data, string fileName, bool useNote, string note, bool autoUpdateChecksum)
        {
            
            foreach (SymbolHelper sh in Tools.Instance.m_symbols)
            {
                if (sh.Length == length)
                {
                    if ((sh.Flash_start_address & 0x0FFFF) == (address & 0x0FFFF))
                    {
                        Tools.Instance.savedatatobinary((int)sh.Flash_start_address, length, data, fileName, useNote, note, EDCFileType.EDC15P);
                    }
                }
            }
            
            if(autoUpdateChecksum) Tools.Instance.UpdateChecksum(fileName, false);
        }
    }
}
