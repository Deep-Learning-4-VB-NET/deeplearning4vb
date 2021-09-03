Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
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

Namespace org.deeplearning4j.nn.weights

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class WeightInitUniform implements IWeightInit
	<Serializable>
	Public Class WeightInitUniform
		Implements IWeightInit

		Public Overridable Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray Implements IWeightInit.init
			Dim a As Double = 1.0 / Math.Sqrt(fanIn)
			Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-a, a))
			Return paramView.reshape(order, shape)
		End Function
	End Class

End Namespace