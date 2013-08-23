(function () {
    "use strict";

    var fileName;
    var file;
    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    function ready(element, options) {
        appBarInit();
        liveInit();
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "200px", left: "0px" });
        enterPage.done();
        CanvasInitialize();
        var startdiv = document.getElementById("startdiv");
        startdiv.addEventListener("click", function () {
            fileName = ECGData.connectionData.beginStore();
            var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.createToastImageAndText02();
            toastContent.textHeading.text = "Info";
            toastContent.textBodyWrap.text = "ECG recorder starting...";
            toastContent.image.src = "ms-appx:///images/warning.png";
            Windows.UI.Notifications.ToastNotificationManager.createToastNotifier().show(toastContent.createNotification());
        }, false);

        var uploaddiv = document.getElementById("uploaddiv");
        uploaddiv.addEventListener("click", function () {
            ECGData.connectionData.endStore().then(
                function (downfile) {
                    file = downfile;
                    var wait = document.getElementById("wait");
                    wait.style.display = "block";
                    WinJS.xhr({ url: myHost + "/uploadrequest" }).then(function (r) {
                        var result = r.responseText;
                        var doc = new Windows.Data.Xml.Dom.XmlDocument;
                        doc.loadXml(result);
                        uploadFile(doc.innerText);
                    }, function () {
                        wait.style.display = "none";
                        var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                        msg.showAsync();
                    });
                });
        }, false);
           

        var deletediv = document.getElementById("deletediv");
        deletediv.addEventListener("click", function () {
            ECGData.connectionData.endStore();
        }, false);

        noticeUri();

        var deletediv = document.getElementById("deletediv");
        startdiv.addEventListener("MSPointerDown", onPointerDown, false);
        startdiv.addEventListener("MSPointerUp", onPointerUp, false);
        uploaddiv.addEventListener("MSPointerDown", onPointerDown, false);
        uploaddiv.addEventListener("MSPointerUp", onPointerUp, false);
        deletediv.addEventListener("MSPointerDown", onPointerDown, false);
        deletediv.addEventListener("MSPointerUp", onPointerUp, false);

    }

    function uploadFile(requeststring) {
        var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.createToastImageAndText02();
        toastContent.textHeading.text = "Info";
        toastContent.textBodyWrap.text = "Start uploading ECG file...";
        toastContent.image.src = "ms-appx:///images/warning.png";
        Windows.UI.Notifications.ToastNotificationManager.createToastNotifier().show(toastContent.createNotification());
        var destinationUri = "http://echelperspace.blob.core.windows.net/" + patientName + "/" + fileName + requeststring;
        var uploader = new Windows.Networking.BackgroundTransfer.BackgroundUploader();
        uploader.method = "PUT";
        var uri = Windows.Foundation.Uri(destinationUri);
        uploader.setSourceFile(file);
        //uploader.setRequestHeader("Filename", file.fileName);
        var upload = uploader.createUpload(uri);
        upload.startAsync().then(function () {
            wait.style.display = "none";
            var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.createToastImageAndText02();
            toastContent.textHeading.text = "Info";
            toastContent.textBodyWrap.text = "ECG upload completed.";
            toastContent.image.src = "ms-appx:///images/warning.png";
            Windows.UI.Notifications.ToastNotificationManager.createToastNotifier().show(toastContent.createNotification());
        }, function (error) {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }
    function pushNotificationReceivedHandler(e) {

        var notificationPayload;
        var doc = new Windows.Data.Xml.Dom.XmlDocument();
        doc.loadXml(e.toastNotification.content.getXml());
        var emergencyConfirm = doc.selectNodes("EmergencyConfirm");
        for (var i = 0; i < emergencyConfirm.length; i++) {
            var type = emergencyConfirm.item(i).selectSingleNode("Type").innerText;
            if (type == 1) {
                var myDoctorName = emergencyConfirm.item(i).selectSingleNode("doctorName").innerText;
                var latitude = emergencyConfirm.item(i).selectSingleNode("latitude").innerText;
                var longitude = emergencyConfirm.item(i).selectSingleNode("longitude").innerText;
                var reply = emergencyConfirm.item(i).selectSingleNode("reply").innerText;
                var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.createToastImageAndText02();
                toastContent.textHeading.text = "Good News";
                toastContent.textBodyWrap.text = "Dr. " + myDoctorName + " is on the way.";
                toastContent.image.src = "ms-appx:///images/warning.png";
                Windows.UI.Notifications.ToastNotificationManager.createToastNotifier().show(toastContent.createNotification());
            } else {
                var num = emergencyConfirm.item(i).selectSingleNode("doctorName").innerText;
                var badgeContent = new NotificationsExtensions.BadgeContent.BadgeNumericNotificationContent(num);
                Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badgeContent.createNotification());
                var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.createToastImageAndText02();
                toastContent.textHeading.text = "A New Message";
                toastContent.textBodyWrap.text = "A new message has arrived.";
                toastContent.image.src = "ms-appx:///images/warning.png";
                Windows.UI.Notifications.ToastNotificationManager.createToastNotifier().show(toastContent.createNotification());
            }
        }
    }

    WinJS.UI.Pages.define("/html/homePage.html", {
        ready: ready
    });

    function noticeUri() {
        var wait = document.getElementById("wait");
        wait.style.display = "block";
        Windows.Networking.PushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync().then(function (newChannel) {
            channel = newChannel;
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            var root = doc.createElement("NoticeUri");
            var uri = doc.createElement("uri");
            uri.innerText = newChannel.uri;
            root.appendChild(uri);
            doc.appendChild(root);
            var docxml = doc.getXml();
            WinJS.xhr({
                type: "POST",
                url: myHost + "/noticeuri",
                headers: {
                    "Content-type": "application/xml",
                    "Content-length": docxml.length
                },
                data: docxml
            }).then(function () {
                wait.style.display = "none";
                channel.addEventListener("pushnotificationreceived", pushNotificationReceivedHandler);
            }, function () {
                wait.style.display = "none";
                var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                msg.showAsync();
            });

        }, function (error) {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });
    }

    function onPointerDown(evt) {
        WinJS.UI.Animation.pointerDown(evt.srcElement);
    }

    function onPointerUp(evt) {
        WinJS.UI.Animation.pointerUp(evt.srcElement);
    }
    var myECG;
    var context;
    var canWidth = 1050;
    var canHight = 520;
    var numRowLine = 52;
    var numColumnLine = 105;
    var widthInterval = canWidth / 840;
    var hightInterval = 0.5;
    var basic;
    var intervalid;
    var intervalid1;
    var interval = canWidth / numColumnLine;
    var start;
    var excursion = 0;
    var isFirst = 1;
    var sign10;
    
    function CanvasInitialize() { 
        isFirst = 1;
        sign10 = 1;
        intervalid = setInterval(updateCanvas, 840);
    }

    function updataHeartRate() {
        var heartrate = document.getElementById("heartrate");
        if (heartrate != null) {
            var rand = new ConnectionLibrary.MyDatetime();
            heartrate.innerText = rand.random() % 6 + 83;
            var tileContent1 = NotificationsExtensions.TileContent.TileContentFactory.createTileWideBlockAndText01();
            tileContent1.textBlock.text = heartrate.innerText;
            tileContent1.textSubBlock.text = "bpm";
            var squareContent1 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareImage();
            squareContent1.image.src = "ms-appx:///images/smallTile.png";
            tileContent1.squareContent = squareContent1;
            tileContent1.branding = 2;
            var tileNotification1 = tileContent1.createNotification();
            tileNotification1.tag = 1;
            Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification1);
        }
        else {
            clearInterval(intervalid1);
        }
    }

    function updateCanvas() {
        if (ECGData.start[1] > 0 && sign10 == 1){
            intervalid1 = setInterval(updataHeartRate, 5000);
            sign10 = 0;
        }
        if (ECGData.start[1] > 0 || isFirst == 1) {
            start = ECGData.start[0];
            excursion = excursion + ECGData.start[1] * 0.625;
            excursion = excursion % 10;
            myECG = document.getElementById("myECG");
            if (myECG == null) (clearInterval(intervalid))
            else {
                context = myECG.getContext('2d');

                basic = 10 + interval;

                context.clearRect(0, 0, canWidth + 50, canHight + 50);

                context.save();

                context.lineWidth = 1;
                context.strokeStyle = "#ffd590";

                for (var i = 0; i < numRowLine ; i++, basic = basic + interval) {

                    context.beginPath();
                    context.moveTo(5, basic);
                    context.lineTo(5 + canWidth, basic);
                    context.stroke();
                }
                basic = 5 + interval - excursion;
                for (i = 0; basic < 1060; i++, basic = basic + interval) {

                    context.beginPath();
                    context.moveTo(basic, 10);
                    context.lineTo(basic, 10 + canHight);
                    context.stroke();
                }
                context.restore();



                context.save();

                context.lineCap = "round";
                context.lineWidth = 2;

                context.shadowBlur = 10;
                context.shadowColor = "#ffd17a";
                context.shadowOffsetX = 0;
                context.shadowOffsetY = 0;

                context.strokeStyle = "#035823";
                basic = 5;
                for (i = start + 1; i < 839 + start; i++) {
                    context.beginPath();
                    context.moveTo(basic, ECGData.datas[(i + 839) % 840]);
                    basic = basic + widthInterval;
                    context.lineTo(basic, ECGData.datas[i % 840]);
                    context.stroke();
                }
                context.restore();


                context.save();
                context.lineCap = "round";
                context.lineWidth = 6;

                context.strokeStyle = "#009D1A";
                context.beginPath();
                context.moveTo(5, 10);
                context.lineTo(5, canHight + 10);
                context.stroke();

                context.beginPath();
                context.moveTo(10, 10);
                context.lineTo(canWidth + 5, 10);
                context.stroke();

                context.beginPath();
                context.moveTo(canWidth + 5, canHight + 10);
                context.lineTo(canWidth + 5, 10);
                context.stroke();

                context.beginPath();
                context.moveTo(canWidth + 5, canHight + 10);
                context.lineTo(5, canHight + 10);
                context.stroke();


                context.restore();

                if (start == 840) start = 0;
                isFirst = 0;

            }

        }
    }
})();
