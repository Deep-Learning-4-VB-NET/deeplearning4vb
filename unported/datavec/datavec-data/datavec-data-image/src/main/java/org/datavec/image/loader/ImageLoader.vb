Imports System
Imports System.IO
Imports TIFFImageReaderSpi = com.github.jaiimageio.impl.plugins.tiff.TIFFImageReaderSpi
Imports TIFFImageWriterSpi = com.github.jaiimageio.impl.plugins.tiff.TIFFImageWriterSpi
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports NDArrayUtil = org.nd4j.linalg.util.NDArrayUtil

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


	<Serializable>
	Public Class ImageLoader
		Inherits BaseImageLoader

		Shared Sub New()
			ImageIO.scanForPlugins()
			Dim registry As IIORegistry = IIORegistry.getDefaultInstance()
			registry.registerServiceProvider(New TIFFImageWriterSpi())
			registry.registerServiceProvider(New TIFFImageReaderSpi())
			registry.registerServiceProvider(New com.twelvemonkeys.imageio.plugins.jpeg.JPEGImageReaderSpi())
			registry.registerServiceProvider(New com.twelvemonkeys.imageio.plugins.jpeg.JPEGImageWriterSpi())
			registry.registerServiceProvider(New com.twelvemonkeys.imageio.plugins.psd.PSDImageReaderSpi())
			registry.registerServiceProvider(Arrays.asList(New com.twelvemonkeys.imageio.plugins.bmp.BMPImageReaderSpi(), New com.twelvemonkeys.imageio.plugins.bmp.CURImageReaderSpi(), New com.twelvemonkeys.imageio.plugins.bmp.ICOImageReaderSpi()))
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Instantiate an image with the given
		''' height and width
		''' </summary>
		''' <param name="height"> the height to load* </param>
		''' <param name="width">  the width to load </param>
		Public Sub New(ByVal height As Long, ByVal width As Long)
			MyBase.New()
			Me.height = height
			Me.width = width
		End Sub


		''' <summary>
		''' Instantiate an image with the given
		''' height and width
		''' </summary>
		''' <param name="height">   the height to load </param>
		''' <param name="width">    the width to load </param>
		''' <param name="channels"> the number of channels for the image* </param>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long)
			MyBase.New()
			Me.height = height
			Me.width = width
			Me.channels = channels
		End Sub

		''' <summary>
		''' Instantiate an image with the given
		''' height and width
		''' </summary>
		''' <param name="height">             the height to load </param>
		''' <param name="width">              the width to load </param>
		''' <param name="channels">           the number of channels for the image* </param>
		''' <param name="centerCropIfNeeded"> to crop before rescaling and converting </param>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal centerCropIfNeeded As Boolean)
			Me.New(height, width, channels)
			Me.centerCropIfNeeded = centerCropIfNeeded
		End Sub

		''' <summary>
		''' Convert a file to a row vector
		''' </summary>
		''' <param name="f"> the image to convert </param>
		''' <returns> the flattened image </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(File f) throws IOException
		Public Overrides Function asRowVector(ByVal f As File) As INDArray
			Return asRowVector(ImageIO.read(f))
			'        if(channels == 3) {
			'            return toRaveledTensor(f);
			'        }
			'        return NDArrayUtil.toNDArray(flattenedImageFromFile(f));
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(InputStream inputStream) throws IOException
		Public Overrides Function asRowVector(ByVal inputStream As Stream) As INDArray
			Return asRowVector(ImageIO.read(inputStream))
			'        return asMatrix(inputStream).ravel();
		End Function

		''' <summary>
		''' Convert an image in to a row vector
		''' </summary>
		''' <param name="image"> the image to convert </param>
		''' <returns> the row vector based on a rastered
		''' representation of the image </returns>
		Public Overridable Overloads Function asRowVector(ByVal image As BufferedImage) As INDArray
			If centerCropIfNeeded Then
				image = centerCropIfNeeded(image)
			End If
			image = scalingIfNeed(image, True)
			If channels = 3 Then
				Return toINDArrayBGR(image).ravel()
			End If
			Dim ret()() As Integer = toIntArrayArray(image)
			Return NDArrayUtil.toNDArray(ArrayUtil.flatten(ret))
		End Function

		''' <summary>
		''' Changes the input stream in to an
		''' bgr based raveled(flattened) vector
		''' </summary>
		''' <param name="file"> the input stream to convert </param>
		''' <returns> the raveled bgr values for this input stream </returns>
		Public Overridable Function toRaveledTensor(ByVal file As File) As INDArray
			Try
				Dim bis As New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
				Dim ret As INDArray = toRaveledTensor(bis)
				bis.close()
				Return ret.ravel()
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Changes the input stream in to an
		''' bgr based raveled(flattened) vector
		''' </summary>
		''' <param name="is"> the input stream to convert </param>
		''' <returns> the raveled bgr values for this input stream </returns>
		Public Overridable Function toRaveledTensor(ByVal [is] As Stream) As INDArray
			Return toBgr([is]).ravel()
		End Function

		''' <summary>
		''' Convert an image in to a raveled tensor of
		''' the bgr values of the image
		''' </summary>
		''' <param name="image"> the image to parse </param>
		''' <returns> the raveled tensor of bgr values </returns>
		Public Overridable Function toRaveledTensor(ByVal image As BufferedImage) As INDArray
			Try
				image = scalingIfNeed(image, False)
				Return toINDArrayBGR(image).ravel()
			Catch e As Exception
				Throw New Exception("Unable to load image", e)
			End Try
		End Function

		''' <summary>
		''' Convert an input stream to an bgr spectrum image
		''' </summary>
		''' <param name="file"> the file to convert </param>
		''' <returns> the input stream to convert </returns>
		Public Overridable Function toBgr(ByVal file As File) As INDArray
			Try
				Dim bis As New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
				Dim ret As INDArray = toBgr(bis)
				bis.close()
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Convert an input stream to an bgr spectrum image
		''' </summary>
		''' <param name="inputStream"> the input stream to convert </param>
		''' <returns> the input stream to convert </returns>
		Public Overridable Function toBgr(ByVal inputStream As Stream) As INDArray
			Try
				Dim image As BufferedImage = ImageIO.read(inputStream)
				Return toBgr(image)
			Catch e As IOException
				Throw New Exception("Unable to load image", e)
			End Try
		End Function

		Private Function toBgrImage(ByVal inputStream As Stream) As org.datavec.image.data.Image
			Try
				Dim image As BufferedImage = ImageIO.read(inputStream)
				Dim img As INDArray = toBgr(image)
				Return New org.datavec.image.data.Image(img, image.getData().getNumBands(), image.getHeight(), image.getWidth())
			Catch e As IOException
				Throw New Exception("Unable to load image", e)
			End Try
		End Function

		''' <summary>
		''' Convert an BufferedImage to an bgr spectrum image
		''' </summary>
		''' <param name="image"> the BufferedImage to convert </param>
		''' <returns> the input stream to convert </returns>
		Public Overridable Function toBgr(ByVal image As BufferedImage) As INDArray
			If image Is Nothing Then
				Throw New System.InvalidOperationException("Unable to load image")
			End If
			image = scalingIfNeed(image, False)
			Return toINDArrayBGR(image)
		End Function

		''' <summary>
		''' Convert an image file
		''' in to a matrix
		''' </summary>
		''' <param name="f"> the file to convert </param>
		''' <returns> a 2d matrix of a rastered version of the image </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(File f) throws IOException
		Public Overrides Function asMatrix(ByVal f As File) As INDArray
			Return asMatrix(f, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(File f, boolean nchw) throws IOException
		Public Overrides Function asMatrix(ByVal f As File, ByVal nchw As Boolean) As INDArray
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				Return asMatrix([is], nchw)
			End Using
		End Function

		''' <summary>
		''' Convert an input stream to a matrix
		''' </summary>
		''' <param name="inputStream"> the input stream to convert </param>
		''' <returns> the input stream to convert </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(InputStream inputStream) throws IOException
		Public Overrides Function asMatrix(ByVal inputStream As Stream) As INDArray
			Return asMatrix(inputStream, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(InputStream inputStream, boolean nchw) throws IOException
		Public Overrides Function asMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As INDArray
			Dim ret As INDArray
			If channels = 3 Then
				ret = toBgr(inputStream)
			Else
				Try
					Dim image As BufferedImage = ImageIO.read(inputStream)
					ret = asMatrix(image)
				Catch e As IOException
					Throw New IOException("Unable to load image", e)
				End Try
			End If
			If ret.rank() = 3 Then
				ret = ret.reshape(ChrW(1), ret.size(0), ret.size(1), ret.size(2))
			End If
			If Not nchw Then
				ret = ret.permute(0,2,3,1) 'NCHW to NHWC
			End If
			Return ret
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(File f) throws IOException
		Public Overrides Function asImageMatrix(ByVal f As File) As org.datavec.image.data.Image
			Return asImageMatrix(f, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(File f, boolean nchw) throws IOException
		Public Overrides Function asImageMatrix(ByVal f As File, ByVal nchw As Boolean) As org.datavec.image.data.Image
			Using bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				Return asImageMatrix(bis, nchw)
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(InputStream inputStream) throws IOException
		Public Overrides Function asImageMatrix(ByVal inputStream As Stream) As org.datavec.image.data.Image
			Return asImageMatrix(inputStream, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(InputStream inputStream, boolean nchw) throws IOException
		Public Overrides Function asImageMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As org.datavec.image.data.Image
			Dim ret As org.datavec.image.data.Image
			If channels = 3 Then
				ret = toBgrImage(inputStream)
			Else
				Try
					Dim image As BufferedImage = ImageIO.read(inputStream)
					Dim asMatrix As INDArray = Me.asMatrix(image)
					ret = New org.datavec.image.data.Image(asMatrix, image.getData().getNumBands(), image.getHeight(), image.getWidth())
				Catch e As IOException
					Throw New IOException("Unable to load image", e)
				End Try
			End If
			If ret.getImage().rank() = 3 Then
				Dim a As INDArray = ret.getImage()
				ret.setImage(a.reshape(ChrW(1), a.size(0), a.size(1), a.size(2)))
			End If
			If Not nchw Then
				ret.setImage(ret.getImage().permute(0,2,3,1)) 'NCHW to NHWC
			End If
			Return ret
		End Function

		''' <summary>
		''' Convert an BufferedImage to a matrix
		''' </summary>
		''' <param name="image"> the BufferedImage to convert </param>
		''' <returns> the input stream to convert </returns>
		Public Overridable Overloads Function asMatrix(ByVal image As BufferedImage) As INDArray
			If channels = 3 Then
				Return toBgr(image)
			Else
				image = scalingIfNeed(image, True)
				Dim w As Integer = image.getWidth()
				Dim h As Integer = image.getHeight()
				Dim ret As INDArray = Nd4j.create(h, w)

				For i As Integer = 0 To h - 1
					For j As Integer = 0 To w - 1
						ret.putScalar(New Integer(){i, j}, image.getRGB(j, i))
					Next j
				Next i
				Return ret
			End If
		End Function

		''' <summary>
		''' Slices up an image in to a mini batch.
		''' </summary>
		''' <param name="f">               the file to load from </param>
		''' <param name="numMiniBatches">  the number of images in a mini batch </param>
		''' <param name="numRowsPerSlice"> the number of rows for each image </param>
		''' <returns> a tensor representing one image as a mini batch </returns>
		Public Overridable Function asImageMiniBatches(ByVal f As File, ByVal numMiniBatches As Integer, ByVal numRowsPerSlice As Integer) As INDArray
			Try
				Dim d As INDArray = asMatrix(f)
				Return Nd4j.create(numMiniBatches, numRowsPerSlice, d.columns())
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int[] flattenedImageFromFile(File f) throws IOException
		Public Overridable Function flattenedImageFromFile(ByVal f As File) As Integer()
			Return ArrayUtil.flatten(fromFile(f))
		End Function

		''' <summary>
		''' Load a rastered image from file
		''' </summary>
		''' <param name="file"> the file to load </param>
		''' <returns> the rastered image </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int[][] fromFile(File file) throws IOException
		Public Overridable Function fromFile(ByVal file As File) As Integer()()
			Dim image As BufferedImage = ImageIO.read(file)
			image = scalingIfNeed(image, True)
			Return toIntArrayArray(image)
		End Function

		''' <summary>
		''' Load a rastered image from file
		''' </summary>
		''' <param name="file"> the file to load </param>
		''' <returns> the rastered image </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int[][][] fromFileMultipleChannels(File file) throws IOException
		Public Overridable Function fromFileMultipleChannels(ByVal file As File) As Integer()()()
			Dim image As BufferedImage = ImageIO.read(file)
			image = scalingIfNeed(image, channels > 3)

			Dim w As Integer = image.getWidth(), h As Integer = image.getHeight()
			Dim bands As Integer = image.getSampleModel().getNumBands()
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][][] As Integer = new Integer[CInt(Math.Min(channels, Integer.MaxValue))][CInt(Math.Min(h, Integer.MaxValue))][CInt(Math.Min(w, Integer.MaxValue))]
			Dim ret()()() As Integer = RectangularArrays.RectangularIntegerArray(CInt(Math.Min(channels, Integer.MaxValue)), CInt(Math.Min(h, Integer.MaxValue)), CInt(Math.Min(w, Integer.MaxValue)))
			Dim pixels() As SByte = CType(image.getRaster().getDataBuffer(), DataBufferByte).getData()

			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					For k As Integer = 0 To channels - 1
						If k >= bands Then
							Exit For
						End If
						ret(k)(i)(j) = pixels(CInt(Math.Min(channels * w * i + channels * j + k, Integer.MaxValue)))
					Next k
				Next j
			Next i
			Return ret
		End Function

		''' <summary>
		''' Convert a matrix in to a buffereed image
		''' </summary>
		''' <param name="matrix"> the </param>
		''' <returns> <seealso cref="java.awt.image.BufferedImage"/> </returns>
		Public Shared Function toImage(ByVal matrix As INDArray) As BufferedImage
			Dim img As New BufferedImage(matrix.rows(), matrix.columns(), BufferedImage.TYPE_INT_ARGB)
			Dim r As WritableRaster = img.getRaster()
			Dim equiv(CInt(matrix.length()) - 1) As Integer
			For i As Integer = 0 To equiv.Length - 1
				equiv(i) = CInt(Math.Truncate(matrix.getDouble(i)))
			Next i

			r.setDataElements(0, 0, matrix.rows(), matrix.columns(), equiv)
			Return img
		End Function


		Private Shared Function rasterData(ByVal matrix As INDArray) As Integer()
			Dim ret(CInt(matrix.length()) - 1) As Integer
			For i As Integer = 0 To ret.Length - 1
				ret(i) = CInt(CLng(Math.Round(DirectCast(matrix.getScalar(i).element(), Double), MidpointRounding.AwayFromZero)))
			Next i
			Return ret
		End Function

		''' <summary>
		''' Convert the given image to an rgb image
		''' </summary>
		''' <param name="arr">   the array to use </param>
		''' <param name="image"> the image to set </param>
		Public Overridable Sub toBufferedImageRGB(ByVal arr As INDArray, ByVal image As BufferedImage)
			If arr.rank() < 3 Then
				Throw New System.ArgumentException("Arr must be 3d")
			End If

			image = scalingIfNeed(image, arr.size(-2), arr.size(-1), image.getType(), True)
			Dim i As Integer = 0
			Do While i < image.getHeight()
				Dim j As Integer = 0
				Do While j < image.getWidth()
					Dim r As Integer = arr.slice(2).getInt(i, j)
					Dim g As Integer = arr.slice(1).getInt(i, j)
					Dim b As Integer = arr.slice(0).getInt(i, j)
					Dim a As Integer = 1
					Dim col As Integer = (a << 24) Or (r << 16) Or (g << 8) Or b
					image.setRGB(j, i, col)
					j += 1
				Loop
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Converts a given Image into a BufferedImage
		''' </summary>
		''' <param name="img">  The Image to be converted </param>
		''' <param name="type"> The color model of BufferedImage </param>
		''' <returns> The converted BufferedImage </returns>
		Public Shared Function toBufferedImage(ByVal img As Image, ByVal type As Integer) As BufferedImage
			If TypeOf img Is BufferedImage AndAlso CType(img, BufferedImage).getType() = type Then
				Return CType(img, BufferedImage)
			End If

			' Create a buffered image with transparency
			Dim bimage As New BufferedImage(img.getWidth(Nothing), img.getHeight(Nothing), type)

			' Draw the image on to the buffered image
			Dim bGr As Graphics2D = bimage.createGraphics()
			bGr.drawImage(img, 0, 0, Nothing)
			bGr.dispose()

			' Return the buffered image
			Return bimage
		End Function

		Protected Friend Overridable Function toIntArrayArray(ByVal image As BufferedImage) As Integer()()
			Dim w As Integer = image.getWidth(), h As Integer = image.getHeight()
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Integer = new Integer[h][w]
			Dim ret()() As Integer = RectangularArrays.RectangularIntegerArray(h, w)
			If image.getRaster().getNumDataElements() = 1 Then
				Dim raster As Raster = image.getRaster()
				For i As Integer = 0 To h - 1
					For j As Integer = 0 To w - 1
						ret(i)(j) = raster.getSample(j, i, 0)
					Next j
				Next i
			Else
				For i As Integer = 0 To h - 1
					For j As Integer = 0 To w - 1
						ret(i)(j) = image.getRGB(j, i)
					Next j
				Next i
			End If
			Return ret
		End Function

		Protected Friend Overridable Function toINDArrayBGR(ByVal image As BufferedImage) As INDArray
			Dim height As Integer = image.getHeight()
			Dim width As Integer = image.getWidth()
			Dim bands As Integer = image.getSampleModel().getNumBands()

			Dim pixels() As SByte = CType(image.getRaster().getDataBuffer(), DataBufferByte).getData()
			Dim shape() As Integer = {height, width, bands}

			Dim ret2 As INDArray = Nd4j.create(1, pixels.Length)
			Dim i As Integer = 0
			Do While i < ret2.length()
				ret2.putScalar(i, (CInt(pixels(i))) And &HFF)
				i += 1
			Loop
			Return ret2.reshape(shape).permute(2, 0, 1)
		End Function

		' TODO build flexibility on where to crop the image
		Public Overridable Function centerCropIfNeeded(ByVal img As BufferedImage) As BufferedImage
			Dim x As Integer = 0
			Dim y As Integer = 0
			Dim height As Integer = img.getHeight()
			Dim width As Integer = img.getWidth()
			Dim diff As Integer = Math.Abs(width - height) \ 2

			If width > height Then
				x = diff
				width = width - diff
			ElseIf height > width Then
				y = diff
				height = height - diff
			End If
			Return img.getSubimage(x, y, width, height)
		End Function

		Protected Friend Overridable Function scalingIfNeed(ByVal image As BufferedImage, ByVal needAlpha As Boolean) As BufferedImage
			Return scalingIfNeed(image, height, width, channels, needAlpha)
		End Function

		Protected Friend Overridable Function scalingIfNeed(ByVal image As BufferedImage, ByVal dstHeight As Long, ByVal dstWidth As Long, ByVal dstImageType As Long, ByVal needAlpha As Boolean) As BufferedImage
			Dim scaled As Image
			' Scale width and height first if necessary
			If dstHeight > 0 AndAlso dstWidth > 0 AndAlso (image.getHeight() <> dstHeight OrElse image.getWidth() <> dstWidth) Then
				scaled = image.getScaledInstance(CInt(dstWidth), CInt(dstHeight), Image.SCALE_SMOOTH)
			Else
				scaled = image
			End If

			' Transfer imageType if necessary and transfer to BufferedImage.
			If TypeOf scaled Is BufferedImage AndAlso CType(scaled, BufferedImage).getType() = dstImageType Then
				Return CType(scaled, BufferedImage)
			End If
			If needAlpha AndAlso image.getColorModel().hasAlpha() AndAlso dstImageType = BufferedImage.TYPE_4BYTE_ABGR Then
				Return toBufferedImage(scaled, BufferedImage.TYPE_4BYTE_ABGR)
			Else
				If dstImageType = BufferedImage.TYPE_BYTE_GRAY Then
					Return toBufferedImage(scaled, BufferedImage.TYPE_BYTE_GRAY)
				Else
					Return toBufferedImage(scaled, BufferedImage.TYPE_3BYTE_BGR)
				End If
			End If
		End Function

	End Class

End Namespace