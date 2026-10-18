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

    ''' <summary>The custom op <c>roll</c> (Java <c>org.nd4j.linalg.api.ops.custom.Roll</c>).</summary>
    Public Class Roll
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal shifts As INDArray, ByVal axes As INDArray)
            Preconditions.checkArgument(axes.rank() = shifts.rank(), "Roll: shifts and axes should be the same rank")
            Preconditions.checkArgument(axes.length() = shifts.length(), "Roll: shifts and axes should be the same length")
            addInputArgument(input, shifts, axes)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal shift As Integer)
            addInputArgument(input)
            addIArgument(shift)
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable shift)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shift As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){input, shift})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable shift, @NonNull SDVariable axes)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shift As SDVariable, ByVal axes As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){input, shift, axes})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull SameDiff sameDiff, @NonNull SDVariable input, int shift)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shift As Integer)
			MyBase.New("", sameDiff, New SDVariable(){input})
			addIArgument(shift)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "roll"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Roll"
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
