Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AdaDeltaUpdater = org.nd4j.linalg.learning.AdaDeltaUpdater
Imports org.nd4j.linalg.learning
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

Namespace org.nd4j.linalg.learning.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder(builderClassName = "Builder") public class AdaDelta implements IUpdater
	<Serializable>
	Public Class AdaDelta
		Implements IUpdater

		Public Const DEFAULT_ADADELTA_RHO As Double = 0.95
		Public Const DEFAULT_ADADELTA_EPSILON As Double = 1e-6

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double rho = DEFAULT_ADADELTA_RHO;
		Private rho As Double = DEFAULT_ADADELTA_RHO
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double epsilon = DEFAULT_ADADELTA_EPSILON;
		Private epsilon As Double = DEFAULT_ADADELTA_EPSILON

		Public Sub New()
			Me.New(DEFAULT_ADADELTA_RHO, DEFAULT_ADADELTA_EPSILON)
		End Sub

		Public Overridable Function stateSize(ByVal numParams As Long) As Long Implements IUpdater.stateSize
			Return 2 * numParams
		End Function

		Public Overridable Function instantiate(ByVal viewArray As INDArray, ByVal initializeViewArray As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Dim u As New AdaDeltaUpdater(Me)
			Dim gradientShape() As Long = viewArray.shape()
			gradientShape = Arrays.CopyOf(gradientShape, gradientShape.Length)
			gradientShape(1) \= 2
			u.setStateViewArray(viewArray, gradientShape, viewArray.ordering(), initializeViewArray)
			Return u
		End Function

		Public Overridable Function instantiate(ByVal updaterState As IDictionary(Of String, INDArray), ByVal initializeStateArrays As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Dim u As New AdaDeltaUpdater(Me)
			u.setState(updaterState, initializeStateArrays)
			Return u
		End Function

		Public Overridable Function clone() As AdaDelta
			Return New AdaDelta(rho, epsilon)
		End Function

		Public Overridable Function getLearningRate(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements IUpdater.getLearningRate
			Return Double.NaN 'No LR for  this updater
		End Function

		Public Overridable Function hasLearningRate() As Boolean Implements IUpdater.hasLearningRate
			Return False
		End Function

		Public Overridable Sub setLrAndSchedule(ByVal lr As Double, ByVal lrSchedule As ISchedule) Implements IUpdater.setLrAndSchedule
			Throw New System.NotSupportedException("Cannot set learning rate or LR schedule: AdaDelta does not have a learning rate")
		End Sub

		'Partial builder implementation to give public no-arg constructor
		Public Class Builder
			Public Sub New()
			End Sub
		End Class
	End Class

End Namespace