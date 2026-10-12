Imports System
Imports System.Collections.Generic
Imports System.Text
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType

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

Namespace org.datavec.api.transform.analysis.columns


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor public class CategoricalAnalysis implements ColumnAnalysis
	<Serializable>
	Public Class CategoricalAnalysis
		Implements ColumnAnalysis

		Private mapOfCounts As IDictionary(Of String, Long)


		Public Overrides Function ToString() As String
			'Returning the counts from highest to lowest here, which seems like a useful default
			Dim keys As IList(Of String) = New List(Of String)(mapOfCounts.Keys)
			keys.Sort(New ComparatorAnonymousInnerClass(Me))

			Dim sb As New StringBuilder()
			sb.Append("CategoricalAnalysis(CategoryCounts={")
			Dim first As Boolean = True
			For Each s As String In keys
				If Not first Then
					sb.Append(", ")
				End If
				first = False

				sb.Append(s).Append("=").Append(mapOfCounts(s))
			Next s
			sb.Append("})")

			Return sb.ToString()
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of String)

			Private ReadOnly outerInstance As CategoricalAnalysis

			Public Sub New(ByVal outerInstance As CategoricalAnalysis)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As String, ByVal o2 As String) As Integer Implements IComparer(Of String).Compare
				Return -Long.compare(outerInstance.mapOfCounts(o1), outerInstance.mapOfCounts(o2)) 'Highest to lowest
			End Function
		End Class

		Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.getCountTotal
			Get
				Dim counts As IDictionary(Of String, Long).ValueCollection = mapOfCounts.Values
				Dim sum As Long = 0
				For Each l As Long? In counts
					sum += l
				Next l
				Return sum
			End Get
		End Property

		Public Overridable ReadOnly Property ColumnType As ColumnType Implements ColumnAnalysis.getColumnType
			Get
				Return ColumnType.Categorical
			End Get
		End Property
	End Class

End Namespace