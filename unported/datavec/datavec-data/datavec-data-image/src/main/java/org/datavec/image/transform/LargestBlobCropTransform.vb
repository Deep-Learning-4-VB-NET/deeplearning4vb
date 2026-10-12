Imports System
Imports Data = lombok.Data
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @Data public class LargestBlobCropTransform extends BaseImageTransform<Mat>
	Public Class LargestBlobCropTransform
		Inherits BaseImageTransform(Of Mat)

		Protected Friend rng As org.nd4j.linalg.api.rng.Random

		Protected Friend mode, method, blurWidth, blurHeight, upperThresh, lowerThresh As Integer
		Protected Friend isCanny As Boolean

		Private x As Integer
		Private y As Integer

		''' <summary>
		''' Calls {@code this(null} </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Calls {@code this(random, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, 3, 3, 100, 200, true)} </summary>
		Public Sub New(ByVal random As Random)
			Me.New(random, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE, 3, 3, 100, 200, True)
		End Sub

		''' 
		''' <param name="random">        Object to use (or null for deterministic) </param>
		''' <param name="mode">          Contour retrieval mode </param>
		''' <param name="method">        Contour approximation method </param>
		''' <param name="blurWidth">     Width of blurring kernel size </param>
		''' <param name="blurHeight">    Height of blurring kernel size </param>
		''' <param name="lowerThresh">   Lower threshold for either Canny or Threshold </param>
		''' <param name="upperThresh">   Upper threshold for either Canny or Threshold </param>
		''' <param name="isCanny">       Whether the edge detector is Canny or Threshold </param>
		Public Sub New(ByVal random As Random, ByVal mode As Integer, ByVal method As Integer, ByVal blurWidth As Integer, ByVal blurHeight As Integer, ByVal lowerThresh As Integer, ByVal upperThresh As Integer, ByVal isCanny As Boolean)
			MyBase.New(random)
			Me.rng = Nd4j.Random
			Me.mode = mode
			Me.method = method
			Me.blurWidth = blurWidth
			Me.blurHeight = blurHeight
			Me.lowerThresh = lowerThresh
			Me.upperThresh = upperThresh
			Me.isCanny = isCanny
			Me.converter = New OpenCVFrameConverter.ToMat()
		End Sub

		''' <summary>
		''' Takes an image and returns a cropped image based on it's largest blob.
		''' </summary>
		''' <param name="image">  to transform, null == end of stream </param>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <returns> transformed image </returns>
		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If

			'Convert image to gray and blur
			Dim original As Mat = converter.convert(image.Frame)
			Dim grayed As New Mat()
			cvtColor(original, grayed, CV_BGR2GRAY)
			If blurWidth > 0 AndAlso blurHeight > 0 Then
				blur(grayed, grayed, New Size(blurWidth, blurHeight))
			End If

			'Get edges from Canny edge detector
			Dim edgeOut As New Mat()
			If isCanny Then
				Canny(grayed, edgeOut, lowerThresh, upperThresh)
			Else
				threshold(grayed, edgeOut, lowerThresh, upperThresh, 0)
			End If

			Dim largestArea As Double = 0
			Dim boundingRect As New Rect()
			Dim contours As New MatVector()
			Dim hierarchy As New Mat()

			findContours(edgeOut, contours, hierarchy, Me.mode, Me.method)

			For i As Integer = 0 To contours.size() - 1
				'  Find the area of contour
				Dim area As Double = contourArea(contours.get(i), False)

				If area > largestArea Then
					' Find the bounding rectangle for biggest contour
					boundingRect = boundingRect(contours.get(i))
				End If
			Next i

			'Apply crop and return result
			x = boundingRect.x()
			y = boundingRect.y()
			Dim result As Mat = original.apply(boundingRect)

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