Imports System
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

Namespace org.deeplearning4j.nn.conf.distribution

	Public Class Distributions
		Private Sub New()
		End Sub

		Public Shared Function createDistribution(ByVal dist As Distribution) As org.nd4j.linalg.api.rng.distribution.Distribution
			If dist Is Nothing Then
				Return Nothing
			End If
			If TypeOf dist Is NormalDistribution Then
				Dim nd As NormalDistribution = DirectCast(dist, NormalDistribution)
				Return Nd4j.Distributions.createNormal(nd.Mean, nd.Std)
			End If
			If TypeOf dist Is GaussianDistribution Then
				Dim nd As GaussianDistribution = DirectCast(dist, GaussianDistribution)
				Return Nd4j.Distributions.createNormal(nd.Mean, nd.Std)
			End If
			If TypeOf dist Is UniformDistribution Then
				Dim ud As UniformDistribution = DirectCast(dist, UniformDistribution)
				Return Nd4j.Distributions.createUniform(ud.getLower(), ud.getUpper())
			End If
			If TypeOf dist Is BinomialDistribution Then
				Dim bd As BinomialDistribution = DirectCast(dist, BinomialDistribution)
				Return Nd4j.Distributions.createBinomial(bd.NumberOfTrials, bd.ProbabilityOfSuccess)
			End If
			If TypeOf dist Is LogNormalDistribution Then
				Dim lnd As LogNormalDistribution = DirectCast(dist, LogNormalDistribution)
				Return Nd4j.Distributions.createLogNormal(lnd.getMean(), lnd.getStd())
			End If
			If TypeOf dist Is TruncatedNormalDistribution Then
				Dim tnd As TruncatedNormalDistribution = DirectCast(dist, TruncatedNormalDistribution)
				Return Nd4j.Distributions.createTruncatedNormal(tnd.getMean(), tnd.getStd())
			End If
			If TypeOf dist Is OrthogonalDistribution Then
				Dim od As OrthogonalDistribution = DirectCast(dist, OrthogonalDistribution)
				Return Nd4j.Distributions.createOrthogonal(od.getGain())
			End If
			If TypeOf dist Is ConstantDistribution Then
				Dim od As ConstantDistribution = DirectCast(dist, ConstantDistribution)
				Return Nd4j.Distributions.createConstant(od.getValue())
			End If
			Throw New Exception("unknown distribution type: " & dist.GetType())
		End Function
	End Class

End Namespace