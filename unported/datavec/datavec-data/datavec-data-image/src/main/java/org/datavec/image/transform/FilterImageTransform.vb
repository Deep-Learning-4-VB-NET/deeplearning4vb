Imports System
Imports Data = lombok.Data
Imports FFmpegFrameFilter = org.bytedeco.javacv.FFmpegFrameFilter
Imports FrameFilter = org.bytedeco.javacv.FrameFilter
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports org.bytedeco.ffmpeg.global.avutil

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
'ORIGINAL LINE: @JsonIgnoreProperties({"filter", "converter"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class FilterImageTransform extends BaseImageTransform
	Public Class FilterImageTransform
		Inherits BaseImageTransform

		Private filter As FFmpegFrameFilter

		Private filters As String
		Private width As Integer
		Private height As Integer
		Private channels As Integer

		''' <summary>
		''' Calls {@code this(filters, width, height, 3)}. </summary>
		Public Sub New(ByVal filters As String, ByVal width As Integer, ByVal height As Integer)
			Me.New(filters, width, height, 3)
		End Sub

		''' <summary>
		''' Constructs a filtergraph out of the filter specification.
		''' </summary>
		''' <param name="filters">  to use </param>
		''' <param name="width">    of the input images </param>
		''' <param name="height">   of the input images </param>
		''' <param name="channels"> of the input images </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FilterImageTransform(@JsonProperty("filters") String filters, @JsonProperty("width") int width, @JsonProperty("height") int height, @JsonProperty("channels") int channels)
		Public Sub New(ByVal filters As String, ByVal width As Integer, ByVal height As Integer, ByVal channels As Integer)
			MyBase.New(Nothing)

			Me.filters = filters
			Me.width = width
			Me.height = height
			Me.channels = channels

			Dim pixelFormat As Integer = If(channels = 1, AV_PIX_FMT_GRAY8, If(channels = 3, AV_PIX_FMT_BGR24, If(channels = 4, AV_PIX_FMT_RGBA, AV_PIX_FMT_NONE)))
			If pixelFormat = AV_PIX_FMT_NONE Then
				Throw New System.ArgumentException("Unsupported number of channels: " & channels)
			End If
			Try
				filter = New FFmpegFrameFilter(filters, width, height)
				filter.setPixelFormat(pixelFormat)
				filter.start()
			Catch e As FrameFilter.Exception
				Throw New Exception(e)
			End Try
		End Sub

		Protected Friend Overrides Function doTransform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable
			If image Is Nothing Then
				Return Nothing
			End If
			Try
				filter.push(image.Frame)
				image = New ImageWritable(filter.pull())
			Catch e As FrameFilter.Exception
				Throw New Exception(e)
			End Try
			Return image
		End Function

	End Class

End Namespace