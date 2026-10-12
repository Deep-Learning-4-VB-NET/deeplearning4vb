Imports System.Collections.Generic

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

Namespace org.deeplearning4j.nn.layers


	Public Interface LayerHelper

		''' <summary>
		''' Return the currently allocated memory for the helper.<br>
		''' (a) Excludes: any shared memory used by multiple helpers/layers<br>
		''' (b) Excludes any temporary memory
		''' (c) Includes all memory that persists for longer than the helper method<br>
		''' This is mainly used for debugging and reporting purposes. Returns a map:<br>
		''' Key: The name of the type of memory<br>
		''' Value: The amount of memory<br>
		''' </summary>
		''' <returns> Map of memory, may be null if none is used. </returns>
		Function helperMemoryUse() As IDictionary(Of String, Long)

		Function checkSupported() As Boolean
	End Interface

End Namespace