Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Condition = org.datavec.api.transform.condition.Condition
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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

Namespace org.datavec.api.transform.condition.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema"}) @EqualsAndHashCode(exclude = {"inputSchema"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public class SequenceLengthCondition implements org.datavec.api.transform.condition.Condition
	<Serializable>
	Public Class SequenceLengthCondition
		Implements Condition

		Private op As ConditionOp
		Private length As Integer?
		Private set As ISet(Of Integer)

'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		Public Sub New(ByVal op As ConditionOp, ByVal length As Integer)
			Me.New(op, length, Nothing)
		End Sub

		Public Sub New(ByVal op As ConditionOp, ByVal set As ISet(Of Integer))
			Me.New(op, Nothing, set)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private SequenceLengthCondition(@JsonProperty("op") org.datavec.api.transform.condition.ConditionOp op, @JsonProperty("length") System.Nullable<Integer> length, @JsonProperty("set") java.util.@Set<Integer> set)
		Private Sub New(ByVal op As ConditionOp, ByVal length As Integer?, ByVal set As ISet(Of Integer))
			If set IsNot Nothing And op <> ConditionOp.InSet AndAlso op <> ConditionOp.NotInSet Then
				Throw New System.ArgumentException("Invalid condition op: can only use this constructor with InSet or NotInSet ops")
			End If
			Me.op = op
			Me.length = length
			Me.set = set
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema 'No op
		End Function

		Public Overridable Function outputColumnName() As String
			Return inputSchema_Conflict.getColumnNames()(0)
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return CType(inputSchema_Conflict.getColumnNames(), List(Of String)).ToArray()
		End Function

		Public Overridable Function columnNames() As String()
			Return outputColumnNames()
		End Function

		Public Overridable Function columnName() As String
			Return outputColumnName()
		End Function

		Public Overridable Function condition(ByVal list As IList(Of Writable)) As Boolean Implements Condition.condition
			Throw New System.NotSupportedException("Cannot apply SequenceLengthCondition on non-sequence data")
		End Function

		Public Overridable Function condition(ByVal input As Object) As Boolean Implements Condition.condition
			Throw New System.NotSupportedException("Cannot apply SequenceLengthCondition on non-sequence data")
		End Function

		Public Overridable Function conditionSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean Implements Condition.conditionSequence
			Return op.apply(sequence.Count, (If(length Is Nothing, 0, length)), set)
		End Function

		Public Overridable Function conditionSequence(ByVal sequence As Object) As Boolean Implements Condition.conditionSequence
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable Property InputSchema Implements Condition.setInputSchema As Schema
			Set(ByVal schema As Schema)
				Me.inputSchema_Conflict = schema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property

	End Class

End Namespace