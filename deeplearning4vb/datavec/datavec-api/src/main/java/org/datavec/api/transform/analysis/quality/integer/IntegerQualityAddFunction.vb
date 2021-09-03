Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports IntegerQuality = org.datavec.api.transform.quality.columns.IntegerQuality
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

Namespace org.datavec.api.transform.analysis.quality.integer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class IntegerQualityAddFunction implements org.nd4j.common.function.BiFunction<org.datavec.api.transform.quality.columns.IntegerQuality, org.datavec.api.writable.Writable, org.datavec.api.transform.quality.columns.IntegerQuality>, java.io.Serializable
	<Serializable>
	Public Class IntegerQualityAddFunction
		Implements BiFunction(Of IntegerQuality, Writable, IntegerQuality)

		Private ReadOnly meta As IntegerMetaData

		Public Overridable Function apply(ByVal v1 As IntegerQuality, ByVal writable As Writable) As IntegerQuality

			Dim valid As Long = v1.getCountValid()
			Dim invalid As Long = v1.getCountInvalid()
			Dim countMissing As Long = v1.getCountMissing()
			Dim countTotal As Long = v1.getCountTotal() + 1
			Dim nonInteger As Long = v1.getCountNonInteger()

			If meta.isValid(writable) Then
				valid += 1
			ElseIf TypeOf writable Is NullWritable OrElse TypeOf writable Is Text AndAlso (String.ReferenceEquals(writable.ToString(), Nothing) OrElse writable.ToString().Length = 0) Then
				countMissing += 1
			Else
				invalid += 1
			End If

			Dim str As String = writable.ToString()
			Try
				Integer.Parse(str)
			Catch e As System.FormatException
				nonInteger += 1
			End Try

			Return New IntegerQuality(valid, invalid, countMissing, countTotal, nonInteger)
		End Function
	End Class

End Namespace