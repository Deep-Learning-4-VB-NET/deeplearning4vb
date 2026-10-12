Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports SDLossMAE = org.deeplearning4j.gradientcheck.sdlosscustom.SDLossMAE
Imports SDLossMSE = org.deeplearning4j.gradientcheck.sdlosscustom.SDLossMSE
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.linalg.lossfunctions.impl
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Tag(TagNames.LOSS_FUNCTIONS) public class LossFunctionGradientCheck extends org.deeplearning4j.BaseDL4JTest
	Public Class LossFunctionGradientCheck
		Inherits BaseDL4JTest

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-5
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void lossFunctionGradientCheck()
		Public Overridable Sub lossFunctionGradientCheck()
			Dim lossFunctions() As ILossFunction = {
				New LossBinaryXENT(),
				New LossBinaryXENT(),
				New LossCosineProximity(),
				New LossHinge(),
				New LossKLD(),
				New LossKLD(),
				New LossL1(),
				New LossL1(),
				New LossL1(),
				New LossL2(),
				New LossL2(),
				New LossMAE(),
				New LossMAE(),
				New LossMAPE(),
				New LossMAPE(),
				New LossMCXENT(),
				New LossMSE(),
				New LossMSE(),
				New LossMSLE(),
				New LossMSLE(),
				New LossNegativeLogLikelihood(),
				New LossNegativeLogLikelihood(),
				New LossPoisson(),
				New LossSquaredHinge(),
				New LossFMeasure(),
				New LossFMeasure(2.0),
				New LossFMeasure(),
				New LossFMeasure(2.0),
				LossMixtureDensity.builder().gaussians(2).labelWidth(3).build(),
				LossMixtureDensity.builder().gaussians(2).labelWidth(3).build(),
				New LossMultiLabel(),
				New LossWasserstein(),
				New LossSparseMCXENT(),
				New SDLossMAE(),
				New SDLossMSE()
			}

			Dim outputActivationFn() As Activation = {Activation.SIGMOID, Activation.SIGMOID, Activation.TANH, Activation.TANH, Activation.SIGMOID, Activation.SOFTMAX, Activation.TANH, Activation.RATIONALTANH, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.TANH, Activation.SIGMOID, Activation.SIGMOID, Activation.SOFTMAX, Activation.SOFTMAX, Activation.IDENTITY, Activation.TANH, Activation.TANH, Activation.IDENTITY, Activation.SOFTMAX, Activation.SOFTMAX, Activation.SIGMOID, Activation.TANH, Activation.SOFTMAX, Activation.SIGMOID, Activation.TANH }

			Dim nOut() As Integer = {1, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 2, 2, 10, 10, 10, 2, 4, 3, 3, 3, 3, 3, 3}

			Dim minibatchSizes() As Integer = {1, 3}


			Dim passed As IList(Of String) = New List(Of String)()
			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To lossFunctions.Length - 1
				For j As Integer = 0 To minibatchSizes.Length - 1
					Dim testName As String = lossFunctions(i) & " - " & outputActivationFn(i) & " - minibatchSize = " & minibatchSizes(j)

					Nd4j.Random.setSeed(12345)
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345).updater(New NoOp()).dist(New UniformDistribution(-2, 2)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).lossFunction(lossFunctions(i)).activation(outputActivationFn(i)).nIn(4).nOut(nOut(i)).build()).validateOutputLayerConfig(False).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim inOut() As INDArray = getFeaturesAndLabels(lossFunctions(i), minibatchSizes(j), 4, nOut(i), 12345)
					Dim input As INDArray = inOut(0)
					Dim labels As INDArray = inOut(1)

					log.info(" ***** Starting test: {} *****", testName)
					'                System.out.println(Arrays.toString(labels.data().asDouble()));
					'                System.out.println(Arrays.toString(net.output(input,false).data().asDouble()));
					'                System.out.println(net.score(new DataSet(input,labels)));

					Dim gradOK As Boolean
					Try
						gradOK = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
					Catch e As Exception
						log.error("",e)
						failed.Add(testName & vbTab & "EXCEPTION")
						Continue For
					End Try

					If gradOK Then
						passed.Add(testName)
					Else
						failed.Add(testName)
					End If
					TestUtils.testModelSerialization(net)
				Next j
			Next i

			If failed.Count > 0 Then
				Console.WriteLine("---- Passed ----")
				For Each s As String In passed
					Console.WriteLine(s)
				Next s
				Console.WriteLine("---- Failed ----")
				For Each s As String In failed
					Console.WriteLine(s)
				Next s
			End If

			assertEquals(0, failed.Count,"Tests failed")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void lossFunctionGradientCheckLossLayer()
		Public Overridable Sub lossFunctionGradientCheckLossLayer()
			Dim lossFunctions() As ILossFunction = {
				New LossBinaryXENT(),
				New LossBinaryXENT(),
				New LossCosineProximity(),
				New LossHinge(),
				New LossKLD(),
				New LossKLD(),
				New LossL1(),
				New LossL1(),
				New LossL2(),
				New LossL2(),
				New LossMAE(),
				New LossMAE(),
				New LossMAPE(),
				New LossMAPE(),
				New LossMCXENT(),
				New LossMSE(),
				New LossMSE(),
				New LossMSLE(),
				New LossMSLE(),
				New LossNegativeLogLikelihood(),
				New LossNegativeLogLikelihood(),
				New LossPoisson(),
				New LossSquaredHinge(),
				New LossFMeasure(),
				New LossFMeasure(2.0),
				New LossFMeasure(),
				New LossFMeasure(2.0),
				LossMixtureDensity.builder().gaussians(2).labelWidth(3).build(),
				LossMixtureDensity.builder().gaussians(2).labelWidth(3).build(),
				New LossMultiLabel(),
				New LossWasserstein(),
				New LossSparseMCXENT()
			}

			Dim outputActivationFn() As Activation = {Activation.SIGMOID, Activation.SIGMOID, Activation.TANH, Activation.TANH, Activation.SIGMOID, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.TANH, Activation.SIGMOID, Activation.SIGMOID, Activation.SOFTMAX, Activation.SOFTMAX, Activation.IDENTITY, Activation.TANH, Activation.TANH, Activation.IDENTITY, Activation.SOFTMAX }

			Dim nOut() As Integer = {1, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 2, 2, 10, 10, 10, 2, 4 }

			Dim minibatchSizes() As Integer = {1, 3}
			'        int[] minibatchSizes = new int[]{3};


			Dim passed As IList(Of String) = New List(Of String)()
			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To lossFunctions.Length - 1
				For j As Integer = 0 To minibatchSizes.Length - 1
					Dim testName As String = lossFunctions(i) & " - " & outputActivationFn(i) & " - minibatchSize = " & minibatchSizes(j)

					' Serialize and de-serialize loss function
					' to ensure that we carry the parameters through
					' the serializer.
					Try
						Dim m As ObjectMapper = NeuralNetConfiguration.mapper()
						Dim s As String = m.writeValueAsString(lossFunctions(i))
						Dim lf2 As ILossFunction = m.readValue(s, lossFunctions(i).GetType())
						lossFunctions(i) = lf2
					Catch ex As IOException
						Console.WriteLine(ex.ToString())
						Console.Write(ex.StackTrace)
						assertEquals(0, 1,"Tests failed: serialization of " & lossFunctions(i))
					End Try
					Nd4j.Random.setSeed(12345)
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345).updater(New NoOp()).dist(New UniformDistribution(-2, 2)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(nOut(i)).activation(Activation.TANH).build()).layer(1, (New LossLayer.Builder()).lossFunction(lossFunctions(i)).activation(outputActivationFn(i)).build()).validateOutputLayerConfig(False).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					assertTrue(CType(net.getLayer(1).conf().getLayer(), LossLayer).getLossFn().GetType() = lossFunctions(i).GetType())

					Dim inOut() As INDArray = getFeaturesAndLabels(lossFunctions(i), minibatchSizes(j), 4, nOut(i), 12345)
					Dim input As INDArray = inOut(0)
					Dim labels As INDArray = inOut(1)

					log.info(" ***** Starting test: {} *****", testName)
					'                System.out.println(Arrays.toString(labels.data().asDouble()));
					'                System.out.println(Arrays.toString(net.output(input,false).data().asDouble()));
					'                System.out.println(net.score(new DataSet(input,labels)));

					Dim gradOK As Boolean
					Try
						gradOK = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
					Catch e As Exception
						log.error("",e)
						failed.Add(testName & vbTab & "EXCEPTION")
						Continue For
					End Try

					If gradOK Then
						passed.Add(testName)
					Else
						failed.Add(testName)
					End If

					TestUtils.testModelSerialization(net)
				Next j
			Next i


			Console.WriteLine("---- Passed ----")
			For Each s As String In passed
				Console.WriteLine(s)
			Next s

			Console.WriteLine("---- Failed ----")
			For Each s As String In failed
				Console.WriteLine(s)
			Next s

			assertEquals(0, failed.Count,"Tests failed")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void lossMultiLabelEdgeCases()
		Public Overridable Sub lossMultiLabelEdgeCases()
			Dim labels As INDArray
			Dim gradientAndScore As Pair(Of Double, INDArray)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.activations.impl.ActivationIdentity activationFn = new org.nd4j.linalg.activations.impl.ActivationIdentity();
			Dim activationFn As New ActivationIdentity()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final LossMultiLabel lossMultiLabel = new LossMultiLabel();
			Dim lossMultiLabel As New LossMultiLabel()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray preOutput = org.nd4j.linalg.factory.Nd4j.rand(3, 3);
			Dim preOutput As INDArray = Nd4j.rand(3, 3)

			' Base Case: Labels are NOT all 1 or 0
			labels = Nd4j.diag(Nd4j.ones(3))
			gradientAndScore = lossMultiLabel.computeGradientAndScore(labels, preOutput, activationFn, Nothing, True)

			assertTrue(Not gradientAndScore.First.isNaN())
			assertTrue(Not gradientAndScore.First.isInfinite())

			' Edge Case: Labels are all 1
			labels = Nd4j.ones(3, 3)
			gradientAndScore = lossMultiLabel.computeGradientAndScore(labels, preOutput, activationFn, Nothing, True)

			assertTrue(Not gradientAndScore.First.isNaN())
			assertTrue(Not gradientAndScore.First.isInfinite())

			' Edge Case: Labels are all 0
			labels = Nd4j.zeros(3, 3)
			gradientAndScore = lossMultiLabel.computeGradientAndScore(labels, preOutput, activationFn, Nothing, True)

			assertTrue(Not gradientAndScore.First.isNaN())
			assertTrue(Not gradientAndScore.First.isInfinite())
		End Sub

		Public Shared Function getFeaturesAndLabels(ByVal l As ILossFunction, ByVal minibatch As Long, ByVal nIn As Long, ByVal nOut As Long, ByVal seed As Long) As INDArray()
			Return getFeaturesAndLabels(l, New Long() {minibatch, nIn}, New Long() {minibatch, nOut}, seed)
		End Function

		Public Shared Function getFeaturesAndLabels(ByVal l As ILossFunction, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer, ByVal seed As Long) As INDArray()
			Return getFeaturesAndLabels(l, ArrayUtil.toLongArray(featuresShape), ArrayUtil.toLongArray(labelsShape), seed)
		End Function

		Public Shared Function getFeaturesAndLabels(ByVal l As ILossFunction, ByVal featuresShape() As Long, ByVal labelsShape() As Long, ByVal seed As Long) As INDArray()
			Nd4j.Random.Seed = seed
			Dim r As New Random(seed)
			Dim ret(1) As INDArray

			ret(0) = Nd4j.rand(featuresShape)

			Select Case l.GetType().Name
				Case "LossBinaryXENT"
					'Want binary vector labels
					ret(1) = Nd4j.rand(labelsShape)
					BooleanIndexing.replaceWhere(ret(1), 0, Conditions.lessThanOrEqual(0.5))
					BooleanIndexing.replaceWhere(ret(1), 1, Conditions.greaterThanOrEqual(0.5))
				Case "LossCosineProximity"
					'Should be real-valued??
					ret(1) = Nd4j.rand(labelsShape).subi(0.5)
				Case "LossKLD"
					'KL divergence: should be a probability distribution for labels??
					ret(1) = Nd4j.rand(labelsShape)
					If labelsShape.Length = 2 Then
						Nd4j.Executioner.exec(New SoftMax(ret(1)))
					ElseIf labelsShape.Length = 3 Then
						Dim i As Integer = 0
						Do While i < labelsShape(2)
							Nd4j.Executioner.exec(New SoftMax(ret(1).get(all(), all(), point(i))))
							i += 1
						Loop
					Else
						Throw New Exception()
					End If
				Case "LossMCXENT", "LossNegativeLogLikelihood"
					ret(1) = Nd4j.zeros(labelsShape)
					If labelsShape.Length = 2 Then
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							ret(1).putScalar(i, r.Next(CInt(labelsShape(1))), 1.0)
							i += 1
						Loop
					ElseIf labelsShape.Length = 3 Then
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							Dim j As Integer = 0
							Do While j < labelsShape(2)
								ret(1).putScalar(i, r.Next(CInt(labelsShape(1))), j, 1.0)
								j += 1
							Loop
							i += 1
						Loop
					Else
						Throw New System.NotSupportedException()
					End If

				Case "LossSparseMCXENT"
					If labelsShape.Length = 2 Then
						ret(1) = Nd4j.create(DataType.INT, labelsShape(0), 1)
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							ret(1).putScalar(i, 0, r.Next(CInt(labelsShape(1))))
							i += 1
						Loop
					ElseIf labelsShape.Length = 3 Then
						ret(1) = Nd4j.create(DataType.INT, labelsShape(0), 1, labelsShape(2))
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							Dim j As Integer = 0
							Do While j < labelsShape(2)
								ret(1).putScalar(i, 0, j, r.Next(CInt(labelsShape(1))))
								j += 1
							Loop
							i += 1
						Loop
					Else
						Throw New System.NotSupportedException()
					End If
				Case "LossHinge", "LossSquaredHinge"
					ret(1) = Nd4j.ones(labelsShape)
					If labelsShape.Length = 2 Then
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							ret(1).putScalar(i, r.Next(CInt(labelsShape(1))), -1.0)
							i += 1
						Loop
					ElseIf labelsShape.Length = 3 Then
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							Dim j As Integer = 0
							Do While j < labelsShape(2)
								ret(1).putScalar(i, r.Next(CInt(labelsShape(1))), j, -1.0)
								j += 1
							Loop
							i += 1
						Loop
					Else
						Throw New System.NotSupportedException()
					End If
				Case "LossMAPE"
					'requires non-zero values for actual...
					ret(1) = Nd4j.rand(labelsShape).addi(1.0) '1 to 2
				Case "LossMAE", "LossMSE", "SDLossMAE", "SDLossMSE", "LossL1", "LossL2"
					ret(1) = Nd4j.rand(labelsShape).muli(2).subi(1)
				Case "LossMSLE"
					'Requires positive labels/activations due to log
					ret(1) = Nd4j.rand(labelsShape)
				Case "LossPoisson"
					'Binary vector labels should be OK here??
					ret(1) = Nd4j.rand(labelsShape)
					BooleanIndexing.replaceWhere(ret(1), 0, Conditions.lessThanOrEqual(0.5))
					BooleanIndexing.replaceWhere(ret(1), 1, Conditions.greaterThanOrEqual(0.5))
				Case "LossFMeasure"
					If labelsShape(1) = 1 Then
						'single binary output case
						ret(1) = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(labelsShape), 0.5))
						If labelsShape(0) >= 2 Then
							'Ensure we have at least one "0" and one "1"
							Dim count As Integer = ret(1).sumNumber().intValue()
							If count = 0 Then
								ret(1).putScalar(0, 0, 1.0)
							ElseIf count = ret(1).size(0) Then
								ret(1).putScalar(0, 0, 0.0)
							End If
						End If
					Else
						'"softmax style" binary output case
						ret(1) = Nd4j.create(labelsShape)
						Dim i As Integer = 0
						Do While i < labelsShape(0)
							ret(1).putScalar(i, i Mod labelsShape(1), 1.0)
							i += 1
						Loop
					End If
				Case "LossMixtureDensity"
					Dim lmd As LossMixtureDensity = DirectCast(l, LossMixtureDensity)
					Dim labelWidth As Integer = lmd.LabelWidth
					ret(1) = Nd4j.rand(New Long() {labelsShape(0), labelWidth})
				Case "LossMultiLabel"
					ret(1) = Nd4j.rand(labelsShape).lt(0.3).castTo(Nd4j.defaultFloatingPointType())
					' ensure that there is no example that is all ones or all zeros
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray sum = ret[1].sum(0);
					Dim sum As INDArray = ret(1).sum(0)
					Dim i As Integer = 0
					Do While i < labelsShape(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int rowSum = sum.getInt(i);
						Dim rowSum As Integer = sum.getInt(i)
						If rowSum = 0 Then
							ret(1).putScalar(i, 0, 1)
						ElseIf rowSum = labelsShape(1) Then
							ret(1).putScalar(i, 0, 0)
						End If
						i += 1
					Loop

				Case "LossWasserstein"
					ret(1) = Nd4j.rand(labelsShape).mul(2).sub(1)

				Case Else
					Throw New System.ArgumentException("Unknown class: " & l.GetType().Name)
			End Select

			Return ret
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void lossFunctionWeightedGradientCheck()
		Public Overridable Sub lossFunctionWeightedGradientCheck()
			Nd4j.Random.setSeed(12345)

			Dim weights() As INDArray = {Nd4j.create(New Double() {0.2, 0.3, 0.5}), Nd4j.create(New Double() {1.0, 0.5, 2.0})}


			Dim passed As IList(Of String) = New List(Of String)()
			Dim failed As IList(Of String) = New List(Of String)()

			For Each w As INDArray In weights

				Dim lossFunctions() As ILossFunction = {
					New LossBinaryXENT(w),
					New LossL1(w),
					New LossL1(w),
					New LossL2(w),
					New LossL2(w),
					New LossMAE(w),
					New LossMAE(w),
					New LossMAPE(w),
					New LossMAPE(w),
					New LossMCXENT(w),
					New LossMSE(w),
					New LossMSE(w),
					New LossMSLE(w),
					New LossMSLE(w),
					New LossNegativeLogLikelihood(w),
					New LossNegativeLogLikelihood(w)
				}

				Dim outputActivationFn() As Activation = {Activation.SIGMOID, Activation.TANH, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX}

				Dim minibatchSizes() As Integer = {1, 3}

				For i As Integer = 0 To lossFunctions.Length - 1
					For j As Integer = 0 To minibatchSizes.Length - 1
						Dim testName As String = lossFunctions(i) & " - " & outputActivationFn(i) & " - minibatchSize = " & minibatchSizes(j) & "; weights = " & w

						Nd4j.Random.setSeed(12345)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345).updater(New NoOp()).dist(New NormalDistribution(0, 1)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).lossFunction(lossFunctions(i)).activation(outputActivationFn(i)).nIn(4).nOut(3).build()).validateOutputLayerConfig(False).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						'Check params to avoid test flakiness on small or large params
						Dim params As INDArray = net.params()
						Dim x As Integer=0
						Do While x<params.length()
							Do While Math.Abs(params.getDouble(x)) < 0.01 OrElse Math.Abs(params.getDouble(x)) > 1.5
								Dim d As Double = Nd4j.Random.nextDouble()
								params.putScalar(x, -1.5 + d * 3)
							Loop
							x += 1
						Loop

						Dim inOut() As INDArray = getFeaturesAndLabels(lossFunctions(i), minibatchSizes(j), 4, 3, 12345)
						Dim input As INDArray = inOut(0)
						Dim labels As INDArray = inOut(1)

						log.info(" ***** Starting test: {} *****", testName)
						'                System.out.println(Arrays.toString(labels.data().asDouble()));
						'                System.out.println(Arrays.toString(net.output(input,false).data().asDouble()));
						'                System.out.println(net.score(new DataSet(input,labels)));

						Dim gradOK As Boolean
						Try
							gradOK = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						Catch e As Exception
							log.error("",e)
							failed.Add(testName & vbTab & "EXCEPTION")
							Continue For
						End Try

						If gradOK Then
							passed.Add(testName)
						Else
							failed.Add(testName)
						End If
					Next j
				Next i
			Next w

			Console.WriteLine("---- Passed ----")
			For Each s As String In passed
				Console.WriteLine(s)
			Next s

			Console.WriteLine("---- Failed ----")
			For Each s As String In failed
				Console.WriteLine(s)
			Next s

			assertEquals(0, failed.Count,"Tests failed")
		End Sub
	End Class

End Namespace