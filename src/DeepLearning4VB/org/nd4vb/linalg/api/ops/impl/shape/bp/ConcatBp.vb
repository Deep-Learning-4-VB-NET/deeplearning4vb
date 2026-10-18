Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
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

    ''' <summary>The custom op <c>concat_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.bp.ConcatBp</c>).</summary>
    Public Class ConcatBp
        Inherits DynamicCustomOp

        Private concatDimension As Integer
        Private dynamicAxis As Boolean

        Public Sub New()

        End Sub

#If SAMEDIFF Then
		''' 
		''' <param name="sameDiff"> </param>
		''' <param name="concatDimension"> </param>
		''' <param name="inputsAndGrad">     Original inputs, followed by output gradient </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConcatBp(@NonNull SameDiff sameDiff, int concatDimension, @NonNull SDVariable... inputsAndGrad)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal concatDimension As Integer, ParamArray ByVal inputsAndGrad() As SDVariable)
			MyBase.New(Nothing, sameDiff, inputsAndGrad)
			addIArgument(concatDimension)
			Me.concatDimension = concatDimension
		End Sub

		''' 
		''' <param name="sameDiff">       SameDiff instance </param>
		''' <param name="inputsGradAxis"> Inputs, gradient array, and axis </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConcatBp(@NonNull SameDiff sameDiff, @NonNull SDVariable... inputsGradAxis)
		Public Sub New(ByVal sameDiff As SameDiff, ParamArray ByVal inputsGradAxis() As SDVariable)
			MyBase.New(Nothing, sameDiff, inputsGradAxis)
			Preconditions.checkState(inputsGradAxis(inputsGradAxis.Length-1).dataType().isIntType(), "When using this constructor, the last input must be an integer array (for the axis)")
			addBArgument(True) 'Last argument
			Me.dynamicAxis = True
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "concat_bp"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length - 1 - (If(dynamicAxis, 1, 0))
			End Get
		End Property
#Else
        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return inputArguments().Count - 1 - (If(dynamicAxis, 1, 0))
            End Get
        End Property
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim args() As SDVariable = Me.args()
			Preconditions.checkState(dataTypes.Count = args.Length, "Expected list with exactly %s datatypes (original inputs + gradient), got %s", args.Length, dataTypes)
			'Output type is same as (original) input types
			Dim n As Integer = NumOutputs
			Dim [out] As IList(Of DataType) = New List(Of DataType)(n)
			For i As Integer = 0 To n - 1
				[out].Add(arg(i).dataType())
			Next i
			Return [out]
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
