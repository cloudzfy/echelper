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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
//using ImagineCupServer.DataModel;
using ImagineCupServer.DataContracts;

namespace ImagineCupServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICallbackService" in both code and config file together.
    [ServiceContract(CallbackContract=typeof(ICallback))]
    interface ICallbackService
    {
        [OperationContract(IsOneWay = true)]
        void Login(String id);

        [OperationContract(IsOneWay = true)]
        void EmergencyCall(string toWho, EmergencyMesg message);
    }
}
