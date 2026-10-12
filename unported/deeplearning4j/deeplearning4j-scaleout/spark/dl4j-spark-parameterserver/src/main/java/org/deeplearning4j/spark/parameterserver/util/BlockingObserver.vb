Imports System.Threading
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j

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

Namespace org.deeplearning4j.spark.parameterserver.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class BlockingObserver implements java.util.Observer
	Public Class BlockingObserver
		Implements Observer

		Protected Friend state As New AtomicBoolean(False)
		Protected Friend exception As AtomicBoolean

		Public Sub New(ByVal exception As AtomicBoolean)
			Me.exception = exception
		End Sub

		Public Overrides Sub update(ByVal o As Observable, ByVal arg As Object)
			state.set(True)
			'notify();
		End Sub

		''' <summary>
		''' This method blocks until state is set to True
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void waitTillDone() throws InterruptedException
		Public Overridable Sub waitTillDone()
			Do While Not exception.get() AndAlso Not state.get()
				'LockSupport.parkNanos(1000L);
				' we don't really need uber precision here, sleep is ok
				Thread.Sleep(5)
			Loop
		End Sub
	End Class

End Namespace