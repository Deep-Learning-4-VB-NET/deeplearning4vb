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

Imports System.Text.Json.Nodes
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType

Namespace org.datavec.api.util.jackson

    ''' <summary>
    ''' Writes a Joda <see cref="DateTimeFieldType"/> as <c>{"fieldType" : "hourOfDay"}</c>, the JSON of the Java
    ''' Jackson serializer of the same name. Used by <c>JsonMappers</c> for every <c>DateTimeFieldType</c> field (a
    ''' System.Text.Json converter cannot be written in VB, so this is a plain function rather than a serializer class).
    ''' </summary>
    Public NotInheritable Class DateTimeFieldTypeSerializer

        Private Sub New()
        End Sub

        ''' <summary>The JSON object for the field type, or Nothing for Nothing.</summary>
        Public Shared Function serialize(ByVal dateTimeFieldType As DateTimeFieldType) As JsonNode
            If dateTimeFieldType Is Nothing Then Return Nothing
            Dim obj As New JsonObject()
            obj.Add("fieldType", JsonValue.Create(dateTimeFieldType.getName()))
            Return obj
        End Function

    End Class

End Namespace
