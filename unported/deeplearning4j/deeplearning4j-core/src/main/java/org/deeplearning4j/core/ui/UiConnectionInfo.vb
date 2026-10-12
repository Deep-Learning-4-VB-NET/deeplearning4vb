Imports System
Imports System.Text
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull

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

Namespace org.deeplearning4j.core.ui

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class UiConnectionInfo
	Public Class UiConnectionInfo
'JAVA TO VB CONVERTER NOTE: The field sessionId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sessionId_Conflict As String
		Private login As String
		Private password As String
		Private address As String = "localhost"
		Private port As Integer = 8080
		Private path As String = ""
		Private useHttps As Boolean

		Public Sub New()
			Me.sessionId_Conflict = System.Guid.randomUUID().ToString()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setSessionId(@NonNull String sessionId)
		Public Overridable WriteOnly Property SessionId As String
			Set(ByVal sessionId As String)
				Me.sessionId_Conflict = sessionId
			End Set
		End Property

		''' <summary>
		''' This method returns scheme, address and port for this UiConnectionInfo
		''' 
		''' i.e: https://localhost:8080
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property FirstPart As String
			Get
				Dim builder As New StringBuilder()
    
				builder.Append(If(useHttps, "https", "http")).Append("://").Append(address).Append(":").Append(port).Append("")
    
				Return builder.ToString()
			End Get
		End Property

		Public Overridable ReadOnly Property SecondPart As String
			Get
				Return getSecondPart("")
			End Get
		End Property

		Public Overridable Function getSecondPart(ByVal nPath As String) As String
			Dim builder As New StringBuilder()

			If path IsNot Nothing AndAlso path.Length > 0 Then
				builder.Append(If(path.StartsWith("/", StringComparison.Ordinal), path, ("/" & path))).Append("/")
			End If

			If nPath IsNot Nothing Then
				nPath = nPath.replaceFirst("^/", "")
				builder.Append(If(nPath.StartsWith("/", StringComparison.Ordinal), nPath, ("/" & nPath))).Append("/")
			End If


			Return builder.ToString().replaceAll("\/{2,}", "/")
		End Function

		Public Overridable Function getFullAddress(ByVal nPath As String) As String
			If nPath Is Nothing OrElse nPath.Length = 0 Then
				Return FullAddress
			Else
				Return FirstPart & getSecondPart(nPath) & "?sid=" & Me.getSessionId()
			End If
		End Function

		Public Overridable ReadOnly Property FullAddress As String
			Get
				Return FirstPart & SecondPart
			End Get
		End Property

		Public Class Builder
			Friend info As New UiConnectionInfo()

			''' <summary>
			''' This method allows you to specify sessionId for this UiConnectionInfo instance
			''' 
			''' PLEASE NOTE: This is not recommended. Advised behaviour - keep it random, as is.
			''' </summary>
			''' <param name="sessionId">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setSessionId(@NonNull String sessionId)
			Public Overridable Function setSessionId(ByVal sessionId As String) As Builder
				info.SessionId = sessionId
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setLogin(@NonNull String login)
			Public Overridable Function setLogin(ByVal login As String) As Builder
				info.setLogin(login)
				Return Me
			End Function

			Public Overridable Function setPassword(ByVal password As String) As Builder
				info.setPassword(password)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setAddress(@NonNull String address)
			Public Overridable Function setAddress(ByVal address As String) As Builder
				info.setAddress(address)
				Return Me
			End Function

			Public Overridable Function setPort(ByVal port As Integer) As Builder
				If port <= 0 Then
					Throw New System.InvalidOperationException("UiServer port can't be <= 0")
				End If
				info.setPort(port)
				Return Me
			End Function

			Public Overridable Function enableHttps(ByVal reallyEnable As Boolean) As Builder
				info.setUseHttps(reallyEnable)
				Return Me
			End Function

			''' <summary>
			''' If you're using UiServer as servlet, located not at root folder of webserver (i.e. http://yourdomain.com/somepath/webui/), you can set path here.
			''' For provided example path will be "/somepath/webui/"
			''' </summary>
			''' <param name="path">
			''' @return </param>
			Public Overridable Function setPath(ByVal path As String) As Builder
				info.setPath(path)
				Return Me
			End Function

			Public Overridable Function build() As UiConnectionInfo
				Return info
			End Function
		End Class
	End Class

End Namespace