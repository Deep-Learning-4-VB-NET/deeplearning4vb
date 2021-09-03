Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.nn.conf.layers.variational


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface ReconstructionDistribution extends java.io.Serializable
	Public Interface ReconstructionDistribution

		''' <summary>
		''' Does this reconstruction distribution has a standard neural network loss function (such as mean squared error,
		''' which is deterministic) or is it a standard VAE with a probabilistic reconstruction distribution? </summary>
		''' <returns> true if the reconstruction distribution has a loss function only (and no probabilistic reconstruction distribution) </returns>
		Function hasLossFunction() As Boolean

		''' <summary>
		''' Get the number of distribution parameters for the given input data size.
		''' For example, a Gaussian distribution has 2 parameters value (mean and log(variance)) for each data value,
		''' whereas a Bernoulli distribution has only 1 parameter value (probability) for each data value.
		''' </summary>
		''' <param name="dataSize"> Size of the data. i.e., nIn value </param>
		''' <returns> Number of distribution parameters for the given reconstruction distribution </returns>
		Function distributionInputSize(ByVal dataSize As Integer) As Integer

		''' <summary>
		''' Calculate the negative log probability (summed or averaged over each example in the minibatch)
		''' </summary>
		''' <param name="x">                        Data to be modelled (reconstructions) </param>
		''' <param name="preOutDistributionParams"> Distribution parameters used by this reconstruction distribution (for example,
		'''                                 mean and log variance values for Gaussian) </param>
		''' <param name="average">                  Whether the log probability should be averaged over the minibatch, or simply summed. </param>
		''' <returns> Average or sum of negative log probability of the reconstruction given the distribution parameters </returns>
		Function negLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray, ByVal average As Boolean) As Double

		''' <summary>
		''' Calculate the negative log probability for each example individually
		''' </summary>
		''' <param name="x">                        Data to be modelled (reconstructions) </param>
		''' <param name="preOutDistributionParams"> Distribution parameters used by this reconstruction distribution (for example,
		'''                                 mean and log variance values for Gaussian) - before applying activation function </param>
		''' <returns> Negative log probability of the reconstruction given the distribution parameters, for each example individually.
		''' Column vector, shape [numExamples, 1] </returns>
		Function exampleNegLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray

		''' <summary>
		''' Calculate the gradient of the negative log probability with respect to the preOutDistributionParams
		''' </summary>
		''' <param name="x">                        Data </param>
		''' <param name="preOutDistributionParams"> Distribution parameters used by this reconstruction distribution (for example,
		'''                                 mean and log variance values for Gaussian) - before applying activation function </param>
		''' <returns> Gradient with respect to the preOutDistributionParams </returns>
		Function gradient(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray

		''' <summary>
		''' Randomly sample from P(x|z) using the specified distribution parameters
		''' </summary>
		''' <param name="preOutDistributionParams"> Distribution parameters used by this reconstruction distribution (for example,
		'''                                 mean and log variance values for Gaussian) - before applying activation function </param>
		''' <returns> A random sample of x given the distribution parameters </returns>
		Function generateRandom(ByVal preOutDistributionParams As INDArray) As INDArray

		''' <summary>
		''' Generate a sample from P(x|z), where x = E[P(x|z)]
		''' i.e., return the mean value for the distribution
		''' </summary>
		''' <param name="preOutDistributionParams"> Distribution parameters used by this reconstruction distribution (for example,
		'''                                 mean and log variance values for Gaussian) - before applying activation function </param>
		''' <returns> A deterministic sample of x (mean/expected value) given the distribution parameters </returns>
		Function generateAtMean(ByVal preOutDistributionParams As INDArray) As INDArray
	End Interface

End Namespace