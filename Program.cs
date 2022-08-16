using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dataset_parser
{
    class Program
    {
        public static string Tokenize(string inputString) 
        {
            string pattern = @"^(\s+|\d+|\w+|[^\d\s\w]+)+$";
            List<string> tmpList = new List<string>();
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(inputString))
            {
                Match match = regex.Match(inputString);

                foreach (Capture capture in match.Groups[1].Captures)
                {
                    if (!string.IsNullOrWhiteSpace(capture.Value))
                        tmpList.Add(capture.Value);
                }
            }
            string returnString = "";
            if (!(inputString.EndsWith("GEOPOLIT") || inputString.EndsWith("ORG") || inputString.EndsWith("PER") || inputString.EndsWith("LOC") || inputString.EndsWith("MEDIA"))) 
            {
                returnString += string.Join('\n', tmpList);
            } else {
                string[] entityArray = inputString.Split(null);
                int indexToRemove = entityArray.Length - 1;
                string entityType = entityArray[indexToRemove];
                entityArray = entityArray.Where((source, index) =>index != indexToRemove).ToArray();
                for (int index = 0; index < entityArray.Length; index++)
                {
                    entityArray[index] = entityArray[index] + "\t" + entityType;
                }
                string entityString = string.Join("\n", entityArray);
                returnString += entityString;
            }
            return returnString;
        }
        static void Main(string[] args)
        {
            string[] outname = args[1].Split("/");
            string outPath = "tsv_" + outname[outname.Length - 1];
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
            string orig_path = args[1];
            string[] orig_doc = File.ReadAllLines(orig_path, Encoding.UTF8);
            string orig_doc_str = string.Join("  ", orig_doc);
            string saved_doc_str = orig_doc_str;
            for (int i = entitiesDict.Count - 1; i >= 0 ; i--) {
                KeyValuePair<System.Tuple<int, int>, string> entry = entitiesDict.ElementAt(i);
                int lastIndex = orig_doc_str.Length;
                orig_doc_str = orig_doc_str.Substring(0, entry.Key.Item1) + "\n" + orig_doc_str.Substring(entry.Key.Item1, entry.Key.Item2 - entry.Key.Item1) + " " + entry.Value + "\n" + orig_doc_str.Substring(entry.Key.Item2, lastIndex - entry.Key.Item2);
            }
            var itemsArray = orig_doc_str.Split("\n");
            for (int index = 0; index < itemsArray.Length; index++)
            {
                itemsArray[index] = Tokenize(itemsArray[index]);
            }
            string outputString = string.Join("\n", itemsArray);
            outputString = outputString.Replace("\n\n", "\n");
            itemsArray = outputString.Split("\n");
            for (int index = 0; index < itemsArray.Length; index++)
            {
                if (!(itemsArray[index].EndsWith("GEOPOLIT") || itemsArray[index].EndsWith("ORG") || itemsArray[index].EndsWith("PER") || itemsArray[index].EndsWith("LOC") || itemsArray[index].EndsWith("MEDIA") || itemsArray[index] == "")) 
                    {
                        itemsArray[index] += "\tO";
                    }
            }
            outputString = string.Join("\n", itemsArray);
            if (outputString.StartsWith("\n") || outputString.StartsWith(" \n"))
            {
                outputString = outputString.Remove(0, 1);
            }
            File.WriteAllText(outPath, outputString);
        }
    }
}
