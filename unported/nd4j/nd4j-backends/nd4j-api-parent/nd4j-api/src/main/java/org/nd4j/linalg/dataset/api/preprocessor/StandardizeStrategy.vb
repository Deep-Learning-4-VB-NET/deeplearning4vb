Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports DistributionStats = org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.dataset.api.preprocessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class StandardizeStrategy implements NormalizerStrategy<org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats>
	Public Class StandardizeStrategy
		Implements NormalizerStrategy(Of DistributionStats)

		Public Overridable Sub preProcess(ByVal array As INDArray, ByVal maskArray As INDArray, ByVal stats As DistributionStats)
			If array.rank() <= 2 Then
				array.subiRowVector(stats.getMean().castTo(array.dataType()))
				array.diviRowVector(filteredStd(stats).castTo(array.dataType()))
			' if array Rank is 3 (time series) samplesxfeaturesxtimesteps
			' if array Rank is 4 (images) samplesxchannelsxrowsxcols
			' both cases operations should be carried out in dimension 1
			Else
				Nd4j.Executioner.execAndReturn(New BroadcastSubOp(array, stats.getMean().castTo(array.dataType()), array, 1))
				Nd4j.Executioner.execAndReturn(New BroadcastDivOp(array, filteredStd(stats).castTo(array.dataType()), array, 1))
			End If

			If maskArray IsNot Nothing Then
				DataSetUtil.setMaskedValuesToZero(array, maskArray)
			End If
		End Sub

		''' <summary>
		''' Denormalize a data array
		''' </summary>
		''' <param name="array"> the data to denormalize </param>
		''' <param name="stats"> statistics of the data population </param>
		Public Overridable Sub revert(ByVal array As INDArray, ByVal maskArray As INDArray, ByVal stats As DistributionStats)
			If array.rank() <= 2 Then
				array.muliRowVector(filteredStd(stats))
				array.addiRowVector(stats.getMean())
			Else
				Nd4j.Executioner.execAndReturn(New BroadcastMulOp(array, filteredStd(stats).castTo(array.dataType()), array, 1))
				Nd4j.Executioner.execAndReturn(New BroadcastAddOp(array, stats.getMean().castTo(array.dataType()), array, 1))
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
			Return New DistributionStats.Builder()
		End Function

		Private Shared Function filteredStd(ByVal stats As DistributionStats) As INDArray
	'        
	'            To avoid division by zero when the std deviation is zero, replace zeros by one
	'         
			Dim stdCopy As INDArray = stats.getStd()
			BooleanIndexing.replaceWhere(stdCopy, 1.0, Conditions.equals(0))
			Return stdCopy
		End Function
	End Class

End Namespace