Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData

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

Namespace org.deeplearning4j.core.storage.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class CollectionStatsStorageRouter implements org.deeplearning4j.core.storage.StatsStorageRouter
	Public Class CollectionStatsStorageRouter
		Implements StatsStorageRouter

		Private metaDatas As ICollection(Of StorageMetaData)
		Private staticInfos As ICollection(Of Persistable)
		Private updates As ICollection(Of Persistable)


		Public Overridable Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData)
			Me.metaDatas.Add(storageMetaData)
		End Sub

		Public Overridable Sub putStorageMetaData(Of T1 As StorageMetaData)(ByVal storageMetaData As ICollection(Of T1)) Implements StatsStorageRouter.putStorageMetaData
			Me.metaDatas.addAll(storageMetaData)
		End Sub

		Public Overridable Sub putStaticInfo(ByVal staticInfo As Persistable)
			Me.staticInfos.Add(staticInfo)
		End Sub

		Public Overridable Sub putStaticInfo(Of T1 As Persistable)(ByVal staticInfo As ICollection(Of T1)) Implements StatsStorageRouter.putStaticInfo
			Me.staticInfos.addAll(staticInfo)
		End Sub

		Public Overridable Sub putUpdate(ByVal update As Persistable)
			Me.updates.Add(update)
		End Sub

		Public Overridable Sub putUpdate(Of T1 As Persistable)(ByVal updates As ICollection(Of T1)) Implements StatsStorageRouter.putUpdate
			Me.updates.addAll(updates)
		End Sub
	End Class

End Namespace