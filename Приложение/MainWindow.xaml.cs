﻿using System;
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
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        public static string StartClient(string text)
        {
            TcpClient clientSocket = new TcpClient();
            clientSocket.Connect("192.168.90.180", 5000);

            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = Encoding.UTF8.GetBytes(text + "\u0017" + 3.ToString());
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            byte[] inStream = new byte[10025];
            serverStream.Read(inStream, 0, inStream.Length);
            return Encoding.UTF8.GetString(inStream);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string output = StartClient(SearchBox.Text);

            //MessageBox.Show(output);
            result.Clear();
            grid.ItemsSource = null;
            grid.DataContext = null;
            grid.Items.Refresh();
            if (output == "")
                MessageBox.Show("Не получилось :/");
            else
            {
                output = output.Replace('"', ' ').Replace("{", "").Replace("}", "").Replace("[", "");
                string[] str = output.Split(',');

                string[] postStr;

                for (int i = 0; i < str.Length; i++)
                {

                    postStr = str[i].Split(':');
                    string n_str = new string(postStr[1].Where(t => char.IsDigit(t)).ToArray()).Substring(1, 2);
                    string x_str = new string(postStr[0].Where(t => char.IsDigit(t)).ToArray());
                    if (n_str[0] == '0' && n_str[1] == '0')
                        n_str = "меньше 0";
                    if (n_str[0] == '0')
                        n_str = n_str.Substring(1);

                    while (x_str.Length < 4)
                        x_str = "0" + x_str;

                    result.Add(new Tables(x_str, "", n_str + "%"));
                }
                grid.ItemsSource = result;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string filename = ofd.FileName;
            string[] fileText;
            if (!(filename == ""))
            {
                fileText = System.IO.File.ReadAllLines(filename);
                List<Tables> result4 = new List<Tables>();
                for (int i = 0; i < fileText.Length; i++)
                {
                    eventsSearchBox(result4, fileText[i]); 
                }
                grid.ItemsSource = result4;
                MessageBox.Show("Успешно");
            }
            else
                MessageBox.Show("Не получилось прочитать файл");

        }
        List<Tables> eventsSearchBox(List<Tables> a, string text)
        {
            string fileName = @"cliet.py" + " \"" + SearchBox.Text.Replace("\"", "") + "\" " + "6";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("python", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = StartClient(text);//p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            if (output == "")
                MessageBox.Show("Не получилось :/");
            else
            {
                output = output.Replace('"', ' ').Replace("{", "").Replace("}", "").Replace("[", "");
                string[] str = output.Split(',');

                string[] postStr;

                for (int i = 0; i < str.Length; i++)
                {

                    postStr = str[i].Split(':');
                    string n_str = new string(postStr[1].Where(t => char.IsDigit(t)).ToArray()).Substring(1, 2);
                    string x_str = new string(postStr[0].Where(t => char.IsDigit(t)).ToArray());
                    if (n_str.Length >= 2)
                        if (n_str[0] == '0' && n_str[1] == '0')
                            n_str = "меньше 0";
                    if (n_str[0] == '0')
                        n_str = n_str.Substring(1);

                    while (x_str.Length < 4)
                        x_str = "0" + x_str;

                    a.Add(new Tables(x_str, text, n_str + "%"));
                }

            }
            return a;
        }
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(null, null);
        }
    }
}
