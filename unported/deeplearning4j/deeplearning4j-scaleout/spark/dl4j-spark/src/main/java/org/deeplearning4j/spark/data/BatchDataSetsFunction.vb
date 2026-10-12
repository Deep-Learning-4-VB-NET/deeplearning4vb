Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.data


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class BatchDataSetsFunction implements org.apache.spark.api.java.function.FlatMapFunction<java.util.Iterator<org.nd4j.linalg.dataset.DataSet>, org.nd4j.linalg.dataset.DataSet>
	Public Class BatchDataSetsFunction
		Implements FlatMapFunction(Of IEnumerator(Of DataSet), DataSet)

		Private ReadOnly minibatchSize As Integer

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<org.nd4j.linalg.dataset.DataSet> call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> iter) throws Exception
		Public Overrides Function [call](ByVal iter As IEnumerator(Of DataSet)) As IEnumerator(Of DataSet)
			Dim [out] As IList(Of DataSet) = New List(Of DataSet)()
			Do While iter.MoveNext()
				Dim list As IList(Of DataSet) = New List(Of DataSet)()

				Dim count As Integer = 0
				Do While count < minibatchSize AndAlso iter.MoveNext()
					Dim ds As DataSet = iter.Current
					count += ds.Features.size(0)
					list.Add(ds)
				Loop

				Dim [next] As DataSet
				If list.Count = 0 Then
					[next] = list(0)
				Else
					[next] = DataSet.merge(list)
				End If

				[out].Add([next])
			Loop
			Return [out].GetEnumerator()
		End Function
	End Class

End Namespace