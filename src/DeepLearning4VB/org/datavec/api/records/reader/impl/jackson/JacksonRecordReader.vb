Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Configuration = org.datavec.api.conf.Configuration
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = org.datavec.api.records.metadata.RecordMetaDataURI
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports TypeReference = org.nd4j.shade.jackson.core.type.TypeReference
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.datavec.api.records.reader.impl.jackson


	<Serializable>
	Public Class JacksonRecordReader
		Inherits BaseRecordReader

		Private Shared ReadOnly typeRef As TypeReference(Of IDictionary(Of String, Object)) = New TypeReferenceAnonymousInnerClass()

		Private Class TypeReferenceAnonymousInnerClass
			Inherits TypeReference(Of IDictionary(Of String, Object))

		End Class

		Private selection As FieldSelection
		Private mapper As ObjectMapper
		Private shuffle As Boolean
		Private rngSeed As Long
		Private labelGenerator As PathLabelGenerator
		Private labelPosition As Integer
		Private [is] As org.datavec.api.Split.InputSplit
		Private r As Random
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private String charset = java.nio.charset.StandardCharsets.UTF_8.name();
		Private charset As String = StandardCharsets.UTF_8.name() 'Using String as StandardCharsets.UTF_8 is not serializable

		Private uris() As URI
		Private cursor As Integer = 0

		Public Sub New(ByVal selection As FieldSelection, ByVal mapper As ObjectMapper)
			Me.New(selection, mapper, False)
		End Sub

		Public Sub New(ByVal selection As FieldSelection, ByVal mapper As ObjectMapper, ByVal shuffle As Boolean)
			Me.New(selection, mapper, shuffle, DateTimeHelper.CurrentUnixTimeMillis(), Nothing)
		End Sub

		Public Sub New(ByVal selection As FieldSelection, ByVal mapper As ObjectMapper, ByVal shuffle As Boolean, ByVal rngSeed As Long, ByVal labelGenerator As PathLabelGenerator)
			Me.New(selection, mapper, shuffle, rngSeed, labelGenerator, -1)
		End Sub

		Public Sub New(ByVal selection As FieldSelection, ByVal mapper As ObjectMapper, ByVal shuffle As Boolean, ByVal rngSeed As Long, ByVal labelGenerator As PathLabelGenerator, ByVal labelPosition As Integer)
			Me.selection = selection
			Me.mapper = mapper
			Me.shuffle = shuffle
			Me.rngSeed = rngSeed
			If shuffle Then
				r = New Random(CInt(rngSeed))
			End If
			Me.labelGenerator = labelGenerator
			Me.labelPosition = labelPosition
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			If TypeOf split Is org.datavec.api.Split.FileSplit Then
				Throw New System.NotSupportedException("Cannot use JacksonRecordReader with FileSplit")
			End If
			MyBase.initialize(inputSplit)
			Me.uris = split.locations()
			If shuffle Then
				Dim list As IList(Of URI) = New List(Of URI) From {uris}
				Collections.shuffle(list, r)
				uris = CType(list, List(Of URI)).ToArray()
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			initialize(split)
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			If uris Is Nothing Then
				Throw New System.InvalidOperationException("URIs are null. Not initialized?")
			End If
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.net.URI uri = uris[cursor++];
			Dim uri As URI = uris(cursor)
				cursor += 1
			invokeListeners(uri)
			Dim fileAsString As String
			Try
					Using s As Stream = streamCreatorFn.apply(uri)
					fileAsString = IOUtils.toString(s, charset)
					End Using
			Catch e As IOException
				Throw New Exception("Error reading URI file", e)
			End Try

			Return readValues(uri, fileAsString)

		End Function

		Public Overrides Function hasNext() As Boolean
			Return cursor < uris.Length
		End Function

		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		Public Overrides Sub reset()
			cursor = 0
			If shuffle Then
				Dim list As IList(Of URI) = New List(Of URI) From {uris}
				Collections.shuffle(list, r)
				uris = CType(list, List(Of URI)).ToArray()
			End If
		End Sub

		Public Overrides Function resetSupported() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Dim br As New StreamReader(dataInputStream)
			Dim sb As New StringBuilder()
			Dim line As String
			line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
			Do While line IsNot Nothing
				sb.Append(line).Append(vbLf)
					line = br.ReadLine()
			Loop

			Return readValues(uri, sb.ToString())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws IOException
		Public Overridable Sub Dispose()

		End Sub

		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
    
			End Set
			Get
				Return Nothing
			End Get
		End Property



		Private Function readValues(ByVal uri As URI, ByVal fileContents As String) As IList(Of Writable)
			Dim [out] As IList(Of Writable) = JacksonReaderUtils.parseRecord(fileContents, selection, mapper)

			'Add label - if required
			If labelGenerator IsNot Nothing Then
				Dim label As Writable = labelGenerator.getLabelForPath(uri)
				Dim paths As IList(Of String()) = selection.getFieldPaths()
				If (labelPosition >= paths.Count OrElse labelPosition = -1) Then
					'Edge case: might want label as the last value
					[out].Add(label)
				Else
					[out].Insert(labelPosition, label) 'Add and shift existing to right
				End If
			End If

			Return [out]
		End Function

		Public Overrides Function nextRecord() As Record
			Dim currentURI As URI = uris(cursor)
			Dim writables As IList(Of Writable) = [next]()
			Dim meta As RecordMetaData = New RecordMetaDataURI(currentURI, GetType(JacksonRecordReader))
			Return New org.datavec.api.records.impl.Record(writables, meta)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Return loadFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.Record> loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)

			Dim [out] As IList(Of Record) = New List(Of Record)()
			For Each metaData As RecordMetaData In recordMetaDatas
				Dim uri As URI = metaData.URI

				Dim fileAsString As String
				Try
					fileAsString = FileUtils.readFileToString(New File(uri.toURL().getFile()))
				Catch e As IOException
					Throw New Exception("Error reading URI file", e)
				End Try

				Dim writables As IList(Of Writable) = readValues(uri, fileAsString)
				[out].Add(New org.datavec.api.records.impl.Record(writables, metaData))
			Next metaData

			Return [out]
		End Function
	End Class

End Namespace