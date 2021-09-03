Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports AllArgsConstructor = lombok.AllArgsConstructor
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

Namespace org.datavec.local.transforms.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class StringToWritablesFunction implements org.nd4j.common.function.@Function<String, java.util.List<org.datavec.api.writable.Writable>>
	Public Class StringToWritablesFunction
		Implements [Function](Of String, IList(Of Writable))

		Private recordReader As RecordReader

		Public Overridable Function apply(ByVal s As String) As IList(Of Writable)
			Try
				recordReader.initialize(New org.datavec.api.Split.StringSplit(s))
			Catch e As IOException
			   Throw New Exception(e)
			Catch e As InterruptedException
				Thread.CurrentThread.Interrupt()
				Throw New Exception(e)
			End Try
			Dim [next] As ICollection(Of Writable) = recordReader.next()
			If TypeOf [next] Is System.Collections.IList Then
				Return CType([next], IList(Of Writable))
			End If
			Return New List(Of Writable)([next])
		End Function
	End Class

End Namespace