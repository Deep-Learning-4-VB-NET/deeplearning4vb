Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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

Namespace org.nd4j.linalg.api.ops.random.impl


	Public Class BernoulliDistribution
		Inherits BaseRandomOp

		Private prob As Double

		Public Sub New(ByVal sd As SameDiff, ByVal prob As Double, ByVal shape() As Long)
			MyBase.New(sd, shape)
			Me.prob = prob
			Me.extraArgs = New Object() {Me.prob}
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal prob As Double, ByVal dataType As DataType, ByVal shape() As Long)
			Me.New(sd, prob, shape)
			Me.prob = prob
			Me.extraArgs = New Object() {Me.prob}
			MyBase.dataType = dataType
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal p As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long)
			Me.New(Nd4j.createUninitialized(datatype, shape), p)
		End Sub

		''' <summary>
		''' This op fills Z with bernoulli trial results, so 0, or 1, depending by common probability </summary>
		''' <param name="z">
		'''  </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BernoulliDistribution(@NonNull INDArray z, double prob)
		Public Sub New(ByVal z As INDArray, ByVal prob As Double)
			MyBase.New(Nothing, Nothing, z)
			Me.prob = prob
			Me.extraArgs = New Object() {Me.prob}
		End Sub

		''' <summary>
		''' This op fills Z with bernoulli trial results, so 0, or 1, each element will have it's own success probability defined in prob array </summary>
		''' <param name="prob"> array with probabilities </param>
		''' <param name="z">
		'''  </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BernoulliDistribution(@NonNull INDArray z, @NonNull INDArray prob)
		Public Sub New(ByVal z As INDArray, ByVal prob As INDArray)
			MyBase.New(prob, Nothing, z)
			If prob.elementWiseStride() <> 1 Then
				Throw New ND4JIllegalStateException("Probabilities should have ElementWiseStride of 1")
			End If

			If prob.length() <> z.length() Then
				Throw New ND4JIllegalStateException("Length of probabilities array [" & prob.length() & "] doesn't match length of output array [" & z.length() & "]")
			End If
			Me.prob = 0.0
			Me.extraArgs = New Object() {Me.prob}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 7
		End Function

		Public Overrides Function opName() As String
			Return "distribution_bernoulli"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Return calculateOutputShape()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape,dataType)
			Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
		End Function
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.emptyList() 'No SDVariable args
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes Is Nothing OrElse inputDataTypes.Count = 0, "Expected no input datatypes (no args) for %s, got %s", Me.GetType(), inputDataTypes)
			'Input data type specifies the shape; output data type should be any float
			'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
			Return Collections.singletonList(dataType)
		End Function
	End Class

End Namespace