Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DeviceLocalNDArray = org.nd4j.linalg.util.DeviceLocalNDArray

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

Namespace org.nd4j.autodiff.samediff.array


	Public Class ThreadSafeArrayHolder
		Implements ArrayHolder

		Private ReadOnly map As IDictionary(Of String, DeviceLocalNDArray) = New ConcurrentDictionary(Of String, DeviceLocalNDArray)()
		Private ReadOnly lazyInit As Boolean

		''' <param name="lazyInit"> If true: use lazy initialization for <seealso cref="DeviceLocalNDArray"/> </param>
		Public Sub New(ByVal lazyInit As Boolean)
			Me.lazyInit = lazyInit
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean hasArray(@NonNull String name)
		Public Overridable Function hasArray(ByVal name As String) As Boolean Implements ArrayHolder.hasArray
			Return map.ContainsKey(name)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray getArray(@NonNull String name)
		Public Overridable Function getArray(ByVal name As String) As INDArray Implements ArrayHolder.getArray
			Return map(name).get()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setArray(@NonNull String name, @NonNull INDArray array)
		Public Overridable Sub setArray(ByVal name As String, ByVal array As INDArray)
			If array.isView() Then
				array = array.dup() 'Device local doesn't support views
			End If
			If Not map.ContainsKey(name) Then
				Dim toBroadcast As INDArray = If(array.dataType() = DataType.UTF8, array.dup(), array)
				Dim dla As New DeviceLocalNDArray(toBroadcast, lazyInit)
				map(name) = dla
			Else
				Dim dla As DeviceLocalNDArray = map(name)
				dla.update(array)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray removeArray(@NonNull String name)
		Public Overridable Function removeArray(ByVal name As String) As INDArray Implements ArrayHolder.removeArray
			Dim arr As DeviceLocalNDArray = map.Remove(name)
			If arr Is Nothing Then
				Return Nothing
			End If
			Return arr.get()
		End Function

		Public Overridable Function size() As Integer Implements ArrayHolder.size
			Return map.Count
		End Function

		Public Overridable Sub initFrom(ByVal arrayHolder As ArrayHolder)
			map.Clear()
			Dim names As ICollection(Of String) = arrayHolder.arrayNames()
			For Each n As String In names
				setArray(n, arrayHolder.getArray(n))
			Next n
		End Sub

		Public Overridable Function arrayNames() As ICollection(Of String) Implements ArrayHolder.arrayNames
			Return Collections.unmodifiableCollection(map.Keys)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void rename(@NonNull String from, @NonNull String to)
		Public Overridable Sub rename(ByVal from As String, ByVal [to] As String) Implements ArrayHolder.rename
			Dim dl As DeviceLocalNDArray = map.Remove(from)
			map([to]) = dl
		End Sub
	End Class

End Namespace