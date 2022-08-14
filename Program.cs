using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace dataset_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var entitiesDict = new Dictionary<Tuple<int,int>, string>(); 
            string path = args[0];
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            foreach (string line in lines)
            {
                string[] splitContent = line.Split(null);
                int start = int.Parse(splitContent[2]);
                int stop = int.Parse(splitContent[3]);
                entitiesDict.Add(new Tuple<int,int>(start, stop), splitContent[1]);
            }
            foreach (var e in entitiesDict)
                Console.WriteLine("{0} {1}", e.Key, e.Value); 
        }
    }
}
