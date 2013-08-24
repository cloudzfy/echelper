/*
 * echelper, An Open Source Mobile Medical System
 * Copyright (C) 2013, Nicefforts
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ECGNotify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyicon = null;
        private SerialPort sp;
        private Thread connectThread, serialportThread;
        private Boolean isOpen = false;
        private Socket serverSocket;
        public MainWindow()
        {
            InitializeComponent();

            this.Visibility = Visibility.Hidden;
            notifyicon = new NotifyIcon();
            notifyicon.BalloonTipTitle = "ECG Collector";
            notifyicon.BalloonTipText = "ECG Collector is now running ...";
            notifyicon.Icon = new System.Drawing.Icon("logo.ico");
            notifyicon.Visible = true;
            notifyicon.ShowBalloonTip(2000);

            MenuItem about = new MenuItem("About");
            about.Click += new EventHandler(About_OnClick);
            MenuItem exit = new MenuItem("Exit");
            exit.Click += new EventHandler(Exit_OnClick);
            MenuItem[] children = new MenuItem[] { about, exit };
            notifyicon.ContextMenu = new ContextMenu(children);

            serialportThread = new Thread(new ThreadStart(CheckSerialport));
            serialportThread.Start();

            connectThread = new Thread(new ThreadStart(BeginConnect));
            connectThread.Start();
        }

        private void About_OnClick(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Copyright Nicefforts");
        }

        private void Exit_OnClick(object sender, EventArgs e)
        {
            byte[] data = { 0xF0, 0xC1, 0xB1 };
            try
            {
                sp.Write(data, 0, 3);
                sp.Close();
            }
            catch (Exception)
            {
            }
            Environment.Exit(0);
        }

        private void CheckSerialport()
        {
            sp = new SerialPort();
            sp.PortName = "COM3";
            sp.BaudRate = 57600;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.Handshake = Handshake.XOnXOff;
            while (true)
            {
                try
                {
                    if (!sp.IsOpen)
                    {
                        sp.Open();
                    }
                    isOpen = sp.IsOpen;
                }
                catch (IOException)
                {
                    isOpen = false;
                }
                Thread.Sleep(1500);
            }
        }
        private void BeginSend(object clientSocket)
        {
            Socket socket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int data = sp.ReadByte();
                    if (data != 240)
                    {
                        continue;
                    }
                    byte[] msg = new byte[5];
                    msg[0] = System.BitConverter.GetBytes(data).ElementAt(0);
                    bool notSend = false;
                    for (int i = 1; i < 5; i++)
                    {
                        data = sp.ReadByte();
                        if (data == 193 && i == 1)
                        {
                            notSend = true;
                        }
                        msg[i] = System.BitConverter.GetBytes(data).ElementAt(0);
                    }
                    if (!notSend)
                    {
                        socket.Send(msg);
                    }
                }
                catch (TimeoutException)
                {
                }
                catch (SocketException)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch (Exception)
                    {
                    }
                    break;
                }
                catch (Exception)
                {
                }
            }

        }

        private void BeginReceive(object clientSocket)
        {
            Socket socket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    byte[] data = new byte[3];
                    socket.Receive(data);
                    if (!isOpen)
                    {
                        byte[] msg = { 0x02, 0x00, 0x01, 0x02, 0x07 };
                        socket.Send(msg);
                    }
                    else
                    {
                        sp.Write(data, 0, 3);
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch (Exception)
                    {
                    }
                    break;
                }
            }
        }

        private void BeginConnect()
        {
            byte[] data = { 0xF0, 0xC1, 0xB1 };
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Thread sendThread = null;
            Thread receiveThread = null;
            Socket lastSocket = null;
            int port = 8798;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, port));
            serverSocket.Listen(10);
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                try
                {
                    if (sendThread != null)
                    {
                        sendThread.Abort();
                    }
                }
                catch (Exception) { }
                try
                {
                    if (receiveThread != null)
                    {
                        receiveThread.Abort();
                    }
                }
                catch (Exception) { }
                try
                {
                    if (lastSocket != null)
                    {
                        lastSocket.Close();
                    }
                }
                catch (Exception) { }
                try
                {
                    sp.Write(data, 0, 3);
                    //sp.ReadLine();
                }
                catch (Exception) { }
                clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                sendThread = new Thread(BeginSend);
                sendThread.Start(clientSocket);
                receiveThread = new Thread(BeginReceive);
                receiveThread.Start(clientSocket);
                lastSocket = clientSocket;
            }
        }
    }
}