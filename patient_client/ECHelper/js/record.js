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

    var recordData;
    var list;
    var month;
    var datas = new Array(2080);
    var dataToDraw = new Array(1040);
    var context;
    var interval = 6;
    var numRowLine = 52;
    var numColumnLine = 130;
    var canWidth = interval * numColumnLine;
    var canHight = interval * numRowLine;
    var widthInterval = canWidth / 1040;
    var basic = 0;
    var connectionReader = new ConnectionLibrary.ECGReader();
    var dataLength = 0;
    var isplay = 0;
    var fileName;
    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    function ready(element, options) {
        appBarInit();
        liveInit();
        var enterPage = WinJS.UI.Animation.enterPage(rootGrid, { top: "100px", left: "100px" });
        enterPage.done();

        recordData = ECGData.recordlist.items;

        month = new Array(["January", 0], ["February", 0], ["March", 0], ["April", 0], ["May", 0], ["June", 0], ["July", 0], ["August", 0], ["September", 0], ["October", 0], ["November", 0], ["December", 0]);
        list = document.getElementById("recordlist").winControl;
        list.itemTemplate = itemTemplateFunction;
        list.addEventListener("selectionchanged", selectionchanged);

        context = document.getElementById("myECGPlay").getContext('2d');

        var ECGSlid = document.getElementById("ECGSlid");
        ECGSlid.onmouseup = function () {
            var ratio = ECGSlid.value;
            datas = connectionReader.getData(ratio / 100 * (dataLength - 2080), 2080);
            for (var i = 0, j = 0; i < 2080; i = i + 2, j++) {
                dataToDraw[j] = 322 - (datas[i] * 256 + datas[i + 1]) * 0.3;
            }
            drawECG();
        }

        var playbutton = document.getElementById("playecgbutton");
        playbutton.onclick = function () {
            if (isplay == 0) {
                playbutton.style.display = "none";
                myECGCanvas.style.display = "block";
                myECGCanvas.style.opacity = "1";
                WinJS.UI.Animation.showPopup(myECGCanvas, null);
                isplay = 1;
            }
            connectionReader.open(fileName).then(function (count) {
                dataLength = count;

                datas = connectionReader.getData(0, 2080);
                for (var i = 0, j = 0; i < 2080; i = i + 2, j++) {
                    dataToDraw[j] = 322 - (datas[i] * 256 + datas[i + 1]) * 0.3;
                }
                drawECG();
            });
        };

        var canvasclosebutton = document.getElementById("canvasclosebutton");
        canvasclosebutton.addEventListener("MSPointerDown", onPointerDown, false);
        canvasclosebutton.addEventListener("MSPointerUp", onPointerUp, false);
        canvasclosebutton.onclick = function () {
            myECGCanvas.style.opacity = "0";
            WinJS.UI.Animation.hidePopup(myECGCanvas);
            isplay = 0;
            myECGCanvas.style.display = "none";
            playbutton.style.display = "block";
        }
        monthInit();
        ulInit();
        $(function () {
            $().timelinr()
        });
    }

    WinJS.UI.Pages.define("/html/record.html", {
        ready: ready
    });

    function onPointerDown(evt) {
        WinJS.UI.Animation.pointerDown(evt.srcElement);
    }

    function onPointerUp(evt) {
        WinJS.UI.Animation.pointerUp(evt.srcElement);
    }

    function monthInit() {
        var i, j;
        for (i = 0; i < recordData.length; i++) {
            if (recordData[i].time[5] == 0) {
                j = recordData[i].time[6];
            }
            else {
                j = recordData[i].time[5] + recordData[i].time[6];
            }
            month[j-1][1]++;
        }
        
        for (i = 1; i < 12; i++) {
            month[i][1] = month[i][1] + month[i - 1][1];
        }
        for (i = 11; i > 0; i--) {
            month[i][1] = month[i - 1][1];
        }
    }

    function itemTemplateFunction(itemPromise) {

        return itemPromise.then(function (item) {
            var div = document.createElement("div");
            div.className = "mediumListIconTextItem";

            var img = document.createElement("img");
            img.src = "/images/record.png";
            img.alt = item.data.title;
            img.className = "mediumListIconTextItem-Image";
            div.appendChild(img);

            var childDiv = document.createElement("div");
            childDiv.className = "mediumListIconTextItem-Detail";
            var title = document.createElement("h3");
            title.innerText = item.data.title;
            childDiv.appendChild(title);

            var desc = document.createElement("h5");
            desc.innerText = item.data.author;
            childDiv.appendChild(desc);

            div.appendChild(childDiv);

            return div;
        });
    };

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
        for (i = 1; i < 1040; i++) {
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

        context.strokeStyle = "#009D1A";
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

    function ulInit() {
        var ul = document.getElementById("dates");
        for (var i = 0; i < 12; i++) {
            var newItem = document.createElement("li");
            var a = document.createElement("a");
            a.href = "#" + month[i][0];
            a.innerText = month[i][0];
            newItem.appendChild(a);
            newItem.title = i;
            newItem.onclick = function scroll(e) {
                e = e || window.event;
                var target = e.target || ev.srcElement;
                if (target.parentNode.title != "") {
                    list.scrollPosition = month[target.parentNode.title][1] * 85;
                }
            }
            ul.appendChild(newItem);
        }
    }

    function selectionchanged() {
        if (list.selection.count() === 1) {
            list.selection.getItems().then(function (items) {
                var mailMessageTitle = document.getElementById("mailMessageTitle");
                var mailMessageAuthor = document.getElementById("mailMessageAuthor");
                var mailMessageTime = document.getElementById("mailMessageTime");
                var recordMessageBody = document.getElementById("recordMessageBody");
                mailMessageTitle.innerText = items[0].data.title;
                mailMessageAuthor.innerText = "  From: " + items[0].data.author;
                mailMessageTime.innerText = "  Time: " + items[0].data.time;
                recordMessageBody.innerText = items[0].data.message;
                fileName = items[0].data.ecg;
                playbutton.style.opacity = 1;
            });
        }
    }

})();
