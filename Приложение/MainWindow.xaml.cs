using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace customs
{
    public class Slovar
    {
        string key;
        string value;
    }
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
        public void loadJson()
        {
            using (StreamReader r = new StreamReader("file.json"))
            {
                string json = r.ReadToEnd();
                //List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
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
            /////////////////////////////////////////////////////////////////////////////////////////////
            string text = SearchBox.Text;
            if (Double.TryParse(text, out double tmp) || String.IsNullOrWhiteSpace(text))
            {
                text = text.Trim();
                if (text.Length < 4)
                {
                    for (int i = text.Length; i < 4; i++)
                        text = "0" + text;
                }

                return;
            }

            string output = StartClient(SearchBox.Text);

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

                    string n_str = new string(postStr[1].Where(t => char.IsDigit(t)).ToArray());

                    if (n_str[0] != '1')
                        n_str = n_str.Substring(1, 2);
                    else
                        n_str = n_str.Substring(0, 3);

                    string x_str = new string(postStr[0].Where(t => char.IsDigit(t)).ToArray());

                    while (x_str.Length < 4)
                        x_str = "0" + x_str;

                    ///..........................................................
                    if (!(int.Parse(n_str) < 1))
                        result.Add(new Tables(x_str, SearchBox.Text, n_str + "%"));
                    ///..........................................................
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
                    filldatatable(fileText[i]);

                    //eventsSearchBox(result4, fileText[i]);
                }
                //grid.ItemsSource = result4;
                MessageBox.Show("Успешно");
            }
            else
                MessageBox.Show("Не получилось прочитать файл");
        }
        private void filldatatable(string input_str)
        { 
            string output = StartClient(input_str);

            //result.Clear();
            //grid.ItemsSource = null;
            //grid.DataContext = null;

            grid.Items.Refresh();
            if (output == "")
                MessageBox.Show("Не получилось :/");
            else
            {
                output = output.Replace('"', ' ').Replace("{", "").Replace("}", "").Replace("[", "");

                string[] str = output.Split(',');
                string[] postStr;

                postStr = str[0].Split(':');
                try {

                    string n_str = new string(postStr[1].Where(t => char.IsDigit(t)).ToArray());
                    string x_str = new string(postStr[0].Where(t => char.IsDigit(t)).ToArray());

                    if (n_str[0] != '1')
                        n_str = n_str.Substring(1, 2);
                    else
                        n_str = n_str.Substring(0, 3);



                    while (x_str.Length < 4)
                        x_str = "0" + x_str;


                    result.Add(new Tables(x_str, input_str, n_str + "%"));
                    grid.ItemsSource = result;
                }
                catch {
                    Console.WriteLine(postStr[0].ToString());
                }
            }
        }
        /*
        List<Tables> eventsSearchBox(List<Tables> a, string text)
        {

            string output = StartClient(text);


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
                    string n_str = new string(postStr[1].Where(t => char.IsDigit(t)).ToArray());
                    if( n_str[0] != '1')
                        n_str = n_str.Substring(1, 2);
                    //else
                    //    n_str = n_str.Substring(0, 2);
                    string x_str = new string(postStr[0].Where(t => char.IsDigit(t)).ToArray());


                    while (x_str.Length < 4)
                        x_str = "0" + x_str;

                    a.Add(new Tables(x_str, text, n_str + "%"));
                }

            }
            return a;
        }
        */
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(null, null);
        }
    }
}
