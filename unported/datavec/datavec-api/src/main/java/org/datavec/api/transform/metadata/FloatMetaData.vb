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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class FloatMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class FloatMetaData
		Inherits BaseColumnMetaData

		'minAllowedValue/maxAllowedValue are nullable: null -> no restriction on minAllowedValue/maxAllowedValue values
		Private ReadOnly minAllowedValue As Single?
		Private ReadOnly maxAllowedValue As Single?
		Private ReadOnly allowNaN As Boolean
		Private ReadOnly allowInfinite As Boolean

		Public Sub New(ByVal name As String)
			Me.New(name, Nothing, Nothing, False, False)
		End Sub

		''' <param name="minAllowedValue"> Min allowed value. If null: no restriction on minAllowedValue value in this column </param>
		''' <param name="maxAllowedValue"> Max allowed value. If null: no restriction on maxAllowedValue value in this column </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FloatMetaData(@JsonProperty("name") String name, @JsonProperty("minAllowedValue") System.Nullable<Single> minAllowedValue, @JsonProperty("maxAllowedValue") System.Nullable<Single> maxAllowedValue)
		Public Sub New(ByVal name As String, ByVal minAllowedValue As Single?, ByVal maxAllowedValue As Single?)
			Me.New(name, minAllowedValue, maxAllowedValue, False, False)
		End Sub

		''' <param name="min">           Min allowed value. If null: no restriction on minAllowedValue value in this column </param>
		''' <param name="maxAllowedValue">           Max allowed value. If null: no restriction on maxAllowedValue value in this column </param>
		''' <param name="allowNaN">      Are NaN values ok? </param>
		''' <param name="allowInfinite"> Are +/- infinite values ok? </param>
		Public Sub New(ByVal name As String, ByVal min As Single?, ByVal maxAllowedValue As Single?, ByVal allowNaN As Boolean, ByVal allowInfinite As Boolean)
			MyBase.New(name)
			Me.minAllowedValue = min
			Me.maxAllowedValue = maxAllowedValue
			Me.allowNaN = allowNaN
			Me.allowInfinite = allowInfinite
		End Sub

		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.Float
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			Dim d As Single?
			Try
				d = writable.toFloat()
			Catch e As Exception
				Return False
			End Try

			If allowNaN AndAlso Single.IsNaN(d) Then
				Return True
			End If
			If allowInfinite AndAlso Single.IsInfinity(d) Then
				Return True
			End If

			If minAllowedValue IsNot Nothing AndAlso d < minAllowedValue Then
				Return False
			End If
			If maxAllowedValue IsNot Nothing AndAlso d > maxAllowedValue Then
				Return False
			End If

			Return True
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
			Dim d As Single?
			Try
				d = DirectCast(input, Single?)
			Catch e As Exception
				Return False
			End Try

			If allowNaN AndAlso Single.IsNaN(d) Then
				Return True
			End If
			If allowInfinite AndAlso Single.IsInfinity(d) Then
				Return True
			End If

			If minAllowedValue IsNot Nothing AndAlso d < minAllowedValue Then
				Return False
			End If
			If maxAllowedValue IsNot Nothing AndAlso d > maxAllowedValue Then
				Return False
			End If

			Return True
		End Function

		Public Overrides Function clone() As FloatMetaData
			Return New FloatMetaData(name_Conflict, minAllowedValue, maxAllowedValue, allowNaN, allowInfinite)
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("FloatMetaData(name=""").Append(name_Conflict).Append(""",")
			Dim needComma As Boolean = False
			If minAllowedValue IsNot Nothing Then
				sb.Append("minAllowed=").Append(minAllowedValue)
				needComma = True
			End If
			If maxAllowedValue IsNot Nothing Then
				If needComma Then
					sb.Append(",")
				End If
				sb.Append("maxAllowed=").Append(maxAllowedValue)
				needComma = True
			End If
			If needComma Then
				sb.Append(",")
			End If
			sb.Append("allowNaN=").Append(allowNaN).Append(",allowInfinite=").Append(allowInfinite).Append(")")
			Return sb.ToString()
		End Function
	End Class

End Namespace