Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmbeddingLayerParamInitializer = org.deeplearning4j.nn.params.EmbeddingLayerParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports ArrayEmbeddingInitializer = org.deeplearning4j.nn.weights.embeddings.ArrayEmbeddingInitializer
Imports EmbeddingInitializer = org.deeplearning4j.nn.weights.embeddings.EmbeddingInitializer
Imports WeightInitEmbedding = org.deeplearning4j.nn.weights.embeddings.WeightInitEmbedding
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class EmbeddingSequenceLayer extends FeedForwardLayer
	<Serializable>
	Public Class EmbeddingSequenceLayer
		Inherits FeedForwardLayer

		Private inputLength As Integer = 1 ' By default only use one index to embed
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasBias_Conflict As Boolean = False
		Private inferInputLength As Boolean = False ' use input length as provided by input data
		Private outputFormat As RNNFormat = RNNFormat.NCW 'Default value for older deserialized models

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hasBias_Conflict = builder.hasBias_Conflict
			Me.inputLength = builder.inputLength_Conflict
			Me.inferInputLength = builder.inferInputLength_Conflict
			Me.outputFormat = builder.outputFormat
			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.feedforward.embedding.EmbeddingSequenceLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse (inputType.getType() <> InputType.Type.FF AndAlso inputType.getType() <> InputType.Type.RNN) Then
				Throw New System.InvalidOperationException("Invalid input for Embedding layer (layer index = " & layerIndex & ", layer name = """ & getLayerName() & """): expect FF/RNN input type. Got: " & inputType)
			End If
			Return InputType.recurrent(nOut, inputLength, outputFormat)
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmbeddingLayerParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim actElementsPerEx As val = outputType.arrayElementsPerExample()
			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			Return (New LayerMemoryReport.Builder(layerName, GetType(EmbeddingSequenceLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, 0, actElementsPerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overridable Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for layer (layer name = """ & getLayerName() & """): input type is null")
			End If

			If inputType.getType() = InputType.Type.RNN Then
				Return Nothing
			End If
			Return MyBase.getPreProcessorForInputType(inputType)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType.getType() = InputType.Type.RNN Then
				If nIn <= 0 OrElse override Then
					Dim f As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
					Me.nIn = f.getSize()
				End If
			ElseIf inputType.getType() = InputType.Type.FF Then
				If nIn <= 0 OrElse override Then
					Dim feedForward As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
					Me.nIn = feedForward.getSize()
					Me.inferInputLength = True
				End If

			Else
				MyBase.setNIn(inputType, override)
			End If

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

			Public Sub New()
				'Default to Identity activation - i.e., don't inherit.
				'For example, if user sets ReLU as global default, they very likely don't intend to use it for Embedding layer also
				Me.activationFn = New ActivationIdentity()
			End Sub

			''' <summary>
			''' If true: include bias parameters in the layer. False (default): no bias.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = False

			''' <summary>
			''' Set input sequence length for this embedding layer.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field inputLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputLength_Conflict As Integer = 1

			''' <summary>
			''' Set input sequence inference mode for embedding layer.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field inferInputLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inferInputLength_Conflict As Boolean = True

			Friend outputFormat As RNNFormat = RNNFormat.NCW 'Default value for older deserialized models

			Public Overridable Function outputDataFormat(ByVal format As RNNFormat) As Builder
				Me.outputFormat = format
				Return Me
			End Function

			''' <summary>
			''' If true: include bias parameters in the layer. False (default): no bias.
			''' </summary>
			''' <param name="hasBias"> If true: include bias parameters in this layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.setHasBias(hasBias_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set input sequence length for this embedding layer.
			''' </summary>
			''' <param name="inputLength"> input sequence length </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter inputLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inputLength(ByVal inputLength_Conflict As Integer) As Builder
				Me.setInputLength(inputLength_Conflict)
				Return Me
			End Function


			''' <summary>
			''' Set input sequence inference mode for embedding layer.
			''' </summary>
			''' <param name="inferInputLength"> whether to infer input length </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter inferInputLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inferInputLength(ByVal inferInputLength_Conflict As Boolean) As Builder
				Me.setInferInputLength(inferInputLength_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function weightInit(ByVal weightInit_Conflict As IWeightInit) As Builder
				Me.WeightInitFn = weightInit_Conflict
				Return Me
			End Function

			Public Overrides WriteOnly Property WeightInitFn As IWeightInit
				Set(ByVal weightInit As IWeightInit)
					If TypeOf weightInit Is WeightInitEmbedding Then
						Dim shape() As Long = DirectCast(weightInit, WeightInitEmbedding).shape()
						nIn(shape(0))
						nOut(shape(1))
					End If
					Me.weightInitFn = weightInit
				End Set
			End Property

			''' <summary>
			''' Initialize the embedding layer using the specified EmbeddingInitializer - such as a Word2Vec instance
			''' </summary>
			''' <param name="embeddingInitializer"> Source of the embedding layer weights </param>
			Public Overridable Overloads Function weightInit(ByVal embeddingInitializer As EmbeddingInitializer) As Builder
				Return weightInit(New WeightInitEmbedding(embeddingInitializer))
			End Function

			''' <summary>
			''' Initialize the embedding layer using values from the specified array. Note that the array should have shape
			''' [vocabSize, vectorSize]. After copying values from the array to initialize the network parameters, the input
			''' array will be discarded (so that, if necessary, it can be garbage collected)
			''' </summary>
			''' <param name="vectors"> Vectors to initialize the embedding layer with </param>
			Public Overridable Overloads Function weightInit(ByVal vectors As INDArray) As Builder
				Return weightInit(New ArrayEmbeddingInitializer(vectors))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public EmbeddingSequenceLayer build()
			Public Overrides Function build() As EmbeddingSequenceLayer
				Return New EmbeddingSequenceLayer(Me)
			End Function
		End Class
	End Class

End Namespace