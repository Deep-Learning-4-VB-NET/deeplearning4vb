﻿Imports System.Collections.Generic
Imports System.IO
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports BytesWritable = org.datavec.api.writable.BytesWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function
Imports org.nd4j.common.primitives

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

Namespace org.datavec.local.transforms.functions.data



	Public Class SequenceRecordReaderBytesFunction
		Implements [Function](Of Pair(Of Text, BytesWritable), IList(Of IList(Of Writable)))

		Private ReadOnly recordReader As SequenceRecordReader

		Public Sub New(ByVal recordReader As SequenceRecordReader)
			Me.recordReader = recordReader
		End Sub

		Public Overridable Function apply(ByVal v1 As Pair(Of Text, BytesWritable)) As IList(Of IList(Of Writable))
			Dim uri As URI = URI.create(v1.First.ToString())
			Dim dis As New DataInputStream(New MemoryStream(v1.Right.getContent()))
			Try
				Return recordReader.sequenceRecord(uri, dis)
			Catch e As IOException
				Throw New System.InvalidOperationException(e)
			End Try

		End Function
	End Class

End Namespace