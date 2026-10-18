Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>
    ''' SplitV op
    ''' </summary>
    Public Class SplitV
        Inherits DynamicCustomOp

        Private numSplit As Integer
        Private splitDim As Integer

        Public Overrides Function opName() As String
            Return "split_v"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "SplitV"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim splitDim As val = TFGraphMapper.getArrayFrom(TFGraphMapper.getNodeWithNameFromGraph(graph,nodeDef.getInput(0)),graph)
			If splitDim IsNot Nothing Then
				Me.splitDim = splitDim.getInt(0)
				addIArgument(splitDim.getInt(0))
			End If

			Dim numSplits As val = CInt(Math.Truncate(attributesForNode("num_split").getI()))
			Me.numSplit = numSplits
			'addIArgument(numSplits);  //libnd4j op doesn't used/need it for execution
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim splitDim As val = PropertyMapping.builder().tfInputPosition(-1).propertyNames(New String(){"splitDim"}).build()

			Dim numSplit As val = PropertyMapping.builder().tfAttrName("num_split").propertyNames(New String(){"numSplit"}).build()

			map("numSplit") = numSplit
			map("splitDim") = splitDim

			ret(tensorflowName()) = map
			'ret.put(onnxName(),map);

			Return ret
		End Function
#End If

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return numSplit
            End Get
        End Property

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Output types are same as first input type - just numSplits of them...
            Dim [out] As IList(Of DataType) = New List(Of DataType)(numSplit)
            For i As Integer = 0 To numSplit - 1
                [out].Add(dataTypes(0))
            Next i
            Return [out]
        End Function

    End Class

End Namespace
