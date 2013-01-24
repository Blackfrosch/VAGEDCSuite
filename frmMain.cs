//DONE: Make a codeblock sequencer.
//DONE: Make sure SOI maps are detected correctly (see screenshot of 4 axis maps issue)
//DONE: Make filelength a variable
//DONE: Axis collection, browser and editor support (refresh open maps after editing an axis)
//DONE: Bugfix, create new project causes System.UnauthorizedAccessException because of default path to Application.StartupPath
//DONE: Compare should not show infobox about missing axis
//DONE: Compare should use correct file for axis and addresses
//DONE: Add axis units to mapviewer
//DONE: Dynograph (torque(Nm) = IQ * 6
//DONE: Add support for flipping maps upside down
//DONE: Find launch control map (limiter actually) 0x4c640, corrupt axis starts with 0x80 0x00 0x00 0x0A
//DONE: Add checksum support
//DONE: axis change in compare mode?
//DONE: undo button in mapviewer (read from file?)
//DONE: EDC15V: sometimes also has three repeating smoke maps (watch the index IDs, they might be shuffled)
/*DONE: Additional information about codeblocks AG/SG indicator
G038906019HJ 1,9l R4 EDC  SG  5259 39S03217 0281010977 FFEWC400 038906019HJ 05/03 <-- Standard Getriebe codeblock 2 (normally manual)
G038906019HJ 1,9l R4 EDC  SG  5259 39S03217 0281010977 FFEWE400 038906019HJ 05/03 <-- Standard Getriebe codeblock 3 (normally 4motion)
G038906019HJ 1,9l R4 EDC  AG  5259 39S03217 0281010977 FFEWB400 038906019HJ 05/03 <-- Automat  Getriebe codeblock 1 (normally automatic)
 *                        ^                                ^
 * */
//DONE: ALSO use codeblock offset in codeblock synchronization option


//TEST: checksum issue with the ARL file (different structure), done for test!
//TEST: Userdescription for symbols + XML import/export
//HOLD: Find RPM limiter (ASZ @541A2 @641A2 and @741A2 value = 0x14B4 = 5300 rpm) <-- not very wise
//TEST: EDC15V: Don't forget the checksum for 1MB files
//TEST: EDC15V: Detect and fill codeBlocks
//HOLD: codeblocks: unreliable

//TODO: Rewrite XDF interface to TunerPro V5 XML specs (not documented...)
//TODO: Add EEPROM read/write support (K-line)
//TODO: KWP1281 support (K-line, slow init)
//TODO: Add EDC15P support ... 85%  LoHi
//TODO: Add EDC15V support ... 70%  LoHi 1MB, 512 Kb    (IMPROVE SUPPORT + CHECKSUM)
//TODO: Add EDC15M support ... 10%  LoHi (Opel?)        (CHECKSUM)
//TODO: Add EDC15C support ... 1%   LoHi                (CHECKSUM) 
//TODO: Add EDC16x support ... 5%   HiLo                (CHECKSUM)
//TODO: Add MSA15  support ... 25%  LoHi                (CHECKSUM)
//TODO: Add MSA6   support ... 0%   8-bit               (CHECKSUM) length indicators only (like ME7 and EDC16)
//TODO: Add EDC17  support ... 1%                       (CHECKSUM)
//TODO: Add major program switches (MAP/MAF switch etc)
//TODO: Add better hex-diff viewer (HexWorkShop example)
//TODO: Compressormap plotter with GT17 and most used upgrade turbo maps, boost/airmass from comperssor map
//TODO: copy from excel into mapviewer
//TODO: A2L/Damos import
//TODO: EDC15V: Don't forget the checksum for 256kB files
//TODO: Check older EDC15V-5 files.. these seem to be different
//TODO: Checksums: determine type on file open and count how many correct in stead of how many false? 12 banks in edc15v?
//TODO: Smoke limiter repeater is seen as a map (3x13) this is incorrect. Fix please vw passat bin @4DC72. 
//      (remember issue with len2Skip, we loose SOI limiter)
//TODO: make partnumber/swid editable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars;
using DevExpress.Skins;

namespace VAGSuite
{

    public delegate void DelegateStartReleaseNotePanel(string filename, string version);

    public enum GearboxType : int
    {
        Automatic,
        Manual,
        FourWheelDrive
    }

    public enum EDCFileType : int
    {
        EDC15P,
        EDC15P6, // different map structure
        EDC15V,
        EDC15M,
        EDC15C,
        EDC16,
        EDC17,  // 512Kb/2048Kb
        MSA6,
        MSA11,
        MSA12,
        MSA15,
        Unknown
    }

    public enum EngineType : int
    {
        cc1200,
        cc1400,
        cc1600,
        cc1900,
        cc2500
    }

    public enum ImportFileType : int
    {
        XML,
        A2L,
        CSV,
        AS2,
        Damos
    }

    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        private AppSettings m_appSettings;
        private msiupdater m_msiUpdater;
        public DelegateStartReleaseNotePanel m_DelegateStartReleaseNotePanel;
        private frmSplash splash;
        public frmMain()
        {
            try
            {
                splash = new frmSplash();
                splash.Show();
                Application.DoEvents();
            }
            catch (Exception)
            {

            }
                
            InitializeComponent();
            try
            {
                m_DelegateStartReleaseNotePanel = new DelegateStartReleaseNotePanel(this.StartReleaseNotesViewer);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }


        private void StartReleaseNotesViewer(string xmlfilename, string version)
        {
            dockManager1.BeginUpdate();
            DockPanel dp = dockManager1.AddPanel(DockingStyle.Right);
            dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
            dp.Tag = xmlfilename;
            ctrlReleaseNotes mv = new ctrlReleaseNotes();
            mv.LoadXML(xmlfilename);
            mv.Dock = DockStyle.Fill;
            dp.Width = 500;
            dp.Text = "Release notes: " + version;
            dp.Controls.Add(mv);
            dockManager1.EndUpdate();
        }

        private void btnBinaryCompare_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Tools.Instance.m_currentfile != "")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                //openFileDialog1.Filter = "Binaries|*.bin;*.ori";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    frmBinCompare bincomp = new frmBinCompare();
                    bincomp.Symbols = Tools.Instance.m_symbols;
                    bincomp.SetCurrentFilename(Tools.Instance.m_currentfile);
                    bincomp.SetCompareFilename(openFileDialog1.FileName);
                    bincomp.CompareFiles();
                    bincomp.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("No file is currently opened, you need to open a binary file first to compare it to another one!");
            }
        }

        private void btnOpenFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Filter = "Binaries|*.bin;*.ori";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                m_appSettings.Lastprojectname = "";
                OpenFile(openFileDialog1.FileName, true);
                m_appSettings.LastOpenedType = 0;
            }
        }


        private void OpenFile(string fileName, bool showMessage)
        {
            // don't allow multiple instances
            lock (this)
            {
                btnOpenFile.Enabled = false;
                btnOpenProject.Enabled = false;
                try
                {

                    Tools.Instance.m_currentfile = fileName;
                    FileInfo fi = new FileInfo(fileName);
                    Tools.Instance.m_currentfilelength = (int)fi.Length;
                    try
                    {
                        fi.IsReadOnly = false;
                        barReadOnly.Caption = "Ok";
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to remove read only flag: " + E.Message);
                        barReadOnly.Caption = "File is READ ONLY";
                    }
                    this.Text = "VAGEDCSuite [ " + Path.GetFileName(Tools.Instance.m_currentfile) + " ]";
                    Tools.Instance.m_symbols = new SymbolCollection();
                    Tools.Instance.codeBlockList = new List<CodeBlock>();
                    barFilenameText.Caption = Path.GetFileName(fileName);

                    Tools.Instance.m_symbols = DetectMaps(Tools.Instance.m_currentfile, out Tools.Instance.codeBlockList, out Tools.Instance.AxisList, showMessage, true);

                    gridControl1.DataSource = null;
                    Application.DoEvents();
                    gridControl1.DataSource = Tools.Instance.m_symbols;
                    //gridViewSymbols.BestFitColumns();
                    Application.DoEvents();
                    try
                    {
                        gridViewSymbols.ExpandAllGroups();
                    }
                    catch (Exception)
                    {

                    }
                    m_appSettings.Lastfilename = Tools.Instance.m_currentfile;
                    VerifyChecksum(fileName, !m_appSettings.AutoChecksum, false);

                    TryToLoadAdditionalSymbols(fileName, ImportFileType.XML, Tools.Instance.m_symbols, true);

                }
                catch (Exception)
                {
                }
                btnOpenFile.Enabled = true;
                btnOpenProject.Enabled = true;
            }
        }

        




        private SymbolCollection DetectMaps(string filename, out List<CodeBlock> newCodeBlocks, out List<AxisHelper> newAxisHelpers, bool showMessage, bool isPrimaryFile)
        {
            IEDCFileParser parser = Tools.Instance.GetParserForFile(filename, isPrimaryFile);
            newCodeBlocks = new List<CodeBlock>();
            newAxisHelpers = new List<AxisHelper>();
            SymbolCollection newSymbols = new SymbolCollection();
            
            if (parser != null)
            {
                byte[] allBytes = File.ReadAllBytes(filename);
                string boschnumber = parser.ExtractBoschPartnumber(allBytes);
                string softwareNumber = parser.ExtractSoftwareNumber(allBytes);
                partNumberConverter pnc = new partNumberConverter();
                ECUInfo info = pnc.ConvertPartnumber(boschnumber,allBytes.Length);
                //MessageBox.Show("Car: " + info.CarMake + "\nECU:" + info.EcuType);

                //1) Vw Hardware Number: 38906019GF, Vw System Type: 1,9l R4 EDC15P, Vw Software Number: SG  1434;
                //2) Vw Hardware Number: 38906019LJ, Vw System Type: 1,9l R4 EDC15P, Vw Software Number: SG  5934.

                if (!info.EcuType.StartsWith("EDC15P") && !info.EcuType.StartsWith("EDC15VM") && info.EcuType != string.Empty && showMessage)
                {
                    frmInfoBox infobx = new frmInfoBox("No EDC15P/VM file [" + info.EcuType + "] " + Path.GetFileName(filename));
                }
                if (info.EcuType == string.Empty)
                {
                    Console.WriteLine("partnumber " + info.PartNumber + " unknown " + filename);
                }
                if (isPrimaryFile)
                {
                    string partNo = parser.ExtractPartnumber(allBytes);
                    partNo = Tools.Instance.StripNonAscii(partNo);
                    softwareNumber = Tools.Instance.StripNonAscii(softwareNumber);
                    barPartnumber.Caption =  partNo + " " + softwareNumber;
                    barAdditionalInfo.Caption = info.PartNumber + " " + info.CarMake + " " + info.EcuType + " " + parser.ExtractInfo(allBytes);
                }

                newSymbols = parser.parseFile(filename, out newCodeBlocks, out newAxisHelpers);
                newSymbols.SortColumn = "Flash_start_address";
                newSymbols.SortingOrder = GenericComparer.SortOrder.Ascending;
                newSymbols.Sort();
                //parser.NameKnownMaps(allBytes, newSymbols, newCodeBlocks);
                //parser.FindSVBL(allBytes, filename, newSymbols, newCodeBlocks);
                /*SymbolTranslator strans = new SymbolTranslator();
                foreach (SymbolHelper sh in newSymbols)
                {
                    sh.Description = strans.TranslateSymbolToHelpText(sh.Varname);
                }*/
                // check for must have maps... if there are maps missing, report it
                if (showMessage && (parser is EDC15PFileParser || parser is EDC15P6FileParser))
                {
                    string _message = string.Empty;
                    if (MapsWithNameMissing("EGR", newSymbols)) _message += "EGR maps missing" + Environment.NewLine;
                    if (MapsWithNameMissing("SVBL", newSymbols)) _message += "SVBL missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Torque limiter", newSymbols)) _message += "Torque limiter missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Smoke limiter", newSymbols)) _message += "Smoke limiter missing" + Environment.NewLine;
                    //if (MapsWithNameMissing("IQ by MAF limiter", newSymbols)) _message += "IQ by MAF limiter missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Injector duration", newSymbols)) _message += "Injector duration maps missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Start of injection", newSymbols)) _message += "Start of injection maps missing" + Environment.NewLine;
                    if (MapsWithNameMissing("N75 duty cycle", newSymbols)) _message += "N75 duty cycle map missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Inverse driver wish", newSymbols)) _message += "Inverse driver wish map missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Boost target map", newSymbols)) _message += "Boost target map missing" + Environment.NewLine;
                    if (MapsWithNameMissing("SOI limiter", newSymbols)) _message += "SOI limiter missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Driver wish", newSymbols)) _message += "Driver wish map missing" + Environment.NewLine;
                    if (MapsWithNameMissing("Boost limit map", newSymbols)) _message += "Boost limit map missing" + Environment.NewLine;

                    if (MapsWithNameMissing("MAF correction", newSymbols)) _message += "MAF correction map missing" + Environment.NewLine;
                    if (MapsWithNameMissing("MAF linearization", newSymbols)) _message += "MAF linearization map missing" + Environment.NewLine;
                    if (MapsWithNameMissing("MAP linearization", newSymbols)) _message += "MAP linearization map missing" + Environment.NewLine;
                    if (_message != string.Empty)
                    {
                        frmInfoBox infobx = new frmInfoBox(_message);
                    }
                }
                if (isPrimaryFile)
                {
                    barSymCount.Caption = newSymbols.Count.ToString() + " symbols";

                    if (MapsWithNameMissing("Launch control map", newSymbols))
                    {
                        btnActivateLaunchControl.Enabled = true;
                    }
                    else
                    {
                        btnActivateLaunchControl.Enabled = false;
                    }
                    btnActivateSmokeLimiters.Enabled = false;
                    try
                    {
                        if (Tools.Instance.codeBlockList.Count > 0)
                        {
                            if ((GetMapCount("Smoke limiter", newSymbols) / Tools.Instance.codeBlockList.Count) == 1)
                            {
                                btnActivateSmokeLimiters.Enabled = true;
                            }
                            else
                            {
                                btnActivateSmokeLimiters.Enabled = false;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return newSymbols;

        }

        private int GetMapCount(string varName, SymbolCollection newSymbols)
        {
            int mapCount = 0;
            foreach (SymbolHelper sh in newSymbols)
            {
                if (sh.Varname.StartsWith(varName)) mapCount ++;
            }
            return mapCount;
        }

        private bool MapsWithNameMissing(string varName, SymbolCollection newSymbols)
        {
            foreach (SymbolHelper sh in newSymbols)
            {
                if(sh.Varname.StartsWith(varName)) return false;
            }
            return true;
        }


        

        
        
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            //TODO: only if mouse on datarow?
            object o = gridViewSymbols.GetFocusedRow();
            if (o is SymbolHelper)
            {
                //SymbolHelper sh = (SymbolHelper)o;
                StartTableViewer();
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column.Name == gcSymbolAddress.Name)
                {
                    if (e.CellValue != null)
                    {
                        //e.DisplayText = Convert.ToInt32(e.CellValue).ToString("X8");
                    }
                }
                else if (e.Column.Name == gcSymbolXID.Name || e.Column.Name == gcSymbolYID.Name)
                {
                }
                else if (e.Column.Name == gcSymbolLength.Name)
                {
                    if (e.CellValue != null)
                    {
                        int len = Convert.ToInt32(e.CellValue);
                        len /= 2;
                        //  e.DisplayText = len.ToString();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void StartTableViewer()
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    int row = (int)selrows.GetValue(0);
                    if (row >= 0)
                    {
                        SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                        if (sh.Flash_start_address == 0 && sh.Start_address == 0) return;
                        
                        if (sh == null) return;
                        DevExpress.XtraBars.Docking.DockPanel dockPanel;
                        bool pnlfound = false;
                        pnlfound = CheckMapViewerActive(sh);
                        if (!pnlfound)
                        {
                            dockManager1.BeginUpdate();
                            try
                            {
                                MapViewerEx tabdet = new MapViewerEx();
                                tabdet.AutoUpdateIfSRAM = false;
                                tabdet.AutoUpdateInterval = 99999;
                                tabdet.SetViewSize(ViewSize.NormalView);
                                tabdet.Visible = false;
                                tabdet.Filename = Tools.Instance.m_currentfile;
                                tabdet.GraphVisible = true;
                                tabdet.Viewtype = m_appSettings.DefaultViewType;
                                tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                                tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                                tabdet.GraphVisible = m_appSettings.ShowGraphs;
                                tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                                tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                                tabdet.Map_name = sh.Varname;
                                tabdet.Map_descr = tabdet.Map_name;
                                tabdet.Map_cat = XDFCategories.Undocumented;
                                SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                                string x_axis = string.Empty;
                                string y_axis = string.Empty;
                                string x_axis_descr = string.Empty;
                                string y_axis_descr = string.Empty;
                                string z_axis_descr = string.Empty;
                                tabdet.X_axis_name = sh.X_axis_descr;
                                tabdet.Y_axis_name = sh.Y_axis_descr;
                                tabdet.Z_axis_name = sh.Z_axis_descr;
                                tabdet.XaxisUnits = sh.XaxisUnits;
                                tabdet.YaxisUnits = sh.YaxisUnits;
                                tabdet.X_axisAddress = sh.Y_axis_address;
                                tabdet.Y_axisAddress = sh.X_axis_address;

                                tabdet.Xaxiscorrectionfactor = sh.X_axis_correction;
                                tabdet.Yaxiscorrectionfactor = sh.Y_axis_correction;
                                tabdet.Xaxiscorrectionoffset = sh.X_axis_offset;
                                tabdet.Yaxiscorrectionoffset = sh.Y_axis_offset;

                                tabdet.X_axisvalues = GetXaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);
                                tabdet.Y_axisvalues = GetYaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);


                                dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                                int dw = 650;
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 45);
                                }
                                if (dw < 400) dw = 400;
                                if (dw > 800) dw = 800;
                                dockPanel.FloatSize = new Size(dw, 900);



                                dockPanel.Tag = Tools.Instance.m_currentfile;
                                dockPanel.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(dockPanel_ClosedPanel);

                                int columns = 8;
                                int rows = 8;
                                int tablewidth = GetTableMatrixWitdhByName(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name, out columns, out rows);
                                int address = Convert.ToInt32(sh.Flash_start_address);
                                int sramaddress = 0;
                                if (address != 0)
                                {
                                    tabdet.Map_address = address;
                                    tabdet.Map_sramaddress = sramaddress;
                                    int length = Convert.ToInt32(sh.Length);
                                    tabdet.Map_length = length;
                                    byte[] mapdata = new byte[sh.Length];
                                    mapdata.Initialize();
                                    mapdata = Tools.Instance.readdatafromfile(Tools.Instance.m_currentfile, address, length, Tools.Instance.m_currentFileType);
                                    tabdet.Map_content = mapdata;
                                    tabdet.Correction_factor = sh.Correction;// GetMapCorrectionFactor(tabdet.Map_name);
                                    tabdet.Correction_offset = sh.Offset;// GetMapCorrectionOffset(tabdet.Map_name);
                                    tabdet.IsUpsideDown = m_appSettings.ShowTablesUpsideDown;
                                    tabdet.ShowTable(columns, true);
                                    tabdet.Dock = DockStyle.Fill;
                                    tabdet.onSymbolSave += new VAGSuite.MapViewerEx.NotifySaveSymbol(tabdet_onSymbolSave);
                                    tabdet.onSymbolRead += new VAGSuite.MapViewerEx.NotifyReadSymbol(tabdet_onSymbolRead);
                                    tabdet.onClose += new VAGSuite.MapViewerEx.ViewerClose(tabdet_onClose);
                                    tabdet.onAxisEditorRequested += new MapViewerEx.AxisEditorRequested(tabdet_onAxisEditorRequested);

                                    tabdet.onSliderMove +=new MapViewerEx.NotifySliderMove(tabdet_onSliderMove);
                                    tabdet.onSplitterMoved +=new MapViewerEx.SplitterMoved(tabdet_onSplitterMoved);
                                    tabdet.onSelectionChanged +=new MapViewerEx.SelectionChanged(tabdet_onSelectionChanged);
                                    tabdet.onSurfaceGraphViewChangedEx += new MapViewerEx.SurfaceGraphViewChangedEx(tabdet_onSurfaceGraphViewChangedEx);
                                    tabdet.onAxisLock +=new MapViewerEx.NotifyAxisLock(tabdet_onAxisLock);
                                    tabdet.onViewTypeChanged +=new MapViewerEx.ViewTypeChanged(tabdet_onViewTypeChanged);

                                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(Tools.Instance.m_currentfile) + "]";
                                    bool isDocked = false;

                                    if (!isDocked)
                                    {
                                        int width = 500;
                                        if (tabdet.X_axisvalues.Length > 0)
                                        {
                                            width = 30 + ((tabdet.X_axisvalues.Length + 1) * 45);
                                        }
                                        if (width < 500) width = 500;
                                        if (width > 800) width = 800;

                                        dockPanel.Width = width;
                                    }
                                    dockPanel.Controls.Add(tabdet);
                                }
                                else
                                {
                                    byte[] mapdata = new byte[sh.Length];
                                    mapdata.Initialize();

                                }
                                tabdet.Visible = true;
                            }
                            catch (Exception newdockE)
                            {
                                Console.WriteLine(newdockE.Message);
                            }
                            Console.WriteLine("End update");
                            dockManager1.EndUpdate();
                        }
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
            }

        }

        private bool CheckMapViewerActive(SymbolHelper sh)
        {
            bool retval = false;
            try
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text == "Symbol: " + sh.Varname + " [" + Path.GetFileName(Tools.Instance.m_currentfile) + "]")
                    {
                        if (pnl.Tag.ToString() == Tools.Instance.m_currentfile)
                        {
                            if (isSymbolDisplaySameAddress(sh, pnl))
                            {
                                retval = true;
                                pnl.Show();
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private bool isSymbolDisplaySameAddress(SymbolHelper sh, DockPanel pnl)
        {
            bool retval = false;
            try
            {
                if (pnl.Text.StartsWith("Symbol: "))
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is MapViewerEx)
                        {
                            MapViewerEx vwr = (MapViewerEx)c;
                            if (vwr.Map_address == sh.Flash_start_address) retval = true;
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is MapViewerEx)
                                {
                                    MapViewerEx vwr2 = (MapViewerEx)c2;
                                    if (vwr2.Map_address == sh.Flash_start_address) retval = true;

                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is MapViewerEx)
                                {
                                    MapViewerEx vwr3 = (MapViewerEx)c3;
                                    if (vwr3.Map_address == sh.Flash_start_address) retval = true;
                                }
                            }
                        }
                    }


                }
            }
            catch (Exception E)
            {
                Console.WriteLine("isSymbolDisplaySameAddress error: " + E.Message);
            }
            return retval;
        }

        void tabdet_onAxisEditorRequested(object sender, MapViewerEx.AxisEditorRequestedEventArgs e)
        {
            // start axis editor
            foreach (SymbolHelper sh in Tools.Instance.m_symbols)
            {
                if (sh.Varname == e.Mapname)
                {
                    if (e.Axisident == MapViewerEx.AxisIdent.X_Axis) StartAxisViewer(sh, Axis.XAxis);
                    else if (e.Axisident == MapViewerEx.AxisIdent.Y_Axis) StartAxisViewer(sh, Axis.YAxis);

                    break;
                }
            }
        }

        void tabdet_onClose(object sender, EventArgs e)
        {
            // close the corresponding dockpanel
            if (sender is MapViewerEx)
            {
                MapViewerEx tabdet = (MapViewerEx)sender;
                string dockpanelname = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]";
                string dockpanelname3 = "Symbol difference: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]";
                foreach (DevExpress.XtraBars.Docking.DockPanel dp in dockManager1.Panels)
                {
                    if (dp.Text == dockpanelname)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                    else if (dp.Text == dockpanelname3)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                }
            }
        }

        void tabdet_onSymbolRead(object sender, MapViewerEx.ReadSymbolEventArgs e)
        {
            if (sender is MapViewerEx)
            {
                MapViewerEx mv = (MapViewerEx)sender;
                mv.Map_content = Tools.Instance.readdatafromfile(e.Filename, (int)GetSymbolAddress(Tools.Instance.m_symbols, e.SymbolName), GetSymbolLength(Tools.Instance.m_symbols, e.SymbolName), Tools.Instance.m_currentFileType);
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(e.Filename, Tools.Instance.m_symbols, e.SymbolName, out cols, out rows);
                mv.IsRAMViewer = false;
                mv.OnlineMode = false;
                mv.ShowTable(cols, true);
                mv.IsRAMViewer = false;
                mv.OnlineMode = false;
                System.Windows.Forms.Application.DoEvents();
            }
        }

        void tabdet_onSymbolSave(object sender, MapViewerEx.SaveSymbolEventArgs e)
        {
            if (sender is MapViewerEx)
            {
                // juiste filename kiezen 
                MapViewerEx tabdet = (MapViewerEx)sender;
                string note = string.Empty;
                if (m_appSettings.RequestProjectNotes && Tools.Instance.m_CurrentWorkingProject != "")
                {
                    //request a small note from the user in which he/she can denote a description of the change
                    frmChangeNote changenote = new frmChangeNote();
                    changenote.ShowDialog();
                    note = changenote.Note;
                }

                SaveDataIncludingSyncOption(e.Filename, e.SymbolName, e.SymbolAddress, e.SymbolLength, e.SymbolDate, true, note);
                
            }
        }

        private void SaveDataIncludingSyncOption(string fileName, string varName, int address, int length, byte[] data, bool useNote, string note)
        {
            Tools.Instance.savedatatobinary(address, length, data, fileName, useNote, note, Tools.Instance.m_currentFileType);
            if (m_appSettings.CodeBlockSyncActive)
            {
                // check for other symbols with the same length and the same END address
                if (fileName == Tools.Instance.m_currentfile)
                {
                    
                    int codeBlockOffset = -1;
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        if (sh.Flash_start_address == address && sh.Length == length)
                        {
                            if (sh.CodeBlock > 0)
                            {
                                foreach (CodeBlock cb in Tools.Instance.codeBlockList)
                                {
                                    if (cb.CodeID == sh.CodeBlock)
                                    {
                                        codeBlockOffset = address - cb.StartAddress;
                                        break;
                                    }
                                }
                            }
                            break;
                            
                        }
                    }
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        bool shSaved = false;
                        if (sh.Length == length)
                        {
                            if (sh.Flash_start_address != address)
                            {
                                if ((sh.Flash_start_address & 0x0FFFF) == (address & 0x0FFFF))
                                {
                                    // 
                                    // if (MessageBox.Show("Do you want to save " + sh.Varname + " at address " + sh.Flash_start_address.ToString("X8") + " as well?", "Codeblock synchronizer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        Tools.Instance.savedatatobinary((int)sh.Flash_start_address, length, data, fileName, useNote, note, Tools.Instance.m_currentFileType);
                                        shSaved = true;
                                    }
                                }
                                // also check wether codeblock start + offset is equal
                            }
                        }
                        if (!shSaved && codeBlockOffset >= 0)
                        {
                            if (sh.Length == length)
                            {
                                if (sh.Flash_start_address != address)
                                {
                                    // determine codeblock offset for this symbol
                                    if (sh.CodeBlock > 0)
                                    {
                                        foreach (CodeBlock cb in Tools.Instance.codeBlockList)
                                        {
                                            if (cb.CodeID == sh.CodeBlock)
                                            {
                                                int thiscodeBlockOffset = (int)sh.Flash_start_address - cb.StartAddress;
                                                if (thiscodeBlockOffset == codeBlockOffset)
                                                {
                                                    // save this as well
                                                    Tools.Instance.savedatatobinary((int)sh.Flash_start_address, length, data, fileName, useNote, note, Tools.Instance.m_currentFileType);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            UpdateRollbackForwardControls();
            VerifyChecksum(fileName, false, false);
        }

        private void SaveAxisDataIncludingSyncOption(int address, int length, byte[] data, string fileName, bool useNote, string note)
        {
            Tools.Instance.savedatatobinary(address, length, data, fileName, useNote, note, Tools.Instance.m_currentFileType);
            if (m_appSettings.CodeBlockSyncActive)
            {
                // check for other symbols with the same length and the same END address
                if (fileName == Tools.Instance.m_currentfile)
                {
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        if (sh.X_axis_address != address)
                        {
                            if ((sh.X_axis_address & 0x0FFFF) == (address & 0x0FFFF))
                            {
                                if (sh.X_axis_length * 2 == length)
                                {
                                    Tools.Instance.savedatatobinary(sh.X_axis_address, length, data, fileName, useNote, note, Tools.Instance.m_currentFileType);
                                }
                            }
                        }
                        else if (sh.Y_axis_address != address)
                        {
                            if ((sh.Y_axis_address & 0x0FFFF) == (address & 0x0FFFF))
                            {
                                if (sh.Y_axis_length * 2 == length)
                                {
                                    Tools.Instance.savedatatobinary(sh.Y_axis_address, length, data, fileName, useNote, note, Tools.Instance.m_currentFileType);
                                }
                            }
                        }
                    }
                }
            }
            UpdateRollbackForwardControls();

            VerifyChecksum(Tools.Instance.m_currentfile, false, false);
        }


        

        private void VerifyChecksum(string filename, bool showQuestion, bool showInfo)
        {
            
            string chkType = string.Empty;
            barChecksum.Caption = "---";
            ChecksumResultDetails result = new ChecksumResultDetails();
            if (m_appSettings.AutoChecksum)
            {
                result = Tools.Instance.UpdateChecksum(filename, false);
                if (showInfo)
                {
                    if (result.CalculationOk)
                    {
                        if (result.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = " V4.1";
                        else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = " V4.1v2";
                        else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = " V4.1 2002";
                        else if (result.TypeResult != ChecksumType.Unknown) chkType = result.TypeResult.ToString();
                        frmInfoBox info = new frmInfoBox("Checksums are correct [" + chkType + "]");
                    }
                    else
                    {
                        if (result.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = " V4.1";
                        else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = " V4.1v2";
                        else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = " V4.1 2002";
                        else if (result.TypeResult != ChecksumType.Unknown) chkType = result.TypeResult.ToString();
                        frmInfoBox info = new frmInfoBox("Checksums are INCORRECT [" + chkType + "]");

                    }
                }
            }
            else
            {
                result = Tools.Instance.UpdateChecksum(filename, true);
                if (!result.CalculationOk)
                {
                    if (showQuestion && result.TypeResult != ChecksumType.Unknown)
                    {
                         if (result.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = " V4.1";
                         else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = " V4.1v2";
                         else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = " V4.1 2002";
                         else if (result.TypeResult != ChecksumType.Unknown) chkType = result.TypeResult.ToString();
                        frmChecksumIncorrect frmchk = new frmChecksumIncorrect();
                        frmchk.ChecksumType = chkType;
                        frmchk.NumberChecksums = result.NumberChecksumsTotal;
                        frmchk.NumberChecksumsFailed = result.NumberChecksumsFail;
                        frmchk.NumberChecksumsPassed = result.NumberChecksumsOk;
                        if(frmchk.ShowDialog() == DialogResult.OK)
                        //if (MessageBox.Show("Checksums are invalid. Do you wish to correct them?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            result = Tools.Instance.UpdateChecksum(filename, false);
                        }
                    }
                    else if (showInfo && result.TypeResult == ChecksumType.Unknown)
                    {
                        frmInfoBox info = new frmInfoBox("Checksum for this filetype is not yet implemented");
                    }
                }
                else
                {
                    if (showInfo)
                    {
                        if (result.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = " V4.1";
                        else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = " V4.1v2";
                        else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = " V4.1 2002";
                        else if (result.TypeResult != ChecksumType.Unknown) chkType = result.TypeResult.ToString();
                        frmInfoBox info = new frmInfoBox("Checksums are correct [" + chkType + "]");
                    }
                }
            }

            if (result.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = " V4.1";
            else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = " V4.1v2";
            else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = " V4.1 2002";
            if (!result.CalculationOk)
            {
                barChecksum.Caption = "Checksum failed" + chkType;
            }
            else
            {
                barChecksum.Caption = "Checksum Ok" + chkType;
            }
            Application.DoEvents();
        }

        

        

        private double GetMapCorrectionOffset(string mapname)
        {
            return 0;
        }

        private int GetTableMatrixWitdhByName(string filename, SymbolCollection curSymbols, string symbolname, out int columns, out int rows)
        {
            columns = GetSymbolWidth(curSymbols, symbolname);
            rows = GetSymbolHeight(curSymbols, symbolname);
            return columns;
        }

        private int GetSymbolWidth(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Y_axis_length;
                }
            }
            return 0;
        }

        private int GetSymbolHeight(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.X_axis_length;
                }
            }
            return 0;
        }

        private int GetSymbolLength(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Length;
                }
            }
            return 0;
        }
        private Int64 GetSymbolAddress(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }

        private double GetMapCorrectionFactor(string symbolname)
        {
            double returnvalue = 1;
            
            return returnvalue;
        }



        

        private int[] GetYaxisValues(string filename, SymbolCollection curSymbols, string symbolname)
        {
            int xlen = GetSymbolHeight(curSymbols, symbolname);
            int xaddress = GetXAxisAddress(curSymbols, symbolname);
            int[] retval = new int[xlen];
            retval.Initialize();
            if(xaddress > 0)
            {
                retval = Tools.Instance.readdatafromfileasint(filename, xaddress, xlen, Tools.Instance.m_currentFileType);
            }
            return retval;
        }

        private int GetXAxisAddress(SymbolCollection curSymbols, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.X_axis_address;
                }
            }
            return 0;
        }

        private int GetYAxisAddress(SymbolCollection curSymbols, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Y_axis_address;
                }
            }
            return 0;
        }
        private int[] GetXaxisValues(string filename, SymbolCollection curSymbols, string symbolname)
        {
            int ylen = GetSymbolWidth(curSymbols, symbolname);
            int yaddress = GetYAxisAddress(curSymbols, symbolname);
            int[] retval = new int[ylen];
            retval.Initialize();
            if (yaddress > 0)
            {
                retval = Tools.Instance.readdatafromfileasint(filename, yaddress, ylen, Tools.Instance.m_currentFileType);
            }
            return retval;

        }

        void dockPanel_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            if (sender is DockPanel)
            {
                DockPanel pnl = (DockPanel)sender;

                foreach (Control c in pnl.Controls)
                {
                    if (c is HexViewer)
                    {
                        HexViewer vwr = (HexViewer)c;
                        vwr.CloseFile();
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is HexViewer)
                            {
                                HexViewer vwr2 = (HexViewer)c2;
                                vwr2.CloseFile();
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                    {
                        DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                        foreach (Control c3 in cntr.Controls)
                        {
                            if (c3 is HexViewer)
                            {
                                HexViewer vwr3 = (HexViewer)c3;
                                vwr3.CloseFile();
                            }
                        }
                    }
                }
                dockManager1.RemovePanel(pnl);
            }
        }

        private void btnCompareFiles_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Filter = "Binaries|*.bin;*.ori";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string compareFile = openFileDialog1.FileName;
                CompareToFile(compareFile);

            }
        }

        private void CompareToFile(string filename)
        {
            if (Tools.Instance.m_symbols.Count > 0)
            {
                dockManager1.BeginUpdate();
                try
                {
                    DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                    CompareResults tabdet = new CompareResults();
                    tabdet.ShowAddressesInHex = true;
                    tabdet.SetFilterMode(true);
                    tabdet.Dock = DockStyle.Fill;
                    tabdet.Filename = filename;
                    tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelect);
                    dockPanel.Controls.Add(tabdet);
                    dockPanel.Text = "Compare results: " + Path.GetFileName(filename);
                    dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);
                    dockPanel.Width = 500;
                    SymbolCollection compare_symbols = new SymbolCollection();
                    List<CodeBlock> compare_blocks = new List<CodeBlock>();
                    List<AxisHelper> compare_axis = new List<AxisHelper>();
                    compare_symbols = DetectMaps(filename, out compare_blocks, out compare_axis, false, false);
                    System.Windows.Forms.Application.DoEvents();

                    Console.WriteLine("ori : " + Tools.Instance.m_symbols.Count.ToString());
                    Console.WriteLine("comp : " + compare_symbols.Count.ToString());

                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                    dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                    dt.Columns.Add("CATEGORY", Type.GetType("System.Int32")); //0
                    dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
                    dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
                    dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
                    dt.Columns.Add("CATEGORYNAME");
                    dt.Columns.Add("SUBCATEGORYNAME");
                    dt.Columns.Add("SymbolNumber1", Type.GetType("System.Int32"));
                    dt.Columns.Add("SymbolNumber2", Type.GetType("System.Int32"));
                    dt.Columns.Add("Userdescription");
                    dt.Columns.Add("MissingInOriFile", Type.GetType("System.Boolean"));
                    dt.Columns.Add("MissingInCompareFile", Type.GetType("System.Boolean"));
                    dt.Columns.Add("CodeBlock1", Type.GetType("System.Int32"));
                    dt.Columns.Add("CodeBlock2", Type.GetType("System.Int32"));
                    string category = "";
                    string ht = string.Empty;
                    double diffperc = 0;
                    int diffabs = 0;
                    double diffavg = 0;
                    int percentageDone = 0;
                    int symNumber = 0;
                    XDFCategories cat = XDFCategories.Undocumented;
                    XDFSubCategory subcat = XDFSubCategory.Undocumented;
                    if (compare_symbols.Count > 0)
                    {
                        CompareResults cr = new CompareResults();
                        cr.ShowAddressesInHex = true;
                        cr.SetFilterMode(true);
                        foreach (SymbolHelper sh_compare in compare_symbols)
                        {
                            foreach (SymbolHelper sh_org in Tools.Instance.m_symbols)
                            {
                                if ((sh_compare.Flash_start_address == sh_org.Flash_start_address) || (sh_compare.Varname == sh_org.Varname))
                                {
                                    // compare
                                    if (!CompareSymbolToCurrentFile(sh_compare.Varname, (int)sh_compare.Flash_start_address, sh_compare.Length, filename, out diffperc, out diffabs, out diffavg, sh_compare.Correction))
                                    {
                                        dt.Rows.Add(sh_compare.Varname, sh_compare.Start_address, sh_compare.Flash_start_address, sh_compare.Length, sh_compare.Length, sh_compare.Varname, false, 0, diffperc, diffabs, diffavg, category, "", sh_org.Symbol_number, sh_compare.Symbol_number, "", false, false, sh_org.CodeBlock, sh_compare.CodeBlock);

                                    }
                                }
                            }
                        }
                        
                        tabdet.CompareSymbolCollection = compare_symbols;
                        tabdet.OriginalSymbolCollection = Tools.Instance.m_symbols;
                        tabdet.OriginalFilename = Tools.Instance.m_currentfile;
                        tabdet.CompareFilename = filename;
                        tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
                        tabdet.gridControl1.DataSource = dt.Copy();
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }
        private bool SymbolExists(string symbolname)
        {
            foreach (SymbolHelper sh in Tools.Instance.m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname) return true;
            }
            return false;
        }

        private void StartCompareMapViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength, SymbolCollection curSymbols, int symbolnumber)
        {
            try
            {
                SymbolHelper sh = FindSymbol(curSymbols, SymbolName);
                
                DevExpress.XtraBars.Docking.DockPanel dockPanel;
                bool pnlfound = false;
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {

                    if (pnl.Text == "Symbol: " + SymbolName + " [" + Path.GetFileName(Filename) + "]")
                    {
                        if (pnl.Tag.ToString() == Filename) // <GS-10052011>
                        {
                            dockPanel = pnl;
                            pnlfound = true;
                            dockPanel.Show();
                        }
                    }
                }
                if (!pnlfound)
                {
                    dockManager1.BeginUpdate();
                    try
                    {
                        dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        dockPanel.Tag = Filename;// Tools.Instance.m_currentfile; changed 24/01/2008
                        MapViewerEx tabdet = new MapViewerEx();

                        tabdet.AutoUpdateIfSRAM = false;// m_appSettings.AutoUpdateSRAMViewers;
                        tabdet.AutoUpdateInterval = 99999;
                        tabdet.SetViewSize(ViewSize.NormalView);

                        //tabdet.IsHexMode = barViewInHex.Checked;
                        tabdet.Viewtype = m_appSettings.DefaultViewType;
                        tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                        tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                        tabdet.GraphVisible = m_appSettings.ShowGraphs;
                        tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                        tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                        tabdet.Filename = Filename;
                        tabdet.Map_name = SymbolName;
                        tabdet.Map_descr = tabdet.Map_name;
                        tabdet.Map_cat = XDFCategories.Undocumented;
                        tabdet.X_axisvalues = GetXaxisValues(Filename, curSymbols, tabdet.Map_name);
                        tabdet.Y_axisvalues = GetYaxisValues(Filename, curSymbols, tabdet.Map_name);

                        SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                        string x_axis = string.Empty;
                        string y_axis = string.Empty;
                        string x_axis_descr = string.Empty;
                        string y_axis_descr = string.Empty;
                        string z_axis_descr = string.Empty;

                        tabdet.X_axis_name = sh.X_axis_descr;
                        tabdet.Y_axis_name = sh.Y_axis_descr;
                        tabdet.Z_axis_name = sh.Z_axis_descr;
                        tabdet.XaxisUnits = sh.XaxisUnits;
                        tabdet.YaxisUnits = sh.YaxisUnits;
                        tabdet.X_axisAddress = sh.Y_axis_address;
                        tabdet.Y_axisAddress = sh.X_axis_address;

                        tabdet.Xaxiscorrectionfactor = sh.X_axis_correction;
                        tabdet.Yaxiscorrectionfactor = sh.Y_axis_correction;

                        //tabdet.X_axisvalues = GetXaxisValues(Tools.Instance.m_currentfile, curSymbols, tabdet.Map_name);
                        //tabdet.Y_axisvalues = GetYaxisValues(Tools.Instance.m_currentfile, curSymbols, tabdet.Map_name);

                        int columns = 8;
                        int rows = 8;
                        int tablewidth = GetTableMatrixWitdhByName(Filename, curSymbols, tabdet.Map_name, out columns, out rows);
                        int address = Convert.ToInt32(SymbolAddress);
                        if (address != 0)
                        {
                            tabdet.Map_address = address;
                            int length = SymbolLength;
                            tabdet.Map_length = length;
                            byte[] mapdata = Tools.Instance.readdatafromfile(Filename, address, length, Tools.Instance.m_currentFileType);
                            tabdet.Map_content = mapdata;
                            tabdet.Correction_factor = sh.Correction;
                            tabdet.Correction_offset = sh.Offset;// GetMapCorrectionOffset(tabdet.Map_name);
                            tabdet.IsUpsideDown = m_appSettings.ShowTablesUpsideDown;
                            tabdet.ShowTable(columns, true);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onSymbolSave += new VAGSuite.MapViewerEx.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onSymbolRead += new VAGSuite.MapViewerEx.NotifyReadSymbol(tabdet_onSymbolRead);
                            tabdet.onClose += new VAGSuite.MapViewerEx.ViewerClose(tabdet_onClose);

                            tabdet.onSliderMove += new MapViewerEx.NotifySliderMove(tabdet_onSliderMove);
                            tabdet.onSplitterMoved += new MapViewerEx.SplitterMoved(tabdet_onSplitterMoved);
                            tabdet.onSelectionChanged += new MapViewerEx.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new MapViewerEx.SurfaceGraphViewChangedEx(tabdet_onSurfaceGraphViewChangedEx);
                            tabdet.onAxisLock += new MapViewerEx.NotifyAxisLock(tabdet_onAxisLock);
                            tabdet.onViewTypeChanged += new MapViewerEx.ViewTypeChanged(tabdet_onViewTypeChanged);


                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "Symbol: " + SymbolName + " [" + Path.GetFileName(Filename) + "]";
                            dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 1);
                            bool isDocked = false;
                            // Try to dock to same symbol
                            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                            {
                                if (pnl.Text.StartsWith("Symbol: " + SymbolName) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                {
                                    dockPanel.DockAsTab(pnl, 0);
                                    isDocked = true;
                                    break;
                                }
                            }
                            if (!isDocked)
                            {
                                int width = 500;
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    width = 30 + ((tabdet.X_axisvalues.Length + 1) * 45);
                                }
                                if (width < 500) width = 500;
                                if (width > 800) width = 800;

                                dockPanel.Width = width;
                            }
                            dockPanel.Controls.Add(tabdet);
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    dockManager1.EndUpdate();
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception startnewcompareE)
            {
                Console.WriteLine(startnewcompareE.Message);
            }

        }

        private SymbolHelper FindSymbol(SymbolCollection curSymbols, string SymbolName)
        {
            foreach (SymbolHelper sh in curSymbols)
            {
                if (sh.Varname == SymbolName || sh.Userdescription == SymbolName) return sh;
            }
            return new SymbolHelper();
        }

        void tabdet_onSymbolSelect(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            
            if (!e.ShowDiffMap)
            {
                DumpDockWindows();
                if (SymbolExists(e.SymbolName))
                {
                    StartTableViewer(e.SymbolName, e.CodeBlock1);
                }
                //DumpDockWindows();
                foreach (SymbolHelper sh in e.Symbols)
                {
                    if (sh.Varname == e.SymbolName || sh.Userdescription == e.SymbolName)
                    {
                        string symName = e.SymbolName;
                        if ((e.SymbolName.StartsWith("2D") || e.SymbolName.StartsWith("3D"))  && sh.Userdescription != string.Empty) symName = sh.Userdescription;
                        StartCompareMapViewer(symName, e.Filename, e.SymbolAddress, e.SymbolLength, e.Symbols, e.Symbolnumber2);
                        break;
                    }
                }
                DumpDockWindows();
            }
            else
            {
                // show difference map
                foreach (SymbolHelper sh in e.Symbols)
                {
                    if (sh.Varname == e.SymbolName || sh.Userdescription == e.SymbolName)
                    {
                        StartCompareDifferenceViewer(sh, e.Filename, e.SymbolAddress);
                        break;
                    }
                }
                
            }
        }

        private void StartCompareDifferenceViewer(SymbolHelper sh, string Filename, int SymbolAddress)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {

                if (pnl.Text == "Symbol difference: " + sh.Varname + " [" + Path.GetFileName(Tools.Instance.m_currentfile) + "]")
                {
                    dockPanel = pnl;
                    pnlfound = true;
                    dockPanel.Show();
                }
            }
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                    dockPanel.Tag = Tools.Instance.m_currentfile;
                    MapViewerEx tabdet = new MapViewerEx();
                    tabdet.Map_name = sh.Varname;
                    tabdet.IsDifferenceViewer = true;
                    tabdet.AutoUpdateIfSRAM = false;
                    tabdet.AutoUpdateInterval = 999999;
                    tabdet.Viewtype = m_appSettings.DefaultViewType;
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Filename = Filename;
                    tabdet.Map_descr = tabdet.Map_name;
                    tabdet.Map_cat = XDFCategories.Undocumented;
                    tabdet.X_axisvalues = GetXaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);
                    tabdet.Y_axisvalues = GetYaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);

                    SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                    string x_axis = string.Empty;
                    string y_axis = string.Empty;
                    string x_axis_descr = string.Empty;
                    string y_axis_descr = string.Empty;
                    string z_axis_descr = string.Empty;

                    tabdet.X_axis_name = sh.X_axis_descr;
                    tabdet.Y_axis_name = sh.Y_axis_descr;
                    tabdet.Z_axis_name = sh.Z_axis_descr;
                    tabdet.XaxisUnits = sh.XaxisUnits;
                    tabdet.YaxisUnits = sh.YaxisUnits;
                    tabdet.X_axisAddress = sh.Y_axis_address;
                    tabdet.Y_axisAddress = sh.X_axis_address;

                    tabdet.Xaxiscorrectionfactor = sh.X_axis_correction;
                    tabdet.Yaxiscorrectionfactor = sh.Y_axis_correction;

                    tabdet.X_axisvalues = GetXaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);
                    tabdet.Y_axisvalues = GetYaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);
                    

                    //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name, out columns, out rows);
                    int address = Convert.ToInt32(SymbolAddress);
                    if (address != 0)
                    {
                        tabdet.Map_address = address;
                        int length = sh.Length;
                        tabdet.Map_length = length;
                        byte[] mapdata = Tools.Instance.readdatafromfile(Filename, address, length, Tools.Instance.m_currentFileType);
                        byte[] mapdataorig = Tools.Instance.readdatafromfile(Filename, address, length, Tools.Instance.m_currentFileType);
                        byte[] mapdata2 = Tools.Instance.readdatafromfile(Tools.Instance.m_currentfile, (int)GetSymbolAddress(Tools.Instance.m_symbols, sh.Varname), GetSymbolLength(Tools.Instance.m_symbols, sh.Varname), Tools.Instance.m_currentFileType);

                        tabdet.Map_original_content = mapdataorig;
                        tabdet.Map_compare_content = mapdata2;

                        if (mapdata.Length == mapdata2.Length)
                        {

                            for (int bt = 0; bt < mapdata2.Length; bt += 2)
                            {
                                int value1 = Convert.ToInt16(mapdata.GetValue(bt)) * 256 + Convert.ToInt16(mapdata.GetValue(bt + 1));
                                int value2 = Convert.ToInt16(mapdata2.GetValue(bt)) * 256 + Convert.ToInt16(mapdata2.GetValue(bt + 1));
                                value1 = Math.Abs((int)value1 - (int)value2);
                                byte v1 = (byte)(value1 / 256);
                                byte v2 = (byte)(value1 - (int)v1 * 256);
                                mapdata.SetValue(v1, bt);
                                mapdata.SetValue(v2, bt + 1);
                            }


                            tabdet.Map_content = mapdata;
                            tabdet.UseNewCompare = true;
                            tabdet.Correction_factor = sh.Correction;
                            tabdet.Correction_offset = sh.Offset;
                            tabdet.IsUpsideDown = m_appSettings.ShowTablesUpsideDown;
                            tabdet.ShowTable(columns, true);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onClose += new MapViewerEx.ViewerClose(tabdet_onClose);
                            dockPanel.Text = "Symbol difference: " + sh.Varname + " [" + Path.GetFileName(Filename) + "]";
                            bool isDocked = false;

                            if (!isDocked)
                            {
                                dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                                if (m_appSettings.AutoSizeNewWindows)
                                {
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 45);
                                    }
                                    else
                                    {
                                        //dockPanel.Width = this.Width - dockSymbols.Width - 10;

                                    }
                                }
                                if (dockPanel.Width < 400) dockPanel.Width = 400;

                                //                    dockPanel.Width = 400;
                            }
                            dockPanel.Controls.Add(tabdet);

                        }
                        else
                        {
                            frmInfoBox info = new frmInfoBox("Map lengths don't match...");
                        }
                    }
                }
                catch (Exception E)
                {

                    Console.WriteLine(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void DumpDockWindows()
        {
            foreach(DockPanel dp in dockManager1.Panels)
            {
                Console.WriteLine(dp.Text);

            }
        }

        
        private void StartTableViewer(string symbolname, int codeblock)
        {
            int rtel = 0;
            bool _vwrstarted = false;
            try
            {
                if (Tools.Instance.GetSymbolAddressLike(Tools.Instance.m_symbols, symbolname) > 0)
                {
                    //Console.WriteLine("Option one");
                    gridViewSymbols.ActiveFilter.Clear(); // clear filter
                    gridViewSymbols.ApplyFindFilter("");

                    SymbolCollection sc = (SymbolCollection)gridControl1.DataSource;
                    rtel = 0;
                    foreach (SymbolHelper sh in sc)
                    {
                        if (sh.Varname.StartsWith(symbolname) && sh.CodeBlock == codeblock)
                        {
                            try
                            {
                                int rhandle = gridViewSymbols.GetRowHandle(rtel);
                                gridViewSymbols.OptionsSelection.MultiSelect = true;
                                gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                                gridViewSymbols.ClearSelection();
                                gridViewSymbols.SelectRow(rhandle);
                                //gridViewSymbols.SelectRows(rhandle, rhandle);
                                gridViewSymbols.MakeRowVisible(rhandle, true);
                                gridViewSymbols.FocusedRowHandle = rhandle;
                                //gridViewSymbols.SelectRange(rhandle, rhandle);
                                _vwrstarted = true;
                                StartTableViewer();
                                break;
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show(E.Message);
                            }
                        }

                        rtel++;
                    }
                    if (!_vwrstarted)
                    {
                        rtel = 0;
                        foreach (SymbolHelper sh in sc)
                        {
                            if (sh.Userdescription.StartsWith(symbolname) && sh.CodeBlock == codeblock)
                            {
                                try
                                {
                                    int rhandle = gridViewSymbols.GetRowHandle(rtel);
                                    gridViewSymbols.OptionsSelection.MultiSelect = true;
                                    gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                                    gridViewSymbols.ClearSelection();
                                    gridViewSymbols.SelectRow(rhandle);
                                    //gridViewSymbols.SelectRows(rhandle, rhandle);
                                    gridViewSymbols.MakeRowVisible(rhandle, true);
                                    gridViewSymbols.FocusedRowHandle = rhandle;
                                    //gridViewSymbols.SelectRange(rhandle, rhandle);
                                    _vwrstarted = true;
                                    StartTableViewer();
                                    break;
                                }
                                catch (Exception E)
                                {
                                    MessageBox.Show(E.Message);
                                }
                            }
                            rtel++;
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("Option two");
                    gridViewSymbols.ActiveFilter.Clear(); // clear filter
                    SymbolCollection sc = (SymbolCollection)gridControl1.DataSource;

                    rtel = 0;
                    foreach (SymbolHelper sh in sc)
                    {
                        if (sh.Userdescription.StartsWith(symbolname) && sh.CodeBlock == codeblock)
                        {
                            try
                            {
                                int rhandle = gridViewSymbols.GetRowHandle(rtel);
                                gridViewSymbols.OptionsSelection.MultiSelect = true;
                                gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                                gridViewSymbols.ClearSelection();
                                gridViewSymbols.SelectRow(rhandle);
                                //gridViewSymbols.SelectRows(rhandle, rhandle);
                                gridViewSymbols.MakeRowVisible(rhandle, true);
                                gridViewSymbols.FocusedRowHandle = rhandle;
                                //gridViewSymbols.SelectRange(rhandle, rhandle);
                                _vwrstarted = true;
                                StartTableViewer();
                                break;
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show(E.Message);
                            }
                        }
                        rtel++;
                    }
                }
            }
            catch (Exception)
            {
                frmInfoBox info = new frmInfoBox("There seems to be a problem opening this map, do you have a file opened?");
            }
        }


        private bool CompareSymbolToCurrentFile(string symbolname, int address, int length, string filename, out double diffperc, out int diffabs, out double diffavg, double correction)
        {
            diffperc = 0;
            diffabs = 0;
            diffavg = 0;

            double totalvalue1 = 0;
            double totalvalue2 = 0;
            bool retval = true;

            if (address > 0)
            {
                int curaddress = (int)GetSymbolAddress(Tools.Instance.m_symbols, symbolname);
                int curlength = GetSymbolLength(Tools.Instance.m_symbols, symbolname);
                byte[] curdata = Tools.Instance.readdatafromfile(Tools.Instance.m_currentfile, curaddress, curlength, Tools.Instance.m_currentFileType);
                byte[] compdata = Tools.Instance.readdatafromfile(filename, address, length, Tools.Instance.m_currentFileType);
                if (curdata.Length != compdata.Length)
                {
                    Console.WriteLine("Lengths didn't match: " + symbolname);
                    return false;
                }
                for (int offset = 0; offset < curdata.Length; offset += 2)
                {
                    int ival1 = Convert.ToInt32(curdata.GetValue(offset)) * 256 + Convert.ToInt32(curdata.GetValue(offset + 1));
                    int ival2 = Convert.ToInt32(compdata.GetValue(offset)) * 256 + Convert.ToInt32(compdata.GetValue(offset + 1)) ;
                    if (ival1 != ival2)
                    {
                        retval = false;
                        diffabs++;
                    }
                    totalvalue1 += Convert.ToDouble(ival1);
                    totalvalue2 += Convert.ToDouble(ival2);
                }
                if (curdata.Length > 0)
                {
                    totalvalue1 /= (curdata.Length/2);
                    totalvalue2 /= (compdata.Length/2);
                }
            }
            diffavg = Math.Abs(totalvalue1 - totalvalue2) * correction;
            diffperc = (diffabs * 100) / (length /2);
            return retval;
        }

        private void btnTestFiles_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBrowseFiles browse = new frmBrowseFiles();
            browse.Show();
                
            /*EDC15P_EEPROM eeprom = new EDC15P_EEPROM();
            eeprom.LoadFile(@"D:\Prive\Audi\TDI\spare eeprom\spare eeprom.bin");
            Console.WriteLine("key: " + eeprom.Key.ToString());
            Console.WriteLine("odo: " + eeprom.Mileage.ToString());
            Console.WriteLine("vin: " + eeprom.Vin);
            Console.WriteLine("immo: " + eeprom.Immo);
            Console.WriteLine("immoact: " + eeprom.ImmoActive.ToString());*/

            /*
            EDC15PTuner tuner = new EDC15PTuner();
            tuner.TuneEDC15PFile(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, 400, 170);*/

            //D:\Prive\ECU\audi\BinCollection
            //
            //ParseDirectory(@"D:\Prive\Audi");
            //ParseDirectory(@"D:\Prive\ECU\audi");
            

            /*
            if (Directory.Exists(@"D:\Prive\Audi\TDI"))
            {
                string[] files = Directory.GetFiles(@"D:\Prive\Audi\TDI", "*.bin", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    OpenFile(file);
                    bool found = false;
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        if (sh.Varname.StartsWith("SVBL Boost limiter"))
                        {
                            Console.WriteLine("SVBL @" + sh.Flash_start_address.ToString("X8") + " in " + file);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        Console.WriteLine("No SVBL found in " + file);
                    }
                }
            }*/
        }

        private void ParseDirectory(string folder)
        {
            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    IEDCFileParser parser = Tools.Instance.GetParserForFile(file, false);

                   
                    
                        OpenFile(file, false);
                        byte[] allBytes = File.ReadAllBytes(file);
                        string boschnumber = parser.ExtractBoschPartnumber(allBytes);
                        string partnumber = parser.ExtractPartnumber(allBytes);
                        string softwareNumber = parser.ExtractSoftwareNumber(allBytes);
                        partNumberConverter pnc = new partNumberConverter();
                        ECUInfo info = pnc.ConvertPartnumber(boschnumber, allBytes.Length);
                        UInt32 chks = AddChecksum(allBytes);
                        // determine peak trq&hp
                        if (info.EcuType.StartsWith("EDC15P"))
                        {
                            // export to the final folder
                            string destFile = Path.Combine(@"D:\Prive\ECU\audi\BinCollection\output", /*info.CarMake + "_" + info.EcuType + "_" +*/ boschnumber + "_" + softwareNumber + "_" + partnumber + "_" + chks.ToString("X8") + ".bin");
                            if (File.Exists(destFile)) Console.WriteLine("Double file: " + destFile);
                            else
                            {
                                File.Copy(file, destFile, false);
                            }
                        }
                    
                }
            }
            Console.WriteLine("Done");
        }

        private UInt32 AddChecksum(byte[] allBytes)
        {
            UInt32 retval = 0;
            foreach (byte b in allBytes)
            {
                retval += b;
            }
            return retval;
        }

        private void btnAppSettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmSettings set = new frmSettings();
            set.AppSettings = m_appSettings;
            set.UseCodeBlockSynchroniser = m_appSettings.CodeBlockSyncActive;
            set.ShowTablesUpsideDown = m_appSettings.ShowTablesUpsideDown;
            set.AutoSizeNewWindows = m_appSettings.AutoSizeNewWindows;
            set.AutoSizeColumnsInViewer = m_appSettings.AutoSizeColumnsInWindows;
            set.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            set.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
            set.ShowGraphsInMapViewer = m_appSettings.ShowGraphs;
            set.UseRedAndWhiteMaps = m_appSettings.ShowRedWhite;
            set.ViewTablesInHex = m_appSettings.Viewinhex;
            set.AutoDockSameFile = m_appSettings.AutoDockSameFile;
            set.AutoDockSameSymbol = m_appSettings.AutoDockSameSymbol;
            set.DisableMapviewerColors = m_appSettings.DisableMapviewerColors;
            set.NewPanelsFloating = m_appSettings.NewPanelsFloating;
            set.AutoLoadLastFile = m_appSettings.AutoLoadLastFile;
            set.DefaultViewType = m_appSettings.DefaultViewType;
            set.DefaultViewSize = m_appSettings.DefaultViewSize;
            set.SynchronizeMapviewers = m_appSettings.SynchronizeMapviewers;
            set.SynchronizeMapviewersDifferentMaps = m_appSettings.SynchronizeMapviewersDifferentMaps;

            set.ProjectFolder = m_appSettings.ProjectFolder;
            set.RequestProjectNotes = m_appSettings.RequestProjectNotes;


            if (set.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.ShowTablesUpsideDown = set.ShowTablesUpsideDown;
                m_appSettings.CodeBlockSyncActive = set.UseCodeBlockSynchroniser;
                m_appSettings.AutoSizeNewWindows = set.AutoSizeNewWindows;
                m_appSettings.AutoSizeColumnsInWindows = set.AutoSizeColumnsInViewer;
                m_appSettings.AutoChecksum = set.AutoUpdateChecksum;
                m_appSettings.ShowAddressesInHex = set.ShowAddressesInHex;
                m_appSettings.ShowGraphs = set.ShowGraphsInMapViewer;
                m_appSettings.ShowRedWhite = set.UseRedAndWhiteMaps;
                m_appSettings.Viewinhex = set.ViewTablesInHex;
                m_appSettings.DisableMapviewerColors = set.DisableMapviewerColors;
                m_appSettings.AutoDockSameFile = set.AutoDockSameFile;
                m_appSettings.AutoDockSameSymbol = set.AutoDockSameSymbol;
                m_appSettings.NewPanelsFloating = set.NewPanelsFloating;
                m_appSettings.DefaultViewType = set.DefaultViewType;
                m_appSettings.DefaultViewSize = set.DefaultViewSize;
                m_appSettings.AutoLoadLastFile = set.AutoLoadLastFile;
                m_appSettings.SynchronizeMapviewers = set.SynchronizeMapviewers;
                m_appSettings.SynchronizeMapviewersDifferentMaps = set.SynchronizeMapviewersDifferentMaps;

                m_appSettings.ProjectFolder = set.ProjectFolder;
                m_appSettings.RequestProjectNotes = set.RequestProjectNotes;

            }
            SetFilterMode();
        }

        private void SetFilterMode()
        {
            if (m_appSettings.ShowAddressesInHex)
            {
                gcSymbolAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolAddress.DisplayFormat.FormatString = "X6";
                gcSymbolAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolLength.DisplayFormat.FormatString = "X6";
                gcSymbolLength.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolXID.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolXID.DisplayFormat.FormatString = "X4";
                gcSymbolXID.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolYID.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolYID.DisplayFormat.FormatString = "X4";
                gcSymbolYID.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
            else
            {
                gcSymbolAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolAddress.DisplayFormat.FormatString = "";
                gcSymbolAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolLength.DisplayFormat.FormatString = "";
                gcSymbolLength.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolXID.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolXID.DisplayFormat.FormatString = "";
                gcSymbolXID.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolYID.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolYID.DisplayFormat.FormatString = "";
                gcSymbolYID.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
            }
        }

        void InitSkins()
        {
            ribbonControl1.ForceInitialize();
            //barManager1.ForceInitialize();
            BarButtonItem item;

            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.BonusSkins).Assembly);
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.OfficeSkins).Assembly);

            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
            {
                item = new BarButtonItem();
                item.Caption = cnt.SkinName;
                //iPaintStyle.AddItem(item);
                rbnPageGroupSkins.ItemLinks.Add(item);
                item.ItemClick += new ItemClickEventHandler(OnSkinClick);
            }

            try
            {
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(m_appSettings.Skinname);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            SetToolstripTheme();
        }

        private void SetToolstripTheme()
        {
            //Console.WriteLine("Rendermode was: " + ToolStripManager.RenderMode.ToString());
            //Console.WriteLine("Visual styles: " + ToolStripManager.VisualStylesEnabled.ToString());
            //Console.WriteLine("Skinname: " + appSettings.SkinName);
            //Console.WriteLine("Backcolor: " + defaultLookAndFeel1.LookAndFeel.Painter.Button.DefaultAppearance.BackColor.ToString());
            //Console.WriteLine("Backcolor2: " + defaultLookAndFeel1.LookAndFeel.Painter.Button.DefaultAppearance.BackColor2.ToString());
            try
            {
                Skin currentSkin = CommonSkins.GetSkin(defaultLookAndFeel1.LookAndFeel);
                Color c = currentSkin.TranslateColor(SystemColors.Control);
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
                ProfColorTable profcolortable = new ProfColorTable();
                profcolortable.CustomToolstripGradientBegin = c;
                profcolortable.CustomToolstripGradientMiddle = c;
                profcolortable.CustomToolstripGradientEnd = c;
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer(profcolortable);
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// OnSkinClick: Als er een skin gekozen wordt door de gebruiker voer deze
        /// dan door in de user interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSkinClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string skinName = e.Item.Caption;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skinName);
            m_appSettings.Skinname = skinName;
            SetToolstripTheme();
        }

        private string GetAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        private void LoadLayoutFiles()
        {
            try
            {
                if (File.Exists(Path.Combine(GetAppDataPath(), "SymbolViewLayout.xml")))
                {
                    gridViewSymbols.RestoreLayoutFromXml(Path.Combine(GetAppDataPath(), "SymbolViewLayout.xml"));
                }
                if(m_appSettings.SymbolDockWidth > 2)
                {
                    dockSymbols.Width = m_appSettings.SymbolDockWidth;
                }
            }
            catch (Exception E1)
            {
                Console.WriteLine(E1.Message);
            }
        }

        private void SaveLayoutFiles()
        {
            try
            {
                m_appSettings.SymbolDockWidth = dockSymbols.Width;
                gridViewSymbols.SaveLayoutToXml(Path.Combine(GetAppDataPath(), "SymbolViewLayout.xml"));

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                m_appSettings = new AppSettings();
            }
            catch (Exception)
            {

            }
            InitSkins();
            LoadLayoutFiles();
            
            if (m_appSettings.DebugMode)
            {
                btnTestFiles.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                btnTestFiles.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        private void btnCheckForUpdates_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (m_msiUpdater != null)
                {
                    m_msiUpdater.CheckForUpdates("Global", "http://trionic.mobixs.eu/vagedcsuite/", "", "", false);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            try
            {
                if (splash != null)
                    splash.Hide();
            }
            catch (Exception)
            {

            }
            try
            {
                if (m_appSettings.AutoLoadLastFile)
                {
                    if (m_appSettings.LastOpenedType == 0)
                    {
                        if (m_appSettings.Lastfilename != "")
                        {
                            if (File.Exists(m_appSettings.Lastfilename))
                            {
                                OpenFile(m_appSettings.Lastfilename, false);
                            }
                        }
                    }
                    else if (m_appSettings.Lastprojectname != "")
                    {
                        OpenProject(m_appSettings.Lastprojectname);
                    }
                }
                SetFilterMode();
            }
            catch (Exception)
            {

            }

            try
            {
                m_msiUpdater = new msiupdater(new Version(System.Windows.Forms.Application.ProductVersion));
                m_msiUpdater.Apppath = System.Windows.Forms.Application.UserAppDataPath;
                m_msiUpdater.onDataPump += new msiupdater.DataPump(m_msiUpdater_onDataPump);
                m_msiUpdater.onUpdateProgressChanged += new msiupdater.UpdateProgressChanged(m_msiUpdater_onUpdateProgressChanged);
                m_msiUpdater.CheckForUpdates("Global", "http://trionic.mobixs.eu/vagedcsuite/", "", "", false);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        void m_msiUpdater_onUpdateProgressChanged(msiupdater.MSIUpdateProgressEventArgs e)
        {

        }


        private void SetStatusText(string text)
        {
            barUpdateText.Caption = text;
            System.Windows.Forms.Application.DoEvents();
        }

        void m_msiUpdater_onDataPump(msiupdater.MSIUpdaterEventArgs e)
        {
            SetStatusText(e.Data);
            if (e.UpdateAvailable)
            {

                if (e.XMLFile != "" && e.Version.ToString() != "0.0")
                {
                    if (!this.IsDisposed)
                    {
                        try
                        {
                            this.Invoke(m_DelegateStartReleaseNotePanel, e.XMLFile, e.Version.ToString());
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }

                //this.Invoke(m_DelegateShowChangeLog, e.Version);
                frmUpdateAvailable frmUpdate = new frmUpdateAvailable();
                frmUpdate.SetVersionNumber(e.Version.ToString());
                if (m_msiUpdater != null)
                {
                    m_msiUpdater.Blockauto_updates = false;
                }
                if (frmUpdate.ShowDialog() == DialogResult.OK)
                {
                    if (m_msiUpdater != null)
                    {
                        m_msiUpdater.ExecuteUpdate(e.Version);
                        System.Windows.Forms.Application.Exit();
                    }
                }
                else
                {
                    // user chose "NO", don't bug him again!
                    if (m_msiUpdater != null)
                    {
                        m_msiUpdater.Blockauto_updates = false;
                    }
                }
            }
        }

        private void btnReleaseNotes_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartReleaseNotesViewer(m_msiUpdater.GetReleaseNotes(), System.Windows.Forms.Application.ProductVersion.ToString());
        }

        private void btnAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmAbout about = new frmAbout();
            about.SetInformation("VAGEDCSuite v" + System.Windows.Forms.Application.ProductVersion.ToString());
            about.ShowDialog();
        }

        private void btnViewFileInHex_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartHexViewer();
        }

        private void StartHexViewer()
        {
            if (Tools.Instance.m_currentfile != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DevExpress.XtraBars.Docking.DockPanel dockPanel;
                    //= dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    if (!m_appSettings.NewPanelsFloating)
                    {
                        dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    }
                    else
                    {
                        System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 10));
                        dockPanel = dockManager1.AddPanel(floatpoint);
                    }

                    dockPanel.Text = "Hexviewer: " + Path.GetFileName(Tools.Instance.m_currentfile);
                    HexViewer hv = new HexViewer();
                    hv.Issramviewer = false;
                    hv.Dock = DockStyle.Fill;
                    dockPanel.Width = 580;
                    hv.LoadDataFromFile(Tools.Instance.m_currentfile, Tools.Instance.m_symbols);
                    dockPanel.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(dockPanel_ClosedPanel);
                    dockPanel.Controls.Add(hv);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private bool ValidateFile()
        {
            bool retval = true;
            if (File.Exists(Tools.Instance.m_currentfile))
            {
                if (Tools.Instance.m_currentfile == string.Empty)
                {
                    retval = false;
                }
                else
                {
                    FileInfo fi = new FileInfo(Tools.Instance.m_currentfile);
                    if (fi.Length != 0x80000)
                    {
                        retval = false;
                    }
                }
            }
            else
            {
                retval = false;
                Tools.Instance.m_currentfile = string.Empty;
            }
            return retval;
        }

        private void btnSearchMaps_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask the user for which value to search and if searching should include symbolnames and/or symbol description
            if (ValidateFile())
            {
                SymbolCollection result_Collection = new SymbolCollection();
                frmSearchMaps searchoptions = new frmSearchMaps();
                if (searchoptions.ShowDialog() == DialogResult.OK)
                {
                    frmProgress progress = new frmProgress();
                    progress.SetProgress("Start searching data...");
                    progress.SetProgressPercentage(0);
                    progress.Show();
                    System.Windows.Forms.Application.DoEvents();
                    int cnt = 0;
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        progress.SetProgress("Searching " + sh.Varname);
                        progress.SetProgressPercentage((cnt * 100) / Tools.Instance.m_symbols.Count);
                        bool hit_found = false;
                        if (searchoptions.IncludeSymbolNames)
                        {
                            if (searchoptions.SearchForNumericValues)
                            {
                                if (sh.Varname.Contains(searchoptions.NumericValueToSearchFor.ToString()))
                                {
                                    hit_found = true;
                                }
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                if (searchoptions.StringValueToSearchFor != string.Empty)
                                {
                                    if (sh.Varname.Contains(searchoptions.StringValueToSearchFor))
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                        }
                        if (searchoptions.IncludeSymbolDescription)
                        {
                            if (searchoptions.SearchForNumericValues)
                            {
                                if (sh.Description.Contains(searchoptions.NumericValueToSearchFor.ToString()))
                                {
                                    hit_found = true;
                                }
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                if (searchoptions.StringValueToSearchFor != string.Empty)
                                {
                                    if (sh.Description.Contains(searchoptions.StringValueToSearchFor))
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                        }
                        // now search the symbol data
                        if (sh.Flash_start_address < Tools.Instance.m_currentfilelength)
                        {
                            byte[] symboldata = Tools.Instance.readdatafromfile(Tools.Instance.m_currentfile, (int)sh.Flash_start_address, sh.Length, Tools.Instance.m_currentFileType);
                            if (searchoptions.SearchForNumericValues)
                            {
                                for (int i = 0; i < symboldata.Length / 2; i += 2)
                                {
                                    float value = Convert.ToInt32(symboldata.GetValue(i)) * 256;
                                    value += Convert.ToInt32(symboldata.GetValue(i + 1));
                                    value *= (float)GetMapCorrectionFactor(sh.Varname);
                                    value += (float)GetMapCorrectionOffset(sh.Varname);
                                    if (value == (float)searchoptions.NumericValueToSearchFor)
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                if (searchoptions.StringValueToSearchFor.Length > symboldata.Length)
                                {
                                    // possible...
                                    string symboldataasstring = System.Text.Encoding.ASCII.GetString(symboldata);
                                    if (symboldataasstring.Contains(searchoptions.StringValueToSearchFor))
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                        }

                        if (hit_found)
                        {
                            // add to collection
                            result_Collection.Add(sh);
                        }
                        cnt++;
                    }
                    progress.Close();
                    if (result_Collection.Count == 0)
                    {
                        frmInfoBox info = new frmInfoBox("No results found...");
                    }
                    else
                    {
                        // start result screen
                        dockManager1.BeginUpdate();
                        try
                        {
                            SymbolTranslator st = new SymbolTranslator();
                            DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                            CompareResults tabdet = new CompareResults();
                            tabdet.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
                            tabdet.SetFilterMode(m_appSettings.ShowAddressesInHex);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.UseForFind = true;
                            tabdet.Filename = Tools.Instance.m_currentfile;
                            tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelectForFind);
                            dockPanel.Controls.Add(tabdet);
                            dockPanel.Text = "Search results: " + Path.GetFileName(Tools.Instance.m_currentfile);
                            dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);

                            dockPanel.Width = 500;

                            System.Data.DataTable dt = new System.Data.DataTable();
                            dt.Columns.Add("SYMBOLNAME");
                            dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                            dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                            dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                            dt.Columns.Add("DESCRIPTION");
                            dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                            dt.Columns.Add("CATEGORY"); //0
                            dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
                            dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
                            dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
                            dt.Columns.Add("CATEGORYNAME");
                            dt.Columns.Add("SUBCATEGORYNAME");
                            dt.Columns.Add("SymbolNumber1", Type.GetType("System.Int32"));
                            dt.Columns.Add("SymbolNumber2", Type.GetType("System.Int32"));
                            dt.Columns.Add("CodeBlock1", Type.GetType("System.Int32"));
                            dt.Columns.Add("CodeBlock2", Type.GetType("System.Int32"));

                            string ht = string.Empty;
                            XDFCategories cat = XDFCategories.Undocumented;
                            XDFSubCategory subcat = XDFSubCategory.Undocumented;
                            foreach (SymbolHelper shfound in result_Collection)
                            {
                                string helptext = st.TranslateSymbolToHelpText(shfound.Varname);
                                if (shfound.Varname.Contains("."))
                                {
                                    try
                                    {
                                        shfound.Category = shfound.Varname.Substring(0, shfound.Varname.IndexOf("."));
                                    }
                                    catch (Exception cE)
                                    {
                                        Console.WriteLine("Failed to assign category to symbol: " + shfound.Varname + " err: " + cE.Message);
                                    }
                                }
                                dt.Rows.Add(shfound.Varname, shfound.Start_address, shfound.Flash_start_address, shfound.Length, shfound.Length, helptext, false, 0, 0, 0, 0, shfound.Category, "", shfound.Symbol_number, shfound.Symbol_number, shfound.CodeBlock, shfound.CodeBlock);
                            }
                            tabdet.CompareSymbolCollection = result_Collection;
                            tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
                            tabdet.gridControl1.DataSource = dt.Copy();

                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                        dockManager1.EndUpdate();

                    }


                }
            }
        }

        void tabdet_onSymbolSelectForFind(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            StartTableViewer(e.SymbolName, e.CodeBlock1);
        }

        private void btnSaveAs_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            sfd.Title = "Save current file as... ";
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.Copy(Tools.Instance.m_currentfile, sfd.FileName, true);
                if (MessageBox.Show("Do you want to open the newly saved file?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    m_appSettings.Lastprojectname = "";
                    CloseProject();
                    OpenFile(sfd.FileName, true);
                    m_appSettings.LastOpenedType = 0;
                }
            }
        }

        private void CloseProject()
        {
            Tools.Instance.m_CurrentWorkingProject = string.Empty;
            Tools.Instance.m_currentfile = string.Empty;
            gridControl1.DataSource = null;
            barFilenameText.Caption = "No file";
            m_appSettings.Lastfilename = string.Empty;

            btnCloseProject.Enabled = false;
            btnShowProjectLogbook.Enabled = false;
            btnProduceLatestBinary.Enabled = false;
            btnAddNoteToProject.Enabled = false;
            btnEditProject.Enabled = false;
            btnRebuildFile.Enabled = false;
            btnRollback.Enabled = false;
            btnRollforward.Enabled = false;
            btnShowTransactionLog.Enabled = false;

            this.Text = "VAGEDCSuite";
        }


        private void OpenProject(string projectname)
        {
            //TODO: Are there pending changes in the optionally currently opened binary file / project?
            if (Directory.Exists(m_appSettings.ProjectFolder + "\\" + projectname))
            {
                m_appSettings.LastOpenedType = 1;
                Tools.Instance.m_CurrentWorkingProject = projectname;
                Tools.Instance.m_ProjectLog.OpenProjectLog(m_appSettings.ProjectFolder + "\\" + projectname);
                //Load the binary file that comes with this project
                LoadBinaryForProject(projectname);
                //LoadAFRMapsForProject(projectname); // <GS-27072010> TODO: nog bekijken voor T7
                if (Tools.Instance.m_currentfile != string.Empty)
                {
                    // transaction log <GS-15032010>

                    Tools.Instance.m_ProjectTransactionLog = new TransactionLog();
                    if (Tools.Instance.m_ProjectTransactionLog.OpenTransActionLog(m_appSettings.ProjectFolder, projectname))
                    {
                        Tools.Instance.m_ProjectTransactionLog.ReadTransactionFile();
                        if (Tools.Instance.m_ProjectTransactionLog.TransCollection.Count > 2000)
                        {
                            frmProjectTransactionPurge frmPurge = new frmProjectTransactionPurge();
                            frmPurge.SetNumberOfTransactions(Tools.Instance.m_ProjectTransactionLog.TransCollection.Count);
                            if (frmPurge.ShowDialog() == DialogResult.OK)
                            {
                                Tools.Instance.m_ProjectTransactionLog.Purge();
                            }
                        }
                    }
                    // transaction log <GS-15032010>
                    btnCloseProject.Enabled = true;
                    btnAddNoteToProject.Enabled = true;
                    btnEditProject.Enabled = true;
                    btnShowProjectLogbook.Enabled = true;
                    btnProduceLatestBinary.Enabled = true;
                    //btncreateb                    
                    btnRebuildFile.Enabled = true;
                    CreateProjectBackupFile();
                    UpdateRollbackForwardControls();
                    m_appSettings.Lastprojectname = Tools.Instance.m_CurrentWorkingProject;
                    this.Text = "VAGEDCSuite [Project: " + projectname + "]";
                }
            }
        }

        private void UpdateRollbackForwardControls()
        {
            btnRollback.Enabled = false;
            btnRollforward.Enabled = false;
            btnShowTransactionLog.Enabled = false;
            if (Tools.Instance.m_ProjectTransactionLog != null)
            {
                for (int t = Tools.Instance.m_ProjectTransactionLog.TransCollection.Count - 1; t >= 0; t--)
                {
                    if (!btnShowTransactionLog.Enabled) btnShowTransactionLog.Enabled = true;
                    if (Tools.Instance.m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                    {
                        btnRollforward.Enabled = true;
                    }
                    else
                    {
                        btnRollback.Enabled = true;
                    }
                }
            }
        }

        private void CreateProjectBackupFile()
        {
            // create a backup file automatically! <GS-16032010>
            if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "\\Backups");
            string filename = m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(Tools.Instance.m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
            File.Copy(GetBinaryForProject(Tools.Instance.m_CurrentWorkingProject), filename);
            if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
            {
                Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.BackupfileCreated, filename);
            }


        }


        private void LoadBinaryForProject(string projectname)
        {
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
                projectprops.Columns.Add("CARMAKE");
                projectprops.Columns.Add("CARMODEL");
                projectprops.Columns.Add("CARMY");
                projectprops.Columns.Add("CARVIN");
                projectprops.Columns.Add("NAME");
                projectprops.Columns.Add("BINFILE");
                projectprops.Columns.Add("VERSION");
                projectprops.ReadXml(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml");
                // valid project, add it to the list
                if (projectprops.Rows.Count > 0)
                {
                    OpenFile(projectprops.Rows[0]["BINFILE"].ToString(), true);
                }
            }
        }

        private string GetBinaryForProject(string projectname)
        {
            string retval = Tools.Instance.m_currentfile;
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
                projectprops.Columns.Add("CARMAKE");
                projectprops.Columns.Add("CARMODEL");
                projectprops.Columns.Add("CARMY");
                projectprops.Columns.Add("CARVIN");
                projectprops.Columns.Add("NAME");
                projectprops.Columns.Add("BINFILE");
                projectprops.Columns.Add("VERSION");
                projectprops.ReadXml(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml");
                // valid project, add it to the list
                if (projectprops.Rows.Count > 0)
                {
                    retval = projectprops.Rows[0]["BINFILE"].ToString();
                }
            }
            return retval;
        }

        private string GetBackupOlderThanDateTime(string project, DateTime mileDT)
        {
            string retval = Tools.Instance.m_currentfile; // default = current file
            string BackupPath = m_appSettings.ProjectFolder + "\\" + project + "\\Backups";
            DateTime MaxDateTime = DateTime.MinValue;
            string foundBackupfile = string.Empty;
            if (Directory.Exists(BackupPath))
            {
                string[] backupfiles = Directory.GetFiles(BackupPath, "*.bin");
                foreach (string backupfile in backupfiles)
                {
                    FileInfo fi = new FileInfo(backupfile);
                    if (fi.LastAccessTime > MaxDateTime && fi.LastAccessTime <= mileDT)
                    {
                        MaxDateTime = fi.LastAccessTime;
                        foundBackupfile = backupfile;
                    }
                }
            }
            if (foundBackupfile != string.Empty)
            {
                retval = foundBackupfile;
            }
            return retval;
        }

        private void btnRebuildFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show the transactionlog again and ask the user upto what datetime he wants to rebuild the file
            // first ask a datetime
            frmRebuildFileParameters filepar = new frmRebuildFileParameters();
            if (filepar.ShowDialog() == DialogResult.OK)
            {

                // get the last backup that is older than the selected datetime
                string file2Process = GetBackupOlderThanDateTime(Tools.Instance.m_CurrentWorkingProject, filepar.SelectedDateTime);
                // now rebuild the file
                // first create a copy of this file
                string tempRebuildFile = m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "rebuild.bin";
                if (File.Exists(tempRebuildFile))
                {
                    File.Delete(tempRebuildFile);
                }
                // CREATE A BACKUP FILE HERE
                CreateProjectBackupFile();
                File.Copy(file2Process, tempRebuildFile);
                // now do all the transactions newer than this file and older than the selected date time
                FileInfo fi = new FileInfo(file2Process);
                foreach (TransactionEntry te in Tools.Instance.m_ProjectTransactionLog.TransCollection)
                {
                    if (te.EntryDateTime >= fi.LastAccessTime && te.EntryDateTime <= filepar.SelectedDateTime)
                    {
                        // apply this change
                        RollForwardOnFile(tempRebuildFile, te);
                    }
                }
                // rename/copy file
                if (filepar.UseAsNewProjectFile)
                {
                    // just delete the current file
                    File.Delete(Tools.Instance.m_currentfile);
                    File.Copy(tempRebuildFile, Tools.Instance.m_currentfile);
                    File.Delete(tempRebuildFile);
                    // done
                }
                else
                {
                    // ask for destination file
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save rebuild file as...";
                    sfd.Filter = "Binary files|*.bin";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                        File.Copy(tempRebuildFile, sfd.FileName);
                        File.Delete(tempRebuildFile);
                    }
                }
                if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
                {
                    Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.ProjectFileRecreated, "Reconstruct upto " + filepar.SelectedDateTime.ToString("dd/MM/yyyy") + " selected file " + file2Process);
                }
                UpdateRollbackForwardControls();
            }
        }

        private void RollForwardOnFile(string file2Rollback, TransactionEntry entry)
        {
            FileInfo fi = new FileInfo(file2Rollback);
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > fi.Length) addressToWrite -= (int)fi.Length;
            Tools.Instance.savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataAfter, file2Rollback, false, Tools.Instance.m_currentFileType);
            VerifyChecksum(Tools.Instance.m_currentfile, false, false);
        }

        private string MakeDirName(string dirname)
        {
            string retval = dirname;
            retval = retval.Replace(@"\", "");
            retval = retval.Replace(@"/", "");
            retval = retval.Replace(@":", "");
            retval = retval.Replace(@"*", "");
            retval = retval.Replace(@"?", "");
            retval = retval.Replace(@">", "");
            retval = retval.Replace(@"<", "");
            retval = retval.Replace(@"|", "");
            return retval;
        }

        private void btnCreateAProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show the project properties screen for the user to fill in
            // if a bin file is loaded, ask the user whether this should be the new projects binary file
            // the project XML should contain a reference to this binfile as well as a lot of other stuff
            frmProjectProperties projectprops = new frmProjectProperties();
            if (Tools.Instance.m_currentfile != string.Empty)
            {
                projectprops.BinaryFile = Tools.Instance.m_currentfile;
                projectprops.CarModel = barPartnumber.Caption;// fileheader.getCarDescription().Trim();

                projectprops.ProjectName = DateTime.Now.ToString("yyyyMMdd") + "_" + barAdditionalInfo.Caption;// fileheader.getPartNumber().Trim() + " " + fileheader.getSoftwareVersion().Trim();
            }
            if (projectprops.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(m_appSettings.ProjectFolder)) Directory.CreateDirectory(m_appSettings.ProjectFolder);
                // create a new folder with these project properties.
                // also copy the binary file into the subfolder for this project
                if (Directory.Exists(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName)))
                {
                    frmInfoBox info = new frmInfoBox("The chosen projectname already exists, please choose another one");
                }
                else
                {
                    // create the project
                    Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName));
                    // copy the selected binary file to this folder
                    string binfilename = m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName) + "\\" + Path.GetFileName(projectprops.BinaryFile);
                    File.Copy(projectprops.BinaryFile, binfilename);
                    // now create the projectproperties.xml in this new folder
                    System.Data.DataTable dtProps = new System.Data.DataTable("T5PROJECT");
                    dtProps.Columns.Add("CARMAKE");
                    dtProps.Columns.Add("CARMODEL");
                    dtProps.Columns.Add("CARMY");
                    dtProps.Columns.Add("CARVIN");
                    dtProps.Columns.Add("NAME");
                    dtProps.Columns.Add("BINFILE");
                    dtProps.Columns.Add("VERSION");
                    dtProps.Rows.Add(projectprops.CarMake, projectprops.CarModel, projectprops.CarMY, projectprops.CarVIN, MakeDirName(projectprops.ProjectName), binfilename, projectprops.Version);
                    dtProps.WriteXml(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName) + "\\projectproperties.xml");
                    OpenProject(projectprops.ProjectName); //?
                }
            }
        }

        private void btnOpenProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            //let the user select a project from the Project folder. If none are present, let the user know
            if (!Directory.Exists(m_appSettings.ProjectFolder)) Directory.CreateDirectory(m_appSettings.ProjectFolder);
            System.Data.DataTable ValidProjects = new System.Data.DataTable();
            ValidProjects.Columns.Add("Projectname");
            ValidProjects.Columns.Add("NumberBackups");
            ValidProjects.Columns.Add("NumberTransactions");
            ValidProjects.Columns.Add("DateTimeModified");
            ValidProjects.Columns.Add("Version");
            string[] projects = Directory.GetDirectories(m_appSettings.ProjectFolder);
            // filter for folders with a projectproperties.xml file
            foreach (string project in projects)
            {
                string[] projectfiles = Directory.GetFiles(project, "projectproperties.xml");

                if (projectfiles.Length > 0)
                {
                    System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
                    projectprops.Columns.Add("CARMAKE");
                    projectprops.Columns.Add("CARMODEL");
                    projectprops.Columns.Add("CARMY");
                    projectprops.Columns.Add("CARVIN");
                    projectprops.Columns.Add("NAME");
                    projectprops.Columns.Add("BINFILE");
                    projectprops.Columns.Add("VERSION");
                    projectprops.ReadXml((string)projectfiles.GetValue(0));
                    // valid project, add it to the list
                    if (projectprops.Rows.Count > 0)
                    {
                        string projectName = projectprops.Rows[0]["NAME"].ToString();
                        ValidProjects.Rows.Add(projectName, GetNumberOfBackups(projectName), GetNumberOfTransactions(projectName), GetLastAccessTime(projectprops.Rows[0]["BINFILE"].ToString()), projectprops.Rows[0]["VERSION"].ToString());
                    }
                }
            }
            if (ValidProjects.Rows.Count > 0)
            {
                frmProjectSelection projselection = new frmProjectSelection();
                projselection.SetDataSource(ValidProjects);
                if (projselection.ShowDialog() == DialogResult.OK)
                {
                    string selectedproject = projselection.GetProjectName();
                    if (selectedproject != "")
                    {
                        OpenProject(selectedproject);
                    }

                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No projects were found, please create one first!");
            }
        }

        private int GetNumberOfBackups(string project)
        {
            int retval = 0;
            string dirname = m_appSettings.ProjectFolder + "\\" + project + "\\Backups";
            if (!Directory.Exists(dirname)) Directory.CreateDirectory(dirname);
            string[] backupfiles = Directory.GetFiles(dirname, "*.bin");
            retval = backupfiles.Length;
            return retval;
        }

        private int GetNumberOfTransactions(string project)
        {
            int retval = 0;
            string filename = m_appSettings.ProjectFolder + "\\" + project + "\\TransActionLogV2.ttl";
            if (File.Exists(filename))
            {
                TransactionLog translog = new TransactionLog();
                translog.OpenTransActionLog(m_appSettings.ProjectFolder, project);
                translog.ReadTransactionFile();
                retval = translog.TransCollection.Count;
            }
            return retval;
        }

        private DateTime GetLastAccessTime(string filename)
        {
            DateTime retval = DateTime.MinValue;
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                retval = fi.LastAccessTime;
            }
            return retval;
        }

        private void btnCloseProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            CloseProject();
            m_appSettings.Lastprojectname = "";
        }

        private void btnShowTransactionLog_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show new form
            if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
            {
                frmTransactionLog translog = new frmTransactionLog();
                translog.onRollBack += new frmTransactionLog.RollBack(translog_onRollBack);
                translog.onRollForward += new frmTransactionLog.RollForward(translog_onRollForward);
                translog.onNoteChanged += new frmTransactionLog.NoteChanged(translog_onNoteChanged);
                foreach (TransactionEntry entry in Tools.Instance.m_ProjectTransactionLog.TransCollection)
                {
                    entry.SymbolName = Tools.Instance.GetSymbolNameByAddress(entry.SymbolAddress);

                }
                translog.SetTransactionLog(Tools.Instance.m_ProjectTransactionLog);
                translog.Show();
            }
        }


        void translog_onNoteChanged(object sender, frmTransactionLog.RollInformationEventArgs e)
        {
            Tools.Instance.m_ProjectTransactionLog.SetEntryNote(e.Entry);
        }

        void translog_onRollForward(object sender, frmTransactionLog.RollInformationEventArgs e)
        {
            // alter the log!
            // rollback the transaction
            // now reload the list
            RollForward(e.Entry);
            if (sender is frmTransactionLog)
            {
                frmTransactionLog logfrm = (frmTransactionLog)sender;
                if (Tools.Instance.m_ProjectTransactionLog != null)
                {
                    logfrm.SetTransactionLog(Tools.Instance.m_ProjectTransactionLog);
                }
            }
        }

        private void RollForward(TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            Tools.Instance.savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataAfter, Tools.Instance.m_currentfile, false, Tools.Instance.m_currentFileType);
            VerifyChecksum(Tools.Instance.m_currentfile, false, false);
            if (Tools.Instance.m_ProjectTransactionLog != null)
            {
                Tools.Instance.m_ProjectTransactionLog.SetEntryRolledForward(entry.TransactionNumber);
            }
            if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
            {

                Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledforward, Tools.Instance.GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
            }

            UpdateRollbackForwardControls();
        }

        void translog_onRollBack(object sender, frmTransactionLog.RollInformationEventArgs e)
        {
            // alter the log!
            // rollback the transaction
            RollBack(e.Entry);
            // now reload the list
            if (sender is frmTransactionLog)
            {
                frmTransactionLog logfrm = (frmTransactionLog)sender;
                logfrm.SetTransactionLog(Tools.Instance.m_ProjectTransactionLog);
            }
        }


        private void RollBack(TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            Tools.Instance.savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataBefore, Tools.Instance.m_currentfile, false, Tools.Instance.m_currentFileType);
            VerifyChecksum(Tools.Instance.m_currentfile, false, false);
            if (Tools.Instance.m_ProjectTransactionLog != null)
            {
                Tools.Instance.m_ProjectTransactionLog.SetEntryRolledBack(entry.TransactionNumber);
            }
            if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
            {
                Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledback, Tools.Instance.GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
            }
            UpdateRollbackForwardControls();
        }

        private void btnRollback_ItemClick(object sender, ItemClickEventArgs e)
        {
            //roll back last entry in the log that has not been rolled back
            if (Tools.Instance.m_ProjectTransactionLog != null)
            {
                for (int t = Tools.Instance.m_ProjectTransactionLog.TransCollection.Count - 1; t >= 0; t--)
                {
                    if (!Tools.Instance.m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                    {
                        RollBack(Tools.Instance.m_ProjectTransactionLog.TransCollection[t]);

                        break;
                    }
                }
            }
        }

        private void btnRollforward_ItemClick(object sender, ItemClickEventArgs e)
        {
            //roll back last entry in the log that has not been rolled back
            if (Tools.Instance.m_ProjectTransactionLog != null)
            {
                for (int t = 0; t < Tools.Instance.m_ProjectTransactionLog.TransCollection.Count; t++)
                {
                    if (Tools.Instance.m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                    {
                        RollForward(Tools.Instance.m_ProjectTransactionLog.TransCollection[t]);

                        break;
                    }
                }
            }
        }

        private void btnRebuildFile_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            // show the transactionlog again and ask the user upto what datetime he wants to rebuild the file
            // first ask a datetime
            frmRebuildFileParameters filepar = new frmRebuildFileParameters();
            if (filepar.ShowDialog() == DialogResult.OK)
            {

                // get the last backup that is older than the selected datetime
                string file2Process = GetBackupOlderThanDateTime(Tools.Instance.m_CurrentWorkingProject, filepar.SelectedDateTime);
                // now rebuild the file
                // first create a copy of this file
                string tempRebuildFile = m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "rebuild.bin";
                if (File.Exists(tempRebuildFile))
                {
                    File.Delete(tempRebuildFile);
                }
                // CREATE A BACKUP FILE HERE
                CreateProjectBackupFile();
                File.Copy(file2Process, tempRebuildFile);
                FileInfo fi = new FileInfo(file2Process);
                foreach (TransactionEntry te in Tools.Instance.m_ProjectTransactionLog.TransCollection)
                {
                    if (te.EntryDateTime >= fi.LastAccessTime && te.EntryDateTime <= filepar.SelectedDateTime)
                    {
                        // apply this change
                        RollForwardOnFile(tempRebuildFile, te);
                    }
                }
                // rename/copy file
                if (filepar.UseAsNewProjectFile)
                {
                    // just delete the current file
                    File.Delete(Tools.Instance.m_currentfile);
                    File.Copy(tempRebuildFile, Tools.Instance.m_currentfile);
                    File.Delete(tempRebuildFile);
                    // done
                }
                else
                {
                    // ask for destination file
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save rebuild file as...";
                    sfd.Filter = "Binary files|*.bin";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                        File.Copy(tempRebuildFile, sfd.FileName);
                        File.Delete(tempRebuildFile);
                    }
                }
                if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
                {
                    Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.ProjectFileRecreated, "Reconstruct upto " + filepar.SelectedDateTime.ToString("dd/MM/yyyy") + " selected file " + file2Process);
                }
                UpdateRollbackForwardControls();
            }
        }

        private void btnEditProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
            {
                EditProjectProperties(Tools.Instance.m_CurrentWorkingProject);
            }
        }

        private void EditProjectProperties(string project)
        {
            // edit current project properties
            System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
            projectprops.Columns.Add("CARMAKE");
            projectprops.Columns.Add("CARMODEL");
            projectprops.Columns.Add("CARMY");
            projectprops.Columns.Add("CARVIN");
            projectprops.Columns.Add("NAME");
            projectprops.Columns.Add("BINFILE");
            projectprops.Columns.Add("VERSION");
            projectprops.ReadXml(m_appSettings.ProjectFolder + "\\" + project + "\\projectproperties.xml");

            frmProjectProperties projectproperties = new frmProjectProperties();
            projectproperties.Version = projectprops.Rows[0]["VERSION"].ToString();
            projectproperties.ProjectName = projectprops.Rows[0]["NAME"].ToString();
            projectproperties.CarMake = projectprops.Rows[0]["CARMAKE"].ToString();
            projectproperties.CarModel = projectprops.Rows[0]["CARMODEL"].ToString();
            projectproperties.CarVIN = projectprops.Rows[0]["CARVIN"].ToString();
            projectproperties.CarMY = projectprops.Rows[0]["CARMY"].ToString();
            projectproperties.BinaryFile = projectprops.Rows[0]["BINFILE"].ToString();
            bool _reopenProject = false;
            if (projectproperties.ShowDialog() == DialogResult.OK)
            {
                // delete the original XML file
                if (project != projectproperties.ProjectName)
                {
                    Directory.Move(m_appSettings.ProjectFolder + "\\" + project, m_appSettings.ProjectFolder + "\\" + projectproperties.ProjectName);
                    project = projectproperties.ProjectName;
                    Tools.Instance.m_CurrentWorkingProject = project;
                    // set the working file to the correct folder
                    projectproperties.BinaryFile = Path.Combine(m_appSettings.ProjectFolder + "\\" + project, Path.GetFileName(projectprops.Rows[0]["BINFILE"].ToString()));
                    _reopenProject = true;
                    // open this project

                }

                File.Delete(m_appSettings.ProjectFolder + "\\" + project + "\\projectproperties.xml");
                System.Data.DataTable dtProps = new System.Data.DataTable("T5PROJECT");
                dtProps.Columns.Add("CARMAKE");
                dtProps.Columns.Add("CARMODEL");
                dtProps.Columns.Add("CARMY");
                dtProps.Columns.Add("CARVIN");
                dtProps.Columns.Add("NAME");
                dtProps.Columns.Add("BINFILE");
                dtProps.Columns.Add("VERSION");
                dtProps.Rows.Add(projectproperties.CarMake, projectproperties.CarModel, projectproperties.CarMY, projectproperties.CarVIN, MakeDirName(projectproperties.ProjectName), projectproperties.BinaryFile, projectproperties.Version);
                dtProps.WriteXml(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectproperties.ProjectName) + "\\projectproperties.xml");
                if (_reopenProject)
                {
                    OpenProject(Tools.Instance.m_CurrentWorkingProject);
                }
                Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.PropertiesEdited, projectproperties.Version);

            }

        }

        private void btnAddNoteToProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmChangeNote newNote = new frmChangeNote();
            newNote.ShowDialog();
            if (newNote.Note != string.Empty)
            {
                if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
                {
                    Tools.Instance.m_ProjectLog.WriteLogbookEntry(LogbookEntryType.Note, newNote.Note);
                }
            }
        }

        private void btnShowProjectLogbook_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Tools.Instance.m_CurrentWorkingProject != string.Empty)
            {
                frmProjectLogbook logb = new frmProjectLogbook();

                logb.LoadLogbookForProject(m_appSettings.ProjectFolder, Tools.Instance.m_CurrentWorkingProject);
                logb.Show();
            }
        }

        private void btnProduceLatestBinary_ItemClick(object sender, ItemClickEventArgs e)
        {
            // save binary as
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // copy the current project file to the selected destination
                File.Copy(Tools.Instance.m_currentfile, sfd.FileName, true);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayoutFiles();
            if (Tools.Instance.m_CurrentWorkingProject != "")
            {
                CloseProject();
            }
        }

        private void btnCreateBackup_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Tools.Instance.m_currentfile != string.Empty)
            {
                VerifyChecksum(Tools.Instance.m_currentfile, false, false);

                if (File.Exists(Tools.Instance.m_currentfile))
                {
                    if (Tools.Instance.m_CurrentWorkingProject != "")
                    {
                        if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "\\Backups");
                        string filename = m_appSettings.ProjectFolder + "\\" + Tools.Instance.m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(Tools.Instance.m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                        File.Copy(GetBinaryForProject(Tools.Instance.m_CurrentWorkingProject), filename);
                    }
                    else
                    {
                        File.Copy(Tools.Instance.m_currentfile, Path.GetDirectoryName(Tools.Instance.m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(Tools.Instance.m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup", true);
                        frmInfoBox info = new frmInfoBox("Backup created: " + Path.GetDirectoryName(Tools.Instance.m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(Tools.Instance.m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup");
                    }
                }
            }
        }

        private void btnLookupPartnumber_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmPartnumberLookup lookup = new frmPartnumberLookup();
            lookup.ShowDialog();
            if (lookup.Open_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    CloseProject();
                    m_appSettings.Lastprojectname = "";
                    OpenFile(filename, true);
                    m_appSettings.LastOpenedType = 0;

                }
            }
            else if (lookup.Compare_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {

                    CompareToFile(filename);
                }
            }
            else if (lookup.CreateNewFile)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    CloseProject();
                    m_appSettings.Lastprojectname = "";
                    File.Copy(filename, lookup.FileNameToSave);
                    OpenFile(lookup.FileNameToSave, true);
                    m_appSettings.LastOpenedType = 0;

                }
            }
        }

        private void btnFirmwareInformation_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Tools.Instance.m_currentfile != string.Empty)
            {
                if(File.Exists(Tools.Instance.m_currentfile))
                {

                    byte[] allBytes = File.ReadAllBytes(Tools.Instance.m_currentfile);
                    IEDCFileParser parser = Tools.Instance.GetParserForFile(Tools.Instance.m_currentfile, false);
                    partNumberConverter pnc = new partNumberConverter();
                    ECUInfo ecuinfo = pnc.ConvertPartnumber(parser.ExtractBoschPartnumber(allBytes), allBytes.Length);
                    frmFirmwareInfo info = new frmFirmwareInfo();
                    info.InfoString = parser.ExtractInfo(allBytes);
                    info.partNumber = parser.ExtractBoschPartnumber(allBytes);
                    if(ecuinfo.SoftwareID == "") ecuinfo.SoftwareID = parser.ExtractPartnumber(allBytes);
                    info.SoftwareID = ecuinfo.SoftwareID + " " + parser.ExtractSoftwareNumber(allBytes);
                    info.carDetails = ecuinfo.CarMake + " " + ecuinfo.CarType;
                    string enginedetails = ecuinfo.EngineType;
                    string hpinfo = string.Empty;
                    string tqinfo = string.Empty;
                    if (ecuinfo.HP > 0) hpinfo = ecuinfo.HP.ToString() + " bhp";
                    if (ecuinfo.TQ > 0) tqinfo = ecuinfo.TQ.ToString() + " Nm";
                    if (hpinfo != string.Empty || tqinfo != string.Empty)
                    {
                        enginedetails += " (";
                        if (hpinfo != string.Empty) enginedetails += hpinfo;
                        if (hpinfo != string.Empty && tqinfo != string.Empty) enginedetails += "/";
                        if (tqinfo != string.Empty) enginedetails += tqinfo;
                        enginedetails += ")";
                    }
                    info.EngineType = /*ecuinfo.EngineType*/ enginedetails;
                    info.ecuDetails = ecuinfo.EcuType;
                    //DumpECUInfo(ecuinfo);
                    ChecksumResultDetails result = Tools.Instance.UpdateChecksum(Tools.Instance.m_currentfile, true);
                    string chkType = string.Empty;
                    if (result.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = "VAG EDC15P V4.1";
                    else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = "VAG EDC15P V4.1v2";
                    else if (result.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = "VAG EDC15P V4.1 2002+";
                    else if (result.TypeResult != ChecksumType.Unknown) chkType = result.TypeResult.ToString();

                    chkType += " " + result.CalculationResult.ToString();

                    info.checksumType = chkType;
                    // number of codeblocks?
                    info.codeBlocks = DetermineNumberOfCodeblocks().ToString();
                    info.ShowDialog();
                }
            }
        }

        private int DetermineNumberOfCodeblocks()
        {
            List<int> blockIds = new List<int>();
            foreach (SymbolHelper sh in Tools.Instance.m_symbols)
            {
                if (!blockIds.Contains(sh.CodeBlock) && sh.CodeBlock != 0) blockIds.Add(sh.CodeBlock);
            }
            return blockIds.Count;
        }

        private void DumpECUInfo(ECUInfo ecuinfo)
        {
            Console.WriteLine("Partnr: " + ecuinfo.PartNumber);
            Console.WriteLine("Make  : " + ecuinfo.CarMake);
            Console.WriteLine("Type  : " + ecuinfo.CarType);
            Console.WriteLine("ECU   : " + ecuinfo.EcuType);
            Console.WriteLine("Engine: " + ecuinfo.EngineType);
            Console.WriteLine("SWID  : " + ecuinfo.SoftwareID);
            Console.WriteLine("HP    : " + ecuinfo.HP.ToString());
            Console.WriteLine("TQ    : " + ecuinfo.TQ.ToString());
        }

        private void btnVINDecoder_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmDecodeVIN vindec = new frmDecodeVIN();
            vindec.Show();
            //frmInfoBox info = new frmInfoBox("Not yet implemented");
        }

        private void btnChecksum_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            if (Tools.Instance.m_currentfile != string.Empty)
            {
                if (File.Exists(Tools.Instance.m_currentfile))
                {
                    VerifyChecksum(Tools.Instance.m_currentfile, true, true);
                }
            }
        }


        private void btnDriverWish_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Driver wish", 2);
        }

        private void btnTorqueLimiter_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Torque limiter", 2);
        }

        private void btnSmokeLimiter_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Smoke limiter", 2);
        }

        private void btnTargetBoost_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Boost target map", 2);
        }

        private void btnBoostPressureLimiter_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Boost limit map", 2);
        }

        private void btnBoostPressureLimitSVBL_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("SVBL Boost limiter", 2);
        }

        private void btnN75Map_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("N75 duty cycle", 2);
        }

        private void editXAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 
            object o = gridViewSymbols.GetFocusedRow();
            if (o is SymbolHelper)
            {
                SymbolHelper sh = (SymbolHelper)o;
                StartAxisViewer(sh, Axis.XAxis);//sh.X_axis_descr, sh.Y_axis_address, sh.Y_axis_length, sh.Y_axis_ID);
            }
        }
        public enum Axis
        {
            XAxis,
            YAxis
        }
        private void StartAxisViewer(SymbolHelper symbol, Axis AxisToShow)//string Name, int address, int length, int axisID)
        {

            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            dockManager1.BeginUpdate();
            try
            {


                dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                int dw = 650;
                dockPanel.FloatSize = new Size(dw, 900);
                dockPanel.Width = dw;
                dockPanel.Tag = Tools.Instance.m_currentfile;
                ctrlAxisEditor tabdet = new ctrlAxisEditor();
                tabdet.FileName = Tools.Instance.m_currentfile;


                if (AxisToShow == Axis.XAxis)
                {
                    tabdet.AxisID = symbol.Y_axis_ID;
                    tabdet.AxisAddress = symbol.Y_axis_address;
                    tabdet.Map_name = symbol.X_axis_descr + " (" + symbol.Y_axis_address.ToString("X8") + ")";
                    int[] values = GetXaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, symbol.Varname);
                    float[] dataValues = new float[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        float fValue = (float)Convert.ToDouble(values.GetValue(i)) * (float)symbol.X_axis_correction;
                        dataValues.SetValue(fValue, i);
                    }
                    tabdet.CorrectionFactor = (float)symbol.X_axis_correction;
                    tabdet.SetData(dataValues);
                    dockPanel.Text = "Axis: (X) " + tabdet.Map_name + " [" + Path.GetFileName(Tools.Instance.m_currentfile) + "]";
                }
                else if (AxisToShow == Axis.YAxis)
                {
                    tabdet.AxisID = symbol.X_axis_ID;
                    tabdet.AxisAddress = symbol.X_axis_address;
                    tabdet.Map_name = symbol.Y_axis_descr + " (" + symbol.X_axis_address.ToString("X8") + ")";
                    int[] values = GetYaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, symbol.Varname);
                    float[] dataValues = new float[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        float fValue = (float)Convert.ToDouble(values.GetValue(i)) * (float)symbol.Y_axis_correction;
                        dataValues.SetValue(fValue, i);
                    }
                    tabdet.CorrectionFactor = (float)symbol.Y_axis_correction;
                    tabdet.SetData(dataValues);
                    dockPanel.Text = "Axis: (Y) " + tabdet.Map_name + " [" + Path.GetFileName(Tools.Instance.m_currentfile) + "]";
                }

                tabdet.onClose += new ctrlAxisEditor.ViewerClose(axis_Close);
                tabdet.onSave += new ctrlAxisEditor.DataSave(axis_Save);
                tabdet.Dock = DockStyle.Fill;
                dockPanel.Controls.Add(tabdet);
            }
            catch (Exception newdockE)
            {
                Console.WriteLine(newdockE.Message);
            }
            dockManager1.EndUpdate();

            System.Windows.Forms.Application.DoEvents();
        }
        
        void axis_Save(object sender, EventArgs e)
        {
            if (sender is ctrlAxisEditor)
            {
                ctrlAxisEditor editor = (ctrlAxisEditor)sender;
                // recalculate the values back and store it in the file at the correct location
                float[] newvalues = editor.GetData();
                // well.. recalculate the data based on these new values
                //editor.CorrectionFactor
                int[] iValues = new int[newvalues.Length];
                // calculate back to integer values
                for (int i = 0; i < newvalues.Length; i++)
                {
                    int iValue = Convert.ToInt32(Convert.ToDouble(newvalues.GetValue(i))/editor.CorrectionFactor);
                    iValues.SetValue(iValue, i);
                }
                byte[] barr = new byte[iValues.Length * 2];
                int bCount = 0;
                for (int i = 0; i < iValues.Length; i++)
                {
                    int iVal = (int)iValues.GetValue(i);
                    byte b1 = (byte)((iVal & 0x00FF00) / 256);
                    byte b2 = (byte)(iVal & 0x0000FF);
                    barr[bCount++] = b1;
                    barr[bCount++] = b2;
                }
                string note = string.Empty;
                if (m_appSettings.RequestProjectNotes && Tools.Instance.m_CurrentWorkingProject != "")
                {
                    //request a small note from the user in which he/she can denote a description of the change
                    frmChangeNote changenote = new frmChangeNote();
                    changenote.ShowDialog();
                    note = changenote.Note;
                }
                SaveAxisDataIncludingSyncOption(editor.AxisAddress, barr.Length, barr, Tools.Instance.m_currentfile, true, note);
                // and we need to update mapviewers maybe?
                UpdateOpenViewers(Tools.Instance.m_currentfile);
            }
        }


        private void UpdateViewer(MapViewerEx tabdet)
        {
            string mapname = tabdet.Map_name;
            if (tabdet.Filename == Tools.Instance.m_currentfile)
            {
                foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                {
                    if (sh.Varname == mapname)
                    {
                        // refresh data and axis in the viewer
                        SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                        string x_axis = string.Empty;
                        string y_axis = string.Empty;
                        string x_axis_descr = string.Empty;
                        string y_axis_descr = string.Empty;
                        string z_axis_descr = string.Empty;
                        tabdet.X_axis_name = sh.X_axis_descr;
                        tabdet.Y_axis_name = sh.Y_axis_descr;
                        tabdet.Z_axis_name = sh.Z_axis_descr;
                        tabdet.X_axisAddress = sh.Y_axis_address;
                        tabdet.Y_axisAddress = sh.X_axis_address;
                        tabdet.Xaxiscorrectionfactor = sh.X_axis_correction;
                        tabdet.Yaxiscorrectionfactor = sh.Y_axis_correction;
                        tabdet.Xaxiscorrectionoffset = sh.X_axis_offset;
                        tabdet.Yaxiscorrectionoffset = sh.Y_axis_offset;
                        tabdet.X_axisvalues = GetXaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);
                        tabdet.Y_axisvalues = GetYaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name);
                        int columns = 8;
                        int rows = 8;
                        int tablewidth = GetTableMatrixWitdhByName(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, tabdet.Map_name, out columns, out rows);
                        int address = Convert.ToInt32(sh.Flash_start_address);
                        tabdet.ShowTable(columns, true);
                        break;
                    }
                }
            }
        }

        private void UpdateOpenViewers(string filename)
        {

            try
            {
                // convert feedback map in memory to byte[] in stead of float[]
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text.StartsWith("Symbol: "))
                    {
                        foreach (Control c in pnl.Controls)
                        {
                            if (c is MapViewerEx)
                            {
                                MapViewerEx vwr = (MapViewerEx)c;
                                if (vwr.Filename == filename || filename == string.Empty)
                                {
                                    UpdateViewer(vwr);
                                }
                            }
                            else if (c is DevExpress.XtraBars.Docking.DockPanel)
                            {
                                DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                                foreach (Control c2 in tpnl.Controls)
                                {
                                    if (c2 is MapViewerEx)
                                    {
                                        MapViewerEx vwr2 = (MapViewerEx)c2;
                                        if (vwr2.Filename == filename || filename == string.Empty)
                                        {
                                            UpdateViewer(vwr2);
                                        }
                                    }
                                }
                            }
                            else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                            {
                                DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                                foreach (Control c3 in cntr.Controls)
                                {
                                    if (c3 is MapViewerEx)
                                    {
                                        MapViewerEx vwr3 = (MapViewerEx)c3;
                                        if (vwr3.Filename == filename || filename == string.Empty)
                                        {
                                            UpdateViewer(vwr3);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Refresh viewer error: " + E.Message);
            }
        }

        void axis_Close(object sender, EventArgs e)
        {
            tabdet_onClose(sender, EventArgs.Empty); // recast
        }
       
        private void editYAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object o = gridViewSymbols.GetFocusedRow();
            if (o is SymbolHelper)
            {
                SymbolHelper sh = (SymbolHelper)o;
                StartAxisViewer(sh, Axis.YAxis);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (gvhi != null)
            {
                if (gvhi.InColumnPanel || gvhi.InFilterPanel || gvhi.InGroupPanel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (gridViewSymbols.FocusedRowHandle < 0)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                object o = gridViewSymbols.GetFocusedRow();
                
                if (o is SymbolHelper)
                {
                    SymbolHelper sh = (SymbolHelper)o;
                    if (sh.X_axis_address > 0 && sh.X_axis_length > 0)
                    {
                        editXAxisToolStripMenuItem.Enabled = true;
                        editXAxisToolStripMenuItem.Text = "Edit x axis (" + sh.X_axis_descr + " " + sh.Y_axis_address.ToString("X8") + ")";
                    }
                    else
                    {
                        editXAxisToolStripMenuItem.Enabled = false;
                        editYAxisToolStripMenuItem.Text = "Edit x axis";
                    }
                    if (sh.Y_axis_address > 0 && sh.Y_axis_length > 0)
                    {
                        editYAxisToolStripMenuItem.Enabled = true;
                        editYAxisToolStripMenuItem.Text = "Edit y axis (" + sh.Y_axis_descr + " " + sh.X_axis_address.ToString("X8") + ")";
                    }
                    else
                    {
                        editYAxisToolStripMenuItem.Enabled = false;
                        editYAxisToolStripMenuItem.Text = "Edit y axis";
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnEGRMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("EGR", 2);
        }

        DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo gvhi;

        private void gridControl1_MouseMove(object sender, MouseEventArgs e)
        {
            gvhi = gridViewSymbols.CalcHitInfo(new Point(e.X, e.Y));
        }

        private bool CheckAllTablesAvailable()
        {
            bool retval = true;
            if (Tools.Instance.m_currentfile != "")
            {
                if (File.Exists(Tools.Instance.m_currentfile))
                {
                    //if (MapsWithNameMissing("SVBL", Tools.Instance.m_symbols)) return false;
                    if (MapsWithNameMissing("Torque limiter", Tools.Instance.m_symbols)) return false;
                    if (MapsWithNameMissing("Smoke limiter", Tools.Instance.m_symbols)) return false;
                    if (MapsWithNameMissing("Driver wish", Tools.Instance.m_symbols)) return false;
                    //if (MapsWithNameMissing("Boost limit map", Tools.Instance.m_symbols)) return false;

                }
                else retval = false;
            }
            else retval = false;
            return retval;
        }

        private void btnAirmassResult_ItemClick(object sender, ItemClickEventArgs e)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            if (CheckAllTablesAvailable())
            {
                dockManager1.BeginUpdate();
                try
                {
                    ctrlAirmassResult airmassResult = new ctrlAirmassResult();
                    airmassResult.Dock = DockStyle.Fill;
                    dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Tag = Tools.Instance.m_currentfile;
                    dockPanel.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(dockPanel_ClosedPanel);
                    dockPanel.Text = "Airmass result viewer: " + Path.GetFileName(Tools.Instance.m_currentfile);
                    dockPanel.Width = 800;
                    airmassResult.onStartTableViewer += new ctrlAirmassResult.StartTableViewer(airmassResult_onStartTableViewer);
                    airmassResult.onClose += new ctrlAirmassResult.ViewerClose(airmassResult_onClose);
                    airmassResult.Currentfile = Tools.Instance.m_currentfile;
                    airmassResult.Symbols = Tools.Instance.m_symbols;
                    airmassResult.Currentfile_size = Tools.Instance.m_currentfilelength;
                    IEDCFileParser parser = Tools.Instance.GetParserForFile(Tools.Instance.m_currentfile, false);
                    byte[] allBytes = File.ReadAllBytes(Tools.Instance.m_currentfile);
                    string additionalInfo = parser.ExtractInfo(allBytes);
                    //GetNumberOfCylinders 
                    string bpn = parser.ExtractBoschPartnumber(allBytes);
                    partNumberConverter pnc = new partNumberConverter();

                    ECUInfo info = pnc.ConvertPartnumber(bpn, allBytes.Length);
                    airmassResult.NumberCylinders = pnc.GetNumberOfCylinders(info.EngineType, additionalInfo);
                    airmassResult.ECUType = info.EcuType;
                   
                    
                    airmassResult.Calculate(Tools.Instance.m_currentfile, Tools.Instance.m_symbols);
                    dockPanel.Controls.Add(airmassResult);
                }
                catch (Exception newdockE)
                {
                    Console.WriteLine(newdockE.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        void airmassResult_onClose(object sender, EventArgs e)
        {
            // lookup the panel which cast this event
            if (sender is ctrlAirmassResult)
            {
                string dockpanelname = "Airmass result viewer: " + Path.GetFileName(Tools.Instance.m_currentfile);
                foreach (DevExpress.XtraBars.Docking.DockPanel dp in dockManager1.Panels)
                {
                    if (dp.Text == dockpanelname)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                }
            }
        }

        void airmassResult_onStartTableViewer(object sender, ctrlAirmassResult.StartTableViewerEventArgs e)
        {
            StartTableViewer(e.SymbolName, 2);
        }

        private void btnExportXDF_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog2 = new SaveFileDialog();
            saveFileDialog2.Filter = "XDF files|*.xdf";
            if (gridControl1.DataSource != null)
            {
                XDFWriter xdf = new XDFWriter();

                string filename = Path.Combine(Path.GetDirectoryName(Tools.Instance.m_currentfile), Path.GetFileNameWithoutExtension(Tools.Instance.m_currentfile));
                saveFileDialog2.FileName = filename;
                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    //filename += ".xdf";
                    filename = saveFileDialog2.FileName;

                    xdf.CreateXDF(filename, Tools.Instance.m_currentfile, Tools.Instance.m_currentfilelength, Tools.Instance.m_currentfilelength);
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        if (sh.Flash_start_address != 0)
                        {
                            int fileoffset = (int)sh.Flash_start_address;
                            while (fileoffset > Tools.Instance.m_currentfilelength) fileoffset -= Tools.Instance.m_currentfilelength;
                            /*if (sh.Varname == "Pgm_mod!") // VSS vlag
                            {
                                xdf.AddFlag("VSS", sh.Flash_start_address, 0x07);
                            }*/
                            if (sh.Varname.StartsWith("SVBL"))
                            {
                                
                            }
                            else 
                            {
                                string xaxis = sh.X_axis_descr;
                                string yaxis = sh.Y_axis_descr;
                                string zaxis = sh.Z_axis_descr;
                                bool m_issixteenbit = true;
                                // special maps are:
                                int xaxisaddress = sh.X_axis_address;
                                int yaxisaddress = sh.Y_axis_address;
                                bool isxaxissixteenbit = true;
                                bool isyaxissixteenbit = true;
                                int columns = sh.X_axis_length;
                                int rows = sh.Y_axis_length;
                                //int tablewidth = GetTableMatrixWitdhByName(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, sh.Varname, out columns, out rows);
                                xdf.AddTable(sh.Varname, sh.Description, XDFCategories.Fuel, xaxis, yaxis, zaxis, columns, rows, fileoffset, m_issixteenbit, xaxisaddress, yaxisaddress, isxaxissixteenbit, isyaxissixteenbit, 1.0F, 1.0F, 1.0F);

                            }
                            /*else
                            {
                                xdf.AddConstant(55, sh.Varname, XDFCategories.Idle, "Aantal", sh.Length, fileoffset, true);
                            }*/
                        }
                    }
                    // add some specific stuff
                    //int fileoffset2 = Tools.Instance.m_currentfile_size - 0x4C;

                    //xdf.AddTable("Vehice Security Code", "VSS code", XDFCategories.Idle, "", "", "", 1, 6, fileoffset2 /*0x3FFB4*/, false, 0, 0, false, false, 1.0F, 1.0F, 1.0F);

                    xdf.CloseFile();
                }
            }
        }

        // van t5
        void tabdet_onViewTypeChanged(object sender, MapViewerEx.ViewTypeChangedEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers || m_appSettings.SynchronizeMapviewersDifferentMaps)
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is MapViewerEx)
                        {
                            if (c != sender)
                            {
                                MapViewerEx vwr = (MapViewerEx)c;
                                if (vwr.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                {
                                    vwr.Viewtype = e.View;
                                    vwr.ReShowTable();
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is MapViewerEx)
                                {
                                    if (c2 != sender)
                                    {
                                        MapViewerEx vwr2 = (MapViewerEx)c2;
                                        if (vwr2.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr2.Viewtype = e.View;
                                            vwr2.ReShowTable();
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is MapViewerEx)
                                {
                                    if (c3 != sender)
                                    {
                                        MapViewerEx vwr3 = (MapViewerEx)c3;
                                        if (vwr3.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr3.Viewtype = e.View;
                                            vwr3.ReShowTable();
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void tabdet_onSurfaceGraphViewChangedEx(object sender, MapViewerEx.SurfaceGraphViewChangedEventArgsEx e)
        {
            if (m_appSettings.SynchronizeMapviewers || m_appSettings.SynchronizeMapviewersDifferentMaps)
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is MapViewerEx)
                        {
                            if (c != sender)
                            {
                                MapViewerEx vwr = (MapViewerEx)c;
                                if (vwr.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                {
                                    vwr.SetSurfaceGraphViewEx(e.DepthX, e.DepthY, e.Zoom, e.Rotation, e.Elevation);
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is MapViewerEx)
                                {
                                    if (c2 != sender)
                                    {
                                        MapViewerEx vwr2 = (MapViewerEx)c2;
                                        if (vwr2.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr2.SetSurfaceGraphViewEx(e.DepthX, e.DepthY, e.Zoom, e.Rotation, e.Elevation);
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is MapViewerEx)
                                {
                                    if (c3 != sender)
                                    {
                                        MapViewerEx vwr3 = (MapViewerEx)c3;
                                        if (vwr3.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr3.SetSurfaceGraphViewEx(e.DepthX, e.DepthY, e.Zoom, e.Rotation, e.Elevation);
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void tabdet_onSplitterMoved(object sender, MapViewerEx.SplitterMovedEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers || m_appSettings.SynchronizeMapviewersDifferentMaps)
            {
                // andere cell geselecteerd, doe dat ook bij andere viewers met hetzelfde symbool (mapname)
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is MapViewerEx)
                        {
                            if (c != sender)
                            {
                                MapViewerEx vwr = (MapViewerEx)c;
                                if (vwr.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                {
                                    vwr.SetSplitter(e.Panel1height, e.Panel2height, e.Splitdistance, e.Panel1collapsed, e.Panel2collapsed);
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is MapViewerEx)
                                {
                                    if (c2 != sender)
                                    {
                                        MapViewerEx vwr2 = (MapViewerEx)c2;
                                        if (vwr2.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr2.SetSplitter(e.Panel1height, e.Panel2height, e.Splitdistance, e.Panel1collapsed, e.Panel2collapsed);
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is MapViewerEx)
                                {
                                    if (c3 != sender)
                                    {
                                        MapViewerEx vwr3 = (MapViewerEx)c3;
                                        if (vwr3.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr3.SetSplitter(e.Panel1height, e.Panel2height, e.Splitdistance, e.Panel1collapsed, e.Panel2collapsed);
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void tabdet_onSelectionChanged(object sender, MapViewerEx.CellSelectionChangedEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers || m_appSettings.SynchronizeMapviewersDifferentMaps)
            {
                // andere cell geselecteerd, doe dat ook bij andere viewers met hetzelfde symbool (mapname)
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is MapViewerEx)
                        {
                            if (c != sender)
                            {
                                MapViewerEx vwr = (MapViewerEx)c;
                                if (vwr.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                {
                                    vwr.SelectCell(e.Rowhandle, e.Colindex);
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is MapViewerEx)
                                {
                                    if (c2 != sender)
                                    {
                                        MapViewerEx vwr2 = (MapViewerEx)c2;
                                        if (vwr2.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr2.SelectCell(e.Rowhandle, e.Colindex);
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is MapViewerEx)
                                {
                                    if (c3 != sender)
                                    {
                                        MapViewerEx vwr3 = (MapViewerEx)c3;
                                        if (vwr3.Map_name == e.Mapname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                        {
                                            vwr3.SelectCell(e.Rowhandle, e.Colindex);
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetMapSliderPosition(string filename, string symbolname, int sliderposition)
        {
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                foreach (Control c in pnl.Controls)
                {
                    if (c is MapViewerEx)
                    {
                        MapViewerEx vwr = (MapViewerEx)c;
                        if (vwr.Map_name == symbolname)
                        {
                            vwr.SliderPosition = sliderposition;
                            vwr.Invalidate();
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is MapViewerEx)
                            {
                                MapViewerEx vwr2 = (MapViewerEx)c2;
                                if (vwr2.Map_name == symbolname)
                                {
                                    vwr2.SliderPosition = sliderposition;
                                    vwr2.Invalidate();
                                }
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                    {
                        DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                        foreach (Control c3 in cntr.Controls)
                        {
                            if (c3 is MapViewerEx)
                            {
                                MapViewerEx vwr3 = (MapViewerEx)c3;
                                if (vwr3.Map_name == symbolname)
                                {
                                    vwr3.SliderPosition = sliderposition;
                                    vwr3.Invalidate();
                                }
                            }
                        }
                    }
                }
            }

        }

        void tabdet_onSliderMove(object sender, MapViewerEx.SliderMoveEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers || m_appSettings.SynchronizeMapviewersDifferentMaps)
            {
                SetMapSliderPosition(e.Filename, e.SymbolName, e.SliderPosition);
            }
        }

        private void SetMapScale(string filename, string symbolname, int axismax, int lockmode)
        {
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                foreach (Control c in pnl.Controls)
                {
                    if (c is MapViewerEx)
                    {
                        MapViewerEx vwr = (MapViewerEx)c;
                        if (vwr.Map_name == symbolname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                        {
                            vwr.Max_y_axis_value = axismax;
                            //vwr.ReShowTable(false);
                            vwr.LockMode = lockmode;
                            vwr.Invalidate();
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is MapViewerEx)
                            {
                                MapViewerEx vwr2 = (MapViewerEx)c2;
                                if (vwr2.Map_name == symbolname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                {
                                    vwr2.Max_y_axis_value = axismax;
                                    //vwr2.ReShowTable(false);
                                    vwr2.LockMode = lockmode;
                                    vwr2.Invalidate();
                                }
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                    {
                        DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                        foreach (Control c3 in cntr.Controls)
                        {
                            if (c3 is MapViewerEx)
                            {
                                MapViewerEx vwr3 = (MapViewerEx)c3;
                                if (vwr3.Map_name == symbolname || m_appSettings.SynchronizeMapviewersDifferentMaps)
                                {

                                    vwr3.Max_y_axis_value = axismax;
                                    vwr3.LockMode = lockmode;
                                    //vwr3.ReShowTable(false);
                                    vwr3.Invalidate();
                                }
                            }
                        }
                    }
                }
            }

        }

        private int FindMaxTableValue(string symbolname, int orgvalue)
        {
            int retval = orgvalue;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                foreach (Control c in pnl.Controls)
                {
                    if (c is MapViewerEx)
                    {
                        MapViewerEx vwr = (MapViewerEx)c;
                        if (vwr.Map_name == symbolname)
                        {
                            if (vwr.MaxValueInTable > retval) retval = vwr.MaxValueInTable;
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is MapViewerEx)
                            {
                                MapViewerEx vwr2 = (MapViewerEx)c2;
                                if (vwr2.Map_name == symbolname)
                                {
                                    if (vwr2.MaxValueInTable > retval) retval = vwr2.MaxValueInTable;
                                }
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                    {
                        DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                        foreach (Control c3 in cntr.Controls)
                        {
                            if (c3 is MapViewerEx)
                            {
                                MapViewerEx vwr3 = (MapViewerEx)c3;
                                if (vwr3.Map_name == symbolname)
                                {
                                    if (vwr3.MaxValueInTable > retval) retval = vwr3.MaxValueInTable;
                                }
                            }
                        }
                    }
                }
            }
            return retval;
        }

        void tabdet_onAxisLock(object sender, MapViewerEx.AxisLockEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers || m_appSettings.SynchronizeMapviewersDifferentMaps)
            {
                int axismaxvalue = e.AxisMaxValue;
                if (e.LockMode == 1)
                {
                    axismaxvalue = FindMaxTableValue(e.SymbolName, axismaxvalue);
                }
                SetMapScale(e.Filename, e.SymbolName, axismaxvalue, e.LockMode);
            }
        }

        private void btnActivateLaunchControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Tools.Instance.m_currentfile != string.Empty)
            {
                if (File.Exists(Tools.Instance.m_currentfile))
                {
                    if (MapsWithNameMissing("Launch control map", Tools.Instance.m_symbols))
                    {
                        byte[] allBytes = File.ReadAllBytes(Tools.Instance.m_currentfile);
                        bool found = true;
                        int offset = 0;
                        while (found)
                        {
                            int LCAddress = Tools.Instance.findSequence(allBytes, offset, new byte[16] { 0xFF, 0xFF, 0x02, 0x00, 0x80, 0x00, 0x00, 0x0A, 0xFF, 0xFF, 0x02, 0x00, 0x00, 0x00, 0x70, 0x17 }, new byte[16] { 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1 });
                            if (LCAddress > 0)
                            {
                                //Console.WriteLine("Working on " + LCAddress.ToString("X8"));
                                btnActivateLaunchControl.Enabled = false;

                                byte[] saveByte = new byte[(0x0E * 2) + 2];
                                int i = 0;
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(0x0E), i++);
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(0), i++); // 1st value = 0 km/h
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(20), i++); // 2nd value = 6 km/h (6 / 0.000039 = 

                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(40), i++); // 

                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(60), i++); // 

                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(80), i++); // 

                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(100), i++); // 
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(120), i++); // 

                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(140), i++); // 
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(160), i++); // 
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(180), i++); // 
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(200), i++); // 
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(220), i++); // 
                                saveByte.SetValue(Convert.ToByte(0), i++);
                                saveByte.SetValue(Convert.ToByte(240), i++); // 
                                saveByte.SetValue(Convert.ToByte(1), i++);
                                saveByte.SetValue(Convert.ToByte(4), i++); // 

                                Tools.Instance.savedatatobinary(LCAddress + 2, saveByte.Length, saveByte, Tools.Instance.m_currentfile, false, Tools.Instance.m_currentFileType);
                                // fill the map with default values as well!
                                VerifyChecksum(Tools.Instance.m_currentfile, false, false);
                                
                                offset = LCAddress + 1;


                            }
                            else found = false;
                        }
                    }
                }
            }
            Application.DoEvents();
            //C2 02 00 xx xx xx xx xx EC 02 00 70 17*/
            if (!btnActivateLaunchControl.Enabled)
            {
                Tools.Instance.m_symbols = DetectMaps(Tools.Instance.m_currentfile, out Tools.Instance.codeBlockList, out Tools.Instance.AxisList, false, true);
                gridControl1.DataSource = null;
                Application.DoEvents();
                gridControl1.DataSource = Tools.Instance.m_symbols;
                Application.DoEvents();
                try
                {
                    gridViewSymbols.ExpandAllGroups();
                }
                catch (Exception)
                {

                }
                Application.DoEvents();

            }

        }

        private void btnEditEEProm_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // check size .. should be 4kb
                FileInfo fi = new FileInfo(ofd.FileName);
                if (fi.Length == 512)
                {
                    frmEEPromEditor editor = new frmEEPromEditor();
                    editor.LoadFile(ofd.FileName);
                    editor.ShowDialog();
                }
            }
        }

        private void gridViewSymbols_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (gridViewSymbols.FocusedColumn.Name == gcSymbolUserdescription.Name)
                {
                    SaveAdditionalSymbols();
                }
                else
                {
                    // start the selected row
                    try
                    {
                        int[] selectedrows = gridViewSymbols.GetSelectedRows();
                        int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                        if (grouplevel >= gridViewSymbols.GroupCount)
                        {
                            if (gridViewSymbols.GetFocusedRow() is SymbolHelper)
                            {
                                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetFocusedRow();
                                StartTableViewer(sh.Varname, sh.CodeBlock);
                                //StartTableViewer();
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }

            }
        }

        private void btnMergeFiles_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmBinmerger frmmerger = new frmBinmerger();
            frmmerger.ShowDialog();
        }

        private void btnSplitFiles_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (Tools.Instance.m_currentfile != "")
            {
                if (File.Exists(Tools.Instance.m_currentfile))
                {
                    string path = Path.GetDirectoryName(Tools.Instance.m_currentfile);
                    FileInfo fi = new FileInfo(Tools.Instance.m_currentfile);
                    FileStream fs = File.Create(path + "\\chip2.bin");
                    BinaryWriter bw = new BinaryWriter(fs);
                    FileStream fs2 = File.Create(path + "\\chip1.bin");
                    BinaryWriter bw2 = new BinaryWriter(fs2);
                    FileStream fsi1 = File.OpenRead(Tools.Instance.m_currentfile);
                    BinaryReader br1 = new BinaryReader(fsi1);
                    bool toggle = false;
                    for (int tel = 0; tel < fi.Length; tel++)
                    {
                        Byte ib1 = br1.ReadByte();
                        if (!toggle)
                        {
                            toggle = true;
                            bw.Write(ib1);
                        }
                        else
                        {
                            toggle = false;
                            bw2.Write(ib1);
                        }
                    }
                    bw.Close();
                    bw2.Close();
                    fs.Close();
                    fs2.Close();
                    fsi1.Close();
                    br1.Close();
                    MessageBox.Show("File split to chip1.bin and chip2.bin");
                }
            }
        }

        private void btnBuildLibrary_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmBrowseFiles browse = new frmBrowseFiles();
            browse.Show();
        }

        private void StartPDFFile(string file, string errormessage)
        {
            try
            {
                if (File.Exists(file))
                {
                    System.Diagnostics.Process.Start(file);
                }
                else
                {
                    MessageBox.Show(errormessage);
                }
            }
            catch (Exception E2)
            {
                Console.WriteLine(E2.Message);
            }
        }

        private void btnUserManual_ItemClick(object sender, ItemClickEventArgs e)
        {
            // start user manual PDF file
            StartPDFFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "EDC15PSuite manual.pdf"), "EDC15P user manual could not be found or opened!");
            
        }
        private void btnEDC15PDocumentation_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartPDFFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "VAG EDC15P.pdf"), "EDC15P documentation could not be found or opened!");
        }

        

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportDescriptorFile(ImportFileType.XML);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportDescriptorFile(ImportFileType.A2L);
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportDescriptorFile(ImportFileType.CSV);
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportDescriptorFile(ImportFileType.AS2);
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportDescriptorFile(ImportFileType.Damos);
        }

        private void ImportDescriptorFile(ImportFileType importFileType)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AS2 documents|*.as2";
            if (importFileType == ImportFileType.A2L) ofd.Filter = "A2L documents|*.a2l";
            else if (importFileType == ImportFileType.CSV) ofd.Filter = "CSV documents|*.csv";
            else if (importFileType == ImportFileType.Damos) ofd.Filter = "Damos documents|*.dam";
            else if (importFileType == ImportFileType.XML) ofd.Filter = "XML documents|*.xml";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalSymbols(ofd.FileName, importFileType, Tools.Instance.m_symbols, false);
                gridControl1.DataSource = Tools.Instance.m_symbols;
                gridControl1.RefreshDataSource();
                SaveAdditionalSymbols();
            }
        }




        private void SaveAdditionalSymbols()
        {
            System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(Tools.Instance.m_currentfile));
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");
            byte[] allBytes = File.ReadAllBytes(Tools.Instance.m_currentfile);
            string boschpartNumber = Tools.Instance.ExtractBoschPartnumber(allBytes);
            partNumberConverter pnc = new partNumberConverter();
            ECUInfo info = pnc.ConvertPartnumber(boschpartNumber,allBytes.Length);
            string checkstring = boschpartNumber + "_" + info.SoftwareID;

            string xmlfilename = Tools.Instance.GetWorkingDirectory() + "\\repository\\" + Path.GetFileNameWithoutExtension(Tools.Instance.m_currentfile) + File.GetCreationTime(Tools.Instance.m_currentfile).ToString("yyyyMMddHHmmss") + checkstring + ".xml";
            if (!Directory.Exists(Tools.Instance.GetWorkingDirectory() + "\\repository"))
            {
                Directory.CreateDirectory(Tools.Instance.GetWorkingDirectory() + "\\repository");
            }
            if (File.Exists(xmlfilename))
            {
                File.Delete(xmlfilename);
            }
            foreach (SymbolHelper sh in Tools.Instance.m_symbols)
            {
                if (sh.Userdescription != "")
                {
                    dt.Rows.Add(sh.Varname, sh.Symbol_number, sh.Flash_start_address, sh.Userdescription);
                }
            }
            dt.WriteXml(xmlfilename);
        }

        private void TryToLoadAdditionalSymbols(string filename, ImportFileType importFileType, SymbolCollection symbolCollection, bool fromRepository)
        {
            if (importFileType == ImportFileType.XML)
            {
                ImportXMLFile(filename, symbolCollection, fromRepository);
            }
            else if (importFileType == ImportFileType.AS2)
            {
                TryToLoadAdditionalAS2Symbols(filename, symbolCollection);
            }
            else if (importFileType == ImportFileType.CSV)
            {
                TryToLoadAdditionalCSVSymbols(filename, symbolCollection);
            }
        }

        private void TryToLoadAdditionalCSVSymbols(string filename, SymbolCollection coll2load)
        {
            // convert to CSV file format
            // ADDRESS;NAME;;;
            try
            {
                SymbolTranslator st = new SymbolTranslator();
                char[] sep = new char[1];
                sep.SetValue(';', 0);
                string[] fileContent = File.ReadAllLines(filename);
                foreach (string line in fileContent)
                {
                    string[] values = line.Split(sep);
                    try
                    {
                        string varname = (string)values.GetValue(1);
                        int flashaddress = Convert.ToInt32(values.GetValue(0));
                        foreach (SymbolHelper sh in coll2load)
                        {
                            if (sh.Flash_start_address == flashaddress)
                            {
                                sh.Userdescription = varname;
                            }
                        }
                    }
                    catch (Exception lineE)
                    {
                        Console.WriteLine("Failed to import a symbol from CSV file " + line + ": " + lineE.Message);
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to import additional CSV symbols: " + E.Message);
            }
        }

        private void TryToLoadAdditionalAS2Symbols(string filename, SymbolCollection coll2load)
        {
            // convert to AS2 file format

            try
            {
                SymbolTranslator st = new SymbolTranslator();
                char[] sep = new char[1];
                sep.SetValue(';', 0);
                string[] fileContent = File.ReadAllLines(filename);
                int symbolnumber = 0;
                foreach (string line in fileContent)
                {
                    if (line.StartsWith("*"))
                    {
                        symbolnumber++;
                        string[] values = line.Split(sep);
                        try
                        {

                            string varname = (string)values.GetValue(0);
                            varname = varname.Substring(1);
                            int idxSymTab = 0;
                            foreach (SymbolHelper sh in coll2load)
                            {
                                if (sh.Length > 0) idxSymTab++;
                                if (idxSymTab == symbolnumber)
                                {
                                    sh.Userdescription = varname;
                                    break;
                                }
                            }
                        }
                        catch (Exception lineE)
                        {
                            Console.WriteLine("Failed to import a symbol from AS2 file " + line + ": " + lineE.Message);
                        }

                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to import additional AS2 symbols: " + E.Message);
            }
        }

        private bool ImportXMLFile(string filename, SymbolCollection coll2load, bool ImportFromRepository)
        {
            bool retval = false;
            SymbolTranslator st = new SymbolTranslator();
            System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(filename));
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");
            if (ImportFromRepository)
            {
                byte[] allBytes = File.ReadAllBytes(filename);
                string boschpartNumber = Tools.Instance.ExtractBoschPartnumber(allBytes);
                partNumberConverter pnc = new partNumberConverter();
                ECUInfo info = pnc.ConvertPartnumber(boschpartNumber, allBytes.Length);
                string checkstring = boschpartNumber + "_" + info.SoftwareID;

                string xmlfilename = Tools.Instance.GetWorkingDirectory() + "\\repository\\" + Path.GetFileNameWithoutExtension(filename) + File.GetCreationTime(filename).ToString("yyyyMMddHHmmss") + checkstring + ".xml";
                if (!Directory.Exists(Tools.Instance.GetWorkingDirectory() + "\\repository"))
                {
                    Directory.CreateDirectory(Tools.Instance.GetWorkingDirectory() + "\\repository");
                }
                if (File.Exists(xmlfilename))
                {
                    dt.ReadXml(xmlfilename);
                    retval = true;
                }
            }
            else
            {
                string binname = GetFileDescriptionFromFile(filename);
                if (binname != string.Empty)
                {
                    dt = new System.Data.DataTable(binname);
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    if (File.Exists(filename))
                    {
                        dt.ReadXml(filename);
                        retval = true;
                    }
                }
            }
            foreach (SymbolHelper sh in coll2load)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        //if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                        {
                            if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                            {
                                sh.Userdescription = dr["DESCRIPTION"].ToString();
                                break;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
            }
            return retval;
        }

        private string GetFileDescriptionFromFile(string file)
        {
            string retval = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    sr.ReadLine();
                    sr.ReadLine();
                    string name = sr.ReadLine();
                    name = name.Trim();
                    name = name.Replace("<", "");
                    name = name.Replace(">", "");
                    //name = name.Replace("x0020", " ");
                    name = name.Replace("_x0020_", " ");
                    for (int i = 0; i <= 9; i++)
                    {
                        name = name.Replace("_x003" + i.ToString() + "_", i.ToString());
                    }
                    retval = name;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private void gridViewSymbols_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            if (e.Column.Name == gcSymbolUserdescription.Name)
            {
                SaveAdditionalSymbols();
            }
        }

        private void dockManager1_LayoutUpgrade(object sender, DevExpress.Utils.LayoutUpgadeEventArgs e)
        {

        }

        private void btnActivateSmokeLimiters_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Tools.Instance.m_currentfile != string.Empty)
            {
                if (File.Exists(Tools.Instance.m_currentfile))
                {
                    btnActivateSmokeLimiters.Enabled = false;
                    // find the smoke limiter control map (selector)
                    foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                    {
                        if (sh.Varname.StartsWith("Smoke limiter"))
                        {
                            byte[] mapdata = new byte[sh.Length];
                            mapdata.Initialize();
                            mapdata = Tools.Instance.readdatafromfile(Tools.Instance.m_currentfile, (int)sh.Flash_start_address, sh.Length, Tools.Instance.m_currentFileType);

                            int selectorAddress = sh.MapSelector.StartAddress;
                            if (selectorAddress > 0)
                            {
                                byte[] mapIndexes = new byte[sh.MapSelector.MapIndexes.Length * 2];
                                int bIdx = 0;
                                for (int i = 0; i < sh.MapSelector.MapIndexes.Length; i++)
                                {
                                    mapIndexes[bIdx++] = Convert.ToByte(i);
                                    mapIndexes[bIdx++] = 0;
                                }
                                Tools.Instance.savedatatobinary(selectorAddress + mapIndexes.Length, mapIndexes.Length, mapIndexes, Tools.Instance.m_currentfile, false, Tools.Instance.m_currentFileType);
                            }
                            for (int i = 1; i < sh.MapSelector.MapIndexes.Length; i++)
                            {
                                // save the map data (copy)
                                int saveAddress = (int)sh.Flash_start_address + i * sh.Length;
                                Tools.Instance.savedatatobinary(saveAddress, sh.Length, mapdata, Tools.Instance.m_currentfile, false, Tools.Instance.m_currentFileType);
                            }
                        }
                    }

                    VerifyChecksum(Tools.Instance.m_currentfile, false, false);
                }
            }
            Application.DoEvents();

            if (!btnActivateSmokeLimiters.Enabled)
            {
                Tools.Instance.m_symbols = DetectMaps(Tools.Instance.m_currentfile, out Tools.Instance.codeBlockList, out Tools.Instance.AxisList, false, true);
                gridControl1.DataSource = null;
                Application.DoEvents();
                gridControl1.DataSource = Tools.Instance.m_symbols;
                Application.DoEvents();
                try
                {

                    gridViewSymbols.ExpandAllGroups();
                }
                catch (Exception)
                {

                }
                Application.DoEvents();

            }
        }

        private void ImportFileInExcelFormat()
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Multiselect = false;
            
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string mapname = string.Empty;
                    string realmapname = string.Empty;
                    int tildeindex = openFileDialog2.FileName.LastIndexOf("~");
                    bool symbolfound = false;
                    if (tildeindex > 0)
                    {
                        tildeindex++;
                        mapname = openFileDialog2.FileName.Substring(tildeindex, openFileDialog2.FileName.Length - tildeindex);
                        mapname = mapname.Replace(".xls", "");
                        mapname = mapname.Replace(".XLS", "");
                        mapname = mapname.Replace(".Xls", "");
                       
                        // look if it is a valid symbolname
                        foreach (SymbolHelper sh in Tools.Instance.m_symbols)
                        {
                            if (sh.Varname.Replace(",", "").Replace("[","").Replace("]","") == mapname || sh.Userdescription.Replace(",", "") == mapname)
                            {
                                symbolfound = true;
                                realmapname = sh.Varname;
                                if (MessageBox.Show("Found valid symbol for import: " + sh.Varname + ". Are you sure you want to overwrite the map in the binary?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    // ok, overwrite info in binary
                                }
                                else
                                {
                                    mapname = string.Empty; // do nothing
                                    realmapname = string.Empty;
                                }
                            }
                        }
                        if (!symbolfound)
                        {
                            // ask user for symbol designation
                            frmSymbolSelect frmselect = new frmSymbolSelect(Tools.Instance.m_symbols);
                            if (frmselect.ShowDialog() == DialogResult.OK)
                            {
                                mapname = frmselect.SelectedSymbol;
                                realmapname = frmselect.SelectedSymbol;
                            }
                        }

                    }
                    else
                    {
                        // ask user for symbol designation
                        frmSymbolSelect frmselect = new frmSymbolSelect(Tools.Instance.m_symbols);
                        if (frmselect.ShowDialog() == DialogResult.OK)
                        {
                            mapname = frmselect.SelectedSymbol;
                            realmapname = frmselect.SelectedSymbol;
                        }

                    }
                    if (realmapname != string.Empty)
                    {
                        ImportExcelSymbol(realmapname, openFileDialog2.FileName);
                    }

                }
                catch (Exception E)
                {
                    frmInfoBox info = new frmInfoBox("Failed to import map from excel: " + E.Message);
                }
            }
        }

        private void ImportExcelSymbol(string symbolname, string filename)
        {
            ExcelInterface excelInterface = new ExcelInterface();
            bool issixteenbit = true;
            System.Data.DataTable dt = excelInterface.getDataFromXLS(filename);
            int symbollength = GetSymbolLength(Tools.Instance.m_symbols, symbolname);
            int datalength = symbollength;
            if (issixteenbit) datalength /= 2;
            int[] buffer = new int[datalength];
            int bcount = 0;
            //            for (int rtel = 1; rtel < dt.Rows.Count; rtel++)
            for (int rtel = dt.Rows.Count; rtel >= 1; rtel--)
            {
                try
                {
                    int idx = 0;
                    foreach (object o in dt.Rows[rtel].ItemArray)
                    {
                        if (idx > 0)
                        {
                            if (o != null)
                            {
                                if (o != DBNull.Value)
                                {
                                    if (bcount < buffer.Length)
                                    {
                                        buffer.SetValue(Convert.ToInt32(o), bcount++);
                                    }
                                    else
                                    {
                                        frmInfoBox info = new frmInfoBox("Too much information in file, abort");
                                        return;
                                    }
                                }
                            }
                        }
                        idx++;
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine("ImportExcelSymbol: " + E.Message);
                }

            }
            if (bcount >= datalength)
            {
                byte[] data = new byte[symbollength];
                int cellcount = 0;
                if (issixteenbit)
                {
                    for (int dcnt = 0; dcnt < buffer.Length; dcnt++)
                    {
                        string bstr1 = "0";
                        string bstr2 = "0";
                        int cellvalue = Convert.ToInt32(buffer.GetValue(dcnt));
                        string svalue = cellvalue.ToString("X4");

                        bstr1 = svalue.Substring(svalue.Length - 4, 2);
                        bstr2 = svalue.Substring(svalue.Length - 2, 2);
                        data.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                        data.SetValue(Convert.ToByte(bstr2, 16), cellcount++);
                    }
                }
                else
                {
                    for (int dcnt = 0; dcnt < buffer.Length; dcnt++)
                    {
                        int cellvalue = Convert.ToInt32(buffer.GetValue(dcnt));
                        data.SetValue(Convert.ToByte(cellvalue.ToString()), cellcount++);
                    }
                }
                Tools.Instance.savedatatobinary((int)GetSymbolAddress(Tools.Instance.m_symbols, symbolname), symbollength, data, Tools.Instance.m_currentfile, true, Tools.Instance.m_currentFileType);
                Tools.Instance.UpdateChecksum(Tools.Instance.m_currentfile, false);
            }


        }

        private void StartExcelExport()
        {
            ExcelInterface excelInterface = new ExcelInterface();
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    //DataRowView dr = (DataRowView)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    //frmTableDetail tabdet = new frmTableDetail();
                    string Map_name = sh.Varname;
                    if ((Map_name.StartsWith("2D") || Map_name.StartsWith("3D")) && sh.Userdescription != "") Map_name = sh.Userdescription;
                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, Map_name, out columns, out rows);

                    int address = (int)sh.Flash_start_address;
                    if (address != 0)
                    {
                        int length = sh.Length;

                        byte[] mapdata = Tools.Instance.readdatafromfile(Tools.Instance.m_currentfile, address, length, Tools.Instance.m_currentFileType);
                        int[] xaxis = GetXaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, Map_name);
                        int[] yaxis = GetYaxisValues(Tools.Instance.m_currentfile, Tools.Instance.m_symbols, Map_name);
                        Map_name = Map_name.Replace(",", "");
                        Map_name = Map_name.Replace("[", "");
                        Map_name = Map_name.Replace("]", "");

                        excelInterface.ExportToExcel(Map_name, address, length, mapdata, columns, rows, true, xaxis, yaxis, m_appSettings.ShowTablesUpsideDown, sh.X_axis_descr, sh.Y_axis_descr, sh.Z_axis_descr);
                    }
                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No symbol selected in the primary symbol list");
            }
        }

        private void btnExportToExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartExcelExport();
        }

        private void btnExcelImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportFileInExcelFormat();
        }

        private void exportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartExcelExport();
        }

        private void btnIQByMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IQ by MAP", 2);
        }

        private void btnIQByMAF_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IQ by MAF", 2);
        }

        private void btnSOILimiter_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("SOI limiter", 2);
            
        }

        private void btnStartOfInjection_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Start of injection", 2);
        }

        private void btnInjectorDuration_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Injector duration", 2);
        }

        private void btnStartIQ_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("Start IQ", 2);
        }

    }
}
