using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfDemo
{

    public partial class MainWindow : Window
    {
        Program programobj = null;

        public MainWindow()
        {
            InitializeComponent();
            
            programobj = new Program();
            programobj.EnableToggler += toggler;
            this.DataContext = programobj;
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

        private void startconnection(object sender, RoutedEventArgs e)
        {
            if (!programobj.IsConnected && serverPort.Text != "" && serverIP.Text != "")
            {
                if (programobj.SetUpConnection(serverIP.Text, Convert.ToInt32(serverPort.Text)))
                {
                    programobj.startrecieve();
                    MessageBox.Show("You are Connected !");
                    connectionbutton.Content = "Stop";
                    programobj.IsConnected = true;
                }
                else
                {
                    MessageBox.Show("Connection does not formed !");
                    programobj.IsConnected = false;
                }
            }
            else if (programobj.IsConnected)
            {
                MessageBox.Show("Stopping Connection............");
                programobj.stop();
                programobj.IsConnected = false;
            }
        }

        private void sendingText_Touch(object sender, EventArgs e)
        {
            sendingText.Clear();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sendingText.Text != "")
            {
                programobj.IncomingData += "Sender << " + sendingText.Text + '\n';
                programobj.send(sendingText.Text);
                sendingText.Clear();
                chatBox.ScrollToEnd();

            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && sendingText.Text != "")
            {
                programobj.IncomingData += "Sender << " + sendingText.Text + '\n';
                programobj.send(sendingText.Text);
                sendingText.Clear();
                chatBox.ScrollToEnd();

            }
        }
        public void toggler(bool value)
        {
            if (value == true)
            {
                
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    status.Text = "You are connected";
                    status.Background = Brushes.Green;
                    connectionbutton.Content = "Stop";
                }));
            }
            else
            {
               
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    status.Text = "You are not connected";
                    status.Background = Brushes.Red;
                    connectionbutton.Content = "Start";
                }));
            }
        }
        public void status_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Client = " + Convert.ToString(programobj.client) +
                "\nlistener = " + Convert.ToString(programobj.listener) +
                "\nStream = " + Convert.ToString(programobj.stream));
        }

    }
}
