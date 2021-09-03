Imports System
Imports Data = lombok.Data
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class ColorConversionTransform extends BaseImageTransform
	Public Class ColorConversionTransform
		Inherits BaseImageTransform

		''' <summary>
		''' Color Conversion code
		''' <seealso cref="org.bytedeco.opencv.global.opencv_imgproc"/>
		''' </summary>
		Private conversionCode As Integer

		''' <summary>
		''' Default conversion BGR to Luv (chroma) color.
		''' </summary>
		Public Sub New()
			Me.New(New Random(1234), COLOR_BGR2Luv)
		End Sub

		''' <summary>
		''' Return new ColorConversion object
		''' </summary>
		''' <param name="conversionCode">  to transform, </param>
		Public Sub New(ByVal conversionCode As Integer)
			Me.New(Nothing, conversionCode)
		End Sub

		''' <summary>
		''' Return new ColorConversion object
		''' </summary>
		''' <param name="random"> Random </param>
		''' <param name="conversionCode">  to transform, </param>
		Public Sub New(ByVal random As Random, ByVal conversionCode As Integer)
			MyBase.New(random)
			Me.conversionCode = conversionCode
			converter = New OpenCVFrameConverter.ToMat()
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
			Dim mat As Mat = CType(converter.convert(image.Frame), Mat)

			Dim result As New Mat()

			Try
				cvtColor(mat, result, conversionCode)
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Return New ImageWritable(converter.convert(result))
		End Function

		Public Overrides Function query(ParamArray ByVal coordinates() As Single) As Single()
			Return coordinates
		End Function
	End Class

End Namespace