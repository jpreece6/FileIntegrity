using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIntegrity
{
    public enum CompareResult
    {
        NotChecked,
        Missing,
        Changed,
        BinarySame
    }

    public struct CompareResultObject
    {
        public string FilePath;
        public CompareResult CompareResult;

        public CompareResultObject(string file, CompareResult result)
        {
            FilePath = file;
            CompareResult = result;
        }
    }
}
