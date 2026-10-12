Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.javacpp
Imports org.bytedeco.tvm
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.bytedeco.tvm.global.tvm_runtime
Imports org.nd4j.tvm.util.TVMUtils

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
Namespace org.nd4j.tvm.runner

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TvmRunner implements java.io.Closeable
	Public Class TvmRunner
		Implements System.IDisposable

		Private Shared ctx As DLContext
		Private modFactory As org.bytedeco.tvm.Module
		Private values As TVMValue
		Private codes As IntPointer
		Private setter As TVMArgsSetter
		Private rv As TVMRetValue
		Private gmod As org.bytedeco.tvm.Module
		Private getNumInputs As PackedFunc
		Private getNumOutputs As PackedFunc
		Private setInput As PackedFunc
		Private getOutput As PackedFunc
		Private run As PackedFunc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public TvmRunner(String modelUri)
		Public Sub New(ByVal modelUri As String)
			If ctx Is Nothing Then
				ctx = (New DLContext()).device_type(kDLCPU).device_id(0)
				ctx.retainReference()
			End If

			' create the runtime module
			Using scope As New PointerScope()
				modFactory = org.bytedeco.tvm.Module.LoadFromFile(modelUri)
				values = New TVMValue(2)
				codes = New IntPointer(2)
				setter = New TVMArgsSetter(values, codes)
				setter.apply(0, ctx)
				rv = New TVMRetValue()
				modFactory.GetFunction("default").CallPacked(New TVMArgs(values, codes, 1), rv)
				gmod = rv.asModule()
				getNumInputs = gmod.GetFunction("get_num_inputs")
				getNumOutputs = gmod.GetFunction("get_num_outputs")
				setInput = gmod.GetFunction("set_input")
				getOutput = gmod.GetFunction("get_output")
				run = gmod.GetFunction("run")
				' retain the session reference to prevent pre emptive release of the session.
				modFactory.retainReference()
				values.retainReference()
				codes.retainReference()
				setter.retainReference()
				rv.retainReference()
				gmod.retainReference()
				getNumInputs.retainReference()
				getNumOutputs.retainReference()
				setInput.retainReference()
				getOutput.retainReference()
				run.retainReference()
			End Using
		End Sub

		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If run IsNot Nothing Then
				run.releaseReference()
			End If
			If getOutput IsNot Nothing Then
				getOutput.releaseReference()
			End If
			If setInput IsNot Nothing Then
				setInput.releaseReference()
			End If
			If getNumOutputs IsNot Nothing Then
				getNumOutputs.releaseReference()
			End If
			If getNumInputs IsNot Nothing Then
				getNumInputs.releaseReference()
			End If
			If gmod IsNot Nothing Then
				gmod.releaseReference()
			End If
			If rv IsNot Nothing Then
				rv.releaseReference()
			End If
			If setter IsNot Nothing Then
				setter.releaseReference()
			End If
			If codes IsNot Nothing Then
				codes.releaseReference()
			End If
			If values IsNot Nothing Then
				values.releaseReference()
			End If
			If modFactory IsNot Nothing Then
				modFactory.releaseReference()
			End If
		End Sub

		''' <summary>
		''' Execute the <seealso cref="run"/> function
		''' using the given input <seealso cref="System.Collections.IDictionary"/> </summary>
		''' <param name="input"> the input map </param>
		''' <returns> a map of the names of the ndarrays </returns>
		Public Overridable Function exec(ByVal input As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)
			Using scope As New PointerScope()
				getNumInputs.CallPacked(New TVMArgs(values, codes, 0), rv)
				Dim numInputNodes As Long = rv.asLong()
				getNumOutputs.CallPacked(New TVMArgs(values, codes, 0), rv)
				Dim numOutputNodes As Long = rv.asLong()

				' set the right input
				For Each e As KeyValuePair(Of String, INDArray) In input.SetOfKeyValuePairs()
					Dim name As String = e.Key
					Dim arr As INDArray = e.Value
					Dim inputTensor As DLTensor = getTensor(arr, ctx)
					Preconditions.checkState(inputTensor IsNot Nothing,"Input must be a tensor.")
					setter.apply(0, New BytePointer(name))
					setter.apply(1, inputTensor)
					setInput.CallPacked(New TVMArgs(values, codes, 2), rv)
				Next e

				' run the code
				run.CallPacked(New TVMArgs(values, codes, 0), rv)

				Dim ret As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()

				' get the output
				For i As Integer = 0 To numOutputNodes - 1
					setter.apply(0, i)
					getOutput.CallPacked(New TVMArgs(values, codes, 1), rv)
					Dim outputTensor As DLTensor = rv.asDLTensor()
					ret(Convert.ToString(i)) = getArray(outputTensor)
				Next i
				Return ret
			End Using
		End Function
	End Class

End Namespace