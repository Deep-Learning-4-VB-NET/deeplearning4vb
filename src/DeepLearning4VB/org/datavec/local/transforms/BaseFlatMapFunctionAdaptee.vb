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
Imports DeepLearning4VB.org.datavec.local.transforms.functions

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' Base class of the flat-map functions of the local executor: delegates to a
    ''' <see cref="FlatMapFunctionAdapter(Of T, R)"/> and wraps any exception it throws in an
    ''' <see cref="InvalidOperationException"/> (Java wrapped it in an <c>IllegalStateException</c>).
    ''' </summary>
    ''' <typeparam name="K">the input type</typeparam>
    ''' <typeparam name="V">the type of each output</typeparam>
    Public Class BaseFlatMapFunctionAdaptee(Of K, V)

        Protected Friend ReadOnly adapter As FlatMapFunctionAdapter(Of K, V)

        Public Sub New(ByVal adapter As FlatMapFunctionAdapter(Of K, V))
            Me.adapter = adapter
        End Sub

        ''' <summary>Applies the adapter to <paramref name="k"/>.</summary>
        ''' <exception cref="InvalidOperationException">wrapping any exception thrown by the adapter</exception>
        Public Overridable Function [call](ByVal k As K) As IList(Of V)
            Try
                Return adapter.call(k)
            Catch e As Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

    End Class

End Namespace
