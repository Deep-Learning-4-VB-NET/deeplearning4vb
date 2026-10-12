Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface Writable extends java.io.Serializable
	Public Interface Writable
		''' <summary>
		''' Serialize the fields of this object to <code>out</code>.
		''' </summary>
		''' <param name="out"> <code>DataOuput</code> to serialize this object into. </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void write(java.io.DataOutput out) throws java.io.IOException;
		Sub write(ByVal [out] As DataOutput)

		''' <summary>
		''' Deserialize the fields of this object from <code>in</code>.
		''' 
		''' <para>For efficiency, implementations should attempt to re-use storage in the
		''' existing object where possible.</para>
		''' </summary>
		''' <param name="in"> <code>DataInput</code> to deseriablize this object from. </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void readFields(java.io.DataInput in) throws java.io.IOException;
		Sub readFields(ByVal [in] As DataInput)

		''' <summary>
		''' Write the type (a single short value) to the DataOutput. See <seealso cref="WritableFactory"/> for details.
		''' </summary>
		''' <param name="out"> DataOutput to write to </param>
		''' <exception cref="IOException"> For errors during writing </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void writeType(java.io.DataOutput out) throws java.io.IOException;
		Sub writeType(ByVal [out] As DataOutput)

		''' <summary>
		''' Convert Writable to double. Whether this is supported depends on the specific writable. </summary>
		Function toDouble() As Double

		''' <summary>
		''' Convert Writable to float. Whether this is supported depends on the specific writable. </summary>
		Function toFloat() As Single

		''' <summary>
		''' Convert Writable to int. Whether this is supported depends on the specific writable. </summary>
		Function toInt() As Integer

		''' <summary>
		''' Convert Writable to long. Whether this is supported depends on the specific writable. </summary>
		Function toLong() As Long

		''' <summary>
		''' Get the type of the writable.
		''' @return
		''' </summary>
		Function [getType]() As WritableType

	End Interface

End Namespace