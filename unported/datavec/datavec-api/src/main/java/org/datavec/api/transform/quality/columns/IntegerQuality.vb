Imports System
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class IntegerQuality extends ColumnQuality
	<Serializable>
	Public Class IntegerQuality
		Inherits ColumnQuality

		Private ReadOnly countNonInteger As Long

		Public Sub New(ByVal countValid As Long, ByVal countInvalid As Long, ByVal countMissing As Long, ByVal countTotal As Long, ByVal countNonInteger As Long)
			MyBase.New(countValid, countInvalid, countMissing, countTotal)
			Me.countNonInteger = countNonInteger
		End Sub


		Public Overridable Function add(ByVal other As IntegerQuality) As IntegerQuality
			Return New IntegerQuality(countValid + other.countValid, countInvalid + other.countInvalid, countMissing + other.countMissing, countTotal + other.countTotal, countNonInteger + other.countNonInteger)
		End Function

		Public Overrides Function ToString() As String
			Return "IntegerQuality(" & MyBase.ToString() & ", countNonInteger=" & countNonInteger & ")"
		End Function

	End Class

End Namespace