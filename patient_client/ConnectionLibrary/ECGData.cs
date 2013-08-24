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
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Storage;
using System.IO;
using System.IO.Compression;

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;


namespace ConnectionLibrary
{
    public sealed class ECGData
    {
        private const int MAX_LENGTH = 500000;
        private static string port = "8798";
        private static HostName address = new HostName("127.0.0.1");
        private static StreamSocket clientSocket = new StreamSocket();
        private static StorageFile myFile = null;
        private static int count = 0;
        private static byte[] data = new byte[MAX_LENGTH];
        private static String fileName = null;
        private static int data_l = 0;
        private static int draw_l = 0;
        private static int flag = 0;

        public async void BeginConnect()
        {
            await clientSocket.ConnectAsync(address, port, SocketProtectionLevel.PlainSocket);
            Task task = new Task(RequestData);
            task.Start();
        }

        public byte[] getECGData()
        {
            if (flag == 0)
            {
                byte[] send = { 255 };
                return send;
            }
            int draw_r = count;
            if (draw_l == draw_r)
            {
                byte[] send = { 0 };
                return send;
            }
            if (draw_r % 2 == 0)
            {
                draw_r = (draw_r - 1 + MAX_LENGTH) % MAX_LENGTH;
            }
            if (draw_r < draw_l)
            {
                draw_r += MAX_LENGTH;
            }
            if (draw_r - draw_l > 250)
            {
                byte[] send = new byte[251];
                for (int i = draw_r, j = 250; j > 0; i--, j--)
                {
                    send[j] = data[i % MAX_LENGTH];
                }
                send[0] = 250;
                draw_l = (draw_r + 1) % MAX_LENGTH;
                return send;
            }
            else
            {
                byte[] send = new byte[draw_r - draw_l + 2];
                for (int i = draw_l, j = 1; i <= draw_r; i++, j++)
                {
                    send[j] = data[i % MAX_LENGTH];
                }
                send[0] = (byte)(draw_r - draw_l + 1);
                draw_l = (draw_r + 1) % MAX_LENGTH;
                return send;
            }
        }

        private async void RequestData()
        {
            byte[] requestData = { 0xF0, 0xC0, 0xB0 };
            byte[] receiver = new byte[5];
            DataReader dataReader = new DataReader(clientSocket.InputStream);
            DataWriter dataWriter = new DataWriter(clientSocket.OutputStream);
            dataWriter.WriteBytes(requestData);
            await dataWriter.StoreAsync();
            await dataWriter.FlushAsync();
            while (true)
            {
                await dataReader.LoadAsync(5);
                dataReader.ReadBytes(receiver);
                byte[] isNull = { 0x02, 0x00, 0x01, 0x02, 0x07 };
                if (receiver[0] == 2 && receiver[1] == 0 && receiver[2] == 1 && receiver[3] == 2 && receiver[4] == 7)
                {
                    flag = 0;
                    dataWriter.WriteBytes(requestData);
                    await dataWriter.StoreAsync();
                    await dataWriter.FlushAsync();
                }
                else
                {
                    flag = 1;
                    if (receiver[4] != (receiver[0] + receiver[1] + receiver[2] + receiver[3]) % 256)
                    {
                        continue;
                    }
                    if (count % 2 == 1)
                    {
                        count = (count - 1 + MAX_LENGTH) % MAX_LENGTH;
                    }
                    data[count] = receiver[2];
                    count = (count + 1) % MAX_LENGTH;
                    data[count] = receiver[3];
                    count = (count + 1) % MAX_LENGTH;
                }
            }
        }

        public string BeginStore()
        {
            string blobName = System.DateTime.Now.Ticks.ToString();
            fileName = "ECHelper Files\\" + blobName + ".ech";
            data_l = count;
            if (data_l % 2 != 0)
            {
                data_l = (data_l + 1) % MAX_LENGTH;
            }
            return blobName;
        }

        //public void EndStore()
        //{
        //    Task task = new Task(SaveStore);
        //    task.Start();
        //}

        public IAsyncOperation<StorageFile> EndStore()
        {
            // this is Key.  It converts an async task into IAsyncOperation: 
            return (IAsyncOperation<StorageFile>)AsyncInfo.Run((System.Threading.CancellationToken ct) => SaveStore());
        }


        private async Task<StorageFile> SaveStore()
        {
            int data_r = count;
            if (data_r % 2 == 0)
            {
                data_r = (data_r - 1 + MAX_LENGTH) % MAX_LENGTH;
            }
            StorageFolder myStorageFolder = KnownFolders.DocumentsLibrary;
            myFile = await myStorageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            using (IRandomAccessStream writeStream = await myFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter fileWriter = new DataWriter(writeStream))
                {
                    if (data_r < data_l)
                    {
                        data_r += MAX_LENGTH;
                    }
                    for (int i = data_l; i < data_r; i++)
                    {
                        fileWriter.WriteByte(data[i % MAX_LENGTH]);
                    }
                    await fileWriter.StoreAsync();
                    await fileWriter.FlushAsync();
                    await writeStream.FlushAsync();
                    fileWriter.Dispose();
                }
                writeStream.Dispose();
            }
            data_l = (data_r + 1) % MAX_LENGTH;
            return myFile;
        }

        //private async void SaveStore()
        //{
        //    int data_r = count;
        //    if (data_r % 2 == 0)
        //    {
        //        data_r = (data_r - 1 + MAX_LENGTH) % MAX_LENGTH;
        //    }
        //    StorageFolder myStorageFolder = KnownFolders.DocumentsLibrary;
        //    myFile = await myStorageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
        //    using (IRandomAccessStream writeStream = await myFile.OpenAsync(FileAccessMode.ReadWrite))
        //    {
        //        using (DataWriter fileWriter = new DataWriter(writeStream))
        //        {
        //            if (data_r < data_l)
        //            {
        //                data_r += MAX_LENGTH;
        //            }
        //            for (int i = data_l; i < data_r; i++)
        //            {
        //                fileWriter.WriteByte(data[i % MAX_LENGTH]);
        //            }
        //            await fileWriter.StoreAsync();
        //            await fileWriter.FlushAsync();
        //            await writeStream.FlushAsync();
        //            fileWriter.Dispose();
        //        }
        //        writeStream.Dispose();
        //    }
        //    data_l = (data_r + 1) % MAX_LENGTH;
        //}
    }
}