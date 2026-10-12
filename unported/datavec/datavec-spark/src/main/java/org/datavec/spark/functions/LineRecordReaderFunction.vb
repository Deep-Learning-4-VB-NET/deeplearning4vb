Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports StringSplit = org.datavec.api.split.StringSplit
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

Namespace org.datavec.spark.functions


	Public Class LineRecordReaderFunction
		Implements [Function](Of String, IList(Of Writable))

		Private ReadOnly recordReader As RecordReader

		Public Sub New(ByVal recordReader As RecordReader)
			Me.recordReader = recordReader
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> call(String s) throws Exception
		Public Overrides Function [call](ByVal s As String) As IList(Of Writable)
			recordReader.initialize(New org.datavec.api.Split.StringSplit(s))
			Return recordReader.next()
		End Function
	End Class

End Namespace