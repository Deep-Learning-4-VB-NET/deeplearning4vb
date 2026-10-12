Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType

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

	Public Class ImageMultiPreProcessingScaler
		Implements MultiDataNormalization


		Private minRange, maxRange As Double
		Private maxPixelVal As Double
		Private featureIndices() As Integer

		Public Sub New(ParamArray ByVal featureIndices() As Integer)
			Me.New(0, 1, 8, featureIndices)
		End Sub

		Public Sub New(ByVal a As Double, ByVal b As Double, ByVal featureIndices() As Integer)
			Me.New(a, b, 8, featureIndices)
		End Sub

		''' <summary>
		''' Preprocessor can take a range as minRange and maxRange </summary>
		''' <param name="a">, default = 0 </param>
		''' <param name="b">, default = 1 </param>
		''' <param name="maxBits"> in the image, default = 8 </param>
		''' <param name="featureIndices"> Indices of feature arrays to process. If only one feature array is present,
		'''                       this should always be 0 </param>
		Public Sub New(ByVal a As Double, ByVal b As Double, ByVal maxBits As Integer, ByVal featureIndices() As Integer)
			If featureIndices Is Nothing OrElse featureIndices.Length = 0 Then
				Throw New System.ArgumentException("Invalid feature indices: the indices of the features arrays to apply " & "the normalizer to must be specified. MultiDataSet/MultiDataSetIterators with only a single feature" & " array, this should be set to 0. Otherwise specify the indexes of all the feature arrays to apply" & " the normalizer to.")
			End If
			'Image values are not always from 0 to 255 though
			'some images are 16-bit, some 32-bit, integer, or float, and those BTW already come with values in [0..1]...
			'If the max expected value is 1, maxBits should be specified as 1
			maxPixelVal = Math.Pow(2, maxBits) - 1
			Me.minRange = a
			Me.maxRange = b
			Me.featureIndices = featureIndices
		End Sub

		Public Overridable Sub fit(ByVal iterator As MultiDataSetIterator) Implements MultiDataNormalization.fit
			'No op
		End Sub

		Public Overridable Sub preProcess(ByVal multiDataSet As MultiDataSet) Implements MultiDataNormalization.preProcess
			For i As Integer = 0 To featureIndices.Length - 1
				Dim f As INDArray = multiDataSet.getFeatures(featureIndices(i))
				f.divi(Me.maxPixelVal) 'Scaled to 0->1
				If Me.maxRange - Me.minRange <> 1 Then
					f.muli(Me.maxRange - Me.minRange) 'Scaled to minRange -> maxRange
				End If
				If Me.minRange <> 0 Then
					f.addi(Me.minRange) 'Offset by minRange
				End If
			Next i
		End Sub

		Public Overridable Sub revertFeatures(ByVal features() As INDArray, ByVal featuresMask() As INDArray) Implements MultiDataNormalization.revertFeatures
			revertFeatures(features)
		End Sub

		Public Overridable Sub revertFeatures(ByVal features() As INDArray) Implements MultiDataNormalization.revertFeatures
			For i As Integer = 0 To featureIndices.Length - 1
				Dim f As INDArray = features(featureIndices(i))
				If minRange <> 0 Then
					f.subi(minRange)
				End If
				If maxRange - minRange <> 1.0 Then
					f.divi(maxRange - minRange)
				End If
				f.muli(Me.maxPixelVal)
			Next i
		End Sub

		Public Overridable Sub revertLabels(ByVal labels() As INDArray, ByVal labelsMask() As INDArray) Implements MultiDataNormalization.revertLabels
			'No op
		End Sub

		Public Overridable Sub revertLabels(ByVal labels() As INDArray) Implements MultiDataNormalization.revertLabels
			'No op
		End Sub

		Public Overridable Sub fit(ByVal dataSet As MultiDataSet)
			'No op
		End Sub

		Public Overridable Sub transform(ByVal toPreProcess As MultiDataSet)
			preProcess(toPreProcess)
		End Sub

		Public Overridable Sub revert(ByVal toRevert As MultiDataSet)
			revertFeatures(toRevert.Features, toRevert.FeaturesMaskArrays)
		End Sub

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.IMAGE_MIN_MAX
		End Function
	End Class

End Namespace