Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports TruncatedNormalDistribution = org.deeplearning4j.nn.conf.distribution.TruncatedNormalDistribution
Imports L2NormalizeVertex = org.deeplearning4j.nn.conf.graph.L2NormalizeVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
Imports InceptionResNetHelper = org.deeplearning4j.zoo.model.helper.InceptionResNetHelper
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.zoo.model

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder public class InceptionResNetV1 extends org.deeplearning4j.zoo.ZooModel
	Public Class InceptionResNetV1
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 160, 160};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 160, 160}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.RmsProp(0.1, 0.96, 0.001);
		Private updater As IUpdater = New RmsProp(0.1, 0.96, 0.001)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private CacheMode cacheMode = CacheMode.NONE;
		Private cacheMode As CacheMode = CacheMode.NONE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private WorkspaceMode workspaceMode = WorkspaceMode.ENABLED;
		Private workspaceMode As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ConvolutionLayer.AlgoMode cudnnAlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST;
		Private cudnnAlgoMode As ConvolutionLayer.AlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST

		Private Sub New()
		End Sub

		Public Overrides Function pretrainedUrl(ByVal pretrainedType As PretrainedType) As String
			Return Nothing
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			Return 0L
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(ComputationGraph)
		End Function

		Public Overrides Function init() As ComputationGraph
			Dim embeddingSize As Integer = 128
			Dim graph As ComputationGraphConfiguration.GraphBuilder = graphBuilder("input1")

			graph.addInputs("input1").setInputTypes(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))).addLayer("bottleneck", (New DenseLayer.Builder()).nIn(5376).nOut(embeddingSize).build(), "avgpool").addVertex("embeddings", New L2NormalizeVertex(New Integer() {1}, 1e-10), "bottleneck").addLayer("outputLayer", (New CenterLossOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD).activation(Activation.SOFTMAX).alpha(0.9).lambda(1e-4).nIn(embeddingSize).nOut(numClasses).build(), "embeddings").setOutputs("outputLayer")

			Dim conf As ComputationGraphConfiguration = graph.build()
			Dim model As New ComputationGraph(conf)
			model.init()

			Return model
		End Function

		Public Overridable Function graphBuilder(ByVal input As String) As ComputationGraphConfiguration.GraphBuilder

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).activation(Activation.RELU).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(New TruncatedNormalDistribution(0.0, 0.5)).l2(5e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).convolutionMode(ConvolutionMode.Truncate).graphBuilder()


			graph.addLayer("stem-cnn1", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(inputShape_Conflict(0)).nOut(32).cudnnAlgoMode(cudnnAlgoMode).build(), input).addLayer("stem-batch1", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), "stem-cnn1").addLayer("stem-cnn2", (New ConvolutionLayer.Builder(New Integer() {3, 3})).nIn(32).nOut(32).cudnnAlgoMode(cudnnAlgoMode).build(), "stem-batch1").addLayer("stem-batch2", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), "stem-cnn2").addLayer("stem-cnn3", (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(32).nOut(64).cudnnAlgoMode(cudnnAlgoMode).build(), "stem-batch2").addLayer("stem-batch3", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(64).nOut(64).build(), "stem-cnn3").addLayer("stem-pool4", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2})).build(), "stem-batch3").addLayer("stem-cnn5", (New ConvolutionLayer.Builder(New Integer() {1, 1})).nIn(64).nOut(80).cudnnAlgoMode(cudnnAlgoMode).build(), "stem-pool4").addLayer("stem-batch5", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(80).nOut(80).build(), "stem-cnn5").addLayer("stem-cnn6", (New ConvolutionLayer.Builder(New Integer() {3, 3})).nIn(80).nOut(128).cudnnAlgoMode(cudnnAlgoMode).build(), "stem-batch5").addLayer("stem-batch6", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), "stem-cnn6").addLayer("stem-cnn7", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(128).nOut(192).cudnnAlgoMode(cudnnAlgoMode).build(), "stem-batch6").addLayer("stem-batch7", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), "stem-cnn7")


			' 5xInception-resnet-A
			InceptionResNetHelper.inceptionV1ResA(graph, "resnetA", 5, 0.17, "stem-batch7")


			' Reduction-A
			graph.addLayer("reduceA-cnn1", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(192).nOut(192).cudnnAlgoMode(cudnnAlgoMode).build(), "resnetA").addLayer("reduceA-batch1", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), "reduceA-cnn1").addLayer("reduceA-cnn2", (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(192).nOut(128).cudnnAlgoMode(cudnnAlgoMode).build(), "resnetA").addLayer("reduceA-batch2", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), "reduceA-cnn2").addLayer("reduceA-cnn3", (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(128).nOut(128).cudnnAlgoMode(cudnnAlgoMode).build(), "reduceA-batch2").addLayer("reduceA-batch3", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), "reduceA-cnn3").addLayer("reduceA-cnn4", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(128).nOut(192).cudnnAlgoMode(cudnnAlgoMode).build(), "reduceA-batch3").addLayer("reduceA-batch4", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), "reduceA-cnn4").addLayer("reduceA-pool5", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2})).build(), "resnetA").addVertex("reduceA", New MergeVertex(), "reduceA-batch1", "reduceA-batch4", "reduceA-pool5")


			' 10xInception-resnet-B
			InceptionResNetHelper.inceptionV1ResB(graph, "resnetB", 10, 0.10, "reduceA")


			' Reduction-B
			graph.addLayer("reduceB-pool1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2})).build(), "resnetB").addLayer("reduceB-cnn2", (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(576).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "resnetB").addLayer("reduceB-batch1", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn2").addLayer("reduceB-cnn3", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(256).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "reduceB-batch1").addLayer("reduceB-batch2", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn3").addLayer("reduceB-cnn4", (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(576).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "resnetB").addLayer("reduceB-batch3", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn4").addLayer("reduceB-cnn5", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(256).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "reduceB-batch3").addLayer("reduceB-batch4", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn5").addLayer("reduceB-cnn6", (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(576).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "resnetB").addLayer("reduceB-batch5", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn6").addLayer("reduceB-cnn7", (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(256).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "reduceB-batch5").addLayer("reduceB-batch6", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn7").addLayer("reduceB-cnn8", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(256).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "reduceB-batch6").addLayer("reduceB-batch7", (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(256).nOut(256).build(), "reduceB-cnn8").addVertex("reduceB", New MergeVertex(), "reduceB-pool1", "reduceB-batch2", "reduceB-batch4", "reduceB-batch7")


			' 10xInception-resnet-C
			InceptionResNetHelper.inceptionV1ResC(graph, "resnetC", 5, 0.20, "reduceB")

			' Average pooling
			graph.addLayer("avgpool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {1, 1})).build(), "resnetC")

			Return graph
		End Function

		Public Overrides Function metaData() As ModelMetaData
			Return New ModelMetaData(New Integer()() {inputShape_Conflict}, 1, ZooType.CNN)
		End Function

		Public Overrides WriteOnly Property InputShape As Integer()()
			Set(ByVal inputShape()() As Integer)
				Me.inputShape_Conflict = inputShape(0)
			End Set
		End Property

	End Class

End Namespace