Imports System
Imports System.Collections.Generic
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports UiConnectionInfo = org.deeplearning4j.core.ui.UiConnectionInfo
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.models.embeddings


	''' <summary>
	''' General weight lookup table
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Interface WeightLookupTable(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)

		''' <summary>
		''' Returns unique ID of this table.
		''' Used for JointStorage/DistributedLookupTable mechanics
		''' </summary>
		''' <returns> ID of this table </returns>
		Property TableId As Long?


		''' <summary>
		''' The layer size for the lookup table </summary>
		''' <returns> the layer size for the lookup table </returns>
		Function layerSize() As Integer

		''' <summary>
		''' Returns gradient for specified word </summary>
		''' <param name="column"> </param>
		''' <param name="gradient">
		''' @return </param>
		Function getGradient(ByVal column As Integer, ByVal gradient As Double) As Double

		''' <summary>
		''' Clear out all weights regardless </summary>
		''' <param name="reset"> </param>
		Sub resetWeights(ByVal reset As Boolean)



		''' 
		''' <param name="codeIndex"> </param>
		''' <param name="code"> </param>
		Sub putCode(ByVal codeIndex As Integer, ByVal code As INDArray)

		''' <summary>
		''' Loads the co-occurrences for the given codes </summary>
		''' <param name="codes"> the codes to load </param>
		''' <returns> an ndarray of code.length by layerSize </returns>
		Function loadCodes(ByVal codes() As Integer) As INDArray

		''' <summary>
		''' Iterate on the given 2 vocab words </summary>
		''' <param name="w1"> the first word to iterate on </param>
		''' <param name="w2"> the second word to iterate on </param>
		<Obsolete>
		Sub iterate(ByVal w1 As T, ByVal w2 As T)

		''' <summary>
		''' Iterate on the given 2 vocab words </summary>
		''' <param name="w1"> the first word to iterate on </param>
		''' <param name="w2"> the second word to iterate on </param>
		''' <param name="nextRandom"> nextRandom for sampling </param>
		''' <param name="alpha"> the alpha to use for learning </param>
		<Obsolete>
		Sub iterateSample(ByVal w1 As T, ByVal w2 As T, ByVal nextRandom As AtomicLong, ByVal alpha As Double)


		''' <summary>
		''' Inserts a word vector </summary>
		''' <param name="word"> the word to insert </param>
		''' <param name="vector"> the vector to insert </param>
		Sub putVector(ByVal word As String, ByVal vector As INDArray)

		''' 
		''' <param name="word">
		''' @return </param>
		Function vector(ByVal word As String) As INDArray

		''' <summary>
		''' Reset the weights of the cache
		''' </summary>
		Sub resetWeights()


		''' <summary>
		''' Sets the learning rate </summary>
		''' <param name="lr"> </param>
		WriteOnly Property LearningRate As Double

		''' <summary>
		''' Iterates through all of the vectors in the cache </summary>
		''' <returns> an iterator for all vectors in the cache </returns>
		Function vectors() As IEnumerator(Of INDArray)

		ReadOnly Property Weights As INDArray

		''' <summary>
		''' Returns corresponding vocabulary
		''' </summary>
		ReadOnly Property VocabCache As VocabCache(Of T)
	End Interface

End Namespace