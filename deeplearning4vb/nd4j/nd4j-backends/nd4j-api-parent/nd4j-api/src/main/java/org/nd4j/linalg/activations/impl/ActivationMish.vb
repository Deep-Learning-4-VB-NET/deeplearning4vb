Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports val = lombok.val
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Mish = org.nd4j.linalg.api.ops.impl.transforms.strict.Mish
Imports MishDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.MishDerivative
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.activations.impl

	''' <summary>
	''' https://arxiv.org/ftp/arxiv/papers/1908/1908.08681.pdf
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationMish extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationMish
		Inherits BaseActivationFunction

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			Nd4j.Executioner.execAndReturn(New Mish([in]))
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)

			Dim dLdZ As val = Nd4j.Executioner.exec(New MishDerivative([in], [in]))

			dLdZ.muli(epsilon)
			Return New Pair(Of INDArray, INDArray)([in], Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "mish"
		End Function

	End Class

End Namespace