Imports System
Imports System.Collections.Generic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.dimensionalityreduction.RandomProjection.johnsonLindenStraussMinDim
import static org.nd4j.linalg.dimensionalityreduction.RandomProjection.targetShape

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

Namespace org.nd4j.linalg.dimensionalityreduction


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.NDARRAY_ETL) @NativeTag public class TestRandomProjection extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestRandomProjection
		Inherits BaseNd4jTestWithBackends

		Friend z1 As INDArray = Nd4j.createUninitialized(New Integer(){CInt(Math.Truncate(1e))6, 1000})


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJohnsonLindenStraussDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJohnsonLindenStraussDim(ByVal backend As Nd4jBackend)
			assertEquals(663, CInt(Math.Truncate(johnsonLindenStraussMinDim(CInt(Math.Truncate(1e))6, 0.5).get(0))))
			assertTrue(johnsonLindenStraussMinDim(CInt(Math.Truncate(1e))6, 0.5).Equals(New List(Of Integer)(Arrays.asList(663))))

			Dim res1 As New List(Of Integer) From {663, 11841, 1112658}
			assertEquals(johnsonLindenStraussMinDim(CInt(Math.Truncate(1e))6, 0.5, 0.1, 0.01), res1)

			Dim res2 As New List(Of Integer) From {7894, 9868, 11841}
			assertEquals(RandomProjection.johnsonLindenstraussMinDim(New Integer(){CInt(Math.Truncate(1e))4, CInt(Math.Truncate(1e))5, CInt(Math.Truncate(1e))6}, 0.1), res2)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTargetShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTargetShape(ByVal backend As Nd4jBackend)
			assertArrayEquals(targetShape(z1, 0.5), New Long(){1000, 663})
			assertArrayEquals(targetShape(Nd4j.createUninitialized(New Integer(){CInt(Math.Truncate(1e))2, 225}), 0.5), New Long(){225, 221})
			' non-changing estimate
			assertArrayEquals(targetShape(z1, 700), New Long(){1000, 700})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTargetEpsilonChecks(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTargetEpsilonChecks(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			targetShape(z1, 0.0)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTargetShapeTooHigh(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTargetShapeTooHigh(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			targetShape(Nd4j.createUninitialized(New Integer(){CInt(Math.Truncate(1e))2, 1}), 0.5)
			targetShape(z1, 1001)
			targetShape(z1, 0.1)
			targetShape(Nd4j.createUninitialized(New Integer(){1, 1000}), 0.5)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicEmbedding(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicEmbedding(ByVal backend As Nd4jBackend)
			Dim z1 As INDArray = Nd4j.randn(10000, 500)
			Dim rp As New RandomProjection(0.5)
			Dim res As INDArray = Nd4j.zeros(10000, 442)
			Dim z2 As INDArray = rp.projecti(z1, res)
			assertArrayEquals(New Long(){10000, 442}, z2.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmbedding(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmbedding(ByVal backend As Nd4jBackend)
			Dim z1 As INDArray = Nd4j.randn(2000, 400)
			Dim z2 As INDArray = z1.dup()
			Dim result As INDArray = Transforms.allEuclideanDistances(z1, z2, 1)

			Dim rp As New RandomProjection(0.5)
			Dim zp As INDArray = rp.project(z1)
			Dim zp2 As INDArray = zp.dup()
			Dim projRes As INDArray = Transforms.allEuclideanDistances(zp, zp2, 1)

			' check that the automatically tuned values for the density respect the
			' contract for eps: pairwise distances are preserved according to the
			' Johnson-Lindenstrauss lemma
			Dim ratios As INDArray = projRes.div(result)

			For i As Integer = 0 To ratios.length() - 1
				Dim val As Double = ratios.getDouble(i)
				' this avoids the NaNs we get along the diagonal
				If val = val Then
					assertTrue(ratios.getDouble(i) < 1.5)
				End If
			Next i

		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function

	End Class

End Namespace