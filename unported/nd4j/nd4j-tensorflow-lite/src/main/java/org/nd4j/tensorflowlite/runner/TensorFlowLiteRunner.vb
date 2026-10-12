Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.javacpp
Imports org.bytedeco.tensorflowlite
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports TFLiteUtils = org.nd4j.tensorflowlite.util.TFLiteUtils
Imports org.bytedeco.tensorflowlite.global.tensorflowlite
Imports org.nd4j.tensorflowlite.util.TFLiteUtils

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
Namespace org.nd4j.tensorflowlite.runner

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TensorFlowLiteRunner implements java.io.Closeable
	Public Class TensorFlowLiteRunner
		Implements System.IDisposable

		Private model As FlatBufferModel
		Private resolver As BuiltinOpResolver
		Private builder As InterpreterBuilder
		Private interpreter As Interpreter

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public TensorFlowLiteRunner(String modelUri)
		Public Sub New(ByVal modelUri As String)
			' Load model
			model = FlatBufferModel.BuildFromFile(modelUri)
			If model Is Nothing OrElse model.isNull() Then
				Throw New Exception("Cannot load " & modelUri)
			End If
			'retain the model reference to prevent pre emptive release of the model.
			model.retainReference()

			' Build the interpreter with the InterpreterBuilder.
			' Note: all Interpreters should be built with the InterpreterBuilder,
			' which allocates memory for the Interpreter and does various set up
			' tasks so that the Interpreter can read the provided model.
			resolver = New BuiltinOpResolver()
			builder = New InterpreterBuilder(model, resolver)
			interpreter = New Interpreter(DirectCast(Nothing, Pointer))
			builder.apply(interpreter)
			If interpreter Is Nothing OrElse interpreter.isNull() Then
				Throw New Exception("Cannot build interpreter for " & modelUri)
			End If
			resolver.retainReference()
			builder.retainReference()
			interpreter.retainReference()

			' Allocate tensor buffers.
			If interpreter.AllocateTensors() <> kTfLiteOk Then
				Throw New Exception("Cannot allocate tensors for " & modelUri)
			End If
			If log.isInfoEnabled() Then
				log.info("=== Pre-invoke Interpreter State ===")
				PrintInterpreterState(interpreter)
			End If
		End Sub

		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If interpreter IsNot Nothing Then
				interpreter.releaseReference()
			End If
			If builder IsNot Nothing Then
				builder.releaseReference()
			End If
			If resolver IsNot Nothing Then
				resolver.releaseReference()
			End If
			If model IsNot Nothing Then
				model.releaseReference()
			End If
		End Sub

		''' <summary>
		''' Execute the <seealso cref="session"/>
		''' using the given input <seealso cref="System.Collections.IDictionary"/>
		''' input </summary>
		''' <param name="input"> the input map </param>
		''' <returns> a map of the names of the ndarrays </returns>
		Public Overridable Function exec(ByVal input As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)
			Dim numInputNodes As Long = interpreter.inputs().capacity()
			Dim numOutputNodes As Long = interpreter.outputs().capacity()

			' Fill input buffers
			For i As Integer = 0 To numInputNodes - 1
				Dim inputName As BytePointer = interpreter.GetInputName(i)
				Dim arr As INDArray = input(inputName.getString())
				Dim inputTensor As TfLiteTensor = interpreter.input_tensor(i)
				Preconditions.checkState(inputTensor IsNot Nothing,"Input must be a tensor.")
				Nd4j.copy(arr, getArray(inputTensor))
			Next i

			' Run inference
			If interpreter.Invoke() <> kTfLiteOk Then
				Throw New Exception("Cannot invoke interpreter for " & model)
			End If
			If log.isInfoEnabled() Then
				log.info("=== Post-invoke Interpreter State ===")
				PrintInterpreterState(interpreter)
			End If

			Dim ret As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()

			' Read output buffers
			For i As Integer = 0 To numOutputNodes - 1
				Dim outputTensor As TfLiteTensor = interpreter.output_tensor(i)
				ret(interpreter.GetOutputName(i).getString()) = getArray(outputTensor)
			Next i
			Return ret
		End Function
	End Class

End Namespace