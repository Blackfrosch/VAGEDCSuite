using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Net.Cache;



namespace VAGSuite
{
    class msiupdater
    {
        private Version m_currentversion;
        private string m_customer = "Global";
        private string m_server = "http://trionic.mobixs.eu/vagedcsuite/";
        private string m_username = "";
        private string m_password = "";
        private Version m_NewVersion;
        private string m_apppath = "";
        private bool m_fromFileLocation = false;
        private bool m_blockauto_updates = false;

        public bool Blockauto_updates
        {
            get { return m_blockauto_updates; }
            set { m_blockauto_updates = value; }
        }
        public string Apppath
        {
            get { return m_apppath; }
            set { m_apppath = value; }
        }

        public Version NewVersion
        {
            get { return m_NewVersion; }
            set { m_NewVersion = value; }
        }
        public delegate void DataPump(MSIUpdaterEventArgs e);
        public event msiupdater.DataPump onDataPump;

        public delegate void UpdateProgressChanged(MSIUpdateProgressEventArgs e);
        public event msiupdater.UpdateProgressChanged onUpdateProgressChanged;
        public class MSIUpdateProgressEventArgs : System.EventArgs
        {
            private Int32 _NoFiles;
            private Int32 _NoFilesDone;
            private Int32 _PercentageDone;
            private Int32 _NoBytes;
            private Int32 _NoBytesDone;

            public Int32 NoBytesDone
            {
                get
                {
                    return _NoBytesDone;
                }
            }

            public Int32 NoBytes
            {
                get
                {
                    return _NoBytes;
                }
            }


            public Int32 NoFiles
            {
                get
                {
                    return _NoFiles;
                }
            }
            public Int32 NoFilesDone
            {
                get
                {
                    return _NoFilesDone;
                }
            }
            public Int32 PercentageDone
            {
                get
                {
                    return _PercentageDone;
                }
            }


            public MSIUpdateProgressEventArgs(Int32 NoFiles, Int32 NoFilesDone, Int32 PercentageDone, Int32 NoBytes, Int32 NoBytesDone)
            {
                this._NoFiles = NoFiles;
                this._NoFilesDone = NoFilesDone;
                this._PercentageDone = PercentageDone;
                this._NoBytes = NoBytes;
                this._NoBytesDone = NoBytesDone;
            }
        }


        public class MSIUpdaterEventArgs : System.EventArgs
        {
            private string _Data;
            private bool _UpdateAvailable;
            private bool _Version2High;
            private bool _info;
            private string _xmlFile;

            public string XMLFile
            {
                get
                {
                    return _xmlFile;
                }
            }

            public bool Info
            {
                get
                {
                    return _info;
                }
            }

            private Version _Version;
            public string Data
            {
                get
                {
                    return _Data;
                }
            }
            public bool UpdateAvailable
            {
                get
                {
                    return _UpdateAvailable;
                }
            }
            public bool Version2High
            {
                get
                {
                    return _Version2High;
                }
            }
            public Version Version
            {
                get
                {
                    return _Version;
                }
            }
            public MSIUpdaterEventArgs(string Data, bool Update, bool mVersion2High, Version NewVersion, bool info, string xmlfile)
            {
                this._Data = Data;
                this._info = info;
                this._UpdateAvailable = Update;
                this._Version2High = mVersion2High;
                this._Version = NewVersion;
                this._xmlFile = xmlfile;
            }
        }

        public msiupdater(Version CurrentVersion)
        {
            m_currentversion = CurrentVersion;
            m_NewVersion = new Version("1.0.0.0");
        }

        public void CheckForUpdates(string customer, string server, string username, string password, bool FromFile)
        {
            m_server = server;
            m_customer = customer;
            m_username = username;
            m_password = password;
            m_fromFileLocation = FromFile;
            if (!m_blockauto_updates)
            {
                System.Threading.Thread t = new System.Threading.Thread(updatecheck);
                t.Start();
            }
        }

        public void ExecuteUpdate(Version ver)
        {
            string command = "http://trionic.mobixs.eu/vagedcsuite/" + ver.ToString() + "/VAGEDCSuite.msi";
            try
            {
                System.Diagnostics.Process.Start(command);
            }
            catch (Exception E)
            {
                PumpString("Exception when checking new update(s): " + E.Message, false, false, new Version(), false, "");
            }
        }


        private void PumpString(string text, bool updateavailable, bool version2high, Version newver, bool info, string xmlfile)
        {
            onDataPump(new MSIUpdaterEventArgs(text, updateavailable, version2high, newver, info, xmlfile));
        }

        private void NotifyProgress(Int32 NoFiles, Int32 NoFilesDone, Int32 PercentageDone, Int32 NoBytes, Int32 NoBytesDone)
        {
            onUpdateProgressChanged(new MSIUpdateProgressEventArgs(NoFiles, NoFilesDone, PercentageDone, NoBytes, NoBytesDone));
        }

        public string GetPageHTML(string pageUrl, int timeoutSeconds)
        {
            System.Net.WebResponse response = null;

            try
            {
                // Setup our Web request
                System.Net.WebRequest request = System.Net.WebRequest.Create(pageUrl);
                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.CachePolicy = noCachePolicy;
                try
                {
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }
                catch (Exception proxyE)
                {
                    PumpString("Error setting proxy server: " + proxyE.Message, false, false, new Version(), false, "");
                }

                request.Timeout = timeoutSeconds * 1000;

                // Retrieve data from request
                response = request.GetResponse();

                System.IO.Stream streamReceive = response.GetResponseStream();
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
                System.IO.StreamReader streamRead = new System.IO.StreamReader(streamReceive, encoding);

                // return the retrieved HTML
                return streamRead.ReadToEnd();
            }
            catch (Exception ex)
            {
                // Error occured grabbing data, return empty string.
                PumpString("An error occurred while retrieving the HTML content. " + ex.Message, false, false, new Version(), false, "");
                return "";
            }
            finally
            {
                // Check if exists, then close the response.
                if (response != null)
                {
                    response.Close();
                }
            }
        }
        private void ExtractNameValue(string input, out string Name, out string Value)
		{
			Name = "";
			Value = "";
			// input : <Element name="I2l" value="00" />
			int id1,id2,id3,id4;
			id1=input.IndexOf("\"",0,input.Length);
			if(id1>0) // eerste " gevonden
			{
				id2=input.IndexOf("\"",id1+1,input.Length - id1 - 1);
				if(id2 > 0) // tweede " gevonden
				{
					id3=input.IndexOf("\"",id2+1,input.Length - id2 - 1);
					if(id3>0)
					{
						id4=input.IndexOf("\"",id3+1,input.Length - id3 - 1);
						if(id4>0)
						{
							Name = input.Substring(id1+1,id2-id1-1);
							Value = input.Substring(id3+1,id4-id3-1);
							// alles gevonden
						}
					}

				}
				
			}
		}

        private string FileToString(string infile)
        {
            StreamReader stream = System.IO.File.OpenText(infile);
            string returnvalues;
            returnvalues = stream.ReadToEnd();
            stream.Close();
            return returnvalues;
        }

        private void updatecheck()
        {
            string URLString="";
            string XMLResult="";
            //string VehicleString;
            bool m_updateavailable = false;
            bool m_version_toohigh = false;
            bool _info = false;
            Version maxversion = new Version("1.0.0.0");
            File.Delete(Apppath + "\\input.xml");
            File.Delete(Apppath + "\\Notes.xml");

            try
            {
                if (m_customer.Length > 0)
                {
                    URLString = "http://trionic.mobixs.eu/vagedcsuite/version.xml";
                    XMLResult = GetPageHTML(URLString, 10);
                    using (StreamWriter xmlfile = new StreamWriter(Apppath + "\\input.xml", false, System.Text.Encoding.ASCII, 2048))
                    {
                        xmlfile.Write(XMLResult);
                        xmlfile.Close();
                    }
                    URLString = "http://trionic.mobixs.eu/vagedcsuite/Notes.xml";
                    XMLResult = GetPageHTML(URLString, 10);
                    using (StreamWriter xmlfile = new StreamWriter(Apppath + "\\Notes.xml", false, System.Text.Encoding.ASCII, 2048))
                    {
                        xmlfile.Write(XMLResult);
                        xmlfile.Close();
                    }
                }

                XmlDocument doc;
                try
                {
                    doc = new XmlDocument();
                    doc.LoadXml(FileToString(Apppath + "\\input.xml"));

                    // Add any other properties that would be useful to store
                    //foreach (
                    System.Xml.XmlNodeList Nodes;
                    Nodes = doc.GetElementsByTagName("vagedcsuite");
                    foreach (System.Xml.XmlNode Item in Nodes)
                    {
                        System.Xml.XmlAttributeCollection XMLColl;
                        XMLColl = Item.Attributes;
                        foreach (System.Xml.XmlAttribute myAttr in XMLColl)
                        {
                            if (myAttr.Name == "version")
                            {
                                Version v = new Version(myAttr.Value);
                                if (v > m_currentversion) 
                                {
                                    if (v > maxversion) maxversion = v;
                                    m_updateavailable = true;
                                    PumpString("Available version: " + myAttr.Value, false, false, new Version(), false, Apppath + "\\Notes.xml");
                                }
                                else if (v.Major < m_currentversion.Major || (v.Major == m_currentversion.Major && v.Minor < m_currentversion.Minor) || (v.Major == m_currentversion.Major && v.Minor == m_currentversion.Minor && v.Build < m_currentversion.Build))
                                {

                                    // mmm .. gebruiker draait een versie die hoger is dan dat is vrijgegeven... 
                                    if (v > maxversion) maxversion = v;
                                    m_updateavailable = false;
                                    m_version_toohigh = true;
                                }
                            }
                            else if (myAttr.Name == "info")
                            {
                                try
                                {
                                    _info = Convert.ToBoolean(myAttr.Value);
                                }
                                catch (Exception sendIE)
                                {
                                    Console.WriteLine(sendIE.Message);
                                }
                            }
                        }

                    }
                }
                catch (Exception E)
                {
                    PumpString(E.Message, false, false, new Version(), false, "");
                }
                if (m_updateavailable)
                {

                    //Console.WriteLine("An update is available: " + maxversion.ToString());
                    PumpString("A newer version is available: " + maxversion.ToString(), m_updateavailable, m_version_toohigh, maxversion, _info, Apppath + "\\Notes.xml");
                    m_NewVersion = maxversion;

                }
                else if (m_version_toohigh)
                {
                    PumpString("Versionnumber is too high: " + maxversion.ToString(), m_updateavailable, m_version_toohigh, maxversion, _info, Apppath + "\\Notes.xml");
                    m_NewVersion = maxversion;
                }
                else
                {
                    PumpString("No new version(s) found...", false, false, new Version(), _info, Apppath + "\\Notes.xml");
                }
            }
            catch (Exception tuE)
            {
                PumpString(tuE.Message, false, false, new Version(), _info, "");
            }
            
        }



        internal string GetReleaseNotes()
        {
            string URLString = "http://trionic.mobixs.eu/vagedcsuite/Notes.xml";
            string XMLResult = GetPageHTML(URLString, 10);
            using (StreamWriter xmlfile = new StreamWriter(Apppath + "\\Notes.xml", false, System.Text.Encoding.ASCII, 2048))
            {
                xmlfile.Write(XMLResult);
                xmlfile.Close();
            }
            return Apppath + "\\Notes.xml";
        }
    }
}
