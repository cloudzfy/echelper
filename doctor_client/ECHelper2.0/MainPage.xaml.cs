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
using System.Text;
using System.Windows.Navigation;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.IO;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;



namespace ECHelper2._0
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数

        GeoCoordinateWatcher watcher;

        public MainPage()
        {
            InitializeComponent();


            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No network connection available!");
                return;
            }

            //var uriParams = new Dictionary<string, string>() {
            //     {"client_id", "000000004009F70C"},
            //     {"response_type", "token"},
            //     {"scope", "wl.signin,wl.basic,wl.offline_access"},
            //     {"redirect_uri", "https://oauth.live.com/desktop"},
            //     {"display", "touch"}
            //  };
            //StringBuilder urlBuilder = new StringBuilder();
            //foreach (var current in uriParams)
            //{
            //    if (urlBuilder.Length > 0)
            //    {
            //        urlBuilder.Append("&");
            //    }
            //    var encoded = HttpUtility.UrlEncode(current.Value);
            //    urlBuilder.AppendFormat("{0}={1}", current.Key, encoded);
            //}
            //var loginUrl = "https://oauth.live.com/authorize?" + urlBuilder.ToString();

            //AuthenticationBrowser.Navigate(new Uri(loginUrl));
            //AuthenticationBrowser.Visibility = Visibility.Visible;


        }

        //private void BTN_CLICK(object sender, RoutedEventArgs e)
        //{
        //    this.NavigationService.Navigate(new Uri("/DoctorInformation.xaml", UriKind.Relative));
        //}


        //public string AccessToken { get; set; }
        //private void BrowserNavigated(object sender, NavigationEventArgs e)
        //{
        //    if (e.Uri.AbsoluteUri.ToLower().Contains("https://oauth.live.com/desktop"))
        //    {
        //        string text = HttpUtility.HtmlDecode(e.Uri.Fragment).TrimStart('#');
        //        var pairs = text.Split('&');
        //        foreach (var pair in pairs)
        //        {
        //            var kvp = pair.Split('=');
        //            if (kvp.Length == 2)
        //            {
        //                if (kvp[0] == "access_token")
        //                {
        //                    AccessToken = kvp[1];
        //                    //   MessageBox.Show("Access granted" + AccessToken);
        //                    this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
        //                }
        //            }
        //        }
        //        if (string.IsNullOrEmpty(AccessToken))
        //        {
        //            MessageBox.Show("Unable to authenticate");
        //        }
        //        AuthenticationBrowser.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //}




        //private void RequestUserProfile()
        //{
        //    var profileUrl = string.Format("https://apis.live.net/v5.0/me?access_token={0}", HttpUtility.UrlEncode(AccessToken));
        //    var request = HttpWebRequest.Create(new Uri(profileUrl));
        //    request.Method = "GET";
        //    request.BeginGetResponse(result =>
        //    {
        //        try
        //        {
        //            var resp = (result.AsyncState as HttpWebRequest).EndGetResponse(result);
        //            using (var strm = resp.GetResponseStream())
        //            {
        //                var serializer = new DataContractJsonSerializer(typeof(WindowsLiveProfile));
        //                var profile = serializer.ReadObject(strm) as WindowsLiveProfile;
        //                this.Dispatcher.BeginInvoke((Action<WindowsLiveProfile>)((user) =>
        //                {
        //                    // this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
        //                    this.UserIdText.Text = user.Id;

        //                    this.UserNameText.Text = user.Name;
        //                }), profile);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            this.Dispatcher.BeginInvoke(() =>
        //            MessageBox.Show("Unable to attain profile information"));
        //        }
        //    }, request);
        //}

        //[DataContract]
        //public class WindowsLiveProfile
        //{
        //    [DataMember(Name = "id")]
        //    public string Id { get; set; }
        //    [DataMember(Name = "name")]
        //    public string Name { get; set; }
        //}

        //private void AuthenticateClick(object sender, RoutedEventArgs e)
        //{
        //    var uriParams = new Dictionary<string, string>() {
        //         {"client_id", "000000004009F70C"},
        //         {"response_type", "token"},
        //         {"scope", "wl.signin,wl.basic,wl.offline_access"},
        //         {"redirect_uri", "https://oauth.live.com/desktop"},
        //         {"display", "touch"}
        //      };
        //    StringBuilder urlBuilder = new StringBuilder();
        //    foreach (var current in uriParams)
        //    {
        //        if (urlBuilder.Length > 0)
        //        {
        //            urlBuilder.Append("&");
        //        }
        //        var encoded = HttpUtility.UrlEncode(current.Value);
        //        urlBuilder.AppendFormat("{0}={1}", current.Key, encoded);
        //    }
        //    var loginUrl = "https://oauth.live.com/authorize?" + urlBuilder.ToString();

        //    AuthenticationBrowser.Navigate(new Uri(loginUrl));
        //    AuthenticationBrowser.Visibility = Visibility.Visible;
        //}

        private void BTN_CLICK(object sender, RoutedEventArgs e)
        {
            // ===========================这个是推送发的信息
            //sendPosition();


            this.NavigationService.Navigate(new Uri("/totalArrangement.xaml", UriKind.Relative));
        }


        // ===========================这个是推送发的信息
        private void sendPosition()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 1;
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();
            }

        }


        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Location location = new Location();

            location.Latitude = e.Position.Location.Latitude;
            location.Longitude = e.Position.Location.Longitude;

            string Latitude = location.Latitude.ToString();
            string Longitude = location.Longitude.ToString();
        }

        // ===========================这个是推送发的信息



    }
}