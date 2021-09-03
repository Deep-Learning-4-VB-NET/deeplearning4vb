Imports System.IO
Imports AllArgsConstructor = lombok.AllArgsConstructor

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

Namespace org.datavec.api.util.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class DataInputWrapperStream extends java.io.InputStream
	Public Class DataInputWrapperStream
		Inherits Stream

		Private ReadOnly underlying As DataInput


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public int read() throws java.io.IOException
		Public Overrides Function read() As Integer
	'        From InputStream.read() javadoc:
	'        "Reads the next byte of data from the input stream. The value byte is
	'         returned as an <code>int</code> in the range <code>0</code> to
	'         <code>255</code>."
	'         Therefore: we need to use readUnsignedByte(), with returns 0 to 255. readByte() returns -128 to 127
	'         
			Return underlying.readUnsignedByte()
		End Function
	End Class

End Namespace