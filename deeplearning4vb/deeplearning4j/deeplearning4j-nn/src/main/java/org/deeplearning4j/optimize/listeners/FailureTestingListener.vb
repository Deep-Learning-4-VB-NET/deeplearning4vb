Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.optimize.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class FailureTestingListener implements org.deeplearning4j.optimize.api.TrainingListener, java.io.Serializable
	<Serializable>
	Public Class FailureTestingListener
		Implements TrainingListener

		Public Enum FailureMode
			OOM
			SYSTEM_EXIT_1
			ILLEGAL_STATE
			INFINITE_SLEEP
		End Enum
		Public Enum CallType
			ANY
			EPOCH_START
			EPOCH_END
			FORWARD_PASS
			GRADIENT_CALC
			BACKWARD_PASS
			ITER_DONE

		End Enum
		Private ReadOnly trigger As FailureTrigger
		Private ReadOnly failureMode As FailureMode

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FailureTestingListener(@NonNull FailureMode mode, @NonNull FailureTrigger trigger)
		Public Sub New(ByVal mode As FailureMode, ByVal trigger As FailureTrigger)
			Me.trigger = trigger
			Me.failureMode = mode
		End Sub

		Public Overridable Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer) Implements TrainingListener.iterationDone
			[call](CallType.ITER_DONE, model)
		End Sub

		Public Overridable Sub onEpochStart(ByVal model As Model) Implements TrainingListener.onEpochStart
			[call](CallType.EPOCH_START, model)
		End Sub

		Public Overridable Sub onEpochEnd(ByVal model As Model) Implements TrainingListener.onEpochEnd
			[call](CallType.EPOCH_END, model)
		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray))
			[call](CallType.FORWARD_PASS, model)
		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray))
			[call](CallType.FORWARD_PASS, model)
		End Sub

		Public Overridable Sub onGradientCalculation(ByVal model As Model) Implements TrainingListener.onGradientCalculation
			[call](CallType.GRADIENT_CALC, model)
		End Sub

		Public Overridable Sub onBackwardPass(ByVal model As Model) Implements TrainingListener.onBackwardPass
			[call](CallType.BACKWARD_PASS, model)
		End Sub

		Protected Friend Overridable Sub [call](ByVal callType As CallType, ByVal model As Model)
			If Not trigger.initialized() Then
				trigger.initialize()
			End If

			Dim iter As Integer
			Dim epoch As Integer
			If TypeOf model Is MultiLayerNetwork Then
				iter = DirectCast(model, MultiLayerNetwork).IterationCount
				epoch = DirectCast(model, MultiLayerNetwork).EpochCount
			Else
				iter = DirectCast(model, ComputationGraph).IterationCount
				epoch = DirectCast(model, ComputationGraph).EpochCount
			End If
			Dim triggered As Boolean = trigger.triggerFailure(callType, iter, epoch, model)

			If triggered Then
				log.error("*** FailureTestingListener was triggered on iteration {}, epoch {} - Failure mode is set to {} ***", iter, epoch, failureMode)
				Select Case failureMode
					Case org.deeplearning4j.optimize.listeners.FailureTestingListener.FailureMode.OOM
						Dim list As IList(Of INDArray) = New List(Of INDArray)()
						Do
							Dim arr As INDArray = Nd4j.createUninitialized(1_000_000_000)
							list.Add(arr)
						Loop
						'break;
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case org.deeplearning4j.optimize.listeners.FailureTestingListener.FailureMode.SYSTEM_EXIT_1
						log.error("Exiting due to FailureTestingListener triggering - calling System.exit(1)")
						Environment.Exit(1)
					Case org.deeplearning4j.optimize.listeners.FailureTestingListener.FailureMode.ILLEGAL_STATE
						log.error("Throwing new IllegalStateException due to FailureTestingListener triggering")
						Throw New System.InvalidOperationException("FailureTestListener was triggered with failure mode " & failureMode & " - iteration " & iter & ", epoch " & epoch)
					Case org.deeplearning4j.optimize.listeners.FailureTestingListener.FailureMode.INFINITE_SLEEP
						Do
							Try
								Thread.Sleep(10000)
							Catch e As InterruptedException
								'Ignore
							End Try
						Loop
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case Else
						Throw New Exception("Unknown enum value: " & failureMode)
				End Select
			End If
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static abstract class FailureTrigger implements java.io.Serializable
		<Serializable>
		Public MustInherit Class FailureTrigger

'JAVA TO VB CONVERTER NOTE: The field initialized was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend initialized_Conflict As Boolean = False

			''' <summary>
			''' If true: trigger the failure. If false: don't trigger failure </summary>
			''' <param name="callType">  Type of call </param>
			''' <param name="iteration"> Iteration number </param>
			''' <param name="epoch">     Epoch number </param>
			''' <param name="model">     Model
			''' @return </param>
			Public MustOverride Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean

			Public Overridable Function initialized() As Boolean
				Return initialized_Conflict
			End Function

			Public Overridable Sub initialize()
				Me.initialized_Conflict = True
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public static class @And extends FailureTrigger
		<Serializable>
		Public Class [And]
			Inherits FailureTrigger

			Protected Friend triggers As IList(Of FailureTrigger)

			Public Sub New(ParamArray ByVal triggers() As FailureTrigger)
				Me.triggers = New List(Of FailureTrigger) From {triggers}
			End Sub

			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Dim b As Boolean = True
				For Each ft As FailureTrigger In triggers
					b = b And ft.triggerFailure(callType, iteration, epoch, model)
				Next ft
				Return b
			End Function

			Public Overrides Sub initialize()
				MyBase.initialize()
				For Each ft As FailureTrigger In triggers
					ft.initialize()
				Next ft
			End Sub
		End Class

		<Serializable>
		Public Class [Or]
			Inherits [And]

			Public Sub New(ParamArray ByVal triggers() As FailureTrigger)
				MyBase.New(triggers)
			End Sub

			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Dim b As Boolean = False
				For Each ft As FailureTrigger In triggers
					b = b Or ft.triggerFailure(callType, iteration, epoch, model)
				Next ft
				Return b
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class RandomProb extends FailureTrigger
		<Serializable>
		Public Class RandomProb
			Inherits FailureTrigger

			Friend ReadOnly callType As CallType
			Friend ReadOnly probability As Double
			Friend rng As Random

			Public Sub New(ByVal callType As CallType, ByVal probability As Double)
				Me.callType = callType
				Me.probability = probability
			End Sub

			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Return (Me.callType = CallType.ANY OrElse callType = Me.callType) AndAlso rng.NextDouble() < probability
			End Function

			Public Overrides Sub initialize()
				MyBase.initialize()
				Me.rng = New Random()
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class TimeSinceInitializedTrigger extends FailureTrigger
		<Serializable>
		Public Class TimeSinceInitializedTrigger
			Inherits FailureTrigger

			Friend ReadOnly msSinceInit As Long
			Friend initTime As Long

			Public Sub New(ByVal msSinceInit As Long)
				Me.msSinceInit = msSinceInit
			End Sub

			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Return (DateTimeHelper.CurrentUnixTimeMillis() - initTime) > msSinceInit
			End Function

			Public Overrides Sub initialize()
				MyBase.initialize()
				Me.initTime = DateTimeHelper.CurrentUnixTimeMillis()
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class UserNameTrigger extends FailureTrigger
		<Serializable>
		Public Class UserNameTrigger
			Inherits FailureTrigger

			Friend ReadOnly userName As String
			Friend shouldFail As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public UserNameTrigger(@NonNull String userName)
			Public Sub New(ByVal userName As String)
				Me.userName = userName
			End Sub


			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Return shouldFail
			End Function

			Public Overrides Sub initialize()
				MyBase.initialize()
				shouldFail = Me.userName.Equals(System.getProperty("user.name"), StringComparison.OrdinalIgnoreCase)
			End Sub
		End Class
		'System.out.println("Hostname: " + InetAddress.getLocalHost().getHostName());

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class HostNameTrigger extends FailureTrigger
		<Serializable>
		Public Class HostNameTrigger
			Inherits FailureTrigger

			Friend ReadOnly hostName As String
			Friend shouldFail As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public HostNameTrigger(@NonNull String hostName)
			Public Sub New(ByVal hostName As String)
				Me.hostName = hostName
			End Sub


			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Return shouldFail
			End Function

			Public Overrides Sub initialize()
				MyBase.initialize()
				Try
'JAVA TO VB CONVERTER NOTE: The variable hostname was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim hostname_Conflict As String = InetAddress.getLocalHost().getHostName()
					log.info("FailureTestingListere hostname: {}", hostname_Conflict)
					shouldFail = Me.hostName.Equals(hostname_Conflict, StringComparison.OrdinalIgnoreCase)
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class IterationEpochTrigger extends FailureTrigger
		<Serializable>
		Public Class IterationEpochTrigger
			Inherits FailureTrigger

			Friend ReadOnly isEpoch As Boolean
			Friend ReadOnly count As Integer

			Public Sub New(ByVal isEpoch As Boolean, ByVal count As Integer)
				Me.isEpoch = isEpoch
				Me.count = count
			End Sub

			Public Overrides Function triggerFailure(ByVal callType As CallType, ByVal iteration As Integer, ByVal epoch As Integer, ByVal model As Model) As Boolean
				Return (isEpoch AndAlso epoch = count) OrElse (Not isEpoch AndAlso iteration = count)
			End Function
		End Class


	End Class

End Namespace