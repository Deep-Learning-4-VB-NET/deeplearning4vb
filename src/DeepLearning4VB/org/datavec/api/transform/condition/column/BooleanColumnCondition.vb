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
Imports BooleanWritable = DeepLearning4VB.org.datavec.api.writable.BooleanWritable

Namespace org.datavec.api.transform.condition.column

    ''' <summary>Condition on a boolean column: satisfied when the value is true.</summary>
    <Serializable>
    Public Class BooleanColumnCondition
        Inherits BaseColumnCondition

        Protected Friend Sub New(ByVal columnName As String, ByVal sequenceConditionMode As SequenceConditionMode)
            MyBase.New(columnName, sequenceConditionMode)
        End Sub

        ''' <summary>
        ''' Returns whether the given element
        ''' meets the condition set by this operation
        ''' </summary>
        ''' <param name="writable"> the element to test </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Public Overrides Function columnCondition(ByVal writable As writable.Writable) As Boolean
            Dim booleanWritable As BooleanWritable = DirectCast(writable, BooleanWritable)
            Return booleanWritable.get()
        End Function

        ''' <summary>
        ''' Condition on arbitrary input
        ''' </summary>
        ''' <param name="input"> the input to return
        '''              the condition for </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Public Overrides Function condition(ByVal input As Object) As Boolean
            Return CBool(input)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetType().ToString()
        End Function
    End Class

End Namespace
