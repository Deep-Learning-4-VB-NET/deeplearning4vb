Imports org.datavec.api.io

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


	Public Class NullWritable
		Implements WritableComparable

		Public Shared ReadOnly INSTANCE As New NullWritable()


		Public Overrides Function compareTo(ByVal o As Object) As Integer
			If Me Is o Then
				Return 0
			End If
			If Not (TypeOf o Is NullWritable) Then
				Throw New System.ArgumentException("Cannot compare NullWritable to " & o.GetType())
			End If
			Return 0
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return TypeOf o Is NullWritable
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)
			'No op
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)
			'No op
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			[out].writeShort(WritableType.Null.typeIdx())
		End Sub

		Public Overridable Function toDouble() As Double
			Throw New System.NotSupportedException("Cannot convert NullWritable to other values")
		End Function

		Public Overridable Function toFloat() As Single
			Throw New System.NotSupportedException("Cannot convert NullWritable to other values")
		End Function

		Public Overridable Function toInt() As Integer
			Throw New System.NotSupportedException("Cannot convert NullWritable to other values")
		End Function

		Public Overridable Function toLong() As Long
			Throw New System.NotSupportedException("Cannot convert NullWritable to other values")
		End Function

		Public Overridable Function [getType]() As WritableType
			Return WritableType.Null
		End Function

		Public Overrides Function ToString() As String
			Return "NullWritable"
		End Function
	End Class

End Namespace