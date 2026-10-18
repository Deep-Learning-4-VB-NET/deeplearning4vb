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
Imports LongWritableT = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.rank

    ''' <summary>
    ''' Given ((sort key, record), rank), returns a copy of the record with the rank appended as a <c>LongWritable</c>.
    ''' </summary>
    Public Class UnzipForCalculateSortedRankFunction
        Implements [Function](Of Pair(Of Pair(Of WritableT, IList(Of WritableT)), Long), IList(Of WritableT))

        Public Overridable Function apply(ByVal v1 As Pair(Of Pair(Of WritableT, IList(Of WritableT)), Long)) As IList(Of WritableT) Implements [Function](Of Pair(Of Pair(Of WritableT, IList(Of WritableT)), Long), IList(Of WritableT)).apply
            Dim inputWritables As IList(Of WritableT) = New List(Of WritableT)(v1.First.Second)
            inputWritables.Add(New LongWritableT(v1.Second))
            Return inputWritables
        End Function

    End Class

End Namespace
