Imports System
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports LFWLoader = org.datavec.image.loader.LFWLoader
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator

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

Namespace org.deeplearning4j.datasets.iterator.impl



	<Serializable>
	Public Class LFWDataSetIterator
		Inherits RecordReaderDataSetIterator

		''' <summary>
		''' Loads subset of images with given imgDim returned by the generator. </summary>
		Public Sub New(ByVal imgDim() As Integer)
			Me.New(LFWLoader.SUB_NUM_IMAGES, LFWLoader.SUB_NUM_IMAGES, imgDim, LFWLoader.SUB_NUM_LABELS, False, New ParentPathLabelGenerator(), True, 1, Nothing, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, numExamples returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer)
			Me.New(batchSize, numExamples, New Integer() {LFWLoader.HEIGHT, LFWLoader.WIDTH, LFWLoader.CHANNELS}, LFWLoader.NUM_LABELS, False, LFWLoader.LABEL_PATTERN, True, 1, Nothing, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, numExamples, imgDim returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer, ByVal imgDim() As Integer)
			Me.New(batchSize, numExamples, imgDim, LFWLoader.NUM_LABELS, False, LFWLoader.LABEL_PATTERN, True, 1, Nothing, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, imgDim, useSubset, returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal imgDim() As Integer, ByVal useSubset As Boolean)
			Me.New(batchSize,If(useSubset, LFWLoader.SUB_NUM_IMAGES, LFWLoader.NUM_IMAGES), imgDim,If(useSubset, LFWLoader.SUB_NUM_LABELS, LFWLoader.NUM_LABELS), useSubset, LFWLoader.LABEL_PATTERN, True, 1, Nothing, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, numExamples, imgDim, train, & splitTrainTest returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer, ByVal imgDim() As Integer, ByVal train As Boolean, ByVal splitTrainTest As Double)
			Me.New(batchSize, numExamples, imgDim, LFWLoader.NUM_LABELS, False, LFWLoader.LABEL_PATTERN, train, splitTrainTest, Nothing, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, numExamples, numLabels, train, & splitTrainTest returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer, ByVal numLabels As Integer, ByVal train As Boolean, ByVal splitTrainTest As Double)
			Me.New(batchSize, numExamples, New Integer() {LFWLoader.HEIGHT, LFWLoader.WIDTH, LFWLoader.CHANNELS}, numLabels, False, Nothing, train, splitTrainTest, Nothing, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, numExamples, imgDim, numLabels, useSubset, train, splitTrainTest & Random returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer, ByVal imgDim() As Integer, ByVal numLabels As Integer, ByVal useSubset As Boolean, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random)
			Me.New(batchSize, numExamples, imgDim, numLabels, useSubset, LFWLoader.LABEL_PATTERN, train, splitTrainTest, Nothing, rng)
		End Sub

		''' <summary>
		''' Loads images with given  batchSize, numExamples, imgDim, numLabels, useSubset, train, splitTrainTest & Random returned by the generator. </summary>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer, ByVal imgDim() As Integer, ByVal numLabels As Integer, ByVal useSubset As Boolean, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random)
			Me.New(batchSize, numExamples, imgDim, numLabels, useSubset, labelGenerator, train, splitTrainTest, Nothing, rng)
		End Sub

		''' <summary>
		''' Create LFW data specific iterator </summary>
		''' <param name="batchSize"> the batch size of the examples </param>
		''' <param name="numExamples"> the overall number of examples </param>
		''' <param name="imgDim"> an array of height, width and channels </param>
		''' <param name="numLabels"> the overall number of examples </param>
		''' <param name="useSubset"> use a subset of the LFWDataSet </param>
		''' <param name="labelGenerator"> path label generator to use </param>
		''' <param name="train"> true if use train value </param>
		''' <param name="splitTrainTest"> the percentage to split data for train and remainder goes to test </param>
		''' <param name="imageTransform"> how to transform the image
		''' </param>
		''' <param name="rng"> random number to lock in batch shuffling
		'''  </param>
		Public Sub New(ByVal batchSize As Integer, ByVal numExamples As Integer, ByVal imgDim() As Integer, ByVal numLabels As Integer, ByVal useSubset As Boolean, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal imageTransform As ImageTransform, ByVal rng As Random)
			MyBase.New((New LFWLoader(imgDim, imageTransform, useSubset)).getRecordReader(batchSize, numExamples, imgDim, numLabels, labelGenerator, train, splitTrainTest, rng), batchSize, 1, numLabels)
		End Sub

	End Class

End Namespace