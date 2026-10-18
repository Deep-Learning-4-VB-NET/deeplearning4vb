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

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Serializer used for converting objects (Transforms, Conditions, etc) to JSON format, with the options of
    ''' <see cref="JsonMappers"/>. (Inside this namespace the name hides <c>System.Text.Json.JsonSerializer</c>.)
    ''' </summary>
    Public Class JsonSerializer
        Inherits BaseSerializer

        ''' <summary>The current <see cref="JsonMappers.Mapper"/> options.</summary>
        Public Overrides ReadOnly Property ObjectMapper As JsonSerializerOptions
            Get
                Return JsonMappers.Mapper
            End Get
        End Property
    End Class

End Namespace
