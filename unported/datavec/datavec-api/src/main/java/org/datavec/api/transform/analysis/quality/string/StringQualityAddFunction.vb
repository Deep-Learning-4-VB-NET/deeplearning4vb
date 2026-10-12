Imports System
Imports HyperLogLogPlus = com.clearspring.analytics.stream.cardinality.HyperLogLogPlus
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports StringQuality = org.datavec.api.transform.quality.columns.StringQuality
Imports NullWritable = org.datavec.api.writable.NullWritable
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

Namespace org.datavec.api.transform.analysis.quality.string


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class StringQualityAddFunction implements org.nd4j.common.function.BiFunction<org.datavec.api.transform.quality.columns.StringQuality, org.datavec.api.writable.Writable, org.datavec.api.transform.quality.columns.StringQuality>, java.io.Serializable
	<Serializable>
	Public Class StringQualityAddFunction
		Implements BiFunction(Of StringQuality, Writable, StringQuality)

		Private ReadOnly meta As StringMetaData

		Public Overridable Function apply(ByVal v1 As StringQuality, ByVal writable As Writable) As StringQuality
			Dim valid As Long = v1.getCountValid()
			Dim invalid As Long = v1.getCountInvalid()
			Dim countMissing As Long = v1.getCountMissing()
			Dim countTotal As Long = v1.getCountTotal() + 1
			Dim empty As Long = v1.getCountEmptyString()
			Dim alphabetic As Long = v1.getCountAlphabetic()
			Dim numerical As Long = v1.getCountNumerical()
			Dim word As Long = v1.getCountWordCharacter()
			Dim whitespaceOnly As Long = v1.getCountWhitespace()
			Dim hll As HyperLogLogPlus = v1.getHll()

			Dim str As String = writable.ToString()

			If TypeOf writable Is NullWritable Then
				countMissing += 1
			ElseIf meta.isValid(writable) Then
				valid += 1
			Else
				invalid += 1
			End If

			If str Is Nothing OrElse str.Length = 0 Then
				empty += 1
			Else
				If str.matches("[a-zA-Z]") Then
					alphabetic += 1
				End If
				If str.matches("\d+") Then
					numerical += 1
				End If
				If str.matches("\w+") Then
					word += 1
				End If
				If str.matches("\s+") Then
					whitespaceOnly += 1
				End If
			End If

			hll.offer(str)
			Return New StringQuality(valid, invalid, countMissing, countTotal, empty, alphabetic, numerical, word, whitespaceOnly, hll)
		End Function
	End Class

End Namespace