Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports DataFormat = org.deeplearning4j.nn.conf.DataFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Convolution3DLayer = org.deeplearning4j.nn.layers.convolution.Convolution3DLayer
Imports Convolution3DParamInitializer = org.deeplearning4j.nn.params.Convolution3DParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Convolution3DUtils = org.deeplearning4j.util.Convolution3DUtils
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Convolution3D extends ConvolutionLayer
	<Serializable>
	Public Class Convolution3D
		Inherits ConvolutionLayer

		''' <summary>
		''' An optional dataFormat: "NDHWC" or "NCDHW". Defaults to "NCDHW".<br> The data format of the input and output
		''' data. <br> For "NCDHW" (also known as 'channels first' format), the data storage order is: [batchSize,
		''' inputChannels, inputDepth, inputHeight, inputWidth].<br> For "NDHWC" ('channels last' format), the data is stored
		''' in the order of: [batchSize, inputDepth, inputHeight, inputWidth, inputChannels].
		''' </summary>
		Public Enum DataFormat
			NCDHW
			NDHWC
		End Enum

		Private mode As ConvolutionMode = ConvolutionMode.Same ' in libnd4j: 0 - same mode, 1 - valid mode
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.nn.conf.DataFormat dataFormat = org.deeplearning4j.nn.conf.DataFormat.NCDHW;
		Private dataFormat As DataFormat = DataFormat.NCDHW ' in libnd4j: 1 - NCDHW, 0 - NDHWC

		''' <summary>
		''' 3-dimensional convolutional layer configuration nIn in the input layer is the number of channels nOut is the
		''' number of filters to be used in the net or in other words the depth The builder specifies the filter/kernel size,
		''' the stride and padding The pooling layer takes the kernel size
		''' </summary>
		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.dataFormat = builder.dataFormat_Conflict
			Me.convolutionMode = builder.convolutionMode_Conflict
		End Sub

		Public Overrides Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function


		Public Overrides Function clone() As Convolution3D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Convolution3D = CType(MyBase.clone(), Convolution3D)
			If clone_Conflict.kernelSize IsNot Nothing Then
				clone_Conflict.kernelSize = CType(clone_Conflict.kernelSize.Clone(), Integer())
			End If
			If clone_Conflict.stride IsNot Nothing Then
				clone_Conflict.stride = CType(clone_Conflict.stride.Clone(), Integer())
			End If
			If clone_Conflict.padding IsNot Nothing Then
				clone_Conflict.padding = CType(clone_Conflict.padding.Clone(), Integer())
			End If
			If clone_Conflict.dilation IsNot Nothing Then
				clone_Conflict.dilation = CType(clone_Conflict.dilation.Clone(), Integer())
			End If
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal iterationListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("Convolution3D", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New Convolution3DLayer(conf, networkDataType)
			ret.setListeners(iterationListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return Convolution3DParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for Convolution3D layer (layer name=""" & getLayerName() & """): Expected CNN3D input, got " & inputType)
			End If
			Return InputTypeUtil.getOutputTypeCnn3DLayers(inputType, dataFormat, kernelSize, stride, padding, dilation, convolutionMode, nOut, layerIndex, getLayerName(), GetType(Convolution3DLayer))
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Convolution3D layer (layer name=""" & getLayerName() & """): input is null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, getLayerName())
		End Function


		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for Convolution 3D layer (layer name=""" & getLayerName() & """): Expected CNN3D input, got " & inputType)
			End If

			If nIn <= 0 OrElse override Then
				Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
				Me.nIn = c.getChannels()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Getter @Setter public static class Builder extends BaseConvBuilder<Builder>
		Public Class Builder
			Inherits BaseConvBuilder(Of Builder)

			''' <summary>
			''' The data format for input and output activations.<br> NCDHW: activations (in/out) should have shape
			''' [minibatch, channels, depth, height, width]<br> NDHWC: activations (in/out) should have shape [minibatch,
			''' depth, height, width, channels]<br>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataFormat_Conflict As DataFormat = DataFormat.NCDHW

			Public Sub New()
				MyBase.New(New Integer() {2, 2, 2}, New Integer() {1, 1, 1}, New Integer() {0, 0, 0}, New Integer() {1, 1, 1}, 3)
			End Sub

			Protected Friend Overrides Function allowCausal() As Boolean
				'Causal convolution - allowed for 1D only
				Return False
			End Function

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer)
				MyBase.New(kernelSize, stride, padding, dilation, 3)
			End Sub

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				Me.New(kernelSize, stride, padding, New Integer() {1, 1, 1})
			End Sub

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer)
				Me.New(kernelSize, stride, New Integer() {0, 0, 0})
			End Sub

			Public Sub New(ParamArray ByVal kernelSize() As Integer)
				Me.New(kernelSize, New Integer() {1, 1, 1})
			End Sub

			''' <summary>
			''' Set kernel size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="kernelSize"> kernel size </param>
			''' <returns> 3D convolution layer builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function kernelSize(ParamArray ByVal kernelSize_Conflict() As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set stride size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="stride"> kernel size </param>
			''' <returns> 3D convolution layer builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function stride(ParamArray ByVal stride_Conflict() As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set padding size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="padding"> kernel size </param>
			''' <returns> 3D convolution layer builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function padding(ParamArray ByVal padding_Conflict() As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set dilation size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="dilation"> kernel size </param>
			''' <returns> 3D convolution layer builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter dilation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function dilation(ParamArray ByVal dilation_Conflict() As Integer) As Builder
				Me.Dilation = dilation_Conflict
				Return Me
			End Function

			Public Overrides Function convolutionMode(ByVal mode As ConvolutionMode) As Builder
				Me.ConvolutionMode = mode
				Return Me
			End Function

			''' <summary>
			''' The data format for input and output activations.<br> NCDHW: activations (in/out) should have shape
			''' [minibatch, channels, depth, height, width]<br> NDHWC: activations (in/out) should have shape [minibatch,
			''' depth, height, width, channels]<br>
			''' </summary>
			''' <param name="dataFormat"> Data format to use for activations </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As DataFormat) As Builder
				Me.setDataFormat(dataFormat_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set kernel size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="kernelSize"> kernel size </param>
			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize_Conflict = ValidationUtils.validate3NonNegative(kernelSize, "kernelSize")
				End Set
			End Property

			''' <summary>
			''' Set stride size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="stride"> kernel size </param>
			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate3NonNegative(stride, "stride")
				End Set
			End Property

			''' <summary>
			''' Set padding size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="padding"> kernel size </param>
			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate3NonNegative(padding, "padding")
				End Set
			End Property

			''' <summary>
			''' Set dilation size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="dilation"> kernel size </param>
			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate3NonNegative(dilation, "dilation")
				End Set
			End Property



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Convolution3D build()
			Public Overrides Function build() As Convolution3D
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding_Conflict)
				Convolution3DUtils.validateCnn3DKernelStridePadding(kernelSize_Conflict, stride_Conflict, padding_Conflict)

				Return New Convolution3D(Me)
			End Function
		End Class

	End Class

End Namespace