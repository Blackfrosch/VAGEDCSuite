using System;
using System.Collections.Generic;
using System.Text;

namespace VAGSuite
{
    public class CellHelper
    {
        private int rowhandle = -1;

        public int Rowhandle
        {
            get { return rowhandle; }
            set { rowhandle = value; }
        }
        private int columnindex = -1;

        public int Columnindex
        {
            get { return columnindex; }
            set { columnindex = value; }
        }
        private int value = 0;

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

    }
}
