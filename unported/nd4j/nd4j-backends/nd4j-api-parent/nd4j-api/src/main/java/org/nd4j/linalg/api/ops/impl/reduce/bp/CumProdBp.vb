Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.bp


	Public Class CumProdBp
		Inherits BaseReductionBp

		Private exclusive As Boolean
		Private reverse As Boolean

		Public Sub New(ByVal sameDiff As SameDiff, ByVal origInput As SDVariable, ByVal gradAtOutput As SDVariable, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer)
			MyBase.New(sameDiff, origInput, gradAtOutput, False, axis)
			Me.exclusive = exclusive
			Me.reverse = reverse

			iArguments.Clear()
			tArguments.Clear()
			addArgs()
		End Sub

		Public Sub New(ByVal origInput As INDArray, ByVal gradAtOutput As INDArray, ByVal output As INDArray, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer)
			MyBase.New(origInput, gradAtOutput, output, False, axis)
			Me.exclusive = exclusive
			Me.reverse = reverse

			iArguments.Clear()
			tArguments.Clear()
			addArgs()
		End Sub

		Public Sub New()
		End Sub

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				If args().Length = 2 Then
					Return 1
				Else
					Return 2
				End If
			End Get
		End Property


		Protected Friend Overrides Sub addArgs()
			addIArgument(If(exclusive, 1, 0))
			addIArgument(If(reverse, 1, 0))
			If dimensions IsNot Nothing AndAlso dimensions.Length > 0 Then
				addIArgument(dimensions)
			End If
		End Sub

		Public Overrides Function opName() As String
			Return "cumprod_bp"
		End Function
	End Class

End Namespace