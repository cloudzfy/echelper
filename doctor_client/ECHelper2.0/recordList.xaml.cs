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
    public partial class recordList : PhoneApplicationPage
    {
        string Patientid;

        public recordList()
        {
            InitializeComponent();

            var app = App.Current as App;
            // mail = app.selectedPatient;
            Patientid = app.selectedPatient.PatientId;

            var http = new Http();

            long A = System.DateTime.Today.Ticks;
            string uri = "http://echelper.cloudapp.net/Service.svc/patient/" + Patientid + "/recordlist?"+A;
            http.StartRequest(@uri,
                result =>
                {
                    //    A = result;
                    //    x = 1;

                    //    getfinished = false;
                    Dispatcher.BeginInvoke(() => showrecordlist(result));
                });

        }

        private void showrecordlist(string result)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfMailDataContract));

            result = "<root>" + result + "</root>";
            XDocument document = XDocument.Parse(result);


            ArrayOfMailDataContract records = (ArrayOfMailDataContract)serializer.Deserialize(document.CreateReader());

            this.listBox_Record.ItemsSource = records.Collection;

        }

        private void patientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var app = App.Current as App;
            app.selectedRecord = (MailDataContract)listBox_Record.SelectedItem;
            this.NavigationService.Navigate(new Uri("/record.xaml", UriKind.Relative));

        }

    }
}