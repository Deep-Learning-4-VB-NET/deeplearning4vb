Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports FlatBuffersMapper = DeepLearning4VB.org.nd4vb.autodiff.samediff.serde.FlatBuffersMapper
Imports DataTypeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.adapters.DataTypeAdapter
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>
    ''' Returns the shape of the input array.
    ''' 
    ''' @author Adam Gibson
    ''' </summary>
    Public Class Shape
        Inherits DynamicCustomOp

        Protected Friend dataTypeField As DataType?

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable)
			Me.New(sameDiff, input, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, inPlace)
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray)
            MyBase.New(Nothing, [in], [out], Nothing, Nothing)
        End Sub

        Public Sub New(ByVal [in] As INDArray)
            Me.New([in], Nothing)
        End Sub

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
        End Function

        Public Overrides Function opName() As String
            Return "shape_of"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Shape"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)

			dataType = TFGraphMapper.convertType(nodeDef.getAttrOrThrow("out_type").getType())
			Dim dtype As val = DataTypeAdapter.dtypeConv(nodeDef.getAttrOrThrow("out_type").getType())
			iArguments.Add(CLng(FlatBuffersMapper.getDataTypeAsByte(dtype)))
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes.Count = 1, "Expected list with exactly 1 datatype for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypeField.GetValueOrDefault(DataType.LONG)}
        End Function

    End Class

End Namespace
