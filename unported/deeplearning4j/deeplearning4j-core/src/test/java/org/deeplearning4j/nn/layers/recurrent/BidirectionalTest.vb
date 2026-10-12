Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.saver
Imports DataSetLossCalculator = org.deeplearning4j.earlystopping.scorecalc.DataSetLossCalculator
Imports MaxEpochsTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxEpochsTerminationCondition
Imports EarlyStoppingGraphTrainer = org.deeplearning4j.earlystopping.trainer.EarlyStoppingGraphTrainer
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports GravesBidirectionalLSTM = org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports BernoulliReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.BernoulliReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports MultiLayerUpdater = org.deeplearning4j.nn.updater.MultiLayerUpdater
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports RnnDataFormat = org.nd4j.enums.RnnDataFormat
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.deeplearning4j.nn.conf.RNNFormat.NCW
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Bidirectional Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class BidirectionalTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class BidirectionalTest
		Inherits BaseDL4JTest



		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each rnnFormat As RNNFormat In System.Enum.GetValues(GetType(RNNFormat))
					args.Add(Arguments.of(rnnFormat,nd4jBackend))
				Next rnnFormat
			Next nd4jBackend
			Return args.stream()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Compare Implementations") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") void compareImplementations(RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub compareImplementations(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Starting workspace mode: " & wsm)
				' Bidirectional(GravesLSTM) and GravesBidirectionalLSTM should be equivalent, given equivalent params
				' Note that GravesBidirectionalLSTM implements ADD mode only
				Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam()).list().layer(New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build())).layer(New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build())).layer((New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).dataFormat(rnnDataFormat).nIn(10).nOut(10).build()).build()
				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam()).list().layer((New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).layer((New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).layer((New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).dataFormat(rnnDataFormat).nIn(10).nOut(10).build()).build()
				Dim net1 As New MultiLayerNetwork(conf1)
				net1.init()
				Dim net2 As New MultiLayerNetwork(conf2)
				net2.init()
				assertEquals(net1.numParams(), net2.numParams())
				For i As Integer = 0 To 2
					Dim n1 As Integer = CInt(net1.getLayer(i).numParams())
					Dim n2 As Integer = CInt(net2.getLayer(i).numParams())
					assertEquals(n1, n2)
				Next i
				' Assuming exact same layout here...
				net2.Params = net1.params()
				Dim [in] As INDArray
				If rnnDataFormat = NCW Then
					[in] = Nd4j.rand(New Integer() { 3, 10, 5 })
				Else
					[in] = Nd4j.rand(New Integer() { 3, 5, 10 })
				End If
				Dim out1 As INDArray = net1.output([in])
				Dim out2 As INDArray = net2.output([in])
				assertEquals(out1, out2)
				Dim labels As INDArray
				If rnnDataFormat = NCW Then
					labels = Nd4j.rand(New Integer() { 3, 10, 5 })
				Else
					labels = Nd4j.rand(New Integer() { 3, 5, 10 })
				End If
				net1.Input = [in]
				net1.Labels = labels
				net2.Input = [in]
				net2.Labels = labels
				net1.computeGradientAndScore()
				net2.computeGradientAndScore()
				' Ensure scores are equal:
				assertEquals(net1.score(), net2.score(), 1e-6)
				' Ensure gradients are equal:
				Dim g1 As Gradient = net1.gradient()
				Dim g2 As Gradient = net2.gradient()
				assertEquals(g1.gradient(), g2.gradient())
				' Ensure updates are equal:
				Dim u1 As MultiLayerUpdater = DirectCast(net1.Updater, MultiLayerUpdater)
				Dim u2 As MultiLayerUpdater = DirectCast(net2.Updater, MultiLayerUpdater)
				assertEquals(u1.getUpdaterStateViewArray(), u2.getUpdaterStateViewArray())
				u1.update(net1, g1, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
				u2.update(net2, g2, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
				assertEquals(g1.gradient(), g2.gradient())
				assertEquals(u1.getUpdaterStateViewArray(), u2.getUpdaterStateViewArray())
				' Ensure params are equal, after fitting
				net1.fit([in], labels)
				net2.fit([in], labels)
				Dim p1 As INDArray = net1.params()
				Dim p2 As INDArray = net2.params()
				assertEquals(p1, p2)
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Compare Implementations Comp Graph") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") void compareImplementationsCompGraph(RNNFormat rnnFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub compareImplementationsCompGraph(ByVal rnnFormat As RNNFormat, ByVal backend As Nd4jBackend)
			' for(WorkspaceMode wsm : WorkspaceMode.values()) {
			For Each wsm As WorkspaceMode In New WorkspaceMode() { WorkspaceMode.NONE, WorkspaceMode.ENABLED }
				log.info("*** Starting workspace mode: " & wsm)
				' Bidirectional(GravesLSTM) and GravesBidirectionalLSTM should be equivalent, given equivalent params
				' Note that GravesBidirectionalLSTM implements ADD mode only
				Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Adam()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).graphBuilder().addInputs("in").layer("0", New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).build()), "in").layer("1", New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).build()), "0").layer("2", (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
				Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Adam()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).graphBuilder().addInputs("in").layer("0", (New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).build(), "in").layer("1", (New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).build(), "0").layer("2", (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
				Dim net1 As New ComputationGraph(conf1)
				net1.init()
				Dim net2 As New ComputationGraph(conf2)
				net2.init()
				assertEquals(net1.numParams(), net2.numParams())
				For i As Integer = 0 To 2
					Dim n1 As Integer = CInt(net1.getLayer(i).numParams())
					Dim n2 As Integer = CInt(net2.getLayer(i).numParams())
					assertEquals(n1, n2)
				Next i
				' Assuming exact same layout here...
				net2.Params = net1.params()
				Dim [in] As INDArray = Nd4j.rand(New Integer() { 3, 10, 5 })
				Dim out1 As INDArray = net1.outputSingle([in])
				Dim out2 As INDArray = net2.outputSingle([in])
				assertEquals(out1, out2)
				Dim labels As INDArray = Nd4j.rand(New Integer() { 3, 10, 5 })
				net1.setInput(0, [in])
				net1.Labels = labels
				net2.setInput(0, [in])
				net2.Labels = labels
				net1.computeGradientAndScore()
				net2.computeGradientAndScore()
				' Ensure scores are equal:
				assertEquals(net1.score(), net2.score(), 1e-6)
				' Ensure gradients are equal:
				Dim g1 As Gradient = net1.gradient()
				Dim g2 As Gradient = net2.gradient()
				assertEquals(g1.gradient(), g2.gradient())
				' Ensure updates are equal:
				Dim u1 As ComputationGraphUpdater = net1.Updater
				Dim u2 As ComputationGraphUpdater = net2.Updater
				assertEquals(u1.getUpdaterStateViewArray(), u2.getUpdaterStateViewArray())
				u1.update(g1, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
				u2.update(g2, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
				assertEquals(g1.gradient(), g2.gradient())
				assertEquals(u1.getUpdaterStateViewArray(), u2.getUpdaterStateViewArray())
				' Ensure params are equal, after fitting
				net1.fit(New DataSet([in], labels))
				net2.fit(New DataSet([in], labels))
				Dim p1 As INDArray = net1.params()
				Dim p2 As INDArray = net2.params()
				assertEquals(p1, p2)
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Serialization") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") void testSerialization(RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSerialization(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Starting workspace mode: " & wsm)
				Nd4j.Random.setSeed(12345)
				Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam()).list().layer(New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build())).layer(New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build())).layer((New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).build()
				Dim net1 As New MultiLayerNetwork(conf1)
				net1.init()
				Dim [in] As INDArray
				Dim labels As INDArray
				Dim inshape() As Long = If(rnnDataFormat = NCW, New Long() { 3, 10, 5 }, New Long()){ 3, 5, 10 }
				[in] = Nd4j.rand(inshape)
				labels = Nd4j.rand(inshape)
				net1.fit([in], labels)
				Dim bytes() As SByte
				Using baos As New MemoryStream()
					ModelSerializer.writeModel(net1, baos, True)
					bytes = baos.toByteArray()
				End Using
				Dim net2 As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(New MemoryStream(bytes), True)
				[in] = Nd4j.rand(inshape)
				labels = Nd4j.rand(inshape)
				Dim out1 As INDArray = net1.output([in])
				Dim out2 As INDArray = net2.output([in])
				assertEquals(out1, out2)
				net1.Input = [in]
				net2.Input = [in]
				net1.Labels = labels
				net2.Labels = labels
				net1.computeGradientAndScore()
				net2.computeGradientAndScore()
				assertEquals(net1.score(), net2.score(), 1e-6)
				assertEquals(net1.gradient().gradient(), net2.gradient().gradient())
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Serialization Comp Graph") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") void testSerializationCompGraph(RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSerializationCompGraph(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Starting workspace mode: " & wsm)
				Nd4j.Random.setSeed(12345)
				Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam()).graphBuilder().addInputs("in").layer("0", New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()), "in").layer("1", New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()), "0").layer("2", (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).dataFormat(rnnDataFormat).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
				Dim net1 As New ComputationGraph(conf1)
				net1.init()
				Dim inshape() As Long = If(rnnDataFormat = NCW, New Long() { 3, 10, 5 }, New Long()){ 3, 5, 10 }
				Dim [in] As INDArray = Nd4j.rand(inshape)
				Dim labels As INDArray = Nd4j.rand(inshape)
				net1.fit(New DataSet([in], labels))
				Dim bytes() As SByte
				Using baos As New MemoryStream()
					ModelSerializer.writeModel(net1, baos, True)
					bytes = baos.toByteArray()
				End Using
				Dim net2 As ComputationGraph = ModelSerializer.restoreComputationGraph(New MemoryStream(bytes), True)
				[in] = Nd4j.rand(inshape)
				labels = Nd4j.rand(inshape)
				Dim out1 As INDArray = net1.outputSingle([in])
				Dim out2 As INDArray = net2.outputSingle([in])
				assertEquals(out1, out2)
				net1.setInput(0, [in])
				net2.setInput(0, [in])
				net1.Labels = labels
				net2.Labels = labels
				net1.computeGradientAndScore()
				net2.computeGradientAndScore()
				assertEquals(net1.score(), net2.score(), 1e-6)
				assertEquals(net1.gradient().gradient(), net2.gradient().gradient())
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Simple Bidirectional") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") public void testSimpleBidirectional(RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleBidirectional(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Starting workspace mode: " & wsm)
				Nd4j.Random.setSeed(12345)
				Dim modes() As Bidirectional.Mode = { Bidirectional.Mode.CONCAT, Bidirectional.Mode.ADD, Bidirectional.Mode.AVERAGE, Bidirectional.Mode.MUL }
				Dim inshape() As Long = If(rnnDataFormat = NCW, New Long() { 3, 10, 6 }, New Long()){ 3, 6, 10 }
				Dim [in] As INDArray = Nd4j.rand(inshape)
				For Each m As Bidirectional.Mode In modes
					Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).weightInit(WeightInit.XAVIER).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam()).list().layer(New Bidirectional(m, (New SimpleRnn.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build())).build()
					Dim net1 As New MultiLayerNetwork(conf1)
					net1.init()
					Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Adam()).list().layer((New SimpleRnn.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).build()
					Dim net2 As New MultiLayerNetwork(conf2.clone())
					net2.init()
					Dim net3 As New MultiLayerNetwork(conf2.clone())
					net3.init()
					net2.setParam("0_W", net1.getParam("0_fW"))
					net2.setParam("0_RW", net1.getParam("0_fRW"))
					net2.setParam("0_b", net1.getParam("0_fb"))
					net3.setParam("0_W", net1.getParam("0_bW"))
					net3.setParam("0_RW", net1.getParam("0_bRW"))
					net3.setParam("0_b", net1.getParam("0_bb"))
					Dim inReverse As INDArray = TimeSeriesUtils.reverseTimeSeries([in], LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT, rnnDataFormat)
					Dim out1 As INDArray = net1.output([in])
					Dim out2 As INDArray = net2.output([in])
					Dim out3 As INDArray = TimeSeriesUtils.reverseTimeSeries(net3.output(inReverse), LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT, rnnDataFormat)
					Dim outExp As INDArray
					Select Case m
						Case Bidirectional.Mode.ADD
							outExp = out2.add(out3)
						Case Bidirectional.Mode.MUL
							outExp = out2.mul(out3)
						Case Bidirectional.Mode.AVERAGE
							outExp = out2.add(out3).muli(0.5)
						Case Bidirectional.Mode.CONCAT
							outExp = Nd4j.concat(If(rnnDataFormat = NCW, 1, 2), out2, out3)
						Case Else
							Throw New Exception()
					End Select
					assertEquals(outExp, out1,m.ToString())
					' Check gradients:
					If m = Bidirectional.Mode.ADD OrElse m = Bidirectional.Mode.CONCAT Then
						Dim eps As INDArray = Nd4j.rand(inshape)
						Dim eps1 As INDArray
						If m = Bidirectional.Mode.CONCAT Then
							eps1 = Nd4j.concat(If(rnnDataFormat = NCW, 1, 2), eps, eps)
						Else
							eps1 = eps
						End If
						net1.Input = [in]
						net2.Input = [in]
						net3.Input = TimeSeriesUtils.reverseTimeSeries([in], LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT, rnnDataFormat)
						net1.feedForward(True, False)
						net2.feedForward(True, False)
						net3.feedForward(True, False)
						Dim p1 As Pair(Of Gradient, INDArray) = net1.backpropGradient(eps1, LayerWorkspaceMgr.noWorkspaces())
						Dim p2 As Pair(Of Gradient, INDArray) = net2.backpropGradient(eps, LayerWorkspaceMgr.noWorkspaces())
						Dim p3 As Pair(Of Gradient, INDArray) = net3.backpropGradient(TimeSeriesUtils.reverseTimeSeries(eps, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT, rnnDataFormat), LayerWorkspaceMgr.noWorkspaces())
						Dim g1 As Gradient = p1.First
						Dim g2 As Gradient = p2.First
						Dim g3 As Gradient = p3.First
						For Each updates As Boolean In New Boolean() { False, True }
							If updates Then
								net1.Updater.update(net1, g1, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
								net2.Updater.update(net2, g2, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
								net3.Updater.update(net3, g3, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
							End If
							assertEquals(g2.gradientForVariable()("0_W"), g1.gradientForVariable()("0_fW"))
							assertEquals(g2.gradientForVariable()("0_RW"), g1.gradientForVariable()("0_fRW"))
							assertEquals(g2.gradientForVariable()("0_b"), g1.gradientForVariable()("0_fb"))
							assertEquals(g3.gradientForVariable()("0_W"), g1.gradientForVariable()("0_bW"))
							assertEquals(g3.gradientForVariable()("0_RW"), g1.gradientForVariable()("0_bRW"))
							assertEquals(g3.gradientForVariable()("0_b"), g1.gradientForVariable()("0_bb"))
						Next updates
					End If
				Next m
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Simple Bidirectional Comp Graph") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") void testSimpleBidirectionalCompGraph(RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSimpleBidirectionalCompGraph(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Starting workspace mode: " & wsm)
				Nd4j.Random.setSeed(12345)
				Dim modes() As Bidirectional.Mode = { Bidirectional.Mode.CONCAT, Bidirectional.Mode.ADD, Bidirectional.Mode.AVERAGE, Bidirectional.Mode.MUL }
				Dim inshape() As Long = If(rnnDataFormat = NCW, New Long() { 3, 10, 6 }, New Long()){ 3, 6, 10 }
				Dim [in] As INDArray = Nd4j.rand(inshape)
				For Each m As Bidirectional.Mode In modes
					Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).weightInit(WeightInit.XAVIER).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam()).graphBuilder().addInputs("in").layer("0", New Bidirectional(m, (New SimpleRnn.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()), "in").setOutputs("0").build()
					Dim net1 As New ComputationGraph(conf1)
					net1.init()
					Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Adam()).graphBuilder().addInputs("in").layer("0", (New SimpleRnn.Builder()).nIn(10).nOut(10).dataFormat(rnnDataFormat).build(), "in").setOutputs("0").build()
					Dim net2 As New ComputationGraph(conf2.clone())
					net2.init()
					Dim net3 As New ComputationGraph(conf2.clone())
					net3.init()
					net2.setParam("0_W", net1.getParam("0_fW"))
					net2.setParam("0_RW", net1.getParam("0_fRW"))
					net2.setParam("0_b", net1.getParam("0_fb"))
					net3.setParam("0_W", net1.getParam("0_bW"))
					net3.setParam("0_RW", net1.getParam("0_bRW"))
					net3.setParam("0_b", net1.getParam("0_bb"))
					Dim out1 As INDArray = net1.outputSingle([in])
					Dim out2 As INDArray = net2.outputSingle([in])
					Dim out3 As INDArray
					Dim inReverse As INDArray
					If rnnDataFormat = RNNFormat.NWC Then
						inReverse = TimeSeriesUtils.reverseTimeSeries([in].permute(0, 2, 1), LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT).permute(0, 2, 1)
						out3 = net3.outputSingle(inReverse)
						out3 = TimeSeriesUtils.reverseTimeSeries(out3.permute(0, 2, 1), LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT).permute(0, 2, 1)
					Else
						inReverse = TimeSeriesUtils.reverseTimeSeries([in], LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT)
						out3 = net3.outputSingle(inReverse)
						out3 = TimeSeriesUtils.reverseTimeSeries(out3, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT)
					End If
					Dim outExp As INDArray
					Select Case m
						Case Bidirectional.Mode.ADD
							outExp = out2.add(out3)
						Case Bidirectional.Mode.MUL
							outExp = out2.mul(out3)
						Case Bidirectional.Mode.AVERAGE
							outExp = out2.add(out3).muli(0.5)
						Case Bidirectional.Mode.CONCAT
							Console.WriteLine(out2.shapeInfoToString())
							Console.WriteLine(out3.shapeInfoToString())
							outExp = Nd4j.concat(If(rnnDataFormat = NCW, 1, 2), out2, out3)
						Case Else
							Throw New Exception()
					End Select
					assertEquals(outExp, out1,m.ToString())
					' Check gradients:
					If m = Bidirectional.Mode.ADD OrElse m = Bidirectional.Mode.CONCAT Then
						Dim eps As INDArray = Nd4j.rand(inshape)
						Dim eps1 As INDArray
						If m = Bidirectional.Mode.CONCAT Then
							eps1 = Nd4j.concat(If(rnnDataFormat = NCW, 1, 2), eps, eps)
						Else
							eps1 = eps
						End If
						Dim epsReversed As INDArray = If(rnnDataFormat = NCW, TimeSeriesUtils.reverseTimeSeries(eps, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT), TimeSeriesUtils.reverseTimeSeries(eps.permute(0, 2, 1), LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT).permute(0, 2, 1))
						net1.outputSingle(True, False, [in])
						net2.outputSingle(True, False, [in])
						net3.outputSingle(True, False, inReverse)
						Dim g1 As Gradient = net1.backpropGradient(eps1)
						Dim g2 As Gradient = net2.backpropGradient(eps)
						Dim g3 As Gradient = net3.backpropGradient(epsReversed)
						For Each updates As Boolean In New Boolean() { False, True }
							If updates Then
								net1.Updater.update(g1, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
								net2.Updater.update(g2, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
								net3.Updater.update(g3, 0, 0, 3, LayerWorkspaceMgr.noWorkspaces())
							End If
							assertEquals(g2.gradientForVariable()("0_W"), g1.gradientForVariable()("0_fW"))
							assertEquals(g2.gradientForVariable()("0_RW"), g1.gradientForVariable()("0_fRW"))
							assertEquals(g2.gradientForVariable()("0_b"), g1.gradientForVariable()("0_fb"))
							assertEquals(g3.gradientForVariable()("0_W"), g1.gradientForVariable()("0_bW"))
							assertEquals(g3.gradientForVariable()("0_RW"), g1.gradientForVariable()("0_bRW"))
							assertEquals(g3.gradientForVariable()("0_b"), g1.gradientForVariable()("0_bb"))
						Next updates
					End If
				Next m
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Issue 5472") @MethodSource("org.deeplearning4j.nn.layers.recurrent.BidirectionalTest#params") @ParameterizedTest void testIssue5472(RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testIssue5472(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			' https://github.com/eclipse/deeplearning4j/issues/5472
			Dim [in] As Integer = 2
			Dim [out] As Integer = 2
			Dim builder As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).activation(Activation.RELU).graphBuilder().addInputs("IN").setInputTypes(InputType.recurrent([in])).addLayer("AUTOENCODER", (New VariationalAutoencoder.Builder()).encoderLayerSizes(64).decoderLayerSizes(64).nOut(7).pzxActivationFunction(Activation.IDENTITY).reconstructionDistribution(New BernoulliReconstructionDistribution(Activation.SIGMOID.getActivationFunction())).build(), "IN").addLayer("RNN", New Bidirectional(Bidirectional.Mode.ADD, (New GravesLSTM.Builder()).nOut(128).build()), "AUTOENCODER").addLayer("OUT", (New RnnOutputLayer.Builder()).nOut([out]).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "RNN").setOutputs("OUT")
			Dim net As New ComputationGraph(builder.build())
			net.init()
			Dim iterator As MultiDataSetIterator = New SingletonMultiDataSetIterator(New MultiDataSet(Nd4j.create(10, [in], 5), Nd4j.create(10, [out], 5)))
			Dim b As EarlyStoppingConfiguration.Builder = (New EarlyStoppingConfiguration.Builder(Of )()).epochTerminationConditions(New MaxEpochsTerminationCondition(10)).scoreCalculator(New DataSetLossCalculator(iterator, True)).evaluateEveryNEpochs(1).modelSaver(New InMemoryModelSaver(Of )())
			Dim earlyStoppingGraphTrainer As New EarlyStoppingGraphTrainer(b.build(), net, iterator, Nothing)
			earlyStoppingGraphTrainer.fit()
		End Sub
	End Class

End Namespace