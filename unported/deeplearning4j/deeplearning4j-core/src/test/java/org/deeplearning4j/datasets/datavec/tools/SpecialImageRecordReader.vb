Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.datasets.datavec.tools


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SpecialImageRecordReader extends org.datavec.image.recordreader.ImageRecordReader
	<Serializable>
	Public Class SpecialImageRecordReader
		Inherits ImageRecordReader

		Private counter As New AtomicInteger(0)
		Private labelsCounter As New AtomicInteger(0)
		Private limit, channels, width, height, numClasses As Integer
		Private Shadows labels_Conflict As IList(Of String) = New List(Of String)()
		Private zFeatures As INDArray



		Public Sub New(ByVal totalExamples As Integer, ByVal numClasses As Integer, ByVal channels As Integer, ByVal width As Integer, ByVal height As Integer)
			Me.limit = totalExamples
			Me.channels = channels
			Me.width = width
			Me.height = height
			Me.numClasses = numClasses

			For i As Integer = 0 To numClasses - 1
				labels_Conflict.Add("" & i)
			Next i

			zFeatures = Nd4j.create(128, channels, height, width)
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return counter.get() < limit
		End Function


		Public Overrides Sub reset()
			counter.set(0)
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			Dim features As INDArray = Nd4j.create(channels, height, width)
			fillNDArray(features, counter.getAndIncrement())
			features = features.reshape(ChrW(1), channels, height, width)
			Dim ret As IList(Of Writable) = RecordConverter.toRecord(features)
			ret.Add(New IntWritable(RandomUtils.nextInt(0, numClasses)))
			Return ret
		End Function

		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Return labels_Conflict
			End Get
		End Property


		Public Overrides Function batchesSupported() As Boolean
			Return True
		End Function

		Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Dim numExamples As Integer = Math.Min(num, limit - counter.get())
			'counter.addAndGet(numExamples);

			Dim features As INDArray = zFeatures
			For i As Integer = 0 To numExamples - 1
				fillNDArray(features.tensorAlongDimension(i, 1, 2, 3), counter.getAndIncrement())
			Next i

			Dim labels As INDArray = Nd4j.create(numExamples, numClasses)
			For i As Integer = 0 To numExamples - 1
				labels.getRow(i).assign(labelsCounter.getAndIncrement())
			Next i

			Dim ret As IList(Of Writable) = RecordConverter.toRecord(features)
			ret.Add(New NDArrayWritable(labels))

			Return Collections.singletonList(ret)
		End Function


		Protected Friend Overridable Sub fillNDArray(ByVal view As INDArray, ByVal value As Double)
			Dim pointer As Pointer = view.data().pointer()
			Dim shape As val = view.shape()
			'        log.info("Shape: {}", Arrays.toString(shape));

			Dim c As Integer = 0
			Do While c < shape(0)
				Dim h As Integer = 0
				Do While h < shape(1)
					Dim w As Integer = 0
					Do While w < shape(2)
						view.putScalar(c, h, w, CSng(value))
						w += 1
					Loop
					h += 1
				Loop
				c += 1
			Loop

	'        
	'        if (pointer instanceof FloatPointer) {
	'            FloatIndexer idx = FloatIndexer.create((FloatPointer) pointer, new long[]{view.shape()[0], view.shape()[1], view.shape()[2]}, new long[]{view.stride()[0], view.stride()[1], view.stride()[2]});
	'            for (long c = 0; c < shape[0]; c++) {
	'                for (long h = 0; h < shape[1]; h++) {
	'                    for (long w = 0; w < shape[2]; w++) {
	'                        idx.put(c, h, w, (float) value);
	'                    }
	'                }
	'            }
	'        } else {
	'            DoubleIndexer idx = DoubleIndexer.create((DoublePointer) pointer, new long[]{view.shape()[0], view.shape()[1], view.shape()[2]}, new long[]{view.stride()[0], view.stride()[1], view.stride()[2]});
	'            for (long c = 0; c < shape[0]; c++) {
	'                for (long h = 0; h < shape[1]; h++) {
	'                    for (long w = 0; w < shape[2]; w++) {
	'                        idx.put(c, h, w, value);
	'                    }
	'                }
	'            }
	'        }
	'        
		End Sub
	End Class
End Namespace