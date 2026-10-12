Imports System
Imports System.Collections.Generic
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation

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

Namespace org.deeplearning4j.nn.conf.layers.samediff


	Public Class SameDiffLayerUtils

		Private Shared activationMap As IDictionary(Of Type, Activation)

		Private Sub New()
		End Sub

		Public Shared Function fromIActivation(ByVal a As IActivation) As Activation

			If activationMap Is Nothing Then
				Dim m As IDictionary(Of Type, Activation) = New Dictionary(Of Type, Activation)()
				For Each act As Activation In Activation.values()
					m(act.getActivationFunction().GetType()) = act
				Next act
				activationMap = m
			End If

			Return activationMap(a.GetType())
		End Function

	End Class

End Namespace