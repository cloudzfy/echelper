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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;


namespace ECHelper2._0
{
    public class MailDataContract
    {
        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Time")]
        public string Time { get; set; }

        [XmlElement("DoctorId")]
        public string DoctorId { get; set; }

        [XmlElement("PatientId")]
        public string PatientId { get; set; }

       

        [XmlElement("TextContent")]
        public string TextContent { get; set; }

        [XmlElement("MailId")]
        public string MailId { get; set; }

        [XmlElement("IsRead")]
        public string IsRead { get; set; }

        [XmlElement("FromOrTo")]
        public string FromOrTo { get; set; }

        [XmlElement("ECG")]
        public string ECG { get; set; }

  



    }
}
