Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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
Namespace org.nd4j.linalg.api.ops.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class Lstsq extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Lstsq
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Lstsq(@NonNull INDArray matrix, @NonNull INDArray rhs, double l2_regularizer, boolean fast)
		Public Sub New(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal l2_regularizer As Double, ByVal fast As Boolean)
			addInputArgument(matrix, rhs)
			addTArgument(l2_regularizer)
			addBArgument(fast)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Lstsq(@NonNull INDArray matrix, @NonNull INDArray rhs)
		Public Sub New(ByVal matrix As INDArray, ByVal rhs As INDArray)
			Me.New(matrix, rhs, 0.0, True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Lstsq(@NonNull SameDiff sameDiff, @NonNull SDVariable matrix, @NonNull SDVariable rhs, double l2_regularizer, boolean fast)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal l2_regularizer As Double, ByVal fast As Boolean)
			MyBase.New(sameDiff, New SDVariable(){matrix, rhs})
			addTArgument(l2_regularizer)
			addBArgument(fast)
		End Sub

		Public Overrides Function opName() As String
			Return "lstsq"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace