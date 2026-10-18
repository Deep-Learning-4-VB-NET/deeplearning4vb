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

Imports System.Collections.Generic
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.misc

    ''' <summary>Maps a record to a (value of one column, 1) pair, for counting the values of a column.</summary>
    Public Class ColumnToKeyPairTransform
        Implements [Function](Of IList(Of WritableT), Pair(Of WritableT, Long))

        Private ReadOnly columnIndex As Integer

        Public Sub New(ByVal columnIndex As Integer)
            Me.columnIndex = columnIndex
        End Sub

        Public Overridable Function apply(ByVal list As IList(Of WritableT)) As Pair(Of WritableT, Long) Implements [Function](Of IList(Of WritableT), Pair(Of WritableT, Long)).apply
            Return Pair.of(list(columnIndex), 1L)
        End Function

    End Class

End Namespace
