Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.indexing.functions


	''' <summary>
	''' Returns a stable number based on infinity
	''' or nan
	''' </summary>
	Public Class StableNumber
		Implements [Function](Of Number, Number)

		Private type As Type

		Public Enum Type
			[DOUBLE]
			FLOAT
		End Enum

		Public Sub New(ByVal type As Type)
			Me.type = type
		End Sub

		Public Overridable Function apply(ByVal number As Number) As Number
			Select Case type
				Case org.nd4j.linalg.indexing.functions.StableNumber.Type.DOUBLE
					If Double.IsInfinity(number.doubleValue()) Then
						Return -Double.MaxValue
					End If
					If Double.IsNaN(number.doubleValue()) Then
						Return Nd4j.EPS_THRESHOLD
					End If
				Case org.nd4j.linalg.indexing.functions.StableNumber.Type.FLOAT
					If Single.IsInfinity(number.floatValue()) Then
						Return -Single.MaxValue
					End If
					If Single.IsNaN(number.floatValue()) Then
						Return Nd4j.EPS_THRESHOLD
					End If
				Case Else
					Throw New System.InvalidOperationException("Illegal opType")

			End Select

		End Function
	End Class

End Namespace