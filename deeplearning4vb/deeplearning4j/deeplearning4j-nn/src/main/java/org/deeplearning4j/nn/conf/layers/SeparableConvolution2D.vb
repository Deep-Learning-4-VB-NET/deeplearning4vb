Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SeparableConvolution2DLayer = org.deeplearning4j.nn.layers.convolution.SeparableConvolution2DLayer
Imports SeparableConvolutionParamInitializer = org.deeplearning4j.nn.params.SeparableConvolutionParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class SeparableConvolution2D extends ConvolutionLayer
	<Serializable>
	Public Class SeparableConvolution2D
		Inherits ConvolutionLayer

		Friend depthMultiplier As Integer

		''' <summary>
		''' SeparableConvolution2D layer nIn in the input layer is the number of channels nOut is the number of filters to be
		''' used in the net or in other words the channels The builder specifies the filter/kernel size, the stride and
		''' padding The pooling layer takes the kernel size
		''' </summary>
		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hasBias_Conflict = builder.hasBias_Conflict
			Me.depthMultiplier = builder.depthMultiplier_Conflict
			Me.convolutionMode = builder.convolutionMode_Conflict
			Me.dilation = builder.dilation_Conflict
			If builder.kernelSize_Conflict.Length <> 2 Then
				Throw New System.ArgumentException("Kernel size of should be rows x columns (a 2d array)")
			End If
			Me.kernelSize = builder.kernelSize_Conflict
			If builder.stride_Conflict.Length <> 2 Then
				Throw New System.ArgumentException("Stride should include stride for rows and columns (a 2d array)")
			End If
			Me.stride = builder.stride_Conflict
			If builder.padding_Conflict.Length <> 2 Then
				Throw New System.ArgumentException("Padding should include padding for rows and columns (a 2d array)")
			End If
			Me.padding = builder.padding_Conflict
			Me.cudnnAlgoMode = builder.cudnnAlgoMode_Conflict
			Me.cudnnFwdAlgo = builder.cudnnFwdAlgo
			Me.cudnnBwdFilterAlgo = builder.cudnnBwdFilterAlgo
			Me.cudnnBwdDataAlgo = builder.cudnnBwdDataAlgo
			Me.cnn2dDataFormat = builder.dataFormat_Conflict


			initializeConstraints(builder)
		End Sub

		Protected Friend Overrides Sub initializeConstraints(Of T1)(ByVal builder As org.deeplearning4j.nn.conf.layers.Layer.Builder(Of T1))
			MyBase.initializeConstraints(builder)
			If DirectCast(builder, Builder).pointWiseConstraints IsNot Nothing Then
				If constraints Is Nothing Then
					constraints = New List(Of LayerConstraint)()
				End If
				For Each constraint As LayerConstraint In (DirectCast(builder, Builder)).pointWiseConstraints
					Dim clonedConstraint As LayerConstraint = constraint.clone()
					clonedConstraint.Params = Collections.singleton(SeparableConvolutionParamInitializer.POINT_WISE_WEIGHT_KEY)
					constraints.Add(clonedConstraint)
				Next constraint
			End If
		End Sub

		Public Overrides Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overrides Function clone() As SeparableConvolution2D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As SeparableConvolution2D = CType(MyBase.clone(), SeparableConvolution2D)
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
			LayerValidation.assertNInNOutSet("SeparableConvolution2D", LayerName, layerIndex, getNIn(), getNOut())

			Dim ret As New SeparableConvolution2DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf

			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return SeparableConvolutionParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Convolution layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If

			Dim format As CNN2DFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()

			Return InputTypeUtil.getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, nOut, layerIndex, LayerName, format, GetType(SeparableConvolution2DLayer))
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends BaseConvBuilder<Builder>
		Public Class Builder
			Inherits BaseConvBuilder(Of Builder)

			''' <summary>
			''' Set channels multiplier of channels-wise step in separable convolution
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field depthMultiplier was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend depthMultiplier_Conflict As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataFormat_Conflict As CNN2DFormat

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
				Me.dataFormat_Conflict = format
				Return Me
			End Function

			''' <summary>
			''' Set channels multiplier of channels-wise step in separable convolution
			''' </summary>
			''' <param name="depthMultiplier"> integer value, for each input map we get depthMultipler outputs in channels-wise
			''' step. </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter depthMultiplier was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function depthMultiplier(ByVal depthMultiplier_Conflict As Integer) As Builder
				Me.setDepthMultiplier(depthMultiplier_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to the point-wise convolution weight parameters of this layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' </summary>
			Protected Friend pointWiseConstraints As IList(Of LayerConstraint)

			''' <summary>
			''' Set constraints to be applied to the point-wise convolution weight parameters of this layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to the point-wise convolution parameters of this layer </param>
			Public Overridable Function constrainPointWise(ParamArray ByVal constraints() As LayerConstraint) As Builder
				Me.setPointWiseConstraints(java.util.Arrays.asList(constraints))
				Return Me
			End Function

			''' <summary>
			''' Size of the convolution rows/columns (height/width)
			''' </summary>
			''' <param name="kernelSize"> the height and width of the kernel </param>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function kernelSize(ParamArray ByVal kernelSize_Conflict() As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Stride of the convolution rows/columns (height/width)
			''' </summary>
			''' <param name="stride"> the stride of the kernel (in h/w dimensions) </param>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function stride(ParamArray ByVal stride_Conflict() As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Padding - rows/columns (height/width)
			''' </summary>
			''' <param name="padding"> the padding in h/w dimensions </param>
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public SeparableConvolution2D build()
			Public Overrides Function build() As SeparableConvolution2D
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding_Conflict)
				ConvolutionUtils.validateCnnKernelStridePadding(kernelSize_Conflict, stride_Conflict, padding_Conflict)

				Return New SeparableConvolution2D(Me)
			End Function
		End Class

	End Class

End Namespace