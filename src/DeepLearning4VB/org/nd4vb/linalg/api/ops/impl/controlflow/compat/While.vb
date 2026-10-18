Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.controlflow.compat

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.controlflow.compat.While</c>.</summary>
    Public Class [While]
        Inherits BaseCompatOp

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Friend isConstantField As Boolean

#If SAMEDIFF Then
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
#End If

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

#If SAMEDIFF Then
		Public Overrides Function outputVariables() As SDVariable()
			Return MyBase.outputVariables()
		End Function
#End If

        Public Overrides Function tensorflowName() As String
            Return "While"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.LOGIC
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			isConstant = attributesForNode("is_constant").getB()
		End Sub
#End If

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return 1
            End Get
        End Property

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
            Return inputDataTypes
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>isConstant</c>.</summary>
        Public Overridable Function isConstant() As Boolean
            Return isConstantField
        End Function

        ''' <summary>Lombok setter of <c>isConstant</c>.</summary>
        Public Overridable Sub setConstant(ByVal value As Boolean)
            Me.isConstantField = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As [While] = TryCast(o, [While])
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(isConstantField, other.isConstantField) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(isConstantField) Is Nothing, 43, isConstantField.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "While(isConstant=" & Arrays.FormatElement(isConstantField) & ")"
        End Function

    End Class

End Namespace
