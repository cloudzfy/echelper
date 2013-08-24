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
    public partial class Record : PhoneApplicationPage
    {
        MailDataContract record;
        //MailDataContract mail;
        PatientUserDataContract patientdesp;
        public Record()
        {
            InitializeComponent();

            var app=App.Current as App;
            record=app.selectedRecord;

            patientdesp = app.PatientDescription;
          //  mail = app.selectedPatient;

            TextBlock_Name.Text = "Name : "+record.PatientId;
            TextBlock_Age.Text = "Age : "+ patientdesp.Age;
            TextBlock_Gender.Text = "Gender : "+ patientdesp.Gender;
            TextBlock_Desp.Text = record.TextContent;
            
            TextBlock_DoctorId.Text =record.DoctorId;
            TextBlock_Time.Text = record.Time;


        

        }

        

    }
}