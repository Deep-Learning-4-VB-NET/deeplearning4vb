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
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat

Namespace org.datavec.api.transform.condition

    ''' <summary>
    ''' Combines conditions with AND, OR, NOT or XOR. The Lombok <c>getType()</c> accessor is
    ''' <see cref="getConditionType"/> here because <c>GetType()</c> is taken by <see cref="Object"/>.
    ''' </summary>
    <Serializable>
    Public Class BooleanCondition
        Implements Condition

        ''' <summary>The boolean operation. Serializes by name.</summary>
        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum Type
            [AND]
            [OR]
            [NOT]
            [XOR]
        End Enum

        Private ReadOnly typeField As Type
        Private ReadOnly conditionsField() As Condition

        <JsonConstructor>
        Public Sub New(ByVal type As Type, ByVal ParamArray conditions() As Condition)
            If conditions Is Nothing OrElse conditions.Length < 1 Then
                Throw New ArgumentException("Invalid input: conditions must be non-null and have at least 1 element")
            End If
            Select Case type
                Case BooleanCondition.Type.NOT
                    If conditions.Length <> 1 Then
                        Throw New ArgumentException("Invalid input: NOT conditions must have exactly 1 element")
                    End If
                Case BooleanCondition.Type.XOR
                    If conditions.Length <> 2 Then
                        Throw New ArgumentException("Invalid input: XOR conditions must have exactly 2 elements")
                    End If
            End Select
            Me.typeField = type
            Me.conditionsField = conditions
        End Sub

        ''' <summary>The boolean operation (Java: <c>getType()</c>).</summary>
        Public ReadOnly Property ConditionType As Type
            Get
                Return typeField
            End Get
        End Property

        Public Function getConditionType() As Type
            Return typeField
        End Function

        ''' <summary>The combined conditions.</summary>
        Public ReadOnly Property Conditions As Condition()
            Get
                Return conditionsField
            End Get
        End Property

        Public Function getConditions() As Condition()
            Return conditionsField
        End Function

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return conditionsField(0).outputColumnName()
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return conditionsField(0).outputColumnNames()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        '''
        ''' @return
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return conditionsField(0).columnNames()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        '''
        ''' @return
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return conditionsField(0).columnName()
        End Function

        Public Overridable Function condition(ByVal list As IList(Of writable.Writable)) As Boolean Implements Condition.condition
            Select Case typeField
                Case BooleanCondition.Type.AND
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.condition(list)
                        If Not thisCond Then
                            Return False 'Any false -> AND is false
                        End If
                    Next c
                    Return True
                Case BooleanCondition.Type.OR
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.condition(list)
                        If thisCond Then
                            Return True 'Any true -> OR is true
                        End If
                    Next c
                    Return False
                Case BooleanCondition.Type.NOT
                    Return Not conditionsField(0).condition(list)
                Case BooleanCondition.Type.XOR
                    Return conditionsField(0).condition(list) Xor conditionsField(1).condition(list)
                Case Else
                    Throw New Exception("Unknown condition type: " & typeField.ToString())
            End Select
        End Function

        ''' <summary>
        ''' Condition on arbitrary input
        ''' </summary>
        ''' <param name="input"> the input to return
        '''              the condition for </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Public Overridable Function condition(ByVal input As Object) As Boolean Implements Condition.condition
            Select Case typeField
                Case BooleanCondition.Type.AND
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.condition(input)
                        If Not thisCond Then
                            Return False 'Any false -> AND is false
                        End If
                    Next c
                    Return True
                Case BooleanCondition.Type.OR
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.condition(input)
                        If thisCond Then
                            Return True 'Any true -> OR is true
                        End If
                    Next c
                    Return False
                Case BooleanCondition.Type.NOT
                    Return Not conditionsField(0).condition(input)
                Case BooleanCondition.Type.XOR
                    Return conditionsField(0).condition(input) Xor conditionsField(1).condition(input)
                Case Else
                    Throw New Exception("Unknown condition type: " & typeField.ToString())
            End Select
        End Function

        Public Overridable Function conditionSequence(ByVal sequence As IList(Of IList(Of writable.Writable))) As Boolean Implements Condition.conditionSequence
            Select Case typeField
                Case BooleanCondition.Type.AND
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.conditionSequence(sequence)
                        If Not thisCond Then
                            Return False 'Any false -> AND is false
                        End If
                    Next c
                    Return True
                Case BooleanCondition.Type.OR
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.conditionSequence(sequence)
                        If thisCond Then
                            Return True 'Any true -> OR is true
                        End If
                    Next c
                    Return False
                Case BooleanCondition.Type.NOT
                    Return Not conditionsField(0).conditionSequence(sequence)
                Case BooleanCondition.Type.XOR
                    Return conditionsField(0).conditionSequence(sequence) Xor conditionsField(1).conditionSequence(sequence)
                Case Else
                    Throw New Exception("Unknown condition type: " & typeField.ToString())
            End Select
        End Function

        ''' <summary>
        ''' Condition on arbitrary input
        ''' </summary>
        ''' <param name="sequence"> the sequence to
        '''                 do a condition on </param>
        ''' <returns> true if the condition for the sequence is met false otherwise </returns>
        Public Overridable Function conditionSequence(ByVal sequence As Object) As Boolean Implements Condition.conditionSequence
            ' Java: List<?> seq = (List<?>) sequence; the element conditions then receive the list as an Object
            Dim seq As Object = DirectCast(sequence, System.Collections.IList)
            Select Case typeField
                Case BooleanCondition.Type.AND
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.conditionSequence(seq)
                        If Not thisCond Then
                            Return False 'Any false -> AND is false
                        End If
                    Next c
                    Return True
                Case BooleanCondition.Type.OR
                    For Each c As Condition In conditionsField
                        Dim thisCond As Boolean = c.conditionSequence(seq)
                        If thisCond Then
                            Return True 'Any true -> OR is true
                        End If
                    Next c
                    Return False
                Case BooleanCondition.Type.NOT
                    Return Not conditionsField(0).conditionSequence(sequence)
                Case BooleanCondition.Type.XOR
                    Return conditionsField(0).conditionSequence(sequence) Xor conditionsField(1).conditionSequence(seq)
                Case Else
                    Throw New Exception("Unknown condition type: " & typeField.ToString())
            End Select
        End Function

        ''' <summary>
        ''' Get the output schema for this transformation, given an input schema
        ''' </summary>
        ''' <param name="inputSchema"> </param>
        Public Overridable Function transform(ByVal inputSchema As schema.Schema) As schema.Schema Implements Operation(Of schema.Schema, schema.Schema).transform
            Return inputSchema
        End Function

        Public Overridable Property InputSchema As schema.Schema Implements ColumnOp.InputSchema
            Set(ByVal value As schema.Schema)
                For Each c As Condition In conditionsField
                    c.InputSchema = value
                Next c
            End Set
            Get
                Return conditionsField(0).InputSchema
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("BooleanCondition(").Append(typeField.ToString())
            For Each c As Condition In conditionsField
                sb.Append(",").Append(c.ToString())
            Next c
            sb.Append(")")
            Return sb.ToString()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other = TryCast(obj, BooleanCondition)
            If other Is Nothing OrElse other.GetType() IsNot Me.GetType() Then Return False
            Return typeField = other.typeField AndAlso Arrays.equals(conditionsField, other.conditionsField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 31 * Objects.hash(typeField) + Arrays.hashCode(conditionsField)
        End Function

        ''' <summary>
        ''' And of all the given conditions </summary>
        ''' <param name="conditions"> the conditions to and </param>
        ''' <returns> a joint and of all these conditions </returns>
        Public Shared Function [AND](ByVal ParamArray conditions() As Condition) As Condition
            Return New BooleanCondition(Type.AND, conditions)
        End Function

        ''' <summary>
        ''' Or of all the given conditions </summary>
        ''' <param name="conditions"> the conditions to or </param>
        ''' <returns> a joint and of all these conditions </returns>
        Public Shared Function [OR](ByVal ParamArray conditions() As Condition) As Condition
            Return New BooleanCondition(Type.OR, conditions)
        End Function

        ''' <summary>
        ''' Not of  the given condition </summary>
        ''' <param name="condition"> the conditions to and </param>
        ''' <returns> a joint and of all these condition </returns>
        Public Shared Function [NOT](ByVal condition As Condition) As Condition
            Return New BooleanCondition(Type.NOT, condition)
        End Function

        ''' <summary>
        ''' And of all the given conditions </summary>
        ''' <param name="first"> the first condition </param>
        ''' <param name="second">  the second condition for xor </param>
        ''' <returns> the xor of these 2 conditions </returns>
        Public Shared Function [XOR](ByVal first As Condition, ByVal second As Condition) As Condition
            Return New BooleanCondition(Type.XOR, first, second)
        End Function

    End Class

End Namespace
