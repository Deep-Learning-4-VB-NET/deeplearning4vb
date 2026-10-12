Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpaqueDataBuffer = org.nd4j.nativeblas.OpaqueDataBuffer
import static org.junit.jupiter.api.Assertions.assertEquals

'
' *
' *  *  ******************************************************************************
' *  *  *
' *  *  *
' *  *  * This program and the accompanying materials are made available under the
' *  *  * terms of the Apache License, Version 2.0 which is available at
' *  *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *  *
' *  *  *  See the NOTICE file distributed with this work for additional
' *  *  *  information regarding copyright ownership.
' *  *  * Unless required by applicable law or agreed to in writing, software
' *  *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  *  * License for the specific language governing permissions and limitations
' *  *  * under the License.
' *  *  *
' *  *  * SPDX-License-Identifier: Apache-2.0
' *  *  *****************************************************************************
' *
' *
' 

Namespace org.nd4j.python4j


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonNumpyBasicTest
	Public Class PythonNumpyBasicTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void init()
		Public Shared Sub init()
			Call (New NumpyArray()).init()
		End Sub

		Public Shared Function params() As Stream(Of Arguments)
			Dim types() As DataType = { DataType.BOOL, DataType.FLOAT16, DataType.BFLOAT16, DataType.FLOAT, DataType.DOUBLE, DataType.INT8, DataType.INT16, DataType.INT32, DataType.INT64, DataType.UINT8, DataType.UINT16, DataType.UINT32, DataType.UINT64 }

			Dim shapes()() As Long = {
				New Long(){2, 3},
				New Long(){3},
				New Long(){1},
				New Long(){}
			}


			Dim ret As IList(Of Object()) = New List(Of Object())()
			For Each type As DataType In types
				For Each shape As Long() In shapes
					ret.Add(New Object(){type, shape, Arrays.toString(shape)})
				Next shape
			Next type
			Return ret.Select(AddressOf Arguments.of)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.python4j.PythonNumpyBasicTest#params") public void testConversion(org.nd4j.linalg.api.buffer.DataType dataType,long[] shape)
		Public Overridable Sub testConversion(ByVal dataType As DataType, ByVal shape() As Long)
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
				Dim arr As INDArray = Nd4j.zeros(dataType, shape)
				Dim npArr As PythonObject = PythonTypes.convert(arr)
				Dim arr2 As INDArray = PythonTypes.getPythonTypeForPythonObject(Of INDArray)(npArr).toJava(npArr)
				If dataType = DataType.BFLOAT16 Then
					arr = arr.castTo(DataType.FLOAT)
				End If
				assertEquals(arr,arr2)
			End Using

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.python4j.PythonNumpyBasicTest#params") public void testExecution(org.nd4j.linalg.api.buffer.DataType dataType,long[] shape)
		Public Overridable Sub testExecution(ByVal dataType As DataType, ByVal shape() As Long)
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
				Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
				Dim x As INDArray = Nd4j.ones(dataType, shape)
				Dim y As INDArray = Nd4j.zeros(dataType, shape)
				Dim z As INDArray = If(dataType = DataType.BOOL, x, x.mul(y.add(2)))
				z = If(dataType = DataType.BFLOAT16, z.castTo(DataType.FLOAT), z)
				Dim arrType As PythonType(Of INDArray) = PythonTypes.get("numpy.ndarray")
				inputs.Add(New PythonVariable(Of )("x", arrType, x))
				inputs.Add(New PythonVariable(Of )("y", arrType, y))
				Dim outputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
				Dim output As New PythonVariable(Of INDArray)("z", arrType)
				outputs.Add(output)
				Dim code As String = If(dataType = DataType.BOOL, "z = x", "z = x * (y + 2)")
				If shape.Length = 0 Then ' scalar special case
					code &= vbLf & "import numpy as np" & vbLf & "z = np.asarray(float(z), dtype=x.dtype)"
				End If
				PythonExecutioner.exec(code, inputs, outputs)
				Dim z2 As INDArray = output.getValue()

				assertEquals(z.dataType(), z2.dataType())
				assertEquals(z, z2)
			End Using


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.python4j.PythonNumpyBasicTest#params") public void testInplaceExecution(org.nd4j.linalg.api.buffer.DataType dataType,long[] shape)
		Public Overridable Sub testInplaceExecution(ByVal dataType As DataType, ByVal shape() As Long)
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
				If dataType = DataType.BOOL OrElse dataType = DataType.BFLOAT16 Then
					Return
				End If
				If shape.Length = 0 Then
					Return
				End If
				Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
				Dim x As INDArray = Nd4j.ones(dataType, shape)
				Dim y As INDArray = Nd4j.zeros(dataType, shape)
				Dim z As INDArray = x.mul(y.add(2))
				' Nd4j.getAffinityManager().ensureLocation(z, AffinityManager.Location.HOST);
				Dim arrType As PythonType(Of INDArray) = PythonTypes.get("numpy.ndarray")
				inputs.Add(New PythonVariable(Of )("x", arrType, x))
				inputs.Add(New PythonVariable(Of )("y", arrType, y))
				Dim outputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
				Dim output As New PythonVariable(Of INDArray)("x", arrType)
				outputs.Add(output)
				Dim code As String = "x *= y + 2"
				PythonExecutioner.exec(code, inputs, outputs)
				Dim z2 As INDArray = output.getValue()
				assertEquals(x.dataType(), z2.dataType())
				assertEquals(z.dataType(), z2.dataType())
				assertEquals(x, z2)
				assertEquals(z, z2)
				assertEquals(x.data().pointer().address(), z2.data().pointer().address())
				If "CUDA".Equals(Nd4j.Executioner.EnvironmentInformation.getProperty("backend"), StringComparison.OrdinalIgnoreCase) Then
					assertEquals(getDeviceAddress(x), getDeviceAddress(z2))
				End If

			End Using


		End Sub


		Private Shared Function getDeviceAddress(ByVal array As INDArray) As Long
			If Not "CUDA".Equals(Nd4j.Executioner.EnvironmentInformation.getProperty("backend"), StringComparison.OrdinalIgnoreCase) Then
				Throw New System.InvalidOperationException("Cannot ge device pointer for non-CUDA device")
			End If

			'Use reflection here as OpaqueDataBuffer is only available on BaseCudaDataBuffer and BaseCpuDataBuffer - not DataBuffer/BaseDataBuffer
			' due to it being defined in nd4j-native-api, not nd4j-api
			Try
				Dim c As Type = Type.GetType("org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer")
				Dim m As System.Reflection.MethodInfo = c.GetMethod("getOpaqueDataBuffer")
				Dim db As OpaqueDataBuffer = CType(m.invoke(array.data()), OpaqueDataBuffer)
				Dim address As Long = db.specialBuffer().address()
				Return address
			Catch t As Exception
				Throw New Exception("Error getting OpaqueDataBuffer", t)
			End Try
		End Function




	End Class

End Namespace