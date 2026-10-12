Imports System
Imports Aeron = io.aeron.Aeron
Imports FragmentHandler = io.aeron.logbuffer.FragmentHandler
Imports Header = io.aeron.logbuffer.Header
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DirectBuffer = org.agrona.DirectBuffer
Imports AeronNDArrayPublisher = org.nd4j.aeron.ipc.AeronNDArrayPublisher
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports NDArrayHolder = org.nd4j.aeron.ipc.NDArrayHolder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.aeron.ipc.response


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Slf4j public class NDArrayResponseFragmentHandler implements io.aeron.logbuffer.FragmentHandler
	Public Class NDArrayResponseFragmentHandler
		Implements FragmentHandler

		Private holder As NDArrayHolder
		Private context As Aeron.Context
		Private aeron As Aeron
		Private streamId As Integer

		''' <summary>
		''' Callback for handling fragments of data being read from a log.
		''' </summary>
		''' <param name="buffer"> containing the data. </param>
		''' <param name="offset"> at which the data begins. </param>
		''' <param name="length"> of the data in bytes. </param>
		''' <param name="header"> representing the meta data for the data. </param>
		Public Overrides Sub onFragment(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal length As Integer, ByVal header As Header)
			If buffer IsNot Nothing AndAlso length > 0 Then
				Dim byteBuffer As ByteBuffer = buffer.byteBuffer().order(ByteOrder.nativeOrder())
				byteBuffer.position(offset)
				Dim b(length - 1) As SByte
				byteBuffer.get(b)
				Dim hostPort As String = StringHelper.NewString(b)
				Console.WriteLine("Host port " & hostPort & " offset " & offset & " length " & length)
				Dim split() As String = hostPort.Split(":", True)
				If split Is Nothing OrElse split.Length <> 3 Then
					Console.Error.WriteLine("no host port stream found")
					Return
				End If

				Dim port As Integer = Integer.Parse(split(1))
				Dim streamToPublish As Integer = Integer.Parse(split(2))
				Dim channel As String = AeronUtil.aeronChannel(split(0), port)
				Dim arrGet As INDArray = holder.get()
				Dim publisher As AeronNDArrayPublisher = AeronNDArrayPublisher.builder().streamId(streamToPublish).aeron(aeron).channel(channel).build()
				Try
					publisher.publish(arrGet)
				Catch e As Exception
					log.error("",e)
				End Try

				Try
					publisher.close()
				Catch e As Exception
					log.error("",e)
				End Try
			End If
		End Sub
	End Class

End Namespace