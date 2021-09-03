Imports System
Imports System.Collections.Generic
Imports lombok
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayerUtils = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayerUtils
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports SDIndex = org.nd4j.autodiff.samediff.SDIndex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonIgnoreProperties({"paramShapes"}) public class LocallyConnected2D extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class LocallyConnected2D
		Inherits SameDiffLayer

		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = Collections.singletonList(ConvolutionParamInitializer.WEIGHT_KEY)
		Private Shared ReadOnly BIAS_KEYS As IList(Of String) = Collections.singletonList(ConvolutionParamInitializer.BIAS_KEY)
		Private Shared ReadOnly PARAM_KEYS As IList(Of String) = New List(Of String) From {ConvolutionParamInitializer.BIAS_KEY, ConvolutionParamInitializer.WEIGHT_KEY}

		Private nIn As Long
		Private nOut As Long
		Private activation As Activation
		Private kernel() As Integer
		Private stride() As Integer
		Private padding() As Integer
		Private paddingBr() As Integer
		Private cm As ConvolutionMode
		Private dilation() As Integer
		Private hasBias As Boolean
		Private inputSize() As Integer
		Private outputSize() As Integer
		Private featureDim As Integer
		Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.nIn = builder.nIn_Conflict
			Me.nOut = builder.nOut_Conflict
			Me.activation = builder.activation_Conflict
			Me.kernel = builder.kernel_Conflict
			Me.stride = builder.stride_Conflict
			Me.padding = builder.padding_Conflict
			Me.cm = builder.cm
			Me.dilation = builder.dilation_Conflict
			Me.hasBias = builder.hasBias_Conflict
			Me.inputSize = builder.inputSize_Conflict
			Me.featureDim = kernel(0) * kernel(1) * CInt(nIn)
			Me.format = builder.format
		End Sub

		Private Sub New()
			'No arg constructor for Jackson/JSON serialization
		End Sub

		Public Overridable Sub computeOutputSize()
			Dim nIn As Integer = CInt(Math.Truncate(getNIn()))

			If inputSize Is Nothing Then
				Throw New System.ArgumentException("Input size has to be specified for locally connected layers.")
			End If

			Dim nchw As Boolean = format = CNN2DFormat.NCHW

			Dim inputShape() As Integer = If(nchw, New Integer() {1, nIn, inputSize(0), inputSize(1)}, New Integer()){1, inputSize(0), inputSize(1), nIn}
			Dim dummyInputForShapeInference As INDArray = Nd4j.ones(inputShape)

			If cm = ConvolutionMode.Same Then
				Me.outputSize = ConvolutionUtils.getOutputSize(dummyInputForShapeInference, kernel, stride, Nothing, cm, dilation, format)
				Me.padding = ConvolutionUtils.getSameModeTopLeftPadding(outputSize, inputSize, kernel, stride, dilation)
				Me.paddingBr = ConvolutionUtils.getSameModeBottomRightPadding(outputSize, inputSize, kernel, stride, dilation)
			Else
				Me.outputSize = ConvolutionUtils.getOutputSize(dummyInputForShapeInference, kernel, stride, padding, cm, dilation, format)
			End If
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.ArgumentException("Provided input type for locally connected 2D layers has to be " & "of CNN type, got: " & inputType)
			End If
			' dynamically compute input size from input type
			Dim cnnType As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Me.inputSize = New Integer() {CInt(Math.Truncate(cnnType.getHeight())), CInt(Math.Truncate(cnnType.getWidth()))}
			computeOutputSize()

			Return InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, padding, New Integer() {1, 1}, cm, nOut, layerIndex, LayerName, format, GetType(LocallyConnected2D))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If nIn <= 0 OrElse override Then
				Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				Me.nIn = c.getChannels()
				Me.featureDim = kernel(0) * kernel(1) * CInt(nIn)
			End If
			Me.format = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()
			Dim weightsShape As val = New Long() {outputSize(0) * outputSize(1), featureDim, nOut}
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
						Dim fanIn As Double = nIn * kernel(0) * kernel(1)
						Dim fanOut As Double = nOut * kernel(0) * kernel(1) / (CDbl(stride(0)) * stride(1))
						WeightInitUtil.initWeights(fanIn, fanOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					End If
				Next e
			End Using
		End Sub

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable

			Dim w As SDVariable = paramTable(ConvolutionParamInitializer.WEIGHT_KEY)

			Dim inputShape() As Long = layerInput.Shape
			Dim miniBatch As Long = inputShape(0)
			Dim outH As Integer = outputSize(0)
			Dim outW As Integer = outputSize(1)
			Dim sH As Integer = stride(0)
			Dim sW As Integer = stride(1)
			Dim kH As Integer = kernel(0)
			Dim kW As Integer = kernel(1)

			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			If Not nchw Then
				layerInput = layerInput.permute(0,3,1,2) 'NHWC to NCHW
			End If

			If padding(0) > 0 OrElse padding(1) > 0 OrElse (cm = ConvolutionMode.Same AndAlso (paddingBr(0) > 0 OrElse paddingBr(1) > 0)) Then
				'Note: for same mode, bottom/right padding can be 1 more than top/left padding
				'NCHW format
				If cm = ConvolutionMode.Same Then
					layerInput = sameDiff.nn().pad(layerInput, sameDiff.constant(Nd4j.createFromArray(New Integer()(){
						New Integer() {0, 0},
						New Integer() {0, 0},
						New Integer() {padding(0), paddingBr(0)},
						New Integer() {padding(1), paddingBr(1)}
					})), PadMode.CONSTANT, 0.0)
				Else
					layerInput = sameDiff.nn().pad(layerInput, sameDiff.constant(Nd4j.createFromArray(New Integer()(){
						New Integer() {0, 0},
						New Integer() {0, 0},
						New Integer() {padding(0), padding(0)},
						New Integer() {padding(1), padding(1)}
					})), PadMode.CONSTANT, 0.0)
				End If
			End If

			Dim inputArray((outH * outW) - 1) As SDVariable
			For y As Integer = 0 To outH - 1
				For x As Integer = 0 To outW - 1
					Dim slice As SDVariable = layerInput.get(SDIndex.all(), SDIndex.all(), SDIndex.interval(y * sH, y * sH + kH), SDIndex.interval(x * sW, x * sW + kW))
					inputArray(x * outH + y) = sameDiff.reshape(slice, 1, miniBatch, featureDim)
				Next x
			Next y
			Dim concatOutput As SDVariable = sameDiff.concat(0, inputArray) ' (outH * outW, miniBatch, featureDim)

			Dim mmulResult As SDVariable = sameDiff.mmul(concatOutput, w) ' (outH * outW, miniBatch, nOut)

			Dim reshapeResult As SDVariable = sameDiff.reshape(mmulResult, outH, outW, miniBatch, nOut)

			Dim permutedResult As SDVariable = If(nchw, reshapeResult.permute(2, 3, 0, 1), reshapeResult.permute(2, 0, 1, 3)) ' (mb, nOut, outH, outW) or (mb, outH, outW, nOut)

			If hasBias Then
				Dim b As SDVariable = paramTable(ConvolutionParamInitializer.BIAS_KEY)
				Dim biasAddedResult As SDVariable = sameDiff.nn().biasAdd(permutedResult, b, nchw)
				Return activation.asSameDiff("out", sameDiff, biasAddedResult)
			Else
				Return activation.asSameDiff("out", sameDiff, permutedResult)
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
			''' Kernel size for the layer. Must be 2 values (height/width)
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] kernel = new int[] {2, 2};
'JAVA TO VB CONVERTER NOTE: The field kernel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend kernel_Conflict() As Integer = {2, 2}

			''' <summary>
			''' Stride for the layer. Must be 2 values (height/width)
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] stride = new int[] {1, 1};
'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stride_Conflict() As Integer = {1, 1}

			''' <summary>
			''' Padding for the layer. Not used if <seealso cref="ConvolutionMode.Same"/> is set. Must be 2 values (height/width)
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] padding = new int[] {0, 0};
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padding_Conflict() As Integer = {0, 0}

			''' <summary>
			''' Dilation for the layer. Must be 2 values (height/width)
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] dilation = new int[] {1, 1};
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dilation_Conflict() As Integer = {1, 1}

			''' <summary>
			''' Set input filter size (h,w) for this locally connected 2D layer
			''' 
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] inputSize;
'JAVA TO VB CONVERTER NOTE: The field inputSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputSize_Conflict() As Integer

			''' <summary>
			''' Convolution mode for the layer. See <seealso cref="ConvolutionMode"/> for details
			''' </summary>
			Friend cm As ConvolutionMode = ConvolutionMode.Same

			''' <summary>
			''' If true (default is false) the layer will have a bias
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

			Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW


			''' <param name="kernel"> Kernel size for the layer. Must be 2 values (height/width) </param>
			Public Overridable WriteOnly Property Kernel As Integer()
				Set(ByVal kernel() As Integer)
					Me.kernel_Conflict = ValidationUtils.validate2NonNegative(kernel, False, "kernel")
				End Set
			End Property

			''' <param name="stride"> Stride for the layer. Must be 2 values (height/width) </param>
			Public Overridable WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate2NonNegative(stride, False, "stride")
				End Set
			End Property

			''' <param name="padding"> Padding for the layer. Not used if <seealso cref="ConvolutionMode.Same"/> is set. Must be 2 values (height/width) </param>
			Public Overridable WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate2NonNegative(padding, False, "padding")
				End Set
			End Property

			''' <param name="dilation"> Dilation for the layer. Must be 2 values (height/width) </param>
			Public Overridable WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate2NonNegative(dilation, False, "dilation")
				End Set
			End Property

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

			''' <param name="k"> Kernel size for the layer. Must be 2 values (height/width) </param>
			Public Overridable Function kernelSize(ParamArray ByVal k() As Integer) As Builder
				Me.Kernel = k
				Return Me
			End Function

			''' <param name="s"> Stride for the layer. Must be 2 values (height/width) </param>
			Public Overridable Function stride(ParamArray ByVal s() As Integer) As Builder
				Me.Stride = s
				Return Me
			End Function

			''' <param name="p"> Padding for the layer. Not used if <seealso cref="ConvolutionMode.Same"/> is set. Must be 2 values (height/width) </param>
			Public Overridable Function padding(ParamArray ByVal p() As Integer) As Builder
				Me.Padding = p
				Return Me
			End Function

			''' <param name="cm"> Convolution mode for the layer. See <seealso cref="ConvolutionMode"/> for details </param>
			Public Overridable Function convolutionMode(ByVal cm As ConvolutionMode) As Builder
				Me.setCm(cm)
				Return Me
			End Function

			''' <param name="d"> Dilation for the layer. Must be 2 values (height/width) </param>
			Public Overridable Function dilation(ParamArray ByVal d() As Integer) As Builder
				Me.Dilation = d
				Return Me
			End Function

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.format = format
				Return Me
			End Function

			''' <param name="hasBias"> If true (default is false) the layer will have a bias </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.setHasBias(hasBias_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set input filter size (h,w) for this locally connected 2D layer
			''' </summary>
			''' <param name="inputSize"> pair of height and width of the input filters to this layer </param>
			''' <returns> Builder </returns>
			Public Overridable Function setInputSize(ParamArray ByVal inputSize() As Integer) As Builder
				Me.inputSize_Conflict = ValidationUtils.validate2(inputSize, False, "inputSize")
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public LocallyConnected2D build()
			Public Overrides Function build() As LocallyConnected2D
				ConvolutionUtils.validateConvolutionModePadding(cm, padding_Conflict)
				ConvolutionUtils.validateCnnKernelStridePadding(kernel_Conflict, stride_Conflict, padding_Conflict)
				Return New LocallyConnected2D(Me)
			End Function
		End Class
	End Class

End Namespace