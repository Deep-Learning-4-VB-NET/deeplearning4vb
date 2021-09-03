Imports System
Imports lombok
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage

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

Namespace org.nd4j.parameterserver.distributed.v2.chunks


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Builder public class VoidChunk implements org.nd4j.parameterserver.distributed.v2.messages.VoidMessage
	<Serializable>
	Public Class VoidChunk
		Implements VoidMessage

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int chunkId;
		Private chunkId As Integer

		''' <summary>
		''' This field contains number of bytes of original message
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long totalSize;
		Private totalSize As Long

		''' <summary>
		''' Unique messageId used to distringuish chunks from each other
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String messageId;
		Private messageId As String

		''' <summary>
		''' This field stores split size for original message
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int splitSize;
		Private splitSize As Integer

		''' <summary>
		''' This field holds ID of the original message
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String originalId;
		Private originalId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private String originatorId;
		Private originatorId As String

		''' <summary>
		''' This field holds number of chunks for original message
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int numberOfChunks;
		Private numberOfChunks As Integer

		''' <summary>
		''' Actual chunk
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private byte[] payload;
		Private payload() As SByte
	End Class

End Namespace