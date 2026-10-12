Imports System.Collections.Generic
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports Condition = org.datavec.api.transform.condition.Condition
Imports SequenceConditionMode = org.datavec.api.transform.condition.SequenceConditionMode
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.condition.column


	Public Interface ColumnCondition
		Inherits Condition, ColumnOp

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		org.datavec.api.transform.condition.SequenceConditionMode DEFAULT_SEQUENCE_CONDITION_MODE = org.datavec.api.transform.condition.SequenceConditionMode.@Or;

		Overrides Property InputSchema As Schema

		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Overrides Function transform(ByVal inputSchema As Schema) As Schema


		Overrides Function condition(ByVal list As IList(Of Writable)) As Boolean

		Overrides Function conditionSequence(ByVal list As IList(Of IList(Of Writable))) As Boolean

		Overrides Function conditionSequence(ByVal list As Object) As Boolean

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Overrides Function outputColumnName() As String

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Overrides Function outputColumnNames() As String()

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Overrides Function columnNames() As String()

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Overrides Function columnName() As String

		''' <summary>
		''' Returns whether the given element
		''' meets the condition set by this operation </summary>
		''' <param name="writable"> the element to test </param>
		''' <returns> true if the condition is met
		''' false otherwise </returns>
		Function columnCondition(ByVal writable As Writable) As Boolean
	End Interface

End Namespace