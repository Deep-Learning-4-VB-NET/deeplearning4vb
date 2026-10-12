Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NoOp = org.nd4j.linalg.learning.config.NoOp

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class NoOpUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.NoOp>
	Public Class NoOpUpdater
		Implements GradientUpdater(Of NoOp)

		Private ReadOnly config As NoOp

		Public Sub New(ByVal config As NoOp)
			Me.config = config
		End Sub

		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean) Implements GradientUpdater(Of NoOp).setState
			Preconditions.checkState(stateMap Is Nothing OrElse stateMap.Count = 0, "No-op updater does not have any updater state," & " attempting to set with %s values", (If(stateMap Is Nothing, 0, stateMap.Count)))
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of NoOp).getState
			Return Collections.emptyMap()
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal shape() As Long, ByVal order As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of NoOp).setStateViewArray
			'No op
		End Sub

		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of NoOp).applyUpdater
			'No op
		End Sub
	End Class

End Namespace