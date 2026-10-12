Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports DateTimeFormat = org.joda.time.format.DateTimeFormat
Imports DateTimeFormatter = org.joda.time.format.DateTimeFormatter

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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor public class TimeAnalysis extends NumericalColumnAnalysis
	<Serializable>
	Public Class TimeAnalysis
		Inherits NumericalColumnAnalysis

		Private Shared ReadOnly formatter As DateTimeFormatter = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss.SSS zzz").withZone(DateTimeZone.UTC)
		Private min As Long
		Private max As Long

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.min = builder.min_Conflict
			Me.max = builder.max_Conflict
		End Sub

		Public Overrides Function ToString() As String
			Return "TimeAnalysis(min=" & min & " (" & formatter.print(min) & "),max=" & max & " (" & formatter.print(max) & ")," & MyBase.ToString() & ")"
		End Function

		Public Overrides ReadOnly Property MinDouble As Double
			Get
				Return min
			End Get
		End Property

		Public Overrides ReadOnly Property MaxDouble As Double
			Get
				Return max
			End Get
		End Property

		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.Long
			End Get
		End Property

		Public Class Builder
			Inherits NumericalColumnAnalysis.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field min was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend min_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field max was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend max_Conflict As Long

'JAVA TO VB CONVERTER NOTE: The parameter min was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function min(ByVal min_Conflict As Long) As Builder
				Me.min_Conflict = min_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter max was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function max(ByVal max_Conflict As Long) As Builder
				Me.max_Conflict = max_Conflict
				Return Me
			End Function

			Public Overridable Function build() As TimeAnalysis
				Return New TimeAnalysis(Me)
			End Function
		End Class

	End Class

End Namespace