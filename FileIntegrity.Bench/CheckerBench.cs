using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIntegrity.Bench
{
    [MemoryDiagnoser]
    public class CheckerBench
    {
        [Benchmark]
        public async Task CompareFiles()
        {
            await foreach (var item in Checker.VerifyAsync(@"C:\Users\joshp\Desktop\Hash.md5"))
            {
                // Do stuff
            }
        }
    }
}
