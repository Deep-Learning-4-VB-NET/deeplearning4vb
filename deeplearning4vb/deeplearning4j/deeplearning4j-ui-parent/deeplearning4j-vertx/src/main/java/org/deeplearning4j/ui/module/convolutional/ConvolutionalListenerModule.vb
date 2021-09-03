Imports System.Collections.Generic
Imports System.IO
Imports Buffer = io.vertx.core.buffer.Buffer
Imports RoutingContext = io.vertx.ext.web.RoutingContext
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports StatsStorageListener = org.deeplearning4j.core.storage.StatsStorageListener
Imports HttpMethod = org.deeplearning4j.ui.api.HttpMethod
Imports Route = org.deeplearning4j.ui.api.Route
Imports UIModule = org.deeplearning4j.ui.api.UIModule
Imports I18NResource = org.deeplearning4j.ui.i18n.I18NResource
Imports ConvolutionListenerPersistable = org.deeplearning4j.ui.model.weights.ConvolutionListenerPersistable

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

Namespace org.deeplearning4j.ui.module.convolutional


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ConvolutionalListenerModule implements org.deeplearning4j.ui.api.UIModule
	Public Class ConvolutionalListenerModule
		Implements UIModule

		Private Const TYPE_ID As String = "ConvolutionalListener"

		Private lastStorage As StatsStorage
		Private lastSessionID As String
		Private lastWorkerID As String
		Private lastTimeStamp As Long

		Public Overridable ReadOnly Property CallbackTypeIDs As IList(Of String) Implements UIModule.getCallbackTypeIDs
			Get
				Return Collections.singletonList(TYPE_ID)
			End Get
		End Property

		Public Overridable ReadOnly Property Routes As IList(Of Route) Implements UIModule.getRoutes
			Get
				Dim r As New Route("/activations", HttpMethod.GET, Function(path, rc) rc.response().sendFile("templates/Activations.html"))
				Dim r2 As New Route("/activations/data", HttpMethod.GET, Sub(path, rc) Me.getImage(rc))
    
				Return New List(Of Route) From {r, r2}
			End Get
		End Property

		Public Overridable Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent)) Implements UIModule.reportStorageEvents
			SyncLock Me
				For Each sse As StatsStorageEvent In events
					If TYPE_ID.Equals(sse.getTypeID()) AndAlso sse.getEventType() = StatsStorageListener.EventType.PostStaticInfo Then
						If sse.getTimestamp() > lastTimeStamp Then
							lastStorage = sse.getStatsStorage()
							lastSessionID = sse.getSessionID()
							lastWorkerID = sse.getWorkerID()
							lastTimeStamp = sse.getTimestamp()
						End If
					End If
				Next sse
			End SyncLock
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

		Private Sub getImage(ByVal rc As RoutingContext)
			If lastTimeStamp > 0 AndAlso lastStorage IsNot Nothing Then
				Dim p As Persistable = lastStorage.getStaticInfo(lastSessionID, TYPE_ID, lastWorkerID)
				If TypeOf p Is ConvolutionListenerPersistable Then
					Dim clp As ConvolutionListenerPersistable = DirectCast(p, ConvolutionListenerPersistable)
					Dim bi As BufferedImage = clp.getImg()
					Dim baos As New MemoryStream()
					Try
						ImageIO.write(bi, "png", baos)
					Catch e As IOException
						log.warn("Error displaying image", e)
					End Try

					rc.response().putHeader("content-type", "image/png").end(Buffer.buffer(baos.toByteArray()))
				Else
					rc.response().putHeader("content-type", "image/png").end(Buffer.buffer(New SByte(){}))
				End If
			Else
				rc.response().putHeader("content-type", "image/png").end(Buffer.buffer(New SByte(){}))
			End If
		End Sub
	End Class

End Namespace