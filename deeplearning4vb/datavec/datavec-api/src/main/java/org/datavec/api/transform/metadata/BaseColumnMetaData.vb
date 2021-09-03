Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.datavec.api.transform.metadata

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public abstract class BaseColumnMetaData implements ColumnMetaData
	<Serializable>
	Public MustInherit Class BaseColumnMetaData
		Implements ColumnMetaData

		Public MustOverride Function isValid(ByVal input As Object) As Boolean Implements ColumnMetaData.isValid
		Public MustOverride Function isValid(ByVal writable As org.datavec.api.writable.Writable) As Boolean Implements ColumnMetaData.isValid
		Public MustOverride ReadOnly Property ColumnType As org.datavec.api.transform.ColumnType Implements ColumnMetaData.getColumnType

'JAVA TO VB CONVERTER NOTE: The field name was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend name_Conflict As String

		Protected Friend Sub New(ByVal name As String)
			Me.name_Conflict = name
		End Sub

		Public Overridable Property Name As String Implements ColumnMetaData.getName
			Get
				Return name_Conflict
			End Get
			Set(ByVal name As String)
				Me.name_Conflict = name
			End Set
		End Property


		Public MustOverride Overrides Function clone() As ColumnMetaData Implements ColumnMetaData.clone
	End Class

End Namespace