Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class VGG19 extends org.deeplearning4j.zoo.ZooModel
	Public Class VGG19
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
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.Nesterovs();
		Private updater As IUpdater = New Nesterovs()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private CacheMode cacheMode = CacheMode.NONE;
		Private cacheMode As CacheMode = CacheMode.NONE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private WorkspaceMode workspaceMode = WorkspaceMode.ENABLED;
		Private workspaceMode As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.deeplearning4j.nn.conf.layers.ConvolutionLayer.AlgoMode cudnnAlgoMode = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.AlgoMode.NO_WORKSPACE;
		Private cudnnAlgoMode As ConvolutionLayer.AlgoMode = ConvolutionLayer.AlgoMode.NO_WORKSPACE

		Private Sub New()
		End Sub

		Public Overrides Function pretrainedUrl(ByVal pretrainedType As PretrainedType) As String
			If pretrainedType = PretrainedType.IMAGENET Then
				Return DL4JResources.getURLString("models/vgg19_dl4j_inference.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				Return 2782932419L
			Else
				Return 0L
			End If
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(ComputationGraph)
		End Function

		Public Overridable Function conf() As ComputationGraphConfiguration
'JAVA TO VB CONVERTER NOTE: The local variable conf was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim conf_Conflict As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).activation(Activation.RELU).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).graphBuilder().addInputs("in").layer(0, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nIn(inputShape_Conflict(0)).nOut(64).cudnnAlgoMode(cudnnAlgoMode).build(), "in").layer(1, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(64).cudnnAlgoMode(cudnnAlgoMode).build(), "0").layer(2, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(2, 2).stride(2, 2).build(), "1").layer(3, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(128).cudnnAlgoMode(cudnnAlgoMode).build(), "2").layer(4, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(128).cudnnAlgoMode(cudnnAlgoMode).build(), "3").layer(5, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(2, 2).stride(2, 2).build(), "4").layer(6, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "5").layer(7, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "6").layer(8, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "7").layer(9, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(256).cudnnAlgoMode(cudnnAlgoMode).build(), "8").layer(10, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(2, 2).stride(2, 2).build(), "9").layer(11, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "10").layer(12, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "11").layer(13, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "12").layer(14, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "13").layer(15, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(2, 2).stride(2, 2).build(), "14").layer(16, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "15").layer(17, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "16").layer(18, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "17").layer(19, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(1, 1).nOut(512).cudnnAlgoMode(cudnnAlgoMode).build(), "18").layer(20, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(2, 2).stride(2, 2).build(), "19").layer(21, (New DenseLayer.Builder()).nOut(4096).build(), "20").layer(22, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).name("output").nOut(numClasses).activation(Activation.SOFTMAX).build(), "21").setOutputs("22").setInputTypes(InputType.convolutionalFlat(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))).build()

			Return conf_Conflict
		End Function

		Public Overrides Function init() As ComputationGraph
			Dim network As New ComputationGraph(conf())
			network.init()
			Return network
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