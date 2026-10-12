Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports PyObject = org.bytedeco.cpython.PyObject
Imports PyTypeObject = org.bytedeco.cpython.PyTypeObject
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports SizeTPointer = org.bytedeco.javacpp.SizeTPointer
Imports PyArrayObject = org.bytedeco.numpy.PyArrayObject
Imports numpy = org.bytedeco.numpy.global.numpy
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports org.bytedeco.cpython.global.python
Imports org.bytedeco.numpy.global.numpy

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
'ORIGINAL LINE: @Slf4j public class NumpyArray extends PythonType<org.nd4j.linalg.api.ndarray.INDArray>
	Public Class NumpyArray
		Inherits PythonType(Of INDArray)

		Public Shared ReadOnly INSTANCE As NumpyArray
'JAVA TO VB CONVERTER NOTE: The field init was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly init_Conflict As New AtomicBoolean(False)
		Private Shared ReadOnly cache As IDictionary(Of String, DataBuffer) = New Dictionary(Of String, DataBuffer)()
		Public Const IMPORT_NUMPY_ARRAY As String = "org.eclipse.python4j.numpyimport"
		Public Const ADD_JAVACPP_NUMPY_TO_PATH As String = "org.eclipse.python4j.numpyimport"
		Public Const DEFAULT_IMPORT_NUMPY_ARRAY As String = "true"
		Public Const DEFAULT_ADD_JAVACPP_NUMPY_TO_PATH As String = "true"

		Shared Sub New()
			Dim tempVar As New PythonExecutioner()
			INSTANCE = New NumpyArray()
			INSTANCE.init()

		End Sub

		Public Overrides Function packages() As File()
			Try
				Return New File(){numpy.cachePackage()}
			Catch e As Exception
				Throw New PythonException(e)
			End Try

		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows public synchronized void init()
		Public Overrides Sub init()
			SyncLock Me
				If init_Conflict.get() Then
					Return
				End If
				init_Conflict.set(True)
        
				If Boolean.Parse(System.getProperty(IMPORT_NUMPY_ARRAY,DEFAULT_IMPORT_NUMPY_ARRAY)) Then
					'See: https://numpy.org/doc/1.17/reference/c-api.array.html#importing-the-api
					'DO NOT REMOVE
					If Boolean.Parse(System.getProperty(ADD_JAVACPP_NUMPY_TO_PATH,DEFAULT_ADD_JAVACPP_NUMPY_TO_PATH)) Then
						Py_AddPath(numpy.cachePackages())
					End If
        
					'ensure python doesn't get initialized twice, this call is needed before numpy import array
					PythonConstants.InitializePython = False
					Py_Initialize()
        
					Dim err As Integer = numpy._import_array()
					If err < 0 Then
						Console.WriteLine("Numpy import failed!")
						Throw New PythonException("Numpy import failed!")
					End If
				End If
        
				If PythonGIL.locked() Then
					Throw New PythonException("Can not initialize numpy - GIL already acquired.")
				End If
        
        
        
			End SyncLock
		End Sub

		Public Sub New()
			MyBase.New("numpy.ndarray", GetType(INDArray))
		End Sub

		Public Overrides Function toJava(ByVal pythonObject As PythonObject) As INDArray
			log.debug("Converting PythonObject to INDArray...")
			Dim np As PyObject = PyImport_ImportModule("numpy")
			Dim ndarray As PyObject = PyObject_GetAttrString(np, "ndarray")
			If PyObject_IsInstance(pythonObject.NativePythonObject, ndarray) <> 1 Then
				Py_DecRef(ndarray)
				Py_DecRef(np)
				Throw New PythonException("Object is not a numpy array! Use Python.ndarray() to convert object to a numpy array.")
			End If
			Py_DecRef(ndarray)
			Py_DecRef(np)
			Dim npArr As New PyArrayObject(pythonObject.NativePythonObject)
			Dim shape(PyArray_NDIM(npArr) - 1) As Long
			Dim shapePtr As SizeTPointer = PyArray_SHAPE(npArr)
			If shapePtr IsNot Nothing Then
				shapePtr.get(shape, 0, shape.Length)
			End If
			Dim strides(shape.Length - 1) As Long
			Dim stridesPtr As SizeTPointer = PyArray_STRIDES(npArr)
			If stridesPtr IsNot Nothing Then
				stridesPtr.get(strides, 0, strides.Length)
			End If
			Dim npdtype As Integer = PyArray_TYPE(npArr)

			Dim dtype As DataType
			Select Case npdtype
				Case NPY_DOUBLE
					dtype = DataType.DOUBLE
				Case NPY_FLOAT
					dtype = DataType.FLOAT
				Case NPY_SHORT
					dtype = DataType.SHORT
				Case NPY_INT
					dtype = DataType.INT32
				Case NPY_LONG
					dtype = DataType.INT64
				Case NPY_UINT
					dtype = DataType.UINT32
				Case NPY_BYTE
					dtype = DataType.INT8
				Case NPY_UBYTE
					dtype = DataType.UINT8
				Case NPY_BOOL
					dtype = DataType.BOOL
				Case NPY_HALF
					dtype = DataType.FLOAT16
				Case NPY_LONGLONG
					dtype = DataType.INT64
				Case NPY_USHORT
					dtype = DataType.UINT16
				Case NPY_ULONG, NPY_ULONGLONG
					dtype = DataType.UINT64
				Case Else
					Throw New PythonException("Unsupported array data type: " & npdtype)
			End Select
			Dim size As Long = 1
			Dim i As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: for (int i = 0; i < shape.length; size *= shape[i++])
			Do While i < shape.Length

				size *= shape(i)
i += 1
			Loop

			Dim ret As INDArray
			Dim address As Long = PyArray_DATA(npArr).address()
			Dim key As String = address & "_" & size & "_" & dtype
			Dim buff As DataBuffer = cache(key)
			If buff Is Nothing Then
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					Dim ptr As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().pointerForAddress(address)
					ptr = ptr.limit(size)
					ptr = ptr.capacity(size)
					buff = Nd4j.createBuffer(ptr, size, dtype)
					cache(key) = buff
				End Using
			End If
			Dim elemSize As Integer = buff.ElementSize
			Dim nd4jStrides(strides.Length - 1) As Long
			For i As Integer = 0 To strides.Length - 1
				nd4jStrides(i) = strides(i) \ elemSize
			Next i
			ret = Nd4j.create(buff, shape, nd4jStrides, 0, Shape.getOrder(shape, nd4jStrides, 1), dtype)
			Nd4j.AffinityManager.tagLocation(ret, AffinityManager.Location.HOST)
			log.debug("Done creating numpy arrray.")
			Return ret


		End Function

		Public Overrides Function toPython(ByVal indArray As INDArray) As PythonObject
			log.debug("Converting INDArray to PythonObject...")
			Dim dataType As DataType = indArray.dataType()
			Dim buff As DataBuffer = indArray.data()
			Dim key As String = buff.pointer().address() & "_" & buff.length() & "_" & dataType
			cache(key) = buff
			Dim numpyType As Integer
			Dim [ctype] As String
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					numpyType = NPY_DOUBLE
					[ctype] = "c_double"
				Case DataType.InnerEnum.FLOAT, BFLOAT16
					numpyType = NPY_FLOAT
					[ctype] = "c_float"
				Case DataType.InnerEnum.SHORT
					numpyType = NPY_SHORT
					[ctype] = "c_short"
				Case DataType.InnerEnum.INT
					numpyType = NPY_INT
					[ctype] = "c_int"
				Case DataType.InnerEnum.LONG
					numpyType = NPY_INT64
					[ctype] = "c_int64"
				Case DataType.InnerEnum.UINT16
					numpyType = NPY_USHORT
					[ctype] = "c_uint16"
				Case DataType.InnerEnum.UINT32
					numpyType = NPY_UINT
					[ctype] = "c_uint"
				Case DataType.InnerEnum.UINT64
					numpyType = NPY_UINT64
					[ctype] = "c_uint64"
				Case DataType.InnerEnum.BOOL
					numpyType = NPY_BOOL
					[ctype] = "c_bool"
				Case DataType.InnerEnum.BYTE
					numpyType = NPY_BYTE
					[ctype] = "c_byte"
				Case DataType.InnerEnum.UBYTE
					numpyType = NPY_UBYTE
					[ctype] = "c_ubyte"
				Case DataType.InnerEnum.HALF
					numpyType = NPY_HALF
					[ctype] = "c_short"
				Case Else
					Throw New Exception("Unsupported dtype: " & dataType)
			End Select

			Dim shape() As Long = indArray.shape()
			Dim inputArray As INDArray = indArray
			If dataType = DataType.BFLOAT16 Then
				log.warn("Creating copy of array as bfloat16 is not supported by numpy.")
				inputArray = indArray.castTo(DataType.FLOAT)
			End If

			'Sync to host memory in the case of CUDA, before passing the host memory pointer to Python

			Nd4j.AffinityManager.ensureLocation(inputArray, AffinityManager.Location.HOST)

			' PyArray_Type() call causes jvm crash in linux cpu if GIL is acquired by non main thread.
			' Using Interpreter for now:

			'likely embedded in python, always use this method instead
			If Not PythonConstants.releaseGilAutomatically() OrElse PythonConstants.createNpyViaPython() Then
				Using context As New PythonContextManager.Context("__np_array_converter")
					log.debug("Stringing exec...")
					Dim code As String = "import ctypes" & vbLf & "import numpy as np" & vbLf & "cArr = (ctypes." & [ctype] & "*" & indArray.length() & ")" & ".from_address(" & indArray.data().pointer().address() & ")" & vbLf & "npArr = np.frombuffer(cArr, dtype=" & (If(numpyType = NPY_HALF, "'half'", "ctypes." & [ctype])) & ").reshape(" & java.util.Arrays.toString(indArray.shape()) & ")"
					PythonExecutioner.exec(code)
					log.debug("exec done.")
					Dim ret As PythonObject = PythonExecutioner.getVariable("npArr")
					Py_IncRef(ret.NativePythonObject)
					Return ret

				End Using
			Else
				log.debug("NUMPY: PyArray_Type()")
				Dim pyTypeObject As PyTypeObject = PyArray_Type()


				log.debug("NUMPY: PyArray_New()")
				Dim npArr As PyObject = PyArray_New(pyTypeObject, shape.Length, New SizeTPointer(shape), numpyType, Nothing, inputArray.data().addressPointer(), 0, NPY_ARRAY_CARRAY, Nothing)
				log.debug("Created numpy array.")
				Return New PythonObject(npArr)
			End If


		End Function

		Public Overrides Function accepts(ByVal javaObject As Object) As Boolean
			Return TypeOf javaObject Is INDArray
		End Function

		Public Overrides Function adapt(ByVal javaObject As Object) As INDArray
			If TypeOf javaObject Is INDArray Then
				Return DirectCast(javaObject, INDArray)
			End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to INDArray")
		End Function
	End Class

End Namespace