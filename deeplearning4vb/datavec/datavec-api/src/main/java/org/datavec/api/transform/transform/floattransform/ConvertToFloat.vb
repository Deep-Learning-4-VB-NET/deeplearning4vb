Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports FloatMetaData = org.datavec.api.transform.metadata.FloatMetaData
Imports FloatWritable = org.datavec.api.writable.FloatWritable
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

Namespace org.datavec.api.transform.transform.floattransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data public class ConvertToFloat extends BaseFloatTransform
	<Serializable>
	Public Class ConvertToFloat
		Inherits BaseFloatTransform

		''' <param name="column"> Name of the column to convert to a Float column </param>
		Public Sub New(ByVal column As String)
			MyBase.New(column)
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As FloatWritable
			If writable.getType() = WritableType.Double Then
				Return DirectCast(writable, FloatWritable)
			End If
			Return New FloatWritable(writable.toFloat())
		End Function


		Public Overrides Function map(ByVal input As Object) As Object
			If TypeOf input Is Number Then
				Return DirectCast(input, Number).floatValue()
			End If
			Return Single.Parse(input.ToString())
		End Function


		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New FloatMetaData(newColumnName)
		End Function

		Public Overrides Function ToString() As String
			Return "ConvertToFloat(columnName=" & columnName_Conflict & ")"
		End Function
	End Class

End Namespace