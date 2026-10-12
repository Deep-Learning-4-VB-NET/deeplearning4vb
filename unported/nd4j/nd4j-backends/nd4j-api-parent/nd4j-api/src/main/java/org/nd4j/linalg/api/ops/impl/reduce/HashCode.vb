Imports NonNull = lombok.NonNull
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

Namespace org.nd4j.linalg.api.ops.impl.reduce

	Public Class HashCode
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public HashCode(@NonNull INDArray array)
		Public Sub New(ByVal array As INDArray)
			Me.inputArguments_Conflict.Add(array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public HashCode(@NonNull INDArray array, @NonNull INDArray result)
		Public Sub New(ByVal array As INDArray, ByVal result As INDArray)
			Me.New(array)
			Preconditions.checkArgument(result.dataType() = DataType.LONG AndAlso result.isScalar(), "HashCode op expects LONG scalar as output")

			Me.outputArguments_Conflict.Add(result)
		End Sub

		Public Overrides Function opName() As String
			Return "hashcode"
		End Function
	End Class

End Namespace