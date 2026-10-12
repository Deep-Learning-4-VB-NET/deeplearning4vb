Imports Frame = org.bytedeco.javacv.Frame
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports Mat = org.bytedeco.opencv.opencv_core.Mat
Imports org.datavec.api.transform
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.bytedeco.opencv.global.opencv_core.CV_32FC
import static org.bytedeco.opencv.global.opencv_core.CV_32FC3
import static org.bytedeco.opencv.global.opencv_core.CV_32S
import static org.bytedeco.opencv.global.opencv_core.CV_32SC
import static org.bytedeco.opencv.global.opencv_core.CV_32SC3
import static org.bytedeco.opencv.global.opencv_core.CV_64FC
import static org.bytedeco.opencv.global.opencv_core.CV_8UC3

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
Namespace org.deeplearning4j.rl4j.observation.transform.legacy

	Public Class EncodableToImageWritableTransform
		Implements Operation(Of Encodable, ImageWritable)

		Friend Shared ReadOnly nativeImageLoader As New NativeImageLoader()

		Public Overridable Function transform(ByVal encodable As Encodable) As ImageWritable
			Return New ImageWritable(nativeImageLoader.asFrame(encodable.Data, Frame.DEPTH_UBYTE))
		End Function

	End Class

End Namespace