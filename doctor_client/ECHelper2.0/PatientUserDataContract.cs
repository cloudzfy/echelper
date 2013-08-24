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
using System.Collections.ObjectModel;

namespace ECHelper2._0
{
    [XmlRoot("PatientUserDataContract")]
    public class PatientUserDataContract
    {
        [XmlElement("Age")]
        public string Age { get; set; }

        [XmlElement("Allery")]
        public string Allery { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("Gender")]
        public string Gender { get; set; }

        [XmlElement("NickName")]
        public string NickName { get; set; }

        [XmlElement("UserName")]
        public string UserName { get; set; }

        //public string Age { get; set; }
        //public string Allergy { get; set; }
        //public string Description { get; set; }
        //public string Gender { get; set; }
        //public string NickName { get; set; }
        //public string UserName { get; set; }

    }
}
