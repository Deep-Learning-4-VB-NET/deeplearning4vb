Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports ListStringRecordReader = org.datavec.api.records.reader.impl.collection.ListStringRecordReader
Imports ListStringSplit = org.datavec.api.split.ListStringSplit
Imports Schema = org.datavec.api.transform.schema.Schema
Imports TextToCharacterIndexTransform = org.datavec.api.transform.transform.nlp.TextToCharacterIndexTransform
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.datavec.api.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestTransformProcess extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestTransformProcess
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExecution()
		Public Overridable Sub testExecution()

			Dim schema As Schema = (New Schema.Builder()).addColumnsString("col").addColumnsDouble("col2").build()

			Dim m As IDictionary(Of Char, Integer) = defaultCharIndex()
'JAVA TO VB CONVERTER NOTE: The variable transformProcess was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim transformProcess_Conflict As TransformProcess = (New TransformProcess.Builder(schema)).doubleMathOp("col2", MathOp.Add, 1.0).build()

			Dim [in] As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim exp As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			Dim [out] As IList(Of Writable) = transformProcess_Conflict.execute([in])
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExecuteToSequence()
		Public Overridable Sub testExecuteToSequence()

			Dim schema As Schema = (New Schema.Builder()).addColumnsString("action").build()

			Dim m As IDictionary(Of Char, Integer) = defaultCharIndex()
'JAVA TO VB CONVERTER NOTE: The variable transformProcess was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim transformProcess_Conflict As TransformProcess = (New TransformProcess.Builder(schema)).removeAllColumnsExceptFor("action").convertToSequence().transform(New TextToCharacterIndexTransform("action", "action_sequence", m, True)).build()

			Dim s As String = "in text"
			Dim input As IList(Of Writable) = Collections.singletonList(Of Writable)(New Text(s))

			Dim expSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(s.Length)
			For i As Integer = 0 To s.Length - 1
				expSeq.Add(Collections.singletonList(Of Writable)(New IntWritable(m(s.Chars(i)))))
			Next i


			Dim [out] As IList(Of IList(Of Writable)) = transformProcess_Conflict.executeToSequence(input)

			assertEquals(expSeq, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInferColumns() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInferColumns()
			Dim categories As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {Arrays.asList("a","d"), Arrays.asList("b","e"), Arrays.asList("c","f")}

			Dim listReader As RecordReader = New ListStringRecordReader()
			listReader.initialize(New org.datavec.api.Split.ListStringSplit(categories))
			Dim inferredSingle As IList(Of String) = TransformProcess.inferCategories(listReader,0)
			assertEquals(3,inferredSingle.Count)
			listReader.initialize(New org.datavec.api.Split.ListStringSplit(categories))
			Dim integerListMap As IDictionary(Of Integer, IList(Of String)) = TransformProcess.inferCategories(listReader, New Integer(){0, 1})
			For i As Integer = 0 To 1
				assertEquals(3,integerListMap(i).Count)
			Next i
		End Sub


		Public Shared Function defaultCharIndex() As IDictionary(Of Char, Integer)
			Dim ret As IDictionary(Of Char, Integer) = New SortedDictionary(Of Char, Integer)()

			ret("a"c) = 0
			ret("b"c) = 1
			ret("c"c) = 2
			ret("d"c) = 3
			ret("e"c) = 4
			ret("f"c) = 5
			ret("g"c) = 6
			ret("h"c) = 7
			ret("i"c) = 8
			ret("j"c) = 9
			ret("k"c) = 10
			ret("l"c) = 11
			ret("m"c) = 12
			ret("n"c) = 13
			ret("o"c) = 14
			ret("p"c) = 15
			ret("q"c) = 16
			ret("r"c) = 17
			ret("s"c) = 18
			ret("t"c) = 19
			ret("u"c) = 20
			ret("v"c) = 21
			ret("w"c) = 22
			ret("x"c) = 23
			ret("y"c) = 24
			ret("z"c) = 25
			ret("/"c) = 26
			ret(" "c) = 27
			ret("("c) = 28
			ret(")"c) = 29

			Return ret
		End Function



	End Class

End Namespace