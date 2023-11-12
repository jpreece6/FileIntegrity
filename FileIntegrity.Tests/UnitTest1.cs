using System.Diagnostics;
using System.Text;

namespace FileIntegrity.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private StringBuilder ConsoleOutput { get; set; } = new StringBuilder();

        [TestInitialize]
        public void Init()
        {
            Console.SetOut(new StringWriter(this.ConsoleOutput));    // Associate StringBuilder with StdOut
            this.ConsoleOutput.Clear();    // Clear text from any previous text runs
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var list = await Checker.GetHashListAsync(@"C:\Users\joshp\Desktop\Hash.md5");

            foreach (var item in list)
            {
                var result = await Checker.VerifyAsync(item);
                Debug.WriteLine($"{item.FilePath} is {result}");
            }
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            var list = await Checker.GetHashListAsync(@"C:\Users\joshp\Desktop\Hash.md5");

            await foreach(var item in Checker.VerifyAsync(list))
            {
                Debug.WriteLine($"{item.FilePath} is {item.CompareResult}");
            }
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            await foreach (var item in Checker.VerifyAsync(@"C:\Users\joshp\Desktop\Hash.md5"))
            {
                Debug.WriteLine($"{item.FilePath} is {item.CompareResult}");
            }
        }
    }
}