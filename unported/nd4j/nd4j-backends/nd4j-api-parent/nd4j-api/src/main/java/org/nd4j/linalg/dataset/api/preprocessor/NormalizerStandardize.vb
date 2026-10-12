Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NormalizerSerializer = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerSerializer
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) public class NormalizerStandardize extends AbstractDataSetNormalizer<org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats>
	<Serializable>
	Public Class NormalizerStandardize
		Inherits AbstractDataSetNormalizer(Of DistributionStats)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NormalizerStandardize(@NonNull INDArray featureMean, @NonNull INDArray featureStd)
		Public Sub New(ByVal featureMean As INDArray, ByVal featureStd As INDArray)
			Me.New()
			setFeatureStats(New DistributionStats(featureMean, featureStd))
			fitLabel(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NormalizerStandardize(@NonNull INDArray featureMean, @NonNull INDArray featureStd, @NonNull INDArray labelMean, @NonNull INDArray labelStd)
		Public Sub New(ByVal featureMean As INDArray, ByVal featureStd As INDArray, ByVal labelMean As INDArray, ByVal labelStd As INDArray)
			Me.New()
			setFeatureStats(New DistributionStats(featureMean, featureStd))
			setLabelStats(New DistributionStats(labelMean, labelStd))
			fitLabel(True)
		End Sub

		Public Sub New()
			MyBase.New(New StandardizeStrategy())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setLabelStats(@NonNull INDArray labelMean, @NonNull INDArray labelStd)
		Public Overridable Sub setLabelStats(ByVal labelMean As INDArray, ByVal labelStd As INDArray)
			setLabelStats(New DistributionStats(labelMean, labelStd))
		End Sub

		Public Overridable ReadOnly Property Mean As INDArray
			Get
				Return FeatureStats.getMean()
			End Get
		End Property

		Public Overridable ReadOnly Property LabelMean As INDArray
			Get
				Return LabelStats.getMean()
			End Get
		End Property

		Public Overridable ReadOnly Property Std As INDArray
			Get
				Return FeatureStats.getStd()
			End Get
		End Property

		Public Overridable ReadOnly Property LabelStd As INDArray
			Get
				Return LabelStats.getStd()
			End Get
		End Property

		''' <summary>
		''' Load the means and standard deviations from the file system
		''' </summary>
		''' <param name="files"> the files to load from. Needs 4 files if normalizing labels, otherwise 2. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void load(java.io.File... files) throws java.io.IOException
		Public Overridable Sub load(ParamArray ByVal files() As File)
			setFeatureStats(DistributionStats.load(files(0), files(1)))
			If FitLabel Then
				setLabelStats(DistributionStats.load(files(2), files(3)))
			End If
		End Sub

		''' <param name="files"> the files to save to. Needs 4 files if normalizing labels, otherwise 2. </param>
		''' @deprecated use <seealso cref="NormalizerSerializer"/> instead
		''' <para>
		''' Save the current means and standard deviations to the file system 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.File... files) throws java.io.IOException
		Public Overridable Sub save(ParamArray ByVal files() As File)
			FeatureStats.save(files(0), files(1))
			If FitLabel Then
				LabelStats.save(files(2), files(3))
			End If
		End Sub

		Protected Friend Overrides Function newBuilder() As NormalizerStats.Builder
			Return New DistributionStats.Builder()
		End Function

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.STANDARDIZE
		End Function
	End Class

End Namespace