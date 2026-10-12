Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports org.nd4j.common.function
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.datavec.api.split


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestStreamInputSplit extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestStreamInputSplit
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCsvSimple(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCsvSimple(ByVal testDir As Path)
			Dim dir As File = testDir.toFile()
			Dim f1 As New File(dir, "file1.txt")
			Dim f2 As New File(dir, "file2.txt")

			FileUtils.writeStringToFile(f1, "a,b,c" & vbLf & "d,e,f", StandardCharsets.UTF_8)
			FileUtils.writeStringToFile(f2, "1,2,3", StandardCharsets.UTF_8)

			Dim uris As IList(Of URI) = New List(Of URI) From {f1.toURI(), f2.toURI()}

			Dim rr As New CSVRecordReader()

			Dim fn As New TestStreamFunction()
			Dim [is] As InputSplit = New StreamInputSplit(uris, fn)
			rr.initialize([is])

			Dim exp As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp.Add(New List(Of Writable) From {Of Writable})
			exp.Add(New List(Of Writable) From {Of Writable})
			exp.Add(New List(Of Writable) From {Of Writable})

			Dim act As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				act.Add(rr.next())
			Loop

			assertEquals(exp, act)

			'Check that the specified stream loading function was used, not the default:
			assertEquals(uris, fn.calledWithUris)

			rr.reset()
			Dim count As Integer = 0
			Do While rr.hasNext()
				count += 1
				rr.next()
			Loop
			assertEquals(3, count)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCsvSequenceSimple(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCsvSequenceSimple(ByVal testDir As Path)

			Dim dir As File = testDir.toFile()
			Dim f1 As New File(dir, "file1.txt")
			Dim f2 As New File(dir, "file2.txt")

			FileUtils.writeStringToFile(f1, "a,b,c" & vbLf & "d,e,f", StandardCharsets.UTF_8)
			FileUtils.writeStringToFile(f2, "1,2,3", StandardCharsets.UTF_8)

			Dim uris As IList(Of URI) = New List(Of URI) From {f1.toURI(), f2.toURI()}

			Dim rr As New CSVSequenceRecordReader()

			Dim fn As New TestStreamFunction()
			Dim [is] As InputSplit = New StreamInputSplit(uris, fn)
			rr.initialize([is])

			Dim exp As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			exp.Add(New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New Text("b"), New Text("c")), Arrays.asList(Of Writable)(New Text("d"), New Text("e"), New Text("f"))})
			exp.Add(New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("1"), New Text("2"), New Text("3"))})

			Dim act As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While rr.hasNext()
				act.Add(rr.sequenceRecord())
			Loop

			assertEquals(exp, act)

			'Check that the specified stream loading function was used, not the default:
			assertEquals(uris, fn.calledWithUris)

			rr.reset()
			Dim count As Integer = 0
			Do While rr.hasNext()
				count += 1
				rr.sequenceRecord()
			Loop
			assertEquals(2, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testShuffle(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testShuffle(ByVal testDir As Path)
			Dim dir As File = testDir.toFile()
			Dim f1 As New File(dir, "file1.txt")
			Dim f2 As New File(dir, "file2.txt")
			Dim f3 As New File(dir, "file3.txt")

			FileUtils.writeStringToFile(f1, "a,b,c", StandardCharsets.UTF_8)
			FileUtils.writeStringToFile(f2, "1,2,3", StandardCharsets.UTF_8)
			FileUtils.writeStringToFile(f3, "x,y,z", StandardCharsets.UTF_8)

			Dim uris As IList(Of URI) = New List(Of URI) From {f1.toURI(), f2.toURI(), f3.toURI()}

			Dim rr As New CSVSequenceRecordReader()

			Dim fn As New TestStreamFunction()
			Dim [is] As InputSplit = New StreamInputSplit(uris, fn, New Random(12345))
			rr.initialize([is])

			Dim act As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While rr.hasNext()
				act.Add(rr.sequenceRecord())
			Loop

			rr.reset()
			Dim act2 As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While rr.hasNext()
				act2.Add(rr.sequenceRecord())
			Loop

			rr.reset()
			Dim act3 As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While rr.hasNext()
				act3.Add(rr.sequenceRecord())
			Loop

			assertEquals(3, act.Count)
			assertEquals(3, act2.Count)
			assertEquals(3, act3.Count)

	'        
	'        System.out.println(act);
	'        System.out.println("---------");
	'        System.out.println(act2);
	'        System.out.println("---------");
	'        System.out.println(act3);
	'        

			'Check not the same. With this RNG seed, results are different for first 3 resets
			assertNotEquals(act, act2)
			assertNotEquals(act2, act3)
			assertNotEquals(act, act3)
		End Sub


		Public Class TestStreamFunction
			Implements [Function](Of URI, Stream)

			Public calledWithUris As IList(Of URI) = New List(Of URI)()
			Public Overridable Function apply(ByVal uri As URI) As Stream
				calledWithUris.Add(uri) 'Just for testing to ensure function is used
				Try
					Return New FileStream(uri, FileMode.Open, FileAccess.Read)
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End Function
		End Class
	End Class

End Namespace