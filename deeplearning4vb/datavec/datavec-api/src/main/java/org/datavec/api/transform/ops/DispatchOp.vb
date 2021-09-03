Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull

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
'ORIGINAL LINE: @AllArgsConstructor public class DispatchOp<T, U> implements IAggregableReduceOp<java.util.List<T>, java.util.List<U>>
	<Serializable>
	Public Class DispatchOp(Of T, U)
		Implements IAggregableReduceOp(Of IList(Of T), IList(Of U))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @NonNull private java.util.List<IAggregableReduceOp<T, java.util.List<U>>> operations;
		Private operations As IList(Of IAggregableReduceOp(Of T, IList(Of U)))

		Public Overridable Sub combine(Of W As IAggregableReduceOp(Of IList(Of T), IList(Of U)))(ByVal accu As W) Implements IAggregableReduceOp(Of IList(Of T), IList(Of U)).combine
			If TypeOf accu Is DispatchOp Then
				Dim otherOps As IList(Of IAggregableReduceOp(Of T, IList(Of U))) = CType(accu, DispatchOp(Of T, U)).getOperations()
				If operations.Count <> otherOps.Count Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.ArgumentException("Tried to combine() incompatible " & Me.GetType().FullName & " operators: received " & otherOps.Count & " operations, expected " & operations.Count)
				End If
				Dim i As Integer = 0
				Do While i < Math.Min(operations.Count, otherOps.Count)
					operations(i).combine(otherOps(i))
					i += 1
				Loop
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
			End If
		End Sub

		Public Overridable Sub accept(ByVal ts As IList(Of T))
			Dim i As Integer = 0
			Do While i < Math.Min(operations.Count, ts.Count)
				operations(i).accept(ts(i))
				i += 1
			Loop
		End Sub

		Public Overridable Function get() As IList(Of U)
			Dim res As IList(Of U) = New List(Of U)()
			For i As Integer = 0 To operations.Count - 1
				CType(res, List(Of U)).AddRange(operations(i).get())
			Next i
			Return res
		End Function
	End Class

End Namespace