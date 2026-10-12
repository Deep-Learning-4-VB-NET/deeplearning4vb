Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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

Namespace org.nd4j.linalg.api.ops.random.impl


	Public Class BinomialDistributionEx
		Inherits BaseRandomOp

		Private trials As Long
		Private probability As Double

		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' This op fills Z with binomial distribution over given trials with single given probability for all trials </summary>
		''' <param name="z"> </param>
		''' <param name="trials"> </param>
		''' <param name="probability"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BinomialDistributionEx(@NonNull INDArray z, long trials, double probability)
		Public Sub New(ByVal z As INDArray, ByVal trials As Long, ByVal probability As Double)
			MyBase.New(z, z, z)
			Me.trials = trials
			Me.probability = probability
			Me.extraArgs = New Object() {CDbl(Me.trials), Me.probability}
		End Sub

		''' <summary>
		''' This op fills Z with binomial distribution over given trials with probability for each trial given as probabilities INDArray </summary>
		''' <param name="z"> </param>
		''' <param name="trials"> </param>
		''' <param name="probabilities"> array with probability value for each trial </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BinomialDistributionEx(@NonNull INDArray z, long trials, @NonNull INDArray probabilities)
		Public Sub New(ByVal z As INDArray, ByVal trials As Long, ByVal probabilities As INDArray)
			MyBase.New(z, probabilities, z)
			If z.length() <> probabilities.length() Then
				Throw New System.InvalidOperationException("Length of probabilities array should match length of target array")
			End If

			If probabilities.elementWiseStride() < 1 Then
				Throw New System.InvalidOperationException("Probabilities array shouldn't have negative elementWiseStride")
			End If

			Preconditions.checkArgument(probabilities.dataType() = z.dataType(), "Probabilities and Z operand should have same data type")

			Me.trials = trials
			Me.probability = 0.0
			Me.extraArgs = New Object() {CDbl(Me.trials), Me.probability}
		End Sub


		''' <summary>
		''' This op fills Z with binomial distribution over given trials with probability for each trial given as probabilities INDArray
		''' </summary>
		''' <param name="z"> </param>
		''' <param name="probabilities"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BinomialDistributionEx(@NonNull INDArray z, @NonNull INDArray probabilities)
		Public Sub New(ByVal z As INDArray, ByVal probabilities As INDArray)
			Me.New(z, probabilities.length(), probabilities)
		End Sub


		Public Overrides Function opNum() As Integer
			Return 9
		End Function

		Public Overrides Function opName() As String
			Return "distribution_binomial_ex"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("BinomialDistributionEx does not have a derivative.")
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Return calculateOutputShape()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape,dataType)
			Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
		End Function
	End Class

End Namespace