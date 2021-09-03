Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
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

Namespace org.deeplearning4j.nn.conf.layers.variational


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class CompositeReconstructionDistribution implements ReconstructionDistribution
	<Serializable>
	Public Class CompositeReconstructionDistribution
		Implements ReconstructionDistribution

		Private ReadOnly distributionSizes() As Integer
		Private ReadOnly reconstructionDistributions() As ReconstructionDistribution
		Private ReadOnly totalSize As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CompositeReconstructionDistribution(@JsonProperty("distributionSizes") int[] distributionSizes, @JsonProperty("reconstructionDistributions") ReconstructionDistribution[] reconstructionDistributions, @JsonProperty("totalSize") int totalSize)
		Public Sub New(ByVal distributionSizes() As Integer, ByVal reconstructionDistributions() As ReconstructionDistribution, ByVal totalSize As Integer)
			Me.distributionSizes = distributionSizes
			Me.reconstructionDistributions = reconstructionDistributions
			Me.totalSize = totalSize
		End Sub

		Private Sub New(ByVal builder As Builder)
			distributionSizes = New Integer(builder.distributionSizes.Count - 1){}
			reconstructionDistributions = New ReconstructionDistribution(distributionSizes.Length - 1){}
			Dim sizeCount As Integer = 0
			For i As Integer = 0 To distributionSizes.Length - 1
				distributionSizes(i) = builder.distributionSizes(i)
				reconstructionDistributions(i) = builder.reconstructionDistributions(i)
				sizeCount += distributionSizes(i)
			Next i
			totalSize = sizeCount
		End Sub

		Public Overridable Function computeLossFunctionScoreArray(ByVal data As INDArray, ByVal reconstruction As INDArray) As INDArray
			If Not hasLossFunction() Then
				Throw New System.InvalidOperationException("Cannot compute score array unless hasLossFunction() == true")
			End If

			'Sum the scores from each loss function...
			Dim inputSoFar As Integer = 0
			Dim paramsSoFar As Integer = 0
			Dim reconstructionScores As INDArray = Nothing
			For i As Integer = 0 To distributionSizes.Length - 1
				Dim thisInputSize As Integer = distributionSizes(i)
				Dim thisParamsSize As Integer = reconstructionDistributions(i).distributionInputSize(thisInputSize)


				Dim dataSubset As INDArray = data.get(NDArrayIndex.all(), NDArrayIndex.interval(inputSoFar, inputSoFar + thisInputSize))
				Dim reconstructionSubset As INDArray = reconstruction.get(NDArrayIndex.all(), NDArrayIndex.interval(paramsSoFar, paramsSoFar + thisParamsSize))

				If i = 0 Then
					reconstructionScores = getScoreArray(reconstructionDistributions(i), dataSubset, reconstructionSubset)
				Else
					reconstructionScores.addi(getScoreArray(reconstructionDistributions(i), dataSubset, reconstructionSubset))
				End If

				inputSoFar += thisInputSize
				paramsSoFar += thisParamsSize
			Next i

			Return reconstructionScores
		End Function

		Private Function getScoreArray(ByVal reconstructionDistribution As ReconstructionDistribution, ByVal dataSubset As INDArray, ByVal reconstructionSubset As INDArray) As INDArray
			If TypeOf reconstructionDistribution Is LossFunctionWrapper Then
				Dim lossFunction As ILossFunction = DirectCast(reconstructionDistribution, LossFunctionWrapper).getLossFunction()
				'Re: the activation identity here - the reconstruction array already has the activation function applied,
				' so we don't want to apply it again. i.e., we are passing the output, not the pre-output.
				Return lossFunction.computeScoreArray(dataSubset, reconstructionSubset, New ActivationIdentity(), Nothing)
			ElseIf TypeOf reconstructionDistribution Is CompositeReconstructionDistribution Then
				Return DirectCast(reconstructionDistribution, CompositeReconstructionDistribution).computeLossFunctionScoreArray(dataSubset, reconstructionSubset)
			Else
				Throw New System.NotSupportedException("Cannot calculate composite reconstruction distribution")
			End If
		End Function

		Public Overridable Function hasLossFunction() As Boolean Implements ReconstructionDistribution.hasLossFunction
			For Each rd As ReconstructionDistribution In reconstructionDistributions
				If Not rd.hasLossFunction() Then
					Return False
				End If
			Next rd
			Return True
		End Function

		Public Overridable Function distributionInputSize(ByVal dataSize As Integer) As Integer Implements ReconstructionDistribution.distributionInputSize
			If dataSize <> totalSize Then
				Throw New System.InvalidOperationException("Invalid input size: Got input size " & dataSize & " for data, but expected input" & " size for all distributions is " & totalSize & ". Distribution sizes: " & Arrays.toString(distributionSizes))
			End If

			Dim sum As Integer = 0
			For i As Integer = 0 To distributionSizes.Length - 1
				sum += reconstructionDistributions(i).distributionInputSize(distributionSizes(i))
			Next i

			Return sum
		End Function

		Public Overridable Function negLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray, ByVal average As Boolean) As Double Implements ReconstructionDistribution.negLogProbability

			Dim inputSoFar As Integer = 0
			Dim paramsSoFar As Integer = 0
			Dim logProbSum As Double = 0.0
			For i As Integer = 0 To distributionSizes.Length - 1
				Dim thisInputSize As Integer = distributionSizes(i)
				Dim thisParamsSize As Integer = reconstructionDistributions(i).distributionInputSize(thisInputSize)


				Dim inputSubset As INDArray = x.get(NDArrayIndex.all(), NDArrayIndex.interval(inputSoFar, inputSoFar + thisInputSize))
				Dim paramsSubset As INDArray = preOutDistributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(paramsSoFar, paramsSoFar + thisParamsSize))

				logProbSum += reconstructionDistributions(i).negLogProbability(inputSubset, paramsSubset, average)

				inputSoFar += thisInputSize
				paramsSoFar += thisParamsSize
			Next i

			Return logProbSum
		End Function

		Public Overridable Function exampleNegLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.exampleNegLogProbability

			Dim inputSoFar As Integer = 0
			Dim paramsSoFar As Integer = 0
			Dim exampleLogProbSum As INDArray = Nothing
			For i As Integer = 0 To distributionSizes.Length - 1
				Dim thisInputSize As Integer = distributionSizes(i)
				Dim thisParamsSize As Integer = reconstructionDistributions(i).distributionInputSize(thisInputSize)


				Dim inputSubset As INDArray = x.get(NDArrayIndex.all(), NDArrayIndex.interval(inputSoFar, inputSoFar + thisInputSize))
				Dim paramsSubset As INDArray = preOutDistributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(paramsSoFar, paramsSoFar + thisParamsSize))

				If i = 0 Then
					exampleLogProbSum = reconstructionDistributions(i).exampleNegLogProbability(inputSubset, paramsSubset)
				Else
					exampleLogProbSum.addi(reconstructionDistributions(i).exampleNegLogProbability(inputSubset, paramsSubset))
				End If

				inputSoFar += thisInputSize
				paramsSoFar += thisParamsSize
			Next i

			Return exampleLogProbSum
		End Function

		Public Overridable Function gradient(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.gradient
			Dim inputSoFar As Integer = 0
			Dim paramsSoFar As Integer = 0
'JAVA TO VB CONVERTER NOTE: The local variable gradient was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim gradient_Conflict As INDArray = preOutDistributionParams.ulike()
			For i As Integer = 0 To distributionSizes.Length - 1
				Dim thisInputSize As Integer = distributionSizes(i)
				Dim thisParamsSize As Integer = reconstructionDistributions(i).distributionInputSize(thisInputSize)


				Dim inputSubset As INDArray = x.get(NDArrayIndex.all(), NDArrayIndex.interval(inputSoFar, inputSoFar + thisInputSize))
				Dim paramsSubset As INDArray = preOutDistributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(paramsSoFar, paramsSoFar + thisParamsSize))

				Dim grad As INDArray = reconstructionDistributions(i).gradient(inputSubset, paramsSubset)
				gradient_Conflict.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(paramsSoFar, paramsSoFar + thisParamsSize)}, grad)

				inputSoFar += thisInputSize
				paramsSoFar += thisParamsSize
			Next i

			Return gradient_Conflict
		End Function

		Public Overridable Function generateRandom(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateRandom
			Return randomSample(preOutDistributionParams, False)
		End Function

		Public Overridable Function generateAtMean(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateAtMean
			Return randomSample(preOutDistributionParams, True)
		End Function

		Private Function randomSample(ByVal preOutDistributionParams As INDArray, ByVal isMean As Boolean) As INDArray
			Dim inputSoFar As Integer = 0
			Dim paramsSoFar As Integer = 0
			Dim [out] As INDArray = Nd4j.createUninitialized(preOutDistributionParams.dataType(), New Long() {preOutDistributionParams.size(0), totalSize})
			For i As Integer = 0 To distributionSizes.Length - 1
				Dim thisDataSize As Integer = distributionSizes(i)
				Dim thisParamsSize As Integer = reconstructionDistributions(i).distributionInputSize(thisDataSize)

				Dim paramsSubset As INDArray = preOutDistributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(paramsSoFar, paramsSoFar + thisParamsSize))

				Dim thisRandomSample As INDArray
				If isMean Then
					thisRandomSample = reconstructionDistributions(i).generateAtMean(paramsSubset)
				Else
					thisRandomSample = reconstructionDistributions(i).generateRandom(paramsSubset)
				End If

				[out].put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(inputSoFar, inputSoFar + thisDataSize)}, thisRandomSample)

				inputSoFar += thisDataSize
				paramsSoFar += thisParamsSize
			Next i

			Return [out]
		End Function

		Public Class Builder

			Friend distributionSizes As IList(Of Integer) = New List(Of Integer)()
			Friend reconstructionDistributions As IList(Of ReconstructionDistribution) = New List(Of ReconstructionDistribution)()

			''' <summary>
			''' Add another distribution to the composite distribution. This will add the distribution for the next 'distributionSize'
			''' values, after any previously added.
			''' For example, calling addDistribution(10, X) once will result in values 0 to 9 (inclusive) being modelled
			''' by the specified distribution X. Calling addDistribution(10, Y) after that will result in values 10 to 19 (inclusive)
			''' being modelled by distribution Y.
			''' </summary>
			''' <param name="distributionSize">    Number of values to model with the specified distribution </param>
			''' <param name="distribution">        Distribution to model data with </param>
			Public Overridable Function addDistribution(ByVal distributionSize As Integer, ByVal distribution As ReconstructionDistribution) As Builder
				distributionSizes.Add(distributionSize)
				reconstructionDistributions.Add(distribution)
				Return Me
			End Function

			Public Overridable Function build() As CompositeReconstructionDistribution
				Return New CompositeReconstructionDistribution(Me)
			End Function
		End Class
	End Class

End Namespace