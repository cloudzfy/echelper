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

(function () {
    "use strict";

    var list;
    var datas = new Array(1680);
    var dataToDraw = new Array(840);
    var context;
    var interval = 8;
    var numRowLine = 52;
    var numColumnLine = 105;
    var canWidth = interval * numColumnLine;
    var canHight = interval * numRowLine;
    var widthInterval = canWidth / 840;
    var basic = 0;
    var connectionReader = new ConnectionLibrary.ECGReader();
    var dataLength = 0;
    var isplay = 0;
    var fileName;

    function ready(element, options) {
        appBarInit();
        liveInit();
        //templateInit();
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "100px", left: "100px" });
        enterPage.done();  

        list = document.getElementById("maillist").winControl;
        list.itemTemplate = itemTemplateFunction;
        list.addEventListener("selectionchanged", selectionchanged);

        var newbutton = document.getElementById("newbutton");
        newbutton.onclick = function () {
            var wait = document.getElementById("wait");
            wait.style.display = "block";
            ECGData.outpatientdoctorlist = new generic.list();
            var rand = new ConnectionLibrary.MyDatetime();
            WinJS.xhr({ url: myHost + "/contact/friendsdoctorlist?"+rand.random() }).then(function (r) {
                var result = r.responseText;
                ECGData.outpatientdoctorlist = new generic.list();
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
                    ECGData.outpatientdoctorlist.add({ id: id, name: name, grade: grade, description: description, email: email, phone: phone, image: image });
                }
                DoctorData.itemList = new WinJS.Binding.List(ECGData.outpatientdoctorlist.items);
                var rand = new ConnectionLibrary.MyDatetime();
                WinJS.xhr({ url: myHost + "/getfilelist?"+rand.random() }).then(function (r) {
                    wait.style.display = "none";
                    var result = r.responseText;
                    ECGData.ecgfilelist = new generic.list();
                    var doc = new Windows.Data.Xml.Dom.XmlDocument;
                    doc.loadXml(result);
                    var arrayOfFile = doc.selectNodes("ArrayOfFileListDataContract");
                    var filelist = arrayOfFile.item(i).selectNodes("FileListDataContract");
                    for (var i = 0; i < filelist.length; i++) {
                        var filename = filelist.item(i).selectSingleNode("Filename").innerText;
                        ECGData.ecgfilelist.add({ filename: filename });
                    }
                    FileListData.itemList = new WinJS.Binding.List(ECGData.ecgfilelist.items);
                    wait.style.display = "none";
                    WinJS.Navigation.navigate("/html/newMessage.html");
                }, function () {
                    wait.style.display = "none";
                    var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                    msg.showAsync();
                });
            }, function () {
                var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                msg.showAsync();
            });
        };

        context = document.getElementById("myECGPlay").getContext('2d');

        var ECGSlid = document.getElementById("ECGSlid");
        ECGSlid.onmouseup = function () {
            var ratio = ECGSlid.value;
            datas = connectionReader.getData(ratio / 100 * (dataLength - 1680), 1680);
            for (var i = 0, j = 0; i < 1680; i = i + 2, j++) {
                dataToDraw[j] = 426 - (datas[i] * 256 + datas[i + 1]) * 0.4;
            }
            drawECG();
        }

        var playbutton = document.getElementById("playbutton");
        playbutton.onclick = function () {
            if (isplay == 0) {
                myECGCanvas.style.display = "block";
                myECGCanvas.style.opacity = 1;
                WinJS.UI.Animation.showPopup(myECGCanvas, null);
                isplay = 1;
            } 
            else {
                myECGCanvas.style.opacity = 0;
                WinJS.UI.Animation.hidePopup(myECGCanvas);
                isplay = 0;
                myECGCanvas.style.display = "none";
            }
            connectionReader.open(fileName).then(function (count) {
                dataLength = count;
                datas = connectionReader.getData(0, 1680);
                for (var i = 0, j = 0; i < 1680; i = i + 2, j++) {
                    dataToDraw[j] = 426 - (datas[i] * 256 + datas[i + 1]) * 0.4;
                }
                drawECG();
            });
        };


        var mailreplybutton = document.getElementById("mailreplybutton");
        mailreplybutton.onclick = function () {
            if (list.selection.count() == 0) {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Please select one message.");
                messageDialog.commands.append(new Windows.UI.Popups.UICommand("OK", function () { }));
                messageDialog.showAsync();
            } else {
                list.selection.getItems().then(function (items) {
                    var wait = document.getElementById("wait");
                    wait.style.display = "block";
                    ECGData.outpatientdoctorlist = new generic.list();
                    var rand = new ConnectionLibrary.MyDatetime();
                    WinJS.xhr({ url: myHost + "/contact/friendsdoctorlist?" + rand.random() }).then(function (r) {
                        var result = r.responseText;
                        ECGData.outpatientdoctorlist = new generic.list();
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
                            ECGData.outpatientdoctorlist.add({ id: id, name: name, grade: grade, description: description, email: email, phone: phone, image: image });
                        }
                        DoctorData.itemList = new WinJS.Binding.List(ECGData.outpatientdoctorlist.items);
                        var rand = new ConnectionLibrary.MyDatetime();
                        WinJS.xhr({ url: myHost + "/getfilelist?" + rand.random() }).then(function (r) {
                            wait.style.display = "none";
                            var result = r.responseText;
                            ECGData.ecgfilelist = new generic.list();
                            var doc = new Windows.Data.Xml.Dom.XmlDocument;
                            doc.loadXml(result);
                            var arrayOfFile = doc.selectNodes("ArrayOfFileListDataContract");
                            var filelist = arrayOfFile.item(i).selectNodes("FileListDataContract");
                            for (var i = 0; i < filelist.length; i++) {
                                var filename = filelist.item(0).selectSingleNode("Filename").innerText;
                                ECGData.ecgfilelist.add({ filename: filename });
                            }
                            FileListData.itemList = new WinJS.Binding.List(ECGData.ecgfilelist.items);
                            wait.style.display = "none";
                            WinJS.Navigation.navigate("/html/newMessage.html", { item: items[0] });
                        }, function () {
                            wait.style.display = "none";
                            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                            msg.showAsync();
                        });
                    }, function () {
                        var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                        msg.showAsync();
                    });
                });
            }
        };
    }
    
    function drawECG() {
        var i = 0;
        basic = 10 + interval;

        context.clearRect(0, 0, canWidth + 20, canHight + 20);

        context.save();

        context.lineWidth = 1;
        context.strokeStyle = "#ffd590";

        for (i = 0; i < numRowLine ; i++, basic = basic + interval) {

            context.beginPath();
            context.moveTo(10, basic);
            context.lineTo(10 + canWidth, basic);
            context.stroke();
        }
        basic = 10 + interval;
        for (i = 0; i < numColumnLine - 1; i++, basic = basic + interval) {

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
        basic = 10;
        for (i = 1; i < 840; i++) {
            context.beginPath();
            context.moveTo(basic, dataToDraw[i - 1]);
            basic = basic + widthInterval;
            context.lineTo(basic, dataToDraw[i]);
            context.stroke();
        }
        context.restore();


        context.save();
        context.lineCap = "round";
        context.lineWidth = 6;

        context.strokeStyle = "#b3b5b4";
        context.beginPath();
        context.moveTo(10, 10);
        context.lineTo(10, canHight + 10);
        context.stroke();

        context.beginPath();
        context.moveTo(10, 10);
        context.lineTo(canWidth + 10, 10);
        context.stroke();

        context.beginPath();
        context.moveTo(canWidth + 10, canHight + 10);
        context.lineTo(canWidth + 10, 10);
        context.stroke();

        context.beginPath();
        context.moveTo(canWidth + 10, canHight + 10);
        context.lineTo(10, canHight + 10);
        context.stroke();


        context.restore();
    }


    function itemTemplateFunction(itemPromise) {

        return itemPromise.then(function (item) {
            var div = document.createElement("div");
            div.className = "mediumListIconTextItem";
            div.id = "maillistItem";

            var img = document.createElement("img");
            if (item.data.fromorto == 1) {
                img.src = "/images/mail2.png";
            }
            else {
                img.src = "/images/mail1.png";
            }
            
            img.alt = item.data.title;
            img.className = "mediumListIconTextItem-Image";
            div.appendChild(img);

            var childDiv = document.createElement("div");
            childDiv.className = "mediumListIconTextItem-Detail";
            var title = document.createElement("h2");
            title.innerText = item.data.title;
            title.id = item.data.id;
            childDiv.appendChild(title);

            var desc = document.createElement("h3");
            desc.innerText = item.data.author;
            childDiv.appendChild(desc);

            div.appendChild(childDiv);
            if (item.data.isread == 0 && item.data.fromorto == 1) {
                title.style.fontWeight = 800;
            }
            return div;
        });
    };

    function addToItem() {
        var affectedItems=document.querySelectorAll(".listItem");
        var newItem = document.createElement("div");
        newItem.className = "listItem";
        newItem.style.background = "#FFFFFF";
        var addToList = WinJS.UI.Animation.createAddToListAnimation(newItem, affectedItems);
        maillist.insertBefore(newItem, maillist.firstChild);
        addToList.execute();
        var a = new WinJS.UI.ListView();
    }

    function selectionchanged() {
        if (list.selection.count() === 1) {
            list.selection.getItems().then(function (items) {
                mailMessageTitle.innerText = items[0].data.title;
                mailMessageAuthor.innerText = "  From: " + items[0].data.author;
                mailMessageTime.innerText = "  Time: " + items[0].data.time;
                mailMessageBody.innerText = items[0].data.message;
                fileName = items[0].data.ecg;
                playbutton.style.opacity = 1;
                if (items[0].data.fromorto == 1) {
                    WinJS.xhr({ url: myHost + "/outpatient/" + items[0].data.id + "/read" }).then(function (r) {
                        var title = document.getElementById(items[0].data.id);
                        title.style.fontWeight = 200;
                    }, function (r) {
                        var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                        msg.commands.append(new Windows.UI.Popups.UICommand("OK", function () { }));
                        msg.showAsync();
                    });
                }
            });
        }
    }

    WinJS.UI.Pages.define("/html/outpatient.html", {
        ready: ready
    });
    
})();
