Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Condition = org.datavec.api.transform.condition.Condition
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

Namespace org.datavec.api.transform.serde.testClasses


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data public class CustomCondition implements org.datavec.api.transform.condition.Condition
	<Serializable>
	Public Class CustomCondition
		Implements Condition

		Private someArg As String

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema
		End Function

		Public Overridable Function outputColumnName() As String
			Return Nothing
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return New String(){}
		End Function

		Public Overridable Function columnNames() As String()
			Return New String(){}
		End Function

		Public Overridable Function columnName() As String
			Return Nothing
		End Function

		Public Overridable Function condition(ByVal list As IList(Of Writable)) As Boolean Implements Condition.condition
			Return False
		End Function

		Public Overridable Function condition(ByVal input As Object) As Boolean Implements Condition.condition
			Return False
		End Function

		Public Overridable Function conditionSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean Implements Condition.conditionSequence
			Return False
		End Function

		Public Overridable Function conditionSequence(ByVal sequence As Object) As Boolean Implements Condition.conditionSequence
			Return False
		End Function

		Public Overridable Property InputSchema Implements Condition.setInputSchema As Schema
			Set(ByVal schema As Schema)
			End Set
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace