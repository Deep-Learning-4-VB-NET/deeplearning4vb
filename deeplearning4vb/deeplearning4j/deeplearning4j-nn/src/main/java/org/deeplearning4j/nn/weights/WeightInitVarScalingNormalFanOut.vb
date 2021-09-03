Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TruncatedNormalDistribution = org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution
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
'ORIGINAL LINE: @Data @NoArgsConstructor public class WeightInitVarScalingNormalFanOut implements IWeightInit
	<Serializable>
	Public Class WeightInitVarScalingNormalFanOut
		Implements IWeightInit

		Private scale As Double?

		Public Sub New(ByVal scale As Double?)
			Me.scale = scale
		End Sub

		Public Overridable Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray Implements IWeightInit.init
			Dim std As Double
			If scale Is Nothing Then
				std = Math.Sqrt(1.0 / fanOut)
			Else
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				std = Math.Sqrt(scale.Value / fanOut)
			End If

			Nd4j.exec(New TruncatedNormalDistribution(paramView, 0.0, std))
			Return paramView.reshape(order, shape)
		End Function
	End Class

End Namespace