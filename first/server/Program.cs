using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server_MX
{

    class Program
    {
        //创建两个监听socket
        static Socket TCPserverListen;
        static Socket UDPserverListen;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            //启动TCP监听
            TCPserverListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint mx_ippt = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            TCPserverListen.Bind(mx_ippt);
            TCPserverListen.Listen(10);
            //启动UDP监听
            UDPserverListen = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint mx_ippu = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
            UDPserverListen.Bind(mx_ippu);
            Console.WriteLine("The server is starting...");
            //创建监听线程，每当有新的连接时建立一个新的线程用于处理
            Thread mx_thread = new Thread(new ThreadStart(WatchConnect));
            mx_thread.Name = "WatchThread";
            mx_thread.Start();   
        }

        //使用异步传输，保证并行处理客户端请求
        static void WatchConnect()
        {
            try
            {
                Console.WriteLine("waiting for connecting...");
                while(true)
                {
                    allDone.Reset();
                    TCPserverListen.BeginAccept(new AsyncCallback(OnConnectRequest), TCPserverListen);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Thread Wrong!");
            }
        }
        //异步回调函数
        static void OnConnectRequest(IAsyncResult ar)
        {
            allDone.Set();
            byte[] bytes = new byte[1024];
            Socket tserver = (Socket)ar.AsyncState;
            Socket client = tserver.EndAccept(ar);
            Console.WriteLine("Connection from TCP client 127.0.0.1:8888 accepted.");
            //等待输入GET UDP PORT口令
            while (true)
            {
                try
                {
                    client.Receive(bytes);
                }
                catch(SocketException se)
                {
                    break;
                }
                string messages = ReceiveMessage.ParseData(bytes);
                Console.WriteLine("Request \"{0}\" from 127.0.0.1:8888 by TCP", messages);
                if (messages.Equals("GET UDP PORT"))
                {
                    Console.WriteLine("Send UDP Port {0} to client...", 6666);
                    byte[] msg = ReceiveMessage.ProcessMsg("6666");
                    client.Send(msg);
                    break;
                }
                else
                {
                    Console.WriteLine("The request can not be processed!");
                    byte[] msg = ReceiveMessage.ProcessMsg("The request can not be processed!");
                    client.Send(msg);
                }
            }
            //传udp端口号成功，等待用户输入功能选项
            int option = 0;
            byte[] ByteOption = new byte[1024];
            while (option != 3)
            {
                ByteOption = new byte[1024];
                try
                {
                    client.Receive(ByteOption);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("The connection was broken unexpected!");
                    break;
                }
                option = int.Parse(ReceiveMessage.ParseData(ByteOption));
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Send the server time to the client...");
                        client.Send(ReceiveMessage.ProcessMsg(DateTime.Now.ToString()));
                        break;
                    case 2:
                        ReceiveMessage.UDPReceiveMsg(UDPserverListen);
                        break;
                    case 3:
                        Console.WriteLine("The client is stopping!");
                        break;
                    default:
                        client.Send(ReceiveMessage.ProcessMsg(""));
                        Console.WriteLine("The input should be the number of the option!");
                        break;
                }
            }
            Console.WriteLine("The connection has been disconnected!");
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        
    }
    
}
