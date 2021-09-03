Imports System.Collections.Generic
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
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

Namespace org.datavec.api.records


	Public Interface SequenceRecord

		''' <summary>
		''' Get the sequence record values
		''' </summary>
		''' <returns> Sequence record values </returns>
		Property SequenceRecord As IList(Of IList(Of Writable))

		''' <summary>
		''' Get the overall length of the sequence record (number of time/sequence steps, etc).
		''' Equivalent to {@code getSequenceRecord().size()}
		''' </summary>
		''' <returns> Length of sequence record </returns>
		ReadOnly Property SequenceLength As Integer

		''' <summary>
		''' Get a single time step. Equivalent to {@code getSequenceRecord().get(timeStep)}
		''' </summary>
		''' <param name="timeStep"> Time step to get. Must be {@code 0 <= timeStep < getSequenceLength()} </param>
		''' <returns> Values for a single time step </returns>
		Function getTimeStep(ByVal timeStep As Integer) As IList(Of Writable)


		''' <summary>
		''' Get the RecordMetaData for this record
		''' </summary>
		''' <returns> Metadata for this record (or null, if none has been set) </returns>
		Property MetaData As RecordMetaData


	End Interface

End Namespace