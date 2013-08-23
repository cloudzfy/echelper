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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Xml.Linq;

//using Microsoft.Phone.PhoneApplication;

namespace ECHelper2._0
{
    public partial class arrangeList : UserControl
    {
        string name, time;

        public arrangeList()
        {
            InitializeComponent();
            showCurrentlist();
        }

        private void showCurrentlist()
        {
            //


         //   DateTime dt = System.DateTime.Now; 
            //xian shi NOW hou mian de shu ju



            for (int i = 1; i < 30; i++)
            {
                loadlist();
                // xian shi dang tian d xin xi
                lstArrange.Items.Add(name + "   " + time);
            }
        }

        private void loadlist()
        {
            // cong shu ju ku zhong xia zai wei zhen duan bing ren de xing ming he shi jian 
            name = "Andrew Jald";
            time = "2012/02/03/14:50";
       //     XDocument loadedData = XDocument.Load("People.xml");

            //var data = from query in loadedData.Descendants("person")
            //           select new Person
            //           {
            //               FirstName = (string)query.Element("firstname"),
            //               LastName = (string)query.Element("lastname"),
            //               Age = (int)query.Element("age")
            //           };
            //listBox.ItemsSource = data;
        }

        private void GestureListenerTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {//在此处传递的参数应当包含病人的ID,等到整合时要填上
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/diagnosisNagivation.xaml", UriKind.Relative));

        }


    }
}
