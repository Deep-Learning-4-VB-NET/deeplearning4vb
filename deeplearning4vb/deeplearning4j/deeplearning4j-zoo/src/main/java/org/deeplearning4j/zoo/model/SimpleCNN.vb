Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
Imports Activation = org.nd4j.linalg.activations.Activation
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater

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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class SimpleCNN extends org.deeplearning4j.zoo.ZooModel
	Public Class SimpleCNN
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = new int[] {3, 48, 48};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 48, 48}
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
'JAVA TO VB CONVERTER NOTE: The local variable conf was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim conf_Conflict As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).activation(Activation.IDENTITY).weightInit(WeightInit.RELU).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).convolutionMode(ConvolutionMode.Same).list().layer(0, (New ConvolutionLayer.Builder(New Integer() {7, 7})).name("image_array").nIn(inputShape_Conflict(0)).nOut(16).build()).layer(1, (New BatchNormalization.Builder()).build()).layer(2, (New ConvolutionLayer.Builder(New Integer() {7, 7})).nIn(16).nOut(16).build()).layer(3, (New BatchNormalization.Builder()).build()).layer(4, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(5, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {2, 2})).build()).layer(6, (New DropoutLayer.Builder(0.5)).build()).layer(7, (New ConvolutionLayer.Builder(New Integer() {5, 5})).nOut(32).build()).layer(8, (New BatchNormalization.Builder()).build()).layer(9, (New ConvolutionLayer.Builder(New Integer() {5, 5})).nOut(32).build()).layer(10, (New BatchNormalization.Builder()).build()).layer(11, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(12, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {2, 2})).build()).layer(13, (New DropoutLayer.Builder(0.5)).build()).layer(14, (New ConvolutionLayer.Builder(New Integer() {3, 3})).nOut(64).build()).layer(15, (New BatchNormalization.Builder()).build()).layer(16, (New ConvolutionLayer.Builder(New Integer() {3, 3})).nOut(64).build()).layer(17, (New BatchNormalization.Builder()).build()).layer(18, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(19, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {2, 2})).build()).layer(20, (New DropoutLayer.Builder(0.5)).build()).layer(21, (New ConvolutionLayer.Builder(New Integer() {3, 3})).nOut(128).build()).layer(22, (New BatchNormalization.Builder()).build()).layer(23, (New ConvolutionLayer.Builder(New Integer() {3, 3})).nOut(128).build()).layer(24, (New BatchNormalization.Builder()).build()).layer(25, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(26, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG, New Integer() {2, 2})).build()).layer(27, (New DropoutLayer.Builder(0.5)).build()).layer(28, (New ConvolutionLayer.Builder(New Integer() {3, 3})).nOut(256).build()).layer(29, (New BatchNormalization.Builder()).build()).layer(30, (New ConvolutionLayer.Builder(New Integer() {3, 3})).nOut(numClasses).build()).layer(31, (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build()).layer(32, (New ActivationLayer.Builder()).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0))).build()

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