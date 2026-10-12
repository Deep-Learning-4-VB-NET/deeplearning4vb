Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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
'ORIGINAL LINE: @AllArgsConstructor public class Vertex<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement>
	Public Class Vertex(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)

		Private ReadOnly idx As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private T value;
		Private value As T

		Public Overridable Function vertexID() As Integer
			Return idx
		End Function


		Public Overrides Function ToString() As String
			Return "vertex(" & idx & "," & (If(value IsNot Nothing, value, "")) & ")"
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
			If (value Is Nothing AndAlso v.value IsNot Nothing) OrElse (value IsNot Nothing AndAlso v.value Is Nothing) Then
				Return False
			End If
			Return value Is Nothing OrElse value.Equals(v.value)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			result = 31 * result + idx
			result = 31 * result + (If(value Is Nothing, 0, value.GetHashCode()))
			Return result
		End Function
	End Class

End Namespace