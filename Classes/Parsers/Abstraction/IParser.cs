using System.Collections.Generic;
using System.Threading.Tasks;
using Classes.Toys;

namespace Classes.Parsers.Abstraction
{
    public interface IToysParser
    {
        public Task<List<ToyInfo>> ParseAsync(string url);
    }
}