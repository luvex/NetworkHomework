using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server_MX
{
    class ReceiveMessage
    {
        public static void TCPReceiveMsg()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);//设置端口和IP
            socket.Bind(ipp);//创建终结点
            socket.Listen(10);//开始侦听，最多挂起数为10
            byte[] bytes = new byte[1024];
            while (true)
            {
                Console.WriteLine("等待连接……");
                Socket client = socket.Accept();//接收挂起的连接，并返回一个新的SOCKET
                Console.WriteLine("连接成功，正在读取消息。");
                client.Receive(bytes);//读取数据
                string messages = ParseData(bytes);//将数据转换为字符串
                Console.WriteLine(messages);
                if (messages.Equals("GET UDP PORT"))
                {
                    byte[] msg = ProcessMsg("6666");
                    client.Send(msg);
                }
                else
                {
                    byte[] msg = ProcessMsg("The input is wrong!");
                    client.Send(msg);
                }
                client.Close();
            }
        }
        public static string ParseData(byte[] data)
        {
            string temp = Encoding.UTF8.GetString(data, 0, data.Length);
            string MSG = temp.Split('|')[1].ToString();
            int length = int.Parse(temp.Split('|')[0].ToString());
            if (length == MSG.Length)
                return MSG;
            else
                return "数据传输不完整！";
        }
        public static byte[] ProcessMsg(string input)
        {
            int length = input.Length;
            string MSG = length.ToString() + '|' + input + '|';
            byte[] data = new byte[1024];
            data = Encoding.UTF8.GetBytes(MSG.ToCharArray());
            return data;
        }
        public static void UDPReceiveMsg(Socket UDPserverListen)
        {
            Socket UDPReceiveSocket = UDPserverListen;
            Console.WriteLine("The UDP SOCKET on {0}:{1} is waiting for a client...","127.0.0.1","6666");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            while (true)
            {
                byte[] data = new byte[1024];
                int recv = UDPReceiveSocket.ReceiveFrom(data, ref Remote);
                Console.WriteLine("Message Received From {0}: ", Remote.ToString());
                if (ParseData(data) == "[q]")
                    break;
                Console.WriteLine("The message from client is : {0} ", ParseData(data));
                data = ProcessMsg(ParseData(data).ToUpper());
                UDPReceiveSocket.SendTo(data, data.Length, SocketFlags.None, Remote);
            }
            Console.WriteLine("A UDP connection has been closed.");
        }
        
    }
}
