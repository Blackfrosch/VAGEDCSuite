using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    class VINDecoder
    {
        //WVWZZZ1KZ4B002803
        public VINCarInfo DecodeVINNumber(string vin)
        {
            vin = vin.ToUpper();
            VINCarInfo retval = new VINCarInfo();
            if (vin.Length != 17) return retval;
            retval.PlantInfo = decodePlantInfo(vin);
            retval.Concern = decodeConcern(vin);
            retval.Makeyear = decodeMakeYear(vin);
            string platform = string.Empty;
            string make = string.Empty;
            retval.Model = decodeModel(vin, retval.Makeyear, out platform, out make);
            retval.Platform = platform;
            retval.Make = make;
            retval.EngineType = decodeEngineType(vin);
            retval.Valid = true;

            return retval;
        }

        private string decodeEngineType(string vin)
        {
            string retval = string.Empty;
            switch (vin[5])
            {
                case 'A':
                    retval = "Lowest output gas engine";
                    break;
                case 'B':
                    retval = "2nd lowest output gas engine";
                    break;
                case 'D':
                    retval = "VR6";
                    break;
                case 'F':
                    retval = "2nd lowest output diesel engine (1.6/1.9L TD/TDI)";
                    break;
                case 'G':
                    retval = "Lowest output diesel (1.5/1.6L)";
                    break;

            }
            return retval;
        }

        private string decodeModel(string vin, int my, out string platform, out string make)
        {
            string retval = string.Empty;
            make = string.Empty;
            platform = string.Empty;
            string md = vin.Substring(6, 2);
            switch (md)
            {
                case "12":
                    retval = "Up";
                    make = "Volkswagen";
                    break;
                case "70":
                    retval = "Transporter T3";
                    make = "Volkswagen";
                    break;
                case "7D":
                    retval = "Transporter T4";
                    make = "Volkswagen";
                    break;
                case "7H":
                case "7E":
                case "7F":
                    retval = "Transporter T5";
                    make = "Volkswagen";
                    break;
                case "7L":
                    retval = "Touareg";
                    platform = "PL75";
                    make = "Volkswagen";
                    break;
                case "1T":
                    retval = "Touran";
                    make = "Volkswagen";
                    break;
                case "5N":
                    retval = "Tiguan";
                    make = "Volkswagen";
                    break;
                case "7M":
                case "7N":
                    retval = "Sharan";
                    make = "Volkswagen";
                    break;
                case "13":
                    retval = "Scirocco";
                    make = "Volkswagen";
                    break;
                case "80":
                case "6N":
                case "9N":
                case "6R":
                case "5R":
                    retval = "Polo";
                    make = "Volkswagen";
                    break;
                case "3D":
                    retval = "Phaeton";
                    make = "Volkswagen";
                    platform = "D1";
                    break;
                case "35":
                    retval = "Passat";
                    make = "Volkswagen";
                    break;
                case "3C":
                    retval = "Passat";
                    platform = "B6";
                    make = "Volkswagen";
                    break;
                case "3B":
                    retval = "Passat";
                    platform = "B5";
                    make = "Volkswagen";
                    break;
                case "3A":
                    retval = "Passat";
                    platform = "B4";
                    make = "Volkswagen";
                    break;
                case "31":
                    retval = "Passat";
                    platform = "B3";
                    make = "Volkswagen";
                    break;
                case "15":
                    retval = "New Bora";
                    make = "Volkswagen";
                    break;
                case "5C":
                case "AT":
                    retval = "New Beetle";
                    make = "Volkswagen";
                    break;
                //case "1E":
                case "1V":
                    retval = "New Beetle";
                    platform = "A3";
                    make = "Volkswagen";
                    break;
                case "1C":
                case "1Y":
                    retval = "New Beetle";
                    platform = "A4";
                    make = "Volkswagen";
                    break;
                case "6E":
                case "6X":
                    retval = "Lupo";
                    make = "Volkswagen";
                    break;
                case "5K":
                case "52":
                    retval = "Golf VI";
                    platform = "A6";
                    make = "Volkswagen";
                    break;
                case "16":
                case "AJ":
                    retval = "Jetta VI";
                    platform = "A6";
                    make = "Volkswagen";
                    break;
                case "5M":
                    retval = "Golf V plus";
                    platform = "A5";
                    make = "Volkswagen";
                    break;
                case "1K":
                    retval = "Golf V/Jetta V";
                    platform = "A5";
                    make = "Volkswagen";
                    break;
                case "1J":
                    retval = "Golf IV/Bora/Jetta IV";
                    make = "Volkswagen";
                    platform = "A4";
                    break;
                case "1H":
                    retval = "Golf III/Jetta III/Vento III";
                    platform = "A3";
                    make = "Volkswagen";
                    break;
                case "1G":
                    retval = "Golf II/Jetta II";
                    make = "Volkswagen";
                    break;
                case "1E":
                    retval = "Golf cabrio";
                    make = "Volkswagen";
                    break;

                case "5Z":
                    retval = "Fox";
                    make = "Volkswagen";
                    break;
                case "1F":
                case "AH":
                    platform = "A5";
                    retval = "Eos";
                    make = "Volkswagen";
                    break;

                case "9K":
                case "2K":
                    retval = "Caddy";
                    make = "Volkswagen";
                    break;
                case "50":
                    retval = "Corrado";
                    platform = "A2";
                    make = "Volkswagen";
                    break;



                case "8C":
                    retval = "80/90/S2";
                    make = "Audi";
                    platform = "B4";
                    break;
                case "4A":
                    retval = "100/A6/S6";
                    make = "Audi";
                    platform = "C4";
                    break;
                case "44":
                    make = "Audi";
                    retval = "200/V8";
                    platform = "D1";
                    break;
                case "8X":
                    make = "Audi";
                    retval = "A1";
                    break;
                case "8Z":
                    make = "Audi";
                    retval = "A2";
                    break;
                case "8L":
                    make = "Audi";
                    retval = "A3/S3";
                    platform = "A4";
                    break;
                case "8P":
                case "FM":
                    make = "Audi";
                    retval = "A3/S3";
                    platform = "A5";
                    break;
                case "8D":
                    make = "Audi";
                    retval = "A4/S4/RS4";
                    platform = "B5";
                    break;
                case "8E":
                    make = "Audi";
                    retval = "A4/S4/RS4";
                    platform = "B6";
                    break;
                case "8H":
                    make = "Audi";
                    retval = "A4/S4/RS4";
                    platform = "B7";
                    break;
                case "8K":
                    make = "Audi";
                    retval = "A4/S4/RS4";
                    platform = "B8";
                    break;
                case "8T":
                    make = "Audi";
                    retval = "A5/S5";
                    platform = "AU48x";
                    break;
                case "4F":
                case "FB":
                    make = "Audi";
                    retval = "A6/S6/RS6";
                    platform = "C6/AU56x";
                    break;
                case "4G":
                    make = "Audi";
                    retval = "A6/A7";
                    platform = "";
                    break;
                case "4L":
                case "FD":
                    make = "Audi";
                    retval = "A7/Q7";
                    platform = "AU756";
                    break;
                case "4D":
                    make = "Audi";
                    retval = "A8/S8";
                    platform = "D2";
                    break;
                case "4E":
                case "FA":
                    make = "Audi";
                    retval = "A8/A8L/S8";
                    platform = "D3";
                    break;
                case "4H":
                    make = "Audi";
                    retval = "A8";
                    break;
                case "8G":
                    make = "Audi";
                    retval = "Cabriolet";
                    platform = "B3";
                    break;
                case "8B":
                    make = "Audi";
                    retval = "Coupé";
                    platform = "B3";
                    break;
                case "8J":
                    make = "Audi";
                    retval = "Q3/TT";
                    break;
                case "8R":
                    make = "Audi";
                    retval = "Q5";
                    break;

                //case "7M":
                case "71":
                    make = "Seat";
                    retval = "Alhambra";
                    break;
                case "5P":
                    make = "Seat";
                    retval = "Altea";
                    break;
                case "6H":
                    make = "Seat";
                    retval = "Arosa";
                    break;
                case "6K":
                case "6L":
                    make = "Seat";
                    retval = "Ibiza/Cordoba";
                    break;
                case "3R":
                    make = "Seat";
                    retval = "Exeo";
                    break;
                case "6J":
                    make = "Seat";
                    retval = "Ibiza";
                    break;
                /*case "9K":
                    make = "Seat";
                    retval = "Inca";
                    break;*/
                case "1M":
                case "1P":
                    make = "Seat";
                    retval = "Leon";
                    break;
                case "1L":
                //case "5P":
                    make = "Seat";
                    retval = "Toledo";
                    break;


                case "6Y":
                case "5J":
                    make = "Skoda";
                    retval = "Fabia";
                    break;
                case "6U":
                    make = "Skoda";
                    retval = "Felicia";
                    break;
                case "67":
                    make = "Skoda";
                    retval = "Pickup";
                    break;
                case "1U":
                case "1Z":
                    make = "Skoda";
                    retval = "Octavia";
                    break;
                /*case "5J":
                    make = "Skoda";
                    retval = "Roomster";
                    break;*/
                case "5L":
                    make = "Skoda";
                    retval = "Yeti";
                    break;
                case "3U":
                case "3Y":
                    make = "Skoda";
                    retval = "Superb";
                    break;



            }
            return retval;
        }

        private int decodeMakeYear(string vin)
        {
            int retval = 0;
            switch (vin[9])
            {
                case 'T':
                    retval = 1996;
                    break;
                case 'V':
                    retval = 1997;
                    break;
                case 'W':
                    retval = 1998;
                    break;
                case 'X':
                    retval = 1999;
                    break;
                case 'Y':
                    retval = 2000;
                    break;
                case '1':
                    retval = 2001;
                    break;
                case '2':
                    retval = 2002;
                    break;
                case '3':
                    retval = 2003;
                    break;
                case '4':
                    retval = 2004;
                    break;
                case '5':
                    retval = 2005;
                    break;
                case '6':
                    retval = 2006;
                    break;
                case '7':
                    retval = 2007;
                    break;
                case '8':
                    retval = 2008;
                    break;
                case '9':
                    retval = 2009;
                    break;
            }
            return retval;
        }

        private string decodeConcern(string vin)
        {
            if (vin.StartsWith("WVW")) return "VAG";
            return "";
        }

        private string decodePlantInfo(string vin)
        {
            string retval = string.Empty;
            switch (vin[10])
            {
                case 'A':
                    retval = "Ingolstadt";
                    break;
                case 'B':
                    retval = "Bruxelles";
                    break;
                case 'C':
                    retval = "Taipeh, Taiwan";
                    break;
                case 'D':
                    retval = "Bratislava";
                    break;
                case 'E':
                    retval = "Emden";
                    break;
                case 'G':
                    retval = "Steyr, Graz";
                    break;
                case 'H':
                    retval = "Hannover";
                    break;
                case 'K':
                    retval = "Osnabrück";
                    break;
                case 'M':
                    retval = "Puebla, Mexico";
                    break;
                case 'N':
                    retval = "Neckarsulm";
                    break;
                case 'P':
                    retval = "Mosel, Sachsen";
                    break;
                case 'R':
                    retval = "Martorell, Spain";
                    break;
                case 'S':
                    retval = "Salzgitter";
                    break;
                case 'T':
                    retval = "Skoda";
                    break;
                case 'U':
                    retval = "Uitenhage, Zuid Afrika";
                    break;
                case 'V':
                    retval = "Palmela, Portugal";
                    break;
                case 'W':
                    retval = "Wolfsburg";
                    break;
                case 'X':
                    retval = "Poznan";
                    break;
                case 'Y':
                    retval = "Pamplona";
                    break;
                case 'Z':
                    retval = "Zuffenhausem";
                    break;
                case '0':
                    retval = "Anchieta, Brasil";
                    break;
                case '2':
                    retval = "Shanghai";
                    break;
                case '4':
                    retval = "Curitiba, Brasil";
                    break;
                case '5':
                    retval = "Taubate, Brasil";
                    break;
                case '8':
                    retval = "Dresden";
                    break;
                case '9':
                    retval = "Sarajevo";
                    break;
                    
            }
            return retval;
        }

    }

    public class VINCarInfo
    {
        private string _concern = string.Empty;

        public string Concern
        {
            get { return _concern; }
            set { _concern = value; }
        }
        private string _make = string.Empty;

        public string Make
        {
            get { return _make; }
            set { _make = value; }
        }
        private string _model = string.Empty;

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string _plantInfo = string.Empty;

        public string PlantInfo
        {
            get { return _plantInfo; }
            set { _plantInfo = value; }
        }

        private Int32 _makeyear = 0;

        public Int32 Makeyear
        {
            get { return _makeyear; }
            set { _makeyear = value; }
        }


        private bool _valid = false;

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        private string _engineType = string.Empty;

        public string EngineType
        {
            get { return _engineType; }
            set { _engineType = value; }
        }

        private string _platform = string.Empty;

        public string Platform
        {
            get { return _platform; }
            set { _platform = value; }
        }
    }
}
