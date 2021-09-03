Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports Writable = org.datavec.api.writable.Writable
Imports Tuple2 = scala.Tuple2

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

Namespace org.datavec.spark.functions


	Public Class SequenceRecordReaderFunction
		Implements [Function](Of Tuple2(Of String, PortableDataStream), IList(Of IList(Of Writable)))

		Protected Friend sequenceRecordReader As SequenceRecordReader

		Public Sub New(ByVal sequenceRecordReader As SequenceRecordReader)
			Me.sequenceRecordReader = sequenceRecordReader
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> call(scala.Tuple2<String, org.apache.spark.input.PortableDataStream> value) throws Exception
		Public Overrides Function [call](ByVal value As Tuple2(Of String, PortableDataStream)) As IList(Of IList(Of Writable))
			Dim uri As New URI(value._1())
			Dim ds As PortableDataStream = value._2()
			Using dis As java.io.DataInputStream = ds.open()
				Return sequenceRecordReader.sequenceRecord(uri, dis)
			End Using
		End Function
	End Class

End Namespace