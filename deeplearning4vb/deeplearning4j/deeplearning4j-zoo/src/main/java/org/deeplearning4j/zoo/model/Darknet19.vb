Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports org.deeplearning4j.nn.conf
Imports GraphBuilder = org.deeplearning4j.nn.conf.ComputationGraphConfiguration.GraphBuilder
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.deeplearning4j.zoo.model.helper.DarknetHelper.addLayers

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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class Darknet19 extends org.deeplearning4j.zoo.ZooModel
	Public Class Darknet19
		Inherits ZooModel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = {3, 224, 224};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 224, 224}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.deeplearning4j.nn.weights.WeightInit weightInit = org.deeplearning4j.nn.weights.WeightInit.RELU;
		Private weightInit As WeightInit = WeightInit.RELU
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.Nesterovs(1e-3, 0.9);
		Private updater As IUpdater = New Nesterovs(1e-3, 0.9)
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
				If inputShape_Conflict(1) = 448 AndAlso inputShape_Conflict(2) = 448 Then
					Return DL4JResources.getURLString("models/darknet19_448_dl4j_inference.v2.zip")
				Else
					Return DL4JResources.getURLString("models/darknet19_dl4j_inference.v2.zip")
				End If
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				If inputShape_Conflict(1) = 448 AndAlso inputShape_Conflict(2) = 448 Then
					Return 1054319943L
				Else
					Return 691100891L
				End If
			Else
				Return 0L
			End If
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(ComputationGraph)
		End Function

		Public Overridable Function conf() As ComputationGraphConfiguration
			Dim graphBuilder As GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).updater(updater).weightInit(weightInit).l2(0.00001).activation(Activation.IDENTITY).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cudnnAlgoMode(cudnnAlgoMode).graphBuilder().addInputs("input").setInputTypes(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0)))

			addLayers(graphBuilder, 1, 3, inputShape_Conflict(0), 32, 2)

			addLayers(graphBuilder, 2, 3, 32, 64, 2)

			addLayers(graphBuilder, 3, 3, 64, 128, 0)
			addLayers(graphBuilder, 4, 1, 128, 64, 0)
			addLayers(graphBuilder, 5, 3, 64, 128, 2)

			addLayers(graphBuilder, 6, 3, 128, 256, 0)
			addLayers(graphBuilder, 7, 1, 256, 128, 0)
			addLayers(graphBuilder, 8, 3, 128, 256, 2)

			addLayers(graphBuilder, 9, 3, 256, 512, 0)
			addLayers(graphBuilder, 10, 1, 512, 256, 0)
			addLayers(graphBuilder, 11, 3, 256, 512, 0)
			addLayers(graphBuilder, 12, 1, 512, 256, 0)
			addLayers(graphBuilder, 13, 3, 256, 512, 2)

			addLayers(graphBuilder, 14, 3, 512, 1024, 0)
			addLayers(graphBuilder, 15, 1, 1024, 512, 0)
			addLayers(graphBuilder, 16, 3, 512, 1024, 0)
			addLayers(graphBuilder, 17, 1, 1024, 512, 0)
			addLayers(graphBuilder, 18, 3, 512, 1024, 0)

			Dim layerNumber As Integer = 19
			graphBuilder.addLayer("convolution2d_" & layerNumber, (New ConvolutionLayer.Builder(1,1)).nIn(1024).nOut(numClasses).weightInit(WeightInit.XAVIER).stride(1,1).convolutionMode(ConvolutionMode.Same).weightInit(WeightInit.RELU).activation(Activation.IDENTITY).build(), "activation_" & (layerNumber - 1)).addLayer("globalpooling", (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build(), "convolution2d_" & layerNumber).addLayer("softmax", (New ActivationLayer.Builder()).activation(Activation.SOFTMAX).build(), "globalpooling").addLayer("loss", (New LossLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).build(), "softmax").setOutputs("loss")

			Return graphBuilder.build()
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