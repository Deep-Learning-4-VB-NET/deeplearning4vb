Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports PowBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.bp.PowBp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>Pow</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Pow</c>).</summary>
    Public Class Pow
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){x, y})
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            MyBase.New(New INDArray(){x, y}, Nothing)
        End Sub

        Public Overrides Function opName() As String
            Return "Pow"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Pow"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO: replace this with discrete op once available: https://github.com/eclipse/deeplearning4j/issues/7461
			'If y=a^b, then:
			'dL/da = b*a^(b-1) * dL/dy
			'dL/db = a^b * log(a) * dL/dy

	'        SDVariable a = arg(0);
	'        SDVariable b = arg(1);
	'        SDVariable dlda = b.mul(sameDiff.math().pow(a,b.sub(1))).mul(f1.get(0));
	'        SDVariable dldb = outputVariable().mul(sameDiff.math().log(a)).mul(f1.get(0));
	'        return Arrays.asList(dlda, dldb);

			Return (New PowBp(sameDiff, arg(0), arg(1), f1(0))).outputs()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
