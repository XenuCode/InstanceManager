// See https://aka.ms/new-console-template for more information

using Console = Colorful.Console;
using System.Diagnostics;

public class Program
{
    private static Process process;

    static void Main(string[] args)
    {
        process = new Process();
        ProcessStartInfo processStartInfo =
            new ProcessStartInfo("java", " -Xms1G -Xmx1G -jar /home/xenu/TESTS/server.jar nogui");
        processStartInfo.WorkingDirectory = "/home/xenu/TESTS/";
        process.StartInfo = processStartInfo;
        process.OutputDataReceived += ProcessOnOutputDataReceived;
        process.ErrorDataReceived += ProcessOnErrorDataReceived;
        process.Start();
        
        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        
        for (;;)
        {
            
            Thread.Sleep(1000);
        }
        
        process.WaitForExit();
    }

    private static void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        Console.WriteLine("SHUTTING DOWN !!!");
        process.CloseMainWindow();
    }

    private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs data)
    {
        Console.WriteLine(data.Data);
    }

    private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs data)
    {
        Console.WriteLine(data.Data);
    }
}
