Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) public abstract class AbstractMultiDataSetNormalizer<S extends org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats> extends AbstractNormalizer implements MultiDataNormalization
	<Serializable>
	Public MustInherit Class AbstractMultiDataSetNormalizer(Of S As org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats)
		Inherits AbstractNormalizer
		Implements MultiDataNormalization

		Public MustOverride Sub revertLabels(ByVal labels() As INDArray, ByVal labelsMask() As INDArray) Implements MultiDataNormalization.revertLabels
		Public MustOverride Sub revertFeatures(ByVal features() As INDArray) Implements MultiDataNormalization.revertFeatures
		Public MustOverride Sub revertFeatures(ByVal features() As INDArray, ByVal featuresMask() As INDArray) Implements MultiDataNormalization.revertFeatures
		Public MustOverride Sub fit(ByVal iterator As MultiDataSetIterator) Implements MultiDataNormalization.fit
		Protected Friend strategy As NormalizerStrategy(Of S)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private java.util.List<S> featureStats;
		Private featureStats As IList(Of S)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private java.util.List<S> labelStats;
		Private labelStats As IList(Of S)
		Private fitLabels As Boolean = False

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Protected Friend Sub New(ByVal strategy As NormalizerStrategy(Of S))
			Me.strategy = strategy
		End Sub

		''' <summary>
		''' Flag to specify if the labels/outputs in the dataset should be also normalized
		''' default value is false
		''' </summary>
		''' <param name="fitLabels"> </param>
		Public Overridable Sub fitLabel(ByVal fitLabels As Boolean)
			Me.fitLabels = fitLabels
		End Sub

		''' <summary>
		''' Whether normalization for the labels is also enabled. Most commonly used for regression, not classification.
		''' </summary>
		''' <returns> True if labels will be </returns>
		Public Overridable ReadOnly Property FitLabel As Boolean
			Get
				Return Me.fitLabels
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property Fit As Boolean
			Get
				Return featureStats IsNot Nothing
			End Get
		End Property

		Protected Friend Overridable Function getFeatureStats(ByVal input As Integer) As S
			Return getFeatureStats()(input)
		End Function

		Protected Friend Overridable ReadOnly Property FeatureStats As IList(Of S)
			Get
				Return featureStats
			End Get
		End Property

		Protected Friend Overridable Function getLabelStats(ByVal output As Integer) As S
			Return getLabelStats()(output)
		End Function

		Protected Friend Overridable ReadOnly Property LabelStats As IList(Of S)
			Get
				Return labelStats
			End Get
		End Property

		''' <summary>
		''' Fit a MultiDataSet (only compute based on the statistics from this <seealso cref="MultiDataSet"/>)
		''' </summary>
		''' <param name="dataSet"> the dataset to compute on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void fit(@NonNull MultiDataSet dataSet)
		Public Overridable Sub fit(ByVal dataSet As MultiDataSet)
			Dim featureNormBuilders As IList(Of S.Builder) = New List(Of S.Builder)()
			Dim labelNormBuilders As IList(Of S.Builder) = New List(Of S.Builder)()

			fitPartial(dataSet, featureNormBuilders, labelNormBuilders)

			featureStats = buildList(featureNormBuilders)
			If FitLabel Then
				labelStats = buildList(labelNormBuilders)
			End If
		End Sub

		''' <summary>
		''' Fit an iterator
		''' </summary>
		''' <param name="iterator"> for the data to iterate over </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void fit(@NonNull MultiDataSetIterator iterator)
		Public Overridable Sub fit(ByVal iterator As MultiDataSetIterator)
			Dim featureNormBuilders As IList(Of S.Builder) = New List(Of S.Builder)()
			Dim labelNormBuilders As IList(Of S.Builder) = New List(Of S.Builder)()

			iterator.reset()
			Do While iterator.hasNext()
				Dim [next] As MultiDataSet = iterator.next()
				fitPartial([next], featureNormBuilders, labelNormBuilders)
			Loop

			featureStats = buildList(featureNormBuilders)
			If FitLabel Then
				labelStats = buildList(labelNormBuilders)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private java.util.List<S> buildList(@NonNull List<S.Builder> builders)
		Private Function buildList(ByVal builders As IList(Of S.Builder)) As IList(Of S)
			Dim result As IList(Of S) = New List(Of S)(builders.size())
			For Each builder As S.Builder In builders
				result.Add(CType(builder.build(), S))
			Next builder
			Return result
		End Function

		Private Sub fitPartial(ByVal dataSet As MultiDataSet, ByVal featureStatsBuilders As IList(Of S.Builder), ByVal labelStatsBuilders As IList(Of S.Builder))
			Dim numInputs As Integer = dataSet.numFeatureArrays()
			Dim numOutputs As Integer = dataSet.numLabelsArrays()

			ensureStatsBuilders(featureStatsBuilders, numInputs)
			ensureStatsBuilders(labelStatsBuilders, numOutputs)

			For i As Integer = 0 To numInputs - 1
				featureStatsBuilders(i).add(dataSet.getFeatures(i), dataSet.getFeaturesMaskArray(i))
			Next i

			If FitLabel Then
				For i As Integer = 0 To numOutputs - 1
					labelStatsBuilders(i).add(dataSet.getLabels(i), dataSet.getLabelsMaskArray(i))
				Next i
			End If
		End Sub

		Private Sub ensureStatsBuilders(ByVal builders As IList(Of S.Builder), ByVal amount As Integer)
			If builders.Count = 0 Then
				For i As Integer = 0 To amount - 1
					builders.Add(newBuilder())
				Next i
			End If
		End Sub

		Protected Friend MustOverride Function newBuilder() As S.Builder


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void transform(@NonNull MultiDataSet toPreProcess)
		Public Overridable Sub transform(ByVal toPreProcess As MultiDataSet)
			preProcess(toPreProcess)
		End Sub

		''' <summary>
		''' Pre process a MultiDataSet
		''' </summary>
		''' <param name="toPreProcess"> the data set to pre process </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void preProcess(@NonNull MultiDataSet toPreProcess)
		Public Overridable Sub preProcess(ByVal toPreProcess As MultiDataSet)
			Dim numFeatures As Integer = toPreProcess.numFeatureArrays()
			Dim numLabels As Integer = toPreProcess.numLabelsArrays()

			For i As Integer = 0 To numFeatures - 1
				strategy.preProcess(toPreProcess.getFeatures(i), toPreProcess.getFeaturesMaskArray(i), getFeatureStats(i))
			Next i
			If FitLabel Then
				For i As Integer = 0 To numLabels - 1
					strategy.preProcess(toPreProcess.getLabels(i), toPreProcess.getLabelsMaskArray(i), getLabelStats(i))
				Next i
			End If
		End Sub

		''' <summary>
		''' Revert the data to what it was before transform
		''' </summary>
		''' <param name="data"> the dataset to revert back </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revert(@NonNull MultiDataSet data)
		Public Overridable Sub revert(ByVal data As MultiDataSet)
			revertFeatures(data.getFeatures(), data.getFeaturesMaskArrays())
			revertLabels(data.getLabels(), data.getLabelsMaskArrays())
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this normalizer to the features arrays
		''' </summary>
		''' <param name="features"> Features to revert the normalization on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertFeatures(@NonNull INDArray[] features)
		Public Overridable Sub revertFeatures(ByVal features() As INDArray)
			revertFeatures(features, Nothing)
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this normalizer to the features arrays
		''' </summary>
		''' <param name="features"> Features to revert the normalization on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertFeatures(@NonNull INDArray[] features, org.nd4j.linalg.api.ndarray.INDArray[] maskArrays)
		Public Overridable Sub revertFeatures(ByVal features() As INDArray, ByVal maskArrays() As INDArray)
			For i As Integer = 0 To features.Length - 1
				Dim mask As INDArray = (If(maskArrays Is Nothing, Nothing, maskArrays(i)))
				revertFeatures(features(i), mask, i)
			Next i
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this normalizer to a specific features array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="features"> features arrays to revert the normalization on </param>
		''' <param name="input">    the index of the array to revert </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertFeatures(@NonNull INDArray features, org.nd4j.linalg.api.ndarray.INDArray mask, int input)
		Public Overridable Sub revertFeatures(ByVal features As INDArray, ByVal mask As INDArray, ByVal input As Integer)
			strategy.revert(features, mask, getFeatureStats(input))
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels"> Labels array to revert the normalization on </param>
		Public Overridable Sub revertLabels(ByVal labels() As INDArray) Implements MultiDataNormalization.revertLabels
			revertLabels(labels, Nothing)
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this normalizer to the labels arrays.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels"> Labels arrays to revert the normalization on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertLabels(@NonNull INDArray[] labels, org.nd4j.linalg.api.ndarray.INDArray[] labelsMask)
		Public Overridable Sub revertLabels(ByVal labels() As INDArray, ByVal labelsMask() As INDArray)
			For i As Integer = 0 To labels.Length - 1
				Dim mask As INDArray = (If(labelsMask Is Nothing, Nothing, labelsMask(i)))
				revertLabels(labels(i), mask, i)
			Next i
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this normalizer to a specific labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels"> Labels arrays to revert the normalization on </param>
		''' <param name="output"> the index of the array to revert </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertLabels(@NonNull INDArray labels, org.nd4j.linalg.api.ndarray.INDArray mask, int output)
		Public Overridable Sub revertLabels(ByVal labels As INDArray, ByVal mask As INDArray, ByVal output As Integer)
			If FitLabel Then
				strategy.revert(labels, mask, getLabelStats(output))
			End If
		End Sub

		''' <summary>
		''' Get the number of inputs
		''' </summary>
		Public Overridable Function numInputs() As Integer
			Return getFeatureStats().Count
		End Function

		''' <summary>
		''' Get the number of outputs
		''' </summary>
		Public Overridable Function numOutputs() As Integer
			Return getLabelStats().Count
		End Function
	End Class

End Namespace