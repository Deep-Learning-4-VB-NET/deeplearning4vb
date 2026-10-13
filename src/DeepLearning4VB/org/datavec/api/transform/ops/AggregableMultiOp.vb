Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.ops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class AggregableMultiOp<T> implements IAggregableReduceOp<T, java.util.List<org.datavec.api.writable.Writable>>
	<Serializable>
	Public Class AggregableMultiOp(Of T)
		Implements IAggregableReduceOp(Of T, IList(Of Writable))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @NonNull private java.util.List<IAggregableReduceOp<T, org.datavec.api.writable.Writable>> operations;
		Private operations As IList(Of IAggregableReduceOp(Of T, Writable))

		Public Overridable Sub accept(ByVal t As T)
			Dim i As Integer = 0
			Do While i < operations.Count
				operations(i).accept(t)
				i += 1
			Loop
		End Sub

		Public Overridable Sub combine(Of U As IAggregableReduceOp(Of T, IList(Of Writable)))(ByVal accu As U)
			If TypeOf accu Is AggregableMultiOp Then
				Dim accumulator As AggregableMultiOp(Of T) = CType(accu, AggregableMultiOp(Of T))
				Dim otherAccumulators As IList(Of IAggregableReduceOp(Of T, Writable)) = accumulator.getOperations()
				If operations.Count <> otherAccumulators.Count Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.ArgumentException("Tried to combine() incompatible " & Me.GetType().FullName & " operators: received " & otherAccumulators.Count & " operations, expected " & operations.Count)
				End If
				Dim i As Integer = 0
				Do While i < operations.Count
					operations(i).combine(otherAccumulators(i))
					i += 1
				Loop
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
			End If
		End Sub

		Public Overridable Function get() As IList(Of Writable)
			Dim res As IList(Of Writable) = New List(Of Writable)(operations.Count)
			For i As Integer = 0 To operations.Count - 1
				res.Add(operations(i).get())
			Next i
			Return res
		End Function

	End Class

End Namespace