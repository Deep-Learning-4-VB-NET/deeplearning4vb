Imports System
Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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


	''' <summary>
	''' LBFGS
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class LBFGS
		Inherits BaseOptimizer

		Private Const serialVersionUID As Long = 9148732140255034888L
		Private m As Integer = 4

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal stepFunction As StepFunction, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal model As Model)
			MyBase.New(conf, stepFunction, trainingListeners, model)
		End Sub

		Public Overridable Overloads Sub setupSearchState(ByVal pair As Pair(Of Gradient, Double))
			MyBase.setupSearchState(pair)
			Dim params As INDArray = DirectCast(searchState(PARAMS_KEY), INDArray)
			searchState("s") = New LinkedList(Of INDArray)() ' holds parameters differences
			searchState("y") = New LinkedList(Of INDArray)() ' holds gradients differences
			searchState("rho") = New LinkedList(Of Double)()
			searchState("oldparams") = params.dup()

		End Sub

		Public Overrides Sub preProcessLine()
			If Not searchState.ContainsKey(SEARCH_DIR) Then
				searchState(SEARCH_DIR) = DirectCast(searchState(GRADIENT_KEY), INDArray).dup()
			End If
		End Sub

		' Numerical Optimization (Nocedal & Wright) section 7.2
		' s = parameters differences (old & current)
		' y = gradient differences (old & current)
		' gamma = initial Hessian approximation (i.e., equiv. to gamma*IdentityMatrix for Hessian)
		' rho = scalar. rho_i = 1/(y_i \dot s_i)
		Public Overrides Sub postStep(ByVal gradient As INDArray)
			Dim previousParameters As INDArray = DirectCast(searchState("oldparams"), INDArray)
			Dim parameters As INDArray = model.params()
			Dim previousGradient As INDArray = DirectCast(searchState(GRADIENT_KEY), INDArray)

			Dim rho As LinkedList(Of Double) = DirectCast(searchState("rho"), LinkedList(Of Double))
			Dim s As LinkedList(Of INDArray) = DirectCast(searchState("s"), LinkedList(Of INDArray))
			Dim y As LinkedList(Of INDArray) = DirectCast(searchState("y"), LinkedList(Of INDArray))

			Dim sy As Double = Nd4j.BlasWrapper.dot(previousParameters, previousGradient) + Nd4j.EPS_THRESHOLD
			Dim yy As Double = Nd4j.BlasWrapper.dot(previousGradient, previousGradient) + Nd4j.EPS_THRESHOLD

			Dim sCurrent As INDArray
			Dim yCurrent As INDArray
			If s.Count >= m Then
				'Optimization: Remove old (no longer needed) INDArrays, and use assign for re-use.
				'Better to do this: fewer objects created -> less memory overall + less garbage collection
				sCurrent = s.RemoveLast()
				yCurrent = y.RemoveLast()
				rho.RemoveLast()
				sCurrent.assign(parameters).subi(previousParameters)
				yCurrent.assign(gradient).subi(previousGradient)
			Else
				'First few iterations. Need to allocate new INDArrays for storage (via copy operation sub)
				sCurrent = parameters.sub(previousParameters)
				yCurrent = gradient.sub(previousGradient)
			End If

			rho.AddFirst(1.0 / sy) 'Most recent first
			s.AddFirst(sCurrent) 'Most recent first. si = currParams - oldParams
			y.AddFirst(yCurrent) 'Most recent first. yi = currGradient - oldGradient

			'assert (s.size()==y.size()) : "Gradient and parameter sizes are not equal";
			If s.Count <> y.Count Then
				Throw New System.InvalidOperationException("Gradient and parameter sizes are not equal")
			End If

			'In general: have m elements in s,y,rho.
			'But for first few iterations, have less.
			Dim numVectors As Integer = Math.Min(m, s.Count)

			Dim alpha(numVectors - 1) As Double

			' First work backwards, from the most recent difference vectors
			Dim sIter As IEnumerator(Of INDArray) = s.GetEnumerator()
			Dim yIter As IEnumerator(Of INDArray) = y.GetEnumerator()
			Dim rhoIter As IEnumerator(Of Double) = rho.GetEnumerator()

			'searchDir: first used as equivalent to q as per N&W, then later used as r as per N&W.
			'Re-using existing array for performance reasons
			Dim searchDir As INDArray = DirectCast(searchState(SEARCH_DIR), INDArray)
			searchDir.assign(gradient)

			For i As Integer = 0 To numVectors - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim si As INDArray = sIter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim yi As INDArray = yIter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim rhoi As Double = rhoIter.next()

				If si.length() <> searchDir.length() Then
					Throw New System.InvalidOperationException("Gradients and parameters length not equal")
				End If

				alpha(i) = rhoi * Nd4j.BlasWrapper.dot(si, searchDir)
				Nd4j.BlasWrapper.level1().axpy(searchDir.length(), -alpha(i), yi, searchDir) 'q = q-alpha[i]*yi
			Next i

			'Use Hessian approximation initialization scheme
			'searchDir = H0*q = (gamma*IdentityMatrix)*q = gamma*q
			Dim gamma As Double = sy / yy
			searchDir.muli(gamma)

			'Reverse iterators: end to start. Java LinkedLists are doubly-linked,
			' so still O(1) for reverse iteration operations.
			sIter = s.GetReverse().GetEnumerator()
			yIter = y.GetReverse().GetEnumerator()
			rhoIter = rho.GetReverse().GetEnumerator()
			For i As Integer = 0 To numVectors - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim si As INDArray = sIter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim yi As INDArray = yIter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim rhoi As Double = rhoIter.next()

				Dim beta As Double = rhoi * Nd4j.BlasWrapper.dot(yi, searchDir) 'beta = rho_i * y_i^T * r
				'r = r + s_i * (alpha_i - beta)
				Nd4j.BlasWrapper.level1().axpy(gradient.length(), alpha(i) - beta, si, searchDir)
			Next i

			previousParameters.assign(parameters)
			previousGradient.assign(gradient) 'Update gradient. Still in searchState map keyed by GRADIENT_KEY
		End Sub
	End Class

End Namespace