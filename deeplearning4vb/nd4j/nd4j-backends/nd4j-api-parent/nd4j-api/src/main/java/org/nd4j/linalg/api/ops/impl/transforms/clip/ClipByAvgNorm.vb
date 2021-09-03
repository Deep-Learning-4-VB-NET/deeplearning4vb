Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
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
Namespace org.nd4j.linalg.api.ops.impl.transforms.clip



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class ClipByAvgNorm extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ClipByAvgNorm
		Inherits DynamicCustomOp

		Private clipValue As Double


		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New("clipbyavgnorm", sameDiff, New SDVariable(){x})
			Me.clipValue = clipValue
			Me.dimensions = dimensions
			addIArgument(dimensions)
			addTArgument(clipValue)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer)
			Me.New([in], Nothing, clipValue, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New("clipbyavgnorm", New INDArray(){[in]}, wrapOrNull([out]), Collections.singletonList(clipValue), dimensions)
		End Sub

		Public Overrides Function opName() As String
			Return "clipbyavgnorm"
		End Function



		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
			Return inputDataTypes
		End Function

	End Class



End Namespace