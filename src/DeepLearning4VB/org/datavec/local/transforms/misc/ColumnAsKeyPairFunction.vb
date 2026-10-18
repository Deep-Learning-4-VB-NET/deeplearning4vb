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

    ''' <summary>Maps a record to a (value of one column, record) pair.</summary>
    Public Class ColumnAsKeyPairFunction
        Implements [Function](Of IList(Of WritableT), Pair(Of WritableT, IList(Of WritableT)))

        Private ReadOnly columnIdx As Integer

        Public Sub New(ByVal columnIdx As Integer)
            Me.columnIdx = columnIdx
        End Sub

        Public Overridable Function apply(ByVal writables As IList(Of WritableT)) As Pair(Of WritableT, IList(Of WritableT)) Implements [Function](Of IList(Of WritableT), Pair(Of WritableT, IList(Of WritableT))).apply
            Return Pair.of(writables(columnIdx), writables)
        End Function

    End Class

End Namespace
