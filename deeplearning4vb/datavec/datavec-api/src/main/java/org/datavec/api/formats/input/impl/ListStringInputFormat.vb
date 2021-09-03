Imports System
Imports Configuration = org.datavec.api.conf.Configuration
Imports InputFormat = org.datavec.api.formats.input.InputFormat
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports ListStringRecordReader = org.datavec.api.records.reader.impl.collection.ListStringRecordReader
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

Namespace org.datavec.api.formats.input.impl


	<Serializable>
	Public Class ListStringInputFormat
		Implements InputFormat

		''' <summary>
		''' Creates a reader from an input split
		''' </summary>
		''' <param name="split"> the split to read </param>
		''' <param name="conf"> </param>
		''' <returns> the reader from the given input split </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.reader.RecordReader createReader(org.datavec.api.split.InputSplit split, org.datavec.api.conf.Configuration conf) throws IOException, InterruptedException
		Public Overridable Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader Implements InputFormat.createReader
			Dim reader As RecordReader = New ListStringRecordReader()
			reader.initialize(conf, split)
			Return reader
		End Function

		''' <summary>
		''' Creates a reader from an input split
		''' </summary>
		''' <param name="split"> the split to read </param>
		''' <returns> the reader from the given input split </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.reader.RecordReader createReader(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Function createReader(ByVal split As InputSplit) As RecordReader Implements InputFormat.createReader
			Dim reader As RecordReader = New ListStringRecordReader()
			reader.initialize(split)
			Return reader
		End Function

		''' <summary>
		''' Serialize the fields of this object to <code>out</code>.
		''' </summary>
		''' <param name="out"> <code>DataOuput</code> to serialize this object into. </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)

		End Sub

		''' <summary>
		''' Deserialize the fields of this object from <code>in</code>.
		''' <para>
		''' </para>
		''' <para>For efficiency, implementations should attempt to re-use storage in the
		''' existing object where possible.</para>
		''' </summary>
		''' <param name="in"> <code>DataInput</code> to deseriablize this object from. </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)

		End Sub

		''' <summary>
		''' Convert Writable to double. Whether this is supported depends on the specific writable.
		''' </summary>
		Public Overridable Function toDouble() As Double
			Return 0
		End Function

		''' <summary>
		''' Convert Writable to float. Whether this is supported depends on the specific writable.
		''' </summary>
		Public Overridable Function toFloat() As Single
			Return 0
		End Function

		''' <summary>
		''' Convert Writable to int. Whether this is supported depends on the specific writable.
		''' </summary>
		Public Overridable Function toInt() As Integer
			Return 0
		End Function

		''' <summary>
		''' Convert Writable to long. Whether this is supported depends on the specific writable.
		''' </summary>
		Public Overridable Function toLong() As Long
			Return 0
		End Function

		Public Overridable Function [getType]() As WritableType
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace