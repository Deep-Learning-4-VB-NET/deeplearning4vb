Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiStandardizeSerializerStrategy = org.nd4j.linalg.dataset.api.preprocessor.serializer.MultiStandardizeSerializerStrategy
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType
Imports DistributionStats = org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats

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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) public class MultiNormalizerStandardize extends AbstractMultiDataSetNormalizer<org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats>
	<Serializable>
	Public Class MultiNormalizerStandardize
		Inherits AbstractMultiDataSetNormalizer(Of DistributionStats)

		Public Sub New()
			MyBase.New(New StandardizeStrategy())
		End Sub

		Protected Friend Overrides Function newBuilder() As NormalizerStats.Builder
			Return New DistributionStats.Builder()
		End Function

		Public Overridable Function getFeatureMean(ByVal input As Integer) As INDArray
			Return getFeatureStats(input).getMean()
		End Function

		Public Overridable Function getLabelMean(ByVal output As Integer) As INDArray
			Return getLabelStats(output).getMean()
		End Function

		Public Overridable Function getFeatureStd(ByVal input As Integer) As INDArray
			Return getFeatureStats(input).getStd()
		End Function

		Public Overridable Function getLabelStd(ByVal output As Integer) As INDArray
			Return getLabelStats(output).getStd()
		End Function

		''' <summary>
		''' Load means and standard deviations from the file system
		''' </summary>
		''' <param name="featureFiles"> source files for features, requires 2 files per input, alternating mean and stddev files </param>
		''' <param name="labelFiles">   source files for labels, requires 2 files per output, alternating mean and stddev files </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void load(@NonNull List<java.io.File> featureFiles, @NonNull List<java.io.File> labelFiles) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub load(ByVal featureFiles As IList(Of File), ByVal labelFiles As IList(Of File))
			setFeatureStats(load(featureFiles))
			If FitLabel Then
				setLabelStats(load(labelFiles))
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.util.List<org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats> load(java.util.List<java.io.File> files) throws java.io.IOException
		Private Function load(ByVal files As IList(Of File)) As IList(Of DistributionStats)
			Dim stats As New List(Of DistributionStats)(files.Count \ 2)
			For i As Integer = 0 To (files.Count \ 2) - 1
				stats.Add(DistributionStats.load(files(i * 2), files(i * 2 + 1)))
			Next i
			Return stats
		End Function

		''' <param name="featureFiles"> target files for features, requires 2 files per input, alternating mean and stddev files </param>
		''' <param name="labelFiles">   target files for labels, requires 2 files per output, alternating mean and stddev files </param>
		''' @deprecated use <seealso cref="MultiStandardizeSerializerStrategy"/> instead
		''' <para>
		''' Save the current means and standard deviations to the file system 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void save(@NonNull List<java.io.File> featureFiles, @NonNull List<java.io.File> labelFiles) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub save(ByVal featureFiles As IList(Of File), ByVal labelFiles As IList(Of File))
			saveStats(getFeatureStats(), featureFiles)
			If FitLabel Then
				saveStats(getLabelStats(), labelFiles)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void saveStats(java.util.List<org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats> stats, java.util.List<java.io.File> files) throws java.io.IOException
		Private Sub saveStats(ByVal stats As IList(Of DistributionStats), ByVal files As IList(Of File))
			Dim requiredFiles As Integer = stats.Count * 2
			If requiredFiles <> files.Count Then
				Throw New Exception(String.Format("Need twice as many files as inputs / outputs ({0:D}), got {1:D}", requiredFiles, files.Count))
			End If

			Dim i As Integer = 0
			Do While i < stats.Count
				stats(i).save(files(i * 2), files(i * 2 + 1))
				i += 1
			Loop
		End Sub

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.MULTI_STANDARDIZE
		End Function
	End Class

End Namespace