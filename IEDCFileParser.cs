using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    abstract public class IEDCFileParser
    {
        abstract public SymbolCollection parseFile(string filename, out List<CodeBlock> newCodeBlocks, out List<AxisHelper> newAxisHelpers);
        abstract public string ExtractBoschPartnumber(byte[] allBytes);
        abstract public string ExtractSoftwareNumber(byte[] allBytes);
        abstract public string ExtractPartnumber(byte[] allBytes);
        abstract public string ExtractInfo(byte[] allBytes);
        abstract public void NameKnownMaps(byte[] allBytes, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks);
        abstract public void FindSVBL(byte[] allBytes, string filename, SymbolCollection newSymbols, List<CodeBlock> newCodeBlocks);

    }
}
