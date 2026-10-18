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
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform.filter

    ''' <summary>Returns true to keep a record, that is when the filter does not remove it.</summary>
    Public Class LocalFilterFunction
        Implements [Function](Of IList(Of WritableT), Boolean)

        Private ReadOnly filter As FilterT

        Public Sub New(ByVal filter As FilterT)
            Me.filter = filter
        End Sub

        Public Overridable Function apply(ByVal v1 As IList(Of WritableT)) As Boolean Implements [Function](Of IList(Of WritableT), Boolean).apply
            Return Not filter.removeExample(v1) 'return true to keep example (Filter: return true to remove)
        End Function

    End Class

End Namespace
