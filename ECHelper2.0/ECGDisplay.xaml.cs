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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Windows.Resources;
using System.IO.IsolatedStorage;

namespace ECHelper2._0
{
    public partial class ECGDisplay : UserControl
    {
        int i;
        public byte[] data1;
        public int totallength;
        List<int> list = new List<int>();
       
        IsolatedStorageFileStream stream;
        IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
		
		DispatcherTimer timer = new DispatcherTimer();
        int count;
       public  int judge;
        //将stream设为全局变量看看能不能实现读取的功能

        String ECGFileName;

        byte[] buffer;




        public ECGDisplay()
        {
            InitializeComponent();

            var app= App.Current as App;
            ECGFileName=app.selectedPatient.ECG;





            updateMyScreen();
        }

        void updateMyScreen()
        {

            //Polyline ChartLine = new Polyline();
            //ChartLine.Stroke = new SolidColorBrush(Colors.Red);
       



            for (i = 0; i <= canvas1.Height; i += 10)
            {
                Line BaseLine = new Line();
                BaseLine.Stroke = new SolidColorBrush(Colors.Orange);

                BaseLine.X1 = 0;
                BaseLine.Y1 = i;
                BaseLine.X2 = canvas1.Width;
                BaseLine.Y2 = i;
                canvas1.Children.Add(BaseLine);

            }

            for (i = 0; i <= canvas1.Width; i += 10)
            {
                Line VerLine = new Line();
                VerLine.Stroke = new SolidColorBrush(Colors.Orange);

                VerLine.X1 = i;
                VerLine.Y1 = 0;
                VerLine.X2 = i;
                VerLine.Y2 = canvas1.Height;
                canvas1.Children.Add(VerLine);

            }

          //  Stream stream1 = App.GetResourceStream(new Uri("shared/transfers/634708322772702206", UriKind.Relative)).Stream;
       //     byte[] data1;
           // data1=ReadFully(stream1);
            string filelocation = "shared/transfers/" + ECGFileName;
            stream = new IsolatedStorageFileStream(filelocation, FileMode.Open, fileStorage);
           // long longth = stream.Length;



            BinaryReader reader = new BinaryReader(stream);
            buffer = new byte[stream.Length];
            buffer=reader.ReadBytes((int)stream.Length);

            count = 0;
            int[] a = new int[768];
            Polyline Chatline = new Polyline();
            Chatline.Stroke = new SolidColorBrush(Colors.Red);
            Chatline.StrokeThickness = 4;
            //for (i = 0; i < 768; i++)
            //{
            //    a[i] = reader.ReadByte();
            //    if (a[i] > -1 && i % 2 == 1)
            //    {
                   
            //        int j = (a[i - 1] * 256 + a[i])/2;
            //        list.Add(j);
            //        Chatline.Points.Add(new Point((i - 1) * 0.625, canvas1.Height - j));
            //    }
            //}
            //canvas1.Children.Add(Chatline);



            for (i = 0; i < 768; i++)
            {
                count++;
                a[i] = buffer[i];
                if (a[i] > -1 && i % 2 == 1)
                {

                    int j = (a[i - 1] * 256 + a[i]) / 2;
                    list.Add(j);
                    Chatline.Points.Add(new Point((i - 1) * 0.625, canvas1.Height - j));
                }
            }
            canvas1.Children.Add(Chatline);



           // judge=1;

         //   DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);

            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

        //    timer.Stop();
        //    timer.Stop();
       //     if()
        
        }


        void timer_Tick(object sender, EventArgs e)
        {

       //     stream = new IsolatedStorageFileStream("shared/transfers/634708322772702206", FileMode.Open, fileStorage);
            //BinaryReader reader = new BinaryReader(stream1);
            //long length = stream1.Length;
            
     //       BinaryReader reader = new BinaryReader(stream);
          //  long length = stream1.Length;
            int[] a = new int[96];

            //清空原有的图像，覆盖上新的ECG===================================
            canvas1.Children.Clear();
            for (i = 0; i <= canvas1.Height; i += 10)
            {
                Line BaseLine = new Line();
                BaseLine.Stroke = new SolidColorBrush(Colors.Orange);

                BaseLine.X1 = 0;
                BaseLine.Y1 = i;
                BaseLine.X2 = canvas1.Width;
                BaseLine.Y2 = i;
                canvas1.Children.Add(BaseLine);

            }

            for (i = 0; i <= canvas1.Width; i += 10)
            {
                Line VerLine = new Line();
                VerLine.Stroke = new SolidColorBrush(Colors.Orange);

                VerLine.X1 = i;
                VerLine.Y1 = 0;
                VerLine.X2 = i;
                VerLine.Y2 = canvas1.Height;
                canvas1.Children.Add(VerLine);

            }
            //========================================================

            Polyline Chatline = new Polyline();
            Chatline.Stroke = new SolidColorBrush(Colors.Red);
            Chatline.StrokeThickness = 4;
            //for (i = list.Count - 1; i >= list.Count-44; i--)
            //{
            //    Chatline.Points.Add(new Point((i - 1) * 5, canvas1.Height - list[i]));

            //}
                //int i = (int)reader.ReadByte();
                //Chatline.Points.Add(new Point(10,i));
                //int j =  (int)reader.ReadByte();
                //Chatline.Points.Add(new Point(20, j));
                //   canvas1.Children.Remove(Chatline);
            try
            {
                count =count+ 80;
                for (i = 0; i < 80; i++)
                {
                    a[i] = buffer[count + i];
                    if (a[i] > -1 && i % 2 == 1)
                    {
                        int j = (a[i - 1] * 256 + a[i]) / 2;

                        list.Add(j);
                        // Chatline.Points.Add(new Point((i - 1) * 5, canvas1.Height - j));
                    }
                    else if (a[i] == -1)
                    {

                        timer.Stop();
                        return;
                    }
                }

                for (i = 0; i < 384; i++)
                {
                    Chatline.Points.Add(new Point(canvas1.Width - (i) * 1.25, canvas1.Height - list[list.Count - 1 - i]));
                    
                }
            }
            catch (Exception) {
                stream.Close();
            }


            //Polyline Chatline = new Polyline();
            //int[] a = new int[48];
            //Chatline.Stroke = new SolidColorBrush(Colors.Red);
            //canvas1.Children.Remove(Chatline);
            //if (count < totallength-48)
            //{
            //    for (int i = 0; i < 48; i++)
            //    {
            //        a[i] = (int)canvas1.Height-((int)data1[i]);
            //        Chatline.Points.Add(new Point(i * 10, a[i]));
            //        // textBlock1.Text += (int)data1[i];
            //        //if (i % 5 == 0)
            //        //{   
            //        //    Chatline.Points.Add(new Point(i % 400, (int)data1[i]));

            //        //}
            //        count += 48;

            //    }
                
            //}

           canvas1.Children.Add(Chatline);

        }


    }
}
