Imports System
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports Source = org.nd4j.common.loader.Source
Imports SourceFactory = org.nd4j.common.loader.SourceFactory

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



	<Serializable>
	Public Class RemoteFileSourceFactory
		Implements SourceFactory

		<NonSerialized>
		Private fileSystem As FileSystem
		Private ReadOnly conf As Broadcast(Of SerializableHadoopConfig)

		Public Sub New(ByVal configuration As Broadcast(Of SerializableHadoopConfig))
			Me.conf = configuration
		End Sub

		Public Overridable Function getSource(ByVal path As String) As Source
			If fileSystem Is Nothing Then
				Dim c As Configuration = If(conf Is Nothing, DefaultHadoopConfig.get(), conf.getValue().getConfiguration())
				Try
					fileSystem = FileSystem.get(New URI(path), c)
				Catch u As Exception When TypeOf u Is IOException OrElse TypeOf u Is URISyntaxException
					Throw New Exception(u)
				End Try
			End If

			Return New RemoteFileSource(path, fileSystem)
		End Function
	End Class

End Namespace