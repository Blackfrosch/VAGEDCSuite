using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    class ECUInfo
    {
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
    }

    class partNumberConverter
    {
        public string EngineTypeToFuellingType(string enginetype)
        {
            string retval = string.Empty;
            switch (enginetype)
            {
                case "AFD":
                case "CDX":
                case "1Z":
                case "AHU":
                case "AGR":
                case "AHH":
                case "ALE":
                case "ALH":
                case "AFN":
                case "AHF":
                case "ASV":
                case "AVG":
                    retval = "VP37";
                    break;
                case "BSU":
                case "BRU":
                case "BXF":
                case "BXJ":
                case "ANU":
                case "ATD":
                case "AXR":
                case "BEW":
                case "BMT":
                case "AVB":
                case "AVQ":
                case "BSW":
                case "BJB":
                case "BKC":
                case "BLS":
                case "BSV":
                case "BXE":
                case "BPZ":
                case "AJM":
                case "ATJ":
                case "AUY":
                case "BVK":
                case "AWX":
                case "ASZ":
                case "AVF":
                case "BLT":
                case "ARL":
                case "BTB":
                case "BPX":
                case "BUK":
                case "AMF":
                case "BAY":
                case "BMS":
                case "BHC":
                case "BNV":
                case "ATL":
                case "BKD":
                case "BKP":
                case "BMN": 
                case "BMR":
                case "BRD":
                case "BNM":
                case "BWB":
                case "AYZ":
                case "ANY":
                    retval = "PD (pumpe duse)";
                    break;
                case "BDJ":
                case "BST":
                case "BDK":
                    retval = "SDI (suction diesel injection)";
                    break;
                case "AKE":
                case "BAU":
                case "BDH":
                case "BCZ":
                case "BDG":
                case "BFC":
                case "AYM":
                case "AFB":
                case "AKN":
                    retval = "VE mechanical distributor-type injection pump with direct injection";
                    break;


            }
            return retval;
        }

        public int GetNumberOfCylinders(string enginetype, string additionalInfo)
        {
            int retval = 0;
            switch (enginetype)
            {
                case "AFD":
                case "CDX":
                case "1Z":
                case "AHU":
                case "AGR":
                case "AHH":
                case "ALE":
                case "ALH":
                case "AFN":
                case "AHF":
                case "ASV":
                case "AVG":
                    retval = 4;
                    break;
                case "BSU":
                case "BRU":
                case "BXF":
                case "BXJ":
                case "ANU":
                case "ATD":
                case "AXR":
                case "BEW":
                case "BMT":
                case "AVB":
                case "AVQ":
                case "BSW":
                case "BJB":
                case "BKC":
                case "BLS":
                case "BSV":
                case "BXE":
                case "BPZ":
                case "AJM":
                case "ATJ":
                case "AUY":
                case "BVK":
                case "AWX":
                case "ASZ":
                case "AVF":
                case "BLT":
                case "ARL":
                case "BTB":
                case "BPX":
                case "BUK":
                    retval = 4;
                    break;
                case "BDJ":
                case "BST":
                case "BDK":
                    retval = 4;
                    break;
                case "AKE":
                case "BAU":
                case "BDH":
                case "BCZ":
                case "BDG":
                case "BFC":
                case "AYM":
                case "AFB":
                case "AKN":
                case "1T":
                    retval = 6;
                    break;
                // 1.4 R3 PD
                case "BNM":
                case "AMF":
                case "BAY":
                case "BHC":
                case "BMS":
                case "BNV":
                case "ATL":
                case "AYZ": //1.2L R3 3L
                case "ANY": //1.2L R3 3L
                    retval = 3;
                    break;
                //1.9D
                case "1Y":
                case "AEF":
                //1.9 SDI VP37 EDC15V+
                case "BXT":
                case "AEY":
                case "BGM":
                case "BEQ":
                case "BGL":
                case "ANC":
                case "ASY":
                case "AQM":
                    retval = 4;
                    break;
                //2.0 R4 TDI PD EDC16/EDC17
                case "BKD":
                case "BKP":
                case "BMN":
                case "BMR":
                case "BRD":
                    retval = 4;
                    break;

            }
            if (retval == 0)
            {
                if (additionalInfo.ToUpper().Contains("R3")) retval = 3;
                else if (additionalInfo.ToUpper().Contains("R4")) retval = 4;
                else if (additionalInfo.ToUpper().Contains("R5")) retval = 5;
                else if (additionalInfo.ToUpper().Contains("1,4L")) retval = 3;
                else if (additionalInfo.ToUpper().Contains("2.5L")) retval = 6;
                else retval = 4;
            }
            return retval;
        }

        public ECUInfo ConvertPartnumber(string partnumber, int length)
        {
            ECUInfo retval = new ECUInfo();
            retval.PartNumber = partnumber;
            string carMake = string.Empty;
            string carType = string.Empty;
            string ecuType = string.Empty;
            string fuelType = string.Empty;
            string engineType = string.Empty;
            string softwareID = string.Empty;

            switch (partnumber)
            {
                case "0261201101":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201112":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201113":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201124":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201125":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201126":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201136":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201137":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201144":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201146":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201147":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201148":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201149":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201157":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201158":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201167":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201168":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201173":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201174":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201175":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201177":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201185":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201186":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201187":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201188":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201189":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201190":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201192":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201193":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201194":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201199":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201200":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201201":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201204":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201220":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201227":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201228":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201232":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201233":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201234":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201235":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201236":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201238":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201239":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201240":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201241":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201242":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201255":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201256":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201258":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201259":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201260":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201261":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201262":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201263":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201265":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201266":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201267":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201268":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201269":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201270":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201271":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201272":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201292":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201293":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201294":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201295":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201296":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201297":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201298":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201299":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201304":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201310":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201336":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201337":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201338":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201357":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201358":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201359":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201360":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201372":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201373":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201387":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201389":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201390":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201391":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201392":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201393":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201394":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201400":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201401":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201405":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201406":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201408":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201409":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201410":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201411":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201412":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201413":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201414":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201424":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201425":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201445":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201446":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201447":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201448":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201449":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201450":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201451":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201452":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201459":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201460":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201468":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201469":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201470":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201471":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201480":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201481":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201482":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201484":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201485":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201486":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201487":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201488":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201489":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201490":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201491":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201492":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201493":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201494":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201495":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201520":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201521":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201522":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201523":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201524":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201525":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201526":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201527":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201528":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201529":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201548":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201575":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201577":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201578":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201597":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261201605":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201606":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201607":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201608":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201628":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261201629":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201630":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201631":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201632":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201633":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201712":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201713":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201714":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201715":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201716":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201717":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201718":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201719":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201720":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201721":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201722":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201723":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201729":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201730":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201731":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201732":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201772":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201805":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201806":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201807":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201808":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201809":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201810":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201811":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201812":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201813":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201814":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201836":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201837":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201838":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201839":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201840":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201841":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201866":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201867":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201868":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201869":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201870":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201871":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201872":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201945":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261201978":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261204873":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204874":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204875":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204896":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204897":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204898":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204899":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204900":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204909":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204910":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204911":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204912":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204913":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204923":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204962":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204964":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204965":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204966":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204967":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204968":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204969":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204970":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261204995":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204996":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204997":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204998":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261204999":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206001":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206002":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206003":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206004":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206021":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206022":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206023":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206030":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206031":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206034":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206035":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206036":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206037":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206038":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206039":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206040":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206041":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206042":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206043":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206044":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206045":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206046":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206047":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206048":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206049":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206050":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206051":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206105":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206140":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206145":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206146":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206147":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206148":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206149":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206150":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206151":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206197":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206228":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206268":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206269":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206317":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206318":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206319":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206320":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206333":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206434":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206435":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206436":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206437":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206438":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206439":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206440":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206441":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206442":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206443":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206444":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206445":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206446":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206447":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206448":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206449":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206450":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206451":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206452":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206453":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206510":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206521":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206522":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206523":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206524":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206525":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206526":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206527":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206528":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206529":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206530":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206531":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206532":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206533":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206534":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206535":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206536":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206537":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206538":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206539":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206543":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206544":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206545":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206547":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206548":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206549":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206550":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206551":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206552":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206553":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206554":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206580":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206582":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206608":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206609":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206610":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206611":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206650":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206651":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206652":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206664":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206665":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206678":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206679":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206722":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206733":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206734":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206737":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206746":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206747":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206748":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206749":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206750":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206751":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206762":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206763":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206764":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206765":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206766":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206767":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206768":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206769":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206770":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206771":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206772":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206790":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206791":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206792":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206793":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206794":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206795":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206796":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206797":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206812":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206823":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206824":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206825":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206826":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206827":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206830":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206833":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206834":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206835":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206836":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206837":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206838":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206839":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206840":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206841":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206842":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206843":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206844":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206845":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206846":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206847":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206848":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206849":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206850":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261206853":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206854":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206855":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206856":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206857":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206858":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206859":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206863":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206864":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206865":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206866":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206867":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206868":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206869":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206870":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206871":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206872":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206873":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206874":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206875":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206876":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206877":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206878":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206879":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206880":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206881":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206884":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206885":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206886":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206887":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206888":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206889":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206890":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206891":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206892":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206893":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206894":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206895":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206896":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206897":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206898":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206906":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206907":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206917":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206918":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206919":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206928":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206929":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206930":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261206958":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206959":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206960":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206961":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261206964":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207018":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207019":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207020":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207021":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207027":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207028":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207029":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207030":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207040":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207041":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207042":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207043":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207044":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207045":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207046":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207065":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207071":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207072":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207074":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207075":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207085":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207114":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207124":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207177":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207178":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207179":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207180":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207181":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207182":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207183":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207184":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207189":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207190":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207191":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207193":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207194":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207199":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207200":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207201":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207202":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207203":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207204":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207205":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207206":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207207":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207208":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207209":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207210":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207211":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207212":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207213":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207214":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207215":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207216":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207217":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207218":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207219":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207220":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207221":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207222":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207228":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207229":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207230":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207231":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207232":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207233":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207236":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207239":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207240":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207241":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207242":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207243":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207244":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207253":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207254":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207255":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207256":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207257":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207281":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207282":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207284":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207285":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207286":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207291":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207292":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207293":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207294":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207295":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207296":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207297":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207298":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207306":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207307":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207313":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207314":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207315":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207323":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207324":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207325":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207335":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207342":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207345":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207346":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207347":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207348":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207350":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207360":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207361":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207362":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207370":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207401":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207412":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207413":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207414":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207415":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207416":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207417":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207418":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207419":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207420":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207424":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207427":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207428":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207433":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207434":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207435":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207436":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207437":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207438":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207439":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207440":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207441":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207442":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207443":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207444":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207445":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207446":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207447":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207473":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207508":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207509":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207510":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207511":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207533":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207534":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207536":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207537":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207538":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207539":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207540":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207541":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207542":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207546":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207565":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207566":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207579":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207589":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207590":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207591":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207592":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207593":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207594":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207595":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207596":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207617":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207624":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207625":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207628":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207629":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207630":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207631":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207632":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207633":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207634":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207635":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207636":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207637":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207638":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207639":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207645":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207646":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207647":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207648":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207650":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207651":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207652":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207662":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207663":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207677":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207678":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207679":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207680":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207681":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207682":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261207687":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207688":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207689":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207690":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207695":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207699":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207713":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207714":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207717":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207718":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207719":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207747":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207748":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207751":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207752":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207753":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207754":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207755":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207756":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207766":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207767":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207768":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207769":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207770":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207771":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207772":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207773":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207774":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207775":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207776":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207777":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207778":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207779":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207780":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207790":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207793":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207794":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207795":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207796":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207799":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207800":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207801":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207817":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207818":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207819":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207839":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207840":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207841":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207854":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207857":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207883":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207884":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207885":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207886":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207890":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207891":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207892":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207893":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207894":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207895":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207896":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207909":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207926":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207927":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207928":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207929":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207931":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207932":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207934":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207935":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207936":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207937":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207938":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207939":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207940":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207941":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207942":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207943":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207952":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207953":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207954":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207957":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207974":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207975":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207976":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207977":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207978":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261207990":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207991":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207992":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207993":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207994":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207995":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207996":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207997":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261207998":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208001":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208002":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208003":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208004":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208005":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208006":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208007":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208008":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208009":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208023":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208024":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208025":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208026":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208027":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208043":
                    carMake = "Audi";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208044":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208045":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208046":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208051":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208053":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208054":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208055":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208057":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208058":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208061":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208072":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208073":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208085":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208086":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208087":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208088":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208095":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208097":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208100":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208110":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208111":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208112":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208113":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208114":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208115":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208120":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208121":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208131":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208132":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208133":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208139":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208141":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208147":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208148":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208149":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208150":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208161":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208162":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208163":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208184":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208196":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208197":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208200":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208203":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208213":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208221":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208222":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208223":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208224":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208225":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208228":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208229":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208230":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208231":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208232":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208233":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208234":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208235":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208236":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208238":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208239":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208240":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208241":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208242":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208244":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208247":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208248":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208261":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208264":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208265":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208267":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208268":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208269":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208270":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208271":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208275":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208276":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208277":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208278":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208279":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208280":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208281":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208285":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208286":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208287":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208288":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208291":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208292":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208293":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208294":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208295":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208296":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208297":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208298":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208306":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208307":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208308":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208309":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208310":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208311":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208312":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208331":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208332":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208333":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208334":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208335":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208336":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208337":
                    carMake = "Seat";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208339":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208340":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208341":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208342":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208343":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208344":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208346":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208347":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208348":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208363":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208364":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208365":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208370":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208371":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208387":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208389":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208390":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208391":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208392":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208442":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208443":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208444":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208445":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208446":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208447":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208458":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208459":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208460":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208461":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208462":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208463":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208464":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208465":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208466":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208467":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208468":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208469":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208484":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208493":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208494":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208495":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208496":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208497":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208500":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208501":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208502":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208507":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208508":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208509":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208510":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208511":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208512":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208521":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208522":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208523":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208524":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208525":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208526":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208527":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208531":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208532":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208533":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208534":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208535":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208536":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208538":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208544":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208545":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208548":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208550":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208559":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208560":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208561":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208568":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208569":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208583":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208589":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208590":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208591":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208597":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208601":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208602":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208608":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208609":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208611":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208612":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208616":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208619":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208620":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208623":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208628":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208631":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208632":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208633":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208634":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208635":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208636":
                    carMake = "Skoda";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208637":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208638":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208639":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208640":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208641":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208642":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208643":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208645":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208646":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208647":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208648":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208649":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208650":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208651":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208665":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208666":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208667":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208673":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208676":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208677":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208684":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208690":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208691":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208692":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208693":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208701":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208712":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208713":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208714":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208715":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208716":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208723":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208728":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208729":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208730":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208731":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208737":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208738":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208750":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208751":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208752":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208753":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208754":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208755":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208756":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208772":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208773":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208774":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208776":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208777":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208778":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208779":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208780":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208781":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208792":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208793":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208794":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208795":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208821":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208822":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208823":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208828":
                    carMake = "Skoda";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208837":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208838":
                    carMake = "Volkswagen";
                    ecuType = "ME7-510";
                    fuelType = "Benzin";
                    break;
                case "0261208845":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208850":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208851":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208852":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208853":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208854":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208855":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208862":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208865":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208866":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208877":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208897":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208898":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208917":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208918":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208921":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208922":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208944":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208950":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261208952":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208953":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208954":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208955":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261208956":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0281001609":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001410":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    softwareID = "038906018A";
                    ecuType = "MSA15"; //<GS-20120913>
                    engineType = "AHU";
                    fuelType = "Diesel";
                    break;
                case "0281001611":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    softwareID = "038906018D";
                    engineType = "AGR";
                    break;
                case "0281001613":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    softwareID = "038906018J";
                    engineType = "AHF";
                    break;
                case "0281001642":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001679":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001681":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001689":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001693":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001694":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001695":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001696":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001707":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0281001720":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    softwareID = "038906018P";
                    engineType = "AFN";
                    break;
                case "0281001721":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    engineType = "AFN";
                    softwareID = "038906018S";
                    break;
                case "0281001724":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001725":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001726":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001727":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001728":
                    carMake = "Audi";
                    carType = "A3";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    engineType = "AHU";
                    softwareID = "038906018AR";
                    break;
                case "0281001732":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001733":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    softwareID = "038906018AN";
                    engineType = "ALH";
                    break;
                case "0281001743":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001748":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001749":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001755":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0281001756":
                    carMake = "Audi";
                    carType = "A3";
                    engineType = "AFN";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    softwareID = "038906018BA";
                    break;
                case "0281001757":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0281001758":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001759":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001765":
                    carMake = "VW/AUDI";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001769":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001770":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001771":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001790":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0281001808":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001820":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001821":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001845":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    engineType = "AGR";
                    softwareID = "038906018BL";
                    break;
                case "0281001846":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    softwareID = "038906018BM";
                    engineType = "AHF";
                    fuelType = "Diesel";
                    break;
                case "0281001847":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001848":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001849":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001850":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001851":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    softwareID = "038906018AE";
                    engineType = "ALH";
                    break;
                case "0281001852":
                    carMake = "Audi";
                    carType = "A3";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    engineType = "ALH";
                    softwareID = "038906018AK";
                    break;
                case "0281001853":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001854":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.43";
                    fuelType = "Diesel";
                    break;
                case "0281001855":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0281001856":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0281001857":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.43";
                    fuelType = "Diesel";
                    break;
                case "0281001864":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001865":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    softwareID = "038906019AB";
                    break;
                case "0281001885":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0281001899":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001900":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001901":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001902":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001903":
                    carMake = "Skoda";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001904":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001907":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001911":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001912":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281001913":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281001914":
                    carMake = "Seat";
                    carType = "Cordoba";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    engineType = "ALH";
                    softwareID = "038906018EC";
                    break;
                case "0281001915":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281001916":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281001925":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001926":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001927":
                    carMake = "Seat";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001958":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281001959":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001960":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001961":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001962":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281001963":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281001964":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281001965":
                    carMake = "Audi";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281001966":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    engineType = "AFN";
                    carType = "A4";
                    softwareID = "038906018FD";
                    break;
                case "0281010006":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    softwareID = "038906018FB";
                    engineType = "AFN";
                    break;
                case "0281010007":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281010015":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.4";
                    fuelType = "Diesel";
                    break;
                case "0281010046":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010048":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    engineType = "ARL";
                    softwareID = "038906019AQ";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010049":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+";
                    fuelType = "Diesel";
                    break;
                case "0281010054":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281010055":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281010056":
                    carMake = "Seat";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281010057":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281010063":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281010064":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010065":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0281010066":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    softwareID = "038906018FH";
                    engineType = "AFN";
                    fuelType = "Diesel";
                    break;
                case "0281010092":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010093":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010094":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    softwareID = "038906019AN";
                    engineType = "ATJ";
                    break;
                case "0281010114":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281010115":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281010116":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281010169":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010170":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010171":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    softwareID = "038906018FS";
                    fuelType = "Diesel";
                    engineType = "AHH";
                    break;
                case "0281010172":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010173":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    break;
                case "0281010180":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010181":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010182":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010211":
                    carMake = "Seat";
                    ecuType = "EDC15P+";
                    fuelType = "Diesel";
                    break;
                case "0281010212":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010213":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010214":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010215":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P+";
                    fuelType = "Diesel";
                    engineType = "AUY";
                    softwareID = "038906019AR";
                    break;
                case "0281010216":
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    engineType = "AUY";
                    softwareID = "038906019J";
                    break;
                case "0281010217":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    softwareID = "038906019CE";
                    engineType = "AJM";
                    break;
                case "0281010218":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    softwareID = "038906019CD";
                    engineType = "AJM";
                    break;
                case "0281010220":
                    carMake = "Audi";
                    carType = "A2";
                    ecuType = "EDC15P+";
                    fuelType = "Diesel";
                    softwareID = "045906019G";
                    engineType = "AMF";
                    break;
                case "0281010221":
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    softwareID = "038906019BF";
                    engineType = "AUY";
                    break;
                case "0281010224":
                    carMake = "Audi";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    carType = "A6";
                    engineType = "AJM";
                    softwareID = "038906019BS";
                    break;
                case "0281010225":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    break;
                case "0281010226":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15P+-22.3";
                    fuelType = "Diesel";
                    engineType = "AJM";
                    softwareID = "038906019BR";
                    break;
                case "0281010227":
                    carMake = "Audi";
                    ecuType = "EDC15P+-22.4";
                    fuelType = "Diesel";
                    break;
                case "0281010242":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281010243":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281010244":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0281010245":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0281010246":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0281010289":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019AF";
                    break;
                case "0281010301":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010302":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P+-22.3.2";
                    engineType = "AJM";
                    fuelType = "Diesel";
                    softwareID = "038906019CJ";
                    break;
                case "0281010303":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    engineType = "AJM";
                    softwareID = "038906019AJ";
                    break;
                case "0281010304":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15P+-22.3.1";
                    fuelType = "Diesel";
                    softwareID = "038906019CC";
                    engineType = "ATJ";
                    break;
                case "0281010305":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P+-22.3.1";
                    fuelType = "Diesel";
                    engineType = "AJM";
                    softwareID = "038906019BK";
                    break;
                case "0281010306":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010307":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010308":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "A3";
                    engineType = "ASZ";
                    softwareID = "038906019CK";
                    break;
                case "0281010309":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019CA";
                    break;
                case "0281010324":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V";
                    fuelType = "Diesel";
                    break;
                case "0281010325":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0281010326":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0281010357":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010392":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010403":
                    carMake = "Seat";
                    carType = "Leon";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019CL";
                    engineType = "ARL"; 
                    break;
                case "0281010406":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "A4";
                    engineType = "AWX";
                    softwareID = "038906019CG";
                    break;
                case "0281010470":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010476":
                    carMake = "Audi";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010477":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010491":
                    carMake = "Seat";
                    ecuType = "EDC15P+-22.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281010497":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019DF";
                    engineType = "ATD";
                    break;
                case "0281010514":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+";
                    fuelType = "Diesel";
                    break;
                case "0281010517":
                    carMake = "Skoda";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019R";
                    break;
                case "0281010541":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010542":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0281010543":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Passat";
                    engineType = "AVF";
                    softwareID = "038906019DS";
                    break;
                case "0281010547":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010548":
                    carMake = "Audi";
                    ecuType = "EDC15P+";
                    fuelType = "Diesel";
                    break;
                case "0281010553":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "039906019EB";
                    break;
                case "0281010554":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010558":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    softwareID = "039906019EH";
                    engineType = "AWX";
                    break;
                case "0281010561":
                    carMake = "Audi";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    engineType = "ASZ";
                    carType = "A3";
                    softwareID = "038906019EF";
                    break;
                case "0281010574":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010625":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010628":
                    carMake = "Volkswagen";
                    carType = "Lupo";
                    softwareID = "045906019AE";
                    ecuType = "EDC15P+22.4.2";
                    fuelType = "Diesel";
                    engineType = "AYZ";
                    break;
                case "0281010629":
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019FA";
                    engineType = "AUY";
                    break;
                case "0281010222":
                    carMake = "Volkswagen"; 
                    carType = "Sharan"; 
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019BF";
                    engineType = "AUY";
                    break;
                case "0281010630":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019ET";
                    engineType = "AJM";
                    break;
                case "0281010662":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019FF";
                    break;
                case "0281010663":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    carType = "Golf";
                    softwareID = "038906019CR";
                    break;
                case "0281010664":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019FB";
                    break;
                case "0281010665":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019EJ";
                    carType = "Passat";
                    engineType = "AVB";
                    break;
                case "0281010666":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019EL";
                    break;
                case "0281010667":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019EK";
                    engineType = "AVB";
                    break;
                case "0281010668":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019EM";
                    break;
                case "0281010669":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019FN";
                    engineType = "AVB";
                    break;
                case "0281010671":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010672":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010673":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010674":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010675":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010687":
                    carMake = "Seat";
                    carType = "Leon";
                    engineType = "ARL";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019FK";
                    break;
                case "0281010697":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P+22.4.2";
                    fuelType = "Diesel";
                    engineType = "AMF";
                    softwareID = "045906019C";
                    break;
                case "0281010698":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281010699":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010700":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010701":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019EP";
                    engineType = "AVF";
                    break;
                case "0281010702":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Golf";
                    engineType = "ASZ";
                    softwareID = "038906019FG";
                    break;
                case "0281010704":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "AWX";
                    softwareID = "038906019ER";
                    break;
                case "0281010705":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010728":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281010729":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "A4";
                    engineType = "AWX";
                    softwareID = "038906019FP";
                    break;
                case "0281010744":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P+-22.3.2";
                    fuelType = "Diesel";
                    engineType = "ARL";
                    softwareID = "038906019FE";
                    break;
                case "0281010751":
                    carMake = "Volkswagen";
                    carType = "Alhambra";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019FC";
                    engineType = "AUY";
                    break;
                case "0281010789":
                    carMake = "Skoda";
                    ecuType = "EDC15P+22.3.2";
                    engineType = "ATD";
                    softwareID = "038906019FJ";
                    fuelType = "Diesel";
                    break;
                case "0281010813":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019GG";
                    break;
                case "0281010838":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010839":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010844":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010845":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010865":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.4.2";
                    fuelType = "Diesel";
                    carType = "Polo 1.4";
                    engineType = "AMF";
                    softwareID = "045906019AP";
                    break;
                case "0281010866":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281010891":
                    carMake = "Seat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Ibiza";
                    engineType = "ASZ";
                    softwareID = "038906019DQ";
                    break;
                case "0281010892":
                    carMake = "Audi";
                    carType = "A3";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019GC";
                    break;
                case "0281010939":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010940":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "AWX";
                    softwareID = "038906019GS";
                    break;
                case "0281010941":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "AVF";
                    softwareID = "038906019GQ";
                    break;
                case "0281010942":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010943":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010944":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Passat";
                    engineType = "AVB";
                    softwareID = "038906019GL";
                    break;
                case "0281010945":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010946":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010947":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019HT";
                    break;
                case "0281010957":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ATB";
                    softwareID = "038906019HQ";
                    break;
                case "0281010958":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010959":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010960":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010976":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Golf";
                    engineType = "ARL";
                    softwareID = "038906019HH";
                    break;
                case "0281010977":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Golf";
                    engineType = "ASZ";
                    softwareID = "038906019HJ";
                    break;
                case "0281010978":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281010981":
                case "0281001308": 
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ASZ";
                    carType = "A3";
                    softwareID = "038906019FT";
                    break;
                case "0281010985":
                    carMake = "Seat";
                    carType = "Leon";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ARL";
                    softwareID = "038906019HK";
                    break;
                case "0281011036":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019JL";
                    engineType = "AVF";
                    break;
                case "0281011065":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    carType = "Golf";
                    engineType = "ATD";
                    softwareID = "038906019DD";
                    break;
                case "0281011109":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2(H)";
                    fuelType = "Diesel";
                    break;
                case "0281011144":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019LQ";
                    break;
                case "0281011191":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019KP";
                    break;
                case "0281011193":
                    carMake = "Seat";
                    carType = "Leon";
                    engineType = "ARL";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019KG";
                    break;
                case "0281011195":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019KH";
                    engineType = "ATD";
                    break;
                case "0281011196":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011197":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011198":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019LD";
                    break;
                case "0281011201":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "AWX";
                    carType = "Passat";
                    softwareID = "038906019KE";
                    break;
                case "0281011206":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019LB";
                    engineType = "ATD";
                    break;
                case "0281011207":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011209":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011211":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011212":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011215":
                    carMake = "Audi";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011217":
                    carMake = "Seat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "ASZ";
                    carType = "Ibiza";
                    softwareID = "038906019LA";
                    break;
                case "0281011218":
                    carMake = "Seat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011220":
                    carMake = "Skoda";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011221":
                    carMake = "Skoda";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011403":
                    carMake = "Seat";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011539":
                    carMake = "Volkswagen";
                    carType = "Beetle";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    engineType = "AXR";
                    softwareID = "038906019KR";
                    break;
                case "0281011819":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281011820":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281012186":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281012194":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281012195":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.4.2";
                    fuelType = "Diesel";
                    softwareID = "045906019CA";
                    break;
                case "0281012211":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281012287":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281012668":
                    carMake = "Seat";
                    carType = "Ibiza"; //Cupra BUK
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019NT";
                    engineType = "BUK";
                    break;
                case "0281012920":
                    carMake = "Skoda";
                    carType = "Octavia";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019PB";
                    engineType = "AXR";
                    break;
                case "0281012924":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281012932":
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    engineType = "BVK";
                    softwareID = "038906019PA";
                    break;
                case "0281013082":
                    carMake = "Skoda";
                    carType = "Fabia";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019NS";
                    break;
                case "0281013886":
                    carMake = "Audi";
                    ecuType = "EDC15P+23.4.1";
                    fuelType = "Diesel";
                    break;
                case "0281015048":
                    carMake = "Seat";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281015334":
                    carMake = "Skoda";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281015430":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281015431":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281015432":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    engineType = "ASZ";
                    softwareID = "038906019PJ";
                    carType = "Polo";
                    break;
                case "0281015570":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P+23.3.2";
                    fuelType = "Diesel";
                    break;
                case "0986262529":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986262535":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264034":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264368":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264494":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264496":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264497":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264498":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264499":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264500":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264501":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264502":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264504":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264505":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264506":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264513":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264514":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264515":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264531":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264533":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264542":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264547":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264557":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264558":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264562":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264563":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264565":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264566":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264567":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264574":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264586":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264588":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264589":
                    carMake = "Volkswagen";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264594":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264595":
                    carMake = "Audi";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0986264599":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264602":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264603":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264604":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264607":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264623":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264633":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264635":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264646":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264647":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264648":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264652":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986264653":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0986282358":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282359":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282360":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282361":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282362":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282363":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282364":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282365":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282366":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282367":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282368":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282369":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282370":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282371":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282372":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282373":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282374":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282375":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282376":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282377":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282378":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282379":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282380":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282381":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282382":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282383":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282384":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282385":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282386":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282387":
                    carMake = "VW/AUDI";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282388":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282389":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282390":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282391":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.1";
                    fuelType = "Diesel";
                    break;
                case "0986282392":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282393":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282394":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282395":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282396":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282397":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282398":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282399":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282400":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282401":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282402":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282403":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.43";
                    fuelType = "Diesel";
                    break;
                case "0986282404":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0986282405":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0986282406":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.43";
                    fuelType = "Diesel";
                    break;
                case "0986282407":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282408":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282409":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282410":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282411":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282412":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282413":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282414":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282415":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282416":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282417":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282418":
                    carMake = "Seat";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282419":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282420":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282421":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282422":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282423":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282424":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282425":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282426":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282427":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282428":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282429":
                    carMake = "VW/SEAT";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282430":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282431":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282432":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282433":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.29";
                    fuelType = "Diesel";
                    break;
                case "0986282434":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282435":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282436":
                    carMake = "Audi";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282437":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282438":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282439":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282440":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282441":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282442":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282443":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282444":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282445":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282446":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282447":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.36";
                    fuelType = "Diesel";
                    break;
                case "0986282448":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282449":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.35";
                    fuelType = "Diesel";
                    break;
                case "0986282450":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0986282451":
                    carMake = "Volkswagen";
                    ecuType = "EDC15V-5.44";
                    fuelType = "Diesel";
                    break;
                case "0986282452":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0986282453":
                    carMake = "Skoda";
                    ecuType = "EDC15V-5.45";
                    fuelType = "Diesel";
                    break;
                case "0261S04218":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04221":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04225":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04226":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04227":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04291":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04292":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04293":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04294":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04300":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04301":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04302":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04389":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04454":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04607":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04608":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04628":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04629":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04630":
                    carMake = "Audi";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04649":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04650":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04651":
                    carMake = "Volkswagen";
                    ecuType = "ME7-11";
                    fuelType = "Benzin";
                    break;
                case "0261S04656":
                    carMake = "Seat";
                    ecuType = "ME7-5";
                    fuelType = "Benzin";
                    break;
                case "0261200258":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200276":
                    carMake = "Audi";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200702":
                    carMake = "Audi";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200711":
                    carMake = "Audi";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200713":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200715":
                    carMake = "Audi";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200734":
                    carMake = "Skoda";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200736":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200738":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200740":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200742":
                    carMake = "Audi";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200744":
                    carMake = "Audi";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200751":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200753":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200755":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200757":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200761":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200765":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200767":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200769":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200771":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261200775":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200777":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200785":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200787":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261200790":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261200791":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261200797":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261200799":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261201123":
                    carMake = "Volkswagen AG , GM DO BRASIL";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261201138":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201139":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201143":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201191":
                    carMake = "Volkswagen AG , VW-FAW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261201222":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201223":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201224":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201225":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201226":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201230":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5";
                    fuelType = "Benzin";
                    break;
                case "0261201231":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5";
                    fuelType = "Benzin";
                    break;
                case "0261201243":
                    carMake = "Volkswagen";
                    ecuType = "ME7.1";
                    fuelType = "Benzin";
                    break;
                case "0261201311":
                    carMake = "AVTO VAZ , AUDI";
                    ecuType = "M7-97";
                    fuelType = "Benzin";
                    break;
                case "0261201328":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201329":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201330":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201331":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201332":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201333":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201334":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201335":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201395":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.20";
                    fuelType = "Benzin";
                    break;
                case "0261201396":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.20";
                    fuelType = "Benzin";
                    break;
                case "0261201397":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.20";
                    fuelType = "Benzin";
                    break;
                case "0261201398":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.20";
                    fuelType = "Benzin";
                    break;
                case "0261201399":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201402":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261201403":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261201417":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5";
                    fuelType = "Benzin";
                    break;
                case "0261201663":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201665":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201666":
                    carMake = "Skoda";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201667":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201668":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201669":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201670":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261201671":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261203183":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203185":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203187":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203189":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203191":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203193":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203195":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203197":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203199":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203201":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203300":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0261203303":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203305":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203307":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203309":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203311":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203313":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203315":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203317":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261203319":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261203321":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0261203341":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203343":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203345":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203347":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203352":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203361":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203383":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203385":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203390":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203392":
                    carMake = "Volkswagen";
                    ecuType = "MA1-7";
                    fuelType = "Benzin";
                    break;
                case "0261203394":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203396":
                    carMake = "Volkswagen";
                    ecuType = "MA1-7";
                    fuelType = "Benzin";
                    break;
                case "0261203400":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203408":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203412":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203414":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203416":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203418":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203420":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203422":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203424":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203426":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203428":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203430":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203432":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203434":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203436":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0261203457":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203486":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203489":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203513":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203515":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203517":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203519":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203521":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203544":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203545":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203546":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203549":
                    carMake = "Skoda";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203551":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261203553":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261203554":
                    carMake = "Audi";
                    ecuType = "M3.2";
                    fuelType = "Benzin";
                    break;
                case "0261203555":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261203592":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203594":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203606":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203608":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203610":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0261203612":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203613":
                    carMake = "Volkswagen";
                    ecuType = "MP9.0";
                    fuelType = "Benzin";
                    break;
                case "0261203614":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203636":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203648":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203651":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203706":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203708":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203710":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203720":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203722":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203724":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203728":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203745":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203747":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203749":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203751":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203753":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203757":
                    carMake = "Seat";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203760":
                    carMake = "Audi";
                    ecuType = "M3-21";
                    fuelType = "Benzin";
                    break;
                case "0261203762":
                    carMake = "Audi";
                    ecuType = "M3-21";
                    fuelType = "Benzin";
                    break;
                case "0261203764":
                    carMake = "Audi";
                    ecuType = "M3-21";
                    fuelType = "Benzin";
                    break;
                case "0261203789":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203793":
                    carMake = "Volkswagen";
                    ecuType = "M1.5.4";
                    fuelType = "Benzin";
                    break;
                case "0261203794":
                    carMake = "Volkswagen";
                    ecuType = "M1.5.4";
                    fuelType = "Benzin";
                    break;
                case "0261203845":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203860":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203865":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203896":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203898":
                    carMake = "Seat";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0261203906":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261203915":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203917":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203919":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203921":
                    carMake = "Skoda";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203923":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203930":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203932":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203934":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261203939":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261203941":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261203957":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261203958":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261203963":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203964":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203965":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261203966":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203967":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203968":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203969":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203970":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203971":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261203973":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203975":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203976":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203977":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261203996":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204022":
                    carMake = "Volkswagen";
                    ecuType = "MP9.0";
                    fuelType = "Benzin";
                    break;
                case "0261204024":
                    carMake = "Volkswagen";
                    ecuType = "MP9.0";
                    fuelType = "Benzin";
                    break;
                case "0261204027":
                    carMake = "Volkswagen";
                    ecuType = "M1.5.4";
                    fuelType = "Benzin";
                    break;
                case "0261204034":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204036":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204044":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204055":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204057":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204060":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261204077":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261204078":
                    carMake = "Volkswagen";
                    ecuType = "M3.8.1";
                    fuelType = "Benzin";
                    break;
                case "0261204095":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204097":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204124":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204125":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204127":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204147":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0261204154":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204164":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204165":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204171":
                    carMake = "Volkswagen";
                    ecuType = "M1.5.4";
                    fuelType = "Benzin";
                    break;
                case "0261204172":
                    carMake = "Volkswagen";
                    ecuType = "M1.5.4";
                    fuelType = "Benzin";
                    break;
                case "0261204175":
                    carMake = "Audi";
                    ecuType = "M3-21";
                    fuelType = "Benzin";
                    break;
                case "0261204176":
                    carMake = "Volkswagen";
                    ecuType = "M3.8.2";
                    fuelType = "Benzin";
                    break;
                case "0261204177":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204179":
                    carMake = "Audi AG , BMW";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204181":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204183":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204185":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204192":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204193":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204205":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204215":
                    carMake = "Audi AG , DC";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204236":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204238":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204252":
                    carMake = "Audi";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0261204254":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204256":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204257":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204258":
                    carMake = "Volkswagen AG , AUDI";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204260":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204262":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204263":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204264":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204266":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261204272":
                    carMake = "Volkswagen";
                    ecuType = "MP9.0";
                    fuelType = "Benzin";
                    break;
                case "0261204280":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204282":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204284":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204317":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204318":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204319":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204334":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204336":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204338":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204340":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204342":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204344":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204345":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204346":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204372":
                    carMake = "Skoda , SAAB";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204373":
                    carMake = "Skoda , VW";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204375":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204384":
                    carMake = "Audi AG , FIAT";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204427":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204431":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204436":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204438":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204441":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204462":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204465":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204470":
                    carMake = "Audi AG , VW";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204472":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261204473":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204474":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204486":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204490":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204501":
                    carMake = "Volkswagen";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261204502":
                    carMake = "Volkswagen";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261204503":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261204504":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0261204515":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204517":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204519":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204563":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204564":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204587":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204593":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204594":
                    carMake = "Audi AG , SEAT";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204595":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204597":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204598":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204599":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204613":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204614":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204615":
                    carMake = "Volkswagen";
                    ecuType = "M3.8.2";
                    fuelType = "Benzin";
                    break;
                case "0261204617":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204619":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204626":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204627":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204641":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204648":
                    carMake = "Volkswagen";
                    ecuType = "MP9.0";
                    fuelType = "Benzin";
                    break;
                case "0261204670":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204671":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204672":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204673":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204674":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204675":
                    carMake = "Audi AG , VW";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204676":
                    carMake = "Audi AG , VW";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204677":
                    carMake = "Audi";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204678":
                    carMake = "Audi";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204679":
                    carMake = "Audi";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204681":
                    carMake = "Skoda , SKODA";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204684":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204685":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204686":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204687":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204688":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204690":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204704":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204714":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204715":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204716":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204720":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0261204725":
                    carMake = "Seat , SEAT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204737":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204738":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204745":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204751":
                    carMake = "Volkswagen AG , GM DO BRASIL";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204752":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204753":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204759":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204760":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204761":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204762":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204766":
                    carMake = "Audi AG , DELTA (SA)";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204767":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204768":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204773":
                    carMake = "Volkswagen";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0261204774":
                    carMake = "Audi";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0261204776":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204777":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204778":
                    carMake = "Audi AG , VW";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204787":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204794":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204795":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204796":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204797":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204800":
                    carMake = "Volkswagen AG , HYUNDAI";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204804":
                    carMake = "Audi";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0261204805":
                    carMake = "Audi AG , KIA";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204806":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204807":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204808":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204811":
                    carMake = "Audi AG , DC";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204812":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204813":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204818":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204823":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204824":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204829":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204844":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204847":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204848":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204864":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204865":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204866":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204867":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204868":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204869":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204870":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204871":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204872":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204878":
                    carMake = "Seat , SEAT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204887":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0261204888":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0261204893":
                    carMake = "Audi AG , OPEL";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204894":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204907":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204908":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204921":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204922":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261204926":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204927":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204950":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204952":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261204956":
                    carMake = "Volkswagen";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0261204957":
                    carMake = "Audi AG , VW";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0261204961":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204963":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261204978":
                    carMake = "Volkswagen AG , VW/AUDI";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261204979":
                    carMake = "VW Mexico , VW/AUDI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261204982":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261204991":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204992":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204993":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261204994":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206005":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206006":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206007":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206015":
                    carMake = "Audi AG , FORD";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206016":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206017":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206018":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206019":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206020":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206033":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206076":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206077":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206078":
                    carMake = "Audi AG , VW";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206079":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206088":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206089":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206094":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261206095":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261206096":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206097":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206106":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206107":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206108":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206109":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206110":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206111":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206112":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206113":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206114":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206115":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206116":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206122":
                    carMake = "Audi AG , MP7.0 PROJ. 14/179.8 PSA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206123":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206124":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206126":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206127":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206141":
                    carMake = "Skoda , SKODA";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206144":
                    carMake = "Skoda , RENAULT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206164":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206165":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206166":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206175":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206176":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206180":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206181":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206184":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206186":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206187":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206188":
                    carMake = "Volkswagen";
                    ecuType = "ME7.1";
                    fuelType = "Benzin";
                    break;
                case "0261206189":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206190":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206191":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206192":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206193":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206194":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206195":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206196":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206198":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206199":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206200":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206201":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206202":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206203":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206204":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206205":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206206":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206207":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206208":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206209":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206210":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206222":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206226":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261206230":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0261206231":
                    carMake = "Skoda , TOYOTA";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206239":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206240":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206260":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206261":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206262":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206263":
                    carMake = "Volkswagen AG , VW MEXICO";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206264":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206265":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206266":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206267":
                    carMake = "Volkswagen AG , VW MEXICO";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206275":
                    carMake = "Audi";
                    ecuType = "M3.8.5";
                    fuelType = "Benzin";
                    break;
                case "0261206279":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206315":
                    carMake = "Audi AG , DC";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206322":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206323":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206324":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206325":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206326":
                    carMake = "Volkswagen AG , VW MEXICO";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206327":
                    carMake = "Volkswagen AG , VW MEXICO";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206328":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206329":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206336":
                    carMake = "Audi AG , PSA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206337":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206343":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206353":
                    carMake = "Audi AG , HYUNDAI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206354":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206355":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206356":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206357":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206358":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206359":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206360":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206361":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206362":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206363":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206364":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206365":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206366":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206367":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206368":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206369":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206370":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206371":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206372":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206373":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206374":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206375":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206376":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206377":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206378":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206379":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206380":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206381":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206382":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206383":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206384":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206385":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206386":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206387":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206388":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206389":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206390":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206391":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206392":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206393":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206394":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206395":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206396":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206397":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206398":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206399":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206400":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206401":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206402":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206427":
                    carMake = "Audi AG , KIA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206429":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206430":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206503":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206504":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206505":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206508":
                    carMake = "Volkswagen AG , VW MEXICO";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206511":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206512":
                    carMake = "Seat , SEAT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206513":
                    carMake = "Seat , SEAT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206514":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206515":
                    carMake = "Audi";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206516":
                    carMake = "Audi";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206517":
                    carMake = "Volkswagen AG , AUDI";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206518":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206519":
                    carMake = "Skoda , SKODA";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206520":
                    carMake = "Skoda , SKODA";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206556":
                    carMake = "Audi AG , AVTO VAZ";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206557":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206558":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206559":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206560":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206561":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206562":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206587":
                    carMake = "Seat , SEAT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206588":
                    carMake = "Seat , SEAT";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0261206589":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206590":
                    carMake = "Audi";
                    ecuType = "M3.8.3";
                    fuelType = "Benzin";
                    break;
                case "0261206591":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206592":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206593":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206594":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206616":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206617":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206619":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206620":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206623":
                    carMake = "Audi";
                    ecuType = "M5.9.2";
                    fuelType = "Benzin";
                    break;
                case "0261206629":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206630":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206632":
                    carMake = "Audi AG , KIA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206635":
                    carMake = "Audi AG , PSA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206636":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206637":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206638":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206639":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206640":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206641":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206642":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206643":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206644":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206645":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206646":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206680":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206681":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206682":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206723":
                    carMake = "Audi AG , DC";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206724":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206726":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206735":
                    carMake = "Volkswagen AG , VW-FAW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206736":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206757":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206758":
                    carMake = "Skoda , SKODA";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206759":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206760":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206761":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206774":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206775":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206776":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206777":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206778":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206779":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206780":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206781":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206782":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206783":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206784":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206785":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206786":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206787":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206788":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206789":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206798":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0261206799":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206804":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261206805":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206806":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206807":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206808":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206809":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206810":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206811":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206820":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206821":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206822":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0261206851":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206921":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206922":
                    carMake = "Skoda , SKODA";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206923":
                    carMake = "Skoda , SKODA";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206924":
                    carMake = "Skoda , SKODA";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261206952":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206953":
                    carMake = "Audi AG , KIA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206954":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261206955":
                    carMake = "Auto Europa VW-Portugal , AUDI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207001":
                    carMake = "Audi AG , KIA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207002":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207003":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207004":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207005":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207006":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207007":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207008":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207009":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207010":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207011":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207012":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207013":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207014":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207015":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207016":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207017":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207035":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207036":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207037":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207048":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207055":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261207084":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207094":
                    carMake = "Volkswagen AG , AUDI";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207095":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207096":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207111":
                    carMake = "Audi AG , HYUNDAI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207115":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207125":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207134":
                    carMake = "Audi AG , PSA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207135":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207136":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207137":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207138":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207139":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207140":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207141":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207142":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207143":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207144":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207145":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207146":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207147":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207148":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207149":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207152":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207153":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207154":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207235":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207247":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261207248":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261207249":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261207250":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0261207251":
                    carMake = "Volkswagen AG , VW MEXICO";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207252":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207260":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207267":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207268":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207269":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207270":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207271":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207272":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207273":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207280":
                    carMake = "Audi AG , AMG";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207283":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207287":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207319":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207371":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207373":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207374":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207375":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207376":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207377":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207378":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207379":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207380":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207381":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207389":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0261207449":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207450":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207451":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207452":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207453":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207454":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207455":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207456":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207457":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207458":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207459":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207460":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207461":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207462":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207463":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207466":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207467":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207468":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207469":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207470":
                    carMake = "Volkswagen AG , AUDI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207471":
                    carMake = "Volkswagen AG , AUDI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207494":
                    carMake = "Audi AG , RENAULT";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207495":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207496":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207497":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207498":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207499":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207500":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207501":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207502":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207503":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207504":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207505":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207506":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207507":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207532":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207557":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5";
                    fuelType = "Benzin";
                    break;
                case "0261207567":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207568":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207569":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207580":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207599":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207600":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207612":
                    carMake = "Audi AG , DC-AMG";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207613":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207614":
                    carMake = "Volkswagen AG , AUDI";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207615":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207616":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207618":
                    carMake = "Volkswagen";
                    ecuType = "M3.8.3";
                    fuelType = "Benzin";
                    break;
                case "0261207640":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207641":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207642":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207643":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207644":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207653":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207660":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207661":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207701":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207702":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207716":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207749":
                    carMake = "Audi AG , VW-FAW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207750":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207812":
                    carMake = "Audi AG , VOLVO";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207813":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207814":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207815":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207816":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207820":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207838":
                    carMake = "Audi AG , AVTO VAZ";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207855":
                    carMake = "Volvo , AUDI";
                    ecuType = "ME7-01";
                    fuelType = "Benzin";
                    break;
                case "0261207879":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207880":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207887":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207888":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207889":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261207930":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5";
                    fuelType = "Benzin";
                    break;
                case "0261207966":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261207989":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208038":
                    carMake = "Audi AG , TOYOTA";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208039":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208040":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208041":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208108":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261208109":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261208237":
                    carMake = "Audi AG , VW";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208262":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208263":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208305":
                    carMake = "Audi";
                    ecuType = "ME7.1";
                    fuelType = "Benzin";
                    break;
                case "0261208313":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208322":
                    carMake = "Volkswagen";
                    ecuType = "ME7.1.1";
                    fuelType = "Benzin";
                    break;
                case "0261208546":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208547":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208595":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261208596":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261208607":
                    carMake = "Volkswagen";
                    ecuType = "M3.8.3";
                    fuelType = "Benzin";
                    break;
                case "0261208696":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0261208707":
                    carMake = "Shanghai Volkswagen , VW-SHANGHAI";
                    ecuType = "MSA15.5";
                    fuelType = "Benzin";
                    break;
                case "0261208736":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5.10";
                    fuelType = "Benzin";
                    break;
                case "0261209553":
                    carMake = "Volkswagen";
                    ecuType = "ME7.5";
                    fuelType = "Benzin";
                    break;
                case "0281001306":
                    carMake = "Volkswagen";
                    carType = "Transporter";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021A";
                    engineType = "ACV";
                    break;
                case "0281001368":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021AT";
                    break;
                case "0281001369":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021AT";
                    break;
                case "0281001424":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001451":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001491":
                    carMake = "Audi";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001533":
                    carMake = "Seat";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001536":
                    carMake = "Seat";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001541":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001560":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001562":
                    carMake = "Audi";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001564":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001568":
                    carMake = "Audi";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001570":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001572":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001576":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001578":
                    carMake = "Audi";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001580":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001582":
                    carMake = "Seat";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001584":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    engineType = "AFN";
                    softwareID = "028906021EK";
                    break;
                case "0281001586":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001596":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001648":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001652":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001666":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001668":
                    carMake = "Audi";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001683":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001691":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0281001729":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001730":
                case "0281001762":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001746":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001764":
                    carMake = "Volkswagen";
                    carType = "Transporter";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    engineType = "ACV";
                    softwareID = "074906021M";
                    break;
                case "0281001777":
                    carMake = "Audi";
                    ecuType = "EDC15P-6.3.2";
                    fuelType = "Diesel";
                    break;
                case "0281001778":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021JB";
                    break;
                case "0281001788":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021AE";
                    break;
                case "0281001789":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001796":
                    carMake = "Audi";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021JC";
                    break;
                case "0281001797":
                    carMake = "Audi";
                    ecuType = "EDC15P-6";
                    fuelType = "Diesel";
                    break;
                case "0281001804":
                    carMake = "Seat";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906012A";
                    break;
                case "0281001807":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021JD";
                    break;
                case "0281001828":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001829":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021S";
                    break;
                case "0281001860":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021JG";
                    break;
                case "0281001861":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001867":
                    carMake = "Audi";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281001868":
                    carMake = "Audi";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281001888":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021AQ";
                    break;
                case "0281001893":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "56028704AF";
                    break;
                case "0281001889":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001908":
                    carMake = "Audi";
                    ecuType = "EDC15P-6";
                    fuelType = "Diesel";
                    break;
                case "0281001909":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6";
                    fuelType = "Diesel";
                    break;
                case "0281001910":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    softwareID = "038906019H";
                    engineType = "AJM";
                    break;
                case "0281001917":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.4";
                    fuelType = "Diesel";
                    break;
                case "0281001919":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001929":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0281001939":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6";
                    fuelType = "Diesel";
                    break;
                case "0281001940":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019";
                    engineType = "AMF";
                    break;
                case "0281001951":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001952":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021AR";
                    break;
                case "0281001967":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021JJ";
                    break;
                case "0281001968":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "028906021JK";
                    break;
                case "0281001979":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    engineType = "ALH";
                    softwareID = "038906012M";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281001980":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281001981":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281005004":
                    carMake = "Volkswagen";
                    ecuType = "DDS1";
                    fuelType = "Diesel";
                    break;
                case "0281010004":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010005":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8D1907401";
                    engineType = "AKN";
                    break;
                case "0281010010":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010011":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281010058":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010059":
                    carMake = "Seat";
                    carType = "Toledo";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012S";
                    engineType = "AGR";
                    break;
                case "0281010060":
                case "0281001060":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    carType = "Toledo";
                    engineType = "AFN";
                    softwareID = "038906012T";
                    break;
                case "0281010061":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010062":
                    carMake = "Seat";
                    carType = "Leon";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012AT";
                    engineType = "ASV";
                    break;
                case "0281010078":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010079":
                    carMake = "Volkswagen";
                    carType = "T4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "074906018D";
                    engineType = "AHY";
                    break;
                case "0281010080":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010081":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010082":
                    carMake = "Volkswagen";
                    carType = "T4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "074906018C";
                    engineType = "ACV";
                    break;
                case "0281010083":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010084":
                    carMake = "Volkswagen";
                    carType = "T4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "074906018B";
                    engineType = "AJT";
                    break;
                case "0281010085":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281010086":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021AK";
                    break;
                case "0281010087":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021AN";
                    break;
                case "0281010088":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    softwareID = "074906021AG";
                    break;
                case "0281010089":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281010091":
                    //VW golf 1.9TDI AUY 115HP 0281010091 038906019AM EDC15P63
                    carMake = "Volkswagen";
                    carType = "Golf/Passat";
                    engineType = "AUY";
                    ecuType = "EDC15P-6.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019AM";
                    break;
                case "0281010095":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4B1907401A";
                    carType = "A6";
                    engineType = "AKN";
                    break;
                case "0281001772":
                    carMake = "Audi";
                    carType = "A6";
                    softwareID = "4B0907401J";
                    fuelType = "Diesel";
                    engineType = "AFB";
                    ecuType = "EDC15VM";
                    break;
                case "0281010096":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4B2907401";
                    engineType = "AKE";
                    break;
                case "0281010097":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010098":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4B2907401B";
                    engineType = "AKE";
                    break;
                case "0281010099":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010100":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8D1907401A";
                    engineType = "AYM";
                    break;
                case "0281001811":
                    carMake = "Audi";
                    carType = "A4";
                    fuelType = "Diesel";
                    ecuType = "EDC15M-4.7";
                    softwareID = "8D0907401";
                    engineType = "AFB";
                    break;
                case "0281010101":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010102":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010104":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012J";
                    engineType = "AQM";
                    break;
                case "0281010105":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010106":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010107":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010108":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010109":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "AGD";
                    softwareID = "038906012AR";
                    break;
                case "0281010110":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010111":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    carType = "Golf";
                    engineType = "AGR";
                    softwareID = "038906012K";
                    break;
                case "0281010112":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15VM+V";
                    engineType = "AHF";
                    softwareID = "038906012EL";
                    fuelType = "Diesel";
                    break;
                case "0281010120":
                    carMake = "Audi";
                    carType = "A3";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012A";
                    engineType = "AHF";
                    break;
                case "0281010121":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010122":
                    carMake = "Audi";
                    carType = "A3";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "ALH";
                    softwareID = "038906012C";
                    break;
                case "0281010123":
                    carMake = "Audi";
                    carType = "A3";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "ASV";
                    softwareID = "038906012BB";
                    break;
                case "0281010124":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010125":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010126":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "ASV";
                    softwareID = "038906012AP";
                    break;
                case "0281010127":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010128":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010129":
                    carMake = "Skoda";
                    carType = "Octavia";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012H";
                    engineType = "AGR";
                    break;
                case "0281010130":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010131":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010132":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010133":
                    carMake = "Volkswagen";
                    carType = "Beetle";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012Q";
                    engineType = "ALH";
                    break;
                case "0281010160":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010174":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010175":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6";
                    fuelType = "Diesel";
                    break;
                case "0281010176":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "EDC15P-6";
                    fuelType = "Diesel";
                    softwareID = "038906019BJ";
                    engineType = "AJM";
                    break;
                case "0281010179":
                    carMake = "Volkswagen";
                    carType = "Lupo";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012BE";
                    engineType = "AKU";
                    break;
                case "0281010183":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010184":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010185":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010196":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010197":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "AHU";
                    softwareID = "038906012AF";
                    break;
                case "0281010229":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "AHF";

                    break;
                case "0281010198":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010199":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010200":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "AVG";
                    softwareID = "038906012AH";
                    break;
                case "0281010201":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012BM";
                    engineType = "AVG";
                    break;
                case "0281010202":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010203":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012AB";
                    engineType = "AVG";
                    break;
                case "0281010204":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "AVG";
                    softwareID = "038906012BN";
                    break;
                case "0281010207":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "1037354429";
                    engineType = "AKE";
                    break;
                case "0281010228":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                /*case "0281010229":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;*/
                case "0281010230":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010240":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0281010247":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010256":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010257":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010258":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.4.2";
                    fuelType = "Diesel";
                    break;
                case "0281010259":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019S";
                    engineType = "AMF";
                    break;
                case "0281010266":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281010322":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281010323":
                    carMake = "Volkswagen";
                    ecuType = "MSA15.5";
                    fuelType = "Diesel";
                    break;
                case "0281010327":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010328":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010329":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010330":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010331":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010372":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010373":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010375":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010376":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010377":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010378":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010379":
                    carMake = "Volkswagen";
                    carType = "Lupo";
                    ecuType = "EDC15VM+V";
                    softwareID = "038906012CM";
                    fuelType = "Diesel";
                    engineType = "AKU";
                    break;
                case "0281010380":
                    carMake = "Skoda";
                    carType = "Octavia";
                    ecuType = "EDC15VM+V";
                    engineType = "ASV";
                    fuelType = "Diesel";
                    softwareID = "038906012CL";
                    break;
                case "0281010381":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010382":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010383":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010384":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010385":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010386":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010387":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010388":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010389":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010393":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010394":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    engineType = "AKE";
                    softwareID = "4B2907401E";
                    break;
                case "0281010395":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4B1907401";
                    engineType = "AKE";
                    break;
                case "0281010404":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281010405":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    carType = "A6";
                    engineType = "AWX";
                    softwareID = "038906019CF";
                    break;
                case "0281010407":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010408":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010443":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4B2907401F";
                    engineType = "AKE";
                    break;
                case "0281010444":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010445":
                    carMake = "Audi";
                    carType = "A4";
                    engineType = "AKE";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4Z7907401A";
                    break;
                case "0281010446":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8E0907401B";
                    engineType = "AKE";
                    break;
                case "0281010447":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010458":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010459":
                    carMake = "Volkswagen";
                    carType = "T4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "074906018AL";
                    engineType = "ACV";
                    break;
                case "0281010460":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010461":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010462":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010465":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010466":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010467":
                    carMake = "Audi";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010469":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019DE";
                    engineType = "ATD";
                    break;
                case "0281010490":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010492":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8E0907401";
                    engineType = "AYM";
                    break;
                case "0281010493":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8E0907401C"; // 180hp
                    engineType = "AKE";
                    break;
                case "0281010494":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    engineType = "AYM";

                    break;
                case "0281010495":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281010503":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "AMF";
                    softwareID = "045906019AB";
                    break;
                case "0281001230":
                    carMake = "Seat";
                    carType = "Toledo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906012CA";
                    engineType = "ALH";
                    break;
                case "0281010505":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281010513":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281010515":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010516":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010545":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281010586":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010587":
                    carMake = "Volkswagen";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281010588":
                    carMake = "Volkswagen";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281010631":
                    carMake = "Volkswagen";
                    carType = "T4";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "074906018BG";
                    engineType = "ACV";
                    break;
                case "0281010638":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010639":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010642":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010643":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010644":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010645":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010646":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010647":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010648":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010649":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010650":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012FA";
                    engineType = "ALH";
                    break;
                case "0281010651":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012FB";
                    engineType = "ASV";
                    break;
                case "0281010652":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010653":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010654":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010655":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010656":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010657":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010658":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010659":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010660":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010670":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019FH";
                    engineType = "AXR";
                    break;
                case "0281010676":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010677":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010678":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010679":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010680":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010681":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010682":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010683":
                    carMake = "Seat";
                    carType = "Leon";
                    engineType = "ASV";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    softwareID = "038906012FK";
                    break;
                case "0281010684":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010685":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010686":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010688":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010689":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010690":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010710":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010711":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010712":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010713":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010714":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010715":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010733":
                    carMake = "Volkswagen";
                    ecuType = "EDC16U1";
                    fuelType = "Diesel";
                    break;
                case "0281011672":
                    carMake = "Volkswagen";
                    //ecuType = "EDC16U1";
                    carMake = "Golf";
                    ecuType = "EDC16U34";
                    fuelType = "Diesel";
                    engineType = "BKD";
                    softwareID = "03G906016AP";
                    break;
                case "03G906021AN":
                case "0281012085":
                    carMake = "Volkswagen";
                    carMake = "Golf";
                    ecuType = "EDC16U34";
                    engineType = "BKC";
                    fuelType = "Diesel";
                    softwareID = "03G906021AN";
                    break;
                case "03G906021CG":
                case "0281014042":
                    carMake = "Volkswagen";
                    carMake = "Touran";
                    ecuType = "EDC16U34";
                    //engineType = "LUHA";
                    fuelType = "Diesel";
                    softwareID = "03G906021CG";
                    break;
                case "0281010736":
                    carMake = "Volkswagen";
                    ecuType = "EDC16U1";
                    fuelType = "Diesel";
                    break;
                case "0281010737":
                    carMake = "Volkswagen";
                    ecuType = "EDC16U1-5.2";
                    fuelType = "Diesel";
                    break;
                case "0281010812":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    carType = "A6";
                    engineType = "AVF";
                    softwareID = "038906019GF";
                    break;
                case "0281010822":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    engineType = "AKE"; // V6 2.5 Tdi
                    carType = "A6";
                    softwareID = "4B2907401J";
                    // engine type
                    break;
                case "0281010823":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    engineType = "AKE";
                    softwareID = "8E0907401D";
                    break;
                case "0281010841":
                    carMake = "Volkswagen";
                    ecuType = "EDC16U1";
                    fuelType = "Diesel";
                    break;
                case "0281010861":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010862":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010863":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010864":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010897":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "4Z7907401B";
                    engineType = "AKE";
                    break;
                case "0281010900":
                    carMake = "Audi";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281010901":
                    carMake = "Audi";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281010905":
                    carMake = "Skoda";
                    carType = "Superb";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "3U0907401";
                    engineType = "AYM";
                    break;
                case "0281010928":
                    carMake = "Skoda";
                    carType = "Superb";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019EG";
                    engineType = "AWX";
                    break;
                case "0281010963":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010964":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010965":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281010974":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "AXR";
                    softwareID = "038906019AT";
                    break;
                case "0281010982":
                    carMake = "Ford";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011017":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011018":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011019":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011027":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019JN";
                    break;
                case "0281011028":
                    carMake = "Volkswagen";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281011029":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011034":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011035":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011046":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011074":
                    carMake = "Seat";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011075":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019JK";
                    engineType = "6Q0";
                    break;
                case "0281011076":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011077":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019DL";
                    break;
                case "0281011100":
                    carMake = "Audi";
                    carType = "A2";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019AT";
                    engineType = "AMF";
                    break;
                case "0281011128":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011129":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011130":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011135":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    engineType = "BFC";
                    carType = "A4";
                    softwareID = "8E0907401Q";
                    break;
                case "0281011136":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8E0907401P";
                    carType = "A6";
                    engineType = "BFC";
                    break;
                case "0281011138":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "AVF";
                    softwareID = "038906019JT";
                    break;
                case "0281011139":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011140":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011141":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011142":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    carType = "A4";
                    engineType = "AWX";
                    softwareID = "038906019JQ";
                    break;
                case "0281011143":
                    carMake = "FORD";
                    ecuType = "EDC15P+22.3.2";
                    fuelType = "Diesel";
                    softwareID = "038906019LR";
                    break;
                case "0281011190":
                    carMake = "Ford";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011199":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019BH";
                    break;
                case "0281011200":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011202":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011203":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011204":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    engineType = "AVB";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019KC";
                    break;
                case "0281011205":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    carType = "Passat";
                    engineType = "AVF";
                    softwareID = "038906019KD";
                    break;
                case "0281011208":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011210":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    carType = "A4";
                    engineType = "AVB";
                    softwareID = "038906019LF";
                    break;
                case "0281011213":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019LM";
                    break;
                case "0281011214":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "AWX";
                    softwareID = "038906019LL";
                    break;
                case "0281011216":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019KJ";
                    engineType = "ASZ";
                    break;
                case "0281011219":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019KT";
                    break;
                case "0281011222":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019LJ";
                    break;
                case "0281011239":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011240":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011241":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019BF";
                    engineType = "AMF";
                    break;
                case "0281011242":
                    carMake = "Volkswagen";
                    carType = "Polo";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019BH";
                    engineType = "BAY";
                    break;
                case "0281011244":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019BG";
                    engineType = "AMF";
                    break;
                case "0281011245":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011255":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281011294":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011309":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "ALH";
                    softwareID = "038906012HA";
                    break;
                case "0281011311":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011312":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011313":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011314":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "ALH";
                    softwareID = "038906012HF";
                    break;
                case "0281011315":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011316":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011319":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011320":
                    carMake = "Seat";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011321":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011322":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011335":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011336":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011337":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011365":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011385":  // 2.5 TDi
                    carMake = "Volkswagen";
                    carType = "LT35";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "AVR";
                    softwareID = "074906018BK";
                    break;
                case "0281011386":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281011387":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281011388":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281011404":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011412":
                    carMake = "Skoda";
                    carType = "Fabia";
                    engineType = "AMF";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019BM";

                    break;
                case "0281011413":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019MT";
                    break;
                case "0281011419":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011435":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281011444":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "E0907401T";
                    engineType = "BCZ";
                    break;
                case "0281011526":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281011607":
                    carMake = "Audi";
                    carType = "A2";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "AMF";
                    softwareID = "045906019F";
                    break;
                case "0281011613":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011614":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038998019A";
                    break;
                case "0281011615":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011652":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011660":
                    carMake = "Volkswagen";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281011720":
                    carMake = "Seat";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011721":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011726":
                    carMake = "Volkswagen";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281011727":
                    carMake = "Volkswagen";
                    ecuType = "EDC15C4";
                    fuelType = "Diesel";
                    break;
                case "0281011764":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011787":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011821":
                    carMake = "Ford";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019NB";
                    break;
                case "0281011822":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019NA";
                    break;
                case "0281011823":
                    carMake = "Seat";
                    carType = "Ibiza";
                    engineType = "BLT";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019NJ";
                    break;
                case "0281011824":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    engineType = "ATD";
                    softwareID = "038906019NE";
                    break;
                case "0281011825":
                    carMake = "Seat";
                    carType = "Ibiza";
                    engineType = "ASZ";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019NF";
                    break;
                case "0281011852":
                    carMake = "Seat";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    carType = "Ibiza";
                    engineType = "BPX";
                    softwareID = "038906019MS";
                    break;
                case "0281011944":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281011948":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011996":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281011997":
                    carMake = "Seat";
                    carType = "Leon";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    engineType = "ASV";
                    softwareID = "038906012JJ";
                    break;
                case "0281012135":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281012142":
                    carMake = "Audi";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    softwareID = "8E0907401AF";
                    engineType = "BCZ";
                    break;
                case "0281012187":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012209":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012210":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012240":
                    carMake = "Audi";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012276":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "038906019NL";
                    engineType = "AXR";
                    break;
                case "0281012318":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012319":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "45906019BS";
                    engineType = "BNM";
                    break;
                case "0281012321":
                    carMake = "Seat";
                    carType = "Ibiza";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019BT";
                    engineType = "BNV";
                    break;
                case "0281012391":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012475":
                    carMake = "Ford";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012642":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281012643":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281012708":
                    carMake = "Skoda";
                    carType = "Fabia";
                    engineType = "BNV";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019BQ";
                    break;
                case "0281012749":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281012750":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281013081":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281013151":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281013153":
                    carMake = "Skoda";
                    ecuType = "EDC15VM+M";
                    fuelType = "Diesel";
                    break;
                case "0281013179":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    softwareID = "045906019CE";
                    engineType = "BNM";
                    carType = "Fabia";
                    break;
                case "0281013180":
                    carMake = "Skoda";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281013639":
                    carMake = "Volkswagen";
                    ecuType = "EDC15VM+V";
                    fuelType = "Diesel";
                    break;
                case "0281014102":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P++";
                    fuelType = "Diesel";
                    break;
                case "0281208189":
                    carMake = "Volkswagen";
                    ecuType = "ME7.1.1";
                    fuelType = "Diesel";
                    break;
                case "0986261113":
                    carMake = "Audi";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0986261114":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0986261115":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0986261137":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0986261139":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261142":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261149":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261150":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261151":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261152":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986261154":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261163":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261164":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261165":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261176":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986261188":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986261261":
                    carMake = "Skoda";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262353":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23";
                    fuelType = "Benzin";
                    break;
                case "0986262355":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0986262356":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0986262357":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262358":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262359":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262386":
                    carMake = "Audi";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262391":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262394":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262395":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262403":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986262405":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262406":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262407":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262413":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262414":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262415":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262421":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262422":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262423":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262424":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262439":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262440":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262442":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986262443":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986262447":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262451":
                    carMake = "Skoda";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262454":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262459":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262460":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986262463":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262468":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262470":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262475":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262478":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262489":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986262490":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262492":
                    carMake = "Skoda";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262493":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986262494":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262498":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262517":
                    carMake = "Seat";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986262518":
                    carMake = "Volkswagen";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986262519":
                    carMake = "Audi";
                    ecuType = "M3-2";
                    fuelType = "Benzin";
                    break;
                case "0986262523":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986262528":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262531":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986262536":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262537":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986262541":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262543":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262544":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262545":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262548":
                    carMake = "Seat";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262552":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262553":
                    carMake = "Volkswagen";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0986262554":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986262555":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986262556":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262557":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262560":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262561":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262562":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262563":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262564":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262565":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262566":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262567":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262568":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262569":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262573":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986262574":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262575":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262576":
                    carMake = "Seat";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262577":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262578":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262579":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262580":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262582":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262583":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262584":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262585":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262586":
                    carMake = "Volkswagen";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262587":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262588":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262589":
                    carMake = "Skoda";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262591":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986262592":
                    carMake = "Seat";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262593":
                    carMake = "Seat";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986262617":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0986263004":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0986263010":
                    carMake = "Seat";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986263047":
                    carMake = "Volkswagen";
                    ecuType = "MA1-23R";
                    fuelType = "Benzin";
                    break;
                case "0986263200":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0986263206":
                    carMake = "Audi";
                    ecuType = "M3-21";
                    fuelType = "Benzin";
                    break;
                case "0986263208":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0986263209":
                    carMake = "Volkswagen";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0986263210":
                    carMake = "Audi";
                    ecuType = "M3-21";
                    fuelType = "Benzin";
                    break;
                case "0986263211":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263212":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263213":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0986263214":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263215":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263216":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986263217":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986263218":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0986263219":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0986263220":
                    carMake = "Audi";
                    ecuType = "M3-85";
                    fuelType = "Benzin";
                    break;
                case "0986263221":
                    carMake = "Audi";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0986263222":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263223":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263224":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0986263225":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0986263226":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263227":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986263228":
                    carMake = "Audi";
                    ecuType = "M3-84";
                    fuelType = "Benzin";
                    break;
                case "0986263229":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986263230":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0986263231":
                    carMake = "Volkswagen";
                    ecuType = "MP9-0";
                    fuelType = "Benzin";
                    break;
                case "0986263234":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0986263235":
                    carMake = "Seat";
                    ecuType = "MA1-3";
                    fuelType = "Benzin";
                    break;
                case "0986263238":
                    carMake = "Audi";
                    ecuType = "M5-4";
                    fuelType = "Benzin";
                    break;
                case "0986263254":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0986264000":
                    carMake = "Volkswagen";
                    ecuType = "MA1-22";
                    fuelType = "Benzin";
                    break;
                case "0986264392":
                    carMake = "Audi";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986264430":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0986264510":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264511":
                    carMake = "Volkswagen";
                    ecuType = "M5-9";
                    fuelType = "Benzin";
                    break;
                case "0986264532":
                    carMake = "Audi";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0986264545":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264548":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264549":
                    carMake = "Volkswagen";
                    ecuType = "M3-82";
                    fuelType = "Benzin";
                    break;
                case "0986264573":
                    carMake = "Volkswagen";
                    ecuType = "M3-83";
                    fuelType = "Benzin";
                    break;
                case "0986264578":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264600":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264605":
                    carMake = "Volkswagen";
                    ecuType = "M3-81";
                    fuelType = "Benzin";
                    break;
                case "0986264606":
                    carMake = "Volkswagen";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264608":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264609":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264610":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264613":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264614":
                    carMake = "Audi";
                    ecuType = "M5-41";
                    fuelType = "Benzin";
                    break;
                case "0986264615":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264616":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264617":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264618":
                    carMake = "Volkswagen";
                    ecuType = "M5-92";
                    fuelType = "Benzin";
                    break;
                case "0986264641":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986264656":
                    carMake = "Audi";
                    ecuType = "ME7-1";
                    fuelType = "Benzin";
                    break;
                case "0986282348":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0986282349":
                    carMake = "Audi";
                    ecuType = "EDC15P-6.3.2";
                    fuelType = "Diesel";
                    break;
                case "0986282351":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0986282352":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0986282354":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.2";
                    fuelType = "Diesel";
                    break;
                case "0986282355":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.3.1";
                    fuelType = "Diesel";
                    break;
                case "0986282356":
                    carMake = "Volkswagen";
                    ecuType = "EDC15P-6.4.2";
                    fuelType = "Diesel";
                    break;
                case "0261S01002":
                    carMake = "Volkswagen";
                    ecuType = "MED7-510";
                    fuelType = "Benzin";
                    break;
                case "0261S01003":
                    carMake = "Volkswagen";
                    ecuType = "MED7-510";
                    fuelType = "Benzin";
                    break;
                case "0261S01004":
                    carMake = "Volkswagen";
                    ecuType = "MED7-511";
                    fuelType = "Benzin";
                    break;
                case "0261S01006":
                    carMake = "Audi";
                    ecuType = "MED7.1.1";
                    fuelType = "Benzin";
                    break;
                case "0261S01009":
                    carMake = "Audi";
                    ecuType = "MED7-511";
                    fuelType = "Benzin";
                    break;
                case "0261S01011":
                    carMake = "Volkswagen";
                    ecuType = "MED7-511";
                    fuelType = "Benzin";
                    break;
                case "0261S01051":
                    carMake = "Volkswagen";
                    ecuType = "MED7-511";
                    fuelType = "Benzin";
                    break;
                case "0261S04104":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04105":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04106":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04107":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04181":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04183":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04184":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04185":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04186":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04187":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04188":
                    carMake = "Seat";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0261S04219":
                    carMake = "Volkswagen";
                    ecuType = "ME7-520";
                    fuelType = "Benzin";
                    break;
                case "0281011883":
                    ecuType = "EDC16";
                    carMake = "Skoda";
                    carType = "Octavia";
                    softwareID = "03G906016DJ";
                    fuelType = "Diesel";
                    engineType = "BJB";
                    break;
                case "0281001617":
                    ecuType = "EDC15M";
                    carMake = "SAAB";
                    carType = "9-3";
                    fuelType = "Diesel";
                    engineType = "2.2 TiD";
                    retval.HP = 115;
                    retval.TQ = 260;
                    break;
                case "0281011970":
                    ecuType = "EDC16C9";
                    carMake = "SAAB";
                    carType = "9-3";
                    fuelType = "Diesel";
                    engineType = "Z19DTH";
                    softwareID = "55559425";
                    break;
                case "0281001421":
                case "0281001422":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    carType = "Golf";
                    fuelType = "Diesel";
                    softwareID = "028906021BF";
                    engineType = "1Z";
                    retval.HP = 90;
                    break;
                case "0281001425":
                case "0281001426":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    carType = "Golf";
                    fuelType = "Diesel";
                    engineType = "AFN";
                    softwareID = "028906021CE";
                    break;
                case "0281001438":
                case "0281001439":
                    ecuType = "MSA15.5";
                    carMake = "Audi";
                    carType = "A4";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    softwareID = "028906021BD";
                    break;
                case "0281001452":
                case "0281001453":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "074906021E";
                    break;
                case "0281001472":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021DF";
                    break;
                case "0281001483":
                case "0281001484":
                    ecuType = "MSA15.5";
                    carMake = "Seat";
                    fuelType = "Diesel";
                    engineType = "AHU"; 
                    softwareID = "028906021DK";
                    break;
                case "0281001486":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021DF";
                    break;
                case "0281001514":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "56028705";
                    break;
                case "0281001528":
                case "0281001529":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021CG";
                    break;
                case "0281001530":
                case "0281001531":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    fuelType = "Diesel";
                    softwareID = "028906021ES";
                    engineType = "1Z";
                    break;
                case "0281001538":
                case "0281001539":
                    ecuType = "MSA15.5";
                    carMake = "Audi";
                    fuelType = "Diesel";
                    softwareID = "028906021FL";
                    break;
                case "0281001550":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "23710-7F405";
                    break;
                case "0281001319":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    engineType = "1Z";
                    softwareID = "028906021AQ";
                    break;
                case "0281001553":
                case "0281001554":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021EM";
                    break;
                case "0281001555":
                case "0281001556":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021DT";
                    engineType = "AFN";
                    break;
                case "0281001566":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021FC";
                    break;
                case "0281001577":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021FE";
                    break;
                case "0281001593":
                case "0281001594":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "074906021Q";
                    break;
                case "0281001597":
                case "0281001598":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "074906021P";
                    break;
                case "0281001600":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "074906021J";
                    break;
                case "0281001604":
                case "0281001605":
                    ecuType = "MSA15.5";
                    carMake = "Seat";
                    fuelType = "Diesel";
                    softwareID = "028906021FN";
                    break;
                case "0281001629":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "23710-78750";
                    break;
                case "0281001639":
                case "0281001640":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "074906021L";
                    break;
                case "0281001649":
                case "0281001650":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021GG";
                    break;
                case "0281001653":
                case "0281001654":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    carType = "Passat";
                    fuelType = "Diesel";
                    engineType = "AHU";
                    softwareID = "028906021GK";
                    break;
                case "0281001655":
                case "0281001656":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021GL";
                    break;
                case "0281001657":
                case "0281001658":
                    ecuType = "MSA15.5";
                    carMake = "Audi";
                    fuelType = "Diesel";
                    softwareID = "028906021GM";
                    break;
                case "0281001659":
                case "0281001660":
                    ecuType = "MSA15.5";
                    carMake = "Audi";
                    carType = "A4";
                    fuelType = "Diesel";
                    softwareID = "028906021GN";
                    engineType = "AFN";
                    break;
                case "0281001662":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021GP";
                    break;
                case "0281001664":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021CS";
                    break;
                case "0281001701":
                    ecuType = "MSA15.5";
                    carMake = "Seat";
                    fuelType = "Diesel";
                    softwareID = "028906021H";
                    break;
                case "0281001719":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "56041996AB";
                    break;
                case "0281001735":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    engineType = "AFN";
                    softwareID = "028906021GT";
                    break;
                case "0281001736":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    softwareID = "028906021HB";
                    break;
                case "0281001737":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    fuelType = "Diesel";
                    engineType = "AHU";
                    softwareID = "028906021HC";
                    break;
                case "0281001738":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    fuelType = "Diesel";
                    softwareID = "028906021GS";
                    engineType = "AHU";
                    break;
                case "0281001558":
                    ecuType = "MSA15.5";
                    carMake = "Volkswagen";
                    carType = "Passat";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    softwareID = "028906021EG";
                    break;
                case "0281001492":
                    ecuType = "MSA15.5";
                    carMake = "Opel";
                    fuelType = "Diesel";
                    softwareID = "91154825";
                    break;
                case "0281001493":
                    ecuType = "MSA15.5";
                    carMake = "Opel";
                    fuelType = "Diesel";
                    softwareID = "91154826";
                    break;
                case "0281001335":
                case "0281001638":
                    ecuType = "MSA15.6";
                    carMake = "Opel";
                    fuelType = "Diesel";
                    break;
                case "0281001634":
                    ecuType = "EDC15M";
                    carMake = "Opel";
                    carType = "Vectra";
                    fuelType = "Diesel";
                    engineType = "";
                    break;
                case "0281010440":
                    ecuType = "EDC15C3";
                    carMake = "Volvo";
                    fuelType = "Diesel";
                    softwareID = "8200049957";
                    break;
                case "0281011609":
                    ecuType = "EDC15C3";
                    carMake = "Volvo";
                    fuelType = "Diesel";
                    softwareID = "8200319969";
                    break;
                case "0281010332":
                    ecuType = "EDC15C7";
                    carMake = "Alfa Romeo";
                    fuelType = "Diesel";
                    softwareID = "55185365";
                    break;
                case "0281011491":
                    ecuType = "EDC15C7";
                    carMake = "Alfa Romeo";
                    fuelType = "Diesel";
                    softwareID = "55188343";
                    break;
                case "0281012072":
                    ecuType = "EDC15C7";
                    carMake = "Alfa Romeo";
                    fuelType = "Diesel";
                    break;
                case "0281010293":
                    ecuType = "EDC15C5";
                    carMake = "Crysler";
                    fuelType = "Diesel";
                    softwareID = "P56044356AC";
                    break;
                case "0281010814":
                    ecuType = "EDC15C5";
                    carMake = "Crysler";
                    fuelType = "Diesel";
                    softwareID = "PO4727664AE";
                    break;
                case "0281011062":
                    ecuType = "EDC15C5";
                    carMake = "Crysler";
                    fuelType = "Diesel";
                    softwareID = "P56044220AF";
                    break;
                case "0281011409":
                    ecuType = "EDC15C5";
                    carMake = "Crysler";
                    fuelType = "Diesel";
                    softwareID = "P56044679AA";
                    break;
                case "0281001830":
                case "0281010144":
                    ecuType = "EDC15C4";
                    carMake = "BMW";
                    fuelType = "Diesel";
                    break;
                case "0281010314":
                    ecuType = "EDC15C4";
                    carMake = "BMW";
                    fuelType = "Diesel";
                    softwareID = "13617786581";
                    break;
                case "0281011085":
                    ecuType = "EDC15C4";
                    carMake = "BMW";
                    fuelType = "Diesel";
                    softwareID = "7791547";
                    break;
                case "0281011257":
                    ecuType = "EDC15C4";
                    carMake = "BMW";
                    fuelType = "Diesel";
                    softwareID = "7790220";
                    break;
                case "0281001879":
                    ecuType = "EDC15C";
                    carMake = "Fiat";
                    fuelType = "Diesel";
                    softwareID = "46479993";
                    break;
                case "0281001928":
                    ecuType = "EDC15C";
                    carMake = "Fiat";
                    fuelType = "Diesel";
                    softwareID = "46546205";
                    break;
                case "0281001936":
                    ecuType = "EDC15C";
                    carMake = "Fiat";
                    fuelType = "Diesel";
                    softwareID = "46753569";
                    break;
                case "0281010022":
                    ecuType = "EDC15C";
                    carMake = "Fiat";
                    fuelType = "Diesel";
                    softwareID = "46774779";
                    break;
                case "0281001487":
                case "0281001955":
                case "0281010333":
                case "0281010334":
                case "0281010337":
                case "0281010341":
                case "0281010336":
                case "0281010342":
                case "0281010344":
                case "0281010454":
                case "0281010484":
                case "0281010485":
                case "0281010486":
                case "0281010488":
                case "0281010489":
                case "0281010738":
                case "0281010739":
                case "0281010740":
                case "0281010741":
                case "0281010743":
                case "0281010846":
                case "0281010929":
                case "0281010930":
                case "0281010931":
                case "0281010932":
                case "0281011047":
                case "0281011396":
                case "0281011420":
                case "0281011499":
                case "0281011505":
                case "0281011552":
                case "0281011553":
                case "0281011557":
                case "0281012597":
                    ecuType = "EDC15C7";
                    carMake = "Fiat";
                    fuelType = "Diesel";
                    break;
                case "0281011087":
                    ecuType = "EDC15C347";
                    carMake = "Volvo";
                    carType = "V40/S40";
                    softwareID = "2920918251234";
                    fuelType = "Diesel";
                    engineType = "1.9 TDI";
                    break;
                case "0281011892":
                    carMake = "Audi";
                    carType = "A3";
                    engineType = "BKD";
                    softwareID = "03G906016DT";
                    ecuType = "EDC16U1";
                    break;
                case "0281001092":
                    carMake = "Audi";
                    carType = "100";
                    engineType = "1T";
                    ecuType = "MSA6";
                    fuelType = "Diesel";
                    softwareID = "443907401A";
                    break;
                case "0281001185":
                case "0281001186":
                    carMake = "Audi";
                    carType = "80";
                    engineType = "1Z";
                    fuelType = "Diesel";
                    //ecuType = "MSA6";
                    softwareID = "8A0907401B";
                    break;
                case "0281011832":
                    carMake = "Audi";
                    carType = "A3";
                    engineType = "BKC";
                    ecuType = "EDC16U1";
                    fuelType = "Diesel";
                    softwareID = "03G906016CC";
                    break;
                case "0281010158":
                    carMake = "Audi";
                    carType = "A4";
                    engineType = "AKN";
                    ecuType = "EDC15M";
                    fuelType = "Diesel";
                    softwareID = "8D0907401P";
                    break;
                case "0281012533":
                    carMake = "Opel";
                    carType = "Vectra";
                    engineType = "DTH";
                    ecuType = "EDC16C9";
                    fuelType = "Diesel";
                    softwareID = "1037379019";
                    break;
                case "0281011914":
                    carMake = "Opel";
                    carType = "Vectra";
                    engineType = "";
                    ecuType = "EDC16C9";
                    fuelType = "Diesel";
                    softwareID = "1037379357";
                    break;
                case "0281015212":
                    carMake = "SAAB";
                    carType = "9-3";
                    engineType = "Z19DTR";
                    ecuType = "EDC16C39";
                    fuelType = "Diesel";
                    softwareID = "55568896";
                    break;
                case "0281012247":
                    carMake = "SAAB";
                    carType = "9-3";
                    engineType = "Z19DTH";
                    ecuType = "EDC16C9";
                    fuelType = "Diesel";
                    softwareID = "55559129";
                    break;
                case "EDC174L0907401A":
                    carMake = "Audi";
                    carType = "Q7";
                    engineType = "CASA"; // 176kw/236hp
                    ecuType = "EDC17";
                    fuelType = "Diesel";
                    break;
                case "ECM20TDI03L906022NH":
                    carMake = "Audi";
                    carType = "Q5";
                    ecuType = "EDC17";
                    fuelType = "Diesel";
                    engineType = "CAHA";
                    break;
                case "ECM20TDI03L906022MK":
                    carMake = "Audi";
                    carType = "A5";
                    ecuType = "EDC17";
                    fuelType = "Diesel";
                    engineType = "CAGA";
                    break;
                case "ECM20TDI03L906022JM":
                    carMake = "Audi";
                    carType = "A4";
                    ecuType = "EDC17";
                    fuelType = "Diesel";
                    engineType = "CAGA";
                    break;
                case "0281001945":
                    carMake = "Audi";
                    ecuType = "EDC15M";
                    engineType = "2.5 TDI";
                    fuelType = "Diesel";
                    softwareID = "8D0907401F";
                    break;
                case "0281001930":
                    carMake = "Audi";
                    ecuType = "EDC15M";
                    engineType = "2.5 TDI";
                    softwareID = "4D0907401H";
                    fuelType = "Diesel";
                    break;
                case "0281001999":
                    ecuType = "EDC15C";
                    carMake = "Renault";
                    carType = "Espace";
                    fuelType = "Diesel";
                    retval.HP = 130;
                    break;
                case "074906032AT":
                    ecuType = "EDC16";
                    carMake = "Volkswagen";
                    carType = "Crafter";
                    fuelType = "Diesel";
                    engineType = "BJK";
                    break;
                case "0281011083":
                    carMake = "Peugeot";
                    carType = "206";
                    fuelType = "Diesel";
                    softwareID = "9648394480";
                    ecuType = "EDC15C2";
                    break;
                case "0281010250":
                    carMake = "Peugeot";
                    carType = "206";
                    fuelType = "Diesel";
                    softwareID = "9637089980";
                    ecuType = "EDC15C2-6.1";
                    break;
                case "0281010500":
                    carMake = "Peugeot";
                    carType = "206";
                    fuelType = "Diesel";
                    softwareID = "9641606980";
                    ecuType = "EDC15C2";
                    break;
                case "0281010161":
                    carMake = "Smart";
                    softwareID = "1037351696";
                    fuelType = "Diesel";
                    ecuType = "EDG15C-51"; // G??
                    break;
                case "0281010567":
                case "0281010632":
                case "0281011529":
                case "0281010819":
                case "0281001934":
                case "0281010483":
                case "0281010189":
                case "0281010637":
                case "0281011254":
                case "0281010297":
                case "0281010556":
                case "0281011324":
                case "0281011101":
                case "0281010248":
                case "0281010166":
                case "0281001977":
                case "0281011446":
                case "0281001782":
                case "0281010162":
                case "0281010252":
                case "0281011360":
                case "0281011081":
                case "0281001866":
                case "0281010779":
                case "0281010747":
                case "0281010249":
                case "0281010362":
                case "0281001976":
                case "0281010594":
                case "0281010358":
                case "0281010137":
                case "0281010251":
                case "0281010338":
                case "0281010339":
                case "0281010954":
                case "0281011283":
                case "0281011579":
                case "0281011279":
                case "0281011694":
                case "0281011260":
                case "0281010368": // < could be EDC15P Audi A4?
                case "0281011522":
                case "0281010764":

                    ecuType = "EDC15C";
                    break;
                case "0281001373":
                    // BMW 525
                    carMake = "BMW";
                    carType = "525";
                    ecuType = "MSA11";
                    fuelType = "Diesel";
                    softwareID = "13612246273";
                    break;
                /*case "0281010732": // EDC16?
                    break;*/
                case "0281001354":
                case "0281001355":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "MSA12";
                    softwareID = "028906021BT";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    break;
                case "0281001173":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "MSA12";
                    softwareID = "028906021B";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    break;
                case "0281001171":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "MSA12"; // 1.1.3 = MSA12
                    softwareID = "028906021C";
                    engineType = "AEY";
                    fuelType = "Diesel";
                    break;
                case "0281001155":
                    carMake = "Volkswagen";
                    carType = "Transporter";
                    ecuType = "MSA15";
                    softwareID = "074906021";
                    engineType = "ACV";
                    fuelType = "Diesel";
                    break;
                case "0281001470":
                    carMake = "Volkswagen";
                    carType = "Transporter";
                    ecuType = "MSA12";
                    softwareID = "074906021F";
                    engineType = "ACV";
                    fuelType = "Diesel";
                    break;
                case "0281001252":
                    carMake = "Volkswagen";
                    carType = "Sharan";
                    ecuType = "MSA12";
                    softwareID = "028906021P";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    break;
                case "0281001313":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "MSA12";
                    engineType = "AHU";
                    fuelType = "Diesel";
                    softwareID = "028906021AK";
                    break;
                case "0281001412":
                    carMake = "Volkswagen";
                    carType = "Passat";
                    ecuType = "MSA12";
                    engineType = "1Z";
                    fuelType = "Diesel";
                    softwareID = "028906021DD";
                    break;
                case "0281001309":
                    carMake = "Volkswagen";
                    carType = "Golf";
                    ecuType = "MSA12";
                    engineType = "1Z";
                    fuelType = "Diesel";
                    softwareID = "028906021AF";
                    break;
                case "0281001321":
                    carMake = "Audi";
                    carType = "A6";
                    ecuType = "MSA12";
                    engineType = "AEL";
                    fuelType = "Diesel";
                    softwareID = "4A0907401P";
                    break;
                case "0281001198":
                    carMake = "Audi";
                    carType = "A4";
                    softwareID = "028906021D";
                    ecuType = "MSA12";
                    engineType = "1Z";
                    fuelType = "Diesel";
                    break;
                case "0281001367":
                    carMake = "Audi";
                    carType = "A4";
                    softwareID = "028906021F";
                    ecuType = "MSA12";
                    engineType = "1Z";
                    fuelType = "Diesel";
                    break;
                case "0281001129":
                    carMake = "Audi";
                    carType = "A6";
                    softwareID = "4A0907401";
                    ecuType = "MSA6";
                    engineType = "";
                    fuelType = "Diesel";
                    break;
                case "0281010340":
                case "0281010335":
                case "0281011488":
                    ecuType = "EDC15C7";
                    break;
                case "0281011649":
                    ecuType = "EDC15C9-7.1";
                    break;
                case "0281001237":
                    carMake = "Alfa Romeo";
                    carType = "164";
                    fuelType = "Diesel";
                    ecuType = "MSA11"; 
                    break;
                case "0281001445":
                    carMake = "BMW";
                    carType = "320";
                    fuelType = "Diesel";
                    softwareID = "7785098";
                    ecuType = "EDC15M-6.1"; // is this MSA11
                    break;
                case "0281001118":
                    carMake = "BMW";
                    carType = "525";
                    ecuType = "MSA11";
                    fuelType = "Diesel";
                    break;
                case "0281001294":
                    carMake = "BMW";
                    carType = "525";
                    ecuType = "MSA11";
                    fuelType = "Diesel";
                    break;
                case "0281001380":
                case "0281001201":
                    carMake = "BMW";
                    carType = "325";
                    ecuType = "MSA11";
                    fuelType = "Diesel";
                    break;
                case "0281001243":
                case "0281001711":
                    carMake = "BMW";
                    carType = "318";
                    ecuType = "MSA11";
                    fuelType = "Diesel";
                    break;
                case "0281001256":
                    carMake = "Audi";
                    carType = "A6";
                    fuelType = "Diesel";
                    softwareID = "4A0907401F";
                    ecuType = "MSA11"; // MSA11 = EDC 1.3.1
                    engineType = "AAT";
                    break;
                case "0281001317":
                    carMake = "Audi";
                    carType = "80";
                    fuelType = "Diesel";
                    softwareID = "028906021AP";
                    ecuType = "MSA12"; // MSA12 = EDC 1.3.3
                    engineType = "1Z";
                    break;
			//case "0281013537": // 118d 143hp!
			case "0281015075": // 120d 177hk
			case "0281013924": // X3?
                case "0281016110": // X1
					carMake = "Bmw";
					carType = "";
					fuelType = "Diesel";
					softwareID = "";
					ecuType = "EDC17CP02/06";
					engineType = "";
					break;
			case "1037396562": // 2008 E91 320d 2Mb!
			case "1037391387": // ???? E92 320d 1.5Mb
			//case "1037390656":
			//case "1037504980": // ?!
			case "1037395778": // 120d
					carMake = "Bmw";
					carType = "";
					fuelType = "Diesel";
					softwareID = partnumber;
					ecuType = "EDC17CP02/06";
					engineType = "";
					break;
/*            case "0281015075": // 120d 177hk
            case "0281013924": // X3?
            case "0281016110": // X1
                    carMake = "Bmw";
                    carType = "";
                    fuelType = "Diesel";
                    softwareID = "";
                    ecuType = "EDC17CP02/06";
                    engineType = "";
                    break;
            case "1037396562": // 2008 E91 320d 2Mb!
            case "1037391387": // ???? E92 320d 1.5Mb
            case "1037395778": // 120d
                    carMake = "Bmw";
                    carType = "";
                    fuelType = "Diesel";
                    softwareID = partnumber;
                    ecuType = "EDC17CP02/06";
                    engineType = "";
                    break;*/
            case "0281013500":
            case "1037387658":
                    carMake = "Bmw";
                    carType = "E90";
                    fuelType = "Diesel";
                    softwareID = partnumber;
                    ecuType = "";
                    engineType = "330D";
                    break;
            case "0281016175":
            case "1037517684":
                    carMake = "Bmw";
                    carType = "E90";
                    fuelType = "Diesel";
                    softwareID = partnumber;
                    ecuType = "EDC17C41";
                    engineType = "320D 184hp";
                    break;
            case "1037396564":
            case "1037389230":
            case "0281013537":
            case "1037395777":
            case "0281014239":
                    carMake = "BMW";
                    carType = "E87/E90";                 // e.g. 118d 143hp!
                    softwareID = "0281013537";
                    ecuType = "EDC17C06";
                    engineType = "N47uL";
                    break;
            case "0281014573":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281014573";
                    ecuType = "EDC17C06";
                    engineType = "N47uL";
                    break;
            case "0281014238":
            case "1037390655":
                    carMake = "BMW";
                    carType = "E92";
                    softwareID = partnumber;
                    ecuType = "EDC17C02";
                    engineType = "N47 320D";
                    break;
            case "0281017593":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281017593";
                    ecuType = "EDC17C06";
                    engineType = "N47uL/N47uuL";
                    break;
            case "0281017551":
                    carMake = "BMW";
                    carType = "E8x/E90";
                    softwareID = "0281017551";
                    ecuType = "EDC17C0";
                    engineType = "N47uL/N47uuL";
                    break;
            // DDE71
            case "1037386030":
            case "1037390656":
            case "0281013776":
                    carMake = "BMW";
                    carType = "E87";
                    softwareID = "0281013776";
                    ecuType = "EDC17CP02";
                    engineType = "N47TL";
                    break;
            case "1037389229":
            case "0281013536":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281013536";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL";
                    break;
            case "1037395781":
            case "0281014572":
                    carMake = "BMW";
                    carType = "E60/E87/E90";
                    softwareID = "0281014572";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL";
                    break;
            case "1037396563":
            case "0281016070":
                    carMake = "BMW";
                    carType = "E87";
                    softwareID = "0281016070";
                    ecuType = "EDC17CP02";
                    engineType = "N47TL";
                    break;
            case "1037399764":
            case "0281016071":
                    carMake = "BMW";
                    carType = "E84/D3";
                    softwareID = "0281016071";
                    ecuType = "EDC17CP02";
                    engineType = "N47TL";
                    break;
            case "1037504298":
            case "0281016924":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281016924";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL";
                    break;
            case "1037504980":
            case "0281017070":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281017070";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL-P-NL";
                    break;
            case "1037509475":
            case "0281017591":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281017591";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL";
                    break;
            case "1037509478":
            case "0281017550":
                    carMake = "BMW";
                    carType = "E84/E87/E90";
                    softwareID = "0281017550";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL";
                    break;
            case "0281017596":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281017596";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL-P";
                    break;
            case "0281017597":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281017597";
                    ecuType = "EDC17CP02";
                    engineType = "N47oL_P";
                    break;
            case "0281017594":
                    carMake = "BMW";
                    carType = "E87";
                    softwareID = "0281017594";
                    ecuType = "EDC17CP02";
                    engineType = "N47TL";
                    break;
            case "1037509479": //E84
            case "0281017552":
                    carMake = "BMW";
                    carType = "E84/E87/D3";
                    softwareID = "0281017552";
                    ecuType = "EDC17CP02";
                    engineType = "N47tL";
                    break;
            //DDE73
            case "0281014726":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281014726";
                    ecuType = "EDC17CP09";
                    engineType = "N57DoL";
                    break;
            case "0281014728":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281014728";
                    ecuType = "EDC17CP09";
                    engineType = "M57TUE2-US";
                    break;
            case "0281017652":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017652";
                    ecuType = "EDC17CP09";
                    engineType = "M57TUE2-US";
                    break;
            case "0281017605":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017605";
                    ecuType = "EDC17CP09";
                    engineType = "M57D30T2-US";
                    break;
            case "0281016838":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281016838";
                    ecuType = "EDC17CP09";
                    engineType = "N57DoL/N57DuL";
                    break;
            case "0281016842":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281016842";
                    ecuType = "EDC17CP09";
                    engineType = "N57DoL/N57DuL";
                    break;
            case "0281016840":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281016840";
                    ecuType = "EDC17CP09";
                    engineType = "N57DoL";
                    break;
            case "0281016844":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281016844";
                    ecuType = "EDC17CP09";
                    engineType = "N57DoL";
                    break;
            case "0281017967":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017967";
                    ecuType = "EDC17CP09";
                    engineType = "M57TUE2-US";
                    break;
            case "0281017487":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017487";
                    ecuType = "EDC17CP09";
                    engineType = "M57D30T2-US";
                    break;
            //DDE603
            case "0281012502":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281012502";
                    ecuType = "EDC16C35";
                    engineType = "M47TUE2uL";
                    break;
            // DDE604
            case "0281012334":
                    carMake = "BMW";
                    carType = "E87/E90";
                    softwareID = "0281012334";
                    ecuType = "EDC16C35";
                    engineType = "M47TUE2oL/D3";
                    break;
            case "0281013501":
                    carMake = "BMW";
                    carType = "E83/E91";
                    softwareID = "0281013501";
                    ecuType = "EDC16C35";
                    engineType = "M47TUE2/M47TUE2oL";
                    break;
            case "0281014575":
                    carMake = "BMW";
                    carType = "E60/E87/E90";
                    softwareID = "0281014575";
                    ecuType = "EDC16C35";
                    engineType = "M47TUE2oL";
                    break;
            //DDE606
            case "0281012994":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281012994";
                    ecuType = "EDC16C35";
                    engineType = "M57TUE2uL";
                    break;
            case "0281016288":
                    carMake = "BMW";
                    carType = "E9x";
                    softwareID = "0281016288";
                    ecuType = "EDC16C35";
                    engineType = "M57TUE2uL";
                    break;
            case "0281014175":
                    carMake = "BMW";
                    carType = "E83/E90";
                    softwareID = "0281014175";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2";
                    break;
            case "0281014205":
                    carMake = "BMW";
                    carType = "E92/E93";
                    softwareID = "0281014205";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2";
                    break;
            case "0281015240":
                    carMake = "BMW";
                    carType = "E70/E9x";
                    softwareID = "0281015240";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2";
                    break;
            case "0281016640":
                    carMake = "BMW";
                    carType = "E70/E90";
                    softwareID = "0281016640";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2TOP";
                    break;
            case "0281011763":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281011763";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2";
                    break;
            case "0281013403":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281013403";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2";
                    break;
            case "0281014206":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281014206";
                    ecuType = "EDC16CP35";
                    engineType = "M57TUE2TOP";
                    break;
            //DDE701
            case "0281017515":
                    carMake = "BMW";
                    carType = "E84";
                    softwareID = "0281017515";
                    ecuType = "EDC17C50";
                    engineType = "N47D20u1";
                    break;
            case "0281017518":
                    carMake = "BMW";
                    carType = "E84";
                    softwareID = "0281017518";
                    ecuType = "EDC17C50";
                    engineType = "N47D20O1";
                    break;
            case "0281017517":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017517";
                    ecuType = "EDC17C50";
                    engineType = "N47D20k1/N47D20u1";
                    break;
            case "0281017520":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017520";
                    ecuType = "EDC17C50";
                    engineType = "N47D20o1";
                    break;
            case "0281017523":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017523";
                    ecuType = "EDC17C50";
                    engineType = "N47D20o1";
                    break;
            case "0281017974":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017974";
                    ecuType = "EDC17C50";
                    engineType = "N47D20o1";
                    break;
            case "0281017848":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017848";
                    ecuType = "EDC17C41";
                    engineType = "N47D20o1";
                    break;
            case "0281017846":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017846";
                    ecuType = "EDC17C41";
                    engineType = "N47D20o1";
                    break;
            case "0281017973":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017973";
                    ecuType = "EDC17C41";
                    engineType = "N47D20o1";
                    break;
            case "0281017847":
                    carMake = "BMW";
                    carType = "E90";
                    softwareID = "0281017847";
                    ecuType = "EDC17C41";
                    engineType = "N47D20u1";
                    break;
            //DDE731
            case "0281B100JF":
                    carMake = "BMW";
                    carType = "E84";
                    softwareID = "0281B100JF";
                    ecuType = "EDC17CP45";
                    engineType = "N47D20T1";
                    break;
                    
			}
            if (ecuType == string.Empty && partnumber.StartsWith("EDC17"))
            {
                ecuType = "EDC17";
            }
            switch (engineType)
            {
                case "Z19DTH":
                    retval.HP = 150;
                    retval.TQ = 320;
                    break;
                case "Z19DTR":
                    retval.HP = 180;
                    retval.TQ = 400;
                    break;
                case "AVR":
                    retval.HP = 110;
                    retval.TQ = 0; //?
                    break;
                case "ACV":
                    retval.HP = 100;
                    retval.TQ = 250; 
                    break;
                case "AYZ":
                case "ANY":
                    //GT1541V 
                    // VW Lupo 1.2l 3L engine
                    retval.HP = 60;
                    retval.TQ = 140;
                    break;
                case "CAHA":
                    retval.HP = 170;
                    retval.TQ = 350;
                    break;
                case "CAGA":
                    retval.HP = 143;
                    retval.TQ = 320;
                    break;
                case "CASA":
                    retval.HP = 236;
                    retval.TQ = 500;
                    break;
                case "1T": //2.5TDI
                    retval.HP = 120;
                    break;
                case "1Z": //VP37
                case "AHU": //VP37
                    retval.HP = 90;
                    retval.TQ = 200;
                    break;
                case "AGR":
                case "AHH":
                case "ALE":
                case "ALH":
                    retval.HP = 90;
                    retval.TQ = 210;
                    break;
                case "AFN":
                case "AHF":
                case "ASV":
                case "AVG":
                    retval.HP = 110;
                    retval.TQ = 235;
                    break;
                case "BSU":
                    retval.HP = 75;
                    retval.TQ = 210;
                    break;
                case "BRU":
                case "BXF":
                case "BXJ":
                    retval.HP = 90;
                    retval.TQ = 210;
                    break;
                case "ANU":
                    retval.HP = 90;
                    retval.TQ = 240;
                    break;
                case "ATD":
                case "AXR":
                case "BEW":
                case "BMT":
                    retval.HP = 100;
                    retval.TQ = 240;
                    break;
                case "AVB":
                case "AVQ":
                    retval.HP = 100;
                    retval.TQ = 250;
                    break;
                case "BSW":
                    retval.HP = 105;
                    retval.TQ = 240;
                    break;
                case "BJB":
                case "BKC":
                case "BLS":
                case "BSV":
                case "BXE":
                    retval.HP = 105;
                    retval.TQ = 250;
                    break;
                case "BPZ":
                    retval.HP = 115;
                    retval.TQ = 250;
                    break;
                case "AJM":
                    retval.HP = 115;
                    retval.TQ = 285;
                    break;
                case "ATJ":
                case "AUY":
                case "BVK":
                    retval.HP = 115;
                    retval.TQ = 310;
                    break;
                case "AWX":
                    retval.HP = 130;
                    retval.TQ = 285;
                    break;
                case "ASZ":
                case "AVF":
                case "BLT":
                    retval.HP = 130;
                    retval.TQ = 310;
                    break;
                case "ARL":
                case "BTB":
                    retval.HP = 150;
                    retval.TQ = 320;
                    break;
                case "BPX":
                case "BUK":
                    retval.HP = 160;
                    retval.TQ = 330;
                    break;
                case "AKE":
                case "BAU":
                case "BDH":
                    retval.HP = 180;
                    retval.TQ = 370;
                    break;
                case "BCZ":
                case "BDG":
                case "BFC":
                    retval.HP = 163;
                    retval.TQ = 310;
                    break;
                case "AYM":
                    retval.HP = 155;
                    retval.TQ = 310;
                    break;
                case "AFB":
                case "AKN":
                    retval.HP = 150;
                    retval.TQ = 310;
                    break;

                    // 1.4 R3 PD
                case "BNM":
                    retval.HP = 70;
                    retval.TQ = 155;
                    break;
                case "AMF":
                case "BAY":
                case "BHC":
                    retval.HP = 75;
                    retval.TQ = 195;
                    break;
                case "BMS":
                case "BNV":
                    retval.HP = 80;
                    retval.TQ = 195;
                    break;
                case "ATL":
                    retval.HP = 90;
                    retval.TQ = 230;
                    break;
                    //1.9D
                case "1Y":
                case "AEF":
                    retval.HP = 65;
                    retval.TQ = 125;
                    break;
                    //1.9 SDI VP37 EDC15V+
                case "BXT":
                    retval.HP = 40;
                    retval.TQ = 130;
                    break;
                case "AEY":
                    retval.HP = 60;
                    retval.TQ = 130;
                    break;
                case "BGM":
                    retval.HP = 40;
                    retval.TQ = 125;
                    break;
                case "BEQ":
                    retval.HP = 45;
                    retval.TQ = 125;
                    break;
                case "BGL":
                    retval.HP = 50;
                    retval.TQ = 125;
                    break;
                case "ANC":
                    retval.HP = 60;
                    retval.TQ = 125;
                    break;
                case "ASY":
                    retval.HP = 64;
                    retval.TQ = 125;
                    break;
                case "AQM":
                    retval.HP = 68;
                    retval.TQ = 133;
                    break;
                //2.0 R4 TDI PD EDC16/EDC17
                case "BKD":
                case "BKP":
                    retval.HP = 140;
                    retval.TQ = 320;
                    break;
                case "BMN":
                case "BMR":
                case "BRD":
                    retval.HP = 170;
                    retval.TQ = 350;
                    break;
                //2.0 R4 16v TDI CR
                case "AAS":
                    retval.HP = 85;
                    retval.TQ = 195;
                    break;
                case "AAT":
                case "ABP":
                    retval.HP = 115;
                    retval.TQ = 265;
                    break;
                case "AEL":
                    retval.HP = 140;
                    retval.TQ = 290;
                    break;
                case "AAN": // petrol
                    retval.HP = 230;
                    retval.TQ = 0;
                    break;



            }
            retval.FuellingType = EngineTypeToFuellingType(engineType);
            if(Tools.Instance.IsEDC16Partnumber(partnumber))
            {
                ecuType = "EDC16";

            }
            if (ecuType == string.Empty && partnumber.StartsWith("028"))
            {
                // might be MSA15 as well, depends on filesize from here on
                // msa15 = 256kB
                if (length == 0x8000)
                {
                    ecuType = "MSA6";
                }
                if (length == 0x10000)
                {
                    ecuType = "MSA15";
                }
                if (length == 0x20000)
                {
                    ecuType = "EDC15M";
                    retval.CarMake = "Opel/SAAB";
                }
                else if (length == 0x40000)
                {
                    ecuType = "EDC15V";
                }
                else if (length == 0x80000)
                {
                    ecuType = "EDC15P";
                }
                else if (length == 0x100000)
                {
                    ecuType = "EDC16";
                }
                else if (length == 0x200000)
                {
                    ecuType = "EDC16";
                }
            }


            retval.FuelType = fuelType;
            retval.EcuType = ecuType;
            retval.CarMake = carMake;
            retval.CarType = carType;
            retval.EngineType = engineType;
            retval.SoftwareID = softwareID;
            return retval;
        }

    }
}
