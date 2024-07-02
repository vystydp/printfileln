using System;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace MyProject.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private string testFilePath;
        private string indexPath;
        private int ChunkSize = 100;

        [SetUp]
        public void Setup()
        {
            testFilePath = "test.txt";
            indexPath = testFilePath + ".idx";
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(testFilePath))
            {
                //File.Delete(testFilePath);
            }

            if (File.Exists(indexPath))
            {
                //File.Delete(indexPath);
            }
        }

        [Test]
        public void TestCreateIndexFile()
        {
            // ARRANGE
            string[] lines = new string[]
            {
                "Line 1",
                "Line 2",
                "Line 3",
                "Line 4",
                "Line 5",
                "Line 6",
                "Line 7",
                "Line 8",
                "Line 9",
                "Line 10"
            };

            File.WriteAllLines(testFilePath, lines);

            // Ensure index file does not exist
            if (File.Exists(indexPath))
            {
                File.Delete(indexPath);
            }

            // ACT
            ProgramHelpers.CreateIndexFile(testFilePath, indexPath);

            // ASSERT
            Assert.True(File.Exists(indexPath));
        }
    }
}
