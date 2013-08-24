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
using Microsoft.Phone.Notification;
using System.Diagnostics;
using System.IO;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;


namespace ECHelper2._0
{
    public partial class ShowRoute : PhoneApplicationPage
    {
        BingMapsDirectionsTask bingMapsDirectionsTask = new BingMapsDirectionsTask();

        public ShowRoute()
        {
            InitializeComponent();
        //    reallyshowroute();

        }

        //public void reallyshowroute()
        //{
        //    GeoCoordinate spaceNeedleLocation = new GeoCoordinate(40.107, 116.3333);
        //    LabeledMapLocation spaceNeedleLML = new LabeledMapLocation("Space Needle", null);
        //    bingMapsDirectionsTask.End = spaceNeedleLML;
        //    bingMapsDirectionsTask.Show();
        //}

    }
}