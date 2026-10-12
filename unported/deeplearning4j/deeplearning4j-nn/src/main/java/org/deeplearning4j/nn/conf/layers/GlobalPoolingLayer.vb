Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class GlobalPoolingLayer extends NoParamLayer
	<Serializable>
	Public Class GlobalPoolingLayer
		Inherits NoParamLayer

		Private poolingType As PoolingType
		Private poolingDimensions() As Integer
		Private pnorm As Integer
		Private collapseDimensions As Boolean = True

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.poolingType = builder.poolingType_Conflict
			Me.poolingDimensions = builder.poolingDimensions_Conflict
			Me.collapseDimensions = builder.collapseDimensions_Conflict
			Me.pnorm = builder.pnorm_Conflict
			Me.layerName = builder.layerName
		End Sub

		Public Sub New()
			Me.New(PoolingType.MAX)
		End Sub

		Public Sub New(ByVal poolingType As PoolingType)
			Me.New((New Builder()).poolingType(poolingType))
		End Sub


		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.pooling.GlobalPoolingLayer(conf, networkDataType)
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

			Select Case inputType.getType()
				Case FF
					Throw New System.NotSupportedException("Global max pooling cannot be applied to feed-forward input type. Got input type = " & inputType)
				Case RNN
					Dim recurrent As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
					'Return 3d activations, with shape [minibatch, timeStepSize, 1]
					Return recurrent
				Case CNN
					Dim conv As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
					If collapseDimensions Then
						Return InputType.feedForward(conv.getChannels())
					Else
						Return InputType.convolutional(1, 1, conv.getChannels(), conv.getFormat())
					End If
				Case CNN3D
					Dim conv3d As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
					If collapseDimensions Then
						Return InputType.feedForward(conv3d.getChannels())
					Else
						Return InputType.convolutional3D(1, 1, 1, conv3d.getChannels())
					End If
				Case CNNFlat
					Dim convFlat As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
					If collapseDimensions Then
						Return InputType.feedForward(convFlat.getDepth())
					Else
						Return InputType.convolutional(1, 1, convFlat.getDepth())
					End If
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported input type: " & inputType)
			End Select
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType.getType() = InputType.Type.CNN Then
				Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				If c.getFormat() = CNN2DFormat.NCHW Then
					poolingDimensions = New Integer(){2, 3}
				Else
					poolingDimensions = New Integer(){1, 2}
				End If
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Select Case inputType.getType()
				''' <summary>
				''' Global pooling won't appear in the context of feed forward, but global pooling itself
				''' typically feeds forward a feed forward type. This converts that back to rnn.
				''' </summary>
				Case FF
					Dim feedForward As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
					If feedForward.getTimeDistributedFormat() IsNot Nothing AndAlso TypeOf feedForward.getTimeDistributedFormat() Is RNNFormat Then
						Dim rnnFormat As RNNFormat = CType(feedForward.getTimeDistributedFormat(), RNNFormat)
						Return New FeedForwardToRnnPreProcessor(rnnFormat)
					ElseIf feedForward.getTimeDistributedFormat() IsNot Nothing AndAlso TypeOf feedForward.getTimeDistributedFormat() Is CNN2DFormat Then
						Dim cnn2DFormat As CNN2DFormat = CType(feedForward.getTimeDistributedFormat(), CNN2DFormat)
						Select Case cnn2DFormat.innerEnumValue
							Case CNN2DFormat.InnerEnum.NCHW
								Return New FeedForwardToRnnPreProcessor(RNNFormat.NCW)
							Case CNN2DFormat.InnerEnum.NHWC
								Return New FeedForwardToRnnPreProcessor(RNNFormat.NWC)
						End Select

					Else
						Return New FeedForwardToRnnPreProcessor()
					End If

				Case RNN, CNN, CNN3D
					'No preprocessor required
					Return Nothing
				Case CNNFlat
					Dim cFlat As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
					Return New FeedForwardToCnnPreProcessor(cFlat.getHeight(), cFlat.getWidth(), cFlat.getDepth())
			End Select

			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("Global pooling layer does not contain parameters")
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim fwdTrainInferenceWorkingPerEx As Long = 0
			'Here: we'll assume we are doing 'full array' global pooling.
			'For max/avg/sum pooling, no working memory (GlobalPoolingLayer.activateHelperFullArray
			'But for pnorm, we have working memory
			If poolingType = PoolingType.PNORM Then
				'Dup the input array once before
				fwdTrainInferenceWorkingPerEx = inputType.arrayElementsPerExample()
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(GlobalPoolingLayer), inputType, outputType)).standardMemory(0, 0).workingMemory(0, fwdTrainInferenceWorkingPerEx, 0, fwdTrainInferenceWorkingPerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

			''' <summary>
			''' Pooling type for global pooling
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field poolingType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend poolingType_Conflict As PoolingType = PoolingType.MAX

			''' <summary>
			''' Pooling dimensions. Note: most of the time, this doesn't need to be set, and the defaults can be used.
			''' Default for RNN data: pooling dimension 2 (time). Default for CNN data: pooling dimensions 2,3 (height and
			''' width) Default for CNN3D data: pooling dimensions 2,3,4 (depth, height and width)
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field poolingDimensions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend poolingDimensions_Conflict() As Integer

			''' <summary>
			''' P-norm constant. Only used if using <seealso cref="PoolingType.PNORM"/> for the pooling type
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field pnorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend pnorm_Conflict As Integer = 2

			''' <summary>
			''' Whether to collapse dimensions when pooling or not. Usually you *do* want to do this. Default: true. If
			''' true:<br> - 3d (time series) input with shape [miniBatchSize, vectorSize, timeSeriesLength] -> 2d output
			''' [miniBatchSize, vectorSize]<br> - 4d (CNN) input with shape [miniBatchSize, channels, height, width] -> 2d
			''' output [miniBatchSize, channels]<br> - 5d (CNN3D) input with shape [miniBatchSize, channels, depth, height,
			''' width] -> 2d output [miniBatchSize, channels]<br>
			''' 
			''' 
			''' If false:<br> - 3d (time series) input with shape [miniBatchSize, vectorSize, timeSeriesLength] -> 3d output
			''' [miniBatchSize, vectorSize, 1]<br> - 4d (CNN) input with shape [miniBatchSize, channels, height, width] -> 2d
			''' output [miniBatchSize, channels, 1, 1]<br> - 5d (CNN3D) input with shape [miniBatchSize, channels, depth,
			''' height, width] -> 2d output [miniBatchSize, channels, 1, 1, 1]<br>
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field collapseDimensions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collapseDimensions_Conflict As Boolean = True

			Public Sub New()

			End Sub

			Public Sub New(ByVal poolingType As PoolingType)
				Me.setPoolingType(poolingType)
			End Sub

			''' <summary>
			''' Pooling dimensions. Note: most of the time, this doesn't need to be set, and the defaults can be used.
			''' Default for RNN data: pooling dimension 2 (time). Default for CNN data: pooling dimensions 2,3 (height and
			''' width) Default for CNN3D data: pooling dimensions 2,3,4 (depth, height and width)
			''' </summary>
			''' <param name="poolingDimensions"> Pooling dimensions to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter poolingDimensions was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function poolingDimensions(ParamArray ByVal poolingDimensions_Conflict() As Integer) As Builder
				Me.setPoolingDimensions(poolingDimensions_Conflict)
				Return Me
			End Function

			''' <param name="poolingType"> Pooling type for global pooling </param>
'JAVA TO VB CONVERTER NOTE: The parameter poolingType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function poolingType(ByVal poolingType_Conflict As PoolingType) As Builder
				Me.setPoolingType(poolingType_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Whether to collapse dimensions when pooling or not. Usually you *do* want to do this. Default: true. If
			''' true:<br> - 3d (time series) input with shape [miniBatchSize, vectorSize, timeSeriesLength] -> 2d output
			''' [miniBatchSize, vectorSize]<br> - 4d (CNN) input with shape [miniBatchSize, channels, height, width] -> 2d
			''' output [miniBatchSize, channels]<br> - 5d (CNN3D) input with shape [miniBatchSize, channels, depth, height,
			''' width] -> 2d output [miniBatchSize, channels]<br>
			''' 
			''' 
			''' If false:<br> - 3d (time series) input with shape [miniBatchSize, vectorSize, timeSeriesLength] -> 3d output
			''' [miniBatchSize, vectorSize, 1]<br> - 4d (CNN) input with shape [miniBatchSize, channels, height, width] -> 2d
			''' output [miniBatchSize, channels, 1, 1]<br> - 5d (CNN3D) input with shape [miniBatchSize, channels, depth,
			''' height, width] -> 2d output [miniBatchSize, channels, 1, 1, 1]<br>
			''' </summary>
			''' <param name="collapseDimensions"> Whether to collapse the dimensions or not </param>
'JAVA TO VB CONVERTER NOTE: The parameter collapseDimensions was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collapseDimensions(ByVal collapseDimensions_Conflict As Boolean) As Builder
				Me.setCollapseDimensions(collapseDimensions_Conflict)
				Return Me
			End Function

			''' <summary>
			''' P-norm constant. Only used if using <seealso cref="PoolingType.PNORM"/> for the pooling type
			''' </summary>
			''' <param name="pnorm"> P-norm constant </param>
'JAVA TO VB CONVERTER NOTE: The parameter pnorm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function pnorm(ByVal pnorm_Conflict As Integer) As Builder
				If pnorm_Conflict <= 0 Then
					Throw New System.ArgumentException("Invalid input: p-norm value must be greater than 0. Got: " & pnorm_Conflict)
				End If
				Me.Pnorm = pnorm_Conflict
				Return Me
			End Function

			Public Overridable WriteOnly Property Pnorm As Integer
				Set(ByVal pnorm As Integer)
					ValidationUtils.validateNonNegative(pnorm, "pnorm")
					Me.pnorm_Conflict = pnorm
				End Set
			End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public GlobalPoolingLayer build()
			Public Overridable Overloads Function build() As GlobalPoolingLayer
				Return New GlobalPoolingLayer(Me)
			End Function
		End Class
	End Class

End Namespace