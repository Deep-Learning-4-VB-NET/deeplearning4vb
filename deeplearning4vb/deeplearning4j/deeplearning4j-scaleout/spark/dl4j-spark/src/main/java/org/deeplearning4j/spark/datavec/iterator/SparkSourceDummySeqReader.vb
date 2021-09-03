Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports Writable = org.datavec.api.writable.Writable

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
'ORIGINAL LINE: @Data public class SparkSourceDummySeqReader extends SparkSourceDummyReader implements org.datavec.api.records.reader.SequenceRecordReader
	<Serializable>
	Public Class SparkSourceDummySeqReader
		Inherits SparkSourceDummyReader
		Implements SequenceRecordReader

		''' <param name="readerIdx"> Index of the reader, in terms of the sequence RDD that it should use. For a single sequence RDD
		'''                  as input, this is always 0; for 2 sequence RDDs used as input, this would be 0 or 1, depending
		'''                  on whether it should pull values from the first or second sequence RDD. Note that the indexing
		'''                  for sequence RDDs doesn't depend on the presence of non-sequence RDDs - they are indexed separately. </param>
		Public Sub New(ByVal readerIdx As Integer)
			MyBase.New(readerIdx)
		End Sub

		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> list) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal list As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace