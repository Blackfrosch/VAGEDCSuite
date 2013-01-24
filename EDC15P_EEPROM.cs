using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    class EDC15P_EEPROM
    {
        private string _fileName = string.Empty;
        byte[] EEPRomBytes;
        Int32 mileage10M =0;
        double mileage = 0;

        public double Mileage
        {
            get { return mileage; }
            set { mileage = value; }
        }
        int key = 0;

        public int Key
        {
            get { return key; }
            set { key = value; }
        }
        byte _immoActiveByte = 0;

        
        string _vin = string.Empty;

        public string Vin
        {
            get { return _vin; }
            set { _vin = value; }
        }
        string _immo = string.Empty;

        public string Immo
        {
            get { return _immo; }
            set { _immo = value; }
        }

        private bool _immoActive = true;

        public bool ImmoActive
        {
            get { return _immoActive; }
            set { _immoActive = value; }
        }

        public void LoadFile(string filename)
        {
            _fileName = filename;
            EEPRomBytes = File.ReadAllBytes(filename);
            key = Convert.ToInt32(EEPRomBytes[0x161]) * 256 + Convert.ToInt32(EEPRomBytes[0x160]);
            mileage10M = Convert.ToInt32(EEPRomBytes[0x1C2] & 0x7F) * 256 * 256 * 256 + Convert.ToInt32(EEPRomBytes[0x1C1]) * 256 * 256 + Convert.ToInt32(EEPRomBytes[0x1C0]) * 256 + Convert.ToInt32(EEPRomBytes[0x1BF]) ;
            mileage = Convert.ToDouble(mileage10M) / 100;
            _immoActiveByte = EEPRomBytes[0x1B0]; // 0x60 = OFF 0x73 = ON
            if (_immoActiveByte == 0x60) _immoActive = false;
            else _immoActive = true;
            _vin = System.Text.ASCIIEncoding.ASCII.GetString(EEPRomBytes, 0x140, 0x11);
            _immo = System.Text.ASCIIEncoding.ASCII.GetString(EEPRomBytes, 0x131, 0x0E);
        }

        public void UpdateFile(string filename)
        {
            EEPRomBytes = File.ReadAllBytes(filename);
            // write key in bytes
            byte khb = Convert.ToByte(key / 256);
            byte klb = Convert.ToByte( key - (Convert.ToInt32(khb) * 256) );
            EEPRomBytes[0x160] = klb;
            EEPRomBytes[0x161] = khb;
            // write mileage in bytes
            // convert to int32


            // write immo active bytes 0x1B0 AND 0x1DE
            byte activeByte = 0x60;
            if(_immoActive) activeByte = 0x73;
            EEPRomBytes[0x1B0] = activeByte;
            EEPRomBytes[0x1DE] = activeByte;

            // write VIN
            if (Vin.Length == 17)
            {
                byte[] _vinbytes = System.Text.ASCIIEncoding.ASCII.GetBytes(_vin);
                int start = 0x140;
                for (int i = 0; i < _vinbytes.Length; i++)
                {
                    EEPRomBytes[start++] = _vinbytes[i];
                }
                start = 0x172;
                for (int i = 0; i < _vinbytes.Length; i++)
                {
                    EEPRomBytes[start++] = _vinbytes[i];
                }
            }
            // write IMMO code
            if (_immo.Length == 14)
            {
                byte[] _immobytes = System.Text.ASCIIEncoding.ASCII.GetBytes(_immo);
                int start = 0x131;
                for (int i = 0; i < _immobytes.Length; i++)
                {
                    EEPRomBytes[start++] = _immobytes[i];
                }
                start = 0x163;
                for (int i = 0; i < _immobytes.Length; i++)
                {
                    EEPRomBytes[start++] = _immobytes[i];
                }
            }
            File.WriteAllBytes(filename, EEPRomBytes);
        }
    }
}
