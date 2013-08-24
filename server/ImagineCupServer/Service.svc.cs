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
using System.ServiceModel;
using System.ServiceModel.Web;
using ImagineCupServer.DataModel;
using ImagineCupServer.DataContracts;
using System.ServiceModel.Syndication;
using System.IO;
using System.ServiceModel.Activation;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Web;


namespace ImagineCupServer
{
    [ServiceContract]
    public class Service
    {
        //echelperDBEntities context = new echelperDBEntities();
        
        static string accessToken = null;
        #region Patient API

        #region /patient/{username}/profile/select
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/profile/select")]
        public PatientUserDataContract GetPatientUser(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            List<string> keylist = context.Patient.Select(c => c.LiveID).Distinct().ToList();
            echelperDBEntities context_ = new echelperDBEntities();
            Patient data = context_.GetObjectByKey(new System.Data.EntityKey("echelperDBEntities.Patient", "LiveID", username)) as Patient;
            return ConvertPatientUserToDataContract(data);
        }
        #endregion

        #region /patient/{username}/profile/update
        [OperationContract]
        [WebInvoke(UriTemplate = "/patient/{username}/profile/update", Method = "POST")]
        public void UpdatePatientUser(string username, PatientUserDataContract user)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Patient patient = context.Patient.First(p => p.LiveID.Equals(user.UserName));
            patient.Name = user.NickName;
            patient.Gender = user.Gender;
            patient.Age = user.Age;
            patient.Description = user.Description;
            patient.Allergy = user.Allery;
            context.SaveChanges();
            
            //return user;
            //context.Refresh
        }
        #endregion

        #region /patient/{username}/contact/alldoctorlist
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/contact/alldoctorlist")]
        public List<MyDoctorDataContract> GetDoctorUserList(string username) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            List<MyDoctorDataContract> temp = new List<MyDoctorDataContract>();
            List<Doctor> tempuser = context.Doctor.ToList();
           // List<string> keylist = context.Patient.Select(c => c.LiveID).Distinct().ToList();
            //echelperDBEntities context_ = new echelperDBEntities();
            //Patient data = context_.GetObjectByKey(new System.Data.EntityKey("echelperDBEntities.Patient", "LiveID", username)) as Patient;
            Patient data = context.Patient.First(p => p.LiveID == username);
            foreach(Doctor i in tempuser){
                bool ismy = false;
                if (i.Patient.Contains(data)) {
                    ismy = true;
                }
                temp.Add(ConvertMyDoctorToDataContract(i,ismy));
            }          
            return temp;
        }
        #endregion

        #region /patient/{username}/contact/adddoctor
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/contact/adddoctor/{doctorid}")]
        public void AddDoctor(string username, string doctorid)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Patient pa = context.Patient.First(p => p.LiveID == username);
            Doctor doc = context.Doctor.First(p => p.LiveID == doctorid);
            if (!pa.Doctor.Contains(doc) && !doc.Patient.Contains(pa))
            {
                pa.Doctor.Add(doc);
                doc.Patient.Add(pa);
            }
            context.SaveChanges();
        }
        #endregion

        #region /patient/{username}/outpatient/maillist
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/outpatient/maillist")]
        public List<MailDataContract> GetPatientMailList(string username) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
             List<MailDataContract> temp = new List<MailDataContract>();
             List<Mail> mails = context.Mail.Where(a => a.PatientID == username).ToList();
            foreach (Mail i in mails) {
                temp.Add(ConvertMailToDataContract(i));
            }
            return temp;
        }
        #endregion

        #region  /patient/{username}/recordlist
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/recordlist")]
        public List<MailDataContract> GetPatientRecordList(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            List<MailDataContract> temp = new List<MailDataContract>();
            List<Mail> mails = context.Mail.Where(a => a.PatientID == username).ToList();
            foreach (Mail i in mails)
            {
                if (i.FromOrTo == 1)
                {
                    temp.Add(ConvertMailToDataContract(i));
                }
            }
            return temp;
        }
        #endregion

        #region /patient/{username}/outpatient/new
        [OperationContract]
        [WebInvoke(UriTemplate = "/patient/{username}/outpatient/new", Method = "POST")]
        public void addnewMail(string username, NewMailDataContract temp) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            DateTime time = DateTime.Now;
            Mail newMail = new Mail();
            newMail.PatientID = temp.PatientId;
            newMail.DoctorID = temp.DoctorId;
            newMail.Title = temp.Title;
            newMail.TextContent = temp.TextContent;
            newMail.Time = time;
            newMail.ECG = temp.ECG;
            newMail.FromOrTo = temp.FromOrTo;
            newMail.IsRead = 0;
            context.AddToMail(newMail);
            context.SaveChanges();

            //Patient patient = context.Patient.First(p => p.LiveID == username);
            Doctor doctor = context.Doctor.First(p => p.LiveID == temp.DoctorId);
            int count = 0;
            foreach (Mail i in context.Mail) {
                if (i.DoctorID==temp.DoctorId&&i.FromOrTo == 1 && i.IsRead==0)
                {
                    count++;
                }
            }

            string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\">" +
                           "<wp:Toast>" +
                              "<wp:Text0>0</wp:Text0>" +
                              "<wp:Text1>" + count + "</wp:Text1>" +
                              "<wp:Text2>0</wp:Text2>" +
                              "<wp:Text3>0</wp:Text3>" +
                           "</wp:Toast>" +
                        "</wp:Notification>";

            byte[] strBytes = new UTF8Encoding().GetBytes(toastMessage);
            if (doctor.Channel != null)
            {
                sendNotificationType(strBytes, doctor.Channel);
            }


            //return ConvertMailToDataContract(newMail);
        }
        #endregion

        #region /patient/{username}/outpatient/{mailid}/read
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/outpatient/{mailid}/read")]
        public void IsReadMail(string username, string mailid) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            int id  = (int)Convert.ToInt32(mailid);
            Mail mail = context.Mail.First(p => p.ID == id);
            mail.IsRead = 1;
            context.SaveChanges();
        }
        #endregion

        #region /patient/{username}/noticeuri
        [OperationContract]
        [WebInvoke(UriTemplate = "/patient/{username}/noticeuri", Method = "POST")]
        public void BuildPatientChannel(string username,NoticeUri uri) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Patient patient = context.Patient.First(p => p.LiveID == username);
            patient.Channel = uri.uri;
            context.SaveChanges();
        }
        #endregion

        #region /patient/{username}/emergency/{doctorid}/call
        [OperationContract]
        [WebInvoke(UriTemplate = "/patient/{username}/emergency/{doctorid}/call", Method = "POST")]
        public void EmergencyCall(string username,string doctorid,EmergencyMesg mesg) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            Patient patient = context.Patient.First(p => p.LiveID == username);
            patient.Latitude = mesg.latitude;
            patient.Longitude = mesg.longitude;
            context.SaveChanges();
            Doctor doctor = context.Doctor.First(p=> p.LiveID == doctorid);

            string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\">" +
                           "<wp:Toast>" +
                              "<wp:Text0>1</wp:Text0>" +
                              "<wp:Text1>" + patient.Name + "</wp:Text1>" +
                              "<wp:Text2>" + patient.Latitude + "</wp:Text2>" +
                              "<wp:Text3>" + patient.Longitude + "</wp:Text3>" +
                           "</wp:Toast>" +
                        "</wp:Notification>";

            byte[] strBytes = new UTF8Encoding().GetBytes(toastMessage);
            if (doctor.Channel != null)
            {
                sendNotificationType(strBytes, doctor.Channel);
            }
        }

        #endregion

        #region /patient/{username}/emergency/{doctorid}/call
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/emergency/{doctorid}/call")]
        public void EmergencyCall1(string username, string doctorid)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            Patient patient = context.Patient.First(p => p.LiveID == username);
            Doctor doctor = context.Doctor.First(p => p.LiveID == doctorid);

            string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\">" +
                           "<wp:Toast>" +
                              "<wp:Text1>"+patient.Name+"</wp:Text1>" +
                              "<wp:Text2>" +patient.Latitude+ "</wp:Text2>" +
                              "<wp:Text3>" + patient.Longitude + "</wp:Text3>" +
                           "</wp:Toast>" +
                        "</wp:Notification>";

            byte[] strBytes = new UTF8Encoding().GetBytes(toastMessage);
            if (doctor.Channel != null)
            {
                sendNotificationType(strBytes, doctor.Channel);
            }
        }

        #endregion

        #region /patient/{username}/emergency/doctorlist
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/emergency/doctorlist")]
        public List<MyDoctorDataContract> GetDoctorAroundUserList(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            List<MyDoctorDataContract> temp = new List<MyDoctorDataContract>();
            List<Doctor> tempuser = context.Doctor.ToList();
            List<string> keylist = context.Patient.Select(c => c.LiveID).Distinct().ToList();
            echelperDBEntities context_ = new echelperDBEntities();
            Patient data = context_.GetObjectByKey(new System.Data.EntityKey("echelperDBEntities.Patient", "LiveID", username)) as Patient;
            foreach (Doctor i in tempuser)
            {
                if (i.Status == 1)
                {
                    bool ismy = false;
                    if (i.Patient.Contains(data))
                    {
                        ismy = true;
                    }
                    temp.Add(ConvertMyDoctorToDataContract(i, ismy));
                }
            }
            return temp;
        }
        #endregion

        #region /patient/{username}/online
        [OperationContract]
        [WebInvoke(UriTemplate = "/patient/{username}/online", Method = "POST")]
        public void UpdatePatientOnline(string username, OnlineStatusDataContract user)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Patient patient = context.Patient.First(p => p.LiveID==username);

            patient.Latitude = user.Latitude;
            patient.Longitude = user.Longtitude;
            patient.Status = 1;
            context.SaveChanges();
        }
        #endregion

        #region /patient/{username}/offline
        [OperationContract]
        [WebInvoke(UriTemplate = "/patient/{username}/offline", Method = "POST")]
        public void UpdatePatientOffline(string username, OnlineStatusDataContract user)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Patient patient = context.Patient.First(p => p.LiveID == username);

            patient.Latitude = user.Latitude;
            patient.Longitude = user.Longtitude;
            patient.Status = 0;
            context.SaveChanges();
        }
        #endregion

        #region /patient/{username}/contact/friendsdoctorlist
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/contact/friendsdoctorlist")]
        public List<MyDoctorDataContract> GetFiendsDoctorUserList(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            List<MyDoctorDataContract> temp = new List<MyDoctorDataContract>();
            List<Doctor> tempuser = context.Doctor.ToList();
           // List<string> keylist = context.Patient.Select(c => c.LiveID).Distinct().ToList();
           // echelperDBEntities context_ = new echelperDBEntities();
           // Patient data = context_.GetObjectByKey(new System.Data.EntityKey("echelperDBEntities.Patient", "LiveID", username)) as Patient;
            Patient data = context.Patient.First(p => p.LiveID == username);
            foreach (Doctor i in tempuser)
            {
                bool ismy = false;
                if (i.Patient.Contains(data))
                {
                    ismy = true;
                    temp.Add(ConvertMyDoctorToDataContract(i, ismy));
                }
                
            }
            return temp;
        }
        #endregion
    
        #region /patient/{username}/uploadrequest GET
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/uploadrequest")]
        public string GetPatientTempKey(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                RoleEnvironment.GetConfigurationSettingValue("StorageAccountConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(username);
            container.CreateIfNotExist();
            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            container.SetPermissions(permissions);
            string sas = container.GetSharedAccessSignature(new SharedAccessPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(60),
                Permissions = SharedAccessPermissions.Read | SharedAccessPermissions.Write
            });
            return sas;
        }
        #endregion

        #region /patient/{username}/getfilelist GET
        [OperationContract]
        [WebGet(UriTemplate = "/patient/{username}/getfilelist")]
        public List<FileListDataContract> GetPatientFileList(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            List<FileListDataContract> list = new List<FileListDataContract>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            RoleEnvironment.GetConfigurationSettingValue("StorageAccountConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(username);
            container.CreateIfNotExist();
            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            foreach (IListBlobItem i in container.ListBlobs()) {
               
                FileListDataContract m = new FileListDataContract();
                m.Filename = i.Uri.ToString();
                list.Add(m);
            }
            return list;
        }
        #endregion

        #endregion


        #region Doctor API

        #region /doctor/{username}/select
        [OperationContract]
        [WebGet(UriTemplate = "/doctor/{username}/select")]
        public DoctorUserDataContract GetDoctorUser(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            //List<string> keylist = context.Doctor.Select(c => c.LiveID).Distinct().ToList();
           // echelperDBEntities context_ = new echelperDBEntities();
           // Doctor data = context_.GetObjectByKey(new System.Data.EntityKey("echelperDBEntities.Doctor", "LiveID", username)) as Doctor;
            Doctor data = context.Doctor.First(p => p.LiveID == username );
            return ConvertDoctorUserToDataContract(data);
        }
        #endregion

        #region /doctor/{username}/update
        [OperationContract]
        [WebInvoke(UriTemplate = "/doctor/{username}/update", Method = "POST")]
        public DoctorUserDataContract UpdateDoctorUser(string username, DoctorUserDataContract user)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            Doctor doctor = context.Doctor.First(p => p.LiveID == username);
            doctor.Name = user.NickName;
            doctor.Description = user.Description;
            doctor.Phone = user.phone;
            doctor.Email = user.email;
           // doctor.
            context.SaveChanges();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, doctor);
            return user;
        }
        #endregion

        #region /doctor/{username}/outpatient/{patientid}/select
        [OperationContract]
        [WebGet(UriTemplate = "/doctor/{username}/outpatient/{patientid}/select")]
        public PatientUserDataContract GetPatientInfo(string username, string patientid) {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            List<string> keylist = context.Patient.Select(c => c.LiveID).Distinct().ToList();
            echelperDBEntities context_ = new echelperDBEntities();
            Patient data = context_.GetObjectByKey(new System.Data.EntityKey("echelperDBEntities.Patient", "LiveID", patientid)) as Patient;
            return ConvertPatientUserToDataContract(data);
        }
        #endregion

        #region /doctor/{username}/outpatient/maillist
        [OperationContract]
        [WebGet(UriTemplate = "/doctor/{username}/outpatient/maillist")]
        public List<MailDataContract> GetDoctorMailList(string username)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            List<MailDataContract> temp = new List<MailDataContract>();
            List<Mail> mails = context.Mail.Where(a => a.DoctorID == username).ToList();
            foreach (Mail i in mails)
            {
                temp.Add(ConvertMailToDataContract(i));
            }
            return temp;
        }
        #endregion

        #region /doctor/{username}/outpatient/diagnosis
        [OperationContract]
        [WebInvoke(UriTemplate = "/doctor/{username}/outpatient/diagnosis", Method = "POST")]
        public void addDiagnosis(string username, NewMailDataContract temp)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            DateTime time = DateTime.Now;
            Mail newMail = new Mail();
            newMail.PatientID = temp.PatientId;
            newMail.DoctorID = temp.DoctorId;
            newMail.Title = temp.Title;
            newMail.TextContent = temp.TextContent;
            newMail.Time = time;
            newMail.ECG = temp.ECG;
            newMail.FromOrTo = temp.FromOrTo;
            newMail.IsRead = 0;
            context.AddToMail(newMail);
            context.SaveChanges();
           // return ConvertMailToDataContract(newMail);


            Patient patient = context.Patient.First(p => p.LiveID == temp.PatientId);

            int count = 0;
            foreach (Mail i in context.Mail)
            {
                if (i.PatientID == temp.PatientId && i.FromOrTo == 0 && i.IsRead == 0)
                {
                    count++;
                }
            }
            string toastMessage = null;

            toastMessage = "<EmergencyConfirm>" +
                        "<Type>0</Type>" +
                          "<doctorName>" + count + "</doctorName>" +
                           "<latitude>0</latitude>" +
                           "<longitude>0</longitude>" +
                           "<reply>0</reply>" +
                    "</EmergencyConfirm>";

            
            byte[] strBytes = new UTF8Encoding().GetBytes(toastMessage);
            if (patient.Channel != null)
            {
                sendWinNotificationType(strBytes, patient.Channel);
            }
        }
        #endregion

        #region /doctor/{username}/outpatient/{mailid}/read
        [OperationContract]
        [WebGet(UriTemplate = "/docotr/{username}/outpatient/{mailid}/read")]
        public void IsReadMailp(string username, string mailid)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Mail);
            int id = (int)Convert.ToInt32(mailid);
            Mail mail = context.Mail.First(p => p.ID == id);
            mail.IsRead = 1;
            context.SaveChanges();
        }
        #endregion

        #region /doctor/{username}/noticeuri
        [OperationContract]
        [WebInvoke(UriTemplate = "/doctor/{username}/noticeuri", Method = "POST")]
        public void BuildDoctorChannel(string username, NoticeUri uri)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            Doctor doctor = context.Doctor.First(p => p.LiveID == username);
            doctor.Channel = uri.uri;
            context.SaveChanges();
        }
        #endregion

        #region /doctor/{username}/emergency/{patientid}/confirm
        [OperationContract]
        [WebInvoke(UriTemplate = "/doctor/{username}/emergency/{patientid}/confirm", Method = "POST")]
        public void EmergencyConfirm(string username, string patientid, EmergencyConfirm mesge)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            Doctor doctor = context.Doctor.First(p => p.LiveID == username);
            doctor.Latitude = mesge.latitude;
            doctor.Longitude = mesge.longitude;
            context.SaveChanges();
            Patient patient = context.Patient.First(p => p.LiveID == patientid);
            string toastMessage="";
            if (mesge.confirm=="yes")
            {
                toastMessage = "<EmergencyConfirm>" +
                            "<Type>1</Type>"+
                              "<doctorName>" + doctor.Name + "</doctorName>" +
                               "<latitude>" + doctor.Longitude + "</latitude>" +
                               "<longitude>" + doctor.Latitude + "</longitude>" +
                               "<reply>true</reply>" +
                        "</EmergencyConfirm>";

            }
            else {
                toastMessage = "<EmergencyConfirm>" +
                                "<Type>1</Type>" +
                              "<doctorName>" + doctor.Name + "</doctorName>" +
                               "<latitude>" + doctor.Longitude + "</latitude>" +
                               "<longitude>" + doctor.Latitude + "</longitude>" +
                               "<reply>false</reply>" +
                        "</EmergencyConfirm>";
            }
            byte[] strBytes = new UTF8Encoding().GetBytes(toastMessage);
            if (patient.Channel != null)
            {
                sendWinNotificationType(strBytes, patient.Channel);
            }
        }

        #endregion

        #region /doctor/{username}/emergency/{patientid}/confirm
        [OperationContract]
        [WebGet(UriTemplate = "/doctor/{username}/emergency/{patientid}/confirm")]
        public void EmergencyConfirm1(string username, string patientid)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Doctor doctor = context.Doctor.First(p => p.LiveID == username);
            Patient patient = context.Patient.First(p => p.LiveID == patientid);
            string toastMessage = null;

                toastMessage = "<EmergencyConfirm>" +
                    "<Type>1</Type>"+
                              "<doctorName>" + doctor.Name + "</doctorName>" +
                               "<latitude>" + doctor.Longitude + "</latitude>" +
                               "<longitude>" + doctor.Latitude + "</longitude>" +
                               "<reply>false</reply>" +
                        "</EmergencyConfirm>";

           
            byte[] strBytes = new UTF8Encoding().GetBytes(toastMessage);
            if (patient.Channel != null)
            {
                sendWinNotificationType(strBytes, patient.Channel);
            }
        }

        #endregion

        #region /patient/{username}/online
        [OperationContract]
        [WebInvoke(UriTemplate = "/doctor/{username}/online", Method = "POST")]
        public void UpdateDoctorOnline(string username, OnlineStatusDataContract user)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            Doctor doctor = context.Doctor.First(p => p.LiveID == username);

            doctor.Latitude = user.Latitude;
            doctor.Longitude = user.Longtitude;
            doctor.Status = 1;
            context.SaveChanges();
        }
        #endregion

        #region /patient/{username}/offline
        [OperationContract]
        [WebInvoke(UriTemplate = "/doctor/{username}/offline", Method = "POST")]
        public void UpdateDoctorOffline(string username, OnlineStatusDataContract user)
        {
            echelperDBEntities context = new echelperDBEntities();
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Doctor);
            context.Refresh(System.Data.Objects.RefreshMode.StoreWins, context.Patient);
            Doctor doctor = context.Doctor.First(p => p.LiveID == username);

            doctor.Latitude = user.Latitude;
            doctor.Longitude = user.Longtitude;
            doctor.Status = 0;
            context.SaveChanges();
        }
        #endregion

        #region /doctor/{username}/downloadrequest GET
        [OperationContract]
        [WebGet(UriTemplate = "/doctor/{username}/downloadrequest")]
        public string GetDoctorTempKey(string username)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                RoleEnvironment.GetConfigurationSettingValue("StorageAccountConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(username);
            container.CreateIfNotExist();
            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            container.SetPermissions(permissions);
            string sas = container.GetSharedAccessSignature(new SharedAccessPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(60),
                Permissions = SharedAccessPermissions.Read
            });
            return sas;
        }
        #endregion

        #endregion


        #region /register/{type}/{username}/info
        [OperationContract]
        [WebInvoke(UriTemplate = "/register/{type}/{username}/info", Method = "POST")]
        public bool Register(string type, string username) {

            return false;
        }
        #endregion


        #region Help Functions

        private static PatientUserDataContract ConvertPatientUserToDataContract(Patient user) //-----------
        {
            return new PatientUserDataContract()
            {
                NickName = user.Name,
                UserName = user.LiveID,
                Age = (int)user.Age,
                Allery = user.Allergy,
                Description = user.Description,
                Gender = user.Gender              
            };
        }

        private static DoctorUserDataContract ConvertDoctorUserToDataContract(Doctor user)  //-----------
        {
            return new DoctorUserDataContract()
            {
                UserName = user.LiveID,
                NickName = user.Name,
                Description = user.Description,
                email = user.Email,
                Grade = user.Grade,
                phone = user.Phone,
                image = user.Image
            };
        }
        private static MyDoctorDataContract ConvertMyDoctorToDataContract(Doctor user,bool ismyt)  //-----------
        {
            return new MyDoctorDataContract()
            {
                UserName = user.LiveID,

                NickName = user.Name,
                Description = user.Description,
                email = user.LiveID,
                Grade = user.Grade,
                phone = user.Phone,
                image = user.Image,
                ismy = ismyt,
                latitude = (double)user.Latitude,
                Longtitude = (double)user.Longitude
            };
        }
        

        private static MailDataContract ConvertMailToDataContract(Mail blog)   //-----------
        {
            return new MailDataContract()
            {
                MailId = blog.ID,
                Title = blog.Title,
                Time = (DateTime)blog.Time,
                TextContent = blog.TextContent,
                DoctorId = blog.DoctorID,
                PatientId = blog.PatientID,
                FromOrTo = (int)blog.FromOrTo,
                ECG = blog.ECG,
                IsRead = (int)blog.IsRead
            };
        }

        private static List<PatientUserDataContract> ConvertUserListToDataContractList(List<Patient> userList)  
        {
            List<PatientUserDataContract> list = new List<PatientUserDataContract>();
            if (userList != null)
            {
                foreach (Patient user in userList)   
                {
                    list.Add(ConvertPatientUserToDataContract(user));
                }
            }
            return list;
        }

        static void sendNotificationType(byte[] payLoad, string uri)
        {
            HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(uri); 
            sendNotificationRequest.Method = WebRequestMethods.Http.Post; 
            sendNotificationRequest.Headers["X-MessageID"] = Guid.NewGuid().ToString();
            sendNotificationRequest.ContentType = "text/xml; charset=utf-8";
            sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
            sendNotificationRequest.Headers.Add("X-NotificationClass", "2");
            sendNotificationRequest.ContentLength = payLoad.Length;
            byte[] notificationMessage = payLoad;
            using (Stream requestStream = sendNotificationRequest.GetRequestStream())
            {
                requestStream.Write(notificationMessage, 0, notificationMessage.Length);
            }

            HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
            string notificationStatus = response.Headers["X-NotificationStatus"];
            string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
            string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];
            string m = String.Format("通知状态：{0}，管道状态：{1}，设备状态：{2}",
                notificationStatus, notificationChannelStatus, deviceConnectionStatus);

        }

        static void sendWinNotificationType(byte[] payLoad, string uri)
        {
            getAccessToken();
            HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(uri);
            sendNotificationRequest.Method = WebRequestMethods.Http.Post;
            sendNotificationRequest.ContentType = "text/xml; charset=utf-8";
            sendNotificationRequest.Headers.Add("X-WNS-Type", "wns/toast");
            sendNotificationRequest.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

          
            sendNotificationRequest.ContentLength = payLoad.Length;
            byte[] notificationMessage = payLoad;
            using (Stream requestStream = sendNotificationRequest.GetRequestStream())
            {
                requestStream.Write(notificationMessage, 0, notificationMessage.Length);
            }

            HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();

        }

        static OAuthToken GetOAuthTokenFromJson(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var ser = new DataContractJsonSerializer(typeof(OAuthToken));
                var oAuthToken = (OAuthToken)ser.ReadObject(ms);
                return oAuthToken;
            }
        }

        protected static void getAccessToken()
        {
            var urlEncodedSid = HttpUtility.UrlEncode(String.Format("{0}", "ms-app://s-1-15-2-3518531015-2748466536-3394391572-3179176183-4182428314-2274179867-1022086756"));
            var urlEncodedSecret = HttpUtility.UrlEncode("F4anGICqHSIld5WTyPqWVn1klyKanFAM");

            var body =
              String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", urlEncodedSid, urlEncodedSecret);

            var client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            string response = client.UploadString("https://login.live.com/accesstoken.srf", body);
            var oAuthToken = GetOAuthTokenFromJson(response);
            accessToken = oAuthToken.AccessToken;
        }

        #endregion
    }
}
