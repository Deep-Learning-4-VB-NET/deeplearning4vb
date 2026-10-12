Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports FileSplit = org.datavec.api.split.FileSplit
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
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
Namespace org.datavec.image


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Label Generator Test") @NativeTag @Tag(TagNames.FILE_IO) class LabelGeneratorTest
	Friend Class LabelGeneratorTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Parent Path Label Generator") void testParentPathLabelGenerator(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testParentPathLabelGenerator(ByVal testDir As Path)
			Dim orig As File = (New ClassPathResource("datavec-data-image/testimages/class0/0.jpg")).File
			For Each dirPrefix As String In New String() { "m.", "m" }
				Dim f As File = testDir.resolve("new-dir-" & System.Guid.randomUUID().ToString()).toFile()
				f.mkdirs()
				Dim numDirs As Integer = 3
				Dim filesPerDir As Integer = 4
				For i As Integer = 0 To numDirs - 1
					Dim currentLabelDir As New File(f, dirPrefix & i)
					currentLabelDir.mkdirs()
					For j As Integer = 0 To filesPerDir - 1
						Dim f3 As New File(currentLabelDir, "myImg_" & j & ".jpg")
						FileUtils.copyFile(orig, f3)
						assertTrue(f3.exists())
					Next j
				Next i
				Dim rr As New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
				rr.initialize(New org.datavec.api.Split.FileSplit(f))
				Dim labelsAct As IList(Of String) = rr.getLabels()
				Dim labelsExp As IList(Of String) = New List(Of String) From {dirPrefix & "0", dirPrefix & "1", dirPrefix & "2"}
				assertEquals(labelsExp, labelsAct)
				Dim expCount As Integer = numDirs * filesPerDir
				Dim actCount As Integer = 0
				Do While rr.hasNext()
					rr.next()
					actCount += 1
				Loop
				assertEquals(expCount, actCount)
			Next dirPrefix
		End Sub
	End Class

End Namespace