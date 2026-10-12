Imports System.Collections.Generic
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Test = org.junit.jupiter.api.Test
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Abstract Data Set Iterator Test") class AbstractDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class AbstractDataSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Next") void next() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub [next]()
			Dim numFeatures As Integer = 128
			Dim batchSize As Integer = 10
			Dim numRows As Integer = 1000
			Dim cnt As New AtomicInteger(0)
			Dim iterator As New FloatsDataSetIterator(floatIterable(numRows, numFeatures), batchSize)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iterator.hasNext())
			Do While iterator.MoveNext()
				Dim dataSet As DataSet = iterator.Current
				Dim features As INDArray = dataSet.Features
				assertEquals(batchSize, features.rows())
				assertEquals(numFeatures, features.columns())
				cnt.incrementAndGet()
			Loop
			assertEquals(numRows \ batchSize, cnt.get())
		End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: protected static Iterable<org.nd4j.common.primitives.Pair<float[], float[]>> floatIterable(final int totalRows, final int numColumns)
		Protected Friend Shared Function floatIterable(ByVal totalRows As Integer, ByVal numColumns As Integer) As IEnumerable(Of Pair(Of Single(), Single()))
			Return New IterableAnonymousInnerClass(totalRows, numColumns)
		End Function

		Private Class IterableAnonymousInnerClass
			Implements IEnumerable(Of Pair(Of Single(), Single()))

			Private totalRows As Integer
			Private numColumns As Integer

			Public Sub New(ByVal totalRows As Integer, ByVal numColumns As Integer)
				Me.totalRows = totalRows
				Me.numColumns = numColumns
			End Sub


			Public Function GetEnumerator() As IEnumerator(Of Pair(Of Single(), Single())) Implements IEnumerator(Of Pair(Of Single(), Single())).GetEnumerator
				Return New IteratorAnonymousInnerClass(Me)
			End Function

			Private Class IteratorAnonymousInnerClass
				Implements IEnumerator(Of Pair(Of Single(), Single()))

				Private ReadOnly outerInstance As IterableAnonymousInnerClass

				Public Sub New(ByVal outerInstance As IterableAnonymousInnerClass)
					Me.outerInstance = outerInstance
					cnt = New AtomicInteger(0)
				End Sub


				Private cnt As AtomicInteger

				Public Function hasNext() As Boolean
					Return cnt.incrementAndGet() <= outerInstance.totalRows
				End Function

				Public Function [next]() As Pair(Of Single(), Single())
					Dim features(outerInstance.numColumns - 1) As Single
					Dim labels(outerInstance.numColumns - 1) As Single
					For i As Integer = 0 To outerInstance.numColumns - 1
						features(i) = CSng(i)
						labels(i) = RandomUtils.nextFloat(0, 5)
					Next i
					Return Pair.makePair(features, labels)
				End Function

				Public Sub remove()
					' no-op
				End Sub
			End Class
		End Class
	End Class

End Namespace