Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports TruncatedNormalDistribution = org.deeplearning4j.nn.conf.distribution.TruncatedNormalDistribution
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class UNet extends org.deeplearning4j.zoo.ZooModel
	Public Class UNet
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 512, 512};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 512, 512}
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
			If pretrainedType = PretrainedType.SEGMENT Then
				Return DL4JResources.getURLString("models/unet_dl4j_segment_inference.v1.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.SEGMENT Then
				Return 712347958L
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

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(weightInit).l2(5e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).graphBuilder()


			graph.addLayer("conv1-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(64).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "input").addLayer("conv1-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(64).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv1-1").addLayer("pool1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2,2).build(), "conv1-2").addLayer("conv2-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(128).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "pool1").addLayer("conv2-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(128).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv2-1").addLayer("pool2", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2,2).build(), "conv2-2").addLayer("conv3-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(256).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "pool2").addLayer("conv3-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(256).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv3-1").addLayer("pool3", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2,2).build(), "conv3-2").addLayer("conv4-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(512).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "pool3").addLayer("conv4-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(512).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv4-1").addLayer("drop4", (New DropoutLayer.Builder(0.5)).build(), "conv4-2").addLayer("pool4", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2,2).build(), "drop4").addLayer("conv5-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(1024).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "pool4").addLayer("conv5-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(1024).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv5-1").addLayer("drop5", (New DropoutLayer.Builder(0.5)).build(), "conv5-2").addLayer("up6-1", (New Upsampling2D.Builder(2)).build(), "drop5").addLayer("up6-2", (New ConvolutionLayer.Builder(2,2)).stride(1,1).nOut(512).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "up6-1").addVertex("merge6", New MergeVertex(), "drop4", "up6-2").addLayer("conv6-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(512).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "merge6").addLayer("conv6-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(512).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv6-1").addLayer("up7-1", (New Upsampling2D.Builder(2)).build(), "conv6-2").addLayer("up7-2", (New ConvolutionLayer.Builder(2,2)).stride(1,1).nOut(256).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "up7-1").addVertex("merge7", New MergeVertex(), "conv3-2", "up7-2").addLayer("conv7-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(256).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "merge7").addLayer("conv7-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(256).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv7-1").addLayer("up8-1", (New Upsampling2D.Builder(2)).build(), "conv7-2").addLayer("up8-2", (New ConvolutionLayer.Builder(2,2)).stride(1,1).nOut(128).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "up8-1").addVertex("merge8", New MergeVertex(), "conv2-2", "up8-2").addLayer("conv8-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(128).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "merge8").addLayer("conv8-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(128).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv8-1").addLayer("up9-1", (New Upsampling2D.Builder(2)).build(), "conv8-2").addLayer("up9-2", (New ConvolutionLayer.Builder(2,2)).stride(1,1).nOut(64).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "up9-1").addVertex("merge9", New MergeVertex(), "conv1-2", "up9-2").addLayer("conv9-1", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(64).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "merge9").addLayer("conv9-2", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(64).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv9-1").addLayer("conv9-3", (New ConvolutionLayer.Builder(3,3)).stride(1,1).nOut(2).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.RELU).build(), "conv9-2").addLayer("conv10", (New ConvolutionLayer.Builder(1,1)).stride(1,1).nOut(1).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).activation(Activation.IDENTITY).build(), "conv9-3").addLayer("output", (New CnnLossLayer.Builder(LossFunctions.LossFunction.XENT)).activation(Activation.SIGMOID).build(), "conv10").setOutputs("output")

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