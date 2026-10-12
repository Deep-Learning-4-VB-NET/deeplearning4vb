Imports System
Imports System.Collections.Generic
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports RemoteFileSource = org.deeplearning4j.spark.data.loader.RemoteFileSource
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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
	Public Class PathSparkMultiDataSetIterator
		Implements MultiDataSetIterator

		Public Const BUFFER_SIZE As Integer = 4194304 '4 MB

		Private ReadOnly dataSetStreams As ICollection(Of String)
'JAVA TO VB CONVERTER NOTE: The field preprocessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preprocessor_Conflict As MultiDataSetPreProcessor
		Private iter As IEnumerator(Of String)
		Private fileSystem As FileSystem
		Private ReadOnly loader As MultiDataSetLoader
		Private ReadOnly hadoopConfig As Broadcast(Of SerializableHadoopConfig)

		Public Sub New(ByVal iter As IEnumerator(Of String), ByVal loader As MultiDataSetLoader, ByVal hadoopConfig As Broadcast(Of SerializableHadoopConfig))
			Me.dataSetStreams = Nothing
			Me.iter = iter
			Me.loader = loader
			Me.hadoopConfig = hadoopConfig
		End Sub

		Public Sub New(ByVal dataSetStreams As ICollection(Of String), ByVal loader As MultiDataSetLoader, ByVal hadoopConfig As Broadcast(Of SerializableHadoopConfig))
			Me.dataSetStreams = dataSetStreams
			iter = dataSetStreams.GetEnumerator()
			Me.loader = loader
			Me.hadoopConfig = hadoopConfig
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return [next]()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return dataSetStreams IsNot Nothing
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			If dataSetStreams Is Nothing Then
				Throw New System.InvalidOperationException("Cannot reset iterator constructed with an iterator")
			End If
			iter = dataSetStreams.GetEnumerator()
		End Sub

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preprocessor_Conflict = preProcessor
			End Set
			Get
				Return preprocessor_Conflict
			End Get
		End Property


		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function

		Public Overrides Function [next]() As MultiDataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As MultiDataSet = load(iter.next())

			If preprocessor_Conflict IsNot Nothing Then
				preprocessor_Conflict.preProcess(ds)
			End If
			Return ds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub


		Private Function load(ByVal path As String) As MultiDataSet
			SyncLock Me
				If fileSystem Is Nothing Then
					Try
						Dim c As Configuration = If(hadoopConfig Is Nothing, DefaultHadoopConfig.get(), hadoopConfig.getValue().getConfiguration())
						fileSystem = FileSystem.get(New URI(path), c)
					Catch e As Exception
						Throw New Exception(e)
					End Try
				End If
        
				Try
					Return loader.load(New RemoteFileSource(path, fileSystem, BUFFER_SIZE))
				Catch e As IOException
					Throw New Exception("Error loading MultiDataSet at path " & path & " - DataSet may be corrupt or invalid." & " Spark MultiDataSets can be validated using org.deeplearning4j.spark.util.data.SparkDataValidation", e)
				End Try
			End SyncLock
		End Function
	End Class

End Namespace