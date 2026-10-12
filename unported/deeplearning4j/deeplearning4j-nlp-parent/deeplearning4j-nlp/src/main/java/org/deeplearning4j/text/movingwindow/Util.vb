Imports System
Imports System.Collections.Generic
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.text.movingwindow



	Public Class Util


		Private Sub New()
		End Sub

		''' <summary>
		''' Returns a thread safe counter map
		''' @return
		''' </summary>
		Public Shared Function parallelCounterMap(Of K, V)() As CounterMap(Of K, V)
			Dim totalWords As New CounterMap(Of K, V)()
			Return totalWords
		End Function


		''' <summary>
		''' Returns a thread safe counter
		''' @return
		''' </summary>
		Public Shared Function parallelCounter(Of K)() As Counter(Of K)
			Dim totalWords As New Counter(Of K)()
			Return totalWords
		End Function



		Public Shared Function matchesAnyStopWord(ByVal stopWords As IList(Of String), ByVal word As String) As Boolean
			For Each s As String In stopWords
				If s.Equals(word, StringComparison.OrdinalIgnoreCase) Then
					Return True
				End If
			Next s
			Return False
		End Function

		Public Shared Function disableLogging() As Level
			Dim logger As Logger = Logger.getLogger("org.apache.uima")
			Do While logger.getLevel() Is Nothing
				logger = logger.getParent()
			Loop
			Dim level As Level = logger.getLevel()
			logger.setLevel(Level.OFF)
			Return level
		End Function


	End Class

End Namespace