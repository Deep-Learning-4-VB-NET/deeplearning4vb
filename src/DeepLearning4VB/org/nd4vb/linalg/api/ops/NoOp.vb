Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports Onnx = onnx.Onnx
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
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

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>The custom op <c>noop</c> (Java <c>org.nd4j.linalg.api.ops.NoOp</c>).</summary>
    Public Class NoOp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable)
			MyBase.New("noop", sd, New SDVariable(){[in]})
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray)
            addInputArgument([in])
        End Sub

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(f1(0))
		End Function
#End If

        Public Overrides Function opName() As String
            Return "noop"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub
#End If

        Public Overrides Function onnxName() As String
            Return "NoOp"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "NoOp"
        End Function

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return 1
            End Get
        End Property

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            If inputArgumentsField IsNot Nothing AndAlso inputArgumentsField.Count > 0 Then
                Return New List(Of LongShapeDescriptor) From {inputArguments(0).shapeDescriptor()}
            End If
            Return New List(Of LongShapeDescriptor) From {Nd4vbT.empty(DataType.BOOL).shapeDescriptor()}
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            If oc.getInputArrays() IsNot Nothing AndAlso oc.getInputArrays().Count > 0 Then
                Return New List(Of LongShapeDescriptor) From {oc.getInputArray(0).shapeDescriptor()}
            End If
            Return New List(Of LongShapeDescriptor) From {Nd4vbT.empty(DataType.BOOL).shapeDescriptor()}
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {DataType.BOOL}
        End Function

    End Class

End Namespace
