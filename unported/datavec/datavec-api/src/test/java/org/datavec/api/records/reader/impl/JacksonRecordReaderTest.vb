Imports System
Imports System.Collections.Generic
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FieldSelection = org.datavec.api.records.reader.impl.jackson.FieldSelection
Imports JacksonRecordReader = org.datavec.api.records.reader.impl.jackson.JacksonRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports JsonFactory = org.nd4j.shade.jackson.core.JsonFactory
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports XmlFactory = org.nd4j.shade.jackson.dataformat.xml.XmlFactory
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Jackson Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class JacksonRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class JacksonRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reading Json") void testReadingJson(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadingJson(ByVal testDir As Path)
			' Load 3 values from 3 JSON files
			' stricture: a:value, b:value, c:x:value, c:y:value
			' And we want to load only a:value, b:value and c:x:value
			' For first JSON file: all values are present
			' For second JSON file: b:value is missing
			' For third JSON file: c:x:value is missing
			Dim cpr As New ClassPathResource("datavec-api/json/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "json_test_%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2)
			Dim rr As RecordReader = New JacksonRecordReader(FieldSelection, New ObjectMapper(New JsonFactory()))
			rr.initialize([is])
			testJacksonRecordReader(rr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reading Yaml") void testReadingYaml(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadingYaml(ByVal testDir As Path)
			' Exact same information as JSON format, but in YAML format
			Dim cpr As New ClassPathResource("datavec-api/yaml/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "yaml_test_%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2)
			Dim rr As RecordReader = New JacksonRecordReader(FieldSelection, New ObjectMapper(New YAMLFactory()))
			rr.initialize([is])
			testJacksonRecordReader(rr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reading Xml") void testReadingXml(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadingXml(ByVal testDir As Path)
			' Exact same information as JSON format, but in XML format
			Dim cpr As New ClassPathResource("datavec-api/xml/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "xml_test_%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2)
			Dim rr As RecordReader = New JacksonRecordReader(FieldSelection, New ObjectMapper(New XmlFactory()))
			rr.initialize([is])
			testJacksonRecordReader(rr)
		End Sub

		Private Shared ReadOnly Property FieldSelection As FieldSelection
			Get
				Return (New FieldSelection.Builder()).addField("a").addField(New Text("MISSING_B"), "b").addField(New Text("MISSING_CX"), "c", "x").build()
			End Get
		End Property

		Private Shared Sub testJacksonRecordReader(ByVal rr As RecordReader)
			Dim json0 As IList(Of Writable) = rr.next()
			Dim exp0 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("aValue0"), Writable), New Text("bValue0"), New Text("cxValue0")}
			assertEquals(exp0, json0)
			Dim json1 As IList(Of Writable) = rr.next()
			Dim exp1 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("aValue1"), Writable), New Text("MISSING_B"), New Text("cxValue1")}
			assertEquals(exp1, json1)
			Dim json2 As IList(Of Writable) = rr.next()
			Dim exp2 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("aValue2"), Writable), New Text("bValue2"), New Text("MISSING_CX")}
			assertEquals(exp2, json2)
			assertFalse(rr.hasNext())
			' Test reset
			rr.reset()
			assertEquals(exp0, rr.next())
			assertEquals(exp1, rr.next())
			assertEquals(exp2, rr.next())
			assertFalse(rr.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Appending Labels") void testAppendingLabels(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testAppendingLabels(ByVal testDir As Path)
			Dim cpr As New ClassPathResource("datavec-api/json/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "json_test_%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2)
			' Insert at the end:
			Dim rr As RecordReader = New JacksonRecordReader(FieldSelection, New ObjectMapper(New JsonFactory()), False, -1, New LabelGen())
			rr.initialize([is])
			Dim exp0 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("aValue0"), Writable), New Text("bValue0"), New Text("cxValue0"), New IntWritable(0)}
			assertEquals(exp0, rr.next())
			Dim exp1 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("aValue1"), Writable), New Text("MISSING_B"), New Text("cxValue1"), New IntWritable(1)}
			assertEquals(exp1, rr.next())
			Dim exp2 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("aValue2"), Writable), New Text("bValue2"), New Text("MISSING_CX"), New IntWritable(2)}
			assertEquals(exp2, rr.next())
			' Insert at position 0:
			rr = New JacksonRecordReader(FieldSelection, New ObjectMapper(New JsonFactory()), False, -1, New LabelGen(), 0)
			rr.initialize([is])
			exp0 = New List(Of Writable) From {DirectCast(New IntWritable(0), Writable), New Text("aValue0"), New Text("bValue0"), New Text("cxValue0")}
			assertEquals(exp0, rr.next())
			exp1 = New List(Of Writable) From {DirectCast(New IntWritable(1), Writable), New Text("aValue1"), New Text("MISSING_B"), New Text("cxValue1")}
			assertEquals(exp1, rr.next())
			exp2 = New List(Of Writable) From {DirectCast(New IntWritable(2), Writable), New Text("aValue2"), New Text("bValue2"), New Text("MISSING_CX")}
			assertEquals(exp2, rr.next())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Appending Labels Meta Data") void testAppendingLabelsMetaData(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testAppendingLabelsMetaData(ByVal testDir As Path)
			Dim cpr As New ClassPathResource("datavec-api/json/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "json_test_%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2)
			' Insert at the end:
			Dim rr As RecordReader = New JacksonRecordReader(FieldSelection, New ObjectMapper(New JsonFactory()), False, -1, New LabelGen())
			rr.initialize([is])
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				[out].Add(rr.next())
			Loop
			assertEquals(3, [out].Count)
			rr.reset()
			Dim out2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim outRecord As IList(Of Record) = New List(Of Record)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			Do While rr.hasNext()
				Dim r As Record = rr.nextRecord()
				out2.Add(r.getRecord())
				outRecord.Add(r)
				meta.Add(r.MetaData)
			Loop
			assertEquals([out], out2)
			Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
			assertEquals(outRecord, fromMeta)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Label Gen") private static class LabelGen implements org.datavec.api.io.labels.PathLabelGenerator
		<Serializable>
		Private Class LabelGen
			Implements PathLabelGenerator

			Public Overridable Function getLabelForPath(ByVal path As String) As Writable Implements PathLabelGenerator.getLabelForPath
				If path.EndsWith("0.txt", StringComparison.Ordinal) Then
					Return New IntWritable(0)
				ElseIf path.EndsWith("1.txt", StringComparison.Ordinal) Then
					Return New IntWritable(1)
				Else
					Return New IntWritable(2)
				End If
			End Function

			Public Overridable Function getLabelForPath(ByVal uri As URI) As Writable Implements PathLabelGenerator.getLabelForPath
				Return getLabelForPath(uri.getPath())
			End Function

			Public Overridable Function inferLabelClasses() As Boolean Implements PathLabelGenerator.inferLabelClasses
				Return True
			End Function
		End Class
	End Class

End Namespace