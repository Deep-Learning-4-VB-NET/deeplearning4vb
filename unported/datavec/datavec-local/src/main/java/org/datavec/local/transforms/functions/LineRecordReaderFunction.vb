Imports System
Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports StringSplit = org.datavec.api.split.StringSplit
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function

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


	Public Class LineRecordReaderFunction
		Implements [Function](Of String, IList(Of Writable))

		Private ReadOnly recordReader As RecordReader

		Public Sub New(ByVal recordReader As RecordReader)
			Me.recordReader = recordReader
		End Sub

		Public Overridable Function apply(ByVal s As String) As IList(Of Writable)
			Try
				recordReader.initialize(New org.datavec.api.Split.StringSplit(s))
			Catch e As Exception
				Throw New System.InvalidOperationException(e)
			End Try
			Return recordReader.next()
		End Function
	End Class

End Namespace