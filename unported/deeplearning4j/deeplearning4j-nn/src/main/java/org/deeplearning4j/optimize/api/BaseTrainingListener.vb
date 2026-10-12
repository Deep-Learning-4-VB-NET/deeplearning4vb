Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
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

Namespace org.deeplearning4j.optimize.api


	Public MustInherit Class BaseTrainingListener
		Implements TrainingListener

		Public Overridable Sub onEpochStart(ByVal model As Model) Implements TrainingListener.onEpochStart
			'No op
		End Sub


		Public Overridable Sub onEpochEnd(ByVal model As Model) Implements TrainingListener.onEpochEnd
			'No op
		End Sub


		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray)) Implements TrainingListener.onForwardPass
			'No op
		End Sub


		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray)) Implements TrainingListener.onForwardPass
			'No op
		End Sub


		Public Overridable Sub onGradientCalculation(ByVal model As Model) Implements TrainingListener.onGradientCalculation
			'No op
		End Sub


		Public Overridable Sub onBackwardPass(ByVal model As Model) Implements TrainingListener.onBackwardPass
			'No op
		End Sub


		Public Overridable Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer) Implements TrainingListener.iterationDone
			'No op
		End Sub
	End Class

End Namespace