Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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
'ORIGINAL LINE: @Getter @EqualsAndHashCode public class DistributionStats implements NormalizerStats
	<Serializable>
	Public Class DistributionStats
		Implements NormalizerStats

		Private Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(NormalizerStandardize))

		Private ReadOnly mean As INDArray
		Private ReadOnly std As INDArray

		''' <param name="mean"> row vector of means </param>
		''' <param name="std">  row vector of standard deviations </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributionStats(@NonNull INDArray mean, @NonNull INDArray std)
		Public Sub New(ByVal mean As INDArray, ByVal std As INDArray)
			Transforms.max(std, Nd4j.EPS_THRESHOLD, False)
			' FIXME: obvious bug here
	'        if (std.min(1) == Nd4j.scalar(Nd4j.EPS_THRESHOLD)) {
	'            logger.info("API_INFO: Std deviation found to be zero. Transform will round up to epsilon to avoid nans.");
	'        }

			Me.mean = mean
			Me.std = std
		End Sub

		''' <summary>
		''' Load distribution statistics from the file system
		''' </summary>
		''' <param name="meanFile"> file containing the means </param>
		''' <param name="stdFile">  file containing the standard deviations </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static DistributionStats load(@NonNull File meanFile, @NonNull File stdFile) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function load(ByVal meanFile As File, ByVal stdFile As File) As DistributionStats
			Return New DistributionStats(Nd4j.readBinary(meanFile), Nd4j.readBinary(stdFile))
		End Function

		''' <summary>
		''' Save distribution statistics to the file system
		''' </summary>
		''' <param name="meanFile"> file to contain the means </param>
		''' <param name="stdFile">  file to contain the standard deviations </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void save(@NonNull File meanFile, @NonNull File stdFile) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub save(ByVal meanFile As File, ByVal stdFile As File)
			Nd4j.saveBinary(getMean(), meanFile)
			Nd4j.saveBinary(getStd(), stdFile)
		End Sub

		''' <summary>
		''' DynamicCustomOpsBuilder class that can incrementally update a running mean and variance in order to create statistics for a
		''' large set of data
		''' </summary>
		Public Class Builder
			Implements NormalizerStats.Builder(Of DistributionStats)

			Friend runningCount As Long = 0
			Friend runningMean As INDArray
			Friend runningVariance As INDArray

			''' <summary>
			''' Add the features of a DataSet to the statistics
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addFeatures(@NonNull org.nd4j.linalg.dataset.api.DataSet dataSet)
			Public Overridable Function addFeatures(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As Builder
				Return add(dataSet.Features, dataSet.FeaturesMaskArray)
			End Function

			''' <summary>
			''' Add the labels of a DataSet to the statistics
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addLabels(@NonNull org.nd4j.linalg.dataset.api.DataSet dataSet)
			Public Overridable Function addLabels(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As Builder
				Return add(dataSet.Labels, dataSet.LabelsMaskArray)
			End Function

			''' <summary>
			''' Add rows of data to the statistics
			''' </summary>
			''' <param name="data"> the matrix containing multiple rows of data to include </param>
			''' <param name="mask"> (optionally) the mask of the data, useful for e.g. time series </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder add(@NonNull INDArray data, org.nd4j.linalg.api.ndarray.INDArray mask)
			Public Overridable Function add(ByVal data As INDArray, ByVal mask As INDArray) As Builder
				data = DataSetUtil.tailor2d(data, mask)

				' Using https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Parallel_algorithm
				If data Is Nothing Then
					' Nothing to add. Either data is empty or completely masked. Just skip it, otherwise we will get
					' null pointer exceptions.
					Return Me
				End If
				Dim mean As INDArray = data.mean(0).reshape(1,data.size(1))
				Dim variance As INDArray = data.var(False, 0).reshape(1,data.size(1))
				Dim count As Long = data.size(0)

				If runningMean Is Nothing Then
					' First batch
					runningMean = mean
					runningVariance = variance
					runningCount = count

					If data.size(0) = 1 Then
						'Handle edge case: currently, reduction ops may return the same array
						'But we don't want to modify this array in-place later
						runningMean = runningMean.dup()
						runningVariance = runningVariance.dup()
					End If
				Else
					' Update running variance
					Dim deltaSquared As INDArray = Transforms.pow(mean.subRowVector(runningMean), 2)
					Dim mB As INDArray = variance.muli(count)
					runningVariance.muli(runningCount).addiRowVector(mB).addiRowVector(deltaSquared.muli(CSng(runningCount * count) / (runningCount + count))).divi(runningCount + count)

					' Update running count
					runningCount += count

					' Update running mean
					Dim xMinusMean As INDArray = data.subRowVector(runningMean)
					runningMean.addi(xMinusMean.sum(0).divi(runningCount))
				End If

				Return Me
			End Function

			''' <summary>
			''' Create a DistributionStats object from the data ingested so far. Can be used multiple times when updating
			''' online.
			''' </summary>
			Public Overridable Function build() As DistributionStats
				If runningMean Is Nothing Then
					Throw New Exception("No data was added, statistics cannot be determined")
				End If
				Return New DistributionStats(runningMean.dup(), Transforms.sqrt(runningVariance, True))
			End Function
		End Class
	End Class

End Namespace