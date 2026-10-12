Imports System.Collections.Generic
Imports System.IO
Imports RecordReader = org.datavec.api.records.reader.RecordReader
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

Namespace org.datavec.local.transforms.functions



	Public Class RecordReaderFunction
		Implements [Function](Of Pair(Of String, Stream), IList(Of Writable))

		Protected Friend recordReader As RecordReader

		Public Sub New(ByVal recordReader As RecordReader)
			Me.recordReader = recordReader
		End Sub

		Public Overridable Function apply(ByVal value As Pair(Of String, Stream)) As IList(Of Writable)
			Dim uri As URI = URI.create(value.First)
			Dim ds As Stream = value.Right
			Try
					Using dis As DataInputStream = CType(ds, DataInputStream)
					Return recordReader.record(uri, dis)
					End Using
			Catch e As IOException
				Throw New System.InvalidOperationException("Something went wrong reading file")
			End Try

		End Function
	End Class

End Namespace