using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    public class PerformanceResults
    {
        private int _torque = 0;

        public int Torque
        {
            get { return _torque; }
            set { _torque = value; }
        }
        private int _horsepower = 0;

        public int Horsepower
        {
            get { return _horsepower; }
            set { _horsepower = value; }
        }
    }
}
