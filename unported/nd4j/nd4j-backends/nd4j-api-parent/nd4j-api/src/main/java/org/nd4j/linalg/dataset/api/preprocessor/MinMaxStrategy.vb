Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports MinMaxStats = org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.dataset.api.preprocessor


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @EqualsAndHashCode public class MinMaxStrategy implements NormalizerStrategy<org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats>, java.io.Serializable
	<Serializable>
	Public Class MinMaxStrategy
		Implements NormalizerStrategy(Of MinMaxStats)

		Private minRange As Double
		Private maxRange As Double

		Public Sub New()
			Me.New(0, 1)
		End Sub

		''' <param name="minRange"> the target range lower bound </param>
		''' <param name="maxRange"> the target range upper bound </param>
		Public Sub New(ByVal minRange As Double, ByVal maxRange As Double)
			Me.minRange = minRange
			Me.maxRange = Math.Max(maxRange, minRange + Nd4j.EPS_THRESHOLD)
		End Sub

		''' <summary>
		''' Normalize a data array
		''' </summary>
		''' <param name="array"> the data to normalize </param>
		''' <param name="stats"> statistics of the data population </param>
		Public Overridable Sub preProcess(ByVal array As INDArray, ByVal maskArray As INDArray, ByVal stats As MinMaxStats)
			If array.rank() <= 2 Then
				array.subiRowVector(stats.getLower().castTo(array.dataType()))
				array.diviRowVector(stats.Range.castTo(array.dataType()))
			' if feature Rank is 3 (time series) samplesxfeaturesxtimesteps
			' if feature Rank is 4 (images) samplesxchannelsxrowsxcols
			' both cases operations should be carried out in dimension 1
			Else
				Nd4j.Executioner.execAndReturn(New BroadcastSubOp(array, stats.getLower().castTo(array.dataType()), array, 1))
				Nd4j.Executioner.execAndReturn(New BroadcastDivOp(array, stats.Range.castTo(array.dataType()), array, 1))
			End If

			' Scale by target range
			array.muli(maxRange - minRange)
			' Add target range minimum values
			array.addi(minRange)

			If maskArray IsNot Nothing Then
				DataSetUtil.setMaskedValuesToZero(array, maskArray)
			End If
		End Sub

		''' <summary>
		''' Denormalize a data array
		''' </summary>
		''' <param name="array"> the data to denormalize </param>
		''' <param name="stats"> statistics of the data population </param>
		Public Overridable Sub revert(ByVal array As INDArray, ByVal maskArray As INDArray, ByVal stats As MinMaxStats)
			' Subtract target range minimum value
			array.subi(minRange)
			' Scale by target range
			array.divi(maxRange - minRange)

			If array.rank() <= 2 Then
				array.muliRowVector(stats.Range)
				array.addiRowVector(stats.getLower())
			Else
				Nd4j.Executioner.execAndReturn(New BroadcastMulOp(array, stats.Range.castTo(array.dataType()), array, 1))
				Nd4j.Executioner.execAndReturn(New BroadcastAddOp(array, stats.getLower().castTo(array.dataType()), array, 1))
			End If

			If maskArray IsNot Nothing Then
				DataSetUtil.setMaskedValuesToZero(array, maskArray)
			End If
		End Sub

		''' <summary>
		''' Create a new <seealso cref="NormalizerStats.Builder"/> instance that can be used to fit new data and of the opType that
		''' belongs to the current NormalizerStrategy implementation
		''' </summary>
		''' <returns> the new builder </returns>
		Public Overridable Function newStatsBuilder() As NormalizerStats.Builder
			Return New MinMaxStats.Builder()
		End Function
	End Class

End Namespace