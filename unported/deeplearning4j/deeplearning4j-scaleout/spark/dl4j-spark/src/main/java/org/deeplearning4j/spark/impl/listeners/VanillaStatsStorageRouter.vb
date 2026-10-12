Imports System.Collections.Generic
Imports Data = lombok.Data
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

Namespace org.deeplearning4j.spark.impl.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class VanillaStatsStorageRouter implements org.deeplearning4j.core.storage.StatsStorageRouter
	Public Class VanillaStatsStorageRouter
		Implements StatsStorageRouter

		Private ReadOnly storageMetaData As IList(Of StorageMetaData) = Collections.synchronizedList(New List(Of StorageMetaData)())
		Private ReadOnly staticInfo As IList(Of Persistable) = Collections.synchronizedList(New List(Of Persistable)())
		Private ReadOnly updates As IList(Of Persistable) = Collections.synchronizedList(New List(Of Persistable)())

		Public Overridable Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData)
			Me.storageMetaData.Add(storageMetaData)
		End Sub

		Public Overridable Sub putStorageMetaData(Of T1 As StorageMetaData)(ByVal storageMetaData As ICollection(Of T1)) Implements StatsStorageRouter.putStorageMetaData
			CType(Me.storageMetaData, List(Of StorageMetaData)).AddRange(storageMetaData)
		End Sub

		Public Overridable Sub putStaticInfo(ByVal staticInfo As Persistable)
			Me.staticInfo.Add(staticInfo)
		End Sub

		Public Overridable Sub putStaticInfo(Of T1 As Persistable)(ByVal staticInfo As ICollection(Of T1)) Implements StatsStorageRouter.putStaticInfo
			CType(Me.staticInfo, List(Of Persistable)).AddRange(staticInfo)
		End Sub

		Public Overridable Sub putUpdate(ByVal update As Persistable)
			Me.updates.Add(update)
		End Sub

		Public Overridable Sub putUpdate(Of T1 As Persistable)(ByVal updates As ICollection(Of T1)) Implements StatsStorageRouter.putUpdate
			CType(Me.updates, List(Of Persistable)).AddRange(updates)
		End Sub


		Public Overridable ReadOnly Property StorageMetaData As IList(Of StorageMetaData)
			Get
				'We can't return synchronized lists list this for Kryo: with default config, it will fail to deserialize the
				' synchronized lists, throwing an obscure null pointer exception
				Return New List(Of StorageMetaData)(storageMetaData)
			End Get
		End Property

		Public Overridable ReadOnly Property StaticInfo As IList(Of Persistable)
			Get
				'We can't return synchronized lists list this for Kryo: with default config, it will fail to deserialize the
				' synchronized lists, throwing an obscure null pointer exception
				Return New List(Of Persistable)(staticInfo)
			End Get
		End Property

		Public Overridable ReadOnly Property Updates As IList(Of Persistable)
			Get
				'We can't return synchronized lists list this for Kryo: with default config, it will fail to deserialize the
				' synchronized lists, throwing an obscure null pointer exception
				Return New List(Of Persistable)(updates)
			End Get
		End Property
	End Class

End Namespace