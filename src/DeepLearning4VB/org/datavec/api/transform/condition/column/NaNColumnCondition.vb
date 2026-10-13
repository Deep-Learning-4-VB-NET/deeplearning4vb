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
Imports System.Text.Json.Serialization

Namespace org.datavec.api.transform.condition.column

    ''' <summary>Condition that is satisfied when the (numeric) column value is NaN.</summary>
    <Serializable>
    Public Class NaNColumnCondition
        Inherits BaseColumnCondition

        ''' <param name="columnName"> Name of the column to check the condition for </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String)
            Me.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE)
        End Sub

        ''' <param name="columnName"> Name of the column to check the condition for </param>
        ''' <param name="sequenceConditionMode"> Sequence condition mode </param>
        Public Sub New(ByVal columnName As String, ByVal sequenceConditionMode As SequenceConditionMode)
            MyBase.New(columnName, sequenceConditionMode)
        End Sub

        Public Overrides Function columnCondition(ByVal writable As writable.Writable) As Boolean
            Return Double.IsNaN(writable.toDouble())
        End Function

        Public Overrides Function condition(ByVal input As Object) As Boolean
            Return Double.IsNaN(JavaNumbers.doubleValue(input))
        End Function

        Public Overrides Function ToString() As String
            Return "NaNColumnCondition()"
        End Function

    End Class

End Namespace
