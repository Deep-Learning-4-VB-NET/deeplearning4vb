﻿Imports System.Collections.Generic
Imports PairFlatMapFunction = org.apache.spark.api.java.function.PairFlatMapFunction
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

Namespace org.deeplearning4j.spark.impl.common.repartition


	Public Class MapTupleToPairFlatMap(Of T, U)
		Implements PairFlatMapFunction(Of IEnumerator(Of Tuple2(Of T, U)), T, U)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<T, U>> call(java.util.Iterator<scala.Tuple2<T, U>> tuple2Iterator) throws Exception
		Public Overrides Function [call](ByVal tuple2Iterator As IEnumerator(Of Tuple2(Of T, U))) As IEnumerator(Of Tuple2(Of T, U))
			Dim list As IList(Of Tuple2(Of T, U)) = New List(Of Tuple2(Of T, U))()
			Do While tuple2Iterator.MoveNext()
				list.Add(tuple2Iterator.Current)
			Loop
			Return list.GetEnumerator()
		End Function
	End Class

End Namespace