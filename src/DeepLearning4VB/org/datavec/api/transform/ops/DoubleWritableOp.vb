Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports Getter = lombok.Getter
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
'ORIGINAL LINE: @AllArgsConstructor @Data public class DoubleWritableOp<T> implements IAggregableReduceOp<org.datavec.api.writable.Writable, T>
	<Serializable>
	Public Class DoubleWritableOp(Of T)
		Implements IAggregableReduceOp(Of Writable, T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private IAggregableReduceOp<Double, T> operation;
		Private operation As IAggregableReduceOp(Of Double, T)

		Public Overridable Sub combine(Of W As IAggregableReduceOp(Of Writable, T))(ByVal accu As W) Implements IAggregableReduceOp(Of Writable, T).combine
			If TypeOf accu Is DoubleWritableOp Then
				operation.combine(CType(accu, DoubleWritableOp).getOperation())
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
			End If
		End Sub

		Public Overridable Sub accept(ByVal writable As Writable)
			operation.accept(writable.toDouble())
		End Sub

		Public Overridable Function get() As T
			Return operation.get()
		End Function
	End Class

End Namespace