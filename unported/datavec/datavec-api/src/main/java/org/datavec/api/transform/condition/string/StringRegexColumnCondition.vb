Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SequenceConditionMode = org.datavec.api.transform.condition.SequenceConditionMode
Imports BaseColumnCondition = org.datavec.api.transform.condition.column.BaseColumnCondition
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

Namespace org.datavec.api.transform.condition.string

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class StringRegexColumnCondition extends org.datavec.api.transform.condition.column.BaseColumnCondition
	<Serializable>
	Public Class StringRegexColumnCondition
		Inherits BaseColumnCondition

		Private ReadOnly regex As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringRegexColumnCondition(@JsonProperty("columnName") String columnName, @JsonProperty("regex") String regex)
		Public Sub New(ByVal columnName As String, ByVal regex As String)
			Me.New(columnName, regex, DEFAULT_SEQUENCE_CONDITION_MODE)
		End Sub

		Public Sub New(ByVal columnName As String, ByVal regex As String, ByVal sequenceConditionMode As SequenceConditionMode)
			MyBase.New(columnName, sequenceConditionMode)
			Me.regex = regex
		End Sub

		Public Overrides Function columnCondition(ByVal writable As Writable) As Boolean
			Return writable.ToString().matches(regex)
		End Function

		Public Overrides Function ToString() As String
			Return "StringRegexColumnCondition(columnName=""" & columnName_Conflict & """,regex=""" & regex & """)"
		End Function

		''' <summary>
		''' Condition on arbitrary input
		''' </summary>
		''' <param name="input"> the input to return
		'''              the condition for </param>
		''' <returns> true if the condition is met
		''' false otherwise </returns>
		Public Overrides Function condition(ByVal input As Object) As Boolean
			Return input.ToString().matches(regex)
		End Function

	End Class

End Namespace