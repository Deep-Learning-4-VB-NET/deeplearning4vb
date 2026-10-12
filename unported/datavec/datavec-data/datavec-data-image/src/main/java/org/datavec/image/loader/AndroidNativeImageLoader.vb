Imports System
Imports Bitmap = android.graphics.Bitmap
Imports AndroidFrameConverter = org.bytedeco.javacv.AndroidFrameConverter
Imports Frame = org.bytedeco.javacv.Frame
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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
	Public Class AndroidNativeImageLoader
		Inherits NativeImageLoader

		Friend converter2 As New AndroidFrameConverter()

		Public Sub New()
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer)
			MyBase.New(height, width)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer)
			MyBase.New(height, width, channels)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal centerCropIfNeeded As Boolean)
			MyBase.New(height, width, channels, centerCropIfNeeded)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal imageTransform As ImageTransform)
			MyBase.New(height, width, channels, imageTransform)
		End Sub

		Protected Friend Sub New(ByVal other As NativeImageLoader)
			MyBase.New(other)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asRowVector(android.graphics.Bitmap image) throws java.io.IOException
		Public Overridable Overloads Function asRowVector(ByVal image As Bitmap) As INDArray
			Return asMatrix(image).ravel()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray asMatrix(android.graphics.Bitmap image) throws java.io.IOException
		Public Overridable Overloads Function asMatrix(ByVal image As Bitmap) As INDArray
			If converter Is Nothing Then
				converter = New OpenCVFrameConverter.ToMat()
			End If
			Return asMatrix(converter.convert(converter2.convert(image)))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(Object image) throws java.io.IOException
		Public Overrides Function asRowVector(ByVal image As Object) As INDArray
			Return If(TypeOf image Is Bitmap, asRowVector(DirectCast(image, Bitmap)), Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(Object image) throws java.io.IOException
		Public Overrides Function asMatrix(ByVal image As Object) As INDArray
			Return If(TypeOf image Is Bitmap, asMatrix(DirectCast(image, Bitmap)), Nothing)
		End Function

		''' <summary>
		''' Returns {@code asBitmap(array, Frame.DEPTH_UBYTE)}. </summary>
		Public Overridable Function asBitmap(ByVal array As INDArray) As Bitmap
			Return asBitmap(array, Frame.DEPTH_UBYTE)
		End Function

		''' <summary>
		''' Converts an INDArray to a Bitmap. Only intended for images with rank 3.
		''' </summary>
		''' <param name="array"> to convert </param>
		''' <param name="dataType"> from JavaCV (DEPTH_FLOAT, DEPTH_UBYTE, etc), or -1 to use same type as the INDArray </param>
		''' <returns> data copied to a Frame </returns>
		Public Overridable Function asBitmap(ByVal array As INDArray, ByVal dataType As Integer) As Bitmap
			Return converter2.convert(asFrame(array, dataType))
		End Function
	End Class

End Namespace