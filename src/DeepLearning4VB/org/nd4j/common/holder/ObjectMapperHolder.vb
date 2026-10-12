Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Serialization

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

Namespace org.nd4j.common.holder

    ''' <summary>
    ''' Holds the shared JSON configuration. The Java original held a Jackson <c>ObjectMapper</c>
    ''' configured to serialise fields only and skip nulls; this port uses System.Text.Json
    ''' <see cref="JsonSerializerOptions"/> with the equivalent settings.
    ''' </summary>
    Public NotInheritable Class ObjectMapperHolder

        Private Shared ReadOnly options As JsonSerializerOptions = CreateMapper()

        Private Sub New()
        End Sub

        ''' <summary>The shared, pre-configured serializer options (treat as read-only).</summary>
        Public Shared ReadOnly Property JsonMapper As JsonSerializerOptions
            Get
                Return options
            End Get
        End Property

        Public Shared Function getJsonMapper() As JsonSerializerOptions
            Return options
        End Function

        Private Shared Function CreateMapper() As JsonSerializerOptions
            Dim om As New JsonSerializerOptions()
            ' Serialize fields as well as properties, skip nulls, accept any property-name casing.
            om.IncludeFields = True
            om.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            om.PropertyNameCaseInsensitive = True
            om.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            om.Converters.Add(New JsonStringEnumConverter())
            Return om
        End Function

    End Class

End Namespace
