Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

	''' <summary>
	''' Uniform U[-a,a] with a=3.0/((fanIn + fanOut)/2)
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class WeightInitVarScalingUniformFanAvg implements IWeightInit
	<Serializable>
	Public Class WeightInitVarScalingUniformFanAvg
		Implements IWeightInit

		Private scale As Double?

		Public Sub New(ByVal scale As Double?)
			Me.scale = scale
		End Sub

		Public Overridable Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray Implements IWeightInit.init
			Dim scalingFanAvg As Double = 3.0 / Math.Sqrt((fanIn + fanOut) / 2)
			If scale IsNot Nothing Then
				scalingFanAvg *= scale
			End If

			Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-scalingFanAvg, scalingFanAvg))
			Return paramView.reshape(order, shape)
		End Function
	End Class

End Namespace