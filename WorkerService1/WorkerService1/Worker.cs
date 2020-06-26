using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.IO.Pipes;
using System.IO;
using System.ServiceModel.Channels;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                /* APP here */

                /* Runtime info */
                string dotNetCoreVersion;
                dotNetCoreVersion = RuntimeInformation.FrameworkDescription;
                _logger.LogInformation("Framework version: {v}", dotNetCoreVersion);

                string osArchitecture;
                osArchitecture = RuntimeInformation.OSArchitecture.ToString();
                _logger.LogInformation("OS architecture: {v}", osArchitecture);

                string osDescription;
                osDescription = RuntimeInformation.OSDescription;
                _logger.LogInformation("OS description: {v}", osDescription);


                /* Prepare frame to send via pipestream */

                string frame = System.String.Format("Framework version: {0}\nOs architecture: {1}\nOs description: {2}",dotNetCoreVersion, osArchitecture, osDescription);

                /* Named Pipe IPC - server */
                const string pipeName = "SystemServicePipe";
                NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out);
                Console.WriteLine("Pipe server created.");

                /* Wait for client */
                Console.WriteLine("Waiting for client...");
                pipeServer.WaitForConnection();

                Console.WriteLine("Client connected");

                try
                {
                    StreamWriter stream = new StreamWriter(pipeServer);

                    stream.AutoFlush = true;
                    stream.Write(frame);

                }
                catch(IOException e)
                {
                    Console.WriteLine("Exception: {error}", e.Message);
                }
                pipeServer.Close();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
