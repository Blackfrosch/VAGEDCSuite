using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    public class CodeBlock
    {
        private GearboxType m_blockGearboxType = GearboxType.Manual;

        public GearboxType BlockGearboxType
        {
            get { return m_blockGearboxType; }
            set { m_blockGearboxType = value; }
        }

        int startAddress = 0;

        public int StartAddress
        {
            get { return startAddress; }
            set { startAddress = value; }
        }
        int endAddress = 0;

        public int EndAddress
        {
            get { return endAddress; }
            set { endAddress = value; }
        }
        /*UInt32 checksum = 0;

        public UInt32 Checksum
        {
            get { return checksum; }
            set { checksum = value; }
        }*/
        int codeID = 0;

        public int CodeID
        {
            get { return codeID; }
            set { codeID = value; }
        }

        int addressID = 0;

        public int AddressID
        {
            get { return addressID; }
            set { addressID = value; }
        }

        /*bool valid = false;

        public bool Valid
        {
            get { return valid; }
            set { valid = value; }
        }*/
    }
}
