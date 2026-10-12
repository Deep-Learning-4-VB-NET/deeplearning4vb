Imports System
Imports System.Collections.Generic
Imports lombok
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayerUtils = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayerUtils
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Activation = org.nd4j.linalg.activations.Activation
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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

Namespace org.deeplearning4j.nn.layers.samediff.testlayers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonIgnoreProperties({"paramShapes"}) public class SameDiffConv extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class SameDiffConv
		Inherits SameDiffLayer

		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = Collections.singletonList(ConvolutionParamInitializer.WEIGHT_KEY)
		Private Shared ReadOnly BIAS_KEYS As IList(Of String) = Collections.singletonList(ConvolutionParamInitializer.BIAS_KEY)
		'Order to match 'vanilla' conv layer implementation, for easy comparison
		Private Shared ReadOnly PARAM_KEYS As IList(Of String) = New List(Of String) From {ConvolutionParamInitializer.BIAS_KEY, ConvolutionParamInitializer.WEIGHT_KEY}

		Private nIn As Long
		Private nOut As Long
		Private activation As Activation
		Private kernel() As Integer
		Private stride() As Integer
		Private padding() As Integer
		Private cm As ConvolutionMode
		Private dilation() As Integer
		Private hasBias As Boolean

		Protected Friend Sub New(ByVal b As Builder)
			MyBase.New(b)
			Me.nIn = b.nIn_Conflict
			Me.nOut = b.nOut_Conflict
			Me.activation = b.activation_Conflict
			Me.kernel = b.kernel
			Me.stride = b.stride_Conflict
			Me.padding = b.padding_Conflict
			Me.cm = b.cm
			Me.dilation = b.dilation_Conflict
			Me.hasBias = b.hasBias_Conflict
		End Sub

		Private Sub New()
			'No arg constructor for Jackson/JSON serialization
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Return InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, padding, New Integer(){1, 1}, cm, nOut, layerIndex, getLayerName(), GetType(SameDiffConv))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If nIn <= 0 OrElse override Then
				Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				Me.nIn = c.getChannels()
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, getLayerName())
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()
			Dim weightsShape As val = New Long(){kernel(0), kernel(1), nIn, nOut} '[kH, kW, iC, oC] in libnd4j
			params.addWeightParam(ConvolutionParamInitializer.WEIGHT_KEY, weightsShape)
			If hasBias Then
				Dim biasShape As val = New Long(){1, nOut}
				params.addBiasParam(ConvolutionParamInitializer.BIAS_KEY, biasShape)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				Dim fanIn As Double = nIn * kernel(0) * kernel(1)
				Dim fanOut As Double = nOut * kernel(0) * kernel(1) / (CDbl(stride(0)) * stride(1))
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
					If paramWeightInit IsNot Nothing AndAlso paramWeightInit.ContainsKey(e.Key) Then
						paramWeightInit(e.Key).init(fanIn, fanOut, e.Value.shape(), "c"c, e.Value)
					Else
						If ConvolutionParamInitializer.BIAS_KEY.Equals(e.Key) Then
							e.Value.assign(0)
						Else
							WeightInitUtil.initWeights(fanIn, fanOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						End If
					End If
				Next e
			End Using
		End Sub

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable

			Dim w As SDVariable = paramTable(ConvolutionParamInitializer.WEIGHT_KEY)

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kernel(0)).kW(kernel(1)).pH(padding(0)).pW(padding(1)).sH(stride(0)).sW(stride(1)).dH(dilation(0)).dW(dilation(1)).isSameMode(Me.cm = ConvolutionMode.Same).build()

			Dim conv As SDVariable = Nothing
			If hasBias Then
				Dim b As SDVariable = paramTable(ConvolutionParamInitializer.BIAS_KEY)
				conv = sameDiff.cnn().conv2d(layerInput, w, b, c)
			Else
				conv = sameDiff.cnn().conv2d(layerInput, w, c)
			End If

			Return activation.asSameDiff("out", sameDiff, conv)
		End Function

		Public Overrides Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)
			If activation = Nothing Then
				activation = SameDiffLayerUtils.fromIActivation(globalConfig.getActivationFn())
			End If
			If cm = Nothing Then
				cm = globalConfig.getConvolutionMode()
			End If
		End Sub

		Public Class Builder
			Inherits SameDiffLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field nIn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nIn_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field nOut was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nOut_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field activation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend activation_Conflict As Activation = Activation.TANH
			Friend kernel() As Integer = {2, 2}

'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stride_Conflict() As Integer = {1, 1}
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padding_Conflict() As Integer = {0, 0}
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dilation_Conflict() As Integer = {1, 1}
			Friend cm As ConvolutionMode = ConvolutionMode.Same
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nIn(ByVal nIn_Conflict As Integer) As Builder
				Me.nIn_Conflict = nIn_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Me.nOut_Conflict = nOut_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Me.activation_Conflict = activation_Conflict
				Return Me
			End Function

			Public Overridable Function kernelSize(ParamArray ByVal k() As Integer) As Builder
				Me.kernel = k
				Return Me
			End Function

			Public Overridable Function stride(ParamArray ByVal s() As Integer) As Builder
				Me.stride_Conflict = s
				Return Me
			End Function

			Public Overridable Function padding(ParamArray ByVal p() As Integer) As Builder
				Me.padding_Conflict = p
				Return Me
			End Function

			Public Overridable Function convolutionMode(ByVal cm As ConvolutionMode) As Builder
				Me.cm = cm
				Return Me
			End Function

			Public Overridable Function dilation(ParamArray ByVal d() As Integer) As Builder
				Me.dilation_Conflict = d
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.hasBias_Conflict = hasBias_Conflict
				Return Me
			End Function

			Public Overrides Function build() As SameDiffConv
				Return New SameDiffConv(Me)
			End Function
		End Class
	End Class

End Namespace