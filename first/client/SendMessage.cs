using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace client_MX
{
    class SendMessage
    {
        public static int GetUDPPort(string IP, int port,Socket mx_socket)
        {
            Socket TCPSendSocket = mx_socket;
            IPEndPoint ipp = new IPEndPoint(IPAddress.Parse(IP), port);
            TCPSendSocket.Connect(ipp);
            Console.WriteLine("连接成功！");
            Console.WriteLine("请输入 \"GET UDP PORT\" !");
            byte[] msg = new byte[1024];
            int UDPport = 0;
            while (true)
            {
                string input = Console.ReadLine();
                byte[] data = ProcessMsg(input);
                TCPSendSocket.Send(data);
                msg = new byte[1024];
                TCPSendSocket.Receive(msg);
                if (IsInt(ParseData(msg)))
                {
                    UDPport = int.Parse(ParseData(msg));
                    break;
                }
                else
                {
                    Console.WriteLine("The return value could not be a port!");
                }
            }
            return UDPport;
        }
        public static void GetServerTime(Socket mx_socket)
        {
            Socket getSeverTimeSocket = mx_socket;
            getSeverTimeSocket.Send(ProcessMsg("1"));
            byte[] data = new byte[1024];
            getSeverTimeSocket.Receive(data);
            Console.WriteLine("The server time is : {0}",ParseData(data));
        }
        public static void EchoMode(string IP,int port)
        {
            Socket UDPSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint mx_ipp = new IPEndPoint(IPAddress.Parse(IP), port);
            byte[] data = new byte[1024];
            while (true)
            {
                Console.WriteLine("**Please enter the message in lower case, and \"[q]\" for exit**");
                string input = Console.ReadLine();
                data = ProcessMsg(input);
                UDPSendSocket.SendTo(data, data.Length, SocketFlags.None, mx_ipp);
                if (input == "[q]")
                    break;
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Remote = (EndPoint)sender;
                data = new byte[1024];
                int recv = UDPSendSocket.ReceiveFrom(data, ref Remote);
                Console.WriteLine("Message received from {0}: ", Remote.ToString());
                Console.WriteLine("The upper case is ; {0} ",ParseData(data));
            }
            Console.WriteLine("Closing The UDP Port...");
            UDPSendSocket.Shutdown(SocketShutdown.Both);
            UDPSendSocket.Close();
        }
        public static void ExitProgram(Socket mx_socket)
        {
            mx_socket.Shutdown(SocketShutdown.Both);
            mx_socket.Close();
        }

        public static byte[] ProcessMsg(string input)
        {
            int length = input.Length;
            string MSG = length.ToString() + '|' + input + '|';
            byte[] data = new byte[1024];
            data = Encoding.UTF8.GetBytes(MSG.ToCharArray());
            return data;
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
        public static bool IsInt(string value)
        {
            try
            {
                int var1 = Convert.ToInt32(value);
                if (var1 >= 0 && var1 <= 65535)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
