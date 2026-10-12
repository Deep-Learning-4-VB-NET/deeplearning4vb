Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FSDataOutputStream = org.apache.hadoop.fs.FSDataOutputStream
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CollectionRecordReader = org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports StringSplit = org.datavec.api.split.StringSplit
Imports Writable = org.datavec.api.writable.Writable
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
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

Namespace org.deeplearning4j.spark.datavec.export


	Public Class StringToDataSetExportFunction
		Implements VoidFunction(Of IEnumerator(Of String))

		Private ReadOnly conf As Broadcast(Of SerializableHadoopConfig)

		Private ReadOnly outputDir As URI
		Private ReadOnly recordReader As RecordReader
		Private ReadOnly batchSize As Integer
		Private ReadOnly regression As Boolean
		Private ReadOnly labelIndex As Integer
		Private ReadOnly numPossibleLabels As Integer
		Private uid As String = Nothing

		Private outputCount As Integer

		Public Sub New(ByVal outputDir As URI, ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal regression As Boolean, ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer)
			Me.New(outputDir, recordReader, batchSize, regression, labelIndex, numPossibleLabels, Nothing)
		End Sub

		Public Sub New(ByVal outputDir As URI, ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal regression As Boolean, ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal configuration As Broadcast(Of SerializableHadoopConfig))
			Me.outputDir = outputDir
			Me.recordReader = recordReader
			Me.batchSize = batchSize
			Me.regression = regression
			Me.labelIndex = labelIndex
			Me.numPossibleLabels = numPossibleLabels
			Me.conf = configuration
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(java.util.Iterator<String> stringIterator) throws Exception
		Public Overrides Sub [call](ByVal stringIterator As IEnumerator(Of String))
			Dim jvmuid As String = UIDProvider.JVMUID
			uid = Thread.CurrentThread.getId() & jvmuid.Substring(0, Math.Min(8, jvmuid.Length))

			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(batchSize)

			Do While stringIterator.MoveNext()
				Dim [next] As String = stringIterator.Current
				recordReader.initialize(New org.datavec.api.Split.StringSplit([next]))
				list.Add(recordReader.next())

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				processBatchIfRequired(list, Not stringIterator.hasNext())
			Loop
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void processBatchIfRequired(java.util.List<java.util.List<org.datavec.api.writable.Writable>> list, boolean finalRecord) throws Exception
		Private Sub processBatchIfRequired(ByVal list As IList(Of IList(Of Writable)), ByVal finalRecord As Boolean)
			If list.Count = 0 Then
				Return
			End If
			If list.Count < batchSize AndAlso Not finalRecord Then
				Return
			End If

			Dim rr As RecordReader = New CollectionRecordReader(list)
			Dim iter As New RecordReaderDataSetIterator(rr, Nothing, batchSize, labelIndex, labelIndex, numPossibleLabels, -1, regression)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String filename = "dataset_" + uid + "_" + (outputCount++) + ".bin";
			Dim filename As String = "dataset_" & uid & "_" & (outputCount) & ".bin"
				outputCount += 1

			Dim uri As New URI(outputDir.getPath() & "/" & filename)
			Dim c As Configuration = If(conf Is Nothing, DefaultHadoopConfig.get(), conf.getValue().getConfiguration())
			Dim file As FileSystem = FileSystem.get(uri, c)
			Using [out] As org.apache.hadoop.fs.FSDataOutputStream = file.create(New org.apache.hadoop.fs.Path(uri))
				ds.save([out])
			End Using

			list.Clear()
		End Sub
	End Class

End Namespace