Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports WritableType = org.datavec.api.writable.WritableType

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

Namespace org.datavec.api.transform.transform.integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class ConvertToInteger extends BaseIntegerTransform
	<Serializable>
	Public Class ConvertToInteger
		Inherits BaseIntegerTransform

		''' 
		''' <param name="column"> Name of the column to convert to an integer </param>
		Public Sub New(ByVal column As String)
			MyBase.New(column)
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As IntWritable
			If writable.getType() = WritableType.Int Then
				Return DirectCast(writable, IntWritable)
			End If
			Return New IntWritable(writable.toInt())
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			If TypeOf input Is Number Then
				Return DirectCast(input, Number).intValue()
			End If
			Return Integer.Parse(input.ToString())
		End Function


		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New IntegerMetaData(newColumnName)
		End Function

		Public Overrides Function ToString() As String
			Return "ConvertToInteger(columnName=" & columnName_Conflict & ")"
		End Function
	End Class

End Namespace