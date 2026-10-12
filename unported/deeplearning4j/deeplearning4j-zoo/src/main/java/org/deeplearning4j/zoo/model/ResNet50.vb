Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports TruncatedNormalDistribution = org.deeplearning4j.nn.conf.distribution.TruncatedNormalDistribution
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class ResNet50 extends org.deeplearning4j.zoo.ZooModel
	Public Class ResNet50
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 224, 224};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 224, 224}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.deeplearning4j.nn.weights.IWeightInit weightInit = new org.deeplearning4j.nn.weights.WeightInitDistribution(new org.deeplearning4j.nn.conf.distribution.TruncatedNormalDistribution(0.0, 0.5));
		Private weightInit As IWeightInit = New WeightInitDistribution(New TruncatedNormalDistribution(0.0, 0.5))
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
			If pretrainedType = PretrainedType.IMAGENET Then
				Return DL4JResources.getURLString("models/resnet50_dl4j_inference.v3.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				Return 3914447815L
			Else
				Return 0L
			End If
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(ComputationGraph)
		End Function

		Public Overrides Function init() As ComputationGraph
			Dim graph As ComputationGraphConfiguration.GraphBuilder = graphBuilder()
			Dim conf As ComputationGraphConfiguration = graph.build()
			Dim model As New ComputationGraph(conf)
			model.init()

			Return model
		End Function

		Private Sub identityBlock(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal kernelSize() As Integer, ByVal filters() As Integer, ByVal stage As String, ByVal block As String, ByVal input As String)
			Dim convName As String = "res" & stage & block & "_branch"
			Dim batchName As String = "bn" & stage & block & "_branch"
			Dim activationName As String = "act" & stage & block & "_branch"
			Dim shortcutName As String = "short" & stage & block & "_branch"

			graph.addLayer(convName & "2a", (New ConvolutionLayer.Builder(New Integer() {1, 1})).nOut(filters(0)).cudnnAlgoMode(cudnnAlgoMode).build(), input).addLayer(batchName & "2a", New BatchNormalization(), convName & "2a").addLayer(activationName & "2a", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), batchName & "2a").addLayer(convName & "2b", (New ConvolutionLayer.Builder(kernelSize)).nOut(filters(1)).cudnnAlgoMode(cudnnAlgoMode).convolutionMode(ConvolutionMode.Same).build(), activationName & "2a").addLayer(batchName & "2b", New BatchNormalization(), convName & "2b").addLayer(activationName & "2b", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), batchName & "2b").addLayer(convName & "2c", (New ConvolutionLayer.Builder(New Integer() {1, 1})).nOut(filters(2)).cudnnAlgoMode(cudnnAlgoMode).build(), activationName & "2b").addLayer(batchName & "2c", New BatchNormalization(), convName & "2c").addVertex(shortcutName, New ElementWiseVertex(ElementWiseVertex.Op.Add), batchName & "2c", input).addLayer(convName, (New ActivationLayer.Builder()).activation(Activation.RELU).build(), shortcutName)
		End Sub

		Private Sub convBlock(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal kernelSize() As Integer, ByVal filters() As Integer, ByVal stage As String, ByVal block As String, ByVal input As String)
			convBlock(graph, kernelSize, filters, stage, block, New Integer() {2, 2}, input)
		End Sub

		Private Sub convBlock(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal kernelSize() As Integer, ByVal filters() As Integer, ByVal stage As String, ByVal block As String, ByVal stride() As Integer, ByVal input As String)
			Dim convName As String = "res" & stage & block & "_branch"
			Dim batchName As String = "bn" & stage & block & "_branch"
			Dim activationName As String = "act" & stage & block & "_branch"
			Dim shortcutName As String = "short" & stage & block & "_branch"

			graph.addLayer(convName & "2a", (New ConvolutionLayer.Builder(New Integer() {1, 1}, stride)).nOut(filters(0)).build(), input).addLayer(batchName & "2a", New BatchNormalization(), convName & "2a").addLayer(activationName & "2a", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), batchName & "2a").addLayer(convName & "2b", (New ConvolutionLayer.Builder(kernelSize)).nOut(filters(1)).convolutionMode(ConvolutionMode.Same).build(), activationName & "2a").addLayer(batchName & "2b", New BatchNormalization(), convName & "2b").addLayer(activationName & "2b", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), batchName & "2b").addLayer(convName & "2c", (New ConvolutionLayer.Builder(New Integer() {1, 1})).nOut(filters(2)).build(), activationName & "2b").addLayer(batchName & "2c", New BatchNormalization(), convName & "2c").addLayer(convName & "1", (New ConvolutionLayer.Builder(New Integer() {1, 1}, stride)).nOut(filters(2)).build(), input).addLayer(batchName & "1", New BatchNormalization(), convName & "1").addVertex(shortcutName, New ElementWiseVertex(ElementWiseVertex.Op.Add), batchName & "2c", batchName & "1").addLayer(convName, (New ActivationLayer.Builder()).activation(Activation.RELU).build(), shortcutName)
		End Sub

		Public Overridable Function graphBuilder() As ComputationGraphConfiguration.GraphBuilder

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).activation(Activation.IDENTITY).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(weightInit).l1(1e-7).l2(5e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cudnnAlgoMode(cudnnAlgoMode).convolutionMode(ConvolutionMode.Truncate).graphBuilder()


			graph.addInputs("input").setInputTypes(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))).addLayer("stem-zero", (New ZeroPaddingLayer.Builder(3, 3)).build(), "input").addLayer("stem-cnn1", (New ConvolutionLayer.Builder(New Integer() {7, 7}, New Integer() {2, 2})).nOut(64).build(), "stem-zero").addLayer("stem-batch1", New BatchNormalization(), "stem-cnn1").addLayer("stem-act1", (New ActivationLayer.Builder()).activation(Activation.RELU).build(), "stem-batch1").addLayer("stem-maxpool1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3}, New Integer() {2, 2})).build(), "stem-act1")

			convBlock(graph, New Integer() {3, 3}, New Integer() {64, 64, 256}, "2", "a", New Integer() {2, 2}, "stem-maxpool1")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {64, 64, 256}, "2", "b", "res2a_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {64, 64, 256}, "2", "c", "res2b_branch")

			convBlock(graph, New Integer() {3, 3}, New Integer() {128, 128, 512}, "3", "a", "res2c_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {128, 128, 512}, "3", "b", "res3a_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {128, 128, 512}, "3", "c", "res3b_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {128, 128, 512}, "3", "d", "res3c_branch")

			convBlock(graph, New Integer() {3, 3}, New Integer() {256, 256, 1024}, "4", "a", "res3d_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {256, 256, 1024}, "4", "b", "res4a_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {256, 256, 1024}, "4", "c", "res4b_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {256, 256, 1024}, "4", "d", "res4c_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {256, 256, 1024}, "4", "e", "res4d_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {256, 256, 1024}, "4", "f", "res4e_branch")

			convBlock(graph, New Integer() {3, 3}, New Integer() {512, 512, 2048}, "5", "a", "res4f_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {512, 512, 2048}, "5", "b", "res5a_branch")
			identityBlock(graph, New Integer() {3, 3}, New Integer() {512, 512, 2048}, "5", "c", "res5b_branch")

			graph.addLayer("avgpool", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() {3, 3})).build(), "res5c_branch").addLayer("output", (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(numClasses).activation(Activation.SOFTMAX).build(), "avgpool").setOutputs("output")

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