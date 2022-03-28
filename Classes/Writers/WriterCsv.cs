using System;
using System.Collections.Generic;
using System.IO;
using Classes.Writers.Abstraction;

namespace Classes.Writers
{
    public class WriterCsv : IWriter
    {
        public void Write(IEnumerable<String> data)
        {
            string filePath = @"Result.csv";
            File.WriteAllLines(filePath, data);
        }
    }
}