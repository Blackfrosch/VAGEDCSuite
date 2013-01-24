using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    public class TransactionEntry
    {

        private bool _isRolledBack = false;

        public bool IsRolledBack
        {
            get { return _isRolledBack; }
            set { _isRolledBack = value; }
        }

        private string _symbolName = string.Empty;

        public string SymbolName
        {
            get { return _symbolName; }
            set { _symbolName = value; }
        }


        byte[] _dataBefore;

        public byte[] DataBefore
        {
            get { return _dataBefore; }
            set { _dataBefore = value; }
        }
        byte[] _dataAfter;

        public byte[] DataAfter
        {
            get { return _dataAfter; }
            set { _dataAfter = value; }
        }
        Int32 _symbolAddress;

        public Int32 SymbolAddress
        {
            get { return _symbolAddress; }
            set { _symbolAddress = value; }
        }
        Int32 _symbolLength;

        public Int32 SymbolLength
        {
            get { return _symbolLength; }
            set { _symbolLength = value; }
        }
        DateTime _entryDateTime;

        public DateTime EntryDateTime
        {
            get { return _entryDateTime; }
            set { _entryDateTime = value; }
        }

        private Int32 _transactionNumber = 0;

        public Int32 TransactionNumber
        {
            get { return _transactionNumber; }
            set { _transactionNumber = value; }
        }

        private string _note = string.Empty;

        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        public TransactionEntry(DateTime entry, Int32 symbolAddress, Int32 symbolLength, byte[] before, byte[] after, byte isRolledBack, Int32 transActionNumber, string note)
        {
            if (isRolledBack != 0) _isRolledBack = true;
            _entryDateTime = entry;
            _symbolAddress = symbolAddress;
            _symbolLength = symbolLength;
            _dataBefore = before;
            _dataAfter = after;
            _note = note;
            _transactionNumber = transActionNumber;
        }
    }
}
