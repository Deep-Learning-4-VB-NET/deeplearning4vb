Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports I18N = org.deeplearning4j.ui.api.I18N
Imports UIModule = org.deeplearning4j.ui.api.UIModule

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

Namespace org.deeplearning4j.ui.i18n


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DefaultI18N implements org.deeplearning4j.ui.api.I18N
	Public Class DefaultI18N
		Implements I18N

		Public Const DEFAULT_LANGUAGE As String = "en"
		Public Const FALLBACK_LANGUAGE As String = "en" 'use this if the specified language doesn't have the requested message

'JAVA TO VB CONVERTER NOTE: The field instance was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared instance_Conflict As DefaultI18N
		Private Shared sessionInstances As IDictionary(Of String, I18N) = Collections.synchronizedMap(New Dictionary(Of String, I18N)())
		Private Shared languageLoadingException As Exception = Nothing


		Private currentLanguage As String = DEFAULT_LANGUAGE
		Private messagesByLanguage As IDictionary(Of String, IDictionary(Of String, String)) = New Dictionary(Of String, IDictionary(Of String, String))()

		''' <summary>
		''' Get global instance (used in single-session mode) </summary>
		''' <returns> global instance </returns>
		Public Shared ReadOnly Property Instance As I18N
			Get
				SyncLock GetType(DefaultI18N)
					If instance_Conflict Is Nothing Then
						instance_Conflict = New DefaultI18N()
					End If
					Return instance_Conflict
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Get instance for session </summary>
		''' <param name="sessionId"> session ID for multi-session mode, leave it {@code null} for global instance </param>
		''' <returns> instance for session, or global instance </returns>
		Public Shared Function getInstance(ByVal sessionId As String) As I18N
			SyncLock GetType(DefaultI18N)
				If sessionId Is Nothing Then
					Return Instance
				Else
					If Not sessionInstances.ContainsKey(sessionId) Then
						sessionInstances(sessionId) = New DefaultI18N()
					End If
					Return sessionInstances(sessionId)
				End If
			End SyncLock
		End Function

		''' <summary>
		''' Remove I18N instance for session </summary>
		''' <param name="sessionId"> session ID </param>
		''' <returns> the previous value associated with {@code sessionId},
		''' or null if there was no mapping for {@code sessionId} </returns>
		Public Shared Function removeInstance(ByVal sessionId As String) As I18N
			SyncLock GetType(DefaultI18N)
				Return sessionInstances.Remove(sessionId)
			End SyncLock
		End Function


		Private Sub New()
			loadLanguages()
		End Sub

		Private Sub loadLanguages()
			SyncLock Me
				Dim loadedModules As ServiceLoader(Of UIModule) = DL4JClassLoading.loadService(GetType(UIModule))
        
				For Each [module] As UIModule In loadedModules
					Dim resources As IList(Of I18NResource) = [module].getInternationalizationResources()
					For Each resource As I18NResource In resources
						Try
							Dim path As String = resource.getResource()
							Dim idxLast As Integer = path.LastIndexOf("."c)
							If idxLast < 0 Then
								log.warn("Skipping language resource file: cannot infer language: {}", path)
								Continue For
							End If
        
							Dim langCode As String = path.Substring(idxLast + 1).ToLower()
							Dim map As IDictionary(Of String, String) = messagesByLanguage.computeIfAbsent(langCode, Function(k) New Dictionary(Of String, String)())
        
							parseFile(resource, map)
						Catch t As Exception
							log.warn("Error parsing UI I18N content file; skipping: {}", resource.getResource(), t)
							languageLoadingException = t
						End Try
					Next resource
				Next [module]
			End SyncLock
		End Sub

		Private Sub parseFile(ByVal r As I18NResource, ByVal results As IDictionary(Of String, String))
			Dim lines As IList(Of String)
			Try
					Using [is] As Stream = r.InputStream
					lines = IOUtils.readLines([is], StandardCharsets.UTF_8)
					End Using
			Catch e As IOException
				log.debug("Error parsing UI I18N content file; skipping: {} - {}", r.getResource(), e.Message)
				Return
			End Try

			Dim count As Integer = 0
			For Each line As String In lines
				If Not line.matches(".+=.*") Then
					log.debug("Invalid line in I18N file: {}, ""{}""", r.getResource(), line)
					Continue For
				End If
				Dim idx As Integer = line.IndexOf("="c)
				Dim key As String = line.Substring(0, idx)
				Dim value As String = line.Substring(Math.Min(idx + 1, line.Length))
				results(key) = value
				count += 1
			Next line

			log.trace("Loaded {} messages from file {}", count, r.getResource())
		End Sub

		Public Overridable Function getMessage(ByVal key As String) As String Implements I18N.getMessage
			Return getMessage(currentLanguage, key)
		End Function

		Public Overridable Function getMessage(ByVal langCode As String, ByVal key As String) As String Implements I18N.getMessage
			Dim messagesForLanguage As IDictionary(Of String, String) = messagesByLanguage(langCode)

			Dim msg As String
			If messagesForLanguage IsNot Nothing Then
				msg = messagesForLanguage(key)
				If msg Is Nothing AndAlso Not FALLBACK_LANGUAGE.Equals(langCode) Then
					'Try getting the result from the fallback language
					Return getMessage(FALLBACK_LANGUAGE, key)
				End If
			Else
				msg = getMessage(FALLBACK_LANGUAGE, key)
			End If
			Return msg
		End Function

		Public Overridable Property DefaultLanguage As String Implements I18N.getDefaultLanguage
			Get
				Return currentLanguage
			End Get
			Set(ByVal langCode As String)
				Me.currentLanguage = langCode
				log.debug("UI: Set language to {}", langCode)
			End Set
		End Property


		Public Overridable Function getMessages(ByVal langCode As String) As IDictionary(Of String, String)
			'Start with map for default language
			'Then overwrite with the actual language - so any missing are reported in default language
			Dim ret As IDictionary(Of String, String) = New Dictionary(Of String, String)(messagesByLanguage(FALLBACK_LANGUAGE))
			If Not langCode.Equals(FALLBACK_LANGUAGE) Then
				ret.PutAll(messagesByLanguage(langCode))
			End If
			Return ret
		End Function
	End Class

End Namespace