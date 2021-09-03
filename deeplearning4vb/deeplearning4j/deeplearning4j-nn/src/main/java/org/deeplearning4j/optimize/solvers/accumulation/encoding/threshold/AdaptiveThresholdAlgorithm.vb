Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports ThresholdAlgorithmReducer = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithmReducer
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @EqualsAndHashCode(exclude = {"lastThreshold", "lastSparsity"}) public class AdaptiveThresholdAlgorithm implements org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
	<Serializable>
	Public Class AdaptiveThresholdAlgorithm
		Implements ThresholdAlgorithm

		Public Const DEFAULT_INITIAL_THRESHOLD As Double = 1e-4
		Public Const DEFAULT_MIN_SPARSITY_TARGET As Double = 1e-4
		Public Const DEFAULT_MAX_SPARSITY_TARGET As Double = 1e-2
		Public Shared ReadOnly DEFAULT_DECAY_RATE As Double = Math.Pow(0.5, (1/20.0)) 'Corresponds to increase/decrease by factor of 2 in 20 iterations


		Private ReadOnly initialThreshold As Double
		Private ReadOnly minTargetSparsity As Double
		Private ReadOnly maxTargetSparsity As Double
		Private ReadOnly decayRate As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double lastThreshold = Double.NaN;
		Private lastThreshold As Double = Double.NaN
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double lastSparsity = Double.NaN;
		Private lastSparsity As Double = Double.NaN

		''' <summary>
		''' Create the adaptive threshold algorithm with the default initial threshold <seealso cref="DEFAULT_INITIAL_THRESHOLD"/>,
		''' default minimum sparsity target <seealso cref="DEFAULT_MIN_SPARSITY_TARGET"/>, default maximum sparsity target <seealso cref="DEFAULT_MAX_SPARSITY_TARGET"/>,
		''' and default decay rate <seealso cref="DEFAULT_DECAY_RATE"/>
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_INITIAL_THRESHOLD, DEFAULT_MIN_SPARSITY_TARGET, DEFAULT_MAX_SPARSITY_TARGET, DEFAULT_DECAY_RATE)
		End Sub

		''' <summary>
		''' Create the adaptive threshold algorithm with the specified initial threshold, but defaults for the other values:
		''' default minimum sparsity target <seealso cref="DEFAULT_MIN_SPARSITY_TARGET"/>, default maximum sparsity target <seealso cref="DEFAULT_MAX_SPARSITY_TARGET"/>,
		''' and default decay rate <seealso cref="DEFAULT_DECAY_RATE"/>
		''' </summary>
		Public Sub New(ByVal initialThreshold As Double)
			Me.New(initialThreshold, DEFAULT_MIN_SPARSITY_TARGET, DEFAULT_MAX_SPARSITY_TARGET, DEFAULT_DECAY_RATE)
		End Sub

		''' 
		''' <param name="initialThreshold">  The initial threshold to use </param>
		''' <param name="minTargetSparsity"> The minimum target sparsity ratio - for example 1e-4 </param>
		''' <param name="maxTargetSparsity"> The maximum target sparsity ratio - for example 1e-2 </param>
		''' <param name="decayRate">         The decay rate. For example 0.95 </param>
		Public Sub New(ByVal initialThreshold As Double, ByVal minTargetSparsity As Double, ByVal maxTargetSparsity As Double, ByVal decayRate As Double)
			Preconditions.checkArgument(initialThreshold > 0.0, "Initial threshold must be positive. Got: %s", initialThreshold)
			Preconditions.checkArgument(minTargetSparsity > 0.0 AndAlso maxTargetSparsity > 0.0, "Minimum and maximum target " & "sparsities must be > 0. Got minTargetSparsity=%s, maxTargetSparsity=%s", minTargetSparsity, maxTargetSparsity)
			Preconditions.checkArgument(minTargetSparsity <= maxTargetSparsity, "Min target sparsity must be less than or equal " & "to max target sparsity. Got minTargetSparsity=%s, maxTargetSparsity=%s", minTargetSparsity, maxTargetSparsity)
			Preconditions.checkArgument(decayRate >= 0.5 AndAlso decayRate < 1.0, "Decay rate must be a number in range 0.5 (inclusive) to 1.0 (exclusive). " & "Usually decay rate is in range 0.95 to 0.999. Got decay rate: %s", decayRate)

			Me.initialThreshold = initialThreshold
			Me.minTargetSparsity = minTargetSparsity
			Me.maxTargetSparsity = maxTargetSparsity
			Me.decayRate = decayRate
		End Sub

		Public Overridable Function calculateThreshold(ByVal iteration As Integer, ByVal epoch As Integer, ByVal lastThreshold As Double?, ByVal lastWasDense As Boolean?, ByVal lastSparsityRatio As Double?, ByVal updatesPlusResidual As INDArray) As Double Implements ThresholdAlgorithm.calculateThreshold

			'handle first iteration - use initial threshold
			If lastThreshold Is Nothing AndAlso Double.IsNaN(Me.lastThreshold) Then
				Me.lastThreshold = initialThreshold
				Return initialThreshold
			End If

			'Check and adapt based on sparsity
			Dim adaptFromThreshold As Double = (If(lastThreshold IsNot Nothing, lastThreshold, Me.lastThreshold))
			Dim prevSparsity As Double
			If lastSparsityRatio IsNot Nothing Then
				prevSparsity = lastSparsityRatio
			ElseIf lastWasDense IsNot Nothing AndAlso lastWasDense.Value Then
				prevSparsity = 1.0/16 'Could be higher, don't know exactly due to dense encoding
			ElseIf Not Double.IsNaN(Me.lastSparsity) Then
				prevSparsity = Me.lastSparsity
			Else
				Throw New System.InvalidOperationException("Unexpected state: not first iteration but no last sparsity value is available: iteration=" & iteration & ", epoch=" & epoch & ", lastThreshold=" & lastThreshold & ", lastWasDense=" & lastWasDense & ", lastSparsityRatio=" & lastSparsityRatio & ", this.lastSparsity=" & Me.lastSparsity)
			End If


			Me.lastSparsity = prevSparsity

			If prevSparsity >= minTargetSparsity AndAlso prevSparsity <= maxTargetSparsity Then
				'OK: keep the last threshold unchanged
				If log.isDebugEnabled() Then
					log.debug("AdaptiveThresholdAlgorithm: iter {} epoch {}: prev sparsity {}, keeping existing threshold of {}", iteration, epoch, prevSparsity, adaptFromThreshold)
				End If
				Return adaptFromThreshold
			End If

			If prevSparsity < minTargetSparsity Then
				'Sparsity ratio was too small (too sparse) - decrease threshold to increase number of values communicated
				Dim retThreshold As Double = decayRate * adaptFromThreshold
				Me.lastThreshold = retThreshold
				If log.isDebugEnabled() Then
					log.debug("AdaptiveThresholdAlgorithm: iter {} epoch {}: prev sparsity {} < min sparsity {}, reducing threshold from {} to  {}", iteration, epoch, prevSparsity, minTargetSparsity, adaptFromThreshold, retThreshold)
				End If
				Return retThreshold
			End If

			If prevSparsity > maxTargetSparsity Then
				'Sparsity ratio was too high (too dense) - increase threshold to decrease number of values communicated
				Dim retThreshold As Double = 1.0/decayRate * adaptFromThreshold
				Me.lastThreshold = retThreshold
				If log.isDebugEnabled() Then
					log.debug("AdaptiveThresholdAlgorithm: iter {} epoch {}: prev sparsity {} > max sparsity {}, increasing threshold from {} to  {}", iteration, epoch, prevSparsity, maxTargetSparsity, adaptFromThreshold, retThreshold)
				End If
				Return retThreshold
			End If

			Throw New System.InvalidOperationException("Invalid previous sparsity value: " & prevSparsity) 'Should never happen, unless NaN?
		End Function

		Public Overridable Function newReducer() As ThresholdAlgorithmReducer
			Return New Reducer(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate)
		End Function

		Public Overridable Function clone() As AdaptiveThresholdAlgorithm
			Dim ret As New AdaptiveThresholdAlgorithm(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate)
			ret.lastThreshold = lastThreshold
			ret.lastSparsity = lastSparsity
			Return ret
		End Function

		Public Overrides Function ToString() As String
			Dim s As String = "AdaptiveThresholdAlgorithm(initialThreshold=" & initialThreshold & ",minTargetSparsity=" & minTargetSparsity & ",maxTargetSparsity=" & maxTargetSparsity & ",decayRate=" & decayRate
			If Double.IsNaN(lastThreshold) Then
				Return s & ")"
			End If
			Return s & ",lastThreshold=" & lastThreshold & ")"
		End Function


		'Reducer stores last threshold between epoch instead of starting adaption from scratch for each epoch
		<Serializable>
		Private Class Reducer
			Implements ThresholdAlgorithmReducer

			Friend ReadOnly initialThreshold As Double
			Friend ReadOnly minTargetSparsity As Double
			Friend ReadOnly maxTargetSparsity As Double
			Friend ReadOnly decayRate As Double

			Friend lastThresholdSum As Double
			Friend lastSparsitySum As Double
			Friend count As Integer

			Friend Sub New(ByVal initialThreshold As Double, ByVal minTargetSparsity As Double, ByVal maxTargetSparsity As Double, ByVal decayRate As Double)
				Me.initialThreshold = initialThreshold
				Me.minTargetSparsity = minTargetSparsity
				Me.maxTargetSparsity = maxTargetSparsity
				Me.decayRate = decayRate
			End Sub

			Public Overridable Sub add(ByVal instance As ThresholdAlgorithm)
				Dim a As AdaptiveThresholdAlgorithm = DirectCast(instance, AdaptiveThresholdAlgorithm)
				If a Is Nothing OrElse Double.IsNaN(a.lastThreshold) Then
					Return
				End If


				lastThresholdSum += a.lastThreshold
				If Not Double.IsNaN(a.lastSparsity) Then
					lastSparsitySum += a.lastSparsity
				End If
				count += 1
			End Sub

			Public Overridable Function merge(ByVal other As ThresholdAlgorithmReducer) As ThresholdAlgorithmReducer
				Dim r As Reducer = DirectCast(other, Reducer)
				Me.lastThresholdSum += r.lastThresholdSum
				Me.lastSparsitySum += r.lastSparsitySum
				Me.count += r.count
				Return Me
			End Function

			Public Overridable ReadOnly Property FinalResult As ThresholdAlgorithm
				Get
					Dim ret As New AdaptiveThresholdAlgorithm(initialThreshold, minTargetSparsity, maxTargetSparsity, decayRate)
					If count > 0 Then
						ret.lastThreshold = lastThresholdSum / count
						ret.lastSparsity = lastSparsitySum / count
					End If
					Return ret
				End Get
			End Property
		End Class
	End Class

End Namespace