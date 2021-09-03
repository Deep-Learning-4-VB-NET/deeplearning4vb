Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.parameterserver.distributed.messages.aggregations


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class VoidAggregationTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class VoidAggregationTest
		Inherits BaseND4JTest

		Private Const NODES As Short = 100
		Private Const ELEMENTS_PER_NODE As Integer = 3

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()

		End Sub



		''' <summary>
		''' In this test we check for aggregation of sample vector.
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void getAccumulatedResult1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub getAccumulatedResult1()
			Dim exp As INDArray = Nd4j.linspace(0, (NODES * ELEMENTS_PER_NODE) - 1, NODES * ELEMENTS_PER_NODE)

			Dim aggregations As IList(Of VectorAggregation) = New List(Of VectorAggregation)()
			Dim i As Integer = 0
			Dim j As Integer = 0
			Do While i < NODES

				Dim array As INDArray = Nd4j.create(ELEMENTS_PER_NODE)

				Dim e As Integer = 0
				Do While e < ELEMENTS_PER_NODE
					array.putScalar(e, CDbl(j))
					j += 1
					e += 1
				Loop
				Dim aggregation As New VectorAggregation(1L, NODES, CShort(i), array)

				aggregations.Add(aggregation)
				i += 1
			Loop


			Dim aggregation As VectorAggregation = aggregations(0)
			For Each vectorAggregation As VectorAggregation In aggregations
				aggregation.accumulateAggregation(vectorAggregation)
			Next vectorAggregation

			Dim payload As INDArray = aggregation.AccumulatedResult
			log.info("Payload shape: {}", payload.shape())
			assertEquals(exp.length(), payload.length())
			assertEquals(exp, payload)
		End Sub


		''' <summary>
		''' This test checks for aggregation of single-array dot
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void getScalarDotAggregation1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub getScalarDotAggregation1()
			Dim x As INDArray = Nd4j.linspace(0, (NODES * ELEMENTS_PER_NODE) - 1, NODES * ELEMENTS_PER_NODE)
			Dim y As INDArray = x.dup()
			Dim exp As Double = Nd4j.BlasWrapper.dot(x, y)

			Dim aggregations As IList(Of DotAggregation) = New List(Of DotAggregation)()
			Dim i As Integer = 0
			Dim j As Integer = 0
			Do While i < NODES
				Dim arrayX As INDArray = Nd4j.create(ELEMENTS_PER_NODE)
				Dim arrayY As INDArray = Nd4j.create(ELEMENTS_PER_NODE)

				Dim e As Integer = 0
				Do While e < ELEMENTS_PER_NODE
					arrayX.putScalar(e, CDbl(j))
					arrayY.putScalar(e, CDbl(j))
					j += 1
					e += 1
				Loop

				Dim dot As Double = Nd4j.BlasWrapper.dot(arrayX, arrayY)

				Dim aggregation As New DotAggregation(1L, NODES, CShort(i), Nd4j.scalar(dot))

				aggregations.Add(aggregation)
				i += 1
			Loop

			Dim aggregation As DotAggregation = aggregations(0)


			For Each vectorAggregation As DotAggregation In aggregations
				aggregation.accumulateAggregation(vectorAggregation)
			Next vectorAggregation

			Dim result As INDArray = aggregation.AccumulatedResult
			assertEquals(True, result.Scalar)
			assertEquals(exp, result.getDouble(0), 1e-5)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void getBatchedDotAggregation1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub getBatchedDotAggregation1()
			Dim x As INDArray = Nd4j.create(5, 300).assign(2.0)
			Dim y As INDArray = x.dup()

			x.muli(y)
			Dim exp As INDArray = x.sum(1)

			Dim aggregations As IList(Of DotAggregation) = New List(Of DotAggregation)()
			Dim i As Integer = 0
			Dim j As Integer = 0
			Do While i < NODES
				Dim arrayX As INDArray = Nd4j.create(5, ELEMENTS_PER_NODE)
				Dim arrayY As INDArray = Nd4j.create(5, ELEMENTS_PER_NODE)

				arrayX.assign(2.0)
				arrayY.assign(2.0)

				Dim aggregation As New DotAggregation(1L, NODES, CShort(i), arrayX.mul(arrayY))

				aggregations.Add(aggregation)
				i += 1
			Loop

			Dim aggregation As DotAggregation = aggregations(0)

			Dim cnt As Integer = 1
			For Each vectorAggregation As DotAggregation In aggregations
				aggregation.accumulateAggregation(vectorAggregation)
				cnt += 1

				' we're checking for actual number of missing chunks
				'assertEquals( NODES - cnt,aggregation.getMissingChunks());
			Next vectorAggregation

			Dim result As INDArray = aggregation.AccumulatedResult
			assertArrayEquals(exp.shapeInfoDataBuffer().asInt(), result.shapeInfoDataBuffer().asInt())
			assertEquals(exp, result)
		End Sub

	End Class

End Namespace