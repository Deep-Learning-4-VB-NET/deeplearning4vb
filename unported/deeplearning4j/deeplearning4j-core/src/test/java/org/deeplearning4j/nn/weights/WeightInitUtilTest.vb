Imports System
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Distributions = org.deeplearning4j.nn.conf.distribution.Distributions
Imports GaussianDistribution = org.deeplearning4j.nn.conf.distribution.GaussianDistribution
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.weights

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Weight Init Util Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class WeightInitUtilTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class WeightInitUtilTest
		Inherits BaseDL4JTest

		Protected Friend fanIn As Integer = 3

		Protected Friend fanOut As Integer = 2

		Protected Friend shape() As Integer = { fanIn, fanOut }

		Protected Friend dist As Distribution = Distributions.createDistribution(New GaussianDistribution(0.0, 0.1))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void doBefore()
		Friend Overridable Sub doBefore()
			Nd4j.Random.setSeed(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Distribution") void testDistribution()
		Friend Overridable Sub testDistribution()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			' fan in/out not used
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(-1, -1, shape, WeightInit.DISTRIBUTION, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim weightsExpected As INDArray = dist.sample(params)
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Relu") void testRelu()
		Friend Overridable Sub testRelu()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.RELU, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim weightsExpected As INDArray = Nd4j.randn("f"c, shape).muli(FastMath.sqrt(2.0 / fanIn))
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sigmoid Uniform") void testSigmoidUniform()
		Friend Overridable Sub testSigmoidUniform()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.SIGMOID_UNIFORM, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim min As Double = -4.0 * Math.Sqrt(6.0 / CDbl(shape(0) + shape(1)))
			Dim max As Double = 4.0 * Math.Sqrt(6.0 / CDbl(shape(0) + shape(1)))
			Dim weightsExpected As INDArray = Nd4j.Distributions.createUniform(min, max).sample(Nd4j.createUninitialized(shape, "f"c))
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Uniform") void testUniform()
		Friend Overridable Sub testUniform()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.UNIFORM, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim a As Double = 1.0 / Math.Sqrt(fanIn)
			Dim weightsExpected As INDArray = Nd4j.Distributions.createUniform(-a, a).sample(Nd4j.create(shape, "f"c))
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Xavier") void testXavier()
		Friend Overridable Sub testXavier()
			Nd4j.Random.setSeed(123)
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.XAVIER, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim weightsExpected As INDArray = Nd4j.randn("f"c, shape)
			weightsExpected.muli(FastMath.sqrt(2.0 / (fanIn + fanOut)))
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Xavier Fan In") void testXavierFanIn()
		Friend Overridable Sub testXavierFanIn()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.XAVIER_FAN_IN, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim weightsExpected As INDArray = Nd4j.randn("f"c, shape)
			weightsExpected.divi(FastMath.sqrt(fanIn))
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Xavier Legacy") void testXavierLegacy()
		Friend Overridable Sub testXavierLegacy()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.XAVIER_LEGACY, dist, params)
			' expected calculation
			Nd4j.Random.setSeed(123)
			Dim weightsExpected As INDArray = Nd4j.randn("f"c, shape)
			weightsExpected.muli(FastMath.sqrt(1.0 / (fanIn + fanOut)))
			assertEquals(weightsExpected, weightsActual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero") void testZero()
		Friend Overridable Sub testZero()
			Dim params As INDArray = Nd4j.create(shape, "f"c)
			Dim weightsActual As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.ZERO, dist, params)
			' expected calculation
			Dim weightsExpected As INDArray = Nd4j.create(shape, "f"c)
			assertEquals(weightsExpected, weightsActual)
		End Sub
	End Class

End Namespace