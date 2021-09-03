Imports FastMath = org.apache.commons.math3.util.FastMath
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Max = org.nd4j.linalg.api.ops.impl.transforms.custom.Max
Imports Sqrt = org.nd4j.linalg.api.ops.impl.transforms.floating.Sqrt
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.nd4j.linalg.ops.transforms.Transforms.sqrt

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
Namespace org.nd4j.linalg.learning

	Public Class UpdaterJavaCode

		Private Sub New()
		End Sub

		Public Shared Sub applyAdaDeltaUpdater(ByVal gradient As INDArray, ByVal msg As INDArray, ByVal msdx As INDArray, ByVal rho As Double, ByVal epsilon As Double)

			'Line 4 of Algorithm 1: https://arxiv.org/pdf/1212.5701v1.pdf
			'E[g^2]_t = rho * E[g^2]_{t-1} + (1-rho)*g^2_t
			msg.muli(rho).addi(gradient.mul(gradient).muli(1 - rho))

			'Calculate update:
			'dX = - g * RMS[delta x]_{t-1} / RMS[g]_t
			'Note: negative is applied in the DL4J step function: params -= update rather than params += update
			Dim rmsdx_t1 As INDArray = Transforms.sqrt(msdx.add(epsilon), False)
			Dim rmsg_t As INDArray = Transforms.sqrt(msg.add(epsilon), False)
			Dim update As INDArray = gradient.muli(rmsdx_t1.divi(rmsg_t))

			'Accumulate gradients: E[delta x^2]_t = rho * E[delta x^2]_{t-1} + (1-rho)* (delta x_t)^2
			msdx.muli(rho).addi(update.mul(update).muli(1 - rho))
		End Sub

		Public Shared Sub applyAdaGradUpdater(ByVal gradient As INDArray, ByVal state As INDArray, ByVal learningRate As Double, ByVal epsilon As Double)
			state.addi(gradient.mul(gradient))

			Dim sqrtHistory As INDArray = sqrt(state.dup("c"c), False).addi(epsilon)
			' lr * gradient / (sqrt(sumSquaredGradients) + epsilon)
			gradient.muli(sqrtHistory.rdivi(learningRate))
		End Sub


		Public Shared Sub applyAdamUpdater(ByVal gradient As INDArray, ByVal m As INDArray, ByVal v As INDArray, ByVal learningRate As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)

			Dim oneMinusBeta1Grad As INDArray = gradient.mul(1.0 - beta1)
			m.muli(beta1).addi(oneMinusBeta1Grad)

			Dim oneMinusBeta2GradSquared As INDArray = gradient.mul(gradient).muli(1 - beta2)
			v.muli(beta2).addi(oneMinusBeta2GradSquared)

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)
			Dim beta2t As Double = FastMath.pow(beta2, iteration + 1)

			Dim alphat As Double = learningRate * FastMath.sqrt(1 - beta2t) / (1 - beta1t)
			If Double.IsNaN(alphat) OrElse alphat = 0.0 Then
				alphat = epsilon
			End If
			Dim sqrtV As INDArray = Transforms.sqrt(v.dup("c"c), False).addi(epsilon)

			gradient.assign(m).muli(alphat).divi(sqrtV)
		End Sub

		Public Shared Sub applyAdaMaxUpdater(ByVal gradient As INDArray, ByVal m As INDArray, ByVal v As INDArray, ByVal learningRate As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)

			'm = B_1 * m + (1-B_1)*grad
			m.muli(beta1).addi(gradient.mul(1 - beta1))

			'u = max(B_2 * u, |grad|)
			v.muli(beta2)
			Transforms.abs(gradient, False) 'In-place should be OK here, original gradient values aren't used again later
			Nd4j.Executioner.exec(New Max(v, gradient, v))

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)

			Dim alphat As Double = learningRate / (1.0 - beta1t)
			If Double.IsNaN(alphat) OrElse Double.IsInfinity(alphat) OrElse alphat = 0.0 Then
				alphat = epsilon
			End If

			v.addi(1e-32) ' prevent NaNs in params
			gradient.assign(m).muli(alphat).divi(v)
		End Sub

		Public Shared Sub applyAmsGradUpdater(ByVal gradient As INDArray, ByVal m As INDArray, ByVal v As INDArray, ByVal vHat As INDArray, ByVal learningRate As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)
			'm_t = b_1 * m_{t-1} + (1-b_1) * g_t       eq 1 pg 3
			Dim oneMinusBeta1Grad As INDArray = gradient.mul(1.0 - beta1)
			m.muli(beta1).addi(oneMinusBeta1Grad)

			'v_t = b_2 * v_{t-1} + (1-b_2) * (g_t)^2   eq 1 pg 3
			Dim oneMinusBeta2GradSquared As INDArray = gradient.mul(gradient).muli(1 - beta2)
			v.muli(beta2).addi(oneMinusBeta2GradSquared)

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)
			Dim beta2t As Double = FastMath.pow(beta2, iteration + 1)

			'vHat_t = max(vHat_{t-1}, v_t)
			Transforms.max(vHat, v, False)

			Dim alphat As Double = learningRate * FastMath.sqrt(1 - beta2t) / (1 - beta1t)
			If Double.IsNaN(alphat) OrElse alphat = 0.0 Then
				alphat = epsilon
			End If

			'gradient array contains: sqrt(vHat) + eps
			Nd4j.Executioner.exec(New Sqrt(vHat, gradient)).addi(epsilon)

			'gradient = alphat * m_t / (sqrt(vHat) + eps)
			gradient.rdivi(m).muli(alphat)
		End Sub

		Public Shared Sub applyNadamUpdater(ByVal gradient As INDArray, ByVal m As INDArray, ByVal v As INDArray, ByVal learningRate As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)

			Dim oneMinusBeta1Grad As INDArray = gradient.mul(1.0 - beta1)
			m.muli(beta1).addi(oneMinusBeta1Grad)

			Dim oneMinusBeta2GradSquared As INDArray = gradient.mul(gradient).muli(1.0 - beta2)
			v.muli(beta2).addi(oneMinusBeta2GradSquared)

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)

			Dim biasCorrectedEstimateOfMomentum As INDArray = m.mul(beta1).divi(1.0 - beta1t)
			Dim secondTerm As INDArray = oneMinusBeta1Grad.divi(1 - beta1t)

			Dim alphat As INDArray = biasCorrectedEstimateOfMomentum.add(secondTerm).muli(learningRate)

			Dim sqrtV As INDArray = Transforms.sqrt(v.dup("c"c), False).addi(epsilon)

			gradient.assign(alphat).divi(sqrtV)
		End Sub

		Public Shared Sub applyNesterovsUpdater(ByVal gradient As INDArray, ByVal v As INDArray, ByVal lr As Double, ByVal momentum As Double)
			'reference https://cs231n.github.io/neural-networks-3/#sgd 2nd equation
			'DL4J default is negative step function thus we flipped the signs:
			' x += mu * v_prev + (-1 - mu) * v
			'i.e., we do params -= updatedGradient, not params += updatedGradient

			'v = mu * v - lr * gradient
			Dim vPrev As INDArray = v.dup("c"c)
			v.muli(momentum).subi(gradient.dup("c"c).muli(lr)) 'Modify state array in-place

	'        
	'        Next line is equivalent to:
	'        INDArray ret = vPrev.muli(momentum).addi(v.mul(-momentum - 1));
	'        gradient.assign(ret);
	'        
			Nd4j.Executioner.exec(New AddOp(vPrev.muli(momentum), v.mul(-momentum - 1), gradient))
		End Sub

		Public Shared Sub applyRmsProp(ByVal gradient As INDArray, ByVal lastGradient As INDArray, ByVal learningRate As Double, ByVal rmsDecay As Double, ByVal epsilon As Double)
			lastGradient.muli(rmsDecay).addi(gradient.mul(gradient).muli(1 - rmsDecay))
			' lr * gradient / (sqrt(cache) + 1e-8)
			gradient.muli(learningRate).divi(Transforms.sqrt(lastGradient.dup("c"c), False).addi(epsilon))
		End Sub

		Public Shared Sub applySgd(ByVal gradient As INDArray, ByVal lr As Double)
			gradient.muli(lr)
		End Sub
	End Class

End Namespace