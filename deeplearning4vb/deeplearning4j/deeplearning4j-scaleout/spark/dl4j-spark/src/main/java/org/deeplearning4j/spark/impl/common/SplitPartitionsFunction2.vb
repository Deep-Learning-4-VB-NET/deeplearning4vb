﻿Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Function2 = org.apache.spark.api.java.function.Function2
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class SplitPartitionsFunction2<T, U> implements org.apache.spark.api.java.function.Function2<Integer, Iterator<scala.Tuple2<T, U>>, Iterator<scala.Tuple2<T, U>>>
	Public Class SplitPartitionsFunction2(Of T, U)
		Implements Function2(Of Integer, IEnumerator(Of Tuple2(Of T, U)), IEnumerator(Of Tuple2(Of T, U)))

		Private ReadOnly splitIndex As Integer
		Private ReadOnly numSplits As Integer
		Private ReadOnly baseRngSeed As Long

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Iterator<scala.Tuple2<T, U>> call(System.Nullable<Integer> v1, Iterator<scala.Tuple2<T, U>> iter) throws Exception
		Public Overrides Function [call](ByVal v1 As Integer?, ByVal iter As IEnumerator(Of Tuple2(Of T, U))) As IEnumerator(Of Tuple2(Of T, U))
			Dim thisRngSeed As Long = baseRngSeed + v1.Value

			Dim r As New Random(CInt(thisRngSeed))
			Dim list As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To numSplits - 1
				list.Add(i)
			Next i

			Dim outputList As IList(Of Tuple2(Of T, U)) = New List(Of Tuple2(Of T, U))()
			Dim i As Integer = 0
			Do While iter.MoveNext()
				If i Mod numSplits = 0 Then
					Collections.shuffle(list, r)
				End If

				Dim [next] As Tuple2(Of T, U) = iter.Current
				If list(i Mod numSplits) = splitIndex Then
					outputList.Add([next])
				End If
				i += 1
			Loop

			Return outputList.GetEnumerator()
		End Function
	End Class

End Namespace