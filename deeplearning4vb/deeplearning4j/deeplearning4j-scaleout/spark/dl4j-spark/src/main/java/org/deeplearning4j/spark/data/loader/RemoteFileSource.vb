Imports System.IO
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports Source = org.nd4j.common.loader.Source

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

Namespace org.deeplearning4j.spark.data.loader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class RemoteFileSource implements org.nd4j.common.loader.Source
	Public Class RemoteFileSource
		Implements Source

		Public Const DEFAULT_BUFFER_SIZE As Integer = 4*1024*2014
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String path;
		Private path As String
		Private ReadOnly fileSystem As FileSystem
		Private ReadOnly bufferSize As Integer

		Public Sub New(ByVal path As String, ByVal fileSystem As FileSystem)
			Me.New(path, fileSystem, DEFAULT_BUFFER_SIZE)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.InputStream getInputStream() throws java.io.IOException
		Public Overridable ReadOnly Property InputStream As Stream Implements Source.getInputStream
			Get
				Return fileSystem.open(New Path(path), bufferSize)
			End Get
		End Property
	End Class

End Namespace