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

Namespace org.datavec.api.transform.ui.components

    ''' <summary>
    ''' Base class of the building blocks of the DataVec HTML reports: <see cref="RenderableComponentTable"/>,
    ''' <see cref="RenderableComponentHistogram"/> and <see cref="RenderableComponentLineChart"/>.
    ''' <para>In Java this was a Jackson polymorphic type (a wrapper object keyed by the subclass name) because the
    ''' components travelled to the browser as JSON and were drawn there by the DL4J UI JavaScript. This port renders
    ''' each component itself, to HTML and inline SVG (<see cref="toHtml"/>), so the components have no JSON form and
    ''' none is planned.</para>
    ''' </summary>
    ' Java: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.As.WRAPPER_OBJECT) @JsonSubTypes({...}) @Data
    Public MustInherit Class RenderableComponent

        ''' <summary>
        ''' Component type (<c>simpletable</c>, <c>histogram</c>, <c>linechart</c>); the Java UI used it to pick the
        ''' JavaScript renderer for the JSON of the component.
        ''' </summary>
        Protected ReadOnly componentType As String

        Public Sub New(ByVal componentType As String)
            Me.componentType = componentType
        End Sub

        Public Function getComponentType() As String
            Return componentType
        End Function

        ''' <summary>
        ''' Renders the component as a self-contained HTML fragment (no scripts, no external resources). The fragment
        ''' carries fallback colours, and picks up the light/dark colour tokens when it is placed in a page made by
        ''' <c>HtmlAnalysis</c> or <c>HtmlSequencePlotting</c>. Every text that comes from data is HTML-encoded.
        ''' </summary>
        Public MustOverride Function toHtml() As String

        ''' <summary>Equal when the other object has the same class and component type (Lombok <c>@Data</c>).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot CObj(Me).GetType() Then Return False
            Return String.Equals(componentType, DirectCast(obj, RenderableComponent).componentType)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(componentType)
        End Function

        Public Overrides Function ToString() As String
            Return "RenderableComponent(componentType=" & Objects.toString(componentType) & ")"
        End Function

    End Class

End Namespace
