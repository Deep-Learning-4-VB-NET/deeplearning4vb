Imports System
Imports System.Collections.Generic
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

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

Namespace org.datavec.api.records.reader.impl.jackson

    ''' <summary>
    ''' Reads one JSON document per line and extracts the fields of a <see cref="FieldSelection"/>. The Java
    ''' constructor also took an <c>ObjectMapper</c> (JSON, YAML or XML); this port reads JSON only with
    ''' System.Text.Json (see <see cref="JacksonReaderUtils"/>).
    ''' </summary>
    <Serializable>
    Public Class JacksonLineRecordReader
        Inherits LineRecordReader

        Private selection As FieldSelection

        Public Sub New(ByVal selection As FieldSelection)
            Me.selection = selection
        End Sub

        Public Overrides Function [next]() As IList(Of writable.Writable)
            Dim t As Text = CType(MyBase.next()(0), Text)
            Dim val As String = t.ToString()
            Return parseLine(val)
        End Function

        Protected Friend Overridable Function parseLine(ByVal line As String) As IList(Of writable.Writable)
            Return JacksonReaderUtils.parseRecord(line, selection)
        End Function

        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Throw New NotSupportedException()
            End Get
        End Property

    End Class

End Namespace
