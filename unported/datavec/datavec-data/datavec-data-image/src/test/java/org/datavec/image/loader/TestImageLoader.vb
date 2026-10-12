Imports System
Imports System.IO
Imports Image = org.datavec.image.data.Image
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.junit.jupiter.api.Assertions.assertEquals

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class TestImageLoader
	Public Class TestImageLoader

		Private Shared seed As Long = 10
		Private Shared rng As New Random(seed)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToIntArrayArray() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testToIntArrayArray()
			Dim img As BufferedImage = makeRandomBufferedImage(True)

			Dim w As Integer = img.getWidth()
			Dim h As Integer = img.getHeight()
			Dim ch As Integer = 4
			Dim loader As New ImageLoader(0, 0, ch)
			Dim arr()() As Integer = loader.toIntArrayArray(img)

			assertEquals(h, arr.Length)
			assertEquals(w, arr(0).Length)

			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					assertEquals(img.getRGB(j, i), arr(i)(j))
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToINDArrayBGR() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testToINDArrayBGR()
			Dim img As BufferedImage = makeRandomBufferedImage(False)
			Dim w As Integer = img.getWidth()
			Dim h As Integer = img.getHeight()
			Dim ch As Integer = 3

			Dim loader As New ImageLoader(0, 0, ch)
			Dim arr As INDArray = loader.toINDArrayBGR(img)

			Dim shape() As Long = arr.shape()
			assertEquals(3, shape.Length)
			assertEquals(ch, shape(0))
			assertEquals(h, shape(1))
			assertEquals(w, shape(2))

			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					Dim srcColor As Integer = img.getRGB(j, i)
					Dim a As Integer = &Hff << 24
					Dim r As Integer = arr.getInt(2, i, j) << 16
					Dim g As Integer = arr.getInt(1, i, j) << 8
					Dim b As Integer = arr.getInt(0, i, j) And &Hff
					Dim dstColor As Integer = a Or r Or g Or b
					assertEquals(srcColor, dstColor)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScalingIfNeed() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScalingIfNeed()
			Dim img1 As BufferedImage = makeRandomBufferedImage(True)
			Dim img2 As BufferedImage = makeRandomBufferedImage(False)

			Dim w1 As Integer = 60, h1 As Integer = 110, ch1 As Integer = 6
			Dim loader1 As New ImageLoader(h1, w1, ch1)

			Dim scaled1 As BufferedImage = loader1.scalingIfNeed(img1, True)
			assertEquals(w1, scaled1.getWidth())
			assertEquals(h1, scaled1.getHeight())
			assertEquals(BufferedImage.TYPE_4BYTE_ABGR, scaled1.getType())
			assertEquals(4, scaled1.getSampleModel().getNumBands())

			Dim scaled2 As BufferedImage = loader1.scalingIfNeed(img1, False)
			assertEquals(w1, scaled2.getWidth())
			assertEquals(h1, scaled2.getHeight())
			assertEquals(BufferedImage.TYPE_3BYTE_BGR, scaled2.getType())
			assertEquals(3, scaled2.getSampleModel().getNumBands())

			Dim scaled3 As BufferedImage = loader1.scalingIfNeed(img2, True)
			assertEquals(w1, scaled3.getWidth())
			assertEquals(h1, scaled3.getHeight())
			assertEquals(BufferedImage.TYPE_3BYTE_BGR, scaled3.getType())
			assertEquals(3, scaled3.getSampleModel().getNumBands())

			Dim scaled4 As BufferedImage = loader1.scalingIfNeed(img2, False)
			assertEquals(w1, scaled4.getWidth())
			assertEquals(h1, scaled4.getHeight())
			assertEquals(BufferedImage.TYPE_3BYTE_BGR, scaled4.getType())
			assertEquals(3, scaled4.getSampleModel().getNumBands())

			Dim w2 As Integer = 70, h2 As Integer = 120, ch2 As Integer = 6
			Dim loader2 As New ImageLoader(h2, w2, ch2)

			Dim scaled5 As BufferedImage = loader2.scalingIfNeed(img1, True)
			assertEquals(w2, scaled5.getWidth())
			assertEquals(h2, scaled5.getHeight(), h2)
			assertEquals(BufferedImage.TYPE_4BYTE_ABGR, scaled5.getType())
			assertEquals(4, scaled5.getSampleModel().getNumBands())

			Dim scaled6 As BufferedImage = loader2.scalingIfNeed(img1, False)
			assertEquals(w2, scaled6.getWidth())
			assertEquals(h2, scaled6.getHeight())
			assertEquals(BufferedImage.TYPE_3BYTE_BGR, scaled6.getType())
			assertEquals(3, scaled6.getSampleModel().getNumBands())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScalingIfNeedWhenSuitableSizeButDiffChannel()
		Public Overridable Sub testScalingIfNeedWhenSuitableSizeButDiffChannel()
			Dim width1 As Integer = 60
			Dim height1 As Integer = 110
			Dim channel1 As Integer = BufferedImage.TYPE_BYTE_GRAY
			Dim img1 As BufferedImage = makeRandomBufferedImage(True, width1, height1)
			Dim loader1 As New ImageLoader(height1, width1, channel1)
			Dim scaled1 As BufferedImage = loader1.scalingIfNeed(img1, False)
			assertEquals(width1, scaled1.getWidth())
			assertEquals(height1, scaled1.getHeight())
			assertEquals(channel1, scaled1.getType())
			assertEquals(1, scaled1.getSampleModel().getNumBands())

			Dim width2 As Integer = 70
			Dim height2 As Integer = 120
			Dim channel2 As Integer = BufferedImage.TYPE_BYTE_GRAY
			Dim img2 As BufferedImage = makeRandomBufferedImage(False, width2, height2)
			Dim loader2 As New ImageLoader(height2, width2, channel2)
			Dim scaled2 As BufferedImage = loader2.scalingIfNeed(img2, False)
			assertEquals(width2, scaled2.getWidth())
			assertEquals(height2, scaled2.getHeight())
			assertEquals(channel2, scaled2.getType())
			assertEquals(1, scaled2.getSampleModel().getNumBands())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToBufferedImageRGB()
		Public Overridable Sub testToBufferedImageRGB()
			Dim img As BufferedImage = makeRandomBufferedImage(False)
			Dim w As Integer = img.getWidth()
			Dim h As Integer = img.getHeight()
			Dim ch As Integer = 3

			Dim loader As New ImageLoader(0, 0, ch)
			Dim arr As INDArray = loader.toINDArrayBGR(img)
			Dim img2 As New BufferedImage(w, h, BufferedImage.TYPE_3BYTE_BGR)
			loader.toBufferedImageRGB(arr, img2)

			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					Dim srcColor As Integer = img.getRGB(j, i)
					Dim restoredColor As Integer = img2.getRGB(j, i)
					assertEquals(srcColor, restoredColor)
				Next j
			Next i

		End Sub

		''' <summary>
		''' Generate a Random BufferedImage with specified width and height
		''' </summary>
		''' <param name="alpha">  Is image alpha </param>
		''' <param name="width">  Proposed width </param>
		''' <param name="height"> Proposed height </param>
		''' <returns> Generated BufferedImage </returns>
		Private Function makeRandomBufferedImage(ByVal alpha As Boolean, ByVal width As Integer, ByVal height As Integer) As BufferedImage
			Dim type As Integer = If(alpha, BufferedImage.TYPE_4BYTE_ABGR, BufferedImage.TYPE_3BYTE_BGR)
			Dim img As New BufferedImage(width, height, type)
			For i As Integer = 0 To height - 1
				For j As Integer = 0 To width - 1
					Dim a As Integer = (If(alpha, rng.Next(), 1)) And &Hff
					Dim r As Integer = rng.Next() And &Hff
					Dim g As Integer = rng.Next() And &Hff
					Dim b As Integer = rng.Next() And &Hff
					Dim v As Integer = (a << 24) Or (r << 16) Or (g << 8) Or b
					img.setRGB(j, i, v)
				Next j
			Next i
			Return img
		End Function

		''' <summary>
		''' Generate a Random BufferedImage with random width and height
		''' </summary>
		''' <param name="alpha"> Is image alpha </param>
		''' <returns> Generated BufferedImage </returns>
		Private Function makeRandomBufferedImage(ByVal alpha As Boolean) As BufferedImage
			Return makeRandomBufferedImage(alpha, rng.Next() Mod 100 + 100, rng.Next() Mod 100 + 100)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNCHW_NHWC() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNCHW_NHWC()
			Dim f As File = Resources.asFile("datavec-data-image/voc/2007/JPEGImages/000005.jpg")

			Dim il As New ImageLoader(32, 32, 3)

			'asMatrix(File, boolean)
			Dim a_nchw As INDArray = il.asMatrix(f)
			Dim a_nchw2 As INDArray = il.asMatrix(f, True)
			Dim a_nhwc As INDArray = il.asMatrix(f, False)

			assertEquals(a_nchw, a_nchw2)
			assertEquals(a_nchw, a_nhwc.permute(0,3,1,2))


			'asMatrix(InputStream, boolean)
			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				a_nchw = il.asMatrix([is])
			End Using
			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				a_nchw2 = il.asMatrix([is], True)
			End Using
			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
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
			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				i_nchw = il.asImageMatrix([is])
			End Using
			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				i_nchw2 = il.asImageMatrix([is], True)
			End Using
			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				i_nhwc = il.asImageMatrix([is], False)
			End Using
			assertEquals(i_nchw.getImage(), i_nchw2.getImage())
			assertEquals(i_nchw.getImage(), i_nhwc.getImage().permute(0,3,1,2)) 'NHWC to NCHW
		End Sub
	End Class

End Namespace