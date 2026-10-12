Imports System
Imports System.Collections.Generic
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports TransformProcessSequenceRecordReader = org.datavec.api.records.reader.impl.transform.TransformProcessSequenceRecordReader
Imports TransformProcess = org.datavec.api.transform.TransformProcess
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

Namespace org.datavec.local.transforms


	<Serializable>
	Public Class LocalTransformProcessSequenceRecordReader
		Inherits TransformProcessSequenceRecordReader

		Public Sub New(ByVal sequenceRecordReader As SequenceRecordReader, ByVal transformProcess As TransformProcess)
			MyBase.New(sequenceRecordReader, transformProcess)
		End Sub

		Public Overrides Function sequenceRecord() As IList(Of IList(Of Writable))
			Return LocalTransformExecutor.executeSequenceToSequence(New List(Of IList(Of IList(Of Writable))) From {sequenceRecordReader.nextSequence().getSequenceRecord()}, transformProcess)(0)
		End Function

		Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Return MyBase.next(num)
		End Function

		Public Overrides Function [next]() As IList(Of Writable)
			Return MyBase.next()
		End Function
	End Class

End Namespace