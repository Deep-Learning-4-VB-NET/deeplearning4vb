Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports org.nd4j.common.primitives
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports RectifiedLinear = org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinear
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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

Namespace org.nd4j.linalg.activations.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @JsonIgnoreProperties({"alpha"}) @Getter public class ActivationRReLU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationRReLU
		Inherits BaseActivationFunction

		Public Const DEFAULT_L As Double = 1.0 / 8
		Public Const DEFAULT_U As Double = 1.0 / 3

		Private l, u As Double
		<NonSerialized>
		Private alpha As INDArray 'don't need to write to json, when streaming

		Public Sub New()
			Me.New(DEFAULT_L, DEFAULT_U)
		End Sub

		Public Sub New(ByVal l As Double, ByVal u As Double)
			If l > u Then
				Throw New System.ArgumentException("Cannot have lower value (" & l & ") greater than upper (" & u & ")")
			End If
			Me.l = l
			Me.u = u
		End Sub

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			If training Then
				Using ignored As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
					Me.alpha = Nd4j.rand(l, u, Nd4j.Random, [in].shape())
				End Using
				Dim inTimesAlpha As INDArray = [in].mul(alpha)
				BooleanIndexing.replaceWhere([in], inTimesAlpha, Conditions.lessThan(0))
			Else
				Me.alpha = Nothing
				Dim a As Double = 0.5 * (l + u)
				Return Nd4j.Executioner.exec(New RectifiedLinear([in], a))
			End If

			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)
			Dim dLdz As INDArray = Nd4j.ones([in].shape())
			BooleanIndexing.replaceWhere(dLdz, alpha, Conditions.lessThanOrEqual(0.0))
			dLdz.muli(epsilon)

			Return New Pair(Of INDArray, INDArray)(dLdz, Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "rrelu(l=" & l & ", u=" & u & ")"
		End Function

	End Class

End Namespace