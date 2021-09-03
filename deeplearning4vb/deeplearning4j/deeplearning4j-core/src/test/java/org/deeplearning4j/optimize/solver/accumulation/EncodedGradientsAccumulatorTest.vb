Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports EncodedGradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.EncodedGradientsAccumulator
Imports EncodingHandler = org.deeplearning4j.optimize.solvers.accumulation.EncodingHandler
Imports FixedThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.FixedThresholdAlgorithm
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports PrintAffinity = org.nd4j.linalg.api.ops.util.PrintAffinity
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpaqueDataBuffer = org.nd4j.nativeblas.OpaqueDataBuffer
import static org.junit.jupiter.api.Assertions.assertNotNull
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
Namespace org.deeplearning4j.optimize.solver.accumulation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Encoded Gradients Accumulator Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class EncodedGradientsAccumulatorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class EncodedGradientsAccumulatorTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 1200000L
			End Get
		End Property

		''' <summary>
		''' This test ensures, that memory amount assigned to buffer is enough for any number of updates </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Store 1") void testStore1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testStore1()
			Dim numParams As Integer
			Dim workers() As Integer
			If IntegrationTests Then
				numParams = 100000
				workers = New Integer() {1, Nd4j.AffinityManager.NumberOfDevices}
			Else
				numParams = 10000
				workers = New Integer() {1, Nd4j.AffinityManager.NumberOfDevices}
			End If
			For Each numWorkers As Integer In workers
				Dim handler As New EncodingHandler(New FixedThresholdAlgorithm(1e-3), Nothing, Nothing, False)
				Dim bufferSize As val = EncodedGradientsAccumulator.getOptimalBufferSize(numParams, numWorkers, 2)
				log.info("Workers: {}; Buffer size: {} bytes", numWorkers, bufferSize)
				Dim accumulator As New EncodedGradientsAccumulator(numWorkers, handler, bufferSize, 2, Nothing, False)
				Dim e As Integer = 10
				Do While e < numParams \ 10
					Dim encoded As INDArray = handler.encodeUpdates(0, 0, getGradients(numParams, e, 2e-3))
					accumulator.receiveUpdate(encoded)
					' just purge updates, like they were consumed
					Dim i As Integer = 0
					Do While i < accumulator.getMessages().size()
						accumulator.getMessages().get(i).clear()
						i += 1
					Loop
					e += 1
				Loop
			Next numWorkers
		End Sub

		''' <summary>
		''' Here we ensure that no matter how dense/sparse our updates are - we're never going above 1/16 of original elements of gradients array
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Encoding Limits 1") void testEncodingLimits1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEncodingLimits1()
			Dim numParams As Integer
			If IntegrationTests Then
				numParams = 100000
			Else
				numParams = 10000
			End If
			Dim handler As New EncodingHandler(New FixedThresholdAlgorithm(1e-3), Nothing, Integer.MaxValue, False)
			Dim e As Integer = 10
			Do While e < numParams \ 5
				Dim gradients As val = getGradients(numParams, e, 2e-3)
				Dim encoded As val = handler.encodeUpdates(0, 0, gradients)
				assertNotNull(encoded,"Failed with e == " & e)
				Dim encFormat As Integer = encoded.data().getInt(3)
				assertTrue(encoded.data().length() < numParams \ 16 + 6,"Failed for E = " & e & "; Format: " & encFormat & "; Length: " & encoded.data().length())
				e += 1
			Loop
		End Sub

		Protected Friend Overridable Function getGradients(ByVal length As Integer, ByVal numPositives As Integer, ByVal value As Double) As INDArray
			Dim grad As INDArray = Nd4j.create(length)
			For i As Integer = 0 To numPositives - 1
				grad.putScalar(i, value)
			Next i
			Return grad
		End Function
	End Class

End Namespace