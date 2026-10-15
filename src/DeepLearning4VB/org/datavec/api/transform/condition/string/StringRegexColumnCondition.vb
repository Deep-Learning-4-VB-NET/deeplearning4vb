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
Imports System.Text.RegularExpressions
Imports SequenceConditionMode = DeepLearning4VB.org.datavec.api.transform.condition.SequenceConditionMode
Imports BaseColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.BaseColumnCondition
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition

Namespace org.datavec.api.transform.condition.string

    ''' <summary>
    ''' Condition that is satisfied when the whole string value of the column matches a regular expression
    ''' (Java <c>String.matches</c> semantics: the expression must match the entire value).
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data; JSON properties "columnName" and "regex".
    <Serializable>
    Public Class StringRegexColumnCondition
        Inherits BaseColumnCondition

        Private ReadOnly regex As String
        <NonSerialized> Private compiled As Regex

        Public Sub New(ByVal columnName As String, ByVal regex As String)
            Me.New(columnName, regex, DEFAULT_SEQUENCE_CONDITION_MODE)
        End Sub

        Public Sub New(ByVal columnName As String, ByVal regex As String, ByVal sequenceConditionMode As SequenceConditionMode)
            MyBase.New(columnName, sequenceConditionMode)
            If regex Is Nothing Then Throw New ArgumentNullException("regex")
            Me.regex = regex
        End Sub

        ''' <summary>Java <c>getRegex()</c> (Lombok).</summary>
        Public Function getRegex() As String
            Return regex
        End Function

        Private Function Matches(ByVal text As String) As Boolean
            Dim re As Regex = compiled
            If re Is Nothing Then
                re = New Regex("\A(?:" & regex & ")\z", RegexOptions.CultureInvariant)
                compiled = re
            End If
            Return re.IsMatch(text)
        End Function

        Public Overrides Function columnCondition(ByVal writable As WritableT) As Boolean
            Return Matches(writable.ToString())
        End Function

        Public Overrides Function ToString() As String
            Return "StringRegexColumnCondition(columnName=""" & columnNameField & """,regex=""" & regex & """)"
        End Function

        ''' <summary>
        ''' Condition on arbitrary input
        ''' </summary>
        ''' <param name="input"> the input to return
        '''              the condition for </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Public Overrides Function condition(ByVal input As Object) As Boolean
            Return Matches(input.ToString())
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Return String.Equals(regex, DirectCast(obj, StringRegexColumnCondition).regex)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(columnNameField, regex)
        End Function

    End Class

End Namespace
