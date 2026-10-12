Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports PyObject = org.bytedeco.cpython.PyObject
Imports IOUtils = org.apache.commons.io.IOUtils
Imports python = org.bytedeco.cpython.global.python
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.bytedeco.cpython.global.python
import static org.bytedeco.cpython.helper.python.Py_SetPath

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
	''' PythonExecutioner handles executing python code either from passed in python code
	''' or via a python script.
	''' 
	''' PythonExecutioner has a few java system properties to be aware of when executing python:
	''' @link {<seealso cref="PythonConstants.DEFAULT_PYTHON_PATH_PROPERTY"/>} : The default python path to be used by the executioner.
	''' This can be passed with -Dorg.eclipse.python4j.path=your/python/path
	''' 
	''' Python4j has a default python path that imports the javacpp python path depending on what is present.
	''' The javacpp python presets such as <seealso cref="org.bytedeco.cpython.global.python"/> have a cachePackages() method
	''' that leverages loading python artifacts from the python path.
	''' 
	''' This python path can be merged with a custom one or just used as is.
	''' A user specifies this behavior with the system property <seealso cref="PythonConstants.JAVACPP_PYTHON_APPEND_TYPE"/>
	''' This property can have 3 possible values:
	''' 1. before
	''' 2. after
	''' 3. none
	''' 
	''' Order can matter when resolving versions of libraries very similar to the system path. Ensure when adding a custom python path
	''' that these properties are well tested and well understood before use.
	''' 
	''' @author Adam Gibson, Fariz Rahman
	''' </summary>
	Public Class PythonExecutioner
'JAVA TO VB CONVERTER NOTE: The field init was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared init_Conflict As New AtomicBoolean(False)

		Shared Sub New()
			init()
		End Sub

		Private Shared Sub init()
			SyncLock GetType(PythonExecutioner)
				If init_Conflict.get() Then
					Return
				End If
        
				init_Conflict.set(True)
				initPythonPath()
				If PythonConstants.initializePython() Then
					Py_InitializeEx(0)
				End If
				'initialize separately to ensure that numpy import array is not imported twice
				For Each type As PythonType In PythonTypes.get()
					type.init()
				Next type
        
				'set the main thread state for the gil
				PythonGIL.setMainThreadState()
				If _Py_IsFinalizing() <> 1 AndAlso PythonConstants.releaseGilAutomatically() Then
					PyEval_SaveThread()
				End If
        
			End SyncLock
		End Sub

		''' <summary>
		''' Sets a variable.
		''' </summary>
		''' <param name="name"> </param>
		''' <param name="value"> </param>
		Public Shared Sub setVariable(ByVal name As String, ByVal value As PythonObject)
			PythonGIL.assertThreadSafe()
			Dim main As PyObject = PyImport_ImportModule("__main__")
			Dim globals As PyObject = PyModule_GetDict(main)
			PyDict_SetItemString(globals, name, value.NativePythonObject)
			Py_DecRef(main)

		End Sub

		''' <summary>
		''' Sets given list of PythonVariables in the interpreter.
		''' </summary>
		''' <param name="pyVars"> </param>
		Public Shared WriteOnly Property Variables As IList(Of PythonVariable)
			Set(ByVal pyVars As IList(Of PythonVariable))
				For Each pyVar As PythonVariable In pyVars
					setVariable(pyVar.getName(), pyVar.getPythonObject())
				Next pyVar
			End Set
		End Property

		''' <summary>
		''' Sets given list of PythonVariables in the interpreter.
		''' </summary>
		''' <param name="pyVars"> </param>
		Public Shared WriteOnly Property Variables As PythonVariable()
			Set(ByVal pyVars() As PythonVariable)
				setVariables(Arrays.asList(pyVars))
			End Set
		End Property

		''' <summary>
		''' Gets the given list of PythonVariables from the interpreter.
		''' </summary>
		''' <param name="pyVars"> </param>
		Public Shared Sub getVariables(ByVal pyVars As IList(Of PythonVariable))
			For Each pyVar As PythonVariable In pyVars
				pyVar.setValue(getVariable(pyVar.getName(), pyVar.getType()).getValue())
			Next pyVar
		End Sub

		''' <summary>
		''' Gets the given list of PythonVariables from the interpreter.
		''' </summary>
		''' <param name="pyVars"> </param>
		Public Shared Sub getVariables(ParamArray ByVal pyVars() As PythonVariable)
			getVariables(Arrays.asList(pyVars))
		End Sub



		''' <summary>
		''' Gets the variable with the given name from the interpreter.
		''' </summary>
		''' <param name="name">
		''' @return </param>
		Public Shared Function getVariable(ByVal name As String) As PythonObject
			PythonGIL.assertThreadSafe()
			Dim main As PyObject = PyImport_ImportModule("__main__")
			Dim globals As PyObject = PyModule_GetDict(main)
			Dim pyName As PyObject = PyUnicode_FromString(name)
			Try
				If PyDict_Contains(globals, pyName) = 1 Then
					Return New PythonObject(PyObject_GetItem(globals, pyName), False)
				End If
			Finally
				Py_DecRef(main)
				'Py_DecRef(globals);
				Py_DecRef(pyName)
			End Try
			Return New PythonObject(Nothing)
		End Function

		''' <summary>
		''' Gets the variable with the given name from the interpreter.
		''' </summary>
		''' <param name="name">
		''' @return </param>
		Public Shared Function getVariable(Of T)(ByVal name As String, ByVal type As PythonType(Of T)) As PythonVariable(Of T)
			Dim val As PythonObject = getVariable(name)
			Return New PythonVariable(Of T)(name, type, type.toJava(val))
		End Function

		''' <summary>
		''' Executes a string of code
		''' </summary>
		''' <param name="code"> </param>
		Public Shared Sub simpleExec(ByVal code As String)
			SyncLock GetType(PythonExecutioner)
				PythonGIL.assertThreadSafe()
        
				Dim result As Integer = PyRun_SimpleStringFlags(code, Nothing)
				If result <> 0 Then
					Throw New PythonException("Execution failed, unable to retrieve python exception.")
				End If
			End SyncLock
		End Sub

		Private Shared Sub throwIfExecutionFailed()
			Dim ex As PythonObject = getVariable(PythonConstants.PYTHON_EXCEPTION_KEY)
			If ex IsNot Nothing AndAlso Not ex.None AndAlso ex.ToString().Length > 0 Then
				setVariable(PythonConstants.PYTHON_EXCEPTION_KEY, PythonTypes.STR.toPython(""))
				Throw New PythonException(ex)
			End If
		End Sub


		Private Shared Function getWrappedCode(ByVal code As String) As String
			Dim resource As New ClassPathResource(PythonConstants.PYTHON_EXEC_RESOURCE)
			If Not resource.exists() Then
				Throw New System.InvalidOperationException("Unable to find class path resource for python script execution: " & PythonConstants.PYTHON_EXEC_RESOURCE & " if using via graalvm, please ensure this resource is included in your resources-config.json")
			End If
			Try
					Using [is] As Stream = resource.InputStream
					Dim base As String = IOUtils.toString([is], StandardCharsets.UTF_8)
					Dim indentedCode As String = "    " & code.Replace(vbLf, vbLf & "    ")
					Dim [out] As String = base.Replace("    pass", indentedCode)
					Return [out]
					End Using
			Catch e As IOException
				Throw New System.InvalidOperationException("Unable to read python code!", e)
			End Try

		End Function

		''' <summary>
		''' Executes a string of code. Throws PythonException if execution fails.
		''' </summary>
		''' <param name="code"> </param>
		Public Shared Sub exec(ByVal code As String)
			simpleExec(getWrappedCode(code))
			throwIfExecutionFailed()
		End Sub

		Public Shared Sub exec(ByVal code As String, ByVal inputs As IList(Of PythonVariable), ByVal outputs As IList(Of PythonVariable))
			If inputs IsNot Nothing Then
				setVariables(CType(inputs, List(Of PythonVariable)).ToArray())
			End If
			exec(code)
			If outputs IsNot Nothing Then
				getVariables(CType(outputs, List(Of PythonVariable)).ToArray())
			End If
		End Sub

		''' <summary>
		''' Return list of all supported variables in the interpreter.
		''' 
		''' @return
		''' </summary>
		Public Shared ReadOnly Property AllVariables As PythonVariables
			Get
				PythonGIL.assertThreadSafe()
				Dim ret As New PythonVariables()
				Dim main As PyObject = PyImport_ImportModule("__main__")
				Dim globals As PyObject = PyModule_GetDict(main)
				Dim keys As PyObject = PyDict_Keys(globals)
				Dim keysIter As PyObject = PyObject_GetIter(keys)
				Try
    
					Dim n As Long = PyObject_Size(globals)
					For i As Integer = 0 To n - 1
						Dim pyKey As PyObject = PyIter_Next(keysIter)
						Try
							If Not (New PythonObject(pyKey, False)).ToString().StartsWith("_", StringComparison.Ordinal) Then
    
								Dim pyVal As PyObject = PyObject_GetItem(globals, pyKey) ' TODO check ref count
								Dim pt As PythonType
								Try
									pt = PythonTypes.getPythonTypeForPythonObject(New PythonObject(pyVal, False))
    
								Catch pe As PythonException
									pt = Nothing
								End Try
								If pt IsNot Nothing Then
									ret.Add(New PythonVariable(Of )((New PythonObject(pyKey, False)).ToString(), pt, pt.toJava(New PythonObject(pyVal, False))))
								End If
							End If
						Finally
							Py_DecRef(pyKey)
						End Try
					Next i
				Finally
					Py_DecRef(keysIter)
					Py_DecRef(keys)
					Py_DecRef(main)
					Return ret
				End Try
    
			End Get
		End Property


		''' <summary>
		''' Executes a string of code and returns a list of all supported variables.
		''' </summary>
		''' <param name="code"> </param>
		''' <param name="inputs">
		''' @return </param>
		Public Shared Function execAndReturnAllVariables(ByVal code As String, ByVal inputs As IList(Of PythonVariable)) As PythonVariables
			setVariables(inputs)
			simpleExec(getWrappedCode(code))
			Return AllVariables
		End Function

		''' <summary>
		''' Executes a string of code and returns a list of all supported variables.
		''' </summary>
		''' <param name="code">
		''' @return </param>
		Public Shared Function execAndReturnAllVariables(ByVal code As String) As PythonVariables
			simpleExec(getWrappedCode(code))
			Return AllVariables
		End Function

		Private Shared Sub initPythonPath()
			SyncLock GetType(PythonExecutioner)
				Try
					Dim path As String = PythonConstants.defaultPythonPath()
        
					Dim packagesList As IList(Of File) = New List(Of File)()
				CType(packagesList, List(Of File)).AddRange(New List(Of File) From {cachePackages()})
					For Each type As PythonType In PythonTypes.get()
					CType(packagesList, List(Of File)).AddRange(New List(Of File) From {type.packages()})
					Next type
					'// TODO: fix in javacpp
				packagesList.Add(New File(python.cachePackage(), "site-packages"))
        
					Dim packages() As File = CType(packagesList, List(Of File)).ToArray()
        
					If path Is Nothing Then
						Py_AddPath(packages)
					Else
						Dim sb As New StringBuilder()
        
						Dim pathAppendValue As JavaCppPathType = PythonConstants.javaCppPythonAppendType()
						Select Case pathAppendValue
							Case org.nd4j.python4j.PythonExecutioner.JavaCppPathType.BEFORE
								For Each cacheDir As File In packages
									sb.Append(cacheDir)
									sb.Append(File.pathSeparator)
								Next cacheDir
        
								sb.Append(path)
							Case org.nd4j.python4j.PythonExecutioner.JavaCppPathType.AFTER
								sb.Append(path)
        
								For Each cacheDir As File In packages
									sb.Append(cacheDir)
									sb.Append(File.pathSeparator)
								Next cacheDir
							Case org.nd4j.python4j.PythonExecutioner.JavaCppPathType.NONE
								sb.Append(path)
						End Select
        
						Py_AddPath(sb.ToString())
					End If
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End SyncLock
		End Sub

		Public Enum JavaCppPathType
			BEFORE
			AFTER
			NONE
		End Enum

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.io.File[] cachePackages() throws java.io.IOException
		Private Shared Function cachePackages() As File()
			Dim path() As File = python.cachePackages()
			path = Arrays.CopyOf(path, path.Length + 1)
			path(path.Length - 1) = cachePackage()
			Return path
		End Function

	End Class
End Namespace