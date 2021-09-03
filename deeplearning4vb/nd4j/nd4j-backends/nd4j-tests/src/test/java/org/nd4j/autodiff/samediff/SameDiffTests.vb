Imports System
Imports System.Collections.Generic
Imports org.junit.jupiter.api.Assertions
Imports org.junit.jupiter.api.Assumptions
import static org.nd4j.linalg.indexing.NDArrayIndex.all
Imports Maps = com.google.common.collect.Maps
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.junit.jupiter.api
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports OutAndGrad = org.nd4j.autodiff.samediff.api.OutAndGrad
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports WeightsFormat = org.nd4j.enums.WeightsFormat
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationBinary = org.nd4j.evaluation.classification.EvaluationBinary
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports LocalResponseNormalizationConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports TensorArray = org.nd4j.linalg.api.ops.impl.shape.tensorops.TensorArray
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports GreaterThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThanOrEqual
Imports IsNonDecreasing = org.nd4j.linalg.api.ops.impl.transforms.custom.IsNonDecreasing
Imports IsNumericTensor = org.nd4j.linalg.api.ops.impl.transforms.custom.IsNumericTensor
Imports IsStrictlyIncreasing = org.nd4j.linalg.api.ops.impl.transforms.custom.IsStrictlyIncreasing
Imports LessThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThanOrEqual
Imports Max = org.nd4j.linalg.api.ops.impl.transforms.custom.Max
Imports Min = org.nd4j.linalg.api.ops.impl.transforms.custom.Min
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports SingletonMultiDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonMultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports UniformInitScheme = org.nd4j.weightinit.impl.UniformInitScheme

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

Namespace org.nd4j.autodiff.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) public class SameDiffTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SameDiffTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function



		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 999999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.create(1)
			Nd4j.Random.setSeed(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub

		Public Overridable Function variablesForInput() As IDictionary(Of String, INDArray)
			Dim inputs As INDArray = Nd4j.create(New Double()(){
				New Double() {0.52, 1.12, 0.77},
				New Double() {0.88, -1.08, 0.15},
				New Double() {0.52, 0.06, -1.30},
				New Double() {0.74, -2.49, 1.39}
			})

			Dim labels As INDArray = Nd4j.create(New Double(){1, 1, 0, 1}).reshape(ChrW(4), 1)

			Dim weights As INDArray = Nd4j.zeros(3, 1).castTo(labels.dataType())

			Dim inputMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			inputMap("x") = inputs
			inputMap("w") = weights
			inputMap("y") = labels
			Return inputMap
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableNaming_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableNaming_1(ByVal backend As Nd4jBackend)
			Dim sd As val = SameDiff.create()

			Dim input As val = sd.var("inp", New Long(){2, 3})

			Dim nodeA As val = sd.math().square(input)
			Dim nodeB As val = sd.math().square(nodeA)

			sd.associateArrayWithVariable(Nd4j.create(New Double(){1, 2, 3, 4, 5, 6}, New Long(){2, 3}).castTo(input.dataType()), input)

			sd.outputAll(Nothing)

			nodeA.isPlaceHolder()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddArgsAndOutput(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddArgsAndOutput(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim varOne As val = sameDiff_Conflict.var("one", Nd4j.ones(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMseBackwards(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMseBackwards(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 3
			Dim input As SDVariable = sd.var("in", DataType.FLOAT, New Long(){minibatch, nOut})
			Dim label As SDVariable = sd.var("label", DataType.FLOAT, New Long(){minibatch, nOut})

			Dim diff As SDVariable = input.sub(label)
			Dim sqDiff As SDVariable = diff.mul(diff)
			Dim msePerEx As SDVariable = sd.mean("msePerEx", sqDiff, 1)
			Dim avgMSE As SDVariable = sd.mean("loss", msePerEx, 0)

			Dim inputArr As INDArray = Nd4j.rand(DataType.FLOAT, minibatch, nOut)
			Dim labelArr As INDArray = Nd4j.rand(DataType.FLOAT, minibatch, nOut)

			sd.associateArrayWithVariable(inputArr, input)
			sd.associateArrayWithVariable(labelArr, label)

			Dim result As INDArray = avgMSE.eval()
			assertEquals(1, result.length())

			sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalVariable(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalVariable(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim ones As INDArray = Nd4j.ones(4)
			Dim twos As INDArray = ones.add(ones)
			Dim inputOne As SDVariable = sameDiff_Conflict.var("inputone", ones)
			Dim inputResult As SDVariable = inputOne.add("extravarname", inputOne)
			assertEquals(twos, inputResult.eval())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 4, 4, DataType.FLOAT)).reshape(ChrW(1), 4)
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim result As SDVariable = sameDiff_Conflict.sum(x, 1) '[1,4].sum(1) == [1]

			Dim exp As INDArray = Nd4j.scalar(arr.sumNumber().floatValue()).reshape(1)
			Dim resultArr As INDArray = result.eval()
			assertEquals(exp, resultArr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddEval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddEval(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim x As INDArray = Nd4j.scalar(1.0)
			Dim y As INDArray = Nd4j.scalar(2.0)
			Dim xVar As SDVariable = sameDiff_Conflict.placeHolder("x", DataType.DOUBLE, 1, 1)
			Dim yVar As SDVariable = sameDiff_Conflict.placeHolder("y", DataType.DOUBLE, 1, 1)
			Dim output As SDVariable = xVar.add(yVar)
			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("x") = x
			m("y") = y
			Dim [out] As INDArray = sameDiff_Conflict.output(m, Collections.singletonList(output.name()))(output.name())
			Dim outputAssertion As INDArray = x.add(y)
			assertEquals(outputAssertion, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWeightedXentWithLogits(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWeightedXentWithLogits(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim targets As INDArray = Nd4j.create(New Long(){1, 5})
			Dim inputs As INDArray = Nd4j.create(New Long(){1, 5})
			Dim weights As INDArray = Nd4j.create(New Long(){1, 5})

			Dim sdInputs As SDVariable = sameDiff_Conflict.var("inputs", inputs)
			Dim sdWeights As SDVariable = sameDiff_Conflict.var("weights", weights)
			Dim sdTargets As SDVariable = sameDiff_Conflict.var("targets", targets)

			Dim res As SDVariable = sameDiff_Conflict.loss().weightedCrossEntropyWithLogits(sdTargets, sdInputs, sdWeights)

			Dim resultArray As INDArray = res.eval()
			assertArrayEquals(New Long(){1, 5}, resultArray.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMseForward(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMseForward(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 3
			Dim input As SDVariable = sd.var("in", New Long(){-1, nOut})
			Dim label As SDVariable = sd.var("label", New Long(){-1, nOut})

			Dim diff As SDVariable = input.sub(label)
			Dim sqDiff As SDVariable = diff.mul(diff)
			Dim msePerEx As SDVariable = sd.mean("msePerEx", sqDiff, 1)
			Dim score As SDVariable = sd.mean("score", msePerEx)

			Dim inputArr As INDArray = Nd4j.rand(minibatch, nOut)
			Dim labelArr As INDArray = Nd4j.rand(minibatch, nOut)

			sd.associateArrayWithVariable(inputArr, input)
			sd.associateArrayWithVariable(labelArr, label)

			Dim result As INDArray = score.eval()
			assertNotNull(result) '*** Fails Here - Null output ***
			assertEquals(1, result.length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistance(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 4, 4)).reshape(ChrW(2), 2)
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim y As SDVariable = sameDiff_Conflict.var("y", arr)
			Dim result As SDVariable = sameDiff_Conflict.math().cosineSimilarity(x, y, 1)
			Dim addResult As SDVariable = result.add(result)
			Dim finalReshape As SDVariable = sameDiff_Conflict.reshape(addResult, 1, 2)
			Dim [out] As IDictionary(Of String, INDArray) = sameDiff_Conflict.output(Collections.emptyMap(), finalReshape.name())
			assertArrayEquals(New Long(){1, 2}, [out](finalReshape.name()).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorGradMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorGradMmul(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 4, 4)).reshape(ChrW(2), 2)
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim y As SDVariable = sameDiff_Conflict.var("y", arr)
			Dim result As SDVariable = sameDiff_Conflict.mmul(x, y)
			Dim otherResult As SDVariable = result.add(result)
			Dim m As IDictionary(Of String, INDArray) = sameDiff_Conflict.outputAll(Nothing)
			assertArrayEquals(New Long(){2, 2}, m(result.name()).shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEval(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4)
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim sigmoid As SDVariable = sameDiff_Conflict.nn().sigmoid("s", x)
			Dim assertion As INDArray = Transforms.sigmoid(arr)
			Dim eval As INDArray = sameDiff_Conflict.output(Collections.singletonMap("x", arr), Collections.singletonList("s"))("s")
			assertEquals(assertion, eval)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFunctionInputsAndArgs(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFunctionInputsAndArgs(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim var As SDVariable = sameDiff_Conflict.var("one", Nd4j.scalar(1.0))
			Dim variable2 As SDVariable = sameDiff_Conflict.var("two", Nd4j.scalar(1.0))
			Dim sum As val = var.add(variable2)
			Dim [out] As INDArray = sum.eval()
			assertArrayEquals(New Long(){}, [out].shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCrossSameDiffVariableInitWithAlloc(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCrossSameDiffVariableInitWithAlloc(ByVal backend As Nd4jBackend)
			Dim first As SameDiff = SameDiff.create()
			Dim second As SameDiff = SameDiff.create()

			Dim firstVar As SDVariable = first.var("one", New Long(){2, 2})
			Dim secondVar As SDVariable = second.var(firstVar)
			assertEquals(firstVar.Arr, secondVar.Arr)
			assertEquals(firstVar.name(), secondVar.name())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCrossSameDiffVariableInitWithPlaceHolder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCrossSameDiffVariableInitWithPlaceHolder(ByVal backend As Nd4jBackend)
			Dim first As SameDiff = SameDiff.create()
			Dim second As SameDiff = SameDiff.create()

			Dim firstVar As SDVariable = first.var("one", New Long(){2, 2})
			Dim secondVar As SDVariable = second.var(firstVar)
			assertNotNull(firstVar.Arr)

			assertEquals(firstVar.Arr, secondVar.Arr)
			assertEquals(firstVar.name(), secondVar.name())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableArrayReference(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableArrayReference(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As SDVariable = sameDiff_Conflict.var("one", New Long(){2, 2})
			assertArrayEquals(New Long(){2, 2}, arr.Shape)
			assertNotNull(arr.Arr)
			assertArrayEquals(New Long(){2, 2}, arr.Arr.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalAddSelf(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalAddSelf(ByVal backend As Nd4jBackend)
			''' <summary>
			''' Note this test fails yet due to needing
			''' to validate simple cases like x * x
			''' matching number of inputs.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4)
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim s As SDVariable = x.mul("s", x)
			Dim assertion As INDArray = arr.mul(arr)
			Dim eval As INDArray = sameDiff_Conflict.output(Collections.singletonMap("x", arr), Collections.singletonList("s"))("s")
			assertEquals(assertion, eval)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalAdd(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4)
			Dim yArr As INDArray = arr.dup()
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim y As SDVariable = sameDiff_Conflict.var("y", yArr)

			Dim sigmoid As SDVariable = x.mul(y)
			Dim assertion As INDArray = arr.mul(arr)
			Dim vars As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			vars("x") = arr
			vars("y") = yArr
			Dim eval As INDArray = sameDiff_Conflict.output(vars, Collections.singletonList(sigmoid.name()))(sigmoid.name())
			assertEquals(assertion, eval)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDup(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDup(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Transforms.sigmoid(Nd4j.linspace(1, 8, 8)).reshape(ChrW(2), 2, 2)
			Dim x As SDVariable = sameDiff_Conflict.var("x", arr)
			Dim y As SDVariable = sameDiff_Conflict.var("y", arr)
			Dim tg2 As SameDiff = sameDiff_Conflict.dup()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testElementWiseDivAndRDiv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testElementWiseDivAndRDiv(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim ones As INDArray = Nd4j.ones(4)
			Dim toDivBy As INDArray = Nd4j.valueArrayOf(4, 0.25)
			Dim xAndY As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			xAndY("x") = ones
			xAndY("y") = toDivBy
			sameDiff_Conflict.defineFunction("div", Function(sameDiff1, inputs, variableInputs)
			Dim x As SDVariable = sameDiff1.var("x", inputs.get("x"))
			Dim y As SDVariable = sameDiff1.var("y", inputs.get("y"))
			Return New SDVariable(){x.div("out", y)}
			End Function, xAndY)

			sameDiff_Conflict.defineFunction("rdiv", Function(sameDiff12, inputs, variableInputs)
			Dim x As SDVariable = sameDiff12.var("x", inputs.get("x"))
			Dim y As SDVariable = sameDiff12.var("y", inputs.get("y"))
			Return New SDVariable(){x.rdiv("out", y)}
			End Function, xAndY)

			Dim assertionForDiv As INDArray = Nd4j.valueArrayOf(4, 4.0)
			Dim assertionForRDiv As INDArray = Nd4j.valueArrayOf(4, 0.25)
			assertEquals(assertionForDiv, sameDiff_Conflict.getFunction("div").outputSingle(Nothing, "out"))
			assertEquals(assertionForRDiv, sameDiff_Conflict.getFunction("rdiv").outputSingle(Nothing, "out"))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNegativeGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNegativeGradient(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim ones As INDArray = Nd4j.ones(4)
			Dim xAndY As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			xAndY("x") = ones
			sameDiff_Conflict.defineFunction("neg", Function(sameDiff1, inputs, variableInputs)
			Dim x As SDVariable = sameDiff1.var("x", inputs.get("x"))
			Return New SDVariable(){sameDiff1.math().neg("out", x)}
			End Function, xAndY)

			Dim assertionForDiv As INDArray = Nd4j.valueArrayOf(4, -1)
			assertEquals(assertionForDiv, sameDiff_Conflict.getFunction("neg").outputSingle(Nothing, "out"))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumOp(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim sumInput As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim inputs As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			inputs("x") = sumInput
			sameDiff_Conflict.defineFunction("sum", Function(sameDiff1, inputs1, variableInputs)
			Dim input As SDVariable = sameDiff1.var("x", inputs1.get("x"))
			Dim sum As SDVariable = sameDiff1.sum("sum", input, 1)
			Return New SDVariable(){sum}
			End Function, inputs)

			Dim assertion As INDArray = sumInput.sum(1)
			Dim [out] As INDArray = sameDiff_Conflict.getFunction("sum").output(Collections.emptyMap(), Collections.singletonList("sum"))("sum")
			assertEquals(assertion, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableReferenceNoFunction(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableReferenceNoFunction(ByVal backend As Nd4jBackend)
			''' <summary>
			''' Creating a variable should not create a differential function.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim sdVariable As SDVariable = sameDiff_Conflict.var("one", Nd4j.scalar(1.0))
			assertNotNull(sameDiff_Conflict.getVariable(sdVariable.name()))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableWithFunction(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableWithFunction(ByVal backend As Nd4jBackend)
			''' <summary>
			''' A variable's function should be null
			''' when just a variable but
			''' have a function result
			''' when the variable itself is the result of a function.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim sdVariable As SDVariable = sameDiff_Conflict.var("one", Nd4j.scalar(1.0))
			Dim add As SDVariable = sdVariable.add(1.0)
			assertEquals(sameDiff_Conflict.getVariable(add.name()), add)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUpdateVariable(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUpdateVariable(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim one As SDVariable = sameDiff_Conflict.one("one", New Long(){1, 1})
			one.rename("one-diff")
			assertEquals(one.eval(), sameDiff_Conflict.getVariable("one-diff").eval())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDefineFunctionArrayExistence(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDefineFunctionArrayExistence(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim testFunctionName As String = "testfunction"
			Dim inputVars() As SDVariable = { sameDiff_Conflict.var("one", New Long(){1, 1}), sameDiff_Conflict.var("two", New Long(){1, 1})}

			Dim functionDef As SameDiff = sameDiff_Conflict.defineFunction(testFunctionName, Function(sameDiff1, inputs, variableInputs) New SDVariable(){variableInputs(0).add(variableInputs(1))}, inputVars)

			'1 input plus 2 outputs
			assertEquals(3, functionDef.variables().Count)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAutoBroadcastAddMatrixVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAutoBroadcastAddMatrixVector(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim row As INDArray = Nd4j.ones(2)
			Dim assertion As INDArray = arr.add(1.0)
			Dim left As SDVariable = sameDiff_Conflict.var("arr", arr)
			Dim right As SDVariable = sameDiff_Conflict.var("row", row)
			Dim test As SDVariable = left.add(right)
			assertEquals(assertion, test.eval())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNegativeOneShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNegativeOneShape(ByVal backend As Nd4jBackend)
			Dim sd As val = SameDiff.create()
			Dim var As SDVariable = sd.placeHolder("test", DataType.FLOAT, -1, 3)
			assertTrue(var.PlaceHolder)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeResolutionMinus1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeResolutionMinus1(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4

			Dim minibatch As Integer = 3

			For Each useMinus1 As Boolean In New Boolean(){False, True}
				log.info("Starting: {}", (If(useMinus1, "minibatch -1", "minibatch 3")))

				Dim inShape() As Long
				If useMinus1 Then
					inShape = New Long(){-1, nIn}
				Else
					inShape = New Long(){minibatch, nIn}
				End If
				Dim wShape As val = New Long(){nIn, nOut}
				Dim bShape As val = New Long(){1, nOut}

				Dim sd As SameDiff = SameDiff.create()
				Dim layerInput As SDVariable = sd.var("in", inShape)
				Dim weights As SDVariable = sd.var("W", wShape)
				Dim bias As SDVariable = sd.var("b", bShape)

				Dim mmul As SDVariable = sd.mmul("mmul", layerInput, weights)
				Dim z As SDVariable = mmul.add("z", bias)
				Dim [out] As SDVariable = sd.nn().sigmoid("out", z)

				Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
				Dim [in] As INDArray = Nd4j.rand(New Long(){minibatch, nIn})
				Dim w As INDArray = Nd4j.rand(wShape)
				Dim b As INDArray = Nd4j.rand(bShape)

				sd.associateArrayWithVariable([in], sd.getVariable("in"))
				assertNotNull(sd.getArrForVarName("in"))
				sd.associateArrayWithVariable(w, sd.getVariable("W"))
				sd.associateArrayWithVariable(b, sd.getVariable("b"))

				Dim outArr As INDArray = [out].eval()

				assertArrayEquals(New Long(){minibatch, nOut}, outArr.shape())
			Next useMinus1
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLabelInputPlaceHolderSgd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLabelInputPlaceHolderSgd(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()

			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim minibatch As Integer = 3
			Dim input As SDVariable = sd.var("in", New Long(){-1, nIn})
			Dim label As SDVariable = sd.var("label", New Long(){-1, nOut})
			assertTrue(input.PlaceHolder)
			assertTrue(label.PlaceHolder)
			Dim weights As SDVariable = sd.var("W", New Long(){nIn, nOut})
			Dim bias As SDVariable = sd.var("b", New Long(){1, nOut})

			Dim mmul As SDVariable = sd.mmul("mmul", input, weights)
			Dim z As SDVariable = mmul.add("z", bias)
			Dim [out] As SDVariable = sd.math().tanh(z)

			Dim diff As SDVariable = [out].sub(label)
			Dim sqDiff As SDVariable = diff.mul(diff)
			Dim msePerEx As SDVariable = sd.mean("msePerEx", sqDiff, 1)
			Dim avgMSE As SDVariable = sd.mean("loss", msePerEx, 0)

			Dim inputArr As INDArray = Nd4j.rand(minibatch, nIn)
			Dim labelArr As INDArray = Nd4j.rand(minibatch, nOut)
			Dim weightsArr As INDArray = Nd4j.rand(nIn, nOut)
			Dim biasArr As INDArray = Nd4j.rand(1, nOut)

			sd.associateArrayWithVariable(inputArr, input)
			sd.associateArrayWithVariable(labelArr, label)
			sd.associateArrayWithVariable(weightsArr, weights)
			sd.associateArrayWithVariable(biasArr, bias)

			Dim result As INDArray = avgMSE.eval()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSequentialMeansPlaceholder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSequentialMeansPlaceholder(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing()
			For Each dim0 As Integer In New Integer(){10, -1}
				Dim msg As String = "Dimension 0 = " & dim0
				Console.WriteLine(msg)
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", New Long(){dim0, 9, 8})
				Dim mean1 As SDVariable = sd.mean([in], 2) '[10,9,8] -> [10,9]
				Dim mean2 As SDVariable = sd.mean(mean1, 1) '[10,9] -> [10]

				Dim inArr As INDArray = Nd4j.create(10, 9, 8)
				sd.associateArrayWithVariable(inArr, [in])

				Dim [out] As INDArray = mean2.eval()

				Dim shape() As Long = [out].shape()
				assertArrayEquals(New Long(){10}, shape,msg)
			Next dim0
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionShapes1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionShapes1(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", New Long(){10, 9, 8})
			Dim mean1 As SDVariable = sd.mean([in], 2) '[10,9] out
			Dim mean2 As SDVariable = sd.mean(mean1, 1) '[10] out
			Dim m As IDictionary(Of String, INDArray) = sd.output(DirectCast(Nothing, IDictionary(Of String, INDArray)), mean1.name(), mean2.name())

			Dim m1 As INDArray = m(mean1.name())
			Dim m2 As INDArray = m(mean2.name())

			assertArrayEquals(New Long(){10, 9}, m1.shape())
			assertArrayEquals(New Long(){10}, m2.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReductionShapes2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReductionShapes2(ByVal backend As Nd4jBackend)

			Dim sd2 As SameDiff = SameDiff.create()
			Dim in2 As SDVariable = sd2.var("in", New Long(){10, 9, 8})
			Dim meanA As SDVariable = sd2.mean(in2, 0) '[9,8] out
			Dim [out] As IDictionary(Of String, INDArray) = sd2.outputAll(Nothing)
			assertArrayEquals(New Long(){9, 8}, [out](meanA.name()).shape())

			Dim meanB As SDVariable = sd2.mean(meanA, 0) '[8] out
			Dim m As IDictionary(Of String, INDArray) = sd2.outputAll(Nothing)
			assertArrayEquals(New Long(){8}, m(meanB.name()).shape())

			assertArrayEquals(New Long(){9, 8}, m(meanA.name()).shape())
			assertArrayEquals(New Long(){8}, m(meanB.name()).shape())

			m = sd2.outputAll(Nothing)

			Dim mA As INDArray = m(meanA.name())
			Dim mB As INDArray = m(meanB.name())

			assertArrayEquals(New Long(){9, 8}, mA.shape())
			assertArrayEquals(New Long(){8}, mB.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNames(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNames(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim in1 As SDVariable = sd.var("in", New Long(){3, 2})
			Dim in2 As SDVariable = sd.var("in2", New Long(){3, 3})

			Dim m As val = in1.add(1.0)
			Dim f As val = m.add(2.0)
			Dim s As val = in2.add(5.0)

			Dim map As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)
	'        log.info("Result M: {}", map.get(m.name()));
	'        log.info("Result F: {}", map.get(f.name()));
	'        log.info("Result S: {}", map.get(s.name()));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRunLogisticRegression(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRunLogisticRegression(ByVal backend As Nd4jBackend)
			Dim vars As IDictionary(Of String, INDArray) = Me.variablesForInput()
			Dim outside As SameDiff = SameDiff.create()
			outside.defineFunction("activate", Function(sameDiff, inputs, variableInputs)
			sameDiff.enableDebugMode()
			Dim x As SDVariable = sameDiff.var("x", inputs.get("x"))
			Dim w As SDVariable = sameDiff.var("w", inputs.get("w"))
			Dim y As SDVariable = sameDiff.var("y", inputs.get("y"))
			Dim activation As SDVariable = sameDiff.nn().sigmoid("activation", sameDiff.mmul("mmul", x, w))
			Dim oneMinusY As SDVariable = y.rsub("oneminusy", 1.0)
			Dim oneMinusPredictions As SDVariable = activation.rsub("oneminusactivations", 1.0)
			Dim outputTimesY As SDVariable = y.mul("output * y", activation)
			Dim yHat As SDVariable = oneMinusPredictions.mul("yhat", oneMinusY)
			Dim probs As SDVariable = outputTimesY.add("probs", yHat)
			Dim logProbs As SDVariable = sameDiff.math().log("logprob", probs)
			Dim ret As SDVariable = sameDiff.sum("totalsum", logProbs, Integer.MaxValue)
			Dim ret2 As SDVariable = sameDiff.math().neg("negtotalsum", ret)
			Return New SDVariable(){ret2}
			End Function, vars)

			Dim activation As SameDiff = outside.getFunction("activate")
			Dim epochsToRun As Integer = 5
			Dim lr As Double = 0.1
	'        for(int i = 0; i < epochsToRun; i++) {
	'            activation.execBackwards();
	'            INDArray wGrad = activation.grad("w").getArr().reshape(vars.get("w").shape());
	'            vars.get("w").subi(wGrad.mul(lr));
	'            System.out.println("Score: " + activation.getVariable("negtotalsum").getArr());
	'        }

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTransposeWithVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTransposeWithVector(ByVal backend As Nd4jBackend)
			Dim sd As val = SameDiff.create()
			Dim matrix As val = Nd4j.linspace(1, 12, 12).reshape(ChrW(4), 3)
			Dim vector As val = Nd4j.linspace(1, 4, 4).reshape(ChrW(4), 1)
			Dim input1 As val = sd.var("input", matrix)
			Dim input2 As val = sd.var("input2", vector)
			Dim output As val = sd.mmul("output", input1, input2, True, False, False)
			Dim [out] As INDArray = output.eval()
			assertArrayEquals(New Long(){3, 1}, [out].shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimpleDefineFunction(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleDefineFunction(ByVal backend As Nd4jBackend)
			Dim sameDiffOuter As SameDiff = SameDiff.create()
			Dim inputs As IDictionary(Of String, INDArray) = variablesForInput()
			inputs.Remove("y")
			Dim logisticForward As String = "logisticPredictions"
			sameDiffOuter.defineFunction(logisticForward, Function(sameDiff, inputs1, variableInputs)
			Dim input As SDVariable = sameDiff.var("x", inputs1.get("x"))
			Dim w As SDVariable = sameDiff.var("w", inputs1.get("w"))
			Dim preOutput As SDVariable = sameDiff.mmul(input, w)
			Dim sigmoid As SDVariable = sameDiff.nn().sigmoid(preOutput)
			Return New SDVariable(){sigmoid}
			End Function, inputs)

			assertEquals(1, sameDiffOuter.definedFunctionNames().Count)

			'note here that we don't add the duplicate ops with define function anymore
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumGradient(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim twoByTwo As SDVariable = sameDiff_Conflict.var("initial", Nd4j.linspace(1, 4, 4, DataType.FLOAT).reshape(ChrW(2), 2))
			Dim sum As SDVariable = sameDiff_Conflict.sum(twoByTwo, Integer.MaxValue)
			Dim grads As IDictionary(Of String, INDArray) = sameDiff_Conflict.calculateGradients(Collections.emptyMap(), sameDiff_Conflict.getVariables().keySet())
			assertEquals(Nd4j.ones(DataType.FLOAT, 2, 2), grads(twoByTwo.name()))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRsubScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRsubScalar(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim params As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim var As INDArray = Nd4j.valueArrayOf(4, 2)
			params("x") = var
			sameDiff_Conflict.defineFunction("rsubop", Function(sameDiff1, inputs, variableInputs)
			Dim input As SDVariable = sameDiff1.var("x", inputs.get("x"))
			Dim ret As SDVariable = input.rsub("rsub", 1.0)
			Return New SDVariable(){ret}
			End Function, params)

			Dim logisticGraph As SameDiff = sameDiff_Conflict.getFunction("rsubop")
			Dim output As INDArray = logisticGraph.output(params, Collections.singletonList("rsub"))("rsub")
			assertEquals(Nd4j.ones(4).muli(-1), output)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFunctionScalarResultPropagation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFunctionScalarResultPropagation(ByVal backend As Nd4jBackend)
			Dim sameDiffOuter As SameDiff = SameDiff.create()
			Dim inputs As IDictionary(Of String, INDArray) = variablesForInput()

			sameDiffOuter.defineFunction("logisticPredictions", Function(sameDiff, inputs12, variableInputs)
			Dim input As SDVariable = sameDiff.var("x", inputs12.get("x"))
			Dim w As SDVariable = sameDiff.var("w", inputs12.get("w"))
			Dim preOutput As SDVariable = sameDiff.mmul(input, w)
			Dim sigmoid As SDVariable = sameDiff.nn().sigmoid(preOutput)
			Return New SDVariable(){sigmoid}
			End Function, inputs)

			sameDiffOuter.defineFunction("oneminuspredictions", Function(sameDiff, inputs1, variableInputs)
			Dim y As SDVariable = sameDiff.var("y", inputs1.get("y"))
			Dim oneMinusPredictions As SDVariable = y.rsub("rsub", 1.0)
			Return New SDVariable(){oneMinusPredictions}
			End Function, inputs)

			Dim logisticGraph As SameDiff = sameDiffOuter.getFunction("oneminuspredictions")
			Dim inputsSubset As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			inputsSubset("y") = inputs("y")
			Dim output As INDArray = logisticGraph.output(inputsSubset, Collections.singletonList("rsub"))("rsub")
			Dim assertion As INDArray = Nd4j.create(New Double(){0, 0, 1, 0}, New Integer(){4, 1})
			assertEquals(assertion, output)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmul(ByVal backend As Nd4jBackend)
			Dim sameDiffOuter As SameDiff = SameDiff.create()
			Dim inputs As IDictionary(Of String, INDArray) = variablesForInput()
			Dim x As SDVariable = sameDiffOuter.var("x", inputs("x"))
			Dim w As SDVariable = sameDiffOuter.var("w", inputs("w"))
			Dim output As SDVariable = sameDiffOuter.mmul(x, w)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGraphBuilding(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGraphBuilding(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SameDiff sameDiffOuter = SameDiff.create();
			Dim sameDiffOuter As SameDiff = SameDiff.create()
			Dim inputs As IDictionary(Of String, INDArray) = variablesForInput()

			sameDiffOuter.defineFunction("logisticPredictions", Function(sameDiff, inputs1, variableInputs)
			Dim input As SDVariable = sameDiff.var("x", inputs1.get("x"))
			Dim w As SDVariable = sameDiff.var("w", inputs1.get("w"))
			Dim y As SDVariable = sameDiff.var("y", inputs1.get("y"))
			Dim preOutput As SDVariable = sameDiff.mmul(input, w)
			Dim sigmoid As SDVariable = sameDiff.nn().sigmoid(preOutput)
			Return New SDVariable(){sigmoid}
			End Function, inputs)

			sameDiffOuter.defineFunction("loss", Function(sameDiff, inputs12, variableInputs)
			Dim outputs As SDVariable = sameDiffOuter.invokeFunctionOn("logisticPredictions", sameDiff)
			Dim y As SDVariable = sameDiff.getVariable("y")
			Dim outputTimesY As SDVariable = outputs.mul(y)
			Return New SDVariable(){outputTimesY}
			End Function, inputs)

			Dim logisticPrediction As SameDiff = sameDiffOuter.getFunction("logisticPredictions")
			Dim logisticOpNameAssertions As IList(Of String) = Arrays.asList("mmul", "sigmoid")


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarAdd(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim twoByTwo As SDVariable = sameDiff_Conflict.var("first", Nd4j.linspace(1, 4, 4).reshape("c"c, 2, 2))
			Dim add As SDVariable = twoByTwo.add(1.0)
			Dim test As INDArray = add.eval()
			Dim assertion As INDArray = Nd4j.linspace(1, 4, 4).reshape("c"c, 2, 2).add(1.0)
			assertEquals(assertion, test)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSums(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSums(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = SameDiff.create()
			Dim ones As INDArray = Nd4j.ones(7, 4)
			Dim sdVariable As SDVariable = sameDiff_Conflict.var("ones", ones)
			Dim result As SDVariable = sdVariable.add(1.0)
			Dim total As SDVariable = sameDiff_Conflict.sum(result, Integer.MaxValue)
			Dim [out] As INDArray = total.eval()
			assertEquals(56, [out].getDouble(0), 1e-1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDenseLayerForwardPass(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDenseLayerForwardPass(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sd As SameDiff = SameDiff.create()

			Dim iInput As INDArray = Nd4j.rand(3, 4)
			Dim iWeights As INDArray = Nd4j.rand(4, 5)
			Dim iBias As INDArray = Nd4j.rand(1, 5)

			Dim input As SDVariable = sd.var("input", iInput)
			Dim weights As SDVariable = sd.var("weights", iWeights)
			Dim bias As SDVariable = sd.var("bias", iBias)

			Dim mmul As SDVariable = sd.mmul("mmul", input, weights)
			Dim z As SDVariable = mmul.add("z", bias)
			Dim [out] As SDVariable = sd.nn().sigmoid("out", z)

			Dim expMmul As INDArray = iInput.mmul(iWeights)
			Dim expZ As INDArray = expMmul.addRowVector(iBias)
			Dim expOut As INDArray = Transforms.sigmoid(expZ, True)

			Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Collections.emptyMap())

			assertEquals(expMmul, m(mmul.name()))
			assertEquals(expZ, m(z.name()))
			assertEquals(expOut, m([out].name()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testActivationBackprop(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testActivationBackprop(ByVal backend As Nd4jBackend)

			Dim afns() As Activation = { Activation.TANH, Activation.SIGMOID, Activation.ELU, Activation.SOFTPLUS, Activation.SOFTSIGN, Activation.HARDTANH, Activation.CUBE, Activation.RELU, Activation.LEAKYRELU }

			For Each a As Activation In afns

				Dim sd As SameDiff = SameDiff.create()
				Dim inArr As INDArray = Nd4j.linspace(-3, 3, 7)
				Dim labelArr As INDArray = Nd4j.linspace(-3, 3, 7).muli(0.5)
				Dim [in] As SDVariable = sd.var("in", inArr.dup())

	'            System.out.println("inArr: " + inArr);

				Dim outExp As INDArray
				Dim [out] As SDVariable
				Select Case a.innerEnumValue
					Case Activation.InnerEnum.ELU
						[out] = sd.nn().elu("out", [in])
						outExp = Transforms.elu(inArr, True)
					Case Activation.InnerEnum.HARDTANH
						[out] = sd.nn().hardTanh("out", [in])
						outExp = Transforms.hardTanh(inArr, True)
					Case Activation.InnerEnum.LEAKYRELU
						[out] = sd.nn().leakyRelu("out", [in], 0.01)
						outExp = Transforms.leakyRelu(inArr, True)
					Case Activation.InnerEnum.RELU
						[out] = sd.nn().relu("out", [in], 0.0)
						outExp = Transforms.relu(inArr, True)
					Case Activation.InnerEnum.SIGMOID
						[out] = sd.nn().sigmoid("out", [in])
						outExp = Transforms.sigmoid(inArr, True)
					Case Activation.InnerEnum.SOFTPLUS
						[out] = sd.nn().softplus("out", [in])
						outExp = Transforms.softPlus(inArr, True)
					Case Activation.InnerEnum.SOFTSIGN
						[out] = sd.nn().softsign("out", [in])
						outExp = Transforms.softsign(inArr, True)
					Case Activation.InnerEnum.TANH
						[out] = sd.math().tanh("out", [in])
						outExp = Transforms.tanh(inArr, True)
					Case Activation.InnerEnum.CUBE
						[out] = sd.math().cube("out", [in])
						outExp = Transforms.pow(inArr, 3, True)
					Case Else
						Throw New Exception(a.ToString())
				End Select

				'Sum squared error loss:
				Dim label As SDVariable = sd.var("label", labelArr.dup())
				Dim diff As SDVariable = label.sub("diff", [out])
				Dim sqDiff As SDVariable = diff.mul("sqDiff", diff)
				Dim totSum As SDVariable = sd.sum("totSum", sqDiff, Integer.MaxValue) 'Loss function...

				Dim m As IDictionary(Of String, INDArray) = sd.output(Collections.emptyMap(), "out")
				Dim outAct As INDArray = m("out")
				assertEquals(outExp, outAct,a.ToString())

				' L = sum_i (label - out)^2
				'dL/dOut = 2(out - label)
				Dim dLdOutExp As INDArray = outExp.sub(labelArr).mul(2)
				Dim dLdInExp As INDArray = a.getActivationFunction().backprop(inArr.dup(), dLdOutExp.dup()).getFirst()

				Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, "out", "in")
	'            sd.execBackwards(Collections.emptyMap());
	'            SameDiff gradFn = sd.getFunction("grad");
				Dim dLdOutAct As INDArray = grads("out")
				Dim dLdInAct As INDArray = grads("in")

				assertEquals(dLdOutExp, dLdOutAct,a.ToString())
				assertEquals(dLdInExp, dLdInAct,a.ToString())
			Next a
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPlaceholderReduceSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPlaceholderReduceSimple(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("in", New Long(){-1, 10})
			Dim vSum As SDVariable = sd.sum(v, 1) 'Exception here
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSequentialMeans(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSequentialMeans(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", New Long(){10, 10, 10})
			Dim mean1 As SDVariable = sd.mean([in], 2) '[10,10] out
			Dim mean2 As SDVariable = sd.mean(mean1, 1) '[10,1] out - ***exception here***
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBatchNormTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchNormTest(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim input As INDArray = Nd4j.rand(1, 10)
			Dim mean As INDArray = Nd4j.rand(1, 10).reshape(10)
			Dim var As INDArray = Nd4j.rand(1, 10).reshape(10)
			Dim gamma As INDArray = Nd4j.rand(1, 10).reshape(10)
			Dim beta As INDArray = Nd4j.rand(1, 10).reshape(10)

			Dim sdInput As SDVariable = sd.var("input", input)
			Dim sdMean As SDVariable = sd.var("mean", mean)
			Dim sdVar As SDVariable = sd.var("var", var)
			Dim sdGamma As SDVariable = sd.var("gamma", gamma)
			Dim sdBeta As SDVariable = sd.var("beta", beta)

			Dim [out] As SDVariable = sd.nn().batchNorm(sdInput, sdMean, sdVar, sdGamma, sdBeta, 0.0, 1)
			[out] = sd.math().tanh([out])

			Dim outArr As INDArray = [out].eval()
			assertArrayEquals(New Long(){1, 10}, outArr.shape())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLrn(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLrn(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim input As INDArray = Nd4j.create(New Single(){4, 4, 4, 4}, New Long(){1, 4, 1, 1})

			Dim sdInput As SDVariable = sd.var("input", input)

			Dim lrn As LocalResponseNormalizationConfig = LocalResponseNormalizationConfig.builder().alpha(1.0).beta(.5).bias(0.0).depth(1).build()

			Dim [out] As SDVariable = sd.cnn().localResponseNormalization(sdInput, lrn)
			Dim sdOut As SDVariable = sd.math().tanh("out", [out])

			Dim map As IDictionary(Of String, INDArray) = sd.output(Collections.emptyMap(), "out", [out].name())

			For i As Integer = 0 To 3
				assertEquals(1, map([out].name()).get(all(), NDArrayIndex.point(i), all(), all()).getInt(0))
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMoments(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMoments(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim input As INDArray = Nd4j.create(New Single(){1, 2, 3, 4}, New Long(){2, 2})

			Dim sdInput As SDVariable = sd.var("input", input)

			Dim moments() As SDVariable = sd.math().moments(sdInput, 0, 1)
			Dim mean As SDVariable = moments(0)
			Dim variance As SDVariable = moments(1)

			Dim sum As SDVariable = mean.add(variance)
			Dim [out] As SDVariable = sd.math().tanh("out", sum)

			Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)

			Dim meanArray As INDArray = m(mean.name())
			Dim varArray As INDArray = m(variance.name())

			assertEquals(meanArray.getDouble(0), 2.5, 1e-5)
			assertEquals(varArray.getDouble(0), 1.25, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizeMoments(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormalizeMoments(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim counts As INDArray = Nd4j.create(New Single(){2}, New Long(){1, 1})
			Dim means As INDArray = Nd4j.create(New Single(){2, 4}, New Long(){1, 2})
			Dim vars As INDArray = Nd4j.create(New Single(){6, 8}, New Long(){1, 2})

			Dim sdCounts As SDVariable = sd.var("counts", counts)
			Dim sdMeans As SDVariable = sd.var("means", means)
			Dim sdVars As SDVariable = sd.var("vars", vars)
			Dim shift As Double = 0.0

			Dim moments() As SDVariable = sd.math().normalizeMoments(sdCounts, sdMeans, sdVars, shift)
			Dim normMean As SDVariable = moments(0)
			Dim normVariance As SDVariable = moments(1)

			Dim sum As SDVariable = normMean.add(normVariance)
			Dim [out] As SDVariable = sd.math().tanh("out", sum)

			Dim m As IDictionary(Of String, INDArray) = sd.outputAll(Nothing)

			Dim meanArray As INDArray = m(normMean.name())
			Dim varArray As INDArray = m(normVariance.name())

			assertEquals(meanArray.getDouble(0, 0), 1, 1e-5)
			assertEquals(meanArray.getDouble(0, 1), 2, 1e-5)
			assertArrayEquals(meanArray.shape(), varArray.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDepthWiseConv2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDepthWiseConv2dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim depthWise As Integer = 4
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 28
			Dim imgW As Integer = 28

			Dim sd As SameDiff = SameDiff.create()
			Dim depthWeightArr As INDArray = Nd4j.create(kH, kW, nIn, depthWise)

			Dim bArr As INDArray = Nd4j.create(1, depthWise * nIn)
			Dim inArr As INDArray = Nd4j.create(mb, nIn, imgH, imgW)

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim dW As SDVariable = sd.var("dW", depthWeightArr)
			Dim b As SDVariable = sd.var("b", bArr)

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).build()

			Dim [out] As SDVariable = sd.cnn().depthWiseConv2d([in], dW, b, c)
			[out] = sd.math().tanh("out", [out])

			Dim outArr As INDArray = [out].eval()
			'Expected output size: out = (in - k + 2*p)/s + 1 = (28-2+0)/1+1 = 27
			Dim outShape As val = outArr.shape()
			assertArrayEquals(New Long(){mb, depthWise * nIn, 27, 27}, outShape)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateMeanDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateMeanDiff(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim arr As INDArray = Nd4j.rand(3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("in", arr)
			Dim mean As SDVariable = sd.mean("mean", v)

			Dim [out] As INDArray = mean.eval()
			assertEquals([out], arr.mean(Integer.MaxValue))

			Dim m As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
			Dim dLdIn As INDArray = m("in")

			'If L = mean(in)
			'then dL/dIn = 1/N

			assertEquals(Nd4j.valueArrayOf(arr.shape(), 1.0 / arr.length()), dLdIn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateSumDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateSumDiff(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim arr As INDArray = Nd4j.rand(3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("in", arr)
			Dim mean As SDVariable = sd.sum("sum", v)

			Dim [out] As INDArray = mean.eval()
			assertEquals([out], arr.sum(Integer.MaxValue))

			Dim m As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
			Dim dLdIn As INDArray = m("in")

			'If L = sum(in)
			'then dL/dIn = 1

			assertEquals(Nd4j.ones(arr.shape()), dLdIn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateStdevDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateStdevDiff(ByVal backend As Nd4jBackend)
			For Each biasCorrected As Boolean In New Boolean(){True, False}
				Nd4j.Random.setSeed(12345)

				Dim arr As INDArray = Nd4j.rand(3, 4)

				Dim sd As SameDiff = SameDiff.create()
				Dim v As SDVariable = sd.var("in", arr)
				Dim stdev As SDVariable = sd.standardDeviation("stdev", v, biasCorrected)

				Dim [out] As INDArray = stdev.eval()
				assertEquals([out], arr.std(biasCorrected, Integer.MaxValue))

				Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
				Dim dLdIn As INDArray = sd.grad("in").Arr

				'If L = stdev(in)
				'then dL/dIn = (in-mean) / (s*(N-1))
				' or /N for non-bias corrected

				Dim m As Double = arr.meanNumber().doubleValue()
				Dim s As Double = arr.stdNumber(biasCorrected).doubleValue()
				Dim exp As INDArray = arr.sub(m).div(s)
				exp.divi(If(biasCorrected, arr.length() - 1, arr.length()))

				assertEquals(exp, dLdIn)
			Next biasCorrected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateVarDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateVarDiff(ByVal backend As Nd4jBackend)
			For Each biasCorrected As Boolean In New Boolean(){True, False}
				Nd4j.Random.setSeed(12345)

				Dim arr As INDArray = Nd4j.rand(3, 4)

				Dim sd As SameDiff = SameDiff.create()
				Dim v As SDVariable = sd.var("in", arr)
				Dim var As SDVariable = sd.variance("var", v, biasCorrected)

				Dim [out] As INDArray = var.eval()
				assertEquals([out], arr.var(biasCorrected, Integer.MaxValue))

				Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
				Dim dLdIn As INDArray = g("in")

				'If L = var(in)
				'then dL/dIn = 2/(N-1) * (in-mean)
				' or /N for non-bias corrected

				Dim m As Double = arr.meanNumber().doubleValue()
				Dim exp As INDArray = arr.sub(m).mul(2)
				exp.divi(If(biasCorrected, arr.length() - 1, arr.length()))

				assertEquals(exp, dLdIn)
			Next biasCorrected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateMinDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateMinDiff(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim arr As INDArray = Nd4j.rand(3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("in", arr)
			Dim min As SDVariable = sd.min("min", v)

			Dim [out] As INDArray = min.eval()
			assertEquals([out], arr.min(Integer.MaxValue))

			Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
			Dim dLdIn As INDArray = sd.grad("in").Arr

			'If L = min(in)
			'then dL/dIn = 1 if in_i == min(in) or 0 otherwise

			'Note that we don't have an "IsMin" op, so use IsMax(neg(in)) which is equivalent
			Dim exp As INDArray = Nd4j.exec(New IsMax(arr.neg()))(0).castTo(Nd4j.defaultFloatingPointType())

			assertEquals(exp, dLdIn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateMaxDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateMaxDiff(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim arr As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("in", arr)
			Dim min As SDVariable = sd.max("max", v)

			Dim [out] As INDArray = min.eval()
			assertEquals([out], arr.max(Integer.MaxValue))

			sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
			Dim dLdIn As INDArray = sd.grad("in").Arr

			'If L = max(in)
			'then dL/dIn = 1 if in_i == max(in) or 0 otherwise

			Dim exp As INDArray = Nd4j.exec(New IsMax(arr.dup()))(0).castTo(DataType.DOUBLE)

			assertEquals(exp, dLdIn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void validateProdDiff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateProdDiff(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim arr As INDArray = Nd4j.rand(3, 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("in", arr)
			Dim prod As SDVariable = sd.prod("prod", v)

			Dim p As Double = arr.prodNumber().doubleValue()
			Dim [out] As INDArray = prod.eval()
			assertEquals([out], arr.prod(Integer.MaxValue))

			Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())
			Dim dLdIn As INDArray = sd.grad("in").Arr

			'If L = prod(in)
			'then dL/dIn = prod(in) / in       i.e., product of input *excluding* in_i as (d/dx(xyzabc) = yzabc

			Dim exp As INDArray = arr.rdiv(p)
			assertEquals(exp, dLdIn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSquare(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSquare(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim mb As Integer = 5
			Dim nOut As Integer = 4

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.rand(mb, nOut))
			Dim label As SDVariable = sd.var("label", Nd4j.rand(mb, nOut))
			Dim diff As SDVariable = [in].sub(label)
			Dim sqDiff As SDVariable = sd.math().square(diff)

			Dim expOut As INDArray = [in].Arr.sub(label.Arr)
			expOut.muli(expOut)

			Dim [out] As INDArray = sqDiff.eval()

			assertEquals([out], expOut)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExpandDims(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExpandDims(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 2
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", Nd4j.create(2, 3))
				Dim expanded As SDVariable = sd.expandDims([in], i)

				Dim [out] As INDArray = expanded.eval()
				Select Case i
					Case 0
						assertArrayEquals(New Long(){1, 2, 3}, [out].shape())
					Case 1
						assertArrayEquals(New Long(){2, 1, 3}, [out].shape())
					Case 2
						assertArrayEquals(New Long(){2, 3, 1}, [out].shape())
					Case Else
						Throw New Exception()
				End Select
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZerosLike(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZerosLike(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim var0 As SDVariable = sd.var("in", DataType.DOUBLE, New Long(){3, 4})
			Dim [out] As SDVariable = sd.zerosLike("out", var0)

			Dim out1 As INDArray = [out].eval()
			assertEquals(Nd4j.zeros(3, 4), out1)

			sd.associateArrayWithVariable(Nd4j.create(3, 4), var0)
			Dim out2 As INDArray = [out].eval()
			assertEquals(Nd4j.zeros(DataType.DOUBLE, 3, 4), out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOnesLike(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOnesLike(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim var0 As SDVariable = sd.var("in", New Long(){3, 4})
			Dim [out] As SDVariable = sd.onesLike("out", var0)

			Dim out1 As INDArray = [out].eval()
			assertEquals(Nd4j.ones(3, 4), out1)

			sd.associateArrayWithVariable(Nd4j.create(3, 4), var0)
			Dim out2 As INDArray = [out].eval()
			assertEquals(Nd4j.ones(3, 4), out2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOnesLikeBackprop(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOnesLikeBackprop(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim var0 As SDVariable = sd.var("in", New Long(){3, 4})
			Dim ones As SDVariable = sd.onesLike("ones", var0)
			Dim [out] As SDVariable = sd.sum("oun", ones)

			Dim outArr As INDArray = [out].eval()
			assertEquals(Nd4j.scalar(12.0), outArr)

			Dim m As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())

			assertEquals(Nd4j.create(3, 4), m("in"))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testManhattanAlongDim0(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testManhattanAlongDim0(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim a As INDArray = Nd4j.rand(New Long(){3, 4, 5})
			Dim b As INDArray = Nd4j.rand(New Long(){3, 4, 5})

			Dim expOut As INDArray = Nd4j.exec(New ManhattanDistance(a, b, 0))

			Dim expShape As val = New Long(){4, 5}

			assertArrayEquals(expShape, expOut.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJaccardDistance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJaccardDistance(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim a As INDArray = Nd4j.rand(New Long(){3, 4}).addi(0.1)
			Dim b As INDArray = Nd4j.rand(New Long(){3, 4}).addi(0.1)


			Dim sd As SameDiff = SameDiff.create()
			Dim in1 As SDVariable = sd.var("in1", a)
			Dim in2 As SDVariable = sd.var("in2", b)

			Dim jaccard As SDVariable = sd.math().jaccardDistance("out", in1, in2)

			Dim min As INDArray = Transforms.min(a, b)
			Dim max As INDArray = Transforms.max(a, b)

			Dim minSum As Double = min.sumNumber().doubleValue()
			Dim maxSum As Double = max.sumNumber().doubleValue()
			Dim jd As Double = 1.0 - minSum / maxSum

			Dim [out] As INDArray = jaccard.eval()
			assertEquals(1, [out].length())

			assertEquals(jd, [out].getDouble(0), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPairwiseBooleanTransforms(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPairwiseBooleanTransforms(ByVal backend As Nd4jBackend)
	'        
	'        eq, neq, gt, lt, gte, lte, or, and, xor
	'         
			'Test transforms (pairwise)
			Nd4j.Random.setSeed(12345)

			For i As Integer = 0 To 10
				Dim sd As SameDiff = SameDiff.create()

				Dim nOut As Integer = 4
				Dim minibatch As Integer = 5

				Dim ia As INDArray = Nd4j.randn(minibatch, nOut)
				Dim ib As INDArray = Nd4j.randn(minibatch, nOut)

				Dim in1 As SDVariable = sd.var("in1", ia)
				Dim in2 As SDVariable = sd.var("in2", ib)

				Dim t As SDVariable
				Dim expOut As INDArray
				Select Case i
					Case 0
						t = sd.eq(in1, in2)
						expOut = ia.eq(ib)
					Case 1
						t = sd.neq(in1, in2)
						expOut = ia.neq(ib)
					Case 2
						t = sd.gt(in1, in2)
						expOut = ia.gt(ib)
					Case 3
						t = sd.lt(in1, in2)
						expOut = ia.lt(ib)
					Case 4
						t = sd.gte(in1, in2)
						expOut = Nd4j.create(DataType.BOOL, ia.shape())
						Nd4j.exec(New GreaterThanOrEqual(New INDArray(){ia, ib}, New INDArray(){expOut}))
					Case 5
						t = sd.lte(in1, in2)
						expOut = Nd4j.create(DataType.BOOL, ia.shape())
						Nd4j.exec(New LessThanOrEqual(New INDArray(){ia, ib}, New INDArray(){expOut}))
					Case 6
						ia = Nd4j.exec(New BernoulliDistribution(ia, 0.5))
						ib = Nd4j.exec(New BernoulliDistribution(ib, 0.5))
						t = sd.math().or(in1.castTo(DataType.BOOL), in2.castTo(DataType.BOOL))
						expOut = Transforms.or(ia, ib)
					Case 7
						t = sd.max(in1, in2)
						expOut = Nd4j.exec(New Max(ia, ib, ia.dup()))(0)
					Case 8
						t = sd.min(in1, in2)
						expOut = Nd4j.exec(New Min(ia, ib, ia.dup()))(0)
					Case 9
						ia = Nd4j.exec(New BernoulliDistribution(ia, 0.5))
						ib = Nd4j.exec(New BernoulliDistribution(ib, 0.5))
						t = sd.math().and(in1.castTo(DataType.BOOL), in2.castTo(DataType.BOOL))
						expOut = Transforms.and(ia, ib)
					Case 10
						ia = Nd4j.exec(New BernoulliDistribution(ia, 0.5))
						ib = Nd4j.exec(New BernoulliDistribution(ib, 0.5))
						t = sd.math().xor(in1.castTo(DataType.BOOL), in2.castTo(DataType.BOOL))
						expOut = Transforms.xor(ia, ib)
					Case Else
						Throw New Exception()
				End Select

				log.info("Executing: " & i)
				Dim [out] As INDArray = t.eval()

				assertEquals(expOut, [out])
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBooleanChecks(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBooleanChecks(ByVal backend As Nd4jBackend)
	'        
	'        isNonDecreasing,
	'         
			Nd4j.Random.setSeed(12345)

			For i As Integer = 0 To 2
				Dim sd As SameDiff = SameDiff.create()

				Dim nOut As Integer = 4
				Dim minibatch As Integer = 5

				Dim ia As INDArray = Nd4j.randn(minibatch, nOut)

				Dim in1 As SDVariable = sd.var("in1", ia)
				Dim expOut As INDArray = Nd4j.scalar(True)
				Dim t As SDVariable

				Select Case i
					Case 0
						t = sd.math().isNonDecreasing(in1)
						Nd4j.exec(New IsNonDecreasing(ia, expOut))
					Case 1
						t = sd.math().isStrictlyIncreasing(in1)
						Nd4j.exec(New IsStrictlyIncreasing(ia, expOut))
					Case 2
						t = sd.isNumericTensor(in1)
						Nd4j.exec(New IsNumericTensor(New INDArray(){ia}, New INDArray(){expOut}))
					Case Else
						Throw New Exception()
				End Select

				log.info("Executing: " & i)
				Dim [out] As INDArray = t.eval()

				assertEquals(expOut, [out])
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled() @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsStrictlyIncShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsStrictlyIncShape(ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 0
			Dim minibatch As Integer = 0

			Dim ia As INDArray = Nd4j.randn(minibatch, nOut)
			Dim expOut As INDArray = Nd4j.create(DataType.BOOL, ia.shape())

			Nd4j.exec(New IsStrictlyIncreasing(ia, expOut))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExpandDims2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExpandDims2d(ByVal backend As Nd4jBackend)
			Dim origShape As val = New Long(){3, 4}

			For i As Integer = 0 To 2
				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAllTestMatricesWithShape(origShape(0), origShape(1), 12345, DataType.FLOAT)
					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim expand As SDVariable = sd.expandDims([in], i)

					Dim [out] As INDArray = expand.eval()

					Dim expOut As INDArray
					Select Case i
						Case 0
							expOut = inArr.dup("c"c).reshape("c"c, 1, origShape(0), origShape(1))
						Case 1
							expOut = inArr.dup("c"c).reshape("c"c, origShape(0), 1, origShape(1))
						Case 2
							expOut = inArr.dup("c"c).reshape("c"c, origShape(0), origShape(1), 1)
						Case Else
							Throw New Exception()
					End Select

					Dim msg As String = "expandDim=" & i & ", source=" & p.Second

					assertEquals([out], expOut,msg)
				Next p
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSqueezeDims(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSqueezeDims(ByVal backend As Nd4jBackend)
			Dim origShape As val = New Long(){3, 4, 5}

			For i As Integer = 0 To 2

				Dim shape As val = origShape.clone()
				shape(i) = 1

				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, shape, DataType.FLOAT)
					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim squeeze As SDVariable = sd.squeeze([in], i)

					Dim [out] As INDArray = squeeze.eval()

					Dim expOut As INDArray
					Select Case i
						Case 0
							expOut = inArr.dup("c"c).reshape("c"c, origShape(1), origShape(2))
						Case 1
							expOut = inArr.dup("c"c).reshape("c"c, origShape(0), origShape(2))
						Case 2
							expOut = inArr.dup("c"c).reshape("c"c, origShape(0), origShape(1))
						Case Else
							Throw New Exception()
					End Select

					Dim msg As String = "squeezeDim=" & i & ", source=" & p.Second

					assertEquals([out], expOut,msg)
				Next p
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExpandSqueezeChain(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExpandSqueezeChain(ByVal backend As Nd4jBackend)

			Dim origShape As val = New Long(){3, 4}

			For i As Integer = 0 To 2
				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAllTestMatricesWithShape(origShape(0), origShape(1), 12345, DataType.FLOAT)
					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim expand As SDVariable = sd.expandDims([in], i)
					Dim squeeze As SDVariable = sd.squeeze(expand, i)

					Dim [out] As INDArray = squeeze.eval()

					Dim msg As String = "expand/Squeeze=" & i & ", source=" & p.Second

					assertEquals([out], inArr,msg) 'expand -> squeeze: should be opposite ops
				Next p
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSqueezeExpandChain(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSqueezeExpandChain(ByVal backend As Nd4jBackend)

			Dim origShape As val = New Long(){3, 4, 5}

			For i As Integer = 0 To 2

				Dim shape As val = origShape.clone()
				shape(i) = 1

				For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, shape, DataType.FLOAT)
					Dim inArr As INDArray = p.First.muli(100)

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", inArr)
					Dim squeeze As SDVariable = sd.squeeze([in], i)
					Dim expand As SDVariable = sd.expandDims(squeeze, i)

					Dim [out] As INDArray = expand.eval()

					Dim msg As String = "expand/Squeeze=" & i & ", source=" & p.Second

					assertEquals([out], inArr,msg) 'squeeze -> expand: should be opposite ops
				Next p
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConfusionMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConfusionMatrix(ByVal backend As Nd4jBackend)
			Dim labels As INDArray = Nd4j.createFromArray(1, 2, 4)
			Dim pred As INDArray = Nd4j.createFromArray(2, 2, 4)
			Dim weights As INDArray = Nd4j.createFromArray(10, 100, 1000)
			Dim numClasses As Integer? = 5
			Dim sd As SameDiff = SameDiff.create()
			Dim labelsVar As SDVariable = sd.constant("labels", labels)
			Dim predictionsVar As SDVariable = sd.constant("predictions", pred)
			Dim weightsVar As SDVariable = sd.constant("weights", weights)
			Dim cm As SDVariable = sd.math().confusionMatrix("cm", labelsVar, predictionsVar, weightsVar, numClasses)
			Dim [out] As INDArray = cm.eval()

			Dim exp As INDArray = Nd4j.create(New Single()(){
				New Single() {0, 0, 0, 0, 0},
				New Single() {0, 0, 10, 0, 0},
				New Single() {0, 0, 100, 0, 0},
				New Single() {0, 0, 0, 0, 0},
				New Single() {0, 0, 0, 0, 1000}
			}).castTo(DataType.INT)

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMax(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			For Each [dim] As val In New Integer()(){
				New Integer() {0},
				New Integer() {1},
				New Integer() {Integer.MaxValue},
				New Integer() {0, 1},
				New Integer() {}
			}
				Dim inArr As INDArray = Nd4j.rand(3, 4)
				Dim sd As SameDiff = SameDiff.create()

				Dim [in] As SDVariable = sd.var("in", inArr)
				Dim argmax As SDVariable = sd.argmax("argmax", [in], [dim])

				Dim [out] As INDArray = argmax.eval()

				Dim exp As INDArray = Nd4j.argMax(inArr, [dim])

				assertEquals(exp, [out])
			Next [dim]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMin(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)

			For Each [dim] As val In New Integer()(){
				New Integer() {0},
				New Integer() {1},
				New Integer() {Integer.MaxValue},
				New Integer() {0, 1},
				New Integer() {}
			}
				Dim inArr As INDArray = Nd4j.rand(3, 4)
				Dim sd As SameDiff = SameDiff.create()

				Dim [in] As SDVariable = sd.var("in", inArr)
				Dim argmin As SDVariable = sd.argmin("argmin", [in], [dim])

				Dim [out] As INDArray = argmin.eval()

				Dim exp As INDArray = Nd4j.argMax(inArr.neg(), [dim]) 'argmin(x) == argmax(-x)

				assertEquals(exp, [out])
			Next [dim]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterAdd(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.zeros(3, 3)
			Dim arr2 As INDArray = Nd4j.createFromArray(0, 1)
			Dim arr3 As INDArray = Nd4j.ones(2, 3)
			Dim expected As INDArray = Nd4j.create(New Single(){1, 1, 1, 1, 1, 1, 0, 0, 0}, New Long(){3, 3}).castTo(Nd4j.defaultFloatingPointType())

			Dim sd As SameDiff = SameDiff.create()
			Dim refs As SDVariable = sd.var("refs", arr1)
			Dim idxs As SDVariable = sd.constant("idxs", arr2)
			Dim upds As SDVariable = sd.placeHolder("upds", arr3.dataType(), arr3.shape())
			upds.Array = arr3

			Dim result As SDVariable = sd.scatterAdd(refs, idxs, upds)
			assertArrayEquals(New Long(){3, 3}, result.eval().shape())
			assertEquals(expected, result.eval())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterMul(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.ones(3, 3)
			Dim arr2 As INDArray = Nd4j.createFromArray(0, 1)
			Dim arr3 As INDArray = Nd4j.zeros(2, 3)
			Dim expected As INDArray = Nd4j.create(New Single(){0, 0, 0, 0, 0, 0, 1, 1, 1}, New Long(){3, 3}).castTo(Nd4j.defaultFloatingPointType())

			Dim sd As SameDiff = SameDiff.create()
			Dim refs As SDVariable = sd.var("refs", arr1)
			Dim idxs As SDVariable = sd.constant("idxs", arr2)
			Dim upds As SDVariable = sd.placeHolder("upds", arr3.dataType(), arr3.shape())
			upds.Array = arr3

			Dim result As SDVariable = sd.scatterMul(refs, idxs, upds)
			assertArrayEquals(New Long(){3, 3}, result.eval().shape())
			assertEquals(expected, result.eval())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterSub(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterSub(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.ones(3, 3)
			Dim arr2 As INDArray = Nd4j.createFromArray(0, 1)
			Dim arr3 As INDArray = Nd4j.ones(2, 3)
			Dim expected As INDArray = Nd4j.create(New Single(){0, 0, 0, 0, 0, 0, 1, 1, 1}, New Long(){3, 3}).castTo(Nd4j.defaultFloatingPointType())

			Dim sd As SameDiff = SameDiff.create()
			Dim refs As SDVariable = sd.var("refs", arr1)
			Dim idxs As SDVariable = sd.constant("idxs", arr2)
			Dim upds As SDVariable = sd.placeHolder("upds", arr3.dataType(), arr3.shape())
			upds.Array = arr3

			Dim result As SDVariable = sd.scatterSub(refs, idxs, upds)
			assertArrayEquals(New Long(){3, 3}, result.eval().shape())
			assertEquals(expected, result.eval())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterDiv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterDiv(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.ones(3, 3)
			Dim arr2 As INDArray = Nd4j.createFromArray(0, 1)
			Dim arr3 As INDArray = Nd4j.ones(2, 3).assign(2)
			Dim expected As INDArray = Nd4j.create(New Single(){0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f}, New Long(){3, 3}).castTo(Nd4j.defaultFloatingPointType())

			Dim sd As SameDiff = SameDiff.create()
			Dim refs As SDVariable = sd.var("refs", arr1)
			Dim idxs As SDVariable = sd.constant("idxs", arr2)
			Dim upds As SDVariable = sd.placeHolder("upds", arr3.dataType(), arr3.shape())
			upds.Array = arr3

			Dim result As SDVariable = sd.scatterDiv(refs, idxs, upds)
			assertArrayEquals(New Long(){3, 3}, result.eval().shape())
			assertEquals(expected, result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterMax(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.ones(3, 3)
			Dim arr2 As INDArray = Nd4j.createFromArray(0, 1)
			Dim arr3 As INDArray = Nd4j.ones(2, 3).assign(2)
			Dim expected As INDArray = Nd4j.create(New Single(){2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f}, New Long(){3, 3}).castTo(Nd4j.defaultFloatingPointType())

			Dim sd As SameDiff = SameDiff.create()
			Dim refs As SDVariable = sd.var("refs", arr1)
			Dim idxs As SDVariable = sd.constant("idxs", arr2)
			Dim upds As SDVariable = sd.placeHolder("upds", arr3.dataType(), arr3.shape())
			upds.Array = arr3

			Dim result As SDVariable = sd.scatterMax(refs, idxs, upds)
			assertArrayEquals(New Long(){3, 3}, result.eval().shape())
			assertEquals(expected, result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterMin(ByVal backend As Nd4jBackend)
			Dim arr1 As INDArray = Nd4j.ones(3, 3)
			Dim arr2 As INDArray = Nd4j.createFromArray(1, 2)
			Dim arr3 As INDArray = Nd4j.ones(2, 3).assign(-2.0f)
			Dim expected As INDArray = Nd4j.create(New Single(){1.0f, 1.0f, 1.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f}, New Long(){3, 3}).castTo(Nd4j.defaultFloatingPointType())

			Dim sd As SameDiff = SameDiff.create()
			Dim refs As SDVariable = sd.var("refs", arr1)
			Dim idxs As SDVariable = sd.constant("idxs", arr2)
			Dim upds As SDVariable = sd.placeHolder("upds", arr3.dataType(), arr3.shape())
			upds.Array = arr3

			Dim result As SDVariable = sd.scatterMin(refs, idxs, upds)
			assertArrayEquals(New Long(){3, 3}, result.eval().shape())
			assertEquals(expected, result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReciprocal(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReciprocal(ByVal backend As Nd4jBackend)
			Dim inArr As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim expected As INDArray = Nd4j.onesLike(inArr).divi(inArr)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim reciprocal As SDVariable = sd.math().reciprocal([in])
			Dim res As INDArray = reciprocal.eval()
			assertEquals(expected, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGather2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGather2(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 10, 10)
			Dim indices As INDArray = Nd4j.createFromArray(0, 1, 5)

			Dim sd As SameDiff = SameDiff.create()

			Dim var As SDVariable = sd.var("in", [in])
			Dim varIndices As SDVariable = sd.constant("indices", indices)
			Dim gather As SDVariable = sd.gather(var, varIndices, 0)

			Dim exp As INDArray = Nd4j.pullRows([in], 1, New Integer(){0, 1, 5}) 'Along dimension 1 -> equiv to "indexes for axis 0"
			Dim act As INDArray = gather.eval()

			assertEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGatherOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGatherOp(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 10)
			Dim indices As INDArray = Nd4j.createFromArray(0, 1, 5)
			Dim [out] As INDArray = Nd4j.create(3, 10)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("gather").addIntegerArguments(0).addInputs([in], indices).addOutputs([out]).build()

			Nd4j.exec(op)

			Dim exp As INDArray = Nd4j.pullRows([in], 1, New Integer(){0, 1, 5}) 'Along dimension 1 == indexes for dimension 0

			assertEquals(exp, [out])

			'Shape function:
			Dim shapes As val = Nd4j.Executioner.calculateOutputShape(op)
			Dim expShape() As Long = {3, 10}

			assertEquals(1, shapes.size())

			assertArrayEquals(expShape, shapes.get(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConditions(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConditions(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()

			Dim ia As INDArray = Nd4j.create(New Single(){4, 2})
			Dim [in] As SDVariable = sd.var("in", 1, 2)
			sd.associateArrayWithVariable(ia, [in])

			Dim expFinite As INDArray = Nd4j.create(New Boolean(){True, True})
			Dim finite As SDVariable = sd.math().isFinite([in])

			Dim expInfinite As INDArray = Nd4j.create(New Boolean(){False, False})
			Dim infinite As SDVariable = sd.math().isInfinite([in])

			Dim expNaN As INDArray = Nd4j.create(New Boolean(){False, False})
			Dim isnan As SDVariable = sd.math().isNaN([in])

			assertEquals(expFinite, finite.eval())
			assertEquals(expInfinite, infinite.eval())
			assertEquals(expNaN, isnan.eval())

		End Sub


		Private Shared Function binArrToInt(ByVal arr() As Integer) As Integer
			Dim x As Integer = 0
			Dim m As Integer = 1
			For i As Integer = arr.Length - 1 To 0 Step -1
				If arr(i) = 1 Then
					x += m
				End If
				m *= 2
			Next i
			Return x
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGet(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1, 100, 100).reshape("c"c, 10L, 10L)
			Dim x As SDVariable = sd.var(arr)

			Dim expOut1 As INDArray = arr.get(NDArrayIndex.point(4), NDArrayIndex.point(5)).reshape()
			Dim result1 As SDVariable = x.get(SDIndex.point(4), SDIndex.point(5))
			assertEquals(expOut1, result1.eval())

			Dim expOut2 As INDArray = arr.get(NDArrayIndex.point(4), NDArrayIndex.all()).reshape(10)
			Dim result2 As SDVariable = x.get(SDIndex.point(4), SDIndex.all())
			assertEquals(expOut2, result2.eval())

			Dim expOut3 As INDArray = arr.get(NDArrayIndex.interval(3, 8)).reshape(5, 10)
			Dim result3 As SDVariable = x.get(SDIndex.interval(3, 8))
			assertEquals(expOut3, result3.eval())

			Dim expOut4 As INDArray = arr.get(NDArrayIndex.point(5), NDArrayIndex.interval(3, 8)).reshape(5)
			Dim result4 As SDVariable = x.get(SDIndex.point(5), SDIndex.interval(3, 8))
			assertEquals(expOut4, result4.eval())

			Dim expOut5 As INDArray = arr.get(NDArrayIndex.interval(5, 6), NDArrayIndex.all())
			Dim result5 As SDVariable = x.get(SDIndex.point(5, True), SDIndex.all())
			assertEquals(expOut5, result5.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRank3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRank3(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("c"c, 10, 10, 10)
			Dim x As SDVariable = sd.var(arr)

			Dim y1 As INDArray = arr.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all())
			Dim s1 As SDVariable = x.get(SDIndex.point(2), SDIndex.all(), SDIndex.all())
			Dim s1a As INDArray = s1.eval()
			assertEquals(s1a, y1)

			Dim y2 As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.point(2), NDArrayIndex.all())
			Dim s2 As SDVariable = x.get(SDIndex.all(), SDIndex.point(2), SDIndex.all())
			Dim s2a As INDArray = s2.eval()
			assertEquals(s2a, y2)

			Dim y3 As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(2))
			Dim s3 As SDVariable = x.get(SDIndex.all(), SDIndex.all(), SDIndex.point(2))
			Dim s3a As INDArray = s3.eval()
			assertEquals(s3a, y3)

			Dim y4 As INDArray = arr.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))
			Dim s4 As SDVariable = x.get(SDIndex.point(2), SDIndex.all(), SDIndex.interval(3, 5))
			Dim s4a As INDArray = s4.eval()
			assertEquals(s4a, y4)

			Dim y5 As INDArray = arr.get(NDArrayIndex.interval(3, 5), NDArrayIndex.point(2), NDArrayIndex.all())
			Dim s5 As SDVariable = x.get(SDIndex.interval(3, 5), SDIndex.point(2), SDIndex.all())
			Dim s5a As INDArray = s5.eval()
			assertEquals(s5a, y5)

			Dim y6 As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.interval(3, 5), NDArrayIndex.point(2))
			Dim s6 As SDVariable = x.get(SDIndex.all(), SDIndex.interval(3, 5), SDIndex.point(2))
			Dim s6a As INDArray = s6.eval()
			assertEquals(s6a, y6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorArray1(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim tensorArray As TensorArray = sd.tensorArray(DataType.FLOAT)
			Dim arr1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Integer(){2, 2})
			Dim var1 As SDVariable = sd.var(arr1)
			Dim arr2 As INDArray = Nd4j.create(New Double(){5, 6, 7, 8}, New Integer(){2, 2})
			Dim var2 As SDVariable = sd.var(arr2)
			Dim write0 As SDVariable = tensorArray.write(var2, 0, var1)
			Dim write1 As SDVariable = tensorArray.write(write0, 1, var2)
			Dim result As SDVariable = tensorArray.stack(write1)
			sd.output(DirectCast(Nothing, IDictionary(Of String, INDArray)), result.name())
			assertEquals(Nd4j.pile(arr1, arr2), result.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorArray2(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim tensorArray As TensorArray = sd.tensorArray(DataType.FLOAT)
			Dim arr1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Integer(){2, 2})
			Dim var1 As SDVariable = sd.var(arr1)
			Dim arr2 As INDArray = Nd4j.create(New Double(){5, 6, 7, 8}, New Integer(){2, 2})
			Dim var2 As SDVariable = sd.var(arr2)
			Dim write1 As SDVariable = tensorArray.write(var2, 0, var1)
			Dim write2 As SDVariable = tensorArray.write(write1, 1, var2)
			Dim result1 As SDVariable = tensorArray.read(0)
			Dim result2 As SDVariable = tensorArray.read(1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorArray3(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim tensorArray As TensorArray = sd.tensorArray(DataType.FLOAT)
			Dim arr1 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4}, New Integer(){2, 2})
			Dim arr2 As INDArray = Nd4j.create(New Double(){5, 6, 7, 8}, New Integer(){2, 2})
			Dim arr3 As INDArray = Nd4j.pile(arr1, arr2)
			Dim var As SDVariable = sd.var(arr3)
			Dim unstack As SDVariable = tensorArray.unstack(var, var)
			Dim result1 As SDVariable = tensorArray.read(0)
			Dim result2 As SDVariable = tensorArray.read(1)
			result1.addControlDependency(unstack)
			result2.addControlDependency(unstack)
			assertEquals(arr1, result1.eval())
			assertEquals(arr2, result2.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFill(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFill(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim shape As INDArray = Nd4j.createFromArray(2, 2)
			Dim expOut As INDArray = Nd4j.valueArrayOf(New Integer(){2, 2}, 42.0)
			Dim x As SDVariable = sd.constant(shape)
			Dim result As SDVariable = sd.fill(x, DataType.DOUBLE, 42)
			assertEquals(expOut, result.eval())
		End Sub

		Private Shared Function getObject(Of T)(ByVal fieldName As String, ByVal from As Object, ByVal fromClass As Type) As T
			Try
				Dim f As System.Reflection.FieldInfo = fromClass.getDeclaredField(fieldName)
				f.setAccessible(True)
				Return CType(f.get(from), T)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.create(New Double(){ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 }, New Integer(){2, 3, 4})

			Dim expOut As INDArray = Nd4j.create(New Double(){ 1, 2, 3, 4, 13, 14, 15, 16, 5, 6, 7, 8, 17, 18, 19, 20, 9, 10, 11, 12, 21, 22, 23, 24 }, New Integer(){3, 2, 4})

			Dim x As SDVariable = sd.var(arr)
			Dim result As SDVariable = sd.permute(x, 1, 0, 2)
			assertEquals(expOut, result.eval())

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecutionDifferentShapesAccumAlongDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecutionDifferentShapesAccumAlongDim(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4))

			Dim sum As SDVariable = [in].sum(1)
			Dim exp As INDArray = [in].Arr.sum(1).reshape(ChrW(3))

			Dim [out] As INDArray = sum.eval()
			assertEquals(exp, [out])

			'Now, replace with minibatch 5:
			[in].Array = Nd4j.linspace(1, 20, 20).reshape(ChrW(5), 4)
			Dim out2 As INDArray = sum.eval()
			assertArrayEquals(New Long(){5}, out2.shape())

			exp = [in].Arr.sum(1).reshape(ChrW(5))
			assertEquals(exp, out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecutionDifferentShapesIndexAccumAlongDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecutionDifferentShapesIndexAccumAlongDim(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4))

			Dim sum As SDVariable = [in].argmax(1)
			Dim exp As INDArray = [in].Arr.argMax(1).reshape(ChrW(3))

			Dim [out] As INDArray = sum.eval()
			assertEquals(exp, [out])

			'Now, replace with minibatch 5:
			[in].Array = Nd4j.linspace(1, 20, 20).reshape(ChrW(5), 4)
			Dim out2 As INDArray = sum.eval()
			assertArrayEquals(New Long(){5}, out2.shape())

			exp = [in].Arr.argMax(1).reshape(ChrW(5))
			assertEquals(exp, out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExternalErrorsSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExternalErrorsSimple(ByVal backend As Nd4jBackend)
			Dim externalGrad As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			Dim sd As SameDiff = SameDiff.create()
			Dim var As SDVariable = sd.var("var", externalGrad)
			Dim [out] As SDVariable = var.mul("out", 0.5)

			Dim gradMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			gradMap("out") = externalGrad
			Dim fn As ExternalErrorsFunction = SameDiffUtils.externalErrors(sd, Nothing, [out])

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("out-grad") = externalGrad
			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(m, sd.getVariables().keySet())

			Dim gradVar As INDArray = grads(var.name())

			assertEquals(externalGrad.mul(0.5), gradVar)

			'Now, update and execute again:
			externalGrad = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4).muli(10)

			m("out-grad") = externalGrad
			grads = sd.calculateGradients(m, sd.getVariables().keySet())

			gradVar = var.Gradient.Arr

			assertEquals(externalGrad.mul(0.5), gradVar)

			'Test model serialization:
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUpdatingGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUpdatingGradient(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4))
			Dim w As SDVariable = sd.var("w", Nd4j.linspace(1, 20, 20).reshape(ChrW(4), 5))
			Dim [out] As SDVariable = sd.mmul([in], w)
			Dim loss As SDVariable = [out].std("out", True)

			Dim outArr As INDArray = loss.eval()
			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, [in].name(), w.name(), [out].name())

			Dim origGrad As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			origGrad("in") = grads([in].name()).dup()
			origGrad("w") = grads(w.name()).dup()
			origGrad("out") = grads([out].name()).dup()

			[in].Arr.assign(Nd4j.rand([in].Arr.shape()))
			Dim outArr2 As INDArray = loss.eval()
			grads = sd.calculateGradients(Nothing, [in].name(), w.name(), [out].name())

			assertNotEquals(outArr, outArr2)

			'Ensure gradients are also changed:
			assertNotEquals(origGrad("in"), grads([in].name()))
			assertNotEquals(origGrad("w"), grads(w.name()))
			assertNotEquals(origGrad("out"), grads([out].name()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUpdatingGradientSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUpdatingGradientSimple(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4))
			Dim [out] As SDVariable = [in].mul(2.0)
			Dim loss As SDVariable = [out].std("out", True)

			Dim outArr As INDArray = loss.eval()
			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, [in].name(), [out].name())

			Dim origGrad As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			origGrad("in") = grads([in].name()).dup()
			origGrad("out") = grads([out].name()).dup()

			Dim stdBefore As Double = [in].Arr.stdNumber().doubleValue()
			[in].Arr.assign(Nd4j.rand([in].Arr.shape()))
			Dim stdAfter As Double = [in].Arr.stdNumber().doubleValue()
			Console.WriteLine("Before vs. after: " & stdBefore & ", " & stdAfter)
			Dim outArr2 As INDArray = loss.eval()
			grads = sd.calculateGradients(Nothing, [in].name(), [out].name())

			assertNotEquals(outArr, outArr2)

			'Ensure gradients are also changed:
			assertNotEquals(origGrad("in"), grads([in].name()))
			assertNotEquals(origGrad("out"), grads([out].name()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeUpdating(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeUpdating(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", DataType.FLOAT, 3, 5)
			Dim w As SDVariable = sd.var("W", DataType.FLOAT, 5, 4)
			Dim b As SDVariable = sd.var("b", DataType.FLOAT, 1, 4)
			Dim z As SDVariable = [in].mmul(w).add(b)
			Dim [out] As SDVariable = sd.math().tanh("tanh", z)
			Dim fn As ExternalErrorsFunction = SameDiffUtils.externalErrors(sd, Nothing, [out])

			Dim inA As INDArray = Nd4j.linspace(1, 15, 15, DataType.FLOAT).reshape(ChrW(3), 5)
			Dim wA As INDArray = Nd4j.linspace(1, 20, 20, DataType.FLOAT).reshape(ChrW(5), 4)
			Dim bA As INDArray = Nd4j.linspace(1, 4, 4, DataType.FLOAT)
			[in].Array = inA
			w.Array = wA
			b.Array = bA

			Dim grad As INDArray = Nd4j.linspace(1, 12, 12, DataType.FLOAT).reshape(ChrW(3), 4)
			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			phMap(fn.GradPlaceholderName) = grad

	'        log.info("--------------- out.eval() ---------------");
			[out].eval()
	'        log.info("--------------- sd.execBackwards() #1 ---------------");
			sd.calculateGradients(phMap, "in", "W", "b")

	'        log.info("--------------- sd.execBackwards() #2 ---------------");
	'        System.out.println(sd.getFunction("grad").summary());
			sd.getFunction("grad").summary()

			[in].Array = Nd4j.linspace(1, 10, 10).reshape(ChrW(2), 5)
			grad = Nd4j.linspace(1, 8, 8).reshape(ChrW(2), 4)
			phMap(fn.GradPlaceholderName) = grad

			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(phMap, sd.getVariables().keySet())
			Dim inGrad As INDArray = grads([in].name())
			assertArrayEquals(New Long(){2, 5}, inGrad.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiOutput1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultiOutput1(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.create(3, 4))
			Dim mean As SDVariable = [in].mean()
			Dim sum As SDVariable = [in].sum()

			Try
				sd.createGradFunction()
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("No loss variables"),e.Message)
			End Try

			Dim add As SDVariable = mean.add(sum)
			sd.createGradFunction()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiOutput2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultiOutput2(ByVal backend As Nd4jBackend)
			'Edge case: no functions
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.scalar(0.0))
			Dim in2 As SDVariable = sd.var("in2", Nd4j.scalar(1.0))

			Try
				sd.createGradFunction()
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("No loss variables"),e.Message)
			End Try

			Dim add As SDVariable = [in].add(in2)
			sd.createGradFunction()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void sameDiffPlaceholderGrad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub sameDiffPlaceholderGrad(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.ones(2, 2)
			Dim y As INDArray = Nd4j.ones(2, 2)

			Dim sd As SameDiff = SameDiff.create()

			Dim xSd As SDVariable = sd.var("x", DataType.FLOAT, x.shape())
			Dim ySd As SDVariable = sd.var("y", DataType.FLOAT, y.shape())

			Dim add As SDVariable = ySd.add("add", xSd)

			Dim placeholders As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			placeholders("x") = x
			placeholders("y") = y
			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(placeholders, xSd.name(), ySd.name())
			Dim xGradientEnforced As INDArray = grads("x")
			assertNotNull(xGradientEnforced)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConvertToConstant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConvertToConstant(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, 1, 3)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 3, 4))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 4))
			Dim mmul As SDVariable = [in].mmul(w)
			Dim add As SDVariable = mmul.add(b)
			Dim tanh As SDVariable = sd.math().tanh(add)
			Dim loss As SDVariable = sd.variance(tanh, True)

			Dim inArr As INDArray = Nd4j.rand(DataType.FLOAT, 1, 3)
			[in].Array = inArr

			Dim c As TrainingConfig = TrainingConfig.builder().updater(New Adam(0.1)).weightDecay(0.01, True).dataSetFeatureMapping("in").skipBuilderValidation(True).build()
			sd.TrainingConfig = c

			sd.fit(New SingletonMultiDataSetIterator((New DataSet(inArr, Nothing)).toMultiDataSet()), 1)

			Dim [out] As INDArray = tanh.eval()

			w.convertToConstant()

			Dim out2 As INDArray = tanh.eval()

			assertEquals([out], out2)
			assertEquals(VariableType.CONSTANT, w.getVariableType())
			assertEquals(VariableType.VARIABLE, b.getVariableType())
			assertEquals(VariableType.ARRAY, add.getVariableType())
			assertEquals(VariableType.ARRAY, tanh.getVariableType())

			'Sanity check on training:
			sd.fit(New SingletonMultiDataSetIterator((New DataSet(inArr, Nothing)).toMultiDataSet()), 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPlaceholderToConstant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPlaceholderToConstant(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, 1, 3)
			Dim in2 As SDVariable = sd.placeHolder("in2", DataType.FLOAT, 3, 4)
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 4))
			Dim mmul As SDVariable = [in].mmul(in2)
			Dim add As SDVariable = mmul.add(b)
			Dim tanh As SDVariable = sd.math().tanh(add)
			Dim loss As SDVariable = sd.variance(tanh, True)

			Dim inArr As INDArray = Nd4j.rand(DataType.FLOAT, 1, 3)
			[in].Array = inArr
			Dim inArr2 As INDArray = Nd4j.rand(DataType.FLOAT, 3, 4)
			in2.Array = inArr2

			Dim c As TrainingConfig = TrainingConfig.builder().updater(New Adam(0.1)).weightDecay(0.01, True).dataSetFeatureMapping("in", "in2").skipBuilderValidation(True).build()
			sd.TrainingConfig = c

			sd.fit(New SingletonMultiDataSetIterator(New MultiDataSet(New INDArray(){inArr, inArr2}, Nothing)), 1)

			Dim [out] As INDArray = tanh.eval()

			[in].convertToConstant()

			Dim out2 As INDArray = tanh.eval()

			assertEquals([out], out2)
			assertEquals(VariableType.CONSTANT, [in].getVariableType())
			assertEquals(inArr, [in].Arr)

			'Sanity check on fitting:
			sd.fit(New SingletonMultiDataSetIterator(New MultiDataSet(New INDArray(){inArr2}, Nothing)), 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConvertToVariable(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConvertToVariable(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, 1, 3)
			Dim const1 As INDArray = Nd4j.rand(DataType.FLOAT, 3, 4)
			Dim w As SDVariable = sd.constant("w",const1)
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 4))
			Dim mmul As SDVariable = [in].mmul(w)
			Dim add As SDVariable = mmul.add(b)
			Dim tanh As SDVariable = sd.math().tanh(add)
			Dim loss As SDVariable = sd.variance(tanh, True)

			Dim inArr As INDArray = Nd4j.rand(DataType.FLOAT, 1, 3)
			[in].Array = inArr

			Dim c As TrainingConfig = TrainingConfig.builder().updater(New Adam(0.1)).weightDecay(0.01, True).dataSetFeatureMapping("in").skipBuilderValidation(True).build()
			sd.TrainingConfig = c

			Dim [out] As INDArray = tanh.eval()
			sd.fit(New SingletonMultiDataSetIterator((New DataSet(inArr, Nothing)).toMultiDataSet()), 1)
			w.convertToVariable()

			Dim out2 As INDArray = tanh.eval()

			assertNotEquals([out], out2)
			assertEquals(VariableType.VARIABLE, w.getVariableType())
			assertEquals(VariableType.VARIABLE, b.getVariableType())
			assertEquals(VariableType.ARRAY, add.getVariableType())
			assertEquals(VariableType.ARRAY, tanh.getVariableType())

			'Sanity check on training:
			sd.fit(New SingletonMultiDataSetIterator((New DataSet(inArr, Nothing)).toMultiDataSet()), 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDoubleUseOfArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDoubleUseOfArray(ByVal backend As Nd4jBackend)
			'If array is reused, gradient check will fail
			Dim a As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){3, 4})
			Dim sd As SameDiff = SameDiff.create()
			Dim a1 As SDVariable = sd.var("a", a)
			Dim a2 As SDVariable = sd.var("b", a)
			a1.add(a2).norm2("out")
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)

			a1.Array = a
			a2.Array = a
			err = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiGradientRecurrent(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultiGradientRecurrent(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, new int[]{3, 4, 2});
			Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){3, 4, 2})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] output = new org.nd4j.linalg.api.ndarray.INDArray[(int) input.size(2)];
			Dim output(CInt(input.size(2)) - 1) As INDArray
			Dim i As Integer = 0
			Do While i < input.size(2)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray x_i = input.get(org.nd4j.linalg.indexing.NDArrayIndex.all(), org.nd4j.linalg.indexing.NDArrayIndex.all(), org.nd4j.linalg.indexing.NDArrayIndex.point(i));
				Dim x_i As INDArray = input.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i))

				output(i) = x_i
				If i > 0 Then
					output(i) = output(i).add(Nd4j.squeeze(output(i - 1), 2))
				End If

				output(i) = Nd4j.expandDims(output(i), 2)
				i += 1
			Loop
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray out = org.nd4j.linalg.factory.Nd4j.concat(2, output).norm2();
			Dim [out] As INDArray = Nd4j.concat(2, output).norm2()

			Dim sd As SameDiff = SameDiff.create()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable sdInput = sd.var("input", input);
			Dim sdInput As SDVariable = sd.var("input", input)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long timeSteps = sdInput.getShape()[2];
			Dim timeSteps As Long = sdInput.Shape(2)
			Dim outputSlices(CInt(timeSteps) - 1) As SDVariable
			Dim prev As SDVariable = Nothing
			For i As Integer = 0 To timeSteps - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val x_i = sdInput.get(SDIndex.all(), SDIndex.all(), SDIndex.point(i));
				Dim x_i As val = sdInput.get(SDIndex.all(), SDIndex.all(), SDIndex.point(i))

				outputSlices(i) = x_i
				If prev IsNot Nothing Then
					outputSlices(i) = outputSlices(i).add(sd.squeeze(prev, 2))
				End If

				outputSlices(i) = sd.expandDims(outputSlices(i), 2)
				prev = outputSlices(i)
			Next i

			Dim t As SDVariable = sd.concat(2, outputSlices)
			t.norm2("out")
			Dim err As String = OpValidation.validate((New TestCase(sd)).testFlatBufferSerialization(TestCase.TestSerialization.BOTH).expectedOutput("out", [out]).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiGradientManualRecurrent(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultiGradientManualRecurrent(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, new int[]{3, 4, 2});
			Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){3, 4, 2})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] output = new org.nd4j.linalg.api.ndarray.INDArray[(int) input.size(2)];
			Dim output(CInt(input.size(2)) - 1) As INDArray
			Dim i As Integer = 0
			Do While i < input.size(2)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray x_i = input.get(org.nd4j.linalg.indexing.NDArrayIndex.all(), org.nd4j.linalg.indexing.NDArrayIndex.all(), org.nd4j.linalg.indexing.NDArrayIndex.point(i));
				Dim x_i As INDArray = input.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i))

				output(i) = x_i
				If i > 0 Then
					output(i) = output(i).add(Nd4j.squeeze(output(i - 1), 2))
				End If

				output(i) = Nd4j.expandDims(output(i), 2)
				i += 1
			Loop
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray out = org.nd4j.linalg.factory.Nd4j.concat(2, output).norm2();
			Dim [out] As INDArray = Nd4j.concat(2, output).norm2()

			Dim sd As SameDiff = SameDiff.create()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable sdInput = sd.var("input", input);
			Dim sdInput As SDVariable = sd.var("input", input)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long timeSteps = sdInput.getShape()[2];
			Dim timeSteps As Long = sdInput.Shape(2)
			Dim outputSlices(CInt(timeSteps) - 1) As SDVariable
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable[] inputSlices = sd.unstack(new String[]{"X_0", "X_1"}, sdInput, 2, 2);
			Dim inputSlices() As SDVariable = sd.unstack(New String(){"X_0", "X_1"}, sdInput, 2, 2)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val x_0 = inputSlices[0];
			Dim x_0 As val = inputSlices(0)
			outputSlices(0) = x_0
			outputSlices(0) = sd.expandDims("X_0-e", outputSlices(0), 2)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val x_1 = inputSlices[1];
			Dim x_1 As val = inputSlices(1)
			outputSlices(1) = x_1
			outputSlices(1) = outputSlices(1).add(sd.squeeze("X_0-s", outputSlices(0), 2))
			outputSlices(1) = sd.expandDims("X_1-e", outputSlices(1), 2)

			Dim t As SDVariable = sd.concat(2, outputSlices)
			t.norm2("out")
			Dim err As String = OpValidation.validate((New TestCase(sd)).testFlatBufferSerialization(TestCase.TestSerialization.BOTH).expectedOutput("out", [out]).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiGradient(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultiGradient(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, new int[]{3, 4, 2});
			Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){3, 4, 2})
			Dim sd As SameDiff = SameDiff.create()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable sdInput = sd.var("input", input);
			Dim sdInput As SDVariable = sd.var("input", input)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable[] inputSlices = sd.unstack(new String[]{"X_0", "X_1"}, sdInput, 2, 2);
			Dim inputSlices() As SDVariable = sd.unstack(New String(){"X_0", "X_1"}, sdInput, 2, 2)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val temp = inputSlices[0].add(inputSlices[1]).div(inputSlices[1]).mul(inputSlices[0]);
			Dim temp As val = inputSlices(0).add(inputSlices(1)).div(inputSlices(1)).mul(inputSlices(0))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val out = temp.add(temp).add(inputSlices[1]);
			Dim [out] As val = temp.add(temp).add(inputSlices(1))
			[out].norm2("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).testFlatBufferSerialization(TestCase.TestSerialization.BOTH).gradientCheck(True))

			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonScalarOutput1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonScalarOutput1(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim linspace As SDVariable = sd.linspace("at", DataType.DOUBLE, 1, 15, 15)
			Dim a As SDVariable = sd.reshape("a", linspace, 3, 5)
			Dim b As SDVariable = sd.var("b", Nd4j.ones(DataType.DOUBLE, 3, 5))

			Dim [out] As SDVariable = a.mul(b)
			[out].markAsLoss()
			[out].eval()

			[out].eval()
			sd.grad("a").eval()

			Dim err As String = OpValidation.validate((New TestCase(sd)).testFlatBufferSerialization(TestCase.TestSerialization.BOTH).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonScalarOutput2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonScalarOutput2(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim a As SDVariable = sd.reshape("a", sd.linspace("at", DataType.DOUBLE, 1, 15, 15), 3, 5)
			Dim b As SDVariable = sd.var("b", Nd4j.ones(DataType.DOUBLE, 3, 5))

			Dim [out] As SDVariable = a.mul(b).mean(1)
			[out].markAsLoss()
			[out].eval()

			'System.out.println(out.eval());
			Dim actGrad As INDArray = sd.grad("a").eval()

			Dim expGrad As INDArray = Nd4j.valueArrayOf(New Long(){3, 5}, 0.2, DataType.DOUBLE)
			assertEquals(expGrad, actGrad)

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonScalarOutput3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonScalarOutput3(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim a As SDVariable = sd.reshape("a", sd.linspace("at", DataType.DOUBLE, 1, 15, 15), 3, 5)
			Dim b As SDVariable = sd.var("b", Nd4j.ones(DataType.DOUBLE, 3, 5)) '.add(3);

			Dim [out] As SDVariable = a.mul(b).mean(0, 1)
			[out].markAsLoss()

			[out].eval()

			Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, "a")
			'System.out.println(out.eval());
			Dim gradAct As INDArray = g("a")
			Dim expGrad As INDArray = Nd4j.valueArrayOf(New Long(){3, 5}, 1.0 / 12, DataType.DOUBLE)

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonScalarOutput4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonScalarOutput4(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim a As SDVariable = sd.var("a", DataType.DOUBLE, 3, 4)
			Dim b As SDVariable = sd.placeHolder("b", DataType.DOUBLE, 4, 5)
			a.Array = Nd4j.rand(DataType.DOUBLE, 3, 4)

			Dim [out] As SDVariable = a.mmul("mmul", b)

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("b") = Nd4j.rand(DataType.DOUBLE, 4, 5)
			Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(m, "a", "b")

			b.Array = m("b")

			Dim err As String = OpValidation.validate((New TestCase(sd)).testFlatBufferSerialization(TestCase.TestSerialization.BOTH).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonScalarOutput5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonScalarOutput5(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim linspace As SDVariable = sd.linspace(DataType.DOUBLE, 1, 75, 75)
			Dim a As SDVariable = sd.reshape("a", linspace, 15, 5)
			Dim b As SDVariable = sd.var("b", Nd4j.ones(DataType.DOUBLE, 15, 5))

			Dim [out] As SDVariable = a.mul(b)
			[out].markAsLoss()
			[out].eval()

			Dim outEvaled As INDArray = [out].eval()
			Dim gradOutput As INDArray = sd.grad("a").eval()
			Dim bOutputEval As INDArray = sd.grad("b").eval()
			Dim err As String = OpValidation.validate((New TestCase(sd)).testFlatBufferSerialization(TestCase.TestSerialization.BOTH).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSameDiffBackprop1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSameDiffBackprop1(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable a = sd.var("a", org.nd4j.linalg.factory.Nd4j.rand(4, 4));
			Dim a As SDVariable = sd.var("a", Nd4j.rand(4, 4))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable b = sd.var("b", org.nd4j.linalg.factory.Nd4j.rand(4, 4));
			Dim b As SDVariable = sd.var("b", Nd4j.rand(4, 4))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable c = sd.var("c", org.nd4j.linalg.factory.Nd4j.rand(4, 4));
			Dim c As SDVariable = sd.var("c", Nd4j.rand(4, 4))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable d = sd.var("d", org.nd4j.linalg.factory.Nd4j.rand(4, 4));
			Dim d As SDVariable = sd.var("d", Nd4j.rand(4, 4))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable out = a.mmul(b).add(c.mmul(d)).sum();
			Dim [out] As SDVariable = a.mmul(b).add(c.mmul(d)).sum()
			[out].markAsLoss()

			Dim g As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, sd.getVariables().keySet())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSameDiffNoGradForConstantAndPlaceholder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSameDiffNoGradForConstantAndPlaceholder(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable a = sd.var("a", org.nd4j.linalg.factory.Nd4j.rand(4, 4));
			Dim a As SDVariable = sd.var("a", Nd4j.rand(4, 4))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable b = sd.constant("b", org.nd4j.linalg.factory.Nd4j.rand(4, 4));
			Dim b As SDVariable = sd.constant("b", Nd4j.rand(4, 4))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable c = sd.placeHolder("c", org.nd4j.linalg.factory.Nd4j.dataType(), 4, 4);
			Dim c As SDVariable = sd.placeHolder("c", Nd4j.dataType(), 4, 4)

			a.add(b.add(c)).sum().markAsLoss()

			sd.calculateGradients(Collections.singletonMap("c", Nd4j.rand(4, 4)), sd.getVariables().keySet())
			assertNotNull(sd.grad("a"))
			assertNull(sd.grad("b"))
			assertNull(sd.grad("c"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDuplicateNamePlaceholder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDuplicateNamePlaceholder(ByVal backend As Nd4jBackend)

			For i As Integer = 0 To 1
				Dim sd As SameDiff = SameDiff.create()
				Dim x1 As SDVariable = If(i = 0, sd.placeHolder("a", DataType.FLOAT, 5, 3), sd.var("a", DataType.FLOAT, 5, 3))
				Dim x2 As SDVariable = If(i = 0, sd.placeHolder("b", DataType.FLOAT, 5, 3), sd.var("b", DataType.FLOAT, 5, 3))
				Try
					sd.placeHolder("a", DataType.FLOAT, 5, 3)
					fail("Expected exception")
				Catch t As Exception
					Dim m As String = t.getMessage()
					assertNotNull(m)
					assertTrue(m.Contains("already exists"),m)
				End Try

				Try
					sd.var("a", DataType.FLOAT, 1, 2)
					fail("Expected exception")
				Catch t As Exception
					Dim m As String = t.getMessage()
					assertNotNull(m)
					assertTrue(m.Contains("already exists"),m)
				End Try

				Try
					sd.var("a", Nd4j.zeros(1))
					fail("Expected exception")
				Catch t As Exception
					Dim m As String = t.getMessage()
					assertNotNull(m)
					assertTrue(m.Contains("already exists"),m)
				End Try

				Try
					sd.var("a", LongShapeDescriptor.fromShape(New Long(){1}, DataType.FLOAT))
					fail("Expected exception")
				Catch t As Exception
					Dim m As String = t.getMessage()
					assertNotNull(m)
					assertTrue(m.Contains("already exists"),m)
				End Try

				Try
					sd.constant("a", Nd4j.zeros(1))
					fail("Expected exception")
				Catch t As Exception
					Dim m As String = t.getMessage()
					assertNotNull(m)
					assertTrue(m.Contains("already exists"),m)
				End Try
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSameDiffGetArrayScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSameDiffGetArrayScalar(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray array = org.nd4j.linalg.factory.Nd4j.rand(1, 1);
			Dim array As INDArray = Nd4j.rand(1, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SameDiff sd = SameDiff.create();
			Dim sd As SameDiff = SameDiff.create()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable a = sd.var("a", array.shape());
			Dim a As SDVariable = sd.var("a", array.shape())
			a.Arr
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableRenaming(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableRenaming(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim v1 As SDVariable = sd.var("x", Nd4j.rand(DataType.FLOAT, 3, 4))
			Dim v2 As SDVariable = sd.var("y", Nd4j.rand(DataType.FLOAT, 4, 5))
			Dim v3 As SDVariable = v1.mmul("oldName", v2)

			Dim [out] As INDArray = sd.outputSingle(Nothing, "oldName")

			Dim renamed As SDVariable = v3.rename("newName")
			assertTrue(v3 Is renamed)
			assertEquals("newName", renamed.name())

			assertNull(sd.getVariable("oldName"))
			assertNotNull(sd.getVariable("newName"))

			Dim out2 As INDArray = sd.outputSingle(Nothing, "newName")

			assertEquals([out], out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableRenaming2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableRenaming2(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim v1 As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3, 4)
			Dim v2 As SDVariable = sd.var("y", Nd4j.rand(DataType.FLOAT, 4, 5))
			Dim v3 As SDVariable = v1.mmul("oldName", v2)
			Dim v4 As SDVariable = v3.std("out", False)

			Dim [out] As INDArray = sd.outputSingle(Collections.singletonMap("x", Nd4j.rand(DataType.FLOAT, 3, 4)), "out")

			sd.TrainingConfig = TrainingConfig.builder().updater(New Adam(1e-3)).dataSetFeatureMapping("x").markLabelsUnused().build()

			sd.fit(New DataSet(Nd4j.rand(DataType.FLOAT, 3, 4), Nothing))
			v3.rename("newName")
			sd.fit(New DataSet(Nd4j.rand(DataType.FLOAT, 3, 4), Nothing))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPlaceholderShapeValidation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPlaceholderShapeValidation(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim scalar As SDVariable = sd.scalar("scalar", 0.0f)
			Dim ph1 As SDVariable = sd.placeHolder("ph1", DataType.FLOAT, 3, 4)
			Dim ph2 As SDVariable = sd.placeHolder("ph2", DataType.FLOAT, -1, 4)
			Dim ph3 As SDVariable = sd.placeHolder("ph3", DataType.FLOAT, 3, -1)
			Dim ph4 As SDVariable = sd.placeHolder("ph4", DataType.FLOAT, -1, -1)

			Dim correctShape As INDArray = Nd4j.create(DataType.FLOAT, 3, 4)
			Dim wrongShape As INDArray = Nd4j.create(DataType.FLOAT, 2, 3)
			Dim wrongRank1 As INDArray = Nd4j.create(DataType.FLOAT, 1)
			Dim wrongRank2 As INDArray = Nd4j.create(DataType.FLOAT, 3, 4, 5)
			For Each v As SDVariable In New SDVariable(){ph1, ph2, ph3, ph4}
				v.Array = correctShape

				If v IsNot ph4 Then
					Try
						v.Array = wrongShape
						fail("Expected exception")
					Catch t As Exception
						Dim msg As String = t.Message
						assertTrue(msg.Contains("shape") AndAlso msg.Contains("[2, 3]") AndAlso msg.Contains(Arrays.toString(v.placeholderShape())),msg)
					End Try
				End If

				Try
					v.Array = wrongRank1
					fail("Expected exception")
				Catch t As Exception
					Dim msg As String = t.Message
					assertTrue(msg.Contains("shape") AndAlso msg.Contains("[1]") AndAlso msg.Contains(Arrays.toString(v.placeholderShape())),msg)
				End Try

				Try
					v.Array = wrongRank2
					fail("Expected exception")
				Catch t As Exception
					Dim msg As String = t.Message
					assertTrue(msg.Contains("shape") AndAlso msg.Contains("[3, 4, 5]") AndAlso msg.Contains(Arrays.toString(v.placeholderShape())),msg)
				End Try
			Next v

			'Also try training:
			Dim sum As SDVariable = sd.math_Conflict.mergeAdd(New SDVariable(){ph1, ph2, ph3, ph4})
			Dim mean As SDVariable = sum.add(scalar).mean()
			Dim mds As New MultiDataSet(New INDArray(){wrongShape, wrongShape, wrongShape, wrongShape}, Nothing)

			sd.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("ph1", "ph2", "ph3", "ph4").markLabelsUnused().updater(New Adam(1e-3)).build()

			Try
				sd.fit(mds)
			Catch t As Exception
				Dim msg As String = t.Message
				assertTrue(msg.Contains("shape") AndAlso msg.Contains("[2, 3]"),msg)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInferenceWithoutLabel(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInferenceWithoutLabel(ByVal backend As Nd4jBackend)
			'We don't need a value for the label placeholder to calculate most values here

			Dim sd As SameDiff = SameDiff.create()

			Dim nIn As Integer = 4
			Dim minibatch As Integer = 3
			Dim input As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)

			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 3))

			Dim mmul As SDVariable = input.mmul(w).add(b)
			Dim softmax As SDVariable = sd.nn().softmax("softmax", mmul)
			Dim loss As SDVariable = sd.loss().logLoss("loss", label, softmax)

			Dim inputArr As INDArray = Nd4j.rand(DataType.FLOAT, minibatch, nIn)

			Dim m As IDictionary(Of String, INDArray) = sd.output(Collections.singletonMap("in", inputArr), "softmax")
			assertEquals(1, m.Count)
			assertTrue(m.ContainsKey("softmax"))

			Dim [out] As INDArray = m("softmax")

			Dim labelUnused As INDArray = Nd4j.rand(DataType.FLOAT, minibatch, 3)
			Dim allPh As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			allPh("in") = inputArr
			allPh("label") = labelUnused
			m = sd.output(allPh, "softmax")
			assertEquals(1, m.Count)
			assertTrue(m.ContainsKey("softmax"))
			Dim out2 As INDArray = m("softmax")
			assertEquals([out], out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInferenceWithoutUnnecessaryPlaceholders(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInferenceWithoutUnnecessaryPlaceholders(ByVal backend As Nd4jBackend)
			'We don't need an array for 2 of the placeholders to calculate the

			Dim sd As SameDiff = SameDiff.create()

			Dim nIn As Integer = 4
			Dim minibatch As Integer = 3
			Dim input As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)

			Dim input2 As SDVariable = sd.placeHolder("in2", DataType.FLOAT) 'Scalar

			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 3))

			Dim mmul As SDVariable = input.mmul(w).add(b)
			Dim softmax As SDVariable = sd.nn().softmax("softmax", mmul)
			Dim loss As SDVariable = sd.loss().logLoss("loss", label, softmax)
			Dim loss2 As SDVariable = softmax.mul(input2)

			Dim inputArr As INDArray = Nd4j.rand(DataType.FLOAT, minibatch, nIn)

			Dim m As IDictionary(Of String, INDArray) = sd.output(Collections.singletonMap("in", inputArr), "softmax")
			assertEquals(1, m.Count)
			assertTrue(m.ContainsKey("softmax"))

			Dim [out] As INDArray = m("softmax")

			Dim labelUnused As INDArray = Nd4j.rand(DataType.FLOAT, minibatch, 3)
			Dim allPh As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			allPh("in") = inputArr
			allPh("label") = labelUnused
			allPh("in2") = Nd4j.scalar(1.0f)
			m = sd.output(allPh, "softmax")
			assertEquals(1, m.Count)
			assertTrue(m.ContainsKey("softmax"))
			Dim out2 As INDArray = m("softmax")
			assertEquals([out], out2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConvertDTypes1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConvertDTypes1(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.var("x", Nd4j.rand(DataType.FLOAT, 3, 4))
			Dim y As SDVariable = sd.var("y", Nd4j.rand(DataType.FLOAT, 4, 2))
			Dim z As SDVariable = x.mmul("z", y)
			Dim tanh As SDVariable = sd.math().tanh("tanh", z)
			Dim stdev As SDVariable = tanh.std("stdev", True)

			assertEquals(DataType.FLOAT, x.dataType())
			assertEquals(DataType.FLOAT, y.dataType())
			assertEquals(DataType.FLOAT, z.dataType())
			assertEquals(DataType.FLOAT, tanh.dataType())
			assertEquals(DataType.FLOAT, stdev.dataType())

			Dim [out] As IDictionary(Of String, INDArray) = sd.output(DirectCast(Nothing, IDictionary(Of String, INDArray)), "x", "y", "z", "tanh", "stdev")
			For Each e As KeyValuePair(Of String, INDArray) In [out].SetOfKeyValuePairs()
				assertEquals(DataType.FLOAT, e.Value.dataType(),e.Key)
			Next e

			assertEquals(DataType.FLOAT, x.Arr.dataType())
			assertEquals(DataType.FLOAT, y.Arr.dataType())

			Dim toConvert As IDictionary(Of String, DataType) = New Dictionary(Of String, DataType)()
			toConvert("x") = DataType.DOUBLE
			toConvert("y") = DataType.DOUBLE
			sd.convertDataTypes(toConvert)

			assertEquals(DataType.DOUBLE, x.dataType())
			assertEquals(DataType.DOUBLE, y.dataType())
			assertEquals(DataType.DOUBLE, z.dataType())
			assertEquals(DataType.DOUBLE, tanh.dataType())
			assertEquals(DataType.DOUBLE, stdev.dataType())

			[out] = sd.output(DirectCast(Nothing, IDictionary(Of String, INDArray)), "x", "y", "z", "tanh", "stdev")
			For Each e As KeyValuePair(Of String, INDArray) In [out].SetOfKeyValuePairs()
				assertEquals(DataType.DOUBLE, e.Value.dataType(),e.Key)
			Next e

			assertEquals(DataType.DOUBLE, x.Arr.dataType())
			assertEquals(DataType.DOUBLE, y.Arr.dataType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConvertDTypes2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConvertDTypes2(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3, 4)
			Dim y As SDVariable = sd.var("y", Nd4j.rand(DataType.FLOAT, 1, 4))
			Dim xD As SDVariable = x.castTo("xD", DataType.DOUBLE)
			Dim yD As SDVariable = y.castTo("yD", DataType.DOUBLE)
			Dim add As SDVariable = xD.add("a", yD)
			Dim relu As SDVariable = sd.nn().relu("r", add, 1)

			assertEquals(DataType.FLOAT, x.dataType())
			assertEquals(DataType.FLOAT, y.dataType())
			assertEquals(DataType.DOUBLE, xD.dataType())
			assertEquals(DataType.DOUBLE, yD.dataType())
			assertEquals(DataType.DOUBLE, add.dataType())
			assertEquals(DataType.DOUBLE, relu.dataType())

			Dim ph As IDictionary(Of String, INDArray) = Collections.singletonMap("x", Nd4j.rand(DataType.FLOAT, 3, 4))

			Dim [out] As IDictionary(Of String, INDArray) = sd.output(ph, "x", "y", "xD", "yD", "a", "r")
			For Each e As KeyValuePair(Of String, INDArray) In [out].SetOfKeyValuePairs()
				If e.Key.Equals("x") OrElse e.Key.Equals("y") Then
					assertEquals(DataType.FLOAT, e.Value.dataType(),e.Key)
				Else
					assertEquals(DataType.DOUBLE, e.Value.dataType(),e.Key)
				End If
			Next e

			assertEquals(DataType.FLOAT, y.Arr.dataType())

			Dim toConvert As IDictionary(Of String, DataType) = New Dictionary(Of String, DataType)()
			toConvert("x") = DataType.DOUBLE
			toConvert("y") = DataType.DOUBLE
			sd.convertDataTypes(toConvert)

			assertEquals(DataType.DOUBLE, x.dataType())
			assertEquals(DataType.DOUBLE, y.dataType())
			assertEquals(DataType.DOUBLE, xD.dataType())
			assertEquals(DataType.DOUBLE, yD.dataType())
			assertEquals(DataType.DOUBLE, add.dataType())
			assertEquals(DataType.DOUBLE, relu.dataType())

			[out] = sd.output(ph, "x", "y", "xD", "yD", "a", "r")
			For Each e As KeyValuePair(Of String, INDArray) In [out].SetOfKeyValuePairs()
				assertEquals(DataType.DOUBLE, e.Value.dataType(),e.Key)
			Next e

			assertEquals(DataType.DOUBLE, y.Arr.dataType())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGradFnRequiredVars(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGradFnRequiredVars(ByVal backend As Nd4jBackend)
			'User can explicitly request that gradients for specific vars are available when differentiating (creating grad function),
			' even if they normally wouldn't be needed or calculated

			For Each reqPhVar As Boolean In New Boolean(){False, True}
	'        for(boolean reqPhVar : new boolean[]{true}){

				Dim sd As SameDiff = SameDiff.create()
				Dim ph As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 5)
				Dim add As SDVariable = ph.add(1.0)
				Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 5, 4))
				Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 4))

				Dim mmul As SDVariable = add.mmul(w).add(b)

				Dim loss As SDVariable = mmul.std(True)

				Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 1, 5)

				If reqPhVar Then
					sd.createGradFunction("in")
					assertNotNull(ph.gradient())
					assertNotNull(w.gradient())
					assertNotNull(b.gradient())

					Dim m As IDictionary(Of String, INDArray) = sd.calculateGradients(Collections.singletonMap("in", [in]), ph.name(), w.name())
					assertNotNull(m(ph.name()))
					assertNotNull(m(w.name()))
				Else
					sd.createGradFunction()
					assertNull(ph.gradient())
					assertNotNull(w.gradient())
					assertNotNull(b.gradient())
				End If
			Next reqPhVar


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIf() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIf()
			Dim sd As SameDiff = SameDiff.create()
			Dim a As SDVariable = sd.placeHolder("a", DataType.DOUBLE)
			Dim b As SDVariable = sd.var("b", Nd4j.createFromArray(5.0))
			Dim c As SDVariable = sd.var("c", Nd4j.createFromArray(9.0))

			Dim output As SDVariable = sd.ifCond("out", Nothing, Function(s) a.lt(b), Function(s) c, Sub(s) c.add(5))

			Dim firstBranch As IDictionary(Of String, INDArray) = Maps.newHashMap()
			firstBranch("a") = Nd4j.createFromArray(3.0)
			assertEquals(Nd4j.createFromArray(9.0), sd.output(firstBranch, "out")("out"))

			Dim secondBranch As IDictionary(Of String, INDArray) = Maps.newHashMap()
			secondBranch("a") = Nd4j.createFromArray(7.0)
	'        System.out.println(sd.summary());
			sd.summary()
			Dim outArr As INDArray = sd.output(secondBranch, "out")("out")
			assertEquals(Nd4j.createFromArray(14.0), outArr)

			Dim bb As ByteBuffer = sd.asFlatBuffers(False)
			sd = SameDiff.fromFlatBuffers(bb)

			assertEquals(Nd4j.createFromArray(9.0), sd.output(firstBranch, "out")("out"))
			assertEquals(Nd4j.createFromArray(14.0), sd.output(secondBranch, "out")("out"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedIf() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNestedIf()
			Dim sd As SameDiff = SameDiff.create()
			Dim a As SDVariable = sd.var("a", Nd4j.createFromArray(2.0))
			Dim b As SDVariable = sd.var("b", Nd4j.createFromArray(5.0))
			Dim c As SDVariable = sd.var("c", Nd4j.createFromArray(9.0))
			Dim d As SDVariable = sd.var("d", Nd4j.createFromArray(-7.0))

			Dim output As SDVariable = sd.ifCond("out", Nothing, Function(s) a.lt(b), Function(s) s.ifCond(Function(sd2) d.lte(0), Sub(sd2) c.add(1), Function(sd2) d), Sub(s) c.add(5))
			Dim [out] As INDArray = output.eval()
			assertEquals(Nd4j.createFromArray(10.0), [out])

			sd = SameDiff.fromFlatBuffers(sd.asFlatBuffers(False))

			assertEquals(Nd4j.createFromArray(10.0), sd.output(Collections.emptyMap(), "out")("out"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhile() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWhile()

			Dim sd As SameDiff = SameDiff.create()
			Dim countIn As SDVariable = sd.constant(5)
			Dim sumIn As SDVariable = sd.constant(0)

			Dim sum() As SDVariable = sd.whileLoop("while_1", New SDVariable(){countIn, sumIn}, Function(s, vars) vars(0).gt(0), Function(s, vars) New SDVariable(){vars(0).sub(1), vars(1).add(vars(0))})

			Dim [out] As INDArray = sum(1).eval()
			assertEquals(15, [out].getInt(0))

			Dim outName As String = sum(1).name()

			sd = SameDiff.fromFlatBuffers(sd.asFlatBuffers(False))

			assertEquals(15, sd.output(Collections.emptyMap(), outName)(outName).getInt(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testNestedWhile() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNestedWhile()
			Dim sd As SameDiff = SameDiff.create()
			Dim countIn As SDVariable = sd.constant(5)
			Dim sumIn As SDVariable = sd.constant(0)
			Dim sum2 As SDVariable = sd.constant(0)
			'TODO creating constant instead of using sum2 causes errors

			Dim sum() As SDVariable = sd.whileLoop(New SDVariable(){countIn, sumIn}, Function(s, vars) vars(0).gt(0), Function(s, vars) New SDVariable(){vars(0).sub(1), vars(1).add(s.whileLoop(New SDVariable(){vars(0), sum2}, Function(sd2, vars2) vars2(0).gt(0), Function(sd2, vars2) New SDVariable(){vars2(0).sub(1), vars2(1).add(vars2(0))})(1))})

			Dim [out] As INDArray = sum(1).eval()
			assertEquals(35, [out].getInt(0))

			Dim outName As String = sum(1).name()

			sd = SameDiff.fromFlatBuffers(sd.asFlatBuffers(False))

			assertEquals(35, sd.output(Collections.emptyMap(), outName)(outName).getInt(0))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWhileIf() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNestedWhileIf()
			Dim sd As SameDiff = SameDiff.create()
			Dim countIn As SDVariable = sd.constant(5)
			Dim sumIn As SDVariable = sd.constant(0)
			Dim hundred As SDVariable = sd.constant(100)

			Dim sum() As SDVariable = sd.whileLoop(New SDVariable(){countIn, sumIn}, Function(s, vars) vars(0).gte(0), Function(s, vars) New SDVariable(){vars(0).sub(1), vars(1).add(s.ifCond(Function(sd2) vars(0).eq(0), Sub(sd2) vars(0).add(100), Function(sd2) vars(0)))})

			Dim [out] As INDArray = sum(1).eval()
			assertEquals(115, [out].getInt(0))

			Dim outName As String = sum(1).name()

			sd = SameDiff.fromFlatBuffers(sd.asFlatBuffers(False))

			assertEquals(115, sd.output(Collections.emptyMap(), outName)(outName).getInt(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMod_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMod_1(ByVal backend As Nd4jBackend)
			Dim sd As val = SameDiff.create()
			Dim initial As val = sd.constant("initial", Nd4j.createFromArray(5.0f, 6.0f, 7.0f))
			Dim four As val = sd.constant("four", 4.0f)
			Dim [mod] As val = initial.mod("mod", four)

			Dim e As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f)

			assertEquals(e, [mod].eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void castShapeTest1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub castShapeTest1(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.constant(Nd4j.createFromArray(1, 2, 3, 4))
			Dim casted As SDVariable = x.castTo(DataType.FLOAT)

			assertEquals(casted.dataType(), DataType.FLOAT)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void castShapeTestEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub castShapeTestEmpty(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.constant(Nd4j.empty(DataType.INT))
			Dim casted As SDVariable = x.castTo(DataType.FLOAT)

			assertEquals(casted.dataType(), DataType.FLOAT)
			assertTrue(casted.ShapeDescriptor.Empty)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyShapeVar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyShapeVar(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Try
				sd.var(DataType.FLOAT, 1, 0, 2)
				fail("Expected exception")
			Catch e As System.ArgumentException
				Dim m As String = e.Message
				assertTrue(m.Contains("variable") AndAlso m.Contains("empty") AndAlso m.Contains("0"),m)
			End Try

			Try
				sd.var(Nd4j.create(1, 0, 2))
				fail("Expected exception")
			Catch e As System.ArgumentException
				Dim m As String = e.Message.ToLower()
				assertTrue(m.Contains("variable") AndAlso m.Contains("empty") AndAlso m.Contains("0"),m)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPReLU(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPReLU(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim input As SDVariable = sd.constant(Nd4j.createFromArray(New Integer()()(){
				New Integer()() {
					New Integer() {-10, 10, 10, -10},
					New Integer() {10, 10, -10, -10}
				}
			}).castTo(DataType.DOUBLE))

			Dim alpha As SDVariable = sd.var(Nd4j.createFromArray(0.01, 0.1).castTo(DataType.DOUBLE))

			Dim [out] As SDVariable = sd.nn_Conflict.prelu("out", input, alpha, 2)

			Dim tc As TestCase = (New TestCase(sd)).expected("out", Nd4j.createFromArray(New Double()()(){
				New Double()() {
					New Double() {-0.1, 10, 10, -0.1},
					New Double() {10, 10, -1, -1}
				}
			}).castTo(DataType.DOUBLE)).gradientCheck(True)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSameDiffSeedReproducibilityVarInit(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSameDiffSeedReproducibilityVarInit(ByVal backend As Nd4jBackend)

			Dim sd0 As SameDiff = SameDiff.create()
			Dim sd1 As SameDiff = SameDiff.create()
			Nd4j.Random.setSeed(12345)
			Dim rand0 As SDVariable = sd0.var("random", New UniformInitScheme("c"c, 3), DataType.FLOAT, 3, 1)

			Nd4j.Random.setSeed(12345)
			Dim rand1 As SDVariable = sd1.var("random", New UniformInitScheme("c"c, 3), DataType.FLOAT, 3, 1)


	'        Nd4j.getRandom().setSeed(0);
	'        System.out.println(rand0.eval());
	'
	'        Nd4j.getRandom().setSeed(0);
	'        System.out.println(rand1.eval());

			Dim a0 As INDArray = rand0.eval()
			Nd4j.Random.setSeed(0)
			Dim a1 As INDArray = rand1.eval()
			assertEquals(a0, a1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCalculateGradientsAndOutputs(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCalculateGradientsAndOutputs(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 3))
			Dim z As SDVariable = [in].mmul(w).add("z", b)
			Dim softmax As SDVariable = sd.nn_Conflict.softmax("softmax", z, 0)

			Dim ph As IDictionary(Of String, INDArray) = Collections.singletonMap("in", Nd4j.rand(DataType.FLOAT, 2, 4))
			Dim outputs As IList(Of String) = New List(Of String) From {"in", "z", "softmax"}
			Dim grads As IList(Of String) = New List(Of String) From {"in", "w", "z"}

			Dim oag As OutAndGrad = sd.calculateGradientsAndOutputs(ph, outputs, grads)
			Dim outs As IDictionary(Of String, INDArray) = oag.getOutputs()
			Dim g As IDictionary(Of String, INDArray) = oag.getGradients()


			Dim outExp As IDictionary(Of String, INDArray) = sd.output(ph, outputs)
			Dim gExp As IDictionary(Of String, INDArray) = sd.calculateGradients(ph, grads)

			assertEquals(outExp, outs)
			assertEquals(gExp, g)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatVariableGrad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatVariableGrad(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim label As SDVariable = sd.var("label", DataType.FLOAT, 3, 4)
			Dim a As SDVariable = sd.var("a", DataType.FLOAT, 3, 2)
			Dim b As SDVariable = sd.var("b", DataType.FLOAT, 3, 2)
			Dim inputArr As INDArray = Nd4j.rand(3,4)
			Dim labelArr As INDArray = Nd4j.rand(3,4)
			Dim c As SDVariable = sd.concat("concat", 1, a, b)
			Dim loss As SDVariable = sd.math().pow(c.sub(label), 2)
			sd.setLossVariables(loss)
			sd.associateArrayWithVariable(labelArr, label)
			sd.associateArrayWithVariable(inputArr.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 2)), a)
			sd.associateArrayWithVariable(inputArr.get(NDArrayIndex.all(), NDArrayIndex.interval(2, 4)), b)
			Dim map As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing, "a", "b", "concat")
			Dim concatArray As INDArray = Nd4j.hstack(map("a"), map("b"))
			assertEquals(concatArray, map("concat"))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceVariableGrad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceVariableGrad(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim label As SDVariable = sd.var("label", DataType.FLOAT, 3, 4)
			Dim input As SDVariable = sd.var("input", DataType.FLOAT, 3, 4)
			Dim inputArr As INDArray = Nd4j.rand(3,4)
			Dim labelArr As INDArray = Nd4j.rand(3,4)
			Dim a As SDVariable = input.get(SDIndex.all(), SDIndex.interval(0, 2))
			Dim b As SDVariable = input.get(SDIndex.all(), SDIndex.interval(2, 4))
			Dim c As SDVariable = sd.concat("concat", 1, a, b)
			Dim loss As SDVariable = sd.math().pow(c.sub(label), 2)
			sd.setLossVariables(loss)
			sd.associateArrayWithVariable(labelArr, label)
			sd.associateArrayWithVariable(inputArr, input)
			Dim map As IDictionary(Of String, INDArray) = sd.calculateGradients(Nothing,"input", "concat")
			assertEquals(map("input"), map("concat"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrainingConfigJson(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTrainingConfigJson(ByVal backend As Nd4jBackend)
			For Each e As IEvaluation In New IEvaluation(){
				New Evaluation(),
				New RegressionEvaluation(),
				New EvaluationBinary(),
				New ROC(),
				New ROCMultiClass(),
				New ROCBinary(),
				New EvaluationCalibration()
			}
				Dim config As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(New Adam(0.1)).dataSetFeatureMapping("out").dataSetLabelMapping("label").trainEvaluation("out", 0, e).build()
				Dim json As String = config.toJson()
				Dim fromJson As TrainingConfig = TrainingConfig.fromJson(json)
				assertEquals(config, fromJson)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRngSanityCheck(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRngSanityCheck(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			For Each dt As DataType In New DataType(){DataType.FLOAT, DataType.DOUBLE, DataType.BFLOAT16}
				If Not dt.isNumerical() Then
					Continue For
				End If
'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim sameDiff_Conflict As SameDiff = SameDiff.create()
				Dim indaShape As INDArray = Nd4j.createFromArray(3, 10)
				Dim sdShape As SDVariable = sameDiff_Conflict.constant(indaShape)
				Dim random As SDVariable = sameDiff_Conflict.random().uniform("data", 0.0, 10.0, dt, 3, 10)
				Dim [out] As INDArray = random.eval()
				Dim s As String = [out].ToString()
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMissingPlaceholderError(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMissingPlaceholderError(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.placeHolder("labels", DataType.DOUBLE, -1, nOut)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim loss As SDVariable = sd.loss().absoluteDifference("loss", labels, predictions, Nothing, reduction)

			Try
				loss.eval()
				fail("Exception should have been thrown")
			Catch e As System.InvalidOperationException
				Dim msg As String = e.Message
				assertTrue(msg.Contains("""labels""") AndAlso msg.Contains("No array was provided"),msg)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEquals1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEquals1(ByVal backend As Nd4jBackend)

			Dim sd1 As SameDiff = SameDiff.create()
			Dim sd2 As SameDiff = SameDiff.create()

			assertEquals(sd1, sd2)

			Dim p1 As SDVariable = sd1.placeHolder("ph", DataType.FLOAT, -1, 10)
			Dim p2 As SDVariable = sd2.placeHolder("ph", DataType.FLOAT, -1, 10)

			assertEquals(sd1, sd2)

			Dim w1 As SDVariable = sd1.constant("c1",1.0f)
			Dim w2 As SDVariable = sd2.constant("c1",1.0f)

			assertEquals(sd1, sd2)

			Dim a1 As SDVariable = p1.add("add", w1)
			Dim a2 As SDVariable = p2.add("add", w2)

			assertEquals(sd1, sd2)

			Dim w1a As SDVariable = sd1.constant("c2", 2.0f)
			Dim w2a As SDVariable = sd2.constant("cX", 2.0f)

			assertNotEquals(sd1, sd2)
			w2a.rename("c2")

			assertEquals(sd1, sd2)

			sd2.createGradFunction("ph")

			assertEquals(sd1, sd2)

			w2a.Arr.assign(3.0f)

			assertNotEquals(sd1, sd2)

			w1a.Arr.assign(3.0f)
			assertEquals(sd1, sd2)

			Dim s1 As SDVariable = p1.sub("op", w1)
			Dim s2 As SDVariable = p2.add("op", w1)
			assertNotEquals(sd1, sd2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2DWeightsFormat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2DWeightsFormat(ByVal backend As Nd4jBackend)
			Dim bS As Integer=2, iH As Integer=4, iW As Integer=3, iC As Integer=4, oC As Integer=3, kH As Integer=3, kW As Integer=2, sH As Integer=1, sW As Integer=1, pH As Integer=0, pW As Integer=0, dH As Integer=1, dW As Integer=1
			Dim oH As Integer=2, oW As Integer=2
			Dim sd As SameDiff = SameDiff.create()

			Dim format As WeightsFormat = WeightsFormat.OIYX

			Dim inArr As INDArray = Nd4j.linspace(DataType.FLOAT, 25, -0.5, 96).reshape(New Long(){bS, iC, iH, iW})
			Dim weights As INDArray = Nd4j.createFromArray(New Single(){ -3.0f, -1.8f, -0.6f, 0.6f, 1.8f, 3.0f, -2.7f, -1.5f, -0.3f, 0.9f, 2.1f, 3.3f, -2.4f, -1.2f, 0.0f, 1.2f, 2.4f, 3.6f, -2.1f, -0.9f, 0.3f, 1.5f, 2.7f, 3.9f, -2.9f, -1.7f, -0.5f, 0.7f, 1.9f, 3.1f, -2.6f, -1.4f, -0.2f, 1.0f, 2.2f, 3.4f, -2.3f, -1.1f, 0.1f, 1.3f, 2.5f, 3.7f, -2.0f, -0.8f, 0.4f, 1.6f, 2.8f, 4.0f, -2.8f, -1.6f, -0.4f, 0.8f, 2.0f, 3.2f, -2.5f, -1.3f, -0.1f, 1.1f, 2.3f, 3.5f, -2.2f, -1.0f, 0.2f, 1.4f, 2.6f, 3.8f, -1.9f, -0.7f, 0.5f, 1.7f, 2.9f, 4.1f}).reshape(New Long(){oC, iC, kH, kW})

			Dim bias As INDArray = Nd4j.createFromArray(New Single(){-1, 2, 0.5f})

			Dim sdInput As SDVariable = sd.var("in", inArr)
			Dim sdWeights As SDVariable = sd.var("dW", weights)
			Dim sdBias As SDVariable = sd.var("b", bias)

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(pH).pW(pW).sH(sH).sW(sW).dH(dH).dW(dW).isSameMode(False).weightsFormat(format).build()

			Dim [out] As SDVariable = sd.cnn().conv2d(sdInput, sdWeights, sdBias, c)

			assertArrayEquals(New Long(){bS, oC, oH, oW}, [out].eval().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2DDifferentWeightsFormat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2DDifferentWeightsFormat(ByVal backend As Nd4jBackend)
			Dim bS As Integer=2, iH As Integer=4, iW As Integer=3, iC As Integer=4, oC As Integer=3, kH As Integer=3, kW As Integer=2, sH As Integer=1, sW As Integer=1, pH As Integer=0, pW As Integer=0, dH As Integer=1, dW As Integer=1
			Dim oH As Integer=2, oW As Integer=2
			Dim sd As SameDiff = SameDiff.create()

			Dim inArr As INDArray = Nd4j.linspace(DataType.FLOAT, 25, -0.5, 96).reshape(New Long(){bS, iC, iH, iW})
			Dim weights As INDArray = Nd4j.rand(DataType.FLOAT, oC, iC, kH, kW)

			Dim bias As INDArray = Nd4j.createFromArray(New Single(){-1, 2, 0.5f})

			Dim sdInput As SDVariable = sd.var("in", inArr)
			Dim sdWeights As SDVariable = sd.var("dW", weights)
			Dim sdBias As SDVariable = sd.var("b", bias)

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(pH).pW(pW).sH(sH).sW(sW).dH(dH).dW(dW).isSameMode(False).weightsFormat(WeightsFormat.OIYX).build()

			Dim [out] As SDVariable = sd.cnn().conv2d(sdInput, sdWeights, sdBias, c)

			assertArrayEquals(New Long(){bS, oC, oH, oW}, [out].eval().shape())

			weights = weights.permute(0,2,3,1)
			Dim permutedWeights As SDVariable = sd.var("weights2", weights)

			' Shape per format tip:
			'[3, 4, 3, 2] - OIYX
			'[3, 3, 2, 4] - OYXI
			'[3, 2, 4, 2] - YXIO
			Dim c2 As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(pH).pW(pW).sH(sH).sW(sW).dH(dH).dW(dW).isSameMode(False).weightsFormat(WeightsFormat.OYXI).build()

			Dim out2 As SDVariable = sd.cnn().conv2d(sdInput, permutedWeights, sdBias, c2)
			assertEquals([out].eval(), out2.eval())
		End Sub
	End Class

End Namespace