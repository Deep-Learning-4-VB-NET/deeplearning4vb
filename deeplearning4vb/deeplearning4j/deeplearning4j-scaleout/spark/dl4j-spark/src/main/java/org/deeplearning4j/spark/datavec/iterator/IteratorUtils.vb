Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports [Function] = org.apache.spark.api.java.function.Function
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports Writable = org.datavec.api.writable.Writable
Imports RecordReaderMultiDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderMultiDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.datavec.iterator


	Public Class IteratorUtils

		''' <summary>
		''' Apply a single reader <seealso cref="RecordReaderMultiDataSetIterator"/> to a {@code JavaRDD<List<Writable>>}.
		''' <b>NOTE</b>: The RecordReaderMultiDataSetIterator <it>must</it> use <seealso cref="SparkSourceDummyReader"/> in place of
		''' "real" RecordReader instances
		''' </summary>
		''' <param name="rdd">      RDD with writables </param>
		''' <param name="iterator"> RecordReaderMultiDataSetIterator with <seealso cref="SparkSourceDummyReader"/> readers </param>
		Public Shared Function mapRRMDSI(ByVal rdd As JavaRDD(Of IList(Of Writable)), ByVal iterator As RecordReaderMultiDataSetIterator) As JavaRDD(Of MultiDataSet)
			checkIterator(iterator, 1, 0)
			Return mapRRMDSIRecords(rdd.map(New FunctionAnonymousInnerClass()), iterator)
		End Function

		Private Class FunctionAnonymousInnerClass
			Inherits [Function](Of IList(Of Writable), DataVecRecords)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public DataVecRecords call(List<org.datavec.api.writable.Writable> v1) throws Exception
			Public Overrides Function [call](ByVal v1 As IList(Of Writable)) As DataVecRecords
				Return New DataVecRecords(Collections.singletonList(v1), Nothing)
			End Function
		End Class

		''' <summary>
		''' Apply a single sequence reader <seealso cref="RecordReaderMultiDataSetIterator"/> to sequence data, in the form of
		''' {@code JavaRDD<List<List<Writable>>>}.
		''' <b>NOTE</b>: The RecordReaderMultiDataSetIterator <it>must</it> use <seealso cref="SparkSourceDummySeqReader"/> in place of
		''' "real" SequenceRecordReader instances
		''' </summary>
		''' <param name="rdd">      RDD with writables </param>
		''' <param name="iterator"> RecordReaderMultiDataSetIterator with <seealso cref="SparkSourceDummySeqReader"/> sequence readers </param>
		Public Shared Function mapRRMDSISeq(ByVal rdd As JavaRDD(Of IList(Of IList(Of Writable))), ByVal iterator As RecordReaderMultiDataSetIterator) As JavaRDD(Of MultiDataSet)
			checkIterator(iterator, 0, 1)
			Return mapRRMDSIRecords(rdd.map(New FunctionAnonymousInnerClass2()), iterator)
		End Function

		Private Class FunctionAnonymousInnerClass2
			Inherits [Function](Of IList(Of IList(Of Writable)), DataVecRecords)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public DataVecRecords call(List<List<org.datavec.api.writable.Writable>> v1) throws Exception
			Public Overrides Function [call](ByVal v1 As IList(Of IList(Of Writable))) As DataVecRecords
				Return New DataVecRecords(Nothing, Collections.singletonList(v1))
			End Function
		End Class

		''' <summary>
		''' Apply to an arbitrary mix of non-sequence and sequence data, in the form of {@code JavaRDD<List<Writable>>}
		''' and {@code JavaRDD<List<List<Writable>>>}.<br>
		''' Note: this method performs a join by key. To perform this, we require that each record (and every step of
		''' sequence records) contain the same key value (could be any Writable).<br>
		''' <b>NOTE</b>: The RecordReaderMultiDataSetIterator <it>must</it> use <seealso cref="SparkSourceDummyReader"/> and
		''' <seealso cref="SparkSourceDummySeqReader"/> instances in place of "real" RecordReader and SequenceRecordReader instances
		''' </summary>
		''' <param name="rdds">      RDD with non-sequence data. May be null. </param>
		''' <param name="seqRdds">   RDDs with sequence data. May be null. </param>
		''' <param name="rddsKeyColumns"> Column indices for the keys in the (non-sequence) RDDs data </param>
		''' <param name="seqRddsKeyColumns"> Column indices for the keys in the sequence RDDs data </param>
		''' <param name="filterMissing"> If true: filter out any records that don't have matching keys in all RDDs </param>
		''' <param name="iterator"> RecordReaderMultiDataSetIterator with <seealso cref="SparkSourceDummyReader"/> and <seealso cref="SparkSourceDummySeqReader"/>readers </param>
		Public Shared Function mapRRMDSI(ByVal rdds As IList(Of JavaRDD(Of IList(Of Writable))), ByVal seqRdds As IList(Of JavaRDD(Of IList(Of IList(Of Writable)))), ByVal rddsKeyColumns() As Integer, ByVal seqRddsKeyColumns() As Integer, ByVal filterMissing As Boolean, ByVal iterator As RecordReaderMultiDataSetIterator) As JavaRDD(Of MultiDataSet)
			checkIterator(iterator, (If(rdds Is Nothing, 0, rdds.Count)), (If(seqRdds Is Nothing, 0, seqRdds.Count)))
			assertNullOrSameLength(rdds, rddsKeyColumns, False)
			assertNullOrSameLength(seqRdds, seqRddsKeyColumns, True)
			If (rdds Is Nothing OrElse rdds.Count = 0) AndAlso (seqRdds Is Nothing OrElse seqRdds.Count = 0) Then
				Throw New System.ArgumentException()
			End If

			Dim allPairs As JavaPairRDD(Of Writable, DataVecRecord) = Nothing
			If rdds IsNot Nothing Then
				For i As Integer = 0 To rdds.Count - 1
					Dim rdd As JavaRDD(Of IList(Of Writable)) = rdds(i)
					Dim currPairs As JavaPairRDD(Of Writable, DataVecRecord) = rdd.mapToPair(New MapToPairFn(i, rddsKeyColumns(i)))
					If allPairs Is Nothing Then
						allPairs = currPairs
					Else
						allPairs = allPairs.union(currPairs)
					End If
				Next i
			End If


			If seqRdds IsNot Nothing Then
				For i As Integer = 0 To seqRdds.Count - 1
					Dim rdd As JavaRDD(Of IList(Of IList(Of Writable))) = seqRdds(i)
					Dim currPairs As JavaPairRDD(Of Writable, DataVecRecord) = rdd.mapToPair(New MapToPairSeqFn(i, seqRddsKeyColumns(i)))
					If allPairs Is Nothing Then
						allPairs = currPairs
					Else
						allPairs = allPairs.union(currPairs)
					End If
				Next i
			End If

			Dim expNumRec As Integer = (If(rddsKeyColumns Is Nothing, 0, rddsKeyColumns.Length))
			Dim expNumSeqRec As Integer = (If(seqRddsKeyColumns Is Nothing, 0, seqRddsKeyColumns.Length))

			'Finally: group by key, filter (if necessary), convert
			Dim grouped As JavaPairRDD(Of Writable, IEnumerable(Of DataVecRecord)) = allPairs.groupByKey()
			If filterMissing Then
				'TODO
				grouped = grouped.filter(New FilterMissingFn(expNumRec, expNumSeqRec))
			End If

			Dim combined As JavaRDD(Of DataVecRecords) = grouped.map(New CombineFunction(expNumRec, expNumSeqRec))
			Return mapRRMDSIRecords(combined, iterator)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class MapToPairFn implements org.apache.spark.api.java.function.PairFunction<List<org.datavec.api.writable.Writable>, org.datavec.api.writable.Writable, DataVecRecord>
		Private Class MapToPairFn
			Implements PairFunction(Of IList(Of Writable), Writable, DataVecRecord)

			Friend readerIdx As Integer
			Friend keyIndex As Integer
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<org.datavec.api.writable.Writable, DataVecRecord> call(List<org.datavec.api.writable.Writable> writables) throws Exception
			Public Overrides Function [call](ByVal writables As IList(Of Writable)) As Tuple2(Of Writable, DataVecRecord)
				Return New Tuple2(Of Writable, DataVecRecord)(writables(keyIndex), New DataVecRecord(readerIdx, writables, Nothing))
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class MapToPairSeqFn implements org.apache.spark.api.java.function.PairFunction<List<List<org.datavec.api.writable.Writable>>, org.datavec.api.writable.Writable, DataVecRecord>
		Private Class MapToPairSeqFn
			Implements PairFunction(Of IList(Of IList(Of Writable)), Writable, DataVecRecord)

			Friend readerIdx As Integer
			Friend keyIndex As Integer
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<org.datavec.api.writable.Writable, DataVecRecord> call(List<List<org.datavec.api.writable.Writable>> seq) throws Exception
			Public Overrides Function [call](ByVal seq As IList(Of IList(Of Writable))) As Tuple2(Of Writable, DataVecRecord)
				If seq.Count = 0 Then
					Throw New System.InvalidOperationException("Sequence of length 0 encountered")
				End If
				Return New Tuple2(Of Writable, DataVecRecord)(seq(0)(keyIndex), New DataVecRecord(readerIdx, Nothing, seq))
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class CombineFunction implements org.apache.spark.api.java.function.@Function<scala.Tuple2<org.datavec.api.writable.Writable, Iterable<DataVecRecord>>, DataVecRecords>
		Private Class CombineFunction
			Implements [Function](Of Tuple2(Of Writable, IEnumerable(Of DataVecRecord)), DataVecRecords)

			Friend expNumRecords As Integer
			Friend expNumSeqRecords As Integer
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public DataVecRecords call(scala.Tuple2<org.datavec.api.writable.Writable, Iterable<DataVecRecord>> all) throws Exception
			Public Overrides Function [call](ByVal all As Tuple2(Of Writable, IEnumerable(Of DataVecRecord))) As DataVecRecords

				Dim allRecordsArr() As IList(Of Writable) = Nothing
				If expNumRecords > 0 Then
					allRecordsArr = CType(New System.Collections.IList(expNumRecords - 1){}, IList(Of Writable)()) 'Array.newInstance(List.class, expNumRecords);
				End If
				Dim allRecordsSeqArr() As IList(Of IList(Of Writable)) = Nothing
				If expNumSeqRecords > 0 Then
					allRecordsSeqArr = CType(New System.Collections.IList(expNumSeqRecords - 1){}, IList(Of IList(Of Writable))())
				End If

				For Each rec As DataVecRecord In all._2()
					If rec.getRecord() IsNot Nothing Then
						allRecordsArr(rec.getReaderIdx()) = rec.getRecord()
					Else
						allRecordsSeqArr(rec.getReaderIdx()) = rec.getSeqRecord()
					End If
				Next rec

				If allRecordsArr IsNot Nothing Then
					For i As Integer = 0 To allRecordsArr.Length - 1
						If allRecordsArr(i) Is Nothing Then
							Throw New System.InvalidOperationException("Encountered null records for input index " & i)
						End If
					Next i
				End If

				If allRecordsSeqArr IsNot Nothing Then
					For i As Integer = 0 To allRecordsSeqArr.Length - 1
						If allRecordsSeqArr(i) Is Nothing Then
							Throw New System.InvalidOperationException("Encountered null sequence records for input index " & i)
						End If
					Next i
				End If

				Dim r As IList(Of IList(Of Writable)) = (If(allRecordsArr Is Nothing, Nothing, java.util.Arrays.asList(allRecordsArr)))
				Dim sr As IList(Of IList(Of IList(Of Writable))) = (If(allRecordsSeqArr Is Nothing, Nothing, java.util.Arrays.asList(allRecordsSeqArr)))
				Return New DataVecRecords(r, sr)
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class FilterMissingFn implements org.apache.spark.api.java.function.@Function<scala.Tuple2<org.datavec.api.writable.Writable, Iterable<DataVecRecord>>, Boolean>
		Private Class FilterMissingFn
			Implements [Function](Of Tuple2(Of Writable, IEnumerable(Of DataVecRecord)), Boolean)

			Friend ReadOnly expNumRec As Integer
			Friend ReadOnly expNumSeqRec As Integer
			<NonSerialized>
			Friend recIdxs As ThreadLocal(Of ISet(Of Integer))
			<NonSerialized>
			Friend seqRecIdxs As ThreadLocal(Of ISet(Of Integer))

			Friend Sub New(ByVal expNumRec As Integer, ByVal expNumSeqRec As Integer)
				Me.expNumRec = expNumRec
				Me.expNumSeqRec = expNumSeqRec
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public System.Nullable<Boolean> call(scala.Tuple2<org.datavec.api.writable.Writable, Iterable<DataVecRecord>> iter) throws Exception
			Public Overrides Function [call](ByVal iter As Tuple2(Of Writable, IEnumerable(Of DataVecRecord))) As Boolean?
				If recIdxs Is Nothing Then
					recIdxs = New ThreadLocal(Of ISet(Of Integer))()
				End If
				If seqRecIdxs Is Nothing Then
					seqRecIdxs = New ThreadLocal(Of ISet(Of Integer))()
				End If

				Dim ri As ISet(Of Integer) = recIdxs.get()
				If ri Is Nothing Then
					ri = New HashSet(Of Integer)()
					recIdxs.set(ri)
				End If
				Dim sri As ISet(Of Integer) = seqRecIdxs.get()
				If sri Is Nothing Then
					sri = New HashSet(Of Integer)()
					seqRecIdxs.set(sri)
				End If

				For Each r As DataVecRecord In iter._2()
					If r.getRecord() IsNot Nothing Then
						ri.Add(r.getReaderIdx())
					ElseIf r.getSeqRecord() IsNot Nothing Then
						sri.Add(r.getReaderIdx())
					End If
				Next r

				Dim count As Integer = ri.Count
				Dim count2 As Integer = sri.Count

				ri.Clear()
				sri.Clear()

				Return (count = expNumRec) AndAlso (count2 = expNumSeqRec)
			End Function
		End Class


		Private Shared Sub assertNullOrSameLength(Of T1)(ByVal list As IList(Of T1), ByVal arr() As Integer, ByVal isSeq As Boolean)
			If list IsNot Nothing AndAlso arr Is Nothing Then
				Throw New System.InvalidOperationException()
			End If
			If list Is Nothing AndAlso (arr IsNot Nothing AndAlso arr.Length > 0) Then
				Throw New System.InvalidOperationException()
			End If
			If list IsNot Nothing AndAlso list.Count <> arr.Length Then
				Throw New System.InvalidOperationException()
			End If
		End Sub


		Public Shared Function mapRRMDSIRecords(ByVal rdd As JavaRDD(Of DataVecRecords), ByVal iterator As RecordReaderMultiDataSetIterator) As JavaRDD(Of MultiDataSet)
			Return rdd.map(New RRMDSIFunction(iterator))
		End Function

		Private Shared Sub checkIterator(ByVal iterator As RecordReaderMultiDataSetIterator, ByVal maxReaders As Integer, ByVal maxSeqReaders As Integer)


			Dim rrs As IDictionary(Of String, RecordReader) = iterator.getRecordReaders()
			Dim seqRRs As IDictionary(Of String, SequenceRecordReader) = iterator.getSequenceRecordReaders()


			If rrs IsNot Nothing AndAlso rrs.Count > maxReaders Then
				Throw New System.InvalidOperationException("Invalid state: iterator has " & rrs.Count & " readers but " & maxReaders & " RDDs of List<Writable> were provided")
			End If
			If seqRRs IsNot Nothing AndAlso seqRRs.Count > maxSeqReaders Then
				Throw New System.InvalidOperationException("Invalid state: iterator has " & seqRRs.Count & " sequence readers but " & maxSeqReaders & " RDDs of sequences - List<List<Writable>> were provided")
			End If

			If rrs IsNot Nothing AndAlso rrs.Count > 0 Then
				For Each e As KeyValuePair(Of String, RecordReader) In rrs.SetOfKeyValuePairs()
					If Not (TypeOf e.Value Is SparkSourceDummyReader) Then
						Throw New System.InvalidOperationException("Invalid state: expected SparkSourceDummyReader for reader with name """ & e.Key & """, but got reader type: " & e.Key.GetType())
					End If
				Next e
			End If

			If seqRRs IsNot Nothing AndAlso seqRRs.Count > 0 Then
				For Each e As KeyValuePair(Of String, SequenceRecordReader) In seqRRs.SetOfKeyValuePairs()
					If Not (TypeOf e.Value Is SparkSourceDummySeqReader) Then
						Throw New System.InvalidOperationException("Invalid state: expected SparkSourceDummySeqReader for sequence reader with name """ & e.Key & """, but got reader type: " & e.Key.GetType())
					End If
				Next e
			End If
		End Sub
	End Class

End Namespace