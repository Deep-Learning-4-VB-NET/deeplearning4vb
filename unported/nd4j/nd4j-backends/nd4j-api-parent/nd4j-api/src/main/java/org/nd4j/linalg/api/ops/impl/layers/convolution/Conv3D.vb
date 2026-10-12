Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports org.nd4j.imports.descriptors.properties.adapters
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Conv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
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
'ORIGINAL LINE: @Slf4j @Getter public class Conv3D extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Conv3D
		Inherits DynamicCustomOp

		Protected Friend config As Conv3DConfig
		Private Const INVALID_CONFIGURATION As String = "Invalid Conv3D configuration : sW = %s pH = %s dW = %s "

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv3D(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, @NonNull Conv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal config As Conv3DConfig)
			Me.New(sameDiff, wrapFilterNull(input, weights, bias), config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") public Conv3D(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal config As Conv3DConfig)
			MyBase.New(sameDiff, inputFunctions)
			initConfig(config)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As Conv3DConfig)
			MyBase.New(inputs, outputs)
			initConfig(config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv3D(@NonNull INDArray input, @NonNull INDArray weights, org.nd4j.linalg.api.ndarray.INDArray bias, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull Conv3DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal config As Conv3DConfig)
			Me.New(wrapFilterNull(input, weights, bias), wrapOrNull(output), config)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal config As Conv3DConfig)
			Me.New(wrapFilterNull(input, weights, bias), Nothing, config)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal config As Conv3DConfig)
			Me.New(wrapFilterNull(input, weights), Nothing, config)
		End Sub

		Private Sub initConfig(ByVal config As Conv3DConfig)
			Me.config = config
			Preconditions.checkState(config.getSW() >= 1 AndAlso config.getPH() >= 0 AndAlso config.getDW() >= 1, INVALID_CONFIGURATION, config.getSW(), config.getPH(), config.getDW())
			addArgs()
		End Sub


		Private Sub addArgs()
			addIArgument(getConfig().getKD(), getConfig().getKH(), getConfig().getKW(), getConfig().getSD(), getConfig().getSH(), getConfig().getSW(), getConfig().getPD(), getConfig().getPH(), getConfig().getPW(), getConfig().getDD(), getConfig().getDH(), getConfig().getDW(),If(getConfig().isSameMode(), 1, 0),If(getConfig().isNCDHW(), 0, 1))
		End Sub


		Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			If config Is Nothing AndAlso iArguments.Count > 0 Then
				config = Conv3DConfig.builder().kD(iArguments(0)).kH(iArguments(1)).kW(iArguments(2)).sD(iArguments(3)).sH(iArguments(4)).sW(iArguments(5)).pD(iArguments(6)).pH(iArguments(7)).pW(iArguments(8)).dD(iArguments(9)).dH(iArguments(10)).dW(iArguments(11)).isSameMode(iArguments(12) = 1).dataFormat(If(iArguments(13) = 1, Conv3DConfig.NCDHW_Conflict, Conv3DConfig.NDHWC)).build()
			End If

			Return config.getValue([property])
		End Function

		Public Overrides Function iArgs() As Long()
			If iArguments.Count = 0 Then
				addArgs()
			End If

			Return MyBase.iArgs()
		End Function

		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New LinkedHashMap(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfAdapters As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()
			Dim fields As val = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)

			'TF uses [kD, kH, kW, iC, oC] for weights
			tfAdapters("kD") = New NDArrayShapeAdapter(0)
			tfAdapters("kH") = New NDArrayShapeAdapter(1)
			tfAdapters("kW") = New NDArrayShapeAdapter(2)

			tfAdapters("sD") = New IntArrayIntIndexAdapter(1)
			tfAdapters("sH") = New IntArrayIntIndexAdapter(2)
			tfAdapters("sW") = New IntArrayIntIndexAdapter(3)

			tfAdapters("pD") = New IntArrayIntIndexAdapter(1)
			tfAdapters("pH") = New IntArrayIntIndexAdapter(2)
			tfAdapters("pW") = New IntArrayIntIndexAdapter(3)


			tfAdapters("isSameMode") = New StringNotEqualsAdapter("VALID")

			ret(tensorflowName()) = tfAdapters

			Return ret
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config Is Nothing Then
				Return java.util.Collections.emptyMap()
			End If
			Return config.toProperties()
		End Function

		Public Overrides Function opName() As String
			Return "conv3dnew"
		End Function


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()


			Dim kernelMapping As val = PropertyMapping.builder().propertyNames(New String(){"kD", "kW", "kH"}).tfInputPosition(1).onnxAttrName("kernel_shape").build()

			Dim strideMapping As val = PropertyMapping.builder().tfAttrName("strides").onnxAttrName("strides").propertyNames(New String(){"sD", "sW", "sH"}).build()

			Dim dilationMapping As val = PropertyMapping.builder().onnxAttrName("dilations").propertyNames(New String(){"dD", "dH", "dW"}).tfAttrName("rates").build()

			Dim sameMode As val = PropertyMapping.builder().onnxAttrName("auto_pad").propertyNames(New String(){"isSameMode"}).tfAttrName("padding").build()

			Dim paddingWidthHeight As val = PropertyMapping.builder().onnxAttrName("padding").propertyNames(New String(){"pD", "pW", "pH"}).build()

			Dim dataFormat As val = PropertyMapping.builder().onnxAttrName("data_format").tfAttrName("data_format").propertyNames(New String(){"dataFormat"}).build()


			Dim outputPadding As val = PropertyMapping.builder().propertyNames(New String(){"aD", "aH", "aW"}).build()


			Dim biasUsed As val = PropertyMapping.builder().propertyNames(New String(){"biasUsed"}).build()


			For Each propertyMapping As val In New PropertyMapping(){ kernelMapping, strideMapping, dilationMapping, sameMode, paddingWidthHeight, dataFormat, outputPadding, biasUsed}
				For Each keys As val In propertyMapping.getPropertyNames()
					map(keys) = propertyMapping
				Next keys
			Next propertyMapping

			ret(tensorflowName()) = map
			Return ret
		End Function


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)()
			CType(inputs, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {args()})
			inputs.Add(f1(0))
			Dim conv3DDerivative As Conv3DDerivative = Conv3DDerivative.derivativeBuilder().conv3DConfig(config).inputFunctions(CType(inputs, List(Of SDVariable)).ToArray()).sameDiff(sameDiff).build()
			CType(ret, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {conv3DDerivative.outputVariables()})
			Return ret
		End Function

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function


		Public Overrides Function onnxName() As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New NoOpNameFoundException("No ONNX op name found for: " & Me.GetType().FullName)
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Conv3D"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace