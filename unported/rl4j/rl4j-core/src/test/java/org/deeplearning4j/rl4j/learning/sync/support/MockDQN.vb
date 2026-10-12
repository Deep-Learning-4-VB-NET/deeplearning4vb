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

Namespace org.deeplearning4j.rl4j.learning.sync.support


	Public Class MockDQN
		Implements IDQN

		Private ReadOnly mult As Double

		Public Sub New()
			Me.New(1.0)
		End Sub

		Public Sub New(ByVal mult As Double)
			Me.mult = mult
		End Sub

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

		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels As INDArray)

		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)

		End Sub

		Public Overrides Function output(ByVal batch As INDArray) As NeuralNetOutput
			Dim result As New NeuralNetOutput()
			Dim data As INDArray = batch
			If mult <> 1.0 Then
				data = data.dup().muli(mult)
			End If
			result.put(CommonOutputNames.QValues, data)

			Return result
		End Function

		Public Overrides Function output(ByVal features As Features) As NeuralNetOutput
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function output(ByVal observation As Observation) As NeuralNetOutput
			Return Me.output(observation.Data)
		End Function

		Public Overrides Function outputAll(ByVal batch As INDArray) As INDArray()
			Return New INDArray(){}
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
			Return Nothing
		End Function

		Public Overridable Function gradient(ByVal input As INDArray, ByVal label As INDArray) As Gradient()
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