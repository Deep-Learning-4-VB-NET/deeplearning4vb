Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
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

    ''' <summary>The custom op <c>in_top_k</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.InTopK</c>).</summary>
    Public Class InTopK
        Inherits DynamicCustomOp

        Private sorted As Boolean
        Private k As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal predictions As SDVariable, ByVal targets As SDVariable, ByVal k As Integer)
			MyBase.New(sd, New SDVariable(){predictions, targets}, False)
			Me.k = k
			addIArgument(k)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "in_top_k"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "InTopKV2"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

			Dim thisName As String = nodeDef.getName()
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
			Preconditions.checkState(kNode IsNot Nothing, "Could not find 'k' parameter node for op: %s", thisName)

			Dim arr As INDArray = TFGraphMapper.getNDArrayFromTensor(kNode)
			Me.k = arr.getInt(0)
			addIArgument(k)
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not implemented yet")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            '3rd input: dynamic K value
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count > 0, "Expected at  least 1 input data types. for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {DataType.BOOL}
        End Function

    End Class

End Namespace
