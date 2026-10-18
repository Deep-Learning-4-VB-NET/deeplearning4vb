Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>dot_product_attention_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.DotProductAttentionBp</c>).</summary>
    Public Class DotProductAttentionBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private scaled As Boolean

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal queries As SDVariable, ByVal keys As SDVariable, ByVal values As SDVariable, ByVal eps As SDVariable, ByVal mask As SDVariable, ByVal scaled As Boolean)
			MyBase.New(Nothing, sameDiff,If(mask Is Nothing, New SDVariable() {queries, keys, values, eps}, New SDVariable()){queries, keys, values, eps, mask}, False)
			Me.scaled = scaled
			addIArgument(If(scaled, 1, 0))
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "dot_product_attention_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 4 OrElse dataTypes.Count = 5), "Expected 4 or 5 input datatypes, got %s", dataTypes)
            Dim first As DataType = dataTypes(0)
            For i As Integer = 0 To dataTypes.Count - 1
                Preconditions.checkState(dataTypes(i).isFPType(), "Input %s datatype must be a floating point type, got datypes %s", dataTypes)
                If i > 0 Then
                    Preconditions.checkState(first = dataTypes(i), "All datatypes must be same type, got input datatypes %s", dataTypes)
                End If
            Next i

            Return New List(Of DataType) From {first, first, first}
        End Function

    End Class

End Namespace
