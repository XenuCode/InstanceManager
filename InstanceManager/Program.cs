// See https://aka.ms/new-console-template for more information

using CoolConsole = Colorful.Console;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Colorful;
using SockNet.ServerSocket;
using WebSocketSharp;
using WebSocketSharp.Server;

public class Program
{
    private static Process process;
    static void Main(string[] args)
    {
        CoolConsole.WriteAscii("Instance Manager ",Color.Green);
        process = new Process();
        ProcessStartInfo processStartInfo =
            new ProcessStartInfo("java", " -Xms1G -Xmx1G -jar /home/xenu/TESTS/server.jar nogui");
        processStartInfo.WorkingDirectory = "/home/xenu/TESTS/";
        process.StartInfo = processStartInfo;
        process.StartInfo.RedirectStandardInput = true;
        process.OutputDataReceived += ProcessOnOutputDataReceived;
        process.ErrorDataReceived += ProcessOnErrorDataReceived;
        process.Start();
        Thread.Sleep(2000);
        var input = process.StandardInput;
        
        //socket server
        
        
        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

        new Thread( SocketConnection).Start();
         
        for (;;)
        {
            string val = CoolConsole.ReadLine();
            input.WriteLine(val);
        }
        
        process.WaitForExit();
    }

    private static void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        CoolConsole.WriteLine("SHUTTING DOWN !!!");
        process.CloseMainWindow();
    }

    private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs data)
    {
        CoolConsole.WriteLine(data.Data,Color.Red);
    }

    private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs data)
    {
        CoolConsole.WriteLine(data.Data,Color.Green);
    }

    private static void SocketConnection()
    {
        CoolConsole.WriteLine("Websocket Server Ready", Color.Green);
        var httpsv = new HttpServer (4649);

        /*httpsv.AddWebSocketService<Echo> ("/Echo");
        httpsv.AddWebSocketService<Chat> ("/Chat");
        httpsv.AddWebSocketService<Chat> ("/ChatWithNyan", s => s.Suffix = " Nyan!");*/
        
    }
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage (MessageEventArgs e)
        {
            var msg = e.Data == "BALUS"
                ? "Are you kidding?"
                : "I'm not available now.";

            Send (msg);
        }
    }
}
