Imports System

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

	<Serializable>
	Public MustInherit Class ArrayWritable
		Implements Writable

		Public MustOverride Function [getType]() As WritableType Implements Writable.getType
		Public MustOverride Sub writeType(ByVal [out] As java.io.DataOutput) Implements Writable.writeType
		Public MustOverride Sub readFields(ByVal [in] As java.io.DataInput) Implements Writable.readFields
		Public MustOverride Sub write(ByVal [out] As java.io.DataOutput) Implements Writable.write

		Public MustOverride Function length() As Long

		Public MustOverride Function getDouble(ByVal i As Long) As Double

		Public MustOverride Function getFloat(ByVal i As Long) As Single

		Public MustOverride Function getInt(ByVal i As Long) As Integer

		Public MustOverride Function getLong(ByVal i As Long) As Long

		Public Overridable Function toDouble() As Double Implements Writable.toDouble
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toFloat() As Single Implements Writable.toFloat
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toInt() As Integer Implements Writable.toInt
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toLong() As Long Implements Writable.toLong
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace