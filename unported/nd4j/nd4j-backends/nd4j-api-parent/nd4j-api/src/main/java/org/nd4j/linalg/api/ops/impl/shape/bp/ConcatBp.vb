Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op

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

Namespace org.nd4j.linalg.api.ops.impl.shape.bp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ConcatBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ConcatBp
		Inherits DynamicCustomOp

		Private concatDimension As Integer
		Private dynamicAxis As Boolean

		Public Sub New()

		End Sub

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

		Public Overrides Function opName() As String
			Return "concat_bp"
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length - 1 - (If(dynamicAxis, 1, 0))
			End Get
		End Property

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
	End Class

End Namespace