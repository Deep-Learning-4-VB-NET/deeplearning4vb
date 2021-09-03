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
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LocalResponseNormalizationConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig
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
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class LocalResponseNormalization extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class LocalResponseNormalization
		Inherits DynamicCustomOp

		Protected Friend config As LocalResponseNormalizationConfig


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") public LocalResponseNormalization(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, boolean inPlace, org.nd4j.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal inPlace As Boolean, ByVal config As LocalResponseNormalizationConfig)
			MyBase.New(Nothing,sameDiff, inputFunctions, inPlace)

			Me.config = config
			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal config As LocalResponseNormalizationConfig)
			Me.New(sameDiff, New SDVariable(){input}, False, config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LocalResponseNormalization(@NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull LocalResponseNormalizationConfig config)
		Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal config As LocalResponseNormalizationConfig)
			MyBase.New(New INDArray(){input}, wrapOrNull(output))

			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LocalResponseNormalization(@NonNull INDArray input, @NonNull LocalResponseNormalizationConfig LocalResponseNormalizationConfig)
		Public Sub New(ByVal input As INDArray, ByVal LocalResponseNormalizationConfig As LocalResponseNormalizationConfig)
			MyBase.New(New INDArray(){input}, Nothing)

			Me.config = config
			addArgs()
		End Sub


		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config IsNot Nothing Then
				Return config.toProperties()
			End If
			Return java.util.Collections.emptyMap()
		End Function

		Private Sub addArgs()
			addTArgument(config.getBias())
			addTArgument(config.getAlpha())
			addTArgument(config.getBeta())
			addIArgument(config.getDepth())
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function

		Public Overrides Function opName() As String
			Return "lrn"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

			Dim aAlpha As val = nodeDef.getAttrOrThrow("alpha")
			Dim aBeta As val = nodeDef.getAttrOrThrow("beta")
			Dim aBias As val = nodeDef.getAttrOrThrow("bias")
			Dim aDepth As val = nodeDef.getAttrOrThrow("depth_radius")

			Dim alpha As Double = aAlpha.getF()
			Dim beta As Double = aBeta.getF()
			Dim bias As Double = aBias.getF()
			Dim depth As Integer = CInt(Math.Truncate(aDepth.getI()))

			Dim localResponseNormalizationConfig As LocalResponseNormalizationConfig = LocalResponseNormalizationConfig.builder().alpha(alpha).beta(beta).bias(bias).depth(CInt(depth)).build()
			Me.config = localResponseNormalizationConfig
			addArgs()
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Dim aAlpha As val = attributesForNode("alpha")
			Dim aBeta As val = attributesForNode("beta")
			Dim aBias As val = attributesForNode("bias")
			Dim aDepth As val = attributesForNode("size")

			Dim alpha As val = aAlpha.getF()
			Dim beta As val = aBeta.getF()
			Dim bias As val = aBias.getF()
			Dim depth As val = aDepth.getF()

			Dim localResponseNormalizationConfig As LocalResponseNormalizationConfig = LocalResponseNormalizationConfig.builder().alpha(alpha).beta(beta).bias(bias).depth(CInt(depth)).build()
			Me.config = localResponseNormalizationConfig
			addArgs()
		End Sub


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim depthMapping As val = PropertyMapping.builder().tfAttrName("depth_radius").propertyNames(New String(){"depth"}).onnxAttrName("size").build()

			Dim alphaMapping As val = PropertyMapping.builder().tfAttrName("alpha").onnxAttrName("alpha").propertyNames(New String(){"alpha"}).build()

			Dim betaMapping As val = PropertyMapping.builder().tfAttrName("beta").onnxAttrName("beta").propertyNames(New String(){"beta"}).build()

			Dim biasMapping As val = PropertyMapping.builder().tfAttrName("bias").onnxAttrName("bias").propertyNames(New String(){"bias"}).build()




			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			map("depth") = depthMapping
			map("alpha") = alphaMapping
			map("beta") = betaMapping
			map("bias") = biasMapping


			ret(tensorflowName()) = map
			ret(onnxName()) = map
			Return ret
		End Function



		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim gradFnInputs() As SDVariable = {arg(), f1(0)}
			Dim lrnGrad As LocalResponseNormalizationDerivative = LocalResponseNormalizationDerivative.derivativeBuilder().inPlace(inPlace).sameDiff(sameDiff).inputFunctions(gradFnInputs).config(config).build()
			Return Collections.singletonList(lrnGrad.outputVariable())
		End Function

		Public Overrides Function onnxName() As String
			Return "LRN"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "LRN"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes(0).isFPType(), "Input 0 should be a floating point type for %s, got %s", Me.GetType(), inputDataTypes(0))
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace