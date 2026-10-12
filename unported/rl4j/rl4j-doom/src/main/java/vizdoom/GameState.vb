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

Namespace vizdoom

	Public Class GameState
		Public number As Integer
		Public tic As Integer

		Public gameVariables() As Double

		Public screenBuffer() As SByte
		Public depthBuffer() As SByte
		Public labelsBuffer() As SByte
		Public automapBuffer() As SByte

		Public labels() As Label

		Friend Sub New(ByVal number As Integer, ByVal tic As Integer, ByVal gameVariables() As Double, ByVal screenBuffer() As SByte, ByVal depthBuffer() As SByte, ByVal labelsBuffer() As SByte, ByVal automapBuffer() As SByte, ByVal labels() As Label)

			Me.number = number
			Me.tic = tic
			Me.gameVariables = gameVariables
			Me.screenBuffer = screenBuffer
			Me.depthBuffer = depthBuffer
			Me.labelsBuffer = labelsBuffer
			Me.automapBuffer = automapBuffer
			Me.labels = labels
		End Sub
	End Class

End Namespace