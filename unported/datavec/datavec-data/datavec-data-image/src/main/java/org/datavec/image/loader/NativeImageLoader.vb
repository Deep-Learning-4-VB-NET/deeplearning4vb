Imports System
Imports System.Diagnostics
Imports System.IO
Imports IOUtils = org.apache.commons.io.IOUtils
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports Frame = org.bytedeco.javacv.Frame
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports Image = org.datavec.image.data.Image
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.bytedeco.leptonica
Imports org.bytedeco.opencv.opencv_core
Imports org.bytedeco.leptonica.global.lept
Imports org.bytedeco.opencv.global.opencv_core
Imports org.bytedeco.opencv.global.opencv_imgcodecs
Imports org.bytedeco.opencv.global.opencv_imgproc

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
	Public Class NativeImageLoader
		Inherits BaseImageLoader

		Private Const MIN_BUFFER_STEP_SIZE As Integer = 64 * 1024
		Private buffer() As SByte = Nothing
		Private bufferMat As Mat = Nothing

		Public Shadows Shared ReadOnly ALLOWED_FORMATS() As String = {"bmp", "gif", "jpg", "jpeg", "jp2", "pbm", "pgm", "ppm", "pnm", "png", "tif", "tiff", "exr", "webp", "BMP", "GIF", "JPG", "JPEG", "JP2", "PBM", "PGM", "PPM", "PNM", "PNG", "TIF", "TIFF", "EXR", "WEBP"}

		Protected Friend converter As New OpenCVFrameConverter.ToMat()

		Friend direct As Boolean = Not Loader.getPlatform().StartsWith("android")

		''' <summary>
		''' Loads images with no scaling or conversion.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Instantiate an image with the given
		''' height and width </summary>
		''' <param name="height"> the height to load </param>
		''' <param name="width">  the width to load
		'''  </param>
		Public Sub New(ByVal height As Long, ByVal width As Long)
			Me.height = height
			Me.width = width
		End Sub


		''' <summary>
		''' Instantiate an image with the given
		''' height and width </summary>
		''' <param name="height"> the height to load </param>
		''' <param name="width">  the width to load </param>
		''' <param name="channels"> the number of channels for the image* </param>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long)
			Me.height = height
			Me.width = width
			Me.channels = channels
		End Sub

		''' <summary>
		''' Instantiate an image with the given
		''' height and width </summary>
		''' <param name="height"> the height to load </param>
		''' <param name="width">  the width to load </param>
		''' <param name="channels"> the number of channels for the image* </param>
		''' <param name="centerCropIfNeeded"> to crop before rescaling and converting </param>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal centerCropIfNeeded As Boolean)
			Me.New(height, width, channels)
			Me.centerCropIfNeeded = centerCropIfNeeded
		End Sub

		''' <summary>
		''' Instantiate an image with the given
		''' height and width </summary>
		''' <param name="height"> the height to load </param>
		''' <param name="width">  the width to load </param>
		''' <param name="channels"> the number of channels for the image* </param>
		''' <param name="imageTransform"> to use before rescaling and converting </param>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal imageTransform As ImageTransform)
			Me.New(height, width, channels)
			Me.imageTransform = imageTransform
		End Sub

		''' <summary>
		''' Instantiate an image with the given
		''' height and width </summary>
		''' <param name="height"> the height to load </param>
		''' <param name="width">  the width to load </param>
		''' <param name="channels"> the number of channels for the image* </param>
		''' <param name="mode"> how to load multipage image </param>
		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal mode As MultiPageMode)
			Me.New(height, width, channels)
			Me.multiPageMode = mode
		End Sub

		Protected Friend Sub New(ByVal other As NativeImageLoader)
			Me.height = other.height
			Me.width = other.width
			Me.channels = other.channels
			Me.centerCropIfNeeded = other.centerCropIfNeeded
			Me.imageTransform = other.imageTransform
			Me.multiPageMode = other.multiPageMode
		End Sub

		Public Overrides ReadOnly Property AllowedFormats As String()
			Get
				Return ALLOWED_FORMATS
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(String filename) throws IOException
		Public Overridable Overloads Function asRowVector(ByVal filename As String) As INDArray
			Return asRowVector(New File(filename))
		End Function

		''' <summary>
		''' Convert a file to a row vector
		''' </summary>
		''' <param name="f"> the image to convert </param>
		''' <returns> the flattened image </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(File f) throws IOException
		Public Overrides Function asRowVector(ByVal f As File) As INDArray
			Return asMatrix(f).ravel()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(InputStream is) throws IOException
		Public Overrides Function asRowVector(ByVal [is] As Stream) As INDArray
			Return asMatrix([is]).ravel()
		End Function

		''' <summary>
		''' Returns {@code asMatrix(image).ravel()}. </summary>
		''' <seealso cref= #asMatrix(Object) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(Object image) throws IOException
		Public Overridable Overloads Function asRowVector(ByVal image As Object) As INDArray
			Return asMatrix(image).ravel()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(org.bytedeco.javacv.Frame image) throws IOException
		Public Overridable Overloads Function asRowVector(ByVal image As Frame) As INDArray
			Return asMatrix(image).ravel()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(Mat image) throws IOException
		Public Overridable Overloads Function asRowVector(ByVal image As Mat) As INDArray
			Dim arr As INDArray = asMatrix(image)
			Return arr.reshape("c"c, 1, arr.length())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(org.opencv.core.Mat image) throws IOException
		Public Overridable Overloads Function asRowVector(ByVal image As org.opencv.core.Mat) As INDArray
			Dim arr As INDArray = asMatrix(image)
			Return arr.reshape("c"c, 1, arr.length())
		End Function

		Friend Shared Function convert(ByVal pix As PIX) As Mat
			Dim tempPix As PIX = Nothing
			Dim dtype As Integer = -1
			Dim height As Integer = pix.h()
			Dim width As Integer = pix.w()
			Dim mat2 As Mat
			If pix.colormap() IsNot Nothing Then
				Dim pix2 As PIX = pixRemoveColormap(pix, REMOVE_CMAP_TO_FULL_COLOR)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: tempPix = pix = pix2;
				pix = pix2
					tempPix = pix
				dtype = CV_8UC4
			ElseIf pix.d() <= 8 OrElse pix.d() = 24 Then
				Dim pix2 As PIX = Nothing
				Select Case pix.d()
					Case 1
						pix2 = pixConvert1To8(Nothing, pix, CSByte(0), CSByte(255))
					Case 2
						pix2 = pixConvert2To8(pix, CSByte(0), CSByte(85), CSByte(170), CSByte(255), 0)
					Case 4
						pix2 = pixConvert4To8(pix, 0)
					Case 8
						pix2 = pix
					Case 24
						pix2 = pix
					Case Else
						Debug.Assert(False)
				End Select
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: tempPix = pix = pix2;
				pix = pix2
					tempPix = pix
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim channels As Integer = pix.d() / 8
				dtype = CV_8UC(channels)
				Dim mat As New Mat(height, width, dtype, pix.data(), 4 * pix.wpl())
				mat2 = New Mat(height, width, CV_8UC(channels))
				' swap bytes if needed
				Dim swap() As Integer = {0, channels - 1, 1, channels - 2, 2, channels - 3, 3, channels - 4}, copy() As Integer = {0, 0, 1, 1, 2, 2, 3, 3}, fromTo() As Integer = If(channels > 1 AndAlso ByteOrder.nativeOrder().Equals(ByteOrder.LITTLE_ENDIAN), swap, copy)
				mixChannels(mat, 1, mat2, 1, fromTo, Math.Min(channels, fromTo.Length \ 2))
			ElseIf pix.d() = 16 Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				dtype = CV_16UC(pix.d() / 16)
			ElseIf pix.d() = 32 Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				dtype = CV_32FC(pix.d() / 32)
			End If
			mat2 = New Mat(height, width, dtype, pix.data())
			If tempPix IsNot Nothing Then
				pixDestroy(tempPix)
			End If
			Return mat2
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(String filename) throws IOException
		Public Overridable Overloads Function asMatrix(ByVal filename As String) As INDArray
			Return asMatrix(New File(filename))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(File f) throws IOException
		Public Overrides Function asMatrix(ByVal f As File) As INDArray
			Return asMatrix(f, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(File f, boolean nchw) throws IOException
		Public Overrides Function asMatrix(ByVal f As File, ByVal nchw As Boolean) As INDArray
			Using bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				Return asMatrix(bis, nchw)
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(InputStream is) throws IOException
		Public Overrides Function asMatrix(ByVal [is] As Stream) As INDArray
			Return asMatrix([is], True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(InputStream inputStream, boolean nchw) throws IOException
		Public Overrides Function asMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As INDArray
			Dim mat As Mat = streamToMat(inputStream)
			Dim a As INDArray
			If Me.multiPageMode <> Nothing Then
				a = asMatrix(mat.data(), mat.cols())
			Else
				Dim image As Mat = imdecode(mat, IMREAD_ANYDEPTH Or IMREAD_ANYCOLOR)
				If image Is Nothing OrElse image.empty() Then
					Dim pix As PIX = pixReadMem(mat.data(), mat.cols())
					If pix Is Nothing Then
						Throw New IOException("Could not decode image from input stream")
					End If
					image = convert(pix)
					pixDestroy(pix)
				End If
				a = asMatrix(image)
				image.deallocate()
			End If
			If nchw Then
				Return a
			Else
				Return a.permute(0, 2, 3, 1) 'NCHW to NHWC
			End If
		End Function

		''' <summary>
		''' Read the stream to the buffer, and return the number of bytes read </summary>
		''' <param name="is"> Input stream to read </param>
		''' <returns> Mat with the buffer data as a row vector </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private Mat streamToMat(InputStream is) throws IOException
		Private Function streamToMat(ByVal [is] As Stream) As Mat
			If buffer Is Nothing Then
				buffer = IOUtils.toByteArray([is])
				If buffer.Length <= 0 Then
					Throw New IOException("Could not decode image from input stream: input stream was empty (no data)")
				End If
				bufferMat = New Mat(buffer)
				Return bufferMat
			Else
				Dim numReadTotal As Integer = [is].Read(buffer, 0, buffer.Length)
				'Need to know if all data has been read.
				'(a) if numRead < buffer.length - got everything
				'(b) if numRead >= buffer.length: we MIGHT have got everything (exact right size buffer) OR we need more data

				If numReadTotal <= 0 Then
					Throw New IOException("Could not decode image from input stream: input stream was empty (no data)")
				End If

				If numReadTotal < buffer.Length Then
					bufferMat.data().put(buffer, 0, numReadTotal)
					bufferMat.cols(numReadTotal)
					Return bufferMat
				End If

				'Buffer is full; reallocate and keep reading
				Dim numReadCurrent As Integer = numReadTotal
				Do While numReadCurrent <> -1
					Dim oldBuffer() As SByte = buffer
					If oldBuffer.Length = Integer.MaxValue Then
						Throw New System.InvalidOperationException("Cannot read more than Integer.MAX_VALUE bytes")
					End If
					'Double buffer, but allocate at least 1MB more
					Dim increase As Long = Math.Max(buffer.Length, MIN_BUFFER_STEP_SIZE)
					Dim newBufferLength As Integer = CInt(Math.Truncate(Math.Min(Integer.MaxValue, buffer.Length + increase)))

					buffer = New SByte(newBufferLength - 1){}
					Array.Copy(oldBuffer, 0, buffer, 0, oldBuffer.Length)
					numReadCurrent = [is].Read(buffer, oldBuffer.Length, buffer.Length - oldBuffer.Length)
					If numReadCurrent > 0 Then
						numReadTotal += numReadCurrent
					End If
				Loop

				bufferMat = New Mat(buffer)
				Return bufferMat
			End If

		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.datavec.image.data.Image asImageMatrix(String filename) throws IOException
		Public Overridable Overloads Function asImageMatrix(ByVal filename As String) As Image
			Return asImageMatrix(New File(filename))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(File f) throws IOException
		Public Overrides Function asImageMatrix(ByVal f As File) As Image
			Return asImageMatrix(f, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(File f, boolean nchw) throws IOException
		Public Overrides Function asImageMatrix(ByVal f As File, ByVal nchw As Boolean) As Image
			Using bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				Return asImageMatrix(bis, nchw)
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(InputStream is) throws IOException
		Public Overrides Function asImageMatrix(ByVal [is] As Stream) As Image
			Return asImageMatrix([is], True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(InputStream inputStream, boolean nchw) throws IOException
		Public Overrides Function asImageMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As Image
			Dim mat As Mat = streamToMat(inputStream)
			Dim image As Mat = imdecode(mat, IMREAD_ANYDEPTH Or IMREAD_ANYCOLOR)
			If image Is Nothing OrElse image.empty() Then
				Dim pix As PIX = pixReadMem(mat.data(), mat.cols())
				If pix Is Nothing Then
					Throw New IOException("Could not decode image from input stream")
				End If
				image = convert(pix)
				pixDestroy(pix)
			End If
			Dim a As INDArray = asMatrix(image)
			If Not nchw Then
				a = a.permute(0,2,3,1) 'NCHW to NHWC
			End If
			Dim i As New Image(a, image.channels(), image.rows(), image.cols())

			image.deallocate()
			Return i
		End Function

		''' <summary>
		''' Calls <seealso cref="AndroidNativeImageLoader.asMatrix(android.graphics.Bitmap)"/> or
		''' <seealso cref="Java2DNativeImageLoader.asMatrix(java.awt.image.BufferedImage)"/>. </summary>
		''' <param name="image"> as a <seealso cref="android.graphics.Bitmap"/> or <seealso cref="java.awt.image.BufferedImage"/> </param>
		''' <returns> the matrix or null for unsupported object classes </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(Object image) throws IOException
		Public Overridable Overloads Function asMatrix(ByVal image As Object) As INDArray
			Dim array As INDArray = Nothing
			If array Is Nothing Then
				Try
					array = (New AndroidNativeImageLoader(Me)).asMatrix(image)
				Catch e As NoClassDefFoundError
					' ignore
				End Try
			End If
			If array Is Nothing Then
				Try
					array = (New Java2DNativeImageLoader(Me)).asMatrix(image)
				Catch e As NoClassDefFoundError
					' ignore
				End Try
			End If
			Return array
		End Function


		Protected Friend Overridable Sub fillNDArray(ByVal image As Mat, ByVal ret As INDArray)
			Dim rows As Long = image.rows()
			Dim cols As Long = image.cols()
			Dim channels As Long = image.channels()

			If ret.length() <> rows * cols * channels Then
				Throw New ND4JIllegalStateException("INDArray provided to store image not equal to image: {channels: " & channels & ", rows: " & rows & ", columns: " & cols & "}")
			End If

			Dim idx As Indexer = image.createIndexer(direct)
			Dim pointer As Pointer = ret.data().pointer()
			Dim stride() As Long = ret.stride()
			Dim done As Boolean = False
			Dim pagedPointer As New PagedPointer(pointer, rows * cols * channels, ret.data().offset() * Nd4j.sizeOfDataType(ret.data().dataType()))

			If TypeOf pointer Is FloatPointer Then
				Dim retidx As FloatIndexer = FloatIndexer.create(CType(pagedPointer.asFloatPointer(), FloatPointer), New Long() {channels, rows, cols}, New Long() {stride(0), stride(1), stride(2)}, direct)
				If TypeOf idx Is UByteIndexer Then
					Dim ubyteidx As UByteIndexer = CType(idx, UByteIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, ubyteidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				ElseIf TypeOf idx Is UShortIndexer Then
					Dim ushortidx As UShortIndexer = CType(idx, UShortIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, ushortidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				ElseIf TypeOf idx Is IntIndexer Then
					Dim intidx As IntIndexer = CType(idx, IntIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, intidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				ElseIf TypeOf idx Is FloatIndexer Then
					Dim floatidx As FloatIndexer = CType(idx, FloatIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, floatidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				End If
				retidx.release()
			ElseIf TypeOf pointer Is DoublePointer Then
				Dim retidx As DoubleIndexer = DoubleIndexer.create(CType(pagedPointer.asDoublePointer(), DoublePointer), New Long() {channels, rows, cols}, New Long() {stride(0), stride(1), stride(2)}, direct)
				If TypeOf idx Is UByteIndexer Then
					Dim ubyteidx As UByteIndexer = CType(idx, UByteIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, ubyteidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				ElseIf TypeOf idx Is UShortIndexer Then
					Dim ushortidx As UShortIndexer = CType(idx, UShortIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, ushortidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				ElseIf TypeOf idx Is IntIndexer Then
					Dim intidx As IntIndexer = CType(idx, IntIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, intidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				ElseIf TypeOf idx Is FloatIndexer Then
					Dim floatidx As FloatIndexer = CType(idx, FloatIndexer)
					For k As Long = 0 To channels - 1
						For i As Long = 0 To rows - 1
							For j As Long = 0 To cols - 1
								retidx.put(k, i, j, floatidx.get(i, j, k))
							Next j
						Next i
					Next k
					done = True
				End If
				retidx.release()
			End If


			If Not done Then
				For k As Long = 0 To channels - 1
					For i As Long = 0 To rows - 1
						For j As Long = 0 To cols - 1
							If ret.rank() = 3 Then
								ret.putScalar(k, i, j, idx.getDouble(i, j, k))
							ElseIf ret.rank() = 4 Then
								ret.putScalar(1, k, i, j, idx.getDouble(i, j, k))
							ElseIf ret.rank() = 2 Then
								ret.putScalar(i, j, idx.getDouble(i, j))
							Else
								Throw New ND4JIllegalStateException("NativeImageLoader expects 2D, 3D or 4D output array, but " & ret.rank() & "D array was given")
							End If
						Next j
					Next i
				Next k
			End If

			idx.release()
			image.data()
			Nd4j.AffinityManager.tagLocation(ret, AffinityManager.Location.HOST)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void asMatrixView(InputStream is, org.nd4j.linalg.api.ndarray.INDArray view) throws IOException
		Public Overridable Sub asMatrixView(ByVal [is] As Stream, ByVal view As INDArray)
			Dim mat As Mat = streamToMat([is])
			Dim image As Mat = imdecode(mat, IMREAD_ANYDEPTH Or IMREAD_ANYCOLOR)
			If image Is Nothing OrElse image.empty() Then
				Dim pix As PIX = pixReadMem(mat.data(), mat.cols())
				If pix Is Nothing Then
					Throw New IOException("Could not decode image from input stream")
				End If
				image = convert(pix)
				pixDestroy(pix)
			End If
			If image Is Nothing Then
				Throw New Exception()
			End If
			asMatrixView(image, view)
			image.deallocate()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void asMatrixView(String filename, org.nd4j.linalg.api.ndarray.INDArray view) throws IOException
		Public Overridable Sub asMatrixView(ByVal filename As String, ByVal view As INDArray)
			asMatrixView(New File(filename), view)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void asMatrixView(File f, org.nd4j.linalg.api.ndarray.INDArray view) throws IOException
		Public Overridable Sub asMatrixView(ByVal f As File, ByVal view As INDArray)
			Using bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				asMatrixView(bis, view)
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void asMatrixView(Mat image, org.nd4j.linalg.api.ndarray.INDArray view) throws IOException
		Public Overridable Sub asMatrixView(ByVal image As Mat, ByVal view As INDArray)
			transformImage(image, view)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void asMatrixView(org.opencv.core.Mat image, org.nd4j.linalg.api.ndarray.INDArray view) throws IOException
		Public Overridable Sub asMatrixView(ByVal image As org.opencv.core.Mat, ByVal view As INDArray)
			transformImage(image, view)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(org.bytedeco.javacv.Frame image) throws IOException
		Public Overridable Overloads Function asMatrix(ByVal image As Frame) As INDArray
			Return asMatrix(converter.convert(image))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(org.opencv.core.Mat image) throws IOException
		Public Overridable Overloads Function asMatrix(ByVal image As org.opencv.core.Mat) As INDArray
			Dim ret As INDArray = transformImage(image, Nothing)

			Return ret.reshape(ArrayUtil.combine(New Long() {1}, ret.shape()))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(Mat image) throws IOException
		Public Overridable Overloads Function asMatrix(ByVal image As Mat) As INDArray
			Dim ret As INDArray = transformImage(image, Nothing)

			Return ret.reshape(ArrayUtil.combine(New Long() {1}, ret.shape()))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.nd4j.linalg.api.ndarray.INDArray transformImage(org.opencv.core.Mat image, org.nd4j.linalg.api.ndarray.INDArray ret) throws IOException
		Protected Friend Overridable Function transformImage(ByVal image As org.opencv.core.Mat, ByVal ret As INDArray) As INDArray
			Dim f As Frame = converter.convert(image)
			Return transformImage(converter.convert(f), ret)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.nd4j.linalg.api.ndarray.INDArray transformImage(Mat image, org.nd4j.linalg.api.ndarray.INDArray ret) throws IOException
		Protected Friend Overridable Function transformImage(ByVal image As Mat, ByVal ret As INDArray) As INDArray
			If imageTransform IsNot Nothing AndAlso converter IsNot Nothing Then
				Dim writable As New ImageWritable(converter.convert(image))
				writable = imageTransform.transform(writable)
				image = converter.convert(writable.Frame)
			End If
			Dim image2 As Mat = Nothing, image3 As Mat = Nothing, image4 As Mat = Nothing
			If channels > 0 AndAlso image.channels() <> channels Then
				Dim code As Integer = -1
				Select Case image.channels()
					Case 1
						Select Case CInt(channels)
							Case 3
								code = CV_GRAY2BGR
							Case 4
								code = CV_GRAY2RGBA
						End Select
					Case 3
						Select Case CInt(channels)
							Case 1
								code = CV_BGR2GRAY
							Case 4
								code = CV_BGR2RGBA
						End Select
					Case 4
						Select Case CInt(channels)
							Case 1
								code = CV_RGBA2GRAY
							Case 3
								code = CV_RGBA2BGR
						End Select
				End Select
				If code < 0 Then
					Throw New IOException("Cannot convert from " & image.channels() & " to " & channels & " channels.")
				End If
				image2 = New Mat()
				cvtColor(image, image2, code)
				image = image2
			End If
			If centerCropIfNeeded Then
				image3 = centerCropIfNeeded(image)
				If image3 <> image Then
					image = image3
				Else
					image3 = Nothing
				End If
			End If
			image4 = scalingIfNeed(image)
			If image4 <> image Then
				image = image4
			Else
				image4 = Nothing
			End If

			If ret Is Nothing Then
				Dim rows As Integer = image.rows()
				Dim cols As Integer = image.cols()
				Dim channels As Integer = image.channels()
				ret = Nd4j.create(channels, rows, cols)
			End If
			fillNDArray(image, ret)

			image.data() ' dummy call to make sure it does not get deallocated prematurely
			If image2 IsNot Nothing Then
				image2.deallocate()
			End If
			If image3 IsNot Nothing Then
				image3.deallocate()
			End If
			If image4 IsNot Nothing Then
				image4.deallocate()
			End If
			Return ret
		End Function

		' TODO build flexibility on where to crop the image
		Protected Friend Overridable Function centerCropIfNeeded(ByVal img As Mat) As Mat
			Dim x As Integer = 0
			Dim y As Integer = 0
			Dim height As Integer = img.rows()
			Dim width As Integer = img.cols()
			Dim diff As Integer = Math.Abs(width - height) \ 2

			If width > height Then
				x = diff
				width = width - diff
			ElseIf height > width Then
				y = diff
				height = height - diff
			End If
			Return img.apply(New Rect(x, y, width, height))
		End Function

		Protected Friend Overridable Function scalingIfNeed(ByVal image As Mat) As Mat
			Return scalingIfNeed(image, height, width)
		End Function

		Protected Friend Overridable Function scalingIfNeed(ByVal image As Mat, ByVal dstHeight As Long, ByVal dstWidth As Long) As Mat
			Dim scaled As Mat = image
			If dstHeight > 0 AndAlso dstWidth > 0 AndAlso (image.rows() <> dstHeight OrElse image.cols() <> dstWidth) Then
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: resize(image, scaled = new Mat(), new Size((int)Math.min(dstWidth, Integer.MAX_VALUE), (int)Math.min(dstHeight, Integer.MAX_VALUE)));
				resize(image, scaled = New Mat(), New Size(CInt(Math.Min(dstWidth, Integer.MaxValue)), CInt(Math.Min(dstHeight, Integer.MaxValue))))
			End If
			Return scaled
		End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.datavec.image.data.ImageWritable asWritable(String filename) throws IOException
		Public Overridable Function asWritable(ByVal filename As String) As ImageWritable
			Return asWritable(New File(filename))
		End Function

		''' <summary>
		''' Convert a file to a INDArray
		''' </summary>
		''' <param name="f"> the image to convert </param>
		''' <returns> INDArray </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.datavec.image.data.ImageWritable asWritable(File f) throws IOException
		Public Overridable Function asWritable(ByVal f As File) As ImageWritable
			Using bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				Dim mat As Mat = streamToMat(bis)
				Dim image As Mat = imdecode(mat, IMREAD_ANYDEPTH Or IMREAD_ANYCOLOR)
				If image Is Nothing OrElse image.empty() Then
					Dim pix As PIX = pixReadMem(mat.data(), mat.cols())
					If pix Is Nothing Then
						Throw New IOException("Could not decode image from input stream")
					End If
					image = convert(pix)
					pixDestroy(pix)
				End If

				Dim writable As New ImageWritable(converter.convert(image))
				Return writable
			End Using
		End Function

		''' <summary>
		''' Convert ImageWritable to INDArray
		''' </summary>
		''' <param name="writable"> ImageWritable to convert </param>
		''' <returns> INDArray </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(org.datavec.image.data.ImageWritable writable) throws IOException
		Public Overridable Overloads Function asMatrix(ByVal writable As ImageWritable) As INDArray
			Dim image As Mat = converter.convert(writable.Frame)
			Return asMatrix(image)
		End Function

		''' <summary>
		''' Returns {@code asFrame(array, -1)}. </summary>
		Public Overridable Function asFrame(ByVal array As INDArray) As Frame
			Return converter.convert(asMat(array))
		End Function

		''' <summary>
		''' Converts an INDArray to a JavaCV Frame. Only intended for images with rank 3.
		''' </summary>
		''' <param name="array"> to convert </param>
		''' <param name="dataType"> from JavaCV (DEPTH_FLOAT, DEPTH_UBYTE, etc), or -1 to use same type as the INDArray </param>
		''' <returns> data copied to a Frame </returns>
		Public Overridable Function asFrame(ByVal array As INDArray, ByVal dataType As Integer) As Frame
			Return converter.convert(asMat(array, OpenCVFrameConverter.getMatDepth(dataType)))
		End Function

		''' <summary>
		''' Returns {@code asMat(array, -1)}. </summary>
		Public Overridable Function asMat(ByVal array As INDArray) As Mat
			Return asMat(array, -1)
		End Function

		''' <summary>
		''' Converts an INDArray to an OpenCV Mat. Only intended for images with rank 3.
		''' </summary>
		''' <param name="array"> to convert </param>
		''' <param name="dataType"> from OpenCV (CV_32F, CV_8U, etc), or -1 to use same type as the INDArray </param>
		''' <returns> data copied to a Mat </returns>
		Public Overridable Function asMat(ByVal array As INDArray, ByVal dataType As Integer) As Mat
			If array.rank() > 4 OrElse (array.rank() > 3 AndAlso array.size(0) <> 1) Then
				Throw New System.NotSupportedException("Only rank 3 (or rank 4 with size(0) == 1) arrays supported")
			End If
			Dim rank As Integer = array.rank()
			Dim stride() As Long = array.stride()
			Dim offset As Long = array.data().offset()
			Dim pointer As Pointer = array.data().pointer().position(offset)

			Dim rows As Long = array.size(If(rank = 3, 1, 2))
			Dim cols As Long = array.size(If(rank = 3, 2, 3))
			Dim channels As Long = array.size(If(rank = 3, 0, 1))
			Dim done As Boolean = False

			If dataType < 0 Then
				dataType = If(TypeOf pointer Is DoublePointer, CV_64F, CV_32F)
			End If
			Dim mat As New Mat(CInt(Math.Min(rows, Integer.MaxValue)), CInt(Math.Min(cols, Integer.MaxValue)), CV_MAKETYPE(dataType, CInt(Math.Min(channels, Integer.MaxValue))))
			Dim matidx As Indexer = mat.createIndexer(direct)

			Nd4j.AffinityManager.ensureLocation(array, AffinityManager.Location.HOST)

			If TypeOf pointer Is FloatPointer AndAlso dataType = CV_32F Then
				Dim ptridx As FloatIndexer = FloatIndexer.create(CType(pointer, FloatPointer), New Long() {channels, rows, cols}, New Long() {stride(If(rank = 3, 0, 1)), stride(If(rank = 3, 1, 2)), stride(If(rank = 3, 2, 3))}, direct)
				Dim idx As FloatIndexer = CType(matidx, FloatIndexer)
				For k As Long = 0 To channels - 1
					For i As Long = 0 To rows - 1
						For j As Long = 0 To cols - 1
							idx.put(i, j, k, ptridx.get(k, i, j))
						Next j
					Next i
				Next k
				done = True
				ptridx.release()
			ElseIf TypeOf pointer Is DoublePointer AndAlso dataType = CV_64F Then
				Dim ptridx As DoubleIndexer = DoubleIndexer.create(CType(pointer, DoublePointer), New Long() {channels, rows, cols}, New Long() {stride(If(rank = 3, 0, 1)), stride(If(rank = 3, 1, 2)), stride(If(rank = 3, 2, 3))}, direct)
				Dim idx As DoubleIndexer = CType(matidx, DoubleIndexer)
				For k As Long = 0 To channels - 1
					For i As Long = 0 To rows - 1
						For j As Long = 0 To cols - 1
							idx.put(i, j, k, ptridx.get(k, i, j))
						Next j
					Next i
				Next k
				done = True
				ptridx.release()
			End If

			If Not done Then
				For k As Long = 0 To channels - 1
					For i As Long = 0 To rows - 1
						For j As Long = 0 To cols - 1
							If rank = 3 Then
								matidx.putDouble(New Long() {i, j, k}, array.getDouble(k, i, j))
							Else
								matidx.putDouble(New Long() {i, j, k}, array.getDouble(0, k, i, j))
							End If
						Next j
					Next i
				Next k
			End If

			matidx.release()
			Return mat
		End Function

		''' <summary>
		''' Read multipage tiff and load into INDArray
		''' </summary>
		''' <param name="bytes"> </param>
		''' <returns> INDArray </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.nd4j.linalg.api.ndarray.INDArray asMatrix(BytePointer bytes, long length) throws IOException
		Private Overloads Function asMatrix(ByVal bytes As BytePointer, ByVal length As Long) As INDArray
			Dim pixa As PIXA
			pixa = pixaReadMemMultipageTiff(bytes, length)
			Dim data As INDArray
			Dim currentD As INDArray
			Dim index() As INDArrayIndex = Nothing
			Select Case Me.multiPageMode
				Case org.datavec.image.loader.BaseImageLoader.MultiPageMode.MINIBATCH
					data = Nd4j.create(pixa.n(), 1, 1, pixa.pix(0).h(), pixa.pix(0).w())
	'            case CHANNELS:
	'                data = Nd4j.create(1, pixa.n(), 1, pixa.pix(0).h(), pixa.pix(0).w());
	'                break;
				Case org.datavec.image.loader.BaseImageLoader.MultiPageMode.FIRST
					data = Nd4j.create(1, 1, 1, pixa.pix(0).h(), pixa.pix(0).w())
					Dim pix As PIX = pixa.pix(0)
					currentD = asMatrix(convert(pix))
					pixDestroy(pix)
					index = New INDArrayIndex(){NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all()}
					data.put(index, currentD.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
					Return data
				Case Else
					Throw New System.NotSupportedException("Unsupported MultiPageMode: " & multiPageMode)
			End Select
			Dim i As Integer = 0
			Do While i < pixa.n()
				Dim pix As PIX = pixa.pix(i)
				currentD = asMatrix(convert(pix))
				pixDestroy(pix)
				Select Case Me.multiPageMode
					Case org.datavec.image.loader.BaseImageLoader.MultiPageMode.MINIBATCH
						index = New INDArrayIndex(){NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}
	'                case CHANNELS:
	'                    index = new INDArrayIndex[]{NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all(),NDArrayIndex.all()};
	'                    break;
					Case Else
						Throw New System.NotSupportedException("Unsupported MultiPageMode: " & multiPageMode)
				End Select
				data.put(index, currentD.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(),NDArrayIndex.all()))
				i += 1
			Loop

			Return data
		End Function

	End Class

End Namespace