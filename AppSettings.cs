using System;
using System.Collections.Generic;
using System.Text;
//using System.Security.Permissions;
using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
//using System.Data.Odbc;
using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace VAGSuite
{
    public class AppSettings
    {
        private bool m_ShowTablesUpsideDown = true;

        public bool ShowTablesUpsideDown
        {
            get { return m_ShowTablesUpsideDown; }
            set
            {
                m_ShowTablesUpsideDown = value;
                SaveRegistrySetting("ShowTablesUpsideDown", m_ShowTablesUpsideDown);
            }
        }


        private bool m_RequestProjectNotes = false;

        public bool RequestProjectNotes
        {
            get { return m_RequestProjectNotes; }
            set
            {
                m_RequestProjectNotes = value;
                SaveRegistrySetting("RequestProjectNotes", m_RequestProjectNotes);
            }
        }

        private string m_ProjectFolder = Path.Combine(Tools.Instance.GetWorkingDirectory(), "Projects");

        public string ProjectFolder
        {
            get { return m_ProjectFolder; }
            set
            {
                m_ProjectFolder = value;
                SaveRegistrySetting("ProjectFolder", m_ProjectFolder);
            }
        }

        private int m_SymbolDockWidth = 0;

        public int SymbolDockWidth
        {
            get { return m_SymbolDockWidth; }
            set
            {
                m_SymbolDockWidth = value;
                SaveRegistrySetting("SymbolDockWidth", m_SymbolDockWidth);
            }
        }

        private int m_LastOpenedType = 0; // 0 = file, 1 = project

        public int LastOpenedType
        {
            get { return m_LastOpenedType; }
            set
            {
                m_LastOpenedType = value;
                SaveRegistrySetting("LastOpenedType", m_LastOpenedType);
            }
        }

        private string m_lastprojectname = "";
        public string Lastprojectname
        {
            get { return m_lastprojectname; }
            set
            {
                if (m_lastprojectname != value)
                {
                    m_lastprojectname = value;
                    SaveRegistrySetting("LastProjectname", m_lastprojectname);
                }
            }
        }

        private bool _codeBlockSyncActive = false;

        public bool CodeBlockSyncActive
        {
            get { return _codeBlockSyncActive; }
            set
            {
                _codeBlockSyncActive = value;
                SaveRegistrySetting("CodeBlockSyncActive", _codeBlockSyncActive);
            }
        }

        

        private string m_skinname = string.Empty;

        public string Skinname
        {
            get { return m_skinname; }
            set
            {
                m_skinname = value;
                SaveRegistrySetting("Skinname", m_skinname);
            }
        }

       

        private bool m_SynchronizeMapviewers = true;

        public bool SynchronizeMapviewers
        {
            get { return m_SynchronizeMapviewers; }
            set
            {
                m_SynchronizeMapviewers = value;
                SaveRegistrySetting("SynchronizeMapviewers", m_SynchronizeMapviewers);
            }
        }

        private bool m_SynchronizeMapviewersDifferentMaps = false;

        public bool SynchronizeMapviewersDifferentMaps
        {
            get { return m_SynchronizeMapviewersDifferentMaps; }
            set
            {
                m_SynchronizeMapviewersDifferentMaps = value;
                SaveRegistrySetting("SynchronizeMapviewersDifferentMaps", m_SynchronizeMapviewersDifferentMaps);
            }
        }

        private bool m_AutoLoadLastFile = true;

        public bool AutoLoadLastFile
        {
            get { return m_AutoLoadLastFile; }
            set
            {
                m_AutoLoadLastFile = value;
                SaveRegistrySetting("AutoLoadLastFile", m_AutoLoadLastFile);
            }
        }

        

        private ViewType m_DefaultViewType = ViewType.Easy;

        public ViewType DefaultViewType
        {
            get { return m_DefaultViewType; }
            set
            {
                m_DefaultViewType = value;
                SaveRegistrySetting("DefaultViewType", (int)m_DefaultViewType);
            }
        }


        private ViewSize m_DefaultViewSize = ViewSize.NormalView;

        public ViewSize DefaultViewSize
        {
            get { return m_DefaultViewSize; }
            set
            {
                m_DefaultViewSize = value;
                SaveRegistrySetting("DefaultViewSize", (int)m_DefaultViewSize);
            }
        }

        private bool m_NewPanelsFloating = false;

        public bool NewPanelsFloating
        {
            get { return m_NewPanelsFloating; }
            set
            {
                m_NewPanelsFloating = value;
                SaveRegistrySetting("NewPanelsFloating", m_NewPanelsFloating);
            }
        }
        


        private bool m_DisableMapviewerColors = false;

        public bool DisableMapviewerColors
        {
            get { return m_DisableMapviewerColors; }
            set
            {
                m_DisableMapviewerColors = value;
                SaveRegistrySetting("DisableMapviewerColors", m_DisableMapviewerColors);
            }
        }

        private bool m_AutoDockSameFile = false;

        public bool AutoDockSameFile
        {
            get { return m_AutoDockSameFile; }
            set
            {
                m_AutoDockSameFile = value;
                SaveRegistrySetting("AutoDockSameFile", m_AutoDockSameFile);
            }
        }


        private bool m_AutoDockSameSymbol = true;

        public bool AutoDockSameSymbol
        {
            get { return m_AutoDockSameSymbol; }
            set
            {
                m_AutoDockSameSymbol = value;
                SaveRegistrySetting("AutoDockSameSymbol", m_AutoDockSameSymbol);
            }
        }


        private bool m_AutoSizeNewWindows = true;

        public bool AutoSizeNewWindows
        {
            get { return m_AutoSizeNewWindows; }
            set
            {
                m_AutoSizeNewWindows = value;
                SaveRegistrySetting("AutoSizeNewWindows", m_AutoSizeNewWindows);
            }
        }

        private bool m_AutoSizeColumnsInWindows = true;

        public bool AutoSizeColumnsInWindows
        {
            get { return m_AutoSizeColumnsInWindows; }
            set
            {
                m_AutoSizeColumnsInWindows = value;
                SaveRegistrySetting("AutoSizeColumnsInWindows", m_AutoSizeColumnsInWindows);
            }
        }


        private bool m_ShowGraphs = true;

        public bool ShowGraphs
        {
            get { return m_ShowGraphs; }
            set
            {
                m_ShowGraphs = value;
                SaveRegistrySetting("ShowGraphs", m_ShowGraphs);
            }
        }

        
        private bool m_ShowAddressesInHex = false;

        public bool ShowAddressesInHex
        {
            get { return m_ShowAddressesInHex; }
            set
            {
                m_ShowAddressesInHex = value;
                SaveRegistrySetting("ShowAddressesInHex", m_ShowAddressesInHex);
            }
        }

        private bool m_AutoChecksum = true;

        public bool AutoChecksum
        {
            get { return m_AutoChecksum; }
            set
            {
                m_AutoChecksum = value;
                SaveRegistrySetting("AutoChecksum", m_AutoChecksum);
            }
        }

       

        private string m_lastfilename = "";

        private bool m_ShowRedWhite = false;

        public bool ShowRedWhite
        {
            get { return m_ShowRedWhite; }
            set
            {
                if (m_ShowRedWhite != value)
                {
                    m_ShowRedWhite = value;
                    SaveRegistrySetting("ShowRedWhite", m_ShowRedWhite);
                }
            }
        }


        

        public string Lastfilename
        {
            get { return m_lastfilename; }
            set {
                if (m_lastfilename != value)
                {
                    m_lastfilename = value;
                    SaveRegistrySetting("LastFilename", m_lastfilename);
                }
            }
        }
        private bool m_viewinhex = false;

        public bool Viewinhex
        {
            get { return m_viewinhex; }
            set 
            {
                if (m_viewinhex != value)
                {
                    m_viewinhex = value;
                    SaveRegistrySetting("ViewInHex", m_viewinhex);
                }
            }
        }

        private bool m_debugmode = false;

        public bool DebugMode
        {
            get { return m_debugmode; }
        }

        private bool m_adminmode = false;

        public bool AdminMode
        {
            get { return m_adminmode; }
        }



        private void SaveRegistrySetting(string key, string value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("VAGEDCSuite"))
            {
                saveSettings.SetValue(key, value);
            }
        }
        private void SaveRegistrySetting(string key, Int32 value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("VAGEDCSuite"))
            {
                saveSettings.SetValue(key, value);
            }
        }
        private void SaveRegistrySetting(string key, bool value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("VAGEDCSuite"))
            {
                saveSettings.SetValue(key, value);
            }
        }

        public void SaveSettings()
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("VAGEDCSuite"))
            {



                saveSettings.SetValue("CodeBlockSyncActive", _codeBlockSyncActive);
                saveSettings.SetValue("RequestProjectNotes", m_RequestProjectNotes);
                saveSettings.SetValue("ShowTablesUpsideDown", m_ShowTablesUpsideDown);

                saveSettings.SetValue("LastProjectname", m_lastprojectname);
                saveSettings.SetValue("LastOpenedType", m_LastOpenedType);
                saveSettings.SetValue("SymbolDockWidth", m_SymbolDockWidth);

                saveSettings.SetValue("ProjectFolder", m_ProjectFolder);
                saveSettings.SetValue("RequestProjectNotes", m_RequestProjectNotes);

                saveSettings.SetValue("ViewInHex", m_viewinhex);
                saveSettings.SetValue("LastFilename", m_lastfilename);
                saveSettings.SetValue("ShowRedWhite", m_ShowRedWhite);
                saveSettings.SetValue("AutoChecksum", m_AutoChecksum);
                saveSettings.SetValue("ShowAddressesInHex", m_ShowAddressesInHex);
                saveSettings.SetValue("ShowGraphs", m_ShowGraphs);
                saveSettings.SetValue("AutoSizeNewWindows", m_AutoSizeNewWindows);
                saveSettings.SetValue("AutoSizeColumnsInWindows", m_AutoSizeColumnsInWindows);
                saveSettings.SetValue("DisableMapviewerColors", m_DisableMapviewerColors);
                saveSettings.SetValue("AutoDockSameFile", m_AutoDockSameFile);
                saveSettings.SetValue("AutoDockSameSymbol", m_AutoDockSameSymbol);
                saveSettings.SetValue("NewPanelsFloating", m_NewPanelsFloating);
                saveSettings.SetValue("Skinname", m_skinname);
                saveSettings.SetValue("DefaultViewType", (int)m_DefaultViewType);
                saveSettings.SetValue("DefaultViewSize", (int)m_DefaultViewSize);
                saveSettings.SetValue("AutoLoadLastFile", m_AutoLoadLastFile);
                saveSettings.SetValue("SynchronizeMapviewers", m_SynchronizeMapviewers);
                saveSettings.SetValue("SynchronizeMapviewersDifferentMaps", m_SynchronizeMapviewersDifferentMaps);
            }
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


        public AppSettings()
        {
            // laad alle waarden uit het register
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");


            using (RegistryKey Settings = TempKey.CreateSubKey("VAGEDCSuite"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == "ViewInHex")
                            {
                                m_viewinhex = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            if (a == "DebugMode")
                            {
                                m_debugmode = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            if (a == "AdminMode")
                            {
                                m_adminmode = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "LastFilename")
                            {
                                m_lastfilename = Settings.GetValue(a).ToString();
                            }
                            else if (a == "AutoChecksum")
                            {
                                m_AutoChecksum = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowAddressesInHex")
                            {
                                m_ShowAddressesInHex = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowGraphs")
                            {
                                m_ShowGraphs = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoSizeNewWindows")
                            {
                                m_AutoSizeNewWindows = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoSizeColumnsInWindows")
                            {
                                m_AutoSizeColumnsInWindows = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }

                            else if (a == "ShowRedWhite")
                            {
                                m_ShowRedWhite = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DisableMapviewerColors")
                            {
                                m_DisableMapviewerColors = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoDockSameFile")
                            {
                                m_AutoDockSameFile = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoDockSameSymbol")
                            {
                                m_AutoDockSameSymbol = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "NewPanelsFloating")
                            {
                                m_NewPanelsFloating = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoLoadLastFile")
                            {
                                m_AutoLoadLastFile = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DefaultViewType")
                            {
                                m_DefaultViewType = (ViewType)Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DefaultViewSize")
                            {
                                m_DefaultViewSize = (ViewSize)Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "SynchronizeMapviewers")
                            {
                                m_SynchronizeMapviewers = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "SynchronizeMapviewersDifferentMaps")
                            {
                                m_SynchronizeMapviewersDifferentMaps = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "Skinname")
                            {
                                m_skinname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "RequestProjectNotes")
                            {
                                m_RequestProjectNotes = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowTablesUpsideDown")
                            {
                                m_ShowTablesUpsideDown = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "LastProjectname")
                            {
                                m_lastprojectname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "CodeBlockSyncActive" || a == "codeBlockSyncActive")
                            {
                                _codeBlockSyncActive = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }

                            else if (a == "LastOpenedType")
                            {
                                m_LastOpenedType = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "SymbolDockWidth")
                            {
                                m_SymbolDockWidth = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ProjectFolder")
                            {
                                m_ProjectFolder = Settings.GetValue(a).ToString();
                            }
                            else if (a == "RequestProjectNotes")
                            {
                                m_RequestProjectNotes = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }

                            //saveSettings.SetValue("RequestProjectNotes", m_RequestProjectNotes);
                            //saveSettings.SetValue("LastProjectname", m_lastprojectname);
                            //saveSettings.SetValue("LastOpenedType", m_LastOpenedType);
                            //saveSettings.SetValue("ProjectFolder", m_ProjectFolder);
                            //saveSettings.SetValue("RequestProjectNotes", m_RequestProjectNotes);


                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("error retrieving registry settings: " + E.Message);
                        }

                    }
                }
            }

        }
    }
}
