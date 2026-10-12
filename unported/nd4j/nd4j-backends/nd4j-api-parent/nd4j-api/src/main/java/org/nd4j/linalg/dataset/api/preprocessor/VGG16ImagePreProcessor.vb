Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
'ORIGINAL LINE: @Slf4j public class VGG16ImagePreProcessor implements DataNormalization
	<Serializable>
	Public Class VGG16ImagePreProcessor
		Implements DataNormalization

		Public Shared ReadOnly VGG_MEAN_OFFSET_BGR As INDArray = Nd4j.create(New Double() {103.939, 116.779, 123.68})
		Public Shared ReadOnly VGG_MEAN_OFFSET_BGR_FLOAT As INDArray = VGG_MEAN_OFFSET_BGR.castTo(DataType.FLOAT)
		Public Shared ReadOnly VGG_MEAN_OFFSET_BGR_FLOAT16 As INDArray = VGG_MEAN_OFFSET_BGR.castTo(DataType.HALF)

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
			Me.preProcess(features)
		End Sub

		Public Overridable Sub preProcess(ByVal features As INDArray)
			Dim mean As INDArray = getMeanFor(features)
			Nd4j.Executioner.execAndReturn(New BroadcastSubOp(features.dup(), mean, features, 1))
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
			'No op
		End Sub

		Public Overridable Sub transformLabel(ByVal labels As INDArray, ByVal labelsMask As INDArray) Implements DataNormalization.transformLabel
			transformLabel(labels)
		End Sub

		Public Overridable Sub revert(ByVal toRevert As DataSet)
			revertFeatures(toRevert.Features)
		End Sub

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.IMAGE_VGG16
		End Function

		Public Overridable Sub revertFeatures(ByVal features As INDArray) Implements DataNormalization.revertFeatures
			Dim mean As INDArray = getMeanFor(features)
			Nd4j.Executioner.execAndReturn(New BroadcastAddOp(features.dup(), mean, features, 1))
		End Sub

		Public Overridable Sub revertFeatures(ByVal features As INDArray, ByVal featuresMask As INDArray) Implements DataNormalization.revertFeatures
			revertFeatures(features)
		End Sub

		Public Overridable Sub revertLabels(ByVal labels As INDArray) Implements DataNormalization.revertLabels
			'No op
		End Sub

		Public Overridable Sub revertLabels(ByVal labels As INDArray, ByVal labelsMask As INDArray) Implements DataNormalization.revertLabels
			revertLabels(labels)
		End Sub

		Public Overridable Sub fitLabel(ByVal fitLabels As Boolean) Implements DataNormalization.fitLabel
			If fitLabels Then
				log.warn("Labels fitting not currently supported for ImagePreProcessingScaler. Labels will not be modified")
			End If
		End Sub

		Public Overridable ReadOnly Property FitLabel As Boolean Implements DataNormalization.isFitLabel
			Get
				Return False
			End Get
		End Property

		Protected Friend Shared Function getMeanFor(ByVal features As INDArray) As INDArray
			Select Case features.dataType()
				Case [DOUBLE]
					Return VGG_MEAN_OFFSET_BGR
				Case FLOAT
					Return VGG_MEAN_OFFSET_BGR_FLOAT
				Case HALF
					Return VGG_MEAN_OFFSET_BGR_FLOAT16
				Case Else
					Throw New System.NotSupportedException("Cannot preprocess features in non-floating point datatype: " & features.dataType())
			End Select
		End Function
	End Class

End Namespace