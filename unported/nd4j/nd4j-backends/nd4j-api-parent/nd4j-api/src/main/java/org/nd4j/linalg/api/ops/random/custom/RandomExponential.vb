Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.linalg.api.ops.random.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class RandomExponential extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class RandomExponential
		Inherits DynamicCustomOp

		Private lambda As Double = 0.0
		Private dataType As DataType = DataType.DOUBLE

		Public Sub New()
			'
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal shape As SDVariable, ByVal lambda As Double)
			MyBase.New(Nothing, sd, New SDVariable(){shape})
			Preconditions.checkState(lambda >= 0, "Lambda parameter must be > 0 - got %s", lambda)
			Me.lambda = lambda
			addTArgument(lambda)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal lambda As Double, ByVal dataType As DataType, ParamArray ByVal shape() As Long)
			MyBase.New(Nothing, sd, New SDVariable(){sd.constant(Nd4j.createFromArray(shape))})
			Me.lambda = lambda
			addTArgument(lambda)
			Me.dataType = dataType
			addDArgument(dataType)
			addIArgument(shape)
		End Sub

		Public Sub New(ByVal lambda As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long)
			Me.New(Nd4j.createFromArray(shape), Nd4j.createUninitialized(datatype, shape), lambda)
		End Sub

		Public Sub New(ByVal shape As INDArray, ByVal [out] As INDArray, ByVal lambda As Double)
			MyBase.New(Nothing, New INDArray(){shape}, New INDArray(){[out]}, Collections.singletonList(lambda), DirectCast(Nothing, IList(Of Integer)))
			Me.lambda = lambda
		End Sub

		Public Overrides Function opName() As String
			Return "random_exponential"
		End Function

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
			'Input data type specifies the shape; output data type should be any float
			'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
			Return Collections.singletonList(DataType.FLOAT)
		End Function
	End Class

End Namespace