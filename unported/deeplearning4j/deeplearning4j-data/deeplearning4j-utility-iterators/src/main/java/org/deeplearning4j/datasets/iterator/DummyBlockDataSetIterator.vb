Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports BlockDataSetIterator = org.nd4j.linalg.dataset.api.iterator.BlockDataSetIterator
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

Namespace org.deeplearning4j.datasets.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DummyBlockDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.BlockDataSetIterator
	Public Class DummyBlockDataSetIterator
		Implements BlockDataSetIterator

		Protected Friend ReadOnly iterator As DataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DummyBlockDataSetIterator(@NonNull DataSetIterator iterator)
		Public Sub New(ByVal iterator As DataSetIterator)
			Me.iterator = iterator
		End Sub

		Public Overridable Function hasAnything() As Boolean Implements BlockDataSetIterator.hasAnything
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterator.hasNext()
		End Function

		Public Overridable Function [next](ByVal maxDatasets As Integer) As DataSet() Implements BlockDataSetIterator.next
			Dim list As val = New List(Of DataSet)(maxDatasets)
			Dim cnt As Integer = 0
			Do While iterator.MoveNext() AndAlso cnt < maxDatasets
				list.add(iterator.Current)
				cnt += 1
			Loop

			Return list.toArray(New DataSet(list.size() - 1){})
		End Function
	End Class

End Namespace