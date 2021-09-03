Imports System
Imports Data = lombok.Data
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports org.bytedeco.opencv.opencv_core
Imports org.bytedeco.opencv.global.opencv_core

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
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class FlipImageTransform extends BaseImageTransform<Mat>
	Public Class FlipImageTransform
		Inherits BaseImageTransform(Of Mat)

		''' <summary>
		''' the deterministic flip mode
		'''                 {@code  0} Flips around x-axis.
		'''                 {@code >0} Flips around y-axis.
		'''                 {@code <0} Flips around both axes.
		''' </summary>
		Private flipMode As Integer

		Private h As Integer
		Private w As Integer
		Private mode As Integer

		''' <summary>
		''' Calls {@code this(null)}.
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Calls {@code this(null)} and sets the flip mode.
		''' </summary>
		''' <param name="flipMode"> the deterministic flip mode
		'''                 {@code  0} Flips around x-axis.
		'''                 {@code >0} Flips around y-axis.
		'''                 {@code <0} Flips around both axes. </param>
		Public Sub New(ByVal flipMode As Integer)
			Me.New(Nothing)
			Me.flipMode = flipMode
		End Sub

		''' <summary>
		''' Constructs an instance of the ImageTransform. Randomly does not flip,
		''' or flips horizontally or vertically, or both.
		''' </summary>
		''' <param name="random"> object to use (or null for deterministic) </param>
		Public Sub New(ByVal random As Random)
			MyBase.New(random)
			converter = New OpenCVFrameConverter.ToMat()
		End Sub

		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If

			Dim mat As Mat = converter.convert(image.Frame)

			If mat Is Nothing Then
				Return Nothing
			End If
			h = mat.rows()
			w = mat.cols()

			mode = If(random IsNot Nothing, random.Next(4) - 2, flipMode)

			Dim result As New Mat()
			If mode < -1 Then
				' no flip
				mat.copyTo(result)
			Else
				flip(mat, result, mode)
			End If

			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Dim transformed(coordinates.Length - 1) As Single
			For i As Integer = 0 To coordinates.Length - 1 Step 2
				Dim x As Single = coordinates(i)
				Dim y As Single = coordinates(i + 1)
				Dim x2 As Single = w - x - 1
				Dim y2 As Single = h - y - 1

				If mode < -1 Then
					transformed(i) = x
					transformed(i + 1) = y
				ElseIf mode = 0 Then
					transformed(i) = x
					transformed(i + 1) = y2
				ElseIf mode > 0 Then
					transformed(i) = x2
					transformed(i + 1) = y
				ElseIf mode < 0 Then
					transformed(i) = x2
					transformed(i + 1) = y2
				End If
			Next i
			Return transformed
		End Function
	End Class


End Namespace