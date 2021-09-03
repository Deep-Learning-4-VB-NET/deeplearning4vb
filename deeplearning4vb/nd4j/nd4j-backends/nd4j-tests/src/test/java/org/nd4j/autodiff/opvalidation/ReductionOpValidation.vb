Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports OpTestCase = org.nd4j.autodiff.validation.OpTestCase
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ArgAmax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgAmax
Imports ArgAmin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgAmin
Imports SoftmaxCrossEntropyWithLogitsLoss = org.nd4j.linalg.api.ops.impl.loss.SoftmaxCrossEntropyWithLogitsLoss
Imports Moments = org.nd4j.linalg.api.ops.impl.reduce.Moments
Imports NormalizeMoments = org.nd4j.linalg.api.ops.impl.reduce.NormalizeMoments
Imports SufficientStatistics = org.nd4j.linalg.api.ops.impl.reduce.SufficientStatistics
Imports AMean = org.nd4j.linalg.api.ops.impl.reduce.floating.AMean
Imports Entropy = org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy
Imports Mean = org.nd4j.linalg.api.ops.impl.reduce.floating.Mean
Imports Norm1 = org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1
Imports Norm2 = org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2
Imports NormMax = org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax
Imports ShannonEntropy = org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy
Imports SquaredNorm = org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm
Imports ASum = org.nd4j.linalg.api.ops.impl.reduce.same.ASum
Imports CosineDistance = org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance
Imports CosineSimilarity = org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity
Imports Dot = org.nd4j.linalg.api.ops.impl.reduce3.Dot
Imports EuclideanDistance = org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance
Imports HammingDistance = org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance
Imports JaccardDistance = org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports StandardDeviation = org.nd4j.linalg.api.ops.impl.summarystats.StandardDeviation
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @Slf4j @NativeTag public class ReductionOpValidation extends BaseOpValidation
	Public Class ReductionOpValidation
		Inherits BaseOpValidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdev(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdev(ByVal backend As Nd4jBackend)
			Dim errors As IList(Of String) = New List(Of String)()

			For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAllTestMatricesWithShape(3, 4, 12345, DataType.DOUBLE)
				For Each biasCorrected As Boolean In New Boolean(){False, True}
					Dim sd As SameDiff = SameDiff.create()
					Dim var As SDVariable = sd.var("in", p.First)
					Dim stdev As SDVariable = var.std(biasCorrected)

					Dim expOut As INDArray = p.First.std(biasCorrected)

					Dim tc As TestCase = (New TestCase(sd)).testName(p.Second & " - biasCorrected=" & biasCorrected).expected(stdev, expOut).gradientCheck(False)

					Dim err As String = OpValidation.validate(tc)
					If err IsNot Nothing Then
						errors.Add(err)
					End If
				Next biasCorrected
			Next p
			assertEquals(0, errors.Count,errors.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZeroCount(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZeroCount(ByVal backend As Nd4jBackend)
			Dim allFailed As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 20
				Dim sd As SameDiff = SameDiff.create()

				Dim ia As INDArray
				If i = 0 Then
					'Not gradient checkable for 0 and 1 values
					ia = Nd4j.create(New Integer(){2, 2}, New Single(){0, 1, 0, 1}).castTo(DataType.DOUBLE)
				Else
					ia = Nd4j.rand(DataType.DOUBLE,2, 2)
				End If

				Dim input As SDVariable = sd.var("in", DataType.DOUBLE, 2, 2)
				sd.associateArrayWithVariable(ia, input)

				Dim nonZero As SDVariable = sd.math().countNonZero(input)
				Dim zero As SDVariable = sd.math().countZero(input)

				Dim loss As SDVariable = nonZero.add(zero).castTo(DataType.DOUBLE).std(True)

				Dim [error] As String = OpValidation.validate((New TestCase(sd)).expectedOutput(nonZero.name(), Nd4j.scalar(DataType.LONG,If(i = 0, 2.0, 4.0))).expectedOutput(zero.name(), Nd4j.scalar(DataType.LONG,If(i = 0, 2.0, 0.0))).gradientCheck(False))
				If [error] IsNot Nothing Then
					allFailed.Add([error])
				End If
			Next i
			assertEquals(0, allFailed.Count,allFailed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZeroFraction(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZeroFraction(ByVal backend As Nd4jBackend)
			Dim allFailed As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 1
				Dim sd As SameDiff = SameDiff.create()

				Dim ia As INDArray
				If i = 0 Then
					'Not gradient checkable for 0 and 1 values
					ia = Nd4j.create(New Integer(){2, 2}, New Single(){0, 1, 0, 1})
				Else
					ia = Nd4j.rand(DataType.FLOAT, 2, 2)
				End If

				Dim input As SDVariable = sd.var("in", 2, 2)
				sd.associateArrayWithVariable(ia, input)

				Dim zeroFraction As SDVariable = sd.math().zeroFraction(input)

				Dim [error] As String = OpValidation.validate((New TestCase(sd)).expectedOutput(zeroFraction.name(), Nd4j.scalar(If(i = 0, 0.5f, 0.0f))).gradientCheck(i <> 0))
				If [error] IsNot Nothing Then
					allFailed.Add([error])
				End If
			Next i

			assertEquals(0, allFailed.Count,allFailed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionGradientsSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionGradientsSimple(ByVal backend As Nd4jBackend)
			'OpValidationSuite.ignoreFailing();  //TODO TEMPORARY DUE TO CRASHES
			'Test reductions: final and only function
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 20

				Dim sd As SameDiff = SameDiff.create()

				Dim nOut As Integer = 4
				Dim minibatch As Integer = 10
				Dim input As SDVariable = sd.var("in", minibatch, nOut)
				Dim inputArr As INDArray = Nd4j.randn(minibatch, nOut).muli(100)
				Dim length As Long = nOut * minibatch

				Dim loss As SDVariable
				Dim name As String
				Dim tc As New TestCase(sd)
				Dim gradCheck As Boolean = True
				Select Case i
					Case 0
						loss = sd.mean("loss", input)
						name = "mean"
						tc.expectedOutput("loss", inputArr.mean())
					Case 1
						loss = sd.sum("loss", input)
						name = "sum"
						tc.expectedOutput("loss", inputArr.sum())
					Case 2
						loss = sd.standardDeviation("loss", input, True)
						name = "stdev"
						tc.expectedOutput("loss", inputArr.std(True))
					Case 3
						loss = sd.min("loss", input)
						name = "min"
						tc.expectedOutput("loss", inputArr.min())
					Case 4
						loss = sd.max("loss", input)
						name = "max"
						tc.expectedOutput("loss", inputArr.max())
					Case 5
						loss = sd.variance("loss", input, True)
						name = "variance"
						tc.expectedOutput("loss", inputArr.var())
					Case 6
						inputArr = Nd4j.rand(minibatch, nOut).addi(0.5)
						loss = sd.prod("loss", input)
						tc.expectedOutput("loss", inputArr.prod())
						name = "prod"
					Case 7
						loss = sd.norm1("loss", input)
						name = "norm1"
						tc.expectedOutput("loss", inputArr.norm1())
					Case 8
						loss = sd.norm2("loss", input)
						name = "norm2"
						tc.expectedOutput("loss", inputArr.norm2())
					Case 9
						loss = sd.normmax("loss", input)
						name = "normmax"
						tc.expectedOutput("loss", inputArr.normmax())
					Case 10
						loss = sd.math().countNonZero("loss", input, 0,1)
						name = "countNonZero"
						tc.expectedOutput("loss", Nd4j.scalar(inputArr.length()))
						gradCheck = False 'Long out, not floating point
					Case 11
						loss = sd.math().countZero("loss", input, 0,1)
						name = "countZero"
						tc.expectedOutput("loss", Nd4j.scalar(0L))
						gradCheck = False 'Long out, not floating point
					Case 12
						loss = sd.math().reduceAMax("loss", input, 0,1)
						name = "amax"
						tc.expectedOutput("loss", inputArr.amax())
					Case 13
						loss = sd.math().reduceAmin("loss", input, 0,1)
						name = "amin"
						tc.expectedOutput("loss", inputArr.amin())
					Case 14
						loss = sd.math().asum("loss", input, 0,1)
						name = "asum"
						tc.expectedOutput("loss", Nd4j.Executioner.exec(New ASum(inputArr.dup())))
					Case 15
						loss = sd.math().reduceAmean("loss", input, 0,1)
						name = "amean"
						tc.expectedOutput("loss", Nd4j.Executioner.exec(New AMean(inputArr.dup())))
					Case 16
						loss = sd.math().entropy("loss", input, 0,1)
						name = "entropy"
						inputArr = Nd4j.linspace(0.01, 0.99, length, DataType.DOUBLE).reshape("c"c, minibatch, nOut)
						tc.expected("loss", inputArr.mul(Transforms.log(inputArr, True)).sum(Integer.MaxValue).negi())
					Case 17
						inputArr = Nd4j.rand(minibatch, nOut)
						name = "logsumexp"
						loss = sd.math().logSumExp("loss", input)
						Dim expArr As INDArray = Transforms.exp(inputArr)
						Dim sum As Double = expArr.sumNumber().doubleValue()
						tc.expected("loss", Nd4j.scalar(Math.Log(sum)))
					Case 18
						inputArr = Nd4j.rand(minibatch, nOut)
						name = "sqnorm"
						loss = sd.squaredNorm("loss", input)
						Dim norm2 As Double = inputArr.norm2Number().doubleValue()
						tc.expected("loss", Nd4j.scalar(norm2 * norm2))
					Case 19
						inputArr = Nd4j.rand(minibatch, nOut)
						name = "logEntropy"
						loss = sd.math().logEntropy("loss", input, 0,1)
						Dim logEntropy As Double = inputArr.logEntropyNumber().doubleValue()
						tc.expected(loss, Nd4j.scalar(logEntropy))
					Case 20
						inputArr = Nd4j.rand(minibatch, nOut)
						name = "shannonEntropy"
						loss = sd.math().shannonEntropy("loss", input, 0)
						Dim shannonEntropy As Double = inputArr.shannonEntropyNumber().doubleValue()
						tc.expected(loss, Nd4j.scalar(shannonEntropy))
						If OpValidationSuite.IGNORE_FAILING Then
							Continue For
						End If
					Case Else
						Throw New Exception()
				End Select


				Dim msg As String = "test: " & i & " - " & name
				log.info("*** Starting test: " & msg)

				sd.associateArrayWithVariable(inputArr, input)
				If gradCheck Then
					sd.addLossVariable(loss)
				End If

				tc.testName(msg)
				If Not gradCheck Then
					tc.gradientCheck(False)
				End If

				Dim [error] As String = OpValidation.validate(tc, True)
				If [error] IsNot Nothing Then
					failed.Add([error])
				End If
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionGradients1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionGradients1(ByVal backend As Nd4jBackend)
			'Test reductions: final, but *not* the only function
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			For Each [dim] As Integer In New Integer(){0, Integer.MaxValue} 'These two cases are equivalent here

				For i As Integer = 0 To 15

					Dim sd As SameDiff = SameDiff.create()

					Dim nOut As Integer = 4
					Dim minibatch As Integer = 10
					Dim input As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
					Dim label As SDVariable = sd.var("label", DataType.DOUBLE, minibatch, nOut)

					Dim diff As SDVariable = input.sub(label)
					Dim sqDiff As SDVariable = diff.mul(diff)
					Dim msePerEx As SDVariable = sd.mean("msePerEx", sqDiff, 1)

					Dim loss As SDVariable
					Dim name As String
					Dim tc As New TestCase(sd)
					Dim uDistInput As Boolean = False
					Dim gradientCheckable As Boolean = True
					Dim exp As INDArray = Nothing
					Select Case i
						Case 0
							loss = sd.mean("loss", msePerEx, [dim])
							name = "mean"
						Case 1
							loss = sd.sum("loss", msePerEx, [dim])
							name = "sum"
						Case 2
							loss = sd.standardDeviation("loss", msePerEx, True, [dim])
							name = "stdev"
						Case 3
							loss = sd.min("loss", msePerEx, [dim])
							name = "min"
						Case 4
							loss = sd.max("loss", msePerEx, [dim])
							name = "max"
						Case 5
							loss = sd.variance("loss", msePerEx, True, [dim])
							name = "variance"
						Case 6
							loss = sd.prod("loss", msePerEx, [dim])
							name = "prod"
						Case 7
							loss = sd.norm1("loss", msePerEx, [dim])
							name = "norm1"
						Case 8
							loss = sd.norm2("loss", msePerEx, [dim])
							name = "norm2"
						Case 9
							loss = sd.normmax("loss", msePerEx, [dim])
							name = "normmax"
						Case 10
							loss = sd.math().entropy("loss", msePerEx, [dim])
							name = "entropy"
						Case 11
							name = "logEntropy"
							loss = sd.math().logEntropy("loss", msePerEx, [dim])
							uDistInput = True
						Case 12
							loss = sd.math().reduceAMax("loss", msePerEx, [dim])
							name = "amax"
						Case 13
							loss = sd.math().reduceAmin("loss", msePerEx, [dim])
							name = "amin"
						Case 14
							loss = sd.math().asum("loss", msePerEx, [dim])
							name = "asum"
						Case 15
							loss = sd.math().reduceAmean("loss", msePerEx, [dim])
							name = "amean"
						Case Else
							Throw New Exception()
					End Select


					Dim msg As String = "(test " & i & " - " & name & ", dimension=" & [dim] & ")"
					log.info("*** Starting test: " & msg)

					Dim inputArr As INDArray = If(uDistInput, Nd4j.rand(DataType.DOUBLE, minibatch, nOut), Nd4j.randn(DataType.DOUBLE, minibatch, nOut).muli(100))
					Dim labelArr As INDArray = If(uDistInput, Nd4j.rand(DataType.DOUBLE, minibatch, nOut), Nd4j.randn(DataType.DOUBLE, minibatch, nOut).muli(100))

					sd.associateArrayWithVariable(inputArr, input)
					sd.associateArrayWithVariable(labelArr, label)

					tc.gradientCheck(gradientCheckable)
					If exp IsNot Nothing Then
						tc.expectedOutput(loss.name(), exp)
					End If

					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(name)
					End If
				Next i
			Next [dim]

			assertEquals(0, failed.Count,"Failed: " & failed)
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionGradients2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionGradients2(ByVal backend As Nd4jBackend)
			'Test reductions: NON-final function
			Nd4j.Random.setSeed(12345)

			Dim d0 As Integer = 3
			Dim d1 As Integer = 4
			Dim d2 As Integer = 5

			Dim failed As IList(Of String) = New List(Of String)()
			For Each reduceDim As Integer In New Integer(){0, 1, 2}
				For i As Integer = 0 To 17

					Dim outShape() As Long
					Select Case reduceDim
						Case 0
							outShape = New Long(){d1, d2}
						Case 1
							outShape = New Long(){d0, d2}
						Case 2
							outShape = New Long(){d0, d1}
						Case Else
							Throw New Exception()
					End Select

					Dim sd As SameDiff = SameDiff.create()
					sd.setLogExecution(False)

					Dim [in] As SDVariable = sd.var("in", d0, d1, d2)
					Dim label As SDVariable = sd.var("label", outShape)
					Dim second As SDVariable = [in].mul(2)

					Dim maxRelError As Double = 1e-4
					Dim minAbsError As Double = 1e-4
					Dim inputArr As INDArray = Nd4j.randn(DataType.DOUBLE, d0, d1, d2).muli(1000)
					Dim labelArr As INDArray = Nd4j.randn(DataType.DOUBLE, outShape).muli(1000)
					Dim reduced As SDVariable
					Dim name As String
					Dim tc As New TestCase(sd)
					Dim gradCheck As Boolean = True
					Dim exp As INDArray = Nothing
					Select Case i
						Case 0
							reduced = sd.mean("reduced", second, reduceDim)
							name = "mean"
						Case 1
							inputArr.divi(100)
							labelArr.divi(100)
							reduced = sd.sum("reduced", second, reduceDim)
							name = "sum"
						Case 2
							reduced = sd.standardDeviation("reduced", second, True, reduceDim)
							inputArr.divi(1000)
							labelArr.divi(1000)
							name = "stdev"
						Case 3
							reduced = sd.min("reduced", second, reduceDim)
							name = "min"
						Case 4
							reduced = sd.max("reduced", second, reduceDim)
							name = "max"
						Case 5
							'Variance is a bit finniky for gradient checks, due to huge score/output...
							maxRelError = 1e-3
							minAbsError = 1 'Most gradients are in the range 1k to >100k
							inputArr.divi(10)
							labelArr.divi(100)
							BooleanIndexing.replaceWhere(inputArr, Nd4j.rand(inputArr.shape()).muli(100).addi(100), Conditions.absLessThan(1.0))
							reduced = sd.variance("reduced", second, True, reduceDim)
							name = "variance"
						Case 6
							inputArr.assign(Nd4j.rand(DataType.DOUBLE, New Integer(){d0, d1, d2}).addi(0.5))
							labelArr.assign(Nd4j.rand(DataType.DOUBLE, outShape).addi(0.5))
							reduced = sd.prod("reduced", second, reduceDim)
							name = "prod"
						Case 7
							maxRelError = 1e-4
							inputArr.assign(Nd4j.rand(DataType.DOUBLE, New Integer(){d0, d1, d2}).muli(10))
							labelArr.assign(Nd4j.rand(DataType.DOUBLE, outShape).muli(10))
							reduced = sd.norm1("reduced", second, reduceDim)
							name = "norm1"
						Case 8
							maxRelError = 1e-3 'Norm2 can also run into numerical precision issues
							reduced = sd.norm2("reduced", second, reduceDim)
							name = "norm2"
						Case 9
							inputArr = Nd4j.rand(DataType.DOUBLE, New Integer(){d0, d1, d2})
							labelArr = Nd4j.rand(DataType.DOUBLE, outShape)
							reduced = sd.normmax("reduced", second, reduceDim)
							name = "normmax"
						Case 10
							reduced = sd.argmax("reduced", second, reduceDim)
							gradCheck = False
							exp = inputArr.mul(2).argMax(reduceDim)
							name = "argmax"
						Case 11
							reduced = sd.argmin("reduced", second, reduceDim)
							gradCheck = False
							exp = Nd4j.argMin(inputArr.mul(2), reduceDim)
							name = "argmin"
						Case 12
							reduced = sd.math().countNonZero("reduced", second, reduceDim)
							gradCheck = False
							exp = inputArr.mul(2).neq(0).castTo(DataType.LONG).sum(reduceDim)
							name = "countNonZero"
						Case 13
							reduced = sd.math().countZero("reduced", second, reduceDim)
							gradCheck = False
							exp = inputArr.mul(2).eq(0).castTo(DataType.LONG).sum(reduceDim)
							name = "countZero"
						Case 14
							reduced = sd.math().reduceAMax("reduced", second, reduceDim)
							name = "amax"
						Case 15
							reduced = sd.math().reduceAmin("reduced", second, reduceDim)
							name = "amin"
						Case 16
							reduced = sd.math().asum("reduced", second, reduceDim)
							name = "asum"
						Case 17
							reduced = sd.math().reduceAmean("reduced", second, reduceDim)
							name = "amean"
						Case Else
							Throw New Exception()
					End Select

					Dim add As SDVariable = reduced.castTo(DataType.DOUBLE).add(1.0)

					Dim diff As SDVariable = label.sub(add)
					Dim sqDiff As SDVariable = diff.mul(diff)
					Dim mseLoss As SDVariable = sd.mean("loss", sqDiff)


					Dim msg As String = "(test " & i & " - " & name & ", dimension=" & reduceDim & ")"
					log.info("*** Starting test: " & msg)

					sd.associateArrayWithVariable(inputArr, [in])
					sd.associateArrayWithVariable(labelArr, label)

					tc.gradCheckMaxRelativeError(maxRelError)
					tc.gradCheckMinAbsError(minAbsError)
					tc.gradientCheck(gradCheck)
					If exp IsNot Nothing Then
						tc.expected(reduced, exp)
					End If

					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(name & " - " & [error])
					End If
				Next i
			Next reduceDim

			assertEquals(0, failed.Count,"Failed: " & failed)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim d0 As Integer = 3
			Dim d1 As Integer = 4
			Dim d2 As Integer = 5

			Dim failed As IList(Of String) = New List(Of String)()
			For Each reduceDims As Integer() In New Integer()(){
				New Integer() {Integer.MaxValue},
				New Integer() {0, 1, 2},
				New Integer() {0},
				New Integer() {1},
				New Integer() {2},
				New Integer() {0, 1},
				New Integer() {0, 2},
				New Integer() {1, 2}
			}
				For i As Integer = 0 To 6

					Dim sd As SameDiff = SameDiff.create()
					sd.setLogExecution(False)


					Dim [in] As SDVariable = sd.var("in", d1, d1, d2)
					Dim in2 As SDVariable = sd.var("in2", d0, d1, d2)

					Dim inArr As INDArray = Nd4j.randn(New Integer(){d0, d1, d2}).muli(100)
					Dim in2Arr As INDArray = Nd4j.randn(inArr.shape()).muli(100)

					Dim exp As INDArray
					Dim reduced As SDVariable
					Dim name As String
					Dim tc As New TestCase(sd)
					Dim maxRelError As Double? = Nothing
					Select Case i
						Case 0
							reduced = sd.math().manhattanDistance([in], in2, reduceDims)
							name = "manhattan"
							exp = Nd4j.Executioner.exec(New ManhattanDistance(inArr, in2Arr, Nothing, False, False, reduceDims))
						Case 1
							reduced = sd.math().euclideanDistance([in], in2, reduceDims)
							name = "euclidean"
							exp = Nd4j.Executioner.exec(New EuclideanDistance(inArr, in2Arr, Nothing, False, False, reduceDims))
						Case 2
							inArr.muli(1e-4)
							in2Arr.muli(1e-4)
							reduced = sd.math().cosineSimilarity([in], in2, reduceDims)
							name = "cosine"
							exp = Nd4j.Executioner.exec(New CosineSimilarity(inArr, in2Arr, Nothing, False, False, reduceDims))
							maxRelError = 1e-4
						Case 3
							reduced = sd.math().cosineDistance([in], in2, reduceDims)
							name = "cosinedistance"
							exp = Nd4j.Executioner.exec(New CosineDistance(inArr, in2Arr, Nothing, False, False, reduceDims))
							maxRelError = 1e-4
						Case 4
							reduced = sd.math().hammingDistance([in], in2, reduceDims)
							name = "hamming"
							exp = Nd4j.Executioner.exec(New HammingDistance(inArr, in2Arr, Nothing, False, False, reduceDims))
						Case 5
							name = "jaccard"
							reduced = sd.math().jaccardDistance(name, [in], in2, reduceDims)
							inArr.divi(100).addi(0.1)
							in2Arr.divi(100).addi(0.1)
							exp = Nd4j.Executioner.exec(New JaccardDistance(inArr, in2Arr, Nothing, False, False, reduceDims))

							If OpValidationSuite.IGNORE_FAILING AndAlso reduceDims.Length = 2 Then
								Continue For
							End If
						Case 6
							If OpValidationSuite.IGNORE_FAILING Then
								'https://github.com/eclipse/deeplearning4j/issues/6069
								Continue For
							End If
							name = "dot"
							reduced = sd.dot(name, [in], in2, reduceDims)
							exp = Nd4j.Executioner.exec(New Dot(inArr, in2Arr, Nothing, True, False, reduceDims))
						Case Else
							Throw New Exception()
					End Select

					'Sum: note that this should be a no-op for the full array cases
					Dim sum As SDVariable = sd.sum(reduced, Integer.MaxValue)


					Dim msg As String = "(test " & i & " - " & name & ", dimensions=" & Arrays.toString(reduceDims) & ")"
					log.info("*** Starting test: " & msg)

					sd.associateArrayWithVariable(inArr, [in])
					sd.associateArrayWithVariable(in2Arr, in2)

					tc.expected(reduced, exp)

					If maxRelError IsNot Nothing Then
						tc.gradCheckMaxRelativeError(maxRelError)
					End If

					Dim [error] As String = OpValidation.validate(tc, True)
					If [error] IsNot Nothing Then
						failed.Add(msg & " - " & [error])
					End If
				Next i
			Next reduceDims

			assertEquals(0, failed.Count,"Failed: " & failed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMoments(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMoments(ByVal backend As Nd4jBackend)
			For Each axes As Integer() In New Integer()(){
				New Integer() {0},
				New Integer() {1},
				New Integer() {0, 1}
			}
				Dim input As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", input)

				Dim moments() As SDVariable = sd.math().moments([in], axes)
				Dim expMean As INDArray = input.mean(axes)
				Dim expVar As INDArray = input.var(False, axes)

				Dim loss As SDVariable
				If axes.Length < 2 Then
					loss = moments(0).add(moments(1)).std(True)
				Else
					loss = moments(0).add(moments(1)).mean()
				End If


				Dim msg As String = Arrays.toString(axes)

				Dim tc As TestCase = (New TestCase(sd)).testName(msg).expected(moments(0), expMean).expected(moments(1), expVar)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next axes
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMomentsOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMomentsOp(ByVal backend As Nd4jBackend)
			Dim axes() As Integer = {0}
			Dim input As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			Dim outMean As INDArray = Nd4j.createUninitialized(New Long(){4})
			Dim outVar As INDArray = Nd4j.createUninitialized(New Long(){4})

			Dim tc As New OpTestCase(New Moments(input, outMean, outVar, axes))

			tc.expectedOutput(0, input.mean(axes).reshape(ChrW(4)))
			tc.expectedOutput(1, input.var(False, axes).reshape(ChrW(4)))

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizeMomentsOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormalizeMomentsOp(ByVal backend As Nd4jBackend)
			Dim data As INDArray = Nd4j.linspace(1, 100, 100, DataType.DOUBLE).reshape(ChrW(10), 10)
			Dim ssSum As INDArray = data.sum(0)
			Dim ssSqSum As INDArray = data.mul(data).sum(0)

			Dim meanExp As INDArray = data.mean(0)
			Dim varExp As INDArray = data.var(False, 0)

			Dim mean As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, meanExp.shape())
			Dim var As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, varExp.shape())

			Dim op As New OpTestCase(New NormalizeMoments(Nd4j.scalar(DataType.DOUBLE, 10), ssSum, ssSqSum, mean, var))
			op.expectedOutput(0, meanExp)
			op.expectedOutput(1, varExp)

			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllAny(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllAny(ByVal backend As Nd4jBackend)

			Dim allZeros As INDArray = Nd4j.zeros(DataType.FLOAT, 3, 4)
			Dim allOnes As INDArray = Nd4j.ones(DataType.FLOAT, 3, 4)
			Dim mixed As INDArray = Nd4j.zeros(DataType.FLOAT, 3, 4)
			mixed.getRow(1).assign(1.0)

			Dim [in]() As INDArray = {allZeros, allOnes, mixed}
			Dim expAll() As Boolean = {False, True, False}
			Dim expAny() As Boolean = {False, True, True}

			For i As Integer = 0 To 2
				Dim sd As SameDiff = SameDiff.create()

				Dim s As SDVariable = sd.var("in", [in](i))
				Dim all As SDVariable = sd.all(s)
				Dim any As SDVariable = sd.any(s)

				Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(False).expected(all, Nd4j.scalar(expAll(i))).expected(any, Nd4j.scalar(expAny(i))))

				assertNull(err)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexAccum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIndexAccum(ByVal backend As Nd4jBackend)
			Dim failed As IList(Of String) = New List(Of String)()
			Dim dims As IList(Of Integer()) = New List(Of Integer()) From {
				New Integer(){0},
				New Integer(){1},
				New Integer(){0, 1}
			}

			Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE,3, 4)

			For t As Integer = 0 To 2
				Dim d() As Integer = dims(t)
				For i As Integer = 0 To 6

					Dim [dim]() As Integer = If(d.Length = 0, New Integer(){}, d)

					Dim sd As SameDiff = SameDiff.create()
					Dim s As SDVariable = sd.var("in", [in])
					Dim reduce As SDVariable

					Dim name As String
					Dim exp As INDArray
					Select Case i
						Case 0
							reduce = s.argmax([dim])
							exp = Nd4j.argMax([in], [dim])
							name = "argmax"
						Case 1
							reduce = s.argmin([dim])
							exp = Nd4j.argMin([in], [dim])
							name = "argmin"
						Case 2
							reduce = sd.math().iamax(s, [dim])
							exp = Nd4j.Executioner.exec(New ArgAmax(New INDArray(){[in].dup()},[dim]))(0)
							name = "iamax"
						Case 3
							reduce = sd.math().iamin(s, [dim])
							exp = Nd4j.Executioner.exec(New ArgAmin(New INDArray(){[in].dup()}, [dim]))(0)
							name = "iamin"
						Case 4
							reduce = sd.math().firstIndex(s, Conditions.greaterThan(0), [dim])
							exp = [in].sum([dim]).assign(0).castTo(DataType.INT64)
							name = "firstindex"
						Case 5
							reduce = sd.math().lastIndex(s, Conditions.greaterThan(0), [dim])
							If t = 0 Then
								exp = Nd4j.createFromArray(2L, 2, 2, 2)
							ElseIf t = 1 Then
								exp = Nd4j.createFromArray(3L, 3, 3)
							Else
								exp = Nd4j.scalar(11L)
							End If
							name = "lastindex"
						Case 6
							reduce = sd.matchConditionCount("count", s, Conditions.greaterThan(0), False, [dim])
							If t = 0 Then
								exp = Nd4j.createFromArray(3L, 3, 3, 3)
							ElseIf t = 1 Then
								exp = Nd4j.createFromArray(4L, 4, 4)
							Else
								exp = Nd4j.scalar(12L)
							End If
							name = "matchConditionCount"
						Case Else
							Throw New Exception()
					End Select
					Dim preCast As SDVariable = reduce
					reduce = reduce.castTo(DataType.DOUBLE)

					Dim loss As SDVariable
					If [dim] Is Nothing OrElse [dim].Length = 2 Then
						loss = reduce.mean()
					Else
						loss = reduce.std(True)
					End If

					Dim tc As TestCase = (New TestCase(sd)).expected(preCast, exp).gradientCheck(False).testName(name & " - " & (If([dim] Is Nothing, Nothing, Arrays.toString([dim]))))

					log.info("Starting: {}", tc.testName())
					Dim err As String = OpValidation.validate(tc, True)
					If err IsNot Nothing Then
						failed.Add(err)
					End If
				Next i
			Next t

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3_2(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim d0 As Integer = 3
			Dim d1 As Integer = 4
			Dim d2 As Integer = 5

			For Each reduceDims As Integer() In New Integer()(){
				New Integer() {Integer.MaxValue},
				New Integer() {0, 1, 2},
				New Integer() {0},
				New Integer() {1},
				New Integer() {2},
				New Integer() {0, 1},
				New Integer() {0, 2},
				New Integer() {1, 2}
			}
				For i As Integer = 0 To 5

					Dim sd As SameDiff = SameDiff.create()
					sd.setLogExecution(False)

					Dim a As INDArray = Nd4j.rand(DataType.DOUBLE, d0, d1, d2)
					Dim b As INDArray = Nd4j.rand(DataType.DOUBLE, d0, d1, d2)


					Dim [in] As SDVariable = sd.var("in", a)
					Dim in2 As SDVariable = sd.var("in2", b)

					Dim expOut As INDArray
					Dim reduced As SDVariable
					Dim name As String
	'                System.out.println(i);
					Select Case i
						Case 0
							reduced = sd.math().manhattanDistance([in], in2, reduceDims)
							name = "manhattan"
							expOut = Nd4j.Executioner.exec(New ManhattanDistance(a, b, Nothing, False, reduceDims))
						Case 1
							reduced = sd.math().euclideanDistance([in], in2, reduceDims)
							name = "euclidean"
							expOut = Nd4j.Executioner.exec(New EuclideanDistance(a, b, Nothing, False, reduceDims))
						Case 2
							reduced = sd.math().cosineSimilarity([in], in2, reduceDims)
							name = "cosine"
							expOut = Nd4j.Executioner.exec(New CosineSimilarity(a, b, Nothing, False, reduceDims))
						Case 3
							reduced = sd.math().jaccardDistance([in], in2, reduceDims)
							name = "jaccard"
							expOut = Nd4j.Executioner.exec(New JaccardDistance(a, b, Nothing, False, reduceDims))
						Case 4
							reduced = sd.math().hammingDistance([in], in2, reduceDims)
							name = "hamming"
							expOut = Nd4j.Executioner.exec(New HammingDistance(a, b, Nothing, False, reduceDims))
						Case 5
							reduced = sd.math().cosineDistance([in], in2, reduceDims)
							name = "reduced"
							expOut = Nd4j.Executioner.exec(New CosineDistance(a, b, Nothing, False, reduceDims))
						Case Else
							Throw New Exception()
					End Select
	'                System.out.println(i + " - end");


					Dim expShape() As Long
					If New Integer(){0}.SequenceEqual(reduceDims) Then
						expShape = New Long(){4, 5}
					ElseIf New Integer(){1}.SequenceEqual(reduceDims) Then
						expShape = New Long(){3, 5}
					ElseIf New Integer(){2}.SequenceEqual(reduceDims) Then
						expShape = New Long(){3, 4}
					ElseIf New Integer(){Integer.MaxValue}.SequenceEqual(reduceDims) Then
						expShape = New Long(){}
					ElseIf New Integer(){0, 1}.SequenceEqual(reduceDims) Then
						expShape = New Long(){5}
					ElseIf New Integer(){0, 2}.SequenceEqual(reduceDims) Then
						expShape = New Long(){4}
					ElseIf New Integer(){1, 2}.SequenceEqual(reduceDims) Then
						expShape = New Long(){3}
					ElseIf New Integer(){0, 1, 2}.SequenceEqual(reduceDims) Then
						expShape = New Long(){}
					Else
						Throw New Exception()
					End If

					Dim msg As String = name & " - dims=" & Arrays.toString(reduceDims)

					Dim [out] As INDArray = reduced.eval()

					log.info(msg & " - expected shape: " & Arrays.toString(expShape) & ", out=" & Arrays.toString([out].shape()) & ", outExp=" & Arrays.toString(expOut.shape()))

					assertArrayEquals(expShape, [out].shape(),msg)
					assertArrayEquals(expShape, expOut.shape(),msg)

					assertEquals(expOut, [out],msg)
				Next i
			Next reduceDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionsBackwards(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionsBackwards(ByVal backend As Nd4jBackend)
	'        for (int i = 0; i < 7; i++) {
			Dim i As Integer=5
			If True Then

				Dim sd As SameDiff = SameDiff.create()

				Dim nOut As Integer = 4
				Dim minibatch As Integer = 3
				Dim input As SDVariable = sd.var("in", DataType.DOUBLE, New Long(){minibatch, nOut})
				Dim label As SDVariable = sd.var("label", DataType.DOUBLE, New Long(){minibatch, nOut})

				Dim diff As SDVariable = input.sub(label)
				Dim sqDiff As SDVariable = diff.mul(diff)
				Dim msePerEx As SDVariable = sd.mean("msePerEx", sqDiff, 1)

				Dim loss As SDVariable 'Scalar value
				Dim name As String
				Select Case i
					Case 0
						loss = sd.mean("loss", msePerEx, 0)
						name = "mean"
					Case 1
						loss = sd.sum("loss", msePerEx, 0)
						name = "sum"
					Case 2
						loss = sd.standardDeviation("loss", msePerEx, True, 0)
						name = "stdev"
					Case 3
						loss = sd.min("loss", msePerEx, 0)
						name = "min"
					Case 4
						loss = sd.max("loss", msePerEx, 0)
						name = "max"
					Case 5
						loss = sd.variance("loss", msePerEx, True, 0)
						name = "variance"
					Case 6
						loss = sd.prod("loss", msePerEx, 0)
						name = "prod"
					Case Else
						Throw New Exception()
				End Select


				Dim msg As String = "test: " & i & " - " & name
				log.info("*** Starting test: " & msg)

				Dim inputArr As INDArray = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
				Dim labelArr As INDArray = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)

				sd.associateArrayWithVariable(inputArr, input)
				sd.associateArrayWithVariable(labelArr, label)

				Dim result As INDArray = loss.eval()
				assertEquals(1, result.length())

				sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDotProductAttention()
		Public Overridable Sub testDotProductAttention()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray keys = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 3});
			Dim keys As INDArray = Nd4j.rand(New Integer(){10, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray values = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 3});
			Dim values As INDArray = Nd4j.rand(New Integer(){10, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray query = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 1});
			Dim query As INDArray = Nd4j.rand(New Integer(){10, 4, 1})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray exec = org.nd4j.linalg.factory.Nd4j.matmul(keys, query, true, false, false).divi(Math.sqrt(keys.size(1)));
			Dim exec As INDArray = Nd4j.matmul(keys, query, True, False, False).divi(Math.Sqrt(keys.size(1)))
			Nd4j.exec(DirectCast(New SoftMax(exec, exec, 1), CustomOp))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = org.nd4j.linalg.factory.Nd4j.matmul(values, exec).norm1();
			Dim finalOut As INDArray = Nd4j.matmul(values, exec).norm1()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdQ As SDVariable = sd.var("q", query)
			Dim sdK As SDVariable = sd.var("k", keys)
			Dim sdV As SDVariable = sd.var("v", values)

			Dim t As SDVariable = sd.nn_Conflict.dotProductAttention(sdQ, sdK, sdV, Nothing, True)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDotProductAttentionWithMask()
		Public Overridable Sub testDotProductAttentionWithMask()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray keys = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 3});
			Dim keys As INDArray = Nd4j.rand(New Integer(){10, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray values = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 3});
			Dim values As INDArray = Nd4j.rand(New Integer(){10, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray query = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 1});
			Dim query As INDArray = Nd4j.rand(New Integer(){10, 4, 1})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray mask = org.nd4j.linalg.factory.Nd4j.rand(10, 3).gte(0.2).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim mask As INDArray = Nd4j.rand(10, 3).gte(0.2).castTo(DataType.DOUBLE)


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray exec = org.nd4j.linalg.factory.Nd4j.matmul(keys, query, true, false, false).divi(Math.sqrt(keys.size(1)));
			Dim exec As INDArray = Nd4j.matmul(keys, query, True, False, False).divi(Math.Sqrt(keys.size(1)))
			exec.addi(mask.reshape(ChrW(10), 3, 1).sub(1).muli(1e9))
			Nd4j.exec(DirectCast(New SoftMax(exec, exec, 1), CustomOp))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = org.nd4j.linalg.factory.Nd4j.matmul(values, exec).norm1();
			Dim finalOut As INDArray = Nd4j.matmul(values, exec).norm1()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdQ As SDVariable = sd.var("q", query)
			Dim sdK As SDVariable = sd.var("k", keys)
			Dim sdV As SDVariable = sd.var("v", values)
			Dim sdMask As SDVariable = sd.constant("mask", mask)

			Dim t As SDVariable = sd.nn_Conflict.dotProductAttention(sdQ, sdK, sdV, sdMask, True)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradCheckSkipVariables("mask").gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDotProductAttentionMultiHeadInputWithMask()
		Public Overridable Sub testDotProductAttentionMultiHeadInputWithMask()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray keys = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 5, 4, 3});
			Dim keys As INDArray = Nd4j.rand(New Integer(){2, 5, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray values = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 5, 4, 3});
			Dim values As INDArray = Nd4j.rand(New Integer(){2, 5, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray query = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 5, 4, 2});
			Dim query As INDArray = Nd4j.rand(New Integer(){2, 5, 4, 2})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray mask = org.nd4j.linalg.factory.Nd4j.rand(2, 3).gte(0.2).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim mask As INDArray = Nd4j.rand(2, 3).gte(0.2).castTo(DataType.DOUBLE)


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray exec = org.nd4j.linalg.factory.Nd4j.matmul(keys, query, true, false, false).divi(Math.sqrt(keys.size(-2)));
			Dim exec As INDArray = Nd4j.matmul(keys, query, True, False, False).divi(Math.Sqrt(keys.size(-2)))
			exec.addi(Nd4j.tile(mask.reshape(ChrW(2), 1, 3, 1), 1, 5, 1, 2).sub(1).muli(1e9))
			Nd4j.exec(DirectCast(New SoftMax(exec, exec, -2), CustomOp))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = org.nd4j.linalg.factory.Nd4j.matmul(values, exec).norm1();
			Dim finalOut As INDArray = Nd4j.matmul(values, exec).norm1()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdQ As SDVariable = sd.var("q", query)
			Dim sdK As SDVariable = sd.var("k", keys)
			Dim sdV As SDVariable = sd.var("v", values)
			Dim sdMask As SDVariable = sd.constant("mask", mask)


			Dim t As SDVariable = sd.nn_Conflict.dotProductAttention(sdQ, sdK, sdV, sdMask, True)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradCheckSkipVariables("mask").gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDotProductAttentionMultiHeadInput()
		Public Overridable Sub testDotProductAttentionMultiHeadInput()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray keys = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 5, 4, 3});
			Dim keys As INDArray = Nd4j.rand(New Integer(){2, 5, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray values = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 5, 4, 3});
			Dim values As INDArray = Nd4j.rand(New Integer(){2, 5, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray query = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 5, 4, 1});
			Dim query As INDArray = Nd4j.rand(New Integer(){2, 5, 4, 1})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray exec = org.nd4j.linalg.factory.Nd4j.matmul(keys, query, true, false, false).divi(Math.sqrt(keys.size(-2)));
			Dim exec As INDArray = Nd4j.matmul(keys, query, True, False, False).divi(Math.Sqrt(keys.size(-2)))
			Nd4j.exec(DirectCast(New SoftMax(exec, exec, -2), CustomOp))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = org.nd4j.linalg.factory.Nd4j.matmul(values, exec).norm1();
			Dim finalOut As INDArray = Nd4j.matmul(values, exec).norm1()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdQ As SDVariable = sd.var("q", query)
			Dim sdK As SDVariable = sd.var("k", keys)
			Dim sdV As SDVariable = sd.var("v", values)

			Dim t As SDVariable = sd.nn_Conflict.dotProductAttention(sdQ, sdK, sdV, Nothing, True)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradientCheck(True))
			assertNull(err)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiHeadedDotProductAttention()
		Public Overridable Sub testMultiHeadedDotProductAttention()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray k = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 5});
			Dim k As INDArray = Nd4j.rand(New Integer(){10, 4, 5})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray v = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 5});
			Dim v As INDArray = Nd4j.rand(New Integer(){10, 4, 5})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray q = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 2});
			Dim q As INDArray = Nd4j.rand(New Integer(){10, 4, 2})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wk = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 3, 4});
			Dim Wk As INDArray = Nd4j.rand(New Integer(){2, 3, 4})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wv = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 3, 4});
			Dim Wv As INDArray = Nd4j.rand(New Integer(){2, 3, 4})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wq = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 3, 4});
			Dim Wq As INDArray = Nd4j.rand(New Integer(){2, 3, 4})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wo = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2* 3, 8});
			Dim Wo As INDArray = Nd4j.rand(New Integer(){2* 3, 8})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray kP = org.nd4j.linalg.factory.Nd4j.tensorMmul(k, Wk, new int[][]{{1}, {2}}).permutei(0, 2, 3, 1);
			Dim kP As INDArray = Nd4j.tensorMmul(k, Wk, New Integer()(){
				New Integer() {1},
				New Integer() {2}
			}).permutei(0, 2, 3, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray vP = org.nd4j.linalg.factory.Nd4j.tensorMmul(v, Wv, new int[][]{{1}, {2}}).permutei(0, 2, 3, 1);
			Dim vP As INDArray = Nd4j.tensorMmul(v, Wv, New Integer()(){
				New Integer() {1},
				New Integer() {2}
			}).permutei(0, 2, 3, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray qP = org.nd4j.linalg.factory.Nd4j.tensorMmul(q, Wq, new int[][]{{1}, {2}}).permutei(0, 2, 3, 1);
			Dim qP As INDArray = Nd4j.tensorMmul(q, Wq, New Integer()(){
				New Integer() {1},
				New Integer() {2}
			}).permutei(0, 2, 3, 1)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray mask = org.nd4j.linalg.factory.Nd4j.rand(10, 5).gte(0.2).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim mask As INDArray = Nd4j.rand(10, 5).gte(0.2).castTo(DataType.DOUBLE)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ops.DynamicCustomOp dot_product_attention = org.nd4j.linalg.api.ops.DynamicCustomOp.builder("dot_product_attention").addInputs(qP, kP, vP, mask).addIntegerArguments(1, 0).build();
			Dim dot_product_attention As DynamicCustomOp = DynamicCustomOp.builder("dot_product_attention").addInputs(qP, kP, vP, mask).addIntegerArguments(1, 0).build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] outputs = org.nd4j.linalg.factory.Nd4j.exec(dot_product_attention);
			Dim outputs() As INDArray = Nd4j.exec(dot_product_attention)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray attOut = outputs[0].permutei(0, 3, 1, 2).reshape(k.size(0), q.size(2), Wv.size(0) * Wv.size(1));
			Dim attOut As INDArray = outputs(0).permutei(0, 3, 1, 2).reshape(k.size(0), q.size(2), Wv.size(0) * Wv.size(1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray out = org.nd4j.linalg.factory.Nd4j.tensorMmul(attOut, Wo, new int[][]{{2}, {0}}).permutei(0, 2, 1);
			Dim [out] As INDArray = Nd4j.tensorMmul(attOut, Wo, New Integer()(){
				New Integer() {2},
				New Integer() {0}
			}).permutei(0, 2, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = out.norm2();
			Dim finalOut As INDArray = [out].norm2()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdQ As SDVariable = sd.var("q", q)
			Dim sdK As SDVariable = sd.var("k", k)
			Dim sdV As SDVariable = sd.var("v", v)
			Dim sdWq As SDVariable = sd.var("Wq", Wq)
			Dim sdWk As SDVariable = sd.var("Wk", Wk)
			Dim sdWv As SDVariable = sd.var("Wv", Wv)
			Dim sdWo As SDVariable = sd.var("Wo", Wo)
			Dim sdMask As SDVariable = sd.constant("mask", mask)


			Dim t As SDVariable = sd.nn_Conflict.multiHeadDotProductAttention(sdQ, sdK, sdV, sdWq, sdWk, sdWv, sdWo, sdMask, True)
			t.norm2("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradientCheck(True).gradCheckSkipVariables("mask"))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDotProductAttentionWeirdInputs()
		Public Overridable Sub testDotProductAttentionWeirdInputs()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray keys = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 3});
			Dim keys As INDArray = Nd4j.rand(New Integer(){10, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray values = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 3});
			Dim values As INDArray = Nd4j.rand(New Integer(){10, 4, 3})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray query = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 1});
			Dim query As INDArray = Nd4j.rand(New Integer(){10, 4, 1})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray mask = org.nd4j.linalg.factory.Nd4j.rand(10, 3).gte(0.2).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim mask As INDArray = Nd4j.rand(10, 3).gte(0.2).castTo(DataType.DOUBLE)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray exec = org.nd4j.linalg.factory.Nd4j.matmul(keys, query, true, false, false).divi(Math.sqrt(keys.size(1)));
			Dim exec As INDArray = Nd4j.matmul(keys, query, True, False, False).divi(Math.Sqrt(keys.size(1)))
			exec.addi(mask.reshape(ChrW(10), 3, 1).sub(1).muli(1e9))
			Nd4j.exec(DirectCast(New SoftMax(exec, exec, 1), CustomOp))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = org.nd4j.linalg.factory.Nd4j.matmul(values, exec).norm1();
			Dim finalOut As INDArray = Nd4j.matmul(values, exec).norm1()

			For Each queryOrder As Char In New Char(){"f"c, "c"c}
				For Each keyOrder As Char In New Char(){"f"c, "c"c}
					For Each valueOrder As Char In New Char(){"f"c, "c"c}
						log.info("-*- Starting Test: query order = {}, key order = {}, value order = {}-*-", queryOrder, keyOrder, valueOrder)
						Dim sd As SameDiff = SameDiff.create()
						Dim sdQ As SDVariable = sd.var("q", query.dup(queryOrder))
						Dim sdK As SDVariable = sd.var("k", keys.dup(keyOrder))
						Dim sdV As SDVariable = sd.var("v", values.dup(valueOrder))
						Dim sdMask As SDVariable = sd.constant("mask", mask)

						Dim t As SDVariable = sd.nn_Conflict.dotProductAttention(sdQ, sdK, sdV, sdMask, True)
						t.norm1("out").markAsLoss()

						Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradientCheck(True).gradCheckPrint(False).gradCheckSkipVariables("mask"))
						assertNull(err)
					Next valueOrder
				Next keyOrder
			Next queryOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiHeadedDotProductAttentionWeirdInputs()
		Public Overridable Sub testMultiHeadedDotProductAttentionWeirdInputs()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray k = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 5});
			Dim k As INDArray = Nd4j.rand(New Integer(){10, 4, 5})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray v = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 5});
			Dim v As INDArray = Nd4j.rand(New Integer(){10, 4, 5})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray q = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4, 2});
			Dim q As INDArray = Nd4j.rand(New Integer(){10, 4, 2})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wk = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 3, 4});
			Dim Wk As INDArray = Nd4j.rand(New Integer(){2, 3, 4})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wv = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 3, 4});
			Dim Wv As INDArray = Nd4j.rand(New Integer(){2, 3, 4})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wq = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2, 3, 4});
			Dim Wq As INDArray = Nd4j.rand(New Integer(){2, 3, 4})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray Wo = org.nd4j.linalg.factory.Nd4j.rand(new int[]{2* 3, 8});
			Dim Wo As INDArray = Nd4j.rand(New Integer(){2* 3, 8})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray mask = org.nd4j.linalg.factory.Nd4j.rand(10, 5).gte(0.2).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim mask As INDArray = Nd4j.rand(10, 5).gte(0.2).castTo(DataType.DOUBLE)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray kP = org.nd4j.linalg.factory.Nd4j.tensorMmul(k, Wk, new int[][]{{1}, {2}}).permutei(0, 2, 3, 1);
			Dim kP As INDArray = Nd4j.tensorMmul(k, Wk, New Integer()(){
				New Integer() {1},
				New Integer() {2}
			}).permutei(0, 2, 3, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray vP = org.nd4j.linalg.factory.Nd4j.tensorMmul(v, Wv, new int[][]{{1}, {2}}).permutei(0, 2, 3, 1);
			Dim vP As INDArray = Nd4j.tensorMmul(v, Wv, New Integer()(){
				New Integer() {1},
				New Integer() {2}
			}).permutei(0, 2, 3, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray qP = org.nd4j.linalg.factory.Nd4j.tensorMmul(q, Wq, new int[][]{{1}, {2}}).permutei(0, 2, 3, 1);
			Dim qP As INDArray = Nd4j.tensorMmul(q, Wq, New Integer()(){
				New Integer() {1},
				New Integer() {2}
			}).permutei(0, 2, 3, 1)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ops.DynamicCustomOp dot_product_attention = org.nd4j.linalg.api.ops.DynamicCustomOp.builder("dot_product_attention").addInputs(qP, kP, vP, mask).addIntegerArguments(1, 0).build();
			Dim dot_product_attention As DynamicCustomOp = DynamicCustomOp.builder("dot_product_attention").addInputs(qP, kP, vP, mask).addIntegerArguments(1, 0).build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] outputs = org.nd4j.linalg.factory.Nd4j.exec(dot_product_attention);
			Dim outputs() As INDArray = Nd4j.exec(dot_product_attention)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray attOut = outputs[0].permutei(0, 3, 1, 2).reshape(k.size(0), q.size(2), Wv.size(0) * Wv.size(1));
			Dim attOut As INDArray = outputs(0).permutei(0, 3, 1, 2).reshape(k.size(0), q.size(2), Wv.size(0) * Wv.size(1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray out = org.nd4j.linalg.factory.Nd4j.tensorMmul(attOut, Wo, new int[][]{{2}, {0}}).permutei(0, 2, 1);
			Dim [out] As INDArray = Nd4j.tensorMmul(attOut, Wo, New Integer()(){
				New Integer() {2},
				New Integer() {0}
			}).permutei(0, 2, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray finalOut = out.norm2();
			Dim finalOut As INDArray = [out].norm2()

			For Each orderWeights As Char In New Char(){"f"c, "c"c}
				For Each orderInput As Char In New Char(){"f"c, "c"c}
					log.info("-*- Starting Test: input Order = {}, weightOrder = {} -*-", orderInput, orderWeights)


					Dim sd As SameDiff = SameDiff.create()
					Dim sdQ As SDVariable = sd.var("q", q.dup(orderInput))
					Dim sdK As SDVariable = sd.var("k", k.dup(orderInput))
					Dim sdV As SDVariable = sd.var("v", v.dup(orderInput))
					Dim sdWq As SDVariable = sd.var("Wq", Wq.dup(orderWeights))
					Dim sdWk As SDVariable = sd.var("Wk", Wk.dup(orderWeights))
					Dim sdWv As SDVariable = sd.var("Wv", Wv.dup(orderWeights))
					Dim sdWo As SDVariable = sd.var("Wo", Wo.dup(orderWeights))
					Dim sdMask As SDVariable = sd.constant("mask", mask)


					Dim t As SDVariable = sd.nn_Conflict.multiHeadDotProductAttention(sdQ, sdK, sdV, sdWq, sdWk, sdWv, sdWo, sdMask, True)
					t.norm2("out")

					Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", finalOut).gradientCheck(False).gradCheckSkipVariables("mask"))

					assertNull(err)
				Next orderInput
			Next orderWeights
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSufficientStatisticsOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSufficientStatisticsOp(ByVal backend As Nd4jBackend)
			Dim data As INDArray = Nd4j.createFromArray(New Double(){5.5, 0.0, 0.3, 5.5, 1.5, 0.0, 1.3, 6.5, 8.6, 0.0, 0.0, 0.4, 2.5, 1.0, 0.3, 4.5, 1.5, 1.0, 1.3, 1.5, 3.5, 0.0, 1.3, 2.5, 2.6, 2.0, 3.0, 1.4, 4.5, 1.0, 0.3, 0.5}).reshape(ChrW(2), 2, 2, 4)
			Dim axes As INDArray = Nd4j.linspace(DataType.LONG, 0, 3, 1)

			Dim op As New OpTestCase(New SufficientStatistics(data, axes))

			Dim expected1 As INDArray = Nd4j.scalar(8.0)
			Dim expected2 As INDArray = Nd4j.createFromArray(New Double(){ 30.2, 5.0, 7.8, 22.8 })
			Dim expected3 As INDArray = Nd4j.createFromArray(New Double(){ 154.22, 7.0, 14.34, 103.62 })

			op.expectedOutput(0, expected1)
			op.expectedOutput(1, expected2)
			op.expectedOutput(2, expected3)

			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStandardDeviation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStandardDeviation(ByVal backend As Nd4jBackend)

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim sameDiff As SameDiff = SameDiff.create()

				Dim [in] As INDArray = Nd4j.linspace(1, 8, 8).reshape(ChrW(2), 4)
				Dim input As SDVariable = sameDiff.var([in])
				Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 2, 2, 2, 2 })

				If keepDims Then
					expected = expected.reshape(ChrW(1), 4)
				End If

				Dim output As SDVariable = (New StandardDeviation(sameDiff, input, False, keepDims, New Integer(){0})).outputVariable()

				Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSquaredNorm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSquaredNorm(ByVal backend As Nd4jBackend)

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim sameDiff As SameDiff = SameDiff.create()

				Dim [in] As INDArray = Nd4j.linspace(1, 4, 4)
				Dim input As SDVariable = sameDiff.var([in])
				Dim expected As INDArray = Nd4j.scalar(30.0000)
				If keepDims Then
					expected = expected.reshape(ChrW(1))
				End If

				Dim output As SDVariable = (New SquaredNorm(sameDiff, input, keepDims, New Integer(){0})).outputVariable()

				Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShannonEntropy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShannonEntropy(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'AB 2020/02/11 https://github.com/eclipse/deeplearning4j/issues/8695

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 4, 4).castTo(DataType.DOUBLE)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As INDArray = Nd4j.scalar(-69.68162)

			Dim output As SDVariable = (New ShannonEntropy(sameDiff, input, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEntropy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEntropy(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 4, 4)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As Double = -10.2273

			Dim output As SDVariable = (New Entropy(sameDiff, input, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), Nd4j.scalar(expected))

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAMean(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 5.0000, 6.0000, 7.0000, 8.0000 })

			Dim output As SDVariable = (New AMean(sameDiff, input, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMean(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 5.0000, 6.0000, 7.0000, 8.0000 })

			Dim output As SDVariable = (New Mean(sameDiff, input, False, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm1(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 15.0000, 18.0000, 21.0000, 24.0000 })

			Dim output As SDVariable = (New Norm1(sameDiff, input, False, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 10.3441, 11.8322, 13.3791, 14.9666 })

			Dim output As SDVariable = (New Norm2(sameDiff, input, False, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormMax(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim input As SDVariable = sameDiff.var([in])
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 9.0000, 10.0000, 11.0000, 12.0000 })

			Dim output As SDVariable = (New NormMax(sameDiff, input, False, New Integer(){0})).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxCrossEntropyWithLogitsLoss(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxCrossEntropyWithLogitsLoss(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing()

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim labels As INDArray = Nd4j.createFromArray(New Double(){0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0}).reshape(ChrW(2), 3, 4)

			Dim logits As INDArray = Nd4j.linspace(DataType.DOUBLE, 0.1, 0.1, 24).reshape(ChrW(2), 3, 4)
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 0.26328, 1.46328, 1.72656, 0.0, 0.26328, 0.0, 1.46328, 0.26328, 1.72656, 0.0, 1.72656, 1.46328 }).reshape(ChrW(3), 4)

			Dim sdLogits As SDVariable = sameDiff.var("logits", logits)
			Dim sdLabels As SDVariable = sameDiff.var("labels", labels)
			Dim loss As SDVariable = sameDiff.math().abs(sdLogits)


			Dim output As SDVariable = (New SoftmaxCrossEntropyWithLogitsLoss(sameDiff, sdLogits, sdLabels, 0)).outputVariable()
			sameDiff.setLossVariables(output)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub
	End Class

End Namespace