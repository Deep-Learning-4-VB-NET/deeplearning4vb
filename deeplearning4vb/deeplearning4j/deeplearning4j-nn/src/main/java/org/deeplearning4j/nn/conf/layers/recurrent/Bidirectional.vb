Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BaseRecurrentLayer = org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports BidirectionalLayer = org.deeplearning4j.nn.layers.recurrent.BidirectionalLayer
Imports BidirectionalParamInitializer = org.deeplearning4j.nn.params.BidirectionalParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.deeplearning4j.nn.conf.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = true, exclude = {"initializer"}) @JsonIgnoreProperties({"initializer"}) public class Bidirectional extends org.deeplearning4j.nn.conf.layers.Layer
	<Serializable>
	Public Class Bidirectional
		Inherits Layer

		''' <summary>
		''' This Mode enumeration defines how the activations for the forward and backward networks should be combined.<br>
		''' ADD: out = forward + backward (elementwise addition)<br> MUL: out = forward * backward (elementwise
		''' multiplication)<br> AVERAGE: out = 0.5 * (forward + backward)<br> CONCAT: Concatenate the activations.<br> Where
		''' 'forward' is the activations for the forward RNN, and 'backward' is the activations for the backward RNN. In all
		''' cases except CONCAT, the output activations size is the same size as the standard RNN that is being wrapped by
		''' this layer. In the CONCAT case, the output activations size (dimension 1) is 2x larger than the standard RNN's
		''' activations array.
		''' </summary>
		Public Enum Mode
			ADD
			MUL
			AVERAGE
			CONCAT
		End Enum

		Private fwd As Layer
		Private bwd As Layer
		Private mode As Mode
'JAVA TO VB CONVERTER NOTE: The field initializer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private initializer_Conflict As BidirectionalParamInitializer

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
		End Sub

		''' <summary>
		''' Create a Bidirectional wrapper, with the default Mode (CONCAT) for the specified layer
		''' </summary>
		''' <param name="layer"> layer to wrap </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Bidirectional(@NonNull Layer layer)
		Public Sub New(ByVal layer As Layer)
			Me.New(Mode.CONCAT, layer)
		End Sub

		''' <summary>
		''' Create a Bidirectional wrapper for the specified layer
		''' </summary>
		''' <param name="mode"> Mode to use to combine activations. See <seealso cref="Mode"/> for details </param>
		''' <param name="layer"> layer to wrap </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Bidirectional(@NonNull Mode mode, @NonNull Layer layer)
		Public Sub New(ByVal mode As Mode, ByVal layer As Layer)
			If Not (TypeOf layer Is BaseRecurrentLayer OrElse TypeOf layer Is LastTimeStep OrElse TypeOf layer Is BaseWrapperLayer) Then
				Throw New System.ArgumentException("Cannot wrap a non-recurrent layer: " & "config must extend BaseRecurrentLayer or LastTimeStep " & "Got class: " & layer.GetType())
			End If
			Me.fwd = layer
			Me.bwd = layer.clone()
			Me.mode = mode
		End Sub

		Public Overridable ReadOnly Property NOut As Long
			Get
				If TypeOf Me.fwd Is LastTimeStep Then
					Return DirectCast(DirectCast(Me.fwd, LastTimeStep).Underlying, FeedForwardLayer).getNOut()
				Else
					Return DirectCast(Me.fwd, FeedForwardLayer).getNOut()
				End If
			End Get
		End Property

		Public Overridable ReadOnly Property NIn As Long
			Get
				If TypeOf Me.fwd Is LastTimeStep Then
					Return DirectCast(DirectCast(Me.fwd, LastTimeStep).Underlying, FeedForwardLayer).getNIn()
				Else
					Return DirectCast(Me.fwd, FeedForwardLayer).getNIn()
				End If
			End Get
		End Property

		Public Overridable ReadOnly Property RNNDataFormat As RNNFormat
			Get
				Return TimeSeriesUtils.getFormatFromRnnLayer(fwd)
			End Get
		End Property

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim c1 As NeuralNetConfiguration = conf.clone()
			Dim c2 As NeuralNetConfiguration = conf.clone()
			c1.setLayer(fwd)
			c2.setLayer(bwd)

			Dim n As Long = layerParamsView.length() \ 2
			Dim fp As INDArray = layerParamsView.get(interval(0,0,True), interval(0, n))
			Dim bp As INDArray = layerParamsView.get(interval(0,0,True), interval(n, 2 * n))
			Dim f As org.deeplearning4j.nn.api.Layer = fwd.instantiate(c1, trainingListeners, layerIndex, fp, initializeParams, networkDataType)

			Dim b As org.deeplearning4j.nn.api.Layer = bwd.instantiate(c2, trainingListeners, layerIndex, bp, initializeParams, networkDataType)

			Dim ret As New BidirectionalLayer(conf, f, b, layerParamsView)
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf

			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			If initializer_Conflict Is Nothing Then
				initializer_Conflict = New BidirectionalParamInitializer(Me)
			End If
			Return initializer_Conflict
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Dim outOrig As InputType = fwd.getOutputType(layerIndex, inputType)

			If TypeOf fwd Is LastTimeStep Then
				Dim ff As InputType.InputTypeFeedForward = DirectCast(outOrig, InputType.InputTypeFeedForward)
				If mode = Mode.CONCAT Then
					Return InputType.feedForward(2 * ff.getSize())
				Else
					Return ff
				End If
			Else
				Dim r As InputType.InputTypeRecurrent = DirectCast(outOrig, InputType.InputTypeRecurrent)
				If mode = Mode.CONCAT Then
					Return InputType.recurrent(2 * r.getSize(), RNNDataFormat)
				Else
					Return r
				End If
			End If
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			fwd.setNIn(inputType, override)
			bwd.setNIn(inputType, override)
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return fwd.getPreProcessorForInputType(inputType)
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'Strip forward/backward prefix from param name
			Return fwd.getRegularizationByParam(paramName.Substring(1))
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return fwd.isPretrainParam(paramName.Substring(1))
		End Function

		''' <summary>
		''' Get the updater for the given parameter. Typically the same updater will be used for all updaters, but this is
		''' not necessarily the case
		''' </summary>
		''' <param name="paramName"> Parameter name </param>
		''' <returns> IUpdater for the parameter </returns>
		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			Dim [sub] As String = paramName.Substring(1)
			Return fwd.getUpdaterByParam([sub])
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return fwd.GradientNormalization
			End Get
		End Property

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return fwd.GradientNormalizationThreshold
			End Get
		End Property

		Public Overrides WriteOnly Property LayerName As String
			Set(ByVal layerName As String)
				Me.layerName = layerName
				fwd.setLayerName(layerName)
				bwd.setLayerName(layerName)
			End Set
		End Property

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim lmr As LayerMemoryReport = fwd.getMemoryReport(inputType)
			lmr.scale(2) 'Double all memory use
			Return lmr
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field mode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend mode_Conflict As Mode
'JAVA TO VB CONVERTER NOTE: The field layer was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend layer_Conflict As Layer

			Public Overridable WriteOnly Property Layer As Layer
				Set(ByVal layer As Layer)
					rnnLayer(layer)
				End Set
			End Property

'JAVA TO VB CONVERTER NOTE: The parameter mode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function mode(ByVal mode_Conflict As Mode) As Builder
				Me.setMode(mode_Conflict)
				Return Me
			End Function

			Public Overridable Function rnnLayer(ByVal layer As Layer) As Builder
				If Not (TypeOf layer Is BaseRecurrentLayer OrElse TypeOf layer Is LastTimeStep OrElse TypeOf layer Is BaseWrapperLayer) Then
					Throw New System.ArgumentException("Cannot wrap a non-recurrent layer: " & "config must extend BaseRecurrentLayer or LastTimeStep " & "Got class: " & layer.GetType())
				End If
				Me.Layer = layer
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public Bidirectional build()
			Public Overridable Overloads Function build() As Bidirectional
				Return New Bidirectional(Me)
			End Function
		End Class
	End Class

End Namespace