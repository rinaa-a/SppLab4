using System;
using System.IO;
using System.Threading.Tasks.Dataflow;

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
            ""
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

            var generateTestClasses = new TransformManyBlock<string, T>(async source =>
            {
                Console.WriteLine("Generating test classes ...");
                // TODO generate test classes
                
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxTasks
            });


            var writeToFile = new ActionBlock<T>(async testClass =>
            {
                Console.WriteLine("Writing {0} to file...", testClass.Name);

                using StreamWriter writer = File.CreateText(destPath);
                await writer.WriteAsync(testClass.value);
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
