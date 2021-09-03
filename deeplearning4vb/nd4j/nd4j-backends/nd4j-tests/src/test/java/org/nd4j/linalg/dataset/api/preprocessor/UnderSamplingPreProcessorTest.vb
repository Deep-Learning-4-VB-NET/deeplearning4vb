Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NotThreadSafe = net.jcip.annotations.NotThreadSafe
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports UnderSamplingByMaskingMultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.preprocessor.classimbalance.UnderSamplingByMaskingMultiDataSetPreProcessor
Imports UnderSamplingByMaskingPreProcessor = org.nd4j.linalg.dataset.api.preprocessor.classimbalance.UnderSamplingByMaskingPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static Math.min
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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


	''' <summary>
	''' @author susaneraly
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NotThreadSafe @Tag(TagNames.NDARRAY_ETL) @NativeTag public class UnderSamplingPreProcessorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class UnderSamplingPreProcessorTest
		Inherits BaseNd4jTestWithBackends

		Friend shortSeq As Integer = 10000
		Friend longSeq As Integer = 20020 'not a perfect multiple of windowSize
		Friend window As Integer = 5000
		Friend minibatchSize As Integer = 3
		Friend targetDist As Double = 0.3
		Friend tolerancePerc As Double = 0.03 '10% +/- because this is not a very large sample


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void allMajority(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub allMajority(ByVal backend As Nd4jBackend)
			Dim someTargets() As Single = {0.01f, 0.1f, 0.5f}
			Dim d As DataSet = allMajorityDataSet(False)
			Dim dToPreProcess As DataSet
			For i As Integer = 0 To someTargets.Length - 1
				'if all majority default is to mask all time steps
				Dim preProcessor As New UnderSamplingByMaskingPreProcessor(someTargets(i), shortSeq \ 2)
				dToPreProcess = d.copy()
				preProcessor.preProcess(dToPreProcess)
				Dim exp As INDArray = Nd4j.zeros(dToPreProcess.LabelsMaskArray.shape())
				Dim lm As INDArray = dToPreProcess.LabelsMaskArray
				assertEquals(exp, lm)

				'change default and check distribution which should be 1-targetMinorityDist
				preProcessor.donotMaskAllMajorityWindows()
				dToPreProcess = d.copy()
				preProcessor.preProcess(dToPreProcess)
				Dim percentagesNow As INDArray = dToPreProcess.LabelsMaskArray.sum(1).div(shortSeq)
				assertTrue(Nd4j.valueArrayOf(percentagesNow.shape(), 1 - someTargets(i)).castTo(Nd4j.defaultFloatingPointType()).equalsWithEps(percentagesNow, tolerancePerc))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void allMinority(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub allMinority(ByVal backend As Nd4jBackend)
			Dim someTargets() As Single = {0.01f, 0.1f, 0.5f}
			Dim d As DataSet = allMinorityDataSet(False)
			Dim dToPreProcess As DataSet
			For i As Integer = 0 To someTargets.Length - 1
				Dim preProcessor As New UnderSamplingByMaskingPreProcessor(someTargets(i), shortSeq \ 2)
				dToPreProcess = d.copy()
				preProcessor.preProcess(dToPreProcess)
				'all minority classes present  - check that no time steps are masked
				assertEquals(Nd4j.ones(minibatchSize, shortSeq), dToPreProcess.LabelsMaskArray)

				'check behavior with override minority - now these are seen as all majority classes
				preProcessor.overrideMinorityDefault()
				preProcessor.donotMaskAllMajorityWindows()
				dToPreProcess = d.copy()
				preProcessor.preProcess(dToPreProcess)
				Dim percentagesNow As INDArray = dToPreProcess.LabelsMaskArray.sum(1).div(shortSeq)
				assertTrue(Nd4j.valueArrayOf(percentagesNow.shape(), 1 - someTargets(i)).castTo(Nd4j.defaultFloatingPointType()).equalsWithEps(percentagesNow,tolerancePerc))
			Next i
		End Sub

	'    
	'        Different distribution of labels within a minibatch, different time series length within a minibatch
	'        Checks distribution of classes after preprocessing
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void mixedDist(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub mixedDist(ByVal backend As Nd4jBackend)

			Dim preProcessor As New UnderSamplingByMaskingPreProcessor(targetDist, window)

			Dim dataSet As DataSet = knownDistVariedDataSet(New Single() {0.1f, 0.2f, 0.8f}, False)

			'Call preprocess for the same dataset multiple times to mimic calls with .next() and checks total distribution
			Dim [loop] As Integer = 2
			For i As Integer = 0 To [loop] - 1
				'preprocess dataset
				Dim dataSetToPreProcess As DataSet = dataSet.copy()
				Dim labelsBefore As INDArray = dataSetToPreProcess.Labels.dup()
				preProcessor.preProcess(dataSetToPreProcess)
				Dim labels As INDArray = dataSetToPreProcess.Labels
				assertEquals(labelsBefore, labels)

				'check masks are zero where there are no time steps
				Dim masks As INDArray = dataSetToPreProcess.LabelsMaskArray
				Dim shouldBeAllZeros As INDArray = masks.get(NDArrayIndex.interval(0, 3), NDArrayIndex.interval(shortSeq, longSeq))
				assertEquals(Nd4j.zeros(shouldBeAllZeros.shape()), shouldBeAllZeros)

				'check distribution of masks in window, going backwards from last time step
				For j As Integer = CInt(Math.Truncate(Math.Ceiling(CDbl(longSeq) / window))) To 1 Step -1
					'collect mask and labels
					Dim maxIndex As Integer = min(longSeq, j * window)
					Dim minIndex As Integer = min(0, maxIndex - window)
					Dim maskWindow As INDArray = masks.get(NDArrayIndex.all(), NDArrayIndex.interval(minIndex, maxIndex))
					Dim labelWindow As INDArray = labels.get(NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.interval(minIndex, maxIndex))

					'calc minority class distribution
					Dim minorityDist As INDArray = labelWindow.mul(maskWindow).sum(1).div(maskWindow.sum(1))

					If j < shortSeq \ window Then
						assertEquals(targetDist, minorityDist.getFloat(0), tolerancePerc,"Failed on window " & j & " batch 0, loop " & i) 'should now be close to target dist
						assertEquals(targetDist, minorityDist.getFloat(1), tolerancePerc,"Failed on window " & j & " batch 1, loop " & i) 'should now be close to target dist
						assertEquals(0.8, minorityDist.getFloat(2), tolerancePerc,"Failed on window " & j & " batch 2, loop " & i) 'should be unchanged as it was already above target dist
					End If
					assertEquals(targetDist, minorityDist.getFloat(3), tolerancePerc,"Failed on window " & j & " batch 3, loop " & i) 'should now be close to target dist
					assertEquals(targetDist, minorityDist.getFloat(4), tolerancePerc,"Failed on window " & j & " batch 4, loop " & i) 'should now be close to target dist
					assertEquals(0.8, minorityDist.getFloat(5), tolerancePerc,"Failed on window " & j & " batch 5, loop " & i) 'should be unchanged as it was already above target dist
				Next j
			Next i
		End Sub

	'    
	'        Same as above but with one hot vectors instead of label size = 1
	'        Also checks minority override
	'    
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void mixedDistOneHot(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub mixedDistOneHot(ByVal backend As Nd4jBackend)

			'preprocessor should give 30% minority class for every "window"
			Dim preProcessor As New UnderSamplingByMaskingPreProcessor(targetDist, window)
			preProcessor.overrideMinorityDefault()

			'construct a dataset with known distribution of minority class and varying time steps
			Dim dataSet As DataSet = knownDistVariedDataSet(New Single() {0.9f, 0.8f, 0.2f}, True)

			'Call preprocess for the same dataset multiple times to mimic calls with .next() and checks total distribution
			Dim [loop] As Integer = 10
			For i As Integer = 0 To [loop] - 1

				'preprocess dataset
				Dim dataSetToPreProcess As DataSet = dataSet.copy()
				preProcessor.preProcess(dataSetToPreProcess)
				Dim labels As INDArray = dataSetToPreProcess.Labels
				Dim masks As INDArray = dataSetToPreProcess.LabelsMaskArray

				'check masks are zero where there were no time steps
				Dim shouldBeAllZeros As INDArray = masks.get(NDArrayIndex.interval(0, 3), NDArrayIndex.interval(shortSeq, longSeq))
				assertEquals(Nd4j.zeros(shouldBeAllZeros.shape()), shouldBeAllZeros)

				'check distribution of masks in the window length, going backwards from last time step
				For j As Integer = CInt(Math.Truncate(Math.Ceiling(CDbl(longSeq) / window))) To 1 Step -1
					'collect mask and labels
					Dim maxIndex As Integer = min(longSeq, j * window)
					Dim minIndex As Integer = min(0, maxIndex - window)
					Dim maskWindow As INDArray = masks.get(NDArrayIndex.all(), NDArrayIndex.interval(minIndex, maxIndex))
					Dim labelWindow As INDArray = labels.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(minIndex, maxIndex))

					'calc minority class distribution after accounting for masks
					Dim minorityClass As INDArray = labelWindow.get(NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.all()).mul(maskWindow)
					Dim majorityClass As INDArray = labelWindow.get(NDArrayIndex.all(), NDArrayIndex.point(1), NDArrayIndex.all()).mul(maskWindow)
					Dim minorityDist As INDArray = minorityClass.sum(1).div(majorityClass.add(minorityClass).sum(1))

					If j < shortSeq \ window Then
						assertEquals(targetDist, minorityDist.getFloat(0), tolerancePerc,"Failed on window " & j & " batch 0, loop " & i) 'should now be close to target dist
						assertEquals(targetDist, minorityDist.getFloat(1), tolerancePerc,"Failed on window " & j & " batch 1, loop " & i) 'should now be close to target dist
						assertEquals(0.8, minorityDist.getFloat(2), tolerancePerc,"Failed on window " & j & " batch 2, loop " & i) 'should be unchanged as it was already above target dist
					End If
					assertEquals(targetDist, minorityDist.getFloat(3), tolerancePerc,"Failed on window " & j & " batch 3, loop " & i) 'should now be close to target dist
					assertEquals(targetDist, minorityDist.getFloat(4), tolerancePerc,"Failed on window " & j & " batch 4, loop " & i) 'should now be close to target dist
					assertEquals(0.8, minorityDist.getFloat(5), tolerancePerc,"Failed on window " & j & " batch 5, loop " & i) 'should be unchanged as it was already above target dist
				Next j
			Next i
		End Sub

		'all the tests above into one multidataset
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testForMultiDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testForMultiDataSet(ByVal backend As Nd4jBackend)
			Dim dataSetA As DataSet = knownDistVariedDataSet(New Single() {0.8f, 0.1f, 0.2f}, False)
			Dim dataSetB As DataSet = knownDistVariedDataSet(New Single() {0.2f, 0.9f, 0.8f}, True)

			Dim targetDists As New Dictionary(Of Integer, Double)()
			targetDists(0) = 0.5 'balance inputA
			targetDists(1) = 0.3 'inputB dist = 0.2%
			Dim maskingMultiDataSetPreProcessor As New UnderSamplingByMaskingMultiDataSetPreProcessor(targetDists, window)
			maskingMultiDataSetPreProcessor.overrideMinorityDefault(1)

			Dim multiDataSet As MultiDataSet = fromDataSet(dataSetA, dataSetB)
			maskingMultiDataSetPreProcessor.preProcess(multiDataSet)

			Dim labels As INDArray
			Dim minorityCount As INDArray
			Dim seqCount As INDArray
			Dim minorityDist As INDArray
			'datasetA
			labels = multiDataSet.getLabels(0).reshape(ChrW(minibatchSize * 2), longSeq).mul(multiDataSet.getLabelsMaskArray(0))
			minorityCount = labels.sum(1)
			seqCount = multiDataSet.getLabelsMaskArray(0).sum(1)
			minorityDist = minorityCount.div(seqCount)
			assertEquals(minorityDist.getDouble(1), 0.5, tolerancePerc)
			assertEquals(minorityDist.getDouble(2), 0.5, tolerancePerc)
			assertEquals(minorityDist.getDouble(4), 0.5, tolerancePerc)
			assertEquals(minorityDist.getDouble(5), 0.5, tolerancePerc)

			'datasetB - override is switched so grab index=0
			labels = multiDataSet.getLabels(1).get(NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.all()).mul(multiDataSet.getLabelsMaskArray(1))
			minorityCount = labels.sum(1)
			seqCount = multiDataSet.getLabelsMaskArray(1).sum(1)
			minorityDist = minorityCount.div(seqCount)
			assertEquals(minorityDist.getDouble(1), 0.3, tolerancePerc)
			assertEquals(minorityDist.getDouble(2), 0.3, tolerancePerc)
			assertEquals(minorityDist.getDouble(4), 0.3, tolerancePerc)
			assertEquals(minorityDist.getDouble(5), 0.3, tolerancePerc)

		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Public Overridable Function fromDataSet(ParamArray ByVal dataSets() As DataSet) As MultiDataSet
			Dim featureArr(dataSets.Length - 1) As INDArray
			Dim labelArr(dataSets.Length - 1) As INDArray
			Dim featureMaskArr(dataSets.Length - 1) As INDArray
			Dim labelMaskArr(dataSets.Length - 1) As INDArray
			For i As Integer = 0 To dataSets.Length - 1
				featureArr(i) = dataSets(i).Features
				labelArr(i) = dataSets(i).Labels
				featureMaskArr(i) = dataSets(i).FeaturesMaskArray
				labelMaskArr(i) = dataSets(i).LabelsMaskArray
			Next i
			Return New MultiDataSet(featureArr, labelArr, featureMaskArr, labelMaskArr)
		End Function

		Public Overridable Function allMinorityDataSet(ByVal twoClass As Boolean) As DataSet
			Return makeDataSetSameL(minibatchSize, shortSeq, New Single() {1.0f, 1.0f, 1.0f}, twoClass)
		End Function

		Public Overridable Function allMajorityDataSet(ByVal twoClass As Boolean) As DataSet
			Return makeDataSetSameL(minibatchSize, shortSeq, New Single() {0.0f, 0.0f, 0.0f}, twoClass)
		End Function

		Public Overridable Function knownDistVariedDataSet(ByVal dist() As Single, ByVal twoClass As Boolean) As DataSet
			'construct a dataset with known distribution of minority class and varying time steps
			Dim batchATimeSteps As DataSet = makeDataSetSameL(minibatchSize, shortSeq, dist, twoClass)
			Dim batchBTimeSteps As DataSet = makeDataSetSameL(minibatchSize, longSeq, dist, twoClass)
			Dim listofbatches As IList(Of DataSet) = New List(Of DataSet)()
			listofbatches.Add(batchATimeSteps)
			listofbatches.Add(batchBTimeSteps)
			Return DataSet.merge(listofbatches)
		End Function

	'    
	'        Make a random dataset with 0,1 distribution of classes specified
	'        Will return as a one-hot vector if twoClass = true
	'     
		Public Shared Function makeDataSetSameL(ByVal batchSize As Integer, ByVal timesteps As Integer, ByVal minorityDist() As Single, ByVal twoClass As Boolean) As DataSet
			Dim features As INDArray = Nd4j.rand(Nd4j.defaultFloatingPointType(), batchSize, 2, timesteps)
			Dim labels As INDArray
			If twoClass Then
				labels = Nd4j.zeros(Nd4j.defaultFloatingPointType(), batchSize, 2, timesteps)
			Else
				labels = Nd4j.zeros(Nd4j.defaultFloatingPointType(), batchSize, 1, timesteps)
			End If
			For i As Integer = 0 To batchSize - 1
				Dim l As INDArray
				If twoClass Then
					l = labels.get(NDArrayIndex.point(i), NDArrayIndex.point(1), NDArrayIndex.all())
					Nd4j.Executioner.exec(New BernoulliDistribution(l, minorityDist(i)))
					Dim lOther As INDArray = labels.get(NDArrayIndex.point(i), NDArrayIndex.point(0), NDArrayIndex.all())
					lOther.assign(l.rsub(1.0))
				Else
					l = labels.get(NDArrayIndex.point(i), NDArrayIndex.point(0), NDArrayIndex.all())
					Nd4j.Executioner.exec(New BernoulliDistribution(l, minorityDist(i)))
				End If
			Next i
			Return New DataSet(features, labels)
		End Function

	End Class

End Namespace