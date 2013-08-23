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
    var showPanel;
    var searchCount;
    var ismine;
    var doctorlist;

    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    function ready(element, options) {
        appBarInit();
        liveInit();
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "100px", left: "100px" });
        enterPage.done();

        doctorlist = ECGData.alldoctorlist.items;

        showPanel = false;
        myDoctorList.style.opacity = "0";
        myDoctorList.style.display = "none";
        ismine = false;
        searchCount = 0;
        searchList();

        myDoctorListButton.addEventListener("MSPointerDown", onPointerDown, false);
        myDoctorListButton.addEventListener("MSPointerUp", onPointerUp, false);
        allDoctorListButton.addEventListener("MSPointerDown", onPointerDown, false);
        allDoctorListButton.addEventListener("MSPointerUp", onPointerUp, false);

        
        myDoctorListButton.onclick = function () {
            var searchInput = document.getElementById("searchInput");
            searchInput.value = "";
            ismine = true;
            var doctorInfoList = document.getElementById("doctorInfoList");
            var listItems = document.querySelectorAll(".doctorInfoItem:not([deleting])");
            for (var i = listItems.length - 1; i >= 0; i--) {
                doctorInfoList.removeChild(listItems[i]);
            }
            searchCount = 0;
            searchList();
        }

        allDoctorListButton.onclick = function () {
            var searchInput = document.getElementById("searchInput");
            searchInput.value = "";
            ismine = false;
            var doctorInfoList = document.getElementById("doctorInfoList");
            var listItems = document.querySelectorAll(".doctorInfoItem:not([deleting])");
            for (var i = listItems.length - 1; i >= 0; i--) {
                doctorInfoList.removeChild(listItems[i]);
            }
            searchCount = 0;
            searchList();
        }

        var searchInput = document.getElementById("searchInput");
        searchInput.onkeyup = function () {
            ismine = false;
            var doctorInfoList = document.getElementById("doctorInfoList");
            var listItems = document.querySelectorAll(".doctorInfoItem:not([deleting])");
            for (var i = listItems.length - 1; i >= 0; i--) {
                doctorInfoList.removeChild(listItems[i]);
            }
            searchCount = 0;
            searchList();
        }

        document.body.onclick = function (args) {
            try{ 
                if (showPanel && args.layerX < document.body.clientWidth - 350) {
                    WinJS.UI.Animation.hidePanel(myDoctorList, { top: "0px", left: "350px" }).done(function () {
                        myDoctorList.style.opacity = "0";
                        myDoctorList.style.display = "none";
                        var askMsg = document.getElementById("askMsg");
                        askMsg.style.display = "none";
                        var askbutton = document.getElementById("askbutton");
                        askbutton.style.display = "none";
                        showPanel = false;
                    });
                }
                else if (!showPanel) {
                    myDoctorList.style.opacity = "1";
                    myDoctorList.style.display = "block";
                    WinJS.UI.Animation.showPanel(myDoctorList, { top: "0px", left: "350px" }).done(function () {
                        showPanel = true;
                    });
                }
            } catch (e) { }
        };

    }

    WinJS.UI.Pages.define("/html/doctorList.html", {
        ready: ready
    });

    function searchList() {
        if (searchCount < doctorlist.length) {
            var searchInput = document.getElementById("searchInput");
            var filter = new RegExp(searchInput.value, "i");
            var a = filter.test(name);
            if (!filter.test(doctorlist[searchCount].name) || (ismine && (!doctorlist[searchCount].ismine))) {
                searchCount++;
                searchList();
                return;
            }
            var affectedItems = document.querySelectorAll(".doctorInfoItem");
            var newItem = document.createElement("div");
            newItem.className = "doctorInfoItem";
            newItem.addEventListener("MSPointerDown", onPointerDown, false);
            newItem.addEventListener("MSPointerUp", onPointerUp, false);

            var img = document.createElement("img");
            img.src = doctorlist[searchCount].image;
            img.alt = doctorlist[searchCount].name;
            img.className = "itemImage";
            newItem.appendChild(img);
            var name = document.createElement("h1");
            name.innerText = doctorlist[searchCount].name;
            name.className = "itemName";
            newItem.appendChild(name);
            var br = document.createElement("h6");
            br.className = "itemSpace";
            br.innerText = " ";
            newItem.appendChild(br);
            var grade = document.createElement("h2");
            grade.innerText = doctorlist[searchCount].grade;
            grade.className = "itemGrade";
            newItem.appendChild(grade);
            var br = document.createElement("h5");
            br.innerHTML = "<br/>";
            br.className = "itemSpace";
            newItem.appendChild(br);
            var description = document.createElement("p");
            description.innerHTML = doctorlist[searchCount].description;
            description.className = "itemDescription";
            newItem.appendChild(description);
            var emailText = document.createElement("p");
            emailText.innerText = "  Email: " + doctorlist[searchCount].email;
            emailText.className = "itemDescription";
            newItem.appendChild(emailText);
            var phoneText = document.createElement("p");
            phoneText.innerText = "  Phone: " + doctorlist[searchCount].phone;
            phoneText.className = "itemDescription";
            newItem.appendChild(phoneText);

            newItem.title = searchCount;
            newItem.onclick = function (e) {
                e = e || window.event;
                var target = e.target || ev.srcElement;
                var myTarget;
                if (target.parentNode.id === "doctorInfoList") {
                    myTarget = target;
                } else {
                    myTarget = target.parentNode;
                }
                var askbutton = document.getElementById("askbutton");
                var askMsg = document.getElementById("askMsg");
                if (doctorlist[myTarget.title].ismine) {
                    askMsg.style.display = "block";
                    askMsg.innerText = "      Dr. " + doctorlist[myTarget.title].name + " is already in your contact.";
                    askbutton.style.display = "none";
                }
                else {
                    askMsg.innerText = "      Are you sure to add Dr. " + doctorlist[myTarget.title].name + " to your contact list?";
                    askMsg.style.display = "block";
                    askbutton.style.display = "block";
                    var askbuttony = document.getElementById("askbuttony");
                    askbuttony.onclick = function () {
                        var wait = document.getElementById("wait");
                        wait.style.display = "block";
                        WinJS.xhr({ url: myHost + "/contact/adddoctor/" + doctorlist[myTarget.title].id }).then(function (r) {
                            doctorlist[myTarget.title].ismine = true;
                            wait.style.display = "none";
                        }, function (result) {
                            var msg = new Windows.UI.Popups.MessageDialog("Sorry, the network is unavailable.", "Error");
                            msg.commands.append(new Windows.UI.Popups.UICommand("OK", function () { }));
                            msg.showAsync();
                            wait.style.display = "none";
                        });
                        WinJS.UI.Animation.hidePanel(myDoctorList, { top: "0px", left: "350px" }).done(function () {
                            myDoctorList.style.opacity = "0";
                            myDoctorList.style.display = "none";
                            var askMsg = document.getElementById("askMsg");
                            askMsg.style.display = "none";
                            var askbutton = document.getElementById("askbutton");
                            askbutton.style.display = "none";
                            showPanel = false;
                        });
                    }
                }
            }

            var addToSearchList;
            if (doctorInfoList.childElementCount > 0) {
                addToSearchList = WinJS.UI.Animation.createAddToListAnimation(newItem, affectedItems);
                doctorInfoList.insertBefore(newItem, doctorInfoList.childNodes[Math.random() * doctorInfoList.childElementCount]);
            } else {
                addToSearchList = WinJS.UI.Animation.createAddToSearchListAnimation(newItem);
                doctorInfoList.appendChild(newItem);
            }
            addToSearchList.execute();
            searchCount++;
            WinJS.Promise.timeout(Math.floor(Math.random() * 200)).then(searchList);
        }

    }

    function onPointerDown(evt) {
        WinJS.UI.Animation.pointerDown(evt.srcElement);
    }

    function onPointerUp(evt) {
        WinJS.UI.Animation.pointerUp(evt.srcElement);
    }

    function itemTemplateFunction(itemPromise) {

        return itemPromise.then(function (item) {
            var div = document.createElement("div");
            div.className = "itemtemplate";

            var img = document.createElement("img");
            var rand = new ConnectionLibrary.MyDatetime();
            var num = rand.random() % 5 + 1;
            
            img.src = "ms-appx:///images/doctor_" + num + ".png";
            img.alt = item.data.name;
            img.className = "itemImage";
            div.appendChild(img);

            var name = document.createElement("h1");
            name.innerText = item.data.name;
            name.className = "itemName";
            div.appendChild(name);
            var br = document.createElement("h6");
            br.className = "itemSpace";
            br.innerText = " ";
            div.appendChild(br);
            var grade = document.createElement("h2");
            grade.innerText = item.data.grade;
            grade.className = "itemGrade";
            div.appendChild(grade);
            var br = document.createElement("h5");
            br.innerHTML = "<br/>";
            br.className = "itemSpace";
            div.appendChild(br);
            var description = document.createElement("p");
            description.innerHTML = item.data.description;
            description.className = "itemDescription";
            div.appendChild(description);
            var emailText = document.createElement("p");
            emailText.innerText = "  Email: " + item.data.email;
            emailText.className = "itemDescription";
            div.appendChild(emailText);
            var phoneText = document.createElement("p");
            phoneText.innerText = "  Phone: " + item.data.phone;
            phoneText.className = "itemDescription";
            div.appendChild(phoneText);

            return div;
        });
    };

    function selectionchanged() {
        list.selection.getItems().then(function (items) {
            var affectedItems = document.querySelectorAll(".myDoctorListItem");
            var newItem = document.createElement("span");
            newItem.className = "doctorInfoItem";

            var img = document.createElement("img");
            img.src = "/images/smalllogo.png"
            img.alt = items[0].data.name;
            img.className = "itemImage";
            newItem.appendChild(img);

            var childDiv = document.createElement("span");
            childDiv.className = "itemDetail";
            var name = document.createElement("h2");
            name.innerText = items[0].data.name;
            childDiv.appendChild(name);

            var grade = document.createElement("h4");
            grade.innerText = items[0].data.grade;
            childDiv.appendChild(grade);
            newItem.appendChild(childDiv);
            
            var addToList = WinJS.UI.Animation.createAddToListAnimation(newItem, affectedItems);
            list_myDoctorList.insertBefore(newItem, list_myDoctorList.firstChild);
            addToList.execute();
        });
    }

})();
