Imports System
Imports System.Collections.Generic
Imports lombok
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports SameDiffLayerUtils = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayerUtils
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports Convolution1DUtils = org.deeplearning4j.util.Convolution1DUtils
Imports SDIndex = org.nd4j.autodiff.samediff.SDIndex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PadMode = org.nd4j.enums.PadMode
Imports Activation = org.nd4j.linalg.activations.Activation
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonIgnoreProperties({"paramShapes"}) public class LocallyConnected1D extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class LocallyConnected1D
		Inherits SameDiffLayer

		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = Collections.singletonList(ConvolutionParamInitializer.WEIGHT_KEY)
		Private Shared ReadOnly BIAS_KEYS As IList(Of String) = Collections.singletonList(ConvolutionParamInitializer.BIAS_KEY)
		Private Shared ReadOnly PARAM_KEYS As IList(Of String) = New List(Of String) From {ConvolutionParamInitializer.BIAS_KEY, ConvolutionParamInitializer.WEIGHT_KEY}

		Private nIn As Long
		Private nOut As Long
		Private activation As Activation
		Private kernel As Integer
		Private stride As Integer
		Private padding As Integer
		Private paddingR As Integer 'Right/bottom padding
		Private cm As ConvolutionMode
		Private dilation As Integer
		Private hasBias As Boolean
		Private inputSize As Integer
		Private outputSize As Integer
		Private featureDim As Integer

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.nIn = builder.nIn_Conflict
			Me.nOut = builder.nOut_Conflict
			Me.activation = builder.activation_Conflict
			Me.kernel = builder.kernel
			Me.stride = builder.stride_Conflict
			Me.padding = builder.padding_Conflict
			Me.cm = builder.cm
			Me.dilation = builder.dilation_Conflict
			Me.hasBias = builder.hasBias_Conflict
			Me.inputSize = builder.inputSize_Conflict
			Me.featureDim = kernel * CInt(nIn)
		End Sub

		Private Sub New()
			'No arg constructor for Jackson/JSON serialization
		End Sub

		Public Overridable Sub computeOutputSize()
			Dim nIn As Integer = CInt(Math.Truncate(getNIn()))
			If inputSize = 0 Then
				Throw New System.ArgumentException("Input size has to be set for Locally connected layers")
			End If
			Dim inputShape() As Integer = {1, nIn, inputSize}
			Dim dummyInputForShapeInference As INDArray = Nd4j.ones(inputShape)

			If cm = ConvolutionMode.Same Then
				Me.outputSize = Convolution1DUtils.getOutputSize(dummyInputForShapeInference, kernel, stride, 0, cm, dilation)
				Me.padding = Convolution1DUtils.getSameModeTopLeftPadding(outputSize, inputSize, kernel, stride, dilation)
				Me.paddingR = Convolution1DUtils.getSameModeBottomRightPadding(outputSize, inputSize, kernel, stride, dilation)
			Else
				Me.outputSize = Convolution1DUtils.getOutputSize(dummyInputForShapeInference, kernel, stride, padding, cm, dilation)
			End If
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.ArgumentException("Provided input type for locally connected 1D layers has to be " & "of CNN1D/RNN type, got: " & inputType)
			End If
			' dynamically compute input size from input type
			Dim rnnType As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Me.inputSize = CInt(Math.Truncate(rnnType.getTimeSeriesLength()))
			computeOutputSize()

			Return InputTypeUtil.getOutputTypeCnn1DLayers(inputType, kernel, stride, padding, 1, cm, nOut, layerIndex, LayerName, GetType(LocallyConnected1D))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If nIn <= 0 OrElse override Then
				Dim c As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				Me.nIn = c.getSize()
			End If
			If featureDim <= 0 OrElse override Then
				Dim c As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				Me.featureDim = kernel * CInt(Math.Truncate(c.getSize()))
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, RNNFormat.NCW, LayerName)
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			Preconditions.checkState(featureDim > 0, "Cannot initialize layer: Feature dimension is set to %s", featureDim)
			params.clear()
			Dim weightsShape As val = New Long() {outputSize, featureDim, nOut}
			params.addWeightParam(ConvolutionParamInitializer.WEIGHT_KEY, weightsShape)
			If hasBias Then
				Dim biasShape As val = New Long() {nOut}
				params.addBiasParam(ConvolutionParamInitializer.BIAS_KEY, biasShape)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
					If ConvolutionParamInitializer.BIAS_KEY.Equals(e.Key) Then
						e.Value.assign(0)
					Else
						Dim fanIn As Double = nIn * kernel
						Dim fanOut As Double = nOut * kernel / (CDbl(stride))
						WeightInitUtil.initWeights(fanIn, fanOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					End If
				Next e
			End Using
		End Sub

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
			Dim w As SDVariable = paramTable(ConvolutionParamInitializer.WEIGHT_KEY) ' (outH, featureDim, nOut)

			Dim outH As Integer = outputSize
			Dim sH As Integer = stride
			Dim kH As Integer = kernel

			If padding > 0 OrElse (cm = ConvolutionMode.Same AndAlso paddingR > 0) Then
				'Note: for same mode, bottom/right padding can be 1 more than top/left padding
				'NCW format.
				If cm = ConvolutionMode.Same Then
					layerInput = sameDiff.nn().pad(layerInput, sameDiff.constant(Nd4j.createFromArray(New Integer()(){
						New Integer() {0, 0},
						New Integer() {0, 0},
						New Integer() {padding, paddingR}
					})), PadMode.CONSTANT, 0)
				Else
					layerInput = sameDiff.nn().pad(layerInput, sameDiff.constant(Nd4j.createFromArray(New Integer()(){
						New Integer() {0, 0},
						New Integer() {0, 0},
						New Integer() {padding, padding}
					})), PadMode.CONSTANT, 0)
				End If
			End If

			Dim inputArray(outH - 1) As SDVariable
			For i As Integer = 0 To outH - 1
				Dim slice As SDVariable = layerInput.get(SDIndex.all(), SDIndex.all(), SDIndex.interval(i * sH, i * sH + kH))
				inputArray(i) = sameDiff.reshape(slice, 1, -1, featureDim)
			Next i
			Dim concatOutput As SDVariable = sameDiff.concat(0, inputArray) ' (outH, miniBatch, featureDim)

			Dim mmulResult As SDVariable = sameDiff.mmul(concatOutput, w) ' (outH, miniBatch, nOut)

			Dim result As SDVariable = sameDiff.permute(mmulResult, 1, 2, 0) ' (miniBatch, nOut, outH)

			If hasBias Then
				Dim b As SDVariable = paramTable(ConvolutionParamInitializer.BIAS_KEY)
				Dim biasAddedResult As SDVariable = sameDiff.nn().biasAdd(result, b, True)
				Return activation.asSameDiff("out", sameDiff, biasAddedResult)
			Else
				Return activation.asSameDiff("out", sameDiff, result)
			End If

		End Function

		Public Overrides Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)
			If activation = Nothing Then
				activation = SameDiffLayerUtils.fromIActivation(globalConfig.getActivationFn())
			End If
			If cm = Nothing Then
				cm = globalConfig.getConvolutionMode()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer.Builder<Builder>
		Public Class Builder
			Inherits SameDiffLayer.Builder(Of Builder)

			''' <summary>
			''' Number of inputs to the layer (input size)
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nIn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nIn_Conflict As Integer

			''' <summary>
			''' Number of outputs (output size)
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nOut was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nOut_Conflict As Integer

			''' <summary>
			''' Activation function for the layer
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field activation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend activation_Conflict As Activation = Activation.TANH

			''' <summary>
			''' Kernel size for the layer
			''' </summary>
			Friend kernel As Integer = 2

			''' <summary>
			''' Stride for the layer
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stride_Conflict As Integer = 1

			''' <summary>
			''' Padding for the layer. Not used if <seealso cref="ConvolutionMode.Same"/> is set
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padding_Conflict As Integer = 0

			''' <summary>
			''' Dilation for the layer
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dilation_Conflict As Integer = 1

			''' <summary>
			''' Input filter size for this locally connected 1D layer
			''' 
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int inputSize;
'JAVA TO VB CONVERTER NOTE: The field inputSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputSize_Conflict As Integer

			''' <summary>
			''' Convolution mode for the layer. See <seealso cref="ConvolutionMode"/> for details
			''' </summary>
			Friend cm As ConvolutionMode = ConvolutionMode.Same

			''' <summary>
			''' If true (default is false) the layer will have a bias
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

			''' <param name="nIn"> Number of inputs to the layer (input size) </param>
'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nIn(ByVal nIn_Conflict As Integer) As Builder
				Me.setNIn(nIn_Conflict)
				Return Me
			End Function

			''' <param name="nOut"> Number of outputs (output size) </param>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Me.setNOut(nOut_Conflict)
				Return Me
			End Function

			''' <param name="activation"> Activation function for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Me.setActivation(activation_Conflict)
				Return Me
			End Function

			''' <param name="k"> Kernel size for the layer </param>
			Public Overridable Function kernelSize(ByVal k As Integer) As Builder
				Me.setKernel(k)
				Return Me
			End Function

			''' <param name="s"> Stride for the layer </param>
			Public Overridable Function stride(ByVal s As Integer) As Builder
				Me.setStride(s)
				Return Me
			End Function

			''' <param name="p"> Padding for the layer. Not used if <seealso cref="ConvolutionMode.Same"/> is set </param>
			Public Overridable Function padding(ByVal p As Integer) As Builder
				Me.setPadding(p)
				Return Me
			End Function

			''' <param name="cm"> Convolution mode for the layer. See <seealso cref="ConvolutionMode"/> for details </param>
			Public Overridable Function convolutionMode(ByVal cm As ConvolutionMode) As Builder
				Me.setCm(cm)
				Return Me
			End Function

			''' <param name="d"> Dilation for the layer </param>
			Public Overridable Function dilation(ByVal d As Integer) As Builder
				Me.setDilation(d)
				Return Me
			End Function

			''' <param name="hasBias"> If true (default is false) the layer will have a bias </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.setHasBias(hasBias_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set input filter size for this locally connected 1D layer
			''' </summary>
			''' <param name="inputSize"> height of the input filters </param>
			''' <returns> Builder </returns>
			Public Overridable Function setInputSize(ByVal inputSize As Integer) As Builder
				Me.inputSize_Conflict = inputSize
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public LocallyConnected1D build()
			Public Overrides Function build() As LocallyConnected1D
				Convolution1DUtils.validateConvolutionModePadding(cm, padding_Conflict)
				Convolution1DUtils.validateCnn1DKernelStridePadding(kernel, stride_Conflict, padding_Conflict)
				Return New LocallyConnected1D(Me)
			End Function
		End Class
	End Class

End Namespace