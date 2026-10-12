Imports System
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.deeplearning4j.spark.parameterserver.python


	<Serializable>
	Public Class ArrayDescriptor

'JAVA TO VB CONVERTER NOTE: The field address was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private address_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field shape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private shape_Conflict() As Long
'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private stride_Conflict() As Long
		Friend type As DataType
'JAVA TO VB CONVERTER NOTE: The field ordering was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend ordering_Conflict As Char
		Private Shared nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public ArrayDescriptor(org.nd4j.linalg.api.ndarray.INDArray array) throws Exception
		Public Sub New(ByVal array As INDArray)
			Me.New(array.data().address(), array.shape(), array.stride(), array.data().dataType(), array.ordering())
			If array.Empty Then
				Throw New System.NotSupportedException("Empty arrays are not supported")
			End If
		End Sub

		Public Sub New(ByVal address As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal type As DataType, ByVal ordering As Char)
			Me.address_Conflict = address
			Me.shape_Conflict = shape
			Me.stride_Conflict = stride
			Me.type = type
			Me.ordering_Conflict = ordering
		End Sub
		Public Overridable ReadOnly Property Address As Long
			Get
				Return address_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Shape As Long()
			Get
				Return shape_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Stride As Long()
			Get
				Return stride_Conflict
			End Get
		End Property

		Public Overridable Function [getType]() As DataType
			Return type
		End Function

		Public Overridable ReadOnly Property Ordering As Char
			Get
				Return ordering_Conflict
			End Get
		End Property

		Private Function size() As Long
			Dim s As Long = 1
			For Each d As Long In shape_Conflict
				s *= d
			Next d
			Return s
		End Function

		Public Overridable ReadOnly Property Array As INDArray
			Get
				Dim ptr As Pointer = nativeOps.pointerForAddress(address_Conflict)
				ptr = ptr.limit(size())
				Dim buff As DataBuffer = Nd4j.createBuffer(ptr, size(), type)
				Return Nd4j.create(buff, shape_Conflict, stride_Conflict, 0, ordering_Conflict, type)
			End Get
		End Property

	End Class

End Namespace