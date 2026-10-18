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
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.join

    ''' <summary>
    ''' Maps a record to a (key, record) pair, where the key is the list of the values of the given (join) columns, in
    ''' the order of <c>columnIndexes</c>.
    ''' </summary>
    Public Class ExtractKeysFunction
        Implements [Function](Of IList(Of WritableT), Pair(Of IList(Of WritableT), IList(Of WritableT)))

        Private columnIndexes() As Integer

        Public Sub New(ByVal columnIndexes() As Integer)
            Me.columnIndexes = columnIndexes
        End Sub

        Public Overridable Function apply(ByVal writables As IList(Of WritableT)) As Pair(Of IList(Of WritableT), IList(Of WritableT)) Implements [Function](Of IList(Of WritableT), Pair(Of IList(Of WritableT), IList(Of WritableT))).apply
            Dim keyValues As IList(Of WritableT)
            If columnIndexes.Length = 1 Then
                keyValues = New List(Of WritableT)(1) From {writables(columnIndexes(0))}
            Else
                keyValues = New List(Of WritableT)(columnIndexes.Length)
                For Each i As Integer In columnIndexes
                    keyValues.Add(writables(i))
                Next i
            End If

            Return Pair.of(keyValues, writables)
        End Function

    End Class

End Namespace
