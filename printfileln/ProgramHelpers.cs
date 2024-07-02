using System;
using System.Text;

public static class ProgramHelpers
{
    internal const int TestLinesNum = 999999; 
    internal const int ChunkSize = 100; 

    public static void CreateIndexFile(string filePath, string indexPath)
    {
        try
        {
            using (var reader = new StreamReader(filePath))
            using (var writer = new BinaryWriter(File.Open(indexPath, FileMode.Create)))
            {
                long currentOffset = 0;
                int lineCount = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (lineCount % ChunkSize == 0)
                    {
                        writer.Write(currentOffset);
                    }

                    currentOffset += line.Length + Environment.NewLine.Length;
                    lineCount++;
                }

                // Write the total number of lines
                writer.Write(currentOffset); // Write the offset of the end of the last chunk
                writer.Write(lineCount); // Write the total number of lines
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    public static string GetLine(string filePath, string indexPath, int lineIndex)
    {
        long startOffset = 0;
        int totalLines = 0;
        int lineOffsetInChunk = lineIndex % ChunkSize;

        using (var fileStream = File.Open(indexPath, FileMode.Open)) 
        {
            using (var indexReader = new BinaryReader(fileStream, Encoding.UTF8, false))
            {
                long totalOffsets = (indexReader.BaseStream.Length - sizeof(int)) / sizeof(long);
                long chunkCount = totalOffsets - 1;

                indexReader.BaseStream.Seek(indexReader.BaseStream.Length - sizeof(int), SeekOrigin.Begin);
                totalLines = indexReader.ReadInt32();

                if (lineIndex >= totalLines)
                {
                    throw new IndexOutOfRangeException("Line index out of file scope");
                }

                int chunkIndex = lineIndex / ChunkSize;

                indexReader.BaseStream.Seek(chunkIndex * sizeof(long), SeekOrigin.Begin);
                startOffset = indexReader.ReadInt64();
            }
        }
        using (var fileReader = new StreamReader(File.Open(filePath, FileMode.Open)))
        {
            fileReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);

            for (int i = 0; i <= lineOffsetInChunk; i++)
            {
                string line = fileReader.ReadLine();
                if (i == lineOffsetInChunk)
                {
                    return line;
                }
            }
        }

        return String.Empty;
    }

    public static string[] GenerateRandomLines()
    {
        string[] lines = new string[TestLinesNum];
        for (int i = 0; i < TestLinesNum; i++)
        {
            int[] range = (i < TestLinesNum / 3)
                ? [1, 1000] : (i < TestLinesNum / 2)
                ? [1000, 100000] : [100000, TestLinesNum];
            lines[i] = $"test {i}" + new Random().Next(range[0], range[1]);
        }
        return lines;
    }
}
