Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.condition


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @Data public class BooleanCondition implements Condition
	<Serializable>
	Public Class BooleanCondition
		Implements Condition

		Public Overridable Function outputColumnName() As String
			Return conditions(0).outputColumnName()
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String()
			Return conditions(0).outputColumnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String()
			Return conditions(0).columnNames()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String
			Return conditions(0).columnName()
		End Function

		Public Enum Type
			[AND]
			[OR]
			[NOT]
			[XOR]
		End Enum

		Private ReadOnly type As Type
		Private ReadOnly conditions() As Condition

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BooleanCondition(@JsonProperty("type") Type type, @JsonProperty("conditions") Condition... conditions)
		Public Sub New(ByVal type As Type, ParamArray ByVal conditions() As Condition)
			If conditions Is Nothing OrElse conditions.Length < 1 Then
				Throw New System.ArgumentException("Invalid input: conditions must be non-null and have at least 1 element")
			End If
			Select Case type
				Case org.datavec.api.transform.condition.BooleanCondition.Type.NOT
					If conditions.Length <> 1 Then
						Throw New System.ArgumentException("Invalid input: NOT conditions must have exactly 1 element")
					End If
				Case org.datavec.api.transform.condition.BooleanCondition.Type.XOR
					If conditions.Length <> 2 Then
						Throw New System.ArgumentException("Invalid input: XOR conditions must have exactly 2 elements")
					End If
			End Select
			Me.type = type
			Me.conditions = conditions
		End Sub

		Public Overridable Function condition(ByVal list As IList(Of Writable)) As Boolean Implements Condition.condition
			Select Case type
				Case org.datavec.api.transform.condition.BooleanCondition.Type.AND
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.condition(list)
						If Not thisCond Then
							Return False 'Any false -> AND is false
						End If
					Next c
					Return True
				Case org.datavec.api.transform.condition.BooleanCondition.Type.OR
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.condition(list)
						If thisCond Then
							Return True 'Any true -> OR is true
						End If
					Next c
					Return False
				Case org.datavec.api.transform.condition.BooleanCondition.Type.NOT
					Return Not conditions(0).condition(list)
				Case org.datavec.api.transform.condition.BooleanCondition.Type.XOR
					Return conditions(0).condition(list) Xor conditions(1).condition(list)
				Case Else
					Throw New Exception("Unknown condition type: " & type)
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
			Select Case type
				Case org.datavec.api.transform.condition.BooleanCondition.Type.AND
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.condition(input)
						If Not thisCond Then
							Return False 'Any false -> AND is false
						End If
					Next c
					Return True
				Case org.datavec.api.transform.condition.BooleanCondition.Type.OR
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.condition(input)
						If thisCond Then
							Return True 'Any true -> OR is true
						End If
					Next c
					Return False
				Case org.datavec.api.transform.condition.BooleanCondition.Type.NOT
					Return Not conditions(0).condition(input)
				Case org.datavec.api.transform.condition.BooleanCondition.Type.XOR
					Return conditions(0).condition(input) Xor conditions(1).condition(input)
				Case Else
					Throw New Exception("Unknown condition type: " & type)
			End Select
		End Function

		Public Overridable Function conditionSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean Implements Condition.conditionSequence
			Select Case type
				Case org.datavec.api.transform.condition.BooleanCondition.Type.AND
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.conditionSequence(sequence)
						If Not thisCond Then
							Return False 'Any false -> AND is false
						End If
					Next c
					Return True
				Case org.datavec.api.transform.condition.BooleanCondition.Type.OR
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.conditionSequence(sequence)
						If thisCond Then
							Return True 'Any true -> OR is true
						End If
					Next c
					Return False
				Case org.datavec.api.transform.condition.BooleanCondition.Type.NOT
					Return Not conditions(0).conditionSequence(sequence)
				Case org.datavec.api.transform.condition.BooleanCondition.Type.XOR
					Return conditions(0).conditionSequence(sequence) Xor conditions(1).conditionSequence(sequence)
				Case Else
					Throw New Exception("Unknown condition type: " & type)
			End Select
		End Function

		''' <summary>
		''' Condition on arbitrary input
		''' </summary>
		''' <param name="sequence"> the sequence to
		'''                 do a condition on </param>
		''' <returns> true if the condition for the sequence is met false otherwise </returns>
		Public Overridable Function conditionSequence(ByVal sequence As Object) As Boolean Implements Condition.conditionSequence
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> seq = (java.util.List<?>) sequence;
			Dim seq As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			Select Case type
				Case org.datavec.api.transform.condition.BooleanCondition.Type.AND
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.conditionSequence(seq)
						If Not thisCond Then
							Return False 'Any false -> AND is false
						End If
					Next c
					Return True
				Case org.datavec.api.transform.condition.BooleanCondition.Type.OR
					For Each c As Condition In conditions
						Dim thisCond As Boolean = c.conditionSequence(seq)
						If thisCond Then
							Return True 'Any true -> OR is true
						End If
					Next c
					Return False
				Case org.datavec.api.transform.condition.BooleanCondition.Type.NOT
					Return Not conditions(0).conditionSequence(sequence)
				Case org.datavec.api.transform.condition.BooleanCondition.Type.XOR
					Return conditions(0).conditionSequence(sequence) Xor conditions(1).conditionSequence(seq)
				Case Else
					Throw New Exception("Unknown condition type: " & type)
			End Select
		End Function

		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema
		End Function

		Public Overridable Property InputSchema Implements Condition.setInputSchema As Schema
			Set(ByVal schema As Schema)
				For Each c As Condition In conditions
					c.InputSchema = schema
				Next c
			End Set
			Get
				Return conditions(0).InputSchema
			End Get
		End Property


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("BooleanCondition(").Append(type)
			For Each c As Condition In conditions
				sb.Append(",").Append(c.ToString())
			Next c
			sb.Append(")")
			Return sb.ToString()
		End Function


		''' <summary>
		''' And of all the given conditions </summary>
		''' <param name="conditions"> the conditions to and </param>
		''' <returns> a joint and of all these conditions </returns>
		Public Shared Function [AND](ParamArray ByVal conditions() As Condition) As Condition
			Return New BooleanCondition(Type.AND, conditions)
		End Function

		''' <summary>
		''' Or of all the given conditions </summary>
		''' <param name="conditions"> the conditions to or </param>
		''' <returns> a joint and of all these conditions </returns>
		Public Shared Function [OR](ParamArray ByVal conditions() As Condition) As Condition
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