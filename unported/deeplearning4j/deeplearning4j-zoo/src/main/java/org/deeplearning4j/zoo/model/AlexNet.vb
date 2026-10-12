Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class AlexNet extends org.deeplearning4j.zoo.ZooModel
	Public Class AlexNet
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
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.Nesterovs(1e-2, 0.9);
		Private updater As IUpdater = New Nesterovs(1e-2, 0.9)
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
			Return GetType(MultiLayerNetwork)
		End Function

		Public Overridable Function conf() As MultiLayerConfiguration
			Dim nonZeroBias As Double = 1

'JAVA TO VB CONVERTER NOTE: The local variable conf was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim conf_Conflict As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).weightInit(New NormalDistribution(0.0, 0.01)).activation(Activation.RELU).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).biasUpdater(New Nesterovs(2e-2, 0.9)).convolutionMode(ConvolutionMode.Same).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cacheMode(cacheMode).l2(5 * 1e-4).miniBatch(False).list().layer(0, (New ConvolutionLayer.Builder(New Integer(){11, 11}, New Integer(){4, 4})).name("cnn1").cudnnAlgoMode(ConvolutionLayer.AlgoMode.PREFER_FASTEST).convolutionMode(ConvolutionMode.Truncate).nIn(inputShape_Conflict(0)).nOut(96).build()).layer(1, (New LocalResponseNormalization.Builder()).build()).layer(2, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).padding(1,1).name("maxpool1").build()).layer(3, (New ConvolutionLayer.Builder(New Integer(){5, 5}, New Integer(){1, 1}, New Integer(){2, 2})).name("cnn2").cudnnAlgoMode(ConvolutionLayer.AlgoMode.PREFER_FASTEST).convolutionMode(ConvolutionMode.Truncate).nOut(256).biasInit(nonZeroBias).build()).layer(4, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer(){3, 3}, New Integer(){2, 2})).convolutionMode(ConvolutionMode.Truncate).name("maxpool2").build()).layer(5, (New LocalResponseNormalization.Builder()).build()).layer(6, (New ConvolutionLayer.Builder()).kernelSize(3,3).stride(1,1).convolutionMode(ConvolutionMode.Same).name("cnn3").cudnnAlgoMode(ConvolutionLayer.AlgoMode.PREFER_FASTEST).nOut(384).build()).layer(7, (New ConvolutionLayer.Builder(New Integer(){3, 3}, New Integer(){1, 1})).name("cnn4").cudnnAlgoMode(ConvolutionLayer.AlgoMode.PREFER_FASTEST).nOut(384).biasInit(nonZeroBias).build()).layer(8, (New ConvolutionLayer.Builder(New Integer(){3, 3}, New Integer(){1, 1})).name("cnn5").cudnnAlgoMode(ConvolutionLayer.AlgoMode.PREFER_FASTEST).nOut(256).biasInit(nonZeroBias).build()).layer(9, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer(){3, 3}, New Integer(){2, 2})).name("maxpool3").convolutionMode(ConvolutionMode.Truncate).build()).layer(10, (New DenseLayer.Builder()).name("ffn1").nIn(256*6*6).nOut(4096).weightInit(New NormalDistribution(0, 0.005)).biasInit(nonZeroBias).build()).layer(11, (New DenseLayer.Builder()).name("ffn2").nOut(4096).weightInit(New NormalDistribution(0, 0.005)).biasInit(nonZeroBias).dropOut(0.5).build()).layer(12, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).name("output").nOut(numClasses).activation(Activation.SOFTMAX).weightInit(New NormalDistribution(0, 0.005)).biasInit(0.1).build()).setInputType(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))).build()

			Return conf_Conflict
		End Function

		Public Overrides Function init() As MultiLayerNetwork
			Dim conf As MultiLayerConfiguration = Me.conf()
			Dim network As New MultiLayerNetwork(conf)
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