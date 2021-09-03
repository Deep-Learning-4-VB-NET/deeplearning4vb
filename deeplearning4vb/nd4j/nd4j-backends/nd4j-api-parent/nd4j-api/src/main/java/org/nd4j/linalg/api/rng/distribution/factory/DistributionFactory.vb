Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution

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

Namespace org.nd4j.linalg.api.rng.distribution.factory

	Public Interface DistributionFactory

		''' <summary>
		''' Create a distribution
		''' </summary>
		''' <param name="n"> the number of trials </param>
		''' <param name="p"> the probabilities </param>
		''' <returns> the biniomial distribution with the given parameters </returns>
		Function createBinomial(ByVal n As Integer, ByVal p As INDArray) As Distribution

		''' <summary>
		''' Create a distribution
		''' </summary>
		''' <param name="n"> the number of trials </param>
		''' <param name="p"> the probabilities </param>
		''' <returns> the biniomial distribution with the given parameters </returns>
		Function createBinomial(ByVal n As Integer, ByVal p As Double) As Distribution

		''' <summary>
		''' Create  a normal distribution
		''' with the given mean and std
		''' </summary>
		''' <param name="mean"> the mean </param>
		''' <param name="std">  the standard deviation </param>
		''' <returns> the distribution with the given
		''' mean and standard deviation </returns>
		Function createNormal(ByVal mean As INDArray, ByVal std As Double) As Distribution

		''' <summary>
		''' Create  a normal distribution
		''' with the given mean and std
		''' </summary>
		''' <param name="mean"> the mean </param>
		''' <param name="std">  the stnadard deviation </param>
		''' <returns> the distribution with the given
		''' mean and standard deviation </returns>
		Function createNormal(ByVal mean As Double, ByVal std As Double) As Distribution

		''' <summary>
		''' Create a uniform distribution with the
		''' given min and max
		''' </summary>
		''' <param name="min"> the min </param>
		''' <param name="max"> the max </param>
		''' <returns> the uniform distribution </returns>
		Function createUniform(ByVal min As Double, ByVal max As Double) As Distribution

		''' <summary>
		''' Creates a log-normal distribution
		''' </summary>
		''' <param name="mean"> </param>
		''' <param name="std">
		''' @return </param>
		Function createLogNormal(ByVal mean As Double, ByVal std As Double) As Distribution

		''' <summary>
		''' Creates truncated normal distribution
		''' </summary>
		''' <param name="mean"> </param>
		''' <param name="std">
		''' @return </param>
		Function createTruncatedNormal(ByVal mean As Double, ByVal std As Double) As Distribution

		''' <summary>
		''' Creates orthogonal distribution
		''' </summary>
		''' <param name="gain">
		''' @return </param>
		Function createOrthogonal(ByVal gain As Double) As Distribution

		''' <summary>
		''' Creates constant distribution
		''' </summary>
		''' <param name="value">
		''' @return </param>
		Function createConstant(ByVal value As Double) As Distribution
	End Interface

End Namespace