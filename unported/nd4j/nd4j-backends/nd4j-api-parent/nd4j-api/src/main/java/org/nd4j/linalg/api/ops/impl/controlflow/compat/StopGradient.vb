Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp

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

Namespace org.nd4j.linalg.api.ops.impl.controlflow.compat



	Public Class StopGradient
		Inherits BaseDynamicTransformOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(sd, New SDVariable(){[in]}, False)
		End Sub

		Public Overrides Function opName() As String
			Return "stop_gradient"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "StopGradient"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal input As IList(Of DataType)) As IList(Of DataType)
			Return input
		End Function

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
	End Class

End Namespace