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

Namespace org.nd4j.parameterserver.util


	Public Class CheckSocket

		''' <summary>
		''' Check if a remote port is taken </summary>
		''' <param name="node"> the host to check </param>
		''' <param name="port"> the port to check </param>
		''' <param name="timeout"> the timeout for the connection </param>
		''' <returns> true if the port is taken false otherwise </returns>
		Public Shared Function remotePortTaken(ByVal node As String, ByVal port As Integer, ByVal timeout As Integer) As Boolean
			Dim s As Socket = Nothing
			Try
				s = New Socket()
				s.setReuseAddress(True)
				Dim sa As SocketAddress = New InetSocketAddress(node, port)
				s.connect(sa, timeout * 1000)
			Catch e As IOException
				If e.Message.Equals("Connection refused") Then
					Return False
				End If
				If TypeOf e Is SocketTimeoutException OrElse TypeOf e Is UnknownHostException Then
					Throw e
				End If
			Finally
				If s IsNot Nothing Then
					If s.isConnected() Then
						Return True
					Else
					End If
					Try
						s.close()
					Catch e As IOException
					End Try
				End If

				Return False
			End Try

		End Function
	End Class

End Namespace