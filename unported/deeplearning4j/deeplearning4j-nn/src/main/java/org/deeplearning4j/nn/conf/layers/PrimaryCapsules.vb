Imports System
Imports System.Collections.Generic
Imports lombok
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeConvolutional = org.deeplearning4j.nn.conf.inputs.InputType.InputTypeConvolutional
Imports Type = org.deeplearning4j.nn.conf.inputs.InputType.Type
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports CapsuleUtils = org.deeplearning4j.util.CapsuleUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class PrimaryCapsules extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class PrimaryCapsules
		Inherits SameDiffLayer

		Private kernelSize() As Integer
		Private stride() As Integer
		Private padding() As Integer
		Private dilation() As Integer
		Private inputChannels As Integer
		Private channels As Integer

		Private hasBias As Boolean

		Private capsules As Integer
		Private capsuleDimensions As Integer

		Private convolutionMode As ConvolutionMode = ConvolutionMode.Truncate

		Private useRelu As Boolean = False
		Private leak As Double = 0

		Private Const WEIGHT_PARAM As String = "weight"
		Private Const BIAS_PARAM As String = "bias"

		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)

			Me.kernelSize = builder.kernelSize_Conflict
			Me.stride = builder.stride_Conflict
			Me.padding = builder.padding_Conflict
			Me.dilation = builder.dilation_Conflict
			Me.channels = builder.channels_Conflict
			Me.hasBias = builder.hasBias_Conflict
			Me.capsules = builder.capsules_Conflict
			Me.capsuleDimensions = builder.capsuleDimensions_Conflict
			Me.convolutionMode = builder.convolutionMode_Conflict
			Me.useRelu = builder.useRelu_Conflict
			Me.leak = builder.leak

			If capsuleDimensions <= 0 OrElse channels <= 0 Then
				Throw New System.ArgumentException("Invalid configuration for Primary Capsules (layer name = """ & layerName & """):" & " capsuleDimensions and channels must be > 0.  Got: " & capsuleDimensions & ", " & channels)
			End If

			If capsules < 0 Then
				Throw New System.ArgumentException("Invalid configuration for Capsule Layer (layer name = """ & layerName & """):" & " capsules must be >= 0 if set.  Got: " & capsules)
			End If

		End Sub

		Public Overrides Function defineLayer(ByVal SD As SameDiff, ByVal input As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
			Dim conf As Conv2DConfig = Conv2DConfig.builder().kH(kernelSize(0)).kW(kernelSize(1)).sH(stride(0)).sW(stride(1)).pH(padding(0)).pW(padding(1)).dH(dilation(0)).dW(dilation(1)).isSameMode(convolutionMode = ConvolutionMode.Same).build()

			Dim conved As SDVariable

			If hasBias Then
				conved = SD.cnn_Conflict.conv2d(input, paramTable(WEIGHT_PARAM), paramTable(BIAS_PARAM), conf)
			Else
				conved = SD.cnn_Conflict.conv2d(input, paramTable(WEIGHT_PARAM), conf)
			End If

			If useRelu Then
				If leak = 0 Then
					conved = SD.nn_Conflict.relu(conved, 0)
				Else
					conved = SD.nn_Conflict.leakyRelu(conved, leak)
				End If
			End If

			Dim reshaped As SDVariable = conved.reshape(-1, capsules, capsuleDimensions)
			Return CapsuleUtils.squash(SD, reshaped, 2)
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()
			params.addWeightParam(WEIGHT_PARAM, kernelSize(0), kernelSize(1), inputChannels, capsuleDimensions * channels)

			If hasBias Then
				params.addBiasParam(BIAS_PARAM, capsuleDimensions * channels)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
					If BIAS_PARAM.Equals(e.Key) Then
						e.Value.assign(0)
					ElseIf WEIGHT_PARAM.Equals(e.Key) Then
						Dim fanIn As Double = inputChannels * kernelSize(0) * kernelSize(1)
						Dim fanOut As Double = capsuleDimensions * channels * kernelSize(0) * kernelSize(1) / (CDbl(stride(0)) * stride(1))
						WeightInitUtil.initWeights(fanIn, fanOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					End If
				Next e
			End Using
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Primary Capsules layer (layer name = """ & layerName & """): expect CNN input.  Got: " & inputType)
			End If

			If capsules > 0 Then
				Return InputType.recurrent(capsules, capsuleDimensions)
			Else

				Dim [out] As InputType.InputTypeConvolutional = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, capsuleDimensions * channels, -1, LayerName, GetType(PrimaryCapsules)), InputType.InputTypeConvolutional)

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return InputType.recurrent(CInt(Math.Truncate([out].getChannels() * [out].getHeight() * [out].getWidth() / capsuleDimensions)), capsuleDimensions)
			End If
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Primary Capsules layer (layer name = """ & layerName & """): expect CNN input.  Got: " & inputType)
			End If

			Dim ci As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

			Me.inputChannels = CInt(Math.Truncate(ci.getChannels()))

			If capsules <= 0 OrElse override Then

				Dim [out] As InputType.InputTypeConvolutional = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, capsuleDimensions * channels, -1, LayerName, GetType(PrimaryCapsules)), InputType.InputTypeConvolutional)

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Me.capsules = CInt(Math.Truncate([out].getChannels() * [out].getHeight() * [out].getWidth() / capsuleDimensions))
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer.Builder<Builder>
		Public Class Builder
			Inherits SameDiffLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] kernelSize = new int[]{9, 9};
'JAVA TO VB CONVERTER NOTE: The field kernelSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend kernelSize_Conflict() As Integer = {9, 9}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] stride = new int[]{2, 2};
'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stride_Conflict() As Integer = {2, 2}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] padding = new int[]{0, 0};
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padding_Conflict() As Integer = {0, 0}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] dilation = new int[]{1, 1};
'JAVA TO VB CONVERTER NOTE: The field dilation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dilation_Conflict() As Integer = {1, 1}

'JAVA TO VB CONVERTER NOTE: The field channels was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend channels_Conflict As Integer = 32

'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

'JAVA TO VB CONVERTER NOTE: The field capsules was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend capsules_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field capsuleDimensions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend capsuleDimensions_Conflict As Integer

'JAVA TO VB CONVERTER NOTE: The field convolutionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend convolutionMode_Conflict As ConvolutionMode = ConvolutionMode.Truncate

'JAVA TO VB CONVERTER NOTE: The field useRelu was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend useRelu_Conflict As Boolean = False
			Friend leak As Double = 0


			Public Overridable WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize_Conflict = ValidationUtils.validate2NonNegative(kernelSize, True, "kernelSize")
				End Set
			End Property

			Public Overridable WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate2NonNegative(stride, True, "stride")
				End Set
			End Property

			Public Overridable WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate2NonNegative(padding, True, "padding")
				End Set
			End Property

			Public Overridable WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate2NonNegative(dilation, True, "dilation")
				End Set
			End Property


			Public Sub New(ByVal capsuleDimensions As Integer, ByVal channels As Integer, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode)
				Me.capsuleDimensions_Conflict = capsuleDimensions
				Me.channels_Conflict = channels
				Me.KernelSize = kernelSize
				Me.Stride = stride
				Me.Padding = padding
				Me.Dilation = dilation
				Me.convolutionMode_Conflict = convolutionMode
			End Sub

			Public Sub New(ByVal capsuleDimensions As Integer, ByVal channels As Integer, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer)
				Me.New(capsuleDimensions, channels, kernelSize, stride, padding, dilation, ConvolutionMode.Truncate)
			End Sub

			Public Sub New(ByVal capsuleDimensions As Integer, ByVal channels As Integer, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
				Me.New(capsuleDimensions, channels, kernelSize, stride, padding, New Integer(){1, 1}, ConvolutionMode.Truncate)
			End Sub

			Public Sub New(ByVal capsuleDimensions As Integer, ByVal channels As Integer, ByVal kernelSize() As Integer, ByVal stride() As Integer)
				Me.New(capsuleDimensions, channels, kernelSize, stride, New Integer(){0, 0}, New Integer(){1, 1}, ConvolutionMode.Truncate)
			End Sub

			Public Sub New(ByVal capsuleDimensions As Integer, ByVal channels As Integer, ByVal kernelSize() As Integer)
				Me.New(capsuleDimensions, channels, kernelSize, New Integer(){2, 2}, New Integer(){0, 0}, New Integer(){1, 1}, ConvolutionMode.Truncate)
			End Sub

			Public Sub New(ByVal capsuleDimensions As Integer, ByVal channels As Integer)
				Me.New(capsuleDimensions, channels, New Integer(){9, 9}, New Integer(){2, 2}, New Integer(){0, 0}, New Integer(){1, 1}, ConvolutionMode.Truncate)
			End Sub

			''' <summary>
			''' Sets the kernel size of the 2d convolution
			''' </summary>
			''' <seealso cref= ConvolutionLayer.Builder#kernelSize(int...) </seealso>
			''' <param name="kernelSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function kernelSize(ParamArray ByVal kernelSize_Conflict() As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the stride of the 2d convolution
			''' </summary>
			''' <seealso cref= ConvolutionLayer.Builder#stride(int...) </seealso>
			''' <param name="stride">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function stride(ParamArray ByVal stride_Conflict() As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the padding of the 2d convolution
			''' </summary>
			''' <seealso cref= ConvolutionLayer.Builder#padding(int...) </seealso>
			''' <param name="padding">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padding(ParamArray ByVal padding_Conflict() As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the dilation of the 2d convolution
			''' </summary>
			''' <seealso cref= ConvolutionLayer.Builder#dilation(int...) </seealso>
			''' <param name="dilation">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter dilation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dilation(ParamArray ByVal dilation_Conflict() As Integer) As Builder
				Me.Dilation = dilation_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the number of channels to use in the 2d convolution.
			''' 
			''' Note that the actual number of channels is channels * capsuleDimensions
			''' 
			''' Does the same thing as nOut()
			''' </summary>
			''' <param name="channels">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter channels was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function channels(ByVal channels_Conflict As Integer) As Builder
				Me.channels_Conflict = channels_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the number of channels to use in the 2d convolution.
			''' 
			''' Note that the actual number of channels is channels * capsuleDimensions
			''' 
			''' Does the same thing as channels()
			''' </summary>
			''' <param name="nOut">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Return channels(nOut_Conflict)
			End Function

			''' <summary>
			''' Sets the number of dimensions to use in the capsules. </summary>
			''' <param name="capsuleDimensions">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter capsuleDimensions was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function capsuleDimensions(ByVal capsuleDimensions_Conflict As Integer) As Builder
				Me.capsuleDimensions_Conflict = capsuleDimensions_Conflict
				Return Me
			End Function

			''' <summary>
			''' Usually inferred automatically. </summary>
			''' <param name="capsules">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter capsules was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function capsules(ByVal capsules_Conflict As Integer) As Builder
				Me.capsules_Conflict = capsules_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.hasBias_Conflict = hasBias_Conflict
				Return Me
			End Function

			''' <summary>
			''' The convolution mode to use in the 2d convolution </summary>
			''' <param name="convolutionMode">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter convolutionMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function convolutionMode(ByVal convolutionMode_Conflict As ConvolutionMode) As Builder
				Me.convolutionMode_Conflict = convolutionMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' Whether to use a ReLU activation on the 2d convolution </summary>
			''' <param name="useRelu">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter useRelu was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function useReLU(ByVal useRelu_Conflict As Boolean) As Builder
				Me.useRelu_Conflict = useRelu_Conflict
				Return Me
			End Function

			''' <summary>
			''' Use a ReLU activation on the 2d convolution
			''' @return
			''' </summary>
			Public Overridable Function useReLU() As Builder
				Return useReLU(True)
			End Function

			''' <summary>
			''' Use a LeakyReLU activation on the 2d convolution </summary>
			''' <param name="leak"> the alpha value for the LeakyReLU activation.
			''' @return </param>
			Public Overridable Function useLeakyReLU(ByVal leak As Double) As Builder
				Me.useRelu_Conflict = True
				Me.leak = leak
				Return Me
			End Function

			Public Overrides Function build(Of E As Layer)() As E
				Return CType(New PrimaryCapsules(Me), E)
			End Function
		End Class
	End Class

End Namespace