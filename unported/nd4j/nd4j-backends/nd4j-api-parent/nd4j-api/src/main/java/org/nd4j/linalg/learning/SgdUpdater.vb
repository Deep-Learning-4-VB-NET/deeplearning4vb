Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd

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

Namespace org.nd4j.linalg.learning


	''' <summary>
	''' SGD updater applies a learning rate only
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SgdUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.Sgd>
	Public Class SgdUpdater
		Implements GradientUpdater(Of Sgd)

		Private ReadOnly config As Sgd

		Public Sub New(ByVal config As Sgd)
			Me.config = config
		End Sub

		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean) Implements GradientUpdater(Of Sgd).setState
			Preconditions.checkState(stateMap Is Nothing OrElse stateMap.Count = 0, "SGD updater does not have any updater state," & " attempting to set with %s values", (If(stateMap Is Nothing, 0, stateMap.Count)))
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of Sgd).getState
			Return Collections.emptyMap()
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of Sgd).setStateViewArray
			'No op
		End Sub

		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of Sgd).applyUpdater
			Dim lr As Double = config.getLearningRate(iteration, epoch)
			Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.SgdUpdater(gradient, lr))
		End Sub
	End Class

End Namespace