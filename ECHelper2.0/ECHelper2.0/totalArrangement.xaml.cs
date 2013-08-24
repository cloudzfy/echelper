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
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;

namespace ECHelper2._0
{
    public partial class totalArrangement : PhoneApplicationPage
    {
        public String name, time;
        List<MailDataContract> checkedmaillist = new List<MailDataContract>();

        Random value = new Random();

          //=========================== 推送通知相关变量
         private HttpNotificationChannel httpChannel;
        private const string channelName = "Channel";
        public string channeluri;

        XDocument URI_Info = new XDocument();
        XDocument Online_Info = new XDocument();

        GeoCoordinateWatcher watcher;
        //============================= 推送通知相关变量

        public totalArrangement()
        {
            InitializeComponent();

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No network connection available!");
                return;
            }

            showCurrentlist();
            sendPosition();
           

            //新添加的接收 推送信息的 

            httpChannel = HttpNotificationChannel.Find(channelName);
            if (httpChannel != null)
            {
                httpChannel.Close();
                httpChannel.Dispose();
            }

            httpChannel = new HttpNotificationChannel(channelName, "NotificationServer");

            //注册URI
            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);

            //发生错误的事件
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ErrorOccurred);

            //toast 推送通知服务事件
            httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);

            //打开连接
            httpChannel.Open();
            //绑定toast 推送服务
            httpChannel.BindToShellToast();


        }

        private void showCurrentlist()
        {
           

            var http = new Http();

            long A = System.DateTime.Now.Ticks;


            string uri = "http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/outpatient/maillist?" + A;
          
            http.StartRequest(@uri,
                result =>
                {
                    //    A = result;
                    //    x = 1;

                    //    getfinished = false;
                    Dispatcher.BeginInvoke(() => Update(result));
                });



                




              
        }


        private void Update(string result)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfMailDataContract));
           
            result = "<root>" + result + "</root>";
            XDocument document = XDocument.Parse(result);
            
       
            ArrayOfMailDataContract mails = (ArrayOfMailDataContract) serializer.Deserialize(document.CreateReader());
            
           //this.listBox_Patient_to_Diag.ItemsSource = mails.Collection;

         //   MailDataContract mail;
           foreach(MailDataContract mail in mails.Collection)
           {
               if (mail.FromOrTo == "0"&&mail.IsRead=="0")
               { this.listBox_Patient_to_Diag.Items.Add(mail); }
           }

        }



        //==================================以下为推送通知服务相关代码

        //下面是推送 上线信息
        // ===========================这个是推送发的信息
        private void sendPosition()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 1;
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();
            }

        }


        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Location location = new Location();

            location.Latitude = e.Position.Location.Latitude;
            location.Longitude = e.Position.Location.Longitude;

            string strLatitude = location.Latitude.ToString();
            string strLongitude = location.Longitude.ToString();

            XElement Latitude = new XElement("Latitude", strLatitude);

            XElement Longtitude = new XElement("Longtitude", strLongitude);
            XElement UserName = new XElement("UserName", "xiaomming");

            Online_Info = new XDocument(new XElement("OnlineStatusDataContract", Latitude, Longtitude, UserName));

            string uristr = "http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/online";
            callREST_Online(uristr);

        }
        public void callREST_Online(string uristr)
        {
            Uri uri = new Uri(uristr);
            //    Uri uri = new Uri("http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/noticeuri");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";

            request.BeginGetRequestStream(sendXML_RequestCallback_Online, request);

        }

        private void sendXML_RequestCallback_Online(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;

            //  byte[] toSign = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("<xml></xml>");
            string a = Online_Info.ToString();
            byte[] toSign = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(Online_Info.ToString());//==========这行是自己写的

            using (var strm = req.EndGetRequestStream(result))
            {
                strm.Write(toSign, 0, toSign.Length);
                strm.Flush();
            }
            req.BeginGetResponse(this.fCallback_Online, req);
        }

        private void fCallback_Online(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;
            var resp = req.EndGetResponse(result);
            var strm = resp.GetResponseStream();
            //    Do something
            //  Save_Status.Text = "Save successfully";
        }


        // ===========================这个是推送发的信息




        //====================下面是推送 Uri
        private void getUri()
        {
            httpChannel = HttpNotificationChannel.Find(channelName);
             if (httpChannel != null)
            {
                httpChannel.Close();
                httpChannel.Dispose();
            }

            httpChannel = new HttpNotificationChannel(channelName, "NotificationServer");

            //注册URI
            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);

            //发生错误的事件
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ErrorOccurred);

            //toast 推送通知服务事件
            httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);

            //打开连接
            httpChannel.Open();
            //绑定toast 推送服务
            httpChannel.BindToShellToast();
        }


        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            string msg = string.Empty;
            EmergencyMesg Info = new EmergencyMesg();

            // 在此处要添加 将类赋值为全局变量, 然后在导航到emergency 界面

            foreach (var key in e.Collection.Keys)
            {
                if (key == "wp:Text1")
                    Info.patientName = e.Collection[key];
                else if (key == "wp:Text2")
                    Info.latitude = e.Collection[key];
                else if (key =="wp:Text3")
                    Info.longitude = e.Collection[key];
               // msg += key + " : " + e.Collection[key] + Environment.NewLine;
            }

            var app = App.Current as App;
            app.patientlocation = Info;

            Dispatcher.BeginInvoke(() =>
            {
                //msgTextBlock.Text = msg;
                this.NavigationService.Navigate(new Uri("/Emergency.xaml", UriKind.Relative));
            });
        }


        void httpChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            //子线程中更新UI
            Dispatcher.BeginInvoke(() =>
            {
              //  msgTextBlock.Text = e.Message;
            });
        }


        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Debug.WriteLine("CahnnelUri:{0}", e.ChannelUri);
            channeluri = e.ChannelUri.ToString();

            XElement uri = new XElement("uri", channeluri);

            URI_Info = new XDocument(new XElement("NoticeUri", uri));
            string uristr = " http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/noticeuri";
              callREST(uristr);
            //Dispatcher.BeginInvoke(() =>
            //{
            //    linkButton.IsEnabled = false;
            //    // ChannelUri=
            //});

        }




        public void callREST(string uristr)
        {
            Uri uri = new Uri(uristr);
        //    Uri uri = new Uri("http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/noticeuri");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";

            request.BeginGetRequestStream(sendXML_RequestCallback, request);

        }

        private void sendXML_RequestCallback(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;

            //  byte[] toSign = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("<xml></xml>");
            string a = URI_Info.ToString();
            byte[] toSign = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(URI_Info.ToString());//==========这行是自己写的

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
        }





        //==================================以上为推送通知代码





		//======================================
        //public List<Patient> totalpatientlist { get; set; }

        //protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    if (totalpatientlist != null)
        //    {
        //        this.listBox_Patient_to_Diag.ItemsSource = totalpatientlist;
        //    }
        //}
		//=====================================
		

        //private void GestureListenerTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        //{//在此处传递的参数应当包含病人的ID,等到整合时要填上
        //    //listBox_Patient_to_Diag.SelectedItem.
        //    this.NavigationService.Navigate(new Uri("/diagnosisNagivation.xaml", UriKind.Relative));

        //}

        private void patientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var app = App.Current as App;
            app.selectedPatient = (MailDataContract) listBox_Patient_to_Diag.SelectedItem;

         //   app.list.Add(app.selectedPatient);
            checkedmaillist.Add((MailDataContract)listBox_Patient_to_Diag.SelectedItem);
            //app.checkedmail.Add(app.selectedPatient);
            app.checkedmail = checkedmaillist;

            MailDataContract just = new MailDataContract();
            just = (MailDataContract)listBox_Patient_to_Diag.SelectedItem;


            string MAILID = just.MailId;


            long A = System.DateTime.Today.Ticks;

            string uri = "http://echelper.cloudapp.net/Service.svc/docotr/xiaoming/outpatient/" + MAILID + "/read?" + A;

            var http = new Http();

            http.StartRequest(@uri,
                result =>
                {
                    //    A = result;
                    //    x = 1;

                    //    getfinished = false;
                    //  Dispatcher.BeginInvoke(() => Update(result));
                });



         //   listBox_Patient_to_Diag.Items.Remove(listBox_Patient_to_Diag.SelectedItem);
            this.NavigationService.Navigate(new Uri("/diagnosisNagivation.xaml", UriKind.Relative));

        }


        private void setget()
        {

        }




        private void Recent_Patient_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           
            this.NavigationService.Navigate(new Uri("/PatientList.xaml", UriKind.Relative));



        }

        private void Setting_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DoctorInformation.xaml", UriKind.Relative));
        }

        private void IM_TAP_RP(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PatientList.xaml", UriKind.Relative));
        }

        private void IM_TAP_MS(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DoctorInformation.xaml", UriKind.Relative));
        }

       

    }

    public class Patient
    {
        public string Name
        {
            get;
            set;
        }

        public string Time
        {
            get;
            set;
        }


        



    }



    //================================以下为  http get 请求类  ================================================

    public class Http
    {
        public delegate void HandleResult(string result);
        private HandleResult handle;

        public void StartRequest(string Url, HandleResult handle)
        {
            this.handle = handle;
            var webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Method = "GET";
            try
            {
                webRequest.BeginGetResponse(new AsyncCallback(HandleResponse), webRequest);
            }
            catch
            {
            }
        }

        public void HandleResponse(IAsyncResult asyncResult)
        {
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            string result = string.Empty;
            try
            {
                httpRequest = (HttpWebRequest)asyncResult.AsyncState;
                httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(asyncResult);

                using (var reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch
            {

            }
            finally
            {
                if (httpRequest != null) httpRequest.Abort();
                if (httpResponse != null) httpResponse.Close();

            }
            handle(result);
        }
    }

    //================================以上为  http get 请求类  ================================================




}