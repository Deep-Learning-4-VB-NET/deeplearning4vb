Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.linalg.api.ops.compat

	Public Class CompatStringSplit
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

		Public Sub New(ByVal strings As INDArray, ByVal delimiter As INDArray)
			Preconditions.checkArgument(strings.S AndAlso delimiter.S, "Input arrays must have one of UTF types")
			inputArguments_Conflict.Add(strings)
			inputArguments_Conflict.Add(delimiter)
		End Sub

		Public Sub New(ByVal strings As INDArray, ByVal delimiter As INDArray, ByVal indices As INDArray, ByVal values As INDArray)
			Me.New(strings, delimiter)

			outputArguments_Conflict.Add(indices)
			outputArguments_Conflict.Add(values)
		End Sub

		Public Overrides Function opName() As String
			Return "compat_string_split"
		End Function
	End Class

End Namespace