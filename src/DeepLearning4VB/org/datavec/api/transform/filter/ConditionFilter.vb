Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Condition = org.datavec.api.transform.condition.Condition
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

Namespace org.datavec.api.transform.filter


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @Data public class ConditionFilter implements Filter
	<Serializable>
	Public Class ConditionFilter
		Implements Filter

		Private ReadOnly condition As Condition

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConditionFilter(@JsonProperty("condition") org.datavec.api.transform.condition.Condition condition)
		Public Sub New(ByVal condition As Condition)
			Me.condition = condition
		End Sub

		''' <param name="writables"> Example </param>
		''' <returns> true if example should be removed, false to keep </returns>
		Public Overridable Function removeExample(ByVal writables As Object) As Boolean Implements Filter.removeExample
			Return condition.condition(writables)
		End Function

		''' <param name="sequence"> sequence example </param>
		''' <returns> true if example should be removed, false to keep </returns>
		Public Overridable Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence
			Return condition.condition(sequence)
		End Function

		Public Overridable Function removeExample(ByVal writables As IList(Of Writable)) As Boolean Implements Filter.removeExample
			Return condition.condition(writables)
		End Function

		Public Overridable Function removeSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean Implements Filter.removeSequence
			Return condition.conditionSequence(sequence)
		End Function

		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema
		End Function

		Public Overridable Property InputSchema Implements Filter.setInputSchema As Schema
			Set(ByVal schema As Schema)
				condition.InputSchema = schema
			End Set
			Get
				Return condition.InputSchema
			End Get
		End Property


		Public Overrides Function ToString() As String
			Return "ConditionFilter(" & condition & ")"
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String
			Return condition.outputColumnName()
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String()
			Return condition.outputColumnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String()
			Return condition.columnNames()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String
			Return condition.columnName()
		End Function
	End Class

End Namespace