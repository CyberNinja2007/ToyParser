using System;
using System.Collections.Generic;

namespace Classes.Writers.Abstraction
{
    public interface IWriter
    {
        public void Write(IEnumerable<String> data);
    }
}