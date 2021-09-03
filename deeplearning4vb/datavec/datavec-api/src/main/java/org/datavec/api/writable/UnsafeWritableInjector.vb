Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.datavec.api.writable

	Public Class UnsafeWritableInjector
		Public Shared Function inject(Of T)(ByVal x As T) As Writable
			If x Is Nothing Then
				Return NullWritable.INSTANCE
			ElseIf TypeOf x Is Writable Then
				Return DirectCast(x, Writable)
			ElseIf TypeOf x Is INDArray Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Wrong argument of type INDArray (" & x.GetType().FullName & ") please use org.datavec.common.data.NDArrayWritable manually to convert.")
			ElseIf x.GetType() = GetType(Integer) Then
				Return New IntWritable(CType(x, Integer?))
			ElseIf x.GetType() = GetType(Long) Then
				Return New LongWritable(CType(x, Long?))
			ElseIf x.GetType() = GetType(Float) Then
				Return New FloatWritable(CType(x, Single?))
			ElseIf x.GetType() = GetType(Double) Then
				Return New DoubleWritable(CType(x, Double?))
			ElseIf TypeOf x Is String Then
				Return New Text(CStr(x))
			ElseIf TypeOf x Is SByte? Then
				Return New ByteWritable(CType(x, SByte?))
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Wrong argument type for writable conversion " & x.GetType().FullName)
			End If
		End Function
	End Class

End Namespace