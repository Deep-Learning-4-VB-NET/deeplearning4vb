Imports MediaDriver = io.aeron.driver.MediaDriver
Imports ThreadingMode = io.aeron.driver.ThreadingMode
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports SigIntBarrier = org.agrona.concurrent.SigIntBarrier
import static System.setProperty
import static org.agrona.concurrent.UnsafeBuffer.DISABLE_BOUNDS_CHECKS_PROP_NAME

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

Namespace org.nd4j.aeron.ipc

	Public Class LowLatencyMediaDriver

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("checkstyle:UncommentedMain") public static void main(final String... args)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Shared Sub Main(ParamArray ByVal args() As String)
			MediaDriver.main(args)
			setProperty(DISABLE_BOUNDS_CHECKS_PROP_NAME, "true")
			setProperty("aeron.mtu.length", "16384")
			setProperty("aeron.socket.so_sndbuf", "2097152")
			setProperty("aeron.socket.so_rcvbuf", "2097152")
			setProperty("aeron.rcv.initial.window.length", "2097152")

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context ctx = new io.aeron.driver.MediaDriver.Context().threadingMode(io.aeron.driver.ThreadingMode.DEDICATED).dirDeleteOnStart(true).dirDeleteOnShutdown(true).termBufferSparseFile(false).conductorIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).receiverIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).senderIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy());
			Dim ctx As MediaDriver.Context = (New MediaDriver.Context()).threadingMode(ThreadingMode.DEDICATED).dirDeleteOnStart(True).dirDeleteOnShutdown(True).termBufferSparseFile(False).conductorIdleStrategy(New BusySpinIdleStrategy()).receiverIdleStrategy(New BusySpinIdleStrategy()).senderIdleStrategy(New BusySpinIdleStrategy())

			Using ignored As io.aeron.driver.MediaDriver = io.aeron.driver.MediaDriver.launch(ctx)
				Call (New SigIntBarrier()).await()

			End Using
		End Sub

	End Class

End Namespace