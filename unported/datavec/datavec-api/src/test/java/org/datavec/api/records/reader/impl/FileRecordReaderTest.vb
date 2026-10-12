Imports System.Collections.Generic
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports CollectionInputSplit = org.datavec.api.split.CollectionInputSplit
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
'ORIGINAL LINE: @DisplayName("File Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class FileRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class FileRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reset") void testReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReset()
			Dim rr As New FileRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim nResets As Integer = 5
			For i As Integer = 0 To nResets - 1
				Dim lineCount As Integer = 0
				Do While rr.hasNext()
					Dim line As IList(Of Writable) = rr.next()
					assertEquals(1, line.Count)
					lineCount += 1
				Loop
				assertFalse(rr.hasNext())
				assertEquals(1, lineCount)
				rr.reset()
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Meta") void testMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMeta()
			Dim rr As New FileRecordReader()
			Dim arr(2) As URI
			arr(0) = (New ClassPathResource("datavec-api/csvsequence_0.txt")).File.toURI()
			arr(1) = (New ClassPathResource("datavec-api/csvsequence_1.txt")).File.toURI()
			arr(2) = (New ClassPathResource("datavec-api/csvsequence_2.txt")).File.toURI()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.CollectionInputSplit(Arrays.asList(arr))
			rr.initialize([is])
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				[out].Add(rr.next())
			Loop
			assertEquals(3, [out].Count)
			rr.reset()
			Dim out2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim out3 As IList(Of Record) = New List(Of Record)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			Dim count As Integer = 0
			Do While rr.hasNext()
				Dim r As Record = rr.nextRecord()
				out2.Add(r.getRecord())
				out3.Add(r)
				meta.Add(r.MetaData)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(arr[count++], r.getMetaData().getURI());
				assertEquals(arr(count), r.MetaData.URI)
					count += 1
			Loop
			assertEquals([out], out2)
			Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
			assertEquals(out3, fromMeta)
		End Sub
	End Class

End Namespace