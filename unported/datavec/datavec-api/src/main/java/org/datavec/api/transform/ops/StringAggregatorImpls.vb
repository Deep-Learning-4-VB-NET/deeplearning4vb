Imports System
Imports System.Text
Imports Getter = lombok.Getter
Imports Text = org.datavec.api.writable.Text
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

	Public Class StringAggregatorImpls

		<Serializable>
		Private MustInherit Class AggregableStringReduce
			Implements IAggregableReduceOp(Of String, Writable)

			Public MustOverride Sub combine(ByVal accu As W) Implements IAggregableReduceOp(Of String, Writable).combine
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected StringBuilder sb = new StringBuilder();
			Protected Friend sb As New StringBuilder()
		End Class

		<Serializable>
		Public Class AggregableStringAppend
			Inherits AggregableStringReduce

			Public Overrides Sub combine(Of W As IAggregableReduceOp(Of String, Writable))(ByVal accu As W)
				If TypeOf accu Is AggregableStringAppend Then
					sb.Append(CType(accu, AggregableStringAppend).getSb())
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where" & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Sub accept(ByVal s As String)
				sb.Append(s)
			End Sub

			Public Overridable Function get() As Writable
				Return New Text(sb.ToString())
			End Function
		End Class

		<Serializable>
		Public Class AggregableStringPrepend
			Inherits AggregableStringReduce

			Public Overrides Sub combine(Of W As IAggregableReduceOp(Of String, Writable))(ByVal accu As W)
				If TypeOf accu Is AggregableStringPrepend Then
					sb.Append(CType(accu, AggregableStringPrepend).getSb())
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where" & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Sub accept(ByVal s As String)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
				Dim rev As String = (New StringBuilder(s)).reverse().ToString()
				sb.Append(rev)
			End Sub

			Public Overridable Function get() As Writable
'JAVA TO VB CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
				Return New Text(sb.reverse().ToString())
			End Function
		End Class

	End Class

End Namespace