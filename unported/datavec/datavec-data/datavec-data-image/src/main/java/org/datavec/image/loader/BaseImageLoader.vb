Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Image = org.datavec.image.data.Image
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils

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
'ORIGINAL LINE: @Slf4j public abstract class BaseImageLoader implements java.io.Serializable
	<Serializable>
	Public MustInherit Class BaseImageLoader

		Public Enum MultiPageMode
			MINIBATCH
			FIRST ', CHANNELS,
		End Enum

		Public Shared ReadOnly BASE_DIR As New File(System.getProperty("user.home"))
		Public Shared ReadOnly ALLOWED_FORMATS() As String = {"tif", "jpg", "png", "jpeg", "bmp", "JPEG", "JPG", "TIF", "PNG"}
		Protected Friend rng As New Random(DateTimeHelper.CurrentUnixTimeMillis())

		Protected Friend height As Long = -1
		Protected Friend width As Long = -1
		Protected Friend channels As Long = -1
		Protected Friend centerCropIfNeeded As Boolean = False
		Protected Friend imageTransform As ImageTransform = Nothing
		Protected Friend multiPageMode As MultiPageMode = Nothing

		Public Overridable ReadOnly Property AllowedFormats As String()
			Get
				Return ALLOWED_FORMATS
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.nd4j.linalg.api.ndarray.INDArray asRowVector(java.io.File f) throws java.io.IOException;
		Public MustOverride Function asRowVector(ByVal f As File) As INDArray

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.nd4j.linalg.api.ndarray.INDArray asRowVector(java.io.InputStream inputStream) throws java.io.IOException;
		Public MustOverride Function asRowVector(ByVal inputStream As Stream) As INDArray

		''' <summary>
		''' As per <seealso cref="asMatrix(File, Boolean)"/> but NCHW/channels_first format </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.File f) throws java.io.IOException;
		Public MustOverride Function asMatrix(ByVal f As File) As INDArray

		''' <summary>
		''' Load an image from a file to an INDArray </summary>
		''' <param name="f">    File to load the image from </param>
		''' <param name="nchw"> If true: return image in NCHW/channels_first [1, channels, height width] format; if false, return
		'''             in NHWC/channels_last [1, height, width, channels] format </param>
		''' <returns> Image file as as INDArray </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.File f, boolean nchw) throws java.io.IOException;
		Public MustOverride Function asMatrix(ByVal f As File, ByVal nchw As Boolean) As INDArray

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.InputStream inputStream) throws java.io.IOException;
		Public MustOverride Function asMatrix(ByVal inputStream As Stream) As INDArray
		''' <summary>
		''' Load an image file from an input stream to an INDArray </summary>
		''' <param name="inputStream"> Input stream to load the image from </param>
		''' <param name="nchw"> If true: return image in NCHW/channels_first [1, channels, height width] format; if false, return
		'''             in NHWC/channels_last [1, height, width, channels] format </param>
		''' <returns> Image file stream as as INDArray </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.InputStream inputStream, boolean nchw) throws java.io.IOException;
		Public MustOverride Function asMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As INDArray

		''' <summary>
		''' As per <seealso cref="asMatrix(File)"/> but as an <seealso cref="Image"/> </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.datavec.image.data.Image asImageMatrix(java.io.File f) throws java.io.IOException;
		Public MustOverride Function asImageMatrix(ByVal f As File) As Image
		''' <summary>
		''' As per <seealso cref="asMatrix(File, Boolean)"/> but as an <seealso cref="Image"/> </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.datavec.image.data.Image asImageMatrix(java.io.File f, boolean nchw) throws java.io.IOException;
		Public MustOverride Function asImageMatrix(ByVal f As File, ByVal nchw As Boolean) As Image

		''' <summary>
		''' As per <seealso cref="asMatrix(InputStream)"/> but as an <seealso cref="Image"/> </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.datavec.image.data.Image asImageMatrix(java.io.InputStream inputStream) throws java.io.IOException;
		Public MustOverride Function asImageMatrix(ByVal inputStream As Stream) As Image
		''' <summary>
		''' As per <seealso cref="asMatrix(InputStream, Boolean)"/> but as an <seealso cref="Image"/> </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.datavec.image.data.Image asImageMatrix(java.io.InputStream inputStream, boolean nchw) throws java.io.IOException;
		Public MustOverride Function asImageMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As Image


		Public Shared Sub downloadAndUntar(ByVal urlMap As System.Collections.IDictionary, ByVal fullDir As File)
			Try
				Dim file As New File(fullDir, urlMap("filesFilename").ToString())
				If Not file.isFile() Then
					FileUtils.copyURLToFile(New URL(urlMap("filesURL").ToString()), file)
				End If

				Dim fileName As String = file.ToString()
				If fileName.EndsWith(".tgz", StringComparison.Ordinal) OrElse fileName.EndsWith(".tar.gz", StringComparison.Ordinal) OrElse fileName.EndsWith(".gz", StringComparison.Ordinal) OrElse fileName.EndsWith(".zip", StringComparison.Ordinal) Then
					ArchiveUtils.unzipFileTo(file.getAbsolutePath(), fullDir.getAbsolutePath(), False)
				End If
			Catch e As IOException
				Throw New System.InvalidOperationException("Unable to fetch images", e)
			End Try
		End Sub

	End Class

End Namespace