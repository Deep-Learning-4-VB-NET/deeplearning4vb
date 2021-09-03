﻿import static org.junit.jupiter.api.Assertions.assertEquals
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.evaluation.custom
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.evaluation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class CustomEvaluationTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CustomEvaluationTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void customEvalTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub customEvalTest(ByVal backend As Nd4jBackend)
			Dim accuracyEval As CustomEvaluation = New CustomEvaluation(Of )(Function(labels, pred, mask, meta) New Pair(Of )(labels.eq(pred).castTo(DataType.INT).sumNumber(), labels.size(0)), CustomEvaluation.mergeConcatenate())

			accuracyEval.eval(Nd4j.createFromArray(1, 1, 2, 1, 3), Nd4j.createFromArray(1, 1, 4, 1, 2))

			Dim acc As Double = accuracyEval.getValue(New CustomEvaluation.Metric(Of Pair(Of Number, Long))(Function(list)
			Dim sum As Integer = 0
			Dim count As Integer = 0
			For Each p As Pair(Of Number, Long) In list
				sum += p.First.intValue()
				count += p.Second
			Next p
			Return (CDbl(sum)) / count
			End Function))

			assertEquals(acc, 3.0/5, 0.001,"Accuracy")

		End Sub

	End Class

End Namespace