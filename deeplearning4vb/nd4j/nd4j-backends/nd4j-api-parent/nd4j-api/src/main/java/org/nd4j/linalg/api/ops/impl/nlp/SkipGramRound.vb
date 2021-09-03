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

	Public Class SkipGramRound
		Inherits DynamicCustomOp

		Public Overrides Function opName() As String
			Return "skipgram"
		End Function

		Public Sub New()
		End Sub

		''' <summary>
		''' sg hs round
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="syn0"> </param>
		''' <param name="syn1"> </param>
		''' <param name="expTable"> </param>
		''' <param name="indices"> </param>
		''' <param name="codes"> </param>
		''' <param name="alpha"> </param>
		''' <param name="randomValue"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SkipGramRound(int target, @NonNull INDArray syn0, @NonNull INDArray syn1, @NonNull INDArray expTable, int[] indices, byte[] codes, double alpha, long randomValue, org.nd4j.linalg.api.ndarray.INDArray inferenceVector)
		Public Sub New(ByVal target As Integer, ByVal syn0 As INDArray, ByVal syn1 As INDArray, ByVal expTable As INDArray, ByVal indices() As Integer, ByVal codes() As SByte, ByVal alpha As Double, ByVal randomValue As Long, ByVal inferenceVector As INDArray)
			Me.New(Nd4j.scalar(target), Nd4j.scalar(-1), syn0, syn1, Nd4j.empty(syn1.dataType()), expTable, Nd4j.empty(syn1.dataType()), 0, Nd4j.createFromArray(indices), Nd4j.createFromArray(codes), Nd4j.scalar(alpha), Nd4j.scalar(randomValue), inferenceVector, False, 1)
		End Sub

		''' <summary>
		''' sg ns round
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="ngStarter"> </param>
		''' <param name="syn0"> </param>
		''' <param name="syn1Neg"> </param>
		''' <param name="expTable"> </param>
		''' <param name="negTable"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SkipGramRound(int target, int ngStarter, @NonNull INDArray syn0, @NonNull INDArray syn1Neg, @NonNull INDArray expTable, @NonNull INDArray negTable, int nsRounds, double alpha, long randomValue, org.nd4j.linalg.api.ndarray.INDArray inferenceVector)
		Public Sub New(ByVal target As Integer, ByVal ngStarter As Integer, ByVal syn0 As INDArray, ByVal syn1Neg As INDArray, ByVal expTable As INDArray, ByVal negTable As INDArray, ByVal nsRounds As Integer, ByVal alpha As Double, ByVal randomValue As Long, ByVal inferenceVector As INDArray)
			Me.New(Nd4j.scalar(target), Nd4j.scalar(ngStarter), syn0, Nd4j.empty(syn0.dataType()), syn1Neg, expTable, negTable, nsRounds, Nd4j.empty(DataType.INT), Nd4j.empty(DataType.BYTE), Nd4j.scalar(alpha), Nd4j.scalar(randomValue), inferenceVector, False, 1)
		End Sub

		''' <summary>
		''' full constructor
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SkipGramRound(@NonNull INDArray target, @NonNull INDArray ngStarter, @NonNull INDArray syn0, @NonNull INDArray syn1, @NonNull INDArray syn1Neg, @NonNull INDArray expTable, @NonNull INDArray negTable, int nsRounds, @NonNull INDArray indices, @NonNull INDArray codes, @NonNull INDArray alpha, @NonNull INDArray randomValue, org.nd4j.linalg.api.ndarray.INDArray inferenceVector, boolean preciseMode, int numWorkers)
		Public Sub New(ByVal target As INDArray, ByVal ngStarter As INDArray, ByVal syn0 As INDArray, ByVal syn1 As INDArray, ByVal syn1Neg As INDArray, ByVal expTable As INDArray, ByVal negTable As INDArray, ByVal nsRounds As Integer, ByVal indices As INDArray, ByVal codes As INDArray, ByVal alpha As INDArray, ByVal randomValue As INDArray, ByVal inferenceVector As INDArray, ByVal preciseMode As Boolean, ByVal numWorkers As Integer)
	'        if (indices != null)
	'            Preconditions.checkArgument(indices.length == codes.length, "Indices length should be equal to codes length");

	'        val idx = indices == null ? Nd4j.empty(DataType.INT) : Nd4j.createFromArray(indices);
	'        val code = codes == null ? Nd4j.empty(DataType.BYTE) : Nd4j.createFromArray(codes);

			inputArguments_Conflict.Add(target)
			inputArguments_Conflict.Add(ngStarter)
			inputArguments_Conflict.Add(indices)
			inputArguments_Conflict.Add(codes)
			inputArguments_Conflict.Add(syn0)
			inputArguments_Conflict.Add(syn1)
			inputArguments_Conflict.Add(syn1Neg)
			inputArguments_Conflict.Add(expTable)
			inputArguments_Conflict.Add(negTable)
			inputArguments_Conflict.Add(alpha)
			inputArguments_Conflict.Add(randomValue)
			inputArguments_Conflict.Add(inferenceVector)

			' temporary arrays for neu1e
			'inputArguments.add(Nd4j.create(syn0.dataType(), new long[]{target.isVector() ? target.size(0) : 1, syn0.columns()}));

			' couple of options
			iArguments.Add(CLng(numWorkers))
			iArguments.Add(CLng(nsRounds))

			bArguments.Add(Not inferenceVector.Empty)
			bArguments.Add(preciseMode)

			' this op is always inplace
			setInPlace(True)
			setInplaceCall(True)

			For Each [in] As val In inputArguments_Conflict
				outputArguments_Conflict.Add([in])
			Next [in]
		End Sub
	End Class

End Namespace