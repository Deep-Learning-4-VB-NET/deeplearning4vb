Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SequenceRecordReaderFunction implements org.nd4j.common.function.@Function<org.nd4j.common.primitives.Pair<String, java.io.InputStream>, java.util.List<java.util.List<org.datavec.api.writable.Writable>>>
	Public Class SequenceRecordReaderFunction
		Implements [Function](Of Pair(Of String, Stream), IList(Of IList(Of Writable)))

		Protected Friend sequenceRecordReader As SequenceRecordReader

		Public Sub New(ByVal sequenceRecordReader As SequenceRecordReader)
			Me.sequenceRecordReader = sequenceRecordReader
		End Sub

		Public Overridable Function apply(ByVal value As Pair(Of String, Stream)) As IList(Of IList(Of Writable))
			Dim uri As URI = URI.create(value.First)
			Try
					Using dis As DataInputStream = CType(value.Right, DataInputStream)
					Return sequenceRecordReader.sequenceRecord(uri, dis)
					End Using
			Catch e As IOException
				log.error("",e)
			End Try

			Throw New System.InvalidOperationException("Something went wrong")
		End Function
	End Class

End Namespace