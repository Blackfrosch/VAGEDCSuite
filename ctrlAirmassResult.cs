using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Controls;

namespace VAGSuite
{

    public enum TurboType : int
    {
        Stock,
        GT17,
        TD0415T,
        TD0419T,
        GT28BB,
        GT28RS,
        GT3071R,
        HX35w,
        HX40w
    }

    public enum limitType : int
    {
        None,
        TorqueLimiterEngine,
        TorqueLimiterGear,
        AirmassLimiter,
        TurboSpeedLimiter,
        FuelCutLimiter,
        AirTorqueCalibration
    }

    public partial class ctrlAirmassResult : DevExpress.XtraEditors.XtraUserControl
    {
        int rows;
        int columns;

        int m_injectorConstant = 0;

        private int[] y_axisvalues;
        private int[] x_axisvalues;
        private int[] limitermap;


        private string m_currentfile = string.Empty;
        private int m_MaxValueInTable = 0;
        public string Currentfile
        {
            get { return m_currentfile; }
            set { m_currentfile = value; }
        }

        private SymbolCollection m_symbols;

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }

        public delegate void StartTableViewer(object sender, StartTableViewerEventArgs e);
        public event ctrlAirmassResult.StartTableViewer onStartTableViewer;

        public class StartTableViewerEventArgs : System.EventArgs
        {
            private string _mapname;

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }

            public StartTableViewerEventArgs(string mapname)
            {
                this._mapname = mapname;
            }
        }

        public ctrlAirmassResult()
        {
            InitializeComponent();
        }



        private int m_currentfile_size = 0x80000;

        public int Currentfile_size
        {
            get { return m_currentfile_size; }
            set { m_currentfile_size = value; }
        }

        private Int64 GetSymbolAddress(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Userdescription == symbolname || sh.Varname == symbolname)
                {
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }
        private int GetSymbolLength(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Userdescription == symbolname || sh.Varname == symbolname)
                {
                    return sh.Length;
                }
            }
            return 0;
        }
        private byte[] reverseEndian(byte[] retval)
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
        private byte[] readdatafromfile(string filename, int address, int length)
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
                retval = reverseEndian(retval);
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

        private int[] readIntdatafromfile(string filename, int address, int length)
        {
            int[] retval = new int[length / 2];
            try
            {
                FileStream fsi1 = File.OpenRead(filename);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryReader br1 = new BinaryReader(fsi1);
                fsi1.Position = address;
                string temp = string.Empty;
                int j = 0;
                for (int i = 0; i < length; i += 2)
                {
                    byte b1 = br1.ReadByte();
                    byte b2 = br1.ReadByte();
                    int value = Convert.ToInt32(b1) + Convert.ToInt32(b2) * 256; // LoHi
                    if (_ECUType.Contains("EDC16"))
                    {
                        value = Convert.ToInt32(b1) * 256 + Convert.ToInt32(b2); // HiLo
                    }
                    retval.SetValue(value, j++);
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

        private string _ECUType = string.Empty;

        public string ECUType
        {
            get { return _ECUType; }
            set { _ECUType = value; }
        }


        private int m_numberCylinders = 4;

        public int NumberCylinders
        {
            set
            {
                m_numberCylinders = value;
            }

        }
        private SymbolHelper GetSymbolLike(SymbolCollection sc, string symbolname, int codeblock)
        {
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Varname.StartsWith(symbolname) && sh.CodeBlock == codeblock)
                {
                    return sh;
                }
            }
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Varname.StartsWith(symbolname))
                {
                    return sh;
                }
            }
            return new SymbolHelper();
        }

        public PerformanceResults Calculate(string filename, SymbolCollection symbols)
        {
            // do the math!
            PerformanceResults pr = new PerformanceResults();
            FillComboBoxBankSelection();
            
            SymbolHelper driverWishHelper = GetSymbolLike(symbols, "Driver wish", selectedBank);
            int[] pedalrequestmap = readIntdatafromfile(filename, (int)driverWishHelper.Flash_start_address, driverWishHelper.Length);
            limitermap = new int[pedalrequestmap.Length];
            int[] resulttable = new int[pedalrequestmap.Length]; // result 

            rows = driverWishHelper.Y_axis_length;
            columns = driverWishHelper.X_axis_length;

            int[] pedalXAxis = readIntdatafromfile(filename, driverWishHelper.Y_axis_address, driverWishHelper.Y_axis_length * 2);
            int[] pedalYAxis = readIntdatafromfile(filename, driverWishHelper.X_axis_address, driverWishHelper.X_axis_length * 2);
            y_axisvalues = pedalYAxis;
            x_axisvalues = pedalXAxis;
            for (int colcount = 0; colcount < columns; colcount++)
            {
                for (int rowcount = 0; rowcount < rows; rowcount++)
                {
                    // get the current value from the request map
                    if (_ECUType.Contains("EDC16"))
                    {
                        int requestedTorque = (int)pedalrequestmap.GetValue((colcount * rows) + rowcount);
                        limitType limiterType = limitType.None;
                        int resultingAirMass = CalculateMaxAirmassforcell(symbols, filename, (int)pedalXAxis.GetValue(rowcount), ((int)pedalYAxis.GetValue(colcount)), requestedTorque, checkEdit1.Checked, out limiterType, true);
                        resulttable.SetValue(resultingAirMass, (colcount * rows) + rowcount);
                        limitermap.SetValue(limiterType, (colcount * rows) + rowcount);
                    }
                    else
                    {
                        int InjectedQuantity = (int)pedalrequestmap.GetValue((colcount * rows) + rowcount);
                        limitType limiterType = limitType.None;
                        int resultingAirMass = CalculateMaxAirmassforcell(symbols, filename, (int)pedalXAxis.GetValue(rowcount), ((int)pedalYAxis.GetValue(colcount)), InjectedQuantity, checkEdit1.Checked, out limiterType, false);
                        resulttable.SetValue(resultingAirMass, (colcount * rows) + rowcount);
                        limitermap.SetValue(limiterType, (colcount * rows) + rowcount);
                    }
                }
            }
            // now show resulttable
            DataTable dt = new DataTable();
            foreach (int xvalue in pedalYAxis)
            {
                dt.Columns.Add(xvalue.ToString());
            }
            // now fill the table rows
            m_MaxValueInTable = 0;
            for (int r = 0; r < pedalXAxis.Length ; r++)
            {
                object[] values = new object[columns];
                for (int t = 0; t < pedalYAxis.Length; t++)
                {
                    int currValue = (int)resulttable.GetValue( (((t + 1) * rows) - 1) - r  );
                    if (currValue > m_MaxValueInTable) m_MaxValueInTable = currValue;
                    values.SetValue(currValue, t);
                }
                dt.Rows.Add(values);
            }
            gridControl1.DataSource = dt;

            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
            try
            {
                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                    // convert to hp
                    int rpm = Convert.ToInt32(y_axisvalues.GetValue(i));
                    int torque = Tools.Instance.IQToTorque(Convert.ToInt32(o), rpm, m_numberCylinders);
                    if (_ECUType.Contains("EDC16"))
                    {
                        torque = Convert.ToInt32(o);
                        torque *= 10; // correction to keep the code identical from here
                        double temptorque = torque * Tools.Instance.GetCorrectionFactorForRpm(rpm, m_numberCylinders);
                        torque = Convert.ToInt32(temptorque);
                    }
                    int horsepower = Tools.Instance.TorqueToPower(torque, rpm);

                   // if (checkEdit5.Checked) horsepower = Tools.Instance.TorqueToPowerkW(torque, rpm);
                   // if (checkEdit6.Checked) torque = Tools.Instance.IQToTorque(Convert.ToInt32(o), rpm, m_numberCylinders);//AirmassToTorqueLbft(Convert.ToInt32(o), rpm);

                    horsepower /= 100;
                    torque /= 100;
                    if (torque > pr.Torque) pr.Torque = torque;
                    if (horsepower > pr.Horsepower) pr.Horsepower = horsepower;
                }
            }
            catch (Exception)
            {

            }
            return pr;

        }

        private void FillComboBoxBankSelection()
        {
            if (Tools.Instance.codeBlockList != null)
            {
                foreach (CodeBlock cb in Tools.Instance.codeBlockList)
                {
                    try
                    {
                        bool found = false;
                        foreach (string cbi in comboBoxEdit1.Properties.Items)
                        {
                            if (cbi == "Codebank " + cb.CodeID.ToString())
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            comboBoxEdit1.Properties.Items.Add("Codebank " + cb.CodeID.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private bool SymbolExists(string symbolname)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname) return true;
            }
            return false;
        }

        private int CalculateMaxAirmassforcell(SymbolCollection symbols, string filename, int pedalposition, int rpm, int requestedQuantity, bool autogearbox, out limitType limiterType, bool torqueBased)
        {
            limiterType = limitType.None;
            // check against torque limiter
            SymbolHelper trqLimiter = GetSymbolLike(symbols, "Torque limiter", selectedBank);
            int[] trqLimitMap = readIntdatafromfile(filename, (int)trqLimiter.Flash_start_address, trqLimiter.Length);
            int[] trqXAxis = readIntdatafromfile(filename, trqLimiter.Y_axis_address, trqLimiter.Y_axis_length * 2);
            int[] trqYAxis = readIntdatafromfile(filename, trqLimiter.X_axis_address, trqLimiter.X_axis_length * 2);

            if (torqueBased)
            {
                int Nmlimit = Convert.ToInt32(GetInterpolatedTableValue(trqLimitMap, trqXAxis, trqYAxis, Convert.ToInt32(spinEdit1.EditValue) * 10, rpm));
                //requestedairmass
                if (requestedQuantity > Nmlimit)
                {
                    Console.WriteLine("Torque is limited from " + requestedQuantity.ToString() + " to " + Nmlimit.ToString() + " at " + rpm.ToString() + " rpm");
                    requestedQuantity = Nmlimit;
                    limiterType = limitType.TorqueLimiterEngine;
                }
            }
            else
            {
                int IQlimit = requestedQuantity;
                if (trqXAxis.Length == 1)
                {
                    // 2d map
                    IQlimit = Convert.ToInt32(GetInterpolatedTableValue(trqLimitMap, trqXAxis, trqYAxis, rpm, Convert.ToInt32(spinEdit1.EditValue) * 10));
                }
                else
                {
                    IQlimit = Convert.ToInt32(GetInterpolatedTableValue(trqLimitMap, trqXAxis, trqYAxis, Convert.ToInt32(spinEdit1.EditValue) * 10, rpm));
                }
                //requestedairmass
                if (requestedQuantity > IQlimit)
                {
//                    Console.WriteLine("IQ is limited from " + requestedQuantity.ToString() + " to " + IQlimit.ToString() + " at " + rpm.ToString() + " rpm");
                    requestedQuantity = IQlimit;

                    limiterType = limitType.TorqueLimiterEngine;
                }
            }

            // check against smoke limiter

            return requestedQuantity; // no limits ... 
        }


        private int CheckAgainstTorqueLimiters(SymbolCollection symbols, string filename, int rpm, int requestedairmass, bool Automatic, out limitType TrqLimiter)
        {
            int torque = 0;
            TrqLimiter = limitType.None;
            int LimitedAirMass = requestedairmass;
            int[] xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(symbols, "TorqueCal.m_AirXSP"));
            int[] yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(symbols, "TorqueCal.n_EngYSP"));
            torque = Tools.Instance.IQToTorque(requestedairmass, rpm, m_numberCylinders);

            int[] enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxTab"));
            if (Automatic)
            {
                enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxAutTab"));
            }

            int[] xdummy = new int[1];
            xdummy.SetValue(0, 0);
            int torquelimit1 = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
            //requestedairmass
            if (torque > torquelimit1)
            {
                //Console.WriteLine("Torque is limited from " + torque.ToString() + " to " + torquelimit1.ToString() + " at " + rpm.ToString() + " rpm");
                torque = torquelimit1;
                TrqLimiter = limitType.TorqueLimiterEngine;
            }



            int[] airtorquemap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.m_AirTorqMap"), GetSymbolLength(symbols, "TorqueCal.m_AirTorqMap"));
            int[] xairtorque = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(symbols, "TorqueCal.M_EngXSP"));
            int TestLimitedAirmass = Convert.ToInt32(GetInterpolatedTableValue(airtorquemap, xairtorque, yaxis, rpm, torque));
            if (TestLimitedAirmass < LimitedAirMass)
            {
                LimitedAirMass = TestLimitedAirmass;
                if (TrqLimiter == limitType.None) TrqLimiter = limitType.AirTorqueCalibration;
            }
            if (TrqLimiter == limitType.None) LimitedAirMass = requestedairmass; // bugfix for if no limiter is active
            return LimitedAirMass;
        }

        private int CheckAgainstFuelcutLimiter(SymbolCollection symbols, string filename, int requestedairmass, ref limitType AirmassLimiter)
        {
            int retval = requestedairmass;
            if ((int)GetSymbolAddress(symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                int[] fuelcutlimit = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(symbols, "FCutCal.m_AirInletLimit"));
                if (fuelcutlimit.Length > 0)
                {
                    if (Convert.ToInt32(fuelcutlimit.GetValue(0)) < requestedairmass)
                    {
                        retval = Convert.ToInt32(fuelcutlimit.GetValue(0));
                        AirmassLimiter = limitType.FuelCutLimiter;
                    }
                }
            }
            return retval;

        }

        private int CheckAgainstTurboSpeedLimiter(SymbolCollection symbols, string filename, int rpm, int requestedairmass, ref limitType AirmassLimiter)
        {
         
            int cols = GetSymbolLength(symbols, "LimEngCal.p_AirSP") / 2;
            int[] turbospeed = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.TurboSpeedTab"), GetSymbolLength(symbols, "LimEngCal.TurboSpeedTab"));
            int[] xaxis = new int[1];
            xaxis.SetValue(0, 0);
            int[] yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.p_AirSP"), GetSymbolLength(symbols, "LimEngCal.p_AirSP"));
            int ambientpressure = /*1000*/ Convert.ToInt32(spinEdit1.EditValue) * 10;
            int airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(turbospeed, xaxis, yaxis, /*rpm*/ ambientpressure, 0));
            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = limitType.TurboSpeedLimiter;
            }
            return requestedairmass;
        }

        private int CheckAgainstAirmassLimiters(SymbolCollection symbols, string filename, int rpm, int requestedairmass, bool autogearbox, ref limitType AirmassLimiter)
        {
            //AirmassLimiter = limitType.None;
            //Y axis = BstKnkCal.n_EngYSP needed for interpolation (16)
            //X axis = BstKnkCal.OffsetXSP needed for length (16)
            // check against BstKnkCal.MaxAirmass
            int cols = GetSymbolLength(symbols, "BstKnkCal.OffsetXSP") / 2;
            int rows = GetSymbolLength(symbols, "BstKnkCal.n_EngYSP") / 2;
            // only the right-most column (no knock)
            int[] bstknk = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(symbols, "BstKnkCal.MaxAirmass"));
            if (autogearbox)
            {
                bstknk = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(symbols, "BstKnkCal.MaxAirmassAu"));
            }
            int[] xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.OffsetXSP"), GetSymbolLength(symbols, "BstKnkCal.OffsetXSP"));
            for (int a = 0; a < xaxis.Length; a++)
            {
                int val = (int)xaxis.GetValue(a);
                if (val > 32000) val = -(65536 - val);
                xaxis.SetValue(val, a);
            }
            int[] yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.n_EngYSP"), GetSymbolLength(symbols, "BstKnkCal.n_EngYSP"));
            int airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknk, xaxis, yaxis, rpm, 0));
            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = limitType.AirmassLimiter;
                //Console.WriteLine("Reduced airmass because of BstKnkCal.MaxAirmass: " + requestedairmass.ToString() + " rpm: " + rpm.ToString());
            }
            return requestedairmass;
        }


        private double GetInterpolatedTableValue(int[] table, int[] xaxis, int[] yaxis, int yvalue, int xvalue)
        {
            int m_yindex = 0;
            int m_xindex = 0;
            double result = 0;
            double m_ydiff = 0;
            double m_xdiff = 0;
            double m_ypercentage = 0;
            double m_xpercentage = 0;
            try
            {
                for (int yindex = 0; yindex < yaxis.Length; yindex++)
                {
                    if (yvalue > Convert.ToInt32(yaxis.GetValue(yindex)))
                    {
                        m_yindex = yindex;
                        m_ydiff = Math.Abs(Convert.ToDouble(yaxis.GetValue(yindex)) - yvalue);
                        if (m_yindex < yaxis.Length - 1)
                        {
                            m_ypercentage = (double)m_ydiff / (Convert.ToInt32(yaxis.GetValue(yindex + 1)) - Convert.ToInt32(yaxis.GetValue(yindex)));
                        }
                        else
                        {
                            m_ypercentage = 0;
                        }
                        // break;
                    }
                }
                for (int xindex = 0; xindex < xaxis.Length; xindex++)
                {
                    if (xvalue > Convert.ToDouble(xaxis.GetValue(xindex)))
                    {
                        m_xindex = xindex;
                        m_xdiff = Math.Abs(Convert.ToDouble(xaxis.GetValue(xindex)) - xvalue);
                        if (m_xindex < xaxis.Length - 1)
                        {
                            m_xpercentage = m_xdiff / (Convert.ToInt32(xaxis.GetValue(xindex + 1)) - Convert.ToInt32(xaxis.GetValue(xindex)));
                        }
                        else
                        {
                            m_xpercentage = 0;
                        }
                        // break;
                    }
                }
                //AddDebugLog("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
                // now we found the indexes of the smaller values
                int a1 = 0;
                int a2 = 0;
                int b1 = 0;
                int b2 = 0;
                if (m_yindex == (yaxis.Length - 1) && (m_xindex < xaxis.Length - 1))
                {
                    // last row in table, extend with the same values
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = a1;
                    b2 = a2;
                }
                else if (m_yindex == (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = a1;
                    b2 = a1;
                    return Convert.ToDouble(a1);
                }
                else if (m_yindex <= (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = (int)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = b1;
                }
                else
                {
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = (int)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = (int)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex + 1);
                }
                //AddDebugLog("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
                // now interpolate the values found
                double aval = 0;
                double bval = 0;
                double adiff = Math.Abs((double)a1 - (double)a2);
                if (a1 > a2) aval = a1 - (m_xpercentage * adiff);
                else aval = a1 + (m_xpercentage * adiff);
                double bdiff = Math.Abs((double)b1 - (double)b2);
                if (b1 > b2) bval = b1 - (m_xpercentage * bdiff);
                else bval = b1 + (m_xpercentage * bdiff);
                // now interpolate vertically (RPM axis)

                //AddDebugLog("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //AddDebugLog("result = " + result.ToString());
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to interpolate: " + E.Message);
            }
            return result;


        }

        private double GetInterpolatedTableValue(byte[] table, int[] xaxis, int[] yaxis, int yvalue, int xvalue)
        {
            int m_yindex = 0;
            int m_xindex = 0;
            double result = 0;
            double m_ydiff = 0;
            double m_xdiff = 0;
            double m_ypercentage = 0;
            double m_xpercentage = 0;
            try
            {
                for (int yindex = 0; yindex < yaxis.Length; yindex++)
                {
                    if (yvalue > Convert.ToInt32(yaxis.GetValue(yindex)))
                    {
                        m_yindex = yindex;
                        m_ydiff = Math.Abs(Convert.ToDouble(yaxis.GetValue(yindex)) - yvalue);
                        if (m_yindex < yaxis.Length - 1)
                        {
                            m_ypercentage = (double)m_ydiff / (Convert.ToInt32(yaxis.GetValue(yindex + 1)) - Convert.ToInt32(yaxis.GetValue(yindex)));
                        }
                        else
                        {
                            m_ypercentage = 0;
                        }
                        // break;
                    }
                }
                for (int xindex = 0; xindex < xaxis.Length; xindex++)
                {
                    if (xvalue > Convert.ToDouble(xaxis.GetValue(xindex)))
                    {
                        m_xindex = xindex;
                        m_xdiff = Math.Abs(Convert.ToDouble(xaxis.GetValue(xindex)) - xvalue);
                        if (m_xindex < xaxis.Length - 1)
                        {
                            m_xpercentage = m_xdiff / (Convert.ToInt32(xaxis.GetValue(xindex + 1)) - Convert.ToInt32(xaxis.GetValue(xindex)));
                        }
                        else
                        {
                            m_xpercentage = 0;
                        }
                        // break;
                    }
                }
                //AddDebugLog("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
                // now we found the indexes of the smaller values
                byte a1 = 0;
                byte a2 = 0;
                byte b1 = 0;
                byte b2 = 0;
                if (m_yindex == (yaxis.Length - 1) && (m_xindex < xaxis.Length - 1))
                {
                    // last row in table, extend with the same values
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = a1;
                    b2 = a2;
                }
                else if (m_yindex == (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = a1;
                    b2 = a1;
                    return Convert.ToDouble(a1);
                }
                else if (m_yindex <= (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = (byte)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = b1;
                }
                else
                {
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = (byte)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = (byte)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex + 1);
                }
                //AddDebugLog("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
                // now interpolate the values found
                double aval = 0;
                double bval = 0;
                double adiff = Math.Abs((double)a1 - (double)a2);
                if (a1 > a2) aval = a1 - (m_xpercentage * adiff);
                else aval = a1 + (m_xpercentage * adiff);
                double bdiff = Math.Abs((double)b1 - (double)b2);
                if (b1 > b2) bval = b1 - (m_xpercentage * bdiff);
                else bval = b1 + (m_xpercentage * bdiff);
                // now interpolate vertically (RPM axis)

                //AddDebugLog("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //AddDebugLog("result = " + result.ToString());
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to interpolate: " + E.Message);
            }
            return result;


        }
      

        /*private int AirmassToTorqueLbft(int IQ, int rpm)
        {
            double tq = Convert.ToDouble(IQ) * 6;
            double correction = Tools.Instance.GetCorrectionFactorForRpm(rpm);
            tq *= correction;
            //tq /= 1.56; // to lbft
            tq /= 1.3558; //<GS-22032010> bugfix
            return Convert.ToInt32(tq);
        }*/

        

        private void CastStartViewerEvent(string mapname)
        {
            if (onStartTableViewer != null)
            {
                onStartTableViewer(this, new StartTableViewerEventArgs(mapname));
            }
        }
        
        int powerSeries = -1;
        int torqueSeries = -1;

        private string MaximizeFileLength(string filename)
        {
            string retval = filename;
            if (retval.Length > 16) retval = retval.Substring(0, 14) + "..";
            return retval;
        }



        

        private void LoadGraphWithDetails()
        {
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                // get only the WOT cells, the last 16 integers
                // and the columns which hold the rpm stages
                chartControl1.Series.Clear();
                string powerLabel = "Power (bhp)";
                if (checkEdit5.Checked) powerLabel = "Power (kW)";
                string torqueLabel = "Torque (Nm)";
                if (checkEdit6.Checked) torqueLabel = "Torque (lbft)";

                string injectorDCLabel = "Injector DC";
                string targetLambdaLabel = "Target lambda";

                powerSeries = chartControl1.Series.Add(powerLabel, DevExpress.XtraCharts.ViewType.Spline);
                torqueSeries = chartControl1.Series.Add(torqueLabel, DevExpress.XtraCharts.ViewType.Spline);
                // set line colors
                chartControl1.Series[powerSeries].Label.Border.Visible = false;
                chartControl1.Series[torqueSeries].Label.Border.Visible = false;
                chartControl1.Series[powerSeries].Label.Shadow.Visible = true;
                chartControl1.Series[torqueSeries].Label.Shadow.Visible = true;


                chartControl1.Series[powerSeries].View.Color = Color.Red;
                chartControl1.Series[torqueSeries].View.Color = Color.Blue;
                chartControl1.Series[powerSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[powerSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[torqueSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[torqueSeries].ValueScaleType = ScaleType.Numerical;

                SplineSeriesView sv = (SplineSeriesView)chartControl1.Series[powerSeries].View;
                sv.LineMarkerOptions.Visible = false;
                sv = (SplineSeriesView)chartControl1.Series[torqueSeries].View;
                sv.LineMarkerOptions.Visible = false;

                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                    // convert to hp
                    int rpm = Convert.ToInt32(y_axisvalues.GetValue(i));
                    int torque = Tools.Instance.IQToTorque(Convert.ToInt32(o), rpm, m_numberCylinders);
                    if (_ECUType.Contains("EDC16"))
                    {
                        torque = Convert.ToInt32(o);
                        torque *= 10; // correction to keep the code identical from here
                        double temptorque = torque * Tools.Instance.GetCorrectionFactorForRpm(rpm, m_numberCylinders);
                        torque = Convert.ToInt32(temptorque);
                    }
                    int horsepower = Tools.Instance.TorqueToPower(torque, rpm);

                    if (checkEdit5.Checked) horsepower = Tools.Instance.TorqueToPowerkW(torque, rpm);
                    if (checkEdit6.Checked) torque = Tools.Instance.IQToTorque(Convert.ToInt32(o), rpm, m_numberCylinders);//AirmassToTorqueLbft(Convert.ToInt32(o), rpm);

                    horsepower /= 100;
                    torque /= 100;

                    double[] dvals = new double[1];
                    dvals.SetValue(Convert.ToDouble(horsepower), 0);
                    chartControl1.Series[powerSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvals));

                    double[] dvalstorq = new double[1];
                    dvalstorq.SetValue(Convert.ToDouble(torque), 0);
                    chartControl1.Series[torqueSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalstorq));


                }


            }

        }

        
        private void UpdateGraphVisibility()
        {
            if (powerSeries >= 0) chartControl1.Series[powerSeries].Visible = checkEdit8.Checked;
            if (torqueSeries >= 0) chartControl1.Series[torqueSeries].Visible = checkEdit9.Checked;
        }
        string m_current_softwareversion = string.Empty;
        string m_current_comparefilename = string.Empty;
        SymbolCollection Compare_symbol_collection = new SymbolCollection();

        


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_current_comparefilename = ofd.FileName;
                SymbolTranslator translator = new SymbolTranslator();
                string help = string.Empty;
                FileInfo fi = new FileInfo(m_current_comparefilename);
                fi.IsReadOnly = false;

                try
                {
                    m_current_softwareversion = "";
                }
                catch (Exception E2)
                {
                    Console.WriteLine(E2.Message);
                }
                
                // show the dynograph
                xtraTabControl1.SelectedTabPage = xtraTabPage2;
                LoadGraphWithDetails(); // initial values from original bin
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            CastCloseEvent();
        }

        public delegate void ViewerClose(object sender, EventArgs e);
        public event ctrlAirmassResult.ViewerClose onClose;

        private void CastCloseEvent()
        {
            if (onClose != null)
            {
                onClose(this, EventArgs.Empty);
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit3_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit4_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit7_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit5_CheckedChanged(object sender, EventArgs e)
        {
            // refresh
            //Calculate();
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
            
        }

        private void checkEdit6_CheckedChanged(object sender, EventArgs e)
        {
            // refresh
            //Calculate();
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
           
        }

        private void labelControl8_DoubleClick(object sender, EventArgs e)
        {
            // start turbospeed limiter viewer!
            CastStartViewerEvent("LimEngCal.TurboSpeedTab");
        }

        private void labelControl1_DoubleClick(object sender, EventArgs e)
        {
            // start airmass limiter viewer
            // if aut
            if (checkEdit1.Checked)
            {
                CastStartViewerEvent("BstKnkCal.MaxAirmassAu");
            }
            else
            {
                CastStartViewerEvent("BstKnkCal.MaxAirmass");
            }
        }

        private void labelControl2_DoubleClick(object sender, EventArgs e)
        {
            // start E85 torque limiter
            CastStartViewerEvent("TorqueCal.M_EngMaxE85Tab");
        }

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            CastStartViewerEvent("Torque limiter");
        }

        private void labelControl12_DoubleClick(object sender, EventArgs e)
        {
            


        }

        private void labelControl14_DoubleClick(object sender, EventArgs e)
        {
            CastStartViewerEvent("FCutCal.m_AirInletLimit");
        }

        private void labelControl16_DoubleClick(object sender, EventArgs e)
        {
            // show overboost tab
            CastStartViewerEvent("TorqueCal.M_OverBoostTab");
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage1.Name)
            {
                // in table view
            }
            else if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                // in graph view
                LoadGraphWithDetails();
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        int b = 0;
                        int cellvalue = 0;
                        //if (m_isHexMode)
                        b = Convert.ToInt32(e.CellValue.ToString());
                        cellvalue = b;
                        b *= 255;
                        if (m_MaxValueInTable != 0)
                        {
                            b /= m_MaxValueInTable;
                        }
                        int red = 128;
                        int green = 128;
                        int blue = 128;
                        Color c = Color.White;
                        red = b;
                        if (red < 0) red = 0;
                        if (red > 255) red = 255;
                        if (b > 255) b = 255;
                        blue = 0;
                        green = 255 - red;
                        c = Color.FromArgb(red, green, blue);
                        SolidBrush sb = new SolidBrush(c);
                        e.Graphics.FillRectangle(sb, e.Bounds);

                        // check limiter type
                        //limitermap
                        int row = rows - (e.RowHandle + 1);
                        limitType curLimit = (limitType)limitermap.GetValue((row * columns) + e.Column.AbsoluteIndex);
                        Point[] pnts = new Point[4];
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 0);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width - (e.Bounds.Height / 2), e.Bounds.Y), 1);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y + (e.Bounds.Height / 2)), 2);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 3);
                        if (curLimit == limitType.AirmassLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.Blue, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TorqueLimiterEngine)
                        {
                            e.Graphics.FillPolygon(Brushes.Yellow, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TurboSpeedLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.Black, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TorqueLimiterGear)
                        {
                            e.Graphics.FillPolygon(Brushes.SaddleBrown, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.FuelCutLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.DarkGray, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        if (comboBoxEdit2.SelectedIndex == 1)
                        {
                            // convert airmass to torque
                            if (_ECUType.Contains("EDC16"))
                            {
                                int rpm = Convert.ToInt32(y_axisvalues.GetValue(e.Column.AbsoluteIndex));
                                int torque = Convert.ToInt32(e.CellValue);
                                torque /= 10;
                                e.DisplayText = torque.ToString();
                            }
                            else
                            {
                                int rpm = Convert.ToInt32(y_axisvalues.GetValue(e.Column.AbsoluteIndex));
                                int torque = Tools.Instance.IQToTorque(Convert.ToInt32(e.CellValue), rpm, m_numberCylinders);
                                if (checkEdit6.Checked)
                                {
                                    torque = Tools.Instance.IQToTorque(Convert.ToInt32(e.CellValue), rpm, m_numberCylinders);// AirmassToTorqueLbft(Convert.ToInt32(e.CellValue), rpm);
                                }
                                torque /= 100;
                                e.DisplayText = torque.ToString();
                            }
                        }
                        else if (comboBoxEdit2.SelectedIndex == 2)
                        {
                            if (_ECUType.Contains("EDC16"))
                            {
                                int rpm = Convert.ToInt32(y_axisvalues.GetValue(e.Column.AbsoluteIndex));
                                int torque = Convert.ToInt32(e.CellValue);
                                torque /= 10;
                                double temptorque = torque * Tools.Instance.GetCorrectionFactorForRpm(rpm, m_numberCylinders);
                                torque = Convert.ToInt32(temptorque);
                                int horsepower = Tools.Instance.TorqueToPower(torque, rpm);
                                if (checkEdit5.Checked)
                                {
                                    horsepower = Tools.Instance.TorqueToPowerkW(torque, rpm);
                                }
                                e.DisplayText = horsepower.ToString();
                            }
                            else
                            {
                                //convert airmass to horsepower
                                int rpm = Convert.ToInt32(y_axisvalues.GetValue(e.Column.AbsoluteIndex));
                                int torque = Tools.Instance.IQToTorque(Convert.ToInt32(e.CellValue), rpm, m_numberCylinders);
                                int horsepower = Tools.Instance.TorqueToPower(torque, rpm);
                                if (checkEdit5.Checked)
                                {
                                    horsepower = Tools.Instance.TorqueToPowerkW(torque, rpm);
                                }
                                horsepower /= 100;
                                e.DisplayText = horsepower.ToString();
                            }
                        }
                        else
                        {
                            if (_ECUType.Contains("EDC16"))
                            {
                                // should display IQ in stead of torque
                                int rpm = Convert.ToInt32(y_axisvalues.GetValue(e.Column.AbsoluteIndex));
                                int torque = Convert.ToInt32(e.CellValue);
                                torque /= 10;
                                e.DisplayText = Tools.Instance.TorqueToIQ(torque, rpm, m_numberCylinders).ToString();
                            }
                            else
                            {
                                int airmass = Convert.ToInt32(e.CellValue);
                                airmass /= 100;
                                e.DisplayText = airmass.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {

                //  e.Painter.DrawCaption(new DevExpress.Utils.Drawing.ObjectInfoArgs(new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics)), "As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, null);
                // e.Cache.DrawString("As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, new StringFormat());
                try
                {
                    if (x_axisvalues.Length > 0)
                    {
                        if (x_axisvalues.Length > e.RowHandle)
                        {
                            int value = (int)x_axisvalues.GetValue((x_axisvalues.Length - 1) - e.RowHandle);
                            value /= 10;
                            string yvalue = value.ToString();
                            /*if (!m_isUpsideDown)
                            {
                                // dan andere waarde nemen
                                yvalue = y_axisvalues.GetValue(e.RowHandle).ToString();
                            }
                            if (m_y_axis_name == "MAP")
                            {
                                if (m_viewtype == ViewType.Easy3Bar || m_viewtype == ViewType.Decimal3Bar)
                                {
                                    int tempval = Convert.ToInt32(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                                    if (!m_isUpsideDown)
                                    {
                                        tempval = Convert.ToInt32(y_axisvalues.GetValue(e.RowHandle));
                                    }
                                    tempval *= 120;
                                    tempval /= 100;
                                    yvalue = tempval.ToString();
                                }
                            }*/

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - 12) / 2));
                            e.Handled = true;
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
        }

       
        private void checkEdit8_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGraphVisibility();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (chartControl1.IsPrintingAvailable)
            {
                printToolStripMenuItem.Enabled = true;
            }
            else
            {
                printToolStripMenuItem.Enabled = false;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save graph as image
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG images|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                chartControl1.ExportToImage(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // print graph
            // set paper to landscape
            chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;
            //DevExpress.XtraPrinting.PrintingSystem ps = new DevExpress.XtraPrinting.PrintingSystem();
            //ps.PageSettings.Landscape = true;

            chartControl1.ShowPrintPreview();
        }

        private void labelControl14_Click(object sender, EventArgs e)
        {

        }

        private void labelControl13_Click(object sender, EventArgs e)
        {

        }

        private int selectedBank = 0;

        private void comboBoxEdit1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            selectedBank = ExtractSelectedBank(comboBoxEdit1.SelectedItem.ToString());
            Calculate(m_currentfile, m_symbols);
        }

        private int ExtractSelectedBank(string itemtext)
        {
            int retval = 0;
            try
            {
                retval = Convert.ToInt32(itemtext.Replace("Codebank ", ""));
            }
            catch (Exception)
            {

            }
            return retval;
        }
    }
}
