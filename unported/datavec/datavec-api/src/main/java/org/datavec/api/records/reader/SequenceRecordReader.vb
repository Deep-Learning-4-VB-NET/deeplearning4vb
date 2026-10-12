Imports System.Collections.Generic
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
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

Namespace org.datavec.api.records.reader


	Public Interface SequenceRecordReader
		Inherits RecordReader

		''' <summary>
		''' Returns a sequence record.
		''' </summary>
		''' <returns> a sequence of records </returns>
		Function sequenceRecord() As IList(Of IList(Of Writable))

		''' <summary>
		''' Load a sequence record from the given DataInputStream
		''' Unlike <seealso cref="next()"/> the internal state of the RecordReader is not modified
		''' Implementations of this method should not close the DataInputStream
		''' </summary>
		''' <exception cref="IOException"> if error occurs during reading from the input stream </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException;
		Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable))

		''' <summary>
		''' Similar to <seealso cref="sequenceRecord()"/>, but returns a <seealso cref="Record"/> object, that may include metadata such as the source
		''' of the data
		''' </summary>
		''' <returns> next sequence record </returns>
		Function nextSequence() As SequenceRecord

		''' <summary>
		''' Load a single sequence record from the given <seealso cref="RecordMetaData"/> instance<br>
		''' Note: that for data that isn't splittable (i.e., text data that needs to be scanned/split), it is more efficient to
		''' load multiple records at once using <seealso cref="loadSequenceFromMetaData(List)"/>
		''' </summary>
		''' <param name="recordMetaData"> Metadata for the sequence record that we want to load from </param>
		''' <returns> Single sequence record for the given RecordMetaData instance </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException;
		Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord

		''' <summary>
		''' Load multiple sequence records from the given a list of <seealso cref="RecordMetaData"/> instances<br>
		''' </summary>
		''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
		''' <returns> Multiple sequence record for the given RecordMetaData instances </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException;
		Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord)
	End Interface

End Namespace