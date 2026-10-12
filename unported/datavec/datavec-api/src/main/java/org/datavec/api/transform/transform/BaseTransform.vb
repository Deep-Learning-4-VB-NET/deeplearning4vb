Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Transform = org.datavec.api.transform.Transform
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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

Namespace org.datavec.api.transform.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema"}) @Data public abstract class BaseTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public MustInherit Class BaseTransform
		Implements Transform

		Public MustOverride Function columnName() As String
		Public MustOverride Function columnNames() As String()
		Public MustOverride Function outputColumnNames() As String()
		Public MustOverride Function outputColumnName() As String
		Public MustOverride Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
		Public MustOverride Function map(ByVal input As Object) As Object Implements Transform.map
		Public MustOverride Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)

'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputSchema_Conflict As Schema

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(sequence.Count)
			For Each c As IList(Of Writable) In sequence
				[out].Add(map(c))
			Next c
			Return [out]
		End Function

		Public MustOverride Overrides Function ToString() As String
	End Class

End Namespace