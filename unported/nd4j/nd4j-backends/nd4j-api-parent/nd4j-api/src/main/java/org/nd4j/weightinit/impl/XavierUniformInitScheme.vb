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

	Public Class XavierUniformInitScheme
		Inherits BaseWeightInitScheme

		Private fanIn As Double
		Private fanOut As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public XavierUniformInitScheme(char order, double fanIn, double fanOut)
		Public Sub New(ByVal order As Char, ByVal fanIn As Double, ByVal fanOut As Double)
			MyBase.New(order)
			Me.fanIn = fanIn
			Me.fanOut = fanOut
		End Sub


		Public Overrides Function doCreate(ByVal dataType As DataType, ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray
			'As per Glorot and Bengio 2010: Uniform distribution U(-s,s) with s = sqrt(6/(fanIn + fanOut))
			'Eq 16: http://jmlr.org/proceedings/papers/v9/glorot10a/glorot10a.pdf
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim s As Double = Math.Sqrt(6.0) / Math.Sqrt(fanIn + fanOut)
			Return Nd4j.rand(Nd4j.Distributions.createUniform(-s, s), shape)
		End Function


		Public Overrides Function type() As WeightInit
			Return WeightInit.XAVIER_UNIFORM
		End Function
	End Class

End Namespace