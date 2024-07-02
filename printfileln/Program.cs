﻿using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2 || args.Length > 3)
        {
            Console.WriteLine("Use: program <file-path> <line-index> [should-generate-test-file]");
            return;
        }

        if (args.Length == 3 && args[2] == "true")
        {
            using (StreamWriter outputFile = new StreamWriter("test.txt"))
            {
                string[] testLines = ProgramHelpers.GenerateRandomLines();
                foreach (string ln in testLines)
                    outputFile.WriteLine(ln);
            }
            File.Delete("test.txt.idx");
        }

        string filePath = args[0];
        int lineIndex = int.Parse(args[1]);

        string indexPath = filePath + ".idx";
        try
        {
            if (!File.Exists(indexPath))
            {
                ProgramHelpers.CreateIndexFile(filePath, indexPath);
            }

            string line = ProgramHelpers.GetLine(filePath, indexPath, lineIndex);

            if (line != null)
            {
                Console.WriteLine(line);
            }
            else
            {
                Console.WriteLine($"Line {lineIndex} not found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}