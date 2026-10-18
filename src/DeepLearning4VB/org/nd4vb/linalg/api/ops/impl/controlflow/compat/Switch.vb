Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.controlflow.compat.Switch</c>.</summary>
    Public Class Switch
        Inherits BaseCompatOp

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.autodiff.samediff.SDVariable predicate;
		Private predicate As SDVariable

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal predicate As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){input, predicate})
			Me.predicate = predicate
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal predicate As INDArray)
            MyBase.New()
            addInputArgument(input, predicate)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' WARNING: do not change without changing serialization methods
        ''' See <seealso cref="org.nd4j.autodiff.samediff.serde.FlatBuffersMapper.getOpNum(String, Type)"/>
        '''  and <seealso cref="org.nd4j.imports.converters.DifferentialFunctionClassHolder.customOpClassForHashAndName(Long, String)"/>
        ''' </summary>
        Public Const OP_NAME As String = "switch"
        Public Const OP_NUM As Integer = 30

        Public Overrides Function opName() As String
            Return OP_NAME
        End Function

#If SAMEDIFF Then
		Public Overrides Function outputVariables() As SDVariable()
			Return MyBase.outputVariables()
		End Function
#End If

        Public Overrides Function tensorflowName() As String
            Return "Switch"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.LOGIC
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
		End Sub
#End If

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return 2 '2 outputs - 2 branches
            End Get
        End Property

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected 2 input dataypes for %s, got %s", Me.GetType(), inputDataTypes)
            Preconditions.checkState(inputDataTypes(1) = DataType.BOOL, "Input datatype 1 (predicate) should be bool for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(0)}
        End Function

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>predicate</c>.</summary>
        Public Overridable Function getPredicate() As SDVariable
            Return predicate
        End Function
#End If

    End Class

End Namespace
