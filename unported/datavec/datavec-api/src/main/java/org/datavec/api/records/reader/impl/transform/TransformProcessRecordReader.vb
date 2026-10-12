Imports System
Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
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


	<Serializable>
	Public Class TransformProcessRecordReader
		Implements RecordReader

		Protected Friend recordReader As RecordReader
		Protected Friend transformProcess As TransformProcess

		'Cached/prefetched values, in case of filtering
'JAVA TO VB CONVERTER NOTE: The field next was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend next_Conflict As Record

		Public Sub New(ByVal recordReader As RecordReader, ByVal transformProcess As TransformProcess)
			Me.recordReader = recordReader
			Me.transformProcess = transformProcess
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
			recordReader.initialize(split)
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
			recordReader.initialize(conf, split)
		End Sub

		Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
			Return True
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable)) Implements RecordReader.next
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim i As Integer=0
			Do While i<num AndAlso hasNext()
				[out].Add([next]())
				i += 1
			Loop
			Return [out]
		End Function

		''' <summary>
		''' Get the next record
		''' 
		''' @return
		''' </summary>
		Public Overridable Function [next]() As IList(Of Writable) Implements RecordReader.next
			If Not hasNext() Then 'Also triggers prefetch
				Throw New NoSuchElementException("No next element")
			End If
			Dim [out] As IList(Of Writable) = next_Conflict.getRecord()
			next_Conflict = Nothing
			Return [out]
		End Function

		''' <summary>
		''' Whether there are anymore records
		''' 
		''' @return
		''' </summary>
		Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
			If next_Conflict IsNot Nothing Then
				Return True
			End If
			If Not recordReader.hasNext() Then
				Return False
			End If

			'Prefetch, until we find one that isn't filtered out - or we run out of data
			Do While next_Conflict Is Nothing AndAlso recordReader.hasNext()
				Dim r As Record = recordReader.nextRecord()
				Dim temp As IList(Of Writable) = transformProcess.execute(r.getRecord())
				If temp Is Nothing Then
					Continue Do
				End If
				next_Conflict = New org.datavec.api.records.impl.Record(temp, r.MetaData)
			Loop

			Return next_Conflict IsNot Nothing
		End Function

		''' <summary>
		''' List of label strings
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.getLabels
			Get
				Return recordReader.getLabels()
			End Get
		End Property

		''' <summary>
		''' Reset record reader iterator
		''' 
		''' @return
		''' </summary>
		Public Overridable Sub reset() Implements RecordReader.reset
			next_Conflict = Nothing
			recordReader.reset()
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
			Return recordReader.resetSupported()
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
		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable) Implements RecordReader.record
			Return transformProcess.execute(recordReader.record(uri, dataInputStream))
		End Function

		''' <summary>
		''' Similar to <seealso cref="next()"/>, but returns a <seealso cref="Record"/> object, that may include metadata such as the source
		''' of the data
		''' </summary>
		''' <returns> next record </returns>
		Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
			If Not hasNext() Then 'Also triggers prefetch
				Throw New NoSuchElementException("No next element")
			End If
			Dim toRet As Record = next_Conflict
			next_Conflict = Nothing
			Return toRet
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
			Return recordReader.loadFromMetaData(recordMetaData)
		End Function

		''' <summary>
		''' Load multiple records from the given a list of <seealso cref="RecordMetaData"/> instances<br>
		''' </summary>
		''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
		''' <returns> Multiple records for the given RecordMetaData instances </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
			Return recordReader.loadFromMetaData(recordMetaDatas)
		End Function

		''' <summary>
		''' Get the record listeners for this record reader.
		''' </summary>
		Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.getListeners
			Get
				Return recordReader.getListeners()
			End Get
			Set(ByVal listeners() As RecordListener)
				recordReader.setListeners(listeners)
			End Set
		End Property


		''' <summary>
		''' Set the record listeners for this record reader.
		''' </summary>
		''' <param name="listeners"> </param>
		Public Overridable WriteOnly Property Listeners Implements RecordReader.setListeners As ICollection(Of RecordListener)
			Set(ByVal listeners As ICollection(Of RecordListener))
				recordReader.setListeners(listeners)
    
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
			recordReader.Dispose()
		End Sub

		''' <summary>
		''' Set the configuration to be used by this object.
		''' </summary>
		''' <param name="conf"> </param>
		Public Overridable Property Conf As Configuration
			Set(ByVal conf As Configuration)
    
			End Set
			Get
				Return recordReader.Conf
			End Get
		End Property

	End Class

End Namespace