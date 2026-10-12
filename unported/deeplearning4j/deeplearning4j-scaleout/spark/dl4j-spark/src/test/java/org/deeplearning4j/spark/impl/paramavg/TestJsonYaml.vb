Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports org.deeplearning4j.spark.api
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.spark.impl.paramavg

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Tag(TagNames.JACKSON_SERDE) public class TestJsonYaml
	Public Class TestJsonYaml
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJsonYaml()
		Public Overridable Sub testJsonYaml()
			Dim tm As TrainingMaster = (New ParameterAveragingTrainingMaster.Builder(2)).batchSizePerWorker(32).exportDirectory("hdfs://SomeDirectory/").saveUpdater(False).averagingFrequency(3).storageLevel(StorageLevel.MEMORY_ONLY_SER_2()).storageLevelStreams(StorageLevel.DISK_ONLY()).build()

			Dim json As String = tm.toJson()
			Dim yaml As String = tm.toYaml()

	'        System.out.println(json);

			Dim fromJson As TrainingMaster = ParameterAveragingTrainingMaster.fromJson(json)
			Dim fromYaml As TrainingMaster = ParameterAveragingTrainingMaster.fromYaml(yaml)


			assertEquals(tm, fromJson)
			assertEquals(tm, fromYaml)

		End Sub

	End Class

End Namespace