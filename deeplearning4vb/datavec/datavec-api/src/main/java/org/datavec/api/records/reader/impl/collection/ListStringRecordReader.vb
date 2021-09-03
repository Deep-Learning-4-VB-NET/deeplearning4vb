Imports System
Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports ListStringSplit = org.datavec.api.split.ListStringSplit
Imports Text = org.datavec.api.writable.Text
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

Namespace org.datavec.api.records.reader.impl.collection


	<Serializable>
	Public Class ListStringRecordReader
		Inherits BaseRecordReader

		Private delimitedData As IList(Of IList(Of String))
		Private dataIter As IEnumerator(Of IList(Of String))
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private conf_Conflict As Configuration

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="split"> the split that defines the range of records to read </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			If TypeOf split Is org.datavec.api.Split.ListStringSplit Then
				Dim listStringSplit As org.datavec.api.Split.ListStringSplit = DirectCast(split, org.datavec.api.Split.ListStringSplit)
				delimitedData = listStringSplit.getData()
				dataIter = delimitedData.GetEnumerator()
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Illegal type of input split " & split.GetType().FullName)
			End If
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
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			initialize(split)
		End Sub

		''' <summary>
		''' Get the next record
		''' </summary>
		''' <returns> The list of next record </returns>
		Public Overrides Function [next]() As IList(Of Writable)
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim next_Conflict As IList(Of String) = dataIter.next()
			invokeListeners(next_Conflict)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			For Each s As String In next_Conflict
				ret.Add(New Text(s))
			Next s
			Return ret
		End Function

		''' <summary>
		''' Check whether there are anymore records
		''' </summary>
		''' <returns> Whether there are more records </returns>
		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return dataIter.hasNext()
		End Function

		''' <summary>
		''' List of label strings
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Reset record reader iterator
		''' 
		''' </summary>
		Public Overrides Sub reset()
			dataIter = delimitedData.GetEnumerator()
		End Sub

		Public Overrides Function resetSupported() As Boolean
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
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Return Nothing
		End Function

		Public Overrides Function nextRecord() As Record
			Return New org.datavec.api.records.impl.Record([next](), Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Throw New System.NotSupportedException("Loading from metadata not yet implemented")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Throw New System.NotSupportedException("Loading from metadata not yet implemented")
		End Function

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
		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.conf_Conflict = conf
			End Set
			Get
				Return conf_Conflict
			End Get
		End Property

	End Class

End Namespace