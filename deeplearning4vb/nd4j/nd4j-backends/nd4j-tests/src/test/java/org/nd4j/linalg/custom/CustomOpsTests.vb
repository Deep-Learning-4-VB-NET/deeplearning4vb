Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports AdjustContrast = org.nd4j.linalg.api.ops.custom.AdjustContrast
Imports AdjustHue = org.nd4j.linalg.api.ops.custom.AdjustHue
Imports AdjustSaturation = org.nd4j.linalg.api.ops.custom.AdjustSaturation
Imports BetaInc = org.nd4j.linalg.api.ops.custom.BetaInc
Imports BitCast = org.nd4j.linalg.api.ops.custom.BitCast
Imports DivideNoNan = org.nd4j.linalg.api.ops.custom.DivideNoNan
Imports DrawBoundingBoxes = org.nd4j.linalg.api.ops.custom.DrawBoundingBoxes
Imports FakeQuantWithMinMaxVarsPerChannel = org.nd4j.linalg.api.ops.custom.FakeQuantWithMinMaxVarsPerChannel
Imports Flatten = org.nd4j.linalg.api.ops.custom.Flatten
Imports FusedBatchNorm = org.nd4j.linalg.api.ops.custom.FusedBatchNorm
Imports HsvToRgb = org.nd4j.linalg.api.ops.custom.HsvToRgb
Imports KnnMinDistance = org.nd4j.linalg.api.ops.custom.KnnMinDistance
Imports Lgamma = org.nd4j.linalg.api.ops.custom.Lgamma
Imports LinearSolve = org.nd4j.linalg.api.ops.custom.LinearSolve
Imports Logdet = org.nd4j.linalg.api.ops.custom.Logdet
Imports Lstsq = org.nd4j.linalg.api.ops.custom.Lstsq
Imports Lu = org.nd4j.linalg.api.ops.custom.Lu
Imports MatrixBandPart = org.nd4j.linalg.api.ops.custom.MatrixBandPart
Imports Polygamma = org.nd4j.linalg.api.ops.custom.Polygamma
Imports RandomCrop = org.nd4j.linalg.api.ops.custom.RandomCrop
Imports RgbToGrayscale = org.nd4j.linalg.api.ops.custom.RgbToGrayscale
Imports RgbToHsv = org.nd4j.linalg.api.ops.custom.RgbToHsv
Imports RgbToYiq = org.nd4j.linalg.api.ops.custom.RgbToYiq
Imports RgbToYuv = org.nd4j.linalg.api.ops.custom.RgbToYuv
Imports Roll = org.nd4j.linalg.api.ops.custom.Roll
Imports ToggleBits = org.nd4j.linalg.api.ops.custom.ToggleBits
Imports TriangularSolve = org.nd4j.linalg.api.ops.custom.TriangularSolve
Imports YiqToRgb = org.nd4j.linalg.api.ops.custom.YiqToRgb
Imports YuvToRgb = org.nd4j.linalg.api.ops.custom.YuvToRgb
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports OpStatus = org.nd4j.linalg.api.ops.executioner.OpStatus
Imports Where = org.nd4j.linalg.api.ops.impl.controlflow.Where
Imports NonMaxSuppression = org.nd4j.linalg.api.ops.impl.image.NonMaxSuppression
Imports ResizeArea = org.nd4j.linalg.api.ops.impl.image.ResizeArea
Imports ResizeBilinear = org.nd4j.linalg.api.ops.impl.image.ResizeBilinear
Imports MmulBp = org.nd4j.linalg.api.ops.impl.reduce.MmulBp
Imports Create = org.nd4j.linalg.api.ops.impl.shape.Create
Imports Linspace = org.nd4j.linalg.api.ops.impl.shape.Linspace
Imports OnesLike = org.nd4j.linalg.api.ops.impl.shape.OnesLike
Imports SequenceMask = org.nd4j.linalg.api.ops.impl.shape.SequenceMask
Imports Cholesky = org.nd4j.linalg.api.ops.impl.transforms.Cholesky
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports Qr = org.nd4j.linalg.api.ops.impl.transforms.custom.Qr
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports ModOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.ModOp
Imports RandomStandardNormal = org.nd4j.linalg.api.ops.random.compat.RandomStandardNormal
Imports DropOut = org.nd4j.linalg.api.ops.random.impl.DropOut
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static Float.NaN
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

Namespace org.nd4j.linalg.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class CustomOpsTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CustomOpsTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonInplaceOp1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonInplaceOp1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)
			Dim arrayY As val = Nd4j.create(10, 10)
			Dim arrayZ As val = Nd4j.create(10, 10)

			arrayX.assign(3.0)
			arrayY.assign(1.0)

			Dim exp As val = Nd4j.create(10,10).assign(4.0)

			Dim op As CustomOp = DynamicCustomOp.builder("add").addInputs(arrayX, arrayY).addOutputs(arrayZ).build()

			Nd4j.Executioner.exec(op)

			assertEquals(exp, arrayZ)
		End Sub

		''' <summary>
		''' This test works inplace, but without inplace declaration
		''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonInplaceOp2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonInplaceOp2(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)
			Dim arrayY As val = Nd4j.create(10, 10)

			arrayX.assign(3.0)
			arrayY.assign(1.0)

			Dim exp As val = Nd4j.create(10,10).assign(4.0)

			Dim op As CustomOp = DynamicCustomOp.builder("add").addInputs(arrayX, arrayY).addOutputs(arrayX).build()

			Nd4j.Executioner.exec(op)

			assertEquals(exp, arrayX)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoOp1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoOp1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)
			Dim arrayY As val = Nd4j.create(5, 3)

			arrayX.assign(3.0)
			arrayY.assign(1.0)

			Dim expX As val = Nd4j.create(10,10).assign(3.0)
			Dim expY As val = Nd4j.create(5,3).assign(1.0)

			Dim op As CustomOp = DynamicCustomOp.builder("noop").addInputs(arrayX, arrayY).addOutputs(arrayX, arrayY).build()

			Nd4j.Executioner.exec(op)

			assertEquals(expX, arrayX)
			assertEquals(expY, arrayY)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFloor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFloor(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)

			arrayX.assign(3.0)

			Dim exp As val = Nd4j.create(10,10).assign(3.0)

			Dim op As CustomOp = DynamicCustomOp.builder("floor").addInputs(arrayX).addOutputs(arrayX).build()

			Nd4j.Executioner.exec(op)

			assertEquals(exp, arrayX)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInplaceOp1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInplaceOp1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim arrayX As val = Nd4j.create(10, 10)
			Dim arrayY As val = Nd4j.create(10, 10)
			arrayX.assign(4.0)
			arrayY.assign(2.0)
			Dim exp As val = Nd4j.create(10,10).assign(6.0)
			Dim op As CustomOp = DynamicCustomOp.builder("add").addInputs(arrayX, arrayY).callInplace(True).build()
			Nd4j.Executioner.exec(op)
			assertEquals(exp, arrayX)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoneInplaceOp3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoneInplaceOp3(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(10, 10)
			Dim arrayY As val = Nd4j.create(10, 10)

			arrayX.assign(4.0)
			arrayY.assign(2.0)

			Dim exp As val = Nd4j.create(10,10).assign(6.0)

			Dim op As CustomOp = DynamicCustomOp.builder("add").addInputs(arrayX, arrayY).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			assertEquals(exp, op.getOutputArgument(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoneInplaceOp4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoneInplaceOp4(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(DataType.INT, 10, 10)
			Dim arrayY As val = Nd4j.create(DataType.INT, 10, 10)

			arrayX.assign(4)
			arrayY.assign(2)

			Dim exp As val = Nd4j.create(DataType.INT,10, 10).assign(6)

			Dim op As CustomOp = DynamicCustomOp.builder("add").addInputs(arrayX, arrayY).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			Dim res As val = op.getOutputArgument(0)
			assertEquals(DataType.INT, res.dataType())
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoneInplaceOp5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoneInplaceOp5(ByVal backend As Nd4jBackend)
			If Not Nd4j.ExperimentalMode Then
				Return
			End If

			Dim arrayX As val = Nd4j.create(DataType.INT, 10, 10)
			Dim arrayY As val = Nd4j.create(DataType.FLOAT, 10, 10)

			arrayX.assign(4)
			arrayY.assign(2.0)

			Dim exp As val = Nd4j.create(DataType.FLOAT,10, 10).assign(6)

			Dim op As CustomOp = DynamicCustomOp.builder("add").addInputs(arrayX, arrayY).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			Dim res As val = op.getOutputArgument(0)
			assertEquals(DataType.FLOAT, res.dataType())
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMax1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMax1(ByVal backend As Nd4jBackend)
			Dim array0 As val = Nd4j.create(New Double() {1, 0, 0, 0, 0})
			Dim array1 As val = Nd4j.create(New Double() {0, 2, 0, 0, 0})
			Dim array2 As val = Nd4j.create(New Double() {0, 0, 3, 0, 0})
			Dim array3 As val = Nd4j.create(New Double() {0, 0, 0, 4, 0})
			Dim array4 As val = Nd4j.create(New Double() {0, 0, 0, 0, 5})

			Dim z As val = Nd4j.create(DataType.DOUBLE, 5)
			Dim exp As val = Nd4j.create(New Double(){1, 2, 3, 4, 5})

			Dim op As CustomOp = DynamicCustomOp.builder("mergemax").addInputs(array0, array1, array2, array3, array4).addOutputs(z).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			assertEquals(exp, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMaxF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMaxF(ByVal backend As Nd4jBackend)

			Dim array0 As val = Nd4j.rand("f"c, 5, 2).add(1) 'some random array with +ve numbers
			Dim array1 As val = array0.dup("f"c).add(5)
			array1.put(0, 0, 0) 'array1 is always bigger than array0 except at 0,0

			'expected value of maxmerge
			Dim exp As val = array1.dup("f"c)
			exp.putScalar(0, 0, array0.getDouble(0, 0))

			Dim zF As val = Nd4j.zeros(array0.shape(), "f"c)
			Dim op As CustomOp = DynamicCustomOp.builder("mergemax").addInputs(array0, array1).addOutputs(zF).build()
			Nd4j.Executioner.exec(op)

			assertEquals(exp, zF)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMaxMixedOrder_Subtract(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMaxMixedOrder_Subtract(ByVal backend As Nd4jBackend)
			Dim exp As val = Nd4j.create(New Integer() {2, 2}, "c"c).assign(5.0)
			Nd4j.Executioner.commit()

			Dim array0 As val = Nd4j.create(New Integer() {2, 2}, "f"c) 'some random array with +ve numbers
			Dim array1 As val = array0.dup("c"c).addi(5.0)

			Nd4j.Executioner.commit()

			assertEquals(exp, array1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMaxSameOrder_Subtract(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMaxSameOrder_Subtract(ByVal backend As Nd4jBackend)
			Dim exp As val = Nd4j.create(New Integer() {2, 2}, "c"c).assign(5.0)
			Nd4j.Executioner.commit()

			Dim array0 As val = Nd4j.create(New Integer() {2, 2}, "c"c) 'some random array with +ve numbers
			Dim array1 As val = array0.dup("c"c).addi(5)

			assertEquals(exp, array1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeMaxMixedOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeMaxMixedOrder(ByVal backend As Nd4jBackend)
			Dim array0 As val = Nd4j.rand("f"c, 5, 2).addi(1) 'some random array with +ve numbers
			Dim array1 As val = array0.dup("c"c).addi(5)
			array1.put(0, 0, 0) 'array1 is always bigger than array0 except at 0,0

			'expected value of maxmerge
			Dim exp As val = array1.dup()
			exp.putScalar(0, 0, array0.getDouble(0, 0))

			Dim zF As val = Nd4j.zeros(array0.shape(),"f"c)
			Dim op As CustomOp = DynamicCustomOp.builder("mergemax").addInputs(array0, array1).addOutputs(zF).callInplace(False).build()
			Nd4j.Executioner.exec(op)

			assertEquals(exp, zF)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOutputShapes1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOutputShapes1(ByVal backend As Nd4jBackend)
			Dim array0 As val = Nd4j.rand("f"c, 5, 2).addi(1) 'some random array with +ve numbers
			Dim array1 As val = array0.dup().addi(5)
			array1.put(0, 0, 0) 'array1 is always bigger than array0 except at 0,0

			'expected value of maxmerge
			Dim exp As val = array1.dup()
			exp.putScalar(0, 0, array0.getDouble(0, 0))

			Dim op As CustomOp = DynamicCustomOp.builder("mergemax").addInputs(array0, array1).build()

			Dim shapes As val = Nd4j.Executioner.calculateOutputShape(op)

			assertEquals(1, shapes.size())
			assertArrayEquals(New Long(){5, 2}, shapes.get(0).getShape())
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpStatus1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOpStatus1(ByVal backend As Nd4jBackend)
			assertEquals(OpStatus.ND4J_STATUS_OK, OpStatus.byNumber(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomStandardNormal_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomStandardNormal_1(ByVal backend As Nd4jBackend)
			If Nd4j.Executioner.type() = OpExecutioner.ExecutionerType.CUDA Then
				Return
			End If

			Dim shape As val = Nd4j.create(New Single() {5, 10})
			Dim op As val = New RandomStandardNormal(shape)

			Nd4j.Executioner.exec(op)

			assertEquals(1, op.outputArguments().size())
			Dim output As val = op.getOutputArgument(0)

			assertArrayEquals(New Long(){5, 10}, output.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomStandardNormal_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomStandardNormal_2(ByVal backend As Nd4jBackend)
			If Nd4j.Executioner.type() = OpExecutioner.ExecutionerType.CUDA Then
				Return
			End If

			Dim shape As val = New Long(){5, 10}
			Dim op As val = New RandomStandardNormal(shape)

			Nd4j.Executioner.exec(op)

			assertEquals(1, op.outputArguments().size())
			Dim output As val = op.getOutputArgument(0)

			assertArrayEquals(New Long(){5, 10}, output.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpContextExecution_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOpContextExecution_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
			Dim arrayY As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
			Dim arrayZ As val = Nd4j.create(DataType.FLOAT, 5)

			Dim exp As val = Nd4j.createFromArray(New Single(){2, 4, 6, 8, 10})

			Dim context As val = Nd4j.Executioner.buildContext()
			context.setInputArray(0, arrayX)
			context.setInputArray(1, arrayY)
			context.setOutputArray(0, arrayZ)

			Dim addOp As val = New AddOp()
			NativeOpsHolder.Instance.getDeviceNativeOps().execCustomOp2(Nothing, addOp.opHash(), context.contextPointer())

			assertEquals(exp, arrayZ)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpContextExecution_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOpContextExecution_2(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
			Dim arrayY As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
			Dim arrayZ As val = Nd4j.create(DataType.FLOAT, 5)

			Dim exp As val = Nd4j.createFromArray(New Single(){2, 4, 6, 8, 10})

			Dim context As val = Nd4j.Executioner.buildContext()
			context.setInputArray(0, arrayX)
			context.setInputArray(1, arrayY)
			context.setOutputArray(0, arrayZ)

			Dim addOp As val = New AddOp()
			Dim output As val = Nd4j.exec(addOp, context)

			assertEquals(exp, arrayZ)
			assertTrue(arrayZ = output(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpContextExecution_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOpContextExecution_3(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(100)
			Dim arrayY As val = Nd4j.ones(100)
			Dim arrayZ As val = Nd4j.create(100)

			Dim exp As val = Nd4j.ones(100)

			Dim context As val = Nd4j.Executioner.buildContext()
			context.setInputArray(0, arrayX)
			context.setInputArray(1, arrayY)

			context.setOutputArray(0, arrayZ)

			Dim addOp As val = New AddOp()
			Dim output As val = Nd4j.exec(addOp, context)

			assertEquals(exp, arrayZ)
			assertTrue(arrayZ = output(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlatten_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlatten_1(ByVal backend As Nd4jBackend)
			Dim arrayA As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f)
			Dim arrayB As val = Nd4j.createFromArray(4.0f, 5.0f, 6.0f)
			Dim arrayC As val = Nd4j.createFromArray(7.0f, 8.0f, 9.0f)

			Dim exp As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f)

			Dim result As val = Nd4j.exec(New Flatten("c"c, arrayA, arrayB, arrayC))(0)

			assertEquals(exp, result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatmulBp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatmulBp(ByVal backend As Nd4jBackend)
			Dim a As val = Nd4j.create(DataType.DOUBLE, 1,3)
			Dim b As val = Nd4j.create(DataType.DOUBLE, 1,4)
			Dim gI As val = Nd4j.create(DataType.DOUBLE, 3,4)

			Dim gA As val = Nd4j.create(DataType.DOUBLE, 1,3)
			Dim gB As val = Nd4j.create(DataType.DOUBLE, 1,4)

			Dim mt As val = MMulTranspose.builder().transposeA(True).transposeB(False).transposeResult(False).build()

			Dim op As val = New MmulBp(a, b, gI, gA, gB, mt)
			Nd4j.exec(op)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSliceEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSliceEdgeCase(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.scalar(10.0).reshape(ChrW(1)) 'Int [1]
			Dim begin As INDArray = Nd4j.ones(DataType.INT, 1)
			Dim [end] As INDArray = Nd4j.zeros(DataType.INT, 1)
			Dim stride As INDArray = Nd4j.ones(DataType.INT, 1)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("strided_slice").addInputs([in], begin, [end], stride).addIntegerArguments(0, 0, 1, 0, 0).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, l.Count)
			assertEquals(DataType.DOUBLE, l(0).dataType())
			assertTrue(l(0).isEmpty()) 'Should be empty array, is rank 0 scalar

			Nd4j.exec(op) 'Execution is OK
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDepthwise(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDepthwise(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.create(DataType.DOUBLE, 1,3,8,8)
			Dim depthwiseWeight As INDArray = Nd4j.create(DataType.DOUBLE, 1,1,3,2)
			Dim bias As INDArray = Nd4j.create(DataType.DOUBLE, 1, 6)

			Dim inputs() As INDArray = {input, depthwiseWeight, bias}

			Dim args() As Integer = {1, 1, 1, 1, 0, 0, 1, 1, 0}

			Dim output As INDArray = Nd4j.create(DataType.DOUBLE, 1, 6, 8, 8)

			Dim op As CustomOp = DynamicCustomOp.builder("depthwise_conv2d").addInputs(inputs).addIntegerArguments(args).addOutputs(output).callInplace(False).build()

			For i As Integer = 0 To 999
	'            System.out.println(i);
				Nd4j.Executioner.exec(op)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMod_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMod_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.createFromArray(5.0f, 6.0f, 7.0f)
			Dim y As val = Nd4j.scalar(4.0f)
			Dim e As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f)

			Dim z As val = Nd4j.exec(New ModOp(New INDArray(){x, y}, New INDArray(){}))(0)

			assertEquals(e, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarVector_edge_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarVector_edge_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.scalar(2.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f})
			Dim e As val = Nd4j.createFromArray(New Single(){4.0f})

			Dim z As val = Nd4j.exec(New AddOp(New INDArray(){x, y}, New INDArray(){}))(0)

			assertTrue(Shape.shapeEquals(e.shape(), z.shape()))
			assertEquals(e, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarVector_edge_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarVector_edge_2(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.scalar(2.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f})
			Dim e As val = Nd4j.createFromArray(New Single(){4.0f})

			Dim z As val = Nd4j.exec(New AddOp(New INDArray(){y, x}, New INDArray(){}))(0)

			assertTrue(Shape.shapeEquals(e.shape(), z.shape()))
			assertEquals(e, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInputValidationMergeMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInputValidationMergeMax(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim inputs() As INDArray = { Nd4j.createFromArray(0.0f, 1.0f, 2.0f).reshape("c"c, 1, 3), Nd4j.createFromArray(1.0f).reshape("c"c, 1, 1)}
			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 1, 3).assign(Double.NaN)
			Dim op As CustomOp = DynamicCustomOp.builder("mergemax").addInputs(inputs).addOutputs([out]).callInplace(False).build()
			Nd4j.exec(op)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUpsampling2dBackprop(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUpsampling2dBackprop(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)
			Dim c As Integer = 2
			Dim sz() As Integer = {2, 2}
			Dim inSize() As Long = {1, c, 3, 3}
			Dim eps As INDArray = Nd4j.rand(DataType.FLOAT, 1, c, sz(0) * inSize(2), sz(1) * inSize(3))

			Dim input As INDArray = Nd4j.create(inSize) 'Unused, not sure why this is even an arg...
			Dim exp As INDArray = Nd4j.create(DataType.FLOAT, inSize)

			For ch As Integer = 0 To c - 1
				Dim h As Integer=0
				Do While h<eps.size(2)
					Dim w As Integer=0
					Do While w<eps.size(3)
						Dim from() As Integer = {0, ch, h, w}
						Dim [to]() As Integer = {0, ch, h\sz(0), w\sz(1)}
						Dim add As Single = eps.getFloat(from)
						Dim current As Single = exp.getFloat([to])
						exp.putScalar([to], current + add)
						w += 1
					Loop
					h += 1
				Loop
			Next ch

	'        System.out.println("Eps:");
	'        System.out.println(eps.shapeInfoToString());
	'        System.out.println(Arrays.toString(eps.data().asFloat()));

	'        System.out.println("Expected:");
	'        System.out.println(exp.shapeInfoToString());
	'        System.out.println(Arrays.toString(exp.data().asFloat()));

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("upsampling2d_bp").addInputs(input, eps).addOutputs(exp.ulike()).addIntegerArguments(1).build()

			Nd4j.exec(op)

			Dim act As INDArray = op.getOutputArgument(0)
			assertEquals(exp, act)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsMaxView(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsMaxView(ByVal backend As Nd4jBackend)
			Dim predictions As INDArray = Nd4j.rand(DataType.FLOAT, 3, 4, 3, 2)

			Dim row As INDArray = predictions.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.point(0))
			row = row.reshape(ChrW(1), row.length())
			assertArrayEquals(New Long(){1, 4}, row.shape())

			Dim result1 As val = row.ulike()
			Dim result2 As val = row.ulike()

			Nd4j.exec(New IsMax(row.dup(), result1, 1)) 'OK
			Nd4j.exec(New IsMax(row, result2, 1)) 'C++ exception

			assertEquals(result1, result2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void isMax4d_2dims(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub isMax4d_2dims(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 3, 3, 4, 4).permute(0, 2, 3, 1)

			Dim out_permutedIn As INDArray = [in].like()
			Dim out_dupedIn As INDArray = [in].like()

			Nd4j.exec(New IsMax([in].dup(), out_dupedIn, 2, 3))
			Nd4j.exec(New IsMax([in], out_permutedIn, 2, 3))

			assertEquals(out_dupedIn, out_permutedIn)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSizeTypes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSizeTypes(ByVal backend As Nd4jBackend)
			Dim failed As IList(Of DataType) = New List(Of DataType)()
			For Each dt As DataType In New DataType(){DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.UINT64, DataType.UINT32, DataType.UINT16, DataType.UBYTE, DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.BFLOAT16}

				Dim [in] As INDArray = Nd4j.create(DataType.FLOAT, 100)
				Dim [out] As INDArray = Nd4j.scalar(dt, 0)
				Dim e As INDArray = Nd4j.scalar(dt, 100)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("size").addInputs([in]).addOutputs([out]).build()

				Try
					Nd4j.exec(op)

					assertEquals(e, [out])
				Catch t As Exception
					failed.Add(dt)
				End Try
			Next dt

			If failed.Count > 0 Then
				fail("Failed datatypes: " & failed.ToString())
			End If
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testListDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testListDiff(ByVal backend As Nd4jBackend)
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxPool2Dbp_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxPool2Dbp_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.HALF, 2,3,16,16).assign(Double.NaN)
			Dim y As val = Nd4j.create(DataType.HALF, 2,3,8,8).assign(Double.NaN)
			Dim z As val = Nd4j.create(DataType.HALF, 2,3,16,16)

			Dim op As val = DynamicCustomOp.builder("maxpool2d_bp").addInputs(x, y).addOutputs(z).addIntegerArguments(2, 2, 2, 2, 8,8, 1,1,1, 0,0).build()

			Nd4j.exec(op)
			Nd4j.Executioner.commit()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub test()

			Dim in1 As INDArray = Nd4j.create(DataType.BFLOAT16, 2, 3, 10, 1) 'Nd4j.createFromArray(0.2019043,0.6464844,0.9116211,0.60058594,0.34033203,0.7036133,0.6772461,0.3815918,0.87353516,0.04650879,0.67822266,0.8618164,0.88378906,0.7573242,0.66796875,0.63427734,0.33764648,0.46923828,0.62939453,0.76464844,-0.8618164,-0.94873047,-0.9902344,-0.88916016,-0.86572266,-0.92089844,-0.90722656,-0.96533203,-0.97509766,-0.4975586,-0.84814453,-0.984375,-0.98828125,-0.95458984,-0.9472656,-0.91064453,-0.80859375,-0.83496094,-0.9140625,-0.82470703,0.4802246,0.45361328,0.28125,0.28320312,0.79345703,0.44604492,-0.30273438,0.11730957,0.56396484,0.73583984,0.1418457,-0.44848633,0.6923828,-0.40234375,0.40185547,0.48632812,0.14538574,0.4638672,0.13000488,0.5058594)
			'.castTo(DataType.BFLOAT16).reshape(2,3,10,1);
			Dim in2 As INDArray = Nd4j.create(DataType.BFLOAT16, 2, 3, 10, 1) 'Nd4j.createFromArray(0.0,-0.13391113,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,-0.1751709,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.51904297,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.5107422,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0)
			'.castTo(DataType.BFLOAT16).reshape(2,3,10,1);

			Dim [out] As INDArray = in1.ulike()

			Nd4j.exec(DynamicCustomOp.builder("maxpool2d_bp").addInputs(in1, in2).addOutputs([out]).addIntegerArguments(5,1,1,2,2,0,1,1,1,0,0).build())

			Nd4j.Executioner.commit()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdjustContrast(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdjustContrast(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 4*4*3).reshape(ChrW(4), 4, 3)
			Dim [out] As INDArray = Nd4j.zeros(DataType.DOUBLE,4, 4, 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){-21.5, -20.5, -19.5, -15.5, -14.5, -13.5, -9.5, -8.5, -7.5, -3.5, -2.5, -1.5, 2.5, 3.5, 4.5, 8.5, 9.5, 10.5, 14.5, 15.5, 16.5, 20.5, 21.5, 22.5, 26.5, 27.5, 28.5, 32.5, 33.5, 34.5, 38.5, 39.5, 40.5, 44.5, 45.5, 46.5, 50.5, 51.5, 52.5, 56.5, 57.5, 58.5, 62.5, 63.5, 64.5, 68.5, 69.5, 70.5 }).reshape(ChrW(4), 4, 3)
			Nd4j.exec(New AdjustContrast([in], 2.0, [out]))

			assertArrayEquals([out].shape(), [in].shape())
			assertEquals(expected, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdjustContrastShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdjustContrastShape(ByVal backend As Nd4jBackend)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("adjust_contrast_v2").addInputs(Nd4j.create(DataType.FLOAT, 256, 256,3), Nd4j.scalar(0.5f)).build()
			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
			assertArrayEquals(New Long(){256, 256, 3}, lsd(0).getShape())
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitCastShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitCastShape(ByVal backend As Nd4jBackend)
			Dim [out] As INDArray = Nd4j.createUninitialized(1,10)
			Dim op As New BitCast(Nd4j.zeros(1,10), DataType.FLOAT.toInt(), [out])
			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
			assertArrayEquals(New Long(){1, 10, 2}, lsd(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdjustSaturation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdjustSaturation(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(New Double(){50, 100, 78, 118.5, 220, 112.5, 190, 163.5, 230, 255, 128.5, 134}).reshape(ChrW(2), 2, 3)
			Dim [out] As INDArray = Nd4j.create([in].shape())
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){0, 100, 56, 17, 220, 5, 150, 97, 230, 255, 2, 13}).reshape(ChrW(2), 2, 3)

			Nd4j.exec(New AdjustSaturation([in], 2.0, [out]))
			assertEquals(expected, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdjustHue(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdjustHue(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(New Double(){0, 100, 56, 17, 220, 5, 150, 97, 230, 255, 2, 13}).reshape(ChrW(2), 2, 3)
			Dim [out] As INDArray = Nd4j.create([in].shape())
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){100, 0, 44, 208, 5, 220, 177, 230, 97, 2, 255, 244}).reshape(ChrW(2), 2, 3)

			Nd4j.exec(New AdjustHue([in], 0.5, [out]))
			assertEquals(expected, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitCast(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitCast(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(DataType.FLOAT, 1.0f, 1.0f, 8).reshape(ChrW(2), 2, 2)
			Dim [out] As INDArray = Nd4j.createUninitialized(2,2)

			Nd4j.exec(New BitCast([in], DataType.DOUBLE.toInt(), [out]))

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){2.0, 512.0, 8192.0, 131072.032 }).reshape(ChrW(2), 2)
			assertArrayEquals(New Long(){2, 2}, [out].shape())
			assertEquals(expected, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDrawBoundingBoxesShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDrawBoundingBoxesShape(ByVal backend As Nd4jBackend)
			Dim images As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f, 0.6591f, 0.5555f, 0.1596f, 0.3087f, 0.1548f, 0.4695f, 0.9939f, 0.6113f, 0.6765f, 0.1800f, 0.6750f, 0.2246f, 0.0509f, 0.4601f, 0.8284f, 0.2354f, 0.9752f, 0.8361f, 0.2585f, 0.4189f, 0.7028f, 0.7679f, 0.5373f, 0.7234f, 0.2690f, 0.0062f, 0.0327f, 0.0644f, 0.8428f, 0.7494f, 0.0755f, 0.6245f, 0.3491f, 0.5793f, 0.5730f, 0.1822f, 0.6420f, 0.9143f}).reshape(ChrW(2), 5, 5, 1)
			Dim boxes As INDArray = Nd4j.createFromArray(New Single(){0.7717f, 0.9281f, 0.9846f, 0.4838f, 0.6433f, 0.6041f, 0.6501f, 0.7612f, 0.7605f, 0.3948f, 0.9493f, 0.8600f, 0.7876f, 0.8945f, 0.4638f, 0.7157f}).reshape(ChrW(2), 2, 4)
			Dim colors As INDArray = Nd4j.createFromArray(New Single(){0.9441f, 0.5957f}).reshape(ChrW(1), 2)
			Dim output As INDArray = Nd4j.create(DataType.FLOAT, images.shape())
			Dim op As val = New DrawBoundingBoxes(images, boxes, colors, output)
			Nd4j.exec(op)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f, 0.9441f, 0.9441f, 0.1596f, 0.3087f, 0.1548f, 0.4695f, 0.9939f, 0.6113f, 0.6765f, 0.1800f, 0.6750f, 0.2246f, 0.0509f, 0.4601f, 0.8284f, 0.2354f, 0.9752f, 0.8361f, 0.2585f, 0.4189f, 0.7028f, 0.7679f, 0.5373f, 0.7234f, 0.2690f, 0.0062f, 0.0327f, 0.0644f, 0.8428f, 0.9441f, 0.9441f, 0.9441f, 0.3491f, 0.5793f, 0.5730f, 0.1822f, 0.6420f, 0.9143f})
			assertEquals(expected, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Failing with results that are close") public void testFakeQuantAgainstTF_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFakeQuantAgainstTF_1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Double(){ 0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f, 0.6591f, 0.5555f, 0.1596f}).reshape(ChrW(3), 5)
			Dim min As INDArray = Nd4j.createFromArray(New Double(){ -0.2283f, -0.0719f, -0.0154f, -0.5162f, -0.3567f})
			Dim max As INDArray = Nd4j.createFromArray(New Double(){ 0.9441f, 0.5957f, 0.8669f, 0.3502f, 0.5100f})

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){0.7801f, 0.5966f, 0.7260f, 0.2320f, 0.5084f, 0.1800f, 0.5046f, 0.8684f, 0.3513f, 0.5084f, 0.0877f, 0.5966f, 0.6600f, 0.3513f, 0.1604f}).reshape(ChrW(3), 5)

			Dim op As val = New FakeQuantWithMinMaxVarsPerChannel(x,min,max)
			Dim output() As INDArray = Nd4j.exec(op)
			assertEquals(expected, output(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhereFail(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWhereFail(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(New Single(){0f, 1.0000f, 1.0000f, 1.0000f, 1.0000f})
			Dim [out] As INDArray = Nd4j.createUninitialized(4,1)
			Dim expected As INDArray = Nd4j.createFromArray(4,1)
			Dim op As val = New Where(New INDArray(){[in]}, New INDArray(){[out]})
			Nd4j.exec(op)
			assertArrayEquals(New Long(){4, 1}, [out].shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResizeBilinear1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testResizeBilinear1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(1, 10,10,4)
			Dim z As INDArray = Nd4j.createUninitialized(x.shape())
			Dim align As Boolean = False
			Dim op As val = New ResizeBilinear(x, z, 10, 10, align, False)
			Nd4j.exec(op)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResizeArea1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testResizeArea1(ByVal backend As Nd4jBackend)

			Dim x As INDArray = Nd4j.rand(DataType.FLOAT, 1, 2,3,4)
			Dim z As INDArray = Nd4j.createUninitialized(DataType.FLOAT, 1, 10, 10, 4)
			Dim op As New ResizeArea(x, z, 10, 10, False)
			Nd4j.exec(op)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResizeArea2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testResizeArea2(ByVal backend As Nd4jBackend)

			Dim image As INDArray = Nd4j.linspace(DataType.FLOAT, 1.0f, 1.0f, 9).reshape(ChrW(1), 3, 3, 1)
			Dim output As INDArray = Nd4j.createUninitialized(DataType.FLOAT, 1, 6, 6, 1)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 1.0f, 1.0f, 2.0f, 2.0f, 3.0f, 3.0f, 1.0f, 1.0f, 2.0f, 2.0f, 3.0f, 3.0f, 4.0f, 4.0f, 5.0f, 5.0f, 6.0f, 6.0f, 4.0f, 4.0f, 5.0f, 5.0f, 6.0f, 6.0f, 7.0f, 7.0f, 8.0f, 8.0f, 9.0f, 9.0f, 7.0f, 7.0f, 8.0f, 8.0f, 9.0f, 9.0f }).reshape(ChrW(1), 6, 6, 1)
			Dim op As New ResizeArea(image, output, 6, 6, False)
			Nd4j.exec(op)
			assertEquals(expected, output)
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDivideNoNan(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDivideNoNan(ByVal backend As Nd4jBackend)
			Dim in1 As INDArray = Nd4j.rand(DataType.DOUBLE, 2,3,4)
			Dim in2 As INDArray = Nd4j.rand(DataType.DOUBLE, 2,3,4)
			Dim [out] As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, 2,3,4)

			Nd4j.exec(New DivideNoNan(in1, in2, [out]))
			assertArrayEquals(New Long(){2, 3, 4}, [out].shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDrawBoundingBoxes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDrawBoundingBoxes(ByVal backend As Nd4jBackend)
			Dim images As INDArray = Nd4j.linspace(DataType.FLOAT, 1.0f, 1.0f, 2*4*5*3).reshape(ChrW(2), 4, 5, 3)
			Dim boxes As INDArray = Nd4j.createFromArray(New Single(){ 0.0f, 0.0f, 1.0f, 1.0f, 0.1f, 0.2f, 0.9f, 0.8f, 0.3f, 0.3f, 0.7f, 0.7f, 0.4f, 0.4f, 0.6f, 0.6f}).reshape(ChrW(2), 2, 4)
			Dim colors As INDArray = Nd4j.createFromArray(New Single(){ 201.0f, 202.0f, 203.0f, 127.0f, 128.0f, 129.0f}).reshape(ChrW(2), 3)
			Dim output As INDArray = Nd4j.create(DataType.FLOAT, images.shape())
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 201.0f, 202.0f, 203.0f, 127.0f, 128.0f, 129.0f, 19.0f, 20.0f, 21.0f, 22.0f, 23.0f, 24.0f, 127.0f, 128.0f, 129.0f, 201.0f, 202.0f, 203.0f, 127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 61.0f, 62.0f, 63.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 70.0f, 71.0f, 72.0f, 73.0f, 74.0f, 75.0f, 76.0f, 77.0f, 78.0f, 127.0f, 128.0f, 129.0f, 127.0f, 128.0f, 129.0f, 85.0f, 86.0f, 87.0f, 88.0f, 89.0f, 90.0f, 91.0f, 92.0f, 93.0f, 201.0f, 202.0f, 203.0f, 201.0f, 202.0f, 203.0f, 100.0f, 101.0f, 102.0f, 103.0f, 104.0f, 105.0f, 106.0f, 107.0f, 108.0f, 109.0f, 110.0f, 111.0f, 112.0f, 113.0f, 114.0f, 115.0f, 116.0f, 117.0f, 118.0f, 119.0f, 120.0f}).reshape(ChrW(2), 4, 5, 3)

			Nd4j.exec(New DrawBoundingBoxes(images, boxes, colors, output))

			assertArrayEquals(images.shape(), output.shape())
			assertEquals(expected, output)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void FakeQuantWithMinMaxVarsPerChannel(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub FakeQuantWithMinMaxVarsPerChannel(ByVal backend As Nd4jBackend)

			Dim x As INDArray = Nd4j.createFromArray(New Single(){-63.80f, -63.75f, -63.4f, -63.5f, 0.0f, 0.1f}).reshape(ChrW(1), 2, 3, 1)

			Dim min As INDArray = Nd4j.createFromArray(New Single(){-63.65f})
			Dim max As INDArray = Nd4j.createFromArray(New Single(){0.1f})

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){-63.75f, -63.75f, -63.5f, -63.5f, 0.0f, 0.0f}).reshape(ChrW(1), 2, 3, 1)

			Dim output() As INDArray = Nd4j.exec(New FakeQuantWithMinMaxVarsPerChannel(x,min,max))

			assertEquals(expected, output(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKnnMinDistance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKnnMinDistance(ByVal backend As Nd4jBackend)
			Dim point As INDArray = Nd4j.rand(DataType.FLOAT, 1, 20)
			Dim lowest As INDArray = Nd4j.rand(DataType.FLOAT, 1, 20)
			Dim highest As INDArray = Nd4j.rand(DataType.FLOAT, 1, 20)
			Dim distance As INDArray = Nd4j.scalar(0.0f)

			Nd4j.exec(New KnnMinDistance(point, lowest, highest, distance))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayersDropoutFail(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayersDropoutFail(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.rand(4, 5)
			Dim output As INDArray = Nd4j.createUninitialized(4, 5)
			Dim op As New DropOut(input, output, 0.1)
			Nd4j.exec(op)
	'        System.out.println(output);
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRange(ByVal backend As Nd4jBackend)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("range").addFloatingPointArguments(-1.0, 1.0, 0.01).build()

			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			'System.out.println("Calculated output shape: " + Arrays.toString(lsd.get(0).getShape()));
			op.setOutputArgument(0, Nd4j.create(lsd(0)))

			Nd4j.exec(op)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitCastShape_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitCastShape_1(ByVal backend As Nd4jBackend)
			Dim [out] As val = Nd4j.createUninitialized(1,10)
			Dim op As New BitCast(Nd4j.zeros(DataType.FLOAT,1,10), DataType.INT.toInt(), [out])
			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
			assertArrayEquals(New Long(){1, 10}, lsd(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitCastShape_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitCastShape_2(ByVal backend As Nd4jBackend)
			Dim [out] As val = Nd4j.createUninitialized(1,10)
			Dim op As New BitCast(Nd4j.zeros(DataType.DOUBLE,1,10), DataType.INT.toInt(), [out])
			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
			assertArrayEquals(New Long(){1, 10, 2}, lsd(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFusedBatchNorm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFusedBatchNorm(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 2*2*3*4).reshape(ChrW(2), 2, 3, 4)
			Dim scale As INDArray = Nd4j.create(DataType.DOUBLE, 4)
			scale.assign(0.5)
			Dim offset As INDArray = Nd4j.create(DataType.DOUBLE, 4)
			offset.assign(2.0)

			Dim y As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, x.shape())
			Dim batchMean As INDArray = Nd4j.create(4)
			Dim batchVar As INDArray = Nd4j.create(4)

			Dim op As New FusedBatchNorm(x,scale,offset,0,1, y, batchMean, batchVar)

			Dim expectedY As INDArray = Nd4j.createFromArray(New Double(){1.20337462, 1.20337462, 1.20337462, 1.20337462, 1.34821558, 1.34821558, 1.34821558, 1.34821558, 1.49305654, 1.49305654, 1.49305654, 1.49305654, 1.63789749, 1.63789749, 1.63789749, 1.63789749, 1.78273857, 1.78273857, 1.78273857, 1.78273857, 1.92757952, 1.92757952, 1.92757952, 1.92757952, 2.0724206, 2.0724206, 2.0724206, 2.0724206, 2.21726155, 2.21726155, 2.21726155, 2.21726155, 2.36210251, 2.36210251, 2.36210251, 2.36210251, 2.50694346, 2.50694346, 2.50694346, 2.50694346, 2.65178442, 2.65178442, 2.65178442, 2.65178442, 2.79662538, 2.79662538, 2.79662538, 2.79662538}).reshape(x.shape())
			Dim expectedBatchMean As INDArray = Nd4j.createFromArray(New Double(){23.0, 24.0, 25.0, 26.0})
			Dim expectedBatchVar As INDArray = Nd4j.createFromArray(New Double(){208.00001526, 208.00001526, 208.00001526, 208.00001526})
			Nd4j.exec(op)
			assertArrayEquals(expectedY.shape(), y.shape())
			assertArrayEquals(expectedBatchMean.shape(), batchMean.shape())
			assertArrayEquals(expectedBatchVar.shape(), batchVar.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFusedBatchNorm1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFusedBatchNorm1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f, 0.6591f, 0.5555f, 0.1596f, 0.3087f, 0.1548f, 0.4695f, 0.9939f, 0.6113f, 0.6765f, 0.1800f, 0.6750f, 0.2246f}).reshape(ChrW(1), 2, 3, 4)
			Dim scale As INDArray = Nd4j.createFromArray(New Single(){ 0.7717f, 0.9281f, 0.9846f, 0.4838f})
			Dim offset As INDArray = Nd4j.createFromArray(New Single(){0.9441f, 0.5957f, 0.8669f, 0.3502f})

			Dim y As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, x.shape())
			Dim batchMean As INDArray = Nd4j.create(4)
			Dim batchVar As INDArray = Nd4j.create(4)

			Dim op As New FusedBatchNorm(x,scale,offset,0,1, y, batchMean, batchVar)

			Dim expectedY As INDArray = Nd4j.createFromArray(New Single(){1.637202024f, 1.521406889f, 1.48303616f, -0.147269756f, 1.44721508f, -0.51030159f, 0.810390055f, 1.03076458f, 0.781284988f, 1.921229601f, -0.481337309f, 0.854952335f, 1.196854949f, 0.717398405f, -0.253610134f, -0.00865117f, -0.658405781f, 0.43602103f, 2.311818838f, 0.529999137f, 1.260738254f, -0.511638165f, 1.331095099f, -0.158477545f}).reshape(x.shape())
			Nd4j.exec(op)
			assertArrayEquals(expectedY.shape(), y.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFusedBatchNormHalf(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFusedBatchNormHalf(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(DataType.HALF, 1,2,3,4)
			'INDArray scale = Nd4j.createFromArray(new float[]{0.7717f, 0.9281f, 0.9846f, 0.4838f});
			'INDArray offset = Nd4j.createFromArray(new float[]{0.9441f, 0.5957f, 0.8669f, 0.3502f});
			Dim scale As INDArray = Nd4j.create(DataType.HALF, 4)
			Dim offset As INDArray = Nd4j.create(DataType.HALF, 4)

			Dim y As INDArray = Nd4j.createUninitialized(DataType.HALF, x.shape())
			Dim batchMean As INDArray = Nd4j.create(4)
			Dim batchVar As INDArray = Nd4j.create(4)

			Dim op As New FusedBatchNorm(x, scale, offset, 0, 1, y, batchMean, batchVar)
			Nd4j.exec(op)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrixBandPart(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixBandPart(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 2*3*3).reshape(ChrW(2), 3, 3)
			Dim op As val = New MatrixBandPart(x,1,1)
			Dim expected As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 2*3*3).reshape(ChrW(2), 3, 3)
	'        expected.putScalar(0, 0, 2, 0.);
	'        expected.putScalar(1, 0, 2, 0.);
	'        expected.putScalar(0, 2, 0, 0.);
	'        expected.putScalar(1, 2, 0, 0.);

			Dim [out]() As INDArray = Nd4j.exec(op)
			assertEquals(expected, x)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPolygamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPolygamma(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim x As INDArray = Nd4j.create(DataType.DOUBLE, 3,3)
			x.assign(0.5)
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){4.934802, -16.828796, 97.409088, -771.474243, 7691.113770f, -92203.460938f, 1290440.250000, -20644900.000000, 3.71595e+08}).reshape(ChrW(3), 3)
			Dim output As INDArray = Nd4j.create(DataType.DOUBLE, expected.shape())
			Dim op As val = New Polygamma(n,x,output)
			Nd4j.exec(op)
			assertEquals(expected, output)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLgamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLgamma(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Double(){0.1, 0.5, 0.7, 1.5, 1.7, 2.0, 2.5, 2.7, 3.0}).reshape(ChrW(3), 3)
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 2.2527127, 0.5723649, 0.26086727, -0.12078223, -0.09580769, 0.0, 0.28468287, 0.4348206, 0.6931472 }).reshape(ChrW(3), 3)
			Dim ret() As INDArray = Nd4j.exec(New Lgamma(x))
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomCrop(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomCrop(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Double(){1.8, 2.5, 4.0, 9.0, 2.1, 2.4, 3.0, 9.0, 2.1, 2.1, 0.7, 0.1, 3.0, 4.2, 2.2, 1.0}).reshape(ChrW(2), 2, 4)
			Dim shape As INDArray = Nd4j.createFromArray(New Integer() {1, 2, 3})
			Dim op As val = New RandomCrop(x, shape)
			Dim res() As INDArray = Nd4j.exec(op)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRoll(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRoll(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Double(){ 11.11, 11.12, 11.21, 11.22, 11.31, 11.32, 11.41, 11.42, 12.11, 12.12, 12.21, 12.22, 12.31, 12.32, 12.41, 12.42, 21.11, 21.12, 21.21, 21.22, 21.31, 21.32, 21.41, 21.42, 22.11, 22.12, 22.21, 22.22, 22.31, 22.32, 22.41, 22.42}).reshape(ChrW(2), 2, 4, 2)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 22.21, 22.22, 22.31, 22.32, 22.41, 22.42, 11.11, 11.12, 11.21, 11.22, 11.31, 11.32, 11.41, 11.42, 12.11, 12.12, 12.21, 12.22, 12.31, 12.32, 12.41, 12.42, 21.11, 21.12, 21.21, 21.22, 21.31, 21.32, 21.41, 21.42, 22.11, 22.12 }).reshape(x.shape())
			Dim op As val = New Roll(x, 6)
			Dim res() As INDArray = Nd4j.exec(op)
			assertEquals(expected, res(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToggleBits(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToggleBits(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.createFromArray(New Integer(){2, 2})
			Dim expected As INDArray = Nd4j.createFromArray(New Integer(){-3, -3})
			Dim op As New ToggleBits(input)
			Dim result As val = Nd4j.exec(op)
			assertEquals(expected, result(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("AS 11.28.2019 - https://github.com/eclipse/deeplearning4j/issues/8449") @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonMaxSuppression(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonMaxSuppression(ByVal backend As Nd4jBackend)
			Dim boxes As INDArray = Nd4j.createFromArray(New Single() {0.8115f, 0.4121f, 0.0771f, 0.4863f, 0.7412f, 0.7607f, 0.1543f, 0.5479f, 0.8223f, 0.2246f, 0.0049f, 0.6465f}).reshape(ChrW(3), 4)
			Dim scores As INDArray = Nd4j.createFromArray(New Single(){0.0029f, 0.8135f, 0.4873f})
			Dim op As val = New NonMaxSuppression(boxes,scores,2,0.5,0.5)
			Dim res As val = Nd4j.exec(op)
			assertEquals(New Long(){1}, res(0).shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrixBand(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixBand(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f}).reshape(ChrW(3), 4)
			Dim op As New MatrixBandPart(input,1,-1)
			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("Failed AS 11.26.2019 - https://github.com/eclipse/deeplearning4j/issues/8450") @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBetaInc1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBetaInc1(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f})
			Dim b As INDArray = Nd4j.createFromArray(New Single(){0.7717f, 0.9281f, 0.9846f, 0.4838f})
			Dim c As INDArray = Nd4j.createFromArray(New Single(){0.9441f, 0.5957f, 0.8669f, 0.3502f})
			Dim op As New BetaInc(a,b,c)
			Dim ret() As INDArray = Nd4j.exec(op)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){0.9122f, 0.6344f, 0.8983f, 0.6245f})
			assertEquals(expected, ret(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("Failure AS 11.28.2019 - https://github.com/eclipse/deeplearning4j/issues/8452") @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPolygamma1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPolygamma1(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f}).reshape(ChrW(3), 4)
			Dim b As INDArray = Nd4j.createFromArray(New Single(){0.7717f, 0.9281f, 0.9846f, 0.4838f, 0.6433f, 0.6041f, 0.6501f, 0.7612f, 0.7605f, 0.3948f, 0.9493f, 0.8600f}).reshape(ChrW(3), 4)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN}).reshape(ChrW(3), 4)
			Dim op As New Polygamma(a,b)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRoll1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRoll1(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f})
			Dim op As New Roll(a,Nd4j.scalar(2),Nd4j.scalar(0))
			Dim ret() As INDArray = Nd4j.exec(op)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){0.7244f, 0.2309f, 0.7788f, 0.8012f})
			assertEquals(expected, ret(0))
			Dim matrix As INDArray = Nd4j.create(New Double(){0.7788, 0.8012, 0.7244, 0.2309, 0.7271, 0.1804, 0.5056, 0.8925}).reshape(ChrW(2), 4)
			Dim roll2 As New Roll(matrix,Nd4j.scalar(0),Nd4j.scalar(1))
			Dim outputs() As INDArray = Nd4j.exec(roll2)
			Console.WriteLine(outputs(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdjustHueShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdjustHueShape(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f, 0.9234f, 0.0856f, 0.7938f, 0.6591f, 0.5555f, 0.1596f, 0.3087f, 0.1548f, 0.4695f, 0.9939f, 0.6113f, 0.6765f, 0.1800f, 0.6750f, 0.2246f, 0.0509f, 0.4601f, 0.8284f, 0.2354f, 0.9752f, 0.8361f, 0.2585f, 0.4189f, 0.7028f, 0.7679f, 0.5373f, 0.7234f, 0.2690f, 0.0062f, 0.0327f, 0.0644f, 0.8428f, 0.7494f, 0.0755f, 0.6245f, 0.3491f, 0.5793f, 0.5730f, 0.1822f, 0.6420f, 0.9143f, 0.3019f, 0.3574f, 0.1704f, 0.8395f, 0.5468f, 0.0744f, 0.9011f, 0.6574f, 0.4124f, 0.2445f, 0.4248f, 0.5219f, 0.6952f, 0.4900f, 0.2158f, 0.9549f, 0.1386f, 0.1544f, 0.5365f, 0.0134f, 0.4163f, 0.1456f, 0.4109f, 0.2484f, 0.3330f, 0.2974f, 0.6636f, 0.3808f, 0.8664f, 0.1896f, 0.7530f, 0.7215f, 0.6612f, 0.7270f, 0.5704f, 0.2666f, 0.7453f, 0.0444f, 0.3024f, 0.4850f, 0.7982f, 0.0965f, 0.7843f, 0.5075f, 0.0844f, 0.8370f, 0.6103f, 0.4604f, 0.6087f, 0.8594f, 0.4599f, 0.6714f, 0.2744f, 0.1981f, 0.4143f, 0.7821f, 0.3505f, 0.5040f, 0.1180f, 0.8307f, 0.1817f, 0.8442f, 0.5074f, 0.4471f, 0.5105f, 0.6666f, 0.2576f, 0.2341f, 0.6801f, 0.2652f, 0.5394f, 0.4690f, 0.6146f, 0.1210f, 0.2576f, 0.0769f, 0.4643f, 0.1628f, 0.2026f, 0.3774f, 0.0506f, 0.3462f, 0.5720f, 0.0838f, 0.4228f, 0.0588f, 0.5362f, 0.4756f, 0.2530f, 0.1778f, 0.0751f, 0.8977f, 0.3648f, 0.3065f, 0.4739f, 0.7014f, 0.4473f, 0.5171f, 0.1744f, 0.3487f, 0.7759f, 0.9491f, 0.2072f, 0.2182f, 0.6520f, 0.3092f, 0.9545f, 0.1881f, 0.9579f, 0.1785f, 0.9636f, 0.4830f, 0.6569f, 0.3353f, 0.9997f, 0.5869f, 0.5747f, 0.0238f, 0.2943f, 0.5248f, 0.5879f, 0.7266f, 0.1965f, 0.9167f, 0.9726f, 0.9206f, 0.0519f, 0.2997f, 0.0039f, 0.7652f, 0.5498f, 0.3794f, 0.3791f, 0.3528f, 0.2873f, 0.8082f, 0.4732f, 0.4399f, 0.6606f, 0.5991f, 0.0034f, 0.4874f}).reshape(ChrW(8), 8, 3)

			Dim op As New AdjustHue(image, 0.2f)
			Dim res() As INDArray = Nd4j.exec(op)
	'        System.out.println(res[0]);
			Dim lsd As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			assertEquals(1, lsd.Count)
			assertArrayEquals(New Long(){8, 8, 3}, lsd(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitCastShape_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitCastShape_3(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.createFromArray(New Integer(){1, 2, 3, 4, 5, 6, 7, 8}).reshape(ChrW(1), 4, 2)
			Dim e As val = Nd4j.createFromArray(New Long(){8589934593L, 17179869187L, 25769803781L, 34359738375L}).reshape(ChrW(1), 4)
			Dim z As val = Nd4j.exec(New BitCast(x, DataType.LONG.toInt()))(0)

			assertEquals(e, z)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatch_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatch_1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.ones(DataType.FLOAT, 3,3)
			Dim y As INDArray = Nd4j.linspace(DataType.FLOAT, -5, 9, 1).reshape(ChrW(3), 3)
			Dim c As val = Conditions.equals(0.0)

			Dim z As INDArray = x.match(y, c)
			Dim exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {False, False, False},
				New Boolean() {False, False, False},
				New Boolean() {True, False, False}
			})

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatch_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatch_2(ByVal backend As Nd4jBackend)
			Dim assignments() As Integer = {0, 0, 0, 1, 0, 2, 2}
			Dim indexes() As Integer = {0, 1, 2, 3, 4, 5, 7}

			Dim asarray As INDArray = Nd4j.createFromArray(assignments)
			Dim idxarray As INDArray = Nd4j.createFromArray(indexes)

			Dim testIndicesForMask() As Integer = {1, 2}
			Dim assertions() As INDArray = { Nd4j.createFromArray(False,False,False,True,False,False,False), Nd4j.createFromArray(False,False,False,False,False,True,True) }

			For j As Integer = 0 To testIndicesForMask.Length - 1
				Dim mask As INDArray = asarray.match(testIndicesForMask(j), Conditions.equals(testIndicesForMask(j)))
				assertEquals(assertions(j),mask)

			Next j

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateOp_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateOp_1(ByVal backend As Nd4jBackend)
			Dim shape As val = Nd4j.createFromArray(New Integer() {3, 4, 5})
			Dim exp As val = Nd4j.create(DataType.INT, 3, 4, 5)

			Dim result As val = Nd4j.exec(New Create(shape, "c"c, True, DataType.INT))(0)

			assertEquals(exp, result)
		End Sub

		' Exact copy of libnd4j test
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRgbToHsv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRgbToHsv(ByVal backend As Nd4jBackend)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 0.545678377f, 0.644941628f, 0.461456001f, 0.588904262f, 0.725874603f, 0.517642438f, 0.0869259685f, 0.54742825f, 0.413571358f, 0.890151322f, 0.928968489f, 0.684074104f, 0.52110225f, 0.753103435f, 0.913557053f, 0.46850124f, 0.761800349f, 0.237176552f, 0.90049392f, 0.965541422f, 0.486593395f, 0.263826847f, 0.290193319f, 0.148351923f, 0.674094439f, 0.0361763388f, 0.3721793f, 0.823592246f, 0.524110138f, 0.2204483f, 0.632020354f, 0.637001634f, 0.216262609f, 0.279114306f, 0.25007084f, 0.30433768f, 0.0448598303f, 0.586083114f, 0.978048146f, 0.91390729f, 0.385092884f, 0.218390301f, 0.762684941f, 0.505838513f, 0.366362303f, 0.931746006f, 0.00208298792f, 0.875348926f, 0.428009957f, 0.270003974f, 0.313204288f, 0.775881767f, 0.367065936f, 0.164243385f, 0.644775152f, 0.575452209f, 0.911922634f, 0.0581932105f, 0.437950462f, 0.946475744f }).reshape(ChrW(5), 4, 3)
			Dim input As INDArray = Nd4j.createFromArray(New Single(){ 0.262831867f, 0.723622441f, 0.740797927f, 0.717254877f, 0.430244058f, 0.418478161f, 0.906427443f, 0.199753001f, 0.725874603f, 0.890151322f, 0.928968489f, 0.684074104f, 0.312434604f, 0.991390795f, 0.163174023f, 0.268038541f, 0.361258626f, 0.685067773f, 0.682347894f, 0.84635365f, 0.761800349f, 0.753103435f, 0.913557053f, 0.965541422f, 0.112067183f, 0.540247589f, 0.280050347f, 0.106776128f, 0.679180562f, 0.870388806f, 0.604331017f, 0.630475283f, 0.674094439f, 0.279114306f, 0.632020354f, 0.823592246f, 0.490824632f, 0.75257351f, 0.129888852f, 0.849081645f, 0.883509099f, 0.765611768f, 0.997870266f, 0.446510047f, 0.385092884f, 0.931746006f, 0.978048146f, 0.91390729f, 0.685308874f, 0.0834472676f, 0.396037966f, 0.756701186f, 0.597481251f, 0.784472764f, 0.514242649f, 0.392005324f, 0.911922634f, 0.270003974f, 0.644775152f, 0.946475744f }).reshape(ChrW(5), 4, 3)
			Dim op As New RgbToHsv(input)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(ret(0), expected)
		End Sub

		' Exact copy of libnd4j test

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHsvToRgb(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHsvToRgb(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.createFromArray(New Single(){0.705504596f, 0.793608069f, 0.65870738f, 0.848827183f, 0.920532584f, 0.887555957f, 0.72317636f, 0.563831031f, 0.773604929f, 0.269532293f, 0.332347751f, 0.111181192f}).reshape(ChrW(4), 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){0.257768334f, 0.135951888f, 0.65870738f, 0.887555957f, 0.0705317783f, 0.811602857f, 0.485313689f, 0.337422464f, 0.773604929f, 0.0883753772f, 0.111181192f, 0.074230373f}).reshape(ChrW(4), 3)

			Dim op As New HsvToRgb(input)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(ret(0), expected)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHsvToRgb_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHsvToRgb_1(ByVal backend As Nd4jBackend)
	'         Emulation of simple TF test:
	'           image = tf.random_uniform(shape = [1,1,3])
	'           tf.image.hsv_to_rgb(image)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){0.778785586f, 0.801197767f, 0.724374652f}).reshape(ChrW(1), 1, 3)
			Dim op As New HsvToRgb(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			Console.WriteLine(ret(0).toStringFull())
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 0.53442812f, 0.144007325f, 0.724374652f}).reshape(ChrW(1), 1, 3)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRgbToHsv_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRgbToHsv_1(ByVal backend As Nd4jBackend)
	'         Emulation of simple TF test:
	'           image = tf.random_uniform(shape = [1,2,3])
	'           tf.image.rgb_to_hsv(image)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){0.778785586f, 0.801197767f, 0.724374652f, 0.230894327f, 0.727141261f, 0.180390716f}).reshape(ChrW(2), 3)
			Dim op As New RgbToHsv(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			Dim expected As INDArray = Nd4j.createFromArray(New Single(){0.215289578f, 0.095885336f, 0.801197767f, 0.317938268f, 0.751917899f, 0.727141261f}).reshape(ChrW(2), 3)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLu(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLu(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.createFromArray(New Single(){1.0f, 2.0f, 3.0f, 0.0f, 2.0f, 3.0f, 0.0f, 0.0f, 7.0f}).reshape(ChrW(3), 3)
			Dim op As New Lu(input)
			Dim ret() As INDArray = Nd4j.exec(op)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){1.0f, 2.0f, 3.0f, 0.0f, 2.0f, 3.0f, 0.0f, 0.0f, 7f}).reshape(ChrW(3), 3)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRgbToYiq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRgbToYiq(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){ 0.48055f, 0.80757356f, 0.2564435f, 0.94277316f, 0.17006584f, 0.33366168f, 0.41727918f, 0.54528666f, 0.48942474f, 0.3305715f, 0.98633456f, 0.00158441f, 0.97605824f, 0.02462568f, 0.14837205f, 0.00112842f, 0.99260217f, 0.9585542f, 0.41196227f, 0.3095014f, 0.6620493f, 0.30888894f, 0.3122602f, 0.7993488f, 0.86656475f, 0.5997049f, 0.9776477f, 0.72481847f, 0.7835693f, 0.14649455f, 0.3573504f, 0.33301765f, 0.7853056f, 0.25830218f, 0.59289205f, 0.41357264f, 0.5934154f, 0.72647524f, 0.6623308f, 0.96197623f, 0.0720306f, 0.23853847f, 0.1427159f, 0.19581454f, 0.06766324f, 0.10614152f, 0.26093867f, 0.9584985f, 0.01258832f, 0.8160156f, 0.56506383f, 0.08418505f, 0.86440504f, 0.6807802f, 0.20662387f, 0.4153733f, 0.76146203f, 0.50057423f, 0.08274968f, 0.9521758f }).reshape(ChrW(5), 4, 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 0.64696468f, -0.01777124f, -0.24070648f, 0.41975525f, 0.40788622f, 0.21433232f, 0.50064416f, -0.05832884f, -0.04447775f, 0.67799989f, -0.07432612f, -0.44518381f, 0.32321111f, 0.52719408f, 0.2397369f, 0.69227005f, -0.57987869f, -0.22032876f, 0.38032767f, -0.05223263f, 0.13137188f, 0.3667803f, -0.15853189f, 0.15085728f, 0.72258149f, 0.03757231f, 0.17403452f, 0.69337627f, 0.16971045f, -0.21071186f, 0.39185397f, -0.13084008f, 0.145886f, 0.47240727f, -0.1417591f, -0.12659159f, 0.67937788f, -0.05867803f, -0.04813048f, 0.35710624f, 0.47681283f, 0.24003804f, 0.1653288f, 0.00953913f, -0.05111816f, 0.29417614f, -0.31640032f, 0.18433114f, 0.54718234f, -0.39812097f, -0.24805083f, 0.61018603f, -0.40592682f, -0.22219216f, 0.39241133f, -0.23560742f, 0.06353694f, 0.3067938f, -0.0304029f, 0.35893188f }).reshape(ChrW(5), 4, 3)

			Dim op As New RgbToYiq(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testYiqToRgb(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testYiqToRgb(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){ 0.775258899f, -0.288912386f, -0.132725924f, 0.0664454922f, -0.212469354f, 0.455438733f, 0.418221354f, 0.349350512f, 0.145902053f, 0.947576523f, -0.471601307f, 0.263960421f, 0.700227439f, 0.32434237f, -0.278446227f, 0.130805135f, -0.438441873f, 0.187127829f, 0.0276055578f, -0.179727226f, 0.305075705f, 0.716282248f, 0.278215706f, -0.44586885f, 0.76971364f, 0.131288841f, -0.141177326f, 0.900081575f, -0.0788725987f, 0.14756602f, 0.387832165f, 0.229834676f, 0.47921446f, 0.632930398f, 0.0443540029f, -0.268817365f, 0.0977194682f, -0.141669706f, -0.140715122f, 0.946808815f, -0.52525419f, -0.106209636f, 0.659476519f, 0.391066104f, 0.426448852f, 0.496989518f, -0.283434421f, -0.177366048f, 0.715208411f, -0.496444523f, 0.189553142f, 0.616444945f, 0.345852494f, 0.447739422f, 0.224696323f, 0.451372236f, 0.298027098f, 0.446561724f, -0.187599331f, -0.448159873f }).reshape(ChrW(5), 4, 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 0.416663059f, 0.939747555f, 0.868814286f, 0.146075352f, -0.170521997f, 1.07776645f, 0.842775284f, 0.228765106f, 0.280231822f, 0.660605291f, 0.905021825f, 1.91936605f, 0.837427991f, 0.792213732f, -0.133271854f, -0.17216571f, 0.128957025f, 0.934955336f, 0.0451873479f, -0.120952621f, 0.746436225f, 0.705446224f, 0.929172217f, -0.351493549f, 0.807577594f, 0.825371955f, 0.383812296f, 0.916293093f, 0.82603058f, 1.23885956f, 0.905059196f, 0.015164554f, 0.950156781f, 0.508443732f, 0.794845279f, 0.12571529f, -0.125074273f, 0.227326869f, 0.0147000261f, 0.378735409f, 1.15842402f, 1.34712305f, 1.2980804f, 0.277102016f, 0.953435072f, 0.115916842f, 0.688879376f, 0.508405162f, 0.35829352f, 0.727568094f, 1.58768577f, 1.22504294f, 0.232589777f, 0.996727258f, 0.841224629f, -0.0909671176f, 0.233051388f, -0.0110094378f, 0.787642119f, -0.109582274f }).reshape(ChrW(5), 4, 3)

			Dim op As New YiqToRgb(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRgbToGrayscale(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRgbToGrayscale(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){1.7750e+01f, -7.1062e+01f, -1.0019e+02f, -2.3406e+01f, 5.2094e+01f, 9.5438e+01f, -6.7461e+00f, 3.8562e+01f, 6.5078e+00f, 3.3562e+01f, -5.8844e+01f, 2.2750e+01f, -1.0477e+01f, 7.7344e+00f, 9.5469e+00f, 2.1391e+01f, -8.5312e+01f, 7.5830e-01f, 2.3125e+01f, 1.8145e+00f, 1.4602e+01f, -4.5859e+00f, 3.9344e+01f, 1.1617e+01f, -8.6562e+01f, 1.0038e+02f, 6.7938e+01f, 5.9961e+00f, 6.7812e+01f, 2.9734e+01f, 2.9609e+01f, -6.1438e+01f, 1.7750e+01f, 6.8562e+01f, -7.4414e+00f, 3.9656e+01f, 1.1641e+01f, -2.7516e+01f, 6.7562e+01f, 7.8438e+01f, 5.4883e+00f, 2.9438e+01f, -3.1344e+01f, 6.5125e+01f, 1.2695e+01f, 4.0531e+01f, -6.1211e+00f, 6.2219e+01f, 4.6812e+01f, 5.2250e+01f, -1.1414e+01f, 1.5404e-02f, 2.9938e+01f, 5.6719e+00f, -2.0125e+01f, 2.1531e+01f, 6.2500e+01f, 7.2188e+01f, 9.3750e+00f, -4.8125e+01f}).reshape(ChrW(5), 4, 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){-47.82958221f, 34.46305847f, 21.36137581f, -21.91625023f, 2.49686432f, -43.59792709f, 9.64180183f, 23.04854202f, 40.7946167f, 44.98754883f, -25.19047546f, 20.64586449f, -4.97033119f, 30.0226841f, 30.30688286f, 15.61459541f, 43.36166f, 18.22480774f, 13.74833488f, 21.59387016f}).reshape(ChrW(5), 4, 1)

			Dim op As New RgbToGrayscale(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRgbToYuv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRgbToYuv(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){10f, 50f, 200f})

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 55.14f, 71.2872001f, -39.6005542f })

			Dim op As New RgbToYuv(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testYuvToRgb(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testYuvToRgb(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.createFromArray(New Single(){ 55.14f, 71.2872001f, -39.6005542f })

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 10f, 50f, 200f })
			Dim op As New YuvToRgb(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRgbToYiqEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRgbToYiqEmpty(ByVal backend As Nd4jBackend)
			Dim image As INDArray = Nd4j.create(0,4,3)
			Dim op As New RgbToYiq(image)
			Dim ret() As INDArray = Nd4j.exec(op)
			assertArrayEquals(image.shape(), ret(0).shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTriangularSolve(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTriangularSolve(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){ 3.0f, 0.0f, 0.0f, 0.0f, 2.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f }).reshape(ChrW(4), 4)

			Dim b As INDArray = Nd4j.createFromArray(New Single(){ 4.0f, 2.0f, 4.0f, 2.0f }).reshape(ChrW(4), 1)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 1.333333f, -0.6666667f, 2.6666667f, -1.3333333f }).reshape(ChrW(4), 1)

			Dim op As val = New TriangularSolve(a, b, True, False)
			Dim ret() As INDArray = Nd4j.exec(op)

			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOnesLike_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOnesLike_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 4, 5)
			Dim e As val = Nd4j.ones(DataType.INT32, 3, 4, 5)

			Dim z As val = Nd4j.exec(New OnesLike(x, DataType.INT32))(0)
			assertEquals(e, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinSpaceEdge_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinSpaceEdge_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.linspace(1,10,1, DataType.FLOAT)
			Dim e As val = Nd4j.scalar(1.0f)

			assertEquals(e, x)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinearSolve(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinearSolve(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){ 2.0f, -1.0f, -2.0f, -4.0f, 6.0f, 3.0f, -4.0f, -2.0f, 8.0f }).reshape(ChrW(3), 3)

			Dim b As INDArray = Nd4j.createFromArray(New Single(){ 2.0f, 4.0f, 3.0f }).reshape(ChrW(3), 1)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 7.625f, 3.25f, 5.0f }).reshape(ChrW(3), 1)

			Dim op As val = New LinearSolve(a, b)
			Dim ret() As INDArray = Nd4j.exec(op)

			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinearSolveAdjust(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinearSolveAdjust(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){ 0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f }).reshape(ChrW(3), 3)

			Dim b As INDArray = Nd4j.createFromArray(New Single(){ 0.7717f, 0.9281f, 0.9846f, 0.4838f, 0.6433f, 0.6041f, 0.6501f, 0.7612f, 0.7605f }).reshape(ChrW(3), 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 1.5504692f, 1.8953944f, 2.2765768f, 0.03399149f, 0.2883001f, 0.5377323f, -0.8774802f, -1.2155888f, -1.8049058f }).reshape(ChrW(3), 3)

			Dim op As val = New LinearSolve(a, b, True)
			Dim ret() As INDArray = Nd4j.exec(op)

			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLstsq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLstsq(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.createFromArray(New Single(){ 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 11.0f, 8.0f, 21.0f }).reshape(ChrW(3), 3)

			Dim b As INDArray = Nd4j.createFromArray(New Single(){ 1.0f, 2.0f, 3.0f }).reshape(ChrW(3), 1)

			Dim op As val = New Lstsq(a,b)
			Dim ret() As INDArray = Nd4j.exec(op)

			Dim matmul As DynamicCustomOp = DynamicCustomOp.builder("matmul").addInputs(a, ret(0)).build()
			Dim matres() As INDArray = Nd4j.exec(matmul)
			For i As Integer = 0 To 2
				assertEquals(b.getFloat(i, 0), matres(0).getFloat(i, 0), 1e-4)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSequenceMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSequenceMask(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.createFromArray(New Integer(){1, 3, 2})
			' Test with static max len
			Dim maxlen As Integer = 2
			Dim expected As INDArray = Nd4j.createFromArray(New Integer(){ 1, 0, 0, 1, 1, 1, 1, 1, 0 }).reshape(ChrW(3), 3)

			Dim ret() As INDArray = Nd4j.exec(New SequenceMask(arr, maxlen, DataType.INT32))
			assertEquals(expected, ret(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCholesky(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCholesky(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Double() {4, 12, -16, 12, 37, -43, -16, -43, 98}).reshape(ChrW(3), 3)
			Dim exp As INDArray = Nd4j.createFromArray(New Double() {2.0, 0.0, 0.0, 6.0, 1.0, 0.0, -8.0, 5.0, 3.0}).reshape(ChrW(3), 3)

			Dim res() As INDArray = Nd4j.exec(New Cholesky(x))
			assertEquals(res(0), exp)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testQr(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testQr(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.createFromArray(New Double(){ 12.0, -51.0, 4.0, 6.0, 167.0, -68.0, -4.0, 24.0, -41.0, -1.0, 1.0, 0.0, 2.0, 0.0, 3.0 }).reshape(ChrW(5), 3)
			Dim op As New Qr([in])
			Dim ret() As INDArray = Nd4j.exec(op)
			Dim res As INDArray = Nd4j.createUninitialized([in].shape())
			Dim matmul As DynamicCustomOp = DynamicCustomOp.builder("matmul").addInputs(ret(0), ret(1)).build()
			ret = Nd4j.exec(matmul)
			assertEquals(ret(0), [in])
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspaceSignature_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLinspaceSignature_1()
			Dim array1 As val = Nd4j.exec(New Linspace(DataType.FLOAT, Nd4j.scalar(1.0f), Nd4j.scalar(10.0f), Nd4j.scalar(10L)))(0)
			Dim array2 As val = Nd4j.exec(New Linspace(DataType.FLOAT, 1.0f, 10.0f, 10L))(0)

			assertEquals(array1.dataType(), array2.dataType())
			assertEquals(array1, array2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogdet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogdet(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(New Double(){4, 12, -16, 12, 37, -43, -16, -43, 98, 4, 1.2, -1.6, 1.2, 3.7, -4.3, -1.6, -4.3, 9.8}).reshape(ChrW(2), 3, 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){3.5835189, 4.159008})
			Dim ret() As INDArray = Nd4j.exec(New Logdet(x))
			assertEquals(ret(0), expected)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBatchNormBpNHWC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchNormBpNHWC(ByVal backend As Nd4jBackend)
			'Nd4j.getEnvironment().allowHelpers(false);        //Passes if helpers/MKLDNN is disabled

			Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 2, 4, 4, 3)
			Dim eps As INDArray = Nd4j.rand(DataType.FLOAT, [in].shape())
			Dim epsStrided As INDArray = eps.permute(1,0,2,3).dup().permute(1,0,2,3)
			Dim mean As INDArray = Nd4j.rand(DataType.FLOAT, 3)
			Dim var As INDArray = Nd4j.rand(DataType.FLOAT, 3)
			Dim gamma As INDArray = Nd4j.rand(DataType.FLOAT, 3)
			Dim beta As INDArray = Nd4j.rand(DataType.FLOAT, 3)

			assertEquals(eps, epsStrided)

			Dim out1eps As INDArray = [in].like().castTo(DataType.FLOAT)
			Dim out1m As INDArray = mean.like().castTo(DataType.FLOAT)
			Dim out1v As INDArray = var.like().castTo(DataType.FLOAT)

			Dim out2eps As INDArray = [in].like().castTo(DataType.FLOAT)
			Dim out2m As INDArray = mean.like().castTo(DataType.FLOAT)
			Dim out2v As INDArray = var.like().castTo(DataType.FLOAT)

			Dim op1 As DynamicCustomOp = DynamicCustomOp.builder("batchnorm_bp").addInputs([in], mean, var, gamma, beta, eps).addOutputs(out1eps, out1m, out1v).addIntegerArguments(1, 1, 3).addFloatingPointArguments(1e-5).build()

			Dim op2 As DynamicCustomOp = DynamicCustomOp.builder("batchnorm_bp").addInputs([in], mean, var, gamma, beta, epsStrided).addOutputs(out2eps, out2m, out2v).addIntegerArguments(1, 1, 3).addFloatingPointArguments(1e-5).build()

			Nd4j.exec(op1)
			Nd4j.exec(op2)

			assertEquals(out1eps, out2eps) 'Fails here
			assertEquals(out1m, out2m)
			assertEquals(out1v, out2v)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpaceToDepthBadStrides(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpaceToDepthBadStrides(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 6, 6)
			Dim inBadStrides As INDArray = [in].permute(1,0,2,3).dup().permute(1,0,2,3)
			assertEquals([in], inBadStrides)

			Console.WriteLine("in: " & [in].shapeInfoToString())
			Console.WriteLine("inBadStrides: " & inBadStrides.shapeInfoToString())

			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 2, 12, 3, 3)
			Dim out2 As INDArray = [out].like()

			Dim op1 As CustomOp = DynamicCustomOp.builder("space_to_depth").addInputs([in]).addIntegerArguments(2, 0).addOutputs([out]).build()
			Nd4j.exec(op1)

			Dim op2 As CustomOp = DynamicCustomOp.builder("space_to_depth").addInputs(inBadStrides).addIntegerArguments(2, 0).addOutputs(out2).build()
			Nd4j.exec(op2)

			assertEquals([out], out2)
		End Sub
	End Class

End Namespace