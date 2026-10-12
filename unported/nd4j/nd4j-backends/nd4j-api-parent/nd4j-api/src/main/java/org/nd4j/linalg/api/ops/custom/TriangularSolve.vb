Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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
'ORIGINAL LINE: @NoArgsConstructor public class TriangularSolve extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class TriangularSolve
		Inherits DynamicCustomOp

		Public Sub New(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal lower As Boolean, ByVal adjoint As Boolean)
			addInputArgument(matrix, rhs)
			addBArgument(lower, adjoint)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal lower As SDVariable, ByVal adjoint As SDVariable)
			MyBase.New(sameDiff, New SDVariable() {matrix, rhs, lower, adjoint})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal lower As Boolean, ByVal adjoint As Boolean)
			MyBase.New(sameDiff, New SDVariable() {matrix, rhs})
			addBArgument(lower, adjoint)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If attributesForNode.ContainsKey("lower") Then
				addBArgument(attributesForNode("lower").getB())
			End If

			If attributesForNode.ContainsKey("adjoint") Then
				addBArgument(attributesForNode("adjoint").getB())
			End If
		End Sub

		Public Overrides Function opName() As String
			Return "triangular_solve"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "MatrixTriangularSolve"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace