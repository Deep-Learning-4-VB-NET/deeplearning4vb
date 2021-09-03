Imports System.Collections.Generic
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

Namespace org.deeplearning4j.ui.module


	Public Class SameDiffModule
		Implements UIModule

		Public Sub New()

		End Sub

		Public Overridable ReadOnly Property CallbackTypeIDs As IList(Of String) Implements UIModule.getCallbackTypeIDs
			Get
				Return Collections.emptyList()
			End Get
		End Property

		Public Overridable ReadOnly Property Routes As IList(Of Route) Implements UIModule.getRoutes
			Get
		'        Route r1 = new Route("/samediff", HttpMethod.GET, FunctionType.Supplier,
		'                        () -> ok(org.deeplearning4j.ui.views.html.samediff.SameDiffUI.apply()));
				Dim r1 As New Route("/samediff", HttpMethod.GET, Function(path,rc) rc.response().sendFile("templates/SameDiffUI.html"))
				Return Collections.singletonList(r1)
			End Get
		End Property

		Public Overridable Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent)) Implements UIModule.reportStorageEvents

		End Sub

		Public Overridable Sub onAttach(ByVal statsStorage As StatsStorage) Implements UIModule.onAttach

		End Sub

		Public Overridable Sub onDetach(ByVal statsStorage As StatsStorage) Implements UIModule.onDetach

		End Sub

		Public Overridable ReadOnly Property InternationalizationResources As IList(Of I18NResource) Implements UIModule.getInternationalizationResources
			Get
				Return Collections.emptyList()
			End Get
		End Property
	End Class

End Namespace