Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

    ''' <summary>The custom op <c>top_k</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.TopK</c>).</summary>
    Public Class TopK
        Inherits DynamicCustomOp

        Private sorted As Boolean
        Private k As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal k As Integer, ByVal sorted As Boolean)
			MyBase.New(sd, New SDVariable(){[in]}, False)
			Me.k = k
			Me.sorted = sorted
			addIArgument(ArrayUtil.fromBoolean(sorted), k)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "top_k"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "TopKV2"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

			Dim thisName As String = nodeDef.getName()

			' FIXME: ????
			Dim inputName As String = thisName & "/k"
			Dim kNode As NodeDef = Nothing
			Dim i As Integer = 0
			Do While i < graph.getNodeCount()
				If graph.getNode(i).getName().Equals(inputName) Then
					kNode = graph.getNode(i)
					Exit Do
				End If
				i += 1
			Loop

			Me.sorted = nodeDef.getAttrOrThrow("sorted").getB()

			If kNode IsNot Nothing Then
				Preconditions.checkState(kNode IsNot Nothing, "Could not find 'k' parameter node for op: %s", thisName)

				Dim arr As INDArray = TFGraphMapper.getNDArrayFromTensor(kNode)
				Me.k = arr.getInt(0)

				addIArgument(ArrayUtil.fromBoolean(sorted), k)
			Else
				addIArgument(ArrayUtil.fromBoolean(sorted))
			End If
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not implemented yet")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            '2 outputs: values and indices
            'TODO make thit configurable
            Return New List(Of DataType) From {dataTypes(0), DataType.INT}
        End Function

    End Class

End Namespace
