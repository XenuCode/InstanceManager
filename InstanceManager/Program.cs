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

    public static string _data;
    private static Process process;
    private static WebSocketServer wssv;
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
        Singleton.data = (string) data.Data.ToString();
    }

    private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs data)
    {
        CoolConsole.WriteLine(data.Data,Color.Green);
        Singleton.data = data.Data;
    }

    private static void SocketConnection()
    {
        wssv = new WebSocketServer ( "ws://127.0.0.1:3000");
        wssv.AddWebSocketService<Laputa> ("/Laputa");
        wssv.Start ();
        CoolConsole.WriteLine("Websocket Server Ready", Color.Green);
        CoolConsole.ReadKey (true);
        wssv.Stop ();
    }
    public class Laputa : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Send("sending info ...");
            string s = "sa";
            for (;;)
            {
                if (Singleton.data.ToString()!=s)
                {
                    CoolConsole.WriteLine("data:" + Singleton.data);
                    Send(Singleton.data.ToString());
                    s = Singleton.data.ToString();    
                }
            }
        }

        protected override void OnMessage (MessageEventArgs e)
        {
            Send("ASDHIOASD");
        }
    }
}
public sealed class Singleton
{
    private static readonly Lazy<Singleton> lazy =
        new Lazy<Singleton>(() => new Singleton());

    public static object data ="";
    public static Singleton Instance { get { return lazy.Value; } }

    

    private Singleton()
    {
    }
}