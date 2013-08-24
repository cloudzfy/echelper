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
using System.Windows.Threading;
using ECHelper2;

namespace ECHelper2._0
{
    public partial class PatientDescription : PhoneApplicationPage
    {
        public PatientDescription()
        {
            InitializeComponent();
            LoadECG();
         //  updateMyScreen();
        }

        private void LoadECG()
        {
            var ecg = new ECGDisplay();
            ECGCanvas.Children.Add(ecg);
        }


        //void updateMyScreen()
        //{
        //    int i, iterator;

        //    Polyline ChartLine = new Polyline();
        //    ChartLine.Stroke = new SolidColorBrush(Colors.Red);
        //    Random a = new Random();



        //    for (i = 5; i < ECGCanvas.Height; i += 20)
        //    {
        //        Line BaseLine = new Line();
        //        BaseLine.Stroke = new SolidColorBrush(Colors.Green);

        //        BaseLine.X1 = 0;
        //        BaseLine.Y1 = i;
        //        BaseLine.X2 = ECGCanvas.Width;
        //        BaseLine.Y2 = i;
        //        ECGCanvas.Children.Add(BaseLine);

        //    }

        //    for (i = 5; i < ECGCanvas.Width; i += 20)
        //    {
        //        Line VerLine = new Line();
        //        VerLine.Stroke = new SolidColorBrush(Colors.Green);

        //        VerLine.X1 = i;
        //        VerLine.Y1 = 0;
        //        VerLine.X2 = i;
        //        VerLine.Y2 = ECGCanvas.Height;
        //        ECGCanvas.Children.Add(VerLine);

        //    }


        //    for (iterator = 0; iterator < 400; iterator += 10)
        //    {

        //        double b = a.Next(1, 10);
        //        double currentXOnGraph = Math.Abs((b * 40));
        //        ChartLine.Points.Add(new Point(iterator, currentXOnGraph));
        //    }
        //    ECGCanvas.Children.Add(ChartLine);
        //}






       private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Description.xaml", UriKind.Relative));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/recordList.xaml", UriKind.Relative));
        }

        private Point currentTransform = new Point(0, 0);

        protected void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            currentTransform = e.CumulativeManipulation.Translation;
        }

    }
}