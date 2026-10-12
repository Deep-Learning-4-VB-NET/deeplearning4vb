Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.impl.nlp

	Public Class CbowRound
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		''' <summary>
		''' hs round
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="context"> </param>
		''' <param name="syn0"> </param>
		''' <param name="syn1"> </param>
		''' <param name="expTable"> </param>
		''' <param name="alpha"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="inferenceVector"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CbowRound(int target, @NonNull int[] context, @NonNull int[] lockedWords, @NonNull INDArray syn0, @NonNull INDArray syn1, @NonNull INDArray expTable, @NonNull int[] indices, @NonNull byte[] codes, double alpha, long nextRandom, @NonNull INDArray inferenceVector, int numLabels)
		Public Sub New(ByVal target As Integer, ByVal context() As Integer, ByVal lockedWords() As Integer, ByVal syn0 As INDArray, ByVal syn1 As INDArray, ByVal expTable As INDArray, ByVal indices() As Integer, ByVal codes() As SByte, ByVal alpha As Double, ByVal nextRandom As Long, ByVal inferenceVector As INDArray, ByVal numLabels As Integer)
			Me.New(Nd4j.scalar(target), Nd4j.createFromArray(context), Nd4j.createFromArray(lockedWords), Nd4j.empty(DataType.INT), syn0, syn1, Nd4j.empty(syn1.dataType()), expTable, Nd4j.empty(syn1.dataType()), Nd4j.createFromArray(indices), Nd4j.createFromArray(codes), 0, Nd4j.scalar(alpha), Nd4j.scalar(nextRandom), inferenceVector, Nd4j.scalar(numLabels), inferenceVector.isEmpty(), 1)
		End Sub

		''' <summary>
		''' ns round
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="context"> </param>
		''' <param name="ngStarter"> </param>
		''' <param name="syn0"> </param>
		''' <param name="syn1Neg"> </param>
		''' <param name="expTable"> </param>
		''' <param name="negTable"> </param>
		''' <param name="alpha"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="inferenceVector"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CbowRound(int target, @NonNull int[] context, @NonNull int[] lockedWords, int ngStarter, @NonNull INDArray syn0, @NonNull INDArray syn1Neg, @NonNull INDArray expTable, @NonNull INDArray negTable, int nsRounds, double alpha, long nextRandom, @NonNull INDArray inferenceVector, int numLabels)
		Public Sub New(ByVal target As Integer, ByVal context() As Integer, ByVal lockedWords() As Integer, ByVal ngStarter As Integer, ByVal syn0 As INDArray, ByVal syn1Neg As INDArray, ByVal expTable As INDArray, ByVal negTable As INDArray, ByVal nsRounds As Integer, ByVal alpha As Double, ByVal nextRandom As Long, ByVal inferenceVector As INDArray, ByVal numLabels As Integer)
			Me.New(Nd4j.scalar(target), Nd4j.createFromArray(context), Nd4j.createFromArray(lockedWords), Nd4j.scalar(ngStarter), syn0, Nd4j.empty(syn0.dataType()), syn1Neg, expTable, negTable, Nd4j.empty(DataType.INT), Nd4j.empty(DataType.BYTE), nsRounds, Nd4j.scalar(alpha), Nd4j.scalar(nextRandom), inferenceVector, Nd4j.scalar(numLabels), inferenceVector.isEmpty(), 1)
		End Sub

		''' <summary>
		''' full constructor
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="context"> </param>
		''' <param name="ngStarter"> </param>
		''' <param name="syn0"> </param>
		''' <param name="syn1"> </param>
		''' <param name="syn1Neg"> </param>
		''' <param name="expTable"> </param>
		''' <param name="negTable"> </param>
		''' <param name="alpha"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="inferenceVector"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CbowRound(@NonNull INDArray target, @NonNull INDArray context, @NonNull INDArray lockedWords, @NonNull INDArray ngStarter, @NonNull INDArray syn0, @NonNull INDArray syn1, @NonNull INDArray syn1Neg, @NonNull INDArray expTable, @NonNull INDArray negTable, @NonNull INDArray indices, @NonNull INDArray codes, int nsRounds, @NonNull INDArray alpha, @NonNull INDArray nextRandom, @NonNull INDArray inferenceVector, @NonNull INDArray numLabels, boolean trainWords, int numWorkers)
		Public Sub New(ByVal target As INDArray, ByVal context As INDArray, ByVal lockedWords As INDArray, ByVal ngStarter As INDArray, ByVal syn0 As INDArray, ByVal syn1 As INDArray, ByVal syn1Neg As INDArray, ByVal expTable As INDArray, ByVal negTable As INDArray, ByVal indices As INDArray, ByVal codes As INDArray, ByVal nsRounds As Integer, ByVal alpha As INDArray, ByVal nextRandom As INDArray, ByVal inferenceVector As INDArray, ByVal numLabels As INDArray, ByVal trainWords As Boolean, ByVal numWorkers As Integer)

			inputArguments_Conflict.Add(target)
			inputArguments_Conflict.Add(ngStarter)
			inputArguments_Conflict.Add(context)
			inputArguments_Conflict.Add(indices)
			inputArguments_Conflict.Add(codes)
			inputArguments_Conflict.Add(syn0)
			inputArguments_Conflict.Add(syn1)
			inputArguments_Conflict.Add(syn1Neg)
			inputArguments_Conflict.Add(expTable)
			inputArguments_Conflict.Add(negTable)
			inputArguments_Conflict.Add(alpha)
			inputArguments_Conflict.Add(nextRandom)
			inputArguments_Conflict.Add(numLabels)
			inputArguments_Conflict.Add(lockedWords)
			inputArguments_Conflict.Add(inferenceVector)

			' couple of options
			iArguments.Add(CLng(numWorkers))
			iArguments.Add(CLng(nsRounds))


			bArguments.Add(trainWords)
			bArguments.Add(Not inferenceVector.isEmpty())

			' this op is always inplace
			setInPlace(True)
			setInplaceCall(True)

			For Each [in] As val In inputArguments_Conflict
				outputArguments_Conflict.Add([in])
			Next [in]
		End Sub

		Public Overrides Function opName() As String
			Return "cbow"
		End Function
	End Class

End Namespace