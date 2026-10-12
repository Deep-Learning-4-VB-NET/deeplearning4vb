Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports CudnnDropoutHelper = org.deeplearning4j.cuda.dropout.CudnnDropoutHelper
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda.lstm

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class ValidateCudnnDropout extends org.deeplearning4j.BaseDL4JTest
	Public Class ValidateCudnnDropout
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCudnnDropoutSimple()
		Public Overridable Sub testCudnnDropoutSimple()
			For Each shape As Integer() In New Integer()(){
				New Integer() {10, 10},
				New Integer() {5, 2, 5, 2}
			}

				Nd4j.Random.setSeed(12345)
				Dim [in] As INDArray = Nd4j.ones(shape)
				Dim pRetain As Double = 0.25
				Dim valueIfKept As Double = 1.0 / pRetain

				Dim d As New CudnnDropoutHelper(DataType.DOUBLE)

				Dim [out] As INDArray = Nd4j.createUninitialized(shape)
				d.applyDropout([in], [out], pRetain)

				Dim countZero As Integer = Nd4j.Executioner.execAndReturn(New MatchCondition([out], Conditions.equals(0.0))).z().getInt(0)
				Dim countNonDropped As Integer = Nd4j.Executioner.execAndReturn(New MatchCondition([out], Conditions.equals(valueIfKept))).z().getInt(0)
	'            System.out.println(countZero);
	'            System.out.println(countNonDropped);

				assertTrue(countZero >= 5 AndAlso countZero <= 90, countZero.ToString())
				assertTrue(countNonDropped >= 5 AndAlso countNonDropped <= 95, countNonDropped.ToString())
				assertEquals(100, countZero + countNonDropped)

				'Test repeatability:
				For i As Integer = 0 To 9
					Nd4j.Random.setSeed(12345)
					d.setRngStates(Nothing)
					d.setMask(Nothing)

					Dim outNew As INDArray = Nd4j.createUninitialized(shape)
					d.applyDropout([in], outNew, pRetain)

					assertEquals([out], outNew)
				Next i

				'Test backprop:
				Dim gradAtOut As INDArray = Nd4j.ones(shape)
				Dim gradAtInput As INDArray = Nd4j.createUninitialized(shape)
				d.backprop(gradAtOut, gradAtInput)
				Nd4j.Executioner.commit()

				'If dropped: expect 0. Otherwise: expect 1/pRetain, i.e., output for 1s input
				assertEquals([out], gradAtInput)
			Next shape
		End Sub

	End Class

End Namespace