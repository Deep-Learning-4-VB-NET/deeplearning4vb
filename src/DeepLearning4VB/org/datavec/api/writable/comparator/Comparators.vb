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
Imports System.Collections.Generic
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.writable.comparator

    ''' <summary>Built-in comparers for the numeric and text writable types.</summary>
    Public Class Comparators

        Private Sub New()
        End Sub

        Public Shared Function forType(ByVal type As WritableType) As IComparer(Of Writable)
            Return forType(type, True)
        End Function

        Public Shared Function forType(ByVal type As WritableType, ByVal ascending As Boolean) As IComparer(Of Writable)
            Dim c As IComparer(Of Writable)
            Select Case type
                Case WritableType.Byte, WritableType.Int
                    c = New IntWritableComparator()
                Case WritableType.Double
                    c = New DoubleWritableComparator()
                Case WritableType.Float
                    c = New FloatWritableComparator()
                Case WritableType.Long
                    c = New LongWritableComparator()
                Case WritableType.Text
                    c = New TextWritableComparator()
                Case Else
                    Throw New NotSupportedException("No built-in comparator for writable type: " & type.ToString())
            End Select
            If ascending Then
                Return c
            End If
            Return New ReverseComparator(Of Writable)(c)
        End Function
    End Class

End Namespace
