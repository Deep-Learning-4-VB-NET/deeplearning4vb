Imports System
Imports Getter = lombok.Getter

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

Namespace org.deeplearning4j.spark.stats

	<Serializable>
	Public Class PartitionCountEventStats
		Inherits BaseEventStats

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final int numPartitions;
		Private ReadOnly numPartitions As Integer

		Public Sub New(ByVal startTime As Long, ByVal durationMs As Long, ByVal numPartitions As Integer)
			MyBase.New(startTime, durationMs)
			Me.numPartitions = numPartitions
		End Sub

		Public Sub New(ByVal machineId As String, ByVal jvmId As String, ByVal threadId As Long, ByVal startTime As Long, ByVal durationMs As Long, ByVal numPartitions As Integer)
			MyBase.New(machineId, jvmId, threadId, startTime, durationMs)
			Me.numPartitions = numPartitions
		End Sub

		Public Overrides Function asString(ByVal delimiter As String) As String
			Return MyBase.asString(delimiter) & delimiter & numPartitions
		End Function

		Public Overrides Function getStringHeader(ByVal delimiter As String) As String
			Return MyBase.getStringHeader(delimiter) & delimiter & "numPartitions"
		End Function
	End Class

End Namespace