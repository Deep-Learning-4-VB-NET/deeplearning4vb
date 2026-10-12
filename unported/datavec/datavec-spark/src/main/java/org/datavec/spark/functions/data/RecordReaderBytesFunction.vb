Imports System.Collections.Generic
Imports System.IO
Imports BytesWritable = org.apache.hadoop.io.BytesWritable
Imports Text = org.apache.hadoop.io.Text
Imports [Function] = org.apache.spark.api.java.function.Function
Imports RecordReader = org.datavec.api.records.reader.RecordReader
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

Namespace org.datavec.spark.functions.data


	Public Class RecordReaderBytesFunction
		Implements [Function](Of Tuple2(Of Text, BytesWritable), IList(Of Writable))

		Private ReadOnly recordReader As RecordReader

		Public Sub New(ByVal recordReader As RecordReader)
			Me.recordReader = recordReader
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> call(scala.Tuple2<org.apache.hadoop.io.Text, org.apache.hadoop.io.BytesWritable> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As Tuple2(Of Text, BytesWritable)) As IList(Of Writable)
			Dim uri As New URI(v1._1().ToString())
			Dim dis As New DataInputStream(New MemoryStream(v1._2().getBytes()))
			Return recordReader.record(uri, dis)
		End Function


	End Class

End Namespace