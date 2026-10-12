Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.learning
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule

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

Namespace org.deeplearning4j.nn.updater.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class CustomIUpdater implements org.nd4j.linalg.learning.config.IUpdater
	<Serializable>
	Public Class CustomIUpdater
		Implements IUpdater

		Public Const DEFAULT_SGD_LR As Double = 1e-3

		Private learningRate As Double


		Public Sub New()
			Me.New(DEFAULT_SGD_LR)
		End Sub

		Public Overridable Function stateSize(ByVal numParams As Long) As Long Implements IUpdater.stateSize
			Return 0
		End Function

		Public Overridable Function instantiate(ByVal viewArray As INDArray, ByVal initializeViewArray As Boolean) As GradientUpdater Implements IUpdater.instantiate
			If viewArray IsNot Nothing Then
				Throw New System.InvalidOperationException("View arrays are not supported/required for SGD updater")
			End If
			Return New CustomGradientUpdater(Me)
		End Function

		Public Overridable Function instantiate(ByVal updaterState As IDictionary(Of String, INDArray), ByVal initializeStateArrays As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function clone() As CustomIUpdater
			Return New CustomIUpdater(learningRate)
		End Function

		Public Overridable Function getLearningRate(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements IUpdater.getLearningRate
			Return learningRate
		End Function

		Public Overridable Function hasLearningRate() As Boolean Implements IUpdater.hasLearningRate
			Return True
		End Function

		Public Overridable Sub setLrAndSchedule(ByVal lr As Double, ByVal iSchedule As ISchedule) Implements IUpdater.setLrAndSchedule
			Me.learningRate = lr
		End Sub
	End Class

End Namespace