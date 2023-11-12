using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace FileIntegrity
{
    public struct HashedItem
    {
        public byte[] Hash;
        public string FilePath;
        public HashAlgorithmType AlgorithmType;
    }
}
