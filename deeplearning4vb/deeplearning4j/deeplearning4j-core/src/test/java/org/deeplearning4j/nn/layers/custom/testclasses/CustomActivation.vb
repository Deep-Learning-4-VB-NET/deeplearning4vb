Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports IActivation = org.nd4j.linalg.activations.IActivation
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

Namespace org.deeplearning4j.nn.layers.custom.testclasses

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class CustomActivation extends org.nd4j.linalg.activations.BaseActivationFunction implements org.nd4j.linalg.activations.IActivation
	<Serializable>
	Public Class CustomActivation
		Inherits BaseActivationFunction
		Implements IActivation

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray Implements IActivation.getActivation
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray) Implements IActivation.backprop
			Return New Pair(Of INDArray, INDArray)([in].muli(epsilon), Nothing)
		End Function
	End Class

End Namespace