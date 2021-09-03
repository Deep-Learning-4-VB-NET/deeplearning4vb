Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports org.deeplearning4j.earlystopping.saver
Imports org.deeplearning4j.earlystopping.scorecalc
Imports EpochTerminationCondition = org.deeplearning4j.earlystopping.termination.EpochTerminationCondition
Imports IterationTerminationCondition = org.deeplearning4j.earlystopping.termination.IterationTerminationCondition
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports Model = org.deeplearning4j.nn.api.Model
Imports org.nd4j.common.function

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

Namespace org.deeplearning4j.earlystopping


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class EarlyStoppingConfiguration<T extends org.deeplearning4j.nn.api.Model> implements java.io.Serializable
	<Serializable>
	Public Class EarlyStoppingConfiguration(Of T As org.deeplearning4j.nn.api.Model)

		Private modelSaver As EarlyStoppingModelSaver(Of T)
		Private epochTerminationConditions As IList(Of EpochTerminationCondition)
		Private iterationTerminationConditions As IList(Of IterationTerminationCondition)
		Private saveLastModel As Boolean
		Private evaluateEveryNEpochs As Integer
		Private scoreCalculator As ScoreCalculator(Of T)
		Private scoreCalculatorSupplier As Supplier(Of ScoreCalculator)

		Private Sub New(ByVal builder As Builder(Of T))
			Me.modelSaver = builder.modelSaver_Conflict
			Me.epochTerminationConditions = builder.epochTerminationConditions_Conflict
			Me.iterationTerminationConditions = builder.iterationTerminationConditions_Conflict
			Me.saveLastModel = builder.saveLastModel_Conflict
			Me.evaluateEveryNEpochs = builder.evaluateEveryNEpochs_Conflict
			Me.scoreCalculator = builder.scoreCalculator_Conflict
			Me.scoreCalculatorSupplier = builder.scoreCalculatorSupplier
		End Sub

		Public Overridable ReadOnly Property ScoreCalculator As ScoreCalculator(Of T)
			Get
				If scoreCalculatorSupplier IsNot Nothing Then
					Return scoreCalculatorSupplier.get()
				End If
				Return scoreCalculator
			End Get
		End Property


		Public Overridable Sub validate()
			If scoreCalculator Is Nothing AndAlso scoreCalculatorSupplier Is Nothing Then
				Throw New DL4JInvalidConfigException("A score calculator or score calculator supplier must be defined.")
			End If

			If modelSaver Is Nothing Then
				Throw New DL4JInvalidConfigException("A model saver must be defined")
			End If

			Dim hasTermination As Boolean = False
			If iterationTerminationConditions IsNot Nothing AndAlso iterationTerminationConditions.Count > 0 Then
				hasTermination = True

			ElseIf epochTerminationConditions IsNot Nothing AndAlso epochTerminationConditions.Count > 0 Then
				hasTermination = True
			End If

			If Not hasTermination Then
				Throw New DL4JInvalidConfigException("No termination conditions defined.")
			End If
		End Sub


		Public Class Builder(Of T As org.deeplearning4j.nn.api.Model)

'JAVA TO VB CONVERTER NOTE: The field modelSaver was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend modelSaver_Conflict As EarlyStoppingModelSaver(Of T) = New InMemoryModelSaver(Of T)()
'JAVA TO VB CONVERTER NOTE: The field epochTerminationConditions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend epochTerminationConditions_Conflict As IList(Of EpochTerminationCondition) = New List(Of EpochTerminationCondition)()
'JAVA TO VB CONVERTER NOTE: The field iterationTerminationConditions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend iterationTerminationConditions_Conflict As IList(Of IterationTerminationCondition) = New List(Of IterationTerminationCondition)()
'JAVA TO VB CONVERTER NOTE: The field saveLastModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend saveLastModel_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field evaluateEveryNEpochs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend evaluateEveryNEpochs_Conflict As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field scoreCalculator was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend scoreCalculator_Conflict As ScoreCalculator(Of T)
			Friend scoreCalculatorSupplier As Supplier(Of ScoreCalculator)


			''' <summary>
			''' How should models be saved? (Default: in memory) </summary>
'JAVA TO VB CONVERTER NOTE: The parameter modelSaver was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function modelSaver(ByVal modelSaver_Conflict As EarlyStoppingModelSaver(Of T)) As Builder(Of T)
				Me.modelSaver_Conflict = modelSaver_Conflict
				Return Me
			End Function

			''' <summary>
			''' Termination conditions to be evaluated every N epochs, with N set by evaluateEveryNEpochs option </summary>
			Public Overridable Function epochTerminationConditions(ParamArray ByVal terminationConditions() As EpochTerminationCondition) As Builder(Of T)
				epochTerminationConditions_Conflict.Clear()
				Collections.addAll(epochTerminationConditions_Conflict, terminationConditions)
				Return Me
			End Function

			''' <summary>
			''' Termination conditions to be evaluated every N epochs, with N set by evaluateEveryNEpochs option </summary>
			Public Overridable Function epochTerminationConditions(ByVal terminationConditions As IList(Of EpochTerminationCondition)) As Builder(Of T)
				Me.epochTerminationConditions_Conflict = terminationConditions
				Return Me
			End Function

			''' <summary>
			''' Termination conditions to be evaluated every iteration (minibatch) </summary>
			Public Overridable Function iterationTerminationConditions(ParamArray ByVal terminationConditions() As IterationTerminationCondition) As Builder(Of T)
				iterationTerminationConditions_Conflict.Clear()
				Collections.addAll(iterationTerminationConditions_Conflict, terminationConditions)
				Return Me
			End Function

			''' <summary>
			''' Save the last model? If true: save the most recent model at each epoch, in addition to the best
			''' model (whenever the best model improves). If false: only save the best model. Default: false
			''' Useful for example if you might want to continue training after a max-time terminatino condition
			''' occurs.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter saveLastModel was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function saveLastModel(ByVal saveLastModel_Conflict As Boolean) As Builder(Of T)
				Me.saveLastModel_Conflict = saveLastModel_Conflict
				Return Me
			End Function

			''' <summary>
			''' How frequently should evaluations be conducted (in terms of epochs)? Defaults to every (1) epochs. </summary>
			Public Overridable Function evaluateEveryNEpochs(ByVal everyNEpochs As Integer) As Builder(Of T)
				Me.evaluateEveryNEpochs_Conflict = everyNEpochs
				Return Me
			End Function

			''' <summary>
			''' Score calculator. Used to calculate a score (such as loss function on a test set), every N epochs,
			''' where N is set by <seealso cref="evaluateEveryNEpochs"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter scoreCalculator was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function scoreCalculator(ByVal scoreCalculator_Conflict As ScoreCalculator) As Builder(Of T)
				Me.scoreCalculator_Conflict = scoreCalculator_Conflict
				Return Me
			End Function

			''' <summary>
			''' Score calculator. Used to calculate a score (such as loss function on a test set), every N epochs,
			''' where N is set by <seealso cref="evaluateEveryNEpochs"/>
			''' </summary>
			Public Overridable Function scoreCalculator(ByVal scoreCalculatorSupplier As Supplier(Of ScoreCalculator)) As Builder(Of T)
				Me.scoreCalculatorSupplier = scoreCalculatorSupplier
				Return Me
			End Function

			''' <summary>
			''' Create the early stopping configuration </summary>
			Public Overridable Function build() As EarlyStoppingConfiguration(Of T)
				Return New EarlyStoppingConfiguration(Of T)(Me)
			End Function

		End Class
	End Class

End Namespace