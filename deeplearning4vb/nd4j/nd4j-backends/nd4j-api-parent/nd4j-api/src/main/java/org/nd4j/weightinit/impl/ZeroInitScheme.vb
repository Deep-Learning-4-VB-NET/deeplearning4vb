Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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
	''' Initialize the weight to zero.
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class ZeroInitScheme extends org.nd4j.weightinit.BaseWeightInitScheme
	Public Class ZeroInitScheme
		Inherits BaseWeightInitScheme

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public ZeroInitScheme(char order)
		Public Sub New(ByVal order As Char)
			MyBase.New(order)
		End Sub

		Public Overrides Function doCreate(ByVal dataType As DataType, ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray
			If shape Is Nothing Then
				Throw New ND4JIllegalStateException("Shape must not be null!")
			End If
			Return Nd4j.createUninitialized(dataType, shape, order()).assign(0.0f)
		End Function


		Public Overrides Function type() As WeightInit
			Return WeightInit.ZERO
		End Function
	End Class

End Namespace