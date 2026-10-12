Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.deeplearning4j.text.movingwindow



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class WindowConverter
	Public Class WindowConverter


		Private Sub New()
		End Sub

		''' <summary>
		''' Converts a window (each word in the window)
		''' 
		''' in to a vector.
		''' 
		''' Keep in mind each window is a multi word context.
		''' 
		''' From there, each word uses the passed in model
		''' as a lookup table to get what vectors are relevant
		''' to the passed in windows </summary>
		''' <param name="window"> the window to take in. </param>
		''' <param name="vec"> the model to use as a lookup table </param>
		''' <returns> a concacneated 1 row array
		''' containing all of the numbers for each word in the window </returns>
		Public Shared Function asExampleArray(ByVal window As Window, ByVal vec As Word2Vec, ByVal normalize As Boolean) As INDArray
			Dim length As Integer = vec.lookupTable().layerSize()
			Dim words As IList(Of String) = window.getWords()
			Dim windowSize As Integer = vec.getWindow()
			Preconditions.checkState(words.Count = vec.getWindow())
			Dim ret As INDArray = Nd4j.create(1, length * windowSize)



			For i As Integer = 0 To words.Count - 1
				Dim word As String = words(i)
				Dim n As INDArray = If(normalize, vec.getWordVectorMatrixNormalized(word), vec.getWordVectorMatrix(word))
				ret.put(New INDArrayIndex() {NDArrayIndex.interval(i * vec.lookupTable().layerSize(), i * vec.lookupTable().layerSize() + vec.lookupTable().layerSize())}, n)
			Next i

			Return ret
		End Function



		''' <summary>
		''' Converts a window (each word in the window)
		''' 
		''' in to a vector.
		''' 
		''' Keep in mind each window is a multi word context.
		''' 
		''' From there, each word uses the passed in model
		''' as a lookup table to get what vectors are relevant
		''' to the passed in windows </summary>
		''' <param name="window"> the window to take in. </param>
		''' <param name="vec"> the model to use as a lookup table </param>
		''' <returns> a concatneated 1 row array
		''' containing all of the numbers for each word in the window </returns>
		Public Shared Function asExampleMatrix(ByVal window As Window, ByVal vec As Word2Vec) As INDArray
			Dim data((window.getWords().Count) - 1) As INDArray
			For i As Integer = 0 To data.Length - 1
				data(i) = vec.getWordVectorMatrix(window.getWord(i))

				' if there's null elements
				If data(i) Is Nothing Then
					data(i) = Nd4j.zeros(1, vec.LayerSize)
				End If
			Next i
			Return Nd4j.hstack(data)
		End Function

	End Class

End Namespace