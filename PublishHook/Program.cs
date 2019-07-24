using Agile.FrameworkNetCore.Log;
using System;
using System.Diagnostics;

namespace PublishHook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PublishHook is running now !");

            var server = new Agile.AServer.Server();
            server.AddHandler(new Agile.AServer.HttpHandler()
            {
                Method = "POST",
                Path = "/api/hook",
                Handler = (req, resp) =>
                {
                    string shell_name = req.Query.shell;
                    if (!string.IsNullOrEmpty(shell_name))
                    {
                        RunShell(shell_name);
                    }
                    return resp.Write("ok");
                }
            });

            server
                .SetIP("0.0.0.0")
                .SetPort(9000)
                .Run();

            Console.Read();
        }

        static void RunShell(string fileName)
        {
            var processStartInfo = new ProcessStartInfo(fileName) { RedirectStandardOutput = true };
            var process = Process.Start(processStartInfo);
            if (process == null)
            {
                Console.WriteLine("Can not run shell .");
            }
            else
            {
                using (var sr = process.StandardOutput)
                {
                    while (!sr.EndOfStream)
                    {
                        var str = sr.ReadLine();
                        Console.WriteLine(str);
                        Logger.Info(str);
                    }

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
            }
        }
    }
}
