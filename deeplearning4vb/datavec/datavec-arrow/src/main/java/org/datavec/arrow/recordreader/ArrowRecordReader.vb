Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = org.datavec.api.records.metadata.RecordMetaDataIndex
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
import static org.datavec.arrow.ArrowConverter.readFromBytes

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

Namespace org.datavec.arrow.recordreader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ArrowRecordReader implements org.datavec.api.records.reader.RecordReader
	<Serializable>
	Public Class ArrowRecordReader
		Implements RecordReader

		Private split As org.datavec.api.Split.InputSplit
		Private configuration As Configuration
		Private pathsIter As IEnumerator(Of String)
		Private currIdx As Integer
		Private currentPath As String
		Private schema As Schema
		Private recordAllocation As IList(Of Writable) = New List(Of Writable)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private ArrowWritableRecordBatch currentBatch;
		Private currentBatch As ArrowWritableRecordBatch
		Private recordListeners As IList(Of RecordListener)

		Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize
			Me.split = split
			Me.pathsIter = split.locationsPathIterator()
		End Sub

		Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize
			Me.split = split
			Me.pathsIter = split.locationsPathIterator()

		End Sub

		Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
			Return True
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			If currentBatch Is Nothing OrElse currIdx >= currentBatch.Count Then
				loadNextBatch()
			End If

			If num = currentBatch.getArrowRecordBatch().getLength() Then
				currIdx += num
				Return currentBatch
			Else
				Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(num)
				Dim numBatches As Integer = 0
				Do While hasNext() AndAlso numBatches < num
					ret.Add([next]())
				Loop

				Return ret
			End If


		End Function

		Public Overridable Function [next]() As IList(Of Writable)
			If currentBatch Is Nothing OrElse currIdx >= currentBatch.Count Then
				loadNextBatch()
			Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: recordAllocation = currentBatch.get(currIdx++);
				recordAllocation = currentBatch(currIdx)
					currIdx += 1
			End If

			Return recordAllocation

		End Function

		Private Sub loadNextBatch()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim url As String = pathsIter.next()
			Try
					Using inputStream As Stream = split.openInputStreamFor(url)
					currIdx = 0
					Dim arr() As SByte = org.apache.commons.io.IOUtils.toByteArray(inputStream)
					Dim read As val = readFromBytes(arr)
					If Me.schema Is Nothing Then
						Me.schema = read.getFirst()
					End If
        
					Me.currentBatch = read.getRight()
					Me.recordAllocation = currentBatch(0)
					currIdx += 1
					Me.currentPath = url
					End Using
			Catch e As Exception
				log.error("",e)
			End Try

		End Sub


		Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return pathsIter.hasNext() OrElse currIdx < Me.currentBatch.Count
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		Public Overridable Sub reset() Implements RecordReader.reset
			If split IsNot Nothing Then
				split.reset()
			End If
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
			Return True
		End Function

		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
			[next]()
			Dim ret As New ArrowRecord(currentBatch,currIdx - 1,URI.create(currentPath))
			Return ret
		End Function

		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
			If Not (TypeOf recordMetaData Is RecordMetaDataIndex) Then
				Throw New System.ArgumentException("Unable to load from meta data. No index specified for record")
			End If

			Dim index As RecordMetaDataIndex = DirectCast(recordMetaData, RecordMetaDataIndex)
			Dim fileSplit As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(New File(index.URI))
			initialize(fileSplit)
			Me.currIdx = CInt(Math.Truncate(index.getIndex()))
			Return nextRecord()
		End Function

		Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Dim metaDataByUri As IDictionary(Of String, IList(Of RecordMetaData)) = New Dictionary(Of String, IList(Of RecordMetaData))()
			'gather all unique locations for the metadata
			'this will prevent initialization multiple times of the record
			For Each recordMetaData As RecordMetaData In recordMetaDatas
				If Not (TypeOf recordMetaData Is RecordMetaDataIndex) Then
					Throw New System.ArgumentException("Unable to load from meta data. No index specified for record")
				End If

				Dim recordMetaData1 As IList(Of RecordMetaData) = metaDataByUri(recordMetaData.URI.ToString())
				If recordMetaData1 Is Nothing Then
					recordMetaData1 = New List(Of RecordMetaData)()
					metaDataByUri(recordMetaData.URI.ToString()) = recordMetaData1
				End If

				recordMetaData1.Add(recordMetaData)

			Next recordMetaData

			Dim ret As IList(Of Record) = New List(Of Record)()
			For Each uri As String In metaDataByUri.Keys
				Dim metaData As IList(Of RecordMetaData) = metaDataByUri(uri)
				Dim fileSplit As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(New File(URI.create(uri)))
				initialize(fileSplit)
				For Each index As RecordMetaData In metaData
					Dim index2 As RecordMetaDataIndex = DirectCast(index, RecordMetaDataIndex)
					Me.currIdx = CInt(Math.Truncate(index2.getIndex()))
					ret.Add(nextRecord())
				Next index

			Next uri

			Return ret
		End Function

		Public Overridable Property Listeners As IList(Of RecordListener)
			Get
				Return recordListeners
			End Get
			Set(ByVal listeners() As RecordListener)
				Me.recordListeners = New List(Of RecordListener)(java.util.Arrays.asList(listeners))
			End Set
		End Property


		Public Overridable WriteOnly Property Listeners As ICollection(Of RecordListener)
			Set(ByVal listeners As ICollection(Of RecordListener))
				Me.recordListeners = New List(Of RecordListener)(listeners)
			End Set
		End Property

		Public Overridable Sub Dispose()
			If currentBatch IsNot Nothing Then
				Try
					currentBatch.Dispose()
				Catch e As IOException
					log.error("",e)
				End Try
			End If
		End Sub

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