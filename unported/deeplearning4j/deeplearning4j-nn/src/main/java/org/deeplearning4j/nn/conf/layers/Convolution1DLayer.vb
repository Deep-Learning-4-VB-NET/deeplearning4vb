Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports org.deeplearning4j.nn.conf
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Convolution1DLayer extends ConvolutionLayer
	<Serializable>
	Public Class Convolution1DLayer
		Inherits ConvolutionLayer

		Private rnnDataFormat As RNNFormat = RNNFormat.NCW
	'    
	'    //TODO: We will eventually want to NOT subclass off of ConvolutionLayer.
	'    //Currently, we just subclass off the ConvolutionLayer and hard code the "width" dimension to 1
	'     * This approach treats a multivariate time series with L timesteps and
	'     * P variables as an L x 1 x P image (L rows high, 1 column wide, P
	'     * channels deep). The kernel should be H<L pixels high and W=1 pixels
	'     * wide.
	'     

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			initializeConstraints(builder)
			Me.rnnDataFormat = builder.rnnDataFormat_Conflict
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			LayerValidation.assertNInNOutSet("Convolution1DLayer", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.convolution.Convolution1DLayer(conf, networkDataType)
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
				Throw New System.InvalidOperationException("Invalid input for 1D CNN layer (layer index = " & layerIndex & ", layer name = """ & getLayerName() & """): expect RNN input type with size > 0. Got: " & inputType)
			End If
			Dim it As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim inputTsLength As Long = it.getTimeSeriesLength()
			Dim outLength As Long
			If inputTsLength < 0 Then
				'Probably: user did InputType.recurrent(x) without specifying sequence length
				outLength = -1
			Else
				outLength = Convolution1DUtils.getOutputSize(inputTsLength, kernelSize(0), stride(0), padding(0), convolutionMode, dilation(0))
			End If

			Return InputType.recurrent(nOut, outLength, rnnDataFormat)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN AndAlso inputType.getType() <> InputType.Type.FF Then
				Throw New System.InvalidOperationException("Invalid input for 1D CNN layer (layer name = """ & getLayerName() & """): expect RNN input type with size > 0 or feed forward. Got: " & inputType)
			End If

			If inputType.getType() = InputType.Type.RNN Then
				Dim r As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				If nIn <= 0 OrElse override Then
					Me.nIn = r.getSize()
				End If
				If Me.rnnDataFormat = Nothing OrElse override Then
					Me.rnnDataFormat = r.getFormat()
				End If

				 If Me.cnn2dDataFormat = Nothing OrElse override Then
					Me.cnn2dDataFormat = If(rnnDataFormat = RNNFormat.NCW, CNN2DFormat.NCHW, CNN2DFormat.NHWC)
				 End If
			ElseIf inputType.getType() = InputType.Type.FF Then
				Dim r As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
				If nIn <= 0 OrElse override Then
					Me.nIn = r.getSize()
				End If
				If Me.rnnDataFormat = Nothing OrElse override Then
					Dim dataFormat As DataFormat = r.getTimeDistributedFormat()
					If TypeOf dataFormat Is CNN2DFormat Then
						Dim cnn2DFormat As CNN2DFormat = DirectCast(dataFormat, CNN2DFormat)
						Me.rnnDataFormat = If(cnn2DFormat = CNN2DFormat.NCHW, RNNFormat.NCW, RNNFormat.NWC)
						Me.cnn2dDataFormat = cnn2DFormat

					ElseIf TypeOf dataFormat Is RNNFormat Then
						Dim rnnFormat As RNNFormat = DirectCast(dataFormat, RNNFormat)
						Me.rnnDataFormat = rnnFormat
					End If

				End If

				If Me.cnn2dDataFormat = Nothing OrElse override Then
					Me.cnn2dDataFormat = If(rnnDataFormat = RNNFormat.NCW, CNN2DFormat.NCHW, CNN2DFormat.NHWC)
				End If

			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Convolution1D layer (layer name=""" & getLayerName() & """): input is null")
			End If

			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, rnnDataFormat,getLayerName())
		End Function

		Public Class Builder
			Inherits BaseConvBuilder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field rnnDataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend rnnDataFormat_Conflict As RNNFormat = RNNFormat.NCW

			Public Sub New()
				Me.New(0, 1, 0)
				Me.KernelSize = DirectCast(Nothing, Integer())
			End Sub

			Protected Friend Overrides Function allowCausal() As Boolean
				Return True
			End Function


'JAVA TO VB CONVERTER NOTE: The parameter rnnDataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function rnnDataFormat(ByVal rnnDataFormat_Conflict As RNNFormat) As Builder
				Me.rnnDataFormat_Conflict = rnnDataFormat_Conflict
				Return Me
			End Function
			''' <param name="kernelSize"> Kernel size </param>
			''' <param name="stride"> Stride </param>
			Public Sub New(ByVal kernelSize As Integer, ByVal stride As Integer)
				Me.New(kernelSize, stride, 0)
			End Sub

			''' <summary>
			''' Constructor with specified kernel size, stride of 1, padding of 0
			''' </summary>
			''' <param name="kernelSize"> Kernel size </param>
			Public Sub New(ByVal kernelSize As Integer)
				Me.New(kernelSize, 1, 0)
			End Sub

			''' <param name="kernelSize"> Kernel size </param>
			''' <param name="stride"> Stride </param>
			''' <param name="padding"> Padding </param>
			Public Sub New(ByVal kernelSize As Integer, ByVal stride As Integer, ByVal padding As Integer)
				Me.kernelSize_Conflict = New Integer() {1, 1}
				Me.stride_Conflict = New Integer() {1, 1}
				Me.padding_Conflict = New Integer() {0, 0}

				Me.KernelSize = kernelSize
				Me.Stride = stride
				Me.Padding = padding
			End Sub

			''' <summary>
			''' Size of the convolution
			''' </summary>
			''' <param name="kernelSize"> the length of the kernel </param>
'JAVA TO VB CONVERTER NOTE: The parameter kernelSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Overloads Function kernelSize(ByVal kernelSize_Conflict As Integer) As Builder
				Me.KernelSize = kernelSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Stride for the convolution. Must be > 0
			''' </summary>
			''' <param name="stride"> Stride </param>
'JAVA TO VB CONVERTER NOTE: The parameter stride was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Overloads Function stride(ByVal stride_Conflict As Integer) As Builder
				Me.Stride = stride_Conflict
				Return Me
			End Function

			''' <summary>
			''' Padding value for the convolution. Not used with <seealso cref="org.deeplearning4j.nn.conf.ConvolutionMode.Same"/>
			''' </summary>
			''' <param name="padding"> Padding value </param>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Overloads Function padding(ByVal padding_Conflict As Integer) As Builder
				Me.Padding = padding_Conflict
				Return Me
			End Function

			Public Overrides WriteOnly Property KernelSize As Integer()
				Set(ByVal kernelSize() As Integer)
    
					If kernelSize Is Nothing Then
						Me.kernelSize_Conflict = Nothing
						Return
					End If
    
					If Me.kernelSize_Conflict Is Nothing Then
						Me.kernelSize_Conflict = New Integer() {1, 1}
					End If
    
					Me.kernelSize_Conflict(0) = ValidationUtils.validate1NonNegative(kernelSize, "kernelSize")(0)
				End Set
			End Property

			Public Overrides WriteOnly Property Stride As Integer()
				Set(ByVal stride() As Integer)
    
					If stride Is Nothing Then
						Me.stride_Conflict = Nothing
						Return
					End If
    
					If Me.stride_Conflict Is Nothing Then
						Me.stride_Conflict = New Integer() {1, 1}
					End If
    
					Me.stride_Conflict(0) = ValidationUtils.validate1NonNegative(stride, "stride")(0)
				End Set
			End Property

			Public Overrides WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
    
					If padding Is Nothing Then
						Me.padding_Conflict = Nothing
						Return
					End If
    
					If Me.padding_Conflict Is Nothing Then
						Me.padding_Conflict = New Integer() {0, 0}
					End If
    
					Me.padding_Conflict(0) = ValidationUtils.validate1NonNegative(padding, "padding")(0)
				End Set
			End Property

			Public Overrides WriteOnly Property Dilation As Integer()
				Set(ByVal dilation() As Integer)
    
					If dilation Is Nothing Then
						Me.dilation_Conflict = Nothing
						Return
					End If
    
					If Me.dilation_Conflict Is Nothing Then
						Me.dilation_Conflict = New Integer() {1, 1}
					End If
    
					Me.dilation_Conflict(0) = ValidationUtils.validate1NonNegative(dilation, "dilation")(0)
				End Set
			End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Convolution1DLayer build()
			Public Overrides Function build() As Convolution1DLayer
				ConvolutionUtils.validateConvolutionModePadding(convolutionMode_Conflict, padding_Conflict)
				ConvolutionUtils.validateCnnKernelStridePadding(kernelSize_Conflict, stride_Conflict, padding_Conflict)

				Return New Convolution1DLayer(Me)
			End Function
		End Class
	End Class

End Namespace