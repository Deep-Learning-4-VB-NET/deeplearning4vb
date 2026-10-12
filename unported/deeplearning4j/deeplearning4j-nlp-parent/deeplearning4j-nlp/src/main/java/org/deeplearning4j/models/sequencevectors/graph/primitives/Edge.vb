Imports Data = lombok.Data

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

Namespace org.deeplearning4j.models.sequencevectors.graph.primitives

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Edge<T extends Number>
	Public Class Edge(Of T As Number)

		Private ReadOnly from As Integer
		Private ReadOnly [to] As Integer
		Private ReadOnly value As T
		Private ReadOnly directed As Boolean

		Public Sub New(ByVal from As Integer, ByVal [to] As Integer, ByVal value As T, ByVal directed As Boolean)
			Me.from = from
			Me.to = [to]
			Me.value = value
			Me.directed = directed
		End Sub

		Public Overrides Function ToString() As String
			Return "edge(" & (If(directed, "directed", "undirected")) & "," & from + (If(directed, "->", "--")) + [to] & "," & (If(value IsNot Nothing, value, "")) & ")"
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is Edge) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Edge<?> e = (Edge<?>) o;
			Dim e As Edge(Of Object) = DirectCast(o, Edge(Of Object))
			If directed <> e.directed Then
				Return False
			End If
			If directed Then
				If from <> e.from Then
					Return False
				End If
				If [to] <> e.to Then
					Return False
				End If
			Else
				If from = e.from Then
					If [to] <> e.to Then
						Return False
					End If
				Else
					If from <> e.to Then
						Return False
					End If
					If [to] <> e.from Then
						Return False
					End If
				End If
			End If
			If (value IsNot Nothing AndAlso e.value Is Nothing) OrElse (value Is Nothing AndAlso e.value IsNot Nothing) Then
				Return False
			End If
			Return value Is Nothing OrElse value.Equals(e.value)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			result = 31 * result + (If(directed, 1, 0))
			result = 31 * result + from
			result = 31 * result + [to]
			result = 31 * result + (If(value Is Nothing, 0, value.GetHashCode()))
			Return result
		End Function
	End Class

End Namespace