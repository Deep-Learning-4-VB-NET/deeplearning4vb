Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.nn.conf.distribution
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports org.junit.jupiter.api
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports RandomFactory = org.nd4j.linalg.factory.RandomFactory
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports org.junit.jupiter.api.Assertions
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
'ORIGINAL LINE: @DisplayName("Legacy Weight Init Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class LegacyWeightInitTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LegacyWeightInitTest
		Inherits BaseDL4JTest

		Private prevFactory As RandomFactory

		Private Const SEED As Integer = 666

		Private Shared ReadOnly distributions As IList(Of Distribution) = New List(Of Distribution) From {
			New LogNormalDistribution(12.3, 4.56),
			New BinomialDistribution(3, 0.3),
			New NormalDistribution(0.666, 0.333),
			New UniformDistribution(-1.23, 4.56),
			New OrthogonalDistribution(3.45),
			New TruncatedNormalDistribution(0.456, 0.123),
			New ConstantDistribution(666)
		}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setRandomFactory()
		Friend Overridable Sub setRandomFactory()
			prevFactory = Nd4j.randomFactory_Conflict
			Nd4j.randomFactory_Conflict = New FixedSeedRandomFactory(prevFactory)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach void resetRandomFactory()
		Friend Overridable Sub resetRandomFactory()
			Nd4j.randomFactory_Conflict = prevFactory
		End Sub

		''' <summary>
		''' Test that param init is identical to legacy implementation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Init Params") void initParams()
		Friend Overridable Sub initParams()
			' To make identity happy
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] shape = { 5, 5 };
			Dim shape() As Long = { 5, 5 }
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanIn = shape[0];
			Dim fanIn As Long = shape(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanOut = shape[1];
			Dim fanOut As Long = shape(1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inLegacy = org.nd4j.linalg.factory.Nd4j.create(org.nd4j.linalg.api.buffer.DataType.@DOUBLE,fanIn * fanOut);
			Dim inLegacy As INDArray = Nd4j.create(DataType.DOUBLE,fanIn * fanOut)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inTest = inLegacy.dup();
			Dim inTest As INDArray = inLegacy.dup()
			For Each legacyWi As WeightInit In WeightInit.values()
				If legacyWi <> WeightInit.DISTRIBUTION Then
					Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = WeightInitUtil.initWeights(fanIn, fanOut, shape, legacyWi, null, inLegacy).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
					Dim expected As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, legacyWi, Nothing, inLegacy).castTo(DataType.DOUBLE)
					Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray actual = legacyWi.getWeightInitFunction().init(fanIn, fanOut, shape, WeightInitUtil.DEFAULT_WEIGHT_INIT_ORDER, inTest).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
					Dim actual As INDArray = legacyWi.getWeightInitFunction().init(fanIn, fanOut, shape, WeightInitUtil.DEFAULT_WEIGHT_INIT_ORDER, inTest).castTo(DataType.DOUBLE)
					assertArrayEquals(shape, actual.shape(),"Incorrect shape for " & legacyWi & "!")
					assertEquals(expected, actual,"Incorrect weight initialization for " & legacyWi & "!")
				End If
			Next legacyWi
		End Sub

		''' <summary>
		''' Test that param init is identical to legacy implementation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Init Params From Distribution") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) @Disabled(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) void initParamsFromDistribution()
		Friend Overridable Sub initParamsFromDistribution()
			' To make identity happy
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] shape = { 3, 7 };
			Dim shape() As Long = { 3, 7 }
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanIn = shape[0];
			Dim fanIn As Long = shape(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanOut = shape[1];
			Dim fanOut As Long = shape(1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inLegacy = org.nd4j.linalg.factory.Nd4j.create(org.nd4j.linalg.api.buffer.DataType.@DOUBLE,fanIn * fanOut);
			Dim inLegacy As INDArray = Nd4j.create(DataType.DOUBLE,fanIn * fanOut)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inTest = inLegacy.dup();
			Dim inTest As INDArray = inLegacy.dup()
			For Each dist As Distribution In distributions
				Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.DISTRIBUTION, Distributions.createDistribution(dist), inLegacy).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
				Dim expected As INDArray = WeightInitUtil.initWeights(fanIn, fanOut, shape, WeightInit.DISTRIBUTION, Distributions.createDistribution(dist), inLegacy).castTo(DataType.DOUBLE)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray actual = new WeightInitDistribution(dist).init(fanIn, fanOut, shape, WeightInitUtil.DEFAULT_WEIGHT_INIT_ORDER, inTest).castTo(org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
				Dim actual As INDArray = (New WeightInitDistribution(dist)).init(fanIn, fanOut, shape, WeightInitUtil.DEFAULT_WEIGHT_INIT_ORDER, inTest).castTo(DataType.DOUBLE)
				assertArrayEquals(shape, actual.shape(),"Incorrect shape for " & dist.GetType().Name & "!")
				assertEquals(expected, actual,"Incorrect weight initialization for " & dist.GetType().Name & "!")
			Next dist
		End Sub

		''' <summary>
		''' Test that weight inits can be serialized and de-serialized in JSON format
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Serialize Deserialize Json") void serializeDeserializeJson() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub serializeDeserializeJson()
			' To make identity happy
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] shape = { 5, 5 };
			Dim shape() As Long = { 5, 5 }
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanIn = shape[0];
			Dim fanIn As Long = shape(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanOut = shape[1];
			Dim fanOut As Long = shape(1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.shade.jackson.databind.ObjectMapper mapper = org.deeplearning4j.nn.conf.serde.JsonMappers.getMapper();
			Dim mapper As ObjectMapper = JsonMappers.Mapper
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inBefore = org.nd4j.linalg.factory.Nd4j.create(fanIn * fanOut);
			Dim inBefore As INDArray = Nd4j.create(fanIn * fanOut)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inAfter = inBefore.dup();
			Dim inAfter As INDArray = inBefore.dup()
			' Just use to enum to loop over all strategies
			For Each legacyWi As WeightInit In WeightInit.values()
				If legacyWi <> WeightInit.DISTRIBUTION Then
					Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final IWeightInit before = legacyWi.getWeightInitFunction();
					Dim before As IWeightInit = legacyWi.getWeightInitFunction()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = before.init(fanIn, fanOut, shape, inBefore.ordering(), inBefore);
					Dim expected As INDArray = before.init(fanIn, fanOut, shape, inBefore.ordering(), inBefore)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String json = mapper.writeValueAsString(before);
					Dim json As String = mapper.writeValueAsString(before)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final IWeightInit after = mapper.readValue(json, IWeightInit.class);
					Dim after As IWeightInit = mapper.readValue(json, GetType(IWeightInit))
					Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray actual = after.init(fanIn, fanOut, shape, inAfter.ordering(), inAfter);
					Dim actual As INDArray = after.init(fanIn, fanOut, shape, inAfter.ordering(), inAfter)
					assertArrayEquals(shape, actual.shape(),"Incorrect shape for " & legacyWi & "!")
					assertEquals(expected, actual,"Incorrect weight initialization for " & legacyWi & "!")
				End If
			Next legacyWi
		End Sub

		''' <summary>
		''' Test that distribution can be serialized and de-serialized in JSON format
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Serialize Deserialize Distribution Json") @Disabled("") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) void serializeDeserializeDistributionJson() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub serializeDeserializeDistributionJson()
			' To make identity happy
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] shape = { 3, 7 };
			Dim shape() As Long = { 3, 7 }
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanIn = shape[0];
			Dim fanIn As Long = shape(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long fanOut = shape[1];
			Dim fanOut As Long = shape(1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.shade.jackson.databind.ObjectMapper mapper = org.deeplearning4j.nn.conf.serde.JsonMappers.getMapper();
			Dim mapper As ObjectMapper = JsonMappers.Mapper
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inBefore = org.nd4j.linalg.factory.Nd4j.create(fanIn * fanOut);
			Dim inBefore As INDArray = Nd4j.create(fanIn * fanOut)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inAfter = inBefore.dup();
			Dim inAfter As INDArray = inBefore.dup()
			For Each dist As Distribution In distributions
				Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final IWeightInit before = new WeightInitDistribution(dist);
				Dim before As IWeightInit = New WeightInitDistribution(dist)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expected = before.init(fanIn, fanOut, shape, inBefore.ordering(), inBefore);
				Dim expected As INDArray = before.init(fanIn, fanOut, shape, inBefore.ordering(), inBefore)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String json = mapper.writeValueAsString(before);
				Dim json As String = mapper.writeValueAsString(before)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final IWeightInit after = mapper.readValue(json, IWeightInit.class);
				Dim after As IWeightInit = mapper.readValue(json, GetType(IWeightInit))
				Nd4j.Random.setSeed(SEED)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray actual = after.init(fanIn, fanOut, shape, inAfter.ordering(), inAfter);
				Dim actual As INDArray = after.init(fanIn, fanOut, shape, inAfter.ordering(), inAfter)
				assertArrayEquals(shape, actual.shape(),"Incorrect shape for " & dist.GetType().Name & "!")
				assertEquals(expected, actual,"Incorrect weight initialization for " & dist.GetType().Name & "!")
			Next dist
		End Sub

		''' <summary>
		''' Test equals and hashcode implementation. Redundant as one can trust Lombok on this??
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Equals And Hash Code") void equalsAndHashCode()
		Friend Overridable Sub equalsAndHashCode()
			Dim lastInit As WeightInit = WeightInit.values()(WeightInit.values().length - 1)
			For Each legacyWi As WeightInit In WeightInit.values()
				If legacyWi <> WeightInit.DISTRIBUTION Then
					assertEquals(legacyWi.getWeightInitFunction(), legacyWi.getWeightInitFunction(), "Shall be equal!")
					assertNotEquals(lastInit.getWeightInitFunction(), legacyWi.getWeightInitFunction(), "Shall not be equal!")
					If legacyWi <> WeightInit.NORMAL AndAlso legacyWi <> WeightInit.LECUN_NORMAL Then
						lastInit = legacyWi
					End If
				End If
			Next legacyWi
			Dim lastDist As Distribution = distributions(distributions.Count - 1)
			For Each distribution As Distribution In distributions
				assertEquals(New WeightInitDistribution(distribution), New WeightInitDistribution(distribution.clone()), "Shall be equal!")
				assertNotEquals(New WeightInitDistribution(lastDist), New WeightInitDistribution(distribution), "Shall not be equal!")
				lastDist = distribution
			Next distribution
		End Sub

		''' <summary>
		''' Assumes RandomFactory will only call no-args constructor while this test runs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Fixed Seed Random Factory") private static class FixedSeedRandomFactory extends org.nd4j.linalg.factory.RandomFactory
		Private Class FixedSeedRandomFactory
			Inherits RandomFactory

			Friend ReadOnly factory As RandomFactory

			Friend Sub New(ByVal factory As RandomFactory)
				MyBase.New(factory.Random.GetType())
				Me.factory = factory
			End Sub

			Public Overrides ReadOnly Property Random As Random
				Get
					Return getNewRandomInstance(SEED)
				End Get
			End Property

			Public Overrides ReadOnly Property NewRandomInstance As Random
				Get
					Return factory.NewRandomInstance
				End Get
			End Property

			Public Overrides Function getNewRandomInstance(ByVal seed As Long) As Random
				Return factory.getNewRandomInstance(seed)
			End Function

			Public Overrides Function getNewRandomInstance(ByVal seed As Long, ByVal size As Long) As Random
				Return factory.getNewRandomInstance(seed, size)
			End Function
		End Class
	End Class

End Namespace