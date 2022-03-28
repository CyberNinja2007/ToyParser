using System.Linq;
using System.Threading.Tasks;
using Classes.Parsers;
using Classes.Parsers.Abstraction;
using Classes.Toys.Extensions;
using Classes.Writers;
using Classes.Writers.Abstraction;

namespace ConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IToysParser toyParser = new ToysParser();
            IWriter writerCsv = new WriterCsv();
            var toys = await toyParser.ParseAsync("https://www.toy.ru/catalog/boy_transport/");
            writerCsv.Write(toys.Select(x => x.ToCsv()));
        }
    }
}