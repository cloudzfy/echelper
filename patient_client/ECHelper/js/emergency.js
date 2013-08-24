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

    var map;
    var geolocator = new Windows.Devices.Geolocation.Geolocator();
    var currentdoctorlist;
    var latitude = 0, longitude = 0;
    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    function ready(element, options) {
        appBarInit();
        liveInit();
        currentdoctorlist = ECGData.emergencydoctorlist.items;
        Microsoft.Maps.loadModule('Microsoft.Maps.Map', { callback: initMap });
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "100px", left: "100px" });
        enterPage.done();


        var list = document.getElementById("doctorSelection").winControl;
        list.itemTemplate = itemTemplateFunction;
    }

    WinJS.UI.Pages.define("/html/emergency.html", {
        ready: ready
    });

    function initMap() {
        try {
            var mapOptions =
                    {
                        credentials: 'AiKfqDwCzlqXs_ZkqdkrOQsYfHS9kJNQg6xVyQ1uhcaCx4Loh5wfbNnDGd1-sH-r',
                        center: new Microsoft.Maps.Location(40.71, -74.00),
                        mapTypeId: Microsoft.Maps.MapTypeId.road,
                        zoom: 8
                    };
            map = new Microsoft.Maps.Map(document.getElementById("mapdiv"), mapOptions);
            geolocator.onpositionchanged = onPositionChanged;
            getLocalInfo();
        }
        catch (e) {
            var md = new Windows.UI.Popups.MessageDialog("Oop! There is something wrong in Bing Maps.\nError: " + e.message, "Error");
            md.showAsync();
        }
    }

    function onPositionChanged(eventArgs) {
        var pos = eventArgs.position;
        var options = map.getOptions();
        latitude = pos.coordinate.latitude;
        longitude = pos.coordinate.longitude;
        options.center = new Microsoft.Maps.Location(pos.coordinate.latitude, pos.coordinate.longitude);
        options.zoom = 20;
        map.setView(options);
        var pin = new Microsoft.Maps.Pushpin(options.center,
                {
                    icon: 'images/BluePushpin.png',
                    width: 50, height: 50,
                });
        WinJS.xhr({ url: "http://dev.virtualearth.net/REST/v1/Locations/" + pos.coordinate.latitude + "," + pos.coordinate.longitude + "?key=" + "AiKfqDwCzlqXs_ZkqdkrOQsYfHS9kJNQg6xVyQ1uhcaCx4Loh5wfbNnDGd1-sH-r" }).then(function (r) {
            var result = r.responseText;
            var address = JSON.parse(result);
            var pinInfobox = new Microsoft.Maps.Infobox(pin.getLocation(),
            {
                title: 'My Location',
                description: address.resourceSets[0].resources[0].name,
                visible: false,
                offset: new Microsoft.Maps.Point(-10, 15)
            });
            Microsoft.Maps.Events.addHandler(pin, 'click', function () {
                pinInfobox.setOptions({ visible: true });
            });
            Microsoft.Maps.Events.addHandler(map, 'viewchange', function () {
                pinInfobox.setOptions({ visible: false });
            });
            map.entities.push(pinInfobox);
        });
        map.entities.push(pin);

        var wait = document.getElementById("wait");
        wait.style.display = "block";
        var doc = new Windows.Data.Xml.Dom.XmlDocument;
        var root = doc.createElement("OnlineStatusDataContract");
        var mlatitude = doc.createElement("Latitude");
        mlatitude.innerText = pos.coordinate.latitude;
        root.appendChild(mlatitude);
        var mlongitude = doc.createElement("Longitude");
        mlongitude.innerText = pos.coordinate.longitude;
        root.appendChild(mlongitude);
        var id = doc.createElement("UserName");
        id.innerText = patientName;
        root.appendChild(id);
        doc.appendChild(root);
        var docxml = doc.getXml();
        WinJS.xhr({
            type: "POST",
            url: myHost + "/online",
            headers: {
                "Content-type": "application/xml",
                "Content-length": docxml.length
            },
            data: docxml
        }).then(function () {
            wait.style.display = "none";
        }, function () {
            wait.style.display = "none";
            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
            msg.showAsync();
        });

         
    }

    function itemTemplateFunction(itemPromise) {

        return itemPromise.then(function (item) {
            var div = document.createElement("div");
            div.className = "mediumListIconTextItem";
            div.id = "doctorSelectionItem";

            var img = document.createElement("img");
            var rand = new ConnectionLibrary.MyDatetime();
            var num = rand.random() % 6 + 1;
            img.src = "ms-appx:///images/doctor_" + num + ".png";
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
                var myTarget=target;
                while (myTarget.id != "doctorSelectionItem") {
                    myTarget = myTarget.parentNode;
                }
                var options = map.getOptions();
                options.center = new Microsoft.Maps.Location(currentdoctorlist[myTarget.title].latitude, currentdoctorlist[myTarget.title].longitude);
                options.zoom = 20;
                map.setView(options);
            }

            return div;
        });
    };

    function getLocalInfo() {
        //map.entities.clear();
        for (var i = 0; i < currentdoctorlist.length; i++) {
            var pin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(currentdoctorlist[i].latitude, currentdoctorlist[i].longitude),
                {
                    icon: '/images/doctormark.png',
                    width: 50, height: 50,
                });
            Microsoft.Maps.Events.addHandler(pin, 'click', function (e) {
                var msg = new Windows.UI.Popups.MessageDialog("Are you sure you want to ask for help?", "HELP");
                msg.commands.append(new Windows.UI.Popups.UICommand("OK", function () {
                    var wait = document.getElementById("wait");
                    wait.style.display = "block";
                    var doc = new Windows.Data.Xml.Dom.XmlDocument;
                    var root = doc.createElement("EmergencyMesg");
                    var id = doc.createElement("patientID");
                    id.innerText = patientName;
                    root.appendChild(id);
                    var mlatitude = doc.createElement("latitude");
                    mlatitude.innerText = latitude;
                    root.appendChild(mlatitude);
                    var mlongitude = doc.createElement("longitude");
                    mlongitude.innerText = longitude;
                    root.appendChild(mlongitude);
                    doc.appendChild(root);
                    var docxml = doc.getXml();

                    WinJS.xhr({
                        type: "POST",
                        url: myHost + "/emergency/" + doctorName + "/call",
                        headers: {
                            "Content-type": "application/xml",
                            "Content-length": docxml.length
                        },
                        data: docxml
                    }).then(function () {
                        wait.style.display = "none";
                    }, function () {
                        var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                        msg.showAsync();
                    });
                }));
                msg.showAsync();
            });
            map.entities.push(pin);
        }
    }

})();
