using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using TestsGenerator;

namespace ConsoleApp
{
    class Program
    {
        private const int maxFilesToLoad = 5;
        private const int maxTasks = 5;
        private const int maxFilesToWrite = 5;

        private const string destPath = "D:\\5sem\\spp\\lab4\\";
        private static readonly string[] filesPathes =
        {
            "D:\\5sem\\spp\\SppLab1\\SppLab1\\SppLab1\\Tracer.cs",
            "D:\\5sem\\spp\\SppLab1\\SppLab1\\SppLab1\\ThreadTracer.cs",
            "D:\\5sem\\spp\\SppLab1\\SppLab1\\SppLab1\\MethodTracer.cs",
            "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\1.cs",
            "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\no_methods.cs",
            "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\valid.cs"
        };

        static void Main()
        {
            var loadSourceFiles = new TransformBlock<string, string>(async path =>
            {
                Console.WriteLine("Loading {0} ...", path);

                using StreamReader reader = File.OpenText(path);
                return await reader.ReadToEndAsync();
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxFilesToLoad
            });

            var generateTestClasses = new TransformBlock<string, TestGenerator.Test>(async source =>
            {
                Console.WriteLine("Generating test classes ...");
                var generator = new TestGenerator();
                return await generator.GetTests(source);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxTasks
            });


            var writeToFile = new ActionBlock<TestGenerator.Test>(async testClass =>
            {
                Console.WriteLine("Writing {0} to file...", testClass.fileName);

                using StreamWriter writer = File.CreateText(destPath + testClass.fileName);
                await writer.WriteAsync(testClass.code);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxFilesToWrite
            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            loadSourceFiles.LinkTo(generateTestClasses, linkOptions);
            generateTestClasses.LinkTo(writeToFile, linkOptions);

            foreach (var item in filesPathes)
            {
                loadSourceFiles.Post(item);
            }

            // Mark the head of the pipeline as complete.
            loadSourceFiles.Complete();

            // Wait for the last block in the pipeline to process all messages.
            writeToFile.Completion.Wait();
        }
    }
}
