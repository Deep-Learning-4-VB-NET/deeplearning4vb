Imports System

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

Namespace org.nd4vb.linalg.indexing.conditions

    ''' <summary>
    ''' Builds a composite condition step by step: each call combines the given conditions with the condition built so
    ''' far (which comes last).
    ''' <para>
    ''' Deliberate difference: Java joined the conditions with <c>ArrayUtil.combine</c>, which creates an array of the
    ''' runtime class of the first condition and so threw <c>ArrayStoreException</c> whenever the conditions had
    ''' different classes; this port joins them into a <see cref="Condition"/> array.
    ''' </para>
    ''' </summary>
    Public Class ConditionBuilder

        Private soFar As Condition

        ''' <summary>Combines with <see cref="[Or]"/>.</summary>
        Public Overridable Function [or](ParamArray ByVal conditions() As Condition) As ConditionBuilder
            If soFar Is Nothing Then
                soFar = New [Or](conditions)
            Else
                soFar = New [Or](combine(conditions))
            End If
            Return Me
        End Function

        ''' <summary>Combines with <see cref="[And]"/>.</summary>
        Public Overridable Function [and](ParamArray ByVal conditions() As Condition) As ConditionBuilder
            If soFar Is Nothing Then
                soFar = New [And](conditions)
            Else
                soFar = New [And](combine(conditions))
            End If
            Return Me
        End Function

        ''' <summary>Combines with <see cref="ConditionEquals"/>.</summary>
        Public Overridable Function eq(ParamArray ByVal conditions() As Condition) As ConditionBuilder
            If soFar Is Nothing Then
                soFar = New ConditionEquals(conditions)
            Else
                soFar = New ConditionEquals(combine(conditions))
            End If
            Return Me
        End Function

        ''' <summary>Negates the condition built so far.</summary>
        ''' <exception cref="InvalidOperationException">Nothing has been built yet.</exception>
        Public Overridable Function [not]() As ConditionBuilder
            If soFar Is Nothing Then
                Throw New InvalidOperationException("No condition to take the opposite of")
            End If
            soFar = New [Not](soFar)
            Return Me
        End Function

        ''' <summary>The given conditions followed by the condition built so far.</summary>
        Private Function combine(ByVal conditions() As Condition) As Condition()
            Dim ret(conditions.Length) As Condition
            Array.Copy(conditions, ret, conditions.Length)
            ret(conditions.Length) = soFar
            Return ret
        End Function

        ''' <summary>The condition built so far (<c>Nothing</c> before the first call).</summary>
        Public Overridable Function build() As Condition
            Return soFar
        End Function

    End Class

End Namespace
