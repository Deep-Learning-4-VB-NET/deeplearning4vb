Imports System
Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.iterator


	<Serializable>
	Public Class KFoldIterator
		Implements DataSetIterator

		Private Const serialVersionUID As Long = 6130298603412865817L

		Protected Friend allData As DataSet
		Protected Friend k As Integer
		Protected Friend N As Integer
		Protected Friend intervalBoundaries() As Integer
		Protected Friend kCursor As Integer = 0
		Protected Friend test As DataSet
		Protected Friend train As DataSet
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor

		''' <summary>
		''' Create a k-fold cross-validation iterator given the dataset and k=10 train-test splits.
		''' N number of samples are split into k batches. The first (N%k) batches contain (N/k)+1 samples, while the remaining batches contain (N/k) samples. 
		''' In case the number of samples (N) in the dataset is a multiple of k, all batches will contain (N/k) samples. </summary>
		''' <param name="allData"> DataSet to split into k folds </param>
		Public Sub New(ByVal allData As DataSet)
			Me.New(10, allData)
		End Sub

		''' <summary>
		''' Create an iterator given the dataset with given k train-test splits
		''' N number of samples are split into k batches. The first (N%k) batches contain (N/k)+1 samples, while the remaining batches contain (N/k) samples.
		''' In case the number of samples (N) in the dataset is a multiple of k, all batches will contain (N/k) samples. </summary>
		''' <param name="k"> number of folds (optional, defaults to 10) </param>
		''' <param name="allData"> DataSet to split into k folds </param>
		Public Sub New(ByVal k As Integer, ByVal allData As DataSet)
			If k <= 1 Then
				Throw New System.ArgumentException()
			End If
			Me.k = k
			Me.N = allData.numExamples()
			Me.allData = allData

			' generate index interval boundaries of test folds
			Dim baseBatchSize As Integer = N \ k
			Dim numIncrementedBatches As Integer = N Mod k

			Me.intervalBoundaries = New Integer(k){}
			intervalBoundaries(0) = 0
			For i As Integer = 1 To k
				If i <= numIncrementedBatches Then
					intervalBoundaries(i) = intervalBoundaries(i-1) + (baseBatchSize + 1)
				Else
					intervalBoundaries(i) = intervalBoundaries(i-1) + baseBatchSize
				End If
			Next i

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet next(int num) throws UnsupportedOperationException
		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Return Nothing
		End Function

		''' <summary>
		''' Returns total number of examples in the dataset (all k folds)
		''' </summary>
		''' <returns> total number of examples in the dataset including all k folds </returns>
		Public Overridable Function totalExamples() As Integer
			Return N
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return CInt(allData.Features.size(1))
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return CInt(allData.Labels.size(1))
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return False
		End Function

		''' <summary>
		''' Shuffles the dataset and resets to the first fold
		''' </summary>
		''' <returns> void </returns>
		Public Overridable Sub reset() Implements DataSetIterator.reset
			'shuffle and return new k folds
			allData.shuffle()
			kCursor = 0
		End Sub


		''' <summary>
		''' The number of examples in every fold is (N / k), 
		''' except when (N % k) > 0, when the first (N % k) folds contain (N / k) + 1 examples  
		''' </summary>
		''' <returns> examples in a fold </returns>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return intervalBoundaries(kCursor+1) - intervalBoundaries(kCursor)
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return allData.getLabelNamesList()
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return kCursor < k
		End Function

		Public Overrides Function [next]() As DataSet
			nextFold()
			Return train
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Protected Friend Overridable Sub nextFold()
			Dim left As Integer = intervalBoundaries(kCursor)
			Dim right As Integer = intervalBoundaries(kCursor + 1)

			Dim kMinusOneFoldList As IList(Of DataSet) = New List(Of DataSet)()
			If right < totalExamples() Then
				If left > 0 Then
					kMinusOneFoldList.Add(DirectCast(allData.getRange(0, left), DataSet))
				End If
				kMinusOneFoldList.Add(DirectCast(allData.getRange(right, totalExamples()), DataSet))
				train = DataSet.merge(kMinusOneFoldList)
			Else
				train = DirectCast(allData.getRange(0, left), DataSet)
			End If
			test = DirectCast(allData.getRange(left, right), DataSet)

			kCursor += 1

		End Sub

		''' <returns> the held out fold as a dataset </returns>
		Public Overridable Function testFold() As DataSet
			Return test
		End Function
	End Class

End Namespace