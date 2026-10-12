Imports System
Imports Builder = lombok.Builder
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.rl4j.learning
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports ActorCriticCompGraph = org.deeplearning4j.rl4j.network.ac.ActorCriticCompGraph
Imports org.deeplearning4j.rl4j.network.ac
Imports org.deeplearning4j.rl4j.network.ac
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
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

Namespace org.deeplearning4j.rl4j.policy


	Public Class ACPolicy(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits Policy(Of Integer)

'JAVA TO VB CONVERTER NOTE: The field neuralNet was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly neuralNet_Conflict As IOutputNeuralNet
		Private ReadOnly isTraining As Boolean
		Private ReadOnly rnd As Random

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public ACPolicy(@NonNull IOutputNeuralNet neuralNet, boolean isTraining, org.nd4j.linalg.api.rng.Random rnd)
		Public Sub New(ByVal neuralNet As IOutputNeuralNet, ByVal isTraining As Boolean, ByVal rnd As Random)
			Me.neuralNet_Conflict = neuralNet
			Me.isTraining = isTraining
			Me.rnd = If(Not isTraining OrElse rnd IsNot Nothing, rnd, Nd4j.Random)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable> ACPolicy<OBSERVATION> load(String path) throws java.io.IOException
		Public Shared Function load(Of OBSERVATION As Encodable)(ByVal path As String) As ACPolicy(Of OBSERVATION)
			' TODO: Add better load/save support
			Return New ACPolicy(Of OBSERVATION)(ActorCriticCompGraph.load(path), False, Nothing)
		End Function
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable> ACPolicy<OBSERVATION> load(String path, org.nd4j.linalg.api.rng.Random rnd) throws java.io.IOException
		Public Shared Function load(Of OBSERVATION As Encodable)(ByVal path As String, ByVal rnd As Random) As ACPolicy(Of OBSERVATION)
			' TODO: Add better load/save support
			Return New ACPolicy(Of OBSERVATION)(ActorCriticCompGraph.load(path), True, rnd)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable> ACPolicy<OBSERVATION> load(String pathValue, String pathPolicy) throws java.io.IOException
		Public Shared Function load(Of OBSERVATION As Encodable)(ByVal pathValue As String, ByVal pathPolicy As String) As ACPolicy(Of OBSERVATION)
			Return New ACPolicy(Of OBSERVATION)(ActorCriticSeparate.load(pathValue, pathPolicy), False, Nothing)
		End Function
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable> ACPolicy<OBSERVATION> load(String pathValue, String pathPolicy, org.nd4j.linalg.api.rng.Random rnd) throws java.io.IOException
		Public Shared Function load(Of OBSERVATION As Encodable)(ByVal pathValue As String, ByVal pathPolicy As String, ByVal rnd As Random) As ACPolicy(Of OBSERVATION)
			Return New ACPolicy(Of OBSERVATION)(ActorCriticSeparate.load(pathValue, pathPolicy), True, rnd)
		End Function

		<Obsolete>
		Public Overrides ReadOnly Property NeuralNet As IOutputNeuralNet
			Get
				Return neuralNet_Conflict
			End Get
		End Property

		Public Overrides Function nextAction(ByVal obs As Observation) As Integer?
			' Review: Should ActorCriticPolicy be a training policy only? Why not use the existing greedy policy when not training instead of duplicating the code?
			Dim output As INDArray = neuralNet_Conflict.output(obs).get(CommonOutputNames.ActorCritic.Policy)
			If Not isTraining Then
				Return Learning.getMaxAction(output)
			End If

			Dim rVal As Single = rnd.nextFloat()
			For i As Integer = 0 To output.length() - 1
				If rVal < output.getFloat(i) Then
					Return i
				Else
					rVal -= output.getFloat(i)
				End If
			Next i

			Throw New Exception("Output from network is not a probability distribution: " & output)
		End Function

		<Obsolete>
		Public Overridable Overloads Function nextAction(ByVal input As INDArray) As Integer?
			Dim output As INDArray = DirectCast(neuralNet_Conflict, IActorCritic).outputAll(input)(1)
			If rnd Is Nothing Then
				Return Learning.getMaxAction(output)
			End If
			Dim rVal As Single = rnd.nextFloat()
			For i As Integer = 0 To output.length() - 1
				'System.out.println(i + " " + rVal + " " + output.getFloat(i));
				If rVal < output.getFloat(i) Then
					Return i
				Else
					rVal -= output.getFloat(i)
				End If
			Next i

			Throw New Exception("Output from network is not a probability distribution: " & output)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String filename) throws java.io.IOException
		Public Overridable Sub save(ByVal filename As String)
			' TODO: Add better load/save support
			DirectCast(neuralNet_Conflict, IActorCritic).save(filename)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String filenameValue, String filenamePolicy) throws java.io.IOException
		Public Overridable Sub save(ByVal filenameValue As String, ByVal filenamePolicy As String)
			' TODO: Add better load/save support
			DirectCast(neuralNet_Conflict, IActorCritic).save(filenameValue, filenamePolicy)
		End Sub

	End Class

End Namespace