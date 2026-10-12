Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports FileBatchRecordReader = org.datavec.api.records.reader.impl.filebatch.FileBatchRecordReader
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports FileBatch = org.nd4j.common.loader.FileBatch
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.junit.jupiter.api.Assertions
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
Namespace org.datavec.image.recordreader

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("File Batch Record Reader Test") @NativeTag @Tag(TagNames.FILE_IO) class FileBatchRecordReaderTest
	Friend Class FileBatchRecordReaderTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Csv") void testCsv(@TempDir Path testDir,@TempDir Path baseDirPath) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCsv(ByVal testDir As Path, ByVal baseDirPath As Path)
			Dim extractedSourceDir As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/testimages")).copyDirectory(extractedSourceDir)
			Dim baseDir As File = baseDirPath.toFile()
			Dim c As IList(Of File) = New List(Of File)(FileUtils.listFiles(extractedSourceDir, Nothing, True))
			assertEquals(6, c.Count)
			c.Sort(New ComparatorAnonymousInnerClass(Me))
			Dim fb As FileBatch = FileBatch.forFiles(c)
			Dim saveFile As New File(baseDir, "saved.zip")
			fb.writeAsZip(saveFile)
			fb = FileBatch.readFromZip(saveFile)
			Dim labelMaker As PathLabelGenerator = New ParentPathLabelGenerator()
			Dim rr As New ImageRecordReader(32, 32, 1, labelMaker)
			rr.Labels = New List(Of String) From {"class0", "class1"}
			Dim fbrr As New FileBatchRecordReader(rr, fb)
			Dim il As New NativeImageLoader(32, 32, 1)
			For test As Integer = 0 To 2
				For i As Integer = 0 To 5
					assertTrue(fbrr.hasNext())
					Dim [next] As IList(Of Writable) = fbrr.next()
					assertEquals(2, [next].Count)
					Dim exp As INDArray
					Select Case i
						Case 0
							exp = il.asMatrix(New File(extractedSourceDir, "class0/0.jpg"))
						Case 1
							exp = il.asMatrix(New File(extractedSourceDir, "class0/1.png"))
						Case 2
							exp = il.asMatrix(New File(extractedSourceDir, "class0/2.jpg"))
						Case 3
							exp = il.asMatrix(New File(extractedSourceDir, "class1/A.jpg"))
						Case 4
							exp = il.asMatrix(New File(extractedSourceDir, "class1/B.png"))
						Case 5
							exp = il.asMatrix(New File(extractedSourceDir, "class1/C.jpg"))
						Case Else
							Throw New Exception()
					End Select
					Dim expLabel As Writable = (If(i < 3, New IntWritable(0), New IntWritable(1)))
					assertEquals(DirectCast([next](0), NDArrayWritable).get(), exp)
					assertEquals(expLabel, [next](1))
				Next i
				assertFalse(fbrr.hasNext())
				assertTrue(fbrr.resetSupported())
				fbrr.reset()
			Next test
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of File)

			Private ReadOnly outerInstance As FileBatchRecordReaderTest

			Public Sub New(ByVal outerInstance As FileBatchRecordReaderTest)
				Me.outerInstance = outerInstance
			End Sub


			Public Function Compare(ByVal o1 As File, ByVal o2 As File) As Integer Implements IComparer(Of File).Compare
				Return o1.getPath().compareTo(o2.getPath())
			End Function
		End Class
	End Class

End Namespace