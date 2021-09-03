Imports System.Collections.Generic
Imports Configurable = org.datavec.api.conf.Configurable
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
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




	Public Interface SequenceRecordWriter
		Inherits Closeable, Configurable

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String APPEND = "org.datavec.api.record.writer.append";

		''' <summary>
		''' Write a record </summary>
		''' <param name="sequence"> the record to write </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.datavec.api.split.partition.PartitionMetaData write(java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequence) throws java.io.IOException;
		Function write(ByVal sequence As IList(Of IList(Of Writable))) As org.datavec.api.Split.partition.PartitionMetaData


		''' <summary>
		''' Close the sequence record writer
		''' </summary>
		Sub close()

	End Interface

End Namespace