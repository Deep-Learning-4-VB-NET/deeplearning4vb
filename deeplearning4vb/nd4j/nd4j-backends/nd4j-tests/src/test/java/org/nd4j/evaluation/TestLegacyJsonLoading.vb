Imports FileUtils = org.apache.commons.io.FileUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.nd4j.evaluation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class TestLegacyJsonLoading extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestLegacyJsonLoading
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalLegacyFormat(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEvalLegacyFormat(ByVal backend As Nd4jBackend)

			Dim f As File = (New ClassPathResource("regression_testing/eval_100b/evaluation.json")).File
			Dim s As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
	'        System.out.println(s);

			Dim e As Evaluation = Evaluation.fromJson(s)

			assertEquals(0.78, e.accuracy(), 1e-4)
			assertEquals(0.80, e.precision(), 1e-4)
			assertEquals(0.7753, e.f1(), 1e-3)

			f = (New ClassPathResource("regression_testing/eval_100b/regressionEvaluation.json")).File
			s = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
			Dim re As RegressionEvaluation = RegressionEvaluation.fromJson(s)
			assertEquals(6.53809e-02, re.meanSquaredError(0), 1e-4)
			assertEquals(3.46236e-01, re.meanAbsoluteError(1), 1e-4)

			f = (New ClassPathResource("regression_testing/eval_100b/rocMultiClass.json")).File
			s = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
			Dim r As ROCMultiClass = ROCMultiClass.fromJson(s)

			assertEquals(0.9838, r.calculateAUC(0), 1e-4)
			assertEquals(0.7934, r.calculateAUC(1), 1e-4)
		End Sub

	End Class

End Namespace