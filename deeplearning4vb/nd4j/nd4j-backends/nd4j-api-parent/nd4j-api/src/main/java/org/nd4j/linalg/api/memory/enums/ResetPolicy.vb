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

Namespace org.nd4j.linalg.api.memory.enums

	Public Enum ResetPolicy
		''' <summary>
		''' This policy means - once end of MemoryWorkspace block reached its end - it'll be reset to the beginning.
		''' </summary>
		BLOCK_LEFT

		''' <summary>
		''' This policy means - this Workspace instance will be acting as
		''' circular buffer, so it'll be reset only after
		''' end of workspace buffer is reached.
		''' </summary>
		ENDOFBUFFER_REACHED
	End Enum

End Namespace