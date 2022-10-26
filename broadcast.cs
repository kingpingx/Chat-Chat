using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfDemo
{
    class broadcast
    {
        public static HashSet<connection> connections = new HashSet<connection>();

        public class connection
        {
            public IPAddress RemoteIP;
            public bool status = false;

            public connection(IPAddress RemoteIP, bool status)
            {
                this.RemoteIP = RemoteIP;
                this.status = status;
            }
        }



        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        public static void OpenBroadcaster()
        {
           Thread StartBroadCast = new Thread(BroadcastMessage);
           Thread StartBroadcastRecieve = new Thread(ReceiveMessages);
           StartBroadCast.Start();
           StartBroadcastRecieve.Start();
        }


        public static IPEndPoint e = new IPEndPoint(IPAddress.Any, 9000);
        public static UdpClient u = new UdpClient(e);
        static IPAddress UserSystemIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];

        public static void BroadcastMessage()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.2.58"), 8000);
            byte[] bytes = Encoding.ASCII.GetBytes("BroadCast Message.........");
            while (true)
            {
                client.Send(bytes, bytes.Length, ip);
                Thread.Sleep(1000);
            }

        }

        public static void ReceiveMessages()
        {
            UdpState s = new UdpState();
            s.e = e;
            s.u = u;

            u.BeginReceive(ReceiveCallback, s);

        }

        //Recieve BroadCast Callback
        public static void ReceiveCallback(IAsyncResult ar)
        {
            u = ((UdpState)(ar.AsyncState)).u;
            e = ((UdpState)(ar.AsyncState)).e;
            byte[] receiveBytes = u.EndReceive(ar, ref e);


            IPAddress ClientSystemIP = e.Address;

            if (ClientSystemIP.ToString() == UserSystemIP.ToString())
            {
                string receiveString = Encoding.ASCII.GetString(receiveBytes);
                if (receiveString == "BroadCast Message.........")
                {
                    connections.Add(new connection(e.Address, false));
                }

            }
                u.BeginReceive(ReceiveCallback, ar.AsyncState);

        }
    }
}
