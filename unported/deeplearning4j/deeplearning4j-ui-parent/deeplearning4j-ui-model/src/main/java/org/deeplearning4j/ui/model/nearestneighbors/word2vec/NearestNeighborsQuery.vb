Imports System

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

Namespace org.deeplearning4j.ui.model.nearestneighbors.word2vec


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class NearestNeighborsQuery
'JAVA TO VB CONVERTER NOTE: The field word was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private word_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field numWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numWords_Conflict As Integer

		Public Sub New(ByVal word As String, ByVal numWords As Integer)
			Me.word_Conflict = word
			Me.numWords_Conflict = numWords
		End Sub

		Public Sub New()
		End Sub

		Public Overridable Property Word As String
			Get
				Return word_Conflict
			End Get
			Set(ByVal word As String)
				Me.word_Conflict = word
			End Set
		End Property


		Public Overridable Property NumWords As Integer
			Get
				Return numWords_Conflict
			End Get
			Set(ByVal numWords As Integer)
				Me.numWords_Conflict = numWords
			End Set
		End Property


		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As NearestNeighborsQuery = DirectCast(o, NearestNeighborsQuery)

			If numWords_Conflict <> that.numWords_Conflict Then
				Return False
			End If
			Return Not (If(word_Conflict IsNot Nothing, Not word_Conflict.Equals(that.word_Conflict), that.word_Conflict IsNot Nothing))

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = If(word_Conflict IsNot Nothing, word_Conflict.GetHashCode(), 0)
			result = 31 * result + numWords_Conflict
			Return result
		End Function
	End Class

End Namespace