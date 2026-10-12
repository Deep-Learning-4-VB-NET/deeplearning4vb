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

Namespace org.nd4j.parameterserver.distributed.v2.messages

	Public Interface MessagesHistoryHolder(Of T)
		''' <summary>
		''' This method adds id of the message to the storage, if message is unknown
		''' </summary>
		''' <param name="id"> </param>
		''' <returns> true if it's known message, false otherwise </returns>
		Function storeIfUnknownMessageId(ByVal id As T) As Boolean

		''' <summary>
		''' This method checks if given id was already seen before
		''' </summary>
		''' <param name="id"> </param>
		''' <returns> true if it's known message, false otherwise </returns>
		Function isKnownMessageId(ByVal id As T) As Boolean
	End Interface

End Namespace