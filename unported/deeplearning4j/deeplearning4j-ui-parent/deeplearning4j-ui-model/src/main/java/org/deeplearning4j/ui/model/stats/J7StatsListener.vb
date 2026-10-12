Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports StatsInitializationConfiguration = org.deeplearning4j.ui.model.stats.api.StatsInitializationConfiguration
Imports StatsInitializationReport = org.deeplearning4j.ui.model.stats.api.StatsInitializationReport
Imports StatsReport = org.deeplearning4j.ui.model.stats.api.StatsReport
Imports StatsUpdateConfiguration = org.deeplearning4j.ui.model.stats.api.StatsUpdateConfiguration
Imports FileStatsStorage = org.deeplearning4j.ui.model.storage.FileStatsStorage
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports JavaStorageMetaData = org.deeplearning4j.ui.model.storage.impl.JavaStorageMetaData
Imports DefaultStatsUpdateConfiguration = org.deeplearning4j.ui.model.stats.impl.DefaultStatsUpdateConfiguration
Imports JavaStatsInitializationReport = org.deeplearning4j.ui.model.stats.impl.java.JavaStatsInitializationReport
Imports JavaStatsReport = org.deeplearning4j.ui.model.stats.impl.java.JavaStatsReport

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

Namespace org.deeplearning4j.ui.model.stats

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class J7StatsListener extends BaseStatsListener
	<Serializable>
	Public Class J7StatsListener
		Inherits BaseStatsListener

		''' <summary>
		''' Create a StatsListener with network information collected at every iteration. Equivalent to <seealso cref="J7StatsListener(StatsStorageRouter, Integer)"/>
		''' with {@code listenerFrequency == 1}
		''' </summary>
		''' <param name="router"> Where/how to store the calculated stats. For example, <seealso cref="InMemoryStatsStorage"/> or
		'''               <seealso cref="FileStatsStorage"/> </param>
		Public Sub New(ByVal router As StatsStorageRouter)
			Me.New(router, Nothing, Nothing, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Create a StatsListener with network information collected every n >= 1 time steps
		''' </summary>
		''' <param name="router">            Where/how to store the calculated stats. For example, <seealso cref="InMemoryStatsStorage"/> or
		'''                          <seealso cref="FileStatsStorage"/> </param>
		''' <param name="listenerFrequency"> Frequency with which to collect stats information </param>
		Public Sub New(ByVal router As StatsStorageRouter, ByVal listenerFrequency As Integer)
			Me.New(router, Nothing, (New DefaultStatsUpdateConfiguration.Builder()).reportingFrequency(listenerFrequency).build(), Nothing, Nothing)
		End Sub

		Public Sub New(ByVal router As StatsStorageRouter, ByVal initConfig As StatsInitializationConfiguration, ByVal updateConfig As StatsUpdateConfiguration, ByVal sessionID As String, ByVal workerID As String)
			MyBase.New(router, initConfig, updateConfig, sessionID, workerID)
		End Sub

		Public Overrides ReadOnly Property NewInitializationReport As StatsInitializationReport
			Get
				Return New JavaStatsInitializationReport()
			End Get
		End Property

		Public Overrides ReadOnly Property NewStatsReport As StatsReport
			Get
				Return New JavaStatsReport()
			End Get
		End Property

		Public Overrides Function getNewStorageMetaData(ByVal initTime As Long, ByVal sessionID As String, ByVal workerID As String) As StorageMetaData
			Return New JavaStorageMetaData(initTime, sessionID, TYPE_ID, workerID, GetType(JavaStatsInitializationReport), GetType(JavaStatsReport))
		End Function

		Public Overrides Function clone() As J7StatsListener
			Return New J7StatsListener(Me.StorageRouter, Me.InitConfig, Me.UpdateConfig, Nothing, Nothing)
		End Function
	End Class

End Namespace