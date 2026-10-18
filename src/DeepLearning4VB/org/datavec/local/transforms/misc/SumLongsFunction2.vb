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

Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives

Namespace org.datavec.local.transforms.misc

    ''' <summary>Adds the two longs of a pair (wrapping on overflow, as in Java).</summary>
    Public Class SumLongsFunction2
        Implements [Function](Of Pair(Of Long, Long), Long)

        Public Overridable Function apply(ByVal input As Pair(Of Long, Long)) As Long Implements [Function](Of Pair(Of Long, Long), Long).apply
            Return input.First + input.Second
        End Function

    End Class

End Namespace
