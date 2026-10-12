Imports System.Collections.Generic
Imports System.Text
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function
Imports org.nd4j.common.primitives

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

Namespace org.datavec.local.transforms.reduce



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class MapToPairForReducerFunction implements org.nd4j.common.function.@Function<java.util.List<org.datavec.api.writable.Writable>, org.nd4j.common.primitives.Pair<String, java.util.List<org.datavec.api.writable.Writable>>>
	Public Class MapToPairForReducerFunction
		Implements [Function](Of IList(Of Writable), Pair(Of String, IList(Of Writable)))

		Public Const GLOBAL_KEY As String = ""

		Private ReadOnly reducer As IAssociativeReducer

		Public Overridable Function apply(ByVal writables As IList(Of Writable)) As Pair(Of String, IList(Of Writable))
			Dim keyColumns As IList(Of String) = reducer.getKeyColumns()

			If keyColumns Is Nothing Then
				'Global reduction
				Return Pair.of(GLOBAL_KEY, writables)
			Else
				Dim schema As Schema = reducer.InputSchema
				Dim key As String
				If keyColumns.Count = 1 Then
					key = writables(schema.getIndexOfColumn(keyColumns(0))).ToString()
				Else
					Dim sb As New StringBuilder()
					For i As Integer = 0 To keyColumns.Count - 1
						If i > 0 Then
							sb.Append("_")
						End If
						sb.Append(writables(schema.getIndexOfColumn(keyColumns(i))).ToString())
					Next i
					key = sb.ToString()
				End If

				Return Pair.of(key, writables)
			End If
		End Function
	End Class

End Namespace