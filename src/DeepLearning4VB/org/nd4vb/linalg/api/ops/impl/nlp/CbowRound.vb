Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.ops.impl.nlp

    ''' <summary>The custom op <c>cbow</c> (Java <c>org.nd4j.linalg.api.ops.impl.nlp.CbowRound</c>).</summary>
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
        Public Sub New(ByVal target As Integer, ByVal context() As Integer, ByVal lockedWords() As Integer, ByVal syn0 As INDArray, ByVal syn1 As INDArray, ByVal expTable As INDArray, ByVal indices() As Integer, ByVal codes() As SByte, ByVal alpha As Double, ByVal nextRandom As Long, ByVal inferenceVector As INDArray, ByVal numLabels As Integer)
            Me.New(Nd4vbT.scalar(target), Nd4vbT.createFromArray(context), Nd4vbT.createFromArray(lockedWords), Nd4vbT.empty(DataType.INT), syn0, syn1, Nd4vbT.empty(syn1.dataType()), expTable, Nd4vbT.empty(syn1.dataType()), Nd4vbT.createFromArray(indices), Nd4vbT.createFromArray(codes), 0, Nd4vbT.scalar(alpha), Nd4vbT.scalar(nextRandom), inferenceVector, Nd4vbT.scalar(numLabels), inferenceVector.Empty, 1)
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
        Public Sub New(ByVal target As Integer, ByVal context() As Integer, ByVal lockedWords() As Integer, ByVal ngStarter As Integer, ByVal syn0 As INDArray, ByVal syn1Neg As INDArray, ByVal expTable As INDArray, ByVal negTable As INDArray, ByVal nsRounds As Integer, ByVal alpha As Double, ByVal nextRandom As Long, ByVal inferenceVector As INDArray, ByVal numLabels As Integer)
            Me.New(Nd4vbT.scalar(target), Nd4vbT.createFromArray(context), Nd4vbT.createFromArray(lockedWords), Nd4vbT.scalar(ngStarter), syn0, Nd4vbT.empty(syn0.dataType()), syn1Neg, expTable, negTable, Nd4vbT.empty(DataType.INT), Nd4vbT.empty(DataType.BYTE), nsRounds, Nd4vbT.scalar(alpha), Nd4vbT.scalar(nextRandom), inferenceVector, Nd4vbT.scalar(numLabels), inferenceVector.Empty, 1)
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
        Public Sub New(ByVal target As INDArray, ByVal context As INDArray, ByVal lockedWords As INDArray, ByVal ngStarter As INDArray, ByVal syn0 As INDArray, ByVal syn1 As INDArray, ByVal syn1Neg As INDArray, ByVal expTable As INDArray, ByVal negTable As INDArray, ByVal indices As INDArray, ByVal codes As INDArray, ByVal nsRounds As Integer, ByVal alpha As INDArray, ByVal nextRandom As INDArray, ByVal inferenceVector As INDArray, ByVal numLabels As INDArray, ByVal trainWords As Boolean, ByVal numWorkers As Integer)

            inputArgumentsField.Add(target)
            inputArgumentsField.Add(ngStarter)
            inputArgumentsField.Add(context)
            inputArgumentsField.Add(indices)
            inputArgumentsField.Add(codes)
            inputArgumentsField.Add(syn0)
            inputArgumentsField.Add(syn1)
            inputArgumentsField.Add(syn1Neg)
            inputArgumentsField.Add(expTable)
            inputArgumentsField.Add(negTable)
            inputArgumentsField.Add(alpha)
            inputArgumentsField.Add(nextRandom)
            inputArgumentsField.Add(numLabels)
            inputArgumentsField.Add(lockedWords)
            inputArgumentsField.Add(inferenceVector)

            ' couple of options
            iArguments.Add(CLng(numWorkers))
            iArguments.Add(CLng(nsRounds))

            bArguments.Add(trainWords)
            bArguments.Add(Not inferenceVector.Empty)

            ' this op is always inplace
            setInPlace(True)
            setInplaceCall(True)

            For Each [in] In inputArgumentsField
                outputArgumentsField.Add([in])
            Next [in]
        End Sub

        Public Overrides Function opName() As String
            Return "cbow"
        End Function

    End Class

End Namespace
