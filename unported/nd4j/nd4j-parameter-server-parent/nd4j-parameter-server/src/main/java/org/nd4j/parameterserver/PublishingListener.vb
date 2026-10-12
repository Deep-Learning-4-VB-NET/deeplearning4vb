Imports System
Imports Aeron = io.aeron.Aeron
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AeronNDArrayPublisher = org.nd4j.aeron.ipc.AeronNDArrayPublisher
Imports NDArrayCallback = org.nd4j.aeron.ipc.NDArrayCallback
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
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

Namespace org.nd4j.parameterserver

	''' <summary>
	''' Publishing listener for
	''' publishing to a master url.
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Slf4j public class PublishingListener implements org.nd4j.aeron.ipc.NDArrayCallback
	Public Class PublishingListener
		Implements NDArrayCallback

		Private masterUrl As String
		Private streamId As Integer
		Private aeronContext As Aeron.Context

		''' <summary>
		''' A listener for ndarray message
		''' </summary>
		''' <param name="message"> the message for the callback </param>
		Public Overridable Sub onNDArrayMessage(ByVal message As NDArrayMessage)
			Try
					Using publisher As AeronNDArrayPublisher = AeronNDArrayPublisher.builder().streamId(streamId).ctx(aeronContext).channel(masterUrl).build()
					publisher.publish(message)
					log.debug("NDArray PublishingListener publishing to channel " & masterUrl & ":" & streamId)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Sub

		''' <summary>
		''' Used for partial updates using tensor along
		''' dimension
		''' </summary>
		''' <param name="arr">        the array to count as an update </param>
		''' <param name="idx">        the index for the tensor along dimension </param>
		''' <param name="dimensions"> the dimensions to act on for the tensor along dimension </param>
		Public Overridable Sub onNDArrayPartial(ByVal arr As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer) Implements NDArrayCallback.onNDArrayPartial
			Try
					Using publisher As AeronNDArrayPublisher = AeronNDArrayPublisher.builder().streamId(streamId).ctx(aeronContext).channel(masterUrl).build()
					publisher.publish(NDArrayMessage.builder().arr(arr).dimensions(dimensions).index(idx).build())
					log.debug("NDArray PublishingListener publishing to channel " & masterUrl & ":" & streamId)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' Setup an ndarray
		''' </summary>
		''' <param name="arr"> </param>
		Public Overridable Sub onNDArray(ByVal arr As INDArray) Implements NDArrayCallback.onNDArray
			Try
					Using publisher As AeronNDArrayPublisher = AeronNDArrayPublisher.builder().streamId(streamId).ctx(aeronContext).channel(masterUrl).build()
					publisher.publish(arr)
					log.debug("NDArray PublishingListener publishing to channel " & masterUrl & ":" & streamId)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try


		End Sub
	End Class

End Namespace