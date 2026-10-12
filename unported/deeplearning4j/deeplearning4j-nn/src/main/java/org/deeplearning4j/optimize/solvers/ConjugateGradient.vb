Imports System
Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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
	Public Class ConjugateGradient
		Inherits BaseOptimizer

		Private Const serialVersionUID As Long = -1269296013474864091L
		Private Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(ConjugateGradient))


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal stepFunction As StepFunction, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal model As Model)
			MyBase.New(conf, stepFunction, trainingListeners, model)
		End Sub

		Public Overrides Sub preProcessLine()
			Dim gradient As INDArray = DirectCast(searchState(GRADIENT_KEY), INDArray)
			Dim searchDir As INDArray = DirectCast(searchState(SEARCH_DIR), INDArray)
			If searchDir Is Nothing Then
				searchState(SEARCH_DIR) = gradient
			Else
				searchDir.assign(gradient)
			End If
		End Sub

		Public Overrides Sub postStep(ByVal gradient As INDArray)
			'line is current gradient
			'Last gradient is stored in searchState map
			Dim gLast As INDArray = DirectCast(searchState(GRADIENT_KEY), INDArray) 'Previous iteration gradient
			Dim searchDirLast As INDArray = DirectCast(searchState(SEARCH_DIR), INDArray) 'Previous iteration search dir

			'Calculate gamma (or beta, by Bengio et al. notation). Polak and Ribiere method.
			' = ((grad(current)-grad(last)) \dot (grad(current))) / (grad(last) \dot grad(last))
			Dim dgg As Double = Nd4j.BlasWrapper.dot(gradient.sub(gLast), gradient)
			Dim gg As Double = Nd4j.BlasWrapper.dot(gLast, gLast)
			Dim gamma As Double = Math.Max(dgg / gg, 0.0)
			If dgg <= 0.0 Then
				logger.debug("Polak-Ribiere gamma <= 0.0; using gamma=0.0 -> SGD line search. dgg={}, gg={}", dgg, gg)
			End If

			'Standard Polak-Ribiere does not guarantee that the search direction is a descent direction
			'But using max(gamma_Polak-Ribiere,0) does guarantee a descent direction. Hence the max above.
			'See Nocedal & Wright, Numerical Optimization, Ch5
			'If gamma==0.0, this is equivalent to SGD line search (i.e., search direction == negative gradient)

			'Compute search direction:
			'searchDir = gradient + gamma * searchDirLast
			Dim searchDir As INDArray = searchDirLast.muli(gamma).addi(gradient)

			'Store current gradient and search direction for
			'(a) use in BaseOptimizer.optimize(), and (b) next iteration
			searchState(GRADIENT_KEY) = gradient
			searchState(SEARCH_DIR) = searchDir
		End Sub



	End Class

End Namespace