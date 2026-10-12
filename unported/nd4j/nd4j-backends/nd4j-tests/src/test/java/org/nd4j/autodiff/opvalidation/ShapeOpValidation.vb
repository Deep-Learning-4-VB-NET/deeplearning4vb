Imports System
Imports System.Collections.Generic
Imports Lists = com.google.common.collect.Lists
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports LUDecomposition = org.apache.commons.math3.linear.LUDecomposition
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TestInfo = org.junit.jupiter.api.TestInfo
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
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Tri = org.nd4j.linalg.api.ops.custom.Tri
Imports Triu = org.nd4j.linalg.api.ops.custom.Triu
Imports DiagPart = org.nd4j.linalg.api.ops.impl.shape.DiagPart
Imports MergeMaxIndex = org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex
Imports Permute = org.nd4j.linalg.api.ops.impl.shape.Permute
Imports SequenceMask = org.nd4j.linalg.api.ops.impl.shape.SequenceMask
Imports SizeAt = org.nd4j.linalg.api.ops.impl.shape.SizeAt
Imports Transpose = org.nd4j.linalg.api.ops.impl.shape.Transpose
Imports Unstack = org.nd4j.linalg.api.ops.impl.shape.Unstack
Imports Fill = org.nd4j.linalg.api.ops.impl.transforms.custom.Fill
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports CheckUtil = org.nd4j.linalg.checkutil.CheckUtil
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.junit.jupiter.api.Assertions
Imports org.nd4j.linalg.indexing.NDArrayIndex

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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) public class ShapeOpValidation extends BaseOpValidation
	Public Class ShapeOpValidation
		Inherits BaseOpValidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat(org.nd4j.linalg.factory.Nd4jBackend backend, org.junit.jupiter.api.TestInfo testInfo)
		Public Overridable Sub testConcat(ByVal backend As Nd4jBackend, ByVal testInfo As TestInfo)
	'        int[] concatDim = new int[]{0,0,0,1,1,1,2,2,2};
			Dim concatDim() As Integer = {0, 0, 0}
			Dim origShapes As IList(Of IList(Of Integer())) = New List(Of IList(Of Integer()))()
			origShapes.Add(New List(Of Integer()) From {
				New Integer(){3, 4},
				New Integer(){5, 4}
			})
			origShapes.Add(New List(Of Integer()) From {
				New Integer(){1, 2, 3},
				New Integer(){1, 2, 3},
				New Integer(){2, 2, 3}
			})
			origShapes.Add(New List(Of Integer()) From {
				New Integer(){1, 2, 3, 4},
				New Integer(){2, 2, 3, 4}
			})

			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To concatDim.Length - 1

				Dim sd As SameDiff = SameDiff.create()
				Dim shapes As IList(Of Integer()) = origShapes(i)

				Dim toConcat(shapes.Count - 1) As SDVariable
				Dim orig(shapes.Count - 1) As INDArray
				For j As Integer = 0 To shapes.Count - 1
					orig(j) = Nd4j.rand(DataType.DOUBLE, shapes(j))
					toConcat(j) = sd.var("concat-in-" & j.ToString(), orig(j))
				Next j

				Dim sdConcat As SDVariable = sd.concat("c", 0, toConcat)
				Dim stdev As SDVariable = sd.standardDeviation("out", sdConcat, True)

				Dim msg As String = "i=" & i & ", concatDim=" & concatDim(i)
				Dim tc As New TestCase(sd)
				tc.testName(msg).expectedOutput("c", Nd4j.concat(concatDim(i), orig))

				Dim [error] As String = OpValidation.validate(tc)
				If [error] IsNot Nothing Then
					failed.Add(testInfo.getTestMethod().get().getName())
				End If
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeGradient(ByVal backend As Nd4jBackend)
			'https://github.com/eclipse/deeplearning4j/issues/6873

			Dim origShape() As Integer = {3, 4, 5}

			Dim failed As IList(Of String) = New List(Of String)()

			For Each toShape As Long() In New Long()(){
				New Long() {3, 4 * 5},
				New Long() {3 * 4, 5},
				New Long() {1, 3 * 4 * 5},
				New Long() {3 * 4 * 5, 1}
			}
				For Each order As Char In New Char(){"c"c, "f"c}
					Dim inArr As INDArray = Nd4j.rand(DataType.DOUBLE, origShape, order).muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim reshape As SDVariable = sd.reshape([in], toShape)
					'Using stdev here: mean/sum would backprop the same gradient for each input...
					Dim stdev As SDVariable = sd.standardDeviation("out", reshape, True)

					Dim [out] As INDArray = stdev.eval()
					Dim expOut As INDArray = [in].Arr.std(True, Integer.MaxValue)

					Dim msg As String = "toShape=" & java.util.Arrays.toString(toShape) & ", order=" & order
					Dim tc As New TestCase(sd)
					tc.testName(msg).expectedOutput("out", expOut)

					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add([error])
					End If
				Next order
			Next toShape

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermuteGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermuteGradient(ByVal backend As Nd4jBackend)
			Dim origShape() As Integer = {3, 4, 5}

			Dim failed As IList(Of String) = New List(Of String)()

			For Each perm As Integer() In New Integer()(){
				New Integer() {0, 1, 2},
				New Integer() {0, 2, 1},
				New Integer() {1, 0, 2},
				New Integer() {1, 2, 0},
				New Integer() {2, 0, 1},
				New Integer() {2, 1, 0}
			}
				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, origShape, DataType.DOUBLE)
					Dim msg As String = "permute=" & java.util.Arrays.toString(perm) & ", source=" & p.Second
					Console.WriteLine(msg)

					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim permute As SDVariable = sd.permute([in], perm)
					'Using stdev here: mean/sum would backprop the same gradient for each input...
					Dim stdev As SDVariable = sd.standardDeviation("out", permute, True)

					Dim exp As INDArray = inArr.permute(perm)
					Dim expOut As INDArray = [in].Arr.std(True, Integer.MaxValue)


					Dim tc As New TestCase(sd)
					tc.testName(msg).expected("out", expOut).expected(permute, exp)

					Dim [error] As String = OpValidation.validate(tc, True)
					If [error] IsNot Nothing Then
						failed.Add(msg)
					End If
				Next p
			Next perm

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRank(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRank(ByVal backend As Nd4jBackend)

			Dim inShape As IList(Of Long()) = New List(Of Long()) From {
				Nothing, New Long(){1},
				New Long(){6},
				New Long(){3, 4},
				New Long(){3, 4, 5}
			}

			For Each shape As Long() In inShape

				Dim sd As SameDiff = SameDiff.create()
				Dim var As SDVariable
				If shape Is Nothing Then
					var = sd.var("in", Nd4j.scalar(1.0))
				Else
					var = sd.var("in", Nd4j.create(DataType.DOUBLE, shape))
				End If

				Dim rank As SDVariable = sd.rank(var)

				Dim expRank As INDArray = Nd4j.scalar(DataType.INT,If(shape Is Nothing, 0, shape.Length))
				Dim msg As String = "Rank " & (If(shape Is Nothing, 0, shape.Length))
				Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(False).expected(rank, expRank))

				assertNull(err)
			Next shape
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExpandDimsGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExpandDimsGradient(ByVal backend As Nd4jBackend)
			Dim origShape As val = New Long(){3, 4}

			Dim failed As IList(Of String) = New List(Of String)()

			Dim first As Boolean = True
			For i As Integer = 0 To 2

				Dim expExpandShape() As Long
				Select Case i
					Case 0
						expExpandShape = New Long(){1, 3, 4}
					Case 1
						expExpandShape = New Long(){3, 1, 4}
					Case 2
						expExpandShape = New Long(){3, 4, 1}
					Case Else
						Throw New Exception()
				End Select

				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAllTestMatricesWithShape(origShape(0), origShape(1), 12345, DataType.DOUBLE)
					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim expand As SDVariable = sd.expandDims([in], i)
					'Using stdev here: mean/sum would backprop the same gradient for each input...
					Dim stdev As SDVariable = sd.standardDeviation("out", expand, True)

					Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)
					Dim expOut As INDArray = [in].Arr.std(True)

					assertArrayEquals(expExpandShape, m(expand.name()).shape())
					Dim expExpand As INDArray = inArr.dup("c"c).reshape(expExpandShape)

					Dim msg As String = "expandDim=" & i & ", source=" & p.Second
					log.info("Starting: " & msg)

					Dim tc As New TestCase(sd)
					tc.testName(msg).expectedOutput("out", expOut).expectedOutput(expand.name(), expExpand)

					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add([error])
					End If
				Next p
			Next i
			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSqueezeGradient(org.nd4j.linalg.factory.Nd4jBackend backend,org.junit.jupiter.api.TestInfo testInfo)
		Public Overridable Sub testSqueezeGradient(ByVal backend As Nd4jBackend, ByVal testInfo As TestInfo)
			Dim origShape As val = New Long(){3, 4, 5}

			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To 2

				Dim shape As val = origShape.clone()
				shape(i) = 1

				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, shape, DataType.DOUBLE)
					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim squeeze As SDVariable = sd.squeeze([in], i)
					'Using stdev here: mean/sum would backprop the same gradient for each input...
					Dim stdev As SDVariable = sd.standardDeviation("out", squeeze, True)

					Dim expShapePostSqueeze() As Long
					Select Case i
						Case 0
							expShapePostSqueeze = New Long(){4, 5}
						Case 1
							expShapePostSqueeze = New Long(){3, 5}
						Case 2
							expShapePostSqueeze = New Long(){3, 4}
						Case Else
							Throw New Exception()
					End Select

					Dim exp As INDArray = inArr.dup("c"c).reshape("c"c, expShapePostSqueeze)

					Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)

					Dim squeezed As INDArray = m(squeeze.name())
	'                assertArrayEquals(expShapePostSqueeze, squeezed.shape());

					Dim [out] As INDArray = m(stdev.name())
					Dim expOut As INDArray = [in].Arr.std(True, Integer.MaxValue)
					assertEquals(expOut, [out])

					Dim msg As String = "squeezeDim=" & i & ", source=" & p.Second
					Dim tc As TestCase = (New TestCase(sd)).testName(msg).expected(squeeze.name(), exp).expectedOutput("out", expOut)


					Dim [error] As String = OpValidation.validate(tc, True)
					If [error] IsNot Nothing Then
						failed.Add(testInfo.getTestMethod().get().getName())
					End If
				Next p
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceGradient(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			'Order here: original shape, begin, size
			Dim testCases As IList(Of Triple(Of Integer(), Integer(), Integer())) = New List(Of Triple(Of Integer(), Integer(), Integer()))()
			testCases.Add(New Triple(Of Integer(), Integer(), Integer())(New Integer(){3, 4}, New Integer(){0, 0}, New Integer(){3, 4}))
			testCases.Add(New Triple(Of Integer(), Integer(), Integer())(New Integer(){3, 4}, New Integer(){1, 1}, New Integer(){2, 2}))
			testCases.Add(New Triple(Of Integer(), Integer(), Integer())(New Integer(){3, 4}, New Integer(){1, 2}, New Integer(){2, 2}))
			testCases.Add(New Triple(Of Integer(), Integer(), Integer())(New Integer(){3, 4, 5}, New Integer(){0, 0, 0}, New Integer(){3, 4, 5}))
			testCases.Add(New Triple(Of Integer(), Integer(), Integer())(New Integer(){3, 4, 5}, New Integer(){1, 1, 1}, New Integer(){2, 3, 4}))

'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As IDictionary(Of Integer, INDArrayIndex()) = New Dictionary(Of Integer, INDArrayIndex())()
			indices(0) = New INDArrayIndex(){all(), all()}
			indices(1) = New INDArrayIndex(){interval(1,3), interval(1,3)}
			indices(2) = New INDArrayIndex(){interval(1,3), interval(2,4)}
			indices(3) = New INDArrayIndex(){all(), all(), all()}
			indices(4) = New INDArrayIndex(){interval(1,3), interval(1,4), interval(1,5)}

			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To testCases.Count - 1
				Dim t As Triple(Of Integer(), Integer(), Integer()) = testCases(i)
				Dim os() As Integer = t.getFirst()
				Dim b() As Integer = t.getSecond()
				Dim e() As Integer = t.getThird()
				Dim prod As Integer = ArrayUtil.prod(os)
				Dim arr As INDArray = Nd4j.linspace(1, prod, prod, DataType.DOUBLE).reshape(os)

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", arr)
				Dim slice As SDVariable = sd.slice([in], b, e)
				Dim stdev As SDVariable = sd.standardDeviation(slice, True)

				Dim msg As String = "i=" & i & ": inShape=" & java.util.Arrays.toString(os) & ", begin=" & java.util.Arrays.toString(b) & ", end=" & java.util.Arrays.toString(e)
				log.info("Starting test: " & msg)

				Dim tc As TestCase = (New TestCase(sd)).testName(msg)

				If indices_Conflict.ContainsKey(i) Then
					tc.expected(slice, arr.get(indices(i)).dup())
				End If

				Dim [error] As String = OpValidation.validate(tc, True)
				If [error] IsNot Nothing Then
					failed.Add([error])
				End If
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderClassName = "Builder") @Data private static class SSCase
		Private Class SSCase
			Friend shape() As Long
			Friend begin() As Long
			Friend [end]() As Long
			Friend strides() As Long
			Friend beginMask As Integer
			Friend endMask As Integer
			Friend ellipsisMask As Integer
			Friend newAxisMask As Integer
			Friend shrinkAxisMask As Integer

			Public Class Builder

'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function shape(ParamArray ByVal shape_Conflict() As Long) As Builder
					Me.shape = shape_Conflict
					Return Me
				End Function

'JAVA TO VB CONVERTER NOTE: The parameter begin was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function begin(ParamArray ByVal begin_Conflict() As Long) As Builder
					Me.begin = begin_Conflict
					Return Me
				End Function

'JAVA TO VB CONVERTER NOTE: The parameter end was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function [end](ParamArray ByVal end_Conflict() As Long) As Builder
					Me.end = end_Conflict
					Return Me
				End Function

'JAVA TO VB CONVERTER NOTE: The parameter strides was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function strides(ParamArray ByVal strides_Conflict() As Long) As Builder
					Me.strides = strides_Conflict
					Return Me
				End Function
			End Class
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceGradient(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			'Order here: original shape, begin, size
			Dim testCases As IList(Of SSCase) = New List(Of SSCase)()
			testCases.Add(SSCase.builder().shape(3, 4).begin(0, 0).end(3, 4).strides(1, 1).build())
			testCases.Add(SSCase.builder().shape(3, 4).begin(1, 1).end(2, 3).strides(1, 1).build())
			testCases.Add(SSCase.builder().shape(3, 4).begin(-999, 0).end(3, 4).strides(1, 1).beginMask(1).build())
			testCases.Add(SSCase.builder().shape(3, 4).begin(1, 1).end(3, -999).strides(1, 1).endMask(1 << 1).build())
			testCases.Add(SSCase.builder().shape(3, 4).begin(-999, 0).end(-999, 4).strides(1, 1).beginMask(1).endMask(1).build())

			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(0, 0, 0).end(3, 4, 5).strides(1, 1, 1).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, 2, 3).end(3, 4, 5).strides(1, 1, 1).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(0, 0, 0).end(3, 3, 5).strides(1, 2, 2).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, -999, 1).end(3, 3, 4).strides(1, 1, 1).beginMask(1 << 1).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, -999, 1).end(3, 3, -999).strides(1, 1, 1).beginMask(1 << 1).endMask(1 << 2).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, 2).end(3, 4).strides(1, 1).ellipsisMask(1 << 1).build()) '[1:3,...,2:4]
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, -999, 1, 2).end(3, -999, 3, 4).strides(1, -999, 1, 2).newAxisMask(1 << 1).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, 0, 1).end(3, -999, 4).strides(1, 1, 1).shrinkAxisMask(1 << 1).build())
			testCases.Add(SSCase.builder().shape(3, 4, 5).begin(1, 1, 1).end(3, -999, 4).strides(1, 1, 1).shrinkAxisMask(1 << 1).build())

'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As IDictionary(Of Integer, INDArrayIndex()) = New Dictionary(Of Integer, INDArrayIndex())()
			indices(0) = New INDArrayIndex(){all(), all()}
			indices(1) = New INDArrayIndex(){interval(1,2), interval(1,3)}
			indices(2) = New INDArrayIndex(){interval(0,3), interval(0,4)}
			indices(3) = New INDArrayIndex(){interval(1,3), interval(1,4)}

			indices(5) = New INDArrayIndex(){all(), all(), all()}
			indices(7) = New INDArrayIndex(){interval(0,1,3), interval(0,2,3), interval(0,2,5)}


			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To testCases.Count - 1
				Dim t As SSCase = testCases(i)
				Dim arr As INDArray = Nd4j.rand(t.getShape())

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", arr)
				Dim slice As SDVariable = sd.stridedSlice([in], t.getBegin(), t.getEnd(), t.getStrides(), t.getBeginMask(), t.getEndMask(), t.getEllipsisMask(), t.getNewAxisMask(), t.getShrinkAxisMask())
				Dim stdev As SDVariable = sd.standardDeviation(slice, True)

				Dim msg As String = "i=" & i & ": " & t
				log.info("Starting test: " & msg)

				Dim tc As New TestCase(sd)
				tc.testName(msg)

				If indices_Conflict.ContainsKey(i) Then
					tc.expected(slice, arr.get(indices(i)).dup())
				End If

				Dim [error] As String = OpValidation.validate(tc, True)
				If [error] IsNot Nothing Then
					failed.Add([error])
				End If
			Next i
			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMerge(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMerge(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			For t As Integer = 0 To 2
				For Each numArrays As Integer In New Integer(){3, 1}
					For Each shape As Long() In New Long()(){
						New Long() {1},
						New Long() {3, 4},
						New Long() {3, 4, 5}
					}


						Dim sd As SameDiff = SameDiff.create()
						Dim arr(numArrays - 1) As SDVariable

						For i As Integer = 0 To numArrays - 1
							arr(i) = sd.var(i.ToString(), Nd4j.rand(shape))
						Next i

						Dim exp As INDArray = arr(0).Arr.dup()
						Dim merge As SDVariable
						Dim name As String
						Select Case t
							Case 0
								name = "mergeAdd"
								merge = sd.math().mergeAdd(arr)
								For i As Integer = 1 To numArrays - 1
									exp.addi(arr(i).Arr.dup())
								Next i
							Case 1
								name = "mergeMax"
								merge = sd.math().mergeMax(arr)
								For i As Integer = 1 To numArrays - 1
									exp = Transforms.max(exp, arr(i).Arr, True)
								Next i
							Case 2
								name = "mergeAvg"
								merge = sd.math().mergeAvg(arr)
								For i As Integer = 1 To numArrays - 1
									exp.addi(arr(i).Arr.dup())
								Next i
								exp.divi(numArrays)
							Case Else
								Throw New Exception()
						End Select

						Dim msg As String = name & " - numArrays=" & numArrays & ", shape=" & java.util.Arrays.toString(shape)
						Dim loss As SDVariable
						If shape.Length > 1 Then
							loss = sd.standardDeviation("loss", merge, True)
						Else
							loss = sd.mean("loss", merge)
						End If


						Dim tc As TestCase = (New TestCase(sd)).expected(merge, exp).testName(msg)
						Dim [error] As String = OpValidation.validate(tc, True)
						If [error] IsNot Nothing Then
							failed.Add(msg & " - " & [error])
						End If
					Next shape
				Next numArrays
			Next t

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStack(org.nd4j.linalg.factory.Nd4jBackend backend,org.junit.jupiter.api.TestInfo testInfo)
		Public Overridable Sub testStack(ByVal backend As Nd4jBackend, ByVal testInfo As TestInfo)
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			Dim origShape As IList(Of Long()) = New List(Of Long()) From {
				New Long(){1},
				New Long(){1, 1},
				New Long(){3, 4},
				New Long(){3, 4, 5},
				New Long(){3, 4, 5, 6}
			}

			For Each shape As Long() In origShape
				Dim axis As Integer = 0
				Do While axis <= shape.Length
					For Each numInputs As Integer In New Integer(){1, 3}

						Dim expOutShape(shape.Length) As Long
						Dim x As Integer = 0
						For i As Integer = 0 To shape.Length
							If i = axis Then
								expOutShape(i) = numInputs
							Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: expOutShape[i] = shape[x++];
								expOutShape(i) = shape(x)
									x += 1
							End If
						Next i


						Dim sd As SameDiff = SameDiff.create()

						Dim [in](numInputs - 1) As SDVariable
						Dim inArr(numInputs - 1) As INDArray
						For i As Integer = 0 To numInputs - 1
							inArr(i) = Nd4j.rand(shape)
							[in](i) = sd.var(i.ToString(), inArr(i))
						Next i

						Dim expStack As INDArray = Nothing
						If New Long(){3, 4}.SequenceEqual(shape) Then
							If axis = 0 Then
								Dim [out] As INDArray = Nd4j.create(numInputs, 3, 4)
								For i As Integer = 0 To numInputs - 1
									[out].get(point(i), all(), all()).assign(inArr(i))
								Next i
								expStack = [out]
							ElseIf axis = 1 Then
								Dim [out] As INDArray = Nd4j.create(3, numInputs, 4)
								For i As Integer = 0 To numInputs - 1
									[out].get(all(), point(i), all()).assign(inArr(i))
								Next i
								expStack = [out]
							Else
								Dim [out] As INDArray = Nd4j.create(3, 4, numInputs)
								For i As Integer = 0 To numInputs - 1
									[out].get(all(), all(), point(i)).assign(inArr(i))
								Next i
								expStack = [out]
							End If
						End If

						Dim stack As SDVariable = sd.stack(axis, [in])

						Dim [out] As INDArray = stack.eval()
						assertArrayEquals(expOutShape, [out].shape())

						If ArrayUtil.prodLong(shape) = 1 Then
							Dim loss As SDVariable = sd.sum("loss", stack)
						Else
							Dim loss As SDVariable = sd.standardDeviation("loss", stack, True)
						End If

						Dim msg As String = java.util.Arrays.toString(shape) & ", axis=" & axis & ", numInputs=" & numInputs

						Dim tc As New TestCase(sd)
						If expStack IsNot Nothing Then
							tc.expected(stack, expStack)
						End If

						Dim [error] As String = OpValidation.validate(tc)
						If [error] IsNot Nothing Then
							failed.Add(testInfo.getTestMethod().get().getName())
						End If
					Next numInputs
					axis += 1
				Loop
			Next shape

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnStack(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnStack(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim failed As IList(Of String) = New List(Of String)()

			Dim unstackedShape As IList(Of Long()) = New List(Of Long()) From {
				New Long(){1},
				New Long(){1, 1},
				New Long(){3, 4},
				New Long(){3, 4, 5},
				New Long(){3, 4, 5, 6}
			}

			For Each shape As Long() In unstackedShape
				Dim axis As Integer = 0
				Do While axis <= shape.Length
	'                for (int numInputs : new int[]{1, 3}) {
					For Each numInputs As Integer In New Integer(){3}

						Dim stackedShape(shape.Length) As Long
						Dim x As Integer = 0
						For i As Integer = 0 To shape.Length
							If i = axis Then
								stackedShape(i) = numInputs
							Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: stackedShape[i] = shape[x++];
								stackedShape(i) = shape(x)
									x += 1
							End If
						Next i


						Dim sd As SameDiff = SameDiff.create()
						Dim [in] As INDArray = Nd4j.rand(stackedShape)
						Dim var As SDVariable = sd.var("var", [in])

						Dim unstacked() As SDVariable = sd.unstack(var, axis, numInputs)

						Dim unstackedExp() As INDArray = Nothing
						If New Long(){3, 4}.SequenceEqual(shape) Then
							unstackedExp = New INDArray(numInputs - 1){}
							If axis = 0 Then
								For i As Integer = 0 To numInputs - 1
									unstackedExp(i) = [in].get(point(i), all(), all())
								Next i
							ElseIf axis = 1 Then
								For i As Integer = 0 To numInputs - 1
									unstackedExp(i) = [in].get(all(), point(i), all())
								Next i
							Else
								For i As Integer = 0 To numInputs - 1
									unstackedExp(i) = [in].get(all(), all(), point(i))
								Next i
							End If
						End If

						'for gradient check, need to combine to single scalar output...
						Dim merged As SDVariable = sd.math().mergeAvg(unstacked)

						If ArrayUtil.prodLong(stackedShape) = 1 OrElse ArrayUtil.prodLong(shape) = 1 Then
							Dim loss As SDVariable = sd.sum("loss", merged)
						Else
							Dim loss As SDVariable = sd.standardDeviation("loss", merged, True)
						End If

						Dim msg As String = "Unstacked shape = " & java.util.Arrays.toString(shape) & ", stacked shape = " & java.util.Arrays.toString(stackedShape) & ", axis=" & axis & ", numInputs=" & numInputs

						Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)
						For Each v As SDVariable In unstacked
							assertArrayEquals(shape, m(v.name()).shape(),msg)
						Next v

						Dim tc As TestCase = (New TestCase(sd)).testName(msg)
						If unstackedExp IsNot Nothing Then
							For i As Integer = 0 To numInputs - 1
								tc.expected(unstacked(i), unstackedExp(i))
							Next i
						End If
						Dim [error] As String = OpValidation.validate(tc, True)
						If [error] IsNot Nothing Then
							failed.Add([error])
						End If
					Next numInputs
					axis += 1
				Loop
			Next shape

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTile(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTile(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim tileArg As IList(Of Integer()) = New List(Of Integer()) From {
				New Integer(){1},
				New Integer(){5},
				New Integer(){3, 4},
				New Integer(){2, 3},
				New Integer(){2, 3, 4}
			}

			Dim orig(tileArg.Count - 1) As INDArray
			orig(0) = Nd4j.valueArrayOf(New Long(){1}, 3.0)
			orig(1) = Nd4j.valueArrayOf(New Long(){1}, 3.0)
			orig(2) = Nd4j.valueArrayOf(New Long(){1, 1}, 3.0)
			orig(3) = Nd4j.rand(2,2).muli(10)
			orig(4) = Nd4j.rand(New Integer(){3, 4, 5}).muli(10)

			Dim exp(tileArg.Count - 1) As INDArray
			exp(0) = Nd4j.create(New Double(){3})
			exp(1) = Nd4j.create(New Double(){3, 3, 3, 3, 3})
			exp(2) = Nd4j.valueArrayOf(New Long(){3, 4}, 3.0)
			exp(3) = Nd4j.create(2*2, 2*3)
			For i As Integer = 0 To 1
				For j As Integer = 0 To 2
					exp(3).get(interval(2*i,2*(i+1)), interval(2*j,2*(j+1))).assign(orig(3))
				Next j
			Next i
			exp(4) = Nd4j.create(3*2, 4*3, 5*4)
			For i As Integer = 0 To 1
				For j As Integer = 0 To 2
					For k As Integer = 0 To 3
						exp(4).get(interval(3 * i, 3 * (i + 1)), interval(4 * j, 4 * (j + 1)), interval(5*k, 5*(k+1))).assign(orig(4))
					Next k
				Next j
			Next i

			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To tileArg.Count - 1
				Dim tArg() As Integer = tileArg(i)
				Dim inArr As INDArray = orig(i)
				log.info("Starting test {} - shape {}, tile arg {}", i, java.util.Arrays.toString(inArr.shape()), java.util.Arrays.toString(tArg))

				Dim sd As SameDiff = SameDiff.create()
				Dim var As SDVariable = sd.var("in", inArr)
				Dim tile As SDVariable = sd.tile(var, tArg)

				If exp(i).length() = 1 OrElse inArr.length() = 1 Then
					Dim loss As SDVariable = sd.sum("loss", tile)
				Else
					Dim loss As SDVariable = sd.standardDeviation("loss", tile, True)
				End If

				Dim msg As String = "Shape=" & java.util.Arrays.toString(inArr.shape()) & " - tile=" & java.util.Arrays.toString(tArg)

				Dim tc As TestCase = (New TestCase(sd)).expected(tile, exp(i)).gradCheckMinAbsError(5e-3).gradCheckMaxRelativeError(5e-3)
				Dim [error] As String = OpValidation.validate(tc)
				If [error] IsNot Nothing Then
					failed.Add(msg & " - " & [error])
				End If
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTileBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTileBp(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim [in] As INDArray = Nd4j.create(1,2,3) 'Values aren't used in backprop, just shape
			Dim tile() As Integer = {2, 3, 4}

			Dim outShape() As Integer = {1*2, 2*3, 3*4}
			Dim length As Integer = ArrayUtil.prod(outShape)
			Dim gradAtOut As INDArray = Nd4j.rand(outShape)

			Dim gradAtInExp As INDArray = Nd4j.create([in].shape())
			Dim i As Integer=0
			Do While i<tile(0)
				Dim j As Integer=0
				Do While j<tile(1)
					Dim k As Integer=0
					Do While k<tile(2)
						Dim subset As INDArray = gradAtOut.get(interval(i*1, (i+1)*1), interval(j*2, (j+1)*2), interval(k*3, (k+1)*3))
						gradAtInExp.addi(subset)
						k += 1
					Loop
					j += 1
				Loop
				i += 1
			Loop

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("tile_bp").addInputs([in], gradAtOut).addOutputs(gradAtInExp).addIntegerArguments(tile).build()
			Dim otc As OpTestCase = (New OpTestCase(op)).expectedOutput(0, gradAtInExp)

			Dim err As String = OpValidation.validate(otc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTileBp2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTileBp2(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim [in] As INDArray = Nd4j.create(3,4,5) 'Values aren't used in backprop, just shape
			Dim tile() As Integer = {2, 3, 4}

			Dim outShape() As Integer = {3*2, 4*3, 5*4}
			Dim length As Integer = ArrayUtil.prod(outShape)
			Dim gradAtOut As INDArray = Nd4j.rand(outShape)

			Dim gradAtInExp As INDArray = Nd4j.create([in].shape())
			Dim i As Integer=0
			Do While i<tile(0)
				Dim j As Integer=0
				Do While j<tile(1)
					Dim k As Integer=0
					Do While k<tile(2)
						Dim subset As INDArray = gradAtOut.get(interval(i*3, (i+1)*3), interval(j*4, (j+1)*4), interval(k*5, (k+1)*5))
						gradAtInExp.addi(subset)
						k += 1
					Loop
					j += 1
				Loop
				i += 1
			Loop

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("tile_bp").addInputs([in], gradAtOut).addOutputs(gradAtInExp).addIntegerArguments(tile).build()
			Dim otc As OpTestCase = (New OpTestCase(op)).expectedOutput(0, gradAtInExp)

			Dim err As String = OpValidation.validate(otc)
			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshape(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(-5, 6, 12)).reshape(ChrW(3), 4)
			Dim x As SDVariable = sameDiff.var("x", arr)
			Dim result1 As SDVariable = sameDiff.reshape(x, 4, 3)
			Dim loss As SDVariable = sameDiff.standardDeviation(result1, True)

			Dim exp As INDArray = arr.dup("c"c).reshape("c"c, 4,3)

			Dim err As String = OpValidation.validate((New TestCase(sameDiff)).expectedOutput(result1.name(), exp))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshape2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshape2(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim origShape() As Integer = {3, 4, 5}

			Dim inArr As INDArray = Nd4j.linspace(1, 60, 60).reshape(origShape)

			For Each toShape As Integer() In New Integer()(){
				New Integer() {3, 4 * 5},
				New Integer() {3 * 4, 5},
				New Integer() {1, 3 * 4 * 5},
				New Integer() {3 * 4 * 5, 1}
			}
				Dim exp As INDArray = inArr.reshape(toShape)

				Dim [out] As INDArray = Nd4j.create(toShape)
				Nd4j.Executioner.exec(DynamicCustomOp.builder("reshape").addInputs(inArr).addOutputs([out]).addIntegerArguments(-AscW("c"c)).addIntegerArguments(toShape).build())

				assertEquals(exp, [out])
			Next toShape
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTranspose(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 4, 4)).reshape(ChrW(1), 4)
			Dim x As SDVariable = sameDiff.var("x", arr)
			Dim result As SDVariable = sameDiff.transpose(x)
			Dim loss As SDVariable = sameDiff.standardDeviation(result, True)

			Dim err As String = OpValidation.validate((New TestCase(sameDiff)).expectedOutput(result.name(), arr.transpose()))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTransposeOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTransposeOp(ByVal backend As Nd4jBackend)

			Dim arr As INDArray = Nd4j.linspace(1,15, 15).reshape(ChrW(5), 3)
			Dim [out] As INDArray = Nd4j.create(Nd4j.defaultFloatingPointType(), New Long(){3, 5}, "c"c)

			Dim op As New OpTestCase(New Transpose(arr, [out]))
			Dim exp As INDArray = arr.transpose()
			op.expectedOutput(0, exp.dup("f"c))
			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShape(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim shape As val = New Long(){2, 3}
			Dim x As SDVariable = sameDiff.var("x", shape)
			Dim result As SDVariable = sameDiff.shape(x).castTo(DataType.DOUBLE)
			Dim loss As SDVariable = sameDiff.standardDeviation(result, True)

			Dim err As String = OpValidation.validate((New TestCase(sameDiff)).gradientCheck(False).expected(result, Nd4j.create(New Double(){2, 3}, New Long(){2})))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSize(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim shape As val = New Long(){2, 3}
			Dim x As SDVariable = sameDiff.var("x", DataType.FLOAT, shape)
			Dim result As SDVariable = sameDiff.size(x)

			Dim err As String = OpValidation.validate((New TestCase(sameDiff)).gradientCheck(False).expected(result, Nd4j.scalar(6L)))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiagShapeFn(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDiagShapeFn(ByVal backend As Nd4jBackend)
			Dim i As INDArray = Nd4j.linspace(1, 16, 16).reshape(ChrW(4), 4)

			Dim op As New OpTestCase(New DiagPart(i, Nothing))

			Dim exp As INDArray = Nd4j.create(New Double(){1, 6, 11, 16}, New Long(){4})
			op.expectedOutput(0, exp)

			Dim err As String = OpValidation.validate(op)
			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1, 60, 60).reshape(ChrW(3), 4, 5)
			Dim exp As INDArray = [in].permute(0,1,2) 'No op

			assertEquals([in], exp)

			Dim [out] As INDArray = Nd4j.create(3,4,5)
			Dim op As New OpTestCase(New Permute([in],[out],0,1,2))
			op.expectedOutput(0, exp)

			assertNull(OpValidation.validate(op))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute2(ByVal backend As Nd4jBackend)
			For Each perm As Integer() In New Integer()(){
				New Integer() {0, 1, 2},
				New Integer() {0, 2, 1},
				New Integer() {1, 0, 2},
				New Integer() {1, 2, 0},
				New Integer() {2, 0, 1},
				New Integer() {2, 1, 0}
			}
				Dim [in] As INDArray = Nd4j.linspace(1, 60, 60).reshape(ChrW(3), 4, 5)
				Dim exp As INDArray = [in].permute(perm).dup("c"c)

				Dim outShape(2) As Integer
				For i As Integer = 0 To 2
					outShape(i) = CInt([in].size(perm(i)))
				Next i

				'System.out.println(Arrays.toString(outShape) + " - permute " + Arrays.toString(perm));
				Dim [out] As INDArray = Nd4j.create(outShape)
				Dim op As New OpTestCase(New Permute([in], [out], perm))
				op.expectedOutput(0, exp)

				assertNull(OpValidation.validate(op))
			Next perm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConstant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConstant(ByVal backend As Nd4jBackend)

			'Case 0: no shape
			Dim sd As SameDiff = SameDiff.create()
			Dim ia As INDArray = Nd4j.create(New Double(){1, 2, 3})
			Dim [in] As SDVariable = sd.var(ia)
			Dim loss As SDVariable = [in].std(True)

			assertNull(OpValidation.validate((New TestCase(sd)).expected([in], ia)))

			'Case 1: shape is provided + scalar

			sd = SameDiff.create()
			ia = Nd4j.scalar(3.0)
			[in] = sd.var(ia)
			Dim constant As SDVariable = sd.constant(Nd4j.create(DataType.FLOAT, 3,4,5))
			Dim exp As INDArray = Nd4j.valueArrayOf(New Long(){3, 4, 5}, 3.0)
			loss = constant.std(True)

			assertNull(OpValidation.validate((New TestCase(sd)).gradientCheck(False).expected(constant, Nd4j.create(DataType.FLOAT, 3,4,5))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnstackEdgeCase2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnstackEdgeCase2(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 2

				Dim arr As INDArray = Nd4j.rand(New Long(){1, 1, 1})

				Dim shapes As val = Nd4j.Executioner.calculateOutputShape(New Unstack(arr, Nothing, i))

				assertEquals(1, shapes.size())
				assertArrayEquals(New Long(){1, 1}, shapes.get(0).getShape())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void invertPermutation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub invertPermutation(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim ia As INDArray = Nd4j.create(New Single() {3, 4, 0, 2, 1}).castTo(DataType.INT)
			Dim expOut As INDArray = Nd4j.create(New Single() {2, 4, 3, 0, 1}).castTo(DataType.INT)

			Dim input As SDVariable = sd.var("in", DataType.INT, 1, 5)
			sd.associateArrayWithVariable(ia, input)
			Dim [out] As SDVariable = sd.invertPermutation(input)

			assertNull(OpValidation.validate((New TestCase(sd)).gradientCheck(False).expected([out], expOut)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGatherNd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherNd(ByVal backend As Nd4jBackend)

'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As IList(Of INDArray) = New List(Of INDArray)()
			Dim params As IList(Of INDArray) = New List(Of INDArray)()
			Dim expected As IList(Of INDArray) = New List(Of INDArray)()


			indices_Conflict.Add(Nd4j.create(New Double()(){
				New Double() {0, 0},
				New Double() {1, 1}
			}).castTo(DataType.INT))
			params.Add(Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 4}
			}))
			expected.Add(Nd4j.create(New Double(){1, 4}))

			indices_Conflict.Add(Nd4j.create(New Double()(){
				New Double() {1},
				New Double() {0}
			}).castTo(DataType.INT))
			params.Add(Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 4}
			}))
			expected.Add(Nd4j.create(New Double()(){
				New Double() {3, 4},
				New Double() {1, 2}
			}))

			indices_Conflict.Add(Nd4j.create(New Double()(){
				New Double() {0, 1},
				New Double() {1, 0}
			}).castTo(DataType.INT))
			params.Add(Nd4j.create(New Double()()(){
				New Double()() {
					New Double() {10, 20},
					New Double() {30, 40}
				},
				New Double()() {
					New Double() {11, 21},
					New Double() {31, 41}
				}
			}))
			expected.Add(Nd4j.create(New Double()(){
				New Double() {30, 40},
				New Double() {11, 21}
			}))

			For i As Integer = 0 To indices_Conflict.Count - 1
				Dim sd As SameDiff = SameDiff.create()
				Dim p As SDVariable = sd.var("p", params(i))
				Dim ind As SDVariable = sd.constant("i", indices(i))
				Dim g As SDVariable = sd.gatherNd(p, ind)

				Dim exp As INDArray = expected(i)
				'INDArray act = sd.execAndEndResult();

				Dim err As String = OpValidation.validate((New TestCase(sd)).expected(g, exp).gradientCheck(False)) 'Grad not implemented
				assertNull(err)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseSequence(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseSequence(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim input_data() As Single = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
			Dim expected_output() As Single = { 7, 8, 9, 4, 5, 6, 1, 2, 3, 0, 0, 0, 0, 0, 0, 4, 5, 6, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
			Dim arr1 As INDArray = Nd4j.create(input_data, New Long(){2, 5, 3}).castTo(DataType.DOUBLE)
			Dim seqLenArr As INDArray = Nd4j.createFromArray(3, 2)
			Dim x As SDVariable = sameDiff.constant("x", arr1)
			Dim seq_lengths As SDVariable = sameDiff.constant("seq_lengths", seqLenArr)
			Dim result As SDVariable = sameDiff.reverseSequence(x, seq_lengths, 1, 0)
			Dim expected As INDArray = Nd4j.create(expected_output, New Long(){2, 5, 3}).castTo(DataType.DOUBLE)
			assertArrayEquals(arr1.shape(), result.eval().shape())
			assertEquals(expected, result.eval())

			Dim loss As SDVariable = sameDiff.standardDeviation(result, True)
			Dim err As String = OpValidation.validate((New TestCase(sameDiff)).expected(result.name(), expected).gradientCheck(False))
			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("MatrixDeterminant does not have a gradient yet.") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testMatrixDeterminant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixDeterminant(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(3,3)

			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", [in])
			Dim md As SDVariable = sd.math().matrixDeterminant(var)

			Dim d As Double = (New LUDecomposition(CheckUtil.convertToApacheMatrix([in]))).getDeterminant()


			Dim outExp As INDArray = Nd4j.scalar(d)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(md.name(), outExp))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("MatrixDeterminant does not have a gradient yet.") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testDeterminant22(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeterminant22(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2.5},
				New Double() {3.5, 4.5}
			})


			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", [in])
			Dim md As SDVariable = sd.math().matrixDeterminant(var)

			Dim d As Double = (New LUDecomposition(CheckUtil.convertToApacheMatrix([in]))).getDeterminant()
			Dim d2 As Double = [in].getDouble(0,0) * [in].getDouble(1,1) - [in].getDouble(1,0) * [in].getDouble(0,1)
			assertEquals(d, d2, 1e-5)


			Dim outExp As INDArray = Nd4j.scalar(d)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(md.name(), outExp))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("MatrixDeterminant does not have a gradient yet.") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testMatrixDeterminant3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixDeterminant3(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(3,3)
			'System.out.println(in.shapeInfoToString());   //Rank: 2,Offset: 0 Order: c Shape: [3,3],  stride: [3,1]
			'System.out.println(Arrays.toString(in.data().asFloat())); //[0.27620894, 0.21801452, 0.062078513, 7.348895E-4, 0.24149609, 0.4948205, 0.93483436, 0.52035654, 0.30292067]

			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", [in])
			Dim md As SDVariable = sd.math().matrixDeterminant(var)

			Dim d As Double = (New LUDecomposition(CheckUtil.convertToApacheMatrix([in]))).getDeterminant()

			'https://en.wikipedia.org/wiki/Determinant
			Dim a()() As Double = [in].toDoubleMatrix()
			Dim d2 As Double = a(0)(0) * a(1)(1) * a(2)(2) + a(0)(1) * a(1)(2) * a(2)(0) + a(0)(2) * a(1)(0) * a(2)(1) - a(0)(2) * a(1)(1) * a(2)(0) - a(0)(1) * a(1)(0) * a(2)(2) - a(0)(0) * a(1)(2) * a(2)(1)
			assertEquals(d, d2, 1e-6) 'Manual calc and Apache commons both match:    0.03589524995561552

			Dim outExp As INDArray = Nd4j.scalar(d)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(md.name(), outExp))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("MatrixDeterminant does not have a gradient yet.") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testMatrixDeterminant4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixDeterminant4(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(4,4)
			'System.out.println(in.shapeInfoToString());   //Rank: 2,Offset: 0 Order: c Shape: [4,4],  stride: [4,1]
			'System.out.println(Arrays.toString(in.data().asFloat())); //[0.27620894, 0.21801452, 0.062078513, 7.348895E-4, 0.24149609, 0.4948205, 0.93483436, 0.52035654, 0.30292067, 0.3289706, 0.7977864, 0.03180518, 0.1455722, 0.90352905, 0.9405744, 0.0048329555]

			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("in", [in])
			Dim md As SDVariable = sd.math().matrixDeterminant(var)

			Dim d As Double = (New LUDecomposition(CheckUtil.convertToApacheMatrix([in]))).getDeterminant() '-0.06713878100086641
			'System.out.println(d);

			Dim err As String = OpValidation.validate((New TestCase(sd)).expected(md.name(), Nd4j.scalar(d)))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentOps(ByVal backend As Nd4jBackend)
			'https://github.com/eclipse/deeplearning4j/issues/6952
			Dim s As INDArray = Nd4j.create(New Double(){0, 0, 0, 1, 2, 2, 3, 3}, New Long(){8}).castTo(DataType.INT)
			Dim d As INDArray = Nd4j.create(New Double(){5, 1, 7, 2, 3, 4, 1, 3}, New Long(){8})
			Dim numSegments As Integer = 4

			Dim failed As IList(Of String) = New List(Of String)()

			For Each op As String In New String(){"max", "min", "mean", "prod", "sum", "umax", "umin", "umean", "uprod", "usum", "usqrtn"}
				log.info("Starting test: {}", op)

				If op.StartsWith("u", StringComparison.Ordinal) Then
					'Unsorted segment cases
					s = Nd4j.create(New Double(){3, 1, 0, 0, 2, 0, 3, 2}, New Long(){8}).castTo(DataType.INT)
					d = Nd4j.create(New Double(){1, 2, 5, 7, 3, 1, 3, 4}, New Long(){8})
				End If

				Dim sd As SameDiff = SameDiff.create()
				Dim data As SDVariable = sd.var("data", d)
				Dim segments As SDVariable = sd.constant("segments", s)

				Dim sm As SDVariable
				Dim exp As INDArray
				Select Case op
					Case "max"
						sm = sd.segmentMax(data, segments)
						exp = Nd4j.create(New Double(){7, 2, 4, 3})
					Case "min"
						sm = sd.segmentMin(data, segments)
						exp = Nd4j.create(New Double(){1, 2, 3, 1})
					Case "mean"
						sm = sd.segmentMean(data, segments)
						exp = Nd4j.create(New Double(){4.3333333333, 2, 3.5, 2})
					Case "prod"
						sm = sd.segmentProd(data, segments)
						exp = Nd4j.create(New Double(){35, 2, 12, 3})
					Case "sum"
						sm = sd.segmentSum(data, segments)
						exp = Nd4j.create(New Double(){13, 2, 7, 4})
					Case "umax"
						sm = sd.unsortedSegmentMax(data, segments, numSegments)
						exp = Nd4j.create(New Double(){7, 2, 4, 3})
					Case "umin"
						sm = sd.unsortedSegmentMin(data, segments, numSegments)
						exp = Nd4j.create(New Double(){1, 2, 3, 1})
					Case "umean"
						sm = sd.unsortedSegmentMean(data, segments, numSegments)
						exp = Nd4j.create(New Double(){4.3333333333, 2, 3.5, 2})
					Case "uprod"
						sm = sd.unsortedSegmentProd(data, segments, numSegments)
						exp = Nd4j.create(New Double(){35, 2, 12, 3})
					Case "usum"
						sm = sd.unsortedSegmentSum(data, segments, numSegments)
						exp = Nd4j.create(New Double(){13, 2, 7, 4})
					Case "usqrtn"
						sm = sd.unsortedSegmentSqrtN(data, segments, numSegments)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						exp = Nd4j.create(New Double(){(5+7+1)/Math.Sqrt(3), 2, (3+4)/Math.Sqrt(2), (1+3)/Math.Sqrt(2)})
					Case Else
						Throw New Exception()
				End Select

				Dim loss As SDVariable = sm.std(True)
				sd.addLossVariable(loss)

				Dim tc As TestCase = (New TestCase(sd)).testName(op).expected(sm, exp).gradientCheck(True).gradCheckSkipVariables(segments.name())

				Dim err As String = OpValidation.validate(tc)
				If err IsNot Nothing Then
					failed.Add(err)
				End If
			Next op

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentMean(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(DataType.FLOAT, 1, 18, 1).reshape(ChrW(6), 3)
			Dim segmentIds As INDArray = Nd4j.createFromArray(0, 0, 1, 1, 2, 2)

			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 3, 3)

			Nd4j.exec(DynamicCustomOp.builder("segment_mean").addInputs(x, segmentIds).addOutputs([out]).build())

			Dim exp As INDArray = [out].like()
			exp.putRow(0, x.getRow(0).add(x.getRow(1)).muli(0.5))
			exp.putRow(1, x.getRow(2).add(x.getRow(3)).muli(0.5))
			exp.putRow(2, x.getRow(4).add(x.getRow(5)).muli(0.5))

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSequenceMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSequenceMask(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.createFromArray(New Integer() {1, 3, 2})
			' arr is not trainable, so it's constant in model
			Dim lengths As SDVariable = sameDiff.constant(arr)

			' Test with static max len
			Dim maxlen As Integer = 5
			Dim expected As INDArray = Nd4j.create(New Single() { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f }).reshape(ChrW(3), 5)
			Dim ret() As INDArray = Nd4j.exec(New SequenceMask(arr, maxlen, DataType.FLOAT))
			Dim result1 As SDVariable = sameDiff.sequenceMask(lengths, maxlen, DataType.FLOAT)
			assertArrayEquals(expected.shape(), result1.eval().shape())
			assertEquals(expected, result1.eval())

			Dim loss As SDVariable = sameDiff.standardDeviation(result1, True)

			Dim err As String = OpValidation.validate((New TestCase(sameDiff)).expected(result1, expected).gradientCheck(False))
			assertNull(err)

			' Test with dynamic maxlen
			lengths = sameDiff.constant("lengths2", arr)
'JAVA TO VB CONVERTER NOTE: The variable maxLen was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim maxLen_Conflict As SDVariable = sameDiff.constant("maxLen", Nd4j.scalar(5))
			Dim result2 As SDVariable = sameDiff.sequenceMask(lengths, maxLen_Conflict, DataType.FLOAT)
	'        assertArrayEquals(expected.shape(), result2.eval().shape());
			assertEquals(expected, result2.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeshGrid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeshGrid(ByVal backend As Nd4jBackend)
			Dim failed As IList(Of String) = New List(Of String)()

			For rank As Integer = 2 To 4
				Dim sd As SameDiff = SameDiff.create()

				Dim arr(rank - 1) As SDVariable
				Dim names(rank - 1) As String
				For i As Integer = 0 To rank - 1
					Dim [in] As INDArray = Nd4j.linspace(1,3+i, 3+i).reshape(ChrW(3+i)).castTo(DataType.DOUBLE)
					arr(i) = sd.var("in" & i, [in])
					names(i) = "meshgrid-" & i
				Next i
				Dim meshgrid() As SDVariable = sd.math().meshgrid(names, arr, False)

				Dim tc As New TestCase(sd)

				Dim shape() As Long
				If rank = 2 Then
					shape = New Long(){3, 4}
				ElseIf rank = 3 Then
					shape = New Long(){3, 4, 5}
				Else
					shape = New Long(){3, 4, 5, 6}
				End If
				Dim exp(shape.Length - 1) As INDArray 'Nd4j.create(shape);
				For i As Integer = 0 To exp.Length - 1
					exp(i) = Nd4j.create(DataType.DOUBLE, shape)
					Dim nTensors As Long = exp(i).tensorsAlongDimension(i)
					For j As Long = 0 To nTensors - 1
						Dim tad As INDArray = exp(i).tensorAlongDimension(CInt(j), i)
						tad.assign(arr(i).Arr)
					Next j

					tc.expected(meshgrid(i), exp(i))
				Next i

				Dim loss As SDVariable = Nothing
				For i As Integer = 0 To rank - 1
					If i = 0 Then
						loss = meshgrid(i).std(True)
					Else
						loss = loss.add("loss-" & i, meshgrid(i).std(True))
					End If
				Next i

				Dim err As String = OpValidation.validate(tc, True)
				If err IsNot Nothing Then
					failed.Add(err)
				End If
			Next rank

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGather(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGather(ByVal backend As Nd4jBackend)
			Dim inArrs As IList(Of INDArray) = New List(Of INDArray)()
			Dim axis As IList(Of Integer) = New List(Of Integer)()
'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As IList(Of INDArray) = New List(Of INDArray)()

			inArrs.Add(Nd4j.linspace(1,48,48).reshape(ChrW(2), 4, 3, 2))
			indices_Conflict.Add(Nd4j.create(New Double(){1, 0}).castTo(DataType.INT))
			axis.Add(-2)

			For i As Integer = 0 To inArrs.Count - 1

				Dim [in] As INDArray = inArrs(i)
				Dim idx As INDArray = indices(i)
				Dim a As Integer = axis(i)
				Dim aNorm As Integer = (If(a >= 0, a, a + [in].rank()))

				Dim expOut As INDArray
				If idx.rank() = 0 Then
					Dim get([in].rank() - 1) As INDArrayIndex
					For j As Integer = 0 To aNorm - 1
						get(j) = all()
					Next j
					get(aNorm) = point(idx.getInt(0))
					Dim j As Integer=aNorm+1
					Do While j<[in].rank()
						get(j) = all()
						j += 1
					Loop
					expOut = [in].get(get)
				ElseIf idx.rank() = 1 Then
					Dim shape() As Long = CType([in].shape().Clone(), Long())
					shape(aNorm) = idx.length()
					expOut = Nd4j.create(shape)

					Dim get([in].rank() - 1) As INDArrayIndex
					Dim put([in].rank() - 1) As INDArrayIndex
					For j As Integer = 0 To aNorm - 1
						get(j) = all()
						put(j) = all()
					Next j
					Dim j As Integer=aNorm+1
					Do While j<[in].rank()
						get(j) = all()
						put(j) = all()
						j += 1
					Loop

					For j As Integer = 0 To idx.length() - 1
						get(aNorm) = point(idx.getInt(j))
						put(aNorm) = point(j)
						expOut.put(put, [in].get(get))
					Next j
				Else
					Throw New Exception("Rank 2+ tests not yet implemented")
				End If


				Dim sd As SameDiff = SameDiff.create()
				Dim sdIn As SDVariable = sd.var("in", [in])
				Dim sdIdx As SDVariable = sd.constant("idx", idx)
				Dim gather As SDVariable = sd.gather(sdIn, sdIdx, a)

				Dim loss As SDVariable = gather.std(True)

				Dim err As String = OpValidation.validate((New TestCase(sd)).expected(gather, expOut).gradCheckSkipVariables("idx"))

				assertNull(err)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGatherSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherSimple(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.create(New Single(){1, 2, 3, 4}, New Long(){2, 2})
			Dim x As SDVariable = sameDiff.var("x", arr)
			Dim result As SDVariable = sameDiff.gather(x, New Integer(){1, 0}, 1)
			Dim expected As INDArray = Nd4j.create(New Single(){2, 1, 4, 3}, New Long(){2, 2})
			assertEquals(expected, result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGatherNdSingle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherNdSingle(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr1 As INDArray = Transforms.sigmoid(Nd4j.linspace(DataType.DOUBLE, 1, 24, 24)).reshape(ChrW(2), 3, 4)
			Dim arr2 As INDArray = Nd4j.create(New Single(){1, 2, 3, 0, 1, 3, 1, 0, 2}, New Long(){3, 3}).castTo(DataType.INT)
			Dim x As SDVariable = sameDiff.var("x", arr1)
			Dim idxs As SDVariable = sameDiff.constant("idxs", arr2)
			Dim result As SDVariable = sameDiff.gatherNd(x, idxs)
			' build expected output array
			Dim expected As INDArray = Nd4j.zeros(3)
			For i As Integer = 0 To 2
				Dim idx As INDArray = arr2.get(point(i), all())
				expected.putScalar(i, arr1.get(point(idx.getInt(0)), point(idx.getInt(1)), point(idx.getInt(2))).getDouble(0))
			Next i
			assertEquals(expected, result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStack2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStack2(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr1 As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 6, 6)).reshape(ChrW(3), 2)
			Dim arr2 As INDArray = Transforms.sigmoid(Nd4j.linspace(7, 12, 6)).reshape(ChrW(3), 2)
			Dim x1 As SDVariable = sameDiff.var("x1", arr1)
			Dim x2 As SDVariable = sameDiff.var("x2", arr2)
			Dim result As SDVariable = sameDiff.stack(1, x1, x2)
			assertArrayEquals(New Long(){3, 2, 2}, result.eval().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testParallelStack(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testParallelStack(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr1 As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 6, 6)).reshape(ChrW(3), 2)
			Dim arr2 As INDArray = Transforms.sigmoid(Nd4j.linspace(7, 12, 6)).reshape(ChrW(3), 2)
			Dim x1 As SDVariable = sameDiff.var("x1", arr1)
			Dim x2 As SDVariable = sameDiff.var("x2", arr2)
			Dim result As SDVariable = sameDiff.stack(0, New SDVariable(){x1, x2})
			assertArrayEquals(New Long(){2, 3, 2}, result.eval().shape())
			assertEquals(Nd4j.concat(0, arr1, arr2).reshape(2, 3, 2), result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnStack2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnStack2(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr1 As INDArray = Nd4j.zeros(3, 2)
			Dim arr2 As INDArray = Nd4j.ones(3, 2)
			Dim x1 As SDVariable = sameDiff.var("x1", arr1)
			Dim x2 As SDVariable = sameDiff.var("x2", arr2)
			Dim stacked As SDVariable = sameDiff.stack(0, x1, x2)
			Dim result() As SDVariable = sameDiff.unstack(stacked, 0, 2)
			assertEquals(arr1, result(0).eval())
			assertEquals(arr2, result(1).eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermuteSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermuteSimple(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 6, 6).reshape(ChrW(2), 3))
			Dim x As SDVariable = sameDiff.var("x", arr)
			Dim result As SDVariable = sameDiff.permute(x, 1, 0)
			Dim m As IDictionary(Of String, INDArray) = sameDiff.outputAll(Nothing)
			assertArrayEquals(New Long(){3, 2}, m(result.name()).shape())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat2(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr1 As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 4, 4)).reshape(ChrW(1), 4)
			Dim arr2 As INDArray = Transforms.sigmoid(Nd4j.linspace(4, 8, 4)).reshape(ChrW(1), 4)
			Dim x1 As SDVariable = sameDiff.var("x1", arr1)
			Dim x2 As SDVariable = sameDiff.var("x2", arr2)
			Dim result As SDVariable = sameDiff.concat(0, x1, x2)
			assertArrayEquals(New Long(){2, 4}, result.eval().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTile2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTile2(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(1), 4))
			Dim x As SDVariable = sameDiff.var("x", arr)
			Dim result As SDVariable = sameDiff.tile(x, New Integer(){2, 2})
			assertArrayEquals(New Long(){2, 8}, result.eval().shape())
			Dim arr2 As INDArray = Nd4j.concat(0, arr, arr) ' (1, 4), (1, 4) -> (2, 4)
			Dim expected As INDArray = Nd4j.concat(1, arr2, arr2) ' (2, 4), (2, 4) -> (2, 8)
			assertEquals(expected, result.eval())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlice2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlice2d(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 12, 12).reshape("c"c, 3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice_full As SDVariable = sd.slice([in], New Integer(){0, 0}, New Integer(){3, 4})
			Dim subPart As SDVariable = sd.slice([in], New Integer(){1, 2}, New Integer(){2, 2})

			Dim m As IDictionary(Of String, INDArray) = sd.outputAll(java.util.Collections.emptyMap())

			assertEquals(inArr, m(slice_full.name()))
			assertEquals(inArr.get(interval(1, 3), interval(2, 4)), m(subPart.name()))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlice3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlice3d(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 60, 60).reshape("c"c, 3, 4, 5)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice_full As SDVariable = sd.slice([in], New Integer(){0, 0, 0}, New Integer(){3, 4, 5})
			Dim subPart As SDVariable = sd.slice([in], New Integer(){1, 2, 3}, New Integer(){2, 2, 1})

			Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)

			assertEquals(inArr, m(slice_full.name()))
			assertEquals(inArr.get(interval(1, 3), interval(2, 4), interval(3, 4)), m(subPart.name()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSlice2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSlice2dBasic(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 12, 12).reshape("c"c, 3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice_full As SDVariable = sd.stridedSlice([in],New Long(){0, 0},New Long(){3, 4},New Long(){1, 1})
			Dim subPart As SDVariable = sd.stridedSlice([in],New Long(){1, 2},New Long(){3, 4},New Long(){1, 1})
			' SDVariable subPart2 = sd.stridedSlice(in,new long[]{0, 0},new long[]{4, 5},new long[]{2, 2});

			sd.outputAll(Nothing)

			assertEquals(inArr, slice_full.Arr)
			assertEquals(inArr.get(interval(1, 3), interval(2, 4)), subPart.Arr)
			' assertEquals(inArr.get(interval(0, 2, 4), interval(0, 2, 5)), subPart2.getArr());
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceBeginEndMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceBeginEndMask(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 12, 12).reshape("c"c, 3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice1 As SDVariable = sd.stridedSlice([in],New Long(){-999, 0},New Long(){2, 4},New Long(){1, 1}, 1 << 1, 0, 0, 0, 0)
			Dim slice2 As SDVariable = sd.stridedSlice([in],New Long(){1, 0},New Long(){-999, 4},New Long(){1, 1}, 0, 1, 0, 0, 0)

			sd.outputAll(Nothing)

			assertEquals(inArr.get(interval(0, 2), all()), slice1.Arr)
			assertEquals(inArr.get(interval(1, 3), all()), slice2.Arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceEllipsisMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceEllipsisMask(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 60, 60).reshape("c"c, 3, 4, 5)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)

			'[1:3,...] -> [1:3,:,:]
			Dim slice As SDVariable = sd.stridedSlice([in],New Long(){1},New Long(){3},New Long(){1}, 0, 0, 1 << 1, 0, 0)
			'[1:3,...,1:4] -> [1:3,:,1:4]
			Dim slice2 As SDVariable = sd.stridedSlice([in],New Long(){1, 1},New Long(){3, 4},New Long(){1, 1}, 0, 0, 1 << 1, 0, 0)

			sd.outputAll(java.util.Collections.emptyMap())

			assertEquals(inArr.get(interval(1, 3), all(), all()), slice.Arr)
			assertEquals(inArr.get(interval(1, 3), all(), all()), slice2.Arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceNewAxisMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceNewAxisMask(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 60, 60).reshape("c"c, 3, 4, 5)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice As SDVariable = sd.stridedSlice([in],New Long(){-999, 0, 0, 0},New Long(){-999, 3, 4, 5},New Long(){-999, 1, 1, 1}, 0, 0, 0, 1, 0)

			Dim [out] As INDArray = slice.eval()

			assertArrayEquals(New Long(){1, 3, 4, 5}, [out].shape())
			assertEquals(inArr, [out].get(point(0), all(), all(), all()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceNewAxisMask2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceNewAxisMask2(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 60, 60).reshape("c"c, 3, 4, 5)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice As SDVariable = sd.stridedSlice([in],New Long(){1, 1, -999, 1},New Long(){3, 3, -999, 4},New Long(){1, 1, -999, 1}, 0, 0, 0, 1 << 2, 0)
			Dim [out] As INDArray = slice.eval()

			assertArrayEquals(New Long(){2, 2, 1, 3}, slice.Arr.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceShrinkAxisMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceShrinkAxisMask(ByVal backend As Nd4jBackend)

			Dim inArr As INDArray = Nd4j.linspace(1, 60, 60).reshape("c"c, 3, 4, 5)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim slice As SDVariable = sd.stridedSlice([in],New Long(){0, 0, 0},New Long(){-999, 4, 5},New Long(){1, 1, 1}, 0, 0, 0, 0, 1)
			Dim slice2 As SDVariable = sd.stridedSlice([in],New Long(){2, 0, 0},New Long(){-999, 4, 5},New Long(){1, 1, 1}, 0, 0, 0, 0, 1)
			Dim slice3 As SDVariable = sd.stridedSlice([in],New Long(){1, 2, 1},New Long(){-999, -999, 5},New Long(){1, 1, 1}, 0, 0, 0, 0, 1 Or 1 << 1)

			sd.outputAll(Nothing)

			assertEquals(inArr.get(point(0), all(), all()), slice.Arr)
			assertEquals(inArr.get(point(2), all(), all()), slice2.Arr)
			assertEquals(inArr.get(point(1), point(2), interval(1, 5)).reshape(4), slice3.Arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSizeAt_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSizeAt_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(10, 20, 30)
			Dim exp As val = Nd4j.scalar(DataType.LONG, 20)

			Dim op As val = New SizeAt(array, 1)

			Nd4j.Executioner.exec(op)

			Dim output As val = op.outputArguments().get(0)

			assertEquals(exp, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEye(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEye(ByVal backend As Nd4jBackend)
			Dim rows() As Integer = {3, 3, 3, 3}
			Dim cols() As Integer = {3, 2, 2, 2}
			Dim batch()() As Integer = {
				Nothing, Nothing, New Integer() {4},
				New Integer() {3, 3}
			}
			Dim expOut(3) As INDArray

			expOut(0) = Nd4j.eye(3)
			expOut(1) = Nd4j.create(New Double()(){
				New Double() {1, 0},
				New Double() {0, 1},
				New Double() {0, 0}
			})
			expOut(2) = Nd4j.create(4,3,2)
			For i As Integer = 0 To 3
				expOut(2).get(point(i), all(), all()).assign(expOut(1))
			Next i
			expOut(3) = Nd4j.create(3,3,3,2)
			For i As Integer = 0 To 2
				For j As Integer = 0 To 2
					expOut(3).get(point(i), point(j), all(), all()).assign(expOut(1))
				Next j
			Next i


			For i As Integer = 0 To 2
				log.info("Starting: " & i)
				Dim [out] As INDArray = Nd4j.create(expOut(i).shape())

				Dim op As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder("eye").addOutputs([out]).addIntegerArguments(rows(i), cols(i))
				If batch(i) <> Nothing Then
					op.addIntegerArguments(batch(i))
				End If

				Nd4j.Executioner.exec(op.build())

				assertEquals(expOut(i), [out])
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSplit1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSplit1(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1,10,10).reshape(ChrW(10))
			Dim axis As INDArray = Nd4j.scalar(-1)

			Dim out1 As INDArray = Nd4j.create(New Long(){5})
			Dim out2 As INDArray = Nd4j.create(New Long(){5})

			Dim exp1 As INDArray = [in].get(interval(0,5)).reshape(5)
			Dim exp2 As INDArray = [in].get(interval(5,10)).reshape(5)

			assertNull(OpValidation.validate((New OpTestCase(DynamicCustomOp.builder("split").addInputs(axis, [in]).addOutputs(out1, out2).addIntegerArguments(2).build())).expectedOutput(0, exp1).expectedOutput(1,exp2)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSplit2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSplit2(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1,24,24).reshape(ChrW(3), 8)
			Dim axis As INDArray = Nd4j.scalar(-1)

			Dim out1 As INDArray = Nd4j.create(New Long(){3, 4}, "c"c)
			Dim out2 As INDArray = Nd4j.create(New Long(){3, 4}, "c"c)

			Dim exp1 As INDArray = [in].get(all(), interval(0,4)).dup("c"c)
			Dim exp2 As INDArray = [in].get(all(), interval(4,8)).dup("c"c)

			assertNull(OpValidation.validate((New OpTestCase(DynamicCustomOp.builder("split").addInputs(axis, [in]).addOutputs(out1, out2).addIntegerArguments(2).build())).expectedOutput(0, exp1).expectedOutput(1,exp2)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistancesExec(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistancesExec(ByVal backend As Nd4jBackend)
			'https://github.com/eclipse/deeplearning4j/issues/7001
			For Each s As String In New String(){"euclidean", "manhattan", "cosinesim", "cosinedist", "jaccard"}
				log.info("Starting: {}", s)
				Dim defaultTestCase As INDArray = Nd4j.create(4, 4)
				defaultTestCase.putRow(0, Nd4j.create(New Single(){0, 2, -2, 0}))
				defaultTestCase.putRow(1, Nd4j.create(New Single(){0, 1, -1, 0}))
				defaultTestCase.putRow(2, Nd4j.create(New Single(){0, -1, 1, 0}))
				defaultTestCase.putRow(3, Nd4j.create(New Single(){0, -2, 2, 0}))
				Dim singleEmbeddingSize As Long = defaultTestCase.size(1) \ 2L

				' Split vectors
				Dim x As INDArray = defaultTestCase.get(all(), interval(0, singleEmbeddingSize))
				Dim y As INDArray = defaultTestCase.get(all(), interval(singleEmbeddingSize, defaultTestCase.size(1)))

				log.info(y.shapeInfoToString())

				Dim sd As SameDiff = SameDiff.create()
				sd.enableDebugMode()

				Dim xSd As SDVariable = sd.var("x", x)
				Dim ySd As SDVariable = sd.var("y", y)

				ySd = ySd.add(ySd)
				Dim dist As SDVariable
				Select Case s
					Case "euclidean"
						dist = sd.math().euclideanDistance(s, ySd, xSd, 0)
					Case "manhattan"
						dist = sd.math().manhattanDistance(s, ySd, xSd, 0)
					Case "cosinesim"
						dist = sd.math().cosineSimilarity(s, ySd, xSd, 0)
					Case "cosinedist"
						dist = sd.math().cosineDistance(s, ySd, xSd, 0)
					Case "jaccard"
						dist = sd.math().jaccardDistance(s, ySd, xSd, 0)
					Case Else
						Throw New Exception()
				End Select

				Dim loss As SDVariable = dist.sum()


	'            log.info(sd.summary());
				sd.output(java.util.Collections.emptyMap(), Lists.newArrayList(s))
				sd.calculateGradients(java.util.Collections.emptyMap(), sd.getVariables().keySet())
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionShape(ByVal backend As Nd4jBackend)

			Dim shape As INDArray = Nd4j.createFromArray(4,2)
			Dim axis As INDArray = Nd4j.scalar(0)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("evaluate_reduction_shape").addInputs(shape,axis).addBooleanArguments(True).build()

			Dim list As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim s() As Long = list(0).getShape()
			Dim exp() As Long = {2} '(4,2).reduce(0,keepDims=true) -> [1,2] requires output array shape [2] here

			assertArrayEquals(exp, s) 'Fails - actual shape [1]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void gatherTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub gatherTest(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 3, 4, 5},
				New Double() {6, 7, 8, 9, 10},
				New Double() {11, 12, 13, 14, 15}
			})
'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As INDArray = Nd4j.createFromArray(2)
			Dim axis As INDArray = Nd4j.scalar(0)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("gather").addInputs([in], indices_Conflict, axis).build()

			Dim shapeList As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim shape() As Long = shapeList(0).getShape()
			Dim expShape() As Long = {1, 5}
			assertArrayEquals(expShape, shape) 'Fails: actual shape: [5]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceShape(ByVal backend As Nd4jBackend)

			Dim arr As INDArray = Nd4j.arange(0, 25).reshape(ChrW(1), 5, 5).castTo(DataType.INT)
	'        System.out.println(Arrays.toString(arr.shape()));
	'        System.out.println(arr);

			Dim begin As INDArray = Nd4j.createFromArray(0, 1, 2)
			Dim size As INDArray = Nd4j.createFromArray(-1, -1, -1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("slice").addInputs(arr, begin, size).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim shape() As Long = l(0).getShape()
			Dim shapeExp() As Long = {1, 4, 3}

			assertArrayEquals(shapeExp, shape)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhereAllFalse(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWhereAllFalse(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.create(DataType.BOOL, 1917)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("Where").addInputs([in]).addOutputs(Nd4j.empty(DataType.LONG)).build()
			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Nd4j.Executioner.exec(op)
			Dim shape() As Long = l(0).getShape()
			Dim isEmpty As Boolean = l(0).isEmpty()
			assertTrue(isEmpty) 'Not empty, but should be
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGatherScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherScalar(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(100, 200, 100, DataType.FLOAT).reshape(ChrW(100))
'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As INDArray = Nd4j.scalar(0)
			Dim axis As INDArray = Nd4j.scalar(0)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("gather").addInputs([in], indices_Conflict, axis).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim shape() As Long = l(0).getShape()
			assertArrayEquals(New Long(){}, shape)

			Dim arr As INDArray = Nd4j.create(l(0))

			op.addOutputArgument(arr)

			Nd4j.exec(op)

			Dim exp As INDArray = Nd4j.scalar(DataType.FLOAT, 100)
			assertEquals(exp, arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCastEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCastEmpty(ByVal backend As Nd4jBackend)
			Dim emptyLong As INDArray = Nd4j.empty(DataType.LONG)
			Dim dtype As Integer = 9 'INT = 9 - https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/array/DataType.h
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("cast").addInputs(emptyLong).addIntegerArguments(dtype).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim shape() As Long = l(0).getShape()
			Dim isEmpty As Boolean = l(0).isEmpty()
			assertEquals(0, shape.Length)
			assertTrue(isEmpty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGatherEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherEmpty(ByVal backend As Nd4jBackend)
	'        
	'        tf.reset_default_graph()
	'        emptyInt = tf.constant([], shape=[0], dtype=tf.int32)
	'        ingather = tf.reshape(tf.range(start=0,limit=100,delta=1,dtype=tf.float32), [25,4])
	'        gather = tf.gather(params=ingather, indices=emptyInt)
	'        sess = tf.Session()
	'        out = sess.run([gather])
	'        print(out[0].shape);
	'        print(out[0]);
	'        >> (0, 4)
	'        >> []
	'         

	'        Nd4j.getExecutioner().enableVerboseMode(true);
	'        Nd4j.getExecutioner().enableDebugMode(true);

			Dim emptyInt As INDArray = Nd4j.create(DataType.INT, 0)
			Dim inGather As INDArray = Nd4j.linspace(1,100,100,DataType.FLOAT).reshape(ChrW(25), 4)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("gather").addInputs(inGather, emptyInt).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim shape() As Long = l(0).getShape()
			assertArrayEquals(New Long(){0, 4}, l(0).getShape())
			Dim isEmpty As Boolean = l(0).isEmpty()
			assertTrue(isEmpty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSplitEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSplitEmpty(ByVal backend As Nd4jBackend)
	'        
	'        tf.reset_default_graph()
	'        # Hack to create empty array
	'        input = tf.constant([False], dtype=tf.bool)
	'        empty = tf.where(condition=input)
	'        empty = tf.reshape(empty, [0,4])
	'        emptyFloat = tf.cast(empty, tf.float32)
	'        const1 = tf.constant(1, dtype=tf.int32)
	'        split = tf.split(value=emptyFloat, num_or_size_splits=4, axis=1)
	'        sess = tf.Session()
	'        out = sess.run([split])
	'        # print(out[0].shape);
	'        print(out[0]);
	'         

			Dim emptyIn As INDArray = Nd4j.empty(DataType.FLOAT).reshape(ChrW(0), 4)
			Dim axis As INDArray = Nd4j.scalar(1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("split").addInputs(axis, emptyIn).addIntegerArguments(4).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(4, l.Count)
			For i As Integer = 0 To 3
				Dim desc As val = l(i)
				assertArrayEquals(New Long(){0, 1}, desc.getShape())
				assertTrue(desc.isEmpty())
				op.addOutputArgument(Nd4j.empty(DataType.FLOAT).reshape(desc.getShape()))
			Next i

			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatEmpty(ByVal backend As Nd4jBackend)
	'        
	'        TF behaviour with concatenatioun of empty arrays:
	'        concat(empty,empty,empty) -> empty
	'        cotcat(empty,nonEmpty) -> nonEmpty, etc (i.e., empty arrays are ignored)
	'
	'        tf.reset_default_graph()
	'        input = tf.constant([False], dtype=tf.bool)
	'        emptyFloat = tf.constant([], shape=[0,1], dtype=tf.float32)
	'        var11 = tf.constant([1], dtype=tf.float32, shape=[1,1])
	'
	'        concat = tf.concat(values=[emptyFloat, emptyFloat, var11, emptyFloat], axis=0)
	'
	'        sess = tf.Session()
	'        out = sess.run([concat])
	'        print(out[0].shape)
	'        print(out[0]);
	'         

			Dim one1 As INDArray = Nd4j.create(DataType.FLOAT, 1, 1)
			Dim empty01 As INDArray = Nd4j.create(DataType.FLOAT, 0, 1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("concat").addInputs(empty01, empty01, empty01).addIntegerArguments(0).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertTrue(l(0).isEmpty())
			assertArrayEquals(New Long(){0, 1}, l(0).getShape())

			op.addOutputArgument(Nd4j.create(DataType.FLOAT, 0, 1))
			Nd4j.exec(op)


			op = DynamicCustomOp.builder("concat").addInputs(empty01, empty01, one1, empty01).addIntegerArguments(0).build()
			l = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertFalse(l(0).isEmpty())
			assertArrayEquals(New Long(){1, 1}, l(0).getShape())
			op.addOutputArgument(Nd4j.create(DataType.FLOAT, 1, 1))
			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatEmpty2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatEmpty2(ByVal backend As Nd4jBackend)
			Dim empty10a As INDArray = Nd4j.create(DataType.INT, 1, 0)
			Dim empty10b As INDArray = Nd4j.create(DataType.INT, 1, 0)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("concat").addInputs(empty10a, empty10b).addIntegerArguments(0).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertTrue(l(0).isEmpty())
			assertArrayEquals(New Long(){2, 0}, l(0).getShape())
			assertEquals(DataType.INT, l(0).dataType())

			op.addOutputArgument(Nd4j.create(DataType.INT, 2, 0))
			Nd4j.exec(op)


			op = DynamicCustomOp.builder("concat").addInputs(empty10a, empty10b).addIntegerArguments(1).build()
			l = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertTrue(l(0).isEmpty())
			assertArrayEquals(New Long(){1, 0}, l(0).getShape())
			op.addOutputArgument(Nd4j.create(DataType.INT, 1, 0))
			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyGather(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyGather(ByVal backend As Nd4jBackend)
	'        
	'        tf.reset_default_graph()
	'        inputFloat = tf.constant([], shape=[0,2,3], dtype=tf.float32)
	'        emptyInt = tf.constant([], shape=[0], dtype=tf.int32)
	'
	'        gather = tf.gather(params=inputFloat, indices=emptyInt)
	'
	'        sess = tf.Session()
	'        out = sess.run([gather])
	'        print(out[0].shape)
	'        print(out[0]);
	'
	'        > (0, 2, 3)
	'        > []
	'         
			Dim emptyFloat As INDArray = Nd4j.create(DataType.FLOAT, 0, 2, 3)
			Dim emptyInt As INDArray = Nd4j.create(DataType.INT, 0)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("gather").addInputs(emptyFloat, emptyInt).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertTrue(l(0).isEmpty())
			assertArrayEquals(New Long(){0, 2, 3}, l(0).getShape())

			Dim [out] As INDArray = Nd4j.empty(DataType.FLOAT)
			op.addOutputArgument([out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastDynamicShape1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastDynamicShape1(ByVal backend As Nd4jBackend)

			'Test case: [2,1] and [4]: expect [2,4]
			Dim [out] As INDArray = Nd4j.create(DataType.INT, 2)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("broadcast_dynamic_shape").addInputs(Nd4j.createFromArray(New Integer(){2, 1}), Nd4j.createFromArray(New Integer(){4})).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.createFromArray(New Integer(){2, 4}), [out])

			'Same thing, reversed input order (expect same output)
			op = DynamicCustomOp.builder("broadcast_dynamic_shape").addInputs(Nd4j.createFromArray(New Integer(){4}), Nd4j.createFromArray(New Integer(){2, 1})).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.createFromArray(New Integer(){2, 4}), [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastDynamicShape2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastDynamicShape2(ByVal backend As Nd4jBackend)

			'Test case: [2,1,4] and [2,2,4]: expect [2,2,4]
			Dim [out] As INDArray = Nd4j.create(DataType.INT, 3)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("broadcast_dynamic_shape").addInputs(Nd4j.createFromArray(New Integer(){2, 1, 4}), Nd4j.createFromArray(New Integer(){2, 2, 4})).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.createFromArray(New Integer(){2, 2, 4}), [out])

			'Test case: [1,1,3] and [2,4,1]: expect [2,4,3]
			[out] = Nd4j.create(DataType.INT, 3)
			op = DynamicCustomOp.builder("broadcast_dynamic_shape").addInputs(Nd4j.createFromArray(New Integer(){1, 1, 3}), Nd4j.createFromArray(New Integer(){2, 4, 1})).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.createFromArray(New Integer(){2, 4, 3}), [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceShrinkAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceShrinkAxis(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.create(DataType.DOUBLE, 3,2,2)
			Dim begin As INDArray = Nd4j.createFromArray(2)
			Dim [end] As INDArray = Nd4j.createFromArray(3) 'Should be ignored due to shrink_axis_mask
			Dim stride As INDArray = Nd4j.createFromArray(1) 'Should be ignored due to shrink_axis_mask

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("strided_slice").addInputs([in], begin, [end], stride).addIntegerArguments(0, 0, 0, 0, 1).build()

			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
			Dim shape() As Long = lsd(0).getShape()
			Dim exp() As Long = {2, 2}
			assertArrayEquals(exp, shape)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceEmpty(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.createFromArray(10) 'Integer, Length 1, rank 1, value 10   - Not used due to begin mask!
			Dim from As INDArray = Nd4j.createFromArray(0)
			Dim [to] As INDArray = Nd4j.createFromArray(0)
			Dim stride As INDArray = Nd4j.createFromArray(1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("stridedslice").addInputs([in], from, [to], stride).addIntegerArguments(1,0,0,0,0).build()

			Dim s As IList(Of LongShapeDescriptor) = Nd4j.Executioner.calculateOutputShape(op)
			assertEquals(1, s.Count)

			'Is returning shape [0], should be empty
			Dim extras As Long = s(0).getExtras()
			Dim isEmpty As Boolean = ArrayOptionsHelper.hasBitSet(extras, ArrayOptionsHelper.ATYPE_EMPTY_BIT)
			assertTrue(isEmpty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceEdgeCase(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.scalar(10).reshape(ChrW(1)) 'Int [1]
			Dim begin As INDArray = Nd4j.ones(DataType.INT, 1)
			Dim [end] As INDArray = Nd4j.zeros(DataType.INT, 1)
			Dim stride As INDArray = Nd4j.ones(DataType.INT, 1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("strided_slice").addInputs([in], begin, [end], stride).addIntegerArguments(0, 0, 1, 0, 0).addOutputs(Nd4j.empty(DataType.INT)).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertEquals(DataType.INT, l(0).dataType())
			assertTrue(l(0).isEmpty()) 'Should be empty array, is rank 0 scalar

			Nd4j.exec(op) 'Execution is OK
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptySlice1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptySlice1(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(38)
			Dim begin As INDArray = Nd4j.createFromArray(1)
			Dim size As INDArray = Nd4j.createFromArray(-1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("slice").addInputs([in], begin, size).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertTrue(l(0).isEmpty())

			Dim [out] As INDArray = Nd4j.create(DataType.INT, 0)
			op.setOutputArgument(0, [out])

			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptySlice2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptySlice2(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(38)
			Dim begin As INDArray = Nd4j.createFromArray(0)
			Dim size As INDArray = Nd4j.createFromArray(0)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("slice").addInputs([in], begin, size).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertTrue(l(0).isEmpty())

			Dim [out] As INDArray = Nd4j.create(DataType.INT, 0)
			op.setOutputArgument(0, [out])

			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFill(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFill(ByVal backend As Nd4jBackend)

			Dim shape As INDArray = Nd4j.createFromArray(0,4)
			Dim value As INDArray = Nd4j.scalar(1.0f)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("fill").addInputs(shape, value).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertArrayEquals(New Long(){0, 4}, l(0).getShape())
			assertTrue(l(0).isEmpty())

			op.setOutputArgument(0, Nd4j.create(DataType.FLOAT, 0, 4))
			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFill2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFill2(ByVal backend As Nd4jBackend)

			Dim shape As INDArray = Nd4j.createFromArray(0,4)
			Dim value As INDArray = Nd4j.scalar(1.0f)

			Dim op As DynamicCustomOp = New Fill(shape, value, Nothing)

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertTrue(l(0).isEmpty())
			assertArrayEquals(New Long(){0, 4}, l(0).getShape())

			op.setOutputArgument(0, Nd4j.create(DataType.FLOAT, 0, 4))
			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermuteShapeDynamicAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermuteShapeDynamicAxis(ByVal backend As Nd4jBackend)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("permute").addInputs(Nd4j.rand(DataType.FLOAT, 3, 4), Nd4j.createFromArray(1, 0)).build()
			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
	'        System.out.println(Arrays.toString(l.get(0).getShape()));
			assertArrayEquals(New Long(){4, 3}, l(0).getShape())

			op = DynamicCustomOp.builder("permute").addInputs(Nd4j.rand(DataType.FLOAT, 3, 4)).addIntegerArguments(1, 0).build()
			l = op.calculateOutputShape()
	'        System.out.println(Arrays.toString(l.get(0).getShape()));
			assertArrayEquals(New Long(){4, 3}, l(0).getShape())


			op = DynamicCustomOp.builder("permute").addInputs(Nd4j.rand(DataType.FLOAT, 3, 4, 5), Nd4j.createFromArray(1, 2, 0)).build()
			l = op.calculateOutputShape()
	'        System.out.println(Arrays.toString(l.get(0).getShape()));
			assertArrayEquals(New Long(){4, 5, 3}, l(0).getShape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGather2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGather2(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim input As SDVariable = sd.var("in", Nd4j.arange(6).castTo(DataType.FLOAT).reshape(ChrW(2), 3))
'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim indices_Conflict As SDVariable = sd.constant("indices", Nd4j.createFromArray(0))

			Dim gathered As SDVariable = sd.gather(input, indices_Conflict, 1)
			Dim loss As SDVariable = gathered.std(True)

			sd.output(DirectCast(Nothing, IDictionary(Of String, INDArray)), gathered.name())
			sd.setLossVariables(gathered.name())

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradCheckEpsilon(1e-3).gradCheckMaxRelativeError(1e-4))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute3(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(DataType.FLOAT, 1, 6, 1).reshape(ChrW(3), 2)
			Dim permute As INDArray = Nd4j.createFromArray(1,0)

	'        System.out.println(in);

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var([in])
			Dim v2 As SDVariable = sd.constant(permute)

			Dim [out] As SDVariable = v.permute(v2)

			Dim exp As INDArray = [in].transpose()
			Dim outArr As INDArray = [out].eval()
			assertEquals(exp, outArr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute4(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(DataType.FLOAT, 1, 6, 1).reshape(ChrW(3), 2)
			Dim permute As INDArray = Nd4j.createFromArray(1,0)

			Dim exp As INDArray = [in].transpose()

			For Each iargs As Boolean In New Boolean(){True, False}


				Dim b As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder("permute").addInputs([in]).addOutputs(Nd4j.create(DataType.FLOAT, 2, 3))

				If iargs Then
					b.addIntegerArguments(1, 0)
				Else
					b.addInputs(permute)
				End If

				Dim op As DynamicCustomOp = b.build()
				Nd4j.exec(op)

	'            System.out.println(in);
	'            System.out.println(op.outputArguments().get(0));

				assertEquals(exp, op.getOutputArgument(0))
			Next iargs
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvertPermutation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvertPermutation(ByVal backend As Nd4jBackend)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("invert_permutation").addInputs(Nd4j.createFromArray(1, 0)).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastInt1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastInt1(ByVal backend As Nd4jBackend)

			Dim [out] As INDArray = Nd4j.create(DataType.INT, 1)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("broadcast_dynamic_shape").addInputs(Nd4j.createFromArray(1), Nd4j.createFromArray(4)).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.createFromArray(4), [out])

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastInt2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastInt2(ByVal backend As Nd4jBackend)
			Dim [out] As INDArray = Nd4j.create(DataType.INT, 2)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("broadcast_dynamic_shape").addInputs(Nd4j.createFromArray(2, 2), Nd4j.createFromArray(1)).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)

			assertEquals(Nd4j.createFromArray(2, 2), [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeZeros(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeZeros(ByVal backend As Nd4jBackend)
			Dim shapes()() As Integer = {
				New Integer() {2, 0},
				New Integer() {10, 0},
				New Integer() {10, 0},
				New Integer() {2, 0, 0, 10},
				New Integer() {10, 0},
				New Integer() {0, 0, 10},
				New Integer() {0, 2, 10},
				New Integer() {1, 2, 0}
			}
			Dim reshape()() As Integer = {
				New Integer() {2, -1},
				New Integer() {2, 0, -1},
				New Integer() {5, 2, -1},
				New Integer() {2, 0, -1},
				New Integer() {-1, 2, 0},
				New Integer() {2, -1, 0},
				New Integer() {2, 0, 0, 0, -1},
				New Integer() {2, 0, -1}
			}
			Dim expected()() As Integer = {
				New Integer() {2, 0},
				New Integer() {2, 0, 5},
				New Integer() {5, 2, 0},
				New Integer() {2, 0, 10},
				New Integer() {5, 2, 0},
				New Integer() {2, 5, 0},
				New Integer() {2, 0, 0, 0, 10},
				New Integer() {2, 0, 1}
			}

			For i As Integer = 0 To shapes.Length - 1
				Console.WriteLine(i)
				Dim orig() As Long = ArrayUtil.toLongArray(shapes(i))
				Dim r() As Integer = reshape(i)
				Dim exp() As Long = ArrayUtil.toLongArray(expected(i))

				Dim sd As SameDiff = SameDiff.create()
				Dim v As SDVariable = sd.placeHolder("orig", DataType.FLOAT, orig)
				Dim rs As SDVariable = v.reshape(r)
				Dim rs2 As SDVariable = v.reshape(sd.constant(Nd4j.createFromArray(r)))

				Dim [out] As INDArray = rs.eval(Collections.singletonMap("orig", Nd4j.create(DataType.FLOAT, orig)))
				assertArrayEquals(exp, [out].shape())

				[out] = rs2.eval(Collections.singletonMap("orig", Nd4j.create(DataType.FLOAT, orig)))
				assertArrayEquals(exp, [out].shape())
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMaxIndex(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMaxIndex(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim inputX As SDVariable = sd.var(Nd4j.createFromArray(New Single() {1, 0, 0}))
			Dim inputY As SDVariable = sd.var(Nd4j.createFromArray(New Single() {0, 1, 0}))
			Dim inputZ As SDVariable = sd.var(Nd4j.createFromArray(New Single() {0, 0, 1}))
			Dim [out] As SDVariable = (New MergeMaxIndex(sd, New SDVariable(){inputX, inputY, inputZ},DataType.INT32)).outputVariable()
			Dim expected As INDArray = Nd4j.createFromArray(0,1,2)
			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("mergemaxindex", expected).gradientCheck(False))
			assertNull(err)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTriOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTriOp(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim [out] As SDVariable = (New Tri(sd, DataType.INT32, 3, 5, 2)).outputVariable()
			Dim expected As INDArray = Nd4j.createFromArray(New Integer()(){
				New Integer() {1, 1, 1, 0, 0},
				New Integer() {1, 1, 1, 1, 0},
				New Integer() {1, 1, 1, 1, 1}
			})
			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("tri", expected).gradientCheck(False))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTriuOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTriuOp(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim input As SDVariable = sd.var(Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 3},
				New Double() {4, 5, 6},
				New Double() {7, 8, 9},
				New Double() {10, 11, 12}
			}))
			Dim [out] As SDVariable = (New Triu(sd, input,-1)).outputVariable()
			[out].markAsLoss()
			Dim expected As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 3},
				New Double() {4, 5, 6},
				New Double() {0, 8, 9},
				New Double() {0, 0, 12}
			})
			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("triu", expected).gradientCheck(True))
			assertNull(err)

		End Sub
	End Class

End Namespace