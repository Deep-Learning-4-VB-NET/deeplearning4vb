Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports Configuration = org.datavec.api.conf.Configuration
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports Partitioner = org.datavec.api.split.partition.Partitioner
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

Namespace org.datavec.api.records.mapper


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public class RecordMapper
	Public Class RecordMapper

		Private recordReader As RecordReader
		Private recordWriter As RecordWriter
		Private inputUrl As org.datavec.api.Split.InputSplit
		Private splitPerReader() As org.datavec.api.Split.InputSplit
		Private readersToConcat() As RecordReader

		Private outputUrl As org.datavec.api.Split.InputSplit
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean callInitRecordReader = true;
		Private callInitRecordReader As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean callInitRecordWriter = true;
		Private callInitRecordWriter As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean callInitPartitioner = true;
		Private callInitPartitioner As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.datavec.api.conf.Configuration configuration = new org.datavec.api.conf.Configuration();
		Private configuration As New Configuration()

		Private configurationsPerReader() As Configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private org.datavec.api.split.partition.Partitioner partitioner = new org.datavec.api.split.partition.NumberOfRecordsPartitioner();
		Private partitioner As org.datavec.api.Split.partition.Partitioner = New org.datavec.api.Split.partition.NumberOfRecordsPartitioner()
		Private batchSize As Integer

		''' <summary>
		''' Copy the <seealso cref="RecordReader"/>
		''' data using the <seealso cref="RecordWriter"/>.
		''' Note that unless batch is supported by
		''' both the <seealso cref="RecordReader"/> and <seealso cref="RecordWriter"/>
		''' then writes will happen one at a time.
		''' You can see if batch is enabled via <seealso cref="RecordReader.batchesSupported()"/>
		''' and <seealso cref="RecordWriter.supportsBatch()"/> respectively. </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void copy() throws Exception
		Public Overridable Sub copy()
			If callInitRecordReader Then
				If recordReader IsNot Nothing Then
					recordReader.initialize(configuration, inputUrl)
				Else
					If readersToConcat Is Nothing OrElse splitPerReader Is Nothing Then
						Throw New System.ArgumentException("No readers or input  splits found.")
					End If

					If readersToConcat.Length <> splitPerReader.Length Then
						Throw New System.ArgumentException("One input split must be specified per record reader")
					End If

					For i As Integer = 0 To readersToConcat.Length - 1
						If readersToConcat(i) Is Nothing Then
							Throw New System.InvalidOperationException("Reader at record " & i & " was null!")
						End If
						If splitPerReader(i) Is Nothing Then
							Throw New System.InvalidOperationException("Split at " & i & " is null!")
						End If
						'allow for, but do not enforce configurations per reader.
						If configurationsPerReader IsNot Nothing Then
							readersToConcat(i).initialize(configurationsPerReader(i), splitPerReader(i))
						Else
							readersToConcat(i).initialize(configuration,splitPerReader(i))
						End If
					Next i
				End If
			End If

			If callInitPartitioner Then
				partitioner.init(configuration, outputUrl)
			End If

			If callInitRecordWriter Then
				recordWriter.initialize(configuration, outputUrl, partitioner)
			End If

			If recordReader IsNot Nothing Then
				write(recordReader,True)
			ElseIf readersToConcat IsNot Nothing Then
				For Each recordReader As RecordReader In readersToConcat
					write(recordReader,False)
				Next recordReader

				'close since we can't do it within the method
				recordWriter.Dispose()
			End If

		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void write(org.datavec.api.records.reader.RecordReader recordReader,boolean closeWriter) throws Exception
		Private Sub write(ByVal recordReader As RecordReader, ByVal closeWriter As Boolean)
			If batchSize > 0 AndAlso recordReader.batchesSupported() AndAlso recordWriter.supportsBatch() Then
				Do While recordReader.hasNext()
					Dim [next] As IList(Of IList(Of Writable)) = recordReader.next(batchSize)
					'ensure we can write a file for either the current or next iterations
					If partitioner.needsNewPartition() Then
						partitioner.currentOutputStream().Flush()
						partitioner.currentOutputStream().Close()
						partitioner.openNewStream()
					End If
					'update records written
					partitioner.updatePartitionInfo(recordWriter.writeBatch([next]))

				Loop

				partitioner.currentOutputStream().Flush()
				recordReader.Dispose()
				If closeWriter Then
					partitioner.currentOutputStream().Close()
					recordWriter.Dispose()
				End If

			Else
				Do While recordReader.hasNext()
					Dim [next] As IList(Of Writable) = recordReader.next()
					'update records written
					partitioner.updatePartitionInfo(recordWriter.write([next]))
					If partitioner.needsNewPartition() Then
						partitioner.openNewStream()
					End If
				Loop
			End If
		End Sub
	End Class

End Namespace