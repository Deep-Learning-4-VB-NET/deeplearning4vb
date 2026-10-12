Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JsonMappers = org.datavec.api.transform.serde.JsonMappers
Imports Writable = org.datavec.api.writable.Writable
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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
'ORIGINAL LINE: @Data @Slf4j @NoArgsConstructor public class ImageTransformProcess
	Public Class ImageTransformProcess

		Private transformList As IList(Of ImageTransform)
		Private seed As Integer

		Public Sub New(ByVal seed As Integer, ParamArray ByVal transforms() As ImageTransform)
			Me.seed = seed
			Me.transformList = New List(Of ImageTransform) From {transforms}
		End Sub

		Public Sub New(ByVal seed As Integer, ByVal transformList As IList(Of ImageTransform))
			Me.seed = seed
			Me.transformList = transformList
		End Sub

		Public Sub New(ByVal builder As Builder)
			Me.New(builder.seed_Conflict, builder.transformList)
		End Sub

		Public Overridable Function execute(ByVal image As IList(Of Writable)) As IList(Of Writable)
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray executeArray(org.datavec.image.data.ImageWritable image) throws java.io.IOException
		Public Overridable Function executeArray(ByVal image As ImageWritable) As INDArray
			Dim random As Random = Nothing
			If seed <> 0 Then
				random = New Random(seed)
			End If

			Dim currentImage As ImageWritable = image
			For Each transform As ImageTransform In transformList
				currentImage = transform.transform(currentImage, random)
			Next transform

			Dim imageLoader As New NativeImageLoader()
			Return imageLoader.asMatrix(currentImage)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.datavec.image.data.ImageWritable execute(org.datavec.image.data.ImageWritable image) throws java.io.IOException
		Public Overridable Function execute(ByVal image As ImageWritable) As ImageWritable
			Dim random As Random = Nothing
			If seed <> 0 Then
				random = New Random(seed)
			End If

			Dim currentImage As ImageWritable = image
			For Each transform As ImageTransform In transformList
				currentImage = transform.transform(currentImage, random)
			Next transform

			Return currentImage
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.datavec.image.data.ImageWritable transformFileUriToInput(java.net.URI uri) throws java.io.IOException
		Public Overridable Function transformFileUriToInput(ByVal uri As URI) As ImageWritable

			Dim imageLoader As New NativeImageLoader()
			Dim img As ImageWritable = imageLoader.asWritable(New File(uri))

			Return img
		End Function

		''' <summary>
		''' Convert the ImageTransformProcess to a JSON string
		''' </summary>
		''' <returns> ImageTransformProcess, as JSON </returns>
		Public Overridable Function toJson() As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				'TODO better exceptions
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Convert the ImageTransformProcess to a YAML string
		''' </summary>
		''' <returns> ImageTransformProcess, as YAML </returns>
		Public Overridable Function toYaml() As String
			Try
				Return JsonMappers.MapperYaml.writeValueAsString(Me)
			Catch e As JsonProcessingException
				'TODO better exceptions
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Deserialize a JSON String (created by <seealso cref="toJson()"/>) to a ImageTransformProcess
		''' </summary>
		''' <returns> ImageTransformProcess, from JSON </returns>
		Public Shared Function fromJson(ByVal json As String) As ImageTransformProcess
			Try
				Return JsonMappers.Mapper.readValue(json, GetType(ImageTransformProcess))
			Catch e As IOException
				'TODO better exceptions
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Deserialize a JSON String (created by <seealso cref="toJson()"/>) to a ImageTransformProcess
		''' </summary>
		''' <returns> ImageTransformProcess, from JSON </returns>
		Public Shared Function fromYaml(ByVal yaml As String) As ImageTransformProcess
			Try
				Return JsonMappers.MapperYaml.readValue(yaml, GetType(ImageTransformProcess))
			Catch e As IOException
				'TODO better exceptions
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Builder class for constructing a ImageTransformProcess
		''' </summary>
		Public Class Builder

			Friend transformList As IList(Of ImageTransform)
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend seed_Conflict As Integer = 0

			Public Sub New()
				transformList = New List(Of ImageTransform)()
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Integer) As Builder
				Me.seed_Conflict = seed_Conflict
				Return Me
			End Function

			Public Overridable Function cropImageTransform(ByVal crop As Integer) As Builder
				transformList.Add(New CropImageTransform(crop))
				Return Me
			End Function

			Public Overridable Function cropImageTransform(ByVal cropTop As Integer, ByVal cropLeft As Integer, ByVal cropBottom As Integer, ByVal cropRight As Integer) As Builder
				transformList.Add(New CropImageTransform(cropTop, cropLeft, cropBottom, cropRight))
				Return Me
			End Function

			Public Overridable Function colorConversionTransform(ByVal conversionCode As Integer) As Builder
				transformList.Add(New ColorConversionTransform(conversionCode))
				Return Me
			End Function

			Public Overridable Function equalizeHistTransform(ByVal conversionCode As Integer) As Builder
				transformList.Add(New EqualizeHistTransform(conversionCode))
				Return Me
			End Function

			Public Overridable Function filterImageTransform(ByVal filters As String, ByVal width As Integer, ByVal height As Integer) As Builder
				transformList.Add(New FilterImageTransform(filters, width, height))
				Return Me
			End Function

			Public Overridable Function filterImageTransform(ByVal filters As String, ByVal width As Integer, ByVal height As Integer, ByVal channels As Integer) As Builder
				transformList.Add(New FilterImageTransform(filters, width, height, channels))
				Return Me
			End Function

			Public Overridable Function flipImageTransform(ByVal flipMode As Integer) As Builder
				transformList.Add(New FlipImageTransform(flipMode))
				Return Me
			End Function

			Public Overridable Function randomCropTransform(ByVal height As Integer, ByVal width As Integer) As Builder
				transformList.Add(New RandomCropTransform(height, width))
				Return Me
			End Function

			Public Overridable Function randomCropTransform(ByVal seed As Long, ByVal height As Integer, ByVal width As Integer) As Builder
				transformList.Add(New RandomCropTransform(seed, height, width))
				Return Me
			End Function

			Public Overridable Function resizeImageTransform(ByVal newWidth As Integer, ByVal newHeight As Integer) As Builder
				transformList.Add(New ResizeImageTransform(newWidth, newHeight))
				Return Me
			End Function

			Public Overridable Function rotateImageTransform(ByVal angle As Single) As Builder
				transformList.Add(New RotateImageTransform(angle))
				Return Me
			End Function

			Public Overridable Function rotateImageTransform(ByVal centerx As Single, ByVal centery As Single, ByVal angle As Single, ByVal scale As Single) As Builder
				transformList.Add(New RotateImageTransform(centerx, centery, angle, scale))
				Return Me
			End Function

			Public Overridable Function scaleImageTransform(ByVal delta As Single) As Builder
				transformList.Add(New ScaleImageTransform(delta))
				Return Me
			End Function

			Public Overridable Function scaleImageTransform(ByVal dx As Single, ByVal dy As Single) As Builder
				transformList.Add(New ScaleImageTransform(dx, dy))
				Return Me
			End Function

			Public Overridable Function warpImageTransform(ByVal delta As Single) As Builder
				transformList.Add(New WarpImageTransform(delta))
				Return Me
			End Function

			Public Overridable Function warpImageTransform(ByVal dx1 As Single, ByVal dy1 As Single, ByVal dx2 As Single, ByVal dy2 As Single, ByVal dx3 As Single, ByVal dy3 As Single, ByVal dx4 As Single, ByVal dy4 As Single) As Builder
				transformList.Add(New WarpImageTransform(dx1, dy1, dx2, dy2, dx3, dy3, dx4, dy4))
				Return Me
			End Function


			Public Overridable Function build() As ImageTransformProcess
				Return New ImageTransformProcess(Me)
			End Function

		End Class
	End Class

End Namespace