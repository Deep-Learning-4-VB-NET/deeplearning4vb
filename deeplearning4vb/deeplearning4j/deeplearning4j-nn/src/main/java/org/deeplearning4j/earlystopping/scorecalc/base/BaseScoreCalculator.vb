Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.earlystopping.scorecalc
Imports Model = org.deeplearning4j.nn.api.Model
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.earlystopping.scorecalc.base

	Public MustInherit Class BaseScoreCalculator(Of T As org.deeplearning4j.nn.api.Model)
		Implements ScoreCalculator(Of T)

		Public MustOverride Function minimizeScore() As Boolean Implements ScoreCalculator(Of T).minimizeScore

		Protected Friend mdsIterator As MultiDataSetIterator
		Protected Friend iterator As DataSetIterator
		Protected Friend scoreSum As Double
		Protected Friend minibatchCount As Integer
		Protected Friend exampleCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseScoreCalculator(@NonNull DataSetIterator iterator)
		Protected Friend Sub New(ByVal iterator As DataSetIterator)
			Me.iterator = iterator
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseScoreCalculator(@NonNull MultiDataSetIterator iterator)
		Protected Friend Sub New(ByVal iterator As MultiDataSetIterator)
			Me.mdsIterator = iterator
		End Sub

		Public Overridable Function calculateScore(ByVal network As T) As Double Implements ScoreCalculator(Of T).calculateScore
			reset()

			If iterator IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not iterator.hasNext() Then
					iterator.reset()
				End If

				Do While iterator.MoveNext()
					Dim ds As DataSet = iterator.Current
					Dim [out] As INDArray = output(network, ds.Features, ds.FeaturesMaskArray, ds.LabelsMaskArray)
					scoreSum += scoreMinibatch(network, ds.Features, ds.Labels, ds.FeaturesMaskArray, ds.LabelsMaskArray, [out])
					minibatchCount += 1
					exampleCount += ds.Features.size(0)
				Loop
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not mdsIterator.hasNext() Then
					mdsIterator.reset()
				End If

				Do While mdsIterator.MoveNext()
					Dim mds As MultiDataSet = mdsIterator.Current
					Dim [out]() As INDArray = output(network, mds.Features, mds.FeaturesMaskArrays, mds.LabelsMaskArrays)
					scoreSum += scoreMinibatch(network, mds.Features, mds.Labels, mds.FeaturesMaskArrays, mds.LabelsMaskArrays, [out])
					minibatchCount += 1
					exampleCount += mds.getFeatures(0).size(0)
				Loop
			End If

			Return finalScore(scoreSum, minibatchCount, exampleCount)
		End Function

		Protected Friend MustOverride Sub reset()

		Protected Friend MustOverride Function output(ByVal network As T, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As INDArray

		Protected Friend MustOverride Function output(ByVal network As T, ByVal input() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray) As INDArray()

		Protected Friend Overridable Function scoreMinibatch(ByVal network As T, ByVal features As INDArray, ByVal labels As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray, ByVal output As INDArray) As Double
			Return scoreMinibatch(network, arr(features), arr(labels), arr(fMask), arr(lMask), arr(output))
		End Function

		Protected Friend MustOverride Function scoreMinibatch(ByVal network As T, ByVal features() As INDArray, ByVal labels() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal output() As INDArray) As Double

		Protected Friend MustOverride Function finalScore(ByVal scoreSum As Double, ByVal minibatchCount As Integer, ByVal exampleCount As Integer) As Double

		Public Shared Function arr(ByVal [in] As INDArray) As INDArray()
			If [in] Is Nothing Then
				Return Nothing
			End If
			Return New INDArray(){[in]}
		End Function

		Public Shared Function get0(ByVal [in]() As INDArray) As INDArray
			If [in] Is Nothing Then
				Return Nothing
			End If
			If [in].Length <> 1 Then
				Throw New System.InvalidOperationException("Expected length 1 array here: got length " & [in].Length)
			End If
			Return [in](0)
		End Function
	End Class

End Namespace