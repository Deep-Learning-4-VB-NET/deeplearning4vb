Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports LossBinaryXENT = org.nd4j.linalg.lossfunctions.impl.LossBinaryXENT
Imports LossL1 = org.nd4j.linalg.lossfunctions.impl.LossL1
Imports LossL2 = org.nd4j.linalg.lossfunctions.impl.LossL2
Imports LossMAE = org.nd4j.linalg.lossfunctions.impl.LossMAE
Imports LossMAPE = org.nd4j.linalg.lossfunctions.impl.LossMAPE
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports LossMSLE = org.nd4j.linalg.lossfunctions.impl.LossMSLE
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
Imports LossSparseMCXENT = org.nd4j.linalg.lossfunctions.impl.LossSparseMCXENT
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

Namespace org.nd4j.linalg.lossfunctions

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.LOSS_FUNCTIONS) @Tag(TagNames.TRAINING) @NativeTag @Tag(TagNames.DL4J_OLD_API) public class LossFunctionTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LossFunctionTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClippingXENT(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testClippingXENT(ByVal backend As Nd4jBackend)

			Dim l1 As ILossFunction = New LossBinaryXENT(0)
			Dim l2 As ILossFunction = New LossBinaryXENT()

			Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(3, 5), 0.5))
			Dim preOut As INDArray = Nd4j.valueArrayOf(3, 5, -1000.0)

			Dim a As IActivation = New ActivationSigmoid()

			Dim score1 As Double = l1.computeScore(labels, preOut.dup(), a, Nothing, False)
			assertTrue(Double.IsNaN(score1))

			Dim score2 As Double = l2.computeScore(labels, preOut.dup(), a, Nothing, False)
			assertFalse(Double.IsNaN(score2))

			Dim grad1 As INDArray = l1.computeGradient(labels, preOut.dup(), a, Nothing)
			Dim grad2 As INDArray = l2.computeGradient(labels, preOut.dup(), a, Nothing)

			Dim c1 As New MatchCondition(grad1, Conditions.Nan)
			Dim c2 As New MatchCondition(grad2, Conditions.Nan)
			Dim match1 As Integer = Nd4j.Executioner.exec(c1).getInt(0)
			Dim match2 As Integer = Nd4j.Executioner.exec(c2).getInt(0)

			assertTrue(match1 > 0)
			assertEquals(0, match2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWeightedLossFunctionDTypes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWeightedLossFunctionDTypes(ByVal backend As Nd4jBackend)

			For Each activationsDt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				For Each weightsDt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					For Each rank1W As Boolean In New Boolean(){False, True}

						Dim preOut As INDArray = Nd4j.rand(activationsDt, 2, 3)
						Dim l As INDArray = Nd4j.rand(activationsDt, 2, 3)

						Dim w As INDArray = Nd4j.createFromArray(1.0f, 2.0f, 3.0f).castTo(weightsDt)
						If Not rank1W Then
							w = w.reshape(ChrW(1), 3)
						End If

						Dim lf As ILossFunction = Nothing
						For i As Integer = 0 To 9
							Select Case i
								Case 0
									lf = New LossBinaryXENT(w)
								Case 1
									lf = New LossL1(w)
								Case 2
									lf = New LossL2(w)
								Case 3
									lf = New LossMAE(w)
								Case 4
									lf = New LossMAPE(w)
								Case 5
									lf = New LossMCXENT(w)
								Case 6
									lf = New LossMSE(w)
								Case 7
									lf = New LossMSLE(w)
								Case 8
									lf = New LossNegativeLogLikelihood(w)
								Case 9
									lf = New LossSparseMCXENT(w)
									l = Nd4j.createFromArray(1,2).reshape(2, 1).castTo(activationsDt)
								Case Else
									Throw New Exception()
							End Select
						Next i

						'Check score
						lf.computeScore(l, preOut, New ActivationSoftmax(), Nothing, True)

						'Check backward
						lf.computeGradient(l, preOut, New ActivationSoftmax(), Nothing)
					Next rank1W
				Next weightsDt
			Next activationsDt

		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace