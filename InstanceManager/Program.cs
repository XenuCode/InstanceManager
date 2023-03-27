// See https://aka.ms/new-console-template for more information

using CoolConsole = Colorful.Console;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Colorful;
using InstanceManager;
using Newtonsoft.Json;
using SockNet.ServerSocket;
using SuperSocket.Channel;
using WebSocketSharp;
using WebSocketSharp.Server;
using Console = System.Console;

public class Program
{
    private static StreamWriter input;

    static void Main(string[] args)
    {
        CoolConsole.WriteAscii("INSTANCE MANAGER",Color.Green);
        CoolConsole.WriteAscii("0.1.0",Color.Green);
        We we = new We();
        var result = we.Start();
        for (;;)
        {
            input.WriteLine(Console.ReadLine());
        }
    }

    public class We
    {
        public static WebSocketServer wssv = new WebSocketServer();
        private static string _data ="";
        private static Process process;
        public async Task<Task> Start()
        {
            SocketConnection();
            StartupConfig startupConfig = new StartupConfig();
            using (StreamReader file = File.OpenText("instance_config.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                startupConfig= (StartupConfig)serializer.Deserialize(file, typeof(StartupConfig));
            }
            CoolConsole.WriteLine("Running: "+ startupConfig.instance_path,Color.Chartreuse );
            CoolConsole.WriteLine("Running with: "+ startupConfig.Xmx+"MB of allocated RAM",Color.Chartreuse );
            CoolConsole.WriteLine("Running with: "+ startupConfig.Xms+"MB of startup RAM",Color.Chartreuse );
            process = new Process();
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo("java", "-Xms"+startupConfig.Xms+"M -Xmx"+startupConfig.Xmx+"M -jar "+startupConfig.instance_path+" nogui");
            processStartInfo.WorkingDirectory = "/home/xenu/TESTS/";
            process.StartInfo = processStartInfo;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true; // Is a MUST!
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            Thread.Sleep(2000);
            input = process.StandardInput;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            //socket server

            await process.WaitForExitAsync();
            return Task.CompletedTask;
        }
        private static void CurrentDomainOnProcessExit(object? sender, EventArgs e)
        {
            CoolConsole.WriteLine("SHUTTING DOWN !!!");
            process.CloseMainWindow();
        }

        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs data)
        {
            string str = data.Data;
            CoolConsole.WriteLine("sending:"+str,Color.Red);
            wssv.WebSocketServices["/ConsoleLogging"].Sessions.Broadcast(str);
        }

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs data)
        {
            string str = data.Data;
            CoolConsole.WriteLine(str,Color.Blue);
            wssv.WebSocketServices["/ConsoleLogging"].Sessions.Broadcast(str);
        }
        private static void SocketConnection()
        {
            wssv = new WebSocketServer ( "ws://127.0.0.1:3141");
            wssv.AddWebSocketService<ConsoleLogging> ("/ConsoleLogging");
            wssv.Start ();
            CoolConsole.WriteLine("Websocket Server Ready", Color.Green);
            CoolConsole.WriteLine("Listening on ws://0.0.0.0:3141/ConsoleLogging", Color.Chartreuse);
        }
        public class ConsoleLogging : WebSocketBehavior
        {
            protected override void OnOpen()
            {
                Send("sending info ...");
                string s = "sa"; 
                //CoolConsole.WriteLine("connected client" + _data,Color.Green);
            }

            protected override void OnMessage (MessageEventArgs e)
            {
                CoolConsole.WriteLine("remote> "+ e.Data,Color.Aqua);
                input.WriteLine(e.Data);
            }
        }
    }
    
    
}