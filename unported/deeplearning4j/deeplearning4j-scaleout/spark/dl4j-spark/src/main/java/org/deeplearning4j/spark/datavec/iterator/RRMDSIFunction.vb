Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports [Function] = org.apache.spark.api.java.function.Function
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports Writable = org.datavec.api.writable.Writable
Imports RecordReaderMultiDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderMultiDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class RRMDSIFunction implements org.apache.spark.api.java.function.@Function<DataVecRecords, org.nd4j.linalg.dataset.api.MultiDataSet>
	Public Class RRMDSIFunction
		Implements [Function](Of DataVecRecords, MultiDataSet)

		Private iterator As RecordReaderMultiDataSetIterator

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet call(DataVecRecords records) throws Exception
		Public Overrides Function [call](ByVal records As DataVecRecords) As MultiDataSet



			Dim nextRRVals As IDictionary(Of String, IList(Of IList(Of Writable))) = Collections.emptyMap()
			Dim nextSeqRRVals As IDictionary(Of String, IList(Of IList(Of IList(Of Writable)))) = Collections.emptyMap()

			If records.getRecords() IsNot Nothing AndAlso Not records.getRecords().isEmpty() Then
				nextRRVals = New Dictionary(Of String, IList(Of IList(Of Writable)))()

				Dim m As IDictionary(Of String, RecordReader) = iterator.getRecordReaders()
				For Each e As KeyValuePair(Of String, RecordReader) In m.SetOfKeyValuePairs()
					Dim dr As SparkSourceDummyReader = CType(e.Value, SparkSourceDummyReader)
					Dim idx As Integer = dr.getReaderIdx()
					nextRRVals(e.Key) = Collections.singletonList(records.getRecords().get(idx))
				Next e

			End If
			If records.getSeqRecords() IsNot Nothing AndAlso Not records.getSeqRecords().isEmpty() Then
				nextSeqRRVals = New Dictionary(Of String, IList(Of IList(Of IList(Of Writable))))()

				Dim m As IDictionary(Of String, SequenceRecordReader) = iterator.getSequenceRecordReaders()
				For Each e As KeyValuePair(Of String, SequenceRecordReader) In m.SetOfKeyValuePairs()
					Dim dr As SparkSourceDummySeqReader = CType(e.Value, SparkSourceDummySeqReader)
					Dim idx As Integer = dr.getReaderIdx()
					nextSeqRRVals(e.Key) = Collections.singletonList(records.getSeqRecords().get(idx))
				Next e
			End If


			Dim mds As MultiDataSet = iterator.nextMultiDataSet(nextRRVals, Nothing, nextSeqRRVals, Nothing)
			Nd4j.Executioner.commit()

			Return mds
		End Function
	End Class

End Namespace