Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives

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

Namespace org.datavec.api.timeseries.util


	Public Class TimeSeriesWritableUtils

		''' <summary>
		''' Unchecked exception, thrown to signify that a zero-length sequence data set was encountered.
		''' </summary>
		Public Class ZeroLengthSequenceException
			Inherits Exception

			Public Sub New()
				Me.New("")
			End Sub

			Public Sub New(ByVal type As String)
				MyBase.New(String.Format("Encountered zero-length {0}sequence",If(type.Equals(""), "", type & " ")))
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Getter public static class RecordDetails
		Public Class RecordDetails
			Friend minValues As Integer
			Friend maxTSLength As Integer

		End Class

		''' <summary>
		''' Get the <seealso cref="RecordDetails"/>
		''' detailing the length of the time series </summary>
		''' <param name="record"> the input time series
		'''               to get the details for </param>
		''' <returns> the record details for the record </returns>
		Public Shared Function getDetails(ByVal record As IList(Of IList(Of IList(Of Writable)))) As RecordDetails
			Dim maxTimeSeriesLength As Integer = 0
			For Each [step] As IList(Of IList(Of Writable)) In record
				maxTimeSeriesLength = Math.Max(maxTimeSeriesLength,[step].Count)

			Next [step]

			Return RecordDetails.builder().minValues(record.Count).maxTSLength(maxTimeSeriesLength).build()
		End Function

		''' <summary>
		''' Convert the writables
		''' to a sequence (3d) data set,
		''' and also return the
		''' mask array (if necessary) </summary>
		''' <param name="timeSeriesRecord"> the input time series
		'''  </param>
		Public Shared Function convertWritablesSequence(ByVal timeSeriesRecord As IList(Of IList(Of IList(Of Writable)))) As Pair(Of INDArray, INDArray)
			Return convertWritablesSequence(timeSeriesRecord,getDetails(timeSeriesRecord))
		End Function

		''' <summary>
		''' Convert the writables
		''' to a sequence (3d) data set,
		''' and also return the
		''' mask array (if necessary)
		''' </summary>
		Public Shared Function convertWritablesSequence(ByVal list As IList(Of IList(Of IList(Of Writable))), ByVal details As RecordDetails) As Pair(Of INDArray, INDArray)


			Dim arr As INDArray

			If list(0).Count = 0 Then
				Throw New ZeroLengthSequenceException("Zero length sequence encountered")
			End If

			Dim firstStep As IList(Of Writable) = list(0)(0)

			Dim size As Integer = 0
			'Need to account for NDArrayWritables etc in list:
			For Each w As Writable In firstStep
				If TypeOf w Is NDArrayWritable Then
					size += DirectCast(w, NDArrayWritable).get().size(1)
				Else
					size += 1
				End If
			Next w


			arr = Nd4j.create(New Integer() {details.getMinValues(), size, details.getMaxTSLength()}, "f"c)

			Dim needMaskArray As Boolean = False
			For Each c As IList(Of IList(Of Writable)) In list
				If c.Count < details.getMaxTSLength() Then
					needMaskArray = True
					Exit For
				End If
			Next c


			Dim maskArray As INDArray
			If needMaskArray Then
				maskArray = Nd4j.ones(details.getMinValues(), details.getMaxTSLength())
			Else
				maskArray = Nothing
			End If



			Dim i As Integer = 0
			Do While i < details.getMinValues()
				Dim sequence As IList(Of IList(Of Writable)) = list(i)
				Dim t As Integer = 0
				Dim k As Integer
				For Each timeStep As IList(Of Writable) In sequence
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: k = t++;
					k = t
						t += 1

					'Convert entire reader contents, without modification
					Dim iter As IEnumerator(Of Writable) = timeStep.GetEnumerator()
					Dim j As Integer = 0
					Do While iter.MoveNext()
						Dim w As Writable = iter.Current

						If TypeOf w Is NDArrayWritable Then
							Dim row As INDArray = DirectCast(w, NDArrayWritable).get()

							arr.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(j, j + row.length()), NDArrayIndex.point(k)}, row)
							j += row.length()
						Else
							arr.putScalar(i, j, k, w.toDouble())
							j += 1
						End If
					Loop



				Next timeStep

				'For any remaining time steps: set mask array to 0 (just padding)
				If needMaskArray Then
					'Masking array entries at end (for align start)
					Dim lastStep As Integer = sequence.Count
					Dim t2 As Integer = lastStep
					Do While t2 < details.getMaxTSLength()
						maskArray.putScalar(i, t2, 0.0)
						t2 += 1
					Loop

				End If
				i += 1
			Loop

			Return New Pair(Of INDArray, INDArray)(arr, maskArray)
		End Function

	End Class

End Namespace