Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class SubsamplingLayer extends NoParamLayer
	<Serializable>
	Public Class SubsamplingLayer
		Inherits NoParamLayer

		Protected Friend convolutionMode As ConvolutionMode = ConvolutionMode.Truncate 'Default to truncate here - default for 0.6.0 and earlier networks on JSON deserialization
		Protected Friend poolingType As org.deeplearning4j.nn.conf.layers.PoolingType
		Protected Friend kernelSize() As Integer ' Same as filter size from the last conv layer
		Protected Friend stride() As Integer ' Default is 2. Down-sample by a factor of 2
		Protected Friend padding() As Integer
		Protected Friend dilation() As Integer = {1, 1}
'JAVA TO VB CONVERTER NOTE: The field pnorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend pnorm_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field eps was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend eps_Conflict As Double
		Protected Friend cudnnAllowFallback As Boolean = True
		Protected Friend cnn2dDataFormat As CNN2DFormat = CNN2DFormat.NCHW 'default value for legacy reasons
		Public Const DEFAULT_FORMAT As CNN2DFormat = CNN2DFormat.NCHW
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore @EqualsAndHashCode.Exclude private boolean defaultValueOverridden = false;
		Private defaultValueOverridden As Boolean = False

	'    
	'    Default here for JSON deserialization of 1.0.0-beta4 and earlier models. New models default to false via builder.
	'    This impacts average pooling only - whether the divisor should include or exclude padding along image edges.
	'    DL4J originally included padding in the count, versions after 1.0.0-beta4 will exclude it by default.
	'     
		Protected Friend avgPoolIncludePadInDivisor As Boolean = True

		Public NotInheritable Class PoolingType
			Public Shared ReadOnly MAX As New PoolingType("MAX", InnerEnum.MAX)
			Public Shared ReadOnly AVG As New PoolingType("AVG", InnerEnum.AVG)
			Public Shared ReadOnly SUM As New PoolingType("SUM", InnerEnum.SUM)
			Public Shared ReadOnly PNORM As New PoolingType("PNORM", InnerEnum.PNORM)

			Private Shared ReadOnly valueList As New List(Of PoolingType)()

			Shared Sub New()
				valueList.Add(MAX)
				valueList.Add(AVG)
				valueList.Add(SUM)
				valueList.Add(PNORM)
			End Sub

			Public Enum InnerEnum
				MAX
				AVG
				SUM
				PNORM
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Public Function toPoolingType() As org.deeplearning4j.nn.conf.layers.PoolingType
				Select Case Me
					Case MAX
						Return org.deeplearning4j.nn.conf.layers.PoolingType.MAX
					Case AVG
						Return org.deeplearning4j.nn.conf.layers.PoolingType.AVG
					Case SUM
						Return org.deeplearning4j.nn.conf.layers.PoolingType.SUM
					Case PNORM
						Return org.deeplearning4j.nn.conf.layers.PoolingType.PNORM
				End Select
				Throw New System.NotSupportedException("Unknown/not supported pooling type: " & Me)
			End Function

			Public Shared Function values() As PoolingType()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As PoolingType, ByVal two As PoolingType) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As PoolingType, ByVal two As PoolingType) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As PoolingType
				For Each enumInstance As PoolingType In PoolingType.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class

		Protected Friend Sub New(ByVal builder As BaseSubsamplingBuilder)
			MyBase.New(builder)
			Me.poolingType = builder.poolingType
			If builder.kernelSize.length <> 2 Then
				Throw New System.ArgumentException("Kernel size of should be rows x columns (a 2d array)")
			End If
			Me.kernelSize = builder.kernelSize
			If builder.stride.length <> 2 Then
				Throw New System.ArgumentException("Invalid stride, must be length 2")
			End If
			Me.stride = builder.stride
			Me.padding = builder.padding
			Me.convolutionMode = builder.convolutionMode
			Me.cnn2dDataFormat = builder.cnn2DFormat

			If TypeOf builder Is Builder Then
				Me.dilation = CType(builder, Builder).dilation_Conflict
			End If

			Me.pnorm_Conflict = builder.pnorm
			Me.eps_Conflict = builder.eps
			Me.cudnnAllowFallback = builder.cudnnAllowFallback
			Me.avgPoolIncludePadInDivisor = builder.avgPoolIncludePadInDivisor
		End Sub

		Public Overrides Function clone() As SubsamplingLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As SubsamplingLayer = CType(MyBase.clone(), SubsamplingLayer)

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

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If

			Return InputTypeUtil.getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, DirectCast(inputType, InputType.InputTypeConvolutional).getChannels(), layerIndex, LayerName, cnn2dDataFormat, GetType(SubsamplingLayer))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op: subsampling layer doesn't have nIn value
			If Not defaultValueOverridden OrElse override Then
				Me.cnn2dDataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
				defaultValueOverridden = True
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling layer (layer name=""" & LayerName & """): input is null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("SubsamplingLayer does not contain parameters")
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim outputType As InputType.InputTypeConvolutional = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional)
			Dim actElementsPerEx As val = outputType.arrayElementsPerExample()

			'TODO Subsampling helper memory use... (CuDNN etc)

			'During forward pass: im2col array + reduce. Reduce is counted as activations, so only im2col is working mem
			Dim im2colSizePerEx As val = c.getChannels() * outputType.getHeight() * outputType.getWidth() * kernelSize(0) * kernelSize(1)

			'Current implementation does NOT cache im2col etc... which means: it's recalculated on each backward pass
			Dim trainingWorkingSizePerEx As Long = im2colSizePerEx
			If getIDropout() IsNot Nothing Then
				'Dup on the input before dropout, but only for training
				trainingWorkingSizePerEx += inputType.arrayElementsPerExample()
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(SubsamplingLayer), inputType, outputType)).standardMemory(0, 0).workingMemory(0, im2colSizePerEx, 0, trainingWorkingSizePerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overridable ReadOnly Property Pnorm As Integer
			Get
				Return pnorm_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Eps As Double
			Get
				Return eps_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder extends BaseSubsamplingBuilder<Builder>
		Public Class Builder
			Inherits BaseSubsamplingBuilder(Of Builder)

			''' <summary>
			''' Kernel dilation. Default: {1, 1}, which is standard convolutions. Used for implementing dilated convolutions,
			''' which are also known as atrous convolutions.<br> NOTE: Kernel dilation is less common in practice for
			''' subsampling layers, compared to convolutional layers.
			''' 
			''' For more details, see:
			''' <a href="https://arxiv.org/abs/1511.07122">Yu and Koltun (2014)</a> and
			''' <a href="https://arxiv.org/abs/1412.7062">Chen et al. (2014)</a>, as well as
			''' <a href="http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions">
			''' http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions</a><br>
			''' 
			''' Dilation for kernel
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dilation_Conflict() As Integer = {1, 1}

			Public Sub New(ByVal poolingType As PoolingType, ByVal kernelSize() As Integer, ByVal stride() As Integer)
				MyBase.New(poolingType, kernelSize, stride)
			End Sub

			Public Sub New(ByVal poolingType As PoolingType, ByVal kernelSize() As Integer)
				MyBase.New(poolingType, kernelSize)
			End Sub

			Public Sub New(ByVal poolingType As PoolingType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				MyBase.New(poolingType, kernelSize, stride, padding)
			End Sub

			Public Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType, ByVal kernelSize() As Integer)
				MyBase.New(poolingType, kernelSize)
			End Sub

			Public Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				MyBase.New(poolingType, kernelSize, stride, padding)
			End Sub

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				MyBase.New(kernelSize, stride, padding)
			End Sub

			Public Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer)
				MyBase.New(kernelSize, stride)
			End Sub

			Public Sub New(ParamArray ByVal kernelSize() As Integer)
				MyBase.New(kernelSize)
			End Sub

			Public Sub New(ByVal poolingType As PoolingType)
				MyBase.New(poolingType)
			End Sub

			Public Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType)
				MyBase.New(poolingType)
			End Sub

			Protected Friend Overrides Function allowCausal() As Boolean
				'Only conv1d/subsampling1d can use causal mode
				Return False
			End Function

			''' <summary>
			''' Kernel size
			''' </summary>
			''' <param name="kernelSize"> kernel size in height and width dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function kernelSize(ParamArray ByVal kernelSize_Conflict() As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Stride
			''' </summary>
			''' <param name="stride"> stride in height and width dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function stride(ParamArray ByVal stride_Conflict() As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Padding
			''' </summary>
			''' <param name="padding"> padding in the height and width dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padding(ParamArray ByVal padding_Conflict() As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function



			''' <summary>
			''' Kernel dilation. Default: {1, 1}, which is standard convolutions. Used for implementing dilated convolutions,
			''' which are also known as atrous convolutions.<br> NOTE: Kernel dilation is less common in practice for
			''' subsampling layers, compared to convolutional layers.
			''' 
			''' For more details, see:
			''' <a href="https://arxiv.org/abs/1511.07122">Yu and Koltun (2014)</a> and
			''' <a href="https://arxiv.org/abs/1412.7062">Chen et al. (2014)</a>, as well as
			''' <a href="http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions">
			''' http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions</a><br>
			''' </summary>
			''' <param name="dilation"> Dilation for kernel </param>
'JAVA TO VB CONVERTER NOTE: The parameter dilation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dilation(ParamArray ByVal dilation_Conflict() As Integer) As Builder
				Me.Dilation = dilation_Conflict
				Return Me
			End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public SubsamplingLayer build()
			Public Overrides Function build() As SubsamplingLayer
				If poolingType_Conflict = org.deeplearning4j.nn.conf.layers.PoolingType.PNORM AndAlso pnorm_Conflict <= 0 Then
					Throw New System.InvalidOperationException("Incorrect Subsampling config: p-norm must be set when using PoolingType.PNORM")
				End If
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding)
				ConvolutionUtils.validateCnnKernelStridePadding(kernelSize, stride, padding)

				Return New SubsamplingLayer(Me)
			End Function

			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize = ValidationUtils.validate2NonNegative(kernelSize,False, "kernelSize")
				End Set
			End Property

			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride = ValidationUtils.validate2NonNegative(stride, False, "stride")
				End Set
			End Property

			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding = ValidationUtils.validate2NonNegative(padding,False, "padding")
				End Set
			End Property


			Public Overridable WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate2NonNegative(dilation, False, "dilation")
				End Set
			End Property

			Public Overridable WriteOnly Property DataFormat As CNN2DFormat
				Set(ByVal format As CNN2DFormat)
					Me.cnn2DFormat = format
				End Set
			End Property
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter protected static abstract class BaseSubsamplingBuilder<T extends BaseSubsamplingBuilder<T>> extends Layer.Builder<T>
		Protected Friend MustInherit Class BaseSubsamplingBuilder(Of T As BaseSubsamplingBuilder(Of T))
			Inherits Layer.Builder(Of T)

'JAVA TO VB CONVERTER NOTE: The field poolingType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend poolingType_Conflict As org.deeplearning4j.nn.conf.layers.PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType.MAX

			Protected Friend kernelSize() As Integer = {1, 1} ' Same as filter size from the last conv layer
			Protected Friend stride() As Integer = {2, 2} ' Default is 2. Down-sample by a factor of 2
			Protected Friend padding() As Integer = {0, 0}

			''' <summary>
			''' Set the convolution mode for the Convolution layer. See <seealso cref="ConvolutionMode"/> for more details
			''' 
			''' Convolution mode for layer
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field convolutionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend convolutionMode_Conflict As ConvolutionMode = Nothing
'JAVA TO VB CONVERTER NOTE: The field pnorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend pnorm_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field eps was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend eps_Conflict As Double = 1e-8

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for ConvolutionLayer will be used
			''' 
			''' Whether fallback to non-CuDNN implementation should be used
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cudnnAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAllowFallback_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field avgPoolIncludePadInDivisor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend avgPoolIncludePadInDivisor_Conflict As Boolean = False

			''' <summary>
			''' Configure the 2d data format
			''' </summary>
			Protected Friend cnn2DFormat As CNN2DFormat = CNN2DFormat.NCHW

			Protected Friend Sub New(ByVal poolingType As PoolingType, ByVal kernelSize() As Integer, ByVal stride() As Integer)
				Me.setPoolingType(poolingType.toPoolingType())
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
			End Sub

			Protected Friend Sub New(ByVal poolingType As PoolingType, ByVal kernelSize() As Integer)
				Me.setPoolingType(poolingType.toPoolingType())
				Me.setKernelSize(kernelSize)
			End Sub

			Protected Friend Sub New(ByVal poolingType As PoolingType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				Me.setPoolingType(poolingType.toPoolingType())
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
			End Sub

			Protected Friend Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType, ByVal kernelSize() As Integer)
				Me.setPoolingType(poolingType)
				Me.setKernelSize(kernelSize)
			End Sub

			Protected Friend Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				Me.setPoolingType(poolingType)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
			End Sub

			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
				Me.setPadding(padding)
			End Sub

			Protected Friend Sub New(ByVal kernelSize() As Integer, ByVal stride() As Integer)
				Me.setKernelSize(kernelSize)
				Me.setStride(stride)
			End Sub

			Protected Friend Sub New(ParamArray ByVal kernelSize() As Integer)
				Me.setKernelSize(kernelSize)
			End Sub

			Protected Friend Sub New(ByVal poolingType As PoolingType)
				Me.setPoolingType(poolingType.toPoolingType())
			End Sub

			Protected Friend Sub New(ByVal poolingType As org.deeplearning4j.nn.conf.layers.PoolingType)
				Me.setPoolingType(poolingType)
			End Sub

			Public Overridable WriteOnly Property Pnorm As Integer
				Set(ByVal pnorm As Integer)
					ValidationUtils.validateNonNegative(pnorm, "pnorm")
					Me.pnorm_Conflict = pnorm
				End Set
			End Property

			Public Overridable WriteOnly Property Eps As Double
				Set(ByVal eps As Double)
					ValidationUtils.validateNonNegative(eps, "eps")
					Me.eps_Conflict = eps
				End Set
			End Property

			Protected Friend MustOverride Function allowCausal() As Boolean

			Public Overridable WriteOnly Property ConvolutionMode As ConvolutionMode
				Set(ByVal convolutionMode As ConvolutionMode)
					Preconditions.checkState(allowCausal() OrElse convolutionMode <> ConvolutionMode.Causal, "Causal convolution mode can only be used with 1D" & " convolutional neural network layers")
					Me.convolutionMode_Conflict = convolutionMode
				End Set
			End Property

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="cnn2DFormat"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal cnn2DFormat As CNN2DFormat) As T
				Me.cnn2DFormat = cnn2DFormat
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

'JAVA TO VB CONVERTER NOTE: The parameter poolingType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function poolingType(ByVal poolingType_Conflict As PoolingType) As T
				Me.setPoolingType(poolingType_Conflict.toPoolingType())
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter poolingType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function poolingType(ByVal poolingType_Conflict As org.deeplearning4j.nn.conf.layers.PoolingType) As T
				Me.setPoolingType(poolingType_Conflict)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter pnorm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function pnorm(ByVal pnorm_Conflict As Integer) As T
				Me.Pnorm = pnorm_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter eps was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function eps(ByVal eps_Conflict As Double) As T
				Me.Eps = eps_Conflict
				Return CType(Me, T)
			End Function

			''' <summary>
			''' When using CuDNN or MKLDNN and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If true, the built-in
			''' (non-MKL/CuDNN) implementation for ConvolutionLayer will be used
			''' </summary>
			''' @deprecated Use <seealso cref="helperAllowFallback(Boolean)"/>
			''' 
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			<Obsolete("Use <seealso cref=""helperAllowFallback(Boolean)""/>")>
			Public Overridable Function cudnnAllowFallback(ByVal allowFallback As Boolean) As T
				Me.cudnnAllowFallback_Conflict = allowFallback
				Return CType(Me, T)
			End Function

			''' <summary>
			''' When using CuDNN or MKLDNN and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If true, the built-in
			''' (non-MKL/CuDNN) implementation for SubsamplingLayer will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As T
				Me.cudnnAllowFallback_Conflict = allowFallback
				Return CType(Me, T)
			End Function

			''' <summary>
			''' When doing average pooling, should the padding values be included in the divisor or not?<br>
			''' Not applicable for max and p-norm pooling.<br>
			''' Users should not usually set this - instead, leave it as the default (false). It is included mainly for backward
			''' compatibility of older models<br>
			''' Consider the following 2x2 segment along the right side of the image:<br>
			''' <pre>
			''' [A, P]
			''' [B, P]
			''' </pre>
			''' Where A and B are actual values, and P is padding (0).<br>
			''' With avgPoolIncludePadInDivisor = true, we have: out = (A+B+0+0)/4<br>
			''' With avgPoolIncludePadInDivisor = false, we have: out = (A+B+0+0)/2<br>
			''' <br>
			''' Earlier versions of DL4J originally included padding in the count, newer versions exclude it.<br>
			''' </summary>
			''' <param name="avgPoolIncludePadInDivisor"> Whether the divisor should include or exclude padding for average pooling </param>
'JAVA TO VB CONVERTER NOTE: The parameter avgPoolIncludePadInDivisor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function avgPoolIncludePadInDivisor(ByVal avgPoolIncludePadInDivisor_Conflict As Boolean) As T
				Me.avgPoolIncludePadInDivisor_Conflict = avgPoolIncludePadInDivisor_Conflict
				Return CType(Me, T)
			End Function
		End Class

	End Class

End Namespace