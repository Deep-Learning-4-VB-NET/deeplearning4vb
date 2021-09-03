Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.stats


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @Slf4j public class MinMaxStats implements NormalizerStats
	<Serializable>
	Public Class MinMaxStats
		Implements NormalizerStats

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.nd4j.linalg.api.ndarray.INDArray lower;
		Private ReadOnly lower As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.nd4j.linalg.api.ndarray.INDArray upper;
		Private ReadOnly upper As INDArray
'JAVA TO VB CONVERTER NOTE: The field range was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private range_Conflict As INDArray

		''' <param name="lower"> row vector of lower bounds </param>
		''' <param name="upper"> row vector of upper bounds </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MinMaxStats(@NonNull INDArray lower, @NonNull INDArray upper)
		Public Sub New(ByVal lower As INDArray, ByVal upper As INDArray)
			' Check for 0 differences and round up to epsilon
			Dim diff As INDArray = upper.sub(lower)
			Dim addedPadding As INDArray = Transforms.max(diff, Nd4j.EPS_THRESHOLD).subi(diff)
			' If any entry in `addedPadding` is not 0, then we had to add something to prevent 0 difference, Add this same
			' value to the upper bounds to actually apply the padding, and log about it
			If addedPadding.sumNumber().doubleValue() > 0 Then
				log.info("NormalizerMinMaxScaler: max val minus min val found to be zero. Transform will round up to epsilon to avoid nans.")
				upper.addi(addedPadding)
			End If

			Me.lower = lower
			Me.upper = upper
		End Sub

		''' <summary>
		''' Get the feature wise
		''' range for the statistics.
		''' Note that this is a lazy getter.
		''' It is only computed when needed. </summary>
		''' <returns> the feature wise range
		''' given the min and max </returns>
		Public Overridable ReadOnly Property Range As INDArray
			Get
				If range_Conflict Is Nothing Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						range_Conflict = upper.sub(lower)
					End Using
				End If
				Return range_Conflict
			End Get
		End Property

		''' <summary>
		''' DynamicCustomOpsBuilder class that can incrementally update a running lower and upper bound in order to create statistics for a
		''' large set of data
		''' </summary>
		Public Class Builder
			Implements NormalizerStats.Builder(Of MinMaxStats)

			Friend runningLower As INDArray
			Friend runningUpper As INDArray

			''' <summary>
			''' Add the features of a DataSet to the statistics
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MinMaxStats.Builder addFeatures(@NonNull org.nd4j.linalg.dataset.api.DataSet dataSet)
			Public Overridable Function addFeatures(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As MinMaxStats.Builder
				Return add(dataSet.Features, dataSet.FeaturesMaskArray)
			End Function

			''' <summary>
			''' Add the labels of a DataSet to the statistics
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MinMaxStats.Builder addLabels(@NonNull org.nd4j.linalg.dataset.api.DataSet dataSet)
			Public Overridable Function addLabels(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As MinMaxStats.Builder
				Return add(dataSet.Labels, dataSet.LabelsMaskArray)
			End Function

			''' <summary>
			''' Add rows of data to the statistics
			''' </summary>
			''' <param name="data"> the matrix containing multiple rows of data to include </param>
			''' <param name="mask"> (optionally) the mask of the data, useful for e.g. time series </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MinMaxStats.Builder add(@NonNull INDArray data, org.nd4j.linalg.api.ndarray.INDArray mask)
			Public Overridable Function add(ByVal data As INDArray, ByVal mask As INDArray) As MinMaxStats.Builder
				data = DataSetUtil.tailor2d(data, mask)
				If data Is Nothing Then
					' Nothing to add. Either data is empty or completely masked. Just skip it, otherwise we will get
					' null pointer exceptions.
					Return Me
				End If

				Dim batchMin As INDArray = data.min(0).reshape(1, data.size(1))
				Dim batchMax As INDArray = data.max(0).reshape(1, data.size(1))
				If Not batchMin.shape().SequenceEqual(batchMax.shape()) Then
					Throw New System.InvalidOperationException("Data min and max must be same shape. Likely a bug in the operation changing the input?")
				End If
				If runningLower Is Nothing Then
					' First batch
					' Create copies because min and max are views to the same data set, which will cause problems with the
					' side effects of Transforms.min and Transforms.max
					runningLower = batchMin.dup()
					runningUpper = batchMax.dup()
				Else
					' Update running bounds
					Transforms.min(runningLower, batchMin, False)
					Transforms.max(runningUpper, batchMax, False)
				End If

				Return Me
			End Function

			''' <summary>
			''' Create a DistributionStats object from the data ingested so far. Can be used multiple times when updating
			''' online.
			''' </summary>
			Public Overridable Function build() As MinMaxStats
				If runningLower Is Nothing Then
					Throw New Exception("No data was added, statistics cannot be determined")
				End If
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					Return New MinMaxStats(runningLower.dup(), runningUpper.dup())
				End Using
			End Function
		End Class
	End Class

End Namespace