Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports var = lombok.var
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports Percentile = org.apache.commons.math3.stat.descriptive.rank.Percentile
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports Isolated = org.junit.jupiter.api.parallel.Isolated
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.nd4j.common.primitives
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports MathUtils = org.nd4j.common.util.MathUtils
Imports WeightsFormat = org.nd4j.enums.WeightsFormat
Imports Level1 = org.nd4j.linalg.api.blas.Level1
Imports GemmParams = org.nd4j.linalg.api.blas.params.GemmParams
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4jEnvironment = org.nd4j.linalg.api.environment.Nd4jEnvironment
Imports INDArrayIterator = org.nd4j.linalg.api.iter.INDArrayIterator
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastOp = org.nd4j.linalg.api.ops.BroadcastOp
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports BroadcastAMax = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAMax
Imports BroadcastAMin = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAMin
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMax = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMax
Imports BroadcastMin = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMin
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports BroadcastEqualTo = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastEqualTo
Imports BroadcastGreaterThan = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastGreaterThan
Imports BroadcastGreaterThanOrEqual = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastGreaterThanOrEqual
Imports BroadcastLessThan = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastLessThan
Imports ArgAmax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgAmax
Imports ArgAmin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgAmin
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports ArgMin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin
Imports Conv2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D
Imports Im2col = org.nd4j.linalg.api.ops.impl.layers.convolution.Im2col
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Mmul = org.nd4j.linalg.api.ops.impl.reduce.Mmul
Imports All = org.nd4j.linalg.api.ops.impl.reduce.bool.All
Imports LogSumExp = org.nd4j.linalg.api.ops.impl.reduce.custom.LogSumExp
Imports Norm1 = org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1
Imports Norm2 = org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2
Imports Sum = org.nd4j.linalg.api.ops.impl.reduce.same.Sum
Imports CosineDistance = org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance
Imports CosineSimilarity = org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity
Imports EuclideanDistance = org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance
Imports HammingDistance = org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports LeakyReLU = org.nd4j.linalg.api.ops.impl.scalar.LeakyReLU
Imports ReplaceNans = org.nd4j.linalg.api.ops.impl.scalar.ReplaceNans
Imports ScalarEquals = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarEquals
Imports ScatterUpdate = org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate
Imports Reshape = org.nd4j.linalg.api.ops.impl.shape.Reshape
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports MatchConditionTransform = org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform
Imports CompareAndSet = org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndSet
Imports Eps = org.nd4j.linalg.api.ops.impl.transforms.comparison.Eps
Imports BatchToSpaceND = org.nd4j.linalg.api.ops.impl.transforms.custom.BatchToSpaceND
Imports Reverse = org.nd4j.linalg.api.ops.impl.transforms.custom.Reverse
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports BinaryRelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryRelativeError
Imports [Set] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.Set
Imports Axpy = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.Axpy
Imports Sign = org.nd4j.linalg.api.ops.impl.transforms.same.Sign
Imports ACosh = org.nd4j.linalg.api.ops.impl.transforms.strict.ACosh
Imports Tanh = org.nd4j.linalg.api.ops.impl.transforms.strict.Tanh
Imports PrintVariable = org.nd4j.linalg.api.ops.util.PrintVariable
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports SpecifiedIndex = org.nd4j.linalg.indexing.SpecifiedIndex
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
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

Namespace org.nd4j.linalg


	''' <summary>
	''' NDArrayTests
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) public class Nd4jTestsC extends BaseNd4jTestWithBackends
	Public Class Nd4jTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub before()
			Nd4j.Random.setSeed(123)
			Nd4j.Executioner.enableDebugMode(False)
			Nd4j.Executioner.enableVerboseMode(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub after()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArangeNegative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArangeNegative(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.arange(-2,2).castTo(DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Double(){-2, -1, 0, 1})
			assertEquals(assertion,arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTri(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTri(ByVal backend As Nd4jBackend)
			Dim assertion As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 1, 1, 0, 0},
				New Double() {1, 1, 1, 1, 0},
				New Double() {1, 1, 1, 1, 1}
			})

			Dim tri As INDArray = Nd4j.tri(3,5,2)
			assertEquals(assertion,tri)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTriu(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTriu(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.linspace(1,12,12, DataType.DOUBLE).reshape(ChrW(4), 3)
			Dim k As Integer = -1
			Dim test As INDArray = Nd4j.triu(input,k)
			Dim create As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2, 3},
				New Double() {4, 5, 6},
				New Double() {0, 8, 9},
				New Double() {0, 0, 12}
			})

			assertEquals(test,create)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiag(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDiag(ByVal backend As Nd4jBackend)
			Dim diag As INDArray = Nd4j.diag(Nd4j.linspace(1,4,4, DataType.DOUBLE).reshape(ChrW(4), 1))
			assertArrayEquals(New Long() {4, 4},diag.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRowEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRowEdgeCase(ByVal backend As Nd4jBackend)
			Dim orig As INDArray = Nd4j.linspace(1,300,300, DataType.DOUBLE).reshape("c"c, 100, 3)
			Dim col As INDArray = orig.getColumn(0).reshape(ChrW(100), 1)

			For i As Integer = 0 To 99
				Dim row As INDArray = col.getRow(i)
				Dim rowDup As INDArray = row.dup()
				Dim d As Double = orig.getDouble(i, 0)
				Dim d2 As Double = col.getDouble(i)
				Dim dRowDup As Double = rowDup.getDouble(0)
				Dim dRow As Double = row.getDouble(0)

				Dim s As String = i.ToString()
				assertEquals(d, d2, 0.0,s)
				assertEquals(d, dRowDup, 0.0,s) 'Fails
				assertEquals(d, dRow, 0.0,s) 'Fails
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNd4jEnvironment(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNd4jEnvironment(ByVal backend As Nd4jBackend)
			Console.WriteLine(Nd4j.Executioner.EnvironmentInformation)
			Dim manualNumCores As Integer = Integer.Parse(Nd4j.Executioner.EnvironmentInformation.get(Nd4jEnvironment.CPU_CORES_KEY).ToString())
			assertEquals(Runtime.getRuntime().availableProcessors(), manualNumCores)
			assertEquals(Runtime.getRuntime().availableProcessors(), Nd4jEnvironment.Environment.getNumCores())
			Console.WriteLine(Nd4jEnvironment.Environment)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerialization(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.rand(1, 20).castTo(DataType.DOUBLE)

			Dim dir As File = testDir.resolve("new-dir-" & System.Guid.randomUUID().ToString()).toFile()
			assertTrue(dir.mkdirs())

			Dim outPath As String = FilenameUtils.concat(dir.getAbsolutePath(), "dl4jtestserialization.bin")

			Using dos As New java.io.DataOutputStream(java.nio.file.Files.newOutputStream(Paths.get(outPath)))
				Nd4j.write(arr, dos)
			End Using

			Dim [in] As INDArray
			Using dis As New java.io.DataInputStream(New FileStream(outPath, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim inDup As INDArray = [in].dup()

			assertEquals(arr, [in]) 'Passes:   Original array "in" is OK, but array "inDup" is not!?
			assertEquals([in], inDup) 'Fails
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorAlongDimension2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorAlongDimension2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single(99){}, New Long() {50, 1, 2})
			assertArrayEquals(New Long() {1, 2}, array.slice(0, 0).shape())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeEqualsOnElementWise(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeEqualsOnElementWise(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Nd4j.ones(10000, 1).sub(Nd4j.ones(1, 2))
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxVectorCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxVectorCase(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 4, 3}, New Long() {2, 2})
			Dim assertion As INDArray = Nd4j.create(New Boolean() {False, False, True, False}, New Long() {2, 2}, DataType.BOOL)
			Dim test As INDArray = Nd4j.Executioner.exec(New IsMax(arr))(0)
			assertEquals(assertion, test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMax(ByVal backend As Nd4jBackend)
			Dim toArgMax As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)
			Dim argMaxZero As INDArray = Nd4j.argMax(toArgMax, 0)
			Dim argMax As INDArray = Nd4j.argMax(toArgMax, 1)
			Dim argMaxTwo As INDArray = Nd4j.argMax(toArgMax, 2)
			Dim valueArray As INDArray = Nd4j.valueArrayOf(New Long() {4, 2}, 2, DataType.LONG)
			Dim valueArrayTwo As INDArray = Nd4j.valueArrayOf(New Long() {3, 2}, 3, DataType.LONG)
			Dim valueArrayThree As INDArray = Nd4j.valueArrayOf(New Long() {4, 3}, 1, DataType.LONG)
			assertEquals(valueArrayTwo, argMaxZero)
			assertEquals(valueArray, argMax)

			assertEquals(valueArrayThree, argMaxTwo)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMax_119(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMax_119(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(New Double(){1, 2, 119, 2})
			Dim max As val = array.argMax()

			assertTrue(max.isScalar())
			assertEquals(2L, max.getInt(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAutoBroadcastShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAutoBroadcastShape(ByVal backend As Nd4jBackend)
			Dim assertion As val = New Long(){2, 2, 2, 5}
			Dim shapeTest As val = Shape.broadcastOutputShape(New Long(){2, 1, 2, 1},New Long(){2, 1, 5})
			assertArrayEquals(assertion,shapeTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testAutoBroadcastAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAutoBroadcastAdd(ByVal backend As Nd4jBackend)
			Dim left As INDArray = Nd4j.linspace(1,4,4, DataType.DOUBLE).reshape(ChrW(2), 1, 2, 1)
			Dim right As INDArray = Nd4j.linspace(1,10,10, DataType.DOUBLE).reshape(ChrW(2), 1, 5)
			Dim assertion As INDArray = Nd4j.create(New Double(){2, 3, 4, 5, 6, 3, 4, 5, 6, 7, 7, 8, 9, 10, 11, 8, 9, 10, 11, 12, 4, 5, 6, 7, 8, 5, 6, 7, 8, 9, 9, 10, 11, 12, 13, 10, 11, 12, 13, 14}).reshape(ChrW(2), 2, 2, 5)
			Dim test As INDArray = left.add(right)
			assertEquals(assertion,test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAudoBroadcastAddMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAudoBroadcastAddMatrix(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1,4,4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim row As INDArray = Nd4j.ones(1, 2)
			Dim assertion As INDArray = arr.add(1.0)
			Dim test As INDArray = arr.add(row)
			assertEquals(assertion,test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarOps(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.ones(27).data(), New Long() {3, 3, 3})
			assertEquals(27R, n.length(), 1e-1)
			n.addi(Nd4j.scalar(1R))
			n.subi(Nd4j.scalar(1.0R))
			n.muli(Nd4j.scalar(1.0R))
			n.divi(Nd4j.scalar(1.0R))

			n = Nd4j.create(Nd4j.ones(27).data(), New Long() {3, 3, 3})
			assertEquals(27, n.sumNumber().doubleValue(), 1e-1,getFailureMessage(backend))
			Dim a As INDArray = n.slice(2)
			assertEquals(True, New Long() {3, 3}.SequenceEqual(a.shape()),getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorAlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorAlongDimension(ByVal backend As Nd4jBackend)
			Dim shape As val = New Long() {4, 5, 7}
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape(shape)


			Dim dim0s() As Integer = {0, 1, 2, 0, 1, 2}
			Dim dim1s() As Integer = {1, 0, 0, 2, 2, 1}

			Dim sums() As Double = {1350.0, 1350.0, 1582, 1582, 630, 630}

			For i As Integer = 0 To dim0s.Length - 1
				Dim firstDim As Integer = dim0s(i)
				Dim secondDim As Integer = dim1s(i)
				Dim tad As INDArray = arr.tensorAlongDimension(0, firstDim, secondDim)
				tad.sumNumber()
				'            assertEquals("I " + i + " failed ",sums[i],tad.sumNumber().doubleValue(),1e-1);
			Next i

			Dim testMem As INDArray = Nd4j.create(10, 10)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulWithTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulWithTranspose(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1,4,4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim arr2 As INDArray = Nd4j.linspace(1,4,4, DataType.DOUBLE).reshape(ChrW(2), 2).transpose()
			Dim arrTransposeAssertion As INDArray = arr.transpose().mmul(arr2)
			Dim mMulTranspose As MMulTranspose = MMulTranspose.builder().transposeA(True).build()

			Dim testResult As INDArray = arr.mmul(arr2,mMulTranspose)
			assertEquals(arrTransposeAssertion,testResult)


			Dim bTransposeAssertion As INDArray = arr.mmul(arr2.transpose())
			mMulTranspose = MMulTranspose.builder().transposeB(True).build()

			Dim bTest As INDArray = arr.mmul(arr2,mMulTranspose)
			assertEquals(bTransposeAssertion,bTest)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetDouble(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetDouble(ByVal backend As Nd4jBackend)
			Dim n2 As INDArray = Nd4j.create(Nd4j.linspace(1, 30, 30, DataType.DOUBLE).data(), New Long() {3, 5, 2})
			Dim swapped As INDArray = n2.swapAxes(n2.shape().Length - 1, 1)
			Dim slice0 As INDArray = swapped.slice(0).slice(1)
			Dim assertion As INDArray = Nd4j.create(New Double() {2, 4, 6, 8, 10})
			assertEquals(assertion, slice0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWriteTxt() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWriteTxt()
			Dim row As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {3, 4}
			})
			Dim bos As New MemoryStream()
			Nd4j.write(row, New DataOutputStream(bos))
			Dim bis As New MemoryStream(bos.toByteArray())
			Dim ret As INDArray = Nd4j.read(bis)
			assertEquals(row, ret)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dMatrixOrderingSwitch(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dMatrixOrderingSwitch(ByVal backend As Nd4jBackend)
			Dim order As Char = Nd4j.order()
			Dim c As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {3, 4}
			}, "c"c)
			assertEquals("c"c, c.ordering())
			assertEquals(order, Nd4j.order().Value)
			Dim f As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {3, 4}
			}, "f"c)
			assertEquals("f"c, f.ordering())
			assertEquals(order, Nd4j.order().Value)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrix(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}, New Long() {2, 2})
			Dim brr As INDArray = Nd4j.create(New Single() {5, 6}, New Long() {2})
			Dim row As INDArray = arr.getRow(0)
			row.subi(brr)
			assertEquals(Nd4j.create(New Single() {-4, -4}), arr.getRow(0))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMul(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 3},
				New Double() {4, 5, 6}
			})

			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {14, 32},
				New Double() {32, 77}
			})

			Dim test As INDArray = arr.mmul(arr.transpose())
			assertEquals(assertion, test,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testMmulOp(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMmulOp(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 3},
				New Double() {4, 5, 6}
			})
			Dim z As INDArray = Nd4j.create(2, 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {14, 32},
				New Double() {32, 77}
			})
			Dim mMulTranspose As MMulTranspose = MMulTranspose.builder().transposeB(True).build()

			Dim op As DynamicCustomOp = New Mmul(arr, arr, z, mMulTranspose)
			Nd4j.Executioner.execAndReturn(op)

			assertEquals(assertion, z,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSubiRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSubiRowVector(ByVal backend As Nd4jBackend)
			Dim oneThroughFour As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape("c"c, 2, 2)
			Dim row1 As INDArray = oneThroughFour.getRow(1).dup()
			oneThroughFour.subiRowVector(row1)
			Dim result As INDArray = Nd4j.create(New Double() {-2, -2, 0, 0}, New Long() {2, 2})
			assertEquals(result, oneThroughFour,getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddiRowVectorWithScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddiRowVectorWithScalar(ByVal backend As Nd4jBackend)
			Dim colVector As INDArray = Nd4j.create(5, 1).assign(0.0)
			Dim scalar As INDArray = Nd4j.create(1, 1).assign(0.0)
			scalar.putScalar(0, 1)

			assertEquals(scalar.getDouble(0), 1.0, 0.0)

			colVector.addiRowVector(scalar) 'colVector is all zeros after this
			For i As Integer = 0 To 4
				assertEquals(colVector.getDouble(i), 1.0, 0.0)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTADOnVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTADOnVector(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim rowVec As INDArray = Nd4j.rand(1, 10)
			Dim thirdElem As INDArray = rowVec.tensorAlongDimension(2, 0)

			assertEquals(rowVec.getDouble(2), thirdElem.getDouble(0), 0.0)

			thirdElem.putScalar(0, 5)
			assertEquals(5, thirdElem.getDouble(0), 0.0)

			assertEquals(5, rowVec.getDouble(2), 0.0) 'Both should be modified if thirdElem is a view

			'Same thing for column vector:
			Dim colVec As INDArray = Nd4j.rand(10, 1)
			thirdElem = colVec.tensorAlongDimension(2, 1)

			assertEquals(colVec.getDouble(2), thirdElem.getDouble(0), 0.0)

			thirdElem.putScalar(0, 5)
			assertEquals(5, thirdElem.getDouble(0), 0.0)
			assertEquals(5, colVec.getDouble(2), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLength(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLength(ByVal backend As Nd4jBackend)
			Dim values As INDArray = Nd4j.create(2, 2)
			Dim values2 As INDArray = Nd4j.create(2, 2)

			values.put(0, 0, 0)
			values2.put(0, 0, 2)
			values.put(1, 0, 0)
			values2.put(1, 0, 2)
			values.put(0, 1, 0)
			values2.put(0, 1, 0)
			values.put(1, 1, 2)
			values2.put(1, 1, 2)


			Dim expected As INDArray = Nd4j.repeat(Nd4j.scalar(DataType.DOUBLE, 2).reshape(ChrW(1), 1), 2).reshape(ChrW(2))

			Dim accum As val = New EuclideanDistance(values, values2)
			accum.setDimensions(1)

			Dim results As INDArray = Nd4j.Executioner.exec(accum)
			assertEquals(expected, results)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadCasting(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadCasting(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.arange(0, 3).reshape(ChrW(3), 1).castTo(DataType.DOUBLE)
			Dim ret As INDArray = first.broadcast(3, 4)
			Dim testRet As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 0, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2}
			})
			assertEquals(testRet, ret)
			Dim r As INDArray = Nd4j.arange(0, 4).reshape(ChrW(1), 4).castTo(DataType.DOUBLE)
			Dim r2 As INDArray = r.broadcast(4, 4)
			Dim testR2 As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3},
				New Double() {0, 1, 2, 3},
				New Double() {0, 1, 2, 3},
				New Double() {0, 1, 2, 3}
			})
			assertEquals(testR2, r2)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetColumns(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim matrixGet As INDArray = matrix.getColumns(1, 2)
			Dim matrixAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {2, 3},
				New Double() {5, 6}
			})
			assertEquals(matrixAssertion, matrixGet)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSort(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSort(ByVal backend As Nd4jBackend)
			Dim toSort As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim ascending As INDArray = Nd4j.sort(toSort.dup(), 1, True)
			'rows are already sorted
			assertEquals(toSort, ascending)

			Dim columnSorted As INDArray = Nd4j.create(New Double() {2, 1, 4, 3}, New Long() {2, 2})
			Dim sorted As INDArray = Nd4j.sort(toSort.dup(), 1, False)
			assertEquals(columnSorted, sorted)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSortRows(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSortRows(ByVal backend As Nd4jBackend)
			Dim nRows As Integer = 10
			Dim nCols As Integer = 5
			Dim r As New Random(12345)

			For i As Integer = 0 To nCols - 1
				Dim [in] As INDArray = Nd4j.linspace(1, nRows * nCols, nRows * nCols, DataType.DOUBLE).reshape(ChrW(nRows), nCols)

				Dim order As IList(Of Integer) = New List(Of Integer)(nRows)
				'in.row(order(i)) should end up as out.row(i) - ascending
				'in.row(order(i)) should end up as out.row(nRows-j-1) - descending
				For j As Integer = 0 To nRows - 1
					order.Add(j)
				Next j
				Collections.shuffle(order, r)
				For j As Integer = 0 To nRows - 1
					[in].putScalar(New Long() {j, i}, order(j))
				Next j

				Dim outAsc As INDArray = Nd4j.sortRows([in], i, True)
				Dim outDesc As INDArray = Nd4j.sortRows([in], i, False)

	'            System.out.println("outDesc: " + Arrays.toString(outAsc.data().asFloat()));
				For j As Integer = 0 To nRows - 1
					assertEquals(outAsc.getDouble(j, i), j, 1e-1)
					Dim origRowIdxAsc As Integer = order.IndexOf(j)
					assertTrue(outAsc.getRow(j).Equals([in].getRow(origRowIdxAsc)))

					assertEquals((nRows - j - 1), outDesc.getDouble(j, i), 0.001f)
					Dim origRowIdxDesc As Integer = order.IndexOf(nRows - j - 1)
					assertTrue(outDesc.getRow(j).Equals([in].getRow(origRowIdxDesc)))
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFlattenedOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToFlattenedOrder(ByVal backend As Nd4jBackend)
			Dim concatC As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape("c"c, 2, 2)
			Dim concatF As INDArray = Nd4j.create(New Long() {2, 2}, "f"c)
			concatF.assign(concatC)
			Dim assertionC As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 1, 2, 3, 4})
			Dim testC As INDArray = Nd4j.toFlattened("c"c, concatC, concatF)
			assertEquals(assertionC, testC)
			Dim test As INDArray = Nd4j.toFlattened("f"c, concatC, concatF)
			Dim assertion As INDArray = Nd4j.create(New Double() {1, 3, 2, 4, 1, 3, 2, 4})
			assertEquals(assertion, test)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZero(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZero(ByVal backend As Nd4jBackend)
			Nd4j.ones(11).sumNumber()
			Nd4j.ones(12).sumNumber()
			Nd4j.ones(2).sumNumber()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumNumberRepeatability(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumNumberRepeatability(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(1, 450).reshape("c"c, 150, 3)

			Dim first As Double = arr.sumNumber().doubleValue()
			Dim assertion As Double = 450
			assertEquals(assertion, first, 1e-1)
			For i As Integer = 0 To 49
				Dim second As Double = arr.sumNumber().doubleValue()
				assertEquals(assertion, second, 1e-1)
				assertEquals(first, second, 1e-2,i.ToString())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFlattened2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToFlattened2(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 3
			Dim cols As Integer = 4
			Dim dim2 As Integer = 5
			Dim dim3 As Integer = 6

			Dim length2d As Integer = rows * cols
			Dim length3d As Integer = rows * cols * dim2
			Dim length4d As Integer = rows * cols * dim2 * dim3

			Dim c2d As INDArray = Nd4j.linspace(1, length2d, length2d, DataType.DOUBLE).reshape("c"c, rows, cols)
			Dim f2d As INDArray = Nd4j.create(New Long() {rows, cols}, "f"c).assign(c2d).addi(0.1)

			Dim c3d As INDArray = Nd4j.linspace(1, length3d, length3d, DataType.DOUBLE).reshape("c"c, rows, cols, dim2)
			Dim f3d As INDArray = Nd4j.create(New Long() {rows, cols, dim2}).assign(c3d).addi(0.3)
			c3d.addi(0.2)

			Dim c4d As INDArray = Nd4j.linspace(1, length4d, length4d, DataType.DOUBLE).reshape("c"c, rows, cols, dim2, dim3)
			Dim f4d As INDArray = Nd4j.create(New Long() {rows, cols, dim2, dim3}).assign(c4d).addi(0.3)
			c4d.addi(0.4)


			assertEquals(toFlattenedViaIterator("c"c, c2d, f2d), Nd4j.toFlattened("c"c, c2d, f2d))
			assertEquals(toFlattenedViaIterator("f"c, c2d, f2d), Nd4j.toFlattened("f"c, c2d, f2d))
			assertEquals(toFlattenedViaIterator("c"c, f2d, c2d), Nd4j.toFlattened("c"c, f2d, c2d))
			assertEquals(toFlattenedViaIterator("f"c, f2d, c2d), Nd4j.toFlattened("f"c, f2d, c2d))

			assertEquals(toFlattenedViaIterator("c"c, c3d, f3d), Nd4j.toFlattened("c"c, c3d, f3d))
			assertEquals(toFlattenedViaIterator("f"c, c3d, f3d), Nd4j.toFlattened("f"c, c3d, f3d))
			assertEquals(toFlattenedViaIterator("c"c, c2d, f2d, c3d, f3d), Nd4j.toFlattened("c"c, c2d, f2d, c3d, f3d))
			assertEquals(toFlattenedViaIterator("f"c, c2d, f2d, c3d, f3d), Nd4j.toFlattened("f"c, c2d, f2d, c3d, f3d))

			assertEquals(toFlattenedViaIterator("c"c, c4d, f4d), Nd4j.toFlattened("c"c, c4d, f4d))
			assertEquals(toFlattenedViaIterator("f"c, c4d, f4d), Nd4j.toFlattened("f"c, c4d, f4d))
			assertEquals(toFlattenedViaIterator("c"c, c2d, f2d, c3d, f3d, c4d, f4d), Nd4j.toFlattened("c"c, c2d, f2d, c3d, f3d, c4d, f4d))
			assertEquals(toFlattenedViaIterator("f"c, c2d, f2d, c3d, f3d, c4d, f4d), Nd4j.toFlattened("f"c, c2d, f2d, c3d, f3d, c4d, f4d))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFlattenedOnViews(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToFlattenedOnViews(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 8
			Dim cols As Integer = 8
			Dim dim2 As Integer = 4
			Dim length As Integer = rows * cols
			Dim length3d As Integer = rows * cols * dim2

			Dim first As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, rows, cols)
			Dim second As INDArray = Nd4j.create(New Long() {rows, cols}, "f"c).assign(first)
			Dim third As INDArray = Nd4j.linspace(1, length3d, length3d, DataType.DOUBLE).reshape("c"c, rows, cols, dim2)
			first.addi(0.1)
			second.addi(0.2)
			third.addi(0.3)

			first = first.get(NDArrayIndex.interval(4, 8), NDArrayIndex.interval(0, 2, 8))
			second = second.get(NDArrayIndex.interval(3, 7), NDArrayIndex.all())
			third = third.permute(0, 2, 1)
			Dim noViewC As INDArray = Nd4j.toFlattened("c"c, first.dup("c"c), second.dup("c"c), third.dup("c"c))
			Dim noViewF As INDArray = Nd4j.toFlattened("f"c, first.dup("f"c), second.dup("f"c), third.dup("f"c))

			assertEquals(noViewC, Nd4j.toFlattened("c"c, first, second, third))

			'val result = Nd4j.exec(new Flatten('f', first, second, third))[0];
			'assertEquals(noViewF, result);
			assertEquals(noViewF, Nd4j.toFlattened("f"c, first, second, third))
		End Sub

		Private Shared Function toFlattenedViaIterator(ByVal order As Char, ParamArray ByVal toFlatten() As INDArray) As INDArray
			Dim length As Integer = 0
			For Each i As INDArray In toFlatten
				length += i.length()
			Next i

			Dim [out] As INDArray = Nd4j.create(length)
			Dim i As Integer = 0
			For Each arr As INDArray In toFlatten
				Dim iter As New NdIndexIterator(order, arr.shape())
				Do While iter.MoveNext()
					Dim [next] As Double = arr.getDouble(iter.Current)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out.putScalar(i++, next);
					[out].putScalar(i, [next])
						i += 1
				Loop
			Next arr

			Return [out]
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMax2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMax2(ByVal backend As Nd4jBackend)
			'Tests: full buffer...
			'1d
			Dim arr1 As INDArray = Nd4j.create(New Double() {1, 2, 3, 1})
			Dim res1 As val = Nd4j.Executioner.exec(New IsMax(arr1))(0)
			Dim exp1 As INDArray = Nd4j.create(New Boolean() {False, False, True, False})

			assertEquals(exp1, res1)

			arr1 = Nd4j.create(New Double() {1, 2, 3, 1})
			Dim result As INDArray = Nd4j.createUninitialized(DataType.BOOL, 4)
			Nd4j.Executioner.execAndReturn(New IsMax(arr1, result))

			assertEquals(Nd4j.create(New Double() {1, 2, 3, 1}), arr1)
			assertEquals(exp1, result)

			'2d
			Dim arr2d As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 2},
				New Double() {2, 9, 1}
			})
			Dim exp2d As INDArray = Nd4j.create(New Boolean()() {
				New Boolean() {False, False, False},
				New Boolean() {False, True, False}
			})

			Dim f As INDArray = arr2d.dup("f"c)
			Dim out2dc As INDArray = Nd4j.Executioner.exec(New IsMax(arr2d.dup("c"c)))(0)
			Dim out2df As INDArray = Nd4j.Executioner.exec(New IsMax(arr2d.dup("f"c)))(0)
			assertEquals(exp2d, out2dc)
			assertEquals(exp2d, out2df)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFlattened3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToFlattened3(ByVal backend As Nd4jBackend)
			Dim inC1 As INDArray = Nd4j.create(New Long() {10, 100}, "c"c)
			Dim inC2 As INDArray = Nd4j.create(New Long() {1, 100}, "c"c)

			Dim inF1 As INDArray = Nd4j.create(New Long() {10, 100}, "f"c)
			'        INDArray inF1 = Nd4j.create(new long[]{784,1000},'f');
			Dim inF2 As INDArray = Nd4j.create(New Long() {1, 100}, "f"c)

			Nd4j.toFlattened("f"c, inF1) 'ok
			Nd4j.toFlattened("f"c, inF2) 'ok

			Nd4j.toFlattened("f"c, inC1) 'crash
			Nd4j.toFlattened("f"c, inC2) 'crash

			Nd4j.toFlattened("c"c, inF1) 'crash on shape [784,1000]. infinite loop on shape [10,100]
			Nd4j.toFlattened("c"c, inF2) 'ok

			Nd4j.toFlattened("c"c, inC1) 'ok
			Nd4j.toFlattened("c"c, inC2) 'ok
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxEqualValues(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxEqualValues(ByVal backend As Nd4jBackend)
			'Assumption here: should only have a 1 for *first* maximum value, if multiple values are exactly equal

			'[1 1 1] -> [1 0 0]
			'Loop to double check against any threading weirdness...
			For i As Integer = 0 To 9
				Dim res As val = Transforms.isMax(Nd4j.ones(3), DataType.BOOL)
				assertEquals(Nd4j.create(New Boolean() {True, False, False}), res)
			Next i

			'[0 0 0 2 2 0] -> [0 0 0 1 0 0]
			assertEquals(Nd4j.create(New Boolean() {False, False, False, True, False, False}), Transforms.isMax(Nd4j.create(New Double() {0, 0, 0, 2, 2, 0}), DataType.BOOL))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMaxVector_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMaxVector_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.ones(3)
			Dim idx As val = array.argMax(0).getInt(0)
			assertEquals(0, idx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMaxVector_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMaxVector_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.ones(3)
			Dim idx As val = array.argMax(Integer.MaxValue).getInt(0)
			assertEquals(0, idx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMaxVector_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMaxVector_3(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.ones(3)
			Dim idx As val = array.argMax().getInt(0)
			assertEquals(0, idx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxEqualValues_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxEqualValues_2(ByVal backend As Nd4jBackend)
			'[0 2]    [0 1]
			'[2 1] -> [0 0]bg
			Dim orig As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 3},
				New Double() {2, 1}
			})
			Dim exp As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {0, 0}
			})
			Dim outc As INDArray = Transforms.isMax(orig.dup("c"c))
			assertEquals(exp, outc)

	'        log.info("Orig: {}", orig.dup('f').data().asFloat());

			Dim outf As INDArray = Transforms.isMax(orig.dup("f"c), orig.dup("f"c).ulike())
	'        log.info("OutF: {}", outf.data().asFloat());
			assertEquals(exp, outf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxEqualValues_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxEqualValues_3(ByVal backend As Nd4jBackend)
			'[0 2]    [0 1]
			'[2 1] -> [0 0]
			Dim orig As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 2},
				New Double() {3, 1}
			})
			Dim exp As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 0},
				New Double() {1, 0}
			})
			Dim outc As INDArray = Transforms.isMax(orig.dup("c"c))
			assertEquals(exp, outc)

			Dim outf As INDArray = Transforms.isMax(orig.dup("f"c), orig.dup("f"c).ulike())
			assertEquals(exp, outf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSqrt_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSqrt_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.createFromArray(9.0, 9.0, 9.0, 9.0)
			Dim x2 As val = Nd4j.createFromArray(9.0, 9.0, 9.0, 9.0)
			Dim e As val = Nd4j.createFromArray(3.0, 3.0, 3.0, 3.0)

			Dim z1 As val = Transforms.sqrt(x, True)
			Dim z2 As val = Transforms.sqrt(x2, False)


			assertEquals(e, z2)
			assertEquals(e, x2)
			assertEquals(e, z1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign_CF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign_CF(ByVal backend As Nd4jBackend)
			Dim orig As val = Nd4j.create(New Double()() {
				New Double() {0, 2},
				New Double() {2, 1}
			})
			Dim oc As val = orig.dup("c"c)
			Dim [of] As val = orig.dup("f"c)

			assertEquals(orig, oc)
			assertEquals(orig, [of])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxAlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxAlongDimension(ByVal backend As Nd4jBackend)
			'1d: row vector
			Dim orig As INDArray = Nd4j.create(New Double() {1, 2, 3, 1}).reshape(ChrW(1), 4)

			Dim alongDim0 As INDArray = Nd4j.Executioner.exec(New IsMax(orig.dup(), Nd4j.createUninitialized(DataType.BOOL, orig.shape()), 0))(0)
			Dim alongDim1 As INDArray = Nd4j.Executioner.exec(New IsMax(orig.dup(), Nd4j.createUninitialized(DataType.BOOL, orig.shape()), 1))(0)

			Dim expAlong0 As INDArray = Nd4j.create(New Boolean(){True, True, True, True}).reshape(ChrW(1), 4)
			Dim expAlong1 As INDArray = Nd4j.create(New Boolean() {False, False, True, False}).reshape(ChrW(1), 4)

			assertEquals(expAlong0, alongDim0)
			assertEquals(expAlong1, alongDim1)


			'1d: col vector
	'        System.out.println("----------------------------------");
			Dim col As INDArray = Nd4j.create(New Double() {1, 2, 3, 1}, New Long() {4, 1})
			Dim alongDim0col As INDArray = Nd4j.Executioner.exec(New IsMax(col.dup(), Nd4j.createUninitialized(DataType.BOOL, col.shape()), 0))(0)
			Dim alongDim1col As INDArray = Nd4j.Executioner.exec(New IsMax(col.dup(), Nd4j.createUninitialized(DataType.BOOL, col.shape()),1))(0)

			Dim expAlong0col As INDArray = Nd4j.create(New Boolean() {False, False, True, False}).reshape(ChrW(4), 1)
			Dim expAlong1col As INDArray = Nd4j.create(New Boolean() {True, True, True, True}).reshape(ChrW(4), 1)



			assertEquals(expAlong1col, alongDim1col)
			assertEquals(expAlong0col, alongDim0col)



	'        
	'        if (blockIdx.x == 0) {
	'            printf("original Z shape: \n");
	'            shape::printShapeInfoLinear(zShapeInfo);
	'
	'            printf("Target dimension: [%i], dimensionLength: [%i]\n", dimension[0], dimensionLength);
	'
	'            printf("TAD shape: \n");
	'            shape::printShapeInfoLinear(tad->tadOnlyShapeInfo);
	'        }
	'        

			'2d:
			'[1 0 2]
			'[2 3 1]
			'Along dim 0:
			'[0 0 1]
			'[1 1 0]
			'Along dim 1:
			'[0 0 1]
			'[0 1 0]
	'        System.out.println("---------------------");
			Dim orig2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0, 2},
				New Double() {2, 3, 1}
			})
			Dim alongDim0c_2d As INDArray = Nd4j.Executioner.exec(New IsMax(orig2d.dup("c"c), Nd4j.createUninitialized(DataType.BOOL, orig2d.shape()), 0))(0)
			Dim alongDim0f_2d As INDArray = Nd4j.Executioner.exec(New IsMax(orig2d.dup("f"c), Nd4j.createUninitialized(DataType.BOOL, orig2d.shape(), "f"c), 0))(0)
			Dim alongDim1c_2d As INDArray = Nd4j.Executioner.exec(New IsMax(orig2d.dup("c"c), Nd4j.createUninitialized(DataType.BOOL, orig2d.shape()), 1))(0)
			Dim alongDim1f_2d As INDArray = Nd4j.Executioner.exec(New IsMax(orig2d.dup("f"c), Nd4j.createUninitialized(DataType.BOOL, orig2d.shape(), "f"c), 1))(0)

			Dim expAlong0_2d As INDArray = Nd4j.create(New Boolean()() {
				New Boolean() {False, False, True},
				New Boolean() {True, True, False}
			})
			Dim expAlong1_2d As INDArray = Nd4j.create(New Boolean()() {
				New Boolean() {False, False, True},
				New Boolean() {False, True, False}
			})

			assertEquals(expAlong0_2d, alongDim0c_2d)
			assertEquals(expAlong0_2d, alongDim0f_2d)
			assertEquals(expAlong1_2d, alongDim1c_2d)
			assertEquals(expAlong1_2d, alongDim1f_2d)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMaxSingleDim1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMaxSingleDim1(ByVal backend As Nd4jBackend)
			Dim orig2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0, 2},
				New Double() {2, 3, 1}
			})

			Dim result As INDArray = Nd4j.argMax(orig2d.dup("c"c), 0)

	'        System.out.println("IMAx result: " + result);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxSingleDim1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxSingleDim1(ByVal backend As Nd4jBackend)
			Dim orig2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0, 2},
				New Double() {2, 3, 1}
			})
			Dim alongDim0c_2d As INDArray = Nd4j.Executioner.exec(New IsMax(orig2d.dup("c"c), Nd4j.createUninitialized(DataType.BOOL, orig2d.shape()), 0))(0)
			Dim expAlong0_2d As INDArray = Nd4j.create(New Boolean()() {
				New Boolean() {False, False, True},
				New Boolean() {True, True, False}
			})

	'        System.out.println("Original shapeInfo: " + orig2d.dup('c').shapeInfoDataBuffer());

	'        System.out.println("Expected: " + Arrays.toString(expAlong0_2d.data().asFloat()));
	'        System.out.println("Actual: " + Arrays.toString(alongDim0c_2d.data().asFloat()));
			assertEquals(expAlong0_2d, alongDim0c_2d)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastRepeated(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastRepeated(ByVal backend As Nd4jBackend)
			Dim z As INDArray = Nd4j.create(1, 4, 4, 3)
			Dim bias As INDArray = Nd4j.create(1, 3)
			Dim op As BroadcastOp = New BroadcastAddOp(z, bias, z, 3)
			Nd4j.Executioner.exec(op)
	'        System.out.println("First: OK");
			'OK at this point: executes successfully


			z = Nd4j.create(1, 4, 4, 3)
			bias = Nd4j.create(1, 3)
			op = New BroadcastAddOp(z, bias, z, 3)
			Nd4j.Executioner.exec(op) 'Crashing here, when we are doing exactly the same thing as before...
	'        System.out.println("Second: OK");
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVStackDifferentOrders(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVStackDifferentOrders(ByVal backend As Nd4jBackend)
			Dim expected As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)

			For Each order As Char In New Char() {"c"c, "f"c}
	'            System.out.println(order);

				Dim arr1 As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3).dup("c"c)
				Dim arr2 As INDArray = Nd4j.linspace(7, 9, 3, DataType.DOUBLE).reshape(ChrW(1), 3).dup("c"c)

				Nd4j.factory().Order = order

	'            log.info("arr1: {}", arr1.data());
	'            log.info("arr2: {}", arr2.data());

				Dim merged As INDArray = Nd4j.vstack(arr1, arr2)
	'            System.out.println(merged.data());
	'            System.out.println(expected);

				assertEquals(expected, merged,"Failed for [" & order & "] order")
			Next order
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVStackEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVStackEdgeCase(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Dim vstacked As INDArray = Nd4j.vstack(arr)
			assertEquals(arr.reshape(ChrW(1), 4), vstacked)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEps3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEps3(ByVal backend As Nd4jBackend)

			Dim first As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Dim second As INDArray = Nd4j.linspace(20, 30, 10, DataType.DOUBLE)

			Dim expAllZeros As INDArray = Nd4j.Executioner.exec(New Eps(first, second, Nd4j.create(DataType.BOOL, 10)))
			Dim expAllOnes As INDArray = Nd4j.Executioner.exec(New Eps(first, first, Nd4j.create(DataType.BOOL, 10)))

	'        System.out.println(expAllZeros);
	'        System.out.println(expAllOnes);

			Dim allones As val = Nd4j.Executioner.exec(New All(expAllOnes)).getDouble(0)

			assertTrue(expAllZeros.none())
			assertTrue(expAllOnes.all())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testSumAlongDim1sEdgeCases(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumAlongDim1sEdgeCases(ByVal backend As Nd4jBackend)
			Dim shapes As val = New Long()() {
				New Long() {2, 2, 3, 4},
				New Long() {1, 2, 3, 4},
				New Long() {1, 1, 2, 3},
				New Long() {4, 3, 2, 1},
				New Long() {4, 3, 1, 1},
				New Long() {4, 1, 3, 2},
				New Long() {4, 3, 1, 2},
				New Long() {4, 1, 1, 2}
			}

			Dim sumDims()() As Integer = {
				New Integer() {0},
				New Integer() {1},
				New Integer() {2},
				New Integer() {3},
				New Integer() {0, 1},
				New Integer() {0, 2},
				New Integer() {0, 3},
				New Integer() {1, 2},
				New Integer() {1, 3},
				New Integer() {0, 1, 2},
				New Integer() {0, 1, 3},
				New Integer() {0, 2, 3},
				New Integer() {0, 1, 2, 3}
			}
	'                for( int[] shape : shapes) {
	'            for (int[] dims : sumDims) {
	'                System.out.println("Shape");
	'                System.out.println(Arrays.toString(shape));
	'                System.out.println("Dimensions");
	'                System.out.println(Arrays.toString(dims));
	'                int length = ArrayUtil.prod(shape);
	'                INDArray inC = Nd4j.linspace(1, length, length).reshape('c', shape);
	'                System.out.println("TAD shape");
	'                System.out.println(Arrays.toString((inC.tensorAlongDimension(0,dims).shape())));
	'
	'                INDArray inF = inC.dup('f');
	'                System.out.println("C stride " + Arrays.toString(inC.tensorAlongDimension(0,dims).stride()) + " and f stride " + Arrays.toString(inF.tensorAlongDimension(0,dims).stride()));
	'                for(int i = 0; i < inC.tensorsAlongDimension(dims); i++) {
	'                    System.out.println(inC.tensorAlongDimension(i,dims).ravel());
	'                }
	'                for(int i = 0; i < inF.tensorsAlongDimension(dims); i++) {
	'                    System.out.println(inF.tensorAlongDimension(i,dims).ravel());
	'                }
	'            }
	'        }
			For Each shape As val In shapes
				For Each dims As Integer() In sumDims
	'                System.out.println("Shape: " + Arrays.toString(shape) + ", sumDims=" + Arrays.toString(dims));
					Dim length As Integer = ArrayUtil.prod(shape)
					Dim inC As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape)
					Dim inF As INDArray = inC.dup("f"c)
					assertEquals(inC, inF)

					Dim sumC As INDArray = inC.sum(dims)
					Dim sumF As INDArray = inF.sum(dims)
					assertEquals(sumC, sumF)

					'Multiple runs: check for consistency between runs (threading issues, etc)
					For i As Integer = 0 To 99
						assertEquals(sumC, inC.sum(dims))
						assertEquals(sumF, inF.sum(dims))
					Next i
				Next dims
			Next shape
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxAlongDimensionSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxAlongDimensionSimple(ByVal backend As Nd4jBackend)
			'Simple test: when doing IsMax along a dimension, we expect all values to be either 0 or 1
			'Do IsMax along dims 0&1 for rank 2, along 0,1&2 for rank 3, etc

			For rank As Integer = 2 To 6

				Dim shape(rank - 1) As Integer
				For i As Integer = 0 To rank - 1
					shape(i) = 2
				Next i
				Dim length As Integer = ArrayUtil.prod(shape)


				For alongDimension As Integer = 0 To rank - 1
	'                System.out.println("Testing rank " + rank + " along dimension " + alongDimension + ", (shape="
	'                        + Arrays.toString(shape) + ")");
					Dim arrC As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape)
					Dim arrF As INDArray = arrC.dup("f"c)
					Dim resC As val = Nd4j.Executioner.exec(New IsMax(arrC, alongDimension))(0)
					Dim resF As val = Nd4j.Executioner.exec(New IsMax(arrF, alongDimension))(0)


					Dim cBuffer() As Double = resC.data().asDouble()
					Dim fBuffer() As Double = resF.data().asDouble()
					For i As Integer = 0 To length - 1
						assertTrue(cBuffer(i) = 0.0 OrElse cBuffer(i) = 1.0,"c buffer value at [" & i & "]=" & cBuffer(i) & ", expected 0 or 1; dimension = " & alongDimension & ", rank = " & rank & ", shape=" & java.util.Arrays.toString(shape))
					Next i
					For i As Integer = 0 To length - 1
						assertTrue(fBuffer(i) = 0.0 OrElse fBuffer(i) = 1.0,"f buffer value at [" & i & "]=" & fBuffer(i) & ", expected 0 or 1; dimension = " & alongDimension & ", rank = " & rank & ", shape=" & java.util.Arrays.toString(shape))
					Next i
				Next alongDimension
			Next rank
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSortColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSortColumns(ByVal backend As Nd4jBackend)
			Dim nRows As Integer = 5
			Dim nCols As Integer = 10
			Dim r As New Random(12345)

			Dim i As Integer = 0
			Do While i < nRows
				Dim [in] As INDArray = Nd4j.rand(New Long() {nRows, nCols})

				Dim order As IList(Of Integer) = New List(Of Integer)(nRows)
				For j As Integer = 0 To nCols - 1
					order.Add(j)
				Next j
				Collections.shuffle(order, r)
				For j As Integer = 0 To nCols - 1
					[in].putScalar(New Long() {i, j}, order(j))
				Next j

				Dim outAsc As INDArray = Nd4j.sortColumns([in], i, True)
				Dim outDesc As INDArray = Nd4j.sortColumns([in], i, False)

				For j As Integer = 0 To nCols - 1
					assertTrue(outAsc.getDouble(i, j) = j)
					Dim origColIdxAsc As Integer = order.IndexOf(j)
					assertTrue(outAsc.getColumn(j).Equals([in].getColumn(origColIdxAsc)))

					assertTrue(outDesc.getDouble(i, j) = (nCols - j - 1))
					Dim origColIdxDesc As Integer = order.IndexOf(nCols - j - 1)
					assertTrue(outDesc.getColumn(j).Equals([in].getColumn(origColIdxDesc)))
				Next j
				i += 1
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddVectorWithOffset(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddVectorWithOffset(ByVal backend As Nd4jBackend)
			Dim oneThroughFour As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim row1 As INDArray = oneThroughFour.getRow(1)
			row1.addi(1)
			Dim result As INDArray = Nd4j.create(New Double() {1, 2, 4, 5}, New Long() {2, 2})
			assertEquals(result, oneThroughFour,getFailureMessage(backend))


		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinearViewGetAndPut(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinearViewGetAndPut(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim linear As INDArray = test.reshape(ChrW(-1))
			linear.putScalar(2, 6)
			linear.putScalar(3, 7)
			assertEquals(6, linear.getFloat(2), 1e-1,getFailureMessage(backend))
			assertEquals(7, linear.getFloat(3), 1e-1,getFailureMessage(backend))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowVectorGemm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowVectorGemm(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(1), 4)
			Dim other As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(4), 4)
			Dim result As INDArray = linspace.mmul(other)
			Dim assertion As INDArray = Nd4j.create(New Double() {90, 100, 110, 120}).reshape(ChrW(4), 1)
			assertEquals(assertion, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemmStrided()
		Public Overridable Sub testGemmStrided()

			For Each x As val In New Integer(){5, 1}

				Dim la As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(5, x, 12345, DataType.DOUBLE)
				Dim lb As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(x, 4, 12345, DataType.DOUBLE)

				For i As Integer = 0 To la.Count - 1
					For j As Integer = 0 To lb.Count - 1

						Dim msg As String = "x=" & x & ", i=" & i & ", j=" & j

						Dim a As INDArray = la(i).getFirst()
						Dim b As INDArray = lb(i).getFirst()

						Dim result1 As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, New Long(){5, 4}, "f"c)
						Dim result2 As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, New Long(){5, 4}, "f"c)
						Dim result3 As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, New Long(){5, 4}, "f"c)

						Nd4j.gemm(a.dup("c"c), b.dup("c"c), result1, False, False, 1.0, 0.0)
						Nd4j.gemm(a.dup("f"c), b.dup("f"c), result2, False, False, 1.0, 0.0)
						Nd4j.gemm(a, b, result3, False, False, 1.0, 0.0)

						assertEquals(result1, result2,msg)
						assertEquals(result1, result3,msg) ' Fails here
					Next j
				Next i
			Next x
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultiSum(ByVal backend As Nd4jBackend)
			''' <summary>
			''' ([[[ 0.,  1.],
			''' [ 2.,  3.]],
			''' 
			''' [[ 4.,  5.],
			''' [ 6.,  7.]]])
			''' 
			''' [0.0,1.0,2.0,3.0,4.0,5.0,6.0,7.0]
			''' 
			''' 
			''' Rank: 3,Offset: 0
			''' Order: c shape: [2,2,2], stride: [4,2,1]
			''' </summary>
			' 
			Dim arr As INDArray = Nd4j.linspace(0, 7, 8, DataType.DOUBLE).reshape("c"c, 2, 2, 2)
	'         [0.0,4.0,2.0,6.0,1.0,5.0,3.0,7.0]
	'        *
	'        * Rank: 3,Offset: 0
	'            Order: f shape: [2,2,2], stride: [1,2,4]
			Dim arrF As INDArray = Nd4j.create(New Long() {2, 2, 2}, "f"c).assign(arr)

			assertEquals(arr, arrF)
			'0,2,4,6 and 1,3,5,7
			assertEquals(Nd4j.create(New Double() {12, 16}), arr.sum(0, 1))
			'0,1,4,5 and 2,3,6,7
			assertEquals(Nd4j.create(New Double() {10, 18}), arr.sum(0, 2))
			'0,2,4,6 and 1,3,5,7
			assertEquals(Nd4j.create(New Double() {12, 16}), arrF.sum(0, 1))
			'0,1,4,5 and 2,3,6,7
			assertEquals(Nd4j.create(New Double() {10, 18}), arrF.sum(0, 2))

			'0,1,2,3 and 4,5,6,7
			assertEquals(Nd4j.create(New Double() {6, 22}), arr.sum(1, 2))
			'0,1,2,3 and 4,5,6,7
			assertEquals(Nd4j.create(New Double() {6, 22}), arrF.sum(1, 2))


			Dim data() As Double = {10, 26, 42}
			Dim assertion As INDArray = Nd4j.create(data)
			For i As Integer = 0 To data.Length - 1
				assertEquals(data(i), assertion.getDouble(i), 1e-1)
			Next i

			Dim twoTwoByThree As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("f"c, 2, 2, 3)
			Dim multiSum As INDArray = twoTwoByThree.sum(0, 1)
			assertEquals(assertion, multiSum)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum2dv2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum2dv2(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape("c"c, 2, 2, 2)

			Dim dims As val = New Integer()() {
				New Integer() {0, 1},
				New Integer() {1, 0},
				New Integer() {0, 2},
				New Integer() {2, 0},
				New Integer() {1, 2},
				New Integer() {2, 1}
			}
			Dim exp()() As Double = {
				New Double() {16, 20},
				New Double() {16, 20},
				New Double() {14, 22},
				New Double() {14, 22},
				New Double() {10, 26},
				New Double() {10, 26}
			}

	'        System.out.println("dims\texpected\t\tactual");
			For i As Integer = 0 To dims.length - 1
				Dim d As val = dims(i)
				Dim e() As Double = exp(i)

				Dim [out] As INDArray = [in].sum(d)

	'            System.out.println(Arrays.toString(d) + "\t" + Arrays.toString(e) + "\t" + out);
				assertEquals(Nd4j.create(e, [out].shape()), [out])
			Next i
		End Sub


		'Passes on 3.9:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum3Of4_2222(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum3Of4_2222(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {2, 2, 2, 2}
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim arrC As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape)
			Dim arrF As INDArray = Nd4j.create(arrC.shape()).assign(arrC)

			Dim dimsToSum()() As Integer = {
				New Integer() {0, 1, 2},
				New Integer() {0, 1, 3},
				New Integer() {0, 2, 3},
				New Integer() {1, 2, 3}
			}
			Dim expD()() As Double = {
				New Double() {64, 72},
				New Double() {60, 76},
				New Double() {52, 84},
				New Double() {36, 100}
			}

			For i As Integer = 0 To dimsToSum.Length - 1
				Dim d() As Integer = dimsToSum(i)

				Dim outC As INDArray = arrC.sum(d)
				Dim outF As INDArray = arrF.sum(d)
				Dim exp As INDArray = Nd4j.create(expD(i), outC.shape()).castTo(DataType.DOUBLE)

				assertEquals(exp, outC)
				assertEquals(exp, outF)

	'            System.out.println(Arrays.toString(d) + "\t" + outC + "\t" + outF);
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcast1d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcast1d(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {4, 3, 2}
			Dim toBroadcastDims() As Integer = {0, 1, 2}
			Dim toBroadcastShapes()() As Integer = {
				New Integer() {1, 4},
				New Integer() {1, 3},
				New Integer() {1, 2}
			}

			'Expected result values in buffer: c order, need to reshape to {4,3,2}. Values taken from 0.4-rc3.8
			Dim expFlat()() As Double = {
				New Double() {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 4.0, 4.0, 4.0, 4.0, 4.0, 4.0},
				New Double() {1.0, 1.0, 2.0, 2.0, 3.0, 3.0, 1.0, 1.0, 2.0, 2.0, 3.0, 3.0, 1.0, 1.0, 2.0, 2.0, 3.0, 3.0, 1.0, 1.0, 2.0, 2.0, 3.0, 3.0},
				New Double() {1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0, 1.0, 2.0}
			}

			Dim expLinspaced()() As Double = {
				New Double() {2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0},
				New Double() {2.0, 3.0, 5.0, 6.0, 8.0, 9.0, 8.0, 9.0, 11.0, 12.0, 14.0, 15.0, 14.0, 15.0, 17.0, 18.0, 20.0, 21.0, 20.0, 21.0, 23.0, 24.0, 26.0, 27.0},
				New Double() {2.0, 4.0, 4.0, 6.0, 6.0, 8.0, 8.0, 10.0, 10.0, 12.0, 12.0, 14.0, 14.0, 16.0, 16.0, 18.0, 18.0, 20.0, 20.0, 22.0, 22.0, 24.0, 24.0, 26.0}
			}

			For i As Integer = 0 To toBroadcastDims.Length - 1
				Dim [dim] As Integer = toBroadcastDims(i)
				Dim vectorShape() As Integer = toBroadcastShapes(i)
				Dim length As Integer = ArrayUtil.prod(vectorShape)

				Dim zC As INDArray = Nd4j.create(shape, "c"c)
				zC.Data = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).data()
				Dim tad As Integer = 0
				Do While tad < zC.tensorsAlongDimension([dim])
					Dim javaTad As INDArray = zC.tensorAlongDimension(tad, [dim])
	'                System.out.println("Tad " + tad + " is " + zC.tensorAlongDimension(tad, dim));
					tad += 1
				Loop

				Dim zF As INDArray = Nd4j.create(shape, "f"c)
				zF.assign(zC)
				Dim toBroadcast As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE)

				Dim opc As Op = New BroadcastAddOp(zC, toBroadcast, zC, [dim])
				Dim opf As Op = New BroadcastAddOp(zF, toBroadcast, zF, [dim])
				Dim exp As INDArray = Nd4j.create(expLinspaced(i), shape, "c"c)
				Dim expF As INDArray = Nd4j.create(shape, "f"c)
				expF.assign(exp)
	'            for (int tad = 0; tad < zC.tensorsAlongDimension(dim); tad++) {
	'                System.out.println(zC.tensorAlongDimension(tad, dim).offset() + " and f offset is "
	'                        + zF.tensorAlongDimension(tad, dim).offset());
	'            }

				Nd4j.Executioner.exec(opc)
				Nd4j.Executioner.exec(opf)

				assertEquals(exp, zC)
				assertEquals(exp, zF)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum3Of4_3322(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum3Of4_3322(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {3, 3, 2, 2}
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim arrC As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape)
			Dim arrF As INDArray = Nd4j.create(arrC.shape()).assign(arrC)

			Dim dimsToSum()() As Integer = {
				New Integer() {0, 1, 2},
				New Integer() {0, 1, 3},
				New Integer() {0, 2, 3},
				New Integer() {1, 2, 3}
			}
			Dim expD()() As Double = {
				New Double() {324, 342},
				New Double() {315, 351},
				New Double() {174, 222, 270},
				New Double() {78, 222, 366}
			}

			For i As Integer = 0 To dimsToSum.Length - 1
				Dim d() As Integer = dimsToSum(i)

				Dim outC As INDArray = arrC.sum(d)
				Dim outF As INDArray = arrF.sum(d)
				Dim exp As INDArray = Nd4j.create(expD(i), outC.shape())

				assertEquals(exp, outC)
				assertEquals(exp, outF)

				'System.out.println(Arrays.toString(d) + "\t" + outC + "\t" + outF);
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFlattened(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToFlattened(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim concat As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 2
				concat.Add(arr.dup())
			Next i

			Dim assertion As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4}, New Integer(){12})
			Dim flattened As INDArray = Nd4j.toFlattened(concat).castTo(assertion.dataType())
			assertEquals(assertion, flattened)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDup(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDup(ByVal backend As Nd4jBackend)
			For x As Integer = 0 To 99
				Dim orig As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
				Dim dup As INDArray = orig.dup()
				assertEquals(orig, dup)

				Dim matrix As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}, New Long() {2, 2})
				Dim dup2 As INDArray = matrix.dup()
				assertEquals(matrix, dup2)

				Dim row1 As INDArray = matrix.getRow(1)
				Dim dupRow As INDArray = row1.dup()
				assertEquals(row1, dupRow)


				Dim columnSorted As INDArray = Nd4j.create(New Single() {2, 1, 4, 3}, New Long() {2, 2})
				Dim dup3 As INDArray = columnSorted.dup()
				assertEquals(columnSorted, dup3)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSortWithIndicesDescending(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSortWithIndicesDescending(ByVal backend As Nd4jBackend)
			Dim toSort As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			'indices,data
			Dim sorted() As INDArray = Nd4j.sortWithIndices(toSort.dup(), 1, False)
			Dim sorted2 As INDArray = Nd4j.sort(toSort.dup(), 1, False)
			assertEquals(sorted(1), sorted2)
			Dim shouldIndex As INDArray = Nd4j.create(New Double() {1, 0, 1, 0}, New Long() {2, 2})
			assertEquals(shouldIndex, sorted(0))


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetFromRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetFromRowVector(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim rowGet As INDArray = matrix.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 2))
			assertArrayEquals(New Long() {2}, rowGet.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSubRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSubRowVector(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim row As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE)
			Dim test As INDArray = matrix.subRowVector(row)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {3, 3, 3}
			})
			assertEquals(assertion, test)

			Dim threeByThree As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim offsetTest As INDArray = threeByThree.get(NDArrayIndex.interval(1, 3), NDArrayIndex.all())
			assertEquals(2, offsetTest.rows())
			Dim offsetAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {3, 3, 3},
				New Double() {6, 6, 6}
			})
			Dim offsetSub As INDArray = offsetTest.subRowVector(row)
			assertEquals(offsetAssertion, offsetSub)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimShuffle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimShuffle(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim twoOneTwo As INDArray = n.dimShuffle(New Object() {0, "x"c, 1}, New Integer() {0, 1}, New Boolean() {False, False})
			assertTrue(New Long() {2, 1, 2}.SequenceEqual(twoOneTwo.shape()))

			Dim reverse As INDArray = n.dimShuffle(New Object() {1, "x"c, 0}, New Integer() {1, 0}, New Boolean() {False, False})
			assertTrue(New Long() {2, 1, 2}.SequenceEqual(reverse.shape()))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetVsGetScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetVsGetScalar(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim element As Single = a.getFloat(0, 1)
			Dim element2 As Double = a.getDouble(0, 1)
			assertEquals(element, element2, 1e-1)
			Dim a2 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim element23 As Single = a2.getFloat(0, 1)
			Dim element22 As Double = a2.getDouble(0, 1)
			assertEquals(element23, element22, 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDivide(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDivide(ByVal backend As Nd4jBackend)
			Dim two As INDArray = Nd4j.create(New Double() {2, 2, 2, 2}).castTo(DataType.DOUBLE)
			Dim div As INDArray = two.div(two)
			assertEquals(Nd4j.ones(4), div)

			Dim half As INDArray = Nd4j.create(New Double() {0.5f, 0.5f, 0.5f, 0.5f}, New Long() {2, 2})
			Dim divi As INDArray = Nd4j.create(New Double() {0.3f, 0.6f, 0.9f, 0.1f}, New Long() {2, 2})
			Dim assertion As INDArray = Nd4j.create(New Double() {1.6666666f, 0.8333333f, 0.5555556f, 5}, New Long() {2, 2})
			Dim result As INDArray = half.div(divi)
			assertEquals(assertion, result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSigmoid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSigmoid(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Single() {0.73105858f, 0.88079708f, 0.95257413f, 0.98201379f}).castTo(DataType.DOUBLE)
			Dim sigmoid As INDArray = Transforms.sigmoid(n, False)
			assertEquals(assertion, sigmoid)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNeg(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNeg(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Single() {-1, -2, -3, -4}).castTo(DataType.DOUBLE)
			Dim neg As INDArray = Transforms.neg(n)
			assertEquals(assertion, neg,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2Double(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2Double(ByVal backend As Nd4jBackend)
			Dim initialType As DataType = Nd4j.dataType()

			Dim n As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim assertion As Double = 5.47722557505
			Dim norm3 As Double = n.norm2Number().doubleValue()
			assertEquals(assertion, norm3, 1e-1,getFailureMessage(backend))

			Dim row As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}, New Long() {2, 2}).castTo(DataType.DOUBLE)
			Dim row1 As INDArray = row.getRow(1)
			Dim norm2 As Double = row1.norm2Number().doubleValue()
			Dim assertion2 As Double = 5.0f
			assertEquals(assertion2, norm2, 1e-1,getFailureMessage(backend))

			Nd4j.DataType = initialType
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim assertion As Single = 5.47722557505f
			Dim norm3 As Single = n.norm2Number().floatValue()
			assertEquals(assertion, norm3, 1e-1,getFailureMessage(backend))


			Dim row As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}, New Long() {2, 2}).castTo(DataType.DOUBLE)
			Dim row1 As INDArray = row.getRow(1)
			Dim norm2 As Single = row1.norm2Number().floatValue()
			Dim assertion2 As Single = 5.0f
			assertEquals(assertion2, norm2, 1e-1,getFailureMessage(backend))

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosineSim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCosineSim(ByVal backend As Nd4jBackend)
			Dim vec1 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim vec2 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim sim As Double = Transforms.cosineSim(vec1, vec2)
			assertEquals(1, sim, 1e-1,getFailureMessage(backend))

			Dim vec3 As INDArray = Nd4j.create(New Single() {0.2f, 0.3f, 0.4f, 0.5f})
			Dim vec4 As INDArray = Nd4j.create(New Single() {0.6f, 0.7f, 0.8f, 0.9f})
			sim = Transforms.cosineSim(vec3, vec4)
			assertEquals(0.98, sim, 1e-1)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScal(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScal(ByVal backend As Nd4jBackend)
			Dim assertion As Double = 2
			Dim answer As INDArray = Nd4j.create(New Double() {2, 4, 6, 8}).castTo(DataType.DOUBLE)
			Dim scal As INDArray = Nd4j.BlasWrapper.scal(assertion, answer)
			assertEquals(answer, scal,getFailureMessage(backend))

			Dim row As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}, New Long() {2, 2})
			Dim row1 As INDArray = row.getRow(1)
			Dim assertion2 As Double = 5.0
			Dim answer2 As INDArray = Nd4j.create(New Double() {15, 20})
			Dim scal2 As INDArray = Nd4j.BlasWrapper.scal(assertion2, row1)
			assertEquals(answer2, scal2,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExp(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}).castTo(DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Double() {2.71828183f, 7.3890561f, 20.08553692f, 54.59815003f}).castTo(DataType.DOUBLE)
			Dim exped As INDArray = Transforms.exp(n)
			assertEquals(assertion, exped)

			assertArrayEquals(New Double() {2.71828183f, 7.3890561f, 20.08553692f, 54.59815003f}, exped.toDoubleVector(), 1e-5)
			assertArrayEquals(New Double() {2.71828183f, 7.3890561f, 20.08553692f, 54.59815003f}, assertion.toDoubleVector(), 1e-5)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlices(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(Nd4j.linspace(1, 24, 24, DataType.DOUBLE).data(), New Long() {4, 3, 2})
			Dim i As Integer = 0
			Do While i < arr.slices()
				assertEquals(2, arr.slice(i).slice(1).slices())
				i += 1
			Loop

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalar(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.scalar(1.0f).castTo(DataType.DOUBLE)
			assertEquals(True, a.Scalar)

			Dim n As INDArray = Nd4j.create(New Single() {1.0f}, New Long(){}).castTo(DataType.DOUBLE)
			assertEquals(n, a)
			assertTrue(n.Scalar)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWrap(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWrap(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {2, 4}
			Dim d As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(shape(0)), shape(1))
			Dim n As INDArray = d
			assertEquals(d.rows(), n.rows())
			assertEquals(d.columns(), n.columns())

			Dim vector As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE)
			Dim testVector As INDArray = vector
			For i As Integer = 0 To vector.length() - 1
				assertEquals(vector.getDouble(i), testVector.getDouble(i), 1e-1)
			Next i
			assertEquals(3, testVector.length())
			assertEquals(True, testVector.Vector)
			assertEquals(True, Shape.shapeEquals(New Long() {3}, testVector.shape()))

			Dim row12 As INDArray = Nd4j.linspace(1, 2, 2, DataType.DOUBLE).reshape(ChrW(2), 1)
			Dim row22 As INDArray = Nd4j.linspace(3, 4, 2, DataType.DOUBLE).reshape(ChrW(1), 2)

			assertEquals(row12.rows(), 2)
			assertEquals(row12.columns(), 1)
			assertEquals(row22.rows(), 1)
			assertEquals(row22.columns(), 2)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorInit(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorInit(ByVal backend As Nd4jBackend)
			Dim data As DataBuffer = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data()
			Dim arr As INDArray = Nd4j.create(data, New Long() {1, 4})
			assertEquals(True, arr.RowVector)
			Dim arr2 As INDArray = Nd4j.create(data, New Long() {1, 4})
			assertEquals(True, arr2.RowVector)

			Dim columnVector As INDArray = Nd4j.create(data, New Long() {4, 1})
			assertEquals(True, columnVector.ColumnVector)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumns(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Long() {3, 2}).castTo(DataType.DOUBLE)
			Dim column2 As INDArray = arr.getColumn(0)
			'assertEquals(true, Shape.shapeEquals(new long[]{3, 1}, column2.shape()));
			Dim column As INDArray = Nd4j.create(New Double() {1, 2, 3}, New Long() {3})
			arr.putColumn(0, column)

			Dim firstColumn As INDArray = arr.getColumn(0)

			assertEquals(column, firstColumn)


			Dim column1 As INDArray = Nd4j.create(New Double() {4, 5, 6}, New Long() {3})
			arr.putColumn(1, column1)
			'assertEquals(true, Shape.shapeEquals(new long[]{3, 1}, arr.getColumn(1).shape()));
			Dim testRow1 As INDArray = arr.getColumn(1)
			assertEquals(column1, testRow1)


			Dim evenArr As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}, New Long() {2, 2})
			Dim put As INDArray = Nd4j.create(New Double() {5, 6}, New Long() {2})
			evenArr.putColumn(1, put)
			Dim testColumn As INDArray = evenArr.getColumn(1)
			assertEquals(put, testColumn)


			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data(), New Long() {2, 2})
			Dim column23 As INDArray = n.getColumn(0)
			Dim column12 As INDArray = Nd4j.create(New Double() {1, 3}, New Long() {2})
			assertEquals(column23, column12)


			Dim column0 As INDArray = n.getColumn(1)
			Dim column01 As INDArray = Nd4j.create(New Double() {2, 4}, New Long() {2})
			assertEquals(column0, column01)


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRow(ByVal backend As Nd4jBackend)
			Dim d As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim slice1 As INDArray = d.slice(1)
			Dim n As INDArray = d.dup()

			'works fine according to matlab, let's go with it..
			'reproduce with:  A = newShapeNoCopy(linspace(1,4,4),[2 2 ]);
			'A(1,2) % 1 index based
			Dim nFirst As Single = 2
			Dim dFirst As Single = d.getFloat(0, 1)
			assertEquals(nFirst, dFirst, 1e-1)
			assertEquals(d, n)
			assertEquals(True, New Long() {2, 2}.SequenceEqual(n.shape()))

			Dim newRow As INDArray = Nd4j.linspace(5, 6, 2, DataType.DOUBLE)
			n.putRow(0, newRow)
			d.putRow(0, newRow)


			Dim testRow As INDArray = n.getRow(0)
			assertEquals(newRow.length(), testRow.length())
			assertEquals(True, Shape.shapeEquals(New Long() {1, 2}, testRow.shape()))


			Dim nLast As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data(), New Long() {2, 2})
			Dim row As INDArray = nLast.getRow(1)
			Dim row1 As INDArray = Nd4j.create(New Double() {3, 4})
			assertEquals(row, row1)


			Dim arr As INDArray = Nd4j.create(New Long() {3, 2})
			Dim evenRow As INDArray = Nd4j.create(New Double() {1, 2})
			arr.putRow(0, evenRow)
			Dim firstRow As INDArray = arr.getRow(0)
			assertEquals(True, Shape.shapeEquals(New Long() {2}, firstRow.shape()))
			Dim testRowEven As INDArray = arr.getRow(0)
			assertEquals(evenRow, testRowEven)


			Dim row12 As INDArray = Nd4j.create(New Double() {5, 6}, New Long() {2})
			arr.putRow(1, row12)
			assertEquals(True, Shape.shapeEquals(New Long() {2}, arr.getRow(0).shape()))
			Dim testRow1 As INDArray = arr.getRow(1)
			assertEquals(row12, testRow1)


			Dim multiSliceTest As INDArray = Nd4j.create(Nd4j.linspace(1, 16, 16, DataType.DOUBLE).data(), New Long() {4, 2, 2})
			Dim test As INDArray = Nd4j.create(New Double() {5, 6}, New Long() {2})
			Dim test2 As INDArray = Nd4j.create(New Double() {7, 8}, New Long() {2})

			Dim multiSliceRow1 As INDArray = multiSliceTest.slice(1).getRow(0)
			Dim multiSliceRow2 As INDArray = multiSliceTest.slice(1).getRow(1)

			assertEquals(test, multiSliceRow1)
			assertEquals(test2, multiSliceRow2)



			Dim threeByThree As INDArray = Nd4j.create(3, 3)
			Dim threeByThreeRow1AndTwo As INDArray = threeByThree.get(NDArrayIndex.interval(1, 3), NDArrayIndex.all())
			threeByThreeRow1AndTwo.putRow(1, Nd4j.ones(3))
			assertEquals(Nd4j.ones(3), threeByThreeRow1AndTwo.getRow(1))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMulRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMulRowVector(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			arr.muliRowVector(Nd4j.linspace(1, 2, 2, DataType.DOUBLE))
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 4},
				New Double() {3, 8}
			})

			assertEquals(assertion, arr)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 8, 8, DataType.DOUBLE).data(), New Long() {2, 2, 2})
			Dim test As INDArray = Nd4j.create(New Double() {3, 7, 11, 15}, New Long() {2, 2})
			Dim sum As INDArray = n.sum(-1)
			assertEquals(test, sum)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInplaceTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInplaceTranspose(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.rand(3, 4).castTo(DataType.DOUBLE)
			Dim orig As INDArray = test.dup()
			Dim transposei As INDArray = test.transposei()

			Dim i As Integer = 0
			Do While i < orig.rows()
				Dim j As Integer = 0
				Do While j < orig.columns()
					assertEquals(orig.getDouble(i, j), transposei.getDouble(j, i), 1e-1)
					j += 1
				Loop
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTADMMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTADMMul(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim shape As val = New Long() {4, 5, 7}
			Dim arr As INDArray = Nd4j.rand(shape).castTo(DataType.DOUBLE)

			Dim tad As INDArray = arr.tensorAlongDimension(0, 1, 2)
			assertArrayEquals(tad.shape(), New Long() {5, 7})


			Dim copy As INDArray = Nd4j.zeros(5, 7).assign(0.0)
			For i As Integer = 0 To 4
				For j As Integer = 0 To 6
					copy.putScalar(New Long() {i, j}, tad.getDouble(i, j))
				Next j
			Next i


			assertTrue(tad.Equals(copy))
			tad = tad.reshape(ChrW(7), 5)
			copy = copy.reshape(ChrW(7), 5)
			Dim first As INDArray = Nd4j.rand(New Long() {2, 7}).castTo(DataType.DOUBLE)
			Dim mmul As INDArray = first.mmul(tad)
			Dim mmulCopy As INDArray = first.mmul(copy)

			assertEquals(mmul, mmulCopy)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTADMMulLeadingOne(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTADMMulLeadingOne(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim shape As val = New Long() {1, 5, 7}
			Dim arr As INDArray = Nd4j.rand(shape).castTo(DataType.DOUBLE)

			Dim tad As INDArray = arr.tensorAlongDimension(0, 1, 2)
			Dim order As Boolean = Shape.cOrFortranOrder(tad.shape(), tad.stride(), 1)
			assertArrayEquals(tad.shape(), New Long() {5, 7})


			Dim copy As INDArray = Nd4j.zeros(5, 7).castTo(DataType.DOUBLE)
			For i As Integer = 0 To 4
				For j As Integer = 0 To 6
					copy.putScalar(New Long() {i, j}, tad.getDouble(i, j))
				Next j
			Next i

			assertTrue(tad.Equals(copy))

			tad = tad.reshape(ChrW(7), 5)
			copy = copy.reshape(ChrW(7), 5)
			Dim first As INDArray = Nd4j.rand(New Long() {2, 7}).castTo(DataType.DOUBLE)
			Dim mmul As INDArray = first.mmul(tad)
			Dim mmulCopy As INDArray = first.mmul(copy)

			assertTrue(mmul.Equals(mmulCopy))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum2(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}, New Long() {2, 2}).castTo(DataType.DOUBLE)
			Dim sum As INDArray = test.sum(1)
			Dim assertion As INDArray = Nd4j.create(New Single() {3, 7}).castTo(DataType.DOUBLE)
			assertEquals(assertion, sum)
			Dim sum0 As INDArray = Nd4j.create(New Single() {4, 6}).castTo(DataType.DOUBLE)
			assertEquals(sum0, test.sum(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetIntervalEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetIntervalEdgeCase(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim shape() As Integer = {3, 2, 4}
			Dim arr3d As INDArray = Nd4j.rand(shape).castTo(DataType.DOUBLE)

			Dim get0 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 1))
			Dim getPoint0 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0))
			get0 = get0.reshape(getPoint0.shape())
			Dim tad0 As INDArray = arr3d.tensorAlongDimension(0, 1, 0)

			assertTrue(get0.Equals(getPoint0)) 'OK
			assertTrue(getPoint0.Equals(tad0)) 'OK

			Dim get1 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(1, 2))
			Dim getPoint1 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(1))
			get1 = get1.reshape(getPoint1.shape())
			Dim tad1 As INDArray = arr3d.tensorAlongDimension(1, 1, 0)

			assertTrue(getPoint1.Equals(tad1)) 'OK
			assertTrue(get1.Equals(getPoint1)) 'Fails
			assertTrue(get1.Equals(tad1))

			Dim get2 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(2, 3))
			Dim getPoint2 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(2))
			get2 = get2.reshape(getPoint2.shape())
			Dim tad2 As INDArray = arr3d.tensorAlongDimension(2, 1, 0)

			assertTrue(getPoint2.Equals(tad2)) 'OK
			assertTrue(get2.Equals(getPoint2)) 'Fails
			assertTrue(get2.Equals(tad2))

			Dim get3 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(3, 4))
			Dim getPoint3 As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(3))
			get3 = get3.reshape(getPoint3.shape())
			Dim tad3 As INDArray = arr3d.tensorAlongDimension(3, 1, 0)

			assertTrue(getPoint3.Equals(tad3)) 'OK
			assertTrue(get3.Equals(getPoint3)) 'Fails
			assertTrue(get3.Equals(tad3))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetIntervalEdgeCase2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetIntervalEdgeCase2(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim shape() As Integer = {3, 2, 4}
			Dim arr3d As INDArray = Nd4j.rand(shape).castTo(DataType.DOUBLE)

			For x As Integer = 0 To 3
				Dim getInterval As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(x, x + 1)) '3d
				Dim getPoint As INDArray = arr3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(x)) '2d
				Dim tad As INDArray = arr3d.tensorAlongDimension(x, 1, 0) '2d

				assertEquals(getPoint, tad)
				'assertTrue(getPoint.equals(tad));   //OK, comparing 2d with 2d
				assertArrayEquals(getInterval.shape(), New Long() {3, 2, 1})
				For i As Integer = 0 To 2
					For j As Integer = 0 To 1
						assertEquals(getInterval.getDouble(i, j, 0), getPoint.getDouble(i, j), 1e-1)
					Next j
				Next i
			Next x
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmul(ByVal backend As Nd4jBackend)
			Dim data As DataBuffer = Nd4j.linspace(1, 10, 10, DataType.DOUBLE).data()
			Dim n As INDArray = Nd4j.create(data, New Long() {1, 10}).castTo(DataType.DOUBLE)
			Dim transposed As INDArray = n.transpose()
			assertEquals(True, n.RowVector)
			assertEquals(True, transposed.ColumnVector)

			Dim d As INDArray = Nd4j.create(n.rows(), n.columns()).castTo(DataType.DOUBLE)
			d.Data = n.data()


			Dim d3 As INDArray = Nd4j.create(New Double() {1, 2}).reshape(ChrW(2), 1)
			Dim d4 As INDArray = Nd4j.create(New Double() {3, 4}).reshape(ChrW(1), 2)
			Dim resultNDArray As INDArray = d3.mmul(d4)
			Dim result As INDArray = Nd4j.create(New Double()() {
				New Double() {3, 4},
				New Double() {6, 8}
			}).castTo(DataType.DOUBLE)
			assertEquals(result, resultNDArray)


			Dim innerProduct As INDArray = n.mmul(transposed)

			Dim scalar As INDArray = Nd4j.scalar(385.0).reshape(ChrW(1), 1)
			assertEquals(scalar, innerProduct,getFailureMessage(backend))

			Dim outerProduct As INDArray = transposed.mmul(n)
			assertEquals(True, Shape.shapeEquals(New Long() {10, 10}, outerProduct.shape()),getFailureMessage(backend))



			Dim three As INDArray = Nd4j.create(New Double() {3, 4}).castTo(DataType.DOUBLE)
			Dim test As INDArray = Nd4j.create(Nd4j.linspace(1, 30, 30, DataType.DOUBLE).data(), New Long() {3, 5, 2})
			Dim sliceRow As INDArray = test.slice(0).getRow(1)
			assertEquals(three, sliceRow,getFailureMessage(backend))

			Dim twoSix As INDArray = Nd4j.create(New Double() {2, 6}, New Long() {2, 1}).castTo(DataType.DOUBLE)
			Dim threeTwoSix As INDArray = three.mmul(twoSix)

			Dim sliceRowTwoSix As INDArray = sliceRow.mmul(twoSix)

			assertEquals(threeTwoSix, sliceRowTwoSix)


			Dim vectorVector As INDArray = Nd4j.create(New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36, 39, 42, 45, 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 44, 48, 52, 56, 60, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 0, 6, 12, 18, 24, 30, 36, 42, 48, 54, 60, 66, 72, 78, 84, 90, 0, 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105, 0, 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96, 104, 112, 120, 0, 9, 18, 27, 36, 45, 54, 63, 72, 81, 90, 99, 108, 117, 126, 135, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 0, 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 121, 132, 143, 154, 165, 0, 12, 24, 36, 48, 60, 72, 84, 96, 108, 120, 132, 144, 156, 168, 180, 0, 13, 26, 39, 52, 65, 78, 91, 104, 117, 130, 143, 156, 169, 182, 195, 0, 14, 28, 42, 56, 70, 84, 98, 112, 126, 140, 154, 168, 182, 196, 210, 0, 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180, 195, 210, 225}, New Long() {16, 16}).castTo(DataType.DOUBLE)


			Dim n1 As INDArray = Nd4j.create(Nd4j.linspace(0, 15, 16, DataType.DOUBLE).data(), New Long() {1, 16})
			Dim k1 As INDArray = n1.transpose()

			Dim testVectorVector As INDArray = k1.mmul(n1)
			assertEquals(vectorVector, testVectorVector,getFailureMessage(backend))


		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowsColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowsColumns(ByVal backend As Nd4jBackend)
			Dim data As DataBuffer = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).data()
			Dim rows As INDArray = Nd4j.create(data, New Long() {2, 3})
			assertEquals(2, rows.rows())
			assertEquals(3, rows.columns())

			Dim columnVector As INDArray = Nd4j.create(data, New Long() {6, 1})
			assertEquals(6, columnVector.rows())
			assertEquals(1, columnVector.columns())
			Dim rowVector As INDArray = Nd4j.create(data, New Long() {1, 6})
			assertEquals(1, rowVector.rows())
			assertEquals(6, rowVector.columns())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTranspose(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.ones(100).data(), New Long() {5, 5, 4}).castTo(DataType.DOUBLE)
			Dim transpose As INDArray = n.transpose()
			assertEquals(n.length(), transpose.length())
			assertEquals(True, New Long() {4, 5, 5}.SequenceEqual(transpose.shape()))

			Dim rowVector As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE).reshape(ChrW(1), -1)
			assertTrue(rowVector.RowVector)
			Dim columnVector As INDArray = rowVector.transpose()
			assertTrue(columnVector.ColumnVector)


			Dim linspaced As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim transposed As INDArray = Nd4j.create(New Double() {1, 3, 2, 4}, New Long() {2, 2})
			Dim linSpacedT As INDArray = linspaced.transpose()
			assertEquals(transposed, linSpacedT)



		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogX1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogX1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(10).assign(7).castTo(DataType.DOUBLE)

			Dim logX5 As INDArray = Transforms.log(x, 5, True)

			Dim exp As INDArray = Transforms.log(x, True).div(Transforms.log(Nd4j.create(10).assign(5)))

			assertEquals(exp, logX5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddMatrix(ByVal backend As Nd4jBackend)
			Dim five As INDArray = Nd4j.ones(5).castTo(DataType.DOUBLE)
			five.addi(five)
			Dim twos As INDArray = Nd4j.valueArrayOf(5, 2)
			assertEquals(twos, five)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutSlice(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(1, 27, 27, DataType.DOUBLE).reshape(ChrW(3), 3, 3)
			Dim newSlice As INDArray = Nd4j.zeros(3, 3)
			n.putSlice(0, newSlice)
			assertEquals(newSlice, n.slice(0))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowVectorMultipleIndices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowVectorMultipleIndices(ByVal backend As Nd4jBackend)
			Dim linear As INDArray = Nd4j.create(1, 4).castTo(DataType.DOUBLE)
			linear.putScalar(New Long() {0, 1}, 1)
			assertEquals(linear.getDouble(0, 1), 1, 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSize(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim arr As INDArray = Nd4j.create(4, 5).castTo(DataType.DOUBLE)
			For i As Integer = 0 To 5
				arr.size(i)
			Next i
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNullPointerDataBuffer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNullPointerDataBuffer(ByVal backend As Nd4jBackend)
			Dim allocate As ByteBuffer = ByteBuffer.allocateDirect(10 * 4).order(ByteOrder.nativeOrder())
			allocate.asFloatBuffer().put(New Single() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})
			Dim buff As DataBuffer = Nd4j.createBuffer(allocate, DataType.FLOAT, 10)
			Dim sum As Single = Nd4j.create(buff).sumNumber().floatValue()
	'        System.out.println(sum);
			assertEquals(55f, sum, 0.001f)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEps(ByVal backend As Nd4jBackend)
			Dim ones As INDArray = Nd4j.ones(5)
			Dim res As val = Nd4j.create(DataType.BOOL, 5)
			Nd4j.Executioner.exec(New Eps(ones, ones, res))

	'        log.info("Result: {}", res);
			assertTrue(res.all())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEps2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEps2(ByVal backend As Nd4jBackend)

			Dim first As INDArray = Nd4j.valueArrayOf(10, 1e-2).castTo(DataType.DOUBLE) '0.01
			Dim second As INDArray = Nd4j.zeros(10).castTo(DataType.DOUBLE) '0.0

			Dim expAllZeros1 As INDArray = Nd4j.Executioner.exec(New Eps(first, second, Nd4j.create(DataType.BOOL, New Long() {1, 10}, "f"c)))
			Dim expAllZeros2 As INDArray = Nd4j.Executioner.exec(New Eps(second, first, Nd4j.create(DataType.BOOL, New Long() {1, 10}, "f"c)))

	'        System.out.println(expAllZeros1);
	'        System.out.println(expAllZeros2);

			assertTrue(expAllZeros1.none())
			assertTrue(expAllZeros2.none())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogDouble(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogDouble(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim log As INDArray = Transforms.log(linspace)
			Dim assertion As INDArray = Nd4j.create(New Double() {0, 0.6931471805599453, 1.0986122886681098, 1.3862943611198906, 1.6094379124341005, 1.791759469228055})
			assertEquals(assertion, log)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupDimension(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			assertEquals(arr.tensorAlongDimension(0, 1), arr.tensorAlongDimension(0, 1))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIterator(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIterator(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim repeated As INDArray = x.repeat(1, 2)
			assertEquals(8, repeated.length())
			Dim arrayIter As IEnumerator(Of Double) = New INDArrayIterator(x)
			Dim vals() As Double = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data().asDouble()
			For i As Integer = 0 To vals.Length - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertEquals(vals(i), arrayIter.next().doubleValue(), 1e-1)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTile(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTile(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim repeated As INDArray = x.repeat(0, 2)
			assertEquals(8, repeated.length())
			Dim repeatAlongDimension As INDArray = x.repeat(1, New Long() {2})
			Dim assertionRepeat As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 2, 2},
				New Double() {3, 3, 4, 4}
			})
			assertArrayEquals(New Long() {2, 4}, assertionRepeat.shape())
			assertEquals(assertionRepeat, repeatAlongDimension)
	'        System.out.println(repeatAlongDimension);
			Dim ret As INDArray = Nd4j.create(New Double() {0, 1, 2}).reshape(ChrW(1), 3)
			Dim tile As INDArray = Nd4j.tile(ret, 2, 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 0, 1, 2},
				New Double() {0, 1, 2, 0, 1, 2}
			})
			assertEquals(assertion, tile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNegativeOneReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNegativeOneReshape(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {0, 1, 2})
			Dim newShape As INDArray = arr.reshape(ChrW(-1))
			assertEquals(newShape, arr)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSmallSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSmallSum(ByVal backend As Nd4jBackend)
			Dim base As INDArray = Nd4j.create(New Double() {5.843333333333335, 3.0540000000000007})
			base.addi(1e-12)
			Dim assertion As INDArray = Nd4j.create(New Double() {5.84333433, 3.054001})
			assertEquals(assertion, base)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2DArraySlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2DArraySlice(ByVal backend As Nd4jBackend)
			Dim array2D As INDArray = Nd4j.ones(5, 7).castTo(DataType.DOUBLE)
			''' <summary>
			''' This should be reverse.
			''' This is compatibility with numpy.
			''' 
			''' If you do numpy.sum along dimension
			''' 1 you will find its row sums.
			''' 
			''' 0 is columns sums.
			''' 
			''' slice(0,axis)
			''' should be consistent with this behavior
			''' </summary>
			For i As Integer = 0 To 6
				Dim slice As INDArray = array2D.slice(i, 1)
				assertArrayEquals(slice.shape(), New Long() {5})
			Next i

			For i As Integer = 0 To 4
				Dim slice As INDArray = array2D.slice(i, 0)
				assertArrayEquals(slice.shape(), New Long(){7})
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testTensorDot(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorDot(ByVal backend As Nd4jBackend)
			Dim oneThroughSixty As INDArray = Nd4j.arange(60).reshape(ChrW(3), 4, 5).castTo(DataType.DOUBLE)
			Dim oneThroughTwentyFour As INDArray = Nd4j.arange(24).reshape(ChrW(4), 3, 2).castTo(DataType.DOUBLE)
			Dim result As INDArray = Nd4j.tensorMmul(oneThroughSixty, oneThroughTwentyFour, New Integer()() {
				New Integer() {1, 0},
				New Integer() {0, 1}
			})
			assertArrayEquals(New Long() {5, 2}, result.shape())
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4400, 4730},
				New Double() {4532, 4874},
				New Double() {4664, 5018},
				New Double() {4796, 5162},
				New Double() {4928, 5306}
			})
			assertEquals(assertion, result)

			Dim w As INDArray = Nd4j.valueArrayOf(New Long() {2, 1, 2, 2}, 0.5)
			Dim col As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4}, New Long() {1, 1, 2, 2, 4, 4})

			Dim test As INDArray = Nd4j.tensorMmul(col, w, New Integer()() {
				New Integer() {1, 2, 3},
				New Integer() {1, 2, 3}
			})
			Dim assertion2 As INDArray = Nd4j.create(New Double() {3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 7.0, 7.0, 7.0, 7.0, 7.0, 7.0, 7.0, 7.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 7.0, 7.0, 7.0, 7.0, 7.0, 7.0, 7.0, 7.0}, New Long() {1, 4, 4, 2}, New Long() {16, 8, 2, 1}, "f"c, DataType.DOUBLE)

			assertEquals(assertion2, test)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRow(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(10, 4).castTo(DataType.DOUBLE)
			For i As Integer = 0 To 9
				Dim row As INDArray = arr.getRow(i)
				assertArrayEquals(row.shape(), New Long() {4})
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetPermuteReshapeSub(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetPermuteReshapeSub(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim first As INDArray = Nd4j.rand(New Long() {10, 4}).castTo(DataType.DOUBLE)

			'Reshape, as per RnnOutputLayer etc on labels
			Dim orig3d As INDArray = Nd4j.rand(New Long() {2, 4, 15})
			Dim subset3d As INDArray = orig3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(5, 10))
			Dim permuted As INDArray = subset3d.permute(0, 2, 1)
			Dim newShape As val = New Long (){subset3d.size(0) * subset3d.size(2), subset3d.size(1)}
			Dim second As INDArray = permuted.reshape(newShape)

			assertArrayEquals(first.shape(), second.shape())
			assertEquals(first.length(), second.length())
			assertArrayEquals(first.stride(), second.stride())

			first.sub(second) 'Exception
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutAtIntervalIndexWithStride(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutAtIntervalIndexWithStride(ByVal backend As Nd4jBackend)
			Dim n1 As INDArray = Nd4j.create(3, 3).assign(0.0).castTo(DataType.DOUBLE)
			Dim indices() As INDArrayIndex = {NDArrayIndex.interval(0, 2, 3), NDArrayIndex.all()}
			n1.put(indices, 1)
			Dim expected As INDArray = Nd4j.create(New Double()() {
				New Double() {1R, 1R, 1R},
				New Double() {0R, 0R, 0R},
				New Double() {1R, 1R, 1R}
			})
			assertEquals(expected, n1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMulMatrixTimesColVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMulMatrixTimesColVector(ByVal backend As Nd4jBackend)
			'[1 1 1 1 1; 10 10 10 10 10; 100 100 100 100 100] x [1; 1; 1; 1; 1] = [5; 50; 500]
			Dim matrix As INDArray = Nd4j.ones(3, 5).castTo(DataType.DOUBLE)
			matrix.getRow(1).muli(10)
			matrix.getRow(2).muli(100)

			Dim colVector As INDArray = Nd4j.ones(5, 1).castTo(DataType.DOUBLE)
			Dim [out] As INDArray = matrix.mmul(colVector)

			Dim expected As INDArray = Nd4j.create(New Double() {5, 50, 500}, New Long() {3, 1})
			assertEquals(expected, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMulMixedOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMulMixedOrder(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.ones(5, 2).castTo(DataType.DOUBLE)
			Dim second As INDArray = Nd4j.ones(2, 3).castTo(DataType.DOUBLE)
			Dim [out] As INDArray = first.mmul(second)
			assertArrayEquals([out].shape(), New Long() {5, 3})
			assertTrue([out].Equals(Nd4j.ones(5, 3).muli(2)))
			'Above: OK

			Dim firstC As INDArray = Nd4j.create(New Long() {5, 2}, "c"c)
			Dim secondF As INDArray = Nd4j.create(New Long() {2, 3}, "f"c)
			Dim i As Integer = 0
			Do While i < firstC.length()
				firstC.putScalar(i, 1.0)
				i += 1
			Loop
			i = 0
			Do While i < secondF.length()
				secondF.putScalar(i, 1.0)
				i += 1
			Loop
			assertTrue(first.Equals(firstC))
			assertTrue(second.Equals(secondF))

			Dim outCF As INDArray = firstC.mmul(secondF)
			assertArrayEquals(outCF.shape(), New Long() {5, 3})
			assertEquals(outCF, Nd4j.ones(5, 3).muli(2))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFTimesCAddiRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFTimesCAddiRow(ByVal backend As Nd4jBackend)

			Dim arrF As INDArray = Nd4j.create(2, 3, "f"c).assign(1.0).castTo(DataType.DOUBLE)
			Dim arrC As INDArray = Nd4j.create(2, 3, "c"c).assign(1.0).castTo(DataType.DOUBLE)
			Dim arr2 As INDArray = Nd4j.create(New Long() {3, 4}, "c"c).assign(1.0).castTo(DataType.DOUBLE)

			Dim mmulC As INDArray = arrC.mmul(arr2) '[2,4] with elements 3.0
			Dim mmulF As INDArray = arrF.mmul(arr2) '[2,4] with elements 3.0
			assertArrayEquals(mmulC.shape(), New Long() {2, 4})
			assertArrayEquals(mmulF.shape(), New Long() {2, 4})
			assertTrue(arrC.Equals(arrF))

			Dim row As INDArray = Nd4j.zeros(1, 4).assign(0.0).addi(0.5).castTo(DataType.DOUBLE)
			mmulC.addiRowVector(row) 'OK
			mmulF.addiRowVector(row) 'Exception

			assertTrue(mmulC.Equals(mmulF))

			For i As Integer = 0 To mmulC.length() - 1
				assertEquals(mmulC.getDouble(i), 3.5, 1e-1) 'OK
			Next i
			For i As Integer = 0 To mmulF.length() - 1
				assertEquals(mmulF.getDouble(i), 3.5, 1e-1) 'Exception
			Next i
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulGet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulGet(ByVal backend As Nd4jBackend)
			Nd4j.Random.Seed = 12345L
			Dim elevenByTwo As INDArray = Nd4j.rand(New Long() {11, 2}).castTo(DataType.DOUBLE)
			Dim twoByEight As INDArray = Nd4j.rand(New Long() {2, 8}).castTo(DataType.DOUBLE)

			Dim view As INDArray = twoByEight.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 2))
			Dim viewCopy As INDArray = view.dup()
			assertTrue(view.Equals(viewCopy))

			Dim mmul1 As INDArray = elevenByTwo.mmul(view)
			Dim mmul2 As INDArray = elevenByTwo.mmul(viewCopy)

			assertTrue(mmul1.Equals(mmul2))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMulRowColVectorMixedOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMulRowColVectorMixedOrder(ByVal backend As Nd4jBackend)
			Dim colVec As INDArray = Nd4j.ones(5, 1).castTo(DataType.DOUBLE)
			Dim rowVec As INDArray = Nd4j.ones(1, 3).castTo(DataType.DOUBLE)
			Dim [out] As INDArray = colVec.mmul(rowVec)
			assertArrayEquals([out].shape(), New Long() {5, 3})
			assertTrue([out].Equals(Nd4j.ones(5, 3)))
			'Above: OK

			Dim colVectorC As INDArray = Nd4j.create(New Long() {5, 1}, "c"c).castTo(DataType.DOUBLE)
			Dim rowVectorF As INDArray = Nd4j.create(New Long() {1, 3}, "f"c).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < colVectorC.length()
				colVectorC.putScalar(i, 1.0)
				i += 1
			Loop
			i = 0
			Do While i < rowVectorF.length()
				rowVectorF.putScalar(i, 1.0)
				i += 1
			Loop
			assertTrue(colVec.Equals(colVectorC))
			assertTrue(rowVec.Equals(rowVectorF))

			Dim outCF As INDArray = colVectorC.mmul(rowVectorF)
			assertArrayEquals(outCF.shape(), New Long() {5, 3})
			assertEquals(outCF, Nd4j.ones(5, 3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMulFTimesC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMulFTimesC(ByVal backend As Nd4jBackend)
			Dim nRows As Integer = 3
			Dim nCols As Integer = 3
			Dim r As New Random(12345)

			Dim arrC As INDArray = Nd4j.create(New Long() {nRows, nCols}, "c"c).castTo(DataType.DOUBLE)
			Dim arrF As INDArray = Nd4j.create(New Long() {nRows, nCols}, "f"c).castTo(DataType.DOUBLE)
			Dim arrC2 As INDArray = Nd4j.create(New Long() {nRows, nCols}, "c"c).castTo(DataType.DOUBLE)
			For i As Integer = 0 To nRows - 1
				For j As Integer = 0 To nCols - 1
					Dim rv As Double = r.NextDouble()
					arrC.putScalar(New Long() {i, j}, rv)
					arrF.putScalar(New Long() {i, j}, rv)
					arrC2.putScalar(New Long() {i, j}, r.NextDouble())
				Next j
			Next i
			assertTrue(arrF.Equals(arrC))

			Dim fTimesC As INDArray = arrF.mmul(arrC2)
			Dim cTimesC As INDArray = arrC.mmul(arrC2)

			assertEquals(fTimesC, cTimesC)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMulColVectorRowVectorMixedOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMulColVectorRowVectorMixedOrder(ByVal backend As Nd4jBackend)
			Dim colVec As INDArray = Nd4j.ones(5, 1).castTo(DataType.DOUBLE)
			Dim rowVec As INDArray = Nd4j.ones(1, 5).castTo(DataType.DOUBLE)
			Dim [out] As INDArray = rowVec.mmul(colVec)
			assertArrayEquals(New Long() {1, 1}, [out].shape())
			assertTrue([out].Equals(Nd4j.ones(1, 1).muli(5)))

			Dim colVectorC As INDArray = Nd4j.create(New Long() {5, 1}, "c"c)
			Dim rowVectorF As INDArray = Nd4j.create(New Long() {1, 5}, "f"c)
			Dim i As Integer = 0
			Do While i < colVectorC.length()
				colVectorC.putScalar(i, 1.0)
				i += 1
			Loop
			i = 0
			Do While i < rowVectorF.length()
				rowVectorF.putScalar(i, 1.0)
				i += 1
			Loop
			assertTrue(colVec.Equals(colVectorC))
			assertTrue(rowVec.Equals(rowVectorF))

			Dim outCF As INDArray = rowVectorF.mmul(colVectorC)
			assertArrayEquals(outCF.shape(), New Long() {1, 1})
			assertTrue(outCF.Equals(Nd4j.ones(1, 1).muli(5)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 20, 20, DataType.DOUBLE).data(), New Long() {5, 4}).castTo(DataType.DOUBLE)
			Dim transpose As INDArray = n.transpose()
			Dim permute As INDArray = n.permute(1, 0)
			assertEquals(permute, transpose)
			assertEquals(transpose.length(), permute.length(), 1e-1)


			Dim toPermute As INDArray = Nd4j.create(Nd4j.linspace(0, 7, 8, DataType.DOUBLE).data(), New Long() {2, 2, 2})
			Dim permuted As INDArray = toPermute.permute(2, 1, 0)
			Dim assertion As INDArray = Nd4j.create(New Double() {0, 4, 2, 6, 1, 5, 3, 7}, New Long() {2, 2, 2})
			assertEquals(permuted, assertion)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermutei(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermutei(ByVal backend As Nd4jBackend)
			'Check in-place permute vs. copy array permute

			'2d:
			Dim orig As INDArray = Nd4j.linspace(1, 3 * 4, 3 * 4, DataType.DOUBLE).reshape("c"c, 3, 4).castTo(DataType.DOUBLE)
			Dim exp01 As INDArray = orig.permute(0, 1)
			Dim exp10 As INDArray = orig.permute(1, 0)
			Dim list1 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 4, 12345, DataType.DOUBLE)
			Dim list2 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 4, 12345, DataType.DOUBLE)
			For i As Integer = 0 To list1.Count - 1
				Dim p1 As INDArray = list1(i).getFirst().assign(orig).permutei(0, 1)
				Dim p2 As INDArray = list2(i).getFirst().assign(orig).permutei(1, 0)

				assertEquals(exp01, p1)
				assertEquals(exp10, p2)

				assertEquals(3, p1.rows())
				assertEquals(4, p1.columns())

				assertEquals(4, p2.rows())
				assertEquals(3, p2.columns())
			Next i

			'2d, v2
			orig = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape("c"c, 1, 4)
			exp01 = orig.permute(0, 1)
			exp10 = orig.permute(1, 0)
			list1 = NDArrayCreationUtil.getAllTestMatricesWithShape(1, 4, 12345, DataType.DOUBLE)
			list2 = NDArrayCreationUtil.getAllTestMatricesWithShape(1, 4, 12345, DataType.DOUBLE)
			For i As Integer = 0 To list1.Count - 1
				Dim p1 As INDArray = list1(i).getFirst().assign(orig).permutei(0, 1)
				Dim p2 As INDArray = list2(i).getFirst().assign(orig).permutei(1, 0)

				assertEquals(exp01, p1)
				assertEquals(exp10, p2)

				assertEquals(1, p1.rows())
				assertEquals(4, p1.columns())
				assertEquals(4, p2.rows())
				assertEquals(1, p2.columns())
				assertTrue(p1.RowVector)
				assertFalse(p1.ColumnVector)
				assertFalse(p2.RowVector)
				assertTrue(p2.ColumnVector)
			Next i

			'3d:
			Dim orig3d As INDArray = Nd4j.linspace(1, 3 * 4 * 5, 3 * 4 * 5, DataType.DOUBLE).reshape("c"c, 3, 4, 5)
			Dim exp012 As INDArray = orig3d.permute(0, 1, 2)
			Dim exp021 As INDArray = orig3d.permute(0, 2, 1)
			Dim exp120 As INDArray = orig3d.permute(1, 2, 0)
			Dim exp102 As INDArray = orig3d.permute(1, 0, 2)
			Dim exp201 As INDArray = orig3d.permute(2, 0, 1)
			Dim exp210 As INDArray = orig3d.permute(2, 1, 0)

			Dim list012 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE)
			Dim list021 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE)
			Dim list120 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE)
			Dim list102 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE)
			Dim list201 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE)
			Dim list210 As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE)

			For i As Integer = 0 To list012.Count - 1
				Dim p1 As INDArray = list012(i).getFirst().assign(orig3d).permutei(0, 1, 2)
				Dim p2 As INDArray = list021(i).getFirst().assign(orig3d).permutei(0, 2, 1)
				Dim p3 As INDArray = list120(i).getFirst().assign(orig3d).permutei(1, 2, 0)
				Dim p4 As INDArray = list102(i).getFirst().assign(orig3d).permutei(1, 0, 2)
				Dim p5 As INDArray = list201(i).getFirst().assign(orig3d).permutei(2, 0, 1)
				Dim p6 As INDArray = list210(i).getFirst().assign(orig3d).permutei(2, 1, 0)

				assertEquals(exp012, p1)
				assertEquals(exp021, p2)
				assertEquals(exp120, p3)
				assertEquals(exp102, p4)
				assertEquals(exp201, p5)
				assertEquals(exp210, p6)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermuteiShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermuteiShape(ByVal backend As Nd4jBackend)

			Dim row As INDArray = Nd4j.create(1, 10).castTo(DataType.DOUBLE)

			Dim permutedCopy As INDArray = row.permute(1, 0)
			Dim permutedInplace As INDArray = row.permutei(1, 0)

			assertArrayEquals(New Long() {10, 1}, permutedCopy.shape())
			assertArrayEquals(New Long() {10, 1}, permutedInplace.shape())

			assertEquals(10, permutedCopy.rows())
			assertEquals(10, permutedInplace.rows())

			assertEquals(1, permutedCopy.columns())
			assertEquals(1, permutedInplace.columns())


			Dim col As INDArray = Nd4j.create(10, 1)
			Dim cPermutedCopy As INDArray = col.permute(1, 0)
			Dim cPermutedInplace As INDArray = col.permutei(1, 0)

			assertArrayEquals(New Long() {1, 10}, cPermutedCopy.shape())
			assertArrayEquals(New Long() {1, 10}, cPermutedInplace.shape())

			assertEquals(1, cPermutedCopy.rows())
			assertEquals(1, cPermutedInplace.rows())

			assertEquals(10, cPermutedCopy.columns())
			assertEquals(10, cPermutedInplace.columns())
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSwapAxes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSwapAxes(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(0, 7, 8, DataType.DOUBLE).data(), New Long() {2, 2, 2})
			Dim assertion As INDArray = n.permute(2, 1, 0)
			Dim permuteTranspose As INDArray = assertion.slice(1).slice(1)
			Dim validate As INDArray = Nd4j.create(New Double() {0, 4, 2, 6, 1, 5, 3, 7}, New Long() {2, 2, 2})
			assertEquals(validate, assertion)

			Dim thirty As INDArray = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape(ChrW(3), 5, 2)
			Dim swapped As INDArray = thirty.swapAxes(2, 1)
			Dim slice As INDArray = swapped.slice(0).slice(0)
			Dim assertion2 As INDArray = Nd4j.create(New Double() {1, 3, 5, 7, 9})
			assertEquals(assertion2, slice)


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMuliRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMuliRowVector(ByVal backend As Nd4jBackend)
			Dim arrC As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape("c"c, 3, 2)
			Dim arrF As INDArray = Nd4j.create(New Long() {3, 2}, "f"c).assign(arrC)

			Dim temp As INDArray = Nd4j.create(New Long() {2, 11}, "c"c)
			Dim vec As INDArray = temp.get(NDArrayIndex.all(), NDArrayIndex.interval(9, 10)).transpose()
			vec.assign(Nd4j.linspace(1, 2, 2, DataType.DOUBLE))

			'Passes if we do one of these...
			'        vec = vec.dup('c');
			'        vec = vec.dup('f');

	'        System.out.println("Vec: " + vec);

			Dim outC As INDArray = arrC.muliRowVector(vec)
			Dim outF As INDArray = arrF.muliRowVector(vec)

			Dim expD()() As Double = {
				New Double() {1, 4},
				New Double() {3, 8},
				New Double() {5, 12}
			}
			Dim exp As INDArray = Nd4j.create(expD)

			assertEquals(exp, outC)
			assertEquals(exp, outF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceConstructor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceConstructor(ByVal backend As Nd4jBackend)
			Dim testList As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 4
				testList.Add(Nd4j.scalar(i + 1.0f))
			Next i

			Dim test As INDArray = Nd4j.create(testList, New Long() {1, testList.Count}).reshape(ChrW(1), 5)
			Dim expected As INDArray = Nd4j.create(New Single() {1, 2, 3, 4, 5}, New Long() {1, 5})
			assertEquals(expected, test)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdev0(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdev0(ByVal backend As Nd4jBackend)
			Dim ind()() As Double = {
				New Double() {5.1, 3.5, 1.4},
				New Double() {4.9, 3.0, 1.4},
				New Double() {4.7, 3.2, 1.3}
			}
			Dim [in] As INDArray = Nd4j.create(ind)
			Dim stdev As INDArray = [in].std(0)
			Dim exp As INDArray = Nd4j.create(New Double() {0.19999999999999973, 0.2516611478423583, 0.057735026918962505})

			assertEquals(exp, stdev)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdev1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdev1(ByVal backend As Nd4jBackend)
			Dim ind()() As Double = {
				New Double() {5.1, 3.5, 1.4},
				New Double() {4.9, 3.0, 1.4},
				New Double() {4.7, 3.2, 1.3}
			}
			Dim [in] As INDArray = Nd4j.create(ind).castTo(DataType.DOUBLE)
			Dim stdev As INDArray = [in].std(1)
	'        log.info("StdDev: {}", stdev.toDoubleVector());
			Dim exp As INDArray = Nd4j.create(New Double() {1.8556220879622372, 1.7521415467935233, 1.7039170558842744})
			assertEquals(exp, stdev)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSignXZ(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSignXZ(ByVal backend As Nd4jBackend)
			Dim d() As Double = {1.0, -1.1, 1.2, 1.3, -1.4, -1.5, 1.6, -1.7, -1.8, -1.9, -1.01, -1.011}
			Dim e() As Double = {1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0}

			Dim arrF As INDArray = Nd4j.create(d, New Long() {4, 3}, "f"c).castTo(DataType.DOUBLE)
			Dim arrC As INDArray = Nd4j.create(New Long() {4, 3}, "c"c).assign(arrF).castTo(DataType.DOUBLE)

			Dim exp As INDArray = Nd4j.create(e, New Long() {4, 3}, "f"c)

			'First: do op with just x (inplace)
			Dim arrFCopy As INDArray = arrF.dup("f"c)
			Dim arrCCopy As INDArray = arrC.dup("c"c)
			Nd4j.Executioner.exec(New Sign(arrFCopy))
			Nd4j.Executioner.exec(New Sign(arrCCopy))
			assertEquals(exp, arrFCopy)
			assertEquals(exp, arrCCopy)

			'Second: do op with both x and z:
			Dim zOutFC As INDArray = Nd4j.create(New Long() {4, 3}, "c"c)
			Dim zOutFF As INDArray = Nd4j.create(New Long() {4, 3}, "f"c)
			Dim zOutCC As INDArray = Nd4j.create(New Long() {4, 3}, "c"c)
			Dim zOutCF As INDArray = Nd4j.create(New Long() {4, 3}, "f"c)
			Nd4j.Executioner.exec(New Sign(arrF, zOutFC))
			Nd4j.Executioner.exec(New Sign(arrF, zOutFF))
			Nd4j.Executioner.exec(New Sign(arrC, zOutCC))
			Nd4j.Executioner.exec(New Sign(arrC, zOutCF))

			assertEquals(exp, zOutFC) 'fails
			assertEquals(exp, zOutFF) 'pass
			assertEquals(exp, zOutCC) 'pass
			assertEquals(exp, zOutCF) 'fails
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTanhXZ(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTanhXZ(ByVal backend As Nd4jBackend)
			Dim arrC As INDArray = Nd4j.linspace(-6, 6, 12, DataType.DOUBLE).reshape("c"c, 4, 3)
			Dim arrF As INDArray = Nd4j.create(New Long() {4, 3}, "f"c).assign(arrC)
			Dim d() As Double = arrC.data().asDouble()
			Dim e(d.Length - 1) As Double
			For i As Integer = 0 To e.Length - 1
				e(i) = Math.Tanh(d(i))
			Next i

			Dim exp As INDArray = Nd4j.create(e, New Long() {4, 3}, "c"c)

			'First: do op with just x (inplace)
			Dim arrFCopy As INDArray = arrF.dup("f"c)
			Dim arrCCopy As INDArray = arrF.dup("c"c)
			Nd4j.Executioner.exec(New Tanh(arrFCopy))
			Nd4j.Executioner.exec(New Tanh(arrCCopy))
			assertEquals(exp, arrFCopy)
			assertEquals(exp, arrCCopy)

			'Second: do op with both x and z:
			Dim zOutFC As INDArray = Nd4j.create(New Long() {4, 3}, "c"c)
			Dim zOutFF As INDArray = Nd4j.create(New Long() {4, 3}, "f"c)
			Dim zOutCC As INDArray = Nd4j.create(New Long() {4, 3}, "c"c)
			Dim zOutCF As INDArray = Nd4j.create(New Long() {4, 3}, "f"c)
			Nd4j.Executioner.exec(New Tanh(arrF, zOutFC))
			Nd4j.Executioner.exec(New Tanh(arrF, zOutFF))
			Nd4j.Executioner.exec(New Tanh(arrC, zOutCC))
			Nd4j.Executioner.exec(New Tanh(arrC, zOutCF))

			assertEquals(exp, zOutFC) 'fails
			assertEquals(exp, zOutFF) 'pass
			assertEquals(exp, zOutCC) 'pass
			assertEquals(exp, zOutCF) 'fails
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastDiv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastDiv(ByVal backend As Nd4jBackend)
			Dim num As INDArray = Nd4j.create(New Double() {1.00, 1.00, 1.00, 1.00, 2.00, 2.00, 2.00, 2.00, 1.00, 1.00, 1.00, 1.00, 2.00, 2.00, 2.00, 2.00, -1.00, -1.00, -1.00, -1.00, -2.00, -2.00, -2.00, -2.00, -1.00, -1.00, -1.00, -1.00, -2.00, -2.00, -2.00, -2.00}).reshape(ChrW(2), 16)

			Dim denom As INDArray = Nd4j.create(New Double() {1.00, 1.00, 1.00, 1.00, 2.00, 2.00, 2.00, 2.00, 1.00, 1.00, 1.00, 1.00, 2.00, 2.00, 2.00, 2.00})

			Dim expected As INDArray = Nd4j.create(New Double() {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0}, New Long() {2, 16})

			Dim actual As INDArray = Nd4j.Executioner.exec(New BroadcastDivOp(num, denom, num.dup(), -1))
			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastDiv2()
		Public Overridable Sub testBroadcastDiv2()
			Dim arr As INDArray = Nd4j.ones(DataType.DOUBLE, 1, 64, 125, 125).muli(2)
			Dim vec As INDArray = Nd4j.ones(DataType.DOUBLE, 64).muli(2)

			Dim exp As INDArray = Nd4j.ones(DataType.DOUBLE, 1, 64, 125, 125)
			Dim [out] As INDArray = arr.like()

			For i As Integer = 0 To 9
				[out].assign(0.0)
				Nd4j.Executioner.exec(New BroadcastDivOp(arr, vec, [out], 1))
				assertEquals(exp, [out])
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastMult(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastMult(ByVal backend As Nd4jBackend)
			Dim num As INDArray = Nd4j.create(New Double() {1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00, -1.00, -2.00, -3.00, -4.00, -5.00, -6.00, -7.00, -8.00}).reshape(ChrW(2), 8)

			Dim denom As INDArray = Nd4j.create(New Double() {1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00})

			Dim expected As INDArray = Nd4j.create(New Double() {1, 4, 9, 16, 25, 36, 49, 64, -1, -4, -9, -16, -25, -36, -49, -64}, New Long() {2, 8})

			Dim actual As INDArray = Nd4j.Executioner.exec(New BroadcastMulOp(num, denom, num.dup(), -1))
			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastSub(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastSub(ByVal backend As Nd4jBackend)
			Dim num As INDArray = Nd4j.create(New Double() {1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00, -1.00, -2.00, -3.00, -4.00, -5.00, -6.00, -7.00, -8.00}).reshape(ChrW(2), 8)

			Dim denom As INDArray = Nd4j.create(New Double() {1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00})

			Dim expected As INDArray = Nd4j.create(New Double() {0, 0, 0, 0, 0, 0, 0, 0, -2, -4, -6, -8, -10, -12, -14, -16}, New Long() {2, 8})

			Dim actual As INDArray = Nd4j.Executioner.exec(New BroadcastSubOp(num, denom, num.dup(), -1))
			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastAdd(ByVal backend As Nd4jBackend)
			Dim num As INDArray = Nd4j.create(New Double() {1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00, -1.00, -2.00, -3.00, -4.00, -5.00, -6.00, -7.00, -8.00}).reshape(ChrW(2), 8)

			Dim denom As INDArray = Nd4j.create(New Double() {1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00})

			Dim expected As INDArray = Nd4j.create(New Double() {2, 4, 6, 8, 10, 12, 14, 16, 0, 0, 0, 0, 0, 0, 0, 0}, New Long() {2, 8})
			Dim dup As INDArray = num.dup()
			Dim actual As INDArray = Nd4j.Executioner.exec(New BroadcastAddOp(num, denom, dup, -1))
			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimension(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data(), New Long() {2, 2})
			'row
			Dim slice0 As INDArray = test.slice(0, 1)
			Dim slice02 As INDArray = test.slice(1, 1)

			Dim assertSlice0 As INDArray = Nd4j.create(New Double() {1, 3})
			Dim assertSlice02 As INDArray = Nd4j.create(New Double() {2, 4})
			assertEquals(assertSlice0, slice0)
			assertEquals(assertSlice02, slice02)

			'column
			Dim assertSlice1 As INDArray = Nd4j.create(New Double() {1, 2})
			Dim assertSlice12 As INDArray = Nd4j.create(New Double() {3, 4})


			Dim slice1 As INDArray = test.slice(0, 0)
			Dim slice12 As INDArray = test.slice(1, 0)


			assertEquals(assertSlice1, slice1)
			assertEquals(assertSlice12, slice12)


			Dim arr As INDArray = Nd4j.create(Nd4j.linspace(1, 24, 24, DataType.DOUBLE).data(), New Long() {4, 3, 2})
			Dim secondSliceFirstDimension As INDArray = arr.slice(1, 1)
			assertEquals(secondSliceFirstDimension, secondSliceFirstDimension)


		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshape(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(Nd4j.linspace(1, 24, 24, DataType.DOUBLE).data(), New Long() {4, 3, 2})
			Dim reshaped As INDArray = arr.reshape(ChrW(2), 3, 4)
			assertEquals(arr.length(), reshaped.length())
			assertEquals(True, New Long() {4, 3, 2}.SequenceEqual(arr.shape()))
			assertEquals(True, New Long() {2, 3, 4}.SequenceEqual(reshaped.shape()))

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDot() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDot()
			Dim vec1 As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			Dim vec2 As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})

			assertEquals(10.0f, vec1.sumNumber().floatValue(), 1e-5)
			assertEquals(10.0f, vec2.sumNumber().floatValue(), 1e-5)

			assertEquals(30, Nd4j.BlasWrapper.dot(vec1, vec2), 1e-1)

			Dim matrix As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim row As INDArray = matrix.getRow(1)

			assertEquals(7.0f, row.sumNumber().floatValue(), 1e-5f)

			assertEquals(25, Nd4j.BlasWrapper.dot(row, row), 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIdentity(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIdentity(ByVal backend As Nd4jBackend)
			Dim eye As INDArray = Nd4j.eye(5)
			assertTrue(New Long() {5, 5}.SequenceEqual(eye.shape()))
			eye = Nd4j.eye(5)
			assertTrue(New Long() {5, 5}.SequenceEqual(eye.shape()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTemp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTemp(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(New Long() {2, 2, 2}).castTo(DataType.DOUBLE)
	'        System.out.println("In:\n" + in);
			Dim permuted As INDArray = [in].permute(0, 2, 1) 'Permute, so we get correct order after reshaping
			Dim [out] As INDArray = permuted.reshape(ChrW(4), 2)
	'        System.out.println("Out:\n" + out);

			Dim countZero As Integer = 0
			For i As Integer = 0 To 7
				If [out].getDouble(i) = 0.0 Then
					countZero += 1
				End If
			Next i
			assertEquals(countZero, 0)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeans(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeans(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim mean1 As INDArray = a.mean(1)
			assertEquals(Nd4j.create(New Double() {1.5, 3.5}), mean1,getFailureMessage(backend))
			assertEquals(Nd4j.create(New Double() {2, 3}), a.mean(0),getFailureMessage(backend))
			assertEquals(2.5, Nd4j.linspace(1, 4, 4, DataType.DOUBLE).meanNumber().doubleValue(), 1e-1,getFailureMessage(backend))
			assertEquals(2.5, a.meanNumber().doubleValue(), 1e-1,getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSums(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSums(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			assertEquals(Nd4j.create(New Double() {3, 7}), a.sum(1),getFailureMessage(backend))
			assertEquals(Nd4j.create(New Double() {4, 6}), a.sum(0),getFailureMessage(backend))
			assertEquals(10, a.sumNumber().doubleValue(), 1e-1,getFailureMessage(backend))


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRSubi(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRSubi(ByVal backend As Nd4jBackend)
			Dim n2 As INDArray = Nd4j.ones(2)
			Dim n2Assertion As INDArray = Nd4j.zeros(2)
			Dim nRsubi As INDArray = n2.rsubi(1)
			assertEquals(n2Assertion, nRsubi)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(2), 2, 2)
			Dim B As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim concat As INDArray = Nd4j.concat(0, A, B)
			assertTrue(New Long() {5, 2, 2}.SequenceEqual(concat.shape()))

			Dim columnConcat As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim concatWith As INDArray = Nd4j.zeros(2, 3)
			Dim columnWiseConcat As INDArray = Nd4j.concat(0, columnConcat, concatWith)
	'        System.out.println(columnConcat);

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatHorizontally(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatHorizontally(ByVal backend As Nd4jBackend)
			Dim rowVector As INDArray = Nd4j.ones(1, 5)
			Dim other As INDArray = Nd4j.ones(1, 5)
			Dim concat As INDArray = Nd4j.hstack(other, rowVector)
			assertEquals(rowVector.rows(), concat.rows())
			assertEquals(rowVector.columns() * 2, concat.columns())
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMaxSameValues(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMaxSameValues(ByVal backend As Nd4jBackend)
			'Here: assume that by convention, argmax returns the index of the FIRST maximum value
			'Thus, argmax(ones(...)) = 0 by convention
			Dim arr As INDArray = Nd4j.ones(DataType.DOUBLE,1,10)

			For i As Integer = 0 To 9
				Dim argmax As Double = Nd4j.argMax(arr, 1).getDouble(0)
				'System.out.println(argmax);
				assertEquals(0.0, argmax, 0.0)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxStability(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxStability(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.create(New Double() {-0.75, 0.58, 0.42, 1.03, -0.61, 0.19, -0.37, -0.40, -1.42, -0.04}).reshape(ChrW(1), -1).transpose()
	'        System.out.println("Input transpose " + Shape.shapeToString(input.shapeInfo()));
			Dim output As INDArray = Nd4j.create(10, 1)
	'        System.out.println("Element wise stride of output " + output.elementWiseStride());
			Nd4j.Executioner.exec(New SoftMax(input, output))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssignOffset(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssignOffset(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(5, 5).castTo(DataType.DOUBLE)
			Dim row As INDArray = arr.slice(1)
			row.assign(1)
			assertEquals(Nd4j.ones(5).castTo(DataType.DOUBLE), row)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddScalar(ByVal backend As Nd4jBackend)
			Dim div As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 4)
			Dim rdiv As INDArray = div.add(1)
			Dim answer As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 5)
			assertEquals(answer, rdiv)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRdivScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRdivScalar(ByVal backend As Nd4jBackend)
			Dim div As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 4).castTo(DataType.DOUBLE)
			Dim rdiv As INDArray = div.rdiv(1)
			Dim answer As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 0.25).castTo(DataType.DOUBLE)
			assertEquals(rdiv, answer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRDivi(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRDivi(ByVal backend As Nd4jBackend)
			Dim n2 As INDArray = Nd4j.valueArrayOf(New Long() {1, 2}, 4).castTo(DataType.DOUBLE)
			Dim n2Assertion As INDArray = Nd4j.valueArrayOf(New Long() {1, 2}, 0.5).castTo(DataType.DOUBLE)
			Dim nRsubi As INDArray = n2.rdivi(2)
			assertEquals(n2Assertion, nRsubi)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testElementWiseAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testElementWiseAdd(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim linspace2 As INDArray = linspace.dup()
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {2, 4},
				New Double() {6, 8}
			})
			linspace.addi(linspace2)
			assertEquals(assertion, linspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSquareMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSquareMatrix(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 8, 8, DataType.DOUBLE).data(), New Long() {2, 2, 2})
			Dim eightFirstTest As INDArray = n.vectorAlongDimension(0, 2)
			Dim eightFirstAssertion As INDArray = Nd4j.create(New Double() {1, 2})
			assertEquals(eightFirstAssertion, eightFirstTest)

			Dim eightFirstTestSecond As INDArray = n.vectorAlongDimension(1, 2)
			Dim eightFirstTestSecondAssertion As INDArray = Nd4j.create(New Double() {3, 4})
			assertEquals(eightFirstTestSecondAssertion, eightFirstTestSecond)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNumVectorsAlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNumVectorsAlongDimension(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)
			assertEquals(12, arr.vectorsAlongDimension(2))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadCast(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadCast(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Dim broadCasted As INDArray = n.broadcast(5, 4)
			Dim i As Integer = 0
			Do While i < broadCasted.rows()
				Dim row As INDArray = broadCasted.getRow(i)
				assertEquals(n, broadCasted.getRow(i))
				i += 1
			Loop

			Dim broadCast2 As INDArray = broadCasted.getRow(0).broadcast(5, 4)
			assertEquals(broadCasted, broadCast2)


			Dim columnBroadcast As INDArray = n.reshape(ChrW(4), 1).broadcast(4, 5)
			i = 0
			Do While i < columnBroadcast.columns()
				Dim column As INDArray = columnBroadcast.getColumn(i)
				assertEquals(column, n)
				i += 1
			Loop

			Dim fourD As INDArray = Nd4j.create(1, 2, 1, 1)
			Dim broadCasted3 As INDArray = fourD.broadcast(1, 2, 36, 36)
			assertTrue(New Long() {1, 2, 36, 36}.SequenceEqual(broadCasted3.shape()))



			Dim ones As INDArray = Nd4j.ones(1, 1, 1).broadcast(2, 1, 1)
			assertArrayEquals(New Long() {2, 1, 1}, ones.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarBroadcast(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarBroadcast(ByVal backend As Nd4jBackend)
			Dim fiveThree As INDArray = Nd4j.ones(5, 3)
			Dim fiveThreeTest As INDArray = Nd4j.scalar(1.0).broadcast(5, 3)
			assertEquals(fiveThree, fiveThreeTest)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRowGetRowOrdering(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRowGetRowOrdering(ByVal backend As Nd4jBackend)
			Dim row1 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim put As INDArray = Nd4j.create(New Double() {5, 6})
			row1.putRow(1, put)


			Dim row1Fortran As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim putFortran As INDArray = Nd4j.create(New Double() {5, 6})
			row1Fortran.putRow(1, putFortran)
			assertEquals(row1, row1Fortran)
			Dim row1CTest As INDArray = row1.getRow(1)
			Dim row1FortranTest As INDArray = row1Fortran.getRow(1)
			assertEquals(row1CTest, row1FortranTest)



		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testElementWiseOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testElementWiseOps(ByVal backend As Nd4jBackend)
			Dim n1 As INDArray = Nd4j.scalar(1.0)
			Dim n2 As INDArray = Nd4j.scalar(2.0)
			Dim nClone As INDArray = n1.add(n2)
			assertEquals(Nd4j.scalar(3.0), nClone)
			assertFalse(n1.add(n2).Equals(n1))

			Dim n3 As INDArray = Nd4j.scalar(3.0)
			Dim n4 As INDArray = Nd4j.scalar(4.0)
			Dim subbed As INDArray = n4.sub(n3)
			Dim mulled As INDArray = n4.mul(n3)
			Dim div As INDArray = n4.div(n3)

			assertFalse(subbed.Equals(n4))
			assertFalse(mulled.Equals(n4))
			assertEquals(Nd4j.scalar(1.0), subbed)
			assertEquals(Nd4j.scalar(12.0), mulled)
			assertEquals(Nd4j.scalar(1.333333333333333333333), div)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNdArrayCreation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNdArrayCreation(ByVal backend As Nd4jBackend)
			Dim delta As Double = 1e-1
			Dim n1 As INDArray = Nd4j.create(New Double() {0R, 1R, 2R, 3R}, New Long() {2, 2}, "c"c)
			Dim lv As INDArray = n1.reshape(ChrW(-1))
			assertEquals(0R, lv.getDouble(0), delta)
			assertEquals(1R, lv.getDouble(1), delta)
			assertEquals(2R, lv.getDouble(2), delta)
			assertEquals(3R, lv.getDouble(3), delta)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFlattenedWithOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToFlattenedWithOrder(ByVal backend As Nd4jBackend)
			Dim firstShape() As Integer = {10, 3}
			Dim firstLen As Integer = ArrayUtil.prod(firstShape)
			Dim secondShape() As Integer = {2, 7}
			Dim secondLen As Integer = ArrayUtil.prod(secondShape)
			Dim thirdShape() As Integer = {3, 3}
			Dim thirdLen As Integer = ArrayUtil.prod(thirdShape)
			Dim firstC As INDArray = Nd4j.linspace(1, firstLen, firstLen, DataType.DOUBLE).reshape("c"c, firstShape)
			Dim firstF As INDArray = Nd4j.create(firstShape, "f"c).assign(firstC)
			Dim secondC As INDArray = Nd4j.linspace(1, secondLen, secondLen, DataType.DOUBLE).reshape("c"c, secondShape)
			Dim secondF As INDArray = Nd4j.create(secondShape, "f"c).assign(secondC)
			Dim thirdC As INDArray = Nd4j.linspace(1, thirdLen, thirdLen, DataType.DOUBLE).reshape("c"c, thirdShape)
			Dim thirdF As INDArray = Nd4j.create(thirdShape, "f"c).assign(thirdC)


			assertEquals(firstC, firstF)
			assertEquals(secondC, secondF)
			assertEquals(thirdC, thirdF)

			Dim cc As INDArray = Nd4j.toFlattened("c"c, firstC, secondC, thirdC)
			Dim cf As INDArray = Nd4j.toFlattened("c"c, firstF, secondF, thirdF)
			assertEquals(cc, cf)

			Dim cmixed As INDArray = Nd4j.toFlattened("c"c, firstC, secondF, thirdF)
			assertEquals(cc, cmixed)

			Dim fc As INDArray = Nd4j.toFlattened("f"c, firstC, secondC, thirdC)
			assertNotEquals(cc, fc)

			Dim ff As INDArray = Nd4j.toFlattened("f"c, firstF, secondF, thirdF)
			assertEquals(fc, ff)

			Dim fmixed As INDArray = Nd4j.toFlattened("f"c, firstC, secondF, thirdF)
			assertEquals(fc, fmixed)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeakyRelu(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeakyRelu(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(-1, 1, 10, DataType.DOUBLE)
			Dim expected(9) As Double
			For i As Integer = 0 To 9
				Dim [in] As Double = arr.getDouble(i)
				expected(i) = (If([in] <= 0.0, 0.01 * [in], [in]))
			Next i

			Dim [out] As INDArray = Nd4j.Executioner.exec(New LeakyReLU(arr, 0.01))

			Dim exp As INDArray = Nd4j.create(expected)
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxRow(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 19
				Dim arr1 As INDArray = Nd4j.zeros(1, 100)
				Nd4j.Executioner.execAndReturn(New SoftMax(arr1))
	'            System.out.println(Arrays.toString(arr1.data().asFloat()));
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeakyRelu2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeakyRelu2(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(-1, 1, 10, DataType.DOUBLE)
			Dim expected(9) As Double
			For i As Integer = 0 To 9
				Dim [in] As Double = arr.getDouble(i)
				expected(i) = (If([in] <= 0.0, 0.01 * [in], [in]))
			Next i

			Dim [out] As INDArray = Nd4j.Executioner.exec(New LeakyReLU(arr, 0.01))

	'        System.out.println("Expected: " + Arrays.toString(expected));
	'        System.out.println("Actual:   " + Arrays.toString(out.data().asDouble()));

			Dim exp As INDArray = Nd4j.create(expected)
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupAndDupWithOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupAndDupWithOrder(ByVal backend As Nd4jBackend)
			Dim testInputs As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(ordering(), 4, 5, 123, DataType.DOUBLE)
			For Each pair As Pair(Of INDArray, String) In testInputs

				Dim msg As String = pair.Second
				Dim [in] As INDArray = pair.First
				Dim dup As INDArray = [in].dup()
				Dim dupc As INDArray = [in].dup("c"c)
				Dim dupf As INDArray = [in].dup("f"c)

				assertEquals(dup.ordering(), ordering())
				assertEquals(dupc.ordering(), "c"c)
				assertEquals(dupf.ordering(), "f"c)
				assertEquals([in], dupc,msg)
				assertEquals([in], dupf,msg)
			Next pair
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToOffsetZeroCopy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToOffsetZeroCopy(ByVal backend As Nd4jBackend)
			Dim testInputs As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(ordering(), 4, 5, 123, DataType.DOUBLE)

			For i As Integer = 0 To testInputs.Count - 1
				Dim pair As Pair(Of INDArray, String) = testInputs(i)
				Dim msg As String = pair.Second
				msg &= "Failed on " & i
				Dim [in] As INDArray = pair.First
				Dim dup As INDArray = Shape.toOffsetZeroCopy([in], ordering())
				Dim dupc As INDArray = Shape.toOffsetZeroCopy([in], "c"c)
				Dim dupf As INDArray = Shape.toOffsetZeroCopy([in], "f"c)
				Dim dupany As INDArray = Shape.toOffsetZeroCopyAnyOrder([in])

				assertEquals([in], dup,msg)
				assertEquals([in], dupc,msg)
				assertEquals([in], dupf,msg)
				assertEquals(dupc.ordering(), "c"c,msg)
				assertEquals(dupf.ordering(), "f"c,msg)
				assertEquals([in], dupany,msg)

				assertEquals(dup.offset(), 0)
				assertEquals(dupc.offset(), 0)
				assertEquals(dupf.offset(), 0)
				assertEquals(dupany.offset(), 0)
				assertEquals(dup.length(), dup.data().length())
				assertEquals(dupc.length(), dupc.data().length())
				assertEquals(dupf.length(), dupf.data().length())
				assertEquals(dupany.length(), dupany.data().length())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void largeInstantiation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub largeInstantiation(ByVal backend As Nd4jBackend)
			Nd4j.ones((1024 * 1024 * 511) + (1024 * 1024 - 1)) ' Still works; this can even be called as often as I want, allowing me even to spill over on disk
			Nd4j.ones((1024 * 1024 * 511) + (1024 * 1024)) ' Crashes
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssignNumber(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssignNumber(ByVal backend As Nd4jBackend)
			Dim nRows As Integer = 10
			Dim nCols As Integer = 20
			Dim [in] As INDArray = Nd4j.linspace(1, nRows * nCols, nRows * nCols, DataType.DOUBLE).reshape("c"c, New Long() {nRows, nCols})

			Dim subset1 As INDArray = [in].get(NDArrayIndex.interval(0, 1), NDArrayIndex.interval(0, nCols \ 2))
			subset1.assign(1.0)

			Dim subset2 As INDArray = [in].get(NDArrayIndex.interval(5, 8), NDArrayIndex.interval(nCols \ 2, nCols))
			subset2.assign(2.0)
			Dim assertion As INDArray = Nd4j.create(New Double() {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0, 31.0, 32.0, 33.0, 34.0, 35.0, 36.0, 37.0, 38.0, 39.0, 40.0, 41.0, 42.0, 43.0, 44.0, 45.0, 46.0, 47.0, 48.0, 49.0, 50.0, 51.0, 52.0, 53.0, 54.0, 55.0, 56.0, 57.0, 58.0, 59.0, 60.0, 61.0, 62.0, 63.0, 64.0, 65.0, 66.0, 67.0, 68.0, 69.0, 70.0, 71.0, 72.0, 73.0, 74.0, 75.0, 76.0, 77.0, 78.0, 79.0, 80.0, 81.0, 82.0, 83.0, 84.0, 85.0, 86.0, 87.0, 88.0, 89.0, 90.0, 91.0, 92.0, 93.0, 94.0, 95.0, 96.0, 97.0, 98.0, 99.0, 100.0, 101.0, 102.0, 103.0, 104.0, 105.0, 106.0, 107.0, 108.0, 109.0, 110.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 121.0, 122.0, 123.0, 124.0, 125.0, 126.0, 127.0, 128.0, 129.0, 130.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 141.0, 142.0, 143.0, 144.0, 145.0, 146.0, 147.0, 148.0, 149.0, 150.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 161.0, 162.0, 163.0, 164.0, 165.0, 166.0, 167.0, 168.0, 169.0, 170.0, 171.0, 172.0, 173.0, 174.0, 175.0, 176.0, 177.0, 178.0, 179.0, 180.0, 181.0, 182.0, 183.0, 184.0, 185.0, 186.0, 187.0, 188.0, 189.0, 190.0, 191.0, 192.0, 193.0, 194.0, 195.0, 196.0, 197.0, 198.0, 199.0, 200.0}, [in].shape(), 0, "c"c)
			assertEquals(assertion, [in])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumDifferentOrdersSquareMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumDifferentOrdersSquareMatrix(ByVal backend As Nd4jBackend)
			Dim arrc As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim arrf As INDArray = Nd4j.create(New Long() {2, 2}, "f"c).assign(arrc)

			Dim cSum As INDArray = arrc.sum(0)
			Dim fSum As INDArray = arrf.sum(0)
			assertEquals(arrc, arrf)
			assertEquals(cSum, fSum) 'Expect: 4,6. Getting [4, 4] for f order
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testAssignMixedC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssignMixedC(ByVal backend As Nd4jBackend)
			Dim shape1() As Integer = {3, 2, 2, 2, 2, 2}
			Dim shape2() As Integer = {12, 8}
			Dim length As Integer = ArrayUtil.prod(shape1)

			assertEquals(ArrayUtil.prod(shape1), ArrayUtil.prod(shape2))

			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape1)
			Dim arr2c As INDArray = Nd4j.create(shape2, "c"c)
			Dim arr2f As INDArray = Nd4j.create(shape2, "f"c)

	'        log.info("2f data: {}", Arrays.toString(arr2f.data().asFloat()));

			arr2c.assign(arr)
	'        System.out.println("--------------");
			arr2f.assign(arr)

			Dim exp As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2)

	'        log.info("arr data: {}", Arrays.toString(arr.data().asFloat()));
	'        log.info("2c data: {}", Arrays.toString(arr2c.data().asFloat()));
	'        log.info("2f data: {}", Arrays.toString(arr2f.data().asFloat()));
	'        log.info("2c shape: {}", Arrays.toString(arr2c.shapeInfoDataBuffer().asInt()));
	'        log.info("2f shape: {}", Arrays.toString(arr2f.shapeInfoDataBuffer().asInt()));
			assertEquals(exp, arr2c)
			assertEquals(exp, arr2f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDummy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDummy(ByVal backend As Nd4jBackend)
			Dim arr2f As INDArray = Nd4j.create(New Double() {1.0, 13.0, 25.0, 37.0, 49.0, 61.0, 73.0, 85.0, 2.0, 14.0, 26.0, 38.0, 50.0, 62.0, 74.0, 86.0, 3.0, 15.0, 27.0, 39.0, 51.0, 63.0, 75.0, 87.0, 4.0, 16.0, 28.0, 40.0, 52.0, 64.0, 76.0, 88.0, 5.0, 17.0, 29.0, 41.0, 53.0, 65.0, 77.0, 89.0, 6.0, 18.0, 30.0, 42.0, 54.0, 66.0, 78.0, 90.0, 7.0, 19.0, 31.0, 43.0, 55.0, 67.0, 79.0, 91.0, 8.0, 20.0, 32.0, 44.0, 56.0, 68.0, 80.0, 92.0, 9.0, 21.0, 33.0, 45.0, 57.0, 69.0, 81.0, 93.0, 10.0, 22.0, 34.0, 46.0, 58.0, 70.0, 82.0, 94.0, 11.0, 23.0, 35.0, 47.0, 59.0, 71.0, 83.0, 95.0, 12.0, 24.0, 36.0, 48.0, 60.0, 72.0, 84.0, 96.0}, New Long() {12, 8}, "f"c)
	'        log.info("arr2f shape: {}", Arrays.toString(arr2f.shapeInfoDataBuffer().asInt()));
	'        log.info("arr2f data: {}", Arrays.toString(arr2f.data().asFloat()));
	'        log.info("render: {}", arr2f);

	'        log.info("----------------------");

			Dim array As INDArray = Nd4j.linspace(1, 96, 96, DataType.DOUBLE).reshape("c"c, 12, 8)
	'        log.info("array render: {}", array);

	'        log.info("----------------------");

			Dim arrayf As INDArray = array.dup("f"c)
	'        log.info("arrayf render: {}", arrayf);
	'        log.info("arrayf shape: {}", Arrays.toString(arrayf.shapeInfoDataBuffer().asInt()));
	'        log.info("arrayf data: {}", Arrays.toString(arrayf.data().asFloat()));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateDetached_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateDetached_1(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){10}
			Dim dataTypes As val = New DataType() {DataType.DOUBLE, DataType.BOOL, DataType.BYTE, DataType.UBYTE, DataType.SHORT, DataType.UINT16, DataType.INT, DataType.UINT32, DataType.LONG, DataType.UINT64, DataType.FLOAT, DataType.BFLOAT16, DataType.HALF}

			For Each dt As DataType In dataTypes
				Dim dataBuffer As val = Nd4j.createBufferDetached(shape, dt)
				assertEquals(dt, dataBuffer.dataType())
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateDetached_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateDetached_2(ByVal backend As Nd4jBackend)
			Dim shape As val = New Long(){10}
			Dim dataTypes As val = New DataType() {DataType.DOUBLE, DataType.BOOL, DataType.BYTE, DataType.UBYTE, DataType.SHORT, DataType.UINT16, DataType.INT, DataType.UINT32, DataType.LONG, DataType.UINT64, DataType.FLOAT, DataType.BFLOAT16, DataType.HALF}

			For Each dt As DataType In dataTypes
				Dim dataBuffer As val = Nd4j.createBufferDetached(shape, dt)
				assertEquals(dt, dataBuffer.dataType())
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPairwiseMixedC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPairwiseMixedC(ByVal backend As Nd4jBackend)
			Dim shape2() As Integer = {12, 8}
			Dim length As Integer = ArrayUtil.prod(shape2)


			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2)
			Dim arr2c As INDArray = arr.dup("c"c)
			Dim arr2f As INDArray = arr.dup("f"c)

			arr2c.addi(arr)
	'        System.out.println("--------------");
			arr2f.addi(arr)

			Dim exp As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2).mul(2.0)

			assertEquals(exp, arr2c)
			assertEquals(exp, arr2f)

	'        log.info("2c data: {}", Arrays.toString(arr2c.data().asFloat()));
	'        log.info("2f data: {}", Arrays.toString(arr2f.data().asFloat()));

			assertTrue(arrayNotEquals(arr2c.data().asFloat(), arr2f.data().asFloat(), 1e-5f))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPairwiseMixedF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPairwiseMixedF(ByVal backend As Nd4jBackend)
			Dim shape2() As Integer = {12, 8}
			Dim length As Integer = ArrayUtil.prod(shape2)


			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2).dup("f"c)
			Dim arr2c As INDArray = arr.dup("c"c)
			Dim arr2f As INDArray = arr.dup("f"c)

			arr2c.addi(arr)
	'        System.out.println("--------------");
			arr2f.addi(arr)

			Dim exp As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2).dup("f"c).mul(2.0)

			assertEquals(exp, arr2c)
			assertEquals(exp, arr2f)

	'        log.info("2c data: {}", Arrays.toString(arr2c.data().asFloat()));
	'        log.info("2f data: {}", Arrays.toString(arr2f.data().asFloat()));

			assertTrue(arrayNotEquals(arr2c.data().asFloat(), arr2f.data().asFloat(), 1e-5f))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign2D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign2D(ByVal backend As Nd4jBackend)
			Dim shape2() As Integer = {8, 4}

			Dim length As Integer = ArrayUtil.prod(shape2)

			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2)
			Dim arr2c As INDArray = Nd4j.create(shape2, "c"c)
			Dim arr2f As INDArray = Nd4j.create(shape2, "f"c)

			arr2c.assign(arr)
	'        System.out.println("--------------");
			arr2f.assign(arr)

			Dim exp As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2)

			assertEquals(exp, arr2c)
			assertEquals(exp, arr2f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign2D_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign2D_2(ByVal backend As Nd4jBackend)
			Dim shape2() As Integer = {8, 4}

			Dim length As Integer = ArrayUtil.prod(shape2)

			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2)
			Dim arr2c As INDArray = Nd4j.create(shape2, "c"c)
			Dim arr2f As INDArray = Nd4j.create(shape2, "f"c)
			Dim z_f As INDArray = Nd4j.create(shape2, "f"c)
			Dim z_c As INDArray = Nd4j.create(shape2, "c"c)

			Nd4j.Executioner.exec(New [Set](arr2f, arr, z_f))

			Nd4j.Executioner.commit()

			Nd4j.Executioner.exec(New [Set](arr2f, arr, z_c))

			Dim exp As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape2)


	'        System.out.println("Zf data: " + Arrays.toString(z_f.data().asFloat()));
	'        System.out.println("Zc data: " + Arrays.toString(z_c.data().asFloat()));

			assertEquals(exp, z_f)
			assertEquals(exp, z_c)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign3D_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign3D_2(ByVal backend As Nd4jBackend)
			Dim shape3() As Integer = {8, 4, 8}

			Dim length As Integer = ArrayUtil.prod(shape3)

			Dim arr As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape3).dup("f"c)
			Dim arr3c As INDArray = Nd4j.create(shape3, "c"c)
			Dim arr3f As INDArray = Nd4j.create(shape3, "f"c)

			Nd4j.Executioner.exec(New [Set](arr3c, arr, arr3f))

			Nd4j.Executioner.commit()

			Nd4j.Executioner.exec(New [Set](arr3f, arr, arr3c))

			Dim exp As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, shape3)

			assertEquals(exp, arr3c)
			assertEquals(exp, arr3f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumDifferentOrders(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumDifferentOrders(ByVal backend As Nd4jBackend)
			Dim arrc As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape("c"c, 3, 2)
			Dim arrf As INDArray = Nd4j.create(New Double(5){}, New Long() {3, 2}, "f"c).assign(arrc)

			assertEquals(arrc, arrf)
			Dim cSum As INDArray = arrc.sum(0)
			Dim fSum As INDArray = arrf.sum(0)
			assertEquals(cSum, fSum) 'Expect: 0.51, 1.79; getting [0.51,1.71] for f order
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateUnitialized(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateUnitialized(ByVal backend As Nd4jBackend)

			Dim arrC As INDArray = Nd4j.createUninitialized(New Long() {10, 10}, "c"c)
			Dim arrF As INDArray = Nd4j.createUninitialized(New Long() {10, 10}, "f"c)

			assertEquals("c"c, arrC.ordering())
			assertArrayEquals(New Long() {10, 10}, arrC.shape())
			assertEquals("f"c, arrF.ordering())
			assertArrayEquals(New Long() {10, 10}, arrF.shape())

			'Can't really test that it's *actually* uninitialized...
			arrC.assign(0)
			arrF.assign(0)

			assertEquals(Nd4j.create(New Long() {10, 10}), arrC)
			assertEquals(Nd4j.create(New Long() {10, 10}), arrF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVarConst(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVarConst(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1, 100, 100, DataType.DOUBLE).reshape(ChrW(10), 10)
	'        System.out.println(x);
			assertFalse(Double.IsNaN(x.var(0).sumNumber().doubleValue()))
	'        System.out.println(x.var(0));
			x.var(0)
			assertFalse(Double.IsNaN(x.var(1).sumNumber().doubleValue()))
	'        System.out.println(x.var(1));
			x.var(1)

	'        System.out.println("=================================");
			' 2d array - all elements are the same
			Dim a As INDArray = Nd4j.ones(10, 10).mul(10)
	'        System.out.println(a);
			assertFalse(Double.IsNaN(a.var(0).sumNumber().doubleValue()))
	'        System.out.println(a.var(0));
			a.var(0)
			assertFalse(Double.IsNaN(a.var(1).sumNumber().doubleValue()))
	'        System.out.println(a.var(1));
			a.var(1)

			' 2d array - constant in one dimension
	'        System.out.println("=================================");
			Dim nums As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Dim b As INDArray = Nd4j.ones(10, 10).mulRowVector(nums)
	'        System.out.println(b);
			assertFalse(Double.IsNaN(CType(b.var(0).sumNumber(), Double?)))
	'        System.out.println(b.var(0));
			b.var(0)
			assertFalse(Double.IsNaN(CType(b.var(1).sumNumber(), Double?)))
	'        System.out.println(b.var(1));
			b.var(1)

	'        System.out.println("=================================");
	'        System.out.println(b.transpose());
			assertFalse(Double.IsNaN(CType(b.transpose().var(0).sumNumber(), Double?)))
	'        System.out.println(b.transpose().var(0));
			b.transpose().var(0)
			assertFalse(Double.IsNaN(CType(b.transpose().var(1).sumNumber(), Double?)))
	'        System.out.println(b.transpose().var(1));
			b.transpose().var(1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVPull1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVPull1(ByVal backend As Nd4jBackend)
			Dim indexes() As Integer = {0, 2, 4}
			Dim array As INDArray = Nd4j.linspace(1, 25, 25, DataType.DOUBLE).reshape(ChrW(5), 5)
			Dim assertion As INDArray = Nd4j.createUninitialized(New Long() {3, 5}, "f"c)
			For i As Integer = 0 To 2
				assertion.putRow(i, array.getRow(indexes(i)))
			Next i

			Dim result As INDArray = Nd4j.pullRows(array, 1, indexes, "f"c)

			assertEquals(3, result.rows())
			assertEquals(5, result.columns())
			assertEquals(assertion, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPullRowsValidation1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPullRowsValidation1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Nd4j.pullRows(Nd4j.create(10, 10), 2, New Integer() {0, 1, 2})
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPullRowsValidation2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPullRowsValidation2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Nd4j.pullRows(Nd4j.create(10, 10), 1, New Integer() {0, -1, 2})
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPullRowsValidation3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPullRowsValidation3(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Nd4j.pullRows(Nd4j.create(10, 10), 1, New Integer() {0, 1, 10})
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPullRowsValidation4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPullRowsValidation4(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Nd4j.pullRows(Nd4j.create(3, 10), 1, New Integer() {0, 1, 2, 3})
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPullRowsValidation5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPullRowsValidation5(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Nd4j.pullRows(Nd4j.create(3, 10), 1, New Integer() {0, 1, 2}, "e"c)
			End Sub)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVPull2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVPull2(ByVal backend As Nd4jBackend)
			Dim indexes As val = New Integer() {0, 2, 4}
			Dim array As INDArray = Nd4j.linspace(1, 25, 25, DataType.DOUBLE).reshape(ChrW(5), 5)
			Dim assertion As INDArray = Nd4j.createUninitialized(New Long() {3, 5}, "c"c)
			For i As Integer = 0 To 2
				assertion.putRow(i, array.getRow(indexes(i)))
			Next i

			Dim result As INDArray = Nd4j.pullRows(array, 1, indexes, "c"c)

			assertEquals(3, result.rows())
			assertEquals(5, result.columns())
			assertEquals(assertion, result)

	'        System.out.println(assertion.toString());
	'        System.out.println(result.toString());
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCompareAndSet1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompareAndSet1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(25)

			Dim assertion As INDArray = Nd4j.zeros(25)

			array.putScalar(0, 0.1f)
			array.putScalar(10, 0.1f)
			array.putScalar(20, 0.1f)

			Nd4j.Executioner.exec(New CompareAndSet(array, 0.1, 0.0, 0.01))

			assertEquals(assertion, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReplaceNaNs(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReplaceNaNs(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(25)
			Dim assertion As INDArray = Nd4j.zeros(25)

			array.putScalar(0, Float.NaN)
			array.putScalar(10, Float.NaN)
			array.putScalar(20, Float.NaN)

			assertNotEquals(assertion, array)

			Nd4j.Executioner.exec(New ReplaceNans(array, 0.0))

	'        System.out.println("Array After: " + array);

			assertEquals(assertion, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaNEquality(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaNEquality(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(25)
			Dim assertion As INDArray = Nd4j.zeros(25)

			array.putScalar(0, Float.NaN)
			array.putScalar(10, Float.NaN)
			array.putScalar(20, Float.NaN)

			assertNotEquals(assertion, array)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Crashes") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testSingleDeviceAveraging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSingleDeviceAveraging(ByVal backend As Nd4jBackend)
			Dim LENGTH As Integer = 512 * 1024 * 2
			Dim array1 As INDArray = Nd4j.valueArrayOf(LENGTH, 1.0)
			Dim array2 As INDArray = Nd4j.valueArrayOf(LENGTH, 2.0)
			Dim array3 As INDArray = Nd4j.valueArrayOf(LENGTH, 3.0)
			Dim array4 As INDArray = Nd4j.valueArrayOf(LENGTH, 4.0)
			Dim array5 As INDArray = Nd4j.valueArrayOf(LENGTH, 5.0)
			Dim array6 As INDArray = Nd4j.valueArrayOf(LENGTH, 6.0)
			Dim array7 As INDArray = Nd4j.valueArrayOf(LENGTH, 7.0)
			Dim array8 As INDArray = Nd4j.valueArrayOf(LENGTH, 8.0)
			Dim array9 As INDArray = Nd4j.valueArrayOf(LENGTH, 9.0)
			Dim array10 As INDArray = Nd4j.valueArrayOf(LENGTH, 10.0)
			Dim array11 As INDArray = Nd4j.valueArrayOf(LENGTH, 11.0)
			Dim array12 As INDArray = Nd4j.valueArrayOf(LENGTH, 12.0)
			Dim array13 As INDArray = Nd4j.valueArrayOf(LENGTH, 13.0)
			Dim array14 As INDArray = Nd4j.valueArrayOf(LENGTH, 14.0)
			Dim array15 As INDArray = Nd4j.valueArrayOf(LENGTH, 15.0)
			Dim array16 As INDArray = Nd4j.valueArrayOf(LENGTH, 16.0)


			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim arrayMean As INDArray = Nd4j.averageAndPropagate(New INDArray() {array1, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12, array13, array14, array15, array16})
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Console.WriteLine("Execution time: " & (time2 - time1))

			assertNotEquals(Nothing, arrayMean)

			assertEquals(8.5f, arrayMean.getFloat(12), 0.1f)
			assertEquals(8.5f, arrayMean.getFloat(150), 0.1f)
			assertEquals(8.5f, arrayMean.getFloat(475), 0.1f)


			assertEquals(8.5f, array1.getFloat(475), 0.1f)
			assertEquals(8.5f, array2.getFloat(475), 0.1f)
			assertEquals(8.5f, array3.getFloat(475), 0.1f)
			assertEquals(8.5f, array5.getFloat(475), 0.1f)
			assertEquals(8.5f, array16.getFloat(475), 0.1f)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistance1and2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistance1and2(ByVal backend As Nd4jBackend)
			Dim d1() As Double = {-1, 3, 2}
			Dim d2() As Double = {0, 1.5, -3.5}
			Dim arr1 As INDArray = Nd4j.create(d1)
			Dim arr2 As INDArray = Nd4j.create(d2)

			Dim expD1 As Double = 0.0
			Dim expD2 As Double = 0.0
			For i As Integer = 0 To d1.Length - 1
				Dim diff As Double = d1(i) - d2(i)
				expD1 += Math.Abs(diff)
				expD2 += diff * diff
			Next i
			expD2 = Math.Sqrt(expD2)

			assertEquals(expD1, arr1.distance1(arr2), 1e-5)
			assertEquals(expD2, arr1.distance2(arr2), 1e-5)
			assertEquals(expD2 * expD2, arr1.squaredDistance(arr2), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEqualsWithEps1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEqualsWithEps1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.create(New Double() {0.5f, 1.5f, 2.5f, 3.5f, 4.5f})
			Dim array2 As INDArray = Nd4j.create(New Double() {0f, 1f, 2f, 3f, 4f})
			Dim array3 As INDArray = Nd4j.create(New Double() {0f, 1.000001f, 2f, 3f, 4f})


			assertFalse(array1.equalsWithEps(array2, Nd4j.EPS_THRESHOLD))
			assertTrue(array2.equalsWithEps(array3, Nd4j.EPS_THRESHOLD))
			assertTrue(array1.equalsWithEps(array2, 0.7f))
			assertEquals(array2, array3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMaxIAMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMaxIAMax(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL

			Dim arr As INDArray = Nd4j.create(New Double() {-0.24, -0.26, -0.07, -0.01})
			Dim iMax As val = New ArgMax(New INDArray(){arr})
			Dim iaMax As val = New ArgAmax(New INDArray(){arr.dup()})
'JAVA TO VB CONVERTER NOTE: The variable imax was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim imax_Conflict As val = Nd4j.Executioner.exec(iMax)(0).getInt(0)
'JAVA TO VB CONVERTER NOTE: The variable iamax was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim iamax_Conflict As val = Nd4j.Executioner.exec(iaMax)(0).getInt(0)
	'        System.out.println("IMAX: " + imax);
	'        System.out.println("IAMAX: " + iamax);
			assertEquals(1, iamax_Conflict)
			assertEquals(3, imax_Conflict)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMinIAMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMinIAMin(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {-0.24, -0.26, -0.07, -0.01})
			Dim abs As INDArray = Transforms.abs(arr)
			Dim iaMin As val = New ArgAmin(New INDArray(){abs})
			Dim iMin As val = New ArgMin(New INDArray(){arr.dup()})
'JAVA TO VB CONVERTER NOTE: The variable imin was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim imin_Conflict As Double = Nd4j.Executioner.exec(iMin)(0).getDouble(0)
'JAVA TO VB CONVERTER NOTE: The variable iamin was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim iamin_Conflict As Double = Nd4j.Executioner.exec(iaMin)(0).getDouble(0)
	'        System.out.println("IMin: " + imin);
	'        System.out.println("IAMin: " + iamin);
			assertEquals(3, iamin_Conflict, 1e-12)
			assertEquals(1, imin_Conflict, 1e-12)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcast3d2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcast3d2d(ByVal backend As Nd4jBackend)
			Dim orders() As Char = {"c"c, "f"c}

			For Each orderArr As Char In orders
				For Each orderbc As Char In orders
	'                System.out.println(orderArr + "\t" + orderbc);
					Dim arrOrig As INDArray = Nd4j.ones(3, 4, 5).dup(orderArr)

					'Broadcast on dimensions 0,1
					Dim bc01 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1},
						New Double() {1, 0, 1, 1},
						New Double() {1, 1, 0, 0}
					}).dup(orderbc)

					Dim result01 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(arrOrig, bc01, result01, 0, 1))

					For i As Integer = 0 To 4
						Dim subset As INDArray = result01.tensorAlongDimension(i, 0, 1) 'result01.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i));
						assertEquals(bc01, subset)
					Next i

					'Broadcast on dimensions 0,2
					Dim bc02 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1},
						New Double() {1, 1, 1, 0, 0}
					}).dup(orderbc)

					Dim result02 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(arrOrig, bc02, result02, 0, 2))

					For i As Integer = 0 To 3
						Dim subset As INDArray = result02.tensorAlongDimension(i, 0, 2) 'result02.get(NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.all());
						assertEquals(bc02, subset)
					Next i

					'Broadcast on dimensions 1,2
					Dim bc12 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1},
						New Double() {0, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1},
						New Double() {1, 1, 1, 0, 0}
					}).dup(orderbc)

					Dim result12 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(arrOrig, bc12, result12, 1, 2))

					For i As Integer = 0 To 2
						Dim subset As INDArray = result12.tensorAlongDimension(i, 1, 2) 'result12.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all());
						assertEquals(bc12, subset,"Failed for subset [" & i & "] orders [" & orderArr & "/" & orderbc & "]")
					Next i
				Next orderbc
			Next orderArr
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcast4d2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcast4d2d(ByVal backend As Nd4jBackend)
			Dim orders() As Char = {"c"c, "f"c}

			For Each orderArr As Char In orders
				For Each orderbc As Char In orders
	'                System.out.println(orderArr + "\t" + orderbc);
					Dim arrOrig As INDArray = Nd4j.ones(3, 4, 5, 6).dup(orderArr)

					'Broadcast on dimensions 0,1
					Dim bc01 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1},
						New Double() {1, 0, 1, 1},
						New Double() {1, 1, 0, 0}
					}).dup(orderbc)

					Dim result01 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(result01, bc01, result01, 0, 1))

					For d2 As Integer = 0 To 4
						For d3 As Integer = 0 To 5
							Dim subset As INDArray = result01.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(d2), NDArrayIndex.point(d3))
							assertEquals(bc01, subset)
						Next d3
					Next d2

					'Broadcast on dimensions 0,2
					Dim bc02 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1},
						New Double() {1, 1, 1, 0, 0}
					}).dup(orderbc)

					Dim result02 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(result02, bc02, result02, 0, 2))

					For d1 As Integer = 0 To 3
						For d3 As Integer = 0 To 5
							Dim subset As INDArray = result02.get(NDArrayIndex.all(), NDArrayIndex.point(d1), NDArrayIndex.all(), NDArrayIndex.point(d3))
							assertEquals(bc02, subset)
						Next d3
					Next d1

					'Broadcast on dimensions 0,3
					Dim bc03 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1, 1},
						New Double() {1, 1, 1, 0, 0, 0}
					}).dup(orderbc)

					Dim result03 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(result03, bc03, result03, 0, 3))

					For d1 As Integer = 0 To 3
						For d2 As Integer = 0 To 4
							Dim subset As INDArray = result03.get(NDArrayIndex.all(), NDArrayIndex.point(d1), NDArrayIndex.point(d2), NDArrayIndex.all())
							assertEquals(bc03, subset)
						Next d2
					Next d1

					'Broadcast on dimensions 1,2
					Dim bc12 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1},
						New Double() {0, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1},
						New Double() {1, 1, 1, 0, 0}
					}).dup(orderbc)

					Dim result12 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(result12, bc12, result12, 1, 2))

					For d0 As Integer = 0 To 2
						For d3 As Integer = 0 To 5
							Dim subset As INDArray = result12.get(NDArrayIndex.point(d0), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(d3))
							assertEquals(bc12, subset)
						Next d3
					Next d0

					'Broadcast on dimensions 1,3
					Dim bc13 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1, 1},
						New Double() {0, 1, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1, 1},
						New Double() {1, 1, 1, 0, 0, 1}
					}).dup(orderbc)

					Dim result13 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(result13, bc13, result13, 1, 3))

					For d0 As Integer = 0 To 2
						For d2 As Integer = 0 To 4
							Dim subset As INDArray = result13.get(NDArrayIndex.point(d0), NDArrayIndex.all(), NDArrayIndex.point(d2), NDArrayIndex.all())
							assertEquals(bc13, subset)
						Next d2
					Next d0

					'Broadcast on dimensions 2,3
					Dim bc23 As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1, 1},
						New Double() {1, 0, 0, 1, 1, 1},
						New Double() {1, 1, 1, 0, 0, 0},
						New Double() {1, 1, 1, 0, 0, 0},
						New Double() {1, 1, 1, 0, 0, 0}
					}).dup(orderbc)

					Dim result23 As INDArray = arrOrig.dup(orderArr)
					Nd4j.Executioner.exec(New BroadcastMulOp(result23, bc23, result23, 2, 3))

					For d0 As Integer = 0 To 2
						For d1 As Integer = 0 To 3
							Dim subset As INDArray = result23.get(NDArrayIndex.point(d0), NDArrayIndex.point(d1), NDArrayIndex.all(), NDArrayIndex.all())
							assertEquals(bc23, subset)
						Next d1
					Next d0

				Next orderbc
			Next orderArr
		End Sub

		Protected Friend Shared Function arrayNotEquals(ByVal arrayX() As Single, ByVal arrayY() As Single, ByVal delta As Single) As Boolean
			If arrayX.Length <> arrayY.Length Then
				Return False
			End If

			' on 2d arrays first & last elements will match regardless of order
			For i As Integer = 1 To arrayX.Length - 2
				If Math.Abs(arrayX(i) - arrayY(i)) < delta Then
					log.info("ArrX[{}]: {}; ArrY[{}]: {}", i, arrayX(i), i, arrayY(i))
					Return False
				End If
			Next i

			Return True
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMax2Of3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMax2Of3d(ByVal backend As Nd4jBackend)
			Dim slices(2)()() As Double
			Dim isMax(2)()() As Boolean

			slices(0) = New Double()() {
				New Double() {1, 10, 2},
				New Double() {3, 4, 5}
			}
			slices(1) = New Double()() {
				New Double() {-10, -9, -8},
				New Double() {-7, -6, -5}
			}
			slices(2) = New Double()() {
				New Double() {4, 3, 2},
				New Double() {1, 0, -1}
			}

			isMax(0) = New Boolean()() {
				New Boolean() {False, True, False},
				New Boolean() {False, False, False}
			}
			isMax(1) = New Boolean()() {
				New Boolean() {False, False, False},
				New Boolean() {False, False, True}
			}
			isMax(2) = New Boolean()() {
				New Boolean() {True, False, False},
				New Boolean() {False, False, False}
			}

			Dim arr As INDArray = Nd4j.create(3, 2, 3)
			Dim expected As INDArray = Nd4j.create(DataType.BOOL, 3, 2, 3)
			For i As Integer = 0 To 2
				arr.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all()).assign(Nd4j.create(slices(i)))
				Dim t As val = Nd4j.create(ArrayUtil.flatten(isMax(i)), New Long(){2, 3}, DataType.BOOL)
				Dim v As val = expected.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all())
				v.assign(t)
			Next i

			Dim result As val = Nd4j.Executioner.exec(New IsMax(arr, Nd4j.createUninitialized(DataType.BOOL, arr.shape(), arr.ordering()), 1, 2))(0)

			assertEquals(expected, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMax2of4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMax2of4d(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim s As val = New Long() {2, 3, 4, 5}
			Dim arr As INDArray = Nd4j.rand(s).castTo(DataType.DOUBLE)

			'Test 0,1
			Dim exp As INDArray = Nd4j.create(DataType.BOOL, s)
			For i As Integer = 0 To 3
				For j As Integer = 0 To 4
					Dim subset As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.point(j))
					Dim subsetExp As INDArray = exp.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.point(j))
					assertArrayEquals(New Long() {2, 3}, subset.shape())

					Dim iter As New NdIndexIterator(2, 3)
					Dim maxIdx As val = New Long(){0, 0}
					Dim max As Double = -Double.MaxValue
					Do While iter.MoveNext()
						Dim [next] As val = iter.Current
						Dim d As Double = subset.getDouble([next])
						If d > max Then
							max = d
							maxIdx(0) = [next](0)
							maxIdx(1) = [next](1)
						End If
					Loop

					subsetExp.putScalar(maxIdx, 1)
				Next j
			Next i

			Dim actC As INDArray = Nd4j.Executioner.exec(New IsMax(arr.dup("c"c), Nd4j.createUninitialized(DataType.BOOL, arr.shape()),0, 1))(0)
			Dim actF As INDArray = Nd4j.Executioner.exec(New IsMax(arr.dup("f"c), Nd4j.createUninitialized(DataType.BOOL, arr.shape(), "f"c), 0, 1))(0)

			assertEquals(exp, actC)
			assertEquals(exp, actF)



			'Test 2,3
			exp = Nd4j.create(s)
			For i As Integer = 0 To 1
				For j As Integer = 0 To 2
					Dim subset As INDArray = arr.get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all())
					Dim subsetExp As INDArray = exp.get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all())
					assertArrayEquals(New Long() {4, 5}, subset.shape())

					Dim iter As New NdIndexIterator(4, 5)
					Dim maxIdx As val = New Long(){0, 0}
					Dim max As Double = -Double.MaxValue
					Do While iter.MoveNext()
						Dim [next] As val = iter.Current
						Dim d As Double = subset.getDouble([next])
						If d > max Then
							max = d
							maxIdx(0) = [next](0)
							maxIdx(1) = [next](1)
						End If
					Loop

					subsetExp.putScalar(maxIdx, 1.0)
				Next j
			Next i

			actC = Nd4j.Executioner.exec(New IsMax(arr.dup("c"c), arr.dup("c"c).ulike(), 2, 3))(0)
			actF = Nd4j.Executioner.exec(New IsMax(arr.dup("f"c), arr.dup("f"c).ulike(), 2, 3))(0)

			assertEquals(exp, actC)
			assertEquals(exp, actF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMax2Of3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMax2Of3d(ByVal backend As Nd4jBackend)
			Dim slices(2)()() As Double

			slices(0) = New Double()() {
				New Double() {1, 10, 2},
				New Double() {3, 4, 5}
			}
			slices(1) = New Double()() {
				New Double() {-10, -9, -8},
				New Double() {-7, -6, -5}
			}
			slices(2) = New Double()() {
				New Double() {4, 3, 2},
				New Double() {1, 0, -1}
			}

			'Based on a c-order traversal of each tensor
			Dim imax As val = New Long() {1, 5, 0}

			Dim arr As INDArray = Nd4j.create(3, 2, 3).castTo(DataType.DOUBLE)
			For i As Integer = 0 To 2
				arr.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all()).assign(Nd4j.create(slices(i)))
			Next i

			Dim [out] As INDArray = Nd4j.exec(New ArgMax(arr, False,New Integer(){1, 2}))(0)

			assertEquals(DataType.LONG, [out].dataType())

			Dim exp As INDArray = Nd4j.create(imax, New Long(){3}, DataType.LONG)

			assertEquals(exp, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMax2of4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMax2of4d(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim s As val = New Long() {2, 3, 4, 5}
			Dim arr As INDArray = Nd4j.rand(s).castTo(DataType.DOUBLE)

			'Test 0,1
			Dim exp As INDArray = Nd4j.create(DataType.LONG, 4, 5).castTo(DataType.DOUBLE)
			For i As Integer = 0 To 3
				For j As Integer = 0 To 4
					Dim subset As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i), NDArrayIndex.point(j))
					assertArrayEquals(New Long() {2, 3}, subset.shape())

					Dim iter As New NdIndexIterator("c"c, 2, 3)
					Dim max As Double = -Double.MaxValue
					Dim maxIdxPos As Integer = -1
					Dim count As Integer = 0
					Do While iter.MoveNext()
						Dim [next] As val = iter.Current
						Dim d As Double = subset.getDouble([next])
						If d > max Then
							max = d
							maxIdxPos = count
						End If
						count += 1
					Loop

					exp.putScalar(i, j, maxIdxPos)
				Next j
			Next i

			Dim actC As INDArray = Nd4j.Executioner.exec(New ArgMax(arr.dup("c"c), False,New Integer(){0, 1}))(0).castTo(DataType.DOUBLE)
			Dim actF As INDArray = Nd4j.Executioner.exec(New ArgMax(arr.dup("f"c), False,New Integer(){0, 1}))(0).castTo(DataType.DOUBLE)
			'
			assertEquals(exp, actC)
			assertEquals(exp, actF)



			'Test 2,3
			exp = Nd4j.create(DataType.LONG, 2, 3)
			For i As Integer = 0 To 1
				For j As Integer = 0 To 2
					Dim subset As INDArray = arr.get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all())
					assertArrayEquals(New Long() {4, 5}, subset.shape())

					Dim iter As New NdIndexIterator("c"c, 4, 5)
					Dim maxIdxPos As Integer = -1
					Dim max As Double = -Double.MaxValue
					Dim count As Integer = 0
					Do While iter.MoveNext()
						Dim [next] As val = iter.Current
						Dim d As Double = subset.getDouble([next])
						If d > max Then
							max = d
							maxIdxPos = count
						End If
						count += 1
					Loop

					exp.putScalar(i, j, maxIdxPos)
				Next j
			Next i

			actC = Nd4j.Executioner.exec(New ArgMax(arr.dup("c"c), False,New Integer(){2, 3}))(0)
			actF = Nd4j.Executioner.exec(New ArgMax(arr.dup("f"c), False,New Integer(){2, 3}))(0)

			assertEquals(exp, actC)
			assertEquals(exp, actF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadPermuteEquals(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadPermuteEquals(ByVal backend As Nd4jBackend)
			Dim d3c As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).reshape("c"c, 1, 5, 1)
			Dim d3f As INDArray = d3c.dup("f"c)

			Dim tadCi As INDArray = d3c.tensorAlongDimension(0, 1, 2).permutei(1, 0)
			Dim tadFi As INDArray = d3f.tensorAlongDimension(0, 1, 2).permutei(1, 0)

			Dim tadC As INDArray = d3c.tensorAlongDimension(0, 1, 2).permute(1, 0)
			Dim tadF As INDArray = d3f.tensorAlongDimension(0, 1, 2).permute(1, 0)

			assertArrayEquals(tadCi.shape(), tadC.shape())
			assertArrayEquals(tadCi.stride(), tadC.stride())
			assertArrayEquals(tadCi.data().asDouble(), tadC.data().asDouble(), 1e-8)
			assertEquals(tadC, tadCi.dup())
			assertEquals(tadC, tadCi)

			assertArrayEquals(tadFi.shape(), tadF.shape())
			assertArrayEquals(tadFi.stride(), tadF.stride())
			assertArrayEquals(tadFi.data().asDouble(), tadF.data().asDouble(), 1e-8)

			assertEquals(tadF, tadFi.dup())
			assertEquals(tadF, tadFi)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRemainder1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRemainder1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(10).assign(5.3).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(10).assign(2.0).castTo(DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(10).assign(-0.7).castTo(DataType.DOUBLE)

			Dim result As INDArray = x.remainder(2.0)
			assertEquals(exp, result)

			result = x.remainder(y)
			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFMod1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFMod1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(10).assign(5.3).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(10).assign(2.0).castTo(DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(10).assign(1.3).castTo(DataType.DOUBLE)

			Dim result As INDArray = x.fmod(2.0)
			assertEquals(exp, result)

			result = x.fmod(y)
			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStrangeDups1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStrangeDups1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(10).assign(0).castTo(DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(10).assign(1.0f).castTo(DataType.DOUBLE)
			Dim copy As INDArray = Nothing

			Dim x As Integer = 0
			Do While x < array.length()
				array.putScalar(x, 1f)
				copy = array.dup()
				x += 1
			Loop

			assertEquals(exp, array)
			assertEquals(exp, copy)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStrangeDups2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStrangeDups2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(10).assign(0).castTo(DataType.DOUBLE)
			Dim exp1 As INDArray = Nd4j.create(10).assign(1.0f).castTo(DataType.DOUBLE)
			Dim exp2 As INDArray = Nd4j.create(10).assign(1.0f).putScalar(9, 0f).castTo(DataType.DOUBLE)
			Dim copy As INDArray = Nothing

			Dim x As Integer = 0
			Do While x < array.length()
				copy = array.dup()
				array.putScalar(x, 1f)
				x += 1
			Loop

			assertEquals(exp1, array)
			assertEquals(exp2, copy)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionAgreement1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionAgreement1(ByVal backend As Nd4jBackend)
			Dim row As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape(ChrW(1), 3)
			Dim mean0 As INDArray = row.mean(0)
			assertFalse(mean0 Is row) 'True: same object (should be a copy)

			Dim col As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape(ChrW(1), -1).transpose()
			Dim mean1 As INDArray = col.mean(1)
			assertFalse(mean1 Is col)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecialConcat1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecialConcat1(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 9
				Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
				For x As Integer = 0 To 9
					arrays.Add(Nd4j.create(1, 100).assign(x).castTo(DataType.DOUBLE))
				Next x

				Dim matrix As INDArray = Nd4j.specialConcat(0, CType(arrays, List(Of INDArray)).ToArray())
				assertEquals(10, matrix.rows())
				assertEquals(100, matrix.columns())

				For x As Integer = 0 To 9
					assertEquals(x, matrix.getRow(x).meanNumber().doubleValue(), 0.1)
					assertEquals(arrays(x), matrix.getRow(x).reshape(ChrW(1), matrix.size(1)))
				Next x
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecialConcat2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecialConcat2(ByVal backend As Nd4jBackend)
			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For x As Integer = 0 To 9
				arrays.Add(Nd4j.create(New Double() {x, x, x, x, x, x}).reshape(ChrW(1), 6))
			Next x

			Dim matrix As INDArray = Nd4j.specialConcat(0, CType(arrays, List(Of INDArray)).ToArray())
			assertEquals(10, matrix.rows())
			assertEquals(6, matrix.columns())

	'        log.info("Result: {}", matrix);

			For x As Integer = 0 To 9
				assertEquals(x, matrix.getRow(x).meanNumber().doubleValue(), 0.1)
				assertEquals(arrays(x), matrix.getRow(x).reshape(ChrW(1), matrix.size(1)))
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutScalar1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutScalar1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(10, 3, 96, 96).castTo(DataType.DOUBLE)

			For i As Integer = 0 To 9
	'            log.info("Trying i: {}", i);
				array.tensorAlongDimension(i, 1, 2, 3).putScalar(1, 2, 3, 1)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAveraging1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAveraging1(ByVal backend As Nd4jBackend)
			Nd4j.AffinityManager.allowCrossDeviceAccess(False)

			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 9
				arrays.Add(Nd4j.create(100).assign(CDbl(i)).castTo(DataType.DOUBLE))
			Next i

			Dim result As INDArray = Nd4j.averageAndPropagate(arrays)

			assertEquals(4.5, result.meanNumber().doubleValue(), 0.01)

			For i As Integer = 0 To 9
				assertEquals(result, arrays(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAveraging2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAveraging2(ByVal backend As Nd4jBackend)

			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 9
				arrays.Add(Nd4j.create(100).assign(CDbl(i)))
			Next i

			Nd4j.averageAndPropagate(Nothing, arrays)

			Dim result As INDArray = arrays(0)

			assertEquals(4.5, result.meanNumber().doubleValue(), 0.01)

			For i As Integer = 0 To 9
				assertEquals(result, arrays(i),"Failed on iteration " & i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAveraging3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAveraging3(ByVal backend As Nd4jBackend)
			Nd4j.AffinityManager.allowCrossDeviceAccess(False)

			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 9
				arrays.Add(Nd4j.create(100).assign(CDbl(i)).castTo(DataType.DOUBLE))
			Next i

			Nd4j.averageAndPropagate(Nothing, arrays)

			Dim result As INDArray = arrays(0)

			assertEquals(4.5, result.meanNumber().doubleValue(), 0.01)

			For i As Integer = 0 To 9
				assertEquals(result, arrays(i),"Failed on iteration " & i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZ1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZ1(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(10, 10).assign(1.0).castTo(DataType.DOUBLE)

			Dim exp As INDArray = Nd4j.create(10).assign(10.0).castTo(DataType.DOUBLE)

			Dim res As INDArray = Nd4j.create(10).castTo(DataType.DOUBLE)
			Dim sums As INDArray = matrix.sum(res, 0)

			assertTrue(res Is sums)

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupDelayed(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupDelayed(ByVal backend As Nd4jBackend)
			If Not (TypeOf Nd4j.Executioner Is GridExecutioner) Then
				Return
			End If

	'        Nd4j.getExecutioner().commit();
			Dim executioner As val = DirectCast(Nd4j.Executioner, GridExecutioner)

	'        log.info("Starting: -------------------------------");

			'log.info("Point A: [{}]", executioner.getQueueLength());

			Dim [in] As INDArray = Nd4j.zeros(10).castTo(DataType.DOUBLE)

			Dim [out] As IList(Of INDArray) = New List(Of INDArray)()
			Dim comp As IList(Of INDArray) = New List(Of INDArray)()

			'log.info("Point B: [{}]", executioner.getQueueLength());
			'log.info("\n\n");

			Dim i As Integer = 0
			Do While i < [in].length()
	'            log.info("Point C: [{}]", executioner.getQueueLength());

				[in].putScalar(i, 1)

	'            log.info("Point D: [{}]", executioner.getQueueLength());

				[out].Add([in].dup())

	'            log.info("Point E: [{}]", executioner.getQueueLength());

				'Nd4j.getExecutioner().commit();
				[in].putScalar(i, 0)
				'Nd4j.getExecutioner().commit();

	'            log.info("Point F: [{}]\n\n", executioner.getQueueLength());
				i += 1
			Loop

			i = 0
			Do While i < [in].length()
				[in].putScalar(i, 1)
				comp.Add(Nd4j.create([in].data().dup()))
				'Nd4j.getExecutioner().commit();
				[in].putScalar(i, 0)
				i += 1
			Loop

			For i As Integer = 0 To [out].Count - 1
				assertEquals([out](i), comp(i),"Failed at iteration: [" & i & "]")
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarReduction1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarReduction1(ByVal backend As Nd4jBackend)
			Dim op As val = New Norm2(Nd4j.create(1).assign(1.0))
			Dim norm2 As Double = Nd4j.Executioner.execAndReturn(op).getFinalResult().doubleValue()
			Dim norm1 As Double = Nd4j.Executioner.execAndReturn(New Norm1(Nd4j.create(1).assign(1.0))).getFinalResult().doubleValue()
			Dim sum As Double = Nd4j.Executioner.execAndReturn(New Sum(Nd4j.create(1).assign(1.0))).getFinalResult().doubleValue()

			assertEquals(1.0, norm2, 0.001)
			assertEquals(1.0, norm1, 0.001)
			assertEquals(1.0, sum, 0.001)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void tesAbsReductions1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub tesAbsReductions1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {-1, -2, -3, -4}).castTo(DataType.DOUBLE)

			assertEquals(4, array.amaxNumber().intValue())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void tesAbsReductions2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub tesAbsReductions2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {-1, -2, -3, -4}).castTo(DataType.DOUBLE)

			assertEquals(1, array.aminNumber().intValue())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void tesAbsReductions3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub tesAbsReductions3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {-2, -2, 2, 2})

			assertEquals(2, array.ameanNumber().intValue())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void tesAbsReductions4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub tesAbsReductions4(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {-2, -2, 2, 3}).castTo(DataType.DOUBLE)
			assertEquals(1.0, array.sumNumber().doubleValue(), 1e-5)

			assertEquals(4, array.scan(Conditions.absGreaterThanOrEqual(0.0)).intValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void tesAbsReductions5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub tesAbsReductions5(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {-2, 0.0, 2, 2})

			assertEquals(3, array.scan(Conditions.absGreaterThan(0.0)).intValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewBroadcastComparison1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewBroadcastComparison1(ByVal backend As Nd4jBackend)
			Dim initial As val = Nd4j.create(3, 5).castTo(DataType.DOUBLE)
			Dim mask As val = Nd4j.create(New Double() {5, 4, 3, 2, 1}).castTo(DataType.DOUBLE)
			Dim result As val = Nd4j.createUninitialized(DataType.BOOL, initial.shape())
			Dim exp As val = Nd4j.create(New Boolean() {True, True, True, False, False})

			Dim i As Integer = 0
			Do While i < initial.columns()
				initial.getColumn(i).assign(i)
				i += 1
			Loop

			Nd4j.Executioner.commit()
	'        log.info("original: \n{}", initial);

			Nd4j.Executioner.exec(New BroadcastLessThan(initial, mask, result, 1))

			Nd4j.Executioner.commit()
	'        log.info("Comparison ----------------------------------------------");
			i = 0
			Do While i < initial.rows()
				Dim row As val = result.getRow(i)
				assertEquals(exp, row,"Failed at row " & i)
	'            log.info("-------------------");
				i += 1
			Loop
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewBroadcastComparison2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewBroadcastComparison2(ByVal backend As Nd4jBackend)
			Dim initial As val = Nd4j.create(3, 5).castTo(DataType.DOUBLE)
			Dim mask As val = Nd4j.create(New Double() {5, 4, 3, 2, 1}).castTo(DataType.DOUBLE)
			Dim result As val = Nd4j.createUninitialized(DataType.BOOL, initial.shape())
			Dim exp As val = Nd4j.create(New Boolean() {False, False, False, True, True})

			Dim i As Integer = 0
			Do While i < initial.columns()
				initial.getColumn(i).assign(i)
				i += 1
			Loop

			Nd4j.Executioner.commit()


			Nd4j.Executioner.exec(New BroadcastGreaterThan(initial, mask, result, 1))



			i = 0
			Do While i < initial.rows()
				assertEquals(exp, result.getRow(i),"Failed at row " & i)
				i += 1
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewBroadcastComparison3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewBroadcastComparison3(ByVal backend As Nd4jBackend)
			Dim initial As val = Nd4j.create(3, 5).castTo(DataType.DOUBLE)
			Dim mask As val = Nd4j.create(New Double() {5, 4, 3, 2, 1}).castTo(DataType.DOUBLE)
			Dim result As val = Nd4j.createUninitialized(DataType.BOOL, initial.shape())
			Dim exp As val = Nd4j.create(New Boolean() {False, False, True, True, True})

			Dim i As Integer = 0
			Do While i < initial.columns()
				initial.getColumn(i).assign(i + 1)
				i += 1
			Loop

			Nd4j.Executioner.commit()


			Nd4j.Executioner.exec(New BroadcastGreaterThanOrEqual(initial, mask, result, 1))


			i = 0
			Do While i < initial.rows()
				assertEquals(exp, result.getRow(i),"Failed at row " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewBroadcastComparison4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewBroadcastComparison4(ByVal backend As Nd4jBackend)
			Dim initial As val = Nd4j.create(3, 5).castTo(DataType.DOUBLE)
			Dim mask As val = Nd4j.create(New Double() {5, 4, 3, 2, 1}).castTo(DataType.DOUBLE)
			Dim result As val = Nd4j.createUninitialized(DataType.BOOL, initial.shape())
			Dim exp As val = Nd4j.create(New Boolean() {False, False, True, False, False})

			Dim i As Integer = 0
			Do While i < initial.columns()
				initial.getColumn(i).assign(i + 1)
				i += 1
			Loop

			Nd4j.Executioner.commit()


			Nd4j.Executioner.exec(New BroadcastEqualTo(initial, mask, result, 1))


			i = 0
			Do While i < initial.rows()
				assertEquals(exp, result.getRow(i),"Failed at row " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadDup_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadDup_1(ByVal backend As Nd4jBackend)
			Dim haystack As INDArray = Nd4j.create(New Double() {-0.84443557262, -0.06822254508, 0.74266910552, 0.61765557527, -0.77555125951, -0.99536740779, -0.0257304441183, -0.6512106060, -0.345789492130, -1.25485503673, 0.62955373525, -0.31357592344, 1.03362500667, -0.59279078245, 1.1914824247}).reshape(ChrW(3), 5).castTo(DataType.DOUBLE)
			Dim needle As INDArray = Nd4j.create(New Double() {-0.99536740779, -0.0257304441183, -0.6512106060, -0.345789492130, -1.25485503673}).castTo(DataType.DOUBLE)

			Dim row As val = haystack.getRow(1)
			Dim drow As val = row.dup()

	'        log.info("row shape: {}", row.shapeInfoDataBuffer());
			assertEquals(needle, drow)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_0(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_0(ByVal backend As Nd4jBackend)
			Dim haystack As INDArray = Nd4j.create(New Double() {-0.84443557262, -0.06822254508, 0.74266910552, 0.61765557527, -0.77555125951, -0.99536740779, -0.0257304441183, -0.6512106060, -0.345789492130, -1.25485503673, 0.62955373525, -0.31357592344, 1.03362500667, -0.59279078245, 1.1914824247}).reshape(ChrW(3), 5).castTo(DataType.DOUBLE)
			Dim needle As INDArray = Nd4j.create(New Double() {-0.99536740779, -0.0257304441183, -0.6512106060, -0.345789492130, -1.25485503673}).castTo(DataType.DOUBLE)

			Dim reduced As INDArray = Nd4j.Executioner.exec(New CosineDistance(haystack, needle, 1))

			Dim exp As INDArray = Nd4j.create(New Double() {0.577452, 0.0, 1.80182}).castTo(DataType.DOUBLE)
			assertEquals(exp, reduced)

			Dim i As Integer = 0
			Do While i < haystack.rows()
				Dim row As val = haystack.getRow(i).dup()
				Dim res As Double = Nd4j.Executioner.execAndReturn(New CosineDistance(row, needle)).z().getDouble(0)
				assertEquals(reduced.getDouble(i), res, 1e-5,"Failed at " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3SignaturesEquality_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3SignaturesEquality_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.rand(DataType.DOUBLE, 3, 4, 5)
			Dim y As val = Nd4j.rand(DataType.DOUBLE, 3, 4, 5)

			Dim reduceOp As val = New ManhattanDistance(x, y, 0)
			Dim op As val = DirectCast(reduceOp, Op)

			Dim z0 As val = Nd4j.Executioner.exec(reduceOp)
			Dim z1 As val = Nd4j.Executioner.exec(op)

			assertEquals(z0, z1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_1(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(5, 10).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < initial.rows()
				initial.getRow(i).assign(i + 1)
				i += 1
			Loop
			Dim needle As INDArray = Nd4j.create(New Double() {0.01, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9}).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.Executioner.exec(New CosineSimilarity(initial, needle, 1))

			log.warn("Reduced: {}", reduced)

			i = 0
			Do While i < initial.rows()
				Dim res As Double = Nd4j.Executioner.execAndReturn(New CosineSimilarity(initial.getRow(i).dup(), needle)).getFinalResult().doubleValue()
				assertEquals(reduced.getDouble(i), res, 0.001,"Failed at " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_2(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(5, 10).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < initial.rows()
				initial.getRow(i).assign(i + 1)
				i += 1
			Loop
			Dim needle As INDArray = Nd4j.create(10).assign(1.0).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.Executioner.exec(New ManhattanDistance(initial, needle, 1))

			log.warn("Reduced: {}", reduced)

			i = 0
			Do While i < initial.rows()
				Dim res As Double = Nd4j.Executioner.execAndReturn(New ManhattanDistance(initial.getRow(i).dup(), needle)).getFinalResult().doubleValue()
				assertEquals(reduced.getDouble(i), res, 0.001,"Failed at " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_3(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(5, 10).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < initial.rows()
				initial.getRow(i).assign(i + 1)
				i += 1
			Loop
			Dim needle As INDArray = Nd4j.create(10).assign(1.0).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.Executioner.exec(New EuclideanDistance(initial, needle, 1))

			log.warn("Reduced: {}", reduced)

			i = 0
			Do While i < initial.rows()
				Dim x As INDArray = initial.getRow(i).dup()
				Dim res As Double = Nd4j.Executioner.execAndReturn(New EuclideanDistance(x, needle)).getFinalResult().doubleValue()
				assertEquals(reduced.getDouble(i), res, 0.001,"Failed at " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_3_NEG(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_3_NEG(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(5, 10).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < initial.rows()
				initial.getRow(i).assign(i + 1)
				i += 1
			Loop
			Dim needle As INDArray = Nd4j.create(10).assign(1.0).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.Executioner.exec(New EuclideanDistance(initial, needle, -1))

			log.warn("Reduced: {}", reduced)

			i = 0
			Do While i < initial.rows()
				Dim x As INDArray = initial.getRow(i).dup()
				Dim res As Double = Nd4j.Executioner.execAndReturn(New EuclideanDistance(x, needle)).getFinalResult().doubleValue()
				assertEquals(reduced.getDouble(i), res, 0.001,"Failed at " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_3_NEG_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_3_NEG_2(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(5, 10).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < initial.rows()
				initial.getRow(i).assign(i + 1)
				i += 1
			Loop
			Dim needle As INDArray = Nd4j.create(10).assign(1.0).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.create(5).castTo(DataType.DOUBLE)
			Nd4j.Executioner.exec(New CosineSimilarity(initial, needle, reduced, -1))

			log.warn("Reduced: {}", reduced)

			i = 0
			Do While i < initial.rows()
				Dim x As INDArray = initial.getRow(i).dup()
				Dim res As Double = Nd4j.Executioner.execAndReturn(New CosineSimilarity(x, needle)).getFinalResult().doubleValue()
				assertEquals(reduced.getDouble(i), res, 0.001,"Failed at " & i)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_5(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim initial As INDArray = Nd4j.create(5, 10).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < initial.rows()
				initial.getRow(i).assign(i + 1)
				i += 1
			Loop
			Dim needle As INDArray = Nd4j.create(2, 10).assign(1.0).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.Executioner.exec(New EuclideanDistance(initial, needle, 1))
			End Sub)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadReduce3_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadReduce3_4(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(5, 6, 7).castTo(DataType.DOUBLE)
			For i As Integer = 0 To 4
				initial.tensorAlongDimension(i, 1, 2).assign(i + 1)
			Next i
			Dim needle As INDArray = Nd4j.create(6, 7).assign(1.0).castTo(DataType.DOUBLE)
			Dim reduced As INDArray = Nd4j.Executioner.exec(New ManhattanDistance(initial, needle, 1,2))

			log.warn("Reduced: {}", reduced)

			For i As Integer = 0 To 4
				Dim res As Double = Nd4j.Executioner.execAndReturn(New ManhattanDistance(initial.tensorAlongDimension(i, 1, 2).dup(), needle)).getFinalResult().doubleValue()
				assertEquals(reduced.getDouble(i), res, 0.001,"Failed at " & i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAtan2_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAtan2_1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(10).assign(-1.0).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(10).assign(0.0).castTo(DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(10).assign(Math.PI).castTo(DataType.DOUBLE)

			Dim z As INDArray = Transforms.atan2(x, y)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAtan2_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAtan2_2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(10).assign(1.0).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(10).assign(0.0).castTo(DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(10).assign(0.0).castTo(DataType.DOUBLE)

			Dim z As INDArray = Transforms.atan2(x, y)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJaccardDistance1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJaccardDistance1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 1, 0, 0, 1, 0}).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(New Double() {1, 1, 0, 1, 0, 0}).castTo(DataType.DOUBLE)

			Dim val As Double = Transforms.jaccardDistance(x, y)

			assertEquals(0.75, val, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJaccardDistance2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJaccardDistance2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 1, 0, 0, 1, 1}).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(New Double() {1, 1, 0, 1, 0, 0}).castTo(DataType.DOUBLE)

			Dim val As Double = Transforms.jaccardDistance(x, y)

			assertEquals(0.8, val, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHammingDistance1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHammingDistance1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 0, 0, 1, 0, 0}).castTo(DataType.DOUBLE)
			Dim y As INDArray = Nd4j.create(New Double() {0, 0, 0, 0, 1, 0}).castTo(DataType.DOUBLE)

			Dim val As Double = Transforms.hammingDistance(x, y)

			assertEquals(2.0 / 6, val, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHammingDistance2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHammingDistance2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 0, 0, 1, 0, 0})
			Dim y As INDArray = Nd4j.create(New Double() {0, 1, 0, 0, 1, 0})

			Dim val As Double = Transforms.hammingDistance(x, y)

			assertEquals(3.0 / 6, val, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHammingDistance3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHammingDistance3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(DataType.DOUBLE, 10, 6)
			Dim r As Integer = 0
			Do While r < x.rows()
				Dim p As val = r Mod x.columns()
				x.getRow(r).putScalar(p, 1)
				r += 1
			Loop

			Dim y As INDArray = Nd4j.create(New Double() {0, 0, 0, 0, 1, 0})

			Dim res As INDArray = Nd4j.Executioner.exec(New HammingDistance(x, y, 1))
			assertEquals(10, res.length())

			r = 0
			Do While r < x.rows()
				If r = 4 Then
					assertEquals(0.0, res.getDouble(r), 1e-5,"Failed at " & r)
				Else
					assertEquals(2.0 / 6, res.getDouble(r), 1e-5,"Failed at " & r)
				End If
				r += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances1(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(5, 10)
			Dim initialY As INDArray = Nd4j.create(7, 10)
			Dim i As Integer = 0
			Do While i < initialX.rows()
				initialX.getRow(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.rows()
				initialY.getRow(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allEuclideanDistances(initialX, initialY, 1)

			Nd4j.Executioner.commit()

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.rows()

				Dim rowX As INDArray = initialX.getRow(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.rows()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.euclideanDistance(rowX, initialY.getRow(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances2(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(5, 10)
			Dim initialY As INDArray = Nd4j.create(7, 10)
			Dim i As Integer = 0
			Do While i < initialX.rows()
				initialX.getRow(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.rows()
				initialY.getRow(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allManhattanDistances(initialX, initialY, 1)

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.rows()

				Dim rowX As INDArray = initialX.getRow(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.rows()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.manhattanDistance(rowX, initialY.getRow(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances2_Large(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances2_Large(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(5, 2000)
			Dim initialY As INDArray = Nd4j.create(7, 2000)
			Dim i As Integer = 0
			Do While i < initialX.rows()
				initialX.getRow(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.rows()
				initialY.getRow(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allManhattanDistances(initialX, initialY, 1)

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.rows()

				Dim rowX As INDArray = initialX.getRow(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.rows()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.manhattanDistance(rowX, initialY.getRow(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances3_Large(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances3_Large(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(5, 2000)
			Dim initialY As INDArray = Nd4j.create(7, 2000)
			Dim i As Integer = 0
			Do While i < initialX.rows()
				initialX.getRow(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.rows()
				initialY.getRow(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allEuclideanDistances(initialX, initialY, 1)

			Nd4j.Executioner.commit()

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.rows()

				Dim rowX As INDArray = initialX.getRow(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.rows()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.euclideanDistance(rowX, initialY.getRow(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances3_Large_Columns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances3_Large_Columns(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(2000, 5)
			Dim initialY As INDArray = Nd4j.create(2000, 7)
			Dim i As Integer = 0
			Do While i < initialX.columns()
				initialX.getColumn(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.columns()
				initialY.getColumn(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allEuclideanDistances(initialX, initialY, 0)

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.columns()

				Dim colX As INDArray = initialX.getColumn(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.columns()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.euclideanDistance(colX, initialY.getColumn(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances4_Large_Columns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances4_Large_Columns(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(2000, 5)
			Dim initialY As INDArray = Nd4j.create(2000, 7)
			Dim i As Integer = 0
			Do While i < initialX.columns()
				initialX.getColumn(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.columns()
				initialY.getColumn(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allManhattanDistances(initialX, initialY, 0)

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.columns()

				Dim colX As INDArray = initialX.getColumn(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.columns()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.manhattanDistance(colX, initialY.getColumn(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances5_Large_Columns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances5_Large_Columns(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(2000, 5)
			Dim initialY As INDArray = Nd4j.create(2000, 7)
			Dim i As Integer = 0
			Do While i < initialX.columns()
				initialX.getColumn(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.columns()
				initialY.getColumn(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allCosineDistances(initialX, initialY, 0)

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.columns()

				Dim colX As INDArray = initialX.getColumn(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.columns()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.cosineDistance(colX, initialY.getColumn(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances3_Small_Columns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances3_Small_Columns(ByVal backend As Nd4jBackend)
			Dim initialX As INDArray = Nd4j.create(200, 5)
			Dim initialY As INDArray = Nd4j.create(200, 7)
			Dim i As Integer = 0
			Do While i < initialX.columns()
				initialX.getColumn(i).assign(i + 1)
				i += 1
			Loop

			i = 0
			Do While i < initialY.columns()
				initialY.getColumn(i).assign(i + 101)
				i += 1
			Loop

			Dim result As INDArray = Transforms.allManhattanDistances(initialX, initialY, 0)

			assertEquals(5 * 7, result.length())

			Dim x As Integer = 0
			Do While x < initialX.columns()
				Dim colX As INDArray = initialX.getColumn(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.columns()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.manhattanDistance(colX, initialY.getColumn(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistances3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistances3(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(123)

			Dim initialX As INDArray = Nd4j.rand(5, 10).castTo(DataType.DOUBLE)
			Dim initialY As INDArray = initialX.mul(-1)

			Dim result As INDArray = Transforms.allCosineSimilarities(initialX, initialY, 1)

			assertEquals(5 * 5, result.length())

			Dim x As Integer = 0
			Do While x < initialX.rows()

				Dim rowX As INDArray = initialX.getRow(x).dup()

				Dim y As Integer = 0
				Do While y < initialY.rows()

					Dim res As Double = result.getDouble(x, y)
					Dim exp As Double = Transforms.cosineSim(rowX, initialY.getRow(y).dup())

					assertEquals(exp, res, 0.001,"Failed for [" & x & ", " & y & "]")
					y += 1
				Loop
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedTransforms1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedTransforms1(ByVal backend As Nd4jBackend)
			'output: Rank: 2,Offset: 0
			'Order: c Shape: [5,2],  stride: [2,1]
			'output: [0.5086864, 0.49131358, 0.50720876, 0.4927912, 0.46074104, 0.53925896, 0.49314, 0.50686, 0.5217741, 0.4782259]

			Dim d() As Double = {0.5086864, 0.49131358, 0.50720876, 0.4927912, 0.46074104, 0.53925896, 0.49314, 0.50686, 0.5217741, 0.4782259}

			Dim [in] As INDArray = Nd4j.create(d, New Long() {5, 2}, "c"c)

			Dim col0 As INDArray = [in].getColumn(0)
			Dim col1 As INDArray = [in].getColumn(1)

			Dim exp0((d.Length \ 2) - 1) As Single
			Dim exp1((d.Length \ 2) - 1) As Single
			For i As Integer = 0 To col0.length() - 1
				exp0(i) = CSng(Math.Log(col0.getDouble(i)))
				exp1(i) = CSng(Math.Log(col1.getDouble(i)))
			Next i

			Dim out0 As INDArray = Transforms.log(col0, True)
			Dim out1 As INDArray = Transforms.log(col1, True)

			assertArrayEquals(exp0, out0.data().asFloat(), 1e-4f)
			assertArrayEquals(exp1, out1.data().asFloat(), 1e-4f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEntropy1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEntropy1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(1, 100).castTo(DataType.DOUBLE)

			Dim exp As Double = MathUtils.entropy(x.data().asDouble())
			Dim res As Double = x.entropyNumber().doubleValue()

			assertEquals(exp, res, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEntropy2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEntropy2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(10, 100).castTo(DataType.DOUBLE)

			Dim res As INDArray = x.entropy(1)

			assertEquals(10, res.length())

			Dim t As Integer = 0
			Do While t < x.rows()
				Dim exp As Double = MathUtils.entropy(x.getRow(t).dup().data().asDouble())

				assertEquals(exp, res.getDouble(t), 1e-5)
				t += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEntropy3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEntropy3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(1, 100).castTo(DataType.DOUBLE)

			Dim exp As Double = getShannonEntropy(x.data().asDouble())
			Dim res As Double = x.shannonEntropyNumber().doubleValue()

			assertEquals(exp, res, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEntropy4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEntropy4(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(1, 100).castTo(DataType.DOUBLE)

			Dim exp As Double = getLogEntropy(x.data().asDouble())
			Dim res As Double = x.logEntropyNumber().doubleValue()

			assertEquals(exp, res, 1e-5)
		End Sub

		Protected Friend Overridable Function getShannonEntropy(ByVal array() As Double) As Double
			Dim ret As Double = 0
			For Each x As Double In array
				ret += FastMath.pow(x, 2) * FastMath.log(FastMath.pow(x, 2))
			Next x

			Return -ret
		End Function

		Protected Friend Overridable Function getLogEntropy(ByVal array() As Double) As Double
			Return Math.Log(MathUtils.entropy(array))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {9, 8, 7, 6, 5, 4, 3, 2, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9})

			Dim rev As INDArray = Nd4j.reverse(array)

			assertEquals(exp, rev)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10})

			Dim rev As INDArray = Nd4j.reverse(array)

			assertEquals(exp, rev)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {9, 8, 7, 6, 5, 4, 3, 2, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9})

			Dim rev As INDArray = Nd4j.Executioner.exec(New Reverse(array, array.ulike()))(0)

			assertEquals(exp, rev)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse4(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10})

			Dim rev As INDArray = Nd4j.Executioner.exec(New Reverse(array,array.ulike()))(0)

			assertEquals(exp, rev)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse5(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10})

			Dim rev As INDArray = Transforms.reverse(array, True)

			assertEquals(exp, rev)
			assertFalse(rev Is array)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse6(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10})

			Dim rev As INDArray = Transforms.reverse(array, False)

			assertEquals(exp, rev)
			assertTrue(rev Is array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNativeSortView1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSortView1(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(10, 10)
			Dim exp As INDArray = Nd4j.linspace(0, 9, 10, DataType.DOUBLE)
			Dim cnt As Integer = 0
			For i As Long = matrix.rows() - 1 To 0 Step -1
				matrix.getRow(CInt(i)).assign(cnt)
				cnt += 1
			Next i

			Nd4j.sort(matrix.getColumn(0), True)

			assertEquals(exp, matrix.getColumn(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNativeSort1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSort1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {9, 2, 1, 7, 6, 5, 4, 3, 8, 0})
			Dim exp1 As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9})
			Dim exp2 As INDArray = Nd4j.create(New Double() {9, 8, 7, 6, 5, 4, 3, 2, 1, 0})

			Dim res As INDArray = Nd4j.sort(array, True)

			assertEquals(exp1, res)

			res = Nd4j.sort(res, False)

			assertEquals(exp2, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNativeSort2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSort2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.rand(1, 10000).castTo(DataType.DOUBLE)

			Dim res As INDArray = Nd4j.sort(array, True)
			Dim exp As INDArray = res.dup()

			res = Nd4j.sort(res, False)
			res = Nd4j.sort(res, True)

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Crashes") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testNativeSort3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSort3(ByVal backend As Nd4jBackend)
			Dim length As Integer = If(IntegrationTests, 1048576, 16484)
			Dim array As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim exp As INDArray = array.dup()
			Nd4j.shuffle(array, 0)

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim res As INDArray = Nd4j.sort(array, True)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			log.info("Time spent: {} ms", time2 - time1)

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLongShapeDescriptor()
		Public Overridable Sub testLongShapeDescriptor()
			Nd4j.setDefaultDataTypes(DataType.DOUBLE, DataType.DOUBLE)
			Dim arr As INDArray = Nd4j.create(New Single(){1, 2, 3})

			Dim lsd As val = arr.shapeDescriptor()
			assertNotNull(lsd) 'Fails here on CUDA, OK on native/cpu
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseSmall_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseSmall_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 10, 10, DataType.INT)
			Dim exp As val = array.dup(array.ordering())

			Transforms.reverse(array, False)
			Transforms.reverse(array, False)

			Dim jexp As val = exp.data().asInt()
			Dim jarr As val = array.data().asInt()
			assertArrayEquals(jexp, jarr)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseSmall_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseSmall_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 10, 10, DataType.INT)
			Dim exp As val = array.dup(array.ordering())

			Dim reversed As val = Transforms.reverse(array, True)
			Dim rereversed As val = Transforms.reverse(reversed, True)

			Dim jexp As val = exp.data().asInt()
			Dim jarr As val = rereversed.data().asInt()
			assertArrayEquals(jexp, jarr)
			assertEquals(exp, rereversed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseSmall_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseSmall_3(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 11, 11, DataType.INT)
			Dim exp As val = array.dup(array.ordering())

			Transforms.reverse(array, False)

			Transforms.reverse(array, False)

			Dim jexp As val = exp.data().asInt()
			Dim jarr As val = array.data().asInt()
			assertArrayEquals(jexp, jarr)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseSmall_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseSmall_4(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 11, 11, DataType.INT)
			Dim exp As val = array.dup(array.ordering())

			Dim reversed As val = Transforms.reverse(array, True)
			Dim rereversed As val = Transforms.reverse(reversed, True)

			Dim jexp As val = exp.data().asInt()
			Dim jarr As val = rereversed.data().asInt()
			assertArrayEquals(jexp, jarr)
			assertEquals(exp, rereversed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 2017152, 2017152, DataType.INT)
			Dim exp As val = array.dup(array.ordering())

			Transforms.reverse(array, False)
			Transforms.reverse(array, False)

			Dim jexp As val = exp.data().asInt()
			Dim jarr As val = array.data().asInt()
			assertArrayEquals(jexp, jarr)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 2017152, 2017152, DataType.INT)
			Dim exp As val = array.dup(array.ordering())

			Dim reversed As val = Transforms.reverse(array, True)
			Dim rereversed As val = Transforms.reverse(reversed, True)

			Dim jexp As val = exp.data().asInt()
			Dim jarr As val = rereversed.data().asInt()
			assertArrayEquals(jexp, jarr)
			assertEquals(exp, rereversed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNativeSort3_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSort3_1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 2017152, 2017152, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim exp As INDArray = array.dup()
			Transforms.reverse(array, False)

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim res As INDArray = Nd4j.sort(array, True)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			log.info("Time spent: {} ms", time2 - time1)

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Crashes") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testNativeSortAlongDimension1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSortAlongDimension1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(1000, 1000)
			Dim exp1 As INDArray = Nd4j.linspace(1, 1000, 1000, DataType.DOUBLE)
			Dim dps As INDArray = exp1.dup()
			Nd4j.shuffle(dps, 0)

			assertNotEquals(exp1, dps)

			Dim r As Integer = 0
			Do While r < array.rows()
				array.getRow(r).assign(dps)
				r += 1
			Loop

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim res As INDArray = Nd4j.sort(array, 1, True)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Time spent: {} ms", time2 - time1)

			Dim e As val = exp1.toDoubleVector()
			r = 0
			Do While r < array.rows()
				Dim d As val = res.getRow(r).dup()

				assertArrayEquals(e, d.toDoubleVector(), 1e-5)
				assertEquals(exp1, d,"Failed at " & r)
				r += 1
			Loop
		End Sub

		Protected Friend Overridable Function checkIfUnique(ByVal array As INDArray, ByVal iteration As Integer) As Boolean
			Dim jarray As var = array.data().asInt()
			Dim set As var = New HashSet(Of Integer)()

			For Each v As val In jarray
				If set.contains(Convert.ToInt32(v)) Then
					Throw New System.InvalidOperationException("Duplicate value found: [" & v & "] on iteration " & iteration)
				End If

				set.add(Convert.ToInt32(v))
			Next v

			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void shuffleTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub shuffleTest(ByVal backend As Nd4jBackend)
			For e As Integer = 0 To 4
				'log.info("---------------------");
				Dim array As val = Nd4j.linspace(1, 1011, 1011, DataType.INT)

				checkIfUnique(array, e)
				Nd4j.Executioner.commit()

				Nd4j.shuffle(array, 0)
				Nd4j.Executioner.commit()

				checkIfUnique(array, e)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Crashes") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testNativeSortAlongDimension3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSortAlongDimension3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(2000, 2000)
			Dim exp1 As INDArray = Nd4j.linspace(1, 2000, 2000, DataType.DOUBLE)
			Dim dps As INDArray = exp1.dup()

			Nd4j.Executioner.commit()
			Nd4j.shuffle(dps, 0)

			assertNotEquals(exp1, dps)


			Dim r As Integer = 0
			Do While r < array.rows()
				array.getRow(r).assign(dps)
				r += 1
			Loop

			Dim arow As val = array.getRow(0).toFloatVector()

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim res As INDArray = Nd4j.sort(array, 1, True)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Time spent: {} ms", time2 - time1)

			Dim jexp As val = exp1.toFloatVector()
			r = 0
			Do While r < array.rows()
				Dim jrow As val = res.getRow(r).toFloatVector()
				'log.info("jrow: {}", jrow);
				assertArrayEquals(jexp, jrow, 1e-5f,"Failed at " & r)
				assertEquals(exp1, res.getRow(r),"Failed at " & r)
				'assertArrayEquals("Failed at " + r, exp1.data().asDouble(), res.getRow(r).dup().data().asDouble(), 1e-5);
				r += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Crashes") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testNativeSortAlongDimension2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNativeSortAlongDimension2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(100, 10)
			Dim exp1 As INDArray = Nd4j.create(New Double() {9, 8, 7, 6, 5, 4, 3, 2, 1, 0})

			Dim r As Integer = 0
			Do While r < array.rows()
				array.getRow(r).assign(Nd4j.create(New Double() {3, 8, 2, 7, 5, 6, 4, 9, 1, 0}))
				r += 1
			Loop

			Dim res As INDArray = Nd4j.sort(array, 1, False)

			r = 0
			Do While r < array.rows()
				assertEquals(exp1, res.getRow(r).dup(),"Failed at " & r)
				r += 1
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPercentile1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPercentile1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Dim percentile As New Percentile(50)
			Dim exp As Double = percentile.evaluate(array.data().asDouble())

			assertEquals(exp, array.percentileNumber(50))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPercentile2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPercentile2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE)
			Dim percentile As New Percentile(50)
			Dim exp As Double = percentile.evaluate(array.data().asDouble())

			assertEquals(exp, array.percentileNumber(50))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPercentile3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPercentile3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE)
			Dim percentile As New Percentile(75)
			Dim exp As Double = percentile.evaluate(array.data().asDouble())

			assertEquals(exp, array.percentileNumber(75))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPercentile4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPercentile4(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Dim percentile As New Percentile(75)
			Dim exp As Double = percentile.evaluate(array.data().asDouble())

			assertEquals(exp, array.percentileNumber(75))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPercentile5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPercentile5(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.createFromArray(New Integer(){1, 1982})
			Dim perc As val = array.percentileNumber(75)
			assertEquals(1982.0f, perc.floatValue(), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadPercentile1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadPercentile1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Transforms.reverse(array, False)
			Dim percentile As New Percentile(75)
			Dim exp As Double = percentile.evaluate(array.data().asDouble())

			Dim matrix As INDArray = Nd4j.create(10, 10)
			Dim i As Integer = 0
			Do While i < matrix.rows()
				matrix.getRow(i).assign(array)
				i += 1
			Loop

			Dim res As INDArray = matrix.percentile(75, 1)

			i = 0
			Do While i < matrix.rows()
				assertEquals(exp, res.getDouble(i), 1e-5)
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutiRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutiRowVector(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.createUninitialized(10, 10)
			Dim exp As INDArray = Nd4j.create(10, 10).assign(1.0)
			Dim row As INDArray = Nd4j.create(10).assign(1.0)

			matrix.putiRowVector(row)

			assertEquals(exp, matrix)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutiColumnsVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutiColumnsVector(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.createUninitialized(5, 10)
			Dim exp As INDArray = Nd4j.create(5, 10).assign(1.0)
			Dim row As INDArray = Nd4j.create(5, 1).assign(1.0)

			matrix.putiColumnVector(row)

			Nd4j.Executioner.commit()

			assertEquals(exp, matrix)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRsub1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRsub1(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(5).assign(2.0)
			Dim exp_0 As INDArray = Nd4j.ones(5).assign(2.0)
			Dim exp_1 As INDArray = Nd4j.create(5).assign(-1)

			Nd4j.Executioner.commit()

			Dim res As INDArray = arr.rsub(1.0)

			assertEquals(exp_0, arr)
			assertEquals(exp_1, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastMin(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(5, 5)
			Dim r As Integer = 0
			Do While r < matrix.rows()
				matrix.getRow(r).assign(Nd4j.create(New Double(){2, 3, 3, 4, 5}))
				r += 1
			Loop

			Dim row As INDArray = Nd4j.create(New Double(){1, 2, 3, 4, 5})

			Nd4j.Executioner.exec(New BroadcastMin(matrix, row, matrix, 1))

			r = 0
			Do While r < matrix.rows()
				assertEquals(Nd4j.create(New Double() {1, 2, 3, 4, 5}), matrix.getRow(r))
				r += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastMax(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(5, 5)
			Dim r As Integer = 0
			Do While r < matrix.rows()
				matrix.getRow(r).assign(Nd4j.create(New Double(){1, 2, 3, 2, 1}))
				r += 1
			Loop

			Dim row As INDArray = Nd4j.create(New Double(){1, 2, 3, 4, 5})

			Nd4j.Executioner.exec(New BroadcastMax(matrix, row, matrix, 1))

			r = 0
			Do While r < matrix.rows()
				assertEquals(Nd4j.create(New Double() {1, 2, 3, 4, 5}), matrix.getRow(r))
				r += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastAMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastAMax(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(5, 5)
			Dim r As Integer = 0
			Do While r < matrix.rows()
				matrix.getRow(r).assign(Nd4j.create(New Double(){1, 2, 3, 2, 1}))
				r += 1
			Loop

			Dim row As INDArray = Nd4j.create(New Double(){1, 2, 3, -4, -5})

			Nd4j.Executioner.exec(New BroadcastAMax(matrix, row, matrix, 1))

			r = 0
			Do While r < matrix.rows()
				assertEquals(Nd4j.create(New Double() {1, 2, 3, -4, -5}), matrix.getRow(r))
				r += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastAMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastAMin(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(5, 5)
			Dim r As Integer = 0
			Do While r < matrix.rows()
				matrix.getRow(r).assign(Nd4j.create(New Double(){2, 3, 3, 4, 1}))
				r += 1
			Loop

			Dim row As INDArray = Nd4j.create(New Double(){1, 2, 3, 4, -5})

			Nd4j.Executioner.exec(New BroadcastAMin(matrix, row, matrix, 1))

			r = 0
			Do While r < matrix.rows()
				assertEquals(Nd4j.create(New Double() {1, 2, 3, 4, 1}), matrix.getRow(r))
				r += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testLogExpSum1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogExpSum1(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(3, 3)
			Dim r As Integer = 0
			Do While r < matrix.rows()
				matrix.getRow(r).assign(Nd4j.create(New Double(){1, 2, 3}))
				r += 1
			Loop

			Dim res As INDArray = Nd4j.Executioner.exec(New LogSumExp(matrix, False, 1))(0)

			For e As Integer = 0 To res.length() - 1
				assertEquals(3.407605, res.getDouble(e), 1e-5)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testLogExpSum2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogExpSum2(ByVal backend As Nd4jBackend)
			Dim row As INDArray = Nd4j.create(New Double(){1, 2, 3})

			Dim res As Double = Nd4j.Executioner.exec(New LogSumExp(row))(0).getDouble(0)

			assertEquals(3.407605, res, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPow1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPow1(ByVal backend As Nd4jBackend)
			Dim argX As val = Nd4j.create(3).assign(2.0)
			Dim argY As val = Nd4j.create(New Double(){1.0, 2.0, 3.0})
			Dim exp As val = Nd4j.create(New Double() {2.0, 4.0, 8.0})
			Dim res As val = Transforms.pow(argX, argY)

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRDiv1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRDiv1(ByVal backend As Nd4jBackend)
			Dim argX As val = Nd4j.create(3).assign(2.0)
			Dim argY As val = Nd4j.create(New Double(){1.0, 2.0, 3.0})
			Dim exp As val = Nd4j.create(New Double() {0.5, 1.0, 1.5})
			Dim res As val = argX.rdiv(argY)

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEqualOrder1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEqualOrder1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim arrayC As val = array.dup("c"c)
			Dim arrayF As val = array.dup("f"c)

			assertEquals(array, arrayC)
			assertEquals(array, arrayF)
			assertEquals(arrayC, arrayF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchTransform(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchTransform(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(New Double() {1, 1, 1, 0, 1, 1},"c"c)
			Dim result As val = Nd4j.createUninitialized(DataType.BOOL, array.shape())
			Dim exp As val = Nd4j.create(New Boolean() {False, False, False, True, False, False})
			Dim op As Op = New MatchConditionTransform(array, result, 1e-5, Conditions.epsEquals(0.0))

			Nd4j.Executioner.exec(op)

			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test4DSumView(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test4DSumView(ByVal backend As Nd4jBackend)
			Dim labels As INDArray = Nd4j.linspace(1, 160, 160, DataType.DOUBLE).reshape(ChrW(2), 5, 4, 4)
			'INDArray labels = Nd4j.linspace(1, 192, 192).reshape(new long[]{2, 6, 4, 4});

			Dim size1 As val = labels.size(1)
			Dim classLabels As INDArray = labels.get(NDArrayIndex.all(), NDArrayIndex.interval(4, size1), NDArrayIndex.all(), NDArrayIndex.all())

	'        
	'        Should be 0s and 1s only in the "classLabels" subset - specifically a 1-hot vector, or all 0s
	'        double minNumber = classLabels.minNumber().doubleValue();
	'        double maxNumber = classLabels.maxNumber().doubleValue();
	'        System.out.println("Min/max: " + minNumber + "\t" + maxNumber);
	'        System.out.println(sum1);
	'        


			assertEquals(classLabels, classLabels.dup())

			'Expect 0 or 1 for each entry (sum of all 0s, or 1-hot vector = 0 or 1)
			Dim sum1 As INDArray = classLabels.max(1)
			Dim sum1_dup As INDArray = classLabels.dup().max(1)

			assertEquals(sum1_dup, sum1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatMul1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatMul1(ByVal backend As Nd4jBackend)
			Dim x As val = 2
			Dim A1 As val = 3
			Dim A2 As val = 4
			Dim B1 As val = 4
			Dim B2 As val = 3

			Dim a As val = Nd4j.linspace(1, x * A1 * A2, x * A1 * A2, DataType.DOUBLE).reshape(x, A1, A2)
			Dim b As val = Nd4j.linspace(1, x * B1 * B2, x * B1 * B2, DataType.DOUBLE).reshape(x, B1, B2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduction_Z1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduction_Z1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10, 10)

			Dim res As val = arrayX.max(1, 2)

			Nd4j.Executioner.commit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduction_Z2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduction_Z2(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)

			Dim res As val = arrayX.max(0)

			Nd4j.Executioner.commit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduction_Z3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduction_Z3(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(200, 300)

			Dim res As val = arrayX.maxNumber().doubleValue()

			Nd4j.Executioner.commit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxZ1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxZ1(ByVal backend As Nd4jBackend)
			Dim original As val = Nd4j.linspace(1, 100, 100, DataType.DOUBLE).reshape(ChrW(10), 10)
			Dim reference As val = original.dup(original.ordering())
			Dim expected As val = original.dup(original.ordering())

			Nd4j.Executioner.execAndReturn(DirectCast(New SoftMax(expected, expected, -1), CustomOp))

			Dim result As val = Nd4j.Executioner.exec(DirectCast(New SoftMax(original, original.dup(original.ordering())), CustomOp))(0)

			assertEquals(reference, original)
			assertEquals(expected, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRDiv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRDiv(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(New Double(){2, 2, 2})
			Dim y As val = Nd4j.create(New Double(){4, 6, 8})
			Dim result As val = Nd4j.createUninitialized(DataType.DOUBLE, 3)

			assertEquals(DataType.DOUBLE, x.dataType())
			assertEquals(DataType.DOUBLE, y.dataType())
			assertEquals(DataType.DOUBLE, result.dataType())

			Dim op As val = DynamicCustomOp.builder("RDiv").addInputs(x,y).addOutputs(result).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			assertEquals(Nd4j.create(New Double(){2, 3, 4}), result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2Col(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2Col(ByVal backend As Nd4jBackend)
			Dim kY As Integer = 5
			Dim kX As Integer = 5
			Dim sY As Integer = 1
			Dim sX As Integer = 1
			Dim pY As Integer = 0
			Dim pX As Integer = 0
			Dim dY As Integer = 1
			Dim dX As Integer = 1
			Dim inY As Integer = 28
			Dim inX As Integer = 28

			Dim isSameMode As Boolean = True

			Dim input As val = Nd4j.linspace(1, 2 * inY * inX, 2 * inY * inX, DataType.DOUBLE).reshape(ChrW(2), 1, inY, inX)
			Dim output As val = Nd4j.create(2, 1, 5, 5, 28, 28)

			Dim im2colOp As val = Im2col.builder().inputArrays(New INDArray(){input}).outputs(New INDArray(){output}).conv2DConfig(Conv2DConfig.builder().kH(kY).kW(kX).kH(kY).kW(kX).sH(sY).sW(sX).pH(pY).pW(pX).dH(dY).dW(dX).isSameMode(isSameMode).build()).build()

			Nd4j.Executioner.exec(im2colOp)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemmStrides(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemmStrides(ByVal backend As Nd4jBackend)
			' 4x5 matrix from arange(20)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray X = org.nd4j.linalg.factory.Nd4j.arange(20).reshape(4,5);
			Dim X As INDArray = Nd4j.arange(20).reshape(ChrW(4), 5)
			For i As Integer = 0 To 4
				' Get i-th column vector
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray xi = X.get(org.nd4j.linalg.indexing.NDArrayIndex.all(), org.nd4j.linalg.indexing.NDArrayIndex.point(i));
				Dim xi As INDArray = X.get(NDArrayIndex.all(), NDArrayIndex.point(i))
				' Build outer product
				Dim trans As val = xi
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray outerProduct = xi.mmul(trans);
				Dim outerProduct As INDArray = xi.mmul(trans)
				' Build outer product from duplicated column vectors
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray outerProductDuped = xi.dup().mmul(xi.dup());
				Dim outerProductDuped As INDArray = xi.dup().mmul(xi.dup())
				' Matrices should equal
				'final boolean eq = outerProduct.equalsWithEps(outerProductDuped, 1e-5);
				'assertTrue(eq);
				assertEquals(outerProductDuped, outerProduct)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeFailure(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeFailure(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim a As val = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim b As val = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim score As val = a.mmul(b)
			Dim reshaped1 As val = score.reshape(2,100)
			Dim reshaped2 As val = score.reshape(2,1)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalar_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalar_1(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.create(New Single(){2.0f}, New Long(){})

			assertTrue(scalar.isScalar())
			assertEquals(1, scalar.length())
			assertFalse(scalar.isMatrix())
			assertFalse(scalar.isVector())
			assertFalse(scalar.isRowVector())
			assertFalse(scalar.isColumnVector())

			assertEquals(2.0f, scalar.getFloat(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalar_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalar_2(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(2.0f)
			Dim scalar2 As val = Nd4j.scalar(2.0f)
			Dim scalar3 As val = Nd4j.scalar(3.0f)

			assertTrue(scalar.isScalar())
			assertEquals(1, scalar.length())
			assertFalse(scalar.isMatrix())
			assertFalse(scalar.isVector())
			assertFalse(scalar.isRowVector())
			assertFalse(scalar.isColumnVector())

			assertEquals(2.0f, scalar.getFloat(0), 1e-5)

			assertEquals(scalar, scalar2)
			assertNotEquals(scalar, scalar3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVector_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVector_1(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.createFromArray(New Single() {1, 2, 3, 4, 5})
			Dim vector2 As val = Nd4j.createFromArray(New Single() {1, 2, 3, 4, 5})
			Dim vector3 As val = Nd4j.createFromArray(New Single() {1, 2, 3, 4, 6})

			assertFalse(vector.isScalar())
			assertEquals(5, vector.length())
			assertFalse(vector.isMatrix())
			assertTrue(vector.isVector())
			assertTrue(vector.isRowVector())
			assertFalse(vector.isColumnVector())

			assertEquals(vector, vector2)
			assertNotEquals(vector, vector3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorScalar_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorScalar_2(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
			Dim scalar As val = Nd4j.scalar(2.0f)
			Dim exp As val = Nd4j.createFromArray(New Single(){3, 4, 5, 6, 7})

			vector.addi(scalar)

			assertEquals(exp, vector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeScalar(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(2.0f)
			Dim newShape As val = scalar.reshape(1, 1, 1, 1)

			assertEquals(4, newShape.rank())
			assertArrayEquals(New Long(){1, 1, 1, 1}, newShape.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeVector(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5, 6})
			Dim newShape As val = vector.reshape(3, 2)

			assertEquals(2, newShape.rank())
			assertArrayEquals(New Long(){3, 2}, newShape.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTranspose1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim vector As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5, 6})
			assertArrayEquals(New Long(){6}, vector.shape())
			assertArrayEquals(New Long(){1}, vector.stride())
			Dim transposed As val = vector.transpose()
			assertArrayEquals(vector.shape(), transposed.shape())
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTranspose2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim scalar As val = Nd4j.scalar(2.0f)
			assertArrayEquals(New Long(){}, scalar.shape())
			assertArrayEquals(New Long(){}, scalar.stride())
			Dim transposed As val = scalar.transpose()
			assertArrayEquals(scalar.shape(), transposed.shape())
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmul_128by256(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmul_128by256(ByVal backend As Nd4jBackend)
			Dim mA As val = Nd4j.create(128, 156).assign(1.0f)
			Dim mB As val = Nd4j.create(156, 256).assign(1.0f)

			Dim mC As val = Nd4j.create(128, 256)
			Dim [mE] As val = Nd4j.create(128, 256).assign(156.0f)
			Dim mL As val = mA.mmul(mB)

			Dim op As val = DynamicCustomOp.builder("matmul").addInputs(mA, mB).addOutputs(mC).build()

			Nd4j.Executioner.exec(op)

			assertEquals([mE], mC)
		End Sub

	'    
	'        Analog of this TF code:
	'         a = tf.constant([], shape=[0,1])
	'         b = tf.constant([], shape=[1, 0])
	'         c = tf.matmul(a, b)
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmul_Empty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmul_Empty(ByVal backend As Nd4jBackend)
			Dim mA As val = Nd4j.create(0,1)
			Dim mB As val = Nd4j.create(1,0)
			Dim mC As val = Nd4j.create(0,0)

			Dim op As val = DynamicCustomOp.builder("matmul").addInputs(mA, mB).addOutputs(mC).build()

			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.create(0,0), mC)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmul_Empty1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmul_Empty1(ByVal backend As Nd4jBackend)
			Dim mA As val = Nd4j.create(1,0, 4)
			Dim mB As val = Nd4j.create(1,4, 0)
			Dim mC As val = Nd4j.create(1,0, 0)

			Dim op As val = DynamicCustomOp.builder("mmul").addInputs(mA, mB).addOutputs(mC).addIntegerArguments(0,0).build()

			Nd4j.Executioner.exec(op)
			assertEquals(Nd4j.create(1,0,0), mC)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarSqueeze(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarSqueeze(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.create(New Single(){2.0f}, New Long(){1, 1})
			Dim output As val = Nd4j.scalar(0.0f)
			Dim exp As val = Nd4j.scalar(2.0f)
			Dim op As val = DynamicCustomOp.builder("squeeze").addInputs(scalar).addOutputs(output).build()

			Dim shape As val = Nd4j.Executioner.calculateOutputShape(op)(0)
			assertArrayEquals(New Long(){}, shape.getShape())

			Nd4j.Executioner.exec(op)

			assertEquals(exp, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarVectorSqueeze(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarVectorSqueeze(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.create(New Single(){2.0f}, New Long(){1})

			assertArrayEquals(New Long(){1}, scalar.shape())

			Dim output As val = Nd4j.scalar(0.0f)
			Dim exp As val = Nd4j.scalar(2.0f)
			Dim op As val = DynamicCustomOp.builder("squeeze").addInputs(scalar).addOutputs(output).build()

			Dim shape As val = Nd4j.Executioner.calculateOutputShape(op)(0)
			assertArrayEquals(New Long(){}, shape.getShape())

			Nd4j.Executioner.exec(op)

			assertEquals(exp, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorSqueeze(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorSqueeze(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.create(New Single(){1, 2, 3, 4, 5, 6}, New Long(){1, 6})
			Dim output As val = Nd4j.createFromArray(New Single() {0, 0, 0, 0, 0, 0})
			Dim exp As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5, 6})

			Dim op As val = DynamicCustomOp.builder("squeeze").addInputs(vector).addOutputs(output).build()

			Dim shape As val = Nd4j.Executioner.calculateOutputShape(op)(0)
			assertArrayEquals(New Long(){6}, shape.getShape())

			Nd4j.Executioner.exec(op)

			assertEquals(exp, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrixReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixReshape(ByVal backend As Nd4jBackend)
			Dim matrix As val = Nd4j.create(New Single(){1, 2, 3, 4, 5, 6, 7, 8, 9}, New Long() {3, 3})
			Dim exp As val = Nd4j.create(New Single(){1, 2, 3, 4, 5, 6, 7, 8, 9}, New Long() {9})

			Dim reshaped As val = matrix.reshape(-1)

			assertArrayEquals(exp.shape(), reshaped.shape())
			assertEquals(exp, reshaped)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorScalarConcat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorScalarConcat(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.createFromArray(New Single() {1, 2})
			Dim scalar As val = Nd4j.scalar(3.0f)

			Dim output As val = Nd4j.createFromArray(New Single(){0, 0, 0})
			Dim exp As val = Nd4j.createFromArray(New Single(){1, 2, 3})

			Dim op As val = DynamicCustomOp.builder("concat").addInputs(vector, scalar).addOutputs(output).addIntegerArguments(0).build()

			Dim shape As val = Nd4j.Executioner.calculateOutputShape(op)(0)
			assertArrayEquals(exp.shape(), shape.getShape())

			Nd4j.Executioner.exec(op)

			assertArrayEquals(exp.shape(), output.shape())
			assertEquals(exp, output)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarPrint_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarPrint_1(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(3.0f)

			Nd4j.exec(New PrintVariable(scalar, True))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testValueArrayOf_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testValueArrayOf_1(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.valueArrayOf(New Long() {5}, 2f, DataType.FLOAT)
			Dim exp As val = Nd4j.createFromArray(New Single(){2, 2, 2, 2, 2})

			assertArrayEquals(exp.shape(), vector.shape())
			assertEquals(exp, vector)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testValueArrayOf_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testValueArrayOf_2(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.valueArrayOf(New Long() {}, 2f)
			Dim exp As val = Nd4j.scalar(2f)

			assertArrayEquals(exp.shape(), scalar.shape())
			assertEquals(exp, scalar)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayCreation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayCreation(ByVal backend As Nd4jBackend)
			Dim vector As val = Nd4j.create(New Single(){1, 2, 3}, New Long() {3}, "c"c)
			Dim exp As val = Nd4j.createFromArray(New Single(){1, 2, 3})

			assertArrayEquals(exp.shape(), vector.shape())
			assertEquals(exp, vector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testACosh()
		Public Overridable Sub testACosh()
			'http://www.wolframalpha.com/input/?i=acosh(x)

			Dim [in] As INDArray = Nd4j.linspace(1, 3, 20, DataType.DOUBLE)
			Dim [out] As INDArray = Nd4j.Executioner.exec(New ACosh([in].dup()))

			Dim exp As INDArray = Nd4j.create([in].shape())
			For i As Integer = 0 To [in].length() - 1
				Dim x As Double = [in].getDouble(i)
				Dim y As Double = Math.Log(x + Math.Sqrt(x-1) * Math.Sqrt(x+1))
				exp.putScalar(i, y)
			Next i

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosh()
		Public Overridable Sub testCosh()
			'http://www.wolframalpha.com/input/?i=cosh(x)

			Dim [in] As INDArray = Nd4j.linspace(-2, 2, 20, DataType.DOUBLE)
			Dim [out] As INDArray = Transforms.cosh([in], True)

			Dim exp As INDArray = Nd4j.create([in].shape())
			For i As Integer = 0 To [in].length() - 1
				Dim x As Double = [in].getDouble(i)
				Dim y As Double = 0.5 * (Math.Exp(-x) + Math.Exp(x))
				exp.putScalar(i, y)
			Next i

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAtanh()
		Public Overridable Sub testAtanh()
			'http://www.wolframalpha.com/input/?i=atanh(x)

			Dim [in] As INDArray = Nd4j.linspace(-0.9, 0.9, 10, DataType.DOUBLE)
			Dim [out] As INDArray = Transforms.atanh([in], True)

			Dim exp As INDArray = Nd4j.create([in].shape())
			For i As Integer = 0 To 9
				Dim x As Double = [in].getDouble(i)
				'Using "alternative form" from: http://www.wolframalpha.com/input/?i=atanh(x)
				Dim y As Double = 0.5 * Math.Log(x+1.0) - 0.5 * Math.Log(1.0-x)
				exp.putScalar(i, y)
			Next i

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLastIndex()
		Public Overridable Sub testLastIndex()

			Dim [in] As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 1, 1, 0},
				New Double() {1, 1, 0, 0}
			})

			Dim exp0 As INDArray = Nd4j.create(New Long(){1, 1, 0, -1}, New Long(){4}, DataType.LONG)
			Dim exp1 As INDArray = Nd4j.create(New Long(){2, 1}, New Long(){2}, DataType.LONG)

			Dim out0 As INDArray = BooleanIndexing.lastIndex([in], Conditions.equals(1), 0)
			Dim out1 As INDArray = BooleanIndexing.lastIndex([in], Conditions.equals(1), 1)

			assertEquals(exp0, out0)
			assertEquals(exp1, out1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadReduce3Call(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadReduce3Call(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim x As val = Nd4j.create(400,20)
			Dim y As val = Nd4j.ones(1, 20)
			x.distance2(y)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3AlexBug(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3AlexBug(ByVal backend As Nd4jBackend)
			Dim arr As val = Nd4j.linspace(1,100,100, DataType.DOUBLE).reshape("f"c, 10, 10).dup("c"c)
			Dim arr2 As val = Nd4j.linspace(1,100,100, DataType.DOUBLE).reshape("c"c, 10, 10)
			Dim [out] As val = Nd4j.Executioner.exec(New EuclideanDistance(arr, arr2, 1))
			Dim exp As val = Nd4j.create(New Double() {151.93748, 128.86038, 108.37435, 92.22256, 82.9759, 82.9759, 92.22256, 108.37435, 128.86038, 151.93748})

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllDistancesEdgeCase1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllDistancesEdgeCase1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(400, 20).assign(2.0).castTo(Nd4j.defaultFloatingPointType())
			Dim y As val = Nd4j.ones(1, 20).castTo(Nd4j.defaultFloatingPointType())
			Dim z As val = Transforms.allEuclideanDistances(x, y, 1)

			Dim exp As val = Nd4j.create(400, 1).assign(4.47214)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat_1(ByVal backend As Nd4jBackend)
			For Each order As Char In New Char(){"c"c, "f"c}

				Dim arr1 As INDArray = Nd4j.create(New Double(){1, 2}, New Long(){1, 2}, order)
				Dim arr2 As INDArray = Nd4j.create(New Double(){3, 4}, New Long(){1, 2}, order)

				Dim [out] As INDArray = Nd4j.concat(0, arr1, arr2)
				Nd4j.Executioner.commit()
				Dim exp As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 2},
					New Double() {3, 4}
				})
				assertEquals(exp, [out],order.ToString())
			Next order
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRdiv()
		Public Overridable Sub testRdiv()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.create(new double[]{2.0, 2.0, 2.0, 2.0});
			Dim a As INDArray = Nd4j.create(New Double(){2.0, 2.0, 2.0, 2.0})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.create(new double[]{1.0, 2.0, 4.0, 8.0});
			Dim b As INDArray = Nd4j.create(New Double(){1.0, 2.0, 4.0, 8.0})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray c = org.nd4j.linalg.factory.Nd4j.create(new double[]{2.0, 2.0}).reshape(2, 1);
			Dim c As INDArray = Nd4j.create(New Double(){2.0, 2.0}).reshape(ChrW(2), 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray d = org.nd4j.linalg.factory.Nd4j.create(new double[]{1.0, 2.0, 4.0, 8.0}).reshape(2, 2);
			Dim d As INDArray = Nd4j.create(New Double(){1.0, 2.0, 4.0, 8.0}).reshape(ChrW(2), 2)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = org.nd4j.linalg.factory.Nd4j.create(new double[]{2.0, 1.0, 0.5, 0.25});
			Dim expected As INDArray = Nd4j.create(New Double(){2.0, 1.0, 0.5, 0.25})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected2 = org.nd4j.linalg.factory.Nd4j.create(new double[]{2.0, 1.0, 0.5, 0.25}).reshape(2, 2);
			Dim expected2 As INDArray = Nd4j.create(New Double(){2.0, 1.0, 0.5, 0.25}).reshape(ChrW(2), 2)

			assertEquals(expected, a.div(b))
			assertEquals(expected, b.rdiv(a))
			assertEquals(expected, b.rdiv(2))
			assertEquals(expected2, d.rdivColumnVector(c))

			assertEquals(expected, b.rdiv(Nd4j.scalar(2.0)))
			assertEquals(expected, b.rdivColumnVector(Nd4j.scalar(2)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRsub()
		Public Overridable Sub testRsub()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.create(new double[]{2.0, 2.0, 2.0, 2.0});
			Dim a As INDArray = Nd4j.create(New Double(){2.0, 2.0, 2.0, 2.0})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.create(new double[]{1.0, 2.0, 4.0, 8.0});
			Dim b As INDArray = Nd4j.create(New Double(){1.0, 2.0, 4.0, 8.0})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray c = org.nd4j.linalg.factory.Nd4j.create(new double[]{2.0, 2.0}).reshape(2, 1);
			Dim c As INDArray = Nd4j.create(New Double(){2.0, 2.0}).reshape(ChrW(2), 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray d = org.nd4j.linalg.factory.Nd4j.create(new double[]{1.0, 2.0, 4.0, 8.0}).reshape("c"c,2, 2);
			Dim d As INDArray = Nd4j.create(New Double(){1.0, 2.0, 4.0, 8.0}).reshape("c"c,2, 2)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = org.nd4j.linalg.factory.Nd4j.create(new double[]{1.0, 0.0, -2.0, -6.0});
			Dim expected As INDArray = Nd4j.create(New Double(){1.0, 0.0, -2.0, -6.0})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected2 = org.nd4j.linalg.factory.Nd4j.create(new double[]{1, 0, -2.0, -6.0}).reshape("c"c,2, 2);
			Dim expected2 As INDArray = Nd4j.create(New Double(){1, 0, -2.0, -6.0}).reshape("c"c,2, 2)

			assertEquals(expected, a.sub(b))
			assertEquals(expected, b.rsub(a))
			assertEquals(expected, b.rsub(2))
			assertEquals(expected2, d.rsubColumnVector(c))

			assertEquals(expected, b.rsub(Nd4j.scalar(2)))
			assertEquals(expected, b.rsubColumnVector(Nd4j.scalar(2)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHalfStuff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHalfStuff(ByVal backend As Nd4jBackend)
			If Not Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim dtype As val = Nd4j.dataType()
			Nd4j.DataType = DataType.HALF

			Dim arr As val = Nd4j.ones(3, 3)
			arr.addi(2.0f)

			Dim exp As val = Nd4j.create(3, 3).assign(3.0f)

			assertEquals(exp, arr)

			Nd4j.DataType = dtype
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) public void testInconsistentOutput(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInconsistentOutput(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.rand(1, 802816).castTo(DataType.DOUBLE)
			Dim W As INDArray = Nd4j.rand(802816, 1).castTo(DataType.DOUBLE)
			Dim b As INDArray = Nd4j.create(1).castTo(DataType.DOUBLE)
			Dim [out] As INDArray = fwd([in], W, b)

			For i As Integer = 0 To 99
				Dim out2 As INDArray = fwd([in], W, b) 'l.activate(inToLayer1, false, LayerWorkspaceMgr.noWorkspaces());
				assertEquals([out], out2,"Failed at iteration [" & i & "]")
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test3D_create_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test3D_create_1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim jArray As val = new Single[2][3][4]
			Dim jArray As val = RectangularArrays.RectangularSingleArray(2, 3, 4)

			fillJvmArray3D(jArray)

			Dim iArray As val = Nd4j.create(jArray)
			Dim fArray As val = ArrayUtil.flatten(jArray)

			assertArrayEquals(New Long(){2, 3, 4}, iArray.shape())

			assertArrayEquals(fArray, iArray.data().asFloat(), 1e-5f)

			Dim cnt As Integer = 0
			For Each f As val In fArray
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertTrue(f > 0.0f,"Failed for element [" + cnt++ +"]");
				assertTrue(f > 0.0f,"Failed for element [" & cnt & "]")
					cnt += 1
			Next f
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test4D_create_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test4D_create_1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim jArray As val = new Single[2][3][4][5]
			Dim jArray As val = RectangularArrays.RectangularSingleArray(2, 3, 4, 5)

			fillJvmArray4D(jArray)

			Dim iArray As val = Nd4j.create(jArray)
			Dim fArray As val = ArrayUtil.flatten(jArray)

			assertArrayEquals(New Long(){2, 3, 4, 5}, iArray.shape())

			assertArrayEquals(fArray, iArray.data().asFloat(), 1e-5f)

			Dim cnt As Integer = 0
			For Each f As val In fArray
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertTrue(f > 0.0f,"Failed for element [" + cnt++ +"]");
				assertTrue(f > 0.0f,"Failed for element [" & cnt & "]")
					cnt += 1
			Next f
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcast_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcast_1(ByVal backend As Nd4jBackend)
			Dim array1 As val = Nd4j.linspace(1, 10, 10, DataType.DOUBLE).reshape(ChrW(5), 1, 2).broadcast(5, 4, 2)
			Dim array2 As val = Nd4j.linspace(1, 20, 20, DataType.DOUBLE).reshape(ChrW(5), 4, 1).broadcast(5, 4, 2)
			Dim exp As val = Nd4j.create(New Double() {2.0f, 3.0f, 3.0f, 4.0f, 4.0f, 5.0f, 5.0f, 6.0f, 8.0f, 9.0f, 9.0f, 10.0f, 10.0f, 11.0f, 11.0f, 12.0f, 14.0f, 15.0f, 15.0f, 16.0f, 16.0f, 17.0f, 17.0f, 18.0f, 20.0f, 21.0f, 21.0f, 22.0f, 22.0f, 23.0f, 23.0f, 24.0f, 26.0f, 27.0f, 27.0f, 28.0f, 28.0f, 29.0f, 29.0f, 30.0f}).reshape(ChrW(5), 4, 2)

			array1.addi(array2)

			assertEquals(exp, array1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddiColumnEdge()
		Public Overridable Sub testAddiColumnEdge()
			Dim arr1 As INDArray = Nd4j.create(1, 5)
			arr1.addiColumnVector(Nd4j.ones(1))
			assertEquals(Nd4j.ones(1,5), arr1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulViews_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulViews_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.linspace(1, 27, 27, DataType.DOUBLE).reshape(ChrW(3), 3, 3)

			Dim arrayA As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)

			Dim arrayB As val = arrayX.dup("f"c)

'JAVA TO VB CONVERTER NOTE: The variable arraya was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim arraya_Conflict As val = arrayX.slice(0)
'JAVA TO VB CONVERTER NOTE: The variable arrayb was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim arrayb_Conflict As val = arrayB.slice(0)

			Dim exp As val = arrayA.mmul(arrayA)

			assertEquals(exp, arraya_Conflict.mmul(arrayA))
			assertEquals(exp, arraya_Conflict.mmul(arraya_Conflict))

			assertEquals(exp, arrayb_Conflict.mmul(arrayb_Conflict))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTile_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTile_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim exp As val = Nd4j.create(New Double() {1.000000, 2.000000, 3.000000, 1.000000, 2.000000, 3.000000, 4.000000, 5.000000, 6.000000, 4.000000, 5.000000, 6.000000, 1.000000, 2.000000, 3.000000, 1.000000, 2.000000, 3.000000, 4.000000, 5.000000, 6.000000, 4.000000, 5.000000, 6.000000}, New Integer() {4, 6})
			Dim output As val = Nd4j.create(4, 6)

			Dim op As val = DynamicCustomOp.builder("tile").addInputs(array).addIntegerArguments(2, 2).addOutputs(output).build()

			Nd4j.Executioner.exec(op)

			assertEquals(exp, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRelativeError_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRelativeError_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)
			Dim arrayY As val = Nd4j.ones(10, 10)
			Dim exp As val = Nd4j.ones(10, 10)

			Nd4j.Executioner.exec(New BinaryRelativeError(arrayX, arrayY, arrayX, 0.1))

			assertEquals(exp, arrayX)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBugMeshgridOnDoubleArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBugMeshgridOnDoubleArray(ByVal backend As Nd4jBackend)
			Nd4j.meshgrid(Nd4j.create(New Double() { 1, 2, 3 }), Nd4j.create(New Double() { 4, 5, 6 }))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeshGrid()
		Public Overridable Sub testMeshGrid()

			Dim x1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}).reshape(ChrW(1), -1)
			Dim y1 As INDArray = Nd4j.create(New Double(){5, 6, 7}).reshape(ChrW(1), -1)

			Dim expX As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2, 3, 4},
				New Double() {1, 2, 3, 4},
				New Double() {1, 2, 3, 4}
			})
			Dim expY As INDArray = Nd4j.create(New Double()(){
				New Double() {5, 5, 5, 5},
				New Double() {6, 6, 6, 6},
				New Double() {7, 7, 7, 7}
			})
			Dim exp() As INDArray = {expX, expY}

			Dim out1() As INDArray = Nd4j.meshgrid(x1, y1)
			assertArrayEquals(exp, out1)

			Dim out2() As INDArray = Nd4j.meshgrid(x1.transpose(), y1.transpose())
			assertArrayEquals(exp, out2)

			Dim out3() As INDArray = Nd4j.meshgrid(x1, y1.transpose())
			assertArrayEquals(exp, out3)

			Dim out4() As INDArray = Nd4j.meshgrid(x1.transpose(), y1)
			assertArrayEquals(exp, out4)

			'Test views:
			Dim x2 As INDArray = Nd4j.create(1,9).get(NDArrayIndex.all(), NDArrayIndex.interval(1,2,7, True)).assign(x1)
			Dim y2 As INDArray = Nd4j.create(1,7).get(NDArrayIndex.all(), NDArrayIndex.interval(1,2,5, True)).assign(y1)

			Dim out5() As INDArray = Nd4j.meshgrid(x2, y2)
			assertArrayEquals(exp, out5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAccumuationWithoutAxis_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAccumuationWithoutAxis_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(3, 3).assign(1.0)

			Dim result As val = array.sum()

			assertEquals(1, result.length())
			assertEquals(9.0, result.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSummaryStatsEquality_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSummaryStatsEquality_1(ByVal backend As Nd4jBackend)
	'        log.info("Datatype: {}", Nd4j.dataType());

			For Each biasCorrected As Boolean In New Boolean(){False, True}

				Dim indArray1 As INDArray = Nd4j.rand(1, 4, 10).castTo(DataType.DOUBLE)
				Dim std As Double = indArray1.stdNumber(biasCorrected).doubleValue()

				Dim standardDeviation As val = New org.apache.commons.math3.stat.descriptive.moment.StandardDeviation(biasCorrected)
				Dim std2 As Double = standardDeviation.evaluate(indArray1.data().asDouble())
	'            log.info("Bias corrected = {}", biasCorrected);
	'            log.info("nd4j std: {}", std);
	'            log.info("apache math3 std: {}", std2);

				assertEquals(std, std2, 1e-5)
			Next biasCorrected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanEdgeCase_C()
		Public Overridable Sub testMeanEdgeCase_C()
			Dim arr As INDArray = Nd4j.linspace(1, 30,30, DataType.DOUBLE).reshape(New Integer(){3, 10, 1}).dup("c"c)
			Dim arr2 As INDArray = arr.mean(2)

			Dim exp As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0))

			assertEquals(exp, arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanEdgeCase_F()
		Public Overridable Sub testMeanEdgeCase_F()
			Dim arr As INDArray = Nd4j.linspace(1, 30,30, DataType.DOUBLE).reshape(New Integer(){3, 10, 1}).dup("f"c)
			Dim arr2 As INDArray = arr.mean(2)

			Dim exp As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0))

			assertEquals(exp, arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanEdgeCase2_C()
		Public Overridable Sub testMeanEdgeCase2_C()
			Dim arr As INDArray = Nd4j.linspace(1, 60,60, DataType.DOUBLE).reshape(New Integer(){3, 10, 2}).dup("c"c)
			Dim arr2 As INDArray = arr.mean(2)

			Dim exp As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0))
			exp.addi(arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(1)))
			exp.divi(2)


			assertEquals(exp, arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanEdgeCase2_F()
		Public Overridable Sub testMeanEdgeCase2_F()
			Dim arr As INDArray = Nd4j.linspace(1, 60,60, DataType.DOUBLE).reshape(New Integer(){3, 10, 2}).dup("f"c)
			Dim arr2 As INDArray = arr.mean(2)

			Dim exp As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0))
			exp.addi(arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(1)))
			exp.divi(2)


			assertEquals(exp, arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLegacyDeserialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLegacyDeserialization_1()
			Dim f As val = (New ClassPathResource("legacy/NDArray_javacpp.bin")).File

			Dim array As val = Nd4j.read(New FileStream(f, FileMode.Open, FileAccess.Read))
			Dim exp As val = Nd4j.linspace(1, 120, 120, DataType.DOUBLE).reshape(ChrW(2), 3, 4, 5)

			assertEquals(120, array.length())
			assertArrayEquals(New Long(){2, 3, 4, 5}, array.shape())
			assertEquals(exp, array)

			Dim bos As val = New MemoryStream()
			Nd4j.write(bos, array)

			Dim bis As val = New MemoryStream(bos.toByteArray())
			Dim array2 As val = Nd4j.read(bis)

			assertEquals(exp, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRndBloat16(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRndBloat16(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(DataType.BFLOAT16, "c"c, New Long(){5})
			assertTrue(x.sumNumber().floatValue() > 0)

			x = Nd4j.randn(DataType.BFLOAT16, 10)
			assertTrue(x.sumNumber().floatValue() <> 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLegacyDeserialization_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLegacyDeserialization_2()
			Dim f As val = (New ClassPathResource("legacy/NDArray_longshape_float.bin")).File

			Dim array As val = Nd4j.read(New FileStream(f, FileMode.Open, FileAccess.Read))
			Dim exp As val = Nd4j.linspace(1, 5, 5, DataType.FLOAT).reshape(ChrW(1), -1)

			assertEquals(5, array.length())
			assertArrayEquals(New Long(){1, 5}, array.shape())
			assertEquals(exp.dataType(), array.dataType())
			assertEquals(exp, array)

			Dim bos As val = New MemoryStream()
			Nd4j.write(bos, array)

			Dim bis As val = New MemoryStream(bos.toByteArray())
			Dim array2 As val = Nd4j.read(bis)

			assertEquals(exp, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLegacyDeserialization_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLegacyDeserialization_3()
			Dim f As val = (New ClassPathResource("legacy/NDArray_longshape_double.bin")).File

			Dim array As val = Nd4j.read(New FileStream(f, FileMode.Open, FileAccess.Read))
			Dim exp As val = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).reshape(ChrW(1), -1)

			assertEquals(5, array.length())
			assertArrayEquals(New Long(){1, 5}, array.shape())
			assertEquals(exp, array)

			Dim bos As val = New MemoryStream()
			Nd4j.write(bos, array)

			Dim bis As val = New MemoryStream(bos.toByteArray())
			Dim array2 As val = Nd4j.read(bis)

			assertEquals(exp, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTearPile_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTearPile_1(ByVal backend As Nd4jBackend)
			Dim source As val = Nd4j.rand(New Integer(){10, 15}).castTo(DataType.DOUBLE)

			Dim list As val = Nd4j.tear(source, 1)

			' just want to ensure that axis is right one
			assertEquals(10, list.length)

			Dim result As val = Nd4j.pile(list)

			assertEquals(source.shapeInfoDataBuffer(), result.shapeInfoDataBuffer())
			assertEquals(source, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariance_4D_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariance_4D_1(ByVal backend As Nd4jBackend)
			Dim dtype As val = Nd4j.dataType()

			Nd4j.DataType = DataType.FLOAT

			Dim x As val = Nd4j.ones(10, 20, 30, 40)
			Dim result As val = x.var(False, 0, 2, 3)

			Nd4j.Executioner.commit()

	'        log.info("Result shape: {}", result.shapeInfoDataBuffer().asLong());

			Nd4j.DataType = dtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose_Custom()
		Public Overridable Sub testTranspose_Custom()

			Dim arr As INDArray = Nd4j.linspace(1,15, 15, DataType.DOUBLE).reshape(ChrW(5), 3)
			Dim [out] As INDArray = Nd4j.create(3,5)

			Dim op As val = DynamicCustomOp.builder("transpose").addInputs(arr).addOutputs([out]).build()

			Nd4j.Executioner.exec(op)

			Dim exp As val = arr.transpose()
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) public void testRowColumnOpsRank1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowColumnOpsRank1(ByVal backend As Nd4jBackend)

			For i As Integer = 0 To 5
				Dim orig As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4)
				Dim in1r As INDArray = orig.dup()
				Dim in2r As INDArray = orig.dup()
				Dim in1c As INDArray = orig.dup()
				Dim in2c As INDArray = orig.dup()

				Dim rv1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Long(){1, 4})
				Dim rv2 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Long(){4})
				Dim cv1 As INDArray = Nd4j.create(New Double(){1, 2, 3}, New Long(){3, 1})
				Dim cv2 As INDArray = Nd4j.create(New Double(){1, 2, 3}, New Long(){3})

				Select Case i
					Case 0
						in1r.addiRowVector(rv1)
						in2r.addiRowVector(rv2)
						in1c.addiColumnVector(cv1)
						in2c.addiColumnVector(cv2)
					Case 1
						in1r.subiRowVector(rv1)
						in2r.subiRowVector(rv2)
						in1c.subiColumnVector(cv1)
						in2c.subiColumnVector(cv2)
					Case 2
						in1r.muliRowVector(rv1)
						in2r.muliRowVector(rv2)
						in1c.muliColumnVector(cv1)
						in2c.muliColumnVector(cv2)
					Case 3
						in1r.diviRowVector(rv1)
						in2r.diviRowVector(rv2)
						in1c.diviColumnVector(cv1)
						in2c.diviColumnVector(cv2)
					Case 4
						in1r.rsubiRowVector(rv1)
						in2r.rsubiRowVector(rv2)
						in1c.rsubiColumnVector(cv1)
						in2c.rsubiColumnVector(cv2)
					Case 5
						in1r.rdiviRowVector(rv1)
						in2r.rdiviRowVector(rv2)
						in1c.rdiviColumnVector(cv1)
						in2c.rdiviColumnVector(cv2)
					Case Else
						Throw New Exception()
				End Select


				assertEquals(in1r, in2r)
				assertEquals(in1c, in2c)

			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyShapeRank0()
		Public Overridable Sub testEmptyShapeRank0()
			Nd4j.Random.setSeed(12345)
			Dim s(-1) As Integer
			Dim create As INDArray = Nd4j.create(s).castTo(DataType.DOUBLE)
			Dim zeros As INDArray = Nd4j.zeros(s).castTo(DataType.DOUBLE)
			Dim ones As INDArray = Nd4j.ones(s).castTo(DataType.DOUBLE)
			Dim uninit As INDArray = Nd4j.createUninitialized(s).assign(0).castTo(DataType.DOUBLE)
			Dim rand As INDArray = Nd4j.rand(s).castTo(DataType.DOUBLE)

			Dim tsZero As INDArray = Nd4j.scalar(0.0).castTo(DataType.DOUBLE)
			Dim tsOne As INDArray = Nd4j.scalar(1.0).castTo(DataType.DOUBLE)
			Nd4j.Random.setSeed(12345)
			Dim tsRand As INDArray = Nd4j.scalar(Nd4j.rand(New Integer(){1, 1}).getDouble(0)).castTo(DataType.DOUBLE)
			assertEquals(tsZero, create)
			assertEquals(tsZero, zeros)
			assertEquals(tsOne, ones)
			assertEquals(tsZero, uninit)
			assertEquals(tsRand, rand)


			Nd4j.Random.setSeed(12345)
			Dim s2(-1) As Long
			create = Nd4j.create(s2).castTo(DataType.DOUBLE)
			zeros = Nd4j.zeros(s2).castTo(DataType.DOUBLE)
			ones = Nd4j.ones(s2).castTo(DataType.DOUBLE)
			uninit = Nd4j.createUninitialized(s2).assign(0).castTo(DataType.DOUBLE)
			rand = Nd4j.rand(s2).castTo(DataType.DOUBLE)

			assertEquals(tsZero, create)
			assertEquals(tsZero, zeros)
			assertEquals(tsOne, ones)
			assertEquals(tsZero, uninit)
			assertEquals(tsRand, rand)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarView_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarView_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)
			Dim exp As val = Nd4j.create(New Double(){1.0, 2.0, 5.0, 4.0, 5.0})
			Dim scalar As val = array.getScalar(2)

			assertEquals(3.0, scalar.getDouble(0), 1e-5)
			scalar.addi(2.0)

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarView_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarView_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim exp As val = Nd4j.create(New Double(){1.0, 2.0, 5.0, 4.0}).reshape(ChrW(2), 2)
			Dim scalar As val = array.getScalar(1, 0)

			assertEquals(3.0, scalar.getDouble(0), 1e-5)
			scalar.addi(2.0)

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSomething_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSomething_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(128, 128, "f"c)
			Dim arrayY As val = Nd4j.create(128, 128, "f"c)
			Dim arrayZ As val = Nd4j.create(128, 128, "f"c)

			Dim iterations As Integer = 100
			' warmup
			For e As Integer = 0 To 9
				arrayX.addi(arrayY)
			Next e

			For e As Integer = 0 To iterations - 1
				Dim c As val = New GemmParams(arrayX, arrayY, arrayZ)
			Next e

			Dim tS As val = System.nanoTime()
			For e As Integer = 0 To iterations - 1
				'val c = new GemmParams(arrayX, arrayY, arrayZ);
				arrayX.mmuli(arrayY, arrayZ)
			Next e

			Dim tE As val = System.nanoTime()

			log.info("Average time: {}", ((tE - tS) / iterations))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexesIteration_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIndexesIteration_1(ByVal backend As Nd4jBackend)
			Dim arrayC As val = Nd4j.linspace(1, 60, 60, DataType.DOUBLE).reshape(ChrW(3), 4, 5)
			Dim arrayF As val = arrayC.dup("f"c)

			Dim iter As val = New NdIndexIterator(arrayC.ordering(), arrayC.shape())
			Do While iter.hasNext()
				Dim idx As val = iter.next()

				Dim c As val = arrayC.getDouble(idx)
				Dim f As val = arrayF.getDouble(idx)

				assertEquals(c, f, 1e-5)
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexesIteration_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIndexesIteration_2(ByVal backend As Nd4jBackend)
			Dim arrayC As val = Nd4j.linspace(1, 60, 60, DataType.DOUBLE).reshape(ChrW(3), 4, 5)
			Dim arrayF As val = arrayC.dup("f"c)

			Dim iter As val = New NdIndexIterator(arrayC.ordering(), arrayC.shape())
			Do While iter.hasNext()
				Dim idx As val = iter.next()

				Dim c As var = arrayC.getDouble(idx)
				Dim f As var = arrayF.getDouble(idx)

				arrayC.putScalar(idx, c + 1.0)
				arrayF.putScalar(idx, f + 1.0)

				c = arrayC.getDouble(idx)
				f = arrayF.getDouble(idx)

				assertEquals(c, f, 1e-5)
			Loop
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPairwiseScalar_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPairwiseScalar_1(ByVal backend As Nd4jBackend)
			Dim exp_1 As val = Nd4j.create(New Double(){2.0, 3.0, 4.0}, New Long(){3})
			Dim exp_2 As val = Nd4j.create(New Double(){0.0, 1.0, 2.0}, New Long(){3})
			Dim exp_3 As val = Nd4j.create(New Double(){1.0, 2.0, 3.0}, New Long(){3})
			Dim arrayX As val = Nd4j.create(New Double(){1.0, 2.0, 3.0}, New Long(){3})
			Dim arrayY As val = Nd4j.scalar(1.0)

			Dim arrayZ_1 As val = arrayX.add(arrayY)
			assertEquals(exp_1, arrayZ_1)

			Dim arrayZ_2 As val = arrayX.sub(arrayY)
			assertEquals(exp_2, arrayZ_2)

			Dim arrayZ_3 As val = arrayX.div(arrayY)
			assertEquals(exp_3, arrayZ_3)

			Dim arrayZ_4 As val = arrayX.mul(arrayY)
			assertEquals(exp_3, arrayZ_4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLTOE_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLTOE_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(New Double(){1.0, 2.0, 3.0, -1.0})
			Dim y As val = Nd4j.create(New Double(){2.0, 2.0, 3.0, -2.0})

			Dim ex As val = Nd4j.create(New Double(){1.0, 2.0, 3.0, -1.0})
			Dim ey As val = Nd4j.create(New Double(){2.0, 2.0, 3.0, -2.0})

			Dim ez As val = Nd4j.create(New Boolean(){True, True, True, False})
			Dim z As val = Transforms.lessThanOrEqual(x, y, True)

			assertEquals(ex, x)
			assertEquals(ey, y)

			assertEquals(ez, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGTOE_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGTOE_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(New Double(){1.0, 2.0, 3.0, -1.0})
			Dim y As val = Nd4j.create(New Double(){2.0, 2.0, 3.0, -2.0})

			Dim ex As val = Nd4j.create(New Double(){1.0, 2.0, 3.0, -1.0})
			Dim ey As val = Nd4j.create(New Double(){2.0, 2.0, 3.0, -2.0})

			Dim ez As val = Nd4j.create(New Boolean(){False, True, True, True}, New Long(){4}, DataType.BOOL)
			Dim z As val = Transforms.greaterThanOrEqual(x, y, True)

			Dim str As val = ez.ToString()
	'        log.info("exp: {}", str);

			assertEquals(ex, x)
			assertEquals(ey, y)

			assertEquals(ez, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastInvalid()
		Public Overridable Sub testBroadcastInvalid()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim arr1 As INDArray = Nd4j.ones(3,4,1)
			Dim arrInvalid As INDArray = Nd4j.create(3,12)
			Nd4j.Executioner.exec(New BroadcastMulOp(arr1, arrInvalid, arr1, 0, 2))
			fail("Excepted exception on invalid input")
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGet()
		Public Overridable Sub testGet()
			'https://github.com/eclipse/deeplearning4j/issues/6133
			Dim m As INDArray = Nd4j.linspace(0,99,100, DataType.DOUBLE).reshape("c"c, 10,10)
			Dim exp As INDArray = Nd4j.create(New Double(){5, 15, 25, 35, 45, 55, 65, 75, 85, 95}, New Integer(){10})
			Dim col As INDArray = m.getColumn(5)

			For i As Integer = 0 To 9
				col.slice(i)
	'            System.out.println(i + "\t" + col.slice(i));
			Next i

			'First element: index 5
			'Last element: index 95
			'91 total elements
			assertEquals(5, m.getDouble(5), 1e-6)
			assertEquals(95, m.getDouble(95), 1e-6)
			assertEquals(91, col.data().length())

			assertEquals(exp, col)
			assertEquals(exp.ToString(), col.ToString())
			assertArrayEquals(exp.toDoubleVector(), col.toDoubleVector(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhere1()
		Public Overridable Sub testWhere1()

			Dim arr As INDArray = Nd4j.create(New Boolean()(){
				New Boolean() {False, True, False},
				New Boolean() {False, False, True},
				New Boolean() {False, False, True}
			})
			Dim exp() As INDArray = { Nd4j.createFromArray(New Long(){0, 1, 2}), Nd4j.createFromArray(New Long(){1, 2, 2})}

			Dim act() As INDArray = Nd4j.where(arr, Nothing, Nothing)

			assertArrayEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhere2()
		Public Overridable Sub testWhere2()

			Dim arr As INDArray = Nd4j.create(DataType.BOOL, 3,3,3)
			arr.putScalar(0,1,0,1.0)
			arr.putScalar(1,2,1,1.0)
			arr.putScalar(2,2,1,1.0)
			Dim exp() As INDArray = { Nd4j.createFromArray(New Long(){0, 1, 2}), Nd4j.createFromArray(New Long(){1, 2, 2}), Nd4j.createFromArray(New Long(){0, 1, 1}) }

			Dim act() As INDArray = Nd4j.where(arr, Nothing, Nothing)

			assertArrayEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhere3()
		Public Overridable Sub testWhere3()
			Dim arr As INDArray = Nd4j.create(New Boolean()(){
				New Boolean() {False, True, False},
				New Boolean() {False, False, True},
				New Boolean() {False, False, True}
			})
			Dim x As INDArray = Nd4j.valueArrayOf(3, 3, 1.0)
			Dim y As INDArray = Nd4j.valueArrayOf(3, 3, 2.0)
			Dim exp As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2, 1},
				New Double() {1, 1, 2},
				New Double() {1, 1, 2}
			})

			Dim act() As INDArray = Nd4j.where(arr, x, y)
			assertEquals(1, act.Length)

			assertEquals(exp, act(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhereEmpty()
		Public Overridable Sub testWhereEmpty()
			Dim inArray As INDArray = Nd4j.zeros(2, 3)
			inArray.putScalar(0, 0, 10.0f)
			inArray.putScalar(1, 2, 10.0f)

			Dim mask1 As INDArray = inArray.match(1, Conditions.greaterThanOrEqual(1))

			assertEquals(1, mask1.castTo(DataType.INT).maxNumber().intValue()) ' ! Not Empty Match

			Dim matchIndexes() As INDArray = Nd4j.where(mask1, Nothing, Nothing)

			assertArrayEquals(New Integer() {0, 1}, matchIndexes(0).toIntVector())
			assertArrayEquals(New Integer() {0, 2}, matchIndexes(1).toIntVector())

			Dim mask2 As INDArray = inArray.match(1, Conditions.greaterThanOrEqual(11))

			assertEquals(0, mask2.castTo(DataType.INT).maxNumber().intValue())

			Dim matchIndexes2() As INDArray = Nd4j.where(mask2, Nothing, Nothing)
			For i As Integer = 0 To matchIndexes2.Length - 1
				assertTrue(matchIndexes2(i).Empty)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarEquality_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarEquality_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.scalar(1.0f)
			Dim e As val = Nd4j.scalar(3.0f)

			x.addi(2.0f)

			assertEquals(e, x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStack()
		Public Overridable Sub testStack()
			Dim [in] As INDArray = Nd4j.linspace(1,12,12, DataType.DOUBLE).reshape(ChrW(3), 4)
			Dim in2 As INDArray = [in].add(100)

			For i As Integer = -3 To 2
				Dim [out] As INDArray = Nd4j.stack(i, [in], in2)
				Dim expShape() As Long
				Select Case i
					Case -3, 0
						expShape = New Long(){2, 3, 4}
					Case -2, 1
						expShape = New Long(){3, 2, 4}
					Case -1, 2
						expShape = New Long(){3, 4, 2}
					Case Else
						Throw New Exception(i.ToString())
				End Select
				assertArrayEquals(expShape, [out].shape(),i.ToString())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutSpecifiedIndex()
		Public Overridable Sub testPutSpecifiedIndex()
			Dim ss()() As Long = {
				New Long() {3, 4},
				New Long() {3, 4, 5},
				New Long() {3, 4, 5, 6}
			}
			Dim st()() As Long = {
				New Long() {4, 4},
				New Long() {4, 4, 5},
				New Long() {4, 4, 5, 6}
			}
			Dim ds()() As Long = {
				New Long() {1, 4},
				New Long() {1, 4, 5},
				New Long() {1, 4, 5, 6}
			}

			For test As Integer = 0 To ss.Length - 1
				Dim shapeSource() As Long = ss(test)
				Dim shapeTarget() As Long = st(test)
				Dim diffShape() As Long = ds(test)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray source = org.nd4j.linalg.factory.Nd4j.ones(shapeSource);
				Dim source As INDArray = Nd4j.ones(shapeSource)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray target = org.nd4j.linalg.factory.Nd4j.zeros(shapeTarget);
				Dim target As INDArray = Nd4j.zeros(shapeTarget)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.indexing.INDArrayIndex[] targetIndexes = new org.nd4j.linalg.indexing.INDArrayIndex[shapeTarget.length];
				Dim targetIndexes(shapeTarget.Length - 1) As INDArrayIndex
				Arrays.Fill(targetIndexes, NDArrayIndex.all())
				Dim arr(CInt(shapeSource(0)) - 1) As Integer
				For i As Integer = 0 To arr.Length - 1
					arr(i) = i
				Next i
				targetIndexes(0) = New SpecifiedIndex(arr)

				' Works
				'targetIndexes[0] = NDArrayIndex.interval(0, shapeSource[0]);

				target.put(targetIndexes, source)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = org.nd4j.linalg.factory.Nd4j.concat(0, org.nd4j.linalg.factory.Nd4j.ones(shapeSource), org.nd4j.linalg.factory.Nd4j.zeros(diffShape));
				Dim expected As INDArray = Nd4j.concat(0, Nd4j.ones(shapeSource), Nd4j.zeros(diffShape))
				assertEquals(expected, target,"Expected array to be set!")
			Next test
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutSpecifiedIndices2d()
		Public Overridable Sub testPutSpecifiedIndices2d()

			Dim arr As INDArray = Nd4j.create(3,4)
			Dim toPut As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Integer(){2, 2}, "c"c)
			Dim indices() As INDArrayIndex = { NDArrayIndex.indices(0,2), NDArrayIndex.indices(1,3)}

			Dim exp As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 1, 0, 2},
				New Double() {0, 0, 0, 0},
				New Double() {0, 3, 0, 4}
			})

			arr.put(indices, toPut)
			assertEquals(exp, arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutSpecifiedIndices3d()
		Public Overridable Sub testPutSpecifiedIndices3d()

			Dim arr As INDArray = Nd4j.create(2,3,4)
			Dim toPut As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Integer(){1, 2, 2}, "c"c)
			Dim indices() As INDArrayIndex = { NDArrayIndex.point(1), NDArrayIndex.indices(0,2), NDArrayIndex.indices(1,3)}

			Dim exp As INDArray = Nd4j.create(2,3,4)
			exp.putScalar(1, 0, 1, 1)
			exp.putScalar(1, 0, 3, 2)
			exp.putScalar(1, 2, 1, 3)
			exp.putScalar(1, 2, 3, 4)

			arr.put(indices, toPut)
			assertEquals(exp, arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecifiedIndexArraySize1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecifiedIndexArraySize1(ByVal backend As Nd4jBackend)
			Dim shape() As Long = {2, 2, 2, 2}
			Dim [in] As INDArray = Nd4j.create(shape)
			Dim idx1() As INDArrayIndex = {NDArrayIndex.all(), New SpecifiedIndex(0), NDArrayIndex.all(), NDArrayIndex.all()}

			Dim arr As INDArray = [in].get(idx1)
			Dim expShape() As Long = {2, 1, 2, 2}
			assertArrayEquals(expShape, arr.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTransposei()
		Public Overridable Sub testTransposei()
			Dim arr As INDArray = Nd4j.linspace(1,12,12).reshape("c"c,3,4)

			Dim ti As INDArray = arr.transposei()
			assertArrayEquals(New Long(){4, 3}, ti.shape())
			assertArrayEquals(New Long(){4, 3}, arr.shape())

			assertTrue(arr Is ti) 'Should be same object
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterUpdateShortcut(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterUpdateShortcut(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.FLOAT, 5, 2)
			Dim updates As val = Nd4j.createFromArray(New Single()() {
				New Single() {1, 1},
				New Single() {2, 2},
				New Single() {3, 3}
			})
			Dim indices As val = Nd4j.createFromArray(New Integer(){1, 2, 3})
			Dim exp As val = Nd4j.createFromArray(New Single()() {
				New Single() {0, 0},
				New Single() {1, 1},
				New Single() {2, 2},
				New Single() {3, 3},
				New Single() {0, 0}
			})

			assertArrayEquals(exp.shape(), array.shape())
			Nd4j.scatterUpdate(ScatterUpdate.UpdateOp.ADD, array, indices, updates, 1)

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterUpdateShortcut_f1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterUpdateShortcut_f1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim array As val = Nd4j.create(DataType.FLOAT, 5, 2)
			Dim updates As val = Nd4j.createFromArray(New Single()() {
				New Single() {1, 1},
				New Single() {2, 2},
				New Single() {3, 3}
			})
			Dim indices As val = Nd4j.createFromArray(New Integer(){1, 2, 3})
			Dim exp As val = Nd4j.createFromArray(New Single()() {
				New Single() {0, 0},
				New Single() {1, 1},
				New Single() {2, 2},
				New Single() {3, 3},
				New Single() {0, 0}
			})
			assertArrayEquals(exp.shape(), array.shape())
			Nd4j.scatterUpdate(ScatterUpdate.UpdateOp.ADD, array, indices, updates, 0)
			assertEquals(exp, array)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStatistics_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStatistics_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.createFromArray(New Single() {-1.0f, 0.0f, 1.0f})
			Dim stats As val = Nd4j.Executioner.inspectArray(array)

			assertEquals(1, stats.getCountPositive())
			assertEquals(1, stats.getCountNegative())
			assertEquals(1, stats.getCountZero())
			assertEquals(0.0f, stats.getMeanValue(), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testINDArrayMmulWithTranspose()
		Public Overridable Sub testINDArrayMmulWithTranspose()
			Nd4j.Random.setSeed(12345)
			Dim a As INDArray = Nd4j.rand(2,5).castTo(DataType.DOUBLE)
			Dim b As INDArray = Nd4j.rand(5,3).castTo(DataType.DOUBLE)
			Dim exp As INDArray = a.mmul(b)
			Nd4j.Executioner.commit()

			exp = exp.transpose()

			Dim act As INDArray = a.mmul(b, MMulTranspose.builder().transposeResult(True).build())

			assertEquals(exp, act)

			a = Nd4j.rand(5,2).castTo(DataType.DOUBLE)
			b = Nd4j.rand(5,3).castTo(DataType.DOUBLE)
			exp = a.transpose().mmul(b)
			act = a.mmul(b, MMulTranspose.builder().transposeA(True).build())
			assertEquals(exp, act)

			a = Nd4j.rand(2,5).castTo(DataType.DOUBLE)
			b = Nd4j.rand(3,5).castTo(DataType.DOUBLE)
			exp = a.mmul(b.transpose())
			act = a.mmul(b, MMulTranspose.builder().transposeB(True).build())
			assertEquals(exp, act)

			a = Nd4j.rand(5,2).castTo(DataType.DOUBLE)
			b = Nd4j.rand(3,5).castTo(DataType.DOUBLE)
			exp = a.transpose().mmul(b.transpose())
			act = a.mmul(b, MMulTranspose.builder().transposeA(True).transposeB(True).build())
			assertEquals(exp, act)

			a = Nd4j.rand(5,2).castTo(DataType.DOUBLE)
			b = Nd4j.rand(3,5).castTo(DataType.DOUBLE)
			exp = a.transpose().mmul(b.transpose()).transpose()
			act = a.mmul(b, MMulTranspose.builder().transposeA(True).transposeB(True).transposeResult(True).build())
			assertEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidOrder()
		Public Overridable Sub testInvalidOrder()

			Try
				Nd4j.create(New Integer(){1}, "z"c)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try

			Try
				Nd4j.zeros(1, "z"c)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try

			Try
				Nd4j.zeros(New Integer(){1}, "z"c)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try

			Try
				Nd4j.create(New Long(){1}, "z"c)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try

			Try
				Nd4j.rand("z"c, 1, 1).castTo(DataType.DOUBLE)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try

			Try
				Nd4j.createUninitialized(New Integer(){1}, "z"c)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try

			Try
				Nd4j.createUninitialized(New Long(){1}, "z"c)
				fail("Expected failure")
			Catch e As System.ArgumentException
				assertTrue(e.Message.ToLower().Contains("order"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssignValid()
		Public Overridable Sub testAssignValid()
			Dim arr1 As INDArray = Nd4j.linspace(1, 12, 12).reshape("c"c, 3, 4)
			Dim arr2 As INDArray = Nd4j.create(3,4)
			arr2.assign(arr1)
			assertEquals(arr1, arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssignInvalid()
		Public Overridable Sub testAssignInvalid()
			Dim arr1 As INDArray = Nd4j.linspace(1, 12, 12).reshape("c"c, 3, 4)
			Dim arr2 As INDArray = Nd4j.create(4,3)
			Try
				arr2.assign(arr1)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("shape"),e.Message)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyCasting()
		Public Overridable Sub testEmptyCasting()
			For Each from As val In DataType.values()
				If from = DataType.UTF8 OrElse from = DataType.UNKNOWN OrElse from = DataType.COMPRESSED Then
					Continue For
				End If

				For Each [to] As val In DataType.values()
					If [to] = DataType.UTF8 OrElse [to] = DataType.UNKNOWN OrElse [to] = DataType.COMPRESSED Then
						Continue For
					End If

					Dim emptyFrom As INDArray = Nd4j.empty(from)
					Dim emptyTo As INDArray = emptyFrom.castTo([to])

					Dim str As String = from & " -> " & [to]

					assertEquals(from, emptyFrom.dataType(),str)
					assertTrue(emptyFrom.Empty,str)
					assertEquals(0, emptyFrom.length(),str)

					assertEquals([to], emptyTo.dataType(),str)
					assertTrue(emptyTo.Empty,str)
					assertEquals(0, emptyTo.length(),str)
				Next [to]
			Next from
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVStackRank1()
		Public Overridable Sub testVStackRank1()
			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			list.Add(Nd4j.linspace(1,3,3, DataType.DOUBLE))
			list.Add(Nd4j.linspace(4,6,3, DataType.DOUBLE))
			list.Add(Nd4j.linspace(7,9,3, DataType.DOUBLE))

			Dim [out] As INDArray = Nd4j.vstack(list)
			Dim exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 3},
				New Double() {4, 5, 6},
				New Double() {7, 8, 9}
			})
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxpyOpRows()
		Public Overridable Sub testAxpyOpRows()
			Dim arr As INDArray = Nd4j.create(1,4).assign(2.0f)
			Dim ones As INDArray = Nd4j.ones(1,4).assign(3.0f)

			Nd4j.exec(New Axpy(arr, ones, arr, 10.0, 4))

			Dim exp As INDArray = Nd4j.valueArrayOf(New Long(){1, 4}, 23.0)

			assertEquals(exp, arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyArray(ByVal backend As Nd4jBackend)
			Dim empty As INDArray = Nd4j.empty(DataType.INT)
			assertEquals(empty.ToString(), "[]")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspaceWithStep()
		Public Overridable Sub testLinspaceWithStep()

			Dim lower As Double = -0.9, upper As Double = 0.9, [step] As Double = 0.2
			Dim [in] As INDArray = Nd4j.linspace(lower, upper, 10, DataType.DOUBLE)
			For i As Integer = 0 To 9
				assertEquals(lower + [step] * i, [in].getDouble(i), 1e-5)
			Next i

			[step] = 0.3
			Dim stepped As INDArray = Nd4j.linspace(DataType.DOUBLE, lower, [step], 10)
			For i As Integer = 0 To 9
				assertEquals(lower + i * [step], stepped.getDouble(i),1e-5)
			Next i

			lower = 0.9
			upper = -0.9
			[step] = -0.2
			[in] = Nd4j.linspace(lower, upper, 10, DataType.DOUBLE)
			For i As Integer = 0 To 9
				assertEquals(lower + i * [step], [in].getDouble(i), 1e-5)
			Next i

			stepped = Nd4j.linspace(DataType.DOUBLE, lower, [step], 10)
			For i As Integer = 0 To 9
				assertEquals(lower + i * [step], stepped.getDouble(i), 1e-5)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspaceWithStepForIntegers()
		Public Overridable Sub testLinspaceWithStepForIntegers()

			Dim lower As Long = -9, upper As Long = 9, [step] As Long = 2
			Dim [in] As INDArray = Nd4j.linspace(lower, upper, 10, DataType.LONG)
			For i As Integer = 0 To 9
				assertEquals(lower + [step] * i, [in].getInt(i))
			Next i

			Dim stepped As INDArray = Nd4j.linspace(DataType.INT, lower, 10, [step])
			For i As Integer = 0 To 9
				assertEquals(lower + i * [step], stepped.getInt(i))
			Next i

			lower = 9
			upper = -9
			[step] = -2
			[in] = Nd4j.linspace(lower, upper, 10, DataType.INT)
			For i As Integer = 0 To 9
				assertEquals(lower + i * [step], [in].getInt(i))
			Next i
			lower = 9
			[step] = -2
			Dim stepped2 As INDArray = Nd4j.linspace(DataType.INT, lower, 10, [step])
			For i As Integer = 0 To 9
				assertEquals(lower + i * [step], stepped2.getInt(i))
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArangeWithStep(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArangeWithStep(ByVal backend As Nd4jBackend)
			Dim begin As Integer = -9, [end] As Integer = 9, [step] As Integer = 2
			Dim [in] As INDArray = Nd4j.arange(begin, [end], [step])
			assertEquals([in].getInt(0), -9)
			assertEquals([in].getInt(1), -7)
			assertEquals([in].getInt(2), -5)
			assertEquals([in].getInt(3), -3)
			assertEquals([in].getInt(4), -1)
			assertEquals([in].getInt(5), 1)
			assertEquals([in].getInt(6), 3)
			assertEquals([in].getInt(7), 5)
			assertEquals([in].getInt(8), 7)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("Crashes") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testRollingMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRollingMean(ByVal backend As Nd4jBackend)
			Dim wsconf As val = WorkspaceConfiguration.builder().initialSize(4L * (32*128*256*256 + 32*128 + 10*1024*1024)).policyLearning(LearningPolicy.FIRST_LOOP).policySpill(SpillPolicy.FAIL).build()

			Dim wsName As String = "testRollingMeanWs"
			Try
				System.GC.Collect()
				Dim iterations1 As Integer = If(IntegrationTests, 5, 2)
				For e As Integer = 0 To 4
					Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsconf, wsName)
						Dim array As val = Nd4j.create(DataType.FLOAT, 32, 128, 256, 256)
						array.mean(2, 3)
					End Using
				Next e

				Dim iterations As Integer = If(IntegrationTests, 20, 3)
				Dim timeStart As val = System.nanoTime()
				For e As Integer = 0 To iterations - 1
					Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsconf, wsName)
						Dim array As val = Nd4j.create(DataType.FLOAT, 32, 128, 256, 256)
						array.mean(2, 3)
					End Using
				Next e
				Dim timeEnd As val = System.nanoTime()
				log.info("Average time: {} ms", (timeEnd - timeStart) / CDbl(iterations) / CDbl(1000) / CDbl(1000))
			Finally
				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZerosRank1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZerosRank1(ByVal backend As Nd4jBackend)
			Nd4j.zeros(New Integer() { 2 }, DataType.DOUBLE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeEnforce()
		Public Overridable Sub testReshapeEnforce()

			Dim arr As INDArray = Nd4j.create(New Long(){2, 2}, "c"c)
			Dim arr2 As INDArray = arr.reshape("c"c, True, 4, 1)

			Dim arr1a As INDArray = Nd4j.create(New Long(){2, 3}, "c"c).get(NDArrayIndex.all(), NDArrayIndex.interval(0,2))
			Dim arr3 As INDArray = arr1a.reshape("c"c, False, 4,1)
			Dim isView As Boolean = arr3.View
			assertFalse(isView) 'Should be copy

			Try
				Dim arr4 As INDArray = arr1a.reshape("c"c, True, 4,1)
				fail("Expected exception")
			Catch e As ND4JIllegalStateException
				assertTrue(e.Message.contains("Unable to reshape array as view"),e.Message)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRepeatSimple()
		Public Overridable Sub testRepeatSimple()

			Dim arr As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 3},
				New Double() {4, 5, 6}
			})

			Dim r0 As INDArray = arr.repeat(0, 2)

			Dim exp0 As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1, 2, 3},
				New Double() {1, 2, 3},
				New Double() {4, 5, 6},
				New Double() {4, 5, 6}
			})

			assertEquals(exp0, r0)


			Dim r1 As INDArray = arr.repeat(1, 2)
			Dim exp1 As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1, 1, 2, 2, 3, 3},
				New Double() {4, 4, 5, 5, 6, 6}
			})
			assertEquals(exp1, r1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowsEdgeCaseView()
		Public Overridable Sub testRowsEdgeCaseView()

			Dim arr As INDArray = Nd4j.linspace(0, 9, 10, DataType.DOUBLE).reshape("f"c, 5, 2).dup("c"c) '0,1,2... along columns
			Dim view As INDArray = arr.getColumn(0)
			assertEquals(Nd4j.createFromArray(0.0, 1.0, 2.0, 3.0, 4.0), view)
			Dim idxs() As Integer = {0, 2, 3, 4}

			Dim [out] As INDArray = Nd4j.pullRows(view.reshape(ChrW(5), 1), 1, idxs)
			Dim exp As INDArray = Nd4j.createFromArray(New Double(){0, 2, 3, 4}).reshape(ChrW(4), 1)

			assertEquals(exp, [out]) 'Failing here
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPullRowsFailure(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPullRowsFailure(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim idxs As val = New Integer(){0, 2, 3, 4}
			Dim [out] As val = Nd4j.pullRows(Nd4j.createFromArray(0.0, 1.0, 2.0, 3.0, 4.0), 0, idxs)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRepeatStrided(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRepeatStrided(ByVal backend As Nd4jBackend)

			' Create a 2D array (shape 5x5)
			Dim array As INDArray = Nd4j.arange(25).reshape(ChrW(5), 5)

			' Get first column (shape 5x1)
			Dim slice As INDArray = array.get(NDArrayIndex.all(), NDArrayIndex.point(0)).reshape(5,1)

			' Repeat column on sliced array (shape 5x3)
			Dim repeatedSlice As INDArray = slice.repeat(1, CLng(3))

			' Same thing but copy array first
			Dim repeatedDup As INDArray = slice.dup().repeat(1, CLng(3))

			' Check result
			assertEquals(repeatedSlice, repeatedDup)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeshgridDtypes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeshgridDtypes(ByVal backend As Nd4jBackend)
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Nd4j.meshgrid(Nd4j.create(New Double() { 1, 2, 3 }), Nd4j.create(New Double() { 4, 5, 6 }))

			Nd4j.meshgrid(Nd4j.createFromArray(1, 2, 3), Nd4j.createFromArray(4, 5, 6))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetColumnRowVector()
		Public Overridable Sub testGetColumnRowVector()
			Dim arr As INDArray = Nd4j.create(1,4)
			Dim col As INDArray = arr.getColumn(0)
	'        System.out.println(Arrays.toString(col.shape()));
			assertArrayEquals(New Long(){1}, col.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyArrayReuse()
		Public Overridable Sub testEmptyArrayReuse()
			'Empty arrays are immutable - no point creating them multiple times
			Dim ef1 As INDArray = Nd4j.empty(DataType.FLOAT)
			Dim ef2 As INDArray = Nd4j.empty(DataType.FLOAT)
			assertTrue(ef1 Is ef2) 'Should be exact same object

			Dim el1 As INDArray = Nd4j.empty(DataType.LONG)
			Dim el2 As INDArray = Nd4j.empty(DataType.LONG)
			assertTrue(el1 Is el2) 'Should be exact same object
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxViewF()
		Public Overridable Sub testMaxViewF()
			Dim arr As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){8, 2}, "f"c).assign(999)

			Dim view As INDArray = arr.get(NDArrayIndex.interval(3,5), NDArrayIndex.all())
			view.assign(Nd4j.createFromArray(New Double()(){
				New Double() {1, 2},
				New Double() {3, 4}
			}))

			assertEquals(Nd4j.create(New Double(){3, 4}), view.max(0))
			assertEquals(Nd4j.create(New Double(){2, 4}), view.max(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMin2()
		Public Overridable Sub testMin2()
			Dim x As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {-999, 0.2236, 0.7973, 0.0962},
				New Double() { 0.7231, 0.3381, -0.7301, 0.9115},
				New Double() {-0.5094, 0.9749, -2.1340, 0.6023}
			})

			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 4)
			Nd4j.exec(DynamicCustomOp.builder("reduce_min").addInputs(x).addOutputs([out]).addIntegerArguments(0).build())

			Dim exp As INDArray = Nd4j.createFromArray(-999, 0.2236, -2.1340, 0.0962)
			assertEquals(exp, [out]) 'Fails here


			Dim out1 As INDArray = Nd4j.create(DataType.DOUBLE, 3)
			Nd4j.exec(DynamicCustomOp.builder("reduce_min").addInputs(x).addOutputs(out1).addIntegerArguments(1).build())

			Dim exp1 As INDArray = Nd4j.createFromArray(-999, -0.7301, -2.1340)
			assertEquals(exp1, out1) 'This is OK
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRowValidation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRowValidation(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim matrix As val = Nd4j.create(5, 10)
			Dim row As val = Nd4j.create(25)
			matrix.putRow(1, row)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutColumnValidation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutColumnValidation(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim matrix As val = Nd4j.create(5, 10)
			Dim column As val = Nd4j.create(25)
			matrix.putColumn(1, column)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateF()
		Public Overridable Sub testCreateF()
			Dim origOrder As Char = Nd4j.order()
			Try
				Nd4j.factory().Order = "f"c


				Dim arr As INDArray = Nd4j.createFromArray(New Double()(){
					New Double() {1, 2, 3},
					New Double() {4, 5, 6}
				})
				Dim arr2 As INDArray = Nd4j.createFromArray(New Single()(){
					New Single() {1, 2, 3},
					New Single() {4, 5, 6}
				})
				Dim arr3 As INDArray = Nd4j.createFromArray(New Integer()(){
					New Integer() {1, 2, 3},
					New Integer() {4, 5, 6}
				})
				Dim arr4 As INDArray = Nd4j.createFromArray(New Long()(){
					New Long() {1, 2, 3},
					New Long() {4, 5, 6}
				})
				Dim arr5 As INDArray = Nd4j.createFromArray(New Short()(){
					New Short() {1, 2, 3},
					New Short() {4, 5, 6}
				})
				Dim arr6 As INDArray = Nd4j.createFromArray(New SByte()(){
					New SByte() {1, 2, 3},
					New SByte() {4, 5, 6}
				})

				Dim exp As INDArray = Nd4j.create(2, 3)
				exp.putScalar(0, 0, 1.0)
				exp.putScalar(0, 1, 2.0)
				exp.putScalar(0, 2, 3.0)
				exp.putScalar(1, 0, 4.0)
				exp.putScalar(1, 1, 5.0)
				exp.putScalar(1, 2, 6.0)

				assertEquals(exp, arr)
				assertEquals(exp.castTo(DataType.FLOAT), arr2)
				assertEquals(exp.castTo(DataType.INT), arr3)
				assertEquals(exp.castTo(DataType.LONG), arr4)
				assertEquals(exp.castTo(DataType.SHORT), arr5)
				assertEquals(exp.castTo(DataType.BYTE), arr6)
			Finally
				Nd4j.factory().Order = origOrder
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceKeepDimsShape()
		Public Overridable Sub testReduceKeepDimsShape()
			Dim arr As INDArray = Nd4j.create(3,4)
			Dim [out] As INDArray = arr.sum(True, 1)
			assertArrayEquals(New Long(){3, 1}, [out].shape())

			Dim out2 As INDArray = arr.sum(True, 0)
			assertArrayEquals(New Long(){1, 4}, out2.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceRow()
		Public Overridable Sub testSliceRow()
			Dim data() As Double = {15.0, 16.0}
			Dim vector As INDArray = Nd4j.createFromArray(data).reshape(ChrW(1), 2)
			Dim slice As INDArray = vector.slice(0)
	'        System.out.println(slice.shapeInfoToString());
			assertEquals(vector.reshape(ChrW(2)), slice)
			slice.assign(-1)
			assertEquals(Nd4j.createFromArray(-1.0, -1.0).reshape(1,2), vector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceMatrix()
		Public Overridable Sub testSliceMatrix()
			Dim arr As INDArray = Nd4j.arange(4).reshape(ChrW(2), 2)
	'        System.out.println(arr.slice(0));
	'        System.out.println();
	'        System.out.println(arr.slice(1));
			arr.slice(0)
			arr.slice(1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarEq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarEq(ByVal backend As Nd4jBackend)
			Dim scalarRank2 As INDArray = Nd4j.scalar(10.0).reshape(ChrW(1), 1)
			Dim scalarRank1 As INDArray = Nd4j.scalar(10.0).reshape(ChrW(1))
			Dim scalarRank0 As INDArray = Nd4j.scalar(10.0)

			assertNotEquals(scalarRank0, scalarRank2)
			assertNotEquals(scalarRank0, scalarRank1)
			assertNotEquals(scalarRank1, scalarRank2)
			assertEquals(scalarRank0, scalarRank0.dup())
			assertEquals(scalarRank1, scalarRank1.dup())
			assertEquals(scalarRank2, scalarRank2.dup())
		End Sub

		'@Disabled // https://github.com/eclipse/deeplearning4j/issues/7632
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetWhereINDArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetWhereINDArray(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.create(New Double() { 1, -3, 4, 8, -2, 5 })
			Dim comp As INDArray = Nd4j.create(New Double(){2, -3, 1, 1, -2, 1 })
			Dim expected As INDArray = Nd4j.create(New Double() { 4, 8, 5 })
			Dim actual As INDArray = input.getWhere(comp, Conditions.greaterThan(1))

			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetWhereNumber(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetWhereNumber(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.create(New Double() { 1, -3, 4, 8, -2, 5 })
			Dim expected As INDArray = Nd4j.create(New Double() { 8, 5 })
			Dim actual As INDArray = input.getWhere(4, Conditions.greaterThan(1))

			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testType1(org.nd4j.linalg.factory.Nd4jBackend backend) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testType1(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 9
				Dim in1 As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){100, 100}).castTo(DataType.DOUBLE)
				Dim dir As File = testDir.toFile()
				Dim oos As New ObjectOutputStream(New FileStream(dir,"test.bin", FileMode.Create, FileAccess.Write))
				oos.writeObject(in1)

				Dim ois As New ObjectInputStream(New FileStream(dir,"test.bin", FileMode.Open, FileAccess.Read))
				Dim in2 As INDArray = Nothing
				Try
					in2 = DirectCast(ois.readObject(), INDArray)
				Catch e As ClassNotFoundException

				End Try

				assertEquals(in1, in2)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOnes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOnes(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones()
			Dim arr2 As INDArray = Nd4j.ones(DataType.LONG)
			assertEquals(0, arr.rank())
			assertEquals(1, arr.length())
			assertEquals(0, arr2.rank())
			assertEquals(1, arr2.length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZeros(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZeros(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.zeros()
			Dim arr2 As INDArray = Nd4j.zeros(DataType.LONG)
			assertEquals(0, arr.rank())
			assertEquals(1, arr.length())
			assertEquals(0, arr2.rank())
			assertEquals(1, arr2.length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testType2(org.nd4j.linalg.factory.Nd4jBackend backend) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testType2(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 9
				Dim in1 As INDArray = Nd4j.ones(DataType.UINT16)
				Dim dir As File = testDir.toFile()
				Dim oos As New ObjectOutputStream(New FileStream(dir, "test1.bin", FileMode.Create, FileAccess.Write))
				oos.writeObject(in1)

				Dim ois As New ObjectInputStream(New FileStream(dir, "test1.bin", FileMode.Open, FileAccess.Read))
				Dim in2 As INDArray = Nothing
				Try
					in2 = DirectCast(ois.readObject(), INDArray)
				Catch e As ClassNotFoundException

				End Try

				assertEquals(in1, in2)
			Next i

			For i As Integer = 0 To 9
				Dim in1 As INDArray = Nd4j.ones(DataType.UINT32)
				Dim dir As File = testDir.toFile()
				Dim oos As New ObjectOutputStream(New FileStream(dir, "test2.bin", FileMode.Create, FileAccess.Write))
				oos.writeObject(in1)

				Dim ois As New ObjectInputStream(New FileStream(dir, "test2.bin", FileMode.Open, FileAccess.Read))
				Dim in2 As INDArray = Nothing
				Try
					in2 = DirectCast(ois.readObject(), INDArray)
				Catch e As ClassNotFoundException

				End Try

				assertEquals(in1, in2)
			Next i

			For i As Integer = 0 To 9
				Dim in1 As INDArray = Nd4j.ones(DataType.UINT64)
				Dim dir As File = testDir.toFile()
				Dim oos As New ObjectOutputStream(New FileStream(dir, "test3.bin", FileMode.Create, FileAccess.Write))
				oos.writeObject(in1)

				Dim ois As New ObjectInputStream(New FileStream(dir, "test3.bin", FileMode.Open, FileAccess.Read))
				Dim in2 As INDArray = Nothing
				Try
					in2 = DirectCast(ois.readObject(), INDArray)
				Catch e As ClassNotFoundException

				End Try

				assertEquals(in1, in2)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToXMatrix()
		Public Overridable Sub testToXMatrix()

			Dim shapes As IList(Of Long()) = New List(Of Long()) From {
				New Long(){3, 4},
				New Long(){3, 1},
				New Long(){1, 3}
			}
			For Each shape As Long() In shapes
				Dim length As Long = ArrayUtil.prodLong(shape)
				Dim orig As INDArray = Nd4j.arange(length).castTo(DataType.DOUBLE).reshape(shape)
				For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT, DataType.LONG, DataType.SHORT, DataType.UBYTE, DataType.UINT16, DataType.UINT32, DataType.UINT64, DataType.BFLOAT16}
					Dim arr As INDArray = orig.castTo(dt)

					Dim fArr()() As Single = arr.toFloatMatrix()
					Dim dArr()() As Double = arr.toDoubleMatrix()
					Dim iArr()() As Integer = arr.toIntMatrix()
					Dim lArr()() As Long = arr.toLongMatrix()

					Dim f As INDArray = Nd4j.createFromArray(fArr).castTo(dt)
					Dim d As INDArray = Nd4j.createFromArray(dArr).castTo(dt)
					Dim i As INDArray = Nd4j.createFromArray(iArr).castTo(dt)
					Dim l As INDArray = Nd4j.createFromArray(lArr).castTo(dt)

					assertEquals(arr, f)
					assertEquals(arr, d)
					assertEquals(arr, i)
					assertEquals(arr, l)
				Next dt
			Next shape
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToXVector()
		Public Overridable Sub testToXVector()

			Dim shapes As IList(Of Long()) = New List(Of Long()) From {
				New Long(){3},
				New Long(){3, 1},
				New Long(){1, 3}
			}
			For Each shape As Long() In shapes
				Dim orig As INDArray = Nd4j.arange(3).castTo(DataType.DOUBLE).reshape(shape)
				For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT, DataType.LONG, DataType.SHORT, DataType.UBYTE, DataType.UINT16, DataType.UINT32, DataType.UINT64, DataType.BFLOAT16}
					Dim arr As INDArray = orig.castTo(dt)

					Dim fArr() As Single = arr.toFloatVector()
					Dim dArr() As Double = arr.toDoubleVector()
					Dim iArr() As Integer = arr.toIntVector()
					Dim lArr() As Long = arr.toLongVector()

					Dim f As INDArray = Nd4j.createFromArray(fArr).castTo(dt).reshape(shape)
					Dim d As INDArray = Nd4j.createFromArray(dArr).castTo(dt).reshape(shape)
					Dim i As INDArray = Nd4j.createFromArray(iArr).castTo(dt).reshape(shape)
					Dim l As INDArray = Nd4j.createFromArray(lArr).castTo(dt).reshape(shape)

					assertEquals(arr, f)
					assertEquals(arr, d)
					assertEquals(arr, i)
					assertEquals(arr, l)
				Next dt
			Next shape
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumEdgeCase()
		Public Overridable Sub testSumEdgeCase()
			Dim row As INDArray = Nd4j.create(1,3)
			Dim sum As INDArray = row.sum(0)
			assertArrayEquals(New Long(){3}, sum.shape())

			Dim twoD As INDArray = Nd4j.create(2,3)
			Dim sum2 As INDArray = twoD.sum(0)
			assertArrayEquals(New Long(){3}, sum2.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMedianEdgeCase()
		Public Overridable Sub testMedianEdgeCase()
			Dim rowVec As INDArray = Nd4j.rand(DataType.FLOAT, 1, 10)
			Dim median As INDArray = rowVec.median(0)
			assertEquals(rowVec.reshape(ChrW(10)), median)

			Dim colVec As INDArray = Nd4j.rand(DataType.FLOAT, 10, 1)
			median = colVec.median(1)
			assertEquals(colVec.reshape(ChrW(10)), median)

			'Non-edge cases:
			rowVec.median(1)
			colVec.median(0)

			'full array case:
			rowVec.median()
			colVec.median()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void mmulToScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub mmulToScalar(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray arr1 = org.nd4j.linalg.factory.Nd4j.create(new float[] {1,2,3}).reshape(1,3);
			Dim arr1 As INDArray = Nd4j.create(New Single() {1, 2, 3}).reshape(ChrW(1), 3)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray arr2 = arr1.reshape(3,1);
			Dim arr2 As INDArray = arr1.reshape(ChrW(3), 1)
			assertEquals(DataType.FLOAT, arr1.mmul(arr2).dataType(),"Incorrect type!")
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateDtypes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateDtypes(ByVal backend As Nd4jBackend)
			Dim sliceShape() As Integer = {9}
			Dim arrays() As Single = {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f}
			Dim arrays_double() As Double = {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0}

			Dim x As INDArray = Nd4j.create(sliceShape, arrays, arrays)
			assertEquals(DataType.FLOAT, x.dataType())

			Dim xd As INDArray = Nd4j.create(sliceShape, arrays_double, arrays_double)
			assertEquals(DataType.DOUBLE, xd.dataType())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateShapeValidation()
		Public Overridable Sub testCreateShapeValidation()
			Try
				Nd4j.create(New Double(){1, 2, 3}, New Integer(){1, 1})
				fail()
			Catch t As Exception
				assertTrue(t.Message.contains("length"))
			End Try

			Try
				Nd4j.create(New Single(){1, 2, 3}, New Integer(){1, 1})
				fail()
			Catch t As Exception
				assertTrue(t.Message.contains("length"))
			End Try

			Try
				Nd4j.create(New SByte(){1, 2, 3}, New Long(){1, 1}, DataType.BYTE)
				fail()
			Catch t As Exception
				assertTrue(t.Message.contains("length"))
			End Try

			Try
				Nd4j.create(New Double(){1, 2, 3}, New Integer(){1, 1}, "c"c)
				fail()
			Catch t As Exception
				assertTrue(t.Message.contains("length"))
			End Try
		End Sub


		'/////////////////////////////////////////////////////
		Protected Friend Shared Sub fillJvmArray3D(ByVal arr()()() As Single)
			Dim cnt As Integer = 1
			For i As Integer = 0 To arr.Length - 1
				Dim j As Integer = 0
				Do While j < arr(0).Length
					Dim k As Integer = 0
					Do While k < arr(0)(0).Length
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: arr[i][j][k] = (float) cnt++;
						arr(i)(j)(k) = CSng(cnt)
							cnt += 1
						k += 1
					Loop
					j += 1
				Loop
			Next i
		End Sub


		Protected Friend Shared Sub fillJvmArray4D(ByVal arr()()()() As Single)
			Dim cnt As Integer = 1
			For i As Integer = 0 To arr.Length - 1
				Dim j As Integer = 0
				Do While j < arr(0).Length
					Dim k As Integer = 0
					Do While k < arr(0)(0).Length
						Dim m As Integer = 0
						Do While m < arr(0)(0)(0).Length
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: arr[i][j][k][m] = (float) cnt++;
							arr(i)(j)(k)(m) = CSng(cnt)
								cnt += 1
							m += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBatchToSpace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchToSpace(ByVal backend As Nd4jBackend)

			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 2, 4, 5)
			Dim c As DynamicCustomOp = New BatchToSpaceND()

			c.addInputArgument(Nd4j.rand(DataType.FLOAT, New Integer(){4, 4, 3}), Nd4j.createFromArray(1, 2), Nd4j.createFromArray(New Integer()(){
				New Integer(){0, 0},
				New Integer(){0, 1}
			}))
			c.addOutputArgument([out])
			Nd4j.Executioner.exec(c)

			Dim l As IList(Of LongShapeDescriptor) = c.calculateOutputShape()

	'        System.out.println(Arrays.toString(l.get(0).getShape()));

			'from [4,4,3] to [2,4,6] then crop to [2,4,5]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToFromByteArray() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testToFromByteArray()
			' simple test to get rid of toByteArray and fromByteArray compiler warnings.
			Dim x As INDArray = Nd4j.arange(10)
			Dim xb() As SByte = Nd4j.toByteArray(x)
			Dim y As INDArray = Nd4j.fromByteArray(xb)
			assertEquals(x,y)
		End Sub

		Private Shared Function fwd(ByVal input As INDArray, ByVal W As INDArray, ByVal b As INDArray) As INDArray
			Dim ret As INDArray = Nd4j.createUninitialized(input.size(0), W.size(1)).castTo(DataType.DOUBLE)
			input.mmuli(W, ret)
			ret.addiRowVector(b)
			Return ret
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVStackHStack1d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVStackHStack1d(ByVal backend As Nd4jBackend)
			Dim rowVector1 As INDArray = Nd4j.create(New Double(){1, 2, 3})
			Dim rowVector2 As INDArray = Nd4j.create(New Double(){4, 5, 6})

			Dim vStack As INDArray = Nd4j.vstack(rowVector1, rowVector2) 'Vertical stack:   [3]+[3] to [2,3]
			Dim hStack As INDArray = Nd4j.hstack(rowVector1, rowVector2) 'Horizontal stack: [3]+[3] to [6]

			assertEquals(Nd4j.createFromArray(1.0,2,3,4,5,6).reshape("c"c, 2, 3), vStack)
			assertEquals(Nd4j.createFromArray(1.0,2,3,4,5,6), hStack)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceAll_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduceAll_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.empty(DataType.FLOAT)
			Dim e As val = Nd4j.scalar(True)
			Dim z As val = Nd4j.exec(New All(x))

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceAll_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduceAll_2(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.ones(DataType.FLOAT, 0)
			Dim e As val = Nd4j.scalar(True)
			Dim z As val = Nd4j.exec(New All(x))

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduceAll_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduceAll_3(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 0)
			assertEquals(1, x.rank())

			Dim e As val = Nd4j.scalar(True)
			Dim z As val = Nd4j.exec(New All(x, 0))

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarEqualsNoResult()
		Public Overridable Sub testScalarEqualsNoResult()
			Dim [out] As INDArray = Nd4j.exec(New ScalarEquals(Nd4j.createFromArray(-2, -1, 0, 1, 2), Nothing, 0))
			Dim exp As INDArray = Nd4j.createFromArray(False, False, True, False, False)
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutOverwrite()
		Public Overridable Sub testPutOverwrite()
			Dim arr As INDArray = Nd4j.create(DataType.DOUBLE, 10)
			arr.putScalar(0, 10)
			Console.WriteLine(arr)
			Dim arr2 As INDArray = Nd4j.createFromArray(3.0, 3.0, 3.0)
			Dim view As val = arr.get(New INDArrayIndex(){NDArrayIndex.interval(1, 4)})
			view.assign(arr2)
			Console.WriteLine(arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyReshapingMinus1()
		Public Overridable Sub testEmptyReshapingMinus1()
			Dim arr0 As INDArray = Nd4j.create(DataType.FLOAT, 2, 0)
			Dim arr1 As INDArray = Nd4j.create(DataType.FLOAT, 0, 1, 2)

			Dim out0 As INDArray = Nd4j.exec(New Reshape(arr0, Nd4j.createFromArray(2, 0, -1)))(0)
			Dim out1 As INDArray = Nd4j.exec(New Reshape(arr1, Nd4j.createFromArray(-1, 1)))(0)
			Dim out2 As INDArray = Nd4j.exec(New Reshape(arr1, Nd4j.createFromArray(10, -1)))(0)

			assertArrayEquals(New Long(){2, 0, 1}, out0.shape())
			assertArrayEquals(New Long(){0, 1}, out1.shape())
			assertArrayEquals(New Long(){10, 0}, out2.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2DWeightsFormat1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2DWeightsFormat1(ByVal backend As Nd4jBackend)
			Dim bS As Integer = 2, iH As Integer = 4, iW As Integer = 3, iC As Integer = 4, oC As Integer = 3, kH As Integer = 3, kW As Integer = 2, sH As Integer = 1, sW As Integer = 1, pH As Integer = 0, pW As Integer = 0, dH As Integer = 1, dW As Integer = 1
			Dim oH As Integer=2, oW As Integer=2
			' Weights format tip :
			' 0 - kH, kW, iC, oC
			' 1 - oC, iC, kH, kW
			' 2 - oC, kH, kW, iC
			Dim format As WeightsFormat = WeightsFormat.OIYX

			Dim inArr As INDArray = Nd4j.linspace(DataType.FLOAT, 25, -0.5, 96).reshape(New Long(){bS, iC, iH, iW})
			Dim weights As INDArray = Nd4j.createFromArray(New Single(){ -3.0f, -1.8f, -0.6f, 0.6f, 1.8f, 3.0f, -2.7f, -1.5f, -0.3f, 0.9f, 2.1f, 3.3f, -2.4f, -1.2f, 0.0f, 1.2f, 2.4f, 3.6f, -2.1f, -0.9f, 0.3f, 1.5f, 2.7f, 3.9f, -2.9f, -1.7f, -0.5f, 0.7f, 1.9f, 3.1f, -2.6f, -1.4f, -0.2f, 1.0f, 2.2f, 3.4f, -2.3f, -1.1f, 0.1f, 1.3f, 2.5f, 3.7f, -2.0f, -0.8f, 0.4f, 1.6f, 2.8f, 4.0f, -2.8f, -1.6f, -0.4f, 0.8f, 2.0f, 3.2f, -2.5f, -1.3f, -0.1f, 1.1f, 2.3f, 3.5f, -2.2f, -1.0f, 0.2f, 1.4f, 2.6f, 3.8f, -1.9f, -0.7f, 0.5f, 1.7f, 2.9f, 4.1f}).reshape(New Long(){oC, iC, kH, kW})

			Dim bias As INDArray = Nd4j.createFromArray(New Single(){-1, 2, 0.5f})

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(pH).pW(pW).sH(sH).sW(sW).dH(dH).dW(dW).isSameMode(False).weightsFormat(format).build()

			Dim ret() As INDArray = Nd4j.exec(New Conv2D(inArr, weights, bias, c))
			assertArrayEquals(New Long(){bS, oC, oH, oW}, ret(0).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2DWeightsFormat2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2DWeightsFormat2(ByVal backend As Nd4jBackend)
			Dim bS As Integer=2, iH As Integer=4, iW As Integer=3, iC As Integer=4, oC As Integer=3, kH As Integer=3, kW As Integer=2, sH As Integer=1, sW As Integer=1, pH As Integer=0, pW As Integer=0, dH As Integer=1, dW As Integer=1
			Dim oH As Integer=4, oW As Integer=3
			Dim format As WeightsFormat = WeightsFormat.OYXI

			Dim inArr As INDArray = Nd4j.linspace(DataType.FLOAT, 25, -0.5, 96).reshape(New Long(){bS, iH, iW, iC})
			Dim weights As INDArray = Nd4j.createFromArray(New Single(){ -3.0f, -1.8f, -0.6f, 0.6f, 1.8f, 3.0f, -2.7f, -1.5f, -0.3f, 0.9f, 2.1f, 3.3f, -2.4f, -1.2f, 0.0f, 1.2f, 2.4f, 3.6f, -2.1f, -0.9f, 0.3f, 1.5f, 2.7f, 3.9f, -2.9f, -1.7f, -0.5f, 0.7f, 1.9f, 3.1f, -2.6f, -1.4f, -0.2f, 1.0f, 2.2f, 3.4f, -2.3f, -1.1f, 0.1f, 1.3f, 2.5f, 3.7f, -2.0f, -0.8f, 0.4f, 1.6f, 2.8f, 4.0f, -2.8f, -1.6f, -0.4f, 0.8f, 2.0f, 3.2f, -2.5f, -1.3f, -0.1f, 1.1f, 2.3f, 3.5f, -2.2f, -1.0f, 0.2f, 1.4f, 2.6f, 3.8f, -1.9f, -0.7f, 0.5f, 1.7f, 2.9f, 4.1f}).reshape(New Long(){oC, kH, kW, iC})

			Dim bias As INDArray = Nd4j.createFromArray(New Single(){-1, 2, 0.5f})

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(pH).pW(pW).sH(sH).sW(sW).dH(dH).dW(dW).isSameMode(True).dataFormat("NHWC").weightsFormat(format).build()

			Dim ret() As INDArray = Nd4j.exec(New Conv2D(inArr, weights, bias, c))
			Console.WriteLine(java.util.Arrays.toString(ret(0).shape()))
			assertArrayEquals(New Long(){bS, oH, oW, oC}, ret(0).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_8(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_8(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.INT8, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.INT8, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.INT8, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_7(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.INT16, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.INT16, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.INT16, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.INT32, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.INT32, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.INT32, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_2(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.INT64, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.INT64, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.INT64, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_6(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT8, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.UINT8, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.UINT8, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_5(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT16, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.UINT16, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.UINT16, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_3(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT32, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.UINT32, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.UINT32, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulMethod_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulMethod_4(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT64, 3, 5).assign(1)
			Dim y As val = Nd4j.create(DataType.UINT64, 5, 3).assign(1)
			Dim e As val = Nd4j.create(DataType.UINT64, 3, 3).assign(5)

			Dim z As val = x.mmul(y)
			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testCreateBufferFromByteBuffer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateBufferFromByteBuffer(ByVal backend As Nd4jBackend)

			For Each dt As DataType In DataType.values()
				If dt = DataType.COMPRESSED OrElse dt = DataType.UTF8 OrElse dt = DataType.UNKNOWN Then
					Continue For
				End If
	'            System.out.println(dt);

				Dim lengthBytes As Integer = 256
				Dim lengthElements As Integer = lengthBytes \ dt.width()
				Dim bb As ByteBuffer = ByteBuffer.allocateDirect(lengthBytes)

				Dim db As DataBuffer = Nd4j.createBuffer(bb, dt, lengthElements, 0)
				Dim arr As INDArray = Nd4j.create(db, New Long(){lengthElements})

				arr.toStringFull()
				arr.ToString()

				For Each dt2 As DataType In DataType.values()
					If dt2 = DataType.COMPRESSED OrElse dt2 = DataType.UTF8 OrElse dt2 = DataType.UNKNOWN Then
						Continue For
					End If
					Dim a2 As INDArray = arr.castTo(dt2)
					a2.toStringFull()
				Next dt2
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateBufferFromByteBufferViews()
		Public Overridable Sub testCreateBufferFromByteBufferViews()

			For Each dt As DataType In DataType.values()
				If dt = DataType.COMPRESSED OrElse dt = DataType.UTF8 OrElse dt = DataType.UNKNOWN Then
					Continue For
				End If
	'            System.out.println(dt);

				Dim lengthBytes As Integer = 256
				Dim lengthElements As Integer = lengthBytes \ dt.width()
				Dim bb As ByteBuffer = ByteBuffer.allocateDirect(lengthBytes)

				Dim db As DataBuffer = Nd4j.createBuffer(bb, dt, lengthElements, 0)
				Dim arr As INDArray = Nd4j.create(db, New Long(){lengthElements\2, 2})

				arr.toStringFull()

				Dim view As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.point(0))
				Dim view2 As INDArray = arr.get(NDArrayIndex.point(1), NDArrayIndex.all())

				view.toStringFull()
				view2.toStringFull()
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTypeCastingToString()
		Public Overridable Sub testTypeCastingToString()

			For Each dt As DataType In DataType.values()
				If dt = DataType.COMPRESSED OrElse dt = DataType.UTF8 OrElse dt = DataType.UNKNOWN Then
					Continue For
				End If
				Dim a1 As INDArray = Nd4j.create(dt, 10)
				For Each dt2 As DataType In DataType.values()
					If dt2 = DataType.COMPRESSED OrElse dt2 = DataType.UTF8 OrElse dt2 = DataType.UNKNOWN Then
						Continue For
					End If

					Dim a2 As INDArray = a1.castTo(dt2)
					a2.toStringFull()
				Next dt2
			Next dt
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShape0Casts()
		Public Overridable Sub testShape0Casts()
			For Each dt As DataType In DataType.values()
				If Not dt.isNumerical() Then
					Continue For
				End If

				Dim a1 As INDArray = Nd4j.create(dt, 1,0,2)

				For Each dt2 As DataType In DataType.values()
					If Not dt2.isNumerical() Then
						Continue For
					End If
					Dim a2 As INDArray = a1.castTo(dt2)

					assertArrayEquals(a1.shape(), a2.shape())
					assertEquals(dt2, a2.dataType())
				Next dt2
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSmallSort()
		Public Overridable Sub testSmallSort()
			Dim arr As INDArray = Nd4j.createFromArray(0.5, 0.4, 0.1, 0.2)
			Dim expected As INDArray = Nd4j.createFromArray(0.1, 0.2, 0.4, 0.5)
			Dim sorted As INDArray = Nd4j.sort(arr, True)
			assertEquals(expected, sorted)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace