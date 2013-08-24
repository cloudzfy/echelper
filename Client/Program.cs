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
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
namespace RESTfulWCFClient
{
    class Program
    {
        static string serviceNamespace = "echelperacs";
        static string acsHostUrl = "https://echelperacs.accesscontrol.windows.net/";
        static string realm = "your realm";
        static string uid = "your service identity name";
        static string pwd = "your service identity password";

        static void Main(string[] args)
        {
            string token = GetTokenFromACS(realm);

            WebClient client = new WebClient();

            string headerValue = string.Format("WRAP access_token=\"{0}\"", token);

            client.Headers.Add("Authorization", headerValue);


            Stream stream = client.OpenRead(@"http://localhost:yourDevIISPort/RESTfulWCFUsersServiceEndPoint.svc/users");

            StreamReader reader = new StreamReader(stream);
            String response = reader.ReadToEnd();
            Console.Write(response);
            Console.ReadLine();

        }

        private static string GetTokenFromACS(string scope)
        {
            string wrapPassword = pwd;
            string wrapUsername = uid;

            // request a token from ACS
            WebClient client = new WebClient();
            client.BaseAddress = string.Format("https://{0}.{1}", serviceNamespace, acsHostUrl);

            NameValueCollection values = new NameValueCollection();
            values.Add("wrap_name", wrapUsername);
            values.Add("wrap_password", wrapPassword);
            values.Add("wrap_scope", scope);

            byte[] responseBytes = client.UploadValues("WRAPv0.9/", "POST", values);

            string response = Encoding.UTF8.GetString(responseBytes);

            Console.WriteLine("\nreceived token from ACS: {0}\n", response);

            return HttpUtility.UrlDecode(
                response
                .Split('&')
                .Single(value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase))
                .Split('=')[1]);
        }

    }
}
