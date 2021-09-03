Imports System
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StatsStorageRouterProvider = org.deeplearning4j.core.storage.StatsStorageRouterProvider

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

Namespace org.deeplearning4j.spark.impl.listeners

	<Serializable>
	Public Class VanillaStatsStorageRouterProvider
		Implements StatsStorageRouterProvider

'JAVA TO VB CONVERTER NOTE: The field router was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private router_Conflict As StatsStorageRouter = Nothing

		Public Overridable ReadOnly Property Router As StatsStorageRouter
			Get
				SyncLock Me
					If router_Conflict Is Nothing Then
						router_Conflict = New VanillaStatsStorageRouter()
					End If
					Return router_Conflict
				End SyncLock
			End Get
		End Property
	End Class

End Namespace