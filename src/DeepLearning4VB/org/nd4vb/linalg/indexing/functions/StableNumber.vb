Imports System
Imports DeepLearning4VB.org.nd4vb.common.function
Imports System.Globalization
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

'
' *  ******************************************************************************
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

Namespace org.nd4vb.linalg.indexing.functions

    ''' <summary>
    ''' Replaces numbers that would make a computation unstable: infinity becomes the most negative finite value
    ''' (<c>-Double.MaxValue</c> or <c>-Single.MaxValue</c>) and NaN becomes <c>Nd4vb.EPS_THRESHOLD</c>.
    ''' <para>
    ''' Deliberate difference: Java's <c>switch</c> had no <c>return</c> for finite numbers and fell through to its
    ''' <c>default</c> branch, so <c>apply</c> threw <c>IllegalStateException</c> for every finite number (and the
    ''' DOUBLE case fell into the FLOAT checks). This port returns finite numbers unchanged and keeps the two cases
    ''' apart. Java's field <c>type</c> is <c>numberType</c> (it would clash with the nested enum <see cref="Type"/>).
    ''' </para>
    ''' </summary>
    Public Class StableNumber
        Implements [Function](Of Object, Object)

        ''' <summary>The precision the number is checked in.</summary>
        Public Enum Type
            [DOUBLE]
            FLOAT
        End Enum

        Private ReadOnly numberType As Type

        ''' <param name="type"> the precision to check the numbers in </param>
        Public Sub New(ByVal type As Type)
            Me.numberType = type
        End Sub

        Public Overridable Function apply(ByVal number As Object) As Object Implements [Function](Of Object, Object).apply
            Select Case numberType
                Case Type.DOUBLE
                    Dim d As Double = Convert.ToDouble(number, CultureInfo.InvariantCulture)
                    If Double.IsInfinity(d) Then
                        Return -Double.MaxValue
                    End If
                    If Double.IsNaN(d) Then
                        Return Nd4vbT.EPS_THRESHOLD
                    End If
                    Return number
                Case Type.FLOAT
                    Dim f As Single = Convert.ToSingle(number, CultureInfo.InvariantCulture)
                    If Single.IsInfinity(f) Then
                        Return -Single.MaxValue
                    End If
                    If Single.IsNaN(f) Then
                        Return Nd4vbT.EPS_THRESHOLD
                    End If
                    Return number
                Case Else
                    Throw New InvalidOperationException("Illegal opType")
            End Select
        End Function

    End Class

End Namespace
