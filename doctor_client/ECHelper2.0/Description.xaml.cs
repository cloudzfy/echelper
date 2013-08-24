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

namespace ECHelper2._0
{
    public partial class Description : PhoneApplicationPage
    {

        PatientUserDataContract desp;
        string Patientid;

        public Description()
        {
            InitializeComponent();

            loadDes_and_Record();
            //var app = App.Current as App;
            //desp = app.PatientDescription;

            //textBlock_Name.Text = desp.UserName;
            //textBlock_Age.Text = desp.Age;
            //textBlock_Gender.Text = desp.Gender;
            //textBlock_AllergyDrugs.Text = desp.Allery;
            //textBlock_PatientDescription.Text = desp.Description;




        }

        private void loadDes_and_Record()
        {
            var http = new Http();

            var app = App.Current as App;
            Patientid = app.selectedPatient.PatientId;

            long A = System.DateTime.Today.Ticks;
            string uri = "http://echelper.cloudapp.net/Service.svc/doctor/" + "xiaoming/" + "outpatient/" + Patientid + "/select?"+A;
            http.StartRequest(@uri,
                result =>
                {
                    //    A = result;
                    //    x = 1;

                    //    getfinished = false;
                    Dispatcher.BeginInvoke(() => showDesp(result));
                });

        }

        private void showDesp(string result)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PatientUserDataContract));

            //   result =   result ;
            XDocument document = XDocument.Parse(result);

            PatientUserDataContract Description = serializer.Deserialize(document.CreateReader()) as PatientUserDataContract;
            // ArrayOfMailDataContract mails = (ArrayOfMailDataContract) serializer.Deserialize(document.CreateReader());

            var app = App.Current as App;
            app.PatientDescription = (PatientUserDataContract)Description;

            textBlock_Name.Text = "Name : "+Description.UserName;
            textBlock_Age.Text = "Age : "+ Description.Age;
            textBlock_Gender.Text = "Gender : "+Description.Gender;
            textBlock_AllergyDrugs.Text = "Allergy Drugs : \n"+Description.Allery;
            textBlock_PatientDescription.Text = "Description : \n"+Description.Description;

        }







        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/recordList.xaml", UriKind.Relative));
        }

        private void btnECG_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PatientDescription.xaml", UriKind.Relative));
        }
    }


    //================================以下为  http get 请求类  ================================================

    

}