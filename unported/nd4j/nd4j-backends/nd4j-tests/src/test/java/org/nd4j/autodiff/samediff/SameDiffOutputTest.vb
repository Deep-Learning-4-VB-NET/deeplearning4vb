Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.SAMEDIFF) public class SameDiffOutputTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SameDiffOutputTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void outputTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub outputTest(ByVal backend As Nd4jBackend)
			Dim data As New DataSet(Nd4j.zeros(10, 10), Nd4j.zeros(10, 10))
			Dim sd As SameDiff = SameDiff.create()

			Dim [in] As SDVariable = sd.placeHolder("input", DataType.FLOAT, 10, 10)
			Dim [out] As SDVariable = [in].add("out", 2)

			Dim conf As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(New Sgd(3e-1)).dataSetFeatureMapping("input").dataSetLabelMapping().build()

			sd.TrainingConfig = conf

			Dim output As INDArray = sd.output(data, "out")("out")

			assertTrue(output.equalsWithEps(Nd4j.zeros(10, 10).add(2).castTo(DataType.FLOAT), 0.0001),"output != input + 2")
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

	End Class

End Namespace