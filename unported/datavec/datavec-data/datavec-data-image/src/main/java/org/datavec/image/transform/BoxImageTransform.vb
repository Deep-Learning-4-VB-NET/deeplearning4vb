Imports System
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Accessors = lombok.experimental.Accessors
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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
'ORIGINAL LINE: @Accessors(fluent = true) @JsonIgnoreProperties({"borderValue"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class BoxImageTransform extends BaseImageTransform<Mat>
	Public Class BoxImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private width As Integer
		Private height As Integer

		Private x As Integer
		Private y As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter Scalar borderValue = Scalar.ZERO;
		Friend borderValue As Scalar = Scalar.ZERO

		''' <summary>
		''' Calls {@code this(null, width, height)}. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BoxImageTransform(@JsonProperty("width") int width, @JsonProperty("height") int height)
		Public Sub New(ByVal width As Integer, ByVal height As Integer)
			Me.New(Nothing, width, height)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform.
		''' </summary>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <param name="width">  of the boxed image (pixels) </param>
		''' <param name="height"> of the boxed image (pixels) </param>
		Public Sub New(ByVal random As Random, ByVal width As Integer, ByVal height As Integer)
			MyBase.New(random)
			Me.width = width
			Me.height = height
			Me.converter = New OpenCVFrameConverter.ToMat()
		End Sub

		''' <summary>
		''' Takes an image and returns a boxed version of the image.
		''' </summary>
		''' <param name="image">  to transform, null == end of stream </param>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <returns> transformed image </returns>
		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If

			Dim mat As Mat = converter.convert(image.Frame)
			Dim box As New Mat(height, width, mat.type())
			box.put(borderValue)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			x = (mat.cols() - width) / 2
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			y = (mat.rows() - height) / 2
			Dim w As Integer = Math.Min(mat.cols(), width)
			Dim h As Integer = Math.Min(mat.rows(), height)
			Dim matRect As New Rect(x, y, w, h)
			Dim boxRect As New Rect(x, y, w, h)

			If x <= 0 Then
				matRect.x(0)
				boxRect.x(-x)
			Else
				matRect.x(x)
				boxRect.x(0)
			End If

			If y <= 0 Then
				matRect.y(0)
				boxRect.y(-y)
			Else
				matRect.y(y)
				boxRect.y(0)
			End If
			mat.apply(matRect).copyTo(box.apply(boxRect))
			Return New ImageWritable(converter.convert(box))
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