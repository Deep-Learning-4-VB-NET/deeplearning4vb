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


	Public Interface Deserializer(Of T)
		''' <summary>
		''' <para>Prepare the deserializer for reading.</para>
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void open(java.io.InputStream in) throws java.io.IOException;
		Sub open(ByVal [in] As Stream)

		''' <summary>
		''' <para>
		''' Deserialize the next object from the underlying input stream.
		''' If the object <code>t</code> is non-null then this deserializer
		''' <i>may</i> set its internal state to the next object read from the input
		''' stream. Otherwise, if the object <code>t</code> is null a new
		''' deserialized object will be created.
		''' </para> </summary>
		''' <returns> the deserialized object </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: T deserialize(T t) throws java.io.IOException;
		Function deserialize(ByVal t As T) As T

		''' <summary>
		''' <para>Close the underlying input stream and clear up any resources.</para>
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void close() throws java.io.IOException;
		Sub close()
	End Interface

End Namespace