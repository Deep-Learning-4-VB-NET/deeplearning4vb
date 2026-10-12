Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
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

Namespace org.datavec.spark.transform.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class StringToWritablesFunction implements org.apache.spark.api.java.function.@Function<String, java.util.List<org.datavec.api.writable.Writable>>
	Public Class StringToWritablesFunction
		Implements [Function](Of String, IList(Of Writable))

		Private recordReader As RecordReader

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> call(String s) throws Exception
		Public Overrides Function [call](ByVal s As String) As IList(Of Writable)
			recordReader.initialize(New org.datavec.api.Split.StringSplit(s))
			Dim [next] As ICollection(Of Writable) = recordReader.next()
			If TypeOf [next] Is System.Collections.IList Then
				Return CType([next], IList(Of Writable))
			End If
			Return New List(Of Writable)([next])
		End Function
	End Class

End Namespace