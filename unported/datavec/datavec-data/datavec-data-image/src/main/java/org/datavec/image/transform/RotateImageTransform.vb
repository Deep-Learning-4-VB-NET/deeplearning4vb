Imports System
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Accessors = lombok.experimental.Accessors
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports org.bytedeco.opencv.opencv_core
Imports org.bytedeco.opencv.global.opencv_core
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

Namespace org.datavec.image.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Accessors(fluent = true) @JsonIgnoreProperties({"interMode", "borderMode", "borderValue", "converter"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class RotateImageTransform extends BaseImageTransform<Mat>
	Public Class RotateImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private centerx As Single
		Private centery As Single
		Private angle As Single
		Private scale As Single

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int interMode = INTER_LINEAR;
		Private interMode As Integer = INTER_LINEAR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int borderMode = BORDER_CONSTANT;
		Private borderMode As Integer = BORDER_CONSTANT
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private Scalar borderValue = Scalar.ZERO;
		Private borderValue As Scalar = Scalar.ZERO

		Private M As Mat

		''' <summary>
		''' Calls {@code this(null, 0, 0, angle, 0)}. </summary>
		Public Sub New(ByVal angle As Single)
			Me.New(Nothing, 0, 0, angle, 0)
		End Sub

		''' <summary>
		''' Calls {@code this(random, 0, 0, angle, 0)}. </summary>
		Public Sub New(ByVal random As Random, ByVal angle As Single)
			Me.New(random, 0, 0, angle, 0)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform.
		''' </summary>
		''' <param name="centerx"> maximum deviation in x of center of rotation (relative to image center) </param>
		''' <param name="centery"> maximum deviation in y of center of rotation (relative to image center) </param>
		''' <param name="angle">   maximum rotation (degrees) </param>
		''' <param name="scale">   maximum scaling (relative to 1) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RotateImageTransform(@JsonProperty("centerx") float centerx, @JsonProperty("centery") float centery, @JsonProperty("angle") float angle, @JsonProperty("scale") float scale)
		Public Sub New(ByVal centerx As Single, ByVal centery As Single, ByVal angle As Single, ByVal scale As Single)
			Me.New(Nothing, centerx, centery, angle, scale)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform.
		''' </summary>
		''' <param name="random">  object to use (or null for deterministic) </param>
		''' <param name="centerx"> maximum deviation in x of center of rotation (relative to image center) </param>
		''' <param name="centery"> maximum deviation in y of center of rotation (relative to image center) </param>
		''' <param name="angle">   maximum rotation (degrees) </param>
		''' <param name="scale">   maximum scaling (relative to 1) </param>
		Public Sub New(ByVal random As Random, ByVal centerx As Single, ByVal centery As Single, ByVal angle As Single, ByVal scale As Single)
			MyBase.New(random)
			Me.centerx = centerx
			Me.centery = centery
			Me.angle = angle
			Me.scale = scale
			Me.converter = New OpenCVFrameConverter.ToMat()
		End Sub

		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If
			Dim mat As Mat = converter.convert(image.Frame)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim cy As Single = mat.rows() / 2 + centery * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1))
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim cx As Single = mat.cols() / 2 + centerx * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1))
			Dim a As Single = angle * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1))
			Dim s As Single = 1 + scale * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1))

			Dim result As New Mat()
			M = getRotationMatrix2D(New Point2f(cx, cy), a, s)
			warpAffine(mat, result, M, mat.size(), interMode, borderMode, borderValue)
			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Dim src As New Mat(1, coordinates.Length \ 2, CV_32FC2, New FloatPointer(coordinates))
			Dim dst As New Mat()
			org.bytedeco.opencv.global.opencv_core.transform(src, dst, M)
			Dim buf As FloatBuffer = dst.createBuffer()
			Dim transformed(coordinates.Length - 1) As Single
			buf.get(transformed)
			Return transformed
		End Function
	End Class

End Namespace