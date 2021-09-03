Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Setter public class MultiNormalizerHybrid extends AbstractNormalizer implements MultiDataNormalization, java.io.Serializable
	<Serializable>
	Public Class MultiNormalizerHybrid
		Inherits AbstractNormalizer
		Implements MultiDataNormalization

		Private inputStats As IDictionary(Of Integer, NormalizerStats)
		Private outputStats As IDictionary(Of Integer, NormalizerStats)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private NormalizerStrategy globalInputStrategy;
		Private globalInputStrategy As NormalizerStrategy
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private NormalizerStrategy globalOutputStrategy;
		Private globalOutputStrategy As NormalizerStrategy
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.Map<Integer, NormalizerStrategy> perInputStrategies = new java.util.HashMap<>();
		Private perInputStrategies As IDictionary(Of Integer, NormalizerStrategy) = New Dictionary(Of Integer, NormalizerStrategy)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.Map<Integer, NormalizerStrategy> perOutputStrategies = new java.util.HashMap<>();
		Private perOutputStrategies As IDictionary(Of Integer, NormalizerStrategy) = New Dictionary(Of Integer, NormalizerStrategy)()

		''' <summary>
		''' Apply standardization to all inputs, except the ones individually configured
		''' </summary>
		''' <returns> the normalizer </returns>
		Public Overridable Function standardizeAllInputs() As MultiNormalizerHybrid
			globalInputStrategy = New StandardizeStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to all inputs, except the ones individually configured
		''' </summary>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleAllInputs() As MultiNormalizerHybrid
			globalInputStrategy = New MinMaxStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to all inputs, except the ones individually configured
		''' </summary>
		''' <param name="rangeFrom"> lower bound of the target range </param>
		''' <param name="rangeTo">   upper bound of the target range </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleAllInputs(ByVal rangeFrom As Double, ByVal rangeTo As Double) As MultiNormalizerHybrid
			globalInputStrategy = New MinMaxStrategy(rangeFrom, rangeTo)
			Return Me
		End Function

		''' <summary>
		''' Apply standardization to a specific input, overriding the global input strategy if any
		''' </summary>
		''' <param name="input"> the index of the input </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function standardizeInput(ByVal input As Integer) As MultiNormalizerHybrid
			perInputStrategies(input) = New StandardizeStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to a specific input, overriding the global input strategy if any
		''' </summary>
		''' <param name="input"> the index of the input </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleInput(ByVal input As Integer) As MultiNormalizerHybrid
			perInputStrategies(input) = New MinMaxStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to a specific input, overriding the global input strategy if any
		''' </summary>
		''' <param name="input">     the index of the input </param>
		''' <param name="rangeFrom"> lower bound of the target range </param>
		''' <param name="rangeTo">   upper bound of the target range </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleInput(ByVal input As Integer, ByVal rangeFrom As Double, ByVal rangeTo As Double) As MultiNormalizerHybrid
			perInputStrategies(input) = New MinMaxStrategy(rangeFrom, rangeTo)
			Return Me
		End Function

		''' <summary>
		''' Apply standardization to all outputs, except the ones individually configured
		''' </summary>
		''' <returns> the normalizer </returns>
		Public Overridable Function standardizeAllOutputs() As MultiNormalizerHybrid
			globalOutputStrategy = New StandardizeStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to all outputs, except the ones individually configured
		''' </summary>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleAllOutputs() As MultiNormalizerHybrid
			globalOutputStrategy = New MinMaxStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to all outputs, except the ones individually configured
		''' </summary>
		''' <param name="rangeFrom"> lower bound of the target range </param>
		''' <param name="rangeTo">   upper bound of the target range </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleAllOutputs(ByVal rangeFrom As Double, ByVal rangeTo As Double) As MultiNormalizerHybrid
			globalOutputStrategy = New MinMaxStrategy(rangeFrom, rangeTo)
			Return Me
		End Function

		''' <summary>
		''' Apply standardization to a specific output, overriding the global output strategy if any
		''' </summary>
		''' <param name="output"> the index of the input </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function standardizeOutput(ByVal output As Integer) As MultiNormalizerHybrid
			perOutputStrategies(output) = New StandardizeStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to a specific output, overriding the global output strategy if any
		''' </summary>
		''' <param name="output"> the index of the input </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleOutput(ByVal output As Integer) As MultiNormalizerHybrid
			perOutputStrategies(output) = New MinMaxStrategy()
			Return Me
		End Function

		''' <summary>
		''' Apply min-max scaling to a specific output, overriding the global output strategy if any
		''' </summary>
		''' <param name="output">    the index of the input </param>
		''' <param name="rangeFrom"> lower bound of the target range </param>
		''' <param name="rangeTo">   upper bound of the target range </param>
		''' <returns> the normalizer </returns>
		Public Overridable Function minMaxScaleOutput(ByVal output As Integer, ByVal rangeFrom As Double, ByVal rangeTo As Double) As MultiNormalizerHybrid
			perOutputStrategies(output) = New MinMaxStrategy(rangeFrom, rangeTo)
			Return Me
		End Function

		''' <summary>
		''' Get normalization statistics for a given input.
		''' </summary>
		''' <param name="input"> the index of the input </param>
		''' <returns> implementation of NormalizerStats corresponding to the normalization strategy selected </returns>
		Public Overridable Function getInputStats(ByVal input As Integer) As NormalizerStats
			Return getInputStats()(input)
		End Function

		''' <summary>
		''' Get normalization statistics for a given output.
		''' </summary>
		''' <param name="output"> the index of the output </param>
		''' <returns> implementation of NormalizerStats corresponding to the normalization strategy selected </returns>
		Public Overridable Function getOutputStats(ByVal output As Integer) As NormalizerStats
			Return getOutputStats()(output)
		End Function

		''' <summary>
		''' Get the map of normalization statistics per input
		''' </summary>
		''' <returns> map of input indices pointing to NormalizerStats instances </returns>
		Public Overridable ReadOnly Property InputStats As IDictionary(Of Integer, NormalizerStats)
			Get
				assertIsFit()
				Return inputStats
			End Get
		End Property

		''' <summary>
		''' Get the map of normalization statistics per output
		''' </summary>
		''' <returns> map of output indices pointing to NormalizerStats instances </returns>
		Public Overridable ReadOnly Property OutputStats As IDictionary(Of Integer, NormalizerStats)
			Get
				assertIsFit()
				Return outputStats
			End Get
		End Property

		''' <summary>
		''' Fit a MultiDataSet (only compute based on the statistics from this dataset)
		''' </summary>
		''' <param name="dataSet"> the dataset to compute on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void fit(@NonNull MultiDataSet dataSet)
		Public Overridable Sub fit(ByVal dataSet As MultiDataSet)
			Dim inputStatsBuilders As IDictionary(Of Integer, NormalizerStats.Builder) = New Dictionary(Of Integer, NormalizerStats.Builder)()
			Dim outputStatsBuilders As IDictionary(Of Integer, NormalizerStats.Builder) = New Dictionary(Of Integer, NormalizerStats.Builder)()

			fitPartial(dataSet, inputStatsBuilders, outputStatsBuilders)

			inputStats = buildAllStats(inputStatsBuilders)
			outputStats = buildAllStats(outputStatsBuilders)
		End Sub

		''' <summary>
		''' Iterates over a dataset
		''' accumulating statistics for normalization
		''' </summary>
		''' <param name="iterator"> the iterator to use for collecting statistics </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void fit(@NonNull MultiDataSetIterator iterator)
		Public Overridable Sub fit(ByVal iterator As MultiDataSetIterator)
			Dim inputStatsBuilders As IDictionary(Of Integer, NormalizerStats.Builder) = New Dictionary(Of Integer, NormalizerStats.Builder)()
			Dim outputStatsBuilders As IDictionary(Of Integer, NormalizerStats.Builder) = New Dictionary(Of Integer, NormalizerStats.Builder)()

			iterator.reset()
			Do While iterator.hasNext()
				fitPartial(iterator.next(), inputStatsBuilders, outputStatsBuilders)
			Loop

			inputStats = buildAllStats(inputStatsBuilders)
			outputStats = buildAllStats(outputStatsBuilders)
		End Sub

		Private Sub fitPartial(ByVal dataSet As MultiDataSet, ByVal inputStatsBuilders As IDictionary(Of Integer, NormalizerStats.Builder), ByVal outputStatsBuilders As IDictionary(Of Integer, NormalizerStats.Builder))
			ensureStatsBuilders(inputStatsBuilders, globalInputStrategy, perInputStrategies, dataSet.numFeatureArrays())
			ensureStatsBuilders(outputStatsBuilders, globalOutputStrategy, perOutputStrategies, dataSet.numLabelsArrays())

			For Each index As Integer In inputStatsBuilders.Keys
				inputStatsBuilders(index).add(dataSet.getFeatures(index), dataSet.getFeaturesMaskArray(index))
			Next index
			For Each index As Integer In outputStatsBuilders.Keys
				outputStatsBuilders(index).add(dataSet.getLabels(index), dataSet.getLabelsMaskArray(index))
			Next index
		End Sub

		Private Sub ensureStatsBuilders(ByVal builders As IDictionary(Of Integer, NormalizerStats.Builder), ByVal globalStrategy As NormalizerStrategy, ByVal perArrayStrategies As IDictionary(Of Integer, NormalizerStrategy), ByVal numArrays As Integer)
			If builders.Count = 0 Then
				For i As Integer = 0 To numArrays - 1
					Dim strategy As NormalizerStrategy = getStrategy(globalStrategy, perArrayStrategies, i)
					If strategy IsNot Nothing Then
						builders(i) = strategy.newStatsBuilder()
					End If
				Next i
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private java.util.Map<Integer, org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats> buildAllStats(@NonNull Map<Integer, org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats.Builder> builders)
		Private Function buildAllStats(ByVal builders As IDictionary(Of Integer, NormalizerStats.Builder)) As IDictionary(Of Integer, NormalizerStats)
			Dim result As IDictionary(Of Integer, NormalizerStats) = New Dictionary(Of Integer, NormalizerStats)(builders.size())
			For Each index As Integer In builders.keySet()
				result(index) = builders.get(index).build()
			Next index
			Return result
		End Function

		''' <summary>
		''' Transform the dataset
		''' </summary>
		''' <param name="data"> the dataset to pre process </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void transform(@NonNull MultiDataSet data)
		Public Overridable Sub transform(ByVal data As MultiDataSet)
			preProcess(data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void preProcess(@NonNull MultiDataSet data)
		Public Overridable Sub preProcess(ByVal data As MultiDataSet)
			preProcess(data.getFeatures(), data.getFeaturesMaskArrays(), globalInputStrategy, perInputStrategies, getInputStats())
			preProcess(data.getLabels(), data.getLabelsMaskArrays(), globalOutputStrategy, perOutputStrategies, getOutputStats())
		End Sub

		Private Sub preProcess(ByVal arrays() As INDArray, ByVal masks() As INDArray, ByVal globalStrategy As NormalizerStrategy, ByVal perArrayStrategy As IDictionary(Of Integer, NormalizerStrategy), ByVal stats As IDictionary(Of Integer, NormalizerStats))

			If arrays IsNot Nothing Then
				For i As Integer = 0 To arrays.Length - 1
					Dim strategy As NormalizerStrategy = getStrategy(globalStrategy, perArrayStrategy, i)
					If strategy IsNot Nothing Then
						'noinspection unchecked
						strategy.preProcess(arrays(i),If(masks Is Nothing, Nothing, masks(i)), stats(i))
					End If
				Next i
			End If
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance (arrays are modified in-place)
		''' </summary>
		''' <param name="data"> MultiDataSet to revert the normalization on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void revert(@NonNull MultiDataSet data)
		Public Overridable Sub revert(ByVal data As MultiDataSet)
			revertFeatures(data.getFeatures(), data.getFeaturesMaskArrays())
			revertLabels(data.getLabels(), data.getLabelsMaskArrays())
		End Sub

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.MULTI_HYBRID
		End Function

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the entire inputs array
		''' </summary>
		''' <param name="features"> The normalized array of inputs </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void revertFeatures(@NonNull INDArray[] features)
		Public Overridable Sub revertFeatures(ByVal features() As INDArray)
			revertFeatures(features, Nothing)
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the entire inputs array
		''' </summary>
		''' <param name="features">   The normalized array of inputs </param>
		''' <param name="maskArrays"> Optional mask arrays belonging to the inputs </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void revertFeatures(@NonNull INDArray[] features, org.nd4j.linalg.api.ndarray.INDArray[] maskArrays)
		Public Overridable Sub revertFeatures(ByVal features() As INDArray, ByVal maskArrays() As INDArray)
			Dim i As Integer = 0
			Do While i < features.Length
				revertFeatures(features, maskArrays, i)
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the features of a particular input
		''' </summary>
		''' <param name="features">   The normalized array of inputs </param>
		''' <param name="maskArrays"> Optional mask arrays belonging to the inputs </param>
		''' <param name="input">      the index of the input to revert normalization on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertFeatures(@NonNull INDArray[] features, org.nd4j.linalg.api.ndarray.INDArray[] maskArrays, int input)
		Public Overridable Sub revertFeatures(ByVal features() As INDArray, ByVal maskArrays() As INDArray, ByVal input As Integer)
			Dim strategy As NormalizerStrategy = getStrategy(globalInputStrategy, perInputStrategies, input)
			If strategy IsNot Nothing Then
				Dim mask As INDArray = (If(maskArrays Is Nothing, Nothing, maskArrays(input)))
				'noinspection unchecked
				strategy.revert(features(input), mask, getInputStats(input))
			End If
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the entire outputs array
		''' </summary>
		''' <param name="labels"> The normalized array of outputs </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void revertLabels(@NonNull INDArray[] labels)
		Public Overridable Sub revertLabels(ByVal labels() As INDArray)
			revertLabels(labels, Nothing)
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the entire outputs array
		''' </summary>
		''' <param name="labels">     The normalized array of outputs </param>
		''' <param name="maskArrays"> Optional mask arrays belonging to the outputs </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void revertLabels(@NonNull INDArray[] labels, org.nd4j.linalg.api.ndarray.INDArray[] maskArrays)
		Public Overridable Sub revertLabels(ByVal labels() As INDArray, ByVal maskArrays() As INDArray)
			Dim i As Integer = 0
			Do While i < labels.Length
				revertLabels(labels, maskArrays, i)
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the labels of a particular output
		''' </summary>
		''' <param name="labels">     The normalized array of outputs </param>
		''' <param name="maskArrays"> Optional mask arrays belonging to the outputs </param>
		''' <param name="output">     the index of the output to revert normalization on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void revertLabels(@NonNull INDArray[] labels, org.nd4j.linalg.api.ndarray.INDArray[] maskArrays, int output)
		Public Overridable Sub revertLabels(ByVal labels() As INDArray, ByVal maskArrays() As INDArray, ByVal output As Integer)
			Dim strategy As NormalizerStrategy = getStrategy(globalOutputStrategy, perOutputStrategies, output)
			If strategy IsNot Nothing Then
				Dim mask As INDArray = (If(maskArrays Is Nothing, Nothing, maskArrays(output)))
				'noinspection unchecked
				strategy.revert(labels(output), mask, getOutputStats(output))
			End If
		End Sub

		Private Function getStrategy(ByVal globalStrategy As NormalizerStrategy, ByVal perArrayStrategy As IDictionary(Of Integer, NormalizerStrategy), ByVal index As Integer) As NormalizerStrategy
			Dim strategy As NormalizerStrategy = globalStrategy
			If perArrayStrategy.ContainsKey(index) Then
				strategy = perArrayStrategy(index)
			End If
			Return strategy
		End Function

		Protected Friend Overrides ReadOnly Property Fit As Boolean
			Get
				Return inputStats IsNot Nothing
			End Get
		End Property
	End Class

End Namespace