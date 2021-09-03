Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
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
Imports org.nd4j.common.primitives
import static org.deeplearning4j.zoo.model.helper.NASNetHelper.normalA
import static org.deeplearning4j.zoo.model.helper.NASNetHelper.reductionA

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
'ORIGINAL LINE: @AllArgsConstructor @Builder public class NASNet extends org.deeplearning4j.zoo.ZooModel
	Public Class NASNet
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
'ORIGINAL LINE: @Builder.@Default private org.deeplearning4j.nn.weights.WeightInit weightInit = org.deeplearning4j.nn.weights.WeightInit.RELU;
		Private weightInit As WeightInit = WeightInit.RELU
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.learning.config.IUpdater updater = new org.nd4j.linalg.learning.config.AdaDelta();
		Private updater As IUpdater = New AdaDelta()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private CacheMode cacheMode = CacheMode.DEVICE;
		Private cacheMode As CacheMode = CacheMode.DEVICE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private WorkspaceMode workspaceMode = WorkspaceMode.ENABLED;
		Private workspaceMode As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ConvolutionLayer.AlgoMode cudnnAlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST;
		Private cudnnAlgoMode As ConvolutionLayer.AlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST

		' NASNet specific
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numBlocks = 6;
		Private numBlocks As Integer = 6
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int penultimateFilters = 1056;
		Private penultimateFilters As Integer = 1056
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int stemFilters = 96;
		Private stemFilters As Integer = 96
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int filterMultiplier = 2;
		Private filterMultiplier As Integer = 2
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean skipReduction = true;
		Private skipReduction As Boolean = True

		Private Sub New()
		End Sub

		Public Overrides Function pretrainedUrl(ByVal pretrainedType As PretrainedType) As String
			If pretrainedType = PretrainedType.IMAGENET Then
				Return DL4JResources.getURLString("models/nasnetmobile_dl4j_inference.v1.zip")
			ElseIf pretrainedType = PretrainedType.IMAGENETLARGE Then
				Return DL4JResources.getURLString("models/nasnetlarge_dl4j_inference.v1.zip")
			Else
				Return Nothing
			End If
		End Function

		Public Overrides Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long
			If pretrainedType = PretrainedType.IMAGENET Then
				Return 3082463801L
			ElseIf pretrainedType = PretrainedType.IMAGENETLARGE Then
				Return 321395591L
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

			If penultimateFilters Mod 24 <> 0 Then
				Throw New System.ArgumentException("For NASNet-A models penultimate filters must be divisible by 24. Current value is " & penultimateFilters)
			End If
			Dim filters As Integer = CInt(Math.Floor(penultimateFilters \ 24))

			Dim graph As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(updater).weightInit(weightInit).l2(5e-5).miniBatch(True).cacheMode(cacheMode).trainingWorkspaceMode(workspaceMode).inferenceWorkspaceMode(workspaceMode).cudnnAlgoMode(cudnnAlgoMode).convolutionMode(ConvolutionMode.Truncate).graphBuilder()

			If Not skipReduction Then
				graph.addLayer("stem_conv1", (New ConvolutionLayer.Builder(3, 3)).stride(2, 2).nOut(stemFilters).hasBias(False).cudnnAlgoMode(cudnnAlgoMode).build(), "input")
			Else
				graph.addLayer("stem_conv1", (New ConvolutionLayer.Builder(3, 3)).stride(1, 1).nOut(stemFilters).hasBias(False).cudnnAlgoMode(cudnnAlgoMode).build(), "input")
			End If

			graph.addLayer("stem_bn1", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), "stem_conv1")

			Dim inputX As String = "stem_bn1"
			Dim inputP As String = Nothing
			If Not skipReduction Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim stem1 As Pair(Of String, String) = reductionA(graph, CInt(Math.Truncate(Math.Floor(stemFilters / Math.Pow(filterMultiplier,2)))), "stem1", "stem_conv1", inputP)
				Dim stem2 As Pair(Of String, String) = reductionA(graph, CInt(Math.Floor(stemFilters \ (filterMultiplier))), "stem2", stem1.First, stem1.Second)
				inputX = stem2.First
				inputP = stem2.Second
			End If

			For i As Integer = 0 To numBlocks - 1
				Dim block As Pair(Of String, String) = normalA(graph, filters, i.ToString(), inputX, inputP)
				inputX = block.First
				inputP = block.Second
			Next i

			Dim inputP0 As String
			Dim reduce As Pair(Of String, String) = reductionA(graph, filters * filterMultiplier, "reduce" & numBlocks, inputX, inputP)
			inputX = reduce.First
			inputP0 = reduce.Second

			If Not skipReduction Then
				inputP = inputP0
			End If

			For i As Integer = 0 To numBlocks - 1
				Dim block As Pair(Of String, String) = normalA(graph, filters * filterMultiplier, (i+numBlocks+1).ToString(), inputX, inputP)
				inputX = block.First
				inputP = block.Second
			Next i

			reduce = reductionA(graph, filters * CInt(Math.Truncate(Math.Pow(filterMultiplier, 2))), "reduce" & (2*numBlocks), inputX, inputP)
			inputX = reduce.First
			inputP0 = reduce.Second

			If Not skipReduction Then
				inputP = inputP0
			End If

			For i As Integer = 0 To numBlocks - 1
				Dim block As Pair(Of String, String) = normalA(graph, filters * CInt(Math.Truncate(Math.Pow(filterMultiplier, 2))), (i+(2*numBlocks)+1).ToString(), inputX, inputP)
				inputX = block.First
				inputP = block.Second
			Next i

			' output
			graph.addLayer("act", New ActivationLayer(Activation.RELU), inputX).addLayer("avg_pool", (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build(), "act").addLayer("output", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).build(), "avg_pool").setOutputs("output")

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