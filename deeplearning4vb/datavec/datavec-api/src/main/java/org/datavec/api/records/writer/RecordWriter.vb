Imports System.Collections.Generic
Imports Configurable = org.datavec.api.conf.Configurable
Imports Configuration = org.datavec.api.conf.Configuration
Imports InputSplit = org.datavec.api.split.InputSplit
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
Imports Partitioner = org.datavec.api.split.partition.Partitioner
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.records.writer



	Public Interface RecordWriter
		Inherits Closeable, Configurable

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String APPEND = "org.datavec.api.record.writer.append";


		''' <summary>
		''' Returns true if this record writer
		''' supports efficient batch writing using <seealso cref="writeBatch(List)"/>
		''' @return
		''' </summary>
		Function supportsBatch() As Boolean
		''' <summary>
		''' Initialize a record writer with the given input split </summary>
		''' <param name="inputSplit"> the input split to initialize with </param>
		''' <param name="partitioner"> </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void initialize(org.datavec.api.split.InputSplit inputSplit, org.datavec.api.split.partition.Partitioner partitioner) throws Exception;
		Sub initialize(ByVal inputSplit As org.datavec.api.Split.InputSplit, ByVal partitioner As org.datavec.api.Split.partition.Partitioner)

		''' <summary>
		''' Initialize the record reader with the given configuration
		''' and <seealso cref="InputSplit"/> </summary>
		''' <param name="configuration"> the configuration to iniailize with </param>
		''' <param name="split"> the split to use </param>
		''' <param name="partitioner"> </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void initialize(org.datavec.api.conf.Configuration configuration, org.datavec.api.split.InputSplit split, org.datavec.api.split.partition.Partitioner partitioner) throws Exception;
		Sub initialize(ByVal configuration As Configuration, ByVal split As org.datavec.api.Split.InputSplit, ByVal partitioner As org.datavec.api.Split.partition.Partitioner)

		''' <summary>
		''' Write a record </summary>
		''' <param name="record"> the record to write </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.datavec.api.split.partition.PartitionMetaData write(java.util.List<org.datavec.api.writable.Writable> record) throws java.io.IOException;
		Function write(ByVal record As IList(Of Writable)) As org.datavec.api.Split.partition.PartitionMetaData


		''' <summary>
		''' Write a batch of records </summary>
		''' <param name="batch"> the batch to write </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.datavec.api.split.partition.PartitionMetaData writeBatch(java.util.List<java.util.List<org.datavec.api.writable.Writable>> batch) throws java.io.IOException;
		Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As org.datavec.api.Split.partition.PartitionMetaData


		''' <summary>
		''' Close the recod reader
		''' </summary>
		Sub close()

	End Interface

End Namespace