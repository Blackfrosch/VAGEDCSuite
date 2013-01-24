using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using System.Runtime.InteropServices;
using Nevron.Chart;
using Nevron.GraphicsCore;
using Nevron.Chart.WinForm;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars.Docking;
using System.IO;
using System.Globalization;

namespace VAGSuite
{
    public enum ViewType : int
    {
        Hexadecimal = 0,
        Decimal,
        Easy,
        ASCII
    }

    public enum ViewSize : int
    {
        NormalView = 0,
        SmallView,
        ExtraSmallView
    }
    public enum XDFCategories : int
    {
        Undocumented = 0,
        Fuel,
        Ignition,
        Boost_control,
        Idle,
        Correction,
        Misc,
        Sensor,
        Runtime,
        Diagnostics
    }

    public enum XDFSubCategory : int
    {
        None = 0,
        Basic,
        Advanced,
        Undocumented,
        Startup,
        Enrichment,
        Idle,
        Axis,
        Basic_manual,
        Basic_automatic,
        Advanced_manual,
        Advanced_automatic,
        Warmup,
        Cranking,
        Temperature_compensation,
        Lambda_sensor,
        Transient,
        Airpump_control,
        Temperature_calculation,
        Limp_home,
        Misfire
    }

    public enum XDFUnits : int
    {
        Seconds,
        Milliseconds,
        Minutes,
        Degrees,
        DegF,   // Deg F
        DegC,   // Deg C
        RPM,
        MPH,
        KMPH,
        DC // %DC

    }
    public partial class MapViewerEx : DevExpress.XtraEditors.XtraUserControl //IMapViewer
    {


        private bool m_isDifferenceViewer = false;

        public bool IsDifferenceViewer
        {
            get { return m_isDifferenceViewer; }
            set
            {
                m_isDifferenceViewer = value;
                if (m_isDifferenceViewer)
                {
                    // disable buttons etc
                    simpleButton10.Visible = false;
                    simpleButton2.Visible = false;
                }
            }
        }

        public class AxisEditorRequestedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private string _filename;

            public string Filename
            {
                get { return _filename; }
                set { _filename = value; }
            }

            private AxisIdent _axisident;

            public AxisIdent Axisident
            {
                get { return _axisident; }
                set { _axisident = value; }
            }

            public AxisEditorRequestedEventArgs(AxisIdent ident, string mapname, string filename)
            {
                this._axisident = ident;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class ViewTypeChangedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private ViewType _view;

            public ViewType View
            {
                get { return _view; }
                set { _view = value; }
            }


            public ViewTypeChangedEventArgs(ViewType view, string mapname)
            {
                this._view = view;
                this._mapname = mapname;
            }
        }

        public class AxisLockEventArgs : System.EventArgs
        {
            private int _y_axis_max_value;
            private string _mapname;
            private string _filename;
            private int _lock_mode;

            public int AxisMaxValue
            {
                get
                {
                    return _y_axis_max_value;
                }
            }

            public int LockMode
            {
                get
                {
                    return _lock_mode;
                }
            }

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public AxisLockEventArgs(int max_value, int lockmode, string mapname, string filename)
            {
                this._y_axis_max_value = max_value;
                this._lock_mode = lockmode;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class SliderMoveEventArgs : System.EventArgs
        {
            private int _slider_position;
            private string _mapname;
            private string _filename;

            public int SliderPosition
            {
                get
                {
                    return _slider_position;
                }
            }

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public SliderMoveEventArgs(int slider_position, string mapname, string filename)
            {
                this._slider_position = slider_position;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class SplitterMovedEventArgs : System.EventArgs
        {
            private int _splitdistance;

            public int Splitdistance
            {
                get { return _splitdistance; }
                set { _splitdistance = value; }
            }


            private int _panel1height;

            public int Panel1height
            {
                get { return _panel1height; }
                set { _panel1height = value; }
            }
            private int _panel2height;

            public int Panel2height
            {
                get { return _panel2height; }
                set { _panel2height = value; }
            }
            private bool _panel1collapsed;

            public bool Panel1collapsed
            {
                get { return _panel1collapsed; }
                set { _panel1collapsed = value; }
            }
            private bool _panel2collapsed;

            public bool Panel2collapsed
            {
                get { return _panel2collapsed; }
                set { _panel2collapsed = value; }
            }

            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public SplitterMovedEventArgs(int panel1height, int panel2height, int splitdistance, bool panel1collapsed, bool panel2collapsed, string mapname)
            {
                this._splitdistance = splitdistance;
                this._panel1collapsed = panel1collapsed;
                this._panel1height = panel1height;
                this._panel2collapsed = panel2collapsed;
                this._panel2height = panel2height;
                this._mapname = mapname;
            }
        }

        public class CellSelectionChangedEventArgs : System.EventArgs
        {
            private int _rowhandle;

            public int Rowhandle
            {
                get { return _rowhandle; }
                set { _rowhandle = value; }
            }
            private int _colindex;

            public int Colindex
            {
                get { return _colindex; }
                set { _colindex = value; }
            }

            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public CellSelectionChangedEventArgs(int rowhandle, int colindex, string mapname)
            {
                this._rowhandle = rowhandle;
                this._colindex = colindex;
                this._mapname = mapname;
            }

        }

        public enum AxisIdent : int
        {
            X_Axis = 0,
            Y_Axis = 1,
            Z_Axis = 2
        }

        public class ReadFromSRAMEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public ReadFromSRAMEventArgs(string mapname)
            {
                this._mapname = mapname;
            }
        }

        public class WriteToSRAMEventArgs : System.EventArgs
        {
            private byte[] _data;

            public byte[] Data
            {
                get { return _data; }
                set { _data = value; }
            }

            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public WriteToSRAMEventArgs(string mapname, byte[] data)
            {
                this._mapname = mapname;
                this._data = data;
            }
        }

        public class ReadSymbolEventArgs : System.EventArgs
        {
            private string _mapname;
            private string _filename;

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }
            public ReadSymbolEventArgs(string mapname, string filename)
            {
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class SaveSymbolEventArgs : System.EventArgs
        {
            private int _address;
            private int _length;
            private byte[] _mapdata;
            private string _mapname;
            private string _filename;

            public int SymbolAddress
            {
                get
                {
                    return _address;
                }
            }

            public int SymbolLength
            {
                get
                {
                    return _length;
                }
            }
            public byte[] SymbolDate
            {
                get
                {
                    return _mapdata;
                }
            }
            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }
            public SaveSymbolEventArgs(int address, int length, byte[] mapdata, string mapname, string filename)
            {
                this._address = address;
                this._length = length;
                this._mapdata = mapdata;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class GraphSelectionChangedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private int _tabpageindex;

            public int Tabpageindex
            {
                get { return _tabpageindex; }
                set { _tabpageindex = value; }
            }


            public GraphSelectionChangedEventArgs(int tabpageindex, string mapname)
            {
                this._tabpageindex = tabpageindex;
                this._mapname = mapname;
            }
        }


        public class SurfaceGraphViewChangedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private int _pov_x;

            public int Pov_x
            {
                get { return _pov_x; }
                set { _pov_x = value; }
            }
            private int _pov_y;

            public int Pov_y
            {
                get { return _pov_y; }
                set { _pov_y = value; }
            }
            private int _pov_z;

            public int Pov_z
            {
                get { return _pov_z; }
                set { _pov_z = value; }
            }
            private int _pan_x;

            public int Pan_x
            {
                get { return _pan_x; }
                set { _pan_x = value; }
            }
            private int _pan_y;

            public int Pan_y
            {
                get { return _pan_y; }
                set { _pan_y = value; }
            }
            private double _pov_d;

            public double Pov_d
            {
                get { return _pov_d; }
                set { _pov_d = value; }
            }

            public SurfaceGraphViewChangedEventArgs(int povx, int povy, int povz, int panx, int pany, double povd, string mapname)
            {
                this._pan_x = panx;
                this._pan_y = pany;
                this._pov_d = povd;
                this._pov_x = povx;
                this._pov_y = povy;
                this._pov_z = povz;
                this._mapname = mapname;
            }
        }

        public class SurfaceGraphViewChangedEventArgsEx : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private float _depthx;

            public float DepthX
            {
                get { return _depthx; }
                set { _depthx = value; }
            }
            private float _depthy;

            public float DepthY
            {
                get { return _depthy; }
                set { _depthy = value; }
            }
            private float _zoom;

            public float Zoom
            {
                get { return _zoom; }
                set { _zoom = value; }
            }
            private float _rotation;

            public float Rotation
            {
                get { return _rotation; }
                set { _rotation = value; }
            }
            private float _elevation;

            public float Elevation
            {
                get { return _elevation; }
                set { _elevation = value; }
            }


            public SurfaceGraphViewChangedEventArgsEx(float depthx, float depthy, float zoom, float rotation, float elevation, string mapname)
            {
                this._depthx = depthx;
                this._depthy = depthy;
                this._zoom = zoom;
                this._rotation = rotation;
                this._elevation = elevation;
                this._mapname = mapname;
            }
        }

        public delegate void ViewerClose(object sender, EventArgs e);
        public event ViewerClose onClose;
        public delegate void AxisEditorRequested(object sender, AxisEditorRequestedEventArgs e);
        public event MapViewerEx.AxisEditorRequested onAxisEditorRequested;
        public delegate void ReadDataFromSRAM(object sender, ReadFromSRAMEventArgs e);
        public event MapViewerEx.ReadDataFromSRAM onReadFromSRAM;
        public delegate void WriteDataToSRAM(object sender, WriteToSRAMEventArgs e);
        public event MapViewerEx.WriteDataToSRAM onWriteToSRAM;
        public delegate void ViewTypeChanged(object sender, ViewTypeChangedEventArgs e);
        public event MapViewerEx.ViewTypeChanged onViewTypeChanged;
        public delegate void GraphSelectionChanged(object sender, GraphSelectionChangedEventArgs e);
        public event MapViewerEx.GraphSelectionChanged onGraphSelectionChanged;
        public delegate void SurfaceGraphViewChanged(object sender, SurfaceGraphViewChangedEventArgs e);
        public event MapViewerEx.SurfaceGraphViewChanged onSurfaceGraphViewChanged;
        public delegate void SurfaceGraphViewChangedEx(object sender, SurfaceGraphViewChangedEventArgsEx e);
        public event MapViewerEx.SurfaceGraphViewChangedEx onSurfaceGraphViewChangedEx;
        public delegate void NotifySaveSymbol(object sender, SaveSymbolEventArgs e);
        public event MapViewerEx.NotifySaveSymbol onSymbolSave;
        public delegate void NotifyReadSymbol(object sender, ReadSymbolEventArgs e);
        public event MapViewerEx.NotifyReadSymbol onSymbolRead;
        public delegate void SplitterMoved(object sender, SplitterMovedEventArgs e);
        public event MapViewerEx.SplitterMoved onSplitterMoved;
        public delegate void SelectionChanged(object sender, CellSelectionChangedEventArgs e);
        public event MapViewerEx.SelectionChanged onSelectionChanged;
        public delegate void NotifyAxisLock(object sender, AxisLockEventArgs e);
        public event MapViewerEx.NotifyAxisLock onAxisLock;
        public delegate void NotifySliderMove(object sender, SliderMoveEventArgs e);
        public event MapViewerEx.NotifySliderMove onSliderMove;
        
        private bool m_issixteenbit = false;
        private int m_TableWidth = 8;
        private bool m_datasourceMutated = false;
        private int m_MaxValueInTable = 0;
        private bool m_prohibitcellchange = false;
        private bool m_prohibitsplitchange = false;
        private bool m_prohibitgraphchange = false;
        private ViewType m_viewtype = ViewType.Hexadecimal;
        private ViewType m_previousviewtype = ViewType.Easy;
        private bool m_prohibit_viewchange = false;
        private bool m_trackbarBlocked = true;
        private ViewSize m_vs = ViewSize.NormalView;
        private bool m_OverlayVisible = true;
        private double m_realMaxValue = -65535;
        private double m_realMinValue = 65535;

        private double m_Yaxiscorrectionfactor = 1;
        private double m_Yaxiscorrectionoffset = 0;

        public double Yaxiscorrectionoffset
        {
            get { return m_Yaxiscorrectionoffset; }
            set { m_Yaxiscorrectionoffset = value; }
        }

        public double Yaxiscorrectionfactor
        {
            get { return m_Yaxiscorrectionfactor; }
            set { m_Yaxiscorrectionfactor = value; }
        }

        private string m_xaxisUnits = string.Empty;

        public string XaxisUnits
        {
            get { return m_xaxisUnits; }
            set
            {
                m_xaxisUnits = value.ToUpper();
            }
        }
        private string m_yaxisUnits = string.Empty;

        public string YaxisUnits
        {
            get { return m_yaxisUnits; }
            set
            {
                m_yaxisUnits = value.ToUpper();
            }
        }

        private double m_Xaxiscorrectionfactor = 1;
        private double m_Xaxiscorrectionoffset = 0;

        public double Xaxiscorrectionoffset
        {
            get { return m_Xaxiscorrectionoffset; }
            set { m_Xaxiscorrectionoffset = value; }
        }

        public double Xaxiscorrectionfactor
        {
            get { return m_Xaxiscorrectionfactor; }
            set { m_Xaxiscorrectionfactor = value; }
        }

        byte[] open_loop;

        public byte[] Open_loop
        {
            get { return open_loop; }
            set { open_loop = value; }
        }

        public void SetViewSize(ViewSize vs)
        {
            m_vs = vs; 
            if (vs == ViewSize.SmallView)
            {
                gridView1.PaintStyleName = "UltraFlat";
                gridView1.Appearance.Row.Font = new Font("Tahoma", 8);
                this.Font = new Font("Tahoma", 8);
                gridView1.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            else if (vs == ViewSize.ExtraSmallView)
            {
                gridView1.PaintStyleName = "UltraFlat";
                gridView1.Appearance.Row.Font = new Font("Tahoma", 7);
                this.Font = new Font("Tahoma", 7);
                gridView1.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }
        
        private SymbolCollection m_SymbolCollection = new SymbolCollection();

        public SymbolCollection mapSymbolCollection
        {
            get { return m_SymbolCollection; }
            set { m_SymbolCollection = value; }
        }

        public ViewType Viewtype
        {
            get { return m_viewtype; }
            set
            {
                m_viewtype = value;
                m_prohibit_viewchange = true;
                toolStripComboBox3.SelectedIndex = (int)m_viewtype;
                m_prohibit_viewchange = false;
            }
        }

        int[] afr_counter;

        public int[] Afr_counter
        {
            get { return afr_counter; }
            set { afr_counter = value; }
        }

        private void ShowHitInfo(GridHitInfo hi)
        {
            if (hi.InRowCell)
            {

                if (afr_counter != null)
                {
                    // fetch correct counter
                    int current_afrcounter = (int)afr_counter[(afr_counter.Length - ((hi.RowHandle + 1) * m_TableWidth)) + hi.Column.AbsoluteIndex];
                    // show number of measurements in balloon
                    string detailline = "# measurements: " + current_afrcounter.ToString();
                    toolTipController1.ShowHint(detailline, "Information", Cursor.Position);
                }
            }
            else
            {
                toolTipController1.HideHint();
            }
        }

        public int MaxValueInTable
        {
            get { return m_MaxValueInTable; }
            set { m_MaxValueInTable = value; }
        }

        public bool AutoSizeColumns
        {
            set
            {
                gridView1.OptionsView.ColumnAutoWidth = value;
            }
        }

        private bool m_disablecolors = false;

        public bool DisableColors
        {
            get
            {
                return m_disablecolors;
            }
            set
            {
                m_disablecolors = value;
                Invalidate();
            }
        }

        private string m_filename;
        //private bool m_isHexMode = true;
        private bool m_isRedWhite = false;
        private int m_textheight = 12;
        private string m_xformatstringforhex = "X4";
        private bool m_isDragging = false;
        private int _mouse_drag_x = 0;
        private int _mouse_drag_y = 0;
        private bool m_prohibitlock_change = false;

        private bool _isCompareViewer = false;

        public bool IsCompareViewer
        {
            get { return _isCompareViewer; }
            set
            {
                _isCompareViewer = value;
                if (_isCompareViewer)
                {
                    gridView1.OptionsBehavior.Editable = false; // don't let the user edit a compare viewer
                    toolStripButton3.Enabled = false;
                    toolStripTextBox1.Enabled = false;
                    toolStripComboBox1.Enabled = false;
                    smoothSelectionToolStripMenuItem.Enabled = false;
                    pasteSelectedCellsToolStripMenuItem.Enabled = false;
                    //exportMapToolStripMenuItem.Enabled = false;
                    simpleButton2.Enabled = false;
//                    simpleButton3.Enabled = false;
                    //btnSaveToRAM.Enabled = false;
                    //btnReadFromRAM.Enabled = false;
                }
            }
        }

        private bool m_tableVisible = false;

        public int SliderPosition
        {
            get { return (int)trackBarControl1.EditValue; }
            set { trackBarControl1.EditValue = value; }
        }

        public bool TableVisible
        {
            get { return m_tableVisible; }
            set
            {
                m_tableVisible = value;
                splitContainer1.Panel1Collapsed = !m_tableVisible;
            }
        }

        public int LockMode
        {
            get
            {
                return toolStripComboBox2.SelectedIndex;
            }
            set
            {
                m_prohibitlock_change = true;
                toolStripComboBox2.SelectedIndex = value;
                m_prohibitlock_change = false;
            }

        }

        private double _max_y_axis_value = 0;

        public double Max_y_axis_value
        {
            get
            {
                //_max_y_axis_value = ((XYDiagram)chartControl1.Diagram).AxisY.Range.MaxValueInternal;
                return _max_y_axis_value;
            }
            set { 
                _max_y_axis_value = value;
                /*if (_max_y_axis_value > 0)
                {
                    ((XYDiagram)chartControl1.Diagram).AxisY.Range.MaxValueInternal = (_max_y_axis_value + correction_offset) * correction_factor;
                }
                else 
                {
                    // set to autoscale
                    ((XYDiagram)chartControl1.Diagram).AxisY.Range.Auto = true;
                }*/
            }
        }

        private bool m_isRAMViewer = false;

        public bool IsRAMViewer
        {
            get { return m_isRAMViewer; }
            set
            {
                m_isRAMViewer = value;
                // Also when software is open
//                simpleButton8.Enabled = m_isRAMViewer;
//                simpleButton9.Enabled = m_isRAMViewer;
            }
        }

        private bool m_isOpenSoftware = false;


        public bool IsOpenSoftware
        {
            get { return m_isOpenSoftware; }
            set
            {
                m_isOpenSoftware = value;
                // Also when software is open
//                simpleButton8.Enabled = m_isOpenSoftware;
//                simpleButton9.Enabled = m_isOpenSoftware;
            }
        }

        private bool m_isUpsideDown = false;

        public bool IsUpsideDown
        {
            get { return m_isUpsideDown; }
            set { m_isUpsideDown = value; }
        }

        private double correction_factor = 1;

        public double Correction_factor
        {
            get { return correction_factor; }
            set { correction_factor = value; }
        }
        private double correction_offset = 0;

        public double Correction_offset
        {
            get { return correction_offset; }
            set { correction_offset = value; }
        }

        public bool GraphVisible
        {
            get
            {
                return !splitContainer1.Panel2Collapsed;
            }
            set
            {
                splitContainer1.Panel2Collapsed = !value;
            }
        }
        private void btnToggleOverlay_Click(object sender, EventArgs e)
        {
            // do/don't show the graph overlay
            m_OverlayVisible = !m_OverlayVisible;
            RefreshMeshGraph();

        }

        private void RefreshMeshGraph()
        {
            try
            {
                NChart chart = nChartControl1.Charts[0];
                NMeshSurfaceSeries surface = null;
                NMeshSurfaceSeries surface2 = null;
                NMeshSurfaceSeries surface3 = null;
                if (chart.Series.Count == 0)
                {
                    surface = (NMeshSurfaceSeries)chart.Series.Add(SeriesType.MeshSurface);
                }
                else
                {
                    surface = (NMeshSurfaceSeries)chart.Series[0];
                    if (chart.Series.Count > 1)
                    {
                        surface2 = (NMeshSurfaceSeries)chart.Series[1];
                        if (chart.Series.Count > 2)
                        {
                            surface3 = (NMeshSurfaceSeries)chart.Series[2];
                        }
                    }
                }

                //SetPallete();

                FillData(surface);
                // hier
                if (surface2 != null)
                {
                    surface2.Palette.Clear();
                    surface2.Palette.Add(-255, Color.YellowGreen);
                    surface2.Palette.Add(255, Color.YellowGreen);
                    surface2.AutomaticPalette = false;
                    //surface2.FillStyle = new NColorFillStyle(Color.YellowGreen);
                    //surface2.FillMode = SurfaceFillMode.CustomColors;
                    FillDataOriginal(surface2);
                    if (!m_OverlayVisible)
                    {
                        surface2.Visible = false;
                    }
                    else
                    {
                        surface2.Visible = true;
                    }

                }
                if (surface3 != null)
                {
                    surface3.Palette.Clear();
                    surface3.Palette.Add(-255, Color.BlueViolet);
                    surface3.Palette.Add(255, Color.BlueViolet);
                    surface3.AutomaticPalette = false;
                    //surface3.FillStyle = new NColorFillStyle(Color.BlueViolet);
                    //surface3.FillMode = SurfaceFillMode.CustomColors;

                    FillDataCompare(surface3);
                    if (!m_OverlayVisible)
                    {
                        surface3.Visible = false;
                    }
                }

                nChartControl1.Refresh();
                Console.WriteLine("Chartcontrol refreshed");
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to refresh mesh chart: " + E.Message);
            }
        }

        double now_realMaxValue = double.MinValue;
        double now_realMinValue = double.MaxValue;

        private void FillData(NMeshSurfaceSeries surface)
        {
            try
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                int rowcount = 0;
                now_realMaxValue = double.MinValue;
                now_realMinValue = double.MaxValue;
                //surface.Data.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    for (int t = 0; t < dt.Columns.Count; t++)
                    {
                        double value = 0;
                        if (m_viewtype == ViewType.Easy || m_viewtype == ViewType.Decimal)
                        {
                            value = Convert.ToInt32(dr[t]);
                        }
                        else if (m_viewtype == ViewType.Hexadecimal)
                        {
                            value = Convert.ToInt32(dr[t].ToString(), 16);

                        }
                        if (m_viewtype != ViewType.Decimal && m_viewtype != ViewType.Hexadecimal && m_viewtype != ViewType.ASCII)
                        {
                            value *= correction_factor;
                            if (!_isCompareViewer) value += correction_offset;
                        }
                        surface.Data.SetValue(rowcount, t, value, /*y_axisvalues.GetValue(rowcount)*/ rowcount, /*x_axisvalues.GetValue(t)*/ t);
                        if (value > now_realMaxValue) now_realMaxValue = value;
                        if (value < now_realMinValue) now_realMinValue = value;
                    }
                    rowcount++;
                }
                if (now_realMaxValue != double.MinValue)
                {
                    surface.Palette.Clear();
                    double diff = now_realMaxValue - now_realMinValue;
                    if (m_OnlineMode)
                    {
                        surface.Palette.Add(now_realMinValue, Color.Wheat);
                        surface.Palette.Add(now_realMinValue + 0.25 * diff, Color.LightBlue);
                        surface.Palette.Add(now_realMinValue + 0.50 * diff, Color.SteelBlue);
                        surface.Palette.Add(now_realMinValue + 0.75 * diff, Color.Blue);
                        surface.Palette.Add(now_realMinValue + diff, Color.DarkBlue);

                    }
                    else
                    {
                        surface.Palette.Add(now_realMinValue, Color.Green);
                        surface.Palette.Add(now_realMinValue + 0.25 * diff, Color.Yellow);
                        surface.Palette.Add(now_realMinValue + 0.50 * diff, Color.Orange);
                        surface.Palette.Add(now_realMinValue + 0.75 * diff, Color.OrangeRed);
                        surface.Palette.Add(now_realMinValue + diff, Color.Red);
                    }
                    surface.PaletteSteps = 4;
                    surface.AutomaticPalette = false;
                }
                /*if (m_issixteenbit)
                {
                    for (int tel = 0; tel < m_map_content.Length; tel+=2)
                    {
                    
                    }
                }
                else
                {
                    for (int tel = 0; tel < m_map_content.Length; tel++)
                    {
                        int _y = tel / m_TableWidth;
                        int _x = tel % m_TableWidth;
                        surface.Data.SetValue(_x, _y, Convert.ToInt32(m_map_content[tel]));
                    }
                }*/
            }
            catch (Exception E)
            {
                Console.WriteLine("FillData: " + E.Message);
            }
        }

        private void FillDataOriginal(NMeshSurfaceSeries surface)
        {
            try
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                int rowcount = dt.Rows.Count;
                int colcount = dt.Columns.Count;
                for (int row = 0; row < rowcount; row++)
                {
                    for (int col = 0; col < colcount; col++)
                    {
                        try
                        {
                            if (m_issixteenbit)
                            {
                                int indexinmap = ((row * colcount) + col) * 2;
                                Int32 ivalue = Convert.ToInt32(m_map_original_content[indexinmap]) * 256;
                                ivalue += Convert.ToInt32(m_map_original_content[indexinmap + 1]);

                                //Int32 diffivalue = Convert.ToInt32(m_map_content[indexinmap]) * 256;
                                //diffivalue += Convert.ToInt32(m_map_content[indexinmap + 1]);
                                //if (diffivalue != 0)
                                {
                                    if (ivalue > 32000)
                                    {
                                        ivalue = 65536 - ivalue;
                                        ivalue = -ivalue;
                                    }

                                    double value = ivalue;
                                    if (m_viewtype != ViewType.Decimal && m_viewtype != ViewType.Hexadecimal && m_viewtype != ViewType.ASCII)
                                    {
                                        value *= correction_factor;
                                        value += correction_offset; // bij origineel wel doen
                                    }
                                    surface.Data.SetValue((rowcount - 1) - row, col, value, (rowcount - 1) - row, col);
                                }
                                //Console.WriteLine(surface.Name + ": " + row.ToString() + " " + col.ToString() + " value: " + value.ToString());
                            }
                            else
                            {

                                int indexinmap = ((row * colcount) + col);
                                Int32 ivalue = Convert.ToInt32(m_map_original_content[indexinmap]);
                                //Int32 diffivalue = Convert.ToInt32(m_map_content[indexinmap]);
                                //if (diffivalue != 0)
                                {
                                    double value = ivalue;
                                    if (m_viewtype != ViewType.Decimal && m_viewtype != ViewType.Hexadecimal && m_viewtype != ViewType.ASCII)
                                    {
                                        value *= correction_factor;
                                        value += correction_offset;
                                    }
                                    surface.Data.SetValue((rowcount - 1) - row, col, value, (rowcount - 1) - row, col);
                                }
                                //Console.WriteLine(surface.Name + ": " + row.ToString() + " " + col.ToString() + " value: " + value.ToString());

                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Failed to fill data for original map: " + E.Message);
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("FilleDataOriginal: " + E.Message);
            }
        }

        private void FillDataCompare(NMeshSurfaceSeries surface)
        {
            DataTable dt = (DataTable)gridControl1.DataSource;
            int rowcount = dt.Rows.Count;
            int colcount = dt.Columns.Count;
            for (int row = 0; row < rowcount; row++)
            {
                for (int col = colcount - 1; col >= 0; col--)
                {
                    if (m_issixteenbit)
                    {
                        int indexinmap = ((row * colcount) + col) * 2;
                        Int32 ivalue = Convert.ToInt32(m_map_compare_content[indexinmap]) * 256;
                        ivalue += Convert.ToInt32(m_map_compare_content[indexinmap + 1]);
                        //Int32 diffivalue = Convert.ToInt32(m_map_content[indexinmap]) * 256;
                        //diffivalue += Convert.ToInt32(m_map_content[indexinmap + 1]);
                        //if (diffivalue != 0)
                        {
                            if (ivalue > 32000)
                            {
                                ivalue = 65536 - ivalue;
                                ivalue = -ivalue;
                            }
                            double value = ivalue;
                            if (m_viewtype != ViewType.Decimal && m_viewtype != ViewType.Hexadecimal && m_viewtype != ViewType.ASCII)
                            {
                                value *= correction_factor;
                                value += correction_offset;
                            }
                            surface.Data.SetValue((rowcount - 1) - row, col, value, (rowcount - 1) - row, col);
                        }
                        //Console.WriteLine(surface.Name + ": " + row.ToString() + " " + col.ToString() + " value: " + value.ToString());

                    }
                    else
                    {

                        int indexinmap = ((row * colcount) + col);
                        Int32 ivalue = Convert.ToInt32(m_map_compare_content[indexinmap]);
                        //Int32 diffivalue = Convert.ToInt32(m_map_content[indexinmap]);
                        //if (diffivalue != 0)
                        {
                            double value = ivalue;
                            if (m_viewtype != ViewType.Decimal && m_viewtype != ViewType.Hexadecimal && m_viewtype != ViewType.ASCII)
                            {
                                value *= correction_factor;
                                value += correction_offset;
                            }
                            surface.Data.SetValue((rowcount - 1) - row, col, value, (rowcount - 1) - row, col);
                        }
                    }
                }
            }
        }

        public bool IsRedWhite
        {
            get { return m_isRedWhite; }
            set { m_isRedWhite = value; }
        }

        /*public bool IsHexMode
        {
            get { return m_isHexMode; }
            set { m_isHexMode = value; }
        }*/

        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }


        public bool DatasourceMutated
        {
            get { return m_datasourceMutated; }
            set { m_datasourceMutated = value; }
        }
        private bool m_SaveChanges = false;

        public bool SaveChanges
        {
            get { return m_SaveChanges; }
            set { m_SaveChanges = value; }
        }
        private byte[] m_map_content;

        public byte[] Map_content
        {
            get { return m_map_content; }
            set { m_map_content = value; }
        }

        private byte[] m_map_compare_content;

        public byte[] Map_compare_content
        {
            get { return m_map_compare_content; }
            set { m_map_compare_content = value; }
        }

        private byte[] m_map_original_content;

        public byte[] Map_original_content
        {
            get { return m_map_original_content; }
            set { m_map_original_content = value; }
        }
        private bool m_UseNewCompare = false;

        public bool UseNewCompare
        {
            get { return m_UseNewCompare; }
            set { m_UseNewCompare = value; }
        }

        private bool m_OnlineMode = false;

        public bool OnlineMode
        {
            get { return m_OnlineMode; }
            set
            {
                m_OnlineMode = value;
                Console.WriteLine("RefreshMeshGraph on online mode");
                if (m_OnlineMode)
                {
                    RefreshMeshGraph();
                    UpdateChartControlSlice(GetDataFromGridView(false));
                    ShowTable(m_TableWidth, m_issixteenbit);
                }
                else
                {
                    RefreshMeshGraph();
                    UpdateChartControlSlice(GetDataFromGridView(false));
                    ShowTable(m_TableWidth, m_issixteenbit);
                }
            }
        }

        private bool _autoUpdateIfSRAM = false;

        public bool AutoUpdateIfSRAM
        {
            get { return _autoUpdateIfSRAM; }
            set { _autoUpdateIfSRAM = value; }
        }

        private int _autoUpdateInterval = 20;

        public int AutoUpdateInterval
        {
            get { return _autoUpdateInterval; }
            set { _autoUpdateInterval = value; }
        }

        private Int32 m_map_address = 0;

        public Int32 Map_address
        {
            get { return m_map_address; }
            set { m_map_address = value; }
        }


        private Int32 m_map_sramaddress = 0;

        public Int32 Map_sramaddress
        {
            get { return m_map_sramaddress; }
            set { m_map_sramaddress = value; }
        }
        private Int32 m_map_length = 0;

        public Int32 Map_length
        {
            get { return m_map_length; }
            set { m_map_length = value; }
        }
        private string m_map_name = string.Empty;

        public string Map_name
        {
            get { return m_map_name; }
            set
            {
                m_map_name = value;
                this.Text = "Table details [" + m_map_name + "]";
                //groupControl1.Text = "Symbol data [" + m_map_name + "] " + m_z_axis_name;
                SetGroupText();
            }
        }

        private void SetGroupText()
        {
            groupControl1.Text = "X: " + m_x_axis_name + " Y: " + m_y_axis_name + " Z: " + m_z_axis_name;
        }

        private string m_map_descr = string.Empty;

        public string Map_descr
        {
            get { return m_map_descr; }
            set
            {
                m_map_descr = value;
            }
        }

        private XDFCategories m_map_cat = XDFCategories.Undocumented;

        public XDFCategories Map_cat
        {
            get { return m_map_cat; }
            set
            {
                m_map_cat = value;
                //.Text = m_map_descr;
            }
        }

        private string m_x_axis_name = string.Empty;

        public string X_axis_name
        {
            get { return m_x_axis_name; }
            set
            {
                m_x_axis_name = value;
                SetGroupText();
            }
        }
        private string m_y_axis_name = string.Empty;

        public string Y_axis_name
        {
            get { return m_y_axis_name; }
            set
            {
                m_y_axis_name = value;
                SetGroupText();
            }
        }
        private string m_z_axis_name = string.Empty;

        public string Z_axis_name
        {
            get { return m_z_axis_name; }
            set
            {
                m_z_axis_name = value;
                //groupControl1.Text = "Symbol data [" + m_map_name + "] " + m_z_axis_name;
                //groupControl1.Text = "Symbol data " + m_z_axis_name;
                SetGroupText();
            }
        }

        private int x_axisAddress = 0;

        public int X_axisAddress
        {
            get { return x_axisAddress; }
            set { x_axisAddress = value; }
        }
        private int y_axisAddress = 0;

        public int Y_axisAddress
        {
            get { return y_axisAddress; }
            set { y_axisAddress = value; }
        }


        private int[] x_axisvalues;

        public int[] X_axisvalues
        {
            get { return x_axisvalues; }
            set { x_axisvalues = value; }
        }
        private int[] y_axisvalues;

        public int[] Y_axisvalues
        {
            get { return y_axisvalues; }
            set { y_axisvalues = value; }
        }

        /*public delegate void AxisEditorRequested(object sender, AxisEditorRequestedEventArgs e);
        public event MapViewerEx.AxisEditorRequested onAxisEditorRequested;

        public delegate void ReadDataFromSRAM(object sender, ReadFromSRAMEventArgs e);
        public event MapViewerEx.ReadDataFromSRAM onReadFromSRAM;
        public delegate void WriteDataToSRAM(object sender, WriteToSRAMEventArgs e);
        public event MapViewerEx.WriteDataToSRAM onWriteToSRAM;

        public delegate void ViewTypeChanged(object sender, ViewTypeChangedEventArgs e);
        public event MapViewerEx.ViewTypeChanged onViewTypeChanged;


        public delegate void GraphSelectionChanged(object sender, GraphSelectionChangedEventArgs e);
        public event MapViewerEx.GraphSelectionChanged onGraphSelectionChanged;

        public delegate void SurfaceGraphViewChanged(object sender, SurfaceGraphViewChangedEventArgs e);
        public event MapViewerEx.SurfaceGraphViewChanged onSurfaceGraphViewChanged;

        public delegate void NotifySaveSymbol(object sender, SaveSymbolEventArgs e);
        public event MapViewerEx.NotifySaveSymbol onSymbolSave;

        public delegate void SplitterMoved(object sender, SplitterMovedEventArgs e);
        public event MapViewerEx.SplitterMoved onSplitterMoved;

        public delegate void SelectionChanged(object sender, CellSelectionChangedEventArgs e);
        public event MapViewerEx.SelectionChanged onSelectionChanged;

        public delegate void NotifyAxisLock(object sender, AxisLockEventArgs e);
        public event MapViewerEx.NotifyAxisLock onAxisLock;

        public delegate void NotifySliderMove(object sender, SliderMoveEventArgs e);
        public event MapViewerEx.NotifySliderMove onSliderMove;
        
        public delegate void ViewerClose(object sender, EventArgs e);
        public event MapViewerEx.ViewerClose onClose;*/

        public MapViewerEx()
        {
            InitializeComponent();
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;

            nChartControl1.MouseWheel += new MouseEventHandler(nChartControl1_MouseWheel);
            nChartControl1.MouseDown += new MouseEventHandler(nChartControl1_MouseDown);
            nChartControl1.MouseUp += new MouseEventHandler(nChartControl1_MouseUp);

            gridView1.MouseMove += new MouseEventHandler(gridView1_MouseMove);
            
            /*string[] datamembers = new string[1];
            chartControl1.Series[0].ValueDataMembers.Clear();
            datamembers.SetValue("Y", 0);
            
            chartControl1.Series[0].ValueDataMembers.AddRange(datamembers);
            timer4.Enabled = false;*/
/*            chartControl1.Series[0].ArgumentDataMember = "X";*/

        }

        void gridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
            {
                ShowHitInfo(gridView1.CalcHitInfo(new Point(e.X, e.Y)));
            }
        }

        void nChartControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                nChartControl1.Controller.Tools.Clear();
                NDragTool dragTool = null;
                dragTool = new NTrackballTool();
                nChartControl1.Controller.Tools.Add(dragTool);
            }
            //<GS-07062010>
            CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);


        }

        private void CastSurfaceGraphChangedEventEx(float xdepth, float ydepth, float zoom, float rotation, float elevation)
        {
            if (onSurfaceGraphViewChangedEx != null)
            {
                if (!m_prohibitgraphchange)
                {
                    onSurfaceGraphViewChangedEx(this, new SurfaceGraphViewChangedEventArgsEx(xdepth, ydepth, zoom, rotation, elevation, m_map_name));
                }
            }
        }


        void nChartControl1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        void nChartControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                nChartControl1.Controller.Tools.Clear();
                NDragTool dragTool = null;
                dragTool = new NOffsetTool();
                nChartControl1.Controller.Tools.Add(dragTool);
                //dragTool.BeginDragMouseCommand.MouseButton = MouseButtons.Right;
                //dragTool.EndDragMouseCommand.MouseButton = MouseButtons.Right;
            }

        }

        void nChartControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            // wieltje wordt zoomin/out
            if (e.Delta > 0)
            {
                nChartControl1.Charts[0].Projection.Zoom += 5;
                nChartControl1.Refresh();
            }
            else
            {
                nChartControl1.Charts[0].Projection.Zoom -= 5;
                nChartControl1.Refresh();
            }
            //<GS-07062010>
            CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);
        }


        public bool SaveData()
        {
            bool retval = false;
            if (simpleButton2.Enabled)
            {
                simpleButton2_Click(this, EventArgs.Empty);
                retval = true;
            }
            return retval;
        }


        public void ShowTable(int tablewidth, bool issixteenbits)
        {
            double m_realValue;

            m_MaxValueInTable = 0;
            //if (m_isHexMode)
            if (m_viewtype == ViewType.Hexadecimal)
            {
                int lenvals = m_map_length;
                if (issixteenbits) lenvals /= 2;
            }
            else
            {
                int lenvals = m_map_length;
                if (issixteenbits) lenvals /= 2;
            }


            m_TableWidth = tablewidth;
            m_issixteenbit = issixteenbits;
            DataTable dt = new DataTable();
            if (m_map_length != 0 && m_map_name != string.Empty)
            {

                if (tablewidth > 0)
                {
                    int numberrows = (int)(m_map_length / tablewidth);
                    if (issixteenbits) numberrows /= 2;
                    int map_offset = 0;
                    /* if (numberrows > 0)
                     {*/
                    // aantal kolommen = 8

                    dt.Columns.Clear();
                    for (int c = 0; c < tablewidth; c++)
                    {
                        dt.Columns.Add(c.ToString());
                    }

                    if (issixteenbits)
                    {

                        for (int i = 0; i < numberrows; i++)
                        {
                            //ListViewItem lvi = new ListViewItem();
                            //lvi.UseItemStyleForSubItems = false;
                            object[] objarr = new object[tablewidth];
                            int b;
                            for (int j = 0; j < tablewidth; j++)
                            {
                                b = (byte)m_map_content.GetValue(map_offset++);
                                b *= 256;
                                b += (byte)m_map_content.GetValue(map_offset++);
                                /*if ( (b & 0x8000) > 0)
                                {
                                    //valtot = 0x10000 - valtot;
                                    b &= 0x7FFF;
                                    b = -b;
                                }*/
                                if (b > 0xF000)
                                {
                                    //b ^= 0xFFFF;
                                    b = 0x10000 - b;
                                    b = -b;
                                }
                                // TEST!!!
                                /* else if ((b & 0x8000) > 0)
                                 {
                                     //valtot = 0x10000 - valtot;
                                     b &= 0x7FFF;
                                     b = -b;
                                 }*/

                                
                                if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                m_realValue = b;
                                m_realValue *= correction_factor;
                                if (!_isCompareViewer) m_realValue += correction_offset;
                                if (m_realValue > m_realMaxValue) m_realMaxValue = m_realValue;
                                if (m_realValue < m_realMinValue) m_realMinValue = m_realValue;
                                // TEST
                                //b = (int)(correction_factor * (double)b);
                                //b = (int)(correction_offset + (double)b);
                                // TEST
                                //if (m_isHexMode)
                                if (m_viewtype == ViewType.Hexadecimal)
                                {
                                    objarr.SetValue(b.ToString("X4"), j);
                                }
                                else if (m_viewtype == ViewType.ASCII)
                                {
                                    //objarr.SetValue(b.ToString("X4"), j);
                                    // show as ascii characters
                                    try
                                    {
                                        objarr.SetValue(Convert.ToChar(b), j);
                                    }
                                    catch (Exception E)
                                    {
                                        Console.WriteLine("Failed to convert to ascii: " + E.Message);
                                        objarr.SetValue(Convert.ToChar(0x20), j);
                                    }
                                }
                                else
                                {
                                    objarr.SetValue(b.ToString(), j);
                                }
                                //lvi.SubItems.Add(b.ToString("X4"));
                                //lvi.SubItems[j + 1].BackColor = Color.FromArgb((int)(b / 256), 255 - (int)(b / 256), 0);
                            }
                            //listView2.Items.Add(lvi);
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {
                                dt.Rows.Add(objarr);
                            }
                        }
                        // en dan het restant nog in een nieuwe rij zetten
                        if (map_offset < m_map_length)
                        {
                            object[] objarr = new object[tablewidth];
                            int b;
                            int sicnt = 0;
                            for (int v = map_offset; v < m_map_length - 1; v++)
                            {
                                //b = (byte)m_map_content.GetValue(map_offset++);
                                if (map_offset <= m_map_content.Length - 1)
                                {
                                    b = (byte)m_map_content.GetValue(map_offset++);
                                    b *= 256;
                                    b += (byte)m_map_content.GetValue(map_offset++);
                                    if (b > 0xF000)
                                    {
                                        //                                        b ^= 0xFFFF;
                                        b = 0x10000 - b;

                                        b = -b;
                                    }
                                    /*if (b > 0x10000)
                                    {
                                        b ^= 0xFFFF;
                                        b = -b;
                                    }*/
                                    // TEST!!!
                                    /*  else if ((b & 0x8000) > 0)
                                      {
                                          //valtot = 0x10000 - valtot;
                                          b &= 0x7FFF;
                                          b = -b;
                                      }*/
                                    /*if ((b & 0x8000) > 0)
                                    {
                                        //valtot = 0x10000 - valtot;
                                        b &= 0x7FFF;
                                        b = -b;
                                    }*/
                                    if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                    m_realValue = b;
                                    m_realValue *= correction_factor;
                                    if (!_isCompareViewer) m_realValue += correction_offset;
                                    if (m_realValue > m_realMaxValue) m_realMaxValue = m_realValue;
                                    if (m_realValue < m_realMinValue) m_realMinValue = m_realValue;
                                    // TEST
                                    //b = (int)(correction_factor * (double)b);
                                    //b = (int)(correction_offset + (double)b);

                                    // TEST

                                    //                                    if (m_isHexMode)
                                    if (m_viewtype == ViewType.Hexadecimal)
                                    {
                                        objarr.SetValue(b.ToString("X4"), sicnt);
                                    }
                                    else if (m_viewtype == ViewType.ASCII)
                                    {
                                        //objarr.SetValue(b.ToString("X4"), j);
                                        // show as ascii characters
                                        objarr.SetValue(Convert.ToChar(b), sicnt);
                                    }
                                    else
                                    {
                                        objarr.SetValue(b.ToString(), sicnt);
                                    }
                                }
                                sicnt++;
                                //lvi.SubItems[lvi.SubItems.Count - 1].BackColor = Color.FromArgb((int)(b / 256), 255 - (int)(b / 256), 0);
                            }
                            /*for (int t = 0; t < (tablewidth - 1) - sicnt; t++)
                            {
                                lvi.SubItems.Add("");
                            }*/
                            //listView2.Items.Add(lvi);
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {

                                dt.Rows.Add(objarr);
                            }
                        }

                    }
                    else
                    {

                        for (int i = 0; i < numberrows; i++)
                        {
                            object[] objarr = new object[tablewidth];
                            int b;
                            for (int j = 0; j < (tablewidth); j++)
                            {
                                b = (byte)m_map_content.GetValue(map_offset++);
                                if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                m_realValue = b;
                                m_realValue *= correction_factor;
                                if (!_isCompareViewer) m_realValue += correction_offset;
                                if (m_realValue > m_realMaxValue) m_realMaxValue = m_realValue;
                                if (m_realValue < m_realMinValue) m_realMinValue = m_realValue;
                                // TEST
                                //b = (byte)(correction_factor * (double)b);
                                //b = (byte)(correction_offset + (double)b);

                                // TEST

                                //if (m_isHexMode)
                                if (m_viewtype == ViewType.Hexadecimal)
                                {
                                    objarr.SetValue(b.ToString("X2"), j);
                                }
                                else if (m_viewtype == ViewType.ASCII)
                                {
                                    //objarr.SetValue(b.ToString("X4"), j);
                                    // show as ascii characters
                                    objarr.SetValue(Convert.ToChar(b), j);
                                }
                                else
                                {
                                    objarr.SetValue(b.ToString(), j);
                                }
                            }
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {

                                dt.Rows.Add(objarr);
                            }
                        }
                        // en dan het restant nog in een nieuwe rij zetten
                        if (map_offset < m_map_length)
                        {
                            object[] objarr = new object[tablewidth];
                            byte b;
                            int sicnt = 0;
                            for (int v = map_offset; v < m_map_length /*- 1*/; v++)
                            {
                                b = (byte)m_map_content.GetValue(map_offset++);
                                if (b > m_MaxValueInTable) m_MaxValueInTable = b;
                                m_realValue = b;
                                m_realValue *= correction_factor;
                                if (!_isCompareViewer) m_realValue += correction_offset;
                                if (m_realValue > m_realMaxValue) m_realMaxValue = m_realValue;
                                if (m_realValue < m_realMinValue) m_realMinValue = m_realValue;
                                // TEST
                                //b = (byte)(correction_factor * (double)b);
                                //b = (byte)(correction_offset + (double)b);

                                // TEST

                                //if (m_isHexMode)
                                if (m_viewtype == ViewType.Hexadecimal)
                                {
                                    objarr.SetValue(b.ToString("X2"), sicnt);
                                }
                                else if (m_viewtype == ViewType.ASCII)
                                {
                                    //objarr.SetValue(b.ToString("X4"), j);
                                    // show as ascii characters
                                    objarr.SetValue(Convert.ToChar(b), sicnt);
                                }
                                else
                                {
                                    objarr.SetValue(b.ToString(), sicnt);
                                }
                                sicnt++;
                            }
                            if (m_isUpsideDown)
                            {
                                System.Data.DataRow r = dt.NewRow();
                                r.ItemArray = objarr;
                                dt.Rows.InsertAt(r, 0);
                            }
                            else
                            {

                                dt.Rows.Add(objarr);
                            }
                        }
                    }
                }

                gridControl1.DataSource = dt;

                if (!gridView1.OptionsView.ColumnAutoWidth)
                {
                    for (int c = 0; c < gridView1.Columns.Count; c++)
                    {
                        gridView1.Columns[c].Width = 40;
                    }
                }


                // set axis
                // scale to 3 bar?
                int indicatorwidth = -1;
                for (int i = 0; i < y_axisvalues.Length; i++)
                {
                    string yval = Convert.ToInt32(y_axisvalues.GetValue(i)).ToString();
                    if (m_viewtype == ViewType.Hexadecimal)
                    {
                        yval = Convert.ToInt32(y_axisvalues.GetValue(i)).ToString("X4");
                    }
                    if (m_y_axis_name == "MAP")
                    {
                    }
                    Graphics g = gridControl1.CreateGraphics();
                    SizeF size = g.MeasureString(yval, this.Font);
                    if (size.Width > indicatorwidth) indicatorwidth = (int)size.Width;
                    m_textheight = (int)size.Height;
                    g.Dispose();

                }
                if (indicatorwidth > 0)
                {
                    gridView1.IndicatorWidth = indicatorwidth + 6; // keep margin
                }

                int maxxval = 0;
                for (int i = 0; i < x_axisvalues.Length; i++)
                {
                    int xval = Convert.ToInt32(x_axisvalues.GetValue(i));
                    if (xval > 0xF000)
                    {
                        //xval ^= 0xFFFF;
                        xval = 0x10000 - xval;
                        xval = -xval;
                        x_axisvalues.SetValue(xval, i);
                    }

                    /* if (xval > 0x10000)
                     {
                         xval ^= 0xFFFF;
                         xval = -xval;
                     }
                     else if ((xval & 0x8000) > 0)
                     {
                         //valtot = 0x10000 - valtot;
                         xval &= 0x7FFF;
                         xval = -xval;
                     }*/

                    if (xval > maxxval) maxxval = xval;
                }
                if (maxxval <= 255) m_xformatstringforhex = "X2";
            }

            if (m_TableWidth > 1)
            {
                xtraTabControl1.SelectedTabPage = xtraTabPage1;
               
                SetViewTypeParams(m_vs);
                trackBarControl1.Properties.Minimum = 0;
                trackBarControl1.Properties.Maximum = x_axisvalues.Length - 1;
                labelControl8.Text = X_axis_name + " values";
                trackBarControl1.Value = 0;
                nChartControl1.Settings.ShapeRenderingMode = ShapeRenderingMode.HighSpeed;
                nChartControl1.Controller.Tools.Add(new NSelectorTool());
                nChartControl1.Controller.Tools.Add(new NTrackballTool());

                // set a chart title
                NLabel title = nChartControl1.Labels.AddHeader(m_map_name);
                title.TextStyle.FontStyle = new NFontStyle("Times New Roman", 18, FontStyle.Italic);
                title.TextStyle.FillStyle = new NColorFillStyle(Color.FromArgb(68, 90, 108));

                // setup chart
                Console.WriteLine("Number of charts: " + nChartControl1.Charts.Count.ToString());



                NChart chart = nChartControl1.Charts[0];
                nChartControl1.Legends.Clear();
                chart.Enable3D = true;
                chart.Width = 60.0f;
                chart.Depth = 60.0f;
                //chart.Height = 25.0f;
                chart.Height = 35.0f;
                chart.Projection.SetPredefinedProjection(PredefinedProjection.PerspectiveTilted);
                chart.LightModel.SetPredefinedLightModel(PredefinedLightModel.ShinyTopLeft);
                /*NOrdinalScaleConfigurator ordinalScale = (NOrdinalScaleConfigurator)chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator;
                ordinalScale.MajorGridStyle.SetShowAtWall(ChartWallType.Floor, true);
                ordinalScale.MajorGridStyle.SetShowAtWall(ChartWallType.Back, true);
                ordinalScale.DisplayDataPointsBetweenTicks = false;*/

                // oud
                NStandardScaleConfigurator scaleConfiguratorX = (NStandardScaleConfigurator)chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator;
                scaleConfiguratorX.MaxTickCount = dt.Rows.Count;
                scaleConfiguratorX.MajorTickMode = MajorTickMode.AutoMaxCount;


                // nieuw
                /*NLinearScaleConfigurator scaleConfiguratorX = new NLinearScaleConfigurator();
                chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator = scaleConfiguratorX;
                scaleConfiguratorX.MajorGridStyle.SetShowAtWall(ChartWallType.Floor, true);
                scaleConfiguratorX.MajorGridStyle.SetShowAtWall(ChartWallType.Back, true);
                scaleConfiguratorX.RoundToTickMax = false;
                scaleConfiguratorX.RoundToTickMin = false;*/

                NScaleTitleStyle titleStyleX = (NScaleTitleStyle)scaleConfiguratorX.Title;
                titleStyleX.Text = m_y_axis_name;
                //<GS-08032010> as waarden nog omzetten indien noodzakelijk (MAP etc)
                scaleConfiguratorX.AutoLabels = false;
                scaleConfiguratorX.Labels.Clear();

                for (int t = y_axisvalues.Length - 1; t >= 0; t--)
                {

                    string yvalue = ConvertYAxisValue(y_axisvalues.GetValue(t).ToString());
                    scaleConfiguratorX.Labels.Add(yvalue);
                }
            
                NStandardScaleConfigurator scaleConfiguratorY = (NStandardScaleConfigurator)chart.Axis(StandardAxis.Depth).ScaleConfigurator;
                scaleConfiguratorY.MajorTickMode = MajorTickMode.AutoMaxCount;
                scaleConfiguratorY.MaxTickCount = dt.Columns.Count;
                NScaleTitleStyle titleStyleY = (NScaleTitleStyle)scaleConfiguratorY.Title;
                titleStyleY.Text = m_x_axis_name;
                scaleConfiguratorY.Labels.Clear();

                scaleConfiguratorY.AutoLabels = false;
                for (int t = 0; t < x_axisvalues.Length; t++)
                {
                    string xvalue = ConvertXAxisValue(x_axisvalues.GetValue(t).ToString());
                    scaleConfiguratorY.Labels.Add(xvalue);
                }

                NStandardScaleConfigurator scaleConfiguratorZ = (NStandardScaleConfigurator)chart.Axis(StandardAxis.PrimaryY).ScaleConfigurator;
                scaleConfiguratorZ.MajorTickMode = MajorTickMode.AutoMaxCount;
                NScaleTitleStyle titleStyleZ = (NScaleTitleStyle)scaleConfiguratorZ.Title;
                titleStyleZ.Text = m_z_axis_name;

                scaleConfiguratorZ.AutoLabels = true;

                NMeshSurfaceSeries surface = null;
                if (chart.Series.Count == 0)
                {
                    surface = (NMeshSurfaceSeries)chart.Series.Add(SeriesType.MeshSurface);
                }
                else
                {
                    surface = (NMeshSurfaceSeries)chart.Series[0];
                }


                if (m_map_original_content != null)
                {
                    btnToggleOverlay.Visible = true;
                    NMeshSurfaceSeries surface2 = null;
                    if (chart.Series.Count == 1)
                    {
                        surface2 = (NMeshSurfaceSeries)chart.Series.Add(SeriesType.MeshSurface);
                    }
                    else
                    {
                        surface2 = (NMeshSurfaceSeries)chart.Series[1];
                    }
                    surface2.PositionValue = 10.0;
                    surface2.Name = "Surface2";
                    if (m_issixteenbit)
                    {
                        surface2.Data.SetGridSize((m_map_content.Length / 2) / m_TableWidth, m_TableWidth);
                    }
                    else
                    {
                        surface2.Data.SetGridSize(m_map_content.Length / m_TableWidth, m_TableWidth);
                    }
                    surface2.ValueFormatter.FormatSpecifier = "0.00";
                    surface2.FillMode = SurfaceFillMode.Zone;
                    surface2.FillStyle.SetTransparencyPercent(50);
                    surface2.SmoothPalette = true;
                    surface2.FrameColorMode = SurfaceFrameColorMode.Zone;//Uniform;
                    surface2.FrameMode = SurfaceFrameMode.MeshContour;
                    NMeshSurfaceSeries surface3 = null;
                    if (chart.Series.Count == 2)
                    {
                        surface3 = (NMeshSurfaceSeries)chart.Series.Add(SeriesType.MeshSurface);
                    }
                    else
                    {
                        surface3 = (NMeshSurfaceSeries)chart.Series[2];
                    }
                    surface3.PositionValue = 10.0;
                    surface3.Name = "Surface3";
                    if (m_issixteenbit)
                    {
                        surface3.Data.SetGridSize((m_map_content.Length / 2) / m_TableWidth, m_TableWidth);
                    }
                    else
                    {
                        surface3.Data.SetGridSize(m_map_content.Length / m_TableWidth, m_TableWidth);
                    }
                    surface3.ValueFormatter.FormatSpecifier = "0.00";
                    surface3.FillMode = SurfaceFillMode.Zone;
                    surface3.FillStyle.SetTransparencyPercent(50);
                    surface3.SmoothPalette = true;
                    surface3.FrameColorMode = SurfaceFrameColorMode.Zone;
                    surface3.FrameMode = SurfaceFrameMode.MeshContour;
                }

                chart.Wall(ChartWallType.Back).Visible = false;
                chart.Wall(ChartWallType.Left).Visible = false;
                chart.Wall(ChartWallType.Right).Visible = false;
                chart.Wall(ChartWallType.Floor).Visible = false;

                surface.Name = "Surface";
                //surface.Legend.Mode = SeriesLegendMode.SeriesLogic;
                surface.PositionValue = 10.0;

                // always 256 * 256 ?

                if (m_issixteenbit)
                {
                    surface.Data.SetGridSize((m_map_content.Length / 2) / m_TableWidth, m_TableWidth);
                }
                else
                {
                    surface.Data.SetGridSize(m_map_content.Length / m_TableWidth, m_TableWidth);
                }
                //surface.SyncPaletteWithAxisScale = true;
                //surface.PaletteSteps = 16;
                surface.ValueFormatter.FormatSpecifier = "0.00";
                //surface.FillStyle = new NColorFillStyle(Color.Green);


                surface.FillMode = SurfaceFillMode.Zone; // <GS-08032010>
                surface.SmoothPalette = true;
                surface.FrameColorMode = SurfaceFrameColorMode.Uniform;
                surface.FillStyle.SetTransparencyPercent(25);
                surface.FrameMode = SurfaceFrameMode.MeshContour;
                RefreshMeshGraph();

                //byte[] bts = GetDataFromGridView(false);
                //UpdateChartControlSlice(bts);

                /*
                x = "MAP";
                y = "RPM";
                z = "Degrees";
                 */
            }
            else if (m_TableWidth == 1)
            {
                xtraTabControl1.SelectedTabPage = xtraTabPage2;
                
                SetViewTypeParams(m_vs);
                trackBarControl1.Properties.Minimum = 0;
                trackBarControl1.Properties.Maximum = x_axisvalues.Length - 1;
                labelControl8.Text = X_axis_name + " values";
                /*** end test ***/
                trackBarControl1.Properties.Minimum = 0;
                trackBarControl1.Properties.Maximum = 0;
                trackBarControl1.Enabled = false;
                labelControl8.Text = X_axis_name;

                DataTable chartdt = new DataTable();
                chartdt.Columns.Add("X", Type.GetType("System.Double"));
                chartdt.Columns.Add("Y", Type.GetType("System.Double"));
                double valcount = 0;
                if (m_issixteenbit)
                {
                    for (int t = 0; t < m_map_length; t += 2)
                    {
                        double yval = valcount;
                        double value = Convert.ToDouble(m_map_content.GetValue(t)) * 256;
                        value += Convert.ToDouble(m_map_content.GetValue(t + 1));
                        if (y_axisvalues.Length > valcount) yval = Convert.ToDouble((int)y_axisvalues.GetValue((int)valcount));
                        chartdt.Rows.Add(yval, value);
                        valcount++;
                    }
                }
                else
                {
                    for (int t = 0; t < m_map_length; t++)
                    {
                        double yval = valcount;
                        double value = Convert.ToDouble(m_map_content.GetValue(t));
                        if (y_axisvalues.Length > valcount) yval = Convert.ToDouble((int)y_axisvalues.GetValue((int)valcount));
                        chartdt.Rows.Add(yval, value);
                        valcount++;
                    }
                }
                //chartControl1.Series[0].Label.Text = m_map_name;
                /*chartControl1.Series[0].LegendText = m_map_name;
                chartControl1.DataSource = chartdt;
                string[] datamembers = new string[1];
                chartControl1.Series[0].ValueDataMembers.Clear();
                datamembers.SetValue("Y", 0);
                chartControl1.Series[0].ValueDataMembers.AddRange(datamembers);
                chartControl1.Series[0].ArgumentDataMember = "X";
                chartControl1.Visible = true;*/
            }
            Init2dChart();
            m_trackbarBlocked = false;

            if (this.m_map_address >= 0xF00000 || (m_isOpenSoftware && m_isRAMViewer))
            {
                //this.m_isRAMViewer = true;
                //this.OnlineMode = true;
                if (_autoUpdateIfSRAM)
                {
                    timer5.Interval = _autoUpdateInterval * 1000;
                    timer5.Enabled = true;
                }
            }
        }

        private void Init2dChart()
        {
            nChartControl2.Settings.ShapeRenderingMode = ShapeRenderingMode.HighSpeed;
            nChartControl2.Legends.Clear(); // no legend
            NChart chart2d = nChartControl2.Charts[0];

            NSmoothLineSeries surface = null;
            if (chart2d.Series.Count == 0)
            {
                surface = (NSmoothLineSeries)chart2d.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                surface = (NSmoothLineSeries)chart2d.Series[0];
            }

            chart2d.BoundsMode = BoundsMode.Stretch;
            NLinearScaleConfigurator linearScale = (NLinearScaleConfigurator)chart2d.Axis(StandardAxis.PrimaryY).ScaleConfigurator;
            linearScale.MajorGridStyle.LineStyle.Pattern = LinePattern.Dot;
            linearScale.MajorGridStyle.SetShowAtWall(ChartWallType.Back, true);
            NScaleStripStyle stripStyle = new NScaleStripStyle(new NColorFillStyle(Color.Beige), null, true, 0, 0, 1, 1);
            stripStyle.Interlaced = true;
            stripStyle.SetShowAtWall(ChartWallType.Back, true);
            stripStyle.SetShowAtWall(ChartWallType.Left, true);
            linearScale.StripStyles.Add(stripStyle);
            NSmoothLineSeries line = null;
            if (chart2d.Series.Count == 0)
            {
                line = (NSmoothLineSeries)chart2d.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line = (NSmoothLineSeries)chart2d.Series[0];
            }
            line.Name = m_map_name;
            line.Legend.Mode = SeriesLegendMode.Series;
            line.UseXValues = true;
            line.UseZValues = false;
            line.DataLabelStyle.Visible = true;
            line.MarkerStyle.Visible = true;
            line.MarkerStyle.PointShape = PointShape.Sphere;
            line.MarkerStyle.AutoDepth = true;
            line.MarkerStyle.Width = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line.MarkerStyle.Height = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line.MarkerStyle.Depth = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            //line.HorizontalAxes = y_axisvalues;

            surface.Name = "Surface";
            //surface.Legend.Mode = SeriesLegendMode.SeriesLogic;
            //surface.PositionValue = 10.0;
            for (int i = 0; i < y_axisvalues.Length; i++)
            {
                surface.XValues.Add(y_axisvalues.GetValue(i));
            }
            NStyleSheet styleSheet = NStyleSheet.CreatePredefinedStyleSheet(PredefinedStyleSheet.Nevron);
            styleSheet.Apply(nChartControl2.Document);
        }

        private void SetViewTypeParams(ViewSize vs)
        {

        }

        private void UpdateChartControlSlice(byte[] data)
        {

            DataTable chartdt = new DataTable();
            chartdt.Columns.Add("X", Type.GetType("System.Double"));
            chartdt.Columns.Add("Y", Type.GetType("System.Double"));
            double valcount = 0;
            int offsetinmap = (int)trackBarControl1.Value;

            try
            {
                labelControl9.Text = X_axis_name + " [" + x_axisvalues.GetValue((int)trackBarControl1.Value).ToString() + "]";
                
            }
            catch (Exception E)
            {
                Console.WriteLine("value: " + (int)trackBarControl1.Value + " " + E.Message);
            }

            int numberofrows = data.Length / m_TableWidth;
            if (m_issixteenbit)
            {
                numberofrows /= 2;
                offsetinmap *= 2;
            }

            if (m_issixteenbit)
            {
                for (int t = (numberofrows - 1); t >= 0; t--)
                {
                    double yval = valcount;
                    double value = Convert.ToDouble(data.GetValue(offsetinmap + (t * (m_TableWidth * 2)))) * 256;
                    value += Convert.ToDouble(data.GetValue(offsetinmap + (t * (m_TableWidth * 2)) + 1));
                    if (value > 32000)
                    {
                        value = 65536 - value;
                        value = -value;
                    }
                    value *= correction_factor;
                    value += correction_offset;
                    //value = ConvertForThreeBarSensor(value);
                    if (y_axisvalues.Length > valcount) yval = Convert.ToDouble((int)y_axisvalues.GetValue((int)valcount));
                    

                    chartdt.Rows.Add(yval, value);
                    valcount++;
                }
            }
            else
            {
                for (int t = (numberofrows - 1); t >= 0; t--)
                {
                    double yval = valcount;
                    double value = Convert.ToDouble(data.GetValue(offsetinmap + (t * (m_TableWidth))));
                    value *= correction_factor;
                    value += correction_offset;
                    //value = ConvertForThreeBarSensor(value);

                    if (y_axisvalues.Length > valcount) yval = Convert.ToDouble((int)y_axisvalues.GetValue((int)valcount));
                    
                    chartdt.Rows.Add(yval, value);
                    valcount++;
                }
            }

            //chartControl1.Series[0].Label.Text = m_map_name;
            /*chartControl1.Series[0].LegendText = m_map_name;
            chartControl1.DataSource = chartdt;
            //chartControl1.Series[0].PointOptions.PointView = PointView.ArgumentAndValues;
            chartControl1.Invalidate();*/

            NChart chart = nChartControl2.Charts[0];
            //NSeries series = (NSeries)chart.Series[0];
            NSmoothLineSeries line = null;
            if (chart.Series.Count == 0)
            {
                line = (NSmoothLineSeries)chart.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line = (NSmoothLineSeries)chart.Series[0];
            }
            // set length of axis
            //NStandardScaleConfigurator scaleConfiguratorX = (NStandardScaleConfigurator)chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator;
            //scaleConfiguratorX.MajorTickMode = MajorTickMode.AutoMaxCount;
            //NScaleTitleStyle titleStyleX = (NScaleTitleStyle)scaleConfiguratorX.Title;
            //titleStyleX.Text = m_y_axis_name;
            //<GS-08032010> as waarden nog omzetten indien noodzakelijk (MAP etc)
            //scaleConfiguratorX.AutoLabels = true;
            //series.HorizontalAxes = y_axisvalues;
            //scaleConfiguratorX.Labels.Clear();
            /*for (int t = y_axisvalues.Length - 1; t >= 0; t--)
            {
                string yvalue = y_axisvalues.GetValue(t).ToString();
                if (m_y_axis_name == "MAP" || m_y_axis_name == "Pressure error (bar)")
                {
                    try
                    {
                        float v = (float)Convert.ToDouble(yvalue);
                        if (m_viewtype == ViewType.Easy3Bar)
                        {
                            v *= 1.2F;
                        }
                        else if (m_viewtype == ViewType.Easy35Bar)
                        {
                            v *= 1.4F;
                        }
                        else if (m_viewtype == ViewType.Easy4Bar)
                        {
                            v *= 1.6F;
                        }
                        v *= (float)0.01F;
                        if (m_y_axis_name == "MAP")
                        {
                            v -= 1;
                        }
                        yvalue = v.ToString("F2");
                    }
                    catch (Exception cE)
                    {
                        Console.WriteLine(cE.Message);
                    }
                }
                scaleConfiguratorX.Labels.Add(yvalue);
                Console.WriteLine("Added axis label: " + yvalue);

            }*/
            line.ClearDataPoints();
            foreach (DataRow dr in chartdt.Rows)
            {
                //<GS-09032010> fill second 2d chart here
                //series.Values.Add(dr["Y"]);
                line.AddDataPoint(new NDataPoint(Convert.ToDouble(dr["X"]), Convert.ToDouble(dr["Y"])));
                //Console.WriteLine("Added value: " + dr["Y"].ToString());
            }
            nChartControl2.Refresh();


        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (!m_isRAMViewer)
            {
                if (m_datasourceMutated)
                {
                    DialogResult dr = MessageBox.Show("Data was mutated, do you want to save these changes in you binary?", "Warning", MessageBoxButtons.YesNoCancel);
                    if (dr == DialogResult.Yes)
                    {
                        m_SaveChanges = true;
                        CastSaveEvent();
                        CastCloseEvent();

                    }
                    else if (dr == DialogResult.No)
                    {
                        m_SaveChanges = false;
                        CastCloseEvent();
                    }
                    else
                    {
                        // cancel
                        // do nothing
                    }
                }
                else
                {
                    m_SaveChanges = false;
                    CastCloseEvent();
                }
            }
            else
            {
                m_SaveChanges = false;
                CastCloseEvent();
            }
        }

        private int m_StandardFill = 0;

        public int StandardFill
        {
            get
            {
                return m_StandardFill;
            }
            set
            {
                m_StandardFill = value;
            }
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            string newv = string.Empty;
            foreach (char c in v)
            {
                if (c == ',' || c == '.') newv += c;
                else if (c >= 0x30 && c <= 0x39) newv += c;
                else if (c == 'A' || c == 'a') newv += c;
                else if (c == 'B' || c == 'b') newv += c;
                else if (c == 'C' || c == 'c') newv += c;
                else if (c == 'D' || c == 'd') newv += c;
                else if (c == 'E' || c == 'e') newv += c;
                else if (c == 'F' || c == 'f') newv += c;
            }
            if (newv == "") return d;
            string vs = "";
            vs = newv.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
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
                        if (m_viewtype == ViewType.Hexadecimal)
                        {
                            b = Convert.ToInt32(e.CellValue.ToString(), 16);
                            cellvalue = b;
                        }
                        else
                        {
                            b = Convert.ToInt32(e.CellValue.ToString());
                            cellvalue = b;
                        }
                        b *= 255;
                        if (m_MaxValueInTable != 0)
                        {
                            b /= m_MaxValueInTable;
                        }

                        // new version

                        b = 0;
                        double td = 0;
                        int tempcellvalue = 0;
                        if (m_viewtype == ViewType.Hexadecimal)
                        {
                            b = Convert.ToInt32(e.CellValue.ToString(), 16);
                            td = b;
                            if (m_viewtype == ViewType.Easy)
                            {
                                td *= correction_factor;
                                td += correction_offset;
                            }
                            tempcellvalue = Convert.ToInt32(td);
                        }
                        else
                        {
                            b = Convert.ToInt32(ConvertToDouble(e.CellValue.ToString()));
                            td = b;
                            if (m_viewtype == ViewType.Easy)
                            {
                                td *= correction_factor;
                                td += correction_offset;
                            }
                            tempcellvalue = Convert.ToInt32(td);

                        }
                        td *= 255;
                        b = Convert.ToInt32(td);
                        if (now_realMaxValue != double.MinValue && now_realMaxValue != 0)
                        {
                            b /= Convert.ToInt32(now_realMaxValue);
                        }

                        // upto here


                        int red = 128;
                        int green = 128;
                        int blue = 128;
                        Color c = Color.White;
                        if (m_OnlineMode)
                        {
                            b /= 2;
                            red = b;
                            if (red < 0) red = 0;
                            if (red > 255) red = 255;
                            if (b > 255) b = 255;
                            green = 255 - red;
                            blue = 255 - red;
                            c = Color.FromArgb(red, green, blue);
                            // if (b == 0) c = Color.Transparent;

                        }
                        else if (!m_isRedWhite)
                        {
                            red = b;
                            if (red < 0) red = 0;
                            if (red > 255) red = 255;
                            if (b > 255) b = 255;
                            blue = 0;
                            green = 255 - red;
                            c = Color.FromArgb(red, green, blue);
                        }
                        else
                        {
                            if (b < 0) b = -b;
                            if (b > 255) b = 255;
                            c = Color.FromArgb(b, Color.Red);
                        }
                        
                        if (m_viewtype == ViewType.Easy )
                        {
                            float dispvalue = 0;
                            dispvalue = (float)cellvalue;
                            if (correction_offset != 0 || correction_factor != 1)
                            {
                                dispvalue = (float)((float)cellvalue * (float)correction_factor) + (float)correction_offset;
                                if (m_viewtype != ViewType.Hexadecimal)
                                //if (!m_isHexMode)
                                {
                                    if (m_map_name.StartsWith("Injector duration") || m_map_name.StartsWith("Start of injection"))
                                    {
                                        e.DisplayText = dispvalue.ToString("F1") + "\u00b0";
                                        /*if (dispvalue < 0)
                                        {
                                            Console.WriteLine("Negative value:  " + cellvalue.ToString());

                                        }*/
                                    }
                                    else if (m_map_name.StartsWith("N75"))
                                    {
                                        e.DisplayText = dispvalue.ToString("F0") + @"%";
                                    }
                                    else
                                    {
                                        e.DisplayText = dispvalue.ToString("F2");
                                    }
                                }
                                else
                                {
                                    //e.DisplayText = dispvalue.ToString();
                                }
                            }
                            else if (m_map_name.StartsWith("N75"))
                            {
                                e.DisplayText = dispvalue.ToString("F0") + @"%";
                            }
                        }
                        //if (m_map_name == "BFuelCal.Map" || m_map_name == "IgnNormCal.Map" || m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
                        if (X_axis_name.ToLower() == "mg/c" && Y_axis_name.ToLower() == "rpm")
                        {
                            
                            try
                            {
                                if (open_loop != null)
                                {
                                    int airmassvalue = GetXaxisValue(e.Column.AbsoluteIndex);
                                    int rpmvalue = GetYaxisValue(e.RowHandle);
                                    if (open_loop.Length > 0)
                                    {
                                        int mapopenloop = GetOpenLoopValue(e.RowHandle);
                                        if (mapopenloop > airmassvalue)
                                        {
                                            if (m_StandardFill == 0)
                                            {

                                            }
                                            else if (m_StandardFill == 1)
                                            {
                                                Pen p = new Pen(Brushes.Black, 2);
                                                e.Graphics.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                                p.Dispose();
                                            }
                                            else
                                            {
                                                Point[] pnts = new Point[4];
                                                pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 0);
                                                pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width - (e.Bounds.Height / 2), e.Bounds.Y), 1);
                                                pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y + (e.Bounds.Height / 2)), 2);
                                                pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 3);
                                                e.Graphics.FillPolygon(Brushes.SeaGreen, pnts, System.Drawing.Drawing2D.FillMode.Winding);
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
                        if (!m_disablecolors)
                        {
                            SolidBrush sb = new SolidBrush(c);
                            e.Graphics.FillRectangle(sb, e.Bounds);
                        }
                    }
                }
                if (m_selectedrowhandle >= 0 && m_selectedcolumnindex >= 0)
                {
                    if (e.RowHandle == m_selectedrowhandle && e.Column.AbsoluteIndex == m_selectedcolumnindex)
                    {
                        SolidBrush sbsb = new SolidBrush(Color.Yellow);
                        e.Graphics.FillRectangle(sbsb, e.Bounds);
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private int GetOpenLoopValue(int index)
        {
            index = (y_axisvalues.Length - 1) - index;
            //int retval = (int)open_loop[(open_loop.Length - e.RowHandle) - 1];
            int retval = 0;
            retval = Convert.ToInt32(open_loop.GetValue(index * 2));
            retval *= 256;
            retval += Convert.ToInt32(open_loop.GetValue((index * 2) + 1));

            return retval;
        }

        private int GetXaxisValue(int index)
        {
            int retval = 0;
            retval = (int)x_axisvalues.GetValue(index);
            return retval;
        }

        private int GetYaxisValue(int index)
        {
            int retval = 0;
            retval = (int)y_axisvalues.GetValue(index );
            /*retval *= 256;
            retval += (int)y_axisvalues.GetValue((index * 2) + 1);*/
            return retval;
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            m_datasourceMutated = true;
            simpleButton2.Enabled = true;
//            simpleButton3.Enabled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            ShowTable(m_TableWidth, m_issixteenbit);
            m_datasourceMutated = false;
            //simpleButton2.Enabled = false;
//            simpleButton3.Enabled = false;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //if (m_isRAMViewer) return;
            //else
            //{
                m_SaveChanges = true;
                m_datasourceMutated = false;
                CastSaveEvent();
            //}
        }

        private byte[] GetDataFromGridView(bool upsidedown)
        {
            byte[] retval = new byte[m_map_length];
            DataTable gdt = (DataTable)gridControl1.DataSource;
            int cellcount = 0;
            if (upsidedown)
            {
                for (int t = gdt.Rows.Count - 1; t >= 0; t -- )
                {
                    foreach (object o in gdt.Rows[t].ItemArray)
                    {
                        if (o != null)
                        {
                            if (o != DBNull.Value)
                            {
                                if (cellcount < retval.Length)
                                {
                                    if (m_issixteenbit)
                                    {
                                        // twee waarde toevoegen
                                        Int32 cellvalue = 0;
                                        string bstr1 = "0";
                                        string bstr2 = "0";
                                        //if (m_isHexMode)
                                        if (m_viewtype == ViewType.Hexadecimal)
                                        {
                                            cellvalue = Convert.ToInt32(o.ToString(), 16);
                                        }
                                        else
                                        {
                                            cellvalue = Convert.ToInt32(o.ToString());
                                        }
                                        /*if (cellvalue < 0)
                                        {
                                            Console.WriteLine("value < 0");
                                        }*/
                                        bstr1 = cellvalue.ToString("X8").Substring(4, 2);
                                        bstr2 = cellvalue.ToString("X8").Substring(6, 2);
                                        retval.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                                        retval.SetValue(Convert.ToByte(bstr2, 16), cellcount++);
                                    }
                                    else
                                    {
                                        //if (m_isHexMode)
                                        if (m_viewtype == ViewType.Hexadecimal)
                                        {
                                            //double v = Convert.ToDouble(o);
                                            int iv = Convert.ToInt32(o.ToString(), 16);//(int)Math.Floor(v);
                                            retval.SetValue(Convert.ToByte(iv), cellcount++);
                                        }
                                        else
                                        {
                                            double v = Convert.ToDouble(o);
                                            retval.SetValue(Convert.ToByte((int)Math.Floor(v)), cellcount++);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else
            {

                foreach (DataRow dr in gdt.Rows)
                {
                    foreach (object o in dr.ItemArray)
                    {
                        if (o != null)
                        {
                            if (o != DBNull.Value)
                            {
                                if (cellcount < retval.Length)
                                {
                                    if (m_issixteenbit)
                                    {
                                        // twee waarde toevoegen
                                        Int32 cellvalue = 0;
                                        string bstr1 = "0";
                                        string bstr2 = "0";
                                        //if (m_isHexMode)
                                        if (m_viewtype == ViewType.Hexadecimal)
                                        {
                                            cellvalue = Convert.ToInt32(o.ToString(), 16);
                                        }
                                        else
                                        {
                                            cellvalue = Convert.ToInt32(o.ToString());
                                        }
                                        bstr1 = cellvalue.ToString("X8").Substring(4, 2);
                                        bstr2 = cellvalue.ToString("X8").Substring(6, 2);
                                        retval.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                                        retval.SetValue(Convert.ToByte(bstr2, 16), cellcount++); 
/*                                        bstr1 = cellvalue.ToString("X4").Substring(0, 2);
                                        bstr2 = cellvalue.ToString("X4").Substring(2, 2);
                                        retval.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                                        retval.SetValue(Convert.ToByte(bstr2, 16), cellcount++);*/
                                    }
                                    else
                                    {
//                                        if (m_isHexMode)
                                        if (m_viewtype == ViewType.Hexadecimal)
                                        {
                                            
                                            //double v = Convert.ToDouble(o);
                                            try
                                            {
                                                int iv = Convert.ToInt32(o.ToString(), 16);
                                                //int iv = (int)Math.Floor(v);
                                                retval.SetValue(Convert.ToByte(iv.ToString()), cellcount++);
                                            }
                                            catch (Exception cE)
                                            {
                                                Console.WriteLine(cE.Message);
                                            }

                                        }
                                        else
                                        {
                                            
                                            try
                                            {
                                                double v = Convert.ToDouble(o);
                                                if (v >= 0)
                                                {
                                                    retval.SetValue(Convert.ToByte((int)Math.Floor(v)), cellcount++);
                                                }
                                            }
                                            catch (Exception sE)
                                            {
                                                Console.WriteLine(sE.Message);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return retval;
        }

        private void CastSliderMoveEvent()
        {
            if (onSliderMove != null)
            {
                onSliderMove(this, new SliderMoveEventArgs((int)trackBarControl1.EditValue, m_map_name, m_filename));
            }
        }

        private void CastLockEvent(int mode)
        {
            if (onAxisLock != null)
            {
                switch (mode)
                {
                    case 0: // autoscale
                        onAxisLock(this, new AxisLockEventArgs(-1, mode, m_map_name, m_filename));
                        break;
                    case 1: // peak value
                        onAxisLock(this, new AxisLockEventArgs(m_MaxValueInTable, mode, m_map_name, m_filename));
                        break;
                    case 2: // max value
                        int max_value = 0xFFFF;
                        onAxisLock(this, new AxisLockEventArgs(max_value, mode, m_map_name, m_filename));
                        break;
                }
            }
            else
            {
                Console.WriteLine("onAxisLock not registered");
            }
        }

        private void CastSelectEvent(int rowhandle, int colindex)
        {
            if (onSelectionChanged != null)
            {
                // haal eerst de data uit de tabel van de gridview
                onSelectionChanged(this, new CellSelectionChangedEventArgs(rowhandle, colindex, m_map_name));
            }
            else
            {
                Console.WriteLine("onSelectionChanged not registered!");
            }

        }

        public void SelectCell(int rowhandle, int colindex)
        {
            try
            {
                m_prohibitcellchange = true;
                gridView1.ClearSelection();
                gridView1.SelectCell(rowhandle, gridView1.Columns[colindex]);
                m_prohibitcellchange = false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        public void SetSplitter(int panel1height, int panel2height, int splitdistance, bool panel1collapsed, bool panel2collapsed)
        {
            try
            {
                m_prohibitsplitchange = true;
                if (panel1collapsed)
                {
                    splitContainer1.Panel1Collapsed = true;
                    splitContainer1.Panel2Collapsed = false;
                }
                else if (panel2collapsed)
                {
                    splitContainer1.Panel2Collapsed = true;
                    splitContainer1.Panel1Collapsed = false;
                }
                else
                {
                    splitContainer1.Panel2Collapsed = false;
                    splitContainer1.Panel1Collapsed = false;

                    splitContainer1.SplitterDistance = splitdistance;
                  //  splitContainer1.Panel1.Height = panel1height;
                  //  splitContainer1.Panel2.Height = panel2height;
                }

                m_prohibitsplitchange = false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }


        private void CastSaveEvent()
        {
            if (onSymbolSave != null)
            {
                // haal eerst de data uit de tabel van de gridview
                byte[] mutateddata = GetDataFromGridView(m_isUpsideDown);
                onSymbolSave(this, new SaveSymbolEventArgs(m_map_address, m_map_length, mutateddata, m_map_name, Filename));
                m_datasourceMutated = false;
                //simpleButton2.Enabled = false;
//                simpleButton3.Enabled = false;
            }
            else
            {
                Console.WriteLine("onSymbolSave not registered!");
            }

        }

        private void CastSplitterMovedEvent()
        {
            if (onSplitterMoved != null)
            {
                // haal eerst de data uit de tabel van de gridview
                if (!m_prohibitsplitchange)
                {
                    onSplitterMoved(this, new SplitterMovedEventArgs(splitContainer1.Panel1.Height, splitContainer1.Panel2.Height,splitContainer1.SplitterDistance ,splitContainer1.Panel1Collapsed, splitContainer1.Panel2Collapsed, m_map_name));
                }
            }
            else
            {
                Console.WriteLine("onSplitterMoved not registered!");
            }

        }



        private void CastCloseEvent()
        {
            if (onClose != null)
            {
                onClose(this, EventArgs.Empty);
            }
        }

        

        


        
        

        private void groupControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            
            m_datasourceMutated = true;
            simpleButton2.Enabled = true;
//            simpleButton3.Enabled = true;
            if (nChartControl1.Visible)
            {
                StartSurfaceChartUpdateTimer();
            }
            else if (nChartControl2.Visible)
            {
                if (m_TableWidth == 1)
                {
                    StartSingleLineGraphTimer();
                }
                else
                {
                    StartChartUpdateTimer();
                    //   UpdateChartControlSlice(GetDataFromGridView(false));
                }
            }
        }

        private void StartSingleLineGraphTimer()
        {
            timer3.Stop();
            timer3.Start();
        }

        private void StartChartUpdateTimer()
        {
            timer1.Stop();
            timer1.Start();
        }

        private void StartSurfaceChartUpdateTimer()
        {
            timer2.Stop();
            timer2.Start();
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length > 0)
            {
                if (e.KeyCode == Keys.Add)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if(m_viewtype == ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value++;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                if (value > 0xFF) value = 0xFF;
                                
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value++;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                if (value > 0xFF) value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value--;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value--;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value += 0x10;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {

                                if (value > 0xFF) value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value+=10;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                if (value > 0xFF) value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == ViewType.Hexadecimal)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value-=0x10;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value-=10;
                            if (!m_issixteenbit)
                            {
                                if (value < 0) value = 0;
                            }
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.Home)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == ViewType.Hexadecimal)
                        {

                            int value = 0xFFFF;
                            if (m_issixteenbit)
                            {
                                value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;

                        }
                        else
                        {
                            int value = 0xFFFF;
                            if (m_issixteenbit)
                            {
                                value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {

                                value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;

                        }
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        //if (IsHexMode)
                        if (m_viewtype == ViewType.Hexadecimal)
                        {
                            int value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }

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
                    if (y_axisvalues.Length > 0)
                    {
                        if (y_axisvalues.Length > e.RowHandle)
                        {
                            string yvalue = y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle).ToString();
                            if (!m_isUpsideDown)
                            {
                                // dan andere waarde nemen
                                yvalue = y_axisvalues.GetValue(e.RowHandle).ToString();
                            }
                            if (m_viewtype == ViewType.Hexadecimal)
                            {
                                yvalue = Convert.ToInt32(/*y_axisvalues.GetValue(e.RowHandle)*/y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle)).ToString("X4");
                            }
                            else
                            {
                                yvalue = ConvertYAxisValue(yvalue);
                            }
                            //yvalue = (ConvertToDouble(yvalue) * m_Yaxiscorrectionfactor).ToString();
                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                            e.Handled = true;
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
            else
            {
                try
                {
                    //<GS-20120801>
                    if (m_yaxisUnits.Length > 0 || m_xaxisUnits.Length > 0)
                    {
                        Font f = new Font(this.Font.FontFamily, 6);
                        
                        SizeF xsize = e.Graphics.MeasureString(m_xaxisUnits, f);
                        SizeF ysize = e.Graphics.MeasureString(m_yaxisUnits, f);

                        e.Graphics.DrawString(m_xaxisUnits, f, Brushes.Black, e.Bounds.X + e.Bounds.Width - xsize.Width - 1, e.Bounds.Y);

                        e.Graphics.DrawString(m_yaxisUnits, f, Brushes.Black, e.Bounds.X + 1, e.Bounds.Y + e.Bounds.Height/2);
                        
                        //Console.WriteLine(e.RowHandle.ToString());
                        //e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
                        e.Handled = true;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private string ConvertYAxisValue(string currValue)
        {
            string retval = currValue.ToString();
            if (m_Yaxiscorrectionfactor == 1) return currValue;
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                provider = new CultureInfo("nl-NL");
                float temp = (float)Convert.ToDouble(currValue, provider);
                temp *= (float)m_Yaxiscorrectionfactor;
                temp += (float)m_Yaxiscorrectionoffset;
                retval = temp.ToString("F1");
            }
            catch (Exception E)
            {
                Console.WriteLine("MapViewer::ConvertYAxisValue: " + E.Message);
            }
            return retval;
        }
        private string ConvertXAxisValue(string currValue)
        {
            string retval = currValue.ToString();
            if (m_Xaxiscorrectionfactor == 1) return currValue;
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                provider = new CultureInfo("nl-NL");
                float temp = (float)Convert.ToDouble(currValue, provider);
                temp *= (float)m_Xaxiscorrectionfactor;
                temp += (float)m_Xaxiscorrectionoffset;

                retval = temp.ToString("F2");
            }
            catch (Exception E)
            {
                Console.WriteLine("MapViewer::ConvertXAxisValue: " + E.Message);
            }
            return retval;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (x_axisvalues.Length > 0)
                {
                    if (e.Column != null)
                    {
                        if (x_axisvalues.Length > e.Column.VisibleIndex)
                        {
                            string xvalue = x_axisvalues.GetValue(e.Column.VisibleIndex).ToString();
                            if (m_viewtype == ViewType.Hexadecimal)
                            {
                                xvalue = Convert.ToInt32(x_axisvalues.GetValue(e.Column.VisibleIndex)).ToString(m_xformatstringforhex);
                            }
                            else
                            {
                                xvalue = ConvertXAxisValue(xvalue);
                            } 
                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                            e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }


        internal void ReShowTable()
        {
            ShowTable(m_TableWidth, m_issixteenbit);
        }

        private void chartControl1_Click(object sender, EventArgs e)
        {

        }

        private void trackBarControl1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_trackbarBlocked)
            {
                if (m_TableWidth > 1)
                {
                    UpdateChartControlSlice(GetDataFromGridView(false));
                    _sp_dragging = null;
                    timer4.Enabled = false;
                    CastSliderMoveEvent();
                }
            }
        }

        private void chartControl1_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            
        }

        private void chartControl1_ObjectHotTracked(object sender, HotTrackEventArgs e)
        {
            if (e.Object is Series)
            {
                Series s = (Series)e.Object;
                if (e.AdditionalObject is SeriesPoint)
                {
                    SeriesPoint sp = (SeriesPoint)e.AdditionalObject;
                    _sp_dragging = (SeriesPoint)e.AdditionalObject;
                    //timer4.Enabled = true;
                    // alleen hier selecteren, niet meer blinken
                    if (_sp_dragging != null)
                    {
                        string yaxisvalue = _sp_dragging.Argument;
                        int rowhandle = GetRowForAxisValue(yaxisvalue);
                        if (m_TableWidth == 1)
                        {
                            // single column graph.. 
                            int numberofrows = m_map_length;
                            if (m_issixteenbit) numberofrows /= 2;
                            rowhandle = (numberofrows - 1) - Convert.ToInt32(yaxisvalue);
                        }
                        if (rowhandle != -1)
                        {
                            gridView1.ClearSelection();
                            gridView1.SelectCell(rowhandle, gridView1.Columns[(int)trackBarControl1.Value]);
                        }
                    }

                    string detailline = Y_axis_name + ": " + sp.Argument + Environment.NewLine + Z_axis_name + ": " + sp.Values[0].ToString();
                    if (m_map_name.StartsWith("Ign_map_0!") || m_map_name.StartsWith("Ign_map_4!")) detailline += " \u00b0";// +"C";
                    toolTipController1.ShowHint(detailline, "Details", Cursor.Position);
                }
            }
            else
            {
                toolTipController1.HideHint();
                
            }

        }

        private void chartControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           /* object[] objs = chartControl1.HitTest(e.X, e.Y);
            foreach (object o in objs)
            {
                Console.WriteLine("Double clicked: " + o.ToString());
            }*/
        }

        private SeriesPoint _sp_dragging;

        private void chartControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_isDragging = true;
                timer4.Enabled = true;
                _mouse_drag_x = e.X;
                _mouse_drag_y = e.Y;
                toolTipController1.HideHint();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateChartControlSlice(GetDataFromGridView(false));
            timer1.Enabled = false;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            UpdateChartControlSlice(GetDataFromGridView(false));

           /* DataTable chartdt = new DataTable();
            chartdt.Columns.Add("X", Type.GetType("System.Double"));
            chartdt.Columns.Add("Y", Type.GetType("System.Double"));
            double valcount = 0;
            byte[] content = GetDataFromGridView(false);
            if (m_issixteenbit)
            {
                for (int t = 0; t < content.Length; t += 2)
                {
                    double value = Convert.ToDouble(content.GetValue((content.Length - 1) - (t + 1))) * 256;
                    value += Convert.ToDouble(content.GetValue((content.Length - 1) - (t)));
                    chartdt.Rows.Add(valcount++, value);
                }
            }
            else
            {
                for (int t = 0; t < m_map_length; t++)
                {
                    double value = Convert.ToDouble(content.GetValue((content.Length - 1) - t));
                    chartdt.Rows.Add(valcount++, value);
                }
            }
            chartControl1.DataSource = chartdt;
            string[] datamembers = new string[1];
            chartControl1.Series[0].ValueDataMembers.Clear();
            datamembers.SetValue("Y", 0);
            chartControl1.Series[0].ValueDataMembers.AddRange(datamembers);
            chartControl1.Series[0].ArgumentDataMember = "X";
            chartControl1.Invalidate();*/
            timer3.Enabled = false;
        }



        private void CastSurfaceGraphChangedEvent(int Pov_x, int Pov_y, int Pov_z, int Pan_x, int Pan_y, double Pov_d)
        {
            if (onSurfaceGraphViewChanged != null)
            {
                if (!m_prohibitgraphchange)
                {
                    onSurfaceGraphViewChanged(this, new SurfaceGraphViewChangedEventArgs(Pov_x, Pov_y, Pov_z, Pan_x, Pan_y, Pov_d, m_map_name));
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            RefreshMeshGraph();

            timer2.Enabled = false;
        }

        public void SetSelectedTabPageIndex(int tabpageindex)
        {
            xtraTabControl1.SelectedTabPageIndex = tabpageindex;
            Invalidate();
        }

        private void CastViewTypeChangedEvent()
        {
            if (onViewTypeChanged != null)
            {
                onViewTypeChanged(this, new ViewTypeChangedEventArgs(m_viewtype, m_map_name));
                m_previousviewtype = m_viewtype;
            }
        }

        private void CastGraphSelectionChangedEvent()
        {
            if (onGraphSelectionChanged != null)
            {
                onGraphSelectionChanged(this, new GraphSelectionChangedEventArgs(xtraTabControl1.SelectedTabPageIndex, m_map_name));
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                // 3d graph
                Console.WriteLine("RefreshMeshGraph on tabindex changed");

                RefreshMeshGraph();
            }
            else
            {
                UpdateChartControlSlice(GetDataFromGridView(false));
            }
            CastGraphSelectionChangedEvent();
        }

        private void chartControl1_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {
            
        }

        private void chartControl1_MouseUp(object sender, MouseEventArgs e)
        {
            m_isDragging = false;
            _sp_dragging = null;
            timer4.Enabled = false;
        }

        private int GetRowForAxisValue(string axisvalue)
        {
            int retval = -1;
            for (int t = 0; t < y_axisvalues.Length; t++)
            {
                if (y_axisvalues.GetValue(t).ToString() == axisvalue)
                {
                    retval = (y_axisvalues.Length - 1) - t;
                }
            }
            return retval;
        }

        private void SetDataValueInMap(string yaxisvalue, double datavalue)
        {
            int rowhandle = GetRowForAxisValue(yaxisvalue);
            if (m_TableWidth == 1)
            {
                // single column graph.. 
                int numberofrows = m_map_length;
                if (m_issixteenbit) numberofrows /= 2;
                rowhandle = (numberofrows -1)- Convert.ToInt32(yaxisvalue);
            }
            if (rowhandle != -1)
            {
                double newvalue = (datavalue - correction_offset) * (1/correction_factor);
                if (newvalue >= 0)
                {
                    gridView1.SetRowCellValue(rowhandle, gridView1.Columns[(int)trackBarControl1.Value], Math.Round(newvalue));
                }
            }
            
        }

        private void chartControl1_MouseMove(object sender, MouseEventArgs e)
        {
            /*if (m_isDragging)
            {
                int delta_y = e.Y - _mouse_drag_y;
                if (Math.Abs(delta_y) > 0)//be able to hover!
                {
                    _mouse_drag_y = e.Y;
                    _mouse_drag_x = e.X;

                    
                    double yaxissize = (double)((XYDiagram)chartControl1.Diagram).AxisY.Range.MaxValue - (double)((XYDiagram)chartControl1.Diagram).AxisY.Range.MinValue;

                    double yaxissizepxls = chartControl1.Height - (((XYDiagram)chartControl1.Diagram).Padding.Top + ((XYDiagram)chartControl1.Diagram).Padding.Bottom) - (chartControl1.Margin.Top + chartControl1.Margin.Bottom);
                    yaxissizepxls -= 64;//(yaxissizepxls / 6);
                    //chartControl1.d
                    double deltavalue = delta_y * (yaxissize / yaxissizepxls);
                    //deltavalue -= correction_offset;
                    //deltavalue *= 1 / correction_factor;
                    //Console.WriteLine("Delta: " + deltavalue.ToString());
                    if (_sp_dragging != null)
                    {
                        double curval = Convert.ToDouble(_sp_dragging.Values.GetValue(0));
                        double newvalue = (curval - deltavalue);
                        // if (newvalue < 0) newvalue = 0;
                        //Console.WriteLine("Current: " + curval.ToString() + " delta: " + deltavalue.ToString() + " new: " + newvalue.ToString());
                        _sp_dragging.Values.SetValue(newvalue, 0);
                        DataTable dt = (DataTable)chartControl1.DataSource;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr[0].ToString() == _sp_dragging.Argument)
                            {
                                dr[1] = newvalue;
                                // zet ook de betreffende waarde in de tabel!
                                SetDataValueInMap(_sp_dragging.Argument, newvalue);
                                //Console.WriteLine("Written: " + _sp_dragging.Argument + " : " + newvalue);
                                //sp.Values.SetValue(curval - 1, 0);
                                //chartControl1.Invalidate();
                            }
                        }

                    }
                }
            }*/
        }


        private void groupControl1_DoubleClick(object sender, EventArgs e)
        {
            gridView1.OptionsView.AllowCellMerge = !gridView1.OptionsView.AllowCellMerge;
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            nChartControl1.Charts[0].Projection.Zoom += 5;
            nChartControl1.Refresh();
            CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);

        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            nChartControl1.Charts[0].Projection.Zoom -= 5;
            nChartControl1.Refresh();
            CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            nChartControl1.Charts[0].Projection.Rotation += 5;
            nChartControl1.Refresh();
            CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            nChartControl1.Charts[0].Projection.Rotation -= 5;
            nChartControl1.Refresh();
            CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
           /* DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length == 1)
            {
                gridView1.ShowEditor();
            }*/            
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
             /*DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
             if (cellcollection.Length == 1)
             {
                 e.RepositoryItem = MapViewerCellEdit;
             }
             else
             {
                 e.RepositoryItem = null;
             }*/
        }

        private void MapViewerCellEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextEdit)
            {
                TextEdit txtedit = (TextEdit)sender;
                if (e.KeyCode == Keys.Add)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    //if (IsHexMode)
                    if (m_viewtype == ViewType.Hexadecimal)
                    {
                        int value = Convert.ToInt32(txtedit.Text, 16);
                        value++;
                        if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                        if (m_issixteenbit)
                        {
                            if (value > 0xFFFF) value = 0xFFFF;
                            txtedit.Text = value.ToString("X4");
                        }
                        else
                        {

                            if (value > 0xFF) value = 0xFF;
                            txtedit.Text = value.ToString("X2");
                        }
                    }
                    else
                    {
                        int value = Convert.ToInt32(txtedit.Text);
                        value++;
                        if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                        if (m_issixteenbit)
                        {
                            if (value > 0xFFFF) value = 0xFFFF;
                            txtedit.Text = value.ToString();
                        }
                        else
                        {
                            if (value > 0xFF) value = 0xFF;
                            txtedit.Text = value.ToString();
                        }

                    }

                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    //if (IsHexMode)
                    if (m_viewtype == ViewType.Hexadecimal)
                    {
                        int value = Convert.ToInt32(txtedit.Text, 16);
                        value--;
                        if (value < 0) value = 0;
                        if (m_issixteenbit)
                        {
                            txtedit.Text = value.ToString("X4");
                        }
                        else
                        {
                            txtedit.Text = value.ToString("X2");
                        }
                    }
                    else
                    {
                        int value = Convert.ToInt32(txtedit.Text);
                        value--;
                        if (value < 0) value = 0;
                        if (m_issixteenbit)
                        {
                            txtedit.Text = value.ToString();
                        }
                        else
                        {
                            txtedit.Text = value.ToString();
                        }

                    }

                }
                /*else if (e.KeyCode == Keys.PageUp)
                {
                    e.Handled = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        if (IsHexMode)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value += 0x10;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                if (value > 0xFF) value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value += 10;
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;
                            if (m_issixteenbit)
                            {
                                if (value > 0xFFFF) value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                if (value > 0xFF) value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    e.Handled = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        if (IsHexMode)
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString(), 16);
                            value -= 0x10;
                            if (value < 0) value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = Convert.ToInt32(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                            value -= 10;
                            if (value < 0) value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }
                else if (e.KeyCode == Keys.Home)
                {
                    e.Handled = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        if (IsHexMode)
                        {

                            int value = 0xFFFF;
                            if (m_issixteenbit)
                            {
                                value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;

                        }
                        else
                        {
                            int value = 0xFFFF;
                            if (m_issixteenbit)
                            {
                                value = 0xFFFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                value = 0xFF;
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            if (value > m_MaxValueInTable) m_MaxValueInTable = value;

                        }
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    e.Handled = true;
                    foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                    {
                        if (IsHexMode)
                        {
                            int value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X4"));
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString("X2"));
                            }
                        }
                        else
                        {
                            int value = 0;
                            if (m_issixteenbit)
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }
                            else
                            {
                                gridView1.SetRowCellValue(gc.RowHandle, gc.Column, value.ToString());
                            }

                        }
                    }
                }*/
            }

        }

        private void CopySelectionToClipboard()
        {
            try
            {
                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                CellHelperCollection chc = new CellHelperCollection();
                foreach (DevExpress.XtraGrid.Views.Base.GridCell gc in cellcollection)
                {
                    CellHelper ch = new CellHelper();
                    ch.Rowhandle = gc.RowHandle;
                    ch.Columnindex = gc.Column.AbsoluteIndex;
                    object o = gridView1.GetRowCellValue(gc.RowHandle, gc.Column);
                    if (m_viewtype == ViewType.Hexadecimal)
                    {
                        ch.Value = Convert.ToInt32(o.ToString(), 16);
                    }
                    else
                    {
                        ch.Value = Convert.ToInt32(o);
                    }
                    chc.Add(ch);
                }
                string serialized = ((int)m_viewtype).ToString();//string.Empty;
                foreach (CellHelper ch in chc)
                {
                    serialized += ch.Columnindex.ToString() + ":" + ch.Rowhandle.ToString() + ":" + ch.Value.ToString() + ":~";
                }

                Clipboard.SetText(serialized);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void CopyMapToClipboard()
        {
            gridView1.SelectAll();
            CopySelectionToClipboard();
            gridView1.ClearSelection();
        }

        private void copySelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length > 0)
            {
                CopySelectionToClipboard();
            }
            else
            {
                if (MessageBox.Show("No selection, copy the entire map?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CopyMapToClipboard();
                }
            }
        }

        private void atCurrentlySelectedLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // Clipboard.ContainsData("System.Object");
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length >= 1)
            {
                try
                {
                    int rowhandlefrom = cellcollection[0].RowHandle;
                    int colindexfrom = cellcollection[0].Column.AbsoluteIndex;
                    int originalrowoffset = -1;
                    int originalcolumnoffset = -1;
                    if (Clipboard.ContainsText())
                    {
                        string serialized = Clipboard.GetText();
                        //   Console.WriteLine(serialized);
                        int viewtypeinclipboard = Convert.ToInt32(serialized.Substring(0, 1));
                        ViewType vtclip = (ViewType)viewtypeinclipboard;
                        serialized = serialized.Substring(1);
                        char[] sep = new char[1];
                        sep.SetValue('~', 0);
                        string[] cells = serialized.Split(sep);
                        foreach (string cell in cells)
                        {
                            char[] sep2 = new char[1];
                            sep2.SetValue(':', 0);
                            string[] vals = cell.Split(sep2);
                            if (vals.Length >= 3)
                            {

                                int rowhandle = Convert.ToInt32(vals.GetValue(1));
                                int colindex = Convert.ToInt32(vals.GetValue(0));
                                int ivalue = 0;
                                double dvalue = 0;
                                if (vtclip == ViewType.Hexadecimal)
                                {
                                    ivalue = Convert.ToInt32(vals.GetValue(2).ToString());
                                    dvalue = ivalue;
                                }
                                else if (vtclip == ViewType.Decimal)
                                {
                                    ivalue = Convert.ToInt32(vals.GetValue(2));
                                    dvalue = ivalue;
                                }
                                else if (vtclip == ViewType.Easy )
                                {
                                    dvalue = Convert.ToDouble(vals.GetValue(2));
                                }

                                if (originalrowoffset == -1) originalrowoffset = rowhandle;
                                if (originalcolumnoffset == -1) originalcolumnoffset = colindex;
                                if (rowhandle >= 0 && colindex >= 0)
                                {
                                    try
                                    {
                                        if (vtclip == ViewType.Hexadecimal)
                                        {
                                            //gridView1.SetRowCellValue(rowhandle, gridView1.Columns[colindex], ivalue.ToString("X"));
                                            gridView1.SetRowCellValue(rowhandlefrom + (rowhandle - originalrowoffset), gridView1.Columns[colindexfrom + (colindex - originalcolumnoffset)], ivalue.ToString("X"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(rowhandlefrom + (rowhandle - originalrowoffset), gridView1.Columns[colindexfrom + (colindex - originalcolumnoffset)], dvalue);
                                        }

                                    }
                                    catch (Exception E)
                                    {
                                        Console.WriteLine(E.Message);
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception pasteE)
                {
                    Console.WriteLine(pasteE.Message);
                }
            }
        }

        private void inOrgininalPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string serialized = Clipboard.GetText();
                try
                {
                    //   Console.WriteLine(serialized);
                    int viewtypeinclipboard = Convert.ToInt32(serialized.Substring(0, 1));
                    ViewType vtclip = (ViewType)viewtypeinclipboard;
                    serialized = serialized.Substring(1);

                    char[] sep = new char[1];
                    sep.SetValue('~', 0);
                    string[] cells = serialized.Split(sep);
                    foreach (string cell in cells)
                    {
                        char[] sep2 = new char[1];
                        sep2.SetValue(':', 0);
                        string[] vals = cell.Split(sep2);
                        if (vals.Length >= 3)
                        {
                            int rowhandle = Convert.ToInt32(vals.GetValue(1));
                            int colindex = Convert.ToInt32(vals.GetValue(0));
                            //int value = Convert.ToInt32(vals.GetValue(2));
                            int ivalue = 0;
                            double dvalue = 0;
                            if (vtclip == ViewType.Hexadecimal)
                            {
                                ivalue = Convert.ToInt32(vals.GetValue(2).ToString());
                                dvalue = ivalue;
                            }
                            else if (vtclip == ViewType.Decimal)
                            {
                                ivalue = Convert.ToInt32(vals.GetValue(2));
                                dvalue = ivalue;
                            }
                            else if (vtclip == ViewType.Easy)
                            {
                                dvalue = Convert.ToDouble(vals.GetValue(2));
                            }
                            if (rowhandle >= 0 && colindex >= 0)
                            {
                                try
                                {
                                    if (vtclip == ViewType.Hexadecimal)
                                    {
                                        gridView1.SetRowCellValue(rowhandle, gridView1.Columns[colindex], ivalue.ToString("X"));
                                    }
                                    else
                                    {
                                        gridView1.SetRowCellValue(rowhandle, gridView1.Columns[colindex], dvalue);
                                    }
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine(E.Message);
                                }
                            }
                        }

                    }
                }
                catch (Exception pasteE)
                {
                    Console.WriteLine(pasteE.Message);
                }
            }
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
//            m_isHexMode = !m_isHexMode;
            if (m_viewtype != ViewType.Hexadecimal) m_viewtype = ViewType.Hexadecimal;
            else m_viewtype = m_previousviewtype;
            ShowTable(m_TableWidth, m_issixteenbit);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                double _workvalue = Convert.ToDouble(toolStripTextBox1.Text);
                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                if (cellcollection.Length > 0)
                {
                    switch (toolStripComboBox1.SelectedIndex)
                    {
                        case 0: // addition
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                try
                                {
                                    int value = 0;
                                    if (m_viewtype == ViewType.Hexadecimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString(), 16);
                                        value += (int)Math.Round(_workvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == ViewType.Decimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value += (int)Math.Round(_workvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    else if (m_viewtype == ViewType.Easy)
                                    {
                                        double dvalue = Convert.ToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue += _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        value = (int)dvalue;

                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                            }
                            break;
                        case 1: // multiply
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                try
                                {
                                    int value = 0;
                                    if (m_viewtype == ViewType.Hexadecimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString(), 16);
                                        value *= (int)Math.Round(_workvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == ViewType.Decimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        value *= (int)Math.Round(_workvalue);
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    
                                    else if (m_viewtype == ViewType.Easy)
                                    {
                                        double dvalue = Convert.ToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        dvalue *= _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        value = (int)dvalue;

                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }

                            }
                            break;
                        case 2: // divide
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                if (_workvalue == 0) _workvalue = 1;
                                try
                                {
                                    int value = 0;
                                    if (m_viewtype == ViewType.Hexadecimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column).ToString(), 16);
                                        if (_workvalue != 0)
                                        {
                                            value /= (int)Math.Round(_workvalue);
                                        }
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == ViewType.Decimal)
                                    {
                                        value = Convert.ToInt32(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        if (_workvalue != 0)
                                        {
                                            value /= (int)Math.Round(_workvalue);
                                        }
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    
                                    else if (m_viewtype == ViewType.Easy)
                                    {
                                        double dvalue = Convert.ToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                                        dvalue *= correction_factor;
                                        dvalue += correction_offset;
                                        if (_workvalue != 0)
                                        {
                                            dvalue /= _workvalue;
                                        }
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        value = (int)dvalue;

                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                           // if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, value.ToString());
                                    }
                                    
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                            }
                            break;
                        case 3: // fill
                            foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                            {
                                try
                                {
                                    double value = _workvalue;
                                    if (m_viewtype == ViewType.Hexadecimal)
                                    {
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                           // if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        int dvalue = (int)value;
                                        if (value > 0xFF)
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString("X4"));
                                        }
                                        else
                                        {
                                            gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString("X2"));
                                        }
                                    }
                                    else if (m_viewtype == ViewType.Decimal)
                                    {
                                        if (m_issixteenbit)
                                        {
                                            if (value > 0xFFFF) value = 0xFFFF;
                                            //if (value < 0) value = 0;
                                        }
                                        else
                                        {
                                            if (value > 0xFF) value = 0xFF;
                                            if (value < 0) value = 0;
                                        }
                                        int dvalue = (int)value;
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, dvalue.ToString());
                                    }
                                    
                                    else if (m_viewtype == ViewType.Easy)
                                    {
                                        double dvalue = _workvalue;
                                        dvalue -= correction_offset;
                                        if (correction_factor != 0)
                                        {
                                            dvalue /= correction_factor;
                                        }
                                        int decvalue = (int)dvalue;
                                        if (m_issixteenbit)
                                        {
                                            if (decvalue > 0xFFFF) decvalue = 0xFFFF;
                                          //  if (decvalue < 0) decvalue = 0;
                                        }
                                        else
                                        {
                                            if (decvalue > 0xFF) decvalue = 0xFF;
                                            if (decvalue < 0) decvalue = 0;
                                        }
                                        gridView1.SetRowCellValue(cell.RowHandle, cell.Column, decvalue.ToString());
                                    }
                                    

                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_map_name != string.Empty && !m_prohibitlock_change)
            {
                switch (toolStripComboBox2.SelectedIndex)
                {
                    case 0: // autoscale
                        CastLockEvent(0);
                        break;
                    case 1: // peak values
                        CastLockEvent(1);
                        break;
                    case 2: // max possible
                        CastLockEvent(2);
                        break;
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.Parent is DevExpress.XtraBars.Docking.DockPanel)
            {
                DevExpress.XtraBars.Docking.DockPanel pnl = (DevExpress.XtraBars.Docking.DockPanel)this.Parent;
                if (pnl.FloatForm == null)
                {
                    pnl.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    pnl.FloatLocation = new System.Drawing.Point(1, 1);
                    pnl.MakeFloat();
                    // alleen grafiek
                    splitContainer1.Panel1Collapsed = true;
                    splitContainer1.Panel2Collapsed = false;
                }
                else
                {
                    pnl.Restore();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }
            }
            else if (this.Parent is DevExpress.XtraBars.Docking.ControlContainer)
            {
                DevExpress.XtraBars.Docking.ControlContainer container = (DevExpress.XtraBars.Docking.ControlContainer)this.Parent;
                if (container.Panel.FloatForm == null)
                {
                    container.Panel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    container.Panel.FloatLocation = new System.Drawing.Point(1, 1);
                    container.Panel.MakeFloat();
                    splitContainer1.Panel1Collapsed = true;
                    splitContainer1.Panel2Collapsed = false;
                }
                else
                {
                    container.Panel.Restore();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }
                
            }
            CastSplitterMovedEvent();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (this.Parent is DevExpress.XtraBars.Docking.DockPanel)
            {
                DevExpress.XtraBars.Docking.DockPanel pnl = (DevExpress.XtraBars.Docking.DockPanel)this.Parent;
                if (pnl.FloatForm == null)
                {
                    pnl.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    pnl.FloatLocation = new System.Drawing.Point(1, 1);
                    pnl.MakeFloat();
                    // alleen grafiek
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = true;
                }
                else
                {
                    pnl.Restore();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }
            }
            else if (this.Parent is DevExpress.XtraBars.Docking.ControlContainer)
            {
                DevExpress.XtraBars.Docking.ControlContainer container = (DevExpress.XtraBars.Docking.ControlContainer)this.Parent;
                if (container.Panel.FloatForm == null)
                {
                    container.Panel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    container.Panel.FloatLocation = new System.Drawing.Point(1, 1);
                    container.Panel.MakeFloat();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = true;
                }
                else
                {
                    container.Panel.Restore();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }

            }
            CastSplitterMovedEvent();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (this.Parent is DevExpress.XtraBars.Docking.DockPanel)
            {
                DevExpress.XtraBars.Docking.DockPanel pnl = (DevExpress.XtraBars.Docking.DockPanel)this.Parent;
                if (pnl.FloatForm == null)
                {
                    pnl.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    pnl.FloatLocation = new System.Drawing.Point(1, 1);
                    pnl.MakeFloat();
                    // alleen grafiek
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }
                else
                {
                    pnl.Restore();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }
            }
            else if (this.Parent is DevExpress.XtraBars.Docking.ControlContainer)
            {
                DevExpress.XtraBars.Docking.ControlContainer container = (DevExpress.XtraBars.Docking.ControlContainer)this.Parent;
                if (container.Panel.FloatForm == null)
                {
                    container.Panel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    container.Panel.FloatLocation = new System.Drawing.Point(1, 1);
                    container.Panel.MakeFloat();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }
                else
                {
                    container.Panel.Restore();
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = false;
                }

            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1Collapsed)
            {
                splitContainer1.Panel1Collapsed = false;
                splitContainer1.Panel2Collapsed = true;
            }
            else if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel1Collapsed = false;
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                splitContainer1.Panel1Collapsed = true;
                splitContainer1.Panel2Collapsed = false;
            }
            CastSplitterMovedEvent();
        }

        bool tmr_toggle = false;

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (_sp_dragging != null)
            {
                string yaxisvalue = _sp_dragging.Argument;
                int rowhandle = GetRowForAxisValue(yaxisvalue);
                if (m_TableWidth == 1)
                {
                    // single column graph.. 
                    int numberofrows = m_map_length;
                    if (m_issixteenbit) numberofrows /= 2;
                    rowhandle = (numberofrows - 1) - Convert.ToInt32(yaxisvalue);
                }
                if (rowhandle != -1)
                {
                    if (tmr_toggle)
                    {
                        gridView1.SelectCell(rowhandle, gridView1.Columns[(int)trackBarControl1.Value]);
                        tmr_toggle = false;
                    }
                    else
                    {
                        gridView1.ClearSelection();
                        tmr_toggle = true;
                    }
                }
            }
        }

        private void popupContainerEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
//            e.Value = System.IO.Path.GetFileName(m_filename) + " : " + m_map_name + " flash address : " + m_map_address.ToString("X6") + " sram address : " + m_map_sramaddress.ToString("X4");
        }

        private void gridView1_SelectionChanged_1(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (!m_prohibitcellchange)
            {
                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                if (cellcollection.Length == 1)
                {
                    object o = cellcollection.GetValue(0);
                    if (o is DevExpress.XtraGrid.Views.Base.GridCell)
                    {
                        DevExpress.XtraGrid.Views.Base.GridCell cell = (DevExpress.XtraGrid.Views.Base.GridCell)o;
                        CastSelectEvent(cell.RowHandle, cell.Column.AbsoluteIndex);
                    }

                }
            }
            
        }

        private bool m_split_dragging = false;

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (m_split_dragging)
            {
                m_split_dragging = false;
                Console.WriteLine("Splitter moved: " + splitContainer1.Panel1.Height.ToString() + ":" + splitContainer1.Panel2.Height.ToString() + splitContainer1.Panel1Collapsed.ToString() + ":" + splitContainer1.Panel2Collapsed.ToString());
                CastSplitterMovedEvent();
            }
        }

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            m_split_dragging = true;
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
         
        }

        private void splitContainer1_MouseLeave(object sender, EventArgs e)
        {

        }


        internal void SetSurfaceGraphZoom(double pov_d)
        {
            try
            {
                m_prohibitgraphchange = true;
                m_prohibitgraphchange = false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        public void SetSurfaceGraphView(int pov_x, int pov_y, int pov_z, int pan_x, int pan_y, double pov_d)
        {
            try
            {
                m_prohibitgraphchange = true;
                m_prohibitgraphchange = false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }

        public void SetSurfaceGraphViewEx(float depthx, float depthy, float zoom, float rotation, float elevation)
        {
            try
            {
                nChartControl1.Charts[0].Projection.XDepth = depthx;
                nChartControl1.Charts[0].Projection.YDepth = depthy;
                nChartControl1.Charts[0].Projection.Zoom = zoom;
                nChartControl1.Charts[0].Projection.Rotation = rotation;
                nChartControl1.Charts[0].Projection.Elevation = elevation;
                nChartControl1.Refresh();
            }
            catch (Exception E)
            {
                Console.WriteLine("SetSurfaceGraphViewEx:" + E.Message);
            }

        }

        private void toolStripComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // refresh the mapviewer with the values like selected
            if (m_prohibit_viewchange) return;
            switch (toolStripComboBox3.SelectedIndex)
            {
                case -1:
                case 0:
                    m_viewtype = ViewType.Hexadecimal;
                    break;
                case 1:
                    m_viewtype = ViewType.Decimal;
                    break;
                case 2:
                    m_viewtype = ViewType.Easy;
                    break;
                case 3:
                    m_viewtype = ViewType.ASCII;
                    break;
                default:
                    m_viewtype = ViewType.Hexadecimal;
                    break;
            }
            ReShowTable();
            CastViewTypeChangedEvent();
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
           
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (m_issixteenbit)
            {
                if (m_viewtype == ViewType.Hexadecimal)
                {
                    try
                    {
                        int value = Convert.ToInt32(Convert.ToString(e.Value), 16);
                        if (value > 0xFFFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                    }
                    catch (Exception hE)
                    {
                        e.Valid = false;
                        e.ErrorText = hE.Message;
                    }
                    /*value = 0xFFFF;
                     e.Value = value.ToString("X4");*/
                }
                else
                {
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == ViewType.Easy)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
/*                                if(value < 0)
                                {
                                    value ^= 0xffffff;
                                    value = -value;
                                }*/
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }
                    if (Math.Abs(value) > 78643)
                    {
                        e.Valid = false;
                        e.ErrorText = "Value not valid...";
                    }
                    else
                    {
                        e.Value = value;
                    }
                }

            }
            else
            {
                if (m_viewtype == ViewType.Hexadecimal)
                {
                    try
                    {
                        int value = Convert.ToInt32(Convert.ToString(e.Value), 16);
                        if (value > 0xFF)
                        {
                            e.Valid = false;
                            e.ErrorText = "Value not valid...";
                        }
                    }
                    catch(Exception hE)
                    {
                        e.Valid = false;
                        e.ErrorText = hE.Message;
                    }
                }
                else
                {
                    double dvalue = Convert.ToDouble(e.Value);
                    int value = 0;
                    if (m_viewtype == ViewType.Easy)
                    {
                        if (gridView1.ActiveEditor != null)
                        {
                            if (gridView1.ActiveEditor.EditValue.ToString() != gridView1.ActiveEditor.OldEditValue.ToString())
                            {
                                Console.WriteLine(gridView1.ActiveEditor.IsModified.ToString());
                                dvalue = Convert.ToDouble(gridView1.ActiveEditor.EditValue);
                                value = Convert.ToInt32((dvalue - correction_offset) / correction_factor);
                            }
                            else
                            {
                                value = Convert.ToInt32(Convert.ToString(e.Value));
                            }
                        }
                        else
                        {
                            value = Convert.ToInt32(Convert.ToString(e.Value));
                        }
                    }
                    else
                    {
                        value = Convert.ToInt32(Convert.ToString(e.Value));
                    }

                    if (value > 255)
                    {
                        e.Valid = false;
                        e.ErrorText = "Value not valid...";
                    }
                    else
                    {
                        e.Value = value;
                    }
                }
            }
        }

        private void popupContainerEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = System.IO.Path.GetFileName(m_filename) + " : " + m_map_name + " flash address : " + m_map_address.ToString("X6") + " sram address : " + m_map_sramaddress.ToString("X4");
        }

        private float ConvertToEasyValue(float editorvalue)
        {
            float retval = editorvalue;
            if (m_viewtype == ViewType.Easy )
            {
                retval = (float)((float)editorvalue * (float)correction_factor) + (float)correction_offset;
            }
            return retval;
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            /*
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "CityCode" && view.ActiveEditor is LookUpEdit)
            {
                Text = view.ActiveEditor.Parent.Name;
                DevExpress.XtraEditors.LookUpEdit edit;
                edit = (LookUpEdit)view.ActiveEditor;

                DataTable table = edit.Properties.LookUpData.DataSource as DataTable;
                clone = new DataView(table);
                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                clone.RowFilter = "[CountryCode] = " + row["CountryCode"].ToString();
                edit.Properties.LookUpData.DataSource = clone;
            }
            */
            if (m_viewtype == ViewType.Easy )
            {
                gridView1.ActiveEditor.EditValue = ConvertToEasyValue((float)Convert.ToDouble(gridView1.ActiveEditor.EditValue)).ToString("F2");
                Console.WriteLine("Started editor with value: " + gridView1.ActiveEditor.EditValue.ToString());
            }
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
        }

        private void gridView1_HiddenEditor(object sender, EventArgs e)
        {
            Console.WriteLine("Hidden editor with value: " + gridView1.GetFocusedRowCellDisplayText(gridView1.FocusedColumn));
        }

        private void MapViewer_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
            }
        }

        internal void ClearSelection()
        {
            gridView1.ClearSelection();
        }

        private int m_selectedrowhandle = -1;
        private int m_selectedcolumnindex = -1;


        public void HighlightCell(int tpsindex, int rpmindex)
        {
            /*
          //  gridView1.BeginUpdate();
            try
            {
                // controleer of het verandert is
                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                if (cellcollection.Length > 1)
                {
                    gridView1.ClearSelection();
                }
                else if (cellcollection.Length == 1)
                {
                    // normal situation
                    DevExpress.XtraGrid.Views.Base.GridCell cell = (DevExpress.XtraGrid.Views.Base.GridCell)cellcollection.GetValue(0);
                    if (cell.RowHandle != (15 - rpmindex) || cell.Column.AbsoluteIndex != tpsindex)
                    {
                        gridView1.ClearSelection();
                        gridView1.SelectCell(15 - rpmindex, gridView1.Columns[tpsindex]);
                    }
                }
                else
                {
                    gridView1.SelectCell(15 - rpmindex, gridView1.Columns[tpsindex]);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
          //  gridView1.EndUpdate();*/

            //  gridView1.BeginUpdate();
            try
            {
                int numberofrows = m_map_content.Length / m_TableWidth;
                if (m_issixteenbit)
                {
                    numberofrows /= 2;
                }

                // controleer of het verandert is
                /*                DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
                                if (cellcollection.Length > 1)
                                {
                                    gridView1.ClearSelection();
                                }
                                else if (cellcollection.Length == 1)
                                {
                                    // normal situation
                                    DevExpress.XtraGrid.Views.Base.GridCell cell = (DevExpress.XtraGrid.Views.Base.GridCell)cellcollection.GetValue(0);

                                    if (cell.RowHandle != ( (numberofrows - 1) - rpmindex) || cell.Column.AbsoluteIndex != tpsindex)
                                    {
                                        gridView1.ClearSelection();
                                        gridView1.SelectCell((numberofrows - 1) - rpmindex, gridView1.Columns[tpsindex]);
                                    }
                                }
                                else
                                {
                                    gridView1.SelectCell((numberofrows-1) - rpmindex, gridView1.Columns[tpsindex]);
                                }*/
                m_selectedrowhandle = (numberofrows - 1) - rpmindex;
                m_selectedcolumnindex = tpsindex;

                if (m_selectedrowhandle > numberofrows) m_selectedrowhandle = numberofrows;
                if (m_selectedcolumnindex > m_TableWidth) m_selectedcolumnindex = m_TableWidth;

                gridControl1.Invalidate();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            //  gridView1.EndUpdate();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            SymbolAxesTranslator sat = new SymbolAxesTranslator();
            string x = string.Empty;
            if (x_axisvalues.Length > 0 && x_axisAddress > 0)
            {
                x = X_axis_name + " (" + x_axisAddress.ToString("X8") + ")";
            }

            string y = string.Empty;
            if (y_axisvalues.Length > 0 && y_axisAddress > 0)
            {
                y = Y_axis_name + " (" + y_axisAddress.ToString("X8") + ")";
            }

            if (x != string.Empty)
            {
                editXaxisSymbolToolStripMenuItem.Enabled = true;
                editXaxisSymbolToolStripMenuItem.Text = "Edit x-axis (" + x + ")";
            }
            else
            {
                editXaxisSymbolToolStripMenuItem.Enabled = false;
                editXaxisSymbolToolStripMenuItem.Text = "Edit x-axis";
            }
            if (y != string.Empty)
            {
                editYaxisSymbolToolStripMenuItem.Enabled = true;
                editYaxisSymbolToolStripMenuItem.Text = "Edit y-axis (" + y + ")";
            }
            else
            {
                editYaxisSymbolToolStripMenuItem.Enabled = false;
                editYaxisSymbolToolStripMenuItem.Text = "Edit y-axis";
            }
        }

        private void editXaxisSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CastEditXaxisEditorRequested();
        }

        private void CastEditXaxisEditorRequested()
        {
            if (onAxisEditorRequested != null)
            {
                onAxisEditorRequested(this, new AxisEditorRequestedEventArgs(AxisIdent.X_Axis, m_map_name, m_filename));
            }
        }

        private void editYaxisSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CastEditYaxisEditorRequested();
        }

        private void CastEditYaxisEditorRequested()
        {
            if (onAxisEditorRequested != null)
            {
                onAxisEditorRequested(this, new AxisEditorRequestedEventArgs(AxisIdent.Y_Axis, m_map_name, m_filename));
            }
        }

        private void CastReadFromSRAM()
        {
            if (onReadFromSRAM != null)
            {
                onReadFromSRAM(this, new ReadFromSRAMEventArgs(m_map_name));
            }
        }

        private void CastWriteToSRAM()
        {
            if (onWriteToSRAM != null)
            {
                onWriteToSRAM(this, new WriteToSRAMEventArgs(m_map_name, GetDataFromGridView(m_isUpsideDown)));
            }
        }

        private void smoothSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_viewtype == ViewType.Hexadecimal)
            {
                MessageBox.Show("Smoothing cannot be done in Hex view!");
                return;
            }
            DevExpress.XtraGrid.Views.Base.GridCell[] cellcollection = gridView1.GetSelectedCells();
            if (cellcollection.Length > 2)
            {
                // get boundaries for this selection
                // we need 4 corners 
                int max_column = 0;
                int min_column = 0xFFFF;
                int max_row = 0;
                int min_row = 0xFFFF;
                foreach (DevExpress.XtraGrid.Views.Base.GridCell cell in cellcollection)
                {
                    if (cell.Column.AbsoluteIndex > max_column) max_column = cell.Column.AbsoluteIndex;
                    if (cell.Column.AbsoluteIndex < min_column) min_column = cell.Column.AbsoluteIndex;
                    if (cell.RowHandle > max_row) max_row = cell.RowHandle;
                    if (cell.RowHandle < min_row) min_row = cell.RowHandle;
                }
                if (max_column == min_column)
                {
                    // one column selected only
                    int top_value = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[max_column]));
                    int bottom_value = Convert.ToInt32(gridView1.GetRowCellValue(min_row, gridView1.Columns[max_column]));
                    double diffvalue = (top_value - bottom_value) / (cellcollection.Length - 1);
                    for (int t = 1; t < cellcollection.Length - 1; t++)
                    {
                        double newvalue = bottom_value + (t * diffvalue);
                        gridView1.SetRowCellValue(min_row + t, gridView1.Columns[max_column], newvalue);
                    }

                }
                else if (max_row == min_row)
                {
                    // one row selected only
                    int top_value = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[max_column]));
                    int bottom_value = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[min_column]));
                    double diffvalue = (top_value - bottom_value) / (cellcollection.Length - 1);
                    for (int t = 1; t < cellcollection.Length - 1; t++)
                    {
                        double newvalue = bottom_value + (t * diffvalue);
                        gridView1.SetRowCellValue(min_row, gridView1.Columns[min_column + t], newvalue);
                    }
                }
                else
                {
                    // block selected
                    // interpolation on 4 points!!!
                    int top_leftvalue = Convert.ToInt32(gridView1.GetRowCellValue(min_row, gridView1.Columns[min_column]));
                    int top_rightvalue = Convert.ToInt32(gridView1.GetRowCellValue(min_row, gridView1.Columns[max_column]));
                    int bottom_leftvalue = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[min_column]));
                    int bottom_rightvalue = Convert.ToInt32(gridView1.GetRowCellValue(max_row, gridView1.Columns[max_column]));

                    for (int tely = 1; tely < max_row - min_row; tely++)
                    {
                        for (int telx = 1; telx < max_column - min_column; telx++)
                        {
                            // get values 
                            double valx1 = 0;
                            double valx2 = 0;
                            double valy1 = 0;
                            double valy2 = 0;

                            if (telx + min_column > 0)
                            {
                                valx1 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column - 1]));
                            }
                            else
                            {
                                valx1 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[min_column]));
                            }
                            if ((telx + min_column) < gridView1.Columns.Count - 1)
                            {
                                valx2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column + 1]));
                            }
                            else
                            {
                                valx2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column]));
                            }

                            if (tely + min_row > 0)
                            {
                                valy1 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row - 1, gridView1.Columns[telx + min_column]));
                            }
                            else
                            {
                                valy1 = Convert.ToDouble(gridView1.GetRowCellValue(min_row, gridView1.Columns[telx + min_column]));
                            }
                            if ((tely + min_row) < gridView1.RowCount - 1)
                            {
                                valy2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row + 1, gridView1.Columns[telx + min_column]));
                            }
                            else
                            {
                                valy2 = Convert.ToDouble(gridView1.GetRowCellValue(tely + min_row, gridView1.Columns[telx + min_column]));
                            }
                            //Console.WriteLine("valx1 = " + valx1.ToString() + " valx2 = " + valx2.ToString() + " valy1 = " + valy1.ToString() + " valy2 = " + valy2.ToString());
                            // x as 
                            double valuex = (valx2 + valx1) / 2;
                            double valuey = (valy2 + valy1) / 2;
                            float newvalue = (float)((valuex + valuey) / 2);

                            gridView1.SetRowCellValue(min_row + tely, gridView1.Columns[min_column + telx], newvalue.ToString("F0"));

                            m_map_content = GetDataFromGridView(m_isUpsideDown);



                            //double diffvaluex = (top_rightvalue - top_leftvalue) / (max_column - min_column - 1);
                            //double diffvaluey = (top_rightvalue - top_leftvalue) / (max_column - min_column - 1);
                        }
                    }

                }
//                simpleButton3.Enabled = false;
            }

        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            CastReadFromSRAM();
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            CastWriteToSRAM();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            timer5.Enabled = false;
            try
            {
                if (!m_datasourceMutated)
                {
                    CastReadFromSRAM(); // only if no changed made to gridView
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            timer5.Enabled = true;

        }

        private void toolStripComboBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // see if there's a selection being made
                gridView1.ClearSelection();
                string strValue = toolStripComboBox3.Text;
                char[] sep = new char[1];
                sep.SetValue(' ', 0);
                string[] strValues = strValue.Split(sep);
                foreach (string strval in strValues)
                {
                    double dblres;
                    if (Double.TryParse(strval, out dblres))
                    {
                        SelectCellsWithValue(dblres);
                    }
                }
                gridView1.Focus();

            }
        }

        private void SelectCellsWithValue(double value)
        {
            for (int rh = 0; rh < gridView1.RowCount; rh++)
            {
                for (int ch = 0; ch < gridView1.Columns.Count; ch++)
                {
                    try
                    {
                        object ov = gridView1.GetRowCellValue(rh, gridView1.Columns[ch]);


                        double val = Convert.ToDouble(ov);
                        val *= correction_factor;
                        val += correction_offset;
                        double diff = Math.Abs(val - value);
                        if (diff < 0.009)
                        {
                            gridView1.SelectCell(rh, gridView1.Columns[ch]);
                        }

                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to select cell: " + E.Message);
                    }

                }
            }
        }

        private void CastReadEvent()
        {
            if (onSymbolRead != null)
            {
                onSymbolRead(this, new ReadSymbolEventArgs(m_map_name, m_filename));
                m_datasourceMutated = false;
            }
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            CastReadEvent();
        }

    }
}
