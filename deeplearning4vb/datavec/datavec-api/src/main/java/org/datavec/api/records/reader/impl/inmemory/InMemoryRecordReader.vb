Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
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

Namespace org.datavec.api.records.reader.impl.inmemory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class InMemoryRecordReader implements org.datavec.api.records.reader.RecordReader
	<Serializable>
	Public Class InMemoryRecordReader
		Implements RecordReader

		Private records As IList(Of IList(Of Writable))
		Private iter As IEnumerator(Of IList(Of Writable))
		Private labels As IList(Of String)
		Private recordListeners As IList(Of RecordListener)
		Private configuration As Configuration

		Public Sub New(ByVal records As IList(Of IList(Of Writable)))
			Me.records = records
			Me.iter = records.GetEnumerator()
		End Sub

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="split"> the split that defines the range of records to read </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize

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
		Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize

		End Sub

		Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
			Return False
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Get the next record
		''' 
		''' @return
		''' </summary>
		Public Overridable Function [next]() As IList(Of Writable)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.next()
		End Function

		''' <summary>
		''' Whether there are anymore records
		''' 
		''' @return
		''' </summary>
		Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function

		''' <summary>
		''' List of label strings
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return labels
			End Get
		End Property

		''' <summary>
		''' Reset record reader iterator
		''' 
		''' @return
		''' </summary>
		Public Overridable Sub reset() Implements RecordReader.reset
			iter = records.GetEnumerator()
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
			Return True
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
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Similar to <seealso cref="next()"/>, but returns a <seealso cref="Record"/> object, that may include metadata such as the source
		''' of the data
		''' </summary>
		''' <returns> next record </returns>
		Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return New org.datavec.api.records.impl.Record(iter.next(), Nothing)
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
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Load multiple records from the given a list of <seealso cref="RecordMetaData"/> instances<br>
		''' </summary>
		''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
		''' <returns> Multiple records for the given RecordMetaData instances </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.Record> loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Get the record listeners for this record reader.
		''' </summary>
		Public Overridable Property Listeners As IList(Of RecordListener)
			Get
				Return recordListeners
			End Get
			Set(ByVal listeners() As RecordListener)
				Me.recordListeners = New List(Of RecordListener) From {listeners}
			End Set
		End Property


		''' <summary>
		''' Set the record listeners for this record reader.
		''' </summary>
		''' <param name="listeners"> </param>
		Public Overridable WriteOnly Property Listeners As ICollection(Of RecordListener)
			Set(ByVal listeners As ICollection(Of RecordListener))
				Me.recordListeners = New List(Of RecordListener)(listeners)
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

		End Sub

		''' <summary>
		''' Set the configuration to be used by this object.
		''' </summary>
		''' <param name="conf"> </param>
		Public Overridable Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.configuration = conf
			End Set
			Get
				Return configuration
			End Get
		End Property

	End Class

End Namespace