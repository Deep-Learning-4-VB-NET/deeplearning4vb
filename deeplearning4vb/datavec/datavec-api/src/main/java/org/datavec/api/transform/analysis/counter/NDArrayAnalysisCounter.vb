Imports System
Imports System.Collections.Generic
Imports org.datavec.api.transform.analysis
Imports NDArrayAnalysis = org.datavec.api.transform.analysis.columns.NDArrayAnalysis
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


	Public Class NDArrayAnalysisCounter
		Implements AnalysisCounter(Of NDArrayAnalysisCounter)

		Private countTotal As Long
		Private countNull As Long
		Private minLength As Long = Long.MaxValue
		Private maxLength As Long = -1
		Private totalNDArrayValues As Long
		Private countsByRank As IDictionary(Of Integer, Long) = New Dictionary(Of Integer, Long)()
		Private minValue As Double = Double.MaxValue
		Private maxValue As Double = -Double.MaxValue

		Public Overridable Function add(ByVal writable As Writable) As NDArrayAnalysisCounter
			Dim n As NDArrayWritable = DirectCast(writable, NDArrayWritable)
			Dim arr As INDArray = n.get()
			countTotal += 1
			If arr Is Nothing Then
				countNull += 1
			Else
				minLength = Math.Min(minLength, arr.length())
				maxLength = Math.Max(maxLength, arr.length())

				Dim r As Integer = arr.rank()
				If countsByRank.ContainsKey(arr.rank()) Then
					countsByRank(r) = countsByRank(r) + 1
				Else
					countsByRank(r) = 1L
				End If

				totalNDArrayValues += arr.length()
				minValue = Math.Min(minValue, arr.minNumber().doubleValue())
				maxValue = Math.Max(maxValue, arr.maxNumber().doubleValue())
			End If

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As NDArrayAnalysisCounter) As NDArrayAnalysisCounter
			Me.countTotal += other.countTotal
			Me.countNull += other.countNull
			Me.minLength = Math.Min(Me.minLength, other.minLength)
			Me.maxLength = Math.Max(Me.maxLength, other.maxLength)
			Me.totalNDArrayValues += other.totalNDArrayValues
			Dim allKeys As ISet(Of Integer) = New HashSet(Of Integer)(countsByRank.Keys)
			allKeys.addAll(other.countsByRank.Keys)
			For Each i As Integer? In allKeys
				Dim count As Long = 0
				If countsByRank.ContainsKey(i) Then
					count += countsByRank(i)
				End If
				If other.countsByRank.ContainsKey(i) Then
					count += other.countsByRank(i)
				End If
				countsByRank(i) = count
			Next i
			Me.minValue = Math.Min(Me.minValue, other.minValue)
			Me.maxValue = Math.Max(Me.maxValue, other.maxValue)

			Return Me
		End Function

		Public Overridable Function toAnalysisObject() As NDArrayAnalysis
			Return NDArrayAnalysis.Builder().countTotal(countTotal).countNull(countNull).minLength(minLength).maxLength(maxLength).totalNDArrayValues(totalNDArrayValues).countsByRank(countsByRank).minValue(minValue).maxValue(maxValue).build()
		End Function
	End Class

End Namespace