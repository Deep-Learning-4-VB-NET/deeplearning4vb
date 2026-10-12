Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports org.nd4j.evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.nn.api

	''' <summary>
	''' @author raver119
	''' </summary>
	Public Interface NeuralNetwork

		''' <summary>
		''' This method does initialization of model
		''' 
		''' PLEASE NOTE: All implementations should track own state, to avoid double spending
		''' </summary>
		Sub init()

		''' <summary>
		''' This method returns model parameters as single INDArray
		''' 
		''' @return
		''' </summary>
		Function params() As INDArray

		''' <summary>
		''' This method returns updater state (if applicable), null otherwise
		''' @return
		''' </summary>
		Function updaterState() As INDArray

		''' <summary>
		''' This method returns Optimizer used for training
		''' 
		''' @return
		''' </summary>
		ReadOnly Property Optimizer As ConvexOptimizer

		''' <summary>
		''' This method fits model with a given DataSet
		''' </summary>
		''' <param name="dataSet"> </param>
		Sub fit(ByVal dataSet As DataSet)

		''' <summary>
		''' This method fits model with a given MultiDataSet
		''' </summary>
		''' <param name="dataSet"> </param>
		Sub fit(ByVal dataSet As MultiDataSet)

		''' <summary>
		''' This method fits model with a given DataSetIterator
		''' </summary>
		''' <param name="iterator"> </param>
		Sub fit(ByVal iterator As DataSetIterator)

		''' <summary>
		''' This method fits model with a given MultiDataSetIterator
		''' </summary>
		''' <param name="iterator"> </param>
		Sub fit(ByVal iterator As MultiDataSetIterator)

		''' <summary>
		''' This method executes evaluation of the model against given iterator and evaluation implementations
		''' </summary>
		''' <param name="iterator"> </param>
		 Function doEvaluation(Of T As IEvaluation)(ByVal iterator As DataSetIterator, ParamArray ByVal evaluations() As T) As T()

		''' <summary>
		''' This method executes evaluation of the model against given iterator and evaluation implementations
		''' </summary>
		''' <param name="iterator"> </param>
		 Function doEvaluation(Of T As IEvaluation)(ByVal iterator As MultiDataSetIterator, ParamArray ByVal evaluations() As T) As T()
	End Interface

End Namespace