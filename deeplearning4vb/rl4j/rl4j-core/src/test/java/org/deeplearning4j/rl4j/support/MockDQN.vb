Imports System.Collections.Generic
Imports System.IO
Imports NeuralNetwork = org.deeplearning4j.nn.api.NeuralNetwork
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports org.deeplearning4j.rl4j.network
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports org.deeplearning4j.rl4j.network.dqn
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.nd4j.common.primitives
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.support


	Public Class MockDQN
		Implements IDQN

		Public hasBeenReset As Boolean = False
		Public ReadOnly outputParams As IList(Of INDArray) = New List(Of INDArray)()
		Public ReadOnly fitParams As IList(Of Pair(Of INDArray, INDArray)) = New List(Of Pair(Of INDArray, INDArray))()
		Public ReadOnly gradientParams As IList(Of Pair(Of INDArray, INDArray)) = New List(Of Pair(Of INDArray, INDArray))()
		Public ReadOnly outputAllParams As IList(Of INDArray) = New List(Of INDArray)()

		Public Overrides ReadOnly Property NeuralNetworks As NeuralNetwork()
			Get
				Return New NeuralNetwork(){}
			End Get
		End Property

		Public Overrides ReadOnly Property Recurrent As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub reset()
			hasBeenReset = True
		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels As INDArray)
			fitParams.Add(New Pair(Of INDArray, INDArray)(input, labels))
		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)

		End Sub

		Public Overrides Function output(ByVal batch As INDArray) As NeuralNetOutput
			outputParams.Add(batch)

			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, batch)
			Return result
		End Function

		Public Overrides Function output(ByVal features As Features) As NeuralNetOutput
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function output(ByVal observation As Observation) As NeuralNetOutput
			Return Me.output(observation.Data)
		End Function

		Public Overrides Function outputAll(ByVal batch As INDArray) As INDArray()
			outputAllParams.Add(batch)
			Return New INDArray() { batch.mul(-1.0) }
		End Function

		Public Overrides Sub fit(ByVal featuresLabels As FeaturesLabels)
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Sub applyGradients(ByVal gradients As Gradients)
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides Sub copyFrom(ByVal from As ITrainableNeuralNet)
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides Function clone() As IDQN
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As New MockDQN()
			clone_Conflict.hasBeenReset = hasBeenReset

			Return clone_Conflict
		End Function

		Public Overridable Function gradient(ByVal input As INDArray, ByVal label As INDArray) As Gradient()
			gradientParams.Add(New Pair(Of INDArray, INDArray)(input, label))
			Return New Gradient(){}
		End Function

		Public Overridable Function gradient(ByVal input As INDArray, ByVal label() As INDArray) As Gradient()
			Return New Gradient(){}
		End Function

		Public Overrides Sub applyGradient(ByVal gradient() As Gradient, ByVal batchSize As Integer)

		End Sub

		Public Overrides ReadOnly Property LatestScore As Double
			Get
				Return 0
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(java.io.OutputStream os) throws java.io.IOException
		Public Overrides Sub save(ByVal os As Stream)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(String filename) throws java.io.IOException
		Public Overrides Sub save(ByVal filename As String)

		End Sub
	End Class
End Namespace