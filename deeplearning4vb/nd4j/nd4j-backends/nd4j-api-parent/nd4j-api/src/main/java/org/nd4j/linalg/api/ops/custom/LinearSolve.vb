﻿Imports System.Collections.Generic
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
'ORIGINAL LINE: @NoArgsConstructor public class LinearSolve extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class LinearSolve
		Inherits DynamicCustomOp

		Public Sub New(ByVal a As INDArray, ByVal b As INDArray, ByVal adjoint As Boolean)
			addInputArgument(a, b)
			addBArgument(adjoint)
		End Sub

		Public Sub New(ByVal a As INDArray, ByVal b As INDArray)
			Me.New(a,b,False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal a As SDVariable, ByVal b As SDVariable, ByVal adjoint As SDVariable)
			MyBase.New(sameDiff, New SDVariable() {a, b, adjoint})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal a As SDVariable, ByVal b As SDVariable, ByVal adjoint As Boolean)
			MyBase.New(sameDiff, New SDVariable() {a, b})
			addBArgument(adjoint)
		End Sub

		Public Overrides Function opName() As String
			Return "solve"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "MatrixSolve"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim adjoint As Boolean = If(attributesForNode.ContainsKey("adjoint"), attributesForNode("adjoint").getB(), False)
			addBArgument(adjoint)
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace