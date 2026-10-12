Imports System
Imports AccessLevel = lombok.AccessLevel
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) public abstract class AbstractDataSetNormalizer<S extends org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats> extends AbstractNormalizer implements DataNormalization
	<Serializable>
	Public MustInherit Class AbstractDataSetNormalizer(Of S As org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats)
		Inherits AbstractNormalizer
		Implements DataNormalization

		Protected Friend strategy As NormalizerStrategy(Of S)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.@PROTECTED) private S featureStats;
'JAVA TO VB CONVERTER NOTE: The field featureStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private featureStats_Conflict As S
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.@PROTECTED) private S labelStats;
'JAVA TO VB CONVERTER NOTE: The field labelStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labelStats_Conflict As S
		Private fitLabels As Boolean = False

		Protected Friend Sub New(ByVal strategy As NormalizerStrategy(Of S))
			Me.strategy = strategy
		End Sub

		''' <summary>
		''' Flag to specify if the labels/outputs in the dataset should be also normalized
		''' default value is false
		''' </summary>
		''' <param name="fitLabels"> </param>
		Public Overridable Sub fitLabel(ByVal fitLabels As Boolean) Implements DataNormalization.fitLabel
			Me.fitLabels = fitLabels
		End Sub


		''' <summary>
		''' Whether normalization for the labels is also enabled. Most commonly used for regression, not classification.
		''' </summary>
		''' <returns> True if labels will be </returns>
		Public Overridable ReadOnly Property FitLabel As Boolean Implements DataNormalization.isFitLabel
			Get
				Return Me.fitLabels
			End Get
		End Property

		''' <summary>
		''' Fit a dataset (only compute based on the statistics from this dataset) </summary>
		''' <param name="dataSet"> the dataset to compute on </param>
		Public Overridable Sub fit(ByVal dataSet As DataSet)
			featureStats_Conflict = CType(newBuilder().addFeatures(dataSet).build(), S)
			If FitLabel Then
				labelStats_Conflict = CType(newBuilder().addLabels(dataSet).build(), S)
			End If
		End Sub

		Protected Friend Overridable ReadOnly Property FeatureStats As S
			Get
				Return featureStats_Conflict
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property LabelStats As S
			Get
				Return labelStats_Conflict
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property Fit As Boolean
			Get
				Return featureStats_Conflict IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Fit the given model
		''' </summary>
		''' <param name="iterator"> for the data to iterate over </param>
		Public Overridable Sub fit(ByVal iterator As DataSetIterator) Implements DataNormalization.fit
			Dim featureNormBuilder As S.Builder = newBuilder()
			Dim labelNormBuilder As S.Builder = newBuilder()

			iterator.reset()
			Do While iterator.MoveNext()
				Dim [next] As DataSet = iterator.Current
				featureNormBuilder.addFeatures([next])
				If fitLabels Then
					labelNormBuilder.addLabels([next])
				End If
			Loop
			featureStats_Conflict = CType(featureNormBuilder.build(), S)
			If fitLabels Then
				labelStats_Conflict = CType(labelNormBuilder.build(), S)
			End If
			iterator.reset()
		End Sub

		Protected Friend MustOverride Function newBuilder() As S.Builder

		''' <summary>
		''' Pre process a dataset
		''' </summary>
		''' <param name="toPreProcess"> the data set to pre process </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void preProcess(@NonNull DataSet toPreProcess)
		Public Overridable Sub preProcess(ByVal toPreProcess As DataSet)
			transform(toPreProcess.getFeatures(), toPreProcess.getFeaturesMaskArray())
			transformLabel(toPreProcess.getLabels(), toPreProcess.getLabelsMaskArray())
		End Sub

		''' <summary>
		''' Transform the given dataset
		''' </summary>
		''' <param name="toPreProcess"> </param>
		Public Overridable Sub transform(ByVal toPreProcess As DataSet)
			preProcess(toPreProcess)
		End Sub

		''' <summary>
		''' Transform the given INDArray
		''' </summary>
		''' <param name="features"> </param>
		Public Overridable Sub transform(ByVal features As INDArray) Implements DataNormalization.transform
			transform(features, Nothing)
		End Sub

		Public Overridable Sub transform(ByVal features As INDArray, ByVal featuresMask As INDArray) Implements DataNormalization.transform
			Dim featureStatsLocal As S = FeatureStats

			If featureStatsLocal Is Nothing Then
				Throw New ND4JIllegalStateException("Features statistics were not yet calculated. Make sure to run fit() first.")
			End If

			strategy.preProcess(features, featuresMask, featureStatsLocal)
		End Sub

		''' <summary>
		''' Transform the labels. If <seealso cref="isFitLabel()"/> == false, this is a no-op
		''' </summary>
		Public Overridable Sub transformLabel(ByVal label As INDArray) Implements DataNormalization.transformLabel
			transformLabel(label, Nothing)
		End Sub

		Public Overridable Sub transformLabel(ByVal label As INDArray, ByVal labelsMask As INDArray) Implements DataNormalization.transformLabel
			If FitLabel Then
				strategy.preProcess(label, labelsMask, LabelStats)
			End If
		End Sub

		Public Overridable Sub revertFeatures(ByVal features As INDArray) Implements DataNormalization.revertFeatures
			revertFeatures(features, Nothing)
		End Sub


		Public Overridable Sub revertFeatures(ByVal features As INDArray, ByVal featuresMask As INDArray) Implements DataNormalization.revertFeatures
			strategy.revert(features, featuresMask, FeatureStats)
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels"> Labels array to revert the normalization on </param>
		Public Overridable Sub revertLabels(ByVal labels As INDArray) Implements DataNormalization.revertLabels
			revertLabels(labels, Nothing)
		End Sub

		Public Overridable Sub revertLabels(ByVal labels As INDArray, ByVal labelsMask As INDArray) Implements DataNormalization.revertLabels
			If FitLabel Then
				strategy.revert(labels, labelsMask, LabelStats)
			End If
		End Sub

		''' <summary>
		''' Revert the data to what it was before transform
		''' </summary>
		''' <param name="data"> the dataset to revert back </param>
		Public Overridable Sub revert(ByVal data As DataSet)
			revertFeatures(data.Features, data.FeaturesMaskArray)
			revertLabels(data.Labels, data.LabelsMaskArray)
		End Sub
	End Class

End Namespace