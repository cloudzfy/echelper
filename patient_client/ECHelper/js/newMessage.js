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

    var contactlist;
    var filename;
    var showPanel;
    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    function ready(element, options) {
        appBarInit();
        liveInit();
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "100px", left: "100px" });
        enterPage.done();

        showPanel = false;
        var myFilelist = document.getElementById("myFilelist");

        contactlist = ECGData.outpatientdoctorlist.items;

        var list = document.getElementById("doctorSelection").winControl;
        list.itemTemplate = itemTemplateFunction;

        var filelist = document.getElementById("fileSelection");
        for (var i = 0; i < ECGData.ecgfilelist.items.length; i++) {
            var div = document.createElement("div");
            div.className = "mediumListIconTextItem";

            //var img = document.createElement("img");
            //img.src = item.data.image;
            //img.alt = item.data.name;
            //img.className = "mediumListIconTextItem-Image";
            //div.appendChild(img);

            var childDiv = document.createElement("div");
            childDiv.className = "mediumListIconTextItem-Detail";
            var time1 = document.createElement("h2");
            var myDT = new ConnectionLibrary.MyDatetime();
            var realfilename = ECGData.ecgfilelist.items[i].filename.substring(9, ECGData.ecgfilelist.items[i].filename.length);
            time1.innerText = myDT.changeToDatetimeFormat(realfilename);
            childDiv.appendChild(time1);

            filename = ECGData.ecgfilelist.items[i].filename.substring(9, ECGData.ecgfilelist.items[i].filename.length);

            div.appendChild(childDiv);
            div.addEventListener("MSPointerDown", onPointerDown, false);
            div.addEventListener("MSPointerUp", onPointerUp, false);
            filelist.appendChild(div);
        }
        function onPointerDown(evt) {
            WinJS.UI.Animation.pointerDown(evt.srcElement);
        }

        function onPointerUp(evt) {
            WinJS.UI.Animation.pointerUp(evt.srcElement);
        }

        var mailTitle = document.getElementById("mailTitle");
        var mailTitle_i = false;
        if (options != null) {
            var myEmail = document.getElementById("myEmail");
            myEmail.value = options.item.data.author;
            mailTitle.value = "Re: " + options.item.data.title;
            mailTitle_i = true;
        }

        if (!mailTitle_i) {
            mailTitle.style.color = "#868181";
            mailTitle.value = "New Title";
        }
        mailTitle.onchange = function () {
            if (mailTitle.value === "") {
                mailTitle.value = "New Title";
                mailTitle.style.color = "#868181";
                mailTitle_i = false;
            } else if (!mailTitle_i) {
                mailTitle.value = "";
                mailTitle.style.color = "black";
                mailTitle_i = true;
            }
        }
        mailTitle.onfocus = function () {
            if (!mailTitle_i) {
                mailTitle.value = "";
                mailTitle.style.color = "black";
                mailTitle_i = true;
            }
        }
        mailTitle.onfocusout = function () {
            if (mailTitle.value === "") {
                mailTitle.value = "New Title";
                mailTitle.style.color = "#868181";
                mailTitle_i = false;
            }
        }

        var sendbutton = document.getElementById("sendbutton");
        sendbutton.onclick = function () {
            var wait = document.getElementById("wait");
            wait.style.display = "block";
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            var root = doc.createElement("NewMailDataContract");
            var author = doc.createElement("DoctorId");
            author.innerText = document.getElementById("myEmail").value;
            root.appendChild(author);
            var ecg = doc.createElement("ECG");
            ecg.innerText = filename;
            root.appendChild(ecg);
            var fromorto = doc.createElement("FromOrTo");
            fromorto.innerText = 0;
            root.appendChild(fromorto);
            var patientid = doc.createElement("PatientId");
            patientid.innerText = "liaomin";
            root.appendChild(patientid);
            var message = doc.createElement("TextContent");
            message.innerText = document.getElementById("mailMessage").value;
            root.appendChild(message);
            var time = doc.createElement("Time");
            time.innerText = "1999-05-31T11:20:01";
            root.appendChild(time);
            var title = doc.createElement("Title");
            title.innerText = document.getElementById("mailTitle").value;
            root.appendChild(title);


            doc.appendChild(root);
            var docxml = doc.getXml();
            WinJS.xhr({
                type: "POST",
                url: myHost + "/outpatient/new",
                headers: {
                    "Content-type": "application/xml",
                    "Content-length": docxml.length
                },
                data: docxml
            }).then(function () {
                wait.style.display = "none";
                var message = document.getElementById("rootGrid");
                message.style.msTransitionProperty = 'all';
                message.style.msTransitionProperty = '-ms-transform, opacity';
                message.style.msTransitionDelay = '0s,0s';
                message.style.msTransitionDuration = '4s,2s';
                message.style.msTransitionTimingFunction = 'ease';
                message.style.msTransformOrigin = '-50%';
                message.style.msTransform = 'translateX(0px) translateY(100px) translateZ(0px) rotateX(0deg) rotateY(-90deg) rotateZ(0deg) scaleX(-1) scaleY(-1) skewX(0deg) skewY(0deg)';
                message.style.opacity = '0';

                message.addEventListener("MSTransitionEnd", resetPage, false);
            }, function (r) {
                wait.style.display = "none";
                var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                msg.commands.append(new Windows.UI.Popups.UICommand("OK", function () { }));
                msg.showAsync();
            });

        }

        var addECGbutton = document.getElementById("addECGbutton");
        addECGbutton.onclick = function () {
            var fileSelection = document.getElementById("fileSelection");
            fileSelection.style.opacity = "1";
            fileSelection.style.display = "block";
            myFilelist.style.opacity = "1";
            myFilelist.style.display = "block";
            WinJS.UI.Animation.showPanel(myFilelist, { top: "0px", left: "350px" }).done(function () {
                showPanel = true;
            });
        }

        document.body.onclick = function (args) {
            try {
                if (showPanel && args.layerX < document.body.clientWidth - 350) {
                    WinJS.UI.Animation.hidePanel(myFilelist, { top: "0px", left: "350px" }).done(function () {
                        myFilelist.style.opacity = "0";
                        myFilelist.style.display = "none";
                        showPanel = false;
                    });
                }
            } catch (e) { }
        };

        var cancelbutton = document.getElementById("cancelbutton");
        cancelbutton.onclick = function () {
            var message = document.getElementById("rootGrid");
            message.style.msTransitionProperty = 'all';
            message.style.msTransitionProperty = '-ms-transform, opacity';
            message.style.msTransitionDelay = '0s,0s';
            message.style.msTransitionDuration = '4s,2s';
            message.style.msTransitionTimingFunction = 'ease';
            message.style.msTransformOrigin = '50%';
            message.style.msTransform = 'translateX(0px) translateY(500px) translateZ(0px) rotateX(0deg) rotateY(0deg) rotateZ(0deg) scaleX(0) scaleY(0) skewX(0deg) skewY(0deg)';
            message.style.opacity = '0';

            message.addEventListener("MSTransitionEnd", resetPage, false);
        }

    }

    function resetPage() {
        WinJS.Navigation.navigate("/html/outpatient.html");
        //var message1 = document.getElementById("xxx");
        //message1.style.msTransition = 'none';
        //message1.style.opacity = '1';
        //message1.style.msTransform = '';
        //message1.style.msTransformOrigin = '50%';
    }
    function itemTemplateFunction(itemPromise) {
        return itemPromise.then(function (item) {
            var div = document.createElement("div");
            div.className = "mediumListIconTextItem";
            div.id = "contactitem";

            var img = document.createElement("img");
            img.src = item.data.image;
            img.alt = item.data.name;
            img.className = "mediumListIconTextItem-Image";
            div.appendChild(img);

            var childDiv = document.createElement("div");
            childDiv.className = "mediumListIconTextItem-Detail";
            var name = document.createElement("h2");
            name.innerText = item.data.name;
            childDiv.appendChild(name);

            var email = document.createElement("h5");
            email.innerText = item.data.email;
            childDiv.appendChild(email);
            div.appendChild(childDiv);

            div.title = item.index;
            div.onclick = function (e) {
                e = e || window.event;
                var target = e.target || ev.srcElement;
                var myTarget = target;
                while (myTarget.id != "contactitem") {
                    myTarget = myTarget.parentNode;
                }
                var myEmail = document.getElementById("myEmail");
                myEmail.value = contactlist[myTarget.title].email;
            }

            return div;
        });
    };

    WinJS.UI.Pages.define("/html/newMessage.html", {
        ready: ready
    });
})();