﻿Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.bp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class VarianceBp extends BaseReductionBp
	Public Class VarianceBp
		Inherits BaseReductionBp

		Private biasCorrected As Boolean

		Public Sub New(ByVal sameDiff As SameDiff, ByVal origInput As SDVariable, ByVal gradAtOutput As SDVariable, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, origInput, gradAtOutput, keepDims, dimensions)
			Me.biasCorrected = biasCorrected
			addTArgument(If(biasCorrected, 1.0, 0.0))
		End Sub

		Public Sub New(ByVal origInput As INDArray, ByVal gradAtOutput As INDArray, ByVal output As INDArray, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(origInput, gradAtOutput, output, keepDims, dimensions)
			Me.biasCorrected = biasCorrected
			addTArgument(If(biasCorrected, 1.0, 0.0))
		End Sub

		Public Overrides Function opName() As String
			Return "reduce_variance_bp"
		End Function
	End Class

End Namespace