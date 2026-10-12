Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports org.nd4j.linalg.api.rng.distribution.impl

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

	Public Class DefaultDistributionFactory
		Implements DistributionFactory

		Public Overridable Function createBinomial(ByVal n As Integer, ByVal p As INDArray) As Distribution Implements DistributionFactory.createBinomial
			Return New BinomialDistribution(n, p)
		End Function

		Public Overridable Function createBinomial(ByVal n As Integer, ByVal p As Double) As Distribution Implements DistributionFactory.createBinomial
			Return New BinomialDistribution(n, p)
		End Function

		Public Overridable Function createNormal(ByVal mean As INDArray, ByVal std As Double) As Distribution Implements DistributionFactory.createNormal
			Return New NormalDistribution(mean, std)
		End Function

		Public Overridable Function createNormal(ByVal mean As Double, ByVal std As Double) As Distribution Implements DistributionFactory.createNormal
			Return New NormalDistribution(mean, std)
		End Function

		Public Overridable Function createLogNormal(ByVal mean As Double, ByVal std As Double) As Distribution Implements DistributionFactory.createLogNormal
			Return New LogNormalDistribution(mean, std)
		End Function

		Public Overridable Function createTruncatedNormal(ByVal mean As Double, ByVal std As Double) As Distribution Implements DistributionFactory.createTruncatedNormal
			Return New TruncatedNormalDistribution(mean, std)
		End Function

		Public Overridable Function createOrthogonal(ByVal gain As Double) As Distribution Implements DistributionFactory.createOrthogonal
			Return New OrthogonalDistribution(gain)
		End Function

		Public Overridable Function createConstant(ByVal value As Double) As Distribution Implements DistributionFactory.createConstant
			Return New ConstantDistribution(value)
		End Function

		Public Overridable Function createUniform(ByVal min As Double, ByVal max As Double) As Distribution Implements DistributionFactory.createUniform
			Return New UniformDistribution(min, max)
		End Function
	End Class

End Namespace