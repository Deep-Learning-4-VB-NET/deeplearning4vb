Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter
Imports SequenceRecordWriter = org.datavec.api.records.writer.SequenceRecordWriter

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

Namespace org.datavec.api.records.converter


	Public Class RecordReaderConverter

		Private Sub New()
		End Sub

		''' <summary>
		''' Write all values from the specified record reader to the specified record writer.
		''' Closes the record writer on completion
		''' </summary>
		''' <param name="reader"> Record reader (source of data) </param>
		''' <param name="writer"> Record writer (location to write data) </param>
		''' <exception cref="IOException"> If underlying reader/writer throws an exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void convert(org.datavec.api.records.reader.RecordReader reader, org.datavec.api.records.writer.RecordWriter writer) throws java.io.IOException
		Public Shared Sub convert(ByVal reader As RecordReader, ByVal writer As RecordWriter)
			convert(reader, writer, True)
		End Sub

		''' <summary>
		''' Write all values from the specified record reader to the specified record writer.
		''' Optionally, close the record writer on completion
		''' </summary>
		''' <param name="reader"> Record reader (source of data) </param>
		''' <param name="writer"> Record writer (location to write data) </param>
		''' <param name="closeOnCompletion"> if true: close the record writer once complete, via <seealso cref="RecordWriter.close()"/> </param>
		''' <exception cref="IOException"> If underlying reader/writer throws an exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void convert(org.datavec.api.records.reader.RecordReader reader, org.datavec.api.records.writer.RecordWriter writer, boolean closeOnCompletion) throws java.io.IOException
		Public Shared Sub convert(ByVal reader As RecordReader, ByVal writer As RecordWriter, ByVal closeOnCompletion As Boolean)

			If Not reader.hasNext() Then
				Throw New System.NotSupportedException("Cannot convert RecordReader: reader has no next element")
			End If

			Do While reader.hasNext()
				writer.write(reader.next())
			Loop

			If closeOnCompletion Then
				writer.Dispose()
			End If
		End Sub

		''' <summary>
		''' Write all sequences from the specified sequence record reader to the specified sequence record writer.
		''' Closes the sequence record writer on completion.
		''' </summary>
		''' <param name="reader"> Sequence record reader (source of data) </param>
		''' <param name="writer"> Sequence record writer (location to write data) </param>
		''' <exception cref="IOException"> If underlying reader/writer throws an exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void convert(org.datavec.api.records.reader.SequenceRecordReader reader, org.datavec.api.records.writer.SequenceRecordWriter writer) throws java.io.IOException
		Public Shared Sub convert(ByVal reader As SequenceRecordReader, ByVal writer As SequenceRecordWriter)
			convert(reader, writer, True)
		End Sub

		''' <summary>
		''' Write all sequences from the specified sequence record reader to the specified sequence record writer.
		''' Closes the sequence record writer on completion.
		''' </summary>
		''' <param name="reader"> Sequence record reader (source of data) </param>
		''' <param name="writer"> Sequence record writer (location to write data) </param>
		''' <param name="closeOnCompletion"> if true: close the record writer once complete, via <seealso cref="SequenceRecordWriter.close()"/> </param>
		''' <exception cref="IOException"> If underlying reader/writer throws an exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void convert(org.datavec.api.records.reader.SequenceRecordReader reader, org.datavec.api.records.writer.SequenceRecordWriter writer, boolean closeOnCompletion) throws java.io.IOException
		Public Shared Sub convert(ByVal reader As SequenceRecordReader, ByVal writer As SequenceRecordWriter, ByVal closeOnCompletion As Boolean)

			If Not reader.hasNext() Then
				Throw New System.NotSupportedException("Cannot convert SequenceRecordReader: reader has no next element")
			End If

			Do While reader.hasNext()
				writer.write(reader.sequenceRecord())
			Loop

			If closeOnCompletion Then
				writer.Dispose()
			End If
		End Sub

	End Class

End Namespace