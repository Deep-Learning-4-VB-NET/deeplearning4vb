Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.classimbalance

	<Serializable>
	Public Class UnderSamplingByMaskingPreProcessor
		Inherits BaseUnderSamplingPreProcessor
		Implements DataSetPreProcessor

		Private targetMinorityDist As Double
		Private minorityLabel As Integer = 1

		''' <summary>
		''' The target distribution to approximate. Valid values are between (0,0.5].
		''' Eg. For a targetDist = 0.25 and tbpttWindowSize = 100:
		''' Every 100 time steps (starting from the last time step) will randomly mask majority time steps to approximate a 25:75 ratio of minorityLabel to majority classes </summary>
		''' <param name="targetDist"> </param>
		''' <param name="windowSize"> Usually set to the size of the tbptt </param>
		Public Sub New(ByVal targetDist As Double, ByVal windowSize As Integer)
			If targetDist > 0.5 OrElse targetDist <= 0 Then
				Throw New System.ArgumentException("Target distribution for the minorityLabel class has to be greater than 0 and no greater than 0.5. Target distribution of " & targetDist & "given")
			End If
			Me.targetMinorityDist = targetDist
			Me.tbpttWindowSize = windowSize
		End Sub

		''' <summary>
		''' Will change the default minority label from "1" to "0" and correspondingly the majority class from "0" to "1"
		''' </summary>
		Public Overridable Sub overrideMinorityDefault()
			Me.minorityLabel = 0
		End Sub

		Public Overridable Sub preProcess(ByVal toPreProcess As DataSet)
			Dim label As INDArray = toPreProcess.Labels
			Dim labelMask As INDArray = toPreProcess.LabelsMaskArray
			Dim sampledMask As INDArray = adjustMasks(label, labelMask, minorityLabel, targetMinorityDist)
			toPreProcess.LabelsMaskArray = sampledMask
		End Sub
	End Class

End Namespace