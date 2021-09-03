Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports StatsInitializationConfiguration = org.deeplearning4j.ui.model.stats.api.StatsInitializationConfiguration

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

Namespace org.deeplearning4j.ui.model.stats.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class DefaultStatsInitializationConfiguration implements org.deeplearning4j.ui.model.stats.api.StatsInitializationConfiguration
	<Serializable>
	Public Class DefaultStatsInitializationConfiguration
		Implements StatsInitializationConfiguration

'JAVA TO VB CONVERTER NOTE: The field collectSoftwareInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly collectSoftwareInfo_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field collectHardwareInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly collectHardwareInfo_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field collectModelInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly collectModelInfo_Conflict As Boolean

		Public Overridable Function collectSoftwareInfo() As Boolean Implements StatsInitializationConfiguration.collectSoftwareInfo
			Return collectSoftwareInfo_Conflict
		End Function

		Public Overridable Function collectHardwareInfo() As Boolean Implements StatsInitializationConfiguration.collectHardwareInfo
			Return collectHardwareInfo_Conflict
		End Function

		Public Overridable Function collectModelInfo() As Boolean Implements StatsInitializationConfiguration.collectModelInfo
			Return collectModelInfo_Conflict
		End Function
	End Class

End Namespace