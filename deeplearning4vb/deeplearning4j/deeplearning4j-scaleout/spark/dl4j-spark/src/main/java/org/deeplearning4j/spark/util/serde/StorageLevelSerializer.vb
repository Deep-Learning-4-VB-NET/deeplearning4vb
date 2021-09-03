Imports System.Collections.Generic
Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports JsonSerializer = org.nd4j.shade.jackson.databind.JsonSerializer
Imports SerializerProvider = org.nd4j.shade.jackson.databind.SerializerProvider

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

Namespace org.deeplearning4j.spark.util.serde


	Public Class StorageLevelSerializer
		Inherits JsonSerializer(Of StorageLevel)

		Private Shared ReadOnly map As IDictionary(Of StorageLevel, String) = initMap()

		Private Shared Function initMap() As IDictionary(Of StorageLevel, String)
			Dim map As IDictionary(Of StorageLevel, String) = New Dictionary(Of StorageLevel, String)()
			map(StorageLevel.NONE()) = "NONE"
			map(StorageLevel.DISK_ONLY()) = "DISK_ONLY"
			map(StorageLevel.DISK_ONLY_2()) = "DISK_ONLY_2"
			map(StorageLevel.MEMORY_ONLY()) = "MEMORY_ONLY"
			map(StorageLevel.MEMORY_ONLY_2()) = "MEMORY_ONLY_2"
			map(StorageLevel.MEMORY_ONLY_SER()) = "MEMORY_ONLY_SER"
			map(StorageLevel.MEMORY_ONLY_SER_2()) = "MEMORY_ONLY_SER_2"
			map(StorageLevel.MEMORY_AND_DISK()) = "MEMORY_AND_DISK"
			map(StorageLevel.MEMORY_AND_DISK_2()) = "MEMORY_AND_DISK_2"
			map(StorageLevel.MEMORY_AND_DISK_SER()) = "MEMORY_AND_DISK_SER"
			map(StorageLevel.MEMORY_AND_DISK_SER_2()) = "MEMORY_AND_DISK_SER_2"
			map(StorageLevel.OFF_HEAP()) = "OFF_HEAP"
			Return map
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.apache.spark.storage.StorageLevel storageLevel, org.nd4j.shade.jackson.core.JsonGenerator jsonGenerator, org.nd4j.shade.jackson.databind.SerializerProvider serializerProvider) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Sub serialize(ByVal storageLevel As StorageLevel, ByVal jsonGenerator As JsonGenerator, ByVal serializerProvider As SerializerProvider)
			'This is a little ugly, but Spark doesn't provide many options here...
			Dim s As String = Nothing
			If storageLevel IsNot Nothing Then
				s = map(storageLevel)
			End If
			jsonGenerator.writeString(s)
		End Sub
	End Class

End Namespace