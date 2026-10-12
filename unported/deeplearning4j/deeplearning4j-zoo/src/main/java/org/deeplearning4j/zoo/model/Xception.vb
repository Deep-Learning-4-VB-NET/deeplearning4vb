Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
Imports Activation = org.nd4j.linalg.activations.Activation
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class Xception extends org.deeplearning4j.zoo.ZooModel
	Public Class Xception
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 299, 299};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 299, 299}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.deeplearning4j.nn.weights.WeightInit weightInit = org.deeplearning4j.nn.weights.WeightInit.RELU;
		Private weightInit As WeightInit = WeightInit.RELU
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.AdaDelta();
		Private updater As IUpdater = New AdaDelta()
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
			If pretrainedType = PretrainedType.IMAGENET Then
				Return DL4JResources.getURLString("models/xception_dl4j_inference.v2.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				Return 3277876097L
			Else
				Return 0L
			End If
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(ComputationGraph)
		End Function

		Public Overrides Function init() As ComputationGraph
			Dim graph As ComputationGraphConfiguration.GraphBuilder = graphBuilder()

			graph.addInputs("input").InputTypes = InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))

			Dim conf As ComputationGraphConfiguration = graph.build()
			Dim model As New ComputationGraph(conf)
			model.init()

			Return model
		End Function

		Public Overridable Function graphBuilder() As ComputationGraphConfiguration.GraphBuilder

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(weightInit).l2(4e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).convolutionMode(ConvolutionMode.Truncate).graphBuilder()


			graph.addLayer("block1_conv1", (New ConvolutionLayer.Builder(3,3)).stride(2,2).nOut(32).hasBias(False).cudnnAlgoMode(cudnnAlgoMode).build(), "input").addLayer("block1_conv1_bn", New BatchNormalization(), "block1_conv1").addLayer("block1_conv1_act", New ActivationLayer(Activation.RELU), "block1_conv1_bn").addLayer("block1_conv2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(64).hasBias(False).cudnnAlgoMode(cudnnAlgoMode).build(), "block1_conv1_act").addLayer("block1_conv2_bn", New BatchNormalization(), "block1_conv2").addLayer("block1_conv2_act", New ActivationLayer(Activation.RELU), "block1_conv2_bn").addLayer("residual1_conv", (New ConvolutionLayer.Builder(1,1)).stride(2,2).nOut(128).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block1_conv2_act").addLayer("residual1", New BatchNormalization(), "residual1_conv").addLayer("block2_sepconv1", (New SeparableConvolution2D.Builder(3,3)).nOut(128).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block1_conv2_act").addLayer("block2_sepconv1_bn", New BatchNormalization(), "block2_sepconv1").addLayer("block2_sepconv1_act",New ActivationLayer(Activation.RELU), "block2_sepconv1_bn").addLayer("block2_sepconv2", (New SeparableConvolution2D.Builder(3,3)).nOut(128).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block2_sepconv1_act").addLayer("block2_sepconv2_bn", New BatchNormalization(), "block2_sepconv2").addLayer("block2_pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), "block2_sepconv2_bn").addVertex("add1", New ElementWiseVertex(ElementWiseVertex.Op.Add), "block2_pool", "residual1").addLayer("residual2_conv", (New ConvolutionLayer.Builder(1,1)).stride(2,2).nOut(256).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "add1").addLayer("residual2", New BatchNormalization(), "residual2_conv").addLayer("block3_sepconv1_act", New ActivationLayer(Activation.RELU), "add1").addLayer("block3_sepconv1", (New SeparableConvolution2D.Builder(3,3)).nOut(256).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block3_sepconv1_act").addLayer("block3_sepconv1_bn", New BatchNormalization(), "block3_sepconv1").addLayer("block3_sepconv2_act", New ActivationLayer(Activation.RELU), "block3_sepconv1_bn").addLayer("block3_sepconv2", (New SeparableConvolution2D.Builder(3,3)).nOut(256).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block3_sepconv2_act").addLayer("block3_sepconv2_bn", New BatchNormalization(), "block3_sepconv2").addLayer("block3_pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), "block3_sepconv2_bn").addVertex("add2", New ElementWiseVertex(ElementWiseVertex.Op.Add), "block3_pool", "residual2").addLayer("residual3_conv", (New ConvolutionLayer.Builder(1,1)).stride(2,2).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "add2").addLayer("residual3", New BatchNormalization(), "residual3_conv").addLayer("block4_sepconv1_act", New ActivationLayer(Activation.RELU), "add2").addLayer("block4_sepconv1", (New SeparableConvolution2D.Builder(3,3)).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block4_sepconv1_act").addLayer("block4_sepconv1_bn", New BatchNormalization(), "block4_sepconv1").addLayer("block4_sepconv2_act", New ActivationLayer(Activation.RELU), "block4_sepconv1_bn").addLayer("block4_sepconv2", (New SeparableConvolution2D.Builder(3,3)).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block4_sepconv2_act").addLayer("block4_sepconv2_bn", New BatchNormalization(), "block4_sepconv2").addLayer("block4_pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), "block4_sepconv2_bn").addVertex("add3", New ElementWiseVertex(ElementWiseVertex.Op.Add), "block4_pool", "residual3")

			' towers
			Dim residual As Integer = 3
			Dim block As Integer = 5
			For i As Integer = 0 To 7
				Dim previousInput As String = "add" & residual
				Dim blockName As String = "block" & block

				graph.addLayer(blockName & "_sepconv1_act", New ActivationLayer(Activation.RELU), previousInput).addLayer(blockName & "_sepconv1", (New SeparableConvolution2D.Builder(3,3)).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), blockName & "_sepconv1_act").addLayer(blockName & "_sepconv1_bn", New BatchNormalization(), blockName & "_sepconv1").addLayer(blockName & "_sepconv2_act", New ActivationLayer(Activation.RELU), blockName & "_sepconv1_bn").addLayer(blockName & "_sepconv2", (New SeparableConvolution2D.Builder(3,3)).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), blockName & "_sepconv2_act").addLayer(blockName & "_sepconv2_bn", New BatchNormalization(), blockName & "_sepconv2").addLayer(blockName & "_sepconv3_act", New ActivationLayer(Activation.RELU), blockName & "_sepconv2_bn").addLayer(blockName & "_sepconv3", (New SeparableConvolution2D.Builder(3,3)).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), blockName & "_sepconv3_act").addLayer(blockName & "_sepconv3_bn", New BatchNormalization(), blockName & "_sepconv3").addVertex("add" & (residual+1), New ElementWiseVertex(ElementWiseVertex.Op.Add), blockName & "_sepconv3_bn", previousInput)

				residual += 1
				block += 1
			Next i

			' residual12
					graph.addLayer("residual12_conv", (New ConvolutionLayer.Builder(1,1)).stride(2,2).nOut(1024).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "add" & residual).addLayer("residual12", New BatchNormalization(), "residual12_conv")

					' block13
			graph.addLayer("block13_sepconv1_act", New ActivationLayer(Activation.RELU), "add11").addLayer("block13_sepconv1", (New SeparableConvolution2D.Builder(3,3)).nOut(728).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block13_sepconv1_act").addLayer("block13_sepconv1_bn", New BatchNormalization(), "block13_sepconv1").addLayer("block13_sepconv2_act", New ActivationLayer(Activation.RELU), "block13_sepconv1_bn").addLayer("block13_sepconv2", (New SeparableConvolution2D.Builder(3,3)).nOut(1024).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block13_sepconv2_act").addLayer("block13_sepconv2_bn", New BatchNormalization(), "block13_sepconv2").addLayer("block13_pool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), "block13_sepconv2_bn").addVertex("add12", New ElementWiseVertex(ElementWiseVertex.Op.Add), "block13_pool", "residual12")

					' block14
			graph.addLayer("block14_sepconv1", (New SeparableConvolution2D.Builder(3,3)).nOut(1536).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "add12").addLayer("block14_sepconv1_bn", New BatchNormalization(), "block14_sepconv1").addLayer("block14_sepconv1_act", New ActivationLayer(Activation.RELU), "block14_sepconv1_bn").addLayer("block14_sepconv2", (New SeparableConvolution2D.Builder(3,3)).nOut(2048).hasBias(False).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), "block14_sepconv1_act").addLayer("block14_sepconv2_bn", New BatchNormalization(), "block14_sepconv2").addLayer("block14_sepconv2_act", New ActivationLayer(Activation.RELU), "block14_sepconv2_bn").addLayer("avg_pool", (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build(), "block14_sepconv2_act").addLayer("predictions", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nOut(numClasses).activation(Activation.SOFTMAX).build(), "avg_pool").setOutputs("predictions")

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