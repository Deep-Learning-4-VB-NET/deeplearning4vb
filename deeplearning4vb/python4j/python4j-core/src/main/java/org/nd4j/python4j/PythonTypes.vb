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



	Public Class PythonTypes


		Private Shared ReadOnly Property PrimitiveTypes As IList(Of PythonType)
			Get
				Return New List(Of PythonType) From {Of PythonType}
			End Get
		End Property

		Private Shared ReadOnly Property CollectionTypes As IList(Of PythonType)
			Get
				Return New List(Of PythonType) From {Of PythonType}
			End Get
		End Property

		Private Shared ReadOnly Property ExternalTypes As IList(Of PythonType)
			Get
				Dim ret As IList(Of PythonType) = New List(Of PythonType)()
				Dim sl As ServiceLoader(Of PythonType) = ServiceLoader.load(GetType(PythonType))
				Dim iter As IEnumerator(Of PythonType) = sl.GetEnumerator()
				Do While iter.MoveNext()
					ret.Add(iter.Current)
				Loop
				Return ret
			End Get
		End Property

		Public Shared Function get() As IList(Of PythonType)
			Dim ret As IList(Of PythonType) = New List(Of PythonType)()
			CType(ret, List(Of PythonType)).AddRange(getPrimitiveTypes())
			CType(ret, List(Of PythonType)).AddRange(getCollectionTypes())
			CType(ret, List(Of PythonType)).AddRange(getExternalTypes())
			Return ret
		End Function

		Public Shared Function get(Of T)(ByVal name As String) As PythonType(Of T)
			For Each pt As PythonType In get()
				If pt.getName().Equals(name) Then ' TODO use map instead?
					Return pt
				End If

			Next pt
			Throw New PythonException("Unknown python type: " & name)
		End Function


		Public Shared Function getPythonTypeForJavaObject(ByVal javaObject As Object) As PythonType
			For Each pt As PythonType In get()
				If pt.accepts(javaObject) Then
					Return pt
				End If
			Next pt
			Throw New PythonException("Unable to find python type for java type: " & javaObject.GetType())
		End Function

		Public Shared Function getPythonTypeForPythonObject(Of T)(ByVal pythonObject As PythonObject) As PythonType(Of T)
			Dim pyType As PyObject = PyObject_Type(pythonObject.NativePythonObject)
			Try
				Dim pyTypeStr As String = PythonTypes.STR.toJava(New PythonObject(pyType, False))

				For Each pt As PythonType In get()
					Dim pyTypeStr2 As String = "<class '" & pt.getName() & "'>"
					If pyTypeStr.Equals(pyTypeStr2) Then
						Return pt
					Else
						Using gc As PythonGC = PythonGC.watch()
							Dim pyType2 As PythonObject = pt.pythonType()
							If pyType2 IsNot Nothing AndAlso Python.isinstance(pythonObject, pyType2) Then
								Return pt
							End If
						End Using

					End If
				Next pt
				Throw New PythonException("Unable to find converter for python object of type " & pyTypeStr)
			Finally
				Py_DecRef(pyType)
			End Try


		End Function

		Public Shared Function convert(ByVal javaObject As Object) As PythonObject
			Dim pt As PythonType = getPythonTypeForJavaObject(javaObject)
			Return pt.toPython(pt.adapt(javaObject))
		End Function

		Public Shared ReadOnly STR As PythonType(Of String) = New PythonTypeAnonymousInnerClass(GetType(String))

		Private Class PythonTypeAnonymousInnerClass
			Inherits PythonType(Of String)

			Public PythonTypeAnonymousInnerClass(java.lang.Class Class)
				Private ReadOnly outerInstance As PythonTypes.PythonTypeAnonymousInnerClass

'JAVA TO VB CONVERTER WARNING: The following constructor is declared outside of its associated class:
'ORIGINAL LINE: public ()
				Sub New(ByVal outerInstance As PythonTypes.PythonTypeAnonymousInnerClass)
					Me.outerInstance = outerInstance
				End Sub

				MyBase("str", class)
			End Class


			public String adapt(Object javaObject)
			If True Then
				If TypeOf javaObject Is String Then
					Return CStr(javaObject)
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New PythonException("Cannot cast object of type " & javaObject.GetType().FullName & " to String")
			End If

			public String toJava(PythonObject pythonObject)
			If True Then
				PythonGIL.assertThreadSafe()
				Dim repr As PyObject = PyObject_Str(pythonObject.getNativePythonObject())
				Dim str As PyObject = PyUnicode_AsEncodedString(repr, "utf-8", "~E~")
				Dim jstr As String = PyBytes_AsString(str).getString()
				Py_DecRef(repr)
				Py_DecRef(str)
				Return jstr
			End If

			public PythonObject toPython(String javaObject)
			If True Then
				Return New PythonObject(PyUnicode_FromString(javaObject))
			End If
		End Class

		public static final PythonType(Of Long) INT = New PythonTypeAnonymousInnerClass(Me, GetType(Long))

		public static final PythonType(Of Double) FLOAT = New PythonTypeAnonymousInnerClass2(Me, GetType(Double))


		public static final PythonType(Of Boolean) BOOL = New PythonTypeAnonymousInnerClass3(Me, GetType(Boolean))


		public static final PythonType(Of System.Collections.IList) LIST = New PythonTypeAnonymousInnerClass4(Me, GetType(System.Collections.IList))

		public static final PythonType(Of System.Collections.IDictionary) DICT = New PythonTypeAnonymousInnerClass5(Me, GetType(System.Collections.IDictionary))


		public static final PythonType(Of SByte()) BYTES = New PythonTypeAnonymousInnerClass6(Me)

		''' <summary>
		''' Crashes on Adopt OpenJDK
		''' Use implementation in python4j-numpy instead for zero-copy byte buffers.
		''' </summary>
	'    public static final PythonType<BytePointer> MEMORYVIEW = new PythonType<BytePointer>("memoryview", BytePointer.class) {
	'        @Override
	'        public BytePointer toJava(PythonObject pythonObject) {
	'            try (PythonGC gc = PythonGC.watch()) {
	'                if (!(Python.isinstance(pythonObject, Python.memoryviewType()))) {
	'                    throw new PythonException("Expected memoryview. Received: " + pythonObject);
	'                }
	'                PythonObject pySize = Python.len(pythonObject);
	'                PythonObject ctypes = Python.importModule("ctypes");
	'                PythonObject charType = ctypes.attr("c_char");
	'                PythonObject charArrayType = new PythonObject(PyNumber_Multiply(charType.getNativePythonObject(),
	'                        pySize.getNativePythonObject()));
	'                PythonObject fromBuffer = charArrayType.attr("from_buffer");
	'                if (pythonObject.attr("readonly").toBoolean()) {
	'                    pythonObject = Python.bytearray(pythonObject);
	'                }
	'                PythonObject arr = fromBuffer.call(pythonObject);
	'                PythonObject cast = ctypes.attr("cast");
	'                PythonObject voidPtrType = ctypes.attr("c_void_p");
	'                PythonObject voidPtr = cast.call(arr, voidPtrType);
	'                long address = voidPtr.attr("value").toLong();
	'                long size = pySize.toLong();
	'                try {
	'                    Field addressField = Buffer.class.getDeclaredField("address");
	'                    addressField.setAccessible(true);
	'                    Field capacityField = Buffer.class.getDeclaredField("capacity");
	'                    capacityField.setAccessible(true);
	'                    ByteBuffer buff = ByteBuffer.allocateDirect(0).order(ByteOrder.nativeOrder());
	'                    addressField.setLong(buff, address);
	'                    capacityField.setInt(buff, (int) size);
	'                    BytePointer ret = new BytePointer(buff);
	'                    ret.limit(size);
	'                    return ret;
	'
	'                } catch (Exception e) {
	'                    throw new RuntimeException(e);
	'                }
	'
	'            }
	'        }
	'
	'        @Override
	'        public PythonObject toPython(BytePointer javaObject) {
	'            long address = javaObject.address();
	'            long size = javaObject.limit();
	'            try (PythonGC gc = PythonGC.watch()) {
	'                PythonObject ctypes = Python.importModule("ctypes");
	'                PythonObject charType = ctypes.attr("c_char");
	'                PythonObject pySize = new PythonObject(size);
	'                PythonObject charArrayType = new PythonObject(PyNumber_Multiply(charType.getNativePythonObject(),
	'                        pySize.getNativePythonObject()));
	'                PythonObject fromAddress = charArrayType.attr("from_address");
	'                PythonObject arr = fromAddress.call(new PythonObject(address));
	'                PythonObject memoryView = Python.memoryview(arr).attr("cast").call("b");
	'                PythonGC.keep(memoryView);
	'                return memoryView;
	'            }
	'
	'        }
	'
	'        @Override
	'        public boolean accepts(Object javaObject) {
	'            return javaObject instanceof Pointer || javaObject instanceof DirectBuffer;
	'        }
	'
	'        @Override
	'        public BytePointer adapt(Object javaObject) {
	'            if (javaObject instanceof BytePointer) {
	'                return (BytePointer) javaObject;
	'            } else if (javaObject instanceof Pointer) {
	'                return new BytePointer((Pointer) javaObject);
	'            } else if (javaObject instanceof DirectBuffer) {
	'                return new BytePointer((ByteBuffer) javaObject);
	'            } else {
	'                throw new PythonException("Cannot cast object of type " + javaObject.getClass().getName() + " to BytePointer");
	'            }
	'        }
	'    };

	End Class

End Namespace