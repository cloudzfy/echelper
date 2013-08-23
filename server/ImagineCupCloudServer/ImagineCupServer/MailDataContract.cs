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
using System.Web;
using System.Runtime.Serialization;

namespace ImagineCupServer
{
    [DataContract(Namespace = "")]
    public class MailDataContract
    {
        [DataMember]
        public int MailId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public String TextContent { get; set; }
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public int FromOrTo { get; set; }
        [DataMember]
        public String PatientId { get; set; }
        [DataMember]
        public String DoctorId { get; set; }
        [DataMember]
        public int IsRead { get; set; }
        [DataMember]
        public String ECG { get; set; }
    }
}