Imports Function2 = org.apache.spark.api.java.function.Function2

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

Namespace org.datavec.spark.transform.analysis.seqlength

	Public Class SequenceLengthAnalysisAddFunction
		Implements Function2(Of SequenceLengthAnalysisCounter, Integer, SequenceLengthAnalysisCounter)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public SequenceLengthAnalysisCounter call(SequenceLengthAnalysisCounter v1, System.Nullable<Integer> length) throws Exception
		Public Overrides Function [call](ByVal v1 As SequenceLengthAnalysisCounter, ByVal length As Integer?) As SequenceLengthAnalysisCounter

			Dim zero As Long = v1.getCountZeroLength()
			Dim one As Long = v1.getCountOneLength()

			If length = 0 Then
				zero += 1
			ElseIf length = 1 Then
				one += 1
			End If

			Dim newMinValue As Integer
			Dim countMinValue As Long = v1.getCountMinLength()
			If length = v1.getMinLengthSeen() Then
				newMinValue = length
				countMinValue += 1
			ElseIf v1.getMinLengthSeen() > length Then
				newMinValue = length
				countMinValue = 1
			Else
				newMinValue = v1.getMinLengthSeen()
				'no change to count
			End If

			Dim newMaxValue As Integer
			Dim countMaxValue As Long = v1.getCountMaxLength()
			If length = v1.getMaxLengthSeen() Then
				newMaxValue = length
				countMaxValue += 1
			ElseIf v1.getMaxLengthSeen() < length Then
				'reset max counter
				newMaxValue = length
				countMaxValue = 1
			Else
				newMaxValue = v1.getMaxLengthSeen()
				'no change to count
			End If

			'New mean:
			Dim sum As Double = v1.getMean() * v1.getCountTotal() + length.Value
			Dim newTotalCount As Long = v1.getCountTotal() + 1
			Dim newMean As Double = sum / newTotalCount

			Return New SequenceLengthAnalysisCounter(zero, one, countMinValue, newMinValue, countMaxValue, newMaxValue, newTotalCount, newMean)
		End Function
	End Class

End Namespace