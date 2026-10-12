Imports System
Imports System.IO
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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

Namespace org.nd4j.versioncheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @Builder public class VersionInfo
	Public Class VersionInfo

		Private Const HTML_SPACE As String = "%2520" 'Space: "%20" -> % character: "%25" -> "%2520"
		Private Shared ReadOnly SUFFIX_LENGTH As Integer = "-git.properties".length()

		Private groupId As String
		Private artifactId As String

		Private tags As String ' =${git.tags} // comma separated tag names
		Private branch As String ' =${git.branch}
		Private dirty As String ' =${git.dirty}
		Private remoteOriginUrl As String ' =${git.remote.origin.url}

		Private commitId As String ' =${git.commit.id.full} OR ${git.commit.id}
		Private commitIdAbbrev As String ' =${git.commit.id.abbrev}
		Private describe As String ' =${git.commit.id.describe}
		Private describeShort As String ' =${git.commit.id.describe-short}
		Private commitUserName As String ' =${git.commit.user.opName}
		Private commitUserEmail As String ' =${git.commit.user.email}
		Private commitMessageFull As String ' =${git.commit.message.full}
		Private commitMessageShort As String ' =${git.commit.message.short}
		Private commitTime As String ' =${git.commit.time}
		Private closestTagName As String ' =${git.closest.tag.opName}
		Private closestTagCommitCount As String ' =${git.closest.tag.commit.count}

		Private buildUserName As String ' =${git.build.user.opName}
		Private buildUserEmail As String ' =${git.build.user.email}
		Private buildTime As String ' =${git.build.time}
		Private buildHost As String ' =${git.build.host}
		Private buildVersion As String ' =${git.build.version}

		Public Sub New(ByVal groupId As String, ByVal artifactId As String, ByVal buildVersion As String)
			Me.groupId = groupId
			Me.artifactId = artifactId
			Me.buildVersion = buildVersion
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public VersionInfo(String propertiesFilePath) throws java.io.IOException
		Public Sub New(ByVal propertiesFilePath As String)
			Me.New((New File(propertiesFilePath)).toURI())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public VersionInfo(java.net.URI uri) throws java.io.IOException
		Public Sub New(ByVal uri As URI)
			'Can't use new File(uri).getPath() for URIs pointing to resources in JARs
			'But URI.toString() returns "%2520" instead of spaces in path - https://github.com/eclipse/deeplearning4j/issues/6056
			Dim path As String = uri.ToString().replaceAll(HTML_SPACE, " ")
			Dim idxOf As Integer = path.LastIndexOf("/"c)
			idxOf = Math.Max(idxOf, path.LastIndexOf("\"c))
			Dim filename As String
			If idxOf <= 0 Then
				filename = path
			Else
				filename = path.Substring(idxOf + 1)
			End If

			idxOf = filename.IndexOf("-"c)
			groupId = filename.Substring(0, idxOf)
			artifactId = filename.Substring(idxOf + 1, (filename.Length - SUFFIX_LENGTH) - (idxOf + 1))


			'Extract values from properties file:
			Dim properties As New Properties()
			Dim u As New URL(path) 'Can't use URI.toUrl() due to spaces in path
			Using [is] As Stream = New java.io.BufferedInputStream(u.openStream())
				properties.load([is])
			End Using

			Me.tags = properties.get("git.tags").ToString()
			Me.branch = properties.get("git.branch").ToString()
			Me.dirty = properties.get("git.dirty").ToString()
			Me.remoteOriginUrl = properties.get("git.remote.origin.url").ToString()

			Me.commitId = properties.get("git.commit.id.full").ToString() ' OR properties.get("git.commit.id") depending on your configuration
			Me.commitIdAbbrev = properties.get("git.commit.id.abbrev").ToString()
			Me.describe = properties.get("git.commit.id.describe").ToString()
			Me.describeShort = properties.get("git.commit.id.describe-short").ToString()
			Me.commitUserName = properties.get("git.commit.user.name").ToString()
			Me.commitUserEmail = properties.get("git.commit.user.email").ToString()
			Me.commitMessageFull = properties.get("git.commit.message.full").ToString()
			Me.commitMessageShort = properties.get("git.commit.message.short").ToString()
			Me.commitTime = properties.get("git.commit.time").ToString()
			Me.closestTagName = properties.get("git.closest.tag.name").ToString()
			Me.closestTagCommitCount = properties.get("git.closest.tag.commit.count").ToString()

			Me.buildUserName = properties.get("git.build.user.name").ToString()
			Me.buildUserEmail = properties.get("git.build.user.email").ToString()
			Me.buildTime = properties.get("git.build.time").ToString()
			Me.buildHost = properties.get("git.build.host").ToString()
			Me.buildVersion = properties.get("git.build.version").ToString()
		End Sub
	End Class

End Namespace