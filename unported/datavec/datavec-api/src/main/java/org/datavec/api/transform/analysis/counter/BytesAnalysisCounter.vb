Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports org.datavec.api.transform.analysis
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.analysis.counter

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class BytesAnalysisCounter implements org.datavec.api.transform.analysis.AnalysisCounter<BytesAnalysisCounter>
	Public Class BytesAnalysisCounter
		Implements AnalysisCounter(Of BytesAnalysisCounter)

		Private countTotal As Long = 0



		Public Sub New()

		End Sub


		Public Overridable Function add(ByVal writable As Writable) As BytesAnalysisCounter
			countTotal += 1

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As BytesAnalysisCounter) As BytesAnalysisCounter

			Return New BytesAnalysisCounter(countTotal + other.countTotal)
		End Function

	End Class

End Namespace