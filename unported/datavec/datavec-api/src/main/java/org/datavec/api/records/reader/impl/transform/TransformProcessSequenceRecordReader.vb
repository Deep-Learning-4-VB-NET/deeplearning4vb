Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports TransformProcess = org.datavec.api.transform.TransformProcess
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

Namespace org.datavec.api.records.reader.impl.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class TransformProcessSequenceRecordReader implements org.datavec.api.records.reader.SequenceRecordReader
	<Serializable>
	Public Class TransformProcessSequenceRecordReader
		Implements SequenceRecordReader

		Protected Friend sequenceRecordReader As SequenceRecordReader
		Protected Friend transformProcess As TransformProcess



		''' <summary>
		''' Set the configuration to be used by this object.
		''' </summary>
		''' <param name="conf"> </param>
		Public Overridable Property Conf As Configuration
			Set(ByVal conf As Configuration)
				sequenceRecordReader.Conf = conf
			End Set
			Get
				Return sequenceRecordReader.Conf
			End Get
		End Property


		''' <summary>
		''' Returns a sequence record.
		''' </summary>
		''' <returns> a sequence of records </returns>
		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Return transformProcess.executeSequence(sequenceRecordReader.sequenceRecord())
		End Function

		Public Overridable Function batchesSupported() As Boolean
			Return False
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Load a sequence record from the given DataInputStream
		''' Unlike <seealso cref="next()"/> the internal state of the RecordReader is not modified
		''' Implementations of this method should not close the DataInputStream
		''' </summary>
		''' <param name="uri"> </param>
		''' <param name="dataInputStream"> </param>
		''' <exception cref="IOException"> if error occurs during reading from the input stream </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Return transformProcess.executeSequence(sequenceRecordReader.sequenceRecord(uri, dataInputStream))
		End Function

		''' <summary>
		''' Similar to <seealso cref="sequenceRecord()"/>, but returns a <seealso cref="Record"/> object, that may include metadata such as the source
		''' of the data
		''' </summary>
		''' <returns> next sequence record </returns>
		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Dim [next] As SequenceRecord = sequenceRecordReader.nextSequence()
			[next].SequenceRecord = transformProcess.executeSequence([next].getSequenceRecord())
			Return [next]
		End Function

		''' <summary>
		''' Load a single sequence record from the given <seealso cref="RecordMetaData"/> instance<br>
		''' Note: that for data that isn't splittable (i.e., text data that needs to be scanned/split), it is more efficient to
		''' load multiple records at once using <seealso cref="loadSequenceFromMetaData(List)"/>
		''' </summary>
		''' <param name="recordMetaData"> Metadata for the sequence record that we want to load from </param>
		''' <returns> Single sequence record for the given RecordMetaData instance </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Dim [next] As SequenceRecord = sequenceRecordReader.loadSequenceFromMetaData(recordMetaData)
			[next].SequenceRecord = transformProcess.executeSequence([next].getSequenceRecord())
			Return [next]
		End Function

		''' <summary>
		''' Load multiple sequence records from the given a list of <seealso cref="RecordMetaData"/> instances<br>
		''' </summary>
		''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
		''' <returns> Multiple sequence record for the given RecordMetaData instances </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
			Return Nothing
		End Function

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="split"> the split that defines the range of records to read </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal split As InputSplit)

		End Sub

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="conf">  a configuration for initialization </param>
		''' <param name="split"> the split that defines the range of records to read </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)

		End Sub

		''' <summary>
		''' Get the next record
		''' 
		''' @return
		''' </summary>
		Public Overridable Function [next]() As IList(Of Writable)
			Return transformProcess.execute(sequenceRecordReader.next())
		End Function

		''' <summary>
		''' Whether there are anymore records
		''' 
		''' @return
		''' </summary>
		Public Overridable Function hasNext() As Boolean
			Return sequenceRecordReader.hasNext()
		End Function

		''' <summary>
		''' List of label strings
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return sequenceRecordReader.getLabels()
			End Get
		End Property

		''' <summary>
		''' Reset record reader iterator
		''' 
		''' @return
		''' </summary>
		Public Overridable Sub reset()
			sequenceRecordReader.reset()
		End Sub

		Public Overridable Function resetSupported() As Boolean
			Return sequenceRecordReader.resetSupported()
		End Function

		''' <summary>
		''' Load the record from the given DataInputStream
		''' Unlike <seealso cref="next()"/> the internal state of the RecordReader is not modified
		''' Implementations of this method should not close the DataInputStream
		''' </summary>
		''' <param name="uri"> </param>
		''' <param name="dataInputStream"> </param>
		''' <exception cref="IOException"> if error occurs during reading from the input stream </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Return transformProcess.execute(sequenceRecordReader.record(uri, dataInputStream))
		End Function

		''' <summary>
		''' Similar to <seealso cref="next()"/>, but returns a <seealso cref="Record"/> object, that may include metadata such as the source
		''' of the data
		''' </summary>
		''' <returns> next record </returns>
		Public Overridable Function nextRecord() As Record
			Dim [next] As Record = sequenceRecordReader.nextRecord()
			[next].Record = transformProcess.execute([next].getRecord())
			Return [next]
		End Function

		''' <summary>
		''' Load a single record from the given <seealso cref="RecordMetaData"/> instance<br>
		''' Note: that for data that isn't splittable (i.e., text data that needs to be scanned/split), it is more efficient to
		''' load multiple records at once using <seealso cref="loadFromMetaData(List)"/>
		''' </summary>
		''' <param name="recordMetaData"> Metadata for the record that we want to load from </param>
		''' <returns> Single record for the given RecordMetaData instance </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Dim load As Record = sequenceRecordReader.loadFromMetaData(recordMetaData)
			load.Record = transformProcess.execute(load.getRecord())
			Return load
		End Function

		''' <summary>
		''' Load multiple records from the given a list of <seealso cref="RecordMetaData"/> instances<br>
		''' </summary>
		''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
		''' <returns> Multiple records for the given RecordMetaData instances </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Dim records As IList(Of Record) = sequenceRecordReader.loadFromMetaData(recordMetaDatas)
			For Each record As Record In records
				record.Record = transformProcess.execute(record.getRecord())
			Next record
			Return records
		End Function

		''' <summary>
		''' Get the record listeners for this record reader.
		''' </summary>
		Public Overridable Property Listeners As IList(Of RecordListener)
			Get
				Return sequenceRecordReader.getListeners()
			End Get
			Set(ByVal listeners() As RecordListener)
				sequenceRecordReader.setListeners(listeners)
			End Set
		End Property


		''' <summary>
		''' Set the record listeners for this record reader.
		''' </summary>
		''' <param name="listeners"> </param>
		Public Overridable WriteOnly Property Listeners As ICollection(Of RecordListener)
			Set(ByVal listeners As ICollection(Of RecordListener))
				sequenceRecordReader.setListeners(listeners)
			End Set
		End Property

		''' <summary>
		''' Closes this stream and releases any system resources associated
		''' with it. If the stream is already closed then invoking this
		''' method has no effect.
		''' <para>
		''' </para>
		''' <para> As noted in <seealso cref="AutoCloseable.close()"/>, cases where the
		''' close may fail require careful attention. It is strongly advised
		''' to relinquish the underlying resources and to internally
		''' <em>mark</em> the {@code Closeable} as closed, prior to throwing
		''' the {@code IOException}.
		''' 
		''' </para>
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overridable Sub Dispose()
			sequenceRecordReader.Dispose()
		End Sub
	End Class

End Namespace