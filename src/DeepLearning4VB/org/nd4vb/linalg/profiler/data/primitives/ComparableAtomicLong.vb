Imports System
Imports System.Threading

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

Namespace org.nd4vb.linalg.profiler.data.primitives

    ''' <summary>
    ''' An atomic counter that sorts in descending order of its value (Java <c>ComparableAtomicLong</c>, which extends
    ''' <c>java.util.concurrent.atomic.AtomicLong</c>; here the value is updated with <see cref="Interlocked"/> and
    ''' the <c>AtomicLong</c> methods the profiler uses are written out).
    ''' </summary>
    Public Class ComparableAtomicLong
        Implements IComparable(Of ComparableAtomicLong)

        Private value As Long

        Public Sub New()
            Me.New(0)
        End Sub

        Public Sub New(ByVal startingValue As Long)
            value = startingValue
        End Sub

        ''' <summary>The current value (Java <c>get()</c>).</summary>
        Public Function [get]() As Long
            Return Interlocked.Read(value)
        End Function

        ''' <summary>The current value (Java <c>longValue()</c>).</summary>
        Public Function longValue() As Long
            Return [get]()
        End Function

        ''' <summary>Sets the value (Java <c>set(long)</c>).</summary>
        Public Sub [set](ByVal newValue As Long)
            Interlocked.Exchange(value, newValue)
        End Sub

        ''' <summary>Adds one and returns the new value.</summary>
        Public Function incrementAndGet() As Long
            Return Interlocked.Increment(value)
        End Function

        ''' <summary>Adds <paramref name="delta"/> and returns the new value.</summary>
        Public Function addAndGet(ByVal delta As Long) As Long
            Return Interlocked.Add(value, delta)
        End Function

        ''' <summary>Descending order: a larger value sorts first.</summary>
        Public Overridable Function CompareTo(ByVal o As ComparableAtomicLong) As Integer Implements IComparable(Of ComparableAtomicLong).CompareTo
            Return o.get().CompareTo(Me.get())
        End Function

        Public Overrides Function ToString() As String
            Return [get]().ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

    End Class

End Namespace
