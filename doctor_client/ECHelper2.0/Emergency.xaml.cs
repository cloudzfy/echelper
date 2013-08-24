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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using System.IO;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace ECHelper2._0
{
    public partial class Emergency : PhoneApplicationPage
    {
        //===========推送相关的变量
        //private HttpNotificationChannel httpChannel;
        //private const string channelName = "Channel";
        //============推送相关的变量声明

        GeoCoordinateWatcher watcher;
        Location thislocation;

       BingMapsDirectionsTask bingMapsDirectionsTask = new BingMapsDirectionsTask();

       string Patientlatitude;
       string PatientLongitude;
       string UserName;

       XDocument accept_Info = new XDocument();

        public Emergency()
        {
            InitializeComponent();

            var app = App.Current as App;
            Patientlatitude = app.patientlocation.latitude;
            PatientLongitude = app.patientlocation.longitude;
            UserName = app.patientlocation.patientName;

            showinfor();



        }


        private void showinfor()
        {
            //Location patientlocation = new Location();
            //patientlocation.Latitude = 40.107;
            //patientlocation.Longitude = 116.3333;

            //Pushpin patientpushin = new Pushpin();
            //patientpushin.Content = "Patient Location";
            //patientpushin.Location = patientlocation;
            //this.map1.Children.Add(patientpushin);
       //    this.map1.SetView(patientlocation,12);

            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High); // 采用高精度
                watcher.MovementThreshold = 20; // PositionChanged事件之间传送的最小距离
                //绑定事件
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                //开始从位置服务获取数据
                watcher.Start();
            }
        }


        //检测到位置更改时
        //当定位服务已准备就绪并接收数据时，它将开始引发 PositionChanged 事件
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Location location = new Location();//实例化一个位置位置类的实例
            //将当前的经纬度值赋给位置对象
            location.Latitude = e.Position.Location.Latitude;
            location.Longitude = e.Position.Location.Longitude;

            thislocation = location;

            Pushpin pushpin = new Pushpin();//实例化一个图钉实例
            pushpin.Content = "You";
            pushpin.Location = location;

            Location patientlocation = new Location();
            //patientlocation.Latitude = 40.106;
            //patientlocation.Longitude = 116.3333;


            patientlocation.Latitude = System.Convert.ToDouble(Patientlatitude);
            patientlocation.Longitude = System.Convert.ToDouble(PatientLongitude);
            

            Pushpin patientpushin = new Pushpin();
            patientpushin.Content = "Patient";
            patientpushin.Location = patientlocation;
            this.map1.Children.Add(patientpushin);

            MapPolyline line = new MapPolyline();
            line.Locations = new LocationCollection { location, patientlocation};

            

            this.map1.Mode = map1.Mode = new RoadMode();
            this.map1.Children.Add(pushpin);//将图钉显示在地图上

            LocationRect rect = LocationRect.CreateLocationRect(line.Locations);
            map1.SetView(rect);


           this.map1.ZoomLevel = 12;
        //    this.map1.SetView(location, 12);//缩放界别为10(1-16),始终将当前位置位于地图中心
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {


            GeoCoordinate spaceNeedleLocation = new GeoCoordinate(thislocation.Latitude, thislocation.Longitude);


            XElement doctorID = new XElement("doctorID", "xiaoming");
            XElement latitude = new XElement("latitude", thislocation.Latitude);
            XElement longitude = new XElement("longitude", thislocation.Longitude);
            XElement confirm = new XElement("confirm", "true");

            accept_Info = new XDocument(new XElement("EmergencyConfirm", doctorID, latitude, longitude, confirm));


            callREST();

            try
            {

                LabeledMapLocation spaceNeedleLML = new LabeledMapLocation("Patient Location", spaceNeedleLocation);

                bingMapsDirectionsTask.End = spaceNeedleLML;

                bingMapsDirectionsTask.Show();
            }
            catch (WebException){
            }

            //this.NavigationService.Navigate(new Uri("/ShowRoute.xaml", UriKind.Relative));
        }


        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            GeoCoordinate spaceNeedleLocation = new GeoCoordinate(thislocation.Latitude, thislocation.Longitude);


            XElement doctorID = new XElement("doctorID", "xiaoming");
            XElement latitude = new XElement("latitude", thislocation.Latitude);
            XElement longitude = new XElement("longitude", thislocation.Longitude);
            XElement confirm = new XElement("confirm", "false");

            accept_Info = new XDocument(new XElement("EmergencyConfirm", doctorID, latitude, longitude, confirm));


            callREST();

        }

        //========================================以下为post方法将数据传送到服务器端
        public void callREST()
        {
           // string uristr = "http://echelper.cloudapp.net/Service.svc/doctor/" + Doctorid + "/emergency/" + patientUserName + "/confirm";
            Uri uri = new Uri("http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/emergency/liaomin/confirm");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";

            request.BeginGetRequestStream(sendXML_RequestCallback, request);

        }

        private void sendXML_RequestCallback(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;

            //  byte[] toSign = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("<xml></xml>");
            string a = accept_Info.ToString();
            byte[] toSign = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(accept_Info.ToString());//==========这行是自己写的

            using (var strm = req.EndGetRequestStream(result))
            {
                strm.Write(toSign, 0, toSign.Length);
                strm.Flush();
            }
            req.BeginGetResponse(this.fCallback, req);
        }

        private void fCallback(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;
            var resp = req.EndGetResponse(result);
            var strm = resp.GetResponseStream();
            //    Do something
            //  Save_Status.Text = "Save successfully";
      //      Dispatcher.BeginInvoke(() => showResult());

        }

        //====================================一下为 推送功能相关代码

        //private void linkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    httpChannel = HttpNotificationChannel.Find(channelName);
        //    //如果存在就删除
        //    if (httpChannel != null)
        //    {
        //        httpChannel.Close();
        //        httpChannel.Dispose();
        //    }

        //    httpChannel = new HttpNotificationChannel(channelName, "NotificationServer");

        //    //注册URI
        //    httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);

        //    //发生错误的事件
        //    httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ErrorOccurred);
        //    //Raw 推送通知服务事件
        ////    httpChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(httpChannel_HttpNotificationReceived);

        //    //toast 推送通知服务事件
        //    httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);

        //    //打开连接
        //    httpChannel.Open();
        //    //绑定toast 推送服务
        //    httpChannel.BindToShellToast();

        //    //绑定Tokens (tile) 推送服务 
        ////    httpChannel.BindToShellTile();
        //}


        //void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        //{
        //    string msg = string.Empty;
        //    foreach (var key in e.Collection.Keys)
        //    {
        //        msg += key + " : " + e.Collection[key] + Environment.NewLine;
        //    }
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        msgTextBlock.Text = msg;
        //    });
        //}


        //void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        //{
        //    //Raw 支持任意格式数据
        //    using (var reader = new StreamReader(e.Notification.Body))
        //    {
        //        string msg = reader.ReadToEnd();
        //        Dispatcher.BeginInvoke(() =>
        //        {
        //            msgTextBlock.Text = msg;
        //        });
        //    }
        //}

        //void httpChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        //{
        //    //子线程中更新UI
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        msgTextBlock.Text = e.Message;
        //    });
        //}

        //void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        //{
        //    Debug.WriteLine("CahnnelUri:{0}", e.ChannelUri);
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        linkButton.IsEnabled = false;
        //    });

        //}
        //=================================================以上为推送功能相关的文档
    }
}