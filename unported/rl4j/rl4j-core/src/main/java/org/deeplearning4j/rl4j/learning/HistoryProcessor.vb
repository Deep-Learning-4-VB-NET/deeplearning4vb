Imports System
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CircularFifoQueue = org.apache.commons.collections4.queue.CircularFifoQueue
Imports org.bytedeco.javacv
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports VideoRecorder = org.deeplearning4j.rl4j.util.VideoRecorder
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.rl4j.learning


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class HistoryProcessor implements IHistoryProcessor
	Public Class HistoryProcessor
		Implements IHistoryProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Configuration conf;
		Private ReadOnly conf As Configuration
'JAVA TO VB CONVERTER NOTE: The field history was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private history_Conflict As CircularFifoQueue(Of INDArray)
		Private videoRecorder As VideoRecorder

		Public Sub New(ByVal conf As Configuration)
			Me.conf = conf
			history_Conflict = New CircularFifoQueue(Of INDArray)(conf.getHistoryLength())
		End Sub


		Public Overridable Sub add(ByVal obs As INDArray) Implements IHistoryProcessor.add
			Dim processed As INDArray = transform(obs)
			history_Conflict.add(processed)
		End Sub

		Public Overridable Sub startMonitor(ByVal filename As String, ByVal shape() As Integer) Implements IHistoryProcessor.startMonitor
			If videoRecorder Is Nothing Then
				videoRecorder = VideoRecorder.builder(shape(1), shape(2)).build()
			End If

			Try
				videoRecorder.startRecording(filename)
			Catch e As Exception
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
		End Sub

		Public Overridable Sub stopMonitor() Implements IHistoryProcessor.stopMonitor
			If videoRecorder IsNot Nothing Then
				Try
					videoRecorder.stopRecording()
				Catch e As Exception
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				End Try
			End If
		End Sub

		Public Overridable ReadOnly Property Monitoring As Boolean Implements IHistoryProcessor.isMonitoring
			Get
				Return videoRecorder IsNot Nothing AndAlso videoRecorder.Recording
			End Get
		End Property

		Public Overridable Sub record(ByVal pixelArray As INDArray) Implements IHistoryProcessor.record
			If Monitoring Then
				' before accessing the raw pointer, we need to make sure that array is actual on the host side
				Nd4j.AffinityManager.ensureLocation(pixelArray, AffinityManager.Location.HOST)

				Try
					videoRecorder.record(pixelArray)
				Catch e As Exception
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				End Try
			End If
		End Sub

		Public Overridable ReadOnly Property History As INDArray() Implements IHistoryProcessor.getHistory
			Get
				Dim array(Conf.getHistoryLength() - 1) As INDArray
				Dim i As Integer = 0
				Do While i < conf.getHistoryLength()
					array(i) = history_Conflict.get(i).castTo(Nd4j.dataType())
					i += 1
				Loop
				Return array
			End Get
		End Property


		Private Function transform(ByVal raw As INDArray) As INDArray
			Dim shape() As Long = raw.shape()

			' before accessing the raw pointer, we need to make sure that array is actual on the host side
			Nd4j.AffinityManager.ensureLocation(raw, AffinityManager.Location.HOST)

			Dim ocvmat As New Mat(CInt(shape(0)), CInt(shape(1)), CV_32FC(3), raw.data().pointer())
			Dim cvmat As New Mat(shape(0), shape(1), CV_8UC(3))
			ocvmat.convertTo(cvmat, CV_8UC(3), 255.0, 0.0)
			cvtColor(cvmat, cvmat, COLOR_RGB2GRAY)
			Dim resized As New Mat(conf.getRescaledHeight(), conf.getRescaledWidth(), CV_8UC(1))
			resize(cvmat, resized, New Size(conf.getRescaledWidth(), conf.getRescaledHeight()))
			'   show(resized);
			'   waitKP();
			'Crop by croppingHeight, croppingHeight
			Dim cropped As Mat = resized.apply(New Rect(conf.getOffsetX(), conf.getOffsetY(), conf.getCroppingWidth(), conf.getCroppingHeight()))
			'System.out.println(conf.getCroppingWidth() + " " + cropped.data().asBuffer().array().length);

			Dim [out] As INDArray = Nothing
			Try
				[out] = (New NativeImageLoader(conf.getCroppingHeight(), conf.getCroppingWidth())).asMatrix(cropped)
			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
			'System.out.println(out.shapeInfoToString());
			[out] = [out].reshape(ChrW(1), conf.getCroppingHeight(), conf.getCroppingWidth())
			Dim compressed As INDArray = [out].castTo(DataType.UBYTE)
			Return compressed
		End Function

		Public Overridable ReadOnly Property Scale As Double Implements IHistoryProcessor.getScale
			Get
				Return 255
			End Get
		End Property

		Public Overridable Sub waitKP()
			Try
				Console.Read()
			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
		End Sub

		Public Overridable Sub show(ByVal m As Mat)
			Dim converter As New OpenCVFrameConverter.ToMat()
			Dim canvas As New CanvasFrame("LOL", 1)
			canvas.showImage(converter.convert(m))
		End Sub


	End Class

End Namespace