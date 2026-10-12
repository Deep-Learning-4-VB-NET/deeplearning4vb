Imports System
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport

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

Namespace org.nd4j.parameterserver.distributed.training

	<Obsolete>
	Public Interface TrainingDriver(Of T As org.nd4j.parameterserver.distributed.messages.TrainingMessage)

		Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport, ByVal storage As Storage, ByVal clipboard As Clipboard)

		Sub startTraining(ByVal message As T)

		Sub pickTraining(ByVal message As T)

		Sub aggregationFinished(ByVal aggregation As VoidAggregation)

		Sub finishTraining(ByVal originatorId As Long, ByVal taskId As Long)

		Sub addCompletionHook(ByVal originatorId As Long, ByVal frameId As Long, ByVal messageId As Long)

		Function targetMessageClass() As String
	End Interface

End Namespace