Imports System
Imports System.Collections.Generic
Imports HttpResponseStatus = io.netty.handler.codec.http.HttpResponseStatus
Imports JsonObject = io.vertx.core.json.JsonObject
Imports RoutingContext = io.vertx.ext.web.RoutingContext
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports org.deeplearning4j.core.storage
Imports RemoteUIStatsStorageRouter = org.deeplearning4j.core.storage.impl.RemoteUIStatsStorageRouter
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


Namespace org.deeplearning4j.ui.module.remote


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class RemoteReceiverModule implements org.deeplearning4j.ui.api.UIModule
	Public Class RemoteReceiverModule
		Implements UIModule

'JAVA TO VB CONVERTER NOTE: The field enabled was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private enabled_Conflict As New AtomicBoolean(False)
'JAVA TO VB CONVERTER NOTE: The field statsStorage was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private statsStorage_Conflict As StatsStorageRouter

		Public Overridable Property Enabled As Boolean
			Set(ByVal enabled As Boolean)
				Me.enabled_Conflict.set(enabled)
				If Not enabled Then
					Me.statsStorage_Conflict = Nothing
				End If
			End Set
			Get
				Return enabled_Conflict.get() AndAlso Me.statsStorage_Conflict IsNot Nothing
			End Get
		End Property


		Public Overridable WriteOnly Property StatsStorage As StatsStorageRouter
			Set(ByVal statsStorage As StatsStorageRouter)
				Me.statsStorage_Conflict = statsStorage
			End Set
		End Property

		Public Overridable ReadOnly Property CallbackTypeIDs As IList(Of String) Implements UIModule.getCallbackTypeIDs
			Get
				Return Collections.emptyList()
			End Get
		End Property

		Public Overridable ReadOnly Property Routes As IList(Of Route) Implements UIModule.getRoutes
			Get
				Dim r As New Route("/remoteReceive", HttpMethod.POST, Sub(path, rc) Me.receiveData(rc))
				Return Collections.singletonList(r)
			End Get
		End Property

		Public Overridable Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent)) Implements UIModule.reportStorageEvents
			'No op

		End Sub

		Public Overridable Sub onAttach(ByVal statsStorage As StatsStorage)
			'No op
		End Sub

		Public Overridable Sub onDetach(ByVal statsStorage As StatsStorage)
			'No op
		End Sub

		Public Overridable ReadOnly Property InternationalizationResources As IList(Of I18NResource) Implements UIModule.getInternationalizationResources
			Get
				Return Collections.emptyList()
			End Get
		End Property

		Private Sub receiveData(ByVal rc As RoutingContext)
			If Not enabled_Conflict.get() Then
				rc.response().setStatusCode(HttpResponseStatus.FORBIDDEN.code()).end("UI server remote listening is currently disabled. Use UIServer.getInstance().enableRemoteListener()")
				Return
			End If

			If statsStorage_Conflict Is Nothing Then
				rc.response().setStatusCode(HttpResponseStatus.INTERNAL_SERVER_ERROR.code()).end("UI Server remote listener: no StatsStorage instance is set/available to store results")
				Return
			End If

			Dim jo As JsonObject = rc.getBodyAsJson()
			Dim map As IDictionary(Of String, Object) = jo.getMap()
			Dim type As String = DirectCast(map("type"), String)
			Dim dataClass As String = DirectCast(map("class"), String)
			Dim data As String = DirectCast(map("data"), String)

			If type Is Nothing OrElse dataClass Is Nothing OrElse data Is Nothing Then
				log.warn("Received incorrectly formatted data from remote listener (has type = " & (type IsNot Nothing) & ", has data class = " & (dataClass IsNot Nothing) & ", has data = " & (data IsNot Nothing) & ")")
				rc.response().setStatusCode(HttpResponseStatus.BAD_REQUEST.code()).end("Received incorrectly formatted data")
				Return
			End If

			Select Case type.ToLower()
				Case "metadata"
					Dim meta As StorageMetaData = getMetaData(dataClass, data)
					If meta IsNot Nothing Then
						statsStorage_Conflict.putStorageMetaData(meta)
					End If
				Case "staticinfo"
					Dim staticInfo As Persistable = getPersistable(dataClass, data)
					If staticInfo IsNot Nothing Then
						statsStorage_Conflict.putStaticInfo(staticInfo)
					End If
				Case "update"
					Dim update As Persistable = getPersistable(dataClass, data)
					If update IsNot Nothing Then
						statsStorage_Conflict.putUpdate(update)
					End If
				Case Else

			End Select

			rc.response().end()
		End Sub

		Private Function getMetaData(ByVal dataClass As String, ByVal content As String) As StorageMetaData
			Dim meta As StorageMetaData
			Try
				Dim clazz As Type = DL4JClassLoading.loadClassByName(dataClass)
				If clazz.IsAssignableFrom(GetType(StorageMetaData)) Then
					meta = clazz.asSubclass(GetType(StorageMetaData)).getDeclaredConstructor().newInstance()
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.warn("Skipping invalid remote data: class {} in not an instance of {}", dataClass, GetType(StorageMetaData).FullName)
					Return Nothing
				End If
			Catch e As Exception
				log.warn("Skipping invalid remote data: exception encountered for class {}", dataClass, e)
				Return Nothing
			End Try

			Try
				Dim bytes() As SByte = DatatypeConverter.parseBase64Binary(content)
				meta.decode(bytes)
			Catch e As Exception
				log.warn("Skipping invalid remote UI data: exception encountered when deserializing data", e)
				Return Nothing
			End Try

			Return meta
		End Function

		Private Function getPersistable(ByVal dataClass As String, ByVal content As String) As Persistable
			Dim persistable As Persistable
			Try
				Dim clazz As Type = DL4JClassLoading.loadClassByName(dataClass)
				If clazz.IsAssignableFrom(GetType(Persistable)) Then
					persistable = clazz.asSubclass(GetType(Persistable)).getDeclaredConstructor().newInstance()
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.warn("Skipping invalid remote data: class {} in not an instance of {}", dataClass, GetType(Persistable).FullName)
					Return Nothing
				End If
			Catch e As Exception
				log.warn("Skipping invalid remote UI data: exception encountered for class {}", dataClass, e)
				Return Nothing
			End Try

			Try
				Dim bytes() As SByte = DatatypeConverter.parseBase64Binary(content)
				persistable.decode(bytes)
			Catch e As Exception
				log.warn("Skipping invalid remote data: exception encountered when deserializing data", e)
				Return Nothing
			End Try

			Return persistable
		End Function
	End Class
End Namespace