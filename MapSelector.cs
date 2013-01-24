using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    class MapSelector
    {
        int[] _mapIndexes;

        public int[] MapIndexes
        {
            get { return _mapIndexes; }
            set { _mapIndexes = value; }
        }

        int[] _mapData;

        public int[] MapData
        {
            get { return _mapData; }
            set { _mapData = value; }
        }

        int startAddress = 0;

        public int StartAddress
        {
            get { return startAddress; }
            set { startAddress = value; }
        }
        int numRepeats = 0;

        public int NumRepeats
        {
            get { return numRepeats; }
            set { numRepeats = value; }
        }
        int xAxisAddress = 0;

        public int XAxisAddress
        {
            get { return xAxisAddress; }
            set { xAxisAddress = value; }
        }
        int yAxisAddress = 0;

        public int YAxisAddress
        {
            get { return yAxisAddress; }
            set { yAxisAddress = value; }
        }
        int xAxisID = 0;

        public int XAxisID
        {
            get { return xAxisID; }
            set { xAxisID = value; }
        }
        int yAxisID = 0;

        public int YAxisID
        {
            get { return yAxisID; }
            set { yAxisID = value; }
        }
        int xAxisLen = 0;

        public int XAxisLen
        {
            get { return xAxisLen; }
            set { xAxisLen = value; }
        }
        int yAxisLen = 0;

        public int YAxisLen
        {
            get { return yAxisLen; }
            set { yAxisLen = value; }
        }
        int mapLength = 0;

        public int MapLength
        {
            get { return mapLength; }
            set { mapLength = value; }
        }
    }
}
