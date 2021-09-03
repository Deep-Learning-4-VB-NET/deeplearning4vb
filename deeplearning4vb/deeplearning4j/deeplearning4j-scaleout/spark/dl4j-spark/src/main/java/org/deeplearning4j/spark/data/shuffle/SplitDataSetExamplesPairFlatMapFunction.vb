Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports PairFlatMapFunction = org.apache.spark.api.java.function.PairFlatMapFunction
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.data.shuffle


	Public Class SplitDataSetExamplesPairFlatMapFunction
		Implements PairFlatMapFunction(Of DataSet, Integer, DataSet)

		<NonSerialized>
		Private r As Random
		Private maxKeyIndex As Integer

		Public Sub New(ByVal maxKeyIndex As Integer)
			Me.maxKeyIndex = maxKeyIndex
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<Integer, org.nd4j.linalg.dataset.DataSet>> call(org.nd4j.linalg.dataset.DataSet dataSet) throws Exception
		Public Overrides Function [call](ByVal dataSet As DataSet) As IEnumerator(Of Tuple2(Of Integer, DataSet))
			If r Is Nothing Then
				r = New Random()
			End If

			Dim singleExamples As IList(Of DataSet) = dataSet.asList()
			Dim [out] As IList(Of Tuple2(Of Integer, DataSet)) = New List(Of Tuple2(Of Integer, DataSet))(singleExamples.Count)
			For Each ds As DataSet In singleExamples
				[out].Add(New Tuple2(Of Integer, DataSet)(r.Next(maxKeyIndex), ds))
			Next ds

			Return [out].GetEnumerator()
		End Function
	End Class

End Namespace