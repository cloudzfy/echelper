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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace ECHelper2._0
{
    public partial class DoctorInformation : PhoneApplicationPage
    {
        //static readonly string[] BirthYears = { "1952", "1953", "1954", "1955", "1956", "1958", "1958", "1959", "1960" ,"1961","1962","1963",
        //                                      "1964","1965","1966","1968","1968","1969","1980","1981","1982","1983","1984","1985","1986","1988","1988","1989",
        //                                      "1980","1981","1982","1983","1984","1985","1986","1987","1988","1989","1990","1991","1992","1993","1994"};
      //  static readonly string[] Grade = { "Private", "Senoir", "Professional" };
        public Boolean Grade_Choosen ;
        public String DoctorName, DoctorBirthYear, DoctorGrade, DoctorSpeciality, DoctorSchool, DoctorEmail, DoctorPhone, shortDescription;
        //服务器地址
     //   public String Uri = "patient/adfadfasdf ";

         XDocument doctor_Info = new XDocument();

         public   Random value = new Random();

        public DoctorInformation()
        {
            InitializeComponent();
            //DataContext = BirthYears;

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No network connection available!");
                return;
            }


            this.Grade.Items.Add("Chief Physician");
            this.Grade.Items.Add("Deputy Chief Physician");
            this.Grade.Items.Add("Residency");
         //   Grade_Choosen = false;
            //DataContext = Grade;

            //var http = new Http();


            //http.StartRequest(@"http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/select",
            //    result =>
            //    {
            //        //    A = result;
            //        //    x = 1;

            //        //    getfinished = false;
            //        Dispatcher.BeginInvoke(() => Show(result));
            //    });

            getDoctor_info();

        }


        private void getDoctor_info()
        {
            var http = new Http();

           long A= System.DateTime.Now.Ticks;


            string uri = "http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/select?" + A;
            http.StartRequest(@uri,
                result =>
                {
                    //    A = result;
                    //    x = 1;

                    //    getfinished = false;
                    Dispatcher.BeginInvoke(() => Show(result));
                });
        }


        private void Show(string result)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DoctorUserDataContract));

            //   result =   result ;
            XDocument document = XDocument.Parse(result);

            DoctorUserDataContract doctor_info = serializer.Deserialize(document.CreateReader()) as DoctorUserDataContract;
            // ArrayOfMailDataContract mails = (ArrayOfMailDataContract) serializer.Deserialize(document.CreateReader());

            textBox_Name.Text = doctor_info.UserName;
            //textBox_Grade.Text = doctor_info.Grade;
            textBox_Phone.Text = doctor_info.phone;
            textBox_Email.Text = doctor_info.email;
            
            textBlock_DetailShortDescription.Text = doctor_info.Description;

        }




        private void Grade_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Grade_Choosen == false)
            {
                this.Grade.Height = 198;
                Grade_Choosen = true;
            }
            else
            {
                this.Grade.Height = 72;
                Grade_Choosen = false;
            }


        }

        //=================从DoctorShortDescription 读取数据
        public String Doctor_SD { get; set; }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Doctor_SD != null)
            {
                textBlock_DetailShortDescription.Text = Doctor_SD;
            }
        }

        //=====================================从DoctorShortDescription 读取数据
              

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if ((textBox_Name.Text == " Your Name") || (textBox_Phone.Text == " Enter Phone Number") || (textBox_Email.Text == " Your Email Address"))
            {
                Save_Status.Text="Incomplete Information";
            }
            else
            {
                



                DoctorName = textBox_Name.Text;
                //   DoctorBirthYear = textBlock_Birth.Text;
                DoctorGrade = textBlock_Grade.Text;
                //DoctorSpeciality = textBlock_Specialty.Text;
            //    DoctorSchool = textBox_Phone.Text;
                DoctorPhone = textBox_Phone.Text;
                DoctorEmail = textBox_Email.Text;
                // YI xml wen jian xing shi cun chu zhe ge wen jian ran hou fa dao Azure shang!
             //   XElement doctor = new XElement("DoctorUserDataContract");//创建一个XML元素
                string shortDescription = "Graduate at harward university";
                XElement UserName = new XElement("UserName", DoctorName);
                XElement NickName = new XElement("NickName", DoctorName);
                XElement Grade = new XElement("Grade", DoctorGrade);//创建一个XML属性
                XElement Description = new XElement("Description", shortDescription);
                XElement phone = new XElement("phone", DoctorPhone);
                XElement email = new XElement("email", DoctorEmail);
                XElement image = new XElement("image", "title_bg.png");

                //XNode doctor = new XNode



              //  doctor.Add(UserName,NickName,Grade,shortDescription,phone,email,image);//将这两个属性添加到 XML元素上
                //用_item 新建一个XML的Linq文档 
                doctor_Info = new XDocument(new XElement("DoctorUserDataContract",Description,Grade,NickName,UserName,email,image,phone));



                callREST();
              //  Save_Status.Text = "Save Successfully !";
           //     this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
            }

        }

        //protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    var targetPage = e.Content as DoctorShortDescription;
        //    if (targetPage != null)
        //    {
        //        DoctorName = textBox_Name.Text;
        //        targetPage.Doctor_Name = DoctorName;
        //    }
        //}

      

        private void GF_Email(object sender, RoutedEventArgs e)
        {
            //if (textBox_Email.Text == " Your Email Address")
            //{
                textBox_Email.Text = " ";
            //}
          
        }

        private void GF_Phone(object sender, RoutedEventArgs e)
        {
            //if (textBox_Phone.Text == " Enter Phone Number")
            //{
                textBox_Phone.Text = " ";
            //}
        }

        private void GF_Name(object sender, RoutedEventArgs e)
        {
            //if (textBox_Name.Text == " Your Name")
            //{ 
                textBox_Name.Text = " "; 
            //}
        }

        private void Edit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DoctorShortDescription.xaml", UriKind.Relative));
        }

        private void LF_Name(object sender, RoutedEventArgs e)
        {
            if (textBox_Name.Text == " ")
            {
                textBox_Name.Text = " Your Name";
            }
        }

        private void LF_Phone(object sender, RoutedEventArgs e)
        {
            if (textBox_Phone.Text == " ")
            {
                textBox_Phone.Text = " Enter Phone Number";
            }
        }

        private void LF_Email(object sender, RoutedEventArgs e)
        {
            if (textBox_Email.Text == " ")
            {
                textBox_Email.Text = " Your Email Address";
            }
        }

        //========================================以下为post方法将数据传送到服务器端
        public void callREST()
        {

            Uri uri = new Uri("http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/update");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";

            request.BeginGetRequestStream(sendXML_RequestCallback, request);

        }

        private void sendXML_RequestCallback(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;

            //  byte[] toSign = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("<xml></xml>");
            string a = doctor_Info.ToString();
            byte[] toSign = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(doctor_Info.ToString());//==========这行是自己写的

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
            Dispatcher.BeginInvoke(() => changePage());
        }

        private void changePage()
        {
            Save_Status.Text = "Save Successfully";
            this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
        }






        //========================================以上为post方法将数据传送到服务器端






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
}