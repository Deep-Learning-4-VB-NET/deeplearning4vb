import static org.junit.jupiter.api.Assertions.assertEquals
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationBinary = org.nd4j.evaluation.classification.EvaluationBinary
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class NewInstanceTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NewInstanceTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testNewInstances(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewInstances(ByVal backend As Nd4jBackend)
			Dim print As Boolean = True
			Nd4j.Random.setSeed(12345)

			Dim evaluation As New Evaluation()
			Dim evaluationBinary As New EvaluationBinary()
			Dim roc As New ROC(2)
			Dim roc2 As New ROCBinary(2)
			Dim roc3 As New ROCMultiClass(2)
			Dim regressionEvaluation As New RegressionEvaluation()
			Dim ec As New EvaluationCalibration()


			Dim arr() As IEvaluation = {evaluation, evaluationBinary, roc, roc2, roc3, regressionEvaluation, ec}

			Dim evalLabel1 As INDArray = Nd4j.create(10, 3)
			For i As Integer = 0 To 9
				evalLabel1.putScalar(i, i Mod 3, 1.0)
			Next i
			Dim evalProb1 As INDArray = Nd4j.rand(10, 3)
			evalProb1.diviColumnVector(evalProb1.sum(1))

			evaluation.eval(evalLabel1, evalProb1)
			roc3.eval(evalLabel1, evalProb1)
			ec.eval(evalLabel1, evalProb1)

			Dim evalLabel2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(10, 3), 0.5))
			Dim evalProb2 As INDArray = Nd4j.rand(10, 3)
			evaluationBinary.eval(evalLabel2, evalProb2)
			roc2.eval(evalLabel2, evalProb2)

			Dim evalLabel3 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(10, 1), 0.5))
			Dim evalProb3 As INDArray = Nd4j.rand(10, 1)
			roc.eval(evalLabel3, evalProb3)

			Dim reg1 As INDArray = Nd4j.rand(10, 3)
			Dim reg2 As INDArray = Nd4j.rand(10, 3)

			regressionEvaluation.eval(reg1, reg2)

			Dim evaluation2 As Evaluation = evaluation.newInstance()
			Dim evaluationBinary2 As EvaluationBinary = evaluationBinary.newInstance()
			Dim roc_2 As ROC = roc.newInstance()
			Dim roc22 As ROCBinary = roc2.newInstance()
			Dim roc32 As ROCMultiClass = roc3.newInstance()
			Dim regressionEvaluation2 As RegressionEvaluation = regressionEvaluation.newInstance()
			Dim ec2 As EvaluationCalibration = ec.newInstance()

			Dim arr2() As IEvaluation = {evaluation2, evaluationBinary2, roc_2, roc22, roc32, regressionEvaluation2, ec2}

			evaluation2.eval(evalLabel1, evalProb1)
			roc32.eval(evalLabel1, evalProb1)
			ec2.eval(evalLabel1, evalProb1)

			evaluationBinary2.eval(evalLabel2, evalProb2)
			roc22.eval(evalLabel2, evalProb2)

			roc_2.eval(evalLabel3, evalProb3)

			regressionEvaluation2.eval(reg1, reg2)

			For i As Integer = 0 To arr.Length - 1

				Dim e As IEvaluation = arr(i)
				Dim e2 As IEvaluation = arr2(i)
				assertEquals("Json not equal ", e.toJson(), e2.toJson())
				assertEquals("Stats not equal ", e.stats(), e2.stats())
			Next i
		End Sub

	End Class

End Namespace