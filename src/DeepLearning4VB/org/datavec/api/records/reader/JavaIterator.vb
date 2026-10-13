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

Namespace org.datavec.api.records.reader

    ''' <summary>
    ''' A Java-style iterator (<c>hasNext()</c>/<c>next()</c>) over an <see cref="IEnumerator(Of T)"/>. The record
    ''' readers were written against <c>java.util.Iterator</c>, whose look-ahead <c>hasNext()</c> has no
    ''' counterpart on <see cref="IEnumerator(Of T)"/>; this wrapper supplies it. <c>next()</c> past the end throws
    ''' <see cref="InvalidOperationException"/> (Java <c>NoSuchElementException</c>). Disposing the wrapper
    ''' disposes the underlying enumerator.
    ''' </summary>
    Public Class JavaIterator(Of T)
        Implements IDisposable

        Private source As IEnumerator(Of T)
        Private peeked As Boolean
        Private peekedValue As T
        Private finished As Boolean

        Public Sub New(ByVal source As IEnumerator(Of T))
            If source Is Nothing Then Throw New ArgumentNullException(NameOf(source))
            Me.source = source
        End Sub

        Public Sub New(ByVal source As IEnumerable(Of T))
            Me.New(source.GetEnumerator())
        End Sub

        ''' <summary>An iterator over a single item (Java <c>Collections.singletonList(item).iterator()</c>).</summary>
        Public Shared Function singleton(ByVal item As T) As JavaIterator(Of T)
            Return New JavaIterator(Of T)(New T() {item})
        End Function

        ''' <summary>True if <see cref="next"/> would return an element.</summary>
        Public Overridable Function hasNext() As Boolean
            If peeked Then Return True
            If finished Then Return False
            If source.MoveNext() Then
                peekedValue = source.Current
                peeked = True
                Return True
            End If
            finished = True
            Return False
        End Function

        ''' <summary>The next element.</summary>
        ''' <exception cref="InvalidOperationException">if there are no more elements (Java <c>NoSuchElementException</c>)</exception>
        Public Overridable Function [next]() As T
            If Not hasNext() Then
                Throw New InvalidOperationException("No more elements (NoSuchElementException)")
            End If
            peeked = False
            Dim v As T = peekedValue
            peekedValue = Nothing
            Return v
        End Function

        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            If source IsNot Nothing Then
                source.Dispose()
            End If
            finished = True
            peeked = False
        End Sub

    End Class

End Namespace
