using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    class ScannedFile
    {
        

        string _filename = string.Empty;

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        int _filesize = 0;

        public int Filesize
        {
            get { return _filesize; }
            set { _filesize = value; }
        }
        EDCFileType _filetype = EDCFileType.Unknown;

        public EDCFileType Filetype
        {
            get { return _filetype; }
            set { _filetype = value; }
        }
        private int _hp = 0;

        public int HP
        {
            get { return _hp; }
            set { _hp = value; }
        }
        private int _tq = 0;

        public int TQ
        {
            get { return _tq; }
            set { _tq = value; }
        }

        private int _realhp = 0;

        public int RealHP
        {
            get { return _realhp; }
            set { _realhp = value; }
        }
        private int _realtq = 0;

        public int RealTQ
        {
            get { return _realtq; }
            set { _realtq = value; }
        }

        private string _fuelType = string.Empty;

        public string FuelType
        {
            get { return _fuelType; }
            set { _fuelType = value; }
        }
        private string _carMake = string.Empty;

        public string CarMake
        {
            get { return _carMake; }
            set { _carMake = value; }
        }

        private string _carType = string.Empty;

        public string CarType
        {
            get { return _carType; }
            set { _carType = value; }
        }

        private string _engineType = string.Empty;

        public string EngineType
        {
            get { return _engineType; }
            set { _engineType = value; }
        }

        private string _ecuType = string.Empty;

        public string EcuType
        {
            get { return _ecuType; }
            set { _ecuType = value; }
        }
        private string _partNumber = string.Empty;

        public string PartNumber
        {
            get { return _partNumber; }
            set { _partNumber = value; }
        }

        private string _softwareID = string.Empty;

        public string SoftwareID
        {
            get { return _softwareID; }
            set { _softwareID = value; }
        }

        private string _fuellingType = string.Empty;

        public string FuellingType
        {
            get { return _fuellingType; }
            set { _fuellingType = value; }
        }
        string _checksumType = string.Empty;

        public string ChecksumType
        {
            get { return _checksumType; }
            set { _checksumType = value; }
        }
        string _checksumResult = string.Empty;

        public string ChecksumResult
        {
            get { return _checksumResult; }
            set { _checksumResult = value; }
        }

        private int _numberChecksums = 0;

        public int NumberChecksums
        {
            get { return _numberChecksums; }
            set { _numberChecksums = value; }
        }
        private int _numberChecksumsFail = 0;

        public int NumberChecksumsFail
        {
            get { return _numberChecksumsFail; }
            set { _numberChecksumsFail = value; }
        }
        private int _numberChecksumsOk = 0;

        public int NumberChecksumsOk
        {
            get { return _numberChecksumsOk; }
            set { _numberChecksumsOk = value; }
        }

        private int _numberMapsDetected = 0;

        public int NumberMapsDetected
        {
            get { return _numberMapsDetected; }
            set { _numberMapsDetected = value; }
        }
        private int _numberMapsRecognized = 0;

        public int NumberMapsRecognized
        {
            get { return _numberMapsRecognized; }
            set { _numberMapsRecognized = value; }
        }
        private bool _mapsOk = false;

        public bool MapsOk
        {
            get { return _mapsOk; }
            set { _mapsOk = value; }
        }


        private string _messages = string.Empty;

        public string Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        /*ECUInfo _ecuinfo = new ECUInfo();
        

        internal ECUInfo Ecuinfo
        {
            get { return _ecuinfo; }
            set { _ecuinfo = value; }
        }*/
    }
}
