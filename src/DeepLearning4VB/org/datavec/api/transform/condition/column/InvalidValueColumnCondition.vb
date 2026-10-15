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
Imports DeepLearning4VB.org.datavec.api.writable
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.condition.column

    ''' <summary>Condition that is satisfied when the column value is invalid according to the column's metadata.</summary>
    ' Java: @Data
    <Serializable>
    Public Class InvalidValueColumnCondition
        Inherits BaseColumnCondition

        Public Sub New(ByVal columnName As String)
            MyBase.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE)
        End Sub

        Public Overrides Function columnCondition(ByVal writable As WritableT) As Boolean
            Return Not schemaField.getMetaData(columnIdx).isValid(writable)
        End Function

        Public Overrides Function ToString() As String
            Return "InvalidValueColumnCondition(columnName=""" & columnNameField & """)"
        End Function

        ''' <summary>
        ''' Condition on arbitrary input
        ''' </summary>
        ''' <param name="input"> the input to return
        '''              the condition for </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Public Overrides Function condition(ByVal input As Object) As Boolean
            If TypeOf input Is String Then
                Return Not schemaField.getMetaData(columnIdx).isValid(New Text(input.ToString()))
            ElseIf TypeOf input Is Double Then
                Return Not schemaField.getMetaData(columnIdx).isValid(New DoubleWritable(CDbl(input)))
            ElseIf TypeOf input Is Integer Then
                Return Not schemaField.getMetaData(columnIdx).isValid(New IntWritable(CInt(input)))
            ElseIf TypeOf input Is Long Then
                Return Not schemaField.getMetaData(columnIdx).isValid(New LongWritable(CLng(input)))
            ElseIf TypeOf input Is Boolean Then
                Return Not schemaField.getMetaData(columnIdx).isValid(New BooleanWritable(CBool(input)))
            Else
                Throw New InvalidOperationException("Illegal type " & input.GetType().ToString())
            End If
        End Function

    End Class

End Namespace
