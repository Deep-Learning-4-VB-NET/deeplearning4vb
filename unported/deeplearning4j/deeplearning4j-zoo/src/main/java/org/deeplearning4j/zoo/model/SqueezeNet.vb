Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class SqueezeNet extends org.deeplearning4j.zoo.ZooModel
	Public Class SqueezeNet
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 227, 227};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 227, 227}
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
				Return DL4JResources.getURLString("models/squeezenet_dl4j_inference.v2.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				Return 3711411239L
			Else
				Return 0L
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.graph.ComputationGraph initPretrained(org.deeplearning4j.zoo.PretrainedType pretrainedType) throws java.io.IOException
		Public Overrides Function initPretrained(ByVal pretrainedType As PretrainedType) As ComputationGraph
			Dim cg As ComputationGraph = CType(MyBase.initPretrained(pretrainedType), ComputationGraph)
			'Set collapse dimensions to true in global avg pooling - more useful for users [N,1000] rather than [N,1000,1,1] out. Also matches non-pretrain config
			CType(cg.getLayer("global_average_pooling2d_5").conf().getLayer(), GlobalPoolingLayer).setCollapseDimensions(True)
			Return cg
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

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(weightInit).l2(5e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).convolutionMode(ConvolutionMode.Truncate).graphBuilder()


			graph.addLayer("conv1", (New ConvolutionLayer.Builder(3,3)).stride(2,2).nOut(64).cudnnAlgoMode(cudnnAlgoMode).build(), "input").addLayer("conv1_act", New ActivationLayer(Activation.RELU), "conv1").addLayer("pool1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).build(), "conv1_act")

			' fire modules
			fireModule(graph, 2, 16, 64, "pool1")
			fireModule(graph, 3, 16, 64, "fire2")
			graph.addLayer("pool3", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).build(), "fire3")

			fireModule(graph, 4, 32, 128, "pool3")
			fireModule(graph, 5, 32, 128, "fire4")
			graph.addLayer("pool5", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).build(), "fire5")

			fireModule(graph, 6, 48, 192, "pool5")
			fireModule(graph, 7, 48, 192, "fire6")
			fireModule(graph, 8, 64, 256, "fire7")
			fireModule(graph, 9, 64, 256, "fire8")

			graph.addLayer("drop9", (New DropoutLayer.Builder(0.5)).build(), "fire9").addLayer("conv10", (New ConvolutionLayer.Builder(1,1)).nOut(numClasses).cudnnAlgoMode(cudnnAlgoMode).build(), "drop9").addLayer("conv10_act", New ActivationLayer(Activation.RELU), "conv10").addLayer("avg_pool", New GlobalPoolingLayer(PoolingType.AVG), "conv10_act").addLayer("softmax", New ActivationLayer(Activation.SOFTMAX), "avg_pool").addLayer("loss", (New LossLayer.Builder(LossFunctions.LossFunction.MCXENT)).build(), "softmax").setOutputs("loss")

			Return graph
		End Function

		Private Function fireModule(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal fireId As Integer, ByVal squeeze As Integer, ByVal expand As Integer, ByVal input As String) As String
			Dim prefix As String = "fire" & fireId

			graphBuilder.addLayer(prefix & "_sq1x1", (New ConvolutionLayer.Builder(1, 1)).nOut(squeeze).cudnnAlgoMode(cudnnAlgoMode).build(), input).addLayer(prefix & "_relu_sq1x1", New ActivationLayer(Activation.RELU), prefix & "_sq1x1").addLayer(prefix & "_exp1x1", (New ConvolutionLayer.Builder(1, 1)).nOut(expand).cudnnAlgoMode(cudnnAlgoMode).build(), prefix & "_relu_sq1x1").addLayer(prefix & "_relu_exp1x1", New ActivationLayer(Activation.RELU), prefix & "_exp1x1").addLayer(prefix & "_exp3x3", (New ConvolutionLayer.Builder(3,3)).nOut(expand).convolutionMode(ConvolutionMode.Same).cudnnAlgoMode(cudnnAlgoMode).build(), prefix & "_relu_sq1x1").addLayer(prefix & "_relu_exp3x3", New ActivationLayer(Activation.RELU), prefix & "_exp3x3").addVertex(prefix, New MergeVertex(), prefix & "_relu_exp1x1", prefix & "_relu_exp3x3")

			Return prefix
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