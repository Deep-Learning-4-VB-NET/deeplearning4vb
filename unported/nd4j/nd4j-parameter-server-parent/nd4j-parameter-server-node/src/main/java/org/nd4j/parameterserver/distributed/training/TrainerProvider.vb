Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
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
	Public Class TrainerProvider
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New TrainerProvider()

		' we use Class.getSimpleName() as key here
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected java.util.Map<String, TrainingDriver<?>> trainers = new java.util.HashMap<>();
		Protected Friend trainers As IDictionary(Of String, TrainingDriver(Of Object)) = New Dictionary(Of String, TrainingDriver(Of Object))()

		Protected Friend voidConfiguration As VoidConfiguration
		Protected Friend transport As Transport
		Protected Friend clipboard As Clipboard
		Protected Friend storage As Storage

		Private Sub New()
			loadProviders()
		End Sub

		Public Shared ReadOnly Property Instance As TrainerProvider
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Protected Friend Overridable Sub loadProviders()
			Dim serviceLoader As ServiceLoader(Of TrainingDriver) = ND4JClassLoading.loadService(GetType(TrainingDriver))
			For Each trainingDriver As TrainingDriver In serviceLoader
				trainers(trainingDriver.targetMessageClass()) = trainingDriver
			Next trainingDriver

			If trainers.Count = 0 Then
				Throw New ND4JIllegalStateException("No TrainingDrivers were found via ServiceLoader mechanism")
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport, @NonNull Storage storage, @NonNull Clipboard clipboard)
		Public Overridable Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport, ByVal storage As Storage, ByVal clipboard As Clipboard)
			Me.voidConfiguration = voidConfiguration
			Me.transport = transport
			Me.clipboard = clipboard
			Me.storage = storage

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (TrainingDriver<?> trainer : trainers.values())
			For Each trainer As TrainingDriver(Of Object) In trainers.Values
				trainer.init(voidConfiguration, transport, storage, clipboard)
			Next trainer
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> TrainingDriver<T> getTrainer(T message)
		Protected Friend Overridable Function getTrainer(Of T As TrainingMessage)(ByVal message As T) As TrainingDriver(Of T)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: TrainingDriver<?> driver = trainers.get(message.getClass().getSimpleName());
			Dim driver As TrainingDriver(Of Object) = trainers(message.GetType().Name)
			If driver Is Nothing Then
				Throw New ND4JIllegalStateException("Can't find trainer for [" & message.GetType().Name & "]")
			End If

			Return CType(driver, TrainingDriver(Of T))
		End Function

		Public Overridable Sub doTraining(Of T As TrainingMessage)(ByVal message As T)
			Dim trainer As TrainingDriver(Of T) = getTrainer(message)
			trainer.startTraining(message)
		End Sub
	End Class

End Namespace