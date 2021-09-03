Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Value = lombok.Value
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
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

Namespace org.deeplearning4j.rl4j.learning

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class Learning<OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable, A, @AS extends org.deeplearning4j.rl4j.space.ActionSpace<A>, NN extends org.deeplearning4j.rl4j.network.NeuralNet> implements ILearning<OBSERVATION, A, @AS>, NeuralNetFetchable<NN>
	Public MustInherit Class Learning(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, A, [AS] As org.deeplearning4j.rl4j.space.ActionSpace(Of A), NN As org.deeplearning4j.rl4j.network.NeuralNet)
		Implements ILearning(Of OBSERVATION, A, [AS]), NeuralNetFetchable(Of NN)

		Public MustOverride ReadOnly Property HistoryProcessor As IHistoryProcessor Implements ILearning(Of OBSERVATION, A, [AS]).getHistoryProcessor
		Public MustOverride Function getMdp() As org.deeplearning4j.rl4j.mdp.MDP(Of OBSERVATION, A, [AS]) Implements ILearning(Of OBSERVATION, A, [AS]).getMdp
		Public MustOverride ReadOnly Property Configuration As org.deeplearning4j.rl4j.learning.configuration.ILearningConfiguration Implements ILearning(Of OBSERVATION, A, [AS]).getConfiguration
		Public MustOverride ReadOnly Property StepCount As Integer Implements ILearning(Of OBSERVATION, A, [AS]).getStepCount
		Public MustOverride Sub train() Implements ILearning(Of OBSERVATION, A, [AS]).train
		Public MustOverride Function getPolicy() As org.deeplearning4j.rl4j.policy.IPolicy(Of A) Implements ILearning(Of OBSERVATION, A, [AS]).getPolicy
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int stepCount = 0;
		Protected Friend stepCount As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int epochCount = 0;
		Private epochCount As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private IHistoryProcessor historyProcessor = null;
'JAVA TO VB CONVERTER NOTE: The field historyProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private historyProcessor_Conflict As IHistoryProcessor = Nothing

		Public Shared Function getMaxAction(ByVal vector As INDArray) As Integer?
			Return Nd4j.argMax(vector, Integer.MaxValue).getInt(0)
		End Function

		Public Shared Function makeShape(ByVal size As Integer, ByVal shape() As Integer) As Integer()
			Dim nshape(shape.Length) As Integer
			nshape(0) = size
			Array.Copy(shape, 0, nshape, 1, shape.Length)
			Return nshape
		End Function

		Public Shared Function makeShape(ByVal batch As Integer, ByVal shape() As Integer, ByVal length As Integer) As Integer()
			Dim nshape(2) As Integer
			nshape(0) = batch
			nshape(1) = 1
			For i As Integer = 0 To shape.Length - 1
				nshape(1) *= shape(i)
			Next i
			nshape(2) = length
			Return nshape
		End Function

		Public MustOverride ReadOnly Property NeuralNet As NN Implements NeuralNetFetchable(Of NN).getNeuralNet

		Public Overridable Sub incrementStep()
			stepCount += 1
		End Sub

		Public Overridable Sub incrementEpoch()
			epochCount += 1
		End Sub

		Public Overridable WriteOnly Property HistoryProcessor As HistoryProcessor.Configuration
			Set(ByVal conf As HistoryProcessor.Configuration)
				setHistoryProcessor(New HistoryProcessor(conf))
			End Set
		End Property

		Public Overridable WriteOnly Property HistoryProcessor As IHistoryProcessor
			Set(ByVal historyProcessor As IHistoryProcessor)
				Me.historyProcessor_Conflict = historyProcessor
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Value public static class InitMdp<O>
		Public Class InitMdp(Of O)
			Friend steps As Integer
			Friend lastObs As O
			Friend reward As Double
		End Class

	End Class

End Namespace