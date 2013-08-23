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

namespace ECHelper2._0
{
    public partial class DoctorShortDescription : PhoneApplicationPage
    {
        static readonly string[] BirthYears = { "1952", "1953", "1954", "1955", "1956", "1958", "1958", "1959", "1960" ,"1961","1962","1963",
                                              "1964","1965","1966","1968","1968","1969","1980","1981","1982","1983","1984","1985","1986","1988","1988","1989",
                                              "1980","1981","1982","1983","1984","1985","1986","1987","1988","1989","1990","1991","1992","1993","1994"};
        String Doctor_Short_Description;
         public String DoctorName;


        public DoctorShortDescription()
        {
            InitializeComponent();
            DataContext = BirthYears;
        }

        private void btn_Save_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Doctor_Short_Description = DoctorName + ", birth in "+textBlock_BirthYear.Text+". Graduate at "+ textBox_School+" get the degree of  "+textBlock_Degree+" "+textBox_Others.Text;
            //将医生的信息保存到本地的存储，等到上一个一面保存时一并保存到服务器
            textBlock_Save_Status.Text ="Save Successfully";
           // textBlock_Save_Status.Text = DoctorName;
            this.NavigationService.Navigate(new Uri("/DoctorInformation.xaml",UriKind.Relative));

        }
		
		private void btn_EM_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Doctor_Short_Description = DoctorName + ", birth in "+textBlock_BirthYear.Text+". Graduate at "+ textBox_School+" get the degree of  "+textBlock_Degree+" "+textBox_Others.Text;
            //将医生的信息保存到本地的存储，等到上一个一面保存时一并保存到服务器
            //textBlock_Save_Status.Text ="Save Successfully";
           // textBlock_Save_Status.Text = DoctorName;
            this.NavigationService.Navigate(new Uri("/Emergency.xaml",UriKind.Relative));

        }


        //此处定义将医生简介发送到上一个编辑界面
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var targetPage = e.Content as DoctorInformation;
            if (targetPage != null)
            {
              //  Doctor_SD = Doctor_Short_Description;
                targetPage.Doctor_SD = Doctor_Short_Description;
            }
        }


        public String Doctor_Name { get; set; }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Doctor_Name != null)
            {
                DoctorName = Doctor_Name;
            }
        }

        private void GF_Degree(object sender, RoutedEventArgs e)
        {
            if (textBox_Degree.Text == "MA")
            { 
                textBox_Degree.Text = "";
            }
        }

        private void GF_School(object sender, RoutedEventArgs e)
        {
            if (textBox_School.Text == "Tianjin Medical University")
            { textBox_School.Text = ""; }
        }

        private void GF_Others(object sender, RoutedEventArgs e)
        {
            if (textBox_Others.Text == "Enter Other Information about You")
            {
                textBox_Others.Text = "";

            }// if
        }

        private void LF_Degree(object sender, RoutedEventArgs e)
        {
            if (textBox_Degree.Text == "")
            {
                textBox_Degree.Text = "MA";
            }
        }

        private void LF_School(object sender, RoutedEventArgs e)
        {
            if (textBox_School.Text == "")
            { textBox_School.Text = "Tianjin Medical University"; }
        }

        private void LF_Others(object sender, RoutedEventArgs e)
        {
            if (textBox_Others.Text == "")
            {
                textBox_Others.Text = "Enter Other Information about You";

            }// if

        } 



    }
}