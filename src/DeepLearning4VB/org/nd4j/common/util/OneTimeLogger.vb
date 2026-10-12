Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Logger = org.slf4j.Logger

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

Namespace org.nd4j.common.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class OneTimeLogger
	Public Class OneTimeLogger
		Protected Friend Shared hashSet As New HashSet(Of String)()
		Protected Friend Shared ReadOnly buffer As LinkedList(Of String) = New LinkedTransferQueue(Of String)()

		Private Shared ReadOnly lock As New ReentrantReadWriteLock()

		Protected Friend Shared Function isEligible(ByVal message As String) As Boolean

			Try
				lock.readLock().lock()

				If hashSet.Contains(message) Then
					Return False
				End If

			Finally
				lock.readLock().unlock()
			End Try

			Try
				lock.writeLock().lock()

				If buffer.Count >= 100 Then
					Dim [rem] As String = buffer.RemoveFirst()
					hashSet.Remove([rem])
				End If

				buffer.AddLast(message)
				hashSet.Add(message)

				Return True
			Finally
				lock.writeLock().unlock()
			End Try
		End Function

		Public Shared Sub info(ByVal logger As Logger, ByVal format As String, ParamArray ByVal arguments() As Object)
			If Not isEligible(format) Then
				Return
			End If

			logger.info(format, arguments)
		End Sub

		Public Shared Sub warn(ByVal logger As Logger, ByVal format As String, ParamArray ByVal arguments() As Object)
			If Not isEligible(format) Then
				Return
			End If

			logger.warn(format, arguments)
		End Sub

		Public Shared Sub [error](ByVal logger As Logger, ByVal format As String, ParamArray ByVal arguments() As Object)
			If Not isEligible(format) Then
				Return
			End If

			logger.error(format, arguments)
		End Sub

		Public Shared Sub reset()
			buffer.Clear()
		End Sub
	End Class

End Namespace