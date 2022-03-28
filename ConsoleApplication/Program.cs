using System.Linq;
using System.Threading.Tasks;
using Classes;

namespace ConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ToysParser toyParser = new ToysParser();
            var toys = await toyParser.ParseAsync("https://www.toy.ru/catalog/boy_transport/");
            WriterCsv.Write(toys.Select(x => x.ToCsv()));
        }
    }
}