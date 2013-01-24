using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VAGSuite
{
    public partial class ctrlCompressorMap : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void RefreshData(object sender, EventArgs e);
        public event ctrlCompressorMap.RefreshData onRefreshData;


        public enum CompressorMap : int
        {
            GT17,
            T25_55,
            T25_60,
            TD04,
            TD0416T, //nieuw
            TD0418T,
            TD0419T,
            TD0620G, //nieuw
            GT2871R, //nieuw
            GT28RS,
            GT3071R86,
            GT30R,
            GT40R,
            HX40W
        }

        private CompressorMap _compressor = CompressorMap.T25_55;

        public CompressorMap Compressor
        {
            get { return _compressor; }
            set { _compressor = value; }
        }

        private bool _isInitiallyLoaded = false;

        public bool IsInitiallyLoaded
        {
            get { return _isInitiallyLoaded; }
            set { _isInitiallyLoaded = value; }
        }

        public ctrlCompressorMap()
        {
            InitializeComponent();

            

        }

        private void t25ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load T25 map
            SetCompressorType(CompressorMap.T25_55);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void tD0415GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load TD04 map
            SetCompressorType(CompressorMap.TD04);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void gT28RSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load GT28RS map
            SetCompressorType(CompressorMap.GT28RS);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void gT30RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load GT30R map
            SetCompressorType(CompressorMap.GT30R);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private double x_offset = 0;
        private double y_offset = 0;
        private double x_multiplier = 1;
        private double y_multiplier = 1;
        private int ori_width = 0;
        private int ori_height = 0;

        private double[] boost_request = new double[16] { 0.20, 0.50, 1.05, 1.08, 1.03, 1.00, 1.00, 0.98, 0.94, 0.91, 0.94, 0.88, 0.79, 0.68, 0.58, 0.50};

        public double[] Boost_request
        {
            get { return boost_request; }
            set { boost_request = value; }
        }
        private int[] rpm_points = new int[16] { 1000, 1500, 1750, 2000, 2250, 2500, 2750, 3000, 3250, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };

        public int[] Rpm_points
        {
            get { return rpm_points; }
            set { rpm_points = value; }
        }

        public enum EngineType : int
        {
            Liter2,
            Liter23
        }

        private EngineType current_engineType = EngineType.Liter23;

        public EngineType Current_engineType
        {
            get { return current_engineType; }
            set
            {
                current_engineType = value;
                if (current_engineType == EngineType.Liter2) toolStripComboBox2.SelectedIndex = 0;
                else toolStripComboBox2.SelectedIndex = 1;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float prev_x_location = 0;
            float prev_y_location = 0;
            Pen p = new Pen(Color.LimeGreen, 3);
            float prev_x_locationLow = 0;
            float prev_y_locationLow = 0;
            Pen pLow = new Pen(Color.CornflowerBlue, 3);
            float prev_x_locationHigh = 0;
            float prev_y_locationHigh = 0;
            Pen pHigh = new Pen(Color.IndianRed, 3);

            //if (_compressor == CompressorMap.None) return;
            try
            {
                switch (_compressor)
                {
                    case CompressorMap.GT17:
                        // set coordinates
                        x_offset = 42 * CalculateXCorrection();
                        y_offset = 539 * CalculateYCorrection();
                        x_multiplier = 10.67 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 166 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.T25_55:
                        // set coordinates
                        x_offset = 64 * CalculateXCorrection();
                        y_offset = 865 * CalculateYCorrection();
                        x_multiplier = 20 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 396 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.T25_60:
                        x_offset = 60 * CalculateXCorrection();
                        y_offset = 867 * CalculateYCorrection();
                        x_multiplier = 17.28 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 398 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT2871R:
                        x_offset = 50 * CalculateXCorrection();
                        y_offset = 595 * CalculateYCorrection();
                        x_multiplier = 9.56 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 276.5 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT28RS:
                        // set coordinates
                        x_offset = 55 * CalculateXCorrection();
                        y_offset = 460 * CalculateYCorrection();
                        x_multiplier = 8 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 211 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT30R:
                        // set coordinates
                        x_offset = 50 * CalculateXCorrection();
                        y_offset = 463 * CalculateYCorrection();
                        x_multiplier = 6.4 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 158 * CalculateYCorrection(); // per bar
                        break;
                    // 100 cfm = 6.91 lbs/m
                    case CompressorMap.TD04:
                        x_offset = 66 * CalculateXCorrection();
                        y_offset = 576 * CalculateYCorrection();
                        x_multiplier = 10.45 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 234.5 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.TD0416T:
                        x_offset = 64 * CalculateXCorrection();
                        y_offset = 573 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.27 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 233 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.TD0418T:
                        x_offset = 65 * CalculateXCorrection();
                        y_offset = 576 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.27 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 234 * CalculateYCorrection(); // per bar
                        /*                        x_offset = 126 * CalculateXCorrection();
                                                y_offset = 940 * CalculateYCorrection();
                                                // 648 - 126 = 0.3 m3/s = 635 cfm = 44 lbs/m
                                                // 940 - 256 = 2 bar 
                                                x_multiplier = 11.86 * CalculateXCorrection(); // per lbs/m
                                                y_multiplier = 342 * CalculateYCorrection(); // per bar*/
                        break;
                    case CompressorMap.TD0419T:
                        x_offset = 65 * CalculateXCorrection();
                        y_offset = 576 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.27 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 234 * CalculateYCorrection(); // per bar
                        /*x_offset = 228 * CalculateXCorrection();
                        y_offset = 1627 * CalculateYCorrection();
                        // 1102 - 228 = 0.3 m3/s = 635 cfm = 44 lbs/m
                        // 1627 - 468 = 2 bar 
                        x_multiplier = 19.86 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 579.5 * CalculateYCorrection(); // per bar*/
                        break;
                    case CompressorMap.TD0620G:
                        x_offset = 58 * CalculateXCorrection();
                        y_offset = 577 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.30 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 235 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT3071R86:
                        x_offset = 42 * CalculateXCorrection();
                        y_offset = 556 * CalculateYCorrection();
                        x_multiplier = 6.67 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 171 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT40R:
                        x_offset = 54 * CalculateXCorrection();
                        y_offset = 482 * CalculateYCorrection();
                        x_multiplier = 5.31 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 171 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.HX40W:
                        x_offset = 35 * CalculateXCorrection();
                        y_offset = 762 * CalculateYCorrection();
                        x_multiplier = 5.03 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 167 * CalculateYCorrection(); // per bar
                        break;
                        break;
                }
                PointF[] pnts = new PointF[16];
                PointF[] pntsLow = new PointF[16];
                PointF[] pntsHigh = new PointF[16];

                for (int i = 0; i < 16; i++)
                {
                    double rpm = Convert.ToDouble(rpm_points[i]);
                    double mReq = boost_request[i];
                    double displacement = 140;
                    double liters = 2;
                    switch (toolStripComboBox2.SelectedIndex)
                    {
                        case 0:
                            displacement = 122;
                            liters = 2;
                            break;
                        case 1:
                            displacement = 140;
                            liters = 2.3;
                            break;
                    }
                    double EVF = (displacement / 1728) * (rpm / 2);

                    double efficiency = GetEngineEfficiency((int)rpm);

                    try
                    {
                        double temp_eff = ConvertToDouble(toolStripTextBox2.Text);
                        if (temp_eff != 0)
                        {
                            efficiency = temp_eff / 100;
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }

                    double suckgrammin = rpm * efficiency * liters * 0.5 * 1.2041;
                    double suckpoundsmin = suckgrammin / 453.59237;
                    double mReqgrammin = rpm * mReq * 2 / 1000;
                    double mReqpoundsmin = mReqgrammin / 453.59237;
                    double intakeLoss = CalculateIntakeLoss((int)rpm);
                    double pAtmNom = 14.7;
                    double pAtmLow = 12.5;
                    double pAtmHigh = 15.4;
                    double MassFactor = mReqpoundsmin / suckpoundsmin;
                    double volAtmNom = MassFactor * pAtmNom / (pAtmNom - intakeLoss);
                    double volAtmLow = MassFactor * pAtmNom / (pAtmLow - intakeLoss);
                    double volAtmHigh = MassFactor * pAtmNom / (pAtmHigh - intakeLoss);
                    double pressureRatioAtmNom = ((pAtmNom - intakeLoss) * volAtmNom) / pAtmNom;
                    double pressureRatioAtmLow = ((pAtmLow - intakeLoss) * volAtmLow) / pAtmLow;
                    double pressureRatioAtmHigh = ((pAtmHigh - intakeLoss) * volAtmHigh) / pAtmHigh;
                    Console.WriteLine("Rpm: " + rpm.ToString() + " mReq: " + mReq.ToString() + " suckpoundsmin: " + suckpoundsmin.ToString() + " mReqpoundsmin: " + mReqpoundsmin.ToString());
                    double temperature = 20;

                    try
                    {
                        temperature = ConvertToDouble(toolStripTextBox1.Text);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    //temperature = ConvertToFahrenheit(temperature);

                    pressureRatioAtmNom *= (273 - 50 + temperature) / (273 - 50);
                    pressureRatioAtmLow *= (273 - 50 + temperature) / (273 - 50);
                    pressureRatioAtmHigh *= (273 - 50 + temperature) / (273 - 50);
                    pressureRatioAtmNom -= 1;
                    pressureRatioAtmLow -= 1;
                    pressureRatioAtmHigh -= 1;


                    // ATM
                    float x_location = (float)(x_offset + mReqpoundsmin * x_multiplier);
                    float y_location = (float)(y_offset - pressureRatioAtmNom * y_multiplier);
                    pnts[i].X = x_location;
                    pnts[i].Y = y_location;
                    if (i > 0)
                    {
                        // draw line
                        e.Graphics.DrawLine(p, new PointF(prev_x_location, prev_y_location), new PointF(x_location, y_location));
                    }
                    e.Graphics.FillEllipse(Brushes.Green, x_location - 4, y_location - 4, 8, 8);
                    prev_x_location = x_location;
                    prev_y_location = y_location;
                    // HIGH ALTITUDE
                    x_location = (float)(x_offset + mReqpoundsmin * x_multiplier);
                    y_location = (float)(y_offset - pressureRatioAtmLow * y_multiplier);
                    pntsLow[i].X = x_location;
                    pntsLow[i].Y = y_location;
                    if (i > 0)
                    {
                        // draw line
                        e.Graphics.DrawLine(pLow, new PointF(prev_x_locationLow, prev_y_locationLow), new PointF(x_location, y_location));
                    }
                    e.Graphics.FillEllipse(Brushes.Blue, x_location - 4, y_location - 4, 8, 8);
                    prev_x_locationLow = x_location;
                    prev_y_locationLow = y_location;
                    // High pressure
                    x_location = (float)(x_offset + mReqpoundsmin * x_multiplier);
                    y_location = (float)(y_offset - pressureRatioAtmHigh * y_multiplier);
                    pntsHigh[i].X = x_location;
                    pntsHigh[i].Y = y_location;
                    if (i > 0)
                    {
                        // draw line
                        e.Graphics.DrawLine(pHigh, new PointF(prev_x_locationHigh, prev_y_locationHigh), new PointF(x_location, y_location));
                    }
                    e.Graphics.FillEllipse(Brushes.Red, x_location - 4, y_location - 4, 8, 8);
                    prev_x_locationHigh = x_location;
                    prev_y_locationHigh = y_location;

                    /*
                    int rpm = rpm_points[i];
                    double boost_req = boost_request[i];
                    //to plot the point in the compressor map, first we need to calculate the flow of the engine
                    double displacement = 140;
                    switch (current_engineType)
                    {
                        case EngineType.Liter2:
                            displacement = 122;
                            break;
                        case EngineType.Liter23:
                            displacement = 140;
                            break;
                    }
                    double EVF = (displacement / 1728) * (rpm / 2);

                    double efficiency = GetEngineEfficiency(rpm);

                    try
                    {
                        double temp_eff = ConvertToDouble(toolStripTextBox2.Text);
                        if(temp_eff != 0)
                        {
                            efficiency = temp_eff / 100;
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    double temperature = 20;

                    try
                    {
                        temperature = ConvertToDouble(toolStripTextBox1.Text);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    temperature = ConvertToFahrenheit(temperature);
                    temperature = 460 + temperature; // to rankin
                    double airflowlbsm = (((14.5 + (boost_req * 14.5)) * EVF * 29) / (10.73 * temperature)) * efficiency;
                    // now we have airflow and pressure, we can draw after we calculate the points
                    float x_location = (float)(x_offset + airflowlbsm * x_multiplier);
                    float y_location = (float)(y_offset - boost_req * y_multiplier);
                    pnts[i].X = x_location;
                    pnts[i].Y = y_location;

                    if (i > 0)
                    {
                        // draw line
                        e.Graphics.DrawLine(p, new PointF(prev_x_location, prev_y_location), new PointF(x_location, y_location));
                    }
                    e.Graphics.FillEllipse(Brushes.OrangeRed, x_location-4, y_location-4, 8, 8);
                    prev_x_location = x_location;
                    prev_y_location = y_location;*/
                }
//                e.Graphics.DrawPolygon(p, pnts);
                e.Graphics.Save();
                p.Dispose();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private double CalculateIntakeLoss(int rpm)
        {
            double retval = 0.08;
            if (rpm < 880) retval = 0.08;
            else if (rpm < 1260) retval = 0.10;
            else if (rpm < 1640) retval = 0.17;
            else if (rpm < 2020) retval = 0.28;
            else if (rpm < 2400) retval = 0.42;
            else if (rpm < 2780) retval = 0.50;
            else if (rpm < 3160) retval = 0.58;
            else if (rpm < 3540) retval = 0.65;
            else if (rpm < 3920) retval = 0.74;
            else if (rpm < 4300) retval = 0.82;
            else if (rpm < 4680) retval = 0.92;
            else if (rpm < 5060) retval = 1.03;
            else if (rpm < 5440) retval = 1.07;
            else if (rpm < 5820) retval = 1.10;
            else if (rpm < 6000) retval = 1.08;
            else retval = 1.43;
            return retval;
        }

        private double ConvertToFahrenheit(double celcius)
        {
            //F =  C × 1.8 + 32
            double retval = (celcius * 1.8) + 32;
            return retval;
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }

        private double CalculateXCorrection()
        {
            double retval = 1;
            retval = (double)pictureBox1.Width / (double)ori_width;
            return retval;
        }
        private double CalculateYCorrection()
        {
            double retval = 1;
            retval = (double)pictureBox1.Height / (double)ori_height;
            return retval;
        }

        private double GetEngineEfficiency(int rpm)
        {
            // efficiency drops with increasing rpm

            double retval = 1.00;
            retval -= ((double)rpm / 100000) * 4;
            /*if (rpm >= 7000) retval = 0.65;
            else if (rpm >= 6000) retval = 0.70;
            else if (rpm >= 5000) retval = 0.75;
            else if (rpm >= 4000) retval = 0.80;
            else if (rpm >= 3000) retval = 0.85;
            else if (rpm >= 2000) retval = 0.90;
            else if (rpm >= 1000) retval = 0.85;*/
            return retval;
        }

        public void SetCompressorType(CompressorMap compressorMap)
        {
            string imagename = "T7.Compressormaps.t25_55_saab.gif";
            switch (compressorMap)
            {
                case CompressorMap.GT17:
                    imagename = "T7.Compressormaps.GT17.jpg";
                    toolStripComboBox1.SelectedIndex = 0;
                    break;
                case CompressorMap.T25_55:
                    imagename = "T7.Compressormaps.t25_55_saab.gif";
                    toolStripComboBox1.SelectedIndex = 1;
                    break;
                case CompressorMap.T25_60:
                    imagename = "T7.Compressormaps.t25-60trim.gif";
                    toolStripComboBox1.SelectedIndex = 2;
                    break;
                case CompressorMap.TD04:
                    imagename = "T7.Compressormaps.td04-15g-cfm.gif";
                    toolStripComboBox1.SelectedIndex = 3;
                    break;
                case CompressorMap.TD0416T:
                    imagename = "T7.Compressormaps.td04h-16t-cfm.gif";
                    toolStripComboBox1.SelectedIndex = 4;
                    break;
                case CompressorMap.TD0418T:
                    imagename = "T7.Compressormaps.td04h-18t-cfm.gif";
                    toolStripComboBox1.SelectedIndex = 5;
                    break;
                case CompressorMap.TD0419T:
                    imagename = "T7.Compressormaps.td04h-19t-cfm.gif";
                    toolStripComboBox1.SelectedIndex = 6;
                    break;
                case CompressorMap.TD0620G:
                    imagename = "T7.Compressormaps.td06h-20g-cfm.gif";
                    toolStripComboBox1.SelectedIndex = 7;
                    break;
                case CompressorMap.GT2871R:
                    imagename = "T7.Compressormaps.gt2871r-48.jpg";
                    toolStripComboBox1.SelectedIndex = 8;
                    break;
                case CompressorMap.GT28RS:
                    imagename = "T7.Compressormaps.gt28rscompress.gif";
                    toolStripComboBox1.SelectedIndex = 9;
                    break;
                case CompressorMap.GT3071R86:
                    imagename = "T7.Compressormaps.GT3071R86.jpg";
                    toolStripComboBox1.SelectedIndex = 10;
                    break;
                case CompressorMap.GT30R:
                    imagename = "T7.Compressormaps.gt30rcompress.gif";
                    toolStripComboBox1.SelectedIndex = 11;
                    break;
                case CompressorMap.GT40R:
                    imagename = "T7.Compressormaps.gt40rcompress.gif";
                    toolStripComboBox1.SelectedIndex = 12;
                    break;
                case CompressorMap.HX40W:
                    toolStripComboBox1.SelectedIndex = 13;
                    imagename = "T7.Compressormaps.hx40w.jpg";
                    break;
            }
            Bitmap bmp = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(imagename));
            ori_width = bmp.Width;
            ori_height = bmp.Height;
            pictureBox1.Image = bmp;
            _compressor = compressorMap;
            this.Invalidate();
        }

        private void tD0418TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.TD0418T);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void tD0419TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.TD0419T);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void gT3071r86ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.GT3071R86);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void gT40RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.GT40R);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void hX40wToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.HX40W);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CompressorMap map = (CompressorMap)toolStripComboBox1.SelectedIndex;
                if (map != _compressor)
                {
                    SetCompressorType(map);
                }
                if (onRefreshData != null)
                {
                    onRefreshData(this, EventArgs.Empty);
                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // cast refresh event
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        public void Redraw()
        {
            SetCompressorType(_compressor);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG images|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(sfd.FileName);
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Redraw();
            }
        }

        private void t1752ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.GT17);
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }
        }
    }
}
