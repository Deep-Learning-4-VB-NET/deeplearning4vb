Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.bp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class PowBp extends org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
	Public Class PowBp
		Inherits BaseDynamicTransformOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal dLdz As SDVariable)
			MyBase.New(sameDiff,New SDVariable(){x, y, dLdz}, False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal dLdz As INDArray, ByVal dLdx As INDArray, ByVal dLdy As INDArray)
			MyBase.New(New INDArray(){x, y, dLdz}, New INDArray(){dLdx, dLdy})
		End Sub

		Public Overrides Function opName() As String
			Return "Pow_bp"
		End Function

		Public Overrides ReadOnly Property InplaceCall As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 input datatypes for %s, got input %s", Me.GetType(), dataTypes)
			'Gradient types: same as input
			Return New List(Of DataType) From {arg(0).dataType(), arg(1).dataType()}
		End Function
	End Class

End Namespace