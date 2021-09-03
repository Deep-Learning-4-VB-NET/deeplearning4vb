Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.common.primitives
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.autodiff.internal


	Public Class TestDependencyTracker
		Inherits BaseNd4jTestWithBackends


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimple(ByVal backend As Nd4jBackend)

			Dim dt As New DependencyTracker(Of String, String)()

			dt.addDependency("y", "x")
			assertTrue(dt.hasDependency("y"))
			assertFalse(dt.hasDependency("x"))
			assertFalse(dt.hasDependency("z"))

			Dim dl As DependencyList(Of String, String) = dt.getDependencies("y")
			assertEquals("y", dl.getDependencyFor())
			assertNotNull(dl.getDependencies())
			assertNull(dl.getOrDependencies())
			assertEquals(Collections.singletonList("x"), dl.getDependencies())

			dt.removeDependency("y", "x")
			assertFalse(dt.hasDependency("y"))
			assertFalse(dt.hasDependency("x"))
			dl = dt.getDependencies("y")
			assertTrue(dl.getDependencies() Is Nothing OrElse dl.getDependencies().isEmpty())
			assertTrue(dl.getOrDependencies() Is Nothing OrElse dl.getOrDependencies().isEmpty())


			'Or dep
			dt.addOrDependency("y", "x1", "x2")
			assertTrue(dt.hasDependency("y"))
			dl = dt.getDependencies("y")
			assertTrue(dl.getDependencies() Is Nothing OrElse dl.getDependencies().isEmpty())
			assertTrue(dl.getOrDependencies() IsNot Nothing AndAlso Not dl.getOrDependencies().isEmpty())
			assertEquals(Collections.singletonList(New Pair(Of )("x1", "x2")), dl.getOrDependencies())

			dt.removeDependency("y", "x1")
			assertFalse(dt.hasDependency("y"))
			dl = dt.getDependencies("y")
			assertTrue(dl.getDependencies() Is Nothing OrElse dl.getDependencies().isEmpty())
			assertTrue(dl.getOrDependencies() Is Nothing OrElse dl.getOrDependencies().isEmpty())

			dt.addOrDependency("y", "x1", "x2")
			dl = dt.getDependencies("y")
			assertTrue(dl.getDependencies() Is Nothing OrElse dl.getDependencies().isEmpty())
			assertTrue(dl.getOrDependencies() IsNot Nothing AndAlso Not dl.getOrDependencies().isEmpty())
			assertEquals(Collections.singletonList(New Pair(Of )("x1", "x2")), dl.getOrDependencies())
			dt.removeDependency("y", "x2")
			assertTrue(dt.Empty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSatisfiedBeforeAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSatisfiedBeforeAdd(ByVal backend As Nd4jBackend)
			Dim dt As New DependencyTracker(Of String, String)()

			'Check different order of adding dependencies: i.e., mark X as satisfied, then add x -> y dependency
			' and check that y is added to satisfied list...
			dt.markSatisfied("x", True)
			dt.addDependency("y", "x")
			assertTrue(dt.hasNewAllSatisfied())
			assertEquals("y", dt.NewAllSatisfied)

			'Same as above - x satisfied, add x->y, then add z->y
			'y should go from satisfied to not satisfied
			dt.clear()
			assertTrue(dt.Empty)
			dt.markSatisfied("x", True)
			dt.addDependency("y", "x")
			assertTrue(dt.hasNewAllSatisfied())
			dt.addDependency("y", "z")
			assertFalse(dt.hasNewAllSatisfied())


			'x satisfied, then or(x,y) -> z added
			dt.markSatisfied("x", True)
			dt.addOrDependency("z", "x", "y")
			assertTrue(dt.hasNewAllSatisfied())
			assertEquals("z", dt.NewAllSatisfied)


			'x satisfied, then or(x,y) -> z added, then or(a,b)->z added (should be unsatisfied)
			dt.clear()
			assertTrue(dt.Empty)
			dt.markSatisfied("x", True)
			dt.addOrDependency("z", "x", "y")
			assertTrue(dt.hasNewAllSatisfied())
			dt.addOrDependency("z", "a", "b")
			assertFalse(dt.hasNewAllSatisfied())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMarkUnsatisfied(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMarkUnsatisfied(ByVal backend As Nd4jBackend)

			Dim dt As New DependencyTracker(Of String, String)()
			dt.addDependency("y", "x")
			dt.markSatisfied("x", True)
			assertTrue(dt.hasNewAllSatisfied())

			dt.markSatisfied("x", False)
			assertFalse(dt.hasNewAllSatisfied())
			dt.markSatisfied("x", True)
			assertTrue(dt.hasNewAllSatisfied())
			assertEquals("y", dt.NewAllSatisfied)
			assertFalse(dt.hasNewAllSatisfied())


			'Same for OR dependencies
			dt.clear()
			assertTrue(dt.Empty)
			dt.addOrDependency("z", "x", "y")
			dt.markSatisfied("x", True)
			assertTrue(dt.hasNewAllSatisfied())

			dt.markSatisfied("x", False)
			assertFalse(dt.hasNewAllSatisfied())
			dt.markSatisfied("x", True)
			assertTrue(dt.hasNewAllSatisfied())
			assertEquals("z", dt.NewAllSatisfied)
			assertFalse(dt.hasNewAllSatisfied())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @NativeTag public void testIdentityDependencyTracker()
		Public Overridable Sub testIdentityDependencyTracker()
			Dim dt As New IdentityDependencyTracker(Of INDArray, String)()
			assertTrue(dt.Empty)

			Dim y1 As INDArray = Nd4j.scalar(0)
			Dim y2 As INDArray = Nd4j.scalar(0)
			Dim x1 As String = "x1"
			dt.addDependency(y1, x1)

			assertFalse(dt.hasNewAllSatisfied())
			assertTrue(dt.hasDependency(y1))
			assertFalse(dt.hasDependency(y2))
			assertFalse(dt.isSatisfied(x1))

			Dim dl As DependencyList(Of INDArray, String) = dt.getDependencies(y1)
			assertSame(y1, dl.getDependencyFor()) 'Should be same object
			assertEquals(Collections.singletonList(x1), dl.getDependencies())
			assertNull(dl.getOrDependencies())


			'Mark as satisfied, check if it's added to list
			dt.markSatisfied(x1, True)
			assertTrue(dt.isSatisfied(x1))
			assertTrue(dt.hasNewAllSatisfied())
			Dim get As INDArray = dt.NewAllSatisfied
			assertSame(y1, get)
			assertFalse(dt.hasNewAllSatisfied())
		End Sub

	End Class

End Namespace