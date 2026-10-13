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

Namespace org.datavec.api.transform.schema.conversion

    ''' <summary>
    ''' Converts writables, strings and arbitrary objects to the primitive column types with Java parsing
    ''' semantics (<c>Double.parseDouble</c> etc.; a <see cref="JavaCompat.NumberFormatException"/> on bad input).
    ''' </summary>
    Public Class TypeConversion

        Private Shared ReadOnly SINGLETON As New TypeConversion()

        Private Sub New()
        End Sub

        Public Shared ReadOnly Property Instance As TypeConversion
            Get
                Return SINGLETON
            End Get
        End Property

        Public Shared Function getInstance() As TypeConversion
            Return SINGLETON
        End Function

        Public Overridable Function convertInt(ByVal o As Object) As Integer
            If TypeOf o Is writable.Writable Then
                Dim writable As writable.Writable = DirectCast(o, writable.Writable)
                Return convertInt(writable)
            Else
                Return convertInt(o.ToString())
            End If
        End Function

        Public Overridable Function convertInt(ByVal writable As writable.Writable) As Integer
            Return writable.toInt()
        End Function

        Public Overridable Function convertInt(ByVal o As String) As Integer
            Return JavaNumbers.intValue(JavaNumbers.parseDouble(o))
        End Function

        Public Overridable Function convertDouble(ByVal writable As writable.Writable) As Double
            Return writable.toDouble()
        End Function

        Public Overridable Function convertDouble(ByVal o As String) As Double
            Return JavaNumbers.parseDouble(o)
        End Function

        Public Overridable Function convertDouble(ByVal o As Object) As Double
            If TypeOf o Is writable.Writable Then
                Dim writable As writable.Writable = DirectCast(o, writable.Writable)
                Return convertDouble(writable)
            Else
                Return convertDouble(o.ToString())
            End If
        End Function

        Public Overridable Function convertFloat(ByVal writable As writable.Writable) As Single
            Return writable.toFloat()
        End Function

        Public Overridable Function convertFloat(ByVal o As String) As Single
            Return JavaNumbers.parseFloat(o)
        End Function

        Public Overridable Function convertFloat(ByVal o As Object) As Single
            If TypeOf o Is writable.Writable Then
                Dim writable As writable.Writable = DirectCast(o, writable.Writable)
                Return convertFloat(writable)
            Else
                Return convertFloat(o.ToString())
            End If
        End Function

        Public Overridable Function convertLong(ByVal writable As writable.Writable) As Long
            Return writable.toLong()
        End Function

        Public Overridable Function convertLong(ByVal o As String) As Long
            Return JavaNumbers.parseLong(o)
        End Function

        Public Overridable Function convertLong(ByVal o As Object) As Long
            If TypeOf o Is writable.Writable Then
                Dim writable As writable.Writable = DirectCast(o, writable.Writable)
                Return convertLong(writable)
            Else
                Return convertLong(o.ToString())
            End If
        End Function

        Public Overridable Function convertString(ByVal writable As writable.Writable) As String
            Return writable.ToString()
        End Function

        Public Overridable Function convertString(ByVal s As String) As String
            Return s
        End Function

        Public Overridable Function convertString(ByVal o As Object) As String
            If TypeOf o Is writable.Writable Then
                Dim writable As writable.Writable = DirectCast(o, writable.Writable)
                Return convertString(writable)
            Else
                Return convertString(o.ToString())
            End If
        End Function

    End Class

End Namespace
