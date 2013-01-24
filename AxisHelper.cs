using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    public class AxisHelper
    {
        private int _axisID = 0;

        public int AxisID
        {
            get { return _axisID; }
            set { _axisID = value; }
        }
        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _units;

        public string Units
        {
            get { return _units; }
            set { _units = value; }
        }
        private double _correction = 1;

        public double Correction
        {
            get { return _correction; }
            set { _correction = value; }
        }
        private double _offset = 0;

        public double Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        
    }
}
