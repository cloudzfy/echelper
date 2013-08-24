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
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace ConnectionLibrary
{
    public sealed class ECGReader
    {
        private StorageFile myFile = null;
        private IRandomAccessStream readStream = null;
        private DataReader fileReader = null;
        private byte[] data;
        private static uint count = 0;

        public IAsyncOperation<uint> open(string url)
        {
            // this is Key.  It converts an async task into IAsyncOperation: 
            return (IAsyncOperation<uint>)AsyncInfo.Run((System.Threading.CancellationToken ct) => OpenFile(url));
        }


        private async Task<uint> OpenFile(string fileName)
        {
            StorageFolder myStorageFolder = KnownFolders.DocumentsLibrary;
            myFile = await myStorageFolder.GetFileAsync("ECHelper Files\\" + fileName+".ech");
            readStream = await myFile.OpenAsync(FileAccessMode.Read);
            fileReader = new DataReader(readStream);
            count = await fileReader.LoadAsync((uint)readStream.Size);
            data = new byte[count];
            fileReader.ReadBytes(data);
            return count;
        }

        public byte[] getData(int offset, int length)
        {
            if (offset < 0) offset = 0;
            if (offset % 2 == 1) offset = offset + 1;
            byte[] tmp = new byte[length];
            if (count - offset < length)
            {
                for (int i = (int)count - offset; i < length; i++)
                {
                    tmp[i] = 0;
                }
                length = (int)count - offset;
            }
            for (int i = 0; i < length; i++)
            {
                tmp[i] = data[offset + i];
            }
            return tmp;
        }
    }
}
