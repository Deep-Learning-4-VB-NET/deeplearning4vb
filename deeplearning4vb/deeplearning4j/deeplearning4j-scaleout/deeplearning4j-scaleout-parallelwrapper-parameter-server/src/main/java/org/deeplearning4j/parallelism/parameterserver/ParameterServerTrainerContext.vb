Imports MediaDriver = io.aeron.driver.MediaDriver
Imports Model = org.deeplearning4j.nn.api.Model
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper
Imports TrainerContext = org.deeplearning4j.parallelism.factory.TrainerContext
Imports Trainer = org.deeplearning4j.parallelism.trainer.Trainer
Imports ParameterServerClient = org.nd4j.parameterserver.client.ParameterServerClient
Imports ParameterServerNode = org.nd4j.parameterserver.node.ParameterServerNode

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

Namespace org.deeplearning4j.parallelism.parameterserver

	Public Class ParameterServerTrainerContext
		Implements TrainerContext

		Private parameterServerNode As ParameterServerNode
		Private mediaDriver As MediaDriver
		Private mediaDriverContext As MediaDriver.Context
		Private statusServerPort As Integer = 33000
		Private numUpdatesPerEpoch As Integer = 1
		Private parameterServerArgs() As String
		Private numWorkers As Integer = 1

		''' <summary>
		''' Initialize the context
		''' </summary>
		''' <param name="model"> </param>
		''' <param name="args"> the arguments to initialize with (maybe null) </param>
		Public Overridable Sub init(ByVal model As Model, ParamArray ByVal args() As Object) Implements TrainerContext.init
			mediaDriverContext = New MediaDriver.Context()
			mediaDriver = MediaDriver.launchEmbedded(mediaDriverContext)
			parameterServerNode = New ParameterServerNode(mediaDriver, statusServerPort, numWorkers)
			If parameterServerArgs Is Nothing Then
				parameterServerArgs = New String() {"-m", "true", "-s", "1," & model.numParams().ToString(), "-p", "40323", "-h", "localhost", "-id", "11", "-md", mediaDriver.aeronDirectoryName(), "-sh", "localhost", "-sp", statusServerPort.ToString(), "-u", numUpdatesPerEpoch.ToString()}
			End If

		End Sub

		''' <summary>
		''' Create a <seealso cref="Trainer"/>
		''' based on the given parameters
		''' </summary>
		''' <param name="threadId">   the thread id to use for this worker </param>
		''' <param name="model">      the model to start the trainer with </param>
		''' <param name="rootDevice"> the root device id </param>
		''' <param name="useMDS">     whether to use MultiDataSet or DataSet
		'''                   or not </param>
		''' <param name="wrapper">    the wrapper instance to use with this trainer (this refernece is needed
		'''                   for coordination with the <seealso cref="ParallelWrapper"/> 's <seealso cref="TrainingListener"/> </param>
		''' <returns> the created training instance </returns>
		Public Overridable Function create(ByVal uuid As String, ByVal threadId As Integer, ByVal model As Model, ByVal rootDevice As Integer, ByVal useMDS As Boolean, ByVal wrapper As ParallelWrapper, ByVal mode As WorkspaceMode, ByVal averagingFrequency As Integer) As Trainer Implements TrainerContext.create
			Return ParameterServerTrainer.builder().originalModel(model).parameterServerClient(ParameterServerClient.builder().aeron(parameterServerNode.getAeron()).ndarrayRetrieveUrl(parameterServerNode.getSubscriber()(threadId).getResponder().connectionUrl()).ndarraySendUrl(parameterServerNode.getSubscriber()(threadId).getSubscriber().connectionUrl()).subscriberHost("localhost").masterStatusHost("localhost").masterStatusPort(statusServerPort).subscriberPort(40625 + threadId).subscriberStream(12 + threadId).build()).replicatedModel(model).threadId(threadId).parallelWrapper(wrapper).useMDS(useMDS).build()
		End Function

		Public Overridable Sub finalizeRound(ByVal originalModel As Model, ParamArray ByVal models() As Model) Implements TrainerContext.finalizeRound
			' no-op
		End Sub

		Public Overridable Sub finalizeTraining(ByVal originalModel As Model, ParamArray ByVal models() As Model) Implements TrainerContext.finalizeTraining
			' no-op
		End Sub
	End Class

End Namespace