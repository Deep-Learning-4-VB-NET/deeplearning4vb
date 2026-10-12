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

Namespace org.deeplearning4j.parallelism.inference.observers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BasicInferenceObserver implements java.util.Observer
	Public Class BasicInferenceObserver
		Implements Observer

		Private finished As AtomicBoolean

		Public Sub New()
			finished = New AtomicBoolean(False)
		End Sub

		Public Overrides Sub update(ByVal o As Observable, ByVal arg As Object)
			finished.set(True)
		End Sub

		''' <summary>
		''' FOR DEBUGGING ONLY, TO BE REMOVED BEFORE MERGE
		''' </summary>
		Public Overridable Sub waitTillDone()
			Do While Not finished.get()
				LockSupport.parkNanos(1000)
			Loop
		End Sub
	End Class

End Namespace