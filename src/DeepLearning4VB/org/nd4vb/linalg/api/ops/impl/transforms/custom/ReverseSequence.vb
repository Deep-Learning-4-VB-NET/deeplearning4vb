Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>reverse_sequence</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.ReverseSequence</c>).</summary>
    Public Class ReverseSequence
        Inherits DynamicCustomOp

        Friend seqDim As Integer
        Friend batchDim As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal seqLengths As SDVariable, ByVal seqDim As Integer, ByVal batchDim As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v, seqLengths}, False)

			Me.seqDim = seqDim
			Me.batchDim = batchDim
			addArguments()

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal seqLengths As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v, seqLengths}, False)
			Me.seqDim = 1
			Me.batchDim = 0
			addArguments()
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal seq_lengths As INDArray, ByVal seqDim As Integer, ByVal batchDim As Integer)
            MyBase.New(New INDArray(){x, seq_lengths}, Nothing)
            Me.seqDim = seqDim
            Me.batchDim = batchDim
            addArguments()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal seq_lengths As INDArray)
            Me.New(x, seq_lengths, 1, 0)
        End Sub

        Private Sub addArguments()
            addIArgument(seqDim)
            addIArgument(batchDim)
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "reverse_sequence"

        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArguments()
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim attrs As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()
			Dim seqDim As val = PropertyMapping.builder().propertyNames(New String(){"seqDim"}).tfAttrName("seq_dim").build()
			Dim batchDim As val = PropertyMapping.builder().propertyNames(New String(){"batchDim"}).tfAttrName("batch_dim").build()
			attrs("seqDim") = seqDim
			attrs("batchDim") = batchDim
			ret(tensorflowName()) = attrs
			Return ret
		End Function
#End If

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ReverseSequence"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = sameDiff.reverseSequence(f1(0), arg(1), seqDim, batchDim)
			Return New List(Of SDVariable) From {ret, sameDiff.zerosLike(arg(1))}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
