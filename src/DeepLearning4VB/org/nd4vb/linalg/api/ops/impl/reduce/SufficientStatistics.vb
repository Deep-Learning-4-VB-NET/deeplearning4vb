Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
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

Namespace org.nd4vb.linalg.api.ops.impl.reduce

    ''' <summary>The custom op <c>sufficient_statistics</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.SufficientStatistics</c>).</summary>
    Public Class SufficientStatistics
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SufficientStatistics(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable axis, org.nd4j.autodiff.samediff.SDVariable shift)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal axis As SDVariable, ByVal shift As SDVariable)
			MyBase.New(Nothing, sameDiff, argsNoNull(x, axis, shift), False)
		End Sub

		Private Shared Function argsNoNull(ByVal x As SDVariable, ByVal axis As SDVariable, ByVal shift As SDVariable) As SDVariable()
			If shift Is Nothing Then
				Return New SDVariable(){x, axis}
			Else
				Return New SDVariable(){x, axis, shift}
			End If
		End Function
#End If

        Public Sub New(ByVal x As INDArray, ByVal axes As INDArray, ByVal shift As INDArray)
            If shift IsNot Nothing Then
                addInputArgument(x, axes, shift)
            Else
                addInputArgument(x, axes)
            End If
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal axes As INDArray)
            Me.New(x,axes,Nothing)
        End Sub

        Public Overrides Function opName() As String
            Return "sufficient_statistics"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop not yet implemented for op: " & Me.GetType().Name)
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            ' FIXME
            Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(0),inputDataTypes(0)}
        End Function

    End Class

End Namespace
