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


	Public Class PythonException
		Inherits Exception

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		Private Shared Function getExceptionString(ByVal exception As PythonObject) As String
			Try
					Using gc As PythonGC = PythonGC.watch()
					If Python.isinstance(exception, Python.ExceptionType()) Then
						Dim exceptionClass As String = Python.type(exception).attr("__name__").ToString()
						Dim message As String = exception.ToString()
						Return exceptionClass & ": " & message
					End If
					Return exception.ToString()
					End Using
			Catch e As Exception
				Throw New Exception("An error occurred while trying to create a PythonException.", e)
			End Try
		End Function

		Public Sub New(ByVal exception As PythonObject)
			Me.New(getExceptionString(exception))
		End Sub

		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub

		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace