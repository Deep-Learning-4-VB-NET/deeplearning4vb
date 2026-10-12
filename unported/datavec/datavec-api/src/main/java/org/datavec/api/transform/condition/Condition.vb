Imports System.Collections.Generic
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface Condition extends java.io.Serializable, org.datavec.api.transform.ColumnOp
	Public Interface Condition
		Inherits ColumnOp

		''' <summary>
		''' Is the condition satisfied for the current input/example?<br>
		''' Returns true if condition is satisfied, or false otherwise.
		''' </summary>
		''' <param name="list"> Current example </param>
		''' <returns> true if condition satisfied, false otherwise </returns>
		Function condition(ByVal list As IList(Of Writable)) As Boolean

		''' <summary>
		''' Condition on arbitrary input </summary>
		''' <param name="input"> the input to return
		'''              the condition for </param>
		''' <returns> true if the condition is met
		''' false otherwise </returns>
		Function condition(ByVal input As Object) As Boolean

		''' <summary>
		''' Is the condition satisfied for the current input/sequence?<br>
		''' Returns true if condition is satisfied, or false otherwise.
		''' </summary>
		''' <param name="sequence"> Current sequence </param>
		''' <returns> true if condition satisfied, false otherwise </returns>
		Function conditionSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean

		''' <summary>
		''' Condition on arbitrary input </summary>
		''' <param name="sequence"> the sequence to
		'''                 do a condition on </param>
		''' <returns> true if the condition for the sequence is met false otherwise </returns>
		Function conditionSequence(ByVal sequence As Object) As Boolean


		''' <summary>
		''' Setter for the input schema </summary>
		''' <param name="schema"> </param>
		Property InputSchema As Schema



	End Interface

End Namespace