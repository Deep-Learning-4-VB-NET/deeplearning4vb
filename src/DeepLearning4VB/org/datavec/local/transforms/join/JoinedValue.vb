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
Imports System.Linq
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.join

    ''' <summary>
    ''' One joined record, with flags telling whether a left and a right record took part in it. Equality (Lombok
    ''' <c>@Data</c>) compares both flags and the values element by element.
    ''' </summary>
    <Serializable>
    Public Class JoinedValue

        Private ReadOnly haveLeftField As Boolean
        Private ReadOnly haveRightField As Boolean
        Private ReadOnly valuesField As IList(Of WritableT)

        Public Sub New(ByVal haveLeft As Boolean, ByVal haveRight As Boolean, ByVal values As IList(Of WritableT))
            Me.haveLeftField = haveLeft
            Me.haveRightField = haveRight
            Me.valuesField = values
        End Sub

        ''' <summary>True if a left record took part in the join.</summary>
        Public ReadOnly Property HaveLeft As Boolean
            Get
                Return haveLeftField
            End Get
        End Property

        ''' <summary>True if a right record took part in the join.</summary>
        Public ReadOnly Property HaveRight As Boolean
            Get
                Return haveRightField
            End Get
        End Property

        ''' <summary>The joined values.</summary>
        Public ReadOnly Property Values As IList(Of WritableT)
            Get
                Return valuesField
            End Get
        End Property

        ''' <summary>Java <c>isHaveLeft()</c> (Lombok).</summary>
        Public Function isHaveLeft() As Boolean
            Return haveLeftField
        End Function

        ''' <summary>Java <c>isHaveRight()</c> (Lombok).</summary>
        Public Function isHaveRight() As Boolean
            Return haveRightField
        End Function

        ''' <summary>Java <c>getValues()</c> (Lombok).</summary>
        Public Function getValues() As IList(Of WritableT)
            Return valuesField
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then
                Return True
            End If
            Dim other As JoinedValue = TryCast(obj, JoinedValue)
            If other Is Nothing Then
                Return False
            End If
            If haveLeftField <> other.haveLeftField OrElse haveRightField <> other.haveRightField Then
                Return False
            End If
            If valuesField Is Nothing OrElse other.valuesField Is Nothing Then
                Return valuesField Is Nothing AndAlso other.valuesField Is Nothing
            End If
            Return valuesField.SequenceEqual(other.valuesField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(haveLeftField, 79, 97)
            result = result * 59 + If(haveRightField, 79, 97)
            Dim valuesHash As Integer = 43
            If valuesField IsNot Nothing Then
                valuesHash = 1
                For Each w As WritableT In valuesField
                    valuesHash = 31 * valuesHash + If(w Is Nothing, 0, w.GetHashCode())
                Next w
            End If
            result = result * 59 + valuesHash
            Return result
        End Function

        Public Overrides Function ToString() As String
            Dim valuesText As String = If(valuesField Is Nothing, "null", "[" & String.Join(", ", valuesField.Select(Function(w) If(w Is Nothing, "null", w.ToString()))) & "]")
            Return "JoinedValue(haveLeft=" & If(haveLeftField, "true", "false") & ", haveRight=" & If(haveRightField, "true", "false") & ", values=" & valuesText & ")"
        End Function

    End Class

End Namespace
