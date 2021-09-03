Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.junit.jupiter.api
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataFormat = org.nd4j.enums.DataFormat
Imports OpTestCase = org.nd4j.autodiff.validation.OpTestCase
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports PadMode = org.nd4j.enums.PadMode
Imports ImageResizeMethod = org.nd4j.enums.ImageResizeMethod
Imports PartitionMode = org.nd4j.enums.PartitionMode
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ImageResize = org.nd4j.linalg.api.ops.impl.image.ImageResize
Imports DepthToSpace = org.nd4j.linalg.api.ops.impl.layers.convolution.DepthToSpace
Imports SpaceToDepth = org.nd4j.linalg.api.ops.impl.layers.convolution.SpaceToDepth
Imports Upsampling3d = org.nd4j.linalg.api.ops.impl.layers.convolution.Upsampling3d
Imports ScalarFMod = org.nd4j.linalg.api.ops.impl.scalar.ScalarFMod
Imports ScalarMultiplication = org.nd4j.linalg.api.ops.impl.scalar.ScalarMultiplication
Imports Cross = org.nd4j.linalg.api.ops.impl.shape.Cross
Imports MergeAvg = org.nd4j.linalg.api.ops.impl.shape.MergeAvg
Imports MergeMax = org.nd4j.linalg.api.ops.impl.shape.MergeMax
Imports MergeMaxIndex = org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex
Imports EmbeddingLookup = org.nd4j.linalg.api.ops.impl.shape.tensorops.EmbeddingLookup
Imports ClipByAvgNorm = org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByAvgNorm
Imports CReLU = org.nd4j.linalg.api.ops.impl.transforms.custom.CReLU
Imports GreaterThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThanOrEqual
Imports LessThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThanOrEqual
Imports Max = org.nd4j.linalg.api.ops.impl.transforms.custom.Max
Imports Min = org.nd4j.linalg.api.ops.impl.transforms.custom.Min
Imports Reverse = org.nd4j.linalg.api.ops.impl.transforms.custom.Reverse
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports Standardize = org.nd4j.linalg.api.ops.impl.transforms.custom.Standardize
Imports RSqrt = org.nd4j.linalg.api.ops.impl.transforms.floating.RSqrt
Imports MergeAddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MergeAddOp
Imports ACosh = org.nd4j.linalg.api.ops.impl.transforms.strict.ACosh
Imports ASinh = org.nd4j.linalg.api.ops.impl.transforms.strict.ASinh
Imports Erf = org.nd4j.linalg.api.ops.impl.transforms.strict.Erf
Imports Erfc = org.nd4j.linalg.api.ops.impl.transforms.strict.Erfc
Imports HardSigmoid = org.nd4j.linalg.api.ops.impl.transforms.strict.HardSigmoid
Imports LogSigmoid = org.nd4j.linalg.api.ops.impl.transforms.strict.LogSigmoid
Imports RationalTanh = org.nd4j.linalg.api.ops.impl.transforms.strict.RationalTanh
Imports RectifiedTanh = org.nd4j.linalg.api.ops.impl.transforms.strict.RectifiedTanh
Imports SELU = org.nd4j.linalg.api.ops.impl.transforms.strict.SELU
Imports Swish = org.nd4j.linalg.api.ops.impl.transforms.strict.Swish
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.function
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) public class TransformOpValidation extends BaseOpValidation
	Public Class TransformOpValidation
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarOps(ByVal backend As Nd4jBackend)
			Dim d0 As Integer = 2
			Dim d1 As Integer = 3
			Dim d2 As Integer = 4

			Dim n As Integer = d0 * d1 * d2

			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To 10
				For Each inOrder As Char In New Char(){"c"c, "f"c}
					Dim sd As SameDiff = SameDiff.create()

					Dim inArr As INDArray = Nd4j.linspace(1, n, n, DataType.DOUBLE).reshape("c"c, d0, d1, d2).dup(inOrder)
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim tc As TestCase = (New TestCase(sd)).gradientCheck(True)

					Dim [out] As SDVariable
					Dim msg As String
					Select Case i
						Case 0
							[out] = [in].mul(2)
							tc.expectedOutput([out].name(), inArr.mul(2))
							msg = "mul - " & inOrder
						Case 1
							[out] = [in].div(2)
							tc.expectedOutput([out].name(), inArr.div(2))
							msg = "div - " & inOrder
						Case 2
							[out] = [in].add(2)
							tc.expectedOutput([out].name(), inArr.add(2))
							msg = "add - " & inOrder
						Case 3
							[out] = [in].sub(2)
							tc.expectedOutput([out].name(), inArr.sub(2))
							msg = "sub - " & inOrder
						Case 4
							[out] = [in].rdiv(2)
							tc.expectedOutput([out].name(), inArr.rdiv(2))
							msg = "rdiv - " & inOrder
						Case 5
							[out] = [in].rsub(2)
							tc.expectedOutput([out].name(), inArr.rsub(2))
							msg = "rsub - " & inOrder
						Case 6
							[out] = sd.math().pow([in], 2)
							tc.expectedOutput([out].name(), Transforms.pow(inArr, 2))
							msg = "pow - " & inOrder
						Case 7
							inArr.assign(Nd4j.rand(inArr.dataType(), inArr.shape()).muli(5).subi(2.5))
							[out] = sd.math().floorMod([in], 2.0)
							tc.expected([out], Nd4j.Executioner.exec(New ScalarFMod(inArr.dup(), 2.0)))
							msg = "scalarFloorMod - " & inOrder
						Case 8
							inArr.assign(Nd4j.rand(inArr.shape()))
							[out] = sd.scalarMax([in], 0.5)
							tc.expected([out], Transforms.max(inArr.dup(), 0.5))
							msg = "scalarMax - " & inOrder
						Case 9
							inArr.assign(Nd4j.rand(inArr.shape()))
							[out] = sd.scalarMin([in], 0.5)
							tc.expected([out], Transforms.min(inArr.dup(), 0.5))
							msg = "scalarMin - " & inOrder
						Case 10
							[out] = [in].assign(0.5)
							tc.expected([out], Nd4j.valueArrayOf(inArr.shape(), 0.5))
							msg = "scalarSet - " & inOrder
						Case Else
							Throw New Exception()
					End Select

					tc.testName(msg)

					Dim loss As SDVariable = sd.standardDeviation([out], True)

					log.info("Starting test: " & msg)
					Dim err As String = OpValidation.validate(tc, True)
					If err IsNot Nothing Then
						failed.Add(err)
					End If
				Next inOrder
			Next i
			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarMulCF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarMulCF(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4)
			Dim outC As INDArray = Nd4j.createUninitialized(3, 4)
			Dim outF As INDArray = Nd4j.createUninitialized(3, 4)

			Nd4j.Executioner.exec(New ScalarMultiplication([in], Nothing, outC, 2.0))
			Nd4j.Executioner.exec(New ScalarMultiplication([in], Nothing, outF, 2.0))

			assertEquals(outC, outF)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarMulCF2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarMulCF2(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4)

			Dim outC As INDArray = Nd4j.Executioner.exec(New ScalarMultiplication([in].dup("c"c), 2.0))
			Dim outF As INDArray = Nd4j.Executioner.exec(New ScalarMultiplication([in].dup("f"c), 2.0))

			assertEquals(outC, outF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCross(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCross(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.create(New Double(){4, 2, 1}, New Integer(){1, 3})
			Dim b As INDArray = Nd4j.create(New Double(){1, 3, 4}, New Integer(){1, 3})

			Dim expOut As INDArray = Nd4j.create(DataType.DOUBLE, 1, 3)

			Dim op As val = New Cross(a, b, expOut)
			Nd4j.Executioner.exec(op)

			Dim sd As SameDiff = SameDiff.create()

			Dim sdA As SDVariable = sd.var("a", expOut.shape())
			Dim sdB As SDVariable = sd.var("b", expOut.shape())


			sd.associateArrayWithVariable(a, sdA)
			sd.associateArrayWithVariable(b, sdB)

			Dim t As SDVariable = sd.math().cross("cross", sdA, sdB)
			Dim loss As SDVariable = sd.mean("loss", t)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("cross", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpaceToDepth(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpaceToDepth(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(1337)

			Dim miniBatch As Integer = 128
			Dim blockSize As Integer = 4
			Dim inputShape() As Integer = {miniBatch, 2 * blockSize, 2 * blockSize, 1}

			Dim input As INDArray = Nd4j.randn(inputShape)
			Dim sd As SameDiff = SameDiff.create()
			Dim sdInput As SDVariable = sd.var("in", inputShape)

			Dim expOut As INDArray = Nd4j.create(miniBatch, 2, 2, blockSize * blockSize)
			Dim op As DynamicCustomOp = New SpaceToDepth(input, expOut, blockSize, DataFormat.NHWC)
			Nd4j.Executioner.exec(op)

			sd.associateArrayWithVariable(input, sdInput)

			Dim t As SDVariable = sd.cnn().spaceToDepth("std", sdInput, blockSize, DataFormat.NHWC)
			'new SpaceToDepth(sd, sdInput, blockSize, dataFormat).outputVariable();
			Dim loss As SDVariable = sd.mean("loss", t)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("std", expOut).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDepthToSpace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDepthToSpace(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(1337)

			Dim miniBatch As Integer = 128
			Dim blockSize As Integer = 4
			Dim inputShape() As Integer = {miniBatch, 2, 2, blockSize * blockSize}

			Dim input As INDArray = Nd4j.randn(inputShape)
			Dim sd As SameDiff = SameDiff.create()
			Dim sdInput As SDVariable = sd.var("in", inputShape)

			Dim expOut As INDArray = Nd4j.create(miniBatch, 2 * blockSize, 2 * blockSize, 1)
			Dim op As DynamicCustomOp = New DepthToSpace(input, expOut, blockSize, DataFormat.NHWC)
			Nd4j.Executioner.exec(op)

			sd.associateArrayWithVariable(input, sdInput)

			Dim t As SDVariable = sd.cnn().depthToSpace("dts", sdInput, blockSize, DataFormat.NHWC)
			Dim loss As SDVariable = sd.mean("loss", t)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("dts", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBatchToSpace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchToSpace(ByVal backend As Nd4jBackend)
			'OpValidationSuite.ignoreFailing();          //TODO: https://github.com/eclipse/deeplearning4j/issues/6863
			Nd4j.Random.setSeed(1337)

			Dim miniBatch As Integer = 4
			Dim inputShape() As Integer = {miniBatch, 1, 1, 1}

			Dim M As Integer = 2
			Dim blockShape() As Integer = {M, 1}
			Dim cropShape() As Integer = {M, 2}

			Dim input As INDArray = Nd4j.randn(inputShape).castTo(DataType.DOUBLE)
			Dim crops As INDArray = Nd4j.create(New Single(){0, 0, 0, 0}, cropShape).castTo(DataType.INT)

			Dim sd As SameDiff = SameDiff.create()

			Dim sdInput As SDVariable = sd.var("in", inputShape)

			Dim expOut As INDArray = Nd4j.create(DataType.DOUBLE, 1, 2, 2, 1)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("batch_to_space").addInputs(input, crops).addIntegerArguments(2).addOutputs(expOut).build()
			Nd4j.Executioner.exec(op)

			sd.associateArrayWithVariable(input, sdInput)

			Dim t As SDVariable = sd.cnn().batchToSpace("bts", sdInput, New Integer(){2, 2}, New Integer(){0, 0}, New Integer(){0, 0})
			Dim loss As SDVariable = sd.mean("loss", t)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("bts", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpaceToBatch(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpaceToBatch(ByVal backend As Nd4jBackend)
			'OpValidationSuite.ignoreFailing();          //TODO: https://github.com/eclipse/deeplearning4j/issues/6863

			Nd4j.Random.setSeed(7331)

			Dim miniBatch As Integer = 4
			Dim inputShape() As Integer = {1, 2, 2, 1}

			Dim M As Integer = 2
			Dim blockShape() As Integer = {M, 1}
			Dim paddingShape() As Integer = {M, 2}

			Dim input As INDArray = Nd4j.randn(inputShape).castTo(DataType.DOUBLE)
			Dim padding As INDArray = Nd4j.create(New Single(){0, 0, 0, 0}, paddingShape).castTo(DataType.INT)

			Dim sd As SameDiff = SameDiff.create()

			Dim sdInput As SDVariable = sd.var("in", inputShape)

			Dim expOut As INDArray = Nd4j.create(DataType.DOUBLE, miniBatch, 1, 1, 1)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("space_to_batch").addIntegerArguments(2).addInputs(input, padding).addOutputs(expOut).build()
			Nd4j.Executioner.exec(op)

			sd.associateArrayWithVariable(input, sdInput)

			Dim t As SDVariable = sd.cnn().spaceToBatch("stb", sdInput, New Integer(){2, 2}, New Integer(){0, 0}, New Integer(){0, 0})
			Dim loss As SDVariable = sd.mean("loss", t)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("stb", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDynamicPartition(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDynamicPartition(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim ia As INDArray = Nd4j.create(New Double(){4, 3, 5, 7, 8, 0})
			Dim partitions As INDArray = Nd4j.create(New Double(){1, 0, 1, 0, 0, 1}).castTo(DataType.INT)
			Dim numPartitions As Integer = 2

			Dim [in] As SDVariable = sd.var("in", DataType.DOUBLE, New Long(){6})
			Dim sdPartitions As SDVariable = sd.var("partitions", DataType.INT, New Long(){6})

			Dim expOut1 As INDArray = Nd4j.create(DataType.DOUBLE, 3L)
			Dim expOut2 As INDArray = Nd4j.create(DataType.DOUBLE, 3L)
			Dim expOut() As INDArray = {expOut1, expOut2}

			Dim dynamicPartition As DynamicCustomOp = DynamicCustomOp.builder("dynamic_partition").addInputs(ia, partitions).addIntegerArguments(numPartitions).addOutputs(expOut1, expOut2).build()
			Nd4j.Executioner.exec(dynamicPartition)

			Dim parts() As SDVariable = sd.dynamicPartition(New String(){"dp0", "dp1"}, [in], sdPartitions, numPartitions)

			' merge the output partitions together again, to retrieve a single
			' tensor and finally a scalar.
			Dim t As SDVariable = sd.math().mergeAdd(parts)
			Dim loss As SDVariable = sd.mean("loss", t)

			sd.associateArrayWithVariable(ia, [in])
			sd.associateArrayWithVariable(partitions, sdPartitions)

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True).gradCheckSkipVariables("partitions").expectedOutput("dp0", expOut(0)).expectedOutput("dp1", expOut(1)).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDynamicPartition2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDynamicPartition2(ByVal backend As Nd4jBackend)
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDynamicStitch(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDynamicStitch(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim ia As INDArray = Nd4j.create(New Double(){5, 1, 3}, New Long(){3})
			Dim ib As INDArray = Nd4j.create(New Double(){7, 2, 4}, New Long(){3})
			Dim indexA As INDArray = Nd4j.create(New Double(){0, 1, 4}, New Long(){3}).castTo(DataType.INT)
			Dim indexB As INDArray = Nd4j.create(New Double(){2, 3, 5}, New Long(){3}).castTo(DataType.INT)

			Dim expOut As INDArray = Nd4j.create(DataType.DOUBLE, 6)

			Dim dynamicStitch As DynamicCustomOp = DynamicCustomOp.builder("dynamic_stitch").addInputs(indexA, indexB, ia, ib).addOutputs(expOut).build()
			Nd4j.Executioner.exec(dynamicStitch)

			Dim expOut2 As INDArray = Nd4j.create(New Double(){5, 1, 7, 2, 3, 4})
			assertEquals(expOut2, expOut)

			Dim in1 As SDVariable = sd.var("in1", ia)
			Dim in2 As SDVariable = sd.var("in2", ib)

			Dim index1 As SDVariable = sd.constant("index1", indexA)
			Dim index2 As SDVariable = sd.constant("index2", indexB)

			Dim t As SDVariable = sd.dynamicStitch("ds", New SDVariable(){index1, index2}, New SDVariable(){in1, in2})
			Dim loss As SDVariable = sd.standardDeviation("loss", t, True)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("ds", expOut).gradientCheck(True).gradCheckSkipVariables("index1", "index2"))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiag(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDiag(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim ia As INDArray = Nd4j.create(New Double(){1, 2}, New Integer(){2})
			Dim [in] As SDVariable = sd.var("in", DataType.DOUBLE, New Long(){2})
			Dim expOut As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 0},
				New Double() {0, 2}
			})

			Dim expOut2 As INDArray = Nd4j.create(DataType.DOUBLE, 2, 2)
			Dim diag As DynamicCustomOp = DynamicCustomOp.builder("diag").addInputs(ia).addOutputs(expOut2).build()
			Nd4j.Executioner.exec(diag)

			assertEquals(expOut, expOut2)

			Dim t As SDVariable = sd.math().diag("diag", [in])

			Dim loss As SDVariable = sd.standardDeviation("loss", t, False, 0, 1)

			sd.associateArrayWithVariable(ia, [in])

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("diag", expOut).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiagPart(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDiagPart(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim input As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(4), 4)
			Dim expOut As INDArray = Nd4j.create(New Single(){1, 6, 11, 16}).castTo(DataType.DOUBLE)

			Dim [in] As SDVariable = sd.var("in", input)
			Dim t As SDVariable = sd.math().diagPart("dp", [in])

			' dimension is 0 here, because output of diagPart is vector, not matrix
			Dim loss As SDVariable = sd.standardDeviation("loss", t, True, 0)

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("dp", expOut).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEye(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEye(ByVal backend As Nd4jBackend)
			Dim rows() As Integer = {3, 3, 3, 3}
			Dim cols() As Integer = {3, 2, 2, 2}
			Dim batch()() As Integer = {
				New Integer() {},
				New Integer() {},
				New Integer() {4},
				New Integer() {3, 3}
			}
			Dim expOut(3) As INDArray

			expOut(0) = Nd4j.eye(3).castTo(DataType.DOUBLE)
			expOut(1) = Nd4j.create(New Double()(){
				New Double() {1, 0},
				New Double() {0, 1},
				New Double() {0, 0}
			})
			expOut(2) = Nd4j.create(DataType.DOUBLE, 4, 3, 2)
			For i As Integer = 0 To 3
				expOut(2).get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.all()).assign(expOut(1))
			Next i
			expOut(3) = Nd4j.create(DataType.DOUBLE, 3, 3, 3, 2)
			For i As Integer = 0 To 2
				For j As Integer = 0 To 2
					expOut(3).get(NDArrayIndex.point(i), NDArrayIndex.point(j), NDArrayIndex.all(), NDArrayIndex.all()).assign(expOut(1))
				Next j
			Next i

			For i As Integer = 0 To 2
				Dim sd As SameDiff = SameDiff.create()
				Dim eye As SDVariable = sd.math().eye("e", rows(i), cols(i), DataType.DOUBLE, batch(i))

				Dim loss As SDVariable = sd.standardDeviation("loss", eye, True)

				Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("e", expOut(i)).gradCheckSkipVariables("e").gradientCheck(False))
				assertNull(err)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEyeShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEyeShape(ByVal backend As Nd4jBackend)
			Dim dco As DynamicCustomOp = DynamicCustomOp.builder("eye").addIntegerArguments(3, 3).build()

			Dim list As val = Nd4j.Executioner.calculateOutputShape(dco)
			assertEquals(1, list.size()) 'Fails here - empty list
			assertArrayEquals(New Long(){3, 3}, list.get(0).getShape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTransforms(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTransforms(ByVal backend As Nd4jBackend)
			'Test transforms (non-pairwise)
			Nd4j.Random.setSeed(12345)

			Dim allFailed As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 81
				Dim sd As SameDiff = SameDiff.create()

				Dim nOut As Integer = 4
				Dim minibatch As Integer = 5
				Dim [in] As SDVariable = sd.var("in", minibatch, nOut)

				Dim ia As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

				Dim [dim] As Integer
				Dim t As SDVariable
				Dim tc As New TestCase(sd)
				Dim stdevLoss As Boolean = False
				Dim opName As String = Nothing
				Select Case i
					Case 0
						t = [in].add(5.0)
						tc.expectedOutput(t.name(), ia.add(5.0))
					Case 1
						t = [in].sub(5.0)
						tc.expectedOutput(t.name(), ia.sub(5.0))
					Case 2
						t = [in].mul(2.5)
						tc.expectedOutput(t.name(), ia.mul(2.5))
					Case 3
						t = [in].div(4.0)
						tc.expectedOutput(t.name(), ia.div(4.0))
					Case 4
						t = [in].rsub(5.0)
						tc.expectedOutput(t.name(), ia.rsub(5.0))
					Case 5
						t = [in].rdiv(1.0)
						tc.expectedOutput(t.name(), ia.rdiv(1.0))
					Case 6
						t = sd.math().pow([in], 2.5)
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.pow(ia, 2.5, True))
					Case 7
						t = sd.nn().sigmoid([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).muli(2).subi(1.0)
						tc.expectedOutput(t.name(), Transforms.sigmoid(ia, True))
					Case 8
						t = sd.math().tanh([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).muli(2).subi(1.0)
						tc.expectedOutput(t.name(), Transforms.tanh(ia, True))
					Case 9
						ia.assign(Nd4j.rand(DataType.DOUBLE, ia.shape()))
						t = sd.math().tan([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.tan(ia))
					Case 10
						t = sd.math().cos([in])
						tc.expectedOutput(t.name(), Transforms.cos(ia, True))
					Case 11
						t = sd.math().sin([in])
						tc.expectedOutput(t.name(), Transforms.sin(ia, True))
					Case 12
						t = sd.nn().softplus([in])
						tc.expectedOutput(t.name(), Transforms.softPlus(ia, True))
					Case 13
						t = sd.math().log([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.log(ia, True))
					Case 14
						t = sd.math().neg([in])
						Dim exp14 As INDArray = ia.neg()
						tc.expectedOutput(t.name(), exp14)
					Case 15
						t = sd.math().acos([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).muli(1.8).subi(0.9)
						tc.expectedOutput(t.name(), Transforms.acos(ia, True))
					Case 16
						t = sd.math().acosh([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).addi(1.01) 'Only defined for x >= 1
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New ACosh(ia.dup())))
					Case 17
						t = sd.math().asin([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).muli(1.8).subi(0.9)
						tc.expectedOutput(t.name(), Transforms.asin(ia, True))
					Case 18
						t = sd.math().atan([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).muli(4).subi(2)
						tc.expectedOutput(t.name(), Transforms.atan(ia, True))
					Case 19
						t = sd.math().atanh([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut).muli(1.8).subi(0.9)
						tc.expectedOutput(t.name(), Transforms.atanh(ia, True))
					Case 20
						t = sd.math().cosh([in])
						tc.expectedOutput(t.name(), Transforms.cosh(ia, True))
					Case 21
						t = sd.math().cube([in])
						tc.expectedOutput(t.name(), Transforms.pow(ia, 3.0, True))
					Case 22
						t = sd.nn().elu([in])
						tc.expectedOutput(t.name(), Transforms.elu(ia, True))
					Case 23
						'TODO SHOULDN'T THIS HAVE A DIMENSION ARG???
						t = sd.nn().softmax([in], -1)
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New SoftMax(ia.dup()))(0))
					Case 24
						t = sd.math().sqrt([in])
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.sqrt(ia, True))
					Case 25
						t = sd.math().square([in])
						tc.expectedOutput(t.name(), Transforms.pow(ia, 2.0, True))
					Case 26
						t = sd.transpose([in])
						tc.expectedOutput(t.name(), ia.transpose().dup())
					Case 27
						t = sd.math().abs([in])
						tc.expectedOutput(t.name(), Transforms.abs(ia, True))
					Case 28
						t = sd.math().sinh([in])
						tc.expectedOutput(t.name(), Transforms.sinh(ia, True))
					Case 29
						t = sd.math().asinh([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New ASinh(ia.dup())))
					Case 30
						t = sd.math().exp([in])
						tc.expectedOutput(t.name(), Transforms.exp(ia, True))
					Case 31
						t = sd.math().floor([in])
						tc.expectedOutput(t.name(), Transforms.floor(ia, True))
					Case 32
						t = sd.nn().relu([in], 0.0)
						ia = Nd4j.rand(minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.relu(ia, True))
					Case 33
						t = sd.nn().hardTanh([in])
						ia = Nd4j.rand(minibatch, nOut).muli(2).subi(1.0)
						tc.expectedOutput(t.name(), Transforms.hardTanh(ia, True))
					Case 34
						t = sd.nn().logSigmoid([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New LogSigmoid(ia.dup())))
					Case 35
						t = sd.nn().swish([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New Swish(ia.dup())))
					Case 36
						t = sd.math().sign([in])
						tc.expectedOutput(t.name(), Transforms.sign(ia, True))
					Case 37
						t = sd.nn().softsign([in])
						tc.expectedOutput(t.name(), Transforms.softsign(ia, True))
					Case 38
						t = sd.nn().leakyRelu([in], 0.0)
						ia = Nd4j.rand(minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.leakyRelu(ia, True))
					Case 39
						If OpValidationSuite.IGNORE_FAILING Then
							Continue For
						End If
						t = sd.nn().logSoftmax([in])
						ia = Nd4j.rand(minibatch, nOut).muli(10).subi(5)
						tc.expectedOutput(t.name(), Transforms.log(Transforms.softmax(ia, True)))
						stdevLoss = True
					Case 40
						t = sd.nn().selu([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New SELU(ia.dup())))
					Case 41
						t = sd.gt([in], 1.0).castTo(DataType.DOUBLE)
						tc.expectedOutput(t.name(), ia.gt(1.0).castTo(DataType.DOUBLE)).gradientCheck(False)
					Case 42
						t = sd.gte([in], 1.0).castTo(DataType.DOUBLE)
						tc.expectedOutput(t.name(), ia.gte(1.0).castTo(DataType.DOUBLE)).gradientCheck(False)
					Case 43
						t = sd.lt([in], 1.0).castTo(DataType.DOUBLE)
						tc.expectedOutput(t.name(), ia.lt(1.0).castTo(DataType.DOUBLE)).gradientCheck(False)
					Case 44
						t = sd.lte([in], 1.0).castTo(DataType.DOUBLE)
						tc.expectedOutput(t.name(), ia.lte(1.0).castTo(DataType.DOUBLE)).gradientCheck(False)
					Case 45
						t = sd.eq([in], 2.0).castTo(DataType.DOUBLE)
						ia = Nd4j.linspace(1, minibatch * nOut, minibatch * nOut, DataType.DOUBLE).reshape("c"c, minibatch, nOut)
						tc.expectedOutput(t.name(), ia.eq(2.0).castTo(DataType.DOUBLE)).gradientCheck(False)
					Case 46
						t = sd.neq([in], 2.0).castTo(DataType.DOUBLE)
						ia = Nd4j.linspace(1, minibatch * nOut, minibatch * nOut, DataType.DOUBLE).reshape("c"c, minibatch, nOut)
						tc.expectedOutput(t.name(), ia.neq(2.0).castTo(DataType.DOUBLE)).gradientCheck(False)
					Case 47
						t = sd.math().ceil([in])
						tc.expectedOutput(t.name(), Transforms.ceil(ia, True))
					Case 48
						ia = Nd4j.randn(DataType.DOUBLE, ia.shape()).muli(2)
						t = sd.math().clipByValue([in], -3, 2)
						Dim expOut48 As INDArray = ia.dup()
						BooleanIndexing.replaceWhere(expOut48, -3, Conditions.lessThan(-3))
						BooleanIndexing.replaceWhere(expOut48, 2, Conditions.greaterThan(2))
						tc.expectedOutput(t.name(), expOut48)
					Case 49
						'Clip by norm, dimension 0, some below threshold, some above
						Dim clip As Double = 2.0
						t = sd.math().clipByNorm([in], clip, 0)
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape())
						ia.diviRowVector(ia.norm2(0)).muli(clip) 'Norm2 is now 'clip' (i.e., exactly at threshold
						'System.out.println(ia.norm2(0));
						ia.muliColumnVector(Nd4j.linspace(0.9, 1.1, ia.size(0), DataType.DOUBLE).reshape(ChrW(ia.size(0)), 1))
						'System.out.println(ia.norm2(0));

						Dim expOut49 As INDArray = Nd4j.create(DataType.DOUBLE, ia.shape())
						Dim j As Integer = 0
						Do While j < ia.columns()
							Dim origCol As INDArray = ia.getColumn(j)
							If origCol.norm2Number().doubleValue() < clip Then
								expOut49.putColumn(j, origCol)
							Else
								expOut49.putColumn(j, origCol.mul(clip / origCol.norm2Number().doubleValue()))
							End If
							j += 1
						Loop
						tc.expectedOutput(t.name(), expOut49)
						'System.out.println(expOut.norm2(0));
					'TODO clip by norm along other dimensions
					Case 50
						[dim] = 1
						t = sd.reverse([in], [dim])
						Dim expOut50 As INDArray = Nd4j.create(DataType.DOUBLE, ia.shape())
						Dim reverse As DynamicCustomOp = DynamicCustomOp.builder("reverse").addIntegerArguments([dim]).addInputs(ia).addOutputs(expOut50).build()
						Nd4j.Executioner.exec(reverse)
						tc.expectedOutput(t.name(), expOut50)
					Case 51
						[dim] = 0
						Dim exclusive As Boolean = False
						Dim reverseBool As Boolean = False

						t = sd.cumsum([in], exclusive, reverseBool, [dim])
						Dim expOut51 As INDArray = Nd4j.create(DataType.DOUBLE, ia.shape())
						Dim cumsum As DynamicCustomOp = DynamicCustomOp.builder("cumsum").addIntegerArguments(If(exclusive, 1, 0),If(reverseBool, 1, 0), [dim]).addInputs(ia).addOutputs(expOut51).build()
						Nd4j.Executioner.exec(cumsum)
						tc.expectedOutput(t.name(), expOut51)
					Case 52
						If OpValidationSuite.IGNORE_FAILING Then
							Continue For
						End If
						Dim ex As Boolean = False
						Dim revBool As Boolean = False
						t = sd.cumprod([in], ex, revBool, 0)
						Dim expOut52 As INDArray = Nd4j.create(DataType.DOUBLE, ia.shape())
						Dim s0 As Integer = 0
						Do While s0 < ia.size(0)
							Dim s1 As Integer = 0
							Do While s1 < ia.size(1)
								Dim prod As Double = 1.0
								For x As Integer = 0 To s0
									prod *= ia.getDouble(x, s1)
								Next x
								expOut52.putScalar(s0, s1, prod)
								s1 += 1
							Loop
							s0 += 1
						Loop
						tc.expectedOutput(t.name(), expOut52)
					Case 53
						If OpValidationSuite.IGNORE_FAILING Then
							Continue For
						End If
						t = sd.math().diag([in])
						ia = Nd4j.create(New Single(){4, 2})
						[in] = sd.var("in", 1, 2)
						Dim expOut53 As INDArray = Nd4j.create(DataType.DOUBLE, 2, 2)
						Dim op As DynamicCustomOp = DynamicCustomOp.builder("diag").addInputs(ia).addOutputs(expOut53).build()
						Nd4j.Executioner.exec(op)
						tc.expectedOutput(t.name(), expOut53)
					Case 54
						t = sd.math().erf([in])
						Dim expOut54 As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, ia.shape(), ia.ordering())
						Nd4j.Executioner.exec(New Erf(ia, expOut54))
						tc.expectedOutput(t.name(), expOut54)
					Case 55
						t = sd.math().erfc([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New Erfc(ia, Nd4j.createUninitialized(ia.shape(), ia.ordering()))))
					Case 56
						t = sd.math().expm1([in])
						tc.expectedOutput(t.name(), Transforms.expm1(ia, True))
					Case 57
						t = sd.math().log1p([in])
						ia = Nd4j.rand(minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.log1p(ia, True))
					Case 58
						t = sd.math().round([in])
						tc.expectedOutput(t.name(), Transforms.round(ia, True))
					Case 59
						ia = Nd4j.create(New Single(){4, 2}).castTo(DataType.DOUBLE)
	'                    in = sd.var("in", new int[]{1, 2});
						t = sd.math().rsqrt([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New RSqrt(ia, Nd4j.create(ia.shape(), ia.ordering()))))
					Case 60
						t = sd.nn().relu6([in], 0)
						ia = Nd4j.rand(DataType.DOUBLE, minibatch, nOut)
						tc.expectedOutput(t.name(), Transforms.relu6(ia, True))
					Case 61
						ia = Nd4j.create(New Single(){2, 2}).castTo(DataType.DOUBLE)
						sd.associateArrayWithVariable(ia, [in])
						Dim value As Double = 42
						t = sd.fill([in].castTo(DataType.INT), DataType.DOUBLE, value)
						tc.expectedOutput(t.name(), Nd4j.valueArrayOf(New Integer(){2, 2}, 42)).gradientCheck(False)
						opName = "fill"
					Case 62
						t = sd.nn().hardSigmoid([in])
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New HardSigmoid(ia, ia.dup())))
					Case 63
						t = sd.scalarMax([in], 0.5)
						tc.expectedOutput(t.name(), Transforms.max(ia, 0.5, True))
					Case 64
						t = sd.scalarMin([in], 0.5)
						tc.expectedOutput(t.name(), Transforms.min(ia, 0.5, True))
					Case 65
						Continue For ' assign op was removed.
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case 66
						t = sd.math().floorMod([in], 0.5)
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New ScalarFMod(ia.dup(), 0.5)))
					Case 67
						t = sd.math().reciprocal([in])
						tc.expectedOutput(t.name(), ia.rdiv(1.0))
					Case 68
						t = sd.shape([in]).castTo(DataType.DOUBLE)
						tc.expectedOutput(t.name(), Nd4j.create(ArrayUtil.toDouble(ia.shape()))).gradientCheck(False)
					Case 69
						t = sd.rank([in]).castTo(DataType.DOUBLE)
						tc.expectedOutput(t.name(), Nd4j.scalar(CDbl(ia.rank()))).gradientCheck(False)
					Case 70
						t = sd.onesLike([in])
						tc.expectedOutput(t.name(), Nd4j.ones(ia.shape()))
					Case 71
						ia = Nd4j.randn(DataType.DOUBLE, nOut, nOut)
						t = sd.math().diagPart([in])
						tc.expectedOutput(t.name(), Nd4j.create(New Double(){ia.getDouble(0, 0), ia.getDouble(1, 1), ia.getDouble(2, 2), ia.getDouble(3, 3)}).castTo(DataType.DOUBLE))
					Case 72
						t = sd.identity([in])
						tc.expected(t, ia.dup())
					Case 73
						t = sd.math().step([in], 1.0)
						tc.expected(t, ia.gte(1.0).castTo(DataType.DOUBLE))
					Case 74
						Continue For
					Case 75
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape())
						t = sd.math().log([in], 2)
						tc.expected(t, Transforms.log(ia, 2, True))
					Case 76
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape())
						t = sd.math().log([in], 10)
						tc.expected(t, Transforms.log(ia, 10, True))
					Case 77
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape())
						t = sd.matchCondition([in], Conditions.lessThan(0.5)).castTo(DataType.DOUBLE)
						Dim exp As INDArray = ia.dup().lt(0.5).castTo(DataType.DOUBLE)
						tc.expected(t, exp).gradientCheck(False)
					Case 78
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape()).muli(2).subi(1)
						t = sd.math().rationalTanh([in])
						tc.expected(t, Nd4j.Executioner.exec(New RationalTanh(ia.dup())))
					Case 79
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape()).muli(2).subi(1)
						t = sd.math().rectifiedTanh([in])
						tc.expected(t, Nd4j.Executioner.exec(New RectifiedTanh(ia.dup())))
					Case 80
						t = sd.nn().gelu([in])
						Dim gelu As INDArray = Transforms.sigmoid(ia.mul(1.702)).mul(ia)
						tc.expected(t, gelu)
					Case 81
						ia = Nd4j.rand(DataType.DOUBLE, ia.shape()).muli(0.5)
						t = sd.nn().preciseGelu([in])
						Dim x3 As INDArray = Transforms.pow(ia.mul(0.044715), 3, True)
						Dim inner1 As INDArray = ia.add(x3).mul(Math.Sqrt(2.0 / Math.PI))
						Dim inner2 As INDArray = Transforms.tanh(inner1, True).addi(1.0)
						Dim geluPrecise As INDArray = inner2.mul(ia).mul(0.5)
						tc.expected(t, geluPrecise)
					Case Else
						Throw New Exception()
				End Select


				Dim funcs() As DifferentialFunction = sd.ops()
				Dim name As String = If(opName Is Nothing, funcs(0).opName(), opName)


				Dim msg As String = "test: " & i & " - " & name
				log.info("*** Starting test: " & msg)

				Dim loss As SDVariable
				If stdevLoss Then
					loss = sd.standardDeviation("loss", t, False, Integer.MaxValue) '.standardDeviation("loss", t, true, Integer.MAX_VALUE);
				Else
					loss = sd.mean("loss", t)
				End If

				sd.associateArrayWithVariable(ia, [in])

				tc.testName(name)
				Dim [error] As String = OpValidation.validate(tc, True)
				If [error] IsNot Nothing Then
					allFailed.Add(name & " - " & [error])
				End If
			Next i

			If allFailed.Count > 0 Then
				log.error("All failed transforms: " & allFailed)
				fail(allFailed.Count & " transforms failed")
			End If
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPairwiseTransforms(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPairwiseTransforms(ByVal backend As Nd4jBackend)
	'        
	'        add, sub, mul, div, rsub, rdiv
	'        eq, neq, gt, lt, gte, lte, or, and, xor
	'        min, max
	'        mmul
	'        tensormmul
	'         
			'Test transforms (pairwise)
			Nd4j.Random.setSeed(12345)

			Dim allFailed As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 22

				Dim sd As SameDiff = SameDiff.create()

				Dim nOut As Integer = 4
				Dim minibatch As Integer = 5
				Dim in1 As SDVariable = sd.var("in1", DataType.DOUBLE, minibatch, nOut)
				Dim in2 As SDVariable = sd.var("in2", DataType.DOUBLE, minibatch, nOut)

				Dim ia As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
				Dim ib As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

				Dim t As SDVariable
				Dim tc As New TestCase(sd)
				Dim opName As String = Nothing
				Select Case i
					Case 0
						t = in1.add(in2)
						tc.expectedOutput(t.name(), ia.add(ib))
					Case 1
						t = in1.sub(in2)
						tc.expectedOutput(t.name(), ia.sub(ib))
					Case 2
						t = in1.mul(in2)
						tc.expectedOutput(t.name(), ia.mul(ib))
					Case 3
						t = in1.div(in2)
						tc.expectedOutput(t.name(), ia.div(ib))
					Case 4
						t = in1.rsub(in2)
						tc.expectedOutput(t.name(), ia.rsub(ib))
					Case 5
						ia.assign(Nd4j.rand(ia.shape())).addi(0.5)
						ib.assign(Nd4j.rand(ib.shape())).addi(0.5)
						t = in1.rdiv(in2)
						tc.expectedOutput(t.name(), ia.rdiv(ib))
					Case 6
						t = sd.eq(in1, in2)
						opName = "eq"
						tc.expectedOutput(t.name(), ia.eq(ib)).gradientCheck(False)
					Case 7
						t = sd.neq(in1, in2)
						opName = "neq"
						tc.expectedOutput(t.name(), ia.neq(ib)).gradientCheck(False)

					Case 8
						t = sd.gt(in1, in2)
						opName = "gt"
						tc.expectedOutput(t.name(), ia.gt(ib)).gradientCheck(False)
					Case 9
						t = sd.lt(in1, in2)
						opName = "lt"
						tc.expectedOutput(t.name(), ia.lt(ib)).gradientCheck(False)
					Case 10
						t = sd.gte(in1, in2)
						opName = "gte"
						Dim expOut10 As INDArray = Nd4j.create(DataType.BOOL, ia.shape())
						Nd4j.Executioner.exec(New GreaterThanOrEqual(New INDArray(){ia, ib}, New INDArray(){expOut10}))
						tc.expectedOutput(t.name(), expOut10).gradientCheck(False)
					Case 11
						t = sd.lte(in1, in2)
						opName = "lte"
						Dim expOut11 As INDArray = Nd4j.create(DataType.BOOL, ia.shape())
						Nd4j.Executioner.exec(New LessThanOrEqual(New INDArray(){ia, ib}, New INDArray(){expOut11}))
						tc.expectedOutput(t.name(), expOut11).gradientCheck(False)
					Case 12
						ia = Nd4j.Executioner.exec(New BernoulliDistribution(ia, 0.5))
						ib = Nd4j.Executioner.exec(New BernoulliDistribution(ib, 0.5))
						t = sd.math().or(in1.castTo(DataType.BOOL), in2.castTo(DataType.BOOL))
						opName = "or"
						tc.expectedOutput(t.name(), Transforms.or(ia.castTo(DataType.BOOL), ib.castTo(DataType.BOOL))).gradientCheck(False)
					Case 13
						ib = Nd4j.randn(DataType.DOUBLE, nOut, nOut)
						t = sd.mmul(in1, in2)
						tc.expectedOutput(t.name(), ia.mmul(ib))
					Case 14
						t = sd.max(in1, in2)
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New Max(ia, ib, ia.dup()))(0))
					Case 15
						t = sd.min(in1, in2)
						tc.expectedOutput(t.name(), Nd4j.Executioner.exec(New Min(ia, ib, ia.dup()))(0))
					Case 16
						ia = Nd4j.Executioner.exec(New BernoulliDistribution(ia, 0.5))
						ib = Nd4j.Executioner.exec(New BernoulliDistribution(ib, 0.5))
						t = sd.math().and(in1.castTo(DataType.BOOL), in2.castTo(DataType.BOOL))
						opName = "and"
						tc.expectedOutput(t.name(), Transforms.and(ia.castTo(DataType.BOOL), ib.castTo(DataType.BOOL))).gradientCheck(False)
					Case 17
						ia = Nd4j.Executioner.exec(New BernoulliDistribution(ia, 0.5))
						ib = Nd4j.Executioner.exec(New BernoulliDistribution(ib, 0.5))
						t = sd.math().xor(in1.castTo(DataType.BOOL), in2.castTo(DataType.BOOL))
						opName = "xor"
						tc.expectedOutput(t.name(), Transforms.xor(ia.castTo(DataType.BOOL), ib.castTo(DataType.BOOL))).gradientCheck(False)
					Case 18
						Continue For 'assign op was removed.
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case 19
						t = sd.math().atan2(in1, in2)
						tc.expectedOutput(t.name(), Transforms.atan2(ib, ia)) 'Note: y,x order for samediff; x,y order for transforms
					Case 20
						t = sd.math().mergeAdd(New SDVariable(){in1, in2, in2})
						tc.expectedOutput(t.name(), ia.add(ib).add(ib))
					Case 21
						t = in1.squaredDifference(in2)
						Dim expOut21 As INDArray = Nd4j.create(ia.shape(), ia.ordering())
						Dim squareDiff As DynamicCustomOp = DynamicCustomOp.builder("squaredsubtract").addInputs(ia, ib).addOutputs(expOut21).build()
						Nd4j.Executioner.exec(squareDiff)
						tc.expectedOutput(t.name(), expOut21)
					Case 22
						'set diag
						ia = Nd4j.randn(DataType.DOUBLE, nOut, nOut)
						ib = Nd4j.randn(DataType.DOUBLE, 1, nOut).reshape(nOut)
						Dim expOut22 As INDArray = ia.dup()
						For j As Integer = 0 To nOut - 1
							expOut22.putScalar(j, j, ib.getDouble(j))
						Next j
						t = sd.math().setDiag(in1, in2)
						tc.expectedOutput(t.name(), expOut22)
					Case Else
						Throw New Exception()
				End Select


				Dim funcs() As DifferentialFunction = sd.ops()
				Dim name As String = (If(opName Is Nothing, funcs(0).opName(), opName))

				Dim msg As String = "test: " & i & " - " & name
				log.info("***** Starting test: {} *****", msg)

				Dim loss As SDVariable = sd.mean("loss", t.castTo(DataType.DOUBLE))

				sd.associateArrayWithVariable(ia, in1)
				sd.associateArrayWithVariable(ib, in2)

				tc.testName(name)
				Dim [error] As String = OpValidation.validate(tc, True)
				If [error] IsNot Nothing Then
					allFailed.Add(name & "(" & [error] & ")")
				End If
			Next i

			If allFailed.Count > 0 Then
				log.error("All failed transforms: " & allFailed)
				fail(allFailed.Count & " transforms failed: " & allFailed)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsX(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsX(ByVal backend As Nd4jBackend)
			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To 3

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", 4)

				Dim [out] As SDVariable
				Dim exp As INDArray
				Dim inArr As INDArray
				Select Case i
					Case 0
						inArr = Nd4j.create(New Double(){10, Double.PositiveInfinity, 0, Double.NegativeInfinity})
						exp = Nd4j.create(New Boolean(){True, False, True, False})
						[out] = sd.math().isFinite([in])
					Case 1
						inArr = Nd4j.create(New Double(){10, Double.PositiveInfinity, 0, Double.NegativeInfinity})
						exp = Nd4j.create(New Boolean(){False, True, False, True})
						[out] = sd.math().isInfinite([in])
					Case 2
						'TODO: IsMax supports both bool and float out: https://github.com/eclipse/deeplearning4j/issues/6872
						inArr = Nd4j.create(New Double(){-3, 5, 0, 2})
						exp = Nd4j.create(New Boolean(){False, True, False, False})
						[out] = sd.math().isMax([in])
					Case 3
						inArr = Nd4j.create(New Double(){0, Double.NaN, 10, Double.NaN})
						exp = Nd4j.create(New Boolean(){False, True, False, True})
						[out] = sd.math().isNaN([in])
					Case Else
						Throw New Exception()
				End Select

				Dim other As SDVariable = sd.var("other", Nd4j.rand(DataType.DOUBLE, 4))

				Dim loss As SDVariable = [out].castTo(DataType.DOUBLE).add(other).mean()
				Dim tc As TestCase = (New TestCase(sd)).gradientCheck(False).expected([out], exp)

				[in].Array = inArr

				Dim err As String = OpValidation.validate(tc, True)
				If err IsNot Nothing Then
					failed.Add(err)
				End If
			Next i
			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReplaceWhereScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReplaceWhereScalar(ByVal backend As Nd4jBackend)
			For Each c As Condition In New Condition(){Conditions.lessThan(0.5), Conditions.greaterThan(0.5), Conditions.equals(0.5)}

				log.info("Testing condition: " & c.GetType().Name)
				Dim inArr As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 4)
				Dim inArr2 As INDArray = inArr.dup()
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", inArr)
				Dim where As SDVariable = sd.replaceWhere("out",[in], 10, c)

				Dim exp As INDArray = inArr.dup()
				BooleanIndexing.replaceWhere(exp, 10, c)

				Dim loss As SDVariable = where.std(True)
				Dim input As IDictionary(Of String, INDArray) = sd.output(Collections.singletonMap("in", inArr2), where.name())
				assertEquals(exp,input(where.name()))
				Dim tc As New TestCase(sd)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next c
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReplaceWhereArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReplaceWhereArray(ByVal backend As Nd4jBackend)
			For Each c As Condition In New Condition(){Conditions.lessThan(0.5), Conditions.greaterThan(0.5), Conditions.equals(0.5)}

				Dim inArr As INDArray = Nd4j.rand(3, 4)
				Dim inArr2 As INDArray = Nd4j.valueArrayOf(3, 4, 10)
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", inArr)
				Dim in2 As SDVariable = sd.var("in2", inArr2)
				Dim where As SDVariable = sd.replaceWhere([in], in2, c)

				Dim exp As INDArray = inArr.dup()
				BooleanIndexing.replaceWhere(exp, inArr2, c)

				Dim loss As SDVariable = where.std(True)

				Dim tc As New TestCase(sd)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next c
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogGrad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogGrad(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim input As SDVariable = sameDiff.var("x", Nd4j.linspace(1, 4, 4, DataType.DOUBLE))
			Dim log As SDVariable = sameDiff.math().log(input)
			Dim sum As SDVariable = sameDiff.sum(log, Integer.MaxValue)
			Dim result As INDArray = Nothing
			sameDiff.calculateGradients(java.util.Collections.emptyMap(), sameDiff.getVariables().keySet())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSigmoidBackwards(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSigmoidBackwards(ByVal backend As Nd4jBackend)
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim sumInput As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim inputs As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			inputs("x") = sumInput
			Dim input As SDVariable = sameDiff.var("x", inputs("x"))
			Dim sigmoid As SDVariable = sameDiff.nn().sigmoid(input)
			Dim sum As SDVariable = sameDiff.sum(sigmoid, Integer.MaxValue)
			Dim m As IDictionary(Of String, INDArray) = sameDiff.calculateGradients(java.util.Collections.emptyMap(), sameDiff.getVariables().keySet())
			Dim arr As INDArray = m(input.name())
			assertTrue(Nd4j.create(New Double()(){
				New Double() {0.1966, 0.1050},
				New Double() {0.0452, 0.0177}
			}).equalsWithEps(arr, 1e-2))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRank0EdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRank0EdgeCase(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim v1 As SDVariable = sd.sum(sd.var(Nd4j.create(New Double(){4, 4})))
			Dim d0 As Double = v1.eval().getDouble(0)
			assertEquals(8, d0, 0)

			Dim v2 As SDVariable = sd.sum(sd.var(Nd4j.create(New Double(){4, 4}))).div(2.0)
			Dim m As IDictionary(Of String, INDArray) = sd.outputAll(java.util.Collections.emptyMap())
			Dim d1 As Double = m(v2.name()).getDouble(0)
			assertEquals(4, d1, 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAtan2BroadcastShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAtan2BroadcastShape(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.create(New Long(){3, 1, 4})
			Dim arr2 As INDArray = Nd4j.create(New Long(){1, 2, 4})

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("tf_atan2").addInputs(arr1, arr2).build()

			Dim outShapes As val = Nd4j.Executioner.calculateOutputShape(op)
			assertEquals(1, outShapes.size())

			assertArrayEquals(New Long(){3, 2, 4}, outShapes.get(0).getShape(),java.util.Arrays.toString(outShapes.get(0).getShape()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBooleanAnd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBooleanAnd(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.create(New Long(){3, 4}).castTo(DataType.FLOAT)
			Dim arr2 As INDArray = Nd4j.create(New Long(){3, 4}).castTo(DataType.FLOAT)
			Dim [out] As INDArray = Nd4j.create(New Long(){3, 4}).castTo(DataType.FLOAT)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("boolean_and").addInputs(arr1, arr2).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterOpsScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterOpsScalar(ByVal backend As Nd4jBackend)
			For Each s As String In New String(){"add", "sub", "mul", "div"}
				Dim ref As INDArray = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape(ChrW(10), 3)
				Dim indices As INDArray = Nd4j.scalar(5)
				Dim upd As INDArray = Nd4j.create(New Double(){10, 20, 30})

				'The non-scalar case works:
	'            INDArray indices = Nd4j.create(new float[]{5});
	'            INDArray upd = Nd4j.create(new double[]{10, 20, 30}, new int[]{1, 3});

				Dim exp As INDArray = ref.dup()
				Select Case s
					Case "add"
						exp.getRow(5).addi(upd)
					Case "sub"
						exp.getRow(5).subi(upd)
					Case "mul"
						exp.getRow(5).muli(upd)
					Case "div"
						exp.getRow(5).divi(upd)
					Case Else
						Throw New Exception()
				End Select


				Dim [out] As INDArray = Nd4j.create(10, 3)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("scatter_" & s).addInputs(ref, indices, upd).addOutputs([out]).build()

				Nd4j.Executioner.exec(op)

				assertEquals(exp, [out],s)
			Next s
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("12/16/2019 https://github.com/eclipse/deeplearning4j/issues/8540") @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPad(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.valueArrayOf(New Long(){5}, 1.0)
			Dim pad As INDArray = Nd4j.create(New Double(){1, 1}, New Long(){1, 2}).castTo(DataType.LONG)
			Dim value As INDArray = Nd4j.scalar(10.0)

			Dim [out] As INDArray = Nd4j.create(New Long(){7})

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("pad").addInputs([in], pad, value).addOutputs([out]).addIntegerArguments(0).build()

			Dim exp As INDArray = Nd4j.create(New Double(){10, 1, 1, 1, 1, 1, 10})
			OpValidation.validate((New OpTestCase(op)).expectedOutput(0, exp))

			Dim sd As SameDiff = SameDiff.create()
			Dim s As SDVariable = sd.var("in", [in])
			Dim padded As SDVariable = sd.nn().pad(s, sd.constant(pad), 10.0)
			Dim err2 As String = OpValidation.validate((New TestCase(sd)).expected(padded, exp).gradientCheck(False))
			assertNull(err2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMirrorPad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMirrorPad(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim pad As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 1},
				New Double() {2, 2}
			}).castTo(DataType.INT)

			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 4, 7)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("mirror_pad").addInputs([in], pad).addOutputs([out]).addIntegerArguments(0).build()

			Nd4j.Executioner.exec(op)

			Dim exp As INDArray = Nd4j.create(New Double()(){
				New Double() {6, 5, 4, 5, 6, 5, 4},
				New Double() {3, 2, 1, 2, 3, 2, 1},
				New Double() {6, 5, 4, 5, 6, 5, 4},
				New Double() {3, 2, 1, 2, 3, 2, 1}
			})
			Dim err As String = OpValidation.validate((New OpTestCase(op)).expectedOutput(0, exp))

			assertNull(err)


			Dim sd As SameDiff = SameDiff.create()
			Dim s As SDVariable = sd.var("in", [in])
			Dim padded As SDVariable = sd.nn().pad(s, sd.constant(Nd4j.createFromArray(New Integer()(){
				New Integer() {1, 1},
				New Integer() {2, 2}
			})), PadMode.REFLECT, 0.0)
			Dim err2 As String = OpValidation.validate((New TestCase(sd)).expected(padded, exp).gradientCheck(False))
			assertNull(err2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMirrorPad2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMirrorPad2(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim pad As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 1},
				New Double() {2, 2}
			}).castTo(DataType.INT)

			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 4, 7)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("mirror_pad").addInputs([in], pad).addOutputs([out]).addIntegerArguments(1).build()

			Nd4j.Executioner.exec(op)

			Dim exp As INDArray = Nd4j.create(New Double()(){
				New Double() {2, 1, 1, 2, 3, 3, 2},
				New Double() {2, 1, 1, 2, 3, 3, 2},
				New Double() {5, 4, 4, 5, 6, 6, 5},
				New Double() {5, 4, 4, 5, 6, 6, 5}
			})
			Dim err As String = OpValidation.validate((New OpTestCase(op)).expectedOutput(0, exp))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMirrorPadSymmetric(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMirrorPadSymmetric(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 4)
			Dim pad As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 1},
				New Double() {1, 1}
			}).castTo(DataType.INT)

			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 5, 6)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("mirror_pad").addInputs([in], pad).addOutputs([out]).addIntegerArguments(1).build()

			Nd4j.Executioner.exec(op)

			Dim exp As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 1, 2, 3, 4, 4},
				New Double() {1, 1, 2, 3, 4, 4},
				New Double() {5, 5, 6, 7, 8, 8},
				New Double() {9, 9, 10, 11, 12, 12},
				New Double() {9, 9, 10, 11, 12, 12}
			})
			Dim err As String = OpValidation.validate((New OpTestCase(op)).expectedOutput(0, exp))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnique(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnique(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.create(New Double(){3, 4, 3, 1, 3, 0, 2, 4, 2, 4})

			Dim expUnique As INDArray = Nd4j.create(New Double(){3, 4, 1, 0, 2})
			Dim expUniqueIdxs As INDArray = Nd4j.create(New Double(){0, 1, 0, 2, 0, 3, 4, 1, 4, 1}).castTo(DataType.LONG)

			Dim outUnique As INDArray = Nd4j.create(DataType.DOUBLE, expUnique.shape())
			Dim outUniqueIdxs As INDArray = Nd4j.create(DataType.LONG, expUniqueIdxs.shape())

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("unique").addInputs([in]).addOutputs(outUnique, outUniqueIdxs).build()

			Dim err As String = OpValidation.validate((New OpTestCase(op)).expectedOutput(0, expUnique).expectedOutput(1, expUniqueIdxs))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTopK(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTopK(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'Can't assume sorted here
			Dim [in] As INDArray = Nd4j.create(New Double(){7, 3, 1, 2, 5, 0, 4, 6, 9, 8})

			Dim expTopK As INDArray = Nd4j.create(New Double(){7, 5, 6, 9, 8})
			Dim expIndices As INDArray = Nd4j.create(New Double(){0, 4, 7, 8, 9})

			Dim expTopK_sorted As INDArray = Nd4j.create(New Double(){9, 8, 7, 6, 5})
			Dim expIndices_sorted As INDArray = Nd4j.create(New Double(){8, 9, 0, 7, 4})

			For Each sort As Boolean In New Boolean(){False, True}
				Dim outUnique As INDArray = Nd4j.create(expTopK.shape())
				Dim outUniqueIdxs As INDArray = Nd4j.create(expIndices.shape())

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("top_k").addInputs([in]).addOutputs(outUnique, outUniqueIdxs).addIntegerArguments(5,If(sort, 1, 0)).build()

				Dim err As String = OpValidation.validate((New OpTestCase(op)).expectedOutput(0,If(sort, expTopK_sorted, expTopK)).expectedOutput(1,If(sort, expIndices_sorted, expIndices)))

				assertNull(err)
			Next sort
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTopK1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTopK1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(0.0, 0.0, 0.0, 10.0, 0.0)
			Dim k As INDArray = Nd4j.scalar(1)
			Dim outValue As INDArray = Nd4j.create(DataType.DOUBLE, 1)
			Dim outIdx As INDArray = Nd4j.create(DataType.INT, 1)

			Nd4j.exec(DynamicCustomOp.builder("top_k").addInputs(x, k).addOutputs(outValue, outIdx).addBooleanArguments(False).addIntegerArguments(1).build())

			Dim expValue As INDArray = Nd4j.createFromArray(10.0)
			Dim expIdx As INDArray = Nd4j.createFromArray(3)

			assertEquals(expValue, outValue)
			assertEquals(expIdx, outIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInTopK(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInTopK(ByVal backend As Nd4jBackend)
			For k As Integer = 4 To 1 Step -1
				log.info("Testing: k=" & k)
				Dim [in] As INDArray = Nd4j.linspace(1, 20, 20, DataType.DOUBLE).reshape(ChrW(4), 5)
				Dim idxs As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}).castTo(DataType.INT)

				Dim expOut As INDArray
				Select Case k
					Case 4
						expOut = Nd4j.create(New Boolean(){True, True, True, True})
					Case 3
						expOut = Nd4j.create(New Boolean(){False, True, True, True})
					Case 2
						expOut = Nd4j.create(New Boolean(){False, False, True, True})
					Case 1
						expOut = Nd4j.create(New Boolean(){False, False, False, True})
					Case Else
						Throw New Exception()
				End Select


				Dim [out] As INDArray = Nd4j.create(DataType.BOOL, expOut.shape())

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("in_top_k").addInputs([in], idxs).addOutputs([out]).addIntegerArguments(k).build()

				Dim err As String = OpValidation.validate((New OpTestCase(op)).expectedOutput(0, expOut))

				assertNull(err)
			Next k
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZeta(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZeta(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'https://github.com/eclipse/deeplearning4j/issues/6182
			Dim x As INDArray = Nd4j.rand(3, 4).addi(1.0)
			Dim q As INDArray = Nd4j.rand(3, 4)

			Dim [out] As INDArray = Nd4j.create(3, 4)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("zeta").addInputs(x, q).addOutputs([out]).build()

			Nd4j.Executioner.exec(op)

			assertNotEquals(Nd4j.create([out].shape()), [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxEmptyScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxEmptyScalar(ByVal backend As Nd4jBackend)
			Dim empty As INDArray = Nd4j.empty(DataType.FLOAT)
			Dim scalar As INDArray = Nd4j.scalar(1.0f)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("maximum").addInputs(empty, scalar).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			Dim shape() As Long = l(0).getShape()
			Dim isEmpty As Boolean = l(0).isEmpty()

			assertTrue(isEmpty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastEmpty(ByVal backend As Nd4jBackend)
	'        Nd4j.getExecutioner().enableVerboseMode(true);
	'        Nd4j.getExecutioner().enableDebugMode(true);
			'Check broadcast behaviour with empty arrays. The idea is to match TF import behaviour, for import
			'TF behaviour: broadcastableOp(x,empty) -> empty

	'        
	'        tf.reset_default_graph()
	'        # Hack to create empty array
	'        input = tf.constant([False], dtype=tf.bool)
	'        empty = tf.where(condition=input)
	'        emptyFloat = tf.cast(empty, tf.float32)
	'        emptyFloat = tf.reshape(emptyFloat, [0,1])
	'        constScalar = tf.constant(1, dtype=tf.float32)
	'        # out = tf.math.maximum(emptyFloat,constScalar)
	'        # out = emptyFloat + constScalar
	'        # out = emptyFloat / constScalar
	'        out = tf.math.less(emptyFloat, constScalar)
	'        sess = tf.Session()
	'        out = sess.run([out])
	'         

			For i As Integer = 0 To 2
				For Each scalar As Boolean In New Boolean(){True, False}
					Dim x As INDArray = If(scalar, Nd4j.scalar(2f), Nd4j.create(DataType.FLOAT, 3, 4))
					Dim y As INDArray = If(scalar, Nd4j.scalar(3f), Nd4j.create(DataType.FLOAT, 3, 4))
					Select Case i
						Case 0
							'x only empty
							x = Nd4j.empty(DataType.FLOAT)
						Case 1
							'y only empty
							y = Nd4j.empty(DataType.FLOAT)
						Case 2
							'Both empty
							x = Nd4j.empty(DataType.FLOAT)
							y = Nd4j.empty(DataType.FLOAT)
						Case Else
							Throw New Exception()
					End Select


					For Each opName As String In New String(){"maximum", "minimum", "add", "subtract", "multiply", "divide", "assign", "boolean_and", "boolean_or", "boolean_xor", "tf_atan2", "equals", "floordiv", "floormod", "greater", "greater_equal", "less", "less_equal", "mod", "not_equals", "realdiv", "reversedivide", "reversesubtract", "squaredsubtract", "truncatediv"}

	'                    log.info("Starting op: {}, case {} - x.isScalar()={}, x.isEmpty()={}, y.isScalar()={}, y.isEmpty()={}", opName, i,
	'                            x.isScalar(), x.isEmpty(), y.isScalar(), y.isEmpty());

						Dim op As DynamicCustomOp = DynamicCustomOp.builder(opName).addInputs(x, y).build()

						Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
						assertEquals(1, l.Count)
						Dim shape() As Long = l(0).getShape()
						Dim empty As Boolean = l(0).isEmpty()

						Dim isBool As Boolean = isBoolBroadcast(opName)
						If isBool Then
							assertEquals(DataType.BOOL, l(0).dataType())
						Else
							assertEquals(DataType.FLOAT, l(0).dataType())
						End If

						assertArrayEquals(New Long(){}, shape)
						assertTrue(empty)


						Dim [out] As INDArray = Nd4j.empty(If(isBool, DataType.BOOL, DataType.FLOAT))
						op.addOutputArgument([out])

						Nd4j.exec(op)
					Next opName
				Next scalar
			Next i
		End Sub

		Private Shared Function isBoolBroadcast(ByVal opName As String) As Boolean
			If opName.StartsWith("greater", StringComparison.Ordinal) OrElse opName.StartsWith("less", StringComparison.Ordinal) OrElse opName.Contains("equals") Then
				Return True
			End If
			'Note that "boolean" ops are inherit
			Return False
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStandardize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStandardize(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4});
			Dim random As INDArray = Nd4j.rand(New Integer(){10, 4})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1};
			Dim axis() As Integer = {1}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray means = random.mean(axis);
			Dim means As INDArray = random.mean(axis)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray std = random.std(false, axis);
			Dim std As INDArray = random.std(False, axis)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = random.subColumnVector(means).divColumnVector(std);
			Dim res As INDArray = random.subColumnVector(means).divColumnVector(std)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expOut = res.norm1();
			Dim expOut As INDArray = res.norm1()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdA As SDVariable = sd.var("a", random)
			Dim t As SDVariable = sd.math_Conflict.standardize(sdA, axis)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStandardizeOP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStandardizeOP(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4});
			Dim random As INDArray = Nd4j.rand(New Integer(){10, 4})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1};
			Dim axis() As Integer = {1}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray means = random.mean(axis);
			Dim means As INDArray = random.mean(axis)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray std = random.std(false, axis);
			Dim std As INDArray = random.std(False, axis)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = random.subColumnVector(means).divColumnVector(std);
			Dim res As INDArray = random.subColumnVector(means).divColumnVector(std)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray output = org.nd4j.linalg.factory.Nd4j.zerosLike(res);
			Dim output As INDArray = Nd4j.zerosLike(res)
			Nd4j.Executioner.exec(New Standardize(random, output, 1))

			assertEquals(res, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStandardizeNoDeviation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStandardizeNoDeviation(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(new int[]{10, 4});
			Dim random As INDArray = Nd4j.rand(New Integer(){10, 4})
			For i As Integer = 0 To 3
				random.putScalar(1, i, 7)
			Next i

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1};
			Dim axis() As Integer = {1}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray means = random.mean(axis);
			Dim means As INDArray = random.mean(axis)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray std = random.std(false, axis);
			Dim std As INDArray = random.std(False, axis)
			std.addi(std.eq(0).castTo(DataType.DOUBLE))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = random.subColumnVector(means).divColumnVector(std);
			Dim res As INDArray = random.subColumnVector(means).divColumnVector(std)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expOut = res.norm1();
			Dim expOut As INDArray = res.norm1()

			Dim sd As SameDiff = SameDiff.create()
			Dim sdA As SDVariable = sd.var("a", random)
			Dim t As SDVariable = sd.math_Conflict.standardize(sdA, axis)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatMulTensor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatMulTensor(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(new int[]{1, 2, 3, 4, 5});
			Dim a As INDArray = Nd4j.rand(New Integer(){1, 2, 3, 4, 5})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(new int[]{1, 2, 3, 5, 6});
			Dim b As INDArray = Nd4j.rand(New Integer(){1, 2, 3, 5, 6})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray z = org.nd4j.linalg.factory.Nd4j.matmul(a, b);
			Dim z As INDArray = Nd4j.matmul(a, b)

			assertArrayEquals(z.shape(), New Long(){1, 2, 3, 4, 6})

			Dim sd As SameDiff = SameDiff.create()
			Dim sdA As SDVariable = sd.var("a", a)
			Dim sdB As SDVariable = sd.var("b", b)
			Dim t As SDVariable = sd.mmul(sdA, sdB)
			t.norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatMulTensorTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatMulTensorTranspose(ByVal backend As Nd4jBackend)
			For Each transposeA As Boolean In New Boolean(){False, True}
				For Each transposeB As Boolean In New Boolean(){False, True}
					For Each transposeResult As Boolean In New Boolean(){False, True}
						log.info("Testing with transposeA={}; transposeB={}; transposeResult={};", transposeA, transposeB, transposeResult)
						Dim m As Integer = 0, n As Integer = 0, k As Integer = 0, l As Integer = 0, i As Integer = 0, j As Integer = 0
						If Not transposeA AndAlso Not transposeB AndAlso Not transposeResult Then
							m = 4
							n = 5
							k = 5
							l = 6
							i = 4
							j = 6
						End If
						If Not transposeA AndAlso transposeB AndAlso Not transposeResult Then
							m = 4
							n = 5
							k = 6
							l = 5
							i = 4
							j = 6
						End If
						If Not transposeA AndAlso Not transposeB AndAlso transposeResult Then
							m = 4
							n = 5
							k = 5
							l = 6
							i = 6
							j = 4
						End If
						If Not transposeA AndAlso transposeB AndAlso transposeResult Then
							m = 4
							n = 5
							k = 6
							l = 5
							i = 6
							j = 4
						End If
						If transposeA AndAlso Not transposeB AndAlso Not transposeResult Then
							m = 5
							n = 4
							k = 5
							l = 6
							i = 4
							j = 6
						End If
						If transposeA AndAlso transposeB AndAlso Not transposeResult Then
							m = 5
							n = 4
							k = 6
							l = 5
							i = 4
							j = 6
						End If
						If transposeA AndAlso Not transposeB AndAlso transposeResult Then
							m = 5
							n = 4
							k = 5
							l = 6
							i = 6
							j = 4
						End If
						If transposeA AndAlso transposeB AndAlso transposeResult Then
							m = 5
							n = 4
							k = 6
							l = 5
							i = 6
							j = 4
						End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(new int[]{1, 2, 3, m, n});
						Dim a As INDArray = Nd4j.rand(New Integer(){1, 2, 3, m, n})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(new int[]{1, 2, 3, k, l});
						Dim b As INDArray = Nd4j.rand(New Integer(){1, 2, 3, k, l})

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray z = org.nd4j.linalg.factory.Nd4j.matmul(a, b, transposeA, transposeB, transposeResult);
						Dim z As INDArray = Nd4j.matmul(a, b, transposeA, transposeB, transposeResult)

						assertArrayEquals(z.shape(), New Long(){1, 2, 3, i, j})

						Dim sd As SameDiff = SameDiff.create()
						Dim sdA As SDVariable = sd.var("a", a)
						Dim sdB As SDVariable = sd.var("b", b)
						Dim t As SDVariable = sd.mmul(sdA, sdB, transposeA, transposeB, transposeResult)
						t.norm1("out")

						Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
						assertNull(err, err)
					Next transposeResult
				Next transposeB
			Next transposeA
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxCF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxCF(ByVal backend As Nd4jBackend)

			Dim arrC As INDArray = Nd4j.rand(DataType.FLOAT, 2, 5)
			Dim arrF As INDArray = arrC.dup("f"c)
			Dim outCC As INDArray = Nd4j.create(DataType.FLOAT, arrC.shape(), "c"c)
			Dim outCF As INDArray = Nd4j.create(DataType.FLOAT, arrC.shape(), "f"c)
			Dim outFC As INDArray = Nd4j.create(DataType.FLOAT, arrC.shape(), "c"c)
			Dim outFF As INDArray = Nd4j.create(DataType.FLOAT, arrC.shape(), "f"c)


			Nd4j.exec(DynamicCustomOp.builder("softmax").addInputs(arrC).addOutputs(outCC).build())
			Nd4j.exec(DynamicCustomOp.builder("softmax").addInputs(arrC).addOutputs(outCF).build())
			Nd4j.exec(DynamicCustomOp.builder("softmax").addInputs(arrF).addOutputs(outFC).build())
			Nd4j.exec(DynamicCustomOp.builder("softmax").addInputs(arrF).addOutputs(outFF).build())

			assertEquals(outCC, outCF)
			assertEquals(outCC, outFC)
			assertEquals(outCC, outFF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogSumExp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogSumExp(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim inputArr As INDArray = Nd4j.rand(DataType.FLOAT, 1, 4)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var(inputArr)
			Dim lse As SDVariable = sd.math().logSumExp([in])
			Dim [out] As INDArray = lse.eval()

			Dim exp As INDArray = Transforms.exp(inputArr, True)
			Dim sum As INDArray = exp.sum()
			Dim log As INDArray = Transforms.log(sum)
			assertEquals(log, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogSumExp2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogSumExp2(ByVal backend As Nd4jBackend)

			For [dim] As Integer = 0 To 2
				Nd4j.Random.setSeed(12345)
				Dim inputArr As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 4, 5)
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var(inputArr)
				Dim lse As SDVariable = sd.math().logSumExp([in], [dim])

				Dim exp As INDArray = Transforms.exp(inputArr, True)
				Dim sum As INDArray = exp.sum([dim])
				Dim log As INDArray = Transforms.log(sum)

				OpValidation.validate((New TestCase(sd)).expectedOutput(lse.name(), log).gradientCheck(True))
			Next [dim]
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCRELU(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCRELU(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim inputArr As INDArray = Nd4j.rand(DataType.DOUBLE, 2, 2)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var(inputArr)

			Dim crelu As SDVariable = (New CReLU(sd, [in])).outputVariable()
			Dim expected As INDArray = Nd4j.concat(1, Nd4j.nn_Conflict.relu(inputArr, 0), Nd4j.nn_Conflict.relu(inputArr.neg(), 0))

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("crelu", expected).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClipByAvgNorm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testClipByAvgNorm(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim inputArr As INDArray = Nd4j.rand(DataType.DOUBLE, 2, 2, 2)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var(inputArr)
			Dim [out] As SDVariable = (New ClipByAvgNorm(sd, [in], 1e-2, 0, 1, 2)).outputVariable()
			Dim expected As SDVariable = sd.math_Conflict.clipByNorm([in], 1e-2, 0, 1, 2).mul(inputArr.length())

			Dim loss As SDVariable = sd.standardDeviation("loss", [out], True)
			loss.markAsLoss()

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("clipbyavgnorm", expected.eval()).gradientCheck(False))
			assertNull(err)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmbeddingLookup(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmbeddingLookup(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim input As SDVariable = sd.var("in", Nd4j.rand(1024, 10))
			Dim indices As SDVariable = sd.constant("indices", Nd4j.createFromArray(New Long(){0, 5, 17, 33}))
			Dim [out] As SDVariable = (New EmbeddingLookup(sd, input, indices, PartitionMode.MOD)).outputVariable()
			' should be matrix of shape [4, 10]
			assertArrayEquals(New Long(){4, 10}, [out].eval().shape())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testImageResize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testImageResize(ByVal backend As Nd4jBackend)

			'TODO: Methods failed ResizeLanczos5, ResizeMitchelcubic, ResizeArea

			For Each method As ImageResizeMethod In System.Enum.GetValues(GetType(ImageResizeMethod))
				If method=ImageResizeMethod.ResizeLanczos5 OrElse method=ImageResizeMethod.ResizeArea OrElse method = ImageResizeMethod.ResizeMitchelcubic Then
					Continue For
				End If

				log.info("Trying {}", method)

				Nd4j.Random.setSeed(12345)
				Dim sd As SameDiff = SameDiff.create()
				Dim preserveAspectRatio As Boolean = True
				Dim antialias As Boolean = True
				Dim inputImage As SDVariable = sd.var(Nd4j.rand(DataType.FLOAT, 1, 5, 5, 3))
				'  NHWC format
				Dim expectedShape() As Long = {1, 3, 3, 3}
				Dim requestedSize As SDVariable = sd.constant(Nd4j.createFromArray(New Long(){3, 3}))

				Dim checkFunction As [Function](Of INDArray, String) = Function([in])
				Dim shapeOk As Boolean = expectedShape.SequenceEqual([in].shape())
				If shapeOk Then
					Return Nothing
				End If
				Return "Failed: shape differs - expected " & java.util.Arrays.toString(expectedShape) & " vs " & java.util.Arrays.toString([in].shape()) & " on method " & method
				End Function


				Dim [out] As SDVariable = (New ImageResize(sd, inputImage, requestedSize, preserveAspectRatio, antialias, method)).outputVariable().std(True)

				Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(False).expected("image_resize", checkFunction))

				assertNull(err)


			Next method
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaximumBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaximumBp(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim inputX As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputY As SDVariable = sd.var(Nd4j.rand(2, 3))


			Dim [out] As SDVariable = (New Max(sd, inputX, inputY)).outputVariable().std(True)
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeAddBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeAddBp(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim inputX As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputY As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputZ As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim [out] As SDVariable = (New MergeAddOp(sd, New SDVariable(){inputX, inputY, inputZ})).outputVariable().std(True)
			[out].markAsLoss()
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMaxBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMaxBp(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim inputX As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputY As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputZ As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim [out] As SDVariable = (New MergeMax(sd, inputX, inputY, inputZ)).outputVariable().std(True)
			[out].markAsLoss()
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeAvgBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeAvgBp(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim inputX As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputY As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim inputZ As SDVariable = sd.var(Nd4j.rand(2, 3))
			Dim [out] As SDVariable = (New MergeAvg(sd, inputX, inputY, inputZ)).outputVariable().std(True)
			[out].markAsLoss()
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseBp(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim input As SDVariable = sd.var(Nd4j.createFromArray(New Double()(){
				New Double() {2, 7},
				New Double() {3, 5},
				New Double() {4, 5}
			}))
			Dim [out] As SDVariable = (New Reverse(sd, input,0)).outputVariable()
			Dim loss As SDVariable = [out].std(True)
			loss.markAsLoss()
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUpsampling3dBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUpsampling3dBp(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			For Each dataformat As Boolean In New Boolean(){True, False}

				Dim sd As SameDiff = SameDiff.create()

				' NCDHW input
				Dim input As SDVariable = If(dataformat, sd.var(Nd4j.rand(DataType.DOUBLE, 2, 1, 5, 5, 5)), sd.var(Nd4j.rand(DataType.DOUBLE, 2, 5, 5, 5, 1)))
				Dim scaleD As Integer = 2
				Dim scaleH As Integer = 2
				Dim scaleW As Integer = 2
				Dim [out] As SDVariable = (New Upsampling3d(sd, input, True, scaleD, scaleH, scaleW)).outputVariable().std(True)
				[out].markAsLoss()
				Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
				assertNull(err)
			Next dataformat
		End Sub
	End Class

End Namespace