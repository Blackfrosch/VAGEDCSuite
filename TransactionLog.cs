/*
ttl (transaction log) files

format:
4 bytes hex, checksum (sum)
4 bytes hex, number of entries in the file
number of entries repeating
1 byte hex day, 1 byte hex month, 2 bytes hex year, 1 byte hex hour 1 byte hex minute, 1 byte hex second, 4 bytes hex address, 2 bytes hex length, x bytes hex (data) before, x bytes (data) after

baseline backups: once every x starts/ once every x records in ttl file?

Functions:

Rollback transactions (per transactions, showing mapname, datetime and data before/after) in gridview with custom dropdown control or popup window
Rebuild tune from baseline x (date time) upto date time y
Set parameters (baseline generation frequency, number of records per ttl file etc)
Purge history (move ttl files and baselines into archive)
Purge archive (delete ttl file and baselines from archive)

Expand project files:

- stage
- hardware properties (get them from the bin file ?)
-  * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace VAGSuite
{
    public class TransactionLog
    {
        private string m_fileName = string.Empty;
        private TransactionCollection _transCollection = new TransactionCollection();

        public TransactionCollection TransCollection
        {
            get { return _transCollection; }
            set { _transCollection = value; }
        }

        public bool OpenTransActionLog(string projectFolder, string project)
        {
            bool retval = false;
            if (!Directory.Exists(projectFolder)) Directory.CreateDirectory(projectFolder);
            if (!Directory.Exists(projectFolder + "\\" + project)) Directory.CreateDirectory(projectFolder + "\\" + project);
            if (File.Exists(projectFolder + "\\" + project + "\\TransActionLogV2.ttl"))
            {
                m_fileName = projectFolder + "\\" + project + "\\TransActionLogV2.ttl";
                retval = true;
            }
            else
            {
                retval = CreateFile(projectFolder, project);
            }

            if (File.Exists(projectFolder + "\\" + project + "\\TransActionLog.ttl"))
            {
                UpgradeTransActionLog(projectFolder + "\\" + project + "\\TransActionLog.ttl", projectFolder + "\\" + project + "\\TransActionLogV2.ttl");
            }
            return retval;

        }

        private void UpgradeTransActionLog(string input, string output)
        {
            _transCollection = ReadTransactionFileVersion1(input);
            // save transactions into new file
            SaveTransactionLog();
            File.Delete(input);
        }

        // holds information about the transaction log for a certain project (should be stored in the project folder)
        public bool CreateFile(string projectFolder, string project)
        {
            bool retval = false;
            
            m_fileName = projectFolder + "\\" + project + "\\TransActionLogV2.ttl";
            FileStream fs = new FileStream(m_fileName, FileMode.Create);
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                Int32 NumberOfRecords = 0;
                bw.Write(NumberOfRecords); // dummy checksum
                bw.Write(NumberOfRecords);
                retval = true;
            }
            fs.Close();
            UpdateChecksum();
            return retval;
        }

        public bool CreateFileByFilename()
        {
            bool retval = false;
            FileStream fs = new FileStream(m_fileName, FileMode.Create);
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                Int32 NumberOfRecords = 0;
                bw.Write(NumberOfRecords); // dummy checksum
                bw.Write(NumberOfRecords);
                retval = true;
            }
            fs.Close();
            UpdateChecksum();
            return retval;
        }

        public bool VerifyChecksum()
        {
            bool retval = false;
            // checksum should be in the file as well, ranging from byte 4 - end
            FileInfo fi = new FileInfo(m_fileName);
            Int32 checksum = readInt32FromFile(m_fileName, 0);
            byte[] fileContent = readbytesFromFile(m_fileName, 4, (int)(fi.Length - 4));
            Int32 calcChecksum = 0;
            foreach (byte b in fileContent)
            {
                calcChecksum += Convert.ToInt32(b);
            }
            if (calcChecksum == checksum) retval = true;
            return retval;
        }

        public void UpdateChecksum()
        {
            // calculate the checksum in the file
            FileInfo fi = new FileInfo(m_fileName);
            byte[] fileContent = readbytesFromFile(m_fileName, 4, (int)(fi.Length - 4));
            Int32 calcChecksum = 0;
            foreach (byte b in fileContent)
            {
                calcChecksum += Convert.ToInt32(b);
            }
            writeInt32ToFile(m_fileName, 0, calcChecksum);

        }

        public void UpdateNumberOfTransActions()
        {
            writeInt32ToFile(m_fileName, 4, _transCollection.Count);
        }

        private void writeInt32ToFile(string filename, int offset, Int32 value)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Seek(offset, SeekOrigin.Begin);
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(value);
            }
            fs.Close();
        }

        private Int32 readInt32FromFile(string filename, int offset)
        {
            Int32 retval = 0;

            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Seek(offset, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fs))
            {
                retval = br.ReadInt32();
            }
            fs.Close();
            return retval;
            
        }

        private byte[] readbytesFromFile(string filename, int offset, int length)
        {
            byte[] retval = new byte[length];
            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Seek(offset, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fs))
            {
                retval = br.ReadBytes(length);
            }
            fs.Close();
            return retval;
        }

        private byte readbyteFromFile(string filename, int offset)
        {
            byte retval;
            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Seek(offset, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fs))
            {
                retval = br.ReadByte();
            }
            fs.Close();
            return retval;
        }

        public void ReadTransactionFile()
        {
            _transCollection = new TransactionCollection();
            if (VerifyChecksum())
            {
                FileInfo fi = new FileInfo(m_fileName);
                Int32 _entryCount = readInt32FromFile(m_fileName, 4);
                FileStream fs = new FileStream(m_fileName, FileMode.Open);
                fs.Seek(8, SeekOrigin.Begin);
                using (BinaryReader br = new BinaryReader(fs))
                {
                  //  byte[] allBytes = br.ReadBytes((int)fi.Length - 8);

                    //1 byte hex day, 1 byte hex month, 2 bytes hex year, 1 byte hex hour 1 byte hex minute, 1 byte hex second, 4 bytes hex address, 2 bytes hex length, x bytes hex (data) before, x bytes (data) after
                    for (int t = 0; t < _entryCount; t++)
                    {
                        Int32 transActionNumber = br.ReadInt32();
                        byte isRolledBack = br.ReadByte();
                        byte day = br.ReadByte();
                        byte month = br.ReadByte();
                        Int16 year = br.ReadInt16();
                        byte hour = br.ReadByte();
                        byte minute = br.ReadByte();
                        byte second = br.ReadByte();
                        DateTime entryDateTime = new DateTime(year, Convert.ToInt16(month), Convert.ToInt16(day), Convert.ToInt16(hour), Convert.ToInt16(minute), Convert.ToInt16(second));
                        Int32 address = br.ReadInt32();
                        Int16 NoteLength = br.ReadInt16();
                        byte[] notedata = br.ReadBytes(NoteLength);
                        string note = string.Empty;
                        for (int i = 0; i < notedata.Length; i += 2)
                        {
                            string Hex = string.Empty;
                            Hex += Convert.ToChar(notedata[i]);
                            Hex += Convert.ToChar(notedata[i + 1]);
                            int value = Convert.ToInt32(Hex, 16);
                            note += Convert.ToChar(value);
                        }
                        Int16 length = br.ReadInt16();
                        byte[] beforeData = br.ReadBytes(length);
                        byte[] afterData = br.ReadBytes(length);
                        // read day
                        TransactionEntry entry = new TransactionEntry(entryDateTime, address, length, beforeData, afterData, isRolledBack, transActionNumber, note);
                        _transCollection.Add(entry);
                    }
                }
                fs.Close();

            }
        }

        public TransactionCollection ReadTransactionFileVersion1(string filename)
        {
            TransactionCollection m_transCollection = new TransactionCollection();
            Int32 _entryCount = readInt32FromFile(filename, 4);
            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Seek(8, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fs))
            {
                //1 byte hex day, 1 byte hex month, 2 bytes hex year, 1 byte hex hour 1 byte hex minute, 1 byte hex second, 4 bytes hex address, 2 bytes hex length, x bytes hex (data) before, x bytes (data) after
                for (int t = 0; t < _entryCount; t++)
                {
                    Int32 transActionNumber = br.ReadInt32();
                    byte isRolledBack = br.ReadByte();
                    byte day = br.ReadByte();
                    byte month = br.ReadByte();
                    Int16 year = br.ReadInt16();
                    byte hour = br.ReadByte();
                    byte minute = br.ReadByte();
                    byte second = br.ReadByte();
                    DateTime entryDateTime = new DateTime(year, Convert.ToInt16(month), Convert.ToInt16(day), Convert.ToInt16(hour), Convert.ToInt16(minute), Convert.ToInt16(second));
                    Int32 address = br.ReadInt32();
                    Int16 length = br.ReadInt16();
                    byte[] beforeData = br.ReadBytes(length);
                    byte[] afterData = br.ReadBytes(length);
                    // read day

                    TransactionEntry entry = new TransactionEntry(entryDateTime, address, length, beforeData, afterData, isRolledBack, transActionNumber, "");
                    m_transCollection.Add(entry);
                }
            }
            fs.Close();


            return m_transCollection;
        }

        private void AppendByteToFile(byte b)
        {
            FileStream fs = new FileStream(m_fileName, FileMode.Append);
            using (BinaryWriter br = new BinaryWriter(fs))
            {
                br.Write(b);
            }
            fs.Close();
        }

        private void AppendInt16ToFile(Int16 b)
        {
            FileStream fs = new FileStream(m_fileName, FileMode.Append);
            using (BinaryWriter br = new BinaryWriter(fs))
            {
                br.Write(b);
            }
            fs.Close();
        }

        private void AppendInt32ToFile(Int32 b)
        {
            FileStream fs = new FileStream(m_fileName, FileMode.Append);
            using (BinaryWriter br = new BinaryWriter(fs))
            {
                br.Write(b);
            }
            fs.Close();
        }

        private void AppendDataToFile(byte[] b)
        {
            FileStream fs = new FileStream(m_fileName, FileMode.Append);
            using (BinaryWriter br = new BinaryWriter(fs))
            {
                br.Write(b);
            }
            fs.Close();
        }

        private void AddTransactionToFile(TransactionEntry entry, bool updateChecksum)
        {
            // append at the end of the file
            //1 byte hex day, 1 byte hex month, 2 bytes hex year, 1 byte hex hour 1 byte hex minute, 1 byte hex second, 4 bytes hex address, 2 bytes hex length, x bytes hex (data) before, x bytes (data) after
            AppendInt32ToFile(entry.TransactionNumber); 
            AppendByteToFile(Convert.ToByte(entry.IsRolledBack)); // not rolled back
            AppendByteToFile(Convert.ToByte(entry.EntryDateTime.Day));
            AppendByteToFile(Convert.ToByte(entry.EntryDateTime.Month));
            AppendInt16ToFile(Convert.ToInt16(entry.EntryDateTime.Year));
            AppendByteToFile(Convert.ToByte(entry.EntryDateTime.Hour));
            AppendByteToFile(Convert.ToByte(entry.EntryDateTime.Minute));
            AppendByteToFile(Convert.ToByte(entry.EntryDateTime.Second));
            AppendInt32ToFile(entry.SymbolAddress);
            AppendInt16ToFile(Convert.ToInt16(entry.Note.Length * 2));
            // now add double hex all characters
            for (int i = 0; i < entry.Note.Length; i++)
            {
                byte curByte = Convert.ToByte(entry.Note[i]);
                string Hex = curByte.ToString("X2");
                AppendByteToFile(Convert.ToByte(Hex[0]));
                AppendByteToFile(Convert.ToByte(Hex[1]));
            }
            AppendInt16ToFile(Convert.ToInt16(entry.SymbolLength));
            AppendDataToFile(entry.DataBefore);
            AppendDataToFile(entry.DataAfter);
            // update number of entries
            Int32 _entryCount = readInt32FromFile(m_fileName, 4);
            _entryCount++;

            if (updateChecksum)
            {
                writeInt32ToFile(m_fileName, 4, _entryCount);
                UpdateChecksum();
            }
            // update checksum
        }

        private void SaveTransactionLog()
        {
            // save the entire file (recreate)
            File.Delete(m_fileName);
            CreateFileByFilename();
            // now save all data
            foreach (TransactionEntry entry in _transCollection)
            {
                AddTransactionToFile(entry, false);
            }
            UpdateNumberOfTransActions();
            UpdateChecksum();
        }

        public void AddToTransactionLog(TransactionEntry entry)
        {
            entry.TransactionNumber = _transCollection.Count + 1;
            _transCollection.Add(entry);
            //SaveTransactionLog();
            AddTransactionToFile(entry, true);
            //UpdateNumberOfTransActions();
            //UpdateChecksum();
        }


        public void SetEntryNote(TransactionEntry ChangedEntry)
        {
            foreach (TransactionEntry entry in _transCollection)
            {
                if (entry.TransactionNumber == ChangedEntry.TransactionNumber)
                {
                    entry.Note = ChangedEntry.Note;
                }
            }
            SaveTransactionLog();
        }

        public void SetEntryRolledBack(int transActionNumber)
        {
            foreach (TransactionEntry entry in _transCollection)
            {
                if (entry.TransactionNumber == transActionNumber)
                {
                    entry.IsRolledBack = true;
                }
            }
            SaveTransactionLog();
        }

        public void SetEntryRolledForward(int transActionNumber)
        {
            foreach (TransactionEntry entry in _transCollection)
            {
                if (entry.TransactionNumber == transActionNumber)
                {
                    entry.IsRolledBack = false;
                }
            }
            SaveTransactionLog();
        }

        public void Purge()
        {
            // purge the file so that only the last 1000 records are kept and the rest is deleted

            // create a copy
            try
            {
                string newFilename = Path.Combine(Path.GetDirectoryName(m_fileName), Path.GetFileNameWithoutExtension(m_fileName) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".btl");
                File.Copy(m_fileName, newFilename);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

            File.Delete(m_fileName);

            while (_transCollection.Count > 1000) _transCollection.RemoveAt(0);
            CreateFileByFilename();
            int entryCount = 0;
            foreach (TransactionEntry entry in _transCollection)
            {
                    entry.TransactionNumber = entryCount++;
                    AddTransactionToFile(entry, false);
            }
            UpdateNumberOfTransActions();
            UpdateChecksum();
            ReadTransactionFile();            
        }
    }
}
