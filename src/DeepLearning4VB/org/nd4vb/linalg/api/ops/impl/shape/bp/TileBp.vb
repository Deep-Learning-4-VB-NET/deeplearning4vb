Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.impl.shape.bp

    ''' <summary>The custom op <c>tile_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.bp.TileBp</c>).</summary>
    Public Class TileBp
        Inherits DynamicCustomOp

        Private repeat() As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal grad As SDVariable, ByVal repeat() As Integer)
			MyBase.New(Nothing,sameDiff, New SDVariable(){[in], grad}, False)
			Me.repeat = repeat
			addArguments()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal repeat As SDVariable, ByVal grad As SDVariable)
			MyBase.New(Nothing,sameDiff, New SDVariable(){[in], repeat, grad}, False)
			Me.repeat = Nothing
		End Sub
#End If

        Public Sub New()
        End Sub

        Private Sub addArguments()
            addIArgument(repeat)
        End Sub

        Public Overrides Function opName() As String
            Return "tile_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop of gradient op not supported")
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2 OrElse (repeat Is Nothing AndAlso dataTypes.Count = 3), "Expected list with exactly 2 datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as (original) input type
			Return Collections.singletonList(arg().dataType())
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
