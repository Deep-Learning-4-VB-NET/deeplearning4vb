Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.datavec.image.loader


	''' 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class LoaderTests
	Public Class LoaderTests

		Private Shared Sub ensureDataAvailable()
			'Ensure test resources available by initializing CifarLoader and relying on auto download
			Dim preProcessCifar As Boolean = False
			Dim numExamples As Integer = 10
			Dim row As Integer = 28
			Dim col As Integer = 28
			Dim channels As Integer = 1
			For Each train As Boolean In New Boolean(){True, False}
				Dim loader As New CifarLoader(row, col, channels, train, preProcessCifar)
				loader.next(numExamples)
			Next train
			Call (New LFWLoader(New Long() {250, 250, 3}, True)).getRecordReader(1, 1, 1, New Random(42)).next()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLfwReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLfwReader()
			Dim rr As RecordReader = (New LFWLoader(New Long() {250, 250, 3}, True)).getRecordReader(1, 1, 1, New Random(42))
			Dim exptedLabel As IList(Of String) = rr.getLabels()

			Dim rr2 As RecordReader = (New LFWLoader(New Long() {250, 250, 3}, True)).getRecordReader(1, 1, 1, New Random(42))

			assertEquals(exptedLabel(0), rr2.getLabels()(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifarLoader()
		Public Overridable Sub testCifarLoader()
			ensureDataAvailable()
			Dim dir As New File(FilenameUtils.concat(System.getProperty("user.home"), "cifar/cifar-10-batches-bin"))
			Dim cifar As New CifarLoader(False, dir)
			assertTrue(dir.exists())
			assertTrue(cifar.getLabels() IsNot Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifarInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCifarInputStream()
			ensureDataAvailable()
			' check train
			Dim subDir As String = "cifar/cifar-10-batches-bin/data_batch_1.bin"
			Dim path As String = FilenameUtils.concat(System.getProperty("user.home"), subDir)
			Dim fullDataExpected(3072) As SByte
			Dim inExpected As New FileStream(path, FileMode.Open, FileAccess.Read)
			inExpected.Read(fullDataExpected, 0, fullDataExpected.Length)

			Dim fullDataActual(3072) As SByte
			Dim cifarLoad As New CifarLoader(True)
			Dim inActual As Stream = cifarLoad.InputStream
			inActual.Read(fullDataActual, 0, fullDataActual.Length)
			assertEquals(fullDataExpected(0), fullDataActual(0))

			' check test
			subDir = "cifar/cifar-10-batches-bin/test_batch.bin"
			path = FilenameUtils.concat(System.getProperty("user.home"), subDir)
			fullDataExpected = New SByte(3072){}
			inExpected = New FileStream(path, FileMode.Open, FileAccess.Read)
			inExpected.Read(fullDataExpected, 0, fullDataExpected.Length)

			fullDataActual = New SByte(3072){}
			cifarLoad = New CifarLoader(False)
			inActual = cifarLoad.InputStream
			inActual.Read(fullDataActual, 0, fullDataActual.Length)
			assertEquals(fullDataExpected(0), fullDataActual(0))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifarLoaderNext() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCifarLoaderNext()
			Dim train As Boolean = True
			Dim preProcessCifar As Boolean = False
			Dim numExamples As Integer = 10
			Dim row As Integer = 28
			Dim col As Integer = 28
			Dim channels As Integer = 1
			Dim loader As New CifarLoader(row, col, channels, train, preProcessCifar)
			Dim data As DataSet = loader.next(numExamples)
			assertEquals(numExamples, data.Labels.size(0))
			assertEquals(channels, data.Features.size(1))

			train = True
			preProcessCifar = True
			row = 32
			col = 32
			channels = 3
			loader = New CifarLoader(row, col, channels, train, preProcessCifar)
			data = loader.next(1)
			assertEquals(1, data.Features.size(0))
			assertEquals(channels * row * col, data.Features.ravel().length())

			train = False
			preProcessCifar = False
			loader = New CifarLoader(row, col, channels, train, preProcessCifar)
			data = loader.next(numExamples)
			assertEquals(row, data.Features.size(2))

			train = False
			preProcessCifar = True
			loader = New CifarLoader(row, col, channels, train, preProcessCifar)
			data = loader.next(numExamples)
			assertEquals(numExamples, data.Labels.size(0))
			assertEquals(col, data.Features.size(3))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifarLoaderReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCifarLoaderReset()
			Dim numExamples As Integer = 50
			Dim row As Integer = 28
			Dim col As Integer = 28
			Dim channels As Integer = 3
			Dim loader As New CifarLoader(row, col, channels, Nothing, False, False, False)
			Dim data As DataSet
			Dim i As Integer = 0
			Do While i < CifarLoader.NUM_TEST_IMAGES \ numExamples
				loader.next(numExamples)
				i += 1
			Loop
			data = loader.next(numExamples)
			assertEquals(New DataSet(), data)
			loader.reset()
			data = loader.next(numExamples)
			assertEquals(numExamples, data.Labels.size(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifarLoaderWithBiggerImage()
		Public Overridable Sub testCifarLoaderWithBiggerImage()
			Dim train As Boolean = True
			Dim preProcessCifar As Boolean = False
			Dim row As Integer = 128
			Dim col As Integer = 128
			Dim channels As Integer = 3
			Dim numExamples As Integer = 10

			Dim loader As New CifarLoader(row, col, channels, train, preProcessCifar)
			Dim data As DataSet = loader.next(numExamples)
			Dim shape() As Long = data.Features.shape()
			assertEquals(shape.Length, 4)
			assertEquals(shape(0), numExamples)
			assertEquals(shape(1), channels)
			assertEquals(shape(2), row)
			assertEquals(shape(2), col)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void testProcessCifar()
		Public Overridable Sub testProcessCifar()
			Dim row As Integer = 32
			Dim col As Integer = 32
			Dim channels As Integer = 3
			Dim cifar As New CifarLoader(row, col, channels, Nothing, True, True, False)
			Dim result As DataSet = cifar.next(1)
			assertEquals(result.Features.length(), 32 * 32 * 3, 0.0)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifarLoaderExpNumExamples() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCifarLoaderExpNumExamples()
			Dim train As Boolean = True
			Dim preProcessCifar As Boolean = False
			Dim numExamples As Integer = 10
			Dim row As Integer = 28
			Dim col As Integer = 28
			Dim channels As Integer = 1
			Dim loader As New CifarLoader(row, col, channels, train, preProcessCifar)

			Dim minibatch As Integer = 100
			Dim nMinibatches As Integer = 50000 \ minibatch

			For i As Integer = 0 To nMinibatches - 1
				Dim ds As DataSet = loader.next(minibatch)
				Dim s As String = i.ToString()
				assertNotNull(ds.Features,s)
				assertNotNull(ds.Labels,s)

				assertEquals(minibatch, ds.Features.size(0),s)
				assertEquals(minibatch, ds.Labels.size(0),s)
				assertEquals(10, ds.Labels.size(1),s)
			Next i

		End Sub
	End Class

End Namespace