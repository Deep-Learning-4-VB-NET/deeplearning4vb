Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports JsonIgnore = org.nd4j.shade.jackson.annotation.JsonIgnore
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.ui.model.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class HistogramBin implements java.io.Serializable
	<Serializable>
	Public Class HistogramBin
		<NonSerialized>
		Private sourceArray As INDArray
		Private numberOfBins As Integer
		Private rounds As Integer
		<NonSerialized>
		Private bins As INDArray
		Private max As Double
		Private min As Double
		Private data As IDictionary(Of Decimal, AtomicInteger) = New LinkedHashMap(Of Decimal, AtomicInteger)()

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(HistogramBin))

		''' <summary>
		''' No-Args constructor should be used only for serialization/deserialization purposes.
		''' In all other cases please use Histogram.Builder()
		''' </summary>
		Public Sub New()

		End Sub

		''' <summary>
		''' Builds histogram bin for specified array </summary>
		''' <param name="array"> </param>
		Public Sub New(ByVal array As INDArray)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private synchronized void calcHistogram()
		Private Sub calcHistogram()
			SyncLock Me
				max = sourceArray.maxNumber().doubleValue()
				min = sourceArray.minNumber().doubleValue()
        
				' TODO: there's probably better way to get around of possible NaNs in max/min
				If Double.IsInfinity(max) Then
					max = Single.MaxValue
				End If
        
				If Double.IsNaN(max) Then
					max = Single.Epsilon
				End If
        
				If Double.IsInfinity(min) Then
					min = Single.MaxValue
				End If
        
				If Double.IsNaN(min) Then
					min = Single.Epsilon
				End If
        
				bins = Nd4j.create(numberOfBins)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double binSize = (max - min) / (numberOfBins - 1);
				Dim binSize As Double = (max - min) / (numberOfBins - 1)
        
        
				data = New LinkedHashMap(Of Decimal, AtomicInteger)()
				Dim keys(numberOfBins - 1) As Decimal
        
				For x As Integer = 0 To numberOfBins - 1
					Dim pos As Decimal = (New Decimal((min + (x * binSize)))).setScale(rounds, Decimal.ROUND_CEILING)
					data(pos) = New AtomicInteger(0)
					keys(x) = pos
				Next x
        
				For x As Integer = 0 To sourceArray.length() - 1
					Dim d As Double = sourceArray.getDouble(x)
					Dim bin As Integer = CInt(Math.Truncate((d - min) / binSize))
        
					If bin < 0 Then
						bins.putScalar(0, bins.getDouble(0) + 1)
						data(keys(0)).incrementAndGet()
					ElseIf bin >= numberOfBins Then
						bins.putScalar(numberOfBins - 1, bins.getDouble(numberOfBins - 1) + 1)
						data(keys(numberOfBins - 1)).incrementAndGet()
					Else
						bins.putScalar(bin, bins.getDouble(bin) + 1)
						data(keys(bin)).incrementAndGet()
					End If
				Next x
			End SyncLock
		End Sub

		Public Class Builder
			Friend source As INDArray
'JAVA TO VB CONVERTER NOTE: The field binCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend binCount_Conflict As Integer
			Friend rounds As Integer = 2

			''' <summary>
			''' Build Histogram Builder instance for specified array </summary>
			''' <param name="array"> </param>
			Public Sub New(ByVal array As INDArray)
				Me.source = array
			End Sub

			''' <summary>
			''' Sets number of numbers behind decimal part
			''' </summary>
			''' <param name="rounds">
			''' @return </param>
			Public Overridable Function setRounding(ByVal rounds As Integer) As Builder
				Me.rounds = rounds
				Return Me
			End Function

			''' <summary>
			''' Specifies number of bins for output histogram
			''' </summary>
			''' <param name="bins">
			''' @return </param>
			Public Overridable Function setBinCount(ByVal bins As Integer) As Builder
				Me.binCount_Conflict = bins
				Return Me
			End Function

			''' <summary>
			''' Returns ready-to-use Histogram instance
			''' @return
			''' </summary>
			Public Overridable Function build() As HistogramBin
				Dim histogram As New HistogramBin()
				histogram.sourceArray = Me.source
				histogram.numberOfBins = Me.binCount_Conflict
				histogram.rounds = Me.rounds

				histogram.calcHistogram()

				Return histogram
			End Function
		End Class
	End Class

End Namespace