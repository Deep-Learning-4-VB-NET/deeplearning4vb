Imports System
Imports Data = lombok.Data
Imports org.datavec.api.writable

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
'ORIGINAL LINE: @Data public class InvalidValueColumnCondition extends BaseColumnCondition
	<Serializable>
	Public Class InvalidValueColumnCondition
		Inherits BaseColumnCondition


		Public Sub New(ByVal columnName As String)
			MyBase.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE)
		End Sub

		Public Overrides Function columnCondition(ByVal writable As Writable) As Boolean
			Return Not schema.getMetaData(columnIdx).isValid(writable)
		End Function

		Public Overrides Function ToString() As String
			Return "InvalidValueColumnCondition(columnName=""" & columnName_Conflict & """)"
		End Function

		''' <summary>
		''' Condition on arbitrary input
		''' </summary>
		''' <param name="input"> the input to return
		'''              the condition for </param>
		''' <returns> true if the condition is met
		''' false otherwise </returns>
		Public Overrides Function condition(ByVal input As Object) As Boolean
			If TypeOf input Is String Then
				Return Not schema.getMetaData(columnIdx).isValid(New Text(input.ToString()))
			ElseIf TypeOf input Is Double? Then
				Dim d As Double? = DirectCast(input, Double?)
				Return Not schema.getMetaData(columnIdx).isValid(New DoubleWritable(d))
			ElseIf TypeOf input Is Integer? Then
				Dim i As Integer? = DirectCast(input, Integer?)
				Return Not schema.getMetaData(columnIdx).isValid(New IntWritable(i))
			ElseIf TypeOf input Is Long? Then
				Dim l As Long? = DirectCast(input, Long?)
				Return Not schema.getMetaData(columnIdx).isValid(New LongWritable(l))
			ElseIf TypeOf input Is Boolean? Then
				Dim b As Boolean? = DirectCast(input, Boolean?)
				Return Not schema.getMetaData(columnIdx).isValid(New BooleanWritable(b))

			Else
				Throw New System.InvalidOperationException("Illegal type " & input.GetType().ToString())
			End If
		End Function
	End Class

End Namespace