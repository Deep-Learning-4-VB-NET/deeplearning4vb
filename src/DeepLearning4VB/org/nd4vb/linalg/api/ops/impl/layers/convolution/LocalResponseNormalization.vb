Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LocalResponseNormalizationConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>The custom op <c>lrn</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.LocalResponseNormalization</c>).</summary>
    Public Class LocalResponseNormalization
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend config As LocalResponseNormalizationConfig

#If SAMEDIFF Then
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
#End If

        Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal config As LocalResponseNormalizationConfig)
            MyBase.New(New INDArray(){input}, wrapOrNull(output))

            Me.config = config
            addArgs()
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal LocalResponseNormalizationConfig As LocalResponseNormalizationConfig)
            MyBase.New(New INDArray(){input}, Nothing)

            Me.config = config
            addArgs()
        End Sub

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            If config IsNot Nothing Then
                Return config.toProperties()
            End If
            Return New Dictionary(Of String, Object)()
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

#If MODEL_IMPORT Then
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
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim gradFnInputs() As SDVariable = {arg(), f1(0)}
			Dim lrnGrad As LocalResponseNormalizationDerivative = LocalResponseNormalizationDerivative.derivativeBuilder().inPlace(inPlace).sameDiff(sameDiff).inputFunctions(gradFnInputs).config(config).build()
			Return Collections.singletonList(lrnGrad.outputVariable())
		End Function
#End If

        Public Overrides Function onnxName() As String
            Return "LRN"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "LRN"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes(0).isFPType(), "Input 0 should be a floating point type for %s, got %s", Me.GetType(), inputDataTypes(0))
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>config</c>.</summary>
        Public Overridable Function getConfig() As LocalResponseNormalizationConfig
            Return config
        End Function

    End Class

End Namespace
