Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.models.embeddings.inmemory
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.spark.models.embeddings.word2vec


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	<Obsolete, Serializable>
	Public Class Word2VecChange
		Private changes As IDictionary(Of Integer, ISet(Of INDArray)) = New Dictionary(Of Integer, ISet(Of INDArray))()

		Public Sub New(ByVal counterMap As IList(Of Triple(Of Integer, Integer, Integer)), ByVal param As Word2VecParam)
			Dim iter As IEnumerator(Of Triple(Of Integer, Integer, Integer)) = counterMap.GetEnumerator()
			Do While iter.MoveNext()
				Dim [next] As Triple(Of Integer, Integer, Integer) = iter.Current
				Dim point As Integer? = [next].getFirst()
				Dim index As Integer? = [next].getSecond()

				Dim changes As ISet(Of INDArray) = Me.changes(point)
				If changes Is Nothing Then
					changes = New HashSet(Of INDArray)()
					Me.changes(point) = changes
				End If

				changes.Add(param.Weights.getSyn1().slice(index))

			Loop
		End Sub

		''' <summary>
		''' Take the changes and apply them
		''' to the given table </summary>
		''' <param name="table"> the memory lookup table
		'''              to apply the changes to </param>
		Public Overridable Sub apply(ByVal table As InMemoryLookupTable)
			For Each entry As KeyValuePair(Of Integer, ISet(Of INDArray)) In changes.SetOfKeyValuePairs()
				Dim changes As ISet(Of INDArray) = entry.Value
				Dim toChange As INDArray = table.getSyn0().slice(entry.Key)
				For Each syn1 As INDArray In changes
					Nd4j.BlasWrapper.level1().axpy(toChange.length(), 1, syn1, toChange)
				Next syn1
			Next entry
		End Sub
	End Class

End Namespace