Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.learning

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
'ORIGINAL LINE: @AllArgsConstructor public class CustomGradientUpdater implements org.nd4j.linalg.learning.GradientUpdater<CustomIUpdater>
	Public Class CustomGradientUpdater
		Implements GradientUpdater(Of CustomIUpdater)

'JAVA TO VB CONVERTER NOTE: The field config was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private config_Conflict As CustomIUpdater

		Public Overridable ReadOnly Property Config As CustomIUpdater
			Get
				Return config_Conflict
			End Get
		End Property

		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean) Implements GradientUpdater(Of CustomIUpdater).setState
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of CustomIUpdater).getState
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of CustomIUpdater).setStateViewArray
			'No op
		End Sub

		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of CustomIUpdater).applyUpdater
			gradient.muli(config_Conflict.getLearningRate())
		End Sub
	End Class

End Namespace