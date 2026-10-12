Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.optimize.api


	Public Interface TrainingListener

		''' <summary>
		''' Event listener for each iteration. Called once, after each parameter update has ocurred while training the network </summary>
		''' <param name="iteration"> the iteration </param>
		''' <param name="model"> the model iterating </param>
		Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)

		''' <summary>
		''' Called once at the start of each epoch, when using methods such as <seealso cref="org.deeplearning4j.nn.multilayer.MultiLayerNetwork.fit(DataSetIterator)"/>,
		''' <seealso cref="org.deeplearning4j.nn.graph.ComputationGraph.fit(DataSetIterator)"/> or <seealso cref="org.deeplearning4j.nn.graph.ComputationGraph.fit(MultiDataSetIterator)"/>
		''' </summary>
		Sub onEpochStart(ByVal model As Model)

		''' <summary>
		''' Called once at the end of each epoch, when using methods such as <seealso cref="org.deeplearning4j.nn.multilayer.MultiLayerNetwork.fit(DataSetIterator)"/>,
		''' <seealso cref="org.deeplearning4j.nn.graph.ComputationGraph.fit(DataSetIterator)"/> or <seealso cref="org.deeplearning4j.nn.graph.ComputationGraph.fit(MultiDataSetIterator)"/>
		''' </summary>
		Sub onEpochEnd(ByVal model As Model)

		''' <summary>
		''' Called once per iteration (forward pass) for activations (usually for a <seealso cref="org.deeplearning4j.nn.multilayer.MultiLayerNetwork"/>),
		''' only at training time
		''' </summary>
		''' <param name="model">       Model </param>
		''' <param name="activations"> Layer activations (including input) </param>
		Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray))

		''' <summary>
		''' Called once per iteration (forward pass) for activations (usually for a <seealso cref="org.deeplearning4j.nn.graph.ComputationGraph"/>),
		''' only at training time
		''' </summary>
		''' <param name="model">       Model </param>
		''' <param name="activations"> Layer activations (including input) </param>
		Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray))


		''' <summary>
		''' Called once per iteration (backward pass) <b>before the gradients are updated</b>
		''' Gradients are available via <seealso cref="Model.gradient()"/>.
		''' Note that gradients will likely be updated in-place - thus they should be copied or processed synchronously
		''' in this method.
		''' <para>
		''' For updates (gradients post learning rate/momentum/rmsprop etc) see <seealso cref="onBackwardPass(Model)"/>
		''' 
		''' </para>
		''' </summary>
		''' <param name="model"> Model </param>
		Sub onGradientCalculation(ByVal model As Model)

		''' <summary>
		''' Called once per iteration (backward pass) after gradients have been calculated, and updated
		''' Gradients are available via <seealso cref="Model.gradient()"/>.
		''' <para>
		''' Unlike <seealso cref="onGradientCalculation(Model)"/> the gradients at this point will be post-update, rather than
		''' raw (pre-update) gradients at that method call.
		''' 
		''' </para>
		''' </summary>
		''' <param name="model"> Model </param>
		Sub onBackwardPass(ByVal model As Model)

	End Interface

End Namespace