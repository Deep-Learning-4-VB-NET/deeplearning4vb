Imports System.IO

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

Namespace org.deeplearning4j.core.storage


	Public Interface Persistable

		''' <summary>
		''' Get the session id
		''' @return
		''' </summary>
		ReadOnly Property SessionID As String

		''' <summary>
		''' Get the type id
		''' @return
		''' </summary>
		ReadOnly Property TypeID As String

		''' <summary>
		''' Get the worker id
		''' @return
		''' </summary>
		ReadOnly Property WorkerID As String

		''' <summary>
		''' Get when this was created.
		''' @return
		''' </summary>
		ReadOnly Property TimeStamp As Long


		'SerDe methods:

		''' <summary>
		''' Length of the encoding, in bytes, when using <seealso cref="encode()"/>
		''' Length may be different using <seealso cref="encode(OutputStream)"/>, due to things like stream headers
		''' @return
		''' </summary>
		Function encodingLengthBytes() As Integer

		Function encode() As SByte()

		''' <summary>
		''' Encode this persistable in to a <seealso cref="ByteBuffer"/> </summary>
		''' <param name="buffer"> </param>
		Sub encode(ByVal buffer As ByteBuffer)

		''' <summary>
		''' Encode this persistable in to an output stream </summary>
		''' <param name="outputStream"> </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void encode(java.io.OutputStream outputStream) throws java.io.IOException;
		Sub encode(ByVal outputStream As Stream)

		''' <summary>
		''' Decode the content of the given
		''' byte array in to this persistable </summary>
		''' <param name="decode"> </param>
		Sub decode(ByVal decode() As SByte)

		''' <summary>
		''' Decode from the given <seealso cref="ByteBuffer"/> </summary>
		''' <param name="buffer"> </param>
		Sub decode(ByVal buffer As ByteBuffer)

		''' <summary>
		''' Decode from the given input stream </summary>
		''' <param name="inputStream"> </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void decode(java.io.InputStream inputStream) throws java.io.IOException;
		Sub decode(ByVal inputStream As Stream)

	End Interface

End Namespace