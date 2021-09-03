Imports System
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
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

Namespace org.datavec.api.transform.metadata

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class StringMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class StringMetaData
		Inherits BaseColumnMetaData

		'regex + min/max length are nullable: null -> no restrictions on these
		Private ReadOnly regex As String
		Private ReadOnly minLength As Integer?
		Private ReadOnly maxLength As Integer?

		Public Sub New()
			MyBase.New(Nothing)
			regex = Nothing
			minLength = Nothing
			maxLength = Nothing
		End Sub

		''' <summary>
		''' Default constructor with no restrictions on allowable strings
		''' </summary>
		Public Sub New(ByVal name As String)
			Me.New(name, Nothing, Nothing, Nothing)
		End Sub

		''' <param name="mustMatchRegex"> Nullable. If not null: this is a regex that each string must match in order for the entry
		'''                       to be considered valid. </param>
		''' <param name="minLength">      Min allowable String length. If null: no restriction on min String length </param>
		''' <param name="maxLength">      Max allowable String length. If null: no restriction on max String length </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringMetaData(@JsonProperty("name") String name, @JsonProperty("regex") String mustMatchRegex, @JsonProperty("minLength") System.Nullable<Integer> minLength, @JsonProperty("maxLength") System.Nullable<Integer> maxLength)
		Public Sub New(ByVal name As String, ByVal mustMatchRegex As String, ByVal minLength As Integer?, ByVal maxLength As Integer?)
			MyBase.New(name)
			Me.regex = mustMatchRegex
			Me.minLength = minLength
			Me.maxLength = maxLength
		End Sub


		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.String
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			Dim str As String = writable.ToString()
			Dim len As Integer = str.Length
			If minLength IsNot Nothing AndAlso len < minLength Then
				Return False
			End If
			If maxLength IsNot Nothing AndAlso len > maxLength Then
				Return False
			End If

			Return regex Is Nothing OrElse str.matches(regex)
		End Function

		''' <summary>
		''' Is the given object valid for this column,
		''' given the column type and any
		''' restrictions given by the
		''' ColumnMetaData object?
		''' </summary>
		''' <param name="input"> object to check </param>
		''' <returns> true if value, false if invalid </returns>
		Public Overrides Function isValid(ByVal input As Object) As Boolean
			Dim str As String = input.ToString()
			Dim len As Integer = str.Length
			If minLength IsNot Nothing AndAlso len < minLength Then
				Return False
			End If
			If maxLength IsNot Nothing AndAlso len > maxLength Then
				Return False
			End If

			Return regex Is Nothing OrElse str.matches(regex)
		End Function

		Public Overrides Function clone() As StringMetaData
			Return New StringMetaData(name_Conflict, regex, minLength, maxLength)
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("StringMetaData(name=""").Append(name_Conflict).Append(""",")
			If minLength IsNot Nothing Then
				sb.Append("minLengthAllowed=").Append(minLength)
			End If
			If maxLength IsNot Nothing Then
				If minLength IsNot Nothing Then
					sb.Append(",")
				End If
				sb.Append("maxLengthAllowed=").Append(maxLength)
			End If
			If regex IsNot Nothing Then
				If minLength IsNot Nothing OrElse maxLength IsNot Nothing Then
					sb.Append(",")
				End If
				sb.Append("regex=").Append(regex)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function

	End Class

End Namespace