﻿Imports Platform = com.sun.jna.Platform
Imports SharedTrainingResult = org.deeplearning4j.spark.parameterserver.training.SharedTrainingResult
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.spark.parameterserver.accumulation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class SharedTrainingAggregateFunctionTest
	Public Class SharedTrainingAggregateFunctionTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAggregate1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAggregate1()
			Dim updates1 As INDArray = Nd4j.create(1000).assign(1.0)
			Dim updates2 As INDArray = Nd4j.create(1000).assign(2.0)
			Dim expUpdates As INDArray = Nd4j.create(1000).assign(3.0)

			Dim result1 As SharedTrainingResult = SharedTrainingResult.builder().updaterStateArray(updates1).aggregationsCount(1).scoreSum(1.0).build()

			Dim result2 As SharedTrainingResult = SharedTrainingResult.builder().updaterStateArray(updates2).aggregationsCount(1).scoreSum(2.0).build()

			' testing null + result
			Dim aggregateFunction As New SharedTrainingAggregateFunction()
			Dim tuple1 As SharedTrainingAccumulationTuple = aggregateFunction.call(Nothing, result1)


			' testing tuple + result
			Dim tuple2 As SharedTrainingAccumulationTuple = aggregateFunction.call(tuple1, result2)


			' testing final result
			assertEquals(2, tuple2.getAggregationsCount())
			assertEquals(3.0, tuple2.getScoreSum(), 0.001)
			assertEquals(expUpdates, tuple2.getUpdaterStateArray())
		End Sub
	End Class

End Namespace