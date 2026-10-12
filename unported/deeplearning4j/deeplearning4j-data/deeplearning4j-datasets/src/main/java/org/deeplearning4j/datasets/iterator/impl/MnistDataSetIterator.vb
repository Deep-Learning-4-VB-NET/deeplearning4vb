Imports System
Imports MnistDataFetcher = org.deeplearning4j.datasets.fetchers.MnistDataFetcher
Imports BaseDatasetIterator = org.nd4j.linalg.dataset.api.iterator.BaseDatasetIterator

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
	Public Class MnistDataSetIterator
		Inherits BaseDatasetIterator

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataSetIterator(int batch, int numExamples) throws java.io.IOException
		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer)
			Me.New(batch, numExamples, False)
		End Sub

		''' <summary>
		'''Get the specified number of examples for the MNIST training data set. </summary>
		''' <param name="batch"> the batch size of the examples </param>
		''' <param name="numExamples"> the overall number of examples </param>
		''' <param name="binarize"> whether to binarize mnist or not </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataSetIterator(int batch, int numExamples, boolean binarize) throws java.io.IOException
		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer, ByVal binarize As Boolean)
			Me.New(batch, numExamples, binarize, True, False, 0)
		End Sub

		''' <summary>
		''' Constructor to get the full MNIST data set (either test or train sets) without binarization (i.e., just normalization
		''' into range of 0 to 1), with shuffling based on a random seed. </summary>
		''' <param name="batchSize"> </param>
		''' <param name="train"> </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataSetIterator(int batchSize, boolean train, int seed) throws java.io.IOException
		Public Sub New(ByVal batchSize As Integer, ByVal train As Boolean, ByVal seed As Integer)
			Me.New(batchSize, (If(train, MnistDataFetcher.NUM_EXAMPLES, MnistDataFetcher.NUM_EXAMPLES_TEST)), False, train, True, seed)
		End Sub

		''' <summary>
		'''Get the specified number of MNIST examples (test or train set), with optional shuffling and binarization. </summary>
		''' <param name="batch"> Size of each patch </param>
		''' <param name="numExamples"> total number of examples to load </param>
		''' <param name="binarize"> whether to binarize the data or not (if false: normalize in range 0 to 1) </param>
		''' <param name="train"> Train vs. test set </param>
		''' <param name="shuffle"> whether to shuffle the examples </param>
		''' <param name="rngSeed"> random number generator seed to use when shuffling examples </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataSetIterator(int batch, int numExamples, boolean binarize, boolean train, boolean shuffle, long rngSeed) throws java.io.IOException
		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer, ByVal binarize As Boolean, ByVal train As Boolean, ByVal shuffle As Boolean, ByVal rngSeed As Long)
			MyBase.New(batch, numExamples, New MnistDataFetcher(binarize, train, shuffle, rngSeed, numExamples))
		End Sub

		Public Overridable Sub close()
			Dim mnistDataFetcher As MnistDataFetcher = DirectCast(fetcher, MnistDataFetcher)
			mnistDataFetcher.close()
		End Sub

	End Class

End Namespace