Imports System
Imports System.Collections.Generic
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports RemoteFileSource = org.deeplearning4j.spark.data.loader.RemoteFileSource
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

Namespace org.deeplearning4j.spark.iterator


	<Serializable>
	Public Class PathSparkDataSetIterator
		Inherits BaseDataSetIterator(Of String)

		Public Const BUFFER_SIZE As Integer = 4194304 '4 MB
		Private fileSystem As FileSystem
		Private dataSetLoader As DataSetLoader
		Private hadoopConfig As Broadcast(Of SerializableHadoopConfig)

		Public Sub New(ByVal iter As IEnumerator(Of String), ByVal dataSetLoader As DataSetLoader, ByVal hadoopConfig As Broadcast(Of SerializableHadoopConfig))
			Me.dataSetStreams = Nothing
			Me.iter = iter
			Me.dataSetLoader = dataSetLoader
			Me.hadoopConfig = hadoopConfig
		End Sub

		Public Sub New(ByVal dataSetStreams As ICollection(Of String), ByVal dataSetLoader As DataSetLoader, ByVal hadoopConfig As Broadcast(Of SerializableHadoopConfig))
			Me.dataSetStreams = dataSetStreams
			iter = dataSetStreams.GetEnumerator()
			Me.dataSetLoader = dataSetLoader
			Me.hadoopConfig = hadoopConfig
		End Sub

		Public Overrides Function [next]() As DataSet
			Dim ds As DataSet
			If preloadedDataSet IsNot Nothing Then
				ds = preloadedDataSet
				preloadedDataSet = Nothing
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ds = load(iter.next())
			End If

			totalOutcomes_Conflict = If(ds.Labels Is Nothing, 0, CInt(ds.Labels.size(1))) 'May be null for layerwise pretraining
			inputColumns_Conflict = CInt(ds.Features.size(1))
			batch_Conflict = ds.numExamples()

			If preprocessor_Conflict IsNot Nothing Then
				preprocessor_Conflict.preProcess(ds)
			End If
			Return ds
		End Function

		Protected Friend Overridable Overloads Function load(ByVal path As String) As DataSet
			SyncLock Me
				If fileSystem Is Nothing Then
					Try
						Dim c As Configuration = If(hadoopConfig Is Nothing, DefaultHadoopConfig.get(), hadoopConfig.getValue().getConfiguration())
						fileSystem = FileSystem.get(New URI(path), c)
					Catch e As Exception
						Throw New Exception(e)
					End Try
				End If
				cursor += 1
				Try
					Return dataSetLoader.load(New RemoteFileSource(path, fileSystem, BUFFER_SIZE))
				Catch e As Exception
					Throw New Exception("Error loading DataSet at path " & path & " - DataSet may be corrupt or invalid." & " Spark DataSets can be validated using org.deeplearning4j.spark.util.data.SparkDataValidation", e)
				End Try
			End SyncLock
		End Function
	End Class

End Namespace