using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{

    public enum LogbookEntryType : int
    {
        Note,
        LogfileStarted,
        TransactionExecuted,
        SynchronizationStarted,
        TransactionRolledback,
        TransactionRolledforward,
        BackupfileCreated,
        ProjectFileRecreated,
        PropertiesEdited
    }

    /// <summary>
    /// Keeps a project log including notes
    /// Start new log file for realtime logging
    /// New transaction entry (when changing binary)
    /// Synchronizing with ECU
    /// Rollback/forward transactions
    /// Creation of backup files
    /// </summary>
    public class ProjectLog
    {

        private string m_filename = string.Empty;

        public void OpenProjectLog(string project)
        {
            m_filename = project + "\\ProjectLogbook.log";
        }

        public void WriteLogbookEntry(LogbookEntryType type, string logbookEntry)
        {
            DateTime dtentry = DateTime.Now;
            string _logEntry = dtentry.Day.ToString("D2") + dtentry.Month.ToString("D2") + dtentry.Year.ToString("D4") + dtentry.Hour.ToString("D2") + dtentry.Minute.ToString("D2") + dtentry.Second.ToString("D2");
            switch (type)
            {
                case LogbookEntryType.BackupfileCreated:
                    _logEntry += "|A backup file was created|"; 
                    break;
                case LogbookEntryType.LogfileStarted:
                    _logEntry += "|A realtime log file was started|";
                    break;
                case LogbookEntryType.Note:
                    _logEntry += "|A project note was inserted|"; 
                    break;
                case LogbookEntryType.SynchronizationStarted:
                    _logEntry += "|Synchronization with the ECU was started|";
                    break;
                case LogbookEntryType.TransactionExecuted:
                    _logEntry += "|A transaction was executed|"; 
                    break;
                case LogbookEntryType.TransactionRolledback:
                    _logEntry += "|A transaction was rolled back|";
                    break;
                case LogbookEntryType.TransactionRolledforward:
                    _logEntry += "|A transaction rolled forward|";
                    break;
                case LogbookEntryType.ProjectFileRecreated:
                    _logEntry += "|A project file was recreated|";
                    break;
                case LogbookEntryType.PropertiesEdited:
                    _logEntry += "|Project properties were edited|";
                    break;
            }
            _logEntry += logbookEntry.Replace("|", " ");
            if (m_filename != string.Empty)
            {
                using (StreamWriter sw = new StreamWriter(m_filename, true))
                {
                    sw.WriteLine(_logEntry);
                }
            }

        }
    }
}
