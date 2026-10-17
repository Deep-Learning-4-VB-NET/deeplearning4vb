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
Imports System.Text.Json.Serialization
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.comparator

    ''' <summary>
    ''' Sequence comparator: compares the time steps on the string value of one column (ordinal comparison of the
    ''' UTF-16 code units, like Java's <c>String.compareTo</c>). Equality compares the column name (the Java
    ''' <c>@Data</c> class compared no fields, so all StringComparators were equal).
    ''' </summary>
    <Serializable>
    Public Class StringComparator
        Inherits BaseColumnComparator

        ''' <param name="columnName"> Name of the column in which to compare values </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String)
            MyBase.New(columnName)
        End Sub

        Protected Friend Overrides Function compare(ByVal w1 As WritableT, ByVal w2 As WritableT) As Integer
            Return String.CompareOrdinal(w1.ToString(), w2.ToString())
        End Function

        Public Overrides Function ToString() As String
            Return "StringComparator()"
        End Function

    End Class

End Namespace
