Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports org.nd4j.imports.descriptors.properties.adapters
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports DeConv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class DeConv2DTF extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DeConv2DTF
		Inherits DynamicCustomOp

		Protected Friend config As DeConv2DConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") public DeConv2DTF(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputs, org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal config As DeConv2DConfig)
			MyBase.New(sameDiff, inputs)

			Me.config = config
			addArgs()
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As DeConv2DConfig)
			MyBase.New(inputs, outputs)

			Me.config = config
			addArgs()
		End Sub

		Public Overrides Function iArgs() As Long()
			If iArguments.Count = 0 Then
				addArgs()
			End If

			Return MyBase.iArgs()
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config Is Nothing AndAlso iArguments.Count > 0 Then
				config = DeConv2DConfig.builder().kH(iArguments(0)).kW(iArguments(1)).sH(iArguments(2)).sW(iArguments(3)).pH(iArguments(4)).pW(iArguments(5)).dH(iArguments(6)).dW(iArguments(7)).isSameMode(iArguments(8) = 1).dataFormat(If(iArguments(9) = 1, DeConv2DConfig.NHWC, Conv2DConfig.NCHW)).build()
			End If
			Return config.toProperties()
		End Function

		Private Sub addArgs()
			addIArgument(config.getKH())
			addIArgument(config.getKW())
			addIArgument(config.getSH())
			addIArgument(config.getSW())
			addIArgument(config.getPH())
			addIArgument(config.getPW())
			addIArgument(config.getDH())
			addIArgument(config.getDW())
			addIArgument(ArrayUtil.fromBoolean(config.isSameMode()))
			addIArgument(If(config.getDataFormat().equalsIgnoreCase(DeConv2DConfig.NCHW), 0, 1))
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function


		Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			If config Is Nothing Then
				config = DeConv2DConfig.builder().build()
			End If

			Return config.getValue([property])
		End Function


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim strideMapping As val = PropertyMapping.builder().tfAttrName("strides").onnxAttrName("strides").propertyNames(New String(){"sH", "sW"}).build()

			Dim kernelMapping As val = PropertyMapping.builder().propertyNames(New String(){"kH", "kW"}).tfInputPosition(1).onnxAttrName("kernel_shape").build()

			Dim dilationMapping As val = PropertyMapping.builder().onnxAttrName("dilations").propertyNames(New String(){"dW", "dH"}).tfAttrName("rates").build()

			Dim sameMode As val = PropertyMapping.builder().onnxAttrName("auto_pad").propertyNames(New String(){"isSameMode"}).tfAttrName("padding").build()

			Dim dataFormat As val = PropertyMapping.builder().onnxAttrName("data_format").tfAttrName("data_format").propertyNames(New String(){"dataFormat"}).build()


			map("sW") = strideMapping
			map("sH") = strideMapping
			map("kH") = kernelMapping
			map("kW") = kernelMapping
			map("dW") = dilationMapping
			map("dH") = dilationMapping
			map("isSameMode") = sameMode
			map("dataFormat") = dataFormat

			ret(tensorflowName()) = map
			Return ret
		End Function

		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New Dictionary(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfMappings As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()
			Dim fields As val = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)


			'TF uses [kH, kW, outC, inC] always for weights
			tfMappings("kH") = New NDArrayShapeAdapter(0)
			tfMappings("kW") = New NDArrayShapeAdapter(1)
	'        tfMappings.put("sH", new IntArrayIntIndexAdpater(1));
	'        tfMappings.put("sW", new IntArrayIntIndexAdpater(2));
			tfMappings("sH") = New ConditionalFieldValueIntIndexArrayAdapter("NCHW", 2, 1, fields.get("dataFormat"))
			tfMappings("sW") = New ConditionalFieldValueIntIndexArrayAdapter("NCHW", 3, 2, fields.get("dataFormat"))
			tfMappings("isSameMode") = New StringEqualsAdapter("SAME")
			tfMappings("isNHWC") = New StringEqualsAdapter("NHWC")


			Dim onnxMappings As IDictionary(Of String, AttributeAdapter) = New Dictionary(Of String, AttributeAdapter)()
			onnxMappings("kH") = New SizeThresholdIntArrayIntIndexAdapter(0, 2, 0)
			onnxMappings("kW") = New SizeThresholdIntArrayIntIndexAdapter(1, 2, 0)
			onnxMappings("dH") = New SizeThresholdIntArrayIntIndexAdapter(0, 2, 0)
			onnxMappings("dW") = New SizeThresholdIntArrayIntIndexAdapter(1, 2, 0)
			onnxMappings("sH") = New SizeThresholdIntArrayIntIndexAdapter(0, 2, 0)
			onnxMappings("sW") = New SizeThresholdIntArrayIntIndexAdapter(1, 2, 0)
			onnxMappings("isSameMode") = New StringEqualsAdapter("SAME")
			onnxMappings("isNHWC") = New StringEqualsAdapter("NHWC")

			ret(tensorflowName()) = tfMappings
			Return ret
		End Function


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Function opName() As String
			Return "deconv2d_tf"
		End Function

		Public Overrides Function onnxName() As String
			Return "ConvTranspose-Absent"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Conv2DBackpropInput"
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("To be implemented yet")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType) 'inShape, weights, input
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			If dArguments.Count > 0 Then
				Return New List(Of DataType) From {dArguments(0)}
			End If
			Return Collections.singletonList(inputDataTypes(2))
		End Function
	End Class

End Namespace