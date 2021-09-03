Imports System
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

Namespace org.nd4j.linalg.activations


	<Serializable>
	Public MustInherit Class BaseActivationFunction
		Implements IActivation

		Public MustOverride Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As org.nd4j.common.primitives.Pair(Of INDArray, INDArray) Implements IActivation.backprop
		Public MustOverride Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray

		Public Overridable Function numParams(ByVal inputSize As Integer) As Integer Implements IActivation.numParams
			Return 0
		End Function

		Protected Friend Overridable Sub assertShape(ByVal [in] As INDArray, ByVal epsilon As INDArray)
			If Not [in].equalShapes(epsilon) Then
				Throw New System.InvalidOperationException("Shapes must be equal during backprop: in.shape{} = " & Arrays.toString([in].shape()) & ", epsilon.shape() = " & Arrays.toString(epsilon.shape()))
			End If
		End Sub
	End Class

End Namespace