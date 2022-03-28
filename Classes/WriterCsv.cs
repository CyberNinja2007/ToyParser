using System;
using System.Collections.Generic;
using System.IO;

namespace Classes
{
    public static class WriterCsv
    {
        public static void  Write(IEnumerable<String> data)
        {
            string filePath = @"Result.csv";
            File.WriteAllLines(filePath, data);
        }
    }
}