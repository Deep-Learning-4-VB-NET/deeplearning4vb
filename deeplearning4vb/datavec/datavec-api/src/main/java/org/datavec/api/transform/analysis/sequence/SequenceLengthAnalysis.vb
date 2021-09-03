Imports System
Imports System.Text
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data

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

Namespace org.datavec.api.transform.analysis.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @Builder public class SequenceLengthAnalysis implements java.io.Serializable
	<Serializable>
	Public Class SequenceLengthAnalysis

		Private totalNumSequences As Long
		Private minSeqLength As Integer
		Private maxSeqLength As Integer
		Private countZeroLength As Long
		Private countOneLength As Long
		Private meanLength As Double
		Private histogramBuckets() As Double
		Private histogramBucketCounts() As Long

		Protected Friend Sub New()
			'No-arg for JSON
		End Sub

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("SequenceLengthAnalysis(").Append("totalNumSequences=").Append(totalNumSequences).Append(",minSeqLength=").Append(minSeqLength).Append(",maxSeqLength=").Append(maxSeqLength).Append(",countZeroLength=").Append(countZeroLength).Append(",countOneLength=").Append(countOneLength).Append(",meanLength=").Append(meanLength).Append(")")
			Return sb.ToString()
		End Function

	End Class

End Namespace