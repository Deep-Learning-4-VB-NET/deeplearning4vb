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
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class LeNet extends org.deeplearning4j.zoo.ZooModel
	Public Class LeNet
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {1, 28, 28};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {1, 28, 28}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
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
'ORIGINAL LINE: @Builder.@Default private org.deeplearning4j.nn.conf.layers.ConvolutionLayer.AlgoMode cudnnAlgoMode = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.AlgoMode.PREFER_FASTEST;
		Private cudnnAlgoMode As ConvolutionLayer.AlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST

		Private Sub New()
		End Sub

		Public Overrides Function pretrainedUrl(ByVal pretrainedType As PretrainedType) As String
			If pretrainedType = PretrainedType.MNIST Then
				Return DL4JResources.getURLString("models/lenet_dl4j_mnist_inference.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.MNIST Then
				Return 1906861161L
			Else
				Return 0L
			End If
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(MultiLayerNetwork)
		End Function

		Public Overridable Function conf() As MultiLayerConfiguration
'JAVA TO VB CONVERTER NOTE: The local variable conf was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim conf_Conflict As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).activation(Activation.IDENTITY).weightInit(WeightInit.XAVIER).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cudnnAlgoMode(cudnnAlgoMode).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).name("cnn1").kernelSize(5, 5).stride(1, 1).nIn(inputShape_Conflict(0)).nOut(20).activation(Activation.RELU).build()).layer((New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).name("maxpool1").kernelSize(2, 2).stride(2, 2).build()).layer((New ConvolutionLayer.Builder()).name("cnn2").kernelSize(5, 5).stride(1, 1).nOut(50).activation(Activation.RELU).build()).layer((New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).name("maxpool2").kernelSize(2, 2).stride(2, 2).build()).layer((New DenseLayer.Builder()).name("ffn1").activation(Activation.RELU).nOut(500).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).name("output").nOut(numClasses).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))).build()

			Return conf_Conflict
		End Function

		Public Overrides Function init() As Model
			Dim network As New MultiLayerNetwork(conf())
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