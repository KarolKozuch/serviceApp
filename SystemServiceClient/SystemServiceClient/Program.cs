using System;
using System.IO;
using System.IO.Pipes;

namespace SystemServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Named Pipe IPC - client*/
            const string pipeName = "SystemServicePipe";
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.In);

            Console.WriteLine("Attempting to connect to pipe...");
            pipeClient.Connect();

            Console.WriteLine("Connected");

            StreamReader stream = new StreamReader(pipeClient);

            string readData;

            while((readData = stream.ReadLine()) != null)
            {
                Console.WriteLine(readData);
            }

            Console.ReadLine();
        
        }
    }
}
