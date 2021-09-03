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

Namespace org.datavec.api.io.serializers


	Public Interface Serializer(Of T)
		''' <summary>
		''' <para>Prepare the serializer for writing.</para>
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void open(java.io.OutputStream out) throws java.io.IOException;
		Sub open(ByVal [out] As Stream)

		''' <summary>
		''' <para>Serialize <code>t</code> to the underlying output stream.</para>
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void serialize(T t) throws java.io.IOException;
		Sub serialize(ByVal t As T)

		''' <summary>
		''' <para>Close the underlying output stream and clear up any resources.</para>
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void close() throws java.io.IOException;
		Sub close()
	End Interface

End Namespace