Imports System
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FSDataInputStream = org.apache.hadoop.fs.FSDataInputStream
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.data


	Public Class PathToDataSetFunction
		Implements [Function](Of String, DataSet)

		Public Const BUFFER_SIZE As Integer = 4194304 '4 MB

		<NonSerialized>
		Private fileSystem As FileSystem
		Private ReadOnly conf As Broadcast(Of SerializableHadoopConfig)

		Public Sub New()
			Me.New(Nothing)
		End Sub

		Public Sub New(ByVal configuration As Broadcast(Of SerializableHadoopConfig))
			Me.conf = configuration
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet call(String path) throws Exception
		Public Overrides Function [call](ByVal path As String) As DataSet
			If fileSystem Is Nothing Then
				Dim c As Configuration = If(conf Is Nothing, DefaultHadoopConfig.get(), conf.getValue().getConfiguration())
				Try
					fileSystem = FileSystem.get(New URI(path), c)
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End If

			Dim ds As New DataSet()
			Try
					Using inputStream As FSDataInputStream = fileSystem.open(New Path(path), BUFFER_SIZE)
					ds.load(inputStream)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try

			Return ds
		End Function
	End Class

End Namespace