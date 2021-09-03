Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Broadcast = org.apache.spark.broadcast.Broadcast

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

Namespace org.datavec.spark.util

	Public Class BroadcastHadoopConfigHolder

		Private Shared config As Broadcast(Of SerializableHadoopConfig)
		Private Shared sparkContextStartTime As Long = -1 'Used to determine if spark context has changed - usually only use multiple spark contexts in tests etc

		Private Sub New()
		End Sub

		Public Shared Function get(ByVal sc As JavaSparkContext) As Broadcast(Of SerializableHadoopConfig)
			If config IsNot Nothing AndAlso (Not config.isValid() OrElse sc.startTime() <> sparkContextStartTime) Then
				config = Nothing
			End If
			If config IsNot Nothing Then
				Return config
			End If
			SyncLock GetType(BroadcastHadoopConfigHolder)
				If config Is Nothing Then
					config = sc.broadcast(New SerializableHadoopConfig(sc.hadoopConfiguration()))
					sparkContextStartTime = sc.startTime()
				End If
			End SyncLock
			Return config
		End Function
	End Class

End Namespace