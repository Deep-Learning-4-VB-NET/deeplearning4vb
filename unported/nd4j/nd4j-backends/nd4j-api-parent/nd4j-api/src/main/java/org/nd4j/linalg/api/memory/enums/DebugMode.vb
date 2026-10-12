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

	Public Enum DebugMode
		''' <summary>
		''' Default mode, means that workspaces work in production mode
		''' </summary>
		DISABLED

		''' <summary>
		''' All allocations will be considered spilled
		''' </summary>
		SPILL_EVERYTHING


		''' <summary>
		''' All workspaces will be disabled. There will be literally no way to enable workspace anywhere
		''' </summary>
		BYPASS_EVERYTHING
	End Enum

End Namespace