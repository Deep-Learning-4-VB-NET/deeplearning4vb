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
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable

Namespace org.datavec.api.util

    ''' <summary>Converts numeric arrays to records (lists of writables).</summary>
    Public Class RecordUtils

        Public Shared Function toRecord(ByVal record() As Double) As IList(Of writable.Writable)
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)(record.Length)
            For i As Integer = 0 To record.Length - 1
                ret.Add(New DoubleWritable(record(i)))
            Next i

            Return ret
        End Function

        Public Shared Function toRecord(ByVal record() As Single) As IList(Of writable.Writable)
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)(record.Length)
            For i As Integer = 0 To record.Length - 1
                ret.Add(New FloatWritable(record(i)))
            Next i

            Return ret
        End Function

    End Class

End Namespace
