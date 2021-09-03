Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.javacpp
Imports org.bytedeco.onnxruntime
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ONNXUtils = org.nd4j.onnxruntime.util.ONNXUtils
Imports org.bytedeco.onnxruntime.global.onnxruntime
import static org.nd4j.onnxruntime.util.ONNXUtils.getDataBuffer
import static org.nd4j.onnxruntime.util.ONNXUtils.getTensor

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
Namespace org.nd4j.onnxruntime.runner


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class OnnxRuntimeRunner implements java.io.Closeable
	Public Class OnnxRuntimeRunner
		Implements System.IDisposable

		Private session As Session
		Private runOptions As RunOptions
		Private memoryInfo As MemoryInfo
		Private allocator As AllocatorWithDefaultOptions
		Private sessionOptions As SessionOptions
		Private Shared env As Env
		Private bp As Pointer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public OnnxRuntimeRunner(String modelUri)
		Public Sub New(ByVal modelUri As String)
			If env Is Nothing Then
				env = New Env(ONNXUtils.getOnnxLogLevelFromLogger(log), New BytePointer("nd4j-serving-onnx-session-" & System.Guid.randomUUID().ToString()))
				env.retainReference()
			End If

			sessionOptions = New SessionOptions()
			sessionOptions.SetGraphOptimizationLevel(ORT_ENABLE_EXTENDED)
			sessionOptions.SetIntraOpNumThreads(1)
			sessionOptions.retainReference()
			allocator = New AllocatorWithDefaultOptions()
			allocator.retainReference()
			bp = If(Loader.getPlatform().ToLower().StartsWith("windows", StringComparison.Ordinal), New CharPointer(modelUri), New BytePointer(modelUri))
			runOptions = New RunOptions()
			memoryInfo = MemoryInfo.CreateCpu(OrtArenaAllocator, OrtMemTypeDefault)
			session = New Session(env, bp, sessionOptions)
			'retain the session reference to prevent pre emptive release of the session.
			session.retainReference()

		End Sub



		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If session IsNot Nothing Then
				session.close()
			End If

			sessionOptions.releaseReference()
			allocator.releaseReference()
			runOptions.releaseReference()
		End Sub


		''' <summary>
		''' Execute the <seealso cref="session"/>
		''' using the given input <seealso cref="System.Collections.IDictionary"/>
		''' input </summary>
		''' <param name="input"> the input map </param>
		''' <returns> a map of the names of the ndarrays </returns>
		Public Overridable Function exec(ByVal input As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)
			Dim numInputNodes As Long = session.GetInputCount()
			Dim numOutputNodes As Long = session.GetOutputCount()

			Dim inputNodeNames As New PointerPointer(Of BytePointer)(numInputNodes)
			Dim outputNodeNames As New PointerPointer(Of BytePointer)(numOutputNodes)

			Dim inputVal As New Value(numInputNodes)

			For i As Integer = 0 To numInputNodes - 1
				Dim inputName As BytePointer = session.GetInputName(i, allocator.asOrtAllocator())
				inputNodeNames.put(i, inputName)
				Dim arr As INDArray = input(inputName.getString())
				Dim inputTensor As Value = getTensor(arr, memoryInfo)
				Preconditions.checkState(inputTensor.IsTensor(),"Input must be a tensor.")
				inputVal.position(i).put(inputTensor)
			Next i

			'reset position after iterating
			inputVal.position(0)



			For i As Integer = 0 To numOutputNodes - 1
				Dim outputName As BytePointer = session.GetOutputName(i, allocator.asOrtAllocator())
				outputNodeNames.put(i, outputName)
			Next i

			Dim outputVector As ValueVector = session.Run(runOptions, inputNodeNames, inputVal, numInputNodes, outputNodeNames, numOutputNodes)

			outputVector.retainReference()
			Dim ret As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()

			For i As Integer = 0 To numOutputNodes - 1
				Dim outValue As Value = outputVector.get(i)
				outValue.retainReference()
				Dim typeInfo As TypeInfo = session.GetOutputTypeInfo(i)
				Dim buffer As DataBuffer = getDataBuffer(outValue)
				Dim longPointer As LongPointer = outValue.GetTensorTypeAndShapeInfo().GetShape()
				'shape info can be null
				If longPointer IsNot Nothing Then
					Dim shape(CInt(longPointer.capacity()) - 1) As Long
					longPointer.get(shape)
					ret((outputNodeNames.get(GetType(BytePointer), i)).getString()) = Nd4j.create(buffer).reshape(shape)
				Else
					ret((outputNodeNames.get(GetType(BytePointer), i)).getString()) = Nd4j.create(buffer)

				End If
			Next i

			Return ret


		End Function


	End Class

End Namespace