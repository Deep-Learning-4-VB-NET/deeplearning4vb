Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.cuda.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports CudnnLSTMHelper = org.deeplearning4j.cuda.recurrent.CudnnLSTMHelper
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda.lstm


	''' <summary>
	''' Created by Alex on 18/07/2017.
	''' </summary>
	Public Class ValidateCudnnLSTM
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateImplSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub validateImplSimple()

			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim inputSize As Integer = 3
			Dim lstmLayerSize As Integer = 4
			Dim timeSeriesLength As Integer = 3
			Dim nOut As Integer = 2
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, inputSize, timeSeriesLength})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut, timeSeriesLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				For j As Integer = 0 To timeSeriesLength - 1
					labels.putScalar(i, r.Next(nOut), j, 1.0)
				Next j
			Next i

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).inferenceWorkspaceMode(WorkspaceMode.NONE).trainingWorkspaceMode(WorkspaceMode.NONE).updater(New NoOp()).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New LSTM.Builder()).nIn(input.size(1)).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build()).build()

			Dim mln1 As New MultiLayerNetwork(conf.clone())
			mln1.init()

			Dim mln2 As New MultiLayerNetwork(conf.clone())
			mln2.init()


			assertEquals(mln1.params(), mln2.params())

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.recurrent.LSTM).getDeclaredField("helper")
			f.setAccessible(True)

			Dim l0 As Layer = mln1.getLayer(0)
			f.set(l0, Nothing)
			assertNull(f.get(l0))

			l0 = mln2.getLayer(0)
			assertTrue(TypeOf f.get(l0) Is CudnnLSTMHelper)


			Dim out1 As INDArray = mln1.output(input)
			Dim out2 As INDArray = mln2.output(input)

			assertEquals(out1, out2)


			mln1.Input = input
			mln1.Labels = labels

			mln2.Input = input
			mln2.Labels = labels

			mln1.computeGradientAndScore()
			mln2.computeGradientAndScore()

			assertEquals(mln1.score(), mln2.score(), 1e-5)

			Dim g1 As Gradient = mln1.gradient()
			Dim g2 As Gradient = mln2.gradient()

			For Each entry As KeyValuePair(Of String, INDArray) In g1.gradientForVariable().SetOfKeyValuePairs()
				Dim exp As INDArray = entry.Value
				Dim act As INDArray = g2.gradientForVariable()(entry.Key)

				'System.out.println(entry.getKey() + "\t" + exp.equals(act));
			Next entry

			assertEquals(mln1.getFlattenedGradients(), mln2.getFlattenedGradients())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateImplMultiLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub validateImplMultiLayer()

			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim inputSize As Integer = 3
			Dim lstmLayerSize As Integer = 4
			Dim timeSeriesLength As Integer = 3
			Dim nOut As Integer = 2
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, inputSize, timeSeriesLength})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut, timeSeriesLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				For j As Integer = 0 To timeSeriesLength - 1
					labels.putScalar(i, r.Next(nOut), j, 1.0)
				Next j
			Next i

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).inferenceWorkspaceMode(WorkspaceMode.NONE).trainingWorkspaceMode(WorkspaceMode.NONE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New LSTM.Builder()).nIn(input.size(1)).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(1, (New LSTM.Builder()).nIn(lstmLayerSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build()).build()

			Dim mln1 As New MultiLayerNetwork(conf.clone())
			mln1.init()

			Dim mln2 As New MultiLayerNetwork(conf.clone())
			mln2.init()


			assertEquals(mln1.params(), mln2.params())

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.recurrent.LSTM).getDeclaredField("helper")
			f.setAccessible(True)

			Dim l0 As Layer = mln1.getLayer(0)
			Dim l1 As Layer = mln1.getLayer(1)
			f.set(l0, Nothing)
			f.set(l1, Nothing)
			assertNull(f.get(l0))
			assertNull(f.get(l1))

			l0 = mln2.getLayer(0)
			l1 = mln2.getLayer(1)
			assertTrue(TypeOf f.get(l0) Is CudnnLSTMHelper)
			assertTrue(TypeOf f.get(l1) Is CudnnLSTMHelper)


			Dim out1 As INDArray = mln1.output(input)
			Dim out2 As INDArray = mln2.output(input)

			assertEquals(out1, out2)

			For x As Integer = 0 To 9
				input = Nd4j.rand(New Integer() {minibatch, inputSize, timeSeriesLength})
				labels = Nd4j.zeros(minibatch, nOut, timeSeriesLength)
				For i As Integer = 0 To minibatch - 1
					For j As Integer = 0 To timeSeriesLength - 1
						labels.putScalar(i, r.Next(nOut), j, 1.0)
					Next j
				Next i

				mln1.Input = input
				mln1.Labels = labels

				mln2.Input = input
				mln2.Labels = labels

				mln1.computeGradientAndScore()
				mln2.computeGradientAndScore()

				assertEquals(mln1.score(), mln2.score(), 1e-5)

				assertEquals(mln1.getFlattenedGradients(), mln2.getFlattenedGradients())

				mln1.fit(New DataSet(input, labels))
				mln2.fit(New DataSet(input, labels))

				assertEquals("Iteration: " & x, mln1.params(), DirectCast(mln2.params(), System.Func(Of String)))
			Next x
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateImplMultiLayerTBPTT() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub validateImplMultiLayerTBPTT()

			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim inputSize As Integer = 3
			Dim lstmLayerSize As Integer = 4
			Dim timeSeriesLength As Integer = 23
			Dim tbpttLength As Integer = 5
			Dim nOut As Integer = 2

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).inferenceWorkspaceMode(WorkspaceMode.NONE).trainingWorkspaceMode(WorkspaceMode.NONE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New LSTM.Builder()).nIn(inputSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(1, (New LSTM.Builder()).nIn(lstmLayerSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTLength(tbpttLength).build()

			Dim mln1 As New MultiLayerNetwork(conf.clone())
			mln1.init()

			Dim mln2 As New MultiLayerNetwork(conf.clone())
			mln2.init()


			assertEquals(mln1.params(), mln2.params())

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.recurrent.LSTM).getDeclaredField("helper")
			f.setAccessible(True)

			Dim l0 As Layer = mln1.getLayer(0)
			Dim l1 As Layer = mln1.getLayer(1)
			f.set(l0, Nothing)
			f.set(l1, Nothing)
			assertNull(f.get(l0))
			assertNull(f.get(l1))

			l0 = mln2.getLayer(0)
			l1 = mln2.getLayer(1)
			assertTrue(TypeOf f.get(l0) Is CudnnLSTMHelper)
			assertTrue(TypeOf f.get(l1) Is CudnnLSTMHelper)

			Dim r As New Random(123456)
			For x As Integer = 0 To 0
				Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, inputSize, timeSeriesLength})
				Dim labels As INDArray = Nd4j.zeros(minibatch, nOut, timeSeriesLength)
				For i As Integer = 0 To minibatch - 1
					For j As Integer = 0 To timeSeriesLength - 1
						labels.putScalar(i, r.Next(nOut), j, 1.0)
					Next j
				Next i

				Dim ds As New DataSet(input, labels)
				mln1.fit(ds)
				mln2.fit(ds)
			Next x

			assertEquals(mln1.params(), mln2.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateImplMultiLayerRnnTimeStep() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub validateImplMultiLayerRnnTimeStep()

			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}
				Nd4j.Random.setSeed(12345)
				Dim minibatch As Integer = 10
				Dim inputSize As Integer = 3
				Dim lstmLayerSize As Integer = 4
				Dim timeSeriesLength As Integer = 3
				Dim tbpttLength As Integer = 5
				Dim nOut As Integer = 2

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).inferenceWorkspaceMode(WorkspaceMode.NONE).trainingWorkspaceMode(WorkspaceMode.NONE).cacheMode(CacheMode.NONE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New LSTM.Builder()).nIn(inputSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(1, (New LSTM.Builder()).nIn(lstmLayerSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTLength(tbpttLength).build()

				Dim mln1 As New MultiLayerNetwork(conf.clone())
				mln1.init()

				Dim mln2 As New MultiLayerNetwork(conf.clone())
				mln2.init()


				assertEquals(mln1.params(), mln2.params())

				Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.recurrent.LSTM).getDeclaredField("helper")
				f.setAccessible(True)

				Dim l0 As Layer = mln1.getLayer(0)
				Dim l1 As Layer = mln1.getLayer(1)
				f.set(l0, Nothing)
				f.set(l1, Nothing)
				assertNull(f.get(l0))
				assertNull(f.get(l1))

				l0 = mln2.getLayer(0)
				l1 = mln2.getLayer(1)
				assertTrue(TypeOf f.get(l0) Is CudnnLSTMHelper)
				assertTrue(TypeOf f.get(l1) Is CudnnLSTMHelper)

				Dim r As New Random(12345)
				For x As Integer = 0 To 4
					Dim input As INDArray = Nd4j.rand(New Integer(){minibatch, inputSize, timeSeriesLength})

					Dim step1 As INDArray = mln1.rnnTimeStep(input)
					Dim step2 As INDArray = mln2.rnnTimeStep(input)

					assertEquals("Step: " & x, step1, DirectCast(step2, System.Func(Of String)))
				Next x

				assertEquals(mln1.params(), mln2.params())

				'Also check fit (mainly for workspaces sanity check):
				Dim [in] As INDArray = Nd4j.rand(New Integer(){minibatch, inputSize, 3 * tbpttLength})
				Dim label As INDArray = TestUtils.randomOneHotTimeSeries(minibatch, nOut, 3 * tbpttLength)
				For i As Integer = 0 To 2
					mln1.fit([in], label)
					mln2.fit([in], label)
				Next i
			Next wsm
		End Sub
	End Class

End Namespace