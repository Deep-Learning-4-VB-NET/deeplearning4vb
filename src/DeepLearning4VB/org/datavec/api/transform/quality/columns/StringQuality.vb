Imports System
Imports HyperLogLogPlus = com.clearspring.analytics.stream.cardinality.HyperLogLogPlus
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.datavec.api.transform.quality.columns

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class StringQuality extends ColumnQuality
	<Serializable>
	Public Class StringQuality
		Inherits ColumnQuality

		Private ReadOnly countEmptyString As Long '"" string
		Private ReadOnly countAlphabetic As Long 'A-Z, a-z only
		Private ReadOnly countNumerical As Long '0-9 only
		Private ReadOnly countWordCharacter As Long 'A-Z, a-z, 0-9
		Private ReadOnly countWhitespace As Long 'tab, spaces etc ONLY
		Private ReadOnly hll As HyperLogLogPlus

		Public Sub New()
			Me.New(0, 0, 0, 0, 0, 0, 0, 0, 0, 0.05)
		End Sub

		Public Sub New(ByVal countValid As Long, ByVal countInvalid As Long, ByVal countMissing As Long, ByVal countTotal As Long, ByVal countEmptyString As Long, ByVal countAlphabetic As Long, ByVal countNumerical As Long, ByVal countWordCharacter As Long, ByVal countWhitespace As Long, ByVal hll As HyperLogLogPlus)
			MyBase.New(countValid, countInvalid, countMissing, countTotal)
			Me.countEmptyString = countEmptyString
			Me.countAlphabetic = countAlphabetic
			Me.countNumerical = countNumerical
			Me.countWordCharacter = countWordCharacter
			Me.countWhitespace = countWhitespace
			Me.hll = hll
		End Sub

		Public Sub New(ByVal countValid As Long, ByVal countInvalid As Long, ByVal countMissing As Long, ByVal countTotal As Long, ByVal countEmptyString As Long, ByVal countAlphabetic As Long, ByVal countNumerical As Long, ByVal countWordCharacter As Long, ByVal countWhitespace As Long, ByVal relativeSD As Double)
	'        
	'         * The algorithm used is based on streamlib's implementation of "HyperLogLog in Practice:
	'         * Algorithmic Engineering of a State of The Art Cardinality Estimation Algorithm", available
	'         * <a href="http://dx.doi.org/10.1145/2452376.2452456">here</a>.
	'         *
	'         * The relative accuracy is approximately `1.054 / sqrt(2^p)`. Setting
	'         * a nonzero `sp > p` in HyperLogLogPlus(p, sp) would trigger sparse
	'         * representation of registers, which may reduce the memory consumption
	'         * and increase accuracy when the cardinality is small.
	'         
			Me.New(countValid, countInvalid, countMissing, countTotal, countEmptyString, countAlphabetic, countNumerical, countWordCharacter, countWhitespace, New HyperLogLogPlus(CInt(Math.Truncate(Math.Ceiling(2.0 * Math.Log(1.054 / relativeSD) / Math.Log(2)))), 0))
		End Sub

		Public Overridable Function add(ByVal other As StringQuality) As StringQuality
			Try
				hll.addAll(other.hll)
			Catch e As Exception
				Throw New Exception(e)
			End Try
			Return New StringQuality(countValid + other.countValid, countInvalid + other.countInvalid, countMissing + other.countMissing, countTotal + other.countTotal, countEmptyString + other.countEmptyString, countAlphabetic + other.countAlphabetic, countNumerical + other.countNumerical, countWordCharacter + other.countWordCharacter, countWhitespace + other.countWhitespace, hll)
		End Function

		Public Overrides Function ToString() As String
			Return "StringQuality(" & MyBase.ToString() & ", countEmptyString=" & countEmptyString & ", countAlphabetic=" & countAlphabetic & ", countNumerical=" & countNumerical & ", countWordCharacter=" & countWordCharacter & ", countWhitespace=" & countWhitespace & ", countApproxUnique=" & hll.cardinality() & ")"
		End Function

	End Class

End Namespace