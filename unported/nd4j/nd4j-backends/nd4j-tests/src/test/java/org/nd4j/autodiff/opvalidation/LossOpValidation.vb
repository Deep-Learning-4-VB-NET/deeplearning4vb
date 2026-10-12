Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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

Namespace org.nd4j.autodiff.opvalidation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class LossOpValidation extends BaseOpValidation
	Public Class LossOpValidation
		Inherits BaseOpValidation

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

		' All tested Loss Ops have backprop at the moment 2019/01/30
		Public Shared ReadOnly NO_BP_YET As ISet(Of String) = New HashSet(Of String)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLoss2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLoss2d(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<String> oneDimensionalOutputFns = Arrays.asList("cosine", "mpwse", "softmaxxent", "softmaxxent_smooth", "mpwse", "sparsesoftmax");
			Dim oneDimensionalOutputFns As IList(Of String) = New List(Of String) From {"cosine", "mpwse", "softmaxxent", "softmaxxent_smooth", "mpwse", "sparsesoftmax"}

			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			Dim totalRun As Integer = 0
			For Each fn As String In New String() { "log_poisson", "log_poisson_full", "absdiff", "cosine", "hinge", "huber", "log", "mse", "sigmoidxent", "sigmoidxent_smooth", "softmaxxent", "softmaxxent_smooth", "mpwse", "sparsesoftmax" }


				For Each weights As String In New String(){"none", "scalar", "perExample", "perOutput"}
					If weights.Equals("perOutput") AndAlso oneDimensionalOutputFns.Contains(fn) Then
						Continue For 'Skip this combination (not possible)
					End If

					For Each reduction As LossReduce In System.Enum.GetValues(GetType(LossReduce))
						If (fn.Equals("softmaxxent") OrElse fn.Equals("softmaxxent_smooth")) AndAlso reduction = LossReduce.NONE Then
							Continue For 'Combination not supported (doesn't make sense)
						End If

						If fn.Equals("sparsesoftmax") AndAlso (Not weights.Equals("none") OrElse reduction <> LossReduce.SUM) Then
							Continue For 'sparse softmax doesn't support weights or reduction confic
						End If

						Dim sd As SameDiff = SameDiff.create()

						Dim nOut As Integer = 4
						Dim minibatch As Integer = 10
						Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
						Dim labels As SDVariable
						If "sparsesoftmax".Equals(fn, StringComparison.OrdinalIgnoreCase) Then
							labels = sd.var("labels", DataType.INT, -1)
						Else
							'ALl other loss functions
							labels = sd.var("labels", DataType.DOUBLE, -1, nOut)
						End If

						Dim w As SDVariable
						Dim wArrBroadcast As INDArray
						Select Case weights
							Case "none"
								w = Nothing
								wArrBroadcast = Nd4j.ones(DataType.DOUBLE, minibatch, nOut)
							Case "scalar"
								w = sd.var("weights", Nd4j.scalar(DataType.DOUBLE, 1.0))
								wArrBroadcast = Nd4j.valueArrayOf(minibatch, nOut, 1.0).castTo(DataType.DOUBLE)
							Case "perExample"
								Dim wpe As INDArray = Nd4j.create(New Double(){0, 0, 1, 1, 2, 2, 3, 3, 4, 4})
								If Not fn.Equals("softmaxxent") AndAlso Not fn.Equals("softmaxxent_smooth") Then
									'Softmaxxent only supports rank 1 not rank 2??
									wpe = wpe.reshape(ChrW(minibatch), 1)
								End If
								w = sd.var("weights", wpe)
								wArrBroadcast = Nd4j.create(DataType.DOUBLE, minibatch, nOut).addiColumnVector(w.Arr)
							Case "perOutput"
								w = sd.var("weights", Nd4j.create(New Double()(){
									New Double() {0, 0, 0, 0},
									New Double() {0, 0, 1, 1},
									New Double() {1, 1, 0, 0},
									New Double() {1, 1, 1, 1},
									New Double() {1, 1, 1, 1},
									New Double() {2, 2, 2, 2},
									New Double() {2, 2, 2, 2},
									New Double() {2, 2, 2, 2},
									New Double() {2, 2, 2, 2},
									New Double() {2, 2, 2, 2}
								}))
								wArrBroadcast = w.Arr
							Case Else
								Throw New Exception()
						End Select
						Dim wArr As INDArray = If(w Is Nothing, Nd4j.scalar(DataType.DOUBLE, 1.0), w.Arr)


						Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
						Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

						Dim expOut As INDArray = Nothing
						Dim loss As SDVariable = Nothing
						Select Case fn
							Case "absdiff"
								expOut = Transforms.abs(predictionsArr.sub(labelsArr))
								loss = sd.loss().absoluteDifference("loss", labels, predictions, w, reduction)
							Case "cosine"
								'Cosine _similarity_: dot(a,b)/(l2Norm(a) * l2Norm(b))
								'Cosine distance = 1 - cosineSimilarity
								'NOTE: both we and TF assume the inputs are normalized
								predictionsArr.diviColumnVector(predictionsArr.norm2(1))
								labelsArr.diviColumnVector(labelsArr.norm2(1))
								expOut = predictionsArr.mul(labelsArr).sum(1).rsub(1.0).reshape(10,1)
								loss = sd.loss().cosineDistance("loss", labels, predictions, w, reduction, 1)
							Case "hinge"
								'0 or 1 labels, but -1 or 1 when calculating loss
								'L = max(0, 1 - prediction * label)
								Nd4j.Executioner.exec(New BernoulliDistribution(labelsArr, 0.5))
								Dim labelMinusOneToOne As INDArray = labelsArr.mul(2).subi(1)
								expOut = Transforms.max(predictionsArr.mul(labelMinusOneToOne).rsubi(1), 0)
								loss = sd.loss().hingeLoss("loss", labels, predictions, w, reduction)
							Case "huber"
								'https://en.wikipedia.org/wiki/Huber_loss
								Dim delta As Double = 1.0
								Dim diff As INDArray = labelsArr.sub(predictionsArr)
								Dim absDiff As INDArray = Transforms.abs(diff)
								Dim lte As INDArray = absDiff.lte(delta).castTo(DataType.DOUBLE)
								Dim gt As INDArray = absDiff.gt(delta).castTo(DataType.DOUBLE)
								expOut = diff.mul(diff).mul(0.5).muli(lte)
								expOut.addi(absDiff.mul(delta).subi(0.5 * delta * delta).mul(gt))
								loss = sd.loss().huberLoss("loss", labels, predictions, w, reduction, delta)
							Case "log"
								Dim eps As Double = 1e-7
								'Loss loss aka binary cross entropy loss
								'Labels are random bernoulli
								Nd4j.Executioner.exec(New BernoulliDistribution(labelsArr, 0.5))
								predictionsArr = Nd4j.rand(predictionsArr.shape()).muli(0.8).addi(0.1)
								Dim logP As INDArray = Transforms.log(predictionsArr.add(eps), True)
								Dim log1p As INDArray = Transforms.log(predictionsArr.rsub(1.0).add(eps), True)
								expOut = labelsArr.mul(logP).addi(labelsArr.rsub(1).mul(log1p)).negi()
								loss = sd.loss().logLoss("loss", labels, predictions, w, reduction, eps)
							Case "log_poisson"
								predictionsArr = Transforms.log(Transforms.abs(predictionsArr))
								labelsArr = Transforms.abs(labelsArr)
								expOut = Transforms.exp(predictionsArr).sub(labelsArr.mul(predictionsArr))
								loss = sd.loss().logPoisson("loss", labels, predictions, w, reduction,False)
							Case "log_poisson_full"
								predictionsArr = Transforms.log(Transforms.abs(predictionsArr))
								labelsArr = Transforms.abs(labelsArr)
								expOut = Transforms.exp(predictionsArr).sub(labelsArr.mul(predictionsArr)).add(labelsArr.mul(Transforms.log(labelsArr))).sub(labelsArr).add(Transforms.log(labelsArr.mul(Math.PI * 2)).mul(0.5))
								loss = sd.loss().logPoisson("loss", labels, predictions, w, reduction,True)
							Case "mse"
								'To match TF, this is actually sum of squares - 1/numExamples (prediction-label)^2
								Dim sqDiff As INDArray = labelsArr.sub(predictionsArr)
								sqDiff.muli(sqDiff)
								expOut = sqDiff
								loss = sd.loss().meanSquaredError("loss", labels, predictions, w, reduction)
							Case "sigmoidxent_smooth", "sigmoidxent" 'Sigmoid xent with label smoothing
								'-1/numExamples * (label * log(p) + (1-label) * log(1-p))
								Nd4j.Executioner.exec(New BernoulliDistribution(labelsArr, 0.5))
								Dim lblSmoothing As Double = If(fn.Equals("sigmoidxent_smooth"), 0.3, 0.0)
								Dim labelArrCopy As INDArray = labelsArr.dup()
								If fn.Equals("sigmoidxent_smooth") Then
									labelArrCopy.muli(1.0 - lblSmoothing).addi(0.5 * lblSmoothing)
								End If

								Dim onePlusExpNegX As INDArray = Transforms.log(Transforms.exp(predictionsArr.neg()).add(1.0))
								expOut = predictionsArr.mul(labelArrCopy.rsub(1.0)).add(onePlusExpNegX)

								loss = sd.loss().sigmoidCrossEntropy("loss", labels, predictions, w, reduction, lblSmoothing)
							Case "softmaxxent", "softmaxxent_smooth"
								'Same as negative log likelihood, but apply softmax on predictions first: For singe example, -sum_outputs label_i * log(p_i)
								'Labels are random one-hot
								'Note that output is shape [minibatch] for NONE reduction, or scalar otherwise
								Dim softmaxPredictions As INDArray = Transforms.softmax(predictionsArr, True)
								labelsArr.assign(0)
								Dim i As Integer = 0
								Do While i < labelsArr.size(0)
									labelsArr.putScalar(i, i Mod labelsArr.size(1), 1.0)
									i += 1
								Loop
								Dim lblSmooth2 As Double = If(fn.Equals("softmaxxent_smooth"), 0.1, 0.0)
								Dim labelsArrCopy As INDArray = labelsArr.dup()
								If fn.Equals("softmaxxent_smooth") Then
									labelsArrCopy.muli(1.0 - lblSmooth2).addi(lblSmooth2 / labelsArrCopy.size(1))
								End If
								Dim logP2 As INDArray = Transforms.log(softmaxPredictions, True)
								expOut = labelsArrCopy.mul(logP2).negi().sum(1)
								loss = sd.loss().softmaxCrossEntropy("loss", labels, predictions, w, reduction, lblSmooth2)
							Case "mpwse"
								expOut = Nd4j.create(labelsArr.size(0), 1)
								Dim n As Double = CDbl(labelsArr.size(1))
								Dim example As Integer = 0
								Do While example < labelsArr.size(0)
									Dim i As Integer = 0
									Do While i < labelsArr.size(1)
										Dim k As Integer = 0
										Do While k < labelsArr.size(1)
											If i <> k Then
												Dim y_i As Double = predictionsArr.getDouble(example, i)
												Dim y_k As Double = predictionsArr.getDouble(example, k)
												Dim q_i As Double = labelsArr.getDouble(example, i)
												Dim q_k As Double = labelsArr.getDouble(example, k)
												Dim add As Double = Math.Pow(((y_i-y_k)-(q_i-q_k)), 2)
												expOut.putScalar(example, expOut.getDouble(example) + add)
											End If
											k += 1
										Loop
										i += 1
									Loop
									example += 1
								Loop

								expOut.muli(1/((n*(n-1)) / 2))

								loss = sd.loss().meanPairwiseSquaredError("loss", labels, predictions,w, reduction)
							Case "sparsesoftmax"
								labelsArr = Nd4j.create(DataType.DOUBLE, minibatch)
								Dim oneHot As INDArray = Nd4j.create(DataType.DOUBLE, minibatch, nOut)
								For i As Integer = 0 To minibatch - 1
									labelsArr.putScalar(i, i Mod nOut)
									oneHot.putScalar(i, i Mod nOut, 1.0)
								Next i

								Dim softmaxPredictions2 As INDArray = Transforms.softmax(predictionsArr, True)
								Dim logP2_2 As INDArray = Transforms.log(softmaxPredictions2, True)
								expOut = oneHot.mul(logP2_2).negi().sum(1)

								loss = sd.loss().sparseSoftmaxCrossEntropy(predictions, labels).sum("loss")

							Case Else
								Throw New Exception()
						End Select

						Select Case weights
							Case "none" 'No changes
							Case "scalar"
								expOut.muli(wArr.getDouble(0))
							Case "perExample"
								expOut.muliColumnVector(wArr)
							Case "perOutput"
								expOut.muli(wArr)
							Case Else
								Throw New Exception()
						End Select

						Dim expOutBefore As INDArray = expOut
						Select Case reduction
							Case LossReduce.SUM
								expOut = expOut.sum().reshape()
							Case LossReduce.MEAN_BY_WEIGHT
								If oneDimensionalOutputFns.Contains(fn) Then
									'1d output, not 2d
									expOut = expOut.sum().divi(wArrBroadcast.getColumn(0).sumNumber().doubleValue())
								Else
									expOut = expOut.sum().divi(wArrBroadcast.sumNumber().doubleValue())
								End If
							Case LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT
								If oneDimensionalOutputFns.Contains(fn) Then
									'1d output, not 2d
									Dim countNonZero As Integer = wArrBroadcast.getColumn(0).neq(0.0).castTo(DataType.DOUBLE).sumNumber().intValue()
									expOut = expOut.sum().divi(countNonZero)
								Else
									Dim countNonZero As Integer = wArrBroadcast.neq(0.0).castTo(DataType.DOUBLE).sumNumber().intValue()
									expOut = expOut.sum().divi(countNonZero)
								End If
						End Select


						Dim msg As String = "test: " & fn & ", reduction=" & reduction & ", weights=" & weights
						log.info("*** Starting test: " & msg)


						sd.associateArrayWithVariable(predictionsArr, predictions)
						sd.associateArrayWithVariable(labelsArr, labels)

						If reduction = LossReduce.NONE Then
							'Sum to make scalar output for gradient check...
							loss = loss.sum()
						End If

						Dim doGradCheck As Boolean = True
						If OpValidationSuite.IGNORE_FAILING AndAlso NO_BP_YET.Contains(fn) Then
							log.warn("--- Skipping gradient check for: {} ---", fn)
							doGradCheck = False
						End If

						Dim tc As TestCase = (New TestCase(sd)).expectedOutput("loss", expOut).gradientCheck(doGradCheck).testFlatBufferSerialization(TestCase.TestSerialization.BOTH)

						If reduction = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT AndAlso Not weights.Equals("none") Then
							tc = tc.gradCheckMask(Collections.singletonMap("weights", w.Arr.neq(0)))
						End If

						If fn.Equals("sparsesoftmax") Then
							tc.gradCheckSkipVariables("labels")
						End If

						Dim [error] As String
						Try
							[error] = OpValidation.validate(tc)
						Catch t As Exception
							log.error("Failed: {}", msg, t)
							[error] = msg & ": " & t.getMessage()
						End Try
						If [error] IsNot Nothing Then
							failed.Add(msg & ": " & [error])
						End If
						totalRun += 1
					Next reduction
				Next weights
			Next fn

			assertEquals(0, failed.Count,failed.Count & " of " & totalRun & " failed: " & failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosineDistance()
		Public Overridable Sub testCosineDistance()
			Dim arr As INDArray = Nd4j.create(New Double()(){
				New Double() {-0.3, -0.2, -0.1},
				New Double() {0, 0.1, 0.2}
			})
			Dim label As INDArray = Nd4j.create(New Double()(){
				New Double() {1.0, 2.0, 3.0},
				New Double() {-1.0, 2.0, 1.0}
			})
			Dim w As INDArray = Nd4j.create(New Double()(){
				New Double() {0},
				New Double() {1}
			})
			Dim [out] As INDArray = Nd4j.scalar(0.0)

			Dim op As CustomOp = DynamicCustomOp.builder("cosine_distance_loss").addInputs(arr, w, label).addOutputs([out]).addIntegerArguments(2, 1).build()
			Nd4j.Executioner.exec(op)

			Dim exp As INDArray = Nd4j.scalar(0.6) 'https://github.com/eclipse/deeplearning4j/issues/6532
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testL2Loss()
		Public Overridable Sub testL2Loss()

			For rank As Integer = 0 To 3
				Dim shape() As Long
				Select Case rank
					Case 0
						shape = New Long(){}
					Case 1
						shape = New Long(){5}
					Case 2
						shape = New Long(){3, 4}
					Case 3
						shape = New Long(){2, 3, 4}
					Case 4
						shape = New Long(){2, 3, 2, 3}
					Case Else
						Throw New Exception()
				End Select
				Dim arr As INDArray = Nd4j.rand(DataType.DOUBLE, shape)

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("v", arr)
				Dim loss As SDVariable = sd.loss().l2Loss("loss", [in])

				Dim exp As INDArray = arr.mul(arr).sum().muli(0.5)

				Dim tc As TestCase = (New TestCase(sd)).expectedOutput("loss", exp).gradientCheck(True).testFlatBufferSerialization(TestCase.TestSerialization.BOTH)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next rank
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonZeroResult(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonZeroResult(ByVal backend As Nd4jBackend)
			Dim predictions As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 5)
			Dim w As INDArray = Nd4j.scalar(1.0)
			Dim label As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 5)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray zero = org.nd4j.linalg.factory.Nd4j.scalar(0.0);
			Dim zero As INDArray = Nd4j.scalar(0.0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray zeroBp = org.nd4j.linalg.factory.Nd4j.zerosLike(predictions);
			Dim zeroBp As INDArray = Nd4j.zerosLike(predictions)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] lossOps = { "absolute_difference_loss", "cosine_distance_loss", "mean_pairwssqerr_loss", "mean_sqerr_loss", "sigm_cross_entropy_loss", "hinge_loss", "huber_loss", "log_loss", "softmax_cross_entropy_loss" };
			Dim lossOps() As String = { "absolute_difference_loss", "cosine_distance_loss", "mean_pairwssqerr_loss", "mean_sqerr_loss", "sigm_cross_entropy_loss", "hinge_loss", "huber_loss", "log_loss", "softmax_cross_entropy_loss" }

			For Each lossOp As String In lossOps
				For Each reductionMode As Integer In New Integer(){1, 2, 3}
					Dim [out] As INDArray = Nd4j.scalar(0.0)
					Dim op As CustomOp = DynamicCustomOp.builder(lossOp).addInputs(predictions, w, label).addOutputs([out]).addIntegerArguments(reductionMode, 0).addFloatingPointArguments(1.0).build()
					Nd4j.Executioner.exec(op)

					assertNotEquals([out], zero,lossOp & " returns zero result. Reduction Mode " & reductionMode)
				Next reductionMode
			Next lossOp

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] lossBPOps = {"absolute_difference_loss", "cosine_distance_loss", "sigm_cross_entropy_loss", "log_loss", "mean_sqerr_loss", "sigm_cross_entropy_loss", "softmax_cross_entropy_loss"};
			Dim lossBPOps() As String = {"absolute_difference_loss", "cosine_distance_loss", "sigm_cross_entropy_loss", "log_loss", "mean_sqerr_loss", "sigm_cross_entropy_loss", "softmax_cross_entropy_loss"}
			For Each lossOp As String In lossBPOps
				For Each reductionMode As Integer In New Integer(){1, 2, 3}
					Dim outBP As INDArray = Nd4j.zerosLike(predictions)
					Dim op As CustomOp = DynamicCustomOp.builder(lossOp & "_grad").addInputs(predictions, w, label).addOutputs(outBP, Nd4j.zerosLike(w), Nd4j.zerosLike(label)).addIntegerArguments(reductionMode, 0).addFloatingPointArguments(1.0).build()
					Nd4j.Executioner.exec(op)

					assertNotEquals(outBP, zeroBp,lossOp & "_grad returns zero result. Reduction Mode " & reductionMode)
				Next reductionMode
			Next lossOp
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void TestStdLossMixedDataType()
		Public Overridable Sub TestStdLossMixedDataType()
			' Default Data Type in this test suite is Double.
			' This test used to throw an Exception that we have mixed data types.

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3,4)
			Dim loss As SDVariable = v.std(True)
		End Sub
	End Class

End Namespace