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
Imports System.Runtime.CompilerServices
Imports System.Text.Json.Serialization

Namespace org.datavec.api.transform.condition

    ''' <summary>
    ''' The comparison a column condition applies. The Java instance methods (<c>apply</c>) are the extension
    ''' methods in <see cref="ConditionOpExtensions"/>. Serializes by name.
    ''' </summary>
    <JsonConverter(GetType(JsonStringEnumConverter))>
    Public Enum ConditionOp
        LessThan
        LessOrEqual
        GreaterThan
        GreaterOrEqual
        Equal
        NotEqual
        InSet
        NotInSet
    End Enum

    ''' <summary>The Java instance methods of the <c>ConditionOp</c> enum, as extension methods.</summary>
    Public Module ConditionOpExtensions

        <Extension>
        Public Function apply(ByVal op As ConditionOp, ByVal x As Double, ByVal value As Double, ByVal [set] As ISet(Of Double)) As Boolean
            Select Case op
                Case ConditionOp.LessThan
                    Return x < value
                Case ConditionOp.LessOrEqual
                    Return x <= value
                Case ConditionOp.GreaterThan
                    Return x > value
                Case ConditionOp.GreaterOrEqual
                    Return x >= value
                Case ConditionOp.Equal
                    Return x = value
                Case ConditionOp.NotEqual
                    Return x <> value
                Case ConditionOp.InSet
                    Return [set].Contains(x)
                Case ConditionOp.NotInSet
                    Return Not [set].Contains(x)
                Case Else
                    Throw New Exception("Unknown or not implemented op: " & op.ToString())
            End Select
        End Function

        <Extension>
        Public Function apply(ByVal op As ConditionOp, ByVal x As Single, ByVal value As Single, ByVal [set] As ISet(Of Single)) As Boolean
            Select Case op
                Case ConditionOp.LessThan
                    Return x < value
                Case ConditionOp.LessOrEqual
                    Return x <= value
                Case ConditionOp.GreaterThan
                    Return x > value
                Case ConditionOp.GreaterOrEqual
                    Return x >= value
                Case ConditionOp.Equal
                    Return x = value
                Case ConditionOp.NotEqual
                    Return x <> value
                Case ConditionOp.InSet
                    Return [set].Contains(x)
                Case ConditionOp.NotInSet
                    Return Not [set].Contains(x)
                Case Else
                    Throw New Exception("Unknown or not implemented op: " & op.ToString())
            End Select
        End Function

        <Extension>
        Public Function apply(ByVal op As ConditionOp, ByVal x As Integer, ByVal value As Integer, ByVal [set] As ISet(Of Integer)) As Boolean
            Select Case op
                Case ConditionOp.LessThan
                    Return x < value
                Case ConditionOp.LessOrEqual
                    Return x <= value
                Case ConditionOp.GreaterThan
                    Return x > value
                Case ConditionOp.GreaterOrEqual
                    Return x >= value
                Case ConditionOp.Equal
                    Return x = value
                Case ConditionOp.NotEqual
                    Return x <> value
                Case ConditionOp.InSet
                    Return [set].Contains(x)
                Case ConditionOp.NotInSet
                    Return Not [set].Contains(x)
                Case Else
                    Throw New Exception("Unknown or not implemented op: " & op.ToString())
            End Select
        End Function

        <Extension>
        Public Function apply(ByVal op As ConditionOp, ByVal x As Long, ByVal value As Long, ByVal [set] As ISet(Of Long)) As Boolean
            Select Case op
                Case ConditionOp.LessThan
                    Return x < value
                Case ConditionOp.LessOrEqual
                    Return x <= value
                Case ConditionOp.GreaterThan
                    Return x > value
                Case ConditionOp.GreaterOrEqual
                    Return x >= value
                Case ConditionOp.Equal
                    Return x = value
                Case ConditionOp.NotEqual
                    Return x <> value
                Case ConditionOp.InSet
                    Return [set].Contains(x)
                Case ConditionOp.NotInSet
                    Return Not [set].Contains(x)
                Case Else
                    Throw New Exception("Unknown or not implemented op: " & op.ToString())
            End Select
        End Function

        <Extension>
        Public Function apply(ByVal op As ConditionOp, ByVal x As String, ByVal value As String, ByVal [set] As ISet(Of String)) As Boolean
            Select Case op
                Case ConditionOp.Equal
                    Return value.Equals(x)
                Case ConditionOp.NotEqual
                    Return Not value.Equals(x)
                Case ConditionOp.InSet
                    Return [set].Contains(x)
                Case ConditionOp.NotInSet
                    Return Not [set].Contains(x)
                Case ConditionOp.LessThan, ConditionOp.LessOrEqual, ConditionOp.GreaterThan, ConditionOp.GreaterOrEqual
                    Throw New NotSupportedException("Cannot use ConditionOp """ & op.ToString() & """ on Strings")
                Case Else
                    Throw New Exception("Unknown or not implemented op: " & op.ToString())
            End Select
        End Function

        ''' <summary>Java <c>ConditionOp.values()</c>.</summary>
        Public Function ConditionOpValues() As ConditionOp()
            Return DirectCast([Enum].GetValues(GetType(ConditionOp)), ConditionOp())
        End Function

        ''' <summary>Parses the enum constant name (Java <c>ConditionOp.valueOf</c>).</summary>
        Public Function ConditionOpValueOf(ByVal name As String) As ConditionOp
            Dim result As ConditionOp
            If [Enum].TryParse(name, False, result) AndAlso [Enum].IsDefined(GetType(ConditionOp), result) Then
                Return result
            End If
            Throw New ArgumentException("No enum constant ConditionOp." & name)
        End Function

    End Module

End Namespace
