Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener

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

Namespace org.deeplearning4j.ui.model.stats.api


	Public Interface StatsInitializationConfiguration

		''' <summary>
		''' Should software configuration information be collected? For example, OS, JVM, and ND4J backend details
		''' </summary>
		''' <returns> true if software information should be collected; false if not </returns>
		Function collectSoftwareInfo() As Boolean

		''' <summary>
		''' Should hardware configuration information be collected? JVM available processors, number of devices, total memory for each device
		''' </summary>
		''' <returns> true if hardware information should be collected </returns>
		Function collectHardwareInfo() As Boolean

		''' <summary>
		''' Should model information be collected? Model class, configuration (JSON), number of layers, number of parameters, etc.
		''' </summary>
		''' <returns> true if model information should be collected </returns>
		Function collectModelInfo() As Boolean

	End Interface

End Namespace