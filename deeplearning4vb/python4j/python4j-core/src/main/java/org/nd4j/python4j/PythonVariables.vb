Imports System.Collections.Generic

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

Namespace org.nd4j.python4j


	''' <summary>
	''' Some syntax sugar for lookup by name
	''' </summary>
	Public Class PythonVariables
		Inherits List(Of PythonVariable)

		Public Overridable Function get(ByVal variableName As String) As PythonVariable
			For Each pyVar As PythonVariable In Me
				If pyVar.getName().Equals(variableName) Then
					Return pyVar
				End If
			Next pyVar
			Return Nothing
		End Function

		Public Overridable Function add(Of T)(ByVal variableName As String, ByVal variableType As PythonType(Of T), ByVal value As Object) As Boolean
			Return Me.Add(New PythonVariable(Of )(variableName, variableType, value))
		End Function

		Public Sub New(ParamArray ByVal variables() As PythonVariable)
			Me.New(Arrays.asList(variables))
		End Sub
		Public Sub New(ByVal list As IList(Of PythonVariable))
			MyBase.New()
			Me.AddRange(list)
		End Sub
	End Class

End Namespace