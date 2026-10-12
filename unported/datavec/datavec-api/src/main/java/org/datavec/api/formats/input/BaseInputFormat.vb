Imports System
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports WritableType = org.datavec.api.writable.WritableType

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

Namespace org.datavec.api.formats.input


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public MustInherit Class BaseInputFormat
		Implements InputFormat

		Public MustOverride Function createReader(ByVal split As InputSplit, ByVal conf As org.datavec.api.conf.Configuration) As RecordReader Implements InputFormat.createReader

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.reader.RecordReader createReader(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Function createReader(ByVal split As InputSplit) As RecordReader Implements InputFormat.createReader
			Return createReader(split, Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)

		End Sub

		Public Overridable Function toDouble() As Double
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toFloat() As Single
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toInt() As Integer
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toLong() As Long
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function [getType]() As WritableType
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace