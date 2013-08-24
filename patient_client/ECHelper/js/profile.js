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

    var myprofile;
    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    function ready(element, options) {
        appBarInit();
        liveInit();
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "100px", left: "100px" });
        enterPage.done();
        myprofile = ECGData.profile.items;
        UIInit();
    }

    function UIInit() {
        var mName_i;
        var mName = document.getElementById("mName");
        if (myprofile[0].name === "") {
            mName.value = "Name";
            mName.style.color = "#868181";
            mName_i = false;
        } else {
            mName.value = myprofile[0].name;
            mName.style.color = "black";
            mName_i = true;
        }
        mName.onchange = function () {
            if (mName.value === "") {
                mName.value = "Name";
                mName.style.color = "#868181";
                mName_i = false;
            } else if (!mName_i) {
                mName.value = "";
                mName.style.color = "black";
                mName_i = true;
            }
        }
        mName.onfocus = function () {
            if (!mName_i) {
                mName.value = "";
                mName.style.color = "black";
                mName_i = true;
            }
        }
        mName.onfocusout = function () {
            if (mName.value === "") {
                mName.value = "Name";
                mName.style.color = "#868181";
                mName_i = false;
            }
        }

        var mAge_i;
        var mAge = document.getElementById("mAge");
        if (myprofile[0].age === "") {
            mAge.value = "Age";
            mAge.style.color = "#868181";
            mAge_i = false;
        } else {
            mAge.value = myprofile[0].age;
            mAge.style.color = "black";
            mAge_i = true;
        }
        mAge.onchange = function () {
            if (mAge.value === "") {
                mAge.value = "Age";
                mAge.style.color = "#868181";
                mAge_i = false;
            } else if (!mAge_i) {
                mAge.value = "";
                mAge.style.color = "black";
                mAge_i = true;
            }
        }
        mAge.onfocus = function () {
            if (!mAge_i) {
                mAge.value = "";
                mAge.style.color = "black";
                mAge_i = true;
            }
        }
        mAge.onfocusout = function () {
            if (mAge.value === "") {
                mAge.value = "Age";
                mAge.style.color = "#868181";
                mAge_i = false;
            }
        }

        var mDesciprion_i;
        var mDesciprion = document.getElementById("mDesciprion");
        if (myprofile[0].description === "") {
            mDesciprion.value = "Please describe your medical history here.";
            mDesciprion.style.color = "#868181";
            mDesciprion_i = false;
        } else {
            mDesciprion.value = myprofile[0].description;
            mDesciprion.style.color = "black";
            mDesciprion_i = true;
        }
        mDesciprion.onchange = function () {
            if (mDesciprion.value === "") {
                mDesciprion.value = "Please describe your medical history here.";
                mDesciprion.style.color = "#868181";
                mDesciprion_i = false;
            } else if (!mAge_i) {
                mDesciprion.value = "";
                mDesciprion.style.color = "black";
                mDesciprion_i = true;
            }
        }
        mDesciprion.onfocus = function () {
            if (!mDesciprion_i) {
                mDesciprion.value = "";
                mDesciprion.style.color = "black";
                mDesciprion_i = true;
            }
        }
        mDesciprion.onfocusout = function () {
            if (mDesciprion.value === "") {
                mDesciprion.value = "Please describe your medical history here.";
                mDesciprion.style.color = "#868181";
                mDesciprion_i = false;
            }
        }

        var mGender = document.getElementById("mGender");
        mGender.value = myprofile[0].gender;

        var mAllery1 = document.getElementById("mAllery1");
        mAllery1.value = myprofile[0].allery;

        var acceptbutton = document.getElementById("acceptbutton");
        acceptbutton.onclick = function () {
            var wait = document.getElementById("wait");
            wait.style.display = "block";
            var doc = new Windows.Data.Xml.Dom.XmlDocument;
            var root = doc.createElement("PatientUserDataContract");
            var age = doc.createElement("Age");
            age.innerText = mAge.value;
            root.appendChild(age);
            var allery = doc.createElement("Allery");
            allery.innerText = mAllery1.value;
            root.appendChild(allery);
            var description = doc.createElement("Description");
            description.innerText = mDesciprion.value;
            root.appendChild(description);
            var gender = doc.createElement("Gender");
            gender.innerText = mGender.value;
            root.appendChild(gender);
            var name = doc.createElement("NickName");
            name.innerText = mName.value;
            root.appendChild(name);
            var id = doc.createElement("UserName");
            id.innerText = "liaomin";
            root.appendChild(id);
            doc.appendChild(root);
            var docxml = doc.getXml();

            WinJS.xhr({
                type: "POST",
                url: myHost + "/profile/update",
                headers: {
                    "Content-type": "application/xml",
                    "Content-length": docxml.length
                },
                data: docxml
            }).then(function () {
                wait.style.display = "none";
                WinJS.Navigation.back();
            }, function () {
                var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                msg.commands.append(new Windows.UI.Popups.UICommand("OK", function () { }));
                msg.showAsync();
            });
        }

        var rejectbutton = document.getElementById("rejectbutton");
        rejectbutton.onclick = function () {
            var exitPage = WinJS.UI.Animation.exitPage(rootGrid, { top: "-200px" });
            exitPage.done();
            WinJS.Navigation.back();
        }
    }
    WinJS.UI.Pages.define("/html/profile.html", {
        ready: ready
    });

})();
