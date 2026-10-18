Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>adjust_saturation</c> (Java <c>org.nd4j.linalg.api.ops.custom.AdjustSaturation</c>).</summary>
    Public Class AdjustSaturation
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal factor As Double, ByVal [out] As INDArray)
            Me.New([in], factor)
            If [out] IsNot Nothing Then
                outputArgumentsField.Add([out])
            End If
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal factor As Double)
            Preconditions.checkArgument([in].rank() >= 3, "AdjustSaturation: op expects rank of input array to be >= 3, but got %s instead", [in].rank())
            inputArgumentsField.Add([in])

            addTArgument(factor)
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustSaturation(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable factor)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal factor As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){[in], factor})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustSaturation(@NonNull SameDiff sameDiff, @NonNull SDVariable in, double factor)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal factor As Double)
			MyBase.New(sameDiff, New SDVariable(){[in]})
			addTArgument(factor)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "adjust_saturation"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "AdjustSaturation"
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
