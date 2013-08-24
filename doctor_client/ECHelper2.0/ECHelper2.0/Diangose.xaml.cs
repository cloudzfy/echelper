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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ECHelper2._0
{
    public partial class Diangose : PhoneApplicationPage
    {
        string Patientid;
        string oldTitle;
        XDocument diagnose_Info = new XDocument();

        string ECGName;

        public Diangose()
        {
            InitializeComponent();
            var app = App.Current as App;
            Patientid = app.selectedPatient.PatientId;
            oldTitle = app.selectedPatient.Title;

            ECGName = app.selectedPatient.ECG;

            textBlock_PatientDescription.Text = "Description : \n" + app.PatientDescription.Description;
            textBlock_AllergyDrugs.Text = "Allergy Drugs : \n" + app.PatientDescription.Allery;
            
        }

        private void GF_ZZ(object sender, RoutedEventArgs e)
        {
            if (textBox_Zhengzhuang.Text == "  Patient's Symptom")
            {
                textBox_Zhengzhuang.FontSize = 25;
                textBox_Zhengzhuang.Text = "";
            }
        }

        private void LF_ZZ(object sender, RoutedEventArgs e)
        {
            if (textBox_Zhengzhuang.Text == "")
            {
                textBox_Zhengzhuang.FontSize = 40;
                textBox_Zhengzhuang.Text = "  Patient's Symptom";
            }
        }


        private void GF_ZD(object sender, RoutedEventArgs e)
        {
            if (textBox_Zhenduan.Text == "  Your Diagnosis")
            {
                textBox_Zhenduan.FontSize = 25;
                textBox_Zhenduan.Text = "";
            }
        }

        private void LF_ZD(object sender, RoutedEventArgs e)
        {
            if (textBox_Zhengzhuang.Text == "")
            {
                textBox_Zhenduan.FontSize = 40;
                textBox_Zhenduan.Text = "  Your Diagnosis";
            }
        }

        private void GF_ZL(object sender, RoutedEventArgs e)
        {
            if (textBox_Zhiliao.Text == " Your Treatment")
            {
                textBox_Zhiliao.FontSize = 25;
                textBox_Zhiliao.Text = "";
            }
        }

        private void LF_ZL(object sender, RoutedEventArgs e)
        {
            if (textBox_Zhiliao.Text == "")
            {
                textBox_Zhiliao.FontSize = 40;
                textBox_Zhiliao.Text = " Your Treatment";
            }
        }

        private void btn_Send(object sender, RoutedEventArgs e)
        {
            if ((textBox_Zhengzhuang.Text == "  Your Diagnosis") || (textBox_Zhiliao.Text == " Your Treatment") || (textBox_Zhenduan.Text == "  Your Diagnosis"))
            {
                textBlock_Diagnosis_Status.Text = "Incomplete Diagnosis";
            }
            else
            {
                string Content = textBox_Zhengzhuang.Text +"\n"+ textBox_Zhenduan.Text +"\n" + textBox_Zhiliao.Text;

                XElement DoctorId = new XElement("DoctorId", "xiaoming");
                XElement ECG = new XElement("ECG", ECGName);
                XElement FromOrTo = new XElement("FromOrTo", 1);//创建一个XML属性
                XElement PatientId = new XElement("PatientId", Patientid);
                XElement TextContent = new XElement("TextContent", Content);
                XElement Time = new XElement("Time", "1999-05-31T11:20:00");
                string newTitle = "Re: " + oldTitle;
                XElement Title = new XElement("Title", newTitle);

                //XNode doctor = new XNode



                //  doctor.Add(UserName,NickName,Grade,shortDescription,phone,email,image);//将这两个属性添加到 XML元素上
                //用_item 新建一个XML的Linq文档 
                diagnose_Info = new XDocument(new XElement("NewMailDataContract", DoctorId, ECG, FromOrTo, PatientId, TextContent, Time, Title));

                callREST();
            }
        }



        //========================================以下为post方法将数据传送到服务器端
        public void callREST()
        {

            Uri uri = new Uri("http://echelper.cloudapp.net/Service.svc/doctor/xiaoming/outpatient/diagnosis");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";

            request.BeginGetRequestStream(sendXML_RequestCallback, request);

        }

        private void sendXML_RequestCallback(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;

            //  byte[] toSign = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("<xml></xml>");
            string a = diagnose_Info.ToString();
            byte[] toSign = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(diagnose_Info.ToString());//==========这行是自己写的

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
            Dispatcher.BeginInvoke(() => showResult());

        }

        private void showResult()
        {
            textBlock_Diagnosis_Status.Text = "Submit successfully";
           
            this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
        }






        //========================================以上为post方法将数据传送到服务器端



    }
}