Imports System
Imports System.Collections.Generic
Imports HttpResponseStatus = io.netty.handler.codec.http.HttpResponseStatus
Imports FileUpload = io.vertx.ext.web.FileUpload
Imports RoutingContext = io.vertx.ext.web.RoutingContext
Imports FileUtils = org.apache.commons.io.FileUtils
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports HttpMethod = org.deeplearning4j.ui.api.HttpMethod
Imports Route = org.deeplearning4j.ui.api.Route
Imports UIModule = org.deeplearning4j.ui.api.UIModule
Imports I18NResource = org.deeplearning4j.ui.i18n.I18NResource
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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

Namespace org.deeplearning4j.ui.module.tsne


	Public Class TsneModule
		Implements UIModule

		Private Const UPLOADED_FILE As String = "UploadedFile"

		Private knownSessionIDs As IDictionary(Of String, IList(Of String)) = Collections.synchronizedMap(New LinkedHashMap(Of String, IList(Of String))())
		Private uploadedFileLines As IList(Of String) = Nothing

		Public Sub New()
		End Sub

		Public Overridable ReadOnly Property CallbackTypeIDs As IList(Of String)
			Get
				Return java.util.Collections.emptyList()
			End Get
		End Property

		Public Overridable ReadOnly Property Routes As IList(Of Route)
			Get
				Dim r1 As New Route("/tsne", HttpMethod.GET, Function(path, rc) rc.response().sendFile("templates/Tsne.html"))
				Dim r2 As New Route("/tsne/sessions", HttpMethod.GET, Sub(path, rc) Me.listSessions(rc))
				Dim r3 As New Route("/tsne/coords/:sid", HttpMethod.GET, Sub(path, rc) Me.getCoords(path.get(0), rc))
				Dim r4 As New Route("/tsne/upload", HttpMethod.POST, Sub(path, rc) Me.uploadFile(rc))
				Dim r5 As New Route("/tsne/post/:sid", HttpMethod.POST, Sub(path, rc) Me.postFile(path.get(0), rc))
				Return New List(Of Route) From {r1, r2, r3, r4, r5}
			End Get
		End Property

		Public Overridable Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent))
			'No-op
		End Sub

		Public Overridable Sub onAttach(ByVal statsStorage As StatsStorage) Implements UIModule.onAttach
			'No-op
		End Sub

		Public Overridable Sub onDetach(ByVal statsStorage As StatsStorage) Implements UIModule.onDetach
			'No-op
		End Sub

		Public Overridable ReadOnly Property InternationalizationResources As IList(Of I18NResource)
			Get
				Return java.util.Collections.emptyList()
			End Get
		End Property

		Private Function asJson(ByVal o As Object) As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(o)
			Catch e As JsonProcessingException
				Throw New Exception("Error converting object to JSON", e)
			End Try
		End Function

		Private Sub listSessions(ByVal rc As RoutingContext)
			Dim list As IList(Of String) = New List(Of String)(knownSessionIDs.Keys)
			If uploadedFileLines IsNot Nothing Then
				list.Add(UPLOADED_FILE)
			End If
			rc.response().putHeader("content-type", "application/json").end(asJson(list))
		End Sub

		Private Sub getCoords(ByVal sessionId As String, ByVal rc As RoutingContext)
			If UPLOADED_FILE.Equals(sessionId) AndAlso uploadedFileLines IsNot Nothing Then
				rc.response().putHeader("content-type", "application/json").end(asJson(uploadedFileLines))
			ElseIf knownSessionIDs.ContainsKey(sessionId) Then
				rc.response().putHeader("content-type", "application/json").end(asJson(knownSessionIDs(sessionId)))
			Else
				rc.response().end()
			End If
		End Sub

		Private Sub uploadFile(ByVal rc As RoutingContext)
			postFile(Nothing, rc)
		End Sub

		Private Sub postFile(ByVal sid As String, ByVal rc As RoutingContext)
			Dim files As ISet(Of FileUpload) = rc.fileUploads()
			If files Is Nothing OrElse files.Count = 0 Then
				rc.response().end()
				Return
			End If

			Dim u As FileUpload = files.GetEnumerator().next()
			Dim f As New File(u.uploadedFileName())
			Dim lines As IList(Of String)
			Try
				lines = FileUtils.readLines(f, StandardCharsets.UTF_8)
			Catch e As IOException
				rc.response().setStatusCode(HttpResponseStatus.BAD_REQUEST.code()).end("Could not read from uploaded file")
				Return
			End Try

			If sid Is Nothing Then
				uploadedFileLines = lines
			Else
				knownSessionIDs(sid) = lines
			End If
			rc.response().end("File uploaded: " & u.fileName() & ", " & u.contentType())
		End Sub
	End Class

End Namespace