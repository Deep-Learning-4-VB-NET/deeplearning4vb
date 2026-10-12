Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports ThresholdAlgorithmReducer = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithmReducer
Imports AdaptiveThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.AdaptiveThresholdAlgorithm
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.optimize.solver.accumulation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class ThresholdAlgorithmTests extends org.deeplearning4j.BaseDL4JTest
	Public Class ThresholdAlgorithmTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdaptiveThresholdAlgorithm() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAdaptiveThresholdAlgorithm()

			Dim initialThreshold As Double = 1e-3
			Dim minTargetSparsity As Double = 1e-3
			Dim maxTargetSparsity As Double = 1e-2
			Dim decayRate As Double = 0.95
			Dim ta As ThresholdAlgorithm = New AdaptiveThresholdAlgorithm(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate)

			'First call: expect last threshold, last was dense, last sparsity etc to be null
			Dim update As INDArray = Nd4j.rand(New Long(){1, 100})
			update.muli(2).subi(1) '-1 to 1
			update.muli(initialThreshold * 2) '-2*initialThreshold to 2*initialThreshold

			Dim t1 As Double = ta.calculateThreshold(0, 0, Nothing, Nothing, Nothing, update)
			assertEquals(initialThreshold, t1, 0.0)

			'Second call: assume first encoding was dense using initial threshold -> increase threshold (reduce sparsity ratio -> more sparse)
			Dim t2 As Double = ta.calculateThreshold(1, 0, initialThreshold, True, Nothing, update)
			Dim expT2 As Double = (1.0 / decayRate) * initialThreshold
			assertEquals(expT2, t2, 1e-6)

			'Third call: assume second encoding was sparse, but greater than max sparsity target -> increase threshold (reduce sparsity ratio -> more sparse)
			Dim t3 As Double = ta.calculateThreshold(2, 0, t2, False, 1e-1, update)
			Dim expT3 As Double = (1.0 / decayRate) * t2
			assertEquals(expT3, t3, 1e-6)

			'Fourth call: assume third encoding was sparse, but smaller than min sparsity target -> decrease threshold (increase sparsity ratio -> less sparse)
			Dim t4 As Double = ta.calculateThreshold(3, 0, t3, False, 1e-4, update)
			Dim expT4 As Double = decayRate * t3
			assertEquals(expT4, t4, 1e-6)

			'Check that the last threshold is set:
			Dim f As System.Reflection.FieldInfo = GetType(AdaptiveThresholdAlgorithm).getDeclaredField("lastThreshold")
			f.setAccessible(True)
			Dim fValue As Double = CType(f.get(ta), Double?)
			assertEquals(t4, fValue, 0.0)

			'Check combining:
			Dim ta2 As AdaptiveThresholdAlgorithm = DirectCast(ta.clone(), AdaptiveThresholdAlgorithm)
			assertEquals(ta, ta2)

			Dim reducer As ThresholdAlgorithmReducer = ta.newReducer()
			reducer.add(ta)
			reducer.add(ta2)

			Dim reduced As ThresholdAlgorithm = reducer.FinalResult
			assertEquals(reduced, ta)

			'Check combining with unused:
			reducer.add(New AdaptiveThresholdAlgorithm(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate))
			reduced = reducer.FinalResult
			assertEquals(reduced, ta)

			'Check "first iteration in second epoch" uses the stored threshold, not the passed in one
			'Should re-use last sparsity ratio of 1e-4 -> decrease threshold
			Dim t5 As Double = reduced.calculateThreshold(5, 1, Nothing, Nothing, Nothing, update)
			Dim expT5 As Double = decayRate * t4
			assertEquals(expT5, t5, 1e-6)


			'Check combining with different values:
			Dim taA As ThresholdAlgorithm = New AdaptiveThresholdAlgorithm(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate)
			Dim taB As ThresholdAlgorithm = New AdaptiveThresholdAlgorithm(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate)

			f.set(taA, 1e-4)
			f.set(taB, 5e-4)

			Dim r2 As ThresholdAlgorithmReducer = taA.newReducer()
			r2.add(taA)

			Dim r3 As ThresholdAlgorithmReducer = taB.newReducer()
			r3.add(taB)

			r2.merge(r3)

			reduced = r2.FinalResult

			fValue = CType(f.get(reduced), Double?)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			assertEquals((1e-4 + 5e-4)/2.0, fValue, 1e-10)
		End Sub

	End Class

End Namespace