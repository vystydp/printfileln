using System;
using System.Text;

public static class ProgramHelpers
{
    internal const int ChunkSize = 100; 

    public static void CreateIndexFile(string filePath, string indexPath)
    {
        try
        {
            using (var reader = new StreamReader(filePath))
            using (var writer = new BinaryWriter(File.Open(indexPath, FileMode.Create)))
            {
                long currentOffset = 0;
                long lineCount = 0;
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

    public static string GetLine(string filePath, string indexPath, long lineIndex)
    {
        long startOffset = 0;
        long totalLines = 0;
        long lineOffsetInChunk = lineIndex % ChunkSize;

        using (var fileStream = File.Open(indexPath, FileMode.Open)) 
        {
            using (var indexReader = new BinaryReader(fileStream, Encoding.UTF8, false))
            {
                long totalOffsets = (indexReader.BaseStream.Length - sizeof(long)) / sizeof(long);
                long chunkCount = totalOffsets - 1;

                indexReader.BaseStream.Seek(indexReader.BaseStream.Length - sizeof(long), SeekOrigin.Begin);
                totalLines = indexReader.ReadInt64();

                if (lineIndex >= totalLines)
                {
                    throw new IndexOutOfRangeException("Line index out of file scope");
                }

                long chunkIndex = lineIndex / ChunkSize;

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
}
