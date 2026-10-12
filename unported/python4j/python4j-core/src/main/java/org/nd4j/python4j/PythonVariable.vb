Imports System
Imports System.Collections
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Data public class PythonVariable<T>
	Public Class PythonVariable(Of T)

		Private name As String

		Private Class PythonTypeAnonymousInnerClass
			Inherits PythonType(Of Long)

			Private ReadOnly outerInstance As PythonTypes

			Public PythonTypeAnonymousInnerClass(PythonTypes outerInstance, java.lang.Class Class)
				Private ReadOnly outerInstance As PythonVariable.PythonTypeAnonymousInnerClass

'JAVA TO VB CONVERTER WARNING: The following constructor is declared outside of its associated class:
'ORIGINAL LINE: public ()
				Sub New(ByVal outerInstance As PythonVariable.PythonTypeAnonymousInnerClass)
					Me.outerInstance = outerInstance
				End Sub

				MyBase("int", class)
				Me.outerInstance = outerInstance
			End Class

			public Long? adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is Number Then
					Return CType(javaObject, Number).longValue()
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to Long")
			End If

			public Long? toJava(PythonObject pythonObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim val As Long = PyLong_AsLong(pythonObject.getNativePythonObject())
				If val = -1 AndAlso PyErr_Occurred() IsNot Nothing Then
					Throw New PythonException("Could not convert value to int: " & pythonObject.ToString())
				End If
				Return val
			End If

			public Boolean accepts(Object javaObject)
			If True Then
				Return (TypeOf javaObject Is Integer?) OrElse (TypeOf javaObject Is Long?)
			End If

			public PythonObject toPython(Long? javaObject)
			If True Then
				Return New PythonObject(PyLong_FromLong(javaObject))
			End If
		End Class

		private static class PythonTypeAnonymousInnerClass2 extends PythonType(Of Double)
		If True Then
			private final PythonTypes outerInstance

			public PythonTypeAnonymousInnerClass2(PythonTypes outerInstance, java.lang.Class class)
			If True Then
				MyBase("float", class)
				Me.outerInstance = outerInstance
			End If


			public Double? adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is Number Then
					Return CType(javaObject, Number).doubleValue()
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to Long")
			End If

			public Double? toJava(PythonObject pythonObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim val As Double = PyFloat_AsDouble(pythonObject.getNativePythonObject())
				If val = -1 AndAlso PyErr_Occurred() IsNot Nothing Then
					Throw New PythonException("Could not convert value to float: " & pythonObject.ToString())
				End If
				Return val
			End If

			public Boolean accepts(Object javaObject)
			If True Then
				Return (TypeOf javaObject Is Single?) OrElse (TypeOf javaObject Is Double?)
			End If

			public PythonObject toPython(Double? javaObject)
			If True Then
				Return New PythonObject(PyFloat_FromDouble(javaObject))
			End If
		End If

		private static class PythonTypeAnonymousInnerClass3 extends PythonType(Of Boolean)
		If True Then
			private final PythonTypes outerInstance

			public PythonTypeAnonymousInnerClass3(PythonTypes outerInstance, java.lang.Class class)
			If True Then
				MyBase("bool", class)
				Me.outerInstance = outerInstance
			End If


			public Boolean? adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is Boolean? Then
					Return CType(javaObject, Boolean?)
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to Boolean")
			End If

			public Boolean? toJava(PythonObject pythonObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim builtins As PyObject = PyImport_ImportModule("builtins")
				Dim boolF As PyObject = PyObject_GetAttrString(builtins, "bool")

				Dim bool As PythonObject = (New PythonObject(boolF, False)).call(pythonObject)
				Dim ret As Boolean = PyLong_AsLong(bool.NativePythonObject) > 0
				bool.del()
				Py_DecRef(boolF)
				Py_DecRef(builtins)
				Return ret
			End If

			public PythonObject toPython(Boolean? javaObject)
			If True Then
				Return New PythonObject(PyBool_FromLong(If(javaObject, 1, 0)))
			End If
		End If

		private static class PythonTypeAnonymousInnerClass4 extends PythonType(Of System.Collections.IList)
		If True Then
			private final PythonTypes outerInstance

			public PythonTypeAnonymousInnerClass4(PythonTypes outerInstance, java.lang.Class class)
			If True Then
				MyBase("list", class)
				Me.outerInstance = outerInstance
			End If


			public Boolean accepts(Object javaObject)
			If True Then
				Return (TypeOf javaObject Is System.Collections.IList OrElse javaObject.GetType().IsArray)
			End If

			public System.Collections.IList adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is System.Collections.IList Then
					Return CType(javaObject, System.Collections.IList)
				ElseIf javaObject.GetType().IsArray Then
					Dim ret As IList(Of Object) = New List(Of Object)()
					If TypeOf javaObject Is Object() Then
						Dim arr() As Object = CType(javaObject, Object())
						Return New List(Of )(Arrays.asList(arr))
					ElseIf TypeOf javaObject Is Short() Then
						Dim arr() As Short = CType(javaObject, Short())
						For Each x As Short In arr
							ret.add(x)
						Next x
						Return ret
					ElseIf TypeOf javaObject Is Integer() Then
						Dim arr() As Integer = CType(javaObject, Integer())
						For Each x As Integer In arr
							ret.add(x)
						Next x
						Return ret
					ElseIf TypeOf javaObject Is SByte() Then
						Dim arr() As SByte = CType(javaObject, SByte())
						For Each x As Integer In arr
							ret.add(x And &Hff)
						Next x
						Return ret
					ElseIf TypeOf javaObject Is Long() Then
						Dim arr() As Long = CType(javaObject, Long())
						For Each x As Long In arr
							ret.add(x)
						Next x
						Return ret
					ElseIf TypeOf javaObject Is Single() Then
						Dim arr() As Single = CType(javaObject, Single())
						For Each x As Single In arr
							ret.add(x)
						Next x
						Return ret
					ElseIf TypeOf javaObject Is Double() Then
						Dim arr() As Double = CType(javaObject, Double())
						For Each x As Double In arr
							ret.add(x)
						Next x
						Return ret
					ElseIf TypeOf javaObject Is Boolean() Then
						Dim arr() As Boolean = CType(javaObject, Boolean())
						For Each x As Boolean In arr
							ret.add(x)
						Next x
						Return ret
					Else
						Throw New PythonException("Unsupported array type: " & javaObject.GetType().ToString())
					End If


				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to List")
				End If
			End If

			public System.Collections.IList toJava(PythonObject pythonObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim ret As System.Collections.IList = New ArrayList()
				Dim n As Long = PyObject_Size(pythonObject.getNativePythonObject())
				If n < 0 Then
					Throw New PythonException("Object cannot be interpreted as a List")
				End If
				For i As Long = 0 To n - 1
					Dim pyIndex As PyObject = PyLong_FromLong(i)
					Dim pyItem As PyObject = PyObject_GetItem(pythonObject.getNativePythonObject(), pyIndex)
					Py_DecRef(pyIndex)
					Dim pyItemType As PythonType = getPythonTypeForPythonObject(New PythonObject(pyItem, False))
					ret.add(pyItemType.toJava(New PythonObject(pyItem, False)))
					Py_DecRef(pyItem)
				Next i
				Return ret
			End If

			public PythonObject toPython(System.Collections.IList javaObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim pyList As PyObject = PyList_New(javaObject.size())
				For i As Integer = 0 To javaObject.size() - 1
					Dim item As Object = javaObject.get(i)
					Dim pyItem As PythonObject
					Dim owned As Boolean
					If TypeOf item Is PythonObject Then
						pyItem = DirectCast(item, PythonObject)
						owned = False
					ElseIf TypeOf item Is PyObject Then
						pyItem = New PythonObject(DirectCast(item, PyObject), False)
						owned = False
					Else
						pyItem = PythonTypes.convert(item)
						owned = True
					End If
					Py_IncRef(pyItem.NativePythonObject) ' reference will be stolen by PyList_SetItem()
					PyList_SetItem(pyList, i, pyItem.NativePythonObject)
					If owned Then
						pyItem.del()
					End If
				Next i
				Return New PythonObject(pyList)
			End If
		End If

		private static class PythonTypeAnonymousInnerClass5 extends PythonType(Of System.Collections.IDictionary)
		If True Then
			private final PythonTypes outerInstance

			public PythonTypeAnonymousInnerClass5(PythonTypes outerInstance, java.lang.Class class)
			If True Then
				MyBase("dict", class)
				Me.outerInstance = outerInstance
			End If


			public System.Collections.IDictionary adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is System.Collections.IDictionary Then
					Return CType(javaObject, System.Collections.IDictionary)
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to Map")
			End If

			public System.Collections.IDictionary toJava(PythonObject pythonObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim ret As New Hashtable()
				Dim dictType As New PyObject(PyDict_Type())
				If PyObject_IsInstance(pythonObject.getNativePythonObject(), dictType) <> 1 Then
					Throw New PythonException("Expected dict, received: " & pythonObject.ToString())
				End If

				Dim keys As PyObject = PyDict_Keys(pythonObject.getNativePythonObject())
				Dim keysIter As PyObject = PyObject_GetIter(keys)
				Dim vals As PyObject = PyDict_Values(pythonObject.getNativePythonObject())
				Dim valsIter As PyObject = PyObject_GetIter(vals)
				Try
					Dim n As Long = PyObject_Size(pythonObject.getNativePythonObject())
					For i As Long = 0 To n - 1
						Dim pyKey As New PythonObject(PyIter_Next(keysIter), False)
						Dim pyVal As New PythonObject(PyIter_Next(valsIter), False)
						Dim pyKeyType As PythonType = getPythonTypeForPythonObject(pyKey)
						Dim pyValType As PythonType = getPythonTypeForPythonObject(pyVal)
						ret.put(pyKeyType.toJava(pyKey), pyValType.toJava(pyVal))
						Py_DecRef(pyKey.NativePythonObject)
						Py_DecRef(pyVal.NativePythonObject)
					Next i
				Finally
					Py_DecRef(keysIter)
					Py_DecRef(valsIter)
					Py_DecRef(keys)
					Py_DecRef(vals)
				End Try
				Return ret
			End If

			public PythonObject toPython(System.Collections.IDictionary javaObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim pyDict As PyObject = PyDict_New()
				For Each k As Object In javaObject.keySet()
					Dim pyKey As PythonObject
					If TypeOf k Is PythonObject Then
						pyKey = DirectCast(k, PythonObject)
					ElseIf TypeOf k Is PyObject Then
						pyKey = New PythonObject(DirectCast(k, PyObject))
					Else
						pyKey = PythonTypes.convert(k)
					End If
					Dim v As Object = javaObject.get(k)
					Dim pyVal As PythonObject
					If TypeOf v Is PythonObject Then
						pyVal = DirectCast(v, PythonObject)
					ElseIf TypeOf v Is PyObject Then
						pyVal = New PythonObject(DirectCast(v, PyObject))
					Else
						pyVal = PythonTypes.convert(v)
					End If
					Dim errCode As Integer = PyDict_SetItem(pyDict, pyKey.NativePythonObject, pyVal.NativePythonObject)
					If errCode <> 0 Then
						Dim keyStr As String = pyKey.ToString()
						pyKey.del()
						pyVal.del()
						Throw New PythonException("Unable to create python dictionary. Unhashable key: " & keyStr)
					End If
					pyKey.del()
					pyVal.del()
				Next k
				Return New PythonObject(pyDict)
			End If
		End If

		private static class PythonTypeAnonymousInnerClass6 extends PythonType(Of SByte())
		If True Then
			private final PythonTypes outerInstance

			public PythonTypeAnonymousInnerClass6(PythonTypes outerInstance)
			If True Then
				MyBase("bytes", GetType(SByte()))
				Me.outerInstance = outerInstance
			End If

			public SByte() toJava(PythonObject pythonObject)
			If True Then
				Using gc As PythonGC = PythonGC.watch()
					If Not (Python.isinstance(pythonObject, Python.bytesType())) Then
						Throw New PythonException("Expected bytes. Received: " & pythonObject)
					End If
					Dim pySize As PythonObject = Python.len(pythonObject)
					Dim ret(pySize.toInt() - 1) As SByte
					For i As Integer = 0 To ret.Length - 1
						ret(i) = CSByte(Math.Truncate(pythonObject.get(i).toInt()))
					Next i
					Return ret
				End Using
			End If

			public PythonObject toPython(SByte() javaObject)
			If True Then
				Using gc As PythonGC = PythonGC.watch()
					Dim ret As PythonObject = Python.bytes(LIST.toPython(LIST.adapt(javaObject)))
					PythonGC.keep(ret)
					Return ret
				End Using
			End If
			public Boolean accepts(Object javaObject)
			If True Then
				Return TypeOf javaObject Is SByte()
			End If
			public SByte() adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is SByte() Then
					Return CType(javaObject, SByte())
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to byte[]")
			End If

		End If
		private String type
		private T value_Conflict

		private static Boolean validateVariableName(String s)
		If True Then
			If s.isEmpty() Then
				Return False
			End If
			If Not Character.isJavaIdentifierStart(s.charAt(0)) Then
				Return False
			End If
			For i As Integer = 1 To s.length() - 1
				If Not Character.isJavaIdentifierPart(s.charAt(i)) Then
					Return False
				End If
			Next i
			Return True
		End If

		public PythonVariable(String name, PythonType(Of T) type, Object value_Conflict)
		If True Then
			If Not validateVariableName(name) Then
				Throw New PythonException("Invalid identifier: " & name)
			End If
			Me.name = name
			Me.type = type.getName()
			setValue(value_Conflict)
		End If

		public PythonVariable(String name, PythonType(Of T) type)
		If True Then
			Me(name, type, Nothing)
		End If

		public PythonType(Of T) [getType]()
		If True Then
			Return PythonTypes.get(Me.type)
		End If

		public T getValue()
		If True Then
			Return Me.value_Conflict
		End If

		public void setValue(Object value_Conflict)
		If True Then
			Me.value_Conflict = If(value_Conflict Is Nothing, Nothing, [getType]().adapt(value_Conflict))
		End If

		public PythonObject PythonObject
		If True Then
			Return [getType]().toPython(value_Conflict)
		End If

	End Class

End Namespace