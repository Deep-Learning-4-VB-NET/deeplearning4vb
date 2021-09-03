Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

Namespace org.nd4j.linalg.dataset



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class BalanceMinibatchesTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BalanceMinibatchesTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBalance(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBalance(ByVal backend As Nd4jBackend)
			Dim iterator As DataSetIterator = New IrisDataSetIterator(10, 150)

			Dim minibatches As New File(testDir.toFile(),"mini-batch-dir")
			Dim saveDir As New File(testDir.toFile(),"save-dir")

			Dim balanceMinibatches As BalanceMinibatches = BalanceMinibatches.builder().dataSetIterator(iterator).miniBatchSize(10).numLabels(3).rootDir(minibatches).rootSaveDir(saveDir).build()
			balanceMinibatches.balance()
			Dim balanced As DataSetIterator = New ExistingMiniBatchDataSetIterator(balanceMinibatches.getRootSaveDir())
			Do While balanced.MoveNext()
				assertTrue(balanced.Current.labelCounts().size() > 0)
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMiniBatchBalanced(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMiniBatchBalanced(ByVal backend As Nd4jBackend)

			Dim miniBatchSize As Integer = 100
			Dim iterator As DataSetIterator = New IrisDataSetIterator(miniBatchSize, 150)

			Dim minibatches As New File(testDir.toFile(),"mini-batch-dir")
			Dim saveDir As New File(testDir.toFile(),"save-dir")

			Dim balanceMinibatches As BalanceMinibatches = BalanceMinibatches.builder().dataSetIterator(iterator).miniBatchSize(miniBatchSize).numLabels(iterator.totalOutcomes()).rootDir(minibatches).rootSaveDir(saveDir).build()
			balanceMinibatches.balance()
			Dim balanced As DataSetIterator = New ExistingMiniBatchDataSetIterator(balanceMinibatches.getRootSaveDir())

			assertTrue(iterator.resetSupported()) ' this is testing the Iris dataset more than anything
			iterator.reset()
			Dim totalCounts(iterator.totalOutcomes() - 1) As Double

			Do While iterator.MoveNext()
				Dim outcomes As IDictionary(Of Integer, Double) = iterator.Current.labelCounts()
				Dim i As Integer = 0
				Do While i < iterator.totalOutcomes()
					If outcomes.ContainsKey(i) Then
						totalCounts(i) += outcomes(i)
					End If
					i += 1
				Loop
			Loop


			Dim fullBatches As IList(Of Integer) = New ArrayList(totalCounts.Length)
			For i As Integer = 0 To totalCounts.Length - 1
				fullBatches.Add(iterator.totalOutcomes() * CInt(Math.Truncate(totalCounts(i))) \ miniBatchSize)
			Next i


			' this is the number of batches for which we can balance every class
			Dim fullyBalanceableBatches As Integer = fullBatches.Min()
			' check the first few batches are actually balanced
			For b As Integer = 0 To fullyBalanceableBatches - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim balancedCounts As IDictionary(Of Integer, Double) = balanced.next().labelCounts()
				Dim i As Integer = 0
				Do While i < iterator.totalOutcomes()
					Dim bCounts As Double = (If(balancedCounts.ContainsKey(i), balancedCounts(i), 0))
					assertTrue(balancedCounts.ContainsKey(i) AndAlso balancedCounts(i) >= CDbl(miniBatchSize) / iterator.totalOutcomes(),"key " & i & " totalOutcomes: " & iterator.totalOutcomes() & " balancedCounts : " & balancedCounts.ContainsKey(i) & " val : " & bCounts)
					i += 1
				Loop
			Next b


		End Sub



		''' <summary>
		''' The ordering for this test
		''' This test will only be invoked for
		''' the given test  and ignored for others
		''' </summary>
		''' <returns> the ordering for this test </returns>
		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace