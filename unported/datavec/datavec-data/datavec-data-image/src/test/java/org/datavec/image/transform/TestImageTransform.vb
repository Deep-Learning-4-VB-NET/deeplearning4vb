Imports System
Imports System.Collections.Generic
Imports UByteIndexer = org.bytedeco.javacpp.indexer.UByteIndexer
Imports CanvasFrame = org.bytedeco.javacv.CanvasFrame
Imports Frame = org.bytedeco.javacv.Frame
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports Tag = org.junit.jupiter.api.Tag
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.nd4j.common.primitives
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports org.bytedeco.opencv.opencv_core
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.bytedeco.opencv.global.opencv_core
Imports org.bytedeco.opencv.global.opencv_imgproc
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

Namespace org.datavec.image.transform


	''' 
	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class TestImageTransform
	Public Class TestImageTransform
		Friend Const seed As Long = 10
		Friend Shared ReadOnly rng As New Random(seed)
		Friend Shared ReadOnly converter As New OpenCVFrameConverter.ToMat()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBoxImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBoxImageTransform()
			Dim transform As ImageTransform = (New BoxImageTransform(rng, 237, 242)).borderValue(Scalar.GRAY)

			For i As Integer = 0 To 99
				Dim writable As ImageWritable = makeRandomImage(0, 0, i Mod 4 + 1)
				Dim frame As Frame = writable.Frame

				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertEquals(237, f.imageWidth)
				assertEquals(242, f.imageHeight)
				assertEquals(frame.imageChannels, f.imageChannels)

				Dim coordinates() As Single = {1, 2, 3, 4, 0, 0}
				Dim transformed() As Single = transform.query(coordinates)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim x As Integer = (frame.imageWidth - f.imageWidth) / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim y As Integer = (frame.imageHeight - f.imageHeight) / 2

				assertEquals(1 - x, transformed(0), 0)
				assertEquals(2 - y, transformed(1), 0)
				assertEquals(3 - x, transformed(2), 0)
				assertEquals(4 - y, transformed(3), 0)
				assertEquals(- x, transformed(4), 0)
				assertEquals(- y, transformed(5), 0)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCropImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCropImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 1)
			Dim frame As Frame = writable.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim transform As ImageTransform = New CropImageTransform(rng, frame.imageHeight / 2, frame.imageWidth / 2, frame.imageHeight / 2, frame.imageWidth / 2)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertTrue(f.imageHeight <= frame.imageHeight)
				assertTrue(f.imageWidth <= frame.imageWidth)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New CropImageTransform(1, 2, 3, 4)
			writable = transform.transform(writable)
			Dim coordinates() As Single = {1, 2, 3, 4}
			Dim transformed() As Single = transform.query(coordinates)
			assertEquals(1 - 2, transformed(0), 0)
			assertEquals(2 - 1, transformed(1), 0)
			assertEquals(3 - 2, transformed(2), 0)
			assertEquals(4 - 1, transformed(3), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFlipImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFlipImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 3)
			Dim frame As Frame = writable.Frame
			Dim transform As ImageTransform = New FlipImageTransform(rng)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertEquals(f.imageHeight, frame.imageHeight)
				assertEquals(f.imageWidth, frame.imageWidth)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New FlipImageTransform(-2)
			writable = transform.transform(writable)
			Dim transformed() As Single = transform.query(New Single() {10, 20})
			assertEquals(10, transformed(0), 0)
			assertEquals(20, transformed(1), 0)

			transform = New FlipImageTransform(0)
			writable = transform.transform(writable)
			transformed = transform.query(New Single() {30, 40})
			assertEquals(30, transformed(0), 0)
			assertEquals(frame.imageHeight - 40 - 1, transformed(1), 0)

			transform = New FlipImageTransform(1)
			writable = transform.transform(writable)
			transformed = transform.query(New Single() {50, 60})
			assertEquals(frame.imageWidth - 50 - 1, transformed(0), 0)
			assertEquals(60, transformed(1), 0)

			transform = New FlipImageTransform(-1)
			writable = transform.transform(writable)
			transformed = transform.query(New Single() {70, 80})
			assertEquals(frame.imageWidth - 70 - 1, transformed(0), 0)
			assertEquals(frame.imageHeight - 80 - 1, transformed(1), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScaleImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScaleImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 4)
			Dim frame As Frame = writable.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim transform As ImageTransform = New ScaleImageTransform(rng, frame.imageWidth / 2, frame.imageHeight / 2)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageHeight >= frame.imageHeight / 2)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageHeight <= 3 * frame.imageHeight / 2)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageWidth >= frame.imageWidth / 2)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageWidth <= 3 * frame.imageWidth / 2)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New ScaleImageTransform(frame.imageWidth, 2 * frame.imageHeight)
			writable = transform.transform(writable)
			Dim coordinates() As Single = {5, 7, 11, 13}
			Dim transformed() As Single = transform.query(coordinates)
			assertEquals(5 * 2, transformed(0), 0)
			assertEquals(7 * 3, transformed(1), 0)
			assertEquals(11 * 2, transformed(2), 0)
			assertEquals(13 * 3, transformed(3), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRotateImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRotateImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 1)
			Dim frame As Frame = writable.Frame
			Dim transform As ImageTransform = (New RotateImageTransform(rng, 180)).interMode(INTER_NEAREST).borderMode(BORDER_REFLECT)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertEquals(f.imageHeight, frame.imageHeight)
				assertEquals(f.imageWidth, frame.imageWidth)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New RotateImageTransform(0, 0, -90, 0)
			writable = transform.transform(writable)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim coordinates() As Single = {frame.imageWidth / 2, frame.imageHeight / 2, 0, 0}
			Dim transformed() As Single = transform.query(coordinates)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			assertEquals(frame.imageWidth / 2, transformed(0), 0)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			assertEquals(frame.imageHeight / 2, transformed(1), 0)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			assertEquals((frame.imageHeight + frame.imageWidth) / 2, transformed(2), 1)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			assertEquals((frame.imageHeight - frame.imageWidth) / 2, transformed(3), 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWarpImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWarpImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 1)
			Dim frame As Frame = writable.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim transform As ImageTransform = (New WarpImageTransform(rng, frame.imageWidth / 10)).interMode(INTER_CUBIC).borderMode(BORDER_REPLICATE)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertEquals(f.imageHeight, frame.imageHeight)
				assertEquals(f.imageWidth, frame.imageWidth)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New WarpImageTransform(1, 2, 3, 4, 5, 6, 7, 8)
			writable = transform.transform(writable)
			Dim coordinates() As Single = { 0, 0, frame.imageWidth, 0, frame.imageWidth, frame.imageHeight, 0, frame.imageHeight}
			Dim transformed() As Single = transform.query(coordinates)
			assertEquals(1, transformed(0), 0)
			assertEquals(2, transformed(1), 0)
			assertEquals(3 + frame.imageWidth, transformed(2), 0)
			assertEquals(4, transformed(3), 0)
			assertEquals(5 + frame.imageWidth, transformed(4), 0)
			assertEquals(6 + frame.imageHeight, transformed(5), 0)
			assertEquals(7, transformed(6), 0)
			assertEquals(8 + frame.imageHeight, transformed(7), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 3)
			Dim frame As Frame = writable.Frame
			Dim transform As ImageTransform = New MultiImageTransform(rng, New CropImageTransform(10), New FlipImageTransform(), New ScaleImageTransform(10), New WarpImageTransform(10))

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertTrue(f.imageHeight >= frame.imageHeight - 30)
				assertTrue(f.imageHeight <= frame.imageHeight + 20)
				assertTrue(f.imageWidth >= frame.imageWidth - 30)
				assertTrue(f.imageWidth <= frame.imageWidth + 20)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New MultiImageTransform(New ColorConversionTransform(COLOR_BGR2RGB))
			writable = transform.transform(writable)
			Dim transformed() As Single = transform.query(New Single() {11, 22})
			assertEquals(11, transformed(0), 0)
			assertEquals(22, transformed(1), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFilterImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFilterImageTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 4)
			Dim frame As Frame = writable.Frame
			Dim transform As ImageTransform = New FilterImageTransform("noise=alls=20:allf=t+u,format=rgba", frame.imageWidth, frame.imageHeight, frame.imageChannels)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
				assertEquals(f.imageHeight, frame.imageHeight)
				assertEquals(f.imageWidth, frame.imageWidth)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testShowImageTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testShowImageTransform()
			If GraphicsEnvironment.isHeadless() Then
				Return
			End If

			Dim writable As ImageWritable = makeRandomImage(0, 0, 3)
			Dim transform As ImageTransform = New ShowImageTransform("testShowImageTransform", 100)

			For i As Integer = 0 To 9
				Dim w As ImageWritable = transform.transform(writable)
				assertEquals(w, writable)
			Next i

			assertEquals(Nothing, transform.transform(Nothing))

			Dim transformed() As Single = transform.query(New Single() {33, 44})
			assertEquals(33, transformed(0), 0)
			assertEquals(44, transformed(1), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvertColorTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConvertColorTransform()
			If GraphicsEnvironment.isHeadless() Then
				Return
			End If

			'        Mat origImage = new Mat();
			'        Mat transImage = new Mat();
			'        OpenCVFrameConverter.ToMat converter = new OpenCVFrameConverter.ToMat();
			Dim writable As ImageWritable = makeRandomImage(32, 32, 3)
			Dim frame As Frame = writable.Frame
			Dim showOrig As ImageTransform = New ShowImageTransform("Original Image", 50)
			showOrig.transform(writable)
			'        origImage = converter.convert(writable.getFrame());

			Dim transform As ImageTransform = New ColorConversionTransform(New Random(42), COLOR_BGR2YCrCb)
			Dim w As ImageWritable = transform.transform(writable)
			Dim showTrans As ImageTransform = New ShowImageTransform("LUV Image", 50)
			showTrans.transform(writable)
			'        transImage = converter.convert(writable.getFrame());

			Dim newframe As Frame = w.Frame
			assertNotEquals(frame, newframe)
			assertEquals(Nothing, transform.transform(Nothing))

			Dim transformed() As Single = transform.query(New Single() {55, 66})
			assertEquals(55, transformed(0), 0)
			assertEquals(66, transformed(1), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHistEqualization() throws org.bytedeco.javacv.CanvasFrame.Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHistEqualization()
			If GraphicsEnvironment.isHeadless() Then
				Return
			End If

			' TODO pull out historgram to confirm equalization...
			Dim writable As ImageWritable = makeRandomImage(32, 32, 3)
			Dim frame As Frame = writable.Frame
			Dim showOrig As ImageTransform = New ShowImageTransform("Original Image", 50)
			showOrig.transform(writable)

			Dim transform As ImageTransform = New EqualizeHistTransform(New Random(42), COLOR_BGR2YCrCb)
			Dim w As ImageWritable = transform.transform(writable)

			Dim showTrans As ImageTransform = New ShowImageTransform("LUV Image", 50)
			showTrans.transform(writable)
			Dim newframe As Frame = w.Frame
			assertNotEquals(frame, newframe)
			assertEquals(Nothing, transform.transform(Nothing))

			Dim transformed() As Single = transform.query(New Single() {66, 77})
			assertEquals(66, transformed(0), 0)
			assertEquals(77, transformed(1), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRandomCropTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRandomCropTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 1)
			Dim frame As Frame = writable.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim transform As ImageTransform = New RandomCropTransform(frame.imageHeight / 2, frame.imageWidth / 2)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageHeight = frame.imageHeight / 2)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageWidth = frame.imageWidth / 2)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New RandomCropTransform(frame.imageHeight, frame.imageWidth)
			writable = transform.transform(writable)
			Dim coordinates() As Single = {2, 4, 6, 8}
			Dim transformed() As Single = transform.query(coordinates)
			assertEquals(2, transformed(0), 0)
			assertEquals(4, transformed(1), 0)
			assertEquals(6, transformed(2), 0)
			assertEquals(8, transformed(3), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testProbabilisticPipelineTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProbabilisticPipelineTransform()
			Dim writable As ImageWritable = makeRandomImage(0, 0, 3)
			Dim frame As Frame = writable.Frame

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim randCrop As ImageTransform = New RandomCropTransform(frame.imageHeight / 2, frame.imageWidth / 2)
			Dim flip As ImageTransform = New FlipImageTransform()
			Dim pipeline As IList(Of Pair(Of ImageTransform, Double)) = New LinkedList(Of Pair(Of ImageTransform, Double))()
			pipeline.Add(New Pair(Of ImageTransform, Double)(randCrop, 1.0))
			pipeline.Add(New Pair(Of ImageTransform, Double)(flip, 0.5))
			Dim transform As ImageTransform = New PipelineImageTransform(pipeline, True)

			For i As Integer = 0 To 99
				Dim w As ImageWritable = transform.transform(writable)
				Dim f As Frame = w.Frame
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageHeight = frame.imageHeight / 2)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				assertTrue(f.imageWidth = frame.imageWidth / 2)
				assertEquals(f.imageChannels, frame.imageChannels)
			Next i
			assertEquals(Nothing, transform.transform(Nothing))

			transform = New PipelineImageTransform(New EqualizeHistTransform())
			writable = transform.transform(writable)
			Dim transformed() As Single = transform.query(New Single() {88, 99})
			assertEquals(88, transformed(0), 0)
			assertEquals(99, transformed(1), 0)
		End Sub

		''' <summary>
		''' This test code is kind of a manual test using specific image(largestblobtest.jpg)
		''' with particular thresholds(blur size, thresholds for edge detector)
		''' The cropped largest blob size should be 74x61
		''' because we use a specific image and thresholds
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLargestBlobCropTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLargestBlobCropTransform()
			If GraphicsEnvironment.isHeadless() Then
				Return
			End If


			Dim f1 As java.io.File = (New ClassPathResource("datavec-data-image/testimages2/largestblobtest.jpg")).File
			Dim loader As New NativeImageLoader()
			Dim writable As ImageWritable = loader.asWritable(f1)

			Dim showOrig As ImageTransform = New ShowImageTransform("Original Image", 50)
			showOrig.transform(writable)

			Dim transform As ImageTransform = New LargestBlobCropTransform(Nothing, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, 3, 3, 100, 300, True)
			Dim w As ImageWritable = transform.transform(writable)

			Dim showTrans As ImageTransform = New ShowImageTransform("Largest Blob", 50)
			showTrans.transform(w)
			Dim newFrame As Frame = w.Frame

			assertEquals(newFrame.imageHeight, 74)
			assertEquals(newFrame.imageWidth, 61)

			Dim transformed() As Single = transform.query(New Single() {88, 32})
			assertEquals(0, transformed(0), 0)
			assertEquals(0, transformed(1), 0)
		End Sub

		Public Shared Function makeRandomImage(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer) As ImageWritable
			If height <= 0 Then
				height = rng.Next() Mod 100 + 200
			End If
			If width <= 0 Then
				width = rng.Next() Mod 100 + 200
			End If
			Dim img As New Mat(height, width, CV_8UC(channels))
			Dim idx As UByteIndexer = img.createIndexer()
			For i As Integer = 0 To height - 1
				For j As Integer = 0 To width - 1
					For k As Integer = 0 To channels - 1
						idx.put(i, j, k, rng.Next())
					Next k
				Next j
			Next i
			Dim frame As Frame = converter.convert(img)
			Return New ImageWritable(frame)
		End Function
	End Class

End Namespace