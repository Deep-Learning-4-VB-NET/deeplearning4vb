Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Deconvolution2DLayer = org.deeplearning4j.nn.layers.convolution.Deconvolution2DLayer
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Deconvolution2D extends ConvolutionLayer
	<Serializable>
	Public Class Deconvolution2D
		Inherits ConvolutionLayer

		''' <summary>
		''' Deconvolution2D layer nIn in the input layer is the number of channels nOut is the number of filters to be used
		''' in the net or in other words the channels The builder specifies the filter/kernel size, the stride and padding
		''' The pooling layer takes the kernel size
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: protected Deconvolution2D(BaseConvBuilder<?> builder)
		Protected Friend Sub New(ByVal builder As BaseConvBuilder(Of T1))
			MyBase.New(builder)
			initializeConstraints(builder)
			If TypeOf builder Is Builder Then
				Me.cnn2dDataFormat = DirectCast(builder, Builder).format
			End If
		End Sub

		Public Overrides Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overrides Function clone() As Deconvolution2D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Deconvolution2D = CType(MyBase.clone(), Deconvolution2D)
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
			LayerValidation.assertNInNOutSet("Deconvolution2D", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New Deconvolution2DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return DeconvolutionParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Convolution layer (layer name=""" & getLayerName() & """): Expected CNN input, got " & inputType)
			End If

			Return InputTypeUtil.getOutputTypeDeconvLayer(inputType, kernelSize, stride, padding, dilation, convolutionMode, nOut, layerIndex, getLayerName(), GetType(Deconvolution2DLayer))
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

			Friend format As CNN2DFormat = CNN2DFormat.NCHW

			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.format = format
				Return Me
			End Function

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
					Me.kernelSize_Conflict = ValidationUtils.validate2NonNegative(kernelSize, False, "kernelSize")
				End Set
			End Property

			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
					Me.stride_Conflict = ValidationUtils.validate2NonNegative(stride, False,"stride")
				End Set
			End Property

			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate2NonNegative(padding, False, "padding")
				End Set
			End Property

			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
					Me.dilation_Conflict = ValidationUtils.validate2NonNegative(dilation, False,"dilation")
				End Set
			End Property

			Public Overrides Function build() As Deconvolution2D
				Return New Deconvolution2D(Me)
			End Function
		End Class

	End Class

End Namespace