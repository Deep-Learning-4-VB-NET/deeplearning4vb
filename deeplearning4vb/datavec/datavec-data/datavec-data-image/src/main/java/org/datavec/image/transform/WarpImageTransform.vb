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
'ORIGINAL LINE: @Accessors(fluent = true) @JsonIgnoreProperties({"interMode", "borderMode", "borderValue", "converter"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class WarpImageTransform extends BaseImageTransform<Mat>
	Public Class WarpImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private deltas() As Single

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter int interMode = INTER_LINEAR;
		Friend interMode As Integer = INTER_LINEAR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter int borderMode = BORDER_CONSTANT;
		Friend borderMode As Integer = BORDER_CONSTANT
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter Scalar borderValue = Scalar.ZERO;
		Friend borderValue As Scalar = Scalar.ZERO

		Private M As Mat

		''' <summary>
		''' Calls {@code this(null, delta, delta, delta, delta, delta, delta, delta, delta)}. </summary>
		Public Sub New(ByVal delta As Single)
			Me.New(Nothing, delta, delta, delta, delta, delta, delta, delta, delta)
		End Sub

		''' <summary>
		''' Calls {@code this(random, delta, delta, delta, delta, delta, delta, delta, delta)}. </summary>
		Public Sub New(ByVal random As Random, ByVal delta As Single)
			Me.New(random, delta, delta, delta, delta, delta, delta, delta, delta)
		End Sub

		''' <summary>
		''' Calls {@code this(null, dx1, dy1, dx2, dy2, dx3, dy3, dx4, dy4)}. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WarpImageTransform(@JsonProperty("deltas[0]") float dx1, @JsonProperty("deltas[1]") float dy1, @JsonProperty("deltas[2]") float dx2, @JsonProperty("deltas[3]") float dy2, @JsonProperty("deltas[4]") float dx3, @JsonProperty("deltas[5]") float dy3, @JsonProperty("deltas[6]") float dx4, @JsonProperty("deltas[7]") float dy4)
		Public Sub New(ByVal dx1 As Single, ByVal dy1 As Single, ByVal dx2 As Single, ByVal dy2 As Single, ByVal dx3 As Single, ByVal dy3 As Single, ByVal dx4 As Single, ByVal dy4 As Single)
			Me.New(Nothing, dx1, dy1, dx2, dy2, dx3, dy3, dx4, dy4)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform.
		''' </summary>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <param name="dx1">    maximum warping in x for the top-left corner (pixels) </param>
		''' <param name="dy1">    maximum warping in y for the top-left corner (pixels) </param>
		''' <param name="dx2">    maximum warping in x for the top-right corner (pixels) </param>
		''' <param name="dy2">    maximum warping in y for the top-right corner (pixels) </param>
		''' <param name="dx3">    maximum warping in x for the bottom-right corner (pixels) </param>
		''' <param name="dy3">    maximum warping in y for the bottom-right corner (pixels) </param>
		''' <param name="dx4">    maximum warping in x for the bottom-left corner (pixels) </param>
		''' <param name="dy4">    maximum warping in y for the bottom-left corner (pixels) </param>
		Public Sub New(ByVal random As Random, ByVal dx1 As Single, ByVal dy1 As Single, ByVal dx2 As Single, ByVal dy2 As Single, ByVal dx3 As Single, ByVal dy3 As Single, ByVal dx4 As Single, ByVal dy4 As Single)
			MyBase.New(random)
			deltas = New Single(7){}
			deltas(0) = dx1
			deltas(1) = dy1
			deltas(2) = dx2
			deltas(3) = dy2
			deltas(4) = dx3
			deltas(5) = dy3
			deltas(6) = dx4
			deltas(7) = dy4
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
			Dim src As New Point2f(4)
			Dim dst As New Point2f(4)
			src.put(0, 0, mat.cols(), 0, mat.cols(), mat.rows(), 0, mat.rows())

			For i As Integer = 0 To 7
				dst.put(i, src.get(i) + deltas(i) * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1)))
			Next i
			Dim result As New Mat()
			M = getPerspectiveTransform(src, dst)
			warpPerspective(mat, result, M, mat.size(), interMode, borderMode, borderValue)

			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Dim src As New Mat(1, coordinates.Length \ 2, CV_32FC2, New FloatPointer(coordinates))
			Dim dst As New Mat()
			perspectiveTransform(src, dst, M)
			Dim buf As FloatBuffer = dst.createBuffer()
			Dim transformed(coordinates.Length - 1) As Single
			buf.get(transformed)
			Return transformed
		End Function
	End Class

End Namespace