Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.nd4j.linalg.lossfunctions.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @JsonInclude(JsonInclude.Include.NON_NULL) public class LossMixtureDensity implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossMixtureDensity
		Implements ILossFunction

		Private mMixtures As Integer
		Private mLabelWidth As Integer
		Private Shared ReadOnly SQRT_TWO_PI As Double = Math.Sqrt(2 * Math.PI)

		Public Sub New()
		End Sub

		''' <summary>
		''' This method constructs a mixture density cost function
		''' which causes the network to learn a mixture of gaussian distributions
		''' for each network output.  The network will learn the 'alpha' (weight
		''' for each distribution), the 'mu' or 'mean' of each distribution,
		''' and the 'sigma' (standard-deviation) of the mixture.  Together,
		''' this distribution can be sampled according to the probability density
		''' learned by the network.
		''' </summary>
		''' <param name="mixtures"> Number of gaussian mixtures to model. </param>
		''' <param name="labelWidth"> Size of the labels vector for each sample. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private LossMixtureDensity(@JsonProperty("mixtures") int mixtures, @JsonProperty("labelWidth") int labelWidth)
		Private Sub New(ByVal mixtures As Integer, ByVal labelWidth As Integer)
			mMixtures = mixtures
			mLabelWidth = labelWidth
		End Sub

		''' <summary>
		''' This class is a data holder for the mixture density
		''' components for convenient manipulation.
		''' These are organized as rank-3 matrices with shape
		''' [nSamples, nLabelsPerSample, nMixturesPerLabel]
		''' and refer to the 'alpha' (weight of that gaussian), 'mu' (mean for that
		''' gaussian), and 'sigma' (standard-deviation for that gaussian).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class MixtureDensityComponents
		Public Class MixtureDensityComponents
			Friend alpha As INDArray
			Friend mu As INDArray
			Friend sigma As INDArray
		End Class

		' This method extracts the "alpha", "mu", and "sigma" from the
		' output of the neural network.
		' This is done manually, but it should ultimately be done
		' through Nd4j operations in order to increase performance.
		Public Overridable Function extractComponents(ByVal output As INDArray) As MixtureDensityComponents
			Dim outputSize As Long = output.size(1)
			If outputSize <> (mLabelWidth + 2) * mMixtures Then
				Throw New System.ArgumentException("Network output size " & outputSize & " must be (labels+2)*mixtures where labels = " & mLabelWidth & " and mixtures = " & mMixtures)
			End If

			Dim mdc As New MixtureDensityComponents()

			' Output is 2 dimensional (samples, labels)
			'
			' For each label vector of length 'labels', we will have
			' an output vector of length '(labels + 2) * nMixtures.
			' The first nMixtures outputs will correspond to the 'alpha' for each mixture.
			' The second nMixtures outputs will correspond to the 'sigma' and the last nMixtures*labels
			' will correspond to the 'mu' (mean) of the output.

			' Reorganize these.
			' alpha = samples, 0 to nMixtures
			' mu = samples, nMixtures to 2*nMixtures
			' sigma = samples, 2*nMixtures to (labels + 2)*nMixtures
			' Alpha is then sub-divided through reshape by mixtures per label and samples.

			mdc.alpha = output.get(NDArrayIndex.all(), NDArrayIndex.interval(0, mMixtures))
			mdc.sigma = output.get(NDArrayIndex.all(), NDArrayIndex.interval(mMixtures, 2 * mMixtures))
			mdc.mu = output.get(NDArrayIndex.all(), NDArrayIndex.interval(2 * mMixtures, (mLabelWidth + 2) * mMixtures)).reshape(output.size(0), mMixtures, mLabelWidth)

			' Alpha is a softmax because
			' the alpha should all sum to 1 for a given gaussian mixture.
			mdc.alpha = Nd4j.exec(DirectCast(New SoftMax(mdc.alpha, mdc.alpha, -1), CustomOp))(0)

			' Mu comes directly from the network as an unmolested value.
			' Note that this effectively means that the output layer of
			' the network should have an activation function at least as large as
			' the expected values.  It is best for the output
			' layer to be an IDENTITY activation function.
			'mdc.mu = mdc.mu;

			' Sigma comes from the network as an exponential in order to
			' ensure that it is positive-definite.
			mdc.sigma = Transforms.exp(mdc.sigma)

			Return mdc
		End Function

		''' <summary>
		''' Computes the aggregate score as a sum of all of the individual scores of
		''' each of the labels against each of the outputs of the network.  For
		''' the mixture density network, this is the negative log likelihood that
		''' the given labels fall within the probability distribution described by
		''' the mixture of gaussians of the network output. </summary>
		''' <param name="labels"> Labels to score against the network. </param>
		''' <param name="preOutput"> Output of the network (before activation function has been called). </param>
		''' <param name="activationFn"> Activation function for the network. </param>
		''' <param name="mask"> Mask to be applied to labels (not used for MDN). </param>
		''' <param name="average"> Whether or not to return an average instead of a total score (not used). </param>
		''' <returns> Returns a single double which corresponds to the total score of all label values. </returns>
		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			' The score overall consists of the
			' sum of the negative log likelihoods for each
			' of the individual labels.
			Dim scoreArr As INDArray = computeScoreArray(labels, preOutput, activationFn, mask)
			Dim score As Double = scoreArr.sumNumber().doubleValue()
			If average Then
				score /= scoreArr.size(0)
			End If
			Return score
		End Function

		''' <summary>
		''' This method returns the score for each of the given outputs against the
		''' given set of labels.  For a mixture density network, this is done by
		''' extracting the "alpha", "mu", and "sigma" components of each gaussian
		''' and computing the negative log likelihood that the labels fall within
		''' a linear combination of these gaussian distributions.  The smaller
		''' the negative log likelihood, the higher the probability that the given
		''' labels actually would fall within the distribution.  Therefore by
		''' minimizing the negative log likelihood, we get to a position of highest
		''' probability that the gaussian mixture explains the phenomenon.
		''' </summary>
		''' <param name="labels"> Labels give the sample output that the network should
		'''               be trying to converge on. </param>
		''' <param name="preOutput"> The output of the last layer (before applying the activation function). </param>
		''' <param name="activationFn"> The activation function of the current layer. </param>
		''' <param name="mask"> Mask to apply to score evaluation (not supported for this cost function). </param>
		''' <returns>  </returns>
		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), False)
			Dim mdc As MixtureDensityComponents = extractComponents(output)
			Dim scoreArr As INDArray = negativeLogLikelihood(labels, mdc.alpha, mdc.mu, mdc.sigma)

			If mask IsNot Nothing Then
				LossUtil.applyMask(scoreArr, mask)
			End If

			Return scoreArr
		End Function

		''' <summary>
		''' This method returns the gradient of the cost function with respect to the
		''' output from the previous layer.  For this cost function, the gradient
		''' is derived from Bishop's paper "Mixture Density Networks" (1994) which
		''' gives an elegant closed-form expression for the derivatives with respect
		''' to each of the output components. </summary>
		''' <param name="labels"> Labels to train on. </param>
		''' <param name="preOutput"> Output of neural network before applying the final activation function. </param>
		''' <param name="activationFn"> Activation function of output layer. </param>
		''' <param name="mask"> Mask to apply to gradients. </param>
		''' <returns> Gradient of cost function with respect to preOutput parameters. </returns>
		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim nSamples As Long = labels.size(0)

			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), False)

			Dim mdc As MixtureDensityComponents = extractComponents(output)

			Dim gradient As INDArray = Nd4j.zeros(nSamples, preOutput.columns())

			Dim labelsMinusMu As INDArray = Me.labelsMinusMu(labels, mdc.mu)
			Dim labelsMinusMuSquared As INDArray = labelsMinusMu.mul(labelsMinusMu).sum(2)

			' This computes pi_i, see Bishop equation (30).
			' See http://www.plsyard.com/dealing-overflow-and-underflow-in-softmax-function/
			' this post for why we calculate the pi_i in this way.
			' With the exponential function here, we have to be very careful
			' about overflow/underflow considerations even with
			' fairly intermediate values.  Subtracting the max
			' here helps to ensure over/underflow does not happen here.
			' This isn't exactly a softmax because there's an 'alpha' coefficient
			' here, but the technique works, nonetheless.
			Dim variance As INDArray = mdc.sigma.mul(mdc.sigma)
			Dim minustwovariance As INDArray = variance.mul(2).negi()
			Dim normalPart As INDArray = mdc.alpha.div(Transforms.pow(mdc.sigma.mul(SQRT_TWO_PI), mLabelWidth))
			Dim exponent As INDArray = labelsMinusMuSquared.div(minustwovariance)
			Dim exponentMax As INDArray = exponent.max(1)
			exponent.subiColumnVector(exponentMax)
			Dim pi As INDArray = Transforms.exp(exponent).muli(normalPart)
			Dim piDivisor As INDArray = pi.sum(True,1)
			pi.diviColumnVector(piDivisor)

			' See Bishop equation (35)
			'INDArray dLdZAlpha = Nd4j.zeros(nSamples, nLabelsPerSample, mMixturesPerLabel); //mdc.alpha.sub(pi);
			Dim dLdZAlpha As INDArray = mdc.alpha.sub(pi)
			' See Bishop equation (38)
			Dim dLdZSigma As INDArray = (labelsMinusMuSquared.div(variance).subi(mLabelWidth)).muli(-1).muli(pi)
			' See Bishop equation (39)

			' This turned out to be way less efficient than
			' the simple 'for' loop here.
			'INDArray dLdZMu = pi
			'        .div(variance)
			'        .reshape(nSamples, mMixtures, 1)
			'        .repeat(2, mLabelWidth)
			'        .muli(labelsMinusMu)
			'        .negi()
			'        .reshape(nSamples, mMixtures * mLabelWidth);

			Dim dLdZMu As INDArray = Nd4j.create(nSamples, mMixtures, mLabelWidth)
			For k As Integer = 0 To mLabelWidth - 1
				dLdZMu.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(k)}, labelsMinusMu.get(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(k)}).muli(pi).divi(variance).negi())
			Next k
			dLdZMu = dLdZMu.reshape(ChrW(nSamples), mMixtures * mLabelWidth)

			' Place components of gradient into gradient holder.
			gradient.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, mMixtures)}, dLdZAlpha)
			gradient.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(mMixtures, mMixtures * 2)}, dLdZSigma)
			gradient.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(mMixtures * 2, (mLabelWidth + 2) * mMixtures)}, dLdZMu)

			Dim gradients As INDArray = activationFn.backprop(preOutput, gradient).First

			If mask IsNot Nothing Then
				LossUtil.applyMask(gradients, mask)
			End If

			Return gradients
		End Function

		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
			Dim score As Double = computeScore(labels, preOutput, activationFn, mask, average)
			Dim gradient As INDArray = computeGradient(labels, preOutput, activationFn, mask)
			Dim returnCode As New Pair(Of Double, INDArray)(score, gradient)
			Return returnCode
		End Function

		''' <summary>
		''' The opName of this function
		''' 
		''' @return
		''' </summary>
		Public Overridable Function name() As String Implements ILossFunction.name
			Return "lossmixturedensity"
		End Function

		''' <summary>
		''' This method returns an array consisting of each of the training samples,
		''' for each label in each sample, the negative log likelihood of that
		''' value falling within the given gaussian mixtures. </summary>
		''' <param name="alpha"> </param>
		''' <param name="mu"> </param>
		''' <param name="sigma"> </param>
		''' <param name="labels"> </param>
		''' <returns>  </returns>
		Private Function negativeLogLikelihood(ByVal labels As INDArray, ByVal alpha As INDArray, ByVal mu As INDArray, ByVal sigma As INDArray) As INDArray
			Dim labelsMinusMu As INDArray = Me.labelsMinusMu(labels, mu)
			Dim diffsquared As INDArray = labelsMinusMu.mul(labelsMinusMu).sum(2)
			Dim phitimesalphasum As INDArray = phi(diffsquared, sigma).muli(alpha).sum(True,1)

			' result = See Bishop(28,29)
			Dim result As INDArray = Transforms.log(phitimesalphasum).negi()
			Return result
		End Function

		Private Function labelsMinusMu(ByVal labels As INDArray, ByVal mu As INDArray) As INDArray
			' Now that we have the mixtures, let's compute the negative
			' log likelihodd of the label against the 
			Dim nSamples As Long = labels.size(0)
			Dim labelsPerSample As Long = labels.size(1)

			' This worked, but was actually much
			' slower than the for loop below.
			' labels = samples, mixtures, labels
			' mu = samples, mixtures
			' INDArray labelMinusMu = labels
			'        .reshape('f', nSamples, labelsPerSample, 1)
			'        .repeat(2, mMixtures)
			'        .permute(0, 2, 1)
			'        .subi(mu);

			' The above code does the same thing as the loop below,
			' but it does it with index magix instead of a for loop.
			' It turned out to be way less efficient than the simple 'for' here.
			Dim labelMinusMu As INDArray = Nd4j.zeros(nSamples, mMixtures, labelsPerSample)
			For k As Integer = 0 To mMixtures - 1
				labelMinusMu.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.point(k), NDArrayIndex.all()}, labels)
			Next k
			labelMinusMu.subi(mu)

			Return labelMinusMu
		End Function

		''' <summary>
		''' This method calculates 'phi' which is the probability
		''' density function (see Bishop 23) </summary>
		''' <param name="diffSquared"> This is the 'x-mu' term of the Gaussian distribution (distance between 'x' and the mean value of the distribution). </param>
		''' <param name="sigma"> This is the standard deviation of the Gaussian distribution. </param>
		''' <returns> This returns an array of shape [nsamples, nlabels, ndistributions] which contains the probability density (phi) for each of the
		'''         samples * labels * distributions for the given x, sigma, mu. </returns>
		Private Function phi(ByVal diffSquared As INDArray, ByVal sigma As INDArray) As INDArray
			' 1/sqrt(2PIs^2) * e^((in-u)^2/2*s^2)
			Dim minustwovariance As INDArray = sigma.mul(sigma).muli(2).negi()

			' This is phi_i(x,mu,sigma)
			Dim likelihoods As INDArray = Transforms.exp(diffSquared.divi(minustwovariance)).divi(Transforms.pow(sigma.mul(SQRT_TWO_PI), CDbl(mLabelWidth)))

			Return likelihoods
		End Function

		''' <summary>
		''' Returns the number of gaussians this loss function
		''' will attempt to find. </summary>
		''' <returns> Number of gaussians to find. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty("mixtures") public int getNMixtures()
		Public Overridable ReadOnly Property NMixtures As Integer
			Get
				Return mMixtures
			End Get
		End Property

		''' <summary>
		''' Returns the width of each label vector. </summary>
		''' <returns> Width of label vectors expected. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty("labelWidth") public int getLabelWidth()
		Public Overridable ReadOnly Property LabelWidth As Integer
			Get
				Return mLabelWidth
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "LossMixtureDensity(mixtures=" & mMixtures & ", labels=" & mLabelWidth & ")"
		End Function

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder
			Friend mGaussians As Integer = 0
			Friend mLabelWidth As Integer = 0

			Friend Sub New()
			End Sub

			''' <summary>
			''' Specifies the number of gaussian functions to attempt
			''' fitting against the data. </summary>
			''' <param name="aGaussians"> Number of gaussian functions to fit. </param>
			''' <returns> DynamicCustomOpsBuilder. </returns>
			Public Overridable Function gaussians(ByVal aGaussians As Integer) As Builder
				mGaussians = aGaussians
				Return Me
			End Function

			''' <summary>
			''' Specifies the width of the labels vector which also corresponds
			''' to the width of the 'mean' vector for each of the gaussian functions. </summary>
			''' <param name="aLabelWidth"> Width of the labels vector. </param>
			''' <returns> DynamicCustomOpsBuilder. </returns>
			Public Overridable Function labelWidth(ByVal aLabelWidth As Integer) As Builder
				mLabelWidth = aLabelWidth
				Return Me
			End Function

			''' <summary>
			''' Creates a new instance of the mixture density
			''' cost function. </summary>
			''' <returns> A new mixture density cost function built with
			'''         the specified parameters. </returns>
			Public Overridable Function build() As LossMixtureDensity
				If mGaussians <= 0 Then
					Throw New System.ArgumentException("Mixture density cost function must specify the number of mixtures to fit")
				End If
				If mLabelWidth <= 0 Then
					Throw New System.ArgumentException("Mixture density cost function must specify the size of the labels vectors")
				End If
				Return New LossMixtureDensity(mGaussians, mLabelWidth)
			End Function
		End Class
	End Class

End Namespace