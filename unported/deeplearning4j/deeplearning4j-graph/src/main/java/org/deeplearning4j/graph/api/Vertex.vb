Imports AllArgsConstructor = lombok.AllArgsConstructor

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

Namespace org.deeplearning4j.graph.api

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class Vertex<T>
	Public Class Vertex(Of T)

		Private ReadOnly idx As Integer
'JAVA TO VB CONVERTER NOTE: The field value was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly value_Conflict As T

		Public Overridable Function vertexID() As Integer
			Return idx
		End Function

		Public Overridable ReadOnly Property Value As T
			Get
				Return value_Conflict
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "vertex(" & idx & "," & (If(value_Conflict IsNot Nothing, value_Conflict, "")) & ")"
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is Vertex) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Vertex<?> v = (Vertex<?>) o;
			Dim v As Vertex(Of Object) = DirectCast(o, Vertex(Of Object))
			If idx <> v.idx Then
				Return False
			End If
			If (value_Conflict Is Nothing AndAlso v.value_Conflict IsNot Nothing) OrElse (value_Conflict IsNot Nothing AndAlso v.value_Conflict Is Nothing) Then
				Return False
			End If
			Return value_Conflict Is Nothing OrElse value_Conflict.Equals(v.value_Conflict)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			result = 31 * result + idx
			result = 31 * result + (If(value_Conflict Is Nothing, 0, value_Conflict.GetHashCode()))
			Return result
		End Function
	End Class

End Namespace