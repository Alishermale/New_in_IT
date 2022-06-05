using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace customs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Tables> result = new List<Tables>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<Tables> result = new List<Tables>();

            //result.Add(new Tables("0001", "Живая Лошадь", "97%"));

            //grid.ItemsSource = result;

        }
        private void ExitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();   
        }
        private void MinButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        public static string StartClient(string text)
        {
            byte[] bytes = new byte[1024];
            string result = "";
            try
            {
                IPHostEntry host = Dns.GetHostEntry("192.168.90.180");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.UTF8.GetBytes(text);

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    result = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    MessageBox.Show("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    MessageBox.Show("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string fileName = @"cliet.py" + " \"" + SearchBox.Text.Replace("\"", "") + "\" "+ "6";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("python", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            //MessageBox.Show(output);
            result.Clear();
            grid.ItemsSource = null;
            grid.DataContext = null;
            grid.Items.Refresh();
            output = output.Replace('"', ' ').Replace("{", "").Replace("}", "");
            string[] str = output.Split(',');
            
            string[] postStr;
            
            for(int i=0; i < str.Length; i++)
            {
                
                postStr = str[i].Split(':');
                string n_str = new string(postStr[1].Where(t => char.IsDigit(t)).ToArray()).Substring(1, 2);
                string x_str = new string(postStr[0].Where(t => char.IsDigit(t)).ToArray());
                if (n_str[0] == '0' && n_str[1] == '0')
                    n_str = "меньше 0";
                if (n_str[0] == '0')
                    n_str = n_str.Substring(1) ;

                while (x_str.Length < 4)
                    x_str = "0" + x_str;

                result.Add(new Tables(new string(postStr[0].Where(t => char.IsDigit(t)).ToArray()), "", n_str + "%"));
            }
            grid.ItemsSource = result;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string filename = ofd.FileName;
            string[] fileText;
            if (!(filename == ""))
                fileText = System.IO.File.ReadAllLines(filename);
            else
                MessageBox.Show("Не получилось прочитать файл");

        }
    }
}
