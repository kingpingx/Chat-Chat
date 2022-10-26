using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfDemo
{
    public class Program : INotifyPropertyChanged
    {
        public delegate void PropertyToggler(bool value);

        public event PropertyToggler EnableToggler;

        public Program()
        {
            OpenListenerHandler();
            broadcast.OpenBroadcaster();

        }

        public void OpenListenerHandler()
        {
            Thread ListenerThread = new Thread(OpenListener);
            ListenerThread.Start();
        }

        public static string IPConvertor(EndPoint IPAddresswithPort)
        {
            string UserIPAdress = (IPAddresswithPort.ToString().Split(new string[1] { ":" }, StringSplitOptions.None))[0];
            return UserIPAdress;
        }



        public TcpListener listener = new TcpListener(IPAddress.Any, 1304);
        public TcpClient client = null;
        public NetworkStream stream = null;

        private string _incomingData;
        private string _sendingData;

        private bool _isConnected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private void Program_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public bool checkstatus(IPAddress IP)
        {
            var find = broadcast.connections.FirstOrDefault(x => x.RemoteIP == IP);
            if (find != null)
            {
                return find.status;
            }
            return false;
        }

        public void OpenListener()
        {

            listener.Start();
            client = listener.AcceptTcpClient();
            broadcast.connection newconnection = new broadcast.connection(((IPEndPoint)client.Client.RemoteEndPoint).Address, true);
            broadcast.connections.Add(newconnection);
            stream = client.GetStream();
            listener.Stop();
            startrecieve();

            IsConnected = true;

        }

        public void startrecieve()
        {
            Thread recievingThread = new Thread(recieve);
            recievingThread.Start();
        }

        public void OnPropertyToggle(bool value)
        {
            if (EnableToggler != null)
            {
                EnableToggler.Invoke(value);
            }
        }


        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
                OnPropertyToggle(value);
            }

        }

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

        public bool SetUpConnection(string ClientIP, int ClientPort)
        {
            try
            {
                client = new TcpClient(ClientIP, ClientPort);
                stream = client.GetStream();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void send(string messageToSend)
        {
            try
            {
                int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);
                stream.Write(sendData, 0, sendData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nfailed to connect...\n");
                if (stream != null)
                {
                    stream.Dispose();
                    stream.Close();
                    IsConnected = false;
                }
            }
        }

        public void recieve()
        {

            while (true)
            {
                try
                {

                    byte[] buffer = new byte[1024];
                    int recv = stream.Read(buffer, 0, buffer.Length);
                    if (recv > 0)
                    {
                        string request = Encoding.UTF8.GetString(buffer, 0, recv);
                        IncomingData += "Client << " + request + '\n';
                    }
                }
                catch (Exception e)
                {
                    IncomingData += "\nSomething went wrong in stream\n";
                    if (stream != null)
                    {
                        stream.Dispose();
                        stream.Close();
                        IsConnected = false;
                        OpenListenerHandler();
                        Thread.CurrentThread.Abort();
                    }
                    break;
                }
            }

        }

        public void stop()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream.Close();
                stream = null;
            }
            client.Close();
            OpenListenerHandler();

        }
    }
}

