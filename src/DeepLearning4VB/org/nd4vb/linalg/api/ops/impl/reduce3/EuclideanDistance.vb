Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SameDiffUtils = DeepLearning4VB.org.nd4vb.autodiff.util.SameDiffUtils
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.reduce3

    ''' <summary>The legacy reduce3 op <c>euclidean</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance</c>).</summary>
    Public Class EuclideanDistance
        Inherits BaseReduce3Op

        Public Const OP_NAME As String = "euclidean"

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, y, CType(Nothing, INDArray), dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            Me.New(x, y, z, CType(Nothing, Integer()))
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, y, z,dimensions)
            extraArgsField = New Object(){0.0f, 0.0f}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
            Me.New(x, y, CType(Nothing, INDArray), allDistances, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
            Me.New(x, y, z, False, allDistances, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, y, z, keepDims, allDistances, dimensions)
            extraArgsField = New Object(){0.0f, 0.0f}
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sd,x,y,keepDims,isComplex,dimensions)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
            MyBase.New(x, y, CType(Nothing, INDArray), keepDims, isComplex, dimensions)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 1
        End Function

        Public Overrides Function opName() As String
            Return OP_NAME
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'ddist(x,y)/dxi = (xi-yi)/dist(x,y)
			Dim euc As SDVariable = outputVariables()(0)
			Dim difference As SDVariable = larg().sub(rarg())
			Dim divBroadcastable As SDVariable = i_v1(0).div(euc)
			If Not keepDims_Conflict AndAlso Not (dimensions Is Nothing OrElse dimensions.Length = 0 OrElse (dimensions.Length = 1 AndAlso dimensions(0) = Integer.MaxValue)) Then
				'Not keep dims, and not full array reduction -> need to make broadcastable
				divBroadcastable = SameDiffUtils.reductionBroadcastableWithOrigShape(arg(), sameDiff.constant(Nd4j.createFromArray(dimensions)), divBroadcastable)
			End If

			Dim gradX As SDVariable = difference.mul(divBroadcastable)
			Dim gradY As SDVariable = sameDiff.math_Conflict.neg(gradX)
			Return New List(Of SDVariable) From {gradX, gradY}
		End Function
#End If

    End Class

End Namespace
