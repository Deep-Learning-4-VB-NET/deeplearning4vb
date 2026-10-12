Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataFormat = org.nd4j.enums.DataFormat
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution



	Public Class DepthToSpace
		Inherits DynamicCustomOp

		Private dataFormat As DataFormat = DataFormat.NHWC
		Private blockSize As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args As SDVariable, ByVal blockSize As Integer, ByVal dataFormat As DataFormat)
			Me.New(sameDiff, New SDVariable(){args}, blockSize, dataFormat)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal blockSize As Integer, ByVal dataFormat As DataFormat)
			MyBase.New(Nothing, sameDiff, args, False)
			Me.blockSize = blockSize
			Me.dataFormat = dataFormat
			Dim isNHWC As Boolean = dataFormat.Equals(DataFormat.NHWC)
			addIArgument(blockSize,If(isNHWC, 1, 0))
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray, ByVal blockSize As Integer, ByVal dataFormat As DataFormat)
			MyBase.New(Nothing, [in], [out], Nothing, Nothing)
			Me.blockSize = blockSize
			Me.dataFormat = dataFormat
			Dim isNHWC As Boolean = dataFormat.Equals(DataFormat.NHWC)
			addIArgument(blockSize,If(isNHWC, 1, 0))
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal blockSize As Integer, ByVal dataFormat As DataFormat)
			Me.New([in], Nothing, blockSize, dataFormat)
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			' Gradient to DepthToSpace is just SpaceToDepth of same block size and data format.
			Dim gradient As SDVariable = i_v(0)
			Dim ret As SDVariable = (New SpaceToDepth(sameDiff, New SDVariable(){gradient}, blockSize, dataFormat)).outputVariable()
			Return New List(Of SDVariable) From {ret}
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			Dim isNHWC As Boolean = dataFormat.Equals(DataFormat.NHWC)
			addIArgument(blockSize,If(isNHWC, 1, 0))
		End Sub


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim attrs As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()

			Dim blockSize As val = PropertyMapping.builder().tfAttrName("block_size").propertyNames(New String(){"blockSize"}).build()
			attrs("blockSize") = blockSize

			Dim dataFormatMapping As val = PropertyMapping.builder().tfAttrName("data_format").propertyNames(New String(){"dataFormat"}).build()
			attrs("dataFormat") = dataFormatMapping

			ret(tensorflowName()) = attrs
			Return ret
		End Function


		Public Overrides Function opName() As String
			Return "depth_to_space"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"DepthToSpace"}
		End Function

		Public Overrides Function tensorflowName() As String
			Return "DepthToSpace"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace