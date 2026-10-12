Imports System
Imports System.Threading

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

Namespace org.deeplearning4j.core.util


	Public Class ThreadUtils
		Public Shared Sub uncheckedSleep(ByVal millis As Long)
			LockSupport.parkNanos(millis * 1000000)
			' we must check the interrupted status in case this is used in a loop
			' Otherwise we may end up spinning 100% without breaking out on an interruption
			If Thread.CurrentThread.isInterrupted() Then
				Throw New UncheckedInterruptedException()
			End If
		End Sub

		Public Shared Sub uncheckedSleepNanos(ByVal nanos As Long)
			LockSupport.parkNanos(nanos)
			' we must check the interrupted status in case this is used in a loop
			' Otherwise we may end up spinning 100% without breaking out on an interruption
			If Thread.CurrentThread.isInterrupted() Then
				Throw New UncheckedInterruptedException()
			End If
		End Sub

		''' <summary>
		''' Similar to <seealso cref="InterruptedException"/> in concept, but unchecked.  Allowing this to be thrown without being 
		''' explicitly declared in the API.
		''' </summary>
		Public Class UncheckedInterruptedException
			Inherits Exception

		End Class
	End Class

End Namespace