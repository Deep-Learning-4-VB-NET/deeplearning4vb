Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FFmpegFrameRecorder = org.bytedeco.javacv.FFmpegFrameRecorder
Imports Frame = org.bytedeco.javacv.Frame
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.bytedeco.ffmpeg.global.avcodec.AV_CODEC_ID_H264

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

Namespace org.deeplearning4j.rl4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class VideoRecorder implements AutoCloseable
	Public Class VideoRecorder
		Implements AutoCloseable

		Private ReadOnly nativeImageLoader As New NativeImageLoader()

		Private ReadOnly height As Integer
		Private ReadOnly width As Integer
		Private ReadOnly codec As Integer
		Private ReadOnly framerate As Double
		Private ReadOnly videoQuality As Integer

		Private fmpegFrameRecorder As FFmpegFrameRecorder = Nothing

		''' <returns> True if the instance is recording a video </returns>
		Public Overridable ReadOnly Property Recording As Boolean
			Get
				Return fmpegFrameRecorder IsNot Nothing
			End Get
		End Property

		Private Sub New(ByVal builder As Builder)
			Me.height = builder.height
			Me.width = builder.width
			codec = builder.codec_Conflict
			framerate = builder.frameRate_Conflict
			videoQuality = builder.videoQuality_Conflict
		End Sub

		''' <summary>
		''' Initiate the recording of a video </summary>
		''' <param name="filename"> Name of the video file to create </param>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void startRecording(String filename) throws Exception
		Public Overridable Sub startRecording(ByVal filename As String)
			stopRecording()

			fmpegFrameRecorder = New FFmpegFrameRecorder(filename, width, height)
			fmpegFrameRecorder.setVideoCodec(codec)
			fmpegFrameRecorder.setFrameRate(framerate)
			fmpegFrameRecorder.setVideoQuality(videoQuality)
			fmpegFrameRecorder.start()
		End Sub

		''' <summary>
		''' Terminates the recording of the video </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void stopRecording() throws Exception
		Public Overridable Sub stopRecording()
			If fmpegFrameRecorder IsNot Nothing Then
				fmpegFrameRecorder.stop()
				fmpegFrameRecorder.release()
			End If
			fmpegFrameRecorder = Nothing
		End Sub

		''' <summary>
		''' Add a frame to the video </summary>
		''' <param name="imageArray"> the INDArray that contains the data to be recorded, the data must be in CHW format </param>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void record(org.nd4j.linalg.api.ndarray.INDArray imageArray) throws Exception
		Public Overridable Sub record(ByVal imageArray As INDArray)
			fmpegFrameRecorder.record(nativeImageLoader.asFrame(imageArray, Frame.DEPTH_UBYTE))
		End Sub

		''' <summary>
		''' Terminate the recording and close the video file </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void close() throws Exception
		Public Overridable Sub close()
			stopRecording()
		End Sub

		''' 
		''' <param name="height"> The height of the video </param>
		''' <param name="width"> Thw width of the video </param>
		''' <returns> A VideoRecorder builder </returns>
		Public Shared Function builder(ByVal height As Integer, ByVal width As Integer) As Builder
			Return New Builder(height, width)
		End Function

		''' <summary>
		''' A builder class for the VideoRecorder
		''' </summary>
		Public Class Builder
			Friend ReadOnly height As Integer
			Friend ReadOnly width As Integer
'JAVA TO VB CONVERTER NOTE: The field codec was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend codec_Conflict As Integer = AV_CODEC_ID_H264
'JAVA TO VB CONVERTER NOTE: The field frameRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend frameRate_Conflict As Double = 30.0
'JAVA TO VB CONVERTER NOTE: The field videoQuality was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend videoQuality_Conflict As Integer = 30

			''' <param name="height"> The height of the video </param>
			''' <param name="width"> The width of the video </param>
			Public Sub New(ByVal height As Integer, ByVal width As Integer)
				Me.height = height
				Me.width = width
			End Sub

			''' <summary>
			''' The codec to use for the video. Default is AV_CODEC_ID_H264 </summary>
			''' <param name="codec"> Code (see <seealso cref="org.bytedeco.ffmpeg.global.avcodec codec codes"/>) </param>
'JAVA TO VB CONVERTER NOTE: The parameter codec was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function codec(ByVal codec_Conflict As Integer) As Builder
				Me.codec_Conflict = codec_Conflict
				Return Me
			End Function

			''' <summary>
			''' The frame rate of the video. Default is 30.0 </summary>
			''' <param name="frameRate"> The frame rate
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter frameRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function frameRate(ByVal frameRate_Conflict As Double) As Builder
				Me.frameRate_Conflict = frameRate_Conflict
				Return Me
			End Function

			''' <summary>
			''' The video quality. Default is 30 </summary>
			''' <param name="videoQuality">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter videoQuality was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function videoQuality(ByVal videoQuality_Conflict As Integer) As Builder
				Me.videoQuality_Conflict = videoQuality_Conflict
				Return Me
			End Function

			''' <summary>
			''' Build an instance of the configured VideoRecorder </summary>
			''' <returns> A VideoRecorder instance </returns>
			Public Overridable Function build() As VideoRecorder
				Return New VideoRecorder(Me)
			End Function
		End Class
	End Class

End Namespace