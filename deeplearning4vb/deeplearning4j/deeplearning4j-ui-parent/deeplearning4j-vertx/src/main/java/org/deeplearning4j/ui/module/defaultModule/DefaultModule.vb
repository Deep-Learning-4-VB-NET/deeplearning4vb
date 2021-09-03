Imports System.Collections.Generic
Imports HttpResponseStatus = io.netty.handler.codec.http.HttpResponseStatus
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports HttpMethod = org.deeplearning4j.ui.api.HttpMethod
Imports Route = org.deeplearning4j.ui.api.Route
Imports UIModule = org.deeplearning4j.ui.api.UIModule
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

Namespace org.deeplearning4j.ui.module.defaultModule


	Public Class DefaultModule
		Implements UIModule

		Private ReadOnly multiSession As Boolean

		Public Sub New()
			Me.New(False)
		End Sub

		''' 
		''' <param name="multiSession"> multi-session mode </param>
		Public Sub New(ByVal multiSession As Boolean)
			Me.multiSession = multiSession
		End Sub

		Public Overridable ReadOnly Property CallbackTypeIDs As IList(Of String) Implements UIModule.getCallbackTypeIDs
			Get
				Return Collections.emptyList()
			End Get
		End Property

		Public Overridable ReadOnly Property Routes As IList(Of Route) Implements UIModule.getRoutes
			Get
				Dim r As New Route("/", HttpMethod.GET, Function(params, rc) rc.response().putHeader("location", "/train" & (If(multiSession, "", "/overview"))).setStatusCode(HttpResponseStatus.FOUND.code()).end())
    
				Return Collections.singletonList(r)
			End Get
		End Property

		Public Overridable Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent)) Implements UIModule.reportStorageEvents
			'Nop-op
		End Sub

		Public Overridable Sub onAttach(ByVal statsStorage As StatsStorage) Implements UIModule.onAttach
			'No-op
		End Sub

		Public Overridable Sub onDetach(ByVal statsStorage As StatsStorage) Implements UIModule.onDetach
			'No-op
		End Sub

		Public Overridable ReadOnly Property InternationalizationResources As IList(Of I18NResource) Implements UIModule.getInternationalizationResources
			Get
				Return Collections.emptyList()
			End Get
		End Property
	End Class

End Namespace