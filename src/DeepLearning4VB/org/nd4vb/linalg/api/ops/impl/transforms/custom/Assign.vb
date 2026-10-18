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
Imports Onnx = onnx.Onnx
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>assign</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Assign</c>).</summary>
    Public Class Assign
        Inherits DynamicCustomOp

        Public Sub New()

        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(Nothing,inputs, outputs)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            Me.New(New INDArray(){y, x},New INDArray(){y}) ' TODO: Still check. y cannot be null, must be same shape as x.
        End Sub

        Public Overrides Sub addIArgument(ParamArray ByVal arg() As Integer)
            MyBase.addIArgument(arg)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x, y})
		End Sub
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "assign"
        End Function

        Public Overrides Function onnxName() As String
            Return "GivenTensorFill"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Assign"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO replace with assign backprop op from libnd4j (that handles the broadcast case properly)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(larg()), f1(0)}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
