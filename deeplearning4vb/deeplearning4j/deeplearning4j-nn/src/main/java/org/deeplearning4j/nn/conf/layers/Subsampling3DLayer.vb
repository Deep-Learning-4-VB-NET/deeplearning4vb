Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Convolution3DUtils = org.deeplearning4j.util.Convolution3DUtils
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Subsampling3DLayer extends NoParamLayer
	<Serializable>
	Public Class Subsampling3DLayer
		Inherits NoParamLayer

		Protected Friend convolutionMode As ConvolutionMode = ConvolutionMode.Truncate
		Protected Friend poolingType As org.deeplearning4j.nn.conf.layers.PoolingType
		Protected Friend kernelSize() As Integer
		Protected Friend stride() As Integer
		Protected Friend padding() As Integer
		Protected Friend dilation() As Integer
		Protected Friend cudnnAllowFallback As Boolean = True
		Protected Friend dataFormat As Convolution3D.DataFormat = Convolution3D.DataFormat.NCDHW 'Default for 1.0.0-beta3 and earlier (before config added)

		Public NotInheritable Class PoolingType
			Public Shared ReadOnly MAX As New PoolingType("MAX", InnerEnum.MAX)
			Public Shared ReadOnly AVG As New PoolingType("AVG", InnerEnum.AVG)

			Private Shared ReadOnly valueList As New List(Of PoolingType)()

			Shared Sub New()
				valueList.Add(MAX)
				valueList.Add(AVG)
			End Sub

			Public Enum InnerEnum
				MAX
				AVG
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

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.poolingType = builder.poolingType_Conflict
			If builder.kernelSize.Length <> 3 Then
				Throw New System.ArgumentException("Kernel size must be length 3")
			End If
			Me.kernelSize = builder.kernelSize
			If builder.stride.Length <> 3 Then
				Throw New System.ArgumentException("Invalid stride, must be length 3")
			End If
			Me.stride = builder.stride
			Me.padding = builder.padding
			Me.dilation = builder.dilation_Conflict
			Me.convolutionMode = builder.convolutionMode_Conflict
			Me.cudnnAllowFallback = builder.cudnnAllowFallback_Conflict
			Me.dataFormat = builder.dataFormat_Conflict
		End Sub

		Public Overrides Function clone() As Subsampling3DLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Subsampling3DLayer = CType(MyBase.clone(), Subsampling3DLayer)

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

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal iterationListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.subsampling.Subsampling3DLayer(conf, networkDataType)
			ret.setListeners(iterationListeners)
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
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling 3D layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If

			Dim inChannels As Long = DirectCast(inputType, InputType.InputTypeConvolutional3D).getChannels()
			If inChannels > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Return InputTypeUtil.getOutputTypeCnn3DLayers(inputType, dataFormat, kernelSize, stride, padding, New Integer() {1, 1, 1}, convolutionMode, CInt(inChannels), layerIndex, LayerName, GetType(Subsampling3DLayer))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op: subsampling layer doesn't have nIn value
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling 3D layer (layer name=""" & LayerName & """): input is null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, LayerName)
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'Not applicable
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("SubsamplingLayer does not contain parameters")
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
			Dim outputType As InputType.InputTypeConvolutional3D = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional3D)
			Dim actElementsPerEx As val = outputType.arrayElementsPerExample()

			'During forward pass: im2col array + reduce. Reduce is counted as activations, so only im2col is working mem
			Dim im2colSizePerEx As val = c.getChannels() * outputType.getHeight() * outputType.getWidth() * outputType.getDepth() * kernelSize(0) * kernelSize(1)

			'Current implementation does NOT cache im2col etc... which means: it's recalculated on each backward pass
			Dim trainingWorkingSizePerEx As Long = im2colSizePerEx
			If getIDropout() IsNot Nothing Then
				'Dup on the input before dropout, but only for training
				trainingWorkingSizePerEx += inputType.arrayElementsPerExample()
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(Subsampling3DLayer), inputType, outputType)).standardMemory(0, 0).workingMemory(0, im2colSizePerEx, 0, trainingWorkingSizePerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder extends BaseSubsamplingBuilder<Builder>
		Public Class Builder
			Inherits BaseSubsamplingBuilder(Of Builder)

			''' <summary>
			''' The data format for input and output activations.<br> NCDHW: activations (in/out) should have shape
			''' [minibatch, channels, depth, height, width]<br> NDHWC: activations (in/out) should have shape [minibatch,
			''' depth, height, width, channels]<br>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataFormat_Conflict As Convolution3D.DataFormat = Convolution3D.DataFormat.NCDHW

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
			''' The data format for input and output activations.<br> NCDHW: activations (in/out) should have shape
			''' [minibatch, channels, depth, height, width]<br> NDHWC: activations (in/out) should have shape [minibatch,
			''' depth, height, width, channels]<br>
			''' </summary>
			''' <param name="dataFormat"> Data format to use for activations </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As Convolution3D.DataFormat) As Builder
				Me.setDataFormat(dataFormat_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Subsampling3DLayer build()
			Public Overrides Function build() As Subsampling3DLayer
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding)
				Convolution3DUtils.validateCnn3DKernelStridePadding(kernelSize, stride, padding)
				Return New Subsampling3DLayer(Me)
			End Function

			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize = ValidationUtils.validate3NonNegative(kernelSize, "kernelSize")
				End Set
			End Property

			''' <summary>
			''' Stride
			''' </summary>
			''' <param name="stride"> stride in height and width dimensions </param>
			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride = ValidationUtils.validate3NonNegative(stride, "stride")
				End Set
			End Property

			''' <summary>
			''' Padding
			''' </summary>
			''' <param name="padding"> padding in the height and width dimensions </param>
			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding = ValidationUtils.validate3NonNegative(padding, "padding")
				End Set
			End Property

			''' <summary>
			''' Dilation
			''' </summary>
			''' <param name="dilation"> padding in the height and width dimensions </param>
			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate3NonNegative(dilation, "dilation")
				End Set
			End Property
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @NoArgsConstructor protected static abstract class BaseSubsamplingBuilder<T extends BaseSubsamplingBuilder<T>> extends Layer.Builder<T>
		Protected Friend MustInherit Class BaseSubsamplingBuilder(Of T As BaseSubsamplingBuilder(Of T))
			Inherits Layer.Builder(Of T)

'JAVA TO VB CONVERTER NOTE: The field poolingType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend poolingType_Conflict As org.deeplearning4j.nn.conf.layers.PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType.MAX

			Protected Friend kernelSize() As Integer = {1, 1, 1}
			Protected Friend stride() As Integer = {2, 2, 2}
			Protected Friend padding() As Integer = {0, 0, 0}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected int[] dilation = new int[] {1, 1, 1};
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dilation_Conflict() As Integer = {1, 1, 1}

			''' <summary>
			''' Set the convolution mode for the Convolution layer. See <seealso cref="ConvolutionMode"/> for more details
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field convolutionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend convolutionMode_Conflict As ConvolutionMode = ConvolutionMode.Same

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for ConvolutionLayer will be used
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cudnnAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAllowFallback_Conflict As Boolean = True

			Public Overridable WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Preconditions.checkArgument(dilation.Length = 1 OrElse dilation.Length = 3, "Must have 1 or 3 dilation values - got %s", dilation)
    
					If dilation.Length = 1 Then
						Me.dilation(dilation(0), dilation(0), dilation(0))
					Else
						Me.dilation(dilation(0), dilation(1), dilation(2))
					End If
				End Set
			End Property

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

			Protected Friend Overridable WriteOnly Property ConvolutionMode As ConvolutionMode
				Set(ByVal convolutionMode As ConvolutionMode)
					Preconditions.checkState(convolutionMode <> ConvolutionMode.Causal, "Causal convolution mode can only be used with 1D" & " convolutional neural network layers")
					Me.convolutionMode_Conflict = convolutionMode
				End Set
			End Property

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

			Public Overridable Function dilation(ByVal dDepth As Integer, ByVal dHeight As Integer, ByVal dWidth As Integer) As T
				Me.Dilation = New Integer() {dDepth, dHeight, dWidth}
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
			''' (non-MKL/CuDNN) implementation for Subsampling3DLayer will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As T
				Me.cudnnAllowFallback_Conflict = allowFallback
				Return CType(Me, T)
			End Function
		End Class

	End Class

End Namespace