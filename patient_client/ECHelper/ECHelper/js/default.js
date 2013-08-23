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

// For an introduction to the Navigation template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232506
/// <reference path="/js/ListItems.js"/>
(function () {
    "use strict";

    Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(true);
    var tileContent0 = NotificationsExtensions.TileContent.TileContentFactory.createTileWideImage();
    tileContent0.image.src = "ms-appx:///images/tile.png";
    var squareContent0 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareImage();
    squareContent0.image.src = "ms-appx:///images/smallTile.png";
    tileContent0.squareContent = squareContent0;
    tileContent0.branding = 2;
    var tileNotification0 = tileContent0.createNotification();
    tileNotification0.tag = 0;
    Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification0);

    var connectionData = new ConnectionLibrary.ECGData();
    var newData = new Array(206);
    var isRemind = false;
    var datas = new Array(840);
    var start = new Array(2);
    var time;

    function ECGInitialize() {
        for (var i = 0; i < 1000; i++) {
            datas[i] = 270;
        }
        time = (new Date()).getTime()-27000;
        connectionData.beginConnect();
        start[0] = 0;
        start[1] = 0;
        setInterval(updateDatas, 210);

    }

    function updateDatas() {
        newData = connectionData.getECGData();

        if (newData[0] == 255) {

            if (!isRemind && (new Date()).getTime() - time > 30000) {
                time = (new Date()).getTime();
                var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.createToastImageAndText02();
                toastContent.textHeading.text = "Warning";
                toastContent.textBodyWrap.text = "Please plugin your device.";
                toastContent.image.src = "ms-appx:///images/warning.png";
                Windows.UI.Notifications.ToastNotificationManager.createToastNotifier().show(toastContent.createNotification());
                var x = 1;
            }
        }
        else {
            start[1] = newData[0];
            for (var i = 1; i <= newData[0]; i = i + 2) {
                datas[start[0]] = 530 - (newData[i] * 256 + newData[i + 1]) * 0.5;
                if (datas[start[0]] < 18) {
                    start[1] = start[1] - 2;
                    continue;
                }
                start[0]++;
                if (start[0] == 840) start[0] = 0;
            }
            
        }
    }

    var app = WinJS.Application;

    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.previousExecutionState !== Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize 
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension. 
                // Restore application state here.
            }
            WinJS.UI.processAll().done(ECGInitialize);
        }
    };

    app.oncheckpoint = function (eventObject) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the 
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // eventObject.setPromise(). 
    };

    app.start();
    WinJS.Namespace.define("ECGData", {
        start: start,
        datas: datas,
        connectionData: connectionData,
        maillist: new generic.list(),
        profile: new generic.list(),
        alldoctorlist: new generic.list(),
        recordlist: new generic.list(),
        emergencydoctorlist: new generic.list(),
        outpatientdoctorlist: new generic.list(),
        ecgfilelist: new generic.list(),
    });
    WinJS.Namespace.define("MailData", {
        itemList: new WinJS.Binding.List(ECGData.maillist.items),
    });
    WinJS.Namespace.define("DoctorData", {
        itemList: new WinJS.Binding.List(ECGData.emergencydoctorlist.items)
    });
    WinJS.Namespace.define("RecordData", {
        itemList: new WinJS.Binding.List(ECGData.recordlist.items)
    });
    WinJS.Namespace.define("DoctorData", {
        itemList: new WinJS.Binding.List(ECGData.outpatientdoctorlist.items)
    });
    WinJS.Namespace.define("FileListData", {
        itemList: new WinJS.Binding.List(ECGData.ecgfilelist.items)
    });
})();

var live_myName;
var live_myImage;
var isLogin = false;
var patientName = "liaomin";
var doctorName = "xiaoming";
var myHost = "http://echelper.cloudapp.net/Service.svc/patient/" + patientName;
var channel;

function appBarInit() {
    var nav = WinJS.Navigation;

    var homebutton = document.getElementById("homebutton");
    homebutton.onclick = function () {
        nav.navigate('html/homePage.html');
    }

    var outpatientbutton=document.getElementById("outpatientbutton");
    outpatientbutton.onclick = function () {
        var wait = document.getElementById("wait");
        wait.style.display = "block";
        var rand = new ConnectionLibrary.MyDatetime();
        WinJS.xhr({ url: myHost + "/outpatient/maillist?"+rand.random() }).then(function (r) {
            var result = r.responseText;
            ECGData.maillist = new generic.list();
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            doc.loadXml(result);
            var arrayOfMails = doc.selectNodes("ArrayOfMailDataContract");
            var mails = arrayOfMails.item(0).selectNodes("MailDataContract");
            for (var i = mails.length - 1; i >= 0 ; i--) {
                var id = mails.item(i).selectSingleNode("MailId").innerText;
                var title = mails.item(i).selectSingleNode("Title").innerText;
                var fromorto = mails.item(i).selectSingleNode("FromOrTo").innerText;
                var author;
                if (fromorto) {
                    author = mails.item(i).selectSingleNode("DoctorId").innerText;
                }
                else {
                    author = mails.item(i).selectSingleNode("PatientId").innerText;
                }
                var message = mails.item(i).selectSingleNode("TextContent").innerText;
                var ecg = mails.item(i).selectSingleNode("ECG").innerText;
                var time = mails.item(i).selectSingleNode("Time").innerText;
                var isread = mails.item(i).selectSingleNode("IsRead").innerText;
                ECGData.maillist.add({ id: id, title: title, fromorto: fromorto, author: author, message: message, ecg: ecg, time: time, isread: isread });
            }
            MailData.itemList = new WinJS.Binding.List(ECGData.maillist.items);
            wait.style.display = "none";
            nav.navigate('html/outpatient.html');
        }, function () {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }
        
    var emergencybutton = document.getElementById("emergencybutton");
    emergencybutton.onclick = function () {
        var wait = document.getElementById("wait");
        wait.style.display = "block";
        var rand = new ConnectionLibrary.MyDatetime();
        WinJS.xhr({ url: myHost + "/emergency/doctorlist?"+rand.random() }).then(function (r) {
            var result = r.responseText;
            ECGData.emergencydoctorlist = new generic.list();
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            doc.loadXml(result);
            var arrayOfDoctors = doc.selectNodes("ArrayOfMyDoctorDataContract");
            var doctors = arrayOfDoctors.item(0).selectNodes("MyDoctorDataContract");
            for (var i = 0; i < doctors.length; i++) {
                var id = doctors.item(i).selectSingleNode("UserName").innerText;
                var name = doctors.item(i).selectSingleNode("NickName").innerText;
                var grade = doctors.item(i).selectSingleNode("Grade").innerText;
                var description = doctors.item(i).selectSingleNode("Description").innerText;
                var email = doctors.item(i).selectSingleNode("email").innerText;
                var phone = doctors.item(i).selectSingleNode("phone").innerText;
                var image = doctors.item(i).selectSingleNode("image").innerText;
                var latitude = doctors.item(i).selectSingleNode("latitude").innerText;
                var longitude = doctors.item(i).selectSingleNode("Longtitude").innerText;
                ECGData.emergencydoctorlist.add({ id: id, name: name, grade: grade, description: description, email: email, phone: phone, image: image, latitude: latitude, longitude: longitude });
            }
            DoctorData.itemList = new WinJS.Binding.List(ECGData.emergencydoctorlist.items);
            wait.style.display = "none";
            nav.navigate('/html/emergency.html');
        }, function () {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }

    var contactbutton = document.getElementById("contactbutton");
    contactbutton.onclick = function () {
        var wait = document.getElementById("wait");
        wait.style.display = "block";
        WinJS.xhr({ url: myHost + "/contact/alldoctorlist" }).then(function (r) {
            var result = r.responseText;
            ECGData.alldoctorlist = new generic.list();
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            doc.loadXml(result);
            var arrayOfDoctors = doc.selectNodes("ArrayOfMyDoctorDataContract");
            var doctors = arrayOfDoctors.item(0).selectNodes("MyDoctorDataContract");
            for (var i = 0; i < doctors.length; i++) {
                var id = doctors.item(i).selectSingleNode("UserName").innerText;
                var name = doctors.item(i).selectSingleNode("NickName").innerText;
                var grade = doctors.item(i).selectSingleNode("Grade").innerText;
                var description = doctors.item(i).selectSingleNode("Description").innerText;
                var email = doctors.item(i).selectSingleNode("email").innerText;
                var phone = doctors.item(i).selectSingleNode("phone").innerText;
                var image = doctors.item(i).selectSingleNode("image").innerText;
                if (doctors.item(i).selectSingleNode("ismy").innerText == "false") {
                    var ismine = false;
                } else {
                    var ismine = true;
                }
                ECGData.alldoctorlist.add({ id: id, name: name, grade: grade, description: description, email: email, phone: phone, image: image, ismine: ismine });
            }
            wait.style.display = "none";
            nav.navigate('/html/doctorList.html');
        }, function () {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }

    var recordbutton = document.getElementById("recordbutton");
    recordbutton.onclick = function () {
        var wait = document.getElementById("wait");
        wait.style.display = "block";
        var rand = new ConnectionLibrary.MyDatetime();
        WinJS.xhr({ url: myHost + "/recordlist?"+rand.random() }).then(function (r) {
            var result = r.responseText;
            ECGData.recordlist = new generic.list();
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            doc.loadXml(result);
            var arrayOfMails = doc.selectNodes("ArrayOfMailDataContract");
            var mails = arrayOfMails.item(0).selectNodes("MailDataContract");
            for (var i = 0; i < mails.length; i++) {
                var id = mails.item(i).selectSingleNode("MailId").innerText;
                var title = mails.item(i).selectSingleNode("Title").innerText;
                var author = mails.item(i).selectSingleNode("PatientId").innerText;
                var message = mails.item(i).selectSingleNode("TextContent").innerText;
                var ecg = mails.item(i).selectSingleNode("ECG").innerText;
                var time = mails.item(i).selectSingleNode("Time").innerText;
                ECGData.recordlist.add({ id: id, title: title, author: author, message: message, ecg: ecg, time: time });
            }
            RecordData.itemList = new WinJS.Binding.List(ECGData.recordlist.items);
            wait.style.display = "none";
            nav.navigate('/html/record.html');
        }, function () {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }

    var profilebutton = document.getElementById("profilebutton");
    profilebutton.onclick = function () {
        var wait = document.getElementById("wait");
        wait.style.display = "block";
        var rand = new ConnectionLibrary.MyDatetime();
        WinJS.xhr({ url: myHost + "/profile/select?"+rand.random() }).then(function (r) {
            var result = r.responseText;
            ECGData.profile = new generic.list();
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            doc.loadXml(result);
            var profile = doc.selectNodes("PatientUserDataContract");
            for (var i = 0; i < profile.length; i++) {
                var id = profile.item(i).selectSingleNode("UserName").innerText;
                var name = profile.item(i).selectSingleNode("NickName").innerText;
                var gender = profile.item(i).selectSingleNode("Gender").innerText;
                var age = profile.item(i).selectSingleNode("Age").innerText;
                var description = profile.item(i).selectSingleNode("Description").innerText;
                var allery = profile.item(i).selectSingleNode("Allery").innerText;
                ECGData.profile.add({ id: id, name: name, gender: gender, age: age, description: description, allery: allery });
            }
            wait.style.display = "none";
            nav.navigate('/html/profile.html');
        }, function () {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }
}

function liveInit() {
    var imgHolder = document.getElementById("image"),
            nameHolder = document.getElementById("myName");
    WL.Event.subscribe("auth.login", onLoginComplete);
    WL.Event.subscribe("auth.sessionChange", onSessionChanged);

    if(isLogin) {
        imgHolder.innerHTML = live_myImage;
        nameHolder.innerHTML = live_myName;
    }

    WL.init();

    WL.ui({
        name: "signin",
        element: "signinbutton",
        scope: "wl.signin"
    });

    function onLoginComplete() {
        var session = WL.getSession();
    }

    function onSessionChanged(e) {
        if (e.session) {
            displayMe();
        }
        else {
            clearMe();
        }
    }

    function displayMe() {

        if (imgHolder.innerHTML != "") return;

        if (WL.getSession() != null) {
            WL.api({ path: "me/picture", method: "get" }).then(
                function (response) {
                    if (response.location) {
                        live_myImage = "<img src='" + response.location + "'/>";
                        imgHolder.innerHTML = live_myImage;
                        isLogin = true;
                    }
                },
                function (failedResponse) {
                    logObject("get-me/picture failure", failedResponse);
                }
                );
            WL.api({ path: "me", method: "get" }).then(
                    function (response) {
                        live_myName = response.name;
                        nameHolder.innerHTML = live_myName;
                        isLogin = true;
                    },
                    function (failedResponse) {
                        logObject("get-me failure", failedResponse);
                    }
                );
        }
    }

    function clearMe() {
        imgHolder.innerHTML = "";
        nameHolder.innerHTML = "";
        isLogin = false;
    }
}




var myMessageData = new WinJS.Binding.List([
        { title: "New Flavors out this week!", text: "Adam Barr", picture: "/images/mail1.png" },
        { title: "Check out this Topping!", text: "David Alexander", picture: "/images/mail2.png" },
        { title: "Ice Cream Party!!!", text: "Josh Bailey", picture: "/images/mail1.png" },
        { title: "I Love Ice Cream", text: "Chris Berry", picture: "/images/mail1.png" },
        { title: "What's Your Favorite?", text: "Sean Bentley", picture: "/images/mail2.png" },
        { title: "Where is the Coupon?", text: "Adrian Lannin", picture: "/images/mail1.png" },
        { title: "Your Invited!", text: "Gary Schare", picture: "/images/mail1.png" },
        { title: "Make a Custom Carton!", text: "Garth Fort", picture: "/images/mail2.png" },
        { title: "Check this out", text: "Raymond Fong", picture: "/images/mail1.png" },
        { title: "When Are You Available", text: "Derek Brown", picture: "/images/mail1.png" },
        { title: "Peanut Butter!", text: "Maria Cameron", picture: "/images/mail2.png" },
        { title: "Caramel Topping Coming", text: "Judy Lew", picture: "/images/mail1.png" },
        { title: "Candy Cane Flavor?", text: "Chris Mayo", picture: "/images/mail1.png" },
        { title: "Spinkles Galor!", text: "Randy", picture: "/images/mail2.png" },
        { title: "Tell Me More", text: "Mike Nash", picture: "/images/mail1.png" }
]);

var recordData = [
        { id: 0, title: "New Flavors out this week!", text: "Adam Barr", time: "Mar. 3rd" },
        { id: 1, title: "Check out this Topping!", text: "David Alexander", time: "Mar. 3rd" },
        { id: 2, title: "Ice Cream Party!!!", text: "Josh Bailey", time: "Mar. 3rd" },
        { id: 3, title: "I Love Ice Cream", text: "Chris Berry", time: "Mar. 3rd" },
        { id: 4, title: "What's Your Favorite?", text: "Sean Bentley", time: "Mar. 3rd" },
        { id: 5, title: "Where is the Coupon?", text: "Adrian Lannin", time: "Mar. 3rd" },
        { id: 6, title: "Your Invited!", text: "Gary Schare", time: "Mar. 3rd" },
        { id: 7, title: "Make a Custom Carton!", text: "Garth Fort", time: "Mar. 3rd" },
        { id: 8, title: "Check this out", text: "Raymond Fong", time: "Mar. 3rd" },
        { id: 9, title: "When Are You Available", text: "Derek Brown", time: "Mar. 3rd" },
        { id: 10, title: "Peanut Butter!", text: "Maria Cameron", time: "Mar. 3rd" },
        { id: 11, title: "Caramel Topping Coming", text: "Judy Lew", time: "Mar. 3rd" },
        { id: 12, title: "Candy Cane Flavor?", text: "Chris Mayo", time: "Mar. 3rd" },
        { id: 13, title: "Spinkles Galor!", text: "Randy", time: "Mar. 3rd" },
        { id: 14, title: "Tell Me More", text: "Mike Nash", time: "Mar. 3rd" }
];
var myRecordData = new WinJS.Binding.List(recordData);

var myDoctorLocal = [
    { name: "Quan Shuo", image: "images/smalllogo.png", email: "a184762097@live.cn", latitude: 40.1, longitude: 116.4 },
    { name: "Chen Youzheng", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.11, longitude: 116.41 },
    { name: "Li Hongjie", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.12, longitude: 116.41 },
    { name: "Ren Yankai", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.13, longitude: 116.41 },
    { name: "Li Jing", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.1, longitude: 116.44 },
    { name: "Quan Shuo", image: "images/smalllogo.png", email: "a184762097@live.cn", latitude: 40.14, longitude: 116.41 },
    { name: "Chen Youzheng", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.12, longitude: 116.42 },
    { name: "Li Hongjie", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.1, longitude: 116.44 },
    { name: "Ren Yankai", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.13, longitude: 116.42 },
    { name: "Li Jing", image: "images/smalllogo.png", email: "184762097@qq.com", latitude: 40.1, longitude: 116.41 }
];
var myDoctorData = new WinJS.Binding.List(myDoctorLocal);

var itemDescription = "To upgrade your Mac to OS X Lion, you don’t need to drive to a store, bring home a box, and install a bunch of discs. All you do is click the Mac App Store icon, buy Lion for $29.99, and your Mac does the rest. Just make sure you have what you need to download Lion to your Mac.To upgrade your Mac to OS X Lion, you don’t need to drive to a store, bring home a box, and install a bunch of discs. All you do is click the Mac App Store icon, buy Lion for $29.99, and your Mac does the rest. Just make sure you have what you need to download Lion to your Mac.";
var doctorlist = [
    { name: "Li Hongjie", grade: "Chief Physician", description: itemDescription, image: "images/smalllogo.png", email: "cloudzfy@qq.com", phone: "15210807538", ismine: true },
    { name: "Li Hongjie", grade: "Chief Physician", description: itemDescription, image: "images/smalllogo.png", email: "cloudzfy@qq.com", phone: "15210807538", ismine: false },
    { name: "Chen Youzheng", grade: "Chief Physician", description: itemDescription, image: "images/smalllogo.png", email: "cloudzfy@qq.com", phone: "15210807538", ismine: false },
    { name: "Li Hongjie", grade: "Chief Physician", description: itemDescription, image: "images/smalllogo.png", email: "cloudzfy@qq.com", phone: "15210807538", ismine: false },
    { name: "Quan Shuo", grade: "Chief Physician", description: itemDescription, image: "images/smalllogo.png", email: "cloudzfy@qq.com", phone: "15210807538", ismine: true },
];