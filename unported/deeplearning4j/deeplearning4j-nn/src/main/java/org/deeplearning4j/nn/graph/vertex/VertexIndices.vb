Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.deeplearning4j.nn.graph.vertex


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @EqualsAndHashCode public class VertexIndices implements java.io.Serializable
	<Serializable>
	Public Class VertexIndices

'JAVA TO VB CONVERTER NOTE: The field vertexIndex was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly vertexIndex_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field vertexEdgeNumber was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly vertexEdgeNumber_Conflict As Integer


		''' <summary>
		'''Index of the vertex </summary>
		Public Overridable ReadOnly Property VertexIndex As Integer
			Get
				Return Me.vertexIndex_Conflict
			End Get
		End Property

		''' <summary>
		''' The edge number. Represents the index of the output of the vertex index, OR the index of the
		''' input to the vertex, depending on the context
		''' </summary>
		Public Overridable ReadOnly Property VertexEdgeNumber As Integer
			Get
				Return Me.vertexEdgeNumber_Conflict
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "VertexIndices(vertexIndex=" & Me.vertexIndex_Conflict & ", vertexEdgeNumber=" & Me.vertexEdgeNumber_Conflict & ")"
		End Function
	End Class

End Namespace