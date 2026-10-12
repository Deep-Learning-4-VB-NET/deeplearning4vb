Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = org.datavec.api.records.metadata.RecordMetaDataURI
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports IntWritable = org.datavec.api.writable.IntWritable
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

Namespace org.datavec.api.records.reader.impl


	''' <summary>
	''' File reader/writer
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class FileRecordReader
		Inherits BaseRecordReader

		Protected Friend locationsIterator As IEnumerator(Of URI)
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As Configuration
		Protected Friend currentUri As URI
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend labels_Conflict As IList(Of String)
		Protected Friend appendLabel As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String charset = java.nio.charset.StandardCharsets.UTF_8.name();
		Protected Friend charset As String = StandardCharsets.UTF_8.name() 'Using String as StandardCharsets.UTF_8 is not serializable

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			MyBase.initialize(split)
			doInitialize(split)
		End Sub


		Protected Friend Overridable Sub doInitialize(ByVal split As InputSplit)

			If labels_Conflict Is Nothing AndAlso appendLabel Then
				Dim locations() As URI = split.locations()
				If locations.Length > 0 Then
					Dim labels As ISet(Of String) = New HashSet(Of String)()
					For Each u As URI In locations
						Dim pathSplit() As String = u.ToString().Split("[/\\]", True)
						labels.Add(pathSplit(pathSplit.Length-2))
					Next u
					Me.labels_Conflict = New List(Of String)(labels)
					Me.labels_Conflict.Sort()
				End If
			End If
			locationsIterator = split.locationsIterator()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			appendLabel = conf.getBoolean(APPEND_LABEL, True)
			doInitialize(split)
			Me.inputSplit = split
			Me.conf_Conflict = conf
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			Return nextRecord().getRecord()
		End Function

		Private Function loadFromStream(ByVal uri As URI, ByVal [next] As Stream, ByVal charset As Charset) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			Try
				If Not (TypeOf [next] Is BufferedInputStream) Then
					[next] = New BufferedInputStream([next])
				End If
				Dim s As String = org.apache.commons.io.IOUtils.toString([next], charset)
				ret.Add(New Text(s))
				If appendLabel Then
					Dim idx As Integer = getLabel(uri)
					ret.Add(New IntWritable(idx))
				End If
			Catch e As IOException
				Throw New System.InvalidOperationException("Error reading from input stream: " & uri)
			End Try
			Return ret
		End Function

		''' <summary>
		''' Return the current label.
		''' The index of the current file's parent directory
		''' in the label list </summary>
		''' <returns> The index of the current file's parent directory </returns>
		Public Overridable ReadOnly Property CurrentLabel As Integer
			Get
				Return getLabel(currentUri)
			End Get
		End Property

		Public Overridable Function getLabel(ByVal uri As URI) As Integer
			Dim s As String = uri.ToString()
			Dim lastIdx As Integer = Math.Max(s.LastIndexOf("/"c), s.LastIndexOf("\"c)) 'Note: if neither are found, -1 is fine here
			Dim [sub] As String = s.Substring(0, lastIdx)
			Dim secondLastIdx As Integer = Math.Max([sub].LastIndexOf("/"c), [sub].LastIndexOf("\"c))
			Dim name As String = s.Substring(secondLastIdx+1, lastIdx - (secondLastIdx+1))
			Return labels_Conflict.IndexOf(name)
		End Function

		Public Overrides Property Labels As IList(Of String)
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels As IList(Of String))
				Me.labels_Conflict = labels
			End Set
		End Property


		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return locationsIterator.hasNext()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws IOException
		Public Overridable Sub Dispose()

		End Sub

		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.conf_Conflict = conf
			End Set
			Get
				Return conf_Conflict
			End Get
		End Property


		Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(num)
			Dim numBatches As Integer = 0
			Do While hasNext() AndAlso numBatches < num
				ret.Add([next]())
			Loop

			Return ret
		End Function
		Public Overrides Sub reset()
			If inputSplit Is Nothing Then
				Throw New System.NotSupportedException("Cannot reset without first initializing")
			End If
			Try
				doInitialize(inputSplit)
			Catch e As Exception
				Throw New Exception("Error during LineRecordReader reset", e)
			End Try
		End Sub

		Public Overrides Function resetSupported() As Boolean
			If inputSplit IsNot Nothing Then
				Return inputSplit.resetSupported()
			End If
			Return False 'reset() throws exception on reset() if inputSplit is null
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			invokeListeners(uri)
			'Here: reading the entire file to a Text writable
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
			Return Collections.singletonList(DirectCast(New Text(sb.ToString()), Writable))
		End Function

		Public Overrides Function nextRecord() As Record
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As URI = locationsIterator.next()
			invokeListeners([next])

			Dim ret As IList(Of Writable)
			Try
					Using s As Stream = streamCreatorFn.apply([next])
					ret = loadFromStream([next], s, Charset.forName(charset))
					End Using
			Catch e As IOException
				Throw New Exception("Error reading from stream for URI: " & [next])
			End Try

			Return New org.datavec.api.records.impl.Record(ret,New RecordMetaDataURI([next], GetType(FileRecordReader)))
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

			For Each meta As RecordMetaData In recordMetaDatas
				Dim uri As URI = meta.URI

				Dim list As IList(Of Writable)
				Try
						Using s As Stream = streamCreatorFn.apply(uri)
						list = loadFromStream(uri, s, Charset.forName(charset))
						End Using
				Catch e As IOException
					Throw New Exception("Error reading from stream for URI: " & uri)
				End Try

				[out].Add(New org.datavec.api.records.impl.Record(list, meta))
			Next meta

			Return [out]
		End Function
	End Class

End Namespace