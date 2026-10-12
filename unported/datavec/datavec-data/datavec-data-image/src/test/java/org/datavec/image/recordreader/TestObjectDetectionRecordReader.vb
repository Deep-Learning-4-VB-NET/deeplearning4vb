Imports System
Imports System.Collections.Generic
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataImageURI = org.datavec.api.records.metadata.RecordMetaDataImageURI
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CollectionInputSplit = org.datavec.api.split.CollectionInputSplit
Imports FileSplit = org.datavec.api.split.FileSplit
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports ImageObject = org.datavec.image.recordreader.objdetect.ImageObject
Imports ImageObjectLabelProvider = org.datavec.image.recordreader.objdetect.ImageObjectLabelProvider
Imports ObjectDetectionRecordReader = org.datavec.image.recordreader.objdetect.ObjectDetectionRecordReader
Imports FlipImageTransform = org.datavec.image.transform.FlipImageTransform
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports PipelineImageTransform = org.datavec.image.transform.PipelineImageTransform
Imports ResizeImageTransform = org.datavec.image.transform.ResizeImageTransform
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
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

Namespace org.datavec.image.recordreader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestObjectDetectionRecordReader
	Public Class TestObjectDetectionRecordReader
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void test(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub test(ByVal testDir As Path)
			For Each nchw As Boolean In New Boolean(){True, False}
				Dim lp As ImageObjectLabelProvider = New TestImageObjectDetectionLabelProvider()

				Dim f As File = testDir.toFile()
				Call (New ClassPathResource("datavec-data-image/objdetect/")).copyDirectory(f)

				Dim path As String = (New File(f, "000012.jpg")).getParent()

				Dim h As Integer = 32
				Dim w As Integer = 32
				Dim c As Integer = 3
				Dim gW As Integer = 13
				Dim gH As Integer = 10

				'Enforce consistent iteration order for tests
				Dim u() As URI = (New org.datavec.api.Split.FileSplit(New File(path))).locations()
				Array.Sort(u)

				Dim rr As RecordReader = New ObjectDetectionRecordReader(h, w, c, gH, gW, nchw, lp)
				rr.initialize(New org.datavec.api.Split.CollectionInputSplit(u))

				Dim imgRR As RecordReader = New ImageRecordReader(h, w, c, nchw)
				imgRR.initialize(New org.datavec.api.Split.CollectionInputSplit(u))

				Dim labels As IList(Of String) = rr.getLabels()
				assertEquals(Arrays.asList("car", "cat"), labels)


				'000012.jpg - originally 500x333
				'000019.jpg - originally 500x375
				Dim origW() As Double = {500, 500}
				Dim origH() As Double = {333, 375}
				Dim l As IList(Of IList(Of ImageObject)) = New List(Of IList(Of ImageObject)) From {Collections.singletonList(New ImageObject(156, 97, 351, 270, "car")), Arrays.asList(New ImageObject(11, 113, 266, 259, "cat"), New ImageObject(231, 88, 483, 256, "cat"))}

				For idx As Integer = 0 To 1
					assertTrue(rr.hasNext())
					Dim [next] As IList(Of Writable) = rr.next()
					Dim nextImgRR As IList(Of Writable) = imgRR.next()

					'Check features:
					assertEquals([next](0), nextImgRR(0))

					'Check labels
					assertEquals(2, [next].Count)
					assertTrue(TypeOf [next](0) Is NDArrayWritable)
					assertTrue(TypeOf [next](1) Is NDArrayWritable)

					Dim objects As IList(Of ImageObject) = l(idx)

					Dim expLabels As INDArray = Nd4j.create(1, 4 + 2, gH, gW)
					For Each io As ImageObject In objects
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim fracImageX1 As Double = io.getX1() / origW(idx)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim fracImageY1 As Double = io.getY1() / origH(idx)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim fracImageX2 As Double = io.getX2() / origW(idx)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim fracImageY2 As Double = io.getY2() / origH(idx)

						Dim x1C As Double = (fracImageX1 + fracImageX2) / 2.0
						Dim y1C As Double = (fracImageY1 + fracImageY2) / 2.0

						Dim labelGridX As Integer = CInt(Math.Truncate(x1C * gW))
						Dim labelGridY As Integer = CInt(Math.Truncate(y1C * gH))

						Dim labelIdx As Integer
						If io.getLabel().Equals("car") Then
							labelIdx = 4
						Else
							labelIdx = 5
						End If
						expLabels.putScalar(0, labelIdx, labelGridY, labelGridX, 1.0)

						expLabels.putScalar(0, 0, labelGridY, labelGridX, fracImageX1 * gW)
						expLabels.putScalar(0, 1, labelGridY, labelGridX, fracImageY1 * gH)
						expLabels.putScalar(0, 2, labelGridY, labelGridX, fracImageX2 * gW)
						expLabels.putScalar(0, 3, labelGridY, labelGridX, fracImageY2 * gH)
					Next io

					Dim lArr As INDArray = DirectCast([next](1), NDArrayWritable).get()
					If nchw Then
						assertArrayEquals(New Long(){1, 4 + 2, gH, gW}, lArr.shape())
					Else
						assertArrayEquals(New Long(){1, gH, gW, 4 + 2}, lArr.shape())
					End If

					If Not nchw Then
						expLabels = expLabels.permute(0,2,3,1) 'NCHW to NHWC
					End If

					assertEquals(expLabels, lArr)
				Next idx

				rr.reset()
				Dim record As Record = rr.nextRecord()
				Dim metadata As RecordMetaDataImageURI = DirectCast(record.MetaData, RecordMetaDataImageURI)
				assertEquals(New File(path, "000012.jpg"), New File(metadata.URI))
				assertEquals(3, metadata.getOrigC())
				assertEquals(CInt(Math.Truncate(origH(0))), metadata.getOrigH())
				assertEquals(CInt(Math.Truncate(origW(0))), metadata.getOrigW())

				Dim [out] As IList(Of Record) = New List(Of Record)()
				Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
				[out].Add(record)
				meta.Add(metadata)
				record = rr.nextRecord()
				metadata = DirectCast(record.MetaData, RecordMetaDataImageURI)
				[out].Add(record)
				meta.Add(metadata)

				Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
				assertEquals([out], fromMeta)

				' make sure we don't lose objects just by explicitly resizing
				Dim i As Integer = 0
				Dim nonzeroCount() As Integer = {5, 10}

				Dim transform As ImageTransform = New ResizeImageTransform(37, 42)
				Dim rrTransform As RecordReader = New ObjectDetectionRecordReader(42, 37, c, gH, gW, nchw, lp, transform)
				rrTransform.initialize(New org.datavec.api.Split.CollectionInputSplit(u))
				i = 0
				Do While rrTransform.hasNext()
					Dim [next] As IList(Of Writable) = rrTransform.next()
					assertEquals(37, transform.CurrentImage.Width)
					assertEquals(42, transform.CurrentImage.Height)
					Dim labelArray As INDArray = DirectCast([next](1), NDArrayWritable).get()
					BooleanIndexing.replaceWhere(labelArray, 1, Conditions.notEquals(0))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(nonzeroCount[i++], labelArray.sum().getInt(0));
					assertEquals(nonzeroCount(i), labelArray.sum().getInt(0))
						i += 1
				Loop

				Dim transform2 As ImageTransform = New ResizeImageTransform(1024, 2048)
				Dim rrTransform2 As RecordReader = New ObjectDetectionRecordReader(2048, 1024, c, gH, gW, nchw, lp, transform2)
				rrTransform2.initialize(New org.datavec.api.Split.CollectionInputSplit(u))
				i = 0
				Do While rrTransform2.hasNext()
					Dim [next] As IList(Of Writable) = rrTransform2.next()
					assertEquals(1024, transform2.CurrentImage.Width)
					assertEquals(2048, transform2.CurrentImage.Height)
					Dim labelArray As INDArray = DirectCast([next](1), NDArrayWritable).get()
					BooleanIndexing.replaceWhere(labelArray, 1, Conditions.notEquals(0))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(nonzeroCount[i++], labelArray.sum().getInt(0));
					assertEquals(nonzeroCount(i), labelArray.sum().getInt(0))
						i += 1
				Loop

				'Make sure image flip does not break labels and are correct for new image size dimensions:
				Dim transform3 As ImageTransform = New PipelineImageTransform(New ResizeImageTransform(2048, 4096), New FlipImageTransform(-1))
				Dim rrTransform3 As RecordReader = New ObjectDetectionRecordReader(2048, 1024, c, gH, gW, nchw, lp, transform3)
				rrTransform3.initialize(New org.datavec.api.Split.CollectionInputSplit(u))
				i = 0
				Do While rrTransform3.hasNext()
					Dim [next] As IList(Of Writable) = rrTransform3.next()
					Dim labelArray As INDArray = DirectCast([next](1), NDArrayWritable).get()
					BooleanIndexing.replaceWhere(labelArray, 1, Conditions.notEquals(0))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(nonzeroCount[i++], labelArray.sum().getInt(0));
					assertEquals(nonzeroCount(i), labelArray.sum().getInt(0))
						i += 1
				Loop

				'Test that doing a downscale with the native image loader directly instead of a transform does not cause an exception:
				Dim transform4 As ImageTransform = New FlipImageTransform(-1)
				Dim rrTransform4 As RecordReader = New ObjectDetectionRecordReader(128, 128, c, gH, gW, nchw, lp, transform4)
				rrTransform4.initialize(New org.datavec.api.Split.CollectionInputSplit(u))
				i = 0
				Do While rrTransform4.hasNext()
					Dim [next] As IList(Of Writable) = rrTransform4.next()

					assertEquals(CInt(Math.Truncate(origW(i))), transform4.CurrentImage.Width)
					assertEquals(CInt(Math.Truncate(origH(i))), transform4.CurrentImage.Height)

					Dim labelArray As INDArray = DirectCast([next](1), NDArrayWritable).get()
					BooleanIndexing.replaceWhere(labelArray, 1, Conditions.notEquals(0))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(nonzeroCount[i++], labelArray.sum().getInt(0));
					assertEquals(nonzeroCount(i), labelArray.sum().getInt(0))
						i += 1
				Loop

			Next nchw
		End Sub

		'2 images: 000012.jpg and 000019.jpg
		Private Class TestImageObjectDetectionLabelProvider
			Implements ImageObjectLabelProvider

			Public Overridable Function getImageObjectsForPath(ByVal uri As URI) As IList(Of ImageObject) Implements ImageObjectLabelProvider.getImageObjectsForPath
				Return getImageObjectsForPath(uri.getPath())
			End Function

			Public Overridable Function getImageObjectsForPath(ByVal path As String) As IList(Of ImageObject) Implements ImageObjectLabelProvider.getImageObjectsForPath
				If path.EndsWith("000012.jpg", StringComparison.Ordinal) Then
					Return Collections.singletonList(New ImageObject(156, 97, 351, 270, "car"))
				ElseIf path.EndsWith("000019.jpg", StringComparison.Ordinal) Then
					Return New List(Of ImageObject) From {
						New ImageObject(11, 113, 266, 259, "cat"),
						New ImageObject(231, 88, 483, 256, "cat")
					}
				Else
					Throw New Exception()
				End If
			End Function
		End Class
	End Class

End Namespace