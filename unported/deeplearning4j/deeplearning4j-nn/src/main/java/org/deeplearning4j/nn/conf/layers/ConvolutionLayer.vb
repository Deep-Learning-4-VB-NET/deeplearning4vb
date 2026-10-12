Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonIgnore = org.nd4j.shade.jackson.annotation.JsonIgnore

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class ConvolutionLayer extends FeedForwardLayer
	<Serializable>
	Public Class ConvolutionLayer
		Inherits FeedForwardLayer

'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend hasBias_Conflict As Boolean = True
		Protected Friend convolutionMode As ConvolutionMode = ConvolutionMode.Truncate 'Default to truncate here - default for 0.6.0 and earlier networks on JSON deserialization
		Protected Friend dilation() As Integer = {1, 1}
		Protected Friend kernelSize() As Integer ' Square filter
		Protected Friend stride() As Integer ' Default is 2. Down-sample by a factor of 2
		Protected Friend padding() As Integer
		Protected Friend cudnnAllowFallback As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected CNN2DFormat cnn2dDataFormat = CNN2DFormat.NCHW;
		Protected Friend cnn2dDataFormat As CNN2DFormat = CNN2DFormat.NCHW 'default value for legacy serialization reasons
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore @EqualsAndHashCode.Exclude private boolean defaultValueOverriden = false;
		Private defaultValueOverriden As Boolean = False

		''' <summary>
		''' The "PREFER_FASTEST" mode will pick the fastest algorithm for the specified parameters from the <seealso cref="FwdAlgo"/>,
		''' <seealso cref="BwdFilterAlgo"/>, and <seealso cref="BwdDataAlgo"/> lists, but they may be very memory intensive, so if weird errors
		''' occur when using cuDNN, please try the "NO_WORKSPACE" mode. Alternatively, it is possible to specify the
		''' algorithm manually by setting the "USER_SPECIFIED" mode, but this is not recommended.
		''' <para>
		''' Note: Currently only supported with cuDNN.
		''' </para>
		''' </summary>
		Public Enum AlgoMode
			NO_WORKSPACE
			PREFER_FASTEST
			USER_SPECIFIED
		End Enum

		''' <summary>
		''' The forward algorithm to use when <seealso cref="AlgoMode"/> is set to "USER_SPECIFIED".
		''' <para>
		''' Note: Currently only supported with cuDNN.
		''' </para>
		''' </summary>
		Public Enum FwdAlgo
			IMPLICIT_GEMM
			IMPLICIT_PRECOMP_GEMM
			GEMM
			DIRECT
			FFT
			FFT_TILING
			WINOGRAD
			WINOGRAD_NONFUSED
			COUNT
		End Enum

		''' <summary>
		''' The backward filter algorithm to use when <seealso cref="AlgoMode"/> is set to "USER_SPECIFIED".
		''' <para>
		''' Note: Currently only supported with cuDNN.
		''' </para>
		''' </summary>
		Public Enum BwdFilterAlgo
			ALGO_0
			ALGO_1
			FFT
			ALGO_3
			WINOGRAD
			WINOGRAD_NONFUSED
			FFT_TILING
			COUNT
		End Enum

		''' <summary>
		''' The backward data algorithm to use when <seealso cref="AlgoMode"/> is set to "USER_SPECIFIED".
		''' <para>
		''' Note: Currently only supported with cuDNN.
		''' </para>
		''' </summary>
		Public Enum BwdDataAlgo
			ALGO_0
			ALGO_1
			FFT
			FFT_TILING
			WINOGRAD
			WINOGRAD_NONFUSED
			COUNT
		End Enum

		''' <summary>
		''' Defaults to "PREFER_FASTEST", but "NO_WORKSPACE" uses less memory.
		''' </summary>
		Protected Friend cudnnAlgoMode As AlgoMode = AlgoMode.PREFER_FASTEST
		Protected Friend cudnnFwdAlgo As FwdAlgo
		Protected Friend cudnnBwdFilterAlgo As BwdFilterAlgo
		Protected Friend cudnnBwdDataAlgo As BwdDataAlgo

		''' <summary>
		''' ConvolutionLayer nIn in the input layer is the number of channels nOut is the number of filters to be used in the
		''' net or in other words the channels The builder specifies the filter/kernel size, the stride and padding The
		''' pooling layer takes the kernel size
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: protected ConvolutionLayer(BaseConvBuilder<?> builder)
		Protected Friend Sub New(ByVal builder As BaseConvBuilder(Of T1))
			MyBase.New(builder)
			Dim [dim] As Integer = builder.convolutionDim

			Me.hasBias_Conflict = builder.hasBias_Conflict
			Me.convolutionMode = builder.convolutionMode_Conflict
			Me.dilation = builder.dilation_Conflict
			If builder.kernelSize_Conflict.Length <> [dim] Then
				Throw New System.ArgumentException("Kernel argument should be a " & [dim] & "d array, got " & Arrays.toString(builder.kernelSize_Conflict))
			End If
			Me.kernelSize = builder.kernelSize_Conflict
			If builder.stride_Conflict.Length <> [dim] Then
				Throw New System.ArgumentException("Strides argument should be a " & [dim] & "d array, got " & Arrays.toString(builder.stride_Conflict))
			End If
			Me.stride = builder.stride_Conflict
			If builder.padding_Conflict.Length <> [dim] Then
				Throw New System.ArgumentException("Padding argument should be a " & [dim] & "d array, got " & Arrays.toString(builder.padding_Conflict))
			End If
			Me.padding = builder.padding_Conflict
			If builder.dilation_Conflict.Length <> [dim] Then
				Throw New System.ArgumentException("Dilation argument should be a " & [dim] & "d array, got " & Arrays.toString(builder.dilation_Conflict))
			End If
			Me.dilation = builder.dilation_Conflict
			Me.cudnnAlgoMode = builder.cudnnAlgoMode_Conflict
			Me.cudnnFwdAlgo = builder.cudnnFwdAlgo
			Me.cudnnBwdFilterAlgo = builder.cudnnBwdFilterAlgo
			Me.cudnnBwdDataAlgo = builder.cudnnBwdDataAlgo
			Me.cudnnAllowFallback = builder.cudnnAllowFallback_Conflict
			If TypeOf builder Is Builder Then
				Me.cnn2dDataFormat = DirectCast(builder, Builder).dataFormat_Conflict
			End If

			initializeConstraints(builder)
		End Sub

		Public Overridable Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overrides Function clone() As ConvolutionLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As ConvolutionLayer = CType(MyBase.clone(), ConvolutionLayer)
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
			LayerValidation.assertNInNOutSet("ConvolutionLayer", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.convolution.ConvolutionLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return ConvolutionParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Convolution layer (layer name=""" & getLayerName() & """): Expected CNN input, got " & inputType)
			End If

			Return InputTypeUtil.getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, nOut, layerIndex, getLayerName(), cnn2dDataFormat, GetType(ConvolutionLayer))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Convolution layer (layer name=""" & getLayerName() & """): Expected CNN input, got " & inputType)
			End If

			If Not defaultValueOverriden OrElse nIn <= 0 OrElse override Then
				Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				Me.nIn = c.getChannels()
				Me.cnn2dDataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
			End If

			If cnn2dDataFormat = Nothing OrElse override Then
				Me.cnn2dDataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Convolution layer (layer name=""" & getLayerName() & """): input is null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, getLayerName())
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim paramSize As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(paramSize)))

			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim outputType As InputType.InputTypeConvolutional = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional)

			'TODO convolution helper memory use... (CuDNN etc)

			'During forward pass: im2col array, mmul (result activations), in-place broadcast add
			Dim im2colSizePerEx As val = c.getChannels() * outputType.getHeight() * outputType.getWidth() * kernelSize(0) * kernelSize(1)

			'During training: have im2col array, in-place gradient calculation, then epsilons...
			'But: im2col array may be cached...
			Dim trainWorkingMemoryPerEx As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			Dim cachedPerEx As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()

			'During backprop: im2col array for forward pass (possibly cached) + the epsilon6d array required to calculate
			' the 4d epsilons (equal size to input)
			'Note that the eps6d array is same size as im2col
			For Each cm As CacheMode In System.Enum.GetValues(GetType(CacheMode))
				Dim trainWorkingSizePerEx As Long
				Dim cacheMemSizePerEx As Long = 0
				If cm = CacheMode.NONE Then
					trainWorkingSizePerEx = 2 * im2colSizePerEx
				Else
					'im2col is cached, but epsNext2d/eps6d is not
					cacheMemSizePerEx = im2colSizePerEx
					trainWorkingSizePerEx = im2colSizePerEx
				End If

				If getIDropout() IsNot Nothing Then
					'Dup on the input before dropout, but only for training
					trainWorkingSizePerEx += inputType.arrayElementsPerExample()
				End If

				trainWorkingMemoryPerEx(cm) = trainWorkingSizePerEx
				cachedPerEx(cm) = cacheMemSizePerEx
			Next cm

			Return (New LayerMemoryReport.Builder(layerName, GetType(ConvolutionLayer), inputType, outputType)).standardMemory(paramSize, updaterStateSize).workingMemory(0, im2colSizePerEx, MemoryReport.CACHE_MODE_ALL_ZEROS, trainWorkingMemoryPerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, cachedPerEx).build()

		End Function

		Public Class Builder
			Inherits BaseConvBuilder(Of Builder)

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				MyBase.New(kernelSize, stride, padding)
			End Sub

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer)
				MyBase.New(kernelSize, stride)
			End Sub

			Public Sub New(ParamArray ByVal kernelSize() As Integer)
				MyBase.New(kernelSize)
			End Sub

			Public Sub New()
				MyBase.New()
			End Sub

'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataFormat_Conflict As CNN2DFormat = CNN2DFormat.NCHW

			Protected Friend Overrides Function allowCausal() As Boolean
				'Causal convolution - allowed for 1D only
				Return False
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

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.dataFormat_Conflict = format
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ConvolutionLayer build()
			Public Overrides Function build() As ConvolutionLayer
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding_Conflict)
				ConvolutionUtils.validateCnnKernelStridePadding(kernelSize_Conflict, stride_Conflict, padding_Conflict)

				Return New ConvolutionLayer(Me)
			End Function

			''' <summary>
			''' Set kernel size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="kernelSize"> kernel size </param>
			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize_Conflict = ValidationUtils.validate2NonNegative(kernelSize, False, "kernelSize")
				End Set
			End Property

			''' <summary>
			''' Set stride size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="stride"> kernel size </param>
			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate2NonNegative(stride, False, "stride")
				End Set
			End Property

			''' <summary>
			''' Set padding size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="padding"> kernel size </param>
			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate2NonNegative(padding, False, "padding")
				End Set
			End Property

			''' <summary>
			''' Set dilation size for 3D convolutions in (depth, height, width) order
			''' </summary>
			''' <param name="dilation"> kernel size </param>
			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate2NonNegative(dilation, False, "dilation")
				End Set
			End Property

			Public Overridable WriteOnly Property DataFormat As CNN2DFormat
				Set(ByVal dataFormat As CNN2DFormat)
					Me.dataFormat_Conflict = dataFormat
				End Set
			End Property
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static abstract class BaseConvBuilder<T extends BaseConvBuilder<T>> extends FeedForwardLayer.Builder<T>
		Public MustInherit Class BaseConvBuilder(Of T As BaseConvBuilder(Of T))
			Inherits FeedForwardLayer.Builder(Of T)

			Protected Friend convolutionDim As Integer = 2 ' 2D convolution by default

			''' <summary>
			''' If true (default): include bias parameters in the model. False: no bias.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend hasBias_Conflict As Boolean = True

			''' <summary>
			''' Set the convolution mode for the Convolution layer. See <seealso cref="ConvolutionMode"/> for more details
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field convolutionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend convolutionMode_Conflict As ConvolutionMode

			''' <summary>
			''' Kernel dilation. Default: {1, 1}, which is standard convolutions. Used for implementing dilated convolutions,
			''' which are also known as atrous convolutions.
			''' <para>
			''' For more details, see:
			''' <a href="https://arxiv.org/abs/1511.07122">Yu and Koltun (2014)</a> and
			''' <a href="https://arxiv.org/abs/1412.7062">Chen et al. (2014)</a>, as well as
			''' <a href="http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions">
			''' http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions</a><br>
			''' 
			''' </para>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dilation_Conflict() As Integer = {1, 1}
'JAVA TO VB CONVERTER NOTE: The field kernelSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Public kernelSize_Conflict() As Integer = {5, 5}
'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend stride_Conflict() As Integer = {1, 1}
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend padding_Conflict() As Integer = {0, 0}

			''' <summary>
			''' Defaults to "PREFER_FASTEST", but "NO_WORKSPACE" uses less memory.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cudnnAlgoMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAlgoMode_Conflict As AlgoMode = Nothing
			Protected Friend cudnnFwdAlgo As FwdAlgo
			Protected Friend cudnnBwdFilterAlgo As BwdFilterAlgo
			Protected Friend cudnnBwdDataAlgo As BwdDataAlgo

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for ConvolutionLayer will be used
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cudnnAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAllowFallback_Conflict As Boolean = True


			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal [dim] As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
				Me.setDilation(dilation)
				Me.setConvolutionDim([dim])
			End Sub

			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
				Me.setDilation(dilation)
			End Sub

			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal [dim] As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
				Me.setConvolutionDim([dim])
			End Sub

			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
			End Sub

			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal [dim] As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setConvolutionDim([dim])
			End Sub


			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
			End Sub

			Protected Friend Sub New(ByVal [dim] As Integer, ParamArray ByVal kernelSize() As Integer)
				Me.setKernelSize(kernelSize)
				Me.setConvolutionDim([dim])
			End Sub


			Protected Friend Sub New(ParamArray ByVal kernelSize() As Integer)
				Me.setKernelSize(kernelSize)
			End Sub

			Protected Friend Sub New()
			End Sub

			Protected Friend MustOverride Function allowCausal() As Boolean

			Protected Friend Overridable WriteOnly Property ConvolutionMode As ConvolutionMode
				Set(ByVal convolutionMode As ConvolutionMode)
					Preconditions.checkState(allowCausal() OrElse convolutionMode <> ConvolutionMode.Causal, "Causal convolution mode can only be used with 1D" & " convolutional neural network layers")
					Me.convolutionMode_Conflict = convolutionMode
				End Set
			End Property


			''' <summary>
			''' If true (default): include bias parameters in the model. False: no bias.
			''' </summary>
			''' <param name="hasBias"> If true: include bias parameters in this model </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As T
				Me.setHasBias(hasBias_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the convolution mode for the Convolution layer. See <seealso cref="ConvolutionMode"/> for more details
			''' </summary>
			''' <param name="convolutionMode"> Convolution mode for layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter convolutionMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function convolutionMode(ByVal convolutionMode_Conflict As ConvolutionMode) As T
				Me.ConvolutionMode = convolutionMode_Conflict
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Kernel dilation. Default: {1, 1}, which is standard convolutions. Used for implementing dilated convolutions,
			''' which are also known as atrous convolutions.
			''' <para>
			''' For more details, see:
			''' <a href="https://arxiv.org/abs/1511.07122">Yu and Koltun (2014)</a> and
			''' <a href="https://arxiv.org/abs/1412.7062">Chen et al. (2014)</a>, as well as
			''' <a href="http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions">
			''' http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions</a><br>
			''' 
			''' </para>
			''' </summary>
			''' <param name="dilation"> Dilation for kernel </param>
'JAVA TO VB CONVERTER NOTE: The parameter dilation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dilation(ParamArray ByVal dilation_Conflict() As Integer) As T
				Me.setDilation(dilation_Conflict)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function kernelSize(ParamArray ByVal kernelSize_Conflict() As Integer) As T
				Me.setKernelSize(kernelSize_Conflict)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function stride(ParamArray ByVal stride_Conflict() As Integer) As T
				Me.setStride(stride_Conflict)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padding(ParamArray ByVal padding_Conflict() As Integer) As T
				Me.setPadding(padding_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Defaults to "PREFER_FASTEST", but "NO_WORKSPACE" uses less memory.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter cudnnAlgoMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function cudnnAlgoMode(ByVal cudnnAlgoMode_Conflict As AlgoMode) As T
				Me.setCudnnAlgoMode(cudnnAlgoMode_Conflict)
				Return CType(Me, T)
			End Function

			Public Overridable Function cudnnFwdMode(ByVal cudnnFwdAlgo As FwdAlgo) As T
				Me.setCudnnFwdAlgo(cudnnFwdAlgo)
				Return CType(Me, T)
			End Function

			Public Overridable Function cudnnBwdFilterMode(ByVal cudnnBwdFilterAlgo As BwdFilterAlgo) As T
				Me.setCudnnBwdFilterAlgo(cudnnBwdFilterAlgo)
				Return CType(Me, T)
			End Function

			Public Overridable Function cudnnBwdDataMode(ByVal cudnnBwdDataAlgo As BwdDataAlgo) As T
				Me.setCudnnBwdDataAlgo(cudnnBwdDataAlgo)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If true, the built-in
			''' (non-CuDNN) implementation for ConvolutionLayer will be used
			''' </summary>
			''' @deprecated Use <seealso cref="helperAllowFallback(Boolean)"/>
			''' 
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			<Obsolete("Use <seealso cref=""helperAllowFallback(Boolean)""/>")>
			Public Overridable Function cudnnAllowFallback(ByVal allowFallback As Boolean) As T
				Me.setCudnnAllowFallback(allowFallback)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' When using CuDNN or MKLDNN and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If true, the built-in
			''' (non-MKL/CuDNN) implementation for ConvolutionLayer will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As T
				Me.cudnnAllowFallback_Conflict = allowFallback
				Return CType(Me, T)
			End Function
		End Class
	End Class

End Namespace