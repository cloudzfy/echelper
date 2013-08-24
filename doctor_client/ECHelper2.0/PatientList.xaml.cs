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
using System.Xml.Linq;

namespace ECHelper2._0
{
    public partial class PatientList : PhoneApplicationPage
    {
        List<Patient> list = new List<Patient>();

        public PatientList()
        {
            InitializeComponent();
            //cong fu wu qi duan xia zai bing ren xin xi bing xian shi
            showPatientsList();
        }

        public void showPatientsList()
        {
           // listBox_PatientsList.Items.Add("Steve Jobs" + "  10:00");
           // listBox_PatientsList.Items.Add("Bill Gates" + "  10:20");

            //此处为下载Azure端的数据，从而能够获取最近病人的信息。等到整合的时候在同一改好
            //XDocument loadedData = XDocument.Load("People.xml");

            //var data = from query in loadedData.Descendants("person")
            //           select new Person
            //           {
            //               FirstName = (string)query.Element("firstname"),
            //               LastName = (string)query.Element("lastname"),
            //               Age = (int)query.Element("age")
            //           };
            //listBox_PatientsList.ItemsSource = data;

            //=======================================下面的这是添加的本地的数据，以后如果晋级的话要改成从服务器端获取
            //Patient item0 = new Patient() { Name = "Zhao Yingxiang", Time = "Today" };
            //Patient item1 = new Patient() { Name = "Tian Wenbiao", Time = "2012/03/28" };
            //Patient item2 = new Patient() { Name = "Zhang Liting", Time = "2012/03/28" };
            //Patient item3 = new Patient() { Name = "Xiao Xin", Time = "2012/03/28" };
            //Patient item4 = new Patient() { Name = "Cheng Zhongxiao", Time = "2012/03/28" };
            //Patient item5 = new Patient() { Name = "Wu Jinqing", Time = "2012/03/28" };
            //Patient item6 = new Patient() { Name = "Lai Yueding", Time = "2012/03/28" };

            //list.Add(item0);
            //list.Add(item1);
            //list.Add(item2);
            //list.Add(item3);
            //list.Add(item4);
            //list.Add(item5);
            //list.Add(item6);

            var app = App.Current as App;
            listBox_PatientsList.ItemsSource = app.checkedmail;

            //=======================================上面的这是添加的本地的数据，以后如果晋级的话要改成从服务器端获取





        }

        private void GestureListenerDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            // 在此处应添加病人ID,并作为参数传递
            this.NavigationService.Navigate(new Uri("/Record.xaml", UriKind.Relative));
        }

        private void GestureListenerHold(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            // 在此处应添加病人ID,并作为参数传递
            this.NavigationService.Navigate(new Uri("/Record.xaml", UriKind.Relative));

        }

       // private int GetBeforeIndex(object sender, RoutedEventArgs e)
       // {
       //     return 
       // }

        private void GestureListenerTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
          //  if(listBox_PatientsList.SelectedItem==null)
          //    if (this.listBox_PatientsList== listBox_PatientsList.SelectedItem)
          //  {
            // 在此处应添加病人ID,并作为参数传递
            this.NavigationService.Navigate(new Uri("/Record.xaml", UriKind.Relative));

          //   }   
        }

    }
}