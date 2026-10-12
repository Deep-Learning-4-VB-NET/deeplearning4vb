Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports ConditionalFieldValueIntIndexArrayAdapter = org.nd4j.imports.descriptors.properties.adapters.ConditionalFieldValueIntIndexArrayAdapter
Imports NDArrayShapeAdapter = org.nd4j.imports.descriptors.properties.adapters.NDArrayShapeAdapter
Imports SizeThresholdIntArrayIntIndexAdapter = org.nd4j.imports.descriptors.properties.adapters.SizeThresholdIntArrayIntIndexAdapter
Imports StringEqualsAdapter = org.nd4j.imports.descriptors.properties.adapters.StringEqualsAdapter
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
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
'ORIGINAL LINE: @Slf4j @Getter public class DepthwiseConv2D extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DepthwiseConv2D
		Inherits DynamicCustomOp

		Protected Friend config As Conv2DConfig


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DepthwiseConv2D(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, @NonNull Conv2DConfig conv2DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal conv2DConfig As Conv2DConfig)
			Me.New(sameDiff, wrapFilterNull(input, weights, bias), conv2DConfig)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") public DepthwiseConv2D(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal config As Conv2DConfig)
			MyBase.New(sameDiff, inputFunctions)

			Me.config = config
			addArgs()
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As Conv2DConfig)
			MyBase.New(inputs, outputs)

			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DepthwiseConv2D(@NonNull INDArray input, @NonNull INDArray weights, org.nd4j.linalg.api.ndarray.INDArray bias, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull Conv2DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal config As Conv2DConfig)
			Me.New(wrapFilterNull(input, weights, bias), wrapOrNull(output), config)
		End Sub

		Public Sub New(ByVal layerInput As INDArray, ByVal depthWeights As INDArray, ByVal bias As INDArray, ByVal config As Conv2DConfig)
			Me.New(layerInput, depthWeights, bias, Nothing, config)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function iArgs() As Long()
			If iArguments.Count = 0 Then
				addArgs()
			End If

			Return MyBase.iArgs()
		End Function

		Protected Friend Overridable Sub addArgs()
			addIArgument(config.getKH(), config.getKW(), config.getSH(), config.getSW(), config.getPH(), config.getPW(), config.getDH(), config.getDW(), ArrayUtil.fromBoolean(config.isSameMode()),If(config.getDataFormat().equalsIgnoreCase(Conv2DConfig.NCHW), 0, 1))

		End Sub

		Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			If config Is Nothing Then
				config = Conv2DConfig.builder().build()
			End If

			Try
				Dim t As val = config.getValue([property])
				Return t
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config Is Nothing AndAlso iArguments.Count > 0 Then
				config = Conv2DConfig.builder().kH(iArguments(0)).kW(iArguments(1)).sH(iArguments(2)).sW(iArguments(3)).pH(iArguments(4)).pW(iArguments(5)).dH(iArguments(6)).dW(iArguments(7)).isSameMode(iArguments(8) = 1).dataFormat(If(iArguments(9) = 1, Conv2DConfig.NHWC_Conflict, Conv2DConfig.NCHW)).build()
			End If
			Return config.toProperties()
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()

	'        
	'        // we must permute weights once during import
	'        val weightsName = nodeDef.getInput(1);
	'        val variable = initWith.getVariable(weightsName);
	'        val tmp = initWith.getArrForVarName(weightsName);
	'        val array = tmp.permute(3, 2, 0, 1).dup('c');
	'
	'        initWith.associateArrayWithVariable(array, variable);
	'        
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub


		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New Dictionary(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfMappings As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()
			Dim fields As val = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)


			'TF uses [kH, kW, inC, outC] always for weights
			tfMappings("kH") = New NDArrayShapeAdapter(0)
			tfMappings("kW") = New NDArrayShapeAdapter(1)
			tfMappings("sH") = New ConditionalFieldValueIntIndexArrayAdapter("NCHW", 2, 1, fields.get("dataFormat"))
			tfMappings("sW") = New ConditionalFieldValueIntIndexArrayAdapter("NCHW", 3, 2, fields.get("dataFormat"))
			tfMappings("dH") = New ConditionalFieldValueIntIndexArrayAdapter("NCHW", 2, 1, fields.get("dataFormat"))
			tfMappings("dW") = New ConditionalFieldValueIntIndexArrayAdapter("NCHW", 3, 2, fields.get("dataFormat"))
			tfMappings("isSameMode") = New StringEqualsAdapter("SAME")


			Dim onnxMappings As IDictionary(Of String, AttributeAdapter) = New Dictionary(Of String, AttributeAdapter)()
			onnxMappings("kH") = New SizeThresholdIntArrayIntIndexAdapter(0, 2, 0)
			onnxMappings("kW") = New SizeThresholdIntArrayIntIndexAdapter(1, 2, 0)
			onnxMappings("dH") = New SizeThresholdIntArrayIntIndexAdapter(0, 2, 0)
			onnxMappings("dW") = New SizeThresholdIntArrayIntIndexAdapter(1, 2, 0)
			onnxMappings("sH") = New SizeThresholdIntArrayIntIndexAdapter(0, 2, 0)
			onnxMappings("sW") = New SizeThresholdIntArrayIntIndexAdapter(1, 2, 0)
			onnxMappings("isSameMode") = New StringEqualsAdapter("SAME")


			Try
				ret(tensorflowName()) = tfMappings
			Catch e As NoOpNameFoundException
				'
			End Try

			Try
				ret(onnxName()) = onnxMappings
			Catch e As NoOpNameFoundException
				'
			End Try

			Return ret
		End Function

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim strideMapping As val = PropertyMapping.builder().tfAttrName("strides").onnxAttrName("strides").propertyNames(New String(){"sW", "sH"}).build()


			Dim kernelMappingH As val = PropertyMapping.builder().propertyNames(New String(){"kH"}).tfInputPosition(1).shapePosition(0).onnxAttrName("kernel_shape").build()

			Dim kernelMappingW As val = PropertyMapping.builder().propertyNames(New String(){"kW"}).tfInputPosition(1).shapePosition(1).onnxAttrName("kernel_shape").build()

			Dim dilationMapping As val = PropertyMapping.builder().onnxAttrName("dilations").propertyNames(New String(){"dW", "dH"}).tfAttrName("rates").build()

			Dim dataFormat As val = PropertyMapping.builder().onnxAttrName("data_format").tfAttrName("data_format").propertyNames(New String(){"dataFormat"}).build()

			Dim nhwc As val = PropertyMapping.builder().onnxAttrName("data_format").tfAttrName("data_format").propertyNames(New String(){"isNHWC"}).build()

			Dim sameMode As val = PropertyMapping.builder().onnxAttrName("auto_pad").propertyNames(New String(){"isSameMode"}).tfAttrName("padding").build()

			Dim paddingWidthHeight As val = PropertyMapping.builder().onnxAttrName("padding").propertyNames(New String(){"pH", "pW"}).build()


			map("sW") = strideMapping
			map("sH") = strideMapping
			map("kH") = kernelMappingH
			map("kW") = kernelMappingW
			map("dW") = dilationMapping
			map("dH") = dilationMapping
			map("isSameMode") = sameMode
			map("pH") = paddingWidthHeight
			map("pW") = paddingWidthHeight
			map("dataFormat") = dataFormat

			Try
				ret(onnxName()) = map
			Catch e As NoOpNameFoundException
				'ignore
			End Try


			Try
				ret(tensorflowName()) = map
			Catch e As NoOpNameFoundException
				'ignore
			End Try

			Return ret
		End Function


		Public Overrides Function opName() As String
			Return "depthwise_conv2d"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim bias As SDVariable = If(args().Length=2, Nothing, arg(2))
			Return New List(Of SDVariable) From {(New DepthwiseConv2DBp(sameDiff, arg(0), arg(1), bias, f1(0), Me.config)).outputVariables()}

		End Function


		Public Overrides Function onnxName() As String
			Return "depth_conv"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "DepthwiseConv2dNative"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace