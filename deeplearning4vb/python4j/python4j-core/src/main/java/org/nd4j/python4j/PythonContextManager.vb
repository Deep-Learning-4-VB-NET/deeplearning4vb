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

Namespace org.nd4j.python4j



	Public Class PythonContextManager

'JAVA TO VB CONVERTER NOTE: The field contexts was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared contexts_Conflict As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER NOTE: The field init was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared init_Conflict As New AtomicBoolean(False)
'JAVA TO VB CONVERTER NOTE: The field currentContext was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared currentContext_Conflict As String
		Private Const MAIN_CONTEXT As String = "main"
		Private Const COLLAPSED_KEY As String = "__collapsed__"

		Shared Sub New()
			init()
		End Sub


		Public Class Context
			Implements System.IDisposable

			Friend ReadOnly name As String
			Friend ReadOnly previous As String
			Friend ReadOnly temp As Boolean
			Public Sub New()
				name = "temp_" & System.Guid.randomUUID().ToString().Replace("-", "_")
				temp = True
				previous = CurrentContext
				Context = name
			End Sub
			Public Sub New(ByVal name As String)
			   Me.name = name
			   temp = False
				previous = CurrentContext
				Context = name
			End Sub

			Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
				Context = previous
				If temp Then
					deleteContext(name)
				End If
			End Sub
		End Class

		Private Shared Sub init()
			If init_Conflict.get() Then
				Return
			End If
			Dim tempVar As New PythonExecutioner()
			init_Conflict.set(True)
			currentContext_Conflict = MAIN_CONTEXT
			contexts_Conflict.Add(currentContext_Conflict)
		End Sub


		''' <summary>
		''' Adds a new context. </summary>
		''' <param name="contextName"> </param>
		Public Shared Sub addContext(ByVal contextName As String)
			If Not validateContextName(contextName) Then
				Throw New PythonException("Invalid context name: " & contextName)
			End If
			contexts_Conflict.Add(contextName)
		End Sub

		''' <summary>
		''' Returns true if context exists, else false. </summary>
		''' <param name="contextName">
		''' @return </param>
		Public Shared Function hasContext(ByVal contextName As String) As Boolean
			Return contexts_Conflict.Contains(contextName)
		End Function

		Private Shared Function validateContextName(ByVal s As String) As Boolean
			For i As Integer = 0 To s.Length - 1
				Dim c As Char = s.ToLower().Chars(i)
				If i = 0 Then
					If c >= "0"c AndAlso c <= "9"c Then
						Return False
					End If
				End If
				If Not (c="_"c OrElse (c >= "a"c AndAlso c <= "z"c) OrElse (c >= "0"c AndAlso c <= "9"c)) Then
					Return False
				End If
			Next i
			Return True
		End Function

		Private Shared Function getContextPrefix(ByVal contextName As String) As String
			Return COLLAPSED_KEY & contextName & "__"
		End Function

		Private Shared Function getCollapsedVarNameForContext(ByVal varName As String, ByVal contextName As String) As String
			Return getContextPrefix(contextName) & varName
		End Function

		Private Shared Function expandCollapsedVarName(ByVal varName As String, ByVal contextName As String) As String
			Dim prefix As String = COLLAPSED_KEY & contextName & "__"
			Return varName.Substring(prefix.Length)

		End Function

		Private Shared Sub collapseContext(ByVal contextName As String)
	'JAVA TO VB CONVERTER NOTE: An underscore by itself is not a valid identifier in VB:
	'ORIGINAL LINE: try (PythonGC _ = PythonGC.watch())
			Try
					Using underscore As PythonGC = PythonGC.watch()
					Dim globals As PythonObject = Python.globals()
					Dim pop As PythonObject = globals.attr("pop")
					Dim keysF As PythonObject = globals.attr("keys")
					Dim keys As PythonObject = keysF.call()
					Dim keysList As PythonObject = Python.list(keys)
					Dim numKeys As Integer = Python.len(keysList).toInt()
					For i As Integer = 0 To numKeys - 1
						Dim key As PythonObject = keysList.get(i)
						Dim keyStr As String = key.ToString()
						If Not ((keyStr.StartsWith("__", StringComparison.Ordinal) AndAlso keyStr.EndsWith("__", StringComparison.Ordinal)) OrElse keyStr.StartsWith("__collapsed_", StringComparison.Ordinal)) Then
							Dim collapsedKey As String = getCollapsedVarNameForContext(keyStr, contextName)
							Dim val As PythonObject = pop.call(key)
        
							Dim pyNewKey As New PythonObject(collapsedKey)
							globals.set(pyNewKey, val)
						End If
					Next i
					End Using
			Catch pe As Exception
				Throw New Exception(pe)
			End Try
		End Sub

		Private Shared Sub expandContext(ByVal contextName As String)
	'JAVA TO VB CONVERTER NOTE: An underscore by itself is not a valid identifier in VB:
	'ORIGINAL LINE: try (PythonGC _ = PythonGC.watch())
			Using underscore As PythonGC = PythonGC.watch()
				Dim prefix As String = getContextPrefix(contextName)
				Dim globals As PythonObject = Python.globals()
				Dim pop As PythonObject = globals.attr("pop")
				Dim keysF As PythonObject = globals.attr("keys")

				Dim keys As PythonObject = keysF.call()

				Dim keysList As PythonObject = Python.list(keys)
				Using __ As PythonGC = PythonGC.pause()
					Dim numKeys As Integer = Python.len(keysList).toInt()

					For i As Integer = 0 To numKeys - 1
						Dim key As PythonObject = keysList.get(i)
						Dim keyStr As String = key.ToString()
						If keyStr.StartsWith(prefix, StringComparison.Ordinal) Then
							Dim expandedKey As String = expandCollapsedVarName(keyStr, contextName)
							Dim val As PythonObject = pop.call(key)
							Dim newKey As New PythonObject(expandedKey)
							globals.set(newKey, val)
						End If
					Next i
				End Using
			End Using
		End Sub


		''' <summary>
		''' Activates the specified context </summary>
		''' <param name="contextName"> </param>
		Public Shared WriteOnly Property Context As String
			Set(ByVal contextName As String)
				If contextName.Equals(currentContext_Conflict) Then
					Return
				End If
				If Not hasContext(contextName) Then
					addContext(contextName)
				End If
    
    
				collapseContext(currentContext_Conflict)
    
				expandContext(contextName)
				currentContext_Conflict = contextName
    
			End Set
		End Property

		''' <summary>
		''' Activates the main context
		''' </summary>
		Public Shared Sub setMainContext()
			Context = MAIN_CONTEXT

		End Sub

		''' <summary>
		''' Returns the current context's name.
		''' @return
		''' </summary>
		Public Shared ReadOnly Property CurrentContext As String
			Get
				Return currentContext_Conflict
			End Get
		End Property

		''' <summary>
		''' Resets the current context.
		''' </summary>
		Public Shared Sub reset()
			Dim tempContext As String = "___temp__context___"
			Dim currContext As String = currentContext_Conflict
			Context = tempContext
			deleteContext(currContext)
			Context = currContext
			deleteContext(tempContext)
		End Sub

		''' <summary>
		''' Deletes the specified context. </summary>
		''' <param name="contextName"> </param>
		Public Shared Sub deleteContext(ByVal contextName As String)
			If contextName.Equals(currentContext_Conflict) Then
				Throw New PythonException("Cannot delete current context!")
			End If
			If Not contexts_Conflict.Contains(contextName) Then
				Return
			End If
			Dim prefix As String = getContextPrefix(contextName)
			Dim globals As PythonObject = Python.globals()
			Dim keysList As PythonObject = Python.list(globals.attr("keys").call())
			Dim numKeys As Integer = Python.len(keysList).toInt()
			For i As Integer = 0 To numKeys - 1
				Dim key As PythonObject = keysList.get(i)
				Dim keyStr As String = key.ToString()
				If keyStr.StartsWith(prefix, StringComparison.Ordinal) Then
					globals.attr("__delitem__").call(key)
				End If
			Next i
			contexts_Conflict.remove(contextName)
		End Sub

		''' <summary>
		''' Deletes all contexts except the main context.
		''' </summary>
		Public Shared Sub deleteNonMainContexts()
			Context = MAIN_CONTEXT ' will never fail
			For Each c As String In contexts_Conflict.ToArray()
				If Not c.Equals(MAIN_CONTEXT) Then
					deleteContext(c) ' will never fail
				End If
			Next c

		End Sub

		''' <summary>
		''' Returns the names of all contexts.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Contexts As String()
			Get
				Return contexts_Conflict.ToArray()
			End Get
		End Property

	End Class

End Namespace