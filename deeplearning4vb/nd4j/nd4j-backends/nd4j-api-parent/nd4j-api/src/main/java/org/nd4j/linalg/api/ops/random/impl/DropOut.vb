Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class DropOut extends org.nd4j.linalg.api.ops.random.BaseRandomOp
	Public Class DropOut
		Inherits BaseRandomOp

		Private p As Double

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal p As Double)
			MyBase.New(sameDiff, input)
			Me.p = p
			Me.extraArgs = New Object() {p}

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DropOut(@NonNull INDArray x, double p)
		Public Sub New(ByVal x As INDArray, ByVal p As Double)
			Me.New(x, x, p)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DropOut(@NonNull INDArray x, @NonNull INDArray z, double p)
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal p As Double)
			MyBase.New(x,Nothing,z)
			Me.p = p
			Me.extraArgs = New Object() {p}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 1
		End Function

		Public Overrides Function opName() As String
			Return "dropout"
		End Function

		Public Overrides Function opType() As Type
			Return Type.RANDOM
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim input As INDArray = oc.getInputArray(0)
			Return New List(Of LongShapeDescriptor) From {input.shapeDescriptor()}
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported") 'We should only use *inverted* dropout with samediff
		End Function
	End Class

End Namespace