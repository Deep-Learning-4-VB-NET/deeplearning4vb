Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api

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

Namespace org.deeplearning4j.graph.graph


	Public Class VertexSequence(Of V)
		Implements IVertexSequence(Of V)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private final org.deeplearning4j.graph.api.IGraph<V, ?> graph;
		Private ReadOnly graph As IGraph(Of V, Object)
		Private indices() As Integer
		Private currIdx As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public VertexSequence(org.deeplearning4j.graph.api.IGraph<V, ?> graph, int[] indices)
		Public Sub New(ByVal graph As IGraph(Of T1), ByVal indices() As Integer)
			Me.graph = graph
			Me.indices = indices
		End Sub

		Public Overridable Function sequenceLength() As Integer Implements IVertexSequence(Of V).sequenceLength
			Return indices.Length
		End Function

		Public Overrides Function hasNext() As Boolean
			Return currIdx < indices.Length
		End Function

		Public Overrides Function [next]() As Vertex(Of V)
			If Not hasNext() Then
				Throw New NoSuchElementException()
			End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return graph.getVertex(indices[currIdx++]);
			Dim tempVar = graph.getVertex(indices(currIdx))
				currIdx += 1
				Return tempVar
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace