Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports GradCheckUtil = org.nd4j.autodiff.validation.GradCheckUtil
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports TrainingTag = org.nd4j.common.tests.tags.TrainingTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.autodiff.opvalidation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.SAMEDIFF) @TrainingTag public class ActivationGradChecks extends BaseOpValidation
	Public Class ActivationGradChecks
		Inherits BaseOpValidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testActivationGradientCheck1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testActivationGradientCheck1(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("x", Nd4j.rand(DataType.DOUBLE, 3, 4))
			Dim tanh As SDVariable = sd.math().tanh("tanh", [in])
			Dim loss As SDVariable = tanh.std(True)

			Dim c As GradCheckUtil.ActGradConfig = GradCheckUtil.ActGradConfig.builder().sd(sd).activationGradsToCheck(Collections.singletonList("tanh")).build()

			Dim ok As Boolean = GradCheckUtil.checkActivationGradients(c)

			assertTrue(ok)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testActivationGradientCheck2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testActivationGradientCheck2(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.placeHolder("x", DataType.DOUBLE, 3, 4)
			Dim y As SDVariable = sd.var("y", Nd4j.rand(DataType.DOUBLE, 4, 5))
			Dim mmul As SDVariable = x.mmul("mmul", y)
			Dim sigmoid As SDVariable = sd.math().tanh("sigmoid", mmul)
			Dim loss As SDVariable = sigmoid.std(True)

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("x") = Nd4j.rand(DataType.DOUBLE, 3, 4)

			Dim c As GradCheckUtil.ActGradConfig = GradCheckUtil.ActGradConfig.builder().sd(sd).placeholderValues(m).activationGradsToCheck(Arrays.asList("sigmoid", "mmul")).subset(GradCheckUtil.Subset.RANDOM).maxPerParam(10).build()

			Dim ok As Boolean = GradCheckUtil.checkActivationGradients(c)

			assertTrue(ok)
		End Sub
	End Class

End Namespace