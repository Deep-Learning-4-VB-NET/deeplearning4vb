Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
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

Namespace org.nd4vb.linalg.api.ops.impl.shape.tensorops

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.shape.tensorops.TensorArraySize</c>.</summary>
    Public Class TensorArraySize
        Inherits BaseTensorOp

       Public Overrides Function tensorflowNames() As String()
          Return New String(){"TensorArraySize", "TensorArraySizeV2", "TensorArraySizeV3"}
       End Function

       Public Overrides Function opName() As String
          Return "size_list"
       End Function

#If MODEL_IMPORT Then
	   Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
		  MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
	   End Sub

	   Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
		  Return MyBase.mappingsForFunction()
	   End Function

	   Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
	   End Sub
#End If

       Public Overrides Function calculateOutputDataTypes(ByVal inputDataType As IList(Of DataType)) As IList(Of DataType)
          'Size is always int32
          Return New List(Of DataType) From {DataType.INT}
       End Function

    End Class

End Namespace
