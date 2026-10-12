Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.gradientcheck

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Bn Gradient Check Test") @NativeTag @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) class BNGradientCheckTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class BNGradientCheckTest
		Inherits BaseDL4JTest

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient 2 d Simple") void testGradient2dSimple()
		Friend Overridable Sub testGradient2dSimple()
			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels
			For Each useLogStd As Boolean In New Boolean() { True, False }
				Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 1)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).useLogStd(useLogStd).nOut(3).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.TANH).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build())
				Dim mln As New MultiLayerNetwork(builder.build())
				mln.init()
				' for (int j = 0; j < mln.getnLayers(); j++)
				' System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
				' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
				' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
				Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "1_log10stdev"))
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).excludeParams(excludeParams))
				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient Cnn Simple") void testGradientCnnSimple()
		Friend Overridable Sub testGradientCnnSimple()
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim depth As Integer = 1
			Dim hw As Integer = 4
			Dim nOut As Integer = 4
			Dim input As INDArray = Nd4j.rand(New Integer() { minibatch, depth, hw, hw })
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i
			For Each useLogStd As Boolean In New Boolean() { True, False }
				Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nIn(depth).nOut(2).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).useLogStd(useLogStd).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.TANH).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(hw, hw, depth))
				Dim mln As New MultiLayerNetwork(builder.build())
				mln.init()
				' for (int j = 0; j < mln.getnLayers(); j++)
				' System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
				' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
				' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
				Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "1_log10stdev"))
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).excludeParams(excludeParams))
				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient BN With CN Nand Subsampling") void testGradientBNWithCNNandSubsampling()
		Friend Overridable Sub testGradientBNWithCNNandSubsampling()
			' Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			' (d) l1 and l2 values
			Dim activFns() As Activation = { Activation.SIGMOID, Activation.TANH, Activation.IDENTITY }
			' If true: run some backprop steps first
			Dim characteristic() As Boolean = { True }
			Dim lossFunctions() As LossFunctions.LossFunction = { LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD, LossFunctions.LossFunction.MSE }
			' i.e., lossFunctions[i] used with outputActivations[i] here
			Dim outputActivations() As Activation = { Activation.SOFTMAX, Activation.TANH }
			Dim l2vals() As Double = { 0.0, 0.1, 0.1 }
			' i.e., use l2vals[j] with l1vals[j]
			Dim l1vals() As Double = { 0.0, 0.0, 0.2 }
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 4
			Dim depth As Integer = 2
			Dim hw As Integer = 5
			Dim nOut As Integer = 2
			Dim input As INDArray = Nd4j.rand(New Integer() { minibatch, depth, hw, hw }).muli(5).subi(2.5)
			Dim labels As INDArray = TestUtils.randomOneHot(minibatch, nOut)
			Dim ds As New DataSet(input, labels)
			Dim rng As New Random(12345)
			For Each useLogStd As Boolean In New Boolean() { True, False }
				For Each afn As Activation In activFns
					For Each doLearningFirst As Boolean In characteristic
						For i As Integer = 0 To lossFunctions.Length - 1
							For j As Integer = 0 To l2vals.Length - 1
								' Skip 2 of every 3 tests: from 24 cases to 8, still with decent coverage
								If rng.Next(3) <> 0 Then
									Continue For
								End If
								Dim lf As LossFunctions.LossFunction = lossFunctions(i)
								Dim outputActivation As Activation = outputActivations(i)
								Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).l2(l2vals(j)).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).updater(New NoOp()).dist(New UniformDistribution(-2, 2)).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder(2, 2)).stride(1, 1).nOut(3).activation(afn).build()).layer(1, (New BatchNormalization.Builder()).useLogStd(useLogStd).build()).layer(2, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(1, 1).build()).layer(3, New BatchNormalization()).layer(4, (New ActivationLayer.Builder()).activation(afn).build()).layer(5, (New OutputLayer.Builder(lf)).activation(outputActivation).nOut(nOut).build()).setInputType(InputType.convolutional(hw, hw, depth))
								Dim conf As MultiLayerConfiguration = builder.build()
								Dim mln As New MultiLayerNetwork(conf)
								mln.init()
								Dim name As String = (New ObjectAnonymousInnerClass(Me)).GetType().getEnclosingMethod().getName()
								' System.out.println("Num params: " + mln.numParams());
								If doLearningFirst Then
									' Run a number of iterations of learning
									mln.Input = ds.Features
									mln.Labels = ds.Labels
									mln.computeGradientAndScore()
									Dim scoreBefore As Double = mln.score()
									For k As Integer = 0 To 19
										mln.fit(ds)
									Next k
									mln.computeGradientAndScore()
									Dim scoreAfter As Double = mln.score()
									' Can't test in 'characteristic mode of operation' if not learning
									Dim msg As String = name & " - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst= " & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
									assertTrue(scoreAfter < 0.9 * scoreBefore,msg)
								End If
								Console.WriteLine(name & " - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l1=" & l1vals(j) & ", l2=" & l2vals(j))
								' for (int k = 0; k < mln.getnLayers(); k++)
								' System.out.println("Layer " + k + " # params: " + mln.getLayer(k).numParams());
								' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
								' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
								' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
								Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "3_mean", "3_var", "1_log10stdev", "3_log10stdev"))
								Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).excludeParams(excludeParams).subset(True).maxPerParam(25))
								assertTrue(gradOK)
								TestUtils.testModelSerialization(mln)
							Next j
						Next i
					Next doLearningFirst
				Next afn
			Next useLogStd
		End Sub

		Private Class ObjectAnonymousInnerClass
			Inherits Object

			Private ReadOnly outerInstance As BNGradientCheckTest

			Public Sub New(ByVal outerInstance As BNGradientCheckTest)
				Me.outerInstance = outerInstance
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient Dense") void testGradientDense()
		Friend Overridable Sub testGradientDense()
			' Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			' (d) l1 and l2 values
			Dim activFns() As Activation = { Activation.TANH, Activation.IDENTITY }
			' If true: run some backprop steps first
			Dim characteristic() As Boolean = { True }
			Dim lossFunctions() As LossFunctions.LossFunction = { LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD, LossFunctions.LossFunction.MSE }
			' i.e., lossFunctions[i] used with outputActivations[i] here
			Dim outputActivations() As Activation = { Activation.SOFTMAX, Activation.TANH }
			Dim l2vals() As Double = { 0.0, 0.1 }
			' i.e., use l2vals[j] with l1vals[j]
			Dim l1vals() As Double = { 0.0, 0.2 }
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim nIn As Integer = 5
			Dim nOut As Integer = 3
			Dim input As INDArray = Nd4j.rand(New Integer() { minibatch, nIn })
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i
			Dim ds As New DataSet(input, labels)
			For Each useLogStd As Boolean In New Boolean() { True, False }
				For Each afn As Activation In activFns
					For Each doLearningFirst As Boolean In characteristic
						For i As Integer = 0 To lossFunctions.Length - 1
							For j As Integer = 0 To l2vals.Length - 1
								Dim lf As LossFunctions.LossFunction = lossFunctions(i)
								Dim outputActivation As Activation = outputActivations(i)
								Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).l2(l2vals(j)).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).updater(New NoOp()).dist(New UniformDistribution(-2, 2)).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(4).activation(afn).build()).layer(1, (New BatchNormalization.Builder()).useLogStd(useLogStd).build()).layer(2, (New DenseLayer.Builder()).nIn(4).nOut(4).build()).layer(3, (New BatchNormalization.Builder()).useLogStd(useLogStd).build()).layer(4, (New OutputLayer.Builder(lf)).activation(outputActivation).nOut(nOut).build())
								Dim conf As MultiLayerConfiguration = builder.build()
								Dim mln As New MultiLayerNetwork(conf)
								mln.init()
								Dim name As String = (New ObjectAnonymousInnerClass2(Me)).GetType().getEnclosingMethod().getName()
								If doLearningFirst Then
									' Run a number of iterations of learning
									mln.Input = ds.Features
									mln.Labels = ds.Labels
									mln.computeGradientAndScore()
									Dim scoreBefore As Double = mln.score()
									For k As Integer = 0 To 9
										mln.fit(ds)
									Next k
									mln.computeGradientAndScore()
									Dim scoreAfter As Double = mln.score()
									' Can't test in 'characteristic mode of operation' if not learning
									Dim msg As String = name & " - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst= " & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
									assertTrue(scoreAfter < 0.8 * scoreBefore,msg)
								End If
								Console.WriteLine(name & " - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l1=" & l1vals(j) & ", l2=" & l2vals(j))
								' for (int k = 0; k < mln.getnLayers(); k++)
								' System.out.println("Layer " + k + " # params: " + mln.getLayer(k).numParams());
								' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
								' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
								' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
								Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "3_mean", "3_var", "1_log10stdev", "3_log10stdev"))
								Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).excludeParams(excludeParams))
								assertTrue(gradOK)
								TestUtils.testModelSerialization(mln)
							Next j
						Next i
					Next doLearningFirst
				Next afn
			Next useLogStd
		End Sub

		Private Class ObjectAnonymousInnerClass2
			Inherits Object

			Private ReadOnly outerInstance As BNGradientCheckTest

			Public Sub New(ByVal outerInstance As BNGradientCheckTest)
				Me.outerInstance = outerInstance
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient 2 d Fixed Gamma Beta") void testGradient2dFixedGammaBeta()
		Friend Overridable Sub testGradient2dFixedGammaBeta()
			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels
			For Each useLogStd As Boolean In New Boolean() { True, False }
				Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 1)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).useLogStd(useLogStd).lockGammaBeta(True).gamma(2.0).beta(0.5).nOut(3).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.TANH).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build())
				Dim mln As New MultiLayerNetwork(builder.build())
				mln.init()
				' for (int j = 0; j < mln.getnLayers(); j++)
				' System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
				' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
				' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
				Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "1_log10stdev"))
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).excludeParams(excludeParams))
				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient Cnn Fixed Gamma Beta") void testGradientCnnFixedGammaBeta()
		Friend Overridable Sub testGradientCnnFixedGammaBeta()
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim depth As Integer = 1
			Dim hw As Integer = 4
			Dim nOut As Integer = 4
			Dim input As INDArray = Nd4j.rand(New Integer() { minibatch, depth, hw, hw })
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i
			For Each useLogStd As Boolean In New Boolean() { True, False }
				Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nIn(depth).nOut(2).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).useLogStd(useLogStd).lockGammaBeta(True).gamma(2.0).beta(0.5).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.TANH).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(hw, hw, depth))
				Dim mln As New MultiLayerNetwork(builder.build())
				mln.init()
				' for (int j = 0; j < mln.getnLayers(); j++)
				' System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
				' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
				' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
				Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "1_log10stdev"))
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).excludeParams(excludeParams))
				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Norm Comp Graph Simple") void testBatchNormCompGraphSimple()
		Friend Overridable Sub testBatchNormCompGraphSimple()
			Dim numClasses As Integer = 2
			Dim height As Integer = 3
			Dim width As Integer = 3
			Dim channels As Integer = 1
			Dim seed As Long = 123
			Dim minibatchSize As Integer = 3
			For Each useLogStd As Boolean In New Boolean() { True, False }
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).updater(New NoOp()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").setInputTypes(InputType.convolutional(height, width, channels)).addLayer("bn", (New BatchNormalization.Builder()).useLogStd(useLogStd).build(), "in").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nOut(numClasses).build(), "bn").setOutputs("out").build()
				Dim net As New ComputationGraph(conf)
				net.init()
				Dim r As New Random(12345)
				' Order: examples, channels, height, width
				Dim input As INDArray = Nd4j.rand(New Integer() { minibatchSize, channels, height, width })
				Dim labels As INDArray = Nd4j.zeros(minibatchSize, numClasses)
				For i As Integer = 0 To minibatchSize - 1
					labels.putScalar(New Integer() { i, r.Next(numClasses) }, 1.0)
				Next i
				' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
				' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
				' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
				Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("bn_mean", "bn_var"))
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(net).inputs(New INDArray() { input }).labels(New INDArray() { labels }).excludeParams(excludeParams))
				assertTrue(gradOK)
				TestUtils.testModelSerialization(net)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient BN With CN Nand Subsampling Comp Graph") void testGradientBNWithCNNandSubsamplingCompGraph()
		Friend Overridable Sub testGradientBNWithCNNandSubsamplingCompGraph()
			' Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			' (d) l1 and l2 values
			Dim activFns() As Activation = { Activation.TANH, Activation.IDENTITY }
			Dim doLearningFirst As Boolean = True
			Dim lossFunctions() As LossFunctions.LossFunction = { LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD }
			' i.e., lossFunctions[i] used with outputActivations[i] here
			Dim outputActivations() As Activation = { Activation.SOFTMAX }
			Dim l2vals() As Double = { 0.0, 0.1 }
			' i.e., use l2vals[j] with l1vals[j]
			Dim l1vals() As Double = { 0.0, 0.2 }
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim depth As Integer = 2
			Dim hw As Integer = 5
			Dim nOut As Integer = 3
			Dim input As INDArray = Nd4j.rand(New Integer() { minibatch, depth, hw, hw })
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i
			Dim ds As New DataSet(input, labels)
			For Each useLogStd As Boolean In New Boolean() { True, False }
				For Each afn As Activation In activFns
					For i As Integer = 0 To lossFunctions.Length - 1
						For j As Integer = 0 To l2vals.Length - 1
							Dim lf As LossFunctions.LossFunction = lossFunctions(i)
							Dim outputActivation As Activation = outputActivations(i)
							Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).updater(New NoOp()).dist(New UniformDistribution(-2, 2)).seed(12345L).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder(2, 2)).stride(1, 1).nOut(3).activation(afn).build(), "in").addLayer("1", (New BatchNormalization.Builder()).useLogStd(useLogStd).build(), "0").addLayer("2", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(1, 1).build(), "1").addLayer("3", (New BatchNormalization.Builder()).useLogStd(useLogStd).build(), "2").addLayer("4", (New ActivationLayer.Builder()).activation(afn).build(), "3").addLayer("5", (New OutputLayer.Builder(lf)).activation(outputActivation).nOut(nOut).build(), "4").setOutputs("5").setInputTypes(InputType.convolutional(hw, hw, depth)).build()
							Dim net As New ComputationGraph(conf)
							net.init()
							Dim name As String = (New ObjectAnonymousInnerClass3(Me)).GetType().getEnclosingMethod().getName()
							If doLearningFirst Then
								' Run a number of iterations of learning
								net.setInput(0, ds.Features)
								net.Labels = ds.Labels
								net.computeGradientAndScore()
								Dim scoreBefore As Double = net.score()
								For k As Integer = 0 To 19
									net.fit(ds)
								Next k
								net.computeGradientAndScore()
								Dim scoreAfter As Double = net.score()
								' Can't test in 'characteristic mode of operation' if not learning
								Dim msg As String = name & " - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst= " & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
								assertTrue(scoreAfter < 0.9 * scoreBefore,msg)
							End If
							Console.WriteLine(name & " - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l1=" & l1vals(j) & ", l2=" & l2vals(j))
							' for (int k = 0; k < net.getNumLayers(); k++)
							' System.out.println("Layer " + k + " # params: " + net.getLayer(k).numParams());
							' Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
							' i.e., runningMean = decay * runningMean + (1-decay) * batchMean
							' However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
							Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "3_mean", "3_var", "1_log10stdev", "3_log10stdev"))
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(net).inputs(New INDArray() { input }).labels(New INDArray() { labels }).excludeParams(excludeParams))
							assertTrue(gradOK)
							TestUtils.testModelSerialization(net)
						Next j
					Next i
				Next afn
			Next useLogStd
		End Sub

		Private Class ObjectAnonymousInnerClass3
			Inherits Object

			Private ReadOnly outerInstance As BNGradientCheckTest

			Public Sub New(ByVal outerInstance As BNGradientCheckTest)
				Me.outerInstance = outerInstance
			End Sub

		End Class
	End Class

End Namespace