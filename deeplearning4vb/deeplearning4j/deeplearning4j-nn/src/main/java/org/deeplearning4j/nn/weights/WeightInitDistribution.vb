Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports Distributions = org.deeplearning4j.nn.conf.distribution.Distributions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OrthogonalDistribution = org.nd4j.linalg.api.rng.distribution.impl.OrthogonalDistribution
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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
'ORIGINAL LINE: @EqualsAndHashCode public class WeightInitDistribution implements IWeightInit
	<Serializable>
	Public Class WeightInitDistribution
		Implements IWeightInit

		Private ReadOnly distribution As Distribution

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WeightInitDistribution(@JsonProperty("distribution") org.deeplearning4j.nn.conf.distribution.Distribution distribution)
		Public Sub New(ByVal distribution As Distribution)
			If distribution Is Nothing Then
				' Would fail later below otherwise
				Throw New System.ArgumentException("Must set distribution!")
			End If
			Me.distribution = distribution
		End Sub

		Public Overridable Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray Implements IWeightInit.init
			'org.nd4j.linalg.api.rng.distribution.Distribution not serializable
			Dim dist As org.nd4j.linalg.api.rng.distribution.Distribution = Distributions.createDistribution(distribution)
			If TypeOf dist Is OrthogonalDistribution Then
				dist.sample(paramView.reshape(order, shape))
			Else
				dist.sample(paramView)
			End If
			Return paramView.reshape(order, shape)
		End Function
	End Class

End Namespace