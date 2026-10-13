Imports System
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.conf

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

Namespace org.datavec.api.split.partition

    ''' <summary>Decides when a record writer has to move on to the next output location (partition).</summary>
    Public Interface Partitioner

        ''' <summary>Returns the total records written</summary>
        Function totalRecordsWritten() As Integer

        ''' <summary>Number of records written so far</summary>
        Function numRecordsWritten() As Integer

        ''' <summary>Returns the number of partitions</summary>
        Function numPartitions() As Integer

        ''' <summary>Initializes this partitioner with the given input split</summary>
        ''' <param name="inputSplit"> the input split to use with this partitioner </param>
        Sub init(ByVal inputSplit As InputSplit)

        ''' <summary>Initializes this partitioner with the given configuration and input split</summary>
        ''' <param name="configuration"> the configuration to configure this partitioner with </param>
        ''' <param name="split"> the input split to use with this partitioner </param>
        Sub init(ByVal configuration As Configuration, ByVal split As InputSplit)

        ''' <summary>Updates the metadata for this partitioner (to indicate whether the next partition is needed or not)</summary>
        Sub updatePartitionInfo(ByVal metadata As PartitionMetaData)

        ''' <summary>
        ''' Returns true if the partition needs to be moved to the next.
        ''' This is controlled with <see cref="updatePartitionInfo"/>
        ''' which handles incrementing counters and the like
        ''' to determine whether the current partition has been exhausted.
        ''' </summary>
        Function needsNewPartition() As Boolean

        ''' <summary>"Increment" to the next stream</summary>
        ''' <returns> the new opened output stream </returns>
        Function openNewStream() As Stream

        ''' <summary>Get the current output stream</summary>
        Function currentOutputStream() As Stream

    End Interface

End Namespace
