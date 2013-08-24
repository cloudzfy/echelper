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
using System.IO.IsolatedStorage;
using System.IO;


using Microsoft.Phone.BackgroundTransfer;


namespace ECHelper2._0
{
    public partial class diagnosisNagivation : PhoneApplicationPage
    {
        MailDataContract mail;
        string ECGName;
        String Patientid;

        PatientUserDataContract patientdesc;

        //================以下为 blob 相关的变量

        private String SAVE_LOCATION="shared/transfers/";
        private Uri fileDownloadUri;
        private Uri saveLocationUri;
        private BackgroundTransferRequest _currentRequest = null;

        string sas;

        //================以上为 blob 相关的变量





        public diagnosisNagivation()
        {
            InitializeComponent();





            var app = App.Current as App;
            mail = app.selectedPatient;
            ECGName = app.selectedPatient.ECG;
            Patientid = app.selectedPatient.PatientId;


          downloadECG(ECGName);
       
        }


       
        //=================================== blob 相关函数定义

        private void downloadECG(string filename)
        {
            if (_currentRequest != null)
            {
                BackgroundTransferService.Remove(_currentRequest);
            }

          
            //解析文件名，赋给SAVE_LOCATION
        //    filename = "634708322772702206";
            SAVE_LOCATION = SAVE_LOCATION + filename;
            Uri saveLocationUri = new Uri(SAVE_LOCATION, UriKind.RelativeOrAbsolute);
           

            //向服务器请求sas
           // string patient

            var http = new Http();
            long A = System.DateTime.Today.Ticks;
            string uri = "http://echelper.cloudapp.net/Service.svc/doctor/liaomin/downloadrequest?" + A;

            http.StartRequest(@uri,
                result =>
                {
                    //    A = result;
                    //    x = 1;

                    //    getfinished = false;
                    Dispatcher.BeginInvoke(() =>GetSas(result));
                });



       

        }


        private void GetSas(string result)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(string));
            result="<root>"+result+"</root>";
            XDocument document = XDocument.Parse(result);
            sas = document.Root.Value;
          //  ECGName = "634708322772702206";

            string fileDownload = "https://echelperspace.blob.core.windows.net/" + Patientid + "/" + ECGName + "/" + sas;
            // Uri fileDownloadUri = new Uri("https://echelperspace.blob.core.windows.net/{patientuser}/{filename}+{sas}");

            Uri fileDownloadUri = new Uri(fileDownload);

            

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists("shared/transfers"))
                {
                    isoStore.CreateDirectory("shared/transfers");
                }
            }
            //解析文件名，赋给SAVE_LOCATION
     //       SAVE_LOCATION = SAVE_LOCATION + filename;
            Uri saveLocationUri = new Uri(SAVE_LOCATION, UriKind.RelativeOrAbsolute);
            using (IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.FileExists(SAVE_LOCATION))
                {
                    userStore.DeleteFile(SAVE_LOCATION);
                }
            }

       
            _currentRequest = new BackgroundTransferRequest(fileDownloadUri, saveLocationUri);
            _currentRequest.Method = "GET";
            _currentRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
           // try
            //{
                foreach (BackgroundTransferRequest request in BackgroundTransferService.Requests)
                {
                      BackgroundTransferService.Remove(request);
                }

                BackgroundTransferService.Add(_currentRequest);

                InitializeTransferRequestEventHandlers();
           // }
            ////catch (InvalidOperationException)
            //{
            //    textBlock_Status.Text = "wocao";
            //}
            //sas = (string)serializer.Deserialize(document.CreateReader());
        }


        private void InitializeTransferRequestEventHandlers()
        {
            if (_currentRequest != null)
            {
                _currentRequest.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(_currentRequest_TransferStatusChanged);
            }
        }

        private void _currentRequest_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            RefreshTransferStatusUI();
        }


        private void RefreshTransferStatusUI()
        {
            String statusMessage = "";
            if (_currentRequest != null)
            {
                if (_currentRequest.TransferStatus == TransferStatus.Completed && _currentRequest.TransferError != null)
                {
                    // statusMessage = String.Format("Status: {0}", _currentRequest.TransferError.Message);
                    statusMessage = "TransferError";
                }
                else if (_currentRequest.TransferStatus == TransferStatus.Completed)
                {
                    //statusMessage = String.Format("Status: {0}", _currentRequest.TransferStatus);
                    //完成的文字状态
                    statusMessage = "Completed";
                }
            }
            textBlock_Status.Text = statusMessage;
        }

        //private void RefreshTransferProgressUI()
        //{
        //    String progressMessage = "";
        //    if (_currentRequest != null)
        //    {
        //        progressMessage = String.Format("Progress: {0} bytes / {1} bytes",
        //            _currentRequest.BytesReceived, _currentRequest.TotalBytesToReceive);
        //    }

        //    progressText.Text = progressMessage;
        //}
        //===================================  blob 相关函数定义




















        //划动手势的定义

        private void GestureListenerFlick_ECG(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {//在此处传递的参数应当包含病人的ID,等到整合时要填上
            if (textBlock_Status.Text == "Completed")
            {
                this.NavigationService.Navigate(new Uri("/PatientDescription.xaml", UriKind.Relative));
            }

        }

        private void GestureListenerFlick_PD(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {//在此处传递的参数应当包含病人的ID,等到整合时要填上

            //var app = App.Current as App;
            //app.PatientDescription = (PatientUserDataContract)patientdesc;
            this.NavigationService.Navigate(new Uri("/Description.xaml", UriKind.Relative));

        }

        private void GestureListenerFlick_RD(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {//在此处传递的参数应当包含病人的ID,等到整合时要填上
            this.NavigationService.Navigate(new Uri("/recordList.xaml", UriKind.Relative));

        }


        private void GestureListenerFlick_GD(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Diangose.xaml", UriKind.Relative));
        }


        //手势划动定义完成===================================

      

        private void Tap_Canvas(object sender, System.Windows.Input.GestureEventArgs e)
        {

            var app = App.Current as App;
            app.PatientDescription = (PatientUserDataContract)patientdesc;
            if (textBlock_Status.Text == "Completed")
            {
                this.NavigationService.Navigate(new Uri("/PatientDescription.xaml", UriKind.Relative));
            }
            else
            {
                textBlock_Status.Text = "No ECG";
            }

        }

        private void Tap_Diag(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Diangose.xaml", UriKind.Relative));
        }

        private void Pres_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Diangose.xaml", UriKind.Relative));
        }

		private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
        }
		
		
		//================================
        //protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    var targetPage = e.Content as totalArrangement;
        //    if (targetPage != null)
        //    {  
        //        List<Patient> patientlist=new List<Patient>();
				
        //        Patient item1 = new Patient() { Name = "Qian Zichen", Time = "2012/03/29" };
        //        Patient item2 = new Patient() { Name = "Sun Ruilin", Time = "2012/03/29" };
        //        Patient item3 = new Patient() { Name = "Li Che", Time = "2012/03/29" };
        //        Patient item4 = new Patient() { Name = "Zhou Yaojie", Time = "2012/03/29" };
        //        Patient item5 = new Patient() { Name = "Wu Yuanhang", Time = "2012/03/29" };
        //        Patient item6 = new Patient() { Name = "Zheng Shengrui", Time = "2012/03/29" };
        //        Patient item7 = new Patient() { Name = "Wang Xiujie", Time = "2012/03/29" };
        //        Patient item8 = new Patient() { Name = "Ma Zhiyuan", Time = "2012/03/30" };
        //        Patient item9 = new Patient() { Name = "Guo Xiaotian", Time = "2012/03/30" };
        //        Patient item10 = new Patient() { Name = "Yin Tiexin", Time = "2012/03/30" };
        //        Patient item11 = new Patient() { Name = "Pei Fei", Time = "2012/03/30" };
        //        Patient item12 = new Patient() { Name = "Deng Daiyan", Time = "2012/03/30" };
        //        Patient item13 = new Patient() { Name = "Jiang Lianer", Time = "2012/03/30" };
        //        Patient item14 = new Patient() { Name = "Meng JiaDi", Time = "2012/03/31" };
        //        Patient item15 = new Patient() { Name = "Chen Feng", Time = "2012/03/31" };
        //        Patient item16 = new Patient() { Name = "Ren Huchong", Time = "2012/03/31" };
        //        Patient item17 = new Patient() { Name = "Quan Gongming", Time = "2012/03/31" };

        //        patientlist.Add(item1);
        //        patientlist.Add(item2);
        //        patientlist.Add(item3);
        //        patientlist.Add(item4);
        //        patientlist.Add(item5);
        //        patientlist.Add(item6);
        //        patientlist.Add(item7);
        //        patientlist.Add(item8);
        //        patientlist.Add(item9);
        //        patientlist.Add(item10);
        //        patientlist.Add(item11);
        //        patientlist.Add(item12);
        //        patientlist.Add(item13);
        //        patientlist.Add(item14);
        //        patientlist.Add(item15);
        //        patientlist.Add(item16);
        //        patientlist.Add(item17);
				
				
        //        targetPage.totalpatientlist=patientlist;
        //    //    targetPage.Doctor_Name = DoctorName;
        //    }
        //}

		
        ////=================================
		
		
		
		
		
        private void Inquire_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Diangose.xaml", UriKind.Relative));
        }

        private void Tap_ECG(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PatientDescription.xaml", UriKind.Relative));
        }

        private void Tap_imecg(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PatientDescription.xaml", UriKind.Relative));
        }



       
    }
}