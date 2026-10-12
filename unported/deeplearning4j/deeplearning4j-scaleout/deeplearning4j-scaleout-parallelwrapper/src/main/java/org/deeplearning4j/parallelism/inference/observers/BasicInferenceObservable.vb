Imports System
Imports System.Collections.Generic
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InferenceObservable = org.deeplearning4j.parallelism.inference.InferenceObservable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.parallelism.inference.observers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BasicInferenceObservable extends java.util.Observable implements org.deeplearning4j.parallelism.inference.InferenceObservable
	Public Class BasicInferenceObservable
		Inherits Observable
		Implements InferenceObservable

		Private input() As INDArray
		Private inputMasks() As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long id;
		Private id As Long
'JAVA TO VB CONVERTER NOTE: The field output was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private output_Conflict() As INDArray
		Protected Friend exception As Exception


		Public Sub New(ParamArray ByVal inputs() As INDArray)
			Me.New(inputs, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal inputMasks() As INDArray)
			MyBase.New()
			Me.input = inputs
			Me.inputMasks = inputMasks
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void addInput(@NonNull INDArray... input)
		Public Overridable Sub addInput(ParamArray ByVal input() As INDArray)
			addInput(input, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void addInput(@NonNull INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks)
		Public Overridable Sub addInput(ByVal input() As INDArray, ByVal inputMasks() As INDArray)
			Me.input = input
			Me.inputMasks = inputMasks
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setOutputBatches(@NonNull List<org.nd4j.linalg.api.ndarray.INDArray[]> output)
		Public Overridable WriteOnly Property OutputBatches As IList(Of INDArray())
			Set(ByVal output As IList(Of INDArray()))
				Preconditions.checkArgument(output.size() = 1, "Expected size 1 output: got size " & output.size())
				Me.output_Conflict = output.get(0)
				Me.setChanged()
				notifyObservers()
			End Set
		End Property

		Public Overridable ReadOnly Property InputBatches As IList(Of Pair(Of INDArray(), INDArray())) Implements InferenceObservable.getInputBatches
			Get
				Return Collections.singletonList(New Pair(Of )(input, inputMasks))
			End Get
		End Property

		Public Overridable WriteOnly Property OutputException Implements InferenceObservable.setOutputException As Exception
			Set(ByVal exception As Exception)
				Me.exception = exception
				Me.setChanged()
				notifyObservers()
			End Set
		End Property

		Public Overridable ReadOnly Property Output As INDArray() Implements InferenceObservable.getOutput
			Get
				checkOutputException()
				Return output_Conflict
			End Get
		End Property

		Protected Friend Overridable Sub checkOutputException()
			If exception IsNot Nothing Then
				If TypeOf exception Is Exception Then
					Throw CType(exception, Exception)
				Else
					Throw New Exception("Exception encountered while getting output: " & exception.Message, exception)
				End If
			End If
		End Sub
	End Class

End Namespace