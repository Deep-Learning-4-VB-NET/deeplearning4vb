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
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class ResizeImageTransform extends BaseImageTransform<Mat>
	Public Class ResizeImageTransform
		Inherits BaseImageTransform(Of Mat)

		Private newHeight As Integer
		Private newWidth As Integer

		Private srch As Integer
		Private srcw As Integer

		''' <summary>
		''' Returns new ResizeImageTransform object
		''' </summary>
		''' <param name="newWidth"> new Width for the outcome images </param>
		''' <param name="newHeight"> new Height for outcome images </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ResizeImageTransform(@JsonProperty("newWidth") int newWidth, @JsonProperty("newHeight") int newHeight)
		Public Sub New(ByVal newWidth As Integer, ByVal newHeight As Integer)
			Me.New(Nothing, newWidth, newHeight)
		End Sub

		''' <summary>
		''' Returns new ResizeImageTransform object
		''' </summary>
		''' <param name="random"> Random </param>
		''' <param name="newWidth"> new Width for the outcome images </param>
		''' <param name="newHeight"> new Height for outcome images </param>
		Public Sub New(ByVal random As Random, ByVal newWidth As Integer, ByVal newHeight As Integer)
			MyBase.New(random)

			Me.newWidth = newWidth
			Me.newHeight = newHeight
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
			Dim result As New Mat()
			srch = mat.rows()
			srcw = mat.cols()
			resize(mat, result, New Size(newWidth, newHeight))
			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Dim transformed(coordinates.Length - 1) As Single
			For i As Integer = 0 To coordinates.Length - 1 Step 2
				transformed(i) = newWidth * coordinates(i) / srcw
				transformed(i + 1) = newHeight * coordinates(i + 1) / srch
			Next i
			Return transformed
		End Function
	End Class

End Namespace