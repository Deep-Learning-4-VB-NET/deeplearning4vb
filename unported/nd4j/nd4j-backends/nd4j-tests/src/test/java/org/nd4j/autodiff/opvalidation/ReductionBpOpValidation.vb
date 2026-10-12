Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpTestCase = org.nd4j.autodiff.validation.OpTestCase
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CumSumBp = org.nd4j.linalg.api.ops.impl.reduce.bp.CumSumBp
Imports MaxBp = org.nd4j.linalg.api.ops.impl.reduce.bp.MaxBp
Imports MeanBp = org.nd4j.linalg.api.ops.impl.reduce.bp.MeanBp
Imports MinBp = org.nd4j.linalg.api.ops.impl.reduce.bp.MinBp
Imports Norm1Bp = org.nd4j.linalg.api.ops.impl.reduce.bp.Norm1Bp
Imports Norm2Bp = org.nd4j.linalg.api.ops.impl.reduce.bp.Norm2Bp
Imports NormMaxBp = org.nd4j.linalg.api.ops.impl.reduce.bp.NormMaxBp
Imports ProdBp = org.nd4j.linalg.api.ops.impl.reduce.bp.ProdBp
Imports StandardDeviationBp = org.nd4j.linalg.api.ops.impl.reduce.bp.StandardDeviationBp
Imports SumBp = org.nd4j.linalg.api.ops.impl.reduce.bp.SumBp
Imports VarianceBp = org.nd4j.linalg.api.ops.impl.reduce.bp.VarianceBp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertNull

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
'ORIGINAL LINE: @Slf4j @NativeTag public class ReductionBpOpValidation extends BaseOpValidation
	Public Class ReductionBpOpValidation
		Inherits BaseOpValidation

		Private initialType As DataType

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.create(1)
			initialType = Nd4j.dataType()

			Nd4j.DataType = DataType.DOUBLE
			Nd4j.Random.setSeed(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.DataType = initialType
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceSumBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduceSumBP(ByVal backend As Nd4jBackend)
			'Full array reduction

			'reduce_sum_bp op: has 2 inputs (original pre-reduce input, and gradient at output (epsilon))
			'out = sum_j (in_j) -> dL/dIn = dL/dOut * dOut/dIn = dL/dOut

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim dLdInExpected As INDArray = Nd4j.valueArrayOf(preReduceInput.shape(), 0.5)
				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New SumBp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceSumAlongDim0BP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduceSumAlongDim0BP(ByVal backend As Nd4jBackend)
			'Reduction along dimension
			'Inputs/outputs as before - but note that the output is no longer a scalar

			'Note: when reducing [3,4] along dimension 0 -> 4 TADs of length 3
			'We have one epsilon/gradient for each of the 4 TADs -> dL/dOut length is 4

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = Nd4j.createUninitialized(preReduceInput.shape())
				For i As Integer = 0 To 2
					dLdInExpected_0.putRow(i, dLdOut_0)
				Next i

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New SumBp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceSumAlongDim1BP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduceSumAlongDim1BP(ByVal backend As Nd4jBackend)
			'Reduction along dimension
			'Inputs/outputs as before - but note that the output is no longer a scalar

			'Note: when reducing [3,4] along dimension 1 -> 3 TADs of length 4
			'We have one epsilon/gradient for each of the 3 TADs -> dL/dOut length is 3

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

				Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
				Dim dLdInExpected_1 As INDArray = Nd4j.createUninitialized(preReduceInput.shape())
				For i As Integer = 0 To 3
					dLdInExpected_1.putColumn(i, dLdOut_1)
				Next i

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New SumBp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err)
			Next keepDims
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanBP(ByVal backend As Nd4jBackend)

			'dL/dIn_i = dL/dOut * dOut/dIn_i = dL/dOut * (1/N * sum_j (in_j))
			'         = 1/N * dL/dOut
			' i.e., same as SUM case but divided by N
			'NOTE: N = num values in array
			'But for "along dimension" case - it's the number of elements in that TAD

			'Full array reduction
			'reduce_mean_bp op: has 2 inputs (original pre-reduce input, and gradient at output (epsilon))

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim dLdInExpected As INDArray = Nd4j.valueArrayOf(preReduceInput.shape(), 0.5 / preReduceInput.length())
				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New MeanBp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanBP_Rank1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanBP_Rank1(ByVal backend As Nd4jBackend)
			Dim dLdOut As INDArray = Nd4j.scalar(0.5)
			Dim preReduceInput As INDArray = Nd4j.create(New Double(){2, 3, 4}, New Long(){3})
			Dim dLdInExp As INDArray = Nd4j.valueArrayOf(New Long(){3}, 0.5 / 3)

			Dim dLdIn As INDArray = Nd4j.createUninitialized(New Long(){3})

			Dim err As String = OpValidation.validate((New OpTestCase(New MeanBp(preReduceInput, dLdOut, dLdIn, False))).expectedOutput(0, dLdInExp))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanAlongDim0BP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanAlongDim0BP(ByVal backend As Nd4jBackend)
			'Reduction along dimension
			'Inputs/outputs as before - but note that the output is no longer a scalar

			'Note: when reducing [3,4] along dimension 0 -> 4 TADs of length 3 -> N=3 -> dL/dIn_i = dL/dOut * 1/3
			'We have one epsilon/gradient for each of the 4 TADs -> dL/dOut length is 4

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape("c"c, 3, 4)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = Nd4j.createUninitialized(preReduceInput.shape())
				For i As Integer = 0 To 2
					dLdInExpected_0.putRow(i, dLdOut_0.div(3))
				Next i

				Dim msg As String = "keepDims=" & keepDims
				log.info("Starting test: " & msg)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)
				Dim err As String = OpValidation.validate((New OpTestCase(New MeanBp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanAlongDim1BP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanAlongDim1BP(ByVal backend As Nd4jBackend)
			'Reduction along dimension
			'Inputs/outputs as before - but note that the output is no longer a scalar

			'Note: when reducing [3,4] along dimension 1 -> 3 TADs of length 4 -> N=4 -> dL/dIn_i = dL/dOut * 1/4
			'We have one epsilon/gradient for each of the 3 TADs -> dL/dOut length is 3

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

				Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
				Dim dLdInExpected_1 As INDArray = Nd4j.createUninitialized(preReduceInput.shape())
				For i As Integer = 0 To 3
					dLdInExpected_1.putColumn(i, dLdOut_1.div(4))
				Next i

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New MeanBp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err)
			Next keepDims
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMinBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMinBP(ByVal backend As Nd4jBackend)
			'Full array min reduction

			'dL/dIn_i  = dL/dOut * dOut/dIn_i
			'          = dL/dOut                   if in_i == out (== min(in))
			'          = 0                         otherwise

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				preReduceInput.putScalar(New Integer(){2, 2}, -1) 'Minimum value at position [2,2]
				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim dLdInExpected As INDArray = Nd4j.zeros(preReduceInput.shape())
				dLdInExpected.putScalar(New Integer(){2, 2}, 0.5) 'Minimum value: position at [2,2]
				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

	'            String err = OpValidation.validate(new OpTestCase(
	'                    DynamicCustomOp.builder("reduce_min_bp")
	'                            .addInputs(preReduceInput, dLdOut)
	'                            .addOutputs(dLdIn)
	'                            //First int arg: Keep dimensions. Lack of other (dimension) args: means "full array reduce"
	'//                            .addIntegerArguments(keepDims ? 1 : 0)
	'                            .addFloatingPointArguments(keepDims ? 1.0 : 0.0)
	'                            .build())
	'                    .expectedOutput(0, dLdInExpected));

				Dim err As String = OpValidation.validate((New OpTestCase(New MinBp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMinAlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMinAlongDimensionBP(ByVal backend As Nd4jBackend)
			'Full array min reduction

			'dL/dIn_i  = dL/dOut * dOut/dIn_i
			'          = dL/dOut                   if in_i == out (== min(in))
			'          = 0                         otherwise

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 16, 16).reshape(ChrW(4), 4)
				preReduceInput.putScalar(0, 0, -1)
				preReduceInput.putScalar(1, 1, -2)
				preReduceInput.putScalar(2, 2, -3)
				preReduceInput.putScalar(3, 3, -4)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = Nd4j.create(preReduceInput.shape()) 'All 0s except along diagonal
				dLdInExpected_0.putScalar(0, 0, 1)
				dLdInExpected_0.putScalar(1, 1, 2)
				dLdInExpected_0.putScalar(2, 2, 3)
				dLdInExpected_0.putScalar(3, 3, 4)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(4, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New MinBp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
				assertNull(err)


				Dim reducedShape_1() As Long = (If(keepDims, New Long(){4, 1}, New Long()){4})
				Dim dLdInExpected_1 As INDArray = dLdInExpected_0 'Same here, only because the maximums are along the diagonal

				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_1)
				dLdIn = Nd4j.createUninitialized(4, 4)

				err = OpValidation.validate((New OpTestCase(New MinBp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err, err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxBP(ByVal backend As Nd4jBackend)
			'Full array max reduction

			'dL/dIn_i  = dL/dOut * dOut/dIn_i
			'          = dL/dOut                   if in_i == out (== max(in))
			'          = 0                         otherwise

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				preReduceInput.putScalar(New Integer(){2, 2}, 20) 'Maximum value at position [2,2]
				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim dLdInExpected As INDArray = Nd4j.zeros(preReduceInput.shape())
				dLdInExpected.putScalar(New Integer(){2, 2}, 0.5) 'Maximum value: position at [2,2]
				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New MaxBp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxAlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxAlongDimensionBP(ByVal backend As Nd4jBackend)
			'Full array min reduction

			'dL/dIn_i  = dL/dOut * dOut/dIn_i
			'          = dL/dOut                   if in_i == out (== min(in))
			'          = 0                         otherwise

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 16, 16).reshape(ChrW(4), 4)
				preReduceInput.putScalar(0, 0, 20)
				preReduceInput.putScalar(1, 1, 21)
				preReduceInput.putScalar(2, 2, 22)
				preReduceInput.putScalar(3, 3, 23)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = Nd4j.create(preReduceInput.shape())
				dLdInExpected_0.putScalar(0, 0, 1)
				dLdInExpected_0.putScalar(1, 1, 2)
				dLdInExpected_0.putScalar(2, 2, 3)
				dLdInExpected_0.putScalar(3, 3, 4)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(4, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New MaxBp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
				assertNull(err)


				Dim reducedShape_1() As Long = (If(keepDims, New Long(){4, 1}, New Long()){4})
				Dim dLdInExpected_1 As INDArray = dLdInExpected_0 'Same here, only because the maximums are along the diagonal

				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_1)
				dLdIn = Nd4j.createUninitialized(4, 4)

				err = OpValidation.validate((New OpTestCase(New MaxBp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testProdBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testProdBP(ByVal backend As Nd4jBackend)
			'Full array product reduction

			'dL/dIn_i  = dL/dOut * dOut/dIn_i
			'          = dL/dOut * d(prod(in))/dIn_i
			'          = dL/dOut * (prod(in) / in_i)

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim prod As Double = preReduceInput.prodNumber().doubleValue()
				Dim dLdInExpected As INDArray = Nd4j.valueArrayOf(preReduceInput.shape(), prod).divi(preReduceInput).muli(0.5)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New ProdBp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testProdAlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testProdAlongDimensionBP(ByVal backend As Nd4jBackend)
			'dL/dIn_i  = dL/dOut * dOut/dIn_i
			'          = dL/dOut * d(prod(in))/dIn_i
			'          = dL/dOut * (prod(in) / in_i)

			For Each keepDims As Boolean In New Boolean(){False, True}
				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim prod_0 As INDArray = preReduceInput.prod(0)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = Nd4j.create(3, 4)
				For i As Integer = 0 To 2
					dLdInExpected_0.putRow(i, prod_0)
				Next i
				dLdInExpected_0.divi(preReduceInput) 'Currently: prod(in)/in_i (along dim 0)
				dLdInExpected_0.muliRowVector(dLdOut_0)
				'System.out.println(dLdInExpected_0);
	'            
	'            [[   45.0000,  120.0000,  231.0000,  384.0000],
	'             [    9.0000,   40.0000,   99.0000,  192.0000],
	'             [    5.0000,   24.0000,   63.0000,  128.0000]]
	'             

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New ProdBp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
				assertNull(err)


				Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
				Dim prod_1 As INDArray = preReduceInput.prod(1)
				Dim dLdInExpected_1 As INDArray = Nd4j.create(3, 4)
				For i As Integer = 0 To 3
					dLdInExpected_1.putColumn(i, prod_1)
				Next i
				dLdInExpected_1.divi(preReduceInput)
				dLdInExpected_1.muliColumnVector(dLdOut_1.reshape(ChrW(3), 1)) 'Reshape is a hack around https://github.com/eclipse/deeplearning4j/issues/5530
				'System.out.println(dLdInExpected_1);
	'            
	'            [[   24.0000,   12.0000,    8.0000,    6.0000],
	'             [  672.0000,  560.0000,  480.0000,  420.0000],
	'             [ 3960.0000, 3564.0000, 3240.0000, 2970.0000]]
	'             


				dLdIn = Nd4j.createUninitialized(3, 4)
				err = OpValidation.validate((New OpTestCase(New ProdBp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err, err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdevBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdevBP(ByVal backend As Nd4jBackend)
			'If out = stdev(in) then:
			'dL/dIn = dL/dOut * dOut/dIn
			'dOut/dIn_i = (in_i-mean)/(stdev * (n-1))
			'OR: n instead of n-1, if not bias corrected

			For Each biasCorrected As Boolean In New Boolean(){True, False}
				For Each keepDims As Boolean In New Boolean(){False, True}

					Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
					Dim dLdOut As INDArray
					If keepDims Then
						dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
					Else
						dLdOut = Nd4j.scalar(0.5)
					End If

					Dim stdev As Double = preReduceInput.stdNumber(biasCorrected).doubleValue()
					Dim mean As Double = preReduceInput.meanNumber().doubleValue()

					Dim divisor As Long = If(biasCorrected, (preReduceInput.length() - 1), preReduceInput.length())

					Dim dLdInExp As INDArray = preReduceInput.dup().subi(mean).divi(stdev * divisor).muli(0.5) '* dL/dOut

					Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

					Dim err As String = OpValidation.validate((New OpTestCase(New StandardDeviationBp(preReduceInput, dLdOut, dLdIn, biasCorrected, keepDims))).expectedOutput(0, dLdInExp))
					assertNull(err)
				Next keepDims
			Next biasCorrected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdevBP_Rank1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdevBP_Rank1(ByVal backend As Nd4jBackend)
			Dim dLdOut As INDArray = Nd4j.scalar(0.5)
			Dim preReduceInput As INDArray = Nd4j.create(New Double(){2, 3, 4}, New Long(){3})
			Dim stdev As Double = preReduceInput.stdNumber(True).doubleValue()
			Dim mean As Double = preReduceInput.meanNumber().doubleValue()

			Dim dLdInExp As INDArray = preReduceInput.dup().subi(mean).divi(stdev * 2).muli(0.5) '* dL/dOut

	'        System.out.println(dLdInExp.shapeInfoToString());
	'        System.out.println(Arrays.toString(dLdInExp.data().asFloat()));

			Dim dLdIn As INDArray = Nd4j.createUninitialized(New Long(){3})

			Dim err As String = OpValidation.validate((New OpTestCase(New StandardDeviationBp(preReduceInput, dLdOut, dLdIn, True, False))).expectedOutput(0, dLdInExp))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdevAlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdevAlongDimensionBP(ByVal backend As Nd4jBackend)
			'If out = stdev(in) then:
			'dL/dIn = dL/dOut * dOut/dIn
			'dOut/dIn_i = (in_i-mean)/(stdev * (n-1))
			'OR: n instead of n-1, if not bias corrected

			For Each biasCorrected As Boolean In New Boolean(){False, True}
				For Each keepDims As Boolean In New Boolean(){False, True}
					Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
					Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
					Dim divisor As Long = If(biasCorrected, 2, 3)
					Dim mean_0 As INDArray = preReduceInput.mean(0)
					Dim stdev_0 As INDArray = preReduceInput.std(biasCorrected, 0)
					Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)

					Dim dLdInExpected_0 As INDArray = preReduceInput.dup()
					dLdInExpected_0.subiRowVector(mean_0).diviRowVector(stdev_0.mul(divisor)).muliRowVector(dLdOut_0)

					Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)
					Dim err As String = OpValidation.validate((New OpTestCase(New StandardDeviationBp(preReduceInput, dLdOut_0, dLdIn, biasCorrected, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
					assertNull(err)


					divisor = If(biasCorrected, 3, 4)
					Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
					Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
					Dim mean_1 As INDArray = preReduceInput.mean(1)
					Dim stdev_1 As INDArray = preReduceInput.std(biasCorrected, 1)
					Dim dLdInExpected_1 As INDArray = preReduceInput.dup()
					dLdInExpected_1.subiColumnVector(mean_1).diviColumnVector(stdev_1.mul(divisor)).muliColumnVector(dLdOut_1.reshape(ChrW(3), 1))

					dLdIn = Nd4j.createUninitialized(3, 4)
					err = OpValidation.validate((New OpTestCase(New StandardDeviationBp(preReduceInput, dLdOut_1, dLdIn, biasCorrected, keepDims, 1))).expectedOutput(0, dLdInExpected_1))
					assertNull(err, err)
				Next keepDims
			Next biasCorrected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVarianceBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVarianceBP(ByVal backend As Nd4jBackend)
			'If out = variance(in) then:
			'dL/dIn = dL/dOut * dOut/dIn
			'dOut/dIn_i = 2*(in_i-mean)/(n-1)
			'OR: n instead of n-1, if not bias corrected

			For Each biasCorrected As Boolean In New Boolean(){True, False}
				For Each keepDims As Boolean In New Boolean(){False, True}

					Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
					Dim dLdOut As INDArray
					If keepDims Then
						dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
					Else
						dLdOut = Nd4j.scalar(0.5)
					End If

					Dim var As Double = preReduceInput.var(biasCorrected).getDouble(0)
					Dim mean As Double = preReduceInput.meanNumber().doubleValue()

					Dim divisor As Long = If(biasCorrected, (preReduceInput.length() - 1), preReduceInput.length())

					Dim dLdInExp As INDArray = preReduceInput.dup().subi(mean).muli(2.0 / divisor).muli(0.5) '* dL/dOut

					Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

					Dim err As String = OpValidation.validate((New OpTestCase(New VarianceBp(preReduceInput, dLdOut, dLdIn, biasCorrected, keepDims))).expectedOutput(0, dLdInExp))
					assertNull(err)
				Next keepDims
			Next biasCorrected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVarianceAlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVarianceAlongDimensionBP(ByVal backend As Nd4jBackend)
			'If out = variance(in) then:
			'dL/dIn = dL/dOut * dOut/dIn
			'dOut/dIn_i = 2*(in_i-mean)/(n-1)
			'OR: n instead of n-1, if not bias corrected

			For Each biasCorrected As Boolean In New Boolean(){False, True}
				For Each keepDims As Boolean In New Boolean(){False, True}
					Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
					Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
					Dim divisor As Long = If(biasCorrected, 2, 3)
					Dim mean_0 As INDArray = preReduceInput.mean(0)
					Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)

					Dim dLdInExpected_0 As INDArray = preReduceInput.dup()
					dLdInExpected_0.subiRowVector(mean_0).muli(2.0 / divisor).muliRowVector(dLdOut_0)

					Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)
					Dim err As String = OpValidation.validate((New OpTestCase(New VarianceBp(preReduceInput, dLdOut_0, dLdIn, biasCorrected, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
					assertNull(err)

					divisor = If(biasCorrected, 3, 4)
					Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
					Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
					Dim mean_1 As INDArray = preReduceInput.mean(1)
					Dim dLdInExpected_1 As INDArray = preReduceInput.dup()
					dLdInExpected_1.subiColumnVector(mean_1).muli(2.0 / divisor).muliColumnVector(dLdOut_1.reshape(ChrW(3), 1))


					dLdIn = Nd4j.createUninitialized(3, 4)
					err = OpValidation.validate((New OpTestCase(New VarianceBp(preReduceInput, dLdOut_1, dLdIn, biasCorrected, keepDims, 1))).expectedOutput(0, dLdInExpected_1))
					assertNull(err)
				Next keepDims
			Next biasCorrected
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCumSumBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCumSumBP(ByVal backend As Nd4jBackend)
			'Standard case, non-reverse, non-exclusive
			'dL/dIn_i  = sum_j dL/dOut_j * dOut_j/dIn_i
			'          = sum_j dL/dOut_j * d(in_0 + ... + in_j)/dIn_i
			'          = reverseCumSum(dL/dOut_j)

			'Reverse case:
			'dL/dIn_i  = sum_j dL/dOut_j * dOut_j/dIn_i
			'          = sum_j dL/dOut_j * d(in_N + ... + in_j)/dIn_i
			'          = cumSum(dL/dOut_j)

			'Exclusive case:
			'dL/dIn_i  = sum_j dL/dOut_j * dOut_j/dIn_i
			'          = sum_j dL/dOut_j * d(in_0 + ... + in_{i-1})/dIn_i
			'          = reverseCumSumExclusive(dL/dOut_j)

			'Reverse exclusive case
			'dL/dIn_i  = sum_j dL/dOut_j * dOut_j/dIn_i
			'          = sum_j dL/dOut_j * d(in_N + ... + in_j)/dIn_i
			'          = cumSumExclusive(dL/dOut_j)


			For Each exclusive As Boolean In New Boolean(){False, True}
				For Each reverse As Boolean In New Boolean(){False, True}

					Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
					Dim dLdOut As INDArray = Nd4j.valueArrayOf(New Long(){3, 4}, 0.5)
					Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

					Dim dLdInExpected As INDArray
					If exclusive Then
						If reverse Then
							dLdInExpected = Nd4j.create(New Double()(){
								New Double() {0.0, 0.0, 0.0, 0.0},
								New Double() {0.5, 0.5, 0.5, 0.5},
								New Double() {1.0, 1.0, 1.0, 1.0}
							})
						Else
							dLdInExpected = Nd4j.create(New Double()(){
								New Double() {1.0, 1.0, 1.0, 1.0},
								New Double() {0.5, 0.5, 0.5, 0.5},
								New Double() {0.0, 0.0, 0.0, 0.0}
							})
						End If
					Else
						If reverse Then
							dLdInExpected = Nd4j.create(New Double()(){
								New Double() {0.5, 0.5, 0.5, 0.5},
								New Double() {1.0, 1.0, 1.0, 1.0},
								New Double() {1.5, 1.5, 1.5, 1.5}
							})
						Else
							'Standard case
							dLdInExpected = Nd4j.create(New Double()(){
								New Double() {1.5, 1.5, 1.5, 1.5},
								New Double() {1.0, 1.0, 1.0, 1.0},
								New Double() {0.5, 0.5, 0.5, 0.5}
							})
						End If
					End If

					Dim err As String = OpValidation.validate((New OpTestCase(New CumSumBp(preReduceInput, dLdOut, dLdIn, exclusive, reverse, 0))).expectedOutput(0, dLdInExpected))
					If err IsNot Nothing Then
						err = err & " - exclusive=" & exclusive & ", reverse=" & reverse
					End If
					assertNull(err)
				Next reverse
			Next exclusive
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2Bp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2Bp(ByVal backend As Nd4jBackend)
			'dL/dIn = dL/dOut * dOut/dIn
			'       = dL/dOut * x/|x|_2

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4).castTo(DataType.DOUBLE)

				Dim norm2 As Double = preReduceInput.norm2Number().doubleValue()

				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(DataType.DOUBLE, 0.5)
				End If
				Dim dLdInExpected As INDArray = preReduceInput.div(norm2).muli(0.5)
				Dim dLdIn As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, 3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New Norm2Bp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2AlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2AlongDimensionBP(ByVal backend As Nd4jBackend)
			'dL/dIn = dL/dOut * dOut/dIn
			'       = dL/dOut * x/|x|_2

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim norm2_0 As INDArray = preReduceInput.norm2(0)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = preReduceInput.divRowVector(norm2_0).mulRowVector(dLdOut_0)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New Norm2Bp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
				assertNull(err)


				Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
				Dim norm2_1 As INDArray = preReduceInput.norm2(1)
				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
				Dim dLdInExpected_1 As INDArray = preReduceInput.divColumnVector(norm2_1).mulColumnVector(dLdOut_1)
				dLdIn = Nd4j.createUninitialized(3, 4)

				err = OpValidation.validate((New OpTestCase(New Norm2Bp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm1Bp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm1Bp(ByVal backend As Nd4jBackend)
			'dL/dIn = dL/dOut * dOut/dIn
			'       = dL/dOut * sgn(in)

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(-5, 6, 12).addi(0.1).reshape(3, 4)

				Dim sgn As INDArray = Transforms.sign(preReduceInput, True)

				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim dLdInExpected As INDArray = sgn.muli(0.5)
				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New Norm1Bp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm1AlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm1AlongDimensionBP(ByVal backend As Nd4jBackend)
			'dL/dIn = dL/dOut * dOut/dIn
			'       = dL/dOut * sgn(in)

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(-5, 6, 12).addi(0.1).reshape(3, 4)
				Dim sgn As INDArray = Transforms.sign(preReduceInput, True)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = sgn.mulRowVector(dLdOut_0)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New Norm1Bp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
				assertNull(err, err)


				Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
				Dim dLdInExpected_1 As INDArray = sgn.mulColumnVector(dLdOut_1)
				dLdIn = Nd4j.createUninitialized(3, 4)

				err = OpValidation.validate((New OpTestCase(New Norm1Bp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err, err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormMaxBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormMaxBp(ByVal backend As Nd4jBackend)
			'out = max_i (|in_i|)
			'dL/dIn = dL/dOut * dOut/dIn
			'       = dL/dOut * (0 if |x_i| is not max; or sgn(x_i) otherwise)

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim preReduceInput As INDArray = Nd4j.linspace(-5, 6, 12).reshape(ChrW(3), 4)

				Dim sgn As INDArray = Transforms.sign(preReduceInput, True)
				Dim max As INDArray = Nd4j.create(3, 4)
				max.putScalar(2, 3, 1.0)

				Dim dLdOut As INDArray
				If keepDims Then
					dLdOut = Nd4j.valueArrayOf(New Long(){1, 1}, 0.5)
				Else
					dLdOut = Nd4j.scalar(0.5)
				End If
				Dim dLdInExpected As INDArray = sgn.mul(max).mul(0.5)
				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New NormMaxBp(preReduceInput, dLdOut, dLdIn, keepDims))).expectedOutput(0, dLdInExpected))

				assertNull(err)
			Next keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormMaxAlongDimensionBP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormMaxAlongDimensionBP(ByVal backend As Nd4jBackend)
			'out = max_i (|in_i|)
			'dL/dIn = dL/dOut * dOut/dIn
			'       = dL/dOut * (0 if |x_i| is not max; or sgn(x_i) otherwise)

			For Each keepDims As Boolean In New Boolean(){False, True}

				Dim reducedShape_0() As Long = (If(keepDims, New Long(){1, 4}, New Long()){4})
				Dim preReduceInput As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
				Dim sgn As INDArray = Transforms.sign(preReduceInput, True)
				Dim max_0 As INDArray = Nd4j.create(3, 4)
				max_0.getRow(2).assign(1.0)
				Dim dLdOut_0 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, reducedShape_0)
				Dim dLdInExpected_0 As INDArray = sgn.mul(max_0).mulRowVector(dLdOut_0)

				Dim dLdIn As INDArray = Nd4j.createUninitialized(3, 4)

				Dim err As String = OpValidation.validate((New OpTestCase(New NormMaxBp(preReduceInput, dLdOut_0, dLdIn, keepDims, 0))).expectedOutput(0, dLdInExpected_0))
				assertNull(err)


				Dim reducedShape_1() As Long = (If(keepDims, New Long(){3, 1}, New Long()){3})
				Dim dLdOut_1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, reducedShape_1)
				Dim max_1 As INDArray = Nd4j.create(3, 4)
				max_1.getColumn(3).assign(1.0)
				Dim dLdInExpected_1 As INDArray = sgn.mul(max_1).mulColumnVector(dLdOut_1)
				dLdIn = Nd4j.createUninitialized(3, 4)

				err = OpValidation.validate((New OpTestCase(New NormMaxBp(preReduceInput, dLdOut_1, dLdIn, keepDims, 1))).expectedOutput(0, dLdInExpected_1))

				assertNull(err, err)
			Next keepDims
		End Sub
	End Class


End Namespace