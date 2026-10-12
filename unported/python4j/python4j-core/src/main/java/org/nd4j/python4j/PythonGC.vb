Imports System
Imports System.Collections.Generic
Imports PyObject = org.bytedeco.cpython.PyObject
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports org.bytedeco.cpython.global.python

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


	Public Class PythonGC
		Implements System.IDisposable

		Private previousFrame As PythonGC = Nothing
		Private active As Boolean = True
		Private Shared currentFrame As New PythonGC()

		Private objects As ISet(Of PyObject) = New HashSet(Of PyObject)()

		Private Function alreadyRegistered(ByVal pyObject As PyObject) As Boolean
			If objects.Contains(pyObject) Then
				Return True
			End If
			If previousFrame Is Nothing Then
				Return False
			End If
			Return previousFrame.alreadyRegistered(pyObject)

		End Function

		Private Sub addObject(ByVal pythonObject As PythonObject)
			If Not active Then
				Return
			End If
			If Pointer.isNull(pythonObject.NativePythonObject) Then
				Return
			End If
			If alreadyRegistered(pythonObject.NativePythonObject) Then
				Return
			End If
			objects.Add(pythonObject.NativePythonObject)
		End Sub

		Public Shared Sub register(ByVal pythonObject As PythonObject)
			currentFrame.addObject(pythonObject)
		End Sub

		Public Shared Sub keep(ByVal pythonObject As PythonObject)
			currentFrame.objects.remove(pythonObject.NativePythonObject)
			If currentFrame.previousFrame IsNot Nothing Then
				currentFrame.previousFrame.addObject(pythonObject)
			End If
		End Sub

		Private Sub New()
		End Sub

		Public Shared Function watch() As PythonGC
			Dim ret As New PythonGC()
			ret.previousFrame = currentFrame
			ret.active = currentFrame.active
			currentFrame = ret
			Return ret
		End Function

		Private Sub collect()
			For Each pyObject As PyObject In objects
				' TODO find out how globals gets collected here
				If pyObject.Equals(Python.globals().NativePythonObject) Then
					Continue For
				End If
	'            try{
	'                System.out.println(PythonTypes.STR.toJava(new PythonObject(pyObject, false)));
	'            }catch (Exception e){}
				Py_DecRef(pyObject)

			Next pyObject
			Me.objects = New HashSet(Of PyObject)()
		End Sub

		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If active Then
				collect()
			End If
			currentFrame = previousFrame
		End Sub

		Public Shared ReadOnly Property Watching As Boolean
			Get
				If Not currentFrame.active Then
					Return False
				End If
				Return currentFrame.previousFrame IsNot Nothing
			End Get
		End Property

		Public Shared Function pause() As PythonGC
			Dim pausedFrame As New PythonGC()
			pausedFrame.active = False
			pausedFrame.previousFrame = currentFrame
			currentFrame = pausedFrame
			Return pausedFrame
		End Function

		Public Shared Sub [resume]()
			If currentFrame.active Then
				Throw New Exception("GC not paused!")
			End If
			currentFrame = currentFrame.previousFrame
		End Sub
	End Class

End Namespace