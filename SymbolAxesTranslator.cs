using System;
using System.Collections.Generic;
using System.Text;

namespace VAGSuite
{
    class SymbolAxesTranslator
    {

        private Int32 GetSymbolAxisXID(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.X_axis_ID;
                }
            }
            return 0;
        }
        private Int32 GetSymbolAxisYID(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Y_axis_ID;
                }
            }
            return 0;
        }

        public bool GetAxisSymbols(SymbolCollection curSymbols, string symbolname, out string x_axis, out string y_axis, out string x_axis_description, out string y_axis_description, out string z_axis_description)
        {
            bool retval = false;
            x_axis = "";
            y_axis = "";
            x_axis_description = "x-axis";
            y_axis_description = "y-axis";
            z_axis_description = "z-axis";
            int xid = GetSymbolAxisXID(curSymbols, symbolname);
            y_axis_description = TranslateAxisID(xid);
            int yid = GetSymbolAxisYID(curSymbols, symbolname);
            x_axis_description = TranslateAxisID(yid);

            /*
            switch (symbolname)
            {
                case "MAFCal.WeightConstFuelMap":
                    x_axis = "MAFCal.n_EngineXSP";
                    y_axis = "MAFCal.p_InletGradYSP";
                    x_axis_description = "Engine speed (%)";
                    y_axis_description = "Inlet pressure (kPa)";
                    break;
            }
            if (y_axis != "") retval = true;*/
            return retval;
        }

        public string TranslateAxisID(int id)
        {
            string retval = id.ToString("X4");
           /* switch (id)
            {
                case 0xEC38:
                case 0xEC2E:
                retval = "Injection Quantity mg/stroke";
                    break;
                case 0xC048:
                case 0xC042:
                case 0xC030:
                case 0xC0BA:
                case 0xC044:
                case 0xC036:
                case 0xDA70:
                case 0xC5A4:
                    retval = "Engine speed (rpm)";
                    break;
                case 0xF94A:
                    retval = "Airflow mg/hub";
                    break;
                case 0xC19E:
                    retval = "Air intake temp";
                    break;
                    break;
            }*/
            return retval;
        }

        public string GetXaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            return retval;
        }

        public string GetYaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            
            return retval;
        }

    }
}
