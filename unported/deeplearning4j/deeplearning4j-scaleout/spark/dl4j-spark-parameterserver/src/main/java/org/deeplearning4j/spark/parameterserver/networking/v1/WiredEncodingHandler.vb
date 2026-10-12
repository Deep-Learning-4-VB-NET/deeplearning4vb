Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports EncodingHandler = org.deeplearning4j.optimize.solvers.accumulation.EncodingHandler
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports SilentUpdatesMessage = org.deeplearning4j.spark.parameterserver.networking.v1.messages.SilentUpdatesMessage
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidParameterServer = org.nd4j.parameterserver.distributed.VoidParameterServer

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

Namespace org.deeplearning4j.spark.parameterserver.networking.v1


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class WiredEncodingHandler extends org.deeplearning4j.optimize.solvers.accumulation.EncodingHandler
	<Obsolete, Serializable>
	Public Class WiredEncodingHandler
		Inherits EncodingHandler

		Protected Friend updatesCounter As New AtomicLong(0)

		''' <summary>
		''' This method builds new WiredEncodingHandler instance
		''' </summary>
		''' <param name="thresholdAlgorithm"> threshold algorithm to use </param>
		''' <param name="boundary"> </param>
		Public Sub New(ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal residualPostProcessor As ResidualPostProcessor, ByVal boundary As Integer?, ByVal encodingDebugMode As Boolean)
			MyBase.New(thresholdAlgorithm, residualPostProcessor, boundary, encodingDebugMode)
		End Sub

		''' <summary>
		''' This method sends given message to all registered recipients
		''' </summary>
		''' <param name="message"> </param>
		Protected Friend Overrides Sub sendMessage(ByVal message As INDArray, ByVal iterationNumber As Integer, ByVal epochNumber As Integer)
			' here we'll send our stuff to other executores over the wire
			' and let's pray for udp broadcast availability

			' Send this message away
			' FIXME: do something with unsafe duplication, which is bad and used ONLY for local spark
			Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Dim updateId As Long = updatesCounter.getAndIncrement()

				VoidParameterServer.Instance.execDistributedImmediately(New SilentUpdatesMessage(message.unsafeDuplication(), updateId))
			End Using


			' heere we update local queue
			MyBase.sendMessage(message, iterationNumber, epochNumber)
		End Sub
	End Class

End Namespace