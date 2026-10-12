Imports System.Collections.Generic
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports AbstractSameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.AbstractSameDiffLayer
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
import static org.junit.jupiter.api.Assertions.assertNotNull

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

Namespace org.deeplearning4j.nn.modelimport.keras


	Public Class KerasTestUtils

		Private Sub New()
		End Sub

		Public Shared Function getL1(ByVal layer As BaseLayer) As Double
			Dim l As IList(Of Regularization) = layer.getRegularization()
			Return getL1(l)
		End Function

		Public Shared Function getL1(ByVal l As IList(Of Regularization)) As Double
			Dim l1Reg As L1Regularization = Nothing
			For Each reg As Regularization In l
				If TypeOf reg Is L1Regularization Then
					l1Reg = DirectCast(reg, L1Regularization)
				End If
			Next reg
			assertNotNull(l1Reg)
			Return l1Reg.getL1().valueAt(0,0)
		End Function

		Public Shared Function getL2(ByVal layer As BaseLayer) As Double
			Dim l As IList(Of Regularization) = layer.getRegularization()
			Return getL2(l)
		End Function

		Public Shared Function getL2(ByVal l As IList(Of Regularization)) As Double
			Dim l2Reg As L2Regularization = Nothing
			For Each reg As Regularization In l
				If TypeOf reg Is L2Regularization Then
					l2Reg = DirectCast(reg, L2Regularization)
				End If
			Next reg
			assertNotNull(l2Reg)
			Return l2Reg.getL2().valueAt(0,0)
		End Function

		Public Shared Function getL1(ByVal layer As AbstractSameDiffLayer) As Double
			Return getL1(layer.getRegularization())
		End Function

		Public Shared Function getL2(ByVal layer As AbstractSameDiffLayer) As Double
			Return getL2(layer.getRegularization())
		End Function

	End Class

End Namespace