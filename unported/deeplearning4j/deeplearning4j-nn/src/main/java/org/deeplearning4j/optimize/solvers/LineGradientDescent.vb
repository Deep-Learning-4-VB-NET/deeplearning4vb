Imports System
Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
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

Namespace org.deeplearning4j.optimize.solvers


	<Serializable>
	Public Class LineGradientDescent
		Inherits BaseOptimizer

		Private Const serialVersionUID As Long = 6336124657542062284L

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal stepFunction As StepFunction, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal model As Model)
			MyBase.New(conf, stepFunction, trainingListeners, model)
		End Sub

		Public Overrides Sub preProcessLine()
			Dim gradient As INDArray = DirectCast(searchState(GRADIENT_KEY), INDArray)
			searchState(SEARCH_DIR) = gradient.dup()
		End Sub

		Public Overrides Sub postStep(ByVal gradient As INDArray)
			Dim norm2 As Double = Nd4j.BlasWrapper.level1().nrm2(gradient)
			If norm2 > stepMax Then
				searchState(SEARCH_DIR) = gradient.dup().muli(stepMax / norm2)
			Else
				searchState(SEARCH_DIR) = gradient.dup()
			End If
			searchState(GRADIENT_KEY) = gradient.dup()
		End Sub

	End Class

End Namespace