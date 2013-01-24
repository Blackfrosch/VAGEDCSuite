using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace VAGSuite
{
    public partial class frmBrowseFiles : DevExpress.XtraEditors.XtraForm
    {
        public frmBrowseFiles()
        {
            InitializeComponent();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            try
            {
                fbd.SelectedPath = buttonEdit1.Text;

            }
            catch (Exception)
            {

            }
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                buttonEdit1.Text = fbd.SelectedPath;
                Application.DoEvents();
                LoadFiles();
            }
        }

        private bool cancelLoad = false;
        private bool loading = false;

        private void LoadFiles()
        {
            loading = true;
            simpleButton1.Text = "Cancel";
            simpleButton2.Enabled = false;
            SetProgressVisibility(true);
            SetScanProgress("scanning...", 0);
            Application.DoEvents();
            ctrlAirmassResult airmassResult = new ctrlAirmassResult();
            partNumberConverter pnc = new partNumberConverter();
            List<ScannedFile> detectedFiles = new List<ScannedFile>();
            int fileCounter = 0;
            if (Directory.Exists(buttonEdit1.Text))
            {
                string[] files = Directory.GetFiles(buttonEdit1.Text, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    fileCounter++;
                    int percentage = (fileCounter * 100) / files.Length;
                    SetScanProgress("importing...", percentage);
                    
                    if (cancelLoad)
                    {
                        cancelLoad = false;
                        break;
                    }
                    FileInfo fi = new FileInfo(file);
                    this.Text = "Library builder - " + Path.GetFileNameWithoutExtension(file);
                    Application.DoEvents();
                    if (IsValidLength(fi.Length))
                    {
                        IEDCFileParser parser = Tools.Instance.GetParserForFile(file, false);
                        byte[] allBytes = File.ReadAllBytes(file);
                        string boschPartnumber = parser.ExtractBoschPartnumber(allBytes);
                        List<CodeBlock> newCodeBlocks = new List<CodeBlock>();
                        List<AxisHelper> newAxisHelpers = new List<AxisHelper>();
                        if (boschPartnumber != string.Empty)
                        {
                            //string additionalInfo = parser.ExtractInfo(allBytes);
                            SymbolCollection sc = parser.parseFile(file, out newCodeBlocks, out newAxisHelpers);
                            ECUInfo info = pnc.ConvertPartnumber(boschPartnumber, allBytes.Length);

                            ScannedFile newFile = new ScannedFile();
                            newFile.CarMake = info.CarMake;
                            newFile.CarType = info.CarType;
                            newFile.EcuType = info.EcuType;
                            newFile.EngineType = info.EngineType;
                            newFile.FuellingType = info.FuellingType;
                            newFile.FuelType = info.FuelType;
                            newFile.HP = info.HP;
                            newFile.PartNumber = info.PartNumber;
                            newFile.SoftwareID = info.SoftwareID;
                            newFile.TQ = info.TQ;

                            if (info.EcuType.Contains("EDC15P-6"))
                            {
                                newFile.Filetype = EDCFileType.EDC15P6;
                            }
                            else if (info.EcuType.Contains("EDC15P"))
                            {
                                newFile.Filetype = EDCFileType.EDC15P;
                            }
                            else if (info.EcuType.Contains("EDC15M"))
                            {
                                newFile.Filetype = EDCFileType.EDC15M;
                            }
                            else if (info.EcuType.Contains("EDC15V-5."))
                            {
                                newFile.Filetype = EDCFileType.MSA15;
                            }
                            else if (info.EcuType.Contains("EDC15V"))
                            {
                                newFile.Filetype = EDCFileType.EDC15V;
                            }
                            else if (info.EcuType.Contains("EDC15C"))
                            {
                                newFile.Filetype = EDCFileType.EDC15C;
                            }
                            else if (info.EcuType.Contains("EDC16"))
                            {
                                newFile.Filetype = EDCFileType.EDC16;
                            }
                            else if (info.EcuType.Contains("EDC17"))
                            {
                                newFile.Filetype = EDCFileType.EDC17;
                            }
                            else if (info.EcuType.Contains("MSA15"))
                            {
                                newFile.Filetype = EDCFileType.MSA15;
                            }
                            else if (info.EcuType.Contains("MSA12"))
                            {
                                newFile.Filetype = EDCFileType.MSA12;
                            }
                            else if (info.EcuType.Contains("MSA11"))
                            {
                                newFile.Filetype = EDCFileType.MSA11;
                            }
                            else if (info.EcuType.Contains("MSA6"))
                            {
                                newFile.Filetype = EDCFileType.MSA6;
                            }

                            else if (boschPartnumber != string.Empty)
                            {
                                if (fi.Length == 1024 * 1024 * 2)
                                {
                                    newFile.Filetype = EDCFileType.EDC17;
                                }
                                else if(boschPartnumber.StartsWith("EDC17"))
                                {
                                    newFile.Filetype = EDCFileType.EDC17;
                                }
                                else
                                {
                                    newFile.Filetype = EDCFileType.EDC15V;
                                }
                            }
                            else
                            {
                                newFile.Filetype = EDCFileType.EDC16; // default to EDC16???
                            }
                            newFile.Filename = file;
                            newFile.Filesize = (int)fi.Length;

                            ChecksumResultDetails crd = Tools.Instance.UpdateChecksum(file, true);
                            
                            string chkType = string.Empty;
                            if (crd.TypeResult == ChecksumType.VAG_EDC15P_V41) chkType = "VAG EDC15P V4.1";
                            else if (crd.TypeResult == ChecksumType.VAG_EDC15P_V41V2) chkType = "VAG EDC15P V4.1v2";
                            else if (crd.TypeResult == ChecksumType.VAG_EDC15P_V41_2002) chkType = "VAG EDC15P V4.1 2002+";
                            newFile.ChecksumType = chkType;
                            newFile.ChecksumResult = crd.CalculationResult.ToString();
                            newFile.NumberChecksums = crd.NumberChecksumsTotal;
                            newFile.NumberChecksumsFail = crd.NumberChecksumsFail;
                            newFile.NumberChecksumsOk = crd.NumberChecksumsOk;

                            newFile.NumberMapsDetected = sc.Count;
                            string _message = string.Empty;
                            newFile.MapsOk = CheckMajorMapsPresent(sc,  newFile.Filetype, out _message);
                            newFile.Messages = _message;
                            foreach (SymbolHelper sh in sc)
                            {
                                if (!sh.Varname.StartsWith("3D") && !sh.Varname.StartsWith("2D"))
                                {
                                    newFile.NumberMapsRecognized++;
                                }
                            }

                            try
                            {
                                airmassResult.Currentfile = file;
                                airmassResult.Symbols = sc;
                                airmassResult.Currentfile_size = (int)fi.Length;
                                string additionalInfo = parser.ExtractInfo(allBytes);
                                airmassResult.NumberCylinders = pnc.GetNumberOfCylinders(info.EngineType, additionalInfo);
                                airmassResult.ECUType = info.EcuType;
                                PerformanceResults pr = airmassResult.Calculate(file, sc);
                                newFile.RealHP = pr.Horsepower;
                                newFile.RealTQ = pr.Torque;
                            }
                            catch (Exception)
                            {

                            }

                            detectedFiles.Add(newFile);

                        }
                        else if (file.ToUpper().EndsWith(".BIN") || file.ToUpper().EndsWith(".ORI"))
                        {
                            Console.WriteLine("Missed " + file);
                            // add it as well
                            if (checkEdit1.Checked)
                            {
                                ScannedFile newFile = new ScannedFile();
                                newFile.CarMake = "";
                                newFile.CarType = "";
                                newFile.EcuType = "Unknown";
                                newFile.EngineType = "";
                                newFile.FuellingType = "";
                                newFile.FuelType = "";
                                newFile.HP = 0;
                                newFile.PartNumber = "";
                                newFile.SoftwareID = "";
                                newFile.TQ = 0;
                                newFile.Filetype = EDCFileType.Unknown;
                                newFile.Filename = file;
                                newFile.Filesize = (int)fi.Length;
                                newFile.ChecksumType = "";
                                newFile.ChecksumResult = "";
                                newFile.NumberChecksums = 0;
                                newFile.NumberChecksumsFail = 0;
                                newFile.NumberChecksumsOk = 0;
                                newFile.NumberMapsDetected = 0;
                                newFile.MapsOk = false;
                                newFile.Messages = "";
                                newFile.NumberMapsRecognized = 0;
                                detectedFiles.Add(newFile);
                            }
                        }

                    }
                }
                gridControl1.DataSource = detectedFiles;
            }
            loading = false;
            SetScanProgress("done", 100);
            SetProgressVisibility(false);
            this.Text = "Library builder";
            simpleButton1.Text = "Close";
            simpleButton2.Enabled = true;
            Application.DoEvents();
        }

        private bool CheckMajorMapsPresent(SymbolCollection newSymbols, EDCFileType type, out string _message)
        {
            _message = string.Empty;
            if (type == EDCFileType.EDC15C || type == EDCFileType.EDC15M || type == EDCFileType.EDC16 || type == EDCFileType.EDC17) return false; ;

            if (type == EDCFileType.EDC15P || type == EDCFileType.EDC15P6)
            {
                if (MapsWithNameMissing("Injector duration", newSymbols)) _message += "Injector duration maps missing" + Environment.NewLine;
                if (MapsWithNameMissing("Inverse driver wish", newSymbols)) _message += "Inverse driver wish map missing" + Environment.NewLine;
            }
            if (type == EDCFileType.EDC15P || type == EDCFileType.EDC15P6 || type == EDCFileType.EDC15V)
            {
                if (MapsWithNameMissing("MAF correction", newSymbols)) _message += "MAF correction map missing" + Environment.NewLine;
                if (MapsWithNameMissing("MAF linearization", newSymbols)) _message += "MAF linearization map missing" + Environment.NewLine;
                if (MapsWithNameMissing("MAP linearization", newSymbols)) _message += "MAP linearization map missing" + Environment.NewLine;
                if (MapsWithNameMissing("SOI limiter", newSymbols)) _message += "SOI limiter missing" + Environment.NewLine;
            }
            if (MapsWithNameMissing("Idle RPM", newSymbols)) _message += "Idle RPM map missing" + Environment.NewLine;
            if (MapsWithNameMissing("EGR", newSymbols)) _message += "EGR maps missing" + Environment.NewLine;
            if (MapsWithNameMissing("SVBL", newSymbols)) _message += "SVBL missing" + Environment.NewLine;
            if (MapsWithNameMissing("Torque limiter", newSymbols)) _message += "Torque limiter missing" + Environment.NewLine;
            if (MapsWithNameMissing("Smoke limiter", newSymbols)) _message += "Smoke limiter missing" + Environment.NewLine;
            //if (MapsWithNameMissing("IQ by MAF limiter", newSymbols)) _message += "IQ by MAF limiter missing" + Environment.NewLine;
            if (MapsWithNameMissing("Start of injection", newSymbols)) _message += "Start of injection maps missing" + Environment.NewLine;
            if (MapsWithNameMissing("N75 duty cycle", newSymbols)) _message += "N75 duty cycle map missing" + Environment.NewLine;
            if (MapsWithNameMissing("Boost target map", newSymbols)) _message += "Boost target map missing" + Environment.NewLine;
            if (MapsWithNameMissing("Driver wish", newSymbols)) _message += "Driver wish map missing" + Environment.NewLine;
            if (MapsWithNameMissing("Boost limit map", newSymbols)) _message += "Boost limit map missing" + Environment.NewLine;

            if (_message == "") return true;
            return false;
        }

        private bool MapsWithNameMissing(string varName, SymbolCollection newSymbols)
        {
            foreach (SymbolHelper sh in newSymbols)
            {
                if (sh.Varname.StartsWith(varName)) return false;
            }
            return true;
        }


        private bool IsValidLength(long len)
        {
            if (len == 0x8000) return true;
            else if (len == 0x10000) return true;
            else if (len == 0x20000) return true;
            else if (len == 0x40000) return true;
            else if (len == 0x80000) return true;
            else if (len == 0x100000) return true;
            else if (len == 0x200000) return true;
            return false;
        }

        private void buttonEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!loading)
                {
                    LoadFiles();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (loading) cancelLoad = true;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (loading) cancelLoad = true;
            else this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel worksheets|*.xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    gridControl1.ExportToXlsx(sfd.FileName);
                }
                catch (Exception)
                {

                }
            }
        }

        private void SetProgressVisibility(bool visible)
        {
            labelControl2.Visible = visible;
            progressBarControl1.Visible = visible;
            Application.DoEvents();
        }

        private void SetScanProgress(string text, int percentage)
        {
            try
            {
                bool _update = false;
                if (labelControl2.Text != text)
                {
                    labelControl2.Text = text;
                    _update = true;
                }
                if (Convert.ToInt32(progressBarControl1.EditValue) != percentage)
                {
                    progressBarControl1.EditValue = percentage;
                    _update = true;
                }
                if (_update) Application.DoEvents();
            }
            catch (Exception)
            {

            }

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                
                if (gridControl1.DataSource != null)
                {
                    if (gridControl1.DataSource is List<ScannedFile>)
                    {
                        List<ScannedFile> detectedFiles = (List<ScannedFile>)gridControl1.DataSource;

                        foreach (ScannedFile sf in detectedFiles)
                        {
                            string outputFolder = fbd.SelectedPath;            
                            if (sf.Filetype.ToString() != string.Empty)
                            {
                                outputFolder = Path.Combine(outputFolder, sf.Filetype.ToString());
                                if (!Directory.Exists(outputFolder))
                                {
                                    Directory.CreateDirectory(outputFolder);
                                }
                            }
                            if (sf.CarMake != string.Empty)
                            {
                                outputFolder = Path.Combine(outputFolder, sf.CarMake);
                                if (!Directory.Exists(outputFolder))
                                {
                                    Directory.CreateDirectory(outputFolder);
                                }
                            }
                            if (sf.CarType != string.Empty)
                            {
                                outputFolder = Path.Combine(outputFolder, sf.CarType);
                                if (!Directory.Exists(outputFolder))
                                {
                                    Directory.CreateDirectory(outputFolder);
                                }
                            }
                            string filename = sf.PartNumber;
                            if (filename == string.Empty) filename = Path.GetFileName(sf.Filename);
                            if (sf.SoftwareID != string.Empty) filename += "_" + sf.SoftwareID;
                            if (sf.EngineType != string.Empty) filename += "_" + sf.EngineType;
                            if (sf.RealHP != 0) filename += "_" + sf.RealHP.ToString() +"hp";
                            if (sf.RealTQ != 0) filename += "_" + sf.RealTQ.ToString() + "Nm";
                            if (sf.NumberChecksumsFail > 0)
                            {
                                filename += "_CHKFAIL";
                            }
                            //if (sf.NumberChecksumsFail == 0)
                            {
                                // only valid checksummed files to the output
                                try
                                {
                                    File.Copy(sf.Filename, Path.Combine(outputFolder, filename), false);
                                }
                                catch (Exception)
                                {

                                }
                            }


                        }
                    }
                }
            }
        }
    }
}