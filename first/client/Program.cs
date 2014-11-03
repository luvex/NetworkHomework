using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace client_MX
{
    class Program
    {
        public static Socket mx_socket;
        static void Main(string[] args)
        {
            mx_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int UDPport = SendMessage.GetUDPPort("127.0.0.1", 8888, mx_socket);
            Console.WriteLine("The UDP port is : {0}.", UDPport);
            int choose=0;
            while(choose!=3)
            {
                displayOptions();
                choose = int.Parse(Console.ReadLine());
                switch (choose)
                {
                    case 1:
                        Console.WriteLine("Get the server time from the sever...");
                        SendMessage.GetServerTime(mx_socket);
                        break;
                    case 2:
                        mx_socket.Send(SendMessage.ProcessMsg("2"));
                        SendMessage.EchoMode("127.0.0.1", UDPport);
                        break;
                    case 3:
                        mx_socket.Send(SendMessage.ProcessMsg("3"));
                        SendMessage.ExitProgram(mx_socket);
                        Console.WriteLine("Stopping the client...");
                        break;
                    default:
                        Console.WriteLine("The input should be the number of the option!");
                        break;
                }
            }
            mx_socket.Close();
            Console.WriteLine("The client has been shut down!");
        }

        static void displayOptions()
        {
            Console.WriteLine("********************************************");
            Console.WriteLine("Please choose one of the options :");
            Console.WriteLine("1.Get server system information (TCP）");
            Console.WriteLine("2.Echo Mode（UDP）");
            Console.WriteLine("3.Exit the program");
            Console.WriteLine("********************************************");
        }
    }
}
