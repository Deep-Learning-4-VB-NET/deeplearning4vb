Imports System
Imports System.Collections.Generic
Imports System.IO
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports InputSplit = org.datavec.api.split.InputSplit
Imports StreamInputSplit = org.datavec.api.split.StreamInputSplit
Imports FileStreamCreatorFunction = org.datavec.api.split.streams.FileStreamCreatorFunction
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function

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


	<Serializable>
	Public MustInherit Class BaseRecordReader
		Implements RecordReader

		Public MustOverride Property Conf As Configuration
		Public MustOverride Function loadFromMetaData(ByVal recordMetaDatas As IList(Of org.datavec.api.records.metadata.RecordMetaData)) As IList(Of org.datavec.api.records.Record)
		Public MustOverride Function loadFromMetaData(ByVal recordMetaData As org.datavec.api.records.metadata.RecordMetaData) As org.datavec.api.records.Record Implements RecordReader.loadFromMetaData
		Public MustOverride Function nextRecord() As org.datavec.api.records.Record Implements RecordReader.nextRecord
		Public MustOverride Function record(ByVal uri As URI, ByVal dataInputStream As java.io.DataInputStream) As IList(Of Writable) Implements RecordReader.record
		Public MustOverride Function resetSupported() As Boolean Implements RecordReader.resetSupported
		Public MustOverride Sub reset() Implements RecordReader.reset
		Public MustOverride ReadOnly Property Labels As IList(Of String) Implements RecordReader.getLabels
		Public MustOverride Function hasNext() As Boolean Implements RecordReader.hasNext
		Public MustOverride Function [next]() As IList(Of Writable) Implements RecordReader.next
		Public MustOverride Sub initialize(ByVal conf As org.datavec.api.conf.Configuration, ByVal split As InputSplit) Implements RecordReader.initialize

		Protected Friend inputSplit As org.datavec.api.Split.InputSplit
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend listeners_Conflict As IList(Of RecordListener) = New List(Of RecordListener)()
		Protected Friend streamCreatorFn As [Function](Of URI, Stream) = New org.datavec.api.Split.streams.FileStreamCreatorFunction()

		''' <summary>
		''' Invokes <seealso cref="RecordListener.recordRead(RecordReader, Object)"/> on all listeners. </summary>
		Protected Friend Overridable Sub invokeListeners(ByVal record As Object)
			For Each listener As RecordListener In listeners_Conflict
				listener.recordRead(Me, record)
			Next listener
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize
			Me.inputSplit = split
			If TypeOf split Is org.datavec.api.Split.StreamInputSplit Then
				Dim s As org.datavec.api.Split.StreamInputSplit = DirectCast(split, org.datavec.api.Split.StreamInputSplit)
				If s.getStreamCreatorFn() IsNot Nothing Then
					Me.streamCreatorFn = s.getStreamCreatorFn()
				End If
			End If
		End Sub

		Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.getListeners
			Get
				Return listeners_Conflict
			End Get
			Set(ByVal listeners As ICollection(Of RecordListener))
				Me.listeners_Conflict = (If(TypeOf listeners Is System.Collections.IList, CType(listeners, IList(Of RecordListener)), New List(Of )(listeners)))
			End Set
		End Property


		Public Overridable WriteOnly Property Listeners Implements RecordReader.setListeners As RecordListener()
			Set(ByVal listeners() As RecordListener)
				setListeners(Arrays.asList(listeners))
			End Set
		End Property


		Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
			Return False
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable)) Implements RecordReader.next
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace