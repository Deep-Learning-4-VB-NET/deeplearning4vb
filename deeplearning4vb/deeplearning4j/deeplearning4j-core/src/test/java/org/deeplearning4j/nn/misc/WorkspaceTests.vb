Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports org.deeplearning4j.nn.conf
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WSTestDataSetIterator = org.deeplearning4j.nn.misc.iter.WSTestDataSetIterator
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.nn.misc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) @Tag(TagNames.WORKSPACES) public class WorkspaceTests extends org.deeplearning4j.BaseDL4JTest
	Public Class WorkspaceTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkScopesTestCGAS() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub checkScopesTestCGAS()
			Dim c As ComputationGraph = createNet()
			For Each wm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}
				log.info("Starting test: {}", wm)
				c.Configuration.setTrainingWorkspaceMode(wm)
				c.Configuration.setInferenceWorkspaceMode(wm)

				Dim f As INDArray = Nd4j.rand(New Integer(){8, 1, 28, 28})
				Dim l As INDArray = Nd4j.rand(8, 10)
				c.Inputs = f
				c.Labels = l

				c.computeGradientAndScore()
			Next wm
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWorkspaceIndependence()
		Public Overridable Sub testWorkspaceIndependence()
			'https://github.com/eclipse/deeplearning4j/issues/4337
			Dim depthIn As Integer = 2
			Dim depthOut As Integer = 2
			Dim nOut As Integer = 2

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(depthOut).kernelSize(2, 2).stride(1, 1).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(5, 5, 2)).build()

			Dim net As New MultiLayerNetwork(conf.clone())
			net.init()
			net.LayerWiseConfigurations.setInferenceWorkspaceMode(WorkspaceMode.ENABLED)
			net.LayerWiseConfigurations.setTrainingWorkspaceMode(WorkspaceMode.ENABLED)

			Dim net2 As New MultiLayerNetwork(conf.clone())
			net2.init()
			net2.LayerWiseConfigurations.setInferenceWorkspaceMode(WorkspaceMode.NONE)
			net2.LayerWiseConfigurations.setTrainingWorkspaceMode(WorkspaceMode.NONE)

			Dim [in] As INDArray = Nd4j.rand(New Integer(){1, 2, 5, 5})

			net.output([in])
			net2.output([in]) 'Op [add_scalar] X argument uses leaked workspace pointer from workspace [LOOP_EXTERNAL]
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph createNet() throws Exception
		Public Shared Function createNet() As ComputationGraph

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2, 2).stride(2, 2).build(), "in").addLayer("1", (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2, 2).stride(2, 2).build(), "0").addLayer("out", (New OutputLayer.Builder()).nOut(10).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "1").setOutputs("out").setInputTypes(InputType.convolutional(28, 28, 1)).build()

			Dim model As New ComputationGraph(conf)
			model.init()

			Return model
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWithPreprocessorsCG()
		Public Overridable Sub testWithPreprocessorsCG()
			'https://github.com/eclipse/deeplearning4j/issues/4347
			'Cause for the above issue was layerVertex.setInput() applying the preprocessor, with the result
			' not being detached properly from the workspace...

			For Each wm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				Console.WriteLine(wm)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wm).inferenceWorkspaceMode(wm).graphBuilder().addInputs("in").addLayer("e", (New GravesLSTM.Builder()).nIn(10).nOut(5).build(), New DupPreProcessor(), "in").addLayer("rnn", (New GravesLSTM.Builder()).nIn(5).nOut(8).build(), "e").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.SIGMOID).nOut(3).build(), "rnn").setInputTypes(InputType.recurrent(10)).setOutputs("out").build()

				Dim cg As New ComputationGraph(conf)
				cg.init()


				Dim input() As INDArray = {Nd4j.zeros(1, 10, 5)}

				For Each train As Boolean In New Boolean(){False, True}
					cg.clear()
					cg.feedForward(input, train)
				Next train

				cg.Inputs = input
				cg.Labels = Nd4j.rand(New Integer(){1, 3, 5})
				cg.computeGradientAndScore()
			Next wm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWithPreprocessorsMLN()
		Public Overridable Sub testWithPreprocessorsMLN()
			For Each wm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				Console.WriteLine(wm)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wm).inferenceWorkspaceMode(wm).list().layer((New GravesLSTM.Builder()).nIn(10).nOut(5).build()).layer((New GravesLSTM.Builder()).nIn(5).nOut(8).build()).layer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.SIGMOID).nOut(3).build()).inputPreProcessor(0, New DupPreProcessor()).setInputType(InputType.recurrent(10)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()


				Dim input As INDArray = Nd4j.zeros(1, 10, 5)

				For Each train As Boolean In New Boolean(){False, True}
					net.clear()
					net.feedForward(input, train)
				Next train

				net.Input = input
				net.Labels = Nd4j.rand(New Integer(){1, 3, 5})
				net.computeGradientAndScore()
			Next wm
		End Sub

		<Serializable>
		Public Class DupPreProcessor
			Implements InputPreProcessor

			Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal mgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
				Return mgr.dup(ArrayType.ACTIVATIONS, input)
			End Function

			Public Overridable Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
				Return workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, output)
			End Function

			Public Overridable Function clone() As InputPreProcessor Implements InputPreProcessor.clone
				Return New DupPreProcessor()
			End Function

			Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
				Return inputType
			End Function

			Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStep()
		Public Overridable Sub testRnnTimeStep()
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				For i As Integer = 0 To 2

					Console.WriteLine("Starting test: " & ws & " - " & i)

					Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).list()

					Dim gb As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).graphBuilder().addInputs("in")

					Select Case i
						Case 0
							b.layer((New SimpleRnn.Builder()).nIn(10).nOut(10).build())
							b.layer((New SimpleRnn.Builder()).nIn(10).nOut(10).build())

							gb.addLayer("0", (New SimpleRnn.Builder()).nIn(10).nOut(10).build(), "in")
							gb.addLayer("1", (New SimpleRnn.Builder()).nIn(10).nOut(10).build(), "0")
						Case 1
							b.layer((New LSTM.Builder()).nIn(10).nOut(10).build())
							b.layer((New LSTM.Builder()).nIn(10).nOut(10).build())

							gb.addLayer("0", (New LSTM.Builder()).nIn(10).nOut(10).build(), "in")
							gb.addLayer("1", (New LSTM.Builder()).nIn(10).nOut(10).build(), "0")
						Case 2
							b.layer((New GravesLSTM.Builder()).nIn(10).nOut(10).build())
							b.layer((New GravesLSTM.Builder()).nIn(10).nOut(10).build())

							gb.addLayer("0", (New GravesLSTM.Builder()).nIn(10).nOut(10).build(), "in")
							gb.addLayer("1", (New GravesLSTM.Builder()).nIn(10).nOut(10).build(), "0")
						Case Else
							Throw New Exception()
					End Select

					b.layer((New RnnOutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build())
					gb.addLayer("out", (New RnnOutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "1")
					gb.Outputs = "out"

					Dim conf As MultiLayerConfiguration = b.build()
					Dim conf2 As ComputationGraphConfiguration = gb.build()


					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim net2 As New ComputationGraph(conf2)
					net2.init()

					For j As Integer = 0 To 2
						net.rnnTimeStep(Nd4j.rand(New Integer(){3, 10, 5}))
					Next j

					For j As Integer = 0 To 2
						net2.rnnTimeStep(Nd4j.rand(New Integer(){3, 10, 5}))
					Next j
				Next i
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTbpttFit()
		Public Overridable Sub testTbpttFit()
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				For i As Integer = 0 To 2

					Console.WriteLine("Starting test: " & ws & " - " & i)

					Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).list()

					Dim gb As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).graphBuilder().addInputs("in")

					Select Case i
						Case 0
							b.layer((New SimpleRnn.Builder()).nIn(10).nOut(10).build())
							b.layer((New SimpleRnn.Builder()).nIn(10).nOut(10).build())

							gb.addLayer("0", (New SimpleRnn.Builder()).nIn(10).nOut(10).build(), "in")
							gb.addLayer("1", (New SimpleRnn.Builder()).nIn(10).nOut(10).build(), "0")
						Case 1
							b.layer((New LSTM.Builder()).nIn(10).nOut(10).build())
							b.layer((New LSTM.Builder()).nIn(10).nOut(10).build())

							gb.addLayer("0", (New LSTM.Builder()).nIn(10).nOut(10).build(), "in")
							gb.addLayer("1", (New LSTM.Builder()).nIn(10).nOut(10).build(), "0")
						Case 2
							b.layer((New GravesLSTM.Builder()).nIn(10).nOut(10).build())
							b.layer((New GravesLSTM.Builder()).nIn(10).nOut(10).build())

							gb.addLayer("0", (New GravesLSTM.Builder()).nIn(10).nOut(10).build(), "in")
							gb.addLayer("1", (New GravesLSTM.Builder()).nIn(10).nOut(10).build(), "0")
						Case Else
							Throw New Exception()
					End Select

					b.layer((New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build())
					gb.addLayer("out", (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build(), "1")
					gb.Outputs = "out"

					Dim conf As MultiLayerConfiguration = b.backpropType(BackpropType.TruncatedBPTT).tBPTTLength(5).build()

					Dim conf2 As ComputationGraphConfiguration = gb.backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(5).tBPTTBackwardLength(5).build()


					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim net2 As New ComputationGraph(conf2)
					net2.init()

					For j As Integer = 0 To 2
						net.fit(Nd4j.rand(New Integer(){3, 10, 20}), Nd4j.rand(New Integer(){3, 10, 20}))
					Next j

					For j As Integer = 0 To 2
						net2.fit(New DataSet(Nd4j.rand(New Integer(){3, 10, 20}), Nd4j.rand(New Integer(){3, 10, 20})))
					Next j
				Next i
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScalarOutputCase()
		Public Overridable Sub testScalarOutputCase()
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("WorkspaceMode = " & ws)

				Nd4j.Random.setSeed(12345)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).list().layer((New OutputLayer.Builder()).nIn(3).nOut(1).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.XENT).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim input As INDArray = Nd4j.linspace(1, 3, 3, Nd4j.dataType()).reshape(ChrW(1), 3)
				Dim [out] As INDArray = net.output(input)
				Dim out2 As INDArray = net.output(input)

				assertEquals(out2, [out])

				assertFalse([out].Attached)
				assertFalse(out2.Attached)

				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Next ws
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWorkspaceSetting()
		Public Overridable Sub testWorkspaceSetting()

			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).list().layer((New OutputLayer.Builder()).nIn(3).nOut(1).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.SIGMOID).build()).build()

				assertEquals(wsm, conf.getTrainingWorkspaceMode())
				assertEquals(wsm, conf.getInferenceWorkspaceMode())


				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).list().layer((New OutputLayer.Builder()).nIn(3).nOut(1).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

				assertEquals(wsm, conf2.getTrainingWorkspaceMode())
				assertEquals(wsm, conf2.getInferenceWorkspaceMode())
			Next wsm
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClearing()
		Public Overridable Sub testClearing()
			For Each wsm As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam()).inferenceWorkspaceMode(wsm).trainingWorkspaceMode(wsm).graphBuilder().addInputs("in").setInputTypes(InputType.recurrent(200)).addLayer("embeddings", (New EmbeddingLayer.Builder()).nIn(200).nOut(50).build(), "in").addLayer("a", (New GravesLSTM.Builder()).nOut(300).activation(Activation.HARDTANH).build(), "embeddings").addVertex("b", New LastTimeStepVertex("in"), "a").addLayer("c", (New DenseLayer.Builder()).nOut(300).activation(Activation.HARDTANH).build(), "b").addLayer("output", (New LossLayer.Builder()).lossFunction(LossFunctions.LossFunction.COSINE_PROXIMITY).build(), "c").setOutputs("output").build()


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph computationGraph = new org.deeplearning4j.nn.graph.ComputationGraph(config);
				Dim computationGraph As New ComputationGraph(config)
				computationGraph.init()
				computationGraph.setListeners(New ScoreIterationListener(3))

				Dim iterator As New WSTestDataSetIterator()
				computationGraph.fit(iterator)
			Next wsm
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputWorkspace()
		Public Overridable Sub testOutputWorkspace()

			Dim wsName As String = "ExternalTestWorkspace"
			Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(1).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(conf, wsName)

			Dim netConf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).list().layer((New DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(3).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim net As New MultiLayerNetwork(netConf)
			net.init()

			Dim [in] As INDArray = Nd4j.rand(3, 4)

			For i As Integer = 0 To 2
				Try
						Using ws As MemoryWorkspace = workspace.notifyScopeEntered()
						Console.WriteLine("MLN - " & i)
						Dim [out] As INDArray = net.output([in], False, ws)
        
						assertTrue([out].Attached)
						assertEquals(wsName, [out].data().ParentWorkspace.Id)
						End Using
				Catch t As Exception
					fail()
					Throw New Exception(t)
				End Try
				Console.WriteLine("MLN SCOPE ACTIVE: " & i & " - " & workspace.ScopeActive)
				assertFalse(workspace.ScopeActive)
			Next i


			'Same test for ComputationGraph:
			Dim cg As ComputationGraph = net.toComputationGraph()

			For i As Integer = 0 To 2
				Try
						Using ws As MemoryWorkspace = workspace.notifyScopeEntered()
						Console.WriteLine("CG - " & i)
						Dim [out] As INDArray = cg.output(False, ws, [in])(0)
        
						assertTrue([out].Attached)
						assertEquals(wsName, [out].data().ParentWorkspace.Id)
						End Using
				Catch t As Exception
					Throw New Exception(t)
				End Try
				Console.WriteLine("CG SCOPE ACTIVE: " & i & " - " & workspace.ScopeActive)
				assertFalse(workspace.ScopeActive)
			Next i

			Nd4j.WorkspaceManager.printAllocationStatisticsForCurrentThread()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimpleOutputWorkspace()
		Public Overridable Sub testSimpleOutputWorkspace()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.memory.MemoryWorkspace workspace = org.nd4j.linalg.factory.Nd4j.getWorkspaceManager().getWorkspaceForCurrentThread("ExternalTestWorkspace");
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("ExternalTestWorkspace")

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.rand(1, 30);
			Dim input As INDArray = Nd4j.rand(1, 30)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ComputationGraphConfiguration computationGraphConfiguration = new NeuralNetConfiguration.Builder().graphBuilder().addInputs("state").addLayer("value_output", new OutputLayer.Builder().nIn(30).nOut(1).activation(org.nd4j.linalg.activations.Activation.IDENTITY).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build(), "state").setOutputs("value_output").build();
'JAVA TO VB CONVERTER NOTE: The variable computationGraphConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim computationGraphConfiguration_Conflict As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("state").addLayer("value_output", (New OutputLayer.Builder()).nIn(30).nOut(1).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build(), "state").setOutputs("value_output").build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph computationGraph = new org.deeplearning4j.nn.graph.ComputationGraph(computationGraphConfiguration);
			Dim computationGraph As New ComputationGraph(computationGraphConfiguration_Conflict)
			computationGraph.init()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: try (final org.nd4j.linalg.api.memory.MemoryWorkspace ws = workspace.notifyScopeEntered())
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				computationGraph.output(False, ws, input)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimpleOutputWorkspaceMLN()
		Public Overridable Sub testSimpleOutputWorkspaceMLN()
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("ExternalTestWorkspace")

			Dim input As INDArray = Nd4j.rand(1, 30)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(30).nOut(1).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				net.output(input, False, ws)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkScoreScopeOutMLN()
		Public Overridable Sub checkScoreScopeOutMLN()

			Dim wsName As String = "WSScopeOutTest"
			Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(1).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()



			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(1).nOut(2).kernelSize(2, 2).stride(1, 1).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutional(5, 5, 1)).build()

			Dim net As New MultiLayerNetwork(mlc)
			net.init()


			For Each wm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}
				log.info("Starting test: {}", wm)
				mlc.setTrainingWorkspaceMode(wm)
				mlc.setInferenceWorkspaceMode(wm)

				Dim f As INDArray = Nd4j.rand(New Integer(){1, 1, 5, 5})
				Dim l As INDArray = Nd4j.rand(1, 10)

				Dim ds As New DataSet(f,l)

				For i As Integer = 0 To 9
					Using wsExternal As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, wsName)
						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							Dim s As Double = net.score(ds)
						End Using
					End Using
				Next i

				For i As Integer = 0 To 9
					Using wsExternal As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, wsName)
						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							Dim s As INDArray = net.scoreExamples(ds, True)
							assertFalse(s.Attached)
						End Using
					End Using
				Next i
			Next wm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkScoreScopeOutCG() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub checkScoreScopeOutCG()

			Dim wsName As String = "WSScopeOutTest"
			Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(1).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

			Dim c As ComputationGraph = createNet()
			For Each wm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}
				log.info("Starting test: {}", wm)
				c.Configuration.setTrainingWorkspaceMode(wm)
				c.Configuration.setInferenceWorkspaceMode(wm)

				Dim f As INDArray = Nd4j.rand(New Integer(){8, 1, 28, 28})
				Dim l As INDArray = Nd4j.rand(8, 10)

				Dim ds As New DataSet(f,l)

				For i As Integer = 0 To 9
					Using wsExternal As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, wsName)
						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							Dim s As Double = c.score(ds)
						End Using
					End Using
				Next i

				For i As Integer = 0 To 9
					Using wsExternal As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, wsName)
						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							Dim s As INDArray = c.scoreExamples(ds, True)
							assertFalse(s.Attached)
						End Using
					End Using
				Next i
			Next wm
		End Sub
	End Class

End Namespace