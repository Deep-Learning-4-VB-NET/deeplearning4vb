Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports AdaMax = org.nd4j.linalg.learning.config.AdaMax
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nadam = org.nd4j.linalg.learning.config.Nadam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
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

Namespace org.nd4j.linalg.learning

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.TRAINING) @NativeTag @Tag(TagNames.DL4J_OLD_API) public class UpdaterTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class UpdaterTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaGradLegacy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaGradLegacy(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 1
			Dim cols As Integer = 1


			Dim grad As New org.nd4j.linalg.learning.legacy.AdaGrad(rows, cols, 1e-3)
			grad.setStateViewArray(Nd4j.zeros(1, rows * cols), New Integer() {rows, cols}, "c"c, True)
			Dim w As INDArray = Nd4j.ones(rows, cols)
			grad.getGradient(w, 0)
			assertEquals(1e-1, w.getDouble(0), 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNesterovs(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNesterovs(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 10
			Dim cols As Integer = 2

			Dim grad As New NesterovsUpdater(New Nesterovs(0.5, 0.9))
			grad.setStateViewArray(Nd4j.zeros(1, rows * cols), New Long() {rows, cols}, "c"c, True)
			Dim W As INDArray = Nd4j.zeros(rows, cols)
			Dim dist As Distribution = Nd4j.Distributions.createNormal(1, 1)
			Dim i As Integer = 0
			Do While i < W.rows()
				W.putRow(i, Nd4j.create(dist.sample(W.columns())))
				i += 1
			Loop

			For i As Integer = 0 To 4
				'            String learningRates = String.valueOf("\nAdagrad\n " + grad.applyUpdater(W, i)).replaceAll(";", "\n");
				'            System.out.println(learningRates);
				W.addi(Nd4j.randn(rows, cols))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaGrad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaGrad(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 10
			Dim cols As Integer = 2

			Dim grad As New AdaGradUpdater(New AdaGrad(0.1, AdaGrad.DEFAULT_ADAGRAD_EPSILON))
			grad.setStateViewArray(Nd4j.zeros(1, rows * cols), New Long() {rows, cols}, "c"c, True)
			Dim W As INDArray = Nd4j.zeros(rows, cols)
			Dim dist As Distribution = Nd4j.Distributions.createNormal(1, 1)
			Dim i As Integer = 0
			Do While i < W.rows()
				W.putRow(i, Nd4j.create(dist.sample(W.columns())))
				i += 1
			Loop

			For i As Integer = 0 To 4
				'            String learningRates = String.valueOf("\nAdagrad\n " + grad.applyUpdater(W, i)).replaceAll(";", "\n");
				'            System.out.println(learningRates);
				W.addi(Nd4j.randn(rows, cols))
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaDelta(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaDelta(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 10
			Dim cols As Integer = 2


			Dim grad As New AdaDeltaUpdater(New AdaDelta())
			grad.setStateViewArray(Nd4j.zeros(1, 2 * rows * cols), New Long() {rows, cols}, "c"c, True)
			Dim W As INDArray = Nd4j.zeros(rows, cols)
			Dim dist As Distribution = Nd4j.Distributions.createNormal(1e-3, 1e-3)
			Dim i As Integer = 0
			Do While i < W.rows()
				W.putRow(i, Nd4j.create(dist.sample(W.columns())))
				i += 1
			Loop

			For i As Integer = 0 To 4
				'            String learningRates = String.valueOf("\nAdaelta\n " + grad.applyUpdater(W, i)).replaceAll(";", "\n");
				'            System.out.println(learningRates);
				W.addi(Nd4j.randn(rows, cols))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdam(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdam(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 10
			Dim cols As Integer = 2


			Dim grad As New AdamUpdater(New Adam())
			grad.setStateViewArray(Nd4j.zeros(1, 2 * rows * cols), New Long() {rows, cols}, "c"c, True)
			Dim W As INDArray = Nd4j.zeros(rows, cols)
			Dim dist As Distribution = Nd4j.Distributions.createNormal(1e-3, 1e-3)
			Dim i As Integer = 0
			Do While i < W.rows()
				W.putRow(i, Nd4j.create(dist.sample(W.columns())))
				i += 1
			Loop

			For i As Integer = 0 To 4
				'            String learningRates = String.valueOf("\nAdamUpdater\n " + grad.applyUpdater(W, i)).replaceAll(";", "\n");
				'            System.out.println(learningRates);
				W.addi(Nd4j.randn(rows, cols))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNadam(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNadam(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 10
			Dim cols As Integer = 2

			Dim grad As New NadamUpdater(New Nadam())
			grad.setStateViewArray(Nd4j.zeros(1, 2 * rows * cols), New Long() {rows, cols}, "c"c, True)
			Dim W As INDArray = Nd4j.zeros(rows, cols)
			Dim dist As Distribution = Nd4j.Distributions.createNormal(1e-3, 1e-3)
			Dim i As Integer = 0
			Do While i < W.rows()
				W.putRow(i, Nd4j.create(dist.sample(W.columns())))
				i += 1
			Loop

			For i As Integer = 0 To 4
				'            String learningRates = String.valueOf("\nAdamUpdater\n " + grad.applyUpdater(W, i)).replaceAll(";", "\n");
				'            System.out.println(learningRates);
				W.addi(Nd4j.randn(rows, cols))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaMax(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 10
			Dim cols As Integer = 2


			Dim grad As New AdaMaxUpdater(New AdaMax())
			grad.setStateViewArray(Nd4j.zeros(1, 2 * rows * cols), New Long() {rows, cols}, "c"c, True)
			Dim W As INDArray = Nd4j.zeros(rows, cols)
			Dim dist As Distribution = Nd4j.Distributions.createNormal(1e-3, 1e-3)
			Dim i As Integer = 0
			Do While i < W.rows()
				W.putRow(i, Nd4j.create(dist.sample(W.columns())))
				i += 1
			Loop

			For i As Integer = 0 To 4
				'            String learningRates = String.valueOf("\nAdaMax\n " + grad.getGradient(W, i)).replaceAll(";", "\n");
				'            System.out.println(learningRates);
				W.addi(Nd4j.randn(rows, cols))
			Next i
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace