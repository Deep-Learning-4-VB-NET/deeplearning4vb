Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports GraphBuilder = org.deeplearning4j.nn.conf.ComputationGraphConfiguration.GraphBuilder
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports SpaceToDepthLayer = org.deeplearning4j.nn.conf.layers.SpaceToDepthLayer
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ModelMetaData = org.deeplearning4j.zoo.ModelMetaData
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports org.deeplearning4j.zoo
Imports ZooType = org.deeplearning4j.zoo.ZooType
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class YOLO2 extends org.deeplearning4j.zoo.ZooModel
	Public Class YOLO2
		Inherits ZooModel

		''' <summary>
		''' Default prior boxes for the model
		''' </summary>
		Public Shared ReadOnly DEFAULT_PRIOR_BOXES()() As Double = {
			New Double() {0.57273, 0.677385},
			New Double() {1.87446, 2.06253},
			New Double() {3.33843, 5.47434},
			New Double() {7.88282, 3.52778},
			New Double() {9.77052, 9.16828}
		}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default @Getter private int nBoxes = 5;
		Private nBoxes As Integer = 5
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default @Getter private double[][] priorBoxes = DEFAULT_PRIOR_BOXES;
		Private priorBoxes()() As Double = DEFAULT_PRIOR_BOXES

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long seed = 1234;
		Private seed As Long = 1234
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int[] inputShape = {3, 608, 608};
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputShape_Conflict() As Integer = {3, 608, 608}
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numClasses = 0;
		Private numClasses As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.Adam(1e-3);
		Private updater As IUpdater = New Adam(1e-3)
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
			If pretrainedType = PretrainedType.IMAGENET Then
				Return DL4JResources.getURLString("models/yolo2_dl4j_inference.v3.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				Return 3658373840L
			Else
				Return 0L
			End If
		End Function

		Public Overrides Function modelType() As Type
			Return GetType(ComputationGraph)
		End Function

		Public Overridable Function conf() As ComputationGraphConfiguration
			Dim priors As INDArray = Nd4j.create(priorBoxes)

			Dim graphBuilder As GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).gradientNormalizationThreshold(1.0).updater(updater).l2(0.00001).activation(Activation.IDENTITY).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cudnnAlgoMode(cudnnAlgoMode).graphBuilder().addInputs("input").setInputTypes(InputType.convolutional(inputShape_Conflict(2), inputShape_Conflict(1), inputShape_Conflict(0)))

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

			' #######

			addLayers(graphBuilder, 19, 3, 1024, 1024, 0)
			addLayers(graphBuilder, 20, 3, 1024, 1024, 0)

			' route
			addLayers(graphBuilder, 21, "activation_13", 1, 512, 64, 0, 0)

			' reorg
			graphBuilder.addLayer("rearrange_21",(New SpaceToDepthLayer.Builder(2)).build(), "activation_21").addVertex("concatenate_21", New MergeVertex(), "rearrange_21", "activation_20")

			addLayers(graphBuilder, 22, "concatenate_21", 3, 1024 + 256, 1024, 0, 0)

			graphBuilder.addLayer("convolution2d_23", (New ConvolutionLayer.Builder(1,1)).nIn(1024).nOut(nBoxes * (5 + numClasses)).weightInit(WeightInit.XAVIER).stride(1,1).convolutionMode(ConvolutionMode.Same).weightInit(WeightInit.RELU).activation(Activation.IDENTITY).cudnnAlgoMode(cudnnAlgoMode).build(), "activation_22").addLayer("outputs", (New Yolo2OutputLayer.Builder()).boundingBoxPriors(priors).build(), "convolution2d_23").setOutputs("outputs")

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