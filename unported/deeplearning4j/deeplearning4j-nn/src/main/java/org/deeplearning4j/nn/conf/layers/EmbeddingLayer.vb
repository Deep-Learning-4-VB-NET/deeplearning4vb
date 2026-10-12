Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class EmbeddingLayer extends FeedForwardLayer
	<Serializable>
	Public Class EmbeddingLayer
		Inherits FeedForwardLayer

'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasBias_Conflict As Boolean = True 'Default for pre-0.9.2 implementations

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hasBias_Conflict = builder.hasBias_Conflict
			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.feedforward.embedding.EmbeddingLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmbeddingLayerParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'Basically a dense layer, but no dropout is possible here, and no epsilons
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim actElementsPerEx As val = outputType.arrayElementsPerExample()
			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			'Embedding layer does not use caching.
			'Inference: no working memory - just activations (pullRows)
			'Training: preout op, the only in-place ops on epsilon (from layer above) + assign ops

			Return (New LayerMemoryReport.Builder(layerName, GetType(EmbeddingLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, 0, actElementsPerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overridable Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

			''' <summary>
			''' If true: include bias parameters in the layer. False (default): no bias.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = False

			Public Sub New()
				'Default to Identity activation - i.e., don't inherit.
				'For example, if user sets ReLU as global default, they very likely don't intend to use it for Embedding layer also
				Me.activationFn = New ActivationIdentity()
			End Sub


			''' <summary>
			''' If true: include bias parameters in the layer. False (default): no bias.
			''' </summary>
			''' <param name="hasBias"> If true: include bias parameters in this layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.hasBias_Conflict = hasBias_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function weightInit(ByVal weightInit_Conflict As IWeightInit) As Builder
				If TypeOf weightInit_Conflict Is WeightInitEmbedding Then
					Dim shape() As Long = DirectCast(weightInit_Conflict, WeightInitEmbedding).shape()
					nIn(shape(0))
					nOut(shape(1))
				End If
				Return MyBase.weightInit(weightInit_Conflict)
			End Function

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
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public EmbeddingLayer build()
			Public Overrides Function build() As EmbeddingLayer
				Return New EmbeddingLayer(Me)
			End Function
		End Class
	End Class

End Namespace