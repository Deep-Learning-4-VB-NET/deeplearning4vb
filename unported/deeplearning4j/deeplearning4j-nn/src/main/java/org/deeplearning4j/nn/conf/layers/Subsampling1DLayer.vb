Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Convolution1DUtils = org.deeplearning4j.util.Convolution1DUtils
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Subsampling1DLayer extends SubsamplingLayer
	<Serializable>
	Public Class Subsampling1DLayer
		Inherits SubsamplingLayer

	'    
	'     * Currently, we just subclass off the SubsamplingLayer and hard code the "width" dimension to 1.
	'     * TODO: We will eventually want to NOT subclass off of SubsamplingLayer.
	'     * This approach treats a multivariate time series with L timesteps and
	'     * P variables as an L x 1 x P image (L rows high, 1 column wide, P
	'     * channels deep). The kernel should be H<L pixels high and W=1 pixels
	'     * wide.
	'     

'JAVA TO VB CONVERTER NOTE: The parameter builder was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Sub New(ByVal builder_Conflict As Builder)
			MyBase.New(builder_Conflict)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.subsampling.Subsampling1DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling1D layer (layer name=""" & LayerName & """): Expected RNN input, got " & inputType)
			End If
			Dim r As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim inputTsLength As Long = r.getTimeSeriesLength()
			Dim outLength As Long
			If inputTsLength < 0 Then
				'Probably: user did InputType.recurrent(x) without specifying sequence length
				outLength = -1
			Else
				outLength = Convolution1DUtils.getOutputSize(inputTsLength, kernelSize(0), stride(0), padding(0), convolutionMode, dilation(0))
			End If
			Return InputType.recurrent(r.getSize(), outLength, r.getFormat())
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op: subsampling layer doesn't have nIn value
			If cnn2dDataFormat = Nothing OrElse override Then
				If inputType.getType() = InputType.Type.RNN Then
					Dim inputTypeConvolutional As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
					Me.cnn2dDataFormat = If(inputTypeConvolutional.getFormat() = RNNFormat.NCW, CNN2DFormat.NCHW, CNN2DFormat.NHWC)

				ElseIf inputType.getType() = InputType.Type.CNN Then
					Dim inputTypeConvolutional As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
					Me.cnn2dDataFormat = inputTypeConvolutional.getFormat()
				End If

			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling1D layer (layer name=""" & LayerName & """): input is null")
			End If

			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, RNNFormat.NCW, LayerName)
		End Function

		Public Overrides Function clone() As Subsampling1DLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Subsampling1DLayer = CType(MyBase.clone(), Subsampling1DLayer)

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

		Public Class Builder
			Inherits BaseSubsamplingBuilder(Of Builder)

			Friend Const DEFAULT_POOLING As org.deeplearning4j.nn.conf.layers.PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType.MAX
			Friend Const DEFAULT_KERNEL As Integer = 2
			Friend Const DEFAULT_STRIDE As Integer = 1
			Friend Const DEFAULT_PADDING As Integer = 0

			Public Sub New(ByVal poolingType As PoolingType, ByVal kernelSize As Integer, ByVal stride As Integer)
				Me.New(poolingType, kernelSize, stride, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal poolingType As PoolingType, ByVal kernelSize As Integer)
				Me.New(poolingType, kernelSize, DEFAULT_STRIDE, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType, ByVal kernelSize As Integer)
				Me.New(poolingType, kernelSize, DEFAULT_STRIDE, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal kernelSize As Integer, ByVal stride As Integer, ByVal padding As Integer)
				Me.New(DEFAULT_POOLING, kernelSize, stride, padding)
			End Sub

			Public Sub New(ByVal kernelSize As Integer, ByVal stride As Integer)
				Me.New(DEFAULT_POOLING, kernelSize, stride, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal kernelSize As Integer)
				Me.New(DEFAULT_POOLING, kernelSize, DEFAULT_STRIDE, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal poolingType As PoolingType)
				Me.New(poolingType, DEFAULT_KERNEL, DEFAULT_STRIDE, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType)
				Me.New(poolingType, DEFAULT_KERNEL, DEFAULT_STRIDE, DEFAULT_PADDING)
			End Sub

			Protected Friend Overrides Function allowCausal() As Boolean
				Return True
			End Function

			Public Sub New()
				Me.New(DEFAULT_POOLING, DEFAULT_KERNEL, DEFAULT_STRIDE, DEFAULT_PADDING)
			End Sub

			Public Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType, ByVal kernelSize As Integer, ByVal stride As Integer, ByVal padding As Integer)
				Me.KernelSize = kernelSize
				Me.Padding = padding
				Me.Stride = stride
			End Sub

			Public Sub New(ByVal poolingType As PoolingType, ByVal kernelSize As Integer, ByVal stride As Integer, ByVal padding As Integer)
				Me.poolingType_Conflict = poolingType.toPoolingType()
				Me.KernelSize = kernelSize
				Me.Stride = stride
				Me.Padding = padding
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public Subsampling1DLayer build()
			Public Overridable Overloads Function build() As Subsampling1DLayer
				If poolingType_Conflict = org.deeplearning4j.nn.conf.layers.PoolingType.PNORM AndAlso pnorm_Conflict <= 0 Then
					Throw New System.InvalidOperationException("Incorrect Subsampling config: p-norm must be set when using PoolingType.PNORM")
				End If
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding)
				ConvolutionUtils.validateCnnKernelStridePadding(kernelSize, stride, padding)

				Return New Subsampling1DLayer(Me)
			End Function

			''' <summary>
			''' Kernel size
			''' </summary>
			''' <param name="kernelSize"> kernel size </param>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function kernelSize(ByVal kernelSize_Conflict As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Stride
			''' </summary>
			''' <param name="stride"> stride value </param>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function stride(ByVal stride_Conflict As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Padding
			''' </summary>
			''' <param name="padding"> padding value </param>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padding(ByVal padding_Conflict As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function

			''' <summary>
			''' Kernel size
			''' </summary>
			''' <param name="kernelSize"> kernel size </param>
			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize(0) = ValidationUtils.validate1NonNegative(kernelSize, "kernelSize")(0)
				End Set
			End Property

			''' <summary>
			''' Stride
			''' </summary>
			''' <param name="stride"> stride value </param>
			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride(0) = ValidationUtils.validate1NonNegative(stride, "stride")(0)
				End Set
			End Property

			''' <summary>
			''' Padding
			''' </summary>
			''' <param name="padding"> padding value </param>
			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding(0) = ValidationUtils.validate1NonNegative(padding, "padding")(0)
				End Set
			End Property
		End Class
	End Class

End Namespace