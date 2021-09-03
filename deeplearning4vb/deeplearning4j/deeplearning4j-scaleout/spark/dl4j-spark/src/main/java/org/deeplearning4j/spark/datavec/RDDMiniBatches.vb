Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
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

Namespace org.deeplearning4j.spark.datavec


	<Serializable>
	Public Class RDDMiniBatches
		Private miniBatches As Integer
		Private toSplitJava As JavaRDD(Of DataSet)

		Public Sub New(ByVal miniBatches As Integer, ByVal toSplit As JavaRDD(Of DataSet))
			Me.miniBatches = miniBatches
			Me.toSplitJava = toSplit
		End Sub

		Public Overridable Function miniBatchesJava() As JavaRDD(Of DataSet)
			'need a new mapping function, doesn't handle mini batches properly
			Return toSplitJava.mapPartitions(New MiniBatchFunction(miniBatches))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public static class MiniBatchFunction implements org.apache.spark.api.java.function.FlatMapFunction<java.util.Iterator<org.nd4j.linalg.dataset.DataSet>, org.nd4j.linalg.dataset.DataSet>
		Public Class MiniBatchFunction
			Implements FlatMapFunction(Of IEnumerator(Of DataSet), DataSet)

			Friend batchSize As Integer

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<org.nd4j.linalg.dataset.DataSet> call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> dataSetIterator) throws Exception
			Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of DataSet)) As IEnumerator(Of DataSet)
				Dim ret As IList(Of DataSet) = New List(Of DataSet)()
				Dim temp As IList(Of DataSet) = New List(Of DataSet)()
				Do While dataSetIterator.MoveNext()
					temp.Add(dataSetIterator.Current.copy())
					If temp.Count = batchSize Then
						ret.Add(DataSet.merge(temp))
						temp.Clear()
					End If
				Loop

				'Add remaining ('left over') data
				If temp.Count > 0 Then
					ret.Add(DataSet.merge(temp))
				End If

				Return ret.GetEnumerator()
			End Function
		End Class
	End Class

End Namespace