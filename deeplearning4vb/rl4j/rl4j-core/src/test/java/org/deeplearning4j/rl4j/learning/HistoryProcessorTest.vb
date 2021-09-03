Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
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

Namespace org.deeplearning4j.rl4j.learning

	''' 
	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class HistoryProcessorTest
	Public Class HistoryProcessorTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHistoryProcessor()
		Public Overridable Sub testHistoryProcessor()
			Dim conf As HistoryProcessor.Configuration = HistoryProcessor.Configuration.builder().croppingHeight(2).croppingWidth(2).rescaledHeight(2).rescaledWidth(2).build()
			Dim hp As IHistoryProcessor = New HistoryProcessor(conf)
			Dim a As INDArray = Nd4j.createFromArray(New Single()()() {
				New Single()() {
					New Single() {0.1f, 0.1f, 0.1f},
					New Single() {0.2f, 0.2f, 0.2f}
				},
				New Single()() {
					New Single() {0.3f, 0.3f, 0.3f},
					New Single() {0.4f, 0.4f, 0.4f}
				}
			})
			hp.add(a)
			hp.add(a)
			hp.add(a)
			hp.add(a)
			Dim h() As INDArray = hp.History
			assertEquals(4, h.Length)
			assertEquals(1, h(0).shape()(0))
			assertEquals(a.shape()(0), h(0).shape()(1))
			assertEquals(a.shape()(1), h(0).shape()(2))
			assertEquals(0.1f * hp.Scale, h(0).getDouble(0, 0, 0), 1)
			assertEquals(0.2f * hp.Scale, h(0).getDouble(0, 0, 1), 1)
			assertEquals(0.3f * hp.Scale, h(0).getDouble(0, 1, 0), 1)
			assertEquals(0.4f * hp.Scale, h(0).getDouble(0, 1, 1), 1)
		End Sub
	End Class

End Namespace