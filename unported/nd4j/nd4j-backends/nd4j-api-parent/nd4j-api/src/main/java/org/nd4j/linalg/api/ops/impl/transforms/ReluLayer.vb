Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports XwPlusB = org.nd4j.linalg.api.ops.impl.transforms.custom.XwPlusB

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

Namespace org.nd4j.linalg.api.ops.impl.transforms



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class ReluLayer extends org.nd4j.linalg.api.ops.impl.transforms.custom.XwPlusB
	Public Class ReluLayer
		Inherits XwPlusB


		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable)
			MyBase.New(sameDiff, input, weights, bias)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReluLayer(@NonNull INDArray input, @NonNull INDArray weights, @NonNull INDArray bias)
		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray)
			MyBase.New(New INDArray(){input, weights, bias}, Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "relu_layer"
		End Function


		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow name found for shape " & opName())
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
		End Function

		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO a native implementation would be faster
			'Backprop through ReLU, then it's same as XwPlusB
			Dim args() As SDVariable = Me.args()
			Dim xwb As SDVariable = sameDiff.nn().linear(args(0), args(1), (If(args.Length = 2, Nothing, args(2))))
			Dim grad As SDVariable = gradient(0).mul(sameDiff.math().step(xwb, 0))
			Return MyBase.doDiff(Collections.singletonList(grad))
		End Function

	End Class

End Namespace