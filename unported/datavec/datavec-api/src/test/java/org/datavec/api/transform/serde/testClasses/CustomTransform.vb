Imports System
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
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

Namespace org.datavec.api.transform.serde.testClasses

	<Serializable>
	Public Class CustomTransform
		Inherits BaseColumnTransform

		Private ReadOnly someArg As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CustomTransform(@JsonProperty("columnName") String columnName, @JsonProperty("someArg") double someArg)
		Public Sub New(ByVal columnName As String, ByVal someArg As Double)
			MyBase.New(columnName)
			Me.someArg = someArg
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New DoubleMetaData(newName)
		End Function

		Public Overrides Function map(ByVal columnWritable As Writable) As Writable
			Return columnWritable
		End Function

		Public Overrides Function ToString() As String
			Return "CustomTransform()"
		End Function

		Public Overridable Overloads Function map(ByVal input As Object) As Object
			Return input
		End Function
	End Class

End Namespace