Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffFunctionDefinition = org.nd4j.autodiff.samediff.SameDiffFunctionDefinition
Imports OpTestCase = org.nd4j.autodiff.validation.OpTestCase
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Digamma = org.nd4j.linalg.api.ops.custom.Digamma
Imports DivideNoNan = org.nd4j.linalg.api.ops.custom.DivideNoNan
Imports Flatten = org.nd4j.linalg.api.ops.custom.Flatten
Imports FusedBatchNorm = org.nd4j.linalg.api.ops.custom.FusedBatchNorm
Imports Igamma = org.nd4j.linalg.api.ops.custom.Igamma
Imports Igammac = org.nd4j.linalg.api.ops.custom.Igammac
Imports Lgamma = org.nd4j.linalg.api.ops.custom.Lgamma
Imports Lu = org.nd4j.linalg.api.ops.custom.Lu
Imports MatrixBandPart = org.nd4j.linalg.api.ops.custom.MatrixBandPart
Imports Polygamma = org.nd4j.linalg.api.ops.custom.Polygamma
Imports Roll = org.nd4j.linalg.api.ops.custom.Roll
Imports TriangularSolve = org.nd4j.linalg.api.ops.custom.TriangularSolve
Imports BiasAdd = org.nd4j.linalg.api.ops.impl.broadcast.BiasAdd
Imports BiasAddGrad = org.nd4j.linalg.api.ops.impl.broadcast.BiasAddGrad
Imports StopGradient = org.nd4j.linalg.api.ops.impl.controlflow.compat.StopGradient
Imports Mmul = org.nd4j.linalg.api.ops.impl.reduce.Mmul
Imports DiagPart = org.nd4j.linalg.api.ops.impl.shape.DiagPart
Imports OneHot = org.nd4j.linalg.api.ops.impl.shape.OneHot
Imports ZerosLike = org.nd4j.linalg.api.ops.impl.shape.ZerosLike
Imports CheckNumerics = org.nd4j.linalg.api.ops.impl.transforms.CheckNumerics
Imports ClipByNorm = org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByNorm
Imports CumProd = org.nd4j.linalg.api.ops.impl.transforms.custom.CumProd
Imports CumSum = org.nd4j.linalg.api.ops.impl.transforms.custom.CumSum
Imports Fill = org.nd4j.linalg.api.ops.impl.transforms.custom.Fill
Imports FloorDivOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorDivOp
Imports FloorModOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorModOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.junit.jupiter.api.Assertions
Imports org.junit.jupiter.api.Assumptions

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.SAMEDIFF) public class MiscOpValidation extends BaseOpValidation
	Public Class MiscOpValidation
		Inherits BaseOpValidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGradientAutoBroadcast1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGradientAutoBroadcast1(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			For Each dim_sz1 As Integer In New Integer(){0, 1, 2}

				Dim in2Shape() As Integer = {3, 4, 5}
				in2Shape(dim_sz1) = 1

				For i As Integer = 0 To 7

					Dim sd As SameDiff = SameDiff.create()

					Dim in3 As SDVariable = sd.var("in3", Nd4j.rand(New Integer(){3, 4, 5}))
					Dim in2 As SDVariable = sd.var("in2", in2Shape)

					Dim bcOp As SDVariable
					Dim name As String
					Select Case i
						Case 0
							bcOp = in3.add(in2)
							name = "add"
						Case 1
							bcOp = in3.sub(in2)
							name = "sub"
						Case 2
							bcOp = in3.mul(in2)
							name = "mul"
						Case 3
							bcOp = in3.div(in2)
							name = "div"
						Case 4
							bcOp = in3.rsub(in2)
							name = "rsub"
						Case 5
							bcOp = in3.rdiv(in2)
							name = "rdiv"
						Case 6
							'bcOp = sd.scalarFloorDiv(in3, in2);
							bcOp = (New FloorDivOp(sd, in3, in2)).outputVariable()
							name = "floordiv"
						Case 7
							'bcOp = sd.scalarFloorMod(in3, in2);
							bcOp = (New FloorModOp(sd, in3, in2)).outputVariable()
							name = "floormod"
							If OpValidationSuite.IGNORE_FAILING Then
								'https://github.com/eclipse/deeplearning4j/issues/5976
								Continue For
							End If
						Case Else
							Throw New Exception()
					End Select

					Dim outVar As SDVariable = sd.sum(bcOp)

					Dim msg As String = "(test " & i & ": " & name & ", dimension=" & dim_sz1 & ")"
					log.info("*** Starting test: " & msg)

					Dim in3Arr As INDArray = Nd4j.randn(New Integer(){3, 4, 5}).muli(100)
					Dim in2Arr As INDArray = Nd4j.randn(in2Shape).muli(100)

					sd.associateArrayWithVariable(in3Arr, in3)
					sd.associateArrayWithVariable(in2Arr, in2)

					Dim tc As New TestCase(sd)

					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(name)
					End If
				Next i
			Next dim_sz1

			assertEquals(0, failed.Count,"Failed: " & failed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGradientAutoBroadcast2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGradientAutoBroadcast2(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			For Each dim_sz1s As Integer() In New Integer()(){
				New Integer() {0, 1},
				New Integer() {0, 2},
				New Integer() {1, 2},
				New Integer() {0, 1, 2}
			}

				Dim otherShape() As Long = {3, 4, 5}
				otherShape(dim_sz1s(0)) = 1
				otherShape(dim_sz1s(1)) = 1
				If dim_sz1s.Length = 3 Then
					otherShape(dim_sz1s(2)) = 1
				End If

				For i As Integer = 0 To 7

					Dim sd As SameDiff = SameDiff.create()

					Dim in3 As SDVariable = sd.var("in3", DataType.DOUBLE, 3, 4, 5)
					Dim in2 As SDVariable = sd.var("inToBc", DataType.DOUBLE, otherShape)

					Dim name As String
					Dim bcOp As SDVariable
					Select Case i
						Case 0
							bcOp = in3.add(in2)
							name = "add"
						Case 1
							bcOp = in3.sub(in2)
							name = "sub"
						Case 2
							bcOp = in3.mul(in2)
							name = "mul"
						Case 3
							bcOp = in3.div(in2)
							name = "div"
						Case 4
							bcOp = in3.rsub(in2)
							name = "rsub"
						Case 5
							bcOp = in3.rdiv(in2)
							name = "rdiv"
						Case 6
							'bcOp = sd.scalarFloorDiv(in3, in2);
							bcOp = (New FloorDivOp(sd, in3, in2)).outputVariable()
							name = "floordiv"
						Case 7
							'bcOp = sd.scalarFloorMod(in3, in2);
							bcOp = (New FloorModOp(sd, in3, in2)).outputVariable()
							name = "floormod"
							If OpValidationSuite.IGNORE_FAILING Then
								'https://github.com/eclipse/deeplearning4j/issues/5976
								Continue For
							End If
						Case Else
							Throw New Exception()
					End Select

					Dim outVar As SDVariable = sd.sum(bcOp)

					Dim msg As String = "(test " & i & ": " & name & ", dimensions=" & java.util.Arrays.toString(dim_sz1s) & ")"
					log.info("*** Starting test: " & msg)

					Dim in3Arr As INDArray = Nd4j.randn(DataType.DOUBLE, 3, 4, 5).muli(100)
					Dim in2Arr As INDArray = Nd4j.randn(DataType.DOUBLE, otherShape).muli(100)

					sd.associateArrayWithVariable(in3Arr, in3)
					sd.associateArrayWithVariable(in2Arr, in2)

					Dim tc As New TestCase(sd)
					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(name)
					End If
				Next i
			Next dim_sz1s

			assertEquals(0, failed.Count,"Failed: " & failed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGradientAutoBroadcast3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGradientAutoBroadcast3(ByVal backend As Nd4jBackend)
			'These tests: output size > input sizes

			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			'Test cases: in1Shape, in2Shape, shapeOf(op(in1,in2))
			Dim testCases As IList(Of Triple(Of Long(), Long(), Long())) = New List(Of Triple(Of Long(), Long(), Long()))()
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1}, New Long(){1, 4}, New Long(){3, 4}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1}, New Long(){3, 4}, New Long(){3, 4}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 4}, New Long(){1, 4}, New Long(){3, 4}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 4, 1}, New Long(){1, 1, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 4, 1}, New Long(){3, 1, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 5}, New Long(){1, 4, 1}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 5}, New Long(){1, 4, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 5}, New Long(){3, 4, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 1, 1}, New Long(){1, 4, 5, 6}, New Long(){3, 4, 5, 6}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){1, 1, 1, 6}, New Long(){3, 4, 5, 6}, New Long(){3, 4, 5, 6}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){1, 4, 5, 1}, New Long(){3, 1, 1, 6}, New Long(){3, 4, 5, 6}))
			If Not OpValidationSuite.IGNORE_FAILING Then
				testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){1, 6}, New Long(){3, 4, 5, 1}, New Long(){3, 4, 5, 6}))
			End If

			For Each p As val In testCases

				For i As Integer = 0 To 7

					Dim sd As SameDiff = SameDiff.create()

					Dim in3 As SDVariable = sd.var("in1", DataType.DOUBLE, p.getFirst())
					Dim in2 As SDVariable = sd.var("in2", DataType.DOUBLE, p.getSecond())

					Dim name As String
					Dim bcOp As SDVariable
					Select Case i
						Case 0
							bcOp = in3.add(in2)
							name = "add"
						Case 1
							bcOp = in3.sub(in2)
							name = "sub"
						Case 2
							bcOp = in3.mul(in2)
							name = "mul"
						Case 3
							bcOp = in3.div(in2)
							name = "div"
						Case 4
							bcOp = in3.rsub(in2)
							name = "rsub"
						Case 5
							bcOp = in3.rdiv(in2)
							name = "rdiv"
						Case 6
							'bcOp = sd.scalarFloorDiv(in3, in2);
							bcOp = (New FloorDivOp(sd, in3, in2)).outputVariable()
							name = "floordiv"
						Case 7
							'bcOp = sd.scalarFloorMod(in3, in2);
							bcOp = (New FloorModOp(sd, in3, in2)).outputVariable()
							name = "floormod"
							If OpValidationSuite.IGNORE_FAILING Then
								'https://github.com/eclipse/deeplearning4j/issues/5976
								Continue For
							End If
						Case Else
							Throw New Exception()
					End Select

					Dim outVar As SDVariable = sd.sum(bcOp)

					Dim msg As String = "(test " & i & ": " & name & ", array 1 size =" & java.util.Arrays.toString(p.getFirst()) & ", array 2 size = " & java.util.Arrays.toString(p.getSecond()) & ")"
					log.info("*** Starting test: " & msg)

					Dim in3Arr As INDArray = Nd4j.rand(DataType.DOUBLE, p.getFirst()).muli(100)
					Dim in2Arr As INDArray = Nd4j.rand(DataType.DOUBLE, p.getSecond()).muli(100)

					sd.associateArrayWithVariable(in3Arr, in3)
					sd.associateArrayWithVariable(in2Arr, in2)

					Dim tc As New TestCase(sd)
					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(name & " " & i & " - " & [error])
					End If
				Next i
			Next p

			assertEquals(0, failed.Count,"Failed: " & failed)
		End Sub


		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterOpGradients(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterOpGradients(ByVal backend As Nd4jBackend)
			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To 6
				Nd4j.Random.setSeed(12345)

				Dim sd As SameDiff = SameDiff.create()

				Dim [in] As SDVariable = sd.var("in", DataType.DOUBLE, 20, 10)
				Dim indices As SDVariable = sd.var("indices", DataType.INT, New Long(){5})
				Dim updates As SDVariable = sd.var("updates", DataType.DOUBLE, 5, 10)


				[in].Array = Nd4j.rand(DataType.DOUBLE, 20, 10)
				indices.Array = Nd4j.create(New Double(){3, 4, 5, 10, 18}).castTo(DataType.INT)
				updates.Array = Nd4j.rand(DataType.DOUBLE, 5, 10).muli(2).subi(1)

				Dim scatter As SDVariable
				Dim name As String
				Select Case i
					Case 0
						scatter = sd.scatterAdd("s", [in], indices, updates)
						name = "scatterAdd"
					Case 1
						scatter = sd.scatterSub("s", [in], indices, updates)
						name = "scatterSub"
					Case 2
						scatter = sd.scatterMul("s", [in], indices, updates)
						name = "scatterMul"
					Case 3
						scatter = sd.scatterDiv("s", [in], indices, updates)
						name = "scatterDiv"
					Case 4
	'                    scatter = sd.scatterUpdate("s", in, indices, updates);
	'                    name = "scatterUpdate";
	'                    break;
						Continue For
					Case 5
						scatter = sd.scatterMax("s", [in], indices, updates)
						name = "scatterMax"
					Case 6
						scatter = sd.scatterMin("s", [in], indices, updates)
						name = "scatterMin"
					Case Else
						Throw New Exception()
				End Select

				Dim exp As INDArray = [in].Arr.dup()
				Dim indicesInt() As Integer = indices.Arr.dup().data().asInt()
				For j As Integer = 0 To indicesInt.Length - 1
					Dim updateRow As INDArray = updates.Arr.getRow(j)
					Dim destinationRow As INDArray = exp.getRow(indicesInt(j))
					Select Case i
						Case 0
							destinationRow.addi(updateRow)
						Case 1
							destinationRow.subi(updateRow)
						Case 2
							destinationRow.muli(updateRow)
						Case 3
							destinationRow.divi(updateRow)
						Case 4
							destinationRow.assign(updateRow)
						Case 5
							destinationRow.assign(Transforms.max(destinationRow, updateRow, True))
						Case 6
							destinationRow.assign(Transforms.min(destinationRow, updateRow, True))
						Case Else
							Throw New Exception()
					End Select
				Next j

				Dim loss As SDVariable = sd.sum(scatter) '.standardDeviation(scatter, true);  //.sum(scatter);  //TODO stdev might be better here as gradients are non-symmetrical...


				Dim tc As TestCase = (New TestCase(sd)).expected(scatter, exp).gradCheckSkipVariables(indices.name())

				Dim [error] As String = OpValidation.validate(tc)
				If [error] IsNot Nothing Then
					failed.Add(name)
				End If
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterUpdate()
		Public Overridable Sub testScatterUpdate()
			Dim x As INDArray = Nd4j.linspace(DataType.FLOAT, 1, 30, 1).reshape(ChrW(10), 3)
			Dim updates As INDArray = Nd4j.create(New Single()(){
				New Single() {100, 101, 102},
				New Single() {200, 201, 202}
			})
			Dim indices As INDArray = Nd4j.createFromArray(2, 5)

			Dim exp As INDArray = x.dup()
			exp.putRow(2, updates.getRow(0))
			exp.putRow(5, updates.getRow(1))

			Dim [out] As INDArray = exp.ulike()
			Nd4j.exec(DynamicCustomOp.builder("scatter_upd").addInputs(x, indices, updates).addOutputs([out]).build())

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testGatherGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherGradient(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			For rank As Integer = 2 To 3
				For [dim] As Integer = 0 To rank - 1
					Dim sd As SameDiff = SameDiff.create()

					Dim inShape() As Integer
					If rank = 2 Then
						inShape = New Integer(){10, 10}
					Else
						inShape = New Integer(){10, 10, 10}
					End If

					Dim [in] As SDVariable = sd.var("in", Nd4j.rand(DataType.DOUBLE, inShape))
					Dim indices As SDVariable = sd.constant("indices", Nd4j.createFromArray(0, 3, 7))

					Dim gatherExp As INDArray = Nothing
					If rank = 2 Then
						Dim tadDim As Integer = If([dim] = 0, 1, 0) 'Swap: pullRows dim is "tensor along dimension" vs. gather's "index is value for this dimension"
						gatherExp = Nd4j.pullRows([in].Arr, tadDim, New Integer(){0, 3, 7})
					End If

					Dim gather As SDVariable = sd.gather([in], indices, [dim])

					Dim loss As SDVariable = sd.standardDeviation("loss", gather, True, Integer.MaxValue)

					Dim msg As String = "rank=" & rank & " dim=" & [dim]

					Dim tc As TestCase = (New TestCase(sd)).testName(msg).gradCheckSkipVariables(indices.name())

					If gatherExp IsNot Nothing Then
						tc.expected(gather, gatherExp)
					End If

					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(msg & " - " & [error])
					End If
				Next [dim]
			Next rank

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrace()
		Public Overridable Sub testTrace()
			'TODO need to work out how to handle shape_op for scalars...
			'OpValidationSuite.ignoreFailing();
			Nd4j.Random.setSeed(12345)
			For Each inShape As Integer() In New Integer()(){
				New Integer() {3, 3}
			}

				Dim [in] As INDArray = Nd4j.rand(inShape)
				Dim sd As SameDiff = SameDiff.create()
				Dim i As SDVariable = sd.var("in", [in])
				Dim trace As SDVariable = sd.math().trace(i)

				Dim exp As Double = Nd4j.diag([in]).sumNumber().doubleValue()

				Dim tc As TestCase = (New TestCase(sd)).expected(trace, Nd4j.scalar(exp)).testName(java.util.Arrays.toString(inShape))

				Dim err As String = OpValidation.validate(tc)

				assertNull(err)
			Next inShape
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorGradTensorMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorGradTensorMmul(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing()

			Nd4j.Random.setSeed(12345)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.rand(New Long(){2, 2, 2})
			Dim arr2 As INDArray = Nd4j.rand(New Long(){2, 2, 2})
			Dim x As SDVariable = sameDiff.var("x", arr)
			Dim y As SDVariable = sameDiff.var("y", arr2)
			Dim result As SDVariable = sameDiff.tensorMmul(x, y, New Integer(){0}, New Integer(){1})
			assertArrayEquals(ArrayUtil.getTensorMmulShape(New Long(){2, 2, 2},
			New Long(){2, 2, 2},
			New Integer()(){
				New Integer() {0},
				New Integer() {1}
			}), result.eval().shape())
			assertEquals(16, sameDiff.numElements())

			Dim loss As SDVariable = sameDiff.standardDeviation(result, True)
			sameDiff.addLossVariable(loss)

			Dim err As String = OpValidation.validate(New TestCase(sameDiff))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMulGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMulGradient(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim arr2 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)

			Dim gradAssertion As INDArray = Nd4j.ones(arr1.shape())
			Dim scalar As INDArray = Nd4j.scalar(1.0)
			Dim aGradAssertion As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 4},
				New Double() {9, 16}
			})

			Dim cGradAssertion As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 4}
			})

			Dim wGradAssertion As INDArray = Nd4j.create(New Double()(){
				New Double() {2, 8},
				New Double() {18, 32}
			})

			Dim dGradAssertion As INDArray = Nd4j.ones(2, 2)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim sdVariable As SDVariable = sameDiff.var("a", arr1)
			Dim sdVariable1 As SDVariable = sameDiff.var("w", arr2)
			Dim varMulPre As SDVariable = sdVariable.mul("c", sdVariable1)
			Dim varMul As SDVariable = varMulPre.mul("d", sdVariable1)
			Dim sum As SDVariable = sameDiff.sum("ret", varMul, Integer.MaxValue)

			Dim m As IDictionary(Of String, INDArray) = sameDiff.outputAll(Nothing)
			Dim gm As IDictionary(Of String, INDArray) = sameDiff.calculateGradients(Nothing, m.Keys)

			Dim finalResult As SDVariable = sameDiff.grad(sum.name())

			Dim cGrad As SDVariable = sameDiff.grad(varMulPre.name())

			Dim mulGradResult As SDVariable = sameDiff.grad(varMul.name())
			Dim aGrad As SDVariable = sameDiff.grad(sdVariable.name())
			Dim wGrad As SDVariable = sameDiff.grad(sdVariable1.name())
			Dim dGrad As SDVariable = sameDiff.grad(varMul.name())

			Dim scalarGradTest As INDArray = gm(sum.name())
			assertEquals(scalar, scalarGradTest)


			Dim gradTest As INDArray = mulGradResult.Arr
			assertEquals(gradAssertion, gradTest)

			Dim aGradTest As INDArray = aGrad.Arr
			assertEquals(aGradAssertion, aGradTest)

			Dim cGradTest As INDArray = cGrad.Arr
			assertEquals(cGradAssertion, cGradTest)

			Dim wGradTest As INDArray = wGrad.Arr
			assertEquals(wGradAssertion, wGradTest)

			Dim dGradTest As INDArray = dGrad.Arr
			assertEquals(dGradAssertion, dGradTest)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulGradients()
		Public Overridable Sub testMmulGradients()
			Dim aShape() As Integer = {2, 3}
			Dim bShape() As Integer = {3, 4}
			Dim failed As IList(Of String) = New List(Of String)()

			For Each aOrder As Char In New Char(){"c"c, "f"c}
				For Each bOrder As Char In New Char(){"c"c, "f"c}
					For Each transposeA As Boolean In New Boolean(){False, True}
						For Each transposeB As Boolean In New Boolean(){False, True}
							For Each transposeResult As Boolean In New Boolean(){False, True} 'https://github.com/eclipse/deeplearning4j/issues/5648
								Nd4j.Random.setSeed(12345)

								Dim aArr As INDArray = Nd4j.rand(DataType.DOUBLE, t(transposeA, aShape)).dup(aOrder)
								Dim bArr As INDArray = Nd4j.rand(DataType.DOUBLE, t(transposeB, bShape)).dup(bOrder)

								Dim sd As SameDiff = SameDiff.create()
								Dim a As SDVariable = sd.var("a", aArr)
								Dim b As SDVariable = sd.var("b", bArr)

								Dim mmul As SDVariable = sd.mmul(a, b, transposeA, transposeB, transposeResult)

								Dim exp As INDArray = (If(transposeA, aArr.transpose(), aArr))
								exp = exp.mmul(If(transposeB, bArr.transpose(), bArr))
								exp = (If(transposeResult, exp.transpose(), exp))

								Dim loss As SDVariable = mmul.std(True)

								Dim name As String = aOrder & "," & bOrder & ",tA=" & transposeA & ",tB=" & transposeB & ",tRes=" & transposeResult
								Dim tc As TestCase = (New TestCase(sd)).testName(name).expected(mmul, exp)

								Dim err As String = OpValidation.validate(tc, True)
								If err IsNot Nothing Then
									failed.Add(err)
								End If
							Next transposeResult
						Next transposeB
					Next transposeA
				Next bOrder
			Next aOrder

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

		Private Shared Function t(ByVal transpose As Boolean, ByVal orig() As Integer) As Integer()
			If Not transpose Then
				Return orig
			End If
			Return New Integer(){orig(1), orig(0)}
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBatchMmulBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchMmulBasic(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'https://github.com/eclipse/deeplearning4j/issues/6873
			Dim M As Integer = 5
			Dim N As Integer = 3
			Dim K As Integer = 4

			Dim A As INDArray = Nd4j.create(New Single(){1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}).reshape(ChrW(M), N).castTo(DataType.DOUBLE)
			Dim B As INDArray = Nd4j.create(New Single(){1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}).reshape(ChrW(N), K).castTo(DataType.DOUBLE)

			Dim sd As SameDiff = SameDiff.create()

			Dim A1 As SDVariable = sd.var("A1", A)
			Dim A2 As SDVariable = sd.var("A2", A)
			Dim B1 As SDVariable = sd.var("B1", B)
			Dim B2 As SDVariable = sd.var("B2", B)

			Dim batchMul() As SDVariable = sd.batchMmul(New SDVariable() {A1, A2}, New SDVariable() {B1, B2})
'JAVA TO VB CONVERTER NOTE: The variable m was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim m_Conflict As IDictionary(Of String, INDArray) = sd.output(java.util.Collections.emptyMap(), sd.outputs())

			Dim resultingMatrix As INDArray = m(batchMul(0).name())
			'System.out.print(resultingMatrix);
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulWithTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulWithTranspose(ByVal backend As Nd4jBackend)

			'Here: [x,3]^T * [x,4] = [3,4]

			For Each i As Integer In New Integer(){2, 1}
				Console.WriteLine("i = " & i)
				Dim first As INDArray = Nd4j.linspace(1, 3 * i, 3 * i, DataType.DOUBLE).reshape("c"c, i, 3) 'To [1,3] or [2,3]
				Dim second As INDArray = Nd4j.linspace(4, 4 + 4 * i, 4 * i, DataType.DOUBLE).reshape("c"c, i, 4) 'To [1,4] or [2,4]

				Console.WriteLine("Shapes: " & java.util.Arrays.toString(first.shape()) & vbTab & java.util.Arrays.toString(second.shape()))

				Dim sd As SameDiff = SameDiff.create()
				Dim f As SDVariable = sd.var("in1", first)
				Dim s As SDVariable = sd.var("in2", second)

				Dim mt As MMulTranspose = MMulTranspose.builder().transposeA(True).transposeB(False).transposeResult(False).build()
				Dim mmul As SDVariable = sd.mmul(f, s, True, False, False)
				sd.updateVariableNameAndReference(mmul, "mmul")

				Dim [out] As INDArray = mmul.eval()

				Dim exp As INDArray = first.transpose().mmul(second)
				assertEquals(exp, [out])

				Dim loss As SDVariable = sd.standardDeviation(mmul, True)
				Dim err As String = OpValidation.validate((New TestCase(sd)).expected(mmul.name(), exp))

				assertNull(err)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulOutputSizeCalculation()
		Public Overridable Sub testMmulOutputSizeCalculation()
			'[3,2] x [2,4] with result transpose: output shape [4,3]
			Dim a As INDArray = Nd4j.create(3,2)
			Dim b As INDArray = Nd4j.create(2,4)
			Dim z As INDArray = Nd4j.create(4,3)
			Dim m As New Mmul(a,b,z,MMulTranspose.builder().transposeA(False).transposeB(False).transposeResult(True).build())

			Dim outShapes As val = Nd4j.Executioner.calculateOutputShape(m)
			assertArrayEquals(New Long(){4, 3}, outShapes.get(0).getShape())
			Nd4j.Executioner.exec(m)

			'Another case: ([3,4]*[2,4]T)T = [2,3]     -   tA=false, tB=true, tR=true
			a = Nd4j.create(3,4)
			b = Nd4j.create(2,4)
			z = Nd4j.create(2,3)
			m = New Mmul(a,b,z,MMulTranspose.builder().transposeA(False).transposeB(True).transposeResult(True).build())

			Dim outShapes2 As val = Nd4j.Executioner.calculateOutputShape(m)
			assertArrayEquals(New Long(){2, 3}, outShapes2.get(0).getShape())
			Nd4j.Executioner.exec(m)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFillOp()
		Public Overridable Sub testFillOp()

			Dim ia As INDArray = Nd4j.createFromArray(New Double(){2, 2}).castTo(DataType.INT)
			Dim value As Double = 42
			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 2,2)
			Dim op As New OpTestCase(New Fill(ia, [out], value))
			Dim expOut As INDArray = Nd4j.valueArrayOf(New Long(){2, 2}, 42.0f)

			op.expectedOutput(0, expOut)
			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClipByNorm()
		Public Overridable Sub testClipByNorm()
			'Expected: if array.norm2(1) is less than 1.0, not modified
			'Otherwise: array.tad(x,1) = array.tad(x,1) * 1.0 / array.tad(x,1).norm2()

			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.rand(3,5)
			Dim norm2_1 As INDArray = arr.norm2(1)
			arr.diviColumnVector(norm2_1)

			norm2_1 = arr.norm2(1)
			assertEquals(Nd4j.ones(3), norm2_1)

			Dim scale As INDArray = Nd4j.create(New Double(){1.1, 1.0, 0.9}, New Integer(){3})
			arr.muliColumnVector(scale)
			norm2_1 = arr.norm2(1)

			Dim [out] As INDArray = Nd4j.create(arr.shape())

			Nd4j.Executioner.exec(DynamicCustomOp.builder("clipbynorm").addInputs(arr).addOutputs([out]).addIntegerArguments(1).addFloatingPointArguments(1.0).build())

			Dim norm2_1b As INDArray = [out].norm2(1)
			Dim exp As INDArray = Nd4j.create(New Double(){1.0, 1.0, norm2_1.getDouble(2)}, New Integer(){3})

			assertEquals(exp, norm2_1b)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClipByNorm2()
		Public Overridable Sub testClipByNorm2()
			'Expected: if array.norm2(1) is less than 1.0, not modified
			'Otherwise: array.tad(x,1) = array.tad(x,1) * 1.0 / array.tad(x,1).norm2()

			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.rand(3,5)
			Dim norm2_1 As INDArray = arr.norm2(1)
			arr.diviColumnVector(norm2_1)

			norm2_1 = arr.norm2(1)
			assertEquals(Nd4j.ones(3), norm2_1)

			Dim scale As INDArray = Nd4j.create(New Double(){1.1, 1.0, 0.9}, New Integer(){3, 1})
			arr.muliColumnVector(scale)
			norm2_1 = arr.norm2(1)

			Dim [out] As INDArray = Nd4j.createUninitialized(arr.shape())

			Dim op As New OpTestCase(DynamicCustomOp.builder("clipbynorm").addInputs(arr).addOutputs([out]).addIntegerArguments(1).addFloatingPointArguments(1.0).build())

			Dim expNorm2 As INDArray = Nd4j.create(New Double(){1.0, 1.0, norm2_1.getDouble(2)}, New Integer(){3, 1})

			Dim expOut As INDArray = arr.divColumnVector(norm2_1).muliColumnVector(expNorm2)
			op.expectedOutput(0, expOut)

			Console.WriteLine("Input")
			Console.WriteLine(arr.shapeInfoToString())
			Console.WriteLine(java.util.Arrays.toString(arr.data().asFloat()))

			Console.WriteLine("Expected")
			Console.WriteLine(expOut.shapeInfoToString())
			Console.WriteLine(java.util.Arrays.toString(expOut.data().asFloat()))

			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClipByNorm1()
		Public Overridable Sub testClipByNorm1()
			'Expected: if array.norm2(1) is less than 1.0, not modified
			'Otherwise: array.tad(x,1) = array.tad(x,1) * 1.0 / array.tad(x,1).norm2()

			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.rand(3,5)
			Dim norm2_1 As INDArray = arr.norm2(1)
			arr.diviColumnVector(norm2_1)

			norm2_1 = arr.norm2(1)
			assertEquals(Nd4j.ones(3), norm2_1)

			Dim scale As INDArray = Nd4j.create(New Double(){1.1, 1.0, 0.9}, New Integer(){3, 1})
			arr.muliColumnVector(scale)
			norm2_1 = arr.norm2(1)

			Dim [out] As INDArray = Nd4j.createUninitialized(arr.shape())

			Dim expNorm2 As INDArray = Nd4j.create(New Double(){1.0, 1.0, norm2_1.getDouble(2)}, New Integer(){3, 1})

			Dim expOut As INDArray = arr.divColumnVector(norm2_1).muliColumnVector(expNorm2)


			Dim op As OpTestCase = (New OpTestCase(New ClipByNorm(arr, [out], 1.0, 1))).expectedOutput(0, expOut)

	'        System.out.println("Input");
	'        System.out.println(arr.shapeInfoToString());
	'        System.out.println(Arrays.toString(arr.data().asFloat()));
	'
	'        System.out.println("Expected");
	'        System.out.println(expOut.shapeInfoToString());
	'        System.out.println(Arrays.toString(expOut.data().asFloat()));

			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClipByNorm0()
		Public Overridable Sub testClipByNorm0()
			'Expected: if array.norm2(0) is less than 1.0, not modified
			'Otherwise: array.tad(x,1) = array.tad(x,1) * 1.0 / array.tad(x,1).norm2()

			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.rand(5,4)
			Dim norm2_0 As INDArray = arr.norm2(0)
			arr.diviRowVector(norm2_0)

			Dim initNorm2 As INDArray = Nd4j.create(New Double(){2.2, 2.1, 2.0, 1.9}, New Integer(){4}) 'Initial norm2s along dimension 0
			arr.muliRowVector(initNorm2)
			norm2_0 = arr.norm2(0)

			assertEquals(initNorm2, norm2_0)

			Dim [out] As INDArray = Nd4j.create(arr.shape())

			Dim norm2_0b As INDArray = [out].norm2(0)
			Dim expNorm As INDArray = Nd4j.create(New Double(){2.0, 2.0, 2.0, 1.9}, New Integer(){1, 4}) 'Post clip norm2s along dimension 0
			Dim exp As INDArray = arr.divRowVector(norm2_0b).muliRowVector(expNorm)

			Dim op As OpTestCase = (New OpTestCase(New ClipByNorm(arr, [out], 2.0, 0))).expectedOutput(0, exp)

			assertNull(OpValidation.validate(op))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCumSum()
		Public Overridable Sub testCumSum()

			Dim failing As IList(Of String) = New List(Of String)()
			For Each order As Char In New Char(){"c"c, "f"c}

				Nd4j.Random.setSeed(12345)
				Dim arr As INDArray = Nd4j.linspace(1, 15, 15, DataType.DOUBLE).reshape(ChrW(3), 5).dup(order)
	'            System.out.println(arr);

				Dim expFF As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 3, 6, 10, 15},
					New Double() {6, 13, 21, 30, 40},
					New Double() {11, 23, 36, 50, 65}
				})

				Dim expTF As INDArray = Nd4j.create(New Double()(){
					New Double() {0, 1, 3, 6, 10},
					New Double() {0, 6, 13, 21, 30},
					New Double() {0, 11, 23, 36, 50}
				})

				Dim expFT As INDArray = Nd4j.create(New Double()(){
					New Double() {15, 14, 12, 9, 5},
					New Double() {40, 34, 27, 19, 10},
					New Double() {65, 54, 42, 29, 15}
				})

				Dim expTT As INDArray = Nd4j.create(New Double()(){
					New Double() {14, 12, 9, 5, 0},
					New Double() {34, 27, 19, 10, 0},
					New Double() {54, 42, 29, 15, 0}
				})

				For Each exclusive As Boolean In New Boolean(){False, True}
					For Each reverse As Boolean In New Boolean(){False, True}

						Dim msg As String = order & ", exclusive=" & exclusive & ", reverse=" & reverse

						Dim [out] As INDArray = Nd4j.create(3, 5)
						Dim op As New OpTestCase(New CumSum(arr, [out], exclusive, reverse, 1))

						If Not exclusive AndAlso Not reverse Then
							op.expectedOutput(0, expFF)
						ElseIf exclusive AndAlso Not reverse Then
							op.expectedOutput(0, expTF)
						ElseIf Not exclusive AndAlso reverse Then
							op.expectedOutput(0, expFT)
						Else
							op.expectedOutput(0, expTT)
						End If

						Dim err As String = OpValidation.validate(op)
						If err IsNot Nothing Then
	'                        System.out.println(err);
							failing.Add(msg & " (" & err & ")")
						End If
					Next reverse
				Next exclusive
			Next order

			assertEquals(0, failing.Count,failing.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCumProd()
		Public Overridable Sub testCumProd()
			Dim failing As IList(Of String) = New List(Of String)()

			For Each order As Char In New Char(){"c"c, "f"c}

				Nd4j.Random.setSeed(12345)
	'            INDArray arr = Nd4j.linspace(1, 15, 15, DataType.DOUBLE).reshape('c',3, 5).dup(order);

				Dim arr As INDArray = Nd4j.create(New Double()(){
					New Double() { 1, 2, 3, 4, 5},
					New Double() { 6, 7, 8, 9, 10},
					New Double() {11, 12, 13, 14, 15}
				})

				Dim expFF As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 2, 6, 24, 120},
					New Double() {6, 42, 336, 3024, 30240},
					New Double() {11, 132, 1716, 24024, 360360}
				})

				Dim expTF As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 1, 2, 6, 24},
					New Double() {1, 6, 42, 336, 3024},
					New Double() {1, 11, 132, 1716, 24024}
				})

				Dim expFT As INDArray = Nd4j.create(New Double()(){
					New Double() {120, 120, 60, 20, 5},
					New Double() {30240, 5040, 720, 90, 10},
					New Double() {360360, 32760, 2730, 210, 15}
				})

				Dim expTT As INDArray = Nd4j.create(New Double()(){
					New Double() {120, 60, 20, 5, 1},
					New Double() {5040, 720, 90, 10, 1},
					New Double() {32760, 2730, 210, 15, 1}
				})

				Dim axisArg As INDArray = Nd4j.scalar(1) 'Along dim 1

				For Each exclusive As Boolean In New Boolean(){False, True}
					For Each reverse As Boolean In New Boolean(){False, True}

						Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 3, 5)
						Dim op As New OpTestCase(New CumProd(arr, [out], exclusive, reverse, 1))
						Dim msg As String = order & ", exclusive=" & exclusive & ", reverse=" & reverse

						If Not exclusive AndAlso Not reverse Then
							op.expectedOutput(0, expFF)
						ElseIf exclusive AndAlso Not reverse Then
							op.expectedOutput(0, expTF)
						ElseIf Not exclusive AndAlso reverse Then
							op.expectedOutput(0, expFT)
						Else
							op.expectedOutput(0, expTT)
						End If

						Dim err As String = OpValidation.validate(op)
						If err IsNot Nothing Then
							failing.Add(msg & " - " & err)
						End If
					Next reverse
				Next exclusive
			Next order

			assertEquals(0, failing.Count,failing.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneHot1()
		Public Overridable Sub testOneHot1()
			Dim failed As IList(Of String) = New List(Of String)()

			'Because it's on the diagonal, should be the same for all axis args...
			For i As Integer = -1 To 0
				Dim indicesArr As INDArray = Nd4j.createFromArray(0, 1, 2)
				Dim depth As Integer = 3

				Dim sd As SameDiff = SameDiff.create()
				Dim indices As SDVariable = sd.constant(indicesArr)
				Dim oneHot As SDVariable = sd.oneHot(indices, depth, i, 1.0, 0.0, DataType.DOUBLE)

				Dim exp As INDArray = Nd4j.eye(3).castTo(DataType.DOUBLE)

				Dim msg As String = "Axis: " & i
				log.info("Test case: " & msg)

				Dim err As String = OpValidation.validate((New TestCase(sd)).testName(msg).gradientCheck(False).expected(oneHot, exp))

				If err IsNot Nothing Then
					failed.Add(err)
				End If
			Next i
			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneHotOp()
		Public Overridable Sub testOneHotOp()
			'https://www.tensorflow.org/api_docs/python/tf/one_hot
			'https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/ops/declarable/generic/parity_ops/onehot.cpp

			For axis As Integer = -1 To 0
				Dim err As String = OpValidation.validate((New OpTestCase(New OneHot(Nd4j.create(New Double(){0, 1, 2}), Nd4j.create(DataType.FLOAT,3,3), 3, axis, 1.0, 0.0))).expectedOutput(0, Nd4j.eye(3).castTo(DataType.FLOAT)))

				assertNull(err)
			Next axis
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneHot2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOneHot2(ByVal backend As Nd4jBackend)

			Dim indicesArr As INDArray = Nd4j.createFromArray(0, 2, -1, 1)

			Dim sd As SameDiff = SameDiff.create()
			Dim indices As SDVariable = sd.constant("indices", indicesArr)
			Dim depth As Integer = 3
			Dim axis As Integer = -1
			Dim oneHot As SDVariable = sd.oneHot("oneHot", indices, depth, axis, 5.0, 0.0, DataType.DOUBLE)

			Dim exp As INDArray = Nd4j.create(New Double()(){
				New Double() {5, 0, 0},
				New Double() {0, 0, 5},
				New Double() {0, 0, 0},
				New Double() {0, 5, 0}
			})

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(oneHot, exp).gradientCheck(False))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneHot4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOneHot4(ByVal backend As Nd4jBackend)

			Dim indicesArr As INDArray = Nd4j.createFromArray(0, 2, -1, 1)

			Dim sd As SameDiff = SameDiff.create()
			Dim indices As SDVariable = sd.constant("indices", indicesArr)
			Dim depth As Integer = 3
			Dim axis As Integer = -1
			Dim oneHot As SDVariable = sd.oneHot("oneHot", indices, depth, axis, 5.0, 0.0, DataType.INT32)

			Dim exp As INDArray = Nd4j.create(New Integer()(){
				New Integer() {5, 0, 0},
				New Integer() {0, 0, 5},
				New Integer() {0, 0, 0},
				New Integer() {0, 5, 0}
			})

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(oneHot, exp).gradientCheck(False))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneHot3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOneHot3(ByVal backend As Nd4jBackend)
			'https://github.com/eclipse/deeplearning4j/issues/6872

			'https://www.tensorflow.org/api_docs/python/tf/one_hot
			'indices = [[0, 2], [1, -1]]
			Dim indicesArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 2},
				New Double() {1, -1}
			}).castTo(DataType.INT)
			Dim expectedOut As INDArray = Nd4j.zeros(DataType.DOUBLE, 2, 2, 3)
	'        
	'        # output: [2 x 2 x 3]
	'        # [[[1.0, 0.0, 0.0],   # one_hot(0)
	'        #   [0.0, 0.0, 1.0]],  # one_hot(2)
	'        #  [[0.0, 1.0, 0.0],   # one_hot(1)
	'        #   [0.0, 0.0, 0.0]]]  # one_hot(-1)
	'        
			expectedOut.putScalar(0, 0, 0, 1.0)
			expectedOut.putScalar(0, 1, 2, 1.0)
			expectedOut.putScalar(1, 0, 1, 1.0)

			Dim sd As SameDiff = SameDiff.create()
			Dim indices As SDVariable = sd.constant("indices", indicesArr)

			Dim depth As Integer = 3
			Dim axis As Integer = -1
			Dim oneHot As SDVariable = sd.oneHot("oneHot", indices, depth, axis, 1.0, 0.0).castTo(DataType.DOUBLE)

			Dim loss As SDVariable = oneHot.std(True)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(oneHot, expectedOut).gradientCheck(False))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspace()
		Public Overridable Sub testLinspace()
			Dim sd As SameDiff = SameDiff.create()
			Dim [out] As SDVariable = sd.linspace("linspace", DataType.DOUBLE, 1,10,10)
			Dim loss As SDVariable = [out].std(True)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected([out], Nd4j.linspace(1,10,10, DataType.DOUBLE)).gradientCheck(False))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspace2()
		Public Overridable Sub testLinspace2()
			OpValidationSuite.ignoreFailing() 'TODO 2019/01/18
			Dim sd As SameDiff = SameDiff.create()
			Dim [out] As SDVariable = sd.linspace("linspace", sd.constant(Nd4j.scalar(1)), sd.constant(Nd4j.scalar(10)), sd.constant(Nd4j.scalar(10)), DataType.DOUBLE)
			Dim loss As SDVariable = [out].std(True)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected([out], Nd4j.linspace(1,10,10, DataType.DOUBLE)))

			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeFn(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeFn(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.create(New Long(){1, 2})

			Dim shapes As val = Nd4j.Executioner.calculateOutputShape(DynamicCustomOp.builder("shape").addInputs([in]).build())

			assertEquals(1, shapes.size())

			assertArrayEquals(New Long(){2}, shapes.get(0).getShape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeFn2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeFn2(ByVal backend As Nd4jBackend)

			Dim i As INDArray = Nd4j.create(1,3)

			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", i)
			Dim shape As SDVariable = sd.shape(var)
			Dim sum As SDVariable = shape.castTo(DataType.DOUBLE).sum()
			sum.eval()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeRank1()
		Public Overridable Sub testMergeRank1()
			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", Nd4j.create(New Long(){1}).assign(5))

			Dim merged As SDVariable = sd.math().mergeAvg("merged", New SDVariable(){var})
			Dim sum As SDVariable = sd.sum(merged)

			Dim m As IDictionary(Of String, INDArray) = sd.output(java.util.Collections.emptyMap(), "merged")
			Dim gm As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, "in")

			Dim [out] As INDArray = m("merged")
			assertEquals(1, [out].rank())

			Dim inGrad As INDArray = gm("in")
			assertEquals(1, inGrad.rank())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiagPart(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDiagPart(ByVal backend As Nd4jBackend)
			Dim i As INDArray = Nd4j.create(5,5)

			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", i)
			Dim diag As SDVariable = sd.math().diagPart(var)

			Dim [out] As INDArray = diag.eval()
			assertEquals(1, [out].rank())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiagShapeFn(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDiagShapeFn(ByVal backend As Nd4jBackend)
			Dim i As INDArray = Nd4j.create(5,5)

			Dim op As CustomOp = New DiagPart(i, Nothing)

			Dim outShape As val = Nd4j.Executioner.calculateOutputShape(op)

			assertEquals(1, outShape.size())
			assertArrayEquals(New Long(){5}, outShape.get(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZerosOnesLike()
		Public Overridable Sub testZerosOnesLike()
			Nd4j.Random.setSeed(12345)

			Dim shapes As IList(Of Integer()) = New List(Of Integer()) From {
				New Integer(){},
				New Integer(){3},
				New Integer(){3, 4},
				New Integer(){3, 4, 5}
			}
			Dim failed As IList(Of String) = New List(Of String)()

			For Each zeros As Boolean In New Boolean(){True, False}
				For Each shape As Integer() In shapes
					Dim sd As SameDiff = SameDiff.create()
					Dim arr As INDArray
					If shape.Length > 0 Then
						arr = Nd4j.rand(shape)
					Else
						arr = Nd4j.scalar(Nd4j.rand(New Integer(){1, 1}).getDouble(0))
					End If
					Dim var As SDVariable = sd.var("in", arr)
					Dim xLike As SDVariable
					If zeros Then
						xLike = sd.zerosLike(var)
					Else
						xLike = sd.onesLike(var)
					End If

					Dim loss As SDVariable
					If shape.Length > 0 Then
						loss = xLike.std(True)
					Else
						loss = xLike.mean()
					End If

					Dim err As String = OpValidation.validate((New TestCase(sd)).expected(xLike, (If(zeros, Nd4j.zeros(shape), Nd4j.ones(shape)))), True)
					If err IsNot Nothing Then
						failed.Add(err)
					End If
				Next shape
			Next zeros

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZerosLikeOp()
		Public Overridable Sub testZerosLikeOp()

			Dim arr As INDArray = Nd4j.scalar(DataType.DOUBLE, 1.0)
			Dim [out] As INDArray = Nd4j.scalar(DataType.DOUBLE, -1)
			Dim exp As INDArray = Nd4j.scalar(DataType.DOUBLE, 0)

			Dim op As New OpTestCase(New ZerosLike(arr, [out]))
			op.expectedOutput(0, exp)

			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConfusionMatrix()
		Public Overridable Sub testConfusionMatrix()
			Dim dt As DataType = DataType.DOUBLE

			For Each withMax As Boolean In New Boolean(){True, False}

				Dim sd As SameDiff = SameDiff.create()

				Dim labels As SDVariable = sd.constant("labels", Nd4j.createFromArray(1, 2, 4))
				Dim predictions As SDVariable = sd.constant("predictions", Nd4j.createFromArray(2, 2, 4))

				Dim exp As INDArray = Nd4j.create(New Double()(){
					New Double() {0, 0, 0, 0, 0},
					New Double() {0, 0, 1, 0, 0},
					New Double() {0, 0, 1, 0, 0},
					New Double() {0, 0, 0, 0, 0},
					New Double() {0, 0, 0, 0, 1}
				}).castTo(DataType.FLOAT)

				Dim confMatrix As SDVariable
				If withMax Then
					confMatrix = sd.math().confusionMatrix(labels, predictions, 5).castTo(DataType.FLOAT)
				Else
					confMatrix = sd.math().confusionMatrix("cm", labels, predictions, DataType.FLOAT)
				End If

				Dim loss As SDVariable = confMatrix.castTo(DataType.DOUBLE).std(True)


				Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(False).expected(confMatrix, exp))

				assertNull(err)
			Next withMax
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsNonDecreasingIsStrictlyIncr()
		Public Overridable Sub testIsNonDecreasingIsStrictlyIncr()
			Dim shapes As IList(Of Long()) = New List(Of Long()) From {
				Nothing, New Long(){12},
				New Long(){1, 12},
				New Long(){3, 4},
				New Long(){2, 2, 3}
			}

			Dim failed As IList(Of String) = New List(Of String)()

			For Each nonDec As Boolean In New Boolean(){True, False}
				For Each shape As Long() In shapes
					For Each expTrue As Boolean In New Boolean(){True, False}
						Dim sd As SameDiff = SameDiff.create()

						Dim inArr As INDArray
						If shape Is Nothing Then
							inArr = Nd4j.scalar(1.0)
						Else
							inArr = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(shape)
						End If

						If nonDec AndAlso Not expTrue Then
							inArr.negi()
						End If
						If Not nonDec AndAlso Not expTrue AndAlso inArr.length() > 0 Then
							inArr.putScalar(inArr.length()-1, inArr.getDouble(inArr.length()-2))
						End If

						Dim [in] As SDVariable = sd.var("in", inArr)
						Dim [out] As SDVariable
						If nonDec Then
							[out] = sd.math().isNonDecreasing([in]).castTo(DataType.DOUBLE)
						Else
							[out] = sd.math().isStrictlyIncreasing([in]).castTo(DataType.DOUBLE)
						End If

						If shape Is Nothing Then
							Dim loss As SDVariable = [out].mean()
						Else
							Dim loss As SDVariable = [out].std(True)
						End If

						Dim exp As INDArray
						If expTrue OrElse shape Is Nothing Then
							exp = Nd4j.scalar(1.0)
						Else
							exp = Nd4j.scalar(0.0)
						End If

						Dim msg As String = (If(nonDec, "isNonDecreasing", "isStrictlyIncreasing")) & " - " & (If(shape Is Nothing, "[]", java.util.Arrays.toString(shape))) & " - expected=" & exp
						Dim tc As TestCase = (New TestCase(sd)).testName(msg).expected([out], exp).gradientCheck(False)

						Dim err As String = OpValidation.validate(tc, True)
						If err IsNot Nothing Then
							failed.Add(err)
						End If
					Next expTrue
				Next shape
			Next nonDec

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExtractImagePatches()
		Public Overridable Sub testExtractImagePatches()
	'        
	'        tf.reset_default_graph()
	'        input = tf.reshape(tf.constant([1,2,3,4,5,6,7,8,9], dtype=tf.float32), [1,3,3,1])
	'        patches = tf.image.extract_image_patches(images=input, ksizes=[1,2,2,1], strides=[1,1,1,1], rates=[1,1,1,1], padding="SAME")
	'        linear = tf.reshape(patches, [3*3*4])
	'        sess = tf.Session()
	'        out = sess.run([patches,linear])
	'         
			Dim [in] As INDArray = Nd4j.linspace(1,9,9, DataType.FLOAT).reshape("c"c, 1,3,3,1)
			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 1,3,3,4)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("extract_image_patches").addInputs([in]).addOutputs([out]).addIntegerArguments(2,2, 1,1, 1,1, 1).build()

			Nd4j.Executioner.exec(op)

			Dim exp As INDArray = Nd4j.create(DataType.FLOAT, 1,3,3,4)
			exp.get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all()).assign(Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 4, 5},
				New Double() {2, 3, 5, 6},
				New Double() {3, 0, 6, 0}
			}))

			exp.get(NDArrayIndex.point(0), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all()).assign(Nd4j.createFromArray(New Double()(){
				New Double() {4, 5, 7, 8},
				New Double() {5, 6, 8, 9},
				New Double() {6, 0, 9, 0}
			}))

			exp.get(NDArrayIndex.point(0), NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all()).assign(Nd4j.createFromArray(New Double()(){
				New Double() {7, 8, 0, 0},
				New Double() {8, 9, 0, 0},
				New Double() {9, 0, 0, 0}
			}))
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentProdBpSimple()
		Public Overridable Sub testSegmentProdBpSimple()

			Dim segmentIdxs As INDArray = Nd4j.create(New Double(){0, 0, 0, 1, 2, 2, 3, 3}, New Long(){8}).castTo(DataType.INT)
			Dim data As INDArray = Nd4j.create(New Double(){5, 1, 7, 2, 3, 4, 1, 3}, New Long(){8})
			Dim grad As INDArray = Nd4j.createFromArray(1.0,2.0,3.0,4.0)
			Dim numSegments As Integer = 4

			Dim gradData As INDArray = data.like()
			Dim gradIdxs As INDArray = segmentIdxs.like()

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("unsorted_segment_prod_bp").addInputs(data,segmentIdxs,grad).addIntegerArguments(numSegments).addOutputs(gradData, gradIdxs).build()

			Nd4j.Executioner.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulRank4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMmulRank4()
			Nd4j.Random.setSeed(12345)

			Dim arr1 As INDArray = Nd4j.rand(DataType.FLOAT, 32, 12, 128, 64)
			Dim arr2 As INDArray = Nd4j.rand(DataType.FLOAT, 32, 12, 128, 64)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("matmul").addInputs(arr1, arr2).addIntegerArguments(0, 1).build()

			Dim shapes As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, shapes.Count)
			Dim shape() As Long = {32, 12, 128, 128}
			assertArrayEquals(shape, shapes(0).getShape())

			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, shape)

			Dim outExp As INDArray = [out].like()
			For i As Integer = 0 To 31
				For j As Integer = 0 To 11
					Dim sub1 As INDArray = arr1.get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all())
					Dim sub2 As INDArray = arr2.get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all())
					Dim mmul As INDArray = sub1.mmul(sub2.transpose())
					outExp.get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all()).assign(mmul)
				Next j
			Next i

			op.setOutputArgument(0, [out])
			Nd4j.exec(op)

			assertEquals(outExp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulRank4_simple()
		Public Overridable Sub testMmulRank4_simple()

			Dim arr1 As INDArray = Nd4j.ones(DataType.FLOAT, 32, 12, 128, 64)
			Dim arr2 As INDArray = Nd4j.ones(DataType.FLOAT, 32, 12, 128, 64)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("matmul").addInputs(arr1, arr2).addIntegerArguments(0, 1).build()

			Dim shapes As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, shapes.Count)
			Dim shape() As Long = {32, 12, 128, 128}
			assertArrayEquals(shape, shapes(0).getShape())

			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, shape)

			op.setOutputArgument(0, [out])
			Nd4j.exec(op)
	'        System.out.println(out);

			Dim exp As INDArray = Nd4j.valueArrayOf(shape, 64.0, DataType.FLOAT) 'Each entry in output is sum of 64 (1.0 x 1.0) multiplications
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNthElementRank1()
		Public Overridable Sub testNthElementRank1()
			Dim [in] As INDArray = Nd4j.createFromArray(New Double(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9})
			Dim n As INDArray = Nd4j.scalar(0)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("nth_element").addInputs([in],n).addIntegerArguments(0).build()

			Dim shapeList As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim shape() As Long = shapeList(0).getShape()
			Dim expShape(-1) As Long
			assertArrayEquals(expShape, shape)

			Dim [out] As INDArray = Nd4j.scalar(0.0)
			op.addOutputArgument([out])

			Nd4j.Executioner.exec(op)
			Console.WriteLine([out])
			assertEquals(0.0, [out].getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorMmulShape()
		Public Overridable Sub testTensorMmulShape()
			Dim a As INDArray = Nd4j.create(New Double(){2}).reshape(ChrW(1))
			Dim b As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}).reshape(ChrW(2), 1, 2)
			Dim axes()() As Integer = {
				New Integer() {0},
				New Integer() {1}
			}

			Dim op As CustomOp = DynamicCustomOp.builder("tensordot").addInputs(a, b).addIntegerArguments(axes(0).Length).addIntegerArguments(axes(0)).addIntegerArguments(axes(1).Length).addIntegerArguments(axes(1)).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertArrayEquals(New Long(){2, 2}, l(0).getShape()) 'Returning [1,2,2]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorMmulShape2()
		Public Overridable Sub testTensorMmulShape2()
			Dim a As INDArray = Nd4j.create(New Double(){2}).reshape(ChrW(1))
			Dim b As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}).reshape(ChrW(2), 1, 2)
			Dim c As INDArray = Nd4j.tensorMmul(a, b, New Integer()(){
				New Integer(){0},
				New Integer(){1}
			})
			assertArrayEquals(New Long(){2, 2}, c.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStopGradient()
		Public Overridable Sub testStopGradient()

			Dim sd As SameDiff = SameDiff.create()
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.DOUBLE, 3, 4))
			Dim v As SDVariable = (New StopGradient(sd, w)).outputVariable()
			Dim loss As SDVariable = v.std(True)

			Dim gm As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, v.name(), w.name())

			Dim vArr As INDArray = gm(v.name())
			Dim wArr As INDArray = gm(w.name())

	'        System.out.println(vArr);
	'        System.out.println(wArr);

			assertEquals(Nd4j.zeros(DataType.DOUBLE, 3, 4), wArr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCheckNumerics()
		Public Overridable Sub testCheckNumerics()
			OpValidationSuite.ignoreFailing() 'https://github.com/eclipse/deeplearning4j/issues/7927

			Dim sd As SameDiff = SameDiff.create()
			Dim ph As SDVariable = sd.placeHolder("in", DataType.DOUBLE, 3, 4)
			Dim msg As SDVariable = sd.constant("message", Nd4j.scalar("My error message!"))
			Dim checkNumerics As SDVariable = (New CheckNumerics(sd, ph, msg)).outputVariable()
			Dim loss As SDVariable = checkNumerics.std("loss",True)

			Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 4)
			Dim expLoss As INDArray = [in].std(True)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput(checkNumerics.name(), [in]).placeholderValue("in", [in]).expectedOutput("loss", expLoss))
			Preconditions.checkState(err Is Nothing, err)


			'Also check that it actually does what it's supposed to:
			sd.outputAll(Collections.singletonMap("in", [in]))

			[in].putScalar(0, Double.NaN)
			Try
				sd.outputAll(Collections.singletonMap("in", [in]))
				fail("Expected exception")
			Catch t As Exception
				'OK
			End Try

			[in].putScalar(0, Double.PositiveInfinity)
			Try
				sd.outputAll(Collections.singletonMap("in", [in]))
				fail("Expected exception")
			Catch t As Exception
				'OK
			End Try

			[in].putScalar(0, 0.0)
			sd.outputAll(Collections.singletonMap("in", [in]))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCheckNumerics2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCheckNumerics2(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 4)
			Dim msg As INDArray = Nd4j.scalar("My error message!")

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("check_numerics").addInputs([in], msg).addOutputs([in].like()).build()

			Nd4j.Executioner.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHistogramFixedWidth()
		Public Overridable Sub testHistogramFixedWidth()
			'Bins: [-inf, 0.2), [0.2, 0.4), [0.4, 0.6), [0.6, 0.8), [0.8, inf]
			Dim [in] As INDArray = Nd4j.createFromArray(0.0, 0.1, 0.1, 0.3, 0.5, 0.5, 0.9)
			Dim range As INDArray = Nd4j.createFromArray(0.0, 1.0)
			Dim n As INDArray = Nd4j.scalar(5)

			Dim [out] As INDArray = Nd4j.create(DataType.INT, 5)

			Nd4j.exec(DynamicCustomOp.builder("histogram_fixed_width").addInputs([in], range, n).addOutputs([out]).build())

			Dim exp As INDArray = Nd4j.createFromArray(3, 1, 2, 0, 1)
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDynamicPartition()
		Public Overridable Sub testDynamicPartition()
			Dim data As INDArray = Nd4j.createFromArray(2, 1, 2, 0)
			Dim partitions As INDArray = Nd4j.createFromArray(0, 2, 1, 0)
			Dim [out]() As INDArray = Nd4j.exec(DynamicCustomOp.builder("dynamic_partition").addOutputs(Nd4j.createUninitialized(DataType.INT, 2), Nd4j.createUninitialized(DataType.INT, 1), Nd4j.createUninitialized(DataType.INT, 1)).addIntegerArguments(3).addInputs(data, partitions).build())

			Dim exp0 As INDArray = Nd4j.createFromArray(2, 0)
			Dim exp1 As INDArray = Nd4j.createFromArray(2)
			Dim exp2 As INDArray = Nd4j.createFromArray(1)

			assertEquals(exp0, [out](0)) 'Usually just gives [0,0]
			assertEquals(exp1, [out](1))
			assertEquals(exp2, [out](2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testListDiff()
		Public Overridable Sub testListDiff()
			Dim x As INDArray = Nd4j.createFromArray(0, 1, 2, 3)
			Dim y As INDArray = Nd4j.createFromArray(3, 1)

			Dim [out] As INDArray = Nd4j.create(DataType.INT, 2)
			Dim outIdx As INDArray = Nd4j.create(DataType.INT, 2)

			Nd4j.exec(DynamicCustomOp.builder("listdiff").addInputs(x, y).addOutputs([out], outIdx).build())

			Dim exp As INDArray = Nd4j.createFromArray(0, 2)

			assertEquals(exp, [out]) 'Values in x not in y
			assertEquals(exp, outIdx) 'Indices of the values in x not in y
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDivideNoNan(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDivideNoNan(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'TODO: implement DivideNoNan.doDiff()

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim in1 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim in2 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			Dim input1 As SDVariable = sameDiff.var(in1)
			Dim input2 As SDVariable = sameDiff.var(in2)

			Dim expected As INDArray = Nd4j.ones(3,4)

			Dim output As SDVariable = (New DivideNoNan(sameDiff, input1, input2)).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDigamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDigamma(ByVal backend As Nd4jBackend)

			Dim in1 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){-0.5772157, 0.42278433, 0.9227843, 1.2561177, 1.5061177, 1.7061176, 1.8727844, 2.0156415, 2.1406415, 2.2517526, 2.3517525, 2.4426618}).reshape(ChrW(3), 4)

			Dim tc As val = (New OpTestCase(New Digamma(in1))).expectedOutput(0, expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlatten(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlatten(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 27, 1).reshape(ChrW(3), 3, 3)
			Dim sdx As SDVariable = sameDiff.var(x)

			Dim expected As INDArray = Nd4j.linspace(DataType.DOUBLE,1,27,1)

			Dim output As SDVariable = (New Flatten(sameDiff, "c"c, sdx)).outputVariable()
			Dim loss As SDVariable = sameDiff.standardDeviation(sdx, True)
			sameDiff.addLossVariable(loss)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFusedBatchNorm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFusedBatchNorm(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing()
			Dim sameDiff As SameDiff = SameDiff.create()

			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 2*2*3*4).reshape(ChrW(2), 2, 3, 4)
			Dim scale As INDArray = Nd4j.create(DataType.DOUBLE, 4)
			scale.assign(0.5)
			Dim offset As INDArray = Nd4j.create(DataType.DOUBLE, 4)
			offset.assign(2.0)

			Dim input1 As SDVariable = sameDiff.var(x)
			Dim input2 As SDVariable = sameDiff.var(scale)
			Dim input3 As SDVariable = sameDiff.var(offset)

			Dim expectedY As INDArray = Nd4j.createFromArray(New Double(){ 985.5258, 985.5258, 985.5258, 985.5258, 659.7321, 659.7321, 659.7321, 659.7321, 399.0972, 399.0972, 399.0972, 399.0972, 203.6210, 203.6210, 203.6210, 203.6210, 73.3036, 73.3036, 73.3036, 73.3036, 8.1448, 8.1448, 8.1448, 8.1448, 8.1448, 8.1448, 8.1448, 8.1448, 73.3036, 73.3036, 73.3036, 73.3036, 203.6210, 203.6210, 203.6210, 203.6210, 399.0972, 399.0972, 399.0972, 399.0972, 659.7321, 659.7321, 659.7321, 659.7321, 985.5258, 985.5258, 985.5258, 985.5258}).reshape(x.shape())
			Dim expectedBatchMean As INDArray = Nd4j.createFromArray(New Double(){23.0, 24.0, 25.0, 26.0})
			Dim expectedBatchVar As INDArray = Nd4j.createFromArray(New Double(){208.00001526, 208.00001526, 208.00001526, 208.00001526})

			Dim outputs() As SDVariable = (New FusedBatchNorm(sameDiff, input1, input2, input3, 0, 1)).outputVariables()
			Dim loss As SDVariable = sameDiff.standardDeviation(input1, True)
			sameDiff.addLossVariable(loss)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(outputs(0).name(), expectedY).expectedOutput(outputs(1).name(), expectedBatchMean).expectedOutput(outputs(2).name(), expectedBatchVar)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIgamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIgamma(ByVal backend As Nd4jBackend)

			Dim in1 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim in2 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){0.63212055, 0.59399414, 0.5768099, 0.56652874, 0.5595013, 0.5542634, 0.5501591, 0.5463888, 0.54329145, 0.54048204, 0.5378594, 0.53233755}).reshape(ChrW(3), 4)

			Dim tc As val = (New OpTestCase(New Igamma(in1, in2))).expectedOutput(0, expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIgammaC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIgammaC(ByVal backend As Nd4jBackend)

			Dim in1 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim in2 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)


			Dim expected As INDArray = Nd4j.createFromArray(New Double(){0.36787945, 0.40600586, 0.42319012, 0.43347126, 0.4404987, 0.44573656, 0.4498409, 0.45361117, 0.45670855, 0.459518, 0.46214062, 0.46766248}).reshape(ChrW(3), 4)

			Dim tc As val = (New OpTestCase(New Igammac(in1, in2))).expectedOutput(0, expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLgamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLgamma(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim [in] As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 12, 1).reshape(ChrW(3), 4)
			Dim sdInput As SDVariable = sameDiff.var([in])

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){0.0, 0.0, 0.6931472, 1.7917595, 3.1780539, 4.787492, 6.5792513, 8.525162, 10.604603, 12.801827, 15.104413, 17.502308}).reshape(ChrW(3), 4)

			Dim output As SDVariable = (New Lgamma(sameDiff, sdInput)).outputVariable()

			Dim loss As SDVariable = sameDiff.standardDeviation(sdInput, True)
			sameDiff.addLossVariable(loss)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLu(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLu(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim in1 As INDArray = Nd4j.createFromArray(New Double(){ 1.0, 2.0, 3.0, 0.0, 2.0, 3.0, 0.0, 0.0, 7.0 }).reshape(ChrW(3), 3)

			Dim input1 As SDVariable = sameDiff.var(in1)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 1.0, 2.0, 3.0, 0.0, 2.0, 3.0, 0.0, 0.0, 7 }).reshape(ChrW(3), 3)

			Dim pexpected As INDArray = Nd4j.createFromArray(New Integer(){ 0, 1, 2 })

			sameDiff.loss_Conflict.l2Loss(input1)
			Dim output() As SDVariable = (New Lu(sameDiff, input1)).outputVariables()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output(0).name(), expected).expectedOutput(output(1).name(), pexpected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrixBandPart(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixBandPart(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing()
			Dim sameDiff As SameDiff = SameDiff.create()

			Dim input As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f}).reshape(ChrW(3), 4)

			Dim sdInput As SDVariable = sameDiff.var(input)
			Dim sdInput1 As SDVariable = sameDiff.constant(1)
			Dim sdInput2 As SDVariable = sameDiff.constant(-1)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.0f, 0.9234f, 0.0856f, 0.7938f }).reshape(ChrW(3), 4)

			sameDiff.loss_Conflict.l2Loss(sdInput)
			Dim output As SDVariable = (New MatrixBandPart(sameDiff, sdInput, 1, -1)).outputVariable()

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPolygamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPolygamma(ByVal backend As Nd4jBackend)

			Dim in1 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
			Dim in2 As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){1.644934, -0.4041138, 0.1189394, -0.03750069, 0.01226151, -0.0041002957, 0.001392272, -4.780109E-4, 1.6549716E-4, -5.7675967E-5, 2.0206635E-5, -7.1101636E-6}).reshape(ChrW(3), 4)

			Dim tc As val = (New OpTestCase(New Polygamma(in1, in2))).expectedOutput(0, expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTriangularSolve(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTriangularSolve(ByVal backend As Nd4jBackend)

			Dim a As INDArray = Nd4j.createFromArray(New Single(){ 3.0f, 0.0f, 0.0f, 0.0f, 2.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f }).reshape(ChrW(4), 4)

			Dim b As INDArray = Nd4j.createFromArray(New Single(){ 4.0f, 2.0f, 4.0f, 2.0f }).reshape(ChrW(4), 1)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 1.333333f, 2.0f, 4.0f, 2.0f }).reshape(ChrW(4), 1)

			Dim tc As val = (New OpTestCase(New TriangularSolve(a, b, False, True))).expectedOutput(0, expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBiasAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBiasAdd(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim in1 As INDArray = Nd4j.linspace(1, 12, 12)
			Dim in2 As INDArray = Nd4j.linspace(1, 12, 12)

			Dim input1 As SDVariable = sameDiff.var(in1)
			Dim input2 As SDVariable = sameDiff.var(in2)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 2.0000, 4.0000, 6.0000, 8.0000, 10.0000, 12.0000, 14.0000, 16.0000, 18.0000, 20.0000, 22.0000, 24.0000 })

			Dim output As SDVariable = (New BiasAdd(sameDiff, input1, input2, False)).outputVariable()
			Dim loss As SDVariable = sameDiff.standardDeviation(input1, True)
			sameDiff.addLossVariable(loss)
			Dim loss2 As SDVariable = sameDiff.standardDeviation(input2, True)
			sameDiff.addLossVariable(loss2)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(output.name(), expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBiasAddGrad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBiasAddGrad(ByVal backend As Nd4jBackend)

			Dim sameDiff As SameDiff = SameDiff.create()

			Dim x As INDArray = Nd4j.linspace(DataType.FLOAT,1, 24, 24).reshape(ChrW(2), 2, 2, 3)
			Dim grad As INDArray = Nd4j.linspace(DataType.FLOAT, 0.1, 0.1, 24).reshape(ChrW(2), 2, 2, 3)

			Dim bias As INDArray = Nd4j.createFromArray(New Single(){-1.0f, -2.0f, -3.0f})

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){9.2f, 10.0f, 10.8f})

			Dim tc As OpTestCase = (New OpTestCase(New BiasAddGrad(x, bias, grad,False))).expectedOutput(0, grad).expectedOutput(1, expected)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRoll(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRoll(ByVal backend As Nd4jBackend)

			Dim x As INDArray = Nd4j.createFromArray(New Double(){ 11.11, 11.12, 11.21, 11.22, 11.31, 11.32, 11.41, 11.42, 12.11, 12.12, 12.21, 12.22, 12.31, 12.32, 12.41, 12.42, 21.11, 21.12, 21.21, 21.22, 21.31, 21.32, 21.41, 21.42, 22.11, 22.12, 22.21, 22.22, 22.31, 22.32, 22.41, 22.42}).reshape(ChrW(2), 2, 4, 2)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 22.21, 22.22, 22.31, 22.32, 22.41, 22.42, 11.11, 11.12, 11.21, 11.22, 11.31, 11.32, 11.41, 11.42, 12.11, 12.12, 12.21, 12.22, 12.31, 12.32, 12.41, 12.42, 21.11, 21.12, 21.21, 21.22, 21.31, 21.32, 21.41, 21.42, 22.11, 22.12 }).reshape(x.shape())

			Dim shift As Integer = 6

			Dim tc As val = (New OpTestCase(New Roll(x,shift))).expectedOutput(0,expected)
			Dim err As String = OpValidation.validate(tc)

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSeqMask()
		Public Overridable Sub testSeqMask()
			Dim arr As INDArray = Nd4j.createFromArray(1,2,3)
			Dim maxLen As INDArray = Nd4j.scalar(4)

			Dim [out] As INDArray = Nd4j.create(DataType.INT32, 3, 4)
			[out].assign(Integer.MaxValue)

			Nd4j.exec(DynamicCustomOp.builder("sequence_mask").addInputs(arr, maxLen).addOutputs([out]).build())

			Dim exp As INDArray = Nd4j.createFromArray(New Integer()(){
				New Integer() {1, 0, 0, 0},
				New Integer() {1, 1, 0, 0},
				New Integer() {1, 1, 1, 0}
			})

			assertEquals(exp, [out])
		End Sub
	End Class

End Namespace