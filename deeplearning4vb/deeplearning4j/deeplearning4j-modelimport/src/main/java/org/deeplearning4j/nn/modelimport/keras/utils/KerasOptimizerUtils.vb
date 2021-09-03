Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports org.nd4j.linalg.learning.config
Imports InverseSchedule = org.nd4j.linalg.schedule.InverseSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType

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

Namespace org.deeplearning4j.nn.modelimport.keras.utils


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasOptimizerUtils
	Public Class KerasOptimizerUtils

		Protected Friend Const LR As String = "lr"
		Protected Friend Const LR2 As String = "learning_rate"
		Protected Friend Const EPSILON As String = "epsilon"
		Protected Friend Const MOMENTUM As String = "momentum"
		Protected Friend Const BETA_1 As String = "beta_1"
		Protected Friend Const BETA_2 As String = "beta_2"
		Protected Friend Const DECAY As String = "decay"
		Protected Friend Const RHO As String = "rho"
		Protected Friend Const SCHEDULE_DECAY As String = "schedule_decay"

		''' <summary>
		''' Map Keras optimizer to DL4J IUpdater.
		''' </summary>
		''' <param name="optimizerConfig"> Optimizer configuration map </param>
		''' <returns> DL4J IUpdater instance </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static IUpdater mapOptimizer(java.util.Map<String, Object> optimizerConfig) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function mapOptimizer(ByVal optimizerConfig As IDictionary(Of String, Object)) As IUpdater

			If Not optimizerConfig.ContainsKey("class_name") Then
				Throw New InvalidKerasConfigurationException("Optimizer config does not contain a name field.")
			End If
			Dim optimizerName As String = DirectCast(optimizerConfig("class_name"), String)

			If Not optimizerConfig.ContainsKey("config") Then
				Throw New InvalidKerasConfigurationException("Field config missing from layer config")
			End If
			Dim optimizerParameters As IDictionary(Of String, Object) = DirectCast(optimizerConfig("config"), IDictionary(Of String, Object))

			Dim dl4jOptimizer As IUpdater


			Select Case optimizerName
				Case "Adam"
'JAVA TO VB CONVERTER NOTE: The variable lr was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim lr_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(LR), optimizerParameters(LR), optimizerParameters(LR2)), Double)
					Dim beta1 As Double = DirectCast(optimizerParameters(BETA_1), Double)
					Dim beta2 As Double = DirectCast(optimizerParameters(BETA_2), Double)
'JAVA TO VB CONVERTER NOTE: The variable epsilon was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim epsilon_Conflict As Double = DirectCast(optimizerParameters(EPSILON), Double)
'JAVA TO VB CONVERTER NOTE: The variable decay was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim decay_Conflict As Double = DirectCast(optimizerParameters(DECAY), Double)

					dl4jOptimizer = (New Adam.Builder()).beta1(beta1).beta2(beta2).epsilon(epsilon_Conflict).learningRate(lr_Conflict).learningRateSchedule(If(decay_Conflict = 0, Nothing, New InverseSchedule(ScheduleType.ITERATION, lr_Conflict, decay_Conflict, 1))).build()
				Case "Adadelta"
'JAVA TO VB CONVERTER NOTE: The variable rho was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim rho_Conflict As Double = DirectCast(optimizerParameters(RHO), Double)
'JAVA TO VB CONVERTER NOTE: The variable epsilon was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim epsilon_Conflict As Double = DirectCast(optimizerParameters(EPSILON), Double)
					' double decay = (double) optimizerParameters.get(DECAY); No decay in DL4J Adadelta

					dl4jOptimizer = (New AdaDelta.Builder()).epsilon(epsilon_Conflict).rho(rho_Conflict).build()
				Case "Adgrad"
'JAVA TO VB CONVERTER NOTE: The variable lr was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim lr_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(LR), optimizerParameters(LR), optimizerParameters(LR2)), Double)
'JAVA TO VB CONVERTER NOTE: The variable epsilon was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim epsilon_Conflict As Double = DirectCast(optimizerParameters(EPSILON), Double)
'JAVA TO VB CONVERTER NOTE: The variable decay was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim decay_Conflict As Double = DirectCast(optimizerParameters(DECAY), Double)

					dl4jOptimizer = (New AdaGrad.Builder()).epsilon(epsilon_Conflict).learningRate(lr_Conflict).learningRateSchedule(If(decay_Conflict = 0, Nothing, New InverseSchedule(ScheduleType.ITERATION, lr_Conflict, decay_Conflict, 1))).build()
				Case "Adamax"
'JAVA TO VB CONVERTER NOTE: The variable lr was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim lr_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(LR), optimizerParameters(LR), optimizerParameters(LR2)), Double)
					Dim beta1 As Double = DirectCast(optimizerParameters(BETA_1), Double)
					Dim beta2 As Double = DirectCast(optimizerParameters(BETA_2), Double)
'JAVA TO VB CONVERTER NOTE: The variable epsilon was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim epsilon_Conflict As Double = DirectCast(optimizerParameters(EPSILON), Double)

					dl4jOptimizer = New AdaMax(lr_Conflict, beta1, beta2, epsilon_Conflict)
				Case "Nadam"
'JAVA TO VB CONVERTER NOTE: The variable lr was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim lr_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(LR), optimizerParameters(LR), optimizerParameters(LR2)), Double)
					Dim beta1 As Double = DirectCast(optimizerParameters(BETA_1), Double)
					Dim beta2 As Double = DirectCast(optimizerParameters(BETA_2), Double)
'JAVA TO VB CONVERTER NOTE: The variable epsilon was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim epsilon_Conflict As Double = DirectCast(optimizerParameters(EPSILON), Double)
					Dim scheduleDecay As Double = DirectCast(optimizerParameters(SCHEDULE_DECAY), Double)

					dl4jOptimizer = (New Nadam.Builder()).beta1(beta1).beta2(beta2).epsilon(epsilon_Conflict).learningRate(lr_Conflict).learningRateSchedule(If(scheduleDecay = 0, Nothing, New InverseSchedule(ScheduleType.ITERATION, lr_Conflict, scheduleDecay, 1))).build()
				Case "SGD"
'JAVA TO VB CONVERTER NOTE: The variable lr was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim lr_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(LR), optimizerParameters(LR), optimizerParameters(LR2)), Double)
'JAVA TO VB CONVERTER NOTE: The variable momentum was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim momentum_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(EPSILON), optimizerParameters(EPSILON), optimizerParameters(MOMENTUM)), Double)

'JAVA TO VB CONVERTER NOTE: The variable decay was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim decay_Conflict As Double = DirectCast(optimizerParameters(DECAY), Double)

					dl4jOptimizer = (New Nesterovs.Builder()).momentum(momentum_Conflict).learningRate(lr_Conflict).learningRateSchedule(If(decay_Conflict = 0, Nothing, New InverseSchedule(ScheduleType.ITERATION, lr_Conflict, decay_Conflict, 1))).build()
				Case "RMSprop"
'JAVA TO VB CONVERTER NOTE: The variable lr was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim lr_Conflict As Double = DirectCast(If(optimizerParameters.ContainsKey(LR), optimizerParameters(LR), optimizerParameters(LR2)), Double)
'JAVA TO VB CONVERTER NOTE: The variable rho was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim rho_Conflict As Double = DirectCast(optimizerParameters(RHO), Double)
'JAVA TO VB CONVERTER NOTE: The variable epsilon was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim epsilon_Conflict As Double = DirectCast(optimizerParameters(EPSILON), Double)
'JAVA TO VB CONVERTER NOTE: The variable decay was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim decay_Conflict As Double = DirectCast(optimizerParameters(DECAY), Double)

					dl4jOptimizer = (New RmsProp.Builder()).epsilon(epsilon_Conflict).rmsDecay(rho_Conflict).learningRate(lr_Conflict).learningRateSchedule(If(decay_Conflict = 0, Nothing, New InverseSchedule(ScheduleType.ITERATION, lr_Conflict, decay_Conflict, 1))).build()
				Case Else
					Throw New UnsupportedKerasConfigurationException("Optimizer with name " & optimizerName & "can not be" & "matched to a DL4J optimizer. Note that custom TFOptimizers are not supported by model import")
			End Select

			Return dl4jOptimizer

		End Function
	End Class

End Namespace