using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private TcpClient client;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        private string str;
        private Thread thread;

        public delegate void updateLogDelegate(string newstr);
        public updateLogDelegate updateLog;

        void Form1_Load(object sender, EventArgs e)
        {
            updateLog = updateLogProc;
        }

        void updateLogProc(string newstr) {
            string text = str + logtext.Text;
            logtext.AppendText(text);
        }

        private void readLoop() {
            while (true) {
                string res = Read();
                Console.WriteLine(res);
                str = res;

                string text = "";
                this.Invoke(updateLog, str);
            }
        }

        private string Read() {
            string res = "";
            if (ns.CanRead)
            {
                int read = 0;
                byte[] bytes = new byte[client.ReceiveBufferSize];
                try
                {
                    do
                    {
                        read = ns.Read(bytes, 0, client.ReceiveBufferSize);
                        //res = reader.ReadLine();
                    } while (ns.DataAvailable);
                }
                catch (ObjectDisposedException dispose) { Console.WriteLine("object disposed :" + dispose.Message); }
                catch (IOException io) { Console.WriteLine("ioexception:" + io.Message); }
                catch (ArgumentOutOfRangeException argrange) { Console.WriteLine("argument out of :" + argrange.Message); }
                catch (ArgumentException arg) { Console.WriteLine("argument:" + arg.Message); }
                
                res = Encoding.ASCII.GetString(bytes, 0, read);
            }
            return res;
        }

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Close();
            thread.Join();
        }

        private void startbutton_Click(object sender, EventArgs e)
        {
            startbutton.Enabled = false;
            string host = IPTextBox.Text;
            int port = int.Parse(portTextBox.Text);
            client = new TcpClient(host, port);
            ns = client.GetStream();
            writer = new StreamWriter(ns);
            reader = new StreamReader(ns);
            string mes = "aaaaaa";
            byte[] tmp = Encoding.ASCII.GetBytes(mes);
            ns.Write(tmp, 0, tmp.Length);
            thread = new Thread(new ThreadStart(this.readLoop));
            thread.IsBackground = true;
            thread.Start();
            while (!thread.IsAlive) { }
        }
    }
}
