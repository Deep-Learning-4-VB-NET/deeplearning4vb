Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.ops.impl.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class MmulBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class MmulBp
		Inherits DynamicCustomOp

		Protected Friend mt As MMulTranspose


		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal eps As SDVariable, ByVal mt As MMulTranspose)
			MyBase.New(Nothing,sameDiff,New SDVariable(){x, y, eps})
			Me.mt = mt
			addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()), ArrayUtil.fromBoolean(mt.isTransposeResult()))
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal eps As SDVariable)
			Me.New(sameDiff,x,y, eps, MMulTranspose.allFalse())
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal eps As INDArray, ByVal dldx As INDArray, ByVal dldy As INDArray, ByVal mt As MMulTranspose)
			MyBase.New(Nothing, New INDArray(){x, y, eps}, New INDArray(){dldx, dldy})
			If mt IsNot Nothing Then
			  Me.mt = mt
			  addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()), ArrayUtil.fromBoolean(mt.isTransposeResult()))
			End If
		End Sub


		Public Sub New()
		End Sub


		Public Overrides Function opName() As String
			Return "matmul_bp"
		End Function


		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function


		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 inputs to matmul_bp op, got %s", dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType() AndAlso dataTypes(0).isFPType(), "Inputs to matmul_bp op must both be a floating" & "point type: got %s", dataTypes)
			Return dataTypes.subList(0, 2)
		End Function
	End Class


End Namespace