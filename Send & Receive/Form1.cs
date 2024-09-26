using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Send___Receive
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            tbFilename.Text = openFileDialog.FileName;
        }

        private void btnSend_Click(object sender, System.EventArgs e)
        {
            try
            {
                using (Stream fileStream = File.OpenRead(tbFilename.Text))
                {
                    // Allocate memory space for the file
                    byte[] fileBuffer = new byte[fileStream.Length];
                    fileStream.Read(fileBuffer, 0, (int)fileStream.Length);

                    // Open a TCP/IP Connection and send the data
                    using (TcpClient clientSocket = new TcpClient(tbServer.Text, 8080))
                    {
                        using (NetworkStream networkStream = clientSocket.GetStream())
                        {
                            networkStream.Write(fileBuffer, 0, fileBuffer.Length);
                        }
                    }
                }

                MessageBox.Show("File sent successfully!");
            }
            catch (Exception ex)
            {
                // Tangani kesalahan di sini
                MessageBox.Show("Error: " + ex.Message);
            }
        }



    }
}