Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Repartition = org.deeplearning4j.spark.api.Repartition
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

Namespace org.deeplearning4j.spark.impl.common


	Public Class CountPartitionsFunction(Of T)
		Implements Function2(Of Integer, IEnumerator(Of T), IEnumerator(Of Tuple2(Of Integer, Integer)))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<Integer, Integer>> call(System.Nullable<Integer> v1, java.util.Iterator<T> v2) throws Exception
		Public Overrides Function [call](ByVal v1 As Integer?, ByVal v2 As IEnumerator(Of T)) As IEnumerator(Of Tuple2(Of Integer, Integer))

			Dim count As Integer = 0
			Do While v2.MoveNext()
				v2.Current
				count += 1
			Loop

			Return Collections.singletonList(New Tuple2(Of )(v1, count)).GetEnumerator()
		End Function
	End Class

End Namespace