Imports System
Imports IrisDataFetcher = org.deeplearning4j.datasets.fetchers.IrisDataFetcher
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports BaseDatasetIterator = org.nd4j.linalg.dataset.api.iterator.BaseDatasetIterator

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

Namespace org.deeplearning4j.datasets.iterator.impl

	<Serializable>
	Public Class IrisDataSetIterator
		Inherits BaseDatasetIterator

		''' 
		Private Const serialVersionUID As Long = -2022454995728680368L

		''' <summary>
		''' Create an iris iterator for full batch training - i.e., all 150 examples are included per minibatch
		''' </summary>
		Public Sub New()
			Me.New(150, 150)
		End Sub

		''' <summary>
		''' IrisDataSetIterator handles traversing through the Iris Data Set. </summary>
		''' <seealso cref= <a href="https://archive.ics.uci.edu/ml/datasets/Iris">https://archive.ics.uci.edu/ml/datasets/Iris</a>
		''' </seealso>
		''' <param name="batch"> Batch size </param>
		''' <param name="numExamples"> Total number of examples </param>
		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer)
			MyBase.New(batch, numExamples, New IrisDataFetcher())
		End Sub


		Public Overrides Function [next]() As DataSet
			fetcher.fetch(batch_Conflict)
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As DataSet = fetcher.next()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(next_Conflict)
			End If

			Return next_Conflict
		End Function
	End Class

End Namespace