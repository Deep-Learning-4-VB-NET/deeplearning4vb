Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.datavec.local.transforms.functions



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class TestLineRecordReaderFunction
	Public Class TestLineRecordReaderFunction
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLineRecordReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLineRecordReader()

			Dim dataFile As File = (New ClassPathResource("iris.dat")).File
			Dim lines As IList(Of String) = FileUtils.readLines(dataFile)

			Dim linesRdd As IList(Of String) = (lines)

			Dim rr As New CSVRecordReader(0, ","c)

			Dim [out] As IList(Of IList(Of Writable)) = linesRdd.Select(Function(input) (New LineRecordReaderFunction(rr)).apply(input)).ToList()
			Dim outList As IList(Of IList(Of Writable)) = [out]


			Dim rr2 As New CSVRecordReader(0, ","c)
			rr2.initialize(New org.datavec.api.Split.FileSplit(dataFile))
			Dim expectedSet As ISet(Of IList(Of Writable)) = New HashSet(Of IList(Of Writable))()
			Dim totalCount As Integer = 0
			Do While rr2.hasNext()
				expectedSet.Add(rr2.next())
				totalCount += 1
			Loop

			assertEquals(totalCount, outList.Count)

			For Each line As IList(Of Writable) In outList
				assertTrue(expectedSet.Contains(line))
			Next line
		End Sub
	End Class

End Namespace