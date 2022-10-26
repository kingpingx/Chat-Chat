using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace WpfDemo
{
    public class chat: INotifyPropertyChanged
    {
        private static bool isClientActive = true;
        private static string User2_IP = "192.168.2.37";
        private static int User2_Port = 1310, User1_Port = 1301;
        TcpClient client = new TcpClient("192.168.2.58", 1301);
        TcpListener listener = new TcpListener(IPAddress.Any, User1_Port);
        


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private string _incomingData;//="Client << ";

        private string _sendingData;

        public string IncomingData
        {
            get
            {
                return _incomingData;
            }
            set
            {
                this.GetHashCode();
                _incomingData = value;
                OnPropertyChanged("IncomingData");
            
            }
        }
        public string SendingData
        {
            get
            {
                return _sendingData;
            }
            set
            {
                    _sendingData = value;
                    OnPropertyChanged("SendingData");
        
            }
        }

        public void send( string messageTosend)
        {

            while (isClientActive)
            {
                NetworkStream stream = null;
                try
                {
                    //client = new TcpClient("192.168.2.58", 1301);
                    stream = client.GetStream();
                   
                    //string messageToSend = Console.ReadLine();
                    string messageToSend = "hello";
                    int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                    byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);
                    stream.Write(sendData, 0, sendData.Length);
                    if (messageToSend == "exit")
                    {
                        isClientActive = false;
                    }
                    

                }
                catch (Exception e)
                {
                    Console.WriteLine("failed to connect...");
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    if (client != null)
                    {
                        client.Close();
                    }
                }
            }
        }


        public void recieve()
        {
            while (isClientActive)
            {
            //    TcpListener listener = null;
            //    TcpClient client = null;
                NetworkStream stream = null;
                try
                {
                    //listener = new TcpListener(IPAddress.Any, User1_Port);
                    listener.Start();
                    //Console.WriteLine("Waiting for a connection.");
                    client = listener.AcceptTcpClient();
                    //Console.WriteLine("User2 accepted.....\n");
                    stream = client.GetStream();
                    StreamReader sr = new StreamReader(client.GetStream());
                    StreamWriter sw = new StreamWriter(client.GetStream());

                    while (isClientActive)
                    {
                        byte[] buffer = new byte[1024];
                        int recv = stream.Read(buffer, 0, buffer.Length);
                        if (recv > 0)
                        {
                            string request = Encoding.UTF8.GetString(buffer, 0, recv);
                            if (request == "exit")
                            {
                                isClientActive = false;
                                IncomingData += "User2 has left the chat....." + '\n';
                            }
                            else IncomingData += "Client << " + request + '\n';
                        }

                    }

                }
                catch (Exception e)
                {
                    IncomingData += '\n' + "Something went wrong in stream.";

                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    if (client != null)
                    {
                        client.Close();
                    }
                    if (listener != null)
                    {
                        listener.Stop();
                    }
                }
            }
        }

        public void Main(string args)
        {
            Thread startingThread = new Thread(()=>send("hello"));
            Thread recievingThread = new Thread(recieve);
            recievingThread.Start();
            startingThread.Start();


        }

    }

}
