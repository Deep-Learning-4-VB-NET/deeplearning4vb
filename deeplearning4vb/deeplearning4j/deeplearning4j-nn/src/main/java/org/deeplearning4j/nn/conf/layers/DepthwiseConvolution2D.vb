Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DepthwiseConvolution2DLayer = org.deeplearning4j.nn.layers.convolution.DepthwiseConvolution2DLayer
Imports DepthwiseConvolutionParamInitializer = org.deeplearning4j.nn.params.DepthwiseConvolutionParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class DepthwiseConvolution2D extends ConvolutionLayer
	<Serializable>
	Public Class DepthwiseConvolution2D
		Inherits ConvolutionLayer

		Protected Friend depthMultiplier As Integer

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Preconditions.checkState(builder.depthMultiplier_Conflict > 0, "Depth multiplier must be > 0,  got %s", builder.depthMultiplier_Conflict)
			Me.depthMultiplier = builder.depthMultiplier_Conflict
			Me.nOut = Me.nIn * Me.depthMultiplier
			Me.cnn2dDataFormat = builder.cnn2DFormat

			initializeConstraints(builder)
		End Sub

		Public Overrides Function clone() As DepthwiseConvolution2D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As DepthwiseConvolution2D = CType(MyBase.clone(), DepthwiseConvolution2D)
			clone_Conflict.depthMultiplier = depthMultiplier
			Return clone_Conflict
		End Function


		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("DepthwiseConvolution2D", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New DepthwiseConvolution2DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf

			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return DepthwiseConvolutionParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for  depth-wise convolution layer (layer name=""" & getLayerName() & """): Expected CNN input, got " & inputType)
			End If

			Return InputTypeUtil.getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, nOut, layerIndex, getLayerName(), cnn2dDataFormat, GetType(DepthwiseConvolution2DLayer))
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			MyBase.setNIn(inputType, override)

			If nOut = 0 OrElse override Then
				nOut = Me.nIn * Me.depthMultiplier
			End If
			Me.cnn2dDataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends BaseConvBuilder<Builder>
		Public Class Builder
			Inherits BaseConvBuilder(Of Builder)

			''' <summary>
			''' Set channels multiplier for depth-wise convolution
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field depthMultiplier was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend depthMultiplier_Conflict As Integer = 1
			Protected Friend cnn2DFormat As CNN2DFormat = CNN2DFormat.NCHW


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

			Protected Friend Overrides Function allowCausal() As Boolean
				'Causal convolution - allowed for 1D only
				Return False
			End Function

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.cnn2DFormat = format
				Return Me
			End Function

			''' <summary>
			''' Set channels multiplier for depth-wise convolution
			''' </summary>
			''' <param name="depthMultiplier"> integer value, for each input map we get depthMultiplier outputs in channels-wise
			''' step. </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter depthMultiplier was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function depthMultiplier(ByVal depthMultiplier_Conflict As Integer) As Builder
				Me.setDepthMultiplier(depthMultiplier_Conflict)
				Return Me
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

			''' <summary>
			''' Stride of the convolution in rows/columns (height/width) dimensions
			''' </summary>
			''' <param name="stride"> Stride of the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function stride(ParamArray ByVal stride_Conflict() As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Padding of the convolution in rows/columns (height/width) dimensions
			''' </summary>
			''' <param name="padding"> Padding of the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function padding(ParamArray ByVal padding_Conflict() As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function

			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
					Me.kernelSize_Conflict = ValidationUtils.validate2NonNegative(kernelSize, False, "kernelSize")
				End Set
			End Property

			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate2NonNegative(stride, False, "stride")
				End Set
			End Property

			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate2NonNegative(padding, False, "padding")
				End Set
			End Property

			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate2NonNegative(dilation, False, "dilation")
				End Set
			End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public DepthwiseConvolution2D build()
			Public Overrides Function build() As DepthwiseConvolution2D
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding_Conflict)
				ConvolutionUtils.validateCnnKernelStridePadding(kernelSize_Conflict, stride_Conflict, padding_Conflict)

				Return New DepthwiseConvolution2D(Me)
			End Function
		End Class

	End Class

End Namespace