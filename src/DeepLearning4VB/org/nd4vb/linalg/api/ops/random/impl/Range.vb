Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
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

Namespace org.nd4vb.linalg.api.ops.random.impl

    ''' <summary>The custom op <c>range</c> (Java <c>org.nd4j.linalg.api.ops.random.impl.Range</c>).</summary>
    Public Class Range
        Inherits DynamicCustomOp

        Public Const DEFAULT_DTYPE As DataType = DataType.FLOAT

        Private from As Double?
        Private [to] As Double?
        Private delta As Double?
        Private dataTypeField As DataType?

        Public Sub New()
            ' no-op
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal from As Double, ByVal [to] As Double, ByVal [step] As Double, ByVal dataType As DataType)
			MyBase.New(Nothing, sd, New SDVariable(){})
			addTArgument(from, [to], [step])
			addDArgument(dataType)
			Me.from = from
			Me.to = [to]
			Me.delta = [step]
			Me.dataType = dataType
		End Sub
#End If

        Public Sub New(ByVal from As Double, ByVal [to] As Double, ByVal [step] As Double, ByVal dataType As DataType)
            addTArgument(from, [to], [step])
            Me.from = from
            Me.to = [to]
            Me.delta = [step]
            Me.dataTypeField = dataType
            addDArgument(dataType)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal from As SDVariable, ByVal [to] As SDVariable, ByVal [step] As SDVariable, ByVal dataType As DataType)
			MyBase.New(Nothing, sd, New SDVariable(){from, [to], [step]})
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub
#End If

        Public Sub New(ByVal from As INDArray, ByVal [to] As INDArray, ByVal [step] As INDArray, ByVal dataType As DataType)
            MyBase.New(New INDArray(){from, [to], [step]}, Nothing)
            Me.dataTypeField = dataType
            addDArgument(dataType)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 4
        End Function

        Public Overrides Function opName() As String
            Return "range"
        End Function

        Public Overrides Function onnxName() As String
            Return "Range"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Range"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			If attributesForNode.ContainsKey("Tidx") Then
				dataType = TFGraphMapper.convertType(attributesForNode("Tidx").getType())
			End If
			addDArgument(dataType)
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.emptyList()
		End Function
#End If

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes Is Nothing OrElse inputDataTypes.Count = 0 OrElse inputDataTypes.Count = 3, "Expected no input datatypes (no args) or 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {dataTypeField.GetValueOrDefault(DEFAULT_DTYPE)}
        End Function

    End Class

End Namespace
