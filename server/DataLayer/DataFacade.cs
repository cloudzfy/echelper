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
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.ServiceModel.Web;

namespace DataLayer
{
    public static class DataFacade
    {
        static SerializableDictionary<string, PatientUser> patientUsers = new SerializableDictionary<string, PatientUser>();
        static SerializableDictionary<string, DoctorUser> doctorUsers = new SerializableDictionary<string, DoctorUser>();
        static SerializableDictionary<string, Topics> topics = new SerializableDictionary<string, Topics>();
        static SerializableDictionary<string, Advices> advices = new SerializableDictionary<string, Advices>();
        static SerializableDictionary<string, Mail> mail = new SerializableDictionary<string, Mail>();
        static string basePath = HostingEnvironment.ApplicationPhysicalPath + "App_Data";
        static string patientuserpath = basePath + "\\PatientUser.xml";
        static string doctoruserpath = basePath + "\\DoctorUsers.xml";
        static string topicspath = basePath + "\\Topics.xml";
        static string advicespath = basePath + "\\Advices.xml";
        static string mailpath = basePath + "\\Mails.xml";

        static private void SerializeToXml(Type t, object o, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(t);
            TextWriter textWriter = new StreamWriter(fileName);
            serializer.Serialize(textWriter, o);
            textWriter.Close();
        }

        static private object DeseializeFromXml(Type t, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(t);
            TextReader textReader = new StreamReader(fileName);
            Object o = serializer.Deserialize(textReader) as SerializableDictionary<string, PatientUser>;
            textReader.Close();
            return o;
        }

        static public void SerializeToXml()
        {
            SerializeToXml(typeof(SerializableDictionary<string, PatientUser>), patientUsers, patientuserpath);
            SerializeToXml(typeof(SerializableDictionary<string, DoctorUser >), doctorUsers, doctoruserpath);
            SerializeToXml(typeof(SerializableDictionary<string, Topics>),topics, topicspath);
            SerializeToXml(typeof(SerializableDictionary<string, Advices>), advices, advicespath);
        }
        static public void DeseializeFromXml() {
            if (File.Exists(patientuserpath))
            {
                patientUsers = DeseializeFromXml(typeof(SerializableDictionary<string, PatientUser>), patientuserpath)
                    as SerializableDictionary<string, PatientUser>;
            }
            if (patientUsers == null)
            {
                patientUsers = new SerializableDictionary<string, PatientUser>();
            }
            if (File.Exists(topicspath))
            {
               topics = DeseializeFromXml(typeof(SerializableDictionary<string, Topics>), topicspath)
                    as SerializableDictionary<string,Topics>;
            }
            if (topics == null)
            {
                topics = new SerializableDictionary<string, Topics>();
            }
            //======================================
            if (File.Exists(doctoruserpath))
            {
                doctorUsers = DeseializeFromXml(typeof(SerializableDictionary<string, DoctorUser>), doctoruserpath)
                    as SerializableDictionary<string, DoctorUser>;
            }
            if (doctorUsers == null)
            {
               doctorUsers = new SerializableDictionary<string, DoctorUser>();
            }
            if (File.Exists(advicespath))
            {
                advices = DeseializeFromXml(typeof(SerializableDictionary<string, Advices>), advicespath)
                     as SerializableDictionary<string,Advices>;
            }
            if (advices == null)
            {
                advices = new SerializableDictionary<string, Advices>();
            }
        }

        static DataFacade()
        {
            DeseializeFromXml();
        }

        static public PatientUser GetPatientUser(string username) {

            if (!patientUsers.ContainsKey(username))
            {
                throw new NotFoundException("The user was not created yet");
            }
            return patientUsers[username];
        }

        static public DoctorUser GetDoctorUser(string username) {
            if (!doctorUsers.ContainsKey(username)) { 
                throw new NotFoundException("The user was not created yet");
            }
            return doctorUsers[username];
        }

        static public PatientUser UpdatePatientUser(string username,string nickname,string gender,int age,string description,string allery) {
            if (!patientUsers.ContainsKey(username)) {
                throw new NotFoundException("The user was not created yet");
            }
            patientUsers[username].NickName = nickname;
            patientUsers[username].Description = description;
            patientUsers[username].Gender = gender;
            patientUsers[username].Age = age;
            patientUsers[username].Allery = allery;
            return patientUsers[username];
        }

        static public DoctorUser UpdateDoctorUser(string username, string nickname, string grade, string description,string email,string phone,string image)
        {
            if (!doctorUsers.ContainsKey(username))
            {
                throw new NotFoundException("The user was not created yet");
            }
            doctorUsers[username].NickName = nickname;
            doctorUsers[username].Grade = grade;
            doctorUsers[username].Description = description;
            doctorUsers[username].email = email;
            doctorUsers[username].phone = phone;
            doctorUsers[username].Image = image;
            return doctorUsers[username];
        }

        static public void FollowPatientToDoctor(string fromuser, string touser) {
            if (!patientUsers.ContainsKey(fromuser) || !doctorUsers.ContainsKey(touser)) {
                throw new NotFoundException("The user is not created yet");
            }
            if (patientUsers[fromuser].FollowingList == null) {
                patientUsers[fromuser].FollowingList = new List<string>();
            }
            patientUsers[fromuser].FollowingList.Add(touser);
            if (doctorUsers[touser].FollowedList == null) {
                doctorUsers[touser].FollowedList = new List<string>();
            }
            doctorUsers[touser].FollowedList.Add(fromuser);
            SerializeToXml();

        }

        static public void FollowDoctorToPatient(string fromuser, string touser)
        {
            if (!doctorUsers.ContainsKey(fromuser) || !patientUsers.ContainsKey(touser))
            {
                throw new NotFoundException("The user is not created yet");
            }
            if (doctorUsers[fromuser].FollowingList == null)
            {
                doctorUsers[fromuser].FollowingList = new List<string>();
            }
            doctorUsers[fromuser].FollowingList.Add(touser);
            if (patientUsers[touser].FollowedList == null)
            {
               patientUsers[touser].FollowedList = new List<string>();
            }
            patientUsers[touser].FollowedList.Add(fromuser);
            SerializeToXml();

        }

        static public Topics AddNewTopic(string patientname, string text, DateTime time) {
            string id = Guid.NewGuid().ToString();
            topics.Add(id,
                new Topics { 
                    TopicId = id,
                    UserName = patientname,
                    Text = text,
                    Time = time

                });
            SerializeToXml();
            return topics[id];
        }
        static public Advices AddNewAdvice(string doctorname, string topicId, string text, DateTime time) {
            string id = Guid.NewGuid().ToString();
            advices.Add(id,
                new Advices {
                    AdviceId = id,
                    UserName = doctorname,
                    Time = time,
                    TopicId = topicId
                });
            SerializeToXml();
            return advices[id];
        }
        static public Mail AddNewMail(string title, string text, DateTime time,bool fromto,string patientusername,string doctorusername,string ecg)
        {
            string id = Guid.NewGuid().ToString();
            mail.Add(id,
                new Mail
                {
                    MailId = id,
                    Title = title,
                    Text = text,
                    FromTo = fromto,
                    Time = time,
                    DoctorUsername = doctorusername,
                    PatientUsername = patientusername,
                    ECG = ecg

                });
            SerializeToXml();
            return mail[id];
        }

        static public List<Mail> GetMailList(string username) {
            List<Mail> temp = new List<Mail>();
            foreach (Mail i in mail.Values) {
                if (i.DoctorUsername.Equals(username) || i.PatientUsername.Equals(username)) {
                    temp.Add(i);
                }
            }
            return temp;
        }
        //A to B 表示A关注B A的Following
        static public bool IsFollowRelationPtoD(string from, string to) {
            List<DoctorUser> list = GetPatientUserFollowingList(from);
            foreach (DoctorUser user in list) { 
                if(string.Equals(user.Username,to)){
                    return true;
                }

            }
            return false;
        }
        static public bool IsFollowRelationDtoP(string from, string to) {
            List<PatientUser> list = GetDoctorUserFollowingList(from);
            foreach (PatientUser user in list) { 
                if(string.Equals(user.Username,to))
                {
                    return true;
                }

            }
            return false;
        }

        public static List<DoctorUser> GetPatientUserFollowedList(string username) {
            if (!patientUsers.ContainsKey(username)) {
                throw new NotFoundException("The user is not created yet");
            }
            List<DoctorUser> list = new List<DoctorUser>();
            foreach (string user in patientUsers[username].FollowedList) {
                list.Add(doctorUsers[user]);
            }
            return list;

        }
        public static List<DoctorUser> GetPatientUserFollowingList(string username)
        {
            if (!patientUsers.ContainsKey(username))
            {
                throw new NotFoundException("The user is not created yet");
            }
            List<DoctorUser> list = new List<DoctorUser>();
            foreach (string user in patientUsers[username].FollowingList)
            {
                list.Add(doctorUsers[user]);
            }
            return list;

        }
        public static List<PatientUser> GetDoctorUserFollowedList(string username) {
            if (!doctorUsers.ContainsKey(username)) {
                throw new NotFoundException("The user is not created yet");
            }
            List<PatientUser> list = new List<PatientUser>();
            foreach (string user in doctorUsers[username].FollowedList) {
                list.Add(patientUsers[user]);
            }
            return list;
        }
        public static List<PatientUser> GetDoctorUserFollowingList(string username)
        {
            if (!doctorUsers.ContainsKey(username))
            {
                throw new NotFoundException("The user is not created yet");
            }
            List<PatientUser> list = new List<PatientUser>();
            foreach (string user in doctorUsers[username].FollowingList)
            {
                list.Add(patientUsers[user]);
            }
            return list;
        }
        //=====================================
        public static List<Topics> GetTopicsByUserName(string userName)
        {
            List<Topics> blogList = new List<Topics>();
            foreach (Topics blog in topics.Values)
            {
                if (string.Equals(blog.UserName, userName, StringComparison.Ordinal))
                {
                    blogList.Add(blog);
                }
            }
            return blogList;
        }
       
    }
}
