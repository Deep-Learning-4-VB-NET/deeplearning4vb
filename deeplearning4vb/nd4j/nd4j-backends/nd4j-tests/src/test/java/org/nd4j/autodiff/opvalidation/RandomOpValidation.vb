Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
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
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports All = org.nd4j.linalg.api.ops.impl.reduce.bool.All
Imports RandomBernoulli = org.nd4j.linalg.api.ops.random.custom.RandomBernoulli
Imports RandomExponential = org.nd4j.linalg.api.ops.random.custom.RandomExponential
Imports BinomialDistribution = org.nd4j.linalg.api.ops.random.impl.BinomialDistribution
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.function
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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.RNG) public class RandomOpValidation extends BaseOpValidation
	Public Class RandomOpValidation
		Inherits BaseOpValidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomOpsSDVarShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomOpsSDVarShape(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim failed As IList(Of String) = New List(Of String)()

			For Each shape As Long() In Arrays.asList(New Long(){1000}, New Long(){100, 10}, New Long(){40, 5, 5})

				For i As Integer = 0 To 3
					Dim arr As INDArray = Nd4j.createFromArray(shape).castTo(DataType.INT)

					Nd4j.Random.setSeed(12345)
					Dim sd As SameDiff = SameDiff.create()
					Dim shapeVar As SDVariable = sd.constant("shape", arr)
					Dim otherVar As SDVariable = sd.var("misc", Nd4j.rand(shape))

					Dim rand As SDVariable
					Dim checkFn As [Function](Of INDArray, String)
					Dim name As String
					Select Case i
						Case 0
							name = "randomUniform"
							rand = sd.random().uniform(1, 2, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim min As Double = [in].minNumber().doubleValue()
							Dim max As Double = [in].maxNumber().doubleValue()
							Dim mean As Double = [in].meanNumber().doubleValue()
							If min >= 1 AndAlso max <= 2 AndAlso ([in].length() = 1 OrElse Math.Abs(mean - 1.5) < 0.2) Then
								Return Nothing
							End If
							Return "Failed: min = " & min & ", max = " & max & ", mean = " & mean
							End Function
						Case 1
							name = "randomNormal"
							rand = sd.random().normal(1, 1, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim mean As Double = [in].meanNumber().doubleValue()
							Dim stdev As Double = [in].std(True).getDouble(0)
							If [in].length() = 1 OrElse (Math.Abs(mean - 1) < 0.2 AndAlso Math.Abs(stdev - 1) < 0.2) Then
								Return Nothing
							End If
							Return "Failed: mean = " & mean & ", stdev = " & stdev
							End Function
						Case 2
							name = "randomBernoulli"
							rand = sd.random().bernoulli(0.5, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim mean As Double = [in].meanNumber().doubleValue()
							Dim min As Double = [in].minNumber().doubleValue()
							Dim max As Double = [in].maxNumber().doubleValue()
							Dim sum0 As Integer = Transforms.not([in].castTo(DataType.BOOL)).castTo(DataType.DOUBLE).sumNumber().intValue()
							Dim sum1 As Integer = [in].sumNumber().intValue()
							If ([in].length() = 1 AndAlso min = max AndAlso (min = 0 OrElse min = 1)) OrElse (Math.Abs(mean - 0.5) < 0.1 AndAlso min = 0 AndAlso max = 1 AndAlso (sum0 + sum1) = [in].length()) Then
								Return Nothing
							End If
							Return "Failed: bernoulli - sum0 = " & sum0 & ", sum1 = " & sum1
							End Function
						Case 3
							name = "randomExponential"
							Const lambda As Double = 2
							rand = sd.random().exponential(lambda, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim mean As Double = [in].meanNumber().doubleValue()
							Dim min As Double = [in].minNumber().doubleValue()
							Dim std As Double = [in].stdNumber().doubleValue()
							'mean: 1/lambda; std: 1/lambda
							If ([in].length() = 1 AndAlso min > 0) OrElse (Math.Abs(mean - 1 / lambda) < 0.1 AndAlso min >= 0 AndAlso Math.Abs(std - 1 / lambda) < 0.1) Then
								Return Nothing
							End If
							Return "Failed: exponential: mean=" & mean & ", std = " & std & ", min=" & min
							End Function
						Case Else
							Throw New Exception()
					End Select

					Dim loss As SDVariable
					If shape.Length > 0 Then
						loss = rand.std(True)
					Else
						loss = rand.mean()
					End If

					Dim msg As String = name & " - " & Arrays.toString(shape)
					Dim tc As TestCase = (New TestCase(sd)).gradientCheck(False).testName(msg).expected(rand, checkFn).testFlatBufferSerialization(TestCase.TestSerialization.NONE) 'Can't compare values due to randomness

					log.info("TEST: " & msg)

					Dim err As String = OpValidation.validate(tc, True)
					If err IsNot Nothing Then
						failed.Add(err)
					End If
				Next i
			Next shape

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomOpsLongShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomOpsLongShape(ByVal backend As Nd4jBackend)
			Dim failed As IList(Of String) = New List(Of String)()

			For Each shape As Long() In Arrays.asList(New Long(){1000}, New Long(){100, 10}, New Long(){40, 5, 5})

				For i As Integer = 0 To 5

					Nd4j.Random.setSeed(12345)
					Dim sd As SameDiff = SameDiff.create()

					Dim rand As SDVariable
					Dim checkFn As [Function](Of INDArray, String)
					Dim name As String
					Select Case i
						Case 0
							name = "randomBernoulli"
							rand = sd.random().bernoulli(0.5, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim mean As Double = [in].meanNumber().doubleValue()
							Dim min As Double = [in].minNumber().doubleValue()
							Dim max As Double = [in].maxNumber().doubleValue()
							Dim sum0 As Integer = Transforms.not([in].castTo(DataType.BOOL)).castTo(DataType.DOUBLE).sumNumber().intValue()
							Dim sum1 As Integer = [in].sumNumber().intValue()
							If ([in].length() = 1 AndAlso min = max AndAlso (min = 0 OrElse min = 1)) OrElse (Math.Abs(mean - 0.5) < 0.1 AndAlso min = 0 AndAlso max = 1 AndAlso (sum0 + sum1) = [in].length()) Then
								Return Nothing
							End If
							Return "Failed: bernoulli - sum0 = " & sum0 & ", sum1 = " & sum1
							End Function
						Case 1
							name = "normal"
							rand = sd.random().normal(1, 2, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim mean As Double = [in].meanNumber().doubleValue()
							Dim stdev As Double = [in].std(True).getDouble(0)
							If [in].length() = 1 OrElse (Math.Abs(mean - 1) < 0.2 AndAlso Math.Abs(stdev - 2) < 0.1) Then
								Return Nothing
							End If
							Return "Failed: mean = " & mean & ", stdev = " & stdev
							End Function
						Case 2
							name = "randomBinomial"
							rand = sd.random().binomial(4, 0.5, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim iter As New NdIndexIterator([in].shape())
							Do While iter.MoveNext()
								Dim idx() As Long = iter.Current
								Dim d As Double = [in].getDouble(idx)
								If d < 0 OrElse d > 4 OrElse d <> Math.Floor(d) Then
									Return "Falied - binomial: indexes " & Arrays.toString(idx) & ", value " & d
								End If
							Loop
							Return Nothing
							End Function
						Case 3
							name = "randomUniform"
							rand = sd.random().uniform(1, 2, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim min As Double = [in].minNumber().doubleValue()
							Dim max As Double = [in].maxNumber().doubleValue()
							Dim mean As Double = [in].meanNumber().doubleValue()
							If min >= 1 AndAlso max <= 2 AndAlso ([in].length() = 1 OrElse Math.Abs(mean - 1.5) < 0.1) Then
								Return Nothing
							End If
							Return "Failed: min = " & min & ", max = " & max & ", mean = " & mean
							End Function
						Case 4
							If OpValidationSuite.IGNORE_FAILING Then
								'https://github.com/eclipse/deeplearning4j/issues/6036
								Continue For
							End If
							name = "truncatednormal"
							rand = sd.random().normalTruncated(1, 2, DataType.DOUBLE, shape)
							checkFn = Function([in])
							Dim mean As Double = [in].meanNumber().doubleValue()
							Dim stdev As Double = [in].std(True).getDouble(0)
							If [in].length() = 1 OrElse (Math.Abs(mean - 1) < 0.1 AndAlso Math.Abs(stdev - 2) < 0.2) Then
								Return Nothing
							End If
							Return "Failed: mean = " & mean & ", stdev = " & stdev
							End Function
						Case 5
							name = "lognormal"
							rand = sd.random().logNormal(1, 2, DataType.DOUBLE, shape)
							'Note: lognormal parameters are mean and stdev of LOGARITHM of values
							checkFn = Function([in])
							Dim log As INDArray = Transforms.log([in], True)
							Dim mean As Double = log.meanNumber().doubleValue()
							Dim stdev As Double = log.std(True).getDouble(0)
							If [in].length() = 1 OrElse (Math.Abs(mean - 1) < 0.2 AndAlso Math.Abs(stdev - 2) < 0.1) Then
								Return Nothing
							End If
							Return "Failed: mean = " & mean & ", stdev = " & stdev
							End Function
						Case Else
							Throw New Exception()
					End Select

					Dim loss As SDVariable
					If shape.Length > 0 Then
						loss = rand.std(True)
					Else
						loss = rand.mean()
					End If

					Dim msg As String = name & " - " & Arrays.toString(shape)
					Dim tc As TestCase = (New TestCase(sd)).gradientCheck(False).testName(msg).expected(rand, checkFn).testFlatBufferSerialization(TestCase.TestSerialization.NONE) 'Can't compare values due to randomness

					log.info("TEST: " & msg)

					Dim err As String = OpValidation.validate(tc, True)
					If err IsNot Nothing Then
						failed.Add(err)
					End If
				Next i
			Next shape

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomBinomial()
		Public Overridable Sub testRandomBinomial()

			Dim z As INDArray = Nd4j.create(New Long(){10})
	'        Nd4j.getExecutioner().exec(new BinomialDistribution(z, 4, 0.5));
			Nd4j.Executioner.exec(New BinomialDistribution(z, 4, 0.5))

			Console.WriteLine(z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUniformRankSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUniformRankSimple(ByVal backend As Nd4jBackend)

			Dim arr As INDArray = Nd4j.createFromArray(New Double(){100.0})
	'        OpTestCase tc = new OpTestCase(DynamicCustomOp.builder("randomuniform")
	'                .addInputs(arr)
	'                .addOutputs(Nd4j.createUninitialized(new long[]{100}))
	'                .addFloatingPointArguments(0.0, 1.0)
	'                .build());

	'        OpTestCase tc = new OpTestCase(new DistributionUniform(arr, Nd4j.createUninitialized(new long[]{100}), 0, 1));
			Dim tc As New OpTestCase(New RandomBernoulli(arr, Nd4j.createUninitialized(New Long(){100}), 0.5))

			tc.expectedOutput(0, LongShapeDescriptor.fromShape(New Long(){100}, DataType.FLOAT), Function([in])
			Dim min As Double = [in].minNumber().doubleValue()
			Dim max As Double = [in].maxNumber().doubleValue()
			Dim mean As Double = [in].meanNumber().doubleValue()
			If min >= 0 AndAlso max <= 1 AndAlso ([in].length() = 1 OrElse Math.Abs(mean - 0.5) < 0.2) Then
				Return Nothing
			End If
			Return "Failed: min = " & min & ", max = " & max & ", mean = " & mean
			End Function)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)

			Dim d As Double = arr.getDouble(0)

			assertEquals(100.0, d, 0.0)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomExponential(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomExponential(ByVal backend As Nd4jBackend)
			Dim length As Long = 1_000_000
			Dim shape As INDArray = Nd4j.createFromArray(New Double(){length})
			Dim [out] As INDArray = Nd4j.createUninitialized(New Long(){length})
			Dim lambda As Double = 2
			Dim op As New RandomExponential(shape, [out], lambda)

			Nd4j.Executioner.exec(op)

			Dim min As Double = [out].minNumber().doubleValue()
			Dim mean As Double = [out].meanNumber().doubleValue()
			Dim std As Double = [out].stdNumber().doubleValue()

			Dim expMean As Double = 1.0/lambda
			Dim expStd As Double = 1.0/lambda

			assertTrue(min >= 0.0)
			assertEquals(expMean, mean, 0.1,"mean")
			assertEquals(expStd, std, 0.1,"std")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRange()
		Public Overridable Sub testRange()
			'Technically deterministic, not random...

			Dim testCases()() As Double = {
				New Double() {3, 18, 3},
				New Double() {3, 1, -0.5},
				New Double() {0, 5, 1}
			}

			Dim exp As IList(Of INDArray) = New List(Of INDArray) From {Nd4j.create(New Double(){3, 6, 9, 12, 15}).castTo(DataType.FLOAT), Nd4j.create(New Double(){3, 2.5, 2, 1.5}).castTo(DataType.FLOAT), Nd4j.create(New Double(){0, 1, 2, 3, 4}).castTo(DataType.FLOAT)}

			For i As Integer = 0 To testCases.Length - 1
				Dim d() As Double = testCases(i)
				Dim e As INDArray = exp(i)

				Dim sd As SameDiff = SameDiff.create()
				Dim range As SDVariable = sd.range(d(0), d(1), d(2), DataType.FLOAT)

				Dim loss As SDVariable = range.std(True)

				Dim tc As TestCase = (New TestCase(sd)).expected(range, e).testName(Arrays.toString(d)).gradientCheck(False)

				assertNull(OpValidation.validate(tc))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllEmptyReduce(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllEmptyReduce(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(True, True, True)
			Dim all As New All(x)
			all.setEmptyReduce(True) 'For TF compatibility - empty array for axis (which means no-op - and NOT all array reduction)
			Dim [out] As INDArray = Nd4j.exec(all)
			assertEquals(x, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUniformDtype(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUniformDtype(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			For Each t As DataType In New DataType(){DataType.FLOAT, DataType.DOUBLE}
				Dim sd As SameDiff = SameDiff.create()
				Dim shape As SDVariable = sd.constant("shape", Nd4j.createFromArray(1, 100))
				Dim [out] As SDVariable = sd.random_Conflict.uniform(0, 10, t, 1, 100)
				Dim arr As INDArray = [out].eval()
				assertEquals(t, arr.dataType())
				If t.Equals(DataType.DOUBLE) Then
					Dim min As Double = arr.minNumber().doubleValue()
					Dim max As Double = arr.maxNumber().doubleValue()
					Dim mean As Double = arr.meanNumber().doubleValue()
					assertEquals(0, min, 0.5)
					assertEquals(10, max, 0.5)
					assertEquals(5.5, mean, 1)
				ElseIf t.Equals(DataType.FLOAT) Then
					Dim min As Single = arr.minNumber().floatValue()
					Dim max As Single = arr.maxNumber().floatValue()
					Dim mean As Single = arr.meanNumber().floatValue()
					assertEquals(0, min, 0.5)
					assertEquals(10, max, 0.5)
					assertEquals(5.0, mean, 1)
				End If
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomExponential2()
		Public Overridable Sub testRandomExponential2()
			Nd4j.Random.setSeed(12345)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("random_exponential").addInputs(Nd4j.createFromArray(100)).addOutputs(Nd4j.create(DataType.FLOAT, 100)).addFloatingPointArguments(0.5).build()

			Nd4j.exec(op)

			Dim [out] As INDArray = op.getOutputArgument(0)
			Dim count0 As Integer = [out].eq(0.0).castTo(DataType.INT32).sumNumber().intValue()
			Dim count1 As Integer = [out].eq(1.0).castTo(DataType.INT32).sumNumber().intValue()

			assertEquals(0, count0)
			assertEquals(0, count1)

			Dim min As Double = [out].minNumber().doubleValue()
			Dim max As Double = [out].maxNumber().doubleValue()

			assertTrue(min > 0.0,min.ToString())
			assertTrue(max > 1.0,max.ToString())
		End Sub
	End Class

End Namespace