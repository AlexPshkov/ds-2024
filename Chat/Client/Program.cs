using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    public static void StartClient()
    {
        try
        {
            // Разрешение сетевых имён
            IPAddress ipAddress = IPAddress.Loopback;
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // CREATE
            Socket sender = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                // CONNECT
                sender.Connect(remoteEP);

                Console.WriteLine("Удалённый адрес подключения сокета: {0}",
                    sender.RemoteEndPoint.ToString());

                // Подготовка данных к отправке
                byte[] msg = Encoding.UTF8.GetBytes("Hello, world!<EOF>");

                // SEND
                int bytesSent = sender.Send(msg);

                // RECEIVE
                byte[] buf = new byte[1024];
                int bytesRec = sender.Receive(buf);

                Console.WriteLine("Ответ: {0}",
                    Encoding.UTF8.GetString(buf, 0, bytesRec));

                // RELEASE
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void Main(string[] args)
    {
        StartClient();
    }
}