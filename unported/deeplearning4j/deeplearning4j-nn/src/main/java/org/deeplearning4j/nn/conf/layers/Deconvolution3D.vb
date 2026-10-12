Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Deconvolution2DLayer = org.deeplearning4j.nn.layers.convolution.Deconvolution2DLayer
Imports Deconvolution3DLayer = org.deeplearning4j.nn.layers.convolution.Deconvolution3DLayer
Imports Deconvolution3DParamInitializer = org.deeplearning4j.nn.params.Deconvolution3DParamInitializer
Imports DeconvolutionParamInitializer = org.deeplearning4j.nn.params.DeconvolutionParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Deconvolution3D extends ConvolutionLayer
	<Serializable>
	Public Class Deconvolution3D
		Inherits ConvolutionLayer

		Private dataFormat As Convolution3D.DataFormat = Convolution3D.DataFormat.NCDHW ' in libnd4j: 1 - NCDHW, 0 - NDHWC

		''' <summary>
		''' Deconvolution3D layer nIn in the input layer is the number of channels nOut is the number of filters to be used
		''' in the net or in other words the channels The builder specifies the filter/kernel size, the stride and padding
		''' The pooling layer takes the kernel size
		''' </summary>
		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.dataFormat = builder.dataFormat_Conflict
			initializeConstraints(builder)
		End Sub

		Public Overrides Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overrides Function clone() As Deconvolution3D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Deconvolution3D = CType(MyBase.clone(), Deconvolution3D)
			If clone_Conflict.kernelSize IsNot Nothing Then
				clone_Conflict.kernelSize = CType(clone_Conflict.kernelSize.Clone(), Integer())
			End If
			If clone_Conflict.stride IsNot Nothing Then
				clone_Conflict.stride = CType(clone_Conflict.stride.Clone(), Integer())
			End If
			If clone_Conflict.padding IsNot Nothing Then
				clone_Conflict.padding = CType(clone_Conflict.padding.Clone(), Integer())
			End If
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("Deconvolution3D", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New Deconvolution3DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return Deconvolution3DParamInitializer.Instance
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Deconvolution3D layer (layer name=""" & getLayerName() & """): input is null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, getLayerName())
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for Deconvolution 3D layer (layer name=""" & getLayerName() & """): Expected CNN3D input, got " & inputType)
			End If

			If nIn <= 0 OrElse override Then
				Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
				Me.nIn = c.getChannels()
			End If
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for Deconvolution layer (layer name=""" & getLayerName() & """): Expected CNN input, got " & inputType)
			End If

			Return InputTypeUtil.getOutputTypeDeconv3dLayer(inputType, kernelSize, stride, padding, dilation, convolutionMode, dataFormat, nOut, layerIndex, getLayerName(), GetType(Deconvolution3DLayer))
		End Function

		Public Class Builder
			Inherits BaseConvBuilder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataFormat_Conflict As Convolution3D.DataFormat = Convolution3D.DataFormat.NCDHW ' in libnd4j: 1 - NCDHW, 0 - NDHWC

			Public Sub New()
				MyBase.New(New Integer() {2, 2, 2}, New Integer() {1, 1, 1}, New Integer() {0, 0, 0}, New Integer() {1, 1, 1}, 3)
			End Sub

			Protected Friend Overrides Function allowCausal() As Boolean
				'Causal convolution - allowed for 1D only
				Return False
			End Function

			''' <summary>
			''' Set the convolution mode for the Convolution layer. See <seealso cref="ConvolutionMode"/> for more details
			''' </summary>
			''' <param name="convolutionMode"> Convolution mode for layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter convolutionMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function convolutionMode(ByVal convolutionMode_Conflict As ConvolutionMode) As Builder
				Return MyBase.convolutionMode(convolutionMode_Conflict)
			End Function

			''' <summary>
			''' Size of the convolution rows/columns
			''' </summary>
			''' <param name="kernelSize"> the height and width of the kernel </param>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function kernelSize(ParamArray ByVal kernelSize_Conflict() As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function stride(ParamArray ByVal stride_Conflict() As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function padding(ParamArray ByVal padding_Conflict() As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function

			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize_Conflict = ValidationUtils.validate3NonNegative(kernelSize, "kernelSize")
				End Set
			End Property

			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate3NonNegative(stride, "stride")
				End Set
			End Property

			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate3NonNegative(padding, "padding")
				End Set
			End Property

			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate3NonNegative(dilation, "dilation")
				End Set
			End Property

'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As Convolution3D.DataFormat) As Builder
				Me.dataFormat_Conflict = dataFormat_Conflict
				Return Me
			End Function

			Public Overrides Function build() As Deconvolution3D
				Return New Deconvolution3D(Me)
			End Function
		End Class

	End Class

End Namespace