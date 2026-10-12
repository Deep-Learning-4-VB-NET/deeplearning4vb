Imports System.Collections.Generic
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

Namespace org.datavec.api.writable.comparator


	Public Class Comparators

		Private Sub New()
		End Sub

		Public Shared Function forType(ByVal type As WritableType) As IComparer(Of Writable)
			Return forType(type, True)
		End Function

		Public Shared Function forType(ByVal type As WritableType, ByVal ascending As Boolean) As IComparer(Of Writable)
			Dim c As IComparer(Of Writable)
			Select Case type.innerEnumValue
				Case SByte?, Int
					c = New IntWritableComparator()
				Case Double?
					c = New DoubleWritableComparator()
				Case Single?
					c = New FloatWritableComparator()
				Case Long?
					c = New LongWritableComparator()
				Case WritableType.InnerEnum.Text
					c = New TextWritableComparator()
				Case Else
					Throw New System.NotSupportedException("No built-in comparator for writable type: " & type)
			End Select
			If ascending Then
				Return c
			End If
			Return New ReverseComparator(Of Writable)(c)
		End Function
	End Class

End Namespace