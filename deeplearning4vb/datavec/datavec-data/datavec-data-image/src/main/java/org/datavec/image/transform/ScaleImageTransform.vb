Imports System
Imports Data = lombok.Data
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports org.bytedeco.opencv.opencv_core
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
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class ScaleImageTransform extends BaseImageTransform<Mat>
	Public Class ScaleImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private dx As Single
		Private dy As Single

		Private srch, h As Integer
		Private srcw, w As Integer

		''' <summary>
		''' Calls {@code this(null, delta, delta)}. </summary>
		Public Sub New(ByVal delta As Single)
			Me.New(Nothing, delta, delta)
		End Sub

		''' <summary>
		''' Calls {@code this(random, delta, delta)}. </summary>
		Public Sub New(ByVal random As Random, ByVal delta As Single)
			Me.New(random, delta, delta)
		End Sub

		''' <summary>
		''' Calls {@code this(null, dx, dy)}. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ScaleImageTransform(@JsonProperty("dx") float dx, @JsonProperty("dy") float dy)
		Public Sub New(ByVal dx As Single, ByVal dy As Single)
			Me.New(Nothing, dx, dy)
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform.
		''' </summary>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <param name="dx">     maximum scaling in width of the image (pixels) </param>
		''' <param name="dy">     maximum scaling in height of the image (pixels) </param>
		Public Sub New(ByVal random As Random, ByVal dx As Single, ByVal dy As Single)
			MyBase.New(random)
			Me.dx = dx
			Me.dy = dy
			Me.converter = New OpenCVFrameConverter.ToMat()
		End Sub

		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If
			Dim mat As Mat = converter.convert(image.Frame)
			srch = mat.rows()
			srcw = mat.cols()
			h = CLng(Math.Round(mat.rows() + dy * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1)), MidpointRounding.AwayFromZero))
			w = CLng(Math.Round(mat.cols() + dx * (If(random IsNot Nothing, 2 * random.nextFloat() - 1, 1)), MidpointRounding.AwayFromZero))

			Dim result As New Mat()
			resize(mat, result, New Size(w, h))
			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Dim transformed(coordinates.Length - 1) As Single
			For i As Integer = 0 To coordinates.Length - 1 Step 2
				transformed(i) = w * coordinates(i) / srcw
				transformed(i + 1) = h * coordinates(i + 1) / srch
			Next i
			Return transformed
		End Function
	End Class

End Namespace