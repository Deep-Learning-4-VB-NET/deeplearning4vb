Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports DynamicPartitionBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.gradient.DynamicPartitionBp
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

    ''' <summary>The custom op <c>dynamic_partition</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.DynamicPartition</c>).</summary>
    Public Class DynamicPartition
        Inherits DynamicCustomOp

        Private numPartitions As Integer
#If SAMEDIFF Then
		Private partitions As SDVariable
#End If

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal partitions() As SDVariable, ByVal numPartitions As Integer)
			Me.New(sameDiff, input, partitions(0), numPartitions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal partitions As SDVariable, ByVal numPartitions As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, partitions}, False)

			Me.partitions = partitions
			Me.numPartitions = numPartitions
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal partitions As INDArray, ByVal numPartitions As Integer)
            MyBase.New(New INDArray(){input, partitions}, Nothing)
            Me.numPartitions = numPartitions
            addArgs()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal partitions() As INDArray, ByVal numPartitions As Integer)
            'TODO; This needs fixing.
            MyBase.New(New INDArray(){x}, Nothing)
            ' this.partitions = partitions;
            Me.numPartitions = numPartitions
            addArgs()
        End Sub

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New DynamicPartitionBp(sameDiff, arg(0), arg(1), CType(i_v, List(Of SDVariable)).ToArray(), numPartitions)).outputs()
		End Function
#End If

        Protected Friend Overridable Sub addArgs()
            addIArgument(numPartitions)
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim attrs As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()

			Dim numPartitions As val = PropertyMapping.builder().tfAttrName("num_partitions").propertyNames(New String(){"numPartitions"}).build()
			attrs("numPartitions") = numPartitions

			ret(tensorflowName()) = attrs
			Return ret
		End Function
#End If

        Public Overrides Function opName() As String
            Return "dynamic_partition"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "DynamicPartition"
        End Function

        Public Overrides Function onnxName() As String
            Return "Dynamic partitioning currently not supported by ONNX"
        End Function

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return numPartitions
            End Get
        End Property

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Output type: same as (data) input type
            Dim [out] As IList(Of DataType) = New List(Of DataType)(numPartitions)
            For i As Integer = 0 To numPartitions - 1
                [out].Add(dataTypes(0))
            Next i
            Return [out]
        End Function

    End Class

End Namespace
