Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = org.datavec.api.records.metadata.RecordMetaDataLine
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports InputStreamInputSplit = org.datavec.api.split.InputStreamInputSplit
Imports StringSplit = org.datavec.api.split.StringSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives

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

Namespace org.datavec.api.records.reader.impl


	''' <summary>
	''' Reads files line by line
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LineRecordReader extends org.datavec.api.records.reader.BaseRecordReader
	<Serializable>
	Public Class LineRecordReader
		Inherits BaseRecordReader


		Private iter As IEnumerator(Of String)
		Protected Friend locations() As URI
		Protected Friend splitIndex As Integer = 0
		Protected Friend lineIndex As Integer = 0 'Line index within the current split
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As Configuration
		Protected Friend initialized As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String charset = java.nio.charset.StandardCharsets.UTF_8.name();
		Protected Friend charset As String = StandardCharsets.UTF_8.name() 'Using String as StandardCharsets.UTF_8 is not serializable

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			MyBase.initialize(split)
			If Not (TypeOf inputSplit Is org.datavec.api.Split.StringSplit OrElse TypeOf inputSplit Is org.datavec.api.Split.InputStreamInputSplit) Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ArrayList<java.net.URI> uris = new ArrayList<>();
				Dim uris As New List(Of URI)()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Iterator<java.net.URI> uriIterator = inputSplit.locationsIterator();
				Dim uriIterator As IEnumerator(Of URI) = inputSplit.locationsIterator()
				Do While uriIterator.MoveNext()
					uris.Add(uriIterator.Current)
				Loop

				Me.locations = uris.ToArray()
			End If
			Me.iter = getIterator(0)
			Me.initialized = True
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			Me.conf_Conflict = conf
			initialize(split)
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			Preconditions.checkState(initialized, "Record reader has not been initialized")
			Dim ret As IList(Of Writable) = New List(Of Writable)()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If iter.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim record As String = iter.next()
				invokeListeners(record)
				ret.Add(New Text(record))
				lineIndex += 1
				Return ret
			Else
				If Not (TypeOf inputSplit Is org.datavec.api.Split.StringSplit) AndAlso splitIndex < locations.Length - 1 Then
					splitIndex += 1
					lineIndex = 0 'New split opened -> reset line index
					Try
						Dispose()
						iter = getIterator(splitIndex)
						onLocationOpen(locations(splitIndex))
					Catch e As IOException
						log.error("",e)
					End Try

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If iter.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim record As String = iter.next()
						invokeListeners(record)
						ret.Add(New Text(record))
						lineIndex += 1
						Return ret
					End If
				End If

				Throw New NoSuchElementException("No more elements found!")
			End If
		End Function

		Public Overrides Function hasNext() As Boolean
			Preconditions.checkState(initialized, "Record reader has not been initialized")

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If iter IsNot Nothing AndAlso iter.hasNext() Then
				Return True
			Else
				If locations IsNot Nothing AndAlso Not (TypeOf inputSplit Is org.datavec.api.Split.StringSplit) AndAlso splitIndex < locations.Length - 1 Then
					splitIndex += 1
					lineIndex = 0 'New split -> reset line count
					Try
						Dispose()
						iter = getIterator(splitIndex)
						onLocationOpen(locations(splitIndex))
					Catch e As IOException
						log.error("",e)
					End Try

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return iter.hasNext()
				End If

				Return False
			End If
		End Function

		Protected Friend Overridable Sub onLocationOpen(ByVal location As URI)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws IOException
		Public Overridable Sub Dispose()
			If iter IsNot Nothing Then
				If TypeOf iter Is LineIterator Then
					Dim iter2 As LineIterator = CType(iter, LineIterator)
					iter2.close()
				End If
			End If
		End Sub

		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.conf_Conflict = conf
			End Set
			Get
				Return conf_Conflict
			End Get
		End Property


		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Sub reset()
			If inputSplit Is Nothing Then
				Throw New System.NotSupportedException("Cannot reset without first initializing")
			End If
			Try
				inputSplit.reset()
				Dispose()
				initialize(inputSplit)
				splitIndex = 0
			Catch e As Exception
				Throw New Exception("Error during LineRecordReader reset", e)
			End Try
			lineIndex = 0
		End Sub

		Public Overrides Function resetSupported() As Boolean
			If inputSplit IsNot Nothing Then
				Return inputSplit.resetSupported()
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			invokeListeners(uri)
			'Here: we are reading a single line from the DataInputStream
			Dim br As New StreamReader(dataInputStream)
			Dim line As String = br.ReadLine()
			Return Collections.singletonList(DirectCast(New Text(line), Writable))
		End Function

		Protected Friend Overridable Function getIterator(ByVal location As Integer) As IEnumerator(Of String)
			Dim iterator As IEnumerator(Of String) = Nothing
			If TypeOf inputSplit Is org.datavec.api.Split.StringSplit Then
				Dim stringSplit As org.datavec.api.Split.StringSplit = DirectCast(inputSplit, org.datavec.api.Split.StringSplit)
'JAVA TO VB CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
				iterator = Collections.singletonList(stringSplit.Data).GetEnumerator()
			ElseIf TypeOf inputSplit Is org.datavec.api.Split.InputStreamInputSplit Then
				Dim [is] As Stream = DirectCast(inputSplit, org.datavec.api.Split.InputStreamInputSplit).Is
				If [is] IsNot Nothing Then
					Try
						iterator = IOUtils.lineIterator(New StreamReader([is], charset))
					Catch e As UnsupportedEncodingException
						Throw New Exception("Unsupported encoding: " & charset, e)
					End Try
				End If
			Else
				If locations.Length > 0 Then
					Dim inputStream As Stream = streamCreatorFn.apply(locations(location))
					Try
						iterator = IOUtils.lineIterator(New StreamReader(inputStream, charset))
					Catch e As UnsupportedEncodingException
						Throw New Exception("Unsupported encoding: " & charset, e)
					End Try
				End If
			End If
			If iterator Is Nothing Then
				Throw New System.NotSupportedException("Unknown input split: " & inputSplit)
			End If
			Return iterator
		End Function

		Protected Friend Overridable Sub closeIfRequired(ByVal iterator As IEnumerator(Of String))
			If TypeOf iterator Is LineIterator Then
				Dim iter As LineIterator = CType(iterator, LineIterator)
				iter.close()
			End If
		End Sub

		Public Overrides Function nextRecord() As Record
			Dim [next] As IList(Of Writable) = Me.next()
			Dim uri As URI = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
			Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(LineRecordReader)) '-1 as line number has been incremented already...
			Return New org.datavec.api.records.impl.Record([next], meta)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Return Nothing
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.Record> loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			'First: create a sorted list of the RecordMetaData
			Dim list As IList(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))) = New List(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)))()
			Dim uris As ISet(Of URI) = New HashSet(Of URI)()
			Dim iter As IEnumerator(Of RecordMetaData) = recordMetaDatas.GetEnumerator()
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim rmd As RecordMetaData = iter.Current
				If Not (TypeOf rmd Is RecordMetaDataLine) Then
					Throw New System.ArgumentException("Invalid metadata; expected RecordMetaDataLine instance; got: " & rmd)
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: list.add(new org.nd4j.common.primitives.Triple<>(count++, (org.datavec.api.records.metadata.RecordMetaDataLine) rmd, (List<org.datavec.api.writable.Writable>) null));
				list.Add(New Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))(count, DirectCast(rmd, RecordMetaDataLine), DirectCast(Nothing, IList(Of Writable))))
					count += 1
				If rmd.URI IsNot Nothing Then
					uris.Add(rmd.URI)
				End If
			Loop
			Dim sortedURIs As IList(Of URI) = Nothing
			If uris.Count > 0 Then
				sortedURIs = New List(Of URI)(uris)
				sortedURIs.Sort()
			End If

			'Sort by URI first (if possible - don't always have URIs though, for String split etc), then sort by line number:
			list.Sort(New ComparatorAnonymousInnerClass(Me))

			If uris.Count > 0 AndAlso sortedURIs IsNot Nothing Then
				'URIs case - possibly with multiple URIs
				Dim metaIter As IEnumerator(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))) = list.GetEnumerator() 'Currently sorted by URI, then line number

				Dim currentURI As URI = sortedURIs(0)
				Dim currentUriIter As IEnumerator(Of String) = IOUtils.lineIterator(streamCreatorFn.apply(currentURI), charset)

				Dim currentURIIdx As Integer = 0 'Index of URI
				Dim currentLineIdx As Integer = 0 'Index of the line for the current URI
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim line As String = currentUriIter.next()
				Do While metaIter.MoveNext()
					Dim t As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)) = metaIter.Current
					Dim thisURI As URI = t.getSecond().getURI()
					Dim nextLineIdx As Integer = t.getSecond().getLineNumber()

					'First: find the right URI for this record...
					Do While Not currentURI.Equals(thisURI)
						'Iterate to the next URI
						currentURIIdx += 1
						If currentURIIdx >= sortedURIs.Count Then
							'Should never happen
							Throw New System.InvalidOperationException("Count not find URI " & thisURI & " in URIs list: " & sortedURIs)
						End If
						currentURI = sortedURIs(currentURIIdx)
						currentLineIdx = 0
						If currentURI.Equals(thisURI) Then
							'Found the correct URI for this MetaData instance
							closeIfRequired(currentUriIter)
							currentUriIter = IOUtils.lineIterator(New StreamReader(currentURI.toURL().openStream()))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							line = currentUriIter.next()
						End If
					Loop

					'Have the correct URI/iter open -> scan to the required line
					Do While currentLineIdx < nextLineIdx AndAlso currentUriIter.MoveNext()
						line = currentUriIter.Current
						currentLineIdx += 1
					Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If currentLineIdx < nextLineIdx AndAlso Not currentUriIter.hasNext() Then
						Throw New System.InvalidOperationException("Could not get line " & nextLineIdx & " from URI " & currentURI & ": has only " & currentLineIdx & " lines")
					End If
					t.setThird(Collections.singletonList(Of Writable)(New Text(line)))
				Loop
			Else
				'Not URI based: String split, etc
				Dim iterator As IEnumerator(Of String) = getIterator(0)
				Dim metaIter As IEnumerator(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))) = list.GetEnumerator()
				Dim currentLineIdx As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim line As String = iterator.next()
				Do While metaIter.MoveNext()
					Dim t As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)) = metaIter.Current
					Dim nextLineIdx As Integer = t.getSecond().getLineNumber()
					Do While currentLineIdx < nextLineIdx AndAlso iterator.MoveNext()
						line = iterator.Current
						currentLineIdx += 1
					Loop
					t.setThird(Collections.singletonList(Of Writable)(New Text(line)))
				Loop
				closeIfRequired(iterator)
			End If


			'Now, sort by the original (request) order:
			list.Sort(New ComparatorAnonymousInnerClass2(Me))

			'And return...
			Dim [out] As IList(Of Record) = New List(Of Record)()
			For Each t As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)) In list
				[out].Add(New org.datavec.api.records.impl.Record(t.getThird(), t.getSecond()))
			Next t
			Return [out]
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)))

			Private ReadOnly outerInstance As LineRecordReader

			Public Sub New(ByVal outerInstance As LineRecordReader)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)), ByVal o2 As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))) As Integer Implements IComparer(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))).Compare
				If o1.getSecond().getURI() IsNot Nothing Then
					If Not o1.getSecond().getURI().Equals(o2.getSecond().getURI()) Then
						Return o1.getSecond().getURI().compareTo(o2.getSecond().getURI())
					End If
				End If
				Return Integer.compare(o1.getSecond().getLineNumber(), o2.getSecond().getLineNumber())
			End Function
		End Class

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)))

			Private ReadOnly outerInstance As LineRecordReader

			Public Sub New(ByVal outerInstance As LineRecordReader)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable)), ByVal o2 As Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))) As Integer Implements IComparer(Of Triple(Of Integer, RecordMetaDataLine, IList(Of Writable))).Compare
				Return Integer.compare(o1.getFirst(), o2.getFirst())
			End Function
		End Class
	End Class

End Namespace