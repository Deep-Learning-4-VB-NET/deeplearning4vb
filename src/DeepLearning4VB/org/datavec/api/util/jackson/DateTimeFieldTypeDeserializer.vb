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

Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType

Namespace org.datavec.api.util.jackson

    ''' <summary>
    ''' Reads a Joda <see cref="DateTimeFieldType"/> from <c>{"fieldType" : "hourOfDay"}</c>, the JSON of the Java
    ''' Jackson serializer. Also accepts a bare name (<c>"hourOfDay"</c>) and the <c>{"Name" : "hourOfDay"}</c> shape
    ''' System.Text.Json writes for the type by default. Used by <c>JsonMappers</c> for every <c>DateTimeFieldType</c> field.
    ''' </summary>
    Public NotInheritable Class DateTimeFieldTypeDeserializer

        Private Sub New()
        End Sub

        ''' <summary>The field type named by the JSON value.</summary>
        ''' <exception cref="JsonException">if the JSON does not name a known field type</exception>
        Public Shared Function deserialize(ByVal node As JsonNode) As DateTimeFieldType
            If node Is Nothing Then Return Nothing
            Dim name As String = Nothing
            If node.GetValueKind() = JsonValueKind.String Then
                name = node.GetValue(Of String)()
            Else
                Dim obj As JsonObject = TryCast(node, JsonObject)
                If obj IsNot Nothing Then
                    Dim v As JsonNode = Nothing
                    If obj.TryGetPropertyValue("fieldType", v) OrElse obj.TryGetPropertyValue("Name", v) Then
                        If v IsNot Nothing AndAlso v.GetValueKind() = JsonValueKind.String Then name = v.GetValue(Of String)()
                    End If
                End If
            End If
            If name Is Nothing Then Throw New JsonException("Expected {""fieldType"" : ""<name>""} for a DateTimeFieldType, got " & node.ToJsonString())
            Try
                Return DateTimeFieldType.forName(name)
            Catch e As System.Exception
                Throw New JsonException("Unknown DateTimeFieldType: """ & name & """", e)
            End Try
        End Function

    End Class

End Namespace
