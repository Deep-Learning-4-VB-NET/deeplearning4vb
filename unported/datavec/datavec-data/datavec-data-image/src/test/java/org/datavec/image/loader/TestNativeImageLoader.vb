Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Loader = org.bytedeco.javacpp.Loader
Imports UByteIndexer = org.bytedeco.javacpp.indexer.UByteIndexer
Imports Frame = org.bytedeco.javacv.Frame
Imports Java2DFrameConverter = org.bytedeco.javacv.Java2DFrameConverter
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports PIX = org.bytedeco.leptonica.PIX
Imports PIXCMAP = org.bytedeco.leptonica.PIXCMAP
Imports Mat = org.bytedeco.opencv.opencv_core.Mat
Imports Image = org.datavec.image.data.Image
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.bytedeco.leptonica.global.lept
Imports org.bytedeco.opencv.global.opencv_core
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
	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class TestNativeImageLoader
	Public Class TestNativeImageLoader
		Friend Const seed As Long = 10
		Friend Shared ReadOnly rng As New Random(seed)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvertPix() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConvertPix()
			Dim pix As PIX
			Dim mat As Mat

			pix = pixCreate(11, 22, 1)
			mat = NativeImageLoader.convert(pix)
			assertEquals(11, mat.cols())
			assertEquals(22, mat.rows())
			assertEquals(CV_8UC1, mat.type())

			pix = pixCreate(33, 44, 2)
			mat = NativeImageLoader.convert(pix)
			assertEquals(33, mat.cols())
			assertEquals(44, mat.rows())
			assertEquals(CV_8UC1, mat.type())

			pix = pixCreate(55, 66, 4)
			mat = NativeImageLoader.convert(pix)
			assertEquals(55, mat.cols())
			assertEquals(66, mat.rows())
			assertEquals(CV_8UC1, mat.type())

			pix = pixCreate(77, 88, 8)
			mat = NativeImageLoader.convert(pix)
			assertEquals(77, mat.cols())
			assertEquals(88, mat.rows())
			assertEquals(CV_8UC1, mat.type())

			pix = pixCreate(99, 111, 16)
			mat = NativeImageLoader.convert(pix)
			assertEquals(99, mat.cols())
			assertEquals(111, mat.rows())
			assertEquals(CV_16UC(1), mat.type())

			pix = pixCreate(222, 333, 24)
			mat = NativeImageLoader.convert(pix)
			assertEquals(222, mat.cols())
			assertEquals(333, mat.rows())
			assertEquals(CV_8UC(3), mat.type())

			pix = pixCreate(444, 555, 32)
			mat = NativeImageLoader.convert(pix)
			assertEquals(444, mat.cols())
			assertEquals(555, mat.rows())
			assertEquals(CV_32FC1, mat.type())

			' a GIF file, for example
			pix = pixCreate(32, 32, 8)
			Dim cmap As PIXCMAP = pixcmapCreateLinear(8, 256)
			pixSetColormap(pix, cmap)
			mat = NativeImageLoader.convert(pix)
			assertEquals(32, mat.cols())
			assertEquals(32, mat.rows())
			assertEquals(CV_8UC4, mat.type())

			Dim w4 As Integer = 100, h4 As Integer = 238, ch4 As Integer = 1, pages As Integer = 1, depth As Integer = 1
			Dim path2MitosisFile As String = "datavec-data-image/testimages2/mitosis.tif"
			Dim loader5 As New NativeImageLoader(h4, w4, ch4, NativeImageLoader.MultiPageMode.FIRST)
			Dim array6 As INDArray = Nothing
			Try
				array6 = loader5.asMatrix((New ClassPathResource(path2MitosisFile)).File.getAbsolutePath())
			Catch e As IOException
				log.error("",e)
				fail()
			End Try
			assertEquals(5, array6.rank())
			assertEquals(pages, array6.size(0))
			assertEquals(ch4, array6.size(1))
			assertEquals(depth, array6.size(2))
			assertEquals(h4, array6.size(3))
			assertEquals(w4, array6.size(4))

	'        int ch5 = 4, pages1 = 1;
	'        NativeImageLoader loader6 = new NativeImageLoader(h4, w4, 1, NativeImageLoader.MultiPageMode.CHANNELS);
	'        loader6.direct = false; // simulate conditions under Android
	'        INDArray array7 = null;
	'        try {
	'            array7 = loader6.asMatrix(
	'                  new ClassPathResource(path2MitosisFile).getFile());
	'        } catch (IOException e) {
	'            e.printStackTrace();
	'        }
	'        assertEquals(5, array7.rank());
	'        assertEquals(pages1, array7.size(0));
	'        assertEquals(ch5, array7.size(1));
	'        assertEquals(depth1, array7.size(2));
	'        assertEquals(h4, array7.size(3));
	'        assertEquals(w4, array7.size(4));

			Dim ch6 As Integer = 1, pages2 As Integer = 4, depth1 As Integer = 1
			Dim loader7 As New NativeImageLoader(h4, w4, ch6, NativeImageLoader.MultiPageMode.MINIBATCH)
			Dim array8 As INDArray = Nothing
			Try
				array8 = loader7.asMatrix((New ClassPathResource(path2MitosisFile)).File.getAbsolutePath())
			Catch e As IOException
				log.error("",e)
			End Try
			assertEquals(5, array8.rank())
			assertEquals(pages2, array8.size(0))
			assertEquals(ch6, array8.size(1))
			assertEquals(depth1, array8.size(2))
			assertEquals(h4, array8.size(3))
			assertEquals(w4, array8.size(4))

			Dim w5 As Integer = 256, h5 As Integer = 256, pages3 As Integer = 2
			Dim braintiff As String = "datavec-data-image/testimages2/3d.tiff" ' this is a 16-bit 3d image
			Dim loader8 As New NativeImageLoader(h5, w5, ch6, NativeImageLoader.MultiPageMode.MINIBATCH)
			Dim array9 As INDArray = Nothing
			Try
				array9 = loader8.asMatrix((New ClassPathResource(braintiff)).File.getAbsolutePath())
			Catch e As IOException
				log.error("",e)
				fail()
			End Try
			assertEquals(5, array9.rank())
			assertEquals(pages3, array9.size(0))
			assertEquals(ch6, array9.size(1))
			assertEquals(depth1, array9.size(2))
			assertEquals(h5, array9.size(3))
			assertEquals(w5, array9.size(4))

	'        int ch8 = 5, pages4 = 1;
	'        NativeImageLoader loader9 = new NativeImageLoader(h5, w5, ch8, NativeImageLoader.MultiPageMode.CHANNELS);
	'        INDArray array10 = null;
	'        try {
	'            array10 = loader9.asMatrix(new ClassPathResource(braintiff).getFile().getAbsolutePath());
	'        } catch (IOException e) {
	'            e.printStackTrace();
	'            fail();
	'        }
	'        assertEquals(5, array10.rank());
	'        assertEquals(pages4, array10.size(0));
	'        assertEquals(ch8, array10.size(1));
	'        assertEquals(depth1, array10.size(2));
	'        assertEquals(h5, array10.size(3));
	'        assertEquals(w5, array10.size(4));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAsRowVector() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAsRowVector()
			Dim img1 As org.opencv.core.Mat = makeRandomOrgOpenCvCoreMatImage(0, 0, 1)
			Dim img2 As Mat = makeRandomImage(0, 0, 3)

			Dim w1 As Integer = 35, h1 As Integer = 79, ch1 As Integer = 3
			Dim loader1 As New NativeImageLoader(h1, w1, ch1)

			Dim array1 As INDArray = loader1.asRowVector(img1)
			assertEquals(2, array1.rank())
			assertEquals(1, array1.rows())
			assertEquals(h1 * w1 * ch1, array1.columns())
			assertNotEquals(0.0, array1.sum().getDouble(0), 0.0)

			Dim array2 As INDArray = loader1.asRowVector(img2)
			assertEquals(2, array2.rank())
			assertEquals(1, array2.rows())
			assertEquals(h1 * w1 * ch1, array2.columns())
			assertNotEquals(0.0, array2.sum().getDouble(0), 0.0)

			Dim w2 As Integer = 103, h2 As Integer = 68, ch2 As Integer = 4
			Dim loader2 As New NativeImageLoader(h2, w2, ch2)
			loader2.direct = False ' simulate conditions under Android

			Dim array3 As INDArray = loader2.asRowVector(img1)
			assertEquals(2, array3.rank())
			assertEquals(1, array3.rows())
			assertEquals(h2 * w2 * ch2, array3.columns())
			assertNotEquals(0.0, array3.sum().getDouble(0), 0.0)

			Dim array4 As INDArray = loader2.asRowVector(img2)
			assertEquals(2, array4.rank())
			assertEquals(1, array4.rows())
			assertEquals(h2 * w2 * ch2, array4.columns())
			assertNotEquals(0.0, array4.sum().getDouble(0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataTypes_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataTypes_1()
			Dim dtypes As val = New DataType(){DataType.FLOAT, DataType.HALF, DataType.SHORT, DataType.INT}

			Dim dt As val = Nd4j.dataType()

			For Each dtype As val In dtypes
				Nd4j.DataType = dtype
				Dim w3 As Integer = 123, h3 As Integer = 77, ch3 As Integer = 3
				Dim loader As val = New NativeImageLoader(h3, w3, ch3)
				Dim f3 As File = (New ClassPathResource("datavec-data-image/testimages/class0/2.jpg")).File
				Dim iw3 As ImageWritable = loader.asWritable(f3)

				Dim array As val = loader.asMatrix(iw3)

				assertEquals(dtype, array.dataType())
			Next dtype

			Nd4j.DataType = dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataTypes_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataTypes_2()
			Dim dtypes As val = New DataType(){DataType.FLOAT, DataType.HALF, DataType.SHORT, DataType.INT}

			Dim dt As val = Nd4j.dataType()

			For Each dtype As val In dtypes
				Nd4j.DataType = dtype
				Dim w3 As Integer = 123, h3 As Integer = 77, ch3 As Integer = 3
				Dim loader As val = New NativeImageLoader(h3, w3, 1)
				Dim f3 As File = (New ClassPathResource("datavec-data-image/testimages/class0/2.jpg")).File
				Dim array As val = loader.asMatrix(f3)

				assertEquals(dtype, array.dataType())
			Next dtype

			Nd4j.DataType = dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAsMatrix() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAsMatrix()
			Dim img1 As BufferedImage = makeRandomBufferedImage(0, 0, 3)
			Dim img2 As Mat = makeRandomImage(0, 0, 4)

			Dim w1 As Integer = 33, h1 As Integer = 77, ch1 As Integer = 1
			Dim loader1 As New NativeImageLoader(h1, w1, ch1)

			Dim array1 As INDArray = loader1.asMatrix(img1)
			assertEquals(4, array1.rank())
			assertEquals(1, array1.size(0))
			assertEquals(1, array1.size(1))
			assertEquals(h1, array1.size(2))
			assertEquals(w1, array1.size(3))
			assertNotEquals(0.0, array1.sum().getDouble(0), 0.0)

			Dim array2 As INDArray = loader1.asMatrix(img2)
			assertEquals(4, array2.rank())
			assertEquals(1, array2.size(0))
			assertEquals(1, array2.size(1))
			assertEquals(h1, array2.size(2))
			assertEquals(w1, array2.size(3))
			assertNotEquals(0.0, array2.sum().getDouble(0), 0.0)

			Dim w2 As Integer = 111, h2 As Integer = 66, ch2 As Integer = 3
			Dim loader2 As New NativeImageLoader(h2, w2, ch2)
			loader2.direct = False ' simulate conditions under Android

			Dim array3 As INDArray = loader2.asMatrix(img1)
			assertEquals(4, array3.rank())
			assertEquals(1, array3.size(0))
			assertEquals(3, array3.size(1))
			assertEquals(h2, array3.size(2))
			assertEquals(w2, array3.size(3))
			assertNotEquals(0.0, array3.sum().getDouble(0), 0.0)

			Dim array4 As INDArray = loader2.asMatrix(img2)
			assertEquals(4, array4.rank())
			assertEquals(1, array4.size(0))
			assertEquals(3, array4.size(1))
			assertEquals(h2, array4.size(2))
			assertEquals(w2, array4.size(3))
			assertNotEquals(0.0, array4.sum().getDouble(0), 0.0)

			Dim w3 As Integer = 123, h3 As Integer = 77, ch3 As Integer = 3
			Dim loader3 As New NativeImageLoader(h3, w3, ch3)
			Dim f3 As File = (New ClassPathResource("datavec-data-image/testimages/class0/2.jpg")).File
			Dim iw3 As ImageWritable = loader3.asWritable(f3)

			Dim array5 As INDArray = loader3.asMatrix(iw3)
			assertEquals(4, array5.rank())
			assertEquals(1, array5.size(0))
			assertEquals(3, array5.size(1))
			assertEquals(h3, array5.size(2))
			assertEquals(w3, array5.size(3))
			assertNotEquals(0.0, array5.sum().getDouble(0), 0.0)

			Dim mat As Mat = loader3.asMat(array5)
			assertEquals(w3, mat.cols())
			assertEquals(h3, mat.rows())
			assertEquals(ch3, mat.channels())
			assertTrue(mat.type() = CV_32FC(ch3) OrElse mat.type() = CV_64FC(ch3))
			assertNotEquals(0.0, sumElems(mat).get(), 0.0)

			Dim frame As Frame = loader3.asFrame(array5, Frame.DEPTH_UBYTE)
			assertEquals(w3, frame.imageWidth)
			assertEquals(h3, frame.imageHeight)
			assertEquals(ch3, frame.imageChannels)
			assertEquals(Frame.DEPTH_UBYTE, frame.imageDepth)

			Dim loader4 As New Java2DNativeImageLoader()
			Dim img12 As BufferedImage = loader4.asBufferedImage(array1)
			assertEquals(array1, loader4.asMatrix(img12))

			Dim loader5 As New NativeImageLoader(0, 0, 0)
			loader5.direct = False ' simulate conditions under Android
			Dim array7 As INDArray = loader5.asMatrix(f3)
			assertEquals(4, array7.rank())
			assertEquals(1, array7.size(0))
			assertEquals(3, array7.size(1))
			assertEquals(32, array7.size(2))
			assertEquals(32, array7.size(3))
			assertNotEquals(0.0, array7.sum().getDouble(0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScalingIfNeed() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScalingIfNeed()
			Dim img1 As Mat = makeRandomImage(0, 0, 1)
			Dim img2 As Mat = makeRandomImage(0, 0, 3)

			Dim w1 As Integer = 60, h1 As Integer = 110, ch1 As Integer = 1
			Dim loader1 As New NativeImageLoader(h1, w1, ch1)

			Dim scaled1 As Mat = loader1.scalingIfNeed(img1)
			assertEquals(h1, scaled1.rows())
			assertEquals(w1, scaled1.cols())
			assertEquals(img1.channels(), scaled1.channels())
			assertNotEquals(0.0, sumElems(scaled1).get(), 0.0)

			Dim scaled2 As Mat = loader1.scalingIfNeed(img2)
			assertEquals(h1, scaled2.rows())
			assertEquals(w1, scaled2.cols())
			assertEquals(img2.channels(), scaled2.channels())
			assertNotEquals(0.0, sumElems(scaled2).get(), 0.0)

			Dim w2 As Integer = 70, h2 As Integer = 120, ch2 As Integer = 3
			Dim loader2 As New NativeImageLoader(h2, w2, ch2)
			loader2.direct = False ' simulate conditions under Android

			Dim scaled3 As Mat = loader2.scalingIfNeed(img1)
			assertEquals(h2, scaled3.rows())
			assertEquals(w2, scaled3.cols())
			assertEquals(img1.channels(), scaled3.channels())
			assertNotEquals(0.0, sumElems(scaled3).get(), 0.0)

			Dim scaled4 As Mat = loader2.scalingIfNeed(img2)
			assertEquals(h2, scaled4.rows())
			assertEquals(w2, scaled4.cols())
			assertEquals(img2.channels(), scaled4.channels())
			assertNotEquals(0.0, sumElems(scaled4).get(), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCenterCropIfNeeded() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCenterCropIfNeeded()
			Dim w1 As Integer = 60, h1 As Integer = 110, ch1 As Integer = 1
			Dim w2 As Integer = 120, h2 As Integer = 70, ch2 As Integer = 3

			Dim img1 As Mat = makeRandomImage(h1, w1, ch1)
			Dim img2 As Mat = makeRandomImage(h2, w2, ch2)

			Dim loader As New NativeImageLoader(h1, w1, ch1, True)

			Dim cropped1 As Mat = loader.centerCropIfNeeded(img1)
			assertEquals(85, cropped1.rows())
			assertEquals(60, cropped1.cols())
			assertEquals(img1.channels(), cropped1.channels())
			assertNotEquals(0.0, sumElems(cropped1).get(), 0.0)

			Dim cropped2 As Mat = loader.centerCropIfNeeded(img2)
			assertEquals(70, cropped2.rows())
			assertEquals(95, cropped2.cols())
			assertEquals(img2.channels(), cropped2.channels())
			assertNotEquals(0.0, sumElems(cropped2).get(), 0.0)
		End Sub


		Friend Overridable Function makeRandomBufferedImage(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer) As BufferedImage
			Dim img As Mat = makeRandomImage(height, width, channels)

			Dim c As New OpenCVFrameConverter.ToMat()
			Dim c2 As New Java2DFrameConverter()

			Return c2.convert(c.convert(img))
		End Function

		Friend Overridable Function makeRandomOrgOpenCvCoreMatImage(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer) As org.opencv.core.Mat
			Dim img As Mat = makeRandomImage(height, width, channels)

			Loader.load(GetType(org.bytedeco.opencv.opencv_java))
			Dim c As New OpenCVFrameConverter.ToOrgOpenCvCoreMat()

			Return c.convert(c.convert(img))
		End Function

		Friend Overridable Function makeRandomImage(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer) As Mat
			If height <= 0 Then
				height = rng.Next() Mod 100 + 100
			End If
			If width <= 0 Then
				width = rng.Next() Mod 100 + 100
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
			Return img
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAsWritable() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAsWritable()
			Dim f0 As String = (New ClassPathResource("datavec-data-image/testimages/class0/0.jpg")).File.getAbsolutePath()

'JAVA TO VB CONVERTER NOTE: The variable imageLoader was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim imageLoader_Conflict As New NativeImageLoader()
			Dim img As ImageWritable = imageLoader_Conflict.asWritable(f0)

			assertEquals(32, img.Frame.imageHeight)
			assertEquals(32, img.Frame.imageWidth)
			assertEquals(3, img.Frame.imageChannels)

			Dim img1 As BufferedImage = makeRandomBufferedImage(0, 0, 3)
			Dim img2 As Mat = makeRandomImage(0, 0, 4)

			Dim w1 As Integer = 33, h1 As Integer = 77, ch1 As Integer = 1
			Dim loader1 As New NativeImageLoader(h1, w1, ch1)

			Dim array1 As INDArray = loader1.asMatrix(f0)
			assertEquals(4, array1.rank())
			assertEquals(1, array1.size(0))
			assertEquals(1, array1.size(1))
			assertEquals(h1, array1.size(2))
			assertEquals(w1, array1.size(3))
			assertNotEquals(0.0, array1.sum().getDouble(0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBufferRealloc() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBufferRealloc()
			Dim f As System.Reflection.FieldInfo = GetType(NativeImageLoader).getDeclaredField("buffer")
			Dim m As System.Reflection.FieldInfo = GetType(NativeImageLoader).getDeclaredField("bufferMat")
			f.setAccessible(True)
			m.setAccessible(True)

			Dim f1 As File = (New ClassPathResource("datavec-data-image/voc/2007/JPEGImages/000005.jpg")).File
			Dim f2 As String = (New ClassPathResource("datavec-data-image/voc/2007/JPEGImages/000007.jpg")).File.getAbsolutePath()

			'Start with a large buffer
			Dim buffer((20*1024*1024) - 1) As SByte
			Dim bufferMat As New Mat(buffer)

			Dim loader As New NativeImageLoader(28, 28, 1)
			f.set(loader, buffer)
			m.set(loader, bufferMat)

			Dim img1LargeBuffer As INDArray = loader.asMatrix(f1)
			Dim img2LargeBuffer As INDArray = loader.asMatrix(f2)

			'Check multiple reads:
			Dim img1LargeBuffer2 As INDArray = loader.asMatrix(f1)
			Dim img1LargeBuffer3 As INDArray = loader.asMatrix(f1)
			assertEquals(img1LargeBuffer2, img1LargeBuffer3)

			Dim img2LargeBuffer2 As INDArray = loader.asMatrix(f1)
			Dim img2LargeBuffer3 As INDArray = loader.asMatrix(f1)
			assertEquals(img2LargeBuffer2, img2LargeBuffer3)

			'Clear the buffer and re-read:
			f.set(loader, Nothing)
			Dim img1NoBuffer1 As INDArray = loader.asMatrix(f1)
			Dim img1NoBuffer2 As INDArray = loader.asMatrix(f1)
			assertEquals(img1LargeBuffer, img1NoBuffer1)
			assertEquals(img1LargeBuffer, img1NoBuffer2)

			f.set(loader, Nothing)
			Dim img2NoBuffer1 As INDArray = loader.asMatrix(f2)
			Dim img2NoBuffer2 As INDArray = loader.asMatrix(f2)
			assertEquals(img2LargeBuffer, img2NoBuffer1)
			assertEquals(img2LargeBuffer, img2NoBuffer2)

			'Assign much too small buffer:
			buffer = New SByte(9){}
			bufferMat = New Mat(buffer)
			f.set(loader, buffer)
			m.set(loader, bufferMat)
			Dim img1SmallBuffer1 As INDArray = loader.asMatrix(f1)
			Dim img1SmallBuffer2 As INDArray = loader.asMatrix(f1)
			assertEquals(img1LargeBuffer, img1SmallBuffer1)
			assertEquals(img1LargeBuffer, img1SmallBuffer2)

			f.set(loader, buffer)
			m.set(loader, bufferMat)
			Dim img2SmallBuffer1 As INDArray = loader.asMatrix(f2)
			Dim img2SmallBuffer2 As INDArray = loader.asMatrix(f2)
			assertEquals(img2LargeBuffer, img2SmallBuffer1)
			assertEquals(img2LargeBuffer, img2SmallBuffer2)

			'Assign an exact buffer:
			Using [is] As Stream = New FileStream(f1, FileMode.Open, FileAccess.Read)
				Dim temp() As SByte = IOUtils.toByteArray([is])
				buffer = New SByte(temp.Length - 1){}
				bufferMat = New Mat(buffer)
			End Using
			f.set(loader, buffer)
			m.set(loader, bufferMat)

			Dim img1ExactBuffer As INDArray = loader.asMatrix(f1)
			assertEquals(img1LargeBuffer, img1ExactBuffer)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNativeImageLoaderEmptyStreams(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNativeImageLoaderEmptyStreams(ByVal testDir As Path)
			Dim dir As File = testDir.toFile()
			Dim f As New File(dir, "myFile.jpg")
			f.createNewFile()

			Dim nil As New NativeImageLoader(32, 32, 3)

			Try
					Using [is] As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
					nil.asMatrix([is])
					fail("Expected exception")
					End Using
			Catch e As IOException
				Dim msg As String = e.Message
				assertTrue(msg.Contains("decode image"),msg)
			End Try

			Try
					Using [is] As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
					nil.asImageMatrix([is])
					fail("Expected exception")
					End Using
			Catch e As IOException
				Dim msg As String = e.Message
				assertTrue(msg.Contains("decode image"),msg)
			End Try

			Try
					Using [is] As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
					nil.asRowVector([is])
					fail("Expected exception")
					End Using
			Catch e As IOException
				Dim msg As String = e.Message
				assertTrue(msg.Contains("decode image"),msg)
			End Try

			Try
					Using [is] As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
					Dim arr As INDArray = Nd4j.create(DataType.FLOAT, 1, 3, 32, 32)
					nil.asMatrixView([is], arr)
					fail("Expected exception")
					End Using
			Catch e As IOException
				Dim msg As String = e.Message
				assertTrue(msg.Contains("decode image"),msg)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNCHW_NHWC() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNCHW_NHWC()
			Dim f As File = Resources.asFile("datavec-data-image/voc/2007/JPEGImages/000005.jpg")

			Dim il As New NativeImageLoader(32, 32, 3)

			'asMatrix(File, boolean)
			Dim a_nchw As INDArray = il.asMatrix(f)
			Dim a_nchw2 As INDArray = il.asMatrix(f, True)
			Dim a_nhwc As INDArray = il.asMatrix(f, False)

			assertEquals(a_nchw, a_nchw2)
			assertEquals(a_nchw, a_nhwc.permute(0,3,1,2))


			'asMatrix(InputStream, boolean)
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				a_nchw = il.asMatrix([is])
			End Using
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				a_nchw2 = il.asMatrix([is], True)
			End Using
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				a_nhwc = il.asMatrix([is], False)
			End Using
			assertEquals(a_nchw, a_nchw2)
			assertEquals(a_nchw, a_nhwc.permute(0,3,1,2))


			'asImageMatrix(File, boolean)
			Dim i_nchw As Image = il.asImageMatrix(f)
			Dim i_nchw2 As Image = il.asImageMatrix(f, True)
			Dim i_nhwc As Image = il.asImageMatrix(f, False)

			assertEquals(i_nchw.getImage(), i_nchw2.getImage())
			assertEquals(i_nchw.getImage(), i_nhwc.getImage().permute(0,3,1,2)) 'NHWC to NCHW


			'asImageMatrix(InputStream, boolean)
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				i_nchw = il.asImageMatrix([is])
			End Using
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				i_nchw2 = il.asImageMatrix([is], True)
			End Using
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				i_nhwc = il.asImageMatrix([is], False)
			End Using
			assertEquals(i_nchw.getImage(), i_nchw2.getImage())
			assertEquals(i_nchw.getImage(), i_nhwc.getImage().permute(0,3,1,2)) 'NHWC to NCHW
		End Sub

	End Class

End Namespace