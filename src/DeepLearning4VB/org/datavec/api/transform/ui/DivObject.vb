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

Imports System

Namespace org.datavec.api.transform.ui

    ''' <summary>
    ''' One block of a report page: an element id and its content. In Java the content was the JSON of a
    ''' <c>RenderableComponent</c>, handed to the FreeMarker template and drawn by JavaScript; in this port the reports
    ''' render on the .NET side, so the content is the finished HTML of the block (see <see cref="HtmlAnalysis"/>).
    ''' </summary>
    ' Java: @AllArgsConstructor @Data
    Public Class DivObject

        Private ReadOnly idValue As String
        Private ReadOnly contentValue As String

        Public Sub New(ByVal id As String, ByVal content As String)
            Me.idValue = id
            Me.contentValue = content
        End Sub

        ''' <summary>The element id of the block.</summary>
        Public ReadOnly Property Id As String
            Get
                Return idValue
            End Get
        End Property

        ''' <summary>The content of the block (HTML in this port).</summary>
        Public ReadOnly Property Content As String
            Get
                Return contentValue
            End Get
        End Property

        ' ---- Lombok @Data accessors ----

        Public Function getId() As String
            Return idValue
        End Function

        Public Function getContent() As String
            Return contentValue
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As DivObject = TryCast(obj, DivObject)
            If other Is Nothing Then Return False
            Return String.Equals(idValue, other.idValue) AndAlso String.Equals(contentValue, other.contentValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(idValue, contentValue)
        End Function

        Public Overrides Function ToString() As String
            Return "DivObject(id=" & Objects.toString(idValue) & ", content=" & Objects.toString(contentValue) & ")"
        End Function

    End Class

End Namespace
