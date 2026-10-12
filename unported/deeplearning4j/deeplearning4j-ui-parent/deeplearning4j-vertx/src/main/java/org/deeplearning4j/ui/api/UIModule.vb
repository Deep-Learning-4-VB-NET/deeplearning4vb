Imports System.Collections.Generic
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports I18NResource = org.deeplearning4j.ui.i18n.I18NResource

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.ui.api


	Public Interface UIModule

		''' <summary>
		''' Get the list of Type IDs that should be collected from the registered <seealso cref="StatsStorage"/> instances, and
		''' passed on to the <seealso cref="reportStorageEvents(Collection)"/> method.
		''' </summary>
		''' <returns> List of relevant Type IDs </returns>
		ReadOnly Property CallbackTypeIDs As IList(Of String)

		''' <summary>
		''' Get a list of <seealso cref="Route"/> objects, that specify GET/SET etc methods, and how these should be handled.
		''' </summary>
		''' <returns> List of routes </returns>
		ReadOnly Property Routes As IList(Of Route)

		''' <summary>
		''' Whenever the <seealso cref="UIServer"/> receives some <seealso cref="StatsStorageEvent"/>s from one of the registered <seealso cref="StatsStorage"/>
		''' instances, it will filter these and pass on to the UI module those ones that match one of the Type IDs from
		''' <seealso cref="getCallbackTypeIDs()"/>.<br>
		''' Typically, these will be batched together at least somewhat, rather than being reported individually.
		''' </summary>
		''' <param name="events">       List of relevant events (type IDs match one of those from <seealso cref="getCallbackTypeIDs()"/> </param>
		Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent))

		''' <summary>
		''' Notify the UI module that the given <seealso cref="StatsStorage"/> instance has been attached to the UI
		''' </summary>
		''' <param name="statsStorage">    Stats storage that has been attached </param>
		Sub onAttach(ByVal statsStorage As StatsStorage)

		''' <summary>
		''' Notify the UI module that the given <seealso cref="StatsStorage"/> instance has been detached from the UI
		''' </summary>
		''' <param name="statsStorage">    Stats storage that has been detached </param>
		Sub onDetach(ByVal statsStorage As StatsStorage)

		''' <returns> List of internationalization resources </returns>
		ReadOnly Property InternationalizationResources As IList(Of I18NResource)
	End Interface

End Namespace