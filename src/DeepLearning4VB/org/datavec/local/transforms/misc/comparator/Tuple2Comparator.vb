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
Imports DeepLearning4VB.org.nd4vb.common.primitives

Namespace org.datavec.local.transforms.misc.comparator

    ''' <summary>Orders (value, count) pairs by their count, ascending or descending.</summary>
    ''' <typeparam name="T">the value type</typeparam>
    <Serializable>
    Public Class Tuple2Comparator(Of T)
        Implements IComparer(Of Pair(Of T, Long))

        Private ReadOnly ascending As Boolean

        Public Sub New(ByVal ascending As Boolean)
            Me.ascending = ascending
        End Sub

        Public Overridable Function Compare(ByVal o1 As Pair(Of T, Long), ByVal o2 As Pair(Of T, Long)) As Integer Implements IComparer(Of Pair(Of T, Long)).Compare
            If ascending Then
                Return o1.Second.CompareTo(o2.Second)
            Else
                Return -o1.Second.CompareTo(o2.Second)
            End If
        End Function

    End Class

End Namespace
