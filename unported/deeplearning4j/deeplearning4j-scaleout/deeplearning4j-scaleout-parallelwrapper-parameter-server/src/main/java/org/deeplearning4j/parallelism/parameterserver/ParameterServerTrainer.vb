Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper
Imports DefaultTrainer = org.deeplearning4j.parallelism.trainer.DefaultTrainer
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports ParameterServerClient = org.nd4j.parameterserver.client.ParameterServerClient

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Slf4j @AllArgsConstructor @NoArgsConstructor public class ParameterServerTrainer extends org.deeplearning4j.parallelism.trainer.DefaultTrainer
	Public Class ParameterServerTrainer
		Inherits DefaultTrainer

		Private parameterServerClient As ParameterServerClient

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void feedMultiDataSet(@NonNull MultiDataSet dataSet, long time)
		Public Overrides Sub feedMultiDataSet(ByVal dataSet As MultiDataSet, ByVal time As Long)
			' FIXME: this is wrong, and should be fixed

			If TypeOf Model Is ComputationGraph Then
				Dim computationGraph As ComputationGraph = DirectCast(Model, ComputationGraph)
				computationGraph.fit(dataSet)
			Else
				Throw New System.ArgumentException("MultiLayerNetworks can't fit multi datasets")
			End If

			log.info("Sending parameters")
			'send the updated params
			parameterServerClient.pushNDArray(Model.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void feedDataSet(@NonNull DataSet dataSet, long time)
		Public Overrides Sub feedDataSet(ByVal dataSet As DataSet, ByVal time As Long)
			' FIXME: this is wrong, and should be fixed. Training should happen within run() loop

			If TypeOf Model Is ComputationGraph Then
				Dim computationGraph As ComputationGraph = DirectCast(Model, ComputationGraph)
				computationGraph.fit(dataSet)
			Else
				Dim multiLayerNetwork As MultiLayerNetwork = DirectCast(Model, MultiLayerNetwork)
				log.info("Calling fit on multi layer network")
				multiLayerNetwork.fit(dataSet)

			End If

			log.info("About to send params in")
			'send the updated params
			parameterServerClient.pushNDArray(Model.params())
			log.info("Sent params")
		End Sub

		Public Overrides ReadOnly Property Model As Model
			Get
				Return MyBase.Model
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void updateModel(@NonNull Model model)
		Public Overrides Sub updateModel(ByVal model As Model)
			MyBase.updateModel(model)
		End Sub

		Public Class ParameterServerTrainerBuilder
			Inherits DefaultTrainerBuilder

'JAVA TO VB CONVERTER NOTE: The parameter originalModel was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function originalModel(ByVal originalModel_Conflict As Model) As ParameterServerTrainerBuilder
				Return CType(MyBase.originalModel(originalModel_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter replicatedModel was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function replicatedModel(ByVal replicatedModel_Conflict As Model) As ParameterServerTrainerBuilder
				Return CType(MyBase.replicatedModel(replicatedModel_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter queue was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function queue(ByVal queue_Conflict As LinkedBlockingQueue(Of DataSet)) As ParameterServerTrainerBuilder
				Return CType(MyBase.queue(queue_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter queueMDS was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function queueMDS(ByVal queueMDS_Conflict As LinkedBlockingQueue(Of MultiDataSet)) As ParameterServerTrainerBuilder
				Return CType(MyBase.queueMDS(queueMDS_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter running was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function running(ByVal running_Conflict As AtomicInteger) As ParameterServerTrainerBuilder
				Return CType(MyBase.running(running_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter threadId was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function threadId(ByVal threadId_Conflict As Integer) As ParameterServerTrainerBuilder
				Return CType(MyBase.threadId(threadId_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter shouldUpdate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function shouldUpdate(ByVal shouldUpdate_Conflict As AtomicBoolean) As ParameterServerTrainerBuilder
				Return CType(MyBase.shouldUpdate(shouldUpdate_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter shouldStop was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function shouldStop(ByVal shouldStop_Conflict As AtomicBoolean) As ParameterServerTrainerBuilder
				Return CType(MyBase.shouldStop(shouldStop_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter thrownException was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function thrownException(ByVal thrownException_Conflict As Exception) As ParameterServerTrainerBuilder
				Return CType(MyBase.thrownException(thrownException_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter useMDS was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function useMDS(ByVal useMDS_Conflict As Boolean) As ParameterServerTrainerBuilder
				Return CType(MyBase.useMDS(useMDS_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter onRootModel was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function onRootModel(ByVal onRootModel_Conflict As Boolean) As ParameterServerTrainerBuilder
				Return CType(MyBase.onRootModel(onRootModel_Conflict), ParameterServerTrainerBuilder)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter parallelWrapper was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function parallelWrapper(ByVal parallelWrapper_Conflict As ParallelWrapper) As ParameterServerTrainerBuilder
				Return CType(MyBase.parallelWrapper(parallelWrapper_Conflict), ParameterServerTrainerBuilder)
			End Function

			Public Overrides Function averagingFrequency(ByVal frequency As Integer) As ParameterServerTrainerBuilder
				Return CType(MyBase.averagingFrequency(frequency), ParameterServerTrainerBuilder)
			End Function
		End Class
	End Class

End Namespace