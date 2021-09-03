Imports System
Imports Builder = lombok.Builder
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BaseWeightInitScheme = org.nd4j.weightinit.BaseWeightInitScheme
Imports WeightInit = org.nd4j.weightinit.WeightInit

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

Namespace org.nd4j.weightinit.impl

	''' <summary>
	''' Initialize the weight to:
	''' range = 1.0 / Math.sqrt(fanIn)
	''' U(-range,range)
	''' @author Adam Gibson
	''' </summary>
	Public Class UniformInitScheme
		Inherits BaseWeightInitScheme

		Private fanIn As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public UniformInitScheme(char order, double fanIn)
		Public Sub New(ByVal order As Char, ByVal fanIn As Double)
			MyBase.New(order)
			Me.fanIn = fanIn
		End Sub

		Public Overrides Function doCreate(ByVal dataType As DataType, ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray
			Dim a As Double = 1.0 / Math.Sqrt(fanIn)
			Return Nd4j.rand(Nd4j.Distributions.createUniform(-a, a), shape)
		End Function


		Public Overrides Function type() As WeightInit
			Return WeightInit.UNIFORM
		End Function
	End Class

End Namespace