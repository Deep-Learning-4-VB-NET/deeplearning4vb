Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
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
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class DeConv2D extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DeConv2D
		Inherits DynamicCustomOp

		Protected Friend config As DeConv2DConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv2D(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal config As DeConv2DConfig)
			Me.New(sameDiff, wrapFilterNull(input, weights, bias), config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") public DeConv2D(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputs, org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig config)
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv2D(@NonNull INDArray input, @NonNull INDArray weights, org.nd4j.linalg.api.ndarray.INDArray bias, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull DeConv2DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal config As DeConv2DConfig)
			Me.New(wrapFilterNull(input, weights, bias), wrapOrNull(output), config)
		End Sub

		Public Sub New(ByVal layerInput As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal config As DeConv2DConfig)
			Me.New(layerInput, weights, bias, Nothing, config)
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
			Dim strideMapping As val = PropertyMapping.builder().tfAttrName("strides").onnxAttrName("strides").build()

			Dim kernelMapping As val = PropertyMapping.builder().propertyNames(New String(){"kH", "kW"}).tfInputPosition(1).onnxAttrName("kernel_shape").build()

			Dim dilationMapping As val = PropertyMapping.builder().onnxAttrName("dilations").propertyNames(New String(){"dW", "dH"}).tfAttrName("rates").build()

			Dim sameMode As val = PropertyMapping.builder().onnxAttrName("auto_pad").propertyNames(New String(){"isSameMode"}).tfAttrName("padding").build()

			Dim paddingWidthHeight As val = PropertyMapping.builder().onnxAttrName("padding").propertyNames(New String(){"pH", "pW"}).build()

			map("sW") = strideMapping
			map("sH") = strideMapping
			map("kH") = kernelMapping
			map("kW") = kernelMapping
			map("dW") = dilationMapping
			map("dH") = dilationMapping
			map("isSameMode") = sameMode
			map("pH") = paddingWidthHeight
			map("pW") = paddingWidthHeight

			ret(onnxName()) = map
			ret(tensorflowName()) = map
			Return ret
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim aStrides As val = nodeDef.getAttrOrThrow("strides")
			Dim tfStrides As val = aStrides.getList().getIList()
			Dim sH As Long = 1
			Dim sW As Long = 1
			Dim kH As Long = 1
			Dim kW As Long = 1

			Dim aPadding As val = nodeDef.getAttrOrDefault("padding", Nothing)

			Dim paddingMode As val = aPadding.getS().toStringUtf8()

			Dim args As val = Me.args()
			Dim arr As INDArray = sameDiff.getVariable(args(1).name()).Arr
			If arr Is Nothing Then
				arr = TFGraphMapper.getNDArrayFromTensor(nodeDef)
				' TODO: arguable. it might be easier to permute weights once
				'arr = (arr.permute(3, 2, 0, 1).dup('c'));
				Dim varForOp As val = initWith.getVariable(args(1).name())
				If arr IsNot Nothing Then
					initWith.associateArrayWithVariable(arr, varForOp)
				End If


			End If

			Dim dataFormat As String = "nhwc"
			If nodeDef.containsAttr("data_format") Then
				Dim attr As val = nodeDef.getAttrOrThrow("data_format")
				dataFormat = attr.getS().toStringUtf8().ToLower()
			End If

			If dataFormat.Equals(DeConv2DConfig.NCHW, StringComparison.OrdinalIgnoreCase) Then
				sH = tfStrides.get(2).longValue()
				sW = tfStrides.get(3).longValue()

				kH = arr.size(2)
				kW = arr.size(3)
			Else
				sH = tfStrides.get(1).longValue()
				sW = tfStrides.get(2).longValue()

				kH = arr.size(0)
				kW = arr.size(1)
			End If


			Dim isSameMode As Boolean = paddingMode.equalsIgnoreCase("SAME")
			Dim conv2DConfig As DeConv2DConfig = DeConv2DConfig.builder().kH(kH).kW(kW).sH(sW).sW(sH).isSameMode(isSameMode).dataFormat(If(dataFormat.Equals(DeConv2DConfig.NHWC, StringComparison.OrdinalIgnoreCase), DeConv2DConfig.NHWC, DeConv2DConfig.NCHW)).build()
			Me.config = conv2DConfig

			addArgs()


		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Dim autoPad As val = If(Not attributesForNode.ContainsKey("auto_pad"), "VALID", attributesForNode("auto_pad").getS().toStringUtf8())
			Dim dilations As val = attributesForNode("dilations")
			Dim dilationY As val = If(dilations Is Nothing, 1, dilations.getIntsList().get(0).intValue())
			Dim dilationX As val = If(dilations Is Nothing, 1, dilations.getIntsList().get(1).intValue())
			Dim group As val = attributesForNode("group")

			Dim kernelShape As val = attributesForNode("kernel_shape")
			Dim kH As Integer = kernelShape.getIntsList().get(0).intValue()
			Dim kW As Integer = If(kernelShape.getIntsList().size() < 2, kH, kernelShape.getIntsList().get(1).intValue())

			Dim vertexId As val = args()(0)

			Dim arr As INDArray = vertexId.getArr()
			arr = (arr.permute(3, 2, 0, 1).dup("c"c))
			initWith.associateArrayWithVariable(arr, vertexId)

			Dim dataFormat As String = "nhwc"

			Dim strides As val = attributesForNode("strides")
			Dim sH As val = strides.getIntsList().get(0)
			Dim sW As val = If(strides.getIntsList().size() < 2, sH, strides.getIntsList().get(1))
			Dim isSameMode As Boolean = autoPad.equalsIgnoreCase("SAME")


			Dim conv2DConfig As DeConv2DConfig = DeConv2DConfig.builder().kH(kH).kW(kW).sH(sH.intValue()).sW(sW.intValue()).isSameMode(isSameMode).dataFormat(If(dataFormat.Equals("nhwc", StringComparison.OrdinalIgnoreCase), DeConv2DConfig.NHWC, DeConv2DConfig.NCHW)).build()
			Me.config = conv2DConfig

			addArgs()

			addOutputArgument(arr)
		End Sub


		Public Overrides Function opName() As String
			Return "deconv2d"
		End Function

		Public Overrides Function onnxName() As String
			Return "ConvTranspose"
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)()
			CType(inputs, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {args()})
			CType(inputs, List(Of SDVariable)).AddRange(f1)
			Dim deConv2DDerivative As DeConv2DDerivative = DeConv2DDerivative.derivativeBuilder().sameDiff(sameDiff).config(config).inputs(CType(inputs, List(Of SDVariable)).ToArray()).build()
			CType(ret, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {deConv2DDerivative.outputVariables()})
			Return ret
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace