using System;
using System.Collections;       //Untuk array List
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Reflection.Emit;
namespace Send___Receive_Server
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private ArrayList alSockets;
        public Form1()
        {
            InitializeComponent();
        }
        // Event handler yang dijalankan saat form dimuat
        private void Form1_Load(object sender, System.EventArgs e)
        {
            IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
            lblStatus.Text = "My IP address is " +
            IPHost.AddressList[0].ToString();
            alSockets = new ArrayList();
            Thread thdListener = new Thread(new
            ThreadStart(listenerThread));
            thdListener.Start();
        }
        // Thread yang mendengarkan koneksi klien
        public void listenerThread()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 8080);
            tcpListener.Start();

            try
            {
                while (true)
                {
                    Socket handlerSocket = tcpListener.AcceptSocket();
                    if (handlerSocket.Connected)
                    {
                        // Menggunakan Invoke untuk menambahkan item ke lbConnections
                        this.Invoke((MethodInvoker)delegate
                        {
                            lbConnections.Items.Add(handlerSocket.RemoteEndPoint.ToString() + " connected.");
                        });

                        lock (this)
                        {
                            alSockets.Add(handlerSocket);
                        }
                        ThreadStart thdstHandler = new ThreadStart(handlerThread);
                        Thread thdHandler = new Thread(thdstHandler);
                        thdHandler.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan di sini
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                tcpListener.Stop(); // Pastikan listener dihentikan jika terjadi kesalahan
            }
        }


        // Thread untuk menangani data yang dikirim oleh klien
        public void handlerThread()
        {
            Socket handlerSocket = (Socket)alSockets[alSockets.Count - 1];
            NetworkStream networkStream = new NetworkStream(handlerSocket);
            int thisRead = 0;
            int blockSize = 1024;
            Byte[] dataByte = new Byte[blockSize];
            lock (this)
            {
                // Only one process can access
                // the same file at any given time
                Stream fileStream = System.IO.File.OpenWrite("E:\\icha\\Kuliah\\Semester 5\\Pemrograman Jaringan Komputer\\test.txt");
                while (true)
                {
                    thisRead = networkStream.Read(dataByte, 0, blockSize);
                    fileStream.Write(dataByte, 0, thisRead);
                    if (thisRead == 0) break;
                }
                fileStream.Close();
            }

            // Menggunakan Invoke untuk menambahkan "File Written" ke lbConnections
            this.Invoke((MethodInvoker)delegate
            {
                lbConnections.Items.Add("File Written");
            });

            handlerSocket = null;
        }


    }
}