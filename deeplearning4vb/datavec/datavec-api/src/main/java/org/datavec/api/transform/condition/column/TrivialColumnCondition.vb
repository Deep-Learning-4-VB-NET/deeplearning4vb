Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.datavec.api.transform.condition.column


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"schema"}) @Data public class TrivialColumnCondition extends BaseColumnCondition
	<Serializable>
	Public Class TrivialColumnCondition
		Inherits BaseColumnCondition

		Private Shadows schema As Schema

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TrivialColumnCondition(@JsonProperty("name") String name)
		Public Sub New(ByVal name As String)
			MyBase.New(name, DEFAULT_SEQUENCE_CONDITION_MODE)
		End Sub

		Public Overrides Function ToString() As String
			Return "Trivial(" & MyBase.columnName_Conflict & ")"
		End Function

		Public Overrides Function columnCondition(ByVal writable As Writable) As Boolean
			Return True
		End Function

		Public Overridable Overloads Function condition(ByVal writables As IList(Of Writable)) As Boolean
			Return True
		End Function

		Public Overrides Function condition(ByVal input As Object) As Boolean
			Return True
		End Function
	End Class

End Namespace