Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FSDataOutputStream = org.apache.hadoop.fs.FSDataOutputStream
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports org.nd4j.common.primitives

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


	Public Class BatchAndExportDataSetsFunction
		Implements Function2(Of Integer, IEnumerator(Of DataSet), IEnumerator(Of String))

		Private ReadOnly minibatchSize As Integer
		Private ReadOnly exportBaseDirectory As String
		Private ReadOnly jvmuid As String
		Private ReadOnly conf As Broadcast(Of SerializableHadoopConfig)

		''' <param name="minibatchSize">       Minibatch size to combine examples to (if necessary) </param>
		''' <param name="exportBaseDirectory"> Base directory for exporting </param>
		Public Sub New(ByVal minibatchSize As Integer, ByVal exportBaseDirectory As String)
			Me.New(minibatchSize, exportBaseDirectory, Nothing)
		End Sub

		''' <param name="minibatchSize">       Minibatch size to combine examples to (if necessary) </param>
		''' <param name="exportBaseDirectory"> Base directory for exporting </param>
		''' <param name="configuration">       Hadoop Configuration </param>
		Public Sub New(ByVal minibatchSize As Integer, ByVal exportBaseDirectory As String, ByVal configuration As Broadcast(Of SerializableHadoopConfig))
			Me.minibatchSize = minibatchSize
			Me.exportBaseDirectory = exportBaseDirectory
			Dim fullUID As String = UIDProvider.JVMUID
			Me.jvmuid = (If(fullUID.Length <= 8, fullUID, fullUID.Substring(0, 8)))
			Me.conf = configuration
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Iterator<String> call(System.Nullable<Integer> partitionIdx, Iterator<org.nd4j.linalg.dataset.DataSet> iterator) throws Exception
		Public Overrides Function [call](ByVal partitionIdx As Integer?, ByVal iterator As IEnumerator(Of DataSet)) As IEnumerator(Of String)

			Dim outputPaths As IList(Of String) = New List(Of String)()
			Dim tempList As New LinkedList(Of DataSet)()

			Dim count As Integer = 0
			Do While iterator.MoveNext()
				Dim [next] As DataSet = iterator.Current
				If [next].numExamples() = minibatchSize Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: outputPaths.add(export(next, partitionIdx, count++));
					outputPaths.Add(export([next], partitionIdx, count))
						count += 1
					Continue Do
				End If
				'DataSet must be either smaller or larger than minibatch size...
				tempList.AddLast([next])
				Dim countAndPaths As Pair(Of Integer, IList(Of String)) = processList(tempList, partitionIdx, count, False)
				If countAndPaths.Second IsNot Nothing AndAlso countAndPaths.Second.Count > 0 Then
					CType(outputPaths, List(Of String)).AddRange(countAndPaths.Second)
				End If
				count = countAndPaths.First
			Loop

			'We might have some left-over examples...
			Dim countAndPaths As Pair(Of Integer, IList(Of String)) = processList(tempList, partitionIdx, count, True)
			If countAndPaths.Second IsNot Nothing AndAlso countAndPaths.Second.Count > 0 Then
				CType(outputPaths, List(Of String)).AddRange(countAndPaths.Second)
			End If

			Return outputPaths.GetEnumerator()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.nd4j.common.primitives.Pair<Integer, List<String>> processList(LinkedList<org.nd4j.linalg.dataset.DataSet> tempList, int partitionIdx, int countBefore, boolean finalExport) throws Exception
		Private Function processList(ByVal tempList As LinkedList(Of DataSet), ByVal partitionIdx As Integer, ByVal countBefore As Integer, ByVal finalExport As Boolean) As Pair(Of Integer, IList(Of String))
			'Go through the list. If we have enough examples: remove the DataSet objects, merge and export them. Otherwise: do nothing
			Dim numExamples As Integer = 0
			For Each ds As DataSet In tempList
				numExamples += ds.numExamples()
			Next ds

			If tempList.Count = 0 OrElse (numExamples < minibatchSize AndAlso Not finalExport) Then
				'No op
				Return New Pair(Of Integer, IList(Of String))(countBefore, Enumerable.Empty(Of String)())
			End If

			Dim exportPaths As IList(Of String) = New List(Of String)()

			Dim countAfter As Integer = countBefore

			'Batch the required number together
			Dim countSoFar As Integer = 0
			Dim tempToMerge As IList(Of DataSet) = New List(Of DataSet)()
			Do While tempList.Count > 0 AndAlso countSoFar <> minibatchSize
				Dim [next] As DataSet = tempList.RemoveFirst()
				If countSoFar + [next].numExamples() <= minibatchSize Then
					'Add the entire DataSet object
					tempToMerge.Add([next])
					countSoFar += [next].numExamples()
				Else
					'Split the DataSet
					Dim examples As IList(Of DataSet) = [next].asList()
					For Each ds As DataSet In examples
						tempList.AddFirst(ds)
					Next ds
				End If
			Loop
			'At this point: we should have the required number of examples in tempToMerge (unless it's a final export)
			Dim toExport As DataSet = DataSet.merge(tempToMerge)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: exportPaths.add(export(toExport, partitionIdx, countAfter++));
			exportPaths.Add(export(toExport, partitionIdx, countAfter))
				countAfter += 1

			Return New Pair(Of Integer, IList(Of String))(countAfter, exportPaths)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private String export(org.nd4j.linalg.dataset.DataSet dataSet, int partitionIdx, int outputCount) throws Exception
		Private Function export(ByVal dataSet As DataSet, ByVal partitionIdx As Integer, ByVal outputCount As Integer) As String
			Dim filename As String = "dataset_" & partitionIdx & jvmuid & "_" & outputCount & ".bin"

			Dim uri As New URI(exportBaseDirectory & (If(exportBaseDirectory.EndsWith("/", StringComparison.Ordinal) OrElse exportBaseDirectory.EndsWith("\", StringComparison.Ordinal), "", "/")) & filename)

			Dim c As Configuration = If(conf Is Nothing, DefaultHadoopConfig.get(), conf.getValue().getConfiguration())

			Dim file As FileSystem = FileSystem.get(uri, c)
			Using [out] As org.apache.hadoop.fs.FSDataOutputStream = file.create(New org.apache.hadoop.fs.Path(uri))
				dataSet.save([out])
			End Using

			Return uri.ToString()
		End Function
	End Class

End Namespace