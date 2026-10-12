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

	''' 
	''' <summary>
	''' This class helps control the runtime's <seealso cref="PythonExecutioner"/> -
	''' the <seealso cref="PythonExecutioner"/> is heavily system properties based.
	''' Various aspects of the python executioner can be controlled with
	''' the properties in this class. Python's core behavior of initialization,
	''' python path setting, and working with javacpp's embedded cpython
	''' are keys to integrating the python executioner successfully with various applications.
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class PythonConstants
		Public Const DEFAULT_PYTHON_PATH_PROPERTY As String = "org.eclipse.python4j.path"
		Public Const JAVACPP_PYTHON_APPEND_TYPE As String = "org.eclipse.python4j.path.append"
		'for embedded execution, this is to ensure we allow customization of the gil state releasing when running in another embedded python situation
		Public Const RELEASE_GIL_AUTOMATICALLY As String = "org.eclipse.python4j.release_gil_automatically"
		Public Const DEFAULT_RELEASE_GIL_AUTOMATICALLY As String = "true"
		Public Const DEFAULT_APPEND_TYPE As String = "before"
		Public Const INITIALIZE_PYTHON As String = "org.eclipse.python4j.python.initialize"
		Public Const DEFAULT_INITIALIZE_PYTHON As String = "true"
		Public Const PYTHON_EXEC_RESOURCE As String = "org/nd4j/python4j/pythonexec/pythonexec.py"
		Friend Const PYTHON_EXCEPTION_KEY As String = "__python_exception__"
		Public Const CREATE_NPY_VIA_PYTHON As String = "org.eclipse.python4j.create_npy_python"
		Public Const DEFAULT_CREATE_NPY_VIA_PYTHON As String = "false"

		''' <summary>
		''' Controls how to create the numpy array objects associated
		''' with the NumpyArray.java module.
		''' 
		''' Depending on how threading is handled, Py_Type() causes a JVM crash
		''' when used. Py_Type() is used to obtain the type of a numpy array.
		''' Defaults to false, as most of the time this is less performant and not needed.
		''' 
		''' The python based method uses raw pointer address + ctypes inline to create the proper numpy array
		''' on the python side.
		''' Otherwise, a more direct c based approach is used.
		''' @return
		''' </summary>
	   Public Shared Function createNpyViaPython() As Boolean
		   Return Boolean.Parse(System.getProperty(CREATE_NPY_VIA_PYTHON,DEFAULT_CREATE_NPY_VIA_PYTHON))
	   End Function

		''' <summary>
		''' Setter for the associated property
		''' from <seealso cref="createNpyViaPython()"/>
		''' please see this function for more information. </summary>
		''' <param name="createNpyViaPython"> </param>
	   Public Shared WriteOnly Property CreateNpyViaPython As Boolean
		   Set(ByVal createNpyViaPython As Boolean)
			   System.setProperty(CREATE_NPY_VIA_PYTHON,createNpyViaPython.ToString())
		   End Set
	   End Property


		''' <summary>
		''' Sets the default python path.
		''' See <seealso cref="defaultPythonPath()"/>
		''' for more information. </summary>
		''' <param name="newPythonPath"> the new python path to use </param>
		Public Shared WriteOnly Property DefaultPythonPath As String
			Set(ByVal newPythonPath As String)
				System.setProperty(DEFAULT_PYTHON_PATH_PROPERTY,newPythonPath)
			End Set
		End Property

		''' <summary>
		''' Returns the default python path.
		''' This python path should be initialized before the <seealso cref="PythonExecutioner"/>
		''' is called.
		''' @return
		''' </summary>
		Public Shared Function defaultPythonPath() As String
			Return System.getProperty(PythonConstants.DEFAULT_PYTHON_PATH_PROPERTY)
		End Function

		''' <summary>
		''' Returns whether to initialize python or not.
		''' This property is used when python should be initialized manually.
		''' Normally, the <seealso cref="PythonExecutioner"/> will handle initialization
		''' in its <seealso cref="PythonExecutioner.init()"/> method
		''' 
		''' @return
		''' </summary>
		Public Shared Function initializePython() As Boolean
			Return Boolean.Parse(System.getProperty(INITIALIZE_PYTHON,DEFAULT_INITIALIZE_PYTHON))
		End Function

		''' <summary>
		''' See <seealso cref="initializePython()"/>
		'''  for more information on this property.
		'''  This is the setter method for the associated value. </summary>
		''' <param name="initializePython"> whether to initialize python or not </param>
		Public Shared WriteOnly Property InitializePython As Boolean
			Set(ByVal initializePython As Boolean)
				System.setProperty(INITIALIZE_PYTHON,initializePython.ToString())
			End Set
		End Property


		''' <summary>
		''' Returns the default javacpp python append type.
		''' In javacpp's cython module, it comes with built in support
		''' for determining the python path of most modules.
		''' 
		''' This can clash when invoking python using another distribution of python
		''' such as anaconda. This property allows the user to control how javacpp
		''' interacts with a different python present on the classpath.
		''' 
		''' The default value is <seealso cref="DEFAULT_APPEND_TYPE"/>
		''' @return
		''' </summary>
		Public Shared Function javaCppPythonAppendType() As PythonExecutioner.JavaCppPathType
			Return System.Enum.Parse(GetType(PythonExecutioner.JavaCppPathType), System.getProperty(JAVACPP_PYTHON_APPEND_TYPE,DEFAULT_APPEND_TYPE).ToUpper())
		End Function

		''' <summary>
		''' Setter for the javacpp append type.
		''' See <seealso cref="javaCppPythonAppendType()"/>
		''' for more information on value set by this setter. </summary>
		''' <param name="appendType"> the append type to use </param>
		Public Shared WriteOnly Property JavacppPythonAppendType As PythonExecutioner.JavaCppPathType
			Set(ByVal appendType As PythonExecutioner.JavaCppPathType)
				System.setProperty(JAVACPP_PYTHON_APPEND_TYPE,appendType.ToString())
			End Set
		End Property


		''' <summary>
		''' See <seealso cref="releaseGilAutomatically()"/>
		''' for more information on this setter. </summary>
		''' <param name="releaseGilAutomatically"> whether to release the gil automatically or not. </param>
		Public Shared WriteOnly Property ReleaseGilAutomatically As Boolean
			Set(ByVal releaseGilAutomatically As Boolean)
				System.setProperty(RELEASE_GIL_AUTOMATICALLY,releaseGilAutomatically.ToString())
			End Set
		End Property

		''' <summary>
		''' Returns true if the GIL is released automatically or not.
		''' For linking against applications where python is already present
		''' this is a knob allowing people to turn automatic python thread management off.
		''' This is enabled by default. See <seealso cref="RELEASE_GIL_AUTOMATICALLY"/>
		''' and its default value <seealso cref="DEFAULT_RELEASE_GIL_AUTOMATICALLY"/>
		''' @return
		''' </summary>
		Public Shared Function releaseGilAutomatically() As Boolean
			Return Boolean.Parse(System.getProperty(RELEASE_GIL_AUTOMATICALLY,DEFAULT_RELEASE_GIL_AUTOMATICALLY))
		End Function

	End Class

End Namespace