Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports DynamicPartitionBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.DynamicPartitionBp
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom



	Public Class DynamicPartition
		Inherits DynamicCustomOp

		Private numPartitions As Integer
		Private partitions As SDVariable

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal partitions() As SDVariable, ByVal numPartitions As Integer)
			Me.New(sameDiff, input, partitions(0), numPartitions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal partitions As SDVariable, ByVal numPartitions As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, partitions}, False)

			Me.partitions = partitions
			Me.numPartitions = numPartitions
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DynamicPartition(@NonNull INDArray input, @NonNull INDArray partitions, int numPartitions)
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

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New DynamicPartitionBp(sameDiff, arg(0), arg(1), CType(i_v, List(Of SDVariable)).ToArray(), numPartitions)).outputs()
		End Function

		Protected Friend Overridable Sub addArgs()
			addIArgument(numPartitions)
		End Sub

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