Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter @Setter @EqualsAndHashCode public class ImagePreProcessingScaler implements DataNormalization
	<Serializable>
	Public Class ImagePreProcessingScaler
		Implements DataNormalization

		Private minRange, maxRange As Double
		Private maxPixelVal As Double
		Private maxBits As Integer
		Private fitLabels As Boolean = False

		Public Sub New()
			Me.New(0, 1, 8)
		End Sub

		Public Sub New(ByVal a As Double, ByVal b As Double)
			Me.New(a, b, 8)
		End Sub

		''' <summary>
		''' Preprocessor can take a range as minRange and maxRange </summary>
		''' <param name="a">, default = 0 </param>
		''' <param name="b">, default = 1 </param>
		''' <param name="maxBits"> in the image, default = 8 </param>
		Public Sub New(ByVal a As Double, ByVal b As Double, ByVal maxBits As Integer)
			'Image values are not always from 0 to 255 though
			'some images are 16-bit, some 32-bit, integer, or float, and those BTW already come with values in [0..1]...
			'If the max expected value is 1, maxBits should be specified as 1
			maxPixelVal = Math.Pow(2, maxBits) - 1
			Me.minRange = a
			Me.maxRange = b
		End Sub

		''' <summary>
		''' Fit a dataset (only compute
		''' based on the statistics from this dataset0
		''' </summary>
		''' <param name="dataSet"> the dataset to compute on </param>
		Public Overridable Sub fit(ByVal dataSet As DataSet)

		End Sub

		''' <summary>
		''' Iterates over a dataset
		''' accumulating statistics for normalization
		''' </summary>
		''' <param name="iterator"> the iterator to use for
		'''                 collecting statistics. </param>
		Public Overridable Sub fit(ByVal iterator As DataSetIterator) Implements DataNormalization.fit

		End Sub

		Public Overridable Sub preProcess(ByVal toPreProcess As DataSet) Implements DataNormalization.preProcess
			Dim features As INDArray = toPreProcess.Features
			preProcess(features)
			If fitLabels AndAlso toPreProcess.Labels IsNot Nothing Then
				preProcess(toPreProcess.Labels)
			End If
		End Sub

		Public Overridable Sub preProcess(ByVal features As INDArray)
			features.divi(Me.maxPixelVal) 'Scaled to 0->1
			If Me.maxRange - Me.minRange <> 1 Then
				features.muli(Me.maxRange - Me.minRange) 'Scaled to minRange -> maxRange
			End If
			If Me.minRange <> 0 Then
				features.addi(Me.minRange) 'Offset by minRange
			End If
		End Sub

		''' <summary>
		''' Transform the data </summary>
		''' <param name="toPreProcess"> the dataset to transform </param>
		Public Overridable Sub transform(ByVal toPreProcess As DataSet)
			Me.preProcess(toPreProcess)
		End Sub

		Public Overridable Sub transform(ByVal features As INDArray) Implements DataNormalization.transform
			Me.preProcess(features)
		End Sub

		Public Overridable Sub transform(ByVal features As INDArray, ByVal featuresMask As INDArray) Implements DataNormalization.transform
			transform(features)
		End Sub

		Public Overridable Sub transformLabel(ByVal label As INDArray) Implements DataNormalization.transformLabel
			Preconditions.checkState(label IsNot Nothing AndAlso label.rank() = 4, "Labels can only be transformed for segmentation use" & " cases using this preprocesser - i.e., labels must be rank 4. Got: %ndShape", label)
			transform(label)
		End Sub

		Public Overridable Sub transformLabel(ByVal labels As INDArray, ByVal labelsMask As INDArray) Implements DataNormalization.transformLabel
			transformLabel(labels)
		End Sub

		Public Overridable Sub revert(ByVal toRevert As DataSet)
			revertFeatures(toRevert.Features)
			revertLabels(toRevert.Labels)
		End Sub

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.IMAGE_MIN_MAX
		End Function

		Public Overridable Sub revertFeatures(ByVal features As INDArray) Implements DataNormalization.revertFeatures
			If minRange <> 0 Then
				features.subi(minRange)
			End If
			If maxRange - minRange <> 1.0 Then
				features.divi(maxRange - minRange)
			End If
			features.muli(Me.maxPixelVal)
		End Sub

		Public Overridable Sub revertFeatures(ByVal features As INDArray, ByVal featuresMask As INDArray) Implements DataNormalization.revertFeatures
			revertFeatures(features)
		End Sub

		Public Overridable Sub revertLabels(ByVal labels As INDArray) Implements DataNormalization.revertLabels
			Preconditions.checkState(labels IsNot Nothing AndAlso labels.rank() = 4, "Labels can only be transformed for segmentation use" & " cases using this preprocesser - i.e., labels must be rank 4. Got: %ndShape", labels)
			revertFeatures(labels)
		End Sub

		Public Overridable Sub revertLabels(ByVal labels As INDArray, ByVal labelsMask As INDArray) Implements DataNormalization.revertLabels
			revertLabels(labels)
		End Sub

		Public Overridable Sub fitLabel(ByVal fitLabels As Boolean) Implements DataNormalization.fitLabel
			'No-op
			Me.fitLabels = fitLabels
		End Sub

		Public Overridable ReadOnly Property FitLabel As Boolean Implements DataNormalization.isFitLabel
			Get
				Return fitLabels
			End Get
		End Property
	End Class

End Namespace