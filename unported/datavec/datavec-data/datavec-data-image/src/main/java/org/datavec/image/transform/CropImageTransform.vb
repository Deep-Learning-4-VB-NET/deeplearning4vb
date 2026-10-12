Imports System
Imports Data = lombok.Data
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports org.bytedeco.opencv.opencv_core

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class CropImageTransform extends BaseImageTransform<Mat>
	Public Class CropImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private cropTop As Integer
		Private cropLeft As Integer
		Private cropBottom As Integer
		Private cropRight As Integer

		Private x As Integer
		Private y As Integer

		''' <summary>
		''' Calls {@code this(null, crop, crop, crop, crop)}. </summary>
		Public Sub New(ByVal crop As Integer)
			Me.New(Nothing, crop, crop, crop, crop)
		End Sub

		''' <summary>
		''' Calls {@code this(random, crop, crop, crop, crop)}. </summary>
		Public Sub New(ByVal random As Random, ByVal crop As Integer)
			Me.New(random, crop, crop, crop, crop)
		End Sub

		''' <summary>
		''' Calls {@code this(random, cropTop, cropLeft, cropBottom, cropRight)}. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CropImageTransform(@JsonProperty("cropTop") int cropTop, @JsonProperty("cropLeft") int cropLeft, @JsonProperty("cropBottom") int cropBottom, @JsonProperty("cropRight") int cropRight)
		Public Sub New(ByVal cropTop As Integer, ByVal cropLeft As Integer, ByVal cropBottom As Integer, ByVal cropRight As Integer)
			Me.New(Nothing, cropTop, cropLeft, cropBottom, cropRight)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform.
		''' </summary>
		''' <param name="random">     object to use (or null for deterministic) </param>
		''' <param name="cropTop">    maximum cropping of the top of the image (pixels) </param>
		''' <param name="cropLeft">   maximum cropping of the left of the image (pixels) </param>
		''' <param name="cropBottom"> maximum cropping of the bottom of the image (pixels) </param>
		''' <param name="cropRight">  maximum cropping of the right of the image (pixels) </param>
		Public Sub New(ByVal random As Random, ByVal cropTop As Integer, ByVal cropLeft As Integer, ByVal cropBottom As Integer, ByVal cropRight As Integer)
			MyBase.New(random)
			Me.cropTop = cropTop
			Me.cropLeft = cropLeft
			Me.cropBottom = cropBottom
			Me.cropRight = cropRight
			Me.converter = New OpenCVFrameConverter.ToMat()
		End Sub

		''' <summary>
		''' Takes an image and returns a transformed image.
		''' Uses the random object in the case of random transformations.
		''' </summary>
		''' <param name="image">  to transform, null == end of stream </param>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <returns> transformed image </returns>
		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If
			Dim mat As Mat = converter.convert(image.Frame)
			Dim top As Integer = If(random IsNot Nothing, random.Next(cropTop + 1), cropTop)
			Dim left As Integer = If(random IsNot Nothing, random.Next(cropLeft + 1), cropLeft)
			Dim bottom As Integer = If(random IsNot Nothing, random.Next(cropBottom + 1), cropBottom)
			Dim right As Integer = If(random IsNot Nothing, random.Next(cropRight + 1), cropRight)

			y = Math.Min(top, mat.rows() - 1)
			x = Math.Min(left, mat.cols() - 1)
			Dim h As Integer = Math.Max(1, mat.rows() - bottom - y)
			Dim w As Integer = Math.Max(1, mat.cols() - right - x)
			Dim result As Mat = mat.apply(New Rect(x, y, w, h))

			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Dim transformed(coordinates.Length - 1) As Single
			For i As Integer = 0 To coordinates.Length - 1 Step 2
				transformed(i) = coordinates(i) - x
				transformed(i + 1) = coordinates(i + 1) - y
			Next i
			Return transformed
		End Function
	End Class

End Namespace