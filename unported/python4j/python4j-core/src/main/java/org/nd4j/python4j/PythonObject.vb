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



	Public Class PythonObject

		Shared Sub New()
			Dim tempVar As New PythonExecutioner()
		End Sub

		Private owned As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field nativePythonObject was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private nativePythonObject_Conflict As PyObject


		Public Sub New(ByVal nativePythonObject As PyObject, ByVal owned As Boolean)
			PythonGIL.assertThreadSafe()
			Me.nativePythonObject_Conflict = nativePythonObject
			Me.owned = owned
			If owned AndAlso nativePythonObject IsNot Nothing Then
				PythonGC.register(Me)
			End If
		End Sub

		Public Sub New(ByVal nativePythonObject As PyObject)
			PythonGIL.assertThreadSafe()
			Me.nativePythonObject_Conflict = nativePythonObject
			If nativePythonObject IsNot Nothing Then
				PythonGC.register(Me)
			End If

		End Sub

		Public Overridable ReadOnly Property NativePythonObject As PyObject
			Get
				Return nativePythonObject_Conflict
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return PythonTypes.STR.toJava(Me)

		End Function

		Public Overridable ReadOnly Property None As Boolean
			Get
				If nativePythonObject_Conflict Is Nothing OrElse Pointer.isNull(nativePythonObject_Conflict) Then
					Return True
				End If
		'JAVA TO VB CONVERTER NOTE: An underscore by itself is not a valid identifier in VB:
		'ORIGINAL LINE: try (PythonGC _ = PythonGC.pause())
				Using underscore As PythonGC = PythonGC.pause()
					Dim type As PythonObject = Python.type(Me)
					Dim ret As Boolean = Python.type(Me).ToString().Equals("<class 'NoneType'>") AndAlso ToString().Equals("None")
					Py_DecRef(type.nativePythonObject_Conflict)
					Return ret
				End Using
			End Get
		End Property

		Public Overridable Sub del()
			PythonGIL.assertThreadSafe()
			If owned AndAlso nativePythonObject_Conflict IsNot Nothing AndAlso Not PythonGC.Watching Then
				Py_DecRef(nativePythonObject_Conflict)
				nativePythonObject_Conflict = Nothing
			End If
		End Sub

		Public Overridable Function callWithArgs(ByVal args As PythonObject) As PythonObject
			Return callWithArgsAndKwargs(args, Nothing)
		End Function

		Public Overridable Function callWithKwargs(ByVal kwargs As PythonObject) As PythonObject
			If Not Python.callable(Me) Then
				Throw New PythonException("Object is not callable: " & ToString())
			End If
			Dim tuple As PyObject = PyTuple_New(0)
			Dim dict As PyObject = kwargs.nativePythonObject_Conflict
			If PyObject_IsInstance(dict, New PyObject(PyDict_Type())) <> 1 Then
				Throw New PythonException("Expected kwargs to be dict. Received: " & kwargs.ToString())
			End If
			Dim ret As New PythonObject(PyObject_Call(nativePythonObject_Conflict, tuple, dict))
			Py_DecRef(tuple)
			Return ret
		End Function

		Public Overridable Function callWithArgsAndKwargs(ByVal args As PythonObject, ByVal kwargs As PythonObject) As PythonObject
			PythonGIL.assertThreadSafe()
			Dim tuple As PyObject = Nothing
			Dim ownsTuple As Boolean = False
			Try
				If Not Python.callable(Me) Then
					Throw New PythonException("Object is not callable: " & ToString())
				End If

				If PyObject_IsInstance(args.nativePythonObject_Conflict, New PyObject(PyTuple_Type())) = 1 Then
					tuple = args.nativePythonObject_Conflict
				ElseIf PyObject_IsInstance(args.nativePythonObject_Conflict, New PyObject(PyList_Type())) = 1 Then
					tuple = PyList_AsTuple(args.nativePythonObject_Conflict)
					ownsTuple = True
				Else
					Throw New PythonException("Expected args to be tuple or list. Received: " & args.ToString())
				End If
				If kwargs IsNot Nothing AndAlso PyObject_IsInstance(kwargs.nativePythonObject_Conflict, New PyObject(PyDict_Type())) <> 1 Then
					Throw New PythonException("Expected kwargs to be dict. Received: " & kwargs.ToString())
				End If
				Return New PythonObject(PyObject_Call(nativePythonObject_Conflict, tuple,If(kwargs Is Nothing, Nothing, kwargs.nativePythonObject_Conflict)))
			Finally
				If ownsTuple Then
					Py_DecRef(tuple)
				End If
			End Try

		End Function


		Public Overridable Function [call](ParamArray ByVal args() As Object) As PythonObject
			Return callWithArgsAndKwargs(java.util.Arrays.asList(args), Nothing)
		End Function

		Public Overridable Function callWithArgs(ByVal args As System.Collections.IList) As PythonObject
			Return [call](args, Nothing)
		End Function

		Public Overridable Function callWithKwargs(ByVal kwargs As System.Collections.IDictionary) As PythonObject
			Return [call](Nothing, kwargs)
		End Function

		Public Overridable Function callWithArgsAndKwargs(ByVal args As System.Collections.IList, ByVal kwargs As System.Collections.IDictionary) As PythonObject
			PythonGIL.assertThreadSafe()
	'JAVA TO VB CONVERTER NOTE: An underscore by itself is not a valid identifier in VB:
	'ORIGINAL LINE: try (PythonGC _ = PythonGC.watch())
			Using underscore As PythonGC = PythonGC.watch()
				If Not Python.callable(Me) Then
					Throw New PythonException("Object is not callable: " & ToString())
				End If
				Dim pyArgs As PythonObject
				Dim pyKwargs As PythonObject

				If args Is Nothing OrElse args.Count = 0 Then
					pyArgs = New PythonObject(PyTuple_New(0))
				Else
					Dim argsList As PythonObject = PythonTypes.convert(args)
					pyArgs = New PythonObject(PyList_AsTuple(argsList.NativePythonObject))
				End If
				If kwargs Is Nothing Then
					pyKwargs = Nothing
				Else
					pyKwargs = PythonTypes.convert(kwargs)
				End If

				Dim ret As New PythonObject(PyObject_Call(nativePythonObject_Conflict, pyArgs.nativePythonObject_Conflict,If(pyKwargs Is Nothing, Nothing, pyKwargs.nativePythonObject_Conflict)))

				PythonGC.keep(ret)

				Return ret
			End Using

		End Function


		Public Overridable Function attr(ByVal attrName As String) As PythonObject
			PythonGIL.assertThreadSafe()
			Return New PythonObject(PyObject_GetAttrString(nativePythonObject_Conflict, attrName))
		End Function


		Public Sub New(ByVal javaObject As Object)
			PythonGIL.assertThreadSafe()
			If TypeOf javaObject Is PythonObject Then
				owned = False
				nativePythonObject_Conflict = DirectCast(javaObject, PythonObject).nativePythonObject_Conflict
			Else
	'JAVA TO VB CONVERTER NOTE: An underscore by itself is not a valid identifier in VB:
	'ORIGINAL LINE: try (PythonGC _ = PythonGC.pause())
				Using underscore As PythonGC = PythonGC.pause()
					nativePythonObject_Conflict = PythonTypes.convert(javaObject).NativePythonObject
				End Using
				PythonGC.register(Me)
			End If

		End Sub

		Public Overridable Function toInt() As Integer
			Return PythonTypes.INT.toJava(Me).intValue()
		End Function

		Public Overridable Function toLong() As Long
			Return PythonTypes.INT.toJava(Me)
		End Function

		Public Overridable Function toFloat() As Single
			Return PythonTypes.FLOAT.toJava(Me).floatValue()
		End Function

		Public Overridable Function toDouble() As Double
			Return PythonTypes.FLOAT.toJava(Me)
		End Function

		Public Overridable Function toBoolean() As Boolean
			Return PythonTypes.BOOL.toJava(Me)

		End Function

		Public Overridable Function toList() As System.Collections.IList
			Return PythonTypes.LIST.toJava(Me)
		End Function

		Public Overridable Function toMap() As System.Collections.IDictionary
			Return PythonTypes.DICT.toJava(Me)
		End Function

		Public Overridable Function get(ByVal key As Integer) As PythonObject
			PythonGIL.assertThreadSafe()
			Return New PythonObject(PyObject_GetItem(nativePythonObject_Conflict, PyLong_FromLong(key)))
		End Function

		Public Overridable Function get(ByVal key As String) As PythonObject
			PythonGIL.assertThreadSafe()
			Return New PythonObject(PyObject_GetItem(nativePythonObject_Conflict, PyUnicode_FromString(key)))
		End Function

		Public Overridable Function get(ByVal key As PythonObject) As PythonObject
			PythonGIL.assertThreadSafe()
			Return New PythonObject(PyObject_GetItem(nativePythonObject_Conflict, key.nativePythonObject_Conflict))
		End Function

		Public Overridable Sub set(ByVal key As PythonObject, ByVal value As PythonObject)
			PythonGIL.assertThreadSafe()
			PyObject_SetItem(nativePythonObject_Conflict, key.nativePythonObject_Conflict, value.nativePythonObject_Conflict)
		End Sub


		Public Overridable Function abs() As PythonObject
			Return New PythonObject(PyNumber_Absolute(nativePythonObject_Conflict))
		End Function
		Public Overridable Function add(ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_Add(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function
		Public Overridable Function [sub](ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_Subtract(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function
		Public Overridable Function [mod](ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_Divmod(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function
		Public Overridable Function mul(ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_Multiply(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function
		Public Overridable Function trueDiv(ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_TrueDivide(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function
		Public Overridable Function floorDiv(ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_FloorDivide(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function
		Public Overridable Function matMul(ByVal pythonObject As PythonObject) As PythonObject
			Return New PythonObject(PyNumber_MatrixMultiply(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict))
		End Function

		Public Overridable Sub addi(ByVal pythonObject As PythonObject)
			PyNumber_InPlaceAdd(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict)
		End Sub
		Public Overridable Sub subi(ByVal pythonObject As PythonObject)
			PyNumber_InPlaceSubtract(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict)
		End Sub
		Public Overridable Sub muli(ByVal pythonObject As PythonObject)
			PyNumber_InPlaceMultiply(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict)
		End Sub
		Public Overridable Sub trueDivi(ByVal pythonObject As PythonObject)
			PyNumber_InPlaceTrueDivide(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict)
		End Sub
		Public Overridable Sub floorDivi(ByVal pythonObject As PythonObject)
			PyNumber_InPlaceFloorDivide(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict)
		End Sub
		Public Overridable Sub matMuli(ByVal pythonObject As PythonObject)
			PyNumber_InPlaceMatrixMultiply(nativePythonObject_Conflict, pythonObject.nativePythonObject_Conflict)
		End Sub
	End Class

End Namespace