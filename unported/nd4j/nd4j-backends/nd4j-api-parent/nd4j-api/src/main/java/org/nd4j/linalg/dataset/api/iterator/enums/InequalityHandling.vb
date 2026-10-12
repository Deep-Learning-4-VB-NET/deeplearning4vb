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

Namespace org.nd4j.linalg.dataset.api.iterator.enums

	Public Enum InequalityHandling
		''' <summary>
		''' Parallel iterator will stop everything once one of producers runs out of data
		''' </summary>
		STOP_EVERYONE

		''' <summary>
		''' Parallel iterator will keep returning true on hasNext(), but next() will return null instead of DataSet
		''' </summary>
		PASS_NULL

		''' <summary>
		''' Parallel iterator will silently reset underlying producer
		''' </summary>
		RESET

		''' <summary>
		''' Parallel iterator will ignore this producer, and will use other producers.
		''' 
		''' PLEASE NOTE: This option will invoke cross-device relocation in multi-device systems.
		''' </summary>
		RELOCATE
	End Enum

End Namespace