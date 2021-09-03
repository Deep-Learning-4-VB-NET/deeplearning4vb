Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports org.datavec.api.records.reader.impl.csv
Imports FieldSelection = org.datavec.api.records.reader.impl.jackson.FieldSelection
Imports JacksonLineRecordReader = org.datavec.api.records.reader.impl.jackson.JacksonLineRecordReader
Imports JacksonRecordReader = org.datavec.api.records.reader.impl.jackson.JacksonRecordReader
Imports LibSvmRecordReader = org.datavec.api.records.reader.impl.misc.LibSvmRecordReader
Imports SVMLightRecordReader = org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports RegexLineRecordReader = org.datavec.api.records.reader.impl.regex.RegexLineRecordReader
Imports RegexSequenceRecordReader = org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader
Imports TransformProcessRecordReader = org.datavec.api.records.reader.impl.transform.TransformProcessRecordReader
Imports TransformProcessSequenceRecordReader = org.datavec.api.records.reader.impl.transform.TransformProcessSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports JsonFactory = org.nd4j.shade.jackson.core.JsonFactory
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestSerialization extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestSerialization
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRR() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRR()

			Dim rrs As IList(Of RecordReader) = New List(Of RecordReader)()

			rrs.Add(New CSVNLinesSequenceRecordReader(10))
			rrs.Add(New CSVRecordReader(10, ","c))
			rrs.Add(New CSVSequenceRecordReader(1, ","))
			rrs.Add(New CSVVariableSlidingWindowRecordReader(5))
			rrs.Add(New CSVRegexRecordReader(0, ",", Nothing, New String() {Nothing, "(.+) (.+) (.+)"}))
			rrs.Add(New JacksonRecordReader((New FieldSelection.Builder()).addField("a").addField(New Text("MISSING_B"), "b").addField(New Text("MISSING_CX"), "c", "x").build(), New ObjectMapper(New JsonFactory())))
			rrs.Add(New JacksonLineRecordReader((New FieldSelection.Builder()).addField("value1").addField("value2").build(), New ObjectMapper(New JsonFactory())))
			rrs.Add(New LibSvmRecordReader())
			rrs.Add(New SVMLightRecordReader())
			rrs.Add(New RegexLineRecordReader("(.+) (.+) (.+)", 0))
			rrs.Add(New RegexSequenceRecordReader("(.+) (.+) (.+)", 0))
			rrs.Add(New TransformProcessRecordReader(New CSVRecordReader(), Tp))
			rrs.Add(New TransformProcessSequenceRecordReader(New CSVSequenceRecordReader(), Tp))
			rrs.Add(New LineRecordReader())

			For Each r As RecordReader In rrs
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.WriteLine(r.GetType().FullName)
				Dim baos As New MemoryStream()
				Dim os As New ObjectOutputStream(baos)
				os.writeObject(r)
				Dim bytes() As SByte = baos.toByteArray()

				Dim ois As New ObjectInputStream(New MemoryStream(bytes))

				Dim r2 As RecordReader = DirectCast(ois.readObject(), RecordReader)
			Next r
		End Sub

		Private Shared ReadOnly Property Tp As TransformProcess
			Get
				Dim s As Schema = (New Schema.Builder()).addColumnDouble("d").build()
				Dim tp As TransformProcess = (New TransformProcess.Builder(s)).doubleMathFunction("d", MathFunction.ABS).build()
				Return tp
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCsvRRSerializationResults() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCsvRRSerializationResults()
			Dim skipLines As Integer = 3
			Dim r1 As RecordReader = New CSVRecordReader(skipLines, ControlChars.Tab)
			Dim baos As New MemoryStream()
			Dim os As New ObjectOutputStream(baos)
			os.writeObject(r1)
			Dim bytes() As SByte = baos.toByteArray()
			Dim ois As New ObjectInputStream(New MemoryStream(bytes))
			Dim r2 As RecordReader = DirectCast(ois.readObject(), RecordReader)

			Dim f As File = (New ClassPathResource("datavec-api/iris_tab_delim.txt")).File

			r1.initialize(New org.datavec.api.Split.FileSplit(f))
			r2.initialize(New org.datavec.api.Split.FileSplit(f))

			Dim count As Integer = 0
			Do While r1.hasNext()
				Dim n1 As IList(Of Writable) = r1.next()
				Dim n2 As IList(Of Writable) = r2.next()
				assertEquals(n1, n2)
				count += 1
			Loop

			assertEquals(150-skipLines, count)
		End Sub

	End Class

End Namespace