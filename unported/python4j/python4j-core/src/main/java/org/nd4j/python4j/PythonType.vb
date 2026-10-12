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

Namespace org.nd4j.python4j



	Public MustInherit Class PythonType(Of T)

'JAVA TO VB CONVERTER NOTE: The field name was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly name_Conflict As String
		Private ReadOnly javaType As Type(Of T)

		Public Sub New(ByVal name As String, ByVal javaType As Type(Of T))
			Me.name_Conflict = name
			Me.javaType = javaType
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public T adapt(Object javaObject) throws PythonException
		Public Overridable Function adapt(ByVal javaObject As Object) As T
			Return DirectCast(javaObject, T)
		End Function

		Public MustOverride Function toJava(ByVal pythonObject As PythonObject) As T

		Public MustOverride Function toPython(ByVal javaObject As T) As PythonObject

		Public Overridable Function accepts(ByVal javaObject As Object) As Boolean
			Return javaType.IsAssignableFrom(javaObject.GetType())
		End Function

		Public Overridable ReadOnly Property Name As String
			Get
				Return name_Conflict
			End Get
		End Property

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not (TypeOf obj Is PythonType) Then
				Return False
			End If
			Dim other As PythonType = DirectCast(obj, PythonType)
			Return Me.GetType().Equals(other.GetType()) AndAlso Me.name_Conflict.Equals(other.name_Conflict)
		End Function

		Public Overridable Function pythonType() As PythonObject
			Return Nothing
		End Function

		Public Overridable Function packages() As File()
			Return New File(){}
		End Function

		Public Overridable Sub init() 'not to be called from constructor

		End Sub

	End Class

End Namespace