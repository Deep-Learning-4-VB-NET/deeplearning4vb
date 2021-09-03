Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports JDKRandomGenerator = org.apache.commons.math3.random.JDKRandomGenerator
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports org.junit.jupiter.api
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Mean = org.nd4j.linalg.api.ops.impl.reduce.floating.Mean
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DistributionUniform = org.nd4j.linalg.api.ops.random.custom.DistributionUniform
Imports RandomBernoulli = org.nd4j.linalg.api.ops.random.custom.RandomBernoulli
Imports RandomGamma = org.nd4j.linalg.api.ops.random.custom.RandomGamma
Imports RandomPoisson = org.nd4j.linalg.api.ops.random.custom.RandomPoisson
Imports RandomShuffle = org.nd4j.linalg.api.ops.random.custom.RandomShuffle
Imports AlphaDropOut = org.nd4j.linalg.api.ops.random.impl.AlphaDropOut
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports BinomialDistribution = org.nd4j.linalg.api.ops.random.impl.BinomialDistribution
Imports DropOut = org.nd4j.linalg.api.ops.random.impl.DropOut
Imports DropOutInverted = org.nd4j.linalg.api.ops.random.impl.DropOutInverted
Imports GaussianDistribution = org.nd4j.linalg.api.ops.random.impl.GaussianDistribution
Imports Linspace = org.nd4j.linalg.api.ops.random.impl.Linspace
Imports LogNormalDistribution = org.nd4j.linalg.api.ops.random.impl.LogNormalDistribution
Imports TruncatedNormalDistribution = org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution
Imports UniformDistribution = org.nd4j.linalg.api.ops.random.impl.UniformDistribution
Imports DefaultRandom = org.nd4j.linalg.api.rng.DefaultRandom
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports NormalDistribution = org.nd4j.linalg.api.rng.distribution.impl.NormalDistribution
Imports OrthogonalDistribution = org.nd4j.linalg.api.rng.distribution.impl.OrthogonalDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports NativeRandom = org.nd4j.rng.NativeRandom
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.nd4j.linalg.rng



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.RNG) @NativeTag @Tag(TagNames.LONG_TEST) public class RandomTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RandomTests
		Inherits BaseNd4jTestWithBackends

		Private initialType As DataType
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(RandomTests).FullName)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			initialType = Nd4j.dataType()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			Nd4j.DataType = initialType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCrossBackendEquality1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCrossBackendEquality1(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {12}
			Dim mean As Double = 0
			Dim standardDeviation As Double = 1.0
			Dim exp As INDArray = Nd4j.create(New Double() {-0.832718168582558, 1.3312306172061867, -0.27101354040045766, 1.0368130323476494, -0.6257379511224601, 0.30653534119847814, 0.28250229228899343, -0.5464191486048424, 0.5182898732953277, 1.463107608378911, 0.5634855878214299, -1.4979616922031507})
			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.Executioner.exec(New GaussianDistribution(Nd4j.createUninitialized(shape, Nd4j.order()), mean, standardDeviation), Nd4j.Random)

			assertEquals(exp, arr)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistribution1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistribution1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.create(1000)
			Dim z2 As INDArray = Nd4j.create(1000)
			Dim distribution As New UniformDistribution(z1, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution, random1)
			Dim distribution2 As New UniformDistribution(z2, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution2, random2)

			For e As Integer = 0 To z1.length() - 1
				Dim val As Double = z1.getDouble(e)
				assertTrue(val >= 1.0 AndAlso val <= 2.0)
			Next e

			assertEquals(z1, z2)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistribution2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistribution2(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			log.info("States cpu: {}/{}", random1.rootState(), random1.nodeState())

			Dim z1 As INDArray = Nd4j.create(32)
			Dim z2 As INDArray = Nd4j.create(32)
			Dim distribution As New UniformDistribution(z1, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution, random1)
			log.info("States cpu: {}/{}", random1.rootState(), random1.nodeState())

			Dim distribution2 As New UniformDistribution(z2, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution2, random2)

			log.info("States cpu: {}/{}", random1.rootState(), random1.nodeState())

			For e As Integer = 0 To z1.length() - 1
				Dim val As Double = z1.getDouble(e)
				assertTrue(val >= 1.0 AndAlso val <= 2.0)
			Next e

			assertEquals(z1, z2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistribution3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistribution3(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.create(128)
			Dim z2 As INDArray = Nd4j.create(128)
			Dim distribution As New UniformDistribution(z1, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution, random1)
			Dim distribution2 As New UniformDistribution(z2, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution2, random1)

			assertNotEquals(z1, z2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistribution4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistribution4(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 99
				Nd4j.Random.setSeed(119)

				Dim z1 As INDArray = Nd4j.randn("f"c, New Integer() {1, 1000})

				Nd4j.Random.setSeed(119)

				Dim z2 As INDArray = Nd4j.randn("c"c, New Integer() {1, 1000})

				assertEquals(z1, z2,"Failed on iteration " & i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistribution5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistribution5(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 99
				Nd4j.Random.setSeed(120)

				Dim z1 As INDArray = Nd4j.rand("f"c, New Integer() {1, 1000})

				Nd4j.Random.setSeed(120)

				Dim z2 As INDArray = Nd4j.rand("c"c, New Integer() {1, 1000})

				assertEquals(z1, z2,"Failed on iteration " & i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDistribution6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDistribution6(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 99
				Nd4j.Random.setSeed(120)

				Dim z1 As INDArray = Nd4j.Executioner.exec(New BinomialDistribution(Nd4j.createUninitialized(1000), 10, 0.2))

				Nd4j.Random.setSeed(120)

				Dim z2 As INDArray = Nd4j.Executioner.exec(New BinomialDistribution(Nd4j.createUninitialized(1000), 10, 0.2))

				assertEquals(z1, z2,"Failed on iteration " & i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspace1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinspace1(ByVal backend As Nd4jBackend)
			Dim z1 As INDArray = Nd4j.linspace(1, 100, 200, DataType.DOUBLE)

			Dim linspace As New Linspace(CDbl(1), CDbl(100), 200, DataType.DOUBLE)
			Nd4j.Executioner.exec(linspace, Nd4j.Random)

			Dim z2 As INDArray = linspace.z()

			assertEquals(z1, z2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDropoutInverted1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDropoutInverted1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.ones(300)
			Dim z2 As INDArray = Nd4j.ones(300)
			Dim zDup As INDArray = z1.dup()

			Dim op1 As New DropOutInverted(z1, z1, 0.10)
			Nd4j.Executioner.exec(op1, random1)

			Dim op2 As New DropOutInverted(z2, z2, 0.10)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(zDup, z1)


			For x As Integer = 0 To z1.length() - 1
				assertEquals(z1.getFloat(x), z2.getFloat(x), 0.01f,"Failed on element: [" & x & "]")
			Next x
			assertEquals(z1, z2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDropout1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDropout1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.ones(300)
			Dim z2 As INDArray = Nd4j.ones(300)
			Dim zDup As INDArray = z1.dup()

			Dim op1 As New DropOut(z1, z1, 0.10)
			Nd4j.Executioner.exec(op1, random1)

			Dim op2 As New DropOut(z2, z2, 0.10)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(zDup, z1)

			assertEquals(z1, z2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAlphaDropout1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAlphaDropout1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.ones(300)
			Dim z2 As INDArray = Nd4j.ones(300)
			Dim zDup As INDArray = z1.dup()

			Dim op1 As New AlphaDropOut(z1, z1, 0.10, 0.3, 0.5, 0.7)
			Nd4j.Executioner.exec(op1, random1)

			Dim op2 As New AlphaDropOut(z2, z2, 0.10, 0.3, 0.5, 0.7)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(zDup, z1)

			assertEquals(z1, z2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGaussianDistribution1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGaussianDistribution1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.create(DataType.DOUBLE, 1000000)
			Dim z2 As INDArray = Nd4j.create(DataType.DOUBLE, 1000000)
			Dim zDup As INDArray = z1.like()

			Dim op1 As New GaussianDistribution(z1, 0.0, 1.0)
			Nd4j.Executioner.exec(op1, random1)

			Dim op2 As New GaussianDistribution(z2, 0.0, 1.0)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(zDup, z1)
			assertEquals(0.0, z1.meanNumber().doubleValue(), 0.01)

			assertEquals(1.0, z1.stdNumber().doubleValue(), 0.01)

			Dim d1() As Double = z1.toDoubleVector()
			Dim d2() As Double = z2.toDoubleVector()

			assertArrayEquals(d1, d2, 1e-4)

			assertEquals(z1, z2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGaussianDistribution2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGaussianDistribution2(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random3 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random4 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.create(100000)
			Dim z2 As INDArray = Nd4j.create(100000)
			Dim z3 As INDArray = Nd4j.create(100000)
			Dim z4 As INDArray = Nd4j.create(100000)

			random3.reSeed(8231)
			random4.reSeed(4453523)

			Dim op1 As New GaussianDistribution(z1, 0.0, 1.0)
			Nd4j.Executioner.exec(op1, random1)

			Dim op2 As New GaussianDistribution(z2, 0.0, 1.0)
			Nd4j.Executioner.exec(op2, random2)

			Dim op3 As New GaussianDistribution(z3, 0.0, 1.0)
			Nd4j.Executioner.exec(op3, random3)

			Dim op4 As New GaussianDistribution(z4, 0.0, 1.0)
			Nd4j.Executioner.exec(op4, random4)

			Nd4j.Executioner.commit()

			assertEquals(0.0, z1.meanNumber().doubleValue(), 0.01)
			assertEquals(1.0, z1.stdNumber().doubleValue(), 0.01)

			assertEquals(z1, z2)

			assertNotEquals(z1, z3)
			assertNotEquals(z2, z4)
			assertNotEquals(z3, z4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGaussianDistribution3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGaussianDistribution3(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.create(DataType.DOUBLE, 1000000)
			Dim z2 As INDArray = Nd4j.create(DataType.DOUBLE, 1000000)

			Dim op1 As New GaussianDistribution(z1, 1.0, 1.0)
			Nd4j.Executioner.exec(op1, random1)

			Dim op2 As New GaussianDistribution(z2, -1.0, 2.0)
			Nd4j.Executioner.exec(op2, random2)


			assertEquals(1.0, z1.meanNumber().doubleValue(), 0.01)
			assertEquals(1.0, z1.stdNumber().doubleValue(), 0.01)

			' check variance
			assertEquals(-1.0, z2.meanNumber().doubleValue(), 0.01)
			assertEquals(4.0, z2.varNumber().doubleValue(), 0.01)

			assertNotEquals(z1, z2)
		End Sub

		''' <summary>
		''' Uses a test of Gaussianity for testing the values out of GaussianDistribution
		''' See https://en.wikipedia.org/wiki/Anderson%E2%80%93Darling_test
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAndersonDarling(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAndersonDarling(ByVal backend As Nd4jBackend)

			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim z1 As INDArray = Nd4j.create(1000)

			Dim op1 As New GaussianDistribution(z1, 0.0, 1.0)
			Nd4j.Executioner.exec(op1, random1)

			Dim n As Long = z1.length()
			'using this just for the cdf
			Dim nd As Distribution = New NormalDistribution(random1, 0.0, 1.0)
			Nd4j.sort(z1, True)

			For i As Integer = 0 To n - 1

				Dim res As Double? = nd.cumulativeProbability(z1.getDouble(i))
				assertTrue(res >= 0.0)
				assertTrue(res <= 1.0)
				' avoid overflow when taking log later.
				If res = 0 Then
					res = 0.0000001
				End If
				If res = 1 Then
					res = 0.9999999
				End If
				z1.putScalar(i, res)
			Next i

			Dim A As Double = 0.0
			For i As Integer = 0 To n - 1

				A -= (2*i+1) * (Math.Log(z1.getDouble(i)) + Math.Log(1-z1.getDouble(n - i - 1)))
			Next i

			A = A / n - n
			A *= (1 + 4.0/n - 25.0/(n*n))

			assertTrue(A < 1.8692,"Critical (max) value for 1000 points and confidence α = 0.0001 is 1.8692, received: " & A)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testStepOver1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStepOver1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z0 As INDArray = Nd4j.Executioner.exec(New GaussianDistribution(Nd4j.createUninitialized(DataType.DOUBLE, 1000000), 0.0, 1.0))

			assertEquals(0.0, z0.meanNumber().doubleValue(), 0.01)
			assertEquals(1.0, z0.stdNumber().doubleValue(), 0.01)

			random1.setSeed(119)

			Dim z1 As INDArray = Nd4j.zeros(DataType.DOUBLE, 55000000)
			Dim z2 As INDArray = Nd4j.zeros(DataType.DOUBLE, 55000000)

			Dim op1 As New GaussianDistribution(z1, 0.0, 1.0)
			Nd4j.Executioner.exec(op1, random1)

			Dim match As INDArray = Nd4j.Executioner.exec(New MatchCondition(z1, Conditions.Nan))
			assertEquals(0.0f, match.getFloat(0), 0.01f)

			assertEquals(0.0, z1.meanNumber().doubleValue(), 0.01)
			assertEquals(1.0, z1.stdNumber().doubleValue(), 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testSum_119(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum_119(ByVal backend As Nd4jBackend)
			Dim z2 As INDArray = Nd4j.zeros(DataType.DOUBLE, 55000000)
			Dim sum As Double = z2.sumNumber().doubleValue()
			assertEquals(0.0, sum, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testLegacyDistribution1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLegacyDistribution1(ByVal backend As Nd4jBackend)
			Dim distribution As New NormalDistribution(New DefaultRandom(), 0.0, 1.0)
			Dim z1 As INDArray = distribution.sample(New Integer() {1, 1000000})

			assertEquals(0.0, z1.meanNumber().doubleValue(), 0.01)
			assertEquals(1.0, z1.stdNumber().doubleValue(), 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSetSeed1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSetSeed1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z01 As INDArray = Nd4j.create(1000)
			Dim z11 As INDArray = Nd4j.create(1000)

			Dim distribution01 As New UniformDistribution(z01, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution01, random1)

			Dim distribution11 As New UniformDistribution(z11, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution11, random2)

			random1.setSeed(1999)
			random2.setSeed(1999)

			Dim z02 As INDArray = Nd4j.create(100)
			Dim distribution02 As New UniformDistribution(z02, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution02, random1)

			Dim z12 As INDArray = Nd4j.create(100)
			Dim distribution12 As New UniformDistribution(z12, 1.0, 2.0)
			Nd4j.Executioner.exec(distribution12, random2)


			For x As Integer = 0 To z01.length() - 1
				assertEquals(z11.getFloat(x), z01.getFloat(x),0.01f,"Failed on element: [" & x & "]")
			Next x

			assertEquals(z01, z11)

			For x As Integer = 0 To z02.length() - 1
				assertEquals(z02.getFloat(x), z12.getFloat(x), 0.01f,"Failed on element: [" & x & "]")
			Next x

			assertEquals(z02, z12)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaSide1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaSide1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim array1(999) As Single
			Dim array2(999) As Single

			For e As Integer = 0 To array1.Length - 1
				array1(e) = random1.nextFloat()
				array2(e) = random2.nextFloat()

				assertTrue(array1(e) <= 1.0f)
			Next e

			assertArrayEquals(array1, array2, 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaSide2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaSide2(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim array1(999) As Integer
			Dim array2(999) As Integer

			For e As Integer = 0 To array1.Length - 1
				array1(e) = random1.nextInt()
				array2(e) = random2.nextInt()

				assertEquals(array1(e), array2(e))
				assertTrue(array1(e) >= 0)
			Next e

			assertArrayEquals(array1, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaSide3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaSide3(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim array1(9999) As Integer
			Dim array2(9999) As Integer

			For e As Integer = 0 To array1.Length - 1
				array1(e) = random1.nextInt(9823)
				array2(e) = random2.nextInt(9823)

				assertTrue(array1(e) >= 0)
				assertTrue(array1(e) < 9823)
			Next e

			assertArrayEquals(array1, array2)
		End Sub

		''' <summary>
		''' This test checks reSeed mechanics for native side
		''' </summary>
		''' <exception cref="Exception"> </exception>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaSide4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaSide4(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim array1(999) As Integer
			Dim array2(999) As Integer

			For e As Integer = 0 To array1.Length - 1
				array1(e) = random1.nextInt()
				array2(e) = random2.nextInt()

				assertEquals(array1(e), array2(e))
				assertTrue(array1(e) >= 0)
			Next e

			assertArrayEquals(array1, array2)

			random1.reSeed()
			random1.reSeed()

			Dim array3(999) As Integer
			Dim array4(999) As Integer

			For e As Integer = 0 To array1.Length - 1
				array3(e) = random1.nextInt()
				array4(e) = random2.nextInt()

				assertNotEquals(array3(e), array4(e))
				assertTrue(array1(e) >= 0)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaSide5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaSide5(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(7)
			Dim length As Integer = 100

			Dim jarray_A(length - 1) As Integer
			Dim jarray_B(length - 1) As Integer

			For e As Integer = 0 To length - 1
				jarray_A(e) = Nd4j.Random.nextInt(0, 1000)
			Next e

			Nd4j.Random.setSeed(7)
			For e As Integer = 0 To length - 1
				jarray_B(e) = Nd4j.Random.nextInt(0, 1000)
			Next e

			assertArrayEquals(jarray_A, jarray_B)

			Dim sum As Integer = 0
			For e As Integer = 0 To length - 1
				sum += jarray_A(e)
			Next e

			assertNotEquals(0, sum)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBernoulliDistribution1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBernoulliDistribution1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.zeros(1000)
			Dim z2 As INDArray = Nd4j.zeros(1000)
			Dim z1Dup As INDArray = Nd4j.zeros(1000)

			Dim op1 As New BernoulliDistribution(z1, 0.25)
			Dim op2 As New BernoulliDistribution(z2, 0.25)

			Nd4j.Executioner.exec(op1, random1)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(z1Dup, z1)

			assertEquals(z1, z2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBernoulliDistribution2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBernoulliDistribution2(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.zeros(20)
			Dim z2 As INDArray = Nd4j.zeros(20)
			Dim z1Dup As INDArray = Nd4j.zeros(20)
			Dim exp As INDArray = Nd4j.create(New Double(){ 0, 1.0000, 0, 1.0000, 1.0000, 0, 1.0000, 1.0000, 0, 1.0000, 1.0000, 1.0000, 0, 1.0000, 1.0000, 0, 0, 1.0000, 0, 1.0000 })

			Dim op1 As New BernoulliDistribution(z1, 0.50)
			Dim op2 As New BernoulliDistribution(z2, 0.50)

			Nd4j.Executioner.exec(op1, random1)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(z1Dup, z1)

			assertEquals(z1, z2)

			assertEquals(exp, z1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBernoulliDistribution3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBernoulliDistribution3(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim prob As INDArray = Nd4j.create(New Double() {1.0, 0.1, 0.2, 0.5, 1.0, 1.0, 0.3, 0.7, 0.34, 0.119})

			Dim z1 As INDArray = Nd4j.zeros(10)
			Dim z2 As INDArray = Nd4j.zeros(10)
			Dim z1Dup As INDArray = Nd4j.zeros(10)
			Dim exp As INDArray = Nd4j.create(New Double(){ 1.0000, 0, 0, 1.0000, 1.0000, 1.0000, 0, 1.0000, 0, 0 })

			Dim op1 As New BernoulliDistribution(z1, prob)
			Dim op2 As New BernoulliDistribution(z2, prob)

			Nd4j.Executioner.exec(op1, random1)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(z1Dup, z1)

			assertEquals(z1, z2)

			assertEquals(exp, z1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBinomialDistribution1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBinomialDistribution1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.zeros(1000)
			Dim z2 As INDArray = Nd4j.zeros(1000)
			Dim z1Dup As INDArray = Nd4j.zeros(1000)

			Dim op1 As New BinomialDistribution(z1, 5, 0.25)
			Dim op2 As New BinomialDistribution(z2, 5, 0.25)

			Nd4j.Executioner.exec(op1, random1)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(z1Dup, z1)

			assertEquals(z1, z2)

			BooleanIndexing.and(z1, Conditions.lessThanOrEqual(5.0))
			BooleanIndexing.and(z1, Conditions.greaterThanOrEqual(0.0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBinomialDistribution2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBinomialDistribution2(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z1 As INDArray = Nd4j.create(DataType.FLOAT, 1000)
			Dim z2 As INDArray = Nd4j.zeros(DataType.FLOAT,1000)
			Dim z1Dup As INDArray = Nd4j.zeros(DataType.FLOAT,1000)

			Dim probs As INDArray = Nd4j.create(New Single() {0.25f, 0.43f, 0.55f, 0.43f, 0.25f})

			Dim op1 As New BinomialDistribution(z1, 5, probs)
			Dim op2 As New BinomialDistribution(z2, 5, probs)

			Nd4j.Executioner.exec(op1, random1)
			Nd4j.Executioner.exec(op2, random2)

			assertNotEquals(z1Dup, z1)

			assertEquals(z1, z2)

			BooleanIndexing.and(z1, Conditions.lessThanOrEqual(5.0))
			BooleanIndexing.and(z1, Conditions.greaterThanOrEqual(0.0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultithreading1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultithreading1(ByVal backend As Nd4jBackend)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger cnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim cnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.CopyOnWriteArrayList<float[]> list = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim list As New CopyOnWriteArrayList(Of Single())()

			Dim threads(9) As Thread
			For x As Integer = 0 To threads.Length - 1
				list.add(Nothing)
			Next x

			For x As Integer = 0 To threads.Length - 1
				threads(x) = New Thread(Sub()
				Dim rnd As Random = Nd4j.Random
				rnd.setSeed(119)
				Dim array(9) As Single
				For e As Integer = 0 To array.Length - 1
					array(e) = rnd.nextFloat()
				Next e
				list.set(cnt.getAndIncrement(), array)
				End Sub)
				threads(x).Start()
			Next x

			' we want all threads finished before comparing arrays
			For x As Integer = 0 To threads.Length - 1
				threads(x).Join()
			Next x

			For x As Integer = 0 To threads.Length - 1
				assertNotEquals(Nothing, list.get(x))

				If x > 0 Then
					assertArrayEquals(list.get(0), list.get(x), 1e-5f)
				End If
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultithreading2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultithreading2()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger cnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim cnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.CopyOnWriteArrayList<org.nd4j.linalg.api.ndarray.INDArray> list = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim list As New CopyOnWriteArrayList(Of INDArray)()

			Dim threads(9) As Thread
			For x As Integer = 0 To threads.Length - 1
				list.add(Nothing)
			Next x

			For x As Integer = 0 To threads.Length - 1
				threads(x) = New Thread(Sub()
				Dim rnd As Random = Nd4j.Random
				rnd.setSeed(119)
				Dim array As INDArray = Nd4j.Executioner.exec(New UniformDistribution(Nd4j.createUninitialized(25)))
				Nd4j.Executioner.commit()
				list.set(cnt.getAndIncrement(), array)
				End Sub)
				threads(x).Start()
			Next x

			' we want all threads finished before comparing arrays
			For x As Integer = 0 To threads.Length - 1
				threads(x).Join()
			Next x

			For x As Integer = 0 To threads.Length - 1
				assertNotEquals(Nothing, list.get(x))

				If x > 0 Then
					assertEquals(list.get(0), list.get(x))
				End If
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStepOver3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStepOver3(ByVal backend As Nd4jBackend)
			Dim random As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
			If TypeOf random Is NativeRandom Then
				Dim rng As NativeRandom = DirectCast(random, NativeRandom)

				Dim someInt As Integer = rng.nextInt()
				For e As Integer = 0 To 9999
					rng.nextInt()
				Next e

				random.setSeed(119)

				Dim sameInt As Integer = rng.nextInt()

				assertEquals(someInt, sameInt)

				random.setSeed(120)

				Dim otherInt As Integer = rng.nextInt()

				assertNotEquals(someInt, otherInt)

			Else
				log.warn("Not a NativeRandom object received, skipping test")
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStepOver4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStepOver4(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119, 100000)
			Dim random2 As Random = Nd4j.RandomFactory.getNewRandomInstance(119, 100000)

			For x As Integer = 0 To 999
				Dim z1 As INDArray = Nd4j.rand(1, 10000, random1)
				Dim z2 As INDArray = Nd4j.rand(1, 10000, random2)

				assertEquals(z1, z2)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSignatures1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSignatures1(ByVal backend As Nd4jBackend)

			For x As Integer = 0 To 99
				Dim z1 As INDArray = Nd4j.randn(5325235, New Long(){128, 1})
				Dim z2 As INDArray = Nd4j.randn(5325235, New Long(){128, 1})

				assertEquals(z1, z2)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testChoice1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testChoice1(ByVal backend As Nd4jBackend)
			Dim source As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5})
			Dim probs As INDArray = Nd4j.create(New Double() {0.0, 0.0, 1.0, 0.0, 0.0})
			Dim exp As INDArray = Nd4j.create(5).assign(3.0)

			Dim sampled As INDArray = Nd4j.choice(source, probs, 5)
			assertEquals(exp, sampled)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testChoice2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testChoice2(ByVal backend As Nd4jBackend)
			Dim source As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5})
			Dim probs As INDArray = Nd4j.create(New Double() {0.0, 0.0, 0.0, 0.0, 0.0})
			Dim exp As INDArray = Nd4j.create(5).assign(5.0)

			Dim sampled As INDArray = Nd4j.choice(source, probs, 5)
			assertEquals(exp, sampled)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testDeallocation1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeallocation1()
			For i As Integer = 0 To 999
				Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)
				random1.nextInt()

				System.GC.Collect()
				Thread.Sleep(50)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void someTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub someTest(ByVal backend As Nd4jBackend)
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Dim x As INDArray = Nd4j.create(New Double() {-0.5753774207320429, 1.0614372269091394, 0.4522970978070401, -0.5752887679689271, 1.0636465735137173, 0.4544011796073467, -0.576361407698785, 1.0656790105069853, 0.4552935317796974, -0.5760602684016433, 1.0658617022858135, 0.4557330858969331, -0.5757970093448411, 1.0622487939115577, 0.45266130626880363, -0.5752622961957029, 1.0582596824316828, 0.44949025343112814, -0.5771479956928688, 1.0665372965638613, 0.4553688166885955, -0.5753088931923759, 1.0620227840548335, 0.45289545873086556, -0.576588580700202, 1.0682150986638697, 0.457411469249719, -0.5747325473572189, 1.0626318659592515, 0.4539743754957771, -0.5745380761623263, 1.0581714324564084, 0.4500640145455051, -0.5756600950978087, 1.0634216668548728, 0.4538595118971328, -0.5751140573519833, 1.0640115397234116, 0.45489343676357286, -0.5772284666676437, 1.0696940198418068, 0.4581879096117204, -0.5744147982066905, 1.0554839926243997, 0.4477135176681925, -0.5754198385793243, 1.0558429782980523, 0.44713394665660644, -0.5761545677071064, 1.0598241808807554, 0.45011696447560207, -0.5758387163599189, 1.0619667903192647, 0.4523652688352249, -0.5737984521578438, 1.0551267152966937, 0.4479433219105848, -0.5759974232799963, 1.061302689492133, 0.4516134441303072, -0.5736901589111626, 1.0576251048845364, 0.4503299444045488, -0.5763311372167914, 1.06192192215954, 0.45187907799834365, -0.5778442414543, 1.0674079152998242, 0.45553705763054314, -0.5758254570690142, 1.0620200161144016, 0.4524260129848761, -0.5749775837304827, 1.062224210147449, 0.45337944519367585, -0.574541903754345, 1.0619442384090578, 0.45351676811211955, -0.5760078457119082, 1.062690890233097, 0.4528757342573996, -0.5748606750551666, 1.060141033285612, 0.4515767478829046, -0.5749561834487571, 1.0606232394644224, 0.45193216220783466, -0.5756803380730748, 1.064483756604441, 0.4548141798773699, -0.5752565746574122, 1.0636651281176792, 0.4544472759986484, -0.5750760910978936, 1.0594989813795266, 0.45079386382003334, -0.5751674161305798, 1.0590858567198587, 0.45033285969135406, -0.5750886065307328, 1.0572011798927974, 0.4486775685374512, -0.5747325473572189, 1.0626318659592515, 0.4539743754957771, -0.5757243201088236, 1.0633839362120128, 0.45376689590426994, -0.5744411030524335, 1.0582391680513001, 0.45021371788814785, -0.5747325473572189, 1.0626318659592515, 0.4539743754957771, -0.5769510974701872, 1.0685324074495908, 0.4573744807836674, -0.5750191442942153, 1.0611238707219008, 0.45233387445404916, -0.5763530480319555, 1.0632592080003551, 0.4530843416356724, -0.5761681009423941, 1.0687223794712288, 0.4582562437719459, -0.5772202009540097, 1.0683672322728441, 0.4569799298001917, -0.5770651807597004, 1.0636720905704742, 0.4528188972040562, -0.5755594444325524, 1.0602552587289935, 0.4510497867771471, -0.5760405012467995, 1.0650797166475576, 0.4550345871790212, -0.5753138307789047, 1.0603836033532072, 0.451389365910235, -0.5764219486333497, 1.066178407227334, 0.4556963003853961, -0.5748294633718319, 1.059070222875785, 0.450624005391455, -0.5754032559272689, 1.062504307475741, 0.453251283125361, 0.357808280229093, -0.17304804748832744, 0.1648877578656923, 0.3550956268779401, -0.16638470955750134, 0.16854004156835015, 0.35761790317790293, -0.17225833018768533, 0.1654391291304103, 0.3536090968875379, -0.1570909141136799, 0.17571031393597503, 0.3561854268639926, -0.167380791258639, 0.16861259032124698, 0.3546448721372181, -0.161229935301283, 0.17285482935309807, 0.354628589295547, -0.16574588493263773, 0.1687031152037963, 0.3515608583761638, -0.15075008903410433, 0.17966769737990534, 0.35735084527857575, -0.1696182518386006, 0.1676162794872508, 0.35146079433904887, -0.15372713783620343, 0.17685002025939964, 0.3528734834345405, -0.1521597664861848, 0.17956276341866134, 0.3532410649497478, -0.160680048791368, 0.1720897037995631, 0.356682698566458, -0.16328251379445335, 0.17281643565506308, 0.3556302932619103, -0.16500416366377244, 0.17028801230489224, 0.35211485765711686, -0.15678608646411626, 0.17463895406650265, 0.35637497011042096, -0.1691665602108546, 0.16714799681616294, 0.35308078531675746, -0.1592600519004829, 0.173245669482832, 0.3556196874799506, -0.16224708681088748, 0.17280414441250597, 0.3559475841193771, -0.16396311971736327, 0.17152848950991376, 0.35435929634532026, -0.15891041774418582, 0.17472158068918403, 0.3528490359864511, -0.16132798573712082, 0.1711417922247098, 0.35462901944485786, -0.16272899207088296, 0.17146723613971174, 0.3567480914698187, -0.16665684870871977, 0.16978436312547981, 0.35677871524326865, -0.16619978521411394, 0.17023075253187472, 0.35606103185316756, -0.16664741773206532, 0.16917198549729348, 0.3562273106630626, -0.16822741271934818, 0.1678748703769742, 0.35803810004503234, -0.17145759936952631, 0.16655247328612868, 0.3563871886834647, -0.16952991173201867, 0.1668261798007235, 0.35436973044992964, -0.1626885508561808, 0.17126991846165585, 0.354059661856123, -0.15883963375895938, 0.17451559223248628, 0.35397652790453105, -0.15754392604138207, 0.1756274285801798, 0.35422920502812466, -0.15772901356550117, 0.1756862615390695, 0.35416424088944914, -0.16022172948512917, 0.17334400078403028, 0.3555600143057507, -0.1643372584279808, 0.17083543107967486, 0.3525087034842565, -0.1575072041681293, 0.17433433660001676, 0.3531659556069536, -0.1624191446662591, 0.17042865350793346, 0.3565696507307317, -0.1697220040407826, 0.166815130002541, 0.3568664974596232, -0.16577658963578037, 0.1706977802149158, 0.35313668277505816, -0.15886990989683572, 0.17365359737044656, 0.3533245352115322, -0.15723031113817726, 0.17532540564635665, 0.35460862238876345, -0.1595238276259829, 0.17438500473685614, 0.35525250874776443, -0.16466741223783185, 0.17025503480284157, 0.3545409063719635, -0.16055812395314287, 0.17337629382343148, 0.35198952012701995, -0.15156979252918573, 0.17930423619280544, 0.3537953559292405, -0.15906206241879808, 0.17407292855724904, 0.35415180834842913, -0.1607628482146717, 0.1728370522185283, 0.3537998855935737, -0.1600845565243993, 0.1731403306802763, 0.3554810273775851, -0.16489175524215102, 0.17025607008052857, 0.3508232195628162, -0.15082599073411826, 0.17893143035496875, 0.35370792374178356, -0.15961008691395126, 0.17349186328292782, 0.05450698542491758, -0.41874678698827594, -0.3343403087067353, 0.05498792881564898, -0.41460440299356255, -0.33011081679631604, 0.059046779421456655, -0.42765937881362637, -0.3384015915928204, 0.057799646609788376, -0.4216980629472357, -0.3340677702649465, 0.05660348398009795, -0.42152485671613177, -0.3349902821139396, 0.062105535400888166, -0.4346085458257504, -0.34200288508621907, 0.05234240369292872, -0.4055153621656568, -0.32417570593377165, 0.062317826890744256, -0.43305655048852, -0.3403892391519301, 0.05999457207577438, -0.4256813340236285, -0.3357328454874602, 0.05678917347058686, -0.42675689269642103, -0.3396154345679126, 0.05573207104665189, -0.42026752129437106, -0.33462610511478547, 0.05714994401613468, -0.4205474351785073, -0.3336009477907372, 0.05726741118080793, -0.4235566120776033, -0.33625143560385046, 0.05425935432791021, -0.41257249501421506, -0.3289079566747622, 0.052303907040540346, -0.41140905979351317, -0.3296096337421601, 0.05435673726351468, -0.41816551767450155, -0.33394362207267464, 0.057990097876612606, -0.4230779753541936, -0.3351597436911609, 0.06092405835204879, -0.43557367866707497, -0.3439549390223794, 0.06258076523991336, -0.4348580034399873, -0.34180186041642535, 0.058062574332262445, -0.41817120503133454, -0.3305992122298355, 0.056667222489482326, -0.4238987396752845, -0.33710735037338646, 0.05331470690325209, -0.4115191773603726, -0.32879687237030414, 0.06346989358689988, -0.4364823047245409, -0.34248619701107236, 0.05644334203874793, -0.4187549773420934, -0.3325975840580061, 0.057027209491249405, -0.4237123787110853, -0.33661124389650665, 0.060880554331858905, -0.43118060018960624, -0.3399698253230048, 0.055782276874872014, -0.41756220328628996, -0.3321024223397344, 0.055446756449796616, -0.41723881862577716, -0.3321094434159544, 0.056669295076943016, -0.4204495146444979, -0.3339456915975489, 0.06175751688116003, -0.4316649644441389, -0.3396208783911239, 0.06173341588037583, -0.43230523620229205, -0.3402292064686406, 0.061875407606504736, -0.4375444226515741, -0.3449004067573839, 0.05614720683299703, -0.41976126084969595, -0.33378709562366216, 0.05830426102392656, -0.4215960573784757, -0.3335182151970676, 0.05970588243873762, -0.42240282576500415, -0.33299039110293827, 0.0601036415102731, -0.43192922004298595, -0.341357858629765, 0.053969290626186925, -0.4179579756815318, -0.3341036998452465, 0.057561584042216396, -0.4222868516155249, -0.3348223303093653, 0.05493041051018744, -0.4159784070400405, -0.3314215115884726, 0.05717139029405299, -0.4240689677351035, -0.3368075882948835, 0.05549340668146566, -0.42106549309448166, -0.3355728387667392, 0.05541833273044943, -0.4214816855580636, -0.3360219642916261, 0.05498792881564898, -0.41460440299356255, -0.33011081679631604, 0.05685500116978513, -0.42385292467441293, -0.336895651128018, 0.054911515245869534, -0.4208844109998573, -0.33593291020280946, 0.05523742901993513, -0.4201363809538045, -0.3349530647612928, 0.05645313933816329, -0.4183606591328087, -0.33222749927008216, 0.05624807825529575, -0.42052328095315095, -0.33439399594116775, 0.05375363814021924, -0.4170276113599561, -0.33344632974645483, 0.05534618656451573, -0.41631425766460334, -0.33135336920555164}, New Integer() {150, 3}, "c"c)
			Dim y As INDArray = Nd4j.create(New Double() {0.2429357202832011, 0.24691828776293456, 0.24583756032730986, 0.24705968192172242, 0.24232827188842557, 0.23861718711877997, 0.24465104629395537, 0.2442738260690249, 0.24817946695739254, 0.24674303971330414, 0.2406225888573061, 0.24498353214674504, 0.24727346570657688, 0.24750033019519094, 0.23490644039834577, 0.23069196589254812, 0.23707179721263205, 0.2426418870958991, 0.23908522418834394, 0.24018555003273495, 0.24389367527050315, 0.24082645976621256, 0.24213120625453619, 0.24456405220771912, 0.24572256827067002, 0.24714763826639866, 0.2440167016142005, 0.24298275912681128, 0.24351951667026728, 0.24651027782106275, 0.24692250660853385, 0.24274136220433426, 0.23744386161723494, 0.23442898757139466, 0.24674303971330414, 0.2449728421653453, 0.2415619815480595, 0.24674303971330414, 0.247534800371069, 0.24404181458275354, 0.24259378898938533, 0.24971978740576964, 0.2464087697036708, 0.24262430969703594, 0.24122273758752688, 0.2468932971669087, 0.24086424499542164, 0.24625774119619978, 0.24091022329137776, 0.2447601281813779, 0.24650971563804278, 0.24694511135813532, 0.24744726876889875, 0.2499998745489847, 0.2488199078775225, 0.2495902563502858, 0.24675189476120526, 0.24998568760015533, 0.24859167567513393, 0.24959567465133717, 0.24963361855789493, 0.24817253327065975, 0.249994140708285, 0.24911602549653417, 0.24836829906001545, 0.24699702031098014, 0.24893611893905115, 0.24965316728681577, 0.2499998535029291, 0.24988790472862127, 0.24775308931283793, 0.24873108439991887, 0.2498966240270357, 0.24955939151129433, 0.2484053958352789, 0.24768482160702535, 0.2488742705575876, 0.24806442277140775, 0.2488691246208544, 0.24947483938321652, 0.24996777647841242, 0.24996708063398507, 0.24933140745588728, 0.24980398107656687, 0.2491285118494222, 0.24625987230771443, 0.24738925580304405, 0.24997065883924532, 0.2486746004152257, 0.2498843535089784, 0.24995370354353233, 0.24864990162099362, 0.2496455019084883, 0.24999998132131074, 0.24963836089792163, 0.24882032311579313, 0.2490425997813645, 0.24864262256994435, 0.24962396682576615, 0.24923850229014768, 0.24758302979788205, 0.2497364221125665, 0.24833712214381948, 0.24945237443576807, 0.2487672378102696, 0.24872684754912008, 0.24999957029328843, 0.24932200515812822, 0.24997350874499308, 0.244153258352151, 0.2470095569300082, 0.2495218586932054, 0.24811062315859542, 0.249937451895014, 0.24908117922763642, 0.2469981453066533, 0.24887643643724272, 0.24388574974855176, 0.2497890295896134, 0.24985753528646346, 0.24695464048531734, 0.2494344204235447, 0.24945925714694822, 0.24932962141151696, 0.24711291849801428, 0.24792873650284003, 0.24901637934722654, 0.24853055677109265, 0.2493702155100004, 0.24875396650445886, 0.24922420532388534, 0.24308868874709608, 0.24927788200579612, 0.2495128512715364, 0.2499941533196743, 0.24750947275677102, 0.24642116577310766, 0.2486200299475723, 0.24850849338968475, 0.24729748244118202, 0.24744445799632175, 0.24628385050522522, 0.2497364221125665, 0.24749963780305764, 0.2463077044861883, 0.24740549843131626, 0.24976137204060936, 0.24818886413733424, 0.24637984563882515, 0.24900312437125263, 0.24725246251464808, 0.2487629429430977, 0.24865171031370353, 0.24898122102519538, 0.24717579119456606, 0.2460980895111744, 0.24863419281317758, 0.247722109456114, 0.249483330285878, 0.24833150510043103, 0.24597530718656643, 0.24808523436616853, 0.24868160690258487, 0.24928092765244053, 0.24371474005808394, 0.2433465092843804, 0.24605554602979549, 0.24756485929873812, 0.24528840047309738, 0.24672264761923993, 0.24694122129988064, 0.24734547447905952, 0.24790888678135556, 0.24858706085548732, 0.24810764262100565, 0.24863482616236307, 0.24827880810787284, 0.24705502393438042, 0.24732810186246684, 0.24867054596532764, 0.2487232960074739, 0.24756546852978628, 0.24464019605672277, 0.24381663076095264, 0.24833150510043103, 0.24818162318958523, 0.24637168326866488, 0.24833150510043103, 0.249349601679655, 0.24753319754810257, 0.24774444170583876, 0.24996939860971737, 0.24904696608261323, 0.2485561286364399, 0.24710085120475833, 0.24909198676990582, 0.24637328290411886, 0.2487969512643165, 0.2462161750985027, 0.24796208770766182, 0.2482717242774289, 0.24927296051372536, 0.24886483945735274, 0.24993221481965383, 0.24967300752206745, 0.24992406337114595, 0.24940315381994102, 0.24994552914860874, 0.24916401420686277, 0.24995963968339815, 0.24973233672772352, 0.2498543255582006, 0.24993325688497398, 0.24974525038828596, 0.24989546071480015, 0.2488904029901035, 0.24996473851724282, 0.24968939120009442, 0.24998514600584043, 0.249970842490889, 0.24994173055054456, 0.24971192503658038, 0.24996592891614833, 0.24962101269911038, 0.24936160593652545, 0.2491775995760193, 0.24927805303451178, 0.24956603508121747, 0.24987730415488038, 0.24981789385147107, 0.24999930237271728, 0.24998381258446437, 0.24986548776192077, 0.24999570010002634, 0.24999591182398645, 0.24954087006361633, 0.24910219683524124, 0.24994978142288657, 0.24984413117714582, 0.2499918867206099, 0.24999323357114836, 0.24964894224451797, 0.24992220590291733, 0.2499341504594762, 0.2499813141457409, 0.24969535176901464, 0.2498639405365019, 0.24954381392197553, 0.24998693285959547, 0.24991793303705503, 0.24997894264010345, 0.24987574787239242, 0.24976012810712447, 0.24995513300895975, 0.24999947057326638, 0.2493918776687613, 0.24924872331895642, 0.24934183397707277, 0.24998712583879595, 0.24955837700116507, 0.2498331267576389, 0.24999981660905926, 0.24990076371927936, 0.24953797044297324, 0.2494265251092823, 0.2499977970977972, 0.24982068183227324, 0.24798657706157284, 0.24991157204598433, 0.249932740378162, 0.24988056441711978, 0.24976599942821817, 0.24941195957117365, 0.24999632523994095, 0.24974702472240795, 0.24897521663897507, 0.24999250490090896, 0.24996305238911065, 0.2499888335767531, 0.24891106929086498, 0.24951603791242954, 0.24695890528687767, 0.24995848440320612, 0.24981122350168533, 0.2499494301459412, 0.24956512364416267, 0.2499975951279776, 0.2498004863662123, 0.24998230392115176, 0.24978402532449248, 0.2499982957455684, 0.2499233623408743, 0.24987574787239242, 0.24992258224573397, 0.2499877309539672, 0.24999575770242835, 0.2499556931106015, 0.24994451980664456, 0.2499917805496643, 0.2499960409062574, 0.2488906858411297, 0.24927221432053384, 0.24959263428325498, 0.2496480416964974, 0.2490800126913869, 0.24700417829410917, 0.24947297522916015, 0.24903554141985265, 0.24986212683419776, 0.2494710235394941, 0.24812395570517215, 0.24933303959179373, 0.24963609512126977, 0.2499981104347387, 0.2471864446636986, 0.24611234486702, 0.2473548098091017, 0.24854393204705955, 0.24653404287294756, 0.24846732083974551, 0.24799207032097062, 0.24806934707439765, 0.24977920077440272, 0.24747851578074453, 0.2491977800330258, 0.24899684819566759, 0.2482703871176769, 0.24861888752204364, 0.24868382859250693, 0.2494854381282655, 0.24934135158120313, 0.24721040655966006, 0.24893513851554547, 0.24790515403657404, 0.2494710235394941, 0.2491904936736734, 0.24801381586026497, 0.2494710235394941, 0.2498798518999066, 0.24883828402114708, 0.24882333106825177, 0.2496579340850042, 0.24987747585242445, 0.2473671119886922, 0.24776221560479675, 0.2491701169747418, 0.24876589576359523, 0.24967670262514718, 0.24837643713184637, 0.24908976954024437, 0.22109653049902084, 0.22548404062580826, 0.2200516152327322, 0.23608366774470158, 0.22444171845768218, 0.23365223802969015, 0.22447052628573314, 0.24346374266820078, 0.2263252080678485, 0.23714899298172334, 0.2427377880302641, 0.23030753471526916, 0.23596746213525796, 0.22891052329406103, 0.23607820782805589, 0.2245286616708374, 0.2319875048139398, 0.2370609309083055, 0.22732603437985477, 0.23769735050651258, 0.2249363649166962, 0.2316955228651909, 0.22550619621604487, 0.23145195006251598, 0.22866901371862267, 0.22540992944493907, 0.22272184895874236, 0.21868396394543543, 0.2288664303844395, 0.2387563329217531, 0.23851371439846614, 0.2396548175328528, 0.235265249196123, 0.22619919934323457, 0.2334753125319801, 0.22747857709673994, 0.22237888950209675, 0.2293405393777329, 0.23512174304783892, 0.23605961692837263, 0.2363884920872055, 0.22911784265366114, 0.23508631539486008, 0.24298537417416582, 0.23496693141570144, 0.2353505896619777, 0.23423355787376254, 0.23026861594819384, 0.24205955937189744, 0.23444214017181403, 0.20711228404659351, 0.22376613724792477, 0.20579434619205228, 0.2194026188338758, 0.2106294523516182, 0.19834879650946866, 0.23482430646847627, 0.20760503194119267, 0.2151216557564706, 0.197085900182565, 0.21569731990066776, 0.217758419139472, 0.21004598240061953, 0.22350675479473048, 0.21633307169842297, 0.21137335744670882, 0.2177596292210227, 0.19501204554661022, 0.19284473370296384, 0.22784579330881613, 0.20506242624967894, 0.22457929209637387, 0.19874826014581815, 0.22121053191398565, 0.2104514538390277, 0.2094343294766834, 0.22234971134381387, 0.22296768210934845, 0.21382831666860633, 0.21326665887229082, 0.20547154049147015, 0.1972613998245205, 0.21223025778509932, 0.22498680501540697, 0.22691830656416243, 0.19521869814219142, 0.20988589092182444, 0.21868921978115488, 0.2240953476388927, 0.20928467448290794, 0.20578906266338623, 0.20681378450968757, 0.22376613724792477, 0.20553847889431168, 0.20376157669628492, 0.20862506432378858, 0.2195097946800901, 0.21546464912779167, 0.2130691837611387, 0.22424979277256304}, New Integer() {150, 3}, "f"c)

			Dim expCUDA As INDArray = Nd4j.create(New Double() {-0.1397797281402293, 0.262442968158004, 0.11257253487714672, -0.14204931755613565, 0.26459585187861423, 0.11326958823058592, -0.14169128233548328, 0.26498290860797713, 0.11363791196902154, -0.14232126667905204, 0.2653795480791151, 0.11377287243047099, -0.13953189423305898, 0.2625621860805628, 0.11274888391033339, -0.13726747097370906, 0.26043568605313927, 0.1110259706999667, -0.14119986101271959, 0.26517763983630427, 0.11360221352588595, -0.14053290451163764, 0.2630865243565184, 0.11278706577163364, -0.14309744661189566, 0.26650186027631995, 0.11428980254509002, -0.14181125575709072, 0.2638849706413404, 0.11325345211563415, -0.13824683928327508, 0.2602840431545141, 0.11167166360958086, -0.14102724341299233, 0.2638192134517527, 0.11316217164895999, -0.14221044613799594, 0.2646000994613115, 0.11355782124995259, -0.1428642360983056, 0.26665431757043373, 0.11454611162697295, -0.13493373555886776, 0.25723700689792417, 0.11066871266027851, -0.13274473377543702, 0.25693570312125485, 0.11004518408130244, -0.1365899988385908, 0.260775617522195, 0.11133859613971274, -0.1397225928004509, 0.26290565902532126, 0.11243264263783195, -0.1371867315730828, 0.2588103442915592, 0.11043327812855466, -0.13834625792794394, 0.2618474094769191, 0.11221118251826752, -0.13991940132336245, 0.26117123507760176, 0.11167825524041167, -0.13879578742895515, 0.2626615816962663, 0.11209734783562991, -0.13991412321056712, 0.26461990802358687, 0.11378368217808009, -0.14082620714516011, 0.26400443437557636, 0.111965718194097, -0.14128496857231843, 0.2635459447146433, 0.11298115125486892, -0.1419966745979669, 0.26403632111095915, 0.11292424586380322, -0.14055553461452114, 0.26384362761416763, 0.11243563386028677, -0.13968123293840568, 0.2619131683521957, 0.11227050868947011, -0.14001305190002286, 0.2623219326079562, 0.11238822036193419, -0.141911120074517, 0.26470575692604925, 0.11346951493365338, -0.1420437953574474, 0.26455829651364116, 0.11331249801989905, -0.1395947537242465, 0.2622953617320538, 0.11144093434955048, -0.13656997236245197, 0.2590949716288484, 0.11210367280536893, -0.13481743979284383, 0.25776322971796567, 0.11122948174103235, -0.14181125575709072, 0.2638849706413404, 0.11325345211563415, -0.14103682300076956, 0.2639123513628277, 0.1130743968031554, -0.13876313113599886, 0.26072016513363033, 0.11165922212607637, -0.14181125575709072, 0.2638849706413404, 0.11325345211563415, -0.14281547473615197, 0.2664381301793583, 0.11428866752101949, -0.14032871539338249, 0.26266338471441153, 0.11255798512378257, -0.13981966971765328, 0.26341655887464027, 0.11273795514065381, -0.14388057567732068, 0.26714789047716925, 0.11440730710165808, -0.14223211956518317, 0.2660736178596304, 0.11418899137369003, -0.14001004113201757, 0.2643822169708257, 0.11201250285527188, -0.13883802483037633, 0.2619899769262556, 0.11175309451997711, -0.1422205386545011, 0.2653028226880685, 0.11338102131495005, -0.13857253148598467, 0.2612501894958287, 0.11229027994882086, -0.1419483670463606, 0.2652619372220056, 0.11377674967870428, -0.13848229437537088, 0.2607602194371945, 0.11192438494521152, -0.14083577467674055, 0.26346078628006814, 0.11290025765751623, 0.08820321741221084, -0.042962937132769455, 0.036456111185867196, 0.08768912912215979, -0.04147520913561469, 0.03800308958007328, 0.08849157340423255, -0.042869041687349965, 0.03640514758784335, 0.08840222986126425, -0.03926208009247603, 0.04148233537457793, 0.08862602509961467, -0.04179046555496778, 0.037843699525301824, 0.0885159045500426, -0.04029524056756361, 0.04038791773259154, 0.0875052763451695, -0.041337546434876894, 0.03786887705583882, 0.08788518291446613, -0.037679310772829086, 0.043742570040689446, 0.08883444543172667, -0.04226276451085631, 0.037935789330510686, 0.08772309407654977, -0.038425579983097494, 0.041939804213313996, 0.08808908456289374, -0.03799921404053968, 0.04358666800484748, 0.08766472994380456, -0.04014660522142602, 0.03963355543195826, 0.0891685847336339, -0.04080973046501342, 0.04077905573678634, 0.08859320520357397, -0.041209006169318566, 0.03898071800741838, 0.08745416827005757, -0.03918013131062083, 0.04122845129298612, 0.08802355573068858, -0.042103933343329215, 0.03752951602609446, 0.08789456036870592, -0.039809397229546725, 0.040190830583142705, 0.08878158132891725, -0.04051137632979936, 0.04096511133924192, 0.0889868438845658, -0.04098834442211815, 0.038992891303455214, 0.08855010208484065, -0.03972297100409324, 0.0415308568061289, 0.0874194387267, -0.04032259594136955, 0.03849601262835472, 0.08820726056619942, -0.04063536986928261, 0.03972819093163967, 0.08915014368639582, -0.04165853399771313, 0.038287425905390665, 0.08903747908029147, -0.041486958695521756, 0.03940023963411198, 0.08844748155900393, -0.041555467710842814, 0.03868439107248724, 0.08823209789313106, -0.04191850288429148, 0.03784066268725205, 0.089106470980532, -0.042740616548806856, 0.037094874798938124, 0.08840698224388845, -0.04230890789862867, 0.03648221028869615, 0.08819168460920213, -0.04065217650480662, 0.03919793487055319, 0.08832897727363223, -0.03968098276580225, 0.0416667028390964, 0.08848272560584433, -0.03938587160340448, 0.04188955034091001, 0.08854564025617767, -0.03942970016629068, 0.042104058952174755, 0.08830426865151225, -0.040033880587860324, 0.04078181954110782, 0.08882030708521758, -0.04108360797322201, 0.03864283772967878, 0.08781996871300206, -0.039376157124858285, 0.04070276372274433, 0.08697060313120034, -0.040530214675006664, 0.038768867596498016, 0.08821150053622706, -0.04227812405783864, 0.037096163362112966, 0.08920615348763587, -0.04143582234449487, 0.03914792098507049, 0.08781612348104591, -0.03969271460836636, 0.040829736500267014, 0.08829027306019399, -0.03930630213110146, 0.04138724809469049, 0.08863573847454138, -0.039879877499865955, 0.04122260831236561, 0.0883335013507428, -0.041109045287316716, 0.039008466274951054, 0.08850954251831919, -0.040127040514003495, 0.040758394091767146, 0.08799737345705212, -0.0378824673311011, 0.04356830692232184, 0.08832089274747237, -0.03976254339418301, 0.040901381865641434, 0.08812016738529858, -0.04014173593635116, 0.040677302155068665, 0.08811124331057293, -0.039999358112224784, 0.04055527566668088, 0.08838773492102094, -0.04114771748741527, 0.03920462961422201, 0.08757388372185691, -0.037704526819131993, 0.04331206318950709, 0.0881576331615599, -0.03988942301339941, 0.04067380373044536, 0.013495004596650092, -0.10467787904526984, -0.06924598498509513, 0.013732488601800671, -0.10359958526920321, -0.073867622338269, 0.014663507273385447, -0.10681226123870459, -0.06964113429219437, 0.014418259088360003, -0.10540559541359698, -0.07329534366412284, 0.014081092360166813, -0.10538099101250491, -0.07055881966477318, 0.015447314035613191, -0.10838784129437379, -0.06783586065961766, 0.013085578431350012, -0.10107418630601421, -0.07612433531982664, 0.01553720555749748, -0.10797911451459238, -0.07066651886657471, 0.014997053687435705, -0.10641485321579136, -0.07222340561309375, 0.013865261741969313, -0.10650075751537919, -0.06693341363771006, 0.013766354176025222, -0.10499674891965531, -0.07217795404205836, 0.014260160255118556, -0.10513678167003707, -0.07264441501434046, 0.01420865307474977, -0.10584712083654362, -0.07062826312502943, 0.013561444762186578, -0.10295250306644092, -0.07351315002254191, 0.013027918843870464, -0.10261633218277293, -0.07130546452883366, 0.013426013289009173, -0.10454045824088537, -0.0705867845954161, 0.014432368908178261, -0.10569362827120234, -0.07298426151600021, 0.014858509648913935, -0.10801642563076536, -0.06707535623461379, 0.01563198862025337, -0.10867604725646529, -0.06591468875118317, 0.014507371715046171, -0.10451467522071968, -0.07532563977777654, 0.013994233557191597, -0.10592405632576582, -0.06912805117416723, 0.013298523016463842, -0.10278349861729164, -0.0738409688404247, 0.015833152505383894, -0.1088639069394899, -0.06806853577990854, 0.01407299710172178, -0.10468720551145808, -0.07357408848277809, 0.0140921601711803, -0.1058209059211477, -0.07084032565658337, 0.015094038913090283, -0.1073532833427305, -0.07120135240882869, 0.013890700619125151, -0.10438742115148218, -0.07384287774382131, 0.013780213251619126, -0.10429428867892578, -0.07404967280508117, 0.014131634326137085, -0.10510768374389001, -0.07140704509303743, 0.015362427285654635, -0.10744618787519382, -0.07242981001774759, 0.01538546151471559, -0.10786708970599292, -0.06990741917330204, 0.015041211700757331, -0.10805549163241167, -0.06803553703700807, 0.013996256799870863, -0.10492288857316887, -0.07083972134954941, 0.014547662409359825, -0.10531942691720375, -0.07503719765162918, 0.014926121528476222, -0.10557934559199808, -0.07556161565121688, 0.014876220620969672, -0.10779446920555454, -0.06663943676230895, 0.013299175512052633, -0.1044884887849407, -0.07012365270229738, 0.014310962748405539, -0.10548746091961465, -0.07322203418066323, 0.013650673557163585, -0.10398724057331998, -0.07427001885442606, 0.014138340887381534, -0.10592565377607648, -0.07048866647966796, 0.013731535938664729, -0.10526565567088782, -0.0690572199450989, 0.013648640373434837, -0.10533812001977037, -0.0694939741135303, 0.013732488601800671, -0.10359958526920321, -0.073867622338269, 0.01407159219681424, -0.10593041742703585, -0.06924501967896152, 0.013525129270068457, -0.10521593889975128, -0.06845021944709595, 0.013666043658741503, -0.10503231289490245, -0.06987960468127483, 0.01409981353709936, -0.1045716285237493, -0.07292719015185552, 0.013960146652089741, -0.10510748952535, -0.0720500850059039, 0.01324381306751248, -0.10425347510224883, -0.07104713730722463, 0.013781373376598662, -0.10407691618897837, -0.07430592437883553}, New Integer() {150, 3}, "c"c)

			Dim res As INDArray = x.muli(y)

			assertEquals(expCUDA, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testTruncatedNormal1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTruncatedNormal1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z01 As INDArray = Nd4j.create(10000000).assign(-119119R)
			Dim z02 As INDArray = Nd4j.createUninitialized(z01.length())

			Dim distribution01 As New TruncatedNormalDistribution(z01, 0.0, 1.0)

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Nd4j.Executioner.exec(distribution01, random1)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			Nd4j.Executioner.exec(New GaussianDistribution(z02, 0.0, 1.0))
			Dim time3 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Truncated: {} ms; Gaussian: {} ms", time2 - time1, time3 - time2)

			For e As Integer = 0 To z01.length() - 1
				assertTrue(FastMath.abs(z01.getDouble(e)) <= 2.0,"Value: " & z01.getDouble(e) & " at " & e)
				assertNotEquals(-119119R, z01.getDouble(e), 1e-3)
			Next e

			assertEquals(0.0, z01.meanNumber().doubleValue(), 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogNormal1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogNormal1(ByVal backend As Nd4jBackend)
			Dim random1 As Random = Nd4j.RandomFactory.getNewRandomInstance(119)

			Dim z01 As INDArray = Nd4j.create(1000000)

			Dim rng As New JDKRandomGenerator()
			rng.setSeed(119)

			Dim dst As New org.apache.commons.math3.distribution.LogNormalDistribution(rng, 0.0, 1.0)
			Dim array() As Double = dst.sample(1000000)


			Dim mean As Double = 0.0
			For Each e As Double In array
				mean += e
			Next e
			mean /= array.Length

			Dim distribution01 As New LogNormalDistribution(z01, 0.0, 1.0)
			Nd4j.Executioner.exec(distribution01, random1)

			log.info("Java mean: {}; Native mean: {}", mean, z01.meanNumber().doubleValue())
			assertEquals(mean, z01.meanNumber().doubleValue(), 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspace2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinspace2(ByVal backend As Nd4jBackend)
			Dim res As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5})

			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOrthogonalDistribution1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOrthogonalDistribution1(ByVal backend As Nd4jBackend)
			Dim dist As New OrthogonalDistribution(1.0)
			Dim array As INDArray = dist.sample(New Integer() {6, 9})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOrthogonalDistribution2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOrthogonalDistribution2(ByVal backend As Nd4jBackend)
			Dim dist As New OrthogonalDistribution(1.0)
			Dim array As INDArray = dist.sample(New Integer() {9, 6})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOrthogonalDistribution3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOrthogonalDistribution3(ByVal backend As Nd4jBackend)
			Dim dist As New OrthogonalDistribution(1.0)
			Dim array As INDArray = dist.sample(New Integer() {9, 9})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reproducabilityTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub reproducabilityTest(ByVal backend As Nd4jBackend)
			Dim numBatches As Integer = 1

			For t As Integer = 0 To 9
				numBatches = t

				Dim initial As IList(Of INDArray) = getList(numBatches)

				For i As Integer = 0 To 9
					Dim list As IList(Of INDArray) = getList(numBatches)
					assertEquals(initial, list)
				Next i
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaInt_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaInt_1(ByVal backend As Nd4jBackend)
			For e As Integer = 0 To 99999
				Dim i As Integer = Nd4j.Random.nextInt(10, 20)

				assertTrue(i >= 10 AndAlso i < 20)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBernoulli(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBernoulli(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim arr As INDArray = Nd4j.create(DataType.DOUBLE, 100)
			Nd4j.exec(New BernoulliDistribution(arr, 0.5))
			Dim sum As Double = arr.sumNumber().doubleValue()
			assertTrue(sum > 0.0 AndAlso sum < 100.0,sum.ToString())
		End Sub

		Private Function getList(ByVal numBatches As Integer) As IList(Of INDArray)
			Nd4j.Random.setSeed(12345)
			Dim [out] As IList(Of INDArray) = New List(Of INDArray)()
			Dim channels As Integer = 3
			Dim imageHeight As Integer = 64
			Dim imageWidth As Integer = 64
			For i As Integer = 0 To numBatches - 1
				[out].Add(Nd4j.rand(New Integer(){32, channels, imageHeight, imageWidth}))
			Next i
			Return [out]
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRngRepeatabilityUniform(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRngRepeatabilityUniform(ByVal backend As Nd4jBackend)
			Dim nexp As INDArray = Nd4j.create(DataType.FLOAT, 10)
			Nd4j.Random.setSeed(12345)
			Dim out1 As INDArray = Nd4j.create(DataType.FLOAT, 10)
			Nd4j.exec(New DistributionUniform(Nd4j.createFromArray(10L), out1, 0.0, 1.0))

			Nd4j.Random.setSeed(12345)
			Dim out2 As INDArray = Nd4j.create(DataType.FLOAT, 10)
			Nd4j.exec(New DistributionUniform(Nd4j.createFromArray(10L), out2, 0.0, 1.0))

			assertEquals(out1, out2)
			assertNotEquals(nexp, out1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRngRepeatabilityBernoulli(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRngRepeatabilityBernoulli(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim out1 As INDArray = Nd4j.create(DataType.FLOAT, 10)
			Nd4j.exec(New RandomBernoulli(Nd4j.createFromArray(10L), out1, 0.5))

			Nd4j.Random.setSeed(12345)
			Dim out2 As INDArray = Nd4j.create(DataType.FLOAT, 10)
			Nd4j.exec(New RandomBernoulli(Nd4j.createFromArray(10L), out2, 0.5))

			assertEquals(out1, out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGamma(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGamma(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim shape As INDArray = Nd4j.createFromArray(New Integer() {1000, 1000})
			Dim alpha As INDArray = Nd4j.createFromArray(New Single(){2.0f})
			Dim beta As INDArray = Nd4j.createFromArray(New Single(){2.0f})
			Dim randomGamma As New RandomGamma(shape, alpha, beta)
			Dim res() As INDArray = Nd4j.exec(randomGamma)

			Dim randomGamma1 As New RandomGamma(shape, alpha, beta)
			Dim res1() As INDArray = Nd4j.exec(randomGamma1)

			Dim meanOp0 As New Mean(res(0))
			Dim meanOp1 As New Mean(res1(0))

			Dim mean0 As INDArray = Nd4j.exec(meanOp0)
			Dim mean1 As INDArray = Nd4j.exec(meanOp1)

			assertArrayEquals(mean0.toFloatVector(), mean1.toFloatVector(), 1e-2f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPoisson(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPoisson(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim shape As INDArray = Nd4j.createFromArray(New Integer() {1, 3})
			Dim alpha As INDArray = Nd4j.rand(1,3)
			Dim randomPoisson As New RandomPoisson(shape, alpha)
			Dim res() As INDArray = Nd4j.exec(randomPoisson)

			Dim randomPoisson1 As New RandomPoisson(shape, alpha)
			Dim res1() As INDArray = Nd4j.exec(randomPoisson1)
			assertEquals(res(0), res1(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShuffle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShuffle(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim alpha As INDArray = Nd4j.rand(1,3)
			Dim randomShuffle As New RandomShuffle(alpha)
			Dim res() As INDArray = Nd4j.exec(randomShuffle)

			Dim randomShuffle1 As New RandomShuffle(alpha)
			Dim res1() As INDArray = Nd4j.exec(randomShuffle1)
			assertEquals(res(0), res1(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandom(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandom(ByVal backend As Nd4jBackend)
			Dim r1 As New Random(119)
			Dim r2 As Random = Nd4j.Random
			r2.setSeed(119)
			Dim jmax As Single = 0.0f
			Dim nmax As Single = 0.0f
			For e As Integer = 0 To 99999999
				Dim f As Single = r1.nextFloat()
				Dim n As Single = r2.nextFloat()
				If f > jmax Then
					jmax = f
				End If

				If n > nmax Then
					nmax = n
				End If
			Next e

			assertTrue(jmax < 1.0)
			assertTrue(nmax < 1.0)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace