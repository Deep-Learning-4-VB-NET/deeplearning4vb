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
Imports System.Text
Imports DeepLearning4VB.org.nd4j.common.function
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' The string aggregations (append, prepend) as combinable accumulators. The Java base class was a private
    ''' nested class; VB cannot derive a public class from a private one, so it is public here.
    ''' </summary>
    Public Class StringAggregatorImpls

        ''' <summary>Base of the string reductions: a shared <see cref="StringBuilder"/>.</summary>
        <Serializable>
        Public MustInherit Class AggregableStringReduce
            Implements IAggregableReduceOp(Of String, writable.Writable)

            Protected Friend sb As New StringBuilder()

            ''' <summary>The builder holding the accumulated text (Java <c>getSb()</c>).</summary>
            Public Function getSb() As StringBuilder
                Return sb
            End Function

            Public MustOverride Sub combine(ByVal accu As IAggregableReduceOp(Of String, writable.Writable)) Implements IAggregableReduceOp(Of String, writable.Writable).combine

            Public MustOverride Sub accept(ByVal s As String) Implements Consumer(Of String).accept

            Public MustOverride Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]

            Protected Shared Function incompatible(ByVal accu As Object, ByVal expected As Object) As NotSupportedException
                Return New NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where" & expected.GetType().FullName & " expected")
            End Function
        End Class

        ''' <summary>Concatenates the strings in the order seen.</summary>
        <Serializable>
        Public Class AggregableStringAppend
            Inherits AggregableStringReduce

            Public Overrides Sub combine(ByVal accu As IAggregableReduceOp(Of String, writable.Writable))
                If TypeOf accu Is AggregableStringAppend Then
                    sb.Append(DirectCast(accu, AggregableStringAppend).getSb())
                Else
                    Throw incompatible(accu, Me)
                End If
            End Sub

            Public Overrides Sub accept(ByVal s As String)
                sb.Append(s)
            End Sub

            Public Overrides Function [get]() As writable.Writable
                Return New Text(sb.ToString())
            End Function
        End Class

        ''' <summary>Concatenates the strings in reverse order (each string is prepended to the result).</summary>
        <Serializable>
        Public Class AggregableStringPrepend
            Inherits AggregableStringReduce

            Public Overrides Sub combine(ByVal accu As IAggregableReduceOp(Of String, writable.Writable))
                If TypeOf accu Is AggregableStringPrepend Then
                    sb.Append(DirectCast(accu, AggregableStringPrepend).getSb())
                Else
                    Throw incompatible(accu, Me)
                End If
            End Sub

            Public Overrides Sub accept(ByVal s As String)
                Dim rev As String = JavaStrings.reverse(s)
                sb.Append(rev)
            End Sub

            Public Overrides Function [get]() As writable.Writable
                Return New Text(JavaStrings.reverse(sb.ToString()))
            End Function
        End Class

    End Class

End Namespace
