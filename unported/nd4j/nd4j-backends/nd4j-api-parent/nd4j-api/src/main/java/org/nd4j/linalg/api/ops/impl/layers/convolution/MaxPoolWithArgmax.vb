Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
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
'ORIGINAL LINE: @Slf4j @Getter public class MaxPoolWithArgmax extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class MaxPoolWithArgmax
		Inherits DynamicCustomOp

		Protected Friend config As Pooling2DConfig
		Protected Friend outputType As DataType

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") @SuppressWarnings("Used in lombok") public MaxPoolWithArgmax(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable input, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal config As Pooling2DConfig)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input}, False)

			config.setType(Pooling2D.Pooling2DType.MAX)
			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MaxPoolWithArgmax(@NonNull INDArray input, @NonNull Pooling2DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal config As Pooling2DConfig)
			Me.New(input, Nothing, Nothing, config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MaxPoolWithArgmax(@NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray output,org.nd4j.linalg.api.ndarray.INDArray outArgMax, @NonNull Pooling2DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal outArgMax As INDArray, ByVal config As Pooling2DConfig)
			MyBase.New(Nothing, New INDArray(){input}, wrapFilterNull(output, outArgMax))
			config.setType(Pooling2D.Pooling2DType.MAX)

			Me.config = config
			addArgs()
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function


		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config Is Nothing AndAlso iArguments.Count > 0 Then
				'Perhaps loaded from FlatBuffers - hence we have IArgs but not Config object
				config = Pooling2DConfig.builder().kH(iArguments(0)).kW(iArguments(1)).sH(iArguments(2)).sW(iArguments(3)).pH(iArguments(4)).pW(iArguments(5)).dH(iArguments(6)).dW(iArguments(7)).isSameMode(iArguments(8) = 1).extra(iArguments(9)).isNHWC(iArguments(10) = 1).type(Pooling2D.Pooling2DType.MAX).build()
			End If
			Return config.toProperties()
		End Function

		Private Sub addArgs()
			addIArgument(config.getKH(), config.getKW(), config.getSH(), config.getSW(), config.getPH(), config.getPW(), config.getDH(), config.getDW(), ArrayUtil.fromBoolean(config.isSameMode()), CInt(Math.Truncate(config.getExtra())), ArrayUtil.fromBoolean(config.isNHWC()))

		End Sub


		Public Overridable ReadOnly Property PoolingPrefix As String
			Get
				Return "max"
			End Get
		End Property

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)()
			CType(inputs, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {args()})
			inputs.Add(f1(0))
			Dim pooling2DDerivative As Pooling2DDerivative = Pooling2DDerivative.derivativeBuilder().inputs(CType(inputs, List(Of SDVariable)).ToArray()).sameDiff(sameDiff).config(config).build()
			CType(ret, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {pooling2DDerivative.outputVariables()})
			Return ret
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim aStrides As val = nodeDef.getAttrOrThrow("strides")
			Dim tfStrides As val = aStrides.getList().getIList()

			Dim aKernels As val = nodeDef.getAttrOrThrow("ksize")
			Dim tfKernels As val = aKernels.getList().getIList()

			Dim sH As Integer = 0
			Dim sW As Integer = 0

			Dim pH As Integer = 0
			Dim pW As Integer = 0

			Dim kH As Integer = 0
			Dim kW As Integer = 0

			Dim aPadding As val = nodeDef.getAttrOrThrow("padding")
			Dim padding As val = aPadding.getList().getIList()

			Dim paddingMode As val = aPadding.getS().toStringUtf8().replaceAll("""", "")

			Dim isSameMode As Boolean = paddingMode.equalsIgnoreCase("SAME")

			Dim data_format As String = "nhwc"
			If nodeDef.containsAttr("data_format") Then
				Dim attr As val = nodeDef.getAttrOrThrow("data_format")

				data_format = attr.getS().toStringUtf8().ToLower()
			End If

			If data_format.Equals("nhwc", StringComparison.OrdinalIgnoreCase) Then
				sH = tfStrides.get(1).intValue()
				sW = tfStrides.get(2).intValue()

				kH = tfKernels.get(1).intValue()
				kW = tfKernels.get(2).intValue()

				pH = If(padding.size() > 0, padding.get(1).intValue(), 0)
				pW = If(padding.size() > 0, padding.get(2).intValue(), 0)
			Else
				sH = tfStrides.get(2).intValue()
				sW = tfStrides.get(3).intValue()

				kH = tfKernels.get(2).intValue()
				kW = tfKernels.get(3).intValue()

				pH = If(padding.size() > 0, padding.get(2).intValue(), 0)
				pW = If(padding.size() > 0, padding.get(3).intValue(), 0)
			End If

			Dim pooling2DConfig As Pooling2DConfig = Pooling2DConfig.builder().sH(sH).sW(sW).type(Pooling2D.Pooling2DType.MAX).isSameMode(isSameMode).kH(kH).kW(kW).pH(pH).pW(pW).isNHWC(data_format.Equals("nhwc", StringComparison.OrdinalIgnoreCase)).extra(1.0).build()
			Me.config = pooling2DConfig
			addArgs()
			If attributesForNode.ContainsKey("argmax") Then
				outputType = TFGraphMapper.convertType(attributesForNode("argmax").getType())
			Else
				outputType = DataType.LONG
			End If
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim strideMapping As val = PropertyMapping.builder().tfAttrName("strides").onnxAttrName("strides").propertyNames(New String(){"sW", "sH"}).build()

			Dim paddingMapping As val = PropertyMapping.builder().onnxAttrName("padding").tfAttrName("padding").propertyNames(New String(){"pH", "pW"}).build()

			Dim kernelMapping As val = PropertyMapping.builder().propertyNames(New String(){"kH", "kW"}).tfInputPosition(1).onnxAttrName("ksize").build()

			Dim dilationMapping As val = PropertyMapping.builder().onnxAttrName("dilations").propertyNames(New String(){"dW", "dH"}).tfAttrName("rates").build()


			'data_format
			Dim dataFormatMapping As val = PropertyMapping.builder().propertyNames(New String(){"isNHWC"}).tfAttrName("data_format").build()

			map("sW") = strideMapping
			map("sH") = strideMapping
			map("kH") = kernelMapping
			map("kW") = kernelMapping
			map("dW") = dilationMapping
			map("dH") = dilationMapping
			map("pH") = paddingMapping
			map("pW") = paddingMapping
			map("isNHWC") = dataFormatMapping

			ret(onnxName()) = map
			ret(tensorflowName()) = map
			Return ret
		End Function

		Public Overrides Function opName() As String
			Return "max_pool_with_argmax"
		End Function

		Public Overrides Function onnxName() As String
			Return "MaxPoolWithArgmax"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "MaxPoolWithArgmax"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
			Dim result As IList(Of DataType) = New List(Of DataType)()
			result.Add(inputDataTypes(0))
			If dArguments.Count = 0 Then
				result.Add(If(outputType = Nothing, DataType.INT, outputType))
			Else
				result.Add(dArguments(0))
			End If
			Return result
		End Function
	End Class

End Namespace