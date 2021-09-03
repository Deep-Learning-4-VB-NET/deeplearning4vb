Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVRegexRecordReader = org.datavec.api.records.reader.impl.csv.CSVRegexRecordReader
Imports FileRecordWriter = org.datavec.api.records.writer.impl.FileRecordWriter
Imports CSVRecordWriter = org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputStreamInputSplit = org.datavec.api.split.InputStreamInputSplit
Imports StringSplit = org.datavec.api.split.StringSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @DisplayName("Csv Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next") void testNext() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNext()
			Dim reader As New CSVRecordReader()
			reader.initialize(New org.datavec.api.Split.StringSplit("1,1,8.0,,,,14.0,,,,15.0,,,,,,,,,,,,1"))
			Do While reader.hasNext()
				Dim vals As IList(Of Writable) = reader.next()
				Dim arr As IList(Of Writable) = New List(Of Writable)(vals)
				assertEquals(23, vals.Count, "Entry count")
				Dim lastEntry As Text = DirectCast(arr(arr.Count - 1), Text)
				assertEquals(1, lastEntry.Length, "Last entry garbage")
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Empty Entries") void testEmptyEntries() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEmptyEntries()
			Dim reader As New CSVRecordReader()
			reader.initialize(New org.datavec.api.Split.StringSplit("1,1,8.0,,,,14.0,,,,15.0,,,,,,,,,,,,"))
			Do While reader.hasNext()
				Dim vals As IList(Of Writable) = reader.next()
				assertEquals(23, vals.Count, "Entry count")
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reset") void testReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReset()
			Dim rr As New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim nResets As Integer = 5
			For i As Integer = 0 To nResets - 1
				Dim lineCount As Integer = 0
				Do While rr.hasNext()
					Dim line As IList(Of Writable) = rr.next()
					assertEquals(5, line.Count)
					lineCount += 1
				Loop
				assertFalse(rr.hasNext())
				assertEquals(150, lineCount)
				rr.reset()
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reset With Skip Lines") void testResetWithSkipLines() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testResetWithSkipLines()
			Dim rr As New CSVRecordReader(10, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim lineCount As Integer = 0
			Do While rr.hasNext()
				rr.next()
				lineCount += 1
			Loop
			assertEquals(140, lineCount)
			rr.reset()
			lineCount = 0
			Do While rr.hasNext()
				rr.next()
				lineCount += 1
			Loop
			assertEquals(140, lineCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Write") void testWrite() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWrite()
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim sb As New StringBuilder()
			For i As Integer = 0 To 9
				Dim temp As IList(Of Writable) = New List(Of Writable)()
				For j As Integer = 0 To 2
					Dim v As Integer = 100 * i + j
					temp.Add(New IntWritable(v))
					sb.Append(v)
					If j < 2 Then
						sb.Append(",")
					ElseIf i <> 9 Then
						sb.Append(vbLf)
					End If
				Next j
				list.Add(temp)
			Next i
			Dim expected As String = sb.ToString()
			Dim p As Path = Files.createTempFile("csvwritetest", "csv")
			p.toFile().deleteOnExit()
			Dim writer As FileRecordWriter = New CSVRecordWriter()
			Dim fileSplit As New org.datavec.api.Split.FileSplit(p.toFile())
			writer.initialize(fileSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
			For Each c As IList(Of Writable) In list
				writer.write(c)
			Next c
			writer.Dispose()
			' Read file back in; compare
			Dim fileContents As String = FileUtils.readFileToString(p.toFile(), FileRecordWriter.DEFAULT_CHARSET.name())
			' System.out.println(expected);
			' System.out.println("----------");
			' System.out.println(fileContents);
			assertEquals(expected, fileContents)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Tabs As Split 1") void testTabsAsSplit1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testTabsAsSplit1()
			Dim reader As New CSVRecordReader(0, ControlChars.Tab)
			reader.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/tabbed.txt")).File))
			Do While reader.hasNext()
				Dim list As IList(Of Writable) = New List(Of Writable)(reader.next())
				assertEquals(2, list.Count)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Pipes As Split") void testPipesAsSplit() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPipesAsSplit()
			Dim reader As New CSVRecordReader(0, "|"c)
			reader.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/issue414.csv")).File))
			Dim lineidx As Integer = 0
			Dim sixthColumn As IList(Of Integer) = New List(Of Integer) From {13, 95, 15, 25}
			Do While reader.hasNext()
				Dim list As IList(Of Writable) = New List(Of Writable)(reader.next())
				assertEquals(10, list.Count)
				assertEquals(CLng(sixthColumn(lineidx)), list(5).toInt())
				lineidx += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test With Quotes") void testWithQuotes() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWithQuotes()
			Dim reader As New CSVRecordReader(0, ","c, """"c)
			reader.initialize(New org.datavec.api.Split.StringSplit("1,0,3,""Braund, Mr. Owen Harris"",male,"""""""""))
			Do While reader.hasNext()
				Dim vals As IList(Of Writable) = reader.next()
				assertEquals(6, vals.Count, "Entry count")
				assertEquals(vals(0).ToString(), "1")
				assertEquals(vals(1).ToString(), "0")
				assertEquals(vals(2).ToString(), "3")
				assertEquals(vals(3).ToString(), "Braund, Mr. Owen Harris")
				assertEquals(vals(4).ToString(), "male")
				assertEquals(vals(5).ToString(), """")
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Meta") void testMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMeta()
			Dim rr As New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim lineCount As Integer = 0
			Dim metaList As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			Dim writables As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				Dim r As Record = rr.nextRecord()
				assertEquals(5, r.getRecord().Count)
				lineCount += 1
				Dim meta As RecordMetaData = r.MetaData
				' System.out.println(r.getRecord() + "\t" + meta.getLocation() + "\t" + meta.getURI());
				metaList.Add(meta)
				writables.Add(r.getRecord())
			Loop
			assertFalse(rr.hasNext())
			assertEquals(150, lineCount)
			rr.reset()
			Console.WriteLine(vbLf & vbLf & vbLf & "--------------------------------")
			Dim contents As IList(Of Record) = rr.loadFromMetaData(metaList)
			assertEquals(150, contents.Count)
			' for(Record r : contents ){
			' System.out.println(r);
			' }
			Dim meta2 As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			meta2.Add(metaList(100))
			meta2.Add(metaList(90))
			meta2.Add(metaList(80))
			meta2.Add(metaList(70))
			meta2.Add(metaList(60))
			Dim contents2 As IList(Of Record) = rr.loadFromMetaData(meta2)
			assertEquals(writables(100), contents2(0).getRecord())
			assertEquals(writables(90), contents2(1).getRecord())
			assertEquals(writables(80), contents2(2).getRecord())
			assertEquals(writables(70), contents2(3).getRecord())
			assertEquals(writables(60), contents2(4).getRecord())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regex") void testRegex() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegex()
			Dim reader As CSVRecordReader = New CSVRegexRecordReader(0, ",", Nothing, New String() { Nothing, "(.+) (.+) (.+)" })
			reader.initialize(New org.datavec.api.Split.StringSplit("normal,1.2.3.4 space separator"))
			Do While reader.hasNext()
				Dim vals As IList(Of Writable) = reader.next()
				assertEquals(4, vals.Count, "Entry count")
				assertEquals(vals(0).ToString(), "normal")
				assertEquals(vals(1).ToString(), "1.2.3.4")
				assertEquals(vals(2).ToString(), "space")
				assertEquals(vals(3).ToString(), "separator")
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Csv Skip All Lines") void testCsvSkipAllLines()
		Friend Overridable Sub testCsvSkipAllLines()
			assertThrows(GetType(NoSuchElementException), Sub()
			Const numLines As Integer = 4
			Dim lineList As IList(Of Writable) = New List(Of Writable) From {DirectCast(New IntWritable(numLines - 1), Writable), DirectCast(New Text("one"), Writable), DirectCast(New Text("two"), Writable), DirectCast(New Text("three"), Writable)}
			Dim header As String = ",one,two,three"
			Dim lines As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To numLines - 1
				lines.Add(Convert.ToString(i) & header)
			Next i
			Dim tempFile As File = File.createTempFile("csvSkipLines", ".csv")
			FileUtils.writeLines(tempFile, lines)
			Dim rr As New CSVRecordReader(numLines, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit(tempFile))
			rr.reset()
			assertTrue(Not rr.hasNext())
			rr.next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Csv Skip All But One Line") void testCsvSkipAllButOneLine() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCsvSkipAllButOneLine()
			Const numLines As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.datavec.api.writable.Writable> lineList = java.util.Arrays.asList<org.datavec.api.writable.Writable>(new org.datavec.api.writable.Text(System.Convert.ToString(numLines - 1)), new org.datavec.api.writable.Text("one"), new org.datavec.api.writable.Text("two"), new org.datavec.api.writable.Text("three"));
			Dim lineList As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim header As String = ",one,two,three"
			Dim lines As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To numLines - 1
				lines.Add(Convert.ToString(i) & header)
			Next i
			Dim tempFile As File = File.createTempFile("csvSkipLines", ".csv")
			FileUtils.writeLines(tempFile, lines)
			Dim rr As New CSVRecordReader(numLines - 1, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit(tempFile))
			rr.reset()
			assertTrue(rr.hasNext())
			assertEquals(rr.next(), lineList)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Stream Reset") void testStreamReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testStreamReset()
			Dim rr As New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.InputStreamInputSplit((New ClassPathResource("datavec-api/iris.dat")).InputStream))
			Dim count As Integer = 0
			Do While rr.hasNext()
				assertNotNull(rr.next())
				count += 1
			Loop
			assertEquals(150, count)
			assertFalse(rr.resetSupported())
			Try
				rr.reset()
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				Dim msg2 As String = e.InnerException.Message
				assertTrue(msg.Contains("Error during LineRecordReader reset"),msg)
				assertTrue(msg2.Contains("Reset not supported from streams"),msg2)
				' e.printStackTrace();
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Useful Exception No Init") void testUsefulExceptionNoInit()
		Friend Overridable Sub testUsefulExceptionNoInit()
			Dim rr As New CSVRecordReader(0, ","c)
			Try
				rr.hasNext()
				fail("Expected exception")
			Catch e As Exception
				assertTrue(e.Message.contains("initialized"),e.Message)
			End Try
			Try
				rr.next()
				fail("Expected exception")
			Catch e As Exception
				assertTrue(e.Message.contains("initialized"),e.Message)
			End Try
		End Sub
	End Class

End Namespace