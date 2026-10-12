Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class Standardize
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v}, False)
			Me.Dimensions = dimensions
		End Sub

		Public Sub New(ByVal input As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(input, Nothing, dimensions)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal result As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New("standardize", New INDArray(){input},wrapOrNull(result))
			Me.Dimensions = dimensions
		End Sub

		Public Sub New()
		End Sub

		Public Overrides WriteOnly Property Dimensions As Integer()
			Set(ByVal dimensions() As Integer)
				Preconditions.checkArgument(dimensions IsNot Nothing, "Standardize: You have to provide dimensions")
				Preconditions.checkArgument(dimensions.Length > 0, "Standardize: You have to provide dimensions")
    
				Me.dimensions = dimensions
				addIArgument(dimensions)
			End Set
		End Property

		Public Overrides Function opName() As String
			Return "standardize"
		End Function


		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New StandardizeBp(sameDiff, arg(0), grad(0), dimensions)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType(), "Input must be a floating point type, got %s", dataTypes(0))
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace