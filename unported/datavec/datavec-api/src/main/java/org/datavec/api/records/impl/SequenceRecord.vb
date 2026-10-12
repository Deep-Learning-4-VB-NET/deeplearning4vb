Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
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

Namespace org.datavec.api.records.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class SequenceRecord implements org.datavec.api.records.SequenceRecord
	Public Class SequenceRecord
		Implements org.datavec.api.records.SequenceRecord

		Private sequenceRecord As IList(Of IList(Of Writable))
		Private metaData As RecordMetaData

		Public Overridable ReadOnly Property SequenceLength As Integer Implements org.datavec.api.records.SequenceRecord.getSequenceLength
			Get
				If sequenceRecord Is Nothing Then
					Return 0
				End If
				Return sequenceRecord.Count
			End Get
		End Property

		Public Overridable Function getTimeStep(ByVal timeStep As Integer) As IList(Of Writable) Implements org.datavec.api.records.SequenceRecord.getTimeStep
			If timeStep < 0 OrElse timeStep > sequenceRecord.Count Then
				Throw New System.ArgumentException("Invalid input: " & sequenceRecord.Count & " time steps available; cannot get " & timeStep)
			End If
			Return sequenceRecord(timeStep)
		End Function
	End Class

End Namespace