﻿Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports Text = org.datavec.api.writable.Text
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

Namespace org.datavec.api.transform.transform.string


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor public abstract class BaseStringTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public MustInherit Class BaseStringTransform
		Inherits BaseColumnTransform

		Public Sub New(ByVal column As String)
			MyBase.New(column)
		End Sub

		''' <summary>
		''' Transform the writable in to a
		''' string </summary>
		''' <param name="writable"> the writable to transform </param>
		''' <returns> the string form of this writable </returns>
		Public Overrides MustOverride Function map(ByVal writable As Writable) As Text

		Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New StringMetaData(newName)
		End Function
	End Class

End Namespace