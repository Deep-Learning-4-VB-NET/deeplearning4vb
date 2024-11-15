﻿Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Type = org.nd4j.linalg.api.ops.Op.Type
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

Namespace org.nd4j.linalg.api.ops.impl.controlflow.compat


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class @While extends BaseCompatOp
	Public Class [While]
		Inherits BaseCompatOp

		Protected Friend isConstant As Boolean

		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable)
			MyBase.New(sameDiff, inputs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal frameName As String, ByVal input As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){input})
			Me.frameName_Conflict = frameName
			isConstant = input.Constant
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal frameName As String, ByVal input As SDVariable, ByVal isConstant As Boolean)
			MyBase.New(sameDiff, New SDVariable(){input})
			Me.frameName_Conflict = frameName
			Me.isConstant = isConstant
		End Sub

		''' <summary>
		''' WARNING: do not change without changing serialization methods
		''' See <seealso cref="org.nd4j.autodiff.samediff.serde.FlatBuffersMapper.getOpNum(String, Type)"/>
		'''  and <seealso cref="org.nd4j.imports.converters.DifferentialFunctionClassHolder.customOpClassForHashAndName(Long, String)"/>
		''' </summary>
		Public Const OP_NAME As String = "While"
		Public Const OP_NUM As Integer = 101

		Public Overrides Function opName() As String
			Return OP_NAME
		End Function

		Public Overrides Function outputVariables() As SDVariable()
			Return MyBase.outputVariables()
		End Function

		Public Overrides Function tensorflowName() As String
			Return "While"
		End Function

		Public Overrides Function opType() As Type
			Return Type.LOGIC
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			isConstant = attributesForNode("is_constant").getB()
		End Sub

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 1
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
			Return inputDataTypes
		End Function
	End Class

End Namespace