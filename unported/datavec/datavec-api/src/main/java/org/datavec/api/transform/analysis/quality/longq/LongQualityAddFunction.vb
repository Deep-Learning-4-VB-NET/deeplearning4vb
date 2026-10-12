Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports LongMetaData = org.datavec.api.transform.metadata.LongMetaData
Imports LongQuality = org.datavec.api.transform.quality.columns.LongQuality
Imports NullWritable = org.datavec.api.writable.NullWritable
Imports Text = org.datavec.api.writable.Text
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

Namespace org.datavec.api.transform.analysis.quality.longq


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class LongQualityAddFunction implements org.nd4j.common.function.BiFunction<org.datavec.api.transform.quality.columns.LongQuality, org.datavec.api.writable.Writable, org.datavec.api.transform.quality.columns.LongQuality>, java.io.Serializable
	<Serializable>
	Public Class LongQualityAddFunction
		Implements BiFunction(Of LongQuality, Writable, LongQuality)

		Private ReadOnly meta As LongMetaData

		Public Overridable Function apply(ByVal v1 As LongQuality, ByVal writable As Writable) As LongQuality

			Dim valid As Long = v1.getCountValid()
			Dim invalid As Long = v1.getCountInvalid()
			Dim countMissing As Long = v1.getCountMissing()
			Dim countTotal As Long = v1.getCountTotal() + 1
			Dim nonLong As Long = v1.getCountNonLong()

			If meta.isValid(writable) Then
				valid += 1
			ElseIf TypeOf writable Is NullWritable OrElse TypeOf writable Is Text AndAlso (String.ReferenceEquals(writable.ToString(), Nothing) OrElse writable.ToString().Length = 0) Then
				countMissing += 1
			Else
				invalid += 1
			End If

			Dim str As String = writable.ToString()
			Try
				Long.Parse(str)
			Catch e As System.FormatException
				nonLong += 1
			End Try

			Return New LongQuality(valid, invalid, countMissing, countTotal, nonLong)
		End Function
	End Class

End Namespace