Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
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
Imports FaceNetHelper = org.deeplearning4j.zoo.model.helper.FaceNetHelper
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class FaceNetNN4Small2 extends org.deeplearning4j.zoo.ZooModel
	Public Class FaceNetNN4Small2
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 96, 96};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 96, 96}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.Adam(0.1, 0.9, 0.999, 0.01);
		Private updater As IUpdater = New Adam(0.1, 0.9, 0.999, 0.01)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.activations.Activation transferFunction = org.nd4j.linalg.activations.Activation.RELU;
		Private transferFunction As Activation = Activation.RELU
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default CacheMode cacheMode = CacheMode.NONE;
		Friend cacheMode As CacheMode = CacheMode.NONE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private WorkspaceMode workspaceMode = WorkspaceMode.ENABLED;
		Private workspaceMode As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ConvolutionLayer.AlgoMode cudnnAlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST;
		Private cudnnAlgoMode As ConvolutionLayer.AlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int embeddingSize = 128;
		Private embeddingSize As Integer = 128

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

		Public Overridable Function conf() As ComputationGraphConfiguration

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).activation(Activation.IDENTITY).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(WeightInit.RELU).l2(5e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cudnnAlgoMode(cudnnAlgoMode).convolutionMode(ConvolutionMode.Same).graphBuilder()


			graph.addInputs("input1").addLayer("stem-cnn1", (New ConvolutionLayer.Builder(New Integer() {7, 7}, New Integer() {2, 2}, New Integer() {3, 3})).nIn(inputShape_Conflict(0)).nOut(64).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "input1").addLayer("stem-batch1", (New BatchNormalization.Builder(False)).nIn(64).nOut(64).build(), "stem-cnn1").addLayer("stem-activation1", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), "stem-batch1").addLayer("stem-pool1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2}, New Integer() {1, 1})).build(), "stem-activation1").addLayer("stem-lrn1", (New LocalResponseNormalization.Builder(1, 5, 1e-4, 0.75)).build(), "stem-pool1").addLayer("inception-2-cnn1", (New ConvolutionLayer.Builder(New Integer() {1, 1})).nIn(64).nOut(64).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "stem-lrn1").addLayer("inception-2-batch1", (New BatchNormalization.Builder(False)).nIn(64).nOut(64).build(), "inception-2-cnn1").addLayer("inception-2-activation1", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), "inception-2-batch1").addLayer("inception-2-cnn2", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {1, 1}, New Integer() {1, 1})).nIn(64).nOut(192).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-2-activation1").addLayer("inception-2-batch2", (New BatchNormalization.Builder(False)).nIn(192).nOut(192).build(), "inception-2-cnn2").addLayer("inception-2-activation2", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), "inception-2-batch2").addLayer("inception-2-lrn1", (New LocalResponseNormalization.Builder(1, 5, 1e-4, 0.75)).build(), "inception-2-activation2").addLayer("inception-2-pool1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2}, New Integer() {1, 1})).build(), "inception-2-lrn1")

			' Inception 3a
			FaceNetHelper.appendGraph(graph, "3a", 192, New Integer() {3, 5}, New Integer() {1, 1}, New Integer() {128, 32}, New Integer() {96, 16, 32, 64}, SubsamplingLayer.PoolingType.MAX, transferFunction, "inception-2-pool1")
			' Inception 3b
			FaceNetHelper.appendGraph(graph, "3b", 256, New Integer() {3, 5}, New Integer() {1, 1}, New Integer() {128, 64}, New Integer() {96, 32, 64, 64}, SubsamplingLayer.PoolingType.PNORM, 2, transferFunction, "inception-3a")
			' Inception 3c
			'    Inception.appendGraph(graph, "3c", 320,
			'        new int[]{3,5}, new int[]{1,1}, new int[]{256,64}, new int[]{128,64},
			'        SubsamplingLayer.PoolingType.PNORM, 2, true, "inception-3b");

			graph.addLayer("3c-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(320).nOut(128).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-3b").addLayer("3c-1x1-norm", FaceNetHelper.batchNorm(128, 128), "3c-1x1").addLayer("3c-transfer1", (New ActivationLayer.Builder()).activation(transferFunction).build(), "3c-1x1-norm").addLayer("3c-3x3", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(128).nOut(256).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "3c-transfer1").addLayer("3c-3x3-norm", FaceNetHelper.batchNorm(256, 256), "3c-3x3").addLayer("3c-transfer2", (New ActivationLayer.Builder()).activation(transferFunction).build(), "3c-3x3-norm").addLayer("3c-2-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(320).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-3b").addLayer("3c-2-1x1-norm", FaceNetHelper.batchNorm(32, 32), "3c-2-1x1").addLayer("3c-2-transfer3", (New ActivationLayer.Builder()).activation(transferFunction).build(), "3c-2-1x1-norm").addLayer("3c-2-5x5", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(32).nOut(64).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "3c-2-transfer3").addLayer("3c-2-5x5-norm", FaceNetHelper.batchNorm(64, 64), "3c-2-5x5").addLayer("3c-2-transfer4", (New ActivationLayer.Builder()).activation(transferFunction).build(), "3c-2-5x5-norm").addLayer("3c-pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2}, New Integer() {1, 1})).build(), "inception-3b").addVertex("inception-3c", New MergeVertex(), "3c-transfer2", "3c-2-transfer4", "3c-pool")

			' Inception 4a
			FaceNetHelper.appendGraph(graph, "4a", 640, New Integer() {3, 5}, New Integer() {1, 1}, New Integer() {192, 64}, New Integer() {96, 32, 128, 256}, SubsamplingLayer.PoolingType.PNORM, 2, transferFunction, "inception-3c")

			'    // Inception 4e
			'    Inception.appendGraph(graph, "4e", 640,
			'        new int[]{3,5}, new int[]{2,2}, new int[]{256,128}, new int[]{160,64},
			'        SubsamplingLayer.PoolingType.MAX, 2, 1, true, "inception-4a");

			graph.addLayer("4e-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(640).nOut(160).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-4a").addLayer("4e-1x1-norm", FaceNetHelper.batchNorm(160, 160), "4e-1x1").addLayer("4e-transfer1", (New ActivationLayer.Builder()).activation(transferFunction).build(), "4e-1x1-norm").addLayer("4e-3x3", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(160).nOut(256).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "4e-transfer1").addLayer("4e-3x3-norm", FaceNetHelper.batchNorm(256, 256), "4e-3x3").addLayer("4e-transfer2", (New ActivationLayer.Builder()).activation(transferFunction).build(), "4e-3x3-norm").addLayer("4e-2-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(640).nOut(64).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-4a").addLayer("4e-2-1x1-norm", FaceNetHelper.batchNorm(64, 64), "4e-2-1x1").addLayer("4e-2-transfer3", (New ActivationLayer.Builder()).activation(transferFunction).build(), "4e-2-1x1-norm").addLayer("4e-2-5x5", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {2, 2})).nIn(64).nOut(128).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "4e-2-transfer3").addLayer("4e-2-5x5-norm", FaceNetHelper.batchNorm(128, 128), "4e-2-5x5").addLayer("4e-2-transfer4", (New ActivationLayer.Builder()).activation(transferFunction).build(), "4e-2-5x5-norm").addLayer("4e-pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2}, New Integer() {1, 1})).build(), "inception-4a").addVertex("inception-4e", New MergeVertex(), "4e-transfer2", "4e-2-transfer4", "4e-pool")

			' Inception 5a
			'    Inception.appendGraph(graph, "5a", 1024,
			'        new int[]{3}, new int[]{1}, new int[]{384}, new int[]{96,96,256},
			'        SubsamplingLayer.PoolingType.PNORM, 2, true, "inception-4e");

			graph.addLayer("5a-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(1024).nOut(256).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-4e").addLayer("5a-1x1-norm", FaceNetHelper.batchNorm(256, 256), "5a-1x1").addLayer("5a-transfer1", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5a-1x1-norm").addLayer("5a-2-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(1024).nOut(96).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-4e").addLayer("5a-2-1x1-norm", FaceNetHelper.batchNorm(96, 96), "5a-2-1x1").addLayer("5a-2-transfer2", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5a-2-1x1-norm").addLayer("5a-2-3x3", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {1, 1})).nIn(96).nOut(384).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "5a-2-transfer2").addLayer("5a-2-3x3-norm", FaceNetHelper.batchNorm(384, 384), "5a-2-3x3").addLayer("5a-transfer3", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5a-2-3x3-norm").addLayer("5a-3-pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.PNORM, New Integer() {3, 3}, New Integer() {1, 1})).pnorm(2).build(), "inception-4e").addLayer("5a-3-1x1reduce", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(1024).nOut(96).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "5a-3-pool").addLayer("5a-3-1x1reduce-norm", FaceNetHelper.batchNorm(96, 96), "5a-3-1x1reduce").addLayer("5a-3-transfer4", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), "5a-3-1x1reduce-norm").addVertex("inception-5a", New MergeVertex(), "5a-transfer1", "5a-transfer3", "5a-3-transfer4")


			' Inception 5b
			'    Inception.appendGraph(graph, "5b", 736,
			'        new int[]{3}, new int[]{1}, new int[]{384}, new int[]{96,96,256},
			'        SubsamplingLayer.PoolingType.MAX, 1, 1, true, "inception-5a");

			graph.addLayer("5b-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(736).nOut(256).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-5a").addLayer("5b-1x1-norm", FaceNetHelper.batchNorm(256, 256), "5b-1x1").addLayer("5b-transfer1", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5b-1x1-norm").addLayer("5b-2-1x1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(736).nOut(96).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "inception-5a").addLayer("5b-2-1x1-norm", FaceNetHelper.batchNorm(96, 96), "5b-2-1x1").addLayer("5b-2-transfer2", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5b-2-1x1-norm").addLayer("5b-2-3x3", (New ConvolutionLayer.Builder(New Integer() {3, 3}, New Integer() {1, 1})).nIn(96).nOut(384).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "5b-2-transfer2").addLayer("5b-2-3x3-norm", FaceNetHelper.batchNorm(384, 384), "5b-2-3x3").addLayer("5b-2-transfer3", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5b-2-3x3-norm").addLayer("5b-3-pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {1, 1}, New Integer() {1, 1})).build(), "inception-5a").addLayer("5b-3-1x1reduce", (New ConvolutionLayer.Builder(New Integer() {1, 1}, New Integer() {1, 1})).nIn(736).nOut(96).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), "5b-3-pool").addLayer("5b-3-1x1reduce-norm", FaceNetHelper.batchNorm(96, 96), "5b-3-1x1reduce").addLayer("5b-3-transfer4", (New ActivationLayer.Builder()).activation(transferFunction).build(), "5b-3-1x1reduce-norm").addVertex("inception-5b", New MergeVertex(), "5b-transfer1", "5b-2-transfer3", "5b-3-transfer4")

			graph.addLayer("avgpool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {3, 3}, New Integer() {3, 3})).build(), "inception-5b").addLayer("bottleneck",(New DenseLayer.Builder()).nOut(embeddingSize).activation(Activation.IDENTITY).build(),"avgpool").addVertex("embeddings", New L2NormalizeVertex(New Integer() {}, 1e-6), "bottleneck").addLayer("lossLayer", (New CenterLossOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.SQUARED_LOSS).activation(Activation.SOFTMAX).nOut(numClasses).lambda(1e-4).alpha(0.9).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).build(), "embeddings").setOutputs("lossLayer").setInputTypes(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0)))

			Return graph.build()
		End Function

		Public Overrides Function init() As ComputationGraph
			Dim model As New ComputationGraph(conf())
			model.init()

			Return model
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