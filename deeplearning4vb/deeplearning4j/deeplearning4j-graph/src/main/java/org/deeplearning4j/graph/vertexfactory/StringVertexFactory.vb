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

Namespace org.deeplearning4j.graph.vertexfactory

	Public Class StringVertexFactory
		Implements VertexFactory(Of String)

		Private ReadOnly format As String

		Public Sub New()
			Me.New(Nothing)
		End Sub

		Public Sub New(ByVal format As String)
			Me.format = format
		End Sub

		Public Overridable Function create(ByVal vertexIdx As Integer) As Vertex(Of String) Implements VertexFactory(Of String).create
			If format IsNot Nothing Then
				Return New Vertex(Of String)(vertexIdx, String.format(format, vertexIdx))
			Else
				Return New Vertex(Of String)(vertexIdx, vertexIdx.ToString())
			End If
		End Function
	End Class

End Namespace