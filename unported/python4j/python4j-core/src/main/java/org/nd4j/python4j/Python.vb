Imports System.Collections.Generic
Imports PyObject = org.bytedeco.cpython.PyObject
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



	Public Class Python

		Shared Sub New()
			Dim tempVar As New PythonExecutioner()
		End Sub

		''' <summary>
		''' Imports a python module, similar to python import statement.
		''' </summary>
		''' <param name="moduleName"> name of the module to be imported </param>
		''' <returns> reference to the module object </returns>
		Public Shared Function importModule(ByVal moduleName As String) As PythonObject
			PythonGIL.assertThreadSafe()
			Dim [module] As New PythonObject(PyImport_ImportModule(moduleName))
			If [module].None Then
				Throw New PythonException("Error importing module: " & moduleName)
			End If
			Return [module]
		End Function

		''' <summary>
		''' Gets a builtins attribute
		''' </summary>
		''' <param name="attrName"> Attribute name
		''' @return </param>
		Public Shared Function attr(ByVal attrName As String) As PythonObject
			PythonGIL.assertThreadSafe()
			Dim builtins As PyObject = PyImport_ImportModule("builtins")
			Try
				Return New PythonObject(PyObject_GetAttrString(builtins, attrName))
			Finally
				Py_DecRef(builtins)
			End Try
		End Function


		''' <summary>
		''' Gets the size of a PythonObject. similar to len() in python.
		''' </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function len(ByVal pythonObject As PythonObject) As PythonObject
			PythonGIL.assertThreadSafe()
			Dim n As Long = PyObject_Size(pythonObject.NativePythonObject)
			If n < 0 Then
				Throw New PythonException("Object has no length: " & pythonObject)
			End If
			Return PythonTypes.INT.toPython(n)
		End Function

		''' <summary>
		''' Gets the string representation of an object.
		''' </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function str(ByVal pythonObject As PythonObject) As PythonObject
			PythonGIL.assertThreadSafe()
			Try
				Return PythonTypes.STR.toPython(pythonObject.ToString())
			Catch e As System.Exception
				Throw New System.Exception(e)
			End Try


		End Function

		''' <summary>
		''' Returns an empty string
		''' 
		''' @return
		''' </summary>
		Public Shared Function str() As PythonObject
			PythonGIL.assertThreadSafe()
			Try
				Return PythonTypes.STR.toPython("")
			Catch e As System.Exception
				Throw New System.Exception(e)
			End Try
		End Function

		''' <summary>
		''' Returns the str type object
		''' @return
		''' </summary>
		Public Shared Function strType() As PythonObject
			Return attr("str")
		End Function

		''' <summary>
		''' Returns a floating point number from a number or a string. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function float_(ByVal pythonObject As PythonObject) As PythonObject
			Return PythonTypes.FLOAT.toPython(PythonTypes.FLOAT.toJava(pythonObject))
		End Function

		''' <summary>
		''' Reutrns 0.
		''' @return
		''' </summary>
		Public Shared Function float_() As PythonObject
			Try
				Return PythonTypes.FLOAT.toPython(0R)
			Catch e As System.Exception
				Throw New System.Exception(e)
			End Try

		End Function

		''' <summary>
		''' Returns the float type object
		''' @return
		''' </summary>
		Public Shared Function floatType() As PythonObject
			Return attr("float")
		End Function


		''' <summary>
		''' Converts a value to a Boolean value i.e., True or False, using the standard truth testing procedure. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function bool(ByVal pythonObject As PythonObject) As PythonObject
			Return PythonTypes.BOOL.toPython(PythonTypes.BOOL.toJava(pythonObject))

		End Function

		''' <summary>
		''' Returns False.
		''' @return
		''' </summary>
		Public Shared Function bool() As PythonObject
			Return PythonTypes.BOOL.toPython(False)

		End Function

		''' <summary>
		''' Returns the bool type object
		''' @return
		''' </summary>
		Public Shared Function boolType() As PythonObject
			Return attr("bool")
		End Function

		''' <summary>
		''' Returns an integer from a number or a string. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function int_(ByVal pythonObject As PythonObject) As PythonObject
			Return PythonTypes.INT.toPython(PythonTypes.INT.toJava(pythonObject))
		End Function

		''' <summary>
		''' Returns 0
		''' @return
		''' </summary>
		Public Shared Function int_() As PythonObject
			Return PythonTypes.INT.toPython(0L)

		End Function

		''' <summary>
		''' Returns the int type object
		''' @return
		''' </summary>
		Public Shared Function intType() As PythonObject
			Return attr("int")
		End Function

		''' <summary>
		'''  Takes sequence types and converts them to lists. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function list(ByVal pythonObject As PythonObject) As PythonObject
			PythonGIL.assertThreadSafe()
	'JAVA TO VB CONVERTER NOTE: An underscore by itself is not a valid identifier in VB:
	'ORIGINAL LINE: try (PythonGC _ = PythonGC.watch())
			Using underscore As PythonGC = PythonGC.watch()
				Dim listF As PythonObject = attr("list")
				Dim ret As PythonObject = listF.call(pythonObject)
				If ret.None Then
					Throw New PythonException("Object is not iterable: " & pythonObject.ToString())
				End If
				Return ret
			End Using
		End Function

		''' <summary>
		''' Returns empty list.
		''' @return
		''' </summary>
		Public Shared Function list() As PythonObject
			Return PythonTypes.LIST.toPython(Collections.emptyList())
		End Function

		''' <summary>
		''' Returns list type object.
		''' @return
		''' </summary>
		Public Shared Function listType() As PythonObject
			Return attr("list")
		End Function

		''' <summary>
		'''  Creates a dictionary. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function dict(ByVal pythonObject As PythonObject) As PythonObject
			Dim dictF As PythonObject = attr("dict")
			Dim ret As PythonObject = dictF.call(pythonObject)
			If ret.None Then
				Throw New PythonException("Cannot build dict from object: " & pythonObject.ToString())
			End If
			dictF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns empty dict
		''' @return
		''' </summary>
		Public Shared Function dict() As PythonObject
			Return PythonTypes.DICT.toPython(Collections.emptyMap())
		End Function

		''' <summary>
		''' Returns dict type object.
		''' @return
		''' </summary>
		Public Shared Function dictType() As PythonObject
			Return attr("dict")
		End Function

		''' <summary>
		''' Creates a set. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function set(ByVal pythonObject As PythonObject) As PythonObject
			Dim setF As PythonObject = attr("set")
			Dim ret As PythonObject = setF.call(pythonObject)
			If ret.None Then
				Throw New PythonException("Cannot build set from object: " & pythonObject.ToString())
			End If
			setF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns empty set.
		''' @return
		''' </summary>
		Public Shared Function set() As PythonObject
			Dim setF As PythonObject = attr("set")
			Dim ret As PythonObject
			ret = setF.call()
			setF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns empty set.
		''' @return
		''' </summary>
		Public Shared Function setType() As PythonObject
			Return attr("set")
		End Function

		''' <summary>
		''' Creates a bytearray. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function bytearray(ByVal pythonObject As PythonObject) As PythonObject
			Dim baF As PythonObject = attr("bytearray")
			Dim ret As PythonObject = baF.call(pythonObject)
			If ret.None Then
				Throw New PythonException("Cannot build bytearray from object: " & pythonObject.ToString())
			End If
			baF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns empty bytearray.
		''' @return
		''' </summary>
		Public Shared Function bytearray() As PythonObject
			Dim baF As PythonObject = attr("bytearray")
			Dim ret As PythonObject
			ret = baF.call()
			baF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns bytearray type object
		''' @return
		''' </summary>
		Public Shared Function bytearrayType() As PythonObject
			Return attr("bytearray")
		End Function

		''' <summary>
		''' Creates a memoryview. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function memoryview(ByVal pythonObject As PythonObject) As PythonObject
			Dim mvF As PythonObject = attr("memoryview")
			Dim ret As PythonObject = mvF.call(pythonObject)
			If ret.None Then
				Throw New PythonException("Cannot build memoryview from object: " & pythonObject.ToString())
			End If
			mvF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns memoryview type object.
		''' @return
		''' </summary>
		Public Shared Function memoryviewType() As PythonObject
			Return attr("memoryview")
		End Function

		''' <summary>
		''' Creates a byte string. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function bytes(ByVal pythonObject As PythonObject) As PythonObject
			Dim bytesF As PythonObject = attr("bytes")
			Dim ret As PythonObject = bytesF.call(pythonObject)
			If ret.None Then
				Throw New PythonException("Cannot build bytes from object: " & pythonObject.ToString())
			End If
			bytesF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns empty byte string.
		''' @return
		''' </summary>
		Public Shared Function bytes() As PythonObject
			Dim bytesF As PythonObject = attr("bytes")
			Dim ret As PythonObject
			ret = bytesF.call()
			bytesF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns bytes type object
		''' @return
		''' </summary>
		Public Shared Function bytesType() As PythonObject
			Return attr("bytes")
		End Function

		''' <summary>
		''' Creates a tuple. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function tuple(ByVal pythonObject As PythonObject) As PythonObject
			Dim tupleF As PythonObject = attr("tupleF")
			Dim ret As PythonObject = tupleF.call(pythonObject)
			If ret.None Then
				Throw New PythonException("Cannot build tuple from object: " & pythonObject.ToString())
			End If
			tupleF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns empty tuple.
		''' @return
		''' </summary>
		Public Shared Function tuple() As PythonObject
			Dim tupleF As PythonObject = attr("tuple")
			Dim ret As PythonObject
			ret = tupleF.call()
			tupleF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns tuple type object
		''' @return
		''' </summary>
		Public Shared Function tupleType() As PythonObject
			Return attr("tuple")
		End Function

		''' <summary>
		''' Creates an Exception </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function Exception(ByVal pythonObject As PythonObject) As PythonObject
			Dim excF As PythonObject = attr("Exception")
			Dim ret As PythonObject = excF.call(pythonObject)
			excF.del()
			Return ret
		End Function

		''' <summary>
		''' Creates an Exception
		''' @return
		''' </summary>
		Public Shared Function Exception() As PythonObject
			Dim excF As PythonObject = attr("Exception")
			Dim ret As PythonObject
			ret = excF.call()
			excF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns Exception type object
		''' @return
		''' </summary>
		Public Shared Function ExceptionType() As PythonObject
			Return attr("Exception")
		End Function


		''' <summary>
		''' Returns the globals dictionary.
		''' @return
		''' </summary>
		Public Shared Function globals() As PythonObject
			PythonGIL.assertThreadSafe()
			Dim main As PyObject = PyImport_ImportModule("__main__")
'JAVA TO VB CONVERTER NOTE: The local variable globals was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim globals_Conflict As PyObject = PyModule_GetDict(main)
			Py_DecRef(main)
			Return New PythonObject(globals_Conflict, False)
		End Function

		''' <summary>
		''' Returns the type of an object. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function type(ByVal pythonObject As PythonObject) As PythonObject
			Dim typeF As PythonObject = attr("type")
			Dim ret As PythonObject = typeF.call(pythonObject)
			typeF.del()
			Return ret
		End Function

		''' <summary>
		''' Returns True if the specified object is of the specified type, otherwise False. </summary>
		''' <param name="obj"> </param>
		''' <param name="type">
		''' @return </param>
		Public Shared Function isinstance(ByVal obj As PythonObject, ParamArray ByVal type() As PythonObject) As Boolean
			PythonGIL.assertThreadSafe()
			Dim argsTuple As PyObject = PyTuple_New(type.Length)
			Try
				For i As Integer = 0 To type.Length - 1
					Dim x As PythonObject = type(i)
					Py_IncRef(x.NativePythonObject)
					PyTuple_SetItem(argsTuple, i, x.NativePythonObject)
				Next i
				Return PyObject_IsInstance(obj.NativePythonObject, argsTuple) <> 0
			Finally
				Py_DecRef(argsTuple)
			End Try

		End Function

		''' <summary>
		''' Evaluates the specified expression. </summary>
		''' <param name="expression">
		''' @return </param>
		Public Shared Function eval(ByVal expression As String) As PythonObject

			PythonGIL.assertThreadSafe()
			Dim compiledCode As PyObject = Py_CompileString(expression, "", Py_eval_input)
			Dim main As PyObject = PyImport_ImportModule("__main__")
			Dim globals As PyObject = PyModule_GetDict(main)
			Dim locals As PyObject = PyDict_New()
			Try
				Return New PythonObject(PyEval_EvalCode(compiledCode, globals, locals))
			Finally
				Py_DecRef(main)
				Py_DecRef(locals)
				Py_DecRef(compiledCode)
			End Try

		End Function

		''' <summary>
		''' Returns the builtins module
		''' @return
		''' </summary>
		Public Shared Function builtins() As PythonObject
			Return importModule("builtins")

		End Function

		''' <summary>
		''' Returns None.
		''' @return
		''' </summary>
		Public Shared Function None() As PythonObject
			Return eval("None")
		End Function

		''' <summary>
		''' Returns True.
		''' @return
		''' </summary>
		Public Shared Function [True]() As PythonObject
			Return eval("True")
		End Function

		''' <summary>
		''' Returns False.
		''' @return
		''' </summary>
		Public Shared Function [False]() As PythonObject
			Return eval("False")
		End Function

		''' <summary>
		''' Returns True if the object passed is callable callable, otherwise False. </summary>
		''' <param name="pythonObject">
		''' @return </param>
		Public Shared Function callable(ByVal pythonObject As PythonObject) As Boolean
			PythonGIL.assertThreadSafe()
			Return PyCallable_Check(pythonObject.NativePythonObject) = 1
		End Function


		Public Shared WriteOnly Property Context As String
			Set(ByVal context As String)
				PythonContextManager.Context = context
			End Set
		End Property

		Public Shared ReadOnly Property CurrentContext As String
			Get
				Return PythonContextManager.CurrentContext
			End Get
		End Property

		Public Shared Sub deleteContext(ByVal context As String)
			PythonContextManager.deleteContext(context)
		End Sub
		Public Shared Sub resetContext()
			PythonContextManager.reset()
		End Sub

		''' <summary>
		''' Executes a string of code. </summary>
		''' <param name="code"> </param>
		''' <exception cref="PythonException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exec(String code) throws PythonException
		Public Shared Sub exec(ByVal code As String)
			PythonExecutioner.exec(code)
		End Sub

		''' <summary>
		''' Executes a string of code. </summary>
		''' <param name="code"> </param>
		''' <param name="inputs"> </param>
		''' <param name="outputs"> </param>
		Public Shared Sub exec(ByVal code As String, ByVal inputs As IList(Of PythonVariable), ByVal outputs As IList(Of PythonVariable))
			PythonExecutioner.exec(code, inputs, outputs)
		End Sub


	End Class

End Namespace