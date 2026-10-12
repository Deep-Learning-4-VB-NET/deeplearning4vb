Imports System.Collections.Generic
Imports System.IO
Imports Text = org.apache.hadoop.io.Text
Imports [Function] = org.apache.spark.api.java.function.Function
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

Namespace org.datavec.spark.functions.pairdata


	Public Class PairSequenceRecordReaderBytesFunction
		Implements [Function](Of Tuple2(Of Text, BytesPairWritable), Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable))))

		Private ReadOnly recordReaderFirst As SequenceRecordReader
		Private ReadOnly recordReaderSecond As SequenceRecordReader

		Public Sub New(ByVal recordReaderFirst As SequenceRecordReader, ByVal recordReaderSecond As SequenceRecordReader)
			Me.recordReaderFirst = recordReaderFirst
			Me.recordReaderSecond = recordReaderSecond
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<java.util.List<java.util.List<org.datavec.api.writable.Writable>>, java.util.List<java.util.List<org.datavec.api.writable.Writable>>> call(scala.Tuple2<org.apache.hadoop.io.Text, BytesPairWritable> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As Tuple2(Of Text, BytesPairWritable)) As Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))
			Dim bpw As BytesPairWritable = v1._2()
			Dim dis1 As New DataInputStream(New MemoryStream(bpw.First))
			Dim dis2 As New DataInputStream(New MemoryStream(bpw.Second))
			Dim u1 As URI = (If(bpw.UriFirst IsNot Nothing, New URI(bpw.UriFirst), Nothing))
			Dim u2 As URI = (If(bpw.UriSecond IsNot Nothing, New URI(bpw.UriSecond), Nothing))
			Dim first As IList(Of IList(Of Writable)) = recordReaderFirst.sequenceRecord(u1, dis1)
			Dim second As IList(Of IList(Of Writable)) = recordReaderSecond.sequenceRecord(u2, dis2)
			Return New Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))(first, second)
		End Function
	End Class

End Namespace