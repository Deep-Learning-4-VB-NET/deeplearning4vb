Imports System.IO
Imports Tag = org.junit.jupiter.api.Tag
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Files = org.nd4j.shade.guava.io.Files
Imports Configuration = org.datavec.api.conf.Configuration
Imports FileSplit = org.datavec.api.split.FileSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
Imports Partitioner = org.datavec.api.split.partition.Partitioner
Imports Test = org.junit.jupiter.api.Test
Imports org.junit.jupiter.api.Assertions

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

Namespace org.datavec.api.split.parittion


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class PartitionerTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class PartitionerTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRecordsPerFilePartition()
		Public Overridable Sub testRecordsPerFilePartition()
			Dim partitioner As org.datavec.api.Split.partition.Partitioner = New org.datavec.api.Split.partition.NumberOfRecordsPartitioner()
			Dim tmpDir As File = Files.createTempDir()
			Dim fileSplit As New org.datavec.api.Split.FileSplit(tmpDir)
			assertTrue(fileSplit.needsBootstrapForWrite())
			fileSplit.bootStrapForWrite()
			partitioner.init(fileSplit)
			assertEquals(1,partitioner.numPartitions())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputAddFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInputAddFile()
			Dim partitioner As org.datavec.api.Split.partition.Partitioner = New org.datavec.api.Split.partition.NumberOfRecordsPartitioner()
			Dim tmpDir As File = Files.createTempDir()
			Dim fileSplit As New org.datavec.api.Split.FileSplit(tmpDir)
			assertTrue(fileSplit.needsBootstrapForWrite())
			fileSplit.bootStrapForWrite()
			Dim configuration As New Configuration()
			configuration.set(org.datavec.api.Split.partition.NumberOfRecordsPartitioner.RECORDS_PER_FILE_CONFIG,5.ToString())
			partitioner.init(configuration,fileSplit)
			partitioner.updatePartitionInfo(org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(5).build())
			assertTrue(partitioner.needsNewPartition())
			Dim os As Stream = partitioner.openNewStream()
			os.Close()
			assertNotNull(os)
			'run more than once to ensure output stream creation works properly
			partitioner.updatePartitionInfo(org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(5).build())
			os = partitioner.openNewStream()
			os.Close()
			assertNotNull(os)


		End Sub

	End Class

End Namespace