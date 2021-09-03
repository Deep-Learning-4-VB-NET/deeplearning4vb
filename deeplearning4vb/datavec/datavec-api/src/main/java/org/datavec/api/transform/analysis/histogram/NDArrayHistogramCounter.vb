Imports System
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
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

Namespace org.datavec.api.transform.analysis.histogram

	<Serializable>
	Public Class NDArrayHistogramCounter
		Implements HistogramCounter

		Private underlying As DoubleHistogramCounter

		Public Sub New(ByVal minValue As Double, ByVal maxValue As Double, ByVal nBins As Integer)
			Me.underlying = New DoubleHistogramCounter(minValue, maxValue, nBins)
		End Sub


		Public Overridable Function add(ByVal w As Writable) As HistogramCounter Implements HistogramCounter.add
			Dim arr As INDArray = DirectCast(w, NDArrayWritable).get()
			If arr Is Nothing Then
				Return Me
			End If

			Dim length As Long = arr.length()
			Dim dw As New DoubleWritable()
			For i As Integer = 0 To length - 1
				dw.set(arr.getDouble(i))
				underlying.add(dw)
			Next i

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As HistogramCounter) As NDArrayHistogramCounter
			If other Is Nothing Then
				Return Me
			End If
			If Not (TypeOf other Is NDArrayHistogramCounter) Then
				Throw New System.ArgumentException("Cannot merge " & other.GetType())
			End If

			Dim o As NDArrayHistogramCounter = DirectCast(other, NDArrayHistogramCounter)

			If Me.underlying Is Nothing Then
				Me.underlying = o.underlying
			Else
				If o.underlying Is Nothing Then
					Return Me
				End If
				Me.underlying.merge(o.underlying)
			End If

			Return Me
		End Function

		Public Overridable ReadOnly Property Bins As Double() Implements HistogramCounter.getBins
			Get
				Return underlying.Bins
			End Get
		End Property

		Public Overridable ReadOnly Property Counts As Long() Implements HistogramCounter.getCounts
			Get
				Return underlying.Counts
			End Get
		End Property
	End Class

End Namespace