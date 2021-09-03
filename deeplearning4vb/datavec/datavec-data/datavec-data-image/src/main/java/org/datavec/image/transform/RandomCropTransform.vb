Imports System
Imports Data = lombok.Data
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @JsonIgnoreProperties({"rng", "converter"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class RandomCropTransform extends BaseImageTransform<Mat>
	Public Class RandomCropTransform
		Inherits BaseImageTransform(Of Mat)

		Protected Friend outputHeight As Integer
		Protected Friend outputWidth As Integer
		Protected Friend rng As org.nd4j.linalg.api.rng.Random

		Private x As Integer
		Private y As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomCropTransform(@JsonProperty("outputHeight") int height, @JsonProperty("outputWidth") int width)
		Public Sub New(ByVal height As Integer, ByVal width As Integer)
			Me.New(1234, height, width)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal height As Integer, ByVal width As Integer)
			Me.New(Nothing, seed, height, width)
		End Sub

		Public Sub New(ByVal random As Random, ByVal seed As Long, ByVal height As Integer, ByVal width As Integer)
			MyBase.New(random)
			Me.outputHeight = height
			Me.outputWidth = width
			Me.rng = Nd4j.Random
			Me.rng.Seed = seed
			Me.converter = New OpenCVFrameConverter.ToMat()
		End Sub

		''' <summary>
		''' Takes an image and returns a randomly cropped image.
		''' </summary>
		''' <param name="image">  to transform, null == end of stream </param>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <returns> transformed image </returns>
		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If
			' ensure that transform is valid
			If image.Frame.imageHeight < outputHeight OrElse image.Frame.imageWidth < outputWidth Then
				Throw New System.NotSupportedException("Output height/width cannot be more than the input image. Requested: " & outputHeight & "+x" & outputWidth & ", got " & image.Frame.imageHeight & "+x" & image.Frame.imageWidth)
			End If

			' determine boundary to place random offset
			Dim cropTop As Integer = image.Frame.imageHeight - outputHeight
			Dim cropLeft As Integer = image.Frame.imageWidth - outputWidth

			Dim mat As Mat = converter.convert(image.Frame)
			Dim top As Integer = rng.nextInt(cropTop + 1)
			Dim left As Integer = rng.nextInt(cropLeft + 1)

			y = Math.Min(top, mat.rows() - 1)
			x = Math.Min(left, mat.cols() - 1)
			Dim result As Mat = mat.apply(New Rect(x, y, outputWidth, outputHeight))


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