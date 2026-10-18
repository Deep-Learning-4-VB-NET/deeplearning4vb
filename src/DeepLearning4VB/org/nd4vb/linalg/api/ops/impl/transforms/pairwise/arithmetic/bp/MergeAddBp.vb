Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp

    ''' <summary>The custom op <c>mergeadd_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.MergeAddBp</c>).</summary>
    Public Class MergeAddBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeAddBp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable[] inputs, @NonNull SDVariable gradO)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal gradO As SDVariable)
			MyBase.New("mergeadd_bp", sameDiff, ArrayUtils.add(inputs, gradO))
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "mergeadd_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim list As New List(Of DataType)()
			Dim i As Integer=0
			Do While i< args().Length-1
				list.Add(inputDataTypes(0))
				i += 1
			Loop
			Return list

		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

#If SAMEDIFF Then
		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length-1
			End Get
		End Property
#Else
        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return inputArguments().Count-1
            End Get
        End Property
#End If

    End Class

End Namespace
