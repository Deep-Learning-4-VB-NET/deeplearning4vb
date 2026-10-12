Imports System.Collections.Generic
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports FloatWritable = org.datavec.api.writable.FloatWritable
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

Namespace org.datavec.api.util


	Public Class RecordUtils

		Public Shared Function toRecord(ByVal record() As Double) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)(record.Length)
			For i As Integer = 0 To record.Length - 1
				ret.Add(New DoubleWritable(record(i)))
			Next i

			Return ret
		End Function


		Public Shared Function toRecord(ByVal record() As Single) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)(record.Length)
			For i As Integer = 0 To record.Length - 1
				ret.Add(New FloatWritable(record(i)))
			Next i

			Return ret
		End Function

	End Class

End Namespace