Imports System
Imports org.datavec.api.transform
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


	Public Class ImageWritableToINDArrayTransform
		Implements Operation(Of ImageWritable, INDArray)

		Private ReadOnly loader As New NativeImageLoader()

		Public Overridable Function transform(ByVal imageWritable As ImageWritable) As INDArray

			Dim height As Integer = imageWritable.Height
			Dim width As Integer = imageWritable.Width
			Dim channels As Integer = imageWritable.Frame.imageChannels

			Dim [out] As INDArray = Nothing
			Try
				[out] = loader.asMatrix(imageWritable)
			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try

			' Convert back to uint8 and reshape to the number of channels in the image
			[out] = [out].reshape(ChrW(channels), height, width)
			Dim compressed As INDArray = [out].castTo(DataType.UINT8)
			Return compressed
		End Function
	End Class

End Namespace