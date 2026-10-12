Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.deeplearning4j.optimize.solvers.accumulation.encoding.residual

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ResidualClippingPostProcessor implements org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
	<Serializable>
	Public Class ResidualClippingPostProcessor
		Implements ResidualPostProcessor

		Private ReadOnly thresholdMultipleClipValue As Double
		Private ReadOnly frequency As Integer

		''' 
		''' <param name="thresholdMultipleClipValue"> The multiple of the current threshold to use for clipping. A value of C means
		'''                                   that the residual vector will be clipped to the range [-C*T, C*T] for the current
		'''                                   threshold T </param>
		''' <param name="frequency">                  Frequency with which to apply the clipping </param>
		Public Sub New(ByVal thresholdMultipleClipValue As Double, ByVal frequency As Integer)
			Preconditions.checkState(thresholdMultipleClipValue >= 1.0, "Threshold multiple must be a positive value and " & "greater than 1.0 (1.0 means clip at 1x the current threshold)")
			Me.thresholdMultipleClipValue = thresholdMultipleClipValue
			Me.frequency = frequency
		End Sub

		Public Overridable Sub processResidual(ByVal iteration As Integer, ByVal epoch As Integer, ByVal lastThreshold As Double, ByVal residualVector As INDArray) Implements ResidualPostProcessor.processResidual
			If iteration > 0 AndAlso iteration Mod frequency = 0 Then
				Dim currClip As Double = lastThreshold * thresholdMultipleClipValue
				'TODO replace with single op once we have GPU version
				BooleanIndexing.replaceWhere(residualVector, currClip, Conditions.greaterThan(currClip))
				BooleanIndexing.replaceWhere(residualVector, -currClip, Conditions.lessThan(-currClip))
				log.debug("Applied residual clipping: iter={}, epoch={}, lastThreshold={}, multiple={}, clipValue={}", iteration, epoch, lastThreshold, thresholdMultipleClipValue, currClip)
			End If
		End Sub

		Public Overridable Function clone() As ResidualClippingPostProcessor
			Return New ResidualClippingPostProcessor(thresholdMultipleClipValue, frequency)
		End Function
	End Class

End Namespace