Imports System
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports DataSetType = org.deeplearning4j.datasets.fetchers.DataSetType
Imports UciSequenceDataFetcher = org.deeplearning4j.datasets.fetchers.UciSequenceDataFetcher
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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
	Public Class UciSequenceDataSetIterator
		Inherits SequenceRecordReaderDataSetIterator

		Protected Friend preProcessor As DataSetPreProcessor

		''' <summary>
		''' Create an iterator for the training set, with the specified minibatch size. Randomized with RNG seed 123
		''' </summary>
		''' <param name="batchSize"> Minibatch size </param>
		Public Sub New(ByVal batchSize As Integer)
			Me.New(batchSize, DataSetType.TRAIN, 123)
		End Sub

		''' <summary>
		''' Create an iterator for the training or test set, with the specified minibatch size. Randomized with RNG seed 123
		''' </summary>
		''' <param name="batchSize"> Minibatch size </param>
		''' <param name="set">       Set: training or test </param>
		Public Sub New(ByVal batchSize As Integer, ByVal set As DataSetType)
			Me.New(batchSize, set, 123)
		End Sub

		''' <summary>
		''' Create an iterator for the training or test set, with the specified minibatch size
		''' </summary>
		''' <param name="batchSize"> Minibatch size </param>
		''' <param name="set">       Set: training or test </param>
		''' <param name="rngSeed">   Random number generator seed to use for randomization </param>
		Public Sub New(ByVal batchSize As Integer, ByVal set As DataSetType, ByVal rngSeed As Long)
			MyBase.New((New UciSequenceDataFetcher()).getRecordReader(rngSeed, set), batchSize, UciSequenceDataFetcher.NUM_LABELS, 1)
			' last parameter is index of label
		End Sub
	End Class
End Namespace