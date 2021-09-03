Imports System
Imports System.Collections.Generic
Imports System.Linq

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

Namespace org.deeplearning4j.models.word2vec


	<Serializable>
	Public Class VocabWork

'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private count_Conflict As New AtomicInteger(0)
'JAVA TO VB CONVERTER NOTE: The field work was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private work_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field stem was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private stem_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field label was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private label_Conflict As IList(Of String)


		Public Sub New(ByVal count As AtomicInteger, ByVal work As String, ByVal stem As Boolean)
			Me.New(count, work, stem, "")
		End Sub



		Public Sub New(ByVal count As AtomicInteger, ByVal work As String, ByVal stem As Boolean, ByVal label As String)
			Me.New(count, work, stem, Arrays.asList(label))
		End Sub

		Public Sub New(ByVal count As AtomicInteger, ByVal work As String, ByVal stem As Boolean, ByVal label As IList(Of String))
			Me.count_Conflict = count
			Me.work_Conflict = work
			Me.stem_Conflict = stem
			Me.label_Conflict = label
		End Sub

		Public Overridable Property Count As AtomicInteger
			Get
				Return count_Conflict
			End Get
			Set(ByVal count As AtomicInteger)
				Me.count_Conflict = count
			End Set
		End Property


		Public Overridable Property Work As String
			Get
				Return work_Conflict
			End Get
			Set(ByVal work As String)
				Me.work_Conflict = work
			End Set
		End Property


		Public Overridable Sub increment()
			count_Conflict.incrementAndGet()
		End Sub

		Public Overridable Property Stem As Boolean
			Get
				Return stem_Conflict
			End Get
			Set(ByVal stem As Boolean)
				Me.stem_Conflict = stem
			End Set
		End Property


		Public Overridable Property Label As IList(Of String)
			Get
				Return label_Conflict
			End Get
			Set(ByVal label As IList(Of String))
				Me.label_Conflict = label
			End Set
		End Property


		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is VocabWork) Then
				Return False
			End If

			Dim vocabWork As VocabWork = DirectCast(o, VocabWork)

			If stem_Conflict <> vocabWork.stem_Conflict Then
				Return False
			End If
			If If(count_Conflict IsNot Nothing, Not count_Conflict.Equals(vocabWork.count_Conflict), vocabWork.count_Conflict IsNot Nothing) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (label != null ? !label.equals(vocabWork.label) : vocabWork.label != null)
			If If(label_Conflict IsNot Nothing, Not label_Conflict.SequenceEqual(vocabWork.label_Conflict), vocabWork.label_Conflict IsNot Nothing) Then
				Return False
			End If
			Return Not (If(work_Conflict IsNot Nothing, Not work_Conflict.Equals(vocabWork.work_Conflict), vocabWork.work_Conflict IsNot Nothing))

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = If(count_Conflict IsNot Nothing, count_Conflict.GetHashCode(), 0)
			result = 31 * result + (If(work_Conflict IsNot Nothing, work_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(stem_Conflict, 1, 0))
			result = 31 * result + (If(label_Conflict IsNot Nothing, label_Conflict.GetHashCode(), 0))
			Return result
		End Function
	End Class

End Namespace